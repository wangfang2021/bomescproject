using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS1102_Logic
    {
        FS1102_DataAccess fs1102_DataAccess;
        FS0617_Logic fS0617_Logic = new FS0617_Logic();
        public FS1102_Logic()
        {
            fs1102_DataAccess = new FS1102_DataAccess();
        }
        public DataTable getSearchInfo(string strReceiver, string strCaseNo, string strTagId)
        {
            return fs1102_DataAccess.getSearchInfo(strReceiver, strCaseNo, strTagId);
        }
        public bool getPrintInfo(List<Dictionary<string, Object>> listInfoData, string strOperId, ref DataTable dtMessage)
        {
            try
            {
                DataTable dataTable = fs1102_DataAccess.getPrintTemp("FS1102");
                DataTable dtSub = dataTable.Clone();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    string strReceiver = listInfoData[0]["vcReceiver"] == null ? "" : listInfoData[0]["vcReceiver"].ToString();
                    string strCaseNo = listInfoData[0]["vcCaseNo"] == null ? "" : listInfoData[0]["vcCaseNo"].ToString();
                    DataTable dtSPInfo = fs1102_DataAccess.getSearchInfo(strReceiver, strCaseNo);
                    string uuid = Guid.NewGuid().ToString("N");
                    for (int j = 0; j < dtSPInfo.Rows.Count; j++)
                    {
                        DataRow dataRow = dtSub.NewRow();
                        dataRow["UUID"] = uuid;
                        dataRow["vcNo"] = (j + 1).ToString();
                        dataRow["vcCpdcode"] = dtSPInfo.Rows[j]["vcCpdcode"].ToString();
                        dataRow["vcCpdname"] = dtSPInfo.Rows[j]["vcCpdname"].ToString();
                        dataRow["vcCpdaddress"] = dtSPInfo.Rows[j]["vcCpdaddress"].ToString();
                        dataRow["vcCaseno"] = dtSPInfo.Rows[j]["vcCaseno"].ToString();
                        dataRow["vcCaseno_name"] = dtSPInfo.Rows[j]["vcCaseno_name"].ToString();
                        dataRow["vcInno"] = dtSPInfo.Rows[j]["vcInno"].ToString();
                        dataRow["vcPart_id"] = dtSPInfo.Rows[j]["vcPart_id"].ToString();
                        dataRow["vcPartsname"] = dtSPInfo.Rows[j]["vcPartsname"].ToString();
                        dataRow["iQty"] = dtSPInfo.Rows[j]["iQty"].ToString();
                        dataRow["iTotalcnt"] = dtSPInfo.Rows[j]["iTotalcnt"].ToString();
                        dataRow["iTotalpiece"] = dtSPInfo.Rows[j]["iTotalpiece"].ToString();
                        dataRow["dPrintDate"] = dtSPInfo.Rows[j]["dPrintDate"].ToString();
                        byte[] vcCodemage = fS0617_Logic.GenerateQRCode(dtSPInfo.Rows[j]["vcCasenoocde"].ToString());//二维码信息
                        dataRow["vcCodemage"] = vcCodemage;
                        dtSub.Rows.Add(dataRow);
                    }
                }
                if (dtSub.Rows.Count == 0)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "没有有效的装箱单数据，请确认后再操作。";
                    dtMessage.Rows.Add(dataRow);
                }
                if (dtMessage.Rows.Count != 0)
                    return false;
                fs1102_DataAccess.setPrintTemp(dtSub, strOperId, ref dtMessage);
                if (dtMessage.Rows.Count != 0)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
