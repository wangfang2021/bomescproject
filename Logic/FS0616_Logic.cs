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
    public class FS0616_Logic
    {
        private MultiExcute excute = new MultiExcute();
        FS0616_DataAccess fs0616_DataAccess = new FS0616_DataAccess();
        FS0625_Logic fs0625_Logic = new FS0625_Logic();
        FS0603_Logic fs0603_Logic = new FS0603_Logic();

        public DataTable getOrderNoList()
        {
            try
            {
                return fs0616_DataAccess.getOrderNoList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getSearchInfo(string strDelete, string strState, List<Object> listOrderNo, string strPartId, string strOrderPlant, string strInOut, string strHaoJiu, string strSupplierId, string strSupplierPlant)
        {
            string strOrderNoList = "";
            if (listOrderNo.Count != 0)
            {

                strOrderNoList += "select '";
                for (int i = 0; i < listOrderNo.Count; i++)
                {

                    strOrderNoList += listOrderNo[i].ToString();
                    if (i < listOrderNo.Count - 1)
                    {
                        strOrderNoList += "' union select '";
                    }
                    else
                    {
                        strOrderNoList += "'";
                    }
                }
            }
            DataTable dataTable = fs0616_DataAccess.getSearchInfo(strDelete, strState, strOrderNoList, strPartId, strOrderPlant, strInOut, strHaoJiu, strSupplierId, strSupplierPlant);
            return dataTable;
        }

        public DataTable checkSaveInfo(List<Dictionary<string, Object>> listInfoData, DataTable dataTable, ref bool bReault, ref DataTable dtMessage)
        {
            try
            {
                DataTable dtInfo = fs0603_Logic.createTable("Multipleof616");
                DataTable dtImport = fs0603_Logic.createTable("Multipleof616");
                DataTable dtCheck = fs0603_Logic.createTable("Multipleof616");
                #region //整理数据
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    string strOrderNo = listInfoData[i]["vcOrderNo"] == null ? "" : listInfoData[i]["vcOrderNo"].ToString();
                    string strPart_id = listInfoData[i]["vcPart_id"] == null ? "" : listInfoData[i]["vcPart_id"].ToString();
                    string strSupplierId = listInfoData[i]["vcSupplierId"] == null ? "" : listInfoData[i]["vcSupplierId"].ToString();
                    string strOrderQuantity = listInfoData[i]["iOrderQuantity"] == null ? "0" : listInfoData[i]["iOrderQuantity"].ToString();
                    string strDuiYingQuantity = listInfoData[i]["iDuiYingQuantity"] == null ? "0" : listInfoData[i]["iDuiYingQuantity"].ToString();
                    string strDeliveryDate = listInfoData[i]["dDeliveryDate"] == null ? "" : listInfoData[i]["dDeliveryDate"].ToString();
                    string strOutPutDate = listInfoData[i]["dOutPutDate"] == null ? "" : listInfoData[i]["dOutPutDate"].ToString();
                    string strInputType = "company";
                    if (dataTable.Select("vcOrderNo='" + strOrderNo + "' and vcPart_id='" + strPart_id + "' and vcSupplierId='" + strSupplierId + "'").Length != 0)
                    {
                        if (strDuiYingQuantity != "0")
                        {
                            DataRow drInfo = dtInfo.NewRow();
                            drInfo["vcOrderNo"] = strOrderNo;
                            drInfo["vcPart_id"] = strPart_id;
                            drInfo["vcSupplierId"] = strSupplierId;
                            drInfo["iOrderQuantity"] = Convert.ToInt32(strOrderQuantity);
                            drInfo["iDuiYingQuantity"] = Convert.ToInt32(strDuiYingQuantity);
                            drInfo["dDeliveryDate"] = strDeliveryDate;
                            drInfo["dOutPutDate"] = strOutPutDate;
                            drInfo["vcInputType"] = strInputType;
                            dtInfo.Rows.Add(drInfo);
                        }
                    }
                }
                #endregion
                dtImport = checkJude("Save", dtInfo, dtImport, dtCheck, ref dtMessage);
                if (dtMessage == null || dtMessage.Rows.Count == 0)
                    bReault = true;
                else
                    bReault = false;
                return dtImport;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable checkJude(string strChecktype, DataTable dtInfo, DataTable dtImport, DataTable dtCheck, ref DataTable dtMessage)
        {
            try
            {
                #region//合并数据
                var query = from t in dtInfo.AsEnumerable()
                            group t by new
                            {
                                t1 = t.Field<string>("vcOrderNo"),
                                t2 = t.Field<string>("vcPart_id"),
                                t3 = t.Field<string>("vcSupplierId"),
                                t4 = t.Field<int>("iOrderQuantity"),
                                t5 = t.Field<string>("dDeliveryDate"),
                                t6 = t.Field<string>("dOutPutDate"),
                                t7 = t.Field<string>("vcInputType")
                            } into m
                            select new
                            {
                                OrderNo = m.Key.t1,
                                Part_id = m.Key.t2,
                                SupplierId = m.Key.t3,
                                OrderQuantity = m.Key.t4,
                                DeliveryDate = m.Key.t5,
                                OutPutDate = m.Key.t6,
                                InputType = m.Key.t7,
                                rowSum = m.Sum(m => m.Field<int>("iDuiYingQuantity")).ToString()
                            };
                if (query.ToList().Count > 0)
                {
                    query.ToList().ForEach(q =>
                    {
                        DataRow drImport = dtImport.NewRow();
                        drImport["vcOrderNo"] = q.OrderNo;
                        drImport["vcPart_id"] = q.Part_id;
                        drImport["vcSupplierId"] = q.SupplierId;
                        drImport["iOrderQuantity"] = q.OrderQuantity;
                        drImport["iDuiYingQuantity"] = q.rowSum;
                        drImport["dDeliveryDate"] = q.DeliveryDate;
                        drImport["dOutPutDate"] = q.OutPutDate;
                        drImport["vcInputType"] = q.InputType;
                        dtImport.Rows.Add(drImport);
                    });
                }
                #endregion

                #region //校验数据和值
                var check = from t in dtImport.AsEnumerable()
                            group t by new
                            {
                                t1 = t.Field<string>("vcOrderNo"),
                                t2 = t.Field<string>("vcPart_id"),
                                t3 = t.Field<string>("vcSupplierId"),
                                t4 = t.Field<int>("iOrderQuantity"),
                                t5 = t.Field<string>("vcInputType")
                            } into m
                            select new
                            {
                                OrderNo = m.Key.t1,
                                Part_id = m.Key.t2,
                                SupplierId = m.Key.t3,
                                OrderQuantity = m.Key.t4,
                                InputType = m.Key.t5,
                                rowSum = m.Sum(m => m.Field<int>("iDuiYingQuantity"))
                            };
                if (check.ToList().Count > 0)
                {
                    check.ToList().ForEach(q =>
                    {
                        DataRow drCheck = dtCheck.NewRow();
                        drCheck["vcOrderNo"] = q.OrderNo;
                        drCheck["vcPart_id"] = q.Part_id;
                        drCheck["vcSupplierId"] = q.SupplierId;
                        drCheck["iOrderQuantity"] = q.OrderQuantity;
                        drCheck["iDuiYingQuantity"] = q.rowSum;
                        drCheck["vcInputType"] = q.InputType;
                        dtCheck.Rows.Add(drCheck);
                    });
                }
                for (int i = 0; i < dtCheck.Rows.Count; i++)
                {
                    string strOrderNo = dtCheck.Rows[i]["vcOrderNo"] == null ? "" : dtCheck.Rows[i]["vcOrderNo"].ToString();
                    string strPart_id = dtCheck.Rows[i]["vcPart_id"] == null ? "" : dtCheck.Rows[i]["vcPart_id"].ToString();
                    string strSupplierId = dtCheck.Rows[i]["vcSupplierId"] == null ? "" : dtCheck.Rows[i]["vcSupplierId"].ToString();
                    string strOrderQuantity = dtCheck.Rows[i]["iOrderQuantity"] == null ? "" : dtCheck.Rows[i]["iOrderQuantity"].ToString();
                    string strDuiYingQuantity = dtCheck.Rows[i]["iDuiYingQuantity"] == null ? "" : dtCheck.Rows[i]["iDuiYingQuantity"].ToString();
                    if (strDuiYingQuantity != strOrderQuantity)
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("订单号{0}，品番{1}，供应商{2}订货总数为{3}个，可对应数量合计与其不一致", strOrderNo, strPart_id, strSupplierId, strOrderQuantity);
                        dtMessage.Rows.Add(dataRow);
                    }
                }
                #endregion

                if (strChecktype == "Reply")
                {
                    #region //检验纳期是否为空
                    for (int i = 0; i < dtImport.Rows.Count; i++)
                    {
                        string strOrderNo = dtImport.Rows[i]["vcOrderNo"] == null ? "" : dtImport.Rows[i]["vcOrderNo"].ToString();
                        string strPart_id = dtImport.Rows[i]["vcPart_id"] == null ? "" : dtImport.Rows[i]["vcPart_id"].ToString();
                        string strSupplierId = dtImport.Rows[i]["vcSupplierId"] == null ? "" : dtImport.Rows[i]["vcSupplierId"].ToString();
                        string strDeliveryDate = dtImport.Rows[i]["dDeliveryDate"] == null ? "" : dtImport.Rows[i]["dDeliveryDate"].ToString();
                        if (strDeliveryDate == "")
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("订单号{0}，品番{1}，供应商{2}可对应纳期为空，请完善后再处理", strOrderNo, strPart_id, strSupplierId);
                            dtMessage.Rows.Add(dataRow);
                        }
                    }
                    #endregion

                    #region //检验出荷日是否为空
                    for (int i = 0; i < dtImport.Rows.Count; i++)
                    {
                        string strOrderNo = dtImport.Rows[i]["vcOrderNo"] == null ? "" : dtImport.Rows[i]["vcOrderNo"].ToString();
                        string strPart_id = dtImport.Rows[i]["vcPart_id"] == null ? "" : dtImport.Rows[i]["vcPart_id"].ToString();
                        string strSupplierId = dtImport.Rows[i]["vcSupplierId"] == null ? "" : dtImport.Rows[i]["vcSupplierId"].ToString();
                        string strOutPutDate = dtImport.Rows[i]["dOutPutDate"] == null ? "" : dtImport.Rows[i]["dOutPutDate"].ToString();
                        if (strOutPutDate == "")
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("订单号{0}，品番{1}，供应商{2}出荷日为空，请完善后再处理", strOrderNo, strPart_id, strSupplierId);
                            dtMessage.Rows.Add(dataRow);
                        }
                    }
                    #endregion

                    return dtCheck;
                }
                else
                {
                    return dtImport;
                }
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
                fs0616_DataAccess.setSaveInfo(dtImport, strOperId, ref dtMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable checkReplyInfo(List<Dictionary<string, Object>> listInfoData, DataTable dtMultiple, DataTable dataTable, ref bool bReault, ref DataTable dtMessage)
        {
            try
            {
                DataTable dtInfo = fs0603_Logic.createTable("Multipleof616");
                DataTable dtImport = fs0603_Logic.createTable("Multipleof616");
                DataTable dtCheck = fs0603_Logic.createTable("Multipleof616");
                if (dtMultiple.Rows.Count == 0)
                {
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        string strOrderNo = dataTable.Rows[i]["vcOrderNo"] == null ? "" : dataTable.Rows[i]["vcOrderNo"].ToString();
                        string strPart_id = dataTable.Rows[i]["vcPart_id"] == null ? "" : dataTable.Rows[i]["vcPart_id"].ToString();
                        string strSupplierId = dataTable.Rows[i]["vcSupplierId"] == null ? "" : dataTable.Rows[i]["vcSupplierId"].ToString();
                        string strOrderQuantity = dataTable.Rows[i]["iOrderQuantity"] == null ? "0" : dataTable.Rows[i]["iOrderQuantity"].ToString();
                        string strDuiYingQuantity = dataTable.Rows[i]["iDuiYingQuantity"] == null ? "0" : dataTable.Rows[i]["iDuiYingQuantity"].ToString();
                        string strDeliveryDate = dataTable.Rows[i]["dDeliveryDate"] == null ? "" : dataTable.Rows[i]["dDeliveryDate"].ToString();
                        string strOutPutDate = dataTable.Rows[i]["dOutPutDate"] == null ? "" : dataTable.Rows[i]["dOutPutDate"].ToString();
                        string strInputType = "company";
                        string strState = dataTable.Rows[i]["vcState"] == null ? "" : dataTable.Rows[i]["vcState"].ToString();
                        string strDelete = dataTable.Rows[i]["vcDelete"] == null ? "" : dataTable.Rows[i]["vcDelete"].ToString();
                        if ((strState == "1" || strState == "2") && strDelete == "0")
                        {
                            //if (dtImport.Select("vcOrderNo='" + strOrderNo + "' and vcPart_id='" + strPart_id + "' and vcSupplierId='" + strSupplierId + "'").Length == 0)
                            //{
                            DataRow drInfo = dtInfo.NewRow();
                            drInfo["vcOrderNo"] = strOrderNo;
                            drInfo["vcPart_id"] = strPart_id;
                            drInfo["vcSupplierId"] = strSupplierId;
                            drInfo["iOrderQuantity"] = Convert.ToInt32(strOrderQuantity);
                            drInfo["iDuiYingQuantity"] = Convert.ToInt32(strDuiYingQuantity);
                            drInfo["dDeliveryDate"] = strDeliveryDate;
                            drInfo["dOutPutDate"] = strOutPutDate;
                            drInfo["vcInputType"] = strInputType;
                            dtInfo.Rows.Add(drInfo);
                            //}
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < listInfoData.Count; i++)
                    {
                        string strOrderNo = listInfoData[i]["vcOrderNo"] == null ? "" : listInfoData[i]["vcOrderNo"].ToString();
                        string strPart_id = listInfoData[i]["vcPart_id"] == null ? "" : listInfoData[i]["vcPart_id"].ToString();
                        string strSupplierId = listInfoData[i]["vcSupplierId"] == null ? "" : listInfoData[i]["vcSupplierId"].ToString();
                        string strOrderQuantity = listInfoData[i]["iOrderQuantity"] == null ? "0" : listInfoData[i]["iOrderQuantity"].ToString();
                        string strDuiYingQuantity = listInfoData[i]["iDuiYingQuantity"] == null ? "0" : listInfoData[i]["iDuiYingQuantity"].ToString();
                        string strDeliveryDate = listInfoData[i]["dDeliveryDate"] == null ? "" : listInfoData[i]["dDeliveryDate"].ToString();
                        string strOutPutDate = listInfoData[i]["dOutPutDate"] == null ? "" : listInfoData[i]["dOutPutDate"].ToString();
                        string strInputType = "company";
                        string strState = listInfoData[i]["vcState"] == null ? "" : listInfoData[i]["vcState"].ToString();
                        string strDelete = listInfoData[i]["vcDelete"] == null ? "" : listInfoData[i]["vcDelete"].ToString();
                        if ((strState == "1" || strState == "2") && strDelete == "0")
                        {
                            if (dtMultiple.Select("vcOrderNo='" + strOrderNo + "' and vcPart_id='" + strPart_id + "' and vcSupplierId='" + strSupplierId + "'").Length != 0)
                            {
                                DataRow drInfo = dtInfo.NewRow();
                                drInfo["vcOrderNo"] = strOrderNo;
                                drInfo["vcPart_id"] = strPart_id;
                                drInfo["vcSupplierId"] = strSupplierId;
                                drInfo["iOrderQuantity"] = Convert.ToInt32(strOrderQuantity);
                                drInfo["iDuiYingQuantity"] = Convert.ToInt32(strDuiYingQuantity);
                                drInfo["dDeliveryDate"] = strDeliveryDate;
                                drInfo["dOutPutDate"] = strOutPutDate;
                                drInfo["vcInputType"] = strInputType;
                                dtInfo.Rows.Add(drInfo);
                            }
                        }
                    }
                }
                if (dtInfo.Rows.Count == 0)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "没有可供纳期回复的新数据";
                    dtMessage.Rows.Add(dataRow);
                    bReault = false;
                }
                else
                {
                    dtImport = checkJude("Reply", dtInfo, dtImport, dtCheck, ref dtMessage);
                }
                if (dtMessage == null || dtMessage.Rows.Count == 0)
                    bReault = true;
                else
                    bReault = false;
                return dtImport;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setReplyInfo(DataTable dtImport, string strOperId, ref DataTable dtMessage)
        {
            try
            {
                fs0616_DataAccess.setReplyInfo(dtImport, strOperId, ref dtMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable checkOpenInfo(List<Dictionary<string, Object>> listInfoData, DataTable dataTable, string strReplyOverDate, ref bool bReault, ref DataTable dtMessage)
        {
            try
            {
                DataTable dtImport = fs0603_Logic.createTable("Multipleof616");
                if (listInfoData.Count == 0)
                {
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        string strOrderNo = dataTable.Rows[i]["vcOrderNo"] == null ? "" : dataTable.Rows[i]["vcOrderNo"].ToString();
                        string strPart_id = dataTable.Rows[i]["vcPart_id"] == null ? "" : dataTable.Rows[i]["vcPart_id"].ToString();
                        string strSupplierId = dataTable.Rows[i]["vcSupplierId"] == null ? "" : dataTable.Rows[i]["vcSupplierId"].ToString();
                        string strState = dataTable.Rows[i]["vcState"] == null ? "" : dataTable.Rows[i]["vcState"].ToString();
                        string strDelete = dataTable.Rows[i]["vcDelete"] == null ? "" : dataTable.Rows[i]["vcDelete"].ToString();
                        if (strState == "0" && strDelete == "0")
                        {
                            if (dtImport.Select("vcOrderNo='" + strOrderNo + "' and vcPart_id='" + strPart_id + "' and vcSupplierId='" + strSupplierId + "'").Length == 0)
                            {
                                DataRow drImport = dtImport.NewRow();
                                drImport["vcOrderNo"] = strOrderNo;
                                drImport["vcPart_id"] = strPart_id;
                                drImport["vcSupplierId"] = strSupplierId;
                                drImport["dReplyOverDate"] = strReplyOverDate;
                                dtImport.Rows.Add(drImport);
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < listInfoData.Count; i++)
                    {
                        string strOrderNo = listInfoData[i]["vcOrderNo"] == null ? "" : listInfoData[i]["vcOrderNo"].ToString();
                        string strPart_id = listInfoData[i]["vcPart_id"] == null ? "" : listInfoData[i]["vcPart_id"].ToString();
                        string strSupplierId = listInfoData[i]["vcSupplierId"] == null ? "" : listInfoData[i]["vcSupplierId"].ToString();
                        string strState = listInfoData[i]["vcState"] == null ? "" : listInfoData[i]["vcState"].ToString();
                        string strDelete = listInfoData[i]["vcDelete"] == null ? "" : listInfoData[i]["vcDelete"].ToString();
                        if (strState == "0" && strDelete == "0")
                        {
                            if (dtImport.Select("vcOrderNo='" + strOrderNo + "' and vcPart_id='" + strPart_id + "' and vcSupplierId='" + strSupplierId + "'").Length == 0)
                            {
                                DataRow drImport = dtImport.NewRow();
                                drImport["vcOrderNo"] = strOrderNo;
                                drImport["vcPart_id"] = strPart_id;
                                drImport["vcSupplierId"] = strSupplierId;
                                drImport["dReplyOverDate"] = strReplyOverDate;
                                dtImport.Rows.Add(drImport);
                            }
                        }
                    }
                }
                if (dtImport.Rows.Count == 0)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "没有可供纳期确认的新数据";
                    dtMessage.Rows.Add(dataRow);
                    bReault = false;
                }
                return dtImport;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setOpenInfo(DataTable dtImport, string strOperId, ref DataTable dtMessage)
        {
            try
            {
                fs0616_DataAccess.setOpenInfo(dtImport, strOperId, ref dtMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable checkOutputInfo(List<Dictionary<string, Object>> listInfoData, DataTable dataTable, string strOutPutDate, ref bool bReault, ref DataTable dtMessage)
        {
            try
            {
                DataTable dtImport = fs0603_Logic.createTable("Multipleof616");
                if (listInfoData.Count == 0)
                {
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        string strLinId = dataTable.Rows[i]["LinId"] == null ? "" : dataTable.Rows[i]["LinId"].ToString();
                        string strOrderNo = dataTable.Rows[i]["vcOrderNo"] == null ? "" : dataTable.Rows[i]["vcOrderNo"].ToString();
                        string strPart_id = dataTable.Rows[i]["vcPart_id"] == null ? "" : dataTable.Rows[i]["vcPart_id"].ToString();
                        string strSupplierId = dataTable.Rows[i]["vcSupplierId"] == null ? "" : dataTable.Rows[i]["vcSupplierId"].ToString();
                        string strState = dataTable.Rows[i]["vcState"] == null ? "" : dataTable.Rows[i]["vcState"].ToString();
                        string strDelete = dataTable.Rows[i]["vcDelete"] == null ? "" : dataTable.Rows[i]["vcDelete"].ToString();
                        if (strDelete == "0" && strState != "3")
                        {
                            if (dtImport.Select("LinId='" + strLinId + "'").Length == 0)
                            {
                                DataRow drImport = dtImport.NewRow();
                                drImport["LinId"] = strLinId;
                                drImport["vcOrderNo"] = strOrderNo;
                                drImport["vcPart_id"] = strPart_id;
                                drImport["vcSupplierId"] = strSupplierId;
                                drImport["dOutPutDate"] = strOutPutDate;
                                dtImport.Rows.Add(drImport);
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < listInfoData.Count; i++)
                    {
                        string strLinId = listInfoData[i]["LinId"] == null ? "" : listInfoData[i]["LinId"].ToString();
                        string strOrderNo = listInfoData[i]["vcOrderNo"] == null ? "" : listInfoData[i]["vcOrderNo"].ToString();
                        string strPart_id = listInfoData[i]["vcPart_id"] == null ? "" : listInfoData[i]["vcPart_id"].ToString();
                        string strSupplierId = listInfoData[i]["vcSupplierId"] == null ? "" : listInfoData[i]["vcSupplierId"].ToString();
                        string strState = listInfoData[i]["vcState"] == null ? "" : listInfoData[i]["vcState"].ToString();
                        string strDelete = listInfoData[i]["vcDelete"] == null ? "" : listInfoData[i]["vcDelete"].ToString();
                        if (strDelete == "0" && strState != "3")
                        {
                            if (dtImport.Select("LinId='" + strLinId + "'").Length == 0)
                            {
                                DataRow drImport = dtImport.NewRow();
                                drImport["LinId"] = strLinId;
                                drImport["vcOrderNo"] = strOrderNo;
                                drImport["vcPart_id"] = strPart_id;
                                drImport["vcSupplierId"] = strSupplierId;
                                drImport["dOutPutDate"] = strOutPutDate;
                                dtImport.Rows.Add(drImport);
                            }
                        }
                    }
                }
                if (dtImport.Rows.Count == 0)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "没有可供一括赋予的新数据";
                    dtMessage.Rows.Add(dataRow);
                    bReault = false;
                }
                return dtImport;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setOutputInfo(DataTable dtImport, string strOperId, ref DataTable dtMessage)
        {
            try
            {
                fs0616_DataAccess.setOutputInfo(dtImport, strOperId, ref dtMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
