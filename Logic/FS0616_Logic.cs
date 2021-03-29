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
using Newtonsoft.Json.Linq;
using NPOI.HSSF.Record;
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

        public DataTable getFormOptions()
        {
            return fs0616_DataAccess.getFormOptions();
        }
        public DataTable getSearchInfo(string strState, List<Object> listOrderNo, string strPartId, string strHaoJiu, string strOrderPlant, string strSupplierId, string strSupplierPlant, string strReplyOverDate, string strOutPutDate)
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
            DataTable dataTable = fs0616_DataAccess.getSearchInfo(strState, strOrderNoList, strPartId, strHaoJiu, strOrderPlant, strSupplierId, strSupplierPlant, strReplyOverDate, strOutPutDate);

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                string decBoxQuantity = dataTable.Rows[i]["decBoxQuantity"].ToString();
                if (decBoxQuantity != "")
                {
                    string strBoxColor = IsInteger(decBoxQuantity);
                    dataTable.Rows[i]["vcBoxColor"] = strBoxColor;
                }
            }

            return dataTable;
        }

        public string IsInteger(string s)
        {
            int i;
            double d;
            if (int.TryParse(s, out i))
                return "0";
            else if (double.TryParse(s, out d))
                return (d == Math.Truncate(d) ? "0" : "1");
            else
                return "0";
        }
        public DataTable setMultipleData(List<Dictionary<string, Object>> listMultipleData, ref DataTable dtMessage)
        {
            try
            {
                //汇集要修改的订单号（订单号+供应商+品番）
                DataTable dtMultiple = fs0603_Logic.createTable("Multipleof616");
                for (int i = 0; i < listMultipleData.Count; i++)
                {
                    string strOrderNo = listMultipleData[i]["vcOrderNo"] == null ? "" : listMultipleData[i]["vcOrderNo"].ToString();
                    string strPart_id = listMultipleData[i]["vcPart_id"] == null ? "" : listMultipleData[i]["vcPart_id"].ToString();
                    string strSupplierId = listMultipleData[i]["vcSupplierId"] == null ? "" : listMultipleData[i]["vcSupplierId"].ToString();
                    if (dtMultiple.Select("vcOrderNo='" + strOrderNo + "' and vcPart_id='" + strPart_id + "' and vcSupplierId='" + strSupplierId + "'").Length == 0)
                    {
                        DataRow drMultiple = dtMultiple.NewRow();
                        drMultiple["vcOrderNo"] = strOrderNo;
                        drMultiple["vcPart_id"] = strPart_id;
                        drMultiple["vcSupplierId"] = strSupplierId;
                        dtMultiple.Rows.Add(drMultiple);
                    }
                }
                return dtMultiple;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public DataTable checkSaveInfo(List<Dictionary<string, Object>> listInfoData, DataTable dtMultiple, ref DataTable dtMessage)
        {
            try
            {
                DataTable dtInfo = fs0603_Logic.createTable("Multipleof616");
                DataTable dtImport = fs0603_Logic.createTable("Multipleof616");
                DataTable dtCheck = fs0603_Logic.createTable("Multipleof616");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    string strOrderNo = listInfoData[i]["vcOrderNo"] == null ? "" : listInfoData[i]["vcOrderNo"].ToString();
                    string strPart_id = listInfoData[i]["vcPart_id"] == null ? "" : listInfoData[i]["vcPart_id"].ToString();
                    string strSupplierId = listInfoData[i]["vcSupplierId"] == null ? "" : listInfoData[i]["vcSupplierId"].ToString();
                    string strPackingQty = listInfoData[i]["iPackingQty"] == null ? "0" : listInfoData[i]["iPackingQty"].ToString();
                    string strOrderQuantity = listInfoData[i]["iOrderQuantity"] == null ? "0" : listInfoData[i]["iOrderQuantity"].ToString();
                    string strDuiYingQuantity = listInfoData[i]["iDuiYingQuantity"] == null ? "0" : listInfoData[i]["iDuiYingQuantity"].ToString();
                    string strDeliveryDate = listInfoData[i]["dDeliveryDate"] == null ? "" : listInfoData[i]["dDeliveryDate"].ToString();
                    string strOutPutDate = listInfoData[i]["dOutPutDate"] == null ? "" : listInfoData[i]["dOutPutDate"].ToString();
                    string strInputType = "company";
                    if (dtMultiple.Select("vcOrderNo='" + strOrderNo + "' and vcPart_id='" + strPart_id + "' and vcSupplierId='" + strSupplierId + "'").Length != 0)
                    {
                        if (strDuiYingQuantity != "0")
                        {
                            DataRow drInfo = dtInfo.NewRow();
                            drInfo["vcOrderNo"] = strOrderNo;
                            drInfo["vcPart_id"] = strPart_id;
                            drInfo["vcSupplierId"] = strSupplierId;
                            drInfo["iPackingQty"] = Convert.ToInt32(strPackingQty);
                            drInfo["iOrderQuantity"] = Convert.ToInt32(strOrderQuantity);
                            drInfo["iDuiYingQuantity"] = Convert.ToInt32(strDuiYingQuantity);
                            drInfo["dDeliveryDate"] = strDeliveryDate;
                            drInfo["dOutPutDate"] = strOutPutDate;
                            drInfo["vcInputType"] = strInputType;
                            dtInfo.Rows.Add(drInfo);
                        }
                    }
                }
                dtImport = checkJude("Save", dtInfo, dtImport, dtCheck, ref dtMessage);
                return dtImport;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable checksubSaveInfo(List<Dictionary<string, Object>> listInfoData, ref DataTable dtMessage)
        {
            try
            {
                DataTable dtInfo = fs0603_Logic.createTable("Multipleof616");
                DataTable dtImport = fs0603_Logic.createTable("Multipleof616");
                DataTable dtCheck = fs0603_Logic.createTable("Multipleof616");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    string strOrderNo = listInfoData[i]["vcOrderNo"] == null ? "" : listInfoData[i]["vcOrderNo"].ToString();
                    string strPart_id = listInfoData[i]["vcPart_id"] == null ? "" : listInfoData[i]["vcPart_id"].ToString();
                    string strSupplierId = listInfoData[i]["vcSupplierId"] == null ? "" : listInfoData[i]["vcSupplierId"].ToString();
                    string strPackingQty = listInfoData[i]["iPackingQty"] == null ? "0" : listInfoData[i]["iPackingQty"].ToString();
                    string strOrderQuantity = listInfoData[i]["iOrderQuantity"] == null ? "0" : listInfoData[i]["iOrderQuantity"].ToString();
                    string strDuiYingQuantity = listInfoData[i]["iDuiYingQuantity"] == null ? "0" : listInfoData[i]["iDuiYingQuantity"].ToString();
                    string strDeliveryDate = listInfoData[i]["dDeliveryDate"] == null ? "" : listInfoData[i]["dDeliveryDate"].ToString();
                    string strOutPutDate = listInfoData[i]["dOutPutDate"] == null ? "" : listInfoData[i]["dOutPutDate"].ToString();
                    string strInputType = "company";
                    //if (dtMultiple.Select("vcOrderNo='" + strOrderNo + "' and vcPart_id='" + strPart_id + "' and vcSupplierId='" + strSupplierId + "'").Length != 0)
                    //{
                    if (strDuiYingQuantity != "0")
                    {
                        DataRow drInfo = dtInfo.NewRow();
                        drInfo["vcOrderNo"] = strOrderNo;
                        drInfo["vcPart_id"] = strPart_id;
                        drInfo["vcSupplierId"] = strSupplierId;
                        drInfo["iPackingQty"] = Convert.ToInt32(strPackingQty);
                        drInfo["iOrderQuantity"] = Convert.ToInt32(strOrderQuantity);
                        drInfo["iDuiYingQuantity"] = Convert.ToInt32(strDuiYingQuantity);
                        drInfo["dDeliveryDate"] = strDeliveryDate;
                        drInfo["dOutPutDate"] = strOutPutDate;
                        drInfo["vcInputType"] = strInputType;
                        dtInfo.Rows.Add(drInfo);
                    }
                    //}
                }
                dtImport = checkJude("Save", dtInfo, dtImport, dtCheck, ref dtMessage);
                return dtImport;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int getManualCode()
        {
            return fs0616_DataAccess.getManualCode();
        }
        public DataTable checkJude(string strChecktype, DataTable dtInfo, DataTable dtImport, DataTable dtCheck, ref DataTable dtMessage)
        {
            try
            {
                //获取推导常量
                int iValue = getManualCode();
                #region//合并数据
                var query = from t in dtInfo.AsEnumerable()
                            group t by new
                            {
                                t1 = t.Field<string>("vcOrderNo"),
                                t2 = t.Field<string>("vcPart_id"),
                                t3 = t.Field<string>("vcSupplierId"),
                                t8 = t.Field<int>("iPackingQty"),
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
                                PackingQty = m.Key.t8,
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
                        drImport["iPackingQty"] = q.PackingQty;
                        drImport["iOrderQuantity"] = q.OrderQuantity;
                        drImport["iDuiYingQuantity"] = q.rowSum;
                        drImport["dDeliveryDate"] = q.DeliveryDate;
                        drImport["dOutPutDate"] = q.OutPutDate;
                        drImport["vcInputType"] = q.InputType;
                        dtImport.Rows.Add(drImport);
                    });
                }
                #endregion
                for (int i = 0; i < dtImport.Rows.Count; i++)
                {
                    string strOrderNo = dtImport.Rows[i]["vcOrderNo"].ToString();
                    string strPart_id = dtImport.Rows[i]["vcPart_id"].ToString();
                    string strSupplierId = dtImport.Rows[i]["vcSupplierId"].ToString();
                    string strPackingQty = dtImport.Rows[i]["iPackingQty"].ToString();
                    string strDeliveryDate = dtImport.Rows[i]["dDeliveryDate"].ToString();
                    string strOutPutDate = dtImport.Rows[i]["dOutPutDate"].ToString();
                    string strDuiYingQuantity = dtImport.Rows[i]["iDuiYingQuantity"].ToString();

                    if (strPackingQty == "0" || strPackingQty == "")
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("订单号{0}，品番{1}，供应商{2}的没有收容数情报", strOrderNo, strPart_id, strSupplierId);
                        dtMessage.Rows.Add(dataRow);
                    }
                    else
                    {
                        decimal input = (Convert.ToDecimal(strDuiYingQuantity) / (Convert.ToDecimal(strPackingQty)));
                        dtImport.Rows[i]["decBoxQuantity"] = input.RoundFirstSignificantDigit().ToString();
                    }
                }
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
                    if (Convert.ToInt32(strDuiYingQuantity) > Convert.ToInt32(strOrderQuantity))
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("订单号{0}，品番{1}，供应商{2}订货总数为{3}个，可对应数量合计大于订货总数", strOrderNo, strPart_id, strSupplierId, strOrderQuantity);
                        dtMessage.Rows.Add(dataRow);
                    }
                }
                #endregion

                if (strChecktype == "Reply")
                {
                    #region //检验纳期出荷日是否为空
                    for (int i = 0; i < dtImport.Rows.Count; i++)
                    {
                        string strOrderNo = dtImport.Rows[i]["vcOrderNo"] == null ? "" : dtImport.Rows[i]["vcOrderNo"].ToString();
                        string strPart_id = dtImport.Rows[i]["vcPart_id"] == null ? "" : dtImport.Rows[i]["vcPart_id"].ToString();
                        string strSupplierId = dtImport.Rows[i]["vcSupplierId"] == null ? "" : dtImport.Rows[i]["vcSupplierId"].ToString();
                        string strDeliveryDate = dtImport.Rows[i]["dDeliveryDate"].ToString();
                        string strOutPutDate = dtImport.Rows[i]["dOutPutDate"].ToString();

                        if (strDeliveryDate == "" && strOutPutDate != "")
                        {
                            strDeliveryDate = getAddDate1(Convert.ToDateTime(strOutPutDate), iValue * -1, false).ToString("yyyy-MM-dd");
                            dtImport.Rows[i]["dDeliveryDate"] = strDeliveryDate;
                        }
                        if (strDeliveryDate != "" && strOutPutDate == "")
                        {
                            strOutPutDate = getAddDate1(Convert.ToDateTime(strDeliveryDate), iValue * 1, false).ToString("yyyy-MM-dd");
                            dtImport.Rows[i]["dOutPutDate"] = strOutPutDate;
                        }

                        if (strOutPutDate == "")
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("订单号{0}，品番{1}，供应商{2}的出荷日为空", strOrderNo, strPart_id, strSupplierId);
                            dtMessage.Rows.Add(dataRow);
                        }
                        else
                        {
                            if (Convert.ToDateTime(strOutPutDate) < Convert.ToDateTime(System.DateTime.Now.ToString("yyyy-MM-dd")))
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = string.Format("订单号{0}，品番{1}，供应商{2}的出荷日小于当前时间", strOrderNo, strPart_id, strSupplierId);
                                dtMessage.Rows.Add(dataRow);
                            }

                            if (strDeliveryDate == "")
                            {
                                strDeliveryDate = getAddDate1(Convert.ToDateTime(strOutPutDate), iValue * -1, false).ToString("yyyy-MM-dd");
                                dtImport.Rows[i]["dDeliveryDate"] = strDeliveryDate;
                            }
                            else
                            {
                                if (Convert.ToDateTime(strDeliveryDate) < Convert.ToDateTime(System.DateTime.Now.ToString("yyyy-MM-dd")))
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcMessage"] = string.Format("订单号{0}，品番{1}，供应商{2}的纳入时间小于当前时间", strOrderNo, strPart_id, strSupplierId);
                                    dtMessage.Rows.Add(dataRow);
                                }
                            }
                        }
                        if (strDeliveryDate != "" && strOutPutDate != "")
                        {
                            if (Convert.ToDateTime(strOutPutDate) < Convert.ToDateTime(strDeliveryDate))
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = string.Format("订单号{0}，品番{1}，供应商{2}的纳入时间大于出荷日", strOrderNo, strPart_id, strSupplierId);
                                dtMessage.Rows.Add(dataRow);
                            }
                        }
                    }
                    #endregion
                }

                return dtImport;

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
        public DataTable checkReplyInfo(List<Dictionary<string, Object>> listInfoData, DataTable dtMultiple, DataTable dataTable, ref DataTable dtMessage)
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
                        string strPackingQty = dataTable.Rows[i]["iPackingQty"] == null ? "0" : dataTable.Rows[i]["iPackingQty"].ToString();
                        string strOrderQuantity = dataTable.Rows[i]["iOrderQuantity"] == null ? "0" : dataTable.Rows[i]["iOrderQuantity"].ToString();
                        string strDuiYingQuantity = dataTable.Rows[i]["iDuiYingQuantity"] == null ? "0" : dataTable.Rows[i]["iDuiYingQuantity"].ToString();
                        string strDeliveryDate = dataTable.Rows[i]["dDeliveryDate"] == null ? "" : dataTable.Rows[i]["dDeliveryDate"].ToString();
                        string strOutPutDate = dataTable.Rows[i]["dOutPutDate"] == null ? "" : dataTable.Rows[i]["dOutPutDate"].ToString();
                        string strInputType = "company";
                        string strState = dataTable.Rows[i]["vcState"] == null ? "" : dataTable.Rows[i]["vcState"].ToString();
                        if (strState == "0" || strState == "1" || strState == "2")
                        {
                            DataRow drInfo = dtInfo.NewRow();
                            drInfo["vcOrderNo"] = strOrderNo;
                            drInfo["vcPart_id"] = strPart_id;
                            drInfo["vcSupplierId"] = strSupplierId;
                            drInfo["iPackingQty"] = Convert.ToInt32(strPackingQty);
                            drInfo["iOrderQuantity"] = Convert.ToInt32(strOrderQuantity);
                            drInfo["iDuiYingQuantity"] = Convert.ToInt32(strDuiYingQuantity);
                            drInfo["dDeliveryDate"] = strDeliveryDate;
                            drInfo["dOutPutDate"] = strOutPutDate;
                            drInfo["vcInputType"] = strInputType;
                            dtInfo.Rows.Add(drInfo);
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
                        string strPackingQty = listInfoData[i]["iPackingQty"] == null ? "0" : listInfoData[i]["iPackingQty"].ToString();
                        string strOrderQuantity = listInfoData[i]["iOrderQuantity"] == null ? "0" : listInfoData[i]["iOrderQuantity"].ToString();
                        string strDuiYingQuantity = listInfoData[i]["iDuiYingQuantity"] == null ? "0" : listInfoData[i]["iDuiYingQuantity"].ToString();
                        string strDeliveryDate = listInfoData[i]["dDeliveryDate"] == null ? "" : listInfoData[i]["dDeliveryDate"].ToString();
                        string strOutPutDate = listInfoData[i]["dOutPutDate"] == null ? "" : listInfoData[i]["dOutPutDate"].ToString();
                        string strInputType = "company";
                        string strState = listInfoData[i]["vcState"] == null ? "" : listInfoData[i]["vcState"].ToString();
                        if (strState == "0" || strState == "1" || strState == "2")
                        {
                            if (dtMultiple.Select("vcOrderNo='" + strOrderNo + "' and vcPart_id='" + strPart_id + "' and vcSupplierId='" + strSupplierId + "'").Length != 0)
                            {
                                DataRow drInfo = dtInfo.NewRow();
                                drInfo["vcOrderNo"] = strOrderNo;
                                drInfo["vcPart_id"] = strPart_id;
                                drInfo["vcSupplierId"] = strSupplierId;
                                drInfo["iPackingQty"] = Convert.ToInt32(strPackingQty);
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
                }
                else
                {
                    dtImport = checkJude("Reply", dtInfo, dtImport, dtCheck, ref dtMessage);
                }
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
        public string setEmailBody(string strReplyOverDate)
        {
            StringBuilder sbr = new StringBuilder();
            sbr.AppendLine("<p>各位供应商殿&nbsp;（请转发给贵司社内相关人员）</p>");
            sbr.AppendLine("<p>非常感谢一直以来对TFTM补给业务的支持！</p>");
            sbr.AppendLine("<p><br></p>");
            sbr.AppendLine("<p>关于标题一事，</p>");
            sbr.AppendLine("<p>紧急订单纳期确认。 </p>");
            sbr.AppendLine("<p>请查收。</p>");
            sbr.AppendLine("<p>回复纳期：<u style=\"color: rgb(230, 0, 0);\">" + strReplyOverDate + "</u>下班前</p><p><br></p><p>请在补给系统上进行调整回复</p>");
            sbr.AppendLine("<p>如有问题，请随时与我联络（联络方式：022-66230666-xxxx）。</p><p><br></p>");
            sbr.AppendLine("<p>以上。</p><p><br></p>");

            return sbr.ToString();
        }
        public DataTable checkOpenInfo(List<Dictionary<string, Object>> listInfoData, DataTable dataTable, string strReplyOverDate, ref DataTable dtMessage)
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
                        string strSupplierPlant = dataTable.Rows[i]["vcSupplierPlant"] == null ? "" : dataTable.Rows[i]["vcSupplierPlant"].ToString();
                        string strState = dataTable.Rows[i]["vcState"] == null ? "" : dataTable.Rows[i]["vcState"].ToString();
                        if (strState == "0")
                        {
                            if (dtImport.Select("vcOrderNo='" + strOrderNo + "' and vcPart_id='" + strPart_id + "' and vcSupplierId='" + strSupplierId + "'").Length == 0)
                            {
                                DataRow drImport = dtImport.NewRow();
                                drImport["vcOrderNo"] = strOrderNo;
                                drImport["vcPart_id"] = strPart_id;
                                drImport["vcSupplierId"] = strSupplierId;
                                drImport["vcSupplierPlant"] = strSupplierPlant;
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
                        string strSupplierPlant = listInfoData[i]["vcSupplierPlant"] == null ? "" : listInfoData[i]["vcSupplierPlant"].ToString();
                        string strState = listInfoData[i]["vcState"] == null ? "" : listInfoData[i]["vcState"].ToString();
                        if (strState == "0")
                        {
                            if (dtImport.Select("vcOrderNo='" + strOrderNo + "' and vcPart_id='" + strPart_id + "' and vcSupplierId='" + strSupplierId + "'").Length == 0)
                            {
                                DataRow drImport = dtImport.NewRow();
                                drImport["vcOrderNo"] = strOrderNo;
                                drImport["vcPart_id"] = strPart_id;
                                drImport["vcSupplierId"] = strSupplierId;
                                drImport["vcSupplierPlant"] = strSupplierPlant;
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
        public DataTable checkOutputInfo(List<Dictionary<string, Object>> listInfoData, DataTable dataTable, string strOutPutDate, ref DataTable dtMessage)
        {
            try
            {
                DataTable dtImport = fs0603_Logic.createTable("Multipleof616");
                if (listInfoData.Count == 0)
                {
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        string strLinId = dataTable.Rows[i]["LinId_sub"] == null ? "" : dataTable.Rows[i]["LinId_sub"].ToString();
                        string strOrderNo = dataTable.Rows[i]["vcOrderNo"] == null ? "" : dataTable.Rows[i]["vcOrderNo"].ToString();
                        string strPart_id = dataTable.Rows[i]["vcPart_id"] == null ? "" : dataTable.Rows[i]["vcPart_id"].ToString();
                        string strSupplierId = dataTable.Rows[i]["vcSupplierId"] == null ? "" : dataTable.Rows[i]["vcSupplierId"].ToString();
                        string strState = dataTable.Rows[i]["vcState"] == null ? "" : dataTable.Rows[i]["vcState"].ToString();
                        if (strState != "3")
                        {
                            if (strLinId != "" && dtImport.Select("LinId='" + strLinId + "'").Length == 0)
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
                        string strLinId = listInfoData[i]["LinId_sub"] == null ? "" : listInfoData[i]["LinId_sub"].ToString();
                        string strOrderNo = listInfoData[i]["vcOrderNo"] == null ? "" : listInfoData[i]["vcOrderNo"].ToString();
                        string strPart_id = listInfoData[i]["vcPart_id"] == null ? "" : listInfoData[i]["vcPart_id"].ToString();
                        string strSupplierId = listInfoData[i]["vcSupplierId"] == null ? "" : listInfoData[i]["vcSupplierId"].ToString();
                        string strState = listInfoData[i]["vcState"] == null ? "" : listInfoData[i]["vcState"].ToString();
                        if (strState != "3")
                        {
                            if (strLinId != "" && dtImport.Select("LinId='" + strLinId + "'").Length == 0)
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
                    dataRow["vcMessage"] = "没有可供一括赋予出荷日的订单数据(可能原因：订单数据未维护可对应数量或者没有未回复销售的数据)";
                    dtMessage.Rows.Add(dataRow);
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
        public DateTime getAddDate(DateTime dateTime, int iday, bool bholiday)
        {
            try
            {
                if (iday > 0)
                {
                    for (int i = 1; i <= iday; i++)
                    {
                        if (!bholiday)
                        {
                            dateTime = dateTime.AddDays(1);
                            while (IsHolidayByDate(dateTime.ToString("yyyyMMdd")))
                            {
                                dateTime = dateTime.AddDays(1);
                            }
                        }
                        else
                        {
                            dateTime = dateTime.AddDays(1);
                        }
                    }
                    return dateTime;
                }
                else if (iday < 0)
                {
                    for (int i = 1; i <= iday * -1; i++)
                    {
                        if (!bholiday)
                        {
                            dateTime = dateTime.AddDays(-1);
                            while (IsHolidayByDate(dateTime.ToString("yyyyMMdd")))
                            {
                                dateTime = dateTime.AddDays(-1);
                            }
                        }
                        else
                        {
                            dateTime = dateTime.AddDays(-1);
                        }
                    }
                    return dateTime;
                }
                else
                {
                    return dateTime;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DateTime getAddDate1(DateTime dateTime, int iday, bool bholiday)
        {
            try
            {
                if (iday > 0)
                {
                    for (int i = 1; i <= iday; i++)
                    {
                        if (!bholiday)
                        {
                            dateTime = dateTime.AddDays(1);
                            int week = caculateWeekDay(dateTime);
                            while (week == 0 || week == 6)
                            {
                                dateTime = dateTime.AddDays(1);
                                week = caculateWeekDay(dateTime);
                            }


                        }
                        else
                        {
                            dateTime = dateTime.AddDays(1);
                        }
                    }
                    return dateTime;
                }
                else if (iday < 0)
                {
                    for (int i = 1; i <= iday * -1; i++)
                    {
                        if (!bholiday)
                        {
                            dateTime = dateTime.AddDays(-1);
                            int week = caculateWeekDay(dateTime);
                            if (week == 0)
                            {
                                dateTime = dateTime.AddDays(-1);
                            }
                            if (week == 6)
                            {
                                dateTime = dateTime.AddDays(-1);
                            }
                        }
                        else
                        {
                            dateTime = dateTime.AddDays(-1);
                        }
                    }
                    return dateTime;
                }
                else
                {
                    return dateTime;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int caculateWeekDay(DateTime dateTime)
        {
            try
            {
                int y = dateTime.Year;
                int m = dateTime.Month;
                int d = dateTime.Day;
                if (m == 1 || m == 2)
                {
                    m += 12;
                    y--;
                }
                int week = (d + 2 * m + 3 * (m + 1) / 5 + y + y / 4 - y / 100 + y / 400 + 1) % 7;
                return week;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 判断是不是节假日,节假日返回true
        /// </summary>
        /// <param name="date">日期格式：yyyyMMdd</param>
        /// <returns></returns>
        public static bool IsHolidayByDate(string date)
        {
            bool isHoliday = false;
            System.Net.WebClient WebClientObj = new System.Net.WebClient();
            System.Collections.Specialized.NameValueCollection PostVars = new System.Collections.Specialized.NameValueCollection();
            PostVars.Add("d", date);//参数
            try
            {

                //  用法举例<br>                 //  检查具体日期是否为节假日，工作日对应结果为 0, 休息日对应结果为 1, 节假日对应的结果为 2；
                //   检查一个日期是否为节假日 http://www.easybots.cn/api/holiday.php?d=20130101
                //  检查多个日期是否为节假日 http://www.easybots.cn/api/holiday.php?d=20130101,20130103,20130105,20130201
                //获取2012年1月份节假日 http://www.easybots.cn/api/holiday.php?m=201201
                //获取2013年1 / 2月份节假日 http://www.easybots.cn/api/holiday.php?m=201301,201302

                byte[] byRemoteInfo = WebClientObj.UploadValues(@"http://www.easybots.cn/api/holiday.php", "POST", PostVars);//请求地址,传参方式,参数集合
                string sRemoteInfo = System.Text.Encoding.UTF8.GetString(byRemoteInfo);//获取返回值

                string result = JObject.Parse(sRemoteInfo)[date].ToString();
                if (result == "0")
                {
                    isHoliday = false;
                }
                else if (result == "1" || result == "2")
                {
                    isHoliday = true;
                }
            }
            catch
            {
                isHoliday = false;
            }
            return isHoliday;
        }
        /// <summary>
        /// 判断是不是周末/节假日
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns>周末和节假日返回true，工作日返回false</returns>
        //public static async Task<bool> IsHolidayByDate(DateTime date)
        //{
        //    var isHoliday = false;
        //    var webClient = new System.Net.WebClient();
        //    var PostVars = new System.Collections.Specialized.NameValueCollection
        //    {
        //        { "d", date.ToString("yyyyMMdd") }//参数
        //    };
        //    try
        //    {
        //        var day = date.DayOfWeek;

        //        //判断是否为周末
        //        if (day == DayOfWeek.Sunday || day == DayOfWeek.Saturday)
        //            return true;

        //        //0为工作日，1为周末，2为法定节假日
        //        var byteResult = await webClient.UploadValuesTaskAsync("http://tool.bitefu.net/jiari/", "POST", PostVars);//请求地址,传参方式,参数集合
        //        var result = Encoding.UTF8.GetString(byteResult);//获取返回值
        //        if (result == "1" || result == "2")
        //            isHoliday = true;
        //    }
        //    catch
        //    {
        //        isHoliday = false;
        //    }
        //    return isHoliday;
        //}

        public DataTable getSearchSubInfo(string strOrderNo, string strPart_id, string strSupplierId)
        {
            DataTable dataTable = fs0616_DataAccess.getSearchSubInfo(strOrderNo, strPart_id, strSupplierId);
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                string decBoxQuantity = dataTable.Rows[i]["decBoxQuantity"].ToString();
                if (decBoxQuantity != "")
                {
                    string strBoxColor = IsInteger(decBoxQuantity);
                    dataTable.Rows[i]["vcBoxColor"] = strBoxColor;
                }
            }

            return dataTable;
        }
    }
    public static class FloatExtension
    {
        public static decimal RoundFirstSignificantDigit(this decimal input)
        {
            if (input==0)
            {
                return 0;
            }
            int precision = 0;
            var val = input;
            while (Math.Abs(val) < 1)
            {
                val *= 10;
                precision++;
            }
            return Math.Round(input, precision);
        }
    }
}
