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
    public class FS0617_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        public DataTable getPlantInfo()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select '1' as plantcode,'泰达' as plantname");//plantcode\plantname
                strSql.AppendLine("");
                strSql.AppendLine("");
                strSql.AppendLine("");
                strSql.AppendLine("");
                strSql.AppendLine("");
                strSql.AppendLine("");
                strSql.AppendLine("");
                strSql.AppendLine("");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getCarTypeInfo()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select '1' as cartypecode,'卡罗拉' as cartypename");//cartypecode\cartypename
                strSql.AppendLine("");
                strSql.AppendLine("");
                strSql.AppendLine("");
                strSql.AppendLine("");
                strSql.AppendLine("");
                strSql.AppendLine("");
                strSql.AppendLine("");
                strSql.AppendLine("");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getRePartyInfo()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select '1' as repartycode,'一丰补给' as repartyname");//repartycode\repartyname
                strSql.AppendLine("");
                strSql.AppendLine("");
                strSql.AppendLine("");
                strSql.AppendLine("");
                strSql.AppendLine("");
                strSql.AppendLine("");
                strSql.AppendLine("");
                strSql.AppendLine("");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getSuPartyInfo()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select vcSupplier_id as vcName,vcSupplier_id as vcValue from TSupplierInfo");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getSearchInfo(string strPlantArea, string vcFZPlant, string strPartId, string strCarType, string strReParty, string strSuparty)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select '1' as Enum ,t1.[iAutoId] as LinId,vcPlantArea,T3.vcFZGC as vcFZPlant,vcPartId,vcFromTime,vcToTime,vcCarType,vcReparty,");
                strSql.AppendLine("vcSuparty,t2.vcDock as vcReceiving,vcBF,T4.vcQty as vcQuantity,vcPartName,");
                strSql.AppendLine("vcRoute,vcAdWorks,vcAdWorksTime,vcOutPutTime,vcItem1,vcItem2 from ");
                strSql.AppendLine("(SELECT iAutoId,[PLANTCODE] as vcPlantArea,vcPlantCode as vcFZPlant,[PARTSNO] as vcPartId,[TIMEFROM] as vcFromTime,[TIMETO] as vcToTime,");
                strSql.AppendLine("[CARFAMILYCODE] as vcCarType,");
                strSql.AppendLine("[CPDCOMPANY] as vcReparty,[SUPPLIERCODE] as vcSuparty,[vcDockCode] as vcReceiving,'' as vcBF,vcQtyCode as vcQuantity,");
                strSql.AppendLine("[PARTSNAMEEN] as vcPartName,[ROUTE] as vcRoute,FORMERPROCESS as vcAdWorks,PASSINGTIME as vcAdWorksTime,SHIPPINGTIME as vcOutPutTime,");
                strSql.AppendLine("REMARK1 as vcItem1,REMARK2	as 	vcItem2 ");
                strSql.AppendLine("from [SP_M_SITEM])T1");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select * from SP_M_SITEM_Dock)T2");
                strSql.AppendLine("on T1.vcReceiving =T2.LinId and T2.dFromTime<=Convert(varchar(10),GETDATE(),23) and T2.dToTime>=Convert(varchar(10),GETDATE(),23)");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select * from SP_M_SITEM_Plant)T3");
                strSql.AppendLine("on T1.vcFZPlant =T3.LinId and T3.dFromTime<=Convert(varchar(10),GETDATE(),23) and T3.dToTime>=Convert(varchar(10),GETDATE(),23)");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select * from SP_M_SITEM_Qty)T4");
                strSql.AppendLine("on T1.vcQuantity =T4.LinId and T4.dFromTime<=Convert(varchar(10),GETDATE(),23) and T4.dToTime>=Convert(varchar(10),GETDATE(),23)");
                strSql.AppendLine("where 1=1");
                if (strPlantArea != "")
                {
                    strSql.AppendLine("and t1.vcPlantArea='"+ strPlantArea + "'");
                }
                if (vcFZPlant != "")
                {
                    strSql.AppendLine("and T3.vcFZGC='"+ vcFZPlant + "'");
                }
                if (strPartId != "")
                {
                    strSql.AppendLine("and t1.vcPartId like '"+ strPartId + "%'");
                }
                if (strCarType != "")
                {
                    strSql.AppendLine("and t1.vcCarType='"+ strCarType + "'");
                }
                if (strReParty != "")
                {
                    strSql.AppendLine("and t1.vcReparty='"+ strReParty + "'");
                }
                if (strSuparty != "")
                {
                    strSql.AppendLine("and t1.vcSuparty='"+ strSuparty + "'");
                }
                strSql.AppendLine("order by T3.vcFZGC,t2.vcDock,t1.vcPartId");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
