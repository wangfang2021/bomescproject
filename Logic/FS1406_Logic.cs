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
        public DataTable checkSaveInfo(DataTable dtImport, string strPath, ref DataTable dtMessage)
        {
            try
            {
                for (int i = 0; i < dtImport.Rows.Count; i++)
                {
                    string strType = dtImport.Rows[i]["vcType"].ToString();
                    if (strType == "add")
                    {
                        string strPartId = dtImport.Rows[i]["vcPartId"].ToString();
                        string strSupplierId = dtImport.Rows[i]["vcSupplierId"].ToString();
                        string strFromTime = dtImport.Rows[i]["dFromTime"].ToString();
                        string strToTime = dtImport.Rows[i]["dToTime"].ToString();
                        string strPicUrl = dtImport.Rows[i]["vcPicUrl"].ToString();
                        if (strPartId == "" || strSupplierId == "" || strFromTime == "" || strToTime == "")
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = "缺少必填项请完善数据。";
                            dtMessage.Rows.Add(dataRow);
                        }
                        else
                        {
                            if (strPicUrl == "")
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = strPartId + "品番SPIS路径为空请完善数据。";
                                dtMessage.Rows.Add(dataRow);
                            }
                            else
                            {
                                if (!System.IO.File.Exists(strPath + strPicUrl))
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcMessage"] = strPartId + "品番SPIS路径没有图片请重新上传。";
                                    dtMessage.Rows.Add(dataRow);
                                }
                                else
                                {
                                    //dtImport.Rows[i]["vcPicUrlUUID"] = strPartId + strSupplierId + Convert.ToDateTime(strFromTime).ToString("yyyyMMdd") + "." + strPicUrl.Substring(strPicUrl.LastIndexOf('.') + 1).Replace("\"", "").ToLower();// 扩展名
                                    //DataTable dtCheckTime = getSearchInfo(strPartId, strSupplierId);
                                    //if (dtCheckTime.Rows.Count > 0)
                                    //{
                                    //    string strLinid_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["LinId"].ToString();
                                    //    string strFromTime_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["dFromTime"].ToString();
                                    //    string strToTime_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["dToTime"].ToString();
                                    //    if (Convert.ToDateTime(strFromTime_before) >= Convert.ToDateTime(strFromTime))
                                    //    {
                                    //        DataRow dataRow = dtMessage.NewRow();
                                    //        dataRow["vcMessage"] = "品番" + strPartId + "【SPIS区分】维护有误(当前有效的开始使用时间小于维护的开始使用时间)";
                                    //        dtMessage.Rows.Add(dataRow);
                                    //    }
                                    //    strToTime_before = Convert.ToDateTime(strFromTime).AddDays(-1).ToString("yyyy-MM-dd");
                                    //    DataRow drImport = dtImport.NewRow();
                                    //    drImport["LinId"] = strLinid_before;
                                    //    drImport["dToTime"] = strToTime_before;
                                    //    drImport["vcType"] = "mod";
                                    //    dtImport.Rows.Add(drImport);
                                    //}
                                }
                            }
                        }
                    }
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
