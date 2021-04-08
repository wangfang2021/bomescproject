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

        public DataTable getBanZhiTime(string strPackPlant,string strFlag)
        {
            try
            {
                SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();
                SqlParameter[] pars = new SqlParameter[]{
                    new SqlParameter("@flag", strFlag),
                    new SqlParameter("@PackPlant",strPackPlant)
                };
                string cmdText = "BSP0811_getBanZhiTime";
                SqlDataAdapter sa = new SqlDataAdapter(cmdText, sqlConnection);
                if (pars != null && pars.Length > 0)
                {
                    foreach (SqlParameter p in pars)
                    {
                        sa.SelectCommand.Parameters.Add(p);
                    }
                }
                sa.SelectCommand.CommandTimeout = 0;
                sa.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                sa.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
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

        //查询人员投入表数据
        public DataSet searchInPutIntoOver(string strPackPlant, string strHosDate, string strBanZhi)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select distinct vcPackingPlant");
                strSql.AppendLine("		,Convert(varchar(10),dHosDate,111) as dHosDate");
                strSql.AppendLine("		,vcBanZhi");
                strSql.AppendLine("		,cast(isnull(vcPeopleNum,0) as decimal(16,2)) as vcPeopleNum");
                strSql.AppendLine("		,cast(isnull(vcCycleTime,0) as decimal(16,2)) as vcCycleTime");
                strSql.AppendLine("		,cast(isnull(vcObjective,0) as decimal(16,2)) as vcObjective");
                strSql.AppendLine("		,cast(isnull(vcWorkOverTime,0) as decimal(16,2)) as vcWorkOverTime ");
                strSql.AppendLine("from TInPutIntoOver_small");
                strSql.AppendLine("where vcPackingPlant='" + strPackPlant + "' and Convert(varchar(10),dHosDate,23)='" + strHosDate + "' and vcBanZhi='" + strBanZhi + "'");
                strSql.AppendLine("select t2.* from ");
                strSql.AppendLine("(SELECT * FROM [TInPutIntoOver])t1");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select vcBigPM");
                strSql.AppendLine("		,sum(cast(isnull(decPackNum,0) as decimal(16,2))) as decPackNum,cast(isnull(decPlannedTime,0) as decimal(16,2)) as decPlannedTime");
                strSql.AppendLine("		,cast(isnull(decPlannedPerson,0) as decimal(16,2)) as decPlannedPerson,cast(isnull(decInputPerson,0) as decimal(16,2)) as decInputPerson");
                strSql.AppendLine("		,cast(isnull(decInputTime,0) as decimal(16,2)) as decInputTime,cast(isnull(decOverFlowTime,0) as decimal(16,2)) as decOverFlowTime ");
                strSql.AppendLine("		,'' as decSysLander,'' as decDiffer ");
                strSql.AppendLine("from TInPutIntoOver_small");
                strSql.AppendLine("where vcPackingPlant='" + strPackPlant + "' and Convert(varchar(10),dHosDate,23)='" + strHosDate + "' and vcBanZhi='" + strBanZhi + "'");
                strSql.AppendLine("group by vcBigPM,decPlannedTime,decPlannedPerson,decInputPerson,decInputTime,decOverFlowTime)t2");
                strSql.AppendLine(" on t1.vcPartItem=t2.vcBigPM");
                strSql.AppendLine(" where vcBigPM is not null");
                strSql.AppendLine(" order by t1.iOrderBy");
                return excute.ExcuteSqlWithSelectToDS(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataSet searchInPutIntoOver_temp(string strPackPlant, string strHosDate, string strBanZhi)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select distinct vcPackingPlant");
                strSql.AppendLine("		,Convert(varchar(10),dHosDate,111) as dHosDate");
                strSql.AppendLine("		,vcBanZhi");
                strSql.AppendLine("		,cast(isnull(vcPeopleNum,0) as decimal(16,2)) as vcPeopleNum");
                strSql.AppendLine("		,cast(isnull(vcCycleTime,0) as decimal(16,2)) as vcCycleTime");
                strSql.AppendLine("		,cast(isnull(vcObjective,0) as decimal(16,2)) as vcObjective");
                strSql.AppendLine("		,cast(isnull(vcWorkOverTime,0) as decimal(16,2)) as vcWorkOverTime ");
                strSql.AppendLine("from TInPutIntoOver_temp");
                strSql.AppendLine("where vcPackingPlant='" + strPackPlant + "' and Convert(varchar(10),dHosDate,23)='" + strHosDate + "' and vcBanZhi='" + strBanZhi + "'");
                strSql.AppendLine("select t2.* from ");
                strSql.AppendLine("(SELECT * FROM [TInPutIntoOver])t1");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select vcBigPM");
                strSql.AppendLine("		,sum(cast(isnull(decPackNum,0) as decimal(16,2))) as decPackNum,cast(isnull(decPlannedTime,0) as decimal(16,2)) as decPlannedTime");
                strSql.AppendLine("		,cast(isnull(decPlannedPerson,0) as decimal(16,2)) as decPlannedPerson,cast(isnull(decInputPerson,0) as decimal(16,2)) as decInputPerson");
                strSql.AppendLine("		,cast(isnull(decInputTime,0) as decimal(16,2)) as decInputTime,cast(isnull(decOverFlowTime,0) as decimal(16,2)) as decOverFlowTime ");
                strSql.AppendLine("		,'' as decSysLander,'' as decDiffer ");
                strSql.AppendLine("from TInPutIntoOver_temp");
                strSql.AppendLine("where vcPackingPlant='" + strPackPlant + "' and Convert(varchar(10),dHosDate,23)='" + strHosDate + "' and vcBanZhi='" + strBanZhi + "'");
                strSql.AppendLine("group by vcBigPM,decPlannedTime,decPlannedPerson,decInputPerson,decInputTime,decOverFlowTime)t2");
                strSql.AppendLine("on t1.vcPartItem=t2.vcBigPM");
                strSql.AppendLine("where vcBigPM is not null");
                strSql.AppendLine("order by t1.iOrderBy");
                return excute.ExcuteSqlWithSelectToDS(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //获取试行计划中的投入初始表
        public DataSet searchPackingPlan(string strPackPlant, string strHosDate, string strBanZhi)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("SELECT '" + strPackPlant + "' AS vcPackingPlant");
                strSql.AppendLine("		,Convert(varchar(10),'" + strHosDate + "',111) AS dHosDate");
                strSql.AppendLine("		,'" + strBanZhi + "' AS vcBanZhi");
                strSql.AppendLine("		,'' AS vcPeopleNum");
                strSql.AppendLine("		,CAST('450' as decimal(16,2)) AS vcCycleTime");
                strSql.AppendLine("		,CAST((select decObjective from TDisplaySettings where vcPackPlant='" + strPackPlant + "') as decimal(16,2)) AS vcObjective");
                strSql.AppendLine("		,'' AS vcWorkOverTime");
                strSql.AppendLine("select t2.* from ");
                strSql.AppendLine("(SELECT * FROM [TInPutIntoOver])t1");
                strSql.AppendLine("left join");
                strSql.AppendLine("(SELECT vcBigPM");
                strSql.AppendLine("		,sum(CAST(iPackNum as int)) as decPackNum");
                //strSql.AppendLine("		,sum(CAST(vcStandardTime as decimal(16,2))*CAST(iPackNum as int)) as decPackNum");
                strSql.AppendLine("		,cast(((sum(CAST(vcStandardTime as decimal(16,2))*CAST(iPackNum as int))/3600)+0.004) as decimal(16,2)) as decPlannedTime");
                strSql.AppendLine("		,cast((sum(CAST(vcStandardTime as decimal(16,2))*CAST(iPackNum as int))/3600)/cast((450/60.0) as decimal(16,2)) as decimal(16,2)) as decPlannedPerson");
                strSql.AppendLine("		,'' as decInputPerson");
                strSql.AppendLine("		,'' as decInputTime");
                strSql.AppendLine("		,'' as decOverFlowTime");
                strSql.AppendLine("		,'' as decSysLander,'' as decDiffer ");
                strSql.AppendLine("FROM ");
                strSql.AppendLine("(select vcPlant as vcPackingPlant");
                strSql.AppendLine("		,dPackDate as dHosDate");
                strSql.AppendLine("		,'" + strBanZhi + "' as vcBanZhi");
                strSql.AppendLine("		,isnull(vcBigPM,'') as vcBigPM");
                strSql.AppendLine("		,isnull(vcSmallPM,'') as vcSmallPM");
                strSql.AppendLine("		,isnull(vcStandardTime,'') as vcStandardTime");
                strSql.AppendLine("		,case when '" + strBanZhi + "'='白' then iSSPlan_Day else iSSPlan_Night end iPackNum ");
                strSql.AppendLine("from TPackingPlan_Summary ");
                strSql.AppendLine(" where dPackDate='" + strHosDate + "' and vcPlant='" + strPackPlant + "')t1");
                strSql.AppendLine(" group by vcBigPM)t2");
                strSql.AppendLine(" on t1.vcPartItem=t2.vcBigPM");
                strSql.AppendLine(" where vcBigPM is not null");
                strSql.AppendLine(" order by t1.iOrderBy");
                return excute.ExcuteSqlWithSelectToDS(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //获取实行计划数据小品目别
        public DataTable searchPackingPlan_small(string strPackPlant, string strHosDate, string strBanZhi)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("SELECT vcBigPM");
                strSql.AppendLine("		,vcSmallPM");
                strSql.AppendLine("		,CAST(vcStandardTime as decimal(16,2)) as decStandardTime");
                strSql.AppendLine("		,CAST(iPackNum as int) as decPackNum");
                strSql.AppendLine("		,'' as decPlannedTime");
                strSql.AppendLine("		,'' as decPlannedPerson");
                strSql.AppendLine("		,'' as decInputPerson");
                strSql.AppendLine("		,'' as decInputTime");
                strSql.AppendLine("		,'' as decOverFlowTime");
                strSql.AppendLine("FROM ");
                strSql.AppendLine("(select vcPlant as vcPackingPlant");
                strSql.AppendLine("		,dPackDate as dHosDate");
                strSql.AppendLine("		,'" + strBanZhi + "' as vcBanZhi");
                strSql.AppendLine("		,isnull(vcBigPM,'') as vcBigPM");
                strSql.AppendLine("		,isnull(vcSmallPM,'') as vcSmallPM");
                strSql.AppendLine("		,isnull(vcStandardTime,'') as vcStandardTime");
                strSql.AppendLine("		,case when '" + strBanZhi + "'='白' then iSSPlan_Day else iSSPlan_Night end iPackNum");
                strSql.AppendLine("from TPackingPlan_Summary ");
                strSql.AppendLine(" where dPackDate='" + strHosDate + "' and vcPlant='" + strPackPlant + "')t1");
                strSql.AppendLine(" group by vcBigPM,vcSmallPM,vcStandardTime,iPackNum");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //查询当前登录者信息
        public DataTable getPointState(string strPackPlant)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select b.vcPointName as vcBigPM,count(*) as decSysLander from ");
                strSql.AppendLine("(select * from TPointState where (vcState='正常' or vcState='暂停' or vcState='异常') and vcPlant='" + strPackPlant + "')a");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select * from TPointInfo WHERE vcPlant='" + strPackPlant + "')b");
                strSql.AppendLine("on a.vcPointNo=b.vcPointNo");
                strSql.AppendLine("where b.vcPointWork='B'");
                strSql.AppendLine("group by b.vcPointName");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setInPutIntoOverInfo_temp(string strPackPlant, string strHosDate, string strBanZhi, DataTable dtSaveInfo,
            string vcPeopleNum, string vcCycleTime, string vcWorkOverTime, string strOperId, ref DataTable dtMessage)
        {
            SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();

            sqlConnection.Open();
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
                string uuid = System.Guid.NewGuid().ToString();
            try
            {
                #region sqlCommand_delinfo
                SqlCommand sqlCommand_delinfo = sqlConnection.CreateCommand();
                sqlCommand_delinfo.Transaction = sqlTransaction;
                sqlCommand_delinfo.CommandType = CommandType.Text;
                StringBuilder strSql_delinfo = new StringBuilder();
                strSql_delinfo.AppendLine("DELETE FROM [TInPutIntoOver_temp] WHERE [vcPackingPlant]='" + strPackPlant + "' and dHosDate='" + strHosDate + "' and vcBanZhi='" + strBanZhi + "'");
                sqlCommand_delinfo.CommandText = strSql_delinfo.ToString();
                sqlCommand_delinfo.Parameters.AddWithValue("@strPackPlant", "");
                sqlCommand_delinfo.Parameters.AddWithValue("@strHosDate", "");
                sqlCommand_delinfo.Parameters.AddWithValue("@strBanZhi", "");
                sqlCommand_delinfo.Parameters["@strPackPlant"].Value = strPackPlant;
                sqlCommand_delinfo.Parameters["@strHosDate"].Value = strHosDate;
                sqlCommand_delinfo.Parameters["@strBanZhi"].Value = strBanZhi;
                sqlCommand_delinfo.ExecuteNonQuery();
                #endregion

                #region sqlCommand_modinfo
                SqlCommand sqlCommand_modinfo = sqlConnection.CreateCommand();
                sqlCommand_modinfo.Transaction = sqlTransaction;
                sqlCommand_modinfo.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo = new StringBuilder();
                strSql_modinfo.AppendLine("INSERT INTO [dbo].[TInPutIntoOver_temp]");
                strSql_modinfo.AppendLine("           ([uuid]");
                strSql_modinfo.AppendLine("           ,[vcPackingPlant]");
                strSql_modinfo.AppendLine("           ,[dHosDate]");
                strSql_modinfo.AppendLine("           ,[vcBanZhi]");
                strSql_modinfo.AppendLine("           ,[vcPeopleNum]");
                strSql_modinfo.AppendLine("           ,[vcCycleTime]");
                strSql_modinfo.AppendLine("           ,[vcObjective]");
                strSql_modinfo.AppendLine("           ,[vcWorkOverTime]");
                strSql_modinfo.AppendLine("           ,[vcBigPM]");
                strSql_modinfo.AppendLine("           ,[vcSmallPM]");
                strSql_modinfo.AppendLine("           ,[vcStandard]");
                strSql_modinfo.AppendLine("           ,[decPackNum]");
                strSql_modinfo.AppendLine("           ,[decPlannedTime]");
                strSql_modinfo.AppendLine("           ,[decPlannedPerson]");
                strSql_modinfo.AppendLine("           ,[decInputPerson]");
                strSql_modinfo.AppendLine("           ,[decInputTime]");
                strSql_modinfo.AppendLine("           ,[decOverFlowTime]");
                strSql_modinfo.AppendLine("           ,[vcOperatorID]");
                strSql_modinfo.AppendLine("           ,[dOperatorTime])");
                strSql_modinfo.AppendLine("     VALUES");
                strSql_modinfo.AppendLine("           ('" + uuid + "'");
                strSql_modinfo.AppendLine("           ,'" + strPackPlant + "'");
                strSql_modinfo.AppendLine("           ,'" + strHosDate + "'");
                strSql_modinfo.AppendLine("           ,'" + strBanZhi + "'");
                strSql_modinfo.AppendLine("           ,'" + vcPeopleNum + "'");
                strSql_modinfo.AppendLine("           ,'" + vcCycleTime + "'");
                strSql_modinfo.AppendLine("           ,null");
                strSql_modinfo.AppendLine("           ,'" + vcWorkOverTime + "'");
                strSql_modinfo.AppendLine("           ,@vcBigPM");
                strSql_modinfo.AppendLine("           ,@vcSmallPM");
                strSql_modinfo.AppendLine("           ,@vcStandard");
                strSql_modinfo.AppendLine("           ,@decPackNum");
                strSql_modinfo.AppendLine("           ,@decPlannedTime");
                strSql_modinfo.AppendLine("           ,@decPlannedPerson");
                strSql_modinfo.AppendLine("           ,@decInputPerson");
                strSql_modinfo.AppendLine("           ,@decInputTime");
                strSql_modinfo.AppendLine("           ,@decOverFlowTime");
                strSql_modinfo.AppendLine("           ,'" + strOperId + "'");
                strSql_modinfo.AppendLine("           ,GETDATE())");
                sqlCommand_modinfo.CommandText = strSql_modinfo.ToString();
                sqlCommand_modinfo.Parameters.AddWithValue("@vcBigPM", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcSmallPM", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcStandard", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@decPackNum", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@decPlannedTime", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@decPlannedPerson", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@decInputPerson", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@decInputTime", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@decOverFlowTime", "");
                foreach (DataRow item in dtSaveInfo.Rows)
                {
                    sqlCommand_modinfo.Parameters["@vcBigPM"].Value = item["vcBigPM"].ToString();
                    sqlCommand_modinfo.Parameters["@vcSmallPM"].Value = item["vcSmallPM"].ToString();
                    sqlCommand_modinfo.Parameters["@vcStandard"].Value = item["decStandardTime"].ToString();
                    sqlCommand_modinfo.Parameters["@decPackNum"].Value = item["decPackNum"].ToString();
                    sqlCommand_modinfo.Parameters["@decPlannedTime"].Value = item["decPlannedTime"].ToString();
                    sqlCommand_modinfo.Parameters["@decPlannedPerson"].Value = item["decPlannedPerson"].ToString();
                    sqlCommand_modinfo.Parameters["@decInputPerson"].Value = item["decInputPerson"].ToString();
                    sqlCommand_modinfo.Parameters["@decInputTime"].Value = item["decInputTime"].ToString();
                    sqlCommand_modinfo.Parameters["@decOverFlowTime"].Value = item["decOverFlowTime"].ToString();
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
        public void setInPutIntoOverInfo(string strPackPlant, string strHosDate, string strBanZhi, ref DataTable dtMessage)
        {
            SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();

            sqlConnection.Open();
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            try
            {
                string uuid = System.Guid.NewGuid().ToString();
                SqlCommand sqlCommand_modinfo = sqlConnection.CreateCommand();
                sqlCommand_modinfo.Transaction = sqlTransaction;
                sqlCommand_modinfo.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo = new StringBuilder();
                string newid = System.Guid.NewGuid().ToString();
                strSql_modinfo.AppendLine("DELETE FROM [TInPutIntoOver_small] WHERE [vcPackingPlant]=@strPackPlant AND [dHosDate]=@strHosDate AND [vcBanZhi]=@strBanZhi");
                strSql_modinfo.AppendLine("INSERT INTO [dbo].[TInPutIntoOver_small]");
                strSql_modinfo.AppendLine("           ([uuid]");
                strSql_modinfo.AppendLine("           ,[vcPackingPlant]");
                strSql_modinfo.AppendLine("           ,[dHosDate]");
                strSql_modinfo.AppendLine("           ,[vcBanZhi]");
                strSql_modinfo.AppendLine("           ,[vcPeopleNum]");
                strSql_modinfo.AppendLine("           ,[vcCycleTime]");
                strSql_modinfo.AppendLine("           ,[vcObjective]");
                strSql_modinfo.AppendLine("           ,[vcWorkOverTime]");
                strSql_modinfo.AppendLine("           ,[vcBigPM]");
                strSql_modinfo.AppendLine("           ,[vcSmallPM]");
                strSql_modinfo.AppendLine("           ,[vcStandard]");
                strSql_modinfo.AppendLine("           ,[decPackNum]");
                strSql_modinfo.AppendLine("           ,[decPlannedTime]");
                strSql_modinfo.AppendLine("           ,[decPlannedPerson]");
                strSql_modinfo.AppendLine("           ,[decInputPerson]");
                strSql_modinfo.AppendLine("           ,[decInputTime]");
                strSql_modinfo.AppendLine("           ,[decOverFlowTime]");
                strSql_modinfo.AppendLine("           ,[vcOperatorID]");
                strSql_modinfo.AppendLine("           ,[dOperatorTime])");
                strSql_modinfo.AppendLine("SELECT [uuid]");
                strSql_modinfo.AppendLine("      ,[vcPackingPlant]");
                strSql_modinfo.AppendLine("      ,[dHosDate]");
                strSql_modinfo.AppendLine("      ,[vcBanZhi]");
                strSql_modinfo.AppendLine("      ,[vcPeopleNum]");
                strSql_modinfo.AppendLine("      ,[vcCycleTime]");
                strSql_modinfo.AppendLine("      ,[vcObjective]");
                strSql_modinfo.AppendLine("      ,[vcWorkOverTime]");
                strSql_modinfo.AppendLine("      ,[vcBigPM]");
                strSql_modinfo.AppendLine("      ,[vcSmallPM]");
                strSql_modinfo.AppendLine("      ,[vcStandard]");
                strSql_modinfo.AppendLine("      ,[decPackNum]");
                strSql_modinfo.AppendLine("      ,[decPlannedTime]");
                strSql_modinfo.AppendLine("      ,[decPlannedPerson]");
                strSql_modinfo.AppendLine("      ,[decInputPerson]");
                strSql_modinfo.AppendLine("      ,[decInputTime]");
                strSql_modinfo.AppendLine("      ,[decOverFlowTime]");
                strSql_modinfo.AppendLine("      ,[vcOperatorID]");
                strSql_modinfo.AppendLine("      ,[dOperatorTime]");
                strSql_modinfo.AppendLine("  FROM [dbo].[TInPutIntoOver_temp] WHERE [vcPackingPlant]=@strPackPlant AND [dHosDate]=@strHosDate AND [vcBanZhi]=@strBanZhi");
                strSql_modinfo.AppendLine("DELETE FROM [TInPutIntoOver_temp] WHERE [vcPackingPlant]=@strPackPlant AND [dHosDate]=@strHosDate AND [vcBanZhi]=@strBanZhi");
                sqlCommand_modinfo.CommandText = strSql_modinfo.ToString();
                sqlCommand_modinfo.Parameters.AddWithValue("@strPackPlant", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@strHosDate", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@strBanZhi", "");
                sqlCommand_modinfo.Parameters["@strPackPlant"].Value = strPackPlant;
                sqlCommand_modinfo.Parameters["@strHosDate"].Value = strHosDate;
                sqlCommand_modinfo.Parameters["@strBanZhi"].Value = strBanZhi;
                sqlCommand_modinfo.ExecuteNonQuery();
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
