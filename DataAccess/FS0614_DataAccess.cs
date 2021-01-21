using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using System.Data;
using System.IO;
using System.Text;
using CrystalDecisions.Shared;
using NPOI.OpenXmlFormats.Dml;

namespace DataAccess
{
    public class FS0614_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 检索

        public DataTable searchApi()
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine(" SELECT vcOrderNo,dTargetDate,vcOrderType,vcOrderState,vcMemo,dUploadDate,dCreateDate ");
                sbr.AppendLine(" FROM TOrderUploadManage ");
                sbr.AppendLine("");
                sbr.AppendLine("");
                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 撤销

        public void cancelFile(List<Dictionary<string, Object>> list, string strUserId)
        {

        }
        #endregion

        #region 生成
        public bool CreateOrder(string type, string inout, string path, string userId, ref string msg)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                Order order = GetPartFromFile(path, ref msg);
                string vcPackingFactory = getFactory();

                if (!string.IsNullOrWhiteSpace(msg))
                {
                    return false;
                }

                //月度
                if (type == "2")
                {
                    //判断头

                    if (order.Head.Type != "3")
                    {
                        msg = "订单类型不正确";
                        return false;
                    }

                    DateTime Date = Convert.ToDateTime(DateTime.ParseExact(order.Head.Date, "yyyyMMdd", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces).ToString("yyyy-MM-dd"));
                    string vcTargetYearMonth = Date.AddMonths(1).ToString("yyyyMM");

                    DataTable dockTmp = getDockTable(vcPackingFactory, inout);
                    DataTable SoqDt = getSoqDt(inout, vcTargetYearMonth, "1");
                    foreach (Detail detail in order.Details)
                    {
                        string tmp = "";
                        //DateTime Date = Convert.ToDateTime(DateTime.ParseExact(detail.Date, "yyyyMMdd", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces).ToString("yyyy-MM-dd"));
                        //string vcTargetYearMonth = Date.AddMonths(1).ToString("yyyyMM");
                        string vcPart_id = detail.PartsNo.Trim();
                        string CPD = detail.CPD.Trim();
                        string vcOrderNo = detail.Type.Trim();
                        string vcSeqno = detail.ItemNo.Trim();
                        string dateTime = detail.Date.Trim();
                        string vcDock = "";
                        string vcSupplierId = "";
                        string vcCarType = "";
                        string vcPartId_Replace = "";
                        string Day = Convert.ToInt32(dateTime.Substring(6, 2)).ToString();
                        string Field = "vcPlantQtyDaily" + Day;
                        //检测品番表是否存在该品番
                        Hashtable hashtable = getDock(vcPart_id, CPD, dockTmp);
                        if (hashtable.Keys.Count > 0)
                        {
                            vcDock = hashtable["vcSufferIn"].ToString();
                            vcSupplierId = hashtable["vcSupplierId"].ToString();
                            vcCarType = hashtable["vcCarfamilyCode"].ToString();
                            vcPartId_Replace = hashtable["vcPartId_Replace"].ToString();
                        }
                        else
                        {
                            tmp += "品番基础数据表不包含品番" + vcPart_id + ";\r\n";
                        }

                        //检测数量
                        if (!CheckTotalNumEqual(inout, vcTargetYearMonth, vcPart_id, SoqDt, detail.QTY))
                        {
                            tmp += "品番" + vcPart_id + "订单数量不正确;\r\n";
                        }

                        if (!string.IsNullOrWhiteSpace(tmp))
                        {
                            msg += tmp;
                            continue;
                        }

                        sbr.Append(" INSERT INTO SP_M_ORD(vcPackingFactory,vcTargetYearMonth,vcDock,vcCpdcompany,vcOrderType,vcOrderNo,vcSeqno,dOrderDate,dOrderExportDate,vcPartNo,vcInsideOutsideType,vcCarType,vcLastPartNo,vcSupplier_id,vcOperatorID,dOperatorTime, ");
                        sbr.Append(Field + ") VALUES");
                        sbr.Append("(");
                        sbr.Append(ComFunction.getSqlValue(vcPackingFactory, false) + ",");
                        sbr.Append(ComFunction.getSqlValue(vcTargetYearMonth, false) + ",");
                        sbr.Append(ComFunction.getSqlValue(vcDock, false) + ",");
                        sbr.Append(ComFunction.getSqlValue(CPD, false) + ",");
                        sbr.Append("'" + type + "',");
                        sbr.Append(ComFunction.getSqlValue(vcOrderNo, false) + ",");
                        sbr.Append(ComFunction.getSqlValue(vcSeqno, false) + ",");
                        sbr.Append(ComFunction.getSqlValue(detail.Date, true) + ",");
                        sbr.Append("GETDATE(),");
                        sbr.Append(ComFunction.getSqlValue(vcPart_id, false) + ",");
                        sbr.Append(ComFunction.getSqlValue(inout, false) + ",");
                        sbr.Append(ComFunction.getSqlValue(vcCarType, false) + ",");
                        sbr.Append(ComFunction.getSqlValue(vcPartId_Replace, false) + ",");
                        sbr.Append(ComFunction.getSqlValue(vcSupplierId, false) + ",");
                        sbr.Append(ComFunction.getSqlValue(userId, false) + ",");
                        sbr.Append("GETDATE(),");
                        sbr.Append(ComFunction.getSqlValue(Convert.ToInt32(detail.QTY), false));
                        sbr.Append(") \r\n");
                    }
                }
                //日度
                else if (type == "0")
                {

                }
                //紧急
                else if (type == "3")
                {

                }

