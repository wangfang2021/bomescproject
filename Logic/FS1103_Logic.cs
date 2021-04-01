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
                        string strTagLianFFrom = listInfoData[i]["vcTagLianFFrom"] == null ? "" : listInfoData[i]["vcTagLianFFrom"].ToString();
                        string strTagLianFTo = listInfoData[i]["vcTagLianFTo"] == null ? "" : listInfoData[i]["vcTagLianFTo"].ToString();
                        DataRow dataRow = dataTable.NewRow();
                        dataRow["vcReceiver"] = strReceiver;
                        dataRow["vcPartId"] = strPartId;
                        dataRow["vcInPutOrderNo"] = strInPutOrderNo;
                        dataRow["vcLabelNum"] = strLabelNum;
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
                        DataRow drInputTemp = dtInputTemp.NewRow();
                        drInputTemp["vcInno"] = strInPutOrderNo;
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
        public DataTable getTempInfo(string strOperId,string strType)
        {
            return fs1103_DataAccess.getTempInfo(strOperId, strType);
        }

    }
}

