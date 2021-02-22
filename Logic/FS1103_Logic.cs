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
        public DataTable getSearchInfo(string strReceiver, string strSupplier, string strInPutOrderNo, string strPartId, string strLianFan)
        {
            return fs1103_DataAccess.getSearchInfo(strReceiver, strSupplier, strInPutOrderNo, strPartId, strLianFan);  
        }
        public string getPrintFile()
        {
            return "";
        }
    }
}
