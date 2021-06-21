using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Common;
using System.Data.SqlClient;
using System.Collections;
using System.Net;

namespace DataAccess
{
    public class P00004_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        public DataTable ValidateHtrst(string partId, string quantity, string dock, string kanbanOrderNo, string kanbanSerial)
        {
            StringBuilder ValidateSql = new StringBuilder();
            ValidateSql.Append("select vcPart_id,vcTrolleyNo from TOperatorQB where vcPart_id='" + partId + "' and vcSR='" + dock + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "' and iQuantity='" + int.Parse(quantity) + "' and vcZYType='S2'");

            return excute.ExcuteSqlWithSelectToDT(ValidateSql.ToString());
        }

        public DataTable ValidateHtrst1(string partId, string quantity, string dock, string kanbanOrderNo, string kanbanSerial)
        {
            StringBuilder ValidateSql1 = new StringBuilder();
            ValidateSql1.Append("select vcPart_id,vcTrolleyNo from TOperatorQB where vcPart_id='" + partId + "' and vcSR='" + dock + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "' and iQuantity='" + int.Parse(quantity) + "' and vcZYType='S3'");

            return excute.ExcuteSqlWithSelectToDT(ValidateSql1.ToString());
        }

        public DataTable ValidateData(string partId, string scanTime, string dock)
        {
            StringBuilder ValidateDataSql = new StringBuilder();

            ValidateDataSql.Append("  select vcPartENName,vcPartNameCn from TSPMaster where vcPartId='" + partId + "' and dFromTime<='" + scanTime + "' and dToTime>='" + scanTime + "'");
            return excute.ExcuteSqlWithSelectToDT(ValidateDataSql.ToString());
        }

        public DataTable getDockInfo(string opearteId)
        {
            StringBuilder GetDockInfoSql = new StringBuilder();
            GetDockInfoSql.Append("SELECT vcForkNo,vcDockSell FROM TSell_DockCar  where vcOperatorID='" + opearteId + "' and vcFlag='0'");
            return excute.ExcuteSqlWithSelectToDT(GetDockInfoSql.ToString());
        }

        public DataTable GetData(string dock, string fork)
        {
            StringBuilder GetDataSql = new StringBuilder();
            GetDataSql.Append("select * from TSell_DockCar where vcDockSell='' and vcForkNo='' and vcFlag='0'");
            return excute.ExcuteSqlWithSelectToDT(GetDataSql.ToString());
        }

        public int UpdateDock2(string dock, string fork, string opearteId, string serverTime)
        {
            StringBuilder UpdateDockSql = new StringBuilder();
            UpdateDockSql.Append("update TSell_DockCar set vcFlag='1',vcOperatorID='" + opearteId + "',dOperatorTime='" + serverTime + "' where vcDockSell='" + dock + "' and vcForkNo='" + fork + "'");



            return excute.ExcuteSqlWithStringOper(UpdateDockSql.ToString());
        }

        public DataTable GetPlant()
        {
            StringBuilder GetPlantSql = new StringBuilder();
            GetPlantSql.Append("select distinct(t3.vcSupplierPlant),t3.vcPackingPlant from TShip_Temp t1 left join TOperateSJ t2 on t1.vcBoxNo=t2.vcBoxNo left join TSPMaster_Box t3 on t2.vcPart_id=t3.vcPartId where t2.vcZYType='S3'");
            return excute.ExcuteSqlWithSelectToDT(GetPlantSql.ToString());
        }

        public DataTable ValidateDock(string dock, string fork)
        {
            StringBuilder ValidateDockSql = new StringBuilder();
            ValidateDockSql.Append("  select vcDockSell from  TSell_DockCar where  vcFlag='0' and vcForkNo='" + fork + "'");
            return excute.ExcuteSqlWithSelectToDT(ValidateDockSql.ToString());
        }





        public DataTable ValidateShip(string fork, string dock)
        {
            StringBuilder ValidateShipSql = new StringBuilder();
            ValidateShipSql.Append("select vcDockSell from TShip_Temp where  vcDockSell='" + dock + "' and vcForkNo='" + fork + "' and vcFlag='0'");
            return excute.ExcuteSqlWithSelectToDT(ValidateShipSql.ToString());
        }

        public DataTable GetSellInfo(string caseNo)
        {
            StringBuilder GetSellInfoSql = new StringBuilder();
            GetSellInfoSql.Append(" select vcInno,iQty  from  TCaseList where vcCaseno='" + caseNo + "' ");
            return excute.ExcuteSqlWithSelectToDT(GetSellInfoSql.ToString());
        }

        public DataTable GetSellInfo2(string caseNo)
        {
            StringBuilder GetSellInfoSql = new StringBuilder();
            GetSellInfoSql.Append(" select vcPart_id from TOperatorQB where vcBoxNo='" + caseNo + "'and vcReflectFlag='0' ");
            return excute.ExcuteSqlWithSelectToDT(GetSellInfoSql.ToString());
        }

        public DataTable GetQBData(string inputNo)
        {
            StringBuilder GetQBDataSql = new StringBuilder();
            GetQBDataSql.Append("select vcTrolleyNo,vcPart_id,vcCpdCompany,vcSR,vcKBOrderNo,vcKBLFNo,vcBZPlant,iPackingQty,vcLabelStart,vcLabelEnd,vcSupplierId,vcSupplierPlant,vcLotid,vcCheckType,vcIOType from TOperatorQB where vcInputNo='" + inputNo + "' and vcZYType='S0' and vcReflectFlag='1' ");
            return excute.ExcuteSqlWithSelectToDT(GetQBDataSql.ToString());
        }

        public DataTable GetSellInfo1(string caseNo)
        {
            StringBuilder GetSellInfoSql = new StringBuilder();
            GetSellInfoSql.Append("select vcPart_id from TOperateSJ where vcBoxNo='" + caseNo + "' and vcZYType='S4'");
            return excute.ExcuteSqlWithSelectToDT(GetSellInfoSql.ToString());
        }

        public int UpdateDock(string dock, string fork, string scanTime, string opearteId)
        {
            StringBuilder UpdateDockSql = new StringBuilder();
            UpdateDockSql.Append("  update TSell_DockCar set    vcFlag='0',vcOperatorID='" + opearteId + "',dOperatorTime='" + scanTime + "' where vcDockSell='" + dock + "' and  vcForkNo='" + fork + "'");
            return excute.ExcuteSqlWithStringOper(UpdateDockSql.ToString());
        }

        public DataTable ValidatePrice(string partId, string scanTime, string cpdCompany, string supplierId)
        {
            StringBuilder ValidatePriceSql = new StringBuilder();
            ValidatePriceSql.Append("   select decPriceTNPWithTax  from TPrice where vcSupplier_id='" + supplierId + "' and vcReceiver='" + cpdCompany + "' and vcPart_id='" + partId + "' and dPricebegin<='" + scanTime + "' and dPriceEnd>='" + scanTime + "'");
            return excute.ExcuteSqlWithSelectToDT(ValidatePriceSql.ToString());
        }

        public int Insert(string trolley, string partId, string quantity, string dock, string kanbanOrderNo, string kanbanSerial, string scanTime, string iP, string serverTime, string cpdCompany, string inno, string opearteId, string packingSpot, string packingQuatity, string lblStart, string lblEnd, string supplierId, string supplierPlant, string lotId, string inoutFlag, string checkType, string caseNo, string dockSell)
        {
            StringBuilder InsertSql = new StringBuilder();
            InsertSql.Append("INSERT INTO TOperatorQB(vcZYType,dScanTime,vcHtNo,vcTrolleyNo ,vcInputNo,vcPart_id,vcCpdCompany,vcSR ,vcBoxNo,iQuantity  \n");
            InsertSql.Append(" ,vcSeqNo,vcReflectFlag,dStart,dEnd,vcHostIp,vcKBOrderNo,vcKBLFNo,vcOperatorID ,dOperatorTime,vcBZPlant,iPackingQty,vcLabelStart,vcLabelEnd,vcSupplierId,vcSupplierPlant,vcLotid,vcIOType,vcCheckType ,vcDockSell)  \n");
            InsertSql.Append("    VALUES('S4', '" + scanTime + "', '', '" + trolley + "', '" + inno + "', '" + partId + "', '" + cpdCompany + "', '" + dock + "', '" + caseNo + "', " + int.Parse(quantity) + ", '', '0', null, null, '" + iP + "', '" + kanbanOrderNo + "', '" + kanbanSerial + "', '" + opearteId + "', '" + serverTime + "','" + packingSpot + "'," + packingQuatity + ",'" + lblStart + "','" + lblEnd + "','" + supplierId + "','" + supplierPlant + "','" + lotId + "','" + inoutFlag + "','" + checkType + "','" + dockSell + "')");
            return excute.ExcuteSqlWithStringOper(InsertSql.ToString());
        }

        public void DelData(string dockSell)
        {
            StringBuilder DelDataSql = new StringBuilder();
            DelDataSql.Append("update TOperatorQB set vcReflectFlag='4' where vcDockSell='" + dockSell + "' and vcReflectFlag='0' and vcZYType='S4'");
            excute.ExcuteSqlWithStringOper(DelDataSql.ToString());
        }

        public DataTable ValidateInv(string partId, string kanbanOrderNo, string kanbanSerial)
        {
            StringBuilder validateInvSql = new StringBuilder();
            validateInvSql.Append("select iDCH from TOperateSJ_InOutput where vcPart_id='" + partId + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "'");
            return excute.ExcuteSqlWithSelectToDT(validateInvSql.ToString());
        }

        public int UpdateDock1(string dock, string fork, string scanTime, string opearteId)
        {
            StringBuilder UpdateDockSql = new StringBuilder();
            UpdateDockSql.Append("   update TSell_DockCar set  vcFlag='1',vcOperatorID='" + opearteId + "',dOperatorTime='" + scanTime + "',vcDockSell='" + dock + "' where vcForkNo='" + fork + "'");
            return excute.ExcuteSqlWithStringOper(UpdateDockSql.ToString());
        }



        public DataTable GetQBData1(string dockSell)
        {
            StringBuilder GetQBDataSql = new StringBuilder();
            GetQBDataSql.Append("select vcTrolleyNo,vcPart_id,vcCpdCompany,vcSR,vcKBOrderNo,vcKBLFNo,vcBZPlant,iPackingQty,vcLabelStart,vcLabelEnd,vcSupplierId,vcSupplierPlant,vcLotid,vcCheckType,vcIOType,vcBoxNo,vcInputNo from TOperatorQB where vcReflectFlag='0' and vcDockSell='" + dockSell + "'");
            return excute.ExcuteSqlWithSelectToDT(GetQBDataSql.ToString());
        }

        public int InsertTool(string sellNo, string opearteId, string scanTime, string hUQuantity, string hUQuantity1, string bPQuantity, string pCQuantity, string cBQuantity, string bianCi)
        {
            StringBuilder InsertToolSql = new StringBuilder();
            InsertToolSql.Append("INSERT INTO TSell_Tool(vcSellNo,vcToolName ,vcToolCode ,vcToolColor,iToolQuantity,vcOperatorID,dOperatorTime,vcYinQuType) VALUES ('" + sellNo + "','金属支柱','PC','红','" + pCQuantity + "','" + opearteId + "','" + scanTime + "','" + bianCi + "');\n");
            InsertToolSql.Append("      INSERT INTO TSell_Tool(vcSellNo, vcToolName, vcToolCode, vcToolColor, iToolQuantity, vcOperatorID, dOperatorTime,vcYinQuType) VALUES('" + sellNo + "', '木托盘', 'BP', '灰白', '" + bPQuantity + "', '" + opearteId + "', '" + scanTime + "','" + bianCi + "');\n");
            InsertToolSql.Append(" INSERT INTO TSell_Tool(vcSellNo, vcToolName, vcToolCode, vcToolColor, iToolQuantity, vcOperatorID, dOperatorTime,vcYinQuType) VALUES('" + sellNo + "', '金属绿筐', 'HU', '绿', '" + hUQuantity1 + "', '" + opearteId + "', '" + scanTime + "','" + bianCi + "');\n");
            InsertToolSql.Append("      INSERT INTO TSell_Tool(vcSellNo, vcToolName, vcToolCode, vcToolColor, iToolQuantity, vcOperatorID, dOperatorTime,vcYinQuType) VALUES('" + sellNo + "', '金属支柱', 'CB', '蓝', '" + cBQuantity + "', '" + opearteId + "', '" + scanTime + "','" + bianCi + "');\n");
            InsertToolSql.Append("      INSERT INTO TSell_Tool(vcSellNo, vcToolName, vcToolCode, vcToolColor, iToolQuantity, vcOperatorID, dOperatorTime,vcYinQuType) VALUES('" + sellNo + "', '金属2HU箱', '2HU', '', '" + hUQuantity + "', '" + opearteId + "', '" + scanTime + "','" + bianCi + "');\n");
            return excute.ExcuteSqlWithStringOper(InsertToolSql.ToString());
        }

        public DataTable GetCount(string partId)
        {
            StringBuilder getCountSql = new StringBuilder();
            getCountSql.Append("select ISNULL(sum(iQuantity),0) as sum from TOperatorQB where vcZYType='S4' and vcPart_id='" + partId + "' and vcReflectFlag='0'");//标志位为0表示只存在实绩情报表中
            return excute.ExcuteSqlWithSelectToDT(getCountSql.ToString());
        }

        public DataTable ValiateOrd2(string partId)
        {
            StringBuilder validateOrdSql = new StringBuilder();
            validateOrdSql.Append(" select  vcTargetYearMonth,vcOrderType,vcOrderNo,vcSeqno,(CAST(ISNULL(vcInputQtyDaily1,0) as int)-CAST(ISNULL(vcResultQtyDaily1,0) as int)) as day1 ,   \n");
            validateOrdSql.Append("  (CAST(ISNULL(vcInputQtyDaily2,0) as int)-CAST(ISNULL(vcResultQtyDaily2,0) as int)) as day2 ,(CAST(ISNULL(vcInputQtyDaily3,0) as int)-CAST(ISNULL(vcResultQtyDaily3,0) as int)) as day3 ,   \n");
            validateOrdSql.Append("  (CAST(ISNULL(vcInputQtyDaily4,0) as int)-CAST(ISNULL(vcResultQtyDaily4,0) as int)) as day4 ,(CAST(ISNULL(vcInputQtyDaily5,0) as int)-CAST(ISNULL(vcResultQtyDaily5,0) as int)) as day5 ,   \n");
            validateOrdSql.Append("  (CAST(ISNULL(vcInputQtyDaily6,0) as int)-CAST(ISNULL(vcResultQtyDaily6,0) as int)) as day6 ,(CAST(ISNULL(vcInputQtyDaily7,0) as int)-CAST(ISNULL(vcResultQtyDaily7,0) as int)) as day7 ,   \n");
            validateOrdSql.Append("  (CAST(ISNULL(vcInputQtyDaily8,0) as int)-CAST(ISNULL(vcResultQtyDaily8,0) as int)) as day8 ,(CAST(ISNULL(vcInputQtyDaily9,0) as int)-CAST(ISNULL(vcResultQtyDaily9,0) as int)) as day9 ,   \n");
            validateOrdSql.Append("  (CAST(ISNULL(vcInputQtyDaily10,0) as int)-CAST(ISNULL(vcResultQtyDaily10,0) as int)) as day10 ,(CAST(ISNULL(vcInputQtyDaily11,0) as int)-CAST(ISNULL(vcResultQtyDaily11,0) as int)) as day11 ,   \n");
            validateOrdSql.Append("  (CAST(ISNULL(vcInputQtyDaily12,0) as int)-CAST(ISNULL(vcResultQtyDaily12,0) as int)) as day12 ,(CAST(ISNULL(vcInputQtyDaily13,0) as int)-CAST(ISNULL(vcResultQtyDaily13,0) as int)) as day13 ,   \n");
            validateOrdSql.Append("  (CAST(ISNULL(vcInputQtyDaily14,0) as int)-CAST(ISNULL(vcResultQtyDaily14,0) as int)) as day14 ,(CAST(ISNULL(vcInputQtyDaily15,0) as int)-CAST(ISNULL(vcResultQtyDaily15,0) as int)) as day15 ,   \n");
            validateOrdSql.Append("  (CAST(ISNULL(vcInputQtyDaily16,0) as int)-CAST(ISNULL(vcResultQtyDaily16,0) as int)) as day16 ,(CAST(ISNULL(vcInputQtyDaily17,0) as int)-CAST(ISNULL(vcResultQtyDaily17,0) as int)) as day17 ,   \n");
            validateOrdSql.Append("  (CAST(ISNULL(vcInputQtyDaily18,0) as int)-CAST(ISNULL(vcResultQtyDaily18,0) as int)) as day18 ,(CAST(ISNULL(vcInputQtyDaily19,0) as int)-CAST(ISNULL(vcResultQtyDaily19,0) as int)) as day19 ,   \n");
            validateOrdSql.Append("   (CAST(ISNULL(vcInputQtyDaily20,0) as int)-CAST(ISNULL(vcResultQtyDaily20,0) as int)) as day20 ,(CAST(ISNULL(vcInputQtyDaily21,0) as int)-CAST(ISNULL(vcResultQtyDaily21,0) as int)) as day21 ,   \n");
            validateOrdSql.Append("  (CAST(ISNULL(vcInputQtyDaily22,0) as int)-CAST(ISNULL(vcResultQtyDaily22,0) as int)) as day22 ,(CAST(ISNULL(vcInputQtyDaily23,0) as int)-CAST(ISNULL(vcResultQtyDaily23,0) as int)) as day23 ,   \n");
            validateOrdSql.Append("  (CAST(ISNULL(vcInputQtyDaily24,0) as int)-CAST(ISNULL(vcResultQtyDaily24,0) as int)) as day24 ,(CAST(ISNULL(vcInputQtyDaily25,0) as int)-CAST(ISNULL(vcResultQtyDaily25,0) as int)) as day25 ,   \n");
            validateOrdSql.Append("  (CAST(ISNULL(vcInputQtyDaily26,0) as int)-CAST(ISNULL(vcResultQtyDaily26,0) as int)) as day26 ,(CAST(ISNULL(vcInputQtyDaily27,0) as int)-CAST(ISNULL(vcResultQtyDaily27,0) as int)) as day27 ,   \n");
            validateOrdSql.Append("  (CAST(ISNULL(vcInputQtyDaily28,0) as int)-CAST(ISNULL(vcResultQtyDaily28,0) as int)) as day28 ,(CAST(ISNULL(vcInputQtyDaily29,0) as int)-CAST(ISNULL(vcResultQtyDaily29,0) as int)) as day29 ,   \n");
            validateOrdSql.Append("   (CAST(ISNULL(vcInputQtyDaily30,0) as int)-CAST(ISNULL(vcResultQtyDaily30,0) as int)) as day30 ,(CAST(ISNULL(vcInputQtyDaily31,0) as int)-CAST(ISNULL(vcResultQtyDaily31,0) as int)) as day31    \n");
            validateOrdSql.Append("  from  SP_M_ORD where vcPartNo='" + partId + "' and vcOrderNo!=''   \n");
            return excute.ExcuteSqlWithSelectToDT(validateOrdSql.ToString());
        }

