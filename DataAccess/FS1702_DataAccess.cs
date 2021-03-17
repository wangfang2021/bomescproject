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
    public class FS1702_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 检索所有的工程下拉框选择
        public DataTable getAllProject()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select distinct vcProject as vcValue,vcProject as vcName from TChuHe_Detail  \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 检索
        public DataTable Search(string vcProject, string dChuHeDateFrom, string dChuHeDateTo)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select iAutoId,vcQueRenNo,dChuHeDate,iQuantity,    \n");
                strSql.Append("case when dQueRenPrintTime is not null then '√' else '' end as QueRenPrintFlag,    \n");
                strSql.Append("case when left(vcQueRenNo,2)='BW' then '━' when dKBPrintTime is not null then '√' else '' end as KBPrintFlag,    \n");
                strSql.Append("case when dChuHeOKTime is not null then '√' else '' end as ChuHeOKFlag    \n");
                strSql.Append("from TChuHe where 1=1   \n");
                if (vcProject != "" && vcProject != null)
                    strSql.Append("and vcQueRenNo like '"+vcProject+"%'    \n");
                if (dChuHeDateFrom == "" || dChuHeDateFrom == null)
                    dChuHeDateFrom = "2001/01/01";
                if (dChuHeDateTo == "" || dChuHeDateTo == null)
                    dChuHeDateTo = "2099/12/31";
                strSql.Append("and dChuHeDate between '" + dChuHeDateFrom + "' and '" + dChuHeDateTo + "'  \n");
                strSql.Append("order by vcQueRenNo,dChuHeDate   \n");
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
