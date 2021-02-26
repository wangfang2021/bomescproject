using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;
using Common;

namespace Logic
{
    public class FS1104_Logic
    {
        FS1104_DataAccess fs1104_DataAccess;
        FS0603_Logic fS0603_Logic = new FS0603_Logic();

        public FS1104_Logic()
        {
            fs1104_DataAccess = new FS1104_DataAccess();
        }
        public string getCaseNoInfo(string strOrderPlant, string strReceiver, string strPackingPlant, string strLianFan)
        {
            return fs1104_DataAccess.getCaseNoInfo(strOrderPlant, strReceiver, strPackingPlant, strLianFan);
        }
        public DataTable setPrintInfo(string strPlant, string strReParty, string strPackPlant, string strCaseLianFanNo, string strPrintNum, string strCaseNo, string strPrintCopy)
        {
            if (strCaseNo == "")
            {
                FS0617_DataAccess fs0617_DataAccess = new FS0617_DataAccess();
                DataTable dataTable = createTable();
                DataTable dtPlantList = ComFunction.getTCode("C000");
                DataTable dtRePartyList = fS0603_Logic.getCodeInfo("Receiver");
                DataTable dtPackPlantList = ComFunction.getTCode("C023");
                for (int i = Convert.ToInt32(strCaseLianFanNo); i < Convert.ToInt32(strPrintNum) + Convert.ToInt32(strCaseLianFanNo); i++)
                {
                    for (int j = 1; j <= Convert.ToInt32(strPrintCopy); j++)
                    {
                        DataRow dataRow = dataTable.NewRow();
                        dataRow["vcPlant"] = strPlant;
                        dataRow["vcPlantName"] = (dtPlantList.Select("vcValue='" + strPlant + "'"))[0]["vcName"].ToString();
                        dataRow["vcReParty"] = strReParty;
                        dataRow["vcRePartyName"] = (dtRePartyList.Select("vcValue='" + strReParty + "'"))[0]["vcName"].ToString();
                        dataRow["vcPackPlant"] = strPackPlant;
                        dataRow["vcPackPlantName"] = "";
                        dataRow["vcPrintNum"] = strPrintNum;
                        dataRow["vcPrintIndex"] = (100000000+Convert.ToInt32(i)).ToString().Substring(1,8);
                        dataTable.Rows.Add(dataRow);
                    }
                }
                return dataTable;

            }
            else
            {
                DataTable dtCaseNoInfo = fs1104_DataAccess.getCaseNoInfo(strCaseNo);
                DataTable dataTable = createTable();
                if (dtCaseNoInfo.Rows.Count != 0)
                {
                    for (int j = 1; j <= Convert.ToInt32(strPrintCopy); j++)
                    {
                        DataRow dataRow = dataTable.NewRow();
                        dataRow["vcPlant"] = dtCaseNoInfo.Rows[0]["vcPlant"].ToString();
                        dataRow["vcPlantName"] = dtCaseNoInfo.Rows[0]["vcPlantName"].ToString();
                        dataRow["vcReParty"] = dtCaseNoInfo.Rows[0]["vcReParty"].ToString();
                        dataRow["vcRePartyName"] = dtCaseNoInfo.Rows[0]["vcRePartyName"].ToString();
                        dataRow["vcPackPlant"] = dtCaseNoInfo.Rows[0]["vcPackPlant"].ToString();
                        dataRow["vcPackPlantName"] = dtCaseNoInfo.Rows[0]["vcPackPlantName"].ToString();
                        dataRow["vcPrintNum"] = dtCaseNoInfo.Rows[0]["vcPrintNum"].ToString();
                        dataRow["vcPrintIndex"] = (100000000 + Convert.ToInt32(dtCaseNoInfo.Rows[0]["vcPrintIndex"].ToString())).ToString().Substring(1, 8);
                        dataTable.Rows.Add(dataRow);
                    }
                }
                return dataTable;

            }
        }

        public void setSaveInfo(DataTable dtImport, ref DataTable dtMessage)
        {
            try
            {
                fs1104_DataAccess.setSaveInfo(dtImport, ref dtMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable createTable()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("vcPlant");
            dataTable.Columns.Add("vcPlantName");
            dataTable.Columns.Add("vcReParty");
            dataTable.Columns.Add("vcRePartyName");
            dataTable.Columns.Add("vcPackPlant");
            dataTable.Columns.Add("vcPackPlantName");
            dataTable.Columns.Add("vcPrintNum");
            dataTable.Columns.Add("vcPrintIndex");
            return dataTable;
        }
        public string getPrintFile()
        {
            return "";
        }
    }
}
