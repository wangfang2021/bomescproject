using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;
using System.Net;

using System.Web;
using System.Drawing;
using DataEntity;
using System.IO;
using System.Xml;
using QRCoder;
using System.Drawing.Imaging;

namespace Logic
{
  public class P00001_Logic
  {


    static P00001_DataAccess P00001_DataAccess = new P00001_DataAccess();


    public static DataTable Validate(string partId, string kanbanOrderNo, string kanbanSerial, string dock)
    {
      return P00001_DataAccess.Validate(partId, kanbanOrderNo, kanbanSerial, dock);
    }

    public static int Insert(string trolley, string partId, string quantity, string dock, string kanbanOrderNo, string kanbanSerial, string scanTime, String iP, string serverTime, string cpdCompany, string inno, string opearteId, string packingSpot, string packQuantity, string lblSart, string lblEnd, string supplierId, string supplierPlant, string trolleySeqNo, string inoutFlag, string kanBan)
    {
      return P00001_DataAccess.Insert(trolley, partId, quantity, dock, kanbanOrderNo, kanbanSerial, scanTime, iP, serverTime, cpdCompany, inno, opearteId, packingSpot, packQuantity, lblSart, lblEnd, supplierId, supplierPlant, trolleySeqNo, inoutFlag, kanBan);
    }

    public static DataTable ValidateData(string partId, string scanTime, string dock)
    {
      return P00001_DataAccess.ValidateData(partId, scanTime, dock);
    }

    public static DataTable GetSum(string iP)
    {
      // return P00001_DataAccess.GetSum(iP);
      return P00001_DataAccess.GetSum(iP);
    }

    public static DataTable GetQuantity(string inno)
    {
      return P00001_DataAccess.GetQuantity(inno);
    }

    public static DataTable GetUserRole(string opearteId)
    {
      return P00001_DataAccess.GetUserRole(opearteId);
    }

    public static DataTable GetPrint1(string iP)
    {
      return P00001_DataAccess.GetPrint1(iP);
    }

    public static DataTable GetPrint(string iP)
    {
      return P00001_DataAccess.GetPrint(iP);
    }

    public static DataTable ValidateOpr2(string inno)
    {
      return P00001_DataAccess.ValidateOpr2(inno);
    }

    public static DataTable ValidateOpr1(string inno)
    {
      return P00001_DataAccess.ValidateOpr1(inno);
    }

    public static DataTable ValidateUser(string opearteId)
    {
      return P00001_DataAccess.ValidateUser(opearteId);
    }

    public static DataTable GetPackQuantity(string inno)
    {
      return P00001_DataAccess.GetPackQuantity(inno);
    }

    public static DataTable GetInoutQuantity(string inno)
    {
      return P00001_DataAccess.GetInputQuantity(inno);
    }

    public static DataTable GetKanBanSum(string iP)
    {
      return P00001_DataAccess.GetKanBanSum(iP);
    }

    public static DataTable GetQuantity1(string inno)
    {
      return P00001_DataAccess.GetQuantity1(inno);
    }

    public static DataTable GetInfoData(string iP)
    {
      return P00001_DataAccess.GetInfoData(iP);
    }

    public static int DeleteTrolley(string trolley, string iP, string lotId)
    {
      return P00001_DataAccess.DeleteTrolley(trolley, iP, lotId);
    }

    public static DataTable GetQuantity(string partId, string scanTime, string dock)
    {
      return P00001_DataAccess.GetQuantity(partId, scanTime, dock);
    }

    public static DataTable GetSeqNo(string tmpString, string formatServerTime)
    {
      return P00001_DataAccess.GetSeqNo(tmpString, formatServerTime);
    }

    public static int InsertSeqNo(string tmpString, string formatServerTime)
    {
      return P00001_DataAccess.InsertSeqNo(tmpString, formatServerTime);
    }

    public static int InsertTrolley1(string seqNo, string trolley, string iP, string opearteId, string serverTime)
    {
      return P00001_DataAccess.InsertTrolley1(seqNo, trolley, iP, opearteId, serverTime);
    }

    public static int UpdateSeqNo(string tmpString, string formatServerTime, int seqNoNew)
    {
      return P00001_DataAccess.UpdateSeqNo(tmpString, formatServerTime, seqNoNew);

    }

    public static DataTable GetTrolley(string opearteId, string iP)
    {
      return P00001_DataAccess.GetTrolley(opearteId, iP);
    }

