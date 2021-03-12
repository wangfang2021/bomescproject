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
    public class FS0504_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        public DataTable getDataInfo(string strSupplierId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("SELECT [LinId] as LinId");
                strSql.AppendLine("      ,[vcOrderNo] as vcOrderNo");
                strSql.AppendLine("      ,[vcTagZIP] as vcTagZIP");
                strSql.AppendLine("      ,convert(varchar(10),[dCreateTime],111) as dCreateTime");
                strSql.AppendLine("	     ,'1' as bSelectFlag");
                strSql.AppendLine("  FROM [TTagDownLoadList]");
                strSql.AppendLine("  WHERE [vcSupplierId]='" + strSupplierId + "' AND [dDownLoadTime] IS NULL");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setDataInfo(string strLinId, string strOperId, ref DataTable dtMessage)
        {
            SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();

            sqlConnection.Open();
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            try
            {
                SqlCommand sqlCommand_modinfo = sqlConnection.CreateCommand();
                sqlCommand_modinfo.Transaction = sqlTransaction;
                sqlCommand_modinfo.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo = new StringBuilder();
                strSql_modinfo.AppendLine("UPDATE [TTagDownLoadList] SET vcDownLoaderId='"+ strOperId + "',dDownLoadTime=GETDATE() WHERE LinId=@LinId");
                sqlCommand_modinfo.CommandText = strSql_modinfo.ToString();
                sqlCommand_modinfo.Parameters.AddWithValue("@vcYearMonth", "");
                sqlCommand_modinfo.Parameters["@LinId"].Value = strLinId.ToString();
                sqlCommand_modinfo.ExecuteNonQuery();
                //提交事务
                sqlTransaction.Commit();
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                DataRow dataRow = dtMessage.NewRow();
                dataRow["vcMessage"] = "数据写入数据库失败！";
                dtMessage.Rows.Add(dataRow);
                //回滚事务
                if (sqlTransaction != null && sqlConnection != null)
                {
                    sqlTransaction.Rollback();
                    sqlConnection.Close();
                }
            }
        }
    }
}
