using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Common;
using DataAccess;

namespace Logic
{
    public class FS0306_Logic
    {
        FS0306_DataAccess fs0306_dataAccess = new FS0306_DataAccess();

        public DataTable searchApi(string vcPart_Id, string vcCarType, string vcState, string strUnitCode)
        {
            return fs0306_dataAccess.searchApi(vcPart_Id, vcCarType, vcState, strUnitCode);
        }

        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs0306_dataAccess.Del(listInfoData, strUserId);
        }
        #endregion

        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            fs0306_dataAccess.Save(listInfoData, strUserId, ref strErrorPartId);
        }

        public void InsertSave(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            fs0306_dataAccess.InsertSave(listInfoData, strUserId, ref strErrorPartId);
        }

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId)
        {
            fs0306_dataAccess.importSave(dt, strUserId);
        }
        #endregion
    }
}
