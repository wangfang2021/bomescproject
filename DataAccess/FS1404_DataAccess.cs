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
        public DataTable getSearchInfo(string strPartId, string strSupplierId)
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
                strSql.AppendLine("		,a.vcPicUrl as vcPicUrl");
                strSql.AppendLine("		,a.vcChangeRea as vcChangeRea");
                strSql.AppendLine("		,b.vcUserName as vcOperator");
                strSql.AppendLine("		,convert(varchar(10),a.dOperatorTime,23)as dOperatorTime");
                strSql.AppendLine("		,'0' as bModFlag,'0' as bAddFlag");
                strSql.AppendLine("		,'1' as bSelectFlag,'' as vcBgColor ");
                strSql.AppendLine("from (");
                strSql.AppendLine("select [LinId] AS [LinId],vcPartId,vcTimeFrom,vcTimeTo,vcSupplierCode,vcSupplierPlant,vcCarfamilyCode,vcPicUrl");
                strSql.AppendLine(",vcChangeRea,vcOperatorID,dOperatorTime from tSPISQf) a");
                strSql.AppendLine("left join ");
                strSql.AppendLine("(select * from sUser) b");
                strSql.AppendLine("on b.vcUserId=a.vcOperatorID");
                strSql.AppendLine("where 1=1 ");
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

        public DataTable getSearchsubInfo(string strCarModel, string strSupplierId, List<Object> listTime)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select a.* from (");
                strSql.AppendLine("select * from tSPISQf where 1=1");
                if (strSupplierId != "")
                {
                    strSql.AppendLine(" and vcSupplierCode='" + strSupplierId + "'");

                }
                if (listTime.Count != 0)
                {
                    strSql.AppendLine("and ( ");
                    for (int i = 0; i < listTime.Count; i++)
                    {
                        if (listTime[i].ToString() == "现在")
                        {
                            strSql.AppendLine("(vcTimeFrom<=Convert(varchar(10),getdate(),23) and vcTimeTo>=Convert(varchar(10),getdate(),23))");
                        }
                        if (listTime[i].ToString() == "将来")
                        {
                            strSql.AppendLine("(vcTimeFrom>Convert(varchar(10),getdate(),23))");
                        }
                        if (listTime[i].ToString() == "作废")
                        {
                            strSql.AppendLine("(vcTimeTo<Convert(varchar(10),getdate(),23))");
                        }
                        if (i < listTime.Count - 1)
                        {
                            strSql.AppendLine(" or ");
                        }
                    }
                    strSql.AppendLine(") ");
                }
                strSql.AppendLine(")a ");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select * from tCheckMethod_Master)b");
                strSql.AppendLine("on a.vcSupplierCode=b.vcSupplierId and a.vcPartId=b.vcPartId");
                if (strCarModel != "")
                {
                    strSql.AppendLine(" where b.vcCarfamilyCode='" + strCarModel + "'");

                }
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
                strSql_addinfo.AppendLine("UPDATE tCheckMethod_Master SET vcSPISStatus='3' WHERE vcSupplierId=@vcSupplierCode and vcPartId=@vcPartId  ");
                strSql_addinfo.AppendLine("INSERT INTO [dbo].[tSPISQf]");
                strSql_addinfo.AppendLine("           ([vcPartId]");
                strSql_addinfo.AppendLine("           ,[vcTimeFrom]");
                strSql_addinfo.AppendLine("           ,[vcTimeTo]");
                strSql_addinfo.AppendLine("           ,[vcSupplierCode]");
                strSql_addinfo.AppendLine("           ,[vcPicUrl]");
                strSql_addinfo.AppendLine("           ,[vcChangeRea]");
                strSql_addinfo.AppendLine("           ,[vcOperatorID]");
                strSql_addinfo.AppendLine("           ,[dOperatorTime])");
                strSql_addinfo.AppendLine("     VALUES");
                strSql_addinfo.AppendLine("           (@vcPartId");
                strSql_addinfo.AppendLine("           ,@vcTimeFrom");
                strSql_addinfo.AppendLine("           ,@vcTimeTo");
                strSql_addinfo.AppendLine("           ,@vcSupplierCode");
                strSql_addinfo.AppendLine("           ,@vcPicUrlUUID");
                strSql_addinfo.AppendLine("           ,@vcChangeRea");
                strSql_addinfo.AppendLine("           ,'" + strOperId + "'");
                strSql_addinfo.AppendLine("           ,GETDATE())");
                sqlCommand_addinfo.CommandText = strSql_addinfo.ToString();
                sqlCommand_addinfo.Parameters.AddWithValue("@vcPartId", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcTimeFrom", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcTimeTo", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcSupplierCode", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcPicUrlUUID", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcChangeRea", "");
                #endregion
                foreach (DataRow item in dtImport.Rows)
                {
                    if (item["vcType"].ToString() == "add")
                    {
                        #region Value
                        sqlCommand_addinfo.Parameters["@vcPartId"].Value = item["vcPartId"].ToString();
                        sqlCommand_addinfo.Parameters["@vcTimeFrom"].Value = item["dFromTime"].ToString();
                        sqlCommand_addinfo.Parameters["@vcTimeTo"].Value = item["dToTime"].ToString();
                        sqlCommand_addinfo.Parameters["@vcSupplierCode"].Value = item["vcSupplierId"].ToString();
                        sqlCommand_addinfo.Parameters["@vcPicUrlUUID"].Value = item["vcPicUrlUUID"].ToString();
                        sqlCommand_addinfo.Parameters["@vcChangeRea"].Value = item["vcChangeRea"].ToString();
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
                strSql_modinfo.AppendLine("UPDATE [tSPISQf] SET [vcTimeTo]=@vcTimeTo WHERE LinId=@LinId");
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
