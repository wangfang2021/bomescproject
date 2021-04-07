using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using Common;
using System.Text.RegularExpressions;

namespace Logic
{
    public class FS0108_Logic
    {
        FS0108_DataAccess fs0108_DataAccess;

        public FS0108_Logic()
        {
            fs0108_DataAccess = new FS0108_DataAccess();

        }

        public DataTable Search(string typeCode, string vcValue1, string vcValue2)
        {
            return fs0108_DataAccess.Search(typeCode, vcValue1, vcValue2);
        }
   
        public void Save(List<Dictionary<string, object>> listInfoData, string userId, ref string strErrorPartId)
        {
            fs0108_DataAccess.Save(listInfoData, userId, ref strErrorPartId);
        }

        public void Del(List<Dictionary<string, object>> listInfoData, string userId)
        {
            fs0108_DataAccess.Del(listInfoData, userId);
        }

        public DataTable CheckDistinctByTable(DataTable dtadd)
        {
            return fs0108_DataAccess.CheckDistinctByTable(dtadd);
        }

        public DataTable createTable(string strSpSub)
        {
            DataTable dataTable = new DataTable();
            if (strSpSub == "fs0108")
            {
                dataTable.Columns.Add("vcSupplier");
                dataTable.Columns.Add("vcWorkArea");
                dataTable.Columns.Add("vcFzgc");
                dataTable.Columns.Add("vcMessage");
            }
            return dataTable;
        }

        public DataTable GetSupplier()
        {
            return fs0108_DataAccess.GetSupplier();
        }

        public DataTable GetWorkArea()
        {
            return fs0108_DataAccess.GetWorkArea();
        }

        public DataTable GetWorkAreaBySupplier(string supplierCode)
        {
            return fs0108_DataAccess.GetWorkAreaBySupplier(supplierCode);
        }

        public DataTable checkData(string vcSupplier, string vcWorkArea, string vcStart, string vcEnd, string strInAutoIds)
        {
            return fs0108_DataAccess.checkData(vcSupplier, vcWorkArea, vcStart, vcEnd, strInAutoIds);
        }

        public DataTable ExcelToDataTableFfs0108(string FileFullName, string sheetName, string[,] Header, ref bool bReault, ref DataTable dataTable)
        {
            FileStream fs = null;
            IWorkbook workbook = null;
            ISheet sheet = null;
            List<int> index = new List<int>();
            DataTable data = new DataTable();
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
                            DataRow dataRow = dataTable.NewRow();
                            dataRow["vcPartNo"] = "";
                            dataRow["vcMessage"] = Header[0, i] + "列不存在";
                            dataTable.Rows.Add(dataRow);
                            bReault = false;
                            //RetMsg = Header[0, i] + "列不存在";
                            //return null;
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

                            DataRow dataRow = dataTable.NewRow();
                            dataRow["vcSupplier"] = data.Rows[i]["vcValue1"].ToString();
                            dataRow["vcWorkArea"] = data.Rows[i]["vcValue2"].ToString();
                            dataRow["vcFzgc"] = data.Rows[i]["vcValue5"].ToString();
                            dataRow["vcMessage"] = string.Format("第{0}行{1}大于设定长度", i + 2, Header[0, j]);
                            dataTable.Rows.Add(dataRow);
                            bReault = false;
                            //RetMsg = string.Format("第{0}行{1}大于设定长度", i + 2, Header[0, j]);
                            //return null;
                        }

                        if (Convert.ToInt32(Header[4, j]) > 0 &&
                            dr[Header[1, j]].ToString().Length < Convert.ToInt32(Header[4, j]))
                        {
                            DataRow dataRow = dataTable.NewRow();
                            dataRow["vcSupplier"] = data.Rows[i]["vcValue1"].ToString();
                            dataRow["vcWorkArea"] = data.Rows[i]["vcValue2"].ToString();
                            dataRow["vcFzgc"] = data.Rows[i]["vcValue5"].ToString();
                            dataRow["vcMessage"] = string.Format("第{0}行{1}小于设定长度", i + 2, Header[0, j]);
                            dataTable.Rows.Add(dataRow);
                            bReault = false;
                            //RetMsg = string.Format("第{0}行{1}小于设定长度", i + 2, Header[0, j]);
                            //return null;
                        }

