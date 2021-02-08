using Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
/// <summary>
/// NQC内制状态及结果获取
/// </summary>
namespace BatchProcess
{
    public class FP00019
    {
        private MultiExcute excute = new MultiExcute();

        #region 主方法
        public bool main(string strUserId)
        {
            string PageId = "FP00019";
            try
            {
                //批处理开始
                ComMessage.GetInstance().ProcessMessage(PageId, "M03PI0200", null, strUserId);
                DataTable dt = GetRequestData();
                if (dt.Rows.Count == 0)
                {//没有超期的数据
                    ComMessage.GetInstance().ProcessMessage(PageId, "M03PI0201", null, strUserId);
                    return true;
                }
                string strMail = getMail(strUserId, "FP00019").Rows[0][0].ToString();
                if (string.IsNullOrEmpty(strMail))
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

        #region 向供应商发送邮件
        public void SendMail(DataSet dsNQC, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();

                sql.Append("                \n");
                sql.Append("                \n");
                sql.Append("                \n");
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

        #region 获取邮件内容
        public DataTable getMail(string strUserId,string strChildFunID)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(" select vcContent from TMailMessageSetting where vcChildFunID = '" + strChildFunID+"' and vcUserId = '"+ strUserId + "'    \n");
                return excute.ExcuteSqlWithSelectToDT(sql.ToString());
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
                sql.Append(" select vcPart_id from TSQJD where dNqDate<GETDATE() and vcYQorNG is null or vcYQorNG = ''    \n");
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
