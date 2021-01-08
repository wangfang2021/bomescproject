using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS0617_Logic
    {
        FS0617_DataAccess fs0617_DataAccess;

        public FS0617_Logic()
        {
            fs0617_DataAccess = new FS0617_DataAccess();
        }
        public DataTable getSuPartyInfo()
        {
            return fs0617_DataAccess.getSuPartyInfo();
        }
        public DataTable getRePartyInfo()
        {
            return fs0617_DataAccess.getRePartyInfo();
        }
        public DataTable getSearchInfo(string strPlantArea, string strPlant, string strPartId, string strCarType, string strReParty, string strSuparty)
        {
            return fs0617_DataAccess.getSearchInfo(strPlantArea, strPlant, strPartId, strCarType, strReParty, strSuparty);
        }
        public DataTable getPrintInfo(List<Dictionary<string, Object>> listInfoData)
        {
            string strParameter = "";
            if (listInfoData.Count != 0)
            {
                strParameter += "select '";
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    strParameter += listInfoData[i]["LinId"].ToString();
                    if (i < listInfoData.Count - 1)
                        strParameter += "' union select '";
                    else
                        strParameter += "'";
                }
                DataTable dataTable = fs0617_DataAccess.getPrintInfo(strParameter);
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    string strLinId = dataTable.Rows[i]["LinId"].ToString();
                    foreach (var item in listInfoData)
                    {
                        if(item["LinId"].ToString()== strLinId)
                        {
                            dataTable.Rows[i]["Enum"] = item["Enum"].ToString();
                        }
                    }
                }
                return dataTable;
            }
            else
            {
                return null;
            }
        }
        public string getPrintFile(string strPlant, string strPartid, string strCarType, string strReParty, string strSuParty)
        {
            return "";
        }

    }
}
