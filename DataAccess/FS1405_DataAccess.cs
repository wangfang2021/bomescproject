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
                strSql.AppendLine("select T1.LinId AS LinId");
                strSql.AppendLine("		,T1.vcApplyId AS vcApplyId");
                strSql.AppendLine("		,t1.vcSPISStatus AS vcSPISStatus");
                strSql.AppendLine("		,T6.vcName AS vcSPISStatus_name");
                strSql.AppendLine("		,T1.vcPartId AS vcPartId");
                strSql.AppendLine("		,T1.vcPartENName AS vcPartENName");
                strSql.AppendLine("		,convert(varchar(10),cast(t1.dFromTime as datetime),111) AS dFromTime");
                strSql.AppendLine("		,convert(varchar(10),cast(t1.dToTime as datetime),111) AS dToTime");
                strSql.AppendLine("		,T1.vcCarfamilyCode AS vcCarfamilyCode");
                strSql.AppendLine("		,T2.vcName AS vcOrderPlant");
                strSql.AppendLine("		,T1.vcSupplierId AS vcSupplierId");
                strSql.AppendLine("		,T3.vcName AS vcInOut");
                strSql.AppendLine("		,T4.vcName AS vcHaoJiu");
                strSql.AppendLine("		,t5.vcSPISUrl");
                strSql.AppendLine("		,t5.vcPicUrl");
                strSql.AppendLine("		,t5.vcPDFUrl");
                strSql.AppendLine("		,t5.vcSupplier_1");
                strSql.AppendLine("		,t5.vcSupplier_2");
                strSql.AppendLine("		,t5.vcModItem");
                strSql.AppendLine("		,t5.vcGM");
                strSql.AppendLine("		,t5.vcOperName");
                strSql.AppendLine("		,CASE when isnull(t5.vcSPISUrl,'')='' then '0' else '1' end as bSPISupload");
                strSql.AppendLine("		,'0' as bModFlag,'0' as bAddFlag,'1' as bSelectFlag");
                strSql.AppendLine("		from ");
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
                strSql.AppendLine("(SELECT * FROM TSPISApply)T5");
                strSql.AppendLine("ON T1.vcApplyId=T5.vcApplyId");
                strSql.AppendLine("where 1=1");
                strSql.AppendLine("order by t1.vcPartId,t1.dFromTime");
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
                strSql.AppendLine("select * from TSPISQf where vcPartId='"+ strPartId + "' and vcSupplierCode='"+ strSupplierId + "' ORDER BY cast(vcTimeFrom as datetime)");
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
                strSql_addinfo.AppendLine("UPDATE tCheckMethod_Master SET vcSPISStatus=@vcSPISStatus,vcApplyId=@vcApplyId WHERE LinId=@LinId ");
                strSql_addinfo.AppendLine("INSERT INTO [dbo].[TSPISApply] ");
                strSql_addinfo.AppendLine("           ([vcApplyId] ");
                strSql_addinfo.AppendLine("           ,[dToTime_SPIS] ");
                strSql_addinfo.AppendLine("           ,[dSPISTime] ");
                strSql_addinfo.AppendLine("           ,[vcPartId] ");
                strSql_addinfo.AppendLine("           ,[vcCarfamilyCode] ");
                strSql_addinfo.AppendLine("           ,[vcSupplierId] ");
                strSql_addinfo.AppendLine("           ,[vcPartENName] ");
                strSql_addinfo.AppendLine("           ,[vcColourNo] ");
                strSql_addinfo.AppendLine("           ,[vcColourCode] ");
                strSql_addinfo.AppendLine("           ,[vcColourName] ");
                strSql_addinfo.AppendLine("           ,[vcModItem] ");
                strSql_addinfo.AppendLine("           ,[vcPicUrl] ");
                strSql_addinfo.AppendLine("           ,[vcPDFUrl] ");
                strSql_addinfo.AppendLine("           ,[vcSPISUrl] ");
                strSql_addinfo.AppendLine("           ,[vcSupplier_1] ");
                strSql_addinfo.AppendLine("           ,[vcSupplier_2] ");
                strSql_addinfo.AppendLine("           ,[vcOperName] ");
                strSql_addinfo.AppendLine("           ,[vcGM] ");
                strSql_addinfo.AppendLine("           ,[vcOperatorID] ");
                strSql_addinfo.AppendLine("           ,[dOperatorTime]) ");
                strSql_addinfo.AppendLine("     VALUES ");
                strSql_addinfo.AppendLine("           (CASE WHEN @vcApplyId='' THEN NULL ELSE @vcApplyId END ");
                strSql_addinfo.AppendLine("           ,CASE WHEN @dToTime_SPIS='' THEN NULL ELSE @dToTime_SPIS END ");
                strSql_addinfo.AppendLine("           ,CASE WHEN @dSPISTime='' THEN NULL ELSE @dSPISTime END ");
                strSql_addinfo.AppendLine("           ,CASE WHEN @vcPartId='' THEN NULL ELSE @vcPartId END ");
                strSql_addinfo.AppendLine("           ,CASE WHEN @vcCarfamilyCode='' THEN NULL ELSE @vcCarfamilyCode END ");
                strSql_addinfo.AppendLine("           ,CASE WHEN @vcSupplierId='' THEN NULL ELSE @vcSupplierId END ");
                strSql_addinfo.AppendLine("           ,CASE WHEN @vcPartENName='' THEN NULL ELSE @vcPartENName END ");
                strSql_addinfo.AppendLine("           ,CASE WHEN @vcColourNo='' THEN NULL ELSE @vcColourNo END ");
                strSql_addinfo.AppendLine("           ,CASE WHEN @vcColourCode='' THEN NULL ELSE @vcColourCode END ");
                strSql_addinfo.AppendLine("           ,CASE WHEN @vcColourName='' THEN NULL ELSE @vcColourName END ");
                strSql_addinfo.AppendLine("           ,CASE WHEN @vcModItem='' THEN NULL ELSE @vcModItem END ");
                strSql_addinfo.AppendLine("           ,CASE WHEN @vcPicUrl='' THEN NULL ELSE @vcPicUrl END ");
                strSql_addinfo.AppendLine("           ,CASE WHEN @vcPDFUrl='' THEN NULL ELSE @vcPDFUrl END ");
                strSql_addinfo.AppendLine("           ,CASE WHEN @vcSPISUrl='' THEN NULL ELSE @vcSPISUrl END ");
                strSql_addinfo.AppendLine("           ,CASE WHEN @vcSupplier_1='' THEN NULL ELSE @vcSupplier_1 END ");
                strSql_addinfo.AppendLine("           ,CASE WHEN @vcSupplier_2='' THEN NULL ELSE @vcSupplier_2 END ");
                strSql_addinfo.AppendLine("           ,CASE WHEN @vcOperName='' THEN NULL ELSE @vcOperName END ");
                strSql_addinfo.AppendLine("           ,CASE WHEN @vcGM='' THEN NULL ELSE @vcGM END ");
                strSql_addinfo.AppendLine("           ,'"+strOperId+"' ");
                strSql_addinfo.AppendLine("           ,GETDATE()) ");
                sqlCommand_addinfo.CommandText = strSql_addinfo.ToString();
                sqlCommand_addinfo.Parameters.AddWithValue("@LinId", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcSPISStatus", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcApplyId", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@dToTime_SPIS", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@dSPISTime", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcPartId", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcCarfamilyCode", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcSupplierId", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcPartENName", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcColourNo", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcColourCode", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcColourName", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcModItem", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcPicUrl", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcPDFUrl", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcSPISUrl", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcSupplier_1", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcSupplier_2", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcOperName", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcGM", "");
                #endregion
                foreach (DataRow item in dtImport.Rows)
                {
                    sqlCommand_addinfo.Parameters["@LinId"].Value = item["LinId"].ToString();
                    sqlCommand_addinfo.Parameters["@vcApplyId"].Value = item["vcApplyId"].ToString();
                    sqlCommand_addinfo.Parameters["@dToTime_SPIS"].Value = item["dToTime_SPIS"].ToString();
                    sqlCommand_addinfo.Parameters["@dSPISTime"].Value = item["dSPISTime"].ToString();
                    sqlCommand_addinfo.Parameters["@vcPartId"].Value = item["vcPartId"].ToString();
                    sqlCommand_addinfo.Parameters["@vcCarfamilyCode"].Value = item["vcCarfamilyCode"].ToString();
                    sqlCommand_addinfo.Parameters["@vcSupplierId"].Value = item["vcSupplierId"].ToString();
                    sqlCommand_addinfo.Parameters["@vcPartENName"].Value = item["vcPartENName"].ToString();
                    sqlCommand_addinfo.Parameters["@vcColourNo"].Value = item["vcColourNo"].ToString();
                    sqlCommand_addinfo.Parameters["@vcColourCode"].Value = item["vcColourCode"].ToString();
                    sqlCommand_addinfo.Parameters["@vcColourName"].Value = item["vcColourName"].ToString();
                    sqlCommand_addinfo.Parameters["@vcModItem"].Value = item["vcModItem"].ToString();
                    sqlCommand_addinfo.Parameters["@vcPicUrl"].Value = item["vcPicUrl"].ToString();
                    sqlCommand_addinfo.Parameters["@vcPDFUrl"].Value = item["vcPDFUrl"].ToString();
                    sqlCommand_addinfo.Parameters["@vcSPISUrl"].Value = item["vcSPISUrl"].ToString();
                    sqlCommand_addinfo.Parameters["@vcSupplier_1"].Value = item["vcSupplier_1"].ToString();
                    sqlCommand_addinfo.Parameters["@vcSupplier_2"].Value = item["vcSupplier_2"].ToString();
                    sqlCommand_addinfo.Parameters["@vcOperName"].Value = item["vcOperName"].ToString();
                    sqlCommand_addinfo.Parameters["@vcGM"].Value = item["vcGM"].ToString();
                    sqlCommand_addinfo.Parameters["@vcSPISStatus"].Value = item["vcSPISStatus"].ToString();
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
                strSql_modinfo.AppendLine("UPDATE tCheckMethod_Master SET vcSPISStatus=@vcSPISStatus WHERE LinId=@LinId ");
                strSql_modinfo.AppendLine(" update TSPISApply set vcPicUrl=@vcPicUrl,vcPDFUrl=@vcPDFUrl,vcSPISUrl=@vcSPISUrl,vcOperName=@vcOperName, vcGM=@vcGM,vcOperatorID='"+ strOperId + "',dOperatorTime=GETDATE() where vcApplyId=@vcApplyId");
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
                strSql_modinfo.AppendLine("UPDATE tCheckMethod_Master SET vcSPISStatus=@vcSPISStatus WHERE LinId=@LinId ");
                strSql_modinfo.AppendLine(" update TSPISApply set vcPicUrl=NULL,vcPDFUrl=NULL,vcSPISUrl=NULL,vcOperName=NULL, vcGM=NULL,vcOperatorID='" + strOperId + "',dOperatorTime=GETDATE() where vcApplyId=@vcApplyId");
                sqlCommand_modinfo.CommandText = strSql_modinfo.ToString();
                sqlCommand_modinfo.Parameters.AddWithValue("@LinId", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcSPISStatus", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcApplyId", "");
                #endregion
                foreach (DataRow item in dtImport.Rows)
                {
                    sqlCommand_modinfo.Parameters["@LinId"].Value = item["LinId"].ToString();
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
