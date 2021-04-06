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
    public class FS0603_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        public DataTable getCodeInfo(string strType)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                if (strType == "Supplier")//供应商
                {
                    strSql.AppendLine("select vcSupplier_id as vcName,vcSupplier_id as vcValue from TSupplierInfo");
                }
                if (strType == "Receiver")//收货方
                {
                    strSql.AppendLine("select vcValue,vcValue as vcName from TCustomerInfo where vcDisting='C' and vcDisable='0'");
                }
                if (strType == "NqState")//纳期状态
                {
                    strSql.AppendLine("select vcValue,vcName from TCode where vcCodeId='C056' order by vcValue");
                }
                if (strType == "Role")//角色
                {
                    strSql.AppendLine("select vcRoleID as vcValue,vcRoleName as vcName from SRole");
                }
                if (strType == "TPMRelation")//品目
                {
                    strSql.AppendLine("select distinct vcBigPM as vcValue,vcBigPM as vcName from TPMRelation");
                }
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable getFormOptions(string strInOut)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("UPDATE [TSPMaster] SET vcMandOrder='0' WHERE vcMandOrder IS NULL");
                strSql.AppendLine("SELECT distinct ISNULL(T1.vcChanges,'--') AS vcChanges_Value,");
                strSql.AppendLine("		ISNULL(T6.vcName,'--') AS vcChanges_Name,");
                strSql.AppendLine("		ISNULL(T1.vcPackingPlant,'--') AS vcPackingPlant_Value,");
                strSql.AppendLine("		ISNULL(T7.vcName,'--') AS vcPackingPlant_Name,");
                strSql.AppendLine("		ISNULL(T1.vcCarfamilyCode,'--') AS vcCarModel_Value,");
                strSql.AppendLine("		ISNULL(T1.vcCarfamilyCode,'--') AS vcCarModel_Name,");
                //strSql.AppendLine("		ISNULL(T1.vcCarModel,'--') AS vcCarModel_Value,");
                //strSql.AppendLine("		ISNULL(T1.vcCarModel,'--') AS vcCarModel_Name,");
                strSql.AppendLine("		ISNULL(T1.vcReceiver,'--') AS vcReceiver_Value,");
                strSql.AppendLine("		ISNULL(T1.vcReceiver,'--') AS vcReceiver_Name,");
                strSql.AppendLine("		ISNULL(T1.vcInOut,'--') AS vcInOut_Value,");
                strSql.AppendLine("		ISNULL(T8.vcName,'--') AS vcInOut_Name,");
                strSql.AppendLine("		ISNULL(T1.vcSupplierId,'--') AS vcSupplierId_Value,");
                strSql.AppendLine("		ISNULL(T1.vcSupplierId,'--') AS vcSupplierId_Name,");
                strSql.AppendLine("		ISNULL(T2.vcSupplierPlant,'--') AS vcSupplierPlant_Value,");
                strSql.AppendLine("		ISNULL(T2.vcSupplierPlant,'--') AS vcSupplierPlant_Name,");
                strSql.AppendLine("		ISNULL(SUBSTRING(T3.vcBoxType,1,2),'--') AS vcBoxType_Value,");
                strSql.AppendLine("		ISNULL(SUBSTRING(T3.vcBoxType,1,2),'--') AS vcBoxType_Name,");
                strSql.AppendLine("		ISNULL(CONVERT(VARCHAR(10),T1.dFromTime,111),'--') AS vcFromTime_Value,");
                strSql.AppendLine("		ISNULL(CONVERT(VARCHAR(10),T1.dFromTime,111),'--') AS vcFromTime_Name,");
                strSql.AppendLine("		ISNULL(CONVERT(VARCHAR(10),T1.dToTime,111),'--') AS vcToTime_Value,");
                strSql.AppendLine("		ISNULL(CONVERT(VARCHAR(10),T1.dToTime,111),'--') AS vcToTime_Name,");
                strSql.AppendLine("		ISNULL(T1.vcHaoJiu,'--') AS vcHaoJiu_Value,");
                strSql.AppendLine("		ISNULL(T9.vcName,'--') AS vcHaoJiu_Name,");
                strSql.AppendLine("		ISNULL(T5.vcOrderPlant,'--') AS vcOrderPlant_Value,");
                strSql.AppendLine("		ISNULL(T10.vcName,'--') AS vcOrderPlant_Name,");
                strSql.AppendLine("		ISNULL(T4.vcSufferIn,'--') AS vcSufferIn_Value,");
                strSql.AppendLine("		ISNULL(T4.vcSufferIn,'--') AS vcSufferIn_Name,");
                strSql.AppendLine("		ISNULL(T1.vcSupplierPacking,'--') AS vcSupplierPacking_Value,");
                strSql.AppendLine("		ISNULL(T11.vcName,'--') AS vcSupplierPacking_Name,");
                strSql.AppendLine("		ISNULL(T1.vcOldProduction,'--') AS vcOldProduction_Value,");
                strSql.AppendLine("		ISNULL(T1.vcOldProduction,'--') AS vcOldProduction_Name,");
                strSql.AppendLine("		ISNULL(CONVERT(VARCHAR(7),T1.dDebugTime,111),'--') AS vcDebugTime_Value,");
                strSql.AppendLine("		ISNULL(CONVERT(VARCHAR(7),T1.dDebugTime,111),'--') AS vcDebugTime_Name,");
                strSql.AppendLine("		ISNULL(T1.vcOrderingMethod,'--') AS vcOrderingMethod_Value,");
                strSql.AppendLine("		ISNULL(T12.vcName,'--') AS vcOrderingMethod_Name,");
                strSql.AppendLine("		ISNULL(T1.vcMandOrder,'--') AS vcMandOrder_Value,");
                strSql.AppendLine("		ISNULL(T13.vcName,'--') AS vcMandOrder_Name");
                strSql.AppendLine("		FROM ");
                strSql.AppendLine("(SELECT * FROM [TSPMaster] WHERE isnull(vcDelete, '') <> '1'");
                if (strInOut != "")
                {
                    strSql.AppendLine("AND vcInOut='" + strInOut + "'");
                }
                strSql.AppendLine(")T1");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT * FROM [TSPMaster_SupplierPlant] WHERE [vcOperatorType]='1' AND [dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))T2");
                strSql.AppendLine("ON T1.[vcPackingPlant]=T2.[vcPackingPlant] AND T1.[vcPartId]=T2.[vcPartId] AND T1.[vcReceiver]=T2.[vcReceiver] AND T1.[vcSupplierId]=T2.[vcSupplierId]");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT * FROM [TSPMaster_Box] WHERE [vcOperatorType]='1' AND [dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))T3");
                strSql.AppendLine("ON T1.[vcPackingPlant]=T3.[vcPackingPlant] AND T1.[vcPartId]=T3.[vcPartId] AND T1.[vcReceiver]=T3.[vcReceiver] AND T1.[vcSupplierId]=T3.[vcSupplierId]");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT * FROM [TSPMaster_SufferIn] WHERE [vcOperatorType]='1' AND [dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))T4");
                strSql.AppendLine("ON T1.[vcPackingPlant]=T4.[vcPackingPlant] AND T1.[vcPartId]=T4.[vcPartId] AND T1.[vcReceiver]=T4.[vcReceiver] AND T1.[vcSupplierId]=T4.[vcSupplierId]");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(select vcValue1 as [vcSupplierId],vcValue2 as vcSupplierPlant,vcValue3 as [dFromTime],vcValue4 as [dToTime],vcValue5 as vcOrderPlant from TOutCode where vcCodeId='C010' and vcIsColum='0'");
                strSql.AppendLine("and vcValue3<=CONVERT(VARCHAR(10),GETDATE(),23) AND vcValue4>=CONVERT(VARCHAR(10),GETDATE(),23))T5");
                strSql.AppendLine("ON T1.[vcSupplierId]=T5.[vcSupplierId] AND T2.vcSupplierPlant=T5.vcSupplierPlant");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C002')T6--变更事项");
                strSql.AppendLine("ON T1.vcChanges=T6.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C017')T7--包装工厂");
                strSql.AppendLine("ON T1.vcPackingPlant=T7.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C003')T8--内外区分");
                strSql.AppendLine("ON T1.vcInOut=T8.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C004')T9--号旧区分");
                strSql.AppendLine("ON T1.vcHaoJiu=T9.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C000')T10--发注工厂");
                strSql.AppendLine("ON T5.vcOrderPlant=T10.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C059')T11--供应商包装");
                strSql.AppendLine("ON T1.vcSupplierPacking=T11.vcValue");
                //strSql.AppendLine("LEFT JOIN");
                //strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C024')T12--旧型年限生产区分");
                //strSql.AppendLine("ON T1.vcOldProduction=T12.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C047')T12--订货方式");
                strSql.AppendLine("ON T1.vcOrderingMethod=T12.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C048')T13--强制订货");
                strSql.AppendLine("ON isnull(T1.vcMandOrder,'0')=T13.vcValue");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getSearchInfo(string strSyncTime_from, string strSyncTime_to, string strChanges, string strPartId, string strCarModel, string strReceiver, string strInOut, string strHaoJiu, string strSupplierId, string strSupplierPlant,
                    string strOrderPlant, string strFromTime, string strToTime, string strBoxType, string strSufferIn, string strSupplierPacking, string strOldProduction, string strDebugTime, string strPackingPlant, bool bCheck, string strOrderby, 
                    string strOrderingMethod, string strMandOrder, string strSPChild, bool bNow)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("SELECT  T1.LinId,");
                strSql.AppendLine("		Convert(varchar(10),T1.dSyncTime,111) as dSyncTime,");
                strSql.AppendLine("		T1.vcChanges as vcChanges,T6.vcName as vcChanges_name,");
                strSql.AppendLine("		T1.vcPackingPlant,T7.vcName as vcPackingPlant_name,");
                strSql.AppendLine("		T1.vcPartId,");
                strSql.AppendLine("		T1.vcPartENName,");
                strSql.AppendLine("		T1.vcCarfamilyCode,");
                strSql.AppendLine("		T1.vcReceiver,");
                strSql.AppendLine("		Convert(varchar(10),T1.dFromTime,111) as dFromTime,");
                strSql.AppendLine("		Convert(varchar(10),T1.dToTime,111) as dToTime,");
                strSql.AppendLine("		T1.vcPartId_Replace,");
                strSql.AppendLine("		T1.vcInOut,T8.vcName as vcInOut_name,");
                strSql.AppendLine("		T1.vcOESP,T9.vcName as vcOESP_name,");
                strSql.AppendLine("		T1.vcHaoJiu,T10.vcName as vcHaoJiu_name,");
                strSql.AppendLine("		T1.vcOldProduction,T1.vcOldProduction as vcOldProduction_name,");
                //strSql.AppendLine("		T1.vcOldProduction,T11.vcName as vcOldProduction_name,");
                strSql.AppendLine("		CONVERT(VARCHAR(7),T1.dDebugTime,111) AS dDebugTime,");
                strSql.AppendLine("		T1.vcSupplierId,");
                strSql.AppendLine("		Convert(varchar(10),T1.dSupplierFromTime,111) as dSupplierFromTime,");
                strSql.AppendLine("		Convert(varchar(10),T1.dSupplierToTime,111) as dSupplierToTime,");
                strSql.AppendLine("		T1.vcSupplierName,");
                strSql.AppendLine("		T2.vcSupplierPlant,");
                strSql.AppendLine("		T2.LinId as SupplierPlantLinId_ed,");
                strSql.AppendLine("		T2.vcSupplierPlant as SupplierPlant_ed,");
                strSql.AppendLine("		Convert(varchar(10),T2.dFromTime,111) as SupplierPlantFromTime_ed,");
                strSql.AppendLine("		Convert(varchar(10),t2.dToTime,111) as SupplierPlantToTime_ed,		");
                strSql.AppendLine("		CAST(T3.iPackingQty AS varchar(10)) AS iPackingQty,");
                //strSql.AppendLine("		'' as vcBgColor_SupplierPlant,");
                strSql.AppendLine("		T1.vcSupplierPlace as vcSupplierPlace,");
                strSql.AppendLine("		T3.vcBoxType,T3.iLength,T3.iWidth,T3.iHeight,T3.iVolume,");
                strSql.AppendLine("		T3.LinId as BoxLinId_ed,");
                strSql.AppendLine("		CAST(T3.iPackingQty AS varchar(10)) AS BoxPackingQty_ed,");
                strSql.AppendLine("		Convert(varchar(10),T3.dFromTime,111) as BoxFromTime_ed,");
                strSql.AppendLine("		Convert(varchar(10),T3.dToTime,111) as BoxToTime_ed,");
                strSql.AppendLine("		T3.vcBoxType as BoxType_ed,T3.iLength as BoxLength_ed,T3.iWidth as BoxWidth_ed,T3.iHeight as BoxHeight_ed,T3.iVolume as BoxVolume_ed,");
                //strSql.AppendLine("		'' as vcBgColor_BoxType,");
                strSql.AppendLine("		T4.vcSufferIn,");
                strSql.AppendLine("		T4.LinId as SufferInLinId_ed,");
                strSql.AppendLine("		T4.vcSufferIn as SufferIn_ed,");
                strSql.AppendLine("		Convert(varchar(10),T4.dFromTime,111) as SufferInFromTime_ed,");
                strSql.AppendLine("		Convert(varchar(10),T4.dToTime,111) as SufferInToTime_ed,");
                //strSql.AppendLine("		'' as vcBgColor_SufferIn,");
                strSql.AppendLine("		T5.vcOrderPlant,");
                strSql.AppendLine("		T12.vcName as vcOrderPlant_name,		");
                strSql.AppendLine("		T1.vcInteriorProject,");
                strSql.AppendLine("		T1.vcPassProject,");
                strSql.AppendLine("		T1.vcFrontProject,");
                strSql.AppendLine("		Convert(varchar(16),T1.dFrontProjectTime,111) as dFrontProjectTime,");
                strSql.AppendLine("		Convert(varchar(16),T1.dShipmentTime,111) as dShipmentTime,");
                strSql.AppendLine("		T1.vcPartImage AS vcPartImage,");
                strSql.AppendLine("		T1.vcBillType,");
                strSql.AppendLine("		T13.vcName as vcBillType_name,");
                strSql.AppendLine("		T1.vcRemark1,");
                strSql.AppendLine("		T1.vcRemark2,");
                strSql.AppendLine("		T1.vcOrderingMethod,");
                strSql.AppendLine("		T14.vcName as vcOrderingMethod_name,");
                strSql.AppendLine("		T1.vcMandOrder,");
                strSql.AppendLine("		T15.vcName as vcMandOrder_name,");
                strSql.AppendLine("		T1.vcSupplierPacking,");
                strSql.AppendLine("		T16.vcName as vcSupplierPacking_name,");
                strSql.AppendLine("		'0' as bModFlag,'0' as bAddFlag,'1' as bSelectFlag,'' as vcBgColor  FROM ");
                strSql.AppendLine("(SELECT * FROM [TSPMaster] WHERE 1=1 ");
                if (!bCheck)
                {
                    strSql.AppendLine(" and isnull(vcDelete, '') <> '1'");
                }
                if (bNow)
                {
                    strSql.AppendLine(" AND (([dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))");
                    strSql.AppendLine(" OR ([dFromTime]>=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))) ");
                }
                if (strSyncTime_from != "" && strSyncTime_to != "")
                {
                    strSql.AppendLine("    AND (CONVERT(varchar(10),[dSyncTime],111)>='" + strSyncTime_from + "' AND CONVERT(varchar(10),[dSyncTime],111)<='" + strSyncTime_to + "')");
                }
                if (strPackingPlant != "")
                {
                    if (strPackingPlant == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcPackingPlant,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND vcPackingPlant='" + strPackingPlant + "'");
                    }
                }
                if (strChanges != "")
                {
                    if (strChanges == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcChanges,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND vcChanges='" + strChanges + "'");
                    }
                }
                if (strPartId != "")
                {
                    strSql.AppendLine("    AND [vcPartId] like '" + strPartId + "%'");
                }
                if (strCarModel != "")
                {
                    if (strCarModel == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcCarfamilyCode,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [vcCarfamilyCode]='" + strCarModel + "'");
                    }

                }
                if (strReceiver != "")
                {
                    if (strReceiver == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcReceiver,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [vcReceiver]='" + strReceiver + "'");
                    }
                }
                if (strInOut != "")
                {
                    if (strInOut == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcInOut,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [vcInOut]='" + strInOut + "'");
                    }
                }
                if (strHaoJiu != "")
                {
                    if (strHaoJiu == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcHaoJiu,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [vcHaoJiu]='" + strHaoJiu + "'");
                    }
                }
                if (strSupplierId != "")
                {
                    if (strSupplierId == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcSupplierId,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [vcSupplierId]='" + strSupplierId + "'");
                    }
                }
                if (strFromTime != "")
                {
                    if (strFromTime == "--")
                    {
                        strSql.AppendLine("    AND isnull(dFromTime,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [dFromTime]='" + strFromTime + "'");
                    }
                }
                if (strToTime != "")
                {
                    if (strToTime == "--")
                    {
                        strSql.AppendLine("    AND isnull(dToTime,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [dToTime]='" + strToTime + "'");
                    }
                }
                if (strSupplierPacking != "")
                {
                    if (strSupplierPacking == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcSupplierPacking,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [vcSupplierPacking]='" + strSupplierPacking + "'");
                    }
                }
                if (strOldProduction != "")
                {
                    if (strOldProduction == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcOldProduction,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [vcOldProduction]='" + strOldProduction + "'");
                    }
                }
                if (strDebugTime != "")
                {
                    if (strDebugTime == "--")
                    {
                        strSql.AppendLine("    AND isnull(dDebugTime,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND CONVERT(varchar(7),[dDebugTime],111)='" + strDebugTime + "'");
                    }
                }
                if (strOrderingMethod != "")
                {
                    if (strOrderingMethod == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcOrderingMethod,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [vcOrderingMethod]='" + strOrderingMethod + "'");
                    }
                }
                if (strMandOrder != "")
                {
                    if (strMandOrder == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcMandOrder,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [vcMandOrder]='" + strMandOrder + "'");
                    }
                }
                strSql.AppendLine("  )T1");
                //strSql.AppendLine("LEFT JOIN");
                //strSql.AppendLine("(SELECT *  FROM [TSPMaster_SupplierPlant] WHERE [vcOperatorType]='1' AND [dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))T2");
                //strSql.AppendLine("ON T1.[vcPackingPlant]=T2.[vcPackingPlant] AND T1.[vcPartId]=T2.[vcPartId] AND T1.[vcReceiver]=T2.[vcReceiver] AND T1.[vcSupplierId]=T2.[vcSupplierId]");
                //strSql.AppendLine("LEFT JOIN");
                //strSql.AppendLine("(SELECT *  FROM [TSPMaster_Box] WHERE [vcOperatorType]='1' AND [dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))T3");
                //strSql.AppendLine("ON T1.[vcPackingPlant]=T3.[vcPackingPlant] AND T1.[vcPartId]=T3.[vcPartId] AND T1.[vcReceiver]=T3.[vcReceiver] AND T1.[vcSupplierId]=T3.[vcSupplierId]");
                //strSql.AppendLine("LEFT JOIN");
                //strSql.AppendLine("(SELECT *  FROM [TSPMaster_SufferIn] WHERE [vcOperatorType]='1' AND [dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))T4");
                //strSql.AppendLine("ON T1.[vcPackingPlant]=T4.[vcPackingPlant] AND T1.[vcPartId]=T4.[vcPartId] AND T1.[vcReceiver]=T4.[vcReceiver] AND T1.[vcSupplierId]=T4.[vcSupplierId] ");
                //strSql.AppendLine("LEFT JOIN");
                //strSql.AppendLine("(select vcValue1 as [vcSupplierId],vcValue2 as vcSupplierPlant,vcValue3 as [dFromTime],vcValue4 as [dToTime],vcValue5 as vcOrderPlant from TOutCode where vcCodeId='C010' and vcIsColum='0'");
                //strSql.AppendLine("and vcValue3<=CONVERT(VARCHAR(10),GETDATE(),23) AND vcValue4>=CONVERT(VARCHAR(10),GETDATE(),23))T5");
                //strSql.AppendLine("ON T1.[vcSupplierId]=T5.[vcSupplierId] AND T2.vcSupplierPlant=T5.vcSupplierPlant");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT ");
                strSql.AppendLine("ROW_NUMBER() OVER(partition by vcPackingPlant,vcPartId,vcReceiver,vcSupplierId order by dFromTime) [rank],");
                strSql.AppendLine("LinId,vcPackingPlant,vcPartId,vcReceiver,vcSupplierId,dFromTime,dToTime,vcSupplierPlant ");
                strSql.AppendLine("FROM [TSPMaster_SupplierPlant] ");
                strSql.AppendLine("WHERE [vcOperatorType]='1' AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23)");
                strSql.AppendLine(")T2");
                strSql.AppendLine("ON T2.rank='1' AND T1.[vcPackingPlant]=T2.[vcPackingPlant] AND T1.[vcPartId]=T2.[vcPartId] AND T1.[vcReceiver]=T2.[vcReceiver] AND T1.[vcSupplierId]=T2.[vcSupplierId]");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT ");
                strSql.AppendLine("ROW_NUMBER() OVER(partition by vcPackingPlant,vcPartId,vcReceiver,vcSupplierId order by dFromTime) [rank],");
                strSql.AppendLine("LinId,vcPackingPlant,vcPartId,vcReceiver,vcSupplierId,dFromTime,dToTime,iPackingQty,vcBoxType,iLength,iWidth,iHeight,iVolume ");
                strSql.AppendLine("FROM [TSPMaster_Box] ");
                strSql.AppendLine("WHERE [vcOperatorType]='1' AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23)");
                strSql.AppendLine(")T3");
                strSql.AppendLine("ON T3.rank='1' AND T1.[vcPackingPlant]=T3.[vcPackingPlant] AND T1.[vcPartId]=T3.[vcPartId] AND T1.[vcReceiver]=T3.[vcReceiver] AND T1.[vcSupplierId]=T3.[vcSupplierId]");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT ");
                strSql.AppendLine("ROW_NUMBER() OVER(partition by vcPackingPlant,vcPartId,vcReceiver,vcSupplierId order by dFromTime) [rank],");
                strSql.AppendLine("LinId,vcPackingPlant,vcPartId,vcReceiver,vcSupplierId,dFromTime,dToTime,vcSufferIn");
                strSql.AppendLine("FROM [TSPMaster_SufferIn] ");
                strSql.AppendLine("WHERE [vcOperatorType]='1' AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23)");
                strSql.AppendLine(")T4");
                strSql.AppendLine("ON T4.rank='1' AND T1.[vcPackingPlant]=T4.[vcPackingPlant] AND T1.[vcPartId]=T4.[vcPartId] AND T1.[vcReceiver]=T4.[vcReceiver] AND T1.[vcSupplierId]=T4.[vcSupplierId] ");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT ");
                strSql.AppendLine("ROW_NUMBER() OVER(partition by vcSupplierId,vcSupplierPlant order by dFromTime) [rank],");
                strSql.AppendLine("vcSupplierId,vcSupplierPlant,dFromTime,dToTime,vcOrderPlant");
                strSql.AppendLine("FROM (select vcValue1 as [vcSupplierId],vcValue2 as vcSupplierPlant,vcValue3 as [dFromTime],vcValue4 as [dToTime],vcValue5 as vcOrderPlant from TOutCode where vcCodeId='C010' and vcIsColum='0'");
                strSql.AppendLine("and vcValue3<=CONVERT(VARCHAR(10),GETDATE(),23) AND vcValue4>=CONVERT(VARCHAR(10),GETDATE(),23))a ");
                strSql.AppendLine("WHERE [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))T5");
                strSql.AppendLine("ON t5.[rank]='1' AND T1.[vcSupplierId]=T5.[vcSupplierId] AND T2.vcSupplierPlant=T5.vcSupplierPlant");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C002')T6--变更事项");
                strSql.AppendLine("ON T1.vcChanges=T6.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C017')T7--包装工厂");
                strSql.AppendLine("ON T1.vcPackingPlant=T7.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C003')T8--内外区分");
                strSql.AppendLine("ON T1.vcInOut=T8.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C012')T9--OE=SP");
                strSql.AppendLine("ON T1.vcOESP=T9.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C004')T10--号旧区分");
                strSql.AppendLine("ON T1.vcHaoJiu=T10.vcValue");
                //strSql.AppendLine("LEFT JOIN");
                //strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C024')T11--旧型年限生产区分");
                //strSql.AppendLine("ON T1.vcOldProduction=T11.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C000')T12--发注工厂");
                strSql.AppendLine("ON T5.vcOrderPlant=T12.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C007')T13--单据区分");
                strSql.AppendLine("ON T1.vcBillType=T13.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C047')T14--订货方式");
                strSql.AppendLine("ON T1.vcOrderingMethod=T14.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C048')T15--强制订货");
                strSql.AppendLine("ON T1.vcMandOrder=T15.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C059')T16--供应商包装");
                strSql.AppendLine("ON T1.vcSupplierPacking=T16.vcValue");
                strSql.AppendLine("WHERE 1=1");
                if (strSupplierPlant != "")
                {
                    if (strSupplierPlant == "--")
                    {
                        strSql.AppendLine("    AND isnull(T2.vcSupplierPlant,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("AND T2.vcSupplierPlant='" + strSupplierPlant + "'");
                    }
                }
                if (strOrderPlant != "")
                {
                    if (strOrderPlant == "--")
                    {
                        strSql.AppendLine("    AND isnull(T5.vcOrderPlant,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("AND T5.vcOrderPlant='" + strOrderPlant + "'");
                    }
                }
                if (strSufferIn != "")
                {
                    if (strSufferIn == "--")
                    {
                        strSql.AppendLine("    AND isnull(T4.vcSufferIn,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("AND T4.vcSufferIn='" + strSufferIn + "'");
                    }
                }
                if (strBoxType != "")
                {
                    if (strBoxType == "--")
                    {
                        strSql.AppendLine("    AND isnull(T3.vcBoxType,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("AND T3.vcBoxType like '" + strBoxType + "%'");
                    }
                }
                if (strSPChild != "")
                {
                    if (strSPChild == "0")//全部
                    {
                        strSql.AppendLine("    AND (ISNULL(T2.vcSupplierPlant,'')='' OR ISNULL(T3.iPackingQty,0)=0 OR ISNULL(T4.vcSufferIn,'')='' OR ISNULL(T5.vcOrderPlant,'')='')");
                    }
                    if (strSPChild == "1")//工区
                    {
                        strSql.AppendLine("    AND (ISNULL(T2.vcSupplierPlant,'')='')");
                    }
                    if (strSPChild == "2")//收容数
                    {
                        strSql.AppendLine("    AND (ISNULL(T3.iPackingQty,0)=0)");
                    }
                    if (strSPChild == "3")//受入
                    {
                        strSql.AppendLine("    AND (ISNULL(T4.vcSufferIn,'')='')");
                    }
                    if (strSPChild == "4")//发注工场
                    {
                        strSql.AppendLine("    AND (ISNULL(T5.vcOrderPlant,'')='')");
                    }
                }
                //strSql.AppendLine("ORDER BY T5.vcOrderPlant,T1.vcReceiver,T1.vcPartId, T1.dFromTime, T1.vcPackingPlant");
                if (strOrderby == "1")
                {
                    strSql.AppendLine("ORDER BY T1.iOrderBy desc,T1.LinId");
                    strSql.AppendLine("update TSPMaster set iOrderBy=null");
                }
                else
                {
                    strSql.AppendLine("ORDER BY T1.LinId");
                }
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getChildInfo_gq(string strSyncTime_from, string strSyncTime_to, string strChanges, string strPartId, string strCarModel, string strReceiver, string strInOut, string strHaoJiu, string strSupplierId, string strSupplierPlant,
                     string strOrderPlant, string strFromTime, string strToTime, string strBoxType, string strSufferIn, string strSupplierPacking, string strOldProduction, string strDebugTime, string strPackingPlant, bool bCheck, string strOrderby, string strOrderingMethod, string strMandOrder, string strSPChild)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("SELECT  T1.LinId,");
                strSql.AppendLine("		T1.vcPackingPlant,");
                strSql.AppendLine("		T1.vcPartId,");
                strSql.AppendLine("		T1.vcReceiver,");
                strSql.AppendLine("		Convert(varchar(10),T1.dFromTime,111) as dFromTime,");
                strSql.AppendLine("		Convert(varchar(10),T1.dToTime,111) as dToTime,");
                strSql.AppendLine("		T1.vcSupplierId,");
                strSql.AppendLine("		T2.LinId as SupplierPlantLinId_ed,");
                strSql.AppendLine("		T2.vcSupplierPlant as SupplierPlant_ed,");
                strSql.AppendLine("		Convert(varchar(10),T2.dFromTime,111) as SupplierPlantFromTime_ed,");
                strSql.AppendLine("		Convert(varchar(10),T2.dToTime,111) as SupplierPlantToTime_ed  FROM");
                strSql.AppendLine("(SELECT * FROM [TSPMaster] WHERE 1=1 ");
                if (!bCheck)
                {
                    strSql.AppendLine(" and isnull(vcDelete, '') <> '1'");
                }
                if (strSyncTime_from != "" && strSyncTime_to != "")
                {
                    strSql.AppendLine("    AND (CONVERT(varchar(10),[dSyncTime],111)>='" + strSyncTime_from + "' AND CONVERT(varchar(10),[dSyncTime],111)<='" + strSyncTime_to + "')");
                }
                if (strPackingPlant != "")
                {
                    if (strPackingPlant == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcPackingPlant,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND vcPackingPlant='" + strPackingPlant + "'");
                    }
                }
                if (strChanges != "")
                {
                    if (strChanges == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcChanges,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND vcChanges='" + strChanges + "'");
                    }
                }
                if (strPartId != "")
                {
                    strSql.AppendLine("    AND [vcPartId] like '" + strPartId + "%'");
                }
                if (strCarModel != "")
                {
                    if (strCarModel == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcCarfamilyCode,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [vcCarfamilyCode]='" + strCarModel + "'");
                    }

                }
                if (strReceiver != "")
                {
                    if (strReceiver == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcReceiver,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [vcReceiver]='" + strReceiver + "'");
                    }
                }
                if (strInOut != "")
                {
                    if (strInOut == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcInOut,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [vcInOut]='" + strInOut + "'");
                    }
                }
                if (strHaoJiu != "")
                {
                    if (strHaoJiu == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcHaoJiu,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [vcHaoJiu]='" + strHaoJiu + "'");
                    }
                }
                if (strSupplierId != "")
                {
                    if (strSupplierId == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcSupplierId,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [vcSupplierId]='" + strSupplierId + "'");
                    }
                }
                if (strFromTime != "")
                {
                    if (strFromTime == "--")
                    {
                        strSql.AppendLine("    AND isnull(dFromTime,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [dFromTime]='" + strFromTime + "'");
                    }
                }
                if (strToTime != "")
                {
                    if (strToTime == "--")
                    {
                        strSql.AppendLine("    AND isnull(dToTime,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [dToTime]='" + strToTime + "'");
                    }
                }
                if (strSupplierPacking != "")
                {
                    if (strSupplierPacking == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcSupplierPacking,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [vcSupplierPacking]='" + strSupplierPacking + "'");
                    }
                }
                if (strOldProduction != "")
                {
                    if (strOldProduction == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcOldProduction,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [vcOldProduction]='" + strOldProduction + "'");
                    }
                }
                if (strDebugTime != "")
                {
                    if (strDebugTime == "--")
                    {
                        strSql.AppendLine("    AND isnull(dDebugTime,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND CONVERT(varchar(7),[dDebugTime],111)='" + strDebugTime + "'");
                    }
                }
                if (strOrderingMethod != "")
                {
                    if (strOrderingMethod == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcOrderingMethod,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [vcOrderingMethod]='" + strOrderingMethod + "'");
                    }
                }
                if (strMandOrder != "")
                {
                    if (strMandOrder == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcMandOrder,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [vcMandOrder]='" + strMandOrder + "'");
                    }
                }
                strSql.AppendLine("  )T1");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT LinId,vcPackingPlant,vcPartId,vcReceiver,vcSupplierId,dFromTime,dToTime,vcSupplierPlant ");
                strSql.AppendLine("FROM [TSPMaster_SupplierPlant] ");
                strSql.AppendLine("WHERE [vcOperatorType]='1'");
                strSql.AppendLine(")T2");
                strSql.AppendLine("ON T1.[vcPackingPlant]=T2.[vcPackingPlant] AND T1.[vcPartId]=T2.[vcPartId] AND T1.[vcReceiver]=T2.[vcReceiver] AND T1.[vcSupplierId]=T2.[vcSupplierId]");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT ");
                strSql.AppendLine("ROW_NUMBER() OVER(partition by vcPackingPlant,vcPartId,vcReceiver,vcSupplierId order by dFromTime) [rank],");
                strSql.AppendLine("LinId,vcPackingPlant,vcPartId,vcReceiver,vcSupplierId,dFromTime,dToTime,iPackingQty,vcBoxType,iLength,iWidth,iHeight,iVolume ");
                strSql.AppendLine("FROM [TSPMaster_Box] ");
                strSql.AppendLine("WHERE [vcOperatorType]='1' AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23)");
                strSql.AppendLine(")T3");
                strSql.AppendLine("ON T3.rank='1' AND T1.[vcPackingPlant]=T3.[vcPackingPlant] AND T1.[vcPartId]=T3.[vcPartId] AND T1.[vcReceiver]=T3.[vcReceiver] AND T1.[vcSupplierId]=T3.[vcSupplierId]");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT ");
                strSql.AppendLine("ROW_NUMBER() OVER(partition by vcPackingPlant,vcPartId,vcReceiver,vcSupplierId order by dFromTime) [rank],");
                strSql.AppendLine("LinId,vcPackingPlant,vcPartId,vcReceiver,vcSupplierId,dFromTime,dToTime,vcSufferIn");
                strSql.AppendLine("FROM [TSPMaster_SufferIn] ");
                strSql.AppendLine("WHERE [vcOperatorType]='1' AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23)");
                strSql.AppendLine(")T4");
                strSql.AppendLine("ON T4.rank='1' AND T1.[vcPackingPlant]=T4.[vcPackingPlant] AND T1.[vcPartId]=T4.[vcPartId] AND T1.[vcReceiver]=T4.[vcReceiver] AND T1.[vcSupplierId]=T4.[vcSupplierId] ");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT ");
                strSql.AppendLine("ROW_NUMBER() OVER(partition by vcSupplierId,vcSupplierPlant order by dFromTime) [rank],");
                strSql.AppendLine("vcSupplierId,vcSupplierPlant,dFromTime,dToTime,vcOrderPlant");
                strSql.AppendLine("FROM (select vcValue1 as [vcSupplierId],vcValue2 as vcSupplierPlant,vcValue3 as [dFromTime],vcValue4 as [dToTime],vcValue5 as vcOrderPlant from TOutCode where vcCodeId='C010' and vcIsColum='0'");
                strSql.AppendLine("and vcValue3<=CONVERT(VARCHAR(10),GETDATE(),23) AND vcValue4>=CONVERT(VARCHAR(10),GETDATE(),23))a ");
                strSql.AppendLine("WHERE [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))T5");
                strSql.AppendLine("ON t5.[rank]='1' AND T1.[vcSupplierId]=T5.[vcSupplierId] AND T2.vcSupplierPlant=T5.vcSupplierPlant");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C002')T6--变更事项");
                strSql.AppendLine("ON T1.vcChanges=T6.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C017')T7--包装工厂");
                strSql.AppendLine("ON T1.vcPackingPlant=T7.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C003')T8--内外区分");
                strSql.AppendLine("ON T1.vcInOut=T8.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C012')T9--OE=SP");
                strSql.AppendLine("ON T1.vcOESP=T9.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C004')T10--号旧区分");
                strSql.AppendLine("ON T1.vcHaoJiu=T10.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C000')T12--发注工厂");
                strSql.AppendLine("ON T5.vcOrderPlant=T12.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C007')T13--单据区分");
                strSql.AppendLine("ON T1.vcBillType=T13.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C047')T14--订货方式");
                strSql.AppendLine("ON T1.vcOrderingMethod=T14.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C048')T15--强制订货");
                strSql.AppendLine("ON T1.vcMandOrder=T15.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C059')T16--供应商包装");
                strSql.AppendLine("ON T1.vcSupplierPacking=T16.vcValue");
                strSql.AppendLine("WHERE 1=1");
                if (strSupplierPlant != "")
                {
                    if (strSupplierPlant == "--")
                    {
                        strSql.AppendLine("    AND isnull(T2.vcSupplierPlant,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("AND T2.vcSupplierPlant='" + strSupplierPlant + "'");
                    }
                }
                if (strOrderPlant != "")
                {
                    if (strOrderPlant == "--")
                    {
                        strSql.AppendLine("    AND isnull(T5.vcOrderPlant,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("AND T5.vcOrderPlant='" + strOrderPlant + "'");
                    }
                }
                if (strSufferIn != "")
                {
                    if (strSufferIn == "--")
                    {
                        strSql.AppendLine("    AND isnull(T4.vcSufferIn,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("AND T4.vcSufferIn='" + strSufferIn + "'");
                    }
                }
                if (strBoxType != "")
                {
                    if (strBoxType == "--")
                    {
                        strSql.AppendLine("    AND isnull(T3.vcBoxType,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("AND T3.vcBoxType like '" + strBoxType + "%'");
                    }
                }
                if (strSPChild != "")
                {
                    if (strSPChild == "0")//全部
                    {
                        strSql.AppendLine("    AND (ISNULL(T2.vcSupplierPlant,'')='' OR ISNULL(T3.iPackingQty,0)=0 OR ISNULL(T4.vcSufferIn,'')='' OR ISNULL(T5.vcOrderPlant,'')='')");
                    }
                    if (strSPChild == "1")//工区
                    {
                        strSql.AppendLine("    AND (ISNULL(T2.vcSupplierPlant,'')='')");
                    }
                    if (strSPChild == "2")//收容数
                    {
                        strSql.AppendLine("    AND (ISNULL(T3.iPackingQty,0)=0)");
                    }
                    if (strSPChild == "3")//受入
                    {
                        strSql.AppendLine("    AND (ISNULL(T4.vcSufferIn,'')='')");
                    }
                    if (strSPChild == "4")//发注工场
                    {
                        strSql.AppendLine("    AND (ISNULL(T5.vcOrderPlant,'')='')");
                    }
                }
                strSql.AppendLine("ORDER BY T1.LinId,T2.dFromTime");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getChildInfo_hz(string strSyncTime_from, string strSyncTime_to, string strChanges, string strPartId, string strCarModel, string strReceiver, string strInOut, string strHaoJiu, string strSupplierId, string strSupplierPlant,
                     string strOrderPlant, string strFromTime, string strToTime, string strBoxType, string strSufferIn, string strSupplierPacking, string strOldProduction, string strDebugTime, string strPackingPlant, bool bCheck, string strOrderby, string strOrderingMethod, string strMandOrder, string strSPChild)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("SELECT  T1.LinId,");
                strSql.AppendLine("		T1.vcPackingPlant,");
                strSql.AppendLine("		T1.vcPartId,");
                strSql.AppendLine("		T1.vcReceiver,");
                strSql.AppendLine("		Convert(varchar(10),T1.dFromTime,111) as dFromTime,");
                strSql.AppendLine("		Convert(varchar(10),T1.dToTime,111) as dToTime,");
                strSql.AppendLine("		T1.vcSupplierId,");
                strSql.AppendLine("		T3.LinId as BoxLinId_ed,");
                strSql.AppendLine("		CAST(T3.iPackingQty AS varchar(10)) AS BoxPackingQty_ed,");
                strSql.AppendLine("		Convert(varchar(10),T3.dFromTime,111) as BoxFromTime_ed,");
                strSql.AppendLine("		Convert(varchar(10),T3.dToTime,111) as BoxToTime_ed,");
                strSql.AppendLine("		T3.vcBoxType as BoxType_ed,T3.iLength as BoxLength_ed,T3.iWidth as BoxWidth_ed,T3.iHeight as BoxHeight_ed,T3.iVolume as BoxVolume_ed  FROM");
                strSql.AppendLine("(SELECT * FROM [TSPMaster] WHERE 1=1 ");
                if (!bCheck)
                {
                    strSql.AppendLine(" and isnull(vcDelete, '') <> '1'");
                }
                if (strSyncTime_from != "" && strSyncTime_to != "")
                {
                    strSql.AppendLine("    AND (CONVERT(varchar(10),[dSyncTime],111)>='" + strSyncTime_from + "' AND CONVERT(varchar(10),[dSyncTime],111)<='" + strSyncTime_to + "')");
                }
                if (strPackingPlant != "")
                {
                    if (strPackingPlant == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcPackingPlant,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND vcPackingPlant='" + strPackingPlant + "'");
                    }
                }
                if (strChanges != "")
                {
                    if (strChanges == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcChanges,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND vcChanges='" + strChanges + "'");
                    }
                }
                if (strPartId != "")
                {
                    strSql.AppendLine("    AND [vcPartId] like '" + strPartId + "%'");
                }
                if (strCarModel != "")
                {
                    if (strCarModel == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcCarfamilyCode,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [vcCarfamilyCode]='" + strCarModel + "'");
                    }

                }
                if (strReceiver != "")
                {
                    if (strReceiver == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcReceiver,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [vcReceiver]='" + strReceiver + "'");
                    }
                }
                if (strInOut != "")
                {
                    if (strInOut == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcInOut,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [vcInOut]='" + strInOut + "'");
                    }
                }
                if (strHaoJiu != "")
                {
                    if (strHaoJiu == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcHaoJiu,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [vcHaoJiu]='" + strHaoJiu + "'");
                    }
                }
                if (strSupplierId != "")
                {
                    if (strSupplierId == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcSupplierId,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [vcSupplierId]='" + strSupplierId + "'");
                    }
                }
                if (strFromTime != "")
                {
                    if (strFromTime == "--")
                    {
                        strSql.AppendLine("    AND isnull(dFromTime,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [dFromTime]='" + strFromTime + "'");
                    }
                }
                if (strToTime != "")
                {
                    if (strToTime == "--")
                    {
                        strSql.AppendLine("    AND isnull(dToTime,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [dToTime]='" + strToTime + "'");
                    }
                }
                if (strSupplierPacking != "")
                {
                    if (strSupplierPacking == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcSupplierPacking,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [vcSupplierPacking]='" + strSupplierPacking + "'");
                    }
                }
                if (strOldProduction != "")
                {
                    if (strOldProduction == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcOldProduction,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [vcOldProduction]='" + strOldProduction + "'");
                    }
                }
                if (strDebugTime != "")
                {
                    if (strDebugTime == "--")
                    {
                        strSql.AppendLine("    AND isnull(dDebugTime,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND CONVERT(varchar(7),[dDebugTime],111)='" + strDebugTime + "'");
                    }
                }
                if (strOrderingMethod != "")
                {
                    if (strOrderingMethod == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcOrderingMethod,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [vcOrderingMethod]='" + strOrderingMethod + "'");
                    }
                }
                if (strMandOrder != "")
                {
                    if (strMandOrder == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcMandOrder,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [vcMandOrder]='" + strMandOrder + "'");
                    }
                }
                strSql.AppendLine("  )T1");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT ");
                strSql.AppendLine("ROW_NUMBER() OVER(partition by vcPackingPlant,vcPartId,vcReceiver,vcSupplierId order by dFromTime) [rank],");
                strSql.AppendLine("LinId,vcPackingPlant,vcPartId,vcReceiver,vcSupplierId,dFromTime,dToTime,vcSupplierPlant ");
                strSql.AppendLine("FROM [TSPMaster_SupplierPlant] ");
                strSql.AppendLine("WHERE [vcOperatorType]='1' AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23)");
                strSql.AppendLine(")T2");
                strSql.AppendLine("ON T2.rank='1' AND T1.[vcPackingPlant]=T2.[vcPackingPlant] AND T1.[vcPartId]=T2.[vcPartId] AND T1.[vcReceiver]=T2.[vcReceiver] AND T1.[vcSupplierId]=T2.[vcSupplierId]");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT LinId,vcPackingPlant,vcPartId,vcReceiver,vcSupplierId,dFromTime,dToTime,iPackingQty,vcBoxType,iLength,iWidth,iHeight,iVolume ");
                strSql.AppendLine("FROM [TSPMaster_Box] ");
                strSql.AppendLine("WHERE [vcOperatorType]='1'");
                strSql.AppendLine(")T3");
                strSql.AppendLine("ON T1.[vcPackingPlant]=T3.[vcPackingPlant] AND T1.[vcPartId]=T3.[vcPartId] AND T1.[vcReceiver]=T3.[vcReceiver] AND T1.[vcSupplierId]=T3.[vcSupplierId]");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT ");
                strSql.AppendLine("ROW_NUMBER() OVER(partition by vcPackingPlant,vcPartId,vcReceiver,vcSupplierId order by dFromTime) [rank],");
                strSql.AppendLine("LinId,vcPackingPlant,vcPartId,vcReceiver,vcSupplierId,dFromTime,dToTime,vcSufferIn");
                strSql.AppendLine("FROM [TSPMaster_SufferIn] ");
                strSql.AppendLine("WHERE [vcOperatorType]='1' AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23)");
                strSql.AppendLine(")T4");
                strSql.AppendLine("ON T4.rank='1' AND T1.[vcPackingPlant]=T4.[vcPackingPlant] AND T1.[vcPartId]=T4.[vcPartId] AND T1.[vcReceiver]=T4.[vcReceiver] AND T1.[vcSupplierId]=T4.[vcSupplierId] ");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT ");
                strSql.AppendLine("ROW_NUMBER() OVER(partition by vcSupplierId,vcSupplierPlant order by dFromTime) [rank],");
                strSql.AppendLine("vcSupplierId,vcSupplierPlant,dFromTime,dToTime,vcOrderPlant");
                strSql.AppendLine("FROM (select vcValue1 as [vcSupplierId],vcValue2 as vcSupplierPlant,vcValue3 as [dFromTime],vcValue4 as [dToTime],vcValue5 as vcOrderPlant from TOutCode where vcCodeId='C010' and vcIsColum='0'");
                strSql.AppendLine("and vcValue3<=CONVERT(VARCHAR(10),GETDATE(),23) AND vcValue4>=CONVERT(VARCHAR(10),GETDATE(),23))a ");
                strSql.AppendLine("WHERE [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))T5");
                strSql.AppendLine("ON t5.[rank]='1' AND T1.[vcSupplierId]=T5.[vcSupplierId] AND T2.vcSupplierPlant=T5.vcSupplierPlant");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C002')T6--变更事项");
                strSql.AppendLine("ON T1.vcChanges=T6.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C017')T7--包装工厂");
                strSql.AppendLine("ON T1.vcPackingPlant=T7.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C003')T8--内外区分");
                strSql.AppendLine("ON T1.vcInOut=T8.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C012')T9--OE=SP");
                strSql.AppendLine("ON T1.vcOESP=T9.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C004')T10--号旧区分");
                strSql.AppendLine("ON T1.vcHaoJiu=T10.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C000')T12--发注工厂");
                strSql.AppendLine("ON T5.vcOrderPlant=T12.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C007')T13--单据区分");
                strSql.AppendLine("ON T1.vcBillType=T13.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C047')T14--订货方式");
                strSql.AppendLine("ON T1.vcOrderingMethod=T14.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C048')T15--强制订货");
                strSql.AppendLine("ON T1.vcMandOrder=T15.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C059')T16--供应商包装");
                strSql.AppendLine("ON T1.vcSupplierPacking=T16.vcValue");
                strSql.AppendLine("WHERE 1=1");
                if (strSupplierPlant != "")
                {
                    if (strSupplierPlant == "--")
                    {
                        strSql.AppendLine("    AND isnull(T2.vcSupplierPlant,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("AND T2.vcSupplierPlant='" + strSupplierPlant + "'");
                    }
                }
                if (strOrderPlant != "")
                {
                    if (strOrderPlant == "--")
                    {
                        strSql.AppendLine("    AND isnull(T5.vcOrderPlant,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("AND T5.vcOrderPlant='" + strOrderPlant + "'");
                    }
                }
                if (strSufferIn != "")
                {
                    if (strSufferIn == "--")
                    {
                        strSql.AppendLine("    AND isnull(T4.vcSufferIn,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("AND T4.vcSufferIn='" + strSufferIn + "'");
                    }
                }
                if (strBoxType != "")
                {
                    if (strBoxType == "--")
                    {
                        strSql.AppendLine("    AND isnull(T3.vcBoxType,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("AND T3.vcBoxType like '" + strBoxType + "%'");
                    }
                }
                if (strSPChild != "")
                {
                    if (strSPChild == "0")//全部
                    {
                        strSql.AppendLine("    AND (ISNULL(T2.vcSupplierPlant,'')='' OR ISNULL(T3.iPackingQty,0)=0 OR ISNULL(T4.vcSufferIn,'')='' OR ISNULL(T5.vcOrderPlant,'')='')");
                    }
                    if (strSPChild == "1")//工区
                    {
                        strSql.AppendLine("    AND (ISNULL(T2.vcSupplierPlant,'')='')");
                    }
                    if (strSPChild == "2")//收容数
                    {
                        strSql.AppendLine("    AND (ISNULL(T3.iPackingQty,0)=0)");
                    }
                    if (strSPChild == "3")//受入
                    {
                        strSql.AppendLine("    AND (ISNULL(T4.vcSufferIn,'')='')");
                    }
                    if (strSPChild == "4")//发注工场
                    {
                        strSql.AppendLine("    AND (ISNULL(T5.vcOrderPlant,'')='')");
                    }
                }
                strSql.AppendLine("ORDER BY T1.LinId,T3.dFromTime");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getChildInfo_sr(string strSyncTime_from, string strSyncTime_to, string strChanges, string strPartId, string strCarModel, string strReceiver, string strInOut, string strHaoJiu, string strSupplierId, string strSupplierPlant,
                     string strOrderPlant, string strFromTime, string strToTime, string strBoxType, string strSufferIn, string strSupplierPacking, string strOldProduction, string strDebugTime, string strPackingPlant, bool bCheck, string strOrderby, string strOrderingMethod, string strMandOrder, string strSPChild)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("SELECT  T1.LinId,");
                strSql.AppendLine("		T1.vcPackingPlant,");
                strSql.AppendLine("		T1.vcPartId,");
                strSql.AppendLine("		T1.vcReceiver,");
                strSql.AppendLine("		Convert(varchar(10),T1.dFromTime,111) as dFromTime,");
                strSql.AppendLine("		Convert(varchar(10),T1.dToTime,111) as dToTime,");
                strSql.AppendLine("		T1.vcSupplierId,");
                strSql.AppendLine("		T4.LinId as SufferInLinId_ed,");
                strSql.AppendLine("		T4.vcSufferIn as SufferIn_ed,");
                strSql.AppendLine("		Convert(varchar(10),T4.dFromTime,111) as SufferInFromTime_ed,");
                strSql.AppendLine("		Convert(varchar(10),T4.dToTime,111) as SufferInToTime_ed  FROM");
                strSql.AppendLine("(SELECT * FROM [TSPMaster] WHERE 1=1 ");
                if (!bCheck)
                {
                    strSql.AppendLine(" and isnull(vcDelete, '') <> '1'");
                }
                if (strSyncTime_from != "" && strSyncTime_to != "")
                {
                    strSql.AppendLine("    AND (CONVERT(varchar(10),[dSyncTime],111)>='" + strSyncTime_from + "' AND CONVERT(varchar(10),[dSyncTime],111)<='" + strSyncTime_to + "')");
                }
                if (strPackingPlant != "")
                {
                    if (strPackingPlant == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcPackingPlant,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND vcPackingPlant='" + strPackingPlant + "'");
                    }
                }
                if (strChanges != "")
                {
                    if (strChanges == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcChanges,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND vcChanges='" + strChanges + "'");
                    }
                }
                if (strPartId != "")
                {
                    strSql.AppendLine("    AND [vcPartId] like '" + strPartId + "%'");
                }
                if (strCarModel != "")
                {
                    if (strCarModel == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcCarfamilyCode,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [vcCarfamilyCode]='" + strCarModel + "'");
                    }

                }
                if (strReceiver != "")
                {
                    if (strReceiver == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcReceiver,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [vcReceiver]='" + strReceiver + "'");
                    }
                }
                if (strInOut != "")
                {
                    if (strInOut == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcInOut,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [vcInOut]='" + strInOut + "'");
                    }
                }
                if (strHaoJiu != "")
                {
                    if (strHaoJiu == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcHaoJiu,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [vcHaoJiu]='" + strHaoJiu + "'");
                    }
                }
                if (strSupplierId != "")
                {
                    if (strSupplierId == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcSupplierId,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [vcSupplierId]='" + strSupplierId + "'");
                    }
                }
                if (strFromTime != "")
                {
                    if (strFromTime == "--")
                    {
                        strSql.AppendLine("    AND isnull(dFromTime,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [dFromTime]='" + strFromTime + "'");
                    }
                }
                if (strToTime != "")
                {
                    if (strToTime == "--")
                    {
                        strSql.AppendLine("    AND isnull(dToTime,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [dToTime]='" + strToTime + "'");
                    }
                }
                if (strSupplierPacking != "")
                {
                    if (strSupplierPacking == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcSupplierPacking,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [vcSupplierPacking]='" + strSupplierPacking + "'");
                    }
                }
                if (strOldProduction != "")
                {
                    if (strOldProduction == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcOldProduction,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [vcOldProduction]='" + strOldProduction + "'");
                    }
                }
                if (strDebugTime != "")
                {
                    if (strDebugTime == "--")
                    {
                        strSql.AppendLine("    AND isnull(dDebugTime,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND CONVERT(varchar(7),[dDebugTime],111)='" + strDebugTime + "'");
                    }
                }
                if (strOrderingMethod != "")
                {
                    if (strOrderingMethod == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcOrderingMethod,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [vcOrderingMethod]='" + strOrderingMethod + "'");
                    }
                }
                if (strMandOrder != "")
                {
                    if (strMandOrder == "--")
                    {
                        strSql.AppendLine("    AND isnull(vcMandOrder,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("    AND [vcMandOrder]='" + strMandOrder + "'");
                    }
                }
                strSql.AppendLine("  )T1");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT ");
                strSql.AppendLine("ROW_NUMBER() OVER(partition by vcPackingPlant,vcPartId,vcReceiver,vcSupplierId order by dFromTime) [rank],");
                strSql.AppendLine("LinId,vcPackingPlant,vcPartId,vcReceiver,vcSupplierId,dFromTime,dToTime,vcSupplierPlant ");
                strSql.AppendLine("FROM [TSPMaster_SupplierPlant] ");
                strSql.AppendLine("WHERE [vcOperatorType]='1' AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23)");
                strSql.AppendLine(")T2");
                strSql.AppendLine("ON T2.rank='1' AND T1.[vcPackingPlant]=T2.[vcPackingPlant] AND T1.[vcPartId]=T2.[vcPartId] AND T1.[vcReceiver]=T2.[vcReceiver] AND T1.[vcSupplierId]=T2.[vcSupplierId]");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT ");
                strSql.AppendLine("ROW_NUMBER() OVER(partition by vcPackingPlant,vcPartId,vcReceiver,vcSupplierId order by dFromTime) [rank],");
                strSql.AppendLine("LinId,vcPackingPlant,vcPartId,vcReceiver,vcSupplierId,dFromTime,dToTime,iPackingQty,vcBoxType,iLength,iWidth,iHeight,iVolume ");
                strSql.AppendLine("FROM [TSPMaster_Box] ");
                strSql.AppendLine("WHERE [vcOperatorType]='1' AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23)");
                strSql.AppendLine(")T3");
                strSql.AppendLine("ON T3.rank='1' AND T1.[vcPackingPlant]=T3.[vcPackingPlant] AND T1.[vcPartId]=T3.[vcPartId] AND T1.[vcReceiver]=T3.[vcReceiver] AND T1.[vcSupplierId]=T3.[vcSupplierId]");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT LinId,vcPackingPlant,vcPartId,vcReceiver,vcSupplierId,dFromTime,dToTime,vcSufferIn");
                strSql.AppendLine("FROM [TSPMaster_SufferIn] ");
                strSql.AppendLine("WHERE [vcOperatorType]='1'");
                strSql.AppendLine(")T4");
                strSql.AppendLine("ON T1.[vcPackingPlant]=T4.[vcPackingPlant] AND T1.[vcPartId]=T4.[vcPartId] AND T1.[vcReceiver]=T4.[vcReceiver] AND T1.[vcSupplierId]=T4.[vcSupplierId] ");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT ");
                strSql.AppendLine("ROW_NUMBER() OVER(partition by vcSupplierId,vcSupplierPlant order by dFromTime) [rank],");
                strSql.AppendLine("vcSupplierId,vcSupplierPlant,dFromTime,dToTime,vcOrderPlant");
                strSql.AppendLine("FROM (select vcValue1 as [vcSupplierId],vcValue2 as vcSupplierPlant,vcValue3 as [dFromTime],vcValue4 as [dToTime],vcValue5 as vcOrderPlant from TOutCode where vcCodeId='C010' and vcIsColum='0'");
                strSql.AppendLine("and vcValue3<=CONVERT(VARCHAR(10),GETDATE(),23) AND vcValue4>=CONVERT(VARCHAR(10),GETDATE(),23))a ");
                strSql.AppendLine("WHERE [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))T5");
                strSql.AppendLine("ON t5.[rank]='1' AND T1.[vcSupplierId]=T5.[vcSupplierId] AND T2.vcSupplierPlant=T5.vcSupplierPlant");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C002')T6--变更事项");
                strSql.AppendLine("ON T1.vcChanges=T6.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C017')T7--包装工厂");
                strSql.AppendLine("ON T1.vcPackingPlant=T7.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C003')T8--内外区分");
                strSql.AppendLine("ON T1.vcInOut=T8.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C012')T9--OE=SP");
                strSql.AppendLine("ON T1.vcOESP=T9.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C004')T10--号旧区分");
                strSql.AppendLine("ON T1.vcHaoJiu=T10.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C000')T12--发注工厂");
                strSql.AppendLine("ON T5.vcOrderPlant=T12.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C007')T13--单据区分");
                strSql.AppendLine("ON T1.vcBillType=T13.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C047')T14--订货方式");
                strSql.AppendLine("ON T1.vcOrderingMethod=T14.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C048')T15--强制订货");
                strSql.AppendLine("ON T1.vcMandOrder=T15.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C059')T16--供应商包装");
                strSql.AppendLine("ON T1.vcSupplierPacking=T16.vcValue");
                strSql.AppendLine("WHERE 1=1");
                if (strSupplierPlant != "")
                {
                    if (strSupplierPlant == "--")
                    {
                        strSql.AppendLine("    AND isnull(T2.vcSupplierPlant,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("AND T2.vcSupplierPlant='" + strSupplierPlant + "'");
                    }
                }
                if (strOrderPlant != "")
                {
                    if (strOrderPlant == "--")
                    {
                        strSql.AppendLine("    AND isnull(T5.vcOrderPlant,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("AND T5.vcOrderPlant='" + strOrderPlant + "'");
                    }
                }
                if (strSufferIn != "")
                {
                    if (strSufferIn == "--")
                    {
                        strSql.AppendLine("    AND isnull(T4.vcSufferIn,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("AND T4.vcSufferIn='" + strSufferIn + "'");
                    }
                }
                if (strBoxType != "")
                {
                    if (strBoxType == "--")
                    {
                        strSql.AppendLine("    AND isnull(T3.vcBoxType,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("AND T3.vcBoxType like '" + strBoxType + "%'");
                    }
                }
                if (strSPChild != "")
                {
                    if (strSPChild == "0")//全部
                    {
                        strSql.AppendLine("    AND (ISNULL(T2.vcSupplierPlant,'')='' OR ISNULL(T3.iPackingQty,0)=0 OR ISNULL(T4.vcSufferIn,'')='' OR ISNULL(T5.vcOrderPlant,'')='')");
                    }
                    if (strSPChild == "1")//工区
                    {
                        strSql.AppendLine("    AND (ISNULL(T2.vcSupplierPlant,'')='')");
                    }
                    if (strSPChild == "2")//收容数
                    {
                        strSql.AppendLine("    AND (ISNULL(T3.iPackingQty,0)=0)");
                    }
                    if (strSPChild == "3")//受入
                    {
                        strSql.AppendLine("    AND (ISNULL(T4.vcSufferIn,'')='')");
                    }
                    if (strSPChild == "4")//发注工场
                    {
                        strSql.AppendLine("    AND (ISNULL(T5.vcOrderPlant,'')='')");
                    }
                }
                strSql.AppendLine("ORDER BY T1.LinId,T4.dFromTime");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void deleteInfo(List<Dictionary<string, Object>> listInfoData, string strOperId)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    string strPackingPlant = listInfoData[i]["vcPackingPlant"].ToString();
                    string strPartId = listInfoData[i]["vcPartId"].ToString();
                    string strReceiver = listInfoData[i]["vcReceiver"].ToString();
                    string strSupplierId = listInfoData[i]["vcSupplierId"].ToString();
                    stringBuilder.AppendLine("UPDATE TSPMaster SET vcDelete='1' WHERE [vcPackingPlant]='" + strPackingPlant + "' AND [vcPartId]='" + strPartId + "' AND [vcReceiver]='" + strReceiver + "' AND [vcSupplierId]='" + strSupplierId + "'");
                    stringBuilder.AppendLine("DELETE FROM TSPMaster_Box WHERE [vcPackingPlant]='" + strPackingPlant + "' AND [vcPartId]='" + strPartId + "' AND [vcReceiver]='" + strReceiver + "' AND [vcSupplierId]='" + strSupplierId + "'");
                    stringBuilder.AppendLine("DELETE FROM TSPMaster_SufferIn WHERE [vcPackingPlant]='" + strPackingPlant + "' AND [vcPartId]='" + strPartId + "' AND [vcReceiver]='" + strReceiver + "' AND [vcSupplierId]='" + strSupplierId + "'");
                    stringBuilder.AppendLine("DELETE FROM TSPMaster_SupplierPlant WHERE [vcPackingPlant]='" + strPackingPlant + "' AND [vcPartId]='" + strPartId + "' AND [vcReceiver]='" + strReceiver + "' AND [vcSupplierId]='" + strSupplierId + "'");
                    string strAction = "包装工场:" + strPackingPlant + "；品番:" + strPartId + "；收货方:" + strReceiver + "；供应商:" + strSupplierId;
                    stringBuilder.AppendLine("INSERT INTO [dbo].[TSPMaster_OperHistory]");
                    stringBuilder.AppendLine("           ([vcPackingPlant]");
                    stringBuilder.AppendLine("           ,[vcPartId]");
                    stringBuilder.AppendLine("           ,[vcReceiver]");
                    stringBuilder.AppendLine("           ,[vcSupplierId]");
                    stringBuilder.AppendLine("           ,[vcChangeItem]");
                    stringBuilder.AppendLine("           ,[vcAction]");
                    stringBuilder.AppendLine("           ,[vcActionTime]");
                    stringBuilder.AppendLine("           ,[vcOperatorID]");
                    stringBuilder.AppendLine("           ,[dOperatorTime])");
                    stringBuilder.AppendLine("     VALUES");
                    stringBuilder.AppendLine("           ('" + strPackingPlant + "'");
                    stringBuilder.AppendLine("           ,'" + strPartId + "'");
                    stringBuilder.AppendLine("           ,'" + strReceiver + "'");
                    stringBuilder.AppendLine("           ,'" + strSupplierId + "'");
                    stringBuilder.AppendLine("           ,'情报删除'");
                    stringBuilder.AppendLine("           ,'" + strAction + "'");
                    stringBuilder.AppendLine("           ,''");
                    stringBuilder.AppendLine("           ,'" + strOperId + "'");
                    stringBuilder.AppendLine("           ,GETDATE())");
                }
                excute.ExcuteSqlWithStringOper(stringBuilder.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setSPInfo(DataTable dtAddInfo, DataTable dtModInfo, DataTable dtDelInfo, DataTable dtModInfo_SP, DataTable dtModInfo_PQ, DataTable dtModInfo_SI, DataTable dtModInfo_OP, DataTable dtOperHistory, DataTable dtSPChild, DataTable dtChildInfo_gq, DataTable dtChildInfo_sr, DataTable dtChildInfo_hz,
            string strOperId, ref DataTable dtMessage)
        {
            SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();

            sqlConnection.Open();
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            try
            {
                #region 写入数据库
                #region 1.sqlCommand_addinfo
                SqlCommand sqlCommand_addinfo = sqlConnection.CreateCommand();
                sqlCommand_addinfo.Transaction = sqlTransaction;
                sqlCommand_addinfo.CommandType = CommandType.Text;
                StringBuilder strSql_addinfo = new StringBuilder();

                #region SQL and Parameters
                strSql_addinfo.AppendLine("INSERT INTO [dbo].[TSPMaster]");
                strSql_addinfo.AppendLine("           ([vcPackingPlant]");
                strSql_addinfo.AppendLine("           ,[vcPartId]");
                strSql_addinfo.AppendLine("           ,[vcPartENName]");
                strSql_addinfo.AppendLine("           ,[vcCarfamilyCode]");
                strSql_addinfo.AppendLine("           ,[vcCarModel]");
                strSql_addinfo.AppendLine("           ,[vcReceiver]");
                strSql_addinfo.AppendLine("           ,[dFromTime]");
                strSql_addinfo.AppendLine("           ,[dToTime]");
                strSql_addinfo.AppendLine("           ,[vcPartId_Replace]");
                strSql_addinfo.AppendLine("           ,[vcInOut]");
                strSql_addinfo.AppendLine("           ,[vcOESP]");
                strSql_addinfo.AppendLine("           ,[vcHaoJiu]");
                strSql_addinfo.AppendLine("           ,[vcOldProduction]");
                strSql_addinfo.AppendLine("           ,[dDebugTime]");
                strSql_addinfo.AppendLine("           ,[vcSupplierId]");
                strSql_addinfo.AppendLine("           ,[dSupplierFromTime]");
                strSql_addinfo.AppendLine("           ,[dSupplierToTime]");
                strSql_addinfo.AppendLine("           ,[vcSupplierName]");
                strSql_addinfo.AppendLine("           ,[vcSupplierPlace]");
                strSql_addinfo.AppendLine("           ,[vcInteriorProject]");
                strSql_addinfo.AppendLine("           ,[vcPassProject]");
                strSql_addinfo.AppendLine("           ,[vcFrontProject]");
                strSql_addinfo.AppendLine("           ,[dFrontProjectTime]");
                strSql_addinfo.AppendLine("           ,[dShipmentTime]");
                strSql_addinfo.AppendLine("           ,[vcBillType]");
                strSql_addinfo.AppendLine("           ,[vcOrderingMethod]");
                strSql_addinfo.AppendLine("           ,[vcMandOrder]");
                strSql_addinfo.AppendLine("           ,[vcPartImage]");
                strSql_addinfo.AppendLine("           ,[vcRemark1]");
                strSql_addinfo.AppendLine("           ,[vcRemark2]");
                strSql_addinfo.AppendLine("           ,[vcSupplierPacking]");
                strSql_addinfo.AppendLine("           ,[vcDelete]");
                strSql_addinfo.AppendLine("           ,[vcOperatorID]");
                strSql_addinfo.AppendLine("           ,[dOperatorTime]");
                strSql_addinfo.AppendLine("           ,[dSyncToSPTime]");
                strSql_addinfo.AppendLine("           ,[iOrderBy])");
                strSql_addinfo.AppendLine("     VALUES");
                strSql_addinfo.AppendLine("           (case when @vcPackingPlant='' then null else @vcPackingPlant end");
                strSql_addinfo.AppendLine("           ,case when @vcPartId='' then null else @vcPartId end");
                strSql_addinfo.AppendLine("           ,case when @vcPartENName='' then null else @vcPartENName end");
                strSql_addinfo.AppendLine("           ,case when @vcCarfamilyCode='' then null else @vcCarfamilyCode end");
                strSql_addinfo.AppendLine("           ,case when @vcCarfamilyCode='' then null else @vcCarfamilyCode end");
                strSql_addinfo.AppendLine("           ,case when @vcReceiver='' then null else @vcReceiver end");
                strSql_addinfo.AppendLine("           ,case when @dFromTime='' then null else @dFromTime end");
                strSql_addinfo.AppendLine("           ,case when @dToTime='' then '9999-12-31' else @dToTime end");
                strSql_addinfo.AppendLine("           ,case when @vcPartId_Replace='' then null else @vcPartId_Replace end");
                strSql_addinfo.AppendLine("           ,case when @vcInOut='' then null else @vcInOut end");
                strSql_addinfo.AppendLine("           ,case when @vcOESP='' then null else @vcOESP end");
                strSql_addinfo.AppendLine("           ,case when @vcHaoJiu='' then null else @vcHaoJiu end");
                strSql_addinfo.AppendLine("           ,case when @vcOldProduction='' then null else @vcOldProduction end");
                strSql_addinfo.AppendLine("           ,case when @dDebugTime='' then null else @dDebugTime end");
                strSql_addinfo.AppendLine("           ,case when @vcSupplierId='' then null else @vcSupplierId end");
                strSql_addinfo.AppendLine("           ,case when @dSupplierFromTime='' then null else @dSupplierFromTime end");
                strSql_addinfo.AppendLine("           ,case when @dSupplierToTime='' then null else @dSupplierToTime end");
                strSql_addinfo.AppendLine("           ,case when @vcSupplierName='' then null else @vcSupplierName end");
                strSql_addinfo.AppendLine("           ,case when @vcSupplierPlace='' then null else @vcSupplierPlace end");
                strSql_addinfo.AppendLine("           ,case when @vcInteriorProject='' then null else @vcInteriorProject end");
                strSql_addinfo.AppendLine("           ,case when @vcPassProject='' then null else @vcPassProject end");
                strSql_addinfo.AppendLine("           ,case when @vcFrontProject='' then null else @vcFrontProject end");
                strSql_addinfo.AppendLine("           ,case when @dFrontProjectTime='' then null else @dFrontProjectTime end");
                strSql_addinfo.AppendLine("           ,case when @dShipmentTime='' then null else @dShipmentTime end");
                strSql_addinfo.AppendLine("           ,case when @vcBillType='' then null else @vcBillType end");
                strSql_addinfo.AppendLine("           ,case when @vcOrderingMethod='' then null else @vcOrderingMethod end");
                strSql_addinfo.AppendLine("           ,case when @vcMandOrder='' then null else @vcMandOrder end");
                strSql_addinfo.AppendLine("           ,case when @vcPartImage='' then null else @vcPartImage end");
                strSql_addinfo.AppendLine("           ,case when @vcRemark1='' then null else @vcRemark1 end");
                strSql_addinfo.AppendLine("           ,case when @vcRemark2='' then null else @vcRemark2 end");
                strSql_addinfo.AppendLine("           ,case when @vcSupplierPacking='' then null else @vcSupplierPacking end");
                strSql_addinfo.AppendLine("           ,'0'");
                strSql_addinfo.AppendLine("           ,'" + strOperId + "'");
                strSql_addinfo.AppendLine("           ,GETDATE()");
                strSql_addinfo.AppendLine("           ,null");
                strSql_addinfo.AppendLine("           ,1)");
                sqlCommand_addinfo.CommandText = strSql_addinfo.ToString();
                //sqlCommand_addinfo.Parameters.AddWithValue("@dSyncTime", "");
                //sqlCommand_addinfo.Parameters.AddWithValue("@vcChanges", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcPackingPlant", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcPartId", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcPartENName", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcCarfamilyCode", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcCarModel", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcReceiver", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@dFromTime", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@dToTime", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcPartId_Replace", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcInOut", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcOESP", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcHaoJiu", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcOldProduction", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@dDebugTime", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcSupplierId", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@dSupplierFromTime", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@dSupplierToTime", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcSupplierName", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcSupplierPlace", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcInteriorProject", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcPassProject", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcFrontProject", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@dFrontProjectTime", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@dShipmentTime", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcBillType", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcOrderingMethod", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcMandOrder", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcPartImage", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcRemark1", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcRemark2", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcSupplierPacking", "");
                #endregion
                foreach (DataRow item in dtAddInfo.Rows)
                {
                    #region Value
                    //sqlCommand_addinfo.Parameters["@dSyncTime"].Value = item["dSyncTime"].ToString();
                    //sqlCommand_addinfo.Parameters["@vcChanges"].Value = item["vcChanges"].ToString();
                    sqlCommand_addinfo.Parameters["@vcPackingPlant"].Value = item["vcPackingPlant"].ToString();
                    sqlCommand_addinfo.Parameters["@vcPartId"].Value = item["vcPartId"].ToString();
                    sqlCommand_addinfo.Parameters["@vcPartENName"].Value = item["vcPartENName"].ToString();
                    sqlCommand_addinfo.Parameters["@vcCarfamilyCode"].Value = item["vcCarfamilyCode"].ToString();
                    sqlCommand_addinfo.Parameters["@vcReceiver"].Value = item["vcReceiver"].ToString();
                    sqlCommand_addinfo.Parameters["@dFromTime"].Value = item["dFromTime"].ToString();
                    sqlCommand_addinfo.Parameters["@dToTime"].Value = item["dToTime"].ToString();
                    sqlCommand_addinfo.Parameters["@vcPartId_Replace"].Value = item["vcPartId_Replace"].ToString();
                    sqlCommand_addinfo.Parameters["@vcInOut"].Value = item["vcInOut"].ToString();
                    sqlCommand_addinfo.Parameters["@vcOESP"].Value = item["vcOESP"].ToString();
                    sqlCommand_addinfo.Parameters["@vcHaoJiu"].Value = item["vcHaoJiu"].ToString();
                    sqlCommand_addinfo.Parameters["@vcOldProduction"].Value = item["vcOldProduction"].ToString();
                    sqlCommand_addinfo.Parameters["@dDebugTime"].Value = item["dDebugTime"].ToString();
                    sqlCommand_addinfo.Parameters["@vcSupplierId"].Value = item["vcSupplierId"].ToString();
                    sqlCommand_addinfo.Parameters["@dSupplierFromTime"].Value = item["dSupplierFromTime"].ToString();
                    sqlCommand_addinfo.Parameters["@dSupplierToTime"].Value = item["dSupplierToTime"].ToString();
                    sqlCommand_addinfo.Parameters["@vcSupplierName"].Value = item["vcSupplierName"].ToString();
                    sqlCommand_addinfo.Parameters["@vcSupplierPlace"].Value = item["vcSupplierPlace"].ToString();
                    sqlCommand_addinfo.Parameters["@vcInteriorProject"].Value = item["vcInteriorProject"].ToString();
                    sqlCommand_addinfo.Parameters["@vcPassProject"].Value = item["vcPassProject"].ToString();
                    sqlCommand_addinfo.Parameters["@vcFrontProject"].Value = item["vcFrontProject"].ToString();
                    sqlCommand_addinfo.Parameters["@dFrontProjectTime"].Value = item["dFrontProjectTime"].ToString();
                    sqlCommand_addinfo.Parameters["@dShipmentTime"].Value = item["dShipmentTime"].ToString();
                    sqlCommand_addinfo.Parameters["@vcBillType"].Value = item["vcBillType"].ToString();
                    sqlCommand_addinfo.Parameters["@vcOrderingMethod"].Value = item["vcOrderingMethod"].ToString();
                    sqlCommand_addinfo.Parameters["@vcMandOrder"].Value = item["vcMandOrder"].ToString();
                    sqlCommand_addinfo.Parameters["@vcPartImage"].Value = item["vcPartImage"].ToString();
                    sqlCommand_addinfo.Parameters["@vcRemark1"].Value = item["vcRemark1"].ToString();
                    sqlCommand_addinfo.Parameters["@vcRemark2"].Value = item["vcRemark2"].ToString();
                    sqlCommand_addinfo.Parameters["@vcSupplierPacking"].Value = item["vcSupplierPacking"].ToString();
                    #endregion
                    sqlCommand_addinfo.ExecuteNonQuery();
                }
                #endregion

                #region 2.sqlCommand_modinfo
                SqlCommand sqlCommand_modinfo = sqlConnection.CreateCommand();
                sqlCommand_modinfo.Transaction = sqlTransaction;
                sqlCommand_modinfo.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo = new StringBuilder();

                #region SQL and Parameters
                strSql_modinfo.AppendLine("UPDATE [dbo].[TSPMaster]");
                strSql_modinfo.AppendLine("   SET [vcPartENName] = case when @vcPartENName='' then null else @vcPartENName end");
                strSql_modinfo.AppendLine("      ,[vcCarfamilyCode] = case when @vcCarfamilyCode='' then null else @vcCarfamilyCode end");
                strSql_modinfo.AppendLine("      ,[vcCarModel] = case when @vcCarfamilyCode='' then null else @vcCarfamilyCode end");
                strSql_modinfo.AppendLine("      ,[dFromTime]=case when @dFromTime='' then null else @dFromTime end");
                strSql_modinfo.AppendLine("      ,[dToTime] =case when @dToTime='' then '9999-12-31' else @dToTime end");
                strSql_modinfo.AppendLine("      ,[vcPartId_Replace] = case when @vcPartId_Replace='' then null else @vcPartId_Replace end");
                strSql_modinfo.AppendLine("      ,[vcInOut] = case when @vcInOut='' then null else @vcInOut end");
                strSql_modinfo.AppendLine("      ,[vcOESP] = case when @vcOESP='' then null else @vcOESP end");
                strSql_modinfo.AppendLine("      ,[vcHaoJiu] = case when @vcHaoJiu='' then null else @vcHaoJiu end");
                strSql_modinfo.AppendLine("      ,[vcOldProduction] = case when @vcOldProduction='' then null else @vcOldProduction end");
                strSql_modinfo.AppendLine("      ,[dDebugTime] = case when @dDebugTime='' then null else @dDebugTime end");
                strSql_modinfo.AppendLine("      ,[dSupplierFromTime] =case when @dSupplierFromTime='' then null else @dSupplierFromTime end");
                strSql_modinfo.AppendLine("      ,[dSupplierToTime] = case when @dSupplierToTime='' then null else @dSupplierToTime end");
                strSql_modinfo.AppendLine("      ,[vcSupplierName] = case when @vcSupplierName='' then null else @vcSupplierName end");
                strSql_modinfo.AppendLine("      ,[vcSupplierPlace] = case when @vcSupplierPlace='' then null else @vcSupplierPlace end");
                strSql_modinfo.AppendLine("      ,[vcInteriorProject] = case when @vcInteriorProject='' then null else @vcInteriorProject end");
                strSql_modinfo.AppendLine("      ,[vcPassProject] = case when @vcPassProject='' then null else @vcPassProject end");
                strSql_modinfo.AppendLine("      ,[vcFrontProject] = case when @vcFrontProject='' then null else @vcFrontProject end");
                strSql_modinfo.AppendLine("      ,[dFrontProjectTime] =case when @dFrontProjectTime='' then null else @dFrontProjectTime end");
                strSql_modinfo.AppendLine("      ,[dShipmentTime] = case when @dShipmentTime='' then null else @dShipmentTime end");
                strSql_modinfo.AppendLine("      ,[vcBillType] = case when @vcBillType='' then null else @vcBillType end");
                strSql_modinfo.AppendLine("      ,[vcOrderingMethod] = case when @vcOrderingMethod='' then null else @vcOrderingMethod end");
                strSql_modinfo.AppendLine("      ,[vcMandOrder] = case when @vcMandOrder='' then null else @vcMandOrder end");
                strSql_modinfo.AppendLine("      ,[vcPartImage] = case when @vcPartImage='' then null else @vcPartImage end");
                strSql_modinfo.AppendLine("      ,[vcRemark1] = case when @vcRemark1='' then null else @vcRemark1 end");
                strSql_modinfo.AppendLine("      ,[vcRemark2] = case when @vcRemark2='' then null else @vcRemark2 end");
                strSql_modinfo.AppendLine("      ,[vcSupplierPacking] = case when @vcSupplierPacking='' then null else @vcSupplierPacking end");
                strSql_modinfo.AppendLine("      ,[vcOperatorID] = '" + strOperId + "'");
                strSql_modinfo.AppendLine("      ,[dOperatorTime] =GETDATE()");
                strSql_modinfo.AppendLine(" WHERE [vcPackingPlant] = @vcPackingPlant");
                strSql_modinfo.AppendLine("      AND [vcPartId] = @vcPartId");
                strSql_modinfo.AppendLine("      AND [vcReceiver] = @vcReceiver");
                strSql_modinfo.AppendLine("      AND [vcSupplierId] = @vcSupplierId");
                sqlCommand_modinfo.CommandText = strSql_modinfo.ToString();
                //sqlCommand_modinfo.Parameters.AddWithValue("@vcChanges", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcPackingPlant", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcPartId", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcPartENName", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcCarfamilyCode", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcCarModel", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcReceiver", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@dFromTime", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@dToTime", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcPartId_Replace", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcInOut", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcOESP", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcHaoJiu", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcOldProduction", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@dDebugTime", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcSupplierId", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@dSupplierFromTime", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@dSupplierToTime", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcSupplierName", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcSupplierPlace", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcInteriorProject", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcPassProject", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcFrontProject", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@dFrontProjectTime", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@dShipmentTime", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcBillType", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcOrderingMethod", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcMandOrder", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcPartImage", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcRemark1", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcRemark2", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcSupplierPacking", "");
                #endregion
                foreach (DataRow item in dtModInfo.Rows)
                {
                    #region Value
                    //sqlCommand_modinfo.Parameters["@vcChanges"].Value = item["vcChanges"].ToString();
                    sqlCommand_modinfo.Parameters["@vcPackingPlant"].Value = item["vcPackingPlant"].ToString();
                    sqlCommand_modinfo.Parameters["@vcPartId"].Value = item["vcPartId"].ToString();
                    sqlCommand_modinfo.Parameters["@vcPartENName"].Value = item["vcPartENName"].ToString();
                    sqlCommand_modinfo.Parameters["@vcCarfamilyCode"].Value = item["vcCarfamilyCode"].ToString();
                    //sqlCommand_modinfo.Parameters["@vcCarModel"].Value = item["vcCarModel"].ToString();
                    sqlCommand_modinfo.Parameters["@vcReceiver"].Value = item["vcReceiver"].ToString();
                    sqlCommand_modinfo.Parameters["@dFromTime"].Value = item["dFromTime"].ToString();
                    sqlCommand_modinfo.Parameters["@dToTime"].Value = item["dToTime"].ToString();
                    sqlCommand_modinfo.Parameters["@vcPartId_Replace"].Value = item["vcPartId_Replace"].ToString();
                    sqlCommand_modinfo.Parameters["@vcInOut"].Value = item["vcInOut"].ToString();
                    sqlCommand_modinfo.Parameters["@vcOESP"].Value = item["vcOESP"].ToString();
                    sqlCommand_modinfo.Parameters["@vcHaoJiu"].Value = item["vcHaoJiu"].ToString();
                    sqlCommand_modinfo.Parameters["@vcOldProduction"].Value = item["vcOldProduction"].ToString();
                    sqlCommand_modinfo.Parameters["@dDebugTime"].Value = item["dDebugTime"].ToString();
                    sqlCommand_modinfo.Parameters["@vcSupplierId"].Value = item["vcSupplierId"].ToString();
                    sqlCommand_modinfo.Parameters["@dSupplierFromTime"].Value = item["dSupplierFromTime"].ToString();
                    sqlCommand_modinfo.Parameters["@dSupplierToTime"].Value = item["dSupplierToTime"].ToString();
                    sqlCommand_modinfo.Parameters["@vcSupplierName"].Value = item["vcSupplierName"].ToString();
                    sqlCommand_modinfo.Parameters["@vcSupplierPlace"].Value = item["vcSupplierPlace"].ToString();
                    sqlCommand_modinfo.Parameters["@vcInteriorProject"].Value = item["vcInteriorProject"].ToString();
                    sqlCommand_modinfo.Parameters["@vcPassProject"].Value = item["vcPassProject"].ToString();
                    sqlCommand_modinfo.Parameters["@vcFrontProject"].Value = item["vcFrontProject"].ToString();
                    sqlCommand_modinfo.Parameters["@dFrontProjectTime"].Value = item["dFrontProjectTime"].ToString();
                    sqlCommand_modinfo.Parameters["@dShipmentTime"].Value = item["dShipmentTime"].ToString();
                    sqlCommand_modinfo.Parameters["@vcBillType"].Value = item["vcBillType"].ToString();
                    sqlCommand_modinfo.Parameters["@vcOrderingMethod"].Value = item["vcOrderingMethod"].ToString();
                    sqlCommand_modinfo.Parameters["@vcMandOrder"].Value = item["vcMandOrder"].ToString();
                    sqlCommand_modinfo.Parameters["@vcPartImage"].Value = item["vcPartImage"].ToString();
                    sqlCommand_modinfo.Parameters["@vcRemark1"].Value = item["vcRemark1"].ToString();
                    sqlCommand_modinfo.Parameters["@vcRemark2"].Value = item["vcRemark2"].ToString();
                    sqlCommand_modinfo.Parameters["@vcSupplierPacking"].Value = item["vcSupplierPacking"].ToString();
                    #endregion
                    sqlCommand_modinfo.ExecuteNonQuery();
                }
                #endregion

                #region 3.sqlCommand_delinfo
                SqlCommand sqlCommand_delinfo = sqlConnection.CreateCommand();
                sqlCommand_delinfo.Transaction = sqlTransaction;
                sqlCommand_delinfo.CommandType = CommandType.Text;
                StringBuilder strSql_delinfo = new StringBuilder();
                strSql_delinfo.AppendLine("UPDATE TSPMaster SET vcDelete='1',[vcOperatorID]='" + strOperId + "',[dOperatorTime]=getdate() WHERE [vcPackingPlant]=@vcPackingPlant AND [vcPartId]=@vcPartId AND [vcReceiver]=@vcReceiver AND [vcSupplierId]=@vcSupplierId");
                strSql_delinfo.AppendLine("DELETE FROM TSPMaster_Box WHERE [vcPackingPlant]=@vcPackingPlant AND [vcPartId]=@vcPartId AND [vcReceiver]=@vcReceiver AND [vcSupplierId]=@vcSupplierId");
                strSql_delinfo.AppendLine("DELETE FROM TSPMaster_SufferIn WHERE [vcPackingPlant]=@vcPackingPlant AND [vcPartId]=@vcPartId AND [vcReceiver]=@vcReceiver AND [vcSupplierId]=@vcSupplierId");
                strSql_delinfo.AppendLine("DELETE FROM TSPMaster_SupplierPlant WHERE [vcPackingPlant]=@vcPackingPlant AND [vcPartId]=@vcPartId AND [vcReceiver]=@vcReceiver AND [vcSupplierId]=@vcSupplierId");
                sqlCommand_delinfo.CommandText = strSql_delinfo.ToString();
                sqlCommand_delinfo.Parameters.AddWithValue("@vcPackingPlant", "");
                sqlCommand_delinfo.Parameters.AddWithValue("@vcPartId", "");
                sqlCommand_delinfo.Parameters.AddWithValue("@vcReceiver", "");
                sqlCommand_delinfo.Parameters.AddWithValue("@vcSupplierId", "");
                foreach (DataRow item in dtDelInfo.Rows)
                {
                    sqlCommand_delinfo.Parameters["@vcPackingPlant"].Value = item["vcPackingPlant"].ToString();
                    sqlCommand_delinfo.Parameters["@vcPartId"].Value = item["vcPartId"].ToString();
                    sqlCommand_delinfo.Parameters["@vcReceiver"].Value = item["vcReceiver"].ToString();
                    sqlCommand_delinfo.Parameters["@vcSupplierId"].Value = item["vcSupplierId"].ToString();
                    sqlCommand_delinfo.ExecuteNonQuery();
                }
                #endregion

                #region 4.sqlCommand_modinfo_sp_add
                SqlCommand sqlCommand_modinfo_sp_add = sqlConnection.CreateCommand();
                sqlCommand_modinfo_sp_add.Transaction = sqlTransaction;
                sqlCommand_modinfo_sp_add.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo_sp_add = new StringBuilder();

                #region SQL and Parameters
                strSql_modinfo_sp_add.AppendLine("INSERT INTO [dbo].[TSPMaster_SupplierPlant]");
                strSql_modinfo_sp_add.AppendLine("           ([vcPackingPlant]");
                strSql_modinfo_sp_add.AppendLine("           ,[vcPartId]");
                strSql_modinfo_sp_add.AppendLine("           ,[vcReceiver]");
                strSql_modinfo_sp_add.AppendLine("           ,[vcSupplierId]");
                strSql_modinfo_sp_add.AppendLine("           ,[dFromTime]");
                strSql_modinfo_sp_add.AppendLine("           ,[dToTime]");
                strSql_modinfo_sp_add.AppendLine("           ,[vcSupplierPlant]");
                strSql_modinfo_sp_add.AppendLine("           ,[vcOperatorType]");
                strSql_modinfo_sp_add.AppendLine("           ,[vcOperatorID]");
                strSql_modinfo_sp_add.AppendLine("           ,[dOperatorTime])");
                strSql_modinfo_sp_add.AppendLine("     VALUES");
                strSql_modinfo_sp_add.AppendLine("           (@vcPackingPlant");
                strSql_modinfo_sp_add.AppendLine("           ,@vcPartId");
                strSql_modinfo_sp_add.AppendLine("           ,@vcReceiver");
                strSql_modinfo_sp_add.AppendLine("           ,@vcSupplierId");
                strSql_modinfo_sp_add.AppendLine("           ,case when @dFromTime='' then null else @dFromTime end");
                strSql_modinfo_sp_add.AppendLine("           ,case when @dToTime='' then '9999-12-31' else @dToTime end");
                strSql_modinfo_sp_add.AppendLine("           ,@vcSupplierPlant");
                strSql_modinfo_sp_add.AppendLine("           ,'1'");
                strSql_modinfo_sp_add.AppendLine("           ,'" + strOperId + "'");
                strSql_modinfo_sp_add.AppendLine("           ,GETDATE())");
                sqlCommand_modinfo_sp_add.CommandText = strSql_modinfo_sp_add.ToString();
                sqlCommand_modinfo_sp_add.Parameters.AddWithValue("@vcPackingPlant", "");
                sqlCommand_modinfo_sp_add.Parameters.AddWithValue("@vcPartId", "");
                sqlCommand_modinfo_sp_add.Parameters.AddWithValue("@vcReceiver", "");
                sqlCommand_modinfo_sp_add.Parameters.AddWithValue("@vcSupplierId", "");
                sqlCommand_modinfo_sp_add.Parameters.AddWithValue("@dFromTime", "");
                sqlCommand_modinfo_sp_add.Parameters.AddWithValue("@dToTime", "");
                sqlCommand_modinfo_sp_add.Parameters.AddWithValue("@vcSupplierPlant", "");
                #endregion
                foreach (DataRow item in dtModInfo_SP.Rows)
                {
                    if (item["status"].ToString() == "add" && item["error"].ToString() == "")
                    {
                        #region Value
                        sqlCommand_modinfo_sp_add.Parameters["@vcPackingPlant"].Value = item["vcPackingPlant"].ToString();
                        sqlCommand_modinfo_sp_add.Parameters["@vcPartId"].Value = item["vcPartId"].ToString();
                        sqlCommand_modinfo_sp_add.Parameters["@vcReceiver"].Value = item["vcReceiver"].ToString();
                        sqlCommand_modinfo_sp_add.Parameters["@vcSupplierId"].Value = item["vcSupplierId"].ToString();
                        sqlCommand_modinfo_sp_add.Parameters["@dFromTime"].Value = item["dFromTime"].ToString();
                        sqlCommand_modinfo_sp_add.Parameters["@dToTime"].Value = item["dToTime"].ToString();
                        sqlCommand_modinfo_sp_add.Parameters["@vcSupplierPlant"].Value = item["vcSupplierPlant"].ToString();
                        #endregion
                        sqlCommand_modinfo_sp_add.ExecuteNonQuery();
                    }
                }
                #endregion

                #region 5.sqlCommand_modinfo_sp_mod
                SqlCommand sqlCommand_modinfo_sp_mod = sqlConnection.CreateCommand();
                sqlCommand_modinfo_sp_mod.Transaction = sqlTransaction;
                sqlCommand_modinfo_sp_mod.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo_sp_mod = new StringBuilder();

                #region SQL and Parameters
                strSql_modinfo_sp_mod.AppendLine("UPDATE [dbo].[TSPMaster_SupplierPlant]");
                strSql_modinfo_sp_mod.AppendLine("   SET [dToTime] = case when @dToTime='' then '9999-12-31' else  @dToTime end");
                strSql_modinfo_sp_mod.AppendLine("      ,[vcOperatorID] = '" + strOperId + "'");
                strSql_modinfo_sp_mod.AppendLine("      ,[dOperatorTime] =GETDATE()");
                strSql_modinfo_sp_mod.AppendLine(" WHERE [LinId]=@LinId");
                sqlCommand_modinfo_sp_mod.CommandText = strSql_modinfo_sp_mod.ToString();
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@LinId", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@dToTime", "");
                #endregion
                foreach (DataRow item in dtModInfo_SP.Rows)
                {
                    if (item["status"].ToString() == "mod" && item["error"].ToString() == "")
                    {
                        #region Value
                        sqlCommand_modinfo_sp_mod.Parameters["@LinId"].Value = item["LinId"].ToString();
                        sqlCommand_modinfo_sp_mod.Parameters["@dToTime"].Value = item["dToTime"].ToString();
                        #endregion
                        sqlCommand_modinfo_sp_mod.ExecuteNonQuery();
                    }
                }
                #endregion

                #region 6.sqlCommand_modinfo_pq_add
                SqlCommand sqlCommand_modinfo_pq_add = sqlConnection.CreateCommand();
                sqlCommand_modinfo_pq_add.Transaction = sqlTransaction;
                sqlCommand_modinfo_pq_add.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo_pq_add = new StringBuilder();
                #region SQL and Parameters
                strSql_modinfo_pq_add.AppendLine("INSERT INTO [dbo].[TSPMaster_Box]");
                strSql_modinfo_pq_add.AppendLine("           ([vcPackingPlant]");
                strSql_modinfo_pq_add.AppendLine("           ,[vcPartId]");
                strSql_modinfo_pq_add.AppendLine("           ,[vcReceiver]");
                strSql_modinfo_pq_add.AppendLine("           ,[vcSupplierId]");
                strSql_modinfo_pq_add.AppendLine("           ,[vcSupplierPlant]");
                strSql_modinfo_pq_add.AppendLine("           ,[dFromTime]");
                strSql_modinfo_pq_add.AppendLine("           ,[dToTime]");
                strSql_modinfo_pq_add.AppendLine("           ,[iPackingQty]");
                strSql_modinfo_pq_add.AppendLine("           ,[vcBoxType]");
                strSql_modinfo_pq_add.AppendLine("           ,[iLength]");
                strSql_modinfo_pq_add.AppendLine("           ,[iWidth]");
                strSql_modinfo_pq_add.AppendLine("           ,[iHeight]");
                strSql_modinfo_pq_add.AppendLine("           ,[iVolume]");
                strSql_modinfo_pq_add.AppendLine("           ,[vcOperatorType]");
                strSql_modinfo_pq_add.AppendLine("           ,[vcOperatorID]");
                strSql_modinfo_pq_add.AppendLine("           ,[dOperatorTime])");
                strSql_modinfo_pq_add.AppendLine("     VALUES");
                strSql_modinfo_pq_add.AppendLine("           (@vcPackingPlant");
                strSql_modinfo_pq_add.AppendLine("           ,@vcPartId");
                strSql_modinfo_pq_add.AppendLine("           ,@vcReceiver");
                strSql_modinfo_pq_add.AppendLine("           ,@vcSupplierId");
                strSql_modinfo_pq_add.AppendLine("           ,@vcSupplierPlant");
                strSql_modinfo_pq_add.AppendLine("           ,case when @dFromTime='' then null else @dFromTime end");
                strSql_modinfo_pq_add.AppendLine("           ,case when @dToTime='' then '9999-12-31' else @dToTime end");
                strSql_modinfo_pq_add.AppendLine("           ,@iPackingQty");
                strSql_modinfo_pq_add.AppendLine("           ,case when @vcBoxType='' then null else @vcBoxType end");
                strSql_modinfo_pq_add.AppendLine("           ,case when @iLength='' then null else @iLength end");
                strSql_modinfo_pq_add.AppendLine("           ,case when @iWidth='' then null else @iWidth end");
                strSql_modinfo_pq_add.AppendLine("           ,case when @iHeight='' then null else @iHeight end");
                strSql_modinfo_pq_add.AppendLine("           ,case when @iVolume='' then null else @iVolume end");
                strSql_modinfo_pq_add.AppendLine("           ,'1'");
                strSql_modinfo_pq_add.AppendLine("           ,'" + strOperId + "'");
                strSql_modinfo_pq_add.AppendLine("           ,GETDATE())");
                sqlCommand_modinfo_pq_add.CommandText = strSql_modinfo_pq_add.ToString();
                sqlCommand_modinfo_pq_add.Parameters.AddWithValue("@vcPackingPlant", "");
                sqlCommand_modinfo_pq_add.Parameters.AddWithValue("@vcPartId", "");
                sqlCommand_modinfo_pq_add.Parameters.AddWithValue("@vcReceiver", "");
                sqlCommand_modinfo_pq_add.Parameters.AddWithValue("@vcSupplierId", "");
                sqlCommand_modinfo_pq_add.Parameters.AddWithValue("@vcSupplierPlant", "");
                sqlCommand_modinfo_pq_add.Parameters.AddWithValue("@dFromTime", "");
                sqlCommand_modinfo_pq_add.Parameters.AddWithValue("@dToTime", "");
                sqlCommand_modinfo_pq_add.Parameters.AddWithValue("@iPackingQty", "");
                sqlCommand_modinfo_pq_add.Parameters.AddWithValue("@vcBoxType", "");
                sqlCommand_modinfo_pq_add.Parameters.AddWithValue("@iLength", "");
                sqlCommand_modinfo_pq_add.Parameters.AddWithValue("@iWidth", "");
                sqlCommand_modinfo_pq_add.Parameters.AddWithValue("@iHeight", "");
                sqlCommand_modinfo_pq_add.Parameters.AddWithValue("@iVolume", "");
                #endregion
                foreach (DataRow item in dtModInfo_PQ.Rows)
                {
                    if (item["status"].ToString() == "add" && item["error"].ToString() == "")
                    {
                        #region Value
                        sqlCommand_modinfo_pq_add.Parameters["@vcPackingPlant"].Value = item["vcPackingPlant"].ToString();
                        sqlCommand_modinfo_pq_add.Parameters["@vcPartId"].Value = item["vcPartId"].ToString();
                        sqlCommand_modinfo_pq_add.Parameters["@vcReceiver"].Value = item["vcReceiver"].ToString();
                        sqlCommand_modinfo_pq_add.Parameters["@vcSupplierId"].Value = item["vcSupplierId"].ToString();
                        sqlCommand_modinfo_pq_add.Parameters["@vcSupplierPlant"].Value = item["vcSupplierPlant"].ToString();
                        sqlCommand_modinfo_pq_add.Parameters["@dFromTime"].Value = item["dFromTime"].ToString();
                        sqlCommand_modinfo_pq_add.Parameters["@dToTime"].Value = item["dToTime"].ToString();
                        sqlCommand_modinfo_pq_add.Parameters["@iPackingQty"].Value = item["iPackingQty"].ToString();
                        sqlCommand_modinfo_pq_add.Parameters["@vcBoxType"].Value = item["vcBoxType"].ToString();
                        sqlCommand_modinfo_pq_add.Parameters["@iLength"].Value = item["iLength"].ToString();
                        sqlCommand_modinfo_pq_add.Parameters["@iWidth"].Value = item["iWidth"].ToString();
                        sqlCommand_modinfo_pq_add.Parameters["@iHeight"].Value = item["iHeight"].ToString();
                        sqlCommand_modinfo_pq_add.Parameters["@iVolume"].Value = item["iVolume"].ToString();
                        #endregion
                        sqlCommand_modinfo_pq_add.ExecuteNonQuery();
                    }
                }
                #endregion

                #region 7.sqlCommand_modinfo_pq_mod
                SqlCommand sqlCommand_modinfo_pq_mod = sqlConnection.CreateCommand();
                sqlCommand_modinfo_pq_mod.Transaction = sqlTransaction;
                sqlCommand_modinfo_pq_mod.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo_pq_mod = new StringBuilder();
                #region SQL and Parameters
                strSql_modinfo_pq_mod.AppendLine("UPDATE [dbo].[TSPMaster_Box]");
                strSql_modinfo_pq_mod.AppendLine("   SET [dToTime] = case when @dToTime='' then '9999-12-31' else  @dToTime end");
                strSql_modinfo_pq_mod.AppendLine("      ,[vcOperatorID] = '" + strOperId + "'");
                strSql_modinfo_pq_mod.AppendLine("      ,[dOperatorTime] =GETDATE()");
                strSql_modinfo_pq_mod.AppendLine(" WHERE [LinId]=@LinId");
                sqlCommand_modinfo_pq_mod.CommandText = strSql_modinfo_pq_mod.ToString();
                sqlCommand_modinfo_pq_mod.Parameters.AddWithValue("@LinId", "");
                sqlCommand_modinfo_pq_mod.Parameters.AddWithValue("@dToTime", "");
                #endregion
                foreach (DataRow item in dtModInfo_PQ.Rows)
                {
                    if (item["status"].ToString() == "mod" && item["error"].ToString() == "")
                    {
                        #region Value
                        sqlCommand_modinfo_pq_mod.Parameters["@LinId"].Value = item["LinId"].ToString();
                        sqlCommand_modinfo_pq_mod.Parameters["@dToTime"].Value = item["dToTime"].ToString();
                        #endregion
                        sqlCommand_modinfo_pq_mod.ExecuteNonQuery();
                    }
                }
                #endregion

                #region 8.sqlCommand_modinfo_si_add
                SqlCommand sqlCommand_modinfo_si_add = sqlConnection.CreateCommand();
                sqlCommand_modinfo_si_add.Transaction = sqlTransaction;
                sqlCommand_modinfo_si_add.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo_si_add = new StringBuilder();
                #region SQL and Parameters
                strSql_modinfo_si_add.AppendLine("INSERT INTO [dbo].[TSPMaster_SufferIn]");
                strSql_modinfo_si_add.AppendLine("           ([vcPackingPlant]");
                strSql_modinfo_si_add.AppendLine("           ,[vcPartId]");
                strSql_modinfo_si_add.AppendLine("           ,[vcReceiver]");
                strSql_modinfo_si_add.AppendLine("           ,[vcSupplierId]");
                strSql_modinfo_si_add.AppendLine("           ,[dFromTime]");
                strSql_modinfo_si_add.AppendLine("           ,[dToTime]");
                strSql_modinfo_si_add.AppendLine("           ,[vcSufferIn]");
                strSql_modinfo_si_add.AppendLine("           ,[vcOperatorType]");
                strSql_modinfo_si_add.AppendLine("           ,[vcOperatorID]");
                strSql_modinfo_si_add.AppendLine("           ,[dOperatorTime])");
                strSql_modinfo_si_add.AppendLine("     VALUES");
                strSql_modinfo_si_add.AppendLine("           (@vcPackingPlant");
                strSql_modinfo_si_add.AppendLine("           ,@vcPartId");
                strSql_modinfo_si_add.AppendLine("           ,@vcReceiver");
                strSql_modinfo_si_add.AppendLine("           ,@vcSupplierId");
                strSql_modinfo_si_add.AppendLine("           ,case when @dFromTime='' then null else @dFromTime end");
                strSql_modinfo_si_add.AppendLine("           ,case when @dToTime='' then '9999-12-31' else @dToTime end");
                strSql_modinfo_si_add.AppendLine("           ,@vcSufferIn");
                strSql_modinfo_si_add.AppendLine("           ,'1'");
                strSql_modinfo_si_add.AppendLine("           ,'" + strOperId + "'");
                strSql_modinfo_si_add.AppendLine("           ,GETDATE())");
                sqlCommand_modinfo_si_add.CommandText = strSql_modinfo_si_add.ToString();
                sqlCommand_modinfo_si_add.Parameters.AddWithValue("@vcPackingPlant", "");
                sqlCommand_modinfo_si_add.Parameters.AddWithValue("@vcPartId", "");
                sqlCommand_modinfo_si_add.Parameters.AddWithValue("@vcReceiver", "");
                sqlCommand_modinfo_si_add.Parameters.AddWithValue("@vcSupplierId", "");
                sqlCommand_modinfo_si_add.Parameters.AddWithValue("@dFromTime", "");
                sqlCommand_modinfo_si_add.Parameters.AddWithValue("@dToTime", "");
                sqlCommand_modinfo_si_add.Parameters.AddWithValue("@vcSufferIn", "");
                #endregion
                foreach (DataRow item in dtModInfo_SI.Rows)
                {
                    if (item["status"].ToString() == "add" && item["error"].ToString() == "")
                    {
                        #region Value
                        sqlCommand_modinfo_si_add.Parameters["@vcPackingPlant"].Value = item["vcPackingPlant"].ToString();
                        sqlCommand_modinfo_si_add.Parameters["@vcPartId"].Value = item["vcPartId"].ToString();
                        sqlCommand_modinfo_si_add.Parameters["@vcReceiver"].Value = item["vcReceiver"].ToString();
                        sqlCommand_modinfo_si_add.Parameters["@vcSupplierId"].Value = item["vcSupplierId"].ToString();
                        sqlCommand_modinfo_si_add.Parameters["@dFromTime"].Value = item["dFromTime"].ToString();
                        sqlCommand_modinfo_si_add.Parameters["@dToTime"].Value = item["dToTime"].ToString();
                        sqlCommand_modinfo_si_add.Parameters["@vcSufferIn"].Value = item["vcSufferIn"].ToString();
                        #endregion
                        sqlCommand_modinfo_si_add.ExecuteNonQuery();
                    }
                }
                #endregion

                #region 9.sqlCommand_modinfo_si_mod
                SqlCommand sqlCommand_modinfo_si_mod = sqlConnection.CreateCommand();
                sqlCommand_modinfo_si_mod.Transaction = sqlTransaction;
                sqlCommand_modinfo_si_mod.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo_si_mod = new StringBuilder();
                #region SQL and Parameters
                strSql_modinfo_si_mod.AppendLine("UPDATE [dbo].[TSPMaster_SufferIn]");
                strSql_modinfo_si_mod.AppendLine("   SET [dToTime] = case when @dToTime='' then '9999-12-31' else  @dToTime end");
                strSql_modinfo_si_mod.AppendLine("      ,[vcOperatorID] = '" + strOperId + "'");
                strSql_modinfo_si_mod.AppendLine("      ,[dOperatorTime] =GETDATE()");
                strSql_modinfo_si_mod.AppendLine(" WHERE [LinId]=@LinId");
                sqlCommand_modinfo_si_mod.CommandText = strSql_modinfo_si_mod.ToString();
                sqlCommand_modinfo_si_mod.Parameters.AddWithValue("@LinId", "");
                sqlCommand_modinfo_si_mod.Parameters.AddWithValue("@dToTime", "");
                #endregion
                foreach (DataRow item in dtModInfo_SI.Rows)
                {
                    if (item["status"].ToString() == "mod" && item["error"].ToString() == "")
                    {
                        #region Value
                        sqlCommand_modinfo_si_mod.Parameters["@LinId"].Value = item["LinId"].ToString();
                        sqlCommand_modinfo_si_mod.Parameters["@dToTime"].Value = item["dToTime"].ToString();
                        #endregion
                        sqlCommand_modinfo_si_mod.ExecuteNonQuery();
                    }
                }
                #endregion

                #region 10.sqlCommand_operhistory
                SqlCommand sqlCommand_operhistory = sqlConnection.CreateCommand();
                sqlCommand_operhistory.Transaction = sqlTransaction;
                sqlCommand_operhistory.CommandType = CommandType.Text;
                StringBuilder strSql_operhistory = new StringBuilder();
                #region SQL and Parameters
                strSql_operhistory.AppendLine("INSERT INTO [dbo].[TSPMaster_OperHistory]");
                strSql_operhistory.AppendLine("           ([vcPackingPlant]");
                strSql_operhistory.AppendLine("           ,[vcPartId]");
                strSql_operhistory.AppendLine("           ,[vcReceiver]");
                strSql_operhistory.AppendLine("           ,[vcSupplierId]");
                strSql_operhistory.AppendLine("           ,[vcChangeItem]");
                strSql_operhistory.AppendLine("           ,[vcAction]");
                strSql_operhistory.AppendLine("           ,[vcActionTime]");
                strSql_operhistory.AppendLine("           ,[vcOperatorID]");
                strSql_operhistory.AppendLine("           ,[dOperatorTime])");
                strSql_operhistory.AppendLine("     VALUES");
                strSql_operhistory.AppendLine("           (@vcPackingPlant");
                strSql_operhistory.AppendLine("           ,@vcPartId");
                strSql_operhistory.AppendLine("           ,@vcReceiver");
                strSql_operhistory.AppendLine("           ,@vcSupplierId");
                strSql_operhistory.AppendLine("           ,@vcChangeItem");
                strSql_operhistory.AppendLine("           ,@vcAction");
                strSql_operhistory.AppendLine("           ,@vcActionTime");
                strSql_operhistory.AppendLine("           ,'" + strOperId + "'");
                strSql_operhistory.AppendLine("           ,GETDATE())");
                sqlCommand_operhistory.CommandText = strSql_operhistory.ToString();
                sqlCommand_operhistory.Parameters.AddWithValue("@vcPackingPlant", "");
                sqlCommand_operhistory.Parameters.AddWithValue("@vcPartId", "");
                sqlCommand_operhistory.Parameters.AddWithValue("@vcReceiver", "");
                sqlCommand_operhistory.Parameters.AddWithValue("@vcSupplierId", "");
                sqlCommand_operhistory.Parameters.AddWithValue("@vcChangeItem", "");
                sqlCommand_operhistory.Parameters.AddWithValue("@vcAction", "");
                sqlCommand_operhistory.Parameters.AddWithValue("@vcActionTime", "");
                #endregion
                foreach (DataRow item in dtOperHistory.Rows)
                {
                    if (item["error"].ToString() == "")
                    {
                        #region Value
                        sqlCommand_operhistory.Parameters["@vcPackingPlant"].Value = item["vcPackingPlant"].ToString();
                        sqlCommand_operhistory.Parameters["@vcPartId"].Value = item["vcPartId"].ToString();
                        sqlCommand_operhistory.Parameters["@vcReceiver"].Value = item["vcReceiver"].ToString();
                        sqlCommand_operhistory.Parameters["@vcSupplierId"].Value = item["vcSupplierId"].ToString();
                        sqlCommand_operhistory.Parameters["@vcChangeItem"].Value = item["vcChangeItem"].ToString();
                        sqlCommand_operhistory.Parameters["@vcAction"].Value = item["vcAction"].ToString();
                        sqlCommand_operhistory.Parameters["@vcActionTime"].Value = item["vcActionTime"].ToString();
                        #endregion
                        sqlCommand_operhistory.ExecuteNonQuery();
                    }
                }
                #endregion

                #region 11.sqlCommand_updateinfo
                SqlCommand sqlCommand_updateinfo = sqlConnection.CreateCommand();
                sqlCommand_updateinfo.Transaction = sqlTransaction;
                sqlCommand_updateinfo.CommandType = CommandType.Text;
                StringBuilder strSql_updateinfo = new StringBuilder();
                #region SQL and Parameters
                strSql_updateinfo.AppendLine("update TSPMaster set vcBillType=");
                strSql_updateinfo.AppendLine("case when isnull(vcInOut,'')='' or  isnull(vcOESP,'')='' then null ");
                strSql_updateinfo.AppendLine("	else ");
                strSql_updateinfo.AppendLine("		case when isnull(vcInOut,'')='1' and  isnull(vcOESP,'')='0' then 'A1'--A1=外注+o");
                strSql_updateinfo.AppendLine("		     when isnull(vcInOut,'')='1' and  isnull(vcOESP,'')='1' then 'A2'--A2=外注+x");
                strSql_updateinfo.AppendLine("		     when isnull(vcInOut,'')='0' and  isnull(vcOESP,'')='0' then 'A3'--A3=内制+o");
                strSql_updateinfo.AppendLine("		     when isnull(vcInOut,'')='0' and  isnull(vcOESP,'')='1' then 'A4'--A4=内制+x");
                strSql_updateinfo.AppendLine("			else null");
                strSql_updateinfo.AppendLine("		end");
                strSql_updateinfo.AppendLine("end");
                strSql_updateinfo.AppendLine("where isnull(vcBillType,'')='' ");
                strSql_updateinfo.AppendLine("update TSPMaster set vcOrderingMethod='0' where isnull(vcOrderingMethod,'')='' ");
                strSql_updateinfo.AppendLine("DELETE FROM [TSPMaster_OperHistory] WHERE [dOperatorTime]<=DATEADD(MONTH,-6,GETDATE()) ");
                sqlCommand_updateinfo.CommandText = strSql_updateinfo.ToString();
                #endregion
                sqlCommand_updateinfo.ExecuteNonQuery();
                #endregion

                #region 12.sqlCommand_modinfo_child_from
                SqlCommand sqlCommand_modinfo_child_from = sqlConnection.CreateCommand();
                sqlCommand_modinfo_child_from.Transaction = sqlTransaction;
                sqlCommand_modinfo_child_from.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo_child_from = new StringBuilder();

                #region SQL and Parameters
                strSql_modinfo_child_from.AppendLine("DELETE FROM TSPMaster_SupplierPlant WHERE dToTime<@dFromTime AND vcPackingPlant=@vcPackingPlant AND vcPartId=@vcPartId AND vcReceiver=@vcReceiver AND vcSupplierId=@vcSupplierId");
                strSql_modinfo_child_from.AppendLine("DELETE FROM TSPMaster_Box WHERE dToTime<@dFromTime AND vcPackingPlant=@vcPackingPlant AND vcPartId=@vcPartId AND vcReceiver=@vcReceiver AND vcSupplierId=@vcSupplierId");
                strSql_modinfo_child_from.AppendLine("DELETE FROM TSPMaster_SufferIn WHERE dToTime<@dFromTime AND vcPackingPlant=@vcPackingPlant AND vcPartId=@vcPartId AND vcReceiver=@vcReceiver AND vcSupplierId=@vcSupplierId");
                strSql_modinfo_child_from.AppendLine("UPDATE A SET A.dFromTime=@dFromTime FROM ");
                strSql_modinfo_child_from.AppendLine("(SELECT ROW_NUMBER() OVER(partition by vcPackingPlant,vcPartId,vcReceiver,vcSupplierId order by dFromTime) [rank],");
                strSql_modinfo_child_from.AppendLine("LinId,vcPackingPlant,vcPartId,vcReceiver,vcSupplierId,dFromTime,dToTime");
                strSql_modinfo_child_from.AppendLine("FROM [TSPMaster_SupplierPlant] WHERE [vcOperatorType]='1' AND vcPackingPlant=@vcPackingPlant AND vcPartId=@vcPartId AND vcReceiver=@vcReceiver AND vcSupplierId=@vcSupplierId)A ");
                strSql_modinfo_child_from.AppendLine("WHERE A.rank='1'");
                strSql_modinfo_child_from.AppendLine("UPDATE A SET A.dFromTime=@dFromTime FROM ");
                strSql_modinfo_child_from.AppendLine("(SELECT ROW_NUMBER() OVER(partition by vcPackingPlant,vcPartId,vcReceiver,vcSupplierId order by dFromTime) [rank],");
                strSql_modinfo_child_from.AppendLine("LinId,vcPackingPlant,vcPartId,vcReceiver,vcSupplierId,dFromTime,dToTime");
                strSql_modinfo_child_from.AppendLine("FROM [TSPMaster_Box] WHERE [vcOperatorType]='1' AND vcPackingPlant=@vcPackingPlant AND vcPartId=@vcPartId AND vcReceiver=@vcReceiver AND vcSupplierId=@vcSupplierId)A ");
                strSql_modinfo_child_from.AppendLine("WHERE A.rank='1'");
                strSql_modinfo_child_from.AppendLine("UPDATE A SET A.dFromTime=@dFromTime FROM ");
                strSql_modinfo_child_from.AppendLine("(SELECT ROW_NUMBER() OVER(partition by vcPackingPlant,vcPartId,vcReceiver,vcSupplierId order by dFromTime) [rank],");
                strSql_modinfo_child_from.AppendLine("LinId,vcPackingPlant,vcPartId,vcReceiver,vcSupplierId,dFromTime,dToTime");
                strSql_modinfo_child_from.AppendLine("FROM [TSPMaster_SufferIn] WHERE [vcOperatorType]='1' AND vcPackingPlant=@vcPackingPlant AND vcPartId=@vcPartId AND vcReceiver=@vcReceiver AND vcSupplierId=@vcSupplierId)A ");
                strSql_modinfo_child_from.AppendLine("WHERE A.rank='1'");
                sqlCommand_modinfo_child_from.CommandText = strSql_modinfo_child_from.ToString();
                sqlCommand_modinfo_child_from.Parameters.AddWithValue("@vcPackingPlant", "");
                sqlCommand_modinfo_child_from.Parameters.AddWithValue("@vcPartId", "");
                sqlCommand_modinfo_child_from.Parameters.AddWithValue("@vcReceiver", "");
                sqlCommand_modinfo_child_from.Parameters.AddWithValue("@vcSupplierId", "");
                sqlCommand_modinfo_child_from.Parameters.AddWithValue("@dFromTime", "");
                #endregion
                foreach (DataRow item in dtSPChild.Rows)
                {
                    if (item["vcFlag"].ToString() == "from")
                    {
                        #region Value
                        sqlCommand_modinfo_child_from.Parameters["@vcPackingPlant"].Value = item["vcPackingPlant"].ToString();
                        sqlCommand_modinfo_child_from.Parameters["@vcPartId"].Value = item["vcPartId"].ToString();
                        sqlCommand_modinfo_child_from.Parameters["@vcReceiver"].Value = item["vcReceiver"].ToString();
                        sqlCommand_modinfo_child_from.Parameters["@vcSupplierId"].Value = item["vcSupplierId"].ToString();
                        sqlCommand_modinfo_child_from.Parameters["@dFromTime"].Value = item["dFromTime"].ToString();
                        #endregion
                        sqlCommand_modinfo_child_from.ExecuteNonQuery();
                    }
                }
                #endregion

                #region 13.sqlCommand_modinfo_child_to
                SqlCommand sqlCommand_modinfo_child_to = sqlConnection.CreateCommand();
                sqlCommand_modinfo_child_to.Transaction = sqlTransaction;
                sqlCommand_modinfo_child_to.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo_child_to = new StringBuilder();

                #region SQL and Parameters
                strSql_modinfo_child_to.AppendLine("UPDATE [TSPMaster_SupplierPlant] SET dToTime=@dToTime WHERE dToTime>@dToTime AND vcPackingPlant=@vcPackingPlant AND vcPartId=@vcPartId AND vcReceiver=@vcReceiver AND vcSupplierId=@vcSupplierId");
                strSql_modinfo_child_to.AppendLine("UPDATE [TSPMaster_Box] SET dToTime=@dToTime WHERE dToTime>@dToTime AND vcPackingPlant=@vcPackingPlant AND vcPartId=@vcPartId AND vcReceiver=@vcReceiver AND vcSupplierId=@vcSupplierId");
                strSql_modinfo_child_to.AppendLine("UPDATE [TSPMaster_SufferIn] SET dToTime=@dToTime WHERE dToTime>@dToTime AND vcPackingPlant=@vcPackingPlant AND vcPartId=@vcPartId AND vcReceiver=@vcReceiver AND vcSupplierId=@vcSupplierId");
                strSql_modinfo_child_to.AppendLine("DELETE FROM [TSPMaster_SupplierPlant] WHERE dToTime<dFromTime AND vcPackingPlant=@vcPackingPlant AND vcPartId=@vcPartId AND vcReceiver=@vcReceiver AND vcSupplierId=@vcSupplierId");
                strSql_modinfo_child_to.AppendLine("DELETE FROM [TSPMaster_Box] WHERE dToTime<dFromTime AND vcPackingPlant=@vcPackingPlant AND vcPartId=@vcPartId AND vcReceiver=@vcReceiver AND vcSupplierId=@vcSupplierId");
                strSql_modinfo_child_to.AppendLine("DELETE FROM [TSPMaster_SufferIn] WHERE dToTime<dFromTime AND vcPackingPlant=@vcPackingPlant AND vcPartId=@vcPartId AND vcReceiver=@vcReceiver AND vcSupplierId=@vcSupplierId");
                strSql_modinfo_child_to.AppendLine("");
                strSql_modinfo_child_to.AppendLine("update A set A.dToTime=@dToTime from ");
                strSql_modinfo_child_to.AppendLine("(SELECT ROW_NUMBER() OVER(partition by vcPackingPlant,vcPartId,vcReceiver,vcSupplierId order by dFromTime desc) [rank],");
                strSql_modinfo_child_to.AppendLine("LinId,vcPackingPlant,vcPartId,vcReceiver,vcSupplierId,dFromTime,dToTime,vcSupplierPlant ");
                strSql_modinfo_child_to.AppendLine("FROM [TSPMaster_SupplierPlant]  ");
                strSql_modinfo_child_to.AppendLine("WHERE [vcOperatorType]='1' AND vcPackingPlant=@vcPackingPlant AND vcPartId=@vcPartId AND vcReceiver=@vcReceiver AND vcSupplierId=@vcSupplierId");
                strSql_modinfo_child_to.AppendLine(")A where A.rank='1' ");
                strSql_modinfo_child_to.AppendLine("");
                strSql_modinfo_child_to.AppendLine("update A set A.dToTime=@dToTime from ");
                strSql_modinfo_child_to.AppendLine("(SELECT ROW_NUMBER() OVER(partition by vcPackingPlant,vcPartId,vcReceiver,vcSupplierId order by dFromTime) [rank],");
                strSql_modinfo_child_to.AppendLine("LinId,vcPackingPlant,vcPartId,vcReceiver,vcSupplierId,dFromTime,dToTime,iPackingQty,vcBoxType,iLength,iWidth,iHeight,iVolume ");
                strSql_modinfo_child_to.AppendLine("FROM [TSPMaster_Box] ");
                strSql_modinfo_child_to.AppendLine("WHERE [vcOperatorType]='1' AND vcPackingPlant=@vcPackingPlant AND vcPartId=@vcPartId AND vcReceiver=@vcReceiver AND vcSupplierId=@vcSupplierId");
                strSql_modinfo_child_to.AppendLine(")A where A.rank='1' ");
                strSql_modinfo_child_to.AppendLine("");
                strSql_modinfo_child_to.AppendLine("update A set A.dToTime=@dToTime from ");
                strSql_modinfo_child_to.AppendLine("(SELECT ROW_NUMBER() OVER(partition by vcPackingPlant,vcPartId,vcReceiver,vcSupplierId order by dFromTime) [rank],");
                strSql_modinfo_child_to.AppendLine("LinId,vcPackingPlant,vcPartId,vcReceiver,vcSupplierId,dFromTime,dToTime,vcSufferIn");
                strSql_modinfo_child_to.AppendLine("FROM [TSPMaster_SufferIn] ");
                strSql_modinfo_child_to.AppendLine("WHERE [vcOperatorType]='1' AND vcPackingPlant=@vcPackingPlant AND vcPartId=@vcPartId AND vcReceiver=@vcReceiver AND vcSupplierId=@vcSupplierId");
                strSql_modinfo_child_to.AppendLine(")A where A.rank='1' ");
                sqlCommand_modinfo_child_to.CommandText = strSql_modinfo_child_to.ToString();
                sqlCommand_modinfo_child_to.Parameters.AddWithValue("@vcPackingPlant", "");
                sqlCommand_modinfo_child_to.Parameters.AddWithValue("@vcPartId", "");
                sqlCommand_modinfo_child_to.Parameters.AddWithValue("@vcReceiver", "");
                sqlCommand_modinfo_child_to.Parameters.AddWithValue("@vcSupplierId", "");
                sqlCommand_modinfo_child_to.Parameters.AddWithValue("@dToTime", "");
                #endregion
                foreach (DataRow item in dtSPChild.Rows)
                {
                    if (item["vcFlag"].ToString() == "to")
                    {
                        #region Value
                        sqlCommand_modinfo_child_to.Parameters["@vcPackingPlant"].Value = item["vcPackingPlant"].ToString();
                        sqlCommand_modinfo_child_to.Parameters["@vcPartId"].Value = item["vcPartId"].ToString();
                        sqlCommand_modinfo_child_to.Parameters["@vcReceiver"].Value = item["vcReceiver"].ToString();
                        sqlCommand_modinfo_child_to.Parameters["@vcSupplierId"].Value = item["vcSupplierId"].ToString();
                        sqlCommand_modinfo_child_to.Parameters["@dToTime"].Value = item["dToTime"].ToString();
                        #endregion
                        sqlCommand_modinfo_child_to.ExecuteNonQuery();
                    }
                }
                #endregion

                #region 14.sqlCommand_childinfo_gq_add
                SqlCommand sqlCommand_childinfo_gq_add = sqlConnection.CreateCommand();
                sqlCommand_childinfo_gq_add.Transaction = sqlTransaction;
                sqlCommand_childinfo_gq_add.CommandType = CommandType.Text;
                StringBuilder strSql_childinfo_gq_add = new StringBuilder();

                #region SQL and Parameters
                strSql_childinfo_gq_add.AppendLine("INSERT INTO [dbo].[TSPMaster_SupplierPlant]");
                strSql_childinfo_gq_add.AppendLine("           ([vcPackingPlant]");
                strSql_childinfo_gq_add.AppendLine("           ,[vcPartId]");
                strSql_childinfo_gq_add.AppendLine("           ,[vcReceiver]");
                strSql_childinfo_gq_add.AppendLine("           ,[vcSupplierId]");
                strSql_childinfo_gq_add.AppendLine("           ,[dFromTime]");
                strSql_childinfo_gq_add.AppendLine("           ,[dToTime]");
                strSql_childinfo_gq_add.AppendLine("           ,[vcSupplierPlant]");
                strSql_childinfo_gq_add.AppendLine("           ,[vcOperatorType]");
                strSql_childinfo_gq_add.AppendLine("           ,[vcOperatorID]");
                strSql_childinfo_gq_add.AppendLine("           ,[dOperatorTime])");
                strSql_childinfo_gq_add.AppendLine("     VALUES");
                strSql_childinfo_gq_add.AppendLine("           (@vcPackingPlant");
                strSql_childinfo_gq_add.AppendLine("           ,@vcPartId");
                strSql_childinfo_gq_add.AppendLine("           ,@vcReceiver");
                strSql_childinfo_gq_add.AppendLine("           ,@vcSupplierId");
                strSql_childinfo_gq_add.AppendLine("           ,case when @dFromTime='' then null else @dFromTime end");
                strSql_childinfo_gq_add.AppendLine("           ,case when @dToTime='' then '9999-12-31' else @dToTime end");
                strSql_childinfo_gq_add.AppendLine("           ,@vcSupplierPlant");
                strSql_childinfo_gq_add.AppendLine("           ,'1'");
                strSql_childinfo_gq_add.AppendLine("           ,'" + strOperId + "'");
                strSql_childinfo_gq_add.AppendLine("           ,GETDATE())");
                sqlCommand_childinfo_gq_add.CommandText = strSql_childinfo_gq_add.ToString();
                sqlCommand_childinfo_gq_add.Parameters.AddWithValue("@vcPackingPlant", "");
                sqlCommand_childinfo_gq_add.Parameters.AddWithValue("@vcPartId", "");
                sqlCommand_childinfo_gq_add.Parameters.AddWithValue("@vcReceiver", "");
                sqlCommand_childinfo_gq_add.Parameters.AddWithValue("@vcSupplierId", "");
                sqlCommand_childinfo_gq_add.Parameters.AddWithValue("@dFromTime", "");
                sqlCommand_childinfo_gq_add.Parameters.AddWithValue("@dToTime", "");
                sqlCommand_childinfo_gq_add.Parameters.AddWithValue("@vcSupplierPlant", "");
                #endregion
                foreach (DataRow item in dtChildInfo_gq.Rows)
                {
                    if (item["status"].ToString() == "add" && item["error"].ToString() == "")
                    {
                        #region Value
                        sqlCommand_childinfo_gq_add.Parameters["@vcPackingPlant"].Value = item["vcPackingPlant"].ToString();
                        sqlCommand_childinfo_gq_add.Parameters["@vcPartId"].Value = item["vcPartId"].ToString();
                        sqlCommand_childinfo_gq_add.Parameters["@vcReceiver"].Value = item["vcReceiver"].ToString();
                        sqlCommand_childinfo_gq_add.Parameters["@vcSupplierId"].Value = item["vcSupplierId"].ToString();
                        sqlCommand_childinfo_gq_add.Parameters["@dFromTime"].Value = item["dFromTime"].ToString();
                        sqlCommand_childinfo_gq_add.Parameters["@dToTime"].Value = item["dToTime"].ToString();
                        sqlCommand_childinfo_gq_add.Parameters["@vcSupplierPlant"].Value = item["vcSupplierPlant"].ToString();
                        #endregion
                        sqlCommand_childinfo_gq_add.ExecuteNonQuery();
                    }
                }
                #endregion

                #region 15.sqlCommand_childinfo_hz_add
                SqlCommand sqlCommand_childinfo_hz_add = sqlConnection.CreateCommand();
                sqlCommand_childinfo_hz_add.Transaction = sqlTransaction;
                sqlCommand_childinfo_hz_add.CommandType = CommandType.Text;
                StringBuilder strSql_childinfo_hz_add = new StringBuilder();

                #region SQL and Parameters
                strSql_childinfo_hz_add.AppendLine("INSERT INTO [dbo].[TSPMaster_Box]");
                strSql_childinfo_hz_add.AppendLine("           ([vcPackingPlant]");
                strSql_childinfo_hz_add.AppendLine("           ,[vcPartId]");
                strSql_childinfo_hz_add.AppendLine("           ,[vcReceiver]");
                strSql_childinfo_hz_add.AppendLine("           ,[vcSupplierId]");
                strSql_childinfo_hz_add.AppendLine("           ,[vcSupplierPlant]");
                strSql_childinfo_hz_add.AppendLine("           ,[dFromTime]");
                strSql_childinfo_hz_add.AppendLine("           ,[dToTime]");
                strSql_childinfo_hz_add.AppendLine("           ,[iPackingQty]");
                strSql_childinfo_hz_add.AppendLine("           ,[vcBoxType]");
                strSql_childinfo_hz_add.AppendLine("           ,[iLength]");
                strSql_childinfo_hz_add.AppendLine("           ,[iWidth]");
                strSql_childinfo_hz_add.AppendLine("           ,[iHeight]");
                strSql_childinfo_hz_add.AppendLine("           ,[iVolume]");
                strSql_childinfo_hz_add.AppendLine("           ,[vcOperatorType]");
                strSql_childinfo_hz_add.AppendLine("           ,[vcOperatorID]");
                strSql_childinfo_hz_add.AppendLine("           ,[dOperatorTime])");
                strSql_childinfo_hz_add.AppendLine("     VALUES");
                strSql_childinfo_hz_add.AppendLine("           (@vcPackingPlant");
                strSql_childinfo_hz_add.AppendLine("           ,@vcPartId");
                strSql_childinfo_hz_add.AppendLine("           ,@vcReceiver");
                strSql_childinfo_hz_add.AppendLine("           ,@vcSupplierId");
                strSql_childinfo_hz_add.AppendLine("           ,@vcSupplierPlant");
                strSql_childinfo_hz_add.AppendLine("           ,case when @dFromTime='' then null else @dFromTime end");
                strSql_childinfo_hz_add.AppendLine("           ,case when @dToTime='' then '9999-12-31' else @dToTime end");
                strSql_childinfo_hz_add.AppendLine("           ,@iPackingQty");
                strSql_childinfo_hz_add.AppendLine("           ,case when @vcBoxType='' then null else @vcBoxType end");
                strSql_childinfo_hz_add.AppendLine("           ,case when @iLength='' then null else @iLength end");
                strSql_childinfo_hz_add.AppendLine("           ,case when @iWidth='' then null else @iWidth end");
                strSql_childinfo_hz_add.AppendLine("           ,case when @iHeight='' then null else @iHeight end");
                strSql_childinfo_hz_add.AppendLine("           ,case when @iVolume='' then null else @iVolume end");
                strSql_childinfo_hz_add.AppendLine("           ,'1'");
                strSql_childinfo_hz_add.AppendLine("           ,'" + strOperId + "'");
                strSql_childinfo_hz_add.AppendLine("           ,GETDATE())");
                sqlCommand_childinfo_hz_add.CommandText = strSql_childinfo_hz_add.ToString();
                sqlCommand_childinfo_hz_add.Parameters.AddWithValue("@vcPackingPlant", "");
                sqlCommand_childinfo_hz_add.Parameters.AddWithValue("@vcPartId", "");
                sqlCommand_childinfo_hz_add.Parameters.AddWithValue("@vcReceiver", "");
                sqlCommand_childinfo_hz_add.Parameters.AddWithValue("@vcSupplierId", "");
                sqlCommand_childinfo_hz_add.Parameters.AddWithValue("@vcSupplierPlant", "");
                sqlCommand_childinfo_hz_add.Parameters.AddWithValue("@dFromTime", "");
                sqlCommand_childinfo_hz_add.Parameters.AddWithValue("@dToTime", "");
                sqlCommand_childinfo_hz_add.Parameters.AddWithValue("@iPackingQty", "");
                sqlCommand_childinfo_hz_add.Parameters.AddWithValue("@vcBoxType", "");
                sqlCommand_childinfo_hz_add.Parameters.AddWithValue("@iLength", "");
                sqlCommand_childinfo_hz_add.Parameters.AddWithValue("@iWidth", "");
                sqlCommand_childinfo_hz_add.Parameters.AddWithValue("@iHeight", "");
                sqlCommand_childinfo_hz_add.Parameters.AddWithValue("@iVolume", "");
                #endregion
                foreach (DataRow item in dtChildInfo_hz.Rows)
                {
                    if (item["status"].ToString() == "add" && item["error"].ToString() == "")
                    {
                        #region Value
                        sqlCommand_childinfo_hz_add.Parameters["@vcPackingPlant"].Value = item["vcPackingPlant"].ToString();
                        sqlCommand_childinfo_hz_add.Parameters["@vcPartId"].Value = item["vcPartId"].ToString();
                        sqlCommand_childinfo_hz_add.Parameters["@vcReceiver"].Value = item["vcReceiver"].ToString();
                        sqlCommand_childinfo_hz_add.Parameters["@vcSupplierId"].Value = item["vcSupplierId"].ToString();
                        sqlCommand_childinfo_hz_add.Parameters["@vcSupplierPlant"].Value = item["vcSupplierPlant"].ToString();
                        sqlCommand_childinfo_hz_add.Parameters["@dFromTime"].Value = item["dFromTime"].ToString();
                        sqlCommand_childinfo_hz_add.Parameters["@dToTime"].Value = item["dToTime"].ToString();
                        sqlCommand_childinfo_hz_add.Parameters["@iPackingQty"].Value = item["iPackingQty"].ToString();
                        sqlCommand_childinfo_hz_add.Parameters["@vcBoxType"].Value = item["vcBoxType"].ToString();
                        sqlCommand_childinfo_hz_add.Parameters["@iLength"].Value = item["iLength"].ToString();
                        sqlCommand_childinfo_hz_add.Parameters["@iWidth"].Value = item["iWidth"].ToString();
                        sqlCommand_childinfo_hz_add.Parameters["@iHeight"].Value = item["iHeight"].ToString();
                        sqlCommand_childinfo_hz_add.Parameters["@iVolume"].Value = item["iVolume"].ToString();
                        #endregion
                        sqlCommand_childinfo_hz_add.ExecuteNonQuery();
                    }
                }
                #endregion

                #region 16.sqlCommand_childinfo_sr_add
                SqlCommand sqlCommand_childinfo_sr_add = sqlConnection.CreateCommand();
                sqlCommand_childinfo_sr_add.Transaction = sqlTransaction;
                sqlCommand_childinfo_sr_add.CommandType = CommandType.Text;
                StringBuilder strSql_childinfo_sr_add = new StringBuilder();

                #region SQL and Parameters
                strSql_childinfo_sr_add.AppendLine("INSERT INTO [dbo].[TSPMaster_SufferIn]");
                strSql_childinfo_sr_add.AppendLine("           ([vcPackingPlant]");
                strSql_childinfo_sr_add.AppendLine("           ,[vcPartId]");
                strSql_childinfo_sr_add.AppendLine("           ,[vcReceiver]");
                strSql_childinfo_sr_add.AppendLine("           ,[vcSupplierId]");
                strSql_childinfo_sr_add.AppendLine("           ,[dFromTime]");
                strSql_childinfo_sr_add.AppendLine("           ,[dToTime]");
                strSql_childinfo_sr_add.AppendLine("           ,[vcSufferIn]");
                strSql_childinfo_sr_add.AppendLine("           ,[vcOperatorType]");
                strSql_childinfo_sr_add.AppendLine("           ,[vcOperatorID]");
                strSql_childinfo_sr_add.AppendLine("           ,[dOperatorTime])");
                strSql_childinfo_sr_add.AppendLine("     VALUES");
                strSql_childinfo_sr_add.AppendLine("           (@vcPackingPlant");
                strSql_childinfo_sr_add.AppendLine("           ,@vcPartId");
                strSql_childinfo_sr_add.AppendLine("           ,@vcReceiver");
                strSql_childinfo_sr_add.AppendLine("           ,@vcSupplierId");
                strSql_childinfo_sr_add.AppendLine("           ,case when @dFromTime='' then null else @dFromTime end");
                strSql_childinfo_sr_add.AppendLine("           ,case when @dToTime='' then '9999-12-31' else @dToTime end");
                strSql_childinfo_sr_add.AppendLine("           ,@vcSufferIn");
                strSql_childinfo_sr_add.AppendLine("           ,'1'");
                strSql_childinfo_sr_add.AppendLine("           ,'" + strOperId + "'");
                strSql_childinfo_sr_add.AppendLine("           ,GETDATE())");
                sqlCommand_childinfo_sr_add.CommandText = strSql_childinfo_sr_add.ToString();
                sqlCommand_childinfo_sr_add.Parameters.AddWithValue("@vcPackingPlant", "");
                sqlCommand_childinfo_sr_add.Parameters.AddWithValue("@vcPartId", "");
                sqlCommand_childinfo_sr_add.Parameters.AddWithValue("@vcReceiver", "");
                sqlCommand_childinfo_sr_add.Parameters.AddWithValue("@vcSupplierId", "");
                sqlCommand_childinfo_sr_add.Parameters.AddWithValue("@dFromTime", "");
                sqlCommand_childinfo_sr_add.Parameters.AddWithValue("@dToTime", "");
                sqlCommand_childinfo_sr_add.Parameters.AddWithValue("@vcSufferIn", "");
                #endregion
                foreach (DataRow item in dtChildInfo_sr.Rows)
                {
                    if (item["status"].ToString() == "add" && item["error"].ToString() == "")
                    {
                        #region Value
                        sqlCommand_childinfo_sr_add.Parameters["@vcPackingPlant"].Value = item["vcPackingPlant"].ToString();
                        sqlCommand_childinfo_sr_add.Parameters["@vcPartId"].Value = item["vcPartId"].ToString();
                        sqlCommand_childinfo_sr_add.Parameters["@vcReceiver"].Value = item["vcReceiver"].ToString();
                        sqlCommand_childinfo_sr_add.Parameters["@vcSupplierId"].Value = item["vcSupplierId"].ToString();
                        sqlCommand_childinfo_sr_add.Parameters["@dFromTime"].Value = item["dFromTime"].ToString();
                        sqlCommand_childinfo_sr_add.Parameters["@dToTime"].Value = item["dToTime"].ToString();
                        sqlCommand_childinfo_sr_add.Parameters["@vcSufferIn"].Value = item["vcSufferIn"].ToString();
                        #endregion
                        sqlCommand_childinfo_sr_add.ExecuteNonQuery();
                    }
                }
                #endregion

                #region 17.sqlCommand_childinfo_gq_mod
                SqlCommand sqlCommand_childinfo_gq_mod = sqlConnection.CreateCommand();
                sqlCommand_childinfo_gq_mod.Transaction = sqlTransaction;
                sqlCommand_childinfo_gq_mod.CommandType = CommandType.Text;
                StringBuilder strSql_childinfo_gq_mod = new StringBuilder();

                #region SQL and Parameters
                strSql_childinfo_gq_mod.AppendLine("UPDATE [dbo].[TSPMaster_SupplierPlant]");
                strSql_childinfo_gq_mod.AppendLine("   SET [dFromTime] = case when @dFromTime='' then null else @dFromTime end");
                strSql_childinfo_gq_mod.AppendLine("      ,[dToTime] = case when @dToTime='' then '9999-12-31' else @dToTime end");
                strSql_childinfo_gq_mod.AppendLine("      ,[vcSupplierPlant] = @vcSupplierPlant");
                strSql_childinfo_gq_mod.AppendLine("      ,[vcOperatorType] = '1'");
                strSql_childinfo_gq_mod.AppendLine("      ,[vcOperatorID] = '" + strOperId + "'");
                strSql_childinfo_gq_mod.AppendLine("      ,[dOperatorTime] = GETDATE()");
                strSql_childinfo_gq_mod.AppendLine(" WHERE [LinId]=@LinId");
                sqlCommand_childinfo_gq_mod.CommandText = strSql_childinfo_gq_mod.ToString();
                sqlCommand_childinfo_gq_mod.Parameters.AddWithValue("@LinId", "");
                sqlCommand_childinfo_gq_mod.Parameters.AddWithValue("@dFromTime", "");
                sqlCommand_childinfo_gq_mod.Parameters.AddWithValue("@dToTime", "");
                sqlCommand_childinfo_gq_mod.Parameters.AddWithValue("@vcSupplierPlant", "");
                #endregion
                foreach (DataRow item in dtChildInfo_gq.Rows)
                {
                    if (item["status"].ToString() == "mod" && item["error"].ToString() == "")
                    {
                        #region Value
                        sqlCommand_childinfo_gq_mod.Parameters["@LinId"].Value = item["LinId"].ToString();
                        sqlCommand_childinfo_gq_mod.Parameters["@dFromTime"].Value = item["dFromTime"].ToString();
                        sqlCommand_childinfo_gq_mod.Parameters["@dToTime"].Value = item["dToTime"].ToString();
                        sqlCommand_childinfo_gq_mod.Parameters["@vcSupplierPlant"].Value = item["vcSupplierPlant"].ToString();
                        #endregion
                        sqlCommand_childinfo_gq_mod.ExecuteNonQuery();
                    }
                }
                #endregion

                #region 18.sqlCommand_childinfo_hz_mod
                SqlCommand sqlCommand_childinfo_hz_mod = sqlConnection.CreateCommand();
                sqlCommand_childinfo_hz_mod.Transaction = sqlTransaction;
                sqlCommand_childinfo_hz_mod.CommandType = CommandType.Text;
                StringBuilder strSql_childinfo_hz_mod = new StringBuilder();

                #region SQL and Parameters
                strSql_childinfo_hz_mod.AppendLine("UPDATE [dbo].[TSPMaster_Box]");
                strSql_childinfo_hz_mod.AppendLine("   SET [dFromTime] = case when @dFromTime='' then null else @dFromTime end");
                strSql_childinfo_hz_mod.AppendLine("      ,[dToTime] = case when @dToTime='' then '9999-12-31' else @dToTime end");
                strSql_childinfo_hz_mod.AppendLine("      ,[iPackingQty] = @iPackingQty");
                strSql_childinfo_hz_mod.AppendLine("      ,[vcBoxType] = case when @vcBoxType='' then null else @vcBoxType end");
                strSql_childinfo_hz_mod.AppendLine("      ,[iLength] = case when @iLength='' then null else @iLength end");
                strSql_childinfo_hz_mod.AppendLine("      ,[iWidth] = case when @iWidth='' then null else @iWidth end");
                strSql_childinfo_hz_mod.AppendLine("      ,[iHeight] =case when @iHeight='' then null else @iHeight end");
                strSql_childinfo_hz_mod.AppendLine("      ,[iVolume] =case when @iVolume='' then null else @iVolume end");
                strSql_childinfo_hz_mod.AppendLine("      ,[vcOperatorType] = '1'");
                strSql_childinfo_hz_mod.AppendLine("      ,[vcOperatorID] = '" + strOperId + "'");
                strSql_childinfo_hz_mod.AppendLine("      ,[dOperatorTime] = GETDATE()");
                strSql_childinfo_hz_mod.AppendLine(" WHERE [LinId]=@LinId");
                sqlCommand_childinfo_hz_mod.CommandText = strSql_childinfo_hz_mod.ToString();
                sqlCommand_childinfo_hz_mod.Parameters.AddWithValue("@LinId", "");
                sqlCommand_childinfo_hz_mod.Parameters.AddWithValue("@dFromTime", "");
                sqlCommand_childinfo_hz_mod.Parameters.AddWithValue("@dToTime", "");
                sqlCommand_childinfo_hz_mod.Parameters.AddWithValue("@iPackingQty", "");
                sqlCommand_childinfo_hz_mod.Parameters.AddWithValue("@vcBoxType", "");
                sqlCommand_childinfo_hz_mod.Parameters.AddWithValue("@iLength", "");
                sqlCommand_childinfo_hz_mod.Parameters.AddWithValue("@iWidth", "");
                sqlCommand_childinfo_hz_mod.Parameters.AddWithValue("@iHeight", "");
                sqlCommand_childinfo_hz_mod.Parameters.AddWithValue("@iVolume", "");
                #endregion
                foreach (DataRow item in dtChildInfo_hz.Rows)
                {
                    if (item["status"].ToString() == "mod" && item["error"].ToString() == "")
                    {
                        #region Value
                        sqlCommand_childinfo_hz_mod.Parameters["@LinId"].Value = item["LinId"].ToString();
                        sqlCommand_childinfo_hz_mod.Parameters["@dFromTime"].Value = item["dFromTime"].ToString();
                        sqlCommand_childinfo_hz_mod.Parameters["@dToTime"].Value = item["dToTime"].ToString();
                        sqlCommand_childinfo_hz_mod.Parameters["@iPackingQty"].Value = item["iPackingQty"].ToString();
                        sqlCommand_childinfo_hz_mod.Parameters["@vcBoxType"].Value = item["vcBoxType"].ToString();
                        sqlCommand_childinfo_hz_mod.Parameters["@iLength"].Value = item["iLength"].ToString();
                        sqlCommand_childinfo_hz_mod.Parameters["@iWidth"].Value = item["iWidth"].ToString();
                        sqlCommand_childinfo_hz_mod.Parameters["@iHeight"].Value = item["iHeight"].ToString();
                        sqlCommand_childinfo_hz_mod.Parameters["@iVolume"].Value = item["iVolume"].ToString();
                        #endregion
                        sqlCommand_childinfo_hz_mod.ExecuteNonQuery();
                    }
                }
                #endregion

                #region 19.sqlCommand_childinfo_sr_mod
                SqlCommand sqlCommand_childinfo_sr_mod = sqlConnection.CreateCommand();
                sqlCommand_childinfo_sr_mod.Transaction = sqlTransaction;
                sqlCommand_childinfo_sr_mod.CommandType = CommandType.Text;
                StringBuilder strSql_childinfo_sr_mod = new StringBuilder();

                #region SQL and Parameters
                strSql_childinfo_sr_mod.AppendLine("UPDATE [dbo].[TSPMaster_SufferIn]");
                strSql_childinfo_sr_mod.AppendLine("   SET [dFromTime] = case when @dFromTime='' then null else @dFromTime end");
                strSql_childinfo_sr_mod.AppendLine("      ,[dToTime] = case when @dToTime='' then '9999-12-31' else @dToTime end");
                strSql_childinfo_sr_mod.AppendLine("      ,[vcSufferIn] = @vcSufferIn");
                strSql_childinfo_sr_mod.AppendLine("      ,[vcOperatorType] = '1'");
                strSql_childinfo_sr_mod.AppendLine("      ,[vcOperatorID] = '" + strOperId + "'");
                strSql_childinfo_sr_mod.AppendLine("      ,[dOperatorTime] = GETDATE()");
                strSql_childinfo_sr_mod.AppendLine(" WHERE [LinId]=@LinId");
                sqlCommand_childinfo_sr_mod.CommandText = strSql_childinfo_sr_mod.ToString();
                sqlCommand_childinfo_sr_mod.Parameters.AddWithValue("@LinId", "");
                sqlCommand_childinfo_sr_mod.Parameters.AddWithValue("@dFromTime", "");
                sqlCommand_childinfo_sr_mod.Parameters.AddWithValue("@dToTime", "");
                sqlCommand_childinfo_sr_mod.Parameters.AddWithValue("@vcSufferIn", "");
                #endregion
                foreach (DataRow item in dtChildInfo_sr.Rows)
                {
                    if (item["status"].ToString() == "mod" && item["error"].ToString() == "")
                    {
                        #region Value
                        sqlCommand_childinfo_sr_mod.Parameters["@LinId"].Value = item["LinId"].ToString();
                        sqlCommand_childinfo_sr_mod.Parameters["@dFromTime"].Value = item["dFromTime"].ToString();
                        sqlCommand_childinfo_sr_mod.Parameters["@dToTime"].Value = item["dToTime"].ToString();
                        sqlCommand_childinfo_sr_mod.Parameters["@vcSufferIn"].Value = item["vcSufferIn"].ToString();
                        #endregion
                        sqlCommand_childinfo_sr_mod.ExecuteNonQuery();
                    }
                }
                #endregion

                #region 20.sqlCommand_childinfo_gq_del
                SqlCommand sqlCommand_childinfo_gq_del = sqlConnection.CreateCommand();
                sqlCommand_childinfo_gq_del.Transaction = sqlTransaction;
                sqlCommand_childinfo_gq_del.CommandType = CommandType.Text;
                StringBuilder strSql_childinfo_gq_del = new StringBuilder();

                #region SQL and Parameters
                strSql_childinfo_gq_del.AppendLine("DELETE FROM [dbo].[TSPMaster_SupplierPlant]");
                strSql_childinfo_gq_del.AppendLine(" WHERE [LinId]=@LinId");
                sqlCommand_childinfo_gq_del.CommandText = strSql_childinfo_gq_del.ToString();
                sqlCommand_childinfo_gq_del.Parameters.AddWithValue("@LinId", "");
                #endregion
                foreach (DataRow item in dtChildInfo_gq.Rows)
                {
                    if (item["status"].ToString() == "del" && item["error"].ToString() == "")
                    {
                        #region Value
                        sqlCommand_childinfo_gq_del.Parameters["@LinId"].Value = item["LinId"].ToString();
                        #endregion
                        sqlCommand_childinfo_gq_del.ExecuteNonQuery();
                    }
                }
                #endregion

                #region 21.sqlCommand_childinfo_hz_del
                SqlCommand sqlCommand_childinfo_hz_del = sqlConnection.CreateCommand();
                sqlCommand_childinfo_hz_del.Transaction = sqlTransaction;
                sqlCommand_childinfo_hz_del.CommandType = CommandType.Text;
                StringBuilder strSql_childinfo_hz_del = new StringBuilder();

                #region SQL and Parameters
                strSql_childinfo_hz_del.AppendLine("DELETE FROM [dbo].[TSPMaster_Box]");
                strSql_childinfo_hz_del.AppendLine(" WHERE [LinId]=@LinId");
                sqlCommand_childinfo_hz_del.CommandText = strSql_childinfo_hz_del.ToString();
                sqlCommand_childinfo_hz_del.Parameters.AddWithValue("@LinId", "");
                #endregion
                foreach (DataRow item in dtChildInfo_hz.Rows)
                {
                    if (item["status"].ToString() == "del" && item["error"].ToString() == "")
                    {
                        #region Value
                        sqlCommand_childinfo_hz_del.Parameters["@LinId"].Value = item["LinId"].ToString();
                        #endregion
                        sqlCommand_childinfo_hz_del.ExecuteNonQuery();
                    }
                }
                #endregion

                #region 22.sqlCommand_childinfo_sr_del
                SqlCommand sqlCommand_childinfo_sr_del = sqlConnection.CreateCommand();
                sqlCommand_childinfo_sr_del.Transaction = sqlTransaction;
                sqlCommand_childinfo_sr_del.CommandType = CommandType.Text;
                StringBuilder strSql_childinfo_sr_del = new StringBuilder();

                #region SQL and Parameters
                strSql_childinfo_sr_del.AppendLine("DELETE FROM [dbo].[TSPMaster_SufferIn]");
                strSql_childinfo_sr_del.AppendLine(" WHERE [LinId]=@LinId");
                sqlCommand_childinfo_sr_del.CommandText = strSql_childinfo_sr_del.ToString();
                sqlCommand_childinfo_sr_del.Parameters.AddWithValue("@LinId", "");
                #endregion
                foreach (DataRow item in dtChildInfo_sr.Rows)
                {
                    if (item["status"].ToString() == "del" && item["error"].ToString() == "")
                    {
                        #region Value
                        sqlCommand_childinfo_sr_del.Parameters["@LinId"].Value = item["LinId"].ToString();
                        #endregion
                        sqlCommand_childinfo_sr_del.ExecuteNonQuery();
                    }
                }
                #endregion

                //提交事务
                sqlTransaction.Commit();
                sqlConnection.Close();
                #endregion

            }
            catch (Exception ex)
            {
                DataRow dataRow = dtMessage.NewRow();
                dataRow["vcMessage"] = "数据写入数据库失败！";
                dtMessage.Rows.Add(dataRow);
                //回滚事务
                if (sqlTransaction != null && sqlConnection != null)
                {
                    sqlTransaction.Rollback();
                    sqlConnection.Close();
                }
            }
        }
        public DataTable searchOperHistory(string strFromTime, string strToFrom, string strOperId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine(" select * from TSPMaster_OperHistory where Convert(varchar(10),dOperatorTime,111)>='" + strFromTime + "' and Convert(varchar(10),dOperatorTime,111)<='" + strToFrom + "' and vcOperatorID='" + strOperId + "' order by dOperatorTime");
            return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
        }
        public DataTable getEditLoadInfo(string strEditType, string strPackingPlant, string strPartId, string strReceiver, string strSupplierId, string strSupplierPlant)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                if (strEditType == "SupplierPlantEdit")
                {
                    stringBuilder.AppendLine("SELECT[LinId],[dFromTime],[dToTime],[vcSupplierPlant],'0' as bAddFlag,'' as vcBgColor FROM [TSPMaster_SupplierPlant] ");
                    stringBuilder.AppendLine("WHERE [vcOperatorType]='1'   ");
                    if (strPackingPlant != "")
                    {
                        stringBuilder.AppendLine("and [vcPackingPlant]='" + strPackingPlant + "'");
                    }
                    if (strPartId != "")
                    {
                        stringBuilder.AppendLine("and [vcPartId]='" + strPartId + "'");
                    }
                    if (strReceiver != "")
                    {
                        stringBuilder.AppendLine("and [vcReceiver]='" + strReceiver + "'");
                    }
                    if (strSupplierId != "")
                    {
                        stringBuilder.AppendLine("and [vcSupplierId]='" + strSupplierId + "'");
                    }
                    stringBuilder.AppendLine("ORDER BY [dFromTime]");
                }
                if (strEditType == "PackingQtyEdit")
                {
                    stringBuilder.AppendLine("SELECT [LinId],[dFromTime],[dToTime],[iPackingQty],[vcBoxType],[iLength],[iWidth],[iHeight],[iVolume],'0' as bAddFlag,'' as vcBgColor FROM [TSPMaster_Box]");
                    stringBuilder.AppendLine("WHERE [vcOperatorType]='1' ");
                    if (strPackingPlant != "")
                    {
                        stringBuilder.AppendLine("and [vcPackingPlant]='" + strPackingPlant + "'");
                    }
                    if (strPartId != "")
                    {
                        stringBuilder.AppendLine("and [vcPartId]='" + strPartId + "'");
                    }
                    if (strReceiver != "")
                    {
                        stringBuilder.AppendLine("and [vcReceiver]='" + strReceiver + "'");
                    }
                    if (strSupplierId != "")
                    {
                        stringBuilder.AppendLine("and [vcSupplierId]='" + strSupplierId + "'");
                    }
                    stringBuilder.AppendLine("ORDER BY [dFromTime]");

                }
                if (strEditType == "SufferInEdit")
                {
                    stringBuilder.AppendLine("SELECT [LinId],[dFromTime],[dToTime],[vcSufferIn],'0' as bAddFlag,'' as vcBgColor FROM [TSPMaster_SufferIn]");
                    stringBuilder.AppendLine("WHERE [vcOperatorType]='1' ");
                    if (strPackingPlant != "")
                    {
                        stringBuilder.AppendLine("and [vcPackingPlant]='" + strPackingPlant + "'");
                    }
                    if (strPartId != "")
                    {
                        stringBuilder.AppendLine("and [vcPartId]='" + strPartId + "'");
                    }
                    if (strReceiver != "")
                    {
                        stringBuilder.AppendLine("and [vcReceiver]='" + strReceiver + "'");
                    }
                    if (strSupplierId != "")
                    {
                        stringBuilder.AppendLine("and [vcSupplierId]='" + strSupplierId + "'");
                    }
                    stringBuilder.AppendLine("ORDER BY [dFromTime]");
                }
                if (strEditType == "OrderPlantEdit")
                {
                    stringBuilder.AppendLine("select '' as [LinId],a.[dFromTime],a.[dToTime],a.[vcOrderPlant],b.vcName as vcOrderPlant_name,'0' as bAddFlag,'' as vcBgColor from (");
                    stringBuilder.AppendLine("select vcValue1 as [vcSupplierId],vcValue2 as vcSupplierPlant,vcValue3 as [dFromTime],vcValue4 as [dToTime],vcValue5 as vcOrderPlant from TOutCode where vcCodeId='C010' and vcIsColum='0')a");
                    stringBuilder.AppendLine("left join");
                    stringBuilder.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C000')b");
                    stringBuilder.AppendLine("on a.vcOrderPlant=b.vcValue");
                    stringBuilder.AppendLine("where 1=1");
                    if (strSupplierId != "")
                    {
                        stringBuilder.AppendLine("and [vcSupplierId]='" + strSupplierId + "'");
                    }
                    if (strSupplierPlant != "")
                    {
                        stringBuilder.AppendLine("and [vcSupplierPlant]='" + strSupplierPlant + "'");
                    }
                    stringBuilder.AppendLine("ORDER BY [dFromTime]");
                }
                return excute.ExcuteSqlWithSelectToDT(stringBuilder.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getSubInfo(string strEditType, string strPackingPlant, string strPartId, string strReceiver, string strSupplierId, string strSupplierPlant)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                if (strEditType == "SupplierPlantEdit")
                {
                    stringBuilder.AppendLine("SELECT[LinId],vcPackingPlant,vcPartId,vcReceiver,vcSupplierId,[dFromTime],[dToTime],[vcSupplierPlant],'0' as bAddFlag,'' as vcBgColor FROM [TSPMaster_SupplierPlant] ");
                    stringBuilder.AppendLine("WHERE [vcOperatorType]='1'   ");
                    if (strPackingPlant != "")
                    {
                        stringBuilder.AppendLine("and [vcPackingPlant]='" + strPackingPlant + "'");
                    }
                    if (strPartId != "")
                    {
                        stringBuilder.AppendLine("and [vcPartId]='" + strPartId + "'");
                    }
                    if (strReceiver != "")
                    {
                        stringBuilder.AppendLine("and [vcReceiver]='" + strReceiver + "'");
                    }
                    if (strSupplierId != "")
                    {
                        stringBuilder.AppendLine("and [vcSupplierId]='" + strSupplierId + "'");
                    }
                    stringBuilder.AppendLine("ORDER BY [dFromTime]");
                }
                if (strEditType == "PackingQtyEdit")
                {
                    stringBuilder.AppendLine("SELECT [LinId],vcPackingPlant,vcPartId,vcReceiver,vcSupplierId,[dFromTime],[dToTime],[iPackingQty],[vcBoxType],[iLength],[iWidth],[iHeight],[iVolume],'0' as bAddFlag,'' as vcBgColor FROM [TSPMaster_Box]");
                    stringBuilder.AppendLine("WHERE [vcOperatorType]='1' ");
                    if (strPackingPlant != "")
                    {
                        stringBuilder.AppendLine("and [vcPackingPlant]='" + strPackingPlant + "'");
                    }
                    if (strPartId != "")
                    {
                        stringBuilder.AppendLine("and [vcPartId]='" + strPartId + "'");
                    }
                    if (strReceiver != "")
                    {
                        stringBuilder.AppendLine("and [vcReceiver]='" + strReceiver + "'");
                    }
                    if (strSupplierId != "")
                    {
                        stringBuilder.AppendLine("and [vcSupplierId]='" + strSupplierId + "'");
                    }
                    stringBuilder.AppendLine("ORDER BY [dFromTime]");

                }
                if (strEditType == "SufferInEdit")
                {
                    stringBuilder.AppendLine("SELECT [LinId],vcPackingPlant,vcPartId,vcReceiver,vcSupplierId,[dFromTime],[dToTime],[vcSufferIn],'0' as bAddFlag,'' as vcBgColor FROM [TSPMaster_SufferIn]");
                    stringBuilder.AppendLine("WHERE [vcOperatorType]='1' ");
                    if (strPackingPlant != "")
                    {
                        stringBuilder.AppendLine("and [vcPackingPlant]='" + strPackingPlant + "'");
                    }
                    if (strPartId != "")
                    {
                        stringBuilder.AppendLine("and [vcPartId]='" + strPartId + "'");
                    }
                    if (strReceiver != "")
                    {
                        stringBuilder.AppendLine("and [vcReceiver]='" + strReceiver + "'");
                    }
                    if (strSupplierId != "")
                    {
                        stringBuilder.AppendLine("and [vcSupplierId]='" + strSupplierId + "'");
                    }
                    stringBuilder.AppendLine("ORDER BY [dFromTime]");
                }
                if (strEditType == "OrderPlantEdit")
                {
                    stringBuilder.AppendLine("select '' as [LinId],a.vcSupplierId,a.vcSupplierPlant,a.[dFromTime],a.[dToTime],a.[vcOrderPlant],b.vcName as vcOrderPlant_name,'0' as bAddFlag,'' as vcBgColor from (");
                    stringBuilder.AppendLine("select vcValue1 as [vcSupplierId],vcValue2 as vcSupplierPlant,vcValue3 as [dFromTime],vcValue4 as [dToTime],vcValue5 as vcOrderPlant from TOutCode where vcCodeId='C010' and vcIsColum='0')a");
                    stringBuilder.AppendLine("left join");
                    stringBuilder.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C000')b");
                    stringBuilder.AppendLine("on a.vcOrderPlant=b.vcValue");
                    stringBuilder.AppendLine("where 1=1");
                    if (strSupplierId != "")
                    {
                        stringBuilder.AppendLine("and [vcSupplierId]='" + strSupplierId + "'");
                    }
                    if (strSupplierPlant != "")
                    {
                        stringBuilder.AppendLine("and [vcSupplierPlant]='" + strSupplierPlant + "'");
                    }
                    stringBuilder.AppendLine("ORDER BY [dFromTime]");
                }
                return excute.ExcuteSqlWithSelectToDT(stringBuilder.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getOrderPlantInfo(string strSupplierId, string strSupplierPlant, string strToTime)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine("select vcValue1 as [vcSupplierId],vcValue2 as vcSupplierPlant,vcValue3 as [dFromTime],vcValue4 as [dToTime],vcValue5 as vcOrderPlant from TOutCode where vcCodeId='C010' and vcIsColum='0'");
            strSql.AppendLine("and vcValue1='" + strSupplierId + "' and vcValue2='" + strSupplierPlant + "' and vcValue3<=CONVERT(VARCHAR(10),'" + strToTime + "',23) AND vcValue4>=CONVERT(VARCHAR(10),'" + strToTime + "',23)");
            return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
        }
        public string setNullValue(Object obj, string strModle, string strDefault)
        {
            if (obj == null)
                return strDefault.ToUpper();
            else if (obj.ToString().Trim() == "")
                return strDefault.ToUpper();
            else
            {
                if (strModle == "date")
                    return Convert.ToDateTime(obj.ToString().Trim().ToUpper()).ToString("yyyy-MM-dd");
                else
                    return obj.ToString().ToUpper();
            }
        }
        public DataTable gettaskNum()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine("SELECT distinct T1.vcPackingPlant,T1.vcPartId,T1.vcReceiver,T1.vcSupplierId,t1.vcInOut  FROM  ");
            strSql.AppendLine("(SELECT * FROM [TSPMaster] WHERE isnull(vcDelete, '') <> '1' ");
            strSql.AppendLine("AND (([dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))  ");
            strSql.AppendLine("OR ([dFromTime]>=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))) ");
            strSql.AppendLine("  )T1 ");
            strSql.AppendLine("LEFT JOIN ");
            strSql.AppendLine("(SELECT ");
            strSql.AppendLine("ROW_NUMBER() OVER(partition by vcPackingPlant,vcPartId,vcReceiver,vcSupplierId order by dFromTime) [rank],");
            strSql.AppendLine("LinId,vcPackingPlant,vcPartId,vcReceiver,vcSupplierId,dFromTime,dToTime,vcSupplierPlant ");
            strSql.AppendLine("FROM [TSPMaster_SupplierPlant] ");
            strSql.AppendLine("WHERE [vcOperatorType]='1' AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23)");
            strSql.AppendLine(")T2");
            strSql.AppendLine("ON T2.rank='1' AND T1.[vcPackingPlant]=T2.[vcPackingPlant] AND T1.[vcPartId]=T2.[vcPartId] AND T1.[vcReceiver]=T2.[vcReceiver] AND T1.[vcSupplierId]=T2.[vcSupplierId] ");
            strSql.AppendLine("LEFT JOIN ");
            strSql.AppendLine("(SELECT ");
            strSql.AppendLine("ROW_NUMBER() OVER(partition by vcPackingPlant,vcPartId,vcReceiver,vcSupplierId order by dFromTime) [rank],");
            strSql.AppendLine("LinId,vcPackingPlant,vcPartId,vcReceiver,vcSupplierId,dFromTime,dToTime,iPackingQty,vcBoxType,iLength,iWidth,iHeight,iVolume ");
            strSql.AppendLine("FROM [TSPMaster_Box] ");
            strSql.AppendLine("WHERE [vcOperatorType]='1' AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23)");
            strSql.AppendLine(")T3");
            strSql.AppendLine("ON T3.rank='1' AND T1.[vcPackingPlant]=T3.[vcPackingPlant] AND T1.[vcPartId]=T3.[vcPartId] AND T1.[vcReceiver]=T3.[vcReceiver] AND T1.[vcSupplierId]=T3.[vcSupplierId] ");
            strSql.AppendLine("LEFT JOIN ");
            strSql.AppendLine("(SELECT ");
            strSql.AppendLine("ROW_NUMBER() OVER(partition by vcPackingPlant,vcPartId,vcReceiver,vcSupplierId order by dFromTime) [rank],");
            strSql.AppendLine("LinId,vcPackingPlant,vcPartId,vcReceiver,vcSupplierId,dFromTime,dToTime,vcSufferIn");
            strSql.AppendLine("FROM [TSPMaster_SufferIn] ");
            strSql.AppendLine("WHERE [vcOperatorType]='1' AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23)");
            strSql.AppendLine(")T4");
            strSql.AppendLine("ON T4.rank='1' AND T1.[vcPackingPlant]=T4.[vcPackingPlant] AND T1.[vcPartId]=T4.[vcPartId] AND T1.[vcReceiver]=T4.[vcReceiver] AND T1.[vcSupplierId]=T4.[vcSupplierId] ");
            strSql.AppendLine("LEFT JOIN ");
            strSql.AppendLine("(SELECT ");
            strSql.AppendLine("ROW_NUMBER() OVER(partition by vcSupplierId,vcSupplierPlant order by dFromTime) [rank],");
            strSql.AppendLine("vcSupplierId,vcSupplierPlant,dFromTime,dToTime,vcOrderPlant");
            strSql.AppendLine("FROM (select vcValue1 as [vcSupplierId],vcValue2 as vcSupplierPlant,vcValue3 as [dFromTime],vcValue4 as [dToTime],vcValue5 as vcOrderPlant from TOutCode where vcCodeId='C010' and vcIsColum='0'");
            strSql.AppendLine("and vcValue3<=CONVERT(VARCHAR(10),GETDATE(),23) AND vcValue4>=CONVERT(VARCHAR(10),GETDATE(),23))a ");
            strSql.AppendLine("WHERE [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))T5");
            strSql.AppendLine("ON t5.[rank]='1' AND T1.[vcSupplierId]=T5.[vcSupplierId] AND T2.vcSupplierPlant=T5.vcSupplierPlant ");
            strSql.AppendLine("WHERE (ISNULL(T2.vcSupplierPlant,'')='' OR ISNULL(T3.iPackingQty,0)=0 OR ISNULL(T4.vcSufferIn,'')='' OR ISNULL(T5.vcOrderPlant,'')='') ");
            DataTable dataTable = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            return dataTable;
        }
        public DataSet getSyncInfo()
        {
            StringBuilder strSql = new StringBuilder();
            #region 初始化植入从临时表原单位到正式表
            strSql.AppendLine("INSERT INTO [dbo].[TSPMaster]");
            strSql.AppendLine("           ([dSyncTime]");
            strSql.AppendLine("           ,[vcChanges]");
            strSql.AppendLine("           ,[vcPackingPlant]");
            strSql.AppendLine("           ,[vcPartId]");
            strSql.AppendLine("           ,[vcPartENName]");
            strSql.AppendLine("           ,[vcCarfamilyCode]");
            strSql.AppendLine("           ,[vcCarModel]");
            strSql.AppendLine("           ,[vcReceiver]");
            strSql.AppendLine("           ,[dFromTime]");
            strSql.AppendLine("           ,[dToTime]");
            strSql.AppendLine("           ,[vcPartId_Replace]");
            strSql.AppendLine("           ,[vcInOut]");
            strSql.AppendLine("           ,[vcOESP]");
            strSql.AppendLine("           ,[vcHaoJiu]");
            strSql.AppendLine("           ,[vcOldProduction]");
            strSql.AppendLine("           ,[dOldStartTime]");
            strSql.AppendLine("           ,[dDebugTime]");
            strSql.AppendLine("           ,[vcSupplierId]");
            strSql.AppendLine("           ,[dSupplierFromTime]");
            strSql.AppendLine("           ,[dSupplierToTime]");
            strSql.AppendLine("           ,[vcSupplierName]");
            strSql.AppendLine("           ,[vcSupplierPlace]");
            strSql.AppendLine("           ,[vcInteriorProject]");
            strSql.AppendLine("           ,[vcPassProject]");
            strSql.AppendLine("           ,[vcFrontProject]");
            strSql.AppendLine("           ,[dFrontProjectTime]");
            strSql.AppendLine("           ,[dShipmentTime]");
            strSql.AppendLine("           ,[vcBillType]");
            strSql.AppendLine("           ,[vcOrderingMethod]");
            strSql.AppendLine("           ,[vcMandOrder]");
            strSql.AppendLine("           ,[vcPartImage]");
            strSql.AppendLine("           ,[vcRemark1]");
            strSql.AppendLine("           ,[vcRemark2]");
            strSql.AppendLine("           ,[vcSupplierPacking]");
            strSql.AppendLine("           ,[vcDelete]");
            strSql.AppendLine("           ,[vcOperatorID]");
            strSql.AppendLine("           ,[dOperatorTime]");
            strSql.AppendLine("           ,[dSyncToSPTime]");
            strSql.AppendLine("           ,[vcPartNameCn]");
            strSql.AppendLine("		   ,[dInSPMasterTime])");
            strSql.AppendLine("SELECT [dSyncTime]");
            strSql.AppendLine("      ,[vcChanges]");
            strSql.AppendLine("      ,[vcPackingPlant]");
            strSql.AppendLine("      ,[vcPartId]");
            strSql.AppendLine("      ,[vcPartENName]");
            strSql.AppendLine("      ,[vcCarfamilyCode]");
            strSql.AppendLine("      ,[vcCarModel]");
            strSql.AppendLine("      ,[vcReceiver]");
            strSql.AppendLine("      ,[dFromTime]");
            strSql.AppendLine("      ,[dToTime]");
            strSql.AppendLine("      ,[vcPartId_Replace]");
            strSql.AppendLine("      ,[vcInOut]");
            strSql.AppendLine("      ,[vcOESP]");
            strSql.AppendLine("      ,[vcHaoJiu]");
            strSql.AppendLine("      ,[vcOldProduction]");
            strSql.AppendLine("      ,[dOldStartTime]");
            strSql.AppendLine("      ,[dDebugTime]");
            strSql.AppendLine("      ,[vcSupplierId]");
            strSql.AppendLine("      ,[dSupplierFromTime]");
            strSql.AppendLine("      ,[dSupplierToTime]");
            strSql.AppendLine("      ,[vcSupplierName]");
            strSql.AppendLine("      ,[vcSupplierPlace]");
            strSql.AppendLine("      ,[vcInteriorProject]");
            strSql.AppendLine("      ,[vcPassProject]");
            strSql.AppendLine("      ,[vcFrontProject]");
            strSql.AppendLine("      ,[dFrontProjectTime]");
            strSql.AppendLine("      ,[dShipmentTime]");
            strSql.AppendLine("      ,[vcBillType]");
            strSql.AppendLine("      ,[vcOrderingMethod]");
            strSql.AppendLine("      ,[vcMandOrder]");
            strSql.AppendLine("      ,[vcPartImage]");
            strSql.AppendLine("      ,[vcRemark1]");
            strSql.AppendLine("      ,[vcRemark2]");
            strSql.AppendLine("      ,[vcSupplierPacking]");
            strSql.AppendLine("      ,[vcDelete]");
            strSql.AppendLine("      ,[vcOperatorID]");
            strSql.AppendLine("      ,[dOperatorTime]");
            strSql.AppendLine("      ,[dSyncToSPTime]");
            strSql.AppendLine("      ,[vcPartNameCn]");
            strSql.AppendLine("	  ,GETDATE()");
            strSql.AppendLine("  FROM [dbo].[TSPMaster_temp]");
            strSql.AppendLine("DELETE  FROM [dbo].[TSPMaster_temp]");
            #endregion

            strSql.AppendLine("select convert(varchar(10),dSyncTime,111) as dSyncTime,");
            strSql.AppendLine("		count(*) as iCount,");
            strSql.AppendLine("		'同步原单位情报'+cast(count(*) as varchar(10))+'条' as vcSyncMessage ,");
            strSql.AppendLine("		'*' as vcChanges,");
            strSql.AppendLine("		'0' as bModFlag,");
            strSql.AppendLine("		'0' as bAddFlag,");
            strSql.AppendLine("		'1' as bSelectFlag,");
            strSql.AppendLine("		'' as vcBgColor,");
            strSql.AppendLine("		convert(varchar(10),dSyncTime,111) as children");
            strSql.AppendLine("from TSPMaster where dSyncToSPTime is not null");
            strSql.AppendLine("group by convert(varchar(10),dSyncTime,111)");
            strSql.AppendLine("order by dSyncTime");
            strSql.AppendLine("select convert(varchar(10),a.dSyncTime,111) as dSyncTime,");
            strSql.AppendLine("		count(*) as iCount,");
            strSql.AppendLine("		'【'+b.vcName+'】原单位情报'+ cast(count(*) as varchar(10))+'条' as vcSyncMessage ,");
            strSql.AppendLine("		b.vcValue as vcChanges,");
            strSql.AppendLine("		'0' as bModFlag,");
            strSql.AppendLine("		'0' as bAddFlag,");
            strSql.AppendLine("		'0' as bSelectFlag,");
            strSql.AppendLine("		'' as vcBgColor,");
            strSql.AppendLine("		convert(varchar(10),a.dSyncTime,111) as children from");
            strSql.AppendLine("(select convert(varchar(10),dSyncTime,111) as dSyncTime,vcChanges from TSPMaster where dSyncToSPTime is not null)a");
            strSql.AppendLine("LEFT JOIN");
            strSql.AppendLine("(SELECT * FROM TCode WHERE vcCodeId='C002')b");
            strSql.AppendLine("ON a.vcChanges=b.vcValue");
            strSql.AppendLine("group by convert(varchar(10),a.dSyncTime,111),b.vcName,b.vcValue");
            strSql.AppendLine("order by a.dSyncTime,b.vcValue");
            DataSet dataSet = excute.ExcuteSqlWithSelectToDS(strSql.ToString());
            return dataSet;
        }
        public void setSyncInfo(DataTable dtImport, string strOperId, ref DataTable dtMessage)
        {
            SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();

            sqlConnection.Open();
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            try
            {
                SqlCommand sqlCommand_modinfo_sy_mod = sqlConnection.CreateCommand();
                sqlCommand_modinfo_sy_mod.Transaction = sqlTransaction;
                sqlCommand_modinfo_sy_mod.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo_sy_mod = new StringBuilder();

                #region SQL and Parameters
                strSql_modinfo_sy_mod.AppendLine("if(@vcChanges='*')");
                strSql_modinfo_sy_mod.AppendLine("begin ");
                strSql_modinfo_sy_mod.AppendLine("update TSPMaster set dSyncToSPTime=null where convert(varchar(10),dSyncTime,111)=@dSyncTime");
                strSql_modinfo_sy_mod.AppendLine("end");
                strSql_modinfo_sy_mod.AppendLine("else");
                strSql_modinfo_sy_mod.AppendLine("begin");
                strSql_modinfo_sy_mod.AppendLine("update TSPMaster set dSyncToSPTime=null where convert(varchar(10),dSyncTime,111)=@dSyncTime and vcChanges=@vcChanges");
                strSql_modinfo_sy_mod.AppendLine("end");
                sqlCommand_modinfo_sy_mod.CommandText = strSql_modinfo_sy_mod.ToString();
                sqlCommand_modinfo_sy_mod.Parameters.AddWithValue("@vcChanges", "");
                sqlCommand_modinfo_sy_mod.Parameters.AddWithValue("@dSyncTime", "");
                #endregion
                foreach (DataRow item in dtImport.Rows)
                {
                    #region Value
                    sqlCommand_modinfo_sy_mod.Parameters["@vcChanges"].Value = item["vcChanges"].ToString();
                    sqlCommand_modinfo_sy_mod.Parameters["@dSyncTime"].Value = item["dSyncTime"].ToString();
                    #endregion
                    sqlCommand_modinfo_sy_mod.ExecuteNonQuery();
                }
                //提交事务
                sqlTransaction.Commit();
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                DataRow dataRow = dtMessage.NewRow();
                dataRow["vcMessage"] = "数据写入数据库失败！";
                dtMessage.Rows.Add(dataRow);
                //回滚事务
                if (sqlTransaction != null && sqlConnection != null)
                {
                    sqlTransaction.Rollback();
                    sqlConnection.Close();
                }
            }
        }

        public string getPrinterName(string strPageId, string strOperId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine("select vcPrintName from TPrinterName where vcOperator='" + strOperId + "' and vcPageId='" + strPageId + "' order by LinId desc");
            DataTable dataTable = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            if (dataTable.Rows.Count != 0)
                return dataTable.Rows[0]["vcPrintName"].ToString();
            else
                return "";
        }
    }
}
