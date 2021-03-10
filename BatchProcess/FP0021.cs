using Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
/// <summary>
/// NQC内外合状态获取_EKANBAN
/// </summary>
namespace BatchProcess
{
    public class FP0021
    {
        private MultiExcute excute = new MultiExcute();

        #region 主方法
        public bool main(string strUserId)
        {
            string PageId = "FP0020";
            try
            {
                //批处理开始
                ComMessage.GetInstance().ProcessMessage(PageId, "M03PI0200", null, strUserId);
                DataTable dt = GetRequestData();
                if (dt.Rows.Count == 0)
                {//没有FORECAST要请求的数据
                    ComMessage.GetInstance().ProcessMessage(PageId, "M03PI0201", null, strUserId);
                    return true;
                }
                DataSet ds = GetNQCData(dt);
                if (ds.Tables.Count == 0)
                {//没有FORECAST结果数据
                    ComMessage.GetInstance().ProcessMessage(PageId, "M03PI0201", null, strUserId);
                    return true;
                }
                UpdateDB(ds, strUserId);
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
                    string strTableName = dsNQC.Tables[i].TableName;//vcPlant + "_" + vcCLYM + "_" + iTimes+ "_" + status;
                    string[] strname = strTableName.Split('_');
                    string strPlant = strname[0];
                    string strCLYM = strname[1];
                    string strTimes = strname[2];
                    string strStatus = strname[3];//处理中：P    处理完成：C     处理失败：E
                    string excutetime = strname[4];
                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        sql.Append("update TNQCStatus_HS_EKANBAN set dWCTime=nullif('" + excutetime + "',''),vcOperatorID='" + strUserId + "',dOperatorTime=GETDATE(),     \n");
                        sql.Append("vcStatus='" + strStatus + "'    \n");
                        sql.Append("where vcCLYM='" + strCLYM + "' and vcPlant='" + strPlant + "' and iTimes=" + strTimes + "    \n");

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
                    sql.Append("where Process_Factory='TFTM" + vcPlant + "' and Process_YYYYMM='" + vcCLYM + "' and Process_Cycle_NO=" + iTimes + " \n");
                    sql.Append("and Process_Name= 'NQCS02'   \n");
                    /* sql.Append("and Process_Name= 'NQCE0" + vcPlant + "'   \n");*///NQCE01/NQCE02/NQCE03：#1/#2/#3内外合EKANBAN   
                    DataTable dtNQCStatus = NQCSearch(sql.ToString());
                    if (dtNQCStatus.Rows.Count > 0)
                    {//有处理完成的数据
                        string status = dtNQCStatus.Rows[0]["Process_Status"].ToString();//处理中：P    处理完成：C     处理失败：E
                        string excutetime = dtNQCStatus.Rows[0]["ExecuteTime"].ToString();
                        dtNQCStatus.TableName = vcPlant + "_" + vcCLYM + "_" + iTimes + "_" + status + "_" + excutetime;
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
                sql.Append("select distinct vcPlant,vcCLYM,iTimes from TNQCStatus_HS_EKANBAN where vcStatus!='C'    \n");
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
