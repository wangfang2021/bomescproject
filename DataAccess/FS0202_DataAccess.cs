using System;
using Common;
using System.Data;
using System.Text;

namespace DataAccess
{
    public class FS0202_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 检索上传历史
        public DataTable searchHistory(string filename, string TimeFrom, string TimeTo)
        {

            StringBuilder sbr = new StringBuilder();
            sbr.Append(" SELECT vcFileName,vcOperatorID,dOperatorTime FROM TSPIHistory \r\n");
            sbr.Append(" WHERE vcType = '0'  \r\n");
            if (!string.IsNullOrWhiteSpace(filename))
                sbr.Append(" and isnull(vcFileName,'') LIKE '" + filename.Trim() + "%' \r\n");
            if (!string.IsNullOrWhiteSpace(TimeFrom))
            {
                DateTime tf = DateTime.ParseExact(TimeFrom, "yyyy/MM/dd", System.Globalization.CultureInfo.CurrentCulture).AddSeconds(-1);

                sbr.Append(" and dOperatorTime> " + ComFunction.getSqlValue(tf, true) + "  \r\n");
            }

            if (!string.IsNullOrWhiteSpace(TimeTo))
            {
                DateTime tt = DateTime.ParseExact(TimeTo, "yyyy/MM/dd", System.Globalization.CultureInfo.CurrentCulture).AddDays(1);
                sbr.Append(" and dOperatorTime<" + ComFunction.getSqlValue(tt, true) + "  \r\n");

            }
            sbr.Append(" order by dOperatorTime desc \r\n");
            return excute.ExcuteSqlWithSelectToDT(sbr.ToString(), "TK");

        }


        #endregion
    }
}