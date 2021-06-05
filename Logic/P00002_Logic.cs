using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;
using System.Net;


namespace Logic
{
  public class P00002_Logic
  {
    static P00002_DataAccess P00002_DataAccess = new P00002_DataAccess();





    public DataTable ValidateOpr(string partId, string quantity, string dock, string kanbanOrderNo, string kanbanSerial)
    {
      return P00002_DataAccess.ValidateOpr(partId, quantity, dock, kanbanOrderNo, kanbanSerial);
    }


    public static DataTable GetCheckType(string partId, string kanbanOrderNo, string kanbanSerial, string dock, string scanTime, string supplierId)
    {
      return P00002_DataAccess.GetCheckType(partId, kanbanOrderNo, kanbanSerial, dock, scanTime,supplierId);
    }



    public static DataTable ValidateOpr1(string partId, string quantity, string dock, string kanbanOrderNo, string kanbanSerial)
    {
      return P00002_DataAccess.ValidateOpr1(partId, quantity, dock, kanbanOrderNo, kanbanSerial);
    }

    public int InsertOpr(string packingSpot, string inputNo, string kanbanOrderNo, string kanbanSerial, string partId, string inOutFlag, string supplierId, string supplierPlant, string scanTime, string serverTime, string quantity, string packingQuantity, string cpdCompany, string dock, string checkType, string lblStart, string lblEnd, string opearteId, string value, string ngQuantity,string pointType,string iP)
    {
      return P00002_DataAccess.InsertOpr(packingSpot, inputNo, kanbanOrderNo, kanbanSerial, partId, inOutFlag, supplierId, supplierPlant, scanTime, serverTime, quantity, packingQuantity, cpdCompany, dock, checkType, lblStart, lblEnd, opearteId, value, ngQuantity,pointType,iP);
    }

    public DataTable GetInnoData(string inno)
    {
      return P00002_DataAccess.GetInnoData(inno);
    }

    public DataTable GetNGReason()
    {
      return P00002_DataAccess.GetNGReason();
    }

    public DataTable GetNGBlame()
    {
      return P00002_DataAccess.GetNGBlame();
    }

    public int InsertNG(string partId, string kanbanOrderNo, string kanbanSerial, string dock, string quantity, string ngReason, string ngBlame, string opearteId, string serverTime, string ngQuantity)
    {
      return P00002_DataAccess.InsertNG(partId, kanbanOrderNo, kanbanSerial, dock, quantity, ngReason, ngBlame, opearteId, serverTime, ngQuantity);
    }

    public DataTable GetCheckType1(string partId, string scanTime)
    {
      return P00002_DataAccess.GetCheckType1(partId, scanTime);
    }

    public DataTable GetInputQuantity(string partId, string kanbanOrderNo, string kanbanSerial, string dock)
    {
      return P00002_DataAccess.GetInputQuantity(partId, kanbanOrderNo, kanbanSerial, dock);
    }

    public int InsertOpr(string packingSpot, string inputNo, string kanbanOrderNo, string kanbanSerial, string partId, string inOutFlag, string supplierId, string supplierPlant, string scanTime, string serverTime, string quantity, string packingQuantity, string cpdCompany, string dock, string checkType, string lblStart, string lblEnd, string opearteId, string value, string pointType, string iP)
    {
      return P00002_DataAccess.InsertOpr(packingSpot, inputNo, kanbanOrderNo, kanbanSerial, partId, inOutFlag, supplierId, supplierPlant, scanTime, serverTime, quantity, packingQuantity, cpdCompany, dock, checkType, lblStart, lblEnd, opearteId, value,pointType,iP);
    }

    public DataTable GetSPIS(string partId, string scanTime, string supplierId)
    {
      return P00002_DataAccess.GetSPIS(partId, scanTime, supplierId);
    }

    public DataTable GetSupplier(string partId, string scanTime)
    {
      return P00002_DataAccess.GetSupplier(partId, scanTime);
    }

    public DataTable GetCheckQuantity(string quantity)
    {
      return P00002_DataAccess.GetCheckQuantity(quantity);
    }

    public int InsertOpr(string packingSpot, string inputNo, string kanbanOrderNo, string kanbanSerial, string partId, string inOutFlag, string supplierId, string supplierPlant, string scanTime, string serverTime, string quantity, string packingQuantity, string cpdCompany, string dock, string checkType, string lblStart, string lblEnd, string opearteId, string value, string ngQuantity, string checkQuantity, string pointType, string iP)
    {
      return P00002_DataAccess.InsertOpr(packingSpot, inputNo, kanbanOrderNo, kanbanSerial, partId, inOutFlag, supplierId, supplierPlant, scanTime, serverTime, quantity, packingQuantity, cpdCompany, dock, checkType, lblStart, lblEnd, opearteId, value, ngQuantity, checkQuantity,pointType,iP);
    }

    public int InsertOpr1(string packingSpot, string inputNo, string kanbanOrderNo, string kanbanSerial, string partId, string inOutFlag, string supplierId, string supplierPlant, string scanTime, string serverTime, string quantity, string packingQuantity, string cpdCompany, string dock, string checkType, string lblStart, string lblEnd, string opearteId, string value, string checkQuantity, string pointType, string iP)
    {
      return P00002_DataAccess.InsertOpr1(packingSpot, inputNo, kanbanOrderNo, kanbanSerial, partId, inOutFlag, supplierId, supplierPlant, scanTime, serverTime, quantity, packingQuantity, cpdCompany, dock, checkType, lblStart, lblEnd, opearteId, value, checkQuantity,pointType,iP);
    }

    public DataTable ValidateData(string partId, string scanTime)
    {
      return P00002_DataAccess.ValidateData(partId, scanTime);
    }

    public DataTable GetPoint(string iP)
    {
      return P00002_DataAccess.GetPoint(iP);
    }

 
  }
}
