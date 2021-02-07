using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Common;
using DataAccess;

namespace Logic
{
    public class FS0311_Logic
    {
        FS0311_DataAccess fs0311_dataAccess = new FS0311_DataAccess();

        public DataTable searchApi(string strPartid, string strSHF, bool vcFlag)
        {
            return fs0311_dataAccess.searchApi(strPartid, strSHF, vcFlag);
        }

        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            fs0311_dataAccess.Save(listInfoData, strUserId, ref strErrorPartId);
        }

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId)
        {
            fs0311_dataAccess.importSave(dt, strUserId);
        }
        #endregion
    }
}