    public static DataTable GetDetail(string pointNo)
    {
      return P00001_DataAccess.GetDetail(pointNo);
    }

    public static DataTable GetTrolleyInfo(string trolley, string iP, string lotId)
    {
      return P00001_DataAccess.GetTrolleyInfo(trolley, iP, lotId);
    }

    public static int UpdateDetail(string uuid, string serverTime)
    {
      return P00001_DataAccess.UpdateDetail(uuid, serverTime);
    }

    public static int UpdateStatus5(string pointNo)
    {
      return P00001_DataAccess.UpdateStatus5(pointNo);
    }

    public static DataTable GetBanZhi(string serverTime)
    {
      return P00001_DataAccess.GetBanZhi(serverTime);
    }

    public static int UpdateStatus4(string pointNo, string opearteId)
    {
      return P00001_DataAccess.UpdateStatus4(pointNo, opearteId);
    }

    public static DataTable GetPointStatus3(string iP, string opearteId)
    {
      return P00001_DataAccess.GetPointStatus3(iP, opearteId);
    }

    public static DataTable GetTrolleyData(string iP)
    {
      return P00001_DataAccess.GetTrolleyData(iP);
    }

    public static int InsertDetail(string date, string banZhi, string pointNo, string uuid, string serverTime, string opearteId)
    {
      return P00001_DataAccess.InsertDetail(date, banZhi, pointNo, uuid, serverTime, opearteId);
    }

    public static int UpdateStatus3(string pointNo)
    {
      return P00001_DataAccess.UpdateStatus3(pointNo);
    }

    public static int UpdateStatus2(string pointNo, string iP, string opearteId)
    {
      return P00001_DataAccess.UpdateStatus2(pointNo, iP, opearteId);
    }

    public static DataTable GetPointStatus2(string opearteId, string iP)
    {
      return P00001_DataAccess.GetPointStatus2(opearteId, iP);
    }

    public static int UpdateStatus1(string pointNo, string opearteId)
    {
      return P00001_DataAccess.UpdateStatus1(pointNo, opearteId);
    }

    public static DataTable GetPointStatus1(string opearteId, string iP)
    {
      return P00001_DataAccess.GetPointStatus1(opearteId, iP);
    }

    public static DataTable GetTrolley(string iP)
    {
      return P00001_DataAccess.GetTrolley(iP);
    }

    public static int UpdateStatus(string opearteId, string iP, string pointNo)
    {
      return P00001_DataAccess.UpdateStatus(opearteId, iP, pointNo);
    }

    public static DataTable GetKanBan(string iP, string trolley, string trolleySeqNo)
    {
      return P00001_DataAccess.GetKanBan(iP, trolley, trolleySeqNo);
    }

    public static DataTable GetPackingSpot(string iP)
    {
      return P00001_DataAccess.GetPackingSpot(iP);
    }

    public static DataTable GetPoint(string iP)
    {
      return P00001_DataAccess.GetPoint(iP);
    }

    public static DataTable GetPointStatus(string pointNo)
    {
      return P00001_DataAccess.GetPointStatus(pointNo);
    }

    public static int InsertPoint(string pointNo)
    {
      return P00001_DataAccess.InsertPoint(pointNo);
    }

    public static int UpdatePoint(string pointNo)
    {
      return P00001_DataAccess.UpdatePoint(pointNo);
    }

    public static int DeleteKanban(string partId, string kanbanOrderNo, string kanbanSerial, string dock, string iP)
    {
      return P00001_DataAccess.DeleteKanban(partId, kanbanOrderNo, kanbanSerial, dock, iP);
    }

    public static DataTable ValidateOrd(string partId)
    {
      return P00001_DataAccess.ValidateOrd(partId);
    }


    public static DataTable ValidateSeqNo(string packingSpot, string serverTime, string tmpString)
    {
      return P00001_DataAccess.ValidateSeqNo(packingSpot, serverTime, tmpString);
    }





    public static int InsertSeqNo(string packingSpot, string serverTime, string tmpString)
    {
      return P00001_DataAccess.InsertSeqNo(packingSpot, serverTime, tmpString);
    }

    public static int UpdateSeqNo(string packingSpot, string serverTime, int seqNoNew, string tmpString)
    {
      return P00001_DataAccess.UpdateSeqNo(packingSpot, serverTime, seqNoNew, tmpString);
    }

