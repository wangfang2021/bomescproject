using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Common;

namespace BatchProcess
{
    public class FP0032
    {
        private MultiExcute excute = new MultiExcute();

        #region 主方法

        public bool main(string strUserId, int flag)
        {
            string PageId = "FP0302";
            try
            {
                //批处理开始
                ComMessage.GetInstance().ProcessMessage(PageId, "批处理开始", null, strUserId);
                //获取发邮件需求
                bool isExist = false;
                string subject = "";

                if (flag == 0)
                {
                    isExist = partNameFlag();
                }
                else if (flag == 1)
                {
                    isExist = tenYearOld();
                }
                if (!isExist)
                {//没有要请求的数据
                    ComMessage.GetInstance().ProcessMessage(PageId, "没有要发送的数据", null, strUserId);
                    return true;
                }

                List<string> list = EmailBody(flag);
                if (list.Count == 0)
                {
                    FailsendMail("buji@tftm.com.cn", "System", "邮件发送失败", "未找到邮件体，请进行邮件体常量维护");
                    ComMessage.GetInstance().ProcessMessage(PageId, "批处理执行失败", null, strUserId);
                    return false;
                }
                //发送邮件
                string[] email = getEmail(strUserId);
                bool res = sendMail(email[0], email[1], list[0], list[1], flag);

                if (res)
                {
                    ComMessage.GetInstance().ProcessMessage(PageId, "批处理执行成功", null, strUserId);
                    return true;
                }
                else
                {
                    FailsendMail("buji@tftm.com.cn", "System", "邮件发送失败", FailEmailBody(flag));
                    ComMessage.GetInstance().ProcessMessage(PageId, "批处理执行失败", null, strUserId);
                    return false;
                }

            }
            catch (Exception ex)
            {
                //批处理异常结束
                ComMessage.GetInstance().ProcessMessage(PageId, ex.ToString(), null, strUserId);
                throw ex;
            }
        }

        #endregion 

        #region 获取销售公司邮箱


        public DataTable getReceiverEmail(int flag)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                string tmp = "";
                if (flag == 0)
                {
                    tmp = "2";
                }
                else if (flag == 1)
                {
                    tmp = "1";
                }
                sbr.AppendLine("SELECT vcValue1,vcValue2 FROM dbo.TOutCode WHERE vcCodeId = 'C052'AND vcIsColum = '0'  AND vcValue4 = '" + tmp + "' ");

                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 获取接收失败信息担当邮箱

        public DataTable getFailEmail()
        {
            StringBuilder sbr = new StringBuilder();
            sbr.AppendLine(
                "SELECT vcValue1,vcValue2 FROM dbo.TOutCode WHERE vcCodeId = 'C053'AND vcIsColum = '0' ");
            return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
        }

        #endregion

        #region 获取邮件体

        public List<string> EmailBody(int flag)
        {
            StringBuilder sbr = new StringBuilder();
            sbr.AppendLine(
                "SELECT vcValue3,vcValue4 FROM TOutCode WHERE vcCodeId = 'C024' AND vcIsColum = '0' AND vcValue2 = '" +
                flag + "'");
            DataTable dt = excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            List<string> res = new List<string>();
            if (dt.Rows.Count > 0)
            {
                res.Add(dt.Rows[0]["vcValue3"].ToString());
                res.Add(dt.Rows[0]["vcValue4"].ToString());
            }

            return res;
        }

        public string FailEmailBody(int flag)
        {
            StringBuilder sbr = new StringBuilder();
            string time = DateTime.Now.ToString("yyyy-MM-dd");

            //中文品名
            if (flag == 1)
            {
                sbr.Append("<p>" + time + "旧型十年年记提醒邮件发送失败</p>");
                sbr.Append("<p>以上</p>");
            }
            //旧型
            else if (flag == 0)
            {
                sbr.Append("<p>" + time + "中文品名提醒邮件发送失败</p>");
                sbr.Append("<p>以上</p>");
            }
            return sbr.ToString();
        }

        #endregion

        #region 发送邮件
        public bool sendMail(string Email, string UserName, string strSubject, string EmailBody, int flag)
        {
            DataTable cCDt = null;
            DataTable receiverDt = new DataTable();
            receiverDt.Columns.Add("address");
            receiverDt.Columns.Add("displayName");
            DataTable dt = getReceiverEmail(flag);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = receiverDt.NewRow();
                dr["address"] = dt.Rows[i]["vcValue2"].ToString();
                dr["displayName"] = dt.Rows[i]["vcValue1"].ToString();
                receiverDt.Rows.Add(dr);
            }
            string result = ComFunction.SendEmailInfo(Email, UserName, EmailBody, receiverDt, cCDt, strSubject, "", false);
            if (result.Equals("Error"))
            {
                return false;
            }

            return true;
        }
        public bool FailsendMail(string Email, string UserName, string strSubject, string EmailBody)
        {
            DataTable cCDt = null;
            DataTable receiverDt = new DataTable();
            receiverDt.Columns.Add("address");
            receiverDt.Columns.Add("displayName");
            DataTable dt = getFailEmail();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = receiverDt.NewRow();
                dr["address"] = dt.Rows[i]["vcValue2"].ToString();
                dr["displayName"] = dt.Rows[i]["vcValue1"].ToString();
                receiverDt.Rows.Add(dr);
            }
            string result = ComFunction.SendEmailInfo(Email, UserName, EmailBody, receiverDt, cCDt, strSubject, "", false);
            if (result.Equals("Error"))
            {
                return false;
            }

            return true;
        }
        #endregion

        #region 判断是否需要发邮件

        public bool partNameFlag()
        {
            StringBuilder sbr = new StringBuilder();
            sbr.AppendLine("SELECT iAutoId");
            sbr.AppendLine("FROM TPartNameCN");
            sbr.AppendLine("WHERE vcIsLock = '0'");
            DataTable dt = excute.ExcuteSqlWithSelectToDT(sbr.ToString(), "TK");
            if (dt.Rows.Count > 0)
            {
                return true;
            }

            return false;
        }

        public bool tenYearOld()
        {
            StringBuilder sbr = new StringBuilder();
            sbr.AppendLine("SELECT iAutoId");
            sbr.AppendLine("FROM TJiuTenYear");
            sbr.AppendLine("WHERE vcIsLock = '0'");
            DataTable dt = excute.ExcuteSqlWithSelectToDT(sbr.ToString(), "TK");
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }

        #endregion

        #region 获取发送者信息

        public string[] getEmail(string userId)
        {
            string[] res = new string[2];
            StringBuilder sbr = new StringBuilder();
            sbr.AppendLine("SELECT vcEmail,vcUserName FROM SUser WHERE vcUserID = '" + userId + "'");
            DataTable dt = excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            if (dt.Rows.Count > 0)
            {
                res[0] = dt.Rows[0]["vcEmail"].ToString();
                res[1] = dt.Rows[0]["vcUserName"].ToString();
            }
            return res;
        }

        #endregion
    }

}
