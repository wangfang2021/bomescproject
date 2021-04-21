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
    public class FS1203_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        public DataTable getPlantype()
        {
            string ssql = " select '' as vcName ,'' as vcValue union all select planType,value from sPlanType where enable='1' order by vcValue";
            return excute.ExcuteSqlWithSelectToDT(ssql);
        }

        public DataTable getMonPackPlanTMP(string mon, string tablename, string plant)//获取临时表的值别计划
        {
            string tmpT = "";
            string tmpE = "";
            DateTime tim = Convert.ToDateTime(mon.Split('-')[0] + "-" + mon.Split('-')[1] + "-1");
            double daynum = (tim.AddMonths(1) - tim).TotalDays;

            for (int i = 1; i < 31 + 1; i++)
            {
                if (i == 31)
                    tmpE += "t2.vcD" + i + "b as ED" + i + "b, t2.vcD" + i + "y as ED" + i + "y";
                else tmpE += "t2.vcD" + i + "b as ED" + i + "b, t2.vcD" + i + "y as ED" + i + "y,";
            }
            double daynum2 = (tim - tim.AddMonths(-1)).TotalDays;
            for (int i = 1; i < 31 + 1; i++)
            {
                if (i == 31)
                    tmpT += "t1.vcD" + i + "b as TD" + i + "b, t1.vcD" + i + "y as	TD" + i + "y";
                else tmpT += "t1.vcD" + i + "b 	as	TD" + i + "b, t1.vcD" + i + "y as	TD" + i + "y,";
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(" select t2.vcMonth ,t5.vcData2 as vcPlant, SUBSTRING(t2.vcPartsno,0,6)+'-'+SUBSTRING(t2.vcPartsno,6,5)+'-'+SUBSTRING(t2.vcPartsno,11,2) as vcPartsno ,t2.vcDock,t2.vcCarType,t4.vcCalendar1,t4.vcCalendar2,t4.vcCalendar3,t4.vcCalendar4,");
            sb.AppendLine(" t3.vcPartNameCN as vcPartsNameCHN, t4.vcProName1 as vcProject1,t3.vcProType+'-'+t3.vcZB as vcProjectName, t3.vcHJ as vcCurrentPastCode,t2.vcMonTotal as vcMonTotal ,");
            sb.AppendFormat(" {0},", tmpT);
            sb.AppendFormat(" {0}", tmpE);
            sb.AppendFormat(" from ( select * from {0} where montouch is not null) t1 ", tablename);
            sb.AppendFormat(" full join (select * from {0} where montouch is null) t2", tablename);
            sb.AppendLine(" on t1.montouch=t2.vcMonth and t1.vcPartsno=t2.vcPartsno and t1.vcDock=t2.vcDock and t1.vcCarType=t2.vcCarType");
            sb.AppendLine(" left join (select distinct vcMonth,vcPartNameCN,vcZB,vcHJ,vcDock,vcCarType,vcPartsNo,vcProType,vcPlant,vcEDFlag from tPlanPartInfo) t3");
            sb.AppendLine(" on t3.vcPartsNo=t2.vcPartsNo and t3.vcDock=t2.vcDock and t3.vcCarType=t2.vcCarType and t3.vcMonth='" + mon + "' ");
            sb.AppendLine(" left join ProRuleMst t4");
            sb.AppendLine(" on t4.vcPorType=t3.vcProType and t4.vcZB=t3.vcZB");
            sb.AppendLine(" left join (select vcData1,vcData2 from ConstMst where vcDataId='kbplant') t5");
            sb.AppendLine(" on t3.vcPlant=t5.vcData1 ");
            sb.AppendFormat(" where t2.vcMonth='{0}' and t3.vcPlant='{1}' and t3.vcEDFlag='S' ", mon, plant);
            try
            {
                return excute.ExcuteSqlWithSelectToDT(sb.ToString());
            }
            catch
            {
                throw;
            }
        }

        public DataTable getMonPackPlanTMPcur(string mon, string tablename, string plant)//获取临时表的值别计划
        {
            string tmpT = "";
            string tmpE = "";
            DateTime tim = Convert.ToDateTime(mon.Split('-')[0] + "-" + mon.Split('-')[1] + "-1");
            double daynum = (tim.AddMonths(1) - tim).TotalDays;

            for (int i = 1; i < 31 + 1; i++)
            {
                if (i == 31)
                    tmpE += "t2.vcD" + i + "b  as ED" + i + "b, t2.vcD" + i + "y as ED" + i + "y";
                else tmpE += "t2.vcD" + i + "b 	as ED" + i + "b, t2.vcD" + i + "y as ED" + i + "y,";
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
            sb.AppendLine("   t3.vcPartsNameCHN, t4.vcProName1 as vcProject1,t3.vcPorType+'-'+t3.vcZB as vcProjectName, t3.vcCurrentPastCode,t2.vcMonTotal as vcMonTotal ,");
            sb.AppendFormat(" {0},", tmpT);
            sb.AppendFormat(" {0}", tmpE);
            sb.AppendFormat("  from ( select  * from   {0} where montouch is not null) t1 ", tablename);
            sb.AppendFormat("  full join (select * from {0} where montouch is null) t2", tablename);
            sb.AppendLine("  on t1.montouch = t2.vcMonth and t1.vcPartsno=t2.vcPartsno and t1.vcDock=t2.vcDock and t1.vcCarType=t2.vcCarType");
            sb.AppendLine("  left join (select * from tPartInfoMaster where dTimeFrom <='" + mon + "-01' and dTimeTo>= '" + mon + "-01' ) t3");
            sb.AppendLine("  on t3.vcPartsNo=t2.vcPartsNo and t3.vcDock = t2.vcDock and t3.vcCarFamilyCode = t2.vcCarType");
            sb.AppendLine("  left join dbo.ProRuleMst t4");
            sb.AppendLine("  on t4.vcPorType = t3.vcPorType and t4.vcZB = t3.vcZB");
            sb.AppendLine(" left join (select vcData1 ,vcData2  from dbo.ConstMst where vcDataId ='kbplant') t5");
            sb.AppendLine(" on t3.vcPartPlant = t5.vcData1 ");
            sb.AppendFormat("  where t2.vcMonth ='{0}' and t3.vcPartPlant ='{1}'", mon, plant);
            try
            {
                return excute.ExcuteSqlWithSelectToDT(sb.ToString());
            }
            catch
            {
                throw;
            }
        }

        public DataTable getMonProALL2(string mon, string TableSName, string TableEDName, string plant)
        {
            DataTable dtS = getMonPackPlanTMP(mon, TableSName, plant);
            addEDflag(ref dtS, "通常");
            DataTable dtE = getMonPlanED(mon, TableEDName, plant);
            addEDflag(ref dtE, "紧急");
            DataTable dtALL = MergeDataTable(dtS, dtE);
            return dtALL;
        }
        public void addEDflag(ref DataTable dt, string flag)
        {
            DataColumn col = new DataColumn();
            col.ColumnName = "vcEDflag";
            col.DataType = typeof(string);
            col.DefaultValue = flag;
            dt.Columns.Add(col);
        }

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

        public DataTable getMonPlanED(string mon, string TblName, string plant)
        {
            DataTable dt = new DataTable();
            string tmpT = "";
            string tmpE = "";
            DateTime tim = Convert.ToDateTime(mon.Split('-')[0] + "-" + mon.Split('-')[1] + "-1");
            double daynum = (tim.AddMonths(1) - tim).TotalDays;
            for (int i = 1; i < 31 + 1; i++)
            {
                if (i == 31)
                    tmpE += "t2.vcD" + i + "b as	ED" + i + "b,	t2.	vcD" + i + "y	as	ED" + i + "y";
                else tmpE += "t2.vcD" + i + "b	as	ED" + i + "b,	t2.vcD" + i + "y	as	ED" + i + "y,";
            }
            double daynum2 = (tim - tim.AddMonths(-1)).TotalDays;
            for (int i = 1; i < 31 + 1; i++)
            {
                if (i == 31)
                    tmpT += "t1.vcD" + i + "b 	as	TD" + i + "b,	t1.vcD" + i + "y	as	TD" + i + "y";
                else tmpT += "t1.vcD" + i + "b 	as	TD" + i + "b,	t1.vcD" + i + "y	as	TD" + i + "y,";
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("   select tt1.vcMonth,t5.vcData2 as vcPlant, SUBSTRING(tt1.vcPartsno,0,6)+'-'+SUBSTRING(tt1.vcPartsno,6,5)+'-'+SUBSTRING(tt1.vcPartsno,11,2) as vcPartsno,");
            sb.AppendLine("   tt1.vcDock,tt1.vcCarType ,t4.vcCalendar1,t4.vcCalendar2,t4.vcCalendar3,t4.vcCalendar4,t3.vcPartNameCN as vcPartsNameCHN,t4.vcProName1 as vcProject1,t3.vcProType+'-'+t3.vcZB as vcProjectName, t3.vcHJ as vcCurrentPastCode ,tt1.vcMonTotal ,");
            sb.AppendLine("  TD1b	,	TD1y	,");
            sb.AppendLine("  TD2b	,	TD2y	,");
            sb.AppendLine("  TD3b	,	TD3y	,");
            sb.AppendLine("  TD4b	,	TD4y	,");
            sb.AppendLine("  TD5b	,	TD5y	,");
            sb.AppendLine("  TD6b	,	TD6y	,");
            sb.AppendLine("  TD7b	,	TD7y	,");
            sb.AppendLine("  TD8b	,	TD8y	,");
            sb.AppendLine("  TD9b	,	TD9y	,");
            sb.AppendLine("  TD10b	,	TD10y	,");
            sb.AppendLine("  TD11b	,	TD11y	,");
            sb.AppendLine("  TD12b	,	TD12y	,");
            sb.AppendLine("  TD13b	,	TD13y	,");
            sb.AppendLine("  TD14b	,	TD14y	,");
            sb.AppendLine("  TD15b	,	TD15y	,");
            sb.AppendLine("  TD16b	,	TD16y	,");
            sb.AppendLine("  TD17b	,	TD17y	,");
            sb.AppendLine("  TD18b	,	TD18y	,");
            sb.AppendLine("  TD19b	,	TD19y	,");
            sb.AppendLine("  TD20b	,	TD20y	,");
            sb.AppendLine("  TD21b	,	TD21y	,");
            sb.AppendLine("  TD22b	,	TD22y	,");
            sb.AppendLine("  TD23b	,	TD23y	,");
            sb.AppendLine("  TD24b	,	TD24y	,");
            sb.AppendLine("  TD25b	,	TD25y	,");
            sb.AppendLine("  TD26b	,	TD26y	,");
            sb.AppendLine("  TD27b	,	TD27y	,");
            sb.AppendLine("  TD28b	,	TD28y	,");
            sb.AppendLine("  TD29b	,	TD29y	,");
            sb.AppendLine("  TD30b	,	TD30y	,");
            sb.AppendLine("  TD31b	,	TD31y	,");
            sb.AppendLine("  ED1b	,	ED1y	,");
            sb.AppendLine("  ED2b	,	ED2y	,");
            sb.AppendLine("  ED3b	,	ED3y	,");
            sb.AppendLine("  ED4b	,	ED4y	,");
            sb.AppendLine("  ED5b	,	ED5y	,");
            sb.AppendLine("  ED6b	,	ED6y	,");
            sb.AppendLine("  ED7b	,	ED7y	,");
            sb.AppendLine("  ED8b	,	ED8y	,");
            sb.AppendLine("  ED9b	,	ED9y	,");
            sb.AppendLine("  ED10b	,	ED10y	,");
            sb.AppendLine("  ED11b	,	ED11y	,");
            sb.AppendLine("  ED12b	,	ED12y	,");
            sb.AppendLine("  ED13b	,	ED13y	,");
            sb.AppendLine("  ED14b	,	ED14y	,");
            sb.AppendLine("  ED15b	,	ED15y	,");
            sb.AppendLine("  ED16b	,	ED16y	,");
            sb.AppendLine("  ED17b	,	ED17y	,");
            sb.AppendLine("  ED18b	,	ED18y	,");
            sb.AppendLine("  ED19b	,	ED19y	,");
            sb.AppendLine("  ED20b	,	ED20y	,");
            sb.AppendLine("  ED21b	,	ED21y	,");
            sb.AppendLine("  ED22b	,	ED22y	,");
            sb.AppendLine("  ED23b	,	ED23y	,");
            sb.AppendLine("  ED24b	,	ED24y	,");
            sb.AppendLine("  ED25b	,	ED25y	,");
            sb.AppendLine("  ED26b	,	ED26y	,");
            sb.AppendLine("  ED27b	,	ED27y	,");
            sb.AppendLine("  ED28b	,	ED28y	,");
            sb.AppendLine("  ED29b	,	ED29y	,");
            sb.AppendLine("  ED30b	,	ED30y	,");
            sb.AppendLine("  ED31b	,	ED31y	");
            sb.AppendLine("  ");
            sb.AppendLine("   from (");
            sb.AppendFormat("    select '{0}' as vcMonth ,", mon);
            sb.AppendLine("   case when t2.vcPartsno is null then t1.vcPartsno  else t2.vcPartsno end as vcPartsno  ,");
            sb.AppendLine("    case when t2.vcDock is null then t1.vcDock else t2.vcDock end as vcDock");
            sb.AppendLine("   ,case when t2.vcCarType is null then t1.vcCarType else t2.vcCarType end as vcCarType");
            sb.AppendLine("  ,'0' as vcMonTotal,");
            sb.AppendFormat(" {0},", tmpT);
            sb.AppendFormat(" {0}", tmpE);
            sb.AppendFormat("   from (select * from  {0} where montouch is not null ) t1  ", TblName);
            sb.AppendFormat("    full join (select * from  {0} where montouch is null ) t2", TblName);
            sb.AppendLine("    on t1.montouch = t2.vcMonth and t1.vcPartsno=t2.vcPartsno and t1.vcDock=t2.vcDock and t1.vcCarType=t2.vcCarType");
            sb.AppendFormat("     where (t1.montouch = '{0}' or t2.vcMonth ='{1}')", mon, mon);
            sb.AppendLine("    ) tt1");
            sb.AppendLine("   left join (select distinct vcMonth,vcPartsNo,vcDock,vcCarType,vcZB,vcProType,vcEDFlag,vcPlant,vcHJ,vcPartNameCN from dbo.tPlanPartInfo) t3");
            sb.AppendLine("     on t3.vcPartsNo=tt1.vcPartsNo and t3.vcDock = tt1.vcDock and t3.vcCarType = tt1.vcCarType and  t3.vcMonth = '" + mon + "' ");
            sb.AppendLine("     left join dbo.ProRuleMst t4");
            sb.AppendLine("   on t4.vcPorType = t3.vcProType and t4.vcZB = t3.vcZB");
            sb.AppendLine(" left join (select vcData1 ,vcData2  from dbo.ConstMst where vcDataId ='kbplant') t5");
            sb.AppendLine(" on t3.vcPlant = t5.vcData1 ");
            sb.AppendFormat("   where t3.vcPlant ='{0}' and vcEDFlag ='E' ", plant);
            try
            {
                dt = excute.ExcuteSqlWithSelectToDT(sb.ToString());
            }
            catch
            {
                throw;
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int a = 0;
                for (int j = 13; j < dt.Columns.Count; j++)
                {
                    a += Convert.ToInt32(dt.Rows[i][j].ToString().Length == 0 ? "0" : dt.Rows[i][j].ToString());
                }
                dt.Rows[i]["vcMonTotal"] = a.ToString();
            }
            return dt;
        }

        public DataTable getPartsno(string mon)
        {
            string tmpmon = mon + "-01";
            string ssql = "select vcPartsNo,vcDock,vcInOutFlag,vcQFflag from tPartInfoMaster where dTimeFrom<='" + tmpmon + "' and dTimeTo>='" + tmpmon + "'";
            return excute.ExcuteSqlWithSelectToDT(ssql);
        }

        #region 订单删减子页面
        #region 检索
        public DataTable getCutPlan(string mon)
        {
            DataTable dt = new DataTable();
            string ssql = " SELECT vcMonth,vcPartsno,vcDock,vcKBorderno,vcKBSerial,vcEDflag,'0' as iFlag,'0' as vcModFlag,'0' as vcAddFlag,iAutoId FROM tPlanCut where updateFlag='0' ";
            if (mon != null && mon.Length > 0)
            {
                ssql += " and vcMonth='" + mon + "'";
                dt = excute.ExcuteSqlWithSelectToDT(ssql);
            }
            else
            {
                dt = excute.ExcuteSqlWithSelectToDT(ssql);
            }
            return dt;
        }
        #endregion

        #region 删除
        public void Del_Plan(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("  delete tPlanCut where iAutoId in(   \r\n ");
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
        #endregion

        public void Del_KanbanPrintTbl(List<Dictionary<string, Object>> listInfoData)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    sql.Append("delete from tKanbanPrintTbl where vcPartsNo+vcKBorderno+vcKBSerial='" + listInfoData[i]["vcPartsNo"].ToString() + listInfoData[i]["vcKBorderno"].ToString() + listInfoData[i]["vcKBSerial"].ToString() + "'; \r\n");
                }
                excute.ExcuteSqlWithStringOper(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region 保存
        public string UpdateCutPlanTMP(DataTable dt, string user)
        {
            string msg = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                DataColumn[] col = new DataColumn[5];
                col[0] = dt.Columns[0];
                col[1] = dt.Columns[1];
                col[2] = dt.Columns[2];
                col[3] = dt.Columns[3];
                col[4] = dt.Columns[4];
                dt.PrimaryKey = col;
            }
            catch
            {
                msg = "更新列表含重复行,请删除后在进行更新。";
                return msg;
            }
            try
            {
                cmd.Connection = new SqlConnection(ComConnectionHelper.GetConnectionString());
                cmd.CommandTimeout = 0;
                cmd.Connection.Open();
                cmd.Transaction = cmd.Connection.BeginTransaction();
                SqlDataAdapter apt = new SqlDataAdapter(cmd);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["vcMonth"].ToString().Length <= 0
                        || dt.Rows[i]["vcPartsno"].ToString().Length <= 0
                        || dt.Rows[i]["vcKBorderno"].ToString().Length <= 0
                        || dt.Rows[i]["vcKBSerial"].ToString().Length <= 0
                        || dt.Rows[i]["vcEDflag"].ToString().Length <= 0
                        || dt.Rows[i]["vcDock"].ToString().Length <= 0)
                    {
                        msg = " 基础信息不能为空，请填充完整再进行更新。";
                        return msg;
                    }
                    else
                    {
                        string tmpsql = "select * from tKanbanPrintTbl where vcDock='" + dt.Rows[i]["vcDock"].ToString() + "' and vcPartsNo='" + dt.Rows[i]["vcPartsno"].ToString() + "'  and vcKBorderno='" + dt.Rows[i]["vcKBorderno"].ToString() + "' and vcKBSerial='" + dt.Rows[i]["vcKBSerial"].ToString() + "' and vcPlanMonth='" + dt.Rows[i]["vcMonth"].ToString() + "' and vcEDflag='" + dt.Rows[i]["vcEDflag"].ToString() + "'";
                        cmd.CommandText = tmpsql;
                        DataTable dttmp = new DataTable();
                        apt.Fill(dttmp);
                        if (dttmp.Rows.Count == 0)
                        {
                            msg = "品番：" + dt.Rows[i]["vcPartsno"].ToString() + ",受入：" + dt.Rows[i]["vcDock"].ToString() + ",订单号：" + dt.Rows[i]["vcKBorderno"].ToString() + ",连番：" + dt.Rows[i]["vcKBSerial"].ToString() + ",订单信息不存在。";
                            return msg;
                        }
                    }
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string ssql = "select top(1) * from tPlanCut where vcPartsno='" + dt.Rows[i]["vcPartsno"].ToString() + "' and vcMonth='" + dt.Rows[i]["vcMonth"].ToString() + "' and vcKBorderno='" + dt.Rows[i]["vcKBorderno"].ToString() + "' and vcKBSerial='" + dt.Rows[i]["vcKBSerial"].ToString() + "' and updateFlag<>'1'";
                    cmd.CommandText = ssql;
                    DataTable dtExist = new DataTable();
                    apt.Fill(dtExist);
                    if (dtExist.Rows.Count > 0)
                    {
                        msg = "品番：" + dt.Rows[i]["vcPartsno"].ToString() + ",受入：" + dt.Rows[i]["vcDock"].ToString() + ",订单号：" + dt.Rows[i]["vcKBorderno"].ToString() + ",连番：" + dt.Rows[i]["vcKBSerial"].ToString() + ",订单信息已经存在。";
                        return msg;
                    }
                    StringBuilder sb = new StringBuilder();
                    sb.Length = 0;
                    sb.AppendLine(" INSERT INTO tPlanCut (vcMonth,vcPartsno, vcKBorderno,vcKBSerial,vcEDflag,updateFlag,dCreatTime,vcUpdateID,vcDock)");
                    sb.AppendFormat(" VALUES ('{0}'", dt.Rows[i]["vcMonth"].ToString());
                    sb.AppendFormat(" ,'{0}' ", dt.Rows[i]["vcPartsno"].ToString());
                    sb.AppendFormat(" ,'{0}' ", dt.Rows[i]["vcKBorderno"].ToString());
                    sb.AppendFormat(" ,'{0}' ", dt.Rows[i]["vcKBSerial"].ToString());
                    sb.AppendFormat(" ,'{0}' ", dt.Rows[i]["vcEDflag"].ToString());
                    sb.AppendFormat(" ,'{0}' ", "0");
                    sb.AppendLine(" ,getdate() ");
                    sb.AppendFormat(" ,'{0}' ", user);
                    sb.AppendFormat(" ,'{0}') ", dt.Rows[i]["vcDock"].ToString());
                    cmd.CommandText = sb.ToString();
                    cmd.ExecuteNonQuery();
                }
                cmd.Transaction.Commit();
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                msg = "更新失败。";
                if (cmd.Connection.State == ConnectionState.Open)
                {
                    cmd.Transaction.Rollback();
                    cmd.Connection.Close();
                }
            }
            return msg;
        }
        #endregion

        #region 更新到计划
        public string UpdatePlan(string mon, string user)//从临时表更新到计划
        {
            string msg = "";
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(ComConnectionHelper.GetConnectionString());
            cmd.CommandTimeout = 0;
            cmd.Connection.Open();
            cmd.Transaction = cmd.Connection.BeginTransaction();
            try
            {
                SqlDataAdapter apt = new SqlDataAdapter(cmd);
                //获取该月要削减的计划
                DataTable dtCut = getCutPlan(mon, cmd, apt);
                if (dtCut.Rows.Count == 0)
                {
                    msg = "无可删除的订单数据，请新增。";
                    return msg;
                }
                //分类：紧急计划削减，月度计划削减
                DataTable dtCut_S = dtCut.Select(" vcEDflag='S'").Length == 0 ? new DataTable() : dtCut.Select(" vcEDflag='S'").CopyToDataTable();
                DataTable dtCut_E = dtCut.Select(" vcEDflag='E'").Length == 0 ? new DataTable() : dtCut.Select(" vcEDflag='E'").CopyToDataTable();
                //月度计划削减处理
                if (dtCut_S.Rows.Count > 0)
                {
                    msg = MonPlanCut(mon, dtCut_S, cmd, apt);
                }
                //紧急计划削减处理
                if (dtCut_E.Rows.Count > 0)
                {
                    msg = EDPlanCut(mon, dtCut_E, cmd, apt);
                    //更新EDMonthPlanTMP 存储表 --仅限紧急计划
                    CutEDMonthPlanTMP(mon, dtCut_E, cmd, apt);
                }
                //删除削减到0的计划。
                //PlanDelIs0(mon, cmd, apt);
                //更新削减临时表
                cmd.CommandText = " update tPlanCut set updateFlag='1',dUpdateTime=getdate(),vcUpdateID='" + user + "' where vcMonth='" + mon + "'";
                cmd.ExecuteNonQuery();
                //更新SOQ导出表 ？？？
                if (msg.Length > 0)
                {
                    msg = "error";
                    cmd.Transaction.Rollback();
                    cmd.Connection.Close();
                }
                else
                {
                    cmd.Transaction.Commit();
                    cmd.Connection.Close();
                }
            }
            catch (Exception ex)
            {
                if (cmd.Connection.State == ConnectionState.Open)
                {
                    cmd.Transaction.Rollback();
                    cmd.Connection.Close();
                }
                throw ex;
            }
            return msg;
        }
        #endregion

        public string checkExcel(DataTable dt, ref DataTable dtre)
        {
            string msg = "";
            msg = checkExcelData(ref dt);//校验数据
            dtre = dt;
            return msg;
        }
        public string checkExcelData(ref DataTable dt)
        {
            string msg = "";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][0].ToString().Trim().Length == 0 || dt.Rows[i][1].ToString().Trim().Length == 0 || dt.Rows[i][2].ToString().Trim().Length == 0 || dt.Rows[i][3].ToString().Trim().Length == 0 || dt.Rows[i][4].ToString().Trim().Length == 0)
                {
                    msg = "第" + (i + 2) + "行，信息填写不完整。";
                    return msg;
                }
            }

            dt.Columns[0].ColumnName = "vcMonth";
            dt.Columns[1].ColumnName = "vcPartsno";
            dt.Columns[2].ColumnName = "vcDock";
            dt.Columns[3].ColumnName = "vcKBorderno";
            dt.Columns[4].ColumnName = "vcKBSerial";
            dt.Columns[5].ColumnName = "vcEDflag";
            DataTable dt1 = new DataTable();
            if (dt.Select("vcKBorderno<>'ALL' and vcKBSerial<>'ALL'").Length > 0)
                dt1 = dt.Select("vcKBorderno<>'ALL' and vcKBSerial<>'ALL'").CopyToDataTable();
            else
                dt1 = dt.Clone();
            DataTable dt2 = new DataTable();
            if (dt.Select("vcKBorderno='ALL' or vcKBSerial='ALL'").Length > 0)
                dt2 = dt.Select("vcKBorderno='ALL' or vcKBSerial='ALL'").CopyToDataTable();
            else
                dt2 = dt.Clone();
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                string mon = dt1.Rows[i]["vcMonth"].ToString().Trim();
                string part = dt1.Rows[i]["vcPartsno"].ToString().Trim().Replace("-", "");
                string dock = dt1.Rows[i]["vcDock"].ToString().Trim();
                string order = dt1.Rows[i]["vcKBorderno"].ToString().Trim();
                string serial = dt1.Rows[i]["vcKBSerial"].ToString().Trim();
                if (dt1.Rows[i]["vcEDflag"].ToString().Trim() != "通常" && dt1.Rows[i]["vcEDflag"].ToString().Trim() != "紧急" && dt1.Rows[i]["vcEDflag"].ToString().Trim() != "S" && dt1.Rows[i]["vcEDflag"].ToString().Trim() != "E" && dt1.Rows[i]["vcEDflag"].ToString().Trim() != "S：通常" && dt1.Rows[i]["vcEDflag"].ToString().Trim() != "E：紧急")
                {
                    msg = "品番：" + part + ",受入：" + dock + ",订单号：" + order + ",连番：" + serial + ",紧急区分输入非法。";
                    break;
                }
                string ed = (dt1.Rows[i]["vcEDflag"].ToString().Trim() == "通常" || dt1.Rows[i]["vcEDflag"].ToString().Trim() == "S" || dt1.Rows[i]["vcEDflag"].ToString().Trim() == "S:通常") ? "S" : "E";
                if (mon.Length == 0 && part.Length == 0 && order.Length == 0 && serial.Length == 0 && ed.Length == 0)
                {
                    break;
                }
                string tmpsql = "select * from tKanbanPrintTbl where vcPartsNo='" + part + "'  and vcKBorderno='" + order + "' and vcKBSerial='" + serial + "' and vcPlanMonth='" + mon + "' and vcEDflag='" + ed + "'  and vcDock='" + dock + "'";
                DataTable dttmp = excute.ExcuteSqlWithSelectToDT(tmpsql);
                if (dttmp.Rows.Count == 0)
                {
                    msg = "品番：" + part + ",受入：" + dock + ",订单号：" + order + ",连番：" + serial + ",订单信息不存在。";
                    break;
                }
                else
                {
                    dt1.Rows[i]["vcEDflag"] = ed;
                }
            }
            // DataTable dt2_tmp = dt2.Clone();
            for (int i = 0; i < dt2.Rows.Count; i++)
            {
                string mon = dt2.Rows[i]["vcMonth"].ToString().Trim();
                string part = dt2.Rows[i]["vcPartsno"].ToString().Trim().Replace("-", "");
                string dock = dt2.Rows[i]["vcDock"].ToString().Trim();
                string order = dt2.Rows[i]["vcKBorderno"].ToString().Trim();
                string serial = dt2.Rows[i]["vcKBSerial"].ToString().Trim();
                string ed = (dt2.Rows[i]["vcEDflag"].ToString().Trim() == "通常" || dt2.Rows[i]["vcEDflag"].ToString().Trim() == "S") ? "S" : "E";
                if (order == "ALL" && serial == "ALL")
                {
                    string ssql = " select vcPlanMonth, vcPartsNo, vcDock,vcKBorderno, vcKBSerial, vcEDflag from tKanbanPrintTbl where vcPartsNo='" + part + "' and vcPlanMonth='" + mon + "' and vcEDflag='" + ed + "' and vcDock='" + dock + "'";
                    DataTable tmp = excute.ExcuteSqlWithSelectToDT(ssql);
                    if (tmp.Rows.Count == 0)
                    {
                        msg = "品番：" + part + ",受入：" + dock + ",订单号：" + order + ",连番：" + serial + ",订单信息不存在。";
                        break;
                    }
                    for (int j = 0; j < tmp.Rows.Count; j++)
                    {
                        DataRow dr = dt1.NewRow();
                        dr["vcMonth"] = tmp.Rows[j]["vcPlanMonth"];
                        dr["vcPartsno"] = tmp.Rows[j]["vcPartsNo"];
                        dr["vcDock"] = tmp.Rows[j]["vcDock"];
                        dr["vcKBorderno"] = tmp.Rows[j]["vcKBorderno"];
                        dr["vcKBSerial"] = tmp.Rows[j]["vcKBSerial"];
                        dr["vcEDflag"] = tmp.Rows[j]["vcEDflag"];
                        dt1.Rows.Add(dr);
                    }
                }
                else if (order != "ALL" && serial == "ALL")
                {
                    string ssql = " select vcPlanMonth, vcPartsNo, vcDock, vcKBorderno, vcKBSerial, vcEDflag from tKanbanPrintTbl where vcPartsNo='" + part + "' and  vcPlanMonth='" + mon + "' and vcKBorderno='" + order + "' and vcDock='" + dock + "' ";
                    DataTable tmp = excute.ExcuteSqlWithSelectToDT(ssql);
                    if (tmp.Rows.Count == 0)
                    {
                        msg = "品番：" + part + ",受入：" + dock + ",订单号：" + order + ",连番：" + serial + ",订单信息不存在。";
                        break;
                    }
                    for (int j = 0; j < tmp.Rows.Count; j++)
                    {
                        DataRow dr = dt1.NewRow();
                        dr["vcMonth"] = tmp.Rows[j]["vcPlanMonth"];
                        dr["vcPartsno"] = tmp.Rows[j]["vcPartsNo"];
                        dr["vcDock"] = tmp.Rows[j]["vcDock"];
                        dr["vcKBorderno"] = tmp.Rows[j]["vcKBorderno"];
                        dr["vcKBSerial"] = tmp.Rows[j]["vcKBSerial"];
                        dr["vcEDflag"] = tmp.Rows[j]["vcEDflag"];
                        dt1.Rows.Add(dr);
                    }
                }
                else if (order == "ALL" && serial != "ALL")
                {
                    string ssql = " select vcPlanMonth, vcPartsNo,vcDock,vcKBorderno,vcKBSerial, vcEDflag from tKanbanPrintTbl where vcPartsNo='" + part + "' and  vcPlanMonth='" + mon + "' and vcKBSerial='" + serial + "' and vcEDflag='" + ed + "' and vcDock='" + dock + "'";
                    DataTable tmp = excute.ExcuteSqlWithSelectToDT(ssql);
                    if (tmp.Rows.Count == 0)
                    {
                        msg = "品番：" + part + ",受入：" + dock + ",订单号：" + order + ",连番：" + serial + ",订单信息不存在。";
                        break;
                    }
                    for (int j = 0; j < tmp.Rows.Count; j++)
                    {
                        DataRow dr = dt1.NewRow();
                        dr["vcMonth"] = tmp.Rows[j]["vcPlanMonth"];
                        dr["vcPartsno"] = tmp.Rows[j]["vcPartsNo"];
                        dr["vcDock"] = tmp.Rows[j]["vcDock"];
                        dr["vcKBorderno"] = tmp.Rows[j]["vcKBorderno"];
                        dr["vcKBSerial"] = tmp.Rows[j]["vcKBSerial"];
                        dr["vcEDflag"] = tmp.Rows[j]["vcEDflag"];
                        dt1.Rows.Add(dr);
                    }
                }
            }
            dt = dt1.Copy();
            return msg;
        }
        public string checkExcelHeadpos(DataTable dt, DataTable dtTmplate)
        {
            string msg = "";
            if (dt.Columns.Count != dtTmplate.Columns.Count)
            {
                return msg = "使用模板错误！";
            }
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (dt.Columns[i].ColumnName.ToString().Trim() != dtTmplate.Columns[i].ColumnName.ToString().Trim())
                {
                    if (ExcelPos(i) != "error")
                        return msg = "模板" + ExcelPos(i) + "列错误！";
                }
            }
            return msg;
        }
        public string ExcelPos(int i)//取得列位置
        {
            string re = "error";
            List<string> A = new List<string>();
            A.Add("A");
            A.Add("B");
            A.Add("C");
            A.Add("D");
            A.Add("E");
            A.Add("F");
            A.Add("G");
            A.Add("H");
            A.Add("I");
            A.Add("J");
            A.Add("K");
            A.Add("L");
            A.Add("M");
            A.Add("N");
            A.Add("O");
            A.Add("P");
            A.Add("Q");
            A.Add("R");
            A.Add("S");
            A.Add("T");
            A.Add("U");
            A.Add("V");
            A.Add("W");
            A.Add("X");
            A.Add("Y");
            A.Add("Z");
            if (i < 26) re = A[i];
            if (i >= 26) re = A[(i / 26) - 1] + A[i % 26];
            return re;
        }
        public DataTable getCutPlan(string mon, SqlCommand cmd, SqlDataAdapter apt)
        {
            DataTable dt = new DataTable();
            string ssql = "  ";
            ssql += " SELECT t1.vcMonth , t1.vcPartsno ,t1.vcDock,t1.vcKBorderno , t1.vcKBSerial , t1.vcEDflag ,";
            ssql += " t2.vcComDate00 , t2.vcComDate01,t2.vcComDate02,t2.vcComDate03,t2.vcComDate04 ,";
            ssql += " t2.vcBanZhi00, t2.vcBanZhi01, t2.vcBanZhi02, t2.vcBanZhi03, t2.vcBanZhi04,t2.vcQuantityPerContainer as srs";
            ssql += "  FROM tPlanCut  t1";
            ssql += " left join dbo.tKanbanPrintTbl t2";
            ssql += " on t1.vcPartsno = t2.vcPartsNo and t1.vcKBorderno = t2.vcKBorderno and t1.vcKBSerial = t2.vcKBSerial and t1.vcDock = t2.vcDock ";
            ssql += " where vcMonth='" + mon + "' and t1.updateFlag ='0' order by vcEDflag ";
            cmd.CommandText = ssql;
            apt.Fill(dt);
            return dt;
        }
        #endregion

        public string MonPlanCut(string mon, DataTable dt, SqlCommand cmd, SqlDataAdapter apt)
        {
            string msg = "";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string Parts = dt.Rows[i]["vcPartsno"].ToString();
                string dock = dt.Rows[i]["vcDock"].ToString();
                string order = dt.Rows[i]["vcKBorderno"].ToString();
                string serial = dt.Rows[i]["vcKBSerial"].ToString();
                string P0date = dt.Rows[i]["vcComDate00"].ToString();
                string P0zhi = dt.Rows[i]["vcBanZhi00"].ToString();
                string P1date = dt.Rows[i]["vcComDate01"].ToString();
                string P1zhi = dt.Rows[i]["vcBanZhi01"].ToString();
                string P2date = dt.Rows[i]["vcComDate02"].ToString();
                string P2zhi = dt.Rows[i]["vcBanZhi02"].ToString();
                string P3date = dt.Rows[i]["vcComDate03"].ToString();
                string P3zhi = dt.Rows[i]["vcBanZhi03"].ToString();
                string P4date = dt.Rows[i]["vcComDate04"].ToString();
                string P4zhi = dt.Rows[i]["vcBanZhi04"].ToString();
                int srs = Convert.ToInt32(dt.Rows[i]["srs"].ToString());
                //P0计划
                if (P0date.Length > 0 && P0zhi.Length > 0)
                {
                    string sql0 = cutPlanS("MonthKanBanPlanTbl", mon, P0date, P0zhi, Parts, dock, srs);
                    cmd.CommandText = sql0;
                    cmd.ExecuteNonQuery();
                }
                //P1计划
                if (P1date.Length > 0 && P1zhi.Length > 0)
                {
                    string sql1 = cutPlanS("MonthProdPlanTbl", mon, P1date, P1zhi, Parts, dock, srs);
                    cmd.CommandText = sql1;
                    cmd.ExecuteNonQuery();
                }
                //P2计划
                if (P2date.Length > 0 && P2zhi.Length > 0)
                {
                    string sql2 = cutPlanS("MonthTZPlanTbl", mon, P2date, P2zhi, Parts, dock, srs);
                    cmd.CommandText = sql2;
                    cmd.ExecuteNonQuery();
                }
                //P3计划
                if (P3date.Length > 0 && P3zhi.Length > 0)
                {
                    string sql3 = cutPlanS("MonthP3PlanTbl", mon, P3date, P3zhi, Parts, dock, srs);
                    cmd.CommandText = sql3;
                    cmd.ExecuteNonQuery();
                }
                //P4计划
                if (P4date.Length > 0 && P4zhi.Length > 0)
                {
                    string sql4 = cutPlanS("MonthPackPlanTbl", mon, P4date, P4zhi, Parts, dock, srs);
                    cmd.CommandText = sql4;
                    cmd.ExecuteNonQuery();
                }
                string sql5 = "delete from tKanbanPrintTbl where vcKBorderno ='" + order + "' and vcKBSerial ='" + serial + "' and vcPartsNo ='" + Parts + "' and vcDock ='" + dock + "' ";
                cmd.CommandText = sql5;
                cmd.ExecuteNonQuery();
            }
            return msg;
        }

        public string cutPlanS(string tablename, string mon, string P0date, string P0zhi, string Parts, string dock, int srs)
        {
            string sqltmp0 = "";
            string day = "";
            if (Convert.ToDateTime(P0date).ToString("yyyy-MM") == mon)
            {
                sqltmp0 = " and vcMonth ='" + mon + "' and montouch is null";
            }
            else
            {
                sqltmp0 = " and vcMonth ='" + Convert.ToDateTime(P0date).ToString("yyyy-MM") + "' and montouch ='" + mon + "'";
            }
            day = "vcD" + Convert.ToDateTime(P0date).Day.ToString() + (P0zhi == "0" ? "b" : "y");

            string sqltmp1 = day + "=" + day + "-" + srs;
            string ssql0 = "update " + tablename + " set " + sqltmp1 + "  where 1=1 " + sqltmp0 + " and vcPartsno ='" + Parts + "' and vcDock ='" + dock + "';";
            ssql0 += " update " + tablename + " set vcMonTotal = vcMonTotal-" + srs + " where vcMonth ='" + mon + "' and montouch is null and vcPartsno ='" + Parts + "' and vcDock ='" + dock + "';";
            return ssql0;
        }

        public string EDPlanCut(string mon, DataTable dt, SqlCommand cmd, SqlDataAdapter apt)
        {
            string msg = "";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string Parts = dt.Rows[i]["vcPartsno"].ToString();
                string dock = dt.Rows[i]["vcDock"].ToString();
                string order = dt.Rows[i]["vcKBorderno"].ToString();
                string serial = dt.Rows[i]["vcKBSerial"].ToString();
                string P0date = dt.Rows[i]["vcComDate00"].ToString();
                string P0zhi = dt.Rows[i]["vcBanZhi00"].ToString();
                string P1date = dt.Rows[i]["vcComDate01"].ToString();
                string P1zhi = dt.Rows[i]["vcBanZhi01"].ToString();
                string P2date = dt.Rows[i]["vcComDate02"].ToString();
                string P2zhi = dt.Rows[i]["vcBanZhi02"].ToString();
                string P3date = dt.Rows[i]["vcComDate03"].ToString();
                string P3zhi = dt.Rows[i]["vcBanZhi03"].ToString();
                string P4date = dt.Rows[i]["vcComDate04"].ToString();
                string P4zhi = dt.Rows[i]["vcBanZhi04"].ToString();
                int srs = Convert.ToInt32(dt.Rows[i]["srs"].ToString());
                //P0计划
                if (P0date.Length > 0 && P0zhi.Length > 0)
                {
                    string sql0 = cutPlanE("dbo.EDMonthKanBanPlanTbl", mon, P0date, P0zhi, Parts, dock, srs);
                    cmd.CommandText = sql0;
                    cmd.ExecuteNonQuery();
                }
                //P1计划
                if (P1date.Length > 0 && P1zhi.Length > 0)
                {
                    string sql1 = cutPlanE("dbo.EDMonthProdPlanTbl", mon, P1date, P1zhi, Parts, dock, srs);
                    cmd.CommandText = sql1;
                    cmd.ExecuteNonQuery();
                }
                //P2计划
                if (P2date.Length > 0 && P2zhi.Length > 0)
                {
                    string sql2 = cutPlanE("dbo.EDMonthTZPlanTbl", mon, P2date, P2zhi, Parts, dock, srs);
                    cmd.CommandText = sql2;
                    cmd.ExecuteNonQuery();
                }
                //P3计划
                if (P3date.Length > 0 && P3zhi.Length > 0)
                {
                    string sql3 = cutPlanE("dbo.EDMonthP3PlanTbl", mon, P3date, P3zhi, Parts, dock, srs);
                    cmd.CommandText = sql3;
                    cmd.ExecuteNonQuery();
                }
                //P4计划
                if (P4date.Length > 0 && P4zhi.Length > 0)
                {
                    string sql4 = cutPlanE("dbo.EDMonthPackPlanTbl", mon, P4date, P4zhi, Parts, dock, srs);
                    cmd.CommandText = sql4;
                    cmd.ExecuteNonQuery();
                }
                string sql5 = "delete from tKanbanPrintTbl where vcKBorderno ='" + order + "' and vcKBSerial ='" + serial + "' and vcPartsno ='" + Parts + "' and vcDock ='" + dock + "';  ";
                cmd.CommandText = sql5;
                cmd.ExecuteNonQuery();
            }

            return msg;
        }

        public string cutPlanE(string tablename, string mon, string P0date, string P0zhi, string Parts, string dock, int srs)
        {
            string sqltmp0 = "";
            string day = "";
            if (Convert.ToDateTime(P0date).ToString("yyyy-MM") == mon)
            {
                sqltmp0 = " and vcMonth ='" + mon + "' and montouch is null";
            }
            else
            {
                sqltmp0 = " and vcMonth ='" + Convert.ToDateTime(P0date).ToString("yyyy-MM") + "' and montouch ='" + mon + "'";
            }
            day = "vcD" + Convert.ToDateTime(P0date).Day.ToString() + (P0zhi == "0" ? "b" : "y");

            string sqltmp1 = day + "=" + day + "-" + srs;

            string ssql0 = "update " + tablename + " set " + sqltmp1 + "  where 1=1 " + sqltmp0 + " and vcPartsno ='" + Parts + "' and vcDock ='" + dock + "';";
            return ssql0;
        }

        public void CutEDMonthPlanTMP(string mon, DataTable dt, SqlCommand cmd, SqlDataAdapter apt)
        {
            foreach (DataRow dr in dt.Rows)
            {
                StringBuilder SSQL = new StringBuilder();
                SSQL.AppendFormat("update EDMonthPlanTMP  set vcNum = vcNum- {0}", Convert.ToInt32(dr["srs"]));
                SSQL.AppendFormat(" where vcMonth ='{0}' and vcPartsno ='{1}' and vcDock ='{2}' and vcOrderNo ='{3}' ;", mon, dr["vcPartsno"].ToString(), dr["vcDock"].ToString(), dr["vcKBorderno"].ToString());
                SSQL.AppendLine(" delete from EDMonthPlanTMP where vcNum =0;");
                cmd.CommandText = SSQL.ToString();
                cmd.ExecuteNonQuery();
            }
        }
        public string MonSQL(string Mon)
        {
            if (Mon == "1" || Mon == "01")
                return "tJanuary";
            else if (Mon == "2" || Mon == "02")
                return "tFebruary";
            else if (Mon == "3" || Mon == "03")
                return "tMarch";
            else if (Mon == "4" || Mon == "04")
                return "tApril";
            else if (Mon == "5" || Mon == "05")
                return "tMay";
            else if (Mon == "6" || Mon == "06")
                return "tJune";
            else if (Mon == "7" || Mon == "07")
                return "tJuly";
            else if (Mon == "8" || Mon == "08")
                return "tAugust";
            else if (Mon == "9" || Mon == "09")
                return "tSeptember";
            else if (Mon == "10")
                return "tOctober";
            else if (Mon == "11")
                return "tNovember";
            else if (Mon == "12")
                return "tDecember";
            else return "";
        }
        private string getClassSql(string date, string dayNight, string vcPlant, string vcGC, string vcZB)
        {
            string year = date.Split('-')[0];
            string month = date.Split('-')[1];
            string day = int.Parse(date.Split('-')[2]).ToString();
            string by = dayNight == "0" ? "b" : "y";//20180921这句话是获取AB值的关键，dayNight是0或1，不可能是00 - 李兴旺
            string MonTable = MonSQL(month);//20180917获取数字月份对应的数据库表名 - 李兴旺
            string strResult = "select D" + day + by + " from " + MonTable + " where vcPlant='" + vcPlant + "' and vcYear='" + year + "' and vcMonth='" + month + "' and vcGC='" + vcGC + "' and vcZB='" + vcZB + "'";
            return strResult;
        }
        public string getABClass(string date, string dayNight, string vcPlant, string vcCalendar)
        {
            if (vcCalendar.Length > 0)
            {
                string vcGC = vcCalendar.Split('-')[0];
                string vcZB = vcCalendar.Split('-')[1];
                DataTable dt = excute.ExcuteSqlWithSelectToDT(getClassSql(date, dayNight, vcPlant, vcGC, vcZB));
                object content = dt.Rows[0][0];
                if (dt.Rows.Count > 1)
                {
                    throw new Exception("Date:" + date + ";vcPlant:" + vcPlant + ";vcGC:" + vcGC + ";vcZB:" + vcZB + "; 对应的稼动日不止一条");
                }
                else if (content != null && content.ToString().Length > 0)
                {
                    return content.ToString().Substring(1);//若为MA\MB\NA\NB则返回值为A\B或空
                }
            }
            return string.Empty;
        }
    }
}
