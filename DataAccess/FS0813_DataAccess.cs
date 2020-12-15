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
    public class FS0813_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 检索
        public DataTable Search(string strSellNo, string strStartTime, string strEndTime)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select distinct vcBianCi,vcSellNo,vcTruckNo,convert(varchar(16),dOperatorTime,120) as dOperatorTime from TSell  \n");
                strSql.Append("where isnull(vcSellNo,'') like '%" + strSellNo + "%' \n");
                strSql.Append("and dOperatorTime >= '" + (strStartTime == "" ? "2001-01-01 00:00" : strStartTime) + "' and dOperatorTime <= '" + (strEndTime == "" ? "2099-12-31 23:59" : strEndTime) + "'  \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 子画面初始化
        public DataTable initSubApi(string strSellNo)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select * from TSell  \n");
                strSql.Append("where isnull(vcSellNo,'') like '%" + strSellNo + "%' \n");
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
