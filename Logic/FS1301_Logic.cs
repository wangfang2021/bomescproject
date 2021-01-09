using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS1301_Logic
    {
        FS1301_DataAccess fs1301_DataAccess;

        public FS1301_Logic()
        {
            fs1301_DataAccess = new FS1301_DataAccess();
        }
        public DataTable getRolerInfo()
        {
            return fs1301_DataAccess.getRolerInfo();
        }
        public DataTable getSearchInfo(string strPlant, string strUser, string strRoler)
        {
            return fs1301_DataAccess.getSearchInfo(strPlant, strUser, strRoler);
        }
        public bool saveDataInfo(List<Dictionary<string, Object>> listInfoData, string strOperId)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("LinId");
            dataTable.Columns.Add("vcChecker");
            dataTable.Columns.Add("vcUnLockChecker");
            dataTable.Columns.Add("vcPacker");
            dataTable.Columns.Add("vcUnLockPacker");
            for (int i = 0; i < listInfoData.Count; i++)
            {
                string strLinId = listInfoData[i]["LinId"].ToString();
                string strChecker = listInfoData[i]["bChecker"].ToString()=="true" ? "1" : "0";
                string strUnLockChecker = listInfoData[i]["bUnLockChecker"].ToString() == "true" ? "1" : "0";
                string strPacker = listInfoData[i]["bPacker"].ToString() == "true" ? "1" : "0";
                string strUnLockPacker = listInfoData[i]["bUnLockPacker"].ToString() == "true" ? "1" : "0";
                DataRow dataRow = dataTable.NewRow();
                dataRow["LinId"] = strLinId;
                dataRow["vcChecker"] = strChecker;
                dataRow["vcUnLockChecker"] = strUnLockChecker;
                dataRow["vcPacker"] = strPacker;
                dataRow["vcUnLockPacker"] = strUnLockPacker;
                dataTable.Rows.Add(dataRow);
            }
            return fs1301_DataAccess.saveDataInfo(dataTable, strOperId);
        }
    }
}
