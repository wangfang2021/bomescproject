using System;
using System.Data;
using DataAccess;
using System.Drawing;
using System.IO;
using QRCoder;
using System.Drawing.Imaging;
using DataEntity;
using System.Xml;
using System.Text;

namespace Logic
{
    public class P00001_Logic
    {
        static P00001_DataAccess P00001_DataAccess = new P00001_DataAccess();

        public static DataTable GetSum(string iP)
        {

            return P00001_DataAccess.GetSum(iP);
        }

        public static DataTable GetUserRole(string opearteId)
        {
            return P00001_DataAccess.GetUserRole(opearteId);
        }


        public static DataTable ValidateOpr2(string inno)
        {
            return P00001_DataAccess.ValidateOpr2(inno);
        }

        public static DataTable ValidateOpr1(string inno)
        {
            return P00001_DataAccess.ValidateOpr1(inno);
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


        public static DataTable GetInfoData(string iP)
        {
            return P00001_DataAccess.GetInfoData(iP);
        }

        public static void DeleteTrolley(string trolley, string iP, string lotId)
        {
            P00001_DataAccess.DeleteTrolley(trolley, iP, lotId);
        }

        public static DataTable GetQuantity(string partId, string scanTime, string dock)
        {
            return P00001_DataAccess.GetQuantity(partId, scanTime, dock);
        }



        public static void InsertTrolley1(string seqNo, string trolley, string iP, string opearteId, string serverTime)
        {
            P00001_DataAccess.InsertTrolley1(seqNo, trolley, iP, opearteId, serverTime);
        }



        public static DataTable GetTrolley(string opearteId, string iP)
        {
            return P00001_DataAccess.GetTrolley(opearteId, iP);
        }



        public static DataTable GetTrolleyInfo(string trolley, string iP, string lotId)
        {
            return P00001_DataAccess.GetTrolleyInfo(trolley, iP, lotId);
        }



        public static DataTable GetBanZhi(string serverTime)
        {
            return P00001_DataAccess.GetBanZhi(serverTime);
        }

        public static void UpdateStatus4(string pointNo, string opearteId)
        {
            P00001_DataAccess.UpdateStatus4(pointNo, opearteId);
        }


        public static void InsertDetail(string date, string banZhi, string pointNo, string uuid, string serverTime, string opearteId)
        {
            P00001_DataAccess.InsertDetail(date, banZhi, pointNo, uuid, serverTime, opearteId);
        }

        public static void UpdateStatus3(string pointNo)
        {
            P00001_DataAccess.UpdateStatus3(pointNo);
        }


        public static DataTable GetPointStatus1(string opearteId, string iP)
        {
            return P00001_DataAccess.GetPointStatus1(opearteId, iP);
        }

        public static DataTable GetTrolley(string iP)
        {
            return P00001_DataAccess.GetTrolley(iP);
        }

        public static void UpdateStatus(string opearteId, string iP, string pointNo)
        {
            P00001_DataAccess.UpdateStatus(opearteId, iP, pointNo);
        }

        public static DataTable GetKanBan(string iP, string trolley, string trolleySeqNo)
        {
            return P00001_DataAccess.GetKanBan(iP, trolley, trolleySeqNo);
        }

        public static DataTable GetPackingSpot(string iP)
        {
            return P00001_DataAccess.GetPackingSpot(iP);
        }


        public static void DeleteKanban(string partId, string kanbanOrderNo, string kanbanSerial, string dock, string iP)
        {
            P00001_DataAccess.DeleteKanban(partId, kanbanOrderNo, kanbanSerial, dock, iP);
        }



        public static DataTable ValidateSeqNo(string packingSpot, string serverTime, string tmpString)
        {
            return P00001_DataAccess.ValidateSeqNo(packingSpot, serverTime, tmpString);
        }





        public static void InsertSeqNo(string packingSpot, string serverTime, string tmpString)
        {
            P00001_DataAccess.InsertSeqNo(packingSpot, serverTime, tmpString);
        }

        public static void UpdateSeqNo(string packingSpot, string serverTime, int seqNoNew, string tmpString)
        {
            P00001_DataAccess.UpdateSeqNo(packingSpot, serverTime, seqNoNew, tmpString);
        }




        public static void UpdateQB(string lotId, string iP, string trolley)
        {
            P00001_DataAccess.UpdateQB(lotId, iP, trolley);
        }


        public static DataTable GetPoint2(string iP)
        {
            return P00001_DataAccess.GetPoint2(iP);
        }


        public static DataTable GetPointStatus4(string pointNo)
        {
            return P00001_DataAccess.GetPointStatus4(pointNo);
        }

        public static void InsertPoint1(string pointNo)
        {
            P00001_DataAccess.InsertPoint1(pointNo);

        }

        public static void UpdatePoint1(string pointNo)
        {
            P00001_DataAccess.UpdatePoint1(pointNo);
        }




        public static DataTable ValidateQB(string trolley)
        {
            return P00001_DataAccess.ValidateQB(trolley);
        }


        public static DataTable ValidateTrolley(string trolley, string opearteId, string iP)
        {
            return P00001_DataAccess.ValidateTrolley(trolley, opearteId, iP);
        }

        public static void InsertTrolley(string trolley, string opearteId, string serverTime, string iP, string lotId)
        {
            P00001_DataAccess.InsertTrolley(trolley, opearteId, serverTime, iP, lotId);
        }

        public static void UpdateTrolley(string trolley, string opearteId, string serverTime, string iP, string lotId)
        {
            P00001_DataAccess.UpdateTrolley(trolley, opearteId, serverTime, iP, lotId);
        }


        public static DataTable GetCase(string opearteId, string iP)
        {
            return P00001_DataAccess.GetCase(opearteId, iP);
        }


        public static DataTable GetCase1(string caseNo)
        {
            return P00001_DataAccess.GetCase1(caseNo);
        }

        public static void UpdateCase(string iP, string serverTime, string opearteId, string caseNo)
        {
            P00001_DataAccess.UpdateCase(iP, serverTime, opearteId, caseNo);
        }


        public static void UpdateTrolley1(string iP, string opearteId, string trolley, string lotId)
        {
            P00001_DataAccess.UpdateTrolley1(iP, opearteId, trolley, lotId);
        }




        public static DataTable GetTrolleyInfo1(string trolley, string iP, string opearteId)
        {
            return P00001_DataAccess.GetTrolleyInfo1(trolley, iP, opearteId);
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







        public static DataTable GetSeqNo1(string iP, string kanbanOrderNo, string kanbanSerial, string dock, string partId)
        {
            return P00001_DataAccess.GetSeqNoSql(iP, kanbanOrderNo, kanbanSerial, dock, partId);

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

        public static void UpdateTrolley3(string trolley, string trolleySeqNo, string iP)
        {
            P00001_DataAccess.UpdateTrolley3(trolley, trolleySeqNo, iP);
        }

        //========================================================================重写========================================================================
        public static DataTable GetPointNo(string iP)
        {
            return P00001_DataAccess.GetPointNo(iP);
        }
        public static DataTable checkPrintName(string iP, string strPointType)
        {
            return P00001_DataAccess.checkPrintName(iP, strPointType);
        }
        public static void setCaseState(string strIP)
        {
            P00001_DataAccess.setCaseState(strIP);
        }
        public static DataTable GetPointState(string strOperater)
        {
            return P00001_DataAccess.GetPointState(strOperater);
        }
        public static void setSysExit(string strIP, string strType)
        {
            try
            {
                P00001_DataAccess.setSysExit(strIP, strType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setAppHide(string strIP, string strPage)
        {
            try
            {
                P00001_DataAccess.setAppHide(strIP, strPage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static DataSet getCheckQBandSJInfo(string partId, string kanbanOrderNo, string kanbanSerial, string dock, string packingSpot, string scanTime, string strType)
        {
            return P00001_DataAccess.getCheckQBandSJInfo(partId, kanbanOrderNo, kanbanSerial, dock, packingSpot, scanTime, strType);
        }
        public static void Insert(string trolley, string partId, string quantity, string dock, string kanbanOrderNo, string kanbanSerial, string scanTime, String iP, string serverTime, string cpdCompany, string inno, string opearteId, string packingSpot, string packQuantity, string lblSart, string lblEnd, string supplierId, string supplierPlant, string trolleySeqNo, string inoutFlag, string kanBan, string orderplant)
        {
            P00001_DataAccess.Insert(trolley, partId, quantity, dock, kanbanOrderNo, kanbanSerial, scanTime, iP, serverTime, cpdCompany, inno, opearteId, packingSpot, packQuantity, lblSart, lblEnd, supplierId, supplierPlant, trolleySeqNo, inoutFlag, kanBan, orderplant);
        }
        public static void delInputInfoQB(string strIP, string serverTime)
        {
            try
            {
                P00001_DataAccess.delInputInfoQB(strIP, serverTime);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static DataSet getInputInfoFromDB(string strIP, string serverTime)
        {
            return P00001_DataAccess.getInputInfoFromDB(strIP, serverTime);
        }
        public static DataTable getPackInfo(string strIP, string serverTime)
        {
            return P00001_DataAccess.getPackInfo(strIP, serverTime);
        }
        public static DataTable getLabelInfo(string strIP, string serverTime)
        {
            return P00001_DataAccess.getLabelInfo(strIP, serverTime);
        }
        public static DataTable getOrderInfo(string strIP, string serverTime)
        {
            return P00001_DataAccess.getOrderInfo(strIP, serverTime);
        }
        public static DataTable getInvInfo(string strIP, string serverTime)
        {
            return P00001_DataAccess.getInvInfo(strIP, serverTime);
        }
        public static DataTable setSeqNo(string tmpString, int iAddNum, string formatServerTime)
        {
            return P00001_DataAccess.setSeqNo(tmpString, iAddNum, formatServerTime);
        }
        public static void setLotToOperatorQB(DataTable dtInfo, string strIP, string formatServerTime, string strLotSeqNo_begin)
        {
            P00001_DataAccess.setLotToOperatorQB(dtInfo, strIP, formatServerTime, strLotSeqNo_begin);
        }
        public static void setTagToOperatorQB(DataTable dtInfo, string strIP, string serverTime, string strTagSeqNo_begin)
        {
            P00001_DataAccess.setTagToOperatorQB(dtInfo, strIP, serverTime, strTagSeqNo_begin);
        }

        public static void setInvToOperatorQB(DataTable dataTable, string iP, string serverTime, string invSeqNo)
        {
            P00001_DataAccess.setInvToOperatorQB(dataTable, iP, serverTime, invSeqNo);
        }
        public static bool setInputInfo(string strIP, string strPointName, string strPrinterName, DataTable dtPackList_Temp, DataTable dtLabelList_Temp, DataTable dtInv_Temp, DataTable dtOrder_Temp, DataTable dtORD_INOUT_Temp, string strOperId, string strPackPrinterName)
        {
            return P00001_DataAccess.setInputInfo(strIP, strPointName, strPrinterName, dtPackList_Temp, dtLabelList_Temp, dtInv_Temp, dtOrder_Temp, dtORD_INOUT_Temp, strOperId, strPackPrinterName);
        }
        public static DataTable GetPrintName(string iP, string strKind)
        {
            return P00001_DataAccess.GetPrintName(iP, strKind);
        }
        public bool checkPointState(string strOperater, string strPlant, string strIP)
        {
            try
            {
                DataTable dataTable = P00001_DataAccess.checkPointState(strOperater, strPlant, strIP);
                if (dataTable.Rows.Count == 0)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getPointState_Site(string strOperater, string strPlant, string strIP)
        {
            try
            {
                return P00001_DataAccess.getPointState_Site(strOperater, strPlant, strIP);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setPointState_Site(string strOperater, string strPlant, string strIP, string strSiteType, string strOperType)
        {
            try
            {
                P00001_DataAccess.setPointState_Site(strOperater, strPlant, strIP, strSiteType, strOperType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
        public static void SaveXml(P00001_DataEntity.ScanData sData, string serverTime, string name, string formatTime, string strPlant)
        {
            #region 写入Y
            string sFilePathY = "";
            if (strPlant == "1")
            {
                sFilePathY = @"A:\" + name + "_Y_" + formatTime + ".xml";
            }
            if (strPlant == "2")
            {
                sFilePathY = @"B:\" + name + "_Y_" + formatTime + ".xml";
            }
            if (strPlant == "3")
            {
                sFilePathY = @"E:\" + name + "_Y_" + formatTime + ".xml";
            }
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

            //XmlElement x12 = doc.CreateElement("DOCK_ARRIALTIME");
            XmlElement x12 = doc.CreateElement("DOCK_ARRIVAL_DATE");
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
            x17.InnerText = "020";
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


        public string ChangeBarCode(string strPartsNo)
        {
            string strBarCode = "";
            try
            {
                int lngBarCodeCount = 0;
                int lngAscCode = 0;
                if (strPartsNo.Substring(10, 2) == "00")
                    strPartsNo = strPartsNo.Substring(0, 10) + "  ";
                int PLen = strPartsNo.Length;
                for (int i = 0; i < PLen; i++)
                {
                    char asc = char.Parse(strPartsNo.Substring(i, 1));
                    lngAscCode = (int)asc;
                    if (lngAscCode != 32)
                    {
                        if (lngAscCode < 65)
                        {
                            lngBarCodeCount = lngBarCodeCount + (lngAscCode - 48);
                        }
                        else
                        {
                            lngBarCodeCount = lngBarCodeCount + (lngAscCode - 55);
                        }
                    }
                    else
                    {
                        lngBarCodeCount = lngBarCodeCount + 38;
                    }
                }
                lngAscCode = lngBarCodeCount % 43;
                if (lngAscCode < 10)
                {
                    strBarCode = Convert.ToChar(lngAscCode + 48).ToString();
                }
                else if (lngAscCode > 9 && lngAscCode < 36)
                {
                    strBarCode = Convert.ToChar(lngAscCode + 55).ToString();
                }
                else
                {
                    switch (lngAscCode)
                    {
                        case 36:
                            strBarCode = "-";
                            break;
                        case 37:
                            strBarCode = ".";
                            break;
                        case 38:
                            strBarCode = " ";
                            break;
                        case 39:
                            strBarCode = "$";
                            break;
                        case 40:
                            strBarCode = "/";
                            break;
                        case 41:
                            strBarCode = "+";
                            break;
                        case 42:
                            strBarCode = "%";
                            break;
                        default:
                            break;

                    }
                }
                strBarCode = strPartsNo + strBarCode;
                return "*" + strBarCode + "*";
            }
            catch (Exception ex)
            {
                strBarCode = strPartsNo + strBarCode;
                return "*" + strBarCode + "*";
            }
        }

    public static void SaveXmlLocal(P00001_DataEntity.ScanData sData, string serverTime, string name, string formatTime, string strPlant)
    {
      #region 写入Y
      string sFilePathY = "";
      if (strPlant == "1")
      {
        sFilePathY = @"G:\Api\Debug\netcoreapp3.1\Doc\TILSXML\1\" + name + "_Y_" + formatTime + ".xml";

      }
      if (strPlant == "2")
      {
        sFilePathY = @"G:\Api\Debug\netcoreapp3.1\Doc\TILSXML\2\" + name + "_Y_" + formatTime + ".xml";
      }
      if (strPlant == "3")
      {
        sFilePathY = @"G:\Api\Debug\netcoreapp3.1\Doc\TILSXML\3\" + name + "_Y_" + formatTime + ".xml";
      }
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

      //XmlElement x12 = doc.CreateElement("DOCK_ARRIALTIME");
      XmlElement x12 = doc.CreateElement("DOCK_ARRIVAL_DATE");
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
      x17.InnerText = "020";
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
  }
}
