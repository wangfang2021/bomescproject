using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS0405_Logic
    {
        FS0405_DataAccess fs0405_DataAccess;

        public FS0405_Logic()
        {
            fs0405_DataAccess = new FS0405_DataAccess();
        }

        #region 检索
        public DataTable Search(string strDXDateMonth, string strFileName, string strInOutFlag, string strState)
        {
            return null;
            //return fs0405_DataAccess.Search(strDXDateMonth, strFileName, strInOutFlag, strState);
        }
        #endregion
    }
}
