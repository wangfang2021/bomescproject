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
        public DataTable Search(string strJD, string strInOutFlag, string strSupplier_id, string strCarType, string strPart_id)
        {
            return FS0304_DataAccess.Search(strJD, strInOutFlag, strSupplier_id, strCarType, strPart_id);
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            FS0304_DataAccess.Save(listInfoData, strUserId, ref strErrorPartId);
        }
        #endregion

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId)
        {
            FS0304_DataAccess.importSave(dt, strUserId);
        }
        #endregion

        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            FS0304_DataAccess.Del(listInfoData, strUserId);
        }
        #endregion

    }
}
