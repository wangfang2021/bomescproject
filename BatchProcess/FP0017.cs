using Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
/// <summary>
/// NQC内制结果的获取
/// </summary>
namespace BatchProcess
{
    public class FP0017
    {
        private MultiExcute excute = new MultiExcute();

        #region 主方法
        public bool main(string strUserId)
        {
            string PageId = "FP0017";
            try
            {
                //批处理开始
                ComMessage.GetInstance().ProcessMessage(PageId, "M03PI0200", null, strUserId);
                DataTable dt = GetRequestData();
                if (dt.Rows.Count == 0)
                {//没有要请求的数据
                    ComMessage.GetInstance().ProcessMessage(PageId, "M03PI0201", null, strUserId);
                    return true;
                }
                DataSet dsNQC = GetNQCData(dt);
                if (dsNQC.Tables.Count == 0)
                {//没有NQC结果数据
                    ComMessage.GetInstance().ProcessMessage(PageId, "M03PI0201", null, strUserId);
                    return true;
                }
                UpdateDB(dsNQC, strUserId);
                //批处理结束
                ComMessage.GetInstance().ProcessMessage(PageId, "M03PI0201", null, strUserId);
                return true;
            }
            catch (Exception ex)
            {
                //批处理异常结束
                ComMessage.GetInstance().ProcessMessage(PageId, "M03PE0200", null, strUserId);
                throw ex;
            }
        }
        #endregion

        #region 更新数据库
        public void UpdateDB(DataSet dsNQC, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < dsNQC.Tables.Count; i++)
                {
                    DataTable dt = dsNQC.Tables[i];
                    string strTableName = dsNQC.Tables[i].TableName;//vcPlant + "_" + vcCLYM + "_" + iTimes;
                    string[] strname = strTableName.Split('_');
                    string strPlant = strname[0];
                    string strCLYM = strname[1];
                    string strTimes = strname[2];
                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        string Process_Name = dt.Rows[j]["Process_Name"].ToString();
                        string ExecuteTime = dt.Rows[j]["ExecuteTime"].ToString();
                        sql.Append("update TNQCStatus_NZAndWZ set dWCTime=nullif('"+ExecuteTime+"',''),vcOperatorID='" + strUserId + "',dOperatorTime=GETDATE()     \n");
                        if(Process_Name== "NQCR01")//这个状态前工程还没确定
                            sql.Append(",vcECASTStatus='送信成功'    \n");
                        if (Process_Name == "NQCS01")//这个状态前工程还没确定
                            sql.Append(",vcEKANBANStatus='送信成功'    \n");
                        sql.Append("where vcCLYM='" + strCLYM + "' and vcPlant=replace('" + strPlant + "','TFTM','') and iTimes=" + strTimes + "    \n");

                    }
                }
                if (sql.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(sql.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 获取NQC结果数据
        public DataSet GetNQCData(DataTable dt)
        {
            try
            {
                DataSet dsNQC = new DataSet();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string vcPlant = dt.Rows[i]["vcPlant"].ToString();
                    string vcCLYM = dt.Rows[i]["vcCLYM"].ToString();
                    string iTimes = dt.Rows[i]["iTimes"].ToString();
                    StringBuilder sql = new StringBuilder();
                    sql.Append("select * from NQCSRStatusInfo    \n");
                    sql.Append("where Process_Factory='" + vcPlant + "' and Process_YYYYMM='" + vcCLYM + "' and Process_Cycle_NO=" + iTimes + " and Process_Status='C' \n");
                    sql.Append("and Process_Name in ('NQCR01','NQCS01')   \n");//按照处理ID来获取，前工程还未确定，暂时空着
                    DataTable dtNQCStatus = NQCSearch(sql.ToString());
                    if (dtNQCStatus.Rows.Count > 0)
                    {//有处理完成的数据
                        dtNQCStatus.TableName = vcPlant + "_" + vcCLYM + "_" + iTimes;
                        dsNQC.Tables.Add(dtNQCStatus);
                    }
                }
                return dsNQC;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 取出需要请求的数据
        public DataTable GetRequestData()
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select t1.* from (    \n");
                sql.Append("	select 'TFTM'+vcPlant as vcPlant,vcCLYM,MAX(iTimes) as iTimes from TNQCStatus_NZAndWZ        \n");
                sql.Append("	where vcECASTStatus='已请求' or vcEKANBANStatus='已请求'      \n");
                sql.Append("	group by vcPlant,vcCLYM       \n");
                sql.Append(")t1    \n");
                sql.Append("inner join (    \n");
                sql.Append("	select vcCLYM,vcPlant,MAX(iTimes) as iTimes from TNQCStatus_NZAndWZ    \n");
                sql.Append("	group by vcCLYM,vcPlant    \n");
                sql.Append(")t2 on t1.vcCLYM=t2.vcCLYM and t1.vcPlant='TFTM'+t2.vcPlant and t1.iTimes=t2.iTimes    \n");
                return excute.ExcuteSqlWithSelectToDT(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region NQC系统取数据连接
        public DataTable NQCSearch(string sql)
        {
            SqlConnection conn = Common.ComConnectionHelper.CreateConnection_NQC();
            try
            {
                SqlDataAdapter da = new SqlDataAdapter();
                DataTable dt = new DataTable();
                da.SelectCommand = new SqlCommand();
                da.SelectCommand.CommandType = System.Data.CommandType.Text;
                da.SelectCommand.Connection = conn;
                da.SelectCommand.CommandText = sql;
                Common.ComConnectionHelper.OpenConection_SQL(ref conn);
                da.Fill(dt);
                Common.ComConnectionHelper.CloseConnection_SQL(ref conn);
                return dt;
            }
            catch (Exception ex)
            {
                Common.ComConnectionHelper.CloseConnection_SQL(ref conn);
                throw ex;
            }
        }
        #endregion
    }
}
