using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0807_Logic
    {
        FS0807_DataAccess fs0807_DataAccess;

        public FS0807_Logic()
        {
            fs0807_DataAccess = new FS0807_DataAccess();
        }

        #region 绑定工区
        public DataTable bindGQ()
        {
            return fs0807_DataAccess.bindGQ();
        }
        #endregion

        #region 绑定供应商
        public DataTable bindSupplier()
        {
            return fs0807_DataAccess.bindSupplier();
        }
        #endregion

        #region 绑定收货方
        public DataTable bindSHF()
        {
            return fs0807_DataAccess.bindSHF();
        }
        #endregion

        #region 检索
        public DataTable Search(string vcGQ, string vcSupplier_id, string vcSHF, string vcPart_id, string vcCarType, string vcTimeFrom, string vcTimeTo)
        {
            return fs0807_DataAccess.Search(vcGQ, vcSupplier_id, vcSHF, vcPart_id, vcCarType, vcTimeFrom, vcTimeTo);
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs0807_DataAccess.Save(listInfoData, strUserId);
        }
        #endregion

        #region 删除
        public void Del(List<Dictionary<string, Object>> checkedInfoData, string strUserId)
        {
            fs0807_DataAccess.Del(checkedInfoData, strUserId);
        }
        #endregion
    }
}