        public DataTable ValidateOrd1(string partId)
        {
            StringBuilder validateOrdSql = new StringBuilder();
            validateOrdSql.Append("select (ISNULL(SUM(CAST(vcPlantQtyDailySum as int)),0)-ISNULL(SUM(CAST(vcResultQtyDailySum as int)),0)) as sum  \n");
            validateOrdSql.Append("from SP_M_ORD where ISNULL(vcPlantQtyDailySum,0)!=ISNULL(vcResultQtyDailySum,0)   \n");
            validateOrdSql.Append("and vcPartNo='" + partId + "' and vcOrderNo!=''\n");
            return excute.ExcuteSqlWithSelectToDT(validateOrdSql.ToString());
        }

        public int InsertSell(string seqNo, string sellNo, string truckNo, string cpdCompany, string partId, string kanbanOrderNo, string kanbanSerial, string invoiceNo, string caseNo, string partsNameEn, string quantity, string bianCi, string opearteId, string scanTime, string supplierId, string lblStart, string lblEnd, string price)
        {
            StringBuilder InsertSellSql = new StringBuilder();
            string priceTotal = (double.Parse(quantity) * double.Parse(price)).ToString("0.00");
            InsertSellSql.Append("INSERT INTO TSell(vcBianCi,vcSellNo,vcTruckNo,vcSHF,vcPart_id,vcOrderNo,vcLianFanNo,vcInvoiceNo,vcBoxNo\n");
            InsertSellSql.Append(",vcPartsNameEN,iQuantity,decPriceWithTax,decMoney,vcOperatorID,dOperatorTime,vcYinQuType,vcSender,vcLabelStart,vcLabelEnd,vcSupplier_id)\n");
            InsertSellSql.Append("VALUES('" + seqNo + "','" + sellNo + "','" + truckNo + "','" + cpdCompany + "','" + partId + "','','','" + invoiceNo + "','" + caseNo + "','" + partsNameEn + "'," + quantity + "," + priceTotal + ",null,'" + opearteId + "','" + scanTime + "','" + bianCi + "','" + opearteId + "','" + lblStart + "','" + lblEnd + "','" + supplierId + "')\n");
            return excute.ExcuteSqlWithStringOper(InsertSellSql.ToString());
        }

        public int UpdateDock(string dockSell, string opearteId, string serverTime)
        {
            StringBuilder UpdateDockSql = new StringBuilder();
            UpdateDockSql.Append("  update TSell_DockCar set vcFlag='1',vcOperatorID='" + opearteId + "',dOperatorTime='" + serverTime + "' where vcDockSell='" + dockSell + "'");
            return excute.ExcuteSqlWithStringOper(UpdateDockSql.ToString());
        }

        public DataTable GetData1(string sellNo)
        {
            StringBuilder GetDataSql = new StringBuilder();
            /*
            GetDataSql.Append("select t2.vcInputNo as inv_no,SUBSTRING(t1.vcControlno,2,10) as inv_date,t1.vcPart_id as part_no,\n");
            GetDataSql.Append("t1.vcPartsnamechn as part_name,t2.vcBoxNo as case_no,t1.vcOrderno as ord_no,t1.vcSeqno as item_no,\n");
            GetDataSql.Append("'TFTM' as dlr_no,t2.iQuantity as qty,t1.vcCostwithtaxes as price\n");
            GetDataSql.Append("from TShipList t1 inner join (select distinct vcSellNo,vcInputNo,vcPart_id,vcBoxNo  from TOperateSJ WHERE vcZYType = 'S4' ) t2\n");
            GetDataSql.Append("on t1.vcControlno=t2.vcSellNo and t1.vcPart_id = t2.vcPart_id and t1.vcCaseno = t2.vcBoxNo where t1.vcControlno='"+sellNo+"'\n");

            */
            GetDataSql.Append("select vcInvoiceno as inv_no,SUBSTRING(vcControlno,3,8) as inv_date,vcPart_id as part_no,vcPartsnamechn as part_name,vcCaseno as case_no,vcOrderno as ord_no,vcSeqno as item_no,'TFTM' AS dlr_no,vcShippingqty as qty,vcCostwithtaxes as price from TShipList where vcControlno='" + sellNo + "'");
            return excute.ExcuteSqlWithSelectToDT(GetDataSql.ToString());
        }





        public DataTable GetPoint(string iP)
        {
            StringBuilder GetPointSql = new StringBuilder();
            GetPointSql.Append("select vcPointType,vcPointNo from TPointInfo where vcPointIp='" + iP + "' and vcUsed='在用'");
            return excute.ExcuteSqlWithSelectToDT(GetPointSql.ToString());
        }

        public int InsertShip1(string cpdCompany, string packingSpot, string sellNo, string partId, string invoiceNo, string seqNo, string quantity, string caseNo, string partsNameEn, string opearteId, string iP, string partsNameCn, string price, string serverTime, string supplierId)
        {
            string priceTotal = (double.Parse(quantity) * double.Parse(price)).ToString("0.00");

            StringBuilder InsertShipSql = new StringBuilder();
            InsertShipSql.Append("INSERT INTO TShipList(vcPackingspot,vcSupplier,vcCpdcompany,vcControlno,vcPart_id,vcOrderno,vcSeqno,vcInvoiceno\n");
            InsertShipSql.Append(",vcPartsnamechn,vcPartsnameen,vcShippingqty,vcCostwithtaxes,vcPrice,iNocount,vcCaseno,vcProgramfrom,vcComputernm\n");
            InsertShipSql.Append(",vcPackcode,vcCompany,vcHostip,vcOperatorID,dOperatorTime,dFirstPrintTime,dLatelyPrintTime)\n");
            InsertShipSql.Append("VALUES('" + packingSpot + "','" + supplierId + "','" + cpdCompany + "','" + sellNo + "','" + partId + "','','','" + invoiceNo + "','" + partsNameCn + "','" + partsNameEn + "','" + quantity + "','" + priceTotal + "','','','" + caseNo + "','','','','','" + iP + "','" + opearteId + "','" + serverTime + "',null,null)\n");
            return excute.ExcuteSqlWithStringOper(InsertShipSql.ToString());
        }

        public int UpdateQB(string dockSell)
        {
            StringBuilder UpdateQBSql = new StringBuilder();
            UpdateQBSql.Append("     update TOperatorQB set vcReflectFlag='1' where vcReflectFlag='0' and vcZYType='S4' and vcDockSell='" + dockSell + "'");
            return excute.ExcuteSqlWithStringOper(UpdateQBSql.ToString());
        }



        public DataTable GetBanZhi(string serverTime)
        {

            StringBuilder GetBanZhiSql = new StringBuilder();
            GetBanZhiSql.Append("select dHosDate,vcBanZhi from (select convert(varchar(10),dateadd(DAY,-1,GETDATE()),23) as dHosDate,vcBanZhi,convert(varchar(10),dateadd(DAY,-1,GETDATE()),23)+' '+convert(varchar(10),tFromTime,24) as tFromTime,case when vcCross='1' then convert(varchar(10),dateadd(day,1,dateadd(DAY,-1,GETDATE())),23) else convert(varchar(10),dateadd(DAY,-1,GETDATE()),23) end +' '+convert(varchar(10),tToTime,24) as tToTime from TBZTime where vcBanZhi='夜' and vcPackPlant='H2'\n");
            GetBanZhiSql.Append("union select convert(varchar(10), dateadd(DAY, 0, GETDATE()), 23) as dHosDate, vcBanZhi, convert(varchar(10), dateadd(DAY, 0, GETDATE()), 23) + ' ' + convert(varchar(10), tFromTime, 24) as tFromTime,case when vcCross = '1' then convert(varchar(10), dateadd(day, 1, dateadd(DAY, 0, GETDATE())), 23) else convert(varchar(10), dateadd(DAY, 0, GETDATE()), 23) end + ' ' + convert(varchar(10), tToTime, 24) as tToTime from TBZTime where vcBanZhi = '白' and vcPackPlant = 'H2'\n");
            GetBanZhiSql.Append("union select convert(varchar(10), dateadd(DAY, 0, GETDATE()), 23) as dHosDate, vcBanZhi, convert(varchar(10), dateadd(DAY, 0, GETDATE()), 23) + ' ' + convert(varchar(10), tFromTime, 24) as tFromTime,case when vcCross = '1' then convert(varchar(10), dateadd(day, 1, dateadd(DAY, 0, GETDATE())), 23) else convert(varchar(10), dateadd(DAY, 0, GETDATE()), 23) end + ' ' + convert(varchar(10), tToTime, 24) as tToTime from TBZTime where vcBanZhi = '夜' and vcPackPlant = 'H2'\n");
            GetBanZhiSql.Append("union select convert(varchar(10), dateadd(DAY, 1, GETDATE()), 23) as dHosDate, vcBanZhi, convert(varchar(10), dateadd(DAY, 1, GETDATE()), 23) + ' ' + convert(varchar(10), tFromTime, 24) as tFromTime,case when vcCross = '1' then convert(varchar(10), dateadd(day, 1, dateadd(DAY, 1, GETDATE())), 23) else convert(varchar(10), dateadd(DAY, 1, GETDATE()), 23) end + ' ' + convert(varchar(10), tToTime, 24) as tToTime from TBZTime where vcBanZhi = '白' and vcPackPlant = 'H2'\n");
            GetBanZhiSql.Append(")t where tFromTime<=GETDATE() and tToTime>=GETDATE()\n");
            return excute.ExcuteSqlWithSelectToDT(GetBanZhiSql.ToString());

        }

        public int InsertSum(string seqNo, string sellNo, string truckNo, int caseSum, string bianCi, string opearteId, string serverTime, string date, string banzhi, string qianFen)
        {
            StringBuilder InsertSumSql = new StringBuilder();
            InsertSumSql.Append("INSERT INTO TSell_Sum (vcBianCi ,vcSellNo ,vcTruckNo,iToolQuantity ,vcYinQuType ,vcOperatorID ,dOperatorTime,vcBanZhi,vcDate,vcQianFen)\n");
            InsertSumSql.Append("     VALUES ('" + seqNo + "','" + sellNo + "','" + truckNo + "','" + caseSum + "','" + bianCi + "','" + opearteId + "','" + serverTime + "','" + banzhi + "','" + date + "','" + qianFen + "')\n");

            return excute.ExcuteSqlWithStringOper(InsertSumSql.ToString());
        }

        public int UpdateShip(string dockSell, string opearteId, string serverTime)
        {
            StringBuilder UpdateShipSql = new StringBuilder();
            UpdateShipSql.Append("  update TShip_Temp set vcFlag='1',dOperatorTime='" + serverTime + "',vcOperatorID='" + opearteId + "' where vcDockSell='" + dockSell + "' ");

            return excute.ExcuteSqlWithStringOper(UpdateShipSql.ToString());
        }

        public int UpdateInv1(string partId, string kanbanOrderNo, string kanbanSerial, string quantity)
        {
            StringBuilder UpdateInvSql = new StringBuilder();
            UpdateInvSql.Append("update TOperateSJ_InOutput set iDCH=iDCH-" + int.Parse(quantity) + " where vcPart_id='" + partId + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "'");

            return excute.ExcuteSqlWithStringOper(UpdateInvSql.ToString());
        }



        public int InsertSj(string packingSpot, string inputNo, string kanbanOrderNo, string kanbanSerial, string partId, string inoutFlag, string supplierId, string supplierPlant, string scanTime, string serverTime, string quantity, string packingQuatity, string cpdCompany, string dock, string checkType, string lblStart, string lblEnd, string opearteId, string checkStatus, string caseNo, string sellNo, string iP, string pointType)
        {
            StringBuilder InsertOprSql = new StringBuilder();
            InsertOprSql.Append("INSERT INTO TOperateSJ (vcZYType,vcBZPlant,vcInputNo,vcKBOrderNo,vcKBLFNo,vcPart_id,vcIOType,vcSupplier_id,vcSupplierGQ,dStart\n");
            InsertOprSql.Append(",dEnd,iQuantity,vcBZUnit,vcSHF,vcSR,vcBoxNo,vcSheBeiNo,vcCheckType,iCheckNum,vcCheckStatus,vcLabelStart,vcLabelEnd\n");
            InsertOprSql.Append(",vcUnlocker,dUnlockTime,vcSellNo,vcOperatorID,dOperatorTime,vcHostIp,packingcondition,vcPackingPlant)\n");
            InsertOprSql.Append("VALUES('S4','" + packingSpot + "','" + inputNo + "','" + kanbanOrderNo + "','" + kanbanSerial + "','" + partId + "','" + inoutFlag + "','" + supplierId + "','" + supplierPlant + "','" + scanTime + "','" + serverTime + "',\n");
            InsertOprSql.Append("" + quantity + ",'" + packingQuatity + "','" + cpdCompany + "','" + dock + "','" + caseNo + "','" + pointType + "','" + checkType + "'," + quantity + ",'" + checkStatus + "','" + lblStart + "','" + lblEnd + "','',null,'" + sellNo + "','" + opearteId + "','" + serverTime + "','" + iP + "','1','')\n");
            return excute.ExcuteSqlWithStringOper(InsertOprSql.ToString());
        }

        public DataTable GetOprData(string caseNo, string inputNo)
        {
            StringBuilder GetOprDataSql = new StringBuilder();
            GetOprDataSql.Append("select iQuantity,vcCheckStatus from TOperateSJ where vcBoxNo='" + caseNo + "' and vcInputNo='" + inputNo + "' and vcZYType='S3'");


            return excute.ExcuteSqlWithSelectToDT(GetOprDataSql.ToString());
        }

        public int UpdateSeqNo(int seqNoNew, string formatDate, string tmpString)
        {
            StringBuilder UpdateSeqNoSql = new StringBuilder();
            UpdateSeqNoSql.Append("update TSeqNo set SEQNO='" + seqNoNew + "' where DDATE='" + formatDate + "' and FLAG='" + tmpString + "'");
            return excute.ExcuteSqlWithStringOper(UpdateSeqNoSql.ToString());
        }

        public int InsertSeqNo(string tmpString, string formatDate)
        {
            StringBuilder InsertSeqNoSql = new StringBuilder();
            InsertSeqNoSql.Append("INSERT INTO  TSeqNo(FLAG,DDATE ,SEQNO) VALUES ('" + tmpString + "','" + formatDate + "','1')");

            return excute.ExcuteSqlWithStringOper(InsertSeqNoSql.ToString());
        }

        public DataTable GetSeqNo(string tmpString, string formatDate)
        {
            StringBuilder GetSeqNoSql = new StringBuilder();
            GetSeqNoSql.Append("select SEQNO from TSeqNo where FLAG='" + tmpString + "' and DDATE='" + formatDate + "'");
            return excute.ExcuteSqlWithSelectToDT(GetSeqNoSql.ToString());
        }

        public DataTable GetToolInfo(string sellNo)
        {
            StringBuilder GetToolInfoSql = new StringBuilder();
            GetToolInfoSql.Append("  SELECT vcToolCode,iToolQuantity FROM TSell_Tool WHERE vcSellNo='" + sellNo + "' ORDER BY vcToolCode");



            return excute.ExcuteSqlWithSelectToDT(GetToolInfoSql.ToString());
        }

        public DataTable GetSellData(string timeFrom, string timeEnd, string type, string date, string banZhi)
        {
            StringBuilder GetSellDataSql = new StringBuilder();
            GetSellDataSql.Append("  select * from TSell_Sum where vcYinQuType='" + type + "' and vcBanZhi='" + banZhi + "' and vcDate='" + date + "'");
            return excute.ExcuteSqlWithSelectToDT(GetSellDataSql.ToString());
        }

        public DataTable GetCode()
        {
            StringBuilder GetCodeSql = new StringBuilder();
            GetCodeSql.Append("select vcValue2 as 'address',vcValue1 as 'displayName' from TOutCode where vcCodeId='C100'");
            return excute.ExcuteSqlWithSelectToDT(GetCodeSql.ToString());
        }

        public DataTable ValidateSeqNo(string packingSpot, string formatServerTime, string tmpString)
        {
            StringBuilder ValidateSeqNoSql = new StringBuilder();
            ValidateSeqNoSql.Append("SELECT  FLAG,DDATE,SEQNO\n");
            ValidateSeqNoSql.Append("  FROM TSeqNo where FLAG='" + tmpString + "'+'" + packingSpot + "' AND DDATE='" + formatServerTime + "'");
            return excute.ExcuteSqlWithSelectToDT(ValidateSeqNoSql.ToString());
        }

        public int InsertSeqNo(string packingSpot, string formatServerTime, string tmpString)
        {
            StringBuilder InsertSeqNoSql = new StringBuilder();
            InsertSeqNoSql.Append("INSERT INTO TSeqNo(FLAG ,DDATE ,SEQNO) \n");
            InsertSeqNoSql.Append("VALUES( '" + tmpString + "'+'" + packingSpot + "','" + formatServerTime + "','1')");
            return excute.ExcuteSqlWithStringOper(InsertSeqNoSql.ToString());
        }

        public DataTable ValidateOpr(string caseNo)
        {
            StringBuilder ValidateOprSql = new StringBuilder();
            ValidateOprSql.Append("select vcInputNo,vcPart_id,vcSHF,vcSR,iQuantity,vcKBOrderNo,vcKBLFNo,vcIOType,vcSupplier_id,vcSupplierGQ,vcBZUnit,vcCheckType,vcLabelStart,vcLabelEnd,vcCheckStatus from TOperateSJ where vcBoxNo='" + caseNo + "' and vcZYType='S3'");
            return excute.ExcuteSqlWithSelectToDT(ValidateOprSql.ToString());
        }

        public DataTable ValidateOpr1(string caseNo, string caseNo1)
        {
            StringBuilder ValidateOprSql = new StringBuilder();
            ValidateOprSql.Append("select vcInputNo,vcPart_id,vcSHF,vcSR,iQuantity,vcKBOrderNo,vcKBLFNo from TOperateSJ where vcBoxNo='" + caseNo + "' and vcZYType='S4'");
            return excute.ExcuteSqlWithSelectToDT(ValidateOprSql.ToString());
        }

