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
  public class P00002_DataAccess
  {
    private MultiExcute excute = new MultiExcute();



    public DataTable GetCheckType(string partId, string kanbanOrderNo, string kanbanSerial, string dock, string scanTime, string supplierId)
    {
      StringBuilder GetCheckTypeSql = new StringBuilder();
      GetCheckTypeSql.Append(" select vcCheckP,vcTJSX from tCheckQf where vcSupplierCode='"+supplierId+"' and  vcPartId='" + partId + "' and vcTimeFrom<='" + scanTime + "'and vcTimeTo>='" + scanTime + "'");
      return excute.ExcuteSqlWithSelectToDT(GetCheckTypeSql.ToString());
    }


    public int InsertOpr(string packingSpot, string inputNo, string kanbanOrderNo, string kanbanSerial, string partId, string inOutFlag, string supplierId, string supplierPlant, string scanTime, string serverTime, string quantity, string packingQuantity, string cpdCompany, string dock, string checkType, string lblStart, string lblEnd, string opearteId, string value, string ngQuantity)
    {
      StringBuilder InsertOprSql = new StringBuilder();
      InsertOprSql.Append("INSERT INTO TOperateSJ (vcZYType,vcBZPlant,vcInputNo,vcKBOrderNo,vcKBLFNo,vcPart_id,vcIOType,vcSupplier_id,vcSupplierGQ,dStart\n");
      InsertOprSql.Append(",dEnd,iQuantity,vcBZUnit,vcSHF,vcSR,vcBoxNo,vcSheBeiNo,vcCheckType,iCheckNum,vcCheckStatus,vcLabelStart,vcLabelEnd\n");
      InsertOprSql.Append(",vcUnlocker,dUnlockTime,vcSellNo,vcOperatorID,dOperatorTime)\n");
      InsertOprSql.Append("VALUES('S1','" + packingSpot + "','" + inputNo + "','" + kanbanOrderNo + "','" + kanbanSerial + "','" + partId + "','" + inOutFlag + "','" + supplierId + "','" + supplierPlant + "','" + scanTime + "','" + serverTime + "',\n");
      InsertOprSql.Append("" + int.Parse(quantity) + ",'" + packingQuantity + "','" + cpdCompany + "','" + dock + "','','','" + checkType + "'," + int.Parse(quantity) + ",'" + value + "','" + lblStart + "','" + lblEnd + "','',null,'','" + opearteId + "','" + serverTime + "')\n");
      return excute.ExcuteSqlWithStringOper(InsertOprSql.ToString());
    }

    public int InsertNG(string partId, string kanbanOrderNo, string kanbanSerial, string dock, string quantity, string ngReason, string ngBlame, string opearteId, string serverTime, string ngQuantity)
    {
      StringBuilder InsertNGSql = new StringBuilder();
      InsertNGSql.Append("INSERT INTO TOperateSJ_NG (vcPart_id ,vcKBOrderNo ,vcKBLFNo,vcSR ,iNGQuantity,vcNGReason,vcZRBS,vcOperatorID,dOperatorTime) \n");
      InsertNGSql.Append(" VALUES ('" + partId + "','" + kanbanOrderNo + "','" + kanbanSerial + "','" + dock + "','" + ngQuantity + "','" + ngReason + "','" + ngBlame + "','" + opearteId + "','" + serverTime + "') \n");


      return excute.ExcuteSqlWithStringOper(InsertNGSql.ToString());
    }

    public DataTable GetInputQuantity(string partId, string kanbanOrderNo, string kanbanSerial, string dock)
    {
      StringBuilder GetInputQuantitySql = new StringBuilder();
      GetInputQuantitySql.Append("select iQuantity from TOperateSJ where vcPart_id='" + partId + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "' and vcSR='" + dock + "' and vcZYType='S0'");
      return excute.ExcuteSqlWithSelectToDT(GetInputQuantitySql.ToString());
    }

    public int InsertOpr(string packingSpot, string inputNo, string kanbanOrderNo, string kanbanSerial, string partId, string inOutFlag, string supplierId, string supplierPlant, string scanTime, string serverTime, string quantity, string packingQuantity, string cpdCompany, string dock, string checkType, string lblStart, string lblEnd, string opearteId, string value)
    {
      StringBuilder InsertOprSql = new StringBuilder();
      InsertOprSql.Append("INSERT INTO TOperateSJ (vcZYType,vcBZPlant,vcInputNo,vcKBOrderNo,vcKBLFNo,vcPart_id,vcIOType,vcSupplier_id,vcSupplierGQ,dStart\n");
      InsertOprSql.Append(",dEnd,iQuantity,vcBZUnit,vcSHF,vcSR,vcBoxNo,vcSheBeiNo,vcCheckType,iCheckNum,vcCheckStatus,vcLabelStart,vcLabelEnd\n");
      InsertOprSql.Append(",vcUnlocker,dUnlockTime,vcSellNo,vcOperatorID,dOperatorTime)\n");
      InsertOprSql.Append("VALUES('S1','" + packingSpot + "','" + inputNo + "','" + kanbanOrderNo + "','" + kanbanSerial + "','" + partId + "','" + inOutFlag + "','" + supplierId + "','" + supplierPlant + "','" + scanTime + "','" + serverTime + "',\n");
      InsertOprSql.Append("" + int.Parse(quantity) + ",'" + packingQuantity + "','" + cpdCompany + "','" + dock + "','','','" + checkType + "'," + int.Parse(quantity) + ",'" + value + "','" + lblStart + "','" + lblEnd + "','',null,'','" + opearteId + "','" + serverTime + "')\n");
      return excute.ExcuteSqlWithStringOper(InsertOprSql.ToString());
    }

    public int InsertOpr(string packingSpot, string inputNo, string kanbanOrderNo, string kanbanSerial, string partId, string inOutFlag, string supplierId, string supplierPlant, string scanTime, string serverTime, string quantity, string packingQuantity, string cpdCompany, string dock, string checkType, string lblStart, string lblEnd, string opearteId, string value, string ngQuantity, string checkQuantity)
    {
      StringBuilder InsertOprSql = new StringBuilder();
      InsertOprSql.Append("INSERT INTO TOperateSJ (vcZYType,vcBZPlant,vcInputNo,vcKBOrderNo,vcKBLFNo,vcPart_id,vcIOType,vcSupplier_id,vcSupplierGQ,dStart\n");
      InsertOprSql.Append(",dEnd,iQuantity,vcBZUnit,vcSHF,vcSR,vcBoxNo,vcSheBeiNo,vcCheckType,iCheckNum,vcCheckStatus,vcLabelStart,vcLabelEnd\n");
      InsertOprSql.Append(",vcUnlocker,dUnlockTime,vcSellNo,vcOperatorID,dOperatorTime)\n");
      InsertOprSql.Append("VALUES('S1','" + packingSpot + "','" + inputNo + "','" + kanbanOrderNo + "','" + kanbanSerial + "','" + partId + "','" + inOutFlag + "','" + supplierId + "','" + supplierPlant + "','" + scanTime + "','" + serverTime + "',\n");
      InsertOprSql.Append("" + int.Parse(quantity) + ",'" + packingQuantity + "','" + cpdCompany + "','" + dock + "','','','" + checkType + "'," + int.Parse(checkQuantity) + ",'" + value + "','" + lblStart + "','" + lblEnd + "','',null,'','" + opearteId + "','" + serverTime + "')\n");
      return excute.ExcuteSqlWithStringOper(InsertOprSql.ToString());
    }

    public DataTable ValidateData(string partId, string scanTime)
    {
      StringBuilder ValidateDataSql = new StringBuilder();
      ValidateDataSql.Append("select vcPartId from TSPMaster where vcPartId='" + partId + "' and dFromTime<='" + scanTime + "' and dToTime>='" + scanTime + "'");
      return excute.ExcuteSqlWithSelectToDT(ValidateDataSql.ToString());
    }

    public int InsertOpr1(string packingSpot, string inputNo, string kanbanOrderNo, string kanbanSerial, string partId, string inOutFlag, string supplierId, string supplierPlant, string scanTime, string serverTime, string quantity, string packingQuantity, string cpdCompany, string dock, string checkType, string lblStart, string lblEnd, string opearteId, string value, string checkQuantity)
    {
      StringBuilder InsertOprSql = new StringBuilder();
      InsertOprSql.Append("INSERT INTO TOperateSJ (vcZYType,vcBZPlant,vcInputNo,vcKBOrderNo,vcKBLFNo,vcPart_id,vcIOType,vcSupplier_id,vcSupplierGQ,dStart\n");
      InsertOprSql.Append(",dEnd,iQuantity,vcBZUnit,vcSHF,vcSR,vcBoxNo,vcSheBeiNo,vcCheckType,iCheckNum,vcCheckStatus,vcLabelStart,vcLabelEnd\n");
      InsertOprSql.Append(",vcUnlocker,dUnlockTime,vcSellNo,vcOperatorID,dOperatorTime)\n");
      InsertOprSql.Append("VALUES('S1','" + packingSpot + "','" + inputNo + "','" + kanbanOrderNo + "','" + kanbanSerial + "','" + partId + "','" + inOutFlag + "','" + supplierId + "','" + supplierPlant + "','" + scanTime + "','" + serverTime + "',\n");
      InsertOprSql.Append("" + int.Parse(quantity) + ",'" + packingQuantity + "','" + cpdCompany + "','" + dock + "','','','" + checkType + "'," + int.Parse(checkQuantity) + ",'" + value + "','" + lblStart + "','" + lblEnd + "','',null,'','" + opearteId + "','" + serverTime + "')\n");
      return excute.ExcuteSqlWithStringOper(InsertOprSql.ToString());
    }

    public DataTable GetCheckQuantity(string quantity)
    {
      StringBuilder GetCheckQuantitySql = new StringBuilder();
      GetCheckQuantitySql.Append("select iSpotQty from tCheckSpot where iPackingQty='" + quantity + "'");


      return excute.ExcuteSqlWithSelectToDT(GetCheckQuantitySql.ToString());
    }

    public DataTable GetSupplier(string partId, string scanTime)
    {
      StringBuilder GetSupplierSql = new StringBuilder();
      GetSupplierSql.Append("select vcSupplierId from TSPMaster where vcPartId='" + partId + "' and dFromTime<='" + scanTime + "' and dToTime>='" + scanTime + "'");
      return excute.ExcuteSqlWithSelectToDT(GetSupplierSql.ToString());
    }

    public DataTable GetSPIS(string partId, string scanTime, string supplierId)
    {
      StringBuilder GetSPISSql = new StringBuilder();
      GetSPISSql.Append("select vcPicUrl from TSPISQf where vcSupplierCode='" + supplierId + "' and vcTimeFrom<='" + scanTime + "' and vcTimeTo>='" + scanTime + "' and vcPartId='" + partId + "'");
      return excute.ExcuteSqlWithSelectToDT(GetSPISSql.ToString());
    }

    public DataTable GetCheckType1(string partId, string scanTime)
    {
      StringBuilder GetCheckTypeSql = new StringBuilder();
      GetCheckTypeSql.Append("select vcCheckP,vcTJSX from tCheckQf where vcPartId='" + partId + "' and vcTimeFrom<='" + scanTime + "' and vcTimeTo>='" + scanTime + "'");
      return excute.ExcuteSqlWithSelectToDT(GetCheckTypeSql.ToString());
    }

    public DataTable GetNGBlame()
    {
      StringBuilder GetNGBlameSql = new StringBuilder();
      GetNGBlameSql.Append("select vcblame from TNG_Blame");

      return excute.ExcuteSqlWithSelectToDT(GetNGBlameSql.ToString());
    }

    public DataTable GetNGReason()
    {
      StringBuilder GetNGReasonSql = new StringBuilder();
      GetNGReasonSql.Append("select vcNgReason from TNgReason");
      return excute.ExcuteSqlWithSelectToDT(GetNGReasonSql.ToString());
    }

    public DataTable GetInnoData(string inno)
    {
      StringBuilder GetInnoDataSql = new StringBuilder();

      GetInnoDataSql.Append("select vcPart_id,vcSR,iQuantity,vcKBOrderNo,vcKBLFNo from TOperateSJ where vcInputNo='" + inno + "' and vcZYType='S0'");
      return excute.ExcuteSqlWithSelectToDT(GetInnoDataSql.ToString());
    }

    public DataTable ValidateOpr1(string partId, string quantity, string dock, string kanbanOrderNo, string kanbanSerial)
    {
      StringBuilder ValiateOpr1Sql = new StringBuilder();
      ValiateOpr1Sql.Append("  select vcBZPlant from TOperateSJ where vcPart_id='" + partId + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "' and vcSR='" + dock + "' and vcZYType='S1'");
      return excute.ExcuteSqlWithSelectToDT(ValiateOpr1Sql.ToString());
    }


    public DataTable ValidateOpr(string partId, string quantity, string dock, string kanbanOrderNo, string kanbanSerial)
    {
      StringBuilder ValidateOprSql = new StringBuilder();
      ValidateOprSql.Append("   select   vcBZPlant,vcInputNo,vcIOType,vcSupplier_id,vcSupplierGQ,vcBZUnit,vcSHF,vcCheckType,vcLabelStart,vcLabelEnd,iQuantity from TOperateSJ where vcPart_id='" + partId + "' and vcSR='" + dock + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "'  and vcZYType='S0'");
      return excute.ExcuteSqlWithSelectToDT(ValidateOprSql.ToString());
    }


  }
}
