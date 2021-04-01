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
        public DataTable Search(string strDownloadDiff,string strSupplier)
        {
            
            DataTable dt_All = fs0718_DataAccess.Search(strDownloadDiff);
            DataTable returnDT = dt_All.Clone();
            if (dt_All!=null && dt_All.Rows.Count>0)
            {
                for (int i = 0; i < dt_All.Rows.Count; i++)
                {
                    //获取dt的供应商字段的值
                    string vcSupplier = dt_All.Rows[i]["vcSupplier"].ToString();
                    //判断供应商是否时多个
                    //获取供应商数组
                    string[] suppliers = vcSupplier.Split(',');
                    if (suppliers.Count()>0)
                    {
                        for (int j = 0; j < suppliers.Count(); j++)
                        {
                            if (suppliers[j]==strSupplier)
                            {
                                dt_All.Rows[i]["vcSupplier"] = strSupplier;
                                returnDT.ImportRow(dt_All.Rows[i]);
                            }
                        }
                    }
                }
            }
            return returnDT;
        }
        #endregion

        #region 检索需要导出月度内示
        public DataTable Search_Month(string strSupplier, string strYearMonth, string dFaBuTime)
        {
            return fs0718_DataAccess.Search_Month(strSupplier, strYearMonth, dFaBuTime);
        }
        #endregion

        #region 检索需要导出的周度内示
        public DataTable Search_Week() 
        {
            fs0718_DataAccess.Search_Week();
            return null;
        }
        #endregion

    }
}
