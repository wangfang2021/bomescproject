using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;
using System.IO;
using Common;

namespace Logic
{
    public class FS1405_Logic
    {
        FS1405_DataAccess fs1405_DataAccess;
        FS0603_DataAccess fs0603_DataAccess = new FS0603_DataAccess();
        FS0602_DataAccess fs0602_DataAccess = new FS0602_DataAccess();
        FS1406_DataAccess fs1406_DataAccess = new FS1406_DataAccess();
        FS0603_Logic fs0603_Logic = new FS0603_Logic();
        FS0617_Logic fS0617_Logic = new FS0617_Logic();
        FS1406_Logic fS1406_Logic = new FS1406_Logic();

        public FS1405_Logic()
        {
            fs1405_DataAccess = new FS1405_DataAccess();
        }
        public DataTable getSearchInfo(string strPartId, string strSupplierId, string strHaoJiu, string strInOut, string strOrderPlant, string strFrom, string strTo, string strCarModel, string strSPISStatus, List<Object> listTime)
        {
            return fs1405_DataAccess.getSearchInfo(strPartId, strSupplierId, strHaoJiu, strInOut, strOrderPlant, strFrom, strTo, strCarModel, strSPISStatus, listTime);
        }
        public void checksendtoInfo(List<Dictionary<string, Object>> multipleInfoData, ref DataTable dtImport, string strSendToTime, string strOperId, string strOpername, ref DataTable dtMessage)
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
                            string strApplyId = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcApplyId"], "", "");
                            string strPartId = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcPartId"], "", "");
                            string strFromTime_SPIS = strSendToTime;
                            string strToTime_SPIS = fs0603_DataAccess.setNullValue(multipleInfoData[i]["dToTime"], "date", "");
                            string strCarfamilyCode = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcCarfamilyCode"], "", "");
                            string strSupplierId = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcSupplierId"], "", "");
                            string strSupplierName = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcSupplierName"], "", "");
                            string strPartENName = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcPartENName"], "", "");
                            string strColourNo = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcColourNo"], "", "");
                            string strColourCode = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcColourCode"], "", "");
                            string strColourName = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcColourName"], "", "");
                            string strModItem = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcModItem"], "", "");

                            string strPICUrl = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcPICUrl"], "", "");
                            string strPICPath = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcPICPath"], "", "");

                            string strPDFUrl = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcPDFUrl"], "", "");
                            string strPDFPath = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcPDFPath"], "", "");

                            string strSPISUrl = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcSPISUrl"], "", "");
                            string strSPISPath = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcSPISPath"], "", "");
                            string strGM = strOpername;
                            string strOperName = strOpername;
                            string uuid = Guid.NewGuid().ToString("N");
                            DataRow drImport = dtImport.NewRow();
                            drImport["LinId"] = strLinId;
                            drImport["vcApplyId"] = uuid;
                            drImport["vcPartId"] = strPartId;
                            drImport["dFromTime_SPIS"] = strFromTime_SPIS;
                            drImport["dToTime_SPIS"] = strToTime_SPIS;
                            drImport["vcCarfamilyCode"] = strCarfamilyCode;
                            drImport["vcSupplierId"] = strSupplierId;
                            drImport["vcSupplierName"] = strSupplierName;
                            drImport["vcPartENName"] = strPartENName;
                            drImport["vcColourNo"] = strColourNo;
                            drImport["vcColourCode"] = strColourCode;
                            drImport["vcColourName"] = strColourName;
                            drImport["vcModItem"] = strModItem;
                            drImport["vcPICUrl"] = strPICUrl;
                            drImport["vcPICPath"] = strPICPath;
                            drImport["vcPDFUrl"] = strPDFUrl;
                            drImport["vcPDFPath"] = strPDFPath;
                            drImport["vcSPISUrl"] = strSPISUrl;
                            drImport["vcSPISPath"] = strSPISPath;
                            drImport["vcGM"] = strGM;
                            drImport["vcOperName"] = strOperName;
                            drImport["vcSPISStatus"] = "1";//依赖标识
                            dtImport.Rows.Add(drImport);

