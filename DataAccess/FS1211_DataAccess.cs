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
    public class FS1211_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        public DataTable SearchData(string strSql)
        {
            return excute.ExcuteSqlWithSelectToDT(strSql);
        }

        public DataSet getOPRData(string mon, string vcTF, string vcTO, string vcPartsno, string vcDock, DataTable dtEDOrder)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add();
            ds.Tables.Add();
            SqlCommand ocmd = new SqlCommand();
            ocmd.Connection = new SqlConnection(ComConnectionHelper.GetConnectionString());
            ocmd.CommandText = "  ";
            ocmd.CommandText += "  select distinct topr.*,titem.Otype from ";
            ocmd.CommandText += "  (select t1.partsno,";
            ocmd.CommandText += "         t1.cpdcompany,";
            ocmd.CommandText += "         t1.dock,";
            ocmd.CommandText += "         t1.kanbanorderno,";
            ocmd.CommandText += "         sum(quantity)  as num   ";
            ocmd.CommandText += "    from sp_m_opr t1";
            ocmd.CommandText += "     where t1.dataid = 'S0'";
            if (mon.Length > 0)
            {
                DateTime monCur = Convert.ToDateTime(mon);
                string monLast = Convert.ToDateTime(mon).AddMonths(-1).ToString("yyyyMM");
                ocmd.CommandText += "       and substring(t1.kanbanorderno,0,7)='" + monLast.Replace("-", "") + "'";
            }
            if (vcTF.Length > 0)
            {
                ocmd.CommandText += "        and t1.daddtime>=CONVERT(datetime,'" + vcTF + "')";
            }
            if (vcTO.Length > 0)
            {
                ocmd.CommandText += "        and t1.daddtime<=CONVERT(datetime,'" + vcTO + "')";
            }
            ocmd.CommandText += "  group by partsno,cpdcompany,dock,kanbanorderno";
            ocmd.CommandText += "        union all ";
            ocmd.CommandText += "  select t1.partsno,";
            ocmd.CommandText += "         t1.cpdcompany,";
            ocmd.CommandText += "         t1.dock,";
            ocmd.CommandText += "         t1.kanbanorderno,";
            ocmd.CommandText += "         sum(quantity)  as num   ";
            ocmd.CommandText += "    from sp_m_opr t1";
            ocmd.CommandText += "     where t1.dataid = 'S0'";
            if (mon.Length > 0)
            {
                DateTime monCur = Convert.ToDateTime(mon);
                string monLast = Convert.ToDateTime(mon).AddMonths(-1).ToString("yyyyMM");
                ocmd.CommandText += "       and substring(t1.kanbanorderno,0,7)='" + mon.Replace("-", "") + "'";
            }
            if (vcTF.Length > 0)
            {
                ocmd.CommandText += "        and t1.daddtime>=CONVERT(datetime,'" + vcTF + "')";
            }
            if (vcTO.Length > 0)
            {
                ocmd.CommandText += "        and t1.daddtime<=CONVERT(datetime,'" + vcTO + "')";
            }
            ocmd.CommandText += "  group by partsno,cpdcompany,dock,kanbanorderno ";
            ocmd.CommandText += "   ) topr";
            ocmd.CommandText += "   left join ";
            ocmd.CommandText += "   (";
            ocmd.CommandText += "   select partsno, inoutflag, dock ,'T0' as Otype from sp_m_sitem union all  select partsno,'0',dock,'T1' as Otype from sp_m_edsitem";
            ocmd.CommandText += " )";
            ocmd.CommandText += "  titem on";
            ocmd.CommandText += " topr.partsno = titem.partsno and topr.dock = titem.dock ";
            ocmd.CommandText += "  where titem.inoutflag='0' ";
            //根据dtEDOrder 找相同品番订单号的入库信息
            string tmpsql = "";
            for (int i = 0; i < dtEDOrder.Rows.Count; i++)
            {
                string part = dtEDOrder.Rows[i]["vcPartsno"].ToString();
                string parttmp = part.Substring(part.Length - 2, 2) == "ED" ? part.Substring(0, 10) : part;
                tmpsql += "select partsno,cpdcompany,dock,kanbanorderno ,kanbanserial , quantity as num,'TO' as OType from sp_m_opr t ";
                tmpsql += " where dataid='S0' and ( ";
                if (i == dtEDOrder.Rows.Count - 1)
                {
                    tmpsql += " ( kanbanorderno ='" + dtEDOrder.Rows[i]["vcOrderNo"].ToString() + "' and partsno like'" + parttmp + "%')) ";
                }
                else
                {
                    tmpsql += " ( kanbanorderno ='" + dtEDOrder.Rows[i]["vcOrderNo"].ToString() + "' and partsno like'" + parttmp + "%')) union all ";
                }
            }
            SqlDataAdapter oapt = new SqlDataAdapter(ocmd);
            oapt.Fill(ds.Tables[0]);
            if (dtEDOrder.Rows.Count == 0)
            {
                ds.Tables[1].Columns.Add("partsno");
                ds.Tables[1].Columns.Add("cpdcompany");
                ds.Tables[1].Columns.Add("dock");
                ds.Tables[1].Columns.Add("kanbanorderno");
                ds.Tables[1].Columns.Add("kanbanserial");
                ds.Tables[1].Columns.Add("num");
                ds.Tables[1].Columns.Add("OType");
            }
            else
            {
                ocmd.CommandText = tmpsql;
                oapt.Fill(ds.Tables[1]);
            }
            oapt.Dispose();
            ocmd.Dispose();
            return ds;
        }
        public DataTable getPlantSource()
        {
            string ssql = " select '' as value,'' as text union all select vcData1 as value,vcData2 as text from dbo.ConstMst  where vcDataId = 'KBPlant'";
            return excute.ExcuteSqlWithSelectToDT(ssql);
        }
        public DataTable getProtypeSource()
        {
            string ssql = " select '' as value union all select distinct vcData1 as value from dbo.ConstMst  where vcDataId = 'ProType'";
            return excute.ExcuteSqlWithSelectToDT(ssql);
        }

        public DataTable getEDOrder(string mon, SqlCommand cmd, SqlDataAdapter apt)
        {
            DataTable dt = new DataTable();
            string ssql = "select * from EDMonthPlanTMP where UpdateFlag ='1' and vcMonth ='" + mon + "' ";
            cmd.CommandText = ssql;
            cmd.CommandType = CommandType.Text;
            apt = new SqlDataAdapter(cmd);
            apt.Fill(dt);
            return dt;
        }

        public DataTable getMonPackPlanTMP(string mon, string tablename, SqlCommand cmd, SqlDataAdapter apt, string plant)//获取临时表的值别计划
        {
            DataTable dt = new DataTable();
            string tmpT = "";
            string tmpE = "";
            DateTime tim = Convert.ToDateTime(mon.Split('-')[0] + "-" + mon.Split('-')[1] + "-1");
            double daynum = (tim.AddMonths(1) - tim).TotalDays;

            for (int i = 1; i < 31 + 1; i++)
            {
                if (i == 31)
                    tmpE += "t2.vcD" + i + "b  as	ED" + i + "b,	t2.	vcD" + i + "y 	as	ED" + i + "y";
                else
                    tmpE += "t2.vcD" + i + "b 	as	ED" + i + "b,	t2.vcD" + i + "y 	as	ED" + i + "y,";
            }
            double daynum2 = (tim - tim.AddMonths(-1)).TotalDays;
            for (int i = 1; i < 31 + 1; i++)
            {
                if (i == 31)
                    tmpT += "t1.vcD" + i + "b  	as	TD" + i + "b,	t1.vcD" + i + "y 	as	TD" + i + "y";
                else
                    tmpT += "t1.vcD" + i + "b  	as	TD" + i + "b,	t1.vcD" + i + "y 	as	TD" + i + "y,";
            }
            string tmpT2 = "";
            string tmpE2 = "";
            for (int i = 1; i < 32; i++)
            {
                if (i == 31)
                {
                    tmpT2 += "TD" + i + "b,	TD" + i + "y";
                    tmpE2 += "ED" + i + "b,	ED" + i + "y";
                }
                else
                {
                    tmpT2 += "TD" + i + "b,	TD" + i + "y,";
                    tmpE2 += "ED" + i + "b,	ED" + i + "y,";
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("    select t2.vcMonth ,t2.vcPartsno,t2.vcDock,t2.vcCarType,t4.vcCalendar1,t4.vcCalendar2,t4.vcCalendar3,t4.vcCalendar4,");
            sb.AppendLine("   t3.vcPartNameCN as vcPartsNameCHN, t3.vcHJ as vcCurrentPastCode,t2.vcMonTotal as vcMonTotal ,t3.vcProType as bushu,'S' as EDflag ,");

            sb.AppendFormat(" {0},", tmpT);
            sb.AppendFormat(" {0}", tmpE);
            sb.AppendFormat("  from ( select  * from   {0} where montouch is not null) t1 ", tablename);
            sb.AppendFormat("  full join (select * from {0} where montouch is null) t2", tablename);
            sb.AppendLine("  on t1.montouch = t2.vcMonth and t1.vcPartsno=t2.vcPartsno and t1.vcDock=t2.vcDock and t1.vcCarType=t2.vcCarType");
            sb.AppendFormat("  left join (select distinct vcPartNameCN,vcHJ,vcProType,vcZB,vcPlant,vcQFflag,vcPartsNo,vcDock,vcCarType from  dbo.tPlanPartInfo where vcMonth ='{0}' and vcEDflag ='S' ) t3", mon);
            sb.AppendLine("  on t3.vcPartsNo=t2.vcPartsNo and t3.vcDock = t2.vcDock and t3.vcCarType = t2.vcCarType");
            sb.AppendLine("  left join dbo.ProRuleMst t4");
            sb.AppendLine("  on t4.vcPorType = t3.vcProType and t4.vcZB = t3.vcZB");
            if (plant.Trim().Length > 0)
            {
                sb.AppendFormat("  where t2.vcMonth ='{0}' and t3.vcQFflag='2' and t3.vcPlant ='{1}' ", mon, plant);
            }
            else
            {
                sb.AppendFormat("  where t2.vcMonth ='{0}' and t3.vcQFflag='2' ", mon);
            }
            sb.AppendLine(" union all");
            sb.AppendLine("      select t2.vcMonth ,t2.vcPartsno,t2.vcDock,t2.vcCarType,t4.vcCalendar1,t4.vcCalendar2,t4.vcCalendar3,t4.vcCalendar4");
            sb.AppendLine("  ,t3.vcPartNameCN as vcPartsNameCHN, t3.vcHJ as vcCurrentPastCode,t2.vcMonTotal ,t3.vcProType as bushu,'S' as EDflag ,");
            sb.AppendFormat(" {0},", tmpT);
            sb.AppendFormat(" {0}", tmpE);
            sb.AppendLine(" from ( select  * from   MonthPackPlanTbl where montouch is not null) t1   full join (select * from MonthPackPlanTbl where montouch is null) t2  on t1.montouch = t2.vcMonth and t1.vcPartsno=t2.vcPartsno and t1.vcDock=t2.vcDock and t1.vcCarType=t2.vcCarType ");
            sb.AppendFormat("  left join (select distinct vcPartNameCN,vcHJ,vcProType,vcZB,vcPlant,vcQFflag,vcPartsNo,vcDock,vcCarType from  dbo.tPlanPartInfo where vcMonth ='{0}' and vcEDflag ='S' ) t3", mon);
            sb.AppendLine("  on t3.vcPartsNo=t2.vcPartsNo and t3.vcDock = t2.vcDock and t3.vcCarType = t2.vcCarType");
            sb.AppendLine("  left join dbo.ProRuleMst t4");
            sb.AppendLine("  on t4.vcPorType = t3.vcProType and t4.vcZB = t3.vcZB");
            if (plant.Trim().Length > 0)
            {
                sb.AppendFormat("  where t2.vcMonth ='{0}' and t3.vcQFflag='1' and t3.vcPlant ='{1}' ", mon, plant);
            }
            else
            {
                sb.AppendFormat("  where t2.vcMonth ='{0}' and t3.vcQFflag='1' ", mon);
            }
            sb.AppendLine("union all");
            sb.AppendLine(" select tS.vcMonth,tQF.vcPartsNo,tQF.vcDock,tQF.vcCarFamilyCode as vcCarType,tS.vcCalendar1,tS.vcCalendar2, ");
            sb.AppendLine(" tS.vcCalendar3 ,tS.vcCalendar4,tS.vcPartsNameCHN,tS.vcCurrentPastCode,tS.vcMonTotal as vcMonTotal ,tS.bushu,tS.EDflag , ");

            sb.AppendFormat(" {0}, ", tmpT2);
            sb.AppendFormat(" {0} ", tmpE2);
            sb.AppendLine(" from ( ");
            sb.AppendLine("    select t2.vcMonth ,t2.vcPartsno,t2.vcDock,t2.vcCarType,t4.vcCalendar1,t4.vcCalendar2,t4.vcCalendar3,t4.vcCalendar4,t3.vcSRS as iQuantityPerContainer,");
            sb.AppendLine("   t3.vcPartNameCN as vcPartsNameCHN, t3.vcHJ as vcCurrentPastCode,t2.vcMonTotal ,t3.vcProType as bushu,'S' as EDflag ,");
            sb.AppendFormat(" {0},", tmpT);
            sb.AppendFormat(" {0}", tmpE);
            sb.AppendFormat("  from ( select  * from   {0} where montouch is not null) t1 ", tablename);
            sb.AppendFormat("  full join (select * from {0} where montouch is null) t2", tablename);
            sb.AppendLine("  on t1.montouch = t2.vcMonth and t1.vcPartsno=t2.vcPartsno and t1.vcDock=t2.vcDock and t1.vcCarType=t2.vcCarType");
            sb.AppendFormat("  left join (select distinct vcPartNameCN,vcHJ,vcProType,vcZB,vcPlant,vcQFflag,vcPartsNo,vcDock,vcCarType,vcSRS from  dbo.tPlanPartInfo where vcMonth ='{0}' and vcEDflag ='S' ) t3", mon);
            sb.AppendLine("  on t3.vcPartsNo=t2.vcPartsNo and t3.vcDock = t2.vcDock and t3.vcCarType = t2.vcCarType");
            sb.AppendLine("  left join dbo.ProRuleMst t4");
            sb.AppendLine("  on t4.vcPorType = t3.vcProType and t4.vcZB = t3.vcZB");
            if (plant.Trim().Length > 0)
            {
                sb.AppendFormat("  where t2.vcMonth ='{0}' and t3.vcQFflag ='1' and t3.vcPlant ='{1}' ", mon, plant);
            }
            else
            {
                sb.AppendFormat("  where t2.vcMonth ='{0}' and t3.vcQFflag ='1' ", mon);
            }
            sb.AppendLine(" ) tS ");
            sb.AppendLine(" left join (select * from dbo.tPartInfoMaster where dTimeFrom <='" + mon + "-01' and dTimeTo>= '" + mon + "-01' and vcInOutFlag ='1') tQF ");
            sb.AppendLine(" on SUBSTRING (tS.vcPartsno,0,10) = SUBSTRING(tQF.vcPartsNo,0,10) ");
            try
            {
                cmd.CommandText = sb.ToString();
                apt.Fill(dt);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable SearchItemData(string vcMon, string vcTF, string vcTO, string vcType, string vcPartsno, string vcDock, string vcSerial, string vcOrder, string vcProType, string plant, string ed)
        {
            //取出补给系统中的对象月的纳入明细
            DataTable dt = new DataTable();
            StringBuilder sb = new StringBuilder();

            //紧急订单
            string ssql = "select * from EDMonthPlanTMP where UpdateFlag ='1' "; //内制品
            if (vcMon.Length > 0)
                ssql += " and vcMonth ='" + vcMon + "'";
            DataTable dtEDorder = SearchData(ssql);
            string tmpsql = " select partsno,cpdcompany,dock,inno,quantity,kanbanorderno,kanbanserial,";
            tmpsql += " case when packingcondition='1' then '未包装' else '已包装' end as packingcondition,";
            tmpsql += " packingspot,convert(datetime,substring(left(scandatetimeht,8)+' ' + substring(scandatetimeht,9,2)+':' + substring(scandatetimeht,11,2)+':' + substring(scandatetimeht,13,2),1,17)) as scandatetimeht,htuser,htno,daddtime,cupduser,'T0' as otype,'紧急' as vcEDflag ";
            tmpsql += " from sp_m_opr t  where dataid = 'S0' ";
            tmpsql += " and (";
            for (int i = 0; i < dtEDorder.Rows.Count; i++)
            {
                if (i == dtEDorder.Rows.Count - 1)
                    tmpsql += " (kanbanorderno = '" + dtEDorder.Rows[i]["vcOrderNo"].ToString() + "' and partsno = '" + dtEDorder.Rows[i]["vcPartsno"].ToString() + "' and dock ='" + dtEDorder.Rows[i]["vcDock"].ToString() + "' )";
                else
                    tmpsql += " (kanbanorderno = '" + dtEDorder.Rows[i]["vcOrderNo"].ToString() + "' and partsno = '" + dtEDorder.Rows[i]["vcPartsno"].ToString() + "' and dock ='" + dtEDorder.Rows[i]["vcDock"].ToString() + "' ) or";
            }
            tmpsql += " ) ";
            if (vcPartsno.Length > 0)
            {
                tmpsql += " and  partsno like '%" + vcPartsno + "%'";
            }
            if (vcDock.Length > 0)
            {
                tmpsql += " and dock ='" + vcDock + "'";
            }
            if (vcTF.Length > 0)
            {
                tmpsql += " and daddtime>=CONVERT(datetime,'" + vcTF + "') ";
            }
            if (vcTO.Length > 0)
            {
                tmpsql += " and daddtime<=CONVERT(datetime,'" + vcTO + "') ";
            }
            if (vcOrder.Length > 0)
            {
                tmpsql += " and kanbanorderno like '%" + vcOrder + "%' ";
            }
            if (vcSerial.Length > 0)
            {
                tmpsql += " and kanbanserial like '%" + vcSerial + "%' ";
            }

            ssql = " select t2.vcPartsNo,t2.vcDock,t1.vcOrderNo from dbo.EDMonthPlanTMP t1 "; //外制品
            ssql += "  left join dbo.tPartInfoMaster t2  on SUBSTRING(t1.vcPartsno,0,11) = SUBSTRING(t2.vcPartsNo ,0,11)  and    t2.dTimeFrom <='" + vcMon + "-01" + "' and t2.dTimeTo>= '" + vcMon + "-01" + "'";
            ssql += " where t1.UpdateFlag ='1' and t2.vcInOutFlag ='1' ";
            if (vcMon.Length > 0)
                ssql += " and vcMonth ='" + vcMon + "'";
            DataTable dtEDorder2 = SearchData(ssql);
            string tmpsql2 = " select partsno,cpdcompany,dock,inno,quantity,kanbanorderno,kanbanserial,";
            tmpsql2 += " case when packingcondition='1' then '未包装' else '已包装' end as packingcondition,";
            tmpsql2 += " packingspot,convert(datetime,substring(left(scandatetimeht,8)+' ' + substring(scandatetimeht,9,2)+':' + substring(scandatetimeht,11,2)+':' + substring(scandatetimeht,13,2),1,17)) as scandatetimeht,htuser,htno,daddtime,cupduser,'T1' as otype ,'紧急' as vcEDflag ";
            tmpsql2 += " from sp_m_opr t  where dataid = 'S0' ";
            tmpsql2 += "  and ( ";
            for (int i = 0; i < dtEDorder2.Rows.Count; i++)
            {
                if (i == dtEDorder2.Rows.Count - 1)
                    tmpsql2 += " (kanbanorderno = '" + dtEDorder2.Rows[i]["vcOrderNo"].ToString() + "' and partsno = '" + dtEDorder2.Rows[i]["vcPartsno"].ToString() + "' and dock ='" + dtEDorder2.Rows[i]["vcDock"].ToString() + "' )";
                else
                    tmpsql2 += " (kanbanorderno = '" + dtEDorder2.Rows[i]["vcOrderNo"].ToString() + "' and partsno = '" + dtEDorder2.Rows[i]["vcPartsno"].ToString() + "' and dock ='" + dtEDorder2.Rows[i]["vcDock"].ToString() + "' ) or";
            }
            tmpsql2 += " ) ";
            if (vcPartsno.Length > 0)
            {
                tmpsql2 += " and  partsno like '%" + vcPartsno + "%'";
            }
            if (vcDock.Length > 0)
            {
                tmpsql2 += " and dock ='" + vcDock + "'";
            }
            if (vcTF.Length > 0)
            {
                tmpsql2 += " and daddtime>=CONVERT(datetime,'" + vcTF + "') ";
            }
            if (vcTO.Length > 0)
            {
                tmpsql2 += " and daddtime<=CONVERT(datetime,'" + vcTO + "') ";
            }
            if (vcOrder.Length > 0)
            {
                tmpsql2 += " and kanbanorderno like '%" + vcOrder + "%' ";
            }
            if (vcSerial.Length > 0)
            {
                tmpsql2 += " and kanbanserial like '%" + vcSerial + "%' ";
            }

            //通常订单
            sb.AppendLine("  select tall.*,t2.otype,'通常' as vcEDflag from (");
            sb.AppendLine("   select t1.partsno,t1.cpdcompany,t1.dock,t1.inno,t1.quantity,t1.kanbanorderno,t1.kanbanserial,");
            sb.AppendLine("   case when t1.packingcondition='1' then '未包装' else '已包装' end as packingcondition,");
            sb.AppendLine("   t1.packingspot,convert(datetime,substring(left(scandatetimeht,8)+' ' + substring(scandatetimeht,9,2)+':' + substring(scandatetimeht,11,2)+':' + substring(scandatetimeht,13,2),1,17)) as scandatetimeht,");
            sb.AppendLine("   t1.htuser,t1.htno,t1.daddtime,t1.cupduser from sp_m_opr t1 ");
            sb.AppendLine("   where t1.dataid ='S0'");
            if (vcMon.Length > 0)
            {
                DateTime monCur = Convert.ToDateTime(vcMon);
                string monLast = Convert.ToDateTime(vcMon).AddMonths(-1).ToString("yyyyMM");
                sb.AppendFormat("   and substring( t1.kanbanorderno,0,7)='{0}'", vcMon.Replace("-", ""));
            }
            if (vcPartsno.Length > 0)
            {
                sb.AppendFormat("   and t1.partsno like '%{0}%'", vcPartsno);
            }
            if (vcDock.Length > 0)
            {
                sb.AppendFormat("   and t1.dock ='{0}'", vcDock);
            }
            if (vcTF.Length > 0)
            {
                sb.AppendFormat("   and t1.daddtime>=CONVERT(datetime,'{0}')", vcTF);
            }
            if (vcTO.Length > 0)
            {
                sb.AppendFormat("   and t1.daddtime<=CONVERT(datetime,'{0}')", vcTO);
            }
            if (vcOrder.Length > 0)
            {
                sb.AppendFormat("   and t1.kanbanorderno like '%{0}%'", vcOrder.Trim());
            }
            if (vcSerial.Length > 0)
            {
                sb.AppendFormat("   and t1.kanbanserial like '%{0}%'", vcSerial.Trim());
            }
            sb.AppendLine("   union all ");
            sb.AppendLine("   select t1.partsno,t1.cpdcompany,t1.dock,t1.inno,t1.quantity,t1.kanbanorderno,t1.kanbanserial,");
            sb.AppendLine("   case when t1.packingcondition='1' then '未包装' else '已包装' end as packingcondition,");
            sb.AppendLine("   t1.packingspot,convert(datetime,substring(left(scandatetimeht,8)+' ' + substring(scandatetimeht,9,2)+':' + substring(scandatetimeht,11,2)+':' + substring(scandatetimeht,13,2),1,17)) as scandatetimeht,");
            sb.AppendLine("   t1.htuser,t1.htno,t1.daddtime,t1.cupduser from sp_m_opr t1 ");
            sb.AppendLine("   where t1.dataid ='S0'");
            if (vcMon.Length > 0)
            {
                DateTime monCur = Convert.ToDateTime(vcMon);
                string monLast = Convert.ToDateTime(vcMon).AddMonths(-1).ToString("yyyyMM");
                sb.AppendFormat("   and substring( t1.kanbanorderno,0,7)='{0}'", monLast);
            }
            if (vcPartsno.Length > 0)
            {
                sb.AppendFormat("   and t1.partsno like '%{0}%'", vcPartsno);
            }
            if (vcDock.Length > 0)
            {
                sb.AppendFormat("   and t1.dock ='{0}'", vcDock);
            }
            if (vcTF.Length > 0)
            {
                sb.AppendFormat("   and t1.daddtime>=CONVERT(datetime,'{0}')", vcTF);
            }
            if (vcTO.Length > 0)
            {
                sb.AppendFormat("   and t1.daddtime<=CONVERT(datetime,'{0}')", vcTO);
            }
            if (vcOrder.Length > 0)
            {
                sb.AppendFormat("   and t1.kanbanorderno like '%{0}%'", vcOrder.Trim());
            }
            if (vcSerial.Length > 0)
            {
                sb.AppendFormat("   and t1.kanbanserial like '%{0}%'", vcSerial.Trim());
            }
            sb.AppendLine("   )tall");
            sb.AppendLine("   left join (select partsno, inoutflag, dock,'T0' as otype from sp_m_sitem union all  select partsno,'0',dock,'T1' as otype from sp_m_edsitem ) t2");
            sb.AppendLine("   on tall.partsno = t2.partsno and tall.dock = t2.dock");
            sb.AppendLine("   where t2.inoutflag ='0'");
            if (dtEDorder.Rows.Count > 0)
            {
                sb.AppendLine(" union all ");
                sb.AppendFormat(" {0}", tmpsql);
            }
            if (dtEDorder2.Rows.Count > 0)
            {
                sb.AppendLine(" union all ");
                sb.AppendFormat(" {0}", tmpsql2);
            }
            DataSet dataSet = excute.ExcuteSqlWithSelectToDS(sb.ToString());
            dt = dataSet.Tables[0];

            //判断纳入明细是否在看板打印表中存在
            DataColumn col = new DataColumn();
            col.ColumnName = "chkFlag";
            col.DataType = typeof(string);
            col.DefaultValue = "0";
            dt.Columns.Add(col);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string kborder = dt.Rows[i]["kanbanorderno"].ToString().Trim();
                string kbserial = dt.Rows[i]["kanbanserial"].ToString().Trim();
                string partsno = dt.Rows[i]["partsno"].ToString().Trim();
                string dock = dt.Rows[i]["dock"].ToString().Trim();
                string otype = dt.Rows[i]["otype"].ToString().Trim();
                ssql = "";
                if (otype == "T1")
                {
                    ssql = "   select top(1) vcPlanMonth  from dbo.tKanbanPrintTbl t1";
                    ssql += "  left join (select *from  dbo.tPartInfoMaster where vcInOutFlag ='1' and dTimeFrom<='" + vcMon + "-01' and dTimeTo>='" + vcMon + "-01' ) t2";
                    ssql += "  on SUBSTRING(t1.vcPartsNo,0,11) = SUBSTRING(t2.vcPartsNo,0,11)";
                    ssql += "  where t2.vcPartsNo='" + partsno + "' and t2.vcDock ='" + dock + "' and  t1.vcKBorderno ='" + kborder + "' and t1.vcPlanMonth='" + vcMon + "' and t1.vcKBSerial='" + kbserial + "' ";
                    if (vcProType.Length > 0)
                    {
                        ssql += " and t2.vcPorType ='" + vcProType + "' ";
                    }
                    if (plant.Trim().Length > 0)
                    {
                        ssql += " and t2.vcPartPlant ='" + plant + "'";
                    }
                }
                else if (otype == "T0")
                {
                    ssql = " select top(1) vcPlanMonth from tKanbanPrintTbl t1 left join dbo.tPlanPartInfo t2 on t1.vcPartsNo = t2.vcPartsNo and t1.vcDock = t2.vcDock   where t1.vcKBorderno= '" + kborder + "' and t1.vcKBSerial='" + kbserial + "' and t1.vcPartsNo='" + partsno + "' and t1.vcDock ='" + dock + "' and t1.vcPlanMonth='" + vcMon + "' ";
                    if (vcProType.Length > 0)
                    {
                        ssql += " and t2.vcProType ='" + vcProType + "'";
                    }
                    if (plant.Trim().Length > 0)
                    {
                        ssql += " and t2.vcPlant ='" + plant + "'";
                    }
                }
                DataTable tmp = new DataTable();
                tmp = SearchData(ssql);
                if (tmp.Rows.Count > 0)
                {
                    dt.Rows[i]["chkFlag"] = "1";
                }
            }

            DataRow[] drs = dt.Select(" chkFlag='1' ");
            if (drs.Length > 0)
                dt = drs.CopyToDataTable();
            else dt = dt.Clone();

            if (ed.Trim().Length > 0)
            {
                dt = dt.Select("vcEDflag ='" + ed.Trim() + "'").Length > 0 ? dt.Select("vcEDflag ='" + ed.Trim() + "'").CopyToDataTable() : dt.Clone();
            }
            return dt;
        }

        public bool deletetKanbanPrintTbl(DataTable dt)
        {
            using (SqlConnection connection = new SqlConnection(ComConnectionHelper.GetConnectionString()))
            {
                connection.Open();
                SqlTransaction trans = connection.BeginTransaction();
                try
                {
                    string strSql = "";
                    string iNo;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        iNo = dt.Rows[i]["iNo"].ToString();
                        strSql = "delete from tKanbanPrintTbl where iNo='" + iNo + "'";
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = connection;
                        cmd.Transaction = trans;
                        cmd.CommandText = strSql;
                        cmd.ExecuteNonQuery();
                    }
                    trans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
            }
        }

        //（作废）SQL文添加string PlanPrintAB和string PlanPackAB两个参数 - 刘刚
        //（作废）SQL文添加string PlanProductionAB和string PlanProductionDate两个参数 - 20180911李兴旺
        //20181007将原先的计划打印日期首尾，改为计划生产日期首尾
        //20181011继续保留PlanPackAB和PlanProductionAB两个参数
        public DataTable getPartListCount(string mon, string partNo, string plant, string GC, string KbOrderId, string packdiv, string PlanProductionDateFrom, string PlanProductionBZFrom, string PlanPackDateFrom, string PlanPackBZFrom, string PlanProductionDateTo, string PlanProductionBZTo, string PlanPackDateTo, string PlanPackBZTo, string PlanProductionAB, string PlanPackAB)
        {
            try
            {
                DataTable tmp = new DataTable();
                StringBuilder sbSQL = new StringBuilder();

                sbSQL.AppendLine("SELECT count(*) count");
                sbSQL.AppendLine("  FROM [tKanbanPrintTbl] t1");
                sbSQL.AppendLine("  left join tPartInfoMaster t2  ");
                sbSQL.AppendLine("    on t1.[vcPartsNo] = t2.[vcPartsNo] and t1.[vcDock] = t2.[vcDock]");
                sbSQL.AppendLine("   and t2.[dTimeFrom] <= t1.[vcPlanMonth] + '-01' and t2.dTimeTo >=  t1.[vcPlanMonth] + '-01' ");//01改为-01
                sbSQL.AppendLine(" where 1=1 ");
                if (mon.Trim() != "")
                {
                    sbSQL.AppendLine(" and t1.vcPlanMonth = '" + mon + "' ");
                }
                if (partNo.Trim() != "")
                {
                    sbSQL.AppendLine(" and t1.vcPartsNo like '" + partNo + "%' ");
                }
                if (plant.Trim() != "")
                {
                    sbSQL.AppendLine(" and t2.vcPartPlant = '" + plant + "' ");
                }
                if (GC.Trim() != "")
                {
                    sbSQL.AppendLine(" and t2.vcPorType = '" + GC + "' ");
                }
                if (KbOrderId.Trim() != "")
                {
                    sbSQL.AppendLine(" and t1.[vcKBorderno] like '" + KbOrderId + "%'  ");
                }
                //if (KbSerial.Trim() != "")
                //{
                //    sbSQL.AppendLine(" and t1.[vcKBSerial] = '" + KbSerial + "' ");
                //}
                //if (PlanPrintDateFrom.Trim() != "")
                //{
                //    sbSQL.AppendLine(" and vcComDate00+vcBanZhi00 >= '" + PlanPrintDateFrom + PlanPrintBZFrom + "' ");
                //}
                //if (PlanPrintDateTo.Trim() != "")
                //{
                //    sbSQL.AppendLine(" and vcComDate00+vcBanZhi00 <= '" + PlanPrintDateTo + PlanPrintBZTo + "' ");
                //}
                if (PlanProductionDateFrom.Trim() != "")
                {
                    sbSQL.AppendLine(" and vcComDate01+vcBanZhi01 >= '" + PlanProductionDateFrom + PlanProductionBZFrom + "' ");//20181007 李兴旺
                }
                if (PlanProductionDateTo.Trim() != "")
                {
                    sbSQL.AppendLine(" and vcComDate01+vcBanZhi01 <= '" + PlanProductionDateTo + PlanProductionBZTo + "' ");//20181007 李兴旺
                }
                //if (PlanPackDateFrom.Trim() != "")
                //{
                //    sbSQL.AppendLine(" and vcComDate04+vcBanZhi04 >= '" + PlanPackDateFrom + PlanPackBZFrom + "' ");
                //}
                //if (PlanPackDateTo.Trim() != "")
                //{
                //    sbSQL.AppendLine(" and vcComDate04+vcBanZhi04 <= '" + PlanPackDateTo + PlanPackBZTo + "' ");
                //}
                if (PlanPackDateFrom.Trim() != "")
                {
                    sbSQL.AppendLine(" and vcComDate04+vcBanZhi04 >= '" + PlanPackDateFrom + PlanPackBZFrom + "' ");//20181007 李兴旺
                }
                if (PlanPackDateTo.Trim() != "")
                {
                    sbSQL.AppendLine(" and vcComDate04+vcBanZhi04 <= '" + PlanPackDateTo + PlanPackBZTo + "' ");//20181007 李兴旺
                }
                //（作废）增加计划打印班直 - 刘刚                
                //if (PlanPrintAB != "")//计划打印班直（AB）
                //{
                //    sbSQL.AppendLine(" and vcAB00 = '" + PlanPrintAB + "' ");
                //}

                //（作废）20180911增加计划生产日期 - 李兴旺                
                //if (PlanProductionDate.Trim() != "")//计划生产日期
                //{
                //    sbSQL.AppendLine(" and vcComDate01 = '" + PlanProductionDate + "' ");
                //}
                //20180911增加计划生产班直 - 李兴旺
                if (PlanProductionAB != "")//计划生产班直（AB）
                {
                    sbSQL.AppendLine(" and vcAB01 = '" + PlanProductionAB + "' ");
                }
                //增加计划包装班直 - 刘刚
                if (PlanPackAB != "")//计划包装班直（AB）
                {
                    sbSQL.AppendLine(" and vcAB04 = '" + PlanPackAB + "' ");
                }
                tmp = SearchData(sbSQL.ToString());
                if (tmp.Rows.Count > 0)
                {
                    string count = tmp.Rows[0]["count"].ToString();
                    if (Convert.ToInt32(count) > 60000)
                    {
                        throw new Exception("检索数据过多，请缩小查询范围。");
                    }
                }
                sbSQL = new StringBuilder();
                //（作废）tKanbanPrintTbl增加了vcAB00、vcAB04列，对应参数vcPlanPrintAB、vcPlanProcAB - 刘刚
                //tKanbanPrintTbl增加了vcAB01列，对应参数vcPlanProductionAB(Fron/To) - 20181007 李兴旺
                sbSQL.AppendLine("SELECT t1.[vcPlanMonth] as vcMonth, ");//对象月
                sbSQL.AppendLine("       t2.[vcPorType] as vcGC, ");//部署
                sbSQL.AppendLine("       t2.[vcPartPlant] as vcPlant, ");//工厂
                sbSQL.AppendLine("       t2.[iQuantityPerContainer] as vcQuantityPerContainer, ");//收容数
                sbSQL.AppendLine("       t1.[iNo] as iNo, ");//ino
                sbSQL.AppendLine("       t1.[vcPartsNo] as vcPartNo, ");//品番
                sbSQL.AppendLine("       t1.[vcDock] as vcDock, ");//受入（有检索但没显示）
                sbSQL.AppendLine("       t1.[vcKBorderno] as vcOrderNo, ");//订单号
                sbSQL.AppendLine("       t1.[vcKBSerial] as vcSerial, ");//连番
                sbSQL.AppendLine("       t1.[vcComDate00] as vcPlanPrintDate, ");//计划打印日期
                //sbSQL.AppendLine("       t1.[vcBanZhi00] as vcPlanPrintBZ, ");//打印班值
                sbSQL.AppendLine("       t1.[vcAB00] as vcPlanPrintAB, ");//打印班值(A/B) - 刘刚
                sbSQL.AppendLine("       t1.[vcPrintTime] as vcRealPrintTime, ");//实际打印时间
                sbSQL.AppendLine("       t1.[vcComDate01] as vcPlanProductionDate, ");//计划生产日期 - 李兴旺
                sbSQL.AppendLine("       t1.[vcAB01] as vcPlanProductionAB, ");//生产班值(A/B) - 李兴旺
                sbSQL.AppendLine("       t1.[vcComDate04] as vcPlanProcDate, ");//计划包装日期
                //sbSQL.AppendLine("       t1.[vcBanZhi04] as vcPlanProcBZ, ");//包装班值
                sbSQL.AppendLine("       t1.[vcAB04] as vcPlanProcAB, ");//包装班值(A/B) - 刘刚
                sbSQL.AppendLine("       '' as vcRealProcTime ");//实际包装时间
                sbSQL.AppendLine("  FROM [tKanbanPrintTbl] t1 ");
                sbSQL.AppendLine("  left join tPartInfoMaster t2  ");
                sbSQL.AppendLine("    on t1.[vcPartsNo] = t2.[vcPartsNo] and t1.[vcDock] = t2.[vcDock] ");
                sbSQL.AppendLine("   and t2.[dTimeFrom] <= t1.[vcPlanMonth] + '-01' and t2.dTimeTo >=  t1.[vcPlanMonth] + '-01' ");//01改为-01
                sbSQL.AppendLine(" where 1=1  ");
                if (mon.Trim() != "")
                {
                    sbSQL.AppendLine(" and t1.vcPlanMonth = '" + mon + "' ");
                }
                if (partNo.Trim() != "")
                {
                    sbSQL.AppendLine(" and t1.vcPartsNo like '" + partNo + "%' ");
                }
                if (plant.Trim() != "")
                {
                    sbSQL.AppendLine(" and t2.vcPartPlant = '" + plant + "' ");
                }
                if (GC.Trim() != "")
                {
                    sbSQL.AppendLine(" and t2.vcPorType = '" + GC + "' ");
                }
                if (KbOrderId.Trim() != "")
                {
                    sbSQL.AppendLine(" and t1.[vcKBorderno] like '" + KbOrderId + "%' ");
                }
                //if (KbSerial.Trim() != "")
                //{
                //    sbSQL.AppendLine(" and t1.[vcKBSerial] = '" + KbSerial + "' ");
                //}
                //if (PlanPrintDateFrom.Trim() != "")
                //{
                //    sbSQL.AppendLine(" and vcComDate00+vcBanZhi00 >= '" + PlanPrintDateFrom + PlanPrintBZFrom + "' ");
                //}
                //if (PlanPrintDateTo.Trim() != "")
                //{
                //    sbSQL.AppendLine(" and vcComDate00+vcBanZhi00 <= '" + PlanPrintDateTo + PlanPrintBZTo + "' ");
                //}
                if (PlanProductionDateFrom.Trim() != "")
                {
                    sbSQL.AppendLine(" and vcComDate01+vcBanZhi01 >= '" + PlanProductionDateFrom + PlanProductionBZFrom + "' ");//20181007 李兴旺
                }
                if (PlanProductionDateTo.Trim() != "")
                {
                    sbSQL.AppendLine(" and vcComDate01+vcBanZhi01 <= '" + PlanProductionDateTo + PlanProductionBZTo + "' ");//20181007 李兴旺
                }
                //if (PlanPackDateFrom.Trim() != "")
                //{
                //    sbSQL.AppendLine(" and vcComDate04+vcBanZhi04 >= '" + PlanPackDateFrom + PlanPackBZFrom + "' ");
                //}
                //if (PlanPackDateTo.Trim() != "")
                //{
                //    sbSQL.AppendLine(" and vcComDate04+vcBanZhi04 <= '" + PlanPackDateTo + PlanPackBZTo + "' ");
                //}
                if (PlanPackDateFrom.Trim() != "")
                {
                    sbSQL.AppendLine(" and vcComDate04+vcBanZhi04 >= '" + PlanPackDateFrom + PlanPackBZFrom + "' ");//20181007 李兴旺
                }
                if (PlanPackDateTo.Trim() != "")
                {
                    sbSQL.AppendLine(" and vcComDate04+vcBanZhi04 <= '" + PlanPackDateTo + PlanPackBZTo + "' ");//20181007 李兴旺
                }
                //（作废）增加计划打印班直 - 刘刚
                //if (PlanPrintAB != "")//计划打印班直
                //{
                //    sbSQL.AppendLine(" and vcAB00 = '" + PlanPrintAB + "' ");
                //}                
                //（作废）20180911增加计划生产日期 - 李兴旺                
                //if (PlanProductionDate.Trim() != "")//计划生产日期
                //{
                //    sbSQL.AppendLine(" and vcComDate01 = '" + PlanProductionDate + "' ");
                //}
                //20180911增加计划生产班直 - 李兴旺
                if (PlanProductionAB != "")//计划生产班直
                {
                    sbSQL.AppendLine(" and vcAB01 = '" + PlanProductionAB + "' ");
                }
                //增加计划包装班直 - 刘刚
                if (PlanPackAB != "")//计划包装班直
                {
                    sbSQL.AppendLine(" and vcAB04 = '" + PlanPackAB + "' ");
                }
                sbSQL.AppendLine(" order by vcMonth, vcPlant, vcPartNo ");
                tmp = SearchData(sbSQL.ToString());

                sbSQL = new StringBuilder();
                DataTable tmp2 = new DataTable();
                sbSQL.AppendLine(" select distinct t.PARTSNO, t.DOCK, t.KANBANORDERNO, t.KANBANSERIAL, t.DADDTIME ");
                sbSQL.AppendLine("   from sp_m_opr t ");
                sbSQL.AppendLine("   left join SP_M_SITEM t2 on t.partsno = t2.partsno and t.dock = t2.dock and t.cpdcompany = t2.cpdcompany ");
                sbSQL.AppendLine("  where 1=1 and dataid = 'S0' ");
                if (mon.Trim() != "")
                {
                    string monfrom = Convert.ToDateTime(mon + "-01").AddMonths(-1).ToString("yyyyMM");
                    string monTo = Convert.ToDateTime(mon + "-01").ToString("yyyyMM");
                    sbSQL.AppendLine(" and substring(KANBANORDERNO, 0, 6) >= '" + monfrom + "' and substring(KANBANORDERNO, 0, 6) <= '" + monTo + "' ");
                }
                if (partNo.Trim() != "")
                {
                    sbSQL.AppendLine(" and t.PARTSNO like '" + partNo + "%' ");
                }
                if (plant.Trim() != "")
                {
                    if (plant == "1")
                    {
                        sbSQL.AppendLine(" and t2.plantcode = '0' ");
                    }
                    else if (plant == "2")
                    {
                        sbSQL.AppendLine(" and t2.plantcode = '4' ");
                    }
                    else if (plant == "3")
                    {
                        sbSQL.AppendLine(" and t2.plantcode = '8' ");
                    }
                }
                if (KbOrderId.Trim() != "")
                {
                    sbSQL.AppendLine(" and t.KANBANORDERNO like '" + KbOrderId + "%' ");
                }
                DataSet dataSet = excute.ExcuteSqlWithSelectToDS(sbSQL.ToString());
                tmp2 = dataSet.Tables[0];
                for (int i = 0; i < tmp.Rows.Count; i++)
                {
                    //if (tmp.Rows[i]["vcPlanPrintBZ"].ToString() == "0")
                    //{
                    //    tmp.Rows[i]["vcPlanPrintBZ"] = "白值";
                    //}
                    //else if (tmp.Rows[i]["vcPlanPrintBZ"].ToString() == "1")
                    //{
                    //    tmp.Rows[i]["vcPlanPrintBZ"] = "夜值";
                    //}

                    //if (tmp.Rows[i]["vcPlanProcBZ"].ToString() == "0")
                    //{
                    //    tmp.Rows[i]["vcPlanProcBZ"] = "白值";
                    //}
                    //else if (tmp.Rows[i]["vcPlanProcBZ"].ToString() == "1")
                    //{
                    //    tmp.Rows[i]["vcPlanProcBZ"] = "夜值";
                    //}
                    //20180911修改tmp表中AB值的显示 - 李兴旺
                    if (tmp.Rows[i]["vcPlanPrintAB"].ToString().Trim() == "A")
                    {
                        tmp.Rows[i]["vcPlanPrintAB"] = "A值";
                    }
                    else if (tmp.Rows[i]["vcPlanPrintAB"].ToString().Trim() == "B")
                    {
                        tmp.Rows[i]["vcPlanPrintAB"] = "B值";
                    }
                    if (tmp.Rows[i]["vcPlanProductionAB"].ToString().Trim() == "A")
                    {
                        tmp.Rows[i]["vcPlanProductionAB"] = "A值";
                    }
                    else if (tmp.Rows[i]["vcPlanProductionAB"].ToString().Trim() == "B")
                    {
                        tmp.Rows[i]["vcPlanProductionAB"] = "B值";
                    }
                    if (tmp.Rows[i]["vcPlanProcAB"].ToString().Trim() == "A")
                    {
                        tmp.Rows[i]["vcPlanProcAB"] = "A值";
                    }
                    else if (tmp.Rows[i]["vcPlanProcAB"].ToString().Trim() == "B")
                    {
                        tmp.Rows[i]["vcPlanProcAB"] = "B值";
                    }
                    if (packdiv == "1") //已纳
                    {
                        DataRow[] arrayDR = tmp2.Select(" PARTSNO = '" + tmp.Rows[i]["vcPartNo"] + "' and DOCK = '" + tmp.Rows[i]["vcDock"] + "' and KANBANORDERNO = '" + tmp.Rows[i]["vcOrderNo"] + "' and KANBANSERIAL = '" + tmp.Rows[i]["vcSerial"] + "'  ");
                        if (arrayDR.Length >= 1)
                        {
                            tmp.Rows[i]["vcRealProcTime"] = arrayDR[0]["DADDTIME"].ToString();
                        }
                        else
                        {
                            //tmp.Rows.Remove(tmp.Rows[i]);
                            tmp.Rows[i].Delete();
                        }
                    }
                    else if (packdiv == "2")  // 未纳
                    {
                        DataRow[] arrayDR = tmp2.Select(" PARTSNO = '" + tmp.Rows[i]["vcPartNo"] + "' and DOCK = '" + tmp.Rows[i]["vcDock"] + "' and KANBANORDERNO = '" + tmp.Rows[i]["vcOrderNo"] + "' and KANBANSERIAL = '" + tmp.Rows[i]["vcSerial"] + "'  ");
                        if (arrayDR.Length >= 1)
                        {
                            tmp.Rows[i].Delete();
                            //tmp.Rows.Remove(tmp.Rows[i]);
                        }
                        else
                        {
                            //tmp.Rows.Remove(tmp.Rows[i]);
                        }
                    }
                    else
                    {
                        DataRow[] arrayDR = tmp2.Select(" PARTSNO = '" + tmp.Rows[i]["vcPartNo"] + "' and DOCK = '" + tmp.Rows[i]["vcDock"] + "' and KANBANORDERNO = '" + tmp.Rows[i]["vcOrderNo"] + "' and KANBANSERIAL = '" + tmp.Rows[i]["vcSerial"] + "'  ");
                        if (arrayDR.Length >= 1)
                        {
                            tmp.Rows[i]["vcRealProcTime"] = arrayDR[0]["DADDTIME"].ToString();
                        }
                    }
                }
                tmp.AcceptChanges();
                return tmp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable getMonPlanED(string mon, string TblName, SqlCommand cmd, SqlDataAdapter apt, string plant)
        {
            DataTable dt = new DataTable();
            string tmpT = "";
            string tmpE = "";
            DateTime tim = Convert.ToDateTime(mon.Split('-')[0] + "-" + mon.Split('-')[1] + "-1");
            double daynum = (tim.AddMonths(1) - tim).TotalDays;

            for (int i = 1; i < 31 + 1; i++)
            {
                if (i == 31)
                    tmpE += "t2.vcD" + i + "b  as	ED" + i + "b,	t2.	vcD" + i + "y 	as	ED" + i + "y";
                else
                    tmpE += "t2.vcD" + i + "b 	as	ED" + i + "b,	t2.vcD" + i + "y  	as	ED" + i + "y,";
            }
            double daynum2 = (tim - tim.AddMonths(-1)).TotalDays;
            for (int i = 1; i < 31 + 1; i++)
            {
                if (i == 31)
                    tmpT += "t1.vcD" + i + "b 	as	TD" + i + "b,	t1.vcD" + i + "y 	as	TD" + i + "y";
                else
                    tmpT += "t1.vcD" + i + "b 	as	TD" + i + "b,	t1.vcD" + i + "y 	as	TD" + i + "y,";
            }
            string tmpT2 = "";
            string tmpE2 = "";
            for (int i = 1; i < 32; i++)
            {
                if (i == 31)
                {
                    tmpT2 += "TD" + i + "b,	TD" + i + "y";
                    tmpE2 += "ED" + i + "b,	ED" + i + "y";
                }
                else
                {
                    tmpT2 += "TD" + i + "b,	TD" + i + "y,";
                    tmpE2 += "ED" + i + "b,	ED" + i + "y,";
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" select '{0}' as vcMonth , case when t2.vcPartsno is null then t1.vcPartsno else t2.vcPartsno end as vcPartsno", mon);
            sb.AppendLine(" ,case when t2.vcDock is null then t1.vcDock else t2.vcDock end as vcDock");
            sb.AppendLine(" ,case when t2.vcCarType is null then t1.vcCarType else t2.vcCarType end as vcCarType");
            sb.AppendLine("  ,t4.vcCalendar1,t4.vcCalendar2,t4.vcCalendar3,t4.vcCalendar4,");
            sb.AppendLine(" ");
            sb.AppendLine("  t3.vcPartNameCN as vcPartsNameCHN, t3.vcHJ as vcCurrentPastCode ,'0' as vcMonTotal,t3.vcProType as bushu,'E' as EDflag,");
            sb.AppendFormat(" {0},", tmpT);
            sb.AppendFormat(" {0}", tmpE);
            sb.AppendFormat("   from (select * from  {0} where montouch is not null ) t1  ", TblName);
            sb.AppendFormat("    full join (select * from  {0} where montouch is null ) t2", TblName);
            sb.AppendLine("     on t1.montouch = t2.vcMonth and t1.vcPartsno=t2.vcPartsno and t1.vcDock=t2.vcDock and t1.vcCarType=t2.vcCarType");
            sb.AppendFormat("   left join (select  distinct vcPartNameCN,vcHJ,vcProType,vcZB,vcPartsNo,vcDock,vcCarType,vcQFflag,vcEDFlag,vcPlant from  dbo.tPlanPartInfo where vcMonth ='{0}' and vcEDflag ='E' ) t3", mon);
            sb.AppendLine("   on (t3.vcPartsNo=t2.vcPartsNo or t3.vcPartsNo = t1.vcPartsno)and (t3.vcDock = t2.vcDock or t3.vcDock = t1.vcDock) and (t3.vcCarType = t2.vcCarType or t3.vcCarType = t1.vcCarType)");
            sb.AppendLine("   left join dbo.ProRuleMst t4");
            sb.AppendLine(" on t4.vcPorType = t3.vcProType and t4.vcZB = t3.vcZB");
            if (plant.Trim().Length == 0)
            {
                sb.AppendFormat("where ((t1.montouch = '{0}' or (t2.vcMonth ='{1}' and t1.montouch is null)) and t3.vcQFflag ='2' and t3.vcEDFlag ='E') ", mon, mon);
            }
            else
            {
                sb.AppendFormat(" where ((t1.montouch = '{0}' or (t2.vcMonth ='{1}' and t1.montouch is null)) and t3.vcQFflag ='2' and t3.vcPlant ='{2}' and t3.vcEDFlag ='E') ", mon, mon, plant);
            }

            sb.AppendLine(" union all ");
            sb.AppendFormat(" select '{0}' as vcMonth , case when t2.vcPartsno is null then t1.vcPartsno else t2.vcPartsno end as vcPartsno", mon);
            sb.AppendLine(" ,case when t2.vcDock is null then t1.vcDock else t2.vcDock end as vcDock");
            sb.AppendLine(" ,case when t2.vcCarType is null then t1.vcCarType else t2.vcCarType end as vcCarType");
            sb.AppendLine("  ,t4.vcCalendar1,t4.vcCalendar2,t4.vcCalendar3,t4.vcCalendar4,");
            sb.AppendLine(" ");
            sb.AppendLine("  t3.vcPartNameCN AS vcPartsNameCHN, t3.vcHJ AS vcCurrentPastCode ,'0' as vcMonTotal,t3.vcProType as bushu,'E' as EDflag,");
            sb.AppendFormat(" {0},", tmpT);
            sb.AppendFormat(" {0}", tmpE);
            sb.AppendFormat("   from (select * from  {0} where montouch is not null ) t1  ", TblName);
            sb.AppendFormat("    full join (select * from  {0} where montouch is null ) t2", TblName);
            sb.AppendLine("     on t1.montouch = t2.vcMonth and t1.vcPartsno=t2.vcPartsno and t1.vcDock=t2.vcDock and t1.vcCarType=t2.vcCarType");
            sb.AppendFormat("   left join (select distinct vcPartNameCN,vcHJ,vcProType,vcZB,vcPartsNo,vcDock,vcCarType,vcQFflag,vcEDFlag,vcPlant from  dbo.tPlanPartInfo where vcMonth ='{0}' and vcEDflag ='E' )t3", mon);
            sb.AppendLine("   on (t3.vcPartsNo=t2.vcPartsNo or t3.vcPartsNo = t1.vcPartsno)and (t3.vcDock = t2.vcDock or t3.vcDock = t1.vcDock) and (t3.vcCarType = t2.vcCarType or t3.vcCarType = t1.vcCarType)");
            sb.AppendLine("   left join dbo.ProRuleMst t4");
            sb.AppendLine(" on t4.vcPorType = t3.vcProType and t4.vcZB = t3.vcZB");
            if (plant.Trim().Length == 0)
            {
                sb.AppendFormat(" where ((t1.montouch = '{0}' or (t2.vcMonth ='{1}' and t1.montouch is null)) and t3.vcQFflag ='1')", mon, mon);
            }
            else
            {
                sb.AppendFormat(" where ((t1.montouch = '{0}' or (t2.vcMonth ='{1}' and t1.montouch is null)) and t3.vcQFflag ='1' and t3.vcPlant ='{2}')", mon, mon, plant);
            }
            sb.AppendLine("  ");
            sb.AppendLine(" union all ");

            sb.AppendLine(" select tS.vcMonth , tQF.vcPartsNo,tQF.vcDock,tQF.vcCarFamilyCode as vcCarType,tS.vcCalendar1,tS.vcCalendar2,tS.vcCalendar3,  ");
            sb.AppendLine("  tS.vcCalendar4,tS.vcPartsNameCHN ,tS.vcCurrentPastCode,tS.vcMonTotal ,tS.bushu,tS.EDflag, ");
            sb.AppendFormat(" {0},", tmpT2);
            sb.AppendFormat(" {0}", tmpE2);
            sb.AppendLine("   from ( ");
            sb.AppendFormat(" select '{0}' as vcMonth , case when t2.vcPartsno is null then t1.vcPartsno else t2.vcPartsno end as vcPartsno", mon);
            sb.AppendLine(" ,case when t2.vcDock is null then t1.vcDock else t2.vcDock end as vcDock");
            sb.AppendLine(" ,case when t2.vcCarType is null then t1.vcCarType else t2.vcCarType end as vcCarType");
            sb.AppendLine("  ,t4.vcCalendar1,t4.vcCalendar2,t4.vcCalendar3,t4.vcCalendar4,");
            sb.AppendLine(" ");
            sb.AppendLine("  t3.vcPartNameCN as vcPartsNameCHN, t3.vcHJ as vcCurrentPastCode ,'0' as vcMonTotal,t3.vcProType as bushu,'E' as EDflag,");
            sb.AppendFormat(" {0},", tmpT);
            sb.AppendFormat(" {0}", tmpE);
            sb.AppendFormat("   from (select * from  {0} where montouch is not null ) t1  ", TblName);
            sb.AppendFormat("    full join (select * from  {0} where montouch is null ) t2", TblName);
            sb.AppendLine("     on t1.montouch = t2.vcMonth and t1.vcPartsno=t2.vcPartsno and t1.vcDock=t2.vcDock and t1.vcCarType=t2.vcCarType");
            sb.AppendFormat("   left join (select distinct vcPartNameCN,vcHJ,vcProType,vcZB,vcPartsNo,vcDock,vcCarType,vcQFflag,vcEDFlag,vcPlant from  dbo.tPlanPartInfo where  vcMonth ='{0}' and vcEDflag ='E' ) t3", mon);
            sb.AppendLine("   on (t3.vcPartsNo=t2.vcPartsNo or t3.vcPartsNo = t1.vcPartsno)and (t3.vcDock = t2.vcDock or t3.vcDock = t1.vcDock) and (t3.vcCarType = t2.vcCarType or t3.vcCarType = t1.vcCarType)");
            sb.AppendLine("   left join dbo.ProRuleMst t4");
            sb.AppendLine(" on t4.vcPorType = t3.vcProType and t4.vcZB = t3.vcZB");
            if (plant.Trim().Length == 0)
            {
                sb.AppendFormat(" where ((t1.montouch = '{0}' or (t2.vcMonth ='{1}' and t1.montouch is null)) and t3.vcQFflag ='1')", mon, mon);
            }
            else
            {
                sb.AppendFormat(" where ((t1.montouch = '{0}' or (t2.vcMonth ='{1}' and t1.montouch is null)) and t3.vcQFflag ='1' and t3.vcPlant ='{2}')", mon, mon, plant);
            }
            sb.AppendLine("  ) tS ");
            sb.AppendFormat("  left join  (select * from dbo.tPartInfoMaster where dTimeFrom <='{0}' and dTimeTo>= '{1}' and vcInOutFlag ='1') tQF  on SUBSTRING (tS.vcPartsno,0,10) = SUBSTRING(tQF.vcPartsNo,0,10) ", mon + "-01", mon + "-01");

            try
            {
                cmd.CommandText = sb.ToString();
                apt.Fill(dt);
            }
            catch
            {

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
    }
}
