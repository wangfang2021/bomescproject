using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Microsoft.Office.Interop.Excel;
using Common;
using DataAccess;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using DataTable = System.Data.DataTable;

namespace Logic
{
    public class FS0201_Logic
    {
        FS0201_DataAccess fs0201_DataAccess = new FS0201_DataAccess();

        #region 检索SPI

        public DataTable searchSPI(string vcSPINO, string vcPart_Id, string vcCarType, string vcState)
        {
            DataTable dt = fs0201_DataAccess.searchSPI(vcSPINO, vcPart_Id, vcCarType);
            if (dt.Rows.Count > 0)
            {

            }

            return dt;
        }

        #endregion

        public void AddSPI(string path, string userId)
        {
            try
            {
                string fileName = @"Result.xlsm";
                CreatePath(path);
                CopyFile(path, fileName);
                ChangePath(path, fileName);
                CreateList(path, fileName);
                string reMsg = "";
                string[,] header =
                {
                    {
                        "SPI No", "旧品番", "新品番", "補給区分（新）", "代替区分", "代替品番（新）", "品名", "品番実施時期(新/ｶﾗ)", "防錆区分", "防錆指示書№（新）",
                        "変更事項", "旧工程", "工程実施時期旧/ﾏﾃﾞ", "新工程", "工程実施時期新/ｶﾗ", "工程参照引当(直上品番)(新)", "処理日", "シート名", "ファイル名"
                    },
                    {
                        "vcSPINo", "vcPart_Id_old", "vcPart_Id_new", "vcBJDiff", "vcDTDiff", "vcPart_id_DT",
                        "vcPartName", "vcStartYearMonth", "vcFXDiff", "vcFXNo", "vcChange", "vcOldProj",
                        "vcOldProjTime", "vcNewProj", "vcNewProjTime", "vcCZYD", "dHandleTime", "vcSheetName",
                        "vcFileName"
                    },
                    {"", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
                    {"0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0"},
                    {"0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0"}
                };
                DataTable dt = new DataTable();
                dt = ExcelToDataTable(path + @"\" + fileName, "list", header, ref reMsg);
                if (dt.Rows.Count > 0)
                {
                    fs0201_DataAccess.addSPI(dt, userId);
                }
                else
                {
                    reMsg = "没有需要导入的数据。";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
        }




        #region 读取Excel
        public DataTable ExcelToDataTable(string FileFullName, string sheetName, string[,] Header, ref string RetMsg)
        {
            FileStream fs = null;
            IWorkbook workbook = null;
            ISheet sheet = null;
            List<int> index = new List<int>();
            DataTable data = new DataTable();

            RetMsg = "";
            int startRow = 4;

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
                    IRow firstRow = sheet.GetRow(4);
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
                    startRow = startRow + 1;
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
                                    dataRow[j] = cell.DateCellValue.ToString();
                                    ;
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
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }
        #endregion

        #region 创建路径
        public void CreatePath(string path)
        {
            try
            {
                if (Directory.GetFiles(path).Length == 0)
                {
                    DirectoryInfo di = new DirectoryInfo(path);
                    di.Create();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 复制模板文件
        public void CopyFile(string path, string fileName)
        {
            try
            {
                string pLocalFilePath = Directory.GetCurrentDirectory() + @"\Doc\Template\FS0201.xlsm";//要复制的文件路径
                string pSaveFilePath = path + @"\" + fileName;//指定存储的路径
                if (File.Exists(pLocalFilePath))//必须判断要复制的文件是否存在
                {
                    File.Copy(pLocalFilePath, pSaveFilePath, true);//三个参数分别是源文件路径，存储路径，若存储路径有相同文件是否替换
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 调用宏
        public void ChangePath(string path, string fileName)
        {
            Application app = new Application();
            app.Visible = false;
            string filePath = path + @"\" + fileName;
            Workbook wb = app.Workbooks.Open(filePath);
            try
            {
                object objRtn = new object();
                ComFunction.RunExcelMacro(app, "getPath", new Object[] { path }, out objRtn);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (wb != null)
                {
                    wb.Save();
                    Marshal.ReleaseComObject(wb);
                    app.Quit();
                    Marshal.ReleaseComObject(app);

                }
            }
        }
        public void CreateList(string path, string fileName)
        {
            Application app = new Application();
            app.Visible = false;
            string filePath = path + @"\" + fileName;

            Workbook wb = app.Workbooks.Open(filePath);
            try
            {
                object objRtn = new object();
                ComFunction.RunExcelMacro(app, "MakeList", new Object[] { }, out objRtn);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (wb != null)
                {
                    wb.Save();
                    Marshal.ReleaseComObject(wb);
                    app.Quit();
                    Marshal.ReleaseComObject(app);
                }
            }
        }

        #endregion

        #region 清空文件夹

        public void emptyFile(string path)
        {

        }

        #endregion
    }
}