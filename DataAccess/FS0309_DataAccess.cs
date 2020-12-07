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
    public class FS0309_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        #region 按检索条件检索,返回dt
        public DataTable Search(string strChange, string strPart_id, string strOriginCompany, string strHaoJiu
            , string strProjectType, string strPriceChangeInfo, string strCarTypeDev, string strSupplier_id
            , string strReceiver, string strPriceState
            )
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("       select * from TPrice         \n");
                strSql.Append("       where          \n");
                strSql.Append("       1=1         \n");
                if(strChange!="")
                    strSql.Append("       and vcPriceChangeInfo=''         \n");
                if (strPart_id != "")
                    strSql.Append("       and vcPart_id like '%%'         \n");
                if (strOriginCompany != "")
                    strSql.Append("       and vcOriginCompany like '%%'         \n");
                if (strHaoJiu != "")
                    strSql.Append("       and vcHaoJiu=''         \n");
                if (strProjectType != "")
                    strSql.Append("       and vcProjectType=''         \n");
                if (strPriceChangeInfo != "")
                    strSql.Append("       and vcPriceChangeInfo=''         \n");
                if (strCarTypeDev != "")
                    strSql.Append("       and vcCarTypeDev=''         \n");
                if (strSupplier_id != "")
                    strSql.Append("       and vcSupplier_id=''         \n");
                if (strReceiver != "")
                    strSql.Append("       and vcReceiver like '%%'         \n");
                if (strPriceState != "")
                    strSql.Append("       and vcPriceState=''         \n");
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
