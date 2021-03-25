using Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace BatchProcess
{
    public class FP0023
    {
        private MultiExcute excute = new MultiExcute();

        #region 主方法
        public bool main()
        {
            string PageId = "FP0023";
            string strUserId = "000000";
            try
            {
                ComMessage.GetInstance().ProcessMessage(PageId, "M00PI2300", null, strUserId);
                Start();
                //批处理
                ComMessage.GetInstance().ProcessMessage(PageId, "M00PI2301", null, strUserId);
                return true;
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(PageId, "M00PE2300", null, strUserId);
                throw ex;
            }
        }
        #endregion

        #region 更新数据库
        public void Start()
        {
            try
            {

                StringBuilder strsql = new StringBuilder();
                strsql.Append("     select vcUserID,vcStop,dLoginTime,dOperatorTime from SUser    \n");
                DataTable dtUser = excute.ExcuteSqlWithSelectToDT(strsql.ToString());


                for (int i = 0; i < dtUser.Rows.Count; i++)
                {

                    DateTime NowTime = Convert.ToDateTime(DateTime.Now);//当前时间
                    DateTime LoginTime = new DateTime();
                    if (dtUser.Rows[i]["dLoginTime"].ToString() != "")
                    {
                        LoginTime = Convert.ToDateTime(dtUser.Rows[i]["dLoginTime"].ToString());//最晚登录时间

                    }
                    else
                    {

                        LoginTime = Convert.ToDateTime(dtUser.Rows[i]["dOperatorTime"].ToString());//创建时间
                    }
                    TimeSpan Time = NowTime.Subtract(LoginTime);
                    if (Time.Days > 30)
                    {
                        strsql.Append("    update SUser set vcStop='1' where vcUserID='" + dtUser.Rows[i]["vcUserID"].ToString() + "'     \n");
                    }
                }

                SqlConnection conn = ComConnectionHelper.CreateSqlConnection();
                ComConnectionHelper.OpenConection_SQL(ref conn);
                SqlTransaction st = conn.BeginTransaction();
                SqlCommand cmd = new SqlCommand(strsql.ToString(), conn, st);
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 0;
                cmd.ExecuteNonQuery();
                st.Commit();
                ComConnectionHelper.CloseConnection_SQL(ref conn);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
