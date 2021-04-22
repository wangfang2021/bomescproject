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
    public class FS1405_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        public DataTable getSearchInfo(string strPartId, string strSupplierId, string strHaoJiu, string strInOut, string strOrderPlant, string strFrom, string strTo, string strCarModel, string strSPISStatus, List<Object> listTime)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select * from (");
                strSql.AppendLine("select T1.LinId AS LinId");
                strSql.AppendLine("		,t1.vcSPISStatus AS vcSPISStatus");
                strSql.AppendLine("		,case when t1.vcSPISStatus='2' then 0");
                strSql.AppendLine("			  when t1.vcSPISStatus='4' then 1");
                strSql.AppendLine("			  when t1.vcSPISStatus='1' then 2");
                strSql.AppendLine("			  when t1.vcSPISStatus='0' then 3");
                strSql.AppendLine("			  when t1.vcSPISStatus='3' then 4 else 5 end AS iOrderBy");
                strSql.AppendLine("		,T6.vcName AS vcSPISStatus_name");
                strSql.AppendLine("		,T1.vcApplyId AS vcApplyId");
                strSql.AppendLine("		,case when t5.LinId is null then convert(varchar(10),t1.dFromTime,23) else convert(varchar(10),t5.dFromTime_SPIS,23) end as dFromTime_SPIS");
                strSql.AppendLine("		,case when t5.LinId is null then convert(varchar(10),t1.dToTime,23) else convert(varchar(10),t5.dToTime_SPIS,23) end as dToTime_SPIS");
                strSql.AppendLine("		,convert(varchar(10),t5.dSPISTime,23) as dSPISTime");
                strSql.AppendLine("		,T1.vcPartId AS vcPartId");
                strSql.AppendLine("		,T1.vcPartENName AS vcPartENName");
                strSql.AppendLine("		,convert(varchar(10),cast(t1.dFromTime as datetime),111) AS dFromTime");
                strSql.AppendLine("		,convert(varchar(10),cast(t1.dToTime as datetime),111) AS dToTime");
                strSql.AppendLine("		,T1.vcCarfamilyCode AS vcCarfamilyCode");
                strSql.AppendLine("		,T2.vcName AS vcOrderPlant");
                strSql.AppendLine("		,T1.vcSupplierId AS vcSupplierId");
                strSql.AppendLine("		,T5.vcSupplierName AS vcSupplierName");
                strSql.AppendLine("		,T3.vcName AS vcInOut");
                strSql.AppendLine("		,T4.vcName AS vcHaoJiu");
                strSql.AppendLine("		,T5.vcColourNo AS vcColourNo");
                strSql.AppendLine("		,T5.vcColourCode AS vcColourCode");
                strSql.AppendLine("		,T5.vcColourName AS vcColourName");
                strSql.AppendLine("		,t5.vcPICUrl");
                strSql.AppendLine("		,t5.vcPDFUrl");
                strSql.AppendLine("		,t5.vcSPISUrl");
                strSql.AppendLine("		,t5.vcPICPath");
                strSql.AppendLine("		,t5.vcPDFPath");
                strSql.AppendLine("		,t5.vcSPISPath");
                strSql.AppendLine("		,t5.vcSupplier_1");
                strSql.AppendLine("		,t5.vcSupplier_2");
                strSql.AppendLine("		,t5.vcModItem");
                strSql.AppendLine("		,t5.vcGM");
                strSql.AppendLine("		,t5.vcOperName");
                strSql.AppendLine("		,CASE when isnull(t5.[vcSPISUrl],'')='' then '0' else '1' end as bSPISupload");
                strSql.AppendLine("		,'0' as bModFlag,'0' as bAddFlag,'1' as bSelectFlag");
                strSql.AppendLine("     from");
                strSql.AppendLine("(select * from tCheckMethod_Master");
                strSql.AppendLine("WHERE 1=1");
                if (strPartId != "")
                {
                    strSql.AppendLine("AND vcPartId like '" + strPartId + "%'");
                }
                if (strSupplierId != "")
                {
                    strSql.AppendLine("AND vcSupplierId='" + strSupplierId + "'");
                }
                if (strHaoJiu != "")
                {
                    strSql.AppendLine("AND vcHaoJiu='" + strHaoJiu + "'");
                }
                if (strInOut != "")
                {
                    strSql.AppendLine("AND vcInOut='" + strInOut + "'");
                }
                if (strOrderPlant != "")
                {
                    strSql.AppendLine("AND vcPartArea='" + strOrderPlant + "'");
                }
                if (strFrom != "" || strTo != "")
                {
                    if (strFrom.Length != 0)
                    {
                        strSql.AppendLine("AND (dFromTime> '" + strFrom + "' or dFromTime= '" + strFrom + "')");
                    }
                    if (strTo.Length != 0)
                    {
                        strSql.AppendLine("AND (dToTime<'" + strTo + "' or  dToTime='" + strTo + "')");
                    }
                }
                if (strCarModel != "")
                {
                    strSql.AppendLine("AND vcCarfamilyCode='" + strCarModel + "'");
                }
                if (strSPISStatus != "")
                {
                    strSql.AppendLine("AND vcSPISStatus='" + strSPISStatus + "'");
                }
                if (listTime.Count != 0)
                {
                    strSql.AppendLine("and ( ");
                    for (int i = 0; i < listTime.Count; i++)
                    {
                        if (listTime[i].ToString() == "现在")
                        {
                            strSql.AppendLine("(dFromTime<=Convert(varchar(10),getdate(),23) and dToTime>=Convert(varchar(10),getdate(),23))");
                        }
                        if (listTime[i].ToString() == "将来")
                        {
                            strSql.AppendLine("(dFromTime>Convert(varchar(10),getdate(),23))");
                        }
                        if (listTime[i].ToString() == "作废")
                        {
                            strSql.AppendLine("(dToTime<Convert(varchar(10),getdate(),23))");
                        }
                        if (i < listTime.Count - 1)
                        {
                            strSql.AppendLine(" or ");
                        }
                    }
                    strSql.AppendLine(") ");
                }
                strSql.AppendLine(")T1");
                strSql.AppendLine("left join");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C000')T2");
                strSql.AppendLine("ON T1.vcPartArea=T2.vcValue");
                strSql.AppendLine("left join");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C003')T3");
                strSql.AppendLine("ON T1.vcInOut=T3.vcValue");
                strSql.AppendLine("left join");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C004')T4");
                strSql.AppendLine("ON T1.vcHaoJiu=T4.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C067')T6");
                strSql.AppendLine("on t1.vcSPISStatus=T6.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT * FROM TSPISApply where vcSPISStatus in ('2','3'))T5");
                strSql.AppendLine("ON T1.vcApplyId=T5.vcApplyId");
                strSql.AppendLine(")tt order by iOrderBy,vcSupplierId,vcPartId,dFromTime");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable getSPISTimeInfo(string strPartId, string strSupplierId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select * from TSPISQf where vcPartId='" + strPartId + "' and vcSupplierCode='" + strSupplierId + "' ORDER BY cast(vcTimeFrom as datetime)");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void setSendtoInfo(DataTable dtImport, DataTable dtSPISTime, string strOperId, ref DataTable dtMessage)
        {
            SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();

            sqlConnection.Open();
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            try
            {
                #region sqlCommand_addinfo
                SqlCommand sqlCommand_addinfo = sqlConnection.CreateCommand();
                sqlCommand_addinfo.Transaction = sqlTransaction;
                sqlCommand_addinfo.CommandType = CommandType.Text;
                StringBuilder strSql_addinfo = new StringBuilder();
                #region SQL and Parameters
                strSql_addinfo.AppendLine("UPDATE tCheckMethod_Master SET vcSPISStatus=@vcSPISStatus,vcApplyId=@vcApplyId WHERE vcPartId=@vcPartId and vcSupplierId=@vcSupplierId ");
                strSql_addinfo.AppendLine("INSERT INTO [dbo].[TSPISApply]");
                strSql_addinfo.AppendLine("           ([vcApplyId]");
                strSql_addinfo.AppendLine("           ,[vcSPISStatus]");
                strSql_addinfo.AppendLine("           ,[dFromTime_SPIS]");
                strSql_addinfo.AppendLine("           ,[dToTime_SPIS]");
                strSql_addinfo.AppendLine("           ,[vcPartId]");
                strSql_addinfo.AppendLine("           ,[vcCarfamilyCode]");
                strSql_addinfo.AppendLine("           ,[vcSupplierId]");
                strSql_addinfo.AppendLine("           ,[vcPartENName]");
                strSql_addinfo.AppendLine("           ,[vcOperatorID]");
                strSql_addinfo.AppendLine("           ,[dOperatorTime])");
                strSql_addinfo.AppendLine("     VALUES");
                strSql_addinfo.AppendLine("           (@vcApplyId");
                strSql_addinfo.AppendLine("           ,@vcSPISStatus");
                strSql_addinfo.AppendLine("           ,@dFromTime_SPIS");
                strSql_addinfo.AppendLine("           ,@dToTime_SPIS");
                strSql_addinfo.AppendLine("           ,@vcPartId");
                strSql_addinfo.AppendLine("           ,@vcCarfamilyCode");
                strSql_addinfo.AppendLine("           ,@vcSupplierId");
                strSql_addinfo.AppendLine("           ,@vcPartENName");
                strSql_addinfo.AppendLine("		   ,'" + strOperId + "'");
                strSql_addinfo.AppendLine("		   ,GETDATE())");
                sqlCommand_addinfo.CommandText = strSql_addinfo.ToString();
                sqlCommand_addinfo.Parameters.AddWithValue("@vcApplyId", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcSPISStatus", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@dFromTime_SPIS", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@dToTime_SPIS", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcPartId", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcCarfamilyCode", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcSupplierId", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcPartENName", "");
                #endregion
                foreach (DataRow item in dtImport.Rows)
                {
                    sqlCommand_addinfo.Parameters["@vcApplyId"].Value = item["vcApplyId"].ToString();
                    sqlCommand_addinfo.Parameters["@vcSPISStatus"].Value = item["vcSPISStatus"].ToString();
                    sqlCommand_addinfo.Parameters["@dFromTime_SPIS"].Value = item["dFromTime_SPIS"].ToString();
                    sqlCommand_addinfo.Parameters["@dToTime_SPIS"].Value = item["dToTime_SPIS"].ToString().Replace("/", "-");
                    sqlCommand_addinfo.Parameters["@vcPartId"].Value = item["vcPartId"].ToString();
                    sqlCommand_addinfo.Parameters["@vcCarfamilyCode"].Value = item["vcCarfamilyCode"].ToString();
                    sqlCommand_addinfo.Parameters["@vcSupplierId"].Value = item["vcSupplierId"].ToString();
                    sqlCommand_addinfo.Parameters["@vcPartENName"].Value = item["vcPartENName"].ToString();
                    sqlCommand_addinfo.ExecuteNonQuery();
                }
                #endregion

                #region sqlCommand_modinfo
                SqlCommand sqlCommand_modinfo = sqlConnection.CreateCommand();
                sqlCommand_modinfo.Transaction = sqlTransaction;
                sqlCommand_modinfo.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo = new StringBuilder();

                #region SQL and Parameters
                strSql_modinfo.AppendLine("UPDATE [TSPISQf] SET [vcTimeTo]=@vcTimeTo WHERE LinId=@LinId");
                sqlCommand_modinfo.CommandText = strSql_modinfo.ToString();
                sqlCommand_modinfo.Parameters.AddWithValue("@vcTimeTo", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@LinId", "");
                #endregion
                foreach (DataRow item in dtSPISTime.Rows)
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

        public void setAdmitInfo(DataTable dtImport, DataTable dtSPISTime, string strOperId, ref DataTable dtMessage)
        {
            SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();

            sqlConnection.Open();
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            try
            {
                #region sqlCommand_modinfo
                SqlCommand sqlCommand_modinfo = sqlConnection.CreateCommand();
                sqlCommand_modinfo.Transaction = sqlTransaction;
                sqlCommand_modinfo.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo = new StringBuilder();
                #region SQL and Parameters
                strSql_modinfo.AppendLine("UPDATE tCheckMethod_Master SET vcSPISStatus=@vcSPISStatus WHERE vcApplyId=@vcApplyId ");
                strSql_modinfo.AppendLine(" update TSPISApply set vcSPISStatus=@vcSPISStatus,vcPicUrl=@vcPicUrl,vcPDFUrl=@vcPDFUrl,vcSPISUrl=@vcSPISUrl,vcOperName=@vcOperName, vcGM=@vcGM,vcOperatorID='" + strOperId + "',dOperatorTime=GETDATE() where vcApplyId=@vcApplyId");
                sqlCommand_modinfo.CommandText = strSql_modinfo.ToString();
                sqlCommand_modinfo.Parameters.AddWithValue("@LinId", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcSPISStatus", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcApplyId", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcPicUrl", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcPDFUrl", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcSPISUrl", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcOperName", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcGM", "");
                #endregion
                foreach (DataRow item in dtImport.Rows)
                {
                    sqlCommand_modinfo.Parameters["@LinId"].Value = item["LinId"].ToString();
                    sqlCommand_modinfo.Parameters["@vcSPISStatus"].Value = item["vcSPISStatus"].ToString();
                    sqlCommand_modinfo.Parameters["@vcApplyId"].Value = item["vcApplyId"].ToString();
                    sqlCommand_modinfo.Parameters["@vcPicUrl"].Value = item["vcPicUrl"].ToString();
                    sqlCommand_modinfo.Parameters["@vcPDFUrl"].Value = item["vcPDFUrl"].ToString();
                    sqlCommand_modinfo.Parameters["@vcSPISUrl"].Value = item["vcSPISUrl"].ToString();
                    sqlCommand_modinfo.Parameters["@vcOperName"].Value = item["vcOperName"].ToString();
                    sqlCommand_modinfo.Parameters["@vcGM"].Value = item["vcGM"].ToString();
                    sqlCommand_modinfo.ExecuteNonQuery();
                }
                #endregion

                #region sqlCommand_addinfo
                SqlCommand sqlCommand_addinfo = sqlConnection.CreateCommand();
                sqlCommand_addinfo.Transaction = sqlTransaction;
                sqlCommand_addinfo.CommandType = CommandType.Text;
                StringBuilder strSql_addinfo = new StringBuilder();

                #region SQL and Parameters
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
                foreach (DataRow item in dtSPISTime.Rows)
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

        public void setRejectInfo(DataTable dtImport, DataTable dtSPISTime, string strOperId, ref DataTable dtMessage)
        {
            SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();

            sqlConnection.Open();
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            try
            {
                #region sqlCommand_modinfo
                SqlCommand sqlCommand_modinfo = sqlConnection.CreateCommand();
                sqlCommand_modinfo.Transaction = sqlTransaction;
                sqlCommand_modinfo.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo = new StringBuilder();
                #region SQL and Parameters
                strSql_modinfo.AppendLine("UPDATE tCheckMethod_Master SET vcSPISStatus=@vcSPISStatus WHERE vcApplyId=@vcApplyId ");
                strSql_modinfo.AppendLine("UPDATE [dbo].[TSPISApply]");
                strSql_modinfo.AppendLine("   SET [vcSPISStatus] = @vcSPISStatus");
                strSql_modinfo.AppendLine("      ,[dSPISTime] = NULL");
                strSql_modinfo.AppendLine("      ,[vcSupplierName] = NULL");
                strSql_modinfo.AppendLine("      ,[vcColourNo] = NULL");
                strSql_modinfo.AppendLine("      ,[vcColourCode] = NULL");
                strSql_modinfo.AppendLine("      ,[vcColourName] = NULL");
                strSql_modinfo.AppendLine("      ,[dModDate] =NULL");
                strSql_modinfo.AppendLine("      ,[vcModItem] = NULL");
                strSql_modinfo.AppendLine("      ,[vcPICUrl] = NULL");
                strSql_modinfo.AppendLine("      ,[vcPICPath] = NULL");
                strSql_modinfo.AppendLine("      ,[vcPDFUrl] =NULL");
                strSql_modinfo.AppendLine("      ,[vcPDFPath] =NULL");
                strSql_modinfo.AppendLine("      ,[vcSPISUrl] =NULL");
                strSql_modinfo.AppendLine("      ,[vcSPISPath] = NULL");
                strSql_modinfo.AppendLine("      ,[vcSupplier_1] = NULL");
                strSql_modinfo.AppendLine("      ,[vcSupplier_2] = NULL");
                strSql_modinfo.AppendLine("      ,[vcOperName] = NULL");
                strSql_modinfo.AppendLine("      ,[vcGM] = NULL");
                strSql_modinfo.AppendLine("      ,[vcOperatorID] = '" + strOperId + "'");
                strSql_modinfo.AppendLine("      ,[dOperatorTime] = GETDATE()");
                strSql_modinfo.AppendLine(" WHERE [vcApplyId] = @vcApplyId");
                sqlCommand_modinfo.CommandText = strSql_modinfo.ToString();
                sqlCommand_modinfo.Parameters.AddWithValue("@vcSPISStatus", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcApplyId", "");
                #endregion
                foreach (DataRow item in dtImport.Rows)
                {
                    sqlCommand_modinfo.Parameters["@vcSPISStatus"].Value = item["vcSPISStatus"].ToString();
                    sqlCommand_modinfo.Parameters["@vcApplyId"].Value = item["vcApplyId"].ToString();
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
