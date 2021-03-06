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
    public class FS0406_Logic
    {
        FS0406_DataAccess fs0406_dataAccess = new FS0406_DataAccess();
        #region 检索

        public DataTable searchApi(string vcReceiver, string vcType, string vcState, string start, string end)
        {
            return fs0406_dataAccess.searchApi(vcReceiver, vcType, vcState, start, end);
        }
        #endregion


    }
}
