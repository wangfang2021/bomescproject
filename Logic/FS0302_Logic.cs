using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Common;
using DataAccess;

namespace Logic
{
    public class FS0302_Logic
    {
        FS0302_DataAccess fs0302_dataAccess = new FS0302_DataAccess();

        #region 初始化

        public DataTable getFinishState()
        {
            return fs0302_dataAccess.getFinishState();
        }
        public DataTable getChange()
        {
            return fs0302_dataAccess.getChange();
        }
        public DataTable SearchApi(string fileNameTJ)
        {
            return fs0302_dataAccess.SearchApi(fileNameTJ);
        }

        #endregion

        //织入原单位
        public void weaveUnit(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs0302_dataAccess.weaveUnit(listInfoData, strUserId);
        }
    }
}