        public int UpdateSeqNo(string packingSpot, string formatServerTime, string shpSqnNoNew, string tmpString)
        {
            StringBuilder UpdateSeqNoSql = new StringBuilder();
            UpdateSeqNoSql.Append("UPDATE TSeqNo SET SEQNO = " + shpSqnNoNew + "\n");
            UpdateSeqNoSql.Append("  WHERE FLAG = ('" + tmpString + "'+'" + packingSpot + "') and DDATE='" + formatServerTime + "'\n");
            return excute.ExcuteSqlWithStringOper(UpdateSeqNoSql.ToString());
        }





        public int UpdateOrd(string targetMonth, string orderNo, string seqNo, int v1, int v2, int v3, int v4, int v5, int v6, int v7, int v8, int v9, int v10, int v11, int v12, int v13, int v14, int v15, int v16, int v17, int v18, int v19, int v20, int v21, int v22, int v23, int v24, int v25, int v26, int v27, int v28, int v29, int v30, int v31, int newSum, string partId)
        {
            StringBuilder UpdateOrdSql = new StringBuilder();
            UpdateOrdSql.Append("update SP_M_ORD set vcResultQtyDaily1=isnull(cast(vcResultQtyDaily1 as int),0)+" + v1 + ",vcResultQtyDaily2=isnull(cast(vcResultQtyDaily2 as int),0)+" + v2 + ",vcResultQtyDaily3=isnull(cast(vcResultQtyDaily3 as int),0)+" + v3 + ",vcResultQtyDaily4=isnull(cast(vcResultQtyDaily4 as int),0)+" + v4 + ",vcResultQtyDaily5=isnull(cast(vcResultQtyDaily5 as int),0)+" + v5 + ",vcResultQtyDaily6=isnull(cast(vcResultQtyDaily6 as int),0)+" + v6 + ",vcResultQtyDaily7=isnull(cast(vcResultQtyDaily7 as int),0)+" + v7 + ",vcResultQtyDaily8=isnull(cast(vcResultQtyDaily8 as int),0)+" + v8 + ",vcResultQtyDaily9=isnull(cast(vcResultQtyDaily9 as int),0)+" + v9 + ",vcResultQtyDaily10=isnull(cast(vcResultQtyDaily10 as int),0)+" + v10 + ",\n");
            UpdateOrdSql.Append("vcResultQtyDaily11=isnull(cast(vcResultQtyDaily11 as int),0)+" + v11 + ",vcResultQtyDaily12=isnull(cast(vcResultQtyDaily12 as int),0)+" + v12 + ",vcResultQtyDaily13=isnull(cast(vcResultQtyDaily13 as int),0)+" + v13 + ",vcResultQtyDaily14=isnull(cast(vcResultQtyDaily14 as int),0)+" + v14 + ",vcResultQtyDaily15=isnull(cast(vcResultQtyDaily15 as int),0)+" + v15 + ",vcResultQtyDaily16=isnull(cast(vcResultQtyDaily16 as int),0)+" + v16 + ",vcResultQtyDaily17=isnull(cast(vcResultQtyDaily17 as int),0)+" + v17 + ",vcResultQtyDaily18=isnull(cast(vcResultQtyDaily18 as int),0)+" + v18 + ",vcResultQtyDaily19=isnull(cast(vcResultQtyDaily19 as int),0)+" + v19 + ",vcResultQtyDaily20=isnull(cast(vcResultQtyDaily20 as int),0)+" + v20 + ",\n");
            UpdateOrdSql.Append("vcResultQtyDaily21=isnull(cast(vcResultQtyDaily21 as int),0)+" + v21 + ",vcResultQtyDaily22=isnull(cast(vcResultQtyDaily22 as int),0)+" + v22 + ",vcResultQtyDaily23=isnull(cast(vcResultQtyDaily23 as int),0)+" + v23 + ",vcResultQtyDaily24=isnull(cast(vcResultQtyDaily24 as int),0)+" + v24 + ",vcResultQtyDaily25=isnull(cast(vcResultQtyDaily25 as int),0)+" + v25 + ",vcResultQtyDaily26=isnull(cast(vcResultQtyDaily26 as int),0)+" + v26 + ",vcResultQtyDaily27=isnull(cast(vcResultQtyDaily27 as int),0)+" + v27 + ",vcResultQtyDaily28=isnull(cast(vcResultQtyDaily28 as int),0)+" + v28 + ",vcResultQtyDaily29=isnull(cast(vcResultQtyDaily29 as int),0)+" + v29 + ",vcResultQtyDaily30=isnull(cast(vcResultQtyDaily30 as int),0)+" + v30 + ",\n");
            UpdateOrdSql.Append("vcResultQtyDaily31=isnull(cast(vcResultQtyDaily31 as int),0)+" + v31 + ",vcResultQtyDailySum=isnull(cast(vcResultQtyDailySum as int),0)+" + newSum + " where vcTargetYearMonth='" + targetMonth + "' and vcOrderNo='" + orderNo + "' and vcSeqno='" + seqNo + "' and vcPartNo='" + partId + "' \n");
            return excute.ExcuteSqlWithStringOper(UpdateOrdSql.ToString());
        }

        public int InsertPrint(string opearteId, string iP, string serverTime, string sellNo)
        {
            StringBuilder InsertPrintSql = new StringBuilder();
            InsertPrintSql.Append("INSERT INTO TPrint_Temp(vcTableName ,vcReportName ,vcClientIP ,vcPrintName\n");
            InsertPrintSql.Append("           ,vcKind,vcOperatorID ,dOperatorTime ,vcCaseNo,vcSellNo,vcLotid,vcFlag)\n");
            InsertPrintSql.Append("     VALUES('TShipList','SPR07SHPP','" + iP + "','LASEL PRINTER','4','" + opearteId + "','" + serverTime + "','','" + sellNo + "','','0')\n");
            return excute.ExcuteSqlWithStringOper(InsertPrintSql.ToString());
        }

        public int InsertShip(string cpdName, string cpdCompany, string packingSpot, string sellNo, string partId, string orderNo, string seqNo, string quantity, string caseNo, string partsName, string opearteId, string iP, string partsNameCn, string price)
        {
            StringBuilder InsertShipSql = new StringBuilder();
            InsertShipSql.Append("INSERT INTO TShipList (vcPackingspot ,vcSupplier ,vcCpdcompany,vcControlno ,vcPart_id ,vcOrderno ,vcSeqno \n");

            InsertShipSql.Append("            ,vcInvoiceno ,vcPartsnamechn,vcPartsnameen ,vcShippingqty ,vcCostwithtaxes ,vcPrice ,iNocount\n");
            InsertShipSql.Append(" , vcCaseno, vcProgramfrom, vcComputernm, vcPackcode, vcCompany, vcHostip, vcOperatorID, dOperatorTime)\n");
            InsertShipSql.Append(" VALUES('" + packingSpot + "','','" + cpdCompany + "','" + sellNo + "','" + partId + "','','','','" + partsNameCn + "','" + partsName + "','" + int.Parse(quantity) + "','" + int.Parse(quantity) * int.Parse(price) + "','',null,'" + caseNo + "','','','','','" + iP + "','" + opearteId + "',NULL)\n");
            return excute.ExcuteSqlWithStringOper(InsertShipSql.ToString());
        }

        public DataTable GetPartsName(string serverTime, string partId)
        {
            StringBuilder getPartsNameSql = new StringBuilder();
            string time = serverTime.Replace("-", "").Substring(0, 8);
            getPartsNameSql.Append("select vcPartENName	 from TSPMaster WHERE vcPartId='" + partId + "' and dFromTime<='" + time + "' and dToTime>='" + time + "'");
            return excute.ExcuteSqlWithSelectToDT(getPartsNameSql.ToString());
        }

        public DataTable ValidateOrd(string partId)
        {
            StringBuilder validateOrdSql = new StringBuilder();
            validateOrdSql.Append(" select  vcTargetYearMonth,vcOrderType,vcOrderNo,vcSeqno,(CAST(ISNULL(vcPlantQtyTotal,0) as int)-CAST(ISNULL(vcResultQtyTotal,0) as int)) as sum,(CAST(ISNULL(vcPlantQtyDaily1,0) as int)-CAST(ISNULL(vcResultQtyDaily1,0) as int)) as day1 ,   \n");
            validateOrdSql.Append("  (CAST(ISNULL(vcPlantQtyDaily2,0) as int)-CAST(ISNULL(vcResultQtyDaily2,0) as int)) as day2 ,(CAST(ISNULL(vcPlantQtyDaily3,0) as int)-CAST(ISNULL(vcResultQtyDaily3,0) as int)) as day3 ,   \n");
            validateOrdSql.Append("  (CAST(ISNULL(vcPlantQtyDaily4,0) as int)-CAST(ISNULL(vcResultQtyDaily4,0) as int)) as day4 ,(CAST(ISNULL(vcPlantQtyDaily5,0) as int)-CAST(ISNULL(vcResultQtyDaily5,0) as int)) as day5 ,   \n");
            validateOrdSql.Append("  (CAST(ISNULL(vcPlantQtyDaily6,0) as int)-CAST(ISNULL(vcResultQtyDaily6,0) as int)) as day6 ,(CAST(ISNULL(vcPlantQtyDaily7,0) as int)-CAST(ISNULL(vcResultQtyDaily7,0) as int)) as day7 ,   \n");
            validateOrdSql.Append("  (CAST(ISNULL(vcPlantQtyDaily8,0) as int)-CAST(ISNULL(vcResultQtyDaily8,0) as int)) as day8 ,(CAST(ISNULL(vcPlantQtyDaily9,0) as int)-CAST(ISNULL(vcResultQtyDaily9,0) as int)) as day9 ,   \n");
            validateOrdSql.Append("  (CAST(ISNULL(vcPlantQtyDaily10,0) as int)-CAST(ISNULL(vcResultQtyDaily10,0) as int)) as day10 ,(CAST(ISNULL(vcPlantQtyDaily11,0) as int)-CAST(ISNULL(vcResultQtyDaily11,0) as int)) as day11 ,   \n");
            validateOrdSql.Append("  (CAST(ISNULL(vcPlantQtyDaily12,0) as int)-CAST(ISNULL(vcResultQtyDaily12,0) as int)) as day12 ,(CAST(ISNULL(vcPlantQtyDaily13,0) as int)-CAST(ISNULL(vcResultQtyDaily13,0) as int)) as day13 ,   \n");
            validateOrdSql.Append("  (CAST(ISNULL(vcPlantQtyDaily14,0) as int)-CAST(ISNULL(vcResultQtyDaily14,0) as int)) as day14 ,(CAST(ISNULL(vcPlantQtyDaily15,0) as int)-CAST(ISNULL(vcResultQtyDaily15,0) as int)) as day15 ,   \n");
            validateOrdSql.Append("  (CAST(ISNULL(vcPlantQtyDaily16,0) as int)-CAST(ISNULL(vcResultQtyDaily16,0) as int)) as day16 ,(CAST(ISNULL(vcPlantQtyDaily17,0) as int)-CAST(ISNULL(vcResultQtyDaily17,0) as int)) as day17 ,   \n");
            validateOrdSql.Append("  (CAST(ISNULL(vcPlantQtyDaily18,0) as int)-CAST(ISNULL(vcResultQtyDaily18,0) as int)) as day18 ,(CAST(ISNULL(vcPlantQtyDaily19,0) as int)-CAST(ISNULL(vcResultQtyDaily19,0) as int)) as day19 ,   \n");
            validateOrdSql.Append("   (CAST(ISNULL(vcPlantQtyDaily20,0) as int)-CAST(ISNULL(vcResultQtyDaily20,0) as int)) as day20 ,(CAST(ISNULL(vcPlantQtyDaily21,0) as int)-CAST(ISNULL(vcResultQtyDaily21,0) as int)) as day21 ,   \n");
            validateOrdSql.Append("  (CAST(ISNULL(vcPlantQtyDaily22,0) as int)-CAST(ISNULL(vcResultQtyDaily22,0) as int)) as day22 ,(CAST(ISNULL(vcPlantQtyDaily23,0) as int)-CAST(ISNULL(vcResultQtyDaily23,0) as int)) as day23 ,   \n");
            validateOrdSql.Append("  (CAST(ISNULL(vcPlantQtyDaily24,0) as int)-CAST(ISNULL(vcResultQtyDaily24,0) as int)) as day24 ,(CAST(ISNULL(vcPlantQtyDaily25,0) as int)-CAST(ISNULL(vcResultQtyDaily25,0) as int)) as day25 ,   \n");
            validateOrdSql.Append("  (CAST(ISNULL(vcPlantQtyDaily26,0) as int)-CAST(ISNULL(vcResultQtyDaily26,0) as int)) as day26 ,(CAST(ISNULL(vcPlantQtyDaily27,0) as int)-CAST(ISNULL(vcResultQtyDaily27,0) as int)) as day27 ,   \n");
            validateOrdSql.Append("  (CAST(ISNULL(vcPlantQtyDaily28,0) as int)-CAST(ISNULL(vcResultQtyDaily28,0) as int)) as day28 ,(CAST(ISNULL(vcPlantQtyDaily29,0) as int)-CAST(ISNULL(vcResultQtyDaily29,0) as int)) as day29 ,   \n");
            validateOrdSql.Append("   (CAST(ISNULL(vcPlantQtyDaily30,0) as int)-CAST(ISNULL(vcResultQtyDaily30,0) as int)) as day30 ,(CAST(ISNULL(vcPlantQtyDaily31,0) as int)-CAST(ISNULL(vcResultQtyDaily31,0) as int)) as day31    \n");
            validateOrdSql.Append("  from  SP_M_ORD where vcPartNo='" + partId + "' and vcOrderNo!=''   \n");
            return excute.ExcuteSqlWithSelectToDT(validateOrdSql.ToString());
        }

        public DataTable GetPlantCode(string partsNoFirst, string serverTime)
        {
            StringBuilder GetPlantCodeSql = new StringBuilder();
            string time = serverTime.Replace("-", "").Substring(0, 8);
            GetPlantCodeSql.Append("SELECT PLANTCODE FROM TSitem \n");
            GetPlantCodeSql.Append("WHERE PARTSNO='" + partsNoFirst + "' AND TIMEFROM<='" + time + "' AND TIMETO>='" + time + "' AND PACKINGSPOT='H2'  ORDER BY TIMEFROM DESC");
            return excute.ExcuteSqlWithSelectToDT(GetPlantCodeSql.ToString());
        }

        public DataTable ValidateParts(string caseNo)
        {
            StringBuilder ValidatePartsSql = new StringBuilder();
            ValidatePartsSql.Append("  select distinct(vcPart_id) from TOperateSJ where vcBoxNo='" + caseNo + "' and vcZYType='S3'");

            return excute.ExcuteSqlWithSelectToDT(ValidatePartsSql.ToString());
        }

        public int Truncate()
        {
            StringBuilder TruncateSql = new StringBuilder();
            TruncateSql.Append("  truncate table TShip_Temp");
            return excute.ExcuteSqlWithStringOper(TruncateSql.ToString());
        }

        public DataTable ValidateRinv(string bzPlant, string partId, string cpdCompany)
        {
            StringBuilder ValidateRinvSql = new StringBuilder();
            ValidateRinvSql.Append("  SELECT PARTSNO FROM TRinv WHERE PARTSNO='" + partId + "' AND PACKINGSPOT='" + bzPlant + "' AND  CPDCOMPANY='" + cpdCompany + "'");
            return excute.ExcuteSqlWithSelectToDT(ValidateRinvSql.ToString());
        }

        public DataTable ValidateInv(string partId, string quantity, string dock, string kanbanOrderNo, string kanbanSerial)
        {
            StringBuilder ValidateInvSql = new StringBuilder();
            ValidateInvSql.Append("  select vcBZPlant,vcSHF,vcInputNo,iDBZ,iDZX,iDCH from TOperateSJ_InOutput where vcPart_id='" + partId + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "'");
            return excute.ExcuteSqlWithSelectToDT(ValidateInvSql.ToString());
        }

        public int UpdateRinv(string bzPlant, string partId, string cpdCompany, int producyqty)
        {
            StringBuilder UpdateRinvSql = new StringBuilder();
            UpdateRinvSql.Append("  update TRinv set PRODUCTQTY=PRODUCTQTY+" + producyqty + " where PACKINGSPOT='" + bzPlant + "' and PARTSNO='" + partId + "' and CPDCOMPANY='" + cpdCompany + "'");
            return excute.ExcuteSqlWithStringOper(UpdateRinvSql.ToString());
        }

        public int Insert(string partId, string quantity, string dock, string kanbanOrderNo, string kanbanSerial, string serverTime, string trolleyNo, string cpdCompany, string inoNo, string caseNo)
        {
            StringBuilder InsertSql = new StringBuilder();
            InsertSql.Append("INSERT INTO TOperatorQB(vcZYType,dScanTime,vcHtNo,vcTrolleyNo ,vcInputNo,vcPart_id,vcCpdCompany,vcSR ,vcBoxNo,iQuantity  \n");
            InsertSql.Append(" ,vcSeqNo,vcReflectFlag,dStart,dEnd,vcHostIp,vcKBOrderNo,vcKBLFNo,vcOperatorID ,dOperatorTime)  \n");
            InsertSql.Append("    VALUES('S4','" + serverTime + "', '', '" + trolleyNo + "', '" + inoNo + "', '" + partId + "', '" + cpdCompany + "', '" + dock + "', '" + caseNo + "', " + int.Parse(quantity) + ", '', '', null, null, '', '" + kanbanOrderNo + "', '" + kanbanSerial + "', '', '" + serverTime + "')");
            return excute.ExcuteSqlWithStringOper(InsertSql.ToString());
        }



        public int UpdateInv(string partId, string quantity, string dock, string kanbanOrderNo, string kanbanSerial, string serverTime)
        {
            StringBuilder UpdateInvSql = new StringBuilder();
            UpdateInvSql.Append(" update TOperateSJ_InOutput set iDCH=iDCH-" + int.Parse(quantity) + " where vcPart_id='" + partId + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "'");
            return excute.ExcuteSqlWithStringOper(UpdateInvSql.ToString());
        }

        public DataTable ValidateQB(string caseNo)
        {
            StringBuilder validateQBSql = new StringBuilder();
            validateQBSql.Append("select vcTrolleyNo,vcInputNo,vcPart_id,vcCpdCompany,vcSR,iQuantity,vcKBOrderNo,vcKBLFNo from TOperatorQB where vcBoxNo='" + caseNo + "' and vcZYType='S3'");
            return excute.ExcuteSqlWithSelectToDT(validateQBSql.ToString());
        }

