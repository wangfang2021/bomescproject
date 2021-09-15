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
    public class FS0629_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        /// <summary>
        /// 验证新增的数据是否已经存在数据库
        /// </summary>
        /// <param name="dtadd"></param>
        /// <returns></returns>
        public bool isExistAddData(DataTable dtadd)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                for (int i = 0; i < dtadd.Rows.Count; i++)
                {
                    if (strSql.Length > 0)
                    {
                        strSql.AppendLine("  union all select iAutoId, vcSupplier_id, vcWorkArea, dBeginDate, dEndDate, vcOperatorID, dOperatorTime from [dbo].[TSpecialSupplier] where vcSupplier_id='" + dtadd.Rows[i]["vcSupplier_id"] + "' and  vcWorkArea='" + dtadd.Rows[i]["vcWorkArea"] + "' ");
                    }
                    else
                    {
                        strSql.AppendLine("  select iAutoId, vcSupplier_id, vcWorkArea, dBeginDate, dEndDate, vcOperatorID, dOperatorTime from [dbo].[TSpecialSupplier] where vcSupplier_id='" + dtadd.Rows[i]["vcSupplier_id"] + "' and  vcWorkArea='" + dtadd.Rows[i]["vcWorkArea"] + "'  ");
                    }
                }
                DataTable dt = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                return dt.Rows.Count > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable getDataChuRuKuByTargetMonth(string vcTargetMonth)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("  select [iAutoId], [vcPackingFactory], left([vcTargetYearMonth],4)+'/'+right([vcTargetYearMonth],2) as [vcTargetYearMonth], [vcDock],a.vcCpdcompany as [vcCpdcompany],b.vcName as [vcOrderType],    ");
                strSql.AppendLine("  [vcOrderNo], [vcSeqno],convert(varchar(10), dOrderDate,111) as [dOrderDate],convert(varchar(10), dOrderExportDate,111) as [dOrderExportDate], [vcPartNo],d.vcName as [vcInsideOutsideType],    ");
                strSql.AppendLine("  [vcCarType], [vcLastPartNo], [vcPackingSpot], [vcSupplier_id], cast(isnull(a.vcPlantQtyDaily1,0) as int) as [vcPlantQtyDaily1],cast(isnull(a.[vcInputQtyDaily1],0) as int) as [vcInputQtyDaily1],      ");
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
                strSql.AppendLine("  cast(isnull(a.[vcPlantQtyDailySum],0) as int) as [vcPlantQtyDailySum],cast(isnull(a.[vcInputQtyDailySum],0) as int) as [vcInputQtyDailySum],cast(isnull(a.[vcResultQtyDailySum],0) as int) as [vcResultQtyDailySum],'0' as vcModFlag,'0' as vcAddFlag, case when [vcTargetMonthFlag]='0' then '0:确定月' else vcTargetMonthFlag  end as vcTargetMonthFlag,convert(varchar(10), vcTargetMonthLast,111) as [vcTargetMonthLast], [vcOperatorID], [dOperatorTime]    ");
                strSql.AppendLine("  from [dbo].[SP_M_ORD] a    ");
                strSql.AppendLine("  left join (select vcOrderInitials as vcValue,vcOrderDifferentiation as vcName from TOrderDifferentiation) b on a.vcOrderType = b.vcValue   ");
                strSql.AppendLine("  left join (select vcValue,vcName from TCode where  vcCodeId='C018') c on a.vcCpdcompany = c.vcValue   ");
                strSql.AppendLine("  left join (select vcValue,vcName from TCode where  vcCodeId='C003') d on a.vcInsideOutsideType = d.vcValue where 1=1    ");
                if (vcTargetMonth.Length > 0)
                {
                    strSql.AppendLine("  and  a.vcTargetYearMonth = '" + vcTargetMonth.Replace("-", "").Replace("/", "") + "' ");
                }
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取千品数据
        /// </summary>
        /// <param name="vcConsignee"></param>
        /// <param name="vcInjectionFactory"></param>
        /// <param name="vcTargetMonth"></param>
        /// <returns></returns>
        public DataSet GetQianPin(string vcReceiver, string vcOrderPlant, string vcTargetYearMonth)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("  select * from ( ");
                strSql.AppendLine("  select chu1.vcReceiver,chu1.vcOrderPlant,chu1.vcClassType,chu1.vcSupplierId, ");
                //增加排序start 20210902
                strSql.AppendLine("case when chu1.vcSupplierId ='A' then 'A'");
                strSql.AppendLine("when chu1.vcSupplierId ='R' then 'R'");
                strSql.AppendLine("when chu1.vcSupplierId ='W' then 'W'");
                strSql.AppendLine("else 'Z' end as vccode,");
                //增加排序end 20200902
                strSql.AppendLine("  chu2.partNum,chu1.qianPinSum from ( ");
                strSql.AppendLine("  select a.vcReceiver,a.vcOrderPlant,a.vcClassType,a.vcSupplierId,sum(chaZhiNaRu) as qianPinSum from  ");
                strSql.AppendLine("  ( ");
                strSql.AppendLine("  select vcReceiver,vcOrderPlant, '纳入' as vcClassType, ");
                strSql.AppendLine("  case when type='外注' then vcSupplier_id+isnull(vcWorkArea,'') ");
                strSql.AppendLine("  else type end as vcSupplierId, ");
                strSql.AppendLine("  vcPartNo,vcPlantQtyTotal-vcInputQtyTotal as chaZhiNaRu ");
                strSql.AppendLine("   from [dbo].[VI_SP_M_ORD_S0629]  ");
                strSql.AppendLine("   where vcTargetYearMonth='"+vcTargetYearMonth+ "' and vcReceiver is not null and (vcPlantQtyTotal-vcInputQtyTotal)>0 ");
                if (vcReceiver.Length>0)
                {
                    strSql.AppendLine("  and vcReceiver='"+vcReceiver+"' ");
                }
                if (vcOrderPlant.Length > 0)
                {
                    strSql.AppendLine("  and vcOrderPlant='" + vcOrderPlant + "' ");
                }
                strSql.AppendLine("   ) a  ");
                strSql.AppendLine("   group by a.vcReceiver,a.vcOrderPlant,a.vcClassType,a.vcSupplierId ");
                strSql.AppendLine("  ) chu1  ");
                strSql.AppendLine("  left join ( ");
                strSql.AppendLine("   select t.vcReceiver,t.vcOrderPlant,t.vcClassType,t.vcSupplierId,count(*) as partNum from ( ");
                strSql.AppendLine("   select distinct vcReceiver,vcOrderPlant,vcClassType,vcSupplierId,vcPartNo from ( ");
                strSql.AppendLine("   select vcReceiver,vcOrderPlant, '纳入' as vcClassType, ");
                strSql.AppendLine("  case when type='外注' then vcSupplier_id+isnull(vcWorkArea,'') ");
                strSql.AppendLine("  else type end as vcSupplierId, ");
                strSql.AppendLine("  vcPartNo,vcPlantQtyTotal-vcInputQtyTotal as chaZhiNaRu ");
                strSql.AppendLine("   from [dbo].[VI_SP_M_ORD_S0629]  ");
                strSql.AppendLine("   where vcTargetYearMonth='"+vcTargetYearMonth+ "' and vcReceiver is not null and (vcPlantQtyTotal-vcInputQtyTotal)>0 ");
                if (vcReceiver.Length > 0)
                {
                    strSql.AppendLine("  and vcReceiver='" + vcReceiver + "' ");
                }
                if (vcOrderPlant.Length > 0)
                {
                    strSql.AppendLine("  and vcOrderPlant='" + vcOrderPlant + "' ");
                }
                strSql.AppendLine("   ) c ");
                strSql.AppendLine("   ) t group by t.vcReceiver,t.vcOrderPlant,t.vcClassType,t.vcSupplierId ");
                strSql.AppendLine("   ) chu2 on chu1.vcReceiver = chu2.vcReceiver and chu1.vcOrderPlant = chu2.vcOrderPlant ");
                strSql.AppendLine("   and chu1.vcClassType = chu2.vcClassType and chu1.vcSupplierId = chu2.vcSupplierId ");
                strSql.AppendLine("   union ");
                strSql.AppendLine("   ----chuhe--- ");
                //增加排序start20210902
                //strSql.AppendLine("   select ru1.vcReceiver,ru1.vcOrderPlant,ru1.vcClassType,ru1.vcSupplierId,ru2.partNum,ru1.qianPinSum from ( ");
                strSql.AppendLine("   select ru1.vcReceiver,ru1.vcOrderPlant,ru1.vcClassType,ru1.vcSupplierId,");
                strSql.AppendLine("case when ru1.vcSupplierId ='A' then 'A'");
                strSql.AppendLine("when ru1.vcSupplierId ='R' then 'R'");
                strSql.AppendLine("when ru1.vcSupplierId ='W' then 'W'");
                strSql.AppendLine("else 'Z' end as vccode,");
                strSql.AppendLine("ru2.partNum,ru1.qianPinSum from ( ");
                //增加排序end20210902

                strSql.AppendLine("  select a.vcReceiver,a.vcOrderPlant,a.vcClassType,a.vcSupplierId,sum(chaZhiNaRu) as qianPinSum from  ");
                strSql.AppendLine("  ( ");
                strSql.AppendLine("  select vcReceiver,vcOrderPlant, '出荷' as vcClassType, ");
                strSql.AppendLine("  case when type='外注' then vcSupplier_id+isnull(vcWorkArea,'') ");
                strSql.AppendLine("  else type end as vcSupplierId, ");
                strSql.AppendLine("  vcPartNo,vcInputQtyTotal-vcResultQtyTotal as chaZhiNaRu ");
                strSql.AppendLine("   from [dbo].[VI_SP_M_ORD_S0629]  ");
                strSql.AppendLine("   where vcTargetYearMonth='"+vcTargetYearMonth+ "' and vcReceiver is not null and (vcInputQtyTotal-vcResultQtyTotal)>0 ");
                if (vcReceiver.Length > 0)
                {
                    strSql.AppendLine("  and vcReceiver='" + vcReceiver + "' ");
                }
                if (vcOrderPlant.Length > 0)
                {
                    strSql.AppendLine("  and vcOrderPlant='" + vcOrderPlant + "' ");
                }
                strSql.AppendLine("   ) a  ");
                strSql.AppendLine("   group by a.vcReceiver,a.vcOrderPlant,a.vcClassType,a.vcSupplierId ");
                strSql.AppendLine("  ) ru1  ");
                strSql.AppendLine("  left join ( ");
                strSql.AppendLine("   select t.vcReceiver,t.vcOrderPlant,t.vcClassType,t.vcSupplierId,count(*) as partNum from ( ");
                strSql.AppendLine("   select distinct vcReceiver,vcOrderPlant,vcClassType,vcSupplierId,vcPartNo from ( ");
                strSql.AppendLine("   select vcReceiver,vcOrderPlant, '出荷' as vcClassType, ");
                strSql.AppendLine("  case when type='外注' then vcSupplier_id+isnull(vcWorkArea,'') ");
                strSql.AppendLine("  else type end as vcSupplierId, ");
                strSql.AppendLine("  vcPartNo,vcInputQtyTotal-vcResultQtyTotal as chaZhiNaRu ");
                strSql.AppendLine("   from [dbo].[VI_SP_M_ORD_S0629]  ");
                strSql.AppendLine("   where vcTargetYearMonth='"+vcTargetYearMonth+ "' and vcReceiver is not null  and (vcInputQtyTotal-vcResultQtyTotal)>0 ");
                if (vcReceiver.Length > 0)
                {
                    strSql.AppendLine("  and vcReceiver='" + vcReceiver + "' ");
                }
                if (vcOrderPlant.Length > 0)
                {
                    strSql.AppendLine("  and vcOrderPlant='" + vcOrderPlant + "' ");
                }
                strSql.AppendLine("   ) c ");
                strSql.AppendLine("   ) t group by t.vcReceiver,t.vcOrderPlant,t.vcClassType,t.vcSupplierId ");
                strSql.AppendLine("   ) ru2 on ru1.vcReceiver = ru2.vcReceiver and ru1.vcOrderPlant = ru2.vcOrderPlant ");
                strSql.AppendLine("   and ru1.vcClassType = ru2.vcClassType and ru1.vcSupplierId = ru2.vcSupplierId ");
                //strSql.AppendLine("   ) huiZong order by huiZong.vcReceiver,huiZong.vcOrderPlant,huiZong.vcClassType desc ");
                strSql.AppendLine("   ) huiZong order by huiZong.vcReceiver,huiZong.vcOrderPlant,huiZong.vcClassType desc,vccode "); //增加排序
                strSql.AppendLine("   ");
                strSql.AppendLine("   ");
                strSql.AppendLine("   select huiZong.vcReceiver,huiZong.vcOrderPlant,'合计' as vcClassType, ");
                strSql.AppendLine("   sum(partNum) as partNum ,sum(qianPinSum) as qianPinSum from ( ");
                strSql.AppendLine("  select chu1.vcReceiver,chu1.vcOrderPlant,chu1.vcClassType,chu1.vcSupplierId,chu2.partNum,chu1.qianPinSum from ( ");
                strSql.AppendLine("  select a.vcReceiver,a.vcOrderPlant,a.vcClassType,a.vcSupplierId,sum(chaZhiNaRu) as qianPinSum from  ");
                strSql.AppendLine("  ( ");
                strSql.AppendLine("  select vcReceiver,vcOrderPlant, '纳入' as vcClassType, ");
                strSql.AppendLine("  case when type='外注' then vcSupplier_id+isnull(vcWorkArea,'') ");
                strSql.AppendLine("  else type end as vcSupplierId, ");
                strSql.AppendLine("  vcPartNo,vcPlantQtyTotal-vcInputQtyTotal as chaZhiNaRu ");
                strSql.AppendLine("   from [dbo].[VI_SP_M_ORD_S0629]  ");
                strSql.AppendLine("   where vcTargetYearMonth='"+vcTargetYearMonth+ "' and vcReceiver is not null  ");
                if (vcReceiver.Length > 0)
                {
                    strSql.AppendLine("  and vcReceiver='" + vcReceiver + "' ");
                }
                if (vcOrderPlant.Length > 0)
                {
                    strSql.AppendLine("  and vcOrderPlant='" + vcOrderPlant + "' ");
                }
                strSql.AppendLine("    and (vcPlantQtyTotal-vcInputQtyTotal)>0 ");
                strSql.AppendLine("   ) a  ");
                strSql.AppendLine("   group by a.vcReceiver,a.vcOrderPlant,a.vcClassType,a.vcSupplierId ");
                strSql.AppendLine("  ) chu1  ");
                strSql.AppendLine("  left join ( ");
                strSql.AppendLine("   select t.vcReceiver,t.vcOrderPlant,t.vcClassType,t.vcSupplierId,count(*) as partNum from ( ");
                strSql.AppendLine("   select distinct vcReceiver,vcOrderPlant,vcClassType,vcSupplierId,vcPartNo from ( ");
                strSql.AppendLine("   select vcReceiver,vcOrderPlant, '纳入' as vcClassType, ");
                strSql.AppendLine("  case when type='外注' then vcSupplier_id+isnull(vcWorkArea,'') ");
                strSql.AppendLine("  else type end as vcSupplierId, ");
                strSql.AppendLine("  vcPartNo,vcPlantQtyTotal-vcInputQtyTotal as chaZhiNaRu ");
                strSql.AppendLine("   from [dbo].[VI_SP_M_ORD_S0629]  ");
                strSql.AppendLine("   where vcTargetYearMonth='"+vcTargetYearMonth+ "' and vcReceiver is not null  and (vcPlantQtyTotal-vcInputQtyTotal)>0 ");
                if (vcReceiver.Length > 0)
                {
                    strSql.AppendLine("  and vcReceiver='" + vcReceiver + "' ");
                }
                if (vcOrderPlant.Length > 0)
                {
                    strSql.AppendLine("  and vcOrderPlant='" + vcOrderPlant + "' ");
                }
                strSql.AppendLine("   ) c ");
                strSql.AppendLine("   ) t group by t.vcReceiver,t.vcOrderPlant,t.vcClassType,t.vcSupplierId ");
                strSql.AppendLine("   ) chu2 on chu1.vcReceiver = chu2.vcReceiver and chu1.vcOrderPlant = chu2.vcOrderPlant ");
                strSql.AppendLine("   and chu1.vcClassType = chu2.vcClassType and chu1.vcSupplierId = chu2.vcSupplierId ");
                strSql.AppendLine("   union ");
                strSql.AppendLine("   ----chuhe--- ");
                strSql.AppendLine("   select ru1.vcReceiver,ru1.vcOrderPlant,ru1.vcClassType,ru1.vcSupplierId,ru2.partNum,ru1.qianPinSum from ( ");
                strSql.AppendLine("  select a.vcReceiver,a.vcOrderPlant,a.vcClassType,a.vcSupplierId,sum(chaZhiNaRu) as qianPinSum from  ");
                strSql.AppendLine("  ( ");
                strSql.AppendLine("  select vcReceiver,vcOrderPlant, '出荷' as vcClassType, ");
                strSql.AppendLine("  case when type='外注' then vcSupplier_id+isnull(vcWorkArea,'') ");
                strSql.AppendLine("  else type end as vcSupplierId, ");
                strSql.AppendLine("  vcPartNo,vcInputQtyTotal-vcResultQtyTotal as chaZhiNaRu ");
                strSql.AppendLine("   from [dbo].[VI_SP_M_ORD_S0629]  ");
                strSql.AppendLine("   where vcTargetYearMonth='"+vcTargetYearMonth+ "' and vcReceiver is not null and (vcInputQtyTotal-vcResultQtyTotal)>0 ");
                if (vcReceiver.Length > 0)
                {
                    strSql.AppendLine("  and vcReceiver='" + vcReceiver + "' ");
                }
                if (vcOrderPlant.Length > 0)
                {
                    strSql.AppendLine("  and vcOrderPlant='" + vcOrderPlant + "' ");
                }
                strSql.AppendLine("   ) a  ");
                strSql.AppendLine("   group by a.vcReceiver,a.vcOrderPlant,a.vcClassType,a.vcSupplierId ");
                strSql.AppendLine("  ) ru1  ");
                strSql.AppendLine("  left join ( ");
                strSql.AppendLine("   select t.vcReceiver,t.vcOrderPlant,t.vcClassType,t.vcSupplierId,count(*) as partNum from ( ");
                strSql.AppendLine("   select distinct vcReceiver,vcOrderPlant,vcClassType,vcSupplierId,vcPartNo from ( ");
                strSql.AppendLine("   select vcReceiver,vcOrderPlant, '出荷' as vcClassType, ");
                strSql.AppendLine("  case when type='外注' then vcSupplier_id+isnull(vcWorkArea,'') ");
                strSql.AppendLine("  else type end as vcSupplierId, ");
                strSql.AppendLine("  vcPartNo,vcInputQtyTotal-vcResultQtyTotal as chaZhiNaRu ");
                strSql.AppendLine("   from [dbo].[VI_SP_M_ORD_S0629]  ");
                strSql.AppendLine("   where vcTargetYearMonth='"+vcTargetYearMonth+ "' and vcReceiver is not null and (vcInputQtyTotal-vcResultQtyTotal)>0 ");
                if (vcReceiver.Length > 0)
                {
                    strSql.AppendLine("  and vcReceiver='" + vcReceiver + "' ");
                }
                if (vcOrderPlant.Length > 0)
                {
                    strSql.AppendLine("  and vcOrderPlant='" + vcOrderPlant + "' ");
                }
                strSql.AppendLine("   ) c ");
                strSql.AppendLine("   ) t group by t.vcReceiver,t.vcOrderPlant,t.vcClassType,t.vcSupplierId ");
                strSql.AppendLine("   ) ru2 on ru1.vcReceiver = ru2.vcReceiver and ru1.vcOrderPlant = ru2.vcOrderPlant ");
                strSql.AppendLine("   and ru1.vcClassType = ru2.vcClassType and ru1.vcSupplierId = ru2.vcSupplierId ");
                strSql.AppendLine("   ) huiZong group by huiZong.vcReceiver,huiZong.vcOrderPlant ");
                strSql.AppendLine("    order by huiZong.vcReceiver,huiZong.vcOrderPlant desc ");

                return excute.ExcuteSqlWithSelectToDS(strSql.ToString());
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
         public DataSet Search(string vcConsignee, string vcInjectionFactory, string vcTargetMonth,string vcLastTargetMonth)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                //strSql.AppendLine("   select currentMonth.收货方,currentMonth.工场,currentMonth.工程,currentMonth.月度订单,   ");
                //strSql.AppendLine("   currentMonth.紧急订单,currentMonth.[应纳合计(A)],currentMonth.月度订单纳入实绩,   ");
                //strSql.AppendLine("   currentMonth.紧急订单纳入实绩,currentMonth.[纳入合计(B)],currentMonth.纳入率(B/A),   ");
                //strSql.AppendLine("   isnull(lastMonth.纳入率(B/A),'0.00%') as 前月纳入率,currentMonth.月度订单出荷实绩,   ");
                //strSql.AppendLine("   currentMonth.紧急订单出荷实绩,currentMonth.[出荷合计(D)],currentMonth.出荷率(D/A),   ");
                //strSql.AppendLine("   isnull(lastMonth.出荷率(D/A),'0.00%') as '前月出荷率' from (   ");
                strSql.AppendLine("   select currentMonth.收货方,currentMonth.工场,currentMonth.工程,currentMonth.月度订单,      ");
                strSql.AppendLine("     currentMonth.紧急订单,currentMonth.[应纳合计(A)],currentMonth.月度订单纳入实绩,      ");
                strSql.AppendLine("     currentMonth.紧急订单纳入实绩,currentMonth.[纳入合计(B)],   ");
                strSql.AppendLine("     '' as  '纳入率(B/A)','' as 前月纳入率,   ");
                strSql.AppendLine("     currentMonth.月度订单出荷实绩,      ");
                strSql.AppendLine("     currentMonth.紧急订单出荷实绩,currentMonth.[出荷合计(D)],     ");
                strSql.AppendLine("      '' as  '出荷率(D/A)','' as 前月出荷率,   ");
                strSql.AppendLine("   lastMonth.月度订单 as lastYuDuOrder,lastMonth.紧急订单 as lastJinJiOrder,   ");
                strSql.AppendLine("   lastMonth.月度订单纳入实绩 as lastYuDuNRSJ,lastMonth.紧急订单纳入实绩 as lastJinJiNRSJ,	 ");
                strSql.AppendLine("   lastMonth.月度订单出荷实绩 as lastYuDuCHSJ,lastMonth.紧急订单出荷实绩 as lastJinJiCHSJ   ");
                strSql.AppendLine("    from (   ");
                strSql.AppendLine("   select a.vcReceiver as '收货方',a.vcOrderPlant as '工场',a.item as '工程',   ");
                strSql.AppendLine("   isnull(b.月度订单,0) as '月度订单' ,isnull(c.紧急订单,0) as '紧急订单',   ");
                strSql.AppendLine("   isnull(b.月度订单,0)+isnull(c.紧急订单,0) as '应纳合计(A)',   ");
                strSql.AppendLine("   isnull(b.月度订单纳入实绩,0) as '月度订单纳入实绩',isnull(c.紧急订单纳入实绩,0) as '紧急订单纳入实绩',   ");
                strSql.AppendLine("   isnull(b.月度订单纳入实绩,0)+isnull(c.紧急订单纳入实绩,0) as '纳入合计(B)',   ");
                strSql.AppendLine("   case when isnull(b.月度订单,0)+isnull(c.紧急订单,0)=0  then  '0.00%'   ");
                strSql.AppendLine("       else   ");
                strSql.AppendLine("   convert(varchar(10),convert(decimal(18,2), (isnull(b.月度订单纳入实绩,0)+isnull(c.紧急订单纳入实绩,0))/(isnull(b.月度订单,0)+isnull(c.紧急订单,0))    ");
                strSql.AppendLine("    ) *100)+'%' end  as '纳入率（B/A）',   ");
                strSql.AppendLine("    isnull(b.月度订单出荷实绩,0) as '月度订单出荷实绩',isnull(c.紧急订单出荷实绩,0) as '紧急订单出荷实绩',   ");
                strSql.AppendLine("     isnull(b.月度订单出荷实绩,0)+isnull(c.紧急订单出荷实绩,0) as '出荷合计(D)',   ");
                strSql.AppendLine("   case when isnull(b.月度订单,0)+isnull(c.紧急订单,0)=0  then  '0.00%'   ");
                strSql.AppendLine("       else   ");
                strSql.AppendLine("    convert(varchar(10),convert(decimal(18,2),(isnull(b.月度订单出荷实绩,0)+isnull(c.紧急订单出荷实绩,0))/(isnull(b.月度订单,0)+isnull(c.紧急订单,0))   ");
                strSql.AppendLine("    ) *100)+'%'end as '出荷率(D/A)'   ");

                strSql.AppendLine("    from (   ");
                strSql.AppendLine("   select distinct vcReceiver,vcOrderPlant,vcOrderPlantCode,Type as item from [dbo].[VI_SP_M_OrderData] where  vcReceiver is not null and vcTargetYearMonth='" + vcTargetMonth + "'     ");
                if (vcConsignee.Length > 0)
                {
                    strSql.AppendLine("  and  vcReceiver = '" + vcConsignee + "' ");
                }
                if (vcInjectionFactory.Length > 0)
                {
                    strSql.AppendLine("  and  vcOrderPlantCode = '" + vcInjectionFactory + "' ");
                }
                strSql.AppendLine("   ) a   ");
                strSql.AppendLine("   left join   ");
                strSql.AppendLine("   (   ");
                strSql.AppendLine("   select s.vcReceiver,s.vcOrderPlant,s.item,   ");
                //strSql.AppendLine("   case when  s.vcOrderType  in  ('S','W','D') then '月度订单'   ");
                //strSql.AppendLine("   else  '紧急订单' end type1,   ");
                strSql.AppendLine("   '月度订单' as type1,   ");
                strSql.AppendLine("   sum(vcPlantQtyTotal) as '月度订单',sum(vcInputQtyTotal) as '月度订单纳入实绩' ,sum(vcResultQtyTotal) as '月度订单出荷实绩'   ");
                strSql.AppendLine("   from (   ");
                strSql.AppendLine("   select vcReceiver, vcOrderPlant, Type as item, vcOrderType,   ");
                strSql.AppendLine("   vcPlantQtyTotal, vcInputQtyTotal, vcResultQtyTotal    ");
                strSql.AppendLine("   from [dbo].[VI_SP_M_OrderData] where vcTargetYearMonth='"+ vcTargetMonth + "' and vcOrderType  in  ('S','W','D')   ");
                strSql.AppendLine("   ) S  group by s.vcReceiver,s.vcOrderPlant,s.item   ");
                strSql.AppendLine("   ) b on a.item=b.item and a.vcReceiver = b.vcReceiver and a.vcOrderPlant=b.vcOrderPlant   ");
                strSql.AppendLine("   left join (   ");
                strSql.AppendLine("   select s.vcReceiver,s.vcOrderPlant,s.item,   ");
                //strSql.AppendLine("   case when  s.vcOrderType  in  ('S','W','D') then '月度订单'   ");
                //strSql.AppendLine("   else  '紧急订单' end orderType,   ");
                strSql.AppendLine("   '紧急订单' as type1,   ");
                strSql.AppendLine("   sum(vcPlantQtyTotal) as '紧急订单',sum(vcInputQtyTotal) as '紧急订单纳入实绩' ,sum(vcResultQtyTotal) as '紧急订单出荷实绩' from (   ");
                strSql.AppendLine("   select vcReceiver, vcOrderPlant, Type as item, vcOrderType,   ");
                strSql.AppendLine("   vcPlantQtyTotal, vcInputQtyTotal, vcResultQtyTotal    ");
                strSql.AppendLine("   from [dbo].[VI_SP_M_OrderData] where vcTargetYearMonth='" + vcTargetMonth + "' and vcOrderType  in  ('H','F','C')     ");
                strSql.AppendLine("   ) S  group by s.vcReceiver,s.vcOrderPlant,s.item  ");
                strSql.AppendLine("   ) c    ");
                strSql.AppendLine("   on c.vcReceiver = a.vcReceiver and c.vcOrderPlant=a.vcOrderPlant and c.item=a.item     ");
                strSql.AppendLine("   )currentMonth   ");
                strSql.AppendLine("   --上一年   ");
                strSql.AppendLine("   left join   ");
                strSql.AppendLine("   (   ");
                strSql.AppendLine("   select a.vcReceiver as '收货方',a.vcOrderPlant as '工场',a.item as '工程',   ");
                strSql.AppendLine("   isnull(b.月度订单,0) as '月度订单' ,isnull(c.紧急订单,0) as '紧急订单',   ");
                strSql.AppendLine("   isnull(b.月度订单,0)+isnull(c.紧急订单,0) as '应纳合计(A)',   ");
                strSql.AppendLine("   isnull(b.月度订单纳入实绩,0) as '月度订单纳入实绩',isnull(c.紧急订单纳入实绩,0) as '紧急订单纳入实绩',   ");
                strSql.AppendLine("   isnull(b.月度订单纳入实绩,0)+isnull(c.紧急订单纳入实绩,0) as '纳入合计(B)',   ");
                strSql.AppendLine("   case when isnull(b.月度订单,0)+isnull(c.紧急订单,0)=0  then  '0.00%'   ");
                strSql.AppendLine("       else   ");
                strSql.AppendLine("   convert(varchar(10),convert(decimal(18,2), (isnull(b.月度订单纳入实绩,0)+isnull(c.紧急订单纳入实绩,0))/(isnull(b.月度订单,0)+isnull(c.紧急订单,0))    ");
                strSql.AppendLine("    ) *100)+'%' end  as '纳入率（B/A）',   ");
                strSql.AppendLine("    null as '前月纳入率',isnull(b.月度订单出荷实绩,0) as '月度订单出荷实绩',isnull(c.紧急订单出荷实绩,0) as '紧急订单出荷实绩',   ");
                strSql.AppendLine("     isnull(b.月度订单出荷实绩,0)+isnull(c.紧急订单出荷实绩,0) as '出荷合计(D)',   ");
                strSql.AppendLine("   case when isnull(b.月度订单,0)+isnull(c.紧急订单,0)=0  then  '0.00%'   ");
                strSql.AppendLine("       else   ");
                strSql.AppendLine("    convert(varchar(10),convert(decimal(18,2),(isnull(b.月度订单出荷实绩,0)+isnull(c.紧急订单出荷实绩,0))/(isnull(b.月度订单,0)+isnull(c.紧急订单,0))   ");
                strSql.AppendLine("    ) *100)+'%' end as '出荷率(D/A)'   ");
                strSql.AppendLine("    from (   ");
                strSql.AppendLine("   select distinct vcReceiver,vcOrderPlant,vcOrderPlantCode,Type as item from [dbo].[VI_SP_M_OrderData] where vcReceiver is not null and vcTargetYearMonth='" + vcLastTargetMonth + "'   ");
                
                if (vcConsignee.Length > 0)
                {
                    strSql.AppendLine("  and  vcReceiver = '" + vcConsignee + "' ");
                }
                if (vcInjectionFactory.Length > 0)
                {
                    strSql.AppendLine("  and  vcOrderPlantCode = '" + vcInjectionFactory + "' ");
                }
                strSql.AppendLine("   ) a   ");
                strSql.AppendLine("   left join   ");
                strSql.AppendLine("   (   ");
                strSql.AppendLine("   select s.vcReceiver,s.vcOrderPlant,s.item,   ");
                //strSql.AppendLine("   case when  s.vcOrderType  in  ('S','W','D') then '月度订单'   ");
                //strSql.AppendLine("   else  '紧急订单' end type1,   ");
                strSql.AppendLine("   '月度订单' as type1,   ");
                strSql.AppendLine("   sum(vcPlantQtyTotal) as '月度订单',sum(vcInputQtyTotal) as '月度订单纳入实绩' ,sum(vcResultQtyTotal) as '月度订单出荷实绩'   ");
                strSql.AppendLine("   from (   ");
                strSql.AppendLine("   select vcReceiver, vcOrderPlant, Type as item, vcOrderType,   ");
                strSql.AppendLine("   vcPlantQtyTotal, vcInputQtyTotal, vcResultQtyTotal    ");
                strSql.AppendLine("   from [dbo].[VI_SP_M_OrderData] where vcTargetYearMonth='"+ vcLastTargetMonth + "' and vcOrderType  in  ('S','W','D')   ");
                strSql.AppendLine("   ) S  group by s.vcReceiver,s.vcOrderPlant,s.item   ");
                strSql.AppendLine("   ) b on a.item=b.item and a.vcReceiver = b.vcReceiver and a.vcOrderPlant=b.vcOrderPlant   ");
                strSql.AppendLine("   left join (   ");
                strSql.AppendLine("   select s.vcReceiver,s.vcOrderPlant,s.item,   ");
                //strSql.AppendLine("   case when  s.vcOrderType  in  ('S','W','D') then '月度订单'   ");
                //strSql.AppendLine("   else  '紧急订单' end orderType,   ");
                strSql.AppendLine("   '紧急订单' as type1,   ");
                strSql.AppendLine("   sum(vcPlantQtyTotal) as '紧急订单',sum(vcInputQtyTotal) as '紧急订单纳入实绩' ,sum(vcResultQtyTotal) as '紧急订单出荷实绩' from (   ");
                strSql.AppendLine("   select vcReceiver, vcOrderPlant, Type as item, vcOrderType,   ");
                strSql.AppendLine("   vcPlantQtyTotal, vcInputQtyTotal, vcResultQtyTotal    ");
                strSql.AppendLine("   from [dbo].[VI_SP_M_OrderData] where vcTargetYearMonth='" + vcLastTargetMonth + "' and vcOrderType  in  ('H','F','C')     ");
                strSql.AppendLine("   ) S  group by s.vcReceiver,s.vcOrderPlant,s.item   ");
                strSql.AppendLine("   ) c    ");
                strSql.AppendLine("   on c.vcReceiver = a.vcReceiver and c.vcOrderPlant=a.vcOrderPlant and c.item=a.item     ");
                strSql.AppendLine("   )lastMonth on currentMonth.收货方=lastMonth.收货方 and currentMonth.工场=lastMonth.工场   ");
                strSql.AppendLine("   and currentMonth.工程=lastMonth.工程   order by currentMonth.工场 asc,currentMonth.工程 asc  ");
                strSql.AppendLine("   ;   ");



                // 单个工场合计的dt
                #region
                /* strSql.AppendLine("   select currentMonth.收货方,currentMonth.工场,currentMonth.工程,currentMonth.月度订单,   ");
                 strSql.AppendLine("   currentMonth.紧急订单,currentMonth.[应纳合计(A)],currentMonth.月度订单纳入实绩,   ");
                 strSql.AppendLine("   currentMonth.紧急订单纳入实绩,currentMonth.[纳入合计(B)],currentMonth.纳入率(B/A),   ");
                 strSql.AppendLine("   isnull(lastMonth.纳入率(B/A),'0.00%') as 前月纳入率,currentMonth.月度订单出荷实绩,   ");
                 strSql.AppendLine("   currentMonth.紧急订单出荷实绩,currentMonth.[出荷合计(D)],currentMonth.出荷率(D/A),   ");
                 strSql.AppendLine("   isnull(lastMonth.出荷率(D/A),'0.00%') as '前月出荷率' from (   ");*/
                strSql.AppendLine("     select currentMonth.收货方,currentMonth.工场,currentMonth.工程,currentMonth.月度订单,       ");
                strSql.AppendLine("     currentMonth.紧急订单,currentMonth.[应纳合计(A)],currentMonth.月度订单纳入实绩,       ");
                strSql.AppendLine("     currentMonth.紧急订单纳入实绩,currentMonth.[纳入合计(B)],    ");
                strSql.AppendLine("         ");
                strSql.AppendLine("     '' as  '纳入率(B/A)','' as 前月纳入率,    ");
                strSql.AppendLine("     currentMonth.月度订单出荷实绩,       ");
                strSql.AppendLine("     currentMonth.紧急订单出荷实绩,currentMonth.[出荷合计(D)],      ");
                strSql.AppendLine("     --currentMonth.出荷率(D/A), isnull(lastMonth.出荷率(D/A),'0.00%') as '前月出荷率'     ");
                strSql.AppendLine("      '' as  '出荷率(D/A)','' as 前月出荷率,    ");
                strSql.AppendLine("   lastMonth.月度订单 as lastYuDuOrder,lastMonth.紧急订单 as lastJinJiOrder,   ");
                strSql.AppendLine("   lastMonth.月度订单纳入实绩 as lastYuDuNRSJ,lastMonth.紧急订单纳入实绩 as lastJinJiNRSJ,	 ");
                strSql.AppendLine("   lastMonth.月度订单出荷实绩 as lastYuDuCHSJ,lastMonth.紧急订单出荷实绩 as lastJinJiCHSJ   ");
                strSql.AppendLine("   from (   ");
                 strSql.AppendLine("   select [收货方],[工场] as '工场','合计' as '工程',   ");
                 strSql.AppendLine("   sum(convert(decimal(18,6),月度订单)) as 月度订单,   ");
                 strSql.AppendLine("   sum(convert(decimal(18,6),[紧急订单])) as 紧急订单,   ");
                 strSql.AppendLine("   sum(convert(decimal(18,6),[应纳合计(A)])) as '应纳合计(A)',   ");
                 strSql.AppendLine("   sum(convert(decimal(18,6),月度订单纳入实绩)) as '月度订单纳入实绩',   ");
                 strSql.AppendLine("   sum(convert(decimal(18,6),紧急订单纳入实绩)) as '紧急订单纳入实绩',   ");
                 strSql.AppendLine("   sum(convert(decimal(18,6),[纳入合计(B)])) as '纳入合计(B)',   ");
                 strSql.AppendLine("   case when isnull(sum(convert(decimal(18,6),月度订单)),0)+isnull(sum(convert(decimal(18,6),[紧急订单])),0)=0  then  '0.00%'   ");
                 strSql.AppendLine("       else    ");
                 strSql.AppendLine("   convert(varchar(10),convert(decimal(18,2), (isnull(sum(convert(decimal(18,6),月度订单纳入实绩)),0)+isnull(sum(convert(decimal(18,6),紧急订单纳入实绩)),0))/(isnull(sum(convert(decimal(18,6),月度订单)),0)+isnull(sum(convert(decimal(18,6),[紧急订单])),0))    ");
                 strSql.AppendLine("    ) *100)+'%' end  as '纳入率（B/A）',   ");
                 strSql.AppendLine("   sum(convert(decimal(18,6),月度订单出荷实绩)) as 月度订单出荷实绩,   ");
                 strSql.AppendLine("   sum(convert(decimal(18,6),紧急订单出荷实绩)) as 紧急订单出荷实绩,   ");
                 strSql.AppendLine("   sum(convert(decimal(18,6),[出荷合计(D)])) as '出荷合计(D)',   ");
                 strSql.AppendLine("   case when isnull(sum(convert(decimal(18,6),月度订单)),0)+isnull(sum(convert(decimal(18,6),[紧急订单])),0)=0  then  '0.00%'   ");
                 strSql.AppendLine("       else    ");
                 strSql.AppendLine("   convert(varchar(10),convert(decimal(18,2), (isnull(sum(convert(decimal(18,6),月度订单出荷实绩)),0)+isnull(sum(convert(decimal(18,6),紧急订单出荷实绩)),0))/(isnull(sum(convert(decimal(18,6),月度订单)),0)+isnull(sum(convert(decimal(18,6),[紧急订单])),0))    ");
                 strSql.AppendLine("    ) *100)+'%' end  as '出荷率(D/A)'   ");
               
                strSql.AppendLine("    from(   ");
                strSql.AppendLine("   select a.vcReceiver as '收货方',a.vcOrderPlant as '工场',a.item as '工程',   ");
                strSql.AppendLine("   isnull(b.月度订单,0) as '月度订单' ,isnull(c.紧急订单,0) as '紧急订单',   ");
                strSql.AppendLine("   isnull(b.月度订单,0)+isnull(c.紧急订单,0) as '应纳合计(A)',   ");
                strSql.AppendLine("   isnull(b.月度订单纳入实绩,0) as '月度订单纳入实绩',isnull(c.紧急订单纳入实绩,0) as '紧急订单纳入实绩',   ");
                strSql.AppendLine("   isnull(b.月度订单纳入实绩,0)+isnull(c.紧急订单纳入实绩,0) as '纳入合计(B)',   ");
                strSql.AppendLine("   case when isnull(b.月度订单,0)+isnull(c.紧急订单,0)=0  then  '0.00%'   ");
                strSql.AppendLine("       else    ");
                strSql.AppendLine("   convert(varchar(10),convert(decimal(18,2), (isnull(b.月度订单纳入实绩,0)+isnull(c.紧急订单纳入实绩,0))/(isnull(b.月度订单,0)+isnull(c.紧急订单,0))    ");
                strSql.AppendLine("    ) *100)+'%' end  as '纳入率（B/A）',   ");
                strSql.AppendLine("    null as '前月纳入率',isnull(b.月度订单出荷实绩,0) as '月度订单出荷实绩',isnull(c.紧急订单出荷实绩,0) as '紧急订单出荷实绩',   ");
                strSql.AppendLine("     isnull(b.月度订单出荷实绩,0)+isnull(c.紧急订单出荷实绩,0) as '出荷合计(D)',   ");
                strSql.AppendLine("    case when isnull(b.月度订单,0)+isnull(c.紧急订单,0)=0  then  '0.00%'   ");
                strSql.AppendLine("       else    ");
                strSql.AppendLine("    convert(varchar(10),convert(decimal(18,2),(isnull(b.月度订单出荷实绩,0)+isnull(c.紧急订单出荷实绩,0))/(isnull(b.月度订单,0)+isnull(c.紧急订单,0))   ");
                strSql.AppendLine("    ) *100)+'%' end as '出荷率(D/A)',   ");
                strSql.AppendLine("    null as '前月出荷率'   ");
                strSql.AppendLine("    from (   ");
                strSql.AppendLine("   select distinct vcReceiver,vcOrderPlant,vcOrderPlantCode,Type as item from [dbo].[VI_SP_M_OrderData]  where vcReceiver is not null and vcTargetYearMonth='" + vcTargetMonth + "'    ");
                if (vcConsignee.Length > 0)
                {
                    strSql.AppendLine("  and  vcReceiver = '" + vcConsignee + "' ");
                }
                if (vcInjectionFactory.Length > 0)
                {
                    strSql.AppendLine("  and  vcOrderPlantCode = '" + vcInjectionFactory + "' ");
                }

                strSql.AppendLine("   ) a   ");
                strSql.AppendLine("   left join   ");
                strSql.AppendLine("   (   ");
                strSql.AppendLine("   select s.vcReceiver,s.vcOrderPlant,s.item,   ");
                //strSql.AppendLine("   case when  s.vcOrderType  in  ('S','W','D') then '月度订单'   ");
                //strSql.AppendLine("   else  '紧急订单' end type1,   ");
                strSql.AppendLine("   '月度订单' as type1,   ");
                strSql.AppendLine("   sum(vcPlantQtyTotal) as '月度订单',sum(vcInputQtyTotal) as '月度订单纳入实绩' ,sum(vcResultQtyTotal) as '月度订单出荷实绩'   ");
                strSql.AppendLine("   from (   ");
                strSql.AppendLine("   select vcReceiver, vcOrderPlant, Type as item, vcOrderType,   ");
                strSql.AppendLine("   vcPlantQtyTotal, vcInputQtyTotal, vcResultQtyTotal    ");
                strSql.AppendLine("   from [dbo].[VI_SP_M_OrderData] where vcTargetYearMonth='" + vcTargetMonth + "' and vcOrderType  in  ('S','W','D')   ");
                strSql.AppendLine("   ) S  group by s.vcReceiver,s.vcOrderPlant,s.item  ");
                strSql.AppendLine("   ) b on a.item=b.item and a.vcReceiver = b.vcReceiver and a.vcOrderPlant=b.vcOrderPlant   ");
                strSql.AppendLine("   left join (   ");
                strSql.AppendLine("   select s.vcReceiver,s.vcOrderPlant,s.item,   ");
                //strSql.AppendLine("   case when  s.vcOrderType  in  ('S','W','D') then '月度订单'   ");
                //strSql.AppendLine("   else  '紧急订单' end orderType,   ");
                strSql.AppendLine("   '紧急订单' as type1,   ");
                strSql.AppendLine("   sum(vcPlantQtyTotal) as '紧急订单',sum(vcInputQtyTotal) as '紧急订单纳入实绩' ,sum(vcResultQtyTotal) as '紧急订单出荷实绩' from (   ");
                strSql.AppendLine("   select vcReceiver, vcOrderPlant, Type as item, vcOrderType,   ");
                strSql.AppendLine("   vcPlantQtyTotal, vcInputQtyTotal, vcResultQtyTotal    ");
                strSql.AppendLine("   from [dbo].[VI_SP_M_OrderData] where vcTargetYearMonth='" + vcTargetMonth + "' and vcOrderType  in  ('H','F','C')     ");
                strSql.AppendLine("   ) S  group by s.vcReceiver,s.vcOrderPlant,s.item   ");
                strSql.AppendLine("   ) c    ");
                strSql.AppendLine("   on c.vcReceiver = a.vcReceiver and c.vcOrderPlant=a.vcOrderPlant and c.item=a.item     ");
                strSql.AppendLine("   ) d group by d.收货方,d.工场   ");
                strSql.AppendLine("   )currentMonth   ");
                strSql.AppendLine("   ---上一年--   ");
                strSql.AppendLine("   left join (   ");
                strSql.AppendLine("   select [收货方],[工场] as '工场','合计' as '工程',   ");
                strSql.AppendLine("   sum(convert(decimal(18,6),月度订单)) as 月度订单,   ");
                strSql.AppendLine("   sum(convert(decimal(18,6),[紧急订单])) as 紧急订单,   ");
                strSql.AppendLine("   sum(convert(decimal(18,6),[应纳合计(A)])) as '应纳合计(A)',   ");
                strSql.AppendLine("   sum(convert(decimal(18,6),月度订单纳入实绩)) as '月度订单纳入实绩',   ");
                strSql.AppendLine("   sum(convert(decimal(18,6),紧急订单纳入实绩)) as '紧急订单纳入实绩',   ");
                strSql.AppendLine("   sum(convert(decimal(18,6),[纳入合计(B)])) as '纳入合计(B)',   ");
                strSql.AppendLine("   case when isnull(sum(convert(decimal(18,6),月度订单)),0)+isnull(sum(convert(decimal(18,6),[紧急订单])),0)=0  then  '0.00%'   ");
                strSql.AppendLine("       else    ");
                strSql.AppendLine("   convert(varchar(10),convert(decimal(18,2), (isnull(sum(convert(decimal(18,6),月度订单纳入实绩)),0)+isnull(sum(convert(decimal(18,6),紧急订单纳入实绩)),0))/(isnull(sum(convert(decimal(18,6),月度订单)),0)+isnull(sum(convert(decimal(18,6),[紧急订单])),0))    ");
                strSql.AppendLine("    ) *100)+'%' end  as '纳入率（B/A）',   ");
                strSql.AppendLine("   sum(convert(decimal(18,6),月度订单出荷实绩)) as 月度订单出荷实绩,   ");
                strSql.AppendLine("   sum(convert(decimal(18,6),紧急订单出荷实绩)) as 紧急订单出荷实绩,   ");
                strSql.AppendLine("   sum(convert(decimal(18,6),[出荷合计(D)])) as '出荷合计(D)',   ");
                strSql.AppendLine("   case when isnull(sum(convert(decimal(18,6),月度订单)),0)+isnull(sum(convert(decimal(18,6),[紧急订单])),0)=0  then  '0.00%'   ");
                strSql.AppendLine("       else    ");
                strSql.AppendLine("   convert(varchar(10),convert(decimal(18,2), (isnull(sum(convert(decimal(18,6),月度订单出荷实绩)),0)+isnull(sum(convert(decimal(18,6),紧急订单出荷实绩)),0))/(isnull(sum(convert(decimal(18,6),月度订单)),0)+isnull(sum(convert(decimal(18,6),[紧急订单])),0))    ");
                strSql.AppendLine("    ) *100)+'%' end  as '出荷率(D/A)'   ");
                strSql.AppendLine("    from(   ");
                strSql.AppendLine("   select a.vcReceiver as '收货方',a.vcOrderPlant as '工场',a.item as '工程',   ");
                strSql.AppendLine("   isnull(b.月度订单,0) as '月度订单' ,isnull(c.紧急订单,0) as '紧急订单',   ");
                strSql.AppendLine("   isnull(b.月度订单,0)+isnull(c.紧急订单,0) as '应纳合计(A)',   ");
                strSql.AppendLine("   isnull(b.月度订单纳入实绩,0) as '月度订单纳入实绩',isnull(c.紧急订单纳入实绩,0) as '紧急订单纳入实绩',   ");
                strSql.AppendLine("   isnull(b.月度订单纳入实绩,0)+isnull(c.紧急订单纳入实绩,0) as '纳入合计(B)',   ");
                strSql.AppendLine("   case when isnull(b.月度订单,0)+isnull(c.紧急订单,0)=0  then  '0.00%'   ");
                strSql.AppendLine("       else    ");
                strSql.AppendLine("   convert(varchar(10),convert(decimal(18,2), (isnull(b.月度订单纳入实绩,0)+isnull(c.紧急订单纳入实绩,0))/(isnull(b.月度订单,0)+isnull(c.紧急订单,0))    ");
                strSql.AppendLine("    ) *100)+'%' end  as '纳入率（B/A）',   ");
                strSql.AppendLine("    null as '前月纳入率',isnull(b.月度订单出荷实绩,0) as '月度订单出荷实绩',isnull(c.紧急订单出荷实绩,0) as '紧急订单出荷实绩',   ");
                strSql.AppendLine("     isnull(b.月度订单出荷实绩,0)+isnull(c.紧急订单出荷实绩,0) as '出荷合计(D)',   ");
                strSql.AppendLine("    case when isnull(b.月度订单,0)+isnull(c.紧急订单,0)=0  then  '0.00%'   ");
                strSql.AppendLine("       else    ");
                strSql.AppendLine("    convert(varchar(10),convert(decimal(18,2),(isnull(b.月度订单出荷实绩,0)+isnull(c.紧急订单出荷实绩,0))/(isnull(b.月度订单,0)+isnull(c.紧急订单,0))   ");
                strSql.AppendLine("    ) *100)+'%' end as '出荷率(D/A)',   ");
                strSql.AppendLine("    null as '前月出荷率'   ");
                strSql.AppendLine("    from (   ");
                strSql.AppendLine("   select distinct vcReceiver,vcOrderPlant,vcOrderPlantCode,Type as item from [dbo].[VI_SP_M_OrderData]  where vcReceiver is not null and vcTargetYearMonth='" + vcLastTargetMonth + "'    ");
                if (vcConsignee.Length > 0)
                {
                    strSql.AppendLine("  and  vcReceiver = '" + vcConsignee + "' ");
                }
                if (vcInjectionFactory.Length > 0)
                {
                    strSql.AppendLine("  and  vcOrderPlantCode = '" + vcInjectionFactory + "' ");
                }
                strSql.AppendLine("   ) a   ");
                strSql.AppendLine("   left join   ");
                strSql.AppendLine("   (   ");
                strSql.AppendLine("   select s.vcReceiver,s.vcOrderPlant,s.item,   ");
                //strSql.AppendLine("   case when  s.vcOrderType  in  ('S','W','D') then '月度订单'   ");
                //strSql.AppendLine("   else  '紧急订单' end type1,   ");
                strSql.AppendLine("    '月度订单' as type1,   ");
                strSql.AppendLine("   sum(vcPlantQtyTotal) as '月度订单',sum(vcInputQtyTotal) as '月度订单纳入实绩' ,sum(vcResultQtyTotal) as '月度订单出荷实绩'   ");
                strSql.AppendLine("   from (   ");
                strSql.AppendLine("   select vcReceiver, vcOrderPlant, Type as item, vcOrderType,   ");
                strSql.AppendLine("   vcPlantQtyTotal, vcInputQtyTotal, vcResultQtyTotal    ");
                strSql.AppendLine("   from [dbo].[VI_SP_M_OrderData] where vcTargetYearMonth='" + vcLastTargetMonth + "' and vcOrderType  in  ('S','W','D')   ");
                strSql.AppendLine("   ) S  group by s.vcReceiver,s.vcOrderPlant,s.item   ");
                strSql.AppendLine("   ) b on a.item=b.item and a.vcReceiver = b.vcReceiver and a.vcOrderPlant=b.vcOrderPlant   ");
                strSql.AppendLine("   left join (   ");
                strSql.AppendLine("   select s.vcReceiver,s.vcOrderPlant,s.item,   ");
                //strSql.AppendLine("   case when  s.vcOrderType  in  ('S','W','D') then '月度订单'   ");
                //strSql.AppendLine("   else  '紧急订单' end orderType,   ");
                strSql.AppendLine("    '紧急订单' as type1,   ");
                strSql.AppendLine("   sum(vcPlantQtyTotal) as '紧急订单',sum(vcInputQtyTotal) as '紧急订单纳入实绩' ,sum(vcResultQtyTotal) as '紧急订单出荷实绩' from (   ");
                strSql.AppendLine("   select vcReceiver, vcOrderPlant, Type as item, vcOrderType,   ");
                strSql.AppendLine("   vcPlantQtyTotal, vcInputQtyTotal, vcResultQtyTotal    ");
                strSql.AppendLine("   from [dbo].[VI_SP_M_OrderData] where vcTargetYearMonth='" + vcLastTargetMonth + "' and vcOrderType  in  ('H','F','C')     ");
                strSql.AppendLine("   ) S  group by s.vcReceiver,s.vcOrderPlant,s.item  ");
                strSql.AppendLine("   ) c    ");
                strSql.AppendLine("   on c.vcReceiver = a.vcReceiver and c.vcOrderPlant=a.vcOrderPlant and c.item=a.item     ");
                strSql.AppendLine("   ) d group by d.收货方,d.工场   ");
                strSql.AppendLine("      ");
                strSql.AppendLine("   ) lastMonth on currentMonth.收货方 = lastMonth.收货方 and currentMonth.工场=lastMonth.工场   order by currentMonth.工场 asc,currentMonth.工程 asc  ");
                #endregion

                // 合计的dt
                #region
                /*strSql.AppendLine("   select currentMonth.收货方,currentMonth.工场,currentMonth.工程,currentMonth.月度订单,   ");
                strSql.AppendLine("   currentMonth.紧急订单,currentMonth.[应纳合计(A)],currentMonth.月度订单纳入实绩,   ");
                strSql.AppendLine("   currentMonth.紧急订单纳入实绩,currentMonth.[纳入合计(B)],currentMonth.纳入率(B/A),   ");
                strSql.AppendLine("   isnull(lastMonth.纳入率(B/A),'0.00%') as 前月纳入率,currentMonth.月度订单出荷实绩,   ");
                strSql.AppendLine("   currentMonth.紧急订单出荷实绩,currentMonth.[出荷合计(D)],currentMonth.出荷率(D/A),   ");
                strSql.AppendLine("   isnull(lastMonth.出荷率(D/A),'0.00%') as '前月出荷率' from (   ");*/
                strSql.AppendLine("    select currentMonth.收货方,currentMonth.工场,currentMonth.工程,currentMonth.月度订单,      ");
                strSql.AppendLine("    currentMonth.紧急订单,currentMonth.[应纳合计(A)],currentMonth.月度订单纳入实绩,      ");
                strSql.AppendLine("    currentMonth.紧急订单纳入实绩,currentMonth.[纳入合计(B)],   ");
                strSql.AppendLine("       ");
                strSql.AppendLine("     --currentMonth.纳入率(B/A),   isnull(lastMonth.纳入率(B/A),'0.00%') as 前月纳入率,   ");
                strSql.AppendLine("    '' as  '纳入率(B/A)','' as 前月纳入率,   ");
                strSql.AppendLine("    currentMonth.月度订单出荷实绩,      ");
                strSql.AppendLine("    currentMonth.紧急订单出荷实绩,currentMonth.[出荷合计(D)],     ");
                strSql.AppendLine("    --currentMonth.出荷率(D/A), isnull(lastMonth.出荷率(D/A),'0.00%') as '前月出荷率'    ");
                strSql.AppendLine("     '' as  '出荷率(D/A)','' as 前月出荷率,   ");
                strSql.AppendLine("   lastMonth.月度订单 as lastYuDuOrder,lastMonth.紧急订单 as lastJinJiOrder,   ");
                strSql.AppendLine("   lastMonth.月度订单纳入实绩 as lastYuDuNRSJ,lastMonth.紧急订单纳入实绩 as lastJinJiNRSJ,   ");
                strSql.AppendLine("   lastMonth.月度订单出荷实绩 as lastYuDuCHSJ,lastMonth.紧急订单出荷实绩 as lastJinJiCHSJ   ");
                strSql.AppendLine("   from (     ");
                strSql.AppendLine("   select [收货方],'' as '工场','' as '工程',   ");
                strSql.AppendLine("   sum(convert(decimal(18,6),月度订单)) as 月度订单,   ");
                strSql.AppendLine("   sum(convert(decimal(18,6),[紧急订单])) as 紧急订单,   ");
                strSql.AppendLine("   sum(convert(decimal(18,6),[应纳合计(A)])) as '应纳合计(A)',   ");
                strSql.AppendLine("   sum(convert(decimal(18,6),月度订单纳入实绩)) as '月度订单纳入实绩',   ");
                strSql.AppendLine("   sum(convert(decimal(18,6),紧急订单纳入实绩)) as '紧急订单纳入实绩',   ");
                strSql.AppendLine("   sum(convert(decimal(18,6),[纳入合计(B)])) as '纳入合计(B)',   ");
                strSql.AppendLine("   case when isnull(sum(convert(decimal(18,6),月度订单)),0)+isnull(sum(convert(decimal(18,6),[紧急订单])),0)=0  then  '0.00%'   ");
                strSql.AppendLine("       else    ");
                strSql.AppendLine("   convert(varchar(10),convert(decimal(18,2), (isnull(sum(convert(decimal(18,6),月度订单纳入实绩)),0)+isnull(sum(convert(decimal(18,6),紧急订单纳入实绩)),0))/(isnull(sum(convert(decimal(18,6),月度订单)),0)+isnull(sum(convert(decimal(18,6),[紧急订单])),0))    ");
                strSql.AppendLine("    ) *100)+'%' end  as '纳入率（B/A）',   ");
                strSql.AppendLine("   sum(convert(decimal(18,6),月度订单出荷实绩)) as 月度订单出荷实绩,   ");
                strSql.AppendLine("   sum(convert(decimal(18,6),紧急订单出荷实绩)) as 紧急订单出荷实绩,   ");
                strSql.AppendLine("   sum(convert(decimal(18,6),[出荷合计(D)])) as '出荷合计(D)',   ");
                strSql.AppendLine("   case when isnull(sum(convert(decimal(18,6),月度订单)),0)+isnull(sum(convert(decimal(18,6),[紧急订单])),0)=0  then  '0.00%'   ");
                strSql.AppendLine("       else    ");
                strSql.AppendLine("   convert(varchar(10),convert(decimal(18,2), (isnull(sum(convert(decimal(18,6),月度订单出荷实绩)),0)+isnull(sum(convert(decimal(18,6),紧急订单出荷实绩)),0))/(isnull(sum(convert(decimal(18,6),月度订单)),0)+isnull(sum(convert(decimal(18,6),[紧急订单])),0))    ");
                strSql.AppendLine("    ) *100)+'%' end  as '出荷率(D/A)'   ");
                strSql.AppendLine("    from(   ");
                strSql.AppendLine("   select a.vcReceiver as '收货方',a.vcOrderPlant as '工场',a.item as '工程',   ");
                strSql.AppendLine("   isnull(b.月度订单,0) as '月度订单' ,isnull(c.紧急订单,0) as '紧急订单',   ");
                strSql.AppendLine("   isnull(b.月度订单,0)+isnull(c.紧急订单,0) as '应纳合计(A)',   ");
                strSql.AppendLine("   isnull(b.月度订单纳入实绩,0) as '月度订单纳入实绩',isnull(c.紧急订单纳入实绩,0) as '紧急订单纳入实绩',   ");
                strSql.AppendLine("   isnull(b.月度订单纳入实绩,0)+isnull(c.紧急订单纳入实绩,0) as '纳入合计(B)',   ");
                strSql.AppendLine("   case when isnull(b.月度订单,0)+isnull(c.紧急订单,0)=0  then  '0.00%'   ");
                strSql.AppendLine("       else    ");
                strSql.AppendLine("   convert(varchar(10),convert(decimal(18,2), (isnull(b.月度订单纳入实绩,0)+isnull(c.紧急订单纳入实绩,0))/(isnull(b.月度订单,0)+isnull(c.紧急订单,0))    ");
                strSql.AppendLine("    ) *100)+'%' end  as '纳入率（B/A）',   ");
                strSql.AppendLine("    null as '前月纳入率',isnull(b.月度订单出荷实绩,0) as '月度订单出荷实绩',isnull(c.紧急订单出荷实绩,0) as '紧急订单出荷实绩',   ");
                strSql.AppendLine("     isnull(b.月度订单出荷实绩,0)+isnull(c.紧急订单出荷实绩,0) as '出荷合计(D)',   ");
                strSql.AppendLine("    case when isnull(b.月度订单,0)+isnull(c.紧急订单,0)=0  then  '0.00%'   ");
                strSql.AppendLine("       else    ");
                strSql.AppendLine("    convert(varchar(10),convert(decimal(18,2),(isnull(b.月度订单出荷实绩,0)+isnull(c.紧急订单出荷实绩,0))/(isnull(b.月度订单,0)+isnull(c.紧急订单,0))   ");
                strSql.AppendLine("    ) *100)+'%' end as '出荷率(D/A)',   ");
                strSql.AppendLine("    null as '前月出荷率'   ");
                strSql.AppendLine("    from (   ");
                strSql.AppendLine("   select distinct vcReceiver,vcOrderPlant,vcOrderPlantCode,Type as item from [dbo].[VI_SP_M_OrderData]  where vcReceiver is not null and vcTargetYearMonth='" + vcTargetMonth + "'    ");
                if (vcConsignee.Length > 0)
                {
                    strSql.AppendLine("  and  vcReceiver = '" + vcConsignee + "' ");
                }
                if (vcInjectionFactory.Length > 0)
                {
                    strSql.AppendLine("  and  vcOrderPlantCode = '" + vcInjectionFactory + "' ");
                }

                strSql.AppendLine("   ) a   ");
                strSql.AppendLine("   left join   ");
                strSql.AppendLine("   (   ");
                strSql.AppendLine("   select s.vcReceiver,s.vcOrderPlant,s.item,   ");
                //strSql.AppendLine("   case when  s.vcOrderType  in  ('S','W','D') then '月度订单'   ");
                //strSql.AppendLine("   else  '紧急订单' end type1,   ");
                strSql.AppendLine("   '月度订单' as type1,   ");
                strSql.AppendLine("   sum(vcPlantQtyTotal) as '月度订单',sum(vcInputQtyTotal) as '月度订单纳入实绩' ,sum(vcResultQtyTotal) as '月度订单出荷实绩'   ");
                strSql.AppendLine("   from (   ");
                strSql.AppendLine("   select vcReceiver, vcOrderPlant, Type as item, vcOrderType,   ");
                strSql.AppendLine("   vcPlantQtyTotal, vcInputQtyTotal, vcResultQtyTotal    ");
                strSql.AppendLine("   from [dbo].[VI_SP_M_OrderData] where vcTargetYearMonth='" + vcTargetMonth + "' and vcOrderType  in  ('S','W','D')   ");
                strSql.AppendLine("   ) S  group by s.vcReceiver,s.vcOrderPlant,s.item   ");
                strSql.AppendLine("   ) b on a.item=b.item and a.vcReceiver = b.vcReceiver and a.vcOrderPlant=b.vcOrderPlant   ");
                strSql.AppendLine("   left join (   ");
                strSql.AppendLine("   select s.vcReceiver,s.vcOrderPlant,s.item,   ");
                //strSql.AppendLine("   case when  s.vcOrderType  in  ('S','W','D') then '月度订单'   ");
                //strSql.AppendLine("   else  '紧急订单' end orderType,   ");
                strSql.AppendLine("   '紧急订单' as orderType,   ");
                strSql.AppendLine("   sum(vcPlantQtyTotal) as '紧急订单',sum(vcInputQtyTotal) as '紧急订单纳入实绩' ,sum(vcResultQtyTotal) as '紧急订单出荷实绩' from (   ");
                strSql.AppendLine("   select vcReceiver, vcOrderPlant, Type as item, vcOrderType,   ");
                strSql.AppendLine("   vcPlantQtyTotal, vcInputQtyTotal, vcResultQtyTotal    ");
                strSql.AppendLine("   from [dbo].[VI_SP_M_OrderData] where vcTargetYearMonth='" + vcTargetMonth + "' and vcOrderType  in  ('H','F','C')     ");
                strSql.AppendLine("   ) S  group by s.vcReceiver,s.vcOrderPlant,s.item   ");
                strSql.AppendLine("   ) c    ");
                strSql.AppendLine("   on c.vcReceiver = a.vcReceiver and c.vcOrderPlant=a.vcOrderPlant and c.item=a.item     ");
                strSql.AppendLine("   ) d group by d.收货方   ");
                strSql.AppendLine("   )currentMonth   ");
                strSql.AppendLine("   ---上一年--   ");
                strSql.AppendLine("   left join (   ");
                strSql.AppendLine("   select [收货方],'' as '工场','' as '工程',   ");
                strSql.AppendLine("   sum(convert(decimal(18,6),月度订单)) as 月度订单,   ");
                strSql.AppendLine("   sum(convert(decimal(18,6),[紧急订单])) as 紧急订单,   ");
                strSql.AppendLine("   sum(convert(decimal(18,6),[应纳合计(A)])) as '应纳合计(A)',   ");
                strSql.AppendLine("   sum(convert(decimal(18,6),月度订单纳入实绩)) as '月度订单纳入实绩',   ");
                strSql.AppendLine("   sum(convert(decimal(18,6),紧急订单纳入实绩)) as '紧急订单纳入实绩',   ");
                strSql.AppendLine("   sum(convert(decimal(18,6),[纳入合计(B)])) as '纳入合计(B)',   ");
                strSql.AppendLine("   case when isnull(sum(convert(decimal(18,6),月度订单)),0)+isnull(sum(convert(decimal(18,6),[紧急订单])),0)=0  then  '0.00%'   ");
                strSql.AppendLine("       else    ");
                strSql.AppendLine("   convert(varchar(10),convert(decimal(18,2), (isnull(sum(convert(decimal(18,6),月度订单纳入实绩)),0)+isnull(sum(convert(decimal(18,6),紧急订单纳入实绩)),0))/(isnull(sum(convert(decimal(18,6),月度订单)),0)+isnull(sum(convert(decimal(18,6),[紧急订单])),0))    ");
                strSql.AppendLine("    ) *100)+'%' end  as '纳入率（B/A）',   ");
                strSql.AppendLine("   sum(convert(decimal(18,6),月度订单出荷实绩)) as 月度订单出荷实绩,   ");
                strSql.AppendLine("   sum(convert(decimal(18,6),紧急订单出荷实绩)) as 紧急订单出荷实绩,   ");
                strSql.AppendLine("   sum(convert(decimal(18,6),[出荷合计(D)])) as '出荷合计(D)',   ");
                strSql.AppendLine("   case when isnull(sum(convert(decimal(18,6),月度订单)),0)+isnull(sum(convert(decimal(18,6),[紧急订单])),0)=0  then  '0.00%'   ");
                strSql.AppendLine("       else    ");
                strSql.AppendLine("   convert(varchar(10),convert(decimal(18,2), (isnull(sum(convert(decimal(18,6),月度订单出荷实绩)),0)+isnull(sum(convert(decimal(18,6),紧急订单出荷实绩)),0))/(isnull(sum(convert(decimal(18,6),月度订单)),0)+isnull(sum(convert(decimal(18,6),[紧急订单])),0))    ");
                strSql.AppendLine("    ) *100)+'%' end  as '出荷率(D/A)'   ");
                strSql.AppendLine("    from(   ");
                strSql.AppendLine("   select a.vcReceiver as '收货方',a.vcOrderPlant as '工场',a.item as '工程',   ");
                strSql.AppendLine("   isnull(b.月度订单,0) as '月度订单' ,isnull(c.紧急订单,0) as '紧急订单',   ");
                strSql.AppendLine("   isnull(b.月度订单,0)+isnull(c.紧急订单,0) as '应纳合计(A)',   ");
                strSql.AppendLine("   isnull(b.月度订单纳入实绩,0) as '月度订单纳入实绩',isnull(c.紧急订单纳入实绩,0) as '紧急订单纳入实绩',   ");
                strSql.AppendLine("   isnull(b.月度订单纳入实绩,0)+isnull(c.紧急订单纳入实绩,0) as '纳入合计(B)',   ");
                strSql.AppendLine("   case when isnull(b.月度订单,0)+isnull(c.紧急订单,0)=0  then  '0.00%'   ");
                strSql.AppendLine("       else    ");
                strSql.AppendLine("   convert(varchar(10),convert(decimal(18,2), (isnull(b.月度订单纳入实绩,0)+isnull(c.紧急订单纳入实绩,0))/(isnull(b.月度订单,0)+isnull(c.紧急订单,0))    ");
                strSql.AppendLine("    ) *100)+'%' end  as '纳入率（B/A）',   ");
                strSql.AppendLine("    null as '前月纳入率',isnull(b.月度订单出荷实绩,0) as '月度订单出荷实绩',isnull(c.紧急订单出荷实绩,0) as '紧急订单出荷实绩',   ");
                strSql.AppendLine("     isnull(b.月度订单出荷实绩,0)+isnull(c.紧急订单出荷实绩,0) as '出荷合计(D)',   ");
                strSql.AppendLine("    case when isnull(b.月度订单,0)+isnull(c.紧急订单,0)=0  then  '0.00%'   ");
                strSql.AppendLine("       else    ");
                strSql.AppendLine("    convert(varchar(10),convert(decimal(18,2),(isnull(b.月度订单出荷实绩,0)+isnull(c.紧急订单出荷实绩,0))/(isnull(b.月度订单,0)+isnull(c.紧急订单,0))   ");
                strSql.AppendLine("    ) *100)+'%' end as '出荷率(D/A)',   ");
                strSql.AppendLine("    null as '前月出荷率'   ");
                strSql.AppendLine("    from (   ");
                strSql.AppendLine("   select distinct vcReceiver,vcOrderPlant,vcOrderPlantCode,Type as item from [dbo].[VI_SP_M_OrderData]  where vcReceiver is not null and vcTargetYearMonth='" + vcLastTargetMonth + "'    ");
                if (vcConsignee.Length > 0)
                {
                    strSql.AppendLine("  and  vcReceiver = '" + vcConsignee + "' ");
                }
                if (vcInjectionFactory.Length > 0)
                {
                    strSql.AppendLine("  and  vcOrderPlantCode = '" + vcInjectionFactory + "' ");
                }
                strSql.AppendLine("   ) a   ");
                strSql.AppendLine("   left join   ");
                strSql.AppendLine("   (   ");
                strSql.AppendLine("   select s.vcReceiver,s.vcOrderPlant,s.item,   ");
                //strSql.AppendLine("   case when  s.vcOrderType  in  ('S','W','D') then '月度订单'   ");
                //strSql.AppendLine("   else  '紧急订单' end type1,   ");
                strSql.AppendLine("   '月度订单' as type1,   ");
                strSql.AppendLine("   sum(vcPlantQtyTotal) as '月度订单',sum(vcInputQtyTotal) as '月度订单纳入实绩' ,sum(vcResultQtyTotal) as '月度订单出荷实绩'   ");
                strSql.AppendLine("   from (   ");
                strSql.AppendLine("   select vcReceiver, vcOrderPlant, Type as item, vcOrderType,   ");
                strSql.AppendLine("   vcPlantQtyTotal, vcInputQtyTotal, vcResultQtyTotal    ");
                strSql.AppendLine("   from [dbo].[VI_SP_M_OrderData] where vcTargetYearMonth='" + vcLastTargetMonth + "' and vcOrderType  in  ('S','W','D')   ");
                strSql.AppendLine("   ) S  group by s.vcReceiver,s.vcOrderPlant,s.item   ");
                strSql.AppendLine("   ) b on a.item=b.item and a.vcReceiver = b.vcReceiver and a.vcOrderPlant=b.vcOrderPlant   ");
                strSql.AppendLine("   left join (   ");
                strSql.AppendLine("   select s.vcReceiver,s.vcOrderPlant,s.item,   ");
                //strSql.AppendLine("   case when  s.vcOrderType  in  ('S','W','D') then '月度订单'   ");
                //strSql.AppendLine("   else  '紧急订单' end orderType,   ");
                strSql.AppendLine("   '紧急订单' as type1,   ");
                strSql.AppendLine("   sum(vcPlantQtyTotal) as '紧急订单',sum(vcInputQtyTotal) as '紧急订单纳入实绩' ,sum(vcResultQtyTotal) as '紧急订单出荷实绩' from (   ");
                strSql.AppendLine("   select vcReceiver, vcOrderPlant, Type as item, vcOrderType,   ");
                strSql.AppendLine("   vcPlantQtyTotal, vcInputQtyTotal, vcResultQtyTotal    ");
                strSql.AppendLine("   from [dbo].[VI_SP_M_OrderData] where vcTargetYearMonth='" + vcLastTargetMonth + "' and vcOrderType  in  ('H','F','C')     ");
                strSql.AppendLine("   ) S  group by s.vcReceiver,s.vcOrderPlant,s.item   ");
                strSql.AppendLine("   ) c    ");
                strSql.AppendLine("   on c.vcReceiver = a.vcReceiver and c.vcOrderPlant=a.vcOrderPlant and c.item=a.item     ");
                strSql.AppendLine("   ) d group by d.收货方   ");
                strSql.AppendLine("      ");
                strSql.AppendLine("   ) lastMonth on currentMonth.收货方 = lastMonth.收货方  order by currentMonth.工场 asc,currentMonth.工程 asc  ");
                #endregion
                return excute.ExcuteSqlWithSelectToDS(strSql.ToString());
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
        public void Del(DataTable dtdel, string userId)
        {
            StringBuilder sql = new StringBuilder();
            for (int i = 0; i < dtdel.Rows.Count; i++)
            {
                DataRow dr = dtdel.Rows[i];
                sql.Append("delete from [TSpecialSupplier]  \n");
                sql.Append("where vcSupplier_id='" + dr["vcSupplier_id"].ToString() + "'  and vcWorkArea='" + dr["vcWorkArea"].ToString() + "' \n");

            }
            if (sql.Length > 0)
            {
                excute.ExcuteSqlWithStringOper(sql.ToString());
            }
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="dtadd"></param>
        /// <param name="userId"></param>
        public void Save(DataTable dtadd, DataTable dtmod, string userId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < dtadd.Rows.Count; i++)
                {
                    DataRow dr = dtadd.Rows[i];
                    sql.Append("insert into [TSpecialSupplier] (vcSupplier_id, vcWorkArea, dBeginDate, dEndDate, vcOperatorID, dOperatorTime)  \n");
                    sql.Append(" values('" + dr["vcSupplier_id"].ToString() + "','" + dr["vcWorkArea"].ToString() + "','" + dr["dBeginDate"].ToString() + "','" + dr["dEndDate"].ToString() + "','" + userId + "',GETDATE()) \n");
                }
                for (int i = 0; i < dtmod.Rows.Count; i++)
                {
                    DataRow dr = dtmod.Rows[i];
                    sql.Append("update TSpecialSupplier set dBeginDate='" + Convert.ToDateTime(dr["dBeginDate"].ToString()) + "', dEndDate='" + Convert.ToDateTime(dr["dEndDate"].ToString()) + "',vcOperatorID='" + userId + "',dOperatorTime=GETDATE()  \n");
                    sql.Append("where vcSupplier_id='" + dr["vcSupplier_id"].ToString() + "' and vcWorkArea ='" + dr["vcWorkArea"].ToString() + "' \n");

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
        /// 验证修改的数据是否已经存在数据库
        /// </summary>
        /// <param name="dtamod"></param>
        /// <returns></returns>
        public bool isExistModData(DataTable dtamod)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                for (int i = 0; i < dtamod.Rows.Count; i++)
                {
                    if (strSql.Length > 0)
                    {
                        strSql.AppendLine("  union all select vcSupplier_id, vcWorkArea, vcIsSureFlag, vcLinkMan, vcPhone, vcEmail, vcOperatorID, dOperatorTime from dbo.TSupplierInfo where vcSupplier_id='" + dtamod.Rows[i]["vcSupplier_id"] + "'  ");
                    }
                    else
                    {
                        strSql.AppendLine("  select vcSupplier_id, vcWorkArea, vcIsSureFlag, vcLinkMan, vcPhone, vcEmail, vcOperatorID, dOperatorTime from dbo.TSupplierInfo where vcSupplier_id='" + dtamod.Rows[i]["vcSupplier_id"] + "'  ");
                    }
                }
                DataTable dt = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                return dt.Rows.Count > 0;
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
        public void allInstall(DateTime dBeginDate,DateTime dEndDate,string userId) {
            try
            {
                StringBuilder sql = new StringBuilder();
                
                sql.Append("update TSpecialSupplier set dBeginDate='" + dBeginDate + "', dEndDate='" + dEndDate + "',vcOperatorID='" + userId + "',dOperatorTime=GETDATE()  \n");

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
    }
}
