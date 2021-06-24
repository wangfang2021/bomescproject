using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS0707_Logic
    {
        FS0707_DataAccess FS0707_DataAccess;

        public FS0707_Logic()
        {
            FS0707_DataAccess = new FS0707_DataAccess();
        }

        #region 供应商
        public DataTable SearchSupplier()
        {
            return FS0707_DataAccess.SearchSupplier();
        }
        #endregion

        #region 工厂
        public DataTable SearchPartPlant()
        {
            return FS0707_DataAccess.SearchPartPlant();
        }
        #endregion

        #region 工程下拉
        public DataTable SearchKind(List<Object> strPartPlant)
        {
            return FS0707_DataAccess.SearchKind(strPartPlant);
        }
        #endregion

        #region 按检索条件检索,返回dt,注意这个dt返回的时候convert了
        public DataTable SearchCalculation(string dFromBegin,string dFromEnd, List<Object> Project,string Kind, List<Object> OrderState, List<Object> OrderPartPlant)
        {
            return FS0707_DataAccess.SearchCalculation(dFromBegin, dFromEnd, Project, Kind, OrderState, OrderPartPlant);
        }
        #endregion

        #region 保存
        public void Save(DataTable dt, string strUserId,ref string strErrorPartId,string  strBegin,string strEnd,string strFromBeginBZ,string strFromEndBZ,string strKind, List<Object> OrderState)
        {
            FS0707_DataAccess.Save(dt, strUserId,ref strErrorPartId, strBegin, strEnd, strFromBeginBZ, strFromEndBZ, strKind, OrderState);
        }
        #endregion

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId)
        {
            FS0707_DataAccess.importSave(dt, strUserId);
        }
        #endregion

        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            FS0707_DataAccess.Del(listInfoData, strUserId);
        }
        #endregion

        #region 
        public DataTable getSupplier()
        {
            return FS0707_DataAccess.getSupplier();
        }
        #endregion

        #region 按检索条件检索,返回dt
        public DataTable Search_GS(string strBegin, string strEnd )
        {
            return FS0707_DataAccess.Search_GS(strBegin, strEnd );
        }
        #endregion

        #region 保存
        public void Save_GS(DataTable listInfoData, string strUserId, ref string strErrorName,string strBegin,string strEnd, List<Object> OrderState)
        {
            FS0707_DataAccess.Save_GS(listInfoData, strUserId, ref strErrorName, strBegin, strEnd,OrderState);
        }

        #endregion

        #region 查找工厂
        public DataTable SearchPackSpot()
        {
            return FS0707_DataAccess.SearchPackSpot();
        }
        #endregion

        public DataTable Search()
        {
            return FS0707_DataAccess.Search();
        }

        public void Updateprint()
        {
            FS0707_DataAccess.Updateprint();
        }
    }
}
