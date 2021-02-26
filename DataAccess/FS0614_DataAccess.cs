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


namespace DataAccess
{
    public class FS0614_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 检索

        public DataTable searchApi(string orderState, string targetYM, string orderNo, string vcOrderType, string dUpload)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine(
                    "SELECT a.iAutoId,a.vcOrderNo,a.vcTargetYear+a.vcTargetMonth+a.vcTargetDay AS vcTargetYM,c.vcName as vcOrderType,b.vcName as vcOrderState,a.vcMemo,a.dUploadDate,a.dCreateDate,a.vcFilePath FROM ");
                sbr.AppendLine("(");
                sbr.AppendLine(
                    "SELECT iAutoId,vcOrderNo,vcTargetYear,vcTargetMonth,vcTargetDay,vcOrderType,vcOrderState,vcMemo,dUploadDate,dCreateDate,vcFilePath");
                sbr.AppendLine("FROM TOrderUploadManage");
                sbr.AppendLine(") a");
                sbr.AppendLine("LEFT JOIN ");
                sbr.AppendLine("(");
                sbr.AppendLine("SELECT vcName,vcValue FROM TCode WHERE vcCodeId = 'C044'");
                sbr.AppendLine(") b ON a.vcOrderState = b.vcValue");
                sbr.AppendLine("LEFT JOIN ");
                sbr.AppendLine("(");
                sbr.AppendLine("SELECT vcName,vcValue FROM TCode WHERE vcCodeId = 'C045'");
                sbr.AppendLine(") c ON a.vcOrderType = c.vcValue");
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
            try
            {
                StringBuilder sbr = new StringBuilder();
                for (int i = 0; i < list.Count; i++)
                {
                    int iAutoId = Convert.ToInt32(list[i]["iAutoId"]);
                    sbr.AppendLine("UPDATE TOrderUploadManage SET vcOrderState = '2' ,vcOperatorID = '" + strUserId + "',dOperatorTime = GETDATE() WHERE iAutoId =" + iAutoId + " ");


                    //删除订单表中该订单数据
                }

                if (sbr.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(sbr.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 生成
        public bool CreateOrder(List<Dictionary<string, Object>> listInfoData, string path, string userId, ref string msg)
        {
            try
            {
                List<string> TargetYM = new List<string>();

                for (int i = 0; i < listInfoData.Count; i++)
                {
                    TargetYM.Add(listInfoData[i]["vcTargetYM"].ToString().Substring(0, 6));
                }
                TargetYM = TargetYM.Distinct().ToList();
                //获取基础信息
                DataTable dockTmp = getDockTable();
                //获取soq验收数量
                DataTable SoqDt = getSoqDt(TargetYM);
                //获取包装厂
                DataTable PackSpot = getPackSpot();
                //获取包装工厂
                string vcPackingFactory = getFactory();

                //读取文件
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    //读取Order
                    Order order = GetPartFromFile(path + listInfoData[i]["vcFilePath"].ToString(), listInfoData[i]["vcOrderNo"].ToString(), ref msg);
                    string vcOrderNo = order.Head.No;
                    DateTime LastTime = DateTime.ParseExact(listInfoData[i]["vcTargetYM"].ToString().Substring(0, 6), "yyyyMM", System.Globalization.CultureInfo.CurrentCulture);
                    LastTime.AddMonths(1).AddDays(-1);
                    if (!string.IsNullOrWhiteSpace(msg))
                    {
                        return false;
                    }

                    StringBuilder sbr = new StringBuilder();

                    if (listInfoData[i]["vcOrderType"].Equals("S"))
                    {
                        string OrderTargetYM = listInfoData[i]["vcTargetYM"].ToString().Substring(0, 6);

                        foreach (Detail detail in order.Details)
                        {
                            string tmp = "";
                            string vcPart_id = detail.PartsNo.Trim();
                            string CPD = detail.CPD.Trim();
                            string vcSeqno = detail.ItemNo.Trim();
                            string vcDock = "";
                            string vcSupplierId = "";
                            string vcCarType = "";
                            string vcPartId_Replace = "";
                            string inout = "";

                            DataRow[] packingSpotRow = PackSpot.Select("vcPart_id = '" + vcPart_id + "' AND vcReceiver = '" + CPD + "' AND vcSupplierId = '" + vcSupplierId + "' AND vcPackingPlant = '" + vcPackingFactory + "'");

                            string packingSpot = packingSpotRow[0]["vcBZPlant"].ToString();
                            DataRow[] rows = SoqDt.Select(" vcPart_id = '" + vcPart_id + "' ");

                            string dateTime = detail.Date.Trim();
                            string Day = Convert.ToInt32(dateTime.Substring(6, 2)).ToString();
                            Hashtable hashtable = getDock(vcPart_id, CPD, vcPackingFactory, dockTmp);
                            if (hashtable.Keys.Count > 0)
                            {
                                inout = hashtable["vcInOut"].ToString();
                                vcDock = hashtable["vcSufferIn"].ToString();
                                vcSupplierId = hashtable["vcSupplierId"].ToString();
                                vcCarType = hashtable["vcCarfamilyCode"].ToString();
                                vcPartId_Replace = hashtable["vcPartId_Replace"].ToString();
                            }
                            else
                            {
                                tmp += "品番基础数据表不包含品番" + vcPart_id + ";\r\n";
                            }

                            //新增订单
                            sbr.Append(" INSERT INTO SP_M_ORD(vcPackingFactory, vcTargetYearMonth, vcDock, vcCpdcompany, vcOrderType, vcOrderNo, vcSeqno, dOrderDate, dOrderExportDate, vcPartNo, vcInsideOutsideType, vcCarType, vcLastPartNo, vcPackingSpot, vcSupplier_id,vcPlantQtyDaily1,vcPlantQtyDaily2,vcPlantQtyDaily3,vcPlantQtyDaily4,vcPlantQtyDaily5,vcPlantQtyDaily6,vcPlantQtyDaily7,vcPlantQtyDaily8,vcPlantQtyDaily9,vcPlantQtyDaily10,vcPlantQtyDaily11,vcPlantQtyDaily12,vcPlantQtyDaily13,vcPlantQtyDaily14,vcPlantQtyDaily15,vcPlantQtyDaily16,vcPlantQtyDaily17,vcPlantQtyDaily18,vcPlantQtyDaily19,vcPlantQtyDaily20,vcPlantQtyDaily21,vcPlantQtyDaily22,vcPlantQtyDaily23,vcPlantQtyDaily24,vcPlantQtyDaily25,vcPlantQtyDaily26,vcPlantQtyDaily27,vcPlantQtyDaily28,vcPlantQtyDaily29,vcPlantQtyDaily30,vcPlantQtyDaily31,  vcTargetMonthFlag, vcTargetMonthLast, vcOperatorID, dOperatorTime)");
                            sbr.Append(" VALUES( ");
                            sbr.Append(ComFunction.getSqlValue(vcPackingFactory, false) + ",");
                            sbr.Append(ComFunction.getSqlValue(OrderTargetYM, false) + ",");
                            sbr.Append(ComFunction.getSqlValue(vcDock, false) + ",");
                            sbr.Append(ComFunction.getSqlValue(CPD, false) + ",");
                            sbr.Append(ComFunction.getSqlValue('S', false) + ",");
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
                        }


                        //N+1
                        DateTime N1 = LastTime.AddMonths(1);
                        sbr.AppendLine("DECLARE @TargetYM VARCHAR(6)");
                        sbr.AppendLine("DECLARE @startTime DATETIME");
                        sbr.AppendLine("DECLARE @endTime DATETIME");
                        sbr.AppendLine("SET @TargetYM = '" + N1.ToString("yyyyMM") + "'");
                        sbr.AppendLine("SET @startTime = CONVERT(DATETIME,@TargetYM+'01')");
                        sbr.AppendLine("SET @endTime = DATEADD(DAY,-1,DATEADD(Month,1,@startTime))");
                        sbr.AppendLine("DELETE TNeiShi WHERE TARGETMONTH = @TargetYM");
                        sbr.AppendLine("INSERT INTO TNeiShi(CPDCOMPANY, dInputDate, TARGETMONTH, PARTSNO, CARFAMCODE, INOUTFLAG, SUPPLIERCODE, iSupplierPlant, DOCK, RESULTQTYTOTAL, varInputUser, vcOperatorID, dOperatorTime)");
                        sbr.AppendLine("SELECT a.*,b.vcSupplierId AS SUPPLIERCODE,c.vcSupplierPlant AS iSupplierPlant,b.vcSufferIn AS DOCK,'" + userId + "' as varInputUser,'" + userId + "' as vcOperatorID,GETDATE() AS dOperatorTime FROM");
                        sbr.AppendLine("(");
                        sbr.AppendLine("	SELECT 'APC06' AS CPDCOMPANY,GETDATE() AS dInputDate,@TargetYM AS TARGETMONTH,vcPart_id AS PARTSNO,vcCarType AS CARFAMCODE,vcInOutFlag AS INOUTFLAG,iPartNums AS RESULTQTYTOTAL FROM  dbo.TSoqReply WHERE vcInOutFlag = '0' AND vcDXYM = '202104' AND vcMakingOrderType = '0'");
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
                        DateTime N2 = LastTime.AddMonths(2);
                        sbr.AppendLine("DECLARE @TargetYM VARCHAR(6)");
                        sbr.AppendLine("DECLARE @startTime DATETIME");
                        sbr.AppendLine("DECLARE @endTime DATETIME");
                        sbr.AppendLine("SET @TargetYM = '" + N2.ToString("yyyyMM") + "'");
                        sbr.AppendLine("SET @startTime = CONVERT(DATETIME,@TargetYM+'01')");
                        sbr.AppendLine("SET @endTime = DATEADD(DAY,-1,DATEADD(Month,1,@startTime))");
                        sbr.AppendLine("DELETE TNeiShi WHERE TARGETMONTH = @TargetYM");
                        sbr.AppendLine("INSERT INTO TNeiShi(CPDCOMPANY, dInputDate, TARGETMONTH, PARTSNO, CARFAMCODE, INOUTFLAG, SUPPLIERCODE, iSupplierPlant, DOCK, RESULTQTYTOTAL, varInputUser, vcOperatorID, dOperatorTime)");
                        sbr.AppendLine("SELECT a.*,b.vcSupplierId AS SUPPLIERCODE,c.vcSupplierPlant AS iSupplierPlant,b.vcSufferIn AS DOCK,'" + userId + "' as varInputUser,'" + userId + "' as vcOperatorID,GETDATE() AS dOperatorTime FROM");
                        sbr.AppendLine("(");
                        sbr.AppendLine("	SELECT 'APC06' AS CPDCOMPANY,GETDATE() AS dInputDate,@TargetYM AS TARGETMONTH,vcPart_id AS PARTSNO,vcCarType AS CARFAMCODE,vcInOutFlag AS INOUTFLAG,iPartNums AS RESULTQTYTOTAL FROM  dbo.TSoqReply WHERE vcInOutFlag = '0' AND vcDXYM = '202104' AND vcMakingOrderType = '0'");
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

                        if (!string.IsNullOrWhiteSpace(msg))
                        {
                            return false;
                        }
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);

                        //修改状态
                        sbr.AppendLine("UPDATE TOrderUploadManage SET vcOrderState = '1' ,vcOperatorID = '" + userId + "',dOperatorTime = GETDATE() WHERE iAutoId =" + iAutoId + " ");

                    }
                    else if (listInfoData[i]["vcOrderType"].Equals("日度"))
                    {

                    }
                    else if (listInfoData[i]["vcOrderType"].Equals("紧急"))
                    {
                        ////判断头
                        //if (order.Head.Type != "2")
                        //{
                        //    msg = "订单" + vcOrderNo + "类型不正确";
                        //    return false;
                        //}

                        string OrderTargetYM = listInfoData[i]["vcTargetYM"].ToString().Substring(0, 6);

                        foreach (Detail detail in order.Details)
                        {
                            string tmp = "";
                            string vcPart_id = detail.PartsNo.Trim();
                            string CPD = detail.CPD.Trim();
                            string vcSeqno = detail.ItemNo.Trim();
                            string vcDock = "";
                            string vcSupplierId = "";
                            string vcCarType = "";
                            string vcPartId_Replace = "";
                            string inout = "";
                            string vcOrderingMethod = "";

                            string dateTime = detail.Date.Trim();
                            string Day = Convert.ToInt32(dateTime.Substring(6, 2)).ToString();
                            string Field = "vcPlantQtyDaily" + Day;
                            //检测品番表是否存在该品番
                            Hashtable hashtable = getDock(vcPart_id, CPD, vcPackingFactory, dockTmp);
                            if (hashtable.Keys.Count > 0)
                            {
                                inout = hashtable["vcInOut"].ToString();
                                vcDock = hashtable["vcSufferIn"].ToString();
                                vcSupplierId = hashtable["vcSupplierId"].ToString();
                                vcCarType = hashtable["vcCarfamilyCode"].ToString();
                                vcPartId_Replace = hashtable["vcPartId_Replace"].ToString();
                                vcOrderingMethod = hashtable["vcOrderingMethod"].ToString();
                            }
                            else
                            {
                                tmp += "品番基础数据表不包含品番" + vcPart_id + ";\r\n";
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
                            sbr.Append(ComFunction.getSqlValue(OrderTargetYM, false) + ",");
                            sbr.Append(ComFunction.getSqlValue(vcDock, false) + ",");
                            sbr.Append(ComFunction.getSqlValue(CPD, false) + ",");
                            sbr.Append("'3',");
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

                        //插入n+1月内示
                        DateTime N1 = LastTime.AddMonths(1);
                        sbr.AppendLine("DECLARE @TargetYM VARCHAR(6)");
                        sbr.AppendLine("DECLARE @startTime DATETIME");
                        sbr.AppendLine("DECLARE @endTime DATETIME");
                        sbr.AppendLine("SET @TargetYM = '" + N1.ToString("yyyyMM") + "'");
                        sbr.AppendLine("SET @startTime = CONVERT(DATETIME,@TargetYM+'01')");
                        sbr.AppendLine("SET @endTime = DATEADD(DAY,-1,DATEADD(Month,1,@startTime))");
                        sbr.AppendLine("DELETE TNeiShi WHERE TARGETMONTH = @TargetYM");
                        sbr.AppendLine("INSERT INTO TNeiShi(CPDCOMPANY, dInputDate, TARGETMONTH, PARTSNO, CARFAMCODE, INOUTFLAG, SUPPLIERCODE, iSupplierPlant, DOCK, RESULTQTYTOTAL, varInputUser, vcOperatorID, dOperatorTime)");
                        sbr.AppendLine("SELECT a.*,b.vcSupplierId AS SUPPLIERCODE,c.vcSupplierPlant AS iSupplierPlant,b.vcSufferIn AS DOCK,'" + userId + "' as varInputUser,'" + userId + "' as vcOperatorID,GETDATE() AS dOperatorTime FROM");
                        sbr.AppendLine("(");
                        sbr.AppendLine("	SELECT 'APC06' AS CPDCOMPANY,GETDATE() AS dInputDate,@TargetYM AS TARGETMONTH,vcPart_id AS PARTSNO,vcCarType AS CARFAMCODE,vcInOutFlag AS INOUTFLAG,iPartNums AS RESULTQTYTOTAL FROM  dbo.TSoqReply WHERE vcInOutFlag = '0' AND vcDXYM = '202104' AND vcMakingOrderType = '0'");
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

                        //插入n+2月内示
                        DateTime N2 = LastTime.AddMonths(2);
                        sbr.AppendLine("DECLARE @TargetYM VARCHAR(6)");
                        sbr.AppendLine("DECLARE @startTime DATETIME");
                        sbr.AppendLine("DECLARE @endTime DATETIME");
                        sbr.AppendLine("SET @TargetYM = '" + N2.ToString("yyyyMM") + "'");
                        sbr.AppendLine("SET @startTime = CONVERT(DATETIME,@TargetYM+'01')");
                        sbr.AppendLine("SET @endTime = DATEADD(DAY,-1,DATEADD(Month,1,@startTime))");
                        sbr.AppendLine("DELETE TNeiShi WHERE TARGETMONTH = @TargetYM");
                        sbr.AppendLine("INSERT INTO TNeiShi(CPDCOMPANY, dInputDate, TARGETMONTH, PARTSNO, CARFAMCODE, INOUTFLAG, SUPPLIERCODE, iSupplierPlant, DOCK, RESULTQTYTOTAL, varInputUser, vcOperatorID, dOperatorTime)");
                        sbr.AppendLine("SELECT a.*,b.vcSupplierId AS SUPPLIERCODE,c.vcSupplierPlant AS iSupplierPlant,b.vcSufferIn AS DOCK,'" + userId + "' as varInputUser,'" + userId + "' as vcOperatorID,GETDATE() AS dOperatorTime FROM");
                        sbr.AppendLine("(");
                        sbr.AppendLine("	SELECT 'APC06' AS CPDCOMPANY,GETDATE() AS dInputDate,@TargetYM AS TARGETMONTH,vcPart_id AS PARTSNO,vcCarType AS CARFAMCODE,vcInOutFlag AS INOUTFLAG,iPartNums AS RESULTQTYTOTAL FROM  dbo.TSoqReply WHERE vcInOutFlag = '0' AND vcDXYM = '202104' AND vcMakingOrderType = '0'");
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

                        if (!string.IsNullOrWhiteSpace(msg))
                        {
                            return false;
                        }
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);

                        //修改状态
                        sbr.AppendLine("UPDATE TOrderUploadManage SET vcOrderState = '1' ,vcOperatorID = '" + userId + "',dOperatorTime = GETDATE() WHERE iAutoId =" + iAutoId + " ");

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
                string target = "";
                foreach (string s in TargetYM)
                {
                    if (!string.IsNullOrWhiteSpace(target))
                    {
                        target += ",'" + s + "'";
                    }
                    else
                    {
                        target += "'" + s + "'";
                    }
                }

                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine(" SELECT vcPart_id,vcInOutFlag,vcDXYM,vcFZGC,vcCarType,vcMakingOrderType,iPartNums,iD1,iD2,iD3,iD4,iD5,iD6,iD7,iD8,iD9,iD10,iD11,iD12,iD13,iD14,iD15,iD16,iD17,iD18,iD19,iD20,iD21,iD22,iD23,iD24,iD25,iD26,iD27,iD28,iD29,iD30,iD31 FROM TSOQReply");
                sbr.AppendLine(" WHERE ");
                sbr.AppendLine(" vcDXYM in (" + target + ") ");
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
                sbr.AppendLine("SELECT a.vcPartId,a.vcPartId_Replace,a.vcSupplierId,a.vcCarfamilyCode,a.vcReceiver,b.vcSufferIn,a.vcPackingPlant,a.vcInOut,a.vcOrderingMethod FROM ");
                sbr.AppendLine("(");
                sbr.AppendLine("	SELECT vcSupplierId,vcCarfamilyCode,vcPackingPlant,vcPartId,vcReceiver,vcPartId_Replace,vcOrderingMethod,vcInOut FROM TSPMaster WHERE ");
                sbr.AppendLine("	dFromTime <= GETDATE() AND dToTime >= GETDATE() ");
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
    }
}