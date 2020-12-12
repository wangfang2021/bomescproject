using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS1103_Logic
    {
        FS1103_DataAccess fs1103_DataAccess;

        public FS1103_Logic()
        {
            fs1103_DataAccess = new FS1103_DataAccess();
        }
        public DataTable getSearchInfo(string strReParty, string strPartId, string strTianFan)
        {
            return fs1103_DataAccess.getSearchInfo(strReParty, strPartId, strTianFan);  
        }
        public string getPrintFile()
        {
            return "";
        }
    }
}
