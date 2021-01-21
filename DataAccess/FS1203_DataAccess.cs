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

        public DataTable bindplant()
        {
            string ssql = " select '' as vcName,'' as vcValue union all select distinct vcData2,vcData1 from ConstMst where vcDataID='KBPlant' ";
            return excute.ExcuteSqlWithSelectToDT(ssql);
        }
        public DataTable getPlantype()
        {
            string ssql = " select '' as vcName ,'' as vcValue union all select planType,value from sPlanType where enable='1' order by vcValue";
            return excute.ExcuteSqlWithSelectToDT(ssql);
        }



        public DataTable existDT(string vcMon, string vcPartsNo, string vcCarType, string vcPlant)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(" select * from dbo.tSOQREPExport where vcMonth='" + vcMon + "' and PartsNo='" + vcPartsNo + "'");
            sql.AppendLine(" and CFC='" + vcCarType + "' and updateFlag='0' and vcPlant not in " + StrToList(vcPlant));
            return excute.ExcuteSqlWithSelectToDT(sql.ToString());
        }

        private string StrToList(string aa)
        {
            string instr = "(";
            if (!string.IsNullOrEmpty(aa.Trim()))
            {
                string[] bb = aa.Split(new string[] { "\r\n", ",", ";", "* " }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < bb.Length; i++)
                {
                    if (!instr.Contains(bb[i]))//去掉重复的輸入值
                    {
                        instr += "'" + bb[i] + "',";
                    }
                }
            }
            instr = instr.Substring(0, instr.LastIndexOf(",")) + ")";
            return instr;
        }

        public string checkPlanExist(string vcMon, string vcPlant)
        {
            try
            {
                StringBuilder ssql = new StringBuilder();
                ssql.AppendLine("select * from MonthPackPlanTbl where (montouch='" + vcMon + "' or");
                ssql.AppendLine(" (vcMonth='" + vcMon + "' and montouch is null)) and");
                ssql.AppendLine(" exists (select distinct vcPartsNo,vcDock ,vcCarType from tPlanPartInfo");
                ssql.AppendLine(" where tPlanPartInfo.vcMonth='" + vcMon + "' and tPlanPartInfo.vcPlant in" + StrToList(vcPlant));
                ssql.AppendLine(" and tPlanPartInfo.vcPartsNo=MonthPackPlanTbl.vcPartsno");
                ssql.AppendLine(" and tPlanPartInfo.vcDock=MonthPackPlanTbl.vcDock");
                ssql.AppendLine(" and tPlanPartInfo.vcCarType=MonthPackPlanTbl.vcCarType)");
                DataTable dt = excute.ExcuteSqlWithSelectToDT(ssql.ToString());
                if (dt.Rows.Count > 0)
                {
                    return "已经存在该月计划！";
                }
                else return "";
            }
            catch
            {
                throw;
            }
        }

        public string swithMon(int Mon)
        {
            string re = "";
            switch (Mon)
            {
                case 1:
                    re = "dbo.tJanuary";
                    break;
                case 2:
                    re = "dbo.tFebruary";
                    break;
                case 3:
                    re = "dbo.tMarch";
                    break;
                case 4:
                    re = "dbo.tApril";
                    break;
                case 5:
                    re = "dbo.tMay";
                    break;
                case 6:
                    re = "dbo.tJune";
                    break;
                case 7:
                    re = "dbo.tJuly";
                    break;
                case 8:
                    re = "dbo.tAugust";
                    break;
                case 9:
                    re = "dbo.tSeptember";
                    break;
                case 10:
                    re = "dbo.tOctober";
                    break;
                case 11:
                    re = "dbo.tNovember";
                    break;
                case 12:
                    re = "dbo.tDecember";
                    break;
            }
            return re;
        }

        public DataTable getPartsInfo(string vcMon)
        {
            vcMon += "-01";
            StringBuilder ssql = new StringBuilder();
            ssql.AppendLine(" select t1.* ,t2.vcProName1, t2.vcProName2, t2.vcProName3, t2.vcProName4 from (");
            ssql.AppendLine(" select vcPartsNo, vcDock, iQuantityPerContainer, vcQJcontainer, vcPorType, vcZB,");
            ssql.AppendLine("vcCarFamilyCode, vcPartType, vcPartPlant, vcInOutFlag");
            ssql.AppendLine(" from dbo.tPartInfoMaster where dTimeFrom<='" + vcMon + "' and dTimeTo>='" + vcMon + "') t1 ");
            ssql.AppendLine(" left join dbo.ProRuleMst t2 on t1.vcPorType=t2.vcPorType and t1.vcZB=t2.vcZB");
            try
            {
                return excute.ExcuteSqlWithSelectToDT(ssql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string updateProPlan(List<DataTable> dt0, List<DataTable> dt1, List<DataTable> dt2, List<DataTable> dt3, List<DataTable> dt4, DataTable partsInfo, string user, string lbltime, string plant)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(ComConnectionHelper.GetConnectionString());
            string msg = "";
            try
            {
                cmd.Connection.Open();
                cmd.Transaction = cmd.Connection.BeginTransaction();
                cmd.CommandTimeout = 0;
                TransactionPlan(dt0, cmd, partsInfo, "MonthKanBanPlanTblTMP", user, lbltime, plant, ref msg);
                if (msg.Length <= 0)
                {
                    TransactionPlan(dt1, cmd, partsInfo, "MonthProPlanTblTMP", user, lbltime, plant, ref msg);
                }
                if (msg.Length <= 0)
                {
                    TransactionPlan(dt2, cmd, partsInfo, "MonthTZPlanTblTMP", user, lbltime, plant, ref msg);
                }
                if (msg.Length <= 0)
                {
                    TransactionPlan(dt3, cmd, partsInfo, "MonthP3PlanTblTMP", user, lbltime, plant, ref msg);
                }
                //if (msg.Length <= 0)
                //{
                //    TransactionPlan(dt4, cmd, partsInfo, "MonthPackPlanTblTMP", user, lbltime, plant, ref msg);
                //}
                if (msg.Length <= 0)
                {
                    cmd.Transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                cmd.Transaction.Rollback();
                msg = "更新失败。" + ex.ToString();
                return msg;
            }
            finally
            {
                cmd.Connection.Close();
            }
            return msg;
        }

        public void TransactionPlan(List<DataTable> dt, SqlCommand cmd, DataTable partsInfo, string TableName, string user, string lbltime, string plant, ref string msg)
        {
            msg = "";
            try
            {
                DataTable dt2 = new DataTable();//20180929实测没用，是为了把变量apt引出 - 李兴旺
                cmd.CommandText = "select TOP(1) * from " + TableName;//20180929实测没用，是为了把变量apt引出 - 李兴旺
                SqlDataAdapter apt = new SqlDataAdapter(cmd);//20180929实测没用，是为了把变量apt引出 - 李兴旺
                apt.Fill(dt2);//20180929实测没用，是为了把变量apt引出 - 李兴旺
                cmd.CommandText = "delete from " + TableName + " where (vcMonth='" + lbltime + "' or montouch ='" + lbltime + "') and exists (select vcPartsNo from dbo.tPartInfoMaster where vcPartPlant in" + StrToList(plant) + " and vcPartsNo = " + TableName + ".vcPartsno   and dTimeFrom<= '" + lbltime + "-01" + "' and dTimeTo >= '" + lbltime + "-01" + "' ) ";
                cmd.ExecuteNonQuery();
                for (int i = 0; i < partsInfo.Rows.Count; i++)
                {
                    string vcPartsno = partsInfo.Rows[i]["partsno"].ToString();
                    string vcDock = partsInfo.Rows[i]["vcDock"].ToString();
                    string vcCarType = partsInfo.Rows[i]["vcCarFamilyCode"].ToString();
                    string vcProjectName = partsInfo.Rows[i]["vcProName1"].ToString();
                    string vcProject1 = partsInfo.Rows[i]["vcCalendar1"].ToString();
                    //string total = (Convert.ToInt32(partsInfo.Rows[i]["num"])/(Convert.ToInt32(partsInfo.Rows[i]["srs"]))).ToString();
                    string total = partsInfo.Rows[i]["num"].ToString();
                    //20180929在从SOQReply数据生成包装计划时就已经把5个工程的计划都生成一遍了，所以要在这里，生成看板打印数据时，把周度品番筛走 - 李兴旺
                    //20180929查看该品番的品番频度 - 李兴旺
                    string vcPartFrequence = "";
                    string sqlPartFrequence = "SELECT vcPartsNo, vcPartFrequence FROM SPPSBS.dbo.tPartInfoMaster where vcPartsNo = '" + vcPartsno + "' and vcDock = '" + vcDock + "' and vcCarFamilyCode = '" + vcCarType + "' and dTimeFrom<='" + lbltime + "-01' and dTimeTo>='" + lbltime + "-01' ";
                    cmd.CommandText = sqlPartFrequence;
                    DataTable dtPartFrequence = new DataTable();
                    apt.Fill(dtPartFrequence);
                    if (dtPartFrequence.Rows.Count <= 0)
                    {
                        msg = "品番：" + vcPartsno + " 在当前对象月范围内没有品番基础信息！";
                        break;
                    }
                    vcPartFrequence = dtPartFrequence.Rows[0]["vcPartFrequence"].ToString().Trim();
                    //20180929不是看板打印计划表，则不区分品番频度；是看板打印计划表，则区分品番频度，不是周度的则更新。
                    //即TableName == "MonthKanBanPlanTblTMP" && vcPartFrequence == "周度"时，不进行更新操作 - 李兴旺
                    if (!(TableName == "MonthKanBanPlanTblTMP" && vcPartFrequence == "周度"))
                    {
                        #region 插入、更新数据操作
                        foreach (DataRow dr in dt[i].Rows)
                        {
                            string tmpY = dr["vcYear"].ToString();
                            string tmpM = dr["vcMonth"].ToString().Length == 1 ? "0" + dr["vcMonth"].ToString() : dr["vcMonth"].ToString();
                            string vcMonth = tmpY + "-" + tmpM;
                            cmd.CommandText = "select top(1) * from " + TableName + " where vcMonth='" + vcMonth + "' and vcPartsno='" + vcPartsno + "' and vcDock ='" + vcDock + "' ";
                            DataTable tmp = new DataTable();
                            apt.Fill(tmp);
                            if (tmp.Rows.Count > 0)
                            {
                                string upsql = "vc" + dr["days"].ToString() + "='" + dr["total"].ToString() + "'";
                                if (lbltime != vcMonth)
                                {
                                    cmd.CommandText = "update " + TableName + " set " + upsql + " , DUPDTIME=getdate(),CUPDUSER='" + user + "' ,montouch ='" + lbltime + "' where vcMonth='" + vcMonth + "' and vcPartsno='" + vcPartsno + "' and vcDock ='" + vcDock + "' ";
                                }
                                else
                                {
                                    cmd.CommandText = "update " + TableName + " set " + upsql + " , DUPDTIME=getdate(),CUPDUSER='" + user + "'  where vcMonth='" + vcMonth + "' and vcPartsno='" + vcPartsno + "' and vcDock ='" + vcDock + "' ";
                                }
                                cmd.ExecuteNonQuery();
                            }
                            else
                            {
                                cmd.CommandText = " INSERT INTO " + TableName + " ([vcMonth],[vcPartsno],[vcDock],[vcCarType],[vcProject1],[vcProjectName] ,[DADDTIME],[CUPDUSER])";
                                cmd.CommandText += " values( '" + vcMonth + "' ,'" + vcPartsno + "','" + vcDock + "','" + vcCarType + "','" + vcProject1 + "','" + vcProjectName + "',getdate(),'" + user + "')  ";
                                cmd.ExecuteNonQuery();
                                string upsql = "vc" + dr["days"].ToString() + "='" + dr["total"].ToString() + "'";
                                if (lbltime != vcMonth)
                                {
                                    cmd.CommandText = "update " + TableName + " set " + upsql + " , DUPDTIME=getdate(),CUPDUSER='" + user + "' ,montouch ='" + lbltime + "'  where vcMonth='" + vcMonth + "' and vcPartsno='" + vcPartsno + "' and vcDock ='" + vcDock + "' ";
                                }
                                else
                                {
                                    cmd.CommandText = "update " + TableName + " set " + upsql + " , DUPDTIME=getdate(),CUPDUSER='" + user + "' where vcMonth='" + vcMonth + "' and vcPartsno='" + vcPartsno + "' and vcDock ='" + vcDock + "' ";
                                }
                                cmd.ExecuteNonQuery();
                            }
                        }
                        cmd.CommandText = "update " + TableName + " set vcMonTotal='" + total + "'  where vcMonth='" + lbltime + "' and vcPartsno='" + vcPartsno + "' and vcDock ='" + vcDock + "' ";
                        cmd.ExecuteNonQuery();
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }
        }

        public string getDiysql(DateTime tim)
        {
            DataTable dt = new DataTable();
            string sqltmp = "";
            double daynum = (tim.AddMonths(1) - tim).TotalDays;
            for (double i = 1; i < daynum + 1; i++)
            {
                if (i == daynum)
                    sqltmp += "D" + i + "b,D" + i + "y";
                else sqltmp += "D" + i + "b,D" + i + "y,";

            }
            return sqltmp;
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
            sb.AppendLine("  left join (select distinct vcMonth,vcPartNameCN,vcZB,vcHJ,vcDock,vcCarType,vcPartsNo,vcProType,vcPlant,vcEDFlag from tPlanPartInfo) t3");
            sb.AppendLine("  on t3.vcPartsNo=t2.vcPartsNo and t3.vcDock = t2.vcDock and t3.vcCarType = t2.vcCarType and  t3.vcMonth = '" + mon + "' ");
            sb.AppendLine("  left join ProRuleMst t4");
            sb.AppendLine("  on t4.vcPorType = t3.vcProType and t4.vcZB = t3.vcZB");
            sb.AppendLine(" left join (select vcData1 ,vcData2 from ConstMst where vcDataId ='kbplant') t5");
            sb.AppendLine(" on t3.vcPlant = t5.vcData1 ");
            sb.AppendFormat("  where t2.vcMonth ='{0}' and t3.vcPlant ='{1}' and t3.vcEDFlag ='S' ", mon, plant);
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

        #region 20180912检索中增加品番频度列 - 李兴旺
        public DataTable getSOQEXData(string mon, string plant)
        {
            string ssql = "";//20180912检索中增加品番频度列 - 李兴旺
            ssql += "  SELECT ";
            ssql += "         a.[PartsNo] ";
            ssql += "        ,a.[CFC] ";
            ssql += "        ,a.[OrdLot] ";
            ssql += "        ,a.[NUnits] ";
            ssql += "        ,a.[NPCS] ";
            ssql += "        ,a.[D1] ";
            ssql += "        ,a.[D2] ";
            ssql += "        ,a.[D3] ";
            ssql += "        ,a.[D4] ";
            ssql += "        ,a.[D5] ";
            ssql += "        ,a.[D6] ";
            ssql += "        ,a.[D7] ";
            ssql += "        ,a.[D8] ";
            ssql += "        ,a.[D9] ";
            ssql += "        ,a.[D10] ";
            ssql += "        ,a.[D11] ";
            ssql += "        ,a.[D12] ";
            ssql += "        ,a.[D13] ";
            ssql += "        ,a.[D14] ";
            ssql += "        ,a.[D15] ";
            ssql += "        ,a.[D16] ";
            ssql += "        ,a.[D17] ";
            ssql += "        ,a.[D18] ";
            ssql += "        ,a.[D19] ";
            ssql += "        ,a.[D20] ";
            ssql += "        ,a.[D21] ";
            ssql += "        ,a.[D22] ";
            ssql += "        ,a.[D23] ";
            ssql += "        ,a.[D24] ";
            ssql += "        ,a.[D25] ";
            ssql += "        ,a.[D26] ";
            ssql += "        ,a.[D27] ";
            ssql += "        ,a.[D28] ";
            ssql += "        ,a.[D29] ";
            ssql += "        ,a.[D30] ";
            ssql += "        ,a.[D31] ";
            ssql += "        ,a.[N+1 O/L] ";
            ssql += "        ,a.[N+1 Units] ";
            ssql += "        ,a.[N+1 PCS] ";
            ssql += "        ,a.[N+2 O/L] ";
            ssql += "        ,a.[N+2 Units] ";
            ssql += "        ,a.[N+2 PCS] ";
            ssql += "        ,a.[orderid] ";//订单号 排序用
            ssql += "        ,b.[vcPartFrequence] ";//品番频度
            ssql += "    FROM tSOQREPExport a ";
            ssql += "    LEFT JOIN (SELECT distinct vcPartsNo, vcPartFrequence FROM tPartInfoMaster where dTimeFrom<='" + mon + "-01' and dTimeTo>='" + mon + "-01' ) b ";//品番频度 - 修改where条件
            ssql += "      ON a.PartsNo = b.vcPartsNo ";//品番频度
            ssql += "   WHERE vcMonth='" + mon + "' and updateFlag='1' and vcPlant in" + StrToList(plant) + " order by orderid ";
            try
            {
                return excute.ExcuteSqlWithSelectToDT(ssql);
            }
            catch
            {
                throw;
            }
        }
        #endregion

        public string updateSOQREP(DataTable dt, string mon, string user, string tmpmsg, string plant, string deletefrom)
        {
            string msg = "";
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(ComConnectionHelper.GetConnectionString());
            string plants = StrToList(plant);
            try
            {
                cmd.Connection = conn;
                cmd.CommandTimeout = 0;
                conn.Open();
                cmd.Transaction = conn.BeginTransaction();
                if (tmpmsg.Length > 0)
                {
                    //替换SQL
                    string ssql = " ";
                    ssql += " delete from MonthPackPlanTbl where (montouch = '" + mon + "' or (vcMonth = '" + mon + "' and  montouch is null)) and exists ( select vcPartsNo,vcDock ,vcCarType from tPlanPartInfo where tPlanPartInfo.vcMonth = '" + mon + "' and tPlanPartInfo.vcPlant in" + plants + " and tPlanPartInfo.vcPartsNo = MonthPackPlanTbl.vcPartsno and tPlanPartInfo.vcDock = MonthPackPlanTbl.vcDock and tPlanPartInfo.vcCarType = MonthPackPlanTbl.vcCarType  ) ;";
                    ssql += " delete from MonthKanBanPlanTbl where (montouch = '" + mon + "' or (vcMonth = '" + mon + "' and  montouch is null)) and exists ( select vcPartsNo,vcDock ,vcCarType from tPlanPartInfo where tPlanPartInfo.vcMonth = '" + mon + "' and tPlanPartInfo.vcPlant in" + plants + " and tPlanPartInfo.vcPartsNo = MonthKanBanPlanTbl.vcPartsno and tPlanPartInfo.vcDock = MonthKanBanPlanTbl.vcDock and tPlanPartInfo.vcCarType = MonthKanBanPlanTbl.vcCarType  ) ;";
                    ssql += " delete from MonthP3PlanTbl where (montouch = '" + mon + "' or (vcMonth = '" + mon + "' and  montouch is null)) and exists ( select vcPartsNo,vcDock ,vcCarType from tPlanPartInfo where tPlanPartInfo.vcMonth = '" + mon + "' and tPlanPartInfo.vcPlant in" + plants + " and tPlanPartInfo.vcPartsNo = MonthP3PlanTbl.vcPartsno and tPlanPartInfo.vcDock = MonthP3PlanTbl.vcDock and tPlanPartInfo.vcCarType = MonthP3PlanTbl.vcCarType  ) ;";
                    ssql += " delete from MonthProdPlanTbl where (montouch = '" + mon + "' or (vcMonth = '" + mon + "' and  montouch is null)) and exists ( select vcPartsNo,vcDock ,vcCarType from tPlanPartInfo where tPlanPartInfo.vcMonth = '" + mon + "' and tPlanPartInfo.vcPlant in" + plants + " and tPlanPartInfo.vcPartsNo = MonthProdPlanTbl.vcPartsno and tPlanPartInfo.vcDock = MonthProdPlanTbl.vcDock and tPlanPartInfo.vcCarType = MonthProdPlanTbl.vcCarType  ) ;";
                    ssql += " delete from MonthTZPlanTbl where (montouch = '" + mon + "' or (vcMonth = '" + mon + "' and  montouch is null)) and exists ( select vcPartsNo,vcDock ,vcCarType from tPlanPartInfo where tPlanPartInfo.vcMonth = '" + mon + "' and tPlanPartInfo.vcPlant in" + plants + " and tPlanPartInfo.vcPartsNo = MonthTZPlanTbl.vcPartsno and tPlanPartInfo.vcDock = MonthTZPlanTbl.vcDock and tPlanPartInfo.vcCarType = MonthTZPlanTbl.vcCarType  ) ;";

                    ssql += " delete from MonthKanBanPlanTblTMP where (montouch = '" + mon + "' or (vcMonth = '" + mon + "' and  montouch is null)) and exists ( select vcPartsNo,vcDock ,vcCarType from tPlanPartInfo where tPlanPartInfo.vcMonth = '" + mon + "' and tPlanPartInfo.vcPlant in" + plants + " and tPlanPartInfo.vcPartsNo = MonthKanBanPlanTblTMP.vcPartsno and tPlanPartInfo.vcDock = MonthKanBanPlanTblTMP.vcDock and tPlanPartInfo.vcCarType = MonthKanBanPlanTblTMP.vcCarType  ) ;";
                    ssql += " delete from MonthP3PlanTblTMP where (montouch = '" + mon + "' or (vcMonth = '" + mon + "' and  montouch is null)) and exists ( select vcPartsNo,vcDock ,vcCarType from tPlanPartInfo where tPlanPartInfo.vcMonth = '" + mon + "' and tPlanPartInfo.vcPlant in" + plants + " and tPlanPartInfo.vcPartsNo = MonthP3PlanTblTMP.vcPartsno and tPlanPartInfo.vcDock = MonthP3PlanTblTMP.vcDock and tPlanPartInfo.vcCarType = MonthP3PlanTblTMP.vcCarType  ) ;";
                    ssql += " delete from MonthPackPlanTblTMP where (montouch = '" + mon + "' or (vcMonth = '" + mon + "' and  montouch is null)) and exists ( select vcPartsNo,vcDock ,vcCarType from tPlanPartInfo where tPlanPartInfo.vcMonth = '" + mon + "' and tPlanPartInfo.vcPlant in" + plants + " and tPlanPartInfo.vcPartsNo = MonthPackPlanTblTMP.vcPartsno and tPlanPartInfo.vcDock = MonthPackPlanTblTMP.vcDock and tPlanPartInfo.vcCarType = MonthPackPlanTblTMP.vcCarType  ) ;";
                    ssql += " delete from MonthProPlanTblTMP where (montouch = '" + mon + "' or (vcMonth = '" + mon + "' and  montouch is null)) and exists ( select vcPartsNo,vcDock ,vcCarType from tPlanPartInfo where tPlanPartInfo.vcMonth = '" + mon + "' and tPlanPartInfo.vcPlant in" + plants + " and tPlanPartInfo.vcPartsNo = MonthProPlanTblTMP.vcPartsno and tPlanPartInfo.vcDock = MonthProPlanTblTMP.vcDock and tPlanPartInfo.vcCarType = MonthProPlanTblTMP.vcCarType  ) ;";
                    ssql += " delete from MonthTZPlanTblTMP where (montouch = '" + mon + "' or (vcMonth = '" + mon + "' and  montouch is null)) and exists ( select vcPartsNo,vcDock ,vcCarType from tPlanPartInfo where tPlanPartInfo.vcMonth = '" + mon + "' and tPlanPartInfo.vcPlant in" + plants + " and tPlanPartInfo.vcPartsNo = MonthTZPlanTblTMP.vcPartsno and tPlanPartInfo.vcDock = MonthTZPlanTblTMP.vcDock and tPlanPartInfo.vcCarType = MonthTZPlanTblTMP.vcCarType  ) ;";
                    //改造2013-6-8 结束 
                    if (deletefrom.Length > 0)
                    {
                        ssql += " delete from tKanbanPrintTbl  where vcPlanMonth = '" + mon + "' and vcEDflag ='S' and vcComDate01 >'" + deletefrom + "' and exists(select vcPartsNo from tPlanPartInfo where vcPlant in" + plants + " and vcEDflag ='S' and vcPartsNo = tKanbanPrintTbl.vcPartsno and vcMonth = '" + mon + "')   ;";
                    }
                    else
                    {
                        ssql += " delete from tKanbanPrintTbl  where vcPlanMonth = '" + mon + "' and vcEDflag ='S' and exists(select vcPartsNo from dbo.tPlanPartInfo where vcPlant in" + plants + " and vcEDflag ='S' and vcPartsNo = tKanbanPrintTbl.vcPartsno and vcMonth = '" + mon + "')   ;";
                    }
                    ssql += " delete from tSOQREPExport where updateFlag ='1' and vcMonth ='" + mon + "' and vcPlant in" + plants + "  ";

                    cmd.CommandText = ssql;
                    cmd.ExecuteNonQuery();
                }
                //改造2013-6-8 开始
                cmd.CommandText = " delete from tPlanPartInfo where vcMonth ='" + mon + "' and vcPlant in" + plants + " and vcEDFlag ='S' ";
                cmd.ExecuteNonQuery();
                //改造2013-6-8 结束  
                cmd.CommandText = "delete from tSOQREPExport where vcMonth ='" + mon + "' and vcPlant in" + plants + " and updateFlag ='0' ";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "select * from tSOQREPExport";
                SqlDataAdapter apt = new SqlDataAdapter(cmd);
                DataTable dtupdate = new DataTable();
                apt.Fill(dtupdate);

                //dtupdate.Rows.Add();
                for (int k = 0; k < dt.Rows.Count; k++)
                {
                    if (dt.Rows[k][0].ToString().Trim().Length <= 0)
                        break;
                    foreach (string curPlant in plant.Split(','))
                    {
                        DataRow dr = dtupdate.NewRow();
                        dr["vcMonth"] = mon;
                        dr["vcPlant"] = curPlant;
                        dr["orderid"] = k;
                        dr["vcCreateUserId"] = user;
                        dr["dCreateTime"] = DateTime.Now;
                        dr["updateFlag"] = "0";
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            dr[i + 2] = dt.Rows[k][i];
                        }
                        dtupdate.Rows.Add(dr);
                    }
                }
                SqlCommandBuilder cmdbd = new SqlCommandBuilder(apt);
                apt.Update(dtupdate);
                cmd.Transaction.Commit();
                return msg;
            }
            catch (Exception ex)
            {
                cmd.Transaction.Rollback();
                conn.Close();
                msg = "error";
                throw ex;
            }
        }

        public DataTable getSoqInfo(string mon, string plant)
        {
            DataTable dt = new DataTable();
            string ssql = "  ";
            ssql += "  select t1.vcPlant,t1.PartsNo ,t2.vcDock,t2.vcCarFamilyCode ,t2.vcQJcontainer as kbsrs,t2.iQuantityPerContainer as srs , ";
            ssql += "  t1.NUnits*t2.iQuantityPerContainer as num ,t2.vcPorType,t2.vcZB,t3.KBpartType,t2.vcQFflag, ";
            ssql += "  t3.vcProName0,t3.vcLT0,	t3.vcCalendar0, ";
            ssql += "  t3.vcProName1,t3.vcLT1,	t3.vcCalendar1, ";
            ssql += "  t3.vcProName2,t3.vcLT2,	t3.vcCalendar2, ";
            ssql += "  t3.vcProName3,t3.vcLT3,	t3.vcCalendar3, ";
            ssql += "  t3.vcProName4,t3.vcLT4,	t3.vcCalendar4  ";
            ssql += "   from dbo.tSOQREPExport t1 ";
            ssql += "  left join dbo.tPartInfoMaster t2  ";
            ssql += "  on t1.PartsNo = t2.vcPartsNo and t1.CFC = t2.vcCarFamilyCode   ";
            ssql += "  left join dbo.ProRuleMst t3 ";
            ssql += "  on t3.vcPorType=t2.vcPorType and t3.vcZB = t2.vcZB ";
            ssql += "  where t1.vcMonth='" + mon + "' and updateFlag ='0' and NPCS <>'0' and vcPlant in" + StrToList(plant) + " and  t2.dTimeFrom<= '" + mon + "-01" + "' and t2.dTimeTo >= '" + mon + "-01" + "'";
            ssql += "  order by vcPlant, vcPorType ,vcZB ,KBpartType  ";
            try
            {
                return excute.ExcuteSqlWithSelectToDT(ssql);
            }
            catch
            {
                throw;
            }
        }

        public DataTable getCalendarSP(int year, int mon, string gc, string zb, int Lt, string plant)//获取 根据Lt计算出的日历 非指定-#2
        {
            DataTable dt = new DataTable();
            DateTime curMon = Convert.ToDateTime(year.ToString() + "-" + mon.ToString() + "-1");
            int year_last = curMon.AddMonths(-1).Year;
            int year_month = curMon.AddMonths(-1).Month;
            string dboMon_current = swithMon(curMon.Month);
            string dboMon_last = swithMon(curMon.AddMonths(-1).Month);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("    declare @Lt int");
            sb.AppendLine("    declare @tmpa int ");
            sb.AppendLine("    declare @tmpb int ");
            sb.AppendLine("    DECLARE @zhinum int");
            sb.AppendLine("     DECLARE @banyue int   ");
            sb.AppendFormat("     set @Lt = {0}", Lt);
            sb.AppendLine("     SET @zhinum= (");
            sb.AppendLine("     --算出该月多少值");
            sb.AppendLine("    select top(1) row_number() over ( order by vcYear,vcMonth) as zhi from (");
            sb.AppendFormat("  select vcYear,vcMonth, vcGC,vcZB ,dayall,days  from {0} ", dboMon_current);
            sb.AppendFormat(" unpivot (dayall for days in({0}", getDiysql(curMon));
            sb.AppendLine(" )) P");

            sb.AppendFormat("    where vcGC= '{0}' and vcZB= '{1}' and vcYear ='{2}' and vcPlant='{3}' and dayall='M'  ) t  ", gc, zb, year, plant);
            sb.AppendLine("    order by zhi desc");
            sb.AppendLine("      )");
            sb.AppendLine("      set @tmpa = @zhinum");
            sb.AppendLine("      SET @tmpb= (");
            sb.AppendLine("     --算出该月多少值");
            sb.AppendLine("    select top(1) row_number() over ( order by vcYear,vcMonth) as zhi from (");
            sb.AppendFormat("  select vcYear,vcMonth, vcGC,vcZB ,dayall,days  from {0} ", dboMon_last);
            sb.AppendFormat(" unpivot (dayall for days in({0}", getDiysql(curMon.AddMonths(-1)));
            sb.AppendLine(" )) P");
            sb.AppendFormat("    where vcGC= '{0}' and vcZB= '{1}' and vcYear ='{2}' and vcPlant='{3}' and dayall='M'  ", gc, zb, year, plant);
            sb.AppendLine("    union all");
            sb.AppendFormat("  select vcYear,vcMonth, vcGC,vcZB ,dayall,days  from {0} ", dboMon_current);
            sb.AppendFormat(" unpivot (dayall for days in({0}", getDiysql(curMon));
            sb.AppendLine(" )) P");
            sb.AppendFormat("    where vcGC= '{0}' and vcZB= '{1}' and vcYear ='{2}' and vcPlant='{3}' and dayall='M'  ) t  ", gc, zb, year, plant);

            sb.AppendLine("    order by zhi desc");
            sb.AppendLine("      )");
            sb.AppendLine("      if @zhinum%2 = 1");
            sb.AppendLine("     	 begin");
            sb.AppendLine("     	 set  @banyue =(@zhinum+1)/2");
            sb.AppendLine("     	 end");
            sb.AppendLine("      else set  @banyue =@zhinum/2");
            sb.AppendLine("    ");
            sb.AppendLine("      select tt.* ,((tt.zhi-1)/10+1) as zhou , 0 as total from ");
            sb.AppendLine("    (");
            sb.AppendLine("    select tall.vcYear,tall.vcMonth,tall.vcGC,tall.vcZB,tall.dayall,tall.days ,row_number() over ( order by vcYear,vcMonth) as zhi from (");
            sb.AppendLine("    select t.*,row_number() over ( order by vcYear,vcMonth) as zhitmp from (");
            sb.AppendFormat("  select vcYear,vcMonth, vcGC,vcZB ,dayall,days  from {0} ", dboMon_last);
            sb.AppendFormat(" unpivot (dayall for days in({0}", getDiysql(curMon.AddMonths(-1)));
            sb.AppendLine(" )) P");
            sb.AppendFormat("    where vcGC= '{0}' and vcZB= '{1}' and vcYear ='{2}' and vcPlant='{3}' and dayall='M'", gc, zb, year, plant);
            sb.AppendLine("    union all");

            sb.AppendFormat("  select vcYear,vcMonth, vcGC,vcZB ,dayall,days  from {0} ", dboMon_current);
            sb.AppendFormat(" unpivot (dayall for days in({0}", getDiysql(curMon));
            sb.AppendLine(" )) P");
            sb.AppendFormat("    where vcGC= '{0}' and vcZB= '{1}' and vcYear ='{2}' and vcPlant='{3}' and dayall='M'  ) t", gc, zb, year, plant);
            sb.AppendLine("    ) tall");
            sb.AppendLine("    where tall.zhitmp >@tmpb-@tmpa-@Lt  and tall.zhitmp<=@tmpb-@Lt");
            sb.AppendLine("    ) tt");
            sb.AppendLine("    union all");
            sb.AppendLine("      select '0' ,'0','0' ,'0','0','0','9999', @banyue as banyue , @zhinum as totalzhi");
            try
            {
                return excute.ExcuteSqlWithSelectToDT(sb.ToString());
            }
            catch
            {
                throw;
            }
        }

        public DataTable getCalendar(int year, int mon, string gc, string zb, string lastflag, string curflag, string a, string plant)
        {
            DataTable dt = new DataTable();
            DateTime curMon = Convert.ToDateTime(year.ToString() + "-" + mon.ToString() + "-1");
            int year_last = curMon.AddMonths(-1).Year;
            int year_month = curMon.AddMonths(-1).Month;
            string dboMon_current = swithMon(curMon.Month);
            string dboMon_last = swithMon(curMon.AddMonths(-1).Month);
            StringBuilder sb = new StringBuilder();
            if (a.Length == 0)
            {
                sb.AppendLine(" select t.*,row_number() over ( order by vcYear,vcMonth) as zhi from (");
                sb.AppendFormat("  select vcYear,vcMonth, vcGC,vcZB ,dayall,days  from {0} ", dboMon_last);
                sb.AppendFormat(" unpivot (dayall for days in({0}", getDiysql(curMon.AddMonths(-1)));
                sb.AppendLine(" )) P");
                sb.AppendFormat("  where vcGC= '{0}' and vcZB= '{1}' and vcYear ='{2}' and vcPlant='{3}'  and dayall IN ('{4}','{4}A','{4}B')", gc, zb, curMon.AddMonths(-1).Year, plant, lastflag);//2018-2-26 Malcolm.L 刘刚
                sb.AppendLine("  ");
                sb.AppendLine("  union all");
                sb.AppendFormat("  select vcYear,vcMonth, vcGC,vcZB ,dayall,days  from {0} ", dboMon_current);
                sb.AppendFormat(" unpivot (dayall for days in({0}", getDiysql(curMon));
                sb.AppendLine(" )) P");
                sb.AppendFormat("  where vcGC= '{0}' and vcZB= '{1}' and vcYear ='{2}' and vcPlant='{3}'  and dayall IN ('{4}','{4}A','{4}B')", gc, zb, curMon.Year, plant, curflag);//2018-2-26 Malcolm.L 刘刚
                sb.AppendLine(" ) t");
            }
            if (a == "1")
            {
                sb.AppendLine("  DECLARE @zhinum int");
                sb.AppendLine("   DECLARE @banyue int   ");
                sb.AppendLine("   SET @zhinum= (");
                sb.AppendLine("   --算出该月多少值");
                sb.AppendLine("  select top(1) row_number() over ( order by vcYear,vcMonth) as zhi from (");
                sb.AppendFormat("  select vcYear,vcMonth, vcGC,vcZB ,dayall,days from {0} ", dboMon_last);
                sb.AppendFormat(" unpivot (dayall for days in({0}", getDiysql(curMon.AddMonths(-1)));
                sb.AppendLine(" )) P");
                sb.AppendFormat("  where vcGC= '{0}' and vcZB= '{1}' and vcYear ='{2}' and vcPlant='{3}' and dayall IN ('{4}','{4}A','{4}B')", gc, zb, curMon.AddMonths(-1).Year, plant, lastflag);//2018-2-26 Malcolm.L 刘刚
                sb.AppendLine("  ");
                sb.AppendLine("  union all");
                sb.AppendFormat("  select vcYear,vcMonth, vcGC,vcZB ,dayall,days   from {0} ", dboMon_current);
                sb.AppendFormat(" unpivot (dayall for days in({0}", getDiysql(curMon));
                sb.AppendLine(" )) P");
                sb.AppendFormat("  where vcGC= '{0}' and vcZB= '{1}' and vcYear ='{2}' and vcPlant='{3}' and dayall IN ('{4}','{4}A','{4}B')", gc, zb, curMon.Year, plant, curflag);//2018-2-26 Malcolm.L 刘刚
                sb.AppendLine("  ) t  ");
                sb.AppendLine("  order by zhi desc");
                sb.AppendLine("    )");
                sb.AppendLine("    ");
                sb.AppendLine("    if @zhinum%2 = 1");
                sb.AppendLine("   	 begin");
                sb.AppendLine("   	 set  @banyue =(@zhinum+1)/2");
                sb.AppendLine("   	 end");
                sb.AppendLine("    else set  @banyue =@zhinum/2");
                sb.AppendLine("  select tt.* ,((tt.zhi-1)/10+1) as zhou , 0 as total from ");
                sb.AppendLine("  (");
                sb.AppendLine("    select t.*,row_number() over ( order by vcYear,vcMonth) as zhi from (");
                sb.AppendFormat("  select vcYear,vcMonth, vcGC,vcZB ,dayall,days  from {0} ", dboMon_last);
                sb.AppendFormat(" unpivot (dayall for days in({0}", getDiysql(curMon.AddMonths(-1)));
                sb.AppendLine(" )) P");
                sb.AppendFormat("  where vcGC= '{0}' and vcZB= '{1}' and vcYear ='{2}' and vcPlant='{3}' and dayall IN ('{4}','{4}A','{4}B')", gc, zb, curMon.AddMonths(-1).Year, plant, lastflag);//2018-2-26 Malcolm.L 刘刚
                sb.AppendLine("  ");
                sb.AppendLine("  union all");
                sb.AppendFormat("  select vcYear,vcMonth, vcGC,vcZB ,dayall,days  from {0} ", dboMon_current);
                sb.AppendFormat(" unpivot (dayall for days in({0}", getDiysql(curMon));
                sb.AppendLine(" )) P");
                sb.AppendFormat("  where vcGC= '{0}' and vcZB= '{1}' and vcYear ='{2}' and vcPlant='{3}' and dayall IN ('{4}','{4}A','{4}B')", gc, zb, curMon.Year, plant, curflag);//2018-2-26 Malcolm.L 刘刚
                sb.AppendLine("  ) t");
                sb.AppendLine("  ) tt");
                sb.AppendLine("  union all ");
                sb.AppendLine("  ");
                sb.AppendLine("  select '0' ,'0','0' ,'0','0','0','9999', @banyue as banyue , @zhinum as totalzhi");
            }
            try
            {
                return excute.ExcuteSqlWithSelectToDT(sb.ToString());
            }
            catch
            {
                throw;
            }
        }

        #region 更新包装计划 和 生产计划
        public string updatePack(DataTable dt, string user)// 更新包装日历 和 生产日历
        {
            string msg = "";
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(ComConnectionHelper.GetConnectionString());
            cmd.CommandTimeout = 0;
            cmd.Connection.Open();
            cmd.Transaction = cmd.Connection.BeginTransaction();
            SqlDataAdapter apt = new SqlDataAdapter(cmd);
            try
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string partsno = dt.Rows[i]["vcPartsno"].ToString().Replace("-", "");
                    string month = dt.Rows[i]["vcMonth"].ToString();
                    string vcDock = dt.Rows[i]["vcDock"].ToString();
                    string vcCarType = dt.Rows[i]["vcCarType"].ToString();
                    cmd.CommandText = "  select iQuantityPerContainer  from tPartInfoMaster  where vcPartsno = '" + partsno + "' and vcDock ='" + vcDock + "' and vcCarFamilyCode ='" + vcCarType + "'  and dTimeFrom<= '" + month + "-01" + "' and dTimeTo >= '" + month + "-01" + "' ";
                    DataTable srsdt = new DataTable();
                    apt.Fill(srsdt);
                    cmd.CommandText = "  select * from dbo.MonthPackPlanTblTMP where  montouch='" + month + "'  and  vcPartsno = '" + partsno + "' and vcDock ='" + vcDock + "' and vcCarType ='" + vcCarType + "'";
                    DataTable dt_udt = new DataTable();
                    apt.Fill(dt_udt);
                    SqlCommandBuilder sb = new SqlCommandBuilder(apt);
                    #region 赋值
                    if (dt_udt.Rows.Count == 1)
                    {
                        int srs = Convert.ToInt32(srsdt.Rows[0]["iQuantityPerContainer"]);
                        for (int j = 7, k = 13; j < dt_udt.Columns.Count - 5; j++, k++)
                        {
                            if (dt_udt.Rows[0][j].ToString().Trim().Length > 0 && dt.Rows[i][k].ToString().Trim().Length == 0)
                            {
                                msg = "稼动日历或LeaderTime在计划做成过程中修改，请重新导入SOQREPLY文件。";
                                cmd.Transaction.Rollback();
                                cmd.Connection.Close();
                                return msg;
                            }
                            else if (dt.Rows[i][k].ToString().Trim().Length > 0 && Convert.ToInt32(dt.Rows[i][k]) % srs != 0)
                            {
                                msg = "品番：" + partsno + "，受入：" + vcDock + " 数量维护需为收容数(" + srs + ")的倍数。";
                                cmd.Transaction.Rollback();
                                cmd.Connection.Close();
                                return msg;
                            }
                        }
                        dt_udt.Rows[0]["vcD1b"] = dt.Rows[i]["TD1b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD1b"]) : dt.Rows[i]["TD1b"];
                        dt_udt.Rows[0]["vcD1y"] = dt.Rows[i]["TD1y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD1y"]) : dt.Rows[i]["TD1y"];
                        dt_udt.Rows[0]["vcD2b"] = dt.Rows[i]["TD2b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD2b"]) : dt.Rows[i]["TD2b"];
                        dt_udt.Rows[0]["vcD2y"] = dt.Rows[i]["TD2y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD2y"]) : dt.Rows[i]["TD2y"];
                        dt_udt.Rows[0]["vcD3b"] = dt.Rows[i]["TD3b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD3b"]) : dt.Rows[i]["TD3b"];
                        dt_udt.Rows[0]["vcD3y"] = dt.Rows[i]["TD3y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD3y"]) : dt.Rows[i]["TD3y"];
                        dt_udt.Rows[0]["vcD4b"] = dt.Rows[i]["TD4b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD4b"]) : dt.Rows[i]["TD4b"];
                        dt_udt.Rows[0]["vcD4y"] = dt.Rows[i]["TD4y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD4y"]) : dt.Rows[i]["TD4y"];
                        dt_udt.Rows[0]["vcD5b"] = dt.Rows[i]["TD5b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD5b"]) : dt.Rows[i]["TD5b"];
                        dt_udt.Rows[0]["vcD5y"] = dt.Rows[i]["TD5y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD5y"]) : dt.Rows[i]["TD5y"];
                        dt_udt.Rows[0]["vcD6b"] = dt.Rows[i]["TD6b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD6b"]) : dt.Rows[i]["TD6b"];
                        dt_udt.Rows[0]["vcD6y"] = dt.Rows[i]["TD6y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD6y"]) : dt.Rows[i]["TD6y"];
                        dt_udt.Rows[0]["vcD7b"] = dt.Rows[i]["TD7b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD7b"]) : dt.Rows[i]["TD7b"];
                        dt_udt.Rows[0]["vcD7y"] = dt.Rows[i]["TD7y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD7y"]) : dt.Rows[i]["TD7y"];
                        dt_udt.Rows[0]["vcD8b"] = dt.Rows[i]["TD8b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD8b"]) : dt.Rows[i]["TD8b"];
                        dt_udt.Rows[0]["vcD8y"] = dt.Rows[i]["TD8y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD8y"]) : dt.Rows[i]["TD8y"];
                        dt_udt.Rows[0]["vcD9b"] = dt.Rows[i]["TD9b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD9b"]) : dt.Rows[i]["TD9b"];
                        dt_udt.Rows[0]["vcD9y"] = dt.Rows[i]["TD9y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD9y"]) : dt.Rows[i]["TD9y"];
                        dt_udt.Rows[0]["vcD10b"] = dt.Rows[i]["TD10b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD10b"]) : dt.Rows[i]["TD10b"];
                        dt_udt.Rows[0]["vcD10y"] = dt.Rows[i]["TD10y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD10y"]) : dt.Rows[i]["TD10y"];
                        dt_udt.Rows[0]["vcD11b"] = dt.Rows[i]["TD11b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD11b"]) : dt.Rows[i]["TD11b"];
                        dt_udt.Rows[0]["vcD11y"] = dt.Rows[i]["TD11y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD11y"]) : dt.Rows[i]["TD11y"];
                        dt_udt.Rows[0]["vcD12b"] = dt.Rows[i]["TD12b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD12b"]) : dt.Rows[i]["TD12b"];
                        dt_udt.Rows[0]["vcD12y"] = dt.Rows[i]["TD12y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD12y"]) : dt.Rows[i]["TD12y"];
                        dt_udt.Rows[0]["vcD13b"] = dt.Rows[i]["TD13b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD13b"]) : dt.Rows[i]["TD13b"];
                        dt_udt.Rows[0]["vcD13y"] = dt.Rows[i]["TD13y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD13y"]) : dt.Rows[i]["TD13y"];
                        dt_udt.Rows[0]["vcD14b"] = dt.Rows[i]["TD14b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD14b"]) : dt.Rows[i]["TD14b"];
                        dt_udt.Rows[0]["vcD14y"] = dt.Rows[i]["TD14y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD14y"]) : dt.Rows[i]["TD14y"];
                        dt_udt.Rows[0]["vcD15b"] = dt.Rows[i]["TD15b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD15b"]) : dt.Rows[i]["TD15b"];
                        dt_udt.Rows[0]["vcD15y"] = dt.Rows[i]["TD15y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD15y"]) : dt.Rows[i]["TD15y"];
                        dt_udt.Rows[0]["vcD16b"] = dt.Rows[i]["TD16b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD16b"]) : dt.Rows[i]["TD16b"];
                        dt_udt.Rows[0]["vcD16y"] = dt.Rows[i]["TD16y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD16y"]) : dt.Rows[i]["TD16y"];
                        dt_udt.Rows[0]["vcD17b"] = dt.Rows[i]["TD17b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD17b"]) : dt.Rows[i]["TD17b"];
                        dt_udt.Rows[0]["vcD17y"] = dt.Rows[i]["TD17y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD17y"]) : dt.Rows[i]["TD17y"];
                        dt_udt.Rows[0]["vcD18b"] = dt.Rows[i]["TD18b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD18b"]) : dt.Rows[i]["TD18b"];
                        dt_udt.Rows[0]["vcD18y"] = dt.Rows[i]["TD18y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD18y"]) : dt.Rows[i]["TD18y"];
                        dt_udt.Rows[0]["vcD19b"] = dt.Rows[i]["TD19b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD19b"]) : dt.Rows[i]["TD19b"];
                        dt_udt.Rows[0]["vcD19y"] = dt.Rows[i]["TD19y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD19y"]) : dt.Rows[i]["TD19y"];
                        dt_udt.Rows[0]["vcD20b"] = dt.Rows[i]["TD20b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD20b"]) : dt.Rows[i]["TD20b"];
                        dt_udt.Rows[0]["vcD20y"] = dt.Rows[i]["TD20y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD20y"]) : dt.Rows[i]["TD20y"];
                        dt_udt.Rows[0]["vcD21b"] = dt.Rows[i]["TD21b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD21b"]) : dt.Rows[i]["TD21b"];
                        dt_udt.Rows[0]["vcD21y"] = dt.Rows[i]["TD21y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD21y"]) : dt.Rows[i]["TD21y"];
                        dt_udt.Rows[0]["vcD22b"] = dt.Rows[i]["TD22b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD22b"]) : dt.Rows[i]["TD22b"];
                        dt_udt.Rows[0]["vcD22y"] = dt.Rows[i]["TD22y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD22y"]) : dt.Rows[i]["TD22y"];
                        dt_udt.Rows[0]["vcD23b"] = dt.Rows[i]["TD23b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD23b"]) : dt.Rows[i]["TD23b"];
                        dt_udt.Rows[0]["vcD23y"] = dt.Rows[i]["TD23y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD23y"]) : dt.Rows[i]["TD23y"];
                        dt_udt.Rows[0]["vcD24b"] = dt.Rows[i]["TD24b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD24b"]) : dt.Rows[i]["TD24b"];
                        dt_udt.Rows[0]["vcD24y"] = dt.Rows[i]["TD24y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD24y"]) : dt.Rows[i]["TD24y"];
                        dt_udt.Rows[0]["vcD25b"] = dt.Rows[i]["TD25b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD25b"]) : dt.Rows[i]["TD25b"];
                        dt_udt.Rows[0]["vcD25y"] = dt.Rows[i]["TD25y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD25y"]) : dt.Rows[i]["TD25y"];
                        dt_udt.Rows[0]["vcD26b"] = dt.Rows[i]["TD26b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD26b"]) : dt.Rows[i]["TD26b"];
                        dt_udt.Rows[0]["vcD26y"] = dt.Rows[i]["TD26y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD26y"]) : dt.Rows[i]["TD26y"];
                        dt_udt.Rows[0]["vcD27b"] = dt.Rows[i]["TD27b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD27b"]) : dt.Rows[i]["TD27b"];
                        dt_udt.Rows[0]["vcD27y"] = dt.Rows[i]["TD27y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD27y"]) : dt.Rows[i]["TD27y"];
                        dt_udt.Rows[0]["vcD28b"] = dt.Rows[i]["TD28b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD28b"]) : dt.Rows[i]["TD28b"];
                        dt_udt.Rows[0]["vcD28y"] = dt.Rows[i]["TD28y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD28y"]) : dt.Rows[i]["TD28y"];
                        dt_udt.Rows[0]["vcD29b"] = dt.Rows[i]["TD29b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD29b"]) : dt.Rows[i]["TD29b"];
                        dt_udt.Rows[0]["vcD29y"] = dt.Rows[i]["TD29y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD29y"]) : dt.Rows[i]["TD29y"];
                        dt_udt.Rows[0]["vcD30b"] = dt.Rows[i]["TD30b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD30b"]) : dt.Rows[i]["TD30b"];
                        dt_udt.Rows[0]["vcD30y"] = dt.Rows[i]["TD30y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD30y"]) : dt.Rows[i]["TD30y"];
                        dt_udt.Rows[0]["vcD31b"] = dt.Rows[i]["TD31b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD31b"]) : dt.Rows[i]["TD31b"];
                        dt_udt.Rows[0]["vcD31y"] = dt.Rows[i]["TD31y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD31y"]) : dt.Rows[i]["TD31y"];

                        sb = new SqlCommandBuilder(apt);
                        apt.Update(dt_udt);
                    }
                    cmd.CommandText = " select *  from dbo.MonthPackPlanTblTMP where vcMonth='" + month + "' and vcPartsno='" + partsno + "' and vcDock='" + vcDock + "' and vcCarType='" + vcCarType + "'";
                    dt_udt = new DataTable();
                    apt.Fill(dt_udt);
                    if (dt_udt.Rows.Count == 1)
                    {
                        int srs = Convert.ToInt32(srsdt.Rows[0]["iQuantityPerContainer"]);
                        for (int j = 7, k = 75; j < dt_udt.Columns.Count - 5; j++, k++)
                        {
                            if (dt_udt.Rows[0][j].ToString().Trim().Length > 0 && dt.Rows[i][k].ToString().Trim().Length == 0)
                            {
                                msg = "稼动日历或LeaderTime在计划做成过程中修改，请重新导入SOQREPLY文件。";
                                cmd.Transaction.Rollback();
                                cmd.Connection.Close();
                                return msg;
                            }
                            if (dt.Rows[i][k].ToString().Trim().Length > 0 && Convert.ToInt32(dt.Rows[i][k]) % srs != 0)
                            {
                                msg = "品番：" + partsno + "，受入：" + vcDock + " 数量维护需为收容数(" + srs + ")的倍数。";
                                cmd.Transaction.Rollback();
                                cmd.Connection.Close();
                                return msg;
                            }
                        }

                        dt_udt.Rows[0]["vcD1b"] = dt.Rows[i]["ED1b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED1b"]) : dt.Rows[i]["ED1b"];
                        dt_udt.Rows[0]["vcD1y"] = dt.Rows[i]["ED1y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED1y"]) : dt.Rows[i]["ED1y"];
                        dt_udt.Rows[0]["vcD2b"] = dt.Rows[i]["ED2b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED2b"]) : dt.Rows[i]["ED2b"];
                        dt_udt.Rows[0]["vcD2y"] = dt.Rows[i]["ED2y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED2y"]) : dt.Rows[i]["ED2y"];
                        dt_udt.Rows[0]["vcD3b"] = dt.Rows[i]["ED3b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED3b"]) : dt.Rows[i]["ED3b"];
                        dt_udt.Rows[0]["vcD3y"] = dt.Rows[i]["ED3y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED3y"]) : dt.Rows[i]["ED3y"];
                        dt_udt.Rows[0]["vcD4b"] = dt.Rows[i]["ED4b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED4b"]) : dt.Rows[i]["ED4b"];
                        dt_udt.Rows[0]["vcD4y"] = dt.Rows[i]["ED4y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED4y"]) : dt.Rows[i]["ED4y"];
                        dt_udt.Rows[0]["vcD5b"] = dt.Rows[i]["ED5b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED5b"]) : dt.Rows[i]["ED5b"];
                        dt_udt.Rows[0]["vcD5y"] = dt.Rows[i]["ED5y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED5y"]) : dt.Rows[i]["ED5y"];
                        dt_udt.Rows[0]["vcD6b"] = dt.Rows[i]["ED6b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED6b"]) : dt.Rows[i]["ED6b"];
                        dt_udt.Rows[0]["vcD6y"] = dt.Rows[i]["ED6y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED6y"]) : dt.Rows[i]["ED6y"];
                        dt_udt.Rows[0]["vcD7b"] = dt.Rows[i]["ED7b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED7b"]) : dt.Rows[i]["ED7b"];
                        dt_udt.Rows[0]["vcD7y"] = dt.Rows[i]["ED7y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED7y"]) : dt.Rows[i]["ED7y"];
                        dt_udt.Rows[0]["vcD8b"] = dt.Rows[i]["ED8b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED8b"]) : dt.Rows[i]["ED8b"];
                        dt_udt.Rows[0]["vcD8y"] = dt.Rows[i]["ED8y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED8y"]) : dt.Rows[i]["ED8y"];
                        dt_udt.Rows[0]["vcD9b"] = dt.Rows[i]["ED9b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED9b"]) : dt.Rows[i]["ED9b"];
                        dt_udt.Rows[0]["vcD9y"] = dt.Rows[i]["ED9y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED9y"]) : dt.Rows[i]["ED9y"];
                        dt_udt.Rows[0]["vcD10b"] = dt.Rows[i]["ED10b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED10b"]) : dt.Rows[i]["ED10b"];
                        dt_udt.Rows[0]["vcD10y"] = dt.Rows[i]["ED10y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED10y"]) : dt.Rows[i]["ED10y"];
                        dt_udt.Rows[0]["vcD11b"] = dt.Rows[i]["ED11b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED11b"]) : dt.Rows[i]["ED11b"];
                        dt_udt.Rows[0]["vcD11y"] = dt.Rows[i]["ED11y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED11y"]) : dt.Rows[i]["ED11y"];
                        dt_udt.Rows[0]["vcD12b"] = dt.Rows[i]["ED12b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED12b"]) : dt.Rows[i]["ED12b"];
                        dt_udt.Rows[0]["vcD12y"] = dt.Rows[i]["ED12y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED12y"]) : dt.Rows[i]["ED12y"];
                        dt_udt.Rows[0]["vcD13b"] = dt.Rows[i]["ED13b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED13b"]) : dt.Rows[i]["ED13b"];
                        dt_udt.Rows[0]["vcD13y"] = dt.Rows[i]["ED13y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED13y"]) : dt.Rows[i]["ED13y"];
                        dt_udt.Rows[0]["vcD14b"] = dt.Rows[i]["ED14b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED14b"]) : dt.Rows[i]["ED14b"];
                        dt_udt.Rows[0]["vcD14y"] = dt.Rows[i]["ED14y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED14y"]) : dt.Rows[i]["ED14y"];
                        dt_udt.Rows[0]["vcD15b"] = dt.Rows[i]["ED15b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED15b"]) : dt.Rows[i]["ED15b"];
                        dt_udt.Rows[0]["vcD15y"] = dt.Rows[i]["ED15y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED15y"]) : dt.Rows[i]["ED15y"];
                        dt_udt.Rows[0]["vcD16b"] = dt.Rows[i]["ED16b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED16b"]) : dt.Rows[i]["ED16b"];
                        dt_udt.Rows[0]["vcD16y"] = dt.Rows[i]["ED16y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED16y"]) : dt.Rows[i]["ED16y"];
                        dt_udt.Rows[0]["vcD17b"] = dt.Rows[i]["ED17b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED17b"]) : dt.Rows[i]["ED17b"];
                        dt_udt.Rows[0]["vcD17y"] = dt.Rows[i]["ED17y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED17y"]) : dt.Rows[i]["ED17y"];
                        dt_udt.Rows[0]["vcD18b"] = dt.Rows[i]["ED18b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED18b"]) : dt.Rows[i]["ED18b"];
                        dt_udt.Rows[0]["vcD18y"] = dt.Rows[i]["ED18y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED18y"]) : dt.Rows[i]["ED18y"];
                        dt_udt.Rows[0]["vcD19b"] = dt.Rows[i]["ED19b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED19b"]) : dt.Rows[i]["ED19b"];
                        dt_udt.Rows[0]["vcD19y"] = dt.Rows[i]["ED19y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED19y"]) : dt.Rows[i]["ED19y"];
                        dt_udt.Rows[0]["vcD20b"] = dt.Rows[i]["ED20b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED20b"]) : dt.Rows[i]["ED20b"];
                        dt_udt.Rows[0]["vcD20y"] = dt.Rows[i]["ED20y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED20y"]) : dt.Rows[i]["ED20y"];
                        dt_udt.Rows[0]["vcD21b"] = dt.Rows[i]["ED21b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED21b"]) : dt.Rows[i]["ED21b"];
                        dt_udt.Rows[0]["vcD21y"] = dt.Rows[i]["ED21y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED21y"]) : dt.Rows[i]["ED21y"];
                        dt_udt.Rows[0]["vcD22b"] = dt.Rows[i]["ED22b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED22b"]) : dt.Rows[i]["ED22b"];
                        dt_udt.Rows[0]["vcD22y"] = dt.Rows[i]["ED22y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED22y"]) : dt.Rows[i]["ED22y"];
                        dt_udt.Rows[0]["vcD23b"] = dt.Rows[i]["ED23b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED23b"]) : dt.Rows[i]["ED23b"];
                        dt_udt.Rows[0]["vcD23y"] = dt.Rows[i]["ED23y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED23y"]) : dt.Rows[i]["ED23y"];
                        dt_udt.Rows[0]["vcD24b"] = dt.Rows[i]["ED24b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED24b"]) : dt.Rows[i]["ED24b"];
                        dt_udt.Rows[0]["vcD24y"] = dt.Rows[i]["ED24y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED24y"]) : dt.Rows[i]["ED24y"];
                        dt_udt.Rows[0]["vcD25b"] = dt.Rows[i]["ED25b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED25b"]) : dt.Rows[i]["ED25b"];
                        dt_udt.Rows[0]["vcD25y"] = dt.Rows[i]["ED25y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED25y"]) : dt.Rows[i]["ED25y"];
                        dt_udt.Rows[0]["vcD26b"] = dt.Rows[i]["ED26b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED26b"]) : dt.Rows[i]["ED26b"];
                        dt_udt.Rows[0]["vcD26y"] = dt.Rows[i]["ED26y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED26y"]) : dt.Rows[i]["ED26y"];
                        dt_udt.Rows[0]["vcD27b"] = dt.Rows[i]["ED27b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED27b"]) : dt.Rows[i]["ED27b"];
                        dt_udt.Rows[0]["vcD27y"] = dt.Rows[i]["ED27y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED27y"]) : dt.Rows[i]["ED27y"];
                        dt_udt.Rows[0]["vcD28b"] = dt.Rows[i]["ED28b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED28b"]) : dt.Rows[i]["ED28b"];
                        dt_udt.Rows[0]["vcD28y"] = dt.Rows[i]["ED28y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED28y"]) : dt.Rows[i]["ED28y"];
                        dt_udt.Rows[0]["vcD29b"] = dt.Rows[i]["ED29b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED29b"]) : dt.Rows[i]["ED29b"];
                        dt_udt.Rows[0]["vcD29y"] = dt.Rows[i]["ED29y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED29y"]) : dt.Rows[i]["ED29y"];
                        dt_udt.Rows[0]["vcD30b"] = dt.Rows[i]["ED30b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED30b"]) : dt.Rows[i]["ED30b"];
                        dt_udt.Rows[0]["vcD30y"] = dt.Rows[i]["ED30y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED30y"]) : dt.Rows[i]["ED30y"];
                        dt_udt.Rows[0]["vcD31b"] = dt.Rows[i]["ED31b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED31b"]) : dt.Rows[i]["ED31b"];
                        dt_udt.Rows[0]["vcD31y"] = dt.Rows[i]["ED31y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED31y"]) : dt.Rows[i]["ED31y"];
                        sb = new SqlCommandBuilder(apt);
                        apt.Update(dt_udt);
                    }
                    #endregion
                }
                #region 更新生产日历
                updateOtherPlan("MonthPackPlanTblTMP", "MonthProPlanTblTMP", cmd, apt, dt, user);
                #endregion
                cmd.Transaction.Commit();
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                cmd.Transaction.Rollback();
                cmd.Connection.Close();
                msg = "error" + ex.ToString();
                return msg;
            }
            return msg;
        }
        #endregion

        #region 由 sourceTable 计划 更新 toTable 计划
        public void updateOtherPlan(string sourceTable, string toTable, SqlCommand cmd, SqlDataAdapter apt, DataTable dt, string user)
        {
            for (int j = 0; j < dt.Rows.Count; j++)
            {
                string partsno = dt.Rows[j]["vcPartsno"].ToString().Replace("-", "").Trim();
                string month = dt.Rows[j]["vcMonth"].ToString();
                string vcDock = dt.Rows[j]["vcDock"].ToString();
                string vcCarType = dt.Rows[j]["vcCarType"].ToString();
                //20180928查看该品番的品番频度 - 李兴旺
                string vcPartFrequence = "";
                string sqlPartFrequence = "SELECT vcPartsNo, vcPartFrequence FROM SPPSBS.dbo.tPartInfoMaster where vcPartsNo = '" + partsno + "' and vcDock = '" + vcDock + "' and vcCarFamilyCode = '" + vcCarType + "' and dTimeFrom<='" + month + "-01' and dTimeTo>='" + month + "-01' ";
                cmd.CommandText = sqlPartFrequence;
                DataTable dtPartFrequence = new DataTable();
                apt.Fill(dtPartFrequence);
                vcPartFrequence = dtPartFrequence.Rows[0]["vcPartFrequence"].ToString().Trim();
                //20180928不是看板打印计划表，则不区分品番频度；是看板打印计划表，则区分品番频度，不是周度的则更新。
                //即toTable == "MonthKanBanPlanTblTMP" && vcPartFrequence == "周度"时，不进行更新操作 - 李兴旺
                if (!(toTable == "MonthKanBanPlanTblTMP" && vcPartFrequence == "周度"))
                {
                    #region 更新操作
                    string tmp = "";
                    for (int i = 1; i < 32; i++)
                    {
                        if (i == 31)
                            tmp += "vcD" + i + "b,	vcD" + i + "y";
                        else tmp += "vcD" + i + "b,	vcD" + i + "y,";
                    }
                    cmd.CommandText = " ";
                    cmd.CommandText += " select vcPartsno, vcDock, vcCartype, sigTotal, allTotal from " + sourceTable + " unpivot( sigTotal for allTotal in( ";
                    cmd.CommandText += tmp;
                    cmd.CommandText += "  )) P where LEN(sigTotal)>0 and vcPartsno = '" + partsno + "' and montouch = '" + month + "' and vcDock = '" + vcDock + "' and vcCartype = '" + vcCarType + "' ";
                    cmd.CommandText += " union all ";
                    cmd.CommandText += " select vcPartsno, vcDock, vcCartype, sigTotal, allTotal from " + sourceTable + " unpivot( sigTotal for allTotal in( ";
                    cmd.CommandText += tmp;
                    cmd.CommandText += "  )) P where LEN(sigTotal)>0 and vcPartsno = '" + partsno + "' and vcMonth = '" + month + "' and vcDock = '" + vcDock + "' and vcCartype = '" + vcCarType + "' ";
                    DataTable dtsource = new DataTable();
                    apt.Fill(dtsource);
                    cmd.CommandText = " select * from " + toTable + " where vcPartsno='" + partsno + "' and montouch ='" + month + "' and vcDock='" + vcDock + "' and vcCartype='" + vcCarType + "'";
                    DataTable dtTo1 = new DataTable();
                    apt.Fill(dtTo1);
                    int num = 0;
                    if (dtTo1.Rows.Count == 1)
                    {
                        cmd.CommandText = "select vcPartsno, vcDock, vcCartype, sigTotal, allTotal from " + toTable + " unpivot (sigTotal for allTotal in( ";
                        cmd.CommandText += tmp;
                        cmd.CommandText += "  )) P where LEN(sigTotal)>0 and vcPartsno = '" + partsno + "' and montouch = '" + month + "' and vcDock = '" + vcDock + "' and vcCartype = '" + vcCarType + "' ";
                        DataTable tmpTo1 = new DataTable();
                        apt.Fill(tmpTo1);
                        string tmpsql = "";
                        for (int k = 0; k < tmpTo1.Rows.Count; k++, num++)
                        {
                            if (k == tmpTo1.Rows.Count - 1)
                            {
                                tmpsql += " " + tmpTo1.Rows[k]["allTotal"].ToString().Trim() + "='" + dtsource.Rows[num]["sigTotal"].ToString().Trim() + "'";
                            }
                            else tmpsql += " " + tmpTo1.Rows[k]["allTotal"].ToString().Trim() + "='" + dtsource.Rows[num]["sigTotal"].ToString().Trim() + "' , ";
                        }
                        cmd.CommandText = "update " + toTable + " set " + tmpsql + " , DUPDTIME=getdate() , CUPDUSER='" + user + "' where vcPartsno ='" + partsno + "' and vcDock='" + vcDock + "' and montouch='" + month + "' and vcCartype='" + vcCarType + "'";
                        cmd.ExecuteNonQuery();
                    }
                    cmd.CommandText = " select * from " + toTable + " where vcPartsno='" + partsno + "' and vcMonth ='" + month + "' and vcDock='" + vcDock + "' and vcCartype='" + vcCarType + "'";
                    DataTable dtTo2 = new DataTable();
                    apt.Fill(dtTo2);
                    if (dtTo2.Rows.Count == 1)
                    {
                        cmd.CommandText = "select vcPartsno, vcDock, vcCartype, sigTotal, allTotal from " + toTable + " unpivot( sigTotal for allTotal in( ";
                        cmd.CommandText += tmp;
                        cmd.CommandText += "  )) P where LEN(sigTotal)>0 and vcPartsno='" + partsno + "' and vcMonth ='" + month + "' and vcDock='" + vcDock + "' and vcCartype='" + vcCarType + "' ";
                        DataTable tmpTo2 = new DataTable();
                        apt.Fill(tmpTo2);
                        string tmpsql = "";
                        for (int k = 0; k < tmpTo2.Rows.Count; k++, num++)
                        {
                            if (k == tmpTo2.Rows.Count - 1)
                            {
                                tmpsql += " " + tmpTo2.Rows[k]["allTotal"].ToString().Trim() + "='" + dtsource.Rows[num]["sigTotal"].ToString().Trim() + "'";
                            }
                            else tmpsql += " " + tmpTo2.Rows[k]["allTotal"].ToString().Trim() + "='" + dtsource.Rows[num]["sigTotal"].ToString().Trim() + "' , ";
                        }
                        cmd.CommandText = "update " + toTable + " set " + tmpsql + " , DUPDTIME=getdate() , CUPDUSER='" + user + "'  where vcPartsno ='" + partsno + "' and vcDock='" + vcDock + "' and vcCartype='" + vcCarType + "' and vcMonth='" + month + "'";
                        cmd.ExecuteNonQuery();
                    }
                    #endregion
                }
            }
        }
        #endregion

        #region 更新生产计划 和 其他计划 生成看板打印数据
        public string updatePro(DataTable dt, string user, string mon, ref Exception e, string plant)
        {
            string msg = "";
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(ComConnectionHelper.GetConnectionString());
            cmd.CommandTimeout = 0;
            cmd.Connection.Open();
            cmd.Transaction = cmd.Connection.BeginTransaction();
            SqlDataAdapter apt = new SqlDataAdapter(cmd);
            try
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string partsno = dt.Rows[i]["vcPartsno"].ToString().Replace("-", "").Trim();
                    string month = dt.Rows[i]["vcMonth"].ToString();
                    string vcDock = dt.Rows[i]["vcDock"].ToString();
                    string vcCarType = dt.Rows[i]["vcCarType"].ToString();
                    cmd.CommandText = "  select iQuantityPerContainer  from tPartInfoMaster  where vcPartsno = '" + partsno + "' and vcDock ='" + vcDock + "' and vcCarFamilyCode ='" + vcCarType + "'   and dTimeFrom<= '" + month + "-01" + "' and dTimeTo >= '" + month + "-01" + "' ";
                    DataTable srsdt = new DataTable();
                    apt.Fill(srsdt);
                    cmd.CommandText = " select *  from dbo.MonthProPlanTblTMP where montouch='" + month + "' and vcPartsno='" + partsno + "' and vcDock='" + vcDock + "' and vcCarType='" + vcCarType + "'";
                    DataTable dt_udt = new DataTable();
                    apt.Fill(dt_udt);
                    SqlCommandBuilder sb = new SqlCommandBuilder(apt);
                    #region 赋值
                    if (dt_udt.Rows.Count == 1)
                    {
                        int srs = Convert.ToInt32(srsdt.Rows[0]["iQuantityPerContainer"]);
                        for (int j = 7, k = 13; j < dt_udt.Columns.Count - 5; j++, k++)
                        {
                            if (dt_udt.Rows[0][j].ToString().Trim().Length > 0 && dt.Rows[i][k].ToString().Trim().Length == 0)
                            {
                                msg = "稼动日历或LeaderTime在计划做成过程中修改，请重新导入SOQREPLY文件。";
                                cmd.Transaction.Rollback();
                                cmd.Connection.Close();
                                return msg;
                            }
                            if (dt.Rows[i][k].ToString().Trim().Length > 0 && Convert.ToInt32(dt.Rows[i][k]) % srs != 0)
                            {
                                msg = "品番：" + partsno + "，受入：" + vcDock + " 数量维护需为收容数(" + srs + ")的倍数。";
                                cmd.Transaction.Rollback();
                                cmd.Connection.Close();
                                return msg;
                            }
                        }
                        dt_udt.Rows[0]["vcD1b"] = dt.Rows[i]["TD1b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD1b"]) : dt.Rows[i]["TD1b"];
                        dt_udt.Rows[0]["vcD1y"] = dt.Rows[i]["TD1y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD1y"]) : dt.Rows[i]["TD1y"];
                        dt_udt.Rows[0]["vcD2b"] = dt.Rows[i]["TD2b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD2b"]) : dt.Rows[i]["TD2b"];
                        dt_udt.Rows[0]["vcD2y"] = dt.Rows[i]["TD2y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD2y"]) : dt.Rows[i]["TD2y"];
                        dt_udt.Rows[0]["vcD3b"] = dt.Rows[i]["TD3b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD3b"]) : dt.Rows[i]["TD3b"];
                        dt_udt.Rows[0]["vcD3y"] = dt.Rows[i]["TD3y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD3y"]) : dt.Rows[i]["TD3y"];
                        dt_udt.Rows[0]["vcD4b"] = dt.Rows[i]["TD4b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD4b"]) : dt.Rows[i]["TD4b"];
                        dt_udt.Rows[0]["vcD4y"] = dt.Rows[i]["TD4y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD4y"]) : dt.Rows[i]["TD4y"];
                        dt_udt.Rows[0]["vcD5b"] = dt.Rows[i]["TD5b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD5b"]) : dt.Rows[i]["TD5b"];
                        dt_udt.Rows[0]["vcD5y"] = dt.Rows[i]["TD5y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD5y"]) : dt.Rows[i]["TD5y"];
                        dt_udt.Rows[0]["vcD6b"] = dt.Rows[i]["TD6b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD6b"]) : dt.Rows[i]["TD6b"];
                        dt_udt.Rows[0]["vcD6y"] = dt.Rows[i]["TD6y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD6y"]) : dt.Rows[i]["TD6y"];
                        dt_udt.Rows[0]["vcD7b"] = dt.Rows[i]["TD7b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD7b"]) : dt.Rows[i]["TD7b"];
                        dt_udt.Rows[0]["vcD7y"] = dt.Rows[i]["TD7y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD7y"]) : dt.Rows[i]["TD7y"];
                        dt_udt.Rows[0]["vcD8b"] = dt.Rows[i]["TD8b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD8b"]) : dt.Rows[i]["TD8b"];
                        dt_udt.Rows[0]["vcD8y"] = dt.Rows[i]["TD8y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD8y"]) : dt.Rows[i]["TD8y"];
                        dt_udt.Rows[0]["vcD9b"] = dt.Rows[i]["TD9b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD9b"]) : dt.Rows[i]["TD9b"];
                        dt_udt.Rows[0]["vcD9y"] = dt.Rows[i]["TD9y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD9y"]) : dt.Rows[i]["TD9y"];
                        dt_udt.Rows[0]["vcD10b"] = dt.Rows[i]["TD10b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD10b"]) : dt.Rows[i]["TD10b"];
                        dt_udt.Rows[0]["vcD10y"] = dt.Rows[i]["TD10y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD10y"]) : dt.Rows[i]["TD10y"];
                        dt_udt.Rows[0]["vcD11b"] = dt.Rows[i]["TD11b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD11b"]) : dt.Rows[i]["TD11b"];
                        dt_udt.Rows[0]["vcD11y"] = dt.Rows[i]["TD11y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD11y"]) : dt.Rows[i]["TD11y"];
                        dt_udt.Rows[0]["vcD12b"] = dt.Rows[i]["TD12b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD12b"]) : dt.Rows[i]["TD12b"];
                        dt_udt.Rows[0]["vcD12y"] = dt.Rows[i]["TD12y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD12y"]) : dt.Rows[i]["TD12y"];
                        dt_udt.Rows[0]["vcD13b"] = dt.Rows[i]["TD13b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD13b"]) : dt.Rows[i]["TD13b"];
                        dt_udt.Rows[0]["vcD13y"] = dt.Rows[i]["TD13y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD13y"]) : dt.Rows[i]["TD13y"];
                        dt_udt.Rows[0]["vcD14b"] = dt.Rows[i]["TD14b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD14b"]) : dt.Rows[i]["TD14b"];
                        dt_udt.Rows[0]["vcD14y"] = dt.Rows[i]["TD14y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD14y"]) : dt.Rows[i]["TD14y"];
                        dt_udt.Rows[0]["vcD15b"] = dt.Rows[i]["TD15b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD15b"]) : dt.Rows[i]["TD15b"];
                        dt_udt.Rows[0]["vcD15y"] = dt.Rows[i]["TD15y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD15y"]) : dt.Rows[i]["TD15y"];
                        dt_udt.Rows[0]["vcD16b"] = dt.Rows[i]["TD16b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD16b"]) : dt.Rows[i]["TD16b"];
                        dt_udt.Rows[0]["vcD16y"] = dt.Rows[i]["TD16y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD16y"]) : dt.Rows[i]["TD16y"];
                        dt_udt.Rows[0]["vcD17b"] = dt.Rows[i]["TD17b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD17b"]) : dt.Rows[i]["TD17b"];
                        dt_udt.Rows[0]["vcD17y"] = dt.Rows[i]["TD17y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD17y"]) : dt.Rows[i]["TD17y"];
                        dt_udt.Rows[0]["vcD18b"] = dt.Rows[i]["TD18b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD18b"]) : dt.Rows[i]["TD18b"];
                        dt_udt.Rows[0]["vcD18y"] = dt.Rows[i]["TD18y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD18y"]) : dt.Rows[i]["TD18y"];
                        dt_udt.Rows[0]["vcD19b"] = dt.Rows[i]["TD19b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD19b"]) : dt.Rows[i]["TD19b"];
                        dt_udt.Rows[0]["vcD19y"] = dt.Rows[i]["TD19y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD19y"]) : dt.Rows[i]["TD19y"];
                        dt_udt.Rows[0]["vcD20b"] = dt.Rows[i]["TD20b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD20b"]) : dt.Rows[i]["TD20b"];
                        dt_udt.Rows[0]["vcD20y"] = dt.Rows[i]["TD20y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD20y"]) : dt.Rows[i]["TD20y"];
                        dt_udt.Rows[0]["vcD21b"] = dt.Rows[i]["TD21b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD21b"]) : dt.Rows[i]["TD21b"];
                        dt_udt.Rows[0]["vcD21y"] = dt.Rows[i]["TD21y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD21y"]) : dt.Rows[i]["TD21y"];
                        dt_udt.Rows[0]["vcD22b"] = dt.Rows[i]["TD22b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD22b"]) : dt.Rows[i]["TD22b"];
                        dt_udt.Rows[0]["vcD22y"] = dt.Rows[i]["TD22y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD22y"]) : dt.Rows[i]["TD22y"];
                        dt_udt.Rows[0]["vcD23b"] = dt.Rows[i]["TD23b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD23b"]) : dt.Rows[i]["TD23b"];
                        dt_udt.Rows[0]["vcD23y"] = dt.Rows[i]["TD23y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD23y"]) : dt.Rows[i]["TD23y"];
                        dt_udt.Rows[0]["vcD24b"] = dt.Rows[i]["TD24b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD24b"]) : dt.Rows[i]["TD24b"];
                        dt_udt.Rows[0]["vcD24y"] = dt.Rows[i]["TD24y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD24y"]) : dt.Rows[i]["TD24y"];
                        dt_udt.Rows[0]["vcD25b"] = dt.Rows[i]["TD25b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD25b"]) : dt.Rows[i]["TD25b"];
                        dt_udt.Rows[0]["vcD25y"] = dt.Rows[i]["TD25y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD25y"]) : dt.Rows[i]["TD25y"];
                        dt_udt.Rows[0]["vcD26b"] = dt.Rows[i]["TD26b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD26b"]) : dt.Rows[i]["TD26b"];
                        dt_udt.Rows[0]["vcD26y"] = dt.Rows[i]["TD26y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD26y"]) : dt.Rows[i]["TD26y"];
                        dt_udt.Rows[0]["vcD27b"] = dt.Rows[i]["TD27b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD27b"]) : dt.Rows[i]["TD27b"];
                        dt_udt.Rows[0]["vcD27y"] = dt.Rows[i]["TD27y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD27y"]) : dt.Rows[i]["TD27y"];
                        dt_udt.Rows[0]["vcD28b"] = dt.Rows[i]["TD28b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD28b"]) : dt.Rows[i]["TD28b"];
                        dt_udt.Rows[0]["vcD28y"] = dt.Rows[i]["TD28y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD28y"]) : dt.Rows[i]["TD28y"];
                        dt_udt.Rows[0]["vcD29b"] = dt.Rows[i]["TD29b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD29b"]) : dt.Rows[i]["TD29b"];
                        dt_udt.Rows[0]["vcD29y"] = dt.Rows[i]["TD29y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD29y"]) : dt.Rows[i]["TD29y"];
                        dt_udt.Rows[0]["vcD30b"] = dt.Rows[i]["TD30b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD30b"]) : dt.Rows[i]["TD30b"];
                        dt_udt.Rows[0]["vcD30y"] = dt.Rows[i]["TD30y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD30y"]) : dt.Rows[i]["TD30y"];
                        dt_udt.Rows[0]["vcD31b"] = dt.Rows[i]["TD31b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD31b"]) : dt.Rows[i]["TD31b"];
                        dt_udt.Rows[0]["vcD31y"] = dt.Rows[i]["TD31y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["TD31y"]) : dt.Rows[i]["TD31y"];
                        dt_udt.Rows[0]["CUPDUSER"] = user;
                        dt_udt.Rows[0]["DUPDTIME"] = DateTime.Now;
                        dt_udt.Rows[0]["CUPDUSER"] = user;
                        dt_udt.Rows[0]["DUPDTIME"] = DateTime.Now;
                        sb = new SqlCommandBuilder(apt);
                        apt.Update(dt_udt);
                    }
                    cmd.CommandText = " select  *  from dbo.MonthProPlanTblTMP  where vcMonth='" + month + "' and vcPartsno='" + partsno + "' and vcDock='" + vcDock + "' and vcCarType='" + vcCarType + "'";
                    dt_udt = new DataTable();
                    apt.Fill(dt_udt);
                    if (dt_udt.Rows.Count == 1)
                    {
                        int srs = Convert.ToInt32(srsdt.Rows[0]["iQuantityPerContainer"]);
                        for (int j = 7, k = 75; j < dt_udt.Columns.Count - 5; j++, k++)
                        {
                            if (dt_udt.Rows[0][j].ToString().Trim().Length > 0 && dt.Rows[i][k].ToString().Trim().Length == 0)
                            {
                                msg = "稼动日历或LeaderTime在计划做成过程中修改，请重新导入SOQREPLY文件。";
                                cmd.Transaction.Rollback();
                                cmd.Connection.Close();
                                return msg;
                            }
                            if (dt.Rows[i][k].ToString().Trim().Length > 0 && Convert.ToInt32(dt.Rows[i][k]) % srs != 0)
                            {
                                msg = "品番：" + partsno + "，受入：" + vcDock + " 数量维护需为收容数(" + srs + ")的倍数。";
                                cmd.Transaction.Rollback();
                                cmd.Connection.Close();
                                return msg;
                            }
                        }
                        dt_udt.Rows[0]["vcD1b"] = dt.Rows[i]["ED1b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED1b"]) : dt.Rows[i]["ED1b"];
                        dt_udt.Rows[0]["vcD1y"] = dt.Rows[i]["ED1y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED1y"]) : dt.Rows[i]["ED1y"];
                        dt_udt.Rows[0]["vcD2b"] = dt.Rows[i]["ED2b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED2b"]) : dt.Rows[i]["ED2b"];
                        dt_udt.Rows[0]["vcD2y"] = dt.Rows[i]["ED2y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED2y"]) : dt.Rows[i]["ED2y"];
                        dt_udt.Rows[0]["vcD3b"] = dt.Rows[i]["ED3b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED3b"]) : dt.Rows[i]["ED3b"];
                        dt_udt.Rows[0]["vcD3y"] = dt.Rows[i]["ED3y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED3y"]) : dt.Rows[i]["ED3y"];
                        dt_udt.Rows[0]["vcD4b"] = dt.Rows[i]["ED4b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED4b"]) : dt.Rows[i]["ED4b"];
                        dt_udt.Rows[0]["vcD4y"] = dt.Rows[i]["ED4y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED4y"]) : dt.Rows[i]["ED4y"];
                        dt_udt.Rows[0]["vcD5b"] = dt.Rows[i]["ED5b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED5b"]) : dt.Rows[i]["ED5b"];
                        dt_udt.Rows[0]["vcD5y"] = dt.Rows[i]["ED5y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED5y"]) : dt.Rows[i]["ED5y"];
                        dt_udt.Rows[0]["vcD6b"] = dt.Rows[i]["ED6b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED6b"]) : dt.Rows[i]["ED6b"];
                        dt_udt.Rows[0]["vcD6y"] = dt.Rows[i]["ED6y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED6y"]) : dt.Rows[i]["ED6y"];
                        dt_udt.Rows[0]["vcD7b"] = dt.Rows[i]["ED7b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED7b"]) : dt.Rows[i]["ED7b"];
                        dt_udt.Rows[0]["vcD7y"] = dt.Rows[i]["ED7y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED7y"]) : dt.Rows[i]["ED7y"];
                        dt_udt.Rows[0]["vcD8b"] = dt.Rows[i]["ED8b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED8b"]) : dt.Rows[i]["ED8b"];
                        dt_udt.Rows[0]["vcD8y"] = dt.Rows[i]["ED8y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED8y"]) : dt.Rows[i]["ED8y"];
                        dt_udt.Rows[0]["vcD9b"] = dt.Rows[i]["ED9b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED9b"]) : dt.Rows[i]["ED9b"];
                        dt_udt.Rows[0]["vcD9y"] = dt.Rows[i]["ED9y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED9y"]) : dt.Rows[i]["ED9y"];
                        dt_udt.Rows[0]["vcD10b"] = dt.Rows[i]["ED10b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED10b"]) : dt.Rows[i]["ED10b"];
                        dt_udt.Rows[0]["vcD10y"] = dt.Rows[i]["ED10y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED10y"]) : dt.Rows[i]["ED10y"];
                        dt_udt.Rows[0]["vcD11b"] = dt.Rows[i]["ED11b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED11b"]) : dt.Rows[i]["ED11b"];
                        dt_udt.Rows[0]["vcD11y"] = dt.Rows[i]["ED11y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED11y"]) : dt.Rows[i]["ED11y"];
                        dt_udt.Rows[0]["vcD12b"] = dt.Rows[i]["ED12b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED12b"]) : dt.Rows[i]["ED12b"];
                        dt_udt.Rows[0]["vcD12y"] = dt.Rows[i]["ED12y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED12y"]) : dt.Rows[i]["ED12y"];
                        dt_udt.Rows[0]["vcD13b"] = dt.Rows[i]["ED13b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED13b"]) : dt.Rows[i]["ED13b"];
                        dt_udt.Rows[0]["vcD13y"] = dt.Rows[i]["ED13y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED13y"]) : dt.Rows[i]["ED13y"];
                        dt_udt.Rows[0]["vcD14b"] = dt.Rows[i]["ED14b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED14b"]) : dt.Rows[i]["ED14b"];
                        dt_udt.Rows[0]["vcD14y"] = dt.Rows[i]["ED14y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED14y"]) : dt.Rows[i]["ED14y"];
                        dt_udt.Rows[0]["vcD15b"] = dt.Rows[i]["ED15b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED15b"]) : dt.Rows[i]["ED15b"];
                        dt_udt.Rows[0]["vcD15y"] = dt.Rows[i]["ED15y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED15y"]) : dt.Rows[i]["ED15y"];
                        dt_udt.Rows[0]["vcD16b"] = dt.Rows[i]["ED16b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED16b"]) : dt.Rows[i]["ED16b"];
                        dt_udt.Rows[0]["vcD16y"] = dt.Rows[i]["ED16y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED16y"]) : dt.Rows[i]["ED16y"];
                        dt_udt.Rows[0]["vcD17b"] = dt.Rows[i]["ED17b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED17b"]) : dt.Rows[i]["ED17b"];
                        dt_udt.Rows[0]["vcD17y"] = dt.Rows[i]["ED17y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED17y"]) : dt.Rows[i]["ED17y"];
                        dt_udt.Rows[0]["vcD18b"] = dt.Rows[i]["ED18b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED18b"]) : dt.Rows[i]["ED18b"];
                        dt_udt.Rows[0]["vcD18y"] = dt.Rows[i]["ED18y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED18y"]) : dt.Rows[i]["ED18y"];
                        dt_udt.Rows[0]["vcD19b"] = dt.Rows[i]["ED19b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED19b"]) : dt.Rows[i]["ED19b"];
                        dt_udt.Rows[0]["vcD19y"] = dt.Rows[i]["ED19y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED19y"]) : dt.Rows[i]["ED19y"];
                        dt_udt.Rows[0]["vcD20b"] = dt.Rows[i]["ED20b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED20b"]) : dt.Rows[i]["ED20b"];
                        dt_udt.Rows[0]["vcD20y"] = dt.Rows[i]["ED20y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED20y"]) : dt.Rows[i]["ED20y"];
                        dt_udt.Rows[0]["vcD21b"] = dt.Rows[i]["ED21b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED21b"]) : dt.Rows[i]["ED21b"];
                        dt_udt.Rows[0]["vcD21y"] = dt.Rows[i]["ED21y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED21y"]) : dt.Rows[i]["ED21y"];
                        dt_udt.Rows[0]["vcD22b"] = dt.Rows[i]["ED22b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED22b"]) : dt.Rows[i]["ED22b"];
                        dt_udt.Rows[0]["vcD22y"] = dt.Rows[i]["ED22y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED22y"]) : dt.Rows[i]["ED22y"];
                        dt_udt.Rows[0]["vcD23b"] = dt.Rows[i]["ED23b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED23b"]) : dt.Rows[i]["ED23b"];
                        dt_udt.Rows[0]["vcD23y"] = dt.Rows[i]["ED23y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED23y"]) : dt.Rows[i]["ED23y"];
                        dt_udt.Rows[0]["vcD24b"] = dt.Rows[i]["ED24b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED24b"]) : dt.Rows[i]["ED24b"];
                        dt_udt.Rows[0]["vcD24y"] = dt.Rows[i]["ED24y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED24y"]) : dt.Rows[i]["ED24y"];
                        dt_udt.Rows[0]["vcD25b"] = dt.Rows[i]["ED25b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED25b"]) : dt.Rows[i]["ED25b"];
                        dt_udt.Rows[0]["vcD25y"] = dt.Rows[i]["ED25y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED25y"]) : dt.Rows[i]["ED25y"];
                        dt_udt.Rows[0]["vcD26b"] = dt.Rows[i]["ED26b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED26b"]) : dt.Rows[i]["ED26b"];
                        dt_udt.Rows[0]["vcD26y"] = dt.Rows[i]["ED26y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED26y"]) : dt.Rows[i]["ED26y"];
                        dt_udt.Rows[0]["vcD27b"] = dt.Rows[i]["ED27b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED27b"]) : dt.Rows[i]["ED27b"];
                        dt_udt.Rows[0]["vcD27y"] = dt.Rows[i]["ED27y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED27y"]) : dt.Rows[i]["ED27y"];
                        dt_udt.Rows[0]["vcD28b"] = dt.Rows[i]["ED28b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED28b"]) : dt.Rows[i]["ED28b"];
                        dt_udt.Rows[0]["vcD28y"] = dt.Rows[i]["ED28y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED28y"]) : dt.Rows[i]["ED28y"];
                        dt_udt.Rows[0]["vcD29b"] = dt.Rows[i]["ED29b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED29b"]) : dt.Rows[i]["ED29b"];
                        dt_udt.Rows[0]["vcD29y"] = dt.Rows[i]["ED29y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED29y"]) : dt.Rows[i]["ED29y"];
                        dt_udt.Rows[0]["vcD30b"] = dt.Rows[i]["ED30b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED30b"]) : dt.Rows[i]["ED30b"];
                        dt_udt.Rows[0]["vcD30y"] = dt.Rows[i]["ED30y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED30y"]) : dt.Rows[i]["ED30y"];
                        dt_udt.Rows[0]["vcD31b"] = dt.Rows[i]["ED31b"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED31b"]) : dt.Rows[i]["ED31b"];
                        dt_udt.Rows[0]["vcD31y"] = dt.Rows[i]["ED31y"].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[i]["ED31y"]) : dt.Rows[i]["ED31y"];
                        dt_udt.Rows[0]["CUPDUSER"] = user;
                        dt_udt.Rows[0]["DUPDTIME"] = DateTime.Now;
                        sb = new SqlCommandBuilder(apt);
                        apt.Update(dt_udt);
                    }
                    #endregion
                }
                #region 更新其他计划临时表 -- 更新确定版计划表
                updateOtherPlan("MonthProPlanTblTMP", "MonthPackPlanTblTMP", cmd, apt, dt, user);//包装
                updateOtherPlan("MonthProPlanTblTMP", "MonthP3PlanTblTMP", cmd, apt, dt, user);//工程3
                updateOtherPlan("MonthProPlanTblTMP", "MonthTZPlanTblTMP", cmd, apt, dt, user);//涂装
                updateOtherPlan("MonthProPlanTblTMP", "MonthKanBanPlanTblTMP", cmd, apt, dt, user);//看板打印计划 20180928看板打印计划不包含周度品番 - 李兴旺

                updatePlan(cmd, mon, plant);//更新确定版计划
                deleteTMP(cmd, mon, plant);//删除临时计划
                deleteTMPplan(cmd, mon, plant);//删除多余计划

                UpdatePlanMST(cmd, mon, plant);//更新到计划品番数据表
                updateSOQEX(cmd, mon, apt, user, plant);//更新SOQREPLY导出表
                #endregion
                #region 生成打印数据
                msg = CreatOrderNo(cmd, mon, apt, user, plant);//2018-2-26增加AB值 - Malcolm.L 刘刚
                if (msg.Length > 0)
                {
                    cmd.Transaction.Rollback();
                    cmd.Connection.Close();
                    return msg;
                }
                #endregion
                cmd.Transaction.Commit();
                cmd.Connection.Close();

            }
            catch (Exception ex)
            {
                e = ex;
                msg = "计划生成失败！";
                if (cmd.Transaction != null) cmd.Transaction.Rollback();
                cmd.Connection.Close();
            }
            return msg;
        }

        private void UpdatePlanMST(SqlCommand cmd, string mon, string plant)
        {
            string tmpmon = mon + "-01";
            StringBuilder sb = new StringBuilder();
            sb.Length = 0;
            sb.AppendLine(" insert into tPlanPartInfo ");
            sb.AppendFormat(" select '{0}' as vcMonth, t1.*,'S' as vcEDFlag,t2.vcPartPlant , ", mon);
            sb.AppendLine(" t2.vcPartsNameCHN,t2.vcCurrentPastCode,t2.vcPorType , t2.vcZB,t2.iQuantityPerContainer,t2.vcQFflag from (");
            sb.AppendLine(" select distinct vcPartsno ,vcCarType,vcDock   from MonthPackPlanTbl ");
            sb.AppendFormat(" where montouch ='{0}' or (vcMonth ='{1}' and montouch is null)", mon, mon);
            sb.AppendLine(" ) t1");
            sb.AppendLine(" left join dbo.tPartInfoMaster t2");
            sb.AppendLine(" on t1.vcPartsno = t2.vcPartsNo  and t1.vcDock = t2.vcDock ");
            sb.AppendFormat(" where t2.vcPartPlant ='{0}'  and t2.dTimeFrom <='{1}' and t2.dTimeTo>='{2}'", plant, tmpmon, tmpmon);

            cmd.CommandText = sb.ToString();
            cmd.ExecuteNonQuery();
        }

        public void updateSOQEX(SqlCommand cmd, string mon, SqlDataAdapter apt, string user, string plant)
        {
            try
            {
                // 设置对象月表
                string sqlTblMonth = "";
                string sqlPartType = "";
                string sqlPartDate = "";
                string strDate = "";

                string[] montmp = mon.Split('-');

                if (montmp[1] == "01")
                {
                    sqlTblMonth = "tJanuary";
                }
                else if (montmp[1] == "02")
                {
                    sqlTblMonth = "tFebruary";
                }
                else if (montmp[1] == "03")
                {
                    sqlTblMonth = "tMarch";
                }
                else if (montmp[1] == "04")
                {
                    sqlTblMonth = "tApril";
                }
                else if (montmp[1] == "05")
                {
                    sqlTblMonth = "tMay";
                }
                else if (montmp[1] == "06")
                {
                    sqlTblMonth = "tJune";
                }
                else if (montmp[1] == "07")
                {
                    sqlTblMonth = "tJuly";
                }
                else if (montmp[1] == "08")
                {
                    sqlTblMonth = "tAugust";
                }
                else if (montmp[1] == "09")
                {
                    sqlTblMonth = "tSeptember";
                }
                else if (montmp[1] == "10")
                {
                    sqlTblMonth = "tOctober";
                }
                else if (montmp[1] == "11")
                {
                    sqlTblMonth = "tNovember";
                }
                else if (montmp[1] == "12")
                {
                    sqlTblMonth = "tDecember";
                }

                cmd.CommandText = " select * from MonthPackPlanTbl where vcMonth='" + mon + "' ";
                DataTable dtPack = new DataTable();
                apt.Fill(dtPack);

                DataTable dt_Update = new DataTable();
                cmd.CommandText = " select * from tSOQREPExport where NPCS<>'0' and  vcMonth='" + mon + "' and vcPlant in" + StrToList(plant) + "  order by orderid ";
                apt.Fill(dt_Update);
                for (int i = 0; i < dt_Update.Rows.Count; i++)
                {
                    ////调试
                    //if(i == 56){
                    //    i = i;
                    //}
                    int tmp = 0;
                    string partsno = dt_Update.Rows[i]["PartsNo"].ToString();
                    string cartype = dt_Update.Rows[i]["CFC"].ToString();

                    sqlPartType = "";
                    sqlPartType += "SELECT  [vcCalendar4]  ";
                    sqlPartType += "FROM [ProRuleMst] t1, tPartInfoMaster  t2   ";
                    sqlPartType += "where t2.[vcPorType] = t1.vcPorType and  t2.vcZB  = t1.vcZB    ";
                    sqlPartType += "and t2.vcPartsNo  = '" + partsno + "' and t2.vcCarFamilyCode = '" + cartype + "'   ";
                    sqlPartType += "and t2.dTimeFrom <= '" + mon + "-01" + "' and t2.dTimeTo >= '" + mon + "-01" + "'    ";

                    DataTable dt_Calendar4 = excute.ExcuteSqlWithSelectToDT(sqlPartType);
                    //cmd.CommandText = sqlPartType;
                    //apt.Fill(dt_Calendar4);
                    sqlPartDate = "";
                    if (dt_Calendar4.Rows.Count != 0 && dt_Calendar4.Rows[0]["vcCalendar4"].ToString() != "")
                    {
                        string Caledar4 = dt_Calendar4.Rows[0]["vcCalendar4"].ToString();
                        string[] Caleders = Caledar4.Split('-');

                        sqlPartDate += "SELECT * FROM  " + sqlTblMonth + " where   ";
                        sqlPartDate += "vcPlant = '" + plant + "' and vcYear = '" + montmp[0] + "' and vcMonth = '" + montmp[1] + "'    ";
                        sqlPartDate += "and vcGC = '" + Caleders[0] + "' and vcZB = '" + Caleders[1] + "'   ";
                        DataTable dt_CalendarData = excute.ExcuteSqlWithSelectToDT(sqlPartDate);
                        //cmd.CommandText = sqlPartDate;
                        //apt.Fill(dt_CalendarData);
                        if (dt_CalendarData.Rows.Count != 0)
                        {
                            if (dt_CalendarData.Rows[0]["D1b"].ToString() != "" || dt_CalendarData.Rows[0]["D1y"].ToString() != "")
                            {
                                strDate = "D1";
                            }
                            else if (dt_CalendarData.Rows[0]["D2b"].ToString() != "" || dt_CalendarData.Rows[0]["D2y"].ToString() != "")
                            {
                                strDate = "D2";
                            }
                            else if (dt_CalendarData.Rows[0]["D3b"].ToString() != "" || dt_CalendarData.Rows[0]["D3y"].ToString() != "")
                            {
                                strDate = "D3";
                            }
                            else if (dt_CalendarData.Rows[0]["D4b"].ToString() != "" || dt_CalendarData.Rows[0]["D4y"].ToString() != "")
                            {
                                strDate = "D4";
                            }
                            else if (dt_CalendarData.Rows[0]["D5b"].ToString() != "" || dt_CalendarData.Rows[0]["D5y"].ToString() != "")
                            {
                                strDate = "D5";
                            }
                            else if (dt_CalendarData.Rows[0]["D6b"].ToString() != "" || dt_CalendarData.Rows[0]["D6y"].ToString() != "")
                            {
                                strDate = "D6";
                            }
                            else if (dt_CalendarData.Rows[0]["D7b"].ToString() != "" || dt_CalendarData.Rows[0]["D7y"].ToString() != "")
                            {
                                strDate = "D7";
                            }
                            else if (dt_CalendarData.Rows[0]["D8b"].ToString() != "" || dt_CalendarData.Rows[0]["D8y"].ToString() != "")
                            {
                                strDate = "D8";
                            }
                            else if (dt_CalendarData.Rows[0]["D9b"].ToString() != "" || dt_CalendarData.Rows[0]["D9y"].ToString() != "")
                            {
                                strDate = "D9";
                            }
                            else if (dt_CalendarData.Rows[0]["D10b"].ToString() != "" || dt_CalendarData.Rows[0]["D10y"].ToString() != "")
                            {
                                strDate = "D10";
                            }
                            else if (dt_CalendarData.Rows[0]["D11b"].ToString() != "" || dt_CalendarData.Rows[0]["D11y"].ToString() != "")
                            {
                                strDate = "D11";
                            }
                            else if (dt_CalendarData.Rows[0]["D12b"].ToString() != "" || dt_CalendarData.Rows[0]["D12y"].ToString() != "")
                            {
                                strDate = "D12";
                            }
                            else if (dt_CalendarData.Rows[0]["D13b"].ToString() != "" || dt_CalendarData.Rows[0]["D13y"].ToString() != "")
                            {
                                strDate = "D13";
                            }
                            else if (dt_CalendarData.Rows[0]["D14b"].ToString() != "" || dt_CalendarData.Rows[0]["D14y"].ToString() != "")
                            {
                                strDate = "D14";
                            }
                            else if (dt_CalendarData.Rows[0]["D15b"].ToString() != "" || dt_CalendarData.Rows[0]["D15y"].ToString() != "")
                            {
                                strDate = "D15";
                            }
                            else if (dt_CalendarData.Rows[0]["D16b"].ToString() != "" || dt_CalendarData.Rows[0]["D16y"].ToString() != "")
                            {
                                strDate = "D16";
                            }
                            else if (dt_CalendarData.Rows[0]["D17b"].ToString() != "" || dt_CalendarData.Rows[0]["D17y"].ToString() != "")
                            {
                                strDate = "D17";

                            }
                            else if (dt_CalendarData.Rows[0]["D18b"].ToString() != "" || dt_CalendarData.Rows[0]["D18y"].ToString() != "")
                            {
                                strDate = "D18";
                            }
                            else if (dt_CalendarData.Rows[0]["D19b"].ToString() != "" || dt_CalendarData.Rows[0]["D19y"].ToString() != "")
                            {
                                strDate = "D19";
                            }
                            else if (dt_CalendarData.Rows[0]["D20b"].ToString() != "" || dt_CalendarData.Rows[0]["D20y"].ToString() != "")
                            {
                                strDate = "D20";
                            }
                            else if (dt_CalendarData.Rows[0]["D21b"].ToString() != "" || dt_CalendarData.Rows[0]["D21y"].ToString() != "")
                            {
                                strDate = "D21";
                            }
                            else if (dt_CalendarData.Rows[0]["D22b"].ToString() != "" || dt_CalendarData.Rows[0]["D22y"].ToString() != "")
                            {
                                strDate = "D22";
                            }
                            else if (dt_CalendarData.Rows[0]["D23b"].ToString() != "" || dt_CalendarData.Rows[0]["D23y"].ToString() != "")
                            {
                                strDate = "D23";
                            }
                            else if (dt_CalendarData.Rows[0]["D24b"].ToString() != "" || dt_CalendarData.Rows[0]["D24y"].ToString() != "")
                            {
                                strDate = "D24";
                            }
                            else if (dt_CalendarData.Rows[0]["D25b"].ToString() != "" || dt_CalendarData.Rows[0]["D25y"].ToString() != "")
                            {
                                strDate = "D25";
                            }
                            else if (dt_CalendarData.Rows[0]["D26b"].ToString() != "" || dt_CalendarData.Rows[0]["D26y"].ToString() != "")
                            {
                                strDate = "D26";
                            }
                            else if (dt_CalendarData.Rows[0]["D27b"].ToString() != "" || dt_CalendarData.Rows[0]["D27y"].ToString() != "")
                            {
                                strDate = "D27";
                            }
                            else if (dt_CalendarData.Rows[0]["D28b"].ToString() != "" || dt_CalendarData.Rows[0]["D28y"].ToString() != "")
                            {
                                strDate = "D28";
                            }
                            else if (dt_CalendarData.Rows[0]["D29b"].ToString() != "" || dt_CalendarData.Rows[0]["D29y"].ToString() != "")
                            {
                                strDate = "D29";
                            }
                            else if (dt_CalendarData.Rows[0]["D30b"].ToString() != "" || dt_CalendarData.Rows[0]["D30y"].ToString() != "")
                            {
                                strDate = "D30";
                            }
                            else if (dt_CalendarData.Rows[0]["D31b"].ToString() != "" || dt_CalendarData.Rows[0]["D31y"].ToString() != "")
                            {
                                strDate = "D31";
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("品番" + partsno + "工程4日历类型取得失败");
                    }
                    DataRow[] dr = dtPack.Select("vcMonth='" + mon + "' and vcPartsno ='" + partsno + "' and vcCarType='" + cartype + "' ");
                    dt_Update.Rows[i]["D1"] = Convert.ToInt32(dr[0]["vcD1b"].ToString().Length == 0 ? "0" : dr[0]["vcD1b"]) + Convert.ToInt32(dr[0]["vcD1y"].ToString().Length == 0 ? "0" : dr[0]["vcD1y"]);
                    dt_Update.Rows[i]["D2"] = Convert.ToInt32(dr[0]["vcD2b"].ToString().Length == 0 ? "0" : dr[0]["vcD2b"]) + Convert.ToInt32(dr[0]["vcD2y"].ToString().Length == 0 ? "0" : dr[0]["vcD2y"]);
                    dt_Update.Rows[i]["D3"] = Convert.ToInt32(dr[0]["vcD3b"].ToString().Length == 0 ? "0" : dr[0]["vcD3b"]) + Convert.ToInt32(dr[0]["vcD3y"].ToString().Length == 0 ? "0" : dr[0]["vcD3y"]);
                    dt_Update.Rows[i]["D4"] = Convert.ToInt32(dr[0]["vcD4b"].ToString().Length == 0 ? "0" : dr[0]["vcD4b"]) + Convert.ToInt32(dr[0]["vcD4y"].ToString().Length == 0 ? "0" : dr[0]["vcD4y"]);
                    dt_Update.Rows[i]["D5"] = Convert.ToInt32(dr[0]["vcD5b"].ToString().Length == 0 ? "0" : dr[0]["vcD5b"]) + Convert.ToInt32(dr[0]["vcD5y"].ToString().Length == 0 ? "0" : dr[0]["vcD5y"]);
                    dt_Update.Rows[i]["D6"] = Convert.ToInt32(dr[0]["vcD6b"].ToString().Length == 0 ? "0" : dr[0]["vcD6b"]) + Convert.ToInt32(dr[0]["vcD6y"].ToString().Length == 0 ? "0" : dr[0]["vcD6y"]);
                    dt_Update.Rows[i]["D7"] = Convert.ToInt32(dr[0]["vcD7b"].ToString().Length == 0 ? "0" : dr[0]["vcD7b"]) + Convert.ToInt32(dr[0]["vcD7y"].ToString().Length == 0 ? "0" : dr[0]["vcD7y"]);
                    dt_Update.Rows[i]["D8"] = Convert.ToInt32(dr[0]["vcD8b"].ToString().Length == 0 ? "0" : dr[0]["vcD8b"]) + Convert.ToInt32(dr[0]["vcD8y"].ToString().Length == 0 ? "0" : dr[0]["vcD8y"]);
                    dt_Update.Rows[i]["D9"] = Convert.ToInt32(dr[0]["vcD9b"].ToString().Length == 0 ? "0" : dr[0]["vcD9b"]) + Convert.ToInt32(dr[0]["vcD9y"].ToString().Length == 0 ? "0" : dr[0]["vcD9y"]);
                    dt_Update.Rows[i]["D10"] = Convert.ToInt32(dr[0]["vcD10b"].ToString().Length == 0 ? "0" : dr[0]["vcD10b"]) + Convert.ToInt32(dr[0]["vcD10y"].ToString().Length == 0 ? "0" : dr[0]["vcD10y"]);
                    dt_Update.Rows[i]["D11"] = Convert.ToInt32(dr[0]["vcD11b"].ToString().Length == 0 ? "0" : dr[0]["vcD11b"]) + Convert.ToInt32(dr[0]["vcD11y"].ToString().Length == 0 ? "0" : dr[0]["vcD11y"]);
                    dt_Update.Rows[i]["D12"] = Convert.ToInt32(dr[0]["vcD12b"].ToString().Length == 0 ? "0" : dr[0]["vcD12b"]) + Convert.ToInt32(dr[0]["vcD12y"].ToString().Length == 0 ? "0" : dr[0]["vcD12y"]);
                    dt_Update.Rows[i]["D13"] = Convert.ToInt32(dr[0]["vcD13b"].ToString().Length == 0 ? "0" : dr[0]["vcD13b"]) + Convert.ToInt32(dr[0]["vcD13y"].ToString().Length == 0 ? "0" : dr[0]["vcD13y"]);
                    dt_Update.Rows[i]["D14"] = Convert.ToInt32(dr[0]["vcD14b"].ToString().Length == 0 ? "0" : dr[0]["vcD14b"]) + Convert.ToInt32(dr[0]["vcD14y"].ToString().Length == 0 ? "0" : dr[0]["vcD14y"]);
                    dt_Update.Rows[i]["D15"] = Convert.ToInt32(dr[0]["vcD15b"].ToString().Length == 0 ? "0" : dr[0]["vcD15b"]) + Convert.ToInt32(dr[0]["vcD15y"].ToString().Length == 0 ? "0" : dr[0]["vcD15y"]);
                    dt_Update.Rows[i]["D16"] = Convert.ToInt32(dr[0]["vcD16b"].ToString().Length == 0 ? "0" : dr[0]["vcD16b"]) + Convert.ToInt32(dr[0]["vcD16y"].ToString().Length == 0 ? "0" : dr[0]["vcD16y"]);
                    dt_Update.Rows[i]["D17"] = Convert.ToInt32(dr[0]["vcD17b"].ToString().Length == 0 ? "0" : dr[0]["vcD17b"]) + Convert.ToInt32(dr[0]["vcD17y"].ToString().Length == 0 ? "0" : dr[0]["vcD17y"]);
                    dt_Update.Rows[i]["D18"] = Convert.ToInt32(dr[0]["vcD18b"].ToString().Length == 0 ? "0" : dr[0]["vcD18b"]) + Convert.ToInt32(dr[0]["vcD18y"].ToString().Length == 0 ? "0" : dr[0]["vcD18y"]);
                    dt_Update.Rows[i]["D19"] = Convert.ToInt32(dr[0]["vcD19b"].ToString().Length == 0 ? "0" : dr[0]["vcD19b"]) + Convert.ToInt32(dr[0]["vcD19y"].ToString().Length == 0 ? "0" : dr[0]["vcD19y"]);
                    dt_Update.Rows[i]["D20"] = Convert.ToInt32(dr[0]["vcD20b"].ToString().Length == 0 ? "0" : dr[0]["vcD20b"]) + Convert.ToInt32(dr[0]["vcD20y"].ToString().Length == 0 ? "0" : dr[0]["vcD20y"]);
                    dt_Update.Rows[i]["D21"] = Convert.ToInt32(dr[0]["vcD21b"].ToString().Length == 0 ? "0" : dr[0]["vcD21b"]) + Convert.ToInt32(dr[0]["vcD21y"].ToString().Length == 0 ? "0" : dr[0]["vcD21y"]);
                    dt_Update.Rows[i]["D22"] = Convert.ToInt32(dr[0]["vcD22b"].ToString().Length == 0 ? "0" : dr[0]["vcD22b"]) + Convert.ToInt32(dr[0]["vcD22y"].ToString().Length == 0 ? "0" : dr[0]["vcD22y"]);
                    dt_Update.Rows[i]["D23"] = Convert.ToInt32(dr[0]["vcD23b"].ToString().Length == 0 ? "0" : dr[0]["vcD23b"]) + Convert.ToInt32(dr[0]["vcD23y"].ToString().Length == 0 ? "0" : dr[0]["vcD23y"]);
                    dt_Update.Rows[i]["D24"] = Convert.ToInt32(dr[0]["vcD24b"].ToString().Length == 0 ? "0" : dr[0]["vcD24b"]) + Convert.ToInt32(dr[0]["vcD24y"].ToString().Length == 0 ? "0" : dr[0]["vcD24y"]);
                    dt_Update.Rows[i]["D25"] = Convert.ToInt32(dr[0]["vcD25b"].ToString().Length == 0 ? "0" : dr[0]["vcD25b"]) + Convert.ToInt32(dr[0]["vcD25y"].ToString().Length == 0 ? "0" : dr[0]["vcD25y"]);
                    dt_Update.Rows[i]["D26"] = Convert.ToInt32(dr[0]["vcD26b"].ToString().Length == 0 ? "0" : dr[0]["vcD26b"]) + Convert.ToInt32(dr[0]["vcD26y"].ToString().Length == 0 ? "0" : dr[0]["vcD26y"]);
                    dt_Update.Rows[i]["D27"] = Convert.ToInt32(dr[0]["vcD27b"].ToString().Length == 0 ? "0" : dr[0]["vcD27b"]) + Convert.ToInt32(dr[0]["vcD27y"].ToString().Length == 0 ? "0" : dr[0]["vcD27y"]);
                    dt_Update.Rows[i]["D28"] = Convert.ToInt32(dr[0]["vcD28b"].ToString().Length == 0 ? "0" : dr[0]["vcD28b"]) + Convert.ToInt32(dr[0]["vcD28y"].ToString().Length == 0 ? "0" : dr[0]["vcD28y"]);
                    dt_Update.Rows[i]["D29"] = Convert.ToInt32(dr[0]["vcD29b"].ToString().Length == 0 ? "0" : dr[0]["vcD29b"]) + Convert.ToInt32(dr[0]["vcD29y"].ToString().Length == 0 ? "0" : dr[0]["vcD29y"]);
                    dt_Update.Rows[i]["D30"] = Convert.ToInt32(dr[0]["vcD30b"].ToString().Length == 0 ? "0" : dr[0]["vcD30b"]) + Convert.ToInt32(dr[0]["vcD30y"].ToString().Length == 0 ? "0" : dr[0]["vcD30y"]);
                    dt_Update.Rows[i]["D31"] = Convert.ToInt32(dr[0]["vcD31b"].ToString().Length == 0 ? "0" : dr[0]["vcD31b"]) + Convert.ToInt32(dr[0]["vcD31y"].ToString().Length == 0 ? "0" : dr[0]["vcD31y"]);
                    //dt_Update.Rows[i]["updateFlag"] = "1";
                    tmp = Convert.ToInt32(dt_Update.Rows[i]["D1"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["D2"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["D3"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["D4"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["D5"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["D6"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["D7"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["D8"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["D9"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["D10"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["D11"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["D12"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["D13"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["D14"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["D15"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["D16"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["D17"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["D18"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["D19"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["D20"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["D21"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["D22"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["D23"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["D24"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["D25"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["D26"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["D27"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["D28"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["D29"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["D30"]) +
                        Convert.ToInt32(dt_Update.Rows[i]["D31"]);
                    dt_Update.Rows[i][strDate] = Convert.ToInt32(dt_Update.Rows[i]["NPCS"]) - tmp + Convert.ToInt32(dt_Update.Rows[i][strDate]);
                }
                SqlCommandBuilder scb = new SqlCommandBuilder(apt);
                apt.Update(dt_Update);
                cmd.CommandText = "update tSOQREPExport set updateFlag='1' where  vcMonth='" + mon + "' and vcPlant in" + StrToList(plant) + "";
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void updatePlan(SqlCommand cmd, string mon, string plant)
        {
            cmd.CommandText = "insert into MonthKanBanPlanTbl select * from MonthKanBanPlanTblTMP t where ((vcMonth = '" + mon + "' and  montouch is null) or montouch ='" + mon + "') and exists (select vcPartsNo from dbo.tPartInfoMaster where vcPartPlant in" + StrToList(plant) + " and vcPartsNo = t.vcPartsno and  dTimeFrom<= '" + mon + "-01" + "' and dTimeTo >= '" + mon + "-01" + "');";
            cmd.CommandText += "insert into MonthP3PlanTbl select * from MonthP3PlanTblTMP t where ((vcMonth = '" + mon + "' and  montouch is null) or montouch ='" + mon + "')  and exists (select vcPartsNo from dbo.tPartInfoMaster where vcPartPlant in" + StrToList(plant) + " and vcPartsNo = t.vcPartsno  and  dTimeFrom<= '" + mon + "-01" + "' and dTimeTo >= '" + mon + "-01" + "');";
            cmd.CommandText += "insert into MonthPackPlanTbl select * from MonthPackPlanTblTMP t where ((vcMonth = '" + mon + "' and  montouch is null) or montouch ='" + mon + "')  and exists (select vcPartsNo from dbo.tPartInfoMaster where vcPartPlant in" + StrToList(plant) + " and vcPartsNo = t.vcPartsno  and  dTimeFrom<= '" + mon + "-01" + "' and dTimeTo >= '" + mon + "-01" + "');";
            cmd.CommandText += "insert into MonthProdPlanTbl select * from MonthProPlanTblTMP t where ((vcMonth = '" + mon + "' and  montouch is null) or montouch ='" + mon + "')  and exists (select vcPartsNo from dbo.tPartInfoMaster where vcPartPlant in" + StrToList(plant) + " and vcPartsNo = t.vcPartsno  and  dTimeFrom<= '" + mon + "-01" + "' and dTimeTo >= '" + mon + "-01" + "');";
            cmd.CommandText += "insert into MonthTZPlanTbl select * from MonthTZPlanTblTMP t where ((vcMonth = '" + mon + "' and  montouch is null) or montouch ='" + mon + "')  and exists (select vcPartsNo from dbo.tPartInfoMaster where vcPartPlant in" + StrToList(plant) + " and vcPartsNo = t.vcPartsno  and  dTimeFrom<= '" + mon + "-01" + "' and dTimeTo >= '" + mon + "-01" + "');";
            cmd.ExecuteNonQuery();
        }

        public void deleteTMP(SqlCommand cmd, string mon, string plant)
        {
            cmd.CommandText = "delete from MonthKanBanPlanTblTMP  where ((vcMonth = '" + mon + "' and  montouch is null) or montouch ='" + mon + "')  and exists (select vcPartsNo from dbo.tPartInfoMaster where vcPartPlant in" + StrToList(plant) + " and vcPartsNo = MonthKanBanPlanTblTMP.vcPartsno  and  dTimeFrom<= '" + mon + "-01" + "' and dTimeTo >= '" + mon + "-01" + "');";
            cmd.CommandText += "delete from MonthP3PlanTblTMP  where ((vcMonth = '" + mon + "' and  montouch is null) or montouch ='" + mon + "')  and exists (select vcPartsNo from dbo.tPartInfoMaster where vcPartPlant in" + StrToList(plant) + " and vcPartsNo = MonthP3PlanTblTMP.vcPartsno and  dTimeFrom<= '" + mon + "-01" + "' and dTimeTo >= '" + mon + "-01" + "');";
            cmd.CommandText += "delete from MonthPackPlanTblTMP  where ((vcMonth = '" + mon + "' and  montouch is null) or montouch ='" + mon + "')  and exists (select vcPartsNo from dbo.tPartInfoMaster where vcPartPlant in" + StrToList(plant) + " and vcPartsNo = MonthPackPlanTblTMP.vcPartsno and  dTimeFrom<= '" + mon + "-01" + "' and dTimeTo >= '" + mon + "-01" + "');";
            cmd.CommandText += "delete from MonthProPlanTblTMP  where ((vcMonth = '" + mon + "' and  montouch is null) or montouch ='" + mon + "')  and exists (select vcPartsNo from dbo.tPartInfoMaster where vcPartPlant in" + StrToList(plant) + " and vcPartsNo = MonthProPlanTblTMP.vcPartsno and  dTimeFrom<= '" + mon + "-01" + "' and dTimeTo >= '" + mon + "-01" + "');";
            cmd.CommandText += "delete from MonthTZPlanTblTMP where ((vcMonth = '" + mon + "' and  montouch is null) or montouch ='" + mon + "')  and exists (select vcPartsNo from dbo.tPartInfoMaster where vcPartPlant in" + StrToList(plant) + " and vcPartsNo = MonthTZPlanTblTMP.vcPartsno and  dTimeFrom<= '" + mon + "-01" + "' and dTimeTo >= '" + mon + "-01" + "');";
            cmd.ExecuteNonQuery();
        }

        public void deleteTMPplan(SqlCommand cmd, string mon, string plant)
        {
            string ssql = "";
            ssql += "  delete  from MonthP3PlanTbl where vcPartsno in  (";
            ssql += "  select t1.vcPartsno from dbo.MonthP3PlanTbl t1";
            ssql += "  left join dbo.tPartInfoMaster t2 ";
            ssql += "  on t1.vcPartsno = t2.vcPartsNo and t1.vcDock = t2.vcDock  and   t2.dTimeFrom<= '" + mon + "-01" + "' and t2.dTimeTo >= '" + mon + "-01" + "' ";
            ssql += "  left join dbo.ProRuleMst t3 on t2.vcPorType = t3.vcPorType and t2.vcZB = t3.vcZB ";
            ssql += "  where t3.vcCalendar3 is null or LEN(t3.vcCalendar3)=0";
            ssql += "  ) and   vcDock in ";
            ssql += "  (";
            ssql += "  select t1.vcDock  from dbo.MonthP3PlanTbl t1";
            ssql += "  left join dbo.tPartInfoMaster t2 ";
            ssql += "  on t1.vcPartsno = t2.vcPartsNo and t1.vcDock = t2.vcDock  and   t2.dTimeFrom<= '" + mon + "-01" + "' and t2.dTimeTo >= '" + mon + "-01" + "' ";
            ssql += "  left join dbo.ProRuleMst t3 on t2.vcPorType = t3.vcPorType and t2.vcZB = t3.vcZB ";
            ssql += "  where t3.vcCalendar3 is null or LEN(t3.vcCalendar3)=0";
            ssql += "  )";

            cmd.CommandText = ssql;
            cmd.ExecuteNonQuery();

            ssql = "";
            ssql += "  delete  from dbo.MonthTZPlanTbl where vcPartsno in  (";
            ssql += "  select t1.vcPartsno from MonthTZPlanTbl t1";
            ssql += "  left join dbo.tPartInfoMaster t2 ";
            ssql += "  on t1.vcPartsno = t2.vcPartsNo and t1.vcDock = t2.vcDock and   t2.dTimeFrom<= '" + mon + "-01" + "' and t2.dTimeTo >= '" + mon + "-01" + "' ";
            ssql += "  left join dbo.ProRuleMst t3 on t2.vcPorType = t3.vcPorType and t2.vcZB = t3.vcZB ";
            ssql += "  where t3.vcCalendar2 is null or LEN(t3.vcCalendar2)=0";
            ssql += "  ) and   vcDock in ";
            ssql += "  (";
            ssql += "  select t1.vcDock  from dbo.MonthTZPlanTbl t1";
            ssql += "  left join dbo.tPartInfoMaster t2 ";
            ssql += "  on t1.vcPartsno = t2.vcPartsNo and t1.vcDock = t2.vcDock  and   t2.dTimeFrom<= '" + mon + "-01" + "' and t2.dTimeTo >= '" + mon + "-01" + "' ";
            ssql += "  left join dbo.ProRuleMst t3 on t2.vcPorType = t3.vcPorType and t2.vcZB = t3.vcZB ";
            ssql += "  where t3.vcCalendar2 is null or LEN(t3.vcCalendar2)=0";
            ssql += "  )";
            cmd.CommandText = ssql;
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 生成看板打印数据
        /// </summary>
        public string CreatOrderNo(SqlCommand cmd, string mon, SqlDataAdapter apt, string user, string plant)
        {
            string msg = "";
            string tmp = "";
            for (int i = 1; i < 32; i++)
            {
                if (i == 31)
                    tmp += "vcD" + i + "b,	vcD" + i + "y";
                else tmp += "vcD" + i + "b,	vcD" + i + "y,";
            }
            StringBuilder sb = new StringBuilder();
            sb.Length = 0;
            sb.AppendLine(" select distinct vcMonth,allTotal, daysig from ( ");
            sb.AppendLine("   select distinct vcMonth,allTotal, daysig,vcPartsno ,vcDock from (");
            sb.AppendLine("   select vcMonth, vcPartsno,vcDock,vcCartype,sigTotal , allTotal from MonthPackPlanTbl");
            sb.AppendFormat("    unpivot( sigTotal for allTotal in( {0} ", tmp);
            sb.AppendFormat("  )) P where LEN(sigTotal)>0 and montouch ='{0}'", mon);
            sb.AppendLine("    union all ");
            sb.AppendLine("    select vcMonth,vcPartsno,vcDock,vcCartype,sigTotal , allTotal from MonthPackPlanTbl  ");
            sb.AppendFormat("    unpivot( sigTotal for allTotal in( {0} ", tmp);
            sb.AppendFormat("  )) P where LEN(sigTotal)>0 and vcMonth ='{0}'", mon);
            sb.AppendLine("    ) t1");
            sb.AppendLine("    left join (");
            sb.AppendFormat("    select daysig , dayN from sPlanConst unpivot ( daysig for dayN in( {0}", tmp);
            sb.AppendLine("     )) P ) t2 ");
            sb.AppendLine("     on t1.allTotal = t2.dayN");
            sb.AppendLine(" ) tall ");
            sb.AppendLine(" left join dbo.tPartInfoMaster tinfo on tall.vcPartsno = tinfo.vcPartsNo and tall.vcDock = tinfo.vcDock  and   tinfo.dTimeFrom<= '" + mon + "-01" + "' and tinfo.dTimeTo >= '" + mon + "-01" + "'");
            sb.AppendFormat(" where tinfo.vcPartPlant ='{0}' ", plant);
            sb.AppendLine("    order by vcMonth , allTotal");
            cmd.CommandText = sb.ToString();
            DataTable DayType = new DataTable();
            apt.Fill(DayType);

            cmd.CommandText = GetPlanSearch(mon, "MonthKanBanPlanTbl", plant);//看板计划
            DataTable pro0 = new DataTable();
            apt.Fill(pro0);
            cmd.CommandText = GetPlanSearch(mon, "MonthProdPlanTbl", plant);//生产计划
            DataTable pro1 = new DataTable();
            apt.Fill(pro1);
            cmd.CommandText = GetPlanSearch(mon, "MonthTZPlanTbl", plant);//涂装计划
            DataTable pro2 = new DataTable();
            apt.Fill(pro2);
            cmd.CommandText = GetPlanSearch(mon, "MonthP3PlanTbl", plant);//P3计划
            DataTable pro3 = new DataTable();
            apt.Fill(pro3);
            cmd.CommandText = GetPlanSearch(mon, "MonthPackPlanTbl", plant);//包装计划
            DataTable pro4 = new DataTable();
            apt.Fill(pro4);
            //------------------优化start
            cmd.CommandText = " select top(1)* from tKanbanPrintTbl ";
            DataTable BulkInsert = new DataTable();
            apt.Fill(BulkInsert);
            BulkInsert = BulkInsert.Clone();
            BulkInsert.Columns.Add("bushu");
            BulkInsert.Columns.Add("dayin");
            BulkInsert.Columns.Add("shengchan");
            string partsql = " select vcPartsno,vcDock,vcCarFamilyCode ,t1.iQuantityPerContainer,t1.vcPorType,t1.vcZB,t2.vcProName0,t2.vcProName1,t2.vcProName2,t2.vcProName3,t2.vcProName4,t2.vcCalendar0,t2.vcCalendar1,t2.vcCalendar2,t2.vcCalendar3,t2.vcCalendar4  from dbo.tPartInfoMaster t1";
            partsql += " left join dbo.ProRuleMst t2 on t1.vcPorType=t2.vcPorType and t1.vcZB = t2.vcZB ";
            partsql += "  where exists (select vcPartsno from MonthPackPlanTbl where (vcMonth='" + mon + "' or montouch ='" + mon + "') and vcPartsno = t1.vcPartsno  )  and t1.dTimeFrom<= '" + mon + "-01" + "' and t1.dTimeTo >= '" + mon + "-01" + "'  ";
            cmd.CommandText = partsql;
            DataTable dtcalendarname = new DataTable();
            apt.Fill(dtcalendarname);
            DataTable dt_calendarname = new DataTable();
            //------------------优化end
            for (int i = 0; i < DayType.Rows.Count; i++)
            {
                DataRow[] dr = pro4.Select(" vcMonth ='" + DayType.Rows[i]["vcMonth"].ToString() + "' and allTotal ='" + DayType.Rows[i]["allTotal"].ToString() + "' ");
                int OrderStart = 0000;
                string tmp_part = "";
                string tmp_dock = "";
                for (int j = 0; j < dr.Length; j++)
                {
                    string vcPartsno = dr[j]["vcPartsno"].ToString();
                    string vcDock = dr[j]["vcDock"].ToString();
                    string vcCartype = dr[j]["vcCartype"].ToString();
                    string flag = dr[j]["flag"].ToString();
                    //20180929查看该品番的品番频度 - 李兴旺                    
                    string vcPartFrequence = "";
                    string sqlPartFrequence = "SELECT vcPartsNo, vcPartFrequence FROM SPPSBS.dbo.tPartInfoMaster where vcPartsNo = '" + vcPartsno + "' and vcDock = '" + vcDock + "' and vcCarFamilyCode = '" + vcCartype + "' and dTimeFrom<='" + mon + "-01' and dTimeTo>='" + mon + "-01'  ";
                    cmd.CommandText = sqlPartFrequence;
                    DataTable dtPartFrequence = new DataTable();
                    apt.Fill(dtPartFrequence);
                    vcPartFrequence = dtPartFrequence.Rows[0]["vcPartFrequence"].ToString().Trim();
                    //20180929看板打印计划表没有周度品番数据，而其他工程的计划表有周度品番，且生成的看板打印数据需要读取其他工程的计划数据，
                    //如果不筛选则会报索引超出了数组界限的错误，因此生成看板打印数据时也要筛选一次周度品番 - 李兴旺
                    if (vcPartFrequence != "周度")
                    {
                        #region 生成看板打印数据
                        dt_calendarname = dtcalendarname.Select("vcPartsno='" + vcPartsno + "'  and vcDock ='" + vcDock + "' and vcCarFamilyCode ='" + vcCartype + "'   ").CopyToDataTable();
                        string srs = dt_calendarname.Rows[0]["iQuantityPerContainer"].ToString().Trim();
                        int k = Convert.ToInt32(dr[j]["sigTotal"]) / Convert.ToInt32(srs);
                        if (tmp_part != vcPartsno || tmp_dock != vcDock)
                        {
                            tmp_part = vcPartsno;
                            tmp_dock = vcDock;
                            OrderStart = 0000;
                        }
                        if (k == 0)
                            continue;
                        //pro0
                        DataRow[] dr0 = pro0.Select(" vcPartsno='" + vcPartsno + "' and vcDock='" + vcDock + "' and vcCartype='" + vcCartype + "' and flag='" + flag + "' ");
                        string day00 = dr0[0]["daysig"].ToString().Split('-')[0];
                        string vcBanZhi00 = dr0[0]["daysig"].ToString().Split('-')[1];
                        string vcComDate00 = dr0[0]["vcMonth"].ToString() + "-" + day00;
                        string zhi00 = vcBanZhi00 == "白" ? "0" : "1";
                        //2018-2-26 Malcolm.L 刘刚 获取工程0的A/B班值
                        string vcAB00 = getABClass(vcComDate00, zhi00, plant, dt_calendarname.Rows[0]["vcCalendar0"].ToString().Trim());
                        string by0 = vcBanZhi00 == "白" ? "01" : "02";
                        string vcProject00 = dt_calendarname.Rows[0]["vcProName0"].ToString();
                        //pro1
                        DataRow[] dr1 = pro1.Select(" vcPartsno='" + vcPartsno + "' and vcDock='" + vcDock + "' and vcCartype='" + vcCartype + "' and flag='" + flag + "' ");
                        string day01 = dr1[0]["daysig"].ToString().Split('-')[0];
                        string vcBanZhi01 = dr1[0]["daysig"].ToString().Split('-')[1];
                        string vcComDate01 = dr1[0]["vcMonth"].ToString() + "-" + day01;
                        string zhi01 = vcBanZhi01 == "白" ? "0" : "1";
                        //2018-2-26 Malcolm.L 刘刚 获取工程1的A/B班值
                        string vcAB01 = getABClass(vcComDate01, zhi01, plant, dt_calendarname.Rows[0]["vcCalendar1"].ToString().Trim());
                        string by1 = vcBanZhi01 == "白" ? "01" : "02";
                        string vcProject01 = dt_calendarname.Rows[0]["vcProName1"].ToString();
                        //pro2
                        DataRow[] dr2 = pro2.Select(" vcPartsno='" + vcPartsno + "' and vcDock='" + vcDock + "' and vcCartype='" + vcCartype + "' and flag='" + flag + "' ");
                        string day02 = "";
                        string vcBanZhi02 = "";
                        string vcComDate02 = "";
                        string zhi02 = "";
                        string vcAB02 = "";//2018-2-26 Malcolm.L 刘刚 获取工程2的A/B班值
                        string by2 = "";
                        string vcProject02 = "";
                        if (dr2.Length > 0)
                        {
                            day02 = dr2[0]["daysig"].ToString().Split('-')[0];
                            vcBanZhi02 = dr2[0]["daysig"].ToString().Split('-')[1];
                            vcComDate02 = dr2[0]["vcMonth"].ToString() + "-" + day02;
                            zhi02 = vcBanZhi02 == "白" ? "0" : "1";
                            //2018-2-26 Malcolm.L 刘刚 获取工程2的A/B班值
                            vcAB02 = getABClass(vcComDate02, zhi02, plant, dt_calendarname.Rows[0]["vcCalendar2"].ToString().Trim());
                            by2 = vcBanZhi02 == "白" ? "01" : "02";
                            vcProject02 = dt_calendarname.Rows[0]["vcProName2"].ToString();
                        }
                        //pro3
                        DataRow[] dr3 = pro3.Select(" vcPartsno='" + vcPartsno + "' and vcDock='" + vcDock + "' and vcCartype='" + vcCartype + "' and flag='" + flag + "' ");
                        string day03 = "";
                        string vcBanZhi03 = "";
                        string vcComDate03 = "";
                        string zhi03 = "";
                        string vcAB03 = "";//2018-2-26 Malcolm.L 刘刚 获取工程3的A/B班值
                        string by3 = "";
                        string vcProject03 = "";
                        if (dr3.Length > 0)
                        {
                            day03 = dr3[0]["daysig"].ToString().Split('-')[0];
                            vcBanZhi03 = dr3[0]["daysig"].ToString().Split('-')[1];
                            vcComDate03 = dr3[0]["vcMonth"].ToString() + "-" + day03;
                            zhi03 = vcBanZhi03 == "白" ? "0" : "1";
                            //2018-2-26 Malcolm.L 刘刚 获取工程3的A/B班值
                            vcAB03 = getABClass(vcComDate03, zhi03, plant, dt_calendarname.Rows[0]["vcCalendar3"].ToString().Trim());
                            by3 = vcBanZhi03 == "白" ? "01" : "02";
                            vcProject03 = dt_calendarname.Rows[0]["vcProName3"].ToString();
                        }
                        //pro4
                        string day04 = dr[j]["daysig"].ToString().Split('-')[0];
                        string vcBanZhi04 = dr[j]["daysig"].ToString().Split('-')[1];//白/夜  白是0 夜是1
                        string vcComDate04 = dr[j]["vcMonth"].ToString() + "-" + day04;
                        string zhi04 = vcBanZhi04 == "白" ? "0" : "1";
                        //2018-2-26 Malcolm.L 刘刚 获取工程0的A/B班值
                        string vcAB04 = getABClass(vcComDate04, zhi04, plant, dt_calendarname.Rows[0]["vcCalendar4"].ToString().Trim());
                        string by04 = vcBanZhi04 == "白" ? "01" : "02";
                        string vcProject04 = dt_calendarname.Rows[0]["vcProName4"].ToString();
                        for (int n = 0; n < k; n++)
                        {
                            OrderStart++;
                            string orderSerial = "";
                            string orderNo = vcComDate04.Replace("-", "") + by04;//订单号
                            //优化start
                            DataRow drInsert = BulkInsert.NewRow();
                            drInsert["iNo"] = DBNull.Value;
                            drInsert["vcTips"] = DBNull.Value;
                            drInsert["vcPrintflag"] = DBNull.Value;
                            drInsert["vcPrintTime"] = DBNull.Value;
                            drInsert["vcKBType"] = DBNull.Value;
                            drInsert["dUpdateTime"] = DBNull.Value;
                            drInsert["vcUpdater"] = DBNull.Value;
                            drInsert["vcPrintSpec"] = DBNull.Value;
                            drInsert["vcPrintflagED"] = DBNull.Value;
                            drInsert["vcPrintTimeED"] = DBNull.Value;

                            drInsert["vcQuantityPerContainer"] = srs;
                            drInsert["vcPartsNo"] = dr[j]["vcPartsno"].ToString();
                            drInsert["vcDock"] = dr[j]["vcDock"].ToString();
                            drInsert["vcCarType"] = dr[j]["vcCartype"].ToString();
                            drInsert["vcEDflag"] = 'S';
                            drInsert["vcKBorderno"] = orderNo;
                            drInsert["vcKBSerial"] = orderSerial;
                            drInsert["vcProject00"] = vcProject00;
                            drInsert["vcProject01"] = vcProject01;
                            drInsert["vcProject02"] = vcProject02;
                            drInsert["vcProject03"] = vcProject03;
                            drInsert["vcProject04"] = vcProject04;
                            drInsert["vcComDate00"] = vcComDate00;
                            drInsert["vcComDate01"] = vcComDate01;
                            drInsert["vcComDate02"] = vcComDate02;
                            drInsert["vcComDate03"] = vcComDate03;
                            drInsert["vcComDate04"] = vcComDate04;
                            drInsert["vcBanZhi00"] = zhi00;
                            drInsert["vcBanZhi01"] = zhi01;
                            drInsert["vcBanZhi02"] = zhi02;
                            drInsert["vcBanZhi03"] = zhi03;
                            drInsert["vcBanZhi04"] = zhi04;
                            drInsert["vcAB00"] = vcAB00;//
                            drInsert["vcAB01"] = vcAB01;//
                            drInsert["vcAB02"] = vcAB02;//
                            drInsert["vcAB03"] = vcAB03;//
                            drInsert["vcAB04"] = vcAB04;//
                            drInsert["vcCreater"] = user;
                            drInsert["dCreatTime"] = DateTime.Now;
                            drInsert["vcPlanMonth"] = mon;

                            drInsert["bushu"] = dt_calendarname.Rows[0]["vcPorType"].ToString();//部署
                            drInsert["dayin"] = vcComDate00 + zhi00;//打印
                            drInsert["shengchan"] = vcComDate01 + zhi01;//生产

                            BulkInsert.Rows.Add(drInsert);
                            //优化end
                        }
                        #endregion
                    }
                }
            }
            //分组
            var query = from t in BulkInsert.AsEnumerable()
                        group t by new
                        {
                            t1 = t.Field<string>("vcKBorderno"),
                            t2 = t.Field<string>("bushu"),
                            t3 = t.Field<string>("dayin"),
                            t4 = t.Field<string>("shengchan")
                        }
                            into m
                        select m;
            //分组排连番
            for (int m = 0; m < query.Count(); m++)
            {
                DataTable dt = query.ElementAt(m).CopyToDataTable();
                //按工位排序
                DataView dv = dt.DefaultView;
                dv.Sort = "vcProject01";
                dt = dv.ToTable();
                string serial = "0000";
                for (int n = 0; n < dt.Rows.Count; n++)
                {
                    serial = (Convert.ToInt32(serial) + 1).ToString("0000");
                    dt.Rows[n]["vcKBSerial"] = serial;
                    //检查该连番下是否存在数据 存在时：
                    string ssql = "select * from tKanbanPrintTbl t1 left join tPartInfoMaster t2 on t1.vcPartsNo = t2.vcPartsNo and t1.vcDock  = t2.vcDock ";
                    ssql += "  where t1.vcEDflag ='S' and t1.vcKBorderno='" + dt.Rows[n]["vcKBorderno"].ToString() + "' ";
                    ssql += " and t2.vcPorType = '" + dt.Rows[n]["bushu"].ToString() + "' and vcComDate00 ='" + dt.Rows[n]["vcComDate00"].ToString() + "' and vcBanZhi00='" + dt.Rows[n]["vcBanZhi00"].ToString() + "' ";
                    ssql += " and vcComDate01 ='" + dt.Rows[n]["vcComDate01"].ToString() + "' and vcBanZhi01 ='" + dt.Rows[n]["vcBanZhi01"].ToString() + "' ";
                    ssql += " and t2.vcPartFrequence = '月度' ";//明早测试去掉这行的效果
                    //20180930上面SQL文最后一行增加对周度月度品番的判断，因为SQL文是按照部署、工程0和工程1的日期和值别检索数据，
                    //会包括周度品番，而看板打印数据不包含周度品番，且其余月度品番的看板序列号也会发生变化，则造成覆盖范围不符的情况
                    //因此需要清空旧有的（若导入对象月为七月，则包括六月底到八月初的）看板打印数据 - 李兴旺
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = ssql;
                    apt = new SqlDataAdapter(cmd);
                    DataTable dttmp = new DataTable();
                    apt.Fill(dttmp);
                    if (dttmp.Rows.Count > 0)//如果存在
                    {
                        DataRow[] existKB = dttmp.Select("vcPartsNo = '" + dt.Rows[n]["vcPartsNo"].ToString().Trim() + "' and vcDock ='" + dt.Rows[n]["vcDock"].ToString().Trim() + "' and vcKBSerial ='" + serial + "'");
                        if (existKB.Length > 0)//1、与该信息相符 
                        {
                            dt.Rows[n].Delete();
                            n--;
                            continue;
                        }
                        else//2、与该信息不符
                        {
                            msg = "生产计划调整与覆盖范围不符，请重新调整后再导入。";
                            return msg;
                        }
                    }
                    else//插入
                    {

                    }
                }
                if (dt.Rows.Count == 0) continue;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "InsertKanBanPrint";
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@vcPartsNo", SqlDbType.VarChar, 12, "vcPartsNo");
                cmd.Parameters.Add("@vcDock", SqlDbType.VarChar, 2, "vcDock");
                cmd.Parameters.Add("@vcCarType", SqlDbType.VarChar, 4, "vcCarType");
                cmd.Parameters.Add("@vcEDflag", SqlDbType.VarChar, 2, "vcEDflag");
                cmd.Parameters.Add("@vcKBorderno", SqlDbType.VarChar, 12, "vcKBorderno");
                cmd.Parameters.Add("@vcKBSerial", SqlDbType.VarChar, 4, "vcKBSerial");
                cmd.Parameters.Add("@vcProject00", SqlDbType.VarChar, 20, "vcProject00");
                cmd.Parameters.Add("@vcProject01", SqlDbType.VarChar, 20, "vcProject01");
                cmd.Parameters.Add("@vcProject02", SqlDbType.VarChar, 20, "vcProject02");
                cmd.Parameters.Add("@vcProject03", SqlDbType.VarChar, 20, "vcProject03");
                cmd.Parameters.Add("@vcProject04", SqlDbType.VarChar, 20, "vcProject04");
                cmd.Parameters.Add("@vcComDate00", SqlDbType.VarChar, 10, "vcComDate00");
                cmd.Parameters.Add("@vcComDate01", SqlDbType.VarChar, 10, "vcComDate01");
                cmd.Parameters.Add("@vcComDate02", SqlDbType.VarChar, 10, "vcComDate02");
                cmd.Parameters.Add("@vcComDate03", SqlDbType.VarChar, 10, "vcComDate03");
                cmd.Parameters.Add("@vcComDate04", SqlDbType.VarChar, 10, "vcComDate04");
                cmd.Parameters.Add("@vcBanZhi00", SqlDbType.VarChar, 1, "vcBanZhi00");
                cmd.Parameters.Add("@vcBanZhi01", SqlDbType.VarChar, 1, "vcBanZhi01");
                cmd.Parameters.Add("@vcBanZhi02", SqlDbType.VarChar, 1, "vcBanZhi02");
                cmd.Parameters.Add("@vcBanZhi03", SqlDbType.VarChar, 1, "vcBanZhi03");
                cmd.Parameters.Add("@vcBanZhi04", SqlDbType.VarChar, 1, "vcBanZhi04");
                cmd.Parameters.Add("@vcAB00", SqlDbType.VarChar, 1, "vcAB00");//
                cmd.Parameters.Add("@vcAB01", SqlDbType.VarChar, 1, "vcAB01");//
                cmd.Parameters.Add("@vcAB02", SqlDbType.VarChar, 1, "vcAB02");//
                cmd.Parameters.Add("@vcAB03", SqlDbType.VarChar, 1, "vcAB03");//
                cmd.Parameters.Add("@vcAB04", SqlDbType.VarChar, 1, "vcAB04");//
                cmd.Parameters.Add("@vcCreater", SqlDbType.VarChar, 10, "vcCreater");
                cmd.Parameters.Add("@vcPlanMonth", SqlDbType.VarChar, 7, "vcPlanMonth");
                cmd.Parameters.Add("@vcQuantityPerContainer", SqlDbType.VarChar, 7, "vcQuantityPerContainer");
                apt = new SqlDataAdapter();
                apt.InsertCommand = cmd;
                apt.Update(dt);
            }
            return msg;
        }
        public string GetPlanSearch(string mon, string TableName, string plant)
        {
            string tmp = "";
            for (int i = 1; i < 32; i++)
            {
                if (i == 31)
                    tmp += "vcD" + i + "b,	vcD" + i + "y";
                else tmp += "vcD" + i + "b,	vcD" + i + "y,";
            }
            StringBuilder sb = new StringBuilder();
            //sb.AppendLine("    select Tall.vcMonth,Tall.vcPartsno,Tall.vcDock,Tall.vcCarType,Tall.sigTotal*Tinfo.iQuantityPerContainer as sigTotal,Tall.allTotal ,Tall.daysig ,ROW_NUMBER() over(partition by Tall.vcPartsno, Tall.vcDock,Tall.vcCartype order by Tall.vcMonth,Tall.daysig,Tall.vcPartsno,Tall.vcDock,Tall.vcCartype) as flag from (");
            sb.AppendLine("    select Tall.vcMonth,Tall.vcPartsno,Tall.vcDock,Tall.vcCarType,Tall.sigTotal ,Tall.allTotal ,Tall.daysig ,ROW_NUMBER() over(partition by Tall.vcPartsno, Tall.vcDock,Tall.vcCartype order by Tall.vcMonth,Tall.daysig,Tall.vcPartsno,Tall.vcDock,Tall.vcCartype) as flag from (");
            sb.AppendLine("   select t1.*,t2.daysig from (");
            sb.AppendFormat("   select vcMonth, vcPartsno,vcDock,vcCartype,sigTotal , allTotal from {0}", TableName);
            sb.AppendFormat("    unpivot( sigTotal for allTotal in( {0} ", tmp);
            sb.AppendFormat("  )) P where LEN(sigTotal)>0 and montouch ='{0}'", mon);
            sb.AppendLine("    union all ");
            sb.AppendFormat("    select vcMonth,vcPartsno,vcDock,vcCartype,sigTotal , allTotal from {0}  ", TableName);
            sb.AppendFormat("    unpivot( sigTotal for allTotal in( {0} ", tmp);
            sb.AppendFormat("  )) P where LEN(sigTotal)>0 and vcMonth ='{0}'", mon);
            sb.AppendLine("    ) t1");
            sb.AppendLine("    left join (");
            sb.AppendFormat("    select daysig , dayN from sPlanConst unpivot ( daysig for dayN in( {0}", tmp);
            sb.AppendLine("     )) P ) t2 ");
            sb.AppendLine("     on t1.allTotal = t2.dayN");
            sb.AppendLine("    ) Tall ");
            sb.AppendLine("  left join dbo.tPartInfoMaster Tinfo on Tall.vcPartsno = Tinfo.vcPartsNo and Tall.vcDock = Tinfo.vcDock and Tall.vcCarType = Tinfo.vcCarFamilyCode  and   Tinfo.dTimeFrom<= '" + mon + "-01" + "' and Tinfo.dTimeTo >= '" + mon + "-01" + "' ");
            sb.AppendFormat("  where Tinfo.vcPartPlant ='{0}'", plant);
            sb.AppendLine(" order by vcMonth ,daysig,vcPartsno,vcDock,vcCartype");
            return sb.ToString();
        }
        #endregion



        public DataTable plantConst()
        {
            string ssql = " select vcData1,vcData2 from dbo.ConstMst where vcDataId ='KBPlant' ";
            return excute.ExcuteSqlWithSelectToDT(ssql);
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

        public DataTable getCutPlanByMon(string mon)
        {
            string ssql = "  ";
            ssql += " SELECT t1.vcMonth, t1.vcPartsno ,t1.vcDock,t1.vcKBorderno , t1.vcKBSerial , t1.vcEDflag ,";
            ssql += " t2.vcComDate00, t2.vcComDate01,t2.vcComDate02,t2.vcComDate03,t2.vcComDate04 ,";
            ssql += " t2.vcBanZhi00, t2.vcBanZhi01, t2.vcBanZhi02, t2.vcBanZhi03, t2.vcBanZhi04,t2.vcQuantityPerContainer as srs";
            ssql += " FROM tPlanCut t1";
            ssql += " left join tKanbanPrintTbl t2";
            ssql += " on t1.vcPartsno=t2.vcPartsNo and t1.vcKBorderno = t2.vcKBorderno and t1.vcKBSerial = t2.vcKBSerial and t1.vcDock = t2.vcDock ";
            ssql += " where vcMonth='" + mon + "' and t1.updateFlag='0' order by vcEDflag ";

            return excute.ExcuteSqlWithSelectToDT(ssql);
        }

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

        public DataTable getTKanbanPrintTbl(string part, string order, string serial, string mon, string ed, string dock)
        {
            string ssql = "select * from tKanbanPrintTbl where " +
                "vcPartsNo='" + part + "' and vcKBorderno='" + order + "' and vcKBSerial='" + serial + "'" +
                " and vcPlanMonth =@mon and vcEDflag =@ed and vcDock =@dock";
            return excute.ExcuteSqlWithSelectToDT(ssql);
        }

        public DataTable getTKanbanPrintTbl(string part, string mon, string ed, string dock)
        {
            string ssql = "select vcPlanMonth, vcPartsNo, vcDock, vcKBorderno, vcKBSerial, vcEDflag" +
                " from tKanbanPrintTbl where vcPartsNo='" + part + "' " +
                " and vcPlanMonth='" + mon + "' and vcEDflag='" + ed + "' and vcDock='" + dock + "'";
            return excute.ExcuteSqlWithSelectToDT(ssql);
        }

        public DataTable getTKanbanPrintTblByOrder(string part, string mon, string order, string dock)
        {
            string ssql = "select vcPlanMonth, vcPartsNo,vcDock ,vcKBorderno ,vcKBSerial, vcEDflag" +
                " from tKanbanPrintTbl where vcPartsNo='" + part + "' " +
                " and vcPlanMonth='" + mon + "' and vcKBorderno='" + order + "' and vcDock='" + dock + "'";
            return excute.ExcuteSqlWithSelectToDT(ssql);
        }

        public DataTable getTKanbanPrintTbl(string part, string mon, string serial, string ed, string dock)
        {
            string ssql = "select vcPlanMonth, vcPartsNo, vcDock, vcKBorderno, vcKBSerial, vcEDflag" +
                " from tKanbanPrintTbl where vcPartsNo='" + part + "' " +
                " and vcPlanMonth='" + mon + "' and vcKBSerial='" + serial + "' and vcEDflag='" + ed + "' and vcDock='" + dock + "'";
            return excute.ExcuteSqlWithSelectToDT(ssql);
        }
        public string PartsChange(string value)
        {
            if (value == "") return "";
            string partsno = value.Split('|')[0];
            string order = value.Split('|')[1];
            string serial = value.Split('|')[2];
            string dock = value.Split('|')[3];
            string ssql = "select top(1) vcEDflag+'|'+vcPlanMonth from tKanbanPrintTbl where vcPartsNo='" + partsno + "' " +
                "and vcKBorderno='" + order + "' and vcKBSerial='" + serial + "' and vcDock='" + dock + "' ";
            DataTable dt = excute.ExcuteSqlWithSelectToDT(ssql);
            if (dt.Rows.Count == 0)
                return "0";
            else
                return dt.Rows[0][0].ToString();
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
            string vcGC = vcCalendar.Split('-')[0];
            string vcZB = vcCalendar.Split('-')[1];
            DataTable dt = excute.ExcuteSqlWithSelectToDT(getClassSql(date, dayNight, vcPlant, vcGC, vcZB));
            Object content = dt.Rows[0][0];
            if (dt.Rows.Count > 1)
            {
                throw new Exception("Date:" + date + ";vcPlant:" + vcPlant + ";vcGC:" + vcGC + ";vcZB:" + vcZB + "; 对应的稼动日不止一条");
            }
            else if (content != null && content.ToString().Length > 0)
            {
                return content.ToString().Substring(1);//若为MA\MB\NA\NB则返回值为A\B或空
            }
            return string.Empty;
        }
    }
}
