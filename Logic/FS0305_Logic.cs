using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS0305_Logic
    {
        FS0305_DataAccess fs0305_DataAccess;

        public FS0305_Logic()
        {
            fs0305_DataAccess = new FS0305_DataAccess();
        }

        #region 检索
        public DataTable Search(string strJD, string strInOutFlag, string strSupplier_id, string strCarType, string strPart_id)
        {
            return fs0305_DataAccess.Search(strJD, strInOutFlag, strSupplier_id, strCarType, strPart_id);
        }
        #endregion

        #region 生确回复
        public void Send(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            fs0305_DataAccess.Send(listInfoData, strUserId, ref strErrorPartId);
        }
        #endregion

        #region 对应可否一括付与
        public void IsDYJG(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId, string strIsDYJG)
        {
            fs0305_DataAccess.IsDYJG(listInfoData, strUserId, ref strErrorPartId, strIsDYJG);
        }
        #endregion

        #region 防锈区分一括付与
        public void IsDYFX(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId, string strIsDYFX)
        {
            fs0305_DataAccess.IsDYFX(listInfoData, strUserId, ref strErrorPartId,strIsDYFX);
        }
        #endregion
    }
}
