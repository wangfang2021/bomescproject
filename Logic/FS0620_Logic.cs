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
    public class FS0620_Logic
    {
        FS0620_DataAccess fs0620_DataAccess;

        public FS0620_Logic()
        {
            fs0620_DataAccess = new FS0620_DataAccess();

        }

        public DataTable Search(string dOperatorTime,string vcTargetYear, string vcPartNo, string vcInjectionFactory, string vcInsideOutsideType, string vcSupplierId,string vcWorkArea, string vcType,string vcPackPlant,string vcReceiver,string vcEmailFlag)
        {
            return fs0620_DataAccess.Search(dOperatorTime,vcTargetYear, vcPartNo, vcInjectionFactory, vcInsideOutsideType, vcSupplierId, vcWorkArea, vcType, vcPackPlant, vcReceiver, vcEmailFlag);
        }

        public bool isExistAddData(DataTable dtadd)
        {
            return fs0620_DataAccess.isExistAddData(dtadd);
        }

        public bool isExistModData(DataTable dtamod)
        {
            return fs0620_DataAccess.isExistModData(dtamod);
        }

        public void importSave(DataTable importDt, string userId)
        {
            fs0620_DataAccess.importSave(importDt, userId);
        }

        public DataTable getEmail(string vcSupplier_id, string vcWorkArea)
        {
            return fs0620_DataAccess.getEmail(vcSupplier_id, vcWorkArea);
        }

        public DataTable getCCEmail(string code)
        {
            return fs0620_DataAccess.getCCEmail(code);
        }

        public DataTable getPlant(string vcTargetYear, string vcType)
        {
            return fs0620_DataAccess.getPlant(vcTargetYear, vcType);
        }

        public DataTable getDtByTargetYearAndPlant(string vcTargetYear, string plantCode, string vcType)
        {
            return fs0620_DataAccess.getDtByTargetYearAndPlant(vcTargetYear,plantCode, vcType);
        }

        public string CreateEmailBody(string date, string flag, string UnitCode, string UnitName)
        {
            StringBuilder sbr = new StringBuilder();
            if (flag == "0")
            {

                sbr.AppendLine("<p>各位供应商 大家好</p>");
                //sbr.AppendLine("<p><br></p>");
                sbr.AppendLine("<p>TFTM补给资材企管课" + UnitName + "<br>感谢大家一直以来对补给业务的协力！</p>");
                //sbr.AppendLine("<p>感谢大家一直以来对补给业务的协力！</p>");
                //sbr.AppendLine("<p><br></p>");
                sbr.AppendLine("<p>现送附"+date+ "年年计，具体内容请查看附！</p>");
                //sbr.AppendLine("<p><br></p>");
                sbr.AppendLine("<p>请厂家根据年计做好相应的工作安排。</p>");
                //sbr.AppendLine("<p><br></p>");
                sbr.AppendLine("<p>特记：年计仅供参考，具体请以实际订单为准。</p>");
                //sbr.AppendLine("<p><br></p>");
            }
            else if (flag == "1")
            {
                sbr.AppendLine("<p>各位供应商殿&nbsp;（请转发给贵司社内相关人员）</p>");
                sbr.AppendLine("<p>非常感谢一直以来对TFTM补给业务的支持！</p>");
                sbr.AppendLine("<p><br></p>");
                sbr.AppendLine("<p>关于标题一事，</p>");
                sbr.AppendLine("<p>本年度的年限调整工作开始展开。 </p>");
                sbr.AppendLine("<p>附件为本年度贵司的旧型年限制度联络单，请查收。</p>");
                sbr.AppendLine("<p>回复纳期：<u style=\"color: rgb(230, 0, 0);\">" + date + "</u>下班前</p><p><br></p><p>回答时，请添付填写完毕的帐票电子版以及</p>");
                sbr.AppendLine("<p>填写完毕并有贵司责任者签字承认的回答书扫描版（PDF）</p>");
                sbr.AppendLine("<p>另外：一括生产零件调达周期超过3个月（包含3个月）的，请进行标注并提示具体调达周期。</p><p><br></p>");
                sbr.AppendLine("<p>如有问题，请随时与我联络。</p><p><br></p>");
                sbr.AppendLine("<p>以上。</p><p><br></p>");
            }

            return sbr.ToString();
        }


        public DataTable getWaiZhuDt(string vcTargetYear, string vcType)
        {
            return fs0620_DataAccess.getWaiZhuDt(vcTargetYear, vcType);
        }
        public DataTable getHuiZongDt(string vcTargetYear, string vcType)
        {
            return fs0620_DataAccess.getHuiZongDt(vcTargetYear, vcType);
        }

        public DataTable GetPackPlant()
        {
            return fs0620_DataAccess.GetPackPlant();
        }

        public DataTable GetPlant()
        {
            return fs0620_DataAccess.GetPlant();
        }

        public DataTable GetSupplier()
        {
            return fs0620_DataAccess.GetSupplier();
        }

        public DataTable GetNeiWai()
        {
            return fs0620_DataAccess.GetNeiWai();
        }

        public DataTable GetWorkArea()
        {
            return fs0620_DataAccess.GetWorkArea();
        }

        public DataTable GetSupplierWorkArea()
        {
            return fs0620_DataAccess.GetSupplierWorkArea();
        }

        public void del(List<Dictionary<string, object>> listInfoData)
        {
            fs0620_DataAccess.Del(listInfoData);
        }
        public DataTable createTable(string strSpSub)
        {
            DataTable dataTable = new DataTable();
            if (strSpSub == "email")
            {
                dataTable.Columns.Add("vcSupplier_id");
                dataTable.Columns.Add("vcWorkArea");
                dataTable.Columns.Add("vcMessage");
            }
            if (strSpSub == "fs0620")
            {
                dataTable.Columns.Add("vcPartNo");
                dataTable.Columns.Add("vcMessage");
            }
            return dataTable;
        }

        public DataTable GetWorkAreaBySupplier(string vcSupplier_id)
        {
            return fs0620_DataAccess.GetWorkAreaBySupplier(vcSupplier_id);
        }

        public DataTable getTCode(string codeId)
        {
            return fs0620_DataAccess.getTCode(codeId);
        }

        public void updateEmailState(DataTable dtNewSupplierandWorkArea)
        {
            fs0620_DataAccess.updateEmailState(dtNewSupplierandWorkArea);
        }
        public DataTable ExcelToDataTableFfs0620(string FileFullName, string sheetName, string[,] Header, ref bool bReault, ref DataTable dataTable)
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
                            dataRow["vcPartNo"] = data.Rows[i]["vcPartNo"].ToString();
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
                            dataRow["vcPartNo"] = data.Rows[i]["vcPartNo"].ToString();
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
                                    dataRow["vcPartNo"] = data.Rows[i]["vcPartNo"].ToString();
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
                                    dataRow["vcPartNo"] = data.Rows[i]["vcPartNo"].ToString();
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
                                    dataRow["vcPartNo"] = data.Rows[i]["vcPartNo"].ToString();
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
                                    dataRow["vcPartNo"] = data.Rows[i]["vcPartNo"].ToString();
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
