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
    public class FS1102_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        public DataTable getSearchInfo(string strReceiver, string strCaseNo, string strTagId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("SELECT distinct '' as LinId,T1.vcCpdcode as vcReceiver,SUBSTRING(T1.vcCaseno,1,5)+'-'+SUBSTRING(T1.vcCaseno,6,5) as vcCaseNo");
                //strSql.AppendLine("SELECT T1.iAutoId as LinId,T1.vcCpdcode as vcReceiver,SUBSTRING(T1.vcCaseno,1,5)+'-'+SUBSTRING(T1.vcCaseno,6,5) as vcCaseNo");
                strSql.AppendLine(",'1' as bSelectFlag FROM");
                //strSql.AppendLine(",T1.vcInno as vcInPutOrderNo,T1.vcPart_id as vcPartId,t2.vcPartsNameEN as vcPartENName,t1.iQty as iQty,'1' as bSelectFlag FROM");
                strSql.AppendLine("(select * from TCaseList where dFirstPrintTime is not null");
                if (strReceiver != "")
                {
                    strSql.AppendLine("AND vcCpdcode='" + strReceiver + "'");
                }
                if (strCaseNo != "")
                {
                    strSql.AppendLine("AND cast(vcCaseNo as int)='" + strCaseNo + "'");
                }
                if (strTagId != "")
                {
                    strSql.AppendLine("AND vcLabelStart<='" + strTagId + "' AND vcLabelEnd>='" + strTagId + "'");
                }
                strSql.AppendLine(")T1");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT * FROM [TPartInfoMaster] WHERE dTimeFrom<=GETDATE() AND dTimeTo>=GETDATE())T2");
                strSql.AppendLine("ON T1.vcPart_id=T2.vcPartsNo");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getSearchInfo(string strReceiver, string strCaseNo)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select vcCpdcode,vcCpdname,vcCpdaddress,vcCaseno,SUBSTRING(vcCaseno,1,4)+'-'+SUBSTRING(vcCaseno,5,4) as vcCaseno_name,vcInno,vcPart_id,vcPartsname,iQty,CONVERT(varchar(23),GETDATE(),23) as dPrintDate");
                strSql.AppendLine("from TCaseList");
                strSql.AppendLine("where vcCpdcode='" + strReceiver + "' and vcCaseno='" + strCaseNo.Replace("-", "") + "'");
                strSql.AppendLine("order by iAutoId");
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
                strSql_deleteinfo.AppendLine("DELETE from tPrintTemp_FS1102 where vcOperator='" + strOperId + "'");
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
                strSql_sub.AppendLine("INSERT INTO [dbo].[tPrintTemp_FS1102]");
                strSql_sub.AppendLine("           ([UUID]");
                strSql_sub.AppendLine("           ,[vcOperator]");
                strSql_sub.AppendLine("           ,[dOperatorTime]");
                strSql_sub.AppendLine("           ,[vcCpdcode]");
                strSql_sub.AppendLine("           ,[vcCpdname]");
                strSql_sub.AppendLine("           ,[vcCpdaddress]");
                strSql_sub.AppendLine("           ,[vcCaseno]");
                strSql_sub.AppendLine("           ,[vcCaseno_name]");
                strSql_sub.AppendLine("           ,[vcInno]");
                strSql_sub.AppendLine("           ,[vcPart_id]");
                strSql_sub.AppendLine("           ,[vcPartsname]");
                strSql_sub.AppendLine("           ,[iQty]");
                strSql_sub.AppendLine("           ,[dPrintDate]");
                strSql_sub.AppendLine("           ,[vcCodemage])");
                strSql_sub.AppendLine("     VALUES");
                strSql_sub.AppendLine("           (@UUID");
                strSql_sub.AppendLine("           ,'" + strOperId + "'");
                strSql_sub.AppendLine("           ,GETDATE()");
                strSql_sub.AppendLine("           ,@vcCpdcode");
                strSql_sub.AppendLine("           ,@vcCpdname");
                strSql_sub.AppendLine("           ,@vcCpdaddress");
                strSql_sub.AppendLine("           ,@vcCaseno");
                strSql_sub.AppendLine("           ,@vcCaseno_name");
                strSql_sub.AppendLine("           ,@vcInno");
                strSql_sub.AppendLine("           ,@vcPart_id");
                strSql_sub.AppendLine("           ,@vcPartsname");
                strSql_sub.AppendLine("           ,@iQty");
                strSql_sub.AppendLine("           ,@dPrintDate");
                strSql_sub.AppendLine("           ,@vcCodemage)");
                sqlCommand_sub.CommandText = strSql_sub.ToString();
                sqlCommand_sub.Parameters.AddWithValue("@UUID", "");
                sqlCommand_sub.Parameters.AddWithValue("@vcCpdcode", "");
                sqlCommand_sub.Parameters.AddWithValue("@vcCpdname", "");
                sqlCommand_sub.Parameters.AddWithValue("@vcCpdaddress", "");
                sqlCommand_sub.Parameters.AddWithValue("@vcCaseno", "");
                sqlCommand_sub.Parameters.AddWithValue("@vcCaseno_name", "");
                sqlCommand_sub.Parameters.AddWithValue("@vcInno", "");
                sqlCommand_sub.Parameters.AddWithValue("@vcPart_id", "");
                sqlCommand_sub.Parameters.AddWithValue("@vcPartsname", "");
                sqlCommand_sub.Parameters.AddWithValue("@iQty", "");
                sqlCommand_sub.Parameters.AddWithValue("@dPrintDate", "");
                sqlCommand_sub.Parameters.Add("@vcCodemage", SqlDbType.Image);
                #endregion
                foreach (DataRow item in dtSub.Rows)
                {
                    #region Value
                    sqlCommand_sub.Parameters["@UUID"].Value = item["UUID"].ToString();
                    sqlCommand_sub.Parameters["@vcCpdcode"].Value = item["vcCpdcode"];
                    sqlCommand_sub.Parameters["@vcCpdname"].Value = item["vcCpdname"];
                    sqlCommand_sub.Parameters["@vcCpdaddress"].Value = item["vcCpdaddress"];
                    sqlCommand_sub.Parameters["@vcCaseno"].Value = item["vcCaseno"];
                    sqlCommand_sub.Parameters["@vcCaseno_name"].Value = item["vcCaseno_name"];
                    sqlCommand_sub.Parameters["@vcInno"].Value = item["vcInno"];
                    sqlCommand_sub.Parameters["@vcPart_id"].Value = item["vcPart_id"];
                    sqlCommand_sub.Parameters["@vcPartsname"].Value = item["vcPartsname"];
                    sqlCommand_sub.Parameters["@iQty"].Value = item["iQty"];
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
    }
}