                if (sbr.Length > 0 && string.IsNullOrWhiteSpace(msg))
                {
                    excute.ExcuteSqlWithStringOper(sbr.ToString());
                    return true;
                }

                if (string.IsNullOrWhiteSpace(msg))
                {
                    msg = "做成订单失败";
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 读取txt

        public Order GetPartFromFile(string path, ref string msg)
        {
            string[] strs = File.ReadAllLines(@path);
            Order order = new Order();

            Head head = new Head();
            List<Detail> details = new List<Detail>();
            Tail tail = new Tail();

            //获取Detail
            for (int i = 0; i < strs.Length; i++)
            {
                string temp = strs[i];
                //判断空行
                if (string.IsNullOrWhiteSpace(temp))
                {
                    continue;
                }
                //获取Head
                if (temp[0] == 'D')
                {
                    Detail detail = new Detail();
                    detail.DataId = temp.Substring(0, 1);
                    detail.CPD = temp.Substring(1, 5);
                    detail.Date = temp.Substring(6, 8);
                    detail.Type = temp.Substring(14, 8);
                    detail.ItemNo = temp.Substring(22, 4);
                    detail.PartsNo = temp.Substring(26, 12);
                    detail.QTY = temp.Substring(41, 7);
                    detail.Price = temp.Substring(48, 9);
                    details.Add(detail);
                }
                else if (temp[0] == 'H')
                {
                    head.DataId = temp.Substring(0, 1);
                    head.CPD = temp.Substring(1, 5);
                    head.Date = temp.Substring(6, 8);
                    head.No = temp.Substring(14, 8);
                    head.Type = temp.Substring(22, 1);
                    head.SendDate = temp.Substring(28, 8);
                }
                else if (temp[0] == 'T')
                {
                    tail.DataId = temp.Substring(0, 1);
                    tail.CPD = temp.Substring(1, 5);
                    tail.Date = temp.Substring(6, 8);
                    tail.No = temp.Substring(14, 8);
                }
            }

            order.Head = head;
            order.Details = details;
            order.Tail = tail;

            if (order.Head == null)
            {
                msg = "订单Head部分有误";
                return null;
            }
            else if (order.Details.Count == 0)
            {
                msg = "订单Detail为空";
                return null;
            }
            else if (order.Tail == null)
            {
                msg = "订单Tail部分有误";
                return null;
            }

            return order;
        }

        public class Order
        {
            public Order()
            {
                this.Details = new List<Detail>();
            }

            public Head Head;
            public List<Detail> Details;
            public Tail Tail;
        }

        public class Head
        {
            public string DataId;
            public string CPD;
            public string Date;
            public string No;
            public string Type;
            public string Code;
            public string SendDate;
        }

        public class Detail
        {
            public string DataId;
            public string CPD;
            public string Date;
            public string Type;
            public string ItemNo;
            public string PartsNo;
            public string QTY;
            public string Price;

        }

        public class Tail
        {
            public string DataId;
            public string CPD;
            public string Date;
            public string No;
        }


        #endregion

        #region 获取soq数量

        public DataTable getSoqDt(string inoutFlag, string TargetYM, string orderType)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine(" SELECT vcPart_id,iPartNums,iD1,iD2,iD3,iD4,iD5,iD6,iD7,iD8,iD9,iD10,iD11,iD12,iD13,iD14,iD15,iD16,iD17,iD18,iD19,iD20,iD21,iD22,iD23,iD24,iD25,iD26,iD27,iD28,iD29,iD30,iD31 FROM TSOQReply");
                sbr.AppendLine(" WHERE ");
                sbr.AppendLine(" vcInOutFlag = '" + inoutFlag + "' ");
                sbr.AppendLine(" AND vcDXYM = '" + TargetYM + "' ");
                sbr.AppendLine(" AND vcMakingOrderType = '" + orderType + "'");
                DataTable dt = excute.ExcuteSqlWithSelectToDT(sbr.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Hashtable getSoqNumMonth(string partId, DataTable dt)
        {
            Hashtable hashtable = new Hashtable();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (partId.Trim().Equals(dt.Rows[i]["vcPart_id"].ToString().Trim()))
                {
                    hashtable.Add("Total", dt.Rows[i]["iPartNums"].ToString());
                    hashtable.Add("iD1", dt.Rows[i]["iD1"].ToString());
                    hashtable.Add("iD2", dt.Rows[i]["iD2"].ToString());
                    hashtable.Add("iD3", dt.Rows[i]["iD3"].ToString());
                    hashtable.Add("iD4", dt.Rows[i]["iD4"].ToString());
                    hashtable.Add("iD5", dt.Rows[i]["iD5"].ToString());
                    hashtable.Add("iD6", dt.Rows[i]["iD6"].ToString());
                    hashtable.Add("iD7", dt.Rows[i]["iD7"].ToString());
                    hashtable.Add("iD8", dt.Rows[i]["iD8"].ToString());
                    hashtable.Add("iD9", dt.Rows[i]["iD9"].ToString());
                    hashtable.Add("iD10", dt.Rows[i]["iD10"].ToString());
                    hashtable.Add("iD11", dt.Rows[i]["iD11"].ToString());
                    hashtable.Add("iD12", dt.Rows[i]["iD12"].ToString());
                    hashtable.Add("iD13", dt.Rows[i]["iD13"].ToString());
                    hashtable.Add("iD14", dt.Rows[i]["iD14"].ToString());
                    hashtable.Add("iD15", dt.Rows[i]["iD15"].ToString());
                    hashtable.Add("iD16", dt.Rows[i]["iD16"].ToString());
                    hashtable.Add("iD17", dt.Rows[i]["iD17"].ToString());
                    hashtable.Add("iD18", dt.Rows[i]["iD18"].ToString());
                    hashtable.Add("iD19", dt.Rows[i]["iD19"].ToString());
                    hashtable.Add("iD20", dt.Rows[i]["iD20"].ToString());
                    hashtable.Add("iD21", dt.Rows[i]["iD21"].ToString());
                    hashtable.Add("iD22", dt.Rows[i]["iD22"].ToString());
                    hashtable.Add("iD23", dt.Rows[i]["iD23"].ToString());
                    hashtable.Add("iD24", dt.Rows[i]["iD24"].ToString());
                    hashtable.Add("iD25", dt.Rows[i]["iD25"].ToString());
                    hashtable.Add("iD26", dt.Rows[i]["iD26"].ToString());
                    hashtable.Add("iD27", dt.Rows[i]["iD27"].ToString());
                    hashtable.Add("iD28", dt.Rows[i]["iD28"].ToString());
                    hashtable.Add("iD29", dt.Rows[i]["iD29"].ToString());
                    hashtable.Add("iD30", dt.Rows[i]["iD30"].ToString());
                    hashtable.Add("iD31", dt.Rows[i]["iD31"].ToString());
                    break;
                }
            }
            return hashtable;

        }

        public bool CheckTotalNumEqual(string inoutFlag, string TargetYM, string partId, DataTable dt, string total)
        {
            try
            {
                bool flag = false;

                int totalCheck = Convert.ToInt32(getSoqNumMonth(partId, dt)["Total"].ToString());

                int totalC = Convert.ToInt32(total);

                if (totalC == totalCheck)
                {
                    flag = true;
                }

                return flag;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion


        #region 获取Dock

        public Hashtable getDock(string PartID, string receiver, DataTable dt)
        {
            try
            {
                Hashtable hashtable = new Hashtable();

                if (dt.Rows.Count > 0)
                {

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (PartID.Trim().Equals(dt.Rows[i]["vcPartId"].ToString().Trim()) && receiver.Trim().Equals(dt.Rows[i]["vcReceiver"].ToString().Trim()))
                        {
                            hashtable.Add("vcPartId_Replace", dt.Rows[i]["vcPartId_Replace"].ToString());
                            hashtable.Add("vcSupplierId", dt.Rows[i]["vcSupplierId"].ToString());
                            hashtable.Add("vcCarfamilyCode", dt.Rows[i]["vcCarfamilyCode"].ToString());
                            hashtable.Add("vcSufferIn", dt.Rows[i]["vcSufferIn"].ToString());
                            break;
                        }
                    }
                }

                return hashtable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public Hashtable getDock(string PartID, string receiver, string packingPlant, string Inout, DataTable alldt)
        //{
        //    try
        //    {
        //        Hashtable hashtable = new Hashtable();
        //        StringBuilder sbr = new StringBuilder();
        //        sbr.AppendLine("SELECT a.vcPartId_Replace,a.vcSupplierId,a.vcCarfamilyCode,b.vcSufferIn FROM ");
        //        sbr.AppendLine("(");
        //        sbr.AppendLine("	SELECT vcSupplierId,vcCarfamilyCode,vcPackingPlant,vcPartId,vcReceiver,vcPartId_Replace FROM TSPMaster WHERE ");
        //        sbr.AppendLine("	vcPartId = '" + PartID + "' ");
        //        sbr.AppendLine("	AND vcReceiver = '" + receiver + "' ");
        //        sbr.AppendLine("	AND vcPackingPlant = '" + packingPlant + "' ");
        //        sbr.AppendLine("	AND vcInOut = '" + Inout + "'");
        //        sbr.AppendLine("	AND dFromTime <= GETDATE() AND dToTime >= GETDATE() ");
        //        sbr.AppendLine(") a");
        //        sbr.AppendLine("LEFT JOIN");
        //        sbr.AppendLine("(");
        //        sbr.AppendLine("SELECT vcPackingPlant,vcPartId,vcReceiver,vcSupplierId,vcSufferIn FROM TSPMaster_SufferIn WHERE dFromTime <= GETDATE() AND dToTime >= GETDATE() AND vcOperatorType = '1' ");
        //        sbr.AppendLine(") b ON a.vcPackingPlant = b.vcPackingPlant AND a.vcPartId = b.vcPartId AND a.vcReceiver = b.vcReceiver AND a.vcSupplierId = b.vcSupplierId");


        //        DataTable dt = excute.ExcuteSqlWithSelectToDT(sbr.ToString());

        //        if (dt.Rows.Count > 0)
        //        {

        //            hashtable.Add("vcPartId_Replace", dt.Rows[0]["vcPartId_Replace"].ToString());
        //            hashtable.Add("vcSupplierId", dt.Rows[0]["vcSupplierId"].ToString());
        //            hashtable.Add("vcCarfamilyCode", dt.Rows[0]["vcCarfamilyCode"].ToString());
        //            hashtable.Add("vcSufferIn", dt.Rows[0]["vcSufferIn"].ToString());
        //        }

        //        return hashtable;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public DataTable getDockTable(string packingPlant, string Inout)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine("SELECT a.vcPartId,a.vcPartId_Replace,a.vcSupplierId,a.vcCarfamilyCode,a.vcReceiver,b.vcSufferIn FROM ");
                sbr.AppendLine("(");
                sbr.AppendLine("	SELECT vcSupplierId,vcCarfamilyCode,vcPackingPlant,vcPartId,vcReceiver,vcPartId_Replace FROM TSPMaster WHERE ");
                sbr.AppendLine("	vcPackingPlant = '" + packingPlant + "' ");
                sbr.AppendLine("	AND vcInOut = '" + Inout + "'");
                sbr.AppendLine("	AND dFromTime <= GETDATE() AND dToTime >= GETDATE() ");
                sbr.AppendLine(") a");
                sbr.AppendLine("LEFT JOIN");
                sbr.AppendLine("(");
                sbr.AppendLine("SELECT vcPackingPlant,vcPartId,vcReceiver,vcSupplierId,vcSufferIn FROM TSPMaster_SufferIn WHERE dFromTime <= GETDATE() AND dToTime >= GETDATE() AND vcOperatorType = '1' ");
                sbr.AppendLine(") b ON a.vcPackingPlant = b.vcPackingPlant AND a.vcPartId = b.vcPartId AND a.vcReceiver = b.vcReceiver AND a.vcSupplierId = b.vcSupplierId");

                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 获取包装工厂

        public string getFactory()
        {
            return "TFTM";
        }

        #endregion

    }
}