    public static int UpdateCase(string iP)
    {
      return P00001_DataAccess.UpdateCase(iP);
    }

    public static int InsertInv(string packingSpot, string inno, string partId, string cpdCompany, string quantity, string serverTime, string kanbanOrderNo, string kanbanSerial, string scanTime, string opearteId)
    {
      return P00001_DataAccess.InsertInv(packingSpot, inno, partId, cpdCompany, quantity, serverTime, kanbanOrderNo, kanbanSerial, scanTime, opearteId);
    }

    public static DataTable GetPointDetails(string iP)
    {
      return P00001_DataAccess.GetPointDetails(iP);
    }

    public static int UpdateQB(string lotId, string iP, string trolley)
    {
      return P00001_DataAccess.UpdateQB(lotId, iP, trolley);
    }

    public static DataTable GetQBData(string iP)
    {
      return P00001_DataAccess.GetQBData(iP);
    }

    public static int InsertOpr(string packingSpot, string inno, string kanbanOrderNo, string kanbanSerial, string partId, string inoutFlag, string supplierCode, string supplierPlant, string scanTime, string serverTime, string quantity, int packingQuantity, string cpdCompany, string dock, string checkType, string lblSart, string lblEnd, string opearteId)
    {
      return P00001_DataAccess.InsertOpr(packingSpot, inno, kanbanOrderNo, kanbanSerial, partId, inoutFlag, supplierCode, supplierPlant, scanTime, serverTime, quantity, packingQuantity, cpdCompany, dock, checkType, lblSart, lblEnd, opearteId);
    }

    public static DataTable GetPoint1(string iP)
    {
      return P00001_DataAccess.GetPoint1(iP);
    }

    public static DataTable ValidateSJ(string partId, string kanbanOrderNo, string kanbanSerial, string inputNo)
    {
      return P00001_DataAccess.ValidateSJ(partId, kanbanOrderNo, kanbanSerial, inputNo);
    }

    public static DataTable ValidateOpr(string partId, string kanbanOrderNo, string kanbanSerial, string inputNo)
    {
      return P00001_DataAccess.ValidateOpr(partId, kanbanOrderNo, kanbanSerial, inputNo);
    }

    public static DataTable GetPackItem(string scanTime, string partId)
    {
      return P00001_DataAccess.GetPackItem(scanTime, partId);
    }

    public static DataTable GetPoint2(string iP)
    {
      return P00001_DataAccess.GetPoint2(iP);
    }

    public static DataTable ValiateOrd1(string partId)
    {
      return P00001_DataAccess.ValidateOrd1(partId);
    }

    public static DataTable GetPointStatus4(string pointNo)
    {
      return P00001_DataAccess.GetPointStatus4(pointNo);
    }

    public static int InsertPoint1(string pointNo)
    {
      return P00001_DataAccess.InsertPoint1(pointNo);

    }

    public static int UpdatePoint1(string pointNo)
    {
      return P00001_DataAccess.UpdatePoint1(pointNo);
    }

    public static DataTable GetInOutFlag(string partId, string scanTime)
    {
      return P00001_DataAccess.GetInOutFlag(partId, scanTime);
    }

    public static DataTable GetCheckType(string partId, string scanTime)
    {
      return P00001_DataAccess.GetCheckType(partId, scanTime);
    }

    public static int UpdateOpr(string partId, string dock, string kanbanOrderNo, string kanbanSerial)
    {
      return P00001_DataAccess.UpdateOpr(partId, dock, kanbanOrderNo, kanbanSerial);
    }

    public static int UpdateOrd(string targetMonth, string orderNo, string seqNo, int v1, int v2, int v3, int v4, int v5, int v6, int v7, int v8, int v9, int v10, int v11, int v12, int v13, int v14, int v15, int v16, int v17, int v18, int v19, int v20, int v21, int v22, int v23, int v24, int v25, int v26, int v27, int v28, int v29, int v30, int v31, int newSum, string partId)
    {
      return P00001_DataAccess.UpdateOrd(targetMonth, orderNo, seqNo, v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11, v12, v13, v14, v15, v16, v17, v18, v19, v20, v21, v22, v23, v24, v25, v26, v27, v28, v29, v30, v31, newSum, partId);
    }

    public static DataTable ValidateData1(string partId, string scanTime)
    {
      return P00001_DataAccess.ValidateData1(partId, scanTime);
    }



