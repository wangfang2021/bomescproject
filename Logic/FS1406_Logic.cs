using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS1406_Logic
    {
        FS1406_DataAccess fs1406_DataAccess;

        public FS1406_Logic()
        {
            fs1406_DataAccess = new FS1406_DataAccess();
        }
        public DataTable getSearchInfo(string strPartId, string strSupplierId, string strOrderPlant, string strCarModel, string strSPISStatus)
        {
            return fs1406_DataAccess.getSearchInfo(strPartId, strSupplierId, strOrderPlant, strCarModel, strSPISStatus);
        }
        public DataTable getSearchInfo(string strApplyId)
        {
            return fs1406_DataAccess.getSearchInfo(strApplyId);
        }
        public DataTable checkreplyInfo(List<Dictionary<string, Object>> checkedInfoData, string strOperId, string strPackingPlant, ref DataTable dtMessage)
        {
            try
            {
                DataTable dtImport = new DataTable();
                return dtImport;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable checkSaveInfo(DataTable dtImport, string strPath, string strOperId, ref DataTable dtMessage)
        {
            try
            {
                dtImport.Columns.Add("vcType", typeof(string));
                for (int i = 0; i < dtImport.Rows.Count; i++)
                {
                    string strApplyId = dtImport.Rows[i]["vcApplyId"].ToString();
                    string strFromTime_SPIS = dtImport.Rows[i]["dFromTime_SPIS"].ToString();
                    string strToTime_SPIS = dtImport.Rows[i]["dToTime_SPIS"].ToString();
                    string strSPISTime = dtImport.Rows[i]["dSPISTime"].ToString();
                    string strPartId = dtImport.Rows[i]["vcPartId"].ToString();
                    string strCarfamilyCode = dtImport.Rows[i]["vcCarfamilyCode"].ToString();
                    string strSupplierId = dtImport.Rows[i]["vcSupplierId"].ToString();
                    string strPartENName = dtImport.Rows[i]["vcPartENName"].ToString();
                    string strColourNo = dtImport.Rows[i]["vcColourNo"].ToString();
                    string strColourCode = dtImport.Rows[i]["vcColourCode"].ToString();
                    string strColourName = dtImport.Rows[i]["vcColourName"].ToString();
                    string strModItem = dtImport.Rows[i]["vcModItem"].ToString();
                    string strOperImage = dtImport.Rows[i]["vcPicUrl"].ToString();
                    string strPDFUrl = string.Empty;
                    string strSPISUrl = string.Empty;
                    string strSupplier_1 = string.Empty;
                    string strSupplier_2 = string.Empty;
                    string strOperName = string.Empty;
                    string strGM = string.Empty;
                    string strSPISStatus = dtImport.Rows[i]["vcSPISStatus"].ToString();

                    if (strColourNo == "" || strColourCode == "" || strColourName == "")
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = "缺少必填项请完善数据。";
                        dtMessage.Rows.Add(dataRow);
                    }
                    else
                    {
                        if (strOperImage == "")
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = strPartId + "品番SPIS路径为空请完善数据。";
                            dtMessage.Rows.Add(dataRow);
                        }
                        else
                        {
                            if (!System.IO.File.Exists(strPath + strOperImage))
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = strPartId + "品番SPIS路径没有图片请重新上传。";
                                dtMessage.Rows.Add(dataRow);
                            }
                            else
                            {
                                if (strApplyId == "")
                                {
                                    dtImport.Rows[i]["vcType"] = "add";
                                    dtImport.Rows[i]["vcApplyId"] = Guid.NewGuid().ToString("N");
                                }
                                else
                                    dtImport.Rows[i]["vcType"] = "mod";

                            }
                        }
                    }
                }
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                    return dtImport;

                return dtImport;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable savePic(DataTable dtImport, string strOperId, ref DataTable dtMessage)
        {
            try
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
                return dtImport;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void replyInfo(DataTable dtImport, string strOperId, ref DataTable dtMessage)
        {
            try
            {
                fs1406_DataAccess.setAdmitInfo(dtImport, strOperId, ref dtMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setSaveInfo(DataTable dtImport, string strOperId, ref DataTable dtMessage)
        {
            try
            {
                fs1406_DataAccess.setSaveInfo(dtImport, strOperId, ref dtMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
