using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS0718_Logic
    {
        FS0718_DataAccess fs0718_DataAccess;

        public FS0718_Logic()
        {
            fs0718_DataAccess = new FS0718_DataAccess();
        }

        #region 按检索条件检索,返回dt
        public DataTable Search(string strSupplier,string strDownloadDiff)
        {
            
            return fs0718_DataAccess.Search(strSupplier,strDownloadDiff);
        }
        #endregion

        #region 检索需要导出月度内示
        public DataTable Search_Month(string strSupplier, string strYearMonth, string dFaBuTime)
        {
            return fs0718_DataAccess.Search_Month(strSupplier, strYearMonth, dFaBuTime);
        }
        #endregion

        #region 检索需要导出的周度内示
        public DataTable Search_Week(string strSupplierCode,string strDFaBuTime) 
        {
            //添加转化时间未固定格式
            strDFaBuTime = Convert.ToDateTime(strDFaBuTime).ToString("yyyy-MM-dd HH:mm:ss");

            return fs0718_DataAccess.Search_Week(strSupplierCode,strDFaBuTime);
        }
        #endregion

        #region 更新包材内示检索表
        public void updateSearchTable(List<Dictionary<string, Object>> listInfoData, string strUserID)
        {
            fs0718_DataAccess.updateSearchTable(listInfoData, strUserID);
        }
        #endregion

    }
}
