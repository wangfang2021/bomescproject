using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS1310_Logic
    {
        FS1310_DataAccess fs1310_DataAccess;

        public FS1310_Logic()
        {
            fs1310_DataAccess = new FS1310_DataAccess();
        }
        public DataTable getPinMuInfo()
        {
            return fs1310_DataAccess.getPinMuInfo();
        }
        public DataTable getSearchInfo(string strPlant, string strPinMu, string strPartId)
        {
            return fs1310_DataAccess.getSearchInfo(strPlant, strPinMu, strPartId);
        }
        public void setDeleteInfo(List<Dictionary<string, Object>> listInfoData)
        {
            try
            {
                fs1310_DataAccess.setDeleteInfo(listInfoData);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getSubInfo(string strPlant, string strPartNo)
        {
            return fs1310_DataAccess.getSubInfo(strPlant, strPartNo);
        }

    }
}
