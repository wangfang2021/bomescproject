using System;
using System.Security;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;

namespace Common
{
    /// <summary>
    /// ComMessage 的摘要说明。

    /// </summary>
    public class ComMessage
    {
        protected static ComMessage oInstance = null;//单例
        protected static readonly object oLock = new object();
        /// <summary>
        /// 单例模式，限制实例化
        /// </summary>
        private ComMessage()
        {
        }
        /// <summary>
        /// 获取实例
        /// </summary>
        /// <returns></returns>
        public static ComMessage GetInstance()
        {
            try
            {
                lock (oLock)
                {
                    if (oInstance == null)
                    {
                        oInstance = new ComMessage();
                    }

                    return oInstance;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
      

      

        /// <summary>
        /// 返回消息内容
        /// </summary>
        /// <param name="strMessageId"></param>
        /// <returns></returns>
        public string getMessageInfo(string strMessageId)
        {
            MultiExcute excute = new MultiExcute();
            System.Data.DataTable dt = new System.Data.DataTable();
            dt = excute.ExcuteSqlWithSelectToDT("   select vcMessageContent from SMessage where vcMessage_Id='"+ strMessageId + "'      \n");
            if (dt != null && dt.Rows.Count > 0)
                return dt.Rows[0]["vcMessageContent"].ToString();
            else
                return null;
        }

        /// <summary>
        /// 消息处理
        /// </summary>
        /// <param name="strFunctionID">机能编号</param>
        /// <param name="oParent">父窗口</param>
        /// <param name="strType">E：异常消息 I：正常消息提醒</param>
        /// <param name="strMsg">消息内容</param>
        /// <param name="strException">异常ex.Message</param>
        /// <param name="strUserID">用户ID</param>
        /// <param name="strSiteID">站点ID</param>
        public void ProcessMessage(string strFunctionID,
                                          string strMessageId,
                                          Exception exParm,
                                          string strUserID)
        {

            string strExMsg = "";
            string strExTrace = "";
            if (exParm != null)
            {
                strExMsg = exParm.Message;
                strExTrace = exParm.StackTrace;
            }
            string strType = strMessageId.Substring(4, 1);//I是消息，E是异常
            try
            {
                //显示到画面
                string strMsg = getMessageInfo(strMessageId);

                //写入数据库
                WriteInDB(strFunctionID, strType, strMsg, strExMsg, strExTrace, strUserID);
 
            }
            catch (Exception ex)
            {
                //消息处理过程中出现错误，不记录
            }
        }

        /// <summary>
        /// Write in DB
        /// </summary>
        /// <param name="strFunctionID">function ID</param>
        /// <param name="oMB">Message body</param>
        /// <param name="strUserID">User ID</param>
        protected static void WriteInDB(string strFunctionID, string strType, string strMsg,string strExMsg,string strExTrace, string strUserID)
        {
            try
            {
                MultiExcute me;
                me = new MultiExcute();
                System.Data.SqlClient.SqlParameter[] parameters = {
                    new SqlParameter("@vcMessage", SqlDbType.NVarChar),
                    new SqlParameter("@vcException", SqlDbType.NVarChar),
                    new SqlParameter("@vcTrack", SqlDbType.NVarChar)
                };
                parameters[0].Value = strMsg;
                parameters[1].Value = strExMsg;
                parameters[2].Value = strExTrace;
                string strSql = "insert into SLog(UUID,vcFunctionID,vcLogType,vcUserID,vcMessage,vcException,vcTrack,dCreateTime) values(newid(),"
                                                            +"'" +strFunctionID + "','"
                                                            + strType + "','"
                                                            + strUserID + "',"
                                                            + "@vcMessage,"
                                                            + "@vcException,"
                                                            + "@vcTrack,"
                                                            + "CONVERT(varchar, GETDATE(),120))";
                me.ExcuteSqlWithStringOper(strSql, parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
         

    }

     
}
