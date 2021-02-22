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
    public class FS0805_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        public DataTable getSearchInfo(string strSellNo)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("SELECT [iAutoId] as LinId");
                strSql.AppendLine("      ,[vcSHF] as vcReceiver");
                strSql.AppendLine("      ,[vcPart_id] as vcPartId");
                strSql.AppendLine("      ,[vcOrderNo] as vcOrderNo");
                strSql.AppendLine("      ,[vcLianFanNo] as vcLianFanNo");
                strSql.AppendLine("      ,[vcInvoiceNo] as vcInvoiceNo");
                strSql.AppendLine("      ,[vcBoxNo] as vcBoxNo ");
                strSql.AppendLine("      ,[vcPartsNameEN] as vcPartENName");
                strSql.AppendLine("      ,[iQuantity] as iQuantity");
                strSql.AppendLine("      ,[decPriceWithTax] as decPriceWithTax");
                strSql.AppendLine("      ,[decMoney] as decMoney");
                strSql.AppendLine("	  ,'1' AS bSelectFlag");
                strSql.AppendLine("  FROM [TSell]");
                strSql.AppendLine("  WHERE vcSellNo ='"+ strSellNo + "' ");
                strSql.AppendLine("  ORDER BY [vcSHF],[vcOrderNo],[vcLianFanNo]");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
