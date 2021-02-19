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
        public DataTable getSearchInfo(string strCheckQf, string strPartId, string strSuplierId, string strSuplierPlant)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select a.[LinId],a.vcPartId as vcPartId,a.vcTimeFrom as vcTimeFrom,a.vcTimeTo as vcTimeTo");
                strSql.AppendLine(",a.vcSupplierCode as vcSupplierCode,a.vcSupplierPlant as vcSupplierPlant,a.vcCarfamilyCode as vcCarfamilyCode");
                strSql.AppendLine(",a.vcCheckP as vcCheckP,a.vcChangeRea as vcChangeRea,a.vcTJSX as vcTJSX,b.vcUserName as vcOperator,a.dOperatorTime as vcOperatorTime");
                strSql.AppendLine("from (");
                strSql.AppendLine("select [LinId] AS [LinId],vcPartId,vcTimeFrom,vcTimeTo,vcSupplierCode,vcSupplierPlant,vcCarfamilyCode,vcCheckP");
                strSql.AppendLine(",vcChangeRea,vcTJSX,vcOperatorID,dOperatorTime from [tCheckQf]) a");
                strSql.AppendLine("left join ");
                strSql.AppendLine("(select * from sUser) b");
                strSql.AppendLine("on b.vcUserId=a.vcOperatorID");
                strSql.AppendLine("where 1=1 ");
                if (strCheckQf != "")
                {
                    strSql.AppendLine(" and a.vcCheckP='" + strCheckQf + "'");
                }
                if (strPartId != "")
                {
                    strSql.AppendLine(" and a.vcPartId like '%" + strPartId + "%'");
                }
                if (strSuplierId != "")
                {
                    strSql.AppendLine(" and a.vcSupplierCode='" + strSuplierId + "'");
                }
                if (strSuplierPlant != "")
                {
                    strSql.AppendLine(" and a.vcSupplierPlant='" + strSuplierPlant + "'");
                }
                strSql.AppendLine("order by a.vcPartId ,a.vcTimeFrom");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getCheckInfo(string strPartId, string strSuplierId, string strSuplierPlant)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select max(vcTimeFrom) as vcTimeFrom from [tCheckQf] ");
                strSql.AppendLine("where vcPartId='" + strPartId.Replace("-", "").Trim().ToString() + "'");
                strSql.AppendLine("and vcSupplierCode='" + strSuplierId + "' ");
                strSql.AppendLine("and vcSupplierPlant='" + strSuplierPlant + "'");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setDataInfo(DataTable dataTable, string strOper)
        {
            SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();

            sqlConnection.Open();
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            try
            {
                #region 写入数据库
                //启动事务
                SqlCommand sqlCommand = sqlConnection.CreateCommand();//主表
                sqlCommand.Transaction = sqlTransaction;
                sqlCommand.CommandType = CommandType.Text;
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("if(@vcType='update')");
                stringBuilder.AppendLine("begin");
                stringBuilder.AppendLine("UPDATE [dbo].[tCheckQf]");
                stringBuilder.AppendLine("   SET [vcTimeTo] = @vcTimeTo");
                stringBuilder.AppendLine("      ,[vcOperatorID] = @vcOperatorID");
                stringBuilder.AppendLine("      ,[dOperatorTime] = GETDATE()");
                stringBuilder.AppendLine(" WHERE  [vcPartId] = @vcPartId");
                stringBuilder.AppendLine(" AND [vcTimeFrom] = @vcTimeFrom");
                stringBuilder.AppendLine(" AND [vcSupplierCode] = @vcSupplierCode");
                stringBuilder.AppendLine(" AND [vcSupplierPlant] = @vcSupplierPlant");
                stringBuilder.AppendLine("end");
                stringBuilder.AppendLine("else");
                stringBuilder.AppendLine("begin");
                stringBuilder.AppendLine("INSERT INTO [dbo].[tCheckQf]");
                stringBuilder.AppendLine("           ([vcPartId],[vcTimeFrom],[vcTimeTo],[vcCarfamilyCode],[vcSupplierCode],[vcSupplierPlant],");
                stringBuilder.AppendLine("		   [vcCheckP],[vcChangeRea],[vcTJSX],[vcOperatorID],[dOperatorTime])");
                stringBuilder.AppendLine("     VALUES");
                stringBuilder.AppendLine("           (@vcPartId,@vcTimeFrom,@vcTimeTo,@vcCarfamilyCode,@vcSupplierCode,@vcSupplierPlant,");
                stringBuilder.AppendLine("		   @vcCheckP,@vcChangeRea,@vcTJSX,@vcOperatorID,GETDATE())");
                stringBuilder.AppendLine("end");
                sqlCommand.CommandText = stringBuilder.ToString();

                sqlCommand.Parameters.AddWithValue("@vcType", "");
                sqlCommand.Parameters.AddWithValue("@vcPartId", "");
                sqlCommand.Parameters.AddWithValue("@vcTimeFrom", "");
                sqlCommand.Parameters.AddWithValue("@vcTimeTo", "");
                sqlCommand.Parameters.AddWithValue("@vcCarfamilyCode", "");
                sqlCommand.Parameters.AddWithValue("@vcSupplierCode", "");
                sqlCommand.Parameters.AddWithValue("@vcSupplierPlant", "");
                sqlCommand.Parameters.AddWithValue("@vcCheckP", "");
                sqlCommand.Parameters.AddWithValue("@vcChangeRea", "");
                sqlCommand.Parameters.AddWithValue("@vcTJSX", "");
                sqlCommand.Parameters.AddWithValue("@vcOperatorID", "");
                foreach (DataRow item in dataTable.Rows)
                {
                    sqlCommand.Parameters["@vcType"].Value = item["vcType"].ToString();
                    sqlCommand.Parameters["@vcPartId"].Value = item["vcPartId"].ToString();
                    sqlCommand.Parameters["@vcTimeFrom"].Value = item["vcTimeFrom"].ToString();
                    sqlCommand.Parameters["@vcTimeTo"].Value = item["vcTimeTo"].ToString();
                    sqlCommand.Parameters["@vcCarfamilyCode"].Value = item["vcCarfamilyCode"].ToString();
                    sqlCommand.Parameters["@vcSupplierCode"].Value = item["vcSupplierCode"].ToString();
                    sqlCommand.Parameters["@vcSupplierPlant"].Value = item["vcSupplierPlant"].ToString();
                    sqlCommand.Parameters["@vcCheckP"].Value = item["vcCheckP"].ToString();
                    sqlCommand.Parameters["@vcChangeRea"].Value = item["vcChangeRea"].ToString();
                    sqlCommand.Parameters["@vcTJSX"].Value = item["vcTJSX"].ToString();
                    sqlCommand.Parameters["@vcOperatorID"].Value = strOper;
                    sqlCommand.ExecuteNonQuery();
                }
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
            }
        }
        public DataTable getSubInfo(string strLinid)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select linid,[vcPartId],Convert(varchar(10),[vcTimeFrom],23) as [vcTimeFrom] ,Convert(varchar(10),[vcTimeTo],23) as [vcTimeTo]");
                strSql.AppendLine(",[vcCarfamilyCode],[vcSupplierCode],[vcSupplierPlant]");
                strSql.AppendLine(",[vcCheckP],[vcChangeRea],[vcTJSX],[vcOperatorID],[dOperatorTime] from [tCheckQf] where linid='"+ strLinid + "'");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
