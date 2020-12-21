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

        #region 绑定供应商
        public DataTable getAllSupplier()
        {
            return fs0807_DataAccess.getAllSupplier();
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

        #region 品番+开始时间+收货方 不能重复
        public bool RepeatCheck(string strPart_id,string strTimeFrom,string strSHF)
        {
            int num = fs0807_DataAccess.RepeatCheck(strPart_id,strTimeFrom,strSHF);
            if(num>0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion 

        public bool DateRegionCheck(string strPart_id, string strSHF, string strTimeFrom, string strTimeTo, string strMode, string strAutoId)
        {
            int num = fs0807_DataAccess.DateRegionCheck(strPart_id, strSHF, strTimeFrom, strTimeTo, strMode, strAutoId);
            if (num > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

}
