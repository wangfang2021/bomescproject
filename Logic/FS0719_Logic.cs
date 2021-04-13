using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS0719_Logic
    {
        FS0719_DataAccess FS0719_DataAccess;

        public FS0719_Logic()
        {
            FS0719_DataAccess = new FS0719_DataAccess();
        }


        public DataTable SearchSupplier()
        {
            return FS0719_DataAccess.SearchSupplier();
        }

        #region 按检索条件检索,返回dt,注意这个dt返回的时候convert了
        public DataTable Search()
        {
            return FS0719_DataAccess.Search();
        }
        #endregion

        #region 
        public DataTable Search_F()
        {
            return FS0719_DataAccess.Search_F();
        }
        #endregion


        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
    
            DataTable dtbase = FS0719_DataAccess.SearchBase();

            DataTable dtFaZhuTime = FS0719_DataAccess.SearchFaZhuTime();

            DataTable dtCode = FS0719_DataAccess.SearchCode(strUserId);

            FS0719_DataAccess.Save(listInfoData, strUserId, ref strErrorPartId, dtbase, dtFaZhuTime, dtCode);
        }
        #endregion


        #region 恢复
        public void recover(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            FS0719_DataAccess.recover(listInfoData, strUserId, ref strErrorPartId);
        }
        #endregion





        #region 导入后保存
        public void importSave(DataTable dt, string strUserId,ref string strErrorPartId)
        {
            DataTable dtOrderNO = FS0719_DataAccess.Search();
            DataTable dtbase = FS0719_DataAccess.SearchBase();
            DataTable dtFaZhuTime = FS0719_DataAccess.SearchFaZhuTime();
            DataTable dtCode = FS0719_DataAccess.SearchCode(strUserId);
            FS0719_DataAccess.importSave(dt, strUserId,dtbase, dtOrderNO, ref strErrorPartId,dtFaZhuTime, dtCode);
        }
        #endregion

        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            FS0719_DataAccess.Del(listInfoData, strUserId);
        }
        #endregion

        #region 
        public DataTable getSupplier()
        {
            return FS0719_DataAccess.getSupplier();
        }
        #endregion

        #region 按检索条件检索,返回dt
        public DataTable Search_GS(string strBegin, string strEnd)
        {
            return FS0719_DataAccess.Search_GS(strBegin, strEnd);
        }
        #endregion

        #region 保存
        public void Save_GS(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorName)
        {
            FS0719_DataAccess.Save_GS(listInfoData, strUserId, ref strErrorName);
        }

        #endregion

        #region 查找工厂
        public DataTable SearchPackSpot()
        {
            return FS0719_DataAccess.SearchPackSpot();
        }

        #endregion


        #region 订单发注
        public void SaveFZ(DataTable dt, string userId, ref string strErrorPartId)
        {
            FS0719_DataAccess.SaveFZ(dt, userId, ref strErrorPartId);
        }

        #endregion

    }
}
