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
                strSql.Append("     select a.vcCLYM,a.vcInOutFlag,case when a.vcFZGC <> null and a.dZhanKaiTime <> null then '可下载' else '未发送' end as '状态',MAX( a.dZhanKaiTime) as '展开时间' from    \n");
                strSql.Append("     (    \n");
                strSql.Append("     select * from     \n");
                strSql.Append("     (    \n");
                strSql.Append("     	select distinct vcCLYM,vcInOutFlag,vcFZGC from TSoqReply     \n");
                strSql.Append("     	where 1=1    \n");
                if (!string.IsNullOrEmpty(strDXDateMonth))
                {
                    strSql.Append("         and vcCLYM = '"+strDXDateMonth+"'    \n");
                }
                if (!string.IsNullOrEmpty(strInOutFlag))
                {
                    strSql.Append("         and vcInOutFlag = '" + strInOutFlag + "'    \n");
                }
                strSql.Append("     ) a    \n");
                strSql.Append("     right join     \n");
                strSql.Append("     (    \n");
                strSql.Append("     	select vcValue from TCode where vcCodeId = 'C000'    \n");
                strSql.Append("     ) b on a.vcFZGC = b.vcValue    \n");
                strSql.Append("     left join     \n");
                strSql.Append("     (    \n");
                strSql.Append("     	select distinct vcCLYM as 'vcCLYM_',vcInOutFlag as 'vcInOutFlag_',vcFZGC as 'vcFZGC_',dZhanKaiTime from TSoqReply    \n");
                strSql.Append("     ) c on a.vcCLYM = c.vcCLYM_ and a.vcInOutFlag = c.vcInOutFlag_ and a.vcFZGC = c.vcFZGC_    \n");
                strSql.Append("     )a    \n");
                strSql.Append("     group by a.vcCLYM,vcInOutFlag    \n");
                strSql.Append("     order by vcCLYM    \n");
                
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
