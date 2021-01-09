using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS1404_Logic
    {
        FS1404_DataAccess fs1404_DataAccess;

        public FS1404_Logic()
        {
            fs1404_DataAccess = new FS1404_DataAccess();
        }
        public DataTable getSearchInfo(string strPartNo, string strSuplier, string strSuplierPlant)
        {
            return fs1404_DataAccess.getSearchInfo(strPartNo, strSuplier, strSuplierPlant);
        }
        public DataTable getSubInfo(string strLinid)
        {
            return fs1404_DataAccess.getSubInfo(strLinid);
        }
    }
}
