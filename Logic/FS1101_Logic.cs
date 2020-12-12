using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS1101_Logic
    {
        FS1101_DataAccess fs1101_DataAccess;

        public FS1101_Logic()
        {
            fs1101_DataAccess = new FS1101_DataAccess();
        }
        public DataTable getSearchInfo(string strDQuNo, string strTrolleyNo, string strPartId, string strOrderNo, string strLianFan)
        {
            return fs1101_DataAccess.getSearchInfo(strDQuNo, strTrolleyNo, strPartId, strOrderNo, strLianFan);
        }
        public string getPrintFile()
        {
            return "";
        }
    }
}
