using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS0715_Logic
    {
        FS0715_DataAccess FS0715_DataAccess;

        public FS0715_Logic()
        {
            FS0715_DataAccess = new FS0715_DataAccess();
        }


        #region 按检索条件检索,返回dt,注意这个dt返回的时候convert了
        public DataTable Search(string  PackSpot, string YearMonth)
        {
            return FS0715_DataAccess.Search(PackSpot, YearMonth);
        }
        #endregion

        #region 导出检索
        public DataTable Search_EX(string PackSpot, string YearMonth)
        {
            return FS0715_DataAccess.Search_EX(PackSpot, YearMonth);
        }
        #endregion

        #region 班制检索
        public DataTable Search_BZ(string PackSpot)
        {
            return FS0715_DataAccess.Search_BZ(PackSpot);
        }
        #endregion



        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId,ref string strErrorPartId)
        {
            FS0715_DataAccess.Save(listInfoData, strUserId,ref strErrorPartId);
        }
        #endregion

        #region 保存班制
        public void Save_BZ(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            FS0715_DataAccess.Save_BZ(listInfoData, strUserId, ref strErrorPartId);
        }
        #endregion



        #region 导入后保存
        public void importSave(DataTable dt, string strUserId)
        {
            FS0715_DataAccess.importSave(dt, strUserId);
        }
        #endregion

        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            FS0715_DataAccess.Del(listInfoData, strUserId);
        }
        #endregion

        #region 查找工厂
        public DataTable SearchPackSpot()
        {
            return FS0715_DataAccess.SearchPackSpot();
        }
        #endregion


    }
}
