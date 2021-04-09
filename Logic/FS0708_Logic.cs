using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS0708_Logic
    {
        FS0708_DataAccess FS0708_DataAccess;

        public FS0708_Logic()
        {
            FS0708_DataAccess = new FS0708_DataAccess();
        }


        public DataTable SearchSupplier()
        {
            return FS0708_DataAccess.SearchSupplier();
        }

        #region 按检索条件检索,返回dt,注意这个dt返回的时候convert了
        public DataTable Search(List<object> PackSpot, string PackNo, string PackGPSNo,string OrderFrom, string OrderTo, List<object> Type,List<Object> OrderState,string IsQianPin, List<object> SupplierName, string ZuCode, string dFaZhuFrom, string dFaZhuTo, string dNaQiFrom,string dNaQiTo,string dNaRuFrom, string dNaRuTo)
        {
            return FS0708_DataAccess.Search(PackSpot, PackNo, PackGPSNo, OrderFrom, OrderTo, Type, OrderState, IsQianPin, SupplierName, ZuCode, dFaZhuFrom, dFaZhuTo, dNaQiFrom, dNaQiTo, dNaRuFrom, dNaRuTo);
        }
        #endregion

    

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId,ref string strErrorPartId)
        {
            FS0708_DataAccess.Save(listInfoData, strUserId,ref strErrorPartId);
        }
        #endregion

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId)
        {
            FS0708_DataAccess.importSave(dt, strUserId);
        }
        #endregion

        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            FS0708_DataAccess.Del(listInfoData, strUserId);
        }
        #endregion

        #region 
        public DataTable getSupplier()
        {
            return FS0708_DataAccess.getSupplier();
        }
        #endregion

        #region 按检索条件检索,返回dt
        public DataTable Search_GS(string strBegin, string strEnd )
        {
            return FS0708_DataAccess.Search_GS(strBegin, strEnd );
        }
        #endregion

        #region 保存
        public void Save_GS(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorName)
        {
            FS0708_DataAccess.Save_GS(listInfoData, strUserId, ref strErrorName);
        }

        #endregion

        #region 查找工厂
        public DataTable SearchPackSpot()
        {
            return FS0708_DataAccess.SearchPackSpot();
        }
        #endregion

    }
}
