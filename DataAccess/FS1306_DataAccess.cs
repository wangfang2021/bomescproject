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
    public class FS1306_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        public DataTable getDataInfo(string strPackingPlant, string strHosDate, string strBanZhi, decimal decRest_screen, decimal decRest_point, string strBZFromTime, ref DataTable dtMessage)
        {
            try
            {
                SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();
                SqlParameter[] pars = new SqlParameter[]{
                    new SqlParameter("@PackingPlant", strPackingPlant),
                    new SqlParameter("@HosDate",strHosDate),
                    new SqlParameter("@BanZhi",strBanZhi),
                    new SqlParameter("@BZFromTime",strBZFromTime),
                    new SqlParameter("@Rest_screen",decRest_screen),
                    new SqlParameter("@Rest_point",decRest_point)
                };
                string cmdText = "BSP1306_DataInfo";
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
                DataRow dataRow = dtMessage.NewRow();
                dataRow["vcMessage"] = "数据读取失败！";
                dtMessage.Rows.Add(dataRow);
                throw ex;
            }
        }
        public DataTable getDataInfo(string strPackingPlant, string strHosDate, string strBanZhi, decimal decRest, string strBZFromTime, ref DataTable dtMessage)
        {
            try
            {
                SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();
                SqlParameter[] pars = new SqlParameter[]{
                    new SqlParameter("@PackingPlant", strPackingPlant),
                    new SqlParameter("@HosDate",strHosDate),
                    new SqlParameter("@BanZhi",strBanZhi),
                    new SqlParameter("@Rest",decRest),
                    new SqlParameter("@BZFromTime",strBZFromTime)
                };
                string cmdText = "BSP1306_DataInfo";
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
                DataRow dataRow = dtMessage.NewRow();
                dataRow["vcMessage"] = "数据读取失败！";
                dtMessage.Rows.Add(dataRow);
                throw ex;
            }
        }




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
        public DataSet getOperPointInfo(string strPackPlant,string strBanZhi,string strHosDate,string strOperater,string strFromTime_nw)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine("--TBZTime_Rest_point	所在班值休息字典");
            strSql.AppendLine("select ROW_NUMBER() OVER(PARTITION BY vcPackPlant,vcBanZhi ");
            strSql.AppendLine("					ORDER BY dateadd(day,cast(vcBeforCross as int),cast('"+ strHosDate + "' as datetime))+' '+tBeforTime) AS TANK");
            strSql.AppendLine("		,vcPackPlant");
            strSql.AppendLine("		,vcBanZhi");
            strSql.AppendLine("		,dateadd(day,cast(vcBeforCross as int),cast('"+ strHosDate + "' as datetime))+' '+tBeforTime as  tBeforTime");
            strSql.AppendLine("		,dateadd(day,cast(vcLastCross as int),cast('"+ strHosDate + "' as datetime))+' '+tLastTime  as  tLastTime");
            strSql.AppendLine("		,iMinute");
            strSql.AppendLine("from TBZTime_Rest_point where vcPackPlant='"+ strPackPlant + "' and vcBanZhi='"+ strBanZhi + "'");
            strSql.AppendLine("--TPointDetails	点位登录履历");
            strSql.AppendLine("select dHosDate,vcPlant as vcPackPlant,vcBanZhi,vcPointNo,UUID,dEntryTime,case when dDestroyTime is null then GETDATE() else dDestroyTime end as dDestroyTime");
            strSql.AppendLine("from TPointDetails where vcPlant='" + strPackPlant + "' and vcBanZhi='" + strBanZhi + "' and dHosDate='" + strHosDate + "' and vcOperater='"+ strOperater + "'");
            strSql.AppendLine("order by dEntryTime");
            strSql.AppendLine("--TOperateSJ	操作人当日作业基准");
            strSql.AppendLine("SELECT ISNULL(SUM(T1.decQuantity*T2.vcStandardTime),0) AS decOperStandard FROM ");
            strSql.AppendLine("(SELECT A1.vcBZPlant,A2.vcSmallPM,sum(iQuantity/vcBZUnit) AS decQuantity  FROM ");
            strSql.AppendLine("(select vcBZPlant,vcPart_id,CAST(iQuantity AS int) AS iQuantity,CAST(vcBZUnit AS int) AS vcBZUnit ");
            strSql.AppendLine("from TOperateSJ ");
            strSql.AppendLine("where vcZYType='S2' and vcBZPlant='"+ strPackPlant + "' and vcOperatorID='"+ strOperater + "' AND dOperatorTime>='"+ strFromTime_nw + "' and dOperatorTime<=GETDATE())A1");
            strSql.AppendLine("LEFT JOIN");
            strSql.AppendLine("(select vcBZPlant,vcPart_id,vcSmallPM");
            strSql.AppendLine("from TPackageMaster");
            strSql.AppendLine("where dTimeFrom<=CONVERT(varchar(10),GETDATE(),23) AND dTimeTo>=CONVERT(varchar(10),GETDATE(),23))A2");
            strSql.AppendLine("ON A1.vcBZPlant=A2.vcBZPlant AND A1.vcPart_id=A2.vcPart_id");
            strSql.AppendLine("GROUP BY A1.vcBZPlant,A2.vcSmallPM)T1");
            strSql.AppendLine("LEFT JOIN");
            strSql.AppendLine("(SELECT * FROM TPMRelation)T2");
            strSql.AppendLine("ON T1.vcSmallPM=T2.vcSmallPM");
            return excute.ExcuteSqlWithSelectToDS(strSql.ToString());
        }
    }
}
