using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS9905_Logic
    {
        FS9905_DataAccess fs9905_DataAccess;

        public FS9905_Logic()
        {
            fs9905_DataAccess = new FS9905_DataAccess();
        }

        #region 检索
        public DataTable Search(string strJD, string strInOutFlag, string strSupplier_id, string strCarType, string strPart_id)
        {
            return fs9905_DataAccess.Search(strJD, strInOutFlag, strSupplier_id, strCarType, strPart_id);
        }
        #endregion

        #region 初始化检索
        public DataTable Search()
        {
            return fs9905_DataAccess.Search();
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErr)
        {
            fs9905_DataAccess.Save(listInfoData, strUserId, ref strErr);
        }
        #endregion

        #region 生确回复
        public void Send(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            fs9905_DataAccess.Send(listInfoData, strUserId, ref strErrorPartId);
        }
        #endregion

        #region 对应可否一括付与
        public void SetFY(List<Dictionary<string, Object>> listInfoData,string strSupplier_BJ, string strSupplier_HK, string strUserId, ref string strErrorPartId)
        {
            //fs9905_DataAccess.SetFY(listInfoData, strSupplier_BJ, strSupplier_HK, strUserId, ref strErrorPartId); 
        }
        #endregion
    }
}
