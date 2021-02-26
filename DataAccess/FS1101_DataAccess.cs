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
    public class FS1101_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        public DataTable getSearchInfo(string strPackMaterNo, string strTrolleyNo, string strPartId, string strOrderNo, string strLianFan)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select a.iAutoId, a.vcInno as vcInPutOrderNo,a.vcLotid as vcPackMaterNo,a.[vcTrolleyNo] AS vcTrolleyNo,a.vcPackingpartsno as vcPackPartId  ");
                strSql.AppendLine("		,dQty as iQty,vcPackingpartslocation as vcLocation,'1' as bSelectFlag 	from   ");
                strSql.AppendLine("(select * from tpacklist)a  ");
                strSql.AppendLine("left join  ");
                strSql.AppendLine("(select * from TOperateSJ where vcZYType='S0')b  ");
                strSql.AppendLine("on a.vcInno=b.vcInputNo  ");
                strSql.AppendLine("WHERE a.[dFirstPrintTime] IS NOT NULL   ");
                if (strPackMaterNo != "")
                {
                    strSql.AppendLine("  AND a.vcLotid like '%" + strPackMaterNo + "%' ");
                }
                if (strTrolleyNo != "")
                {
                    strSql.AppendLine("  AND a.vcTrolleyNo='" + strTrolleyNo + "' ");
                }
                if (strPartId != "")
                {
                    strSql.AppendLine("  AND b.vcPart_id like '" + strPartId + "%' ");
                }
                if (strOrderNo != "")
                {
                    strSql.AppendLine("  AND b.vcKBOrderNo='" + strOrderNo + "' ");
                }
                if (strLianFan != "")
                {
                    strSql.AppendLine("  AND b.vcKBLFNo='" + strLianFan + "'");
                }
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
