using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using Common;
using System.Text.RegularExpressions;

namespace Logic
{
    public class FS0503_Logic
    {
        FS0503_DataAccess fs0503_DataAccess;

        public FS0503_Logic()
        {
            fs0503_DataAccess = new FS0503_DataAccess();

        }

        public DataTable Search(string vcSupplier_id, string vcWorkArea, string vcState,  string vcPartNo, string vcCarType, string dExpectDeliveryDate,string UserId,string  dSendDate,string dReplyDate)
        {
            return fs0503_DataAccess.Search(vcSupplier_id, vcWorkArea, vcState, vcPartNo, vcCarType, dExpectDeliveryDate, UserId, dSendDate, dReplyDate);
        }

        public DataTable GetBoxType()
        {
            return fs0503_DataAccess.GetBoxType();
        }
        public void Save(List<Dictionary<string, object>> listInfoData, string userId, ref string strErrorPartId)
        {
            fs0503_DataAccess.Save(listInfoData, userId, ref strErrorPartId);
        }
        public void Del(List<Dictionary<string, object>> listInfoData, string userId)
        {
            fs0503_DataAccess.Del(listInfoData, userId);
        }
        public void allInstall(List<Dictionary<string, object>> listInfoData,  string vcIntake, string userId)
        {
            fs0503_DataAccess.allInstall(listInfoData,  vcIntake, userId);
        }
        public void importSave(DataTable importDt, string userId)
        {
            fs0503_DataAccess.importSave(importDt, userId);
        }

        public DataTable GetTaskNum(string userId)
        {
            return fs0503_DataAccess.GetTaskNum(userId);
        }

        public void hZZK(List<Dictionary<string, object>> listInfoData, string dExpectDeliveryDate, string userId)
        {
            fs0503_DataAccess.hZZK(listInfoData, dExpectDeliveryDate, userId);
        }

        public DataTable GetTaskNum1(string userId)
        {
            return fs0503_DataAccess.GetTaskNum1(userId);
        }

        public DataTable GetCarType(string userId)
        {
            return fs0503_DataAccess.GetCarType(userId);
        }

        public DataTable GetExpectDeliveryDate(string userId)
        {
            return fs0503_DataAccess.GetExpectDeliveryDate(userId);
        }

        public DataTable GetReplyDate(string userId)
        {
            return fs0503_DataAccess.GetReplyDate(userId);
        }

        public DataTable GetSendDate(string userId)
        {
            return fs0503_DataAccess.GetSendDate(userId);
        }

        public DataTable GetWorkArea( string supplierId)
        {
            return fs0503_DataAccess.GetWorkArea(supplierId);
        }

        public void admit(List<Dictionary<string, object>> listInfoData, string userId)
        {
            fs0503_DataAccess.admit(listInfoData, userId);
        }
        public void returnHandle(List<Dictionary<string, object>> listInfoData, string userId)
        {
            fs0503_DataAccess.returnHandle(listInfoData, userId);
        }
        public void weaveHandle(List<Dictionary<string, object>> listInfoData, string userId)
        {
            fs0503_DataAccess.weaveHandle(listInfoData, userId);
        }

        public DataTable SearchByIAutoId(string strIAutoId)
        {
            return fs0503_DataAccess.SearchByIAutoId(strIAutoId); 
        }

        public bool editOk(dynamic dataForm, string userId)
        {
            return fs0503_DataAccess.editOk(dataForm, userId);
        }

        public void reply(List<Dictionary<string, object>> listInfoData, string userId)
        {
            fs0503_DataAccess.reply(listInfoData, userId);
        }

        public DataTable SearchTmp(string vcSupplier_id, string vcWorkArea, string vcState, string vcPartNo, string vcCarType, string dExpectDeliveryDate, string UserId, string dSendDate, string dReplyDate)
        {
            return fs0503_DataAccess.SearchTmp(vcSupplier_id, vcWorkArea, vcState, vcPartNo, vcCarType, dExpectDeliveryDate, UserId, dSendDate, dReplyDate);
        }

        public DataTable createTable(string strSpSub)
        {
            DataTable dataTable = new DataTable();
            if (strSpSub == "fs0503")
            {
                dataTable.Columns.Add("vcPartNo");
                dataTable.Columns.Add("vcMessage");
            }
            return dataTable;
        }

        public DataTable ExcelToDataTableFfs0503(string FileFullName, string sheetName, string[,] Header, ref bool bReault, ref DataTable dataTable)
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
                            dataRow["vcPartNo"] = "";
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
                            dataRow["vcPartNo"] = "";
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
                                    dataRow["vcPartNo"] = "";
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
                                    dataRow["vcPartNo"] = "";
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
                                    dataRow["vcPartNo"] = "";
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
                                    dataRow["vcPartNo"] = "";
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
    }
}
