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
                strSql.AppendLine("  chu2.partNum,chu1.qianPinSum from ( ");
                strSql.AppendLine("  select a.vcReceiver,a.vcOrderPlant,a.vcClassType,a.vcSupplierId,sum(chaZhiNaRu) as qianPinSum from  ");
                strSql.AppendLine("  ( ");
                strSql.AppendLine("  select vcReceiver,vcOrderPlant, '纳入' as vcClassType, ");
                strSql.AppendLine("  case when type='外注' then vcSupplier_id+isnull(vcWorkArea,'') ");
                strSql.AppendLine("  else type end as vcSupplierId, ");
                strSql.AppendLine("  vcPartNo,vcPlantQtyTotal-vcInputQtyTotal as chaZhiNaRu ");
                strSql.AppendLine("   from [dbo].[VI_SP_M_ORD_S0629]  ");
                strSql.AppendLine("   where vcTargetYearMonth='"+vcTargetYearMonth+"' and (vcPlantQtyTotal-vcInputQtyTotal)>0 ");
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
                strSql.AppendLine("   where vcTargetYearMonth='"+vcTargetYearMonth+"'  and (vcPlantQtyTotal-vcInputQtyTotal)>0 ");
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
                strSql.AppendLine("   where vcTargetYearMonth='"+vcTargetYearMonth+ "' and (vcInputQtyTotal-vcResultQtyTotal)>0 ");
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
                strSql.AppendLine("   where vcTargetYearMonth='"+vcTargetYearMonth+ "'  and (vcInputQtyTotal-vcResultQtyTotal)>0 ");
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
                strSql.AppendLine("   ) huiZong order by huiZong.vcReceiver,huiZong.vcOrderPlant,huiZong.vcClassType desc ");
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
                strSql.AppendLine("   where vcTargetYearMonth='"+vcTargetYearMonth+"'  ");
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
                strSql.AppendLine("   where vcTargetYearMonth='"+vcTargetYearMonth+"'  and (vcPlantQtyTotal-vcInputQtyTotal)>0 ");
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
                strSql.AppendLine("   where vcTargetYearMonth='"+vcTargetYearMonth+ "' and (vcInputQtyTotal-vcResultQtyTotal)>0 ");
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
                strSql.AppendLine("   where vcTargetYearMonth='"+vcTargetYearMonth+ "' and (vcInputQtyTotal-vcResultQtyTotal)>0 ");
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
                strSql.AppendLine("   select currentMonth.收货方,currentMonth.工厂,currentMonth.工程,currentMonth.月度订单,   ");
                strSql.AppendLine("   currentMonth.紧急订单,currentMonth.[应纳合计(A)],currentMonth.月度订单纳入实绩,   ");
                strSql.AppendLine("   currentMonth.紧急订单纳入实绩,currentMonth.[纳入合计(B)],currentMonth.[纳入率（B/A）],   ");
                strSql.AppendLine("   isnull(lastMonth.[纳入率（B/A）],'0.00%') as 前月纳入率,currentMonth.月度订单出荷实绩,   ");
                strSql.AppendLine("   currentMonth.紧急订单出荷实绩,currentMonth.[出荷合计(D)],currentMonth.[出荷率(D/A)],   ");
                strSql.AppendLine("   isnull(lastMonth.[出荷率(D/A)],'0.00%') as '前月出荷率' from (   ");
                strSql.AppendLine("   select a.vcReceiver as '收货方',a.vcOrderPlant as '工厂',a.item as '工程',   ");
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
                strSql.AppendLine("   select distinct vcReceiver,vcOrderPlant,Type as item from [dbo].[VI_SP_M_OrderData] where vcTargetYearMonth='" + vcTargetMonth + "'     ");
                if (vcConsignee.Length > 0)
                {
                    strSql.AppendLine("  and  vcReceiver = '" + vcConsignee + "' ");
                }
                if (vcInjectionFactory.Length > 0)
                {
                    strSql.AppendLine("  and  vcOrderPlant = '" + vcInjectionFactory + "' ");
                }
                strSql.AppendLine("   ) a   ");
                strSql.AppendLine("   left join   ");
                strSql.AppendLine("   (   ");
                strSql.AppendLine("   select s.vcReceiver,s.vcOrderPlant,s.item,   ");
                strSql.AppendLine("   case when  s.vcOrderType='2' then '月度订单'   ");
                strSql.AppendLine("   else  '紧急订单' end type1,   ");
                strSql.AppendLine("   sum(vcPlantQtyTotal) as '月度订单',sum(vcInputQtyTotal) as '月度订单纳入实绩' ,sum(vcResultQtyTotal) as '月度订单出荷实绩'   ");
                strSql.AppendLine("   from (   ");
                strSql.AppendLine("   select vcReceiver, vcOrderPlant, Type as item, vcOrderType,   ");
                strSql.AppendLine("   vcPlantQtyTotal, vcInputQtyTotal, vcResultQtyTotal    ");
                strSql.AppendLine("   from [dbo].[VI_SP_M_OrderData] where vcTargetYearMonth='"+ vcTargetMonth + "' and vcOrderType='2'   ");
                strSql.AppendLine("   ) S  group by s.vcReceiver,s.vcOrderPlant,s.item,s.vcOrderType   ");
                strSql.AppendLine("   ) b on a.item=b.item and a.vcReceiver = b.vcReceiver and a.vcOrderPlant=b.vcOrderPlant   ");
                strSql.AppendLine("   left join (   ");
                strSql.AppendLine("   select s.vcReceiver,s.vcOrderPlant,s.item,   ");
                strSql.AppendLine("   case when  s.vcOrderType='2' then '月度订单'   ");
                strSql.AppendLine("   else  '紧急订单' end orderType,   ");
                strSql.AppendLine("   sum(vcPlantQtyTotal) as '紧急订单',sum(vcInputQtyTotal) as '紧急订单纳入实绩' ,sum(vcResultQtyTotal) as '紧急订单出荷实绩' from (   ");
                strSql.AppendLine("   select vcReceiver, vcOrderPlant, Type as item, vcOrderType,   ");
                strSql.AppendLine("   vcPlantQtyTotal, vcInputQtyTotal, vcResultQtyTotal    ");
                strSql.AppendLine("   from [dbo].[VI_SP_M_OrderData] where vcTargetYearMonth='" + vcTargetMonth + "' and vcOrderType='3'   ");
                strSql.AppendLine("   ) S  group by s.vcReceiver,s.vcOrderPlant,s.item,s.vcOrderType   ");
                strSql.AppendLine("   ) c    ");
                strSql.AppendLine("   on c.vcReceiver = b.vcReceiver and c.vcOrderPlant=b.vcOrderPlant and c.item=b.item   ");
                strSql.AppendLine("   )currentMonth   ");
                strSql.AppendLine("   --上一年   ");
                strSql.AppendLine("   left join   ");
                strSql.AppendLine("   (   ");
                strSql.AppendLine("   select a.vcReceiver as '收货方',a.vcOrderPlant as '工厂',a.item as '工程',   ");
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
                strSql.AppendLine("   select distinct vcReceiver,vcOrderPlant,Type as item from [dbo].[VI_SP_M_OrderData] where vcTargetYearMonth='"+ vcLastTargetMonth + "'   ");
                
                if (vcConsignee.Length > 0)
                {
                    strSql.AppendLine("  and  vcReceiver = '" + vcConsignee + "' ");
                }
                if (vcInjectionFactory.Length > 0)
                {
                    strSql.AppendLine("  and  vcOrderPlant = '" + vcInjectionFactory + "' ");
                }
                strSql.AppendLine("   ) a   ");
                strSql.AppendLine("   left join   ");
                strSql.AppendLine("   (   ");
                strSql.AppendLine("   select s.vcReceiver,s.vcOrderPlant,s.item,   ");
                strSql.AppendLine("   case when  s.vcOrderType='2' then '月度订单'   ");
                strSql.AppendLine("   else  '紧急订单' end type1,   ");
                strSql.AppendLine("   sum(vcPlantQtyTotal) as '月度订单',sum(vcInputQtyTotal) as '月度订单纳入实绩' ,sum(vcResultQtyTotal) as '月度订单出荷实绩'   ");
                strSql.AppendLine("   from (   ");
                strSql.AppendLine("   select vcReceiver, vcOrderPlant, Type as item, vcOrderType,   ");
                strSql.AppendLine("   vcPlantQtyTotal, vcInputQtyTotal, vcResultQtyTotal    ");
                strSql.AppendLine("   from [dbo].[VI_SP_M_OrderData] where vcTargetYearMonth='"+ vcLastTargetMonth + "' and vcOrderType='2'   ");
                strSql.AppendLine("   ) S  group by s.vcReceiver,s.vcOrderPlant,s.item,s.vcOrderType   ");
                strSql.AppendLine("   ) b on a.item=b.item and a.vcReceiver = b.vcReceiver and a.vcOrderPlant=b.vcOrderPlant   ");
                strSql.AppendLine("   left join (   ");
                strSql.AppendLine("   select s.vcReceiver,s.vcOrderPlant,s.item,   ");
                strSql.AppendLine("   case when  s.vcOrderType='2' then '月度订单'   ");
                strSql.AppendLine("   else  '紧急订单' end orderType,   ");
                strSql.AppendLine("   sum(vcPlantQtyTotal) as '紧急订单',sum(vcInputQtyTotal) as '紧急订单纳入实绩' ,sum(vcResultQtyTotal) as '紧急订单出荷实绩' from (   ");
                strSql.AppendLine("   select vcReceiver, vcOrderPlant, Type as item, vcOrderType,   ");
                strSql.AppendLine("   vcPlantQtyTotal, vcInputQtyTotal, vcResultQtyTotal    ");
                strSql.AppendLine("   from [dbo].[VI_SP_M_OrderData] where vcTargetYearMonth='" + vcLastTargetMonth + "' and vcOrderType='3'   ");
                strSql.AppendLine("   ) S  group by s.vcReceiver,s.vcOrderPlant,s.item,s.vcOrderType   ");
                strSql.AppendLine("   ) c    ");
                strSql.AppendLine("   on c.vcReceiver = b.vcReceiver and c.vcOrderPlant=b.vcOrderPlant and c.item=b.item   ");
                strSql.AppendLine("   )lastMonth on currentMonth.收货方=lastMonth.收货方 and currentMonth.工厂=lastMonth.工厂   ");
                strSql.AppendLine("   and currentMonth.工程=lastMonth.工程   ");
                strSql.AppendLine("   ;   ");



                // 单个工厂合计的dt
                #region
                strSql.AppendLine("   select currentMonth.收货方,currentMonth.工厂,currentMonth.工程,currentMonth.月度订单,   ");
                strSql.AppendLine("   currentMonth.紧急订单,currentMonth.[应纳合计(A)],currentMonth.月度订单纳入实绩,   ");
                strSql.AppendLine("   currentMonth.紧急订单纳入实绩,currentMonth.[纳入合计(B)],currentMonth.[纳入率（B/A）],   ");
                strSql.AppendLine("   isnull(lastMonth.[纳入率（B/A）],'0.00%') as 前月纳入率,currentMonth.月度订单出荷实绩,   ");
                strSql.AppendLine("   currentMonth.紧急订单出荷实绩,currentMonth.[出荷合计(D)],currentMonth.[出荷率(D/A)],   ");
                strSql.AppendLine("   isnull(lastMonth.[出荷率(D/A)],'0.00%') as '前月出荷率' from (   ");
                strSql.AppendLine("      ");
                strSql.AppendLine("   select [收货方],[工厂] as '工厂','合计' as '工程',   ");
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
                strSql.AppendLine("   select a.vcReceiver as '收货方',a.vcOrderPlant as '工厂',a.item as '工程',   ");
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
                strSql.AppendLine("   select distinct vcReceiver,vcOrderPlant,Type as item from [dbo].[VI_SP_M_OrderData]  where vcTargetYearMonth='" + vcTargetMonth + "'    ");
                if (vcConsignee.Length > 0)
                {
                    strSql.AppendLine("  and  vcReceiver = '" + vcConsignee + "' ");
                }
                if (vcInjectionFactory.Length > 0)
                {
                    strSql.AppendLine("  and  vcOrderPlant = '" + vcInjectionFactory + "' ");
                }

                strSql.AppendLine("   ) a   ");
                strSql.AppendLine("   left join   ");
                strSql.AppendLine("   (   ");
                strSql.AppendLine("   select s.vcReceiver,s.vcOrderPlant,s.item,   ");
                strSql.AppendLine("   case when  s.vcOrderType='2' then '月度订单'   ");
                strSql.AppendLine("   else  '紧急订单' end type1,   ");
                strSql.AppendLine("   sum(vcPlantQtyTotal) as '月度订单',sum(vcInputQtyTotal) as '月度订单纳入实绩' ,sum(vcResultQtyTotal) as '月度订单出荷实绩'   ");
                strSql.AppendLine("   from (   ");
                strSql.AppendLine("   select vcReceiver, vcOrderPlant, Type as item, vcOrderType,   ");
                strSql.AppendLine("   vcPlantQtyTotal, vcInputQtyTotal, vcResultQtyTotal    ");
                strSql.AppendLine("   from [dbo].[VI_SP_M_OrderData] where vcTargetYearMonth='" + vcTargetMonth + "' and vcOrderType='2'   ");
                strSql.AppendLine("   ) S  group by s.vcReceiver,s.vcOrderPlant,s.item,s.vcOrderType   ");
                strSql.AppendLine("   ) b on a.item=b.item and a.vcReceiver = b.vcReceiver and a.vcOrderPlant=b.vcOrderPlant   ");
                strSql.AppendLine("   left join (   ");
                strSql.AppendLine("   select s.vcReceiver,s.vcOrderPlant,s.item,   ");
                strSql.AppendLine("   case when  s.vcOrderType='2' then '月度订单'   ");
                strSql.AppendLine("   else  '紧急订单' end orderType,   ");
                strSql.AppendLine("   sum(vcPlantQtyTotal) as '紧急订单',sum(vcInputQtyTotal) as '紧急订单纳入实绩' ,sum(vcResultQtyTotal) as '紧急订单出荷实绩' from (   ");
                strSql.AppendLine("   select vcReceiver, vcOrderPlant, Type as item, vcOrderType,   ");
                strSql.AppendLine("   vcPlantQtyTotal, vcInputQtyTotal, vcResultQtyTotal    ");
                strSql.AppendLine("   from [dbo].[VI_SP_M_OrderData] where vcTargetYearMonth='" + vcTargetMonth + "' and vcOrderType='3'   ");
                strSql.AppendLine("   ) S  group by s.vcReceiver,s.vcOrderPlant,s.item,s.vcOrderType   ");
                strSql.AppendLine("   ) c    ");
                strSql.AppendLine("   on c.vcReceiver = b.vcReceiver and c.vcOrderPlant=b.vcOrderPlant and c.item=b.item   ");
                strSql.AppendLine("   ) d group by d.收货方,d.工厂   ");
                strSql.AppendLine("   )currentMonth   ");
                strSql.AppendLine("   ---上一年--   ");
                strSql.AppendLine("   left join (   ");
                strSql.AppendLine("   select [收货方],[工厂] as '工厂','合计' as '工程',   ");
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
                strSql.AppendLine("   select a.vcReceiver as '收货方',a.vcOrderPlant as '工厂',a.item as '工程',   ");
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
                strSql.AppendLine("   select distinct vcReceiver,vcOrderPlant,Type as item from [dbo].[VI_SP_M_OrderData]  where vcTargetYearMonth='" + vcLastTargetMonth + "'    ");
                if (vcConsignee.Length > 0)
                {
                    strSql.AppendLine("  and  vcReceiver = '" + vcConsignee + "' ");
                }
                if (vcInjectionFactory.Length > 0)
                {
                    strSql.AppendLine("  and  vcOrderPlant = '" + vcInjectionFactory + "' ");
                }
                strSql.AppendLine("   ) a   ");
                strSql.AppendLine("   left join   ");
                strSql.AppendLine("   (   ");
                strSql.AppendLine("   select s.vcReceiver,s.vcOrderPlant,s.item,   ");
                strSql.AppendLine("   case when  s.vcOrderType='2' then '月度订单'   ");
                strSql.AppendLine("   else  '紧急订单' end type1,   ");
                strSql.AppendLine("   sum(vcPlantQtyTotal) as '月度订单',sum(vcInputQtyTotal) as '月度订单纳入实绩' ,sum(vcResultQtyTotal) as '月度订单出荷实绩'   ");
                strSql.AppendLine("   from (   ");
                strSql.AppendLine("   select vcReceiver, vcOrderPlant, Type as item, vcOrderType,   ");
                strSql.AppendLine("   vcPlantQtyTotal, vcInputQtyTotal, vcResultQtyTotal    ");
                strSql.AppendLine("   from [dbo].[VI_SP_M_OrderData] where vcTargetYearMonth='" + vcLastTargetMonth + "' and vcOrderType='2'   ");
                strSql.AppendLine("   ) S  group by s.vcReceiver,s.vcOrderPlant,s.item,s.vcOrderType   ");
                strSql.AppendLine("   ) b on a.item=b.item and a.vcReceiver = b.vcReceiver and a.vcOrderPlant=b.vcOrderPlant   ");
                strSql.AppendLine("   left join (   ");
                strSql.AppendLine("   select s.vcReceiver,s.vcOrderPlant,s.item,   ");
                strSql.AppendLine("   case when  s.vcOrderType='2' then '月度订单'   ");
                strSql.AppendLine("   else  '紧急订单' end orderType,   ");
                strSql.AppendLine("   sum(vcPlantQtyTotal) as '紧急订单',sum(vcInputQtyTotal) as '紧急订单纳入实绩' ,sum(vcResultQtyTotal) as '紧急订单出荷实绩' from (   ");
                strSql.AppendLine("   select vcReceiver, vcOrderPlant, Type as item, vcOrderType,   ");
                strSql.AppendLine("   vcPlantQtyTotal, vcInputQtyTotal, vcResultQtyTotal    ");
                strSql.AppendLine("   from [dbo].[VI_SP_M_OrderData] where vcTargetYearMonth='" + vcLastTargetMonth + "' and vcOrderType='3'   ");
                strSql.AppendLine("   ) S  group by s.vcReceiver,s.vcOrderPlant,s.item,s.vcOrderType   ");
                strSql.AppendLine("   ) c    ");
                strSql.AppendLine("   on c.vcReceiver = b.vcReceiver and c.vcOrderPlant=b.vcOrderPlant and c.item=b.item   ");
                strSql.AppendLine("   ) d group by d.收货方,d.工厂   ");
                strSql.AppendLine("      ");
                strSql.AppendLine("   ) lastMonth on currentMonth.收货方 = lastMonth.收货方 and currentMonth.工厂=lastMonth.工厂   ");
                #endregion

                // 合计的dt
                #region
                strSql.AppendLine("   select currentMonth.收货方,currentMonth.工厂,currentMonth.工程,currentMonth.月度订单,   ");
                strSql.AppendLine("   currentMonth.紧急订单,currentMonth.[应纳合计(A)],currentMonth.月度订单纳入实绩,   ");
                strSql.AppendLine("   currentMonth.紧急订单纳入实绩,currentMonth.[纳入合计(B)],currentMonth.[纳入率（B/A）],   ");
                strSql.AppendLine("   isnull(lastMonth.[纳入率（B/A）],'0.00%') as 前月纳入率,currentMonth.月度订单出荷实绩,   ");
                strSql.AppendLine("   currentMonth.紧急订单出荷实绩,currentMonth.[出荷合计(D)],currentMonth.[出荷率(D/A)],   ");
                strSql.AppendLine("   isnull(lastMonth.[出荷率(D/A)],'0.00%') as '前月出荷率' from (   ");
                strSql.AppendLine("      ");
                strSql.AppendLine("   select [收货方],'' as '工厂','' as '工程',   ");
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
                strSql.AppendLine("   select a.vcReceiver as '收货方',a.vcOrderPlant as '工厂',a.item as '工程',   ");
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
                strSql.AppendLine("   select distinct vcReceiver,vcOrderPlant,Type as item from [dbo].[VI_SP_M_OrderData]  where vcTargetYearMonth='" + vcTargetMonth + "'    ");
                if (vcConsignee.Length > 0)
                {
                    strSql.AppendLine("  and  vcReceiver = '" + vcConsignee + "' ");
                }
                if (vcInjectionFactory.Length > 0)
                {
                    strSql.AppendLine("  and  vcOrderPlant = '" + vcInjectionFactory + "' ");
                }

                strSql.AppendLine("   ) a   ");
                strSql.AppendLine("   left join   ");
                strSql.AppendLine("   (   ");
                strSql.AppendLine("   select s.vcReceiver,s.vcOrderPlant,s.item,   ");
                strSql.AppendLine("   case when  s.vcOrderType='2' then '月度订单'   ");
                strSql.AppendLine("   else  '紧急订单' end type1,   ");
                strSql.AppendLine("   sum(vcPlantQtyTotal) as '月度订单',sum(vcInputQtyTotal) as '月度订单纳入实绩' ,sum(vcResultQtyTotal) as '月度订单出荷实绩'   ");
                strSql.AppendLine("   from (   ");
                strSql.AppendLine("   select vcReceiver, vcOrderPlant, Type as item, vcOrderType,   ");
                strSql.AppendLine("   vcPlantQtyTotal, vcInputQtyTotal, vcResultQtyTotal    ");
                strSql.AppendLine("   from [dbo].[VI_SP_M_OrderData] where vcTargetYearMonth='" + vcTargetMonth + "' and vcOrderType='2'   ");
                strSql.AppendLine("   ) S  group by s.vcReceiver,s.vcOrderPlant,s.item,s.vcOrderType   ");
                strSql.AppendLine("   ) b on a.item=b.item and a.vcReceiver = b.vcReceiver and a.vcOrderPlant=b.vcOrderPlant   ");
                strSql.AppendLine("   left join (   ");
                strSql.AppendLine("   select s.vcReceiver,s.vcOrderPlant,s.item,   ");
                strSql.AppendLine("   case when  s.vcOrderType='2' then '月度订单'   ");
                strSql.AppendLine("   else  '紧急订单' end orderType,   ");
                strSql.AppendLine("   sum(vcPlantQtyTotal) as '紧急订单',sum(vcInputQtyTotal) as '紧急订单纳入实绩' ,sum(vcResultQtyTotal) as '紧急订单出荷实绩' from (   ");
                strSql.AppendLine("   select vcReceiver, vcOrderPlant, Type as item, vcOrderType,   ");
                strSql.AppendLine("   vcPlantQtyTotal, vcInputQtyTotal, vcResultQtyTotal    ");
                strSql.AppendLine("   from [dbo].[VI_SP_M_OrderData] where vcTargetYearMonth='" + vcTargetMonth + "' and vcOrderType='3'   ");
                strSql.AppendLine("   ) S  group by s.vcReceiver,s.vcOrderPlant,s.item,s.vcOrderType   ");
                strSql.AppendLine("   ) c    ");
                strSql.AppendLine("   on c.vcReceiver = b.vcReceiver and c.vcOrderPlant=b.vcOrderPlant and c.item=b.item   ");
                strSql.AppendLine("   ) d group by d.收货方   ");
                strSql.AppendLine("   )currentMonth   ");
                strSql.AppendLine("   ---上一年--   ");
                strSql.AppendLine("   left join (   ");
                strSql.AppendLine("   select [收货方],'' as '工厂','' as '工程',   ");
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
                strSql.AppendLine("   select a.vcReceiver as '收货方',a.vcOrderPlant as '工厂',a.item as '工程',   ");
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
                strSql.AppendLine("   select distinct vcReceiver,vcOrderPlant,Type as item from [dbo].[VI_SP_M_OrderData]  where vcTargetYearMonth='" + vcLastTargetMonth + "'    ");
                if (vcConsignee.Length > 0)
                {
                    strSql.AppendLine("  and  vcReceiver = '" + vcConsignee + "' ");
                }
                if (vcInjectionFactory.Length > 0)
                {
                    strSql.AppendLine("  and  vcOrderPlant = '" + vcInjectionFactory + "' ");
                }
                strSql.AppendLine("   ) a   ");
                strSql.AppendLine("   left join   ");
                strSql.AppendLine("   (   ");
                strSql.AppendLine("   select s.vcReceiver,s.vcOrderPlant,s.item,   ");
                strSql.AppendLine("   case when  s.vcOrderType='2' then '月度订单'   ");
                strSql.AppendLine("   else  '紧急订单' end type1,   ");
                strSql.AppendLine("   sum(vcPlantQtyTotal) as '月度订单',sum(vcInputQtyTotal) as '月度订单纳入实绩' ,sum(vcResultQtyTotal) as '月度订单出荷实绩'   ");
                strSql.AppendLine("   from (   ");
                strSql.AppendLine("   select vcReceiver, vcOrderPlant, Type as item, vcOrderType,   ");
                strSql.AppendLine("   vcPlantQtyTotal, vcInputQtyTotal, vcResultQtyTotal    ");
                strSql.AppendLine("   from [dbo].[VI_SP_M_OrderData] where vcTargetYearMonth='" + vcLastTargetMonth + "' and vcOrderType='2'   ");
                strSql.AppendLine("   ) S  group by s.vcReceiver,s.vcOrderPlant,s.item,s.vcOrderType   ");
                strSql.AppendLine("   ) b on a.item=b.item and a.vcReceiver = b.vcReceiver and a.vcOrderPlant=b.vcOrderPlant   ");
                strSql.AppendLine("   left join (   ");
                strSql.AppendLine("   select s.vcReceiver,s.vcOrderPlant,s.item,   ");
                strSql.AppendLine("   case when  s.vcOrderType='2' then '月度订单'   ");
                strSql.AppendLine("   else  '紧急订单' end orderType,   ");
                strSql.AppendLine("   sum(vcPlantQtyTotal) as '紧急订单',sum(vcInputQtyTotal) as '紧急订单纳入实绩' ,sum(vcResultQtyTotal) as '紧急订单出荷实绩' from (   ");
                strSql.AppendLine("   select vcReceiver, vcOrderPlant, Type as item, vcOrderType,   ");
                strSql.AppendLine("   vcPlantQtyTotal, vcInputQtyTotal, vcResultQtyTotal    ");
                strSql.AppendLine("   from [dbo].[VI_SP_M_OrderData] where vcTargetYearMonth='" + vcLastTargetMonth + "' and vcOrderType='3'   ");
                strSql.AppendLine("   ) S  group by s.vcReceiver,s.vcOrderPlant,s.item,s.vcOrderType   ");
                strSql.AppendLine("   ) c    ");
                strSql.AppendLine("   on c.vcReceiver = b.vcReceiver and c.vcOrderPlant=b.vcOrderPlant and c.item=b.item   ");
                strSql.AppendLine("   ) d group by d.收货方   ");
                strSql.AppendLine("      ");
                strSql.AppendLine("   ) lastMonth on currentMonth.收货方 = lastMonth.收货方   ");
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
