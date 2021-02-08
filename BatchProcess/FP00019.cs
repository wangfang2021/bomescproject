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

                #region 邮件发送准备

                //用户邮箱
                string strUserEmail = getUserEmail();

                //用户名称




                #region 获取所有超期的供应商名称，如果没有，则提示批处理已完成
                DataTable dt = GetRequestData();
                if (dt.Rows.Count == 0)
                {//没有超期的数据
                    ComMessage.GetInstance().ProcessMessage(PageId, "M03PI0201", null, strUserId);
                    return true;
                }
                #endregion

                #region 获取邮箱内容，如果没有获取到，则提示邮箱内容获取失败，批处理结束
                DataTable dtMail = getMail(strUserId, "FP00019");
                if (dtMail == null || dtMail.Rows.Count <= 0)
                {//未获取邮箱信息
                    ComMessage.GetInstance().ProcessMessage(PageId, "M03PI0201", null, strUserId);
                    return true;
                }
                #endregion

                #region 获取收件人信息、抄送人信息
                DataTable sendUser = getSendUser();
                if (sendUser==null || sendUser.Rows.Count<=0)
                {
                    ComMessage.GetInstance().ProcessMessage(PageId, "M03PI0201", null, strUserId);
                    return true;
                }
                DataTable sendUsers = getSendUsers();
                if (sendUsers==null || sendUsers.Rows.Count<=0)
                {
                    ComMessage.GetInstance().ProcessMessage(PageId, "M03PI0201", null, strUserId);
                    return true;
                }
                #endregion

                #endregion

                #region 开始发送邮件

                string strErr = "";     //记录错误信息

                SendMail(dt, dtMail, strUserId, sendUser.Rows[0][1].ToString(), sendUser.Rows[0][0].ToString(), ref strErr);
                if (strErr!="")
                {
                    ComMessage.GetInstance().ProcessMessage(PageId, "M03PI0201", null, strUserId);
                    return true;
                }

                #endregion

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

        #region 获取用户邮箱
        public string getUserEmail()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("         select vcValue from TCode where vcCodeId = 'C009'        ");
                DataTable dt = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                if (dt.Rows[0][0]!=null && dt.Rows[0][0].ToString()!="")
                {

                }
                return "";
            }
            catch (Exception)
            {

                throw;
            }

        }
        #endregion


        #region 向供应商发送邮件
        public void SendMail(DataTable dt,DataTable dtMail, string strUserId, string strEmail, string strUserName, ref string strErr)
        {
            string strTitle = "";//邮件标题
            string strContent = "";//邮件内容

            strTitle = dtMail.Rows[0]["vcTitle"].ToString();
            strContent = dtMail.Rows[0]["vcContent"].ToString();
            string dateTime = DateTime.Now.ToString("yyyy年MM月");
            strContent = strContent.Replace("##yearmonth##", dateTime);

            //再向供应商发邮件
            StringBuilder strEmailBody = new StringBuilder();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string strSupplier_id = dt.Rows[i]["vcSupplier_id"].ToString();
                DataTable receiverDt = getSupplierEmail(strSupplier_id);
                if (receiverDt == null)
                {
                    strErr += "未找到 '" + strSupplier_id + "' 供应商邮件信息";
                    return;
                }
                ComFunction.SendEmailInfo(strEmail, strUserName, strContent, receiverDt, null, strTitle, "", false);
            }
        }
        #endregion

        #region 获取邮件内容
        public DataTable getMail(string strUserId,string strChildFunID)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(" select * from TMailMessageSetting where vcChildFunID = '" + strChildFunID+"' and vcUserId = '"+ strUserId + "'    \n");
                return excute.ExcuteSqlWithSelectToDT(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 根据供应商获取邮件地址
        public DataTable getSupplierEmail(string strSupplierId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("    select vcEmail1,vcEmail2,vcEmail3 from TSupplier where vcSupplier_id='" + strSupplierId + "'   \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
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
                sql.Append(" select vcSupplier_id from TSQJD where dNqDate<GETDATE() and vcYQorNG is null or vcYQorNG = ''    \n");
                return excute.ExcuteSqlWithSelectToDT(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 获取收件人信息
        public DataTable getSendUser()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("       select top 1 vcValue,vcName from TCode where vcCodeId = 'C002'      ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 获取发件人信息
        public DataTable getSendUsers()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("      select vcValue1 as 'UserName',vcValue2 as 'Email' from TOutCode where vcCodeId = 'C005' and vcIsColum = 0       ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
