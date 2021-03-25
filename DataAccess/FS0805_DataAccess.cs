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
    public class FS0805_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        public DataTable getSearchInfo(string strSellNo,string strType)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                if(strType== "Info")
                {
                    strSql.AppendLine("select vcCpdcompany");
                    strSql.AppendLine(",vcCompany");
                    strSql.AppendLine(",vcPackingspot");
                    strSql.AppendLine(",vcControlno");
                    strSql.AppendLine(",vcPart_id as vcPartsno");
                    strSql.AppendLine(",vcOrderno");
                    strSql.AppendLine(",vcSeqno");
                    strSql.AppendLine(",vcInvoiceno");
                    strSql.AppendLine(",vcPartsnamechn");
                    strSql.AppendLine(",vcPartsnameen");
                    strSql.AppendLine(",vcShippingqty");
                    strSql.AppendLine(",vcCaseno");
                    strSql.AppendLine(",vcCostwithtaxes");
                    strSql.AppendLine(",vcPrice,'1' as bSelectFlag from tshiplist");
                    strSql.AppendLine("where vcControlno='"+ strSellNo + "' and dFirstPrintTime is not  null");
                    strSql.AppendLine("order by iAutoId");
                }
                if (strType == "List")
                {
                    strSql.AppendLine("select vcCpdcompany");
                    strSql.AppendLine(",vcCompany");
                    strSql.AppendLine(",vcPackingspot");
                    strSql.AppendLine(",vcControlno");
                    strSql.AppendLine(",vcPart_id as vcPartsno");
                    strSql.AppendLine(",vcOrderno");
                    strSql.AppendLine(",vcSeqno");
                    strSql.AppendLine(",vcInvoiceno");
                    strSql.AppendLine(",vcPartsnamechn");
                    strSql.AppendLine(",vcPartsnameen");
                    strSql.AppendLine(",vcShippingqty");
                    strSql.AppendLine(",vcCaseno");
                    strSql.AppendLine(",vcCostwithtaxes");
                    strSql.AppendLine(",vcPrice,CONVERT(varchar(10),GETDATE(),23) as dPrintDate from tshiplist");
                    strSql.AppendLine("where vcControlno='" + strSellNo + "'");
                    strSql.AppendLine("order by iAutoId");

                }
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
                strSql_deleteinfo.AppendLine("DELETE from tPrintTemp_FS0805 where vcOperator='" + strOperId + "'");
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
                strSql_sub.AppendLine("INSERT INTO [dbo].[tPrintTemp_FS0805]");
                strSql_sub.AppendLine("           ([UUID]");
                strSql_sub.AppendLine("           ,[vcOperator]");
                strSql_sub.AppendLine("           ,[dOperatorTime]");
                strSql_sub.AppendLine("           ,[vcCpdcompany]");
                strSql_sub.AppendLine("           ,[vcCompany]");
                strSql_sub.AppendLine("           ,[vcPackingspot]");
                strSql_sub.AppendLine("           ,[vcControlno]");
                strSql_sub.AppendLine("           ,[vcPartsno]");
                strSql_sub.AppendLine("           ,[vcOrderno]");
                strSql_sub.AppendLine("           ,[vcSeqno]");
                strSql_sub.AppendLine("           ,[vcInvoiceno]");
                strSql_sub.AppendLine("           ,[vcPartsnamechn]");
                strSql_sub.AppendLine("           ,[vcPartsnameen]");
                strSql_sub.AppendLine("           ,[vcShippingqty]");
                strSql_sub.AppendLine("           ,[vcCaseno]");
                strSql_sub.AppendLine("           ,[vcCostwithtaxes]");
                strSql_sub.AppendLine("           ,[vcPrice]");
                strSql_sub.AppendLine("           ,[dPrintDate]");
                strSql_sub.AppendLine("           ,[vcCodemage])");
                strSql_sub.AppendLine("     VALUES");
                strSql_sub.AppendLine("           (@UUID");
                strSql_sub.AppendLine("           ,'"+ strOperId + "'");
                strSql_sub.AppendLine("           ,GETDATE()");
                strSql_sub.AppendLine("           ,@vcCpdcompany");
                strSql_sub.AppendLine("           ,@vcCompany");
                strSql_sub.AppendLine("           ,@vcPackingspot");
                strSql_sub.AppendLine("           ,@vcControlno");
                strSql_sub.AppendLine("           ,@vcPartsno");
                strSql_sub.AppendLine("           ,@vcOrderno");
                strSql_sub.AppendLine("           ,@vcSeqno");
                strSql_sub.AppendLine("           ,@vcInvoiceno");
                strSql_sub.AppendLine("           ,@vcPartsnamechn");
                strSql_sub.AppendLine("           ,@vcPartsnameen");
                strSql_sub.AppendLine("           ,@vcShippingqty");
                strSql_sub.AppendLine("           ,@vcCaseno");
                strSql_sub.AppendLine("           ,@vcCostwithtaxes");
                strSql_sub.AppendLine("           ,@vcPrice");
                strSql_sub.AppendLine("           ,@dPrintDate");
                strSql_sub.AppendLine("           ,@vcCodemage)");
                sqlCommand_sub.CommandText = strSql_sub.ToString();
                sqlCommand_sub.Parameters.AddWithValue("@UUID", "");
                sqlCommand_sub.Parameters.AddWithValue("@vcCpdcompany", "");
                sqlCommand_sub.Parameters.AddWithValue("@vcCompany", "");
                sqlCommand_sub.Parameters.AddWithValue("@vcPackingspot", "");
                sqlCommand_sub.Parameters.AddWithValue("@vcControlno", "");
                sqlCommand_sub.Parameters.AddWithValue("@vcPartsno", "");
                sqlCommand_sub.Parameters.AddWithValue("@vcOrderno", "");
                sqlCommand_sub.Parameters.AddWithValue("@vcSeqno", "");
                sqlCommand_sub.Parameters.AddWithValue("@vcInvoiceno", "");
                sqlCommand_sub.Parameters.AddWithValue("@vcPartsnamechn", "");
                sqlCommand_sub.Parameters.AddWithValue("@vcPartsnameen", "");
                sqlCommand_sub.Parameters.AddWithValue("@vcShippingqty", "");
                sqlCommand_sub.Parameters.AddWithValue("@vcCaseno", "");
                sqlCommand_sub.Parameters.AddWithValue("@vcCostwithtaxes", "");
                sqlCommand_sub.Parameters.AddWithValue("@vcPrice", "");
                sqlCommand_sub.Parameters.AddWithValue("@dPrintDate", "");
                sqlCommand_sub.Parameters.Add("@vcCodemage", SqlDbType.Image);
                #endregion
                foreach (DataRow item in dtSub.Rows)
                {
                    #region Value
                    sqlCommand_sub.Parameters["@UUID"].Value = item["UUID"].ToString();
                    sqlCommand_sub.Parameters["@vcCpdcompany"].Value = item["vcCpdcompany"];
                    sqlCommand_sub.Parameters["@vcCompany"].Value = item["vcCompany"];
                    sqlCommand_sub.Parameters["@vcPackingspot"].Value = item["vcPackingspot"];
                    sqlCommand_sub.Parameters["@vcControlno"].Value = item["vcControlno"];
                    sqlCommand_sub.Parameters["@vcPartsno"].Value = item["vcPartsno"];
                    sqlCommand_sub.Parameters["@vcOrderno"].Value = item["vcOrderno"];
                    sqlCommand_sub.Parameters["@vcSeqno"].Value = item["vcSeqno"];
                    sqlCommand_sub.Parameters["@vcInvoiceno"].Value = item["vcInvoiceno"];
                    sqlCommand_sub.Parameters["@vcPartsnamechn"].Value = item["vcPartsnamechn"];
                    sqlCommand_sub.Parameters["@vcPartsnameen"].Value = item["vcPartsnameen"];
                    sqlCommand_sub.Parameters["@vcShippingqty"].Value = item["vcShippingqty"];
                    sqlCommand_sub.Parameters["@vcCaseno"].Value = item["vcCaseno"];
                    sqlCommand_sub.Parameters["@vcCostwithtaxes"].Value = item["vcCostwithtaxes"];
                    sqlCommand_sub.Parameters["@vcPrice"].Value = item["vcPrice"];
                    sqlCommand_sub.Parameters["@dPrintDate"].Value = item["dPrintDate"];
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
        public DataTable getPrintInfo(string strOperId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("SELECT '' AS [LinId]");
                strSql.AppendLine("      ,[UUID]");
                strSql.AppendLine("      ,[vcOperator]");
                strSql.AppendLine("      ,[dOperatorTime]");
                strSql.AppendLine("      ,[vcCpdcompany]");
                strSql.AppendLine("      ,[vcCompany]");
                strSql.AppendLine("      ,[vcPackingspot]");
                strSql.AppendLine("      ,[vcControlno]");
                strSql.AppendLine("      ,[vcPartsno]");
                strSql.AppendLine("      ,[vcOrderno]");
                strSql.AppendLine("      ,[vcSeqno]");
                strSql.AppendLine("      ,[vcInvoiceno]");
                strSql.AppendLine("      ,[vcPartsnamechn]");
                strSql.AppendLine("      ,[vcPartsnameen]");
                strSql.AppendLine("      ,[vcShippingqty]");
                strSql.AppendLine("      ,[vcCaseno]");
                strSql.AppendLine("      ,[vcCostwithtaxes]");
                strSql.AppendLine("      ,[vcPrice]");
                strSql.AppendLine("      ,[dPrintDate]");
                strSql.AppendLine("      ,[vcCodemage]");
                strSql.AppendLine("  FROM [tPrintTemp_FS0805] WHERE [vcOperator]='"+strOperId+"'");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
