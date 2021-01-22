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

        #region 检索

        public DataTable SearchApi(string fileNameTJ)
        {
            return fs0302_dataAccess.SearchApi(fileNameTJ);
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs0302_dataAccess.Save(listInfoData, strUserId);
        }
        #endregion

        //织入原单位
        public void weaveUnit(List<Dictionary<string, Object>> listInfoData, string strUserId, string SYTCode)
        {
            fs0302_dataAccess.weaveUnit(listInfoData, strUserId, SYTCode);
        }
    }
}
