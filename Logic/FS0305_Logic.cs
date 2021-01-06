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
        public DataTable Search(string strSSDateMonth, string strJD, string strPart_id, string strInOutFlag, string strIsDYJG, string strCarType, string strSupplier_id)
        {
            return fs0305_DataAccess.Search(strSSDateMonth, strJD, strPart_id, strInOutFlag, strIsDYJG, strCarType, strSupplier_id);
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            fs0305_DataAccess.Save(listInfoData, strUserId, ref strErrorPartId);
        }
        #endregion

    }
}
