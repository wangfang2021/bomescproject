using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS0703_Logic
    {
        FS0703_DataAccess FS0703_DataAccess;

        public FS0703_Logic()
        {
            FS0703_DataAccess = new FS0703_DataAccess();
        }


        public DataTable SearchSupplier()
        {
            return FS0703_DataAccess.SearchSupplier();
        }

        public DataTable SearchException()
        {
            return FS0703_DataAccess.SearchException();
        }

        public DataTable SearchExceptionCK()
        {
            return FS0703_DataAccess.SearchExceptionCK();
        }


        #region 计算
        public DataTable Calculation(List<Object> PackSpot, string PackFrom, List<Object> strSupplierCode)
        {
            return FS0703_DataAccess.Calculation(PackSpot, PackFrom, strSupplierCode);
        }
        #endregion

    


        #region 插入品番错误
        public void InsertCheck(DataTable drImport, string strUserId)
        {
            FS0703_DataAccess.InsertCheck(drImport, strUserId);
        }
        #endregion



        #region 
        public DataTable getSupplier()
        {
            return FS0703_DataAccess.getSupplier();
        }
        #endregion

        #region 按检索条件检索,返回dt
        public DataTable Search(List<Object> PackSpot, string PackFrom, List<Object> strSupplierCode)
        {
            return FS0703_DataAccess.Search( PackSpot,  PackFrom, strSupplierCode);
        }
        #endregion


        #region 按检索条件检索,返回dt
        public DataTable SearchCheck()
        {
            return FS0703_DataAccess.SearchCheck();
        }
        #endregion

        #region 保存
        public void Save_GS(DataTable listInfoData, string strUserId, ref string strErrorName)
        {
            FS0703_DataAccess.Save_GS(listInfoData, strUserId, ref strErrorName);
        }

        #endregion

        #region 查找工厂
        public DataTable SearchPackSpot()
        {
            return FS0703_DataAccess.SearchPackSpot();
        }


        #endregion


        #region 发送
        public void Save(DataTable listInfoData, string userId, ref string strErrorPartId,string PackFrom, List<Object> SupplierCodeList, List<Object> PackSpot)
        {
            FS0703_DataAccess.Save(listInfoData, userId, ref strErrorPartId,PackFrom,SupplierCodeList, PackSpot);
        }

        public void Updateprint()
        {
            FS0703_DataAccess.Updateprint();
        }
        #endregion
    }
}
