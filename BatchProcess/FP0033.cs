﻿using Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
/// <summary>
/// 每日现场实行计划抓取数据
/// </summary>
namespace BatchProcess
{
    public class FP0033
    {
        private MultiExcute excute = new MultiExcute();

        #region 主方法
        public bool main(string strUserId)
        {
            string PageId = "FP0033";
            try
            {
                //批处理开始
                ComMessage.GetInstance().ProcessMessage(PageId, "M03PI0200", null, strUserId);

                Start();

                //批处理结束
                ComMessage.GetInstance().ProcessMessage(PageId, "M03PI0201", null, strUserId);
                return true;
            }
            catch (Exception ex)
            {
                //批处理异常结束
                ComMessage.GetInstance().ProcessMessage(PageId, "M03PE0200", null, strUserId);
                throw ex;
            }
        }
        #endregion

        public void Start()
        {
            try
            {
                DateTime now = DateTime.Now;
                string now_day = now.Day.ToString();
                string now_YYYYMM = now.ToString("yyyyMM");
                string now_YYYYMMDD = now.ToString("yyyy-MM-dd");
                DateTime sub1 = now.AddDays(-1);
                string sub1_day = sub1.Day.ToString();
                string sub1_YYYYMM = sub1.ToString("yyyyMM");
                string sub1_YYYYMMDD = sub1.ToString("yyyy-MM-dd");
                DateTime sub2 = now.AddDays(-2);
                string sub2_day = sub2.Day.ToString();
                string sub2_YYYYMM = sub2.ToString("yyyyMM");

                StringBuilder sql = new StringBuilder();

                #region 往report表中插入条目（单独执行）
                sql.AppendLine("with temp as");
                sql.AppendLine("(");
                sql.AppendLine("	select t0.kind,t2.vcPlant,t2.vcBigPM,t2.vcSmallPM");
                sql.AppendLine("	from ");
                sql.AppendLine("	(");
                sql.AppendLine("		select '纳入计划' as kind,1 as id");
                sql.AppendLine("		union all ");
                sql.AppendLine("		select '当日实际' as kind,2 as id");
                sql.AppendLine("		union all");
                sql.AppendLine("		select '当日残' as kind,3 as id");
                sql.AppendLine("		union all");
                sql.AppendLine("		select '累计残量' as kind ,4 as id");
                sql.AppendLine("		union all");
                sql.AppendLine("		select '紧急订单' as kind ,5 as id");
                sql.AppendLine("");
                sql.AppendLine("	)t0");
                sql.AppendLine("	left join (    ");
                sql.AppendLine("		--大品目  ");
                sql.AppendLine("		select t1.vcValue as vcPlant,t2.* from(");
                sql.AppendLine("			select * from TCode where vcCodeId='C023' ");
                sql.AppendLine("		)t1");
                sql.AppendLine("		left join (");
                sql.AppendLine("			select t1.vcBigPM,t2.vcSmallPM from (");
                sql.AppendLine("				select vcValue1 as vcBigPM from TOutCode where vcCodeId='C003' and vcIsColum='0'");
                sql.AppendLine("			)t1");
                sql.AppendLine("			left join TPMRelation t2 on t1.vcBigPM=t2.vcBigPM");
                sql.AppendLine("		)t2 on 1=1");
                sql.AppendLine("	)t2 on 1=1  ");
                sql.AppendLine(")");
                sql.AppendLine("");
                sql.AppendLine("insert into TPackingPlan_Report (vcKind,vcPlant,vcYearMonth,vcBigPM,vcSmallPM)");
                sql.AppendLine("select t1.kind,t1.vcPlant,'" + now_YYYYMM + "',t1.vcBigPM,t1.vcSmallPM from temp t1");
                sql.AppendLine("left join (select * from TPackingPlan_Report where vcYearMonth='" + now_YYYYMM + "') t2 ");
                sql.AppendLine("on t1.kind=t2.vcKind and t1.vcPlant=t2.vcPlant");
                sql.AppendLine("and t1.vcBigPM=t2.vcBigPM and t1.vcSmallPM=t2.vcSmallPM");
                sql.AppendLine("where t2.iAutoId is null");
                excute.ExcuteSqlWithStringOper(sql.ToString());
                #endregion

                sql.Length = 0;
                #region 更新report表
                sql.AppendLine("--更新N日纳入计划");
                sql.AppendLine("update t1 set t1.iD"+now_day+"=isnull(t2.iD"+ now_day + ",0)+isnull(t3.iD"+ now_day + ",0)");
                sql.AppendLine("from");
                sql.AppendLine("(");
                sql.AppendLine("	select * from TPackingPlan_Report where vcYearMonth='"+ now_YYYYMM + "' and vcKind='纳入计划'");
                sql.AppendLine(")t1");
                sql.AppendLine("left join (");
                sql.AppendLine("	select vcPlant,vcBigPM,vcSmallPM,isnull(iBZPlan_Heji,0) as iD"+ now_day + " ");
                sql.AppendLine("	from TPackingPlan_Summary ");
                sql.AppendLine("	where dPackDate='"+ now_YYYYMMDD + "'");
                sql.AppendLine(")t2 on t1.vcPlant=t2.vcPlant and t1.vcBigPM=t2.vcBigPM and t1.vcSmallPM=t2.vcSmallPM");
                sql.AppendLine("left join (");
                sql.AppendLine("	--紧急订单        ");
                sql.AppendLine("	select t2.vcBZPlant,t3.vcBigPM,t2.vcSmallPM,SUM(CAST(ISNULL(t1.vcOrderNum,0) as int)) as iD"+ now_day + "       ");
                sql.AppendLine("	from         ");
                sql.AppendLine("	(        ");
                sql.AppendLine("		select vcPartNo,vcInsideOutsideType,vcDock,vcOrderNum,vcSupplier_id         ");
                sql.AppendLine("		from TEmergentOrderManage         ");
                sql.AppendLine("		where dExpectReceiveDate='"+ now_YYYYMMDD + "' and vcInsideOutsideType='1'        ");
                sql.AppendLine("	)t1        ");
                sql.AppendLine("	left join (    ");
                sql.AppendLine("		select * from TPackageMaster where vcReceiver='APC06' and vcPackingPlant='TFTM'    ");
                sql.AppendLine("	) t2 on t1.vcPartNo=t2.vcPart_id and t1.vcSupplier_id=t2.vcSupplierId    ");
                sql.AppendLine("	left join TPMRelation t3 on t2.vcSmallPM=t3.vcSmallPM    ");
                sql.AppendLine("	group by t2.vcBZPlant,t3.vcBigPM,t2.vcSmallPM    ");
                sql.AppendLine(")t3 on t1.vcPlant=t3.vcBZPlant and t1.vcBigPM=t3.vcBigPM and t1.vcSmallPM=t3.vcSmallPM");

                sql.AppendLine("--更新N日紧急订单");
                sql.AppendLine("update t1 set t1.iD" + now_day + "=isnull(t3.iD" + now_day + ",0)");
                sql.AppendLine("from");
                sql.AppendLine("(");
                sql.AppendLine("	select * from TPackingPlan_Report where vcYearMonth='" + now_YYYYMM + "' and vcKind='紧急订单'");
                sql.AppendLine(")t1");
                sql.AppendLine("left join (");
                sql.AppendLine("	--紧急订单        ");
                sql.AppendLine("	select t2.vcBZPlant,t3.vcBigPM,t2.vcSmallPM,SUM(CAST(ISNULL(t1.vcOrderNum,0) as int)) as iD" + now_day + "       ");
                sql.AppendLine("	from         ");
                sql.AppendLine("	(        ");
                sql.AppendLine("		select vcPartNo,vcInsideOutsideType,vcDock,vcOrderNum,vcSupplier_id         ");
                sql.AppendLine("		from TEmergentOrderManage         ");
                sql.AppendLine("		where dExpectReceiveDate='" + now_YYYYMMDD + "' and vcInsideOutsideType='1'        ");
                sql.AppendLine("	)t1        ");
                sql.AppendLine("	left join (    ");
                sql.AppendLine("		select * from TPackageMaster where vcReceiver='APC06' and vcPackingPlant='TFTM'    ");
                sql.AppendLine("	) t2 on t1.vcPartNo=t2.vcPart_id and t1.vcSupplier_id=t2.vcSupplierId    ");
                sql.AppendLine("	left join TPMRelation t3 on t2.vcSmallPM=t3.vcSmallPM    ");
                sql.AppendLine("	group by t2.vcBZPlant,t3.vcBigPM,t2.vcSmallPM    ");
                sql.AppendLine(")t3 on t1.vcPlant=t3.vcBZPlant and t1.vcBigPM=t3.vcBigPM and t1.vcSmallPM=t3.vcSmallPM");

                sql.AppendLine("--更新N-1日当日实际");
                sql.AppendLine("update t1 set t1.iD"+ sub1_day + "=t2.iD"+ sub1_day + "");
                sql.AppendLine("from");
                sql.AppendLine("(");
                sql.AppendLine("	select * from TPackingPlan_Report where vcYearMonth='"+ sub1_YYYYMM + "' and vcKind='当日实际'");
                sql.AppendLine(")t1");
                sql.AppendLine("left join (");
                sql.AppendLine("	select t2.vcBZPlant,t3.vcBigPM,t2.vcSmallPM,SUM(ISNULL(t1.iQuantity,0)) as iD"+ sub1_day + "         ");
                sql.AppendLine("	from         ");
                sql.AppendLine("	(        ");
                sql.AppendLine("		select vcPart_id,vcSR,iQuantity,vcSupplier_id from TOperateSJ         ");
                sql.AppendLine("		where vcZYType='S2'   ");
                sql.AppendLine("		and dEnd between '"+ sub1_YYYYMMDD + " 08:00' and '"+ now_YYYYMMDD + " 08:00'      ");
                sql.AppendLine("	)t1       ");
                sql.AppendLine("	left join (    ");
                sql.AppendLine("		select * from TPackageMaster where vcReceiver='APC06' and vcPackingPlant='TFTM'    ");
                sql.AppendLine("	) t2 on t1.vcPart_id=t2.vcPart_id and t1.vcSupplier_id=t2.vcSupplierId    ");
                sql.AppendLine("	left join TPMRelation t3 on t2.vcSmallPM=t3.vcSmallPM    ");
                sql.AppendLine("	group by t2.vcBZPlant,t3.vcBigPM,t2.vcSmallPM ");
                sql.AppendLine(")t2 on t1.vcPlant=t2.vcBZPlant and t1.vcBigPM=t2.vcBigPM and t1.vcSmallPM=t2.vcSmallPM");

                sql.AppendLine("--更新N-1日当日残=N-1日计划-N-1日实际");
                sql.AppendLine("update t1 set t1.iD"+ sub1_day + "=isnull(t2.iD"+ sub1_day + ",0)-isnull(t3.iD"+ sub1_day + ",0) ");
                sql.AppendLine("from (");
                sql.AppendLine("	select * from TPackingPlan_Report where vcYearMonth='"+ sub1_YYYYMM + "' and vcKind='当日残'");
                sql.AppendLine(")t1");
                sql.AppendLine("left join (");
                sql.AppendLine("	select * from TPackingPlan_Report where vcYearMonth='"+ sub1_YYYYMM + "' and vcKind='纳入计划'");
                sql.AppendLine(")t2 on t1.vcPlant=t2.vcPlant and t1.vcBigPM=t2.vcBigPM and t1.vcSmallPM=t2.vcSmallPM");
                sql.AppendLine("left join (");
                sql.AppendLine("	select * from TPackingPlan_Report where vcYearMonth='"+ sub1_YYYYMM + "' and vcKind='当日实际'");
                sql.AppendLine(")t3 on t1.vcPlant=t3.vcPlant and t1.vcBigPM=t3.vcBigPM and t1.vcSmallPM=t3.vcSmallPM");

                sql.AppendLine("--更新N-1日累计残量=N-2日累计残量+N-1日当日残");
                sql.AppendLine("update t1 set t1.iD"+ sub1_day + "=isnull(t2.iD"+ sub2_day + ",0)+isnull(t3.iD"+ sub1_day + ",0) ");
                sql.AppendLine("from (");
                sql.AppendLine("	select * from TPackingPlan_Report where vcYearMonth='"+ sub1_YYYYMM + "' and vcKind='累计残量'");
                sql.AppendLine(")t1");
                sql.AppendLine("left join (");
                sql.AppendLine("	select * from TPackingPlan_Report where vcYearMonth='"+ sub2_YYYYMM + "' and vcKind='累计残量'");
                sql.AppendLine(")t2 on t1.vcPlant=t2.vcPlant and t1.vcBigPM=t2.vcBigPM and t1.vcSmallPM=t2.vcSmallPM");
                sql.AppendLine("left join (");
                sql.AppendLine("	select * from TPackingPlan_Report where vcYearMonth='"+ sub1_YYYYMM + "' and vcKind='当日残'");
                sql.AppendLine(")t3 on t1.vcPlant=t3.vcPlant and t1.vcBigPM=t3.vcBigPM and t1.vcSmallPM=t3.vcSmallPM");
                #endregion

                #region 更新汇总表
                sql.AppendLine("--更新N日紧急订单");
                sql.AppendLine("update t1 set t1.iEmergencyOrder=t2.iD"+ now_day + "");
                sql.AppendLine("from ");
                sql.AppendLine("(");
                sql.AppendLine("	select * from TPackingPlan_Summary where dPackDate='"+ now_YYYYMMDD + "'");
                sql.AppendLine(")t1");
                sql.AppendLine("left join (");
                sql.AppendLine("	select id"+ now_day + ",vcPlant,vcBigPM,vcSmallPM from TPackingPlan_Report ");
                sql.AppendLine("	where vcYearMonth='"+ now_YYYYMM + "' and vcKind='紧急订单'");
                sql.AppendLine(")t2 on t1.vcPlant=t2.vcPlant and t1.vcBigPM=t2.vcBigPM and t1.vcSmallPM=t2.vcSmallPM");

                sql.AppendLine("--更新N-1日累计残");
                sql.AppendLine("update t1 set t1.iLJBZRemain=t2.iD" + sub1_day + "");
                sql.AppendLine("from ");
                sql.AppendLine("(");
                sql.AppendLine("	select * from TPackingPlan_Summary where dPackDate='"+ sub1_YYYYMMDD + "'");
                sql.AppendLine(")t1");
                sql.AppendLine("left join (");
                sql.AppendLine("	select id"+ sub1_day + ",vcPlant,vcBigPM,vcSmallPM from TPackingPlan_Report ");
                sql.AppendLine("	where vcYearMonth='"+ sub1_YYYYMM + "' and vcKind='累计残量'");
                sql.AppendLine(")t2 on t1.vcPlant=t2.vcPlant and t1.vcBigPM=t2.vcBigPM and t1.vcSmallPM=t2.vcSmallPM");

                sql.AppendLine("--更新N日实行计划");
                sql.AppendLine("update t set ");
                sql.AppendLine("t.iSSPlan_Day=");
                sql.AppendLine("case when vcBigPM='成型' then A+cast((D+F)/2.0 as decimal(18,1)) else cast((C+D+F)/2.0 as decimal(18,1)) end,");
                sql.AppendLine("t.iSSPlan_Night=");
                sql.AppendLine("case when vcBigPM='成型' then B+ D+F-cast((D+F)/2.0 as decimal(18,1)) else C+D+F-cast((C+D+F)/2.0 as decimal(18,1)) end,");
                sql.AppendLine("t.iSSPlan_Heji=");
                sql.AppendLine("case when vcBigPM='成型' then A+B+D+F else C+D+F end");
                sql.AppendLine("from (");
                sql.AppendLine("	select vcPlant,dPackDate,vcBigPM,vcSmallPM,");
                sql.AppendLine("	isnull(iBZPlan_Day,0) as A,isnull(iBZPlan_Night,0) as B,isnull(iBZPlan_Heji,0) as C,");
                sql.AppendLine("	isnull(iEmergencyOrder,0) as D,isnull(iLJBZRemain,0) as E,0 as F,");
                sql.AppendLine("	iSSPlan_Day,iSSPlan_Night,iSSPlan_Heji");
                sql.AppendLine("	from TPackingPlan_Summary where dPackDate='"+ now_YYYYMMDD + "'");
                sql.AppendLine(")t");
                #endregion

                excute.ExcuteSqlWithStringOper(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
