using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS1103_Logic
    {
        FS1103_DataAccess fs1103_DataAccess;
        FS0603_Logic fS0603_Logic = new FS0603_Logic();
        FS0617_Logic fS0617_Logic = new FS0617_Logic();

        public FS1103_Logic()
        {
            fs1103_DataAccess = new FS1103_DataAccess();
        }
        public DataTable getSearchInfo(string strReceiver, string strOrderNo, string strInPutOrderNo, string strPartId, string strLianFan)
        {
            return fs1103_DataAccess.getSearchInfo(strReceiver, strOrderNo, strInPutOrderNo, strPartId, strLianFan);
        }
        public void getPrintInfo(List<Dictionary<string, Object>> listInfoData, string strOperId, ref DataTable dtMessage)
        {
            try
            {
                DataTable dataTable = fS0603_Logic.createTable("print1103");

                for (int i = 0; i < listInfoData.Count; i++)
                {
                    string bInPutOrder = listInfoData[i]["bInPutOrder"].ToString().ToLower() == "true" ? "1" : "0";
                    string bTag = listInfoData[i]["bTag"].ToString().ToLower() == "true" ? "1" : "0";
                    if (bInPutOrder == "1" || bTag == "1")
                    {
                        string strReceiver = listInfoData[i]["vcReceiver"] == null ? "" : listInfoData[i]["vcReceiver"].ToString();
                        string strPartId = listInfoData[i]["vcPartId"] == null ? "" : listInfoData[i]["vcPartId"].ToString();
                        string strInPutOrderNo = listInfoData[i]["vcInPutOrderNo"] == null ? "" : listInfoData[i]["vcInPutOrderNo"].ToString();
                        string strLabelNum = listInfoData[i]["vcLabelNum"] == null ? "" : listInfoData[i]["vcLabelNum"].ToString();
                        string strInPutNum = listInfoData[i]["vcInPutNum"] == null ? "" : listInfoData[i]["vcInPutNum"].ToString();
                        string strTagLianFFrom = listInfoData[i]["vcTagLianFFrom"] == null ? "" : listInfoData[i]["vcTagLianFFrom"].ToString();
                        string strTagLianFTo = listInfoData[i]["vcTagLianFTo"] == null ? "" : listInfoData[i]["vcTagLianFTo"].ToString();
                        DataRow dataRow = dataTable.NewRow();
                        dataRow["vcReceiver"] = strReceiver;
                        dataRow["vcPartId"] = strPartId;
                        dataRow["vcInPutOrderNo"] = strInPutOrderNo;
                        dataRow["vcLabelNum"] = strLabelNum;
                        dataRow["vcInPutNum"] = strInPutNum;
                        dataRow["bInPutOrder"] = bInPutOrder;
                        dataRow["bTag"] = bTag;
                        dataRow["vcTagLianFFrom"] = strTagLianFFrom;
                        dataRow["vcTagLianFTo"] = strTagLianFTo;
                        dataTable.Rows.Add(dataRow);
                    }
                }
                if (dataTable.Rows.Count == 0)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "没有可供打印的指令书或者标签";
                    dtMessage.Rows.Add(dataRow);
                    return;
                }
                //整理数据
                DataTable dtInput = fs1103_DataAccess.getPrintTemp("input_FS1103");
                DataTable dtTag = fs1103_DataAccess.getPrintTemp("tag_FS1103");
                DataTable dtInputTemp = dtInput.Clone();
                DataTable dtTagTemp = dtTag.Clone();
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    string strPartId = dataTable.Rows[i]["vcPartId"].ToString();
                    string bInPutOrder = dataTable.Rows[i]["bInPutOrder"].ToString();
                    string bTag = dataTable.Rows[i]["bTag"].ToString();
                    string strInPutOrderNo = dataTable.Rows[i]["vcInPutOrderNo"].ToString();
                    if (bInPutOrder == "1")
                    {
                        #region 指令书
                        string strLabelNum = dataTable.Rows[i]["vcLabelNum"].ToString();
                        string strInPutNum = dataTable.Rows[i]["vcInPutNum"].ToString();
                        if (strInPutNum == "" || strInPutNum == "0")
                        {
                            strInPutNum = strLabelNum;
                        }
                        if (!fS0603_Logic.IsInt(strInPutNum))
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = "所选择的入库指令书" + strInPutOrderNo + "的指令书数量不为数字";
                            dtMessage.Rows.Add(dataRow);
                        }
                        DataRow drInputTemp = dtInputTemp.NewRow();
                        drInputTemp["vcInno"] = strInPutOrderNo;
                        drInputTemp["vcInPutNum"] = strInPutNum;
                        dtInputTemp.Rows.Add(drInputTemp);
                        #endregion
                    }
                    if (bTag == "1")
                    {
                        #region 标签
                        string strTagLianFFrom = dataTable.Rows[i]["vcTagLianFFrom"].ToString();
                        string strTagLianFTo = dataTable.Rows[i]["vcTagLianFTo"].ToString();
                        if (strTagLianFFrom.Length != 11)
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = "所选择的入库指令书" + strInPutOrderNo + "标签连番起输入有误";
                            dtMessage.Rows.Add(dataRow);
                        }
                        else
                        {
                            if (strTagLianFFrom != "" && strTagLianFTo == "")
                            {
                                strTagLianFTo = strTagLianFFrom;
                            }
                            if (strTagLianFFrom.Substring(0, 6) != strTagLianFTo.Substring(0, 6))
                            {
                                DataRow dataRow = dtMessage.NewRow();
                                dataRow["vcMessage"] = "所选择的入库指令书" + strInPutOrderNo + "标签连番起止输入有误";
                                dtMessage.Rows.Add(dataRow);
                            }
                            else
                            {
                                if (Convert.ToInt32(strTagLianFFrom.Substring(6, 5)) > Convert.ToInt32(strTagLianFTo.Substring(6, 5)))
                                {
                                    DataRow dataRow = dtMessage.NewRow();
                                    dataRow["vcMessage"] = "所选择的入库指令书" + strInPutOrderNo + "标签连番起止输入有误";
                                    dtMessage.Rows.Add(dataRow);
                                }
                                else
                                {
                                    DataTable dtTagInfo = fs1103_DataAccess.getSearchInfo(strInPutOrderNo);
                                    for (int j = Convert.ToInt32(strTagLianFFrom.Substring(6, 5)); j <= Convert.ToInt32(strTagLianFTo.Substring(6, 5)); j++)
                                    {
                                        string strTagNo = strTagLianFFrom.Substring(0, 6) + (100000 + j).ToString().Substring(1, 5);
                                        DataRow[] drTagList = dtTagInfo.Select("vcInno='" + strInPutOrderNo + "' and vcPrintcount='" + strPartId + strTagNo + "'");
                                        if (drTagList.Length == 0)
                                        {
                                            DataRow dataRow = dtMessage.NewRow();
                                            dataRow["vcMessage"] = "所选标签ID" + strTagNo + "超出入库指令书" + strInPutOrderNo + "发行范围";
                                            dtMessage.Rows.Add(dataRow);
                                        }
                                        else
                                        {
                                            DataRow drTagTemp = dtTagTemp.NewRow();
                                            drTagTemp["iAutoId"] = drTagList[0]["iAutoId"].ToString();
                                            dtTagTemp.Rows.Add(drTagTemp);
                                        }
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                }
                if (dtMessage.Rows.Count != 0)
                    return;
                //重新设置指令书
                dtInputTemp = setInvInfo(dtInputTemp, ref dtMessage);
                if (dtMessage.Rows.Count != 0)
                    return;
                //插入临时表准备打印
                setPrintTemp(dtInputTemp, dtTagTemp, strOperId, ref dtMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable setInvInfo(DataTable dtInputTemp, ref DataTable dtMessage)
        {
            try
            {
                DataTable dtInvInfo = dtInputTemp.Clone();
                for (int i = 0; i < dtInputTemp.Rows.Count; i++)
                {
                    string strInvo = dtInputTemp.Rows[i]["vcInno"].ToString();
                    string strInPutNum = dtInputTemp.Rows[i]["vcInPutNum"].ToString();
                    DataTable dtInvInfo_temp = fs1103_DataAccess.getInvInfo(strInvo);
                    if (dtInvInfo_temp.Rows.Count != 0)
                    {
                        string strInPutNum_temp = dtInvInfo_temp.Rows[0]["vcInputnum"].ToString();
                        #region 赋值
                        DataRow drInvInfo = dtInvInfo.NewRow();
                        drInvInfo["vcNo"] = dtInvInfo_temp.Rows[0]["vcNo"].ToString();
                        drInvInfo["vcData"] = dtInvInfo_temp.Rows[0]["vcData"].ToString();
                        drInvInfo["vcPrintdate"] = dtInvInfo_temp.Rows[0]["vcPrintdate"].ToString();
                        drInvInfo["vcInno"] = dtInvInfo_temp.Rows[0]["vcInno"].ToString();
                        drInvInfo["vcPart_Id"] = dtInvInfo_temp.Rows[0]["vcPart_Id"].ToString();
                        drInvInfo["vcPartsnamechn"] = dtInvInfo_temp.Rows[0]["vcPartsnamechn"].ToString();
                        drInvInfo["vcPartslocation"] = dtInvInfo_temp.Rows[0]["vcPartslocation"].ToString();
                        drInvInfo["vcInputnum"] = strInPutNum;
                        drInvInfo["vcPackingquantity"] = dtInvInfo_temp.Rows[0]["vcPackingquantity"].ToString();
                        if (Convert.ToInt32(strInPutNum) % Convert.ToInt32(dtInvInfo_temp.Rows[0]["vcPackingquantity"].ToString()) != 0)
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = "所选择的入库指令书" + strInvo + "入库指令书数量不为包装单位整数倍";
                            dtMessage.Rows.Add(dataRow);
                        }
                        drInvInfo["vcItemname1"] = dtInvInfo_temp.Rows[0]["vcItemname1"].ToString();
                        drInvInfo["vcPackingpartslocation1"] = dtInvInfo_temp.Rows[0]["vcPackingpartslocation1"].ToString();
                        drInvInfo["vcSuppliernamechn1"] = dtInvInfo_temp.Rows[0]["vcSuppliernamechn1"].ToString();
                        drInvInfo["vcOutnum1"] = dtInvInfo_temp.Rows[0]["vcOutnum1"].ToString();
                        if (dtInvInfo_temp.Rows[0]["vcOutnum1"].ToString() != "")
                        {
                            drInvInfo["vcOutnum1"] = (Convert.ToDecimal(dtInvInfo_temp.Rows[0]["vcOutnum1"].ToString()) * (Convert.ToDecimal(strInPutNum) / Convert.ToDecimal(strInPutNum_temp))).ToString();
                        }
                        drInvInfo["vcTemname2"] = dtInvInfo_temp.Rows[0]["vcTemname2"].ToString();
                        drInvInfo["vcPackingpartslocation2"] = dtInvInfo_temp.Rows[0]["vcPackingpartslocation2"].ToString();
                        drInvInfo["vcSuppliernamechn2"] = dtInvInfo_temp.Rows[0]["vcSuppliernamechn2"].ToString();
                        drInvInfo["vcOutnum2"] = dtInvInfo_temp.Rows[0]["vcOutnum2"].ToString();
                        if (dtInvInfo_temp.Rows[0]["vcOutnum2"].ToString() != "")
                        {
                            drInvInfo["vcOutnum2"] = (Convert.ToDecimal(dtInvInfo_temp.Rows[0]["vcOutnum2"].ToString()) * (Convert.ToDecimal(strInPutNum) / Convert.ToDecimal(strInPutNum_temp))).ToString();
                        }
                        drInvInfo["vcItemname3"] = dtInvInfo_temp.Rows[0]["vcItemname3"].ToString();
                        drInvInfo["vcPackingpartslocation3"] = dtInvInfo_temp.Rows[0]["vcPackingpartslocation3"].ToString();
                        drInvInfo["vcSuppliernamechn3"] = dtInvInfo_temp.Rows[0]["vcSuppliernamechn3"].ToString();
                        drInvInfo["vcOutnum3"] = dtInvInfo_temp.Rows[0]["vcOutnum3"].ToString();
                        if (dtInvInfo_temp.Rows[0]["vcOutnum3"].ToString() != "")
                        {
                            drInvInfo["vcOutnum3"] = (Convert.ToDecimal(dtInvInfo_temp.Rows[0]["vcOutnum3"].ToString()) * (Convert.ToDecimal(strInPutNum) / Convert.ToDecimal(strInPutNum_temp))).ToString();
                        }
                        drInvInfo["vcItemname4"] = dtInvInfo_temp.Rows[0]["vcItemname4"].ToString();
                        drInvInfo["vcPackingpartslocation4"] = dtInvInfo_temp.Rows[0]["vcPackingpartslocation4"].ToString();
                        drInvInfo["vcSuppliernamechn4"] = dtInvInfo_temp.Rows[0]["vcSuppliernamechn4"].ToString();
                        drInvInfo["vcOutnum4"] = dtInvInfo_temp.Rows[0]["vcOutnum4"].ToString();
                        if (dtInvInfo_temp.Rows[0]["vcOutnum4"].ToString() != "")
                        {
                            drInvInfo["vcOutnum4"] = (Convert.ToDecimal(dtInvInfo_temp.Rows[0]["vcOutnum4"].ToString()) * (Convert.ToDecimal(strInPutNum) / Convert.ToDecimal(strInPutNum_temp))).ToString();
                        }
                        drInvInfo["vcItemname5"] = dtInvInfo_temp.Rows[0]["vcItemname5"].ToString();
                        drInvInfo["vcPackingpartslocation5"] = dtInvInfo_temp.Rows[0]["vcPackingpartslocation5"].ToString();
                        drInvInfo["vcSuppliernamechn5"] = dtInvInfo_temp.Rows[0]["vcSuppliernamechn5"].ToString();
                        drInvInfo["vcOutnum5"] = dtInvInfo_temp.Rows[0]["vcOutnum5"].ToString();
                        if (dtInvInfo_temp.Rows[0]["vcOutnum5"].ToString() != "")
                        {
                            drInvInfo["vcOutnum5"] = (Convert.ToDecimal(dtInvInfo_temp.Rows[0]["vcOutnum5"].ToString()) * (Convert.ToDecimal(strInPutNum) / Convert.ToDecimal(strInPutNum_temp))).ToString();
                        }
                        drInvInfo["vcItemname6"] = dtInvInfo_temp.Rows[0]["vcItemname6"].ToString();
                        drInvInfo["vcPackingpartslocation6"] = dtInvInfo_temp.Rows[0]["vcPackingpartslocation6"].ToString();
                        drInvInfo["vcSuppliernamechn6"] = dtInvInfo_temp.Rows[0]["vcSuppliernamechn6"].ToString();
                        drInvInfo["vcOutnum6"] = dtInvInfo_temp.Rows[0]["vcOutnum6"].ToString();
                        if (dtInvInfo_temp.Rows[0]["vcOutnum6"].ToString() != "")
                        {
                            drInvInfo["vcOutnum6"] = (Convert.ToDecimal(dtInvInfo_temp.Rows[0]["vcOutnum6"].ToString()) * (Convert.ToDecimal(strInPutNum) / Convert.ToDecimal(strInPutNum_temp))).ToString();
                        }
                        drInvInfo["vcItemname7"] = dtInvInfo_temp.Rows[0]["vcItemname7"].ToString();
                        drInvInfo["vcPackingpartslocation7"] = dtInvInfo_temp.Rows[0]["vcPackingpartslocation7"].ToString();
                        drInvInfo["vcSuppliernamechn7"] = dtInvInfo_temp.Rows[0]["vcSuppliernamechn7"].ToString();
                        drInvInfo["vcOutnum7"] = dtInvInfo_temp.Rows[0]["vcOutnum7"].ToString();
                        if (dtInvInfo_temp.Rows[0]["vcOutnum7"].ToString() != "")
                        {
                            drInvInfo["vcOutnum7"] = (Convert.ToDecimal(dtInvInfo_temp.Rows[0]["vcOutnum7"].ToString()) * (Convert.ToDecimal(strInPutNum) / Convert.ToDecimal(strInPutNum_temp))).ToString();
                        }
                        drInvInfo["vcItemname8"] = dtInvInfo_temp.Rows[0]["vcItemname8"].ToString();
                        drInvInfo["vcPackingpartslocation8"] = dtInvInfo_temp.Rows[0]["vcPackingpartslocation8"].ToString();
                        drInvInfo["vcSuppliernamechn8"] = dtInvInfo_temp.Rows[0]["vcSuppliernamechn8"].ToString();
                        drInvInfo["vcOutnum8"] = dtInvInfo_temp.Rows[0]["vcOutnum8"].ToString();
                        if (dtInvInfo_temp.Rows[0]["vcOutnum8"].ToString() != "")
                        {
                            drInvInfo["vcOutnum8"] = (Convert.ToDecimal(dtInvInfo_temp.Rows[0]["vcOutnum8"].ToString()) * (Convert.ToDecimal(strInPutNum) / Convert.ToDecimal(strInPutNum_temp))).ToString();
                        }
                        drInvInfo["vcItemname9"] = dtInvInfo_temp.Rows[0]["vcItemname9"].ToString();
                        drInvInfo["vcPackingpartslocation9"] = dtInvInfo_temp.Rows[0]["vcPackingpartslocation9"].ToString();
                        drInvInfo["vcSuppliernamechn9"] = dtInvInfo_temp.Rows[0]["vcSuppliernamechn9"].ToString();
                        drInvInfo["vcOutnum9"] = dtInvInfo_temp.Rows[0]["vcOutnum9"].ToString();
                        if (dtInvInfo_temp.Rows[0]["vcOutnum9"].ToString() != "")
                        {
                            drInvInfo["vcOutnum9"] = (Convert.ToDecimal(dtInvInfo_temp.Rows[0]["vcOutnum9"].ToString()) * (Convert.ToDecimal(strInPutNum) / Convert.ToDecimal(strInPutNum_temp))).ToString();
                        }
                        drInvInfo["vcItemname10"] = dtInvInfo_temp.Rows[0]["vcItemname10"].ToString();
                        drInvInfo["vcPackingpartslocation10"] = dtInvInfo_temp.Rows[0]["vcPackingpartslocation10"].ToString();
                        drInvInfo["vcSuppliernamechn10"] = dtInvInfo_temp.Rows[0]["vcSuppliernamechn10"].ToString();
                        drInvInfo["vcOutnum10"] = dtInvInfo_temp.Rows[0]["vcOutnum10"].ToString();
                        if (dtInvInfo_temp.Rows[0]["vcOutnum10"].ToString() != "")
                        {
                            drInvInfo["vcOutnum10"] = (Convert.ToDecimal(dtInvInfo_temp.Rows[0]["vcOutnum10"].ToString()) * (Convert.ToDecimal(strInPutNum) / Convert.ToDecimal(strInPutNum_temp))).ToString();
                        }
                        string strCode = dtInvInfo_temp.Rows[0]["vcPart_Id"].ToString() +
                            strInPutNum.PadLeft(5, '0') +
                            //(Convert.ToInt32(strInPutNum) + 100000).ToString().Substring(1, 5) +
                            dtInvInfo_temp.Rows[0]["vcCpdcompany"].ToString() +
                            dtInvInfo_temp.Rows[0]["vcInno"].ToString();

                        drInvInfo["vcPartsnoandnum"] = strCode;
                        drInvInfo["vcLabel"] = strCode;
                        drInvInfo["vcComputernm"] = dtInvInfo_temp.Rows[0]["vcComputernm"].ToString();
                        drInvInfo["vcCpdcompany"] = dtInvInfo_temp.Rows[0]["vcCpdcompany"].ToString();
                        drInvInfo["vcPlantcode"] = dtInvInfo_temp.Rows[0]["vcPlantcode"].ToString();
                        drInvInfo["vcCompanyname"] = dtInvInfo_temp.Rows[0]["vcCompanyname"].ToString();
                        drInvInfo["vcPlantname"] = dtInvInfo_temp.Rows[0]["vcPlantname"].ToString();
                        byte[] bCodemage = fS0617_Logic.GenerateQRCode(strCode);//二维码信息
                        drInvInfo["iQrcode"] = bCodemage;
                        dtInvInfo.Rows.Add(drInvInfo);
                        #endregion

                    }
                    else
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = "所选择的入库指令书" + strInvo + "查询数据为空，不可打印";
                        dtMessage.Rows.Add(dataRow);
                    }
                }
                return dtInvInfo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setPrintTemp(DataTable dtInputTemp, DataTable dtTagTemp, string strOperId, ref DataTable dtMessage)
        {
            fs1103_DataAccess.setPrintTemp(dtInputTemp, dtTagTemp, strOperId, ref dtMessage);
        }
        public DataTable getTempInfo(string strOperId, string strType)
        {
            return fs1103_DataAccess.getTempInfo(strOperId, strType);
        }

        public void getPrintInfo(string strPartId, string strPrintNum, string strOperId, ref DataTable dtMessage)
        {
            //校验品番是否存在
            DataTable dtPartInfo = fs1103_DataAccess.getPartInfo(strPartId);
            if (dtPartInfo.Rows.Count == 0)
            {
                DataRow dataRow = dtMessage.NewRow();
                dataRow["vcMessage"] = "品番不在采购基础数据中，请确认";
                dtMessage.Rows.Add(dataRow);
            }
            if (dtMessage != null && dtMessage.Rows.Count != 0)
                return;
            int iPrintNum = Convert.ToInt32(strPrintNum);
            DataTable dtTag = fs1103_DataAccess.getPrintTemp("tag_FS1103");
            DataTable dtTagTemp = dtTag.Clone();
            for (int i = 1; i <= iPrintNum; i++)
            {
                DataRow drTagTemp = dtTagTemp.NewRow();

                drTagTemp["vcPartsnameen"] = dtPartInfo.Rows[0]["vcPartsnameen"].ToString();
                drTagTemp["vcPart_id"] = dtPartInfo.Rows[0]["vcPart_id"].ToString();
                drTagTemp["vcInno"] = dtPartInfo.Rows[0]["vcInno"].ToString();
                drTagTemp["vcCpdcompany"] = dtPartInfo.Rows[0]["vcCpdcompany"].ToString();
                //drTagTemp["vcLabel"] = dtPartInfo.Rows[0]["vcLabel1"].ToString();
                drTagTemp["vcLabel"] = ChangeBarCode(dtPartInfo.Rows[0]["vcPart_id"].ToString()).ToString();
                drTagTemp["vcGetnum"] = dtPartInfo.Rows[0]["vcGetnum"].ToString();
                drTagTemp["iDateprintflg"] = dtPartInfo.Rows[0]["iDateprintflg"].ToString();
                drTagTemp["vcComputernm"] = dtPartInfo.Rows[0]["vcComputernm"].ToString();
                drTagTemp["vcPrindate"] = dtPartInfo.Rows[0]["vcPrindate"].ToString();
                byte[] iCodemage = fS0617_Logic.GenerateQRCode(dtPartInfo.Rows[0]["vcQrcodeString1"].ToString());//二维码信息
                drTagTemp["iQrcode"] = iCodemage;
                drTagTemp["vcPrintcount"] = dtPartInfo.Rows[0]["vcPart_id"].ToString() + dtPartInfo.Rows[0]["vcPrintcount1"].ToString();
                drTagTemp["vcPartnamechineese"] = dtPartInfo.Rows[0]["vcPartnamechineese"].ToString();
                drTagTemp["vcSuppliername"] = dtPartInfo.Rows[0]["vcSuppliername"].ToString();
                drTagTemp["vcSupplieraddress"] = dtPartInfo.Rows[0]["vcSupplieraddress"].ToString();
                drTagTemp["vcExecutestandard"] = dtPartInfo.Rows[0]["vcExecutestandard"].ToString();
                drTagTemp["vcCartype"] = dtPartInfo.Rows[0]["vcCartype"].ToString();
                drTagTemp["vcHostip"] = "";
                dtTagTemp.Rows.Add(drTagTemp);
            }
            fs1103_DataAccess.setPrintTemp(dtTagTemp, strOperId, ref dtMessage);
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

