using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS1101_Logic
    {
        FS1101_DataAccess fs1101_DataAccess;

        public FS1101_Logic()
        {
            fs1101_DataAccess = new FS1101_DataAccess();
        }
        public DataTable getSearchInfo(string strPackMaterNo, string strTrolleyNo, string strPartId, string strOrderNo, string strLianFan)
        {
            return fs1101_DataAccess.getSearchInfo(strPackMaterNo, strTrolleyNo, strPartId, strOrderNo, strLianFan);
        }
        public bool getPrintInfo(List<Dictionary<string, Object>> listInfoData, string strOperId, ref DataTable dtMessage)
        {
            try
            {
                DataTable dataTable = fs1101_DataAccess.getPrintTemp("FS1101");
                DataTable dtSub = dataTable.Clone();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    string strPackMaterNo = listInfoData[i]["vcPackMaterNo"] == null ? "" : listInfoData[i]["vcPackMaterNo"].ToString();
                    DataTable dtSPInfo = fs1101_DataAccess.getSearchInfo(strPackMaterNo);
                    string uuid = Guid.NewGuid().ToString("N");
                    for (int j = 0; j < dtSPInfo.Rows.Count; j++)
                    {
                        DataRow dataRow = dtSub.NewRow();
                        dataRow["UUID"] = uuid;
                        dataRow["vcTrolleyNo"] = dtSPInfo.Rows[j]["vcTrolleyNo"].ToString();
                        dataRow["vcLotid"] = dtSPInfo.Rows[j]["vcLotid"].ToString();
                        dataRow["vcPackingpartsno"] = dtSPInfo.Rows[j]["vcPackingpartsno"].ToString();
                        dataRow["vcPackinggroup"] = dtSPInfo.Rows[j]["vcPackinggroup"].ToString();
                        dataRow["dQty"] = dtSPInfo.Rows[j]["dQty"].ToString();
                        dataRow["vcPackingpartslocation"] = dtSPInfo.Rows[j]["vcPackingpartslocation"].ToString();
                        dataRow["dPrintDate"] = dtSPInfo.Rows[j]["dPrintDate"].ToString();
                        dataRow["vcCodemage"] = null;
                        dtSub.Rows.Add(dataRow);
                    }
                }
                if (dtSub.Rows.Count == 0)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "没有有效的断取数据，请确认后再操作。";
                    dtMessage.Rows.Add(dataRow);
                }
                if (dtMessage.Rows.Count != 0)
                    return false;
                fs1101_DataAccess.setPrintTemp(dtSub, strOperId, ref dtMessage);
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
