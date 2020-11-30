using System;
using Common;
using System.Data;
using System.Text;

namespace DataAccess
{
    public class FS0202_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        public DataTable searchHistory(string filename, string TimeFrom, string TimeTo)
        {

            StringBuilder sbr = new StringBuilder();
            sbr.Append(" SELECT vcFileName,vcCreater,dCreateTime FROM TSprlPartHistory \r\n");
            sbr.Append(" WHERE 1=1  \r\n");
            if (!string.IsNullOrWhiteSpace(filename))
                sbr.Append(" and vcFileName LIKE '" + filename + "%' \r\n");
            if (!string.IsNullOrWhiteSpace(TimeFrom))
                sbr.Append(" and dCreateTime>= " + TimeFrom + "  \r\n");
            if (!string.IsNullOrWhiteSpace(TimeTo))
                sbr.Append(" and dCreateTime<= " + TimeTo + "  \r\n");
            string a = sbr.ToString();
            return excute.ExcuteSqlWithSelectToDT(sbr.ToString());

        }

    }
}