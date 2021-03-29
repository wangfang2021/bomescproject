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
    public class FS0618_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        
        /// <summary>
        /// 检索数据
        /// </summary>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        public DataTable Search(string vcTargetYearMonth, string vcCpdcompany, string vcOrderNo, string vcDock, string vcPartNo, string vcOrderType, string vcSupplier_id, string dOrderExportDate)
        {
            try
            {
                string vcWorkArea = string.Empty;
                if (vcSupplier_id.Length > 4)
                {
                     vcWorkArea = vcSupplier_id.Substring(4, 1);
                }
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("  select [iAutoId], [vcPackingFactory], left([vcTargetYearMonth],4)+'/'+right([vcTargetYearMonth],2) as [vcTargetYearMonth], [vcDock],a.vcCpdcompany as [vcCpdcompany],b.vcName as [vcOrderType],    ");
                strSql.AppendLine("  [vcOrderNo], [vcSeqno],convert(varchar(10), dOrderDate,111) as [dOrderDate],convert(varchar(10), dOrderExportDate,111) as [dOrderExportDate], [vcPartNo],d.vcName as [vcInsideOutsideType],    ");
                //strSql.AppendLine("  [vcCarType], [vcLastPartNo], [vcPackingSpot], [vcSupplier_id], [vcPlantQtyDaily1], [vcInputQtyDaily1],   ");
                //strSql.AppendLine("  [vcResultQtyDaily1], [vcPlantQtyDaily2], [vcInputQtyDaily2], [vcResultQtyDaily2], [vcPlantQtyDaily3],   ");
                //strSql.AppendLine("  [vcInputQtyDaily3], [vcResultQtyDaily3], [vcPlantQtyDaily4], [vcInputQtyDaily4], [vcResultQtyDaily4],   ");
                //strSql.AppendLine("  [vcPlantQtyDaily5], [vcInputQtyDaily5], [vcResultQtyDaily5], [vcPlantQtyDaily6], [vcInputQtyDaily6],   ");
                //strSql.AppendLine("  [vcResultQtyDaily6], [vcPlantQtyDaily7], [vcInputQtyDaily7], [vcResultQtyDaily7], [vcPlantQtyDaily8],    ");
                //strSql.AppendLine("  [vcInputQtyDaily8], [vcResultQtyDaily8], [vcPlantQtyDaily9], [vcInputQtyDaily9], [vcResultQtyDaily9],   ");
                //strSql.AppendLine("  [vcPlantQtyDaily10], [vcInputQtyDaily10], [vcResultQtyDaily10], [vcPlantQtyDaily11], [vcInputQtyDaily11],   ");
                //strSql.AppendLine("  [vcResultQtyDaily11], [vcPlantQtyDaily12], [vcInputQtyDaily12], [vcResultQtyDaily12], [vcPlantQtyDaily13],   ");
                //strSql.AppendLine("  [vcInputQtyDaily13], [vcResultQtyDaily13], [vcPlantQtyDaily14], [vcInputQtyDaily14], [vcResultQtyDaily14],   ");
                //strSql.AppendLine("  [vcPlantQtyDaily15], [vcInputQtyDaily15], [vcResultQtyDaily15], [vcPlantQtyDaily16], [vcInputQtyDaily16],   ");
                //strSql.AppendLine("  [vcResultQtyDaily16], [vcPlantQtyDaily17], [vcInputQtyDaily17], [vcResultQtyDaily17], [vcPlantQtyDaily18],   ");
                //strSql.AppendLine("  [vcInputQtyDaily18], [vcResultQtyDaily18], [vcPlantQtyDaily19], [vcInputQtyDaily19], [vcResultQtyDaily19],   ");
                //strSql.AppendLine("  [vcPlantQtyDaily20], [vcInputQtyDaily20], [vcResultQtyDaily20], [vcPlantQtyDaily21], [vcInputQtyDaily21],   ");
                //strSql.AppendLine("  [vcResultQtyDaily21], [vcPlantQtyDaily22], [vcInputQtyDaily22], [vcResultQtyDaily22], [vcPlantQtyDaily23],   ");
                //strSql.AppendLine("  [vcInputQtyDaily23], [vcResultQtyDaily23], [vcPlantQtyDaily24], [vcInputQtyDaily24], [vcResultQtyDaily24],   ");
                //strSql.AppendLine("  [vcPlantQtyDaily25], [vcInputQtyDaily25], [vcResultQtyDaily25], [vcPlantQtyDaily26], [vcInputQtyDaily26],   ");
                //strSql.AppendLine("  [vcResultQtyDaily26], [vcPlantQtyDaily27], [vcInputQtyDaily27], [vcResultQtyDaily27], [vcPlantQtyDaily28],   ");
                //strSql.AppendLine("  [vcInputQtyDaily28], [vcResultQtyDaily28], [vcPlantQtyDaily29], [vcInputQtyDaily29], [vcResultQtyDaily29],   ");
                //strSql.AppendLine("  [vcPlantQtyDaily30], [vcInputQtyDaily30], [vcResultQtyDaily30], [vcPlantQtyDaily31], [vcInputQtyDaily31],[vcResultQtyDaily31],   ");
                strSql.AppendLine("  [vcCarType],vcPartNo as [vcLastPartNo], [vcPackingSpot], [vcSupplier_id], cast(isnull(a.vcPlantQtyDaily1,0) as int) as [vcPlantQtyDaily1],cast(isnull(a.[vcInputQtyDaily1],0) as int) as [vcInputQtyDaily1],      ");
                strSql.AppendLine("  cast(isnull(a.vcResultQtyDaily1,0) as int) as vcResultQtyDaily1, cast(isnull(a.[vcPlantQtyDaily2],0) as int) as [vcPlantQtyDaily2],cast(isnull(a.[vcInputQtyDaily2],0) as int) as [vcInputQtyDaily2],cast(isnull(a.[vcResultQtyDaily2],0) as int) as [vcResultQtyDaily2],cast(isnull(a.[vcPlantQtyDaily3],0) as int) as [vcPlantQtyDaily3],      ");
                strSql.AppendLine("  cast(isnull(a.[vcInputQtyDaily3],0) as int) as [vcInputQtyDaily3], cast(isnull(a.[vcResultQtyDaily3],0) as int) as [vcResultQtyDaily3],cast(isnull(a.[vcPlantQtyDaily4],0) as int) as [vcPlantQtyDaily4],cast(isnull(a.[vcInputQtyDaily4],0) as int) as [vcInputQtyDaily4],cast(isnull(a.[vcResultQtyDaily4],0) as int) as [vcResultQtyDaily4],      ");
                strSql.AppendLine("  cast(isnull(a.[vcPlantQtyDaily5],0) as int) as [vcPlantQtyDaily5], cast(isnull(a.[vcInputQtyDaily5],0) as int) as [vcInputQtyDaily5],cast(isnull(a.[vcResultQtyDaily5],0) as int) as [vcResultQtyDaily5],cast(isnull(a.[vcPlantQtyDaily6],0) as int) as [vcPlantQtyDaily6],cast(isnull(a.[vcInputQtyDaily6],0) as int) as [vcInputQtyDaily6],      ");
                strSql.AppendLine("  cast(isnull(a.[vcResultQtyDaily6],0) as int) as [vcResultQtyDaily6],cast(isnull(a.[vcPlantQtyDaily7],0) as int) as [vcPlantQtyDaily7],cast(isnull(a.[vcInputQtyDaily7],0) as int) as [vcInputQtyDaily7],cast(isnull(a.[vcResultQtyDaily7],0) as int) as [vcResultQtyDaily7],cast(isnull(a.[vcPlantQtyDaily8],0) as int) as [vcPlantQtyDaily8],       ");
                strSql.AppendLine("  cast(isnull(a.[vcInputQtyDaily8],0) as int) as [vcInputQtyDaily8],cast(isnull(a.[vcResultQtyDaily8],0) as int) as [vcResultQtyDaily8],cast(isnull(a.[vcPlantQtyDaily9],0) as int) as [vcPlantQtyDaily9],cast(isnull(a.[vcInputQtyDaily9],0) as int) as [vcInputQtyDaily9],cast(isnull(a.[vcResultQtyDaily9],0) as int) as [vcResultQtyDaily9],      ");
                strSql.AppendLine("  cast(isnull(a.[vcPlantQtyDaily10],0) as int) as [vcPlantQtyDaily10],cast(isnull(a.[vcInputQtyDaily10],0) as int) as [vcInputQtyDaily10],cast(isnull(a.[vcResultQtyDaily10],0) as int) as [vcResultQtyDaily10],cast(isnull(a.[vcPlantQtyDaily11],0) as int) as [vcPlantQtyDaily11],cast(isnull(a.[vcInputQtyDaily11],0) as int) as [vcInputQtyDaily11],      ");
                strSql.AppendLine("  cast(isnull(a.[vcResultQtyDaily11],0) as int) as [vcResultQtyDaily11],cast(isnull(a.[vcPlantQtyDaily12],0) as int) as [vcPlantQtyDaily12],cast(isnull(a.[vcInputQtyDaily12],0) as int) as [vcInputQtyDaily12],cast(isnull(a.[vcResultQtyDaily12],0) as int) as [vcResultQtyDaily12],cast(isnull(a.[vcPlantQtyDaily13],0) as int) as [vcPlantQtyDaily13],      ");
                strSql.AppendLine("  cast(isnull(a.[vcInputQtyDaily13],0) as int) as [vcInputQtyDaily13],cast(isnull(a.[vcResultQtyDaily13],0) as int) as [vcResultQtyDaily13],cast(isnull(a.[vcPlantQtyDaily14],0) as int) as [vcPlantQtyDaily14],cast(isnull(a.[vcInputQtyDaily14],0) as int) as [vcInputQtyDaily14],cast(isnull(a.[vcResultQtyDaily14],0) as int) as [vcResultQtyDaily14],      ");
                strSql.AppendLine("  cast(isnull(a.[vcPlantQtyDaily15],0) as int) as [vcPlantQtyDaily15],cast(isnull(a.[vcInputQtyDaily15],0) as int) as [vcInputQtyDaily15],cast(isnull(a.[vcResultQtyDaily15],0) as int) as [vcResultQtyDaily15],cast(isnull(a.[vcPlantQtyDaily16],0) as int) as [vcPlantQtyDaily16],cast(isnull(a.[vcInputQtyDaily16],0) as int) as [vcInputQtyDaily16],      ");
                strSql.AppendLine("  cast(isnull(a.[vcResultQtyDaily16],0) as int) as [vcResultQtyDaily16],cast(isnull(a.[vcPlantQtyDaily17],0) as int) as [vcPlantQtyDaily17],cast(isnull(a.[vcInputQtyDaily17],0) as int) as [vcInputQtyDaily17],cast(isnull(a.[vcResultQtyDaily17],0) as int) as [vcResultQtyDaily17],cast(isnull(a.[vcPlantQtyDaily18],0) as int) as [vcPlantQtyDaily18],      ");
                strSql.AppendLine("  cast(isnull(a.[vcInputQtyDaily18],0) as int) as [vcInputQtyDaily18],cast(isnull(a.[vcResultQtyDaily18],0) as int) as [vcResultQtyDaily18],cast(isnull(a.[vcPlantQtyDaily19],0) as int) as [vcPlantQtyDaily19],cast(isnull(a.[vcInputQtyDaily19],0) as int) as [vcInputQtyDaily19],cast(isnull(a.[vcResultQtyDaily19],0) as int) as [vcResultQtyDaily19],      ");
                strSql.AppendLine("  cast(isnull(a.[vcPlantQtyDaily20],0) as int) as [vcPlantQtyDaily20],cast(isnull(a.[vcInputQtyDaily20],0) as int) as [vcInputQtyDaily20],cast(isnull(a.[vcResultQtyDaily20],0) as int) as [vcResultQtyDaily20],cast(isnull(a.[vcPlantQtyDaily21],0) as int) as [vcPlantQtyDaily21],cast(isnull(a.[vcInputQtyDaily21],0) as int) as [vcInputQtyDaily21],      ");
                strSql.AppendLine("  cast(isnull(a.[vcResultQtyDaily21],0) as int) as [vcResultQtyDaily21],cast(isnull(a.[vcPlantQtyDaily22],0) as int) as [vcPlantQtyDaily22],cast(isnull(a.[vcInputQtyDaily22],0) as int) as [vcInputQtyDaily22],cast(isnull(a.[vcResultQtyDaily22],0) as int) as [vcResultQtyDaily22],cast(isnull(a.[vcPlantQtyDaily23],0) as int) as [vcPlantQtyDaily23],      ");
                strSql.AppendLine("  cast(isnull(a.[vcInputQtyDaily23],0) as int) as [vcInputQtyDaily23],cast(isnull(a.[vcResultQtyDaily23],0) as int) as [vcResultQtyDaily23],cast(isnull(a.[vcPlantQtyDaily24],0) as int) as [vcPlantQtyDaily24],cast(isnull(a.[vcInputQtyDaily24],0) as int) as [vcInputQtyDaily24], cast(isnull(a.[vcResultQtyDaily24],0) as int) as[vcResultQtyDaily24],      ");
                strSql.AppendLine("  cast(isnull(a.[vcPlantQtyDaily25],0) as int) as [vcPlantQtyDaily25],cast(isnull(a.[vcInputQtyDaily25],0) as int) as [vcInputQtyDaily25], cast(isnull(a.[vcResultQtyDaily25],0) as int) as [vcResultQtyDaily25],cast(isnull(a.[vcPlantQtyDaily26],0) as int) as [vcPlantQtyDaily26],cast(isnull(a.[vcInputQtyDaily26],0) as int) as [vcInputQtyDaily26],      ");
                strSql.AppendLine("  cast(isnull(a.[vcResultQtyDaily26],0) as int) as [vcResultQtyDaily26],cast(isnull(a.[vcPlantQtyDaily27],0) as int) as [vcPlantQtyDaily27],cast(isnull(a.[vcInputQtyDaily27],0) as int) as [vcInputQtyDaily27],cast(isnull(a.[vcResultQtyDaily27],0) as int) as [vcResultQtyDaily27],cast(isnull(a.[vcPlantQtyDaily28],0) as int) as [vcPlantQtyDaily28],      ");
                strSql.AppendLine("  cast(isnull(a.[vcInputQtyDaily28],0) as int) as [vcInputQtyDaily28],cast(isnull(a.[vcResultQtyDaily28],0) as int) as [vcResultQtyDaily28], cast(isnull(a.[vcPlantQtyDaily29],0) as int) as[vcPlantQtyDaily29],cast(isnull(a.[vcInputQtyDaily29],0) as int) as [vcInputQtyDaily29],cast(isnull(a.[vcResultQtyDaily29],0) as int) as [vcResultQtyDaily29],      ");
                strSql.AppendLine("  cast(isnull(a.[vcPlantQtyDaily30],0) as int) as [vcPlantQtyDaily30],cast(isnull(a.[vcInputQtyDaily30],0) as int) as [vcInputQtyDaily30], cast(isnull(a.[vcResultQtyDaily30],0) as int) as [vcResultQtyDaily30],cast(isnull(a.[vcPlantQtyDaily31],0) as int) as [vcPlantQtyDaily31],cast(isnull(a.[vcInputQtyDaily31],0) as int) as [vcInputQtyDaily31],cast(isnull(a.[vcResultQtyDaily31],0) as int) as[vcResultQtyDaily31],      ");
               
                //strSql.AppendLine("  cast(isnull(a.vcPlantQtyDaily1,0) as decimal(18,6))+cast(isnull(a.vcPlantQtyDaily2,0) as decimal(18,6))+cast(isnull(a.vcPlantQtyDaily3,0) as decimal(18,6))   ");
                //strSql.AppendLine("  +cast(isnull(a.vcPlantQtyDaily4,0) as decimal(18,6))+cast(isnull(a.vcPlantQtyDaily5,0) as decimal(18,6))+cast(isnull(a.vcPlantQtyDaily6,0) as decimal(18,6))   ");
                //strSql.AppendLine("  +cast(isnull(a.vcPlantQtyDaily7,0) as decimal(18,6))+cast(isnull(a.vcPlantQtyDaily8,0) as decimal(18,6))+cast(isnull(a.vcPlantQtyDaily9,0) as decimal(18,6))   ");
                //strSql.AppendLine("  +cast(isnull(a.vcPlantQtyDaily10,0) as decimal(18,6))+cast(isnull(a.vcPlantQtyDaily11,0) as decimal(18,6))+cast(isnull(a.vcPlantQtyDaily12,0) as decimal(18,6))   ");
                //strSql.AppendLine("  +cast(isnull(a.vcPlantQtyDaily13,0) as decimal(18,6))+cast(isnull(a.vcPlantQtyDaily14,0) as decimal(18,6))+cast(isnull(a.vcPlantQtyDaily15,0) as decimal(18,6))   ");
                //strSql.AppendLine("  +cast(isnull(a.vcPlantQtyDaily16,0) as decimal(18,6))+cast(isnull(a.vcPlantQtyDaily17,0) as decimal(18,6))+cast(isnull(a.vcPlantQtyDaily18,0) as decimal(18,6))   ");
                //strSql.AppendLine("  +cast(isnull(a.vcPlantQtyDaily19,0) as decimal(18,6))+cast(isnull(a.vcPlantQtyDaily20,0) as decimal(18,6))+cast(isnull(a.vcPlantQtyDaily21,0) as decimal(18,6))   ");
                //strSql.AppendLine("  +cast(isnull(a.vcPlantQtyDaily22,0) as decimal(18,6))+cast(isnull(a.vcPlantQtyDaily23,0) as decimal(18,6))+cast(isnull(a.vcPlantQtyDaily24,0) as decimal(18,6))   ");
                //strSql.AppendLine("  +cast(isnull(a.vcPlantQtyDaily25,0) as decimal(18,6))+cast(isnull(a.vcPlantQtyDaily26,0) as decimal(18,6))+cast(isnull(a.vcPlantQtyDaily27,0) as decimal(18,6))   ");
                //strSql.AppendLine("  +cast(isnull(a.vcPlantQtyDaily28,0) as decimal(18,6))+cast(isnull(a.vcPlantQtyDaily29,0) as decimal(18,6))+cast(isnull(a.vcPlantQtyDaily30,0) as decimal(18,6))   ");
                //strSql.AppendLine("  +cast(isnull(a.vcPlantQtyDaily31,0) as decimal(18,6)) as vcPlantQtyDailySum,   ");
                //strSql.AppendLine("  cast(isnull(a.vcInputQtyDaily1,0) as decimal(18,6))+cast(isnull(a.vcInputQtyDaily2,0) as decimal(18,6))+cast(isnull(a.vcInputQtyDaily3,0) as decimal(18,6))   ");
                //strSql.AppendLine("  +cast(isnull(a.vcInputQtyDaily4,0) as decimal(18,6))+cast(isnull(a.vcInputQtyDaily5,0) as decimal(18,6))+cast(isnull(a.vcInputQtyDaily6,0) as decimal(18,6))   ");
                //strSql.AppendLine("  +cast(isnull(a.vcInputQtyDaily7,0) as decimal(18,6))+cast(isnull(a.vcInputQtyDaily8,0) as decimal(18,6))+cast(isnull(a.vcInputQtyDaily9,0) as decimal(18,6))   ");
                //strSql.AppendLine("  +cast(isnull(a.vcInputQtyDaily10,0) as decimal(18,6))+cast(isnull(a.vcInputQtyDaily11,0) as decimal(18,6))+cast(isnull(a.vcInputQtyDaily12,0) as decimal(18,6))   ");
                //strSql.AppendLine("  +cast(isnull(a.vcInputQtyDaily13,0) as decimal(18,6))+cast(isnull(a.vcInputQtyDaily14,0) as decimal(18,6))+cast(isnull(a.vcInputQtyDaily15,0) as decimal(18,6))   ");
                //strSql.AppendLine("  +cast(isnull(a.vcInputQtyDaily16,0) as decimal(18,6))+cast(isnull(a.vcInputQtyDaily17,0) as decimal(18,6))+cast(isnull(a.vcInputQtyDaily18,0) as decimal(18,6))   ");
                //strSql.AppendLine("  +cast(isnull(a.vcInputQtyDaily19,0) as decimal(18,6))+cast(isnull(a.vcInputQtyDaily20,0) as decimal(18,6))+cast(isnull(a.vcInputQtyDaily21,0) as decimal(18,6))   ");
                //strSql.AppendLine("  +cast(isnull(a.vcInputQtyDaily22,0) as decimal(18,6))+cast(isnull(a.vcInputQtyDaily23,0) as decimal(18,6))+cast(isnull(a.vcInputQtyDaily24,0) as decimal(18,6))   ");
                //strSql.AppendLine("  +cast(isnull(a.vcInputQtyDaily25,0) as decimal(18,6))+cast(isnull(a.vcInputQtyDaily26,0) as decimal(18,6))+cast(isnull(a.vcInputQtyDaily27,0) as decimal(18,6))   ");
                //strSql.AppendLine("  +cast(isnull(a.vcInputQtyDaily28,0) as decimal(18,6))+cast(isnull(a.vcInputQtyDaily29,0) as decimal(18,6))+cast(isnull(a.vcInputQtyDaily30,0) as decimal(18,6))   ");
                //strSql.AppendLine("  +cast(isnull(a.vcInputQtyDaily31,0) as decimal(18,6)) as vcInputQtyDailySum,   ");
                //strSql.AppendLine("  cast(isnull(a.vcResultQtyDaily1,0) as decimal(18,6))+cast(isnull(a.vcResultQtyDaily2,0) as decimal(18,6))+cast(isnull(a.vcResultQtyDaily3,0) as decimal(18,6))   ");
                //strSql.AppendLine("  +cast(isnull(a.vcResultQtyDaily4,0) as decimal(18,6))+cast(isnull(a.vcResultQtyDaily5,0) as decimal(18,6))+cast(isnull(a.vcResultQtyDaily6,0) as decimal(18,6))   ");
                //strSql.AppendLine("  +cast(isnull(a.vcResultQtyDaily7,0) as decimal(18,6))+cast(isnull(a.vcResultQtyDaily8,0) as decimal(18,6))+cast(isnull(a.vcResultQtyDaily9,0) as decimal(18,6))   ");
                //strSql.AppendLine("  +cast(isnull(a.vcResultQtyDaily10,0) as decimal(18,6))+cast(isnull(a.vcResultQtyDaily11,0) as decimal(18,6))+cast(isnull(a.vcResultQtyDaily12,0) as decimal(18,6))   ");
                //strSql.AppendLine("  +cast(isnull(a.vcResultQtyDaily13,0) as decimal(18,6))+cast(isnull(a.vcResultQtyDaily14,0) as decimal(18,6))+cast(isnull(a.vcResultQtyDaily15,0) as decimal(18,6))   ");
                //strSql.AppendLine("  +cast(isnull(a.vcResultQtyDaily16,0) as decimal(18,6))+cast(isnull(a.vcResultQtyDaily17,0) as decimal(18,6))+cast(isnull(a.vcResultQtyDaily18,0) as decimal(18,6))   ");
                //strSql.AppendLine("  +cast(isnull(a.vcResultQtyDaily19,0) as decimal(18,6))+cast(isnull(a.vcResultQtyDaily20,0) as decimal(18,6))+cast(isnull(a.vcResultQtyDaily21,0) as decimal(18,6))   ");
                //strSql.AppendLine("  +cast(isnull(a.vcResultQtyDaily22,0) as decimal(18,6))+cast(isnull(a.vcResultQtyDaily23,0) as decimal(18,6))+cast(isnull(a.vcResultQtyDaily24,0) as decimal(18,6))   ");
                //strSql.AppendLine("  +cast(isnull(a.vcResultQtyDaily25,0) as decimal(18,6))+cast(isnull(a.vcResultQtyDaily26,0) as decimal(18,6))+cast(isnull(a.vcResultQtyDaily27,0) as decimal(18,6))   ");
                //strSql.AppendLine("  +cast(isnull(a.vcResultQtyDaily28,0) as decimal(18,6))+cast(isnull(a.vcResultQtyDaily29,0) as decimal(18,6))+cast(isnull(a.vcResultQtyDaily30,0) as decimal(18,6))   ");
                //strSql.AppendLine("  +cast(isnull(a.vcResultQtyDaily31,0) as decimal(18,6)) as vcResultQtyDailySum,   ");
                strSql.AppendLine("  cast(isnull(a.[vcPlantQtyDailySum],0) as int) as [vcPlantQtyDailySum],cast(isnull(a.[vcInputQtyDailySum],0) as int) as [vcInputQtyDailySum],cast(isnull(a.[vcResultQtyDailySum],0) as int) as [vcResultQtyDailySum],'0' as vcModFlag,'0' as vcAddFlag, case when [vcTargetMonthFlag]='0' then '0:确定月' else vcTargetMonthFlag  end as vcTargetMonthFlag,convert(varchar(10), vcTargetMonthLast,111) as [vcTargetMonthLast], [vcOperatorID], [dOperatorTime]    ");
                strSql.AppendLine("  from [dbo].[SP_M_ORD] a    ");
                strSql.AppendLine("  left join (select vcOrderInitials as vcValue,vcOrderDifferentiation as vcName from TOrderDifferentiation) b on a.vcOrderType = b.vcValue   ");
                strSql.AppendLine("  left join (select vcValue,vcName from TCode where  vcCodeId='C018') c on a.vcCpdcompany = c.vcValue   ");
                strSql.AppendLine("  left join (select vcValue,vcName from TCode where  vcCodeId='C003') d on a.vcInsideOutsideType = d.vcValue where 1=1    ");
                //string vcTargetYearMonth, string vcCpdcompany, string vcOrderNo, string vcDock, string vcPartNo, string vcOrderType, string vcSupplier_id, string dOrderExportDate
                if (vcTargetYearMonth.Length > 0)
                {
                    strSql.AppendLine("  and  a.vcTargetYearMonth = '" + vcTargetYearMonth.Replace("-","").Replace("/","") + "' ");
                }
                if (vcCpdcompany.Length > 0)
                {
                    strSql.AppendLine("  and  a.vcCpdcompany = '" + vcCpdcompany + "' ");
                }
                if (vcOrderNo.Length > 0)
                {
                    strSql.AppendLine("  and  a.vcOrderNo like '" + vcOrderNo + "%' ");
                }
                if (vcDock.Length > 0)
                {
                    strSql.AppendLine("  and  a.vcDock = '" + vcDock + "' ");
                }
                if (vcPartNo.Length > 0)
                {
                    strSql.AppendLine("  and  a.vcPartNo like '" + vcPartNo + "%' ");
                }
                if (vcOrderType.Length > 0)
                {
                    strSql.AppendLine("  and  a.vcOrderType = '" + vcOrderType + "' ");
                }
                if (vcSupplier_id.Length > 0)
                {
                    strSql.AppendLine("  and  vcSupplier_id like '" + vcSupplier_id.Substring(0,4) + "%' ");
                }
                if (vcWorkArea.Length > 0)
                {
                    strSql.AppendLine("  and  vcWorkArea like '" + vcWorkArea + "%' ");
                }
                if (dOrderExportDate.Length > 0)
                {
                    strSql.AppendLine("  and   CONVERT(varchar(10),  dOrderExportDate,112) =  '" + dOrderExportDate.Replace("-", "").Replace("/", "") + "' ");
                }
                //strSql.AppendLine("  order by  a.vcWorkArea desc,a.vcTargetYearMonth desc,a.vcDock desc,a.vcOrderType desc,a.vcOrderNo desc,a.vcSeqno asc,a.vcPartNo desc ");
                strSql.AppendLine("  order by a.vcTargetYearMonth desc,a.vcSeqno asc ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable getOrderType()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine(" select vcOrderInitials as vcValue,vcOrderDifferentiation as vcName from TOrderDifferentiation   ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable getSupplier()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                //strSql.AppendLine("     select a.vcSupplier_id as vcValue,a.vcSupplier_id  as vcName from       ");
                //strSql.AppendLine("     (select distinct vcSupplier_id+isnull(vcWorkArea,'') as vcSupplier_id from SP_M_ORD) a      ");
                //strSql.AppendLine("     order by a.vcSupplier_id asc     ");
                //strSql.AppendLine("      select a.vcSupplier_id as vcValue, isnull(a.vcSupplier_id+':'+b.vcSupplier_name,a.vcSupplier_id)   as vcName from      ");
                //strSql.AppendLine("     (select distinct vcSupplier_id+isnull(vcWorkArea,'') as vcSupplier_id from SP_M_ORD) a      ");
                //strSql.AppendLine("      left join (select vcSupplier_id,vcSupplier_name from TSupplier) b      ");
                //strSql.AppendLine("     on left(a.vcSupplier_id,4) =b.vcSupplier_id order by a.vcSupplier_id asc     ");

                strSql.AppendLine("      select a.vcSupplier_id as vcValue, a.vcSupplier_id   as vcName from          ");
                strSql.AppendLine("     (select distinct vcSupplier_id+isnull(vcWorkArea,'') as vcSupplier_id from SP_M_ORD) a      ");
                strSql.AppendLine("     order by a.vcSupplier_id asc        ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable getDock()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("  select a.vcDock as vcValue,a.vcDock as vcName from ( select distinct vcDock from SP_M_ORD) a  ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 验证重复项
        /// </summary>
        /// <param name="dtadd"></param>
        /// <returns></returns>
        public DataTable CheckDistinctByTable(DataTable dtadd)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                for (int i = 0; i < dtadd.Rows.Count; i++)
                {
                    if (strSql.Length > 0)
                    {
                        strSql.AppendLine("  union all SELECT * FROM [dbo].[TSpecialSupplier] where vcSupplier_id='" + dtadd.Rows[i]["vcSupplier_id"] + "' and vcWorkArea='" + dtadd.Rows[i]["vcWorkArea"] + "'  ");
                    }
                    else
                    {
                        strSql.AppendLine("  SELECT * FROM [dbo].[TSpecialSupplier] where vcSupplier_id='" + dtadd.Rows[i]["vcSupplier_id"] + "' and vcWorkArea='" + dtadd.Rows[i]["vcWorkArea"] + "'  ");
                    }
                }
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
                StringBuilder sql = new StringBuilder();

                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑

 //                   "vcPackingFactory", "vcTargetYearMonth", "vcDock", "vcCpdcompany", "vcOrderType", "vcOrderNo", "vcSeqno",
 //"dOrderDate", "dOrderExportDate", "vcPartNo", "vcInsideOutsideType", 
 //"vcSupplier_id", "vcPlantQtyDaily1", "vcInputQtyDaily1", "vcResultQtyDaily1", "vcPlantQtyDaily2", "vcInputQtyDaily2",
 //"vcResultQtyDaily2", "vcPlantQtyDaily3", "vcInputQtyDaily3", "vcResultQtyDaily3", "vcPlantQtyDaily4", "vcInputQtyDaily4",
 //"vcResultQtyDaily4", "vcPlantQtyDaily5", "vcInputQtyDaily5", "vcResultQtyDaily5", "vcPlantQtyDaily6", "vcInputQtyDaily6", 
 //"vcResultQtyDaily6", "vcPlantQtyDaily7", "vcInputQtyDaily7", "vcResultQtyDaily7", "vcPlantQtyDaily8", "vcInputQtyDaily8", 
 //"vcResultQtyDaily8", "vcPlantQtyDaily9", "vcInputQtyDaily9", "vcResultQtyDaily9", "vcPlantQtyDaily10", "vcInputQtyDaily10", 
 //"vcResultQtyDaily10", "vcPlantQtyDaily11", "vcInputQtyDaily11", "vcResultQtyDaily11", "vcPlantQtyDaily12", "vcInputQtyDaily12",
 //"vcResultQtyDaily12", "vcPlantQtyDaily13", "vcInputQtyDaily13", "vcResultQtyDaily13", "vcPlantQtyDaily14", "vcInputQtyDaily14", 
 //"vcResultQtyDaily14", "vcPlantQtyDaily15", "vcInputQtyDaily15", "vcResultQtyDaily15", "vcPlantQtyDaily16", "vcInputQtyDaily16", 
 //"vcResultQtyDaily16", "vcPlantQtyDaily17", "vcInputQtyDaily17", "vcResultQtyDaily17", "vcPlantQtyDaily18", "vcInputQtyDaily18", 
 //"vcResultQtyDaily18", "vcPlantQtyDaily19", "vcInputQtyDaily19", "vcResultQtyDaily19", "vcPlantQtyDaily20", "vcInputQtyDaily20",
 //"vcResultQtyDaily20", "vcPlantQtyDaily21", "vcInputQtyDaily21", "vcResultQtyDaily21", "vcPlantQtyDaily22", "vcInputQtyDaily22", 
 //"vcResultQtyDaily22", "vcPlantQtyDaily23", "vcInputQtyDaily23", "vcResultQtyDaily23", "vcPlantQtyDaily24", "vcInputQtyDaily24", 
 //"vcResultQtyDaily24", "vcPlantQtyDaily25", "vcInputQtyDaily25", "vcResultQtyDaily25", "vcPlantQtyDaily26", "vcInputQtyDaily26",
 //"vcResultQtyDaily26", "vcPlantQtyDaily27", "vcInputQtyDaily27", "vcResultQtyDaily27", "vcPlantQtyDaily28", "vcInputQtyDaily28",
 //"vcResultQtyDaily28", "vcPlantQtyDaily29", "vcInputQtyDaily29", "vcResultQtyDaily29", "vcPlantQtyDaily30", "vcInputQtyDaily30",
 //"vcResultQtyDaily30", "vcPlantQtyDaily31", "vcInputQtyDaily31", "vcResultQtyDaily31","vcPlantQtyTotal","vcInputQtyTotal",
 //"vcResultQtyTotal", "vcCarType", "vcLastPartNo", "vcPackingSpot","vcTargetMonthFlag", "vcTargetMonthLast",
 
 //"vcOperatorID", "dOperatorTime"
                    if (bAddFlag == true)
                    {//新增
                        
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);


                        int vcPlantQtyDailySum = Convert.ToInt32(listInfoData[i]["vcPlantQtyDaily1"]) + Convert.ToInt32(listInfoData[i]["vcPlantQtyDaily2"])+ Convert.ToInt32(listInfoData[i]["vcPlantQtyDaily3"])+ Convert.ToInt32(listInfoData[i]["vcPlantQtyDaily4"])+
                            Convert.ToInt32(listInfoData[i]["vcPlantQtyDaily5"])+ Convert.ToInt32(listInfoData[i]["vcPlantQtyDaily6"])+ Convert.ToInt32(listInfoData[i]["vcPlantQtyDaily7"])+ Convert.ToInt32(listInfoData[i]["vcPlantQtyDaily8"])
                            +Convert.ToInt32(listInfoData[i]["vcPlantQtyDaily9"]) + Convert.ToInt32(listInfoData[i]["vcPlantQtyDaily10"]) + Convert.ToInt32(listInfoData[i]["vcPlantQtyDaily11"]) + Convert.ToInt32(listInfoData[i]["vcPlantQtyDaily12"]) + Convert.ToInt32(listInfoData[i]["vcPlantQtyDaily13"])
                            + Convert.ToInt32(listInfoData[i]["vcPlantQtyDaily14"]) + Convert.ToInt32(listInfoData[i]["vcPlantQtyDaily15"]) + Convert.ToInt32(listInfoData[i]["vcPlantQtyDaily16"]) + Convert.ToInt32(listInfoData[i]["vcPlantQtyDaily17"]) + Convert.ToInt32(listInfoData[i]["vcPlantQtyDaily18"])
                            + Convert.ToInt32(listInfoData[i]["vcPlantQtyDaily19"]) + Convert.ToInt32(listInfoData[i]["vcPlantQtyDaily20"]) + Convert.ToInt32(listInfoData[i]["vcPlantQtyDaily21"]) + Convert.ToInt32(listInfoData[i]["vcPlantQtyDaily22"]) + Convert.ToInt32(listInfoData[i]["vcPlantQtyDaily23"])
                            + Convert.ToInt32(listInfoData[i]["vcPlantQtyDaily24"]) + Convert.ToInt32(listInfoData[i]["vcPlantQtyDaily25"]) + Convert.ToInt32(listInfoData[i]["vcPlantQtyDaily26"]) + Convert.ToInt32(listInfoData[i]["vcPlantQtyDaily27"]) + Convert.ToInt32(listInfoData[i]["vcPlantQtyDaily28"])
                            + Convert.ToInt32(listInfoData[i]["vcPlantQtyDaily29"]) + Convert.ToInt32(listInfoData[i]["vcPlantQtyDaily30"]) + Convert.ToInt32(listInfoData[i]["vcPlantQtyDaily31"]);
                        int vcInputQtyDailySum = Convert.ToInt32(listInfoData[i]["vcInputQtyDaily1"]) + Convert.ToInt32(listInfoData[i]["vcInputQtyDaily2"]) + Convert.ToInt32(listInfoData[i]["vcInputQtyDaily3"]) + Convert.ToInt32(listInfoData[i]["vcInputQtyDaily4"]) +
                            Convert.ToInt32(listInfoData[i]["vcInputQtyDaily5"]) + Convert.ToInt32(listInfoData[i]["vcInputQtyDaily6"]) + Convert.ToInt32(listInfoData[i]["vcInputQtyDaily7"]) + Convert.ToInt32(listInfoData[i]["vcInputQtyDaily8"])
                            + Convert.ToInt32(listInfoData[i]["vcInputQtyDaily9"]) + Convert.ToInt32(listInfoData[i]["vcInputQtyDaily10"]) + Convert.ToInt32(listInfoData[i]["vcInputQtyDaily11"]) + Convert.ToInt32(listInfoData[i]["vcInputQtyDaily12"]) + Convert.ToInt32(listInfoData[i]["vcInputQtyDaily13"])
                            + Convert.ToInt32(listInfoData[i]["vcInputQtyDaily14"]) + Convert.ToInt32(listInfoData[i]["vcInputQtyDaily15"]) + Convert.ToInt32(listInfoData[i]["vcInputQtyDaily16"]) + Convert.ToInt32(listInfoData[i]["vcInputQtyDaily17"]) + Convert.ToInt32(listInfoData[i]["vcInputQtyDaily18"])
                            + Convert.ToInt32(listInfoData[i]["vcInputQtyDaily19"]) + Convert.ToInt32(listInfoData[i]["vcInputQtyDaily20"]) + Convert.ToInt32(listInfoData[i]["vcInputQtyDaily21"]) + Convert.ToInt32(listInfoData[i]["vcInputQtyDaily22"]) + Convert.ToInt32(listInfoData[i]["vcInputQtyDaily23"])
                            + Convert.ToInt32(listInfoData[i]["vcInputQtyDaily24"]) + Convert.ToInt32(listInfoData[i]["vcInputQtyDaily25"]) + Convert.ToInt32(listInfoData[i]["vcInputQtyDaily26"]) + Convert.ToInt32(listInfoData[i]["vcInputQtyDaily27"]) + Convert.ToInt32(listInfoData[i]["vcInputQtyDaily28"])
                            + Convert.ToInt32(listInfoData[i]["vcInputQtyDaily29"]) + Convert.ToInt32(listInfoData[i]["vcInputQtyDaily30"]) + Convert.ToInt32(listInfoData[i]["vcInputQtyDaily31"]);

                        int vcResultQtyDailySum = Convert.ToInt32(listInfoData[i]["vcResultQtyDaily1"]) + Convert.ToInt32(listInfoData[i]["vcResultQtyDaily2"]) + Convert.ToInt32(listInfoData[i]["vcResultQtyDaily3"]) + Convert.ToInt32(listInfoData[i]["vcResultQtyDaily4"]) +
                            Convert.ToInt32(listInfoData[i]["vcResultQtyDaily5"]) + Convert.ToInt32(listInfoData[i]["vcResultQtyDaily6"]) + Convert.ToInt32(listInfoData[i]["vcResultQtyDaily7"]) + Convert.ToInt32(listInfoData[i]["vcResultQtyDaily8"])
                           + Convert.ToInt32(listInfoData[i]["vcResultQtyDaily9"]) + Convert.ToInt32(listInfoData[i]["vcResultQtyDaily10"]) + Convert.ToInt32(listInfoData[i]["vcResultQtyDaily11"]) + Convert.ToInt32(listInfoData[i]["vcResultQtyDaily12"]) + Convert.ToInt32(listInfoData[i]["vcResultQtyDaily13"])
                           + Convert.ToInt32(listInfoData[i]["vcResultQtyDaily14"]) + Convert.ToInt32(listInfoData[i]["vcResultQtyDaily15"]) + Convert.ToInt32(listInfoData[i]["vcResultQtyDaily16"]) + Convert.ToInt32(listInfoData[i]["vcResultQtyDaily17"]) + Convert.ToInt32(listInfoData[i]["vcResultQtyDaily18"])
                           + Convert.ToInt32(listInfoData[i]["vcResultQtyDaily19"]) + Convert.ToInt32(listInfoData[i]["vcResultQtyDaily20"]) + Convert.ToInt32(listInfoData[i]["vcResultQtyDaily21"]) + Convert.ToInt32(listInfoData[i]["vcResultQtyDaily22"]) + Convert.ToInt32(listInfoData[i]["vcResultQtyDaily23"])
                           + Convert.ToInt32(listInfoData[i]["vcResultQtyDaily24"]) + Convert.ToInt32(listInfoData[i]["vcResultQtyDaily25"]) + Convert.ToInt32(listInfoData[i]["vcResultQtyDaily26"]) + Convert.ToInt32(listInfoData[i]["vcResultQtyDaily27"]) + Convert.ToInt32(listInfoData[i]["vcResultQtyDaily28"])
                           + Convert.ToInt32(listInfoData[i]["vcResultQtyDaily29"]) + Convert.ToInt32(listInfoData[i]["vcResultQtyDaily30"]) + Convert.ToInt32(listInfoData[i]["vcResultQtyDaily31"]);

                        sql.Append("  update SP_M_ORD set    \r\n");
                        sql.Append("  vcPlantQtyDaily1=" + getSqlValue(listInfoData[i]["vcPlantQtyDaily1"], true) + ",vcInputQtyDaily1=" + getSqlValue(listInfoData[i]["vcInputQtyDaily1"], true) + ",vcResultQtyDaily1=" + getSqlValue(listInfoData[i]["vcResultQtyDaily1"], true) + ",   \r\n");
                        sql.Append("  vcPlantQtyDaily2=" + getSqlValue(listInfoData[i]["vcPlantQtyDaily2"], true) + ",vcInputQtyDaily2=" + getSqlValue(listInfoData[i]["vcInputQtyDaily2"], true) + ",vcResultQtyDaily2=" + getSqlValue(listInfoData[i]["vcResultQtyDaily2"], true) + ",   \r\n");
                        sql.Append("  vcPlantQtyDaily3=" + getSqlValue(listInfoData[i]["vcPlantQtyDaily3"], true) + ",vcInputQtyDaily3=" + getSqlValue(listInfoData[i]["vcInputQtyDaily3"], true) + ",vcResultQtyDaily3=" + getSqlValue(listInfoData[i]["vcResultQtyDaily3"], true) + ",   \r\n");
                        sql.Append("  vcPlantQtyDaily4=" + getSqlValue(listInfoData[i]["vcPlantQtyDaily4"], true) + ",vcInputQtyDaily4=" + getSqlValue(listInfoData[i]["vcInputQtyDaily4"], true) + ",vcResultQtyDaily4=" + getSqlValue(listInfoData[i]["vcResultQtyDaily4"], true) + ",   \r\n");
                        sql.Append("  vcPlantQtyDaily5=" + getSqlValue(listInfoData[i]["vcPlantQtyDaily5"], true) + ",vcInputQtyDaily5=" + getSqlValue(listInfoData[i]["vcInputQtyDaily5"], true) + ",vcResultQtyDaily5=" + getSqlValue(listInfoData[i]["vcResultQtyDaily5"], true) + ",   \r\n");
                        sql.Append("  vcPlantQtyDaily6=" + getSqlValue(listInfoData[i]["vcPlantQtyDaily6"], true) + ",vcInputQtyDaily6=" + getSqlValue(listInfoData[i]["vcInputQtyDaily6"], true) + ",vcResultQtyDaily6=" + getSqlValue(listInfoData[i]["vcResultQtyDaily6"], true) + ",   \r\n");
                        sql.Append("  vcPlantQtyDaily7=" + getSqlValue(listInfoData[i]["vcPlantQtyDaily7"], true) + ",vcInputQtyDaily7=" + getSqlValue(listInfoData[i]["vcInputQtyDaily7"], true) + ",vcResultQtyDaily7=" + getSqlValue(listInfoData[i]["vcResultQtyDaily7"], true) + ",   \r\n");
                        sql.Append("  vcPlantQtyDaily8=" + getSqlValue(listInfoData[i]["vcPlantQtyDaily8"], true) + ",vcInputQtyDaily8=" + getSqlValue(listInfoData[i]["vcInputQtyDaily8"], true) + ",vcResultQtyDaily8=" + getSqlValue(listInfoData[i]["vcResultQtyDaily8"], true) + ",   \r\n");
                        sql.Append("  vcPlantQtyDaily9=" + getSqlValue(listInfoData[i]["vcPlantQtyDaily9"], true) + ",vcInputQtyDaily9=" + getSqlValue(listInfoData[i]["vcInputQtyDaily9"], true) + ",vcResultQtyDaily9=" + getSqlValue(listInfoData[i]["vcResultQtyDaily9"], true) + ",   \r\n");
                        sql.Append("  vcPlantQtyDaily10=" + getSqlValue(listInfoData[i]["vcPlantQtyDaily10"], true) + ",vcInputQtyDaily10=" + getSqlValue(listInfoData[i]["vcInputQtyDaily10"], true) + ",vcResultQtyDaily10=" + getSqlValue(listInfoData[i]["vcResultQtyDaily10"], true) + ",   \r\n");
                        sql.Append("  vcPlantQtyDaily11=" + getSqlValue(listInfoData[i]["vcPlantQtyDaily11"], true) + ",vcInputQtyDaily11=" + getSqlValue(listInfoData[i]["vcInputQtyDaily11"], true) + ",vcResultQtyDaily11=" + getSqlValue(listInfoData[i]["vcResultQtyDaily11"], true) + ",   \r\n");
                        sql.Append("  vcPlantQtyDaily12=" + getSqlValue(listInfoData[i]["vcPlantQtyDaily12"], true) + ",vcInputQtyDaily12=" + getSqlValue(listInfoData[i]["vcInputQtyDaily12"], true) + ",vcResultQtyDaily12=" + getSqlValue(listInfoData[i]["vcResultQtyDaily12"], true) + ",   \r\n");
                        sql.Append("  vcPlantQtyDaily13=" + getSqlValue(listInfoData[i]["vcPlantQtyDaily13"], true) + ",vcInputQtyDaily13=" + getSqlValue(listInfoData[i]["vcInputQtyDaily13"], true) + ",vcResultQtyDaily13=" + getSqlValue(listInfoData[i]["vcResultQtyDaily13"], true) + ",   \r\n");
                        sql.Append("  vcPlantQtyDaily14=" + getSqlValue(listInfoData[i]["vcPlantQtyDaily14"], true) + ",vcInputQtyDaily14=" + getSqlValue(listInfoData[i]["vcInputQtyDaily14"], true) + ",vcResultQtyDaily14=" + getSqlValue(listInfoData[i]["vcResultQtyDaily14"], true) + ",   \r\n");
                        sql.Append("  vcPlantQtyDaily15=" + getSqlValue(listInfoData[i]["vcPlantQtyDaily15"], true) + ",vcInputQtyDaily15=" + getSqlValue(listInfoData[i]["vcInputQtyDaily15"], true) + ",vcResultQtyDaily15=" + getSqlValue(listInfoData[i]["vcResultQtyDaily15"], true) + ",   \r\n");
                        sql.Append("  vcPlantQtyDaily16=" + getSqlValue(listInfoData[i]["vcPlantQtyDaily16"], true) + ",vcInputQtyDaily16=" + getSqlValue(listInfoData[i]["vcInputQtyDaily16"], true) + ",vcResultQtyDaily16=" + getSqlValue(listInfoData[i]["vcResultQtyDaily16"], true) + ",   \r\n");
                        sql.Append("  vcPlantQtyDaily17=" + getSqlValue(listInfoData[i]["vcPlantQtyDaily17"], true) + ",vcInputQtyDaily17=" + getSqlValue(listInfoData[i]["vcInputQtyDaily17"], true) + ",vcResultQtyDaily17=" + getSqlValue(listInfoData[i]["vcResultQtyDaily17"], true) + ",   \r\n");
                        sql.Append("  vcPlantQtyDaily18=" + getSqlValue(listInfoData[i]["vcPlantQtyDaily18"], true) + ",vcInputQtyDaily18=" + getSqlValue(listInfoData[i]["vcInputQtyDaily18"], true) + ",vcResultQtyDaily18=" + getSqlValue(listInfoData[i]["vcResultQtyDaily18"], true) + ",   \r\n");
                        sql.Append("  vcPlantQtyDaily19=" + getSqlValue(listInfoData[i]["vcPlantQtyDaily19"], true) + ",vcInputQtyDaily19=" + getSqlValue(listInfoData[i]["vcInputQtyDaily19"], true) + ",vcResultQtyDaily19=" + getSqlValue(listInfoData[i]["vcResultQtyDaily19"], true) + ",   \r\n");
                        sql.Append("  vcPlantQtyDaily20=" + getSqlValue(listInfoData[i]["vcPlantQtyDaily20"], true) + ",vcInputQtyDaily20=" + getSqlValue(listInfoData[i]["vcInputQtyDaily20"], true) + ",vcResultQtyDaily20=" + getSqlValue(listInfoData[i]["vcResultQtyDaily20"], true) + ",   \r\n");
                        sql.Append("  vcPlantQtyDaily21=" + getSqlValue(listInfoData[i]["vcPlantQtyDaily21"], true) + ",vcInputQtyDaily21=" + getSqlValue(listInfoData[i]["vcInputQtyDaily21"], true) + ",vcResultQtyDaily21=" + getSqlValue(listInfoData[i]["vcResultQtyDaily21"], true) + ",   \r\n");
                        sql.Append("  vcPlantQtyDaily22=" + getSqlValue(listInfoData[i]["vcPlantQtyDaily22"], true) + ",vcInputQtyDaily22=" + getSqlValue(listInfoData[i]["vcInputQtyDaily22"], true) + ",vcResultQtyDaily22=" + getSqlValue(listInfoData[i]["vcResultQtyDaily22"], true) + ",   \r\n");
                        sql.Append("  vcPlantQtyDaily23=" + getSqlValue(listInfoData[i]["vcPlantQtyDaily23"], true) + ",vcInputQtyDaily23=" + getSqlValue(listInfoData[i]["vcInputQtyDaily23"], true) + ",vcResultQtyDaily23=" + getSqlValue(listInfoData[i]["vcResultQtyDaily23"], true) + ",   \r\n");
                        sql.Append("  vcPlantQtyDaily24=" + getSqlValue(listInfoData[i]["vcPlantQtyDaily24"], true) + ",vcInputQtyDaily24=" + getSqlValue(listInfoData[i]["vcInputQtyDaily24"], true) + ",vcResultQtyDaily24=" + getSqlValue(listInfoData[i]["vcResultQtyDaily24"], true) + ",   \r\n");
                        sql.Append("  vcPlantQtyDaily25=" + getSqlValue(listInfoData[i]["vcPlantQtyDaily25"], true) + ",vcInputQtyDaily25=" + getSqlValue(listInfoData[i]["vcInputQtyDaily25"], true) + ",vcResultQtyDaily25=" + getSqlValue(listInfoData[i]["vcResultQtyDaily25"], true) + ",   \r\n");
                        sql.Append("  vcPlantQtyDaily26=" + getSqlValue(listInfoData[i]["vcPlantQtyDaily26"], true) + ",vcInputQtyDaily26=" + getSqlValue(listInfoData[i]["vcInputQtyDaily26"], true) + ",vcResultQtyDaily26=" + getSqlValue(listInfoData[i]["vcResultQtyDaily26"], true) + ",   \r\n");
                        sql.Append("  vcPlantQtyDaily27=" + getSqlValue(listInfoData[i]["vcPlantQtyDaily27"], true) + ",vcInputQtyDaily27=" + getSqlValue(listInfoData[i]["vcInputQtyDaily27"], true) + ",vcResultQtyDaily27=" + getSqlValue(listInfoData[i]["vcResultQtyDaily27"], true) + ",   \r\n");
                        sql.Append("  vcPlantQtyDaily28=" + getSqlValue(listInfoData[i]["vcPlantQtyDaily28"], true) + ",vcInputQtyDaily28=" + getSqlValue(listInfoData[i]["vcInputQtyDaily28"], true) + ",vcResultQtyDaily28=" + getSqlValue(listInfoData[i]["vcResultQtyDaily28"], true) + ",   \r\n");
                        sql.Append("  vcPlantQtyDaily29=" + getSqlValue(listInfoData[i]["vcPlantQtyDaily29"], true) + ",vcInputQtyDaily29=" + getSqlValue(listInfoData[i]["vcInputQtyDaily29"], true) + ",vcResultQtyDaily29=" + getSqlValue(listInfoData[i]["vcResultQtyDaily29"], true) + ",   \r\n");
                        sql.Append("  vcPlantQtyDaily30=" + getSqlValue(listInfoData[i]["vcPlantQtyDaily30"], true) + ",vcInputQtyDaily30=" + getSqlValue(listInfoData[i]["vcInputQtyDaily30"], true) + ",vcResultQtyDaily30=" + getSqlValue(listInfoData[i]["vcResultQtyDaily30"], true) + ",   \r\n");
                        sql.Append("  vcPlantQtyDaily31=" + getSqlValue(listInfoData[i]["vcPlantQtyDaily31"], true) + ",vcInputQtyDaily31=" + getSqlValue(listInfoData[i]["vcInputQtyDaily31"], true) + ",vcResultQtyDaily31=" + getSqlValue(listInfoData[i]["vcResultQtyDaily31"], true) + ",   \r\n");
                        sql.Append("  vcPlantQtyDailySum=" + vcPlantQtyDailySum + ",vcInputQtyDailySum=" + vcInputQtyDailySum + ",vcResultQtyDailySum=" + vcResultQtyDailySum + ",   \r\n");
                        sql.Append("  vcOperatorID='" + userId + "',dOperatorTime=GETDATE() \r\n");
                        sql.Append(" where iAutoId=" + iAutoId + " ;  \n");

                    }
                }

                if (sql.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(sql.ToString());
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
                sql.Append("  delete [SP_M_ORD] where iAutoId in(   \r\n ");
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
        /// 一括赋予
        /// </summary>
        /// <returns></returns>
        public void allInstall(List<Dictionary<string, object>> listInfoData, DateTime dBeginDate, DateTime dEndDate, string userId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();

                sql.Append("update TSpecialSupplier set dBeginDate='" + dBeginDate + "', dEndDate='" + dEndDate + "',vcOperatorID='" + userId + "',dOperatorTime=GETDATE() where iAutoId in( \n");

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
        /// 导入 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="strUserId"></param>
        public void importSave(DataTable dt, object strUserId)
        {
            try
            {

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //[vcSupplier_id], [vcWorkArea], [dBeginDate], [dEndDate],, 

                    StringBuilder strSql = new StringBuilder();
                    string vcSupplier_id = dt.Rows[i]["vcSupplier_id"] == System.DBNull.Value ? "" : dt.Rows[i]["vcSupplier_id"].ToString();
                    string vcWorkArea = dt.Rows[i]["vcWorkArea"] == System.DBNull.Value ? "" : dt.Rows[i]["vcWorkArea"].ToString();
                    DateTime dBeginDate = Convert.ToDateTime(dt.Rows[i]["dBeginDate"].ToString());
                    DateTime dEndDate = Convert.ToDateTime(dt.Rows[i]["dEndDate"].ToString());

                    strSql.AppendLine("  declare @isExist int =0;   ");
                    strSql.AppendLine("  select @isExist=COUNT(*) from TSpecialSupplier where vcSupplier_id='" + vcSupplier_id + "' and vcWorkArea='" + vcWorkArea + "' ");
                    strSql.AppendLine("     ");
                    strSql.AppendLine("  if @isExist>0   ");
                    strSql.AppendLine("  begin   ");
                    strSql.AppendLine("  		update TSpecialSupplier set dBeginDate = '" + dBeginDate + "',dEndDate='" + dEndDate + "',  ");
                    strSql.AppendLine("  		vcOperatorID='" + strUserId + "',dOperatorTime=GETDATE() where vcSupplier_id='" + vcSupplier_id + "' and vcWorkArea='" + vcWorkArea + "' ;  ");
                    strSql.AppendLine("  end   ");
                    strSql.AppendLine("  else   ");
                    strSql.AppendLine("  begin   ");
                    strSql.AppendLine("  		insert into dbo.TSpecialSupplier (vcSupplier_id, vcWorkArea, dBeginDate, dEndDate, ");
                    strSql.AppendLine("  		 vcOperatorID, dOperatorTime )    ");
                    strSql.AppendLine("  		values   ");
                    strSql.AppendLine("  		('" + vcSupplier_id + "','" + vcWorkArea + "','" + dBeginDate + "','" + dEndDate + "','" + strUserId + "',GETDATE()) ;   ");
                    strSql.AppendLine("  end ;  ");
                    strSql.AppendLine("     ");

                    excute.ExcuteSqlWithStringOper(strSql.ToString());
                }

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
    }
}