        public int InsertOpr(string bzPlant, string inoNo, string kanbanOrderNo, string kanbanSerial, string partId, string inoutFlag, string supplier_id, string supplierGQ, string serverTime, string quantity, string bZUnit, string cpdCompany, string dock, string checkType, string labelStart, string labelEnd, string caseNo, string checkStatus, string sellNo)
        {
            StringBuilder InsertOprSql = new StringBuilder();
            InsertOprSql.Append("INSERT INTO TOperateSJ (vcZYType,vcBZPlant,vcInputNo,vcKBOrderNo,vcKBLFNo,vcPart_id,vcIOType,vcSupplier_id,vcSupplierGQ,dStart\n");
            InsertOprSql.Append(",dEnd,iQuantity,vcBZUnit,vcSHF,vcSR,vcBoxNo,vcSheBeiNo,vcCheckType,iCheckNum,vcCheckStatus,vcLabelStart,vcLabelEnd\n");
            InsertOprSql.Append(",vcUnlocker,dUnlockTime,vcSellNo,vcOperatorID,dOperatorTime)\n");
            InsertOprSql.Append("VALUES('S4','" + bzPlant + "','" + inoNo + "','" + kanbanOrderNo + "','" + kanbanSerial + "','" + partId + "','" + inoutFlag + "','" + supplier_id + "','" + supplierGQ + "','" + serverTime + "','" + serverTime + "',\n");
            InsertOprSql.Append("" + int.Parse(quantity) + ",'" + bZUnit + "','" + cpdCompany + "','" + dock + "','" + caseNo + "','1','" + checkType + "'," + int.Parse(quantity) + ",'" + checkStatus + "','" + labelStart + "','" + labelEnd + "','',null,'" + sellNo + "','system','" + serverTime + "')\n");
            return excute.ExcuteSqlWithStringOper(InsertOprSql.ToString());
        }

        public int UpdateOrd(string targetMonth, string orderGroup, string orderNo, string seqNo, string releaseDate, int v1, int v2, int v3, int v4, int v5, int v6, int v7, int v8, int v9, int v10, int v11, int v12, int v13, int v14, int v15, int v16, int v17, int v18, int v19, int v20, int v21, int v22, int v23, int v24, int v25, int v26, int v27, int v28, int v29, int v30, int v31, int newSum)
        {
            StringBuilder UpdateOrdSql = new StringBuilder();
            UpdateOrdSql.Append("update TOrd set resultqtydaily1=resultqtydaily1+" + v1 + ",resultqtydaily2=resultqtydaily2+" + v2 + ",resultqtydaily3=resultqtydaily3+" + v3 + ",resultqtydaily4=resultqtydaily4+" + v4 + ",resultqtydaily5=resultqtydaily5+" + v5 + ",resultqtydaily6=resultqtydaily6+" + v6 + ",resultqtydaily7=resultqtydaily7+" + v7 + ",resultqtydaily8=resultqtydaily8+" + v8 + ",resultqtydaily9=resultqtydaily9+" + v9 + ",resultqtydaily10=resultqtydaily10+" + v10 + ",\n");
            UpdateOrdSql.Append("resultqtydaily11=resultqtydaily11+" + v11 + ",resultqtydaily12=resultqtydaily12+" + v12 + ",resultqtydaily13=resultqtydaily13+" + v13 + ",resultqtydaily14=resultqtydaily14+" + v14 + ",resultqtydaily15=resultqtydaily15+" + v15 + ",resultqtydaily16=resultqtydaily16+" + v16 + ",resultqtydaily17=resultqtydaily17+" + v17 + ",resultqtydaily18=resultqtydaily18+" + v18 + ",resultqtydaily19=resultqtydaily19+" + v19 + ",resultqtydaily20=resultqtydaily20+" + v20 + ",\n");
            UpdateOrdSql.Append("resultqtydaily21=resultqtydaily21+" + v21 + ",resultqtydaily22=resultqtydaily22+" + v22 + ",resultqtydaily23=resultqtydaily23+" + v23 + ",resultqtydaily24=resultqtydaily24+" + v24 + ",resultqtydaily25=resultqtydaily25+" + v25 + ",resultqtydaily26=resultqtydaily26+" + v26 + ",resultqtydaily27=resultqtydaily27+" + v27 + ",resultqtydaily28=resultqtydaily28+" + v28 + ",resultqtydaily29=resultqtydaily29+" + v29 + ",resultqtydaily30=resultqtydaily30+" + v30 + ",\n");
            UpdateOrdSql.Append("resultqtydaily31=resultqtydaily31+" + v31 + ",resultqtytotal=resultqtytotal+" + newSum + "  where targetmonth='" + targetMonth + "' and ordergroup='" + orderGroup + "' and orderno='" + orderNo + "' and seqno='" + seqNo + "' and releasedatetime='" + releaseDate + "'");
            return excute.ExcuteSqlWithStringOper(UpdateOrdSql.ToString());
        }

        public DataTable ValidateQB1(string caseNo)
        {
            StringBuilder validateQBSql = new StringBuilder();
            validateQBSql.Append("select vcTrolleyNo,vcInputNo,vcPart_id,vcCpdCompany,vcSR,iQuantity,vcKBOrderNo,vcKBLFNo from TOperatorQB where vcBoxNo='" + caseNo + "' and vcZYType='S4'");
            return excute.ExcuteSqlWithSelectToDT(validateQBSql.ToString());
        }

        public DataTable GetShipData(string dock)
        {
            StringBuilder GetShipDataSql = new StringBuilder();
            GetShipDataSql.Append("  select distinct(vcBoxNo) from TShip_Temp where vcDockSell='" + dock + "' and vcFlag='0'");
            return excute.ExcuteSqlWithSelectToDT(GetShipDataSql.ToString());
        }

