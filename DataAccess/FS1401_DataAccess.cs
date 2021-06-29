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
    public class FS1401_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        public DataTable getSearchInfo(string strPartId, string strSupplierId, string strSupplierPlant, string strHaoJiu, string strInOut, string strOrderPlant, string strFrom, string strTo, string strCarModel, string strCheckType, string strSPISStatus, List<Object> listTime)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select T1.LinId AS LinId");
                strSql.AppendLine("		,T1.vcPartId AS vcPartId");
                strSql.AppendLine("		,T1.vcPartENName AS vcPartENName");
                strSql.AppendLine("		,convert(varchar(10),t1.dFromTime,111) AS dFromTime");
                strSql.AppendLine("		,convert(varchar(10),t1.dToTime,111) AS dToTime");
                strSql.AppendLine("		,T1.vcCarfamilyCode AS vcCarfamilyCode");
                strSql.AppendLine("		,T2.vcName AS vcOrderPlant");
                strSql.AppendLine("		,T1.vcSupplierId AS vcSupplierId");
                strSql.AppendLine("		,T1.vcSupplierPlant AS vcSupplierPlant");
                strSql.AppendLine("		,T3.vcName AS vcInOut");
                strSql.AppendLine("		,T4.vcName AS vcHaoJiu");
                strSql.AppendLine("		,T5.vcName AS vcPackType");
                strSql.AppendLine("		,T6.vcCheckP AS vcCheckType");
                strSql.AppendLine("		,T7.vcName AS vcSPISStatus");
                strSql.AppendLine("		,T1.vcOESP AS vcOESP");
                strSql.AppendLine("		,T1.vcStateFX AS vcStateFX");
                strSql.AppendLine("		,T1.vcFXNO AS vcFXNO");
                strSql.AppendLine("		,'0' as bModFlag,'0' as bAddFlag,'1' as bSelectFlag");
                strSql.AppendLine("		from ");
                strSql.AppendLine("(select * from tCheckMethod_Master");
                strSql.AppendLine("WHERE 1=1");
                if (strPartId != "")
                {
                    strSql.AppendLine("AND vcPartId like '" + strPartId + "%'");
                }
                if (strSupplierId != "")
                {
                    strSql.AppendLine("AND vcSupplierId='" + strSupplierId + "'");
                }
                if (strSupplierPlant != "")
                {
                    strSql.AppendLine("AND vcSupplierPlant='" + strSupplierPlant + "'");
                }
                if (strHaoJiu != "")
                {
                    strSql.AppendLine("AND vcHaoJiu='" + strHaoJiu + "'");
                }
                if (strInOut != "")
                {
                    strSql.AppendLine("AND vcInOut='" + strInOut + "'");
                }
                if (strOrderPlant != "")
                {
                    strSql.AppendLine("AND vcPartArea='" + strOrderPlant + "'");
                }
                if (strSPISStatus != "")
                {
                    strSql.AppendLine("AND vcSPISStatus='" + strSPISStatus + "'");
                }
                if (strFrom != "" || strTo != "")
                {
                    if (strFrom.Length != 0)
                    {
                        strSql.AppendLine("AND (dFromTime> '" + strFrom + "' or dFromTime= '" + strFrom + "')");
                    }
                    if (strTo.Length != 0)
                    {
                        strSql.AppendLine("AND (dToTime<'" + strTo + "' or  dToTime='" + strTo + "')");
                    }
                }
                if (strCarModel != "")
                {
                    strSql.AppendLine("AND vcCarfamilyCode='" + strCarModel + "'");
                }
                if (listTime.Count != 0)
                {
                    strSql.AppendLine("and ( ");
                    for (int i = 0; i < listTime.Count; i++)
                    {
                        if (listTime[i].ToString() == "现在")
                        {
                            strSql.AppendLine("(dFromTime<=Convert(varchar(10),getdate(),23) and dToTime>=Convert(varchar(10),getdate(),23))");
                        }
                        if (listTime[i].ToString() == "将来")
                        {
                            strSql.AppendLine("(dFromTime>Convert(varchar(10),getdate(),23))");
                        }
                        if (listTime[i].ToString() == "作废")
                        {
                            strSql.AppendLine("(dToTime<Convert(varchar(10),getdate(),23))");
                        }
                        if (i < listTime.Count - 1)
                        {
                            strSql.AppendLine(" or ");
                        }
                    }
                    strSql.AppendLine(") ");
                }
                strSql.AppendLine(")T1");
                strSql.AppendLine("left join");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C000')T2");
                strSql.AppendLine("ON T1.vcPartArea=T2.vcValue");
                strSql.AppendLine("left join");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C003')T3");
                strSql.AppendLine("ON T1.vcInOut=T3.vcValue");
                strSql.AppendLine("left join");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C004')T4");
                strSql.AppendLine("ON T1.vcHaoJiu=T4.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C059')T5");
                strSql.AppendLine("on t1.vcPackType=t5.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C067')T7");
                strSql.AppendLine("on t1.vcSPISStatus=T7.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT [vcPartId],[vcTimeFrom],[vcTimeTo],[vcCarfamilyCode],[vcSupplierCode],[vcSupplierPlant],[vcCheckP] ");
                strSql.AppendLine("FROM [tCheckQf] where [vcTimeFrom]<=convert(varchar(10),GETDATE(),23) and [vcTimeTo]>=convert(varchar(10),GETDATE(),23))T6");
                strSql.AppendLine("ON T1.vcPartId=T6.vcPartId AND T1.vcSupplierId=T6.[vcSupplierCode]");
                strSql.AppendLine("where 1=1");
                if (strCheckType != "")
                {
                    strSql.AppendLine("and t6.vcCheckP='" + strCheckType + "'");
                }
                strSql.AppendLine("order by t1.vcPartId,t1.dFromTime");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
