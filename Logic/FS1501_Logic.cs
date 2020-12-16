using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS1501_Logic
    {
        FS1501_DataAccess fs1501_DataAccess;

        public FS1501_Logic()
        {
            fs1501_DataAccess = new FS1501_DataAccess();
        }

        #region 检索
        public DataTable Search(string vcSupplier_id, string vcGQ, string vcSR, string vcOrderNo, string vcNRBianCi, string vcNRBJSK)
        {
            return fs1501_DataAccess.Search(vcSupplier_id, vcGQ, vcSR, vcOrderNo, vcNRBianCi, vcNRBJSK);
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs1501_DataAccess.Save(listInfoData, strUserId);
        }
        #endregion

        #region 删除
        public void Del(List<Dictionary<string, Object>> checkedInfoData, string strUserId)
        {
            fs1501_DataAccess.Del(checkedInfoData, strUserId);
        }
        #endregion

        #region 取出所有部品纳入补给时刻便次
        public DataTable GetNRBJSKBianCiInfo()
        {
            return fs1501_DataAccess.GetNRBJSKBianCiInfo();
        }
        #endregion

    }
}