    public static DataTable GetCount(string partId)
    {
      return P00001_DataAccess.GetCount(partId);
    }

    public static DataTable GetPackBase(string scanTime, string packNo, string packingSpot)
    {
      return P00001_DataAccess.GetPackBase(scanTime, packNo, packingSpot);
    }

    public static DataTable GetPointType(string pointNo)
    {
      return P00001_DataAccess.GetPointType(pointNo);
    }

    public static DataTable ValidatePack(string lotId, string iP, string opearteId, string packNo)
    {
      return P00001_DataAccess.ValidatePack(lotId, iP, opearteId, packNo);
    }

    public static int InsertPack(string lotId, string packNo, string distinguish, string inputNo, double qty, string packLocation, string scanTime, string serverTime, string opearteId, string iP, string trolleyNo, string lblStart, string lblEnd)
    {
      return P00001_DataAccess.InsertPack(lotId, packNo, distinguish, inputNo, qty, packLocation, scanTime, serverTime, opearteId, iP, trolleyNo, lblStart, lblEnd);
    }
    /*
    public static int InsertLbl(string partsNameEn, string partId, string cpdCompany, string quantity, string printCount, string supplierName, string supplierAddress, string carFamilyCode, string opearteId, string scanTime, string iP,, string partsNameCn, string inputNo)
    {
      return P00001_DataAccess.InsertLbl(partsNameEn,partId,cpdCompany,quantity, printCount, supplierName,supplierAddress,carFamilyCode,opearteId,scanTime,iP,vs,partsNameCn,inputNo);
    }
    */
    public static int UpdatePack(string lotId, string packNo, string iP, string opearteId, double qty)
    {
      return P00001_DataAccess.UpdatePack(lotId, packNo, iP, opearteId, qty);
    }

    public static int InsertTP(string iP, string opearteId, string serverTime, string lotIdNew)
    {
      return P00001_DataAccess.InsertTP(iP, opearteId, serverTime, lotIdNew);
    }

    public static int InsertTP1(string iP, string opearteId, string serverTime, string printName)
    {
      return P00001_DataAccess.InsertTP1(iP, opearteId, serverTime, printName);
    }

    public static DataTable ValidatePrice(string partId, string scanTime)
    {
      return P00001_DataAccess.ValidatePrice(partId, scanTime);
    }

    public static DataTable ValidateQF(string partId, string scanTime)
    {
      return P00001_DataAccess.ValidateQF(partId, scanTime);
    }

    public static void UpdateOpr1(string kanbanOrderNo, string kanbanSerial, string partId, string dock)
    {
      P00001_DataAccess.UpdateOpr1(kanbanOrderNo, kanbanSerial, partId, dock);
    }

    public static DataTable GetPackBase(string iP)
    {
      return P00001_DataAccess.GetPackBase(iP);
    }

    public static DataTable ValidateSR(string partId, string scanTime)
    {
      return P00001_DataAccess.ValidateSR(partId, scanTime);
    }

    public static DataTable ValidateQB(string partId, string kanbanOrderNo, string kanbanSerial, string dock)
    {
      return P00001_DataAccess.ValidateQB(partId, kanbanOrderNo, kanbanSerial, dock);
    }

    public static DataTable GetTagInfo(string partId, string scanTime)
    {
      return P00001_DataAccess.GetTagInfo(partId, scanTime);
    }

    public static DataTable GetPackInfo(string packingQuantity, string partId, string scanTime, string quantity)
    {
      return P00001_DataAccess.GetPackInfo(packingQuantity, partId, scanTime, quantity);
    }

    public static DataTable GetInv()
    {
      return P00001_DataAccess.GetInv();
    }

