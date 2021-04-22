using Common;
using System;
using System.Text;
using System.Data;
namespace BatchProcess
{
    public class FP0019
    {
        /*
         * 时间：2020-02-08
         * 作者：董镇
         * 描述：此批处理用来做生确超期邮件提醒
         *       原单位向供应商发生确后，有一个纳期时间，生确表中如果这条数据的纳期时间已经小于当前时间(纳期超期了)，就会给供应商发送邮件
         *       并且生确进度还是未确认状态
         */
        private MultiExcute excute = new MultiExcute();

        #region 主方法
        public bool main(string strUserId)
        {
            string PageId = "FP0019";
            try
            {
                //批处理开始
                ComMessage.GetInstance().ProcessMessage(PageId, "M00PI1901", null, strUserId);

                #region 邮件发送准备

                #region 用户邮箱
                string strUserEmail = getUserEmail();
                if (string.IsNullOrEmpty(strUserEmail))
                {
                    ComMessage.GetInstance().ProcessMessage(PageId, "M00PE1901", null, strUserId);
                    return true;
                }
                #endregion

                #region 用户名称
                string strUserName = getUserName();
                if (string.IsNullOrEmpty(strUserEmail))
                {
                    ComMessage.GetInstance().ProcessMessage(PageId, "M00PI1902", null, strUserId);
                    return true;
                }
                #endregion

                #region 邮件内容
                string strEmailBody = getEmailBody();
                if (string.IsNullOrEmpty(strEmailBody))
                {
                    ComMessage.GetInstance().ProcessMessage(PageId, "M00PE1903", null, strUserId);
                    return true;
                }
                //这里做了年月的转换
                strEmailBody = strEmailBody.Replace("##yearmonth##", DateTime.Now.ToString("yyyy年MM月dd日"));
                #endregion

                #region 收件人
                DataTable receiverDt = getReceiverDt();
                if (receiverDt==null || receiverDt.Rows.Count<=0)
                {
                    ComMessage.GetInstance().ProcessMessage(PageId, "M00PI1903", null, strUserId);
                    return true;
                } 
                #endregion

                #region 抄送人
                /*
                 * 注意：抄送人不需要判断是否拿到数据，如果没有拿到数据，说明没有添加抄送人，对于发送邮件无影响
                 */
                DataTable cCDt = getCDt();
                #endregion

                #region 邮件主题
                string strSubject = getSubject();
                if (string.IsNullOrEmpty(strSubject))
                {
                    ComMessage.GetInstance().ProcessMessage(PageId, "M00PE1905", null, strUserId);
                    return true;
                }
                #endregion

                #region 附件
                /*
                 * 有附件给地址，无给空字符
                 */
                string strFilePath = "";
                #endregion

                #region 传入附件后，是否需要删除附件
                /*
                 * true:需要删除附件
                 * false:需要删除附件/没有附件
                 */
                bool delFileNameFlag = false;
                #endregion

                #endregion

                #region 开始发送邮件
                //记录错误信息
                string strErr = "";     
                SendMail(strUserEmail, strUserName, strEmailBody, receiverDt, cCDt, strSubject, strFilePath, delFileNameFlag,ref strErr);
                if (strErr!="")
                {
                    ComMessage.GetInstance().ProcessMessage(PageId, "M00PE1906", null, strUserId);
                    return true;
                }
                #endregion

                //批处理结束
                ComMessage.GetInstance().ProcessMessage(PageId, "M00PI1902", null, strUserId);
                return true;
            }
            catch (Exception ex)
            {
                //批处理异常结束
                ComMessage.GetInstance().ProcessMessage(PageId, "M00PE1907", null, strUserId);
                throw ex;
            }
        }
        #endregion

        #region 获取用户邮箱
        public string getUserEmail()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("         select vcName from TCode where vcCodeId = 'C009'   ");
                DataTable dt = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                if (dt.Rows.Count>0)
                {
                    return dt.Rows[0][0].ToString();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region 获取用户名称
        public string getUserName()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("         select vcValue from TCode where vcCodeId = 'C009'        ");
                DataTable dt = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                if (dt.Rows.Count>0)
                {
                    return dt.Rows[0][0].ToString();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 获取邮件内容
        public string getEmailBody()
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(" select vcValue4 from TOutCode where vcCodeId = 'C016' and vcValue1 = 'FP0019'    \n");
                DataTable dt = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                if (dt.Rows.Count>0)
                {
                    return dt.Rows[0][0].ToString();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 获取收件人
        public DataTable getReceiverDt()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("      select b.address,b.displayName from        ");
                strSql.AppendLine("      (       ");
                strSql.AppendLine("      	select vcSupplier_id from TSQJD       ");
                strSql.AppendLine("      	where vcJD='1' and dNqDate<GETDATE() and ( vcYQorNG is null or vcYQorNG = '')       ");
                strSql.AppendLine("      	group by vcSupplier_id       ");
                strSql.AppendLine("      )  a       ");
                strSql.AppendLine("      inner join        ");
                strSql.AppendLine("      (       ");
                strSql.AppendLine("      select vcEmail1 as 'address',vcSupplier_id as 'displayName' from TSupplier where vcEmail1 is not null and vcEmail1 !=''       ");
                strSql.AppendLine("      union all       ");
                strSql.AppendLine("      select vcEmail2 as 'address',vcSupplier_id as 'displayName' from TSupplier where vcEmail2 is not null and vcEmail2 !=''       ");
                strSql.AppendLine("      union all       ");
                strSql.AppendLine("      select vcEmail3 as 'address',vcSupplier_id as 'displayName' from TSupplier where vcEmail3 is not null and vcEmail3 !=''       ");
                strSql.AppendLine("      ) b on a.vcSupplier_id = b.displayName       ");
                strSql.AppendLine("      group by address,displayName        ");

                DataTable dt = excute.ExcuteSqlWithSelectToDT(strSql.ToString(),"TK");
                if (dt.Rows.Count>0)
                {
                    return dt;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region 获取抄送人
        public DataTable getCDt()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("        select vcValue2 as 'address',vcValue1 as 'displayName' from TOutCode where vcCodeId = 'C005' and vcIsColum = '0'         ");
                DataTable dt = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                if (dt.Rows.Count>0)
                {
                    return dt;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region 获取邮件主题
        public string getSubject()
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(" select vcValue3 from TOutCode where vcCodeId = 'C016' and vcValue1 = 'FP0019'    \n");
                DataTable dt = excute.ExcuteSqlWithSelectToDT(sql.ToString(),"TK");
                if (dt.Rows.Count>0)
                {
                    return dt.Rows[0][0].ToString();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 向供应商发送邮件
        public void SendMail(string strUserEmail, string strUserName, string strEmailBody, DataTable receiverDt, DataTable cCDt, string strSubject, string strFilePath, bool delFileNameFlag,ref string strErr)
        {
            try
            {
                ComFunction.SendEmailInfo(strUserEmail, strUserName, strEmailBody, receiverDt, cCDt, strSubject, strFilePath, delFileNameFlag);
            }
            catch (Exception ex)
            {
                strErr = ex.Message;
                throw ex;
            }
            
        }
        #endregion

    }
}
