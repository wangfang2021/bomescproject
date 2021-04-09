using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS0701_Logic
    {
        FS0701_DataAccess FS0701_DataAccess;

        public FS0701_Logic()
        {
            FS0701_DataAccess = new FS0701_DataAccess();
        }


        public DataTable SearchSupplier()
        {
            return FS0701_DataAccess.SearchSupplier();
        }

        public DataTable SearchFZLJ()
        {
            return FS0701_DataAccess.SearchFZLJ();
        }


        #region 按检索条件检索,返回dt,注意这个dt返回的时候convert了
        public DataTable Search(List<object> PackSpot, string PackNo, string PackGPSNo, List<Object> strSupplierCode, string dFromB, string dFromE, string dToB, string dToE)
        {
            return FS0701_DataAccess.Search(PackSpot, PackNo, PackGPSNo, strSupplierCode, dFromB, dFromE, dToB, dToE);
        }
        #endregion

        #region 按检索条件检索,返回dt
        public DataTable Search_1()
        {
            return FS0701_DataAccess.Search_1();
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId,ref string strErrorPartId)
        {
            FS0701_DataAccess.Save(listInfoData, strUserId,ref strErrorPartId);
        }
        #endregion


        #region checkTime
        public DataTable searchcheckTime(string vcPackSpot,string strPackNo,string dFrom,string dTo,int iAutoId)
        {
            return FS0701_DataAccess.searchcheckTime(vcPackSpot,strPackNo, dFrom, dTo, iAutoId);
        }


        #endregion

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId)
        {
            FS0701_DataAccess.importSave(dt, strUserId);
        }
        #endregion

        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            FS0701_DataAccess.Del(listInfoData, strUserId);
        }
        #endregion

        #region 
        public DataTable getSupplier()
        {
            return FS0701_DataAccess.getSupplier();
        }
        #endregion

        #region 按检索条件检索,返回dt
        public DataTable Search_GS(string strBegin, string strEnd )
        {
            return FS0701_DataAccess.Search_GS(strBegin, strEnd );
        }
        #endregion

        #region 保存
        public void Save_GS(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorName)
        {
            FS0701_DataAccess.Save_GS(listInfoData, strUserId, ref strErrorName);
        }

        #endregion

        #region 查找工厂
        public DataTable SearchPackSpot(string userid)
        {
            return FS0701_DataAccess.SearchPackSpot(userid);
        }
        #endregion

    }
}