    public static int InsertInvList(string data1, string printDate, string inputNo, string partId, string partsNameEn, string quantity, string packingQuantity, string itemname1, string packLocation1, string suppName1, string outNum1, string itemname2, string packLocation2, string suppName2, string outNum2, string itemname3, string packLocation3, string suppName3, string outNum3, string itemname4, string packLocation4, string suppName4, string outNum4, string itemname5, string packLocation5, string suppName5, string outNum5, string itemname6, string packLocation6, string suppName6, string outNum6, string itemname7, string packLocation7, string suppName7, string outNum7, string itemname8, string packLocation8, string suppName8, string outNum8, string itemname9, string packLocation9, string suppName9, string outNum9, string itemname10, string packLocation10, string suppName10, string outNum10, string partsAndNum, string cpdCompany, string opearteId, string scanTime, byte[] vs1)
    {
      return P00001_DataAccess.InsertInvList(data1, printDate, inputNo, partId, partsNameEn, quantity, packingQuantity, itemname1, packLocation1, suppName1, outNum1,
itemname2, packLocation2, suppName2, outNum2, itemname3, packLocation3, suppName3, outNum3, itemname4, packLocation4, suppName4, outNum4, itemname5, packLocation5, suppName5, outNum5,
itemname6, packLocation6, suppName6, outNum6, itemname7, packLocation7, suppName7, outNum7, itemname8, packLocation8, suppName8, outNum8, itemname9, packLocation9, suppName9, outNum9,
itemname10, packLocation10, suppName10, outNum10, partsAndNum, cpdCompany, opearteId, scanTime, vs1);
    }

    public static DataTable ValidateQB(string trolley)
    {
      return P00001_DataAccess.ValidateQB(trolley);
    }

    public static int UpdateQB(string iP)
    {
      return P00001_DataAccess.UpdateQB(iP);
    }

    public static int InsertLbl(string partsNameEn, string partId, string cpdCompany, string quantity, string printCount, string supplierName, string supplierAddress, string carFamilyCode, string opearteId, string scanTime, string iP, string partsNameCn, string inputNo, byte[] vs, string printCount1, byte[] vs2, string excuteStand, string packingQuatity, string partId1)
    {
      return P00001_DataAccess.InsertLbl(partsNameEn, partId, cpdCompany, quantity, printCount, supplierName, supplierAddress, carFamilyCode, opearteId, scanTime, iP, partsNameCn, inputNo, vs, printCount1, vs2, excuteStand, packingQuatity, partId1);
    }



    public static int UpdatePack1(string iP, string serverTime)
    {
      return P00001_DataAccess.UpdatePack1(iP, serverTime);
    }

    public static int UpdatePrint(string iP)
    {
      return P00001_DataAccess.UpdatePrint(iP);
    }

    public static int UpdatePrint1(string iP)
    {
      return P00001_DataAccess.UpdatePrint1(iP);
    }

    public static int UpdateLabel(string iP, string serverTime)
    {
      return P00001_DataAccess.UpdateLabel(iP, serverTime);
    }

    public static DataTable ValidateTrolley(string trolley, string opearteId, string iP)
    {
      return P00001_DataAccess.ValidateTrolley(trolley, opearteId, iP);
    }

    public static int InsertTrolley(string trolley, string opearteId, string serverTime, string iP, string lotId)
    {
      return P00001_DataAccess.InsertTrolley(trolley, opearteId, serverTime, iP, lotId);
    }

    public static int UpdateTrolley(string trolley, string opearteId, string serverTime, string iP, string lotId)
    {
      return P00001_DataAccess.UpdateTrolley(trolley, opearteId, serverTime, iP, lotId);
    }

    public static int UpdateTrolley(string opearteId, string iP)
    {
      return P00001_DataAccess.UpdateTrolley(opearteId, iP);
    }

    public static DataTable GetLabel(string iP, string lotIdNew)
    {
      return P00001_DataAccess.GetLabel(iP, lotIdNew);
    }

    public static int UpdatePack(string iP, string minLabel, string maxLabel, string lotIdNew)
    {
      return P00001_DataAccess.UpdatePack(iP, minLabel, maxLabel, lotIdNew);
    }

    public static int UpdateLabel1(string iP, string serverTime)
    {
      return P00001_DataAccess.UpdateLabel1(iP, serverTime);
    }

    public static int UpdateTrolley1(string iP, string opearteId, string trolley, string lotId)
    {
      return P00001_DataAccess.UpdateTrolley1(iP, opearteId, trolley, lotId);
    }

    public static DataTable ValidateQB5(string lotIdNew)
    {
      return P00001_DataAccess.ValidateQB5(lotIdNew);
    }



    public static DataTable GetTrolleyInfo1(string trolley, string iP, string opearteId)
    {
      return P00001_DataAccess.GetTrolleyInfo1(trolley, iP, opearteId);
    }

