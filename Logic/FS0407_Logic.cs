using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Common;
using DataAccess;

namespace Logic
{
    public class FS0407_Logic
    {
        FS0407_DataAccess fs0407_dataAccess = new FS0407_DataAccess();

        #region 检索

        public DataTable SearchApi(string OrderNo, string strPartId)
        {
            return fs0407_dataAccess.searchApi(OrderNo, strPartId);
        }
        #endregion


    }
}
