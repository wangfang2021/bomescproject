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
        public DataTable getSearchInfo(string strPackPlant, string strPinMu, string strPartId, string strOperImage)
        {
            return fs1310_DataAccess.getSearchInfo(strPackPlant, strPinMu, strPartId, strOperImage);
        }

        public DataTable getSubInfo(string strPlant, string strPartNo)
        {
            return fs1310_DataAccess.getSubInfo(strPlant, strPartNo);
        }
        public void setPackOperImage(string strPartId, string strPinMu, string strPackPlant, string strOperImage, string strOperId)
        {
            try
            {
                fs1310_DataAccess.setPackOperImage(strPartId, strPinMu, strPackPlant, strOperImage, strOperId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void deleteInfo(List<Dictionary<string, Object>> listInfoData)
        {
            try
            {
                fs1310_DataAccess.deleteInfo(listInfoData);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
