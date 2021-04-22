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
    public class FS1406_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        public DataTable getSearchInfo(string strPartId, string strSupplierId, string strOrderPlant, string strCarModel, string strSPISStatus)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select * from (");
                strSql.AppendLine("select T1.LinId AS LinId");
                strSql.AppendLine("		,t1.vcSPISStatus AS vcSPISStatus");
                strSql.AppendLine("		,case when t1.vcSPISStatus='4' then 0");
                strSql.AppendLine("			  when t1.vcSPISStatus='1' then 1");
                strSql.AppendLine("			  when t1.vcSPISStatus='0' then 2");
                strSql.AppendLine("			  when t1.vcSPISStatus='2' then 3");
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
                if (strOrderPlant != "")
                {
                    strSql.AppendLine("AND vcPartArea='" + strOrderPlant + "'");
                }
                if (strCarModel != "")
                {
                    strSql.AppendLine("AND vcCarfamilyCode='" + strCarModel + "'");
                }
                if (strSPISStatus != "")
                {
                    strSql.AppendLine("AND vcSPISStatus='" + strSPISStatus + "'");
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
                strSql.AppendLine(")tt");
                strSql.AppendLine("order by iOrderBy,vcPartId,dFromTime");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setReplyInfo(DataTable dtImport, string strOperId, ref DataTable dtMessage)
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
                strSql_modinfo.AppendLine("update tCheckMethod_Master set vcSPISStatus=@vcSPISStatus where vcApplyId=@vcApplyId");
                strSql_modinfo.AppendLine("update TSPISApply set vcSPISStatus=@vcSPISStatus,vcOperatorID='" + strOperId + "',dOperatorTime=GETDATE() where vcApplyId=@vcApplyId");
                sqlCommand_modinfo.CommandText = strSql_modinfo.ToString();
                sqlCommand_modinfo.Parameters.AddWithValue("@vcApplyId", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcSPISStatus", "");
                foreach (DataRow item in dtImport.Rows)
                {
                    sqlCommand_modinfo.Parameters["@vcApplyId"].Value = item["vcApplyId"].ToString();
                    sqlCommand_modinfo.Parameters["@vcSPISStatus"].Value = item["vcSPISStatus"].ToString();
                    sqlCommand_modinfo.ExecuteNonQuery();
                }
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
        public void setSaveInfo(DataTable dtImport, string strOperId, ref DataTable dtMessage)
        {
            SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();

            sqlConnection.Open();
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            try
            {
                #region add
                SqlCommand sqlCommand_add = sqlConnection.CreateCommand();
                sqlCommand_add.Transaction = sqlTransaction;
                sqlCommand_add.CommandType = CommandType.Text;
                StringBuilder strSql_add = new StringBuilder();
                strSql_add.AppendLine("update tCheckMethod_Master set vcApplyId=@vcApplyId where vcPartId=@vcPartId and vcSupplierId=@vcSupplierId");
                strSql_add.AppendLine("INSERT INTO [dbo].[TSPISApply]");
                strSql_add.AppendLine("           ([vcApplyId]");
                strSql_add.AppendLine("           ,[dFromTime_SPIS]");
                strSql_add.AppendLine("           ,[dToTime_SPIS]");
                strSql_add.AppendLine("           ,[dSPISTime]");
                strSql_add.AppendLine("           ,[vcPartId]");
                strSql_add.AppendLine("           ,[vcCarfamilyCode]");
                strSql_add.AppendLine("           ,[vcSupplierId]");
                strSql_add.AppendLine("           ,[vcSupplierName]");
                strSql_add.AppendLine("           ,[vcPartENName]");
                strSql_add.AppendLine("           ,[vcColourNo]");
                strSql_add.AppendLine("           ,[vcColourCode]");
                strSql_add.AppendLine("           ,[vcColourName]");
                strSql_add.AppendLine("           ,[dModDate]");
                strSql_add.AppendLine("           ,[vcModItem]");
                strSql_add.AppendLine("           ,[vcPICUrl]");
                strSql_add.AppendLine("           ,[vcPICPath]");
                strSql_add.AppendLine("           ,[vcPDFUrl]");
                strSql_add.AppendLine("           ,[vcPDFPath]");
                strSql_add.AppendLine("           ,[vcSPISUrl]");
                strSql_add.AppendLine("           ,[vcSPISPath]");
                strSql_add.AppendLine("           ,[vcSupplier_1]");
                strSql_add.AppendLine("           ,[vcSupplier_2]");
                strSql_add.AppendLine("           ,[vcOperName]");
                strSql_add.AppendLine("           ,[vcGM]");
                strSql_add.AppendLine("           ,[vcOperatorID]");
                strSql_add.AppendLine("           ,[dOperatorTime])");
                strSql_add.AppendLine("     VALUES");
                strSql_add.AppendLine("           (@vcApplyId");
                strSql_add.AppendLine("           ,@dFromTime_SPIS");
                strSql_add.AppendLine("           ,@dToTime_SPIS");
                strSql_add.AppendLine("           ,@dSPISTime");
                strSql_add.AppendLine("           ,@vcPartId");
                strSql_add.AppendLine("           ,@vcCarfamilyCode");
                strSql_add.AppendLine("           ,@vcSupplierId");
                strSql_add.AppendLine("           ,@vcSupplierName");
                strSql_add.AppendLine("           ,@vcPartENName");
                strSql_add.AppendLine("           ,@vcColourNo");
                strSql_add.AppendLine("           ,@vcColourCode");
                strSql_add.AppendLine("           ,@vcColourName");
                strSql_add.AppendLine("           ,@dModDate");
                strSql_add.AppendLine("           ,@vcModItem");
                strSql_add.AppendLine("           ,@vcPICUrl");
                strSql_add.AppendLine("           ,@vcPICPath");
                strSql_add.AppendLine("           ,@vcPDFUrl");
                strSql_add.AppendLine("           ,@vcPDFPath");
                strSql_add.AppendLine("           ,@vcSPISUrl");
                strSql_add.AppendLine("           ,@vcSPISPath");
                strSql_add.AppendLine("           ,@vcSupplier_1");
                strSql_add.AppendLine("           ,@vcSupplier_2");
                strSql_add.AppendLine("           ,@vcOperName");
                strSql_add.AppendLine("           ,@vcGM");
                strSql_add.AppendLine("           ,'" + strOperId + "'");
                strSql_add.AppendLine("           ,GETDATE())");
                sqlCommand_add.CommandText = strSql_add.ToString();
                sqlCommand_add.Parameters.AddWithValue("@vcApplyId", "");
                sqlCommand_add.Parameters.AddWithValue("@dFromTime_SPIS", "");
                sqlCommand_add.Parameters.AddWithValue("@dToTime_SPIS", "");
                sqlCommand_add.Parameters.AddWithValue("@dSPISTime", "");
                sqlCommand_add.Parameters.AddWithValue("@vcPartId", "");
                sqlCommand_add.Parameters.AddWithValue("@vcCarfamilyCode", "");
                sqlCommand_add.Parameters.AddWithValue("@vcSupplierId", "");
                sqlCommand_add.Parameters.AddWithValue("@vcSupplierName", "");
                sqlCommand_add.Parameters.AddWithValue("@vcPartENName", "");
                sqlCommand_add.Parameters.AddWithValue("@vcColourNo", "");
                sqlCommand_add.Parameters.AddWithValue("@vcColourCode", "");
                sqlCommand_add.Parameters.AddWithValue("@vcColourName", "");
                sqlCommand_add.Parameters.AddWithValue("@dModDate", "");
                sqlCommand_add.Parameters.AddWithValue("@vcModItem", "");
                sqlCommand_add.Parameters.AddWithValue("@vcPICUrl", "");
                sqlCommand_add.Parameters.AddWithValue("@vcPICPath", "");
                sqlCommand_add.Parameters.AddWithValue("@vcPDFUrl", "");
                sqlCommand_add.Parameters.AddWithValue("@vcPDFPath", "");
                sqlCommand_add.Parameters.AddWithValue("@vcSPISUrl", "");
                sqlCommand_add.Parameters.AddWithValue("@vcSPISPath", "");
                sqlCommand_add.Parameters.AddWithValue("@vcSupplier_1", "");
                sqlCommand_add.Parameters.AddWithValue("@vcSupplier_2", "");
                sqlCommand_add.Parameters.AddWithValue("@vcOperName", "");
                sqlCommand_add.Parameters.AddWithValue("@vcGM", "");
                foreach (DataRow item in dtImport.Rows)
                {
                    if (item["vcType"].ToString() == "add")
                    {
                        sqlCommand_add.Parameters["@vcApplyId"].Value = item["vcApplyId"].ToString();
                        sqlCommand_add.Parameters["@dFromTime_SPIS"].Value = item["dFromTime_SPIS"].ToString();
                        sqlCommand_add.Parameters["@dToTime_SPIS"].Value = item["dToTime_SPIS"].ToString();
                        sqlCommand_add.Parameters["@dSPISTime"].Value = item["dSPISTime"].ToString();
                        sqlCommand_add.Parameters["@vcPartId"].Value = item["vcPartId"].ToString();
                        sqlCommand_add.Parameters["@vcCarfamilyCode"].Value = item["vcCarfamilyCode"].ToString();
                        sqlCommand_add.Parameters["@vcSupplierId"].Value = item["vcSupplierId"].ToString();
                        sqlCommand_add.Parameters["@vcSupplierName"].Value = item["vcSupplierName"].ToString();
                        sqlCommand_add.Parameters["@vcPartENName"].Value = item["vcPartENName"].ToString();
                        sqlCommand_add.Parameters["@vcColourNo"].Value = item["vcColourNo"].ToString();
                        sqlCommand_add.Parameters["@vcColourCode"].Value = item["vcColourCode"].ToString();
                        sqlCommand_add.Parameters["@vcColourName"].Value = item["vcColourName"].ToString();
                        sqlCommand_add.Parameters["@dModDate"].Value = item["dModDate"].ToString();
                        sqlCommand_add.Parameters["@vcModItem"].Value = item["vcModItem"].ToString();
                        sqlCommand_add.Parameters["@vcPICUrl"].Value = item["vcPICUrl"].ToString();
                        sqlCommand_add.Parameters["@vcPICPath"].Value = item["vcPICPath"].ToString();
                        sqlCommand_add.Parameters["@vcPDFUrl"].Value = item["vcPDFUrl"].ToString();
                        sqlCommand_add.Parameters["@vcPDFPath"].Value = item["vcPDFPath"].ToString();
                        sqlCommand_add.Parameters["@vcSPISUrl"].Value = item["vcSPISUrl"].ToString();
                        sqlCommand_add.Parameters["@vcSPISPath"].Value = item["vcSPISPath"].ToString();
                        sqlCommand_add.Parameters["@vcSupplier_1"].Value = item["vcSupplier_1"].ToString();
                        sqlCommand_add.Parameters["@vcSupplier_2"].Value = item["vcSupplier_2"].ToString();
                        sqlCommand_add.Parameters["@vcOperName"].Value = item["vcOperName"].ToString();
                        sqlCommand_add.Parameters["@vcGM"].Value = item["vcGM"].ToString();
                        sqlCommand_add.ExecuteNonQuery();
                    }
                }
                #endregion

                #region mod
                SqlCommand sqlCommand_mod = sqlConnection.CreateCommand();
                sqlCommand_mod.Transaction = sqlTransaction;
                sqlCommand_mod.CommandType = CommandType.Text;
                StringBuilder strSql_mod = new StringBuilder();
                strSql_mod.AppendLine("UPDATE [dbo].[TSPISApply]");
                strSql_mod.AppendLine("   SET [dFromTime_SPIS] =@dFromTime_SPIS");
                strSql_mod.AppendLine("      ,[dToTime_SPIS] =@dToTime_SPIS");
                strSql_mod.AppendLine("      ,[dSPISTime] = @dSPISTime");
                strSql_mod.AppendLine("      ,[vcPartId] = @vcPartId");
                strSql_mod.AppendLine("      ,[vcCarfamilyCode] = @vcCarfamilyCode");
                strSql_mod.AppendLine("      ,[vcSupplierId] =@vcSupplierId");
                strSql_mod.AppendLine("      ,[vcSupplierName] = @vcSupplierName");
                strSql_mod.AppendLine("      ,[vcPartENName] =@vcPartENName");
                strSql_mod.AppendLine("      ,[vcColourNo] = @vcColourNo");
                strSql_mod.AppendLine("      ,[vcColourCode] = @vcColourCode");
                strSql_mod.AppendLine("      ,[vcColourName] = @vcColourName");
                strSql_mod.AppendLine("      ,[dModDate] = @dModDate");
                strSql_mod.AppendLine("      ,[vcModItem] = @vcModItem");
                strSql_mod.AppendLine("      ,[vcPICUrl] = @vcPICUrl");
                strSql_mod.AppendLine("      ,[vcPICPath] = @vcPICPath");
                strSql_mod.AppendLine("      ,[vcPDFUrl] = @vcPDFUrl");
                strSql_mod.AppendLine("      ,[vcPDFPath] = @vcPDFPath");
                strSql_mod.AppendLine("      ,[vcSPISUrl] = @vcSPISUrl");
                strSql_mod.AppendLine("      ,[vcSPISPath] = @vcSPISPath");
                strSql_mod.AppendLine("      ,[vcSupplier_1] = @vcSupplier_1");
                strSql_mod.AppendLine("      ,[vcSupplier_2] = @vcSupplier_2");
                strSql_mod.AppendLine("      ,[vcOperName] = @vcOperName");
                strSql_mod.AppendLine("      ,[vcGM] = @vcGM");
                strSql_mod.AppendLine("      ,[vcOperatorID] ='" + strOperId + "'");
                strSql_mod.AppendLine("      ,[dOperatorTime] = GETDATE()");
                strSql_mod.AppendLine(" WHERE [vcApplyId] = @vcApplyId");
                sqlCommand_mod.CommandText = strSql_mod.ToString();
                sqlCommand_mod.Parameters.AddWithValue("@vcApplyId", "");
                sqlCommand_mod.Parameters.AddWithValue("@dFromTime_SPIS", "");
                sqlCommand_mod.Parameters.AddWithValue("@dToTime_SPIS", "");
                sqlCommand_mod.Parameters.AddWithValue("@dSPISTime", "");
                sqlCommand_mod.Parameters.AddWithValue("@vcPartId", "");
                sqlCommand_mod.Parameters.AddWithValue("@vcCarfamilyCode", "");
                sqlCommand_mod.Parameters.AddWithValue("@vcSupplierId", "");
                sqlCommand_mod.Parameters.AddWithValue("@vcSupplierName", "");
                sqlCommand_mod.Parameters.AddWithValue("@vcPartENName", "");
                sqlCommand_mod.Parameters.AddWithValue("@vcColourNo", "");
                sqlCommand_mod.Parameters.AddWithValue("@vcColourCode", "");
                sqlCommand_mod.Parameters.AddWithValue("@vcColourName", "");
                sqlCommand_mod.Parameters.AddWithValue("@dModDate", "");
                sqlCommand_mod.Parameters.AddWithValue("@vcModItem", "");
                sqlCommand_mod.Parameters.AddWithValue("@vcPICUrl", "");
                sqlCommand_mod.Parameters.AddWithValue("@vcPICPath", "");
                sqlCommand_mod.Parameters.AddWithValue("@vcPDFUrl", "");
                sqlCommand_mod.Parameters.AddWithValue("@vcPDFPath", "");
                sqlCommand_mod.Parameters.AddWithValue("@vcSPISUrl", "");
                sqlCommand_mod.Parameters.AddWithValue("@vcSPISPath", "");
                sqlCommand_mod.Parameters.AddWithValue("@vcSupplier_1", "");
                sqlCommand_mod.Parameters.AddWithValue("@vcSupplier_2", "");
                sqlCommand_mod.Parameters.AddWithValue("@vcOperName", "");
                sqlCommand_mod.Parameters.AddWithValue("@vcGM", "");
                foreach (DataRow item in dtImport.Rows)
                {
                    if (item["vcType"].ToString() == "mod")
                    {
                        sqlCommand_mod.Parameters["@vcApplyId"].Value = item["vcApplyId"].ToString();
                        sqlCommand_mod.Parameters["@dFromTime_SPIS"].Value = item["dFromTime_SPIS"].ToString();
                        sqlCommand_mod.Parameters["@dToTime_SPIS"].Value = item["dToTime_SPIS"].ToString();
                        sqlCommand_mod.Parameters["@dSPISTime"].Value = item["dSPISTime"].ToString();
                        sqlCommand_mod.Parameters["@vcPartId"].Value = item["vcPartId"].ToString();
                        sqlCommand_mod.Parameters["@vcCarfamilyCode"].Value = item["vcCarfamilyCode"].ToString();
                        sqlCommand_mod.Parameters["@vcSupplierId"].Value = item["vcSupplierId"].ToString();
                        sqlCommand_mod.Parameters["@vcSupplierName"].Value = item["vcSupplierName"].ToString();
                        sqlCommand_mod.Parameters["@vcPartENName"].Value = item["vcPartENName"].ToString();
                        sqlCommand_mod.Parameters["@vcColourNo"].Value = item["vcColourNo"].ToString();
                        sqlCommand_mod.Parameters["@vcColourCode"].Value = item["vcColourCode"].ToString();
                        sqlCommand_mod.Parameters["@vcColourName"].Value = item["vcColourName"].ToString();
                        sqlCommand_mod.Parameters["@dModDate"].Value = item["dModDate"].ToString();
                        sqlCommand_mod.Parameters["@vcModItem"].Value = item["vcModItem"].ToString();
                        sqlCommand_mod.Parameters["@vcPICUrl"].Value = item["vcPICUrl"].ToString();
                        sqlCommand_mod.Parameters["@vcPICPath"].Value = item["vcPICPath"].ToString();
                        sqlCommand_mod.Parameters["@vcPDFUrl"].Value = item["vcPDFUrl"].ToString();
                        sqlCommand_mod.Parameters["@vcPDFPath"].Value = item["vcPDFPath"].ToString();
                        sqlCommand_mod.Parameters["@vcSPISUrl"].Value = item["vcSPISUrl"].ToString();
                        sqlCommand_mod.Parameters["@vcSPISPath"].Value = item["vcSPISPath"].ToString();
                        sqlCommand_mod.Parameters["@vcSupplier_1"].Value = item["vcSupplier_1"].ToString();
                        sqlCommand_mod.Parameters["@vcSupplier_2"].Value = item["vcSupplier_2"].ToString();
                        sqlCommand_mod.Parameters["@vcOperName"].Value = item["vcOperName"].ToString();
                        sqlCommand_mod.Parameters["@vcGM"].Value = item["vcGM"].ToString();
                        sqlCommand_mod.ExecuteNonQuery();
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

        public DataTable getPrintTemp(string strPage)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select top(1)* from [tPrintTemp_" + strPage + "]");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setPrintTemp(DataRow drPDF_temp, string strOperId, ref DataTable dtMessage)
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
                strSql_deleteinfo.AppendLine("DELETE from tPrintTemp_FS1406 where vcOperator='" + strOperId + "'");
                sqlCommand_deleteinfo.CommandText = strSql_deleteinfo.ToString();
                #endregion
                sqlCommand_deleteinfo.ExecuteNonQuery();
                #endregion

                #region sqlCommand_add
                SqlCommand sqlCommand_add = sqlConnection.CreateCommand();
                sqlCommand_add.Transaction = sqlTransaction;
                sqlCommand_add.CommandType = CommandType.Text;
                StringBuilder strSql_add = new StringBuilder();

                #region SQL and Parameters
                strSql_add.AppendLine("INSERT INTO [dbo].[tPrintTemp_FS1406]");
                strSql_add.AppendLine("           ([UUID]");
                strSql_add.AppendLine("           ,[vcOperator]");
                strSql_add.AppendLine("           ,[dOperatorTime]");
                strSql_add.AppendLine("           ,[vcSPISTime]");
                strSql_add.AppendLine("           ,[vcCarfamilyCode]");
                strSql_add.AppendLine("           ,[vcSupplierId]");
                strSql_add.AppendLine("           ,[vcSupplierName]");
                strSql_add.AppendLine("           ,[vcPartId]");
                strSql_add.AppendLine("           ,[vcPartENName]");
                strSql_add.AppendLine("           ,[vcColourNo]");
                strSql_add.AppendLine("           ,[vcColourCode]");
                strSql_add.AppendLine("           ,[vcColourName]");
                strSql_add.AppendLine("           ,[vcPICImage]");
                strSql_add.AppendLine("           ,[vcModDate1]");
                strSql_add.AppendLine("           ,[vcModItem1]");
                strSql_add.AppendLine("           ,[vcModDate2]");
                strSql_add.AppendLine("           ,[vcModItem2]");
                strSql_add.AppendLine("           ,[vcModDate3]");
                strSql_add.AppendLine("           ,[vcModItem3]");
                strSql_add.AppendLine("           ,[vcSupplier_1]");
                strSql_add.AppendLine("           ,[vcSupplier_2]");
                strSql_add.AppendLine("           ,[vcOperName]");
                strSql_add.AppendLine("           ,[vcGM]");
                strSql_add.AppendLine("           ,[vcPDFPath])");
                strSql_add.AppendLine("     VALUES");
                strSql_add.AppendLine("           (@UUID");
                strSql_add.AppendLine("           ,'" + strOperId + "'");
                strSql_add.AppendLine("           ,GETDATE()");
                strSql_add.AppendLine("           ,@vcSPISTime");
                strSql_add.AppendLine("           ,@vcCarfamilyCode");
                strSql_add.AppendLine("           ,@vcSupplierId");
                strSql_add.AppendLine("           ,@vcSupplierName");
                strSql_add.AppendLine("           ,@vcPartId");
                strSql_add.AppendLine("           ,@vcPartENName");
                strSql_add.AppendLine("           ,@vcColourNo");
                strSql_add.AppendLine("           ,@vcColourCode");
                strSql_add.AppendLine("           ,@vcColourName");
                strSql_add.AppendLine("           ,@vcPICImage");
                strSql_add.AppendLine("           ,@vcModDate1");
                strSql_add.AppendLine("           ,@vcModItem1");
                strSql_add.AppendLine("           ,@vcModDate2");
                strSql_add.AppendLine("           ,@vcModItem2");
                strSql_add.AppendLine("           ,@vcModDate3");
                strSql_add.AppendLine("           ,@vcModItem3");
                strSql_add.AppendLine("           ,@vcSupplier_1");
                strSql_add.AppendLine("           ,@vcSupplier_2");
                strSql_add.AppendLine("           ,@vcOperName");
                strSql_add.AppendLine("           ,@vcGM");
                strSql_add.AppendLine("           ,@vcPDFPath)");
                sqlCommand_add.CommandText = strSql_add.ToString();
                sqlCommand_add.Parameters.AddWithValue("@UUID", "");
                sqlCommand_add.Parameters.AddWithValue("@vcSPISTime", "");
                sqlCommand_add.Parameters.AddWithValue("@vcCarfamilyCode", "");
                sqlCommand_add.Parameters.AddWithValue("@vcSupplierId", "");
                sqlCommand_add.Parameters.AddWithValue("@vcSupplierName", "");
                sqlCommand_add.Parameters.AddWithValue("@vcPartId", "");
                sqlCommand_add.Parameters.AddWithValue("@vcPartENName", "");
                sqlCommand_add.Parameters.AddWithValue("@vcColourNo", "");
                sqlCommand_add.Parameters.AddWithValue("@vcColourCode", "");
                sqlCommand_add.Parameters.AddWithValue("@vcColourName", "");
                sqlCommand_add.Parameters.Add("@vcPICImage", SqlDbType.Image);
                sqlCommand_add.Parameters.AddWithValue("@vcModDate1", "");
                sqlCommand_add.Parameters.AddWithValue("@vcModItem1", "");
                sqlCommand_add.Parameters.AddWithValue("@vcModDate2", "");
                sqlCommand_add.Parameters.AddWithValue("@vcModItem2", "");
                sqlCommand_add.Parameters.AddWithValue("@vcModDate3", "");
                sqlCommand_add.Parameters.AddWithValue("@vcModItem3", "");
                sqlCommand_add.Parameters.AddWithValue("@vcSupplier_1", "");
                sqlCommand_add.Parameters.AddWithValue("@vcSupplier_2", "");
                sqlCommand_add.Parameters.AddWithValue("@vcOperName", "");
                sqlCommand_add.Parameters.AddWithValue("@vcGM", "");
                sqlCommand_add.Parameters.AddWithValue("@vcPDFPath", "");
                #endregion
                #region Value
                sqlCommand_add.Parameters["@UUID"].Value = drPDF_temp["UUID"].ToString();
                sqlCommand_add.Parameters["@vcSPISTime"].Value = drPDF_temp["vcSPISTime"].ToString();
                sqlCommand_add.Parameters["@vcCarfamilyCode"].Value = drPDF_temp["vcCarfamilyCode"].ToString();
                sqlCommand_add.Parameters["@vcSupplierId"].Value = drPDF_temp["vcSupplierId"].ToString();
                sqlCommand_add.Parameters["@vcSupplierName"].Value = drPDF_temp["vcSupplierName"].ToString();
                sqlCommand_add.Parameters["@vcPartId"].Value = drPDF_temp["vcPartId"].ToString();
                sqlCommand_add.Parameters["@vcPartENName"].Value = drPDF_temp["vcPartENName"].ToString();
                sqlCommand_add.Parameters["@vcColourNo"].Value = drPDF_temp["vcColourNo"].ToString();
                sqlCommand_add.Parameters["@vcColourCode"].Value = drPDF_temp["vcColourCode"].ToString();
                sqlCommand_add.Parameters["@vcColourName"].Value = drPDF_temp["vcColourName"].ToString();
                sqlCommand_add.Parameters["@vcPICImage"].Value = drPDF_temp["vcPICImage"];
                sqlCommand_add.Parameters["@vcModDate1"].Value = drPDF_temp["vcModDate1"].ToString();
                sqlCommand_add.Parameters["@vcModItem1"].Value = drPDF_temp["vcModItem1"].ToString();
                sqlCommand_add.Parameters["@vcModDate2"].Value = drPDF_temp["vcModDate2"].ToString();
                sqlCommand_add.Parameters["@vcModItem2"].Value = drPDF_temp["vcModItem2"].ToString();
                sqlCommand_add.Parameters["@vcModDate3"].Value = drPDF_temp["vcModDate3"].ToString();
                sqlCommand_add.Parameters["@vcModItem3"].Value = drPDF_temp["vcModItem3"].ToString();
                sqlCommand_add.Parameters["@vcSupplier_1"].Value = drPDF_temp["vcSupplier_1"].ToString();
                sqlCommand_add.Parameters["@vcSupplier_2"].Value = drPDF_temp["vcSupplier_2"].ToString();
                sqlCommand_add.Parameters["@vcOperName"].Value = drPDF_temp["vcOperName"].ToString();
                sqlCommand_add.Parameters["@vcGM"].Value = drPDF_temp["vcGM"].ToString();
                sqlCommand_add.Parameters["@vcPDFPath"].Value = drPDF_temp["vcPDFPath"].ToString();
                #endregion
                sqlCommand_add.ExecuteNonQuery();
                #endregion
                //提交事务
                sqlTransaction.Commit();
                sqlConnection.Close();
                #endregion

            }
            catch (Exception ex)
            {
                //回滚事务
                if (sqlTransaction != null && sqlConnection != null)
                {
                    sqlTransaction.Rollback();
                    sqlConnection.Close();
                }
                throw ex;
            }
        }
    }
}
