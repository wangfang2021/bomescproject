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
                strSql.AppendLine("SELECT '' as LinId,vcInPutOrderNo,vcPackMaterNo,vcTrolleyNo,vcPackPartId,vcLocation,sum(vcQty) as iQty,'1' as bSelectFlag ");
                strSql.AppendLine("FROM (");
                strSql.AppendLine("SELECT [vcInPutOrderNo] AS vcInPutOrderNo,[vcPackMaterNo] AS vcPackMaterNo,[vcTrolleyNo] AS vcTrolleyNo,[vcPackPartId] AS vcPackPartId");
                strSql.AppendLine("      ,[vcQty] AS vcQty,[vcLocation] AS vcLocation");
                strSql.AppendLine("  FROM [dbo].[TPackMaterInfo] ");
                strSql.AppendLine("  WHERE [dFirstPrintTime] IS NOT NULL ");
                if (strPackMaterNo != "")
                {
                    strSql.AppendLine("  AND [vcPackMaterNo] like '%"+ strPackMaterNo + "%' ");
                }
                if (strTrolleyNo != "")
                {
                    strSql.AppendLine("  AND [vcTrolleyNo]='"+ strTrolleyNo + "' ");
                }
                if (strPartId != "")
                {
                    strSql.AppendLine("  AND vcPartId like '"+ strPartId + "%' ");
                }
                if (strOrderNo != "")
                {
                    strSql.AppendLine("  AND vcOrderNo='"+ strOrderNo + "' ");
                }
                if (strLianFan != "")
                {
                    strSql.AppendLine("  AND vcLianFan='"+ strLianFan + "'");
                }
                strSql.AppendLine(")T1	GROUP BY vcInPutOrderNo,vcPackMaterNo,vcTrolleyNo,vcPackPartId,vcLocation");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
