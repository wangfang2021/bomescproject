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
                sbr.Append(" and vcFileName LIKE '" + filename + "%' \r\n");
            if (!string.IsNullOrWhiteSpace(TimeFrom))
                sbr.Append(" and dOperatorTime>= '" + TimeFrom + "'  \r\n");
            if (!string.IsNullOrWhiteSpace(TimeTo))
                sbr.Append(" and dOperatorTime<= '" + TimeTo + "'  \r\n");
            return excute.ExcuteSqlWithSelectToDT(sbr.ToString());

        }


        #endregion
    }
}