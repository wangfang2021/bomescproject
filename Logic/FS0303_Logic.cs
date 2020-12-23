using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS0303_Logic
    {
        FS0303_DataAccess fs0303_DataAccess;

        public FS0303_Logic()
        {
            fs0303_DataAccess = new FS0303_DataAccess();
        }

        #region 检索
        public DataTable Search(string strIsShowAll)
        {
            return fs0303_DataAccess.Search(strIsShowAll);
        }
        #endregion

        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs0303_DataAccess.Del(listInfoData, strUserId);
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            fs0303_DataAccess.Save(listInfoData, strUserId, ref strErrorPartId);
        }
        #endregion
    }
}
