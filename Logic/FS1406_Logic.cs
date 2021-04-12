using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;
using System.IO;
using ImageMagick;

namespace Logic
{
    public class FS1406_Logic
    {
        FS1406_DataAccess fs1406_DataAccess;
        FS0603_DataAccess fs0603_DataAccess = new FS0603_DataAccess();
        FS0617_Logic fS0617_Logic = new FS0617_Logic();
        FS0603_Logic fS0603_Logic = new FS0603_Logic();

        public FS1406_Logic()
        {
            fs1406_DataAccess = new FS1406_DataAccess();
        }
        public DataTable getSearchInfo(string strPartId, string strSupplierId, string strOrderPlant, string strCarModel, string strSPISStatus)
        {
            return fs1406_DataAccess.getSearchInfo(strPartId, strSupplierId, strOrderPlant, strCarModel, strSPISStatus);
        }
        public DataTable checkreplyInfo(List<Dictionary<string, Object>> multipleInfoData, string strOperId, string strOpername, ref DataTable dtMessage)
        {
            try
            {
                DataTable dtImport = fS0603_Logic.createTable("SPISApply");
                if (multipleInfoData.Count != 0)
                {
                    for (int i = 0; i < multipleInfoData.Count; i++)
                    {
                        if (fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcSPISStatus"], "", "") == "0" ||
                            fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcSPISStatus"], "", "") == "1" ||
                            fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcSPISStatus"], "", "") == "4")
                        {
                            string strLinId = fs0603_DataAccess.setNullValue(multipleInfoData[i]["LinId"], "", "");
                            string strApplyId = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcApplyId"], "", "");
                            string strPartId = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcPartId"], "", "");
                            string strCarfamilyCode = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcCarfamilyCode"], "", "");
                            string strSupplierId = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcSupplierId"], "", "");
                            string strSupplierName = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcSupplierName"], "", "");
                            string strPartENName = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcPartENName"], "", "");
                            string strColourNo = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcColourNo"], "", "");
                            string strColourCode = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcColourCode"], "", "");
                            string strColourName = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcColourName"], "", "");
                            string strModItem = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcModItem"], "", "");
                            string strSPISUrl = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcSPISUrl"], "", "");
                            string strSPISPath = fs0603_DataAccess.setNullValue(multipleInfoData[i]["vcSPISPath"], "", "");
                            string strSupplier_1 = strOpername;
                            string strSupplier_2 = strOpername;
                            DataRow drImport = dtImport.NewRow();
                            drImport["LinId"] = strLinId;
                            drImport["vcApplyId"] = strApplyId;
                            drImport["vcPartId"] = strPartId;
                            drImport["vcCarfamilyCode"] = strCarfamilyCode;
                            drImport["vcSupplierId"] = strSupplierId;
                            drImport["vcSupplierName"] = strSupplierName;
                            drImport["vcPartENName"] = strPartENName;
                            drImport["vcColourNo"] = strColourNo;
                            drImport["vcColourCode"] = strColourCode;
                            drImport["vcColourName"] = strColourName;
                            drImport["vcModItem"] = strModItem;
                            drImport["vcSPISUrl"] = strSPISUrl;
                            drImport["vcSPISPath"] = strSPISPath;
                            drImport["vcSupplier_1"] = strSupplier_1;
                            drImport["vcSupplier_2"] = strSupplier_2;
                            drImport["vcSPISStatus"] = "2";//提交标识
                            dtImport.Rows.Add(drImport);
                        }
                    }
                }
                if (dtImport.Rows.Count == 0)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "没有可供回复的数据";
                    dtMessage.Rows.Add(dataRow);
                }
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                    return null;
                for (int i = 0; i < dtImport.Rows.Count; i++)
                {
                    string strApplyId = dtImport.Rows[i]["vcApplyId"].ToString();
                    string strPartId = dtImport.Rows[i]["vcPartId"].ToString();
                    string strCarfamilyCode = dtImport.Rows[i]["vcCarfamilyCode"].ToString();
                    string strSupplierId = dtImport.Rows[i]["vcSupplierId"].ToString();
                    string strSupplierName = dtImport.Rows[i]["vcSupplierName"].ToString();
                    string strPartENName = dtImport.Rows[i]["vcPartENName"].ToString();
                    string strColourNo = dtImport.Rows[i]["vcColourNo"].ToString();
                    string strColourCode = dtImport.Rows[i]["vcColourCode"].ToString();
                    string strColourName = dtImport.Rows[i]["vcColourName"].ToString();
                    string strModItem = dtImport.Rows[i]["vcModItem"].ToString();
                    string strSPISPath = dtImport.Rows[i]["vcSPISPath"].ToString();

                    if (strSupplierName == "" || strColourNo == "" || strColourCode == "" || strColourName == "" || strModItem == "")
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = strPartId + "缺少必填项请完善数据。";
                        dtMessage.Rows.Add(dataRow);
                    }
                    if (!System.IO.File.Exists(strSPISPath))
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = strPartId + "品番没有有效的SPIS请完善数据。";
                        dtMessage.Rows.Add(dataRow);
                    }
                }
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                    return null;
                return dtImport;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable setInfoList(dynamic dataForm)
        {
            try
            {
                DataTable dtImport = fS0603_Logic.createTable("SPISApply");

                string strModel = dataForm.model == null ? "" : dataForm.model.ToString();
                string strSPISStatus = dataForm.SPISStatus == null ? "" : dataForm.SPISStatus.ToString();

                string strApplyId = dataForm.ApplyId == null ? "" : dataForm.ApplyId.ToString();
                string strFromTime_SPIS = dataForm.FromTime_SPIS == null ? "" : dataForm.FromTime_SPIS.ToString();
                string strToTime_SPIS = dataForm.ToTime_SPIS == null ? "" : dataForm.ToTime_SPIS.ToString();
                string strSPISTime = dataForm.SPISTime == null ? "" : dataForm.SPISTime.ToString();
                string strPartId = dataForm.PartId == null ? "" : dataForm.PartId.ToString();
                string strCarfamilyCode = dataForm.CarfamilyCode == null ? "" : dataForm.CarfamilyCode.ToString();
                string strSupplierId = dataForm.SupplierId == null ? "" : dataForm.SupplierId.ToString();
                string strSupplierName = dataForm.SupplierName == null ? "" : dataForm.SupplierName.ToString();
                string strPartENName = dataForm.PartENName == null ? "" : dataForm.PartENName.ToString();
                string strColourNo = dataForm.ColourNo == null ? "" : dataForm.ColourNo.ToString();
                string strColourCode = dataForm.ColourCode == null ? "" : dataForm.ColourCode.ToString();
                string strColourName = dataForm.ColourName == null ? "" : dataForm.ColourName.ToString();
                string strModItem = dataForm.ModItem == null ? "" : dataForm.ModItem.ToString();
                string strSupplier_1 = dataForm.strSupplier_1 == null ? "" : dataForm.strSupplier_1.ToString();
                string strSupplier_2 = dataForm.strSupplier_2 == null ? "" : dataForm.strSupplier_2.ToString();
                string strOperImage = dataForm.PicRoutes == null ? "" : dataForm.PicRoutes.ToString();
                string strDelImageRoutes = dataForm.DelPicRoutes == null ? "" : dataForm.DelPicRoutes.ToString();
                DataRow drImport = dtImport.NewRow();
                drImport["vcApplyId"] = strApplyId;
                drImport["dFromTime_SPIS"] = strFromTime_SPIS;
                drImport["dToTime_SPIS"] = strToTime_SPIS;
                drImport["dSPISTime"] = strSPISTime;
                drImport["vcPartId"] = strPartId;
                drImport["vcCarfamilyCode"] = strCarfamilyCode;
                drImport["vcSupplierId"] = strSupplierId;
                drImport["vcSupplierName"] = strSupplierName;
                drImport["vcPartENName"] = strPartENName;
                drImport["vcColourNo"] = strColourNo;
                drImport["vcColourCode"] = strColourCode;
                drImport["vcColourName"] = strColourName;
                drImport["vcModItem"] = strModItem;
                drImport["vcSupplier_1"] = strSupplier_1;
                drImport["vcSupplier_2"] = strSupplier_2;
                drImport["vcTempUrl"] = strOperImage;
                drImport["vcSPISStatus"] = strSPISStatus;
                dtImport.Rows.Add(drImport);

                return dtImport;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void checkSaveInfo(DataTable dtImport, ref DataTable dtApplyList, ref DataTable dtPDF_temp, string strPath_temp, string strPath_pic, string strPath_pdf, string strPath_sips, string strOperId, ref DataTable dtMessage)
        {
            try
            {
                for (int i = 0; i < dtImport.Rows.Count; i++)
                {
                    string strApplyId = dtImport.Rows[i]["vcApplyId"].ToString();
                    string strFromTime_SPIS = dtImport.Rows[i]["dFromTime_SPIS"].ToString();
                    string strToTime_SPIS = dtImport.Rows[i]["dToTime_SPIS"].ToString();
                    string strSPISTime = dtImport.Rows[i]["dSPISTime"].ToString();
                    string strPartId = dtImport.Rows[i]["vcPartId"].ToString();
                    string strCarfamilyCode = dtImport.Rows[i]["vcCarfamilyCode"].ToString();
                    string strSupplierId = dtImport.Rows[i]["vcSupplierId"].ToString();
                    string strSupplierName = dtImport.Rows[i]["vcSupplierName"].ToString();
                    string strPartENName = dtImport.Rows[i]["vcPartENName"].ToString();
                    string strColourNo = dtImport.Rows[i]["vcColourNo"].ToString();
                    string strColourCode = dtImport.Rows[i]["vcColourCode"].ToString();
                    string strColourName = dtImport.Rows[i]["vcColourName"].ToString();
                    string strModItem = dtImport.Rows[i]["vcModItem"].ToString();
                    string strSupplier_1 = dtImport.Rows[i]["vcSupplier_1"].ToString();
                    string strSupplier_2 = dtImport.Rows[i]["vcSupplier_2"].ToString();
                    string strOperImage = dtImport.Rows[i]["vcTempUrl"].ToString();
                    string strSPISStatus = dtImport.Rows[i]["vcSPISStatus"].ToString();

                    if (strSupplierName == ""|| strSupplier_1 == "" || strSupplier_2=="")
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = "缺少必填项请完善数据。";
                        dtMessage.Rows.Add(dataRow);
                    }
                    else
                    {
                        if (strOperImage == "")
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = strPartId + "品番SPIS为空请完善数据。";
                            dtMessage.Rows.Add(dataRow);
                        }
                        else
                        {
                            if (!System.IO.File.Exists(strPath_temp + strOperImage))
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = strPartId + "品番SPIS路径没有图片请重新上传。";
                                dtMessage.Rows.Add(dataRow);
                            }
                            else
                            {
                                //原图名
                                dtImport.Rows[i]["vcPicUrl"] = strPartId + strSupplierId + Convert.ToDateTime(strFromTime_SPIS).ToString("yyyyMMdd") + "_1.jpg";
                                //PDF名
                                dtImport.Rows[i]["vcPDFUrl"] = strPartId + strSupplierId + Convert.ToDateTime(strFromTime_SPIS).ToString("yyyyMMdd") + "_2.pdf";
                                //SPIS名
                                dtImport.Rows[i]["vcSPISUrl"] = strPartId + strSupplierId + Convert.ToDateTime(strFromTime_SPIS).ToString("yyyyMMdd") + "_3.jpg";
                                if (strApplyId == "")
                                {
                                    dtImport.Rows[i]["vcType"] = "add";
                                    dtImport.Rows[i]["vcApplyId"] = Guid.NewGuid().ToString("N");
                                }
                                else
                                    dtImport.Rows[i]["vcType"] = "mod";

                            }
                        }
                    }
                }
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                    return;
                saveSPISPicAndApplyList(dtImport, ref dtApplyList, ref dtPDF_temp, strPath_temp, strPath_pic, strPath_pdf, strPath_sips, strOperId, ref dtMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getTempDataInfo()
        {
            return fs1406_DataAccess.getPrintTemp("FS1406").Clone();
        }
        public void saveSPISPicAndApplyList(DataTable dtImport, ref DataTable dtApplyList, ref DataTable dtPDF_temp,
            string strPath_temp, string strPath_pic, string strPath_pdf, string strPath_sips,
            string strOperId, ref DataTable dtMessage)
        {
            try
            {
                for (int i = 0; i < dtImport.Rows.Count; i++)
                {
                    #region  Data
                    string uuid = Guid.NewGuid().ToString("N");
                    string strLinId = dtImport.Rows[i]["LinId"].ToString();
                    string strApplyId = dtImport.Rows[i]["vcApplyId"].ToString();
                    string strFromTime_SPIS = dtImport.Rows[i]["dFromTime_SPIS"].ToString();
                    string strToTime_SPIS = dtImport.Rows[i]["dToTime_SPIS"].ToString();
                    string strSPISTime = dtImport.Rows[i]["dSPISTime"].ToString();
                    string strPartId = dtImport.Rows[i]["vcPartId"].ToString();
                    string strCarfamilyCode = dtImport.Rows[i]["vcCarfamilyCode"].ToString();
                    string strSupplierId = dtImport.Rows[i]["vcSupplierId"].ToString();
                    string strSupplierName = dtImport.Rows[i]["vcSupplierName"].ToString();
                    string strPartENName = dtImport.Rows[i]["vcPartENName"].ToString();
                    string strColourNo = dtImport.Rows[i]["vcColourNo"].ToString();
                    string strColourCode = dtImport.Rows[i]["vcColourCode"].ToString();
                    string strColourName = dtImport.Rows[i]["vcColourName"].ToString();
                    string strModItem = dtImport.Rows[i]["vcModItem"].ToString();
                    string strSupplier_1 = dtImport.Rows[i]["vcSupplier_1"].ToString();
                    string strSupplier_2 = dtImport.Rows[i]["vcSupplier_2"].ToString();

                    string strTempUrl = dtImport.Rows[i]["vcTempUrl"].ToString();//原图临时文件
                    string sources_temp = strPath_temp + strTempUrl;//原图临时文件地址
                    string strPICUrl = dtImport.Rows[i]["vcPicUrl"].ToString();//原图正式文件
                    string sources_pic = strPath_pic + strPICUrl;//原图正式文件地址
                    //移动到原图正式地址
                    if (System.IO.File.Exists(sources_temp))
                    {
                        File.Copy(sources_temp, sources_pic, true);//true代表可以覆盖同名文件
                        File.Delete(sources_temp);
                    }
                    //正式地址图片转成二进制流
                    byte[] btPICImage = fS0617_Logic.PhotoToArray(sources_pic);

                    string strPDFUrl = dtImport.Rows[i]["vcPDFUrl"].ToString();//PDF文件
                    string sources_pdf = strPath_pdf + strPDFUrl;//PDF文件地址

                    string strSPISUrl = dtImport.Rows[i]["vcSPISUrl"].ToString();//正式文件
                    string sources_spis = strPath_sips + strSPISUrl;//式文件地址

                    //string strSupplier_1 = dtImport.Rows[i]["vcSupplier_1"].ToString();
                    //string strSupplier_2 = dtImport.Rows[i]["vcSupplier_2"].ToString();
                    string strOperName = dtImport.Rows[i]["vcOperName"].ToString();
                    string strGM = dtImport.Rows[i]["vcGM"].ToString();
                    #endregion

                    #region dtPDF_temp
                    DataRow drPDF_temp = dtPDF_temp.NewRow();
                    drPDF_temp["UUID"] = uuid;
                    drPDF_temp["vcSPISTime"] = strSPISTime;
                    drPDF_temp["vcCarfamilyCode"] = strCarfamilyCode;
                    drPDF_temp["vcSupplierId"] = strSupplierId;
                    drPDF_temp["vcSupplierName"] = strSupplierName;
                    drPDF_temp["vcPartId"] = strPartId;
                    drPDF_temp["vcPartENName"] = strPartENName;
                    drPDF_temp["vcColourNo"] = strColourNo;
                    drPDF_temp["vcColourCode"] = strColourCode;
                    drPDF_temp["vcColourName"] = strColourName;
                    drPDF_temp["vcPICImage"] = btPICImage;
                    drPDF_temp["vcModDate1"] = System.DateTime.Now.ToString("yyyy-MM-dd");
                    drPDF_temp["vcModItem1"] = strModItem;
                    drPDF_temp["vcModDate2"] = string.Empty;
                    drPDF_temp["vcModItem2"] = string.Empty;
                    drPDF_temp["vcModDate3"] = string.Empty;
                    drPDF_temp["vcModItem3"] = string.Empty;
                    drPDF_temp["vcSupplier_1"] = strSupplier_1;
                    drPDF_temp["vcSupplier_2"] = strSupplier_2;
                    drPDF_temp["vcOperName"] = strOperName;
                    drPDF_temp["vcGM"] = strGM;
                    drPDF_temp["vcPDFPath"] = sources_pdf;
                    dtPDF_temp.Rows.Add(drPDF_temp);
                    #endregion

                    #region dtApplyList
                    DataRow drApplyList = dtApplyList.NewRow();
                    drApplyList["vcType"] = dtImport.Rows[i]["vcType"].ToString();
                    drApplyList["vcApplyId"] = strApplyId;
                    drApplyList["dFromTime_SPIS"] = strFromTime_SPIS;
                    drApplyList["dToTime_SPIS"] = strToTime_SPIS;
                    drApplyList["dSPISTime"] = strSPISTime;
                    drApplyList["vcPartId"] = strPartId;
                    drApplyList["vcCarfamilyCode"] = strCarfamilyCode;
                    drApplyList["vcSupplierId"] = strSupplierId;
                    drApplyList["vcSupplierName"] = strSupplierName;
                    drApplyList["vcPartENName"] = strPartENName;
                    drApplyList["vcColourNo"] = strColourNo;
                    drApplyList["vcColourCode"] = strColourCode;
                    drApplyList["vcColourName"] = strColourName;
                    drApplyList["dModDate"] = System.DateTime.Now.ToString("yyyy-MM-dd");
                    drApplyList["vcModItem"] = strModItem;
                    drApplyList["vcPICUrl"] = strPICUrl;
                    drApplyList["vcPICPath"] = sources_pic;
                    drApplyList["vcPDFUrl"] = strPDFUrl;
                    drApplyList["vcPDFPath"] = sources_pdf;
                    drApplyList["vcSPISUrl"] = strSPISUrl;
                    drApplyList["vcSPISPath"] = sources_spis;
                    drApplyList["vcSupplier_1"] = strSupplier_1;
                    drApplyList["vcSupplier_2"] = strSupplier_2;
                    drApplyList["vcOperName"] = strOperName;
                    drApplyList["vcGM"] = strGM;
                    dtApplyList.Rows.Add(drApplyList);
                    #endregion

                    #region TSPISQf表更新和插入用于tftm承认阶段

                    #endregion
                }
                ////处理图像
                ////1.插入并打印
                //setCRVtoPdf(dtPDF_temp, strOperId, ref dtMessage);
                ////2.PDF转SPIS图片
                //setPdftoImgs(dtApplyList, strOperId, ref dtMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setCRVtoPdf(DataRow drPDF_temp, string strOperId, ref DataTable dtMessage)
        {
            try
            {
                string sources_pdf = drPDF_temp["vcPDFPath"].ToString();
                fs1406_DataAccess.setPrintTemp(drPDF_temp, strOperId, ref dtMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setPdftoImgs(DataTable dtApplyList, string strOperId, ref DataTable dtMessage)
        {
            try
            {
                for (int i = 0; i < dtApplyList.Rows.Count; i++)
                {
                    string sources_pdf = dtApplyList.Rows[i]["vcPDFPath"].ToString();
                    string sources_spis = dtApplyList.Rows[i]["vcSPISPath"].ToString();
                    GetPdfAllPageImgs(sources_pdf, sources_spis, 0, "");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 将PDF所有页转换为图片并返回图片路径
        /// </summary>
        /// <param name="pdfPath">pdf文件路径</param>
        /// <param name="imgPath">生成图片路径</param>
        /// <param name="imgIndex">图片ID</param>
        /// <param name="imgName">图片名称前缀</param>
        public List<string> GetPdfAllPageImgs(string pdfPath, string imgPath, int imgIndex, string imgName)
        {
            var list = new List<string>();
            try
            {
                MagickReadSettings settings = new MagickReadSettings();
                settings.Density = new Density(72, 72); //设置格式
                using (MagickImageCollection images = new MagickImageCollection())
                {
                    images.Read(pdfPath, settings);
                    int pageCount = images.Count;
                    if (pageCount != 0)
                    {
                        IMagickImage image = images[imgIndex];
                        image.Alpha(AlphaOption.Remove);//遇到电子签章的此属性可以解决黑屏问题
                        image.Format = MagickFormat.Jpeg;
                        string path = imgPath;//相对路径   
                        image.Write(path);
                        list.Add(path);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public void replyInfo(DataTable dtImport, string strOperId, ref DataTable dtMessage)
        {
            try
            {
                fs1406_DataAccess.setReplyInfo(dtImport, strOperId, ref dtMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setSaveInfo(DataTable dtImport, string strOperId, ref DataTable dtMessage)
        {
            try
            {
                fs1406_DataAccess.setSaveInfo(dtImport, strOperId, ref dtMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
