using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Common;
using DataAccess;

namespace Logic
{
    public class FS0314_Logic
    {
        FS0314_DataAccess fs0314_dataAccess = new FS0314_DataAccess();

        public DataTable searchApi(string vcSupplier_id, string vcSupplier_name)
        {
            return fs0314_dataAccess.searchApi(vcSupplier_id, vcSupplier_name);
        }

        #region 删除
        public void delSPI(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs0314_dataAccess.delSPI(listInfoData, strUserId);
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            fs0314_dataAccess.Save(listInfoData, strUserId, ref strErrorPartId);
        }
        #endregion

        #region 导入保存

        public void importSave(DataTable dt, string userId)
        {
            fs0314_dataAccess.importSave(dt, userId);
        }

        #endregion
    }
}
