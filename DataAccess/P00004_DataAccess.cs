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

    public DataTable ValidateDock1(string dock)
    {
      StringBuilder ValidateDockSql = new StringBuilder();
      ValidateDockSql.Append("   select * from TSell_DockCar where vcDockSell='" + dock + "' and vcFlag='0'");
      return excute.ExcuteSqlWithSelectToDT(ValidateDockSql.ToString());
    }

    public DataTable GetCaseSum(string dock)
    {
      StringBuilder GetCaseSumSql = new StringBuilder();
      GetCaseSumSql.Append("      select distinct(vcBoxNo) from TShip_Temp where vcDockSell='" + dock + "' and vcFlag='0'");

      return excute.ExcuteSqlWithSelectToDT(GetCaseSumSql.ToString());
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
      ValidatePriceSql.Append("   select decPriceTNPWithTax  from TPrice where vcSupplier_id='"+supplierId+"' and vcReceiver='"+cpdCompany+"' and vcPart_id='"+partId+"' and dPricebegin<='"+scanTime+"' and dPriceEnd>='"+scanTime+"'");
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

    public int InsertDock(string dock, string fork, string scanTime, string opearteId)
    {
      StringBuilder InsertDockSql = new StringBuilder();
      InsertDockSql.Append("INSERT INTO TSell_DockCar (vcForkNo,vcDockSell,vcFlag,vcOperatorID,dOperatorTime) \n");
      InsertDockSql.Append("VALUES('" + fork + "','" + dock + "','0','" + opearteId + "','" + scanTime + "')");
      return excute.ExcuteSqlWithStringOper(InsertDockSql.ToString());
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



    public DataTable GetSellInfo3(string sellNo, string bianCi)
    {
      StringBuilder GetSellInfoSql = new StringBuilder();
      GetSellInfoSql.Append("select vcToolName,iToolQuantity from TSell_Tool where vcYinQuType='" + bianCi + "' and  vcSellNo='" + sellNo + "'");
      return excute.ExcuteSqlWithSelectToDT(GetSellInfoSql.ToString());
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

    public DataTable GetDataInfo(string sellNo, string bianCi)
    {
      StringBuilder GetDataInfoSql = new StringBuilder();
      GetDataInfoSql.Append("select vcBianCi,vcTruckNo,iToolQuantity,vcYinQuType,vcBanZhi,vcDate from TSell_Sum where vcYinQuType='" + bianCi + "' and  vcSellNo='" + sellNo + "'");
      return excute.ExcuteSqlWithSelectToDT(GetDataInfoSql.ToString());
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
      InsertOprSql.Append("" + quantity + ",'" + packingQuatity + "','" + cpdCompany + "','" + dock + "','" + caseNo + "','"+pointType+"','" + checkType + "'," + quantity + ",'" + checkStatus + "','" + lblStart + "','" + lblEnd + "','',null,'" + sellNo + "','" + opearteId + "','" + serverTime + "','"+iP+"','1','')\n");
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



    public int DelCase(string caseNo)
    {
      StringBuilder DelCaseSql = new StringBuilder();
      DelCaseSql.Append("  update TShip_Temp set vcFlag='1' where vcBoxNo='" + caseNo + "'");
      return excute.ExcuteSqlWithStringOper(DelCaseSql.ToString());
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

















  }
}
