using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS1402_Logic
    {
        FS1402_DataAccess fs1402_DataAccess;

        public FS1402_Logic()
        {
            fs1402_DataAccess = new FS1402_DataAccess();
        }
        public DataTable getSearchInfo(string strCheckQf, string strPartNo, string strSuplier, string strSuplierPlant)
        {
            return fs1402_DataAccess.getSearchInfo(strCheckQf, strPartNo, strSuplier, strSuplierPlant);
        }
        public DataTable getSubInfo(string strLinid)
        {
            return fs1402_DataAccess.getSubInfo(strLinid);
        }
    }
}
