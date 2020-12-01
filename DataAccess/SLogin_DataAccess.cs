using System;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Common;

namespace DataAccess
{

    public class SLogin_DataAccess
    {
        MultiExcute me;
        public SLogin_DataAccess()
        {
            me = new MultiExcute();
        }
        #region 用户登录
        public DataTable UserLogin(string strUserID, string strPassword)
        {
            System.Data.SqlClient.SqlParameter[] parameters = {
                    new SqlParameter("@userId", SqlDbType.NVarChar,50),
                    new SqlParameter("@userPwd", SqlDbType.NVarChar,100)
            };
            parameters[0].Value = strUserID;
            parameters[1].Value = strPassword;

            StringBuilder sbSql = new StringBuilder();
            sbSql.Append("  select a.*,b.vcUnitName from   \n");
            sbSql.Append("  (   \n");
            sbSql.Append("  select * from SUser where  vcUserID=@userId and vcPassWord=@userPwd and vcStop='0'     \n");
            sbSql.Append("  )a   \n");
            sbSql.Append("  left join         \n");
            sbSql.Append("  (        \n");
            sbSql.Append("  select vcUnitCode,vcUnitName from SUnitInfo         \n");
            sbSql.Append("  )b on a.vcUnitCode=b.vcUnitCode        \n");
            return me.ExcuteSqlWithSelectToDT(sbSql.ToString(), parameters);
        }
        #endregion

        #region 获取用户信息
        public DataTable getUserInfo(string strUserID)
        {
            System.Data.SqlClient.SqlParameter[] parameters = {
                    new SqlParameter("@userId", SqlDbType.NVarChar,50)
            };
            parameters[0].Value = strUserID;

            StringBuilder sbSql = new StringBuilder();
            sbSql.Append("  select * from SUser where vcUserID=@userId and vcStop='0'   \n");
            return me.ExcuteSqlWithSelectToDT(sbSql.ToString(), parameters);
        }
        #endregion

        public DataTable GetRouter(string strUserId)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.Append("  select b.*,a.vcRead,a.vcWrite      \r\n ");
                sbr.Append("  from       \r\n ");
                sbr.Append("  (      \r\n ");
                sbr.Append("  	 select vcChildFunID,max(vcRead) as 'vcRead',max(vcWrite) as 'vcWrite' from SRoleFunction    \r\n ");
                sbr.Append("  	 where vcRoleID in      \r\n ");
                sbr.Append("  	 (      \r\n ");
                sbr.Append("  	     select vcRoleId from SUSerRole where vcUserId='" + strUserId + "'      \r\n ");
                sbr.Append("  	 )      \r\n ");
                sbr.Append("  	 group by vcChildFunID       \r\n ");
                sbr.Append("  )a      \r\n ");
                sbr.Append("  left join      \r\n ");
                sbr.Append("  (      \r\n ");
                sbr.Append("       select *   \r\n ");
                sbr.Append("       from SFunction      \r\n ");
                sbr.Append("       where vcType='0'      \r\n ");
                sbr.Append("  )b on a.vcChildFunID=b.vcChildFunID      \r\n ");
                sbr.Append("  where b.vcChildFunID is not null      \r\n ");
                sbr.Append("  order by b.iFatherSort asc,b.iSort asc      \r\n ");

                return me.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }
}
