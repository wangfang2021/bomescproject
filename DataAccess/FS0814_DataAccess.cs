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
    public class FS0814_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 检索
        public DataTable Search(string strYearMonth)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select * from TCalendar  \n");
                strSql.Append("where isnull(vcYearMonth,'') like '%" + strYearMonth + "%' \n");
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