    public static void SaveXml(P00001_DataEntity.ScanData sData, string serverTime, string name, string formatTime)
    {
      #region 写入Y

      string sFilePathY = @"Y:\" + name + "_Y_" + formatTime + ".xml";
      string s = "";
      if (File.Exists(sFilePathY))
      {
        StreamReader sr = File.OpenText(sFilePathY);
        s = sr.ReadToEnd();
        s = s.Replace("</Detils>", "");
        s = s.Replace("<?xml version=\"1.0\"?>\r\n<Detils>\r\n", "");
        sr.Close();
      }

      System.Xml.XmlDocument doc = new XmlDocument();
      XmlElement root = doc.CreateElement("Books");
      doc.AppendChild(root);
      XmlElement nodes = doc.CreateElement("Infor");


      XmlElement x1 = doc.CreateElement("SUPPLIER_CODE");
      x1.InnerText = sData.SUPPLIER_CODE.Trim();
      nodes.AppendChild(x1);

      XmlElement x2 = doc.CreateElement("SUPPLIER_PLANT");
      x2.InnerText = sData.SUPPLIER_PLANT.Trim();
      nodes.AppendChild(x2);

      XmlElement x3 = doc.CreateElement("SHIPPING_DOCK");
      x3.InnerText = sData.SHIPPING_DOCK.Trim();
      nodes.AppendChild(x3);

      XmlElement x4 = doc.CreateElement("SR_GRP_CODE");
      x4.InnerText = "";
      nodes.AppendChild(x4);

      XmlElement x5 = doc.CreateElement("INVOICE_NO");
      x5.InnerText = sData.INVOICE_NO.Trim();
      nodes.AppendChild(x5);

      XmlElement x6 = doc.CreateElement("ORDER_NO");
      x6.InnerText = sData.ORDER_NO.Trim();
      nodes.AppendChild(x6);

      XmlElement x7 = doc.CreateElement("KNBN_PRN_ADDRESS");
      x7.InnerText = sData.KNBN_PRN_ADDRESS.Trim();
      nodes.AppendChild(x7);

      XmlElement x8 = doc.CreateElement("KNBN_NO");
      x8.InnerText = sData.KNBN_NO.Trim();
      nodes.AppendChild(x8);

      XmlElement x9 = doc.CreateElement("PART_NO");
      x9.InnerText = sData.PART_NO.Trim();
      nodes.AppendChild(x9);

      XmlElement x10 = doc.CreateElement("DOCK_CODE");
      x10.InnerText = sData.DOCK_CODE.Trim();
      nodes.AppendChild(x10);

      XmlElement x11 = doc.CreateElement("SERIAL_NO");
      x11.InnerText = sData.SERIAL_NO.Trim();
      nodes.AppendChild(x11);

      XmlElement x12 = doc.CreateElement("DOCK_ARRIALTIME");
      x12.InnerText = "";
      nodes.AppendChild(x12);

      XmlElement x13 = doc.CreateElement("P_TIME");
      x13.InnerText = "";
      nodes.AppendChild(x13);

      XmlElement x14 = doc.CreateElement("SCAN_TIME");
      x14.InnerText = serverTime;
      nodes.AppendChild(x14);

      XmlElement x15 = doc.CreateElement("SCAN_FLG");
      x15.InnerText = "1";
      nodes.AppendChild(x15);

      XmlElement x16 = doc.CreateElement("SCAN_USER");
      x16.InnerText = "buji";
      nodes.AppendChild(x16);

      //2017-04-20 增加扫描点位 李志远 start
      XmlElement x17 = doc.CreateElement("SCAN_AREA");
      x17.InnerText = "buji";
      nodes.AppendChild(x17);

      //end

      root.AppendChild(nodes);
      s += doc.ChildNodes[0].InnerXml;

      s = "<?xml version=\"1.0\"?>\r\n<Detils>\r\n" + s + "</Detils>";

      byte[] b = UTF8Encoding.UTF8.GetBytes(s);
      FileStream fs = new FileStream(sFilePathY, FileMode.OpenOrCreate, FileAccess.ReadWrite);
      try
      {
        fs.Write(b, 0, b.Length);
      }
      catch (Exception ex)
      {
        throw ex;
      }
      finally
      {
        fs.Close();
      }
      #endregion
    }



