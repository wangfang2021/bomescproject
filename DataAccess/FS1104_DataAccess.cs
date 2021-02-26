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
    public class FS1104_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        public string getCaseNoInfo(string strOrderPlant, string strReceiver, string strPackingPlant, string strLianFan)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("declare @LianFanNo int ");
                strSql.AppendLine("set @LianFanNo=(select vcLianFanNo from TCaseNoInfo where vcPlant='" + strOrderPlant + "' and vcReParty='" + strReceiver + "' and vcPackPlant='" + strPackingPlant + "')");
                strSql.AppendLine("if((select count(*) from TCaseNoInfo where vcPlant='" + strOrderPlant + "' and vcReParty='" + strReceiver + "' and vcPackPlant='" + strPackingPlant + "')=0)");
                strSql.AppendLine("begin");
                strSql.AppendLine("select '00000001' as vcLianFanNo");
                strSql.AppendLine("end");
                strSql.AppendLine("else");
                strSql.AppendLine("begin");
                strSql.AppendLine("select substring(cast(100000001+@LianFanNo as varchar(8)),2,8)  as vcLianFanNo");
                strSql.AppendLine("end");
                DataTable data = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                return data.Rows[0]["vcLianFanNo"].ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getCaseNoInfo(string strCaseNo)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select a.vcPlant,b.vcName as vcPlantName,a.vcReParty,c.vcName as vcRePartyName");
                strSql.AppendLine(",a.vcPackPlant,c.vcName as vcPackPlantName,'' as vcPrintNum,vcLianFanNo as vcPrintIndex from");
                strSql.AppendLine("(select  * from TCaseNoInfo)a");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select * from TCode where vcCodeId='C000')b");
                strSql.AppendLine("on a.vcPlant=b.vcValue");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select vcValue,vcName from TCustomerInfo where vcDisting='C' and vcDisable='0')c");
                strSql.AppendLine("on a.vcReParty=c.vcValue");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select * from TCode where vcCodeId='C023')d");
                strSql.AppendLine("on a.vcPackPlant=d.vcValue");
                strSql.AppendLine("where vcCaseNo='" + strCaseNo + "'");
                DataTable data = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setSaveInfo(DataTable dtImport, ref DataTable dtMessage)
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
                strSql_modinfo.AppendLine("INSERT INTO [dbo].[TCaseNoInfo]");
                strSql_modinfo.AppendLine("           ([vcPlant]");
                strSql_modinfo.AppendLine("           ,[vcReParty]");
                strSql_modinfo.AppendLine("           ,[vcPackPlant]");
                strSql_modinfo.AppendLine("           ,[vcCaseNo]");
                strSql_modinfo.AppendLine("           ,[vcLianFanNo]");
                strSql_modinfo.AppendLine("           ,[dFirstPrintTime]");
                strSql_modinfo.AppendLine("           ,[dLatelyPrintTime])");
                strSql_modinfo.AppendLine("     VALUES");
                strSql_modinfo.AppendLine("           (@vcPlant");
                strSql_modinfo.AppendLine("           ,@vcReParty");
                strSql_modinfo.AppendLine("           ,@vcPackPlant");
                strSql_modinfo.AppendLine("           ,'2021-'+@vcCaseNo");
                strSql_modinfo.AppendLine("           ,cast(@vcLianFanNo as int)");
                strSql_modinfo.AppendLine("           ,GETDATE()");
                strSql_modinfo.AppendLine("           ,null)");
                sqlCommand_modinfo.CommandText = strSql_modinfo.ToString();
                sqlCommand_modinfo.Parameters.AddWithValue("@vcPlant", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcReParty", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcPackPlant", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcCaseNo", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcLianFanNo", "");
                foreach (DataRow item in dtImport.Rows)
                {
                    sqlCommand_modinfo.Parameters["@vcPlant"].Value = item["vcPlant"].ToString();
                    sqlCommand_modinfo.Parameters["@vcReParty"].Value = item["vcReParty"].ToString();
                    sqlCommand_modinfo.Parameters["@vcPackPlant"].Value = item["vcPackPlant"].ToString();
                    sqlCommand_modinfo.Parameters["@vcCaseNo"].Value = item["vcPrintIndex"].ToString();
                    sqlCommand_modinfo.Parameters["@vcLianFanNo"].Value = item["vcPrintIndex"].ToString();
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
