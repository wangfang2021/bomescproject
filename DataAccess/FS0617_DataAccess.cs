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
    public class FS0617_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        public DataTable getSearchInfo(string strOrderPlant, string strPartId, string strCarModel, string strReceiver, string strSupplier)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("SELECT  1 AS iEnum,T1.LinId,");
                strSql.AppendLine("		T1.vcPackingPlant,T7.vcName as vcPackingPlant_name,");
                strSql.AppendLine("		T5.vcOrderPlant,		");
                strSql.AppendLine("		T12.vcName as vcOrderPlant_name,	");
                strSql.AppendLine("		T1.vcPartId,");
                strSql.AppendLine("		Convert(varchar(10),T1.dFromTime,111) as dFromTime,");
                strSql.AppendLine("		Convert(varchar(10),T1.dToTime,111) as dToTime,");
                strSql.AppendLine("		T1.vcCarfamilyCode,");
                strSql.AppendLine("		T1.vcReceiver,");
                strSql.AppendLine("		T1.vcSupplierId,");
                strSql.AppendLine("		T4.vcSufferIn,");
                strSql.AppendLine("		CAST(T3.iPackingQty AS varchar(10)) AS iPackingQty,");
                strSql.AppendLine("		T1.vcPartENName,");
                strSql.AppendLine("		T1.vcPassProject,");
                strSql.AppendLine("		T1.vcInteriorProject,");
                strSql.AppendLine("		Convert(varchar(16),T1.dFrontProjectTime,111) as dFrontProjectTime,");
                strSql.AppendLine("		Convert(varchar(16),T1.dShipmentTime,111) as dShipmentTime,");
                strSql.AppendLine("		T1.vcRemark1,");
                strSql.AppendLine("		T1.vcRemark2,");
                strSql.AppendLine("		T1.vcPartImage AS vcPartImage,");
                strSql.AppendLine("		'0' as bModFlag,'0' as bAddFlag,'1' as bSelectFlag,'' as vcBgColor  FROM");
                strSql.AppendLine("(SELECT * FROM [TSPMaster] WHERE vcInOut='0'");
                if (strPartId != "")
                {
                    strSql.AppendLine("AND vcPartId like '" + strPartId + "%'");
                }
                if (strCarModel != "")
                {
                    strSql.AppendLine("AND vcCarfamilyCode='" + strCarModel + "'");
                }
                if (strReceiver != "")
                {
                    strSql.AppendLine("AND vcReceiver='" + strReceiver + "'");
                }
                if (strSupplier != "")
                {
                    strSql.AppendLine("AND vcSupplierId='" + strSupplier + "'");
                }
                strSql.AppendLine(")T1");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT *  FROM [TSPMaster_SupplierPlant] WHERE [vcOperatorType]='1' AND [dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))T2");
                strSql.AppendLine("ON T1.[vcPackingPlant]=T2.[vcPackingPlant] AND T1.[vcPartId]=T2.[vcPartId] AND T1.[vcReceiver]=T2.[vcReceiver] AND T1.[vcSupplierId]=T2.[vcSupplierId]");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT *  FROM [TSPMaster_Box] WHERE [vcOperatorType]='1' AND [dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))T3");
                strSql.AppendLine("ON T1.[vcPackingPlant]=T3.[vcPackingPlant] AND T1.[vcPartId]=T3.[vcPartId] AND T1.[vcReceiver]=T3.[vcReceiver] AND T1.[vcSupplierId]=T3.[vcSupplierId]");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT *  FROM [TSPMaster_SufferIn] WHERE [vcOperatorType]='1' AND [dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))T4");
                strSql.AppendLine("ON T1.[vcPackingPlant]=T4.[vcPackingPlant] AND T1.[vcPartId]=T4.[vcPartId] AND T1.[vcReceiver]=T4.[vcReceiver] AND T1.[vcSupplierId]=T4.[vcSupplierId] ");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(select vcValue1 as [vcSupplierId],vcValue2 as vcSupplierPlant,vcValue3 as [dFromTime],vcValue4 as [dToTime],vcValue5 as vcOrderPlant from TOutCode where vcCodeId='C010' and vcIsColum='0'");
                strSql.AppendLine("and vcValue3<=CONVERT(VARCHAR(10),GETDATE(),23) AND vcValue4>=CONVERT(VARCHAR(10),GETDATE(),23))T5");
                strSql.AppendLine("ON T1.[vcSupplierId]=T5.[vcSupplierId] AND T2.vcSupplierPlant=T5.vcSupplierPlant");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C017')T7--包装工厂");
                strSql.AppendLine("ON T1.vcPackingPlant=T7.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C000')T12--发注工厂");
                strSql.AppendLine("ON T5.vcOrderPlant=T12.vcValue");
                strSql.AppendLine("WHERE 1=1");
                if (strOrderPlant != "")
                {
                    strSql.AppendLine("WHERE T5.vcOrderPlant='" + strOrderPlant + "'");
                }
                strSql.AppendLine("ORDER BY T1.vcPackingPlant,T5.vcOrderPlant,T1.vcReceiver,T1.vcSupplierId,T1.vcPartId");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
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
        public void setPrintTemp(DataTable dtMain, DataTable dtSub, string strOperId, ref DataTable dtMessage)
        {
            SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();

            sqlConnection.Open();
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            try
            {
                #region 写入数据库
                #region sqlCommand_main
                SqlCommand sqlCommand_main = sqlConnection.CreateCommand();
                sqlCommand_main.Transaction = sqlTransaction;
                sqlCommand_main.CommandType = CommandType.Text;
                StringBuilder strSql_main = new StringBuilder();

                #region SQL and Parameters
                strSql_main.AppendLine("INSERT INTO [dbo].[tPrintTemp_main_FS0617]");
                strSql_main.AppendLine("           ([UUID1]");
                strSql_main.AppendLine("           ,[UUID2]");
                strSql_main.AppendLine("           ,[UUID3]");
                strSql_main.AppendLine("           ,[vcOperator]");
                strSql_main.AppendLine("           ,[dOperatorTime])");
                strSql_main.AppendLine("     VALUES");
                strSql_main.AppendLine("           (case when @UUID1='' then null else @UUID1 end");
                strSql_main.AppendLine("           ,case when @UUID2='' then null else @UUID2 end");
                strSql_main.AppendLine("           ,case when @UUID3='' then null else @UUID3 end");
                strSql_main.AppendLine("           ,'"+strOperId+"'");
                strSql_main.AppendLine("           ,GETDATE())");
                sqlCommand_main.CommandText = strSql_main.ToString();
                sqlCommand_main.Parameters.AddWithValue("@UUID1", "");
                sqlCommand_main.Parameters.AddWithValue("@UUID2", "");
                sqlCommand_main.Parameters.AddWithValue("@UUID3", "");
                #endregion
                foreach (DataRow item in dtMain.Rows)
                {
                    #region Value
                    sqlCommand_main.Parameters["@UUID1"].Value = item["UUID1"].ToString();
                    sqlCommand_main.Parameters["@UUID2"].Value = item["UUID2"].ToString();
                    sqlCommand_main.Parameters["@UUID3"].Value = item["UUID3"].ToString();
                    #endregion
                    sqlCommand_main.ExecuteNonQuery();
                }
                #endregion

                #region sqlCommand_sub
                SqlCommand sqlCommand_sub = sqlConnection.CreateCommand();
                sqlCommand_sub.Transaction = sqlTransaction;
                sqlCommand_sub.CommandType = CommandType.Text;
                StringBuilder strSql_sub = new StringBuilder();

                #region SQL and Parameters
                strSql_sub.AppendLine("INSERT INTO [dbo].[tPrintTemp_FS0617]");
                strSql_sub.AppendLine("           ([UUID]");
                strSql_sub.AppendLine("           ,[vcOperator]");
                strSql_sub.AppendLine("           ,[dOperatorTime]");
                strSql_sub.AppendLine("           ,[vcSupplierId]");
                strSql_sub.AppendLine("           ,[vcReceiver]");
                strSql_sub.AppendLine("           ,[vcCarfamilyCode]");
                strSql_sub.AppendLine("           ,[vcBF]");
                strSql_sub.AppendLine("           ,[vcPartId]");
                strSql_sub.AppendLine("           ,[vcPartENName]");
                strSql_sub.AppendLine("           ,[iPackingQty]");
                strSql_sub.AppendLine("           ,[vcSufferIn]");
                strSql_sub.AppendLine("           ,[vcInteriorProject]");
                strSql_sub.AppendLine("           ,[dFrontProjectTime]");
                strSql_sub.AppendLine("           ,[dShipmentTime]");
                strSql_sub.AppendLine("           ,[vcRemark1]");
                strSql_sub.AppendLine("           ,[vcRemark2]");
                strSql_sub.AppendLine("           ,[vcKanBanNo]");
                strSql_sub.AppendLine("           ,[vcPartImage]");
                strSql_sub.AppendLine("           ,[vcCodemage])");
                strSql_sub.AppendLine("     VALUES");
                strSql_sub.AppendLine("           (@UUID");
                strSql_sub.AppendLine("           ,'"+strOperId+"'");
                strSql_sub.AppendLine("           ,GETDATE()");
                strSql_sub.AppendLine("           ,@vcSupplierId");
                strSql_sub.AppendLine("           ,@vcReceiver");
                strSql_sub.AppendLine("           ,@vcCarfamilyCode");
                strSql_sub.AppendLine("           ,@vcBF");
                strSql_sub.AppendLine("           ,@vcPartId");
                strSql_sub.AppendLine("           ,@vcPartENName");
                strSql_sub.AppendLine("           ,@iPackingQty");
                strSql_sub.AppendLine("           ,@vcSufferIn");
                strSql_sub.AppendLine("           ,@vcInteriorProject");
                strSql_sub.AppendLine("           ,@dFrontProjectTime");
                strSql_sub.AppendLine("           ,@dShipmentTime");
                strSql_sub.AppendLine("           ,@vcRemark1");
                strSql_sub.AppendLine("           ,@vcRemark2");
                strSql_sub.AppendLine("           ,@vcKanBanNo");
                strSql_sub.AppendLine("           ,@vcPartImage");
                strSql_sub.AppendLine("           ,@vcCodemage)");
                sqlCommand_sub.CommandText = strSql_sub.ToString();
                sqlCommand_sub.Parameters.AddWithValue("@UUID", "");
                sqlCommand_sub.Parameters.AddWithValue("@vcSupplierId", "");
                sqlCommand_sub.Parameters.AddWithValue("@vcReceiver", "");
                sqlCommand_sub.Parameters.AddWithValue("@vcCarfamilyCode", "");
                sqlCommand_sub.Parameters.AddWithValue("@vcBF", "");
                sqlCommand_sub.Parameters.AddWithValue("@vcPartId", "");
                sqlCommand_sub.Parameters.AddWithValue("@vcPartENName", "");
                sqlCommand_sub.Parameters.AddWithValue("@iPackingQty", "");
                sqlCommand_sub.Parameters.AddWithValue("@vcSufferIn", "");
                sqlCommand_sub.Parameters.AddWithValue("@vcInteriorProject", "");
                sqlCommand_sub.Parameters.AddWithValue("@dFrontProjectTime", "");
                sqlCommand_sub.Parameters.AddWithValue("@dShipmentTime", "");
                sqlCommand_sub.Parameters.AddWithValue("@vcRemark1", "");
                sqlCommand_sub.Parameters.AddWithValue("@vcRemark2", "");
                sqlCommand_sub.Parameters.AddWithValue("@vcKanBanNo", "");
                sqlCommand_sub.Parameters.Add("@vcPartImage", SqlDbType.Image);
                sqlCommand_sub.Parameters.Add("@vcCodemage", SqlDbType.Image);
                #endregion
                foreach (DataRow item in dtSub.Rows)
                {
                    #region Value
                    sqlCommand_sub.Parameters["@UUID"].Value = item["UUID"].ToString();
                    sqlCommand_sub.Parameters["@vcSupplierId"].Value = item["vcSupplierId"].ToString();
                    sqlCommand_sub.Parameters["@vcReceiver"].Value = item["vcReceiver"].ToString();
                    sqlCommand_sub.Parameters["@vcCarfamilyCode"].Value = item["vcCarfamilyCode"].ToString();
                    sqlCommand_sub.Parameters["@vcBF"].Value = item["vcBF"].ToString();
                    sqlCommand_sub.Parameters["@vcPartId"].Value = item["vcPartId"].ToString();
                    sqlCommand_sub.Parameters["@vcPartENName"].Value = item["vcPartENName"].ToString();
                    sqlCommand_sub.Parameters["@iPackingQty"].Value = item["iPackingQty"].ToString();
                    sqlCommand_sub.Parameters["@vcSufferIn"].Value = item["vcSufferIn"].ToString();
                    sqlCommand_sub.Parameters["@vcInteriorProject"].Value = item["vcInteriorProject"].ToString();
                    sqlCommand_sub.Parameters["@dFrontProjectTime"].Value = item["dFrontProjectTime"].ToString();
                    sqlCommand_sub.Parameters["@dShipmentTime"].Value = item["dShipmentTime"].ToString();
                    sqlCommand_sub.Parameters["@vcRemark1"].Value = item["vcRemark1"].ToString();
                    sqlCommand_sub.Parameters["@vcRemark2"].Value = item["vcRemark2"].ToString();
                    sqlCommand_sub.Parameters["@vcKanBanNo"].Value = item["vcKanBanNo"].ToString();
                    sqlCommand_sub.Parameters["@vcPartImage"].Value = item["vcPartImage"];
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
    }
}
