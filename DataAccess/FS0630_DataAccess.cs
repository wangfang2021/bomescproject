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
    public class FS0630_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 检索
        public DataTable Search(string vcDXYM, string vcPlant, string vcCLYM)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select t1.vcCLYM,LEFT(vcCLYM,4)+'/'+RIGHT(vcCLYM,2) as vcCLYMFormat,t1.vcPlant,t1.iTimes,        \n");
                strSql.Append("t1.vcDXYM,LEFT(vcDXYM,4)+'/'+RIGHT(vcDXYM,2) as vcDXYMFormat,        \n");
                strSql.Append("t2.vcName as vcPlantName,t1.dRequestTime,case when t1.vcStatus='C' then t1.dWCTime else null end as dWCTime,t1.vcStatus,     \n");
                strSql.Append("case t1.vcStatus when 'C' then '处理完成' when 'P' then '处理中' when 'E' then '处理失败' else t1.vcStatus end as vcStatusName    \n");
                strSql.Append("from (    \n");
                strSql.Append("	select * from VI_NQCStatus    \n");
                strSql.Append("	where 1=1      \n");
                if (vcCLYM != "" && vcCLYM != null)
                    strSql.Append("and vcCLYM = '" + vcCLYM + "'   \n");
                if (vcPlant != "" && vcPlant != null)
                    strSql.Append("and vcPlant = '" + vcPlant + "'  \n");
                if (vcDXYM != "" && vcDXYM != null)
                    strSql.Append("and vcDXYM = '" + vcDXYM + "'   \n");
                strSql.Append(")t1    \n");
                strSql.Append("left join (select vcValue,vcName from TCode where vcCodeId='C000') t2 on t1.vcPlant=t2.vcValue    \n");
                strSql.Append("order by t1.vcCLYM,t1.vcPlant,t1.vcDXYM,t1.iTimes    \n");
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
                string sql = "select MAX(iTimes)+1 as iTimes from TNQCStatus where vcCLYM='" + vcCLYM + "' and vcPlant='" + strPlant + "' ";
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
        public void CreateView(string vcCLYM, List<string> plantList, List<string> lsdxym, string strUserId)
        {
            SqlConnection conn_sql = Common.ComConnectionHelper.CreateSqlConnection();
            Common.ComConnectionHelper.OpenConection_SQL(ref conn_sql);
            SqlTransaction st = conn_sql.BeginTransaction();
            try
            {
                //记录请求状态
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < plantList.Count; i++)
                {
                    string strPlant = plantList[i].ToString();
                    string strMaxTimes = GetMaxTimes(strPlant, vcCLYM);
                    for (int j = 0; j < lsdxym.Count; j++)
                    {
                        string strdxym = lsdxym[j].ToString();
                        sql.Append("insert into TNQCStatus (vcCLYM,vcPlant,vcDXYM,vcStatus,iTimes,dRequestTime,vcOperatorID,dOperatorTime) values     \n");
                        sql.Append("('" + vcCLYM + "','" + strPlant + "','" + strdxym + "','已请求'," + strMaxTimes + ",getdate(),'" + strUserId + "',getdate())    \n");
                    }
                }
                SqlCommand cd0 = new SqlCommand(sql.ToString(), conn_sql, st);
                cd0.CommandType = System.Data.CommandType.Text;
                cd0.Transaction = st;
                cd0.ExecuteNonQuery();
                //删视图
                //string strViewName = "VI_NQCRequest";
                //DataTable dt = excute.ExcuteSqlWithSelectToDT("select top 1 * from sysObjects where Id=OBJECT_ID(N'" + strViewName + "') and xtype='V'");
                //if (dt.Rows.Count == 1)
                //{
                //    SqlCommand cd1 = new SqlCommand("DROP VIEW [" + strViewName + "]", conn_sql, st);
                //    cd1.CommandType = System.Data.CommandType.Text;
                //    cd1.Transaction = st;
                //    cd1.ExecuteNonQuery();
                //}
                //创建视图
                //string strplants = "";
                //for(int i=0;i<plantList.Count;i++)
                //{
                //    strplants += plantList[i].ToString() + ',';
                //}
                //strplants = strplants.Substring(0, strplants.Length-1);
                //string strmonths = "";
                //for(int i=0;i<lsdxym.Count;i++)
                //{
                //    strmonths += "'" + lsdxym[i].ToString() + "',";
                //}
                //strmonths = strmonths.Substring(0, strmonths.Length - 1);
                //sql = new StringBuilder();
                //sql.Append("Create VIEW [" + strViewName + "]  \n");
                //sql.Append("AS  \n");
                //sql.Append("select '"+vcCLYM+"' as vcCLYM,iFZGC,TARGETMONTH,t2.iTimes,PARTSNO,RIGHT(PARTSNO,2) as vcType,    \n");
                //sql.Append("D1,D2,D3,D4,D5,D6,D7,D8,D9,D10,D11,D12,D13,D14,D15,D16,D17,D18,D19,D20,D21,D22,    \n");
                //sql.Append("D23,D24,D25,D26,D27,D28,D29,D30,D31    \n");
                //sql.Append("from TSOQReply t1    \n");
                //sql.Append("left join     \n");
                //sql.Append("(    \n");
                //sql.Append("	select vcPlant,MAX(iTimes) as iTimes from TNQCStatus where vcCLYM='"+vcCLYM+"'    \n");
                //sql.Append("	group by vcPlant    \n");
                //sql.Append(")t2 on t1.iFZGC=t2.vcPlant    \n");
                //sql.Append("where t1.iFZGC in ("+strplants+")     \n");
                //sql.Append("and t1.TARGETMONTH in ("+strmonths+")    \n");
                //SqlCommand cd2 = new SqlCommand(sql.ToString(), conn_sql, st);
                //cd2.CommandType = System.Data.CommandType.Text;
                //cd2.Transaction = st;
                //cd2.ExecuteNonQuery();
                ////设置视图权限
                //SqlCommand cd3 = new SqlCommand("GRANT SELECT  ON " + strViewName + " TO seeview", conn_sql, st);//seeview是视图名称
                //cd3.CommandType = System.Data.CommandType.Text;
                //cd3.Transaction = st;
                //cd3.ExecuteNonQuery();

                st.Commit();
            }
            catch (Exception ex)
            {
                st.Rollback();
                throw ex;
            }
        }
        #endregion

        #region 取soq结果
        public DataTable GetSOQReply(string vcCLYM,string kind)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select vcCLYM,vcDXYM,vcFZGC,COUNT(1) as num from TSOQReply     \n");
                sql.Append("where vcCLYM='"+vcCLYM+"' and vcInOutFlag='"+kind+"' and (isnull(vcZhanKaiID,'')!='' or dZhanKaiTime is not null)    \n");
                sql.Append("group by vcCLYM,vcDXYM,vcFZGC    \n");
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
                sql.Append("select * from VI_NQCStatus where vcCLYM='"+vcCLYM+"'    \n");
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
