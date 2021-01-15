using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Common;
using DataAccess;

namespace Logic
{
    public class FS0401_Logic
    {
        FS0401_DataAccess fs0401_dataAccess = new FS0401_DataAccess();

        #region 检索

        public DataTable searchApi(string state, string TimeFrom, string TimeTo, string carType, string InOut, string DHFlag)
        {
            return fs0401_dataAccess.searchApi(state, TimeFrom, TimeTo, carType, InOut, DHFlag);
        }


        #endregion

    }

}
