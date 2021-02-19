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
        public DataTable Search(string strSellNo, string strStartTime, string strEndTime,string strYinQuType)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select distinct t1.vcYinQuType,t2.vcName as vcYinQuTypeName,t1.vcBianCi,t1.vcSellNo,t1.vcTruckNo, \n");
                strSql.Append("convert(varchar(16),t1.dOperatorTime,120) as dOperatorTime,t1.vcSender from TSell t1    \n");
                strSql.Append("left join (select * from tCode where vcCodeid='C058') t2 on t1.vcYinQuType=t2.vcValue   \n");
                strSql.Append("where 1=1 \n");
                if (strSellNo != "" && strSellNo != null)
                    strSql.Append("and isnull(t1.vcSellNo,'') like '%" + strSellNo + "%' \n");
                if (strStartTime == "" || strStartTime == null)
                    strStartTime = "2001-01-01 00:00";
                if (strEndTime == "" || strEndTime == null)
                    strEndTime = "2099-12-31 23:59";
                strSql.Append("and t1.dOperatorTime >= '" + strStartTime + "' and t1.dOperatorTime <= '" + strEndTime + "'  \n");
                if (strYinQuType != "" && strYinQuType != null)
                    strSql.Append("and isnull(t1.vcYinQuType,'') = '" + strYinQuType + "' \n");
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
