using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;
using System.IO;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;

namespace Logic
{
    public class FS0617_Logic
    {
        FS0603_DataAccess fs0603_DataAccess;
        FS0617_DataAccess fs0617_DataAccess;
        FS0603_Logic fS0603_Logic = new FS0603_Logic();

        public FS0617_Logic()
        {
            fs0603_DataAccess = new FS0603_DataAccess();
            fs0617_DataAccess = new FS0617_DataAccess();
        }
        public DataTable getSearchInfo(string strOrderPlant, string strPartId, string strCarModel, string strReceiver, string strSupplier)
        {
            return fs0617_DataAccess.getSearchInfo(strOrderPlant, strPartId, strCarModel, strReceiver, strSupplier);
        }
        public void getPrintInfo(List<Dictionary<string, Object>> listInfoData, string imagefile_sp, string imagefile_qr, string strOperId, ref DataTable dtMessage)
        {
            try
            {
                DataTable dtSPInfo = fs0603_DataAccess.getSearchInfo("", "", "", "", "", "", "0", "", "", "", "", "", "", "", "", "", "", "", "", false, "");
                DataTable dataTable = fs0617_DataAccess.getPrintTemp("FS0617");
                DataTable dtSub = dataTable.Clone();
                DataTable dtMain = fS0603_Logic.createTable("mainFs0617");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    string strEnum = listInfoData[i]["iEnum"] == null ? "0" : listInfoData[i]["iEnum"].ToString();
                    string strLind = listInfoData[i]["LinId"] == null ? "" : listInfoData[i]["LinId"].ToString();
                    DataRow[] drSPInfo = dtSPInfo.Select("LinId='" + strLind + "'");
                    for (int e = 1; e <= Convert.ToInt32(strEnum); e++)
                    {
                        if (drSPInfo.Length != 0)
                        {
                            string uuid = Guid.NewGuid().ToString("N");
                            DataRow dataRow = dtSub.NewRow();
                            dataRow["UUID"] = uuid;
                            dataRow["vcSupplierId"] = drSPInfo[0]["vcSupplierId"].ToString();
                            dataRow["vcReceiver"] = drSPInfo[0]["vcReceiver"].ToString();
                            dataRow["vcCarfamilyCode"] = drSPInfo[0]["vcCarfamilyCode"].ToString();
                            dataRow["vcBF"] = "";
                            dataRow["vcPartId"] = drSPInfo[0]["vcPartId"].ToString();
                            dataRow["vcPartENName"] = drSPInfo[0]["vcPartENName"].ToString();
                            dataRow["iPackingQty"] = Convert.ToInt32(drSPInfo[0]["iPackingQty"].ToString() == "" ? "0" : drSPInfo[0]["iPackingQty"].ToString()).ToString();
                            dataRow["vcSufferIn"] = drSPInfo[0]["vcSufferIn"].ToString();
                            dataRow["vcInteriorProject"] = drSPInfo[0]["vcInteriorProject"].ToString();
                            if (drSPInfo[0]["dFrontProjectTime"].ToString() != "")
                            {
                                dataRow["dFrontProjectTime"] = drSPInfo[0]["dFrontProjectTime"].ToString();
                            }
                            if (drSPInfo[0]["dShipmentTime"].ToString() != "")
                            {
                                dataRow["dShipmentTime"] = drSPInfo[0]["dShipmentTime"].ToString();
                            }
                            dataRow["vcRemark1"] = drSPInfo[0]["vcRemark1"].ToString();
                            dataRow["vcRemark2"] = drSPInfo[0]["vcRemark2"].ToString();
                            string strKanBanNo = drSPInfo[0]["vcPartId"].ToString() + (100000 + Convert.ToInt32(drSPInfo[0]["iPackingQty"].ToString() == "" ? "0" : drSPInfo[0]["iPackingQty"].ToString())).ToString().Substring(1, 5) + drSPInfo[0]["vcSufferIn"].ToString();
                            dataRow["vcKanBanNo"] = strKanBanNo;
                            //byte[] vcPhotoPath = null;//图片二进制流
                            byte[] vcPhotoPath = PhotoToArray(imagefile_sp + drSPInfo[0]["vcPartImage"].ToString(), imagefile_sp + "null.jpg");//图片二进制流
                            dataRow["vcPartImage"] = vcPhotoPath;
                            string strPath = imagefile_qr + strOperId + "_" + DateTime.Now.ToString("yyyyMMddhhmmss") + "_" + uuid.Replace("-", "") + ".png";
                            byte[] vcCodemage = GenerateQRCode(strKanBanNo);//二维码信息
                            dataRow["vcCodemage"] = vcCodemage;
                            dtSub.Rows.Add(dataRow);
                        }
                    }
                }
                for (int i = 0; i < dtSub.Rows.Count; i = i + 3)
                {
                    string UUID1 = "";
                    string UUID2 = "";
                    string UUID3 = "";
                    if (i < dtSub.Rows.Count)
                    {
                        if (i + 1 < dtSub.Rows.Count)
                        {
                            if (i + 2 < dtSub.Rows.Count)
                            {
                                UUID1 = dtSub.Rows[i]["UUID"].ToString();
                                UUID2 = dtSub.Rows[i + 1]["UUID"].ToString();
                                UUID3 = dtSub.Rows[i + 2]["UUID"].ToString();
                            }
                            else
                            {
                                UUID1 = dtSub.Rows[i]["UUID"].ToString();
                                UUID2 = dtSub.Rows[i + 1]["UUID"].ToString();
                            }
                        }
                        else
                        {
                            UUID1 = dtSub.Rows[i]["UUID"].ToString();
                        }
                    }
                    DataRow dataRow = dtMain.NewRow();
                    dataRow["UUID1"] = UUID1;
                    dataRow["UUID2"] = UUID2;
                    dataRow["UUID3"] = UUID3;
                    dtMain.Rows.Add(dataRow);
                }
                if (dtMain.Rows.Count == 0 || dtSub.Rows.Count == 0)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "没有有效的看板数据，请确认后再操作。";
                    dtMessage.Rows.Add(dataRow);
                }
                for (int i = 0; i < dtSub.Rows.Count; i++)
                {
                    //string vcSupplierId = dtSub.Rows[i]["vcSupplierId"].ToString();
                    //string vcReceiver = dtSub.Rows[i]["vcReceiver"].ToString();
                    string vcCarfamilyCode = dtSub.Rows[i]["vcCarfamilyCode"].ToString();
                    string strPartId = dtSub.Rows[i]["vcPartId"].ToString();
                    //string vcPartENName = dtSub.Rows[i]["vcPartENName"].ToString();
                    string strPackingQty = dtSub.Rows[i]["iPackingQty"].ToString();
                    string strSufferIn = dtSub.Rows[i]["vcSufferIn"].ToString();
                    //string strKanBanNo = dtSub.Rows[i]["vcKanBanNo"].ToString();
                    if (strPackingQty == "0")
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("{0}的收容数为空或0", strPartId);
                        dtMessage.Rows.Add(dataRow);
                    }
                    if (strSufferIn == "")
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("{0}的受入为空", strPartId);
                        dtMessage.Rows.Add(dataRow);
                    }
                }
                if (dtMessage.Rows.Count != 0)
                    return;
                fs0617_DataAccess.setPrintTemp(dtMain, dtSub, strOperId, ref dtMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region 将照片转换为二进制数组
        /// <param name="path">文件路径</param>
        /// <returns>二进制流</returns>
        public byte[] PhotoToArray(string path, string path2)
        {
            try
            {
                FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                byte[] bufferPhoto = new byte[stream.Length];
                stream.Read(bufferPhoto, 0, Convert.ToInt32(stream.Length));
                stream.Flush();
                stream.Close();
                return bufferPhoto;
            }
            catch
            {
                FileStream stream = new FileStream(path2, FileMode.Open, FileAccess.Read);
                byte[] bufferPhoto = new byte[stream.Length];
                stream.Read(bufferPhoto, 0, Convert.ToInt32(stream.Length));
                stream.Flush();
                stream.Close();
                return bufferPhoto;
            }
        }
        public byte[] PhotoToArray(string path)
        {
            try
            {
                FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                byte[] bufferPhoto = new byte[stream.Length];
                stream.Read(bufferPhoto, 0, Convert.ToInt32(stream.Length));
                stream.Flush();
                stream.Close();
                return bufferPhoto;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 生成QRCODE二维码
        public byte[] GenerateQRCode(string content)
        {
            var generator = new QRCodeGenerator();

            var codeData = generator.CreateQrCode(content, QRCodeGenerator.ECCLevel.M, true);
            QRCoder.QRCode qrcode = new QRCoder.QRCode(codeData);

            var bitmapImg = qrcode.GetGraphic(10, Color.Black, Color.White, false);

            using MemoryStream stream = new MemoryStream();
            bitmapImg.Save(stream, ImageFormat.Jpeg);
            return stream.GetBuffer();
        }
        #endregion

    }
}
