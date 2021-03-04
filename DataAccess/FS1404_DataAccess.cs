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
    public class FS1404_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        public DataTable getSearchInfo(string strPartId, string strSupplierId, string strSupplierPlant)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("");
                strSql.AppendLine("");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setSaveInfo(DataTable dtImport, string strOperId, ref DataTable dtMessage)
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
                strSql_addinfo.AppendLine("INSERT INTO [dbo].[tCheckQf]");
                strSql_addinfo.AppendLine("           ([vcPartId]");
                strSql_addinfo.AppendLine("           ,[vcTimeFrom]");
                strSql_addinfo.AppendLine("           ,[vcTimeTo]");
                strSql_addinfo.AppendLine("           ,[vcCarfamilyCode]");
                strSql_addinfo.AppendLine("           ,[vcSupplierCode]");
                strSql_addinfo.AppendLine("           ,[vcSupplierPlant]");
                strSql_addinfo.AppendLine("           ,[vcCheckP]");
                strSql_addinfo.AppendLine("           ,[vcChangeRea]");
                strSql_addinfo.AppendLine("           ,[vcTJSX]");
                strSql_addinfo.AppendLine("           ,[vcOperatorID]");
                strSql_addinfo.AppendLine("           ,[dOperatorTime])");
                strSql_addinfo.AppendLine("     VALUES");
                strSql_addinfo.AppendLine("           (@vcPartId");
                strSql_addinfo.AppendLine("           ,@vcTimeFrom");
                strSql_addinfo.AppendLine("           ,@vcTimeTo");
                strSql_addinfo.AppendLine("           ,@vcCarfamilyCode");
                strSql_addinfo.AppendLine("           ,@vcSupplierCode");
                strSql_addinfo.AppendLine("           ,@vcSupplierPlant");
                strSql_addinfo.AppendLine("           ,@vcCheckP");
                strSql_addinfo.AppendLine("           ,@vcChangeRea");
                strSql_addinfo.AppendLine("           ,@vcTJSX");
                strSql_addinfo.AppendLine("           ,'" + strOperId + "'");
                strSql_addinfo.AppendLine("           ,GETDATE())");
                sqlCommand_addinfo.CommandText = strSql_addinfo.ToString();
                sqlCommand_addinfo.Parameters.AddWithValue("@vcPartId", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcTimeFrom", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcTimeTo", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcCarfamilyCode", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcSupplierCode", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcSupplierPlant", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcCheckP", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcChangeRea", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcTJSX", "");
                #endregion
                foreach (DataRow item in dtImport.Rows)
                {
                    if (item["vcType"].ToString() == "add")
                    {
                        #region Value
                        sqlCommand_addinfo.Parameters["@vcPartId"].Value = item["vcPartId"].ToString();
                        sqlCommand_addinfo.Parameters["@vcTimeFrom"].Value = item["dFromTime"].ToString();
                        sqlCommand_addinfo.Parameters["@vcTimeTo"].Value = item["dToTime"].ToString();
                        sqlCommand_addinfo.Parameters["@vcCarfamilyCode"].Value = item["vcCarfamilyCode"].ToString();
                        sqlCommand_addinfo.Parameters["@vcSupplierCode"].Value = item["vcSupplierId"].ToString();
                        sqlCommand_addinfo.Parameters["@vcSupplierPlant"].Value = item["vcSupplierPlant"].ToString();
                        sqlCommand_addinfo.Parameters["@vcCheckP"].Value = item["vcCheckP"].ToString();
                        sqlCommand_addinfo.Parameters["@vcChangeRea"].Value = item["vcChangeRea"].ToString();
                        sqlCommand_addinfo.Parameters["@vcTJSX"].Value = item["vcTJSX"].ToString();
                        #endregion
                        sqlCommand_addinfo.ExecuteNonQuery();
                    }
                }
                #endregion

                #region sqlCommand_modinfo
                SqlCommand sqlCommand_modinfo = sqlConnection.CreateCommand();
                sqlCommand_modinfo.Transaction = sqlTransaction;
                sqlCommand_modinfo.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo = new StringBuilder();

                #region SQL and Parameters
                strSql_modinfo.AppendLine("UPDATE [tCheckQf] SET [vcTimeTo]=@vcTimeTo WHERE LinId=@LinId");
                sqlCommand_modinfo.CommandText = strSql_modinfo.ToString();
                sqlCommand_modinfo.Parameters.AddWithValue("@vcTimeTo", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@LinId", "");
                #endregion
                foreach (DataRow item in dtImport.Rows)
                {
                    if (item["vcType"].ToString() == "mod")
                    {
                        #region Value
                        sqlCommand_modinfo.Parameters["@vcTimeTo"].Value = item["dToTime"].ToString();
                        sqlCommand_modinfo.Parameters["@LinId"].Value = item["LinId"].ToString();
                        #endregion
                        sqlCommand_modinfo.ExecuteNonQuery();
                    }
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
