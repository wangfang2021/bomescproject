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
    public class FS1402_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        public DataTable getSearchInfo(string strCheckType, string strPartId, string strSupplierId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select a.[LinId]");
                strSql.AppendLine("		,a.vcPartId as vcPartId");
                strSql.AppendLine("		,convert(varchar(10),a.vcTimeFrom,111) as dFromTime");
                strSql.AppendLine("		,convert(varchar(10),a.vcTimeTo,111)  as dToTime");
                strSql.AppendLine("		,a.vcSupplierCode as vcSupplierId");
                strSql.AppendLine("		,a.vcSupplierPlant as vcSupplierPlant");
                strSql.AppendLine("		,a.vcCarfamilyCode as vcCarfamilyCode");
                strSql.AppendLine("		,a.vcCheckP as vcCheckP");
                strSql.AppendLine("		,a.vcChangeRea as vcChangeRea");
                strSql.AppendLine("		,a.vcTJSX as vcTJSX");
                strSql.AppendLine("		,b.vcUserName as vcOperator");
                strSql.AppendLine("		,convert(varchar(10),a.dOperatorTime,23)as dOperatorTime");
                strSql.AppendLine("		,'0' as bModFlag,'0' as bAddFlag");
                strSql.AppendLine("		,'1' as bSelectFlag,'' as vcBgColor ");
                strSql.AppendLine("from (");
                strSql.AppendLine("select [LinId] AS [LinId],vcPartId,vcTimeFrom,vcTimeTo,vcSupplierCode,vcSupplierPlant,vcCarfamilyCode,vcCheckP");
                strSql.AppendLine(",vcChangeRea,vcTJSX,vcOperatorID,dOperatorTime from [tCheckQf]) a");
                strSql.AppendLine("left join ");
                strSql.AppendLine("(select * from sUser) b");
                strSql.AppendLine("on b.vcUserId=a.vcOperatorID");
                strSql.AppendLine("where 1=1 ");
                if (strCheckType != "")
                {
                    strSql.AppendLine(" and a.vcCheckP='" + strCheckType + "'");
                }
                if (strPartId != "")
                {
                    strSql.AppendLine(" and a.vcPartId like '" + strPartId + "%'");
                }
                if (strSupplierId != "")
                {
                    strSql.AppendLine(" and a.vcSupplierCode='" + strSupplierId + "'");
                }
                strSql.AppendLine("order by a.vcPartId ,a.vcTimeFrom");
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
                //strSql_addinfo.AppendLine("           ,[vcCarfamilyCode]");
                strSql_addinfo.AppendLine("           ,[vcSupplierCode]");
                //strSql_addinfo.AppendLine("           ,[vcSupplierPlant]");
                strSql_addinfo.AppendLine("           ,[vcCheckP]");
                strSql_addinfo.AppendLine("           ,[vcChangeRea]");
                strSql_addinfo.AppendLine("           ,[vcTJSX]");
                strSql_addinfo.AppendLine("           ,[vcOperatorID]");
                strSql_addinfo.AppendLine("           ,[dOperatorTime])");
                strSql_addinfo.AppendLine("     VALUES");
                strSql_addinfo.AppendLine("           (@vcPartId");
                strSql_addinfo.AppendLine("           ,@vcTimeFrom");
                strSql_addinfo.AppendLine("           ,@vcTimeTo");
                //strSql_addinfo.AppendLine("           ,@vcCarfamilyCode");
                strSql_addinfo.AppendLine("           ,@vcSupplierCode");
                //strSql_addinfo.AppendLine("           ,@vcSupplierPlant");
                strSql_addinfo.AppendLine("           ,@vcCheckP");
                strSql_addinfo.AppendLine("           ,@vcChangeRea");
                strSql_addinfo.AppendLine("           ,@vcTJSX");
                strSql_addinfo.AppendLine("           ,'"+ strOperId + "'");
                strSql_addinfo.AppendLine("           ,GETDATE())");
                sqlCommand_addinfo.CommandText = strSql_addinfo.ToString();
                sqlCommand_addinfo.Parameters.AddWithValue("@vcPartId", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcTimeFrom", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcTimeTo", "");
                //sqlCommand_addinfo.Parameters.AddWithValue("@vcCarfamilyCode", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcSupplierCode", "");
                //sqlCommand_addinfo.Parameters.AddWithValue("@vcSupplierPlant", "");
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
                        //sqlCommand_addinfo.Parameters["@vcCarfamilyCode"].Value = item["vcCarfamilyCode"].ToString();
                        sqlCommand_addinfo.Parameters["@vcSupplierCode"].Value = item["vcSupplierId"].ToString();
                        //sqlCommand_addinfo.Parameters["@vcSupplierPlant"].Value = item["vcSupplierPlant"].ToString();
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

        public DataTable getSearchSubInfo()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("SELECT [iPackingQty],[iSpotQty],'0' as vcBgColor,'1' as bModFlag,'1' as bAddFlag,'1'as bSelectFlag  FROM [dbo].[tCheckSpot] order by iPackingQty,iSpotQty");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setSavesubInfo(DataTable dtInfo, string strOperId, ref DataTable dtMessage)
        {
            SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();

            sqlConnection.Open();
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            string uuid = System.Guid.NewGuid().ToString();
            try
            {
                #region sqlCommand_delinfo
                SqlCommand sqlCommand_delinfo = sqlConnection.CreateCommand();
                sqlCommand_delinfo.Transaction = sqlTransaction;
                sqlCommand_delinfo.CommandType = CommandType.Text;
                StringBuilder strSql_delinfo = new StringBuilder();
                strSql_delinfo.AppendLine("DELETE FROM [tCheckSpot]");
                sqlCommand_delinfo.CommandText = strSql_delinfo.ToString();
                sqlCommand_delinfo.ExecuteNonQuery();
                #endregion

                #region sqlCommand_modinfo
                SqlCommand sqlCommand_modinfo = sqlConnection.CreateCommand();
                sqlCommand_modinfo.Transaction = sqlTransaction;
                sqlCommand_modinfo.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo = new StringBuilder();
                strSql_modinfo.AppendLine("INSERT INTO [dbo].[tCheckSpot]");
                strSql_modinfo.AppendLine("           ([iPackingQty]");
                strSql_modinfo.AppendLine("           ,[iSpotQty]");
                strSql_modinfo.AppendLine("           ,[vcOperatorID]");
                strSql_modinfo.AppendLine("           ,[dOperatorTime])");
                strSql_modinfo.AppendLine("     VALUES");
                strSql_modinfo.AppendLine("           (@iPackingQty");
                strSql_modinfo.AppendLine("           ,@iSpotQty");
                strSql_modinfo.AppendLine("           ,'" + strOperId + "'");
                strSql_modinfo.AppendLine("           ,GETDATE())");
                sqlCommand_modinfo.CommandText = strSql_modinfo.ToString();
                sqlCommand_modinfo.Parameters.AddWithValue("@iPackingQty", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@iSpotQty", "");
                foreach (DataRow item in dtInfo.Rows)
                {
                    sqlCommand_modinfo.Parameters["@iPackingQty"].Value = item["iPackingQty"].ToString();
                    sqlCommand_modinfo.Parameters["@iSpotQty"].Value = item["iSpotQty"].ToString();
                    sqlCommand_modinfo.ExecuteNonQuery();
                }
                #endregion
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
