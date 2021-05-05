using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Common;
using DataAccess;
using DataEntity;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Logic
{
    public class FS0602_Logic
    {
        private MultiExcute excute = new MultiExcute();
        FS0602_DataAccess fs0602_DataAccess = new FS0602_DataAccess();
        FS0603_DataAccess fs0603_DataAccess = new FS0603_DataAccess();
        FS0307_DataAccess fs0307_dataAccess = new FS0307_DataAccess();
        FS0501_DataAccess fs0501_DataAccess = new FS0501_DataAccess();
        FS0603_Logic fs0603_Logic = new FS0603_Logic();
        FS0625_Logic fs0625_Logic = new FS0625_Logic();

        public DataTable getSearchInfo(string strYearMonth, string strDyState, string strHyState, string strPartId, string strCarModel,
                  string strInOut, string strOrderingMethod, string strOrderPlant, string strHaoJiu, string strSupplierId, string strSupplierPlant, string strDataState, string strOverDue)
        {
            string strDyInfo = "";
            string strHyInfo = "";
            if (strDataState == "0")
            {
                strDyInfo = "'0','1','2','3'";//对应状态全部
                strHyInfo = "'0','3'";//合意状态 -和退回
            }
            else
            {
                strDyInfo = "'0','1','2','3'";//对应状态全部
                strHyInfo = "'1','2','4'";//合意状态 待确认和已合意答复销售不可订货
            }
            if (strDataState == "")
            {
                strDyInfo = "'0','1','2','3'";//对应状态全部
                strHyInfo = "'0','1','2','3','4'";//合意状态全部
            }
            DataTable dataTable = fs0602_DataAccess.getSearchInfo(strYearMonth, strDyState, strHyState, strPartId, strCarModel,
                   strInOut, strOrderingMethod, strOrderPlant, strHaoJiu, strSupplierId, strSupplierPlant, strDyInfo, strHyInfo, strOverDue);
            return dataTable;
        }

        public DataTable getHeJiInfo(string strYearMonth, string strDyState, string strHyState, string strPartId, string strCarModel,
             string strInOut, string strOrderingMethod, string strOrderPlant, string strHaoJiu, string strSupplierId, string strSupplierPlant, string strDataState, string strOverDue)
        {
            string strDyInfo = "";
            string strHyInfo = "";
            if (strDataState == "0")
            {
                strDyInfo = "'0','1','2','3'";//对应状态全部
                strHyInfo = "'0','3'";//合意状态 -和退回
            }
            else
            {
                strDyInfo = "'0','1','2','3'";//对应状态全部
                strHyInfo = "'1','2','4'";//合意状态 待确认和已合意答复销售不可订货
            }
            if (strDataState == "")
            {
                strDyInfo = "'0','1','2','3'";//对应状态全部
                strHyInfo = "'0','1','2','3','4'";//合意状态全部
            }
            DataTable dataTable = fs0602_DataAccess.getHeJiInfo(strYearMonth, strDyState, strHyState, strPartId, strCarModel,
                   strInOut, strOrderingMethod, strOrderPlant, strHaoJiu, strSupplierId, strSupplierPlant, strDyInfo, strHyInfo, strOverDue);
            return dataTable;
        }

        public DataTable getTankInfo(string strYearMonth)
        {
            DataTable dataTable = fs0602_DataAccess.getTankInfo(strYearMonth);
            return dataTable;
        }

        public string generateExcelWithXlt(DataTable dt, string[] field, string rootPath, string xltName, int startRow, string strUserId, string strFunctionName, string strYearMonth, string strYearMonth_2, string strYearMonth_3)
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
                ICellStyle style = hssfworkbook.CreateCellStyle();
                style.Alignment = HorizontalAlignment.Center;


                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row = sheet.CreateRow(startRow + i);
                    for (int j = 0; j < field.Length; j++)
                    {
                        Type type = dt.Columns[field[j]].DataType;
                        ICell cell = row.CreateCell(j);
                        if (type == Type.GetType("System.Decimal"))
                        {
                            if (dt.Rows[i][field[j]].ToString().Trim() != "")
                                cell.SetCellValue(Convert.ToDouble(dt.Rows[i][field[j]].ToString()));
                        }
                        else if (type == Type.GetType("System.Int32"))
                        {
                            if (dt.Rows[i][field[j]].ToString().Trim() != "")
                                cell.SetCellValue(Convert.ToInt32(dt.Rows[i][field[j]].ToString()));
                        }
                        else if (type == Type.GetType("System.Int16"))
                        {
                            if (dt.Rows[i][field[j]].ToString().Trim() != "")
                                cell.SetCellValue(Convert.ToInt16(dt.Rows[i][field[j]].ToString()));
                        }
                        else if (type == Type.GetType("System.Int64"))
                        {
                            if (dt.Rows[i][field[j]].ToString().Trim() != "")
                                cell.SetCellValue(Convert.ToInt64(dt.Rows[i][field[j]].ToString()));
                        }
                        else
                        {
                            cell.SetCellValue(dt.Rows[i][field[j]].ToString());
                        }
                        cell.CellStyle = style;
                    }
                }

                //以下业务特别处理

                int iMonth = Convert.ToInt32(strYearMonth.Substring(4, 2));//对象月
                int iMonth_2 = Convert.ToInt32(strYearMonth_2.Substring(4, 2));//内示月
                int iMonth_3 = Convert.ToInt32(strYearMonth_3.Substring(4, 2));//内内示月

                sheet.GetRow(1).GetCell(12).SetCellValue(iMonth + "月");
                sheet.GetRow(1).GetCell(14).SetCellValue(iMonth_2 + "月");
                sheet.GetRow(1).GetCell(15).SetCellValue(iMonth_3 + "月");
                sheet.GetRow(1).GetCell(16).SetCellValue(iMonth + "月");
                sheet.GetRow(1).GetCell(17).SetCellValue(iMonth_2 + "月");
                sheet.GetRow(1).GetCell(18).SetCellValue(iMonth_3 + "月");
                sheet.GetRow(1).GetCell(19).SetCellValue(iMonth + "月");
                sheet.GetRow(1).GetCell(20).SetCellValue(iMonth_2 + "月");
                sheet.GetRow(1).GetCell(21).SetCellValue(iMonth_3 + "月");



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
        public DataTable setListInfo(List<Dictionary<string, Object>> listMultipleData, DataTable dtMultiple, string dExpectTime)
        {
            try
            {
                for (int i = 0; i < listMultipleData.Count; i++)
                {
                    string strYearMonth = fs0603_DataAccess.setNullValue(listMultipleData[i]["vcYearMonth"], "", "");
                    string strDyState = fs0603_DataAccess.setNullValue(listMultipleData[i]["vcDyState"], "", "");
                    string strHyState = fs0603_DataAccess.setNullValue(listMultipleData[i]["vcHyState"], "", "");
                    string strPart_id = fs0603_DataAccess.setNullValue(listMultipleData[i]["vcPart_id"], "", "");
                    string strCbSOQN = fs0603_DataAccess.setNullValue(listMultipleData[i]["iCbSOQN"], "", "0");
                    string strCbSOQN1 = fs0603_DataAccess.setNullValue(listMultipleData[i]["iCbSOQN1"], "", "0");
                    string strCbSOQN2 = fs0603_DataAccess.setNullValue(listMultipleData[i]["iCbSOQN2"], "", "0");
                    string strTzhSOQN = fs0603_DataAccess.setNullValue(listMultipleData[i]["iTzhSOQN"], "", "0");
                    string strTzhSOQN1 = fs0603_DataAccess.setNullValue(listMultipleData[i]["iTzhSOQN1"], "", "0");
                    string strTzhSOQN2 = fs0603_DataAccess.setNullValue(listMultipleData[i]["iTzhSOQN2"], "", "0");
                    string strSupplierId = fs0603_DataAccess.setNullValue(listMultipleData[i]["vcSupplierId"], "", "");
                    string strSupplierPlant = fs0603_DataAccess.setNullValue(listMultipleData[i]["vcSupplierPlant"], "", "");
                    string strExpectTime = fs0603_DataAccess.setNullValue(dExpectTime, "", "");
                    string strInputType = "company";
                    DataRow drMultiple = dtMultiple.NewRow();
                    drMultiple["vcYearMonth"] = strYearMonth;
                    drMultiple["vcDyState"] = strDyState;
                    drMultiple["vcHyState"] = strHyState;
                    drMultiple["vcPart_id"] = strPart_id;
                    drMultiple["iCbSOQN"] = strCbSOQN;
                    drMultiple["iCbSOQN1"] = strCbSOQN1;
                    drMultiple["iCbSOQN2"] = strCbSOQN2;
                    drMultiple["iTzhSOQN"] = strTzhSOQN;
                    drMultiple["iTzhSOQN1"] = strTzhSOQN1;
                    drMultiple["iTzhSOQN2"] = strTzhSOQN2;
                    drMultiple["vcSupplierId"] = strSupplierId;
                    drMultiple["vcSupplierPlant"] = strSupplierPlant;
                    drMultiple["dExpectTime"] = strExpectTime;
                    drMultiple["vcInputType"] = strInputType;
                    dtMultiple.Rows.Add(drMultiple);
                }
                return dtMultiple;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public DataTable checkSaveInfo(DataTable dtMultiple, string strYearMonth, string strYearMonth1, string strYearMonth2,
            string strOperId, string strPackingPlant, string strReceiver, ref DataTable dtMessage)
        {
            try
            {
                dtMultiple.Columns.Add("vcHy4");
                DataTable dterrMessage = new DataTable();
                dterrMessage.Columns.Add("vcPart_id");
                dterrMessage.Columns.Add("vcMsg");
                //fs0602_DataAccess.checkSaveInfo(dtMultiple, strYearMonth, strYearMonth1, strYearMonth2,
                //    strOperId, strPackingPlant, strReceiver, ref dtMessage);
                fs0501_DataAccess.SaveCheck(dtMultiple, strOperId, strYearMonth, strYearMonth1, strYearMonth2, ref dterrMessage, strPackingPlant, strReceiver);
                for (int i = 0; i < dterrMessage.Rows.Count; i++)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = dterrMessage.Rows[i]["vcPart_id"].ToString() + "     " + dterrMessage.Rows[i]["vcMsg"].ToString();
                    dtMessage.Rows.Add(dataRow);
                    for (int j = 0; j < dtMultiple.Rows.Count; j++)
                    {
                        if (dtMultiple.Rows[j]["vcPart_id"].ToString() == dterrMessage.Rows[i]["vcPart_id"].ToString())
                        {
                            dtMultiple.Rows[j]["vcHy4"] = "4";
                        }
                    }
                }
                return dtMultiple;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setSaveInfo(string strYearMonth, string strOperId, ref DataTable dtMessage)
        {
            try
            {
                fs0602_DataAccess.setSaveInfo(strYearMonth, strOperId, ref dtMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable checkopenplanInfo(List<Dictionary<string, Object>> multipleInfoData, DataTable dtInfo, string dExpectTime, string strOperId, string strPackingPlant, string strReceiver, ref DataTable dtMessage)
        {
            try
            {
                DataTable dtImport = fs0603_Logic.createTable("SOQ602");
                if (multipleInfoData.Count == 0)
                {
                    for (int i = 0; i < dtInfo.Rows.Count; i++)
                    {
                        string strYearMonth = dtInfo.Rows[i]["vcYearMonth"] == null ? "" : dtInfo.Rows[i]["vcYearMonth"].ToString();
                        string strPart_id = dtInfo.Rows[i]["vcPart_id"] == null ? "" : dtInfo.Rows[i]["vcPart_id"].ToString();
                        string strSupplierId = dtInfo.Rows[i]["vcSupplierId"] == null ? "" : dtInfo.Rows[i]["vcSupplierId"].ToString();
                        string strSupplierPlant = dtInfo.Rows[i]["vcSupplierPlant"] == null ? "" : dtInfo.Rows[i]["vcSupplierPlant"].ToString();
                        string strDyState = dtInfo.Rows[i]["vcDyState"] == null ? "" : dtInfo.Rows[i]["vcDyState"].ToString();
                        string strHyState = dtInfo.Rows[i]["vcHyState"] == null ? "" : dtInfo.Rows[i]["vcHyState"].ToString();
                        string strCbSOQN = dtInfo.Rows[i]["iCbSOQN"].ToString() == "" ? "0" : dtInfo.Rows[i]["iCbSOQN"].ToString();
                        string strCbSOQN1 = dtInfo.Rows[i]["iCbSOQN1"].ToString() == "" ? "0" : dtInfo.Rows[i]["iCbSOQN1"].ToString();
                        string strCbSOQN2 = dtInfo.Rows[i]["iCbSOQN2"].ToString() == "" ? "0" : dtInfo.Rows[i]["iCbSOQN2"].ToString();
                        string strTzhSOQN = dtInfo.Rows[i]["iTzhSOQN"].ToString() == "" ? "0" : dtInfo.Rows[i]["iTzhSOQN"].ToString();
                        string strTzhSOQN1 = dtInfo.Rows[i]["iTzhSOQN1"].ToString() == "" ? "0" : dtInfo.Rows[i]["iTzhSOQN1"].ToString();
                        string strTzhSOQN2 = dtInfo.Rows[i]["iTzhSOQN2"].ToString() == "" ? "0" : dtInfo.Rows[i]["iTzhSOQN2"].ToString();
                        string strExpectTime = fs0603_DataAccess.setNullValue(dExpectTime, "", "");
                        string strInputType = "company";
                        if ((strDyState == "0" || strDyState == "2" || strDyState == "3") &&
                            (strHyState == "0"))//0：未发送；2：有调整；3：无调整//0：待回复(TFTM)；3：退回
                                                //(strHyState == "0" || strHyState == "3"))//0：未发送；2：有调整；3：无调整//0：待回复(TFTM)；3：退回
                        {
                            DataRow drImport = dtImport.NewRow();
                            drImport["vcYearMonth"] = strYearMonth;
                            drImport["vcDyState"] = strDyState;
                            drImport["vcHyState"] = strHyState;
                            drImport["vcPart_id"] = strPart_id;
                            drImport["iCbSOQN"] = strCbSOQN;
                            drImport["iCbSOQN1"] = strCbSOQN1;
                            drImport["iCbSOQN2"] = strCbSOQN2;
                            drImport["iTzhSOQN"] = strTzhSOQN;
                            drImport["iTzhSOQN1"] = strTzhSOQN1;
                            drImport["iTzhSOQN2"] = strTzhSOQN2;
                            drImport["vcSupplierId"] = strSupplierId;
                            drImport["vcSupplierPlant"] = strSupplierPlant;
                            drImport["dExpectTime"] = strExpectTime;
                            drImport["vcInputType"] = strInputType;
                            dtImport.Rows.Add(drImport);
                        }
                    }
                }
                else
                {
                    dtImport = setListInfo(multipleInfoData, dtImport, dExpectTime);
                    int count = dtImport.Rows.Count;
                    for (int i = count - 1; i >= 0; i--)
                    {
                        string strDyState = dtImport.Rows[i]["vcDyState"].ToString();
                        string strHyState = dtImport.Rows[i]["vcHyState"].ToString();
                        if (!((strDyState == "0" || strDyState == "2" || strDyState == "3")
                            && (strHyState == "0")))
                            //&& (strHyState == "0" || strHyState == "3")))
                            dtImport.Rows.RemoveAt(i);
                    }
                }
                if (dtImport.Rows.Count == 0)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "没有可供内示展开的数据(原因：内示数据已经展开或销售退回内示不能再次展开)";
                    dtMessage.Rows.Add(dataRow);
                }
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                    return null;
                string sYearMonth = "";
                string sYearMonth1 = "";
                string sYearMonth2 = "";
                if (dtImport.Rows.Count != 0)
                {
                    sYearMonth = Convert.ToDateTime(dtImport.Rows[0]["vcYearMonth"].ToString().Substring(0, 4) + "-" + dtImport.Rows[0]["vcYearMonth"].ToString().Substring(4, 2) + "-01").ToString("yyyyMM");
                    sYearMonth1 = Convert.ToDateTime(dtImport.Rows[0]["vcYearMonth"].ToString().Substring(0, 4) + "-" + dtImport.Rows[0]["vcYearMonth"].ToString().Substring(4, 2) + "-01").AddMonths(1).ToString("yyyyMM");
                    sYearMonth2 = Convert.ToDateTime(dtImport.Rows[0]["vcYearMonth"].ToString().Substring(0, 4) + "-" + dtImport.Rows[0]["vcYearMonth"].ToString().Substring(4, 2) + "-01").AddMonths(2).ToString("yyyyMM");
                }
                if (sYearMonth == "" || sYearMonth1 == "" || sYearMonth2 == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "对象月为空，无法继续操作";
                    dtMessage.Rows.Add(dataRow);
                }
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                    return null;
                return checkSaveInfo(dtImport, sYearMonth, sYearMonth1, sYearMonth2, strOperId, strPackingPlant, strReceiver, ref dtMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void openPlan(DataTable dtImport, string strOperId, ref DataTable dtMessage)
        {
            try
            {
                string strDyState = "1";
                string strHyState = "";
                fs0602_DataAccess.setSaveInfo_op(dtImport, strDyState, strHyState, strOperId, ref dtMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="strFRId">发件人ID</param>
        /// <param name="strFRName">发件人Name</param>
        /// <param name="strFRAddress">发件人地址</param>
        /// <param name="strTheme">主题</param>
        /// <param name="strEmailBody">邮件体</param>
        /// <param name="dtToList">收件人List</param>
        /// <param name=""></param>
        public void sendEmailInfo(string strFRId, string strFRName, string strFRAddress, string strTheme, string strEmailBody, DataTable dtToList, ref DataTable dtMessage)
        {
            try
            {
                DataTable dtEmail = fs0602_DataAccess.getSupplierEmail();
                for (int i = 0; i < dtToList.Rows.Count; i++)
                {
                    string strSupplierId = dtToList.Rows[i]["vcSupplierId"].ToString();
                    string strSupplierPlant = dtToList.Rows[i]["vcSupplierPlant"].ToString();
                    DataTable dtToInfo = fs0603_Logic.createTable("mailaddress");
                    DataRow[] drEmail = dtEmail.Select("vcSupplier_id = '" + strSupplierId + "' and vcSupplierPlant = '" + strSupplierPlant + "'");
                    if (drEmail.Length != 0)
                    {
                        for (int j = 0; j < drEmail.Length; j++)
                        {
                            DataRow drToInfo = dtToInfo.NewRow();
                            drToInfo["address"] = drEmail[j]["vcEmail1"].ToString();
                            drToInfo["displayName"] = drEmail[j]["vcLXR1"].ToString();
                            dtToInfo.Rows.Add(drToInfo);
                        }
                        DataTable dtCcInfo = null;
                        string result = ComFunction.SendEmailInfo(strFRAddress, strFRName, strEmailBody, dtToInfo, dtCcInfo, strTheme, "", false);
                        if (result != "Success")
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = "供应商代码：" + strSupplierId + "工区：" + strSupplierPlant + "邮件发送失败，请采取其他形式联络。";
                            dtMessage.Rows.Add(dataRow);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public void sendEmailInfo_FTMS(string strFRId, string strFRName, string strFRAddress, string strTheme, string strEmailBody, ref DataTable dtMessage)
        {
            try
            {
                DataTable dtEmail = fs0602_DataAccess.getFTMSEmail();
                DataTable dtToInfo = fs0603_Logic.createTable("mailaddress");
                for (int j = 0; j < dtEmail.Rows.Count; j++)
                {
                    DataRow drToInfo = dtToInfo.NewRow();
                    drToInfo["address"] = dtEmail.Rows[j]["vcEmail1"].ToString();
                    drToInfo["displayName"] = dtEmail.Rows[j]["vcLXR1"].ToString();
                    dtToInfo.Rows.Add(drToInfo);
                }
                DataTable dtCcInfo = null;
                string result = ComFunction.SendEmailInfo(strFRAddress, strFRName, strEmailBody, dtToInfo, dtCcInfo, strTheme, "", false);
                if (result != "Success")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "回复销售公司邮件发送失败，请采取其他形式联络。";
                    dtMessage.Rows.Add(dataRow);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public DataTable getToList(DataTable dataTable, ref DataTable dtMessage)
        {
            try
            {
                //根据供应商及纳期进行分组
                DataTable dtb = new DataTable("dtb");
                DataColumn dc1 = new DataColumn("vcSupplierId", Type.GetType("System.String"));
                DataColumn dc2 = new DataColumn("vcSupplierPlant", Type.GetType("System.String"));
                dtb.Columns.Add(dc1);
                dtb.Columns.Add(dc2);
                var query = from t in dataTable.AsEnumerable()
                            group t by new { t1 = t.Field<string>("vcSupplierId"), t2 = t.Field<string>("vcSupplierPlant") } into m
                            select new
                            {
                                SupplierId = m.Key.t1,
                                SupplierPlant = m.Key.t2,
                                rowcount = m.Count()
                            };
                if (query.ToList().Count > 0)
                {
                    query.ToList().ForEach(q =>
                    {
                        DataRow dr = dtb.NewRow();
                        dr["vcSupplierId"] = q.SupplierId;
                        dr["vcSupplierPlant"] = q.SupplierPlant;
                        dtb.Rows.Add(dr);
                    });
                }
                return dtb;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable checkreplyplanInfo(List<Dictionary<string, Object>> listInfoData, DataTable dataTable, string dExpectTime,
            string strOperId, string strPackingPlant, string strReceiver, ref DataTable dtMessage)
        {
            try
            {
                DataTable dtImport = fs0603_Logic.createTable("SOQ602");
                if (listInfoData.Count == 0)
                {
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        string strYearMonth = dataTable.Rows[i]["vcYearMonth"] == null ? "" : dataTable.Rows[i]["vcYearMonth"].ToString();
                        string strPart_id = dataTable.Rows[i]["vcPart_id"] == null ? "" : dataTable.Rows[i]["vcPart_id"].ToString();
                        string strSupplierId = dataTable.Rows[i]["vcSupplierId"] == null ? "" : dataTable.Rows[i]["vcSupplierId"].ToString();
                        string strSupplierPlant = dataTable.Rows[i]["vcSupplierPlant"] == null ? "" : dataTable.Rows[i]["vcSupplierPlant"].ToString();
                        string strDyState = dataTable.Rows[i]["vcDyState"] == null ? "" : dataTable.Rows[i]["vcDyState"].ToString();
                        string strHyState = dataTable.Rows[i]["vcHyState"] == null ? "" : dataTable.Rows[i]["vcHyState"].ToString();
                        string strCbSOQN = dataTable.Rows[i]["iCbSOQN"].ToString() == "" ? "0" : dataTable.Rows[i]["iCbSOQN"].ToString();
                        string strCbSOQN1 = dataTable.Rows[i]["iCbSOQN1"].ToString() == "" ? "0" : dataTable.Rows[i]["iCbSOQN1"].ToString();
                        string strCbSOQN2 = dataTable.Rows[i]["iCbSOQN2"].ToString() == "" ? "0" : dataTable.Rows[i]["iCbSOQN2"].ToString();
                        string strTzhSOQN = dataTable.Rows[i]["iTzhSOQN"].ToString() == "" ? "0" : dataTable.Rows[i]["iTzhSOQN"].ToString();
                        string strTzhSOQN1 = dataTable.Rows[i]["iTzhSOQN1"].ToString() == "" ? "0" : dataTable.Rows[i]["iTzhSOQN1"].ToString();
                        string strTzhSOQN2 = dataTable.Rows[i]["iTzhSOQN2"].ToString() == "" ? "0" : dataTable.Rows[i]["iTzhSOQN2"].ToString();
                        string strExpectTime = fs0603_DataAccess.setNullValue(dExpectTime, "", "");
                        string strInputType = "company";
                        if ((strHyState == "0" || strHyState == "3"))
                        {
                            DataRow drImport = dtImport.NewRow();
                            drImport["vcYearMonth"] = strYearMonth;
                            drImport["vcDyState"] = strDyState;
                            drImport["vcHyState"] = strHyState;
                            drImport["vcPart_id"] = strPart_id;
                            drImport["iCbSOQN"] = strCbSOQN;
                            drImport["iCbSOQN1"] = strCbSOQN1;
                            drImport["iCbSOQN2"] = strCbSOQN2;
                            drImport["iTzhSOQN"] = strTzhSOQN;
                            drImport["iTzhSOQN1"] = strTzhSOQN1;
                            drImport["iTzhSOQN2"] = strTzhSOQN2;
                            drImport["vcSupplierId"] = strSupplierId;
                            drImport["vcSupplierPlant"] = strSupplierPlant;
                            drImport["dExpectTime"] = strExpectTime;
                            drImport["vcInputType"] = strInputType;
                            dtImport.Rows.Add(drImport);
                        }
                    }
                }
                else
                {
                    dtImport = setListInfo(listInfoData, dtImport, dExpectTime);
                    int count = dtImport.Rows.Count;
                    for (int i = count - 1; i >= 0; i--)
                    {
                        if (!((dtImport.Rows[i]["vcHyState"].ToString() == "0" || dtImport.Rows[i]["vcHyState"].ToString() == "3")))
                            dtImport.Rows.RemoveAt(i);
                    }
                }
                if (dtImport.Rows.Count == 0)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "没有可供内示回复的数据(原因：内示数据已经完成回复不能重复该操作)";
                    dtMessage.Rows.Add(dataRow);
                    return null;
                }

                string sYearMonth = "";
                string sYearMonth1 = "";
                string sYearMonth2 = "";
                if (dtImport.Rows.Count != 0)
                {
                    sYearMonth = Convert.ToDateTime(dtImport.Rows[0]["vcYearMonth"].ToString().Substring(0, 4) + "-" + dtImport.Rows[0]["vcYearMonth"].ToString().Substring(4, 2) + "-01").ToString("yyyyMM");
                    sYearMonth1 = Convert.ToDateTime(dtImport.Rows[0]["vcYearMonth"].ToString().Substring(0, 4) + "-" + dtImport.Rows[0]["vcYearMonth"].ToString().Substring(4, 2) + "-01").AddMonths(1).ToString("yyyyMM");
                    sYearMonth2 = Convert.ToDateTime(dtImport.Rows[0]["vcYearMonth"].ToString().Substring(0, 4) + "-" + dtImport.Rows[0]["vcYearMonth"].ToString().Substring(4, 2) + "-01").AddMonths(2).ToString("yyyyMM");
                }
                dtImport = checkSaveInfo(dtImport, sYearMonth, sYearMonth1, sYearMonth2,
                     strOperId, strPackingPlant, strReceiver, ref dtMessage);
                return dtImport;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void replyPlan(DataTable dtImport, string strOperId, ref DataTable dtMessage)
        {
            try
            {
                string strDyState = "";
                string strHyState = "1";
                //for (int i = 0; i < dtImport.Rows.Count; i++)
                //{
                //    if (dtImport.Rows[i]["vcHy4"].ToString() == "")
                //    {
                //        dtImport.Rows[i]["vcHy4"] = "1";
                //    }
                //}
                fs0602_DataAccess.setSaveInfo_rp(dtImport, strDyState, strHyState, strOperId, ref dtMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string checkreturnplanInfo(List<Dictionary<string, Object>> listInfoData, DataTable dataTable, string dExpectTime,
              ref DataTable dtMessage)
        {
            try
            {
                DataTable dtImport = fs0603_Logic.createTable("SOQ602");
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    string strYearMonth = dataTable.Rows[i]["vcYearMonth"] == null ? "" : dataTable.Rows[i]["vcYearMonth"].ToString();
                    string strPart_id = dataTable.Rows[i]["vcPart_id"] == null ? "" : dataTable.Rows[i]["vcPart_id"].ToString();
                    string strSupplierId = dataTable.Rows[i]["vcSupplierId"] == null ? "" : dataTable.Rows[i]["vcSupplierId"].ToString();
                    string strSupplierPlant = dataTable.Rows[i]["vcSupplierPlant"] == null ? "" : dataTable.Rows[i]["vcSupplierPlant"].ToString();
                    string strDyState = dataTable.Rows[i]["vcDyState"] == null ? "" : dataTable.Rows[i]["vcDyState"].ToString();
                    string strHyState = dataTable.Rows[i]["vcHyState"] == null ? "" : dataTable.Rows[i]["vcHyState"].ToString();
                    string strCbSOQN = dataTable.Rows[i]["iCbSOQN"].ToString() == "" ? "0" : dataTable.Rows[i]["iCbSOQN"].ToString();
                    string strCbSOQN1 = dataTable.Rows[i]["iCbSOQN1"].ToString() == "" ? "0" : dataTable.Rows[i]["iCbSOQN1"].ToString();
                    string strCbSOQN2 = dataTable.Rows[i]["iCbSOQN2"].ToString() == "" ? "0" : dataTable.Rows[i]["iCbSOQN2"].ToString();
                    string strTzhSOQN = dataTable.Rows[i]["iTzhSOQN"].ToString() == "" ? "0" : dataTable.Rows[i]["iTzhSOQN"].ToString();
                    string strTzhSOQN1 = dataTable.Rows[i]["iTzhSOQN1"].ToString() == "" ? "0" : dataTable.Rows[i]["iTzhSOQN1"].ToString();
                    string strTzhSOQN2 = dataTable.Rows[i]["iTzhSOQN2"].ToString() == "" ? "0" : dataTable.Rows[i]["iTzhSOQN2"].ToString();
                    string strExpectTime = fs0603_DataAccess.setNullValue(dExpectTime, "", "");
                    string strInputType = "company";
                    if ((strHyState == "0" || strHyState == "3"))
                    {
                        DataRow drImport = dtImport.NewRow();
                        drImport["vcYearMonth"] = strYearMonth;
                        drImport["vcDyState"] = strDyState;
                        drImport["vcHyState"] = strHyState;
                        drImport["vcPart_id"] = strPart_id;
                        drImport["iCbSOQN"] = strCbSOQN;
                        drImport["iCbSOQN1"] = strCbSOQN1;
                        drImport["iCbSOQN2"] = strCbSOQN2;
                        drImport["iTzhSOQN"] = strTzhSOQN;
                        drImport["iTzhSOQN1"] = strTzhSOQN1;
                        drImport["iTzhSOQN2"] = strTzhSOQN2;
                        drImport["vcSupplierId"] = strSupplierId;
                        drImport["vcSupplierPlant"] = strSupplierPlant;
                        drImport["dExpectTime"] = strExpectTime;
                        drImport["vcInputType"] = strInputType;
                        dtImport.Rows.Add(drImport);
                    }
                }
                if (dtImport.Rows.Count == 0)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "没有可供回退的内示情报";
                    dtMessage.Rows.Add(dataRow);
                }
                if (dtImport.Rows.Count != dataTable.Rows.Count)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "该对象月内示情报已经有待确认或已合意状态，不能整体退回";
                    dtMessage.Rows.Add(dataRow);
                }
                return dtImport.Rows[0]["vcYearMonth"].ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void replyPlan(string strReturnym, string strOperId, ref DataTable dtMessage)
        {
            try
            {
                string strDyState = "";
                string strHyState = "";
                fs0602_DataAccess.setSaveInfo_rn(strReturnym, strDyState, strHyState, strOperId, ref dtMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string setEmailBody(string strExpectTime, string strFlag)
        {
            StringBuilder sbr = new StringBuilder();
            //sbr.AppendLine(" <p><strong class=\"ql - size - huge\" style = \"background - color: rgb(102, 185, 102); color: rgb(230, 0, 0); \"><em>请用户尽快提供符合用户标准的邮件样式！</em></strong></p><p><br></p>");
            sbr.AppendLine("<p>各位供应商殿&nbsp;（请转发给贵司社内相关人员）</p>");
            sbr.AppendLine("<p><br></p>");
            sbr.AppendLine("<p>TFTM 补给资材企管课 </p>");
            sbr.AppendLine("<p>长期补给业务协力感谢！</p>");
            sbr.AppendLine("<p><br></p>");
            sbr.AppendLine("<p>关于标题一事，联络如下：</p>");
            sbr.AppendLine("<p><br></p>");
            sbr.AppendLine("<p>现送附TFTM补给未来三个月内示（N月=确定、N+1=内示、N+2=内示），请查收。</p>");
            sbr.AppendLine("<p>请厂家在系统上确认内示能否对应，并在系统上进行回复。</p>");
            sbr.AppendLine("<p>如个别月数量需调整，可以在三个月范围内修正，但三个月总数不能增加或减少。</p>");
            sbr.AppendLine("<p><br></p>");
            sbr.AppendLine("<p>回复纳期：<u style=\"color: rgb(230, 0, 0);\">" + strExpectTime + "</u>下班前（逾期或未回复视为可以对应。）</p>");
            sbr.AppendLine("<p>如有问题，请随时与我联络（联络方式：022-66230666）。</p><p><br></p>");
            sbr.AppendLine("<p>以上。</p><p><br></p>");

            return sbr.ToString();
        }

        public DataTable getExportRef(string strYearMonth, string strDXYNum, string strNSYNum, string strNNSYNum, ref DataTable dtMessage)
        {
            try
            {
                string strYear = strYearMonth.Substring(0, 4);
                string strYear_Last = (Convert.ToInt32(strYearMonth.Substring(0, 4)) + 1).ToString();

                string strYearMonth_before = Convert.ToDateTime(strYearMonth.Substring(0, 4) + "-" + strYearMonth.Substring(4, 2) + "-01").AddMonths(-1).ToString("yyyyMM");

                string strYearMonth_now = Convert.ToDateTime(strYearMonth.Substring(0, 4) + "-" + strYearMonth.Substring(4, 2) + "-01").AddMonths(0).ToString("yyyy年MM月");
                string strYearMonth_next = Convert.ToDateTime(strYearMonth.Substring(0, 4) + "-" + strYearMonth.Substring(4, 2) + "-01").AddMonths(1).ToString("yyyy年MM月");
                string strYearMonth_last = Convert.ToDateTime(strYearMonth.Substring(0, 4) + "-" + strYearMonth.Substring(4, 2) + "-01").AddMonths(2).ToString("yyyy年MM月");

                string strMonth = Convert.ToDateTime(strYearMonth.Substring(0, 4) + "-" + strYearMonth.Substring(4, 2) + "-01").AddMonths(0).ToString("MM月");
                string strMonth_next = Convert.ToDateTime(strYearMonth.Substring(0, 4) + "-" + strYearMonth.Substring(4, 2) + "-01").AddMonths(1).ToString("MM月");
                string strMonth_last = Convert.ToDateTime(strYearMonth.Substring(0, 4) + "-" + strYearMonth.Substring(4, 2) + "-01").AddMonths(2).ToString("MM月");

                DataTable dtRef_SOQ = fs0602_DataAccess.getExportRef_SOQ(strYearMonth, strYearMonth_before, ref dtMessage);
                DataTable dtRef_ANN = fs0602_DataAccess.getExportRef_ANN(strYear, strYear_Last, ref dtMessage);
                DataTable dtRef = fs0603_Logic.createTable("ExportRef0602");
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                    return null;
                for (int i = 0; i < dtRef_SOQ.Rows.Count; i++)
                {
                    DataRow drRef = dtRef.NewRow();
                    drRef["vcExportDate"] = System.DateTime.Now.ToString("yyyy-MM-dd");
                    drRef["vcRefDate"] = strYearMonth_now + "～" + strYearMonth_last;
                    drRef["vcMonth_dx"] = strMonth;
                    drRef["vcMonth_ns"] = strMonth_next;
                    drRef["vcMonth_nns"] = strMonth_last;
                    drRef["decDXYNum"] = strDXYNum;
                    drRef["decNSYNum"] = strNSYNum;
                    drRef["decNNSYNum"] = strNNSYNum;
                    string vcProject = dtRef_SOQ.Rows[i]["vcProject"].ToString();
                    drRef["vcProject"] = vcProject;
                    string vcYearMonth_dx = dtRef_SOQ.Rows[i]["vcYearMonth_dx"].ToString();
                    drRef["vcYearMonth_dx"] = vcYearMonth_dx;
                    drRef["decSOQ_dx"] = dtRef_SOQ.Rows[i]["decSOQ_dx"].ToString();
                    drRef["decSOQ_ns_before"] = dtRef_SOQ.Rows[i]["decSOQ_ns_before"].ToString();
                    string vcYearMonth_ns = dtRef_SOQ.Rows[i]["vcYearMonth_ns"].ToString();
                    drRef["vcYearMonth_ns"] = vcYearMonth_ns;
                    drRef["decSOQ_ns"] = dtRef_SOQ.Rows[i]["decSOQ_ns"].ToString();
                    drRef["decSOQ_nns_before"] = dtRef_SOQ.Rows[i]["decSOQ_nns_before"].ToString();
                    string vcYearMonth_nns = dtRef_SOQ.Rows[i]["vcYearMonth_nns"].ToString();
                    drRef["vcYearMonth_nns"] = vcYearMonth_nns;
                    drRef["decSOQ_nns"] = dtRef_SOQ.Rows[i]["decSOQ_nns"].ToString();
                    DataRow[] drNNA_dx = dtRef_ANN.Select("vcProject='" + vcProject + "' and vcYear='" + vcYearMonth_dx.Substring(0, 4) + "'");
                    DataRow[] drNNA_ns = dtRef_ANN.Select("vcProject='" + vcProject + "' and vcYear='" + vcYearMonth_ns.Substring(0, 4) + "'");
                    DataRow[] drNNA_nns = dtRef_ANN.Select("vcProject='" + vcProject + "' and vcYear='" + vcYearMonth_nns.Substring(0, 4) + "'");
                    #region drRef["decNNA_dx"]
                    if (strMonth == "01月")
                        drRef["decNNA_dx"] = drNNA_dx.Length == 0 ? "" : drNNA_dx[0]["vcJanuary"].ToString();
                    if (strMonth == "02月")
                        drRef["decNNA_dx"] = drNNA_dx.Length == 0 ? "" : drNNA_dx[0]["vcFebruary"].ToString();
                    if (strMonth == "03月")
                        drRef["decNNA_dx"] = drNNA_dx.Length == 0 ? "" : drNNA_dx[0]["vcMarch"].ToString();
                    if (strMonth == "04月")
                        drRef["decNNA_dx"] = drNNA_dx.Length == 0 ? "" : drNNA_dx[0]["vcApril"].ToString();
                    if (strMonth == "05月")
                        drRef["decNNA_dx"] = drNNA_dx.Length == 0 ? "" : drNNA_dx[0]["vcMay"].ToString();
                    if (strMonth == "06月")
                        drRef["decNNA_dx"] = drNNA_dx.Length == 0 ? "" : drNNA_dx[0]["vcJune"].ToString();
                    if (strMonth == "07月")
                        drRef["decNNA_dx"] = drNNA_dx.Length == 0 ? "" : drNNA_dx[0]["vcJuly"].ToString();
                    if (strMonth == "08月")
                        drRef["decNNA_dx"] = drNNA_dx.Length == 0 ? "" : drNNA_dx[0]["vcAugust"].ToString();
                    if (strMonth == "09月")
                        drRef["decNNA_dx"] = drNNA_dx.Length == 0 ? "" : drNNA_dx[0]["vcSeptember"].ToString();
                    if (strMonth == "10月")
                        drRef["decNNA_dx"] = drNNA_dx.Length == 0 ? "" : drNNA_dx[0]["vcOctober"].ToString();
                    if (strMonth == "11月")
                        drRef["decNNA_dx"] = drNNA_dx.Length == 0 ? "" : drNNA_dx[0]["vcNovember"].ToString();
                    if (strMonth == "12月")
                        drRef["decNNA_dx"] = drNNA_dx.Length == 0 ? "" : drNNA_dx[0]["vcDecember"].ToString();
                    #endregion
                    #region drRef["decNNA_ns"]
                    if (strMonth_next == "01月")
                        drRef["decNNA_ns"] = drNNA_ns.Length == 0 ? "" : drNNA_ns[0]["vcJanuary"].ToString();
                    if (strMonth_next == "02月")
                        drRef["decNNA_ns"] = drNNA_ns.Length == 0 ? "" : drNNA_ns[0]["vcFebruary"].ToString();
                    if (strMonth_next == "03月")
                        drRef["decNNA_ns"] = drNNA_ns.Length == 0 ? "" : drNNA_ns[0]["vcMarch"].ToString();
                    if (strMonth_next == "04月")
                        drRef["decNNA_ns"] = drNNA_ns.Length == 0 ? "" : drNNA_ns[0]["vcApril"].ToString();
                    if (strMonth_next == "05月")
                        drRef["decNNA_ns"] = drNNA_ns.Length == 0 ? "" : drNNA_ns[0]["vcMay"].ToString();
                    if (strMonth_next == "06月")
                        drRef["decNNA_ns"] = drNNA_ns.Length == 0 ? "" : drNNA_ns[0]["vcJune"].ToString();
                    if (strMonth_next == "07月")
                        drRef["decNNA_ns"] = drNNA_ns.Length == 0 ? "" : drNNA_ns[0]["vcJuly"].ToString();
                    if (strMonth_next == "08月")
                        drRef["decNNA_ns"] = drNNA_ns.Length == 0 ? "" : drNNA_ns[0]["vcAugust"].ToString();
                    if (strMonth_next == "09月")
                        drRef["decNNA_ns"] = drNNA_ns.Length == 0 ? "" : drNNA_ns[0]["vcSeptember"].ToString();
                    if (strMonth_next == "10月")
                        drRef["decNNA_ns"] = drNNA_ns.Length == 0 ? "" : drNNA_ns[0]["vcOctober"].ToString();
                    if (strMonth_next == "11月")
                        drRef["decNNA_ns"] = drNNA_ns.Length == 0 ? "" : drNNA_ns[0]["vcNovember"].ToString();
                    if (strMonth_next == "12月")
                        drRef["decNNA_ns"] = drNNA_ns.Length == 0 ? "" : drNNA_ns[0]["vcDecember"].ToString();
                    #endregion
                    #region drRef["decNNA_nns"]
                    if (strMonth_last == "01月")
                        drRef["decNNA_nns"] = drNNA_nns.Length == 0 ? "" : drNNA_nns[0]["vcJanuary"].ToString();
                    if (strMonth_last == "02月")
                        drRef["decNNA_nns"] = drNNA_nns.Length == 0 ? "" : drNNA_nns[0]["vcFebruary"].ToString();
                    if (strMonth_last == "03月")
                        drRef["decNNA_nns"] = drNNA_nns.Length == 0 ? "" : drNNA_nns[0]["vcMarch"].ToString();
                    if (strMonth_last == "04月")
                        drRef["decNNA_nns"] = drNNA_nns.Length == 0 ? "" : drNNA_nns[0]["vcApril"].ToString();
                    if (strMonth_last == "05月")
                        drRef["decNNA_nns"] = drNNA_nns.Length == 0 ? "" : drNNA_nns[0]["vcMay"].ToString();
                    if (strMonth_last == "06月")
                        drRef["decNNA_nns"] = drNNA_nns.Length == 0 ? "" : drNNA_nns[0]["vcJune"].ToString();
                    if (strMonth_last == "07月")
                        drRef["decNNA_nns"] = drNNA_nns.Length == 0 ? "" : drNNA_nns[0]["vcJuly"].ToString();
                    if (strMonth_last == "08月")
                        drRef["decNNA_nns"] = drNNA_nns.Length == 0 ? "" : drNNA_nns[0]["vcAugust"].ToString();
                    if (strMonth_last == "09月")
                        drRef["decNNA_nns"] = drNNA_nns.Length == 0 ? "" : drNNA_nns[0]["vcSeptember"].ToString();
                    if (strMonth_last == "10月")
                        drRef["decNNA_nns"] = drNNA_nns.Length == 0 ? "" : drNNA_nns[0]["vcOctober"].ToString();
                    if (strMonth_last == "11月")
                        drRef["decNNA_nns"] = drNNA_nns.Length == 0 ? "" : drNNA_nns[0]["vcNovember"].ToString();
                    if (strMonth_last == "12月")
                        drRef["decNNA_nns"] = drNNA_nns.Length == 0 ? "" : drNNA_nns[0]["vcDecember"].ToString();
                    #endregion
                    dtRef.Rows.Add(drRef);
                }
                return dtRef;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void checkDBInfo(string strYearMonth, string strPackingPlant, string strReceiver, ref DataTable dtMessage)
        {
            DataTable dtCheckInfo = fs0602_DataAccess.checkDBInfo(strYearMonth, strPackingPlant, strReceiver);
            if (dtCheckInfo.Rows.Count != 0)
            {
                for (int i = 0; i < dtCheckInfo.Rows.Count; i++)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "品番：" + dtCheckInfo.Rows[i]["vcPart_id"].ToString() + "，供应商：" + dtCheckInfo.Rows[i]["vcSupplier_id"].ToString() + "在对象月：" + dtCheckInfo.Rows[i]["vcYM"].ToString() + "存在多条有效的" + dtCheckInfo.Rows[i]["vcItem"].ToString() + "。";
                    dtMessage.Rows.Add(dataRow);
                }
            }
        }
    }
}
