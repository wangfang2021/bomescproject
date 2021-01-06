using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS0304_Logic
    {
        FS0304_DataAccess FS0304_DataAccess;

        public FS0304_Logic()
        {
            FS0304_DataAccess = new FS0304_DataAccess();
        }

        #region 检索
        public DataTable Search(string strJD, string strInOutFlag, string strSupplier_id, string strCarType, string strPart_id,string strIsDYJG,string strIsDYFX)
        {
            return FS0304_DataAccess.Search(strJD, strInOutFlag, strSupplier_id, strCarType, strPart_id, strIsDYJG, strIsDYFX);
        }
        #endregion

    }
}
