using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS0710_Logic
    {
        FS0710_DataAccess FS0710_DataAccess;

        public FS0710_Logic()
        {
            FS0710_DataAccess = new FS0710_DataAccess();
        }


        public DataTable SearchSupplier()
        {
            return FS0710_DataAccess.SearchSupplier();
        }



        #region 按检索条件检索,返回dt,注意这个dt返回的时候convert了
        public DataTable Search_NR(List<Object> PackSpot, List<Object> strSupplierCode, string dFrom, string dTo)
        {
            return FS0710_DataAccess.Search_NR(PackSpot, strSupplierCode, dFrom, dTo);
        }
        #endregion

        #region 按检索条件检索,返回dt,注意这个dt返回的时候convert了（订单）
        public DataTable Search_DD(List<Object> PackSpot, List<Object> strSupplierCode, string dFrom, string dTo)
        {
            return FS0710_DataAccess.Search_DD(PackSpot, strSupplierCode, dFrom, dTo);
        }
        #endregion

        #region 保存
        public void Save_NR(DataTable listInfoData, ref string strErrorPartId,string UserId)
        {
           FS0710_DataAccess.Save_NR(listInfoData, ref strErrorPartId,UserId);
        }
        #endregion

        #region 保存（订单）
        public void Save_DD(DataTable listInfoData, ref string strErrorPartId)
        {
            FS0710_DataAccess.Save_DD(listInfoData, ref strErrorPartId);
        }
        #endregion



        #region 
        public DataTable getSupplier()
        {
            return FS0710_DataAccess.getSupplier();
        }
        #endregion

        #region 按检索条件检索,返回dt
        public DataTable Search_NaRu_export()
        {
            return FS0710_DataAccess.Search_NaRu_export();
        }
        #endregion

        #region 按检索条件检索,返回dt（订单）
        public DataTable Search_DingDan_export()
        {
            return FS0710_DataAccess.Search_DingDan_export();
        }
        #endregion

        #region 保存
        public void Save_GS(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorName)
        {
            FS0710_DataAccess.Save_GS(listInfoData, strUserId, ref strErrorName);
        }
        #endregion

        #region 生成水晶报表数据
        public void InsertCaystal(DataTable dt,string PackSpot,string strSupplierCode,string  dFrom,string dTo)
        {
            FS0710_DataAccess.Save_Caystal(dt, PackSpot, strSupplierCode, dFrom, dTo);
        }
        #endregion


        #region 
        public DataTable SearchNRCaystal(string strSupplierCode, string PackSpot)
        {
            return FS0710_DataAccess.SearchNRCaystal(strSupplierCode, PackSpot);
        }
        #endregion
    }
}
