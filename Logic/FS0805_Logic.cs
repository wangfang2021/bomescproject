using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS0805_Logic
    {
        FS0805_DataAccess fs0805_DataAccess;
        FS0617_Logic fS0617_Logic = new FS0617_Logic();

        public FS0805_Logic()
        {
            fs0805_DataAccess = new FS0805_DataAccess();
        }
        public DataTable getSearchInfo(string strSellNo)
        {
            return fs0805_DataAccess.getSearchInfo(strSellNo, "Info");
        }
        public bool getPrintInfo(string strSellNo, string strOperId, ref DataTable dtMessage)
        {
            try
            {
                DataTable dataTable = fs0805_DataAccess.getPrintTemp("FS0805");
                DataTable dtSub = dataTable.Clone();
                DataTable dtSPInfo = fs0805_DataAccess.getSearchInfo(strSellNo,"List");
                string uuid = Guid.NewGuid().ToString("N");
                for (int j = 0; j < dtSPInfo.Rows.Count; j++)
                {
                    DataRow dataRow = dtSub.NewRow();
                    dataRow["UUID"] = uuid;
                    dataRow["vcCpdcompany"] = dtSPInfo.Rows[j]["vcCpdcompany"].ToString();
                    dataRow["vcCompany"] = dtSPInfo.Rows[j]["vcCompany"].ToString();
                    dataRow["vcPackingspot"] = dtSPInfo.Rows[j]["vcPackingspot"].ToString();
                    dataRow["vcControlno"] = dtSPInfo.Rows[j]["vcControlno"].ToString();
                    dataRow["vcPartsno"] = dtSPInfo.Rows[j]["vcPartsno"].ToString();
                    dataRow["vcOrderno"] = dtSPInfo.Rows[j]["vcOrderno"].ToString();
                    dataRow["vcSeqno"] = dtSPInfo.Rows[j]["vcSeqno"].ToString();
                    dataRow["vcInvoiceno"] = dtSPInfo.Rows[j]["vcInvoiceno"].ToString();
                    dataRow["vcPartsnamechn"] = dtSPInfo.Rows[j]["vcPartsnamechn"].ToString();
                    dataRow["vcPartsnameen"] = dtSPInfo.Rows[j]["vcPartsnameen"].ToString();
                    dataRow["vcShippingqty"] = dtSPInfo.Rows[j]["vcShippingqty"].ToString();
                    dataRow["vcCaseno"] = dtSPInfo.Rows[j]["vcCaseno"].ToString();
                    dataRow["vcCostwithtaxes"] = dtSPInfo.Rows[j]["vcCostwithtaxes"].ToString();
                    dataRow["vcPrice"] = dtSPInfo.Rows[j]["vcPrice"].ToString();
                    dataRow["dPrintDate"] = dtSPInfo.Rows[j]["dPrintDate"].ToString();
                    dataRow["vcCodemage"] = null;
                    dtSub.Rows.Add(dataRow);
                }
                if (dtSub.Rows.Count == 0)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "没有有效的发货明细数据，请确认后再操作。";
                    dtMessage.Rows.Add(dataRow);
                }
                if (dtMessage.Rows.Count != 0)
                    return false;
                fs0805_DataAccess.setPrintTemp(dtSub, strOperId, ref dtMessage);
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
