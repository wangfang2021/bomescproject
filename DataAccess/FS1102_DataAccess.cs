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
                strSql.AppendLine("SELECT T1.iAutoId as LinId,T1.vcCpdcode as vcReceiver,SUBSTRING(T1.vcCaseno,1,5)+'-'+SUBSTRING(T1.vcCaseno,6,5) as vcCaseNo");
                strSql.AppendLine(",T1.vcInno as vcInPutOrderNo,T1.vcPart_id as vcPartId,t2.vcPartsNameEN as vcPartENName,t1.iQty as iQty,'1' as bSelectFlag FROM");
                strSql.AppendLine("(select * from TCaseList where dFirstPrintTime is not null");
                if (strReceiver != "")
                {
                    strSql.AppendLine("AND vcCpdcode='" + strReceiver + "'");
                }
                if (strCaseNo != "")
                {
                    strSql.AppendLine("AND cast(vcCaseNo as int)='" + strCaseNo + "'");
                }
                if (strTagId != "")
                {
                    strSql.AppendLine("AND vcCasebarcode<='" + strTagId + "' AND vcCasebarcode>='" + strTagId + "'");
                }
                strSql.AppendLine(")T1");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT * FROM [TPartInfoMaster] WHERE dTimeFrom<=GETDATE() AND dTimeTo>=GETDATE())T2");
                strSql.AppendLine("ON T1.vcPart_id=T2.vcPartsNo");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
