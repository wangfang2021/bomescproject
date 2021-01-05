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
    public class FS1205_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        public string checkPlanExist(string mon, string plant)
        {
            try
            {
                //2013-6-8 改造 start
                //string ssql = " select * from dbo.MonthPackPlanTbl t where (montouch = '" + mon + "' or vcMonth = '" + mon + "') and exists (select vcPartsNo from dbo.tPartInfoMaster where vcPartPlant ='" + plant + "' and vcPartsNo = t.vcPartsno) ";
                string ssql = " select * from WeekPackPlanTbl where (montouch = '" + mon + "' or (vcMonth = '" + mon + "' and  montouch is null)) and exists ( select distinct vcPartsNo,vcDock ,vcCarType from tPlanPartInfo where tPlanPartInfo.vcMonth = '" + mon + "' and tPlanPartInfo.vcPlant ='" + plant + "' and tPlanPartInfo.vcPartsNo = MonthPackPlanTbl.vcPartsno and tPlanPartInfo.vcDock = MonthPackPlanTbl.vcDock and tPlanPartInfo.vcCarType = MonthPackPlanTbl.vcCarType  ) ;";
                //2013-6-8 改造 end
                DataTable dt =excute.ExcuteSqlWithSelectToDT(ssql);
                if (dt.Rows.Count > 0)
                {
                    return "已经存在该月计划！";
                }
                else return "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //获取临时表的值别计划
        public DataTable getMonPackPlanTMP(string mon, string tablename, string plant)
        {
            DataTable dt = new DataTable();
            string tmpT = "";
            string tmpE = "";
            DateTime tim = Convert.ToDateTime(mon.Split('-')[0] + "-" + mon.Split('-')[1] + "-1");
            double daynum = (tim.AddMonths(1) - tim).TotalDays;

            for (int i = 1; i < 31 + 1; i++)
            {
                if (i == 31)
                    tmpE += "t2.vcD" + i + "b   as  ED" + i + "b,	t2.	vcD" + i + "y 	as	ED" + i + "y";
                else tmpE += "t2.vcD" + i + "b 	as 	ED" + i + "b,	t2.vcD" + i + "y 	as	ED" + i + "y,";
            }
            double daynum2 = (tim - tim.AddMonths(-1)).TotalDays;
            for (int i = 1; i < 31 + 1; i++)
            {
                if (i == 31)
                    tmpT += "t1.vcD" + i + "b as	TD" + i + "b,	t1.vcD" + i + "y 	as	TD" + i + "y";
                else tmpT += "t1.vcD" + i + "b 	as	TD" + i + "b,	t1.vcD" + i + "y 	as	TD" + i + "y,";
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("    select t2.vcMonth ,t5.vcData2 as vcPlant, SUBSTRING(t2.vcPartsno,0,6)+'-'+SUBSTRING(t2.vcPartsno,6,5)+'-'+SUBSTRING(t2.vcPartsno,11,2) as vcPartsno ,t2.vcDock,t2.vcCarType,t4.vcCalendar1,t4.vcCalendar2,t4.vcCalendar3,t4.vcCalendar4,");
            sb.AppendLine("   t3.vcPartNameCN as vcPartsNameCHN, t4.vcProName1 as vcProject1,t3.vcProType+'-'+t3.vcZB as vcProjectName, t3.vcHJ as vcCurrentPastCode,t2.vcMonTotal as vcMonTotal ,");
            sb.AppendFormat(" {0},", tmpT);
            sb.AppendFormat(" {0}", tmpE);
            sb.AppendFormat("  from ( select  * from   {0} where montouch is not null) t1 ", tablename);
            sb.AppendFormat("  full join (select * from {0} where montouch is null) t2", tablename);
            sb.AppendLine("  on t1.montouch = t2.vcMonth and t1.vcPartsno=t2.vcPartsno and t1.vcDock=t2.vcDock and t1.vcCarType=t2.vcCarType");
            sb.AppendLine("  left join (select distinct vcMonth,vcPartNameCN,vcZB,vcHJ,vcDock,vcCarType,vcPartsNo,vcProType,vcPlant,vcEDFlag from dbo.tPlanPartInfo) t3");
            sb.AppendLine("  on t3.vcPartsNo=t2.vcPartsNo and t3.vcDock = t2.vcDock and t3.vcCarType = t2.vcCarType and  t3.vcMonth = '" + mon + "' ");
            sb.AppendLine("  left join dbo.ProRuleMst t4");
            sb.AppendLine("  on t4.vcPorType = t3.vcProType and t4.vcZB = t3.vcZB");
            sb.AppendLine(" left join (select vcData1 ,vcData2  from dbo.ConstMst where vcDataId ='kbplant') t5");
            sb.AppendLine(" on t3.vcPlant = t5.vcData1 ");
            sb.AppendFormat("  where t2.vcMonth ='{0}' and t3.vcPlant ='{1}' and t3.vcEDFlag ='S' ", mon, plant);
            try
            {
                dt = excute.ExcuteSqlWithSelectToDT(sb.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region 俩表合并
        public DataTable MergeDataTable(DataTable dt1, DataTable dt2)
        {
            DataTable DataTable1 = dt1;
            DataTable DataTable2 = dt2;
            DataTable newDataTable = DataTable1.Clone();
            object[] obj = new object[newDataTable.Columns.Count];
            for (int i = 0; i < DataTable1.Rows.Count; i++)
            {
                DataTable1.Rows[i].ItemArray.CopyTo(obj, 0);
                newDataTable.Rows.Add(obj);
            }
            for (int i = 0; i < DataTable2.Rows.Count; i++)
            {
                DataTable2.Rows[i].ItemArray.CopyTo(obj, 0);
                newDataTable.Rows.Add(obj);
            }
            return newDataTable;
        }
        #endregion

        #region 初始化工厂DDL - 李兴旺整理

        public DataTable bindplant()
        {
            string ssql = " select '<-请选择->' as vcData1,'<-请选择->' as vcData2 union all select distinct vcData1,vcData2 from ConstMst where vcDataID='KBPlant' ";
            DataTable dt = new DataTable();
            try
            {
                dt = excute.ExcuteSqlWithSelectToDT(ssql);
            }
            catch (Exception ex)
            {
                return null;
            }
            return dt;
        }
        #endregion

        #region 查询周计划变动幅度管理表当前月当前周当前厂区的所有信息 - 李兴旺

        /// <summary>
        /// 查询周计划变动幅度管理表当前月当前周当前厂区的所有信息

        /// </summary>
        /// <param name="strMonth">对象月</param>
        /// <param name="strWeek">对象周</param>
        /// <param name="strPlant">厂区</param>
        /// <returns>检索结果</returns>
        public DataTable getWeekLevelPercentage(string strMonth, string strWeek, string strPlant)
        {
            DataTable dt = new DataTable();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("select * from  WeekLevelPercentage where vcMonth='{0}' and vcWeek='{1}' and vcPlant='{2}' ", strMonth, strWeek, strPlant);
            try
            {
                dt = excute.ExcuteSqlWithSelectToDT(sb.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        #endregion

        #region 查询周度订单平准化管理表当前月当前周当前厂区的所有信息 - 李兴旺

        /// <summary>
        /// 查询周度订单平准化管理表当前月当前周当前厂区的所有信息

        /// </summary>
        /// <param name="strMonth">对象月</param>
        /// <param name="strWeek">对象周</param>
        /// <param name="strPlant">厂区</param>
        /// <returns>检索结果</returns>
        public DataTable getWeekLevelSchedule(string strMonth, string strWeek, string strPlant)
        {
            DataTable dt = new DataTable();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("select * from  WeekLevelSchedule where vcMonth='{0}' and vcWeek='{1}' and vcPlant='{2}' ", strMonth, strWeek, strPlant);
            try
            {
                dt = excute.ExcuteSqlWithSelectToDT(sb.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        #endregion

        #region 查询周度包装计划管理表当前月当前厂区的所有信息 - 李兴旺

        /// <summary>
        /// 查询周度包装计划管理表当前月当前厂区的所有信息

        /// </summary>
        /// <param name="strMonth">对象月</param>
        /// <param name="strPlant">厂区</param>
        /// <returns>检索结果</returns>
        public DataTable getWeekPackPlan(string strMonth, string strPlant)
        {
            DataTable dt = new DataTable();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" select * from WeekPackPlanTbl where vcMonth='{0}' and (vcMonTotal <> '' or vcMonTotal is not null) ", strMonth);
            sb.AppendFormat(" and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='{0}' and vcPartsNo = WeekPackPlanTbl.vcPartsno ", strPlant);
            sb.AppendFormat(" and dTimeFrom <= '{0}-01' and dTimeTo >= '{0}-01') ", strMonth);
            sb.AppendLine(" union all ");
            sb.AppendFormat(" select * from WeekPackPlanTbl where montouch = '{0}' ", strMonth);
            sb.AppendFormat(" and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='{0}' and vcPartsNo = WeekPackPlanTbl.vcPartsno ", strPlant);
            sb.AppendFormat(" and dTimeFrom <= '{0}-01' and dTimeTo >= '{0}-01') ", strMonth);
            try
            {
                dt = excute.ExcuteSqlWithSelectToDT(sb.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        #endregion
    }
}
