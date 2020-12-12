using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS1102_Logic
    {
        FS1102_DataAccess fs1102_DataAccess;

        public FS1102_Logic()
        {
            fs1102_DataAccess = new FS1102_DataAccess();
        }
        public DataTable getSearchInfo(string strReParty, string strPackingNo, string strTagNo)
        {
            return fs1102_DataAccess.getSearchInfo(strReParty, strPackingNo, strTagNo);
        }
        public string getPrintFile()
        {
            return "";
        }
    }
}
