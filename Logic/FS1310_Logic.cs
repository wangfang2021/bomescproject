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
        public DataTable getPlantInfo()
        {
            return fs1310_DataAccess.getPlantInfo();
        }
        public DataTable getPinMuInfo()
        {
            return fs1310_DataAccess.getPinMuInfo();
        }
        public DataTable getSearchInfo(string strPlant, string strPinMu, string strPartNo)
        {
            return fs1310_DataAccess.getSearchInfo(strPlant, strPinMu, strPartNo);
        }
        public bool setDeleteInfo(ArrayList delList)
        {
            try
            {
                //返回值定义，默认为失败false
                bool bIsOK = false;
                int count = fs1310_DataAccess.setDeleteInfo(delList);
                if (count > 0)
                    bIsOK = true;
                return bIsOK;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
