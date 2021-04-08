﻿using System;
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
        public DataTable getSearchInfo(string strReceiver, string strSupplier, string strInPutOrderNo, string strPartId, string strLianFan)
        {
            return fs1103_DataAccess.getSearchInfo(strReceiver, strSupplier, strInPutOrderNo, strPartId, strLianFan);
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
                    string bInPutOrder = dataTable.Rows[i]["bInPutOrder"].ToString();
                    string bTag = dataTable.Rows[i]["bTag"].ToString();
                    string strInPutOrderNo = dataTable.Rows[i]["vcInPutOrderNo"].ToString();
                    if (bInPutOrder == "1")
                    {
                        string strLabelNum = dataTable.Rows[i]["vcLabelNum"].ToString();
                        string strInPutNum= dataTable.Rows[i]["vcInPutNum"].ToString();
                        if(strInPutNum==""|| strInPutNum=="0")
                        {
                            strInPutNum = strLabelNum;
                        }
                        if(!fS0603_Logic.IsInt(strInPutNum))
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = "所选择的入库指令书" + strInPutOrderNo + "的指令书数量不为数字";
                            dtMessage.Rows.Add(dataRow);
                        }
                        DataRow drInputTemp = dtInputTemp.NewRow();
                        drInputTemp["vcInno"] = strInPutOrderNo;
                        drInputTemp["vcInPutNum"] = strInPutNum;
                        dtInputTemp.Rows.Add(drInputTemp);
                    }
                    if (bTag == "1")
                    {
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
                                        DataRow[] drTagList = dtTagInfo.Select("vcInno='" + strInPutOrderNo + "' and vcPrintcount='" + strTagNo + "'");
                                        if (drTagList.Length == 0)
                                        {
                                            DataRow dataRow = dtMessage.NewRow();
                                            dataRow["vcMessage"] = "所选择的入库指令书" + strInPutOrderNo + "标签不属于该入库指令书已发行标签，不可打印";
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
                    }
                }
                if (dtMessage.Rows.Count != 0)
                    return;
                else
                {
                    //插入临时表准备打印
                    setPrintTemp(dtInputTemp, dtTagTemp, strOperId, ref dtMessage);
                }

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
                drTagTemp["vcLabel"] = dtPartInfo.Rows[0]["vcLabel1"].ToString();
                drTagTemp["vcGetnum"] = dtPartInfo.Rows[0]["vcGetnum"].ToString();
                drTagTemp["iDateprintflg"] = dtPartInfo.Rows[0]["iDateprintflg"].ToString();
                drTagTemp["vcComputernm"] = dtPartInfo.Rows[0]["vcComputernm"].ToString();
                drTagTemp["vcPrindate"] = dtPartInfo.Rows[0]["vcPrindate"].ToString();
                byte[] iCodemage = fS0617_Logic.GenerateQRCode(dtPartInfo.Rows[0]["vcQrcodeString1"].ToString());//二维码信息
                drTagTemp["iQrcode"] = iCodemage;
                drTagTemp["vcPrintcount"] = dtPartInfo.Rows[0]["vcPrintcount1"].ToString();
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

    }
}

