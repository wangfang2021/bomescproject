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
    public class FS1104_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        public string getCaseNoInfo(string strOrderPlant, string strReceiver, string strPackingPlant, string strLianFan)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("declare @LianFanNo int ");
                strSql.AppendLine("set @LianFanNo=(select vcLianFanNo from TCaseNoInfo where vcPlant='" + strOrderPlant + "' and vcReParty='" + strReceiver + "' and vcPackPlant='" + strPackingPlant + "')");
                strSql.AppendLine("if((select count(*) from TCaseNoInfo where vcPlant='" + strOrderPlant + "' and vcReParty='" + strReceiver + "' and vcPackPlant='" + strPackingPlant + "')=0)");
                strSql.AppendLine("begin");
                strSql.AppendLine("select '0000001' as vcLianFanNo");
                strSql.AppendLine("end");
                strSql.AppendLine("else");
                strSql.AppendLine("begin");
                strSql.AppendLine("select substring(cast(10000001+@LianFanNo as varchar(8)),2,7)  as vcLianFanNo");
                strSql.AppendLine("end");
                DataTable data = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                return data.Rows[0]["vcLianFanNo"].ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getCaseNoInfo(string strCaseNo)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select a.vcPlant,b.vcName as vcPlantName,a.vcReParty,c.vcName as vcRePartyName");
                strSql.AppendLine(",a.vcPackPlant,c.vcName as vcPackPlantName,'' as vcPrintNum,vcLianFanNo as vcPrintIndex from");
                strSql.AppendLine("(select  * from TCaseNoInfo)a");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select * from TCode where vcCodeId='C000')b");
                strSql.AppendLine("on a.vcPlant=b.vcValue");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select vcValue,vcName from TCustomerInfo where vcDisting='C' and vcDisable='0')c");
                strSql.AppendLine("on a.vcReParty=c.vcValue");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select * from TCode where vcCodeId='C023')d");
                strSql.AppendLine("on a.vcPackPlant=d.vcValue");
                strSql.AppendLine("where vcCaseNo='"+ strCaseNo + "'");
                DataTable data = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
