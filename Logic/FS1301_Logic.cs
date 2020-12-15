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
        public DataTable getPlantInfo()
        {
            return fs1301_DataAccess.getPlantInfo();
        }
        public DataTable getRolerInfo()
        {
            return fs1301_DataAccess.getRolerInfo();
        }
        public DataTable getSearchInfo(string strPlant, string strUser, string strRoler)
        {
            return fs1301_DataAccess.getSearchInfo(strPlant, strUser, strRoler);
        }
        public void Save(DataTable dataTable, string strOperId)
        {
            fs1301_DataAccess.saveDataInfo(dataTable, strOperId);
        }
    }
}