                        switch (Header[2, j])
                        {
                            case "decimal":
                                if (Convert.ToInt32(Header[4, j]) > 0 && !ComFunction.CheckDecimal(dr[Header[1, j]].ToString()))
                                {
                                    DataRow dataRow = dataTable.NewRow();
                                    dataRow["vcSupplier"] = data.Rows[i]["vcValue1"].ToString();
                                    dataRow["vcWorkArea"] = data.Rows[i]["vcValue2"].ToString();
                                    dataRow["vcFzgc"] = data.Rows[i]["vcValue5"].ToString();
                                    dataRow["vcMessage"] = string.Format("第{0}行{1}不是合法数值", i + 2, Header[0, j]);
                                    dataTable.Rows.Add(dataRow);
                                    bReault = false;
                                    //RetMsg = string.Format("第{0}行{1}不是合法数值", i + 2, Header[0, j]);
                                    //return null;
                                }

                                break;
                            case "d":
                                if (Convert.ToInt32(Header[4, j]) > 0 && !ComFunction.CheckDate(dr[Header[1, j]].ToString()))
                                {
                                    DataRow dataRow = dataTable.NewRow();
                                    dataRow["vcSupplier"] = data.Rows[i]["vcValue1"].ToString();
                                    dataRow["vcWorkArea"] = data.Rows[i]["vcValue2"].ToString();
                                    dataRow["vcFzgc"] = data.Rows[i]["vcValue5"].ToString();
                                    dataRow["vcMessage"] = string.Format("第{0}行{1}不是合法日期", i + 2, Header[0, j]);
                                    dataTable.Rows.Add(dataRow);
                                    bReault = false;
                                    //RetMsg = string.Format("第{0}行{1}不是合法日期", i + 2, Header[0, j]);
                                    //return null;
                                }

                                break;
                            case "ym":
                                if (Convert.ToInt32(Header[4, j]) > 0 && !ComFunction.CheckYearMonth(dr[Header[1, j]].ToString()))
                                {
                                    DataRow dataRow = dataTable.NewRow();
                                    dataRow["vcSupplier"] = data.Rows[i]["vcValue1"].ToString();
                                    dataRow["vcWorkArea"] = data.Rows[i]["vcValue2"].ToString();
                                    dataRow["vcFzgc"] = data.Rows[i]["vcValue5"].ToString();
                                    dataRow["vcMessage"] = string.Format("第{0}行{1}不是合法日期", i + 2, Header[0, j]);
                                    dataTable.Rows.Add(dataRow);
                                    bReault = false;
                                    //RetMsg = string.Format("第{0}行{1}不是合法日期", i + 2, Header[0, j]);
                                    //return null;
                                }

                                break;
                            default:
                                if (Header[2, j].Length > 0 && Regex.Match(dr[Header[1, j]].ToString(), Header[2, j],
                                    RegexOptions.None).Success)
                                {
                                    DataRow dataRow = dataTable.NewRow();
                                    dataRow["vcSupplier"] = data.Rows[i]["vcValue1"].ToString();
                                    dataRow["vcWorkArea"] = data.Rows[i]["vcValue2"].ToString();
                                    dataRow["vcFzgc"] = data.Rows[i]["vcValue5"].ToString();
                                    dataRow["vcMessage"] = string.Format("第{0}行{1}有非法字符", i + 2, Header[0, j]);
                                    dataTable.Rows.Add(dataRow);
                                    bReault = false;
                                    //RetMsg = string.Format("第{0}行{1}有非法字符", i + 2, Header[0, j]);
                                    //return null;
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

        public void importSave(DataTable importDt, string userId)
        {
            fs0108_DataAccess.importSave(importDt,userId);
        }
    }
}
