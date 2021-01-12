using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using Common;
using DataAccess;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Logic
{
    public class FS0402_Logic
    {
        FS0402_DataAccess fs0402_DataAccess = new FS0402_DataAccess();

        #region 按检索条件检索,返回dt
        public DataTable Search(string strYearMonth, string strDyState, string strHyState, string strPart_id)
        {
            return fs0402_DataAccess.Search(strYearMonth, strDyState, strHyState, strPart_id);
        }
        #endregion

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId, string strYearMonth)
        {
            fs0402_DataAccess.importSave(dt, strUserId, strYearMonth);
        }
        #endregion

        //#region 向SOQ导入履历中新增数据
        //public void importHistory(string strYearMonth, string strFileName, int iState, string strErrorUrl, string strUserId)
        //{
        //    fs0402_DataAccess.importHistory(strYearMonth, strFileName, iState, strErrorUrl, strUserId);
        //}
        //#endregion

        #region 承认。将合意后SOQ数据复制到合意SOQ，并改变合意状态，赋予合意时间
        public int Cr(string strYearMonth, string strDyState, string strHyState, string strPart_id)
        {
            return fs0402_DataAccess.Cr(strYearMonth, strDyState, strHyState, strPart_id);
        }
        #endregion

        #region 插入导入履历
        public void importHistory(string strYearMonth, List<string> errMessageList)
        {
            fs0402_DataAccess.importHistory(strYearMonth, errMessageList);
        }
        #endregion


        #region 导入
        /// <summary>
        /// 导入标准Excel，第一行为列头
        /// </summary>
        /// <param name="FileFullName">文件完整路径</param>
        /// <param name="sheetName">指定sheet页名称，如未匹配则默认选择第一个sheet</param>
        /// <param name="Header">5个数组,第一个为文件中列头，第二个为对应DataTable列头，第三个对应校验数据类型，第四个为最大长度限定（为0不校验），第五个为最小长度限定（为0不校验）</param>
        /// <param name="RetMsg">返回信息</param>
        /// <returns></returns>
        public DataTable ExcelToDataTable(string FileFullName, string sheetName, string[,] Header, ref string RetMsg)
        {
            FileStream fs = null;
            IWorkbook workbook = null;
            ISheet sheet = null;
            List<int> index = new List<int>();
            DataTable data = new DataTable();
            RetMsg = "";
            int startRow = 0;

            try
            {
                fs = new FileStream(FileFullName, FileMode.Open, FileAccess.Read);

                if (FileFullName.IndexOf(".xlsx") > 0 || FileFullName.IndexOf(".xlsm") > 0) // 2007版本
                    workbook = new XSSFWorkbook(fs);
                else if (FileFullName.IndexOf(".xls") > 0) // 2003版本
                    workbook = new HSSFWorkbook(fs);

                if (sheetName != null)
                {
                    sheet = workbook.GetSheet(sheetName);
                    if (sheet == null) //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                }
                else
                {
                    sheet = workbook.GetSheetAt(0);
                }

                if (sheet != null)
                {
                    IRow firstRow = sheet.GetRow(0);
                    int cellCount = firstRow.LastCellNum; //一行最后一个cell的编号 即总的列数

                    //对应索引
                    for (int i = 0; i < Header.GetLength(1); i++)
                    {
                        bool bFound = false;
                        for (int j = 0; j < cellCount; j++)
                        {
                            ICell cell = firstRow.GetCell(j);
                            string cellValue = cell.StringCellValue;
                            if (!string.IsNullOrEmpty(cellValue))
                            {
                                if (Header[0, i] == cellValue)
                                {
                                    bFound = true;
                                    index.Add(j);
                                    break;
                                }
                            }
                        }

                        if (bFound == false)
                        {
                            RetMsg = Header[0, i] + "列不存在";
                            return null;
                        }
                    }

                    //创建Datatable的列
                    for (int i = 0; i < Header.GetLength(1); i++)
                    {
                        data.Columns.Add(Header[1, i].ToString().Trim());
                    }

                    //获取数据首尾行
                    startRow = sheet.FirstRowNum + 1-1;//注意这减1是为了把表头取出来
                    int rowCount = sheet.LastRowNum;

                    //读取数据
                    for (int i = startRow; i <= rowCount; ++i)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue; //没有数据的行默认是null　　　　　　　

                        DataRow dataRow = data.NewRow();
                        for (int j = 0; j < Header.GetLength(1); j++)
                        {
                            ICell cell = row.GetCell(index[j]);
                            if (cell != null) //同理，没有数据的单元格都默认是null
                            {

                                if (cell.CellType == CellType.Numeric && DateUtil.IsCellDateFormatted(cell))
                                {
                                    dataRow[j] = DateTime.FromOADate(cell.NumericCellValue);
                                }
                                else
                                {
                                    dataRow[j] = cell.ToString();
                                }
                            }
                        }

                        data.Rows.Add(dataRow);
                    }
                }

                for (int i = 0; i < data.Columns.Count; i++)
                {
                    data.Columns[i].DataType = typeof(string);
                }

                #region 校验格式

                for (int i = 1; i < data.Rows.Count; i++)
                {
                    DataRow dr = data.Rows[i];
                    for (int j = 0; j < Header.GetLength(1); j++)
                    {
                        if (Convert.ToInt32(Header[3, j]) > 0 &&
                            dr[Header[1, j]].ToString().Length > Convert.ToInt32(Header[3, j]))
                        {
                            RetMsg = string.Format("第{0}行{1}大于设定长度", i + 2, Header[0, j]);
                            return null;
                        }

                        if (Convert.ToInt32(Header[4, j]) > 0 &&
                            dr[Header[1, j]].ToString().Length < Convert.ToInt32(Header[4, j]))
                        {
                            RetMsg = string.Format("第{0}行{1}小于设定长度", i + 2, Header[0, j]);
                            return null;
                        }

                        switch (Header[2, j])
                        {
                            case "decimal":
                                if (Convert.ToInt32(Header[4, j]) > 0 && !ComFunction.CheckDecimal(dr[Header[1, j]].ToString()))
                                {
                                    RetMsg = string.Format("第{0}行{1}不是合法数值", i + 2, Header[0, j]);
                                    return null;
                                }

                                break;
                            case "d":
                                if (Convert.ToInt32(Header[4, j]) > 0 && !ComFunction.CheckDate(dr[Header[1, j]].ToString()))
                                {
                                    RetMsg = string.Format("第{0}行{1}不是合法日期", i + 2, Header[0, j]);
                                    return null;
                                }

                                break;
                            case "ym":
                                if (Convert.ToInt32(Header[4, j]) > 0 && !ComFunction.CheckYearMonth(dr[Header[1, j]].ToString()))
                                {
                                    RetMsg = string.Format("第{0}行{1}不是合法日期", i + 2, Header[0, j]);
                                    return null;
                                }

                                break;
                            default:
                                if (Header[2, j].Length > 0 && Regex.Match(dr[Header[1, j]].ToString(), Header[2, j],
                                    RegexOptions.None).Success)
                                {
                                    RetMsg = string.Format("第{0}行{1}有非法字符", i + 2, Header[0, j]);
                                    return null;
                                }

                                break;
                        }
                    }
                }


                #endregion


                return data;
            }
            catch (Exception ex)
            {

                Console.WriteLine("Exception: " + ex.Message);
                return null;
            }
            finally
            {
                if (workbook != null)
                {
                    workbook.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
        }
        #endregion


        #region 导出带模板
        public string generateExcelWithXlt(DataTable dt, string[] field, string rootPath, string xltName, int startRow, string strUserId, string strFunctionName)
        {
            try
            {
                XSSFWorkbook hssfworkbook = new XSSFWorkbook();

                string XltPath = rootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Template" + Path.DirectorySeparatorChar + xltName;
                using (FileStream fs = File.OpenRead(XltPath))
                {
                    hssfworkbook = new XSSFWorkbook(fs);
                    fs.Close();
                }

                ISheet sheet = hssfworkbook.GetSheetAt(0);


                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row = sheet.CreateRow(startRow + i);
                    for (int j = 0; j < field.Length; j++)
                    {
                        ICell cell = row.CreateCell(j);
                        cell.SetCellValue(dt.Rows[i][field[j]].ToString());
                    }
                }
                sheet.GetRow(1).GetCell(1).SetCellValue("X月");
                sheet.GetRow(1).GetCell(2).SetCellValue("X月");
                sheet.GetRow(1).GetCell(3).SetCellValue("X月");
                //sheet.CreateRow(1).CreateCell(2).SetCellValue("X月");
                //sheet.GetRow(1).Cells[2].SetCellValue("X月");

                string strFileName = strFunctionName + "_导出信息_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + strUserId + ".xlsx";
                string fileSavePath = rootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Export" + Path.DirectorySeparatorChar;//文件临时目录，导入完成后 删除
                string path = fileSavePath + strFileName;
                using (FileStream fs = File.OpenWrite(path))
                {
                    hssfworkbook.Write(fs);//向打开的这个xls文件中写入数据  
                    fs.Close();
                }
                return strFileName;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        #endregion
    }
}
