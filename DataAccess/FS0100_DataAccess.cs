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
    public class FS0100_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 确认当前输入密码是否正确
        public DataTable checkPwd(string strUserId, string strPWD)
        {
            try
            {
                SqlParameter[] parameters = {
                    new SqlParameter("@userId", SqlDbType.NVarChar,50),
                    new SqlParameter("@userPwd", SqlDbType.NVarChar,100)
                };
                parameters[0].Value = strUserId;
                parameters[1].Value = strPWD;
                StringBuilder strSql = new StringBuilder();
                strSql.Append("    select * from SUser where vcUserID=@userId and vcPassWord=@userPwd            \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString(), parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 修改密码操作
        public int changePwd(string strUserId, string strPwd)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("  update SUser     \n");
                strSql.Append("  set vcPassWord='" + strPwd + "'      \n");
                strSql.Append("  where vcUserId='" + strUserId + "'  ;   \n");
                return excute.ExcuteSqlWithStringOper(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
