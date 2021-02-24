using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Common;

namespace BatchProcess
{
    public class FP0302
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
                    subject = "中文品名";
                }
                else if (flag == 1)
                {
                    isExist = tenYearOld();
                    subject = "旧型十年年记";
                }
                if (!isExist)
                {//没有要请求的数据
                    ComMessage.GetInstance().ProcessMessage(PageId, "没有要发送的数据", null, strUserId);
                    return true;
                }
                //发送邮件
                string[] email = getEmail(strUserId);
                bool res = sendMail(email[0], email[1], subject, EmailBody(flag));

                if (res)
                {
                    ComMessage.GetInstance().ProcessMessage(PageId, "批处理执行成功", null, strUserId);
                    return true;
                }
                else
                {
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


        public DataTable getReceiverEmail()
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine(
                    "SELECT vcValue1,vcValue2 FROM dbo.TOutCode WHERE vcCodeId = 'C052'AND vcIsColum = '0' ");
                return excute.ExcuteSqlWithSelectToDT(sbr.ToString(), "TK");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 获取邮件体

        public string EmailBody(int flag)
        {
            StringBuilder sbr = new StringBuilder();
            //中文品名
            if (flag == 1)
            {
                sbr.Append("<p>FTMS 相关各位殿：</p>");
                sbr.Append("<p>大家好</p><p><br></p>");
                sbr.Append("<p>补给工作长期支持感谢！</p>");
                sbr.Append("<p>标题事宜，拜托提示一下附件中品番的十年年计数量。</p>");
                sbr.Append("<p>拜托及时回复</p>");
                sbr.Append("<p>谢谢</p>");
                sbr.Append("<p>以上</p>");
            }
            //旧型
            else if (flag == 0)
            {
                sbr.Append("<p>晶文：</p>");
                sbr.Append("<p>你好！</p>");
                sbr.Append("<p><br></p>");
                sbr.Append("<p>补给工作长期支持感谢！</p>");
                sbr.Append("<p>附件为本次中文品名依赖，</p>");
                sbr.Append("<p>拜托及时回复</p>");
                sbr.Append("<p>谢谢</p>");
                sbr.Append("<p>以上</p>");
            }
            return sbr.ToString();
        }

        #endregion

        #region 发送邮件
        public bool sendMail(string Email, string UserName, string strSubject, string EmailBody)
        {
            DataTable cCDt = null;
            DataTable receiverDt = new DataTable();
            receiverDt.Columns.Add("address");
            receiverDt.Columns.Add("displayName");
            DataTable dt = getReceiverEmail();
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
