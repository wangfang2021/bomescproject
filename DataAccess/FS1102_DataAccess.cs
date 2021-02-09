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
    public class FS1102_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        public DataTable getSearchInfo(string strReceiver, string strCaseNo, string strTagId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("SELECT T1.LinId,T1.vcCustomerId as vcReceiver,T1.vcCaseNo as vcCaseNo");
                strSql.AppendLine(",T1.vcInPutOrderNo as vcInPutOrderNo,T1.vcPartId as vcPartId,t3.vcPartsNameEN as vcPartENName,t1.vcQty as iQty,'1' as bSelectFlag FROM");
                strSql.AppendLine("(select * from TCaseList where dFirstPrintTime is not null ");
                if (strReceiver != "")
                {
                    strSql.AppendLine("AND vcCustomerId='" + strReceiver + "'");
                }
                if (strCaseNo != "")
                {
                    strSql.AppendLine("AND vcCaseNo='" + strCaseNo + "'");
                }
                if (strTagId != "")
                {
                    strSql.AppendLine("AND vcTagBegin<='" + strTagId + "' AND vcTagEnd>='" + strTagId + "'");
                }
                strSql.AppendLine(")T1");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT * FROM TCustomerInfo)T2");
                strSql.AppendLine("ON T1.vcCustomerId=T2.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT * FROM [TPartInfoMaster] WHERE dTimeFrom<=GETDATE() AND dTimeTo>=GETDATE())T3");
                strSql.AppendLine("ON T1.vcPartId=T3.vcPartsNo");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
