using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using Microsoft.Office.Interop.Excel;
using DataTable = System.Data.DataTable;


namespace DataAccess
{
    public class FS0614_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        private bool bReault;

        #region 检索
        public DataTable searchApi(string orderState, string targetYM, string orderNo, string vcOrderType, string dUpload, string memo)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine(
                    "SELECT a.iAutoId,a.vcOrderNo,a.vcTargetYear+a.vcTargetMonth+a.vcTargetDay AS vcTargetYM,c.vcOrderDifferentiation as vcOrderType,b.vcName as vcOrderState,a.vcMemo,a.dUploadDate,a.dCreateDate,a.vcFilePath FROM ");
                sbr.AppendLine("(");
                sbr.AppendLine(
                    "SELECT iAutoId,vcOrderNo,vcTargetYear,vcTargetMonth,vcTargetDay,vcOrderType,vcOrderState,vcMemo,dUploadDate,dCreateDate,vcFilePath");
                sbr.AppendLine("FROM TOrderUploadManage  WHERE vcOrderShowFlag='1' ");
                sbr.AppendLine(") a");
                sbr.AppendLine("LEFT JOIN ");
                sbr.AppendLine("(");
                sbr.AppendLine("SELECT vcName,vcValue FROM TCode WHERE vcCodeId = 'C044'");
                sbr.AppendLine(") b ON a.vcOrderState = b.vcValue");
                sbr.AppendLine("LEFT JOIN ");
                sbr.AppendLine("(");
                sbr.AppendLine("SELECT vcOrderDifferentiation,vcOrderInitials FROM TOrderDifferentiation");
                sbr.AppendLine(") c ON a.vcOrderType = c.vcOrderInitials");
                sbr.AppendLine("WHERE 1=1");
                if (!string.IsNullOrWhiteSpace(orderState))
                {
                    sbr.AppendLine("AND a.vcOrderState = '" + orderState + "'");
                }

                if (!string.IsNullOrWhiteSpace(targetYM))
                {
                    sbr.AppendLine("AND a.vcTargetYear+a.vcTargetMonth = '" + targetYM + "'");
                }

                if (!string.IsNullOrWhiteSpace(orderNo))
                {
                    sbr.AppendLine("AND a.vcOrderNo like '" + orderNo + "%'");
                }

                if (!string.IsNullOrWhiteSpace(vcOrderType))
                {
                    sbr.AppendLine("AND a.vcOrderType = '" + vcOrderType + "'");
                }

                if (!string.IsNullOrWhiteSpace(dUpload))
                {
                    sbr.AppendLine("AND Convert(varchar(10),a.dUploadDate,120) = " + ComFunction.getSqlValue(dUpload, true) + "");
                }

