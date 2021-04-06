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
    public class FS0604_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

      /// <summary>
      /// 获取箱种目的
      /// </summary>
      /// <returns></returns>
        public DataTable GetBoxType()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                //strSql.AppendLine("  select vcBoxType as vcValue,vcBoxType as vcName from(select distinct isnull(left(vcBoxType,2),'无') as vcBoxType from THeZiManage) a  ");

                strSql.AppendLine("  select vcBoxType as vcValue,vcBoxType as vcName from(        ");
                strSql.AppendLine("                 select distinct isnull(left(vcBoxType,2),'无') as vcBoxType from (        ");
                strSql.AppendLine("                   select            ");
                strSql.AppendLine("                    n.[iAutoId], n.vcPackingPlant,n.vcReceiver, n.[dSynchronizationDate], o.vcName as [vcState], n.[vcPartNo],              ");
                strSql.AppendLine("                      	n.[dUseStartDate],n.[dUserEndDate], n.[vcPartName],                 ");
                strSql.AppendLine("                      	n.[vcCarType],p.vcName as [vcOEOrSP], n.[vcSupplier_id], n.[vcWorkArea], n.[dExpectDeliveryDate], n.[vcExpectIntake],                ");
                strSql.AppendLine("                      	n.[vcIntake], n.[vcBoxMaxIntake], n.[vcBoxType], n.[vcLength], n.[vcWide], n.[vcHeight], n.[vcEmptyWeight],                 ");
                strSql.AppendLine("                      	n.[vcUnitNetWeight], n.[dSendDate], n.[dReplyDate], n.[dAdmitDate], n.[dWeaveDate], n.[vcMemo], n.vcImageRoutes,                 ");
                strSql.AppendLine("                      	n.[vcInserter], n.[vcInserterDate],n.[vcFactoryOperatorID], n.[dFactoryOperatorTime],                ");
                strSql.AppendLine("                      	n.[vcOperatorID], n.[dOperatorTime],'0' as vcModFlag,'0' as vcAddFlag           ");
                strSql.AppendLine("                    from (              ");
                strSql.AppendLine("                       select null as iAutoId,  a.vcPackingPlant,a.vcReceiver, a.dSyncTime as dSynchronizationDate, '0' as vcState,a.vcPartId as vcPartNo,              ");
                strSql.AppendLine("                       a.dFromTime as dUseStartDate,a.dToTime as dUserEndDate,a.vcPartENName as vcPartName,a.vcCarModel as vcCarType,              ");
                strSql.AppendLine("                       a.vcOESP as vcOEOrSP,a.vcSupplierId as vcSupplier_id,c.vcSupplierPlant as vcWorkArea,              ");
                strSql.AppendLine("                       '' as dExpectDeliveryDate,'' as vcExpectIntake,'' as [vcIntake],'' as [vcBoxMaxIntake],'' as [vcBoxType],'' as [vcLength],'' as [vcWide],              ");
                strSql.AppendLine("                       '' as [vcHeight],'' as [vcEmptyWeight],'' as [vcUnitNetWeight],null as [dSendDate],null as [dReplyDate],null as [dAdmitDate],null as [dWeaveDate],             ");
                strSql.AppendLine("                       '' as [vcMemo],null as  [vcImageRoutes],'' as [vcInserter],null as [vcInserterDate],'' as [vcFactoryOperatorID],null as [dFactoryOperatorTime],               ");
                strSql.AppendLine("                      null as [vcOperatorID], null as [dOperatorTime]           ");
                strSql.AppendLine("                         from  (select * from [dbo].[TSPMaster] where vcInOut='1' and isnull(vcDelete, '') <> '1'            ");
                strSql.AppendLine("                         AND (([dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))             ");
                strSql.AppendLine("                          OR ([dFromTime]>=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23)))) a            ");
                strSql.AppendLine("                         left join (select vcPackingPlant,vcPartNo,vcReceiver,vcSupplier_id,dUseStartDate,dUserEndDate from THeZiManage)b                ");
                strSql.AppendLine("                         on a.vcPackingPlant = b.vcPackingPlant and a.vcPartId = b.vcPartNo and a.vcReceiver=b.vcReceiver and a.vcSupplierId=b.vcSupplier_id                ");
                strSql.AppendLine("                         left join (            ");
                strSql.AppendLine("                    	  SELECT  [vcPackingPlant] ,[vcPartId],[vcReceiver],[vcSupplierId],[dFromTime] ,[dToTime] ,[vcSupplierPlant],[vcOperatorType]                 ");
                strSql.AppendLine("                         FROM [SPPSdb].[dbo].[TSPMaster_SupplierPlant] where vcOperatorType='1'            ");
                strSql.AppendLine("                    	 AND (([dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))             ");
                strSql.AppendLine("                          OR ([dFromTime]>=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23)))           ");
                strSql.AppendLine("                    	 ) c on a.vcPackingPlant=c.vcPackingPlant and a.vcPartId=c.vcPartId                ");
                strSql.AppendLine("                         and a.vcReceiver=c.vcReceiver and a.vcSupplierId = c.vcSupplierId                 ");
                strSql.AppendLine("                      left join  (select vcPackingPlant, vcPartId, vcReceiver, vcSupplierId, vcSupplierPlant, dFromTime, dToTime,                  ");
                strSql.AppendLine("                      iPackingQty, vcBoxType, iLength, iWidth, iHeight, iVolume, vcOperatorType, vcOperatorID, dOperatorTime                  ");
                strSql.AppendLine("                       from TSPMaster_Box  where vcOperatorType='1'           ");
                strSql.AppendLine("                       AND (([dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))             ");
                strSql.AppendLine("                       OR ([dFromTime]>=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23)))           ");
                strSql.AppendLine("                       ) d on a.vcPackingPlant=d.vcPackingPlant and a.vcPartId=d.vcPartId               ");
                strSql.AppendLine("                       and a.vcReceiver=d.vcReceiver and  a.[vcSupplierId]=d.[vcSupplierId]             ");
                strSql.AppendLine("                       LEFT JOIN            ");
                strSql.AppendLine("                    	(SELECT *  FROM [TSPMaster_SufferIn] WHERE [vcOperatorType]='1'             ");
                strSql.AppendLine("                    	AND (([dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))             ");
                strSql.AppendLine("                    	OR ([dFromTime]>=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))))e           ");
                strSql.AppendLine("                    	ON a.[vcPackingPlant]=e.[vcPackingPlant] AND a.[vcPartId]=e.[vcPartId] AND a.[vcReceiver]=e.[vcReceiver] AND a.[vcSupplierId]=e.[vcSupplierId]            ");
                strSql.AppendLine("                    	LEFT JOIN            ");
                strSql.AppendLine("                    	(select vcValue1 as [vcSupplierId],vcValue2 as vcSupplierPlant,vcValue3 as [dFromTime],vcValue4 as [dToTime],vcValue5 as vcOrderPlant from TOutCode where vcCodeId='C010' and vcIsColum='0'            ");
                strSql.AppendLine("                    	and vcValue3<=CONVERT(VARCHAR(10),GETDATE(),23) AND vcValue4>=CONVERT(VARCHAR(10),GETDATE(),23))f            ");
                strSql.AppendLine("                    	ON a.[vcSupplierId]=f.[vcSupplierId] AND c.vcSupplierPlant=f.vcSupplierPlant            ");
                strSql.AppendLine("                         where b.vcPartNo is null            ");
                strSql.AppendLine("                    	 and (ISNULL(c.vcSupplierPlant,'')='' OR ISNULL(d.iPackingQty,0)=0 OR ISNULL(e.vcSufferIn,'')='' OR ISNULL(f.vcOrderPlant,'')='')                  ");
                strSql.AppendLine("                      union all              ");
                strSql.AppendLine("                        select * from (              ");
                strSql.AppendLine("                      	select [iAutoId], a.vcPackingPlant,a.vcReceiver, [dSynchronizationDate],  [vcState], [vcPartNo],              ");
                strSql.AppendLine("                      	[dUseStartDate],[dUserEndDate], [vcPartName],                 ");
                strSql.AppendLine("                      	[vcCarType], [vcOEOrSP], [vcSupplier_id], [vcWorkArea], [dExpectDeliveryDate], [vcExpectIntake],                ");
                strSql.AppendLine("                      	[vcIntake], [vcBoxMaxIntake], [vcBoxType], [vcLength], [vcWide], [vcHeight], [vcEmptyWeight],                 ");
                strSql.AppendLine("                      	[vcUnitNetWeight], [dSendDate], [dReplyDate], [dAdmitDate], [dWeaveDate], [vcMemo], vcImageRoutes,                 ");
                strSql.AppendLine("                      	[vcInserter], [vcInserterDate],[vcFactoryOperatorID], [dFactoryOperatorTime],                ");
                strSql.AppendLine("                      	[vcOperatorID], [dOperatorTime] as vcAddFlag from [dbo].[THeZiManage] a                 ");
                strSql.AppendLine("                      	               ");
                strSql.AppendLine("                      	) m              ");
                strSql.AppendLine("                      	) n            ");
                strSql.AppendLine("                    	left join (select vcValue,vcName from TCode where vcCodeId='C033') o on n.vcState = o.vcValue            ");
                strSql.AppendLine("                    	left join (select vcValue,vcName from TCode where vcCodeId='C012') p on n.vcOEOrSP = p.vcValue            ");
                strSql.AppendLine("                 ) T        ");
                strSql.AppendLine("                 ) S order by s.vcBoxType asc        ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 检索数据
        /// </summary>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        public DataTable Search(string dSynchronizationDateFrom, string dSynchronizationDateTo, string dSynchronizationDate, string vcState, string vcPartNo, string vcSupplier_id, string vcWorkArea, string vcCarType, string dExpectDeliveryDate, string vcOEOrSP, string vcBoxType,string dSendDate)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                //strSql.AppendLine("  select [iAutoId], [dSynchronizationDate], b.vcName as [vcState], [vcPartNo], [dUseStartDate], [vcPartName],   ");
                //strSql.AppendLine("  [vcCarType], [vcOEOrSP], [vcSupplier_id], [vcWorkArea], [dExpectDeliveryDate], [vcExpectIntake],   ");
                //strSql.AppendLine("  [vcIntake], [vcBoxMaxIntake], [vcBoxType], [vcLength], [vcWide], [vcHeight], [vcEmptyWeight],    ");
                //strSql.AppendLine("  [vcUnitNetWeight], [dSendDate], [dReplyDate], [dAdmitDate], [dWeaveDate], [vcMemo], vcImageRoutes,    ");
                //strSql.AppendLine("  [vcInserter], [vcInserterDate],[vcFactoryOperatorID], [dFactoryOperatorTime],   ");
                //strSql.AppendLine("  [vcOperatorID], [dOperatorTime],'0' as vcModFlag,'0' as vcAddFlag from [dbo].[THeZiManage] a   ");
                //strSql.AppendLine("  left join (select vcValue,vcName from TCode where vcCodeId='C033') b on a.vcState = b.vcValue  where 1=1   ");
                strSql.AppendLine("   select    ");
                strSql.AppendLine("   n.[iAutoId], n.vcPackingPlant,n.vcReceiver,convert(varchar(10), n.[dSynchronizationDate],111) as [dSynchronizationDate], o.vcName as [vcState], n.[vcPartNo],      ");
                strSql.AppendLine("     	convert(varchar(10), n.[dUseStartDate],111) as [dUseStartDate],convert(varchar(10), n.[dUserEndDate],111) as [dUserEndDate], n.[vcPartName],         ");
                strSql.AppendLine("     	n.[vcCarType],p.vcName as [vcOEOrSP], n.[vcSupplier_id], n.[vcWorkArea],convert(varchar(10), n.[dExpectDeliveryDate],111) as  [dExpectDeliveryDate], n.[vcExpectIntake] as [vcExpectIntake],        ");
                strSql.AppendLine("     	n.[vcIntake] as [vcIntake], n.[vcBoxMaxIntake] as [vcBoxMaxIntake], n.[vcBoxType], n.[vcLength], n.[vcWide], n.[vcHeight], n.[vcEmptyWeight],         ");
                strSql.AppendLine("     	n.[vcUnitNetWeight],convert(varchar(10), n.[dSendDate],111) as [dSendDate],convert(varchar(10), n.[dReplyDate],111) as [dReplyDate],convert(varchar(10), n.[dAdmitDate],111) as [dAdmitDate],convert(varchar(10), n.[dWeaveDate],111) as [dWeaveDate], n.[vcMemo], n.vcImageRoutes,         ");
                strSql.AppendLine("     	n.[vcInserter], n.[vcInserterDate],n.[vcFactoryOperatorID], n.[dFactoryOperatorTime],        ");
                strSql.AppendLine("     	n.[vcOperatorID], n.[dOperatorTime],'0' as vcModFlag,'0' as vcAddFlag   ");
                strSql.AppendLine("   from (      ");
                strSql.AppendLine("      select a.LinId as iAutoId,  a.vcPackingPlant,a.vcReceiver, a.dSyncTime as dSynchronizationDate, '0' as vcState,a.vcPartId as vcPartNo,      ");
                strSql.AppendLine("      a.dFromTime as dUseStartDate,a.dToTime as dUserEndDate,a.vcPartENName as vcPartName,a.vcCarModel as vcCarType,      ");
                strSql.AppendLine("      a.vcOESP as vcOEOrSP,a.vcSupplierId as vcSupplier_id,c.vcSupplierPlant as vcWorkArea,      ");
                strSql.AppendLine("      null as dExpectDeliveryDate,'' as vcExpectIntake,'' as [vcIntake],'' as [vcBoxMaxIntake],'' as [vcBoxType],'' as [vcLength],'' as [vcWide],      ");
                strSql.AppendLine("      '' as [vcHeight],'' as [vcEmptyWeight],'' as [vcUnitNetWeight],null as [dSendDate],null as [dReplyDate],null as [dAdmitDate],null as [dWeaveDate],     ");
                strSql.AppendLine("      '' as [vcMemo],null as  [vcImageRoutes],'' as [vcInserter],null as [vcInserterDate],'' as [vcFactoryOperatorID],null as [dFactoryOperatorTime],       ");
                strSql.AppendLine("     null as [vcOperatorID], null as [dOperatorTime]   ");
                //strSql.AppendLine("      from (select * from [dbo].[TSPMaster] where vcInOut='1') a      ");
                //strSql.AppendLine("      left join (select vcPackingPlant,vcPartNo,vcReceiver,vcSupplier_id,dUseStartDate,dUserEndDate from THeZiManage)b      ");
                //strSql.AppendLine("      on a.vcPackingPlant = b.vcPackingPlant and a.vcPartId = b.vcPartNo and a.vcReceiver=b.vcReceiver and a.vcSupplierId=b.vcSupplier_id      ");
                //strSql.AppendLine("      left join (  SELECT  [vcPackingPlant] ,[vcPartId],[vcReceiver],[vcSupplierId],[dFromTime] ,[dToTime] ,[vcSupplierPlant],[vcOperatorType]       ");
                //strSql.AppendLine("      FROM [SPPSdb].[dbo].[TSPMaster_SupplierPlant] where vcOperatorType='1' ) c on a.vcPackingPlant=c.vcPackingPlant and a.vcPartId=c.vcPartId      ");
                //strSql.AppendLine("      and a.vcReceiver=c.vcReceiver and a.dFromTime = c.dFromTime and a.dToTime=c.dToTime       ");
                //strSql.AppendLine("     left join  (select vcPackingPlant, vcPartId, vcReceiver, vcSupplierId, vcSupplierPlant, dFromTime, dToTime,    ");
                //strSql.AppendLine("     iPackingQty, vcBoxType, iLength, iWidth, iHeight, iVolume, vcOperatorType, vcOperatorID, dOperatorTime    ");
                //strSql.AppendLine("      from TSPMaster_Box  where vcOperatorType='1') d on a.vcPackingPlant=d.vcPackingPlant and a.vcPartId=d.vcPartId    ");
                //strSql.AppendLine("      and a.vcReceiver=d.vcReceiver and a.dFromTime = d.dFromTime and a.dToTime=d.dToTime   ");
                //strSql.AppendLine("        ");
                //strSql.AppendLine("      where b.vcPartNo is null and d.iPackingQty is null     ");
                strSql.AppendLine("        from  (select * from [dbo].[TSPMaster] where vcInOut='1' and isnull(vcDelete, '') <> '1'    ");
                strSql.AppendLine("        AND (([dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))     ");
                strSql.AppendLine("         OR ([dFromTime]>=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23)))) a    ");
                strSql.AppendLine("        left join (select vcPackingPlant,vcPartNo,vcReceiver,vcSupplier_id,dUseStartDate,dUserEndDate from THeZiManage)b        ");
                strSql.AppendLine("        on a.vcPackingPlant = b.vcPackingPlant and a.vcPartId = b.vcPartNo and a.vcReceiver=b.vcReceiver and a.vcSupplierId=b.vcSupplier_id        ");
                strSql.AppendLine("        left join (    ");
                strSql.AppendLine("   	  SELECT  [vcPackingPlant] ,[vcPartId],[vcReceiver],[vcSupplierId],[dFromTime] ,[dToTime] ,[vcSupplierPlant],[vcOperatorType]         ");
                strSql.AppendLine("        FROM [SPPSdb].[dbo].[TSPMaster_SupplierPlant] where vcOperatorType='1'    ");
                strSql.AppendLine("   	 AND (([dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))     ");
                strSql.AppendLine("         OR ([dFromTime]>=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23)))   ");
                strSql.AppendLine("   	 ) c on a.vcPackingPlant=c.vcPackingPlant and a.vcPartId=c.vcPartId        ");
                strSql.AppendLine("        and a.vcReceiver=c.vcReceiver and a.vcSupplierId = c.vcSupplierId         ");
                strSql.AppendLine("     left join  (select vcPackingPlant, vcPartId, vcReceiver, vcSupplierId, vcSupplierPlant, dFromTime, dToTime,          ");
                strSql.AppendLine("     iPackingQty, vcBoxType, iLength, iWidth, iHeight, iVolume, vcOperatorType, vcOperatorID, dOperatorTime          ");
                strSql.AppendLine("      from TSPMaster_Box  where vcOperatorType='1'   ");
                strSql.AppendLine("      AND (([dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))     ");
                strSql.AppendLine("      OR ([dFromTime]>=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23)))   ");
                strSql.AppendLine("      ) d on a.vcPackingPlant=d.vcPackingPlant and a.vcPartId=d.vcPartId       ");
                strSql.AppendLine("      and a.vcReceiver=d.vcReceiver and  a.[vcSupplierId]=d.[vcSupplierId]     ");
                strSql.AppendLine("      LEFT JOIN    ");
                strSql.AppendLine("   	(SELECT *  FROM [TSPMaster_SufferIn] WHERE [vcOperatorType]='1'     ");
                strSql.AppendLine("   	AND (([dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))     ");
                strSql.AppendLine("   	OR ([dFromTime]>=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))))e   ");
                strSql.AppendLine("   	ON a.[vcPackingPlant]=e.[vcPackingPlant] AND a.[vcPartId]=e.[vcPartId] AND a.[vcReceiver]=e.[vcReceiver] AND a.[vcSupplierId]=e.[vcSupplierId]    ");
                strSql.AppendLine("   	LEFT JOIN    ");
                strSql.AppendLine("   	(select vcValue1 as [vcSupplierId],vcValue2 as vcSupplierPlant,vcValue3 as [dFromTime],vcValue4 as [dToTime],vcValue5 as vcOrderPlant from TOutCode where vcCodeId='C010' and vcIsColum='0'    ");
                strSql.AppendLine("   	and vcValue3<=CONVERT(VARCHAR(10),GETDATE(),23) AND vcValue4>=CONVERT(VARCHAR(10),GETDATE(),23))f    ");
                strSql.AppendLine("   	ON a.[vcSupplierId]=f.[vcSupplierId] AND c.vcSupplierPlant=f.vcSupplierPlant    ");
                strSql.AppendLine("        where b.vcPartNo is null    ");
                strSql.AppendLine("   	 and (ISNULL(c.vcSupplierPlant,'')='' OR ISNULL(d.iPackingQty,0)=0 OR ISNULL(e.vcSufferIn,'')='' OR ISNULL(f.vcOrderPlant,'')='')          ");
                strSql.AppendLine("     union all      ");
                strSql.AppendLine("       select * from (      ");
                strSql.AppendLine("     	select [iAutoId], a.vcPackingPlant,a.vcReceiver, [dSynchronizationDate],  [vcState], [vcPartNo],      ");
                strSql.AppendLine("     	[dUseStartDate],[dUserEndDate], [vcPartName],         ");
                strSql.AppendLine("     	[vcCarType], [vcOEOrSP], [vcSupplier_id], [vcWorkArea], [dExpectDeliveryDate], [vcExpectIntake],        ");
                strSql.AppendLine("     	[vcIntake], [vcBoxMaxIntake], [vcBoxType], [vcLength], [vcWide], [vcHeight], [vcEmptyWeight],         ");
                strSql.AppendLine("     	[vcUnitNetWeight], [dSendDate], [dReplyDate], [dAdmitDate], [dWeaveDate], [vcMemo], vcImageRoutes,         ");
                strSql.AppendLine("     	[vcInserter], [vcInserterDate],[vcFactoryOperatorID], [dFactoryOperatorTime],        ");
                strSql.AppendLine("     	[vcOperatorID], [dOperatorTime] as vcAddFlag from [dbo].[THeZiManage] a         ");
                strSql.AppendLine("     	       ");
                strSql.AppendLine("     	) m      ");
                strSql.AppendLine("     	) n    ");
                strSql.AppendLine("   	left join (select vcValue,vcName from TCode where vcCodeId='C033') o on n.vcState = o.vcValue    ");
                strSql.AppendLine("   	left join (select vcValue,vcName from TCode where vcCodeId='C012') p on n.vcOEOrSP = p.vcValue    ");
                strSql.AppendLine("   	where 1=1     ");
                strSql.AppendLine("      ");

                if (dSynchronizationDateFrom.Length > 0)
                {
                    strSql.AppendLine("  and  CONVERT(varchar(10),  n.dSynchronizationDate,112) >= '" + dSynchronizationDateFrom.Replace("-","").Replace("/", "") + "' ");
                }
                if (dSynchronizationDateTo.Length > 0)
                {
                    strSql.AppendLine("  and  CONVERT(varchar(10),  n.dSynchronizationDate,112) <= '" + dSynchronizationDateTo.Replace("-", "").Replace("/", "") + "' ");
                }
                if (dSynchronizationDate.Length > 0)
                {
                    if (dSynchronizationDateFrom.Length > 0 || dSynchronizationDateTo.Length > 0)
                    {
                        strSql.AppendLine("  or  isnull(n.dSynchronizationDate,'') = '' ");
                    } else
                    {
                        strSql.AppendLine("  and  isnull(n.dSynchronizationDate,'') = '' ");
                    }
                    
                }
                if (vcState.Length > 0)
                {
                    strSql.AppendLine("  and  n.vcState = '" + vcState + "' ");
                }
                if (vcPartNo.Length > 0)
                {
                    strSql.AppendLine("  and  n.vcPartNo like '" + vcPartNo + "%' ");
                }
                
                if (vcSupplier_id.Length > 0)
                {
                    strSql.AppendLine("  and  n.vcSupplier_id  like  '" + vcSupplier_id + "%' ");
                }
                if (vcWorkArea.Length > 0)
                {
                    if (vcWorkArea == "无")
                    {
                        strSql.AppendLine("  and  isnull(n.vcWorkArea,'') = '' ");
                    }
                    else
                    {
                        strSql.AppendLine("  and  n.vcWorkArea = '" + vcWorkArea + "' ");
                    }
                }
                if (vcCarType.Length > 0)
                {
                    if (vcCarType == "无")
                    {
                        strSql.AppendLine("  and  isnull(n.vcCarType,'') = '' ");
                    }
                    else
                    {
                        strSql.AppendLine("  and  n.vcCarType like '" + vcCarType + "%' ");
                    }
                }
                if (dExpectDeliveryDate.Length > 0)
                {
                    strSql.AppendLine("  and  CONVERT(varchar(10),  n.dExpectDeliveryDate,112) = '" + dExpectDeliveryDate.Replace("-", "").Replace("/", "") + "' ");
                }
                if (dSendDate.Length > 0)
                {
                    if (dSendDate == "无")
                    {
                        strSql.AppendLine("  and  isnull(n.dSendDate,'') = '' ");
                    }
                    else
                    {
                        strSql.AppendLine("  and  CONVERT(varchar(10),  n.dSendDate,112) = '" + dSendDate.Replace("-", "").Replace("/", "") + "' ");
                    }
                }
                if (vcOEOrSP.Length > 0)
                {
                    strSql.AppendLine("  and  n.vcOEOrSP = '" + vcOEOrSP + "' ");
                }
                if (vcBoxType.Length > 0)
                {
                    if (vcBoxType == "无")
                    {
                        strSql.AppendLine("  and  isnull(n.vcBoxType,'') = '' ");
                    }
                    else
                    {
                        strSql.AppendLine("  and  left(n.vcBoxType,2) like '" + vcBoxType + "%' ");
                    }
                }

                strSql.AppendLine("  order by  n.[vcState] asc,  n.vcPartNo desc ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 检索数据
        /// </summary>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        public DataTable SearchImport(string dSynchronizationDateFrom, string dSynchronizationDateTo, string dSynchronizationDate, string vcState, string vcPartNo, string vcSupplier_id, string vcWorkArea, string vcCarType, string dExpectDeliveryDate, string vcOEOrSP, string vcBoxType, string dSendDate)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                //strSql.AppendLine("  select [iAutoId], [dSynchronizationDate], b.vcName as [vcState], [vcPartNo], [dUseStartDate], [vcPartName],   ");
                //strSql.AppendLine("  [vcCarType], [vcOEOrSP], [vcSupplier_id], [vcWorkArea], [dExpectDeliveryDate], [vcExpectIntake],   ");
                //strSql.AppendLine("  [vcIntake], [vcBoxMaxIntake], [vcBoxType], [vcLength], [vcWide], [vcHeight], [vcEmptyWeight],    ");
                //strSql.AppendLine("  [vcUnitNetWeight], [dSendDate], [dReplyDate], [dAdmitDate], [dWeaveDate], [vcMemo], vcImageRoutes,    ");
                //strSql.AppendLine("  [vcInserter], [vcInserterDate],[vcFactoryOperatorID], [dFactoryOperatorTime],   ");
                //strSql.AppendLine("  [vcOperatorID], [dOperatorTime],'0' as vcModFlag,'0' as vcAddFlag from [dbo].[THeZiManage] a   ");
                //strSql.AppendLine("  left join (select vcValue,vcName from TCode where vcCodeId='C033') b on a.vcState = b.vcValue  where 1=1   ");
                strSql.AppendLine("   select    ");
                strSql.AppendLine("   n.[iAutoId], n.vcPackingPlant,n.vcReceiver,convert(varchar(10), n.[dSynchronizationDate],111) as [dSynchronizationDate], o.vcName as [vcState], n.[vcPartNo],      ");
                strSql.AppendLine("     	convert(varchar(10), n.[dUseStartDate],111) as [dUseStartDate],convert(varchar(10), n.[dUserEndDate],111) as [dUserEndDate], n.[vcPartName],         ");
                strSql.AppendLine("     	n.[vcCarType],p.vcName as [vcOEOrSP], n.[vcSupplier_id], n.[vcWorkArea],convert(varchar(10), n.[dExpectDeliveryDate],111) as  [dExpectDeliveryDate], n.[vcExpectIntake] as [vcExpectIntake],        ");
                strSql.AppendLine("     	n.[vcIntake] as [vcIntake], n.[vcBoxMaxIntake] as [vcBoxMaxIntake], n.[vcBoxType], n.[vcLength], n.[vcWide], n.[vcHeight], n.[vcEmptyWeight],         ");
                strSql.AppendLine("     	n.[vcUnitNetWeight],convert(varchar(10), n.[dSendDate],111) as [dSendDate],convert(varchar(10), n.[dReplyDate],111) as [dReplyDate],convert(varchar(10), n.[dAdmitDate],111) as [dAdmitDate],convert(varchar(10), n.[dWeaveDate],111) as [dWeaveDate], n.[vcMemo], n.vcImageRoutes,         ");
                strSql.AppendLine("     	n.[vcInserter], n.[vcInserterDate],n.[vcFactoryOperatorID], n.[dFactoryOperatorTime],        ");
                strSql.AppendLine("     	n.[vcOperatorID], n.[dOperatorTime],'0' as vcModFlag,'0' as vcAddFlag   ");
                strSql.AppendLine("   from (      ");
                strSql.AppendLine("      select a.LinId as iAutoId,  a.vcPackingPlant,a.vcReceiver, a.dSyncTime as dSynchronizationDate, '0' as vcState,a.vcPartId as vcPartNo,      ");
                strSql.AppendLine("      a.dFromTime as dUseStartDate,a.dToTime as dUserEndDate,a.vcPartENName as vcPartName,a.vcCarModel as vcCarType,      ");
                strSql.AppendLine("      a.vcOESP as vcOEOrSP,a.vcSupplierId as vcSupplier_id,c.vcSupplierPlant as vcWorkArea,      ");
                strSql.AppendLine("      null as dExpectDeliveryDate,'' as vcExpectIntake,'' as [vcIntake],'' as [vcBoxMaxIntake],'' as [vcBoxType],'' as [vcLength],'' as [vcWide],      ");
                strSql.AppendLine("      '' as [vcHeight],'' as [vcEmptyWeight],'' as [vcUnitNetWeight],null as [dSendDate],null as [dReplyDate],null as [dAdmitDate],null as [dWeaveDate],     ");
                strSql.AppendLine("      '' as [vcMemo],null as  [vcImageRoutes],'' as [vcInserter],null as [vcInserterDate],'' as [vcFactoryOperatorID],null as [dFactoryOperatorTime],       ");
                strSql.AppendLine("     null as [vcOperatorID], null as [dOperatorTime]   ");
                //strSql.AppendLine("      from (select * from [dbo].[TSPMaster] where vcInOut='1') a      ");
                //strSql.AppendLine("      left join (select vcPackingPlant,vcPartNo,vcReceiver,vcSupplier_id,dUseStartDate,dUserEndDate from THeZiManage)b      ");
                //strSql.AppendLine("      on a.vcPackingPlant = b.vcPackingPlant and a.vcPartId = b.vcPartNo and a.vcReceiver=b.vcReceiver and a.vcSupplierId=b.vcSupplier_id      ");
                //strSql.AppendLine("      left join (  SELECT  [vcPackingPlant] ,[vcPartId],[vcReceiver],[vcSupplierId],[dFromTime] ,[dToTime] ,[vcSupplierPlant],[vcOperatorType]       ");
                //strSql.AppendLine("      FROM [SPPSdb].[dbo].[TSPMaster_SupplierPlant] where vcOperatorType='1' ) c on a.vcPackingPlant=c.vcPackingPlant and a.vcPartId=c.vcPartId      ");
                //strSql.AppendLine("      and a.vcReceiver=c.vcReceiver and a.dFromTime = c.dFromTime and a.dToTime=c.dToTime       ");
                //strSql.AppendLine("     left join  (select vcPackingPlant, vcPartId, vcReceiver, vcSupplierId, vcSupplierPlant, dFromTime, dToTime,    ");
                //strSql.AppendLine("     iPackingQty, vcBoxType, iLength, iWidth, iHeight, iVolume, vcOperatorType, vcOperatorID, dOperatorTime    ");
                //strSql.AppendLine("      from TSPMaster_Box  where vcOperatorType='1') d on a.vcPackingPlant=d.vcPackingPlant and a.vcPartId=d.vcPartId    ");
                //strSql.AppendLine("      and a.vcReceiver=d.vcReceiver and a.dFromTime = d.dFromTime and a.dToTime=d.dToTime   ");
                //strSql.AppendLine("        ");
                //strSql.AppendLine("      where b.vcPartNo is null and d.iPackingQty is null     ");
                strSql.AppendLine("        from  (select * from [dbo].[TSPMaster] where vcInOut='1' and isnull(vcDelete, '') <> '1'    ");
                strSql.AppendLine("        AND (([dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))     ");
                strSql.AppendLine("         OR ([dFromTime]>=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23)))) a    ");
                strSql.AppendLine("        left join (select vcPackingPlant,vcPartNo,vcReceiver,vcSupplier_id,dUseStartDate,dUserEndDate from THeZiManage)b        ");
                strSql.AppendLine("        on a.vcPackingPlant = b.vcPackingPlant and a.vcPartId = b.vcPartNo and a.vcReceiver=b.vcReceiver and a.vcSupplierId=b.vcSupplier_id        ");
                strSql.AppendLine("        left join (    ");
                strSql.AppendLine("   	  SELECT  [vcPackingPlant] ,[vcPartId],[vcReceiver],[vcSupplierId],[dFromTime] ,[dToTime] ,[vcSupplierPlant],[vcOperatorType]         ");
                strSql.AppendLine("        FROM [SPPSdb].[dbo].[TSPMaster_SupplierPlant] where vcOperatorType='1'    ");
                strSql.AppendLine("   	 AND (([dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))     ");
                strSql.AppendLine("         OR ([dFromTime]>=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23)))   ");
                strSql.AppendLine("   	 ) c on a.vcPackingPlant=c.vcPackingPlant and a.vcPartId=c.vcPartId        ");
                strSql.AppendLine("        and a.vcReceiver=c.vcReceiver and a.vcSupplierId = c.vcSupplierId         ");
                strSql.AppendLine("     left join  (select vcPackingPlant, vcPartId, vcReceiver, vcSupplierId, vcSupplierPlant, dFromTime, dToTime,          ");
                strSql.AppendLine("     iPackingQty, vcBoxType, iLength, iWidth, iHeight, iVolume, vcOperatorType, vcOperatorID, dOperatorTime          ");
                strSql.AppendLine("      from TSPMaster_Box  where vcOperatorType='1'   ");
                strSql.AppendLine("      AND (([dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))     ");
                strSql.AppendLine("      OR ([dFromTime]>=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23)))   ");
                strSql.AppendLine("      ) d on a.vcPackingPlant=d.vcPackingPlant and a.vcPartId=d.vcPartId       ");
                strSql.AppendLine("      and a.vcReceiver=d.vcReceiver and  a.[vcSupplierId]=d.[vcSupplierId]     ");
                strSql.AppendLine("      LEFT JOIN    ");
                strSql.AppendLine("   	(SELECT *  FROM [TSPMaster_SufferIn] WHERE [vcOperatorType]='1'     ");
                strSql.AppendLine("   	AND (([dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))     ");
                strSql.AppendLine("   	OR ([dFromTime]>=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))))e   ");
                strSql.AppendLine("   	ON a.[vcPackingPlant]=e.[vcPackingPlant] AND a.[vcPartId]=e.[vcPartId] AND a.[vcReceiver]=e.[vcReceiver] AND a.[vcSupplierId]=e.[vcSupplierId]    ");
                strSql.AppendLine("   	LEFT JOIN    ");
                strSql.AppendLine("   	(select vcValue1 as [vcSupplierId],vcValue2 as vcSupplierPlant,vcValue3 as [dFromTime],vcValue4 as [dToTime],vcValue5 as vcOrderPlant from TOutCode where vcCodeId='C010' and vcIsColum='0'    ");
                strSql.AppendLine("   	and vcValue3<=CONVERT(VARCHAR(10),GETDATE(),23) AND vcValue4>=CONVERT(VARCHAR(10),GETDATE(),23))f    ");
                strSql.AppendLine("   	ON a.[vcSupplierId]=f.[vcSupplierId] AND c.vcSupplierPlant=f.vcSupplierPlant    ");
                strSql.AppendLine("        where b.vcPartNo is null    ");
                strSql.AppendLine("   	 and (ISNULL(c.vcSupplierPlant,'')='' OR ISNULL(d.iPackingQty,0)=0 OR ISNULL(e.vcSufferIn,'')='' OR ISNULL(f.vcOrderPlant,'')='')          ");
                strSql.AppendLine("     union all      ");
                strSql.AppendLine("       select * from (      ");
                strSql.AppendLine("     	select [iAutoId], a.vcPackingPlant,a.vcReceiver, [dSynchronizationDate],  [vcState], [vcPartNo],      ");
                strSql.AppendLine("     	[dUseStartDate],[dUserEndDate], [vcPartName],         ");
                strSql.AppendLine("     	[vcCarType], [vcOEOrSP], [vcSupplier_id], [vcWorkArea], [dExpectDeliveryDate], [vcExpectIntake],        ");
                strSql.AppendLine("     	[vcIntake], [vcBoxMaxIntake], [vcBoxType], [vcLength], [vcWide], [vcHeight], [vcEmptyWeight],         ");
                strSql.AppendLine("     	[vcUnitNetWeight], [dSendDate], [dReplyDate], [dAdmitDate], [dWeaveDate], [vcMemo], vcImageRoutes,         ");
                strSql.AppendLine("     	[vcInserter], [vcInserterDate],[vcFactoryOperatorID], [dFactoryOperatorTime],        ");
                strSql.AppendLine("     	[vcOperatorID], [dOperatorTime] as vcAddFlag from [dbo].[THeZiManage] where vcState<>'5'  a         ");
                strSql.AppendLine("     	       ");
                strSql.AppendLine("     	) m      ");
                strSql.AppendLine("     	) n    ");
                strSql.AppendLine("   	left join (select vcValue,vcName from TCode where vcCodeId='C033') o on n.vcState = o.vcValue    ");
                strSql.AppendLine("   	left join (select vcValue,vcName from TCode where vcCodeId='C012') p on n.vcOEOrSP = p.vcValue    ");
                strSql.AppendLine("   	where 1=1     ");
                strSql.AppendLine("      ");

                if (dSynchronizationDateFrom.Length > 0)
                {
                    strSql.AppendLine("  and  CONVERT(varchar(10),  n.dSynchronizationDate,112) >= '" + dSynchronizationDateFrom.Replace("-", "").Replace("/", "") + "' ");
                }
                if (dSynchronizationDateTo.Length > 0)
                {
                    strSql.AppendLine("  and  CONVERT(varchar(10),  n.dSynchronizationDate,112) <= '" + dSynchronizationDateTo.Replace("-", "").Replace("/", "") + "' ");
                }
                if (dSynchronizationDate.Length > 0)
                {
                    if (dSynchronizationDateFrom.Length > 0 || dSynchronizationDateTo.Length > 0)
                    {
                        strSql.AppendLine("  or  isnull(n.dSynchronizationDate,'') = '' ");
                    }
                    else
                    {
                        strSql.AppendLine("  and  isnull(n.dSynchronizationDate,'') = '' ");
                    }

                }
                if (vcState.Length > 0)
                {
                    strSql.AppendLine("  and  n.vcState = '" + vcState + "' ");
                }
                if (vcPartNo.Length > 0)
                {
                    strSql.AppendLine("  and  n.vcPartNo like '" + vcPartNo + "%' ");
                }

                if (vcSupplier_id.Length > 0)
                {
                    strSql.AppendLine("  and  n.vcSupplier_id  like  '" + vcSupplier_id + "%' ");
                }
                if (vcWorkArea.Length > 0)
                {
                    if (vcWorkArea == "无")
                    {
                        strSql.AppendLine("  and  isnull(n.vcWorkArea,'') = '' ");
                    }
                    else
                    {
                        strSql.AppendLine("  and  n.vcWorkArea = '" + vcWorkArea + "' ");
                    }
                }
                if (vcCarType.Length > 0)
                {
                    if (vcCarType == "无")
                    {
                        strSql.AppendLine("  and  isnull(n.vcCarType,'') = '' ");
                    }
                    else
                    {
                        strSql.AppendLine("  and  n.vcCarType like '" + vcCarType + "%' ");
                    }
                }
                if (dExpectDeliveryDate.Length > 0)
                {
                    strSql.AppendLine("  and  CONVERT(varchar(10),  n.dExpectDeliveryDate,112) = '" + dExpectDeliveryDate.Replace("-", "").Replace("/", "") + "' ");
                }
                if (dSendDate.Length > 0)
                {
                    if (dSendDate == "无")
                    {
                        strSql.AppendLine("  and  isnull(n.dSendDate,'') = '' ");
                    }
                    else
                    {
                        strSql.AppendLine("  and  CONVERT(varchar(10),  n.dSendDate,112) = '" + dSendDate.Replace("-", "").Replace("/", "") + "' ");
                    }
                }
                if (vcOEOrSP.Length > 0)
                {
                    strSql.AppendLine("  and  n.vcOEOrSP = '" + vcOEOrSP + "' ");
                }
                if (vcBoxType.Length > 0)
                {
                    if (vcBoxType == "无")
                    {
                        strSql.AppendLine("  and  isnull(n.vcBoxType,'') = '' ");
                    }
                    else
                    {
                        strSql.AppendLine("  and  left(n.vcBoxType,2) like '" + vcBoxType + "%' ");
                    }
                }

                strSql.AppendLine("  order by  n.[vcState] asc,  n.vcPartNo desc ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable dSendDate()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("  select isnull(dSendDate,'无') as vcValue,isnull(dSendDate,'无') as vcName from(     ");
                strSql.AppendLine("    	select  distinct convert(varchar(10), dSendDate,111) as dSendDate from [THeZiManage] where vcState<>5   ");
                strSql.AppendLine("  ) t order by vcValue desc  ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetTaskNum2()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("  select * from [THeZiManage] where vcState in ('3')  ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable checkIsExistByPartNo(string vcPartNo)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("    select LinId, dSyncTime, vcChanges, vcPackingPlant, vcPartId, vcPartENName, vcCarfamilyCode,     ");
                strSql.AppendLine("    vcCarModel, vcReceiver, dFromTime, dToTime, vcPartId_Replace, vcInOut, vcOESP, vcHaoJiu,     ");
                strSql.AppendLine("     vcOldProduction, dOldStartTime, dDebugTime, vcSupplierId, dSupplierFromTime, dSupplierToTime,     ");
                strSql.AppendLine("  	vcSupplierName, vcSupplierPlace, vcInteriorProject, vcPassProject, vcFrontProject,     ");
                strSql.AppendLine("  	dFrontProjectTime, dShipmentTime, vcBillType, vcOrderingMethod, vcMandOrder, vcPartImage,     ");
                strSql.AppendLine("  	 vcRemark1, vcRemark2, vcSupplierPacking, vcDelete,    ");
                strSql.AppendLine("     vcOperatorID, dOperatorTime, dSyncToSPTime from [dbo].[TSPMaster] where vcPartId='"+ vcPartNo + "'    ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetCarType()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("   select vcCarType as vcValue,vcCarType as vcName from(        ");
                strSql.AppendLine("                  select distinct isnull(vcCarType,'无') as vcCarType from (        ");
                strSql.AppendLine("                    select            ");
                strSql.AppendLine("                     n.[iAutoId], n.vcPackingPlant,n.vcReceiver, n.[dSynchronizationDate], o.vcName as [vcState], n.[vcPartNo],              ");
                strSql.AppendLine("                       	n.[dUseStartDate],n.[dUserEndDate], n.[vcPartName],                 ");
                strSql.AppendLine("                       	n.[vcCarType],p.vcName as [vcOEOrSP], n.[vcSupplier_id], n.[vcWorkArea], n.[dExpectDeliveryDate], n.[vcExpectIntake],                ");
                strSql.AppendLine("                       	n.[vcIntake], n.[vcBoxMaxIntake], n.[vcBoxType], n.[vcLength], n.[vcWide], n.[vcHeight], n.[vcEmptyWeight],                 ");
                strSql.AppendLine("                       	n.[vcUnitNetWeight], n.[dSendDate], n.[dReplyDate], n.[dAdmitDate], n.[dWeaveDate], n.[vcMemo], n.vcImageRoutes,                 ");
                strSql.AppendLine("                       	n.[vcInserter], n.[vcInserterDate],n.[vcFactoryOperatorID], n.[dFactoryOperatorTime],                ");
                strSql.AppendLine("                       	n.[vcOperatorID], n.[dOperatorTime],'0' as vcModFlag,'0' as vcAddFlag           ");
                strSql.AppendLine("                     from (              ");
                strSql.AppendLine("                        select null as iAutoId,  a.vcPackingPlant,a.vcReceiver, a.dSyncTime as dSynchronizationDate, '0' as vcState,a.vcPartId as vcPartNo,              ");
                strSql.AppendLine("                        a.dFromTime as dUseStartDate,a.dToTime as dUserEndDate,a.vcPartENName as vcPartName,a.vcCarModel as vcCarType,              ");
                strSql.AppendLine("                        a.vcOESP as vcOEOrSP,a.vcSupplierId as vcSupplier_id,c.vcSupplierPlant as vcWorkArea,              ");
                strSql.AppendLine("                        null as dExpectDeliveryDate,'' as vcExpectIntake,'' as [vcIntake],'' as [vcBoxMaxIntake],'' as [vcBoxType],'' as [vcLength],'' as [vcWide],              ");
                strSql.AppendLine("                        '' as [vcHeight],'' as [vcEmptyWeight],'' as [vcUnitNetWeight],null as [dSendDate],null as [dReplyDate],null as [dAdmitDate],null as [dWeaveDate],             ");
                strSql.AppendLine("                        '' as [vcMemo],null as  [vcImageRoutes],'' as [vcInserter],null as [vcInserterDate],'' as [vcFactoryOperatorID],null as [dFactoryOperatorTime],               ");
                strSql.AppendLine("                       null as [vcOperatorID], null as [dOperatorTime]           ");
                strSql.AppendLine("                          from  (select * from [dbo].[TSPMaster] where vcInOut='1' and isnull(vcDelete, '') <> '1'            ");
                strSql.AppendLine("                          AND (([dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))             ");
                strSql.AppendLine("                           OR ([dFromTime]>=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23)))) a            ");
                strSql.AppendLine("                          left join (select vcPackingPlant,vcPartNo,vcReceiver,vcSupplier_id,dUseStartDate,dUserEndDate from THeZiManage)b                ");
                strSql.AppendLine("                          on a.vcPackingPlant = b.vcPackingPlant and a.vcPartId = b.vcPartNo and a.vcReceiver=b.vcReceiver and a.vcSupplierId=b.vcSupplier_id                ");
                strSql.AppendLine("                          left join (            ");
                strSql.AppendLine("                     	  SELECT  [vcPackingPlant] ,[vcPartId],[vcReceiver],[vcSupplierId],[dFromTime] ,[dToTime] ,[vcSupplierPlant],[vcOperatorType]                 ");
                strSql.AppendLine("                          FROM [SPPSdb].[dbo].[TSPMaster_SupplierPlant] where vcOperatorType='1'            ");
                strSql.AppendLine("                     	 AND (([dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))             ");
                strSql.AppendLine("                           OR ([dFromTime]>=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23)))           ");
                strSql.AppendLine("                     	 ) c on a.vcPackingPlant=c.vcPackingPlant and a.vcPartId=c.vcPartId                ");
                strSql.AppendLine("                          and a.vcReceiver=c.vcReceiver and a.vcSupplierId = c.vcSupplierId                 ");
                strSql.AppendLine("                       left join  (select vcPackingPlant, vcPartId, vcReceiver, vcSupplierId, vcSupplierPlant, dFromTime, dToTime,                  ");
                strSql.AppendLine("                       iPackingQty, vcBoxType, iLength, iWidth, iHeight, iVolume, vcOperatorType, vcOperatorID, dOperatorTime                  ");
                strSql.AppendLine("                        from TSPMaster_Box  where vcOperatorType='1'           ");
                strSql.AppendLine("                        AND (([dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))             ");
                strSql.AppendLine("                        OR ([dFromTime]>=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23)))           ");
                strSql.AppendLine("                        ) d on a.vcPackingPlant=d.vcPackingPlant and a.vcPartId=d.vcPartId               ");
                strSql.AppendLine("                        and a.vcReceiver=d.vcReceiver and  a.[vcSupplierId]=d.[vcSupplierId]             ");
                strSql.AppendLine("                        LEFT JOIN            ");
                strSql.AppendLine("                     	(SELECT *  FROM [TSPMaster_SufferIn] WHERE [vcOperatorType]='1'             ");
                strSql.AppendLine("                     	AND (([dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))             ");
                strSql.AppendLine("                     	OR ([dFromTime]>=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))))e           ");
                strSql.AppendLine("                     	ON a.[vcPackingPlant]=e.[vcPackingPlant] AND a.[vcPartId]=e.[vcPartId] AND a.[vcReceiver]=e.[vcReceiver] AND a.[vcSupplierId]=e.[vcSupplierId]            ");
                strSql.AppendLine("                     	LEFT JOIN            ");
                strSql.AppendLine("                     	(select vcValue1 as [vcSupplierId],vcValue2 as vcSupplierPlant,vcValue3 as [dFromTime],vcValue4 as [dToTime],vcValue5 as vcOrderPlant from TOutCode where vcCodeId='C010' and vcIsColum='0'            ");
                strSql.AppendLine("                     	and vcValue3<=CONVERT(VARCHAR(10),GETDATE(),23) AND vcValue4>=CONVERT(VARCHAR(10),GETDATE(),23))f            ");
                strSql.AppendLine("                     	ON a.[vcSupplierId]=f.[vcSupplierId] AND c.vcSupplierPlant=f.vcSupplierPlant            ");
                strSql.AppendLine("                          where b.vcPartNo is null            ");
                strSql.AppendLine("                     	 and (ISNULL(c.vcSupplierPlant,'')='' OR ISNULL(d.iPackingQty,0)=0 OR ISNULL(e.vcSufferIn,'')='' OR ISNULL(f.vcOrderPlant,'')='')                  ");
                strSql.AppendLine("                       union all              ");
                strSql.AppendLine("                         select * from (              ");
                strSql.AppendLine("                       	select [iAutoId], a.vcPackingPlant,a.vcReceiver, [dSynchronizationDate],  [vcState], [vcPartNo],              ");
                strSql.AppendLine("                       	[dUseStartDate],[dUserEndDate], [vcPartName],                 ");
                strSql.AppendLine("                       	[vcCarType], [vcOEOrSP], [vcSupplier_id], [vcWorkArea], [dExpectDeliveryDate], [vcExpectIntake],                ");
                strSql.AppendLine("                       	[vcIntake], [vcBoxMaxIntake], [vcBoxType], [vcLength], [vcWide], [vcHeight], [vcEmptyWeight],                 ");
                strSql.AppendLine("                       	[vcUnitNetWeight], [dSendDate], [dReplyDate], [dAdmitDate], [dWeaveDate], [vcMemo], vcImageRoutes,                 ");
                strSql.AppendLine("                       	[vcInserter], [vcInserterDate],[vcFactoryOperatorID], [dFactoryOperatorTime],                ");
                strSql.AppendLine("                       	[vcOperatorID], [dOperatorTime] as vcAddFlag from [dbo].[THeZiManage] a                 ");
                strSql.AppendLine("                       	               ");
                strSql.AppendLine("                       	) m              ");
                strSql.AppendLine("                       	) n            ");
                strSql.AppendLine("                     	left join (select vcValue,vcName from TCode where vcCodeId='C033') o on n.vcState = o.vcValue            ");
                strSql.AppendLine("                     	left join (select vcValue,vcName from TCode where vcCodeId='C012') p on n.vcOEOrSP = p.vcValue            ");
                strSql.AppendLine("                  ) T        ");
                strSql.AppendLine("                  ) S order by s.vcCarType asc        ");
                strSql.AppendLine("                          ");
                strSql.AppendLine("                 ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 通过供应商查看邮箱
        /// </summary>
        /// <param name="strSupplier"></param>
        /// <returns></returns>
        public DataTable CheckEmail(string strSupplier)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("   select * from TSupplierInfo where vcSupplier_id='" + strSupplier + "'   ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable isCheckImportAddData(string vcPackingPlant, string vcReceiver, string vcSupplier_id, string vcPartNo)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("   select * from [THeZiManage] where vcPackingPlant='"+ vcPackingPlant + "'   ");
                strSql.AppendLine("    and vcReceiver='" + vcReceiver + "' and vcSupplier_id='" + vcSupplier_id + "' and vcPartNo='" + vcPartNo + "'   ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void sdweaveHandle(List<Dictionary<string, object>> listInfoData, string userId)
        {
            try
            {

                StringBuilder sql = new StringBuilder();
                sql.Append("  update  [THeZiManage] set vcState='5',dWeaveDate=GETDATE(),vcOperatorID='" + userId + "',dOperatorTime=GETDATE() where iAutoId in(   \r\n ");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (i != 0)
                        sql.Append(",");
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                    sql.Append(iAutoId);
                }
                sql.Append("  )   \r\n ");
                excute.ExcuteSqlWithStringOper(sql.ToString());

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetExpectDeliveryDate()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("  select dExpectDeliveryDate as vcValue,dExpectDeliveryDate as vcName from(    ");
                strSql.AppendLine("    	select  distinct convert(varchar(10), dExpectDeliveryDate,111) as dExpectDeliveryDate from [THeZiManage] where vcState<>5   ");
                strSql.AppendLine("  ) t order by vcValue desc  ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetSupplier()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

               
                strSql.AppendLine("     select vcSupplier_id as vcValue,vcSupplier_id as vcName from(      ");
                strSql.AppendLine("     select distinct vcSupplier_id from (      ");
                strSql.AppendLine("       select          ");
                strSql.AppendLine("        n.[iAutoId], n.vcPackingPlant,n.vcReceiver, n.[dSynchronizationDate], o.vcName as [vcState], n.[vcPartNo],            ");
                strSql.AppendLine("          	n.[dUseStartDate],n.[dUserEndDate], n.[vcPartName],               ");
                strSql.AppendLine("          	n.[vcCarType],p.vcName as [vcOEOrSP], n.[vcSupplier_id], n.[vcWorkArea], n.[dExpectDeliveryDate], n.[vcExpectIntake],              ");
                strSql.AppendLine("          	n.[vcIntake], n.[vcBoxMaxIntake], n.[vcBoxType], n.[vcLength], n.[vcWide], n.[vcHeight], n.[vcEmptyWeight],               ");
                strSql.AppendLine("          	n.[vcUnitNetWeight], n.[dSendDate], n.[dReplyDate], n.[dAdmitDate], n.[dWeaveDate], n.[vcMemo], n.vcImageRoutes,               ");
                strSql.AppendLine("          	n.[vcInserter], n.[vcInserterDate],n.[vcFactoryOperatorID], n.[dFactoryOperatorTime],              ");
                strSql.AppendLine("          	n.[vcOperatorID], n.[dOperatorTime],'0' as vcModFlag,'0' as vcAddFlag         ");
                strSql.AppendLine("        from (            ");
                strSql.AppendLine("           select null as iAutoId,  a.vcPackingPlant,a.vcReceiver, a.dSyncTime as dSynchronizationDate, '0' as vcState,a.vcPartId as vcPartNo,            ");
                strSql.AppendLine("           a.dFromTime as dUseStartDate,a.dToTime as dUserEndDate,a.vcPartENName as vcPartName,a.vcCarModel as vcCarType,            ");
                strSql.AppendLine("           a.vcOESP as vcOEOrSP,a.vcSupplierId as vcSupplier_id,c.vcSupplierPlant as vcWorkArea,            ");
                strSql.AppendLine("           null as dExpectDeliveryDate,'' as vcExpectIntake,'' as [vcIntake],'' as [vcBoxMaxIntake],'' as [vcBoxType],'' as [vcLength],'' as [vcWide],            ");
                strSql.AppendLine("           '' as [vcHeight],'' as [vcEmptyWeight],'' as [vcUnitNetWeight],null as [dSendDate],null as [dReplyDate],null as [dAdmitDate],null as [dWeaveDate],           ");
                strSql.AppendLine("           '' as [vcMemo],null as  [vcImageRoutes],'' as [vcInserter],null as [vcInserterDate],'' as [vcFactoryOperatorID],null as [dFactoryOperatorTime],             ");
                strSql.AppendLine("          null as [vcOperatorID], null as [dOperatorTime]         ");
                strSql.AppendLine("             from  (select * from [dbo].[TSPMaster] where vcInOut='1' and isnull(vcDelete, '') <> '1'          ");
                strSql.AppendLine("             AND (([dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))           ");
                strSql.AppendLine("              OR ([dFromTime]>=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23)))) a          ");
                strSql.AppendLine("             left join (select vcPackingPlant,vcPartNo,vcReceiver,vcSupplier_id,dUseStartDate,dUserEndDate from THeZiManage)b              ");
                strSql.AppendLine("             on a.vcPackingPlant = b.vcPackingPlant and a.vcPartId = b.vcPartNo and a.vcReceiver=b.vcReceiver and a.vcSupplierId=b.vcSupplier_id              ");
                strSql.AppendLine("             left join (          ");
                strSql.AppendLine("        	  SELECT  [vcPackingPlant] ,[vcPartId],[vcReceiver],[vcSupplierId],[dFromTime] ,[dToTime] ,[vcSupplierPlant],[vcOperatorType]               ");
                strSql.AppendLine("             FROM [SPPSdb].[dbo].[TSPMaster_SupplierPlant] where vcOperatorType='1'          ");
                strSql.AppendLine("        	 AND (([dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))           ");
                strSql.AppendLine("              OR ([dFromTime]>=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23)))         ");
                strSql.AppendLine("        	 ) c on a.vcPackingPlant=c.vcPackingPlant and a.vcPartId=c.vcPartId              ");
                strSql.AppendLine("             and a.vcReceiver=c.vcReceiver and a.vcSupplierId = c.vcSupplierId               ");
                strSql.AppendLine("          left join  (select vcPackingPlant, vcPartId, vcReceiver, vcSupplierId, vcSupplierPlant, dFromTime, dToTime,                ");
                strSql.AppendLine("          iPackingQty, vcBoxType, iLength, iWidth, iHeight, iVolume, vcOperatorType, vcOperatorID, dOperatorTime                ");
                strSql.AppendLine("           from TSPMaster_Box  where vcOperatorType='1'         ");
                strSql.AppendLine("           AND (([dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))           ");
                strSql.AppendLine("           OR ([dFromTime]>=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23)))         ");
                strSql.AppendLine("           ) d on a.vcPackingPlant=d.vcPackingPlant and a.vcPartId=d.vcPartId             ");
                strSql.AppendLine("           and a.vcReceiver=d.vcReceiver and  a.[vcSupplierId]=d.[vcSupplierId]           ");
                strSql.AppendLine("           LEFT JOIN          ");
                strSql.AppendLine("        	(SELECT *  FROM [TSPMaster_SufferIn] WHERE [vcOperatorType]='1'           ");
                strSql.AppendLine("        	AND (([dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))           ");
                strSql.AppendLine("        	OR ([dFromTime]>=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))))e         ");
                strSql.AppendLine("        	ON a.[vcPackingPlant]=e.[vcPackingPlant] AND a.[vcPartId]=e.[vcPartId] AND a.[vcReceiver]=e.[vcReceiver] AND a.[vcSupplierId]=e.[vcSupplierId]          ");
                strSql.AppendLine("        	LEFT JOIN          ");
                strSql.AppendLine("        	(select vcValue1 as [vcSupplierId],vcValue2 as vcSupplierPlant,vcValue3 as [dFromTime],vcValue4 as [dToTime],vcValue5 as vcOrderPlant from TOutCode where vcCodeId='C010' and vcIsColum='0'          ");
                strSql.AppendLine("        	and vcValue3<=CONVERT(VARCHAR(10),GETDATE(),23) AND vcValue4>=CONVERT(VARCHAR(10),GETDATE(),23))f          ");
                strSql.AppendLine("        	ON a.[vcSupplierId]=f.[vcSupplierId] AND c.vcSupplierPlant=f.vcSupplierPlant          ");
                strSql.AppendLine("             where b.vcPartNo is null          ");
                strSql.AppendLine("        	 and (ISNULL(c.vcSupplierPlant,'')='' OR ISNULL(d.iPackingQty,0)=0 OR ISNULL(e.vcSufferIn,'')='' OR ISNULL(f.vcOrderPlant,'')='')                ");
                strSql.AppendLine("          union all            ");
                strSql.AppendLine("            select * from (            ");
                strSql.AppendLine("          	select [iAutoId], a.vcPackingPlant,a.vcReceiver, [dSynchronizationDate],  [vcState], [vcPartNo],            ");
                strSql.AppendLine("          	[dUseStartDate],[dUserEndDate], [vcPartName],               ");
                strSql.AppendLine("          	[vcCarType], [vcOEOrSP], [vcSupplier_id], [vcWorkArea], [dExpectDeliveryDate], [vcExpectIntake],              ");
                strSql.AppendLine("          	[vcIntake], [vcBoxMaxIntake], [vcBoxType], [vcLength], [vcWide], [vcHeight], [vcEmptyWeight],               ");
                strSql.AppendLine("          	[vcUnitNetWeight], [dSendDate], [dReplyDate], [dAdmitDate], [dWeaveDate], [vcMemo], vcImageRoutes,               ");
                strSql.AppendLine("          	[vcInserter], [vcInserterDate],[vcFactoryOperatorID], [dFactoryOperatorTime],              ");
                strSql.AppendLine("          	[vcOperatorID], [dOperatorTime] as vcAddFlag from [dbo].[THeZiManage] a               ");
                strSql.AppendLine("          	             ");
                strSql.AppendLine("          	) m            ");
                strSql.AppendLine("          	) n          ");
                strSql.AppendLine("        	left join (select vcValue,vcName from TCode where vcCodeId='C033') o on n.vcState = o.vcValue          ");
                strSql.AppendLine("        	left join (select vcValue,vcName from TCode where vcCodeId='C012') p on n.vcOEOrSP = p.vcValue          ");
                strSql.AppendLine("     ) T      ");
                strSql.AppendLine("     ) S order by s.vcSupplier_id asc      ");
                strSql.AppendLine("           ");
               

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetWorkArea()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("    select vcWorkArea as vcValue,vcWorkArea as vcName from(       ");
                strSql.AppendLine("    select distinct isnull( vcWorkArea,'无') as vcWorkArea from (       ");
                strSql.AppendLine("      select           ");
                strSql.AppendLine("       n.[iAutoId], n.vcPackingPlant,n.vcReceiver, n.[dSynchronizationDate], o.vcName as [vcState], n.[vcPartNo],             ");
                strSql.AppendLine("         	n.[dUseStartDate],n.[dUserEndDate], n.[vcPartName],                ");
                strSql.AppendLine("         	n.[vcCarType],p.vcName as [vcOEOrSP], n.[vcSupplier_id], n.[vcWorkArea], n.[dExpectDeliveryDate], n.[vcExpectIntake],               ");
                strSql.AppendLine("         	n.[vcIntake], n.[vcBoxMaxIntake], n.[vcBoxType], n.[vcLength], n.[vcWide], n.[vcHeight], n.[vcEmptyWeight],                ");
                strSql.AppendLine("         	n.[vcUnitNetWeight], n.[dSendDate], n.[dReplyDate], n.[dAdmitDate], n.[dWeaveDate], n.[vcMemo], n.vcImageRoutes,                ");
                strSql.AppendLine("         	n.[vcInserter], n.[vcInserterDate],n.[vcFactoryOperatorID], n.[dFactoryOperatorTime],               ");
                strSql.AppendLine("         	n.[vcOperatorID], n.[dOperatorTime],'0' as vcModFlag,'0' as vcAddFlag          ");
                strSql.AppendLine("       from (             ");
                strSql.AppendLine("          select null as iAutoId,  a.vcPackingPlant,a.vcReceiver, a.dSyncTime as dSynchronizationDate, '0' as vcState,a.vcPartId as vcPartNo,             ");
                strSql.AppendLine("          a.dFromTime as dUseStartDate,a.dToTime as dUserEndDate,a.vcPartENName as vcPartName,a.vcCarModel as vcCarType,             ");
                strSql.AppendLine("          a.vcOESP as vcOEOrSP,a.vcSupplierId as vcSupplier_id,c.vcSupplierPlant as vcWorkArea,             ");
                strSql.AppendLine("          null as dExpectDeliveryDate,'' as vcExpectIntake,'' as [vcIntake],'' as [vcBoxMaxIntake],'' as [vcBoxType],'' as [vcLength],'' as [vcWide],             ");
                strSql.AppendLine("          '' as [vcHeight],'' as [vcEmptyWeight],'' as [vcUnitNetWeight],null as [dSendDate],null as [dReplyDate],null as [dAdmitDate],null as [dWeaveDate],            ");
                strSql.AppendLine("          '' as [vcMemo],null as  [vcImageRoutes],'' as [vcInserter],null as [vcInserterDate],'' as [vcFactoryOperatorID],null as [dFactoryOperatorTime],              ");
                strSql.AppendLine("         null as [vcOperatorID], null as [dOperatorTime]          ");
                strSql.AppendLine("            from  (select * from [dbo].[TSPMaster] where vcInOut='1' and isnull(vcDelete, '') <> '1'           ");
                strSql.AppendLine("            AND (([dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))            ");
                strSql.AppendLine("             OR ([dFromTime]>=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23)))) a           ");
                strSql.AppendLine("            left join (select vcPackingPlant,vcPartNo,vcReceiver,vcSupplier_id,dUseStartDate,dUserEndDate from THeZiManage)b               ");
                strSql.AppendLine("            on a.vcPackingPlant = b.vcPackingPlant and a.vcPartId = b.vcPartNo and a.vcReceiver=b.vcReceiver and a.vcSupplierId=b.vcSupplier_id               ");
                strSql.AppendLine("            left join (           ");
                strSql.AppendLine("       	  SELECT  [vcPackingPlant] ,[vcPartId],[vcReceiver],[vcSupplierId],[dFromTime] ,[dToTime] ,[vcSupplierPlant],[vcOperatorType]                ");
                strSql.AppendLine("            FROM [SPPSdb].[dbo].[TSPMaster_SupplierPlant] where vcOperatorType='1'           ");
                strSql.AppendLine("       	 AND (([dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))            ");
                strSql.AppendLine("             OR ([dFromTime]>=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23)))          ");
                strSql.AppendLine("       	 ) c on a.vcPackingPlant=c.vcPackingPlant and a.vcPartId=c.vcPartId               ");
                strSql.AppendLine("            and a.vcReceiver=c.vcReceiver and a.vcSupplierId = c.vcSupplierId                ");
                strSql.AppendLine("         left join  (select vcPackingPlant, vcPartId, vcReceiver, vcSupplierId, vcSupplierPlant, dFromTime, dToTime,                 ");
                strSql.AppendLine("         iPackingQty, vcBoxType, iLength, iWidth, iHeight, iVolume, vcOperatorType, vcOperatorID, dOperatorTime                 ");
                strSql.AppendLine("          from TSPMaster_Box  where vcOperatorType='1'          ");
                strSql.AppendLine("          AND (([dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))            ");
                strSql.AppendLine("          OR ([dFromTime]>=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23)))          ");
                strSql.AppendLine("          ) d on a.vcPackingPlant=d.vcPackingPlant and a.vcPartId=d.vcPartId              ");
                strSql.AppendLine("          and a.vcReceiver=d.vcReceiver and  a.[vcSupplierId]=d.[vcSupplierId]            ");
                strSql.AppendLine("          LEFT JOIN           ");
                strSql.AppendLine("       	(SELECT *  FROM [TSPMaster_SufferIn] WHERE [vcOperatorType]='1'            ");
                strSql.AppendLine("       	AND (([dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))            ");
                strSql.AppendLine("       	OR ([dFromTime]>=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))))e          ");
                strSql.AppendLine("       	ON a.[vcPackingPlant]=e.[vcPackingPlant] AND a.[vcPartId]=e.[vcPartId] AND a.[vcReceiver]=e.[vcReceiver] AND a.[vcSupplierId]=e.[vcSupplierId]           ");
                strSql.AppendLine("       	LEFT JOIN           ");
                strSql.AppendLine("       	(select vcValue1 as [vcSupplierId],vcValue2 as vcSupplierPlant,vcValue3 as [dFromTime],vcValue4 as [dToTime],vcValue5 as vcOrderPlant from TOutCode where vcCodeId='C010' and vcIsColum='0'           ");
                strSql.AppendLine("       	and vcValue3<=CONVERT(VARCHAR(10),GETDATE(),23) AND vcValue4>=CONVERT(VARCHAR(10),GETDATE(),23))f           ");
                strSql.AppendLine("       	ON a.[vcSupplierId]=f.[vcSupplierId] AND c.vcSupplierPlant=f.vcSupplierPlant           ");
                strSql.AppendLine("            where b.vcPartNo is null           ");
                strSql.AppendLine("       	 and (ISNULL(c.vcSupplierPlant,'')='' OR ISNULL(d.iPackingQty,0)=0 OR ISNULL(e.vcSufferIn,'')='' OR ISNULL(f.vcOrderPlant,'')='')                 ");
                strSql.AppendLine("         union all             ");
                strSql.AppendLine("           select * from (             ");
                strSql.AppendLine("         	select [iAutoId], a.vcPackingPlant,a.vcReceiver, [dSynchronizationDate],  [vcState], [vcPartNo],             ");
                strSql.AppendLine("         	[dUseStartDate],[dUserEndDate], [vcPartName],                ");
                strSql.AppendLine("         	[vcCarType], [vcOEOrSP], [vcSupplier_id], [vcWorkArea], [dExpectDeliveryDate], [vcExpectIntake],               ");
                strSql.AppendLine("         	[vcIntake], [vcBoxMaxIntake], [vcBoxType], [vcLength], [vcWide], [vcHeight], [vcEmptyWeight],                ");
                strSql.AppendLine("         	[vcUnitNetWeight], [dSendDate], [dReplyDate], [dAdmitDate], [dWeaveDate], [vcMemo], vcImageRoutes,                ");
                strSql.AppendLine("         	[vcInserter], [vcInserterDate],[vcFactoryOperatorID], [dFactoryOperatorTime],               ");
                strSql.AppendLine("         	[vcOperatorID], [dOperatorTime] as vcAddFlag from [dbo].[THeZiManage] a                ");
                strSql.AppendLine("         	              ");
                strSql.AppendLine("         	) m             ");
                strSql.AppendLine("         	) n           ");
                strSql.AppendLine("       	left join (select vcValue,vcName from TCode where vcCodeId='C033') o on n.vcState = o.vcValue           ");
                strSql.AppendLine("       	left join (select vcValue,vcName from TCode where vcCodeId='C012') p on n.vcOEOrSP = p.vcValue           ");
                strSql.AppendLine("    ) T       ");
                strSql.AppendLine("    ) S order by s.vcWorkArea asc       ");
                strSql.AppendLine("           ");


                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetWorkAreaBySupplier( string vcSupplier_id)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("     select vcWorkArea as vcValue,vcWorkArea as vcName from(      ");
                strSql.AppendLine("     select distinct isnull( vcWorkArea,'无') as vcWorkArea from (      ");
                strSql.AppendLine("       select          ");
                strSql.AppendLine("        n.[iAutoId], n.vcPackingPlant,n.vcReceiver, n.[dSynchronizationDate], o.vcName as [vcState], n.[vcPartNo],            ");
                strSql.AppendLine("          	n.[dUseStartDate],n.[dUserEndDate], n.[vcPartName],               ");
                strSql.AppendLine("          	n.[vcCarType],p.vcName as [vcOEOrSP], n.[vcSupplier_id], n.[vcWorkArea], n.[dExpectDeliveryDate], n.[vcExpectIntake],              ");
                strSql.AppendLine("          	n.[vcIntake], n.[vcBoxMaxIntake], n.[vcBoxType], n.[vcLength], n.[vcWide], n.[vcHeight], n.[vcEmptyWeight],               ");
                strSql.AppendLine("          	n.[vcUnitNetWeight], n.[dSendDate], n.[dReplyDate], n.[dAdmitDate], n.[dWeaveDate], n.[vcMemo], n.vcImageRoutes,               ");
                strSql.AppendLine("          	n.[vcInserter], n.[vcInserterDate],n.[vcFactoryOperatorID], n.[dFactoryOperatorTime],              ");
                strSql.AppendLine("          	n.[vcOperatorID], n.[dOperatorTime],'0' as vcModFlag,'0' as vcAddFlag         ");
                strSql.AppendLine("        from (            ");
                strSql.AppendLine("           select null as iAutoId,  a.vcPackingPlant,a.vcReceiver, a.dSyncTime as dSynchronizationDate, '0' as vcState,a.vcPartId as vcPartNo,            ");
                strSql.AppendLine("           a.dFromTime as dUseStartDate,a.dToTime as dUserEndDate,a.vcPartENName as vcPartName,a.vcCarModel as vcCarType,            ");
                strSql.AppendLine("           a.vcOESP as vcOEOrSP,a.vcSupplierId as vcSupplier_id,c.vcSupplierPlant as vcWorkArea,            ");
                strSql.AppendLine("           null as dExpectDeliveryDate,'' as vcExpectIntake,'' as [vcIntake],'' as [vcBoxMaxIntake],'' as [vcBoxType],'' as [vcLength],'' as [vcWide],            ");
                strSql.AppendLine("           '' as [vcHeight],'' as [vcEmptyWeight],'' as [vcUnitNetWeight],null as [dSendDate],null as [dReplyDate],null as [dAdmitDate],null as [dWeaveDate],           ");
                strSql.AppendLine("           '' as [vcMemo],null as  [vcImageRoutes],'' as [vcInserter],null as [vcInserterDate],'' as [vcFactoryOperatorID],null as [dFactoryOperatorTime],             ");
                strSql.AppendLine("          null as [vcOperatorID], null as [dOperatorTime]         ");
                strSql.AppendLine("             from  (select * from [dbo].[TSPMaster] where vcInOut='1' and isnull(vcDelete, '') <> '1'          ");
                strSql.AppendLine("             AND (([dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))           ");
                strSql.AppendLine("              OR ([dFromTime]>=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23)))) a          ");
                strSql.AppendLine("             left join (select vcPackingPlant,vcPartNo,vcReceiver,vcSupplier_id,dUseStartDate,dUserEndDate from THeZiManage)b              ");
                strSql.AppendLine("             on a.vcPackingPlant = b.vcPackingPlant and a.vcPartId = b.vcPartNo and a.vcReceiver=b.vcReceiver and a.vcSupplierId=b.vcSupplier_id              ");
                strSql.AppendLine("             left join (          ");
                strSql.AppendLine("        	  SELECT  [vcPackingPlant] ,[vcPartId],[vcReceiver],[vcSupplierId],[dFromTime] ,[dToTime] ,[vcSupplierPlant],[vcOperatorType]               ");
                strSql.AppendLine("             FROM [SPPSdb].[dbo].[TSPMaster_SupplierPlant] where vcOperatorType='1'          ");
                strSql.AppendLine("        	 AND (([dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))           ");
                strSql.AppendLine("              OR ([dFromTime]>=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23)))         ");
                strSql.AppendLine("        	 ) c on a.vcPackingPlant=c.vcPackingPlant and a.vcPartId=c.vcPartId              ");
                strSql.AppendLine("             and a.vcReceiver=c.vcReceiver and a.vcSupplierId = c.vcSupplierId               ");
                strSql.AppendLine("          left join  (select vcPackingPlant, vcPartId, vcReceiver, vcSupplierId, vcSupplierPlant, dFromTime, dToTime,                ");
                strSql.AppendLine("          iPackingQty, vcBoxType, iLength, iWidth, iHeight, iVolume, vcOperatorType, vcOperatorID, dOperatorTime                ");
                strSql.AppendLine("           from TSPMaster_Box  where vcOperatorType='1'         ");
                strSql.AppendLine("           AND (([dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))           ");
                strSql.AppendLine("           OR ([dFromTime]>=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23)))         ");
                strSql.AppendLine("           ) d on a.vcPackingPlant=d.vcPackingPlant and a.vcPartId=d.vcPartId             ");
                strSql.AppendLine("           and a.vcReceiver=d.vcReceiver and  a.[vcSupplierId]=d.[vcSupplierId]           ");
                strSql.AppendLine("           LEFT JOIN          ");
                strSql.AppendLine("        	(SELECT *  FROM [TSPMaster_SufferIn] WHERE [vcOperatorType]='1'           ");
                strSql.AppendLine("        	AND (([dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))           ");
                strSql.AppendLine("        	OR ([dFromTime]>=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))))e         ");
                strSql.AppendLine("        	ON a.[vcPackingPlant]=e.[vcPackingPlant] AND a.[vcPartId]=e.[vcPartId] AND a.[vcReceiver]=e.[vcReceiver] AND a.[vcSupplierId]=e.[vcSupplierId]          ");
                strSql.AppendLine("        	LEFT JOIN          ");
                strSql.AppendLine("        	(select vcValue1 as [vcSupplierId],vcValue2 as vcSupplierPlant,vcValue3 as [dFromTime],vcValue4 as [dToTime],vcValue5 as vcOrderPlant from TOutCode where vcCodeId='C010' and vcIsColum='0'          ");
                strSql.AppendLine("        	and vcValue3<=CONVERT(VARCHAR(10),GETDATE(),23) AND vcValue4>=CONVERT(VARCHAR(10),GETDATE(),23))f          ");
                strSql.AppendLine("        	ON a.[vcSupplierId]=f.[vcSupplierId] AND c.vcSupplierPlant=f.vcSupplierPlant          ");
                strSql.AppendLine("             where b.vcPartNo is null          ");
                strSql.AppendLine("        	 and (ISNULL(c.vcSupplierPlant,'')='' OR ISNULL(d.iPackingQty,0)=0 OR ISNULL(e.vcSufferIn,'')='' OR ISNULL(f.vcOrderPlant,'')='')                ");
                strSql.AppendLine("          union all            ");
                strSql.AppendLine("            select * from (            ");
                strSql.AppendLine("          	select [iAutoId], a.vcPackingPlant,a.vcReceiver, [dSynchronizationDate],  [vcState], [vcPartNo],            ");
                strSql.AppendLine("          	[dUseStartDate],[dUserEndDate], [vcPartName],               ");
                strSql.AppendLine("          	[vcCarType], [vcOEOrSP], [vcSupplier_id], [vcWorkArea], [dExpectDeliveryDate], [vcExpectIntake],              ");
                strSql.AppendLine("          	[vcIntake], [vcBoxMaxIntake], [vcBoxType], [vcLength], [vcWide], [vcHeight], [vcEmptyWeight],               ");
                strSql.AppendLine("          	[vcUnitNetWeight], [dSendDate], [dReplyDate], [dAdmitDate], [dWeaveDate], [vcMemo], vcImageRoutes,               ");
                strSql.AppendLine("          	[vcInserter], [vcInserterDate],[vcFactoryOperatorID], [dFactoryOperatorTime],              ");
                strSql.AppendLine("          	[vcOperatorID], [dOperatorTime] as vcAddFlag from [dbo].[THeZiManage] a               ");
                strSql.AppendLine("          	             ");
                strSql.AppendLine("          	) m            ");
                strSql.AppendLine("          	) n          ");
                strSql.AppendLine("        	left join (select vcValue,vcName from TCode where vcCodeId='C033') o on n.vcState = o.vcValue          ");
                strSql.AppendLine("        	left join (select vcValue,vcName from TCode where vcCodeId='C012') p on n.vcOEOrSP = p.vcValue          ");
                strSql.AppendLine("     ) T where t.vcSupplier_id='"+ vcSupplier_id + "'      ");
                strSql.AppendLine("     ) S order by s.vcWorkArea      ");
                strSql.AppendLine("           ");
                

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 承认操作
        /// </summary>
        /// <param name="listInfoData"></param>
        /// <param name="userId"></param>
        public void admit(List<Dictionary<string, object>> listInfoData, string userId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();

                sql.Append("update [THeZiManage] set  vcState='4',dAdmitDate=GETDATE(), \n");
               
                sql.Append(" vcOperatorID='" + userId + "',dOperatorTime=GETDATE() where iAutoId in( \n");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (i != 0)
                        sql.Append(",");
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                    sql.Append(iAutoId);
                }
                sql.Append("  )   \r\n ");
                excute.ExcuteSqlWithStringOper(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 退回操作
        /// </summary>
        /// <param name="listInfoData"></param>
        /// <param name="userId"></param>
        public void returnHandle(List<Dictionary<string, object>> listInfoData, string userId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();

                sql.Append("update [THeZiManage] set  vcState='3', \n");

                sql.Append(" vcOperatorID='" + userId + "',dOperatorTime=GETDATE() where iAutoId in( \n");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (i != 0)
                        sql.Append(",");
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                    sql.Append(iAutoId);
                }
                sql.Append("  )   \r\n ");
                excute.ExcuteSqlWithStringOper(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 织入原单位
        /// </summary>
        /// <param name="listInfoData"></param>
        /// <param name="userId"></param>
        public void weaveHandle(List<Dictionary<string, object>> listInfoData, string userId, ref bool bReault, ref DataTable dtMessage)
        {
            try
            {
                
                for (var i=0; i<listInfoData.Count;i++)
                {
                    StringBuilder strSql = new StringBuilder();
                    StringBuilder strSqlZ = new StringBuilder();
                    StringBuilder strSqlZZ = new StringBuilder();
                    StringBuilder strSqlZGQ = new StringBuilder();
                    //[LinId], [vcPackingPlant], [vcPartId], [vcReceiver], [vcSupplierId], [vcSupplierPlant],
                    //[dFromTime], [dToTime], [iPackingQty], [vcBoxType], [iLength], [iWidth], [iHeight], 
                    //[iVolume], [vcOperatorType], [vcOperatorID], [dOperatorTime]
                    string vcPackingPlant = listInfoData[i]["vcPackingPlant"]==null? null : listInfoData[i]["vcPackingPlant"].ToString();
                    string vcReceiver = listInfoData[i]["vcReceiver"]==null?"":listInfoData[i]["vcReceiver"].ToString();
                    string vcPartId = listInfoData[i]["vcPartNo"] == null ? "" : listInfoData[i]["vcPartNo"].ToString();
                    string vcSupplierId = listInfoData[i]["vcSupplier_id"] == null ? null : listInfoData[i]["vcSupplier_id"].ToString();
                    string vcSupplierPlant = listInfoData[i]["vcWorkArea"] == null ? null : listInfoData[i]["vcWorkArea"].ToString();

                    string dFromTime = listInfoData[i]["dUseStartDate"] == null ? null : listInfoData[i]["dUseStartDate"].ToString();
                    string dToTime = listInfoData[i]["dUserEndDate"] == null ? null : listInfoData[i]["dUserEndDate"].ToString();
                    string iPackingQty = listInfoData[i]["vcIntake"] == null ? null : listInfoData[i]["vcIntake"].ToString();
                    string vcBoxType = listInfoData[i]["vcBoxType"] == null ? null : listInfoData[i]["vcBoxType"].ToString();
                    string iLength = listInfoData[i]["vcLength"] == null ? "0" : listInfoData[i]["vcLength"].ToString();
                    string iWidth = listInfoData[i]["vcWide"] == null ? "0" : listInfoData[i]["vcWide"].ToString();
                    string iHeight = listInfoData[i]["vcHeight"] == null ? "0" : listInfoData[i]["vcHeight"].ToString();
                    string iVolume = ((Convert.ToDecimal(iLength)/1000)* (Convert.ToDecimal(iWidth)/1000) * (Convert.ToDecimal(iHeight)/1000)).ToString();
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                    strSqlZ.AppendLine("  select * from [dbo].[TSPMaster] where vcPackingPlant='"+ vcPackingPlant + "' and vcPartId='" + vcPartId + "' and vcReceiver='" + vcReceiver + "' and vcSupplierId='" + vcSupplierId + "'   ");
                    strSqlZ.AppendLine("   select * from TSPMaster_Box where vcPackingPlant='" + vcPackingPlant + "' and vcReceiver='" + vcReceiver + "' and vcPartId='" + vcPartId + "' and vcSupplierId='" + vcSupplierId + "'  ");
                    strSqlZ.AppendLine("  select * from TSPMaster_SupplierPlant where vcPackingPlant='" + vcPackingPlant + "' and vcReceiver='" + vcReceiver + "' and vcPartId='" + vcPartId + "' and vcSupplierId='" + vcSupplierId + "'   ");
                    DataSet ds =  excute.ExcuteSqlWithSelectToDS(strSqlZ.ToString());
                    DataTable dtZ = ds.Tables[0];
                    DataTable dtZZ = ds.Tables[1];
                    DataTable dtZGQ = ds.Tables[2];
                    if (dtZ.Rows.Count == 0)
                    {
                        DataRow dataRow = dtMessage.NewRow(); 
                        dataRow["vcPartNo"] = vcPartId;
                        dataRow["vcMessage"] = "原单位表数据不存在,无法织入";
                        dtMessage.Rows.Add(dataRow);
                        bReault = false;
                    }
                    else
                    {
                        if (dtZZ.Rows.Count > 0|| dtZGQ.Rows.Count > 0)
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcPartNo"] = vcPartId;
                            dataRow["vcMessage"] = "字表数据表已经存在数据,无法织入，请手动织入";
                            dtMessage.Rows.Add(dataRow);
                            bReault = false;
                        }else
                        { 
                            strSql.AppendLine("  INSERT INTO [dbo].[TSPMaster_Box]   ");
                            strSql.AppendLine("             ([vcPackingPlant],[vcPartId]  ,[vcReceiver],[vcSupplierId]   ");
                            strSql.AppendLine("             ,[vcSupplierPlant],[dFromTime] ,[dToTime] ,[iPackingQty],[vcBoxType],[iLength]  ,[iWidth]   ");
                            strSql.AppendLine("            ,[iHeight]  ,[iVolume] ,[vcOperatorType] ,[vcOperatorID] ,[dOperatorTime]) values (  ");
                            strSql.AppendLine("  '" + vcPackingPlant + "','" + vcPartId + "','" + vcReceiver + "','" + vcSupplierId + "',   ");
                            strSql.AppendLine("  '" + vcSupplierPlant + "','" + dFromTime + "','" + dToTime + "','" + iPackingQty + "','" + vcBoxType + "','" + iLength + "','" + iWidth + "',   ");
                            strSql.AppendLine("   '" + iHeight + "','" + iVolume + "','1', '" + userId + "',getdate()); ");
                            strSql.AppendLine("  INSERT INTO [dbo].[TSPMaster_SupplierPlant]   ");
                            strSql.AppendLine("             ([vcPackingPlant],[vcPartId],[vcReceiver],[vcSupplierId]    ");
                            strSql.AppendLine("             ,[dFromTime]  ,[dToTime],[vcSupplierPlant]     ");
                            strSql.AppendLine("             ,[vcOperatorType] ,[vcOperatorID]  ,[dOperatorTime]) values (  ");
                            strSql.AppendLine("  '" + vcPackingPlant + "','" + vcPartId + "','" + vcReceiver + "','" + vcSupplierId + "',   ");
                            strSql.AppendLine("  '" + dFromTime + "','" + dToTime + "','" + vcSupplierPlant + "',   ");
                            strSql.AppendLine("  '1', '" + userId + "',getdate()); ");
                            strSql.AppendLine("    update [THeZiManage] set  vcState='5',dWeaveDate =GETDATE(), vcOperatorID='" + userId + "',dOperatorTime=GETDATE() where iAutoId = " + iAutoId + "  ; ");
                        }
                        if (strSql.Length > 0)
                        {
                            excute.ExcuteSqlWithStringOper(strSql.ToString());
                        }
                    }

                    //strSql.AppendLine("  declare @isExist int =0;   ");
                    //strSql.AppendLine("  select @isExist=COUNT(*) from TSPMaster_Box where vcPackingPlant='" + vcPackingPlant + "' and vcReceiver='" + vcReceiver + "' and vcPartId='" + vcPartId + "' and vcSupplierId='" + vcSupplierId + "' and vcSupplierPlant='" + vcSupplierPlant + "'   ");
                    //strSql.AppendLine("     ");
                    //strSql.AppendLine("  if @isExist>0   ");
                    //strSql.AppendLine("  begin   ");
                    //strSql.AppendLine("  INSERT INTO [dbo].[TSPMaster_Box]   ");
                    //strSql.AppendLine("             ([vcPackingPlant],[vcPartId]  ,[vcReceiver],[vcSupplierId]   ");
                    //strSql.AppendLine("             ,[vcSupplierPlant],[dFromTime] ,[dToTime] ,[iPackingQty],[vcBoxType],[iLength]  ,[iWidth]   ");
                    //strSql.AppendLine("            ,[iHeight]  ,[iVolume] ,[vcOperatorType] ,[vcOperatorID] ,[dOperatorTime]) values (  ");
                    //strSql.AppendLine("  '" + vcPackingPlant + "','" + vcPartId + "','" + vcReceiver + "','" + vcSupplierId + "',   ");
                    //strSql.AppendLine("  '" + vcSupplierPlant + "','" + dFromTime + "','" + dToTime + "','" + iPackingQty + "','" + vcBoxType + "','" + iLength + "','" + iWidth + "',   ");
                    //strSql.AppendLine("   '" + iHeight + "','" + iVolume + "','0', '"+ userId + "',getdate()); ");

                    //strSql.AppendLine("  end   ");
                    //strSql.AppendLine("  else   ");
                    //strSql.AppendLine("  begin   ");
                    //strSql.AppendLine("  INSERT INTO [dbo].[TSPMaster_Box]   ");
                    //strSql.AppendLine("             ([vcPackingPlant],[vcPartId]  ,[vcReceiver],[vcSupplierId]   ");
                    //strSql.AppendLine("             ,[vcSupplierPlant],[dFromTime] ,[dToTime] ,[iPackingQty],[vcBoxType],[iLength]  ,[iWidth]   ");
                    //strSql.AppendLine("            ,[iHeight]  ,[iVolume] ,[vcOperatorType] ,[vcOperatorID] ,[dOperatorTime]) values (  ");
                    //strSql.AppendLine("  '" + vcPackingPlant + "','" + vcPartId + "','" + vcReceiver + "','" + vcSupplierId + "',   ");
                    //strSql.AppendLine("  '" + vcSupplierPlant + "','" + dFromTime + "','" + dToTime + "','" + iPackingQty + "','" + vcBoxType + "','" + iLength + "','" + iWidth + "',   ");
                    //strSql.AppendLine("   '" + iHeight + "','" + iVolume + "','1', '" + userId + "',getdate()); ");
                    //strSql.AppendLine("  end ;  ");
                    //strSql.AppendLine("    update [THeZiManage] set  vcState='5',dWeaveDate =GETDATE(), vcOperatorID='" + userId + "',dOperatorTime=GETDATE() where iAutoId = "+ iAutoId + "  ; ");
                    
                }
                if (!bReault)
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 荷姿展开
        /// </summary>
        /// <param name="listInfoData"></param>
        /// <param name="dExpectDeliveryDate"></param>
        /// <param name="userId"></param>
        public void hZZK(List<Dictionary<string, object>> listInfoData, string dExpectDeliveryDate, string userId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();

                sql.Append("update [THeZiManage] set  \n");
                sql.Append(" dExpectDeliveryDate='" + Convert.ToDateTime(dExpectDeliveryDate) + "', \n");
                sql.Append(" vcState='1',dSendDate=GETDATE(), vcOperatorID='" + userId + "',dOperatorTime=GETDATE() where iAutoId in( \n");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (i != 0)
                        sql.Append(",");
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                    sql.Append(iAutoId);
                }
                sql.Append("  )   \r\n ");
                excute.ExcuteSqlWithStringOper(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 荷姿展开
        /// </summary>
        /// <param name="listInfoData"></param>
        /// <param name="dExpectDeliveryDate"></param>
        /// <param name="userId"></param>
        public void hZZK(DataTable dtNewSupplierand, string dExpectDeliveryDate, string userId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();

                sql.Append("update [THeZiManage] set  \n");
                sql.Append(" dExpectDeliveryDate='" + Convert.ToDateTime(dExpectDeliveryDate) + "', \n");
                sql.Append(" vcState='1',dSendDate=GETDATE(), vcOperatorID='" + userId + "',dOperatorTime=GETDATE() where iAutoId in( \n");
                for (int i = 0; i < dtNewSupplierand.Rows.Count; i++)
                {
                    if (i != 0)
                        sql.Append(",");
                    int iAutoId = Convert.ToInt32(dtNewSupplierand.Rows[i]["iAutoId"]);
                    sql.Append(iAutoId);
                }
                sql.Append("  )   \r\n ");
                excute.ExcuteSqlWithStringOper(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取未发送的数据
        /// </summary>
        public DataTable GetTaskNum()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("   select * from (     ");
                strSql.AppendLine("       select null as iAutoId,  a.vcPackingPlant,a.vcReceiver, a.dSyncTime as dSynchronizationDate, '0' as vcState,a.vcPartId as vcPartNo,       ");
                strSql.AppendLine("       a.dFromTime as dUseStartDate,a.dToTime as dUserEndDate,a.vcPartENName as vcPartName,a.vcCarModel as vcCarType,       ");
                strSql.AppendLine("       a.vcOESP as vcOEOrSP,a.vcSupplierId as vcSupplier_id,c.vcSupplierPlant as vcWorkArea,       ");
                strSql.AppendLine("       null as dExpectDeliveryDate,'' as vcExpectIntake,'' as [vcIntake],'' as [vcBoxMaxIntake],'' as [vcBoxType],'' as [vcLength],'' as [vcWide],       ");
                strSql.AppendLine("       '' as [vcHeight],'' as [vcEmptyWeight],'' as [vcUnitNetWeight],null as [dSendDate],null as [dReplyDate],null as [dAdmitDate],null as [dWeaveDate],       ");
                strSql.AppendLine("       '' as [vcMemo],null as  [vcImageRoutes],'' as [vcInserter],null as [vcInserterDate],'' as [vcFactoryOperatorID],null as [dFactoryOperatorTime],        ");
                strSql.AppendLine("      null as [vcOperatorID], null as [dOperatorTime]    ");
                strSql.AppendLine("       from  (select * from [dbo].[TSPMaster] where vcInOut='1' and isnull(vcDelete, '') <> '1'   ");
                strSql.AppendLine("       AND (([dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))    ");
                strSql.AppendLine("        OR ([dFromTime]>=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23)))) a   ");
                strSql.AppendLine("       left join (select vcPackingPlant,vcPartNo,vcReceiver,vcSupplier_id,dUseStartDate,dUserEndDate from THeZiManage)b       ");
                strSql.AppendLine("       on a.vcPackingPlant = b.vcPackingPlant and a.vcPartId = b.vcPartNo and a.vcReceiver=b.vcReceiver and a.vcSupplierId=b.vcSupplier_id       ");
                strSql.AppendLine("       left join (   ");
                strSql.AppendLine("   	  SELECT  [vcPackingPlant] ,[vcPartId],[vcReceiver],[vcSupplierId],[dFromTime] ,[dToTime] ,[vcSupplierPlant],[vcOperatorType]        ");
                strSql.AppendLine("       FROM [SPPSdb].[dbo].[TSPMaster_SupplierPlant] where vcOperatorType='1'   ");
                strSql.AppendLine("   	 AND (([dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))    ");
                strSql.AppendLine("        OR ([dFromTime]>=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23)))  ");
                strSql.AppendLine("   	 ) c on a.vcPackingPlant=c.vcPackingPlant and a.vcPartId=c.vcPartId       ");
                strSql.AppendLine("       and a.vcReceiver=c.vcReceiver and a.vcSupplierId = c.vcSupplierId        ");
                strSql.AppendLine("    left join  (select vcPackingPlant, vcPartId, vcReceiver, vcSupplierId, vcSupplierPlant, dFromTime, dToTime,         ");
                strSql.AppendLine("    iPackingQty, vcBoxType, iLength, iWidth, iHeight, iVolume, vcOperatorType, vcOperatorID, dOperatorTime         ");
                strSql.AppendLine("     from TSPMaster_Box  where vcOperatorType='1'  ");
                strSql.AppendLine("     AND (([dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))    ");
                strSql.AppendLine("     OR ([dFromTime]>=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23)))  ");
                strSql.AppendLine("     ) d on a.vcPackingPlant=d.vcPackingPlant and a.vcPartId=d.vcPartId      ");
                strSql.AppendLine("     and a.vcReceiver=d.vcReceiver and  a.[vcSupplierId]=d.[vcSupplierId]    ");
                strSql.AppendLine("     LEFT JOIN   ");
                strSql.AppendLine("   	(SELECT *  FROM [TSPMaster_SufferIn] WHERE [vcOperatorType]='1'    ");
                strSql.AppendLine("   	AND (([dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))    ");
                strSql.AppendLine("   	OR ([dFromTime]>=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))))e  ");
                strSql.AppendLine("   	ON a.[vcPackingPlant]=e.[vcPackingPlant] AND a.[vcPartId]=e.[vcPartId] AND a.[vcReceiver]=e.[vcReceiver] AND a.[vcSupplierId]=e.[vcSupplierId]   ");
                strSql.AppendLine("   	LEFT JOIN   ");
                strSql.AppendLine("   	(select vcValue1 as [vcSupplierId],vcValue2 as vcSupplierPlant,vcValue3 as [dFromTime],vcValue4 as [dToTime],vcValue5 as vcOrderPlant from TOutCode where vcCodeId='C010' and vcIsColum='0'   ");
                strSql.AppendLine("   	and vcValue3<=CONVERT(VARCHAR(10),GETDATE(),23) AND vcValue4>=CONVERT(VARCHAR(10),GETDATE(),23))f   ");
                strSql.AppendLine("   	ON a.[vcSupplierId]=f.[vcSupplierId] AND c.vcSupplierPlant=f.vcSupplierPlant   ");
                strSql.AppendLine("       where b.vcPartNo is null   ");
                strSql.AppendLine("   	 and (ISNULL(c.vcSupplierPlant,'')='' OR ISNULL(d.iPackingQty,0)=0 OR ISNULL(e.vcSufferIn,'')='' OR ISNULL(f.vcOrderPlant,'')='')         ");
                strSql.AppendLine("      union all       ");
                strSql.AppendLine("        select * from (       ");
                strSql.AppendLine("      	select [iAutoId], a.vcPackingPlant,a.vcReceiver, [dSynchronizationDate],  [vcState], [vcPartNo],       ");
                strSql.AppendLine("      	[dUseStartDate],[dUserEndDate], [vcPartName],          ");
                strSql.AppendLine("      	[vcCarType], [vcOEOrSP], [vcSupplier_id], [vcWorkArea], [dExpectDeliveryDate], [vcExpectIntake],         ");
                strSql.AppendLine("      	[vcIntake], [vcBoxMaxIntake], [vcBoxType], [vcLength], [vcWide], [vcHeight], [vcEmptyWeight],          ");
                strSql.AppendLine("      	[vcUnitNetWeight], [dSendDate], [dReplyDate], [dAdmitDate], [dWeaveDate], [vcMemo], vcImageRoutes,          ");
                strSql.AppendLine("      	[vcInserter], [vcInserterDate],[vcFactoryOperatorID], [dFactoryOperatorTime],         ");
                strSql.AppendLine("      	[vcOperatorID], [dOperatorTime] as vcAddFlag from [dbo].[THeZiManage] a          ");
                strSql.AppendLine("      	) m       ");
                strSql.AppendLine("      	) n     ");
                strSql.AppendLine("    	left join (select vcValue,vcName from TCode where vcCodeId='C033') o on n.vcState = o.vcValue     ");
                strSql.AppendLine("    	where n.vcState='0'      ");
                /*strSql.AppendLine("  select * from (   ");
                strSql.AppendLine("     select null as iAutoId,  a.vcPackingPlant,a.vcReceiver, a.dSyncTime as dSynchronizationDate, '0' as vcState,a.vcPartId as vcPartNo,     ");
                strSql.AppendLine("     a.dFromTime as dUseStartDate,a.dToTime as dUserEndDate,a.vcPartENName as vcPartName,a.vcCarModel as vcCarType,     ");
                strSql.AppendLine("     a.vcOESP as vcOEOrSP,a.vcSupplierId as vcSupplier_id,c.vcSupplierPlant as vcWorkArea,     ");
                strSql.AppendLine("     '' as dExpectDeliveryDate,'' as vcExpectIntake,'' as [vcIntake],'' as [vcBoxMaxIntake],'' as [vcBoxType],'' as [vcLength],'' as [vcWide],     ");
                strSql.AppendLine("     '' as [vcHeight],'' as [vcEmptyWeight],'' as [vcUnitNetWeight],null as [dSendDate],null as [dReplyDate],null as [dAdmitDate],null as [dWeaveDate],     ");
                strSql.AppendLine("     '' as [vcMemo],null as  [vcImageRoutes],'' as [vcInserter],null as [vcInserterDate],'' as [vcFactoryOperatorID],null as [dFactoryOperatorTime],      ");
                strSql.AppendLine("    null as [vcOperatorID], null as [dOperatorTime]  ");
                strSql.AppendLine("     from  (select * from [dbo].[TSPMaster] where vcInOut='1') a     ");
                strSql.AppendLine("     left join (select vcPackingPlant,vcPartNo,vcReceiver,vcSupplier_id,dUseStartDate,dUserEndDate from THeZiManage)b     ");
                strSql.AppendLine("     on a.vcPackingPlant = b.vcPackingPlant and a.vcPartId = b.vcPartNo and a.vcReceiver=b.vcReceiver and a.vcSupplierId=b.vcSupplier_id     ");
                strSql.AppendLine("     left join (  SELECT  [vcPackingPlant] ,[vcPartId],[vcReceiver],[vcSupplierId],[dFromTime] ,[dToTime] ,[vcSupplierPlant],[vcOperatorType]      ");
                strSql.AppendLine("     FROM [SPPSdb].[dbo].[TSPMaster_SupplierPlant] where vcOperatorType='1' ) c on a.vcPackingPlant=c.vcPackingPlant and a.vcPartId=c.vcPartId     ");
                strSql.AppendLine("     and a.vcReceiver=c.vcReceiver and a.dFromTime = c.dFromTime and a.dToTime=c.dToTime      ");
                strSql.AppendLine("  left join  (select vcPackingPlant, vcPartId, vcReceiver, vcSupplierId, vcSupplierPlant, dFromTime, dToTime,       ");
                strSql.AppendLine("  iPackingQty, vcBoxType, iLength, iWidth, iHeight, iVolume, vcOperatorType, vcOperatorID, dOperatorTime       ");
                strSql.AppendLine("   from TSPMaster_Box  where vcOperatorType='1') d on a.vcPackingPlant=d.vcPackingPlant and a.vcPartId=d.vcPartId    ");
                strSql.AppendLine("   and a.vcReceiver=d.vcReceiver and a.dFromTime = d.dFromTime and a.dToTime=d.dToTime      ");
                strSql.AppendLine("     where b.vcPartNo is null and d.iPackingQty is null       ");
                strSql.AppendLine("    union all     ");
                strSql.AppendLine("      select * from (     ");
                strSql.AppendLine("    	select [iAutoId], a.vcPackingPlant,a.vcReceiver, [dSynchronizationDate],  [vcState], [vcPartNo],     ");
                strSql.AppendLine("    	[dUseStartDate],[dUserEndDate], [vcPartName],        ");
                strSql.AppendLine("    	[vcCarType], [vcOEOrSP], [vcSupplier_id], [vcWorkArea], [dExpectDeliveryDate], [vcExpectIntake],       ");
                strSql.AppendLine("    	[vcIntake], [vcBoxMaxIntake], [vcBoxType], [vcLength], [vcWide], [vcHeight], [vcEmptyWeight],        ");
                strSql.AppendLine("    	[vcUnitNetWeight], [dSendDate], [dReplyDate], [dAdmitDate], [dWeaveDate], [vcMemo], vcImageRoutes,        ");
                strSql.AppendLine("    	[vcInserter], [vcInserterDate],[vcFactoryOperatorID], [dFactoryOperatorTime],       ");
                strSql.AppendLine("    	[vcOperatorID], [dOperatorTime] as vcAddFlag from [dbo].[THeZiManage] a        ");
                strSql.AppendLine("    	) m     ");
                strSql.AppendLine("    	) n   ");
                strSql.AppendLine("  	left join (select vcValue,vcName from TCode where vcCodeId='C033') o on n.vcState = o.vcValue   ");
                strSql.AppendLine("  	where n.vcState='0'    ");*/

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取未待回复 含退回
        /// </summary>
        public DataTable GetTaskNum1()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("  select * from [THeZiManage] where vcState in ('1')  ");
               
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="listInfoData"></param>
        /// <param name="userId"></param>
        /// <param name="strErrorPartId"></param>
        public void Save(List<Dictionary<string, object>> listInfoData, string userId, ref string strErrorPartId)
        {
            try
            {
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑

                    StringBuilder strSql = new StringBuilder();
                    string vcPackingPlant = listInfoData[i]["vcPackingPlant"].ToString();
                    string vcReceiver = listInfoData[i]["vcReceiver"].ToString();
                    //string dSynchronizationDate = listInfoData[i]["dSynchronizationDate"].ToString();
                    string vcPartNo = listInfoData[i]["vcPartNo"].ToString();
                    string vcState = listInfoData[i]["vcState"].ToString();
                    string dUseStartDate = listInfoData[i]["dUseStartDate"] == null ? null : listInfoData[i]["dUseStartDate"].ToString();
                    string dUserEndDate = listInfoData[i]["dUserEndDate"] == null ? null : listInfoData[i]["dUserEndDate"].ToString();
                    string vcPartName = listInfoData[i]["vcPartName"] == null ? null : listInfoData[i]["vcPartName"].ToString();
                    string vcCarType = listInfoData[i]["vcCarType"]==null?null: listInfoData[i]["vcCarType"].ToString();
                    string vcOEOrSP = listInfoData[i]["vcOEOrSP"] == null ?null: listInfoData[i]["vcOEOrSP"].ToString();
                    if (vcOEOrSP == "×")
                    {
                        vcOEOrSP = "1";
                    }
                    else if (vcOEOrSP == "⭕" || vcOEOrSP == "○")
                    {
                        vcOEOrSP = "0";
                    }
                    else
                    { }
                    string vcSupplier_id = listInfoData[i]["vcSupplier_id"] == null ? null : listInfoData[i]["vcSupplier_id"].ToString();
                    string vcWorkArea = listInfoData[i]["vcWorkArea"] == null ? null : listInfoData[i]["vcWorkArea"].ToString();
                    string dExpectDeliveryDate = listInfoData[i]["dExpectDeliveryDate"] == null ? null : listInfoData[i]["dExpectDeliveryDate"].ToString();
                    string vcExpectIntake = listInfoData[i]["vcExpectIntake"] == null ? null : listInfoData[i]["vcExpectIntake"].ToString();
                    //[vcIntake]  ,[vcBoxMaxIntake]  ,[vcBoxType] ,[vcLength] ,[vcWide] ,[vcHeight]  ,[vcEmptyWeight]  ,[vcUnitNetWeight]
                    string vcIntake = listInfoData[i]["vcIntake"] == null ? null : listInfoData[i]["vcIntake"].ToString();
                    string vcBoxMaxIntake = listInfoData[i]["vcBoxMaxIntake"] == null ? null : listInfoData[i]["vcBoxMaxIntake"].ToString();
                    string vcBoxType = listInfoData[i]["vcBoxType"] == null ? null : listInfoData[i]["vcBoxType"].ToString();
                    string vcLength = listInfoData[i]["vcLength"] == null ? null : listInfoData[i]["vcLength"].ToString();
                    string vcWide = listInfoData[i]["vcWide"] == null ? null : listInfoData[i]["vcWide"].ToString();
                    string vcHeight = listInfoData[i]["vcHeight"] == null ? null : listInfoData[i]["vcHeight"].ToString();
                    string vcEmptyWeight = listInfoData[i]["vcEmptyWeight"] == null ? null : listInfoData[i]["vcEmptyWeight"].ToString();
                    string vcUnitNetWeight = listInfoData[i]["vcUnitNetWeight"] == null ? null : listInfoData[i]["vcUnitNetWeight"].ToString();
                    string vcMemo = listInfoData[i]["vcMemo"] == null ? null : listInfoData[i]["vcMemo"].ToString();
                    if (bAddFlag == true)
                    {//新增
                        strSql.AppendLine("   INSERT INTO [dbo].[THeZiManage]   ");
                        strSql.AppendLine("         ([vcPackingPlant] ,[vcReceiver],[vcState] ,[vcPartNo] ,[dUseStartDate] ,[dUserEndDate]   ");
                        strSql.AppendLine("         ,[vcPartName] ,[vcCarType] ,[vcOEOrSP] ,[vcSupplier_id]  ,[vcWorkArea]  ,[dExpectDeliveryDate]  ,[vcExpectIntake]   ");
                        strSql.AppendLine("          ,[vcIntake]  ,[vcBoxMaxIntake]  ,[vcBoxType] ,[vcLength] ,[vcWide] ,[vcHeight]  ,[vcEmptyWeight]  ,[vcUnitNetWeight]   ");
                        strSql.AppendLine("          ,[vcMemo]    ");
                        strSql.AppendLine("         ,[vcInserter]  ,[vcInserterDate] ,[vcOperatorID] ,[dOperatorTime])  values (    ");
                        strSql.AppendLine("   '" + vcPackingPlant + "','" + vcReceiver + "'," + getSqlValue(listInfoData[i]["vcState"], true) + ",'" + vcPartNo + "'," + getSqlValue(listInfoData[i]["dUseStartDate"], true) + "," + getSqlValue(listInfoData[i]["dUserEndDate"], true) + ",   ");
                        strSql.AppendLine("   " + getSqlValue(listInfoData[i]["vcPartName"], true) + "," + getSqlValue(listInfoData[i]["vcCarType"], true) + "," + getSqlValue(vcOEOrSP, true) + "," + getSqlValue(listInfoData[i]["vcSupplier_id"], true) + "," + getSqlValue(listInfoData[i]["vcWorkArea"], true) + " ," + getSqlValue(listInfoData[i]["dExpectDeliveryDate"], true) + " ," + getSqlValue(listInfoData[i]["vcExpectIntake"], true) + ",   ");
                        strSql.AppendLine("   " + getSqlValue(listInfoData[i]["vcIntake"], true) + "," + getSqlValue(listInfoData[i]["vcBoxMaxIntake"], true) + "," + getSqlValue(listInfoData[i]["vcBoxType"], true) + "," + getSqlValue(listInfoData[i]["vcLength"], true) + "," + getSqlValue(listInfoData[i]["vcWide"], true) + " ," + getSqlValue(listInfoData[i]["vcHeight"], true) + " ," + getSqlValue(listInfoData[i]["vcEmptyWeight"], true) + " ," + getSqlValue(listInfoData[i]["vcUnitNetWeight"], true) + ",    ");
                        strSql.AppendLine("   " + getSqlValue(listInfoData[i]["vcMemo"], true) + " ,   ");
                        strSql.AppendLine("   '" + userId + "',getdate(), '" + userId + "',getdate());   ");
                        excute.ExcuteSqlWithStringOper(strSql.ToString());
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                      
                       
                        //string vcWorkArea = listInfoData[i]["vcWorkArea"].ToString();

                        strSql.AppendLine("  declare @isExist int =0;   ");
                        strSql.AppendLine("  select @isExist=COUNT(*) from THeZiManage where vcPackingPlant='" + vcPackingPlant + "' and vcReceiver='" + vcReceiver + "' and vcPartNo='" + vcPartNo + "' and vcSupplier_id='" + vcSupplier_id + "' and dUseStartDate='" + dUseStartDate + "' and dUserEndDate='" + dUserEndDate + "'  ");
                        strSql.AppendLine("     ");
                        strSql.AppendLine("  if @isExist>0   ");
                        strSql.AppendLine("  begin   ");
                        strSql.AppendLine("   UPDATE [dbo].[THeZiManage]   ");
                        strSql.AppendLine("      SET [vcExpectIntake] = " + getSqlValue(listInfoData[i]["vcExpectIntake"], false) + "   ");
                        strSql.AppendLine("         ,[vcWorkArea] = " + getSqlValue(vcWorkArea, true) + "   ");
                        strSql.AppendLine("         ,[dExpectDeliveryDate] = " + getSqlValue(listInfoData[i]["dExpectDeliveryDate"], true) + "   ");
                        strSql.AppendLine("         ,[vcIntake] = " + getSqlValue(listInfoData[i]["vcIntake"], true) + "   ");
                        strSql.AppendLine("         ,[vcBoxMaxIntake] = " + getSqlValue(listInfoData[i]["vcBoxMaxIntake"], true) + "   ");
                        strSql.AppendLine("         ,[vcBoxType] = " + getSqlValue(listInfoData[i]["vcBoxType"], true) + "   ");
                        strSql.AppendLine("         ,[vcLength] = " + getSqlValue(listInfoData[i]["vcLength"], true) + "   ");
                        strSql.AppendLine("         ,[vcWide] = " + getSqlValue(listInfoData[i]["vcWide"], true) + "   ");
                        strSql.AppendLine("         ,[vcHeight] = " + getSqlValue(listInfoData[i]["vcHeight"], true) + "   ");
                        strSql.AppendLine("         ,[vcEmptyWeight] = " + getSqlValue(listInfoData[i]["vcEmptyWeight"], true) + "   ");
                        strSql.AppendLine("         ,[vcUnitNetWeight] = " + getSqlValue(listInfoData[i]["vcUnitNetWeight"], true) + "   ");
                        strSql.AppendLine("         ,[vcMemo] =  " + getSqlValue(listInfoData[i]["vcMemo"], false) + "   ");
                        strSql.AppendLine("  ,vcOperatorID='" + userId + "',dOperatorTime=GETDATE() ");
                        strSql.AppendLine("  where   vcPackingPlant='" + vcPackingPlant + "' and vcReceiver='" + vcReceiver + "' and vcPartNo='" + vcPartNo + "' and vcSupplier_id='" + vcSupplier_id + "' and dUseStartDate='" + dUseStartDate + "' and dUserEndDate='" + dUserEndDate + "'  ;  \n");
                        strSql.AppendLine("  end   ");
                        strSql.AppendLine("  else   ");
                        strSql.AppendLine("  begin   ");
                        strSql.AppendLine("   INSERT INTO [dbo].[THeZiManage]   ");
                        strSql.AppendLine("         ([vcPackingPlant] ,[vcReceiver],[dSynchronizationDate],[vcState] ,[vcPartNo] ,[dUseStartDate] ,[dUserEndDate]   ");
                        strSql.AppendLine("         ,[vcPartName] ,[vcCarType] ,[vcOEOrSP] ,[vcSupplier_id]  ,[vcWorkArea]  ,[dExpectDeliveryDate]  ,[vcExpectIntake]   ");
                        strSql.AppendLine("          ,[vcIntake]  ,[vcBoxMaxIntake]  ,[vcBoxType] ,[vcLength] ,[vcWide] ,[vcHeight]  ,[vcEmptyWeight]  ,[vcUnitNetWeight]   ");
                        strSql.AppendLine("          ,[dSendDate] ,[dReplyDate]  ,[dAdmitDate],[dWeaveDate] ,[vcMemo]  ,[vcImageRoutes]   ");
                        strSql.AppendLine("         ,[vcInserter]  ,[vcInserterDate] ,[vcFactoryOperatorID] ,[dFactoryOperatorTime]  ,[vcOperatorID] ,[dOperatorTime])  values (    ");
                        strSql.AppendLine("   '" + vcPackingPlant + "','" + vcReceiver + "'," + getSqlValue(listInfoData[i]["dSynchronizationDate"], true) + " ,'0','" + vcPartNo + "'," + getSqlValue(listInfoData[i]["dUseStartDate"], true) + "," + getSqlValue(listInfoData[i]["dUserEndDate"], true) + ",   ");
                        strSql.AppendLine("   " + getSqlValue(listInfoData[i]["vcPartName"], true) + "," + getSqlValue(listInfoData[i]["vcCarType"], true) + "," + getSqlValue(vcOEOrSP, true) + "," + getSqlValue(listInfoData[i]["vcSupplier_id"], true) + "," + getSqlValue(listInfoData[i]["vcWorkArea"], true) + " ," + getSqlValue(listInfoData[i]["dExpectDeliveryDate"], true) + " ," + getSqlValue(listInfoData[i]["vcExpectIntake"], true) + ",   ");
                        strSql.AppendLine("   " + getSqlValue(listInfoData[i]["vcIntake"], true) + "," + getSqlValue(listInfoData[i]["vcBoxMaxIntake"], true) + "," + getSqlValue(listInfoData[i]["vcBoxType"], true) + "," + getSqlValue(listInfoData[i]["vcLength"], true) + "," + getSqlValue(listInfoData[i]["vcWide"], true) + " ," + getSqlValue(listInfoData[i]["vcHeight"], true) + " ," + getSqlValue(listInfoData[i]["vcEmptyWeight"], true) + " ," + getSqlValue(listInfoData[i]["vcUnitNetWeight"], true) + ",    ");
                        strSql.AppendLine("   " + getSqlValue(listInfoData[i]["dSendDate"], true) + "," + getSqlValue(listInfoData[i]["dReplyDate"], true) + "," + getSqlValue(listInfoData[i]["dAdmitDate"], true) + "," + getSqlValue(listInfoData[i]["dWeaveDate"], true) + "," + getSqlValue(listInfoData[i]["vcMemo"], true) + " ," + getSqlValue(listInfoData[i]["vcImageRoutes"], true) + " ,   ");
                        strSql.AppendLine("   '" + userId + "',getdate()," + getSqlValue(listInfoData[i]["vcFactoryOperatorID"], true) + "," + getSqlValue(listInfoData[i]["dFactoryOperatorTime"], true) + ", '" + userId + "',getdate());   ");
                        strSql.AppendLine("  end ;  ");
                        strSql.AppendLine("     ");

                        excute.ExcuteSqlWithStringOper(strSql.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="dtdel"></param>
        /// <param name="userId"></param>
        public void Del(List<Dictionary<string, object>> listInfoData, string userId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("  delete [THeZiManage] where iAutoId in(   \r\n ");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (i != 0)
                        sql.Append(",");
                    int iAutoId = listInfoData[i]["iAutoId"] == null ? 0 : Convert.ToInt32(listInfoData[i]["iAutoId"]);
                    sql.Append(iAutoId);
                }
                sql.Append("  )   \r\n ");
                excute.ExcuteSqlWithStringOper(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 一括赋予 更改要望收容数
        /// </summary>
        /// <returns></returns>
        public void allInstall(List<Dictionary<string, object>> listInfoData,  string vcExpectIntake, string userId)
        {
            try
            {

                for (var i = 0; i < listInfoData.Count; i++)
                {
                    StringBuilder strSql = new StringBuilder();
                    string vcPackingPlant = listInfoData[i]["vcPackingPlant"]==null?null: listInfoData[i]["vcPackingPlant"].ToString();
                    string vcReceiver = listInfoData[i]["vcReceiver"] == null ? null : listInfoData[i]["vcReceiver"].ToString();
                    string dSynchronizationDate = listInfoData[i]["dSynchronizationDate"] == null ? null : listInfoData[i]["dSynchronizationDate"].ToString();
                    string vcPartNo = listInfoData[i]["vcPartNo"] == null ? null : listInfoData[i]["vcPartNo"].ToString();
                    
                    string dUseStartDate = listInfoData[i]["dUseStartDate"] == null ? null : listInfoData[i]["dUseStartDate"].ToString();
                    string dUserEndDate = listInfoData[i]["dUserEndDate"] == null ? null : listInfoData[i]["dUserEndDate"].ToString();
                    string vcPartName = listInfoData[i]["vcPartName"] == null ? null : listInfoData[i]["vcPartName"].ToString();
                    //string vcCarType = listInfoData[i]["vcCarType"].ToString();
                    string vcOEOrSP = listInfoData[i]["vcOEOrSP"] == null ? null : listInfoData[i]["vcOEOrSP"].ToString();
                    if (vcOEOrSP == "×")
                    {
                        vcOEOrSP = "1";
                    }
                    else if (vcOEOrSP == "⭕" || vcOEOrSP == "○")
                    {
                        vcOEOrSP = "0";
                    }
                    else
                    { }
                    string vcSupplier_id = listInfoData[i]["vcSupplier_id"] == null ? null : listInfoData[i]["vcSupplier_id"].ToString();
                    //string vcWorkArea = listInfoData[i]["vcWorkArea"].ToString();

                    strSql.AppendLine("  declare @isExist int =0;   ");
                    strSql.AppendLine("  select @isExist=COUNT(*) from THeZiManage where vcPackingPlant='" + vcPackingPlant + "' and vcReceiver='" + vcReceiver + "' and vcPartNo='" + vcPartNo + "' and vcSupplier_id='" + vcSupplier_id + "' and dUseStartDate='" + dUseStartDate + "' and dUserEndDate='" + dUserEndDate + "'  ");
                    strSql.AppendLine("     ");
                    strSql.AppendLine("  if @isExist>0   ");
                    strSql.AppendLine("  begin   ");
                    strSql.AppendLine("  update [dbo].[THeZiManage]   ");
                    strSql.AppendLine("      set   vcExpectIntake='"+ vcExpectIntake + "', vcOperatorID='"+ userId + "',dOperatorTime=getdate() where   vcPackingPlant='" + vcPackingPlant + "' and vcReceiver='" + vcReceiver + "' and vcPartNo='" + vcPartNo + "' and vcSupplier_id='" + vcSupplier_id + "'   ");
                    strSql.AppendLine("  end   ");
                    strSql.AppendLine("  else   ");
                    strSql.AppendLine("  begin   ");
                    strSql.AppendLine("   INSERT INTO [dbo].[THeZiManage]   ");
                    strSql.AppendLine("         ([vcPackingPlant] ,[vcReceiver],[dSynchronizationDate],[vcState] ,[vcPartNo] ,[dUseStartDate] ,[dUserEndDate]   ");
                    strSql.AppendLine("         ,[vcPartName] ,[vcCarType] ,[vcOEOrSP] ,[vcSupplier_id]  ,[vcWorkArea]  ,[dExpectDeliveryDate]  ,[vcExpectIntake]   ");
                    strSql.AppendLine("          ,[vcIntake]  ,[vcBoxMaxIntake]  ,[vcBoxType] ,[vcLength] ,[vcWide] ,[vcHeight]  ,[vcEmptyWeight]  ,[vcUnitNetWeight]   ");
                    strSql.AppendLine("          ,[dSendDate] ,[dReplyDate]  ,[dAdmitDate],[dWeaveDate] ,[vcMemo]  ,[vcImageRoutes]   ");
                    strSql.AppendLine("         ,[vcInserter]  ,[vcInserterDate] ,[vcFactoryOperatorID] ,[dFactoryOperatorTime]  ,[vcOperatorID] ,[dOperatorTime])  values (    ");
                    strSql.AppendLine("   '" + vcPackingPlant + "','" + vcReceiver + "'," + getSqlValue(listInfoData[i]["dSynchronizationDate"], true) + " ,'0','" + vcPartNo + "'," + getSqlValue(listInfoData[i]["dUseStartDate"], true) + "," + getSqlValue(listInfoData[i]["dUserEndDate"], true) + ",   ");
                    strSql.AppendLine("   " + getSqlValue(listInfoData[i]["vcPartName"], true) + "," + getSqlValue(listInfoData[i]["vcCarType"], true) + "," + getSqlValue(vcOEOrSP, true) + "," + getSqlValue(listInfoData[i]["vcSupplier_id"], true) + "," + getSqlValue(listInfoData[i]["vcWorkArea"], true) + " ," + getSqlValue(listInfoData[i]["dExpectDeliveryDate"], true) + " ,'" + vcExpectIntake + "',   ");
                    strSql.AppendLine("   " + getSqlValue(listInfoData[i]["vcIntake"], true) + "," + getSqlValue(listInfoData[i]["vcBoxMaxIntake"], true) + "," + getSqlValue(listInfoData[i]["vcBoxType"], true) + "," + getSqlValue(listInfoData[i]["vcLength"], true) + "," + getSqlValue(listInfoData[i]["vcWide"], true) + " ," + getSqlValue(listInfoData[i]["vcHeight"], true) + " ," + getSqlValue(listInfoData[i]["vcEmptyWeight"], true) + " ," + getSqlValue(listInfoData[i]["vcUnitNetWeight"], true) + ",    ");
                    strSql.AppendLine("   " + getSqlValue(listInfoData[i]["dSendDate"], true) + "," + getSqlValue(listInfoData[i]["dReplyDate"], true) + "," + getSqlValue(listInfoData[i]["dAdmitDate"], true) + "," + getSqlValue(listInfoData[i]["dWeaveDate"], true) + "," + getSqlValue(listInfoData[i]["vcMemo"], true) + " ," + getSqlValue(listInfoData[i]["vcImageRoutes"], true) + " ,   ");
                    strSql.AppendLine("   '" + userId + "',getdate()," + getSqlValue(listInfoData[i]["vcFactoryOperatorID"], true) + "," + getSqlValue(listInfoData[i]["dFactoryOperatorTime"], true) + ", '" + userId + "',getdate());   ");
                    strSql.AppendLine("  end ;  ");
                    strSql.AppendLine("     ");

                    excute.ExcuteSqlWithStringOper(strSql.ToString());
                }

                //StringBuilder sql = new StringBuilder();

                //sql.Append("update [THeZiManage] set  \n");
                
                //if (vcIntake.Length > 0)
                //{
                //    sql.Append(" vcIntake='" + vcIntake + "', \n");
                //}
                //sql.Append(" vcOperatorID='" + userId + "',dOperatorTime=GETDATE() where iAutoId in( \n");
                //for (int i = 0; i < listInfoData.Count; i++)
                //{
                //    if (i != 0)
                //        sql.Append(",");
                //    int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                //    sql.Append(iAutoId);
                //}
                //sql.Append("  )   \r\n ");
                //excute.ExcuteSqlWithStringOper(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 导入 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="strUserId"></param>
        public void importSave(DataTable dt, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("DELETE FROM [dbo].[THeZiManageImportTmp] where  vcOperatorID='" + strUserId + "' \n");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string vcType = dt.Rows[i]["vcType"] == System.DBNull.Value ? "" : dt.Rows[i]["vcType"].ToString().Trim();
                    if (vcType == "新增")
                    {
                        vcType = "1";
                    }
                    else if (vcType == "删除")
                    {
                        vcType = "2";
                    }
                    else if (vcType == "修改")
                    {
                        vcType = "3";
                    }

                    string vcPackingPlant = dt.Rows[i]["vcPackingPlant"] == System.DBNull.Value ? "" : dt.Rows[i]["vcPackingPlant"].ToString().Trim();
                    string vcReceiver = dt.Rows[i]["vcReceiver"] == System.DBNull.Value ? "" : dt.Rows[i]["vcReceiver"].ToString().Trim();
                    string dSynchronizationDate = dt.Rows[i]["dSynchronizationDate"] == System.DBNull.Value ? "" : dt.Rows[i]["dSynchronizationDate"].ToString().Trim();
                    string vcState = dt.Rows[i]["vcState"] == System.DBNull.Value ? "" : dt.Rows[i]["vcState"].ToString().Trim();
                    #region

                    if (vcState == "未发送")
                    {
                        vcState = "0";
                    }
                    else if (vcState == "待回复")
                    {
                        vcState = "1";
                    }
                    else if (vcState == "已回复")
                    {
                        vcState = "2";
                    }
                    else if (vcState == "退回")
                    {
                        vcState = "3";
                    }
                    else if (vcState == "已承认")
                    {
                        vcState = "4";
                    }
                    else if (vcState == "已织入")
                    {
                        vcState = "5";
                    }
                    else
                    {
                        vcState = "0";
                    }
                    #endregion
                    string vcPartNo = dt.Rows[i]["vcPartNo"] == System.DBNull.Value ? "" : dt.Rows[i]["vcPartNo"].ToString().Trim();
                    string dUseStartDate = dt.Rows[i]["dUseStartDate"] == System.DBNull.Value ? "" : dt.Rows[i]["dUseStartDate"].ToString().Trim();
                    //string dUserEndDate = dt.Rows[i]["dUserEndDate"] == System.DBNull.Value ? "" : dt.Rows[i]["dUserEndDate"].ToString();
                    string vcPartName = dt.Rows[i]["vcPartName"] == System.DBNull.Value ? "" : dt.Rows[i]["vcPartName"].ToString().Trim();
                    string vcCarType = dt.Rows[i]["vcCarType"] == System.DBNull.Value ? "" : dt.Rows[i]["vcCarType"].ToString().Trim();
                    string vcOEOrSP = dt.Rows[i]["vcOEOrSP"] == System.DBNull.Value ? "" : dt.Rows[i]["vcOEOrSP"].ToString().Trim();
                    if (vcOEOrSP == "×")
                    {
                        vcOEOrSP = "1";
                    }
                    else if (vcOEOrSP == "⭕" || vcOEOrSP == "○")
                    {
                        vcOEOrSP = "0";
                    }
                    else
                    { }
                    string vcSupplier_id = dt.Rows[i]["vcSupplier_id"] == System.DBNull.Value ? "" : dt.Rows[i]["vcSupplier_id"].ToString().Trim();
                    string vcWorkArea = dt.Rows[i]["vcWorkArea"] == System.DBNull.Value ? "" : dt.Rows[i]["vcWorkArea"].ToString();
                    string dExpectDeliveryDate = dt.Rows[i]["dExpectDeliveryDate"] == System.DBNull.Value ? "" : dt.Rows[i]["dExpectDeliveryDate"].ToString().Trim();
                    string vcExpectIntake = dt.Rows[i]["vcExpectIntake"] == System.DBNull.Value ? "" : dt.Rows[i]["vcExpectIntake"].ToString().Trim();
                    string vcIntake = dt.Rows[i]["vcIntake"] == System.DBNull.Value ? "" : dt.Rows[i]["vcIntake"].ToString().Trim();
                    string vcBoxMaxIntake = dt.Rows[i]["vcBoxMaxIntake"] == System.DBNull.Value ? "" : dt.Rows[i]["vcBoxMaxIntake"].ToString().Trim();
                    string vcBoxType = dt.Rows[i]["vcBoxType"] == System.DBNull.Value ? "" : dt.Rows[i]["vcBoxType"].ToString().Trim();
                    string vcLength = dt.Rows[i]["vcLength"] == System.DBNull.Value ? "" : dt.Rows[i]["vcLength"].ToString().Trim();
                    string vcWide = dt.Rows[i]["vcWide"] == System.DBNull.Value ? "" : dt.Rows[i]["vcWide"].ToString().Trim();
                    string vcHeight = dt.Rows[i]["vcHeight"] == System.DBNull.Value ? "" : dt.Rows[i]["vcHeight"].ToString();
                    string vcEmptyWeight = dt.Rows[i]["vcEmptyWeight"] == System.DBNull.Value ? "" : dt.Rows[i]["vcEmptyWeight"].ToString().Trim();
                    string vcUnitNetWeight = dt.Rows[i]["vcUnitNetWeight"] == System.DBNull.Value ? "" : dt.Rows[i]["vcUnitNetWeight"].ToString().Trim();
                    string dSendDate = dt.Rows[i]["dSendDate"] == System.DBNull.Value ? "" : dt.Rows[i]["dSendDate"].ToString().Trim();
                    string dReplyDate = dt.Rows[i]["dReplyDate"] == System.DBNull.Value ? "" : dt.Rows[i]["dReplyDate"].ToString().Trim();
                    string dAdmitDate = dt.Rows[i]["dAdmitDate"] == System.DBNull.Value ? "" : dt.Rows[i]["dAdmitDate"].ToString().Trim();
                    string dWeaveDate = dt.Rows[i]["dWeaveDate"] == System.DBNull.Value ? "" : dt.Rows[i]["dWeaveDate"].ToString().Trim();
                    string vcMemo = dt.Rows[i]["vcMemo"] == System.DBNull.Value ? "" : dt.Rows[i]["vcMemo"].ToString().Trim();
                    string vcOperatorID = strUserId;


                    sql.Append("   INSERT INTO [dbo].[THeZiManageImportTmp]   \n");
                    sql.Append("              ([vcType],[vcPackingPlant],[vcReceiver],[dSynchronizationDate]   \n");
                    sql.Append("              ,[vcState] ,[vcPartNo]  ,[dUseStartDate]    \n");
                    sql.Append("              ,[vcPartName]  ,[vcCarType],[vcOEOrSP] ,[vcSupplier_id]   \n");
                    sql.Append("              ,[vcWorkArea] ,[dExpectDeliveryDate] ,[vcExpectIntake],[vcIntake]   \n");
                    sql.Append("              ,[vcBoxMaxIntake] ,[vcBoxType] ,[vcLength],[vcWide] ,[vcHeight]   \n");
                    sql.Append("              ,[vcEmptyWeight] ,[vcUnitNetWeight] ,[dSendDate] ,[dReplyDate]   \n");
                    sql.Append("              ,[dAdmitDate] ,[dWeaveDate] ,[vcMemo],[vcOperatorID],[dOperatorTime]   \n");
                    sql.Append("             ) values    \n");
                    sql.Append("   		  ( " + getSqlValue(vcType, true) + "," + getSqlValue(vcPackingPlant, true) + "," + getSqlValue(vcReceiver, true) + "," + getSqlValue(dSynchronizationDate, true) + ",  \n");
                    sql.Append("    " + getSqlValue(vcState, true) + "," + getSqlValue(vcPartNo, true) + "," + getSqlValue(dUseStartDate, true) + ",   \n");
                    sql.Append("     " + getSqlValue(vcPartName, true) + "," + getSqlValue(vcCarType, true) + "," + getSqlValue(vcOEOrSP, true) + "," + getSqlValue(vcSupplier_id, true) + ",   \n");
                    sql.Append("     " + getSqlValue(vcWorkArea, true) + "," + getSqlValue(dExpectDeliveryDate, true) + "," + getSqlValue(vcExpectIntake, true) + "," + getSqlValue(vcIntake, true) + ",   \n");
                    sql.Append("   " + getSqlValue(vcBoxMaxIntake, true) + "," + getSqlValue(vcBoxType, true) + "," + getSqlValue(vcLength, true) + "," + getSqlValue(vcWide, true) + ", " + getSqlValue(vcHeight, true) + ",   \n");
                    sql.Append("    " + getSqlValue(vcEmptyWeight, true) + "," + getSqlValue(vcUnitNetWeight, true) + "," + getSqlValue(dSendDate, true) + "," + getSqlValue(dReplyDate, true) + ",  \n");
                    sql.Append("    " + getSqlValue(dAdmitDate, true) + "," + getSqlValue(dWeaveDate, true) + "," + getSqlValue(vcMemo, true) + ",  \n");
                    sql.Append("   '" + strUserId + "' \n");
                    sql.Append("     ,getdate())  \n");
                }
                //删除
                sql.Append("   delete b from    \n");
                sql.Append("   (select * from [THeZiManageImportTmp] where vcType='2' and vcOperatorID='"+ strUserId + "') a   \n");
                sql.Append("   left join [THeZiManage] b    \n");
                sql.Append("   on a.vcPackingPlant = b.vcPackingPlant and a.vcReceiver=b.vcReceiver   \n");
                sql.Append("   and a.vcPartNo = b.vcPartNo    \n");
                sql.Append("   and isnull(a.vcSupplier_id,'')=isnull(b.vcSupplier_id,'')   \n");
                sql.Append("      \n");
                //更新
                sql.Append("   update b set b.[dUseStartDate] = a.dUseStartDate,   \n");
                sql.Append("    b.[vcPartName]=a.vcPartName  ,b.[vcCarType]=a.vcCarType,   \n");
                sql.Append("    b.[vcOEOrSP]=a.vcOEOrSP ,b.[vcWorkArea]=a.vcWorkArea ,   \n");
                sql.Append("    b.[dExpectDeliveryDate]=a.dExpectDeliveryDate ,   \n");
                sql.Append("    b.[vcExpectIntake]=a.vcExpectIntake,b.[vcIntake]=a.vcIntake,   \n");
                sql.Append("    b.[vcBoxMaxIntake]=a.vcBoxMaxIntake ,   \n");
                sql.Append("    b.[vcBoxType]=a.vcBoxType ,b.[vcLength]=a.vcLength,   \n");
                sql.Append("    b.[vcWide]=a.vcWide ,b.[vcHeight]=a.vcHeight   \n");
                sql.Append("    ,b.[vcEmptyWeight]=a.vcEmptyWeight ,b.[vcUnitNetWeight]=a.vcUnitNetWeight ,   \n");
                sql.Append("    b.vcMemo=a.vcMemo from    \n");
                sql.Append("    (select * from [THeZiManageImportTmp] where vcType='3' and vcOperatorID='" + strUserId + "') a   \n");
                sql.Append("   	left join [THeZiManage] b    \n");
                sql.Append("   	on a.vcPackingPlant = b.vcPackingPlant and a.vcReceiver=b.vcReceiver   \n");
                sql.Append("   	and a.vcPartNo = b.vcPartNo    \n");
                sql.Append("   	and isnull(a.vcSupplier_id,'')=isnull(b.vcSupplier_id,'')     \n");
                sql.Append("      \n");
                sql.Append("      \n");
                sql.Append("   INSERT INTO [dbo].[THeZiManage]   \n");
                sql.Append("              ([vcPackingPlant],[vcReceiver],[dSynchronizationDate]   \n");
                sql.Append("              ,[vcState] ,[vcPartNo]  ,[dUseStartDate] ,[dUserEndDate]   \n");
                sql.Append("              ,[vcPartName]  ,[vcCarType],[vcOEOrSP] ,[vcSupplier_id]   \n");
                sql.Append("              ,[vcWorkArea] ,[dExpectDeliveryDate] ,[vcExpectIntake],[vcIntake]   \n");
                sql.Append("              ,[vcBoxMaxIntake] ,[vcBoxType] ,[vcLength],[vcWide] ,[vcHeight]   \n");
                sql.Append("              ,[vcEmptyWeight] ,[vcUnitNetWeight] ,[dSendDate] ,[dReplyDate]   \n");
                sql.Append("              ,[dAdmitDate] ,[dWeaveDate] ,[vcMemo],[vcOperatorID],[dOperatorTime],vcInserter,vcInserterDate   \n");
                sql.Append("             )    \n");
                sql.Append("       select [vcPackingPlant],[vcReceiver],[dSynchronizationDate]   \n");
                sql.Append("              ,[vcState] ,[vcPartNo]  ,[dUseStartDate] ,[dUserEndDate]   \n");
                sql.Append("              ,[vcPartName]  ,[vcCarType],[vcOEOrSP] ,[vcSupplier_id]   \n");
                sql.Append("              ,[vcWorkArea] ,[dExpectDeliveryDate] ,[vcExpectIntake],[vcIntake]   \n");
                sql.Append("              ,[vcBoxMaxIntake] ,[vcBoxType] ,[vcLength],[vcWide] ,[vcHeight]   \n");
                sql.Append("              ,[vcEmptyWeight] ,[vcUnitNetWeight] ,[dSendDate] ,[dReplyDate]   \n");
                sql.Append("              ,[dAdmitDate] ,[dWeaveDate] ,[vcMemo],[vcOperatorID],GETDATE(),[vcOperatorID],GETDATE()   \n");
                sql.Append("   		    from  [THeZiManageImportTmp]      \n");
                sql.Append("   	        where vcType='1' and vcOperatorID='" + strUserId + "'   \n");
                sql.Append("      \n");
                if (sql.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(sql.ToString());
                }
                #region

                //for (int i =           0; i < dt.Rows.Count; i++)
                //{                     
                //    StringBuilder strSql = new StringBuilder();
                //    string vcType = dt.Rows[i]["vcOperWay"] == System.DBNull.Value ? "" : dt.Rows[i]["vcOperWay"].ToString();
                //    if (vcType == "新增") {
                //        vcType = "1";
                //    }
                //    else if (vcType == "删除") {
                //        vcType = "2";
                //    } else if (vcType == "修改") {
                //        vcType = "3";
                //    }

                //    string vcPackingPlant = dt.Rows[i]["vcPackingPlant"] == System.DBNull.Value ? "" : dt.Rows[i]["vcPackingPlant"].ToString();
                //    string vcReceiver = dt.Rows[i]["vcReceiver"] == System.DBNull.Value ? "" : dt.Rows[i]["vcReceiver"].ToString();
                //    string dSynchronizationDate = dt.Rows[i]["dSynchronizationDate"] == System.DBNull.Value ? "" : dt.Rows[i]["dSynchronizationDate"].ToString();
                //    string vcState = dt.Rows[i]["vcState"] == System.DBNull.Value ? "" : dt.Rows[i]["vcState"].ToString();
                //    #region

                //    if (vcState == "未发送")
                //    {
                //        vcState = "0";
                //    }
                //    else if (vcState == "待回复")
                //    {
                //        vcState = "1";
                //    }
                //    else if (vcState == "已回复")
                //    {
                //        vcState = "2";
                //    }
                //    else if (vcState == "退回")
                //    {
                //        vcState = "3";
                //    }
                //    else if (vcState == "已承认")
                //    {
                //        vcState = "4";
                //    }
                //    else if (vcState == "已织入")
                //    {
                //        vcState = "5";
                //    } else
                //    {
                //        vcState = "0";
                //    }
                //    #endregion
                //    string vcPartNo = dt.Rows[i]["vcPartNo"] == System.DBNull.Value ? "" : dt.Rows[i]["vcPartNo"].ToString();
                //    string dUseStartDate = dt.Rows[i]["dUseStartDate"] == System.DBNull.Value ? "" : dt.Rows[i]["dUseStartDate"].ToString();
                //    string dUserEndDate = dt.Rows[i]["dUserEndDate"] == System.DBNull.Value ? "" : dt.Rows[i]["dUserEndDate"].ToString();
                //    string vcPartName = dt.Rows[i]["vcPartName"] == System.DBNull.Value ? "" : dt.Rows[i]["vcPartName"].ToString();
                //    string vcCarType = dt.Rows[i]["vcCarType"] == System.DBNull.Value ? "" : dt.Rows[i]["vcCarType"].ToString();
                //    string vcOEOrSP = dt.Rows[i]["vcOEOrSP"] == System.DBNull.Value ? "" : dt.Rows[i]["vcOEOrSP"].ToString();
                //    if (vcOEOrSP == "×")
                //    {
                //        vcOEOrSP = "1";
                //    }
                //    else if (vcOEOrSP == "⭕" || vcOEOrSP == "○")
                //    {
                //        vcOEOrSP = "0";
                //    }
                //    else
                //    { }

                //    string vcSupplier_id = dt.Rows[i]["vcSupplier_id"] == System.DBNull.Value ? "" : dt.Rows[i]["vcSupplier_id"].ToString();
                //    string vcWorkArea = dt.Rows[i]["vcWorkArea"] == System.DBNull.Value ? "" : dt.Rows[i]["vcWorkArea"].ToString();
                //    string dExpectDeliveryDate = dt.Rows[i]["dExpectDeliveryDate"] == System.DBNull.Value ? "" : dt.Rows[i]["dExpectDeliveryDate"].ToString();
                //    string vcExpectIntake = dt.Rows[i]["vcExpectIntake"] == System.DBNull.Value ? "" : dt.Rows[i]["vcExpectIntake"].ToString();
                //    string vcIntake = dt.Rows[i]["vcIntake"] == System.DBNull.Value ? "" : dt.Rows[i]["vcIntake"].ToString();
                //    string vcBoxMaxIntake = dt.Rows[i]["vcBoxMaxIntake"] == System.DBNull.Value ? "" : dt.Rows[i]["vcBoxMaxIntake"].ToString();
                //    string vcBoxType = dt.Rows[i]["vcBoxType"] == System.DBNull.Value ? "" : dt.Rows[i]["vcBoxType"].ToString();
                //    string vcLength = dt.Rows[i]["vcLength"] == System.DBNull.Value ? "" : dt.Rows[i]["vcLength"].ToString();
                //    string vcWide = dt.Rows[i]["vcWide"] == System.DBNull.Value ? "" : dt.Rows[i]["vcWide"].ToString();
                //    string vcHeight = dt.Rows[i]["vcHeight"] == System.DBNull.Value ? "" : dt.Rows[i]["vcHeight"].ToString();
                //    string vcEmptyWeight = dt.Rows[i]["vcEmptyWeight"] == System.DBNull.Value ? "" : dt.Rows[i]["vcEmptyWeight"].ToString();
                //    string vcUnitNetWeight = dt.Rows[i]["vcUnitNetWeight"] == System.DBNull.Value ? "" : dt.Rows[i]["vcUnitNetWeight"].ToString();
                //    string dSendDate = dt.Rows[i]["dSendDate"] == System.DBNull.Value ? "" : dt.Rows[i]["dSendDate"].ToString();
                //    string dReplyDate = dt.Rows[i]["dReplyDate"] == System.DBNull.Value ? "" : dt.Rows[i]["dReplyDate"].ToString();
                //    string dAdmitDate = dt.Rows[i]["dAdmitDate"] == System.DBNull.Value ? "" : dt.Rows[i]["dAdmitDate"].ToString();
                //    string dWeaveDate = dt.Rows[i]["dWeaveDate"] == System.DBNull.Value ? "" : dt.Rows[i]["dWeaveDate"].ToString();
                //    string vcMemo = dt.Rows[i]["vcMemo"] == System.DBNull.Value ? "" : dt.Rows[i]["vcMemo"].ToString();
                //    string vcOperatorID = dt.Rows[i]["vcOperatorID"] == System.DBNull.Value ? "" : dt.Rows[i]["vcOperatorID"].ToString();





                //    if (vcOperWay=="新增")
                //    {
                //        strSql.AppendLine("   INSERT INTO [dbo].[THeZiManage]   ");
                //        strSql.AppendLine("         ([vcPackingPlant] ,[vcReceiver],[dSynchronizationDate],[vcState] ,[vcPartNo] ,[dUseStartDate] ,[dUserEndDate]   ");
                //        strSql.AppendLine("         ,[vcPartName] ,[vcCarType] ,[vcOEOrSP] ,[vcSupplier_id]  ,[vcWorkArea]  , [dExpectDeliveryDate]  , [vcExpectIntake]   ");
                //        strSql.AppendLine("          ,[vcIntake]  ,[vcBoxMaxIntake]  ,[vcBoxType] , [vcLength] , [vcWide] ,[vcHeight]  ,[vcEmptyWeight]  ,[vcUnitNetWeight]   ");
                //        strSql.AppendLine("          ,[dSendDate] ,[dReplyDate]  ,[dAdmitDate],[dWeaveDate] ,[vcMemo]  ");
                //        strSql.AppendLine("         ,[vcInserter]  ,vcInserterDate,[vcOperatorID] , [dOperatorTime])");
                //        strSql.AppendLine("  values ( '" + vcPackingPlant + "','" + vcReceiver + "',null,0,'" + vcPartNo + "',null,null,  ");
                //        strSql.AppendLine("   null,null,null,'" + vcSupplier_id + "',null," +getSqlValue(dExpectDeliveryDate, true) + "," + getSqlValue(vcExpectIntake, true) + ", ");
                //        strSql.AppendLine("   " + getSqlValue(vcIntake, true) + "," + getSqlValue(vcBoxMaxIntake, true) + "," + getSqlValue(vcBoxType, true) + "," + getSqlValue(vcLength, true) + "," + getSqlValue(vcWide, true) + "," + getSqlValue(vcHeight, true) + "," + getSqlValue(vcEmptyWeight, true) + "," + getSqlValue(vcUnitNetWeight, true) + ", ");
                //        strSql.AppendLine("   null,null,null,null," + getSqlValue(vcMemo, true) + ",'"+strUserId+ "',getdate(),'" + strUserId + "',getdate()) ");

                //    }
                //    if (vcOperWay == "删除")
                //    {
                //        strSql.AppendLine("  delete from [THeZiManage] where vcPackingPlant='"+ vcPackingPlant + "'  ");
                //        strSql.AppendLine("   and vcReceiver='" + vcReceiver + "' and vcSupplier_id='" + vcSupplier_id + "' and vcPartNo='" + vcPartNo + "'  ");
                //    }
                //    if (vcOperWay == "修改")
                //    {
                //        strSql.AppendLine("   update [THeZiManage]  ");
                //        strSql.AppendLine("    set dExpectDeliveryDate=" + getSqlValue(dExpectDeliveryDate, true) + ",  vcExpectIntake=" + getSqlValue(vcExpectIntake, true) + ",  vcIntake =" + getSqlValue(vcIntake, true) + ",  ");
                //        strSql.AppendLine("    vcBoxMaxIntake=" + getSqlValue(vcBoxMaxIntake, true) + ",  ");
                //        strSql.AppendLine("    vcBoxType=" + getSqlValue(vcBoxType, true) + ",  ");
                //        strSql.AppendLine("    vcLength=" + getSqlValue(vcLength, true) + ",  ");
                //        strSql.AppendLine("    vcWide=" + getSqlValue(vcWide, true) + ",  ");
                //        strSql.AppendLine("    vcHeight=" + getSqlValue(vcHeight, true) + ",  ");
                //        strSql.AppendLine("    vcEmptyWeight=" + getSqlValue(vcEmptyWeight, true) + ",  ");
                //        strSql.AppendLine("   vcUnitNetWeight=" + getSqlValue(vcUnitNetWeight, true) + ",vcMemo=" + getSqlValue(vcMemo, true) + ", vcOperatorID = '" + strUserId + "', dOperatorTime = GETDATE()  ");
                //        strSql.AppendLine("   where vcPackingPlant='" + vcPackingPlant + "' and vcReceiver='" + vcReceiver + "' and  vcPartNo='" + vcPartNo + "' and vcSupplier_id='" + vcSupplier_id + "'  ;  ");
                //    }

                //    //strSql.AppendLine("  declare @isExist int =0;   ");
                //    //strSql.AppendLine("  select @isExist=COUNT(*) from [dbo].[VI_THeZiManage] where vcPackingPlant='" + vcPackingPlant + "' and vcReceiver='" + vcReceiver + "' and  vcPartNo='" + vcPartNo + "' and vcSupplier_id='" + vcSupplier_id + "' and  vcState in ('1','0')    ");
                //    //strSql.AppendLine("     ");
                //    //strSql.AppendLine("  if @isExist>0   ");
                //    //strSql.AppendLine("  begin   ");
                //    //strSql.AppendLine("  declare @isExistTHezi int =0;   ");
                //    //strSql.AppendLine("  select @isExistTHezi=COUNT(*) from [dbo].[THeZiManage] where vcPackingPlant='" + vcPackingPlant + "' and vcReceiver='" + vcReceiver + "' and  vcPartNo='" + vcPartNo + "' and vcSupplier_id='" + vcSupplier_id + "' and  vcState in ('1','0')    ");
                //    //strSql.AppendLine("  if @isExistTHezi>0   ");
                //    //strSql.AppendLine("  begin   ");
                //    //strSql.AppendLine("   update [THeZiManage]  ");
                //    //strSql.AppendLine("    set dExpectDeliveryDate='" + dExpectDeliveryDate + "',  vcExpectIntake='" + vcExpectIntake + "',  vcIntake ='" + vcIntake + "',  ");
                //    //strSql.AppendLine("    vcBoxMaxIntake='" + vcBoxMaxIntake + "',  ");
                //    //strSql.AppendLine("    vcBoxType='" + vcBoxType + "',  ");
                //    //strSql.AppendLine("    vcLength='" + vcLength + "',  ");
                //    //strSql.AppendLine("    vcWide='" + vcWide + "',  ");
                //    //strSql.AppendLine("    vcHeight='" + vcHeight + "',  ");
                //    //strSql.AppendLine("    vcEmptyWeight='" + vcEmptyWeight + "',  ");
                //    //strSql.AppendLine("   vcUnitNetWeight='" + vcUnitNetWeight + "', vcOperatorID = '" + strUserId + "', dOperatorTime = GETDATE()  ");
                //    //strSql.AppendLine("   where vcPackingPlant='" + vcPackingPlant + "' and vcReceiver='" + vcReceiver + "' and  vcPartNo='" + vcPartNo + "' and vcSupplier_id='" + vcSupplier_id + "' and  vcState in ('1','0')  ;  ");
                //    //strSql.AppendLine("  end   ");
                //    //strSql.AppendLine("  else   ");
                //    //strSql.AppendLine("  begin   ");
                //    //strSql.AppendLine("   INSERT INTO [dbo].[THeZiManage]   ");
                //    //strSql.AppendLine("         ([vcPackingPlant] ,[vcReceiver],[dSynchronizationDate],[vcState] ,[vcPartNo] ,[dUseStartDate] ,[dUserEndDate]   ");
                //    //strSql.AppendLine("         ,[vcPartName] ,[vcCarType] ,[vcOEOrSP] ,[vcSupplier_id]  ,[vcWorkArea]  , [dExpectDeliveryDate]  , [vcExpectIntake]   ");
                //    //strSql.AppendLine("          ,[vcIntake]  ,[vcBoxMaxIntake]  ,[vcBoxType] , [vcLength] , [vcWide] ,[vcHeight]  ,[vcEmptyWeight]  ,[vcUnitNetWeight]   ");
                //    //strSql.AppendLine("          ,[dSendDate] ,[dReplyDate]  ,[dAdmitDate],[dWeaveDate] ,[vcMemo]  ,[vcImageRoutes]   ");
                //    //strSql.AppendLine("         ,[vcInserter]  ,vcInserterDate,[vcFactoryOperatorID] ,[dFactoryOperatorTime]  ,[vcOperatorID] , [dOperatorTime])");
                //    //strSql.AppendLine("  select  [vcPackingPlant], [vcReceiver],[dSynchronizationDate], [vcState], [vcPartNo], [dUseStartDate], [dUserEndDate],   ");
                //    //strSql.AppendLine("   [vcPartName], [vcCarType], [vcOEOrSP], [vcSupplier_id], [vcWorkArea],   ");
                //    //strSql.AppendLine("  '" + dExpectDeliveryDate + "' as [dExpectDeliveryDate], '" + vcExpectIntake + "' as [vcExpectIntake],'" + vcIntake + "' as  [vcIntake],'" + vcBoxMaxIntake + "' as   [vcBoxMaxIntake], '" + vcBoxType + "' as  [vcBoxType],'" + vcLength + "' as   [vcLength],'" + vcWide + "' as  [vcWide],    ");
                //    //strSql.AppendLine("  '" + vcHeight + "' as   [vcHeight],'" + vcEmptyWeight + "' as   [vcEmptyWeight],'" + vcUnitNetWeight + "' as   [vcUnitNetWeight], [dSendDate], [dReplyDate], [dAdmitDate], [dWeaveDate], [vcMemo],   ");
                //    //strSql.AppendLine("  [vcImageRoutes], '" + strUserId + "' as   [vcInserter], getdate() as  [vcInserterDate],    ");
                //    //strSql.AppendLine("  [vcFactoryOperatorID], [dFactoryOperatorTime],'" + strUserId + "' as   [vcOperatorID], getdate() as  [dOperatorTime] from [dbo].[VI_THeZiManage]   ");
                //    //strSql.AppendLine("   where vcPackingPlant='" + vcPackingPlant + "' and vcReceiver='" + vcReceiver + "' and  vcPartNo='" + vcPartNo + "' and vcSupplier_id='" + vcSupplier_id + "' and  vcState in ('1','0') ;  ");
                //    //strSql.AppendLine("  end   ");
                //    //strSql.AppendLine("  end   ");
                //    excute.ExcuteSqlWithStringOper(strSql.ToString());
                //}
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #region 返回insert语句值
        /// <summary>
        /// 返回insert语句值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="isObject">如果insert时间、金额或者其他对象类型数据，为true</param>
        /// <returns></returns>
        private string getSqlValue(Object obj, bool isObject)
        {
            if (obj == null)
                return "null";
            else if (obj.ToString().Trim() == "" && isObject)
                return "null";
            else
                return "'" + obj.ToString() + "'";
        }
        #endregion

        ///同步数据
        public void getData(string userId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();

                sql.AppendLine("   insert into  THeZiManage    ");
                sql.AppendLine("   ( vcPackingPlant, vcReceiver, dSynchronizationDate, vcState, vcPartNo,   ");
                sql.AppendLine("    dUseStartDate, dUserEndDate, vcPartName, vcCarType, vcOEOrSP, vcSupplier_id,    ");
                sql.AppendLine("    vcWorkArea, dExpectDeliveryDate, vcExpectIntake, vcIntake, vcBoxMaxIntake,    ");
                sql.AppendLine("    vcBoxType, vcLength, vcWide, vcHeight, vcEmptyWeight, vcUnitNetWeight,   ");
                sql.AppendLine("     dSendDate, dReplyDate, dAdmitDate, dWeaveDate, vcMemo, vcImageRoutes,   ");
                sql.AppendLine("      vcInserter, vcInserterDate, vcOperatorID, dOperatorTime)   ");
                sql.AppendLine("     select   a.vcPackingPlant,a.vcReceiver, a.dSyncTime as dSynchronizationDate, '0' as vcState,a.vcPartId as vcPartNo,         ");
                sql.AppendLine("             a.dFromTime as dUseStartDate,a.dToTime as dUserEndDate,a.vcPartENName as vcPartName,a.vcCarModel as vcCarType,         ");
                sql.AppendLine("             a.vcOESP as vcOEOrSP,a.vcSupplierId as vcSupplier_id,c.vcSupplierPlant as vcWorkArea,         ");
                sql.AppendLine("             null as dExpectDeliveryDate,null as vcExpectIntake,   ");
                sql.AppendLine("  		   d.iPackingQty as [vcIntake],null as [vcBoxMaxIntake],   ");
                sql.AppendLine("  		   d.vcBoxType as [vcBoxType],d.iLength as [vcLength],d.iWidth as [vcWide],         ");
                sql.AppendLine("             d.iHeight as [vcHeight],null as [vcEmptyWeight],   ");
                sql.AppendLine("  		   null as [vcUnitNetWeight], null as [dSendDate],   ");
                sql.AppendLine("  		   null as [dReplyDate],null as [dAdmitDate],null as [dWeaveDate],        ");
                sql.AppendLine("             null as [vcMemo],null as  [vcImageRoutes],   ");
                sql.AppendLine("  		   '"+ userId + "' as [vcInserter],GETDATE() as [vcInserterDate],   ");
                sql.AppendLine("            '" + userId + "' as [vcOperatorID], GETDATE() as [dOperatorTime]      ");
                sql.AppendLine("               from  (select * from [dbo].[TSPMaster] where vcInOut='1' and isnull(vcDelete, '') <> '1'       ");
                sql.AppendLine("               AND (([dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))        ");
                sql.AppendLine("                OR ([dFromTime]>=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23)))) a       ");
                sql.AppendLine("               left join (select vcPackingPlant,vcPartNo,vcReceiver,vcSupplier_id,dUseStartDate,dUserEndDate from THeZiManage)b           ");
                sql.AppendLine("               on a.vcPackingPlant = b.vcPackingPlant and a.vcPartId = b.vcPartNo and a.vcReceiver=b.vcReceiver and a.vcSupplierId=b.vcSupplier_id           ");
                sql.AppendLine("               left join (       ");
                sql.AppendLine("          	  SELECT  [vcPackingPlant] ,[vcPartId],[vcReceiver],[vcSupplierId],[dFromTime] ,[dToTime] ,[vcSupplierPlant],[vcOperatorType]            ");
                sql.AppendLine("               FROM [SPPSdb].[dbo].[TSPMaster_SupplierPlant] where vcOperatorType='1'       ");
                sql.AppendLine("          	 AND (([dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))        ");
                sql.AppendLine("                OR ([dFromTime]>=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23)))      ");
                sql.AppendLine("          	 ) c on a.vcPackingPlant=c.vcPackingPlant and a.vcPartId=c.vcPartId           ");
                sql.AppendLine("               and a.vcReceiver=c.vcReceiver and a.vcSupplierId = c.vcSupplierId            ");
                sql.AppendLine("            left join  (select vcPackingPlant, vcPartId, vcReceiver, vcSupplierId, vcSupplierPlant, dFromTime, dToTime,             ");
                sql.AppendLine("            iPackingQty, vcBoxType, iLength, iWidth, iHeight, iVolume, vcOperatorType, vcOperatorID, dOperatorTime             ");
                sql.AppendLine("             from TSPMaster_Box  where vcOperatorType='1'      ");
                sql.AppendLine("             AND (([dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))        ");
                sql.AppendLine("             OR ([dFromTime]>=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23)))      ");
                sql.AppendLine("             ) d on a.vcPackingPlant=d.vcPackingPlant and a.vcPartId=d.vcPartId          ");
                sql.AppendLine("             and a.vcReceiver=d.vcReceiver and  a.[vcSupplierId]=d.[vcSupplierId]        ");
                sql.AppendLine("             LEFT JOIN       ");
                sql.AppendLine("          	(SELECT *  FROM [TSPMaster_SufferIn] WHERE [vcOperatorType]='1'        ");
                sql.AppendLine("          	AND (([dFromTime]<=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))        ");
                sql.AppendLine("          	OR ([dFromTime]>=CONVERT(VARCHAR(10),GETDATE(),23) AND [dToTime]>=CONVERT(VARCHAR(10),GETDATE(),23))))e      ");
                sql.AppendLine("          	ON a.[vcPackingPlant]=e.[vcPackingPlant] AND a.[vcPartId]=e.[vcPartId] AND a.[vcReceiver]=e.[vcReceiver] AND a.[vcSupplierId]=e.[vcSupplierId]       ");
                sql.AppendLine("          	LEFT JOIN       ");
                sql.AppendLine("          	(select vcValue1 as [vcSupplierId],vcValue2 as vcSupplierPlant,vcValue3 as [dFromTime],vcValue4 as [dToTime],vcValue5 as vcOrderPlant from TOutCode where vcCodeId='C010' and vcIsColum='0'       ");
                sql.AppendLine("          	and vcValue3<=CONVERT(VARCHAR(10),GETDATE(),23) AND vcValue4>=CONVERT(VARCHAR(10),GETDATE(),23))f       ");
                sql.AppendLine("          	ON a.[vcSupplierId]=f.[vcSupplierId] AND c.vcSupplierPlant=f.vcSupplierPlant       ");
                sql.AppendLine("               where b.vcPartNo is null       ");
                sql.AppendLine("          	 and (ISNULL(c.vcSupplierPlant,'')='' OR ISNULL(d.iPackingQty,0)=0 OR ISNULL(e.vcSufferIn,'')='' OR ISNULL(f.vcOrderPlant,'')='')             ");
                excute.ExcuteSqlWithStringOper(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
