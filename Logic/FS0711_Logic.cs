using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS0711_Logic
    {
        FS0711_DataAccess FS0711_DataAccess;

        public FS0711_Logic()
        {
            FS0711_DataAccess = new FS0711_DataAccess();
        }


        public DataTable SearchSupplier()
        {
            return FS0711_DataAccess.SearchSupplier();
        }

        #region 按检索条件检索,返回dt,注意这个dt返回的时候convert了
        public DataTable Search(string PackSpot, string PackNo, string PackGPSNo, string Supplier)
        {
            return FS0711_DataAccess.Search(PackSpot, PackNo, PackGPSNo, Supplier);
        }
        #endregion



        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            FS0711_DataAccess.Save(listInfoData, strUserId, ref strErrorPartId);
        }
        #endregion

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId)
        {
            FS0711_DataAccess.importSave(dt, strUserId);
        }
        #endregion

        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            FS0711_DataAccess.Del(listInfoData, strUserId);
        }
        #endregion

        #region 
        public DataTable getSupplier()
        {
            return FS0711_DataAccess.getSupplier();
        }
        #endregion

        #region 按检索条件检索,返回dt
        public DataTable Search_GS(string strBegin, string strEnd)
        {
            return FS0711_DataAccess.Search_GS(strBegin, strEnd);
        }
        #endregion

        #region 保存
        public void Save_GS(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorName)
        {
            FS0711_DataAccess.Save_GS(listInfoData, strUserId, ref strErrorName);
        }
        #endregion


    }
}