                if (!string.IsNullOrWhiteSpace(memo))
                {
                    sbr.AppendLine("AND vcMemo LIKE '%" + memo + "%'");
                }
                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region 生成
        public bool CreateOrder(List<Dictionary<string, Object>> listInfoData, string path, string userId, string uionCode, ref bool bReault, ref DataTable dtMessage)
        {
            try
            {
                FS0403_DataAccess fs0403_dataAccess = new FS0403_DataAccess();
                List<string> TargetYM = new List<string>();

                for (int i = 0; i < listInfoData.Count; i++)
                {
                    string tmp = ObjToString(listInfoData[i]["vcTargetYM"]);
                    if (!string.IsNullOrWhiteSpace(tmp))
                    {
                        TargetYM.Add(tmp.Substring(0, 6));

                    }
                }
                TargetYM = TargetYM.Distinct().ToList();
                //获取基础信息
                DataTable dockTmp = getDockTable();
                //获取soq验收数量
                DataTable SoqDt = new DataTable();
                DataRow[] drArrayN = null; //0 內制 1外制
                DataRow[] drArrayW = null; //0 內制 1外制
                DataRow[] drArrayTmp = null;
                if (TargetYM.Count > 0)
                {
                    //获取soq验收数量
                    SoqDt = getSoqDt(TargetYM);

                }



                //获取包装厂
                DataTable PackSpot = getPackSpot();
                //获取包装工厂
                string vcPackingFactory = uionCode;
                //获取订单号
                DataTable OrderNo = getOrderNo();
                //获取纳期
                DataTable OrderNQ = getNQ();


                //读取文件
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    string msg = "";
                    //获取类型
                    string Type = getOrderType(listInfoData[i]["vcOrderType"].ToString());

                    //读取Order
                    Order order = GetPartFromFile(path + listInfoData[i]["vcFilePath"].ToString(), listInfoData[i]["vcOrderNo"].ToString(), ref msg);
                    string vcOrderNo = order.Head.No;

                    StringBuilder sbr = new StringBuilder();

                    if (Type.Equals("S"))
                    {
                        string vcMakingOrderType = getTypeMethod(Type);
                        drArrayN = SoqDt.Select("vcInOutFlag='0' AND vcMakingOrderType in (" + vcMakingOrderType + ")"); //0 內制 1外制
                        drArrayW = SoqDt.Select("vcInOutFlag='1' AND vcMakingOrderType in (" + vcMakingOrderType + ")"); //0 內制 1外制
                        drArrayTmp = drArrayN;

                        string OrderTargetYM = listInfoData[i]["vcTargetYM"].ToString().Substring(0, 6);
                        string TargetTmp = ObjToString(listInfoData[i]["vcTargetYM"]);
                        //DateTime Time = DateTime.ParseExact(TargetTmp.Substring(0, 6), "yyyyMM", System.Globalization.CultureInfo.CurrentCulture);
                        DateTime Time = DateTime.Parse(TargetTmp.Substring(0, 4) + "-" + TargetTmp.Substring(4, 2) + "-01");
                        DateTime LastTime = Time.AddMonths(1).AddDays(-1);

                        #region 月度校验
                        int No = 1;
                        string strInOutflag = string.Empty;
                        Dictionary<string, string> dicPartNo = new Dictionary<string, string>();
                        foreach (Detail detail in order.Details)
                        {
                            string tmp = "";
                            string vcPart_id = detail.PartsNo.Trim();
                            string CPD = detail.CPD.Trim();
                            string vcSeqno = detail.ItemNo.Trim();
                            string vcSupplierId = "";
                            string inout = "";
                            string vcSupplierPlant = "";//工区
                            string vcSupplierPlace = "";//出荷场
                            string vcOrderNum = detail.QTY;
                            string vcOrderingMethod = "";

                            string dateTime = detail.Date.Trim();
                            string Day = Convert.ToInt32(dateTime.Substring(6, 2)).ToString();
                            //检测品番表是否存在该品番
                            Hashtable hashtable = getDock(vcPart_id, CPD, vcPackingFactory, dockTmp);
                            if (hashtable.Keys.Count > 0)
                            {
                                inout = ObjToString(hashtable["vcInOut"]);
                                vcSupplierId = ObjToString(hashtable["vcSupplierId"]);
                                vcSupplierPlant = ObjToString(hashtable["vcSupplierPlant"]);
                                vcSupplierPlace = ObjToString(hashtable["vcSupplierPlace"]);
                                vcOrderingMethod = ObjToString(hashtable["vcOrderingMethod"]);


                                //验证品番个数 及其总数 以及内外区分
                                if (No == 1)
                                {
                                    strInOutflag = inout;
                                    if (inout == "1")
                                    {
                                        drArrayTmp = drArrayW;
                                    }
                                }
                                else
                                {
                                    if (inout != strInOutflag) //判定此次文件的内外区分是否一致
                                    {
                                        DataRow dataRow = dtMessage.NewRow();
                                        dataRow["vcOrder"] = vcOrderNo;
                                        dataRow["vcPartNo"] = vcPart_id;
                                        dataRow["vcMessage"] = vcOrderNo + "文件中的品番" + vcPart_id + "与第一个品番的内外区分不一样";
                                        dtMessage.Rows.Add(dataRow);
                                        bReault = false;
                                    }
                                }
                                // 判定品番是否在临时品番里面
                                bool isExist = false;
                                for (int m = 0; m < drArrayTmp.Length; m++)
                                {
                                    if (drArrayTmp[m]["vcPart_id"].ToString() == vcPart_id)
                                    {
                                        isExist = true;
                                        break;
                                    }
                                }
                                if (!isExist)
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcOrder"] = vcOrderNo;
                                    dataRow["vcPartNo"] = vcPart_id;
                                    dataRow["vcMessage"] = vcOrderNo + "文件中的品番" + vcPart_id + "解析的内外区分与主数据该品番的内外不对应";
                                    dtMessage.Rows.Add(dataRow);
                                    bReault = false;
                                }
                                if (!dicPartNo.ContainsKey(vcPart_id))
                                {
                                    dicPartNo.Add(vcPart_id, vcPart_id);
                                }

                                ////检测数量
                                if (SoqDt.Rows.Count > 0)
                                {
                                    if (!CheckTotalNumEqual(OrderTargetYM, vcPart_id, vcOrderingMethod, SoqDt, detail.QTY))
                                    {
                                        DataRow dataRow = dtMessage.NewRow();
                                        dataRow["vcOrder"] = vcOrderNo;
                                        dataRow["vcPartNo"] = vcPart_id;
                                        dataRow["vcMessage"] = vcOrderNo + "文件中的品番" + vcPart_id + "订单数量不正确";
                                        dtMessage.Rows.Add(dataRow);
                                        bReault = false;
                                    }
                                }
                                else
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcOrder"] = vcOrderNo;
                                    dataRow["vcPartNo"] = "";
                                    dataRow["vcMessage"] = OrderTargetYM + "月的月度Replay的数据为0条";
                                    dtMessage.Rows.Add(dataRow);
                                    bReault = false;
                                }

                            }
                            else
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcOrder"] = vcOrderNo;
                                dataRow["vcPartNo"] = vcPart_id;
                                dataRow["vcMessage"] = vcOrderNo + "文件中品番基础数据表不包含品番" + vcPart_id;
                                dtMessage.Rows.Add(dataRow);
                                bReault = false;
                            }
                        }
                        if (dicPartNo.Count != drArrayTmp.Length)
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcOrder"] = vcOrderNo;
                            dataRow["vcPartNo"] = "";
                            dataRow["vcMessage"] = vcOrderNo + "文件的品番个数与月度replay的品番个数不一样";
                            dtMessage.Rows.Add(dataRow);
                            bReault = false;
                        }

                        #endregion

                        if (bReault)
                        {
                            foreach (Detail detail in order.Details)
                            {
                                string tmp = "";
                                string vcPart_id = detail.PartsNo.Trim();
                                string CPD = detail.CPD.Trim();
                                string vcSeqno = detail.ItemNo.Trim();
                                string vcDock = "";
                                string vcSupplierId = "";
                                string vcSupplierPlant = "";//工区

                                string vcCarType = "";
                                string vcPartId_Replace = "";
                                string inout = "";
                                string packingSpot = "";

                                Hashtable hashtable = getDock(vcPart_id, CPD, vcPackingFactory, dockTmp);

                                inout = ObjToString(hashtable["vcInOut"]);
                                vcDock = ObjToString(hashtable["vcSufferIn"]);
                                vcSupplierId = ObjToString(hashtable["vcSupplierId"]);
                                vcCarType = ObjToString(hashtable["vcCarfamilyCode"]);
                                vcPartId_Replace = ObjToString(hashtable["vcPartId_Replace"]);
                                vcSupplierPlant = ObjToString(hashtable["vcSupplierPlant"]);

                                DataRow[] packingSpotRow = PackSpot.Select("vcPart_id = '" + vcPart_id + "' AND vcReceiver = '" + CPD + "' AND vcSupplierId = '" + vcSupplierId + "' AND vcPackingPlant = '" + vcPackingFactory + "'");

                                if (packingSpotRow.Length > 0)
                                {
                                    packingSpot = ObjToString(packingSpotRow[0]["vcBZPlant"]);
                                }


                                DataRow[] rows = SoqDt.Select(" vcPart_id = '" + vcPart_id + "' ");

                                string dateTime = detail.Date.Trim();
                                string Day = Convert.ToInt32(dateTime.Substring(6, 2)).ToString();


                                //新增订单
                                sbr.Append(" INSERT INTO SP_M_ORD(vcPackingFactory, vcTargetYearMonth, vcDock, vcCpdcompany, vcOrderType, vcOrderNo, vcSeqno, dOrderDate, dOrderExportDate, vcPartNo, vcInsideOutsideType, vcCarType, vcLastPartNo, vcPackingSpot, vcSupplier_id,vcPlantQtyDaily1,vcPlantQtyDaily2,vcPlantQtyDaily3,vcPlantQtyDaily4,vcPlantQtyDaily5,vcPlantQtyDaily6,vcPlantQtyDaily7,vcPlantQtyDaily8,vcPlantQtyDaily9,vcPlantQtyDaily10,vcPlantQtyDaily11,vcPlantQtyDaily12,vcPlantQtyDaily13,vcPlantQtyDaily14,vcPlantQtyDaily15,vcPlantQtyDaily16,vcPlantQtyDaily17,vcPlantQtyDaily18,vcPlantQtyDaily19,vcPlantQtyDaily20,vcPlantQtyDaily21,vcPlantQtyDaily22,vcPlantQtyDaily23,vcPlantQtyDaily24,vcPlantQtyDaily25,vcPlantQtyDaily26,vcPlantQtyDaily27,vcPlantQtyDaily28,vcPlantQtyDaily29,vcPlantQtyDaily30,vcPlantQtyDaily31,  vcTargetMonthFlag, vcTargetMonthLast, vcOperatorID, dOperatorTime,vcPlantQtyDailySum,vcWorkArea,vcInputQtyDailySum,vcResultQtyDailySum)");
                                sbr.Append(" VALUES( ");
                                sbr.Append(ComFunction.getSqlValue(vcPackingFactory, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(OrderTargetYM, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(vcDock, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(CPD, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(Type, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(vcOrderNo, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(vcSeqno, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(dateTime, true) + ",");
                                sbr.Append("GetDate(),");
                                sbr.Append(ComFunction.getSqlValue(vcPart_id, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(inout, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(vcCarType, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(vcPartId_Replace, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(packingSpot, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(vcSupplierId, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD1"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD2"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD3"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD4"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD5"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD6"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD7"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD8"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD9"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD10"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD11"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD12"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD13"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD14"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD15"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD16"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD17"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD18"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD19"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD20"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD21"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD22"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD23"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD24"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD25"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD26"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD27"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD28"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD29"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD30"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iD31"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue('0', false) + ",");
                                sbr.Append(ComFunction.getSqlValue(LastTime, true) + ",");
                                sbr.Append("'" + vcSupplierId + "',");
                                sbr.Append("GetDate(),");
                                sbr.Append(ComFunction.getSqlValue(rows[0]["iPartNums"], true) + ",");
                                sbr.Append(ComFunction.getSqlValue(vcSupplierPlant, true) + ",");
                                sbr.Append("'0','0') \r\n");
                            }

                            //进入内示
                            sbr.AppendLine(MonthToNeishi(Time, userId));
                            //修改状态
                            int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                            sbr.AppendLine("UPDATE TOrderUploadManage SET vcOrderState = '1' ,vcOperatorID = '" + userId + "',dOperatorTime = GETDATE() WHERE iAutoId =" + iAutoId + " ");

                        }

                    }
                    else if (Type.Equals("H") || Type.Equals("F") || Type.Equals("C"))
                    {

                        #region 紧急检测
                        string vcOrderNoOld = getOrderNo(OrderNo, vcOrderNo);
                        //判断头
                        foreach (Detail detail in order.Details)
                        {
                            string tmp = "";
                            string vcPart_id = detail.PartsNo.Trim();
                            string CPD = detail.CPD.Trim();
                            string vcSeqno = detail.ItemNo.Trim();
                            string vcSupplierId = "";
                            string vcSupplierPlant = "";//工区
                            string vcSupplierPlace = "";//出荷场
                            string vcOrderNum = detail.QTY;
                            string vcOrderingMethod = "";

                            string dateTime = detail.Date.Trim();
                            string Day = Convert.ToInt32(dateTime.Substring(6, 2)).ToString();
                            //检测品番表是否存在该品番
                            Hashtable hashtable = getDock(vcPart_id, CPD, vcPackingFactory, dockTmp);
                            if (hashtable.Keys.Count == 0)
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcOrder"] = vcOrderNo;
                                dataRow["vcPartNo"] = vcPart_id;
                                dataRow["vcMessage"] = vcOrderNo + "文件中品番基础数据表不包含品番" + vcPart_id;
                                dtMessage.Rows.Add(dataRow);
                                bReault = false;
                            }

                            DataRow[] rowNQ = OrderNQ.Select(" vcOrderNo = '" + vcOrderNoOld + "' AND vcPart_id = '" + vcPart_id + "'");
                            if (rowNQ.Length == 0)
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcOrder"] = vcOrderNo;
                                dataRow["vcPartNo"] = vcPart_id;
                                dataRow["vcMessage"] = vcOrderNo + "文件中" + vcPart_id + "纳期不存在";
                                dtMessage.Rows.Add(dataRow);
                                bReault = false;
                            }
                        }

                        if (bReault)
                        {
                            foreach (Detail detail in order.Details)
                            {
                                string tmp = "";
                                string vcPart_id = detail.PartsNo.Trim();
                                string CPD = detail.CPD.Trim();
                                string vcSeqno = detail.ItemNo.Trim();
                                string QTY = detail.QTY;
                                string vcDock = "";
                                string vcSupplierId = "";
                                string vcCarType = "";
                                string vcPartId_Replace = "";
                                string inout = "";
                                string packingSpot = "";
                                string vcSupplierPlant = "";//工区
                                string vcHaoJiu = "";
                                string vcOrderPlant = "";
                                string vcSupplierPacking = "";
                                Hashtable hashtable = getDock(vcPart_id, CPD, vcPackingFactory, dockTmp);

                                inout = ObjToString(hashtable["vcInOut"]);
                                vcDock = ObjToString(hashtable["vcSufferIn"]);
                                vcSupplierId = ObjToString(hashtable["vcSupplierId"]);
                                vcCarType = ObjToString(hashtable["vcCarfamilyCode"]);
                                vcPartId_Replace = ObjToString(hashtable["vcPartId_Replace"]);
                                vcSupplierPlant = ObjToString(hashtable["vcSupplierPlant"]);
                                vcHaoJiu = ObjToString(hashtable["vcHaoJiu"]);
                                vcOrderPlant = ObjToString(hashtable["vcOrderPlant"]);
                                vcSupplierPacking = ObjToString(hashtable["vcSupplierPacking"]);
                                DataRow[] packingSpotRow = PackSpot.Select("vcPart_id = '" + vcPart_id + "' AND vcReceiver = '" + CPD + "' AND vcSupplierId = '" + vcSupplierId + "' AND vcPackingPlant = '" + vcPackingFactory + "'");
                                if (packingSpotRow.Length > 0)
                                {
                                    packingSpot = ObjToString(packingSpotRow[0]["vcBZPlant"]);
                                }

                                DataRow[] rowNQ = OrderNQ.Select(" vcOrderNo = '" + vcOrderNoOld + "' AND vcPart_id = '" + vcPart_id + "'");
                                string NQ = rowNQ[0]["dOutPutDate"].ToString();

                                string TargetYMJJ = NQ.Substring(0, 6);
                                string TargetD = Convert.ToInt32(NQ.Substring(6, 2)).ToString();
                                //DateTime Time = DateTime.ParseExact(NQ.Substring(0, 6), "yyyyMM", System.Globalization.CultureInfo.CurrentCulture);
                                DateTime Time = DateTime.Parse(NQ.Substring(0, 4) + "-" + NQ.Substring(4, 2)  + "-01");
                                DateTime LastTime = Time.AddMonths(1).AddDays(-1);



                                string dateTime = detail.Date.Trim();
                                string Day = Convert.ToInt32(dateTime.Substring(6, 2)).ToString();


                                //新增订单
                                sbr.Append(" INSERT INTO SP_M_ORD(vcPackingFactory, vcTargetYearMonth, vcDock, vcCpdcompany, vcOrderType, vcOrderNo, vcSeqno, dOrderDate, dOrderExportDate, vcPartNo, vcInsideOutsideType, vcCarType, vcLastPartNo, vcPackingSpot, vcSupplier_id,vcPlantQtyDaily" + TargetD + ",vcTargetMonthFlag, vcTargetMonthLast, vcOperatorID, dOperatorTime,vcPlantQtyDailySum,vcWorkArea,vcInputQtyDailySum,vcResultQtyDailySum)");
                                sbr.Append(" VALUES( ");
                                sbr.Append(ComFunction.getSqlValue(vcPackingFactory, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(TargetYMJJ, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(vcDock, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(CPD, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(Type, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(vcOrderNo, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(vcSeqno, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(dateTime, true) + ",");
                                sbr.Append("GetDate(),");
                                sbr.Append(ComFunction.getSqlValue(vcPart_id, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(inout, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(vcCarType, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(vcPartId_Replace, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(packingSpot, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(vcSupplierId, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(QTY, true) + ",");
                                sbr.Append(ComFunction.getSqlValue('0', false) + ",");
                                sbr.Append(ComFunction.getSqlValue(LastTime, true) + ",");
                                sbr.Append("'" + vcSupplierId + "',");
                                sbr.Append("GetDate(),");
                                sbr.Append(ComFunction.getSqlValue(QTY, true) + ",");
                                sbr.Append(ComFunction.getSqlValue(vcSupplierPlant, true) + ",");
                                sbr.Append("'0','0') \r\n");


                                //写入紧急台账
                                sbr.Append("INSERT INTO TEmergentOrderManage(dOrderHandleDate, vcOrderNo, vcPartNo, vcInsideOutsideType, vcNewOldFlag, vcInjectionFactory, vcDock, vcSupplier_id, vcWorkArea, vcCHCCode, vcCarType, vcOrderNum, dExpectReceiveDate, vcOderTimes, vcInjectionOrderNo, vcMemo, vcOperatorID, dOperatorTime, vcIsExportFlag)");
                                sbr.Append("VALUES(GETDATE(),");
                                sbr.Append(ComFunction.getSqlValue(vcOrderNo, false) + "  ,");
                                sbr.Append(ComFunction.getSqlValue(vcPart_id, false) + "  , ");
                                sbr.Append(ComFunction.getSqlValue(inout, false) + "  , ");
                                sbr.Append(ComFunction.getSqlValue(vcHaoJiu, false) + "  ,");
                                sbr.Append(ComFunction.getSqlValue(vcOrderPlant, false) + "  ,");
                                sbr.Append(ComFunction.getSqlValue(vcDock, false) + "  ,");
                                sbr.Append(ComFunction.getSqlValue(vcSupplierId, false) + "  , ");
                                sbr.Append(ComFunction.getSqlValue(vcSupplierPlant, false) + "  ,");
                                sbr.Append(ComFunction.getSqlValue(vcSupplierPacking, false) + "  ,");
                                sbr.Append(ComFunction.getSqlValue(vcCarType, false) + "  ,");
                                sbr.Append(ComFunction.getSqlValue(QTY, false) + "  , ");
                                sbr.Append(ComFunction.getSqlValue(NQ, true) + ",");
                                sbr.Append("'01'  , ");
                                sbr.Append("null  ,");
                                sbr.Append("null  ,");
                                sbr.Append("'" + userId + "'  ,");
                                sbr.Append("GETDATE(),");
                                sbr.Append("'0') \r\n");

                            }
                            //修改状态
                            int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                            sbr.AppendLine("UPDATE TOrderUploadManage SET vcOrderState = '1' ,vcOperatorID = '" + userId + "',dOperatorTime = GETDATE() WHERE iAutoId =" + iAutoId + " ");

                        }
                        #endregion
                    }
                    else if (Type.Equals("D"))
                    {
                        #region 日度订单校验
                        //TODO 校验
                        FS0403_DataAccess fs0403DataAccess = new FS0403_DataAccess();
                        DataTable dtRiDuCheck = fs0403DataAccess.getModify(Convert.ToDateTime(listInfoData[i]["vcTargetYM"].ToString().Substring(0, 8)));
                        if (dtRiDuCheck.Rows.Count == 0)
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcOrder"] = vcOrderNo;
                            dataRow["vcPartNo"] = "";
                            dataRow["vcMessage"] = "当前日的Soq日度订单平准化的品番数量为0条,无法进行日度校验!";
                            dtMessage.Rows.Add(dataRow);
                            bReault = false;
                        }
                        else
                        {
                            foreach (Detail detail in order.Details)
                            {
                                DataRow[] drArrayPart = dtRiDuCheck.Select("vcPart_id='" + detail.PartsNo.Trim() + "'"); //获取品番的数量
                                if (drArrayPart.Length == 0)
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcOrder"] = vcOrderNo;
                                    dataRow["vcPartNo"] = detail.PartsNo.Trim();
                                    dataRow["vcMessage"] = "当前日的Soq日度订单平准化未找到该品番!";
                                    dtMessage.Rows.Add(dataRow);
                                    bReault = false;
                                }
                                else
                                {
                                    if (Convert.ToInt32(drArrayPart[0]["DayNum"]) == 0)
                                    {
                                        DataRow dataRow = dtMessage.NewRow();
                                        dataRow["vcOrder"] = vcOrderNo;
                                        dataRow["vcPartNo"] = detail.PartsNo.Trim();
                                        dataRow["vcMessage"] = "当前日的Soq日度订单平准化品番数量为0!";
                                        dtMessage.Rows.Add(dataRow);
                                        bReault = false;
                                    }
                                    else
                                    {
                                        if (Convert.ToInt32(drArrayPart[0]["DayNum"]) != Convert.ToInt32(detail.QTY))
                                        {
                                            DataRow dataRow = dtMessage.NewRow();
                                            dataRow["vcOrder"] = vcOrderNo;
                                            dataRow["vcPartNo"] = detail.PartsNo.Trim();
                                            dataRow["vcMessage"] = "当前日的日度订单与Soq日度订单平准化品番的数量" + Convert.ToInt32(drArrayPart[0]["DayNum"]) + "无法匹配";
                                            dtMessage.Rows.Add(dataRow);
                                            bReault = false;
                                        }
                                    }
                                }
                            }
                        }



                        #endregion

                        #region 生成

                        bReault = true;
                        if (bReault)
                        {
                            string OrderTargetYM = listInfoData[i]["vcTargetYM"].ToString().Substring(0, 8);
                            DateTime DXR;
                            //DateTime.TryParseExact(OrderTargetYM, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DXR);
                            DXR = DateTime.Parse(OrderTargetYM.Substring(0, 4) + "-" + OrderTargetYM.Substring(4, 2) + "-" + OrderTargetYM.Substring(6, 2));

                            DataTable dt = fs0403_dataAccess.getCalendar(DXR);
                            int count = fs0403_dataAccess.getCountDay();
                            Hashtable hs = fs0403_dataAccess.getDay(dt, DXR, count);

                            foreach (Detail detail in order.Details)
                            {
                                string tmp = "";
                                string vcPart_id = detail.PartsNo.Trim();
                                string CPD = detail.CPD.Trim();
                                string vcSeqno = detail.ItemNo.Trim();
                                string QTY = detail.QTY;
                                string vcDock = "";
                                string vcSupplierId = "";
                                string vcCarType = "";
                                string vcPartId_Replace = "";
                                string inout = "";
                                string packingSpot = "";
                                string vcSupplierPlant = "";//工区
                                string vcHaoJiu = "";
                                string vcOrderPlant = "";
                                string vcSupplierPacking = "";
                                Hashtable hashtable = getDock(vcPart_id, CPD, vcPackingFactory, dockTmp);

                                inout = ObjToString(hashtable["vcInOut"]);
                                vcDock = ObjToString(hashtable["vcSufferIn"]);
                                vcSupplierId = ObjToString(hashtable["vcSupplierId"]);
                                vcCarType = ObjToString(hashtable["vcCarfamilyCode"]);
                                vcPartId_Replace = ObjToString(hashtable["vcPartId_Replace"]);
                                vcSupplierPlant = ObjToString(hashtable["vcSupplierPlant"]);
                                vcHaoJiu = ObjToString(hashtable["vcHaoJiu"]);
                                vcOrderPlant = ObjToString(hashtable["vcOrderPlant"]);
                                vcSupplierPacking = ObjToString(hashtable["vcSupplierPacking"]);
                                DataRow[] packingSpotRow = PackSpot.Select("vcPart_id = '" + vcPart_id + "' AND vcReceiver = '" + CPD + "' AND vcSupplierId = '" + vcSupplierId + "' AND vcPackingPlant = '" + vcPackingFactory + "'");
                                if (packingSpotRow.Length > 0)
                                {
                                    packingSpot = ObjToString(packingSpotRow[0]["vcBZPlant"]);
                                }

                                string timeYM = hs[vcOrderPlant].ToString().Substring(0, 6);
                                string timeD = hs[vcOrderPlant].ToString().Substring(6, 2);

                                string dateTime = detail.Date.Trim();

                                //DateTime Time = DateTime.ParseExact(timeYM, "yyyyMM", System.Globalization.CultureInfo.CurrentCulture);
                                DateTime Time = DateTime.Parse(timeYM.Substring(0, 4) + "-" + timeYM.Substring(4, 2) + "-01");
                                DateTime LastTime = Time.AddMonths(1).AddDays(-1);

                                //新增订单
                                sbr.Append(" INSERT INTO SP_M_ORD(vcPackingFactory, vcTargetYearMonth, vcDock, vcCpdcompany, vcOrderType, vcOrderNo, vcSeqno, dOrderDate, dOrderExportDate, vcPartNo, vcInsideOutsideType, vcCarType, vcLastPartNo, vcPackingSpot, vcSupplier_id,vcPlantQtyDaily" + timeD + ",vcTargetMonthFlag, vcTargetMonthLast, vcOperatorID, dOperatorTime,vcPlantQtyDailySum,vcWorkArea,vcInputQtyDailySum,vcResultQtyDailySum)");
                                sbr.Append(" VALUES( ");
                                sbr.Append(ComFunction.getSqlValue(vcPackingFactory, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(timeYM, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(vcDock, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(CPD, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(Type, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(vcOrderNo, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(vcSeqno, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(dateTime, true) + ",");
                                sbr.Append("GetDate(),");
                                sbr.Append(ComFunction.getSqlValue(vcPart_id, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(inout, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(vcCarType, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(vcPartId_Replace, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(packingSpot, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(vcSupplierId, false) + ",");
                                sbr.Append(ComFunction.getSqlValue(QTY, true) + ",");
                                sbr.Append(ComFunction.getSqlValue('0', false) + ",");
                                sbr.Append(ComFunction.getSqlValue(LastTime, true) + ",");
                                sbr.Append("'" + vcSupplierId + "',");
                                sbr.Append("GetDate(),");
                                sbr.Append(ComFunction.getSqlValue(QTY, true) + ",");
                                sbr.Append(ComFunction.getSqlValue(vcSupplierPlant, true) + ",");
                                sbr.Append("'0','0') \r\n");


                            }
                            //修改状态
                            int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                            sbr.AppendLine("UPDATE TOrderUploadManage SET vcOrderState = '1' ,vcOperatorID = '" + userId + "',dOperatorTime = GETDATE() WHERE iAutoId =" + iAutoId + " ");

                        }


                        #endregion
                    }

                    if (dtMessage.Rows.Count > 0)
                    {
                        return false;
                    }

                    if (sbr.Length > 0)
                    {
                        excute.ExcuteSqlWithStringOper(sbr.ToString());
                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion



        #region 月度订单传入内示

        public string MonthToNeishi(DateTime time, string userId)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                //N+1
                DateTime N1 = time.AddMonths(1);

                DateTime CL = time.AddMonths(-1);
                sbr.AppendLine("DECLARE @TargetYM VARCHAR(6)");
                sbr.AppendLine("DECLARE @CLYM VARCHAR(6)");
                sbr.AppendLine("DECLARE @startTime DATETIME");
                sbr.AppendLine("DECLARE @endTime DATETIME");
                sbr.AppendLine("SET @TargetYM = '" + N1.ToString("yyyyMM") + "'");
                sbr.AppendLine("SET @CLYM = '" + CL.ToString("yyyyMM") + "'");
                sbr.AppendLine("SET @startTime = CONVERT(DATETIME,@TargetYM+'01')");
                sbr.AppendLine("SET @endTime = DATEADD(DAY,-1,DATEADD(Month,1,@startTime))");
                sbr.AppendLine("DELETE TNeiShi WHERE TARGETMONTH = @TargetYM");
                sbr.AppendLine("INSERT INTO TNeiShi(CPDCOMPANY, dInputDate, TARGETMONTH, PARTSNO, CARFAMCODE, INOUTFLAG, SUPPLIERCODE, iSupplierPlant, DOCK, RESULTQTYTOTAL, varInputUser, vcOperatorID, dOperatorTime)");
                sbr.AppendLine("SELECT a.CPDCOMPANY,a.dInputDate,a.TARGETMONTH,a.PARTSNO,a.CARFAMCODE,a.INOUTFLAG,b.vcSupplierId AS SUPPLIERCODE,c.vcSupplierPlant AS iSupplierPlant,b.vcSufferIn AS DOCK,a.RESULTQTYTOTAL,'" + userId + "' as varInputUser,'" + userId + "' as vcOperatorID,GETDATE() AS dOperatorTime FROM");
                sbr.AppendLine("(");
                //sbr.AppendLine("	SELECT 'APC06' AS CPDCOMPANY,GETDATE() AS dInputDate,@TargetYM AS TARGETMONTH,vcPart_id AS PARTSNO,vcCarType AS CARFAMCODE,vcInOutFlag AS INOUTFLAG,iPartNums AS RESULTQTYTOTAL FROM  dbo.TSoqReply WHERE vcInOutFlag = '0' AND vcDXYM = @TargetYM  AND vcCLYM = @CLYM AND vcMakingOrderType = '0'");
                sbr.AppendLine("	SELECT 'APC06' AS CPDCOMPANY,GETDATE() AS dInputDate,@TargetYM AS TARGETMONTH,vcPart_id AS PARTSNO,vcCarType AS CARFAMCODE,vcInOutFlag AS INOUTFLAG,iPartNums AS RESULTQTYTOTAL FROM  dbo.TSoqReply WHERE vcDXYM = @TargetYM  AND vcCLYM = @CLYM ");
                sbr.AppendLine(") A");
                sbr.AppendLine("LEFT JOIN ");
                sbr.AppendLine("(");
                sbr.AppendLine("	SELECT DISTINCT a.vcPartId,a.vcSupplierId,b.vcSufferIn FROM");
                sbr.AppendLine("	(");
                sbr.AppendLine("		SELECT * FROM TSPMaster WHERE dFromTime <= @startTime AND dToTime >= @endTime");
                sbr.AppendLine("	) a");
                sbr.AppendLine("	LEFT JOIN");
                sbr.AppendLine("	(");
                sbr.AppendLine("		SELECT * FROM dbo.TSPMaster_SufferIn WHERE vcOperatorType = '1' AND dFromTime <= @startTime AND dToTime >= @endTime");
                sbr.AppendLine("	) b ON a.vcPartId = b.vcPartId AND a.vcReceiver = b.vcReceiver AND a.vcPackingPlant = b.vcPackingPlant AND a.vcSupplierId = b.vcSupplierId");
                sbr.AppendLine("	");
                sbr.AppendLine("	WHERE  b.vcSufferIn IS NOT NULL ");
                sbr.AppendLine(") B ON a.PARTSNO = b.vcPartId ");
                sbr.AppendLine("LEFT JOIN");
                sbr.AppendLine("(");
                sbr.AppendLine("	SELECT DISTINCT a.vcPartId,a.vcSupplierId,b.vcSupplierPlant FROM");
                sbr.AppendLine("	(");
                sbr.AppendLine("		SELECT * FROM TSPMaster WHERE dFromTime <= @startTime AND dToTime >= @endTime");
                sbr.AppendLine("	) a");
                sbr.AppendLine("	LEFT JOIN");
                sbr.AppendLine("	(");
                sbr.AppendLine("		SELECT * FROM dbo.TSPMaster_SupplierPlant WHERE vcOperatorType = '1' AND dFromTime <= @startTime AND dToTime >= @endTime");
                sbr.AppendLine("	) b ON a.vcPartId = b.vcPartId AND a.vcReceiver = b.vcReceiver AND a.vcPackingPlant = b.vcPackingPlant AND a.vcSupplierId = b.vcSupplierId");
                sbr.AppendLine("	WHERE b.vcSupplierPlant IS NOT NULL ");
                sbr.AppendLine(") C  ON a.PARTSNO = C.vcPartId ");


                //N+2

                DateTime N2 = time.AddMonths(2);

                sbr.AppendLine("SET @TargetYM = '" + N2.ToString("yyyyMM") + "'");
                sbr.AppendLine("SET @startTime = CONVERT(DATETIME,@TargetYM+'01')");
                sbr.AppendLine("SET @endTime = DATEADD(DAY,-1,DATEADD(Month,1,@startTime))");
                sbr.AppendLine("DELETE TNeiShi WHERE TARGETMONTH = @TargetYM");
                sbr.AppendLine("INSERT INTO TNeiShi(CPDCOMPANY, dInputDate, TARGETMONTH, PARTSNO, CARFAMCODE, INOUTFLAG, SUPPLIERCODE, iSupplierPlant, DOCK, RESULTQTYTOTAL, varInputUser, vcOperatorID, dOperatorTime)");
                sbr.AppendLine("SELECT a.CPDCOMPANY,a.dInputDate,a.TARGETMONTH,a.PARTSNO,a.CARFAMCODE,a.INOUTFLAG,b.vcSupplierId AS SUPPLIERCODE,c.vcSupplierPlant AS iSupplierPlant,b.vcSufferIn AS DOCK,a.RESULTQTYTOTAL,'" + userId + "' as varInputUser,'" + userId + "' as vcOperatorID,GETDATE() AS dOperatorTime FROM");
                sbr.AppendLine("(");
                //sbr.AppendLine("	SELECT 'APC06' AS CPDCOMPANY,GETDATE() AS dInputDate,@TargetYM AS TARGETMONTH,vcPart_id AS PARTSNO,vcCarType AS CARFAMCODE,vcInOutFlag AS INOUTFLAG,iPartNums AS RESULTQTYTOTAL FROM  dbo.TSoqReply WHERE vcInOutFlag = '0' AND vcDXYM = @TargetYM  AND vcCLYM = @CLYM AND vcMakingOrderType = '0'");
                sbr.AppendLine("	SELECT 'APC06' AS CPDCOMPANY,GETDATE() AS dInputDate,@TargetYM AS TARGETMONTH,vcPart_id AS PARTSNO,vcCarType AS CARFAMCODE,vcInOutFlag AS INOUTFLAG,iPartNums AS RESULTQTYTOTAL FROM  dbo.TSoqReply WHERE vcDXYM = @TargetYM  AND vcCLYM = @CLYM ");
                sbr.AppendLine(") A");
                sbr.AppendLine("LEFT JOIN ");
                sbr.AppendLine("(");
                sbr.AppendLine("	SELECT DISTINCT a.vcPartId,a.vcSupplierId,b.vcSufferIn FROM");
                sbr.AppendLine("	(");
                sbr.AppendLine("		SELECT * FROM TSPMaster WHERE dFromTime <= @startTime AND dToTime >= @endTime");
                sbr.AppendLine("	) a");
                sbr.AppendLine("	LEFT JOIN");
                sbr.AppendLine("	(");
                sbr.AppendLine("		SELECT * FROM dbo.TSPMaster_SufferIn WHERE vcOperatorType = '1' AND dFromTime <= @startTime AND dToTime >= @endTime");
                sbr.AppendLine("	) b ON a.vcPartId = b.vcPartId AND a.vcReceiver = b.vcReceiver AND a.vcPackingPlant = b.vcPackingPlant AND a.vcSupplierId = b.vcSupplierId");
                sbr.AppendLine("	");
                sbr.AppendLine("	WHERE  b.vcSufferIn IS NOT NULL ");
                sbr.AppendLine(") B ON a.PARTSNO = b.vcPartId ");
                sbr.AppendLine("LEFT JOIN");
                sbr.AppendLine("(");
                sbr.AppendLine("	SELECT DISTINCT a.vcPartId,a.vcSupplierId,b.vcSupplierPlant FROM");
                sbr.AppendLine("	(");
                sbr.AppendLine("		SELECT * FROM TSPMaster WHERE dFromTime <= @startTime AND dToTime >= @endTime");
                sbr.AppendLine("	) a");
                sbr.AppendLine("	LEFT JOIN");
                sbr.AppendLine("	(");
                sbr.AppendLine("		SELECT * FROM dbo.TSPMaster_SupplierPlant WHERE vcOperatorType = '1' AND dFromTime <= @startTime AND dToTime >= @endTime");
                sbr.AppendLine("	) b ON a.vcPartId = b.vcPartId AND a.vcReceiver = b.vcReceiver AND a.vcPackingPlant = b.vcPackingPlant AND a.vcSupplierId = b.vcSupplierId");
                sbr.AppendLine("	WHERE b.vcSupplierPlant IS NOT NULL ");
                sbr.AppendLine(") C  ON a.PARTSNO = C.vcPartId ");
                return sbr.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 读取txt

        public Order GetPartFromFile(string path, string orderNo, ref string msg)
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
                    detail.PartsNo = temp.Substring(26, 12).Replace(" ", "0");
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
                msg = "订单" + orderNo + "Head部分有误";
                return null;
            }
            else if (order.Details.Count == 0)
            {
                msg = "订单" + orderNo + "Detail为空";
                return null;
            }
            else if (order.Tail == null)
            {
                msg = "订单" + orderNo + "Tail部分有误";
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
        public DataTable getSoqDt(List<string> TargetYM)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                foreach (string s in TargetYM)
                {
                    DateTime t2;
                    //DateTime.TryParseExact(s + "01", "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out t2);
                    t2 = DateTime.Parse(s.Substring(0, 4) + "-" + s.Substring(4, 2) + "-" + "01");
                    string CLYM = t2.AddMonths(-1).ToString("yyyyMM");
                    if (sql.Length > 0)
                    {
                        sql.Append(" OR ");
                    }
                    sql.Append("(vcDXYM = '" + s + "' AND vcCLYM = '" + CLYM + "')");
                }

                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine(" SELECT vcPart_id,vcInOutFlag,vcDXYM,vcFZGC,vcCarType,vcMakingOrderType,iPartNums,iD1,iD2,iD3,iD4,iD5,iD6,iD7,iD8,iD9,iD10,iD11,iD12,iD13,iD14,iD15,iD16,iD17,iD18,iD19,iD20,iD21,iD22,iD23,iD24,iD25,iD26,iD27,iD28,iD29,iD30,iD31 FROM TSOQReply");
                sbr.AppendLine(" WHERE 1=1 AND  ISNULL(iPartNums,0) <> 0 AND ");
                sbr.AppendLine(sql.ToString());
                DataTable dt = excute.ExcuteSqlWithSelectToDT(sbr.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Hashtable getSoqNumMonth(string partId, string TargetYM, string vcOrderingMethod, DataTable dt)
        {
            Hashtable hashtable = new Hashtable();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //品番，内外，订货方式，车型
                if (partId.Trim().Equals(dt.Rows[i]["vcPart_id"].ToString().Trim()) && TargetYM.Equals(dt.Rows[i]["vcDXYM"].ToString().Trim()))
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

        public bool CheckTotalNumEqual(string TargetYM, string partId, string vcOrderingMethod, DataTable dt, string total)
        {
            try
            {
                bool flag = false;

                int totalCheck = Convert.ToInt32(getSoqNumMonth(partId, TargetYM, vcOrderingMethod, dt)["Total"].ToString());

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

        public Hashtable getDock(string PartID, string receiver, string vcPackingPlant, DataTable dt)
        {
            try
            {
                Hashtable hashtable = new Hashtable();

                if (dt.Rows.Count > 0)
                {

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (PartID.Trim().Equals(dt.Rows[i]["vcPartId"].ToString().Trim())
                            && receiver.Trim().Equals(dt.Rows[i]["vcReceiver"].ToString().Trim())
                            && vcPackingPlant.Trim().Equals(dt.Rows[i]["vcPackingPlant"].ToString().Trim()))
                        {
                            hashtable.Add("vcPartId_Replace", dt.Rows[i]["vcPartId_Replace"].ToString());
                            hashtable.Add("vcSupplierId", dt.Rows[i]["vcSupplierId"].ToString());
                            hashtable.Add("vcCarfamilyCode", dt.Rows[i]["vcCarfamilyCode"].ToString());
                            hashtable.Add("vcSufferIn", dt.Rows[i]["vcSufferIn"].ToString());
                            hashtable.Add("vcInOut", dt.Rows[i]["vcInOut"].ToString());
                            hashtable.Add("vcOrderingMethod", dt.Rows[i]["vcOrderingMethod"].ToString());
                            hashtable.Add("vcHaoJiu", dt.Rows[i]["vcHaoJiu"].ToString());
                            hashtable.Add("vcSupplierPlant", dt.Rows[i]["vcSupplierPlant"].ToString());
                            hashtable.Add("vcOrderPlant", dt.Rows[i]["vcOrderPlant"].ToString());
                            hashtable.Add("vcSupplierPacking", dt.Rows[i]["vcSupplierPacking"].ToString());


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

        public DataTable getDockTable()
        {
            try
            {
                StringBuilder sbr = new StringBuilder();

                sbr.AppendLine("SELECT a.vcPartId,a.vcPartId_Replace,a.vcSupplierId,a.vcCarfamilyCode,a.vcReceiver,b.vcSufferIn,a.vcPackingPlant,a.vcInOut,a.vcOrderingMethod,c.vcSupplierPlant,vcHaoJiu,d.vcOrderPlant,vcSupplierPacking FROM ");
                sbr.AppendLine("(");
                sbr.AppendLine("	SELECT vcSupplierId,vcCarfamilyCode,vcPackingPlant,vcPartId,vcReceiver,vcPartId_Replace,vcOrderingMethod,vcInOut,vcHaoJiu,vcSupplierPacking FROM TSPMaster WHERE ");
                sbr.AppendLine("	dFromTime <= GETDATE() AND dToTime >= GETDATE() ");
                sbr.AppendLine(") a");
                sbr.AppendLine("LEFT JOIN");
                sbr.AppendLine("(");
                sbr.AppendLine("SELECT vcPackingPlant,vcPartId,vcReceiver,vcSupplierId,vcSufferIn FROM TSPMaster_SufferIn WHERE dFromTime <= GETDATE() AND dToTime >= GETDATE() AND vcOperatorType = '1' ");
                sbr.AppendLine(") b ON a.vcPackingPlant = b.vcPackingPlant AND a.vcPartId = b.vcPartId AND a.vcReceiver = b.vcReceiver AND a.vcSupplierId = b.vcSupplierId");
                sbr.AppendLine("LEFT JOIN");
                sbr.AppendLine("(");
                sbr.AppendLine("SELECT vcSupplierPlant,vcSupplierId,vcPartId,vcReceiver,vcPackingPlant FROM TSPMaster_SupplierPlant WHERE vcOperatorType = '1' AND dFromTime <= GETDATE() AND dToTime >= GETDATE()");
                sbr.AppendLine(") c ON a.vcSupplierId = b.vcSupplierId AND a.vcPartId = c.vcPartId AND a.vcReceiver = c.vcReceiver AND a.vcPackingPlant = c.vcPackingPlant");
                sbr.AppendLine("LEFT JOIN");
                sbr.AppendLine("(");
                sbr.AppendLine("select vcValue1 as vcSupplierId,vcValue2 as vcSupplierPlant,vcValue3 as dFromTime,vcValue4 as dToTime,vcValue5 as vcOrderPlant from TOutCode where vcCodeId='C010' and vcIsColum='0' AND vcValue3<=CONVERT(VARCHAR(10),GETDATE(),23) AND vcValue4>=CONVERT(VARCHAR(10),GETDATE(),23)");
                sbr.AppendLine(") d ON a.vcSupplierId = d.vcSupplierId AND c.vcSupplierPlant = d.vcSupplierPlant");

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

        #region 获取路径
        public string getPath(string orderNo)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine("SELECT vcFilePath FROM TOrderUploadManage WHERE vcOrderNo = '" + orderNo + "'");
                DataTable dt = excute.ExcuteSqlWithSelectToDT(sbr.ToString());
                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0]["vcFilePath"].ToString();
                }

                return "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 获取包装厂

        public DataTable getPackSpot()
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine("SELECT vcPart_id,vcReceiver,vcSupplierId,vcPackingPlant,vcBZPlant FROM dbo.TPackageMaster");
                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 获取订单类型

        public DataTable getType()
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine(
                    "SELECT vcOrderDifferentiation AS vcName,vcOrderInitials AS vcValue FROM TOrderDifferentiation");
                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        public string ObjToString(Object obj)
        {
            try
            {
                return obj.ToString();
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public string getOrderType(string type)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine("SELECT vcOrderInitials FROM TOrderDifferentiation WHERE vcOrderDifferentiation = '" + type + "'");
                DataTable dt = excute.ExcuteSqlWithSelectToDT(sbr.ToString());
                if (dt.Rows.Count > 0)
                    return dt.Rows[0]["vcOrderInitials"].ToString();
                return "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable getOrderNo()
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine("SELECT vcLastOrderNo,vcNewOrderNo FROM TUploadOrderRelation");
                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable getNQ()
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine("SELECT vcOrderNo,vcPart_id,CONVERT(VARCHAR(8),dOutPutDate,112) AS dOutPutDate FROM dbo.VI_UrgentOrder_OperHistory WHERE dOutPutDate IS NOT null");
                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string getOrderNo(DataTable dt, string orderNo)
        {
            try
            {
                DataRow[] row = dt.Select("vcNewOrderNo = '" + orderNo + "'");
                if (row.Length > 0)
                    return row[0]["vcLastOrderNo"].ToString();
                return orderNo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string getTypeMethod(string vcType)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine("SELECT vcValue FROM TCode WHERE iAutoId IN(");
                sbr.AppendLine("SELECT vcOrderGoodsId FROM TOrderGoodsAndDifferentiation WHERE vcOrderDifferentiationId in");
                sbr.AppendLine("(SELECT iAutoId FROM TOrderDifferentiation WHERE vcOrderInitials = '" + vcType + "')) ");

                string res = "";

                DataTable dt = excute.ExcuteSqlWithSelectToDT(sbr.ToString());
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(res))
                        {
                            res += ",";
                        }
                        res += "'" + dt.Rows[i]["vcValue"].ToString() + "'";
                    }
                }

                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}