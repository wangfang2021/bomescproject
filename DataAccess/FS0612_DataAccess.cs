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

        #region 检索
        public DataTable Search(string vcPlant, string vcCLYM)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select t1.* from (    \n");
                strSql.Append("	select t1.vcCLYM,LEFT(vcCLYM,4)+'/'+RIGHT(vcCLYM,2) as vcCLYMFormat,t1.vcPlant,t1.iTimes,    \n");
                strSql.Append("	t2.vcName as vcPlantName,t1.vcECASTStatus,t1.vcEKANBANStatus,t1.dRequestTime,t1.dWCTime      \n");
                strSql.Append("	from TNQCStatus_NZAndWZ t1      \n");
                strSql.Append("	left join (select vcValue,vcName from TCode where vcCodeId='C000') t2 on t1.vcPlant=t2.vcValue      \n");
                strSql.Append(")t1    \n");
                strSql.Append("inner join (    \n");
                strSql.Append("	select vcCLYM,vcPlant,MAX(iTimes) as iTimes from TNQCStatus_NZAndWZ    \n");
                strSql.Append("	group by vcCLYM,vcPlant    \n");
                strSql.Append(")t2 on t1.vcCLYM=t2.vcCLYM and t1.vcPlant=t2.vcPlant and t1.iTimes=t2.iTimes   \n");
                strSql.Append("where 1=1  \n");
                if (vcCLYM != "" && vcCLYM != null)
                    strSql.Append("and isnull(t1.vcCLYM,'') like '%" + vcCLYM + "%'   \n");
                if (vcPlant != "" && vcPlant != null)
                    strSql.Append("and isnull(t1.vcPlant,'') like '%" + vcPlant + "%'  \n");
                strSql.Append("order by t1.vcCLYM,t1.vcPlant,t1.iTimes  \n");
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
                string sql = "select MAX(iTimes)+1 as iTimes from TNQCStatus_NZAndWZ where vcCLYM='" + vcCLYM + "' and vcPlant='" + strPlant + "' ";
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
        public void CreateView(string vcCLYM, List<string> plantList, string strUserId)
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
                    sql.Append("insert into TNQCStatus_NZAndWZ (vcCLYM,vcPlant,vcECASTStatus,vcEKANBANStatus,iTimes,dRequestTime,vcOperatorID,dOperatorTime) values     \n");
                    sql.Append("('" + vcCLYM + "','" + strPlant + "','已请求','已请求'," + strMaxTimes + ",getdate(),'" + strUserId + "',getdate())    \n");

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

    }
}
