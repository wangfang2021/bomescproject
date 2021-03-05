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
    public class FS0612_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 检索_FORECAST
        public DataTable Search(string vcCLYM)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select distinct t1.vcCLYM,LEFT(vcCLYM,4)+'/'+RIGHT(vcCLYM,2) as vcCLYMFormat,t1.iTimes,    \n");
                strSql.Append("t1.dRequestTime,case when t1.vcStatus='C' then t1.dWCTime else null end as dWCTime,t1.vcStatus,     \n");
                strSql.Append("case t1.vcStatus when 'C' then '处理完成' when 'P' then '处理中' when 'E' then '处理失败' else t1.vcStatus end as vcStatusName    \n");
                strSql.Append("from (    \n");
                strSql.Append("	select * from VI_NQCStatus_HS_FORECAST where 1=1     \n");
                if (vcCLYM != "" && vcCLYM != null)
                    strSql.Append("and vcCLYM= '" + vcCLYM + "'   \n");
                strSql.Append(")t1    \n");
                strSql.Append("order by t1.vcCLYM,t1.iTimes    \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 检索_EKANBAN
        public DataTable Search2(string vcCLYM)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select distinct t1.vcCLYM,LEFT(vcCLYM,4)+'/'+RIGHT(vcCLYM,2) as vcCLYMFormat,t1.vcPlant,t2.vcName as vcPlantName,    \n");
                strSql.Append("t1.iTimes,t1.dRequestTime,case when t1.vcStatus='C' then t1.dWCTime else null end as dWCTime,t1.vcStatus,         \n");
                strSql.Append("case t1.vcStatus when 'C' then '处理完成' when 'P' then '处理中' when 'E' then '处理失败' else t1.vcStatus end as vcStatusName    \n");
                strSql.Append("from (        \n");
                strSql.Append("	select * from VI_NQCStatus_HS_EKANBAN where 1=1         \n");
                if (vcCLYM != "" && vcCLYM != null)
                    strSql.Append("and vcCLYM = '" + vcCLYM + "'   \n");
                strSql.Append(")t1        \n");
                strSql.Append("left join (select vcValue,vcName from tcode where vcCodeId='C000')t2 on t1.vcPlant=t2.vcValue    \n");
                strSql.Append("order by t1.vcCLYM,t1.vcPlant,t1.iTimes        \n");
               

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 返回最大次数+1
        public string GetMaxTimes(string strPlant, string vcCLYM)
        {
            try
            {
                string sql = "select MAX(iTimes)+1 as iTimes from VI_NQCStatus_HS_FORECAST where vcCLYM='" + vcCLYM + "' and vcPlant='" + strPlant + "' ";
                DataTable dt = excute.ExcuteSqlWithSelectToDT(sql);
                if (dt.Rows[0][0].ToString() != "")
                {
                    return dt.Rows[0][0].ToString();
                }
                else
                {
                    return "1";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string GetMaxTimes2(string strPlant, string vcCLYM)
        {
            try
            {
                string sql = "select MAX(iTimes)+1 as iTimes from VI_NQCStatus_HS_EKANBAN where vcCLYM='" + vcCLYM + "' and vcPlant='" + strPlant + "' ";
                DataTable dt = excute.ExcuteSqlWithSelectToDT(sql);
                if (dt.Rows[0][0].ToString() != "")
                {
                    return dt.Rows[0][0].ToString();
                }
                else
                {
                    return "1";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 记录请求时间
        public void CreateView(string vcCLYM, DataTable dtPlant, string strUserId)
        {
            SqlConnection conn_sql = Common.ComConnectionHelper.CreateSqlConnection();
            Common.ComConnectionHelper.OpenConection_SQL(ref conn_sql);
            SqlTransaction st = conn_sql.BeginTransaction();
            try
            {
                //记录请求状态
                StringBuilder sql = new StringBuilder();
                string strdate = DateTime.Now.ToString();
                for (int i = 0; i < dtPlant.Rows.Count; i++)
                {
                    string strPlant = dtPlant.Rows[i]["vcValue"].ToString();
                    string strMaxTimes = GetMaxTimes(strPlant, vcCLYM);
                    sql.Append("insert into TNQCStatus_HS_FORECAST (vcCLYM,vcPlant,vcStatus,iTimes,dRequestTime,vcOperatorID,dOperatorTime) values     \n");
                    sql.Append("('" + vcCLYM + "','" + strPlant + "','已请求'," + strMaxTimes + ",getdate(),'" + strUserId + "','"+ strdate + "')    \n");
                }
                SqlCommand cd0 = new SqlCommand(sql.ToString(), conn_sql, st);
                cd0.CommandType = System.Data.CommandType.Text;
                cd0.Transaction = st;
                cd0.ExecuteNonQuery();

                st.Commit();
            }
            catch (Exception ex)
            {
                st.Rollback();
                throw ex;
            }
        }
        public void CreateView2(string vcCLYM, List<string> plantList, string strUserId)
        {
            SqlConnection conn_sql = Common.ComConnectionHelper.CreateSqlConnection();
            Common.ComConnectionHelper.OpenConection_SQL(ref conn_sql);
            SqlTransaction st = conn_sql.BeginTransaction();
            try
            {
                //记录请求状态
                StringBuilder sql = new StringBuilder();
                string strdate = DateTime.Now.ToString();
                for (int i = 0; i < plantList.Count; i++)
                {
                    string strPlant = plantList[i];
                    string strMaxTimes = GetMaxTimes2(strPlant, vcCLYM);
                    sql.Append("insert into TNQCStatus_HS_EKANBAN (vcCLYM,vcPlant,vcStatus,iTimes,dRequestTime,vcOperatorID,dOperatorTime) values     \n");
                    sql.Append("('" + vcCLYM + "','" + strPlant + "','已请求'," + strMaxTimes + ",getdate(),'" + strUserId + "','" + strdate + "')    \n");
                }
                SqlCommand cd0 = new SqlCommand(sql.ToString(), conn_sql, st);
                cd0.CommandType = System.Data.CommandType.Text;
                cd0.Transaction = st;
                cd0.ExecuteNonQuery();

                st.Commit();
            }
            catch (Exception ex)
            {
                st.Rollback();
                throw ex;
            }
        }
        #endregion

        #region 取NQC处理结果
        public DataTable dtNQCReceive(string vcCLYM)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select Process_YYYYMM,Start_date_for_daily_qty,Process_Factory,COUNT(1) as num from TNQCReceiveInfo    \n");
                sql.Append("where Process_YYYYMM='"+vcCLYM+"'    \n");
                sql.Append("group by Process_YYYYMM,Start_date_for_daily_qty,Process_Factory    \n");
                return excute.ExcuteSqlWithSelectToDT(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 取最大次数的内制结果
        public DataTable GetMaxCLResult(string vcCLYM)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select * from VI_NQCStatus_HS_FORECAST where vcCLYM='" + vcCLYM + "'    \n");
                return excute.ExcuteSqlWithSelectToDT(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetMaxCLResult2(string vcCLYM)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select * from VI_NQCStatus_HS_EKANBAN where vcCLYM='" + vcCLYM + "'    \n");
                return excute.ExcuteSqlWithSelectToDT(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
