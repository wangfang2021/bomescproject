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
        FS0603_Logic fs0603_Logic = new FS0603_Logic();
        FS0625_Logic fs0625_Logic = new FS0625_Logic();

        public DataTable getSearchInfo(string strYearMonth, string strDyState, string strHyState, string strPartId, string strCarModel,
                  string strInOut, string strOrderingMethod, string strOrderPlant, string strHaoJiu, string strSupplierId, string strSupplierPlant, string strDataState)
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
                strHyInfo = "'1','2'";//合意状态 待确认和已合意
            }
            if (strDataState == "")
            {
                strDyInfo = "'0','1','2','3'";//对应状态全部
                strHyInfo = "'0','1','2','3'";//合意状态 -和退回
            }
            DataTable dataTable = fs0602_DataAccess.getSearchInfo(strYearMonth, strDyState, strHyState, strPartId, strCarModel,
                   strInOut, strOrderingMethod, strOrderPlant, strHaoJiu, strSupplierId, strSupplierPlant, strDyInfo, strHyInfo);
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


                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row = sheet.CreateRow(startRow + i);
                    for (int j = 0; j < field.Length; j++)
                    {
                        ICell cell = row.CreateCell(j);
                        cell.SetCellValue(dt.Rows[i][field[j]].ToString());
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
            string strOperId, string strPackingPlant, string strReceiver, ref bool bReault, ref DataTable dtMessage)
        {
            try
            {
                fs0602_DataAccess.checkSaveInfo(dtMultiple, strYearMonth, strYearMonth1, strYearMonth2,
                    strOperId, strPackingPlant, strReceiver, ref dtMessage);
                if (dtMessage == null || dtMessage.Rows.Count == 0)
                    bReault = true;
                else
                    bReault = false;
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
        public DataTable checkopenplanInfo(List<Dictionary<string, Object>> listInfoData, DataTable dataTable, string dExpectTime,
            string strOperId, string strPackingPlant, string strReceiver, ref bool bReault, ref DataTable dtMessage)
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
                        if (strDyState == "0" && (strHyState == "0" || strHyState == "3"))
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
                        if (!(dtImport.Rows[i]["vcDyState"].ToString() == "0" && (dtImport.Rows[i]["vcHyState"].ToString() == "0" || dtImport.Rows[i]["vcHyState"].ToString() == "3")))
                            dtImport.Rows.RemoveAt(i);
                    }
                }
                if (dtImport.Rows.Count == 0)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "没有可供内示展开的数据";
                    dtMessage.Rows.Add(dataRow);
                    bReault = false;
                }
                string sYearMonth = Convert.ToDateTime(dtImport.Rows[0]["vcYearMonth"].ToString().Substring(0, 4) + "-" + dtImport.Rows[0]["vcYearMonth"].ToString().Substring(4, 2) + "-01").ToString("yyyyMM");
                string sYearMonth1 = Convert.ToDateTime(dtImport.Rows[0]["vcYearMonth"].ToString().Substring(0, 4) + "-" + dtImport.Rows[0]["vcYearMonth"].ToString().Substring(4, 2) + "-01").AddMonths(1).ToString("yyyyMM");
                string sYearMonth2 = Convert.ToDateTime(dtImport.Rows[0]["vcYearMonth"].ToString().Substring(0, 4) + "-" + dtImport.Rows[0]["vcYearMonth"].ToString().Substring(4, 2) + "-01").AddMonths(2).ToString("yyyyMM");

                dtImport = checkSaveInfo(dtImport, sYearMonth, sYearMonth1, sYearMonth2,
                     strOperId, strPackingPlant, strReceiver, ref bReault, ref dtMessage);
                return dtImport;
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
        public void sendMail(LoginInfo loginInfo, DataTable dataTable, ref DataTable dtMessage)
        {
            try
            {
                //根据供应商及纳期进行分组
                DataTable dtb = new DataTable("dtb");
                DataColumn dc1 = new DataColumn("vcYearMonth", Type.GetType("System.String"));
                DataColumn dc2 = new DataColumn("vcSupplierId", Type.GetType("System.String"));
                DataColumn dc3 = new DataColumn("vcSupplierName", Type.GetType("System.String"));
                DataColumn dc4 = new DataColumn("vcAddress", Type.GetType("System.String"));
                DataColumn dc5 = new DataColumn("dExpectTime", Type.GetType("System.String"));
                dtb.Columns.Add(dc1);
                dtb.Columns.Add(dc2);
                dtb.Columns.Add(dc3);
                dtb.Columns.Add(dc4);
                dtb.Columns.Add(dc5);
                var query = from t in dataTable.AsEnumerable()
                            group t by new { t1 = t.Field<string>("vcYearMonth"), t2 = t.Field<string>("vcSupplierId"), t3 = t.Field<string>("dExpectTime") } into m
                            select new
                            {
                                YearMonth = m.Key.t1,
                                SupplierId = m.Key.t2,
                                ExpectTime = m.Key.t3,
                                rowcount = m.Count()
                            };
                if (query.ToList().Count > 0)
                {
                    query.ToList().ForEach(q =>
                    {
                        DataRow dr = dtb.NewRow();
                        dr["vcYearMonth"] = q.YearMonth;
                        dr["vcSupplierId"] = q.SupplierId;
                        dr["dExpectTime"] = q.ExpectTime;
                        dtb.Rows.Add(dr);
                    });
                }
                for (int cc = 0; cc < dtb.Rows.Count; cc++)
                {
                    string strYearMonth = dtb.Rows[cc]["vcYearMonth"].ToString();
                    string strSupplierId = dtb.Rows[cc]["vcSupplierId"].ToString();
                    string strExpectTime = dtb.Rows[cc]["dExpectTime"].ToString();
                    string strCharacter = "select '" + strSupplierId + "'";
                    //string strCharacter = this.setCharacter(dtb);
                    //发件人
                    string strReceiver = loginInfo.Email;
                    //收件人
                    DataTable dtSender = new DataTable();
                    dtSender.Columns.Add("address");
                    dtSender.Columns.Add("displayName");
                    DataTable dtEmail = fs0602_DataAccess.getEmail(strCharacter);
                    if (dtEmail != null && dtEmail.Rows.Count != 0)
                    {
                        for (int i = 0; i < dtEmail.Rows.Count; i++)
                        {
                            string[] emailArray = dtEmail.Rows[i]["vcEmail"].ToString().Split(';');
                            for (int j = 0; j < emailArray.Length; j++)
                            {
                                if (emailArray[j].ToString().Length > 0)
                                {
                                    string strmail = emailArray[j].ToString();
                                    string strname = dtEmail.Rows[i]["vcLinkMan"].ToString();
                                    if (dtSender.Select("address='" + strmail + "'").Length == 0)
                                    {
                                        DataRow dr = dtSender.NewRow();
                                        dr["address"] = strmail;
                                        dr["displayName"] = strname;
                                        dtSender.Rows.Add(dr);
                                    }
                                }
                            }
                        }
                        //抄送人
                        DataTable dtCCer = new DataTable();
                        dtCCer.Columns.Add("address");
                        dtCCer.Columns.Add("displayName");
                        DataTable dtCCEmail = fs0625_Logic.getCCEmail("C054");
                        if (dtCCEmail.Rows.Count > 0)
                        {
                            dtCCer = dtCCEmail;
                        }
                        //主题
                        string strThemeInfo = strYearMonth + " SOQ月度内示待确认";
                        //内容
                        string strEmailBody = "";
                        strEmailBody += "<div style='font-family:宋体;font-size:12'>" + "供应商：" + strSupplierId + "<br /><br />";
                        strEmailBody += "  您好 ! <br /><br />";
                        strEmailBody += "请尽快进行" + strYearMonth + " SOQ月度内示确认，本次确认纳期为:" + strExpectTime + " <br /><br />";
                        strEmailBody += "以上，请对应，<br /><br />";
                        strEmailBody += "谢谢！！<br /><br />";
                        strEmailBody += " <div style='color:Red;font-weight:bold;'>";
                        strEmailBody += "  （系统邮件，请勿直接回复）<br /></div>";
                        strEmailBody += "   <div>";
                        strEmailBody += " </div>";
                        //附件
                        string strEnclosure = "";

                        string result = ComFunction.SendEmailInfo(strReceiver, loginInfo.UnitName, strEmailBody, dtSender, dtCCer, strThemeInfo, strEnclosure, false);
                        //if (result == "Success")
                        //{
                        //    logs = System.DateTime.Now.ToString() + "供应商：" + vcSupplier_id + "工区" + vcWorkArea + "邮箱发送成功！\n";
                        //}
                        //else
                        //{
                        //    logs = System.DateTime.Now.ToString() + "供应商：" + vcSupplier_id + "工区" + vcWorkArea + "邮箱发送失败，邮件发送公共方法未知原因！\n";
                        //}
                    }
                    else
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = "内示展开成功，供应商" + strSupplierId + "未找到邮箱信息，未进行邮件提醒发送";
                        dtMessage.Rows.Add(dataRow);
                    }
                }

            }
            catch (Exception ex)
            {
                DataRow dataRow = dtMessage.NewRow();
                dataRow["vcMessage"] = "内示展开成功，对供应商邮件提醒发送失败！";
                dtMessage.Rows.Add(dataRow);
            }
        }
        public DataTable checkreplyplanInfo(List<Dictionary<string, Object>> listInfoData, DataTable dataTable, string dExpectTime,
            string strOperId, string strPackingPlant, string strReceiver, ref bool bReault, ref DataTable dtMessage)
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
                    dataRow["vcMessage"] = "没有可供内示回复的数据";
                    dtMessage.Rows.Add(dataRow);
                    bReault = false;
                }

                string sYearMonth = Convert.ToDateTime(dtImport.Rows[0]["vcYearMonth"].ToString().Substring(0, 4) + "-" + dtImport.Rows[0]["vcYearMonth"].ToString().Substring(4, 2) + "-01").ToString("yyyyMM");
                string sYearMonth1 = Convert.ToDateTime(dtImport.Rows[0]["vcYearMonth"].ToString().Substring(0, 4) + "-" + dtImport.Rows[0]["vcYearMonth"].ToString().Substring(4, 2) + "-01").AddMonths(1).ToString("yyyyMM");
                string sYearMonth2 = Convert.ToDateTime(dtImport.Rows[0]["vcYearMonth"].ToString().Substring(0, 4) + "-" + dtImport.Rows[0]["vcYearMonth"].ToString().Substring(4, 2) + "-01").AddMonths(2).ToString("yyyyMM");



                dtImport = checkSaveInfo(dtImport, sYearMonth, sYearMonth1, sYearMonth2,
                     strOperId, strPackingPlant, strReceiver, ref bReault, ref dtMessage);
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
                fs0602_DataAccess.setSaveInfo_rp(dtImport, strDyState, strHyState, strOperId, ref dtMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string checkreturnplanInfo(List<Dictionary<string, Object>> listInfoData, DataTable dataTable, string dExpectTime,
             ref bool bReault, ref DataTable dtMessage)
        {
            try
            {
                DataTable dtImport = fs0603_Logic.createTable("SOQ602");
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    string strYearMonth = dataTable.Rows[i]["vcYearMonth"] == null ? "" : dataTable.Rows[i]["vcYearMonth"].ToString();
                    string strPart_id = dataTable.Rows[i]["vcPart_id"] == null ? "" : dataTable.Rows[i]["vcPart_id"].ToString();
                    string strSupplierId = dataTable.Rows[i]["vcSupplierId"] == null ? "" : dataTable.Rows[i]["vcSupplierId"].ToString();
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
                    bReault = false;
                }
                if (dtImport.Rows.Count != dataTable.Rows.Count)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "该对象月内示情报已经有待确认或已合意状态，不能整体退回";
                    dtMessage.Rows.Add(dataRow);
                    bReault = false;
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

    }
}
