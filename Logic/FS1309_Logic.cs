using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS1309_Logic
    {
        FS1309_DataAccess fs1309_DataAccess;

        public FS1309_Logic()
        {
            fs1309_DataAccess = new FS1309_DataAccess();
        }
        public DataTable getPlantInfo()
        {
            return fs1309_DataAccess.getPlantInfo();
        }
        public DataTable getSearchInfo(string strPlant)
        {
            return fs1309_DataAccess.getSearchInfo(strPlant);
        }
        public void Save(DataTable dataTable, string strOperId)
        {
            fs1309_DataAccess.saveDataInfo(dataTable, strOperId);
        }
    }
}
