using System;
using System.Data;
using DataAccess;
using System.Drawing;
using System.IO;
using QRCoder;
using System.Drawing.Imaging;

namespace Logic
{
    public class P00001_Logic
    {
        static P00001_DataAccess P00001_DataAccess = new P00001_DataAccess();

        public static DataTable GetSum(string iP)
        {
            // return P00001_DataAccess.GetSum(iP);
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

        public static int UpdateSeqNo(string packingSpot, string serverTime, int seqNoNew, string tmpString)
        {
            return P00001_DataAccess.UpdateSeqNo(packingSpot, serverTime, seqNoNew, tmpString);
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

        public static int InsertPoint1(string pointNo)
        {
            return P00001_DataAccess.InsertPoint1(pointNo);

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
        public static void Insert(string trolley, string partId, string quantity, string dock, string kanbanOrderNo, string kanbanSerial, string scanTime, String iP, string serverTime, string cpdCompany, string inno, string opearteId, string packingSpot, string packQuantity, string lblSart, string lblEnd, string supplierId, string supplierPlant, string trolleySeqNo, string inoutFlag, string kanBan,string orderplant)
        {
            P00001_DataAccess.Insert(trolley, partId, quantity, dock, kanbanOrderNo, kanbanSerial, scanTime, iP, serverTime, cpdCompany, inno, opearteId, packingSpot, packQuantity, lblSart, lblEnd, supplierId, supplierPlant, trolleySeqNo, inoutFlag, kanBan, orderplant);
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
        public static bool setInputInfo(string strIP, string strPointName, string strPrinterName, DataTable dtPackList_Temp, DataTable dtLabelList_Temp, DataTable dtInv_Temp, DataTable dtOrder_Temp, string strOperId)
        {
            return P00001_DataAccess.setInputInfo(strIP, strPointName, strPrinterName, dtPackList_Temp, dtLabelList_Temp, dtInv_Temp, dtOrder_Temp, strOperId);
        }
        public static DataTable GetPrintName(string iP)
        {
            return P00001_DataAccess.GetPrintName(iP);
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
    }
}
