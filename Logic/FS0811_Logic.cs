using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS0811_Logic
    {
        FS0811_DataAccess fs0811_DataAccess;

        public FS0811_Logic()
        {
            fs0811_DataAccess = new FS0811_DataAccess();
        }
        public DataTable getSearchInfo(string strDate)
        {
            return fs0811_DataAccess.getSearchInfo(strDate);
        }
        public string QueryInfo(string strSaleno)
        {
            return "";
        }
        public string SaveInfo(string strSaleno)
        {
            return "";
        }
    }
}
