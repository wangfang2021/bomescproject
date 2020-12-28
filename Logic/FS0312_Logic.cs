using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS0312_Logic
    {
        FS0312_DataAccess fs0312_DataAccess;

        public FS0312_Logic()
        {
            fs0312_DataAccess = new FS0312_DataAccess();
        }

        #region 检索
        public DataTable Search(string strPart_id, string strSupplier_id)
        {
            return fs0312_DataAccess.Search(strPart_id,strSupplier_id);
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            fs0312_DataAccess.Save(listInfoData, strUserId, ref strErrorPartId);
        }
        #endregion

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId)
        {
            fs0312_DataAccess.importSave(dt, strUserId);
        }
        #endregion

        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs0312_DataAccess.Del(listInfoData, strUserId);
        }
        #endregion

    }
}
