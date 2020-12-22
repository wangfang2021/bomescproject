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
        public DataTable Search(string vcFunctionID, string vcLogType, string vcTimeFrom, string vcTimeTo)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
               
                strSql.AppendLine("  select  vcFunctionID, vcLogType, vcMessage, vcException,    ");
                strSql.AppendLine("  vcTrack, convert(varchar(20), dCreateTime,120) as dCreateTime, a.vcIp,   ");
                strSql.AppendLine("  a.vcUserID+'-'+b.vcUserName as vcUserName   ");
                strSql.AppendLine("  from [dbo].[SLog] a   ");
                strSql.AppendLine("  left join [dbo].[SUser] b on a.vcUserID=b.vcUserID   ");
                strSql.AppendLine("where 1=1 ");
                if (vcFunctionID.Length>0)
                {
                    strSql.AppendLine(" and a.vcFunctionID  like '%" + vcFunctionID + "%' ");
                }
                if (vcLogType.Length > 0)
                {
                    strSql.AppendLine(" and a.vcLogType  = '" + vcLogType + "' ");
                }
                if (vcTimeFrom.Length > 0)
                {
                    strSql.AppendLine(" and a.dCreateTime>='"+ vcTimeFrom + "'  ");
                }
                if (vcTimeTo.Length > 0)
                {
                    strSql.AppendLine(" and a.dCreateTime<'" + vcTimeTo + "' ");
                }
                strSql.AppendLine(" order by dCreateTime desc ");
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
