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
        FS0603_Logic fS0603_Logic = new FS0603_Logic();

        public FS1301_Logic()
        {
            fs1301_DataAccess = new FS1301_DataAccess();
        }
        public DataTable getSearchInfo(string strPackingPlant, string strUser, string strRoler)
        {
            return fs1301_DataAccess.getSearchInfo(strPackingPlant, strUser, strRoler);
        }
        public void saveDataInfo(List<Dictionary<string, Object>> listInfoData, string strOperId)
        {
            DataTable dataTable = fS0603_Logic.createTable("Power1301");
            for (int i = 0; i < listInfoData.Count; i++)
            {
                string strLinId = listInfoData[i]["LinId"]==null?"": listInfoData[i]["LinId"].ToString();
                string strPackingPlant = listInfoData[i]["vcPackingPlant"] == null ? "" : listInfoData[i]["vcPackingPlant"].ToString();
                string strUserId = listInfoData[i]["vcUserId"] == null ? "" : listInfoData[i]["vcUserId"].ToString();
                string strInPut = listInfoData[i]["bInPut"].ToString().ToLower() == "true" ? "1" : "0";
                string strInPutUnLock = listInfoData[i]["bInPutUnLock"].ToString().ToLower() == "true" ? "1" : "0";
                string strCheck = listInfoData[i]["bCheck"].ToString().ToLower() == "true" ? "1" : "0";
                string strCheckUnLock = listInfoData[i]["bCheckUnLock"].ToString().ToLower() == "true" ? "1" : "0";
                string strPack = listInfoData[i]["bPack"].ToString().ToLower() == "true" ? "1" : "0";
                string strPackUnLock = listInfoData[i]["bPackUnLock"].ToString().ToLower() == "true" ? "1" : "0";
                string strOutPut = listInfoData[i]["bOutPut"].ToString().ToLower() == "true" ? "1" : "0";
                string strOutPutUnLock = listInfoData[i]["bOutPutUnLock"].ToString().ToLower() == "true" ? "1" : "0";
                DataRow dataRow = dataTable.NewRow();
                dataRow["LinId"] = strLinId;
                dataRow["vcUserId"] = strUserId;
                dataRow["vcPlant"] = strPackingPlant;
                dataRow["vcInPut"] = strInPut;
                dataRow["vcInPutUnLock"] = strInPutUnLock;
                dataRow["vcCheck"] = strCheck;
                dataRow["vcCheckUnLock"] = strCheckUnLock;
                dataRow["vcPack"] = strPack;
                dataRow["vcPackUnLock"] = strPackUnLock;
                dataRow["vcOutPut"] = strOutPut;
                dataRow["vcOutPutUnLock"] = strOutPutUnLock;
                dataTable.Rows.Add(dataRow);
            }
            fs1301_DataAccess.saveDataInfo(dataTable, strOperId);
        }

        public DataTable isCheckUserId(string userId)
        {
            return fs1301_DataAccess.isCheckUserId(userId);
        }

        public void importSave(DataTable importDt, string userId)
        {
             fs1301_DataAccess.importSave(importDt,userId);
        }
    }
}
