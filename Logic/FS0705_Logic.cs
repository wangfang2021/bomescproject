using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS0705_Logic
    {
        FS0705_DataAccess FS0705_DataAccess;

        public FS0705_Logic()
        {
            FS0705_DataAccess = new FS0705_DataAccess();
        }


        public DataTable SearchSupplier()
        {
            return FS0705_DataAccess.SearchSupplier();
        }

        #region 按检索条件检索,
        public DataTable Search_TiaoZheng(string PackGPSNo, string PackNo, string TiaoZhengType, string dFromB, string dToE)
        {
            return FS0705_DataAccess.Search_TiaoZheng(PackGPSNo, PackNo, TiaoZhengType, dFromB, dToE);
        }
        #endregion

        #region 保存
        public void Save_TiaoZheng(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            FS0705_DataAccess.Save_TiaoZheng(listInfoData, strUserId, ref strErrorPartId);
        }
        #endregion

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId)
        {
            FS0705_DataAccess.importSave(dt, strUserId);
        }
        #endregion

        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            FS0705_DataAccess.Del(listInfoData, strUserId);
        }
        #endregion

        #region 
        public DataTable getSupplier()
        {
            return FS0705_DataAccess.getSupplier();
        }
        #endregion

        #region 按检索条件检索,返回dt
        public DataTable Search_GS(string strBegin, string strEnd)
        {
            return FS0705_DataAccess.Search_GS(strBegin, strEnd);
        }
        #endregion

        #region 保存
        public void Save_GS(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorName)
        {
            FS0705_DataAccess.Save_GS(listInfoData, strUserId, ref strErrorName);
        }
        #endregion


        #region 发注便次更新
        public DataTable SearchFaZhuTime(string PackSpot)
        {
            try
            {
                DataTable dtFZTime = new DataTable();
                //获取当前时间所有维护的逻辑
                dtFZTime = FS0705_DataAccess.SearchFZTime(PackSpot);
                //插入临时表
                FS0705_DataAccess.InsertFZ_Temp(dtFZTime);

                //  剩下所有待发送
                //select* from TPack_FaZhuBC_Temp where vcFaZhuBCID + vcFaZhuBCID + CONVERT(varchar(19), dFaZhuStart, 120) not in(
                //     select vcFaZhuBCID + vcFaZhuBCID + CONVERT(varchar(19), dFaZhuPlan, 120) from TPack_FaZhu_BCSJ
                //     )

                return dtFZTime;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


    }
}
