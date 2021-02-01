using System;
using Common;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using DataEntity;
using System.Collections.Generic;

namespace DataAccess
{
    public class FS0616_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        public DataTable getOrderNoList()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select distinct vcOrderNo as vcValue,vcOrderNo as vcName from [TUrgentOrder] where vcDelete='0' and vcStatus not in ('3')");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getSearchInfo(string strDelete, string strState, string strOrderNoList, string strPartId, string strOrderPlant, string strInOut, string strHaoJiu, string strSupplierId, string strSupplierPlant)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("SELECT T2.iAutoId as LinId,T7.vcName as vcState_name,T1.vcState,T1.vcOrderNo,T1.vcPart_id,T5.vcName as vcOrderPlant,");
                strSql.AppendLine("t3.vcName as vcInOut,T4.vcName as vcHaoJiu,T6.vcName as vcOESP,T1.vcSupplierId,T1.vcSupplierPlant,");
                strSql.AppendLine("t1.vcSupplierPlace,T1.vcSufferIn,t1.iPackingQty,t1.iOrderQuantity,");
                strSql.AppendLine("t2.iDuiYingQuantity,t2.dDeliveryDate,t2.dOutPutDate,t1.dReplyOverDate,");
                strSql.AppendLine("'0' as bModFlag,'0' as bAddFlag,'' as vcBgColor,");
                strSql.AppendLine("CASE WHEN T1.vcDelete='1' THEN '0' ELSE (CASE WHEN T1.vcState='3' THEN '0' ELSE '1' END)  END bSelectFlag FROM ");
                strSql.AppendLine("(SELECT [iAutoId] as LinId,[vcStatus] as vcState,[vcOrderNo] as vcOrderNo,[vcPart_id] as vcPart_id,[vcOrderPlant] as vcOrderPlant,");
                strSql.AppendLine("	   [vcInOut] as vcInOut,[vcHaoJiu] as vcHaoJiu,[vcOESP] as vcOESP,[vcSupplier_id] as vcSupplierId,[vcGQ] as vcSupplierPlant,");
                strSql.AppendLine("	   [vcChuHePlant] as vcSupplierPlace,[vcSufferIn] as vcSufferIn,[iPackingQty] as iPackingQty,[iOrderQuantity] as iOrderQuantity,");
                strSql.AppendLine("	   [dReplyOverDate] as dReplyOverDate,isnull([vcDelete],'0') as [vcDelete]");
                strSql.AppendLine("  FROM [TUrgentOrder]");
                strSql.AppendLine("  WHERE isnull([vcDelete],'0')='" + strDelete + "'");
                if (strState != "")
                {
                    strSql.AppendLine("AND [vcStatus]='" + strState + "'");
                }
                if (strOrderNoList != "")
                {
                    strSql.AppendLine("AND [vcOrderNo] IN (" + strOrderNoList + ")");
                }
                if (strPartId != "")
                {
                    strSql.AppendLine("AND [vcPart_id] LIKE '" + strPartId + "%'");
                }
                if (strOrderPlant != "")
                {
                    strSql.AppendLine("AND [vcOrderPlant]='" + strOrderPlant + "'");
                }
                if (strInOut != "")
                {
                    strSql.AppendLine("AND [vcInOut]='"+ strInOut + "'");
                }
                if (strHaoJiu != "")
                {
                    strSql.AppendLine("AND [vcHaoJiu]='"+ strHaoJiu + "'");
                }
                if (strSupplierId != "")
                {
                    strSql.AppendLine("AND [vcSupplier_id]='"+ strSupplierId + "'");
                }
                if (strSupplierPlant != "")
                {
                    strSql.AppendLine("AND [vcGQ]='"+ strSupplierPlant + "'");
                }
                strSql.AppendLine("  )T1");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(select * from VI_UrgentOrder_OperHistory)T2");
                strSql.AppendLine("ON T1.vcOrderNo=T2.vcOrderNo AND T1.vcPart_id=T2.vcPart_id AND T1.vcSupplierId=T2.vcSupplier_id");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C003')T3--内外区分");
                strSql.AppendLine("ON T1.vcInOut=T3.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C004')T4--号旧区分");
                strSql.AppendLine("ON T1.vcHaoJiu=T4.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C000')T5--发注工厂");
                strSql.AppendLine("ON T1.vcOrderPlant=T5.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C012')T6--OE=SP");
                strSql.AppendLine("ON T1.vcOESP=T6.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C056')T7--状态");
                strSql.AppendLine("ON T1.vcState=T7.vcValue");
                strSql.AppendLine("ORDER BY T1.vcOrderNo,t1.vcPart_id,t1.vcState");
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
                SqlCommand sqlCommand_modinfo = sqlConnection.CreateCommand();
                sqlCommand_modinfo.Transaction = sqlTransaction;
                sqlCommand_modinfo.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo = new StringBuilder();
                strSql_modinfo.AppendLine("");
                sqlCommand_modinfo.CommandText = strSql_modinfo.ToString();
                sqlCommand_modinfo.Parameters.AddWithValue("@vcDyState", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@dExpectTime", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcYearMonth", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcPart_id", "");
                foreach (DataRow item in dtImport.Rows)
                {
                    sqlCommand_modinfo.Parameters["@vcDyState"].Value = item["vcDyState"].ToString();
                    sqlCommand_modinfo.Parameters["@dExpectTime"].Value = item["dExpectTime"].ToString();
                    sqlCommand_modinfo.Parameters["@vcYearMonth"].Value = item["vcYearMonth"].ToString();
                    sqlCommand_modinfo.Parameters["@vcPart_id"].Value = item["vcPart_id"].ToString();
                    sqlCommand_modinfo.ExecuteNonQuery();
                }
                //提交事务
                sqlTransaction.Commit();
                sqlConnection.Close();

            }
            catch (Exception ex)
            {
                //回滚事务
                if (sqlTransaction != null && sqlConnection != null)
                {
                    sqlTransaction.Rollback();
                    sqlConnection.Close();
                }
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
                strSql_modinfo.AppendLine("");
                sqlCommand_modinfo.CommandText = strSql_modinfo.ToString();
                sqlCommand_modinfo.Parameters.AddWithValue("@vcDyState", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@dExpectTime", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcYearMonth", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcPart_id", "");
                foreach (DataRow item in dtImport.Rows)
                {
                    sqlCommand_modinfo.Parameters["@vcDyState"].Value = item["vcDyState"].ToString();
                    sqlCommand_modinfo.Parameters["@dExpectTime"].Value = item["dExpectTime"].ToString();
                    sqlCommand_modinfo.Parameters["@vcYearMonth"].Value = item["vcYearMonth"].ToString();
                    sqlCommand_modinfo.Parameters["@vcPart_id"].Value = item["vcPart_id"].ToString();
                    sqlCommand_modinfo.ExecuteNonQuery();
                }
                //提交事务
                sqlTransaction.Commit();
                sqlConnection.Close();

            }
            catch (Exception ex)
            {
                //回滚事务
                if (sqlTransaction != null && sqlConnection != null)
                {
                    sqlTransaction.Rollback();
                    sqlConnection.Close();
                }
            }
        }

