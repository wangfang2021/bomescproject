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

        public DataTable getNowBanZhiInfo()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select * from (");
                strSql.AppendLine("select convert(varchar(10),dateadd(DAY,-1,GETDATE()),23) as dHosDate,vcBanZhi,convert(varchar(10),dateadd(DAY,-1,GETDATE()),23)+' '+convert(varchar(10),tFromTime,24) as tFromTime,case when vcCross='1' then convert(varchar(10),dateadd(day,1,dateadd(DAY,-1,GETDATE())),23) else convert(varchar(10),dateadd(DAY,-1,GETDATE()),23) end +' '+convert(varchar(10),tToTime,24) as tToTime from TBZTime where vcBanZhi='夜'");
                strSql.AppendLine("union");
                strSql.AppendLine("select convert(varchar(10),dateadd(DAY,0,GETDATE()),23) as dHosDate,vcBanZhi,convert(varchar(10),dateadd(DAY,0,GETDATE()),23)+' '+convert(varchar(10),tFromTime,24) as tFromTime,case when vcCross='1' then convert(varchar(10),dateadd(day,1,dateadd(DAY,0,GETDATE())),23) else convert(varchar(10),dateadd(DAY,0,GETDATE()),23) end +' '+convert(varchar(10),tToTime,24) as tToTime from TBZTime where vcBanZhi='白'");
                strSql.AppendLine("union																																																											    ");
                strSql.AppendLine("select convert(varchar(10),dateadd(DAY,0,GETDATE()),23) as dHosDate,vcBanZhi,convert(varchar(10),dateadd(DAY,0,GETDATE()),23)+' '+convert(varchar(10),tFromTime,24) as tFromTime,case when vcCross='1' then convert(varchar(10),dateadd(day,1,dateadd(DAY,0,GETDATE())),23) else convert(varchar(10),dateadd(DAY,0,GETDATE()),23) end +' '+convert(varchar(10),tToTime,24) as tToTime from TBZTime where vcBanZhi='夜'");
                strSql.AppendLine("union");
                strSql.AppendLine("select convert(varchar(10),dateadd(DAY,1,GETDATE()),23) as dHosDate,vcBanZhi,convert(varchar(10),dateadd(DAY,1,GETDATE()),23)+' '+convert(varchar(10),tFromTime,24) as tFromTime,case when vcCross='1' then convert(varchar(10),dateadd(day,1,dateadd(DAY,1,GETDATE())),23) else convert(varchar(10),dateadd(DAY,1,GETDATE()),23) end +' '+convert(varchar(10),tToTime,24) as tToTime from TBZTime where vcBanZhi='白'");
                strSql.AppendLine(")t where tFromTime<=GETDATE() and tToTime>=GETDATE()");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataSet getPackingPlanInfo(string strHosDate, string strBanZhi)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("SELECT distinct '' as [uuid],'' as vcPeopleNum,'450' as vcCycleTime,'90' as vcObjective,'' as vcWorkOverTime");
                strSql.AppendLine("select '' as [LinId],'' as [uuid],vcPartItem,vcStandard,vcPackTotalNum");
                strSql.AppendLine(",'' as vcPlannedTime,'' as vcPlannedPerson,'' as vcInputPerson,'' as vcInputTime,'' as vcOverFlowTime");
                strSql.AppendLine(",'' as vcSysLander,'' as vcDiffer");
                strSql.AppendLine("from (");
                strSql.AppendLine("select isnull(t3.vcBigPM,'') as vcPartItem");
                strSql.AppendLine(",cast(isnull(t4.vcStandardTime,-1) as int) as vcStandard");
                strSql.AppendLine(",sum(cast(vcPackNum as int)) as vcPackTotalNum from ");
                strSql.AppendLine("(select [vcPartId],[vcPackNum] from [TPackingPlan] where vcPackDate='"+ strHosDate + "' and vcPackBZ='"+ strBanZhi + "')t1");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select * from [TPMSmall])t2");
                strSql.AppendLine("on substring(t1.vcPartId,1,5)=t2.[vcPartsNoBefore5]");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select * from [TPMRelation])t3");
                strSql.AppendLine("on t2.vcSmallPM=t3.vcSmallPM");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select * from TPMStandardTime)t4");
                strSql.AppendLine("on t3.vcBigPM=t4.vcBigPM");
                strSql.AppendLine("group by t3.vcBigPM,t4.vcStandardTime)TT order by vcPartItem");
                return excute.ExcuteSqlWithSelectToDS(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataSet getSearchInfo(string strHosDate, string strBanZhi)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("SELECT distinct uuid,vcPeopleNum,vcCycleTime,vcObjective,vcWorkOverTime");
                strSql.AppendLine("  FROM [dbo].[TInPutIntoOver] where dHosDate='" + strHosDate + "' and vcBanZhi='" + strBanZhi + "';");
                strSql.AppendLine("SELECT [LinId],[uuid],[vcPartItem] as vcPartItem,[vcStandard] as vcStandard,[decPackTotalNum] as vcPackTotalNum");
                strSql.AppendLine("      ,[decPlannedTime] as vcPlannedTime,[decPlannedPerson] as vcPlannedPerson,[decInputPerson] as vcInputPerson,[decInputTime] as vcInputTime,[decOverFlowTime] as vcOverFlowTime");
                strSql.AppendLine("      ,'' as vcSysLander,'' as vcDiffer");
                strSql.AppendLine("  FROM [dbo].[TInPutIntoOver] where dHosDate='"+ strHosDate + "' and vcBanZhi='"+ strBanZhi + "' order by vcPartItem;");
                return excute.ExcuteSqlWithSelectToDS(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
