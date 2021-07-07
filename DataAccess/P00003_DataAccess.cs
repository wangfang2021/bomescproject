using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Common;
using System.Data.SqlClient;
using System.Collections;
using System.Net;

namespace DataAccess
{
    public class P00003_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        public void UpdateCase1(string opearteId, string iP)
        {
            //StringBuilder UpdateCaseSql = new StringBuilder();
            //UpdateCaseSql.Append("  update TCaseInfo set vcStatus='1' where vcHostIp='" + iP + "' and vcOperatorID='" + opearteId + "' and vcStatus='0'");
            //return excute.ExcuteSqlWithStringOper(UpdateCaseSql.ToString());
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("  update TCaseInfo set vcStatus='1' where vcHostIp='" + iP + "' and vcOperatorID='" + opearteId + "' and vcStatus='0'");

            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                //return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public DataTable GetUserRole(string user)
        {
            //StringBuilder GetUserRoleSql = new StringBuilder();
            //GetUserRoleSql.Append("select vcPackUnLock from TPointPower  where vcUserId='" + user + "'");
            //return excute.ExcuteSqlWithSelectToDT(GetUserRoleSql.ToString());
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("select vcPackUnLock from TPointPower  where vcUserId='" + user + "'");

            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public void UpdateEffi1(string pointNo, decimal effi)
        {
            //StringBuilder UpdateEffiSql = new StringBuilder();
            //UpdateEffiSql.Append("update TPointState set decEfficacy='" + effi * 100 + "' where vcPointNo='" + pointNo + "'");
            //return excute.ExcuteSqlWithStringOper(UpdateEffiSql.ToString());

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("update TPointState set decEfficacy='" + effi * 100 + "' where vcPointNo='" + pointNo + "'");

            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                //return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public DataTable getBanZhiTime(string strPackPlant, string strFlag)
        {
            SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();
            try
            {
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
            finally
            {
                if (ConnectionState.Open == sqlConnection.State)
                {
                    sqlConnection.Close();
                }
            }
        }

        public DataTable GetPoint(string iP)
        {
            //StringBuilder GetPointSql = new StringBuilder();
            //GetPointSql.Append("select vcPointNo from TPointInfo where vcPointIp = '" + iP + "' and vcUsed = '在用'");
            //return excute.ExcuteSqlWithSelectToDT(GetPointSql.ToString());

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("select vcPointNo from TPointInfo where vcPointIp = '" + iP + "' and vcUsed = '在用'");

            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public DataSet getOperPointInfo(string strPackPlant, string strBanZhi, string strHosDate, string strOperater, string strFromTime_nw)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine("--TBZTime_Rest_point	所在班值休息字典");
            strSql.AppendLine("select ROW_NUMBER() OVER(PARTITION BY vcPackPlant,vcBanZhi ");
            strSql.AppendLine("					ORDER BY dateadd(day,cast(vcBeforCross as int),cast('" + strHosDate + "' as datetime))+' '+tBeforTime) AS TANK");
            strSql.AppendLine("		,vcPackPlant");
            strSql.AppendLine("		,vcBanZhi");
            strSql.AppendLine("		,dateadd(day,cast(vcBeforCross as int),cast('" + strHosDate + "' as datetime))+' '+tBeforTime as  tBeforTime");
            strSql.AppendLine("		,dateadd(day,cast(vcLastCross as int),cast('" + strHosDate + "' as datetime))+' '+tLastTime  as  tLastTime");
            strSql.AppendLine("		,iMinute");
            strSql.AppendLine("from TBZTime_Rest_point where vcPackPlant='" + strPackPlant + "' and vcBanZhi='" + strBanZhi + "'");
            strSql.AppendLine("--TPointDetails	点位登录履历");
            strSql.AppendLine("select dHosDate,vcPlant as vcPackPlant,vcBanZhi,vcPointNo,UUID,dEntryTime,case when dDestroyTime is null then GETDATE() else dDestroyTime end as dDestroyTime");
            strSql.AppendLine("from TPointDetails where vcPlant='" + strPackPlant + "' and vcBanZhi='" + strBanZhi + "' and dHosDate='" + strHosDate + "' and vcOperater='" + strOperater + "'");
            strSql.AppendLine("order by dEntryTime");
            strSql.AppendLine("--TOperateSJ	操作人当日作业基准");
            strSql.AppendLine("SELECT ISNULL(SUM(T1.decQuantity*T2.vcStandardTime),0) AS decOperStandard FROM ");
            strSql.AppendLine("(SELECT A1.vcBZPlant,A2.vcSmallPM,sum(iQuantity) AS decQuantity  FROM ");
            strSql.AppendLine("(select vcBZPlant,vcPart_id,CAST(iQuantity AS int) AS iQuantity,CAST(vcBZUnit AS int) AS vcBZUnit ");
            strSql.AppendLine("from TOperateSJ ");
            strSql.AppendLine("where vcZYType='S2' and vcBZPlant='" + strPackPlant + "' and vcOperatorID='" + strOperater + "' AND dOperatorTime>='" + strFromTime_nw + "' and dOperatorTime<=GETDATE())A1");
            strSql.AppendLine("LEFT JOIN");
            strSql.AppendLine("(select vcBZPlant,vcPart_id,vcSmallPM");
            strSql.AppendLine("from TPackageMaster");
            strSql.AppendLine("where dTimeFrom<=CONVERT(varchar(10),GETDATE(),23) AND dTimeTo>=CONVERT(varchar(10),GETDATE(),23))A2");
            strSql.AppendLine("ON A1.vcBZPlant=A2.vcBZPlant AND A1.vcPart_id=A2.vcPart_id");
            strSql.AppendLine("GROUP BY A1.vcBZPlant,A2.vcSmallPM)T1");
            strSql.AppendLine("LEFT JOIN");
            strSql.AppendLine("(SELECT * FROM TPMRelation)T2");
            strSql.AppendLine("ON T1.vcSmallPM=T2.vcSmallPM");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = strSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
            //return excute.ExcuteSqlWithSelectToDS(strSql.ToString());
        }

        public DataTable checkPrintName(string iP, string strPointType)
        {
            StringBuilder GetPrintSql = new StringBuilder();
            if (strPointType == "COM" || strPointType == "PAD")
            {
                GetPrintSql.Append("select vcUserFlag from TPrint where vcKind in ('CASE PRINTER','LABEL PRINTER') and vcPrinterIp='" + iP + "'");
            }
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();

            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = GetPrintSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public void UpdateStatus4(string pointNo, string opearteId)
        {
            StringBuilder UpdateStatusSql = new StringBuilder();
            UpdateStatusSql.Append("update TPointState set vcState='暂停' where vcPointNo='" + pointNo + "' and vcOperater='" + opearteId + "'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();

            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = UpdateStatusSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                //return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public void UpdateStatus5(string pointNo, string opearteId)
        {
            StringBuilder UpdateStatusSql = new StringBuilder();
            UpdateStatusSql.Append("update TPointState set vcState='正常' where vcPointNo='" + pointNo + "' and vcOperater='" + opearteId + "'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();

            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = UpdateStatusSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                //return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public void UpdateEffi(string formatDate, string opearteId, string stopTime)
        {
            StringBuilder UpdateEffiSql = new StringBuilder();
            UpdateEffiSql.Append("update  TOperateSJ_Effiency set iStopTime=" + stopTime + " where vcDate='" + formatDate + "' and vcBZPlant='H2' and vcOperatorID='" + opearteId + "'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();

            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = UpdateEffiSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                //return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public DataTable GetStatus2(string iP, string opearteId)
        {
            StringBuilder GetStatusSql = new StringBuilder();
            GetStatusSql.Append("select t1.vcState,t1.vcPointNo from TPointState t1,TPointInfo t2 where t1.vcPointNo=t2.vcPointNo and t2.vcPointIp='" + iP + "' and t2.vcUsed='在用'   and t1.vcOperater='" + opearteId + "'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();

            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = GetStatusSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public DataTable GetTime(string formatDate, string opearteId)
        {
            StringBuilder GetTimeSql = new StringBuilder();
            GetTimeSql.Append("  select vcTotalTime,iFrequency,vcEffiency,dStartTime,ISNULL(vcPackTotalTime,0) from TOperateSJ_Effiency where vcOperatorID='" + opearteId + "' and vcDate='" + formatDate + "'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();

            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = GetTimeSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }

        }

        public void UpdateFre(string time, string serverTime, string formatDate, string opearteId)
        {
            StringBuilder UpdateFreSql = new StringBuilder();
            UpdateFreSql.Append("update TOperateSJ_Effiency set dStartTime='" + time + "',dOperatorTime='" + serverTime + "' where vcOperatorID='" + opearteId + "' and vcDate='" + formatDate + "'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();

            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = UpdateFreSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                //return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public void InsertFre(string time, string formatDate, string effiEncy, string opearteId, string serverTime, string iP, string date, string banZhi)
        {
            StringBuilder InsertFreSql = new StringBuilder();
            // InsertFreSql.Append("INSERT INTO TOperateSJ_Effiency (dStartTime ,vcDate ,vcTotalTime,iFrequency ,vcEffiency,vcOperatorID,dOperatorTime) \n");
            // InsertFreSql.Append("     VALUES ('" + time + "','" + formatDate + "','0',0,'" + effiEncy + "','" + opearteId + "','" + serverTime + "') \n");
            InsertFreSql.Append("INSERT INTO TOperateSJ_Effiency(vcBZPlant,vcPointIp,dHosDate,vcBanZhi,dStartTime\n");
            InsertFreSql.Append(" ,iStopTime,vcDate,vcTotalTime,iFrequency,vcEffiency,vcOperatorID,dOperatorTime,vcPackTotalTime)\n");
            InsertFreSql.Append("     VALUES('H2','" + iP + "','" + date + "','" + banZhi + "','" + time + "',0,'" + formatDate + "',0,0,'" + effiEncy + "','" + opearteId + "','" + serverTime + "','0')\n");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();

            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = InsertFreSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                //return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public DataTable GetStanTime()
        {
            StringBuilder GetStanTimeSql = new StringBuilder();
            GetStanTimeSql.Append("select decObjective from TDisplaySettings");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();

            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = GetStanTimeSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public DataTable GetPackData(string partId, string scanTime)
        {
            StringBuilder GetPackDataSql = new StringBuilder();
            string time = scanTime.Replace("-", "").Substring(0, 8);
            GetPackDataSql.Append("  select vcDistinguish as distinguish,count(vcDistinguish) as sum from TPackItem where vcPartsNo='" + partId + "' and dFrom<'" + time + "' and dTo>'" + time + "' group by vcDistinguish order by vcDistinguish");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();

            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = GetPackDataSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public DataTable GetBanZhi(string serverTime)
        {
            StringBuilder GetBanZhiSql = new StringBuilder();
            GetBanZhiSql.Append("select dHosDate,vcBanZhi from (select convert(varchar(10),dateadd(DAY,-1,GETDATE()),23) as dHosDate,vcBanZhi,convert(varchar(10),dateadd(DAY,-1,GETDATE()),23)+' '+convert(varchar(10),tFromTime,24) as tFromTime,case when vcCross='1' then convert(varchar(10),dateadd(day,1,dateadd(DAY,-1,GETDATE())),23) else convert(varchar(10),dateadd(DAY,-1,GETDATE()),23) end +' '+convert(varchar(10),tToTime,24) as tToTime from TBZTime where vcBanZhi='夜' and vcPackPlant='H2'\n");
            GetBanZhiSql.Append("union select convert(varchar(10), dateadd(DAY, 0, GETDATE()), 23) as dHosDate, vcBanZhi, convert(varchar(10), dateadd(DAY, 0, GETDATE()), 23) + ' ' + convert(varchar(10), tFromTime, 24) as tFromTime,case when vcCross = '1' then convert(varchar(10), dateadd(day, 1, dateadd(DAY, 0, GETDATE())), 23) else convert(varchar(10), dateadd(DAY, 0, GETDATE()), 23) end + ' ' + convert(varchar(10), tToTime, 24) as tToTime from TBZTime where vcBanZhi = '白' and vcPackPlant = 'H2'\n");
            GetBanZhiSql.Append("union select convert(varchar(10), dateadd(DAY, 0, GETDATE()), 23) as dHosDate, vcBanZhi, convert(varchar(10), dateadd(DAY, 0, GETDATE()), 23) + ' ' + convert(varchar(10), tFromTime, 24) as tFromTime,case when vcCross = '1' then convert(varchar(10), dateadd(day, 1, dateadd(DAY, 0, GETDATE())), 23) else convert(varchar(10), dateadd(DAY, 0, GETDATE()), 23) end + ' ' + convert(varchar(10), tToTime, 24) as tToTime from TBZTime where vcBanZhi = '夜' and vcPackPlant = 'H2'\n");
            GetBanZhiSql.Append("union select convert(varchar(10), dateadd(DAY, 1, GETDATE()), 23) as dHosDate, vcBanZhi, convert(varchar(10), dateadd(DAY, 1, GETDATE()), 23) + ' ' + convert(varchar(10), tFromTime, 24) as tFromTime,case when vcCross = '1' then convert(varchar(10), dateadd(day, 1, dateadd(DAY, 1, GETDATE())), 23) else convert(varchar(10), dateadd(DAY, 1, GETDATE()), 23) end + ' ' + convert(varchar(10), tToTime, 24) as tToTime from TBZTime where vcBanZhi = '白' and vcPackPlant = 'H2'\n");
            GetBanZhiSql.Append(")t where tFromTime<=GETDATE() and tToTime>=GETDATE()\n");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();

            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = GetBanZhiSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public DataTable GetCase(string opearteId)
        {
            StringBuilder GetCaseSql = new StringBuilder();
            GetCaseSql.Append("select top(1)vcBoxNo from TCaseInfo where  vcStatus='0' and vcOperatorID='" + opearteId + "' order by dOperatorTime desc");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();

            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = GetCaseSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public DataTable GetPM(string dock, string partId)
        {
            StringBuilder GetPMSql = new StringBuilder();
            string formatpartId = partId.Substring(0, 5);
            GetPMSql.Append("  select vcSmallPM from TPackageMaster where vcPart_id='" + partId + "' and vcSR='" + dock + "'");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();

            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = GetPMSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public DataTable GetData(string partId, string dock, string kanbanOrderNo, string kanbanSerial)
        {
            StringBuilder GetDataSql = new StringBuilder();
            GetDataSql.Append("  select vcBZUnit,vcCheckType from  TOperateSJ where vcPart_id='" + partId + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "' and vcSR='" + dock + "' and vcZYType='S0' ");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();

            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = GetDataSql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        //========================================================================重写========================================================================
        public DataTable getOperCaseNo(string iP, string strPointState, string strOperatorID)
        {
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("select * from TCaseInfo where vcPointState='" + strPointState + "' and vcHostIp='" + iP + "' and dBoxPrintTime is null order by dOperatorTime desc");
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }
        public DataTable GetCaseNoInfo(string strCaseNo)
        {
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("SELECT A.vcPlant,A.vcReceiver,A.vcCaseNo,");
            stringBuilder.AppendLine("		B.vcBoxNo,B.vcHostIp,B.vcSheBeiNo,B.vcOperatorID,B.vcPointState,B.dBoxPrintTime,");
            stringBuilder.AppendLine("		C.vcPointType+C.vcPointNo as vcPointType,");
            stringBuilder.AppendLine("		ISNULL(D.kanbanQuantity,0) AS kanbanQuantity");
            stringBuilder.AppendLine("		FROM ");
            stringBuilder.AppendLine("(select * from TCaseNoInfo where vcPlant+'*'+vcCaseNo='" + strCaseNo + "')A");
            stringBuilder.AppendLine("LEFT JOIN");
            stringBuilder.AppendLine("(select * from TCaseInfo)B");
            stringBuilder.AppendLine("ON A.vcPlant+'*'+A.vcCaseNo=B.vcCaseNo");
            stringBuilder.AppendLine("LEFT JOIN");
            stringBuilder.AppendLine("(select vcPointType,vcPointNo,vcPointIp from TPointInfo where vcUsed='在用')C");
            stringBuilder.AppendLine("ON B.vcHostIp=c.vcPointIp");
            stringBuilder.AppendLine(" LEFT JOIN");
            stringBuilder.AppendLine("(select vcCaseNo,COUNT(*) AS kanbanQuantity from TBoxMaster where vcDelete='0' and vcCaseNo='" + strCaseNo + "' GROUP BY vcCaseNo)D");
            stringBuilder.AppendLine("ON A.vcPlant+'*'+A.vcCaseNo=D.vcCaseNo");
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }
        public void SetCaseNoInfo(string strBoxNo, string strCaseNo, string strHostIp, string strSheBeiNo, string strPointState, string strOperatorID)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DELETE FROM [TCaseInfo] WHERE [vcBoxNo]='" + strBoxNo + "'");
            stringBuilder.AppendLine("INSERT INTO [dbo].[TCaseInfo]([vcBoxNo],[vcCaseNo],[vcHostIp],[vcSheBeiNo],[vcPointState],[dBoxPrintTime],[vcOperatorID],[dOperatorTime])");
            stringBuilder.AppendLine("     VALUES('" + strBoxNo + "','" + strCaseNo + "','" + strHostIp + "','" + strSheBeiNo + "','" + strPointState + "',NULL,'" + strOperatorID + "',GETDATE())");
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }
        public DataTable GetKanBanInfo(string partId, string kanbanOrderNo, string kanbanSerial, string dock, string scanTime)
        {
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("select a.vcPart_id--入荷品番");
            stringBuilder.AppendLine("		,a.vcKBOrderNo");
            stringBuilder.AppendLine("		,a.vcKBLFNo");
            stringBuilder.AppendLine("		,a.vcSR");
            stringBuilder.AppendLine("		,a.vcSupplier_id");
            stringBuilder.AppendLine("		,a.iQuantity");
            stringBuilder.AppendLine("		,a.vcBZUnit");
            stringBuilder.AppendLine("		,b.vcCheckP--检查区分");
            stringBuilder.AppendLine("		,c.vcCheckStatus--检查状态");
            stringBuilder.AppendLine("		,d.vcPart_id as vcPart_id_InOut--入库履历校验");
            stringBuilder.AppendLine("		,isnull(d.iDBZ,0) as iDBZ--待包装");
            stringBuilder.AppendLine("		,isnull(d.iDZX,0) as iDZX--待装箱");
            stringBuilder.AppendLine("		,isnull(d.iDCH,0) as iDCH--待出荷");
            stringBuilder.AppendLine("		,a.vcInputNo--指令书号");
            stringBuilder.AppendLine("		,isnull(g.vcPicUrl,'暂无图像.jpg') as vcPicUrl--四分图");
            stringBuilder.AppendLine("		,h.vcSmallPM--小品目");
            stringBuilder.AppendLine("		,a.vcBZUnit--包装单位");
            stringBuilder.AppendLine("		,isnull(e.iQuantity,0) as iQuantity_bz--包装数量");
            stringBuilder.AppendLine("		,isnull(f.iQuantity,0) as iQuantity_zx--装箱数量");
            stringBuilder.AppendLine("from ");
            stringBuilder.AppendLine("(select * from TOperateSJ ");
            stringBuilder.AppendLine("where vcPart_id='" + partId + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "' and vcSR='" + dock + "' and vcZYType='S0')a");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(	select t1.vcPartId,t1.vcSupplierId,t2.vcCheckP from ");
            stringBuilder.AppendLine("	(select * from tCheckMethod_Master where vcPartId='" + partId + "' and dFromTime<=GETDATE() and dToTime>=GETDATE())t1");
            stringBuilder.AppendLine("	left join");
            stringBuilder.AppendLine("	(SELECT * FROM [tCheckQf] ");
            stringBuilder.AppendLine("	where [vcTimeFrom]<=GETDATE()  and [vcTimeTo]>=GETDATE())t2");
            stringBuilder.AppendLine("	ON t1.vcPartId=t2.vcPartId AND t1.vcSupplierId=t2.[vcSupplierCode])b");
            stringBuilder.AppendLine("on a.vcPart_id=b.vcPartId and a.vcSupplier_id=b.vcSupplierId");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select * from TOperateSJ ");
            stringBuilder.AppendLine("where  vcPart_id='" + partId + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "' and vcSR='" + dock + "' and vcZYType='S1')c");
            stringBuilder.AppendLine("on a.vcPart_id=c.vcPart_id and a.vcKBOrderNo=c.vcKBOrderNo and a.vcKBLFNo=c.vcKBLFNo and a.vcSR=c.vcSR");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select * from TOperateSJ_InOutput ");
            stringBuilder.AppendLine("where vcPart_id='" + partId + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "' and vcSR='" + dock + "' )d");
            stringBuilder.AppendLine("on a.vcPart_id=d.vcPart_id and a.vcKBOrderNo=d.vcKBOrderNo and a.vcKBLFNo=d.vcKBLFNo and a.vcSR=d.vcSR");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select vcPart_id,vcKBOrderNo,vcKBLFNo,vcSR,SUM(CAST(iQuantity as int)) as iQuantity from TOperateSJ ");
            stringBuilder.AppendLine("where vcPart_id='" + partId + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "' and vcSR='" + dock + "' and vcZYType='S2'");
            stringBuilder.AppendLine("group by vcPart_id,vcKBOrderNo,vcKBLFNo,vcSR)e");
            stringBuilder.AppendLine("on a.vcPart_id=e.vcPart_id and a.vcKBOrderNo=e.vcKBOrderNo and a.vcKBLFNo=e.vcKBLFNo and a.vcSR=e.vcSR");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select vcPart_id,vcOrderNo,vcLianFanNo,vcSR,SUM(CAST(iQuantity as int)) as iQuantity from TBoxMaster ");
            stringBuilder.AppendLine("where vcPart_id='" + partId + "' and vcOrderNo='" + kanbanOrderNo + "' and vcLianFanNo='" + kanbanSerial + "' and vcSR='" + dock + "' and vcDelete='0'");
            stringBuilder.AppendLine("group by vcPart_id,vcOrderNo,vcLianFanNo,vcSR)f");
            stringBuilder.AppendLine("on a.vcPart_id=f.vcPart_id and a.vcKBOrderNo=f.vcOrderNo and a.vcKBLFNo=f.vcLianFanNo and a.vcSR=f.vcSR");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select * from TPackOperImage)g");
            stringBuilder.AppendLine("on a.vcBZPlant=g.vcPlant and a.vcPart_id=g.vcPartId");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select * from TPackageMaster where vcPart_id='" + partId + "' and  dTimeFrom<=GETDATE()  and dTimeTo>=GETDATE() )h");
            stringBuilder.AppendLine("on a.vcPart_id=h.vcPart_id and a.vcSHF=h.vcReceiver and a.vcSupplier_id=h.vcSupplierId and a.vcSR=h.vcSR and a.vcBZPlant=h.vcBZPlant");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("");
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public DataTable GetPrintName(string iP, string strKind)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (strKind == "LABEL PRINTER")
            {
                stringBuilder.Append("select vcPrinterName from TPrint where vcPrinterIp='" + iP + "' and vcKind='LABEL PRINTER'");
            }
            if (strKind == "CASE PRINTER")
            {
                stringBuilder.Append("select vcPrinterName from TPrint where vcPrinterIp='" + iP + "' and vcKind='CASE PRINTER'");

            }
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();

            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }

        public DataTable GetPackList(string strInno)
        {
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("select vcPackingpartsno as vcPackNo,vcDistinguish as vcDistinguish,sum(cast(dQty as decimal(16,5))) as sum from TPackList ");
            stringBuilder.Append("where vcInno='" + strInno + "'");
            stringBuilder.Append("group by vcPackingpartsno,vcDistinguish");
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }
        public bool setPrintLable(string strIP, string strInvNo, string strPrinterName, string strOperId)
        {
            SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();
            sqlConnection.Open();
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            try
            {
                #region 写入数据库

                #region 11.sqlCommand_printinfo
                SqlCommand sqlCommand_printinfo = sqlConnection.CreateCommand();
                sqlCommand_printinfo.Transaction = sqlTransaction;
                sqlCommand_printinfo.CommandType = CommandType.Text;
                StringBuilder strSql_printinfo = new StringBuilder();

                #region SQL and Parameters
                strSql_printinfo.AppendLine("INSERT INTO [dbo].[TPrint_Temp]");
                strSql_printinfo.AppendLine("           ([vcTableName]");
                strSql_printinfo.AppendLine("           ,[vcReportName]");
                strSql_printinfo.AppendLine("           ,[vcClientIP]");
                strSql_printinfo.AppendLine("           ,[vcPrintName]");
                strSql_printinfo.AppendLine("           ,[vcKind]");
                strSql_printinfo.AppendLine("           ,[vcOperatorID]");
                strSql_printinfo.AppendLine("           ,[dOperatorTime]");
                strSql_printinfo.AppendLine("           ,[vcCaseNo]");
                strSql_printinfo.AppendLine("           ,[vcSellNo]");
                strSql_printinfo.AppendLine("           ,[vcLotid]");
                strSql_printinfo.AppendLine("           ,[vcSupplierId]");
                strSql_printinfo.AppendLine("           ,[vcInno]");
                strSql_printinfo.AppendLine("           ,[vcFlag])");
                strSql_printinfo.AppendLine("		   select distinct 'TLabelList'");
                strSql_printinfo.AppendLine("		   ,'SPR06LBIP'");
                strSql_printinfo.AppendLine("		   ,@IP");
                strSql_printinfo.AppendLine("		   ,'" + strPrinterName + "'");
                strSql_printinfo.AppendLine("		   ,'6'");
                strSql_printinfo.AppendLine("		   ,'" + strOperId + "'");
                strSql_printinfo.AppendLine("		   ,GETDATE()");
                strSql_printinfo.AppendLine("		   ,null");
                strSql_printinfo.AppendLine("		   ,null");
                strSql_printinfo.AppendLine("		   ,null");
                strSql_printinfo.AppendLine("		   ,null");
                strSql_printinfo.AppendLine("		   ,@InvNo");
                strSql_printinfo.AppendLine("		   ,'0'");
                sqlCommand_printinfo.CommandText = strSql_printinfo.ToString();
                sqlCommand_printinfo.Parameters.AddWithValue("@IP", strIP);
                sqlCommand_printinfo.Parameters.AddWithValue("@InvNo", strInvNo);
                #endregion
                sqlCommand_printinfo.ExecuteNonQuery();
                #endregion

                //提交事务
                sqlTransaction.Commit();
                sqlConnection.Close();
                return true;
                #endregion

            }
            catch (Exception ex)
            {
                //0613记录日志

                ComMessage.GetInstance().ProcessMessage("P00001", "M03UE0901", ex, "000000");
                //回滚事务
                if (sqlTransaction != null && sqlConnection != null)
                {
                    sqlTransaction.Rollback();
                    sqlConnection.Close();
                }
                return false;
            }
        }
        public DataTable getPackInfo(string partId, string kanbanOrderNo, string kanbanSerial, string dock, string packQuantity)
        {
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("select  '2' as vcZuoYeQuFen");
            stringBuilder.AppendLine("		,a.vcInputNo as vcOrderNo");
            stringBuilder.AppendLine("		,b.vcPackNo as vcPackNo");
            stringBuilder.AppendLine("		,b.vcPackGPSNo as vcPackGPSNo");
            stringBuilder.AppendLine("		,c.vcSupplierCode as vcSupplierID");
            stringBuilder.AppendLine("		,a.vcBZPlant as vcPackSpot");
            stringBuilder.AppendLine("		,cast(cast(b.iBiYao as decimal(16,5))*cast('" + packQuantity + "' as int)/cast(d.vcBZUnit as decimal(16,5)) as decimal(16,5)) as dQty");
            stringBuilder.AppendLine("		from ");
            stringBuilder.AppendLine("(select * from TOperateSJ where vcPart_id='" + partId + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "' and vcSR='" + dock + "' and vcZYType='S0')a");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select * from TPackItem)b");
            stringBuilder.AppendLine("on a.vcPart_id = b.vcPartsNo and b.dUsedFrom<=a.dstart and b.dUsedTo>=a.dstart");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select * from TPackBase)c");
            stringBuilder.AppendLine("on b.vcPackNo=c.vcPackNo and a.vcBZPlant=c.vcPackSpot and c.dPackFrom<=a.dstart and c.dPackTo>=a.dstart");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select * from TPackageMaster)d");
            stringBuilder.AppendLine("on a.vcPart_id=d.vcPart_id and d.dTimeFrom<=a.dstart and d.dTimeTo>=a.dstart");
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }
        public DataSet getTableFromDB(string serverTime)
        {
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("--作业实绩表");
            stringBuilder.AppendLine("SELECT TOP (1)*  FROM [TOperateSJ]");
            stringBuilder.AppendLine("--装箱单表");
            stringBuilder.AppendLine("SELECT TOP (1)*  FROM [TCaseList]");
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }
        public bool setPackAndZxInfo(string strIP, string strPointName, string strType, string partId, string kanbanOrderNo, string kanbanSerial, string dock, string packQuantity, string caseno, string boxno, string scanTime, DataTable dtPackList, string strOperId)
        {
            //------
            SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();
            sqlConnection.Open();
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            try
            {
                //StringBuilder stringBuilder = new StringBuilder();
                //stringBuilder.AppendLine("select * from ");
                //stringBuilder.AppendLine("(select vcInputNo,vcPart_id,vcKBOrderNo,vcKBLFNo,vcSR,iQuantity as iQuantity_rh");
                //stringBuilder.AppendLine("		,cast(case when ''='' then '0' else '' end as int) as iQuantity_bcbz ");
                //stringBuilder.AppendLine("from TOperateSJ WHERE vcZYType in ('S0')");
                //stringBuilder.AppendLine("and vcPart_id='' and vcKBOrderNo='' and vcKBLFNo='' and vcSR='')a");
                //stringBuilder.AppendLine("left join");
                //stringBuilder.AppendLine("(select vcPart_id,vcKBOrderNo,vcKBLFNo,vcSR,ISNULL(sum(cast(iQuantity as int)),0) as iQuantity_sum");
                //stringBuilder.AppendLine("from TOperateSJ where vcZYType in ('S2') group by vcPart_id,vcKBOrderNo,vcKBLFNo,vcSR)b");
                //stringBuilder.AppendLine("on a.vcPart_id=b.vcPart_id and a.vcKBOrderNo=b.vcKBOrderNo and a.vcKBLFNo=b.vcKBLFNo and a.vcSR=b.vcSR");
                //stringBuilder.AppendLine("WHERE a.iQuantity_bcbz+ISNULL(b.iQuantity_sum,0)<=a.iQuantity_rh AND cast(a.iQuantity_bcbz as int)>0");
                //string strSQL = stringBuilder.ToString();
                //SqlDataAdapter da = new SqlDataAdapter(strSQL, sqlConnection);
                //DataSet ds = new DataSet();
                //da.Fill(ds);
                //if (ds.Tables[0].Rows.Count != 0)
                //{
                if (strType != "包装不装箱")
                {
                    #region 2.插入装箱实绩TBoxMaster  sqlCommand_add_bm
                    SqlCommand sqlCommand_add_bm = sqlConnection.CreateCommand();
                    sqlCommand_add_bm.Transaction = sqlTransaction;
                    sqlCommand_add_bm.CommandType = CommandType.Text;
                    StringBuilder strSql_add_bm = new StringBuilder();

                    #region SQL and Parameters
                    strSql_add_bm.AppendLine("declare @flag int");
                    strSql_add_bm.AppendLine("set @flag=(select count(*) from TBoxMaster where vcCaseNo=@CaseNo and vcPart_id=@Part_id and vcOrderNo=@KBOrderNo and vcLianFanNo=@KBLFNo and vcSR=@SR AND vcDelete='0')");
                    strSql_add_bm.AppendLine("if(@flag=0)");
                    strSql_add_bm.AppendLine("begin");
                    strSql_add_bm.AppendLine("INSERT INTO [dbo].[TBoxMaster]");
                    strSql_add_bm.AppendLine("           ([vcStatus]");
                    strSql_add_bm.AppendLine("           ,[vcCaseNo]");
                    strSql_add_bm.AppendLine("           ,[vcBoxNo]");
                    strSql_add_bm.AppendLine("           ,[vcInstructionNo]");
                    strSql_add_bm.AppendLine("           ,[vcPart_id]");
                    strSql_add_bm.AppendLine("           ,[vcOrderNo]");
                    strSql_add_bm.AppendLine("           ,[vcLianFanNo]");
                    strSql_add_bm.AppendLine("           ,[iQuantity]");
                    strSql_add_bm.AppendLine("           ,[dBZID]");
                    strSql_add_bm.AppendLine("           ,[dBZTime]");
                    strSql_add_bm.AppendLine("           ,[dZXID]");
                    strSql_add_bm.AppendLine("           ,[dZXTime]");
                    strSql_add_bm.AppendLine("           ,[vcOperatorID]");
                    strSql_add_bm.AppendLine("           ,[dOperatorTime]");
                    strSql_add_bm.AppendLine("           ,[iRHQuantity]");
                    strSql_add_bm.AppendLine("           ,[vcLabelStart]");
                    strSql_add_bm.AppendLine("           ,[vcLabelEnd]");
                    strSql_add_bm.AppendLine("           ,[vcDelete]");
                    strSql_add_bm.AppendLine("           ,[dPrintBoxTime]");
                    strSql_add_bm.AppendLine("           ,[vcSR])");
                    strSql_add_bm.AppendLine("select top(1)");
                    strSql_add_bm.AppendLine("		'' as vcStatus");
                    strSql_add_bm.AppendLine("		,@CaseNo");
                    strSql_add_bm.AppendLine("		,@BoxNo");
                    strSql_add_bm.AppendLine("		,vcInputNo as vcInstructionNo");
                    strSql_add_bm.AppendLine("		,vcPart_id as vcPart_id");
                    strSql_add_bm.AppendLine("		,vcKBOrderNo as vcOrderNo");
                    strSql_add_bm.AppendLine("		,vcKBLFNo as vcLianFanNo");
                    strSql_add_bm.AppendLine("		,@Quantity");
                    strSql_add_bm.AppendLine("		,@OperId");
                    strSql_add_bm.AppendLine("		,GETDATE()");
                    strSql_add_bm.AppendLine("		,@OperId");
                    strSql_add_bm.AppendLine("		,GETDATE()");
                    strSql_add_bm.AppendLine("		,@OperId");
                    strSql_add_bm.AppendLine("		,GETDATE()");
                    strSql_add_bm.AppendLine("		,iQuantity");
                    strSql_add_bm.AppendLine("		,vcLabelStart");
                    strSql_add_bm.AppendLine("		,vcLabelEnd");
                    strSql_add_bm.AppendLine("		,'0' as vcDelete");
                    strSql_add_bm.AppendLine("		,null as dPrintBoxTime");
                    strSql_add_bm.AppendLine("		,vcSR");
                    strSql_add_bm.AppendLine("		from TOperateSJ ");
                    strSql_add_bm.AppendLine("		where vcPart_id=@Part_id and vcKBOrderNo=@KBOrderNo and vcKBLFNo=@KBLFNo and vcSR=@SR and vcZYType in ('S0','S1') order by iAutoId desc");
                    strSql_add_bm.AppendLine("end");
                    strSql_add_bm.AppendLine("else");
                    strSql_add_bm.AppendLine("begin");
                    strSql_add_bm.AppendLine("UPDATE [TBoxMaster] SET iQuantity=iQuantity+@Quantity,dBZID=@OperId,dBZTime=GETDATE(),dZXID=@OperId,dZXTime=GETDATE(),vcOperatorID=@OperId,dOperatorTime=GETDATE()");
                    strSql_add_bm.AppendLine("where vcCaseNo=@CaseNo and vcPart_id=@Part_id and vcOrderNo=@KBOrderNo and vcLianFanNo=@KBLFNo and vcSR=@SR AND vcDelete='0'");
                    strSql_add_bm.AppendLine("end");

                    sqlCommand_add_bm.CommandText = strSql_add_bm.ToString();
                    sqlCommand_add_bm.Parameters.AddWithValue("@CaseNo", caseno);
                    sqlCommand_add_bm.Parameters.AddWithValue("@BoxNo", boxno);
                    sqlCommand_add_bm.Parameters.AddWithValue("@Quantity", packQuantity);
                    sqlCommand_add_bm.Parameters.AddWithValue("@OperId", strOperId);
                    sqlCommand_add_bm.Parameters.AddWithValue("@Part_id", partId);
                    sqlCommand_add_bm.Parameters.AddWithValue("@KBOrderNo", kanbanOrderNo);
                    sqlCommand_add_bm.Parameters.AddWithValue("@KBLFNo", kanbanSerial);
                    sqlCommand_add_bm.Parameters.AddWithValue("@SR", dock);
                    #endregion
                    sqlCommand_add_bm.ExecuteNonQuery();
                    #endregion
                }
                if (strType != "只装箱")
                {
                    #region 1.插入作业实绩TOperateSJ  sqlCommand_add_sj
                    SqlCommand sqlCommand_add_sj = sqlConnection.CreateCommand();
                    sqlCommand_add_sj.Transaction = sqlTransaction;
                    sqlCommand_add_sj.CommandType = CommandType.Text;
                    StringBuilder strSql_add_sj = new StringBuilder();

                    #region SQL and Parameters
                    strSql_add_sj.AppendLine("INSERT INTO [dbo].[TOperateSJ]");
                    strSql_add_sj.AppendLine("           ([vcZYType]");
                    strSql_add_sj.AppendLine("           ,[vcBZPlant]");
                    strSql_add_sj.AppendLine("           ,[vcInputNo]");
                    strSql_add_sj.AppendLine("           ,[vcKBOrderNo]");
                    strSql_add_sj.AppendLine("           ,[vcKBLFNo]");
                    strSql_add_sj.AppendLine("           ,[vcPart_id]");
                    strSql_add_sj.AppendLine("           ,[vcIOType]");
                    strSql_add_sj.AppendLine("           ,[vcSupplier_id]");
                    strSql_add_sj.AppendLine("           ,[vcSupplierGQ]");
                    strSql_add_sj.AppendLine("           ,[dStart]");
                    strSql_add_sj.AppendLine("           ,[dEnd]");
                    strSql_add_sj.AppendLine("           ,[iQuantity]");
                    strSql_add_sj.AppendLine("           ,[vcBZUnit]");
                    strSql_add_sj.AppendLine("           ,[vcSHF]");
                    strSql_add_sj.AppendLine("           ,[vcSR]");
                    strSql_add_sj.AppendLine("           ,[vcBoxNo]");
                    strSql_add_sj.AppendLine("           ,[vcSheBeiNo]");
                    strSql_add_sj.AppendLine("           ,[vcCheckType]");
                    strSql_add_sj.AppendLine("           ,[iCheckNum]");
                    strSql_add_sj.AppendLine("           ,[vcCheckStatus]");
                    strSql_add_sj.AppendLine("           ,[vcLabelStart]");
                    strSql_add_sj.AppendLine("           ,[vcLabelEnd]");
                    strSql_add_sj.AppendLine("           ,[vcUnlocker]");
                    strSql_add_sj.AppendLine("           ,[dUnlockTime]");
                    strSql_add_sj.AppendLine("           ,[vcSellNo]");
                    strSql_add_sj.AppendLine("           ,[vcOperatorID]");
                    strSql_add_sj.AppendLine("           ,[dOperatorTime]");
                    strSql_add_sj.AppendLine("           ,[vcHostIp]");
                    strSql_add_sj.AppendLine("           ,[packingcondition]");
                    strSql_add_sj.AppendLine("           ,[vcPackingPlant])");
                    strSql_add_sj.AppendLine("select top(1)'S2' AS vcZYType,");
                    strSql_add_sj.AppendLine("vcBZPlant,");
                    strSql_add_sj.AppendLine("vcInputNo,");
                    strSql_add_sj.AppendLine("vcKBOrderNo,");
                    strSql_add_sj.AppendLine("vcKBLFNo,");
                    strSql_add_sj.AppendLine("vcPart_id,");
                    strSql_add_sj.AppendLine("vcIOType,");
                    strSql_add_sj.AppendLine("vcSupplier_id as vcSupplierId,");
                    strSql_add_sj.AppendLine("vcSupplierGQ as vcSupplierPlant,");
                    strSql_add_sj.AppendLine("@Start,");
                    strSql_add_sj.AppendLine("getdate(),");
                    strSql_add_sj.AppendLine("@Quantity,");
                    strSql_add_sj.AppendLine("vcBZUnit,");
                    strSql_add_sj.AppendLine("vcSHF,");
                    strSql_add_sj.AppendLine("vcSR,");
                    strSql_add_sj.AppendLine("null as vcBoxNo,");
                    strSql_add_sj.AppendLine("@PointName,");
                    strSql_add_sj.AppendLine("case when vcZYType='S0' THEN '免检' else vcCheckType end as vcCheckType,");
                    strSql_add_sj.AppendLine("case when vcZYType='S0' THEN '0' else iCheckNum end as iCheckNum,");
                    strSql_add_sj.AppendLine("case when vcZYType='S0' THEN 'OK' else vcCheckStatus end as vcCheckStatus,");
                    strSql_add_sj.AppendLine("vcLabelStart,");
                    strSql_add_sj.AppendLine("vcLabelEnd,");
                    strSql_add_sj.AppendLine("null,");
                    strSql_add_sj.AppendLine("null,");
                    strSql_add_sj.AppendLine("null,");
                    strSql_add_sj.AppendLine("@OperId,");
                    strSql_add_sj.AppendLine("getdate(),");
                    strSql_add_sj.AppendLine("@IP,");
                    strSql_add_sj.AppendLine("1,");
                    strSql_add_sj.AppendLine("vcPackingPlant from");
                    strSql_add_sj.AppendLine("TOperateSJ where vcPart_id=@Part_id and vcKBOrderNo=@KBOrderNo and vcKBLFNo=@KBLFNo and vcSR=@SR and vcZYType in ('S0','S1')");
                    strSql_add_sj.AppendLine("order by iAutoId desc");

                    sqlCommand_add_sj.CommandText = strSql_add_sj.ToString();
                    sqlCommand_add_sj.Parameters.AddWithValue("@Start", scanTime);
                    sqlCommand_add_sj.Parameters.AddWithValue("@Quantity", packQuantity);
                    sqlCommand_add_sj.Parameters.AddWithValue("@PointName", strPointName);
                    sqlCommand_add_sj.Parameters.AddWithValue("@OperId", strOperId);
                    sqlCommand_add_sj.Parameters.AddWithValue("@IP", strIP);
                    sqlCommand_add_sj.Parameters.AddWithValue("@Part_id", partId);
                    sqlCommand_add_sj.Parameters.AddWithValue("@KBOrderNo", kanbanOrderNo);
                    sqlCommand_add_sj.Parameters.AddWithValue("@KBLFNo", kanbanSerial);
                    sqlCommand_add_sj.Parameters.AddWithValue("@SR", dock);
                    #endregion
                    sqlCommand_add_sj.ExecuteNonQuery();
                    #endregion

                    #region 3.更新入出库履历TOperateSJ_InOutput    sqlCommand_mod_io
                    SqlCommand sqlCommand_mod_io = sqlConnection.CreateCommand();
                    sqlCommand_mod_io.Transaction = sqlTransaction;
                    sqlCommand_mod_io.CommandType = CommandType.Text;
                    StringBuilder strSql_mod_io = new StringBuilder();

                    #region SQL and Parameters
                    strSql_mod_io.AppendLine("update TOperateSJ_InOutput set iDBZ=iDBZ-@Quantity,iDZX=iDZX+@Quantity,vcOperatorID='"+ strOperId + "',dOperatorTime=GETDATE() ");
                    strSql_mod_io.AppendLine("where vcPart_id=@Part_id and vcKBOrderNo=@KBOrderNo and vcKBLFNo=@KBLFNo and vcSR=@SR");
                    sqlCommand_mod_io.CommandText = strSql_mod_io.ToString();
                    sqlCommand_mod_io.Parameters.AddWithValue("@Quantity", packQuantity);
                    sqlCommand_mod_io.Parameters.AddWithValue("@Part_id", partId);
                    sqlCommand_mod_io.Parameters.AddWithValue("@KBOrderNo", kanbanOrderNo);
                    sqlCommand_mod_io.Parameters.AddWithValue("@KBLFNo", kanbanSerial);
                    sqlCommand_mod_io.Parameters.AddWithValue("@SR", dock);
                    #endregion
                    sqlCommand_mod_io.ExecuteNonQuery();

                    #endregion

                    #region 4.插入包材履历TPackWork-TPackZaiKu   sqlCommand_add_pw
                    SqlCommand sqlCommand_add_pw = sqlConnection.CreateCommand();
                    sqlCommand_add_pw.Transaction = sqlTransaction;
                    sqlCommand_add_pw.CommandType = CommandType.Text;
                    StringBuilder strSql_add_pw = new StringBuilder();

                    #region SQL and Parameters
                    strSql_add_pw.AppendLine("INSERT INTO [dbo].[TPackWork]");
                    strSql_add_pw.AppendLine("           ([vcZuoYeQuFen]");
                    strSql_add_pw.AppendLine("           ,[vcOrderNo]");
                    strSql_add_pw.AppendLine("           ,[vcPackNo]");
                    strSql_add_pw.AppendLine("           ,[vcPackGPSNo]");
                    strSql_add_pw.AppendLine("           ,[vcSupplierID]");
                    strSql_add_pw.AppendLine("           ,[vcPackSpot]");
                    strSql_add_pw.AppendLine("           ,[iNumber]");
                    strSql_add_pw.AppendLine("           ,[dBuJiTime]");
                    strSql_add_pw.AppendLine("           ,[vcOperatorID]");
                    strSql_add_pw.AppendLine("           ,[dOperatorTime])");
                    strSql_add_pw.AppendLine("     VALUES");
                    strSql_add_pw.AppendLine("           (@vcZuoYeQuFen");
                    strSql_add_pw.AppendLine("           ,@vcOrderNo");
                    strSql_add_pw.AppendLine("           ,@vcPackNo");
                    strSql_add_pw.AppendLine("           ,@vcPackGPSNo");
                    strSql_add_pw.AppendLine("           ,@vcSupplierID");
                    strSql_add_pw.AppendLine("           ,@vcPackSpot");
                    strSql_add_pw.AppendLine("           ,@iNumber");
                    strSql_add_pw.AppendLine("           ,GETDATE()");
                    strSql_add_pw.AppendLine("           ,@OperId");
                    strSql_add_pw.AppendLine("           ,GETDATE())");
                    strSql_add_pw.AppendLine("declare @flag int ");
                    strSql_add_pw.AppendLine("set @flag=(select count(*) from TPackZaiKu where vcPackNo = @vcPackNo and vcPackGPSNo = @vcPackGPSNo and vcSupplierID = @vcSupplierID)");
                    strSql_add_pw.AppendLine("if(@flag=0)");
                    strSql_add_pw.AppendLine("begin ");
                    strSql_add_pw.AppendLine("INSERT INTO [dbo].[TPackZaiKu]");
                    strSql_add_pw.AppendLine("           ([vcPackSpot]");
                    strSql_add_pw.AppendLine("           ,[vcPackNo]");
                    strSql_add_pw.AppendLine("           ,[vcPackGPSNo]");
                    strSql_add_pw.AppendLine("           ,[vcSupplierID]");
                    strSql_add_pw.AppendLine("           ,[iLiLun]");
                    strSql_add_pw.AppendLine("           ,[iAnQuan]");
                    strSql_add_pw.AppendLine("           ,[dChange]");
                    strSql_add_pw.AppendLine("           ,[vcOperatorID]");
                    strSql_add_pw.AppendLine("           ,[dOperatorTime])");
                    strSql_add_pw.AppendLine("     VALUES");
                    strSql_add_pw.AppendLine("           (@vcPackSpot");
                    strSql_add_pw.AppendLine("           ,@vcPackNo");
                    strSql_add_pw.AppendLine("           ,@vcPackGPSNo");
                    strSql_add_pw.AppendLine("           ,@vcSupplierID");
                    strSql_add_pw.AppendLine("           ,-1*cast(@iNumber as decimal(16,5))");
                    strSql_add_pw.AppendLine("           ,0");
                    strSql_add_pw.AppendLine("           ,-1*cast(@iNumber as decimal(16,5))");
                    strSql_add_pw.AppendLine("           ,@OperId");
                    strSql_add_pw.AppendLine("           ,GETDATE())");
                    strSql_add_pw.AppendLine("end");
                    strSql_add_pw.AppendLine("else");
                    strSql_add_pw.AppendLine("begin");
                    strSql_add_pw.AppendLine("update [TPackZaiKu] set [iLiLun]=[iLiLun]-cast(@iNumber as decimal(16,5)),[dChange]=[dChange]-cast(@iNumber as decimal(16,5)) ");
                    strSql_add_pw.AppendLine("where vcPackNo = @vcPackNo and vcPackGPSNo = @vcPackGPSNo and vcSupplierID = @vcSupplierID");
                    strSql_add_pw.AppendLine("end");

                    sqlCommand_add_pw.CommandText = strSql_add_pw.ToString();
                    sqlCommand_add_pw.Parameters.AddWithValue("@vcZuoYeQuFen", "");
                    sqlCommand_add_pw.Parameters.AddWithValue("@vcOrderNo", "");
                    sqlCommand_add_pw.Parameters.AddWithValue("@vcPackNo", "");
                    sqlCommand_add_pw.Parameters.AddWithValue("@vcPackGPSNo", "");
                    sqlCommand_add_pw.Parameters.AddWithValue("@vcSupplierID", "");
                    sqlCommand_add_pw.Parameters.AddWithValue("@vcPackSpot", "");
                    sqlCommand_add_pw.Parameters.AddWithValue("@iNumber", "");
                    sqlCommand_add_pw.Parameters.AddWithValue("@OperId", "");
                    #endregion
                    foreach (DataRow item in dtPackList.Rows)
                    {
                        #region Value
                        sqlCommand_add_pw.Parameters["@vcZuoYeQuFen"].Value = item["vcZuoYeQuFen"].ToString();
                        sqlCommand_add_pw.Parameters["@vcOrderNo"].Value = item["vcOrderNo"].ToString();
                        sqlCommand_add_pw.Parameters["@vcPackNo"].Value = item["vcPackNo"].ToString();
                        sqlCommand_add_pw.Parameters["@vcPackGPSNo"].Value = item["vcPackGPSNo"].ToString();
                        sqlCommand_add_pw.Parameters["@vcSupplierID"].Value = item["vcSupplierID"].ToString();
                        sqlCommand_add_pw.Parameters["@vcPackSpot"].Value = item["vcPackSpot"].ToString();
                        sqlCommand_add_pw.Parameters["@iNumber"].Value = item["dQty"].ToString();
                        sqlCommand_add_pw.Parameters["@OperId"].Value = strOperId;
                        #endregion
                        sqlCommand_add_pw.ExecuteNonQuery();
                    }
                    #endregion
                }
                //}
                #region 写入数据库

                //提交事务
                sqlTransaction.Commit();
                sqlConnection.Close();
                return true;
                #endregion

            }
            catch (Exception ex)
            {
                //0613记录日志

                ComMessage.GetInstance().ProcessMessage("P00001", "M03UE0901", ex, "000000");
                //回滚事务
                if (sqlTransaction != null && sqlConnection != null)
                {
                    sqlTransaction.Rollback();
                    sqlConnection.Close();
                }
                return false;
            }
        }
        public DataTable getBoxMasterInfo(string caseno, string serverTime)
        {
            SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("select ");
            stringBuilder.AppendLine("	a.vcCaseNo");
            stringBuilder.AppendLine("	,a.vcBoxNo");
            stringBuilder.AppendLine("	,a.vcInstructionNo as vcInputNo");
            stringBuilder.AppendLine("	,a.vcOrderNo as vcKBOrderNo");
            stringBuilder.AppendLine("	,a.vcLianFanNo as vcKBLFNo");
            stringBuilder.AppendLine("	,a.vcPart_id as vcPart_id");
            stringBuilder.AppendLine("	,a.vcSR");
            stringBuilder.AppendLine("	,a.iQuantity as iQuantity_zx");
            stringBuilder.AppendLine("	,c.iQuantity as iQuantity_rk");
            stringBuilder.AppendLine("	,0 as iTotalcnt");
            stringBuilder.AppendLine("	,0 as iTotalpiece");
            stringBuilder.AppendLine("	,isnull(c.iDBZ,0) as iDBZ");
            stringBuilder.AppendLine("	,isnull(c.iDZX,0) as iDZX");
            stringBuilder.AppendLine("	,isnull(c.iDCH,0) as iDCH");
            stringBuilder.AppendLine("	,'S3' as vcZYType");
            stringBuilder.AppendLine("	,b.vcBZPlant");
            stringBuilder.AppendLine("	,b.vcIOType");
            stringBuilder.AppendLine("	,b.vcSupplier_id");
            stringBuilder.AppendLine("	,b.vcSupplierGQ");
            stringBuilder.AppendLine("	,b.vcBZUnit");
            stringBuilder.AppendLine("	,b.vcSHF");
            stringBuilder.AppendLine("	,case when isnull(b.vcCheckType,'')='' then '免检' else b.vcCheckType end vcCheckType");
            stringBuilder.AppendLine("	,case when isnull(b.iCheckNum,'')='' then 0 else b.iCheckNum end iCheckNum");
            stringBuilder.AppendLine("	,case when isnull(b.vcCheckStatus,'')='' then 'OK' else b.vcCheckStatus end vcCheckStatus");
            stringBuilder.AppendLine("	,b.vcLabelStart");
            stringBuilder.AppendLine("	,b.vcLabelEnd");
            stringBuilder.AppendLine("	,b.packingcondition");
            stringBuilder.AppendLine("	,b.vcPackingPlant");
            stringBuilder.AppendLine("	,d.vcPartENName");
            stringBuilder.AppendLine("	,d.vcCarfamilyCode");
            stringBuilder.AppendLine("from ");
            stringBuilder.AppendLine("(select * from TBoxMaster where vcCaseNo='" + caseno + "' and vcDelete='0' and dPrintBoxTime is null)a");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(SELECT DISTINCT [vcZYType],[vcBZPlant],[vcInputNo],[vcKBOrderNo],[vcKBLFNo],[vcPart_id],[vcIOType],[vcSupplier_id],[vcSupplierGQ],");
            stringBuilder.AppendLine("		[vcBZUnit],[vcSHF],[vcSR],[vcCheckType],[iCheckNum],[vcCheckStatus],[vcLabelStart],[vcLabelEnd],[packingcondition],[vcPackingPlant]");
            stringBuilder.AppendLine("  FROM [TOperateSJ] where [vcZYType]='S2')b");
            stringBuilder.AppendLine("on a.vcPart_id=b.vcPart_id and a.vcOrderNo=b.vcKBOrderNo and a.vcLianFanNo=b.vcKBLFNo and a.vcSR=b.vcSR");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select * from TOperateSJ_InOutput)c");
            stringBuilder.AppendLine("on a.vcPart_id=c.vcPart_id and a.vcOrderNo=c.vcKBOrderNo and a.vcLianFanNo=c.vcKBLFNo and a.vcSR=c.vcSR");
            stringBuilder.AppendLine("left join");
            stringBuilder.AppendLine("(select vcPackingPlant,vcPartId,vcSupplierId,vcReceiver,vcInOut,vcPartENName,vcCarfamilyCode,vcSupplierName,vcSupplierPlace from TSPMaster WHERE dFromTime<=GETDATE() and dToTime>=GETDATE())d");
            stringBuilder.AppendLine("on b.vcPart_id = d.vcPartId and b.[vcSHF]=d.vcReceiver and b.[vcSupplier_id]=d.vcSupplierId");
            stringBuilder.AppendLine("");
            DataSet ds = new DataSet();
            try
            {
                ConnSql.Open();
                string strSQL = stringBuilder.ToString();
                SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == ConnSql.State)
                {
                    ConnSql.Close();
                }
            }
        }
        public bool setCastListInfo(DataTable dtOperateSJ_Temp, DataTable dtCaseList_Temp, string strIP, string caseno, string boxno, string scanTime, string strOperId, string strCasePrinterName)
        {
            SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();
            sqlConnection.Open();
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            try
            {
                #region 1-2.插入作业实绩表-装箱数据、更新入出库履历-待装箱+待出荷、更新装箱表  sqlCommand_add_sj
                SqlCommand sqlCommand_add_sj = sqlConnection.CreateCommand();
                sqlCommand_add_sj.Transaction = sqlTransaction;
                sqlCommand_add_sj.CommandType = CommandType.Text;
                StringBuilder strSql_add_sj = new StringBuilder();

                #region SQL and Parameters
                strSql_add_sj.AppendLine("INSERT INTO [dbo].[TOperateSJ]");
                strSql_add_sj.AppendLine("           ([vcZYType]");
                strSql_add_sj.AppendLine("           ,[vcBZPlant]");
                strSql_add_sj.AppendLine("           ,[vcInputNo]");
                strSql_add_sj.AppendLine("           ,[vcKBOrderNo]");
                strSql_add_sj.AppendLine("           ,[vcKBLFNo]");
                strSql_add_sj.AppendLine("           ,[vcPart_id]");
                strSql_add_sj.AppendLine("           ,[vcIOType]");
                strSql_add_sj.AppendLine("           ,[vcSupplier_id]");
                strSql_add_sj.AppendLine("           ,[vcSupplierGQ]");
                strSql_add_sj.AppendLine("           ,[dStart]");
                strSql_add_sj.AppendLine("           ,[dEnd]");
                strSql_add_sj.AppendLine("           ,[iQuantity]");
                strSql_add_sj.AppendLine("           ,[vcBZUnit]");
                strSql_add_sj.AppendLine("           ,[vcSHF]");
                strSql_add_sj.AppendLine("           ,[vcSR]");
                strSql_add_sj.AppendLine("           ,[vcCaseNo]");
                strSql_add_sj.AppendLine("           ,[vcBoxNo]");
                strSql_add_sj.AppendLine("           ,[vcSheBeiNo]");
                strSql_add_sj.AppendLine("           ,[vcCheckType]");
                strSql_add_sj.AppendLine("           ,[iCheckNum]");
                strSql_add_sj.AppendLine("           ,[vcCheckStatus]");
                strSql_add_sj.AppendLine("           ,[vcLabelStart]");
                strSql_add_sj.AppendLine("           ,[vcLabelEnd]");
                strSql_add_sj.AppendLine("           ,[vcUnlocker]");
                strSql_add_sj.AppendLine("           ,[dUnlockTime]");
                strSql_add_sj.AppendLine("           ,[vcSellNo]");
                strSql_add_sj.AppendLine("           ,[vcOperatorID]");
                strSql_add_sj.AppendLine("           ,[dOperatorTime]");
                strSql_add_sj.AppendLine("           ,[vcHostIp]");
                strSql_add_sj.AppendLine("           ,[packingcondition]");
                strSql_add_sj.AppendLine("           ,[vcPackingPlant])");
                strSql_add_sj.AppendLine("     VALUES");
                strSql_add_sj.AppendLine("           (@vcZYType");
                strSql_add_sj.AppendLine("           ,@vcBZPlant");
                strSql_add_sj.AppendLine("           ,@vcInputNo");
                strSql_add_sj.AppendLine("           ,@vcKBOrderNo");
                strSql_add_sj.AppendLine("           ,@vcKBLFNo");
                strSql_add_sj.AppendLine("           ,@vcPart_id");
                strSql_add_sj.AppendLine("           ,@vcIOType");
                strSql_add_sj.AppendLine("           ,@vcSupplier_id");
                strSql_add_sj.AppendLine("           ,@vcSupplierGQ");
                strSql_add_sj.AppendLine("           ,@dStart");
                strSql_add_sj.AppendLine("           ,GETDATE()");
                strSql_add_sj.AppendLine("           ,@iQuantity");
                strSql_add_sj.AppendLine("           ,@vcBZUnit");
                strSql_add_sj.AppendLine("           ,@vcSHF");
                strSql_add_sj.AppendLine("           ,@vcSR");
                strSql_add_sj.AppendLine("           ,@vcCaseNo");
                strSql_add_sj.AppendLine("           ,@vcBoxNo");
                strSql_add_sj.AppendLine("           ,@vcSheBeiNo");
                strSql_add_sj.AppendLine("           ,@vcCheckType");
                strSql_add_sj.AppendLine("           ,@iCheckNum");
                strSql_add_sj.AppendLine("           ,@vcCheckStatus");
                strSql_add_sj.AppendLine("           ,@vcLabelStart");
                strSql_add_sj.AppendLine("           ,@vcLabelEnd");
                strSql_add_sj.AppendLine("           ,null");
                strSql_add_sj.AppendLine("           ,null");
                strSql_add_sj.AppendLine("           ,null");
                strSql_add_sj.AppendLine("           ,@vcOperatorID");
                strSql_add_sj.AppendLine("           ,GETDATE()");
                strSql_add_sj.AppendLine("           ,@vcHostIp");
                strSql_add_sj.AppendLine("           ,@packingcondition");
                strSql_add_sj.AppendLine("           ,@vcPackingPlant)");
                strSql_add_sj.AppendLine("update TOperateSJ_InOutput set iDZX = iDZX - cast(@iQuantity as int), iDCH = iDCH +  cast(@iQuantity as int),vcOperatorID=@vcOperatorID,dOperatorTime=GETDATE()");
                strSql_add_sj.AppendLine("where vcPart_id=@vcPart_id and vcKBOrderNo=@vcKBOrderNo and vcKBLFNo=@vcKBLFNo and vcSR=@vcSR");
                strSql_add_sj.AppendLine("update TBoxMaster set dPrintBoxTime=GETDATE() ");
                strSql_add_sj.AppendLine("where vcCaseNo=@vcCaseNo and vcPart_id=@vcPart_id and vcOrderNo=@vcKBOrderNo and vcLianFanNo=@vcKBLFNo and vcSR=@vcSR and vcDelete='0'");

                sqlCommand_add_sj.CommandText = strSql_add_sj.ToString();
                sqlCommand_add_sj.Parameters.AddWithValue("@vcZYType", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcBZPlant", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcInputNo", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcKBOrderNo", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcKBLFNo", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcPart_id", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcIOType", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcSupplier_id", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcSupplierGQ", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@dStart", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@iQuantity", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcBZUnit", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcSHF", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcSR", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcCaseNo", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcBoxNo", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcSheBeiNo", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcCheckType", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@iCheckNum", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcCheckStatus", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcLabelStart", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcLabelEnd", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcOperatorID", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcHostIp", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@packingcondition", "");
                sqlCommand_add_sj.Parameters.AddWithValue("@vcPackingPlant", "");
                #endregion
                foreach (DataRow item in dtOperateSJ_Temp.Rows)
                {
                    #region Value
                    sqlCommand_add_sj.Parameters["@vcZYType"].Value = item["vcZYType"].ToString();
                    sqlCommand_add_sj.Parameters["@vcBZPlant"].Value = item["vcBZPlant"].ToString();
                    sqlCommand_add_sj.Parameters["@vcInputNo"].Value = item["vcInputNo"].ToString();
                    sqlCommand_add_sj.Parameters["@vcKBOrderNo"].Value = item["vcKBOrderNo"].ToString();
                    sqlCommand_add_sj.Parameters["@vcKBLFNo"].Value = item["vcKBLFNo"].ToString();
                    sqlCommand_add_sj.Parameters["@vcPart_id"].Value = item["vcPart_id"].ToString();
                    sqlCommand_add_sj.Parameters["@vcIOType"].Value = item["vcIOType"].ToString();
                    sqlCommand_add_sj.Parameters["@vcSupplier_id"].Value = item["vcSupplier_id"].ToString();
                    sqlCommand_add_sj.Parameters["@vcSupplierGQ"].Value = item["vcSupplierGQ"].ToString();
                    sqlCommand_add_sj.Parameters["@dStart"].Value = item["dStart"].ToString();
                    sqlCommand_add_sj.Parameters["@iQuantity"].Value = item["iQuantity"].ToString();
                    sqlCommand_add_sj.Parameters["@vcBZUnit"].Value = item["vcBZUnit"].ToString();
                    sqlCommand_add_sj.Parameters["@vcSHF"].Value = item["vcSHF"].ToString();
                    sqlCommand_add_sj.Parameters["@vcSR"].Value = item["vcSR"].ToString();
                    sqlCommand_add_sj.Parameters["@vcCaseNo"].Value = item["vcCaseNo"].ToString();
                    sqlCommand_add_sj.Parameters["@vcBoxNo"].Value = item["vcBoxNo"].ToString();
                    sqlCommand_add_sj.Parameters["@vcSheBeiNo"].Value = item["vcSheBeiNo"].ToString();
                    sqlCommand_add_sj.Parameters["@vcCheckType"].Value = item["vcCheckType"].ToString();
                    sqlCommand_add_sj.Parameters["@iCheckNum"].Value = item["iCheckNum"].ToString();
                    sqlCommand_add_sj.Parameters["@vcCheckStatus"].Value = item["vcCheckStatus"].ToString();
                    sqlCommand_add_sj.Parameters["@vcLabelStart"].Value = item["vcLabelStart"].ToString();
                    sqlCommand_add_sj.Parameters["@vcLabelEnd"].Value = item["vcLabelEnd"].ToString();
                    sqlCommand_add_sj.Parameters["@vcOperatorID"].Value = item["vcOperatorID"].ToString();
                    sqlCommand_add_sj.Parameters["@vcHostIp"].Value = item["vcHostIp"].ToString();
                    sqlCommand_add_sj.Parameters["@packingcondition"].Value = item["packingcondition"].ToString();
                    sqlCommand_add_sj.Parameters["@vcPackingPlant"].Value = item["vcPackingPlant"].ToString();
                    #endregion
                    sqlCommand_add_sj.ExecuteNonQuery();
                }
                #endregion

                #region 3.插入装箱单表   sqlCommand_add_cl
                SqlCommand sqlCommand_add_cl = sqlConnection.CreateCommand();
                sqlCommand_add_cl.Transaction = sqlTransaction;
                sqlCommand_add_cl.CommandType = CommandType.Text;
                StringBuilder strSql_add_cl = new StringBuilder();

                #region SQL and Parameters
                strSql_add_cl.AppendLine("INSERT INTO [dbo].[TCaseList]");
                strSql_add_cl.AppendLine("           ([vcCpdcode]");
                strSql_add_cl.AppendLine("           ,[vcCpdname]");
                strSql_add_cl.AppendLine("           ,[vcCpdaddress]");
                strSql_add_cl.AppendLine("           ,[vcCasenoIntact]");
                strSql_add_cl.AppendLine("           ,[vcCaseno]");
                strSql_add_cl.AppendLine("           ,[vcCasebarcode]");
                strSql_add_cl.AppendLine("           ,[iNo]");
                strSql_add_cl.AppendLine("           ,[vcInno]");
                strSql_add_cl.AppendLine("           ,[vcPart_id]");
                strSql_add_cl.AppendLine("           ,[vcPartsname]");
                strSql_add_cl.AppendLine("           ,[iQty]");
                strSql_add_cl.AppendLine("           ,[iTotalcnt]");
                strSql_add_cl.AppendLine("           ,[iTotalpiece]");
                strSql_add_cl.AppendLine("           ,[vcPcname]");
                strSql_add_cl.AppendLine("           ,[vcHostip]");
                strSql_add_cl.AppendLine("           ,[iDatamatrixcode]");
                strSql_add_cl.AppendLine("           ,[vcOperatorID]");
                strSql_add_cl.AppendLine("           ,[dOperatorTime]");
                strSql_add_cl.AppendLine("           ,[dFirstPrintTime]");
                strSql_add_cl.AppendLine("           ,[dLatelyPrintTime]");
                strSql_add_cl.AppendLine("           ,[vcLabelStart]");
                strSql_add_cl.AppendLine("           ,[vcLabelEnd])");
                strSql_add_cl.AppendLine("     VALUES");
                strSql_add_cl.AppendLine("           (@vcCpdcode");
                strSql_add_cl.AppendLine("           ,@vcCpdname");
                strSql_add_cl.AppendLine("           ,@vcCpdaddress");
                strSql_add_cl.AppendLine("           ,null");
                strSql_add_cl.AppendLine("           ,@vcCaseno");
                strSql_add_cl.AppendLine("           ,@vcCasebarcode");
                strSql_add_cl.AppendLine("           ,null");
                strSql_add_cl.AppendLine("           ,@vcInno");
                strSql_add_cl.AppendLine("           ,@vcPart_id");
                strSql_add_cl.AppendLine("           ,@vcPartsname");
                strSql_add_cl.AppendLine("           ,@iQty");
                strSql_add_cl.AppendLine("           ,@iTotalcnt");
                strSql_add_cl.AppendLine("           ,@iTotalpiece");
                strSql_add_cl.AppendLine("           ,@vcPcname");
                strSql_add_cl.AppendLine("           ,@vcHostip");
                strSql_add_cl.AppendLine("           ,@iDatamatrixcode");
                strSql_add_cl.AppendLine("           ,@vcOperatorID");
                strSql_add_cl.AppendLine("           ,GETDATE()");
                strSql_add_cl.AppendLine("           ,null");
                strSql_add_cl.AppendLine("           ,null");
                strSql_add_cl.AppendLine("           ,@vcLabelStart");
                strSql_add_cl.AppendLine("           ,@vcLabelEnd)");

                sqlCommand_add_cl.CommandText = strSql_add_cl.ToString();
                sqlCommand_add_cl.Parameters.AddWithValue("@vcCpdcode", "");
                sqlCommand_add_cl.Parameters.AddWithValue("@vcCpdname", "");
                sqlCommand_add_cl.Parameters.AddWithValue("@vcCpdaddress", "");
                sqlCommand_add_cl.Parameters.AddWithValue("@vcCaseno", "");
                sqlCommand_add_cl.Parameters.AddWithValue("@vcCasebarcode", "");
                sqlCommand_add_cl.Parameters.AddWithValue("@vcInno", "");
                sqlCommand_add_cl.Parameters.AddWithValue("@vcPart_id", "");
                sqlCommand_add_cl.Parameters.AddWithValue("@vcPartsname", "");
                sqlCommand_add_cl.Parameters.AddWithValue("@iQty", "");
                sqlCommand_add_cl.Parameters.AddWithValue("@iTotalcnt", "");
                sqlCommand_add_cl.Parameters.AddWithValue("@iTotalpiece", "");
                sqlCommand_add_cl.Parameters.AddWithValue("@vcPcname", "");
                sqlCommand_add_cl.Parameters.AddWithValue("@vcHostip", "");
                sqlCommand_add_cl.Parameters.Add("@iDatamatrixcode", SqlDbType.Image);
                sqlCommand_add_cl.Parameters.AddWithValue("@vcOperatorID", "");
                sqlCommand_add_cl.Parameters.AddWithValue("@vcLabelStart", "");
                sqlCommand_add_cl.Parameters.AddWithValue("@vcLabelEnd", "");
                #endregion
                foreach (DataRow item in dtCaseList_Temp.Rows)
                {
                    #region Value
                    sqlCommand_add_cl.Parameters["@vcCpdcode"].Value = item["vcCpdcode"].ToString();
                    sqlCommand_add_cl.Parameters["@vcCpdname"].Value = item["vcCpdname"].ToString();
                    sqlCommand_add_cl.Parameters["@vcCpdaddress"].Value = item["vcCpdaddress"].ToString();
                    sqlCommand_add_cl.Parameters["@vcCaseno"].Value = item["vcCaseno"].ToString();
                    sqlCommand_add_cl.Parameters["@vcCasebarcode"].Value = item["vcCasebarcode"].ToString();
                    sqlCommand_add_cl.Parameters["@vcInno"].Value = item["vcInno"].ToString();
                    sqlCommand_add_cl.Parameters["@vcPart_id"].Value = item["vcPart_id"].ToString();
                    sqlCommand_add_cl.Parameters["@vcPartsname"].Value = item["vcPartsname"].ToString();
                    sqlCommand_add_cl.Parameters["@iQty"].Value = item["iQty"].ToString();
                    sqlCommand_add_cl.Parameters["@iTotalcnt"].Value = item["iTotalcnt"].ToString();
                    sqlCommand_add_cl.Parameters["@iTotalpiece"].Value = item["iTotalpiece"].ToString();
                    sqlCommand_add_cl.Parameters["@vcPcname"].Value = item["vcPcname"].ToString();
                    sqlCommand_add_cl.Parameters["@vcHostip"].Value = item["vcHostip"].ToString();
                    sqlCommand_add_cl.Parameters["@iDatamatrixcode"].Value = item["iDatamatrixcode"];
                    sqlCommand_add_cl.Parameters["@vcOperatorID"].Value = item["vcOperatorID"].ToString();
                    sqlCommand_add_cl.Parameters["@vcLabelStart"].Value = item["vcLabelStart"].ToString();
                    sqlCommand_add_cl.Parameters["@vcLabelEnd"].Value = item["vcLabelEnd"].ToString();
                    #endregion
                    sqlCommand_add_cl.ExecuteNonQuery();
                }
                #endregion

                #region 4-5.更新箱号使用情况表、插入装箱单打印表    sqlCommand_mod_pt
                SqlCommand sqlCommand_mod_pt = sqlConnection.CreateCommand();
                sqlCommand_mod_pt.Transaction = sqlTransaction;
                sqlCommand_mod_pt.CommandType = CommandType.Text;
                StringBuilder strSql_mod_pt = new StringBuilder();

                #region SQL and Parameters
                strSql_mod_pt.AppendLine("INSERT INTO [dbo].[TPrint_Temp]");
                strSql_mod_pt.AppendLine("           ([vcTableName]");
                strSql_mod_pt.AppendLine("           ,[vcReportName]");
                strSql_mod_pt.AppendLine("           ,[vcClientIP]");
                strSql_mod_pt.AppendLine("           ,[vcPrintName]");
                strSql_mod_pt.AppendLine("           ,[vcKind]");
                strSql_mod_pt.AppendLine("           ,[vcOperatorID]");
                strSql_mod_pt.AppendLine("           ,[dOperatorTime]");
                strSql_mod_pt.AppendLine("           ,[vcCaseNo]");
                strSql_mod_pt.AppendLine("           ,[vcSellNo]");
                strSql_mod_pt.AppendLine("           ,[vcLotid]");
                strSql_mod_pt.AppendLine("           ,[vcSupplierId]");
                strSql_mod_pt.AppendLine("           ,[vcInno]");
                strSql_mod_pt.AppendLine("           ,[vcFlag])");
                strSql_mod_pt.AppendLine("     VALUES");
                strSql_mod_pt.AppendLine("           ('TCaseList'");
                strSql_mod_pt.AppendLine("           ,'SPR09PACP'");
                strSql_mod_pt.AppendLine("           ,@IP");
                strSql_mod_pt.AppendLine("           ,'" + strCasePrinterName + "'");
                strSql_mod_pt.AppendLine("           ,'3'");
                strSql_mod_pt.AppendLine("           ,@vcOperatorID");
                strSql_mod_pt.AppendLine("           ,GETDATE()");
                strSql_mod_pt.AppendLine("           ,@vcCaseNo");
                strSql_mod_pt.AppendLine("           ,null");
                strSql_mod_pt.AppendLine("           ,null");
                strSql_mod_pt.AppendLine("           ,null");
                strSql_mod_pt.AppendLine("           ,null");
                strSql_mod_pt.AppendLine("           ,'0')");
                strSql_mod_pt.AppendLine("update TCaseInfo set dBoxPrintTime=GETDATE(),vcOperatorID=@vcOperatorID,dOperatorTime=GETDATE()");
                strSql_mod_pt.AppendLine("where vcCaseNo=@vcCaseNo and dBoxPrintTime is null");
                sqlCommand_mod_pt.CommandText = strSql_mod_pt.ToString();
                sqlCommand_mod_pt.Parameters.AddWithValue("@IP", strIP);
                sqlCommand_mod_pt.Parameters.AddWithValue("@vcOperatorID", strOperId);
                sqlCommand_mod_pt.Parameters.AddWithValue("@vcCaseNo", caseno);
                #endregion
                sqlCommand_mod_pt.ExecuteNonQuery();

                #endregion

                #region 写入数据库
                //提交事务
                sqlTransaction.Commit();
                sqlConnection.Close();
                return true;
                #endregion

            }
            catch (Exception ex)
            {
                //0613记录日志

                ComMessage.GetInstance().ProcessMessage("P00001", "M03UE0901", ex, "000000");
                //回滚事务
                if (sqlTransaction != null && sqlConnection != null)
                {
                    sqlTransaction.Rollback();
                    sqlConnection.Close();
                }
                return false;
            }
        }


    }
}
