using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS0712_Logic
    {
        FS0712_DataAccess FS0712_DataAccess;

        public FS0712_Logic()
        {
            FS0712_DataAccess = new FS0712_DataAccess();
        }


        public DataTable SearchSupplier()
        {
            return FS0712_DataAccess.SearchSupplier();
        }

        #region 按检索条件检索,返回dt,注意这个dt返回的时候convert了
        public DataTable Search(string  PackSpot, string PackNo, string PackGPSNo, List<Object> ZuoYeQuFen, List<Object> PackSupplier, string dFrom, string dTo)
        {
            return FS0712_DataAccess.Search(PackSpot, PackNo, PackGPSNo, ZuoYeQuFen, PackSupplier, dFrom, dTo);
        }
        #endregion

    

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId,ref string strErrorPartId)
        {
            FS0712_DataAccess.Save(listInfoData, strUserId,ref strErrorPartId);
        }
        #endregion

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId)
        {
            FS0712_DataAccess.importSave(dt, strUserId);
        }
        #endregion

        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            FS0712_DataAccess.Del(listInfoData, strUserId);
        }
        #endregion

        #region 
        public DataTable getSupplier()
        {
            return FS0712_DataAccess.getSupplier();
        }
        #endregion

        #region 按检索条件检索,返回dt
        public DataTable Search_GS(string strBegin, string strEnd )
        {
            return FS0712_DataAccess.Search_GS(strBegin, strEnd );
        }
        #endregion

        #region 保存
        public void Save_GS(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorName)
        {
            FS0712_DataAccess.Save_GS(listInfoData, strUserId, ref strErrorName);
        }


        #endregion


  


    }
}
