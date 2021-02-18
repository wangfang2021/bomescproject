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
    public class FS0811_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        public DataTable getNowBZInfo(string strPackPlant)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select * from (");
                strSql.AppendLine("select convert(varchar(10),dateadd(DAY,-1,GETDATE()),23) as dHosDate,vcBanZhi,convert(varchar(10),dateadd(DAY,-1,GETDATE()),23)+' '+convert(varchar(10),tFromTime,24) as tFromTime,case when vcCross='1' then convert(varchar(10),dateadd(day,1,dateadd(DAY,-1,GETDATE())),23) else convert(varchar(10),dateadd(DAY,-1,GETDATE()),23) end +' '+convert(varchar(10),tToTime,24) as tToTime from TBZTime where vcBanZhi='夜' and vcPackPlant='" + strPackPlant + "'");
                strSql.AppendLine("union");
                strSql.AppendLine("select convert(varchar(10),dateadd(DAY,0,GETDATE()),23) as dHosDate,vcBanZhi,convert(varchar(10),dateadd(DAY,0,GETDATE()),23)+' '+convert(varchar(10),tFromTime,24) as tFromTime,case when vcCross='1' then convert(varchar(10),dateadd(day,1,dateadd(DAY,0,GETDATE())),23) else convert(varchar(10),dateadd(DAY,0,GETDATE()),23) end +' '+convert(varchar(10),tToTime,24) as tToTime from TBZTime where vcBanZhi='白' and vcPackPlant='" + strPackPlant + "'");
                strSql.AppendLine("union																																																											    ");
                strSql.AppendLine("select convert(varchar(10),dateadd(DAY,0,GETDATE()),23) as dHosDate,vcBanZhi,convert(varchar(10),dateadd(DAY,0,GETDATE()),23)+' '+convert(varchar(10),tFromTime,24) as tFromTime,case when vcCross='1' then convert(varchar(10),dateadd(day,1,dateadd(DAY,0,GETDATE())),23) else convert(varchar(10),dateadd(DAY,0,GETDATE()),23) end +' '+convert(varchar(10),tToTime,24) as tToTime from TBZTime where vcBanZhi='夜' and vcPackPlant='" + strPackPlant + "'");
                strSql.AppendLine("union");
                strSql.AppendLine("select convert(varchar(10),dateadd(DAY,1,GETDATE()),23) as dHosDate,vcBanZhi,convert(varchar(10),dateadd(DAY,1,GETDATE()),23)+' '+convert(varchar(10),tFromTime,24) as tFromTime,case when vcCross='1' then convert(varchar(10),dateadd(day,1,dateadd(DAY,1,GETDATE())),23) else convert(varchar(10),dateadd(DAY,1,GETDATE()),23) end +' '+convert(varchar(10),tToTime,24) as tToTime from TBZTime where vcBanZhi='白' and vcPackPlant='" + strPackPlant + "'");
                strSql.AppendLine(")t where tFromTime<=GETDATE() and tToTime>=GETDATE()");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataSet getPackingPlanInfo(string strPackPlant, string strHosDate, string strBanZhi)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("SELECT distinct '' as uuid,'" + strPackPlant + "'vcPackingPlant,Convert(varchar(10),'" + strHosDate + "',23) as dHosDate,'" + strBanZhi + "' as vcBanZhi,'' as vcPeopleNum,'450' as vcCycleTime,'90' as vcObjective,'' as vcWorkOverTime");
                strSql.AppendLine("select ");
                strSql.AppendLine("'' as uuid,'" + strPackPlant + "'vcPackingPlant,Convert(varchar(10),'" + strHosDate + "',23) as dHosDate,'" + strBanZhi + "' as vcBanZhi ");
                strSql.AppendLine(",'' as LinId");
                strSql.AppendLine(",vcPartItem as vcPartItem");
                strSql.AppendLine(",vcStandard as vcStandard");
                strSql.AppendLine(",vcPackTotalNum as decPackTotalNum");
                strSql.AppendLine(",'' as decPlannedTime ");
                strSql.AppendLine(",'' as decPlannedPerson");
                strSql.AppendLine(",'' as decInputPerson");
                strSql.AppendLine(",'' as decInputTime");
                strSql.AppendLine(",'' as decOverFlowTime");
                strSql.AppendLine(",'' as decSysLander");
                strSql.AppendLine(",'' as decDiffer,'1' as bSelectFlag");
                strSql.AppendLine("from (");
                strSql.AppendLine("SELECT isnull(t3.vcBigPM,'') as vcPartItem");
                strSql.AppendLine(",cast(isnull(t4.vcStandardTime,-1) as int) as vcStandard");
                strSql.AppendLine(",sum(cast(T1.iPackNum as int)) as vcPackTotalNum FROM ");
                strSql.AppendLine("(select * from [TPackingPlan] ");
                strSql.AppendLine("WHERE vcPlant='" + strPackPlant + "'");
                strSql.AppendLine("AND vcPackDate='" + strHosDate + "' ");
                strSql.AppendLine("AND vcPackBZ='" + strBanZhi + "'");
                strSql.AppendLine(")T1");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select * from [TPMSmall])T2");
                strSql.AppendLine("on substring(t1.vcPartId,1,5)=t2.[vcPartsNoBefore5]");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select * from [TPMRelation])T3");
                strSql.AppendLine("on t2.vcSmallPM=t3.vcSmallPM");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select * from TPMStandardTime)T4");
                strSql.AppendLine("on t3.vcBigPM=t4.vcBigPM");
                strSql.AppendLine("group by t3.vcBigPM,t4.vcStandardTime");
                strSql.AppendLine(")TT order by vcPartItem");
                return excute.ExcuteSqlWithSelectToDS(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataSet getSearchInfo(string strPackPlant, string strHosDate, string strBanZhi)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("SELECT distinct uuid,vcPackingPlant,Convert(varchar(10),dHosDate,23) as dHosDate,vcBanZhi,vcPeopleNum,vcCycleTime,vcObjective,vcWorkOverTime");
                strSql.AppendLine("  FROM [dbo].[TInPutIntoOver]  where vcPackingPlant='" + strPackPlant + "' and dHosDate='" + strHosDate + "' and vcBanZhi='" + strBanZhi + "';");
                strSql.AppendLine("SELECT [LinId],[uuid],vcPackingPlant,Convert(varchar(10),dHosDate,23) as dHosDate,vcBanZhi,[vcPartItem] as vcPartItem,[vcStandard] as vcStandard,[decPackTotalNum]");
                strSql.AppendLine("      ,[decPlannedTime],[decPlannedPerson],[decInputPerson],[decInputTime],[decOverFlowTime]");
                strSql.AppendLine("      ,'' as decSysLander,'' as decDiffer,'1' as bSelectFlag");
                strSql.AppendLine("  FROM [dbo].[TInPutIntoOver] where vcPackingPlant='" + strPackPlant + "' and dHosDate='" + strHosDate + "' and vcBanZhi='" + strBanZhi + "' order by vcPartItem;");
                return excute.ExcuteSqlWithSelectToDS(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void setInPutIntoOverInfo(DataTable dtRowinfo, 
            string uuid,string vcPeopleNum,string vcCycleTime,string vcObjective,string vcWorkOverTime, 
            string strOperId, ref DataTable dtMessage)
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
                string newid = System.Guid.NewGuid().ToString();
                strSql_modinfo.AppendLine("DELETE FROM [TInPutIntoOver] WHERE [uuid]='" + uuid + "'");
                strSql_modinfo.AppendLine("INSERT INTO [dbo].[TInPutIntoOver]");
                strSql_modinfo.AppendLine("           ([uuid],[vcPeopleNum],[vcCycleTime],[vcObjective],[vcWorkOverTime]");
                strSql_modinfo.AppendLine("		   ,[vcPackingPlant],[dHosDate],[vcBanZhi]");
                strSql_modinfo.AppendLine("           ,[vcPartItem],[vcStandard],[decPackTotalNum],[decPlannedTime],[decPlannedPerson],[decInputPerson],[decInputTime],[decOverFlowTime]");
                strSql_modinfo.AppendLine("           ,[vcOperatorID],[dOperatorTime])");
                strSql_modinfo.AppendLine("     VALUES");
                strSql_modinfo.AppendLine("           ('" + newid + "',@vcPeopleNum,@vcCycleTime,@vcObjective,@vcWorkOverTime");
                strSql_modinfo.AppendLine("           ,@vcPackingPlant,@dHosDate,@vcBanZhi");
                strSql_modinfo.AppendLine("           ,@vcPartItem,@vcStandard,@decPackTotalNum,@decPlannedTime,@decPlannedPerson,@decInputPerson,@decInputTime,@decOverFlowTime");
                strSql_modinfo.AppendLine("           ,'" + strOperId + "',GETDATE())");
                sqlCommand_modinfo.CommandText = strSql_modinfo.ToString();
                sqlCommand_modinfo.Parameters.AddWithValue("@vcPeopleNum", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcCycleTime", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcObjective", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcWorkOverTime", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcPackingPlant", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@dHosDate", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcBanZhi", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcPartItem", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcStandard", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@decPackTotalNum", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@decPlannedTime", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@decPlannedPerson", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@decInputPerson", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@decInputTime", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@decOverFlowTime", "");
                foreach (DataRow item in dtRowinfo.Rows)
                {
                    sqlCommand_modinfo.Parameters["@vcPeopleNum"].Value = vcPeopleNum;
                    sqlCommand_modinfo.Parameters["@vcCycleTime"].Value = vcCycleTime;
                    sqlCommand_modinfo.Parameters["@vcObjective"].Value = vcObjective;
                    sqlCommand_modinfo.Parameters["@vcWorkOverTime"].Value = vcWorkOverTime;
                    sqlCommand_modinfo.Parameters["@vcPackingPlant"].Value = item["vcPackingPlant"].ToString();
                    sqlCommand_modinfo.Parameters["@dHosDate"].Value = item["dHosDate"].ToString();
                    sqlCommand_modinfo.Parameters["@vcBanZhi"].Value = item["vcBanZhi"].ToString();
                    sqlCommand_modinfo.Parameters["@vcPartItem"].Value = item["vcPartItem"].ToString();
                    sqlCommand_modinfo.Parameters["@vcStandard"].Value = item["vcStandard"].ToString();
                    sqlCommand_modinfo.Parameters["@decPackTotalNum"].Value = item["decPackTotalNum"].ToString();
                    sqlCommand_modinfo.Parameters["@decPlannedTime"].Value = item["decPlannedTime"].ToString();
                    sqlCommand_modinfo.Parameters["@decPlannedPerson"].Value = item["decPlannedPerson"].ToString();
                    sqlCommand_modinfo.Parameters["@decInputPerson"].Value = item["decInputPerson"].ToString();
                    sqlCommand_modinfo.Parameters["@decInputTime"].Value = item["decInputTime"].ToString();
                    sqlCommand_modinfo.Parameters["@decOverFlowTime"].Value = item["decOverFlowTime"].ToString();
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
