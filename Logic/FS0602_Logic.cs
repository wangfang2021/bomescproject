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
        FS0625_Logic fs0625_Logic = new FS0625_Logic();

        public DataTable getSearchInfo(string strYearMonth, string strDyState, string strHyState, string strPartId, string strCarModel,
                  string strInOut, string strOrderingMethod, string strOrderPlant, string strHaoJiu, string strSupplierId, string strSupplierPlant, string strDataState)
        {
            string strDyInfo = "";
            string strHyInfo = "";
            if (strDataState == "待处理")
            {
                strDyInfo = "'0','1','2','3'";//对应状态全部
                strHyInfo = "'0','3'";//合意状态 -和退回
            }
            else
            {
                strDyInfo = "'0','1','2','3'";//对应状态全部
                strHyInfo = "'1','2'";//合意状态 待确认和已合意
            }
            DataTable dataTable = fs0602_DataAccess.getSearchInfo(strYearMonth, strDyState, strHyState, strPartId, strCarModel,
                   strInOut, strOrderingMethod, strOrderPlant, strHaoJiu, strSupplierId, strSupplierPlant, strDyInfo, strHyInfo);
            return dataTable;
        }
        public void openPlan(List<Dictionary<string, Object>> listInfoData, dynamic dataForm, string dExpectTime, LoginInfo loginInfo,string strOperId, ref string strMessageList)
        {
            try
            {
                string strOperationType = "展开内示";
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("vcDyState");
                dataTable.Columns.Add("vcHyState");
                dataTable.Columns.Add("dExpectTime");
                dataTable.Columns.Add("vcYearMonth");
                dataTable.Columns.Add("vcPart_id");
                dataTable.Columns.Add("vcSupplierId");
                dataTable.Columns.Add("vcSupplierName");
                dataTable.Columns.Add("vcAddress");
                if (listInfoData == null)//按照检索条件全部提交
                {
                    string strYearMonth = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth + "/01").ToString("yyyyMM");
                    string strDyState = dataForm.DyState;
                    string strHyState = dataForm.HyState;
                    string strPartId = dataForm.PartId;
                    string strCarModel = dataForm.CarModel;
                    string strInOut = dataForm.InOut;
                    string strOrderingMethod = dataForm.OrderingMethod;
                    string strOrderPlant = dataForm.OrderPlant;
                    string strHaoJiu = dataForm.HaoJiu;
                    string strSupplierId = dataForm.SupplierId;
                    string strSupplierPlant = dataForm.SupplierPlant;
                    string strDataState = dataForm.DataState;
                    DataTable dt = getSearchInfo(strYearMonth, strDyState, strHyState, strPartId, strCarModel,
                    strInOut, strOrderingMethod, strOrderPlant, strHaoJiu, strSupplierId, strSupplierPlant, strDataState);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i]["vcDyState"].ToString() == "0")
                        {
                            DataRow dataRow = dataTable.NewRow();
                            dataRow["vcDyState"] = "1";
                            dataRow["vcHyState"] = "";
                            dataRow["dExpectTime"] = dExpectTime;
                            dataRow["vcYearMonth"] = dt.Rows[i]["vcYearMonth"].ToString();
                            dataRow["vcPart_id"] = dt.Rows[i]["vcPart_id"].ToString();
                            dataRow["vcSupplierId"] = dt.Rows[i]["vcSupplierId"].ToString();
                            dataTable.Rows.Add(dataRow);
                        }
                        else
                        {
                            strMessageList = "已经内示展开过，不能重复展开";
                        }
                    }

                }
                else//按照勾选提交
                {
                    for (int i = 0; i < listInfoData.Count; i++)
                    {
                        if (listInfoData[i]["vcDyState"].ToString() == "0")
                        {
                            DataRow dataRow = dataTable.NewRow();
                            dataRow["vcDyState"] = "1";
                            dataRow["vcHyState"] = "";
                            dataRow["dExpectTime"] = dExpectTime;
                            dataRow["vcYearMonth"] = listInfoData[i]["vcYearMonth"].ToString(); ;
                            dataRow["vcPart_id"] = listInfoData[i]["vcPart_id"].ToString(); ;
                            dataRow["vcSupplierId"] = listInfoData[i]["vcSupplierId"].ToString(); ;
                            dataTable.Rows.Add(dataRow);
                        }
                        else
                        {
                            strMessageList = "已经内示展开过，不能重复展开";
                        }
                    }
                }
                if (strMessageList == "" || dataTable.Rows.Count != 0)
                {
                    fs0602_DataAccess.setSOQInfo(strOperationType, dataTable, strOperId);
                    sendMail(loginInfo, dataTable);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void sendMail(LoginInfo loginInfo, DataTable dataTable)
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
                    string strCharacter = "select " + strSupplierId;
                    //string strCharacter = this.setCharacter(dtb);
                    //发件人
                    string strReceiver = loginInfo.Email;
                    //收件人
                    DataTable dtSender = new DataTable();
                    dtSender.Columns.Add("address");
                    dtSender.Columns.Add("displayName");
                    DataTable dtEmail = fs0602_DataAccess.getEmail(strCharacter);
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
                    strEmailBody += "请尽快进行"+ strYearMonth + " SOQ月度内示确认，本次确认纳期为:" + strExpectTime + " <br /><br />";
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

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string setCharacter(DataTable dtCharacter)
        {
            string vcCharacter = "";
            if (dtCharacter.Rows.Count != 0)
            {

                vcCharacter += "select '";
                for (int i = 0; i < dtCharacter.Rows.Count; i++)
                {

                    vcCharacter += dtCharacter.Rows[i]["vcSupplierId"].ToString();
                    if (i < dtCharacter.Rows.Count - 1)
                    {
                        vcCharacter += "' union select '";
                    }
                    else
                    {
                        vcCharacter += "'";
                    }
                }
            }
            return vcCharacter;
        }

        public void replyPlan(List<Dictionary<string, Object>> listInfoData, dynamic dataForm, string strOperId, ref string strMessageList)
        {
            try
            {
                string strOperationType = "回复内示";
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("vcDyState");
                dataTable.Columns.Add("vcHyState");
                dataTable.Columns.Add("dExpectTime");
                dataTable.Columns.Add("vcYearMonth");
                dataTable.Columns.Add("vcPart_id");
                if (listInfoData == null)//按照检索条件全部提交
                {
                    string strYearMonth = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth + "/01").ToString("yyyyMM");
                    string strDyState = dataForm.DyState;
                    string strHyState = dataForm.HyState;
                    string strPartId = dataForm.PartId;
                    string strCarModel = dataForm.CarModel;
                    string strInOut = dataForm.InOut;
                    string strOrderingMethod = dataForm.OrderingMethod;
                    string strOrderPlant = dataForm.OrderPlant;
                    string strHaoJiu = dataForm.HaoJiu;
                    string strSupplierId = dataForm.SupplierId;
                    string strSupplierPlant = dataForm.SupplierPlant;
                    string strDataState = dataForm.DataState;
                    DataTable dt = getSearchInfo(strYearMonth, strDyState, strHyState, strPartId, strCarModel,
                    strInOut, strOrderingMethod, strOrderPlant, strHaoJiu, strSupplierId, strSupplierPlant, strDataState);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i]["vcHyState"].ToString() == "0" || dt.Rows[i]["vcHyState"].ToString() == "3")
                        {
                            DataRow dataRow = dataTable.NewRow();
                            dataRow["vcDyState"] = "";
                            dataRow["vcHyState"] = "1";
                            dataRow["dExpectTime"] = "";
                            dataRow["vcYearMonth"] = dt.Rows[i]["vcYearMonth"].ToString();
                            dataRow["vcPart_id"] = dt.Rows[i]["vcPart_id"].ToString();
                            dataTable.Rows.Add(dataRow);
                        }
                        else
                        {
                            strMessageList = "已经内示回复过，不能重复回复";
                        }
                    }

                }
                else//按照勾选提交
                {
                    for (int i = 0; i < listInfoData.Count; i++)
                    {
                        if (listInfoData[i]["vcHyState"].ToString() == "0" || listInfoData[i]["vcHyState"].ToString() == "3")
                        {
                            DataRow dataRow = dataTable.NewRow();
                            dataRow["vcDyState"] = "";
                            dataRow["vcHyState"] = "1";
                            dataRow["dExpectTime"] = "";
                            dataRow["vcYearMonth"] = listInfoData[i]["vcYearMonth"].ToString(); ;
                            dataRow["vcPart_id"] = listInfoData[i]["vcPart_id"].ToString(); ;
                            dataTable.Rows.Add(dataRow);
                        }
                        else
                        {
                            strMessageList = "已经内示回复过，不能重复回复";
                        }
                    }

                }
                if (strMessageList == "" || dataTable.Rows.Count != 0)
                {
                    fs0602_DataAccess.setSOQInfo(strOperationType, dataTable, strOperId);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public void returnPlan(dynamic dataForm, string strOperId, ref string strMessageList)
        {
            try
            {
                string strOperationType = "退回内示";
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("vcDyState");
                dataTable.Columns.Add("vcHyState");
                dataTable.Columns.Add("dExpectTime");
                dataTable.Columns.Add("vcYearMonth");
                dataTable.Columns.Add("vcPart_id");
                string strYearMonth = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth + "/01").ToString("yyyyMM");
                DataRow dataRow = dataTable.NewRow();
                dataRow["vcDyState"] = "";
                dataRow["vcHyState"] = "";
                dataRow["dExpectTime"] = "";
                dataRow["vcYearMonth"] = strYearMonth;
                dataRow["vcPart_id"] = "";
                dataTable.Rows.Add(dataRow);

                //对象月需要都是未展开才能进行退回
                if (fs0602_DataAccess.checkDbInfo(strYearMonth))
                    strMessageList = "对象月" + strYearMonth + "已经存在内示展开的数据，无法在进行退回操作";
                if (strMessageList == "")
                {
                    fs0602_DataAccess.setSOQInfo(strOperationType, dataTable, strOperId);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
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
        public bool IsDQR(string strYearMonth, List<Dictionary<string, Object>> listInfoData, ref string strMsg, string strType)
        {
            DataTable dt = fs0602_DataAccess.IsDQR(strYearMonth, listInfoData, strType);
            if (dt.Rows.Count == 0)
                return true;
            else
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    strMsg += dt.Rows[i]["vcPart_id"].ToString() + "/";
                }
                strMsg = strMsg.Substring(0, strMsg.Length - 1);
                return false;
            }
        }
        public void SaveCheck(List<Dictionary<string, Object>> listInfoData, string strUserId, string strYearMonth, string strYearMonth_2, string strYearMonth_3,
    ref List<string> errMessageList, string strUnit)
        {
            fs0602_DataAccess.SaveCheck(listInfoData, strUserId, strYearMonth, strYearMonth_2, strYearMonth_3, ref errMessageList, strUnit);
        }
        public void importSave(string strYearMonth, string strUserId, string strUnit)
        {
            fs0602_DataAccess.importSave(strYearMonth, strUserId, strUnit);
        }
        #region 承认。将合意后SOQ数据复制到合意SOQ，并改变合意状态，赋予合意时间
        public int Cr(string varDxny, string varDyzt, string varHyzt, string PARTSNO)
        {
            return fs0602_DataAccess.Cr(varDxny, varDyzt, varHyzt, PARTSNO);
        }
        #endregion

        #region 展开。将初版SOQ数据复制到调整后SOQ，并改变对应状态
        public int Zk(FS0602_DataEntity searchForm)
        {
            return fs0602_DataAccess.Zk(searchForm);
        }
        #endregion

        #region 回复。改变合意状态
        public int Hf(FS0602_DataEntity searchForm)
        {
            return fs0602_DataAccess.Hf(searchForm);
        }
        #endregion

        #region 退回内示。删除该对象月3个月所有SOQ数据，并将soq履历表中的状态改为退回。
        public int thns(FS0602_DataEntity searchForm)
        {
            return fs0602_DataAccess.thns(searchForm);
        }
        #endregion
    }
}