        public void setOpenInfo(DataTable dtImport, string strOperId, ref DataTable dtMessage)
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
                strSql_modinfo.AppendLine("");
                sqlCommand_modinfo.CommandText = strSql_modinfo.ToString();
                sqlCommand_modinfo.Parameters.AddWithValue("@vcDyState", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@dExpectTime", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcYearMonth", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcPart_id", "");
                foreach (DataRow item in dtImport.Rows)
                {
                    sqlCommand_modinfo.Parameters["@vcDyState"].Value = item["vcDyState"].ToString();
                    sqlCommand_modinfo.Parameters["@dExpectTime"].Value = item["dExpectTime"].ToString();
                    sqlCommand_modinfo.Parameters["@vcYearMonth"].Value = item["vcYearMonth"].ToString();
                    sqlCommand_modinfo.Parameters["@vcPart_id"].Value = item["vcPart_id"].ToString();
                    sqlCommand_modinfo.ExecuteNonQuery();
                }
                //提交事务
                sqlTransaction.Commit();
                sqlConnection.Close();

            }
            catch (Exception ex)
            {
                //回滚事务
                if (sqlTransaction != null && sqlConnection != null)
                {
                    sqlTransaction.Rollback();
                    sqlConnection.Close();
                }
            }
        }

        public void setOutputInfo(DataTable dtImport, string strOperId, ref DataTable dtMessage)
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
                strSql_modinfo.AppendLine("");
                sqlCommand_modinfo.CommandText = strSql_modinfo.ToString();
                sqlCommand_modinfo.Parameters.AddWithValue("@vcDyState", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@dExpectTime", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcYearMonth", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcPart_id", "");
                foreach (DataRow item in dtImport.Rows)
                {
                    sqlCommand_modinfo.Parameters["@vcDyState"].Value = item["vcDyState"].ToString();
                    sqlCommand_modinfo.Parameters["@dExpectTime"].Value = item["dExpectTime"].ToString();
                    sqlCommand_modinfo.Parameters["@vcYearMonth"].Value = item["vcYearMonth"].ToString();
                    sqlCommand_modinfo.Parameters["@vcPart_id"].Value = item["vcPart_id"].ToString();
                    sqlCommand_modinfo.ExecuteNonQuery();
                }
                //提交事务
                sqlTransaction.Commit();
                sqlConnection.Close();

            }
            catch (Exception ex)
            {
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