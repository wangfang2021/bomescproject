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
    public class FS0104_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        #region 按检索条件检索,返回dt
        public DataTable Search(string strRoleName)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                System.Data.SqlClient.SqlParameter[] parameters = {
                    new SqlParameter("@vcFunctionID", SqlDbType.NVarChar,20),
                };
                parameters[0].Value = strRoleName;
                strSql.AppendLine("  select  vcFunctionID, vcLogType, vcMessage, vcException,    ");
                strSql.AppendLine("  vcTrack, convert(varchar(20), dCreateTime,120) as dCreateTime, a.vcIp,   ");
                strSql.AppendLine("  a.vcUserID+'-'+b.vcUserName as vcUserName   ");
                strSql.AppendLine("  from [dbo].[SLog] a   ");
                strSql.AppendLine("  left join [dbo].[SUser] b on a.vcUserID=b.vcUserID   ");
                strSql.AppendLine("  where vcFunctionID like '%'+@vcFunctionID+'%'   ");
                strSql.AppendLine("     ");
               
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString(), parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