    public static byte[] GenerateQRCode(string content)
    {
      var generator = new QRCodeGenerator();
      var codeData = generator.CreateQrCode(content, QRCodeGenerator.ECCLevel.M, true);

      QRCoder.QRCode qrcode = new QRCoder.QRCode(codeData);
      var bitmapImg = qrcode.GetGraphic(10, Color.Black, Color.White, false);
      using MemoryStream stream = new MemoryStream();
      bitmapImg.Save(stream, ImageFormat.Jpeg);
      return stream.GetBuffer();
    }

    public static P00001_DataEntity.ScanData CutScanData(string kanBan)
    {

      DataEntity.P00001_DataEntity.ScanData scanData = new P00001_DataEntity.ScanData();
      scanData.SUPPLIER_CODE = kanBan.Substring(1, 4);       //厂家
      scanData.SUPPLIER_PLANT = kanBan.Substring(5, 1);      //工区
      scanData.SHIPPING_DOCK = kanBan.Substring(6, 3);       //出荷厂
      scanData.INVOICE_NO = kanBan.Substring(61, 20).Trim(); //纳受领书号
      scanData.ORDER_NO = kanBan.Substring(219, 12);         //订单号
      scanData.KNBN_PRN_ADDRESS = kanBan.Substring(100, 10);  //所番地
      scanData.PART_NO = kanBan.Substring(16, 12);           //品番
      scanData.DOCK_CODE = kanBan.Substring(14, 2);          //受入
      scanData.SERIAL_NO = Convert.ToInt32(kanBan.Substring(57, 4)).ToString();          //顺番号
      scanData.KNBN_NO = kanBan.Substring(53, 4).Trim();     //背番
      scanData.PLANE_NO = kanBan.Substring(kanBan.Length - 2, 2).Trim();    //链号
      return scanData;

    }

    public static DataTable GetPointNo(string iP)
    {
      return P00001_DataAccess.GetPointNo(iP);
    }

    public static DataTable ValidatePrint(string iP)
    {
      return P00001_DataAccess.ValidatePrint(iP);
    }

    public static DataTable GetLotInfo(string iP)
    {
      return P00001_DataAccess.GetLotInfo(iP);
    }

    public static int UpdateTrolley2(string trolleySeqNo, string lotId, string iP)
    {
      return P00001_DataAccess.UpdateTrolley2(trolleySeqNo, lotId, iP);
    }

    public static DataTable GetID(string iP, string serverTime)
    {
      return P00001_DataAccess.GetID(iP, serverTime);
    }

    public static int UpdateLabel2(string lblSart, string lblEnd, string iP, string partId, string kanbanOrderNo, string kanbanSerial, string dock)
    {
      return P00001_DataAccess.UpdateLabel2(lblSart, lblEnd, iP, partId, kanbanOrderNo, kanbanSerial, dock);
    }

    public static DataTable GetSeqNo1(string iP, string kanbanOrderNo, string kanbanSerial, string dock, string partId)
    {
      return P00001_DataAccess.GetSeqNoSql(iP, kanbanOrderNo, kanbanSerial, dock, partId);

    }

    public static DataTable GetQB(string trolleySeqNo, string iP, string trolley)
    {
      return P00001_DataAccess.GetQB(trolleySeqNo, iP, trolley);
    }

    public static DataTable GetSeqNo2(string iP, string kanbanOrderNo, string kanbanSerial, string dock, string partId)
    {
      return P00001_DataAccess.GetSeqNo2(iP, kanbanOrderNo, kanbanSerial, dock, partId);
    }

    public static DataTable GetData1(string trolley1, string trolleySeqNo1, string iP)
    {
      return P00001_DataAccess.GetData1(trolley1, trolleySeqNo1, iP);
    }

    public static DataTable GetData(string trolley1, string trolleySeqNo1, string iP)
    {
      return P00001_DataAccess.GetData(trolley1, trolleySeqNo1, iP);
    }

    public static int UpdateTrolley3(string trolley, string trolleySeqNo, string iP)
    {
      return P00001_DataAccess.UpdateTrolley3(trolley, trolleySeqNo, iP);
    }

    public static DataTable GetInv(string iP)
    {
      return P00001_DataAccess.GetInv(iP);
    }

    public static int UpdateInv(string partId, string kanbanOrderNo, string kanbanSerial, string dock, string inno, string iP)
    {
      return P00001_DataAccess.UpdateInv(partId, kanbanOrderNo, kanbanSerial, dock, inno, iP);
    }

    public static DataTable GetPrintName(string iP)
    {
      return P00001_DataAccess.GetPrintName(iP);
    }
  }
}
