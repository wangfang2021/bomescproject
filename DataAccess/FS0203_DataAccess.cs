using System;
using System.Data;
using System.Text;
using Common;

namespace DataAccess
{
    public class FS0203_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        public DataTable searchHistory(int flag, string UploadTime)
        {
            string TableName = flag == 0 ? "TPartHistory" : "TSPIHistory";
            StringBuilder sbr = new StringBuilder();
            sbr.Append(" SELECT vcFileName,vcCreater,CONVERT(varchar(20), dCreateTime, 23) as dCreateTime FROM " + TableName + " \r\n");
            sbr.Append(" WHERE 1=1  \r\n");
            if (flag == 1)
            {
                sbr.Append(" and vcType = '1' \r\n");
            }
            if (!string.IsNullOrWhiteSpace(UploadTime))
            {
                sbr.Append(" and dCreateTime = '" + UploadTime + "' \r\n");
            }

            return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
        }
    }
}