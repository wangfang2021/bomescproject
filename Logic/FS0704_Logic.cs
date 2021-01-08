using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS0704_Logic
    {
        FS0704_DataAccess FS0704_DataAccess;

        public FS0704_Logic()
        {
            FS0704_DataAccess = new FS0704_DataAccess();
        }

        public DataTable SearchSupplier()
        {
            return FS0704_DataAccess.SearchSupplier();
        }
        #region 按检索条件检索,返回dt,注意这个dt返回的时候convert了
        public DataTable Search(string  PackSpot,  string FaZhu, string dFromB, string dFromE, string dToB, string dToE)
        {
            return FS0704_DataAccess.Search(PackSpot, FaZhu, dFromB, dFromE, dToB, dToE);
        }
        #endregion

    

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId,ref string strErrorPartId)
        {
            FS0704_DataAccess.Save(listInfoData, strUserId,ref strErrorPartId);
        }
        #endregion

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId)
        {
            FS0704_DataAccess.importSave(dt, strUserId);
        }
        #endregion

        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            FS0704_DataAccess.Del(listInfoData, strUserId);
        }
        #endregion

        #region 
        public DataTable getSupplier()
        {
            return FS0704_DataAccess.getSupplier();
        }
        #endregion

        #region 按检索条件检索,返回dt
        public DataTable Search_GS(string strBegin, string strEnd )
        {
            return FS0704_DataAccess.Search_GS(strBegin, strEnd );
        }
        #endregion

        #region 保存
        public void Save_GS(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorName)
        {
            FS0704_DataAccess.Save_GS(listInfoData, strUserId, ref strErrorName);
        }
        #endregion


    }
}