                            string strLinId_before = "";
                            string strFromTime_before = "";
                            string strToTime_before = "";
                            DataTable dtCheckTime = fs1405_DataAccess.getSPISTimeInfo(strPartId, strSupplierId);
                            if (dtCheckTime.Rows.Count > 0)
                            {
                                strLinId_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["LinId"].ToString();
                                strFromTime_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["vcTimeFrom"].ToString();
                                if (Convert.ToDateTime(strFromTime_before) >= Convert.ToDateTime(strSendToTime))
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcMessage"] = "品番【" + strPartId + "】情报【SPIS有效期】维护有误(当前有效的开始使用时间小于维护的开始使用时间)";
                                    dtMessage.Rows.Add(dataRow);
                                }
                                strToTime_before = Convert.ToDateTime(strSendToTime).AddDays(-1).ToString("yyyy-MM-dd");
                                DataRow drSPISTime = dtSPISTime.NewRow();
                                drSPISTime["LinId"] = strLinId_before;
                                drSPISTime["dToTime"] = strToTime_before;
                                drSPISTime["vcType"] = "mod";
                                dtSPISTime.Rows.Add(drSPISTime);
                            }
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
                sendtoInfo(dtImport, dtSPISTime, strOperId, ref dtMessage);//更新
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void checkadmitInfo(List<Dictionary<string, Object>> multipleInfoData, ref DataTable dtImport, ref DataTable dtApplyList,ref DataTable dtPDF_temp,ref DataTable dtSPISTime,
            string strOperId, string strOpername, ref DataTable dtMessage)
        {
            try
            {
                
                if (multipleInfoData.Count != 0)
                {
                    for (int i = 0; i < multipleInfoData.Count; i++)
                    {
                        if (fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcSPISStatus"], "", "") == "2")
                        {
                            string strLinId = fs0603_DataAccess.setNullValue(multipleInfoData[i]["LinId"], "", "");
                            string strApplyId = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcApplyId"], "", "");
                            string strPartId = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcPartId"], "", "");
                            string strFromTime_SPIS = fs0603_DataAccess.setNullValue(multipleInfoData[i]["dFromTime_SPIS"], "date", "");
                            string strToTime_SPIS = fs0603_DataAccess.setNullValue(multipleInfoData[i]["dToTime_SPIS"], "date", "");
                            string strSPISTime = fs0603_DataAccess.setNullValue(multipleInfoData[i]["dSPISTime"], "", "");
                            string strCarfamilyCode = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcCarfamilyCode"], "", "");
                            string strSupplierId = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcSupplierId"], "", "");
                            string strSupplierName = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcSupplierName"], "", "");
                            string strPartENName = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcPartENName"], "", "");
                            string strColourNo = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcColourNo"], "", "");
                            string strColourCode = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcColourCode"], "", "");
                            string strColourName = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcColourName"], "", "");
                            string strModItem = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcModItem"], "", "");

                            string strPICUrl = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcPICUrl"], "", "");
                            string strPICPath = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcPICPath"], "", "");

                            string strPDFUrl = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcPDFUrl"], "", "");
                            string strPDFPath = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcPDFPath"], "", "");

                            string strSPISUrl = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcSPISUrl"], "", "");
                            string strSPISPath = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcSPISPath"], "", "");
                            string strSupplier_1 = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcSupplier_1"], "", "");
                            string strSupplier_2 = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcSupplier_2"], "", "");
                            string strGM = strOpername;
                            string strOperName = strOpername;
                            DataRow drImport = dtImport.NewRow();
                            drImport["LinId"] = strLinId;
                            drImport["vcApplyId"] = strApplyId;
                            drImport["vcPartId"] = strPartId;
                            drImport["dFromTime_SPIS"] = strFromTime_SPIS;
                            drImport["dToTime_SPIS"] = strToTime_SPIS;
                            drImport["dSPISTime"] = strSPISTime;
                            drImport["vcCarfamilyCode"] = strCarfamilyCode;
                            drImport["vcSupplierId"] = strSupplierId;
                            drImport["vcSupplierName"] = strSupplierName;
                            drImport["vcPartENName"] = strPartENName;
                            drImport["vcColourNo"] = strColourNo;
                            drImport["vcColourCode"] = strColourCode;
                            drImport["vcColourName"] = strColourName;
                            drImport["vcModItem"] = strModItem;
                            drImport["vcPICUrl"] = strPICUrl;
                            drImport["vcPICPath"] = strPICPath;
                            drImport["vcPDFUrl"] = strPDFUrl;
                            drImport["vcPDFPath"] = strPDFPath;
                            drImport["vcSPISUrl"] = strSPISUrl;
                            drImport["vcSPISPath"] = strSPISPath;
                            drImport["vcSupplier_1"] = strSupplier_1;
                            drImport["vcSupplier_2"] = strSupplier_2;
                            drImport["vcGM"] = strGM;
                            drImport["vcOperName"] = strOperName;
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
                saveSPISPicAndApplyList_ad(dtImport, ref dtApplyList, ref dtPDF_temp, ref dtSPISTime, strOperId, ref dtMessage);//赋值
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                    return;
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void saveSPISPicAndApplyList_ad(DataTable dtImport, ref DataTable dtApplyList, ref DataTable dtPDF_temp, ref DataTable dtSPISTime,
            string strOperId, ref DataTable dtMessage)
        {
            try
            {
                for (int i = 0; i < dtImport.Rows.Count; i++)
                {
                    #region  Data
                    string uuid = Guid.NewGuid().ToString("N");
                    string strLinId = dtImport.Rows[i]["LinId"].ToString();
                    string strApplyId = dtImport.Rows[i]["vcApplyId"].ToString();
                    string strFromTime_SPIS = dtImport.Rows[i]["dFromTime_SPIS"].ToString();
                    string strToTime_SPIS = dtImport.Rows[i]["dToTime_SPIS"].ToString();
                    string strSPISTime = dtImport.Rows[i]["dSPISTime"].ToString();
                    string strPartId = dtImport.Rows[i]["vcPartId"].ToString();
                    string strCarfamilyCode = dtImport.Rows[i]["vcCarfamilyCode"].ToString();
                    string strSupplierId = dtImport.Rows[i]["vcSupplierId"].ToString();
                    string strSupplierName = dtImport.Rows[i]["vcSupplierName"].ToString();
                    string strPartENName = dtImport.Rows[i]["vcPartENName"].ToString();
                    string strColourNo = dtImport.Rows[i]["vcColourNo"].ToString();
                    string strColourCode = dtImport.Rows[i]["vcColourCode"].ToString();
                    string strColourName = dtImport.Rows[i]["vcColourName"].ToString();
                    string strModItem = dtImport.Rows[i]["vcModItem"].ToString();
                    string strSupplier_1 = dtImport.Rows[i]["vcSupplier_1"].ToString();
                    string strSupplier_2 = dtImport.Rows[i]["vcSupplier_2"].ToString();

                    string strPICUrl = dtImport.Rows[i]["vcPicUrl"].ToString();//原图正式文件
                    string sources_pic = dtImport.Rows[i]["vcPICPath"].ToString();//原图正式文件地址
                    //正式地址图片转成二进制流
                    byte[] btPICImage = fS0617_Logic.PhotoToArray(sources_pic);

                    string strPDFUrl = dtImport.Rows[i]["vcPDFUrl"].ToString();//PDF文件
                    string sources_pdf = dtImport.Rows[i]["vcPDFPath"].ToString();//PDF文件地址

                    string strSPISUrl = dtImport.Rows[i]["vcSPISUrl"].ToString();//正式文件
                    string sources_spis = dtImport.Rows[i]["vcSPISPath"].ToString();//式文件地址


                    string strOperName = dtImport.Rows[i]["vcOperName"].ToString();
                    string strGM = dtImport.Rows[i]["vcGM"].ToString();
                    #endregion

                    #region dtPDF_temp
                    DataRow drPDF_temp = dtPDF_temp.NewRow();
                    drPDF_temp["UUID"] = uuid;
                    drPDF_temp["vcSPISTime"] = strSPISTime;
                    drPDF_temp["vcCarfamilyCode"] = strCarfamilyCode;
                    drPDF_temp["vcSupplierId"] = strSupplierId;
                    drPDF_temp["vcSupplierName"] = strSupplierName;
                    drPDF_temp["vcPartId"] = strPartId;
                    drPDF_temp["vcPartENName"] = strPartENName;
                    drPDF_temp["vcColourNo"] = strColourNo;
                    drPDF_temp["vcColourCode"] = strColourCode;
                    drPDF_temp["vcColourName"] = strColourName;
                    drPDF_temp["vcPICImage"] = btPICImage;
                    drPDF_temp["vcModDate1"] = System.DateTime.Now.ToString("yyyy-MM-dd");
                    drPDF_temp["vcModItem1"] = strModItem;
                    drPDF_temp["vcModDate2"] = string.Empty;
                    drPDF_temp["vcModItem2"] = string.Empty;
                    drPDF_temp["vcModDate3"] = string.Empty;
                    drPDF_temp["vcModItem3"] = string.Empty;
                    drPDF_temp["vcSupplier_1"] = strSupplier_1;
                    drPDF_temp["vcSupplier_2"] = strSupplier_2;
                    drPDF_temp["vcOperName"] = strOperName;
                    drPDF_temp["vcGM"] = strGM;
                    drPDF_temp["vcPDFPath"] = sources_pdf;
                    drPDF_temp["vcSPISPath"] = sources_spis;
                    dtPDF_temp.Rows.Add(drPDF_temp);
                    #endregion

                    #region dtApplyList
                    DataRow drApplyList = dtApplyList.NewRow();
                    drApplyList["vcType"] = "mod";
                    drApplyList["vcApplyId"] = strApplyId;
                    drApplyList["vcPartId"] = strPartId;
                    drApplyList["vcSupplierId"] = strSupplierId;
                    drApplyList["vcSupplierName"] = strSupplierName;
                    drApplyList["vcPartENName"] = strPartENName;
                    drApplyList["vcColourNo"] = strColourNo;
                    drApplyList["vcColourCode"] = strColourCode;
                    drApplyList["vcColourName"] = strColourName;
                    drApplyList["dModDate"] = System.DateTime.Now.ToString("yyyy-MM-dd");
                    drApplyList["vcModItem"] = strModItem;
                    drApplyList["vcPICUrl"] = strPICUrl;
                    drApplyList["vcPICPath"] = sources_pic;
                    drApplyList["vcPDFUrl"] = strPDFUrl;
                    drApplyList["vcPDFPath"] = sources_pdf;
                    drApplyList["vcSPISUrl"] = strSPISUrl;
                    drApplyList["vcSPISPath"] = sources_spis;
                    drApplyList["vcSupplier_1"] = strSupplier_1;
                    drApplyList["vcSupplier_2"] = strSupplier_2;
                    drApplyList["vcOperName"] = strOperName;
                    drApplyList["vcGM"] = strGM;
                    drApplyList["vcSPISStatus"] = dtImport.Rows[i]["vcSPISStatus"].ToString();
                    dtApplyList.Rows.Add(drApplyList);
                    #endregion

                    #region TSPISQf表更新和插入用于tftm承认阶段
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
                    #endregion
                }
                ////处理图像
                ////1.插入并打印
                //fS1406_Logic.setCRVtoPdf(dtPDF_temp, strOperId, ref dtMessage);
                ////2.PDF转SPIS图片
                //fS1406_Logic.setPdftoImgs(dtApplyList, strOperId, ref dtMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void checkrejectInfo(List<Dictionary<string, Object>> multipleInfoData, string strOperId, string strOpername, ref DataTable dtMessage)
        {
            try
            {
                DataTable dtSPISTime = fs0603_Logic.createTable("savFs1404");
                DataTable dtImport = fs0603_Logic.createTable("SPISApply");
                if (multipleInfoData.Count != 0)
                {
                    for (int i = 0; i < multipleInfoData.Count; i++)
                    {
                        if (fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcSPISStatus"], "", "") == "2")
                        {
                            string strLinId = fs0603_DataAccess.setNullValue(multipleInfoData[i]["LinId"], "", "");
                            string strApplyId = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcApplyId"], "", "");
                            string strPartId = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcPartId"], "", "");
                            string strFromTime_SPIS = fs0603_DataAccess.setNullValue(multipleInfoData[i]["dFromTime_SPIS"], "date", "");
                            string strToTime_SPIS = fs0603_DataAccess.setNullValue(multipleInfoData[i]["dToTime_SPIS"], "date", "");
                            string strCarfamilyCode = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcCarfamilyCode"], "", "");
                            string strSupplierId = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcSupplierId"], "", "");
                            string strSupplierName = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcSupplierName"], "", "");
                            string strPartENName = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcPartENName"], "", "");
                            string strColourNo = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcColourNo"], "", "");
                            string strColourCode = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcColourCode"], "", "");
                            string strColourName = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcColourName"], "", "");
                            string strModItem = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcModItem"], "", "");

                            string strPICUrl = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcPICUrl"], "", "");
                            string strPICPath = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcPICPath"], "", "");

                            string strPDFUrl = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcPDFUrl"], "", "");
                            string strPDFPath = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcPDFPath"], "", "");

                            string strSPISUrl = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcSPISUrl"], "", "");
                            string strSPISPath = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcSPISPath"], "", "");
                            string strGM = strOpername;
                            string strOperName = strOpername;
                            DataRow drImport = dtImport.NewRow();
                            drImport["LinId"] = strLinId;
                            drImport["vcApplyId"] = strApplyId;
                            drImport["vcPartId"] = strPartId;
                            drImport["dFromTime_SPIS"] = strFromTime_SPIS;
                            drImport["dToTime_SPIS"] = strToTime_SPIS;
                            drImport["vcCarfamilyCode"] = strCarfamilyCode;
                            drImport["vcSupplierId"] = strSupplierId;
                            drImport["vcSupplierName"] = strSupplierName;
                            drImport["vcPartENName"] = strPartENName;
                            drImport["vcColourNo"] = strColourNo;
                            drImport["vcColourCode"] = strColourCode;
                            drImport["vcColourName"] = strColourName;
                            drImport["vcModItem"] = strModItem;
                            drImport["vcPICUrl"] = strPICUrl;
                            drImport["vcPICPath"] = strPICPath;
                            drImport["vcPDFUrl"] = strPDFUrl;
                            drImport["vcPDFPath"] = strPDFPath;
                            drImport["vcSPISUrl"] = strSPISUrl;
                            drImport["vcSPISPath"] = strSPISPath;
                            drImport["vcGM"] = strGM;
                            drImport["vcOperName"] = strOperName;
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
                DataTable dtApplyList = dtImport.Clone();
                saveSPISPicAndApplyList_re(dtImport, ref dtApplyList, strOperId, ref dtMessage);//赋值
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
        public void saveSPISPicAndApplyList_re(DataTable dtImport, ref DataTable dtApplyList, string strOperId, ref DataTable dtMessage)
        {
            try
            {
                DataTable dtPDF_temp = fs1406_DataAccess.getPrintTemp("FS1406").Clone();
                for (int i = 0; i < dtImport.Rows.Count; i++)
                {
                    #region  Data
                    string uuid = Guid.NewGuid().ToString("N");
                    string strLinId = dtImport.Rows[i]["LinId"].ToString();
                    string strApplyId = dtImport.Rows[i]["vcApplyId"].ToString();
                    string strFromTime_SPIS = dtImport.Rows[i]["dFromTime_SPIS"].ToString();
                    string strToTime_SPIS = dtImport.Rows[i]["dToTime_SPIS"].ToString();
                    string strSPISTime = dtImport.Rows[i]["dSPISTime"].ToString();
                    string strPartId = dtImport.Rows[i]["vcPartId"].ToString();
                    string strCarfamilyCode = dtImport.Rows[i]["vcCarfamilyCode"].ToString();
                    string strSupplierId = dtImport.Rows[i]["vcSupplierId"].ToString();
                    string strSupplierName = dtImport.Rows[i]["vcSupplierName"].ToString();
                    string strPartENName = dtImport.Rows[i]["vcPartENName"].ToString();
                    string strColourNo = dtImport.Rows[i]["vcColourNo"].ToString();
                    string strColourCode = dtImport.Rows[i]["vcColourCode"].ToString();
                    string strColourName = dtImport.Rows[i]["vcColourName"].ToString();
                    string strModItem = dtImport.Rows[i]["vcModItem"].ToString();

                    string strPICUrl = dtImport.Rows[i]["vcPicUrl"].ToString();//原图正式文件
                    string sources_pic = dtImport.Rows[i]["vcPICPath"].ToString();//原图正式文件地址
                    if (System.IO.File.Exists(sources_pic))
                    {
                        File.Delete(sources_pic);
                    }
                    string strPDFUrl = dtImport.Rows[i]["vcPDFUrl"].ToString();//PDF文件
                    string sources_pdf = dtImport.Rows[i]["vcPDFPath"].ToString();//PDF文件地址
                    if (System.IO.File.Exists(sources_pdf))
                    {
                        File.Delete(sources_pdf);
                    }
                    string strSPISUrl = dtImport.Rows[i]["vcSPISUrl"].ToString();//正式文件
                    string sources_spis = dtImport.Rows[i]["vcSPISPath"].ToString();//式文件地址
                    if (System.IO.File.Exists(sources_spis))
                    {
                        File.Delete(sources_spis);
                    }
                    string strSupplier_1 = dtImport.Rows[i]["vcSupplier_1"].ToString();
                    string strSupplier_2 = dtImport.Rows[i]["vcSupplier_2"].ToString();
                    string strOperName = dtImport.Rows[i]["vcOperName"].ToString();
                    string strGM = dtImport.Rows[i]["vcGM"].ToString();
                    #endregion

                    #region dtPDF_temp
                    #endregion

                    #region dtApplyList
                    DataRow drApplyList = dtApplyList.NewRow();
                    drApplyList["vcType"] = "mod";
                    drApplyList["vcApplyId"] = strApplyId;
                    drApplyList["vcPartId"] = strPartId;
                    drApplyList["vcSupplierId"] = strSupplierId;
                    drApplyList["vcSupplierName"] = strSupplierName;
                    drApplyList["vcPartENName"] = strPartENName;
                    drApplyList["vcColourNo"] = strColourNo;
                    drApplyList["vcColourCode"] = strColourCode;
                    drApplyList["vcColourName"] = strColourName;
                    drApplyList["dModDate"] = System.DateTime.Now.ToString("yyyy-MM-dd");
                    drApplyList["vcModItem"] = strModItem;
                    drApplyList["vcPICUrl"] = strPICUrl;
                    drApplyList["vcPICPath"] = sources_pic;
                    drApplyList["vcPDFUrl"] = strPDFUrl;
                    drApplyList["vcPDFPath"] = sources_pdf;
                    drApplyList["vcSPISUrl"] = strSPISUrl;
                    drApplyList["vcSPISPath"] = sources_spis;
                    drApplyList["vcSupplier_1"] = strSupplier_1;
                    drApplyList["vcSupplier_2"] = strSupplier_2;
                    drApplyList["vcOperName"] = strOperName;
                    drApplyList["vcGM"] = strGM;
                    drApplyList["vcSPISStatus"] = dtImport.Rows[i]["vcSPISStatus"].ToString();
                    dtApplyList.Rows.Add(drApplyList);
                    #endregion

                    #region TSPISQf表更新和插入用于tftm承认阶段

                    #endregion
                }
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
        public DataTable getToList(DataTable dataTable, ref DataTable dtMessage)
        {
            try
            {
                //根据供应商及纳期进行分组
                DataTable dtb = new DataTable("dtb");
                DataColumn dc1 = new DataColumn("vcSupplierId", Type.GetType("System.String"));
                //DataColumn dc2 = new DataColumn("vcSupplierPlant", Type.GetType("System.String"));
                dtb.Columns.Add(dc1);
                //dtb.Columns.Add(dc2);
                var query = from t in dataTable.AsEnumerable()
                            group t by new { t1 = t.Field<string>("vcSupplierId")} into m
                            select new
                            {
                                SupplierId = m.Key.t1,
                                //SupplierPlant = m.Key.t2,
                                rowcount = m.Count()
                            };
                if (query.ToList().Count > 0)
                {
                    query.ToList().ForEach(q =>
                    {
                        DataRow dr = dtb.NewRow();
                        dr["vcSupplierId"] = q.SupplierId;
                        //dr["vcSupplierPlant"] = q.SupplierPlant;
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
        public void sendEmailInfo(string strFRId, string strFRName, string strFRAddress, string strTheme, string strEmailBody, DataTable dtToList, ref DataTable dtMessage)
        {
            try
            {
                DataTable dtEmail = fs0602_DataAccess.getSupplierEmail();
                for (int i = 0; i < dtToList.Rows.Count; i++)
                {
                    string strSupplierId = dtToList.Rows[i]["vcSupplierId"].ToString();
                    //string strSupplierPlant = dtToList.Rows[i]["vcSupplierPlant"].ToString();
                    DataTable dtToInfo = fs0603_Logic.createTable("mailaddress");
                    DataRow[] drEmail = dtEmail.Select("vcSupplier_id = '" + strSupplierId + "'");
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
                        dataRow["vcMessage"] = "供应商代码：" + strSupplierId +"邮件发送失败，请采取其他形式联络。";
                        dtMessage.Rows.Add(dataRow);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
