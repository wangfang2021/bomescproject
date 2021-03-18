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
                strSql.AppendLine("select T1.LinId AS LinId");
                strSql.AppendLine("		,T1.vcApplyId AS vcApplyId");
                strSql.AppendLine("		,t1.vcSPISStatus AS vcSPISStatus");
                strSql.AppendLine("		,t5.dToTime_SPIS AS dToTime_SPIS");
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
                strSql.AppendLine("where 1=1");
                strSql.AppendLine("order by t1.vcPartId,t1.dFromTime");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void setAdmitInfo(DataTable dtImport, string strOperId, ref DataTable dtMessage)
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
                strSql_modinfo.AppendLine(" ");
                sqlCommand_modinfo.CommandText = strSql_modinfo.ToString();
                sqlCommand_modinfo.Parameters.AddWithValue("@vcYearMonth", "");
                foreach (DataRow item in dtImport.Rows)
                {
                    sqlCommand_modinfo.Parameters["@vcYearMonth"].Value = item["vcYearMonth"].ToString();
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
                SqlCommand sqlCommand_modinfo = sqlConnection.CreateCommand();
                sqlCommand_modinfo.Transaction = sqlTransaction;
                sqlCommand_modinfo.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo = new StringBuilder();
                strSql_modinfo.AppendLine(" ");
                sqlCommand_modinfo.CommandText = strSql_modinfo.ToString();
                sqlCommand_modinfo.Parameters.AddWithValue("@vcYearMonth", "");
                foreach (DataRow item in dtImport.Rows)
                {
                    sqlCommand_modinfo.Parameters["@vcYearMonth"].Value = item["vcYearMonth"].ToString();
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
    }
}
