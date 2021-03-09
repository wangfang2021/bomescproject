﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Common;
using DataAccess;
using Org.BouncyCastle.Crmf;

namespace Logic
{
    public class FS0406_Logic
    {
        FS0406_DataAccess fs0406_dataAccess = new FS0406_DataAccess();
        #region 检索

        public DataTable searchApi(string vcReceiver, string vcType, string vcState, string start, string end)
        {
            return fs0406_dataAccess.searchApi(vcReceiver, vcType, vcState, start, end);
        }
        #endregion

        public void createInfo(string Receiver, bool inFlag, string inTime, bool outFlag, string outStart, string outEnd, string userId, ref string msg)
        {
            fs0406_dataAccess.createInfo(Receiver, inFlag, inTime, outFlag, outStart, outEnd, userId, ref msg);
        }

        public void delApi(List<Dictionary<string, Object>> listInfoData)
        {
            fs0406_dataAccess.delApi(listInfoData);
        }

        public DataTable getData(string vcRelation, string vcFlag)
        {
            if (vcFlag.Equals("入库"))
            {
                return fs0406_dataAccess.getIn(vcRelation);
            }
            else if (vcFlag.Equals("出库"))
            {
                return fs0406_dataAccess.getOut(vcRelation);
            }

            return new DataTable();
        }

        #region 发送

        public void sendMail(List<Dictionary<string, Object>> listInfoData, string strUserId, string strUserName, ref string refMsg, string Email, string unit)
        {
            try
            {
                fs0406_dataAccess.changeState(listInfoData[0]["iAuto_id"].ToString(), "1", strUserId);
                string total = fs0406_dataAccess.getTotal(listInfoData[0]["vcRelation"].ToString());
                string range = listInfoData[0]["vcRange"].ToString();
                //TODO 发送邮件
                string strSubject = "FTMS财务对账。";
                DataTable cCDt = null;

                DataTable receiverDt = new DataTable();
                receiverDt.Columns.Add("address");
                receiverDt.Columns.Add("displayName");
                DataTable dt = fs0406_dataAccess.getReceiverEmail();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = receiverDt.NewRow();
                    dr["address"] = dt.Rows[i]["vcValue2"].ToString();
                    dr["displayName"] = dt.Rows[i]["vcValue1"].ToString();
                    receiverDt.Rows.Add(dr);
                }
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine("<p class=\"ql-align-justify\"><strong style=\"color: black;\">FTMS &nbsp; 相关各位殿</strong></p>");
                sbr.AppendLine("<p class=\"ql-align-justify\"><span style=\"color: black;\">&nbsp;</span></p><p class=\"ql-align-justify\"><span style=\"color: black;\">" + unit + "补给管理课&nbsp;" + strUserName + "</span></p><p class=\"ql-align-justify\"><strong>&nbsp;</strong></p>");
                sbr.AppendLine("<p><strong style=\"color: black;\">" + unit + " " + range + "&nbsp;对帐数据&nbsp;</strong><strong style=\"color: blue;\">&nbsp;<u>月度对帐数</u></strong><strong style=\"color: rgb(0, 51, 204);\"><u>据</u></strong><strong style=\"color: black;\"><u>&nbsp;" + total + "</u></strong><strong style=\"color: blue;\"><u>元&nbsp;&nbsp;</u></strong><strong><u>，请</u></strong><strong style=\"color: black;\"><u>确认</u></strong><strong><u>。</u></strong></p><p class=\"ql-align-justify\"><strong>&nbsp;</strong></p>");
                sbr.AppendLine("<p class=\"ql-align-justify\"><strong>以</strong><strong style=\"color: black;\">上</strong></p><p><br></p>");
                string EmailBody = sbr.ToString();

                string result = ComFunction.SendEmailInfo(Email, unit, EmailBody, receiverDt, cCDt, strSubject, "", false);

                //邮件发送失败
                if (result.Equals("Error"))
                {
                    refMsg = "FTMS发送成功，但邮件发送失败。";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        #endregion
    }
}
