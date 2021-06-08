using System;
using System.Collections.Generic;
using Common;
using System.Data;
using System.Text;

namespace DataAccess
{
    public class FS0401_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        public DataTable searchApi(string PartId, string TimeFrom, string TimeTo, string carType, string InOut, string DHFlag)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("SELECT  T1.LinId,");
                strSql.AppendLine("		Convert(varchar(10),T1.dSyncTime,111) as dSyncTime,");
                strSql.AppendLine("		T1.vcChanges as vcChanges,T6.vcName as vcChanges_name,");
                strSql.AppendLine("		T1.vcPackingPlant,T7.vcName as vcPackingPlant_name,");
                strSql.AppendLine("		T1.vcPartId,");
                strSql.AppendLine("		CASE SUBSTRING(T1.vcPartId,11,2) WHEN '00' THEN SUBSTRING(T1.vcPartId,1,10) ELSE T1.vcPartId END AS vcPartIdDC,");
                strSql.AppendLine("		T1.vcPartENName,");
                strSql.AppendLine("		T1.vcCarfamilyCode,");
                strSql.AppendLine("		T1.vcReceiver,");
                strSql.AppendLine("		Convert(varchar(10),T1.dFromTime,111) as dFromTime,");
                strSql.AppendLine("		Convert(varchar(10),T1.dToTime,111) as dToTime,");
                strSql.AppendLine("		T1.vcPartId_Replace,");
                strSql.AppendLine("		CASE SUBSTRING(T1.vcPartId_Replace,11,2) WHEN '00' THEN SUBSTRING(T1.vcPartId_Replace,1,10) ELSE T1.vcPartId_Replace END AS vcPartId_ReplaceDC,");
                strSql.AppendLine("		T1.vcInOut,T8.vcName as vcInOut_name,");
                strSql.AppendLine("		T1.vcOESP,T9.vcName as vcOESP_name,");
                strSql.AppendLine("		T1.vcHaoJiu,T10.vcName as vcHaoJiu_name,");
                strSql.AppendLine("		T1.vcOldProduction,T1.vcOldProduction as vcOldProduction_name,");
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
                strSql.AppendLine("		T3.iPackingQty AS iPackingQty,");
                //strSql.AppendLine("		CAST(T3.iPackingQty AS varchar(10)) AS iPackingQty,");
                strSql.AppendLine("		T1.vcSupplierPlace as vcSupplierPlace,");
                strSql.AppendLine("		T3.vcBoxType,T3.iLength,T3.iWidth,T3.iHeight,T3.iVolume,");
                strSql.AppendLine("		T3.LinId as BoxLinId_ed,");
                strSql.AppendLine("		CAST(T3.iPackingQty AS varchar(10)) AS BoxPackingQty_ed,");
                strSql.AppendLine("		Convert(varchar(10),T3.dFromTime,111) as BoxFromTime_ed,");
                strSql.AppendLine("		Convert(varchar(10),T3.dToTime,111) as BoxToTime_ed,");
                strSql.AppendLine("		T3.vcBoxType as BoxType_ed,T3.iLength as BoxLength_ed,T3.iWidth as BoxWidth_ed,T3.iHeight as BoxHeight_ed,T3.iVolume as BoxVolume_ed,");
                strSql.AppendLine("		T4.vcSufferIn,");
                strSql.AppendLine("		T4.LinId as SufferInLinId_ed,");
                strSql.AppendLine("		T4.vcSufferIn as SufferIn_ed,");
                strSql.AppendLine("		Convert(varchar(10),T4.dFromTime,111) as SufferInFromTime_ed,");
                strSql.AppendLine("		Convert(varchar(10),T4.dToTime,111) as SufferInToTime_ed,");
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
                if (!string.IsNullOrWhiteSpace(DHFlag))
                {
                    strSql.AppendLine(" AND vcOrderingMethod = '" + DHFlag + "'");
                }
                if (PartId != "")
                {
                    strSql.AppendLine("    AND [vcPartId] like '" + PartId + "%'");
                }
                if (carType != "")
                {
                    strSql.AppendLine("    AND [vcCarfamilyCode] like '" + carType + "%'");
                }
                if (InOut != "")
                {
                    strSql.AppendLine("    AND [vcInOut]='" + InOut + "'");
                }
                if (TimeFrom != "")
                {
                    //strSql.AppendLine("    AND [dFromTime]<='" + TimeFrom + "'");
                    strSql.AppendLine("    AND [dFromTime]>='" + TimeFrom + "'");

                }
                if (TimeTo != "")
                {
                    //strSql.AppendLine("    AND [dToTime]>='" + TimeTo + "'");
                    strSql.AppendLine("    AND [dToTime]<='" + TimeTo + "'");

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
                //strSql.AppendLine("WHERE [vcOperatorType]='1' AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23)");
                strSql.AppendLine("WHERE  [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23)");
                strSql.AppendLine(")T3");
                //strSql.AppendLine("ON T3.rank='1' AND T1.[vcPackingPlant]=T3.[vcPackingPlant] AND T1.[vcPartId]=T3.[vcPartId] AND T1.[vcReceiver]=T3.[vcReceiver] AND T1.[vcSupplierId]=T3.[vcSupplierId]");
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
                strSql.AppendLine(
                    " AND ISNULL(T2.vcSupplierPlant,'')<>'' AND ISNULL(T3.iPackingQty,0)<>0 AND ISNULL(T4.vcSufferIn,'')<>'' AND ISNULL(T5.vcOrderPlant,'')<>''");
                strSql.AppendLine("ORDER BY T1.LinId ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable getCarType()
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine("SELECT vcCarfamilyCode AS vcName,a.vcCarfamilyCode AS vcValue FROM");
                sbr.AppendLine("(");
                sbr.AppendLine("SELECT DISTINCT vcCarfamilyCode FROM dbo.TSPMaster ");
                sbr.AppendLine(") a");

                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}