        //========================================================================重写========================================================================
        public DataTable getDockAndForkInfo(string dock, string fork, string strFlag)
        {
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("select * from  TSell_DockCar where  vcFlag='" + strFlag + "' and vcForkNo='" + fork + "'");
            //stringBuilder.AppendLine(" and vcDockSell='" + dock + "'");
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }
        public void setDockAndForkInfo(string strType, string dock, string fork, string strFlag, string strOperatorID)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (strType == "绑定")
            {
                stringBuilder.AppendLine("INSERT INTO [dbo].[TSell_DockCar]");
                stringBuilder.AppendLine("           ([vcForkNo],[vcDockSell],[vcFlag],[vcOperatorID],[dOperatorTime])");
                stringBuilder.AppendLine("     VALUES('" + fork + "','" + dock + "','" + strFlag + "','" + strOperatorID + "',GETDATE())");
            }
            if (strType == "解绑")
            {
                stringBuilder.AppendLine("update TSell_DockCar set  vcFlag='" + strFlag + "',vcOperatorID='" + strOperatorID + "',dOperatorTime=GETDATE() ");
                stringBuilder.AppendLine("where vcForkNo='" + fork + "' and vcDockSell='" + dock + "'");
            }
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }
        public DataSet checkDockAndForkInfo(string dock, string fork)
        {
            StringBuilder stringBuilder = new StringBuilder();
            //绑定中的
            stringBuilder.AppendLine("select * from TSell_DockCar where vcDockSell='" + dock + "' and vcFlag='0'");
            //已出货或者箱号已删除
            stringBuilder.AppendLine("select * from TShip_Temp where  vcDockSell='" + dock + "' and vcForkNo='" + fork + "' and vcFlag='0'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }
        public DataTable getBoxList(string dock, string fork)
        {
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("select distinct(vcBoxNo) from TShip_Temp where vcFlag='0'");
            if (dock != "")
            {
                stringBuilder.AppendLine("and vcDockSell='" + dock + "'");
            }
            if (fork != "")
            {
                stringBuilder.AppendLine("and vcForkNo='" + fork + "'");
            }
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }
        public void delBoxList(string caseNo, string dock, string fork)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("update TShip_Temp set vcFlag='1' where vcBoxNo='" + caseNo + "'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }
        public DataSet getDockInfo(string dock, string fork, string strFlag, string strPackingPlant)
        {
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("--本次出荷主数据表");
            stringBuilder.AppendLine("select a.vcForkNo--叉车号");
            stringBuilder.AppendLine("		,a.vcDockSell--DOCK号");
            stringBuilder.AppendLine("		,a.vcBoxNo as vcCaseNo--箱号全位");
            stringBuilder.AppendLine("		,c.vcBoxNo--箱号截位");
            stringBuilder.AppendLine("		,b.vcPart_id as vcPart_id--品番");
            stringBuilder.AppendLine("		,b.vcOrderNo as vcKBOrderNo --");
            stringBuilder.AppendLine("		,b.vcLianFanNo as vcKBLFNo");
            stringBuilder.AppendLine("		,b.vcSR");
            stringBuilder.AppendLine("		,c.vcZYType as vcFlag_zx--已装箱标志");
            stringBuilder.AppendLine("		,c.vcBZPlant");
            stringBuilder.AppendLine("		,c.vcInputNo");
            stringBuilder.AppendLine("		,c.vcIOType");
            stringBuilder.AppendLine("		,c.vcSupplier_id");
            stringBuilder.AppendLine("		,c.vcSupplierGQ");
            stringBuilder.AppendLine("		,CAST(isnull(d.iQuantity, 0) AS INT) as iQuantity_rk--入库量");
            stringBuilder.AppendLine("		,CAST(isnull(c.iQuantity, 0) AS INT) as iQuantity_bcck--本次出库量");
            stringBuilder.AppendLine("		,CAST(isnull(g.vcKeResultQty, 0) AS INT) as iQuantity_kck--可出库量");
            stringBuilder.AppendLine("		,CAST(isnull(d.iDCH, 0) AS INT) as iQuantity_dck--待出库量");
            stringBuilder.AppendLine("		,c.vcBZUnit");
            stringBuilder.AppendLine("		,c.vcSHF");
            stringBuilder.AppendLine("		,c.vcCheckType");
            stringBuilder.AppendLine("		,c.iCheckNum");
            stringBuilder.AppendLine("		,c.vcCheckStatus");
            stringBuilder.AppendLine("		,c.vcLabelStart");
            stringBuilder.AppendLine("		,c.vcLabelEnd");
            stringBuilder.AppendLine("		,c.vcSellNo");
            stringBuilder.AppendLine("		,c.packingcondition");
            stringBuilder.AppendLine("		,c.vcPackingPlant");
            stringBuilder.AppendLine("		,e.decPriceOrigin");
            stringBuilder.AppendLine("		,e.decPriceAfter");
            stringBuilder.AppendLine("		,e.decPriceTNPWithTax");
            stringBuilder.AppendLine("		,h.vcPartENName");
            stringBuilder.AppendLine("		,h.vcPartNameCn");
            stringBuilder.AppendLine("		,f.vcCaseNo as vcFlag_ck--是否已经出荷过");
            stringBuilder.AppendLine("from ");
            stringBuilder.AppendLine("(select * from TShip_Temp where vcDockSell='" + dock + "' and vcFlag='" + strFlag + "')a");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select * from TBoxMaster where dPrintBoxTime is not null)b");
            stringBuilder.AppendLine("on a.vcBoxNo=b.vcCaseNo");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select * from TOperateSJ where vcZYType='S3' and isnull(vcSellNo,'')='')c");
            stringBuilder.AppendLine("on b.vcCaseNo=c.vcCaseNo and b.vcPart_id=c.vcPart_id and b.vcOrderNo=c.vcKBOrderNo and b.vcLianFanNo=c.vcKBLFNo and b.vcSR=c.vcSR");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select * from TOperateSJ_InOutput)d");
            stringBuilder.AppendLine("on b.vcPart_id=d.vcPart_id and b.vcOrderNo=d.vcKBOrderNo and b.vcLianFanNo=d.vcKBLFNo and b.vcSR=d.vcSR");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select * from TPrice where dPricebegin<=GETDATE() and dPriceEnd>=GETDATE())e");
            stringBuilder.AppendLine("on c.vcPart_id=e.vcPart_id and c.vcSupplier_id=e.vcSupplier_id");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select distinct vcCaseNo from TOperateSJ where vcZYType='S4')f");
            stringBuilder.AppendLine("on b.vcCaseNo=f.vcCaseNo");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select vcPartNo,vcCpdcompany,(ISNULL(SUM(CAST(vcPlantQtyDailySum as int)),0)-ISNULL(SUM(CAST(vcResultQtyDailySum as int)),0)) as vcKeResultQty");
            stringBuilder.AppendLine("from SP_M_ORD ");
            stringBuilder.AppendLine("where ISNULL(vcPlantQtyDailySum,0)!=ISNULL(vcResultQtyDailySum,0) and isnull(vcOrderNo,'')<>'' ");
            stringBuilder.AppendLine("group by vcPartNo,vcCpdcompany)g");
            stringBuilder.AppendLine("on c.vcSHF=g.vcCpdcompany and c.vcPart_id=g.vcPartNo");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select * from TSPMaster where dFromTime<=GETDATE() and dToTime>=GETDATE() and vcPackingPlant='" + strPackingPlant + "')h");
            stringBuilder.AppendLine("on c.vcPart_id=h.vcPartId and c.vcSupplier_id=h.vcSupplierId and c.vcSHF=h.vcReceiver");
            stringBuilder.AppendLine("--检验关联重复性");
            stringBuilder.AppendLine("select * from ");
            stringBuilder.AppendLine("(select * from TShip_Temp where vcDockSell='" + dock + "' and vcFlag='" + strFlag + "')a");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select * from TBoxMaster where dPrintBoxTime is not null)b");
            stringBuilder.AppendLine("on a.vcBoxNo=b.vcCaseNo");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("");
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }
        public void setOutPut_Temp(string dock, string fork, string strFlag, string strPackingPlant, string strIP, string strOperater)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DELETE FROM [dbo].[TOperate_OutPut_Temp] WHERE vcDockSell='" + dock + "' ");
            stringBuilder.AppendLine("INSERT INTO [dbo].[TOperate_OutPut_Temp]");
            stringBuilder.AppendLine("           ([vcForkNo]");
            stringBuilder.AppendLine("           ,[vcDockSell]");
            stringBuilder.AppendLine("           ,[vcCaseNo]");
            stringBuilder.AppendLine("           ,[vcBoxNo]");
            stringBuilder.AppendLine("           ,[vcPart_id]");
            stringBuilder.AppendLine("           ,[vcKBOrderNo]");
            stringBuilder.AppendLine("           ,[vcKBLFNo]");
            stringBuilder.AppendLine("           ,[vcSR]");
            stringBuilder.AppendLine("           ,[vcFlag_zx]");
            stringBuilder.AppendLine("           ,[vcBZPlant]");
            stringBuilder.AppendLine("           ,[vcInputNo]");
            stringBuilder.AppendLine("           ,[vcIOType]");
            stringBuilder.AppendLine("           ,[vcSupplier_id]");
            stringBuilder.AppendLine("           ,[vcSupplierGQ]");
            stringBuilder.AppendLine("           ,[iQuantity_rk]");
            stringBuilder.AppendLine("           ,[iQuantity_bcck]");
            stringBuilder.AppendLine("           ,[iQuantity_kck]");
            stringBuilder.AppendLine("           ,[iQuantity_dck]");
            stringBuilder.AppendLine("           ,[vcBZUnit]");
            stringBuilder.AppendLine("           ,[vcSHF]");
            stringBuilder.AppendLine("           ,[vcCheckType]");
            stringBuilder.AppendLine("           ,[iCheckNum]");
            stringBuilder.AppendLine("           ,[vcCheckStatus]");
            stringBuilder.AppendLine("           ,[vcLabelStart]");
            stringBuilder.AppendLine("           ,[vcLabelEnd]");
            stringBuilder.AppendLine("           ,[vcSellNo]");
            stringBuilder.AppendLine("           ,[packingcondition]");
            stringBuilder.AppendLine("           ,[vcPackingPlant]");
            stringBuilder.AppendLine("           ,[decPriceOrigin]");
            stringBuilder.AppendLine("           ,[decPriceAfter]");
            stringBuilder.AppendLine("           ,[decPriceTNPWithTax]");
            stringBuilder.AppendLine("           ,[vcPartENName]");
            stringBuilder.AppendLine("           ,[vcFlag_ck]");
            stringBuilder.AppendLine("           ,[vcHostIp]");
            stringBuilder.AppendLine("           ,[vcOperatorID]");
            stringBuilder.AppendLine("           ,[dOperatorTime])");
            stringBuilder.AppendLine("--本次出荷主数据表");
            stringBuilder.AppendLine("select a.vcForkNo--叉车号");
            stringBuilder.AppendLine("		,a.vcDockSell--DOCK号");
            stringBuilder.AppendLine("		,a.vcBoxNo as vcCaseNo--箱号全位");
            stringBuilder.AppendLine("		,c.vcBoxNo--箱号截位");
            stringBuilder.AppendLine("		,b.vcPart_id as vcPart_id--品番");
            stringBuilder.AppendLine("		,b.vcOrderNo as vcKBOrderNo --");
            stringBuilder.AppendLine("		,b.vcLianFanNo as vcKBLFNo");
            stringBuilder.AppendLine("		,b.vcSR");
            stringBuilder.AppendLine("		,c.vcZYType as vcFlag_zx--已装箱标志");
            stringBuilder.AppendLine("		,c.vcBZPlant");
            stringBuilder.AppendLine("		,c.vcInputNo");
            stringBuilder.AppendLine("		,c.vcIOType");
            stringBuilder.AppendLine("		,c.vcSupplier_id");
            stringBuilder.AppendLine("		,c.vcSupplierGQ");
            stringBuilder.AppendLine("		,CAST(isnull(d.iQuantity, 0) AS INT) as iQuantity_rk--入库量");
            stringBuilder.AppendLine("		,CAST(isnull(c.iQuantity, 0) AS INT) as iQuantity_bcck--本次出库量");
            stringBuilder.AppendLine("		,CAST(isnull(g.vcKeResultQty, 0) AS INT) as iQuantity_kck--可出库量");
            stringBuilder.AppendLine("		,CAST(isnull(d.iDCH, 0) AS INT) as iQuantity_dck--待出库量");
            stringBuilder.AppendLine("		,c.vcBZUnit");
            stringBuilder.AppendLine("		,c.vcSHF");
            stringBuilder.AppendLine("		,c.vcCheckType");
            stringBuilder.AppendLine("		,c.iCheckNum");
            stringBuilder.AppendLine("		,c.vcCheckStatus");
            stringBuilder.AppendLine("		,c.vcLabelStart");
            stringBuilder.AppendLine("		,c.vcLabelEnd");
            stringBuilder.AppendLine("		,c.vcSellNo");
            stringBuilder.AppendLine("		,c.packingcondition");
            stringBuilder.AppendLine("		,c.vcPackingPlant");
            stringBuilder.AppendLine("		,e.decPriceOrigin");
            stringBuilder.AppendLine("		,e.decPriceAfter");
            stringBuilder.AppendLine("		,e.decPriceTNPWithTax");
            stringBuilder.AppendLine("		,h.vcPartENName");
            stringBuilder.AppendLine("		,f.vcCaseNo as vcFlag_ck--是否已经出荷过");
            stringBuilder.AppendLine("		,'" + strIP + "'");
            stringBuilder.AppendLine("		,'" + strOperater + "'");
            stringBuilder.AppendLine("		,getdate()");
            stringBuilder.AppendLine("from ");
            stringBuilder.AppendLine("(select * from TShip_Temp where vcDockSell='" + dock + "' and vcFlag='" + strFlag + "')a");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select * from TBoxMaster where dPrintBoxTime is not null)b");
            stringBuilder.AppendLine("on a.vcBoxNo=b.vcCaseNo");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select * from TOperateSJ where vcZYType='S3' and isnull(vcSellNo,'')='')c");
            stringBuilder.AppendLine("on b.vcCaseNo=c.vcCaseNo and b.vcPart_id=c.vcPart_id and b.vcOrderNo=c.vcKBOrderNo and b.vcLianFanNo=c.vcKBLFNo and b.vcSR=c.vcSR");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select * from TOperateSJ_InOutput)d");
            stringBuilder.AppendLine("on b.vcPart_id=d.vcPart_id and b.vcOrderNo=d.vcKBOrderNo and b.vcLianFanNo=d.vcKBLFNo and b.vcSR=d.vcSR");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select * from TPrice where dPricebegin<=GETDATE() and dPriceEnd>=GETDATE())e");
            stringBuilder.AppendLine("on c.vcPart_id=e.vcPart_id and c.vcSupplier_id=e.vcSupplier_id");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select distinct vcCaseNo from TOperateSJ where vcZYType='S4')f");
            stringBuilder.AppendLine("on b.vcCaseNo=f.vcCaseNo");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select vcPartNo,vcCpdcompany,(ISNULL(SUM(CAST(vcPlantQtyDailySum as int)),0)-ISNULL(SUM(CAST(vcResultQtyDailySum as int)),0)) as vcKeResultQty");
            stringBuilder.AppendLine("from SP_M_ORD ");
            stringBuilder.AppendLine("where ISNULL(vcPlantQtyDailySum,0)!=ISNULL(vcResultQtyDailySum,0) and isnull(vcOrderNo,'')<>'' ");
            stringBuilder.AppendLine("group by vcPartNo,vcCpdcompany)g");
            stringBuilder.AppendLine("on c.vcSHF=g.vcCpdcompany and c.vcPart_id=g.vcPartNo");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select * from TSPMaster where dFromTime<=GETDATE() and dToTime>=GETDATE() and vcPackingPlant='" + strPackingPlant + "')h");
            stringBuilder.AppendLine("on c.vcPart_id=h.vcPartId and c.vcSupplier_id=h.vcSupplierId and c.vcSHF=h.vcReceiver");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }
        public DataSet getTableInfoFromDB()
        {
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("SELECT top(1)* FROM TOperateSJ");//0
            stringBuilder.AppendLine("SELECT top(1)* FROM TOperateSJ_InOutput");//1
            stringBuilder.AppendLine("SELECT top(1)* FROM TShipList");//2
            stringBuilder.AppendLine("SELECT top(1)* FROM TSell");//3
            stringBuilder.AppendLine("SELECT top(1)* FROM TSell_Tool");//4
            stringBuilder.AppendLine("SELECT top(1)* FROM TSell_Sum");//5
            stringBuilder.AppendLine("SELECT top(1)* FROM [dbo].[SP_M_ORD]");//6;
            stringBuilder.AppendLine("--全品番订单信息表");
            stringBuilder.AppendLine("select  vcTargetYearMonth,vcOrderType,vcOrderNo,vcSeqno,vcPartNo,vcDock,vcCpdcompany,iAutoId,(CAST(ISNULL(vcInputQtyDaily1,0) as int)-CAST(ISNULL(vcResultQtyDaily1,0) as int)) as day1 ,");
            stringBuilder.AppendLine("(CAST(ISNULL(vcInputQtyDaily2,0) as int)-CAST(ISNULL(vcResultQtyDaily2,0) as int)) as day2 ,(CAST(ISNULL(vcInputQtyDaily3,0) as int)-CAST(ISNULL(vcResultQtyDaily3,0) as int)) as day3 ,");
            stringBuilder.AppendLine("(CAST(ISNULL(vcInputQtyDaily4,0) as int)-CAST(ISNULL(vcResultQtyDaily4,0) as int)) as day4 ,(CAST(ISNULL(vcInputQtyDaily5,0) as int)-CAST(ISNULL(vcResultQtyDaily5,0) as int)) as day5 ,");
            stringBuilder.AppendLine("(CAST(ISNULL(vcInputQtyDaily6,0) as int)-CAST(ISNULL(vcResultQtyDaily6,0) as int)) as day6 ,(CAST(ISNULL(vcInputQtyDaily7,0) as int)-CAST(ISNULL(vcResultQtyDaily7,0) as int)) as day7 ,");
            stringBuilder.AppendLine("(CAST(ISNULL(vcInputQtyDaily8,0) as int)-CAST(ISNULL(vcResultQtyDaily8,0) as int)) as day8 ,(CAST(ISNULL(vcInputQtyDaily9,0) as int)-CAST(ISNULL(vcResultQtyDaily9,0) as int)) as day9 ,");
            stringBuilder.AppendLine("(CAST(ISNULL(vcInputQtyDaily10,0) as int)-CAST(ISNULL(vcResultQtyDaily10,0) as int)) as day10 ,(CAST(ISNULL(vcInputQtyDaily11,0) as int)-CAST(ISNULL(vcResultQtyDaily11,0) as int)) as day11 ,");
            stringBuilder.AppendLine("(CAST(ISNULL(vcInputQtyDaily12,0) as int)-CAST(ISNULL(vcResultQtyDaily12,0) as int)) as day12 ,(CAST(ISNULL(vcInputQtyDaily13,0) as int)-CAST(ISNULL(vcResultQtyDaily13,0) as int)) as day13 ,");
            stringBuilder.AppendLine("(CAST(ISNULL(vcInputQtyDaily14,0) as int)-CAST(ISNULL(vcResultQtyDaily14,0) as int)) as day14 ,(CAST(ISNULL(vcInputQtyDaily15,0) as int)-CAST(ISNULL(vcResultQtyDaily15,0) as int)) as day15 ,");
            stringBuilder.AppendLine("(CAST(ISNULL(vcInputQtyDaily16,0) as int)-CAST(ISNULL(vcResultQtyDaily16,0) as int)) as day16 ,(CAST(ISNULL(vcInputQtyDaily17,0) as int)-CAST(ISNULL(vcResultQtyDaily17,0) as int)) as day17 ,");
            stringBuilder.AppendLine("(CAST(ISNULL(vcInputQtyDaily18,0) as int)-CAST(ISNULL(vcResultQtyDaily18,0) as int)) as day18 ,(CAST(ISNULL(vcInputQtyDaily19,0) as int)-CAST(ISNULL(vcResultQtyDaily19,0) as int)) as day19 ,");
            stringBuilder.AppendLine("(CAST(ISNULL(vcInputQtyDaily20,0) as int)-CAST(ISNULL(vcResultQtyDaily20,0) as int)) as day20 ,(CAST(ISNULL(vcInputQtyDaily21,0) as int)-CAST(ISNULL(vcResultQtyDaily21,0) as int)) as day21 ,");
            stringBuilder.AppendLine("(CAST(ISNULL(vcInputQtyDaily22,0) as int)-CAST(ISNULL(vcResultQtyDaily22,0) as int)) as day22 ,(CAST(ISNULL(vcInputQtyDaily23,0) as int)-CAST(ISNULL(vcResultQtyDaily23,0) as int)) as day23 ,");
            stringBuilder.AppendLine("(CAST(ISNULL(vcInputQtyDaily24,0) as int)-CAST(ISNULL(vcResultQtyDaily24,0) as int)) as day24 ,(CAST(ISNULL(vcInputQtyDaily25,0) as int)-CAST(ISNULL(vcResultQtyDaily25,0) as int)) as day25 ,");
            stringBuilder.AppendLine("(CAST(ISNULL(vcInputQtyDaily26,0) as int)-CAST(ISNULL(vcResultQtyDaily26,0) as int)) as day26 ,(CAST(ISNULL(vcInputQtyDaily27,0) as int)-CAST(ISNULL(vcResultQtyDaily27,0) as int)) as day27 ,");
            stringBuilder.AppendLine("(CAST(ISNULL(vcInputQtyDaily28,0) as int)-CAST(ISNULL(vcResultQtyDaily28,0) as int)) as day28 ,(CAST(ISNULL(vcInputQtyDaily29,0) as int)-CAST(ISNULL(vcResultQtyDaily29,0) as int)) as day29 ,");
            stringBuilder.AppendLine("(CAST(ISNULL(vcInputQtyDaily30,0) as int)-CAST(ISNULL(vcResultQtyDaily30,0) as int)) as day30 ,(CAST(ISNULL(vcInputQtyDaily31,0) as int)-CAST(ISNULL(vcResultQtyDaily31,0) as int)) as day31 ");
            stringBuilder.AppendLine("from  SP_M_ORD where vcResultQtyDailySum<vcInputQtyDailySum order by vcPartNo,vcTargetYearMonth");
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }
        public DataTable getBanZhiTime(string strPackPlant, string strFlag)
        {
            try
            {
                SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();
                SqlParameter[] pars = new SqlParameter[]{
                    new SqlParameter("@flag", strFlag),
                    new SqlParameter("@PackPlant",strPackPlant)
                };
                string cmdText = "BSP0811_getBanZhiTime";
                SqlDataAdapter sa = new SqlDataAdapter(cmdText, sqlConnection);
                if (pars != null && pars.Length > 0)
                {
                    foreach (SqlParameter p in pars)
                    {
                        sa.SelectCommand.Parameters.Add(p);
                    }
                }
                sa.SelectCommand.CommandTimeout = 0;
                sa.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                sa.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable setSeqNo(string tmpString, int iAddNum, string formatServerTime, string strHosDate, string strBanZhi, string strType)
        {

            StringBuilder stringBuilder = new StringBuilder();
            if (strType == "销售单号")
            {
                stringBuilder.AppendLine("declare @flag int");
                stringBuilder.AppendLine("set @flag=(select count(*) from TSeqNo where FLAG='" + tmpString + "' and DDATE='" + formatServerTime + "')");
                stringBuilder.AppendLine("if(@flag=0)");
                stringBuilder.AppendLine("begin");
                stringBuilder.AppendLine("INSERT INTO TSeqNo(FLAG ,DDATE ,SEQNO)VALUES( '" + tmpString + "','" + formatServerTime + "'," + iAddNum + ")");
                stringBuilder.AppendLine("select 1 as vcSeqNo");
                stringBuilder.AppendLine("end");
                stringBuilder.AppendLine("else");
                stringBuilder.AppendLine("begin");
                stringBuilder.AppendLine("update TSeqNo set SEQNO=SEQNO+" + iAddNum + "  where FLAG='" + tmpString + "' and DDATE='" + formatServerTime + "'");
                stringBuilder.AppendLine("select SEQNO as vcSeqNo  from TSeqNo   where FLAG='" + tmpString + "' and DDATE='" + formatServerTime + "'");
                stringBuilder.AppendLine("end");
            }
            if (strType == "便次号")
            {
                stringBuilder.AppendLine("declare @flag int");
                stringBuilder.AppendLine("set @flag=(select count(*) from TSeqNo where FLAG='" + tmpString + "' and DDATE='" + strHosDate + "' and FIELD1='" + strBanZhi + "')");
                stringBuilder.AppendLine("if(@flag=0)");
                stringBuilder.AppendLine("begin");
                stringBuilder.AppendLine("INSERT INTO TSeqNo(FLAG ,DDATE ,SEQNO,FIELD1)VALUES( '" + tmpString + "','" + strHosDate + "'," + iAddNum + ",'" + strBanZhi + "')");
                stringBuilder.AppendLine("select 1 as vcSeqNo");
                stringBuilder.AppendLine("end");
                stringBuilder.AppendLine("else");
                stringBuilder.AppendLine("begin");
                stringBuilder.AppendLine("update TSeqNo set SEQNO=SEQNO+" + iAddNum + "  where FLAG='" + tmpString + "' and DDATE='" + strHosDate + "' and FIELD1='" + strBanZhi + "'");
                stringBuilder.AppendLine("select SEQNO as vcSeqNo  from TSeqNo   where FLAG='" + tmpString + "' and DDATE='" + strHosDate + "' and FIELD1='" + strBanZhi + "'");
                stringBuilder.AppendLine("end");
            }
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();

            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }

        }
        public bool setCastListInfo(DataTable dtOperateSJ_Temp, 
            DataTable dtOperateSJ_InOutput_Temp, 
            DataTable dtOrder_Temp,
            DataTable dtSell_Temp,
            DataTable dtShipList_Temp,
            DataTable dtSell_Sum_Temp,
            DataTable dtSell_Tool_Temp,
            string strIP, string strSellno,string strDock, string strOperId)
        {
            SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();
            sqlConnection.Open();
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            try
            {
                #region 1.OperateSJ  sqlCommand_add_sj
                SqlCommand sqlCommand_add_sj = sqlConnection.CreateCommand();
                sqlCommand_add_sj.Transaction = sqlTransaction;
                sqlCommand_add_sj.CommandType = CommandType.Text;
                StringBuilder strSql_add_sj = new StringBuilder();

                #region SQL and Parameters
                strSql_add_sj.AppendLine("INSERT INTO [dbo].[TOperateSJ]");
                strSql_add_sj.AppendLine("           ([vcZYType]");
                strSql_add_sj.AppendLine("           ,[vcBZPlant]");
                strSql_add_sj.AppendLine("           ,[vcInputNo]");
                strSql_add_sj.AppendLine("           ,[vcKBOrderNo]");
                strSql_add_sj.AppendLine("           ,[vcKBLFNo]");
                strSql_add_sj.AppendLine("           ,[vcPart_id]");
                strSql_add_sj.AppendLine("           ,[vcIOType]");
                strSql_add_sj.AppendLine("           ,[vcSupplier_id]");
                strSql_add_sj.AppendLine("           ,[vcSupplierGQ]");
                strSql_add_sj.AppendLine("           ,[dStart]");
                strSql_add_sj.AppendLine("           ,[dEnd]");
                strSql_add_sj.AppendLine("           ,[iQuantity]");
                strSql_add_sj.AppendLine("           ,[vcBZUnit]");
                strSql_add_sj.AppendLine("           ,[vcSHF]");
                strSql_add_sj.AppendLine("           ,[vcSR]");
                strSql_add_sj.AppendLine("           ,[vcCaseNo]");
                strSql_add_sj.AppendLine("           ,[vcBoxNo]");
                strSql_add_sj.AppendLine("           ,[vcSheBeiNo]");
                strSql_add_sj.AppendLine("           ,[vcCheckType]");
                strSql_add_sj.AppendLine("           ,[iCheckNum]");
                strSql_add_sj.AppendLine("           ,[vcCheckStatus]");
                strSql_add_sj.AppendLine("           ,[vcLabelStart]");
                strSql_add_sj.AppendLine("           ,[vcLabelEnd]");
                strSql_add_sj.AppendLine("           ,[vcUnlocker]");
                strSql_add_sj.AppendLine("           ,[dUnlockTime]");
                strSql_add_sj.AppendLine("           ,[vcSellNo]");
                strSql_add_sj.AppendLine("           ,[vcOperatorID]");
                strSql_add_sj.AppendLine("           ,[dOperatorTime]");
                strSql_add_sj.AppendLine("           ,[vcHostIp]");
                strSql_add_sj.AppendLine("           ,[packingcondition]");
                strSql_add_sj.AppendLine("           ,[vcPackingPlant])");
                strSql_add_sj.AppendLine("     VALUES");
                strSql_add_sj.AppendLine("           (@vcZYType");
                strSql_add_sj.AppendLine("           ,@vcBZPlant");
                strSql_add_sj.AppendLine("           ,@vcInputNo");
                strSql_add_sj.AppendLine("           ,@vcKBOrderNo");
                strSql_add_sj.AppendLine("           ,@vcKBLFNo");
                strSql_add_sj.AppendLine("           ,@vcPart_id");
                strSql_add_sj.AppendLine("           ,@vcIOType");
                strSql_add_sj.AppendLine("           ,@vcSupplier_id");
                strSql_add_sj.AppendLine("           ,@vcSupplierGQ");
                strSql_add_sj.AppendLine("           ,@dStart");
                strSql_add_sj.AppendLine("           ,GETDATE()");
                strSql_add_sj.AppendLine("           ,@iQuantity");
                strSql_add_sj.AppendLine("           ,@vcBZUnit");
                strSql_add_sj.AppendLine("           ,@vcSHF");
                strSql_add_sj.AppendLine("           ,@vcSR");
                strSql_add_sj.AppendLine("           ,@vcCaseNo");
                strSql_add_sj.AppendLine("           ,@vcBoxNo");
                strSql_add_sj.AppendLine("           ,@vcSheBeiNo");
                strSql_add_sj.AppendLine("           ,@vcCheckType");
                strSql_add_sj.AppendLine("           ,@iCheckNum");
                strSql_add_sj.AppendLine("           ,@vcCheckStatus");
                strSql_add_sj.AppendLine("           ,@vcLabelStart");
                strSql_add_sj.AppendLine("           ,@vcLabelEnd");
                strSql_add_sj.AppendLine("           ,null");
                strSql_add_sj.AppendLine("           ,null");
                strSql_add_sj.AppendLine("           ,@vcSellNo");
                strSql_add_sj.AppendLine("           ,@vcOperatorID");
                strSql_add_sj.AppendLine("           ,GETDATE()");
                strSql_add_sj.AppendLine("           ,@vcHostIp");
                strSql_add_sj.AppendLine("           ,@packingcondition");
                strSql_add_sj.AppendLine("           ,@vcPackingPlant)");

                sqlCommand_add_sj.CommandText = strSql_add_sj.ToString();
                sqlCommand_add_sj.Parameters.AddWithValue("@vcZYType", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcBZPlant", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcInputNo", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcKBOrderNo", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcKBLFNo", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcPart_id", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcIOType", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcSupplier_id", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcSupplierGQ", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@dStart", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@iQuantity", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcBZUnit", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcSHF", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcSR", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcCaseNo", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcBoxNo", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcSheBeiNo", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcCheckType", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@iCheckNum", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcCheckStatus", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcLabelStart", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcLabelEnd", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcSellNo", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcOperatorID", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcHostIp", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@packingcondition", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcPackingPlant", "");
                #endregion
                foreach (DataRow item in dtOperateSJ_Temp.Rows)
                {
                    #region Value
                    sqlCommand_add_sj.Parameters["@vcZYType"].Value = item["vcZYType"].ToString();
                    sqlCommand_add_sj.Parameters["@vcBZPlant"].Value = item["vcBZPlant"].ToString();
                    sqlCommand_add_sj.Parameters["@vcInputNo"].Value = item["vcInputNo"].ToString();
                    sqlCommand_add_sj.Parameters["@vcKBOrderNo"].Value = item["vcKBOrderNo"].ToString();
                    sqlCommand_add_sj.Parameters["@vcKBLFNo"].Value = item["vcKBLFNo"].ToString();
                    sqlCommand_add_sj.Parameters["@vcPart_id"].Value = item["vcPart_id"].ToString();
                    sqlCommand_add_sj.Parameters["@vcIOType"].Value = item["vcIOType"].ToString();
                    sqlCommand_add_sj.Parameters["@vcSupplier_id"].Value = item["vcSupplier_id"].ToString();
                    sqlCommand_add_sj.Parameters["@vcSupplierGQ"].Value = item["vcSupplierGQ"].ToString();
                    sqlCommand_add_sj.Parameters["@dStart"].Value = item["dStart"].ToString();
                    sqlCommand_add_sj.Parameters["@iQuantity"].Value = item["iQuantity"].ToString();
                    sqlCommand_add_sj.Parameters["@vcBZUnit"].Value = item["vcBZUnit"].ToString();
                    sqlCommand_add_sj.Parameters["@vcSHF"].Value = item["vcSHF"].ToString();
                    sqlCommand_add_sj.Parameters["@vcSR"].Value = item["vcSR"].ToString();
                    sqlCommand_add_sj.Parameters["@vcCaseNo"].Value = item["vcCaseNo"].ToString();
                    sqlCommand_add_sj.Parameters["@vcBoxNo"].Value = item["vcBoxNo"].ToString();
                    sqlCommand_add_sj.Parameters["@vcSheBeiNo"].Value = item["vcSheBeiNo"].ToString();
                    sqlCommand_add_sj.Parameters["@vcCheckType"].Value = item["vcCheckType"].ToString();
                    sqlCommand_add_sj.Parameters["@iCheckNum"].Value = item["iCheckNum"].ToString();
                    sqlCommand_add_sj.Parameters["@vcCheckStatus"].Value = item["vcCheckStatus"].ToString();
                    sqlCommand_add_sj.Parameters["@vcLabelStart"].Value = item["vcLabelStart"].ToString();
                    sqlCommand_add_sj.Parameters["@vcLabelEnd"].Value = item["vcLabelEnd"].ToString();
                    sqlCommand_add_sj.Parameters["@vcSellNo"].Value = item["vcSellNo"].ToString();
                    sqlCommand_add_sj.Parameters["@vcOperatorID"].Value = item["vcOperatorID"].ToString();
                    sqlCommand_add_sj.Parameters["@vcHostIp"].Value = item["vcHostIp"].ToString();
                    sqlCommand_add_sj.Parameters["@packingcondition"].Value = item["packingcondition"].ToString();
                    sqlCommand_add_sj.Parameters["@vcPackingPlant"].Value = item["vcPackingPlant"].ToString();
                    #endregion
                    sqlCommand_add_sj.ExecuteNonQuery();
                }
                #endregion

                #region 2.OperateSJ_InOutput    sqlCommand_mod_io
                SqlCommand sqlCommand_mod_io = sqlConnection.CreateCommand();
                sqlCommand_mod_io.Transaction = sqlTransaction;
                sqlCommand_mod_io.CommandType = CommandType.Text;
                StringBuilder strSql_mod_io = new StringBuilder();

                #region SQL and Parameters
                strSql_mod_io.AppendLine("UPDATE [dbo].[TOperateSJ_InOutput]");
                strSql_mod_io.AppendLine("   SET [iDCH] = [iDCH]-CAST(@iDCH AS INT)");
                strSql_mod_io.AppendLine("      ,[vcOperatorID] = @vcOperatorID");
                strSql_mod_io.AppendLine("      ,[dOperatorTime] = GETDATE()");
                strSql_mod_io.AppendLine(" WHERE [vcKBOrderNo] = @vcKBOrderNo");
                strSql_mod_io.AppendLine("      AND [vcKBLFNo] = @vcKBLFNo");
                strSql_mod_io.AppendLine("      AND [vcPart_id] = @vcPart_id");
                strSql_mod_io.AppendLine("      AND [vcSR] = @vcSR");
                sqlCommand_mod_io.CommandText = strSql_mod_io.ToString();
                sqlCommand_mod_io.Parameters.AddWithValue("@vcKBOrderNo", "");
                sqlCommand_mod_io.Parameters.AddWithValue("@vcKBLFNo", "");
                sqlCommand_mod_io.Parameters.AddWithValue("@vcPart_id", "");
                sqlCommand_mod_io.Parameters.AddWithValue("@vcSR", "");
                sqlCommand_mod_io.Parameters.AddWithValue("@iDCH", "");
                sqlCommand_mod_io.Parameters.AddWithValue("@vcOperatorID", "");
                #endregion
                foreach (DataRow item in dtOperateSJ_InOutput_Temp.Rows)
                {
                    #region Value
                    sqlCommand_mod_io.Parameters["@vcKBOrderNo"].Value = item["vcKBOrderNo"].ToString();
                    sqlCommand_mod_io.Parameters["@vcKBLFNo"].Value = item["vcKBLFNo"].ToString();
                    sqlCommand_mod_io.Parameters["@vcPart_id"].Value = item["vcPart_id"].ToString();
                    sqlCommand_mod_io.Parameters["@vcSR"].Value = item["vcSR"].ToString();
                    sqlCommand_mod_io.Parameters["@iDCH"].Value = item["iDCH"].ToString();
                    sqlCommand_mod_io.Parameters["@vcOperatorID"].Value = item["vcOperatorID"].ToString();
                    #endregion
                    sqlCommand_mod_io.ExecuteNonQuery();
                }
                #endregion

                #region 3.SP_M_ORD  sqlCommand_modinfo_od
                SqlCommand sqlCommand_modinfo_od = sqlConnection.CreateCommand();
                sqlCommand_modinfo_od.Transaction = sqlTransaction;
                sqlCommand_modinfo_od.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo_od = new StringBuilder();
                #region SQL and Parameters
                strSql_modinfo_od.AppendLine("update SP_M_ORD set ");
                strSql_modinfo_od.AppendLine("vcResultQtyDaily1=isnull(cast(vcResultQtyDaily1 as int),0)+cast(@vcResultQtyDaily1 as int),");
                strSql_modinfo_od.AppendLine("vcResultQtyDaily2=isnull(cast(vcResultQtyDaily2 as int),0)+cast(@vcResultQtyDaily2 as int),");
                strSql_modinfo_od.AppendLine("vcResultQtyDaily3=isnull(cast(vcResultQtyDaily3 as int),0)+cast(@vcResultQtyDaily3 as int),");
                strSql_modinfo_od.AppendLine("vcResultQtyDaily4=isnull(cast(vcResultQtyDaily4 as int),0)+cast(@vcResultQtyDaily4 as int),");
                strSql_modinfo_od.AppendLine("vcResultQtyDaily5=isnull(cast(vcResultQtyDaily5 as int),0)+cast(@vcResultQtyDaily5 as int),");
                strSql_modinfo_od.AppendLine("vcResultQtyDaily6=isnull(cast(vcResultQtyDaily6 as int),0)+cast(@vcResultQtyDaily6 as int),");
                strSql_modinfo_od.AppendLine("vcResultQtyDaily7=isnull(cast(vcResultQtyDaily7 as int),0)+cast(@vcResultQtyDaily7 as int),");
                strSql_modinfo_od.AppendLine("vcResultQtyDaily8=isnull(cast(vcResultQtyDaily8 as int),0)+cast(@vcResultQtyDaily8 as int),");
                strSql_modinfo_od.AppendLine("vcResultQtyDaily9=isnull(cast(vcResultQtyDaily9 as int),0)+cast(@vcResultQtyDaily9 as int),");
                strSql_modinfo_od.AppendLine("vcResultQtyDaily10=isnull(cast(vcResultQtyDaily10 as int),0)+cast(@vcResultQtyDaily10 as int),");
                strSql_modinfo_od.AppendLine("vcResultQtyDaily11=isnull(cast(vcResultQtyDaily11 as int),0)+cast(@vcResultQtyDaily11 as int),");
                strSql_modinfo_od.AppendLine("vcResultQtyDaily12=isnull(cast(vcResultQtyDaily12 as int),0)+cast(@vcResultQtyDaily12 as int),");
                strSql_modinfo_od.AppendLine("vcResultQtyDaily13=isnull(cast(vcResultQtyDaily13 as int),0)+cast(@vcResultQtyDaily13 as int),");
                strSql_modinfo_od.AppendLine("vcResultQtyDaily14=isnull(cast(vcResultQtyDaily14 as int),0)+cast(@vcResultQtyDaily14 as int),");
                strSql_modinfo_od.AppendLine("vcResultQtyDaily15=isnull(cast(vcResultQtyDaily15 as int),0)+cast(@vcResultQtyDaily15 as int),");
                strSql_modinfo_od.AppendLine("vcResultQtyDaily16=isnull(cast(vcResultQtyDaily16 as int),0)+cast(@vcResultQtyDaily16 as int),");
                strSql_modinfo_od.AppendLine("vcResultQtyDaily17=isnull(cast(vcResultQtyDaily17 as int),0)+cast(@vcResultQtyDaily17 as int),");
                strSql_modinfo_od.AppendLine("vcResultQtyDaily18=isnull(cast(vcResultQtyDaily18 as int),0)+cast(@vcResultQtyDaily18 as int),");
                strSql_modinfo_od.AppendLine("vcResultQtyDaily19=isnull(cast(vcResultQtyDaily19 as int),0)+cast(@vcResultQtyDaily19 as int),");
                strSql_modinfo_od.AppendLine("vcResultQtyDaily20=isnull(cast(vcResultQtyDaily20 as int),0)+cast(@vcResultQtyDaily20 as int),");
                strSql_modinfo_od.AppendLine("vcResultQtyDaily21=isnull(cast(vcResultQtyDaily21 as int),0)+cast(@vcResultQtyDaily21 as int),");
                strSql_modinfo_od.AppendLine("vcResultQtyDaily22=isnull(cast(vcResultQtyDaily22 as int),0)+cast(@vcResultQtyDaily22 as int),");
                strSql_modinfo_od.AppendLine("vcResultQtyDaily23=isnull(cast(vcResultQtyDaily23 as int),0)+cast(@vcResultQtyDaily23 as int),");
                strSql_modinfo_od.AppendLine("vcResultQtyDaily24=isnull(cast(vcResultQtyDaily24 as int),0)+cast(@vcResultQtyDaily24 as int),");
                strSql_modinfo_od.AppendLine("vcResultQtyDaily25=isnull(cast(vcResultQtyDaily25 as int),0)+cast(@vcResultQtyDaily25 as int),");
                strSql_modinfo_od.AppendLine("vcResultQtyDaily26=isnull(cast(vcResultQtyDaily26 as int),0)+cast(@vcResultQtyDaily26 as int),");
                strSql_modinfo_od.AppendLine("vcResultQtyDaily27=isnull(cast(vcResultQtyDaily27 as int),0)+cast(@vcResultQtyDaily27 as int),");
                strSql_modinfo_od.AppendLine("vcResultQtyDaily28=isnull(cast(vcResultQtyDaily28 as int),0)+cast(@vcResultQtyDaily28 as int),");
                strSql_modinfo_od.AppendLine("vcResultQtyDaily29=isnull(cast(vcResultQtyDaily29 as int),0)+cast(@vcResultQtyDaily29 as int),");
                strSql_modinfo_od.AppendLine("vcResultQtyDaily30=isnull(cast(vcResultQtyDaily30 as int),0)+cast(@vcResultQtyDaily30 as int),");
                strSql_modinfo_od.AppendLine("vcResultQtyDaily31=isnull(cast(vcResultQtyDaily31 as int),0)+cast(@vcResultQtyDaily31 as int),");
                strSql_modinfo_od.AppendLine("vcResultQtyDailySum=isnull(cast(vcResultQtyDailySum as int),0)+cast(@vcResultQtyDailySum as int) where iAutoId=@iAutoId");
                sqlCommand_modinfo_od.CommandText = strSql_modinfo_od.ToString();
                sqlCommand_modinfo_od.Parameters.AddWithValue("@iAutoId", "");
                sqlCommand_modinfo_od.Parameters.AddWithValue("@vcResultQtyDailySum", "");
                sqlCommand_modinfo_od.Parameters.AddWithValue("@vcResultQtyDaily1", "");
                sqlCommand_modinfo_od.Parameters.AddWithValue("@vcResultQtyDaily2", "");
                sqlCommand_modinfo_od.Parameters.AddWithValue("@vcResultQtyDaily3", "");
                sqlCommand_modinfo_od.Parameters.AddWithValue("@vcResultQtyDaily4", "");
                sqlCommand_modinfo_od.Parameters.AddWithValue("@vcResultQtyDaily5", "");
                sqlCommand_modinfo_od.Parameters.AddWithValue("@vcResultQtyDaily6", "");
                sqlCommand_modinfo_od.Parameters.AddWithValue("@vcResultQtyDaily7", "");
                sqlCommand_modinfo_od.Parameters.AddWithValue("@vcResultQtyDaily8", "");
                sqlCommand_modinfo_od.Parameters.AddWithValue("@vcResultQtyDaily9", "");
                sqlCommand_modinfo_od.Parameters.AddWithValue("@vcResultQtyDaily10", "");
                sqlCommand_modinfo_od.Parameters.AddWithValue("@vcResultQtyDaily11", "");
                sqlCommand_modinfo_od.Parameters.AddWithValue("@vcResultQtyDaily12", "");
                sqlCommand_modinfo_od.Parameters.AddWithValue("@vcResultQtyDaily13", "");
                sqlCommand_modinfo_od.Parameters.AddWithValue("@vcResultQtyDaily14", "");
                sqlCommand_modinfo_od.Parameters.AddWithValue("@vcResultQtyDaily15", "");
                sqlCommand_modinfo_od.Parameters.AddWithValue("@vcResultQtyDaily16", "");
                sqlCommand_modinfo_od.Parameters.AddWithValue("@vcResultQtyDaily17", "");
                sqlCommand_modinfo_od.Parameters.AddWithValue("@vcResultQtyDaily18", "");
                sqlCommand_modinfo_od.Parameters.AddWithValue("@vcResultQtyDaily19", "");
                sqlCommand_modinfo_od.Parameters.AddWithValue("@vcResultQtyDaily20", "");
                sqlCommand_modinfo_od.Parameters.AddWithValue("@vcResultQtyDaily21", "");
                sqlCommand_modinfo_od.Parameters.AddWithValue("@vcResultQtyDaily22", "");
                sqlCommand_modinfo_od.Parameters.AddWithValue("@vcResultQtyDaily23", "");
                sqlCommand_modinfo_od.Parameters.AddWithValue("@vcResultQtyDaily24", "");
                sqlCommand_modinfo_od.Parameters.AddWithValue("@vcResultQtyDaily25", "");
                sqlCommand_modinfo_od.Parameters.AddWithValue("@vcResultQtyDaily26", "");
                sqlCommand_modinfo_od.Parameters.AddWithValue("@vcResultQtyDaily27", "");
                sqlCommand_modinfo_od.Parameters.AddWithValue("@vcResultQtyDaily28", "");
                sqlCommand_modinfo_od.Parameters.AddWithValue("@vcResultQtyDaily29", "");
                sqlCommand_modinfo_od.Parameters.AddWithValue("@vcResultQtyDaily30", "");
                sqlCommand_modinfo_od.Parameters.AddWithValue("@vcResultQtyDaily31", "");
                #endregion
                foreach (DataRow item in dtOrder_Temp.Rows)
                {
                    #region Value
                    sqlCommand_modinfo_od.Parameters["@iAutoId"].Value = item["iAutoId"].ToString();
                    sqlCommand_modinfo_od.Parameters["@vcResultQtyDailySum"].Value = item["vcResultQtyDailySum"].ToString();
                    sqlCommand_modinfo_od.Parameters["@vcResultQtyDaily1"].Value = item["vcResultQtyDaily1"].ToString();
                    sqlCommand_modinfo_od.Parameters["@vcResultQtyDaily2"].Value = item["vcResultQtyDaily2"].ToString();
                    sqlCommand_modinfo_od.Parameters["@vcResultQtyDaily3"].Value = item["vcResultQtyDaily3"].ToString();
                    sqlCommand_modinfo_od.Parameters["@vcResultQtyDaily4"].Value = item["vcResultQtyDaily4"].ToString();
                    sqlCommand_modinfo_od.Parameters["@vcResultQtyDaily5"].Value = item["vcResultQtyDaily5"].ToString();
                    sqlCommand_modinfo_od.Parameters["@vcResultQtyDaily6"].Value = item["vcResultQtyDaily6"].ToString();
                    sqlCommand_modinfo_od.Parameters["@vcResultQtyDaily7"].Value = item["vcResultQtyDaily7"].ToString();
                    sqlCommand_modinfo_od.Parameters["@vcResultQtyDaily8"].Value = item["vcResultQtyDaily8"].ToString();
                    sqlCommand_modinfo_od.Parameters["@vcResultQtyDaily9"].Value = item["vcResultQtyDaily9"].ToString();
                    sqlCommand_modinfo_od.Parameters["@vcResultQtyDaily10"].Value = item["vcResultQtyDaily10"].ToString();
                    sqlCommand_modinfo_od.Parameters["@vcResultQtyDaily11"].Value = item["vcResultQtyDaily11"].ToString();
                    sqlCommand_modinfo_od.Parameters["@vcResultQtyDaily12"].Value = item["vcResultQtyDaily12"].ToString();
                    sqlCommand_modinfo_od.Parameters["@vcResultQtyDaily13"].Value = item["vcResultQtyDaily13"].ToString();
                    sqlCommand_modinfo_od.Parameters["@vcResultQtyDaily14"].Value = item["vcResultQtyDaily14"].ToString();
                    sqlCommand_modinfo_od.Parameters["@vcResultQtyDaily15"].Value = item["vcResultQtyDaily15"].ToString();
                    sqlCommand_modinfo_od.Parameters["@vcResultQtyDaily16"].Value = item["vcResultQtyDaily16"].ToString();
                    sqlCommand_modinfo_od.Parameters["@vcResultQtyDaily17"].Value = item["vcResultQtyDaily17"].ToString();
                    sqlCommand_modinfo_od.Parameters["@vcResultQtyDaily18"].Value = item["vcResultQtyDaily18"].ToString();
                    sqlCommand_modinfo_od.Parameters["@vcResultQtyDaily19"].Value = item["vcResultQtyDaily19"].ToString();
                    sqlCommand_modinfo_od.Parameters["@vcResultQtyDaily20"].Value = item["vcResultQtyDaily20"].ToString();
                    sqlCommand_modinfo_od.Parameters["@vcResultQtyDaily21"].Value = item["vcResultQtyDaily21"].ToString();
                    sqlCommand_modinfo_od.Parameters["@vcResultQtyDaily22"].Value = item["vcResultQtyDaily22"].ToString();
                    sqlCommand_modinfo_od.Parameters["@vcResultQtyDaily23"].Value = item["vcResultQtyDaily23"].ToString();
                    sqlCommand_modinfo_od.Parameters["@vcResultQtyDaily24"].Value = item["vcResultQtyDaily24"].ToString();
                    sqlCommand_modinfo_od.Parameters["@vcResultQtyDaily25"].Value = item["vcResultQtyDaily25"].ToString();
                    sqlCommand_modinfo_od.Parameters["@vcResultQtyDaily26"].Value = item["vcResultQtyDaily26"].ToString();
                    sqlCommand_modinfo_od.Parameters["@vcResultQtyDaily27"].Value = item["vcResultQtyDaily27"].ToString();
                    sqlCommand_modinfo_od.Parameters["@vcResultQtyDaily28"].Value = item["vcResultQtyDaily28"].ToString();
                    sqlCommand_modinfo_od.Parameters["@vcResultQtyDaily29"].Value = item["vcResultQtyDaily29"].ToString();
                    sqlCommand_modinfo_od.Parameters["@vcResultQtyDaily30"].Value = item["vcResultQtyDaily30"].ToString();
                    sqlCommand_modinfo_od.Parameters["@vcResultQtyDaily31"].Value = item["vcResultQtyDaily31"].ToString();
                    #endregion
                    sqlCommand_modinfo_od.ExecuteNonQuery();
                }
                #endregion

                #region 4.Sell  sqlCommand_add_sl
                SqlCommand sqlCommand_add_sl = sqlConnection.CreateCommand();
                sqlCommand_add_sl.Transaction = sqlTransaction;
                sqlCommand_add_sl.CommandType = CommandType.Text;
                StringBuilder strSql_add_sl = new StringBuilder();

                #region SQL and Parameters
                strSql_add_sl.AppendLine("INSERT INTO [dbo].[TSell]");
                strSql_add_sl.AppendLine("           ([dHosDate]");
                strSql_add_sl.AppendLine("           ,[vcBanZhi]");
                strSql_add_sl.AppendLine("           ,[vcBianCi]");
                strSql_add_sl.AppendLine("           ,[vcSellNo]");
                strSql_add_sl.AppendLine("           ,[vcTruckNo]");
                strSql_add_sl.AppendLine("           ,[vcSHF]");
                strSql_add_sl.AppendLine("           ,[vcPart_id]");
                strSql_add_sl.AppendLine("           ,[vcOrderNo]");
                strSql_add_sl.AppendLine("           ,[vcLianFanNo]");
                strSql_add_sl.AppendLine("           ,[vcInvoiceNo]");
                strSql_add_sl.AppendLine("           ,[vcCaseNo]");
                strSql_add_sl.AppendLine("           ,[vcBoxNo]");
                strSql_add_sl.AppendLine("           ,[vcPartsNameEN]");
                strSql_add_sl.AppendLine("           ,[iQuantity]");
                strSql_add_sl.AppendLine("           ,[decPriceWithTax]");
                strSql_add_sl.AppendLine("           ,[decMoney]");
                strSql_add_sl.AppendLine("           ,[vcOperatorID]");
                strSql_add_sl.AppendLine("           ,[dOperatorTime]");
                strSql_add_sl.AppendLine("           ,[vcYinQuType]");
                strSql_add_sl.AppendLine("           ,[vcSender]");
                strSql_add_sl.AppendLine("           ,[vcLabelStart]");
                strSql_add_sl.AppendLine("           ,[vcLabelEnd]");
                strSql_add_sl.AppendLine("           ,[vcSupplier_id])");
                strSql_add_sl.AppendLine("     VALUES");
                strSql_add_sl.AppendLine("           (@dHosDate");
                strSql_add_sl.AppendLine("           ,@vcBanZhi");
                strSql_add_sl.AppendLine("           ,@vcBianCi");
                strSql_add_sl.AppendLine("           ,@vcSellNo");
                strSql_add_sl.AppendLine("           ,@vcTruckNo");
                strSql_add_sl.AppendLine("           ,@vcSHF");
                strSql_add_sl.AppendLine("           ,@vcPart_id");
                strSql_add_sl.AppendLine("           ,@vcOrderNo");
                strSql_add_sl.AppendLine("           ,@vcLianFanNo");
                strSql_add_sl.AppendLine("           ,@vcInvoiceNo");
                strSql_add_sl.AppendLine("           ,@vcCaseNo");
                strSql_add_sl.AppendLine("           ,@vcBoxNo");
                strSql_add_sl.AppendLine("           ,@vcPartsNameEN");
                strSql_add_sl.AppendLine("           ,@iQuantity");
                strSql_add_sl.AppendLine("           ,@decPriceWithTax");
                strSql_add_sl.AppendLine("           ,@decMoney");
                strSql_add_sl.AppendLine("           ,@vcOperatorID");
                strSql_add_sl.AppendLine("           ,GETDATE()");
                strSql_add_sl.AppendLine("           ,@vcYinQuType");
                strSql_add_sl.AppendLine("           ,NULL");
                strSql_add_sl.AppendLine("           ,@vcLabelStart");
                strSql_add_sl.AppendLine("           ,@vcLabelEnd");
                strSql_add_sl.AppendLine("           ,@vcSupplier_id)");

                sqlCommand_add_sl.CommandText = strSql_add_sl.ToString();
                sqlCommand_add_sl.Parameters.AddWithValue("@dHosDate", "");
                sqlCommand_add_sl.Parameters.AddWithValue("@vcBanZhi", "");
                sqlCommand_add_sl.Parameters.AddWithValue("@vcBianCi", "");
                sqlCommand_add_sl.Parameters.AddWithValue("@vcSellNo", "");
                sqlCommand_add_sl.Parameters.AddWithValue("@vcTruckNo", "");
                sqlCommand_add_sl.Parameters.AddWithValue("@vcSHF", "");
                sqlCommand_add_sl.Parameters.AddWithValue("@vcPart_id", "");
                sqlCommand_add_sl.Parameters.AddWithValue("@vcOrderNo", "");
                sqlCommand_add_sl.Parameters.AddWithValue("@vcLianFanNo", "");
                sqlCommand_add_sl.Parameters.AddWithValue("@vcInvoiceNo", "");
                sqlCommand_add_sl.Parameters.AddWithValue("@vcCaseNo", "");
                sqlCommand_add_sl.Parameters.AddWithValue("@vcBoxNo", "");
                sqlCommand_add_sl.Parameters.AddWithValue("@vcPartsNameEN", "");
                sqlCommand_add_sl.Parameters.AddWithValue("@iQuantity", "");
                sqlCommand_add_sl.Parameters.AddWithValue("@decPriceWithTax", "");
                sqlCommand_add_sl.Parameters.AddWithValue("@decMoney", "");
                sqlCommand_add_sl.Parameters.AddWithValue("@vcOperatorID", "");
                //sqlCommand_add_sl.Parameters.AddWithValue("@dOperatorTime", "");
                sqlCommand_add_sl.Parameters.AddWithValue("@vcYinQuType", "");
                //sqlCommand_add_sl.Parameters.AddWithValue("@vcSender", "");
                sqlCommand_add_sl.Parameters.AddWithValue("@vcLabelStart", "");
                sqlCommand_add_sl.Parameters.AddWithValue("@vcLabelEnd", "");
                sqlCommand_add_sl.Parameters.AddWithValue("@vcSupplier_id", "");
                #endregion
                foreach (DataRow item in dtSell_Temp.Rows)
                {
                    #region Value
                    sqlCommand_add_sl.Parameters["@dHosDate"].Value = item["dHosDate"].ToString();
                    sqlCommand_add_sl.Parameters["@vcBanZhi"].Value = item["vcBanZhi"].ToString();
                    sqlCommand_add_sl.Parameters["@vcBianCi"].Value = item["vcBianCi"].ToString();
                    sqlCommand_add_sl.Parameters["@vcSellNo"].Value = item["vcSellNo"].ToString();
                    sqlCommand_add_sl.Parameters["@vcTruckNo"].Value = item["vcTruckNo"].ToString();
                    sqlCommand_add_sl.Parameters["@vcSHF"].Value = item["vcSHF"].ToString();
                    sqlCommand_add_sl.Parameters["@vcPart_id"].Value = item["vcPart_id"].ToString();
                    sqlCommand_add_sl.Parameters["@vcOrderNo"].Value = item["vcOrderNo"].ToString();
                    sqlCommand_add_sl.Parameters["@vcLianFanNo"].Value = item["vcLianFanNo"].ToString();
                    sqlCommand_add_sl.Parameters["@vcInvoiceNo"].Value = item["vcInvoiceNo"].ToString();
                    sqlCommand_add_sl.Parameters["@vcCaseNo"].Value = item["vcCaseNo"].ToString();
                    sqlCommand_add_sl.Parameters["@vcBoxNo"].Value = item["vcBoxNo"].ToString();
                    sqlCommand_add_sl.Parameters["@vcPartsNameEN"].Value = item["vcPartsNameEN"].ToString();
                    sqlCommand_add_sl.Parameters["@iQuantity"].Value = item["iQuantity"].ToString();
                    sqlCommand_add_sl.Parameters["@decPriceWithTax"].Value = item["decPriceWithTax"].ToString();
                    sqlCommand_add_sl.Parameters["@decMoney"].Value = item["decMoney"].ToString();
                    sqlCommand_add_sl.Parameters["@vcOperatorID"].Value = item["vcOperatorID"].ToString();
                    //sqlCommand_add_sl.Parameters["@dOperatorTime"].Value = item["dOperatorTime"].ToString();
                    sqlCommand_add_sl.Parameters["@vcYinQuType"].Value = item["vcYinQuType"].ToString();
                    //sqlCommand_add_sl.Parameters["@vcSender"].Value = item["vcSender"].ToString();
                    sqlCommand_add_sl.Parameters["@vcLabelStart"].Value = item["vcLabelStart"].ToString();
                    sqlCommand_add_sl.Parameters["@vcLabelEnd"].Value = item["vcLabelEnd"].ToString();
                    sqlCommand_add_sl.Parameters["@vcSupplier_id"].Value = item["vcSupplier_id"].ToString();
                    #endregion
                    sqlCommand_add_sl.ExecuteNonQuery();
                }
                #endregion

                #region 5.ShipList  sqlCommand_add_slt
                SqlCommand sqlCommand_add_slt = sqlConnection.CreateCommand();
                sqlCommand_add_slt.Transaction = sqlTransaction;
                sqlCommand_add_slt.CommandType = CommandType.Text;
                StringBuilder strSql_add_slt = new StringBuilder();

                #region SQL and Parameters
                strSql_add_slt.AppendLine("INSERT INTO [dbo].[TShipList]");
                strSql_add_slt.AppendLine("           ([vcPackingspot]");
                strSql_add_slt.AppendLine("           ,[vcSupplier]");
                strSql_add_slt.AppendLine("           ,[vcCpdcompany]");
                strSql_add_slt.AppendLine("           ,[vcControlno]");
                strSql_add_slt.AppendLine("           ,[vcPart_id]");
                strSql_add_slt.AppendLine("           ,[vcOrderno]");
                strSql_add_slt.AppendLine("           ,[vcSeqno]");
                strSql_add_slt.AppendLine("           ,[vcInvoiceno]");
                strSql_add_slt.AppendLine("           ,[vcPartsnamechn]");
                strSql_add_slt.AppendLine("           ,[vcPartsnameen]");
                strSql_add_slt.AppendLine("           ,[vcShippingqty]");
                strSql_add_slt.AppendLine("           ,[vcCostwithtaxes]");
                strSql_add_slt.AppendLine("           ,[vcPrice]");
                strSql_add_slt.AppendLine("           ,[iNocount]");
                strSql_add_slt.AppendLine("           ,[vcCaseNo]");
                strSql_add_slt.AppendLine("           ,[vcBoxNo]");
                strSql_add_slt.AppendLine("           ,[vcProgramfrom]");
                strSql_add_slt.AppendLine("           ,[vcComputernm]");
                strSql_add_slt.AppendLine("           ,[vcPackcode]");
                strSql_add_slt.AppendLine("           ,[vcCompany]");
                strSql_add_slt.AppendLine("           ,[vcHostip]");
                strSql_add_slt.AppendLine("           ,[vcOperatorID]");
                strSql_add_slt.AppendLine("           ,[dOperatorTime]");
                strSql_add_slt.AppendLine("           ,[dFirstPrintTime]");
                strSql_add_slt.AppendLine("           ,[dLatelyPrintTime])");
                strSql_add_slt.AppendLine("     VALUES");
                strSql_add_slt.AppendLine("           (@vcPackingspot");
                strSql_add_slt.AppendLine("           ,@vcSupplier");
                strSql_add_slt.AppendLine("           ,@vcCpdcompany");
                strSql_add_slt.AppendLine("           ,@vcControlno");
                strSql_add_slt.AppendLine("           ,@vcPart_id");
                strSql_add_slt.AppendLine("           ,@vcOrderno");
                strSql_add_slt.AppendLine("           ,@vcSeqno");
                strSql_add_slt.AppendLine("           ,@vcInvoiceno");
                strSql_add_slt.AppendLine("           ,@vcPartsnamechn");
                strSql_add_slt.AppendLine("           ,@vcPartsnameen");
                strSql_add_slt.AppendLine("           ,@vcShippingqty");
                strSql_add_slt.AppendLine("           ,@vcCostwithtaxes");
                strSql_add_slt.AppendLine("           ,@vcPrice");
                strSql_add_slt.AppendLine("           ,@iNocount");
                strSql_add_slt.AppendLine("           ,@vcCaseNo");
                strSql_add_slt.AppendLine("           ,@vcBoxNo");
                strSql_add_slt.AppendLine("           ,NULL");
                strSql_add_slt.AppendLine("           ,NULL");
                strSql_add_slt.AppendLine("           ,NULL");
                strSql_add_slt.AppendLine("           ,NULL");
                strSql_add_slt.AppendLine("           ,@vcHostip");
                strSql_add_slt.AppendLine("           ,@vcOperatorID");
                strSql_add_slt.AppendLine("           ,GETDATE()");
                strSql_add_slt.AppendLine("           ,NULL");
                strSql_add_slt.AppendLine("           ,NULL)");
                strSql_add_slt.AppendLine("");

                sqlCommand_add_slt.CommandText = strSql_add_slt.ToString();
                sqlCommand_add_slt.Parameters.AddWithValue("@vcPackingspot", "");
                sqlCommand_add_slt.Parameters.AddWithValue("@vcSupplier", "");
                sqlCommand_add_slt.Parameters.AddWithValue("@vcCpdcompany", "");
                sqlCommand_add_slt.Parameters.AddWithValue("@vcControlno", "");
                sqlCommand_add_slt.Parameters.AddWithValue("@vcPart_id", "");
                sqlCommand_add_slt.Parameters.AddWithValue("@vcOrderno", "");
                sqlCommand_add_slt.Parameters.AddWithValue("@vcSeqno", "");
                sqlCommand_add_slt.Parameters.AddWithValue("@vcInvoiceno", "");
                sqlCommand_add_slt.Parameters.AddWithValue("@vcPartsnamechn", "");
                sqlCommand_add_slt.Parameters.AddWithValue("@vcPartsnameen", "");
                sqlCommand_add_slt.Parameters.AddWithValue("@vcShippingqty", "");
                sqlCommand_add_slt.Parameters.AddWithValue("@vcCostwithtaxes", "");
                sqlCommand_add_slt.Parameters.AddWithValue("@vcPrice", "");
                sqlCommand_add_slt.Parameters.AddWithValue("@iNocount", "");
                sqlCommand_add_slt.Parameters.AddWithValue("@vcCaseNo", "");
                sqlCommand_add_slt.Parameters.AddWithValue("@vcBoxNo", "");
                //sqlCommand_add_slt.Parameters.AddWithValue("@vcProgramfrom", "");
                //sqlCommand_add_slt.Parameters.AddWithValue("@vcComputernm", "");
                //sqlCommand_add_slt.Parameters.AddWithValue("@vcPackcode", "");
                //sqlCommand_add_slt.Parameters.AddWithValue("@vcCompany", "");
                sqlCommand_add_slt.Parameters.AddWithValue("@vcHostip", "");
                sqlCommand_add_slt.Parameters.AddWithValue("@vcOperatorID", "");
                //sqlCommand_add_slt.Parameters.AddWithValue("@dOperatorTime", "");
                //sqlCommand_add_slt.Parameters.AddWithValue("@dFirstPrintTime", "");
                //sqlCommand_add_slt.Parameters.AddWithValue("@dLatelyPrintTime", "");
                #endregion
                foreach (DataRow item in dtShipList_Temp.Rows)
                {
                    #region Value
                    sqlCommand_add_slt.Parameters["@vcPackingspot"].Value = item["vcPackingspot"].ToString();
                    sqlCommand_add_slt.Parameters["@vcSupplier"].Value = item["vcSupplier"].ToString();
                    sqlCommand_add_slt.Parameters["@vcCpdcompany"].Value = item["vcCpdcompany"].ToString();
                    sqlCommand_add_slt.Parameters["@vcControlno"].Value = item["vcControlno"].ToString();
                    sqlCommand_add_slt.Parameters["@vcPart_id"].Value = item["vcPart_id"].ToString();
                    sqlCommand_add_slt.Parameters["@vcOrderno"].Value = item["vcOrderno"].ToString();
                    sqlCommand_add_slt.Parameters["@vcSeqno"].Value = item["vcSeqno"].ToString();
                    sqlCommand_add_slt.Parameters["@vcInvoiceno"].Value = item["vcInvoiceno"].ToString();
                    sqlCommand_add_slt.Parameters["@vcPartsnamechn"].Value = item["vcPartsnamechn"].ToString();
                    sqlCommand_add_slt.Parameters["@vcPartsnameen"].Value = item["vcPartsnameen"].ToString();
                    sqlCommand_add_slt.Parameters["@vcShippingqty"].Value = item["vcShippingqty"].ToString();
                    sqlCommand_add_slt.Parameters["@vcCostwithtaxes"].Value = item["vcCostwithtaxes"].ToString();
                    sqlCommand_add_slt.Parameters["@vcPrice"].Value = item["vcPrice"].ToString();
                    sqlCommand_add_slt.Parameters["@iNocount"].Value = item["iNocount"].ToString();
                    sqlCommand_add_slt.Parameters["@vcCaseNo"].Value = item["vcCaseNo"].ToString();
                    sqlCommand_add_slt.Parameters["@vcBoxNo"].Value = item["vcBoxNo"].ToString();
                    //sqlCommand_add_slt.Parameters["@vcProgramfrom"].Value = item["vcProgramfrom"].ToString();
                    //sqlCommand_add_slt.Parameters["@vcComputernm"].Value = item["vcComputernm"].ToString();
                    //sqlCommand_add_slt.Parameters["@vcPackcode"].Value = item["vcPackcode"].ToString();
                    //sqlCommand_add_slt.Parameters["@vcCompany"].Value = item["vcCompany"].ToString();
                    sqlCommand_add_slt.Parameters["@vcHostip"].Value = item["vcHostip"].ToString();
                    sqlCommand_add_slt.Parameters["@vcOperatorID"].Value = item["vcOperatorID"].ToString();
                    //sqlCommand_add_slt.Parameters["@dOperatorTime"].Value = item["dOperatorTime"].ToString();
                    //sqlCommand_add_slt.Parameters["@dFirstPrintTime"].Value = item["dFirstPrintTime"].ToString();
                    //sqlCommand_add_slt.Parameters["@dLatelyPrintTime"].Value = item["dLatelyPrintTime"].ToString();
                    #endregion
                    sqlCommand_add_slt.ExecuteNonQuery();
                }
                #endregion

                #region 6.Sell_Sum  sqlCommand_add_ses
                SqlCommand sqlCommand_add_ses = sqlConnection.CreateCommand();
                sqlCommand_add_ses.Transaction = sqlTransaction;
                sqlCommand_add_ses.CommandType = CommandType.Text;
                StringBuilder strSql_add_ses = new StringBuilder();

                #region SQL and Parameters
                strSql_add_ses.AppendLine("INSERT INTO [dbo].[TSell_Sum]");
                strSql_add_ses.AppendLine("           ([vcBianCi]");
                strSql_add_ses.AppendLine("           ,[vcSellNo]");
                strSql_add_ses.AppendLine("           ,[vcTruckNo]");
                strSql_add_ses.AppendLine("           ,[iToolQuantity]");
                strSql_add_ses.AppendLine("           ,[vcYinQuType]");
                strSql_add_ses.AppendLine("           ,[vcOperatorID]");
                strSql_add_ses.AppendLine("           ,[dOperatorTime]");
                strSql_add_ses.AppendLine("           ,[vcBanZhi]");
                strSql_add_ses.AppendLine("           ,[vcDate]");
                strSql_add_ses.AppendLine("           ,[vcQianFen]");
                strSql_add_ses.AppendLine("           ,[vcLabelStart]");
                strSql_add_ses.AppendLine("           ,[vcLabelEnd]");
                strSql_add_ses.AppendLine("           ,[vcSender])");
                strSql_add_ses.AppendLine("     VALUES");
                strSql_add_ses.AppendLine("           (@vcBianCi");
                strSql_add_ses.AppendLine("           ,@vcSellNo");
                strSql_add_ses.AppendLine("           ,@vcTruckNo");
                strSql_add_ses.AppendLine("           ,@iToolQuantity");
                strSql_add_ses.AppendLine("           ,@vcYinQuType");
                strSql_add_ses.AppendLine("           ,@vcOperatorID");
                strSql_add_ses.AppendLine("           ,GETDATE()");
                strSql_add_ses.AppendLine("           ,@vcBanZhi");
                strSql_add_ses.AppendLine("           ,@vcDate");
                strSql_add_ses.AppendLine("           ,@vcQianFen");
                strSql_add_ses.AppendLine("           ,NULL");
                strSql_add_ses.AppendLine("           ,NULL");
                strSql_add_ses.AppendLine("           ,@vcSender)");

                sqlCommand_add_ses.CommandText = strSql_add_ses.ToString();
                sqlCommand_add_ses.Parameters.AddWithValue("@vcBianCi", "");
                sqlCommand_add_ses.Parameters.AddWithValue("@vcSellNo", "");
                sqlCommand_add_ses.Parameters.AddWithValue("@vcTruckNo", "");
                sqlCommand_add_ses.Parameters.AddWithValue("@iToolQuantity", "");
                sqlCommand_add_ses.Parameters.AddWithValue("@vcYinQuType", "");
                sqlCommand_add_ses.Parameters.AddWithValue("@vcOperatorID", "");
                //sqlCommand_add_ses.Parameters.AddWithValue("@dOperatorTime", "");
                sqlCommand_add_ses.Parameters.AddWithValue("@vcBanZhi", "");
                sqlCommand_add_ses.Parameters.AddWithValue("@vcDate", "");
                sqlCommand_add_ses.Parameters.AddWithValue("@vcQianFen", "");
                //sqlCommand_add_ses.Parameters.AddWithValue("@vcLabelStart", "");
                //sqlCommand_add_ses.Parameters.AddWithValue("@vcLabelEnd", "");
                sqlCommand_add_ses.Parameters.AddWithValue("@vcSender", "");
                #endregion
                foreach (DataRow item in dtSell_Sum_Temp.Rows)
                {
                    #region Value
                    sqlCommand_add_ses.Parameters["@vcBianCi"].Value = item["vcBianCi"].ToString();
                    sqlCommand_add_ses.Parameters["@vcSellNo"].Value = item["vcSellNo"].ToString();
                    sqlCommand_add_ses.Parameters["@vcTruckNo"].Value = item["vcTruckNo"].ToString();
                    sqlCommand_add_ses.Parameters["@iToolQuantity"].Value = item["iToolQuantity"].ToString();
                    sqlCommand_add_ses.Parameters["@vcYinQuType"].Value = item["vcYinQuType"].ToString();
                    sqlCommand_add_ses.Parameters["@vcOperatorID"].Value = item["vcOperatorID"].ToString();
                    //sqlCommand_add_ses.Parameters["@dOperatorTime"].Value = item["dOperatorTime"].ToString();
                    sqlCommand_add_ses.Parameters["@vcBanZhi"].Value = item["vcBanZhi"].ToString();
                    sqlCommand_add_ses.Parameters["@vcDate"].Value = item["vcDate"].ToString();
                    sqlCommand_add_ses.Parameters["@vcQianFen"].Value = item["vcQianFen"].ToString();
                    //sqlCommand_add_ses.Parameters["@vcLabelStart"].Value = item["vcLabelStart"].ToString();
                    //sqlCommand_add_ses.Parameters["@vcLabelEnd"].Value = item["vcLabelEnd"].ToString();
                    sqlCommand_add_ses.Parameters["@vcSender"].Value = item["vcSender"].ToString();
                    #endregion
                    sqlCommand_add_ses.ExecuteNonQuery();
                }
                #endregion

                #region 7.Sell_Tool  sqlCommand_add_set
                SqlCommand sqlCommand_add_set = sqlConnection.CreateCommand();
                sqlCommand_add_set.Transaction = sqlTransaction;
                sqlCommand_add_set.CommandType = CommandType.Text;
                StringBuilder strSql_add_set = new StringBuilder();

                #region SQL and Parameters
                strSql_add_set.AppendLine("INSERT INTO [dbo].[TSell_Tool]");
                strSql_add_set.AppendLine("           ([vcSellNo]");
                strSql_add_set.AppendLine("           ,[vcToolName]");
                strSql_add_set.AppendLine("           ,[vcToolCode]");
                strSql_add_set.AppendLine("           ,[vcToolColor]");
                strSql_add_set.AppendLine("           ,[iToolQuantity]");
                strSql_add_set.AppendLine("           ,[vcOperatorID]");
                strSql_add_set.AppendLine("           ,[dOperatorTime]");
                strSql_add_set.AppendLine("           ,[vcYinQuType])");
                strSql_add_set.AppendLine("     VALUES");
                strSql_add_set.AppendLine("           (@vcSellNo");
                strSql_add_set.AppendLine("           ,@vcToolName");
                strSql_add_set.AppendLine("           ,@vcToolCode");
                strSql_add_set.AppendLine("           ,@vcToolColor");
                strSql_add_set.AppendLine("           ,@iToolQuantity");
                strSql_add_set.AppendLine("           ,@vcOperatorID");
                strSql_add_set.AppendLine("           ,GETDATE()");
                strSql_add_set.AppendLine("           ,@vcYinQuType)");

                sqlCommand_add_set.CommandText = strSql_add_set.ToString();
                sqlCommand_add_set.Parameters.AddWithValue("@vcSellNo", "");
                sqlCommand_add_set.Parameters.AddWithValue("@vcToolName", "");
                sqlCommand_add_set.Parameters.AddWithValue("@vcToolCode", "");
                sqlCommand_add_set.Parameters.AddWithValue("@vcToolColor", "");
                sqlCommand_add_set.Parameters.AddWithValue("@iToolQuantity", "");
                sqlCommand_add_set.Parameters.AddWithValue("@vcOperatorID", "");
                //sqlCommand_add_set.Parameters.AddWithValue("@dOperatorTime", "");
                sqlCommand_add_set.Parameters.AddWithValue("@vcYinQuType", "");
                #endregion
                foreach (DataRow item in dtSell_Tool_Temp.Rows)
                {
                    #region Value
                    sqlCommand_add_set.Parameters["@vcSellNo"].Value = item["vcSellNo"].ToString();
                    sqlCommand_add_set.Parameters["@vcToolName"].Value = item["vcToolName"].ToString();
                    sqlCommand_add_set.Parameters["@vcToolCode"].Value = item["vcToolCode"].ToString();
                    sqlCommand_add_set.Parameters["@vcToolColor"].Value = item["vcToolColor"].ToString();
                    sqlCommand_add_set.Parameters["@iToolQuantity"].Value = item["iToolQuantity"].ToString();
                    sqlCommand_add_set.Parameters["@vcOperatorID"].Value = item["vcOperatorID"].ToString();
                    //sqlCommand_add_set.Parameters["@dOperatorTime"].Value = item["dOperatorTime"].ToString();
                    sqlCommand_add_set.Parameters["@vcYinQuType"].Value = item["vcYinQuType"].ToString();
                    #endregion
                    sqlCommand_add_set.ExecuteNonQuery();
                }
                #endregion

                #region 8.TPrint_Temp  sqlCommand_mod_pt
                SqlCommand sqlCommand_mod_pt = sqlConnection.CreateCommand();
                sqlCommand_mod_pt.Transaction = sqlTransaction;
                sqlCommand_mod_pt.CommandType = CommandType.Text;
                StringBuilder strSql_mod_pt = new StringBuilder();

                #region SQL and Parameters
                strSql_mod_pt.AppendLine("INSERT INTO [dbo].[TPrint_Temp]");
                strSql_mod_pt.AppendLine("           ([vcTableName]");
                strSql_mod_pt.AppendLine("           ,[vcReportName]");
                strSql_mod_pt.AppendLine("           ,[vcClientIP]");
                strSql_mod_pt.AppendLine("           ,[vcPrintName]");
                strSql_mod_pt.AppendLine("           ,[vcKind]");
                strSql_mod_pt.AppendLine("           ,[vcOperatorID]");
                strSql_mod_pt.AppendLine("           ,[dOperatorTime]");
                strSql_mod_pt.AppendLine("           ,[vcCaseNo]");
                strSql_mod_pt.AppendLine("           ,[vcSellNo]");
                strSql_mod_pt.AppendLine("           ,[vcLotid]");
                strSql_mod_pt.AppendLine("           ,[vcSupplierId]");
                strSql_mod_pt.AppendLine("           ,[vcInno]");
                strSql_mod_pt.AppendLine("           ,[vcFlag])");
                strSql_mod_pt.AppendLine("     VALUES");
                strSql_mod_pt.AppendLine("           ('TShipList'");
                strSql_mod_pt.AppendLine("           ,'SPR07SHPP'");
                strSql_mod_pt.AppendLine("           ,@IP");
                strSql_mod_pt.AppendLine("           ,'LASEL PRINTER'");
                strSql_mod_pt.AppendLine("           ,'4'");
                strSql_mod_pt.AppendLine("           ,@vcOperatorID");
                strSql_mod_pt.AppendLine("           ,GETDATE()");
                strSql_mod_pt.AppendLine("           ,null");
                strSql_mod_pt.AppendLine("           ,@vcSellNo");
                strSql_mod_pt.AppendLine("           ,null");
                strSql_mod_pt.AppendLine("           ,null");
                strSql_mod_pt.AppendLine("           ,null");
                strSql_mod_pt.AppendLine("           ,'0')");
                sqlCommand_mod_pt.CommandText = strSql_mod_pt.ToString();
                sqlCommand_mod_pt.Parameters.AddWithValue("@IP", strIP);
                sqlCommand_mod_pt.Parameters.AddWithValue("@vcOperatorID", strOperId);
                sqlCommand_mod_pt.Parameters.AddWithValue("@vcSellNo", strSellno);
                #endregion
                sqlCommand_mod_pt.ExecuteNonQuery();

                #endregion

                #region 9.TShip_Temp、TSell_DockCar、TOperate_OutPut_Temp   sqlCommand_mod_dock
                SqlCommand sqlCommand_mod_dock = sqlConnection.CreateCommand();
                sqlCommand_mod_dock.Transaction = sqlTransaction;
                sqlCommand_mod_dock.CommandType = CommandType.Text;
                StringBuilder strSql_mod_dock = new StringBuilder();

                #region SQL and Parameters
                strSql_mod_dock.AppendLine("update TShip_Temp set vcFlag='1',vcOperatorID=@vcOperatorID,dOperatorTime=GETDATE() where vcDockSell=@vcDockSell");
                strSql_mod_dock.AppendLine("update TSell_DockCar set vcFlag='1',vcOperatorID=@vcOperatorID,dOperatorTime=GETDATE() where vcDockSell=@vcDockSell");
                strSql_mod_dock.AppendLine("DELETE FROM [dbo].[TOperate_OutPut_Temp] WHERE vcDockSell=@vcDockSell");

                sqlCommand_mod_dock.CommandText = strSql_mod_dock.ToString();
                sqlCommand_mod_dock.Parameters.AddWithValue("@vcOperatorID", strOperId);
                sqlCommand_mod_dock.Parameters.AddWithValue("@vcDockSell", strDock);
                sqlCommand_mod_dock.ExecuteNonQuery();
                #endregion
                #endregion

                #region 写入数据库
                //提交事务
                sqlTransaction.Commit();
                sqlConnection.Close();
                return true;
                #endregion

            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage("P00001", "M03UE0901", ex, "000000");
                //回滚事务
                if (sqlTransaction != null && sqlConnection != null)
                {
                    sqlTransaction.Rollback();
                    sqlConnection.Close();
                }
                return false;
            }
        }
        public DataTable getSellInfo(string strSellno)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("select vcInvoiceno as inv_no");
            stringBuilder.AppendLine(",SUBSTRING(vcControlno,3,8) as inv_date");
            stringBuilder.AppendLine(",vcPart_id as part_no");
            stringBuilder.AppendLine(",vcPartsnamechn as part_name");
            stringBuilder.AppendLine(",vcCaseno as case_no");
            stringBuilder.AppendLine(",vcOrderno as ord_no");
            stringBuilder.AppendLine(",vcSeqno as item_no");
            stringBuilder.AppendLine(",'TFTM' AS dlr_no");
            stringBuilder.AppendLine(",vcShippingqty as qty");
            stringBuilder.AppendLine(",vcCostwithtaxes as price ");
            stringBuilder.AppendLine("from TShipList ");
            stringBuilder.AppendLine("where vcControlno='" + strSellno + "'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();

            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }


        public DataSet getSumandToolOfSell(string strSellNo,string strYinQuType)
        {
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("select vcBianCi,vcTruckNo,iToolQuantity,vcYinQuType,vcBanZhi,vcDate ");
            stringBuilder.AppendLine("from TSell_Sum ");
            stringBuilder.AppendLine("where vcYinQuType='" + strYinQuType + "' and  vcSellNo='" + strSellNo + "'");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("select vcToolName,iToolQuantity ");
            stringBuilder.AppendLine("from TSell_Tool");
            stringBuilder.AppendLine("where vcYinQuType='" + strYinQuType + "' and  vcSellNo='" + strSellNo + "'");
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }
        public DataTable GetDataInfo(string sellNo, string bianCi)
        {
            StringBuilder GetDataInfoSql = new StringBuilder();
            GetDataInfoSql.Append("select vcBianCi,vcTruckNo,iToolQuantity,vcYinQuType,vcBanZhi,vcDate from TSell_Sum where vcYinQuType='" + bianCi + "' and  vcSellNo='" + sellNo + "'");
            return excute.ExcuteSqlWithSelectToDT(GetDataInfoSql.ToString());
        }
        public DataTable GetSellInfo3(string sellNo, string bianCi)
        {
            StringBuilder GetSellInfoSql = new StringBuilder();
            GetSellInfoSql.Append("select vcToolName,iToolQuantity from TSell_Tool where vcYinQuType='" + bianCi + "' and  vcSellNo='" + sellNo + "'");
            return excute.ExcuteSqlWithSelectToDT(GetSellInfoSql.ToString());
        }
        public DataTable GetCaseSum(string dock)
        {
            StringBuilder GetCaseSumSql = new StringBuilder();
            GetCaseSumSql.Append("      select distinct(vcBoxNo) from TShip_Temp where vcDockSell='" + dock + "' and vcFlag='0'");

            return excute.ExcuteSqlWithSelectToDT(GetCaseSumSql.ToString());
        }










    }
}
