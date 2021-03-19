using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS1405_Logic
    {
        FS1405_DataAccess fs1405_DataAccess;
        FS0603_DataAccess fs0603_DataAccess = new FS0603_DataAccess();
        FS0603_Logic fs0603_Logic = new FS0603_Logic();

        public FS1405_Logic()
        {
            fs1405_DataAccess = new FS1405_DataAccess();
        }
        public DataTable getSearchInfo(string strPartId, string strSupplierId, string strHaoJiu, string strInOut, string strOrderPlant, string strFrom, string strTo, string strCarModel, string strSPISStatus, List<Object> listTime)
        {
            return fs1405_DataAccess.getSearchInfo(strPartId, strSupplierId, strHaoJiu, strInOut, strOrderPlant, strFrom, strTo, strCarModel, strSPISStatus, listTime);
        }
        public void checksendtoInfo(List<Dictionary<string, Object>> multipleInfoData, string strToTime, ref DataTable dtImport, string strOperId, string strPackingPlant, ref DataTable dtMessage)
        {
            try
            {
                DataTable dtSPISTime = fs0603_Logic.createTable("savFs1404");
                if (multipleInfoData.Count != 0)
                {
                    for (int i = 0; i < multipleInfoData.Count; i++)
                    {
                        if (fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcSPISStatus"], "", "") == "0" ||
                            fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcSPISStatus"], "", "") == "3")
                        {
                            string strLinId = fs0603_DataAccess.setNullValue(multipleInfoData[i]["LinId"], "", "");
                            string strApplyId = Guid.NewGuid().ToString("N");
                            string strToTime_SPIS = strToTime;
                            string strSPISTime = string.Empty;
                            string strPartId = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcPartId"], "", "");
                            string strCarfamilyCode = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcCarfamilyCode"], "", "");
                            string strSupplierId = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcSupplierId"], "", "");
                            string strPartENName = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcPartENName"], "", "");
                            string strColourNo = string.Empty;
                            string strColourCode = string.Empty;
                            string strColourName = string.Empty;
                            string strModItem = string.Empty;
                            string strPicUrl = string.Empty;
                            string strPDFUrl = string.Empty;
                            string strSPISUrl = string.Empty;
                            string strSupplier_1 = string.Empty;
                            string strSupplier_2 = string.Empty;
                            string strOperName = string.Empty;
                            string strGM = string.Empty;
                            DataRow drImport = dtImport.NewRow();
                            drImport["LinId"] = strLinId;
                            drImport["vcApplyId"] = strApplyId;
                            drImport["dToTime_SPIS"] = strSPISTime;
                            drImport["dSPISTime"] = strSPISTime;
                            drImport["vcPartId"] = strPartId;
                            drImport["vcCarfamilyCode"] = strCarfamilyCode;
                            drImport["vcSupplierId"] = strSupplierId;
                            drImport["vcPartENName"] = strPartENName;
                            drImport["vcColourNo"] = strColourNo;
                            drImport["vcColourCode"] = strColourCode;
                            drImport["vcColourName"] = strColourName;
                            drImport["vcModItem"] = strModItem;
                            drImport["vcPicUrl"] = strPicUrl;
                            drImport["vcPDFUrl"] = strPDFUrl;
                            drImport["vcSPISUrl"] = strSPISUrl;
                            drImport["vcSupplier_1"] = strSupplier_1;
                            drImport["vcSupplier_2"] = strSupplier_2;
                            drImport["vcOperName"] = strOperName;
                            drImport["vcGM"] = strGM;
                            drImport["vcSPISStatus"] = "1";//依赖标识
                            dtImport.Rows.Add(drImport);
                        }
                    }
                }
                if (dtImport.Rows.Count == 0)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "没有可供依赖发送的数据";
                    dtMessage.Rows.Add(dataRow);
                }
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                    return;
                dtSPISTime = checkTimeAndPic(dtImport, dtSPISTime, "sendto", ref dtMessage);//赋值
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                    return;
                sendtoInfo(dtImport, dtSPISTime, strOperId, ref dtMessage);//更新
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void checkadmitInfo(List<Dictionary<string, Object>> multipleInfoData, ref DataTable dtImport, string strOperId, string strPackingPlant, ref DataTable dtMessage)
        {
            try
            {
                DataTable dtSPISTime = fs0603_Logic.createTable("savFs1404");
                if (multipleInfoData.Count != 0)
                {
                    for (int i = 0; i < multipleInfoData.Count; i++)
                    {
                        if (fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcSPISStatus"], "", "") == "2")
                        {
                            string strLinId = fs0603_DataAccess.setNullValue(multipleInfoData[i]["LinId"], "", "");
                            string strApplyId = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcApplyId"], "", "");
                            string strFromTime_SPIS = fs0603_DataAccess.setNullValue(multipleInfoData[i]["dToTime_SPIS"], "date", "");
                            string strToTime_SPIS = fs0603_DataAccess.setNullValue(multipleInfoData[i]["dToTime"], "date", "");
                            string strPicUrl = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcPicUrl"], "", "");
                            string strPDFUrl = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcPDFUrl"], "", "");
                            string strSPISUrl = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcSPISUrl"], "", "");
                            string strPartId = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcPartId"], "", "");
                            string strCarfamilyCode = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcCarfamilyCode"], "", "");
                            string strSupplierId = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcSupplierId"], "", "");
                            string strModItem = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcModItem"], "", "");
                            DataRow drImport = dtImport.NewRow();
                            drImport["LinId"] = strLinId;
                            drImport["vcApplyId"] = strApplyId;
                            drImport["dFromTime_SPIS"] = strFromTime_SPIS;
                            drImport["dToTime_SPIS"] = strToTime_SPIS;
                            drImport["vcPartId"] = strPartId;
                            drImport["vcCarfamilyCode"] = strCarfamilyCode;
                            drImport["vcSupplierId"] = strSupplierId;
                            drImport["vcModItem"] = strModItem;
                            drImport["vcPicUrl"] = strPicUrl;
                            drImport["vcPDFUrl"] = strPDFUrl;
                            drImport["vcSPISUrl"] = strSPISUrl;
                            drImport["vcSPISStatus"] = "3";//依赖标识
                            dtImport.Rows.Add(drImport);
                        }
                    }
                }
                if (dtImport.Rows.Count == 0)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "没有可供承认的数据";
                    dtMessage.Rows.Add(dataRow);
                }
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                    return;
                dtSPISTime = checkTimeAndPic(dtImport, dtSPISTime, "admit", ref dtMessage);//赋值
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                    return;
                admitInfo(dtImport, dtSPISTime, strOperId, ref dtMessage);//更新
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void checkrejectInfo(List<Dictionary<string, Object>> multipleInfoData, ref DataTable dtImport, string strOperId, string strPackingPlant, ref DataTable dtMessage)
        {
            try
            {
                DataTable dtSPISTime = fs0603_Logic.createTable("savFs1404");
                if (multipleInfoData.Count != 0)
                {
                    for (int i = 0; i < multipleInfoData.Count; i++)
                    {
                        if (fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcSPISStatus"], "", "") == "2")
                        {
                            string strLinId = fs0603_DataAccess.setNullValue(multipleInfoData[i]["LinId"], "", "");
                            string strApplyId = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcApplyId"], "", "");
                            string strFromTime_SPIS = fs0603_DataAccess.setNullValue(multipleInfoData[i]["dToTime_SPIS"], "date", "");
                            string strToTime_SPIS = fs0603_DataAccess.setNullValue(multipleInfoData[i]["dToTime"], "date", "");
                            string strPicUrl = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcPicUrl"], "", "");
                            string strPDFUrl = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcPDFUrl"], "", "");
                            string strSPISUrl = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcSPISUrl"], "", "");
                            string strPartId = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcPartId"], "", "");
                            string strCarfamilyCode = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcCarfamilyCode"], "", "");
                            string strSupplierId = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcSupplierId"], "", "");
                            string strModItem = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcModItem"], "", "");
                            DataRow drImport = dtImport.NewRow();
                            drImport["LinId"] = strLinId;
                            drImport["vcApplyId"] = strApplyId;
                            drImport["dFromTime_SPIS"] = strFromTime_SPIS;
                            drImport["dToTime_SPIS"] = strToTime_SPIS;
                            drImport["vcPartId"] = strPartId;
                            drImport["vcCarfamilyCode"] = strCarfamilyCode;
                            drImport["vcSupplierId"] = strSupplierId;
                            drImport["vcModItem"] = strModItem;
                            drImport["vcPicUrl"] = strPicUrl;
                            drImport["vcPDFUrl"] = strPDFUrl;
                            drImport["vcSPISUrl"] = strSPISUrl;
                            drImport["vcSPISStatus"] = "4";//依赖标识
                            dtImport.Rows.Add(drImport);
                        }
                    }
                }
                if (dtImport.Rows.Count == 0)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "没有可供驳回的数据";
                    dtMessage.Rows.Add(dataRow);
                }
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                    return;
                dtSPISTime = checkTimeAndPic(dtImport, dtSPISTime, "reject", ref dtMessage);//赋值
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                    return;
                rejectInfo(dtImport, dtSPISTime, strOperId, ref dtMessage);//更新
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable checkTimeAndPic(DataTable dtImport, DataTable dtSPISTime, string strFunType, ref DataTable dtMessage)
        {
            try
            {
                if (strFunType == "sendto")
                {
                    for (int i = 0; i < dtImport.Rows.Count; i++)
                    {
                        string strPartId = dtImport.Rows[i]["vcPartId"].ToString();
                        string strSupplierId = dtImport.Rows[i]["vcSupplierId"].ToString();
                        string strToTime_SPIS = dtImport.Rows[i]["dToTime_SPIS"].ToString();
                        string strLinId_before = "";
                        string strFromTime_before = "";
                        string strToTime_before = "";
                        DataTable dtCheckTime = fs1405_DataAccess.getSPISTimeInfo(strPartId, strSupplierId);
                        if (dtCheckTime.Rows.Count > 0)
                        {
                            strLinId_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["LinId"].ToString();
                            strFromTime_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["vcTimeFrom"].ToString();
                            if (Convert.ToDateTime(strFromTime_before) >= Convert.ToDateTime(strToTime_SPIS))
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = "品番【" + strPartId + "】情报【SPIS有效期】维护有误(当前有效的开始使用时间小于维护的开始使用时间)";
                                dtMessage.Rows.Add(dataRow);
                            }
                            strToTime_before = Convert.ToDateTime(strToTime_SPIS).AddDays(-1).ToString("yyyy-MM-dd");
                            DataRow drSPISTime = dtSPISTime.NewRow();
                            drSPISTime["LinId"] = strLinId_before;
                            drSPISTime["dToTime"] = strToTime_before;
                            drSPISTime["vcType"] = "mod";
                            dtSPISTime.Rows.Add(drSPISTime);
                        }
                    }
                }
                if (strFunType == "admit")
                {
                    for (int i = 0; i < dtImport.Rows.Count; i++)
                    {
                        string strLinId = dtImport.Rows[i]["strLinId"].ToString();
                        string strApplyId = dtImport.Rows[i]["vcApplyId"].ToString();
                        string strFromTime_SPIS = dtImport.Rows[i]["dFromTime_SPIS"].ToString();
                        string strToTime_SPIS = dtImport.Rows[i]["dToTime_SPIS"].ToString();
                        string strPartId = dtImport.Rows[i]["vcPartId"].ToString();
                        string strCarfamilyCode = dtImport.Rows[i]["vcCarfamilyCode"].ToString();
                        string strSupplierId = dtImport.Rows[i]["vcSupplierId"].ToString();
                        string strModItem = dtImport.Rows[i]["vcModItem"].ToString();
                        string strPicUrl = dtImport.Rows[i]["vcPicUrl"].ToString();
                        //根据条件制作最终SPIS照片
                        string strPDFUrl = dtImport.Rows[i]["vcPDFUrl"].ToString();
                        string strSPISUrl = "";

                        DataRow drSPISTime = dtSPISTime.NewRow();
                        drSPISTime["vcPartId"] = strPartId;
                        drSPISTime["dFromTime"] = strFromTime_SPIS;
                        drSPISTime["dToTime"] = strToTime_SPIS;
                        drSPISTime["vcCarfamilyCode"] = strCarfamilyCode;
                        drSPISTime["vcSupplierId"] = strSupplierId;
                        drSPISTime["vcSupplierPlant"] = string.Empty;
                        drSPISTime["vcPicUrl"] = string.Empty;
                        drSPISTime["vcPicUrlUUID"] = strSPISUrl;
                        drSPISTime["vcChangeRea"] = strModItem;
                        drSPISTime["vcTJSX"] = string.Empty;
                        drSPISTime["vcType"] = "add";
                        dtSPISTime.Rows.Add(drSPISTime);
                    }
                }
                if (strFunType == "reject")
                {
                    for (int i = 0; i < dtImport.Rows.Count; i++)
                    {
                        string strLinId = dtImport.Rows[i]["strLinId"].ToString();
                        string strApplyId = dtImport.Rows[i]["vcApplyId"].ToString();
                        string strFromTime_SPIS = dtImport.Rows[i]["dFromTime_SPIS"].ToString();
                        string strToTime_SPIS = dtImport.Rows[i]["dToTime_SPIS"].ToString();
                        string strPartId = dtImport.Rows[i]["vcPartId"].ToString();
                        string strCarfamilyCode = dtImport.Rows[i]["vcCarfamilyCode"].ToString();
                        string strSupplierId = dtImport.Rows[i]["vcSupplierId"].ToString();
                        string strModItem = dtImport.Rows[i]["vcModItem"].ToString();
                        string strPicUrl = dtImport.Rows[i]["vcPicUrl"].ToString();
                        //根据条件删除照片
                        string strPDFUrl = dtImport.Rows[i]["vcPDFUrl"].ToString();
                        string strSPISUrl = "";
                    }
                }
                return dtSPISTime;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void sendtoInfo(DataTable dtImport, DataTable dtSPISTime, string strOperId, ref DataTable dtMessage)
        {
            try
            {
                fs1405_DataAccess.setSendtoInfo(dtImport, dtSPISTime, strOperId, ref dtMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void admitInfo(DataTable dtImport, DataTable dtSPISTime, string strOperId, ref DataTable dtMessage)
        {
            try
            {
                fs1405_DataAccess.setAdmitInfo(dtImport, dtSPISTime, strOperId, ref dtMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void rejectInfo(DataTable dtImport, DataTable dtSPISTime, string strOperId, ref DataTable dtMessage)
        {
            try
            {
                fs1405_DataAccess.setRejectInfo(dtImport, dtSPISTime, strOperId, ref dtMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string setEmailBody(string strToTime, string strFlag)
        {
            StringBuilder sbr = new StringBuilder();
            sbr.AppendLine("<p>各位供应商殿&nbsp;（请转发给贵司社内相关人员）</p>");
            sbr.AppendLine("<p>非常感谢一直以来对TFTM补给业务的支持！</p>");
            sbr.AppendLine("<p><br></p>");
            sbr.AppendLine("<p>关于标题一事，</p>");
            sbr.AppendLine("<p>品番SPIS变更维护。 </p>");
            sbr.AppendLine("<p>请查收。</p>");
            sbr.AppendLine("<p>品番SPIS生效时间：<u style=\"color: rgb(230, 0, 0);\">" + strToTime + "</u>请在该时间之前</p><p><br></p><p>在补给系统上进行维护上传</p>");
            sbr.AppendLine("<p>如有问题，请随时与我联络（联络方式：022-66230666）。</p><p><br></p>");
            sbr.AppendLine("<p>以上。</p><p><br></p>");

            return sbr.ToString();
        }

    }
}
