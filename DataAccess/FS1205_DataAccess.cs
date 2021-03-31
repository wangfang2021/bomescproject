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
                DataTable dt = excute.ExcuteSqlWithSelectToDT(ssql);
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

        #region 取得txt文件路径
        /// <summary>
        /// 取得txt路径
        /// </summary>
        /// <param name="vcYM"></param>
        /// <param name="vcWeek"></param>
        /// <returns></returns>
        public DataTable getTxtFileRoute(string vcYM, string vcWeek)
        {
            string sql = "select top 1 * from TOrderUploadManage where vcTargetYear+'-'+vcTargetMonth='" + vcYM + "' and vcTargetWeek='" + vcWeek + "' order by dUploadDate desc";
            return excute.ExcuteSqlWithSelectToDT(sql);
        }
        #endregion

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
            sb.AppendFormat("  from ( select  * from {0} where montouch is not null) t1 ", tablename);
            sb.AppendFormat("  full join (select * from {0} where montouch is null) t2", tablename);
            sb.AppendLine("  on t1.montouch = t2.vcMonth and t1.vcPartsno=t2.vcPartsno and t1.vcDock=t2.vcDock and t1.vcCarType=t2.vcCarType");
            sb.AppendLine("  left join (");
            sb.AppendLine("             select distinct vcMonth,vcPartNameCN,vcZB,s2.vcName as vcHJ,vcDock,vcCarType,vcPartsNo,vcProType,vcPlant,vcEDFlag from tPlanPartInfo s1 ");
            sb.AppendLine("             left join (select vcName, vcValue from tcode where vcCodeId='C004') s2 ");
            sb.AppendLine("             on s1.vcHJ=s2.vcValue ");
            sb.AppendLine("             ) t3 ");
            sb.AppendLine("  on t3.vcPartsNo=t2.vcPartsNo and t3.vcDock = t2.vcDock and t3.vcCarType = t2.vcCarType and  t3.vcMonth = '" + mon + "' ");
            sb.AppendLine("  left join ProRuleMst t4");
            sb.AppendLine("  on t4.vcPorType = t3.vcProType and t4.vcZB = t3.vcZB");
            sb.AppendLine(" left join (select vcData1 ,vcData2 from ConstMst where vcDataId ='kbplant') t5");
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
            string ssql = " select '' as vcData1,'' as vcData2 union all select distinct vcData1,vcData2 from ConstMst where vcDataID='KBPlant' ";
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

        public DataTable getWeekPackPlan_Sum(string strMonth, string strPlant) //wlw
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select vcMonth, vcPartsno, vcDock, vcCarType, vcProject1, vcProjectName, vcMonTotal, \r\n");
            sb.Append("isnull(vcD1b,0)+isnull(vcD1y,0) as vcD1,isnull(vcD2b,0)+isnull(vcD2y,0) as vcD2,isnull(vcD3b,0)+isnull(vcD3y,0) as vcD3, \r\n");
            sb.Append("isnull(vcD4b,0)+isnull(vcD4y,0) as vcD4,isnull(vcD5b,0)+isnull(vcD5y,0) as vcD5,isnull(vcD3b,0)+isnull(vcD6y,0) as vcD6, \r\n");
            sb.Append("isnull(vcD7b,0)+isnull(vcD7y,0) as vcD7,isnull(vcD8b,0)+isnull(vcD8y,0) as vcD8,isnull(vcD9b,0)+isnull(vcD9y,0) as vcD9, \r\n");
            sb.Append("isnull(vcD10b,0)+isnull(vcD10y,0) as vcD10,isnull(vcD11b,0)+isnull(vcD11y,0) as vcD11,isnull(vcD12b,0)+isnull(vcD12y,0) as vcD12, \r\n");
            sb.Append("isnull(vcD13b,0)+isnull(vcD13y,0) as vcD13,isnull(vcD14b,0)+isnull(vcD14y,0) as vcD14,isnull(vcD15b,0)+isnull(vcD15y,0) as vcD15, \r\n");
            sb.Append("isnull(vcD16b,0)+isnull(vcD16y,0) as vcD16,isnull(vcD17b,0)+isnull(vcD17y,0) as vcD17,isnull(vcD18b,0)+isnull(vcD18y,0) as vcD18, \r\n");
            sb.Append("isnull(vcD19b,0)+isnull(vcD19y,0) as vcD19,isnull(vcD20b,0)+isnull(vcD20y,0) as vcD20,isnull(vcD21b,0)+isnull(vcD21y,0) as vcD21, \r\n");
            sb.Append("isnull(vcD22b,0)+isnull(vcD22y,0) as vcD22,isnull(vcD23b,0)+isnull(vcD23y,0) as vcD23,isnull(vcD24b,0)+isnull(vcD24y,0) as vcD24, \r\n");
            sb.Append("isnull(vcD25b,0)+isnull(vcD25y,0) as vcD25,isnull(vcD26b,0)+isnull(vcD26y,0) as vcD26,isnull(vcD27b,0)+isnull(vcD27y,0) as vcD27, \r\n");
            sb.Append("isnull(vcD28b,0)+isnull(vcD28y,0) as vcD28,isnull(vcD29b,0)+isnull(vcD29y,0) as vcD29,isnull(vcD30b,0)+isnull(vcD30y,0) as vcD30, \r\n");
            sb.Append("isnull(vcD31b,0)+isnull(vcD31y,0) as vcD31,montouch,DADDTIME,DUPDTIME,CUPDUSER,vcSupplier_id from WeekPackPlanTbl  \r\n");
            sb.Append("where vcMonth='" + strMonth + "' and (vcMonTotal<>'' or vcMonTotal is not null) and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant='" + strPlant + "'  \r\n");
            sb.Append("and vcPartsNo=WeekPackPlanTbl.vcPartsno and dTimeFrom<='" + strMonth + "' and dTimeTo>='" + strMonth + "')   \r\n");
            sb.Append("union all  \r\n");
            sb.Append("select vcMonth, vcPartsno, vcDock, vcCarType, vcProject1, vcProjectName, vcMonTotal, \r\n");
            sb.Append("isnull(vcD1b,0)+isnull(vcD1y,0) as vcD1,isnull(vcD2b,0)+isnull(vcD2y,0) as vcD2,isnull(vcD3b,0)+isnull(vcD3y,0) as vcD3, \r\n");
            sb.Append("isnull(vcD4b,0)+isnull(vcD4y,0) as vcD4,isnull(vcD5b,0)+isnull(vcD5y,0) as vcD5,isnull(vcD3b,0)+isnull(vcD6y,0) as vcD6, \r\n");
            sb.Append("isnull(vcD7b,0)+isnull(vcD7y,0) as vcD7,isnull(vcD8b,0)+isnull(vcD8y,0) as vcD8,isnull(vcD9b,0)+isnull(vcD9y,0) as vcD9, \r\n");
            sb.Append("isnull(vcD10b,0)+isnull(vcD10y,0) as vcD10,isnull(vcD11b,0)+isnull(vcD11y,0) as vcD11,isnull(vcD12b,0)+isnull(vcD12y,0) as vcD12, \r\n");
            sb.Append("isnull(vcD13b,0)+isnull(vcD13y,0) as vcD13,isnull(vcD14b,0)+isnull(vcD14y,0) as vcD14,isnull(vcD15b,0)+isnull(vcD15y,0) as vcD15, \r\n");
            sb.Append("isnull(vcD16b,0)+isnull(vcD16y,0) as vcD16,isnull(vcD17b,0)+isnull(vcD17y,0) as vcD17,isnull(vcD18b,0)+isnull(vcD18y,0) as vcD18, \r\n");
            sb.Append("isnull(vcD19b,0)+isnull(vcD19y,0) as vcD19,isnull(vcD20b,0)+isnull(vcD20y,0) as vcD20,isnull(vcD21b,0)+isnull(vcD21y,0) as vcD21, \r\n");
            sb.Append("isnull(vcD22b,0)+isnull(vcD22y,0) as vcD22,isnull(vcD23b,0)+isnull(vcD23y,0) as vcD23,isnull(vcD24b,0)+isnull(vcD24y,0) as vcD24, \r\n");
            sb.Append("isnull(vcD25b,0)+isnull(vcD25y,0) as vcD25,isnull(vcD26b,0)+isnull(vcD26y,0) as vcD26,isnull(vcD27b,0)+isnull(vcD27y,0) as vcD27, \r\n");
            sb.Append("isnull(vcD28b,0)+isnull(vcD28y,0) as vcD28,isnull(vcD29b,0)+isnull(vcD29y,0) as vcD29,isnull(vcD30b,0)+isnull(vcD30y,0) as vcD30, \r\n");
            sb.Append("isnull(vcD31b,0)+isnull(vcD31y,0) as vcD31,montouch,DADDTIME,DUPDTIME,CUPDUSER,vcSupplier_id from WeekPackPlanTbl  \r\n");
            sb.Append("where montouch='" + strMonth + "' and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant='" + strPlant + "'  \r\n");
            sb.Append("and vcPartsNo=WeekPackPlanTbl.vcPartsno  and dTimeFrom<='" + strMonth + "' and dTimeTo>='" + strMonth + "')  \r\n");
            DataTable dt;
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

        #region 日程别更新
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    if (bAddFlag == false && bModFlag == true)
                    {//修改
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                        sql.Append("  update WeekLevelSchedule set  \r\n");
                        sql.Append("  vcLevelD1b=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD1b"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD1y=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD1y"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD2b=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD2b"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD2y=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD2y"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD3b=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD3b"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD3y=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD3y"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD4b=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD4b"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD4y=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD4y"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD5b=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD5b"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD5y=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD5y"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD6b=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD6b"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD6y=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD6y"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD7b=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD7b"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD7y=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD7y"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD8b=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD8b"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD8y=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD8y"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD9b=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD9b"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD9y=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD9y"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD10b=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD10b"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD10y=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD10y"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD11b=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD11b"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD11y=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD11y"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD12b=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD12b"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD12y=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD12y"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD13b=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD13b"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD13y=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD13y"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD14b=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD14b"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD14y=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD14y"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD15b=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD15b"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD15y=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD15y"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD16b=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD16b"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD16y=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD16y"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD17b=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD17b"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD17y=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD17y"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD18b=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD18b"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD18y=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD18y"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD19b=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD19b"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD19y=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD19y"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD20b=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD20b"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD20y=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD20y"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD21b=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD21b"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD21y=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD21y"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD22b=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD22b"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD22y=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD22y"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD23b=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD23b"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD23y=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD23y"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD24b=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD24b"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD24y=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD24y"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD25b=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD25b"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD25y=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD25y"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD26b=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD26b"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD26y=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD26y"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD27b=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD27b"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD27y=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD27y"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD28b=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD28b"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD28y=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD28y"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD29b=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD29b"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD29y=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD29y"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD30b=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD30b"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD30y=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD30y"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD31b=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD31b"], false) + "   \r\n");
                        sql.Append("  ,vcLevelD31y=" + ComFunction.getSqlValue(listInfoData[i]["vcLevelD31y"], false) + "   \r\n");
                        sql.Append("  where iAutoId=" + iAutoId + " ; \r\n");
                    }
                    excute.ExcuteSqlWithStringOper(sql.ToString());
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("-->") != -1)
                {//主动判断抛出的异常
                    int startIndex = ex.Message.IndexOf("-->");
                    int endIndex = ex.Message.LastIndexOf("<--");
                    strErrorPartId = ex.Message.Substring(startIndex + 3, endIndex - startIndex - 3);
                }
                else
                    throw ex;
            }
        }
        #endregion
    }
}
