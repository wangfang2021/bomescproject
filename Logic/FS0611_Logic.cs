using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using Common;
using DataAccess;
using DataEntity;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Logic
{
    public class FS0611_Logic
    {
        FS0611_DataAccess fs0611_DataAccess = new FS0611_DataAccess();

        #region 取日历
        public DataTable GetCalendar(string strPlant, string vcDXYM)
        {
            return fs0611_DataAccess.GetCalendar(strPlant, vcDXYM);
        }
        #endregion

        #region 取soq数据
        public DataTable GetSoq(string strPlant, string strYearMonth, string strType)
        {
            return fs0611_DataAccess.GetSoq(strPlant, strYearMonth, strType);
        }
        #endregion


        #region 取soq数据-已合意
        public DataTable GetSoqHy(string strPlant, string strYearMonth, string strType)
        {
            return fs0611_DataAccess.GetSoqHy(strPlant, strYearMonth, strType);
        }
        #endregion
        

        #region 取特殊厂家对应的品番
        public DataTable GetSpecialSupplier(string strPlant, string strDXYearMonth, string strYearMonth)
        {
            return fs0611_DataAccess.GetSpecialSupplier(strPlant, strDXYearMonth, strYearMonth);
        }
        #endregion


        #region 取特殊品番
        public DataTable GetSpecialPartId(string strPlant, string strDXYearMonth, string strYearMonth)
        {
            return fs0611_DataAccess.GetSpecialPartId(strPlant, strDXYearMonth, strYearMonth);
        }
        #endregion

        #region 更新平准化结果
        public void SaveResult(string strCLYM, string strDXYM, string strNSYM, string strNNSYM, string strPlant,
            ArrayList arrResult_DXYM, ArrayList arrResult_NSYM, ArrayList arrResult_NNSYM, string strUserId,string strUnit)
        {
            fs0611_DataAccess.SaveResult(strCLYM, strDXYM, strNSYM, strNNSYM, strPlant,
             arrResult_DXYM, arrResult_NSYM, arrResult_NNSYM, strUserId,strUnit);
        }
        #endregion

        #region 获取没有展开的数据
        public DataTable getZhankaiData(bool isZhankai)
        {
            return fs0611_DataAccess.getZhankaiData(isZhankai);
        }
        #endregion

        #region 展开SOQReply
        public int zk( string userId)
        {
            return fs0611_DataAccess.zk(userId);
        }
        #endregion

        #region 下载SOQReply（检索内容）
        public DataTable search(string strYearMonth, string strYearMonth_2, string strYearMonth_3)
        {
            return fs0611_DataAccess.search(strYearMonth,strYearMonth_2,strYearMonth_3);
        }
        #endregion

        #region 导入后保存
        public void importSave(DataTable dt, string strYearMonth, string strUserId)
        {
            fs0611_DataAccess.importSave(dt, strYearMonth, strUserId);
        }
        #endregion


        #region 获取当月SOQ导出理论数量，用来导入验证用
        public DataTable getNowMonthNum()
        {
            return fs0611_DataAccess.getNowMonthNum();
        }
        #endregion

        #region 获取平准化加减天数
        public int getPingZhunAddSubDay()
        {
            DataTable dt=fs0611_DataAccess.getPingZhunAddSubDay();
            if (dt == null || dt.Rows.Count == 0)
                return 0;
            return Convert.ToInt32(dt.Rows[0]["vcValue1"]);
        }
        #endregion

        /// <summary>
        /// 导入文件同时，验证44列名必须是N+2 PCS
        /// </summary>
        /// <param name="FileFullName"></param>
        /// <param name="sheetName"></param>
        /// <param name="Header"></param>
        /// <param name="RetMsg"></param>
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
                            if (i == 43 && j == 43 && cellValue != "N+2 PCS")
                            {
                                RetMsg = "模板列有调整，请用原始导出文件导入！";
                                return null;
                            }
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
                    startRow = sheet.FirstRowNum + 1;
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

                for (int i = 0; i < data.Rows.Count; i++)
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
                                if (dr[Header[1, j]] != null && dr[Header[1, j]].ToString() != "" && !ComFunction.CheckDecimal(dr[Header[1, j]].ToString()))
                                {
                                    RetMsg = string.Format("第{0}行{1}不是合法数值", i + 2, Header[0, j]);
                                    return null;
                                }

                                break;
                            case "d":
                                if (dr[Header[1, j]] != null && dr[Header[1, j]].ToString() != "" && !ComFunction.CheckDate(dr[Header[1, j]].ToString()))
                                {
                                    RetMsg = string.Format("第{0}行{1}不是合法日期", i + 2, Header[0, j]);
                                    return null;
                                }

                                break;
                            case "ym":
                                if (dr[Header[1, j]] != null && dr[Header[1, j]].ToString() != "" && !ComFunction.CheckYearMonth(dr[Header[1, j]].ToString()))
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
                throw ex;
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


    }
}
