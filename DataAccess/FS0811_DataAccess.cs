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

        public DataTable getBanZhiTime(string strPackPlant, string strFlag)
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
        public DataTable getPageDataInfo()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("SELECT '' as vcPackPlant");
                strSql.AppendLine("		,'' as dHosDate");
                strSql.AppendLine("		,'' as vcBanZhi");
                strSql.AppendLine("		,vcPartItem as vcBigPM");
                strSql.AppendLine("		,'0' as decPackNum");
                strSql.AppendLine("		,'0' as decPlannedTime");
                strSql.AppendLine("		,'0' as decPlannedPerson");
                strSql.AppendLine("		,'0' as decInputPerson");
                strSql.AppendLine("		,'0' as decInputTime");
                strSql.AppendLine("		,'0' as decOverFlowTime");
                strSql.AppendLine("		,'0' as decSysLander");
                strSql.AppendLine("		,'0' as decDiffer");
                strSql.AppendLine("		,'0' as bSelectFlag");
                strSql.AppendLine("FROM [TInPutIntoOver] where vcFlag=0");
                strSql.AppendLine("order by iOrderBy");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataSet getInPutIntoOver_small(string strPackPlant, string strHosDate, string strBanZhi)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("--头部信息");
                strSql.AppendLine("select distinct vcPackPlant");
                strSql.AppendLine("		,Convert(varchar(10),dHosDate,111) as dHosDate");
                strSql.AppendLine("		,vcBanZhi");
                strSql.AppendLine("		,cast(isnull(decPeopleNum,0) as decimal(16,2)) as decPeopleNum");
                strSql.AppendLine("		,cast(isnull(decCycleTime_mi,0) as decimal(16,2)) as decCycleTime_mi");
                strSql.AppendLine("		,cast(isnull(decCycleTime_hr,0) as decimal(16,2)) as decCycleTime_hr");
                strSql.AppendLine("		,cast(isnull(decWorkOverTime,0) as decimal(16,2)) as decWorkOverTime ");
                strSql.AppendLine("from TInPutIntoOver_small");
                strSql.AppendLine("where vcPackPlant='" + strPackPlant + "' and Convert(varchar(10),dHosDate,23)='" + strHosDate + "' and vcBanZhi='" + strBanZhi + "'");
                strSql.AppendLine("--列表信息");
                strSql.AppendLine(" select vcPackPlant");
                strSql.AppendLine("		,Convert(varchar(10),dHosDate,111) as dHosDate");
                strSql.AppendLine("		,vcBanZhi");
                strSql.AppendLine("		,vcBigPM");
                strSql.AppendLine("		,vcSmallPM");
                strSql.AppendLine("		,vcStandard");
                strSql.AppendLine("		,iSSPlan");
                strSql.AppendLine("		,iPackNum");
                strSql.AppendLine("		,iPackNum_summary");
                strSql.AppendLine("		,decPlannedTime");
                strSql.AppendLine("		,decPlannedTime_summary");
                strSql.AppendLine("		,decPlannedPerson");
                strSql.AppendLine("		,decPlannedPerson_summary");
                strSql.AppendLine("		,decPlannedPerson_ratio");
                strSql.AppendLine("		,decInputPerson");
                strSql.AppendLine("		,decInputPerson_summary");
                strSql.AppendLine("		,decInputTime");
                strSql.AppendLine("		,decOverFlowTime");
                strSql.AppendLine("from TInPutIntoOver_small");
                strSql.AppendLine("where vcPackPlant='" + strPackPlant + "' and Convert(varchar(10),dHosDate,23)='" + strHosDate + "' and vcBanZhi='" + strBanZhi + "'");
                return excute.ExcuteSqlWithSelectToDS(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataSet getInPutIntoOver_small_temp(string strPackPlant, string strHosDate, string strBanZhi)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("--头部信息");
                strSql.AppendLine("select distinct vcPackPlant");
                strSql.AppendLine("		,Convert(varchar(10),dHosDate,111) as dHosDate");
                strSql.AppendLine("		,vcBanZhi");
                strSql.AppendLine("		,cast(isnull(decPeopleNum,0) as decimal(16,2)) as decPeopleNum");
                strSql.AppendLine("		,cast(isnull(decCycleTime_mi,0) as decimal(16,2)) as decCycleTime_mi");
                strSql.AppendLine("		,cast(isnull(decCycleTime_hr,0) as decimal(16,2)) as decCycleTime_hr");
                strSql.AppendLine("		,cast(isnull(decWorkOverTime,0) as decimal(16,2)) as decWorkOverTime ");
                strSql.AppendLine("from TInPutIntoOver_small_temp");
                strSql.AppendLine("where vcPackPlant='" + strPackPlant + "' and Convert(varchar(10),dHosDate,23)='" + strHosDate + "' and vcBanZhi='" + strBanZhi + "'");
                strSql.AppendLine("--列表信息");
                strSql.AppendLine(" select vcPackPlant");
                strSql.AppendLine("		,Convert(varchar(10),dHosDate,111) as dHosDate");
                strSql.AppendLine("		,vcBanZhi");
                strSql.AppendLine("		,vcBigPM");
                strSql.AppendLine("		,vcSmallPM");
                strSql.AppendLine("		,vcStandard");
                strSql.AppendLine("		,iSSPlan");
                strSql.AppendLine("		,iPackNum");
                strSql.AppendLine("		,iPackNum_summary");
                strSql.AppendLine("		,decPlannedTime");
                strSql.AppendLine("		,decPlannedTime_summary");
                strSql.AppendLine("		,decPlannedPerson");
                strSql.AppendLine("		,decPlannedPerson_summary");
                strSql.AppendLine("		,decPlannedPerson_ratio");
                strSql.AppendLine("		,decInputPerson");
                strSql.AppendLine("		,decInputPerson_summary");
                strSql.AppendLine("		,decInputTime");
                strSql.AppendLine("		,decOverFlowTime");
                strSql.AppendLine("from TInPutIntoOver_small_temp");
                strSql.AppendLine("where vcPackPlant='" + strPackPlant + "' and Convert(varchar(10),dHosDate,23)='" + strHosDate + "' and vcBanZhi='" + strBanZhi + "'");
                DataSet dataSet = excute.ExcuteSqlWithSelectToDS(strSql.ToString());
                return dataSet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataSet getPackingPlan_summary(string strPackPlant, string strHosDate, string strBanZhi, string strPeopleNum, int iCycleTime_mi)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("declare @iCycleTime_mi int");
                strSql.AppendLine("set @iCycleTime_mi=" + iCycleTime_mi + "");
                strSql.AppendLine("--头部信息");
                strSql.AppendLine("SELECT '" + strPackPlant + "' AS vcPackPlant");
                strSql.AppendLine("		,Convert(varchar(10),'" + strHosDate + "',111) AS dHosDate");
                strSql.AppendLine("		,'" + strBanZhi + "' AS vcBanZhi");
                strSql.AppendLine("		,'" + strPeopleNum + "' AS decPeopleNum");
                strSql.AppendLine("		,CAST(@iCycleTime_mi as decimal(16,2)) AS decCycleTime_mi");
                strSql.AppendLine("		,cast(CAST(@iCycleTime_mi as decimal(16,2))/60.00 as decimal(16,2)) AS decCycleTime_hr");
                strSql.AppendLine("		,'' AS decWorkOverTime");
                strSql.AppendLine("--列表信息");
                strSql.AppendLine("select vcPackPlant");
                strSql.AppendLine("		,dHosDate");
                strSql.AppendLine("		,'" + strBanZhi + "' as vcBanZhi");
                strSql.AppendLine("		,vcBigPM");
                strSql.AppendLine("		,vcSmallPM");
                strSql.AppendLine("		,vcStandard");
                strSql.AppendLine("		,iSSPlan");
                strSql.AppendLine("		,CAST(iSSPlan as int) as iPackNum");
                strSql.AppendLine("		,0 as [iPackNum_summary]");
                strSql.AppendLine("		,CAST(((CAST(vcStandard as int)*CAST(iSSPlan as int))/3600.0000) as decimal(16,4)) as decPlannedTime");
                strSql.AppendLine("		,0.00 as [decPlannedTime_summary]");
                strSql.AppendLine("		,CAST(CAST(((CAST(vcStandard as int)*CAST(iSSPlan as int))/3600.0000) as decimal(16,4))/cast((@iCycleTime_mi/60.00) as decimal(16,4)) as decimal(16,4)) as decPlannedPerson");
                strSql.AppendLine("		,0.00 as [decPlannedPerson_summary]");
                strSql.AppendLine("		,0.0000 as [decPlannedPerson_ratio]");
                strSql.AppendLine("		,0.00 as [decInputPerson]");
                strSql.AppendLine("		,0.00 as [decInputPerson_summary]");
                strSql.AppendLine("		,0.00 as [decInputTime]");
                strSql.AppendLine("		,0.00 as [decOverFlowTime]");
                strSql.AppendLine("from (select vcPlant as vcPackPlant");
                strSql.AppendLine("				,dPackDate as dHosDate");
                strSql.AppendLine("				,isnull(vcBigPM,'') as vcBigPM");
                strSql.AppendLine("				,isnull(vcSmallPM,'') as vcSmallPM");
                strSql.AppendLine("				,isnull(vcStandardTime,'') as vcStandard");
                strSql.AppendLine("				,case when '" + strBanZhi + "'='白' then Cast(iSSPlan_Day as int) else  Cast(iSSPlan_Night as int) end iSSPlan");
                strSql.AppendLine("		from TPackingPlan_Summary ");
                strSql.AppendLine("		where dPackDate='" + strHosDate + "' and vcPlant='" + strPackPlant + "')t1");
                strSql.AppendLine("order by vcBigPM,iSSPlan,vcSmallPM ");
                return excute.ExcuteSqlWithSelectToDS(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
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
                strSql.AppendLine("where b.vcPointWork='Boards'");
                strSql.AppendLine("group by b.vcPointName");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setInPutIntoOver_small_temp(string strPackPlant, string strHosDate, string strBanZhi, DataTable dtSaveInfo,
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
                strSql_delinfo.AppendLine("DELETE FROM [TInPutIntoOver_small_temp] WHERE [vcPackPlant]='" + strPackPlant + "' and dHosDate='" + strHosDate + "' and vcBanZhi='" + strBanZhi + "'");
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
                strSql_modinfo.AppendLine("INSERT INTO [dbo].[TInPutIntoOver_small_temp]");
                strSql_modinfo.AppendLine("           ([uuid]");
                strSql_modinfo.AppendLine("           ,[vcPackPlant]");
                strSql_modinfo.AppendLine("           ,[dHosDate]");
                strSql_modinfo.AppendLine("           ,[vcBanZhi]");
                strSql_modinfo.AppendLine("           ,[decPeopleNum]");
                strSql_modinfo.AppendLine("           ,[decCycleTime_mi]");
                strSql_modinfo.AppendLine("           ,[decCycleTime_hr]");
                strSql_modinfo.AppendLine("           ,[decWorkOverTime]");
                strSql_modinfo.AppendLine("           ,[vcBigPM]");
                strSql_modinfo.AppendLine("           ,[vcSmallPM]");
                strSql_modinfo.AppendLine("           ,[vcStandard]");
                strSql_modinfo.AppendLine("           ,[iSSPlan]");
                strSql_modinfo.AppendLine("           ,[iPackNum]");
                strSql_modinfo.AppendLine("           ,[iPackNum_summary]");
                strSql_modinfo.AppendLine("           ,[decPlannedTime]");
                strSql_modinfo.AppendLine("           ,[decPlannedTime_summary]");
                strSql_modinfo.AppendLine("           ,[decPlannedPerson]");
                strSql_modinfo.AppendLine("           ,[decPlannedPerson_summary]");
                strSql_modinfo.AppendLine("           ,[decPlannedPerson_ratio]");
                strSql_modinfo.AppendLine("           ,[decInputPerson]");
                strSql_modinfo.AppendLine("           ,[decInputPerson_summary]");
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
                strSql_modinfo.AppendLine("           ,CAST('" + vcCycleTime + "' AS decimal(16,2))/60.0");
                strSql_modinfo.AppendLine("           ,'" + vcWorkOverTime + "'");
                strSql_modinfo.AppendLine("           ,@vcBigPM");
                strSql_modinfo.AppendLine("           ,@vcSmallPM");
                strSql_modinfo.AppendLine("           ,@vcStandard");
                strSql_modinfo.AppendLine("           ,@iSSPlan");
                strSql_modinfo.AppendLine("           ,@iPackNum");
                strSql_modinfo.AppendLine("           ,@iPackNum_summary");
                strSql_modinfo.AppendLine("           ,@decPlannedTime");
                strSql_modinfo.AppendLine("           ,@decPlannedTime_summary");
                strSql_modinfo.AppendLine("           ,@decPlannedPerson");
                strSql_modinfo.AppendLine("           ,@decPlannedPerson_summary");
                strSql_modinfo.AppendLine("           ,@decPlannedPerson_ratio");
                strSql_modinfo.AppendLine("           ,@decInputPerson");
                strSql_modinfo.AppendLine("           ,@decInputPerson_summary");
                strSql_modinfo.AppendLine("           ,@decInputTime");
                strSql_modinfo.AppendLine("           ,@decOverFlowTime");
                strSql_modinfo.AppendLine("           ,'" + strOperId + "'");
                strSql_modinfo.AppendLine("           ,GETDATE())");
                sqlCommand_modinfo.CommandText = strSql_modinfo.ToString();
                sqlCommand_modinfo.Parameters.AddWithValue("@vcBigPM", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcSmallPM", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcStandard", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@iSSPlan", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@iPackNum", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@iPackNum_summary", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@decPlannedTime", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@decPlannedTime_summary", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@decPlannedPerson", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@decPlannedPerson_summary", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@decPlannedPerson_ratio", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@decInputPerson", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@decInputPerson_summary", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@decInputTime", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@decOverFlowTime", "");
                foreach (DataRow item in dtSaveInfo.Rows)
                {
                    sqlCommand_modinfo.Parameters["@vcBigPM"].Value = item["vcBigPM"].ToString();
                    sqlCommand_modinfo.Parameters["@vcSmallPM"].Value = item["vcSmallPM"].ToString();
                    sqlCommand_modinfo.Parameters["@vcStandard"].Value = item["vcStandard"].ToString();
                    sqlCommand_modinfo.Parameters["@iSSPlan"].Value = item["iSSPlan"].ToString();
                    sqlCommand_modinfo.Parameters["@iPackNum"].Value = item["iPackNum"].ToString();
                    sqlCommand_modinfo.Parameters["@iPackNum_summary"].Value = item["iPackNum_summary"].ToString();
                    sqlCommand_modinfo.Parameters["@decPlannedTime"].Value = item["decPlannedTime"].ToString();
                    sqlCommand_modinfo.Parameters["@decPlannedTime_summary"].Value = item["decPlannedTime_summary"].ToString();
                    sqlCommand_modinfo.Parameters["@decPlannedPerson"].Value = item["decPlannedPerson"].ToString();
                    sqlCommand_modinfo.Parameters["@decPlannedPerson_summary"].Value = item["decPlannedPerson_summary"].ToString();
                    sqlCommand_modinfo.Parameters["@decPlannedPerson_ratio"].Value = item["decPlannedPerson_ratio"].ToString();
                    sqlCommand_modinfo.Parameters["@decInputPerson"].Value = item["decInputPerson"].ToString();
                    sqlCommand_modinfo.Parameters["@decInputPerson_summary"].Value = item["decInputPerson_summary"].ToString();
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
                strSql_modinfo.AppendLine("DELETE FROM [TInPutIntoOver_small] WHERE [vcPackPlant]=@strPackPlant AND [dHosDate]=@strHosDate AND [vcBanZhi]=@strBanZhi");
                strSql_modinfo.AppendLine("INSERT INTO [dbo].[TInPutIntoOver_small]");
                strSql_modinfo.AppendLine("           ([uuid]");
                strSql_modinfo.AppendLine("           ,[vcPackPlant]");
                strSql_modinfo.AppendLine("           ,[dHosDate]");
                strSql_modinfo.AppendLine("           ,[vcBanZhi]");
                strSql_modinfo.AppendLine("           ,[decPeopleNum]");
                strSql_modinfo.AppendLine("           ,[decCycleTime_mi]");
                strSql_modinfo.AppendLine("           ,[decCycleTime_hr]");
                strSql_modinfo.AppendLine("           ,[decWorkOverTime]");
                strSql_modinfo.AppendLine("           ,[vcBigPM]");
                strSql_modinfo.AppendLine("           ,[vcSmallPM]");
                strSql_modinfo.AppendLine("           ,[vcStandard]");
                strSql_modinfo.AppendLine("           ,[iSSPlan]");
                strSql_modinfo.AppendLine("           ,[iPackNum]");
                strSql_modinfo.AppendLine("           ,[iPackNum_summary]");
                strSql_modinfo.AppendLine("           ,[decPlannedTime]");
                strSql_modinfo.AppendLine("           ,[decPlannedTime_summary]");
                strSql_modinfo.AppendLine("           ,[decPlannedPerson]");
                strSql_modinfo.AppendLine("           ,[decPlannedPerson_summary]");
                strSql_modinfo.AppendLine("           ,[decPlannedPerson_ratio]");
                strSql_modinfo.AppendLine("           ,[decInputPerson]");
                strSql_modinfo.AppendLine("           ,[decInputPerson_summary]");
                strSql_modinfo.AppendLine("           ,[decInputTime]");
                strSql_modinfo.AppendLine("           ,[decOverFlowTime]");
                strSql_modinfo.AppendLine("           ,[vcOperatorID]");
                strSql_modinfo.AppendLine("           ,[dOperatorTime])");
                strSql_modinfo.AppendLine("SELECT [uuid]");
                strSql_modinfo.AppendLine("      ,[vcPackPlant]");
                strSql_modinfo.AppendLine("      ,[dHosDate]");
                strSql_modinfo.AppendLine("      ,[vcBanZhi]");
                strSql_modinfo.AppendLine("      ,[decPeopleNum]");
                strSql_modinfo.AppendLine("      ,[decCycleTime_mi]");
                strSql_modinfo.AppendLine("      ,[decCycleTime_hr]");
                strSql_modinfo.AppendLine("      ,[decWorkOverTime]");
                strSql_modinfo.AppendLine("      ,[vcBigPM]");
                strSql_modinfo.AppendLine("      ,[vcSmallPM]");
                strSql_modinfo.AppendLine("      ,[vcStandard]");
                strSql_modinfo.AppendLine("      ,[iSSPlan]");
                strSql_modinfo.AppendLine("      ,[iPackNum]");
                strSql_modinfo.AppendLine("      ,[iPackNum_summary]");
                strSql_modinfo.AppendLine("      ,[decPlannedTime]");
                strSql_modinfo.AppendLine("      ,[decPlannedTime_summary]");
                strSql_modinfo.AppendLine("      ,[decPlannedPerson]");
                strSql_modinfo.AppendLine("      ,[decPlannedPerson_summary]");
                strSql_modinfo.AppendLine("      ,[decPlannedPerson_ratio]");
                strSql_modinfo.AppendLine("      ,[decInputPerson]");
                strSql_modinfo.AppendLine("      ,[decInputPerson_summary]");
                strSql_modinfo.AppendLine("      ,[decInputTime]");
                strSql_modinfo.AppendLine("      ,[decOverFlowTime]");
                strSql_modinfo.AppendLine("      ,[vcOperatorID]");
                strSql_modinfo.AppendLine("      ,[dOperatorTime]");
                strSql_modinfo.AppendLine("FROM [TInPutIntoOver_small_temp] WHERE [vcPackPlant]=@strPackPlant AND [dHosDate]=@strHosDate AND [vcBanZhi]=@strBanZhi");
                strSql_modinfo.AppendLine("DELETE FROM [TInPutIntoOver_small_temp] WHERE [vcPackPlant]=@strPackPlant AND [dHosDate]=@strHosDate AND [vcBanZhi]=@strBanZhi");
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
