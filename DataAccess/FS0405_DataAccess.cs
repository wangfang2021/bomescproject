using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Common;
using System.Data.SqlClient;
using System.Collections;
namespace DataAccess
{
    public class FS0405_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 按检索条件返回dt
        public DataTable Search(string strDXDateMonth, string strInOutFlag, string strState)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("    select  a.vcDXYM,'文件名'as'vcFileName',b.vcName as 'vcInOutFlag'     \n");
                strSql.Append("    ,case when dZhanKaiTime is null then '未展开' when dZhanKaiTime is not null then '已展开' end as 'vcState'     \n");
                strSql.Append("    from TSoqReply a     \n");
                strSql.Append("    inner join     \n");
                strSql.Append("    (     \n");
                strSql.Append("    	select * from TCode where vcCodeId = 'C003'     \n");
                strSql.Append("    )b     \n");
                strSql.Append("    on a.vcInOutFlag = b.vcValue     \n");
                strSql.Append("    where 1=1     \n");
                if (!string.IsNullOrEmpty(strDXDateMonth))
                {
                    strSql.Append("     and vcDXYM = '"+strDXDateMonth+"'    \n");
                }
                if (!string.IsNullOrEmpty(strInOutFlag))
                {
                    strSql.Append("     and vcInOutFlag = '"+strInOutFlag+"'    \n");
                }

                if (!string.IsNullOrEmpty(strState))
                {
                    strSql.Append("         \n");
                }
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }
}
