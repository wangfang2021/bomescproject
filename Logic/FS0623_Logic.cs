using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0623_Logic
    {
        FS0623_DataAccess fs0623_DataAccess;

        public FS0623_Logic()
        {
            fs0623_DataAccess = new FS0623_DataAccess();

        }

        public DataTable Search_Sub()
        {
            return fs0623_DataAccess.Search_Sub();
        }

        public DataTable CheckDistinctByTable(DataTable dtadd)
        {
            return fs0623_DataAccess.CheckDistinctByTable(dtadd);
        }

        public void Save_Sub(List<Dictionary<string, object>> listInfoData, string userId, ref string strErrorName)
        {
            fs0623_DataAccess.Save_Sub(listInfoData,userId, strErrorName);
        }

        public void Del_Sub(List<Dictionary<string, object>> listInfoData, string userId)
        {
            fs0623_DataAccess.Del_Sub(listInfoData, userId);
        }

        public DataSet Search()
        {
            return fs0623_DataAccess.Search();
        }

        public DataTable CheckDistinctByTableOrderGoods(DataTable dtadd)
        {
            return fs0623_DataAccess.CheckDistinctByTableOrderGoods(dtadd);
        }

        public string GetOrderDifferentiation()
        {
            DataTable dt = fs0623_DataAccess.GetOrderDifferentiation();
            string str = string.Empty;
            for (int i=0;i< dt.Rows.Count;i++)
            {
                str += dt.Rows[i]["vcOrderDifferentiation"].ToString()+",";
            }
            return str.Substring(0, str.LastIndexOf(','));
        }

        public bool AddOrderGoods(DataTable dtadd, String userId)
        {
            return fs0623_DataAccess.AddOrderGoods(dtadd, userId);
        }

        public void AddOrderGoodsAndDifferentiation(DataTable dtaddZJB, string userId)
        {
             fs0623_DataAccess.AddOrderGoodsAndDifferentiation(dtaddZJB, userId);
        }

        public void UpdateOrderGoodsAndDifferentiation(DataTable dtamodify, DataTable dtamodifyZJB, string userId)
        {
            fs0623_DataAccess.UpdateOrderGoodsAndDifferentiation(dtamodify, dtamodifyZJB, userId);
        }

        public void Del(List<Dictionary<string, object>> listInfoData, string userId)
        {
            fs0623_DataAccess.Del(listInfoData, userId);
        }
    }
}
