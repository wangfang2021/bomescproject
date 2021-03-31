using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Common;
using System.Data.SqlClient;
using System.Collections;

namespace DataAccess
{
    public class FS0718_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 按检索条件检索,返回dt
        public DataTable Search(string strDownloadDiff)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("           select * from TPackSearch where  1=1               \n");
                if (!string.IsNullOrEmpty(strDownloadDiff))
                {
                    strSql.AppendLine("       and dFirstDownload " + strDownloadDiff + "          ");
                }
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
