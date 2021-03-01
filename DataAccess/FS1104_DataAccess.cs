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
    public class FS1104_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        public string getCaseNoInfo(string strOrderPlant, string strReceiver, string strCaseNo)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                if (strCaseNo == "")
                {
                    strSql.AppendLine("declare @CaseNo int ");
                    strSql.AppendLine("set @CaseNo=(select max(cast(vcCaseNo as int)) from TCaseNoInfo where vcPlant='" + strOrderPlant + "' and vcReceiver='" + strReceiver + "')");
                    strSql.AppendLine("if((select count(*) from TCaseNoInfo where vcPlant='" + strOrderPlant + "' and vcReceiver='" + strReceiver + "')=0)");
                    strSql.AppendLine("begin");
                    strSql.AppendLine("select '00000001' as vcCaseNo");
                    strSql.AppendLine("end");
                    strSql.AppendLine("else");
                    strSql.AppendLine("begin");
                    strSql.AppendLine("select substring(cast((100000001+@CaseNo) as varchar(9)),2,8)  as vcCaseNo");
                    strSql.AppendLine("end");
                    DataTable data = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                    return data.Rows[0]["vcCaseNo"].ToString();
                }
                else
                {
                    strSql.AppendLine("select * from [TCaseNoInfo] where [vcPlant]='" + strOrderPlant + "' and [vcReceiver]='" + strReceiver + "' and cast([vcCaseNo] as int)='" + strCaseNo + "' and [dFirstPrintTime] is not null");
                    DataTable data = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                    if (data.Rows.Count == 0)
                        return "";
                    else
                        return data.Rows[0]["vcCaseNo"].ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void setPrintTemp(DataTable dtSub, string strOperId, ref DataTable dtMessage)
        {
            SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();

            sqlConnection.Open();
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            try
            {
                #region 写入数据库
                #region sqlCommand_deleteinfo
                SqlCommand sqlCommand_deleteinfo = sqlConnection.CreateCommand();
                sqlCommand_deleteinfo.Transaction = sqlTransaction;
                sqlCommand_deleteinfo.CommandType = CommandType.Text;
                StringBuilder strSql_deleteinfo = new StringBuilder();
                #region SQL and Parameters
                strSql_deleteinfo.AppendLine("DELETE from tPrintTemp_FS1104 where vcOperator='" + strOperId + "'");
                sqlCommand_deleteinfo.CommandText = strSql_deleteinfo.ToString();
                #endregion
                sqlCommand_deleteinfo.ExecuteNonQuery();
                #endregion

                #region sqlCommand_sub
                SqlCommand sqlCommand_sub = sqlConnection.CreateCommand();
                sqlCommand_sub.Transaction = sqlTransaction;
                sqlCommand_sub.CommandType = CommandType.Text;
                StringBuilder strSql_sub = new StringBuilder();

                #region SQL and Parameters
                strSql_sub.AppendLine("INSERT INTO [dbo].[tPrintTemp_FS1104]");
                strSql_sub.AppendLine("           ([vcOperator]");
                strSql_sub.AppendLine("           ,[UUID]");
                strSql_sub.AppendLine("           ,[dOperatorTime]");
                strSql_sub.AppendLine("           ,[vcCaseNo1]");
                strSql_sub.AppendLine("           ,[vcCaseNo2]");
                strSql_sub.AppendLine("           ,[vcPlant]");
                strSql_sub.AppendLine("           ,[vcReceiver]");
                strSql_sub.AppendLine("           ,[vcCodemage])");
                strSql_sub.AppendLine("     VALUES");
                strSql_sub.AppendLine("           ('" + strOperId + "'");
                strSql_sub.AppendLine("           ,@UUID");
                strSql_sub.AppendLine("           ,GETDATE()");
                strSql_sub.AppendLine("           ,@vcCaseNo1");
                strSql_sub.AppendLine("           ,@vcCaseNo2");
                strSql_sub.AppendLine("           ,@vcPlant");
                strSql_sub.AppendLine("           ,@vcReceiver");
                strSql_sub.AppendLine("           ,@vcCodemage)");
                sqlCommand_sub.CommandText = strSql_sub.ToString();
                sqlCommand_sub.Parameters.AddWithValue("@UUID", "");
                sqlCommand_sub.Parameters.AddWithValue("@vcCaseNo1", "");
                sqlCommand_sub.Parameters.AddWithValue("@vcCaseNo2", "");
                sqlCommand_sub.Parameters.AddWithValue("@vcPlant", "");
                sqlCommand_sub.Parameters.AddWithValue("@vcReceiver", "");
                sqlCommand_sub.Parameters.Add("@vcCodemage", SqlDbType.Image);
                #endregion
                foreach (DataRow item in dtSub.Rows)
                {
                    #region Value
                    sqlCommand_sub.Parameters["@UUID"].Value = item["UUID"].ToString();
                    sqlCommand_sub.Parameters["@vcCaseNo1"].Value = item["vcCaseNo1"].ToString();
                    sqlCommand_sub.Parameters["@vcCaseNo2"].Value = item["vcCaseNo2"].ToString();
                    sqlCommand_sub.Parameters["@vcPlant"].Value = item["vcPlant"].ToString();
                    sqlCommand_sub.Parameters["@vcReceiver"].Value = item["vcReceiver"].ToString();
                    sqlCommand_sub.Parameters["@vcCodemage"].Value = item["vcCodemage"];
                    #endregion
                    sqlCommand_sub.ExecuteNonQuery();
                }
                #endregion
                //提交事务
                sqlTransaction.Commit();
                sqlConnection.Close();
                #endregion

            }
            catch (Exception ex)
            {
                DataRow dataRow = dtMessage.NewRow();
                dataRow["vcMessage"] = "数据写入打印数据失败！";
                dtMessage.Rows.Add(dataRow);
                //回滚事务
                if (sqlTransaction != null && sqlConnection != null)
                {
                    sqlTransaction.Rollback();
                    sqlConnection.Close();
                }
            }
        }
        public void setSaveInfo(string strOperId, ref DataTable dtMessage)
        {
            SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();

            sqlConnection.Open();
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            try
            {
                #region 写入数据库
                #region sqlCommand_addinfo
                SqlCommand sqlCommand_addinfo = sqlConnection.CreateCommand();
                sqlCommand_addinfo.Transaction = sqlTransaction;
                sqlCommand_addinfo.CommandType = CommandType.Text;
                StringBuilder strSql_addinfo = new StringBuilder();
                #region SQL and Parameters
                strSql_addinfo.AppendLine("INSERT INTO [dbo].[TCaseNoInfo]");
                strSql_addinfo.AppendLine("           ([vcPlant]");
                strSql_addinfo.AppendLine("           ,[vcReceiver]");
                strSql_addinfo.AppendLine("           ,[vcCaseNo]");
                strSql_addinfo.AppendLine("           ,[dFirstPrintTime]");
                strSql_addinfo.AppendLine("           ,[dLatelyPrintTime])");
                strSql_addinfo.AppendLine("SELECT [vcPlant]");
                strSql_addinfo.AppendLine("      ,[vcReceiver]");
                strSql_addinfo.AppendLine("      ,[vcCaseNo1]");
                strSql_addinfo.AppendLine("      ,GETDATE(),null");
                strSql_addinfo.AppendLine("  FROM [dbo].[tPrintTemp_FS1104]");
                strSql_addinfo.AppendLine("  where vcOperator='" + strOperId + "'");
                sqlCommand_addinfo.CommandText = strSql_addinfo.ToString();
                #endregion
                sqlCommand_addinfo.ExecuteNonQuery();
                #endregion
                //提交事务
                sqlTransaction.Commit();
                sqlConnection.Close();
                #endregion

            }
            catch (Exception ex)
            {
                DataRow dataRow = dtMessage.NewRow();
                dataRow["vcMessage"] = "数据写入打印数据失败！";
                dtMessage.Rows.Add(dataRow);
                //回滚事务
                if (sqlTransaction != null && sqlConnection != null)
                {
                    sqlTransaction.Rollback();
                    sqlConnection.Close();
                }
            }
        }

        public void setSaveInfo(string strCaseNo, string strOperId, ref DataTable dtMessage)
        {
            SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();

            sqlConnection.Open();
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            try
            {
                #region 写入数据库
                #region sqlCommand_modinfo
                SqlCommand sqlCommand_modinfo = sqlConnection.CreateCommand();
                sqlCommand_modinfo.Transaction = sqlTransaction;
                sqlCommand_modinfo.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo = new StringBuilder();
                #region SQL and Parameters
                strSql_modinfo.AppendLine("update [TCaseNoInfo] set [dLatelyPrintTime]=GETDATE() where [vcCaseNo]='"+ strCaseNo + "'");
                sqlCommand_modinfo.CommandText = strSql_modinfo.ToString();
                #endregion
                sqlCommand_modinfo.ExecuteNonQuery();
                #endregion
                //提交事务
                sqlTransaction.Commit();
                sqlConnection.Close();
                #endregion

            }
            catch (Exception ex)
            {
                DataRow dataRow = dtMessage.NewRow();
                dataRow["vcMessage"] = "数据写入打印数据失败！";
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
