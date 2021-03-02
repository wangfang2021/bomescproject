using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS1401_Logic
    {
        FS1401_DataAccess fs1401_DataAccess;
        FS0603_Logic fS0603_Logic = new FS0603_Logic();

        public FS1401_Logic()
        {
            fs1401_DataAccess = new FS1401_DataAccess();
        }
        public DataTable getSearchInfo(string strPartId, string strSupplierId, string strSupplierPlant, string strHaoJiu, string strInOut, string strPartArea, string strSPISStatus, string strCheckType, string strFrom, string strTo, string strCarModel, string strSPISType, List<Object> listTime)
        {
            try
            {
                DataTable dataTable = fs1401_DataAccess.getSearchInfo(strPartId, strSupplierId, strSupplierPlant, strHaoJiu, strInOut, strPartArea, strSPISStatus, strCheckType, strFrom, strTo, strCarModel, strSPISType, listTime);
                return dataTable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable checkSaveInfo(List<Dictionary<string, Object>> listMultipleData, string strModel, ref bool bReault, ref DataTable dtMessage)
        {
            try
            {
                DataTable dtImport = fS0603_Logic.createTable("updtFs1401");
                string strSPISStatus = "";
                if (strModel == "WCL")
                {
                    strSPISStatus = "未处理";
                }
                if (strModel == "FXW")
                {
                    strSPISStatus = "发行完了";
                }
                if (strModel == "XTD")
                {
                    strSPISStatus = "系统导入";
                }
                #region //整理数据
                for (int i = 0; i < listMultipleData.Count; i++)
                {
                    string strLinId = listMultipleData[i]["LinId"] == null ? "" : listMultipleData[i]["LinId"].ToString();
                    DataRow drImport = dtImport.NewRow();
                    drImport["LinId"] = strLinId;
                    drImport["vcSPISStatus"] = strSPISStatus;
                    dtImport.Rows.Add(drImport);
                }
                #endregion
                return dtImport;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setSaveInfo(DataTable dtImport, string strOperId, ref DataTable dtMessage)
        {
            try
            {
                fs1401_DataAccess.setSaveInfo(dtImport, strOperId, ref dtMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
