using System;
using Common;
using System.Data;
using System.Text;

namespace DataAccess
{
    public class FS0406_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 检索

        public DataTable searchApi(string vcReceiver, string vcType, string vcState, string start, string end)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine("SELECT a.vcReceiver,b.vcName AS vcState, c.vcName AS vcType,CASE vcType WHEN '0' THEN CONVERT(VARCHAR(10),dRangeStart,111) WHEN '1' THEN (CONVERT(VARCHAR(10),dRangeStart,111)+'-'+CONVERT(VARCHAR(10),dRangeEnd,111)) END AS vcRange,a.dSendTime, a.dCommitTime, a.vcRelation FROM (");
                sbr.AppendLine("SELECT * FROM TIF_Master");
                sbr.AppendLine(") a");
                sbr.AppendLine("LEFT JOIN");
                sbr.AppendLine("(");
                sbr.AppendLine("SELECT vcName,vcValue FROM TCode WHERE vcCodeId = 'C064'");
                sbr.AppendLine(") b ON a.vcState = b.vcValue");
                sbr.AppendLine("LEFT JOIN");
                sbr.AppendLine("(");
                sbr.AppendLine("SELECT vcName,vcValue FROM TCode WHERE vcCodeId = 'C065'");
                sbr.AppendLine(") c ON a.vcType = b.vcValue");
                sbr.AppendLine("WHERE 1=1");
                if (!string.IsNullOrWhiteSpace(vcReceiver))
                {
                    sbr.AppendLine("AND a.vcReceiver = '" + vcReceiver + "' ");
                }
                if (!string.IsNullOrWhiteSpace(vcType))
                {
                    sbr.AppendLine("AND a.vcType = '" + vcType + "' ");
                }
                if (!string.IsNullOrWhiteSpace(vcState))
                {
                    sbr.AppendLine("AND a.vcState = '" + vcState + "' ");
                }
                if (vcType == "0")
                {
                    if (!string.IsNullOrWhiteSpace(start))
                    {
                        start = start.Substring(0, 6);
                        sbr.AppendLine("AND CONVERT(VARCHAR(6),a.dRangeStart,112) >= '" + start + "'");
                    }
                    if (!string.IsNullOrWhiteSpace(end))
                    {
                        start = start.Substring(0, 8);
                        sbr.AppendLine("AND CONVERT(VARCHAR(6),a.dRangeEnd,112) <= '" + end + "'");
                    }
                }
                else if (vcType == "1")
                {
                    if (!string.IsNullOrWhiteSpace(start))
                    {
                        sbr.AppendLine("AND CONVERT(VARCHAR(8),a.dRangeStart,112) >= '" + start + "'");
                    }
                    if (!string.IsNullOrWhiteSpace(end))
                    {
                        sbr.AppendLine("AND CONVERT(VARCHAR(8),a.dRangeEnd,112) <= '" + end + "'");
                    }
                }

                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        #endregion
    }
}