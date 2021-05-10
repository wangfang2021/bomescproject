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
    public class FS1208_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        //public string PartsChange(FS1208_ViewModel fS1208_ViewModel)
        //{
        //    string ssql = "select top(1) vcCarFamilyCode from dbo.tPartInfoMaster where" +
        //        " dTimeFrom <= @month and dTimeTo>=@month and vcInOutFlag = '0'  and vcPartsNo = @partsno and vcDock = @dock";

        //    DataSet dataSet = SqlHelper.ExecuteDataset(ComConnectionHelper.GetConnectionString(), CommandType.Text, ssql
        //        , new SqlParameter("@month", fS1208_ViewModel.vcMon)
        //        , new SqlParameter("@partsno", fS1208_ViewModel.vcPartsNo)
        //        , new SqlParameter("@dock", fS1208_ViewModel.vcDock));
        //    DataTable dt = dataSet.Tables[0];
        //    if (dt.Rows.Count == 0)
        //    {
        //        return "0";
        //    }
        //    return dt.Rows[0][0].ToString();
        //}

        public DataTable getEDPlanInfo(string Mon, string Partsno, string cartype, string dock, string type, string pro, string zhi, string day, string order)
        {
            DataTable dt = new DataTable();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(" SELECT iAutoId, t1.vcMonth,t1.vcPlant ,t1.vcPartsno, t1.vcDock, t1.vcCarType, vcNum as vcNum,vcOrderNo, vcPro0Day, vcPro0Zhi, vcPro1Day, vcPro1Zhi, vcPro2Day, vcPro2Zhi, vcPro3Day, vcPro3Zhi, ");
            sb.AppendLine(" vcPro4Day, vcPro4Zhi, case when UpdateFlag IS NULL then '' else UpdateFlag end vcState, '0' as vcModFlag, '0' as vcAddFlag ");
            sb.AppendLine(" FROM EDMonthPlanTMP t1");
            sb.AppendLine(" left join tPlanPartInfo t2 on t1.vcPartsno = t2.vcPartsNo and t1.vcDock = t2.vcDock and t1.vcCarType=t2.vcCarType and t1.vcMonth = t2.vcMonth ");
            sb.AppendLine(" where t2.vcEDFlag='E' ");
            if (Mon.Length > 0)
            {
                sb.AppendFormat(" and t1.vcMonth='{0}' ", Mon);
            }
            if (Partsno.Length > 0)
            {
                sb.AppendFormat(" and t1.vcPartsno='{0}' ", Partsno);
            }
            if (cartype.Length > 0)
            {
                sb.AppendFormat(" and t1.vcCarType='{0}' ", cartype);
            }
            if (dock.Length > 0)
            {
                sb.AppendFormat(" and t1.vcDock='{0}'", dock);
            }
            if (type == "0")
            {
                sb.AppendLine(" and (UpdateFlag is null or UpdateFlag<>'1' )");
            }
            else if (type == "1")
            {
                sb.AppendLine(" and UpdateFlag='1' ");
            }
            if (pro != "")
            {
                string colday = "vcPro" + pro + "Day";
                string colzhi = "vcPro" + pro + "Zhi";
                sb.AppendFormat(" and {0}='{1}' and {2}='{3}' ", colday, day, colzhi, zhi);
            }
            if (order.Length > 0)
            {
                sb.AppendFormat(" and t1.vcOrderNo='{0}'", order);
            }
            try
            {
                return excute.ExcuteSqlWithSelectToDT(sb.ToString());
            }
            catch
            {
                dt = null;
            }
            return dt;
        }

        public DataTable GetPlanType()
        {
            string sql = "select planType,value from sPlanType where enable='1'";
            return excute.ExcuteSqlWithSelectToDT(sql);
        }

        public string UpdateTable(DataTable dt, string user)
        {
            string msg = "";
            SqlCommand cmd = new SqlCommand();
            DataTable dtInfo = new DataTable();
            //if (dt.Select("vc ='0' or vcState is null ").Length == 0) return "无可更新的数据。";
            //dt = dt.Select("vcState ='0' or vcState is null ").CopyToDataTable();
            //dt.Columns.Remove("vcState");
            try
            {
                cmd.Connection = new SqlConnection(ComConnectionHelper.GetConnectionString());
                cmd.CommandTimeout = 0;
                cmd.Connection.Open();
                cmd.Transaction = cmd.Connection.BeginTransaction();
                SqlDataAdapter apt = new SqlDataAdapter(cmd);
                cmd.CommandText = "select vcPartsNo,vcDock,iQuantityPerContainer,dTimeFrom,dTimeTo,vcPorType,vcZB,vcPartPlant,vcCurrentPastCode,vcPartsNameCHN,vcQFflag from tPartInfoMaster where vcInOutFlag='0';select distinct vcPartsNo,vcDock,vcOrderNo from EDMonthPlanTMP;";
                DataSet ds = new DataSet();
                apt.Fill(ds);
                dtInfo = ds.Tables[0];
                DataTable dtOrder = ds.Tables[1];//订单号校验？？？？？？？？？？？？？

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DateTime mon = Convert.ToDateTime(dt.Rows[i]["vcMonth"].ToString());
                    DateTime pro0 = Convert.ToDateTime(dt.Rows[i]["vcPro0Day"].ToString().Length == 0 ? mon.ToString() : dt.Rows[i]["vcPro0Day"].ToString());
                    DateTime pro1 = Convert.ToDateTime(dt.Rows[i]["vcPro1Day"].ToString().Length == 0 ? mon.ToString() : dt.Rows[i]["vcPro1Day"].ToString());
                    DateTime pro2 = Convert.ToDateTime(dt.Rows[i]["vcPro2Day"].ToString().Length == 0 ? mon.ToString() : dt.Rows[i]["vcPro2Day"].ToString());
                    DateTime pro3 = Convert.ToDateTime(dt.Rows[i]["vcPro3Day"].ToString().Length == 0 ? mon.ToString() : dt.Rows[i]["vcPro3Day"].ToString());
                    DateTime pro4 = Convert.ToDateTime(dt.Rows[i]["vcPro4Day"].ToString().Length == 0 ? mon.ToString() : dt.Rows[i]["vcPro4Day"].ToString());
                    if ((mon.Month != pro0.Month && mon.AddMonths(-1).Month != pro0.Month) ||
                        (mon.Month != pro1.Month && mon.AddMonths(-1).Month != pro1.Month) ||
                        (mon.Month != pro2.Month && mon.AddMonths(-1).Month != pro2.Month) ||
                        (mon.Month != pro3.Month && mon.AddMonths(-1).Month != pro3.Month) ||
                        (mon.Month != pro4.Month && mon.AddMonths(-1).Month != pro4.Month))
                    {
                        msg = "第" + ((i % 10) + 1) + " 行月份维护错误。";
                        return msg;
                    }
                    if (dt.Rows[i]["vcPro0Day"].ToString().Length > 0 && dt.Rows[i]["vcPro0Zhi"].ToString().Length <= 0
                        || dt.Rows[i]["vcPro1Day"].ToString().Length > 0 && dt.Rows[i]["vcPro1Zhi"].ToString().Length <= 0
                        || dt.Rows[i]["vcPro2Day"].ToString().Length > 0 && dt.Rows[i]["vcPro2Zhi"].ToString().Length <= 0
                        || dt.Rows[i]["vcPro3Day"].ToString().Length > 0 && dt.Rows[i]["vcPro3Zhi"].ToString().Length <= 0
                        || dt.Rows[i]["vcPro4Day"].ToString().Length > 0 && dt.Rows[i]["vcPro4Zhi"].ToString().Length <= 0
                        || dt.Rows[i]["vcPro0Day"].ToString().Length <= 0 && dt.Rows[i]["vcPro0Zhi"].ToString().Length > 0
                        || dt.Rows[i]["vcPro1Day"].ToString().Length <= 0 && dt.Rows[i]["vcPro1Zhi"].ToString().Length > 0
                        || dt.Rows[i]["vcPro2Day"].ToString().Length <= 0 && dt.Rows[i]["vcPro2Zhi"].ToString().Length > 0
                        || dt.Rows[i]["vcPro3Day"].ToString().Length <= 0 && dt.Rows[i]["vcPro3Zhi"].ToString().Length > 0
                        || dt.Rows[i]["vcPro4Day"].ToString().Length <= 0 && dt.Rows[i]["vcPro4Zhi"].ToString().Length > 0
                        )
                    {
                        msg = "第" + ((i % 10) + 1) + " 行值别维护错误。";
                        return msg;
                    }
                    string tmpmon = dt.Rows[i]["vcMonth"].ToString() + "-01";

                    if (dtInfo.Select(" vcPartsNo ='" + dt.Rows[i]["vcPartsno"].ToString() + "' and vcDock ='" + dt.Rows[i]["vcDock"].ToString() + "'  and dTimeFrom <='" + tmpmon + "' and dTimeTo>='" + tmpmon + "'  ").Length == 0)
                    {
                        msg = "品番：" + dt.Rows[i]["vcPartsno"].ToString() + "受入：" + dt.Rows[i]["vcDock"].ToString() + "，品番信息不存在。";
                        return msg;
                    }

                    DataRow tmp = dtInfo.Select(" vcPartsNo ='" + dt.Rows[i]["vcPartsno"].ToString() + "' and vcDock ='" + dt.Rows[i]["vcDock"].ToString() + "'  and dTimeFrom <='" + tmpmon + "' and dTimeTo>='" + tmpmon + "'  ")[0];
                    dt.Rows[i]["vcPlant"] = tmp["vcPartPlant"].ToString();
                    string srs = tmp["iQuantityPerContainer"].ToString();
                    if (dt.Rows[i]["vcMonth"].ToString().Length <= 0 || dt.Rows[i]["vcPartsno"].ToString().Length <= 0 || dt.Rows[i]["vcDock"].ToString().Length <= 0 || dt.Rows[i]["vcCarType"].ToString().Length <= 0 || dt.Rows[i]["vcOrderNo"].ToString().Length <= 0)
                    {
                        msg = " 基础信息不能为空，请填充完整再进行更新。";
                        return msg;
                    }
                    if (dt.Rows[i]["vcPro4Day"].ToString().Length == 0 || dt.Rows[i]["vcPro0Day"].ToString().Length == 0 || dt.Rows[i]["vcPro1Day"].ToString().Length == 0)
                    {
                        msg = "品番：" + dt.Rows[i]["vcPartsno"].ToString() + "受入：" + dt.Rows[i]["vcDock"].ToString() + "，工程0、工程1与工程4不能为空。";
                        return msg;
                    }
                    if (Convert.ToInt32(dt.Rows[i]["vcNum"]) == 0 || Convert.ToInt32(dt.Rows[i]["vcNum"]) % Convert.ToInt32(srs) != 0)
                    {
                        msg = "品番：" + dt.Rows[i]["vcPartsno"].ToString() + "受入：" + dt.Rows[i]["vcDock"].ToString() + "， 数量需大于0且为收容数的倍数，请修正后再进行更新。";
                        return msg;
                    }
                    else
                    {
                        //dt.Rows[i]["vcNum"] = Convert.ToInt32(dt.Rows[i]["vcNum"]) / Convert.ToInt32(srs);
                        dt.Rows[i]["vcNum"] = Convert.ToInt32(dt.Rows[i]["vcNum"]);
                    }
                    if (tmp["vcPorType"].ToString().Length == 0 || tmp["vcZB"].ToString().Length == 0 || tmp["vcPartPlant"].ToString().Length == 0)
                    {
                        msg = "品番：" + dt.Rows[i]["vcPartsno"].ToString() + "受入：" + dt.Rows[i]["vcDock"].ToString() + "， 生产部署或组别或工场未维护。";
                        return msg;
                    }
                    //if (dt.Rows[i]["vcOrderNo"].ToString().Trim().Length != 10 && dt.Rows[i]["vcOrderNo"].ToString().Trim().Length != 12)
                    //{
                    //    msg = "品番：" + dt.Rows[i]["vcPartsno"].ToString() + ", 受入：" + dt.Rows[i]["vcDock"].ToString() + ",订单号位数需为10位或12位";
                    //    return msg;
                    //}
                }

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string tmpmon = dt.Rows[i]["vcMonth"].ToString() + "-01";
                    DataRow tmp = dtInfo.Select(" vcPartsNo ='" + dt.Rows[i]["vcPartsno"].ToString() + "' and vcDock ='" + dt.Rows[i]["vcDock"].ToString() + "'  and dTimeFrom <='" + tmpmon + "' and dTimeTo>='" + tmpmon + "'  ")[0];
                    if (Convert.ToBoolean(dt.Rows[i]["vcModFlag"]) && Convert.ToBoolean(dt.Rows[i]["vcAddFlag"]))//新增
                    {
                        //if (dt.Rows[i]["vcMonth"].ToString().Length <= 0 || dt.Rows[i]["vcPartsno"].ToString().Length <= 0 || dt.Rows[i]["vcDock"].ToString().Length <= 0 || dt.Rows[i]["vcCarType"].ToString().Length <= 0)
                        //{
                        //    msg = " 基础信息不能为空，请填充完整再进行更新。";
                        //    cmd.Transaction.Rollback();
                        //    cmd.Connection.Close();
                        //    return msg;
                        //}
                        //if (Convert.ToInt32(dt.Rows[i]["vcNum"]) == 0)
                        //{
                        //    msg = " 数量不能为0，请修正后再进行更新。";
                        //    cmd.Transaction.Rollback();
                        //    cmd.Connection.Close();
                        //    return msg; 
                        //}
                        DataRow[] drOrder = dtOrder.Select(" vcPartsNo ='" + dt.Rows[i]["vcPartsno"].ToString() + "'  and vcDock ='" + dt.Rows[i]["vcDock"].ToString() + "' and vcOrderNo ='" + dt.Rows[i]["vcOrderNo"].ToString() + "' ");
                        if (drOrder.Length > 0)
                        {
                            msg = "品番：" + dt.Rows[i]["vcPartsno"].ToString() + ", 受入：" + dt.Rows[i]["vcDock"].ToString() + ",订单号：" + dt.Rows[i]["vcOrderNo"].ToString() + ",已经存在。";
                            cmd.Transaction.Rollback();
                            cmd.Connection.Close();
                            return msg;
                        }
                        StringBuilder sb = new StringBuilder();
                        sb.Length = 0;
                        sb.AppendLine(" INSERT INTO EDMonthPlanTMP ([vcMonth],[vcPlant],[vcPartsno],[vcDock],[vcCarType],[vcNum],[vcOrderNo],[vcPro0Day],[vcPro0Zhi],[vcPro1Day]");
                        sb.AppendLine(" ,[vcPro1Zhi],[vcPro2Day],[vcPro2Zhi],[vcPro3Day],[vcPro3Zhi],[vcPro4Day],[vcPro4Zhi],[vcCreaterID],[dCreatTime]) ");
                        sb.AppendFormat(" VALUES ('{0}'", dt.Rows[i]["vcMonth"].ToString());
                        sb.AppendFormat(" ,'{0}' ", dt.Rows[i]["vcPlant"].ToString());
                        sb.AppendFormat(" ,'{0}' ", dt.Rows[i]["vcPartsno"].ToString());
                        sb.AppendFormat(" ,'{0}' ", dt.Rows[i]["vcDock"].ToString());
                        sb.AppendFormat(" ,'{0}' ", dt.Rows[i]["vcCarType"].ToString());
                        sb.AppendFormat(" ,'{0}' ", dt.Rows[i]["vcNum"].ToString());
                        sb.AppendFormat(" ,'{0}' ", dt.Rows[i]["vcOrderNo"].ToString());
                        sb.AppendFormat(" ,'{0}' ", dt.Rows[i]["vcPro0Day"].ToString());
                        sb.AppendFormat(" ,'{0}' ", dt.Rows[i]["vcPro0Zhi"].ToString());
                        sb.AppendFormat(" ,'{0}' ", dt.Rows[i]["vcPro1Day"].ToString());
                        sb.AppendFormat(" ,'{0}' ", dt.Rows[i]["vcPro1Zhi"].ToString());
                        sb.AppendFormat(" ,'{0}' ", dt.Rows[i]["vcPro2Day"].ToString());
                        sb.AppendFormat(" ,'{0}' ", dt.Rows[i]["vcPro2Zhi"].ToString());
                        sb.AppendFormat(" ,'{0}' ", dt.Rows[i]["vcPro3Day"].ToString());
                        sb.AppendFormat(" ,'{0}' ", dt.Rows[i]["vcPro3Zhi"].ToString());
                        sb.AppendFormat(" ,'{0}' ", dt.Rows[i]["vcPro4Day"].ToString());
                        sb.AppendFormat(" ,'{0}' ", dt.Rows[i]["vcPro4Zhi"].ToString());
                        sb.AppendFormat(" ,'{0}' ", user);
                        sb.AppendLine(" ,getdate() ) ");
                        cmd.CommandText = sb.ToString();
                        cmd.ExecuteNonQuery();

                        sb.Length = 0;
                        sb.AppendFormat(" select * from tPlanPartInfo where vcMonth ='{0}' ", dt.Rows[i]["vcMonth"].ToString());
                        sb.AppendFormat(" and  vcPartsNo ='{0}'", dt.Rows[i]["vcPartsno"].ToString());
                        sb.AppendFormat(" and  vcCarType ='{0}'", dt.Rows[i]["vcCarType"].ToString());
                        sb.AppendFormat(" and  vcDock ='{0}'", dt.Rows[i]["vcDock"].ToString());
                        sb.AppendLine(" and  vcEDFlag ='E'");
                        cmd.CommandText = sb.ToString();
                        apt = new SqlDataAdapter(cmd);
                        DataTable tmpdata = new DataTable();
                        apt.Fill(tmpdata);
                        if (tmpdata.Rows.Count == 0)
                        {
                            sb.Length = 0;
                            sb.AppendLine(" insert into tPlanPartInfo([vcMonth],[vcPartsNo],[vcCarType],[vcDock],[vcEDFlag],[vcPlant],[vcPartNameCN],[vcHJ],[vcProType],[vcZB],[vcSRS],[vcQFflag]) ");
                            sb.AppendFormat(" VALUES ('{0}'", dt.Rows[i]["vcMonth"].ToString());
                            sb.AppendFormat(" ,'{0}' ", dt.Rows[i]["vcPartsno"].ToString());
                            sb.AppendFormat(" ,'{0}' ", dt.Rows[i]["vcCarType"].ToString());
                            sb.AppendFormat(" ,'{0}' ", dt.Rows[i]["vcDock"].ToString());
                            sb.AppendLine(" ,'E' ");
                            sb.AppendFormat(" ,'{0}' ", dt.Rows[i]["vcPlant"].ToString());
                            //sb.AppendFormat(" ,'{0}' ", tmp["vcPartsNameCHN"].ToString());
                            sb.AppendFormat(" ,'s' ");
                            sb.AppendFormat(" ,'{0}' ", tmp["vcCurrentPastCode"].ToString());
                            sb.AppendFormat(" ,'{0}' ", tmp["vcPorType"].ToString());
                            sb.AppendFormat(" ,'{0}' ", tmp["vcZB"].ToString());
                            sb.AppendFormat(" ,'{0}' ", tmp["iQuantityPerContainer"].ToString());
                            sb.AppendFormat(" ,'{0}' )", tmp["vcQFflag"].ToString());
                            cmd.CommandText = sb.ToString();
                            cmd.ExecuteNonQuery();
                        }
                    }
                    if (Convert.ToBoolean(dt.Rows[i]["vcModFlag"]) && !Convert.ToBoolean(dt.Rows[i]["vcAddFlag"]))//修改
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Length = 0;
                        sb.AppendLine(" UPDATE EDMonthPlanTMP");
                        sb.AppendFormat("    SET [vcNum] = '{0}' ", dt.Rows[i]["vcNum"].ToString());
                        sb.AppendFormat("       ,[vcOrderNo] = '{0}' ", dt.Rows[i]["vcOrderNo"].ToString());
                        sb.AppendFormat("       ,[vcPro0Day] = '{0}' ", dt.Rows[i]["vcPro0Day"].ToString());
                        sb.AppendFormat("       ,[vcPro0Zhi] = '{0}' ", dt.Rows[i]["vcPro0Zhi"].ToString());
                        sb.AppendFormat("       ,[vcPro1Day] = '{0}' ", dt.Rows[i]["vcPro1Day"].ToString());
                        sb.AppendFormat("       ,[vcPro1Zhi] = '{0}' ", dt.Rows[i]["vcPro1Zhi"].ToString());
                        sb.AppendFormat("       ,[vcPro2Day] = '{0}' ", dt.Rows[i]["vcPro2Day"].ToString());
                        sb.AppendFormat("       ,[vcPro2Zhi] = '{0}' ", dt.Rows[i]["vcPro2Zhi"].ToString());
                        sb.AppendFormat("       ,[vcPro3Day] = '{0}' ", dt.Rows[i]["vcPro3Day"].ToString());
                        sb.AppendFormat("       ,[vcPro3Zhi] = '{0}' ", dt.Rows[i]["vcPro3Zhi"].ToString());
                        sb.AppendFormat("       ,[vcPro4Day] = '{0}' ", dt.Rows[i]["vcPro4Day"].ToString());
                        sb.AppendFormat("       ,[vcPro4Zhi] = '{0}' ", dt.Rows[i]["vcPro4Zhi"].ToString());
                        sb.AppendFormat("       ,[vcUpdateID] = '{0}' ", user);
                        sb.AppendLine("       ,[dUpdateTime] = getdate() ");
                        sb.AppendFormat("  WHERE [iAutoID] ='{0}' ", dt.Rows[i]["iAutoID"].ToString());
                        cmd.CommandText = sb.ToString();
                        cmd.ExecuteNonQuery();
                    }
                    //if (dt.Rows[i]["iFlag"].ToString() == "3")//删除
                    //{
                    //    StringBuilder sb = new StringBuilder();
                    //    sb.AppendLine(" DELETE FROM EDMonthPlanTMP ");
                    //    sb.AppendFormat(" WHERE [iAutoID]='{0}' ", dt.Rows[i]["iAutoID"].ToString());
                    //    cmd.CommandText = sb.ToString();
                    //    cmd.ExecuteNonQuery();
                    //}
                    //updatePartMst(cmd, dt.Rows[i]["vcMonth"].ToString());
                    updatePartMst2(cmd, dt.Rows[i]["vcMonth"].ToString());
                }
                cmd.Transaction.Commit();
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                if (cmd.Connection.State == ConnectionState.Open)
                {
                    cmd.Transaction.Rollback();
                    cmd.Connection.Close();
                }
                msg = ex.ToString();
                throw ex;
            }
            return msg;
        }

        public void updatePartMst2(SqlCommand cmd, string mon)
        {
            string tmpmon = mon + "-01";
            StringBuilder sb = new StringBuilder();
            sb.Length = 0;
            sb.AppendLine("  delete from tPlanPartInfo");
            sb.AppendLine("  where not exists(");
            sb.AppendFormat(" select distinct vcPartsno, vcDock, vcCarType from EDMonthPlanTMP where vcMonth='{0}' ", mon);
            sb.AppendLine("  and vcPartsNo=tPlanPartInfo.vcPartsNo and vcDock=tPlanPartInfo.vcDock and vcCarType=tPlanPartInfo.vcCarType ");
            sb.AppendFormat("  ) and vcMonth='{0}' and vcEDFlag='E'", mon);
            cmd.CommandText = sb.ToString();
            cmd.ExecuteNonQuery();
        }

        public string UpdatePlan(string mon, string user)
        {
            string msg = "";
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(ComConnectionHelper.GetConnectionString());
            cmd.CommandTimeout = 0;
            try
            {
                cmd.Connection.Open();
                cmd.Transaction = cmd.Connection.BeginTransaction();
                //--1 取对象月紧急计划
                SqlDataAdapter apt = new SqlDataAdapter(cmd);
                SqlCommandBuilder scb = new SqlCommandBuilder();
                //生成紧急订单内制看板
                cmd.CommandText = "   ";
                cmd.CommandText += "   SELECT t1.vcMonth, t1.vcPartsno, t1.vcDock, t1.vcCarType,t2.vcSRS as iQuantityPerContainer, vcNum,vcOrderNo, vcPro0Day, vcPro0Zhi, vcPro1Day,";
                cmd.CommandText += "   vcPro1Zhi, vcPro2Day, vcPro2Zhi, vcPro3Day, vcPro3Zhi, vcPro4Day, vcPro4Zhi, UpdateFlag,t3.vcPorType, ";
                cmd.CommandText += "   t3.vcProName0,t3.vcProName1,t3.vcProName2,t3.vcProName3,t3.vcProName4 ";
                cmd.CommandText += "  FROM EDMonthPlanTMP t1";
                cmd.CommandText += "  left join (select * from tPlanPartInfo where vcmonth='" + mon + "' and vcEDFlag='E') t2 ";
                cmd.CommandText += "  on t1.vcPartsno=t2.vcPartsNo and t1.vcCarType=t2.vcCarType and t1.vcDock=t2.vcDock ";
                cmd.CommandText += "  left join ProRuleMst t3 ";
                cmd.CommandText += "  on t3.vcPorType=t2.vcProType and t3.vcZB=t2.vcZB";
                cmd.CommandText += "  where (UpdateFlag is null or UpdateFlag<>'1') and t1.vcMonth='" + mon + "'";
                DataTable dt_print = new DataTable();
                apt.Fill(dt_print);
                if (dt_print.Rows.Count == 0)
                {
                    msg = "该月没有紧急计划请新增后重新生成。";
                    return msg;
                }
                DataTable dtupdate = getInsertKB();
                for (int i = 0; i < dt_print.Rows.Count; i++)
                {
                    string partsno = dt_print.Rows[i]["vcPartsno"].ToString();
                    string dock = dt_print.Rows[i]["vcDock"].ToString();
                    string CarType = dt_print.Rows[i]["vcCarType"].ToString();
                    string srs = dt_print.Rows[i]["iQuantityPerContainer"].ToString();
                    string plant = getPartsPlant(partsno, dock, mon);//20180917根据品番和受入确定该品番厂区 - 李兴旺
                                                                     //根据对象月、品番、受入确定该品番5个工程的vcCalendar - 李兴旺
                    string partsql = " select vcPartsno, vcDock, vcCarFamilyCode, t1.iQuantityPerContainer, t1.vcPorType, t1.vcZB, t2.vcProName0, t2.vcProName1, t2.vcProName2, t2.vcProName3, t2.vcProName4, t2.vcCalendar0, t2.vcCalendar1, t2.vcCalendar2, t2.vcCalendar3, t2.vcCalendar4 from tPartInfoMaster t1 ";
                    partsql += " left join ProRuleMst t2 on t1.vcPorType=t2.vcPorType and t1.vcZB=t2.vcZB ";
                    partsql += " where exists (select vcPartsno from EDMonthPlanTMP where vcMonth='" + mon + "' and vcPartsno=t1.vcPartsno) and t1.dTimeFrom<='" + mon + "-01" + "' and t1.dTimeTo>='" + mon + "-01" + "' ";//20181204修改
                    partsql += " and vcPartsno='" + partsno + "' and vcDock='" + dock + "' ";
                    DataTable dtCalendar = getCalendar(partsql);

                    int num = Convert.ToInt32(dt_print.Rows[i]["vcNum"].ToString().Trim().Length == 0 ? "0" : dt_print.Rows[i]["vcNum"].ToString().Trim());
                    //num = num / Convert.ToInt32(srs);
                    //工程0
                    DataTable dtED0 = getEDProplan(cmd, ref apt, "EDMonthKanBanPlanTbl", mon);
                    string Pro0day = dt_print.Rows[i]["vcPro0Day"].ToString().Trim();
                    string Pro0zhi = dt_print.Rows[i]["vcPro0Zhi"].ToString().Trim();
                    string sql0 = setProData2(Pro0day, Pro0zhi, dtED0, mon, partsno, dock, CarType, num, user, "EDMonthKanBanPlanTbl");
                    if (sql0.Trim().Length > 0)
                    {
                        cmd.CommandText = sql0;
                        cmd.CommandTimeout = 0;
                        cmd.ExecuteNonQuery();
                    }
                    //dtED0 = setProData(Pro0day, Pro0zhi, dtED0, mon, partsno, dock, CarType, num, user);
                    //scb = new SqlCommandBuilder(apt);
                    //apt.Update(dtED0);
                    //20180917获取工程0的A/B班值 - 李兴旺

                    FS1203_DataAccess fS0000_DataAccess = new FS1203_DataAccess();
                    string vcAB00 = fS0000_DataAccess.getABClass(Pro0day, Pro0zhi, plant, dtCalendar.Rows[0]["vcCalendar0"].ToString().Trim());

                    //工程1
                    DataTable dtED1 = getEDProplan(cmd, ref apt, "EDMonthProdPlanTbl", mon);
                    string Pro1day = dt_print.Rows[i]["vcPro1Day"].ToString().Trim();
                    string Pro1zhi = dt_print.Rows[i]["vcPro1Zhi"].ToString().Trim();
                    string sql1 = setProData2(Pro1day, Pro1zhi, dtED1, mon, partsno, dock, CarType, num, user, "EDMonthProdPlanTbl");
                    if (sql1.Trim().Length > 0)
                    {
                        cmd.CommandText = sql1;
                        cmd.CommandTimeout = 0;
                        cmd.ExecuteNonQuery();
                    }
                    //dtED1 = setProData(Pro1day, Pro1zhi, dtED1, mon, partsno, dock, CarType, num, user);
                    //scb = new SqlCommandBuilder(apt);
                    //apt.Update(dtED1);
                    //20180920获取工程1的A/B班值 - 李兴旺
                    string vcAB01 = fS0000_DataAccess.getABClass(Pro1day, Pro1zhi, plant, dtCalendar.Rows[0]["vcCalendar1"].ToString().Trim());

                    //工程2
                    DataTable dtED2 = getEDProplan(cmd, ref apt, "EDMonthTZPlanTbl", mon);
                    string Pro2day = dt_print.Rows[i]["vcPro2Day"].ToString().Trim();
                    string Pro2zhi = dt_print.Rows[i]["vcPro2Zhi"].ToString().Trim();
                    string sql2 = setProData2(Pro2day, Pro2zhi, dtED2, mon, partsno, dock, CarType, num, user, "EDMonthTZPlanTbl");
                    if (sql2.Trim().Length > 0)
                    {
                        cmd.CommandText = sql2;
                        cmd.CommandTimeout = 0;
                        cmd.ExecuteNonQuery();
                    }
                    //dtED2 = setProData(Pro2day, Pro2zhi, dtED2, mon, partsno, dock, CarType, num, user);
                    //scb = new SqlCommandBuilder(apt);
                    //apt.Update(dtED2);
                    //20180920获取工程2的A/B班值 - 李兴旺
                    //20180920工程2（涂装）可能为空，需要空值判断 - 李兴旺
                    string vcAB02 = string.Empty;
                    if (Pro2day != string.Empty && Pro2zhi != string.Empty && dtCalendar.Rows[0]["vcCalendar2"].ToString().Trim() != string.Empty)
                    {
                        vcAB02 = fS0000_DataAccess.getABClass(Pro2day, Pro2zhi, plant, dtCalendar.Rows[0]["vcCalendar2"].ToString().Trim());
                    }

                    //工程3
                    DataTable dtED3 = getEDProplan(cmd, ref apt, "EDMonthP3PlanTbl", mon);
                    string Pro3day = dt_print.Rows[i]["vcPro3Day"].ToString().Trim();
                    string Pro3zhi = dt_print.Rows[i]["vcPro3Zhi"].ToString().Trim();
                    string sql3 = setProData2(Pro3day, Pro3zhi, dtED3, mon, partsno, dock, CarType, num, user, "EDMonthP3PlanTbl");
                    if (sql3.Trim().Length > 0)
                    {
                        cmd.CommandText = sql3;
                        cmd.CommandTimeout = 0;
                        cmd.ExecuteNonQuery();
                    }

                    //20180920获取工程3的A/B班值 - 李兴旺
                    //20180920工程3为预留工程，需要空值判断 - 李兴旺
                    string vcAB03 = string.Empty;
                    if (Pro3day != string.Empty && Pro3zhi != string.Empty && dtCalendar.Rows[0]["vcCalendar3"].ToString().Trim() != string.Empty)
                    {
                        vcAB03 = fS0000_DataAccess.getABClass(Pro3day, Pro3zhi, plant, dtCalendar.Rows[0]["vcCalendar3"].ToString().Trim());
                    }

                    //工程4
                    DataTable dtED4 = getEDProplan(cmd, ref apt, "EDMonthPackPlanTbl", mon);
                    string Pro4day = dt_print.Rows[i]["vcPro4Day"].ToString().Trim();
                    string Pro4zhi = dt_print.Rows[i]["vcPro4Zhi"].ToString().Trim();
                    string sql4 = setProData2(Pro4day, Pro4zhi, dtED4, mon, partsno, dock, CarType, num, user, "EDMonthPackPlanTbl");
                    if (sql4.Trim().Length > 0)
                    {
                        cmd.CommandText = sql4;
                        cmd.CommandTimeout = 0;
                        cmd.ExecuteNonQuery();
                    }

                    //20180920获取工程4的A/B班值 - 李兴旺
                    string vcAB04 = fS0000_DataAccess.getABClass(Pro4day, Pro4zhi, plant, dtCalendar.Rows[0]["vcCalendar4"].ToString().Trim());

                    //生成订单号和连番              
                    string orderID = dt_print.Rows[i]["vcOrderNo"].ToString().Trim();

                    num = num / Convert.ToInt32(srs);

                    //生成内制看板打印信息
                    DataRow dr_Rules = dt_print.Rows[i];
                    for (int k = 0; k < num; k++)
                    {
                        DataRow dr = dtupdate.NewRow();
                        dr["vcPartsNo"] = partsno;
                        dr["vcDock"] = dock;
                        dr["vcCarType"] = CarType;
                        dr["vcEDflag"] = "E";
                        dr["vcKBorderno"] = orderID;
                        dr["vcProject00"] = dr_Rules["vcProName0"].ToString();
                        dr["vcProject01"] = dr_Rules["vcProName1"].ToString();
                        dr["vcProject02"] = dr_Rules["vcProName2"].ToString();
                        dr["vcProject03"] = dr_Rules["vcProName3"].ToString();
                        dr["vcProject04"] = dr_Rules["vcProName4"].ToString();
                        dr["vcComDate00"] = Pro0day;
                        dr["vcComDate01"] = Pro1day;
                        dr["vcComDate02"] = Pro2day;
                        dr["vcComDate03"] = Pro3day;
                        dr["vcComDate04"] = Pro4day;
                        dr["vcBanZhi00"] = Pro0zhi;
                        dr["vcBanZhi01"] = Pro1zhi;
                        dr["vcBanZhi02"] = Pro2zhi;
                        dr["vcBanZhi03"] = Pro3zhi;
                        dr["vcBanZhi04"] = Pro4zhi;
                        dr["vcAB00"] = vcAB00;//20180920紧急订单增加AB值 - 李兴旺
                        dr["vcAB01"] = vcAB01;//20180920紧急订单增加AB值 - 李兴旺
                        dr["vcAB02"] = vcAB02;//20180920紧急订单增加AB值 - 李兴旺
                        dr["vcAB03"] = vcAB03;//20180920紧急订单增加AB值 - 李兴旺
                        dr["vcAB04"] = vcAB04;//20180920紧急订单增加AB值 - 李兴旺
                        dr["vcQuantityPerContainer"] = srs;
                        dr["vcCreater"] = user;
                        dr["vcPlanMonth"] = mon;
                        dr["bushu"] = dr_Rules["vcPorType"].ToString();
                        dr["dayin"] = Pro0day + Pro0zhi;
                        dr["shengchan"] = Pro1day + Pro1zhi;
                        dtupdate.Rows.Add(dr);

                    }
                }
                var query = from t in dtupdate.AsEnumerable()
                            group t by new
                            {
                                t1 = t.Field<string>("vcKBorderno"),
                                t2 = t.Field<string>("bushu"),
                                t3 = t.Field<string>("dayin"),
                                t4 = t.Field<string>("shengchan")
                            }
                                into m
                            select m;

                for (int j = 0; j < query.Count(); j++)
                {
                    DataTable dt = query.ElementAt(j).CopyToDataTable();
                    DataView dv = dt.DefaultView;
                    dv.Sort = "vcProject01";//按工位排序
                    dt = dv.ToTable();
                    //找到打印表中的紧急订单，相同订单号相同部署相同打印日期生产日期的最大连番
                    string tmpsql = "select MAX(vcKBSerial) as serial from tKanbanPrintTbl t1 left join tPartInfoMaster t2 on t1.vcPartsNo=t2.vcPartsNo and t1.vcDock=t2.vcDock where vcEDflag='E' and  ";
                    tmpsql += " vcKBorderno='" + dt.Rows[0]["vcKBorderno"].ToString() + "' and vcPorType='" + dt.Rows[0]["bushu"].ToString() + "'";
                    tmpsql += " and vcComDate00='" + dt.Rows[0]["vcComDate00"].ToString() + "' and vcBanZhi00='" + dt.Rows[0]["vcBanZhi00"].ToString() + "'";
                    tmpsql += " and vcComDate01='" + dt.Rows[0]["vcComDate01"].ToString() + "' and vcBanZhi01='" + dt.Rows[0]["vcBanZhi01"].ToString() + "'";
                    cmd.CommandText = tmpsql;
                    cmd.CommandType = CommandType.Text;
                    DataTable dt_serial = new DataTable();
                    apt = new SqlDataAdapter(cmd);
                    apt.Fill(dt_serial);
                    string serial = "0000";
                    if (dt_serial.Rows.Count == 0)
                    {
                        serial = "0000";
                    }
                    else
                    {
                        if (dt_serial.Rows[0][0].ToString() != "")
                        {
                            serial = (Convert.ToInt32(dt_serial.Rows[0][0])).ToString("0000");
                        }
                        else serial = "0000";
                    }
                    //if (dtserial.Rows.Count > 0)
                    //{
                    //    serial = Convert.ToInt32(dtserial.Rows[0]["serial"]).ToString("0000");
                    //}
                    for (int k = 0; k < dt.Rows.Count; k++)
                    {
                        serial = (Convert.ToInt32(serial) + 1).ToString("0000");
                        dt.Rows[k]["vcKBSerial"] = serial;
                    }
                    //更新到打印表
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "InsertEDKanBanPrint";
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@vcPartsNo", SqlDbType.VarChar, 12, "vcPartsNo");
                    cmd.Parameters.Add("@vcDock", SqlDbType.VarChar, 2, "vcDock");
                    cmd.Parameters.Add("@vcCarType", SqlDbType.VarChar, 4, "vcCarType");
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
                    cmd.Parameters.Add("@vcAB00", SqlDbType.VarChar, 1, "vcAB00");//20180920紧急订单增加AB值 - 李兴旺
                    cmd.Parameters.Add("@vcAB01", SqlDbType.VarChar, 1, "vcAB01");//20180920紧急订单增加AB值 - 李兴旺
                    cmd.Parameters.Add("@vcAB02", SqlDbType.VarChar, 1, "vcAB02");//20180920紧急订单增加AB值 - 李兴旺
                    cmd.Parameters.Add("@vcAB03", SqlDbType.VarChar, 1, "vcAB03");//20180920紧急订单增加AB值 - 李兴旺
                    cmd.Parameters.Add("@vcAB04", SqlDbType.VarChar, 1, "vcAB04");//20180920紧急订单增加AB值 - 李兴旺
                    cmd.Parameters.Add("@vcCreater", SqlDbType.VarChar, 10, "vcCreater");
                    cmd.Parameters.Add("@vcPlanMonth", SqlDbType.VarChar, 7, "vcPlanMonth");
                    cmd.Parameters.Add("@vcQuantityPerContainer", SqlDbType.VarChar, 8, "vcQuantityPerContainer");
                    apt = new SqlDataAdapter();
                    apt.InsertCommand = cmd;
                    dt.Columns.Remove("bushu");
                    dt.Columns.Remove("dayin");
                    dt.Columns.Remove("shengchan");
                    apt.Update(dt);
                }
                //优化测试

                //将更新后的条目的updateFlag 设置成1
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = " update EDMonthPlanTMP set UpdateFlag='1', vcUpdateID='" + user + "', dUpdateTime=getdate() where vcMonth='" + mon + "' ";
                cmd.CommandText += " and (select COUNT(1) from tPlanPartInfo where tPlanPartInfo.vcMonth='" + mon + "' and tPlanPartInfo.vcPartsno=EDMonthPlanTMP.vcPartsno and tPlanPartInfo.vcDock=EDMonthPlanTMP.vcDock ";
                //cmd.CommandText += " and tPlanPartInfo.vcCarType = EDMonthPlanTMP.vcCarType and vcPlant ='" + plant + "' ) >0 ";
                cmd.CommandText += " and tPlanPartInfo.vcCarType=EDMonthPlanTMP.vcCarType)>0 ";
                cmd.ExecuteNonQuery();
                cmd.Transaction.Commit();
                cmd.Connection.Close();
            }
            catch (Exception e)
            {
                msg = "更新失败！";
                if (cmd.Connection.State == ConnectionState.Open)
                {
                    cmd.Transaction.Rollback();
                    cmd.Connection.Close();
                }
                throw e;
            }
            return msg;
        }

        //20180917根据品番和受入确定该品番厂区 - 李兴旺
        public string getPartsPlant(string strPartsNo, string strDock, string strMonth)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(ComConnectionHelper.GetConnectionString());
            cmd.Connection.Open();
            cmd.Transaction = cmd.Connection.BeginTransaction();
            DataTable dt = new DataTable();
            string strSQL = "SELECT vcPartPlant FROM tPartInfoMaster where vcPartsNo='" + strPartsNo + "' and vcDock='" + strDock + "' and dTimeFrom<='" + strMonth + "-01' and dTimeTo>='" + strMonth + "-01'";
            cmd.CommandText = strSQL;
            SqlDataAdapter apt = new SqlDataAdapter(cmd);
            apt.Fill(dt);
            string strPlant = dt.Rows[0]["vcPartPlant"].ToString().Trim();
            return strPlant;
        }

        //20180920根据品番和它的受入、对象月确定它的5个工程的vcCalendar - 李兴旺
        public DataTable getCalendar(string strSQL)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(ComConnectionHelper.GetConnectionString());
            cmd.Connection.Open();
            cmd.Transaction = cmd.Connection.BeginTransaction();
            DataTable dt = new DataTable();
            cmd.CommandText = strSQL;
            SqlDataAdapter apt = new SqlDataAdapter(cmd);
            apt.Fill(dt);
            return dt;
        }

        public DataTable getInsertKB()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("vcPartsNo");
            dt.Columns.Add("vcDock");
            dt.Columns.Add("vcCarType");
            dt.Columns.Add("vcEDflag");
            dt.Columns.Add("vcKBorderno");
            dt.Columns.Add("vcKBSerial");
            dt.Columns.Add("vcProject00");
            dt.Columns.Add("vcProject01");
            dt.Columns.Add("vcProject02");
            dt.Columns.Add("vcProject03");
            dt.Columns.Add("vcProject04");
            dt.Columns.Add("vcComDate00");
            dt.Columns.Add("vcComDate01");
            dt.Columns.Add("vcComDate02");
            dt.Columns.Add("vcComDate03");
            dt.Columns.Add("vcComDate04");
            dt.Columns.Add("vcBanZhi00");
            dt.Columns.Add("vcBanZhi01");
            dt.Columns.Add("vcBanZhi02");
            dt.Columns.Add("vcBanZhi03");
            dt.Columns.Add("vcBanZhi04");
            dt.Columns.Add("vcAB00");//20180917紧急订单增加AB值 - 李兴旺
            dt.Columns.Add("vcAB01");//20180917紧急订单增加AB值 - 李兴旺
            dt.Columns.Add("vcAB02");//20180917紧急订单增加AB值 - 李兴旺
            dt.Columns.Add("vcAB03");//20180917紧急订单增加AB值 - 李兴旺
            dt.Columns.Add("vcAB04");//20180917紧急订单增加AB值 - 李兴旺
            dt.Columns.Add("vcQuantityPerContainer");
            dt.Columns.Add("vcCreater");
            dt.Columns.Add("vcPlanMonth");
            dt.Columns.Add("bushu");//部署
            dt.Columns.Add("dayin");//打印
            dt.Columns.Add("shengchan");//生产
            return dt;
        }

        public string setProData2(string Pro0day, string Pro0zhi, DataTable dtED0, string mon, string partsno, string dock, string CarType, int num, string user, string TableName)
        {
            string ssql = "";
            if (Pro0day.Length > 0 && Pro0zhi.Length > 0)
            {
                string a = Pro0zhi == "0" ? "b" : "y";
                string month = Pro0day.Split('-')[0] + "-" + Pro0day.Split('-')[1];
                string day = Convert.ToInt32(Pro0day.Split('-')[2]).ToString();
                string col = "vcD" + day + a;
                object montouch;
                if (month == mon)
                {
                    montouch = DBNull.Value;
                }
                else
                {
                    montouch = mon;
                }
                DataRow[] dr = dtED0.Select("vcMonth='" + month + "' and vcPartsno='" + partsno + "' and vcDock ='" + dock + "' and vcCarType ='" + CarType + "'");
                if (dr.Length == 1)
                {
                    int tmp = Convert.ToInt32(dr[0][col].ToString().Length == 0 ? "0" : dr[0][col].ToString());
                    if (montouch.ToString().Length > 0)
                    {
                        ssql = " update " + TableName + " set " + col + "='" + (tmp + num) + "' , DUPDTIME =getdate(), CUPDUSER ='" + user + "' where vcMonth ='" + month + "' and  vcDock ='" + dock + "' and vcCarType ='" + CarType + "' and vcPartsno ='" + partsno + "' and montouch = '" + montouch + "'  ";
                    }
                    else
                    {
                        ssql = " update " + TableName + " set " + col + "='" + (tmp + num) + "' , DUPDTIME =getdate(), CUPDUSER ='" + user + "' where vcMonth ='" + month + "' and  vcDock ='" + dock + "' and vcCarType ='" + CarType + "' and vcPartsno ='" + partsno + "' and montouch is null ";
                    }
                }
                else
                {
                    if (montouch.ToString().Length > 0)
                    {
                        ssql = "insert into " + TableName + "(vcMonth,vcPartsno,vcDock,vcCarType," + col + ",montouch,DADDTIME,CUPDUSER) values('" + month + "','" + partsno + "','" + dock + "','" + CarType + "','" + num + "','" + montouch + "','" + DateTime.Now + "','" + user + "')";
                    }
                    else ssql = "insert into " + TableName + "(vcMonth,vcPartsno,vcDock,vcCarType," + col + ",DADDTIME,CUPDUSER) values('" + month + "','" + partsno + "','" + dock + "','" + CarType + "','" + num + "','" + DateTime.Now + "','" + user + "')";
                }
            }
            return ssql;
        }

        public DataTable getEDProplan(SqlCommand cmd, ref SqlDataAdapter apt, string TableName, string mon)
        {
            DataTable dt = new DataTable();
            cmd.CommandText = " select * from " + TableName + " where (vcMonth='" + mon + "' and montouch is null) or montouch='" + mon + "' ";
            apt.Fill(dt);
            return dt;
        }

        public DataTable getSearch(string mon, string plant, string TblName, ref Exception e)
        {
            DataTable dt = new DataTable();
            string tmpT = "";
            string tmpE = "";
            for (int i = 1; i < 31 + 1; i++)
            {
                if (i == 31)
                    tmpE += "t2.vcD" + i + "b as	ED" + i + "b,	t2.	vcD" + i + "y	as	ED" + i + "y";
                else tmpE += "t2.vcD" + i + "b	as	ED" + i + "b,	t2.vcD" + i + "y	as	ED" + i + "y,";
            }
            for (int i = 1; i < 31 + 1; i++)
            {
                if (i == 31)
                    tmpT += "t1.vcD" + i + "b 	as	TD" + i + "b,	t1.vcD" + i + "y	as	TD" + i + "y";
                else tmpT += "t1.vcD" + i + "b 	as	TD" + i + "b,	t1.vcD" + i + "y	as	TD" + i + "y,";
            }
            StringBuilder sb = new StringBuilder();
            //sb.AppendFormat(" select '{0}' as vcMonth ,case when t2.vcPartsno is null then SUBSTRING(t1.vcPartsno,0,6)+'-'+SUBSTRING(t1.vcPartsno,6,5)+'-'+SUBSTRING(t1.vcPartsno,11,2) else SUBSTRING(t2.vcPartsno,0,6)+'-'+SUBSTRING(t2.vcPartsno,7,5)+'-'+SUBSTRING(t2.vcPartsno,11,2) end as vcPartsno ", mon);
            //sb.AppendLine(" ,case when t2.vcDock is null then t1.vcDock else t2.vcDock end as vcDock");
            //sb.AppendLine(" ,case when t2.vcCarType is null then t1.vcCarType else t2.vcCarType end as vcCarType");
            //sb.AppendLine("  ,t4.vcCalendar1,t4.vcCalendar2,t4.vcCalendar3,t4.vcCalendar4,");
            //sb.AppendLine(" ");
            //sb.AppendLine("  t3.vcPartsNameCHN, t3.vcCurrentPastCode ,'0' as vcMonTotal,");
            //sb.AppendFormat(" {0},", tmpT);
            //sb.AppendFormat(" {0}", tmpE);
            //sb.AppendFormat("   from (select * from  {0} where montouch is not null ) t1  ", TblName);
            //sb.AppendFormat("    full join (select * from  {0} where montouch is null ) t2", TblName);
            //sb.AppendLine("     on t1.montouch = t2.vcMonth and t1.vcPartsno=t2.vcPartsno and t1.vcDock=t2.vcDock and t1.vcCarType=t2.vcCarType");
            //sb.AppendLine("   left join dbo.tPartInfoMaster t3");
            //sb.AppendLine("   on (t3.vcPartsNo=t2.vcPartsNo or t3.vcPartsNo = t1.vcPartsno)and (t3.vcDock = t2.vcDock or t3.vcDock = t1.vcDock) and (t3.vcCarFamilyCode = t2.vcCarType or t3.vcCarFamilyCode = t1.vcCarType)");
            //sb.AppendLine("   left join dbo.ProRuleMst t4");
            //sb.AppendLine(" on t4.vcPorType = t3.vcPorType and t4.vcZB = t3.vcZB");
            //sb.AppendFormat(" where t1.montouch = '{0}' or t2.vcMonth ='{1}'", mon, mon);

            sb.AppendLine("   select tt1.vcMonth,t5.vcData2 as vcPlant, SUBSTRING(tt1.vcPartsno,0,6)+'-'+SUBSTRING(tt1.vcPartsno,6,5)+'-'+SUBSTRING(tt1.vcPartsno,11,2) as vcPartsno,");
            sb.AppendLine("   tt1.vcDock,tt1.vcCarType ,t4.vcCalendar1,t4.vcCalendar2,t4.vcCalendar3,t4.vcCalendar4,t3.vcPartNameCN as vcPartsNameCHN, t3.vcHJ as vcCurrentPastCode ,tt1.vcMonTotal ,");
            sb.AppendLine("	TD1b	 as TD1b 	, TD1y	 as TD1y ,");
            sb.AppendLine("	TD2b	 as TD2b	, TD2y	 as TD2y ,");
            sb.AppendLine("	TD3b	 as TD3b	, TD3y	 as TD3y ,");
            sb.AppendLine("	TD4b	 as TD4b	, TD4y	 as TD4y ,");
            sb.AppendLine("	TD5b	 as TD5b	, TD5y	 as TD5y ,");
            sb.AppendLine("	TD6b	 as TD6b	, TD6y	 as TD6y ,");
            sb.AppendLine("	TD7b	 as TD7b	, TD7y	 as TD7y ,");
            sb.AppendLine("	TD8b	 as TD8b	, TD8y	 as TD8y ,");
            sb.AppendLine("	TD9b	 as TD9b	, TD9y	 as TD9y ,");
            sb.AppendLine("	TD10b	 as TD10b	, TD10y	 as TD10y ,");
            sb.AppendLine("	TD11b	 as TD11b	, TD11y	 as TD11y ,");
            sb.AppendLine("	TD12b	 as TD12b	, TD12y	 as TD12y ,");
            sb.AppendLine("	TD13b	 as TD13b	, TD13y	 as TD13y ,");
            sb.AppendLine("	TD14b	 as TD14b	, TD14y	 as TD14y ,");
            sb.AppendLine("	TD15b	 as TD15b	, TD15y	 as TD15y ,");
            sb.AppendLine("	TD16b	 as TD16b	, TD16y	 as TD16y ,");
            sb.AppendLine("	TD17b	 as TD17b	, TD17y	 as TD17y ,");
            sb.AppendLine("	TD18b	 as TD18b	, TD18y	 as TD18y ,");
            sb.AppendLine("	TD19b	 as TD19b	, TD19y	 as TD19y ,");
            sb.AppendLine("	TD20b	 as TD20b	, TD20y	 as TD20y ,");
            sb.AppendLine("	TD21b	 as TD21b	, TD21y	 as TD21y ,");
            sb.AppendLine("	TD22b	 as TD22b	, TD22y	 as TD22y ,");
            sb.AppendLine("	TD23b	 as TD23b	, TD23y	 as TD23y ,");
            sb.AppendLine("	TD24b	 as TD24b	, TD24y	 as TD24y ,");
            sb.AppendLine("	TD25b	 as TD25b	, TD25y	 as TD25y ,");
            sb.AppendLine("	TD26b	 as TD26b	, TD26y	 as TD26y ,");
            sb.AppendLine("	TD27b	 as TD27b	, TD27y	 as TD27y ,");
            sb.AppendLine("	TD28b	 as TD28b	, TD28y	 as TD28y ,");
            sb.AppendLine("	TD29b	 as TD29b	, TD29y	 as TD29y ,");
            sb.AppendLine("	TD30b	 as TD30b	, TD30y	 as TD30y ,");
            sb.AppendLine("	TD31b	 as TD31b	, TD31y	 as TD31y ,");

            sb.AppendLine("	ED1b	 as ED1b 	, ED1y	 as ED1y ,");
            sb.AppendLine("	ED2b	 as ED2b	, ED2y	 as ED2y ,");
            sb.AppendLine("	ED3b	 as ED3b	, ED3y	 as ED3y ,");
            sb.AppendLine("	ED4b	 as ED4b	, ED4y	 as ED4y ,");
            sb.AppendLine("	ED5b	 as ED5b	, ED5y	 as ED5y ,");
            sb.AppendLine("	ED6b	 as ED6b	, ED6y	 as ED6y ,");
            sb.AppendLine("	ED7b	 as ED7b	, ED7y	 as ED7y ,");
            sb.AppendLine("	ED8b	 as ED8b	, ED8y	 as ED8y ,");
            sb.AppendLine("	ED9b	 as ED9b	, ED9y	 as ED9y ,");
            sb.AppendLine("	ED10b	 as ED10b	, ED10y	 as ED10y ,");
            sb.AppendLine("	ED11b	 as ED11b	, ED11y	 as ED11y ,");
            sb.AppendLine("	ED12b	 as ED12b	, ED12y	 as ED12y ,");
            sb.AppendLine("	ED13b	 as ED13b	, ED13y	 as ED13y ,");
            sb.AppendLine("	ED14b	 as ED14b	, ED14y	 as ED14y ,");
            sb.AppendLine("	ED15b	 as ED15b	, ED15y	 as ED15y ,");
            sb.AppendLine("	ED16b	 as ED16b	, ED16y	 as ED16y ,");
            sb.AppendLine("	ED17b	 as ED17b	, ED17y	 as ED17y ,");
            sb.AppendLine("	ED18b	 as ED18b	, ED18y	 as ED18y ,");
            sb.AppendLine("	ED19b	 as ED19b	, ED19y	 as ED19y ,");
            sb.AppendLine("	ED20b	 as ED20b	, ED20y	 as ED20y ,");
            sb.AppendLine("	ED21b	 as ED21b	, ED21y	 as ED21y ,");
            sb.AppendLine("	ED22b	 as ED22b	, ED22y	 as ED22y ,");
            sb.AppendLine("	ED23b	 as ED23b	, ED23y	 as ED23y ,");
            sb.AppendLine("	ED24b	 as ED24b	, ED24y	 as ED24y ,");
            sb.AppendLine("	ED25b	 as ED25b	, ED25y	 as ED25y ,");
            sb.AppendLine("	ED26b	 as ED26b	, ED26y	 as ED26y ,");
            sb.AppendLine("	ED27b	 as ED27b	, ED27y	 as ED27y ,");
            sb.AppendLine("	ED28b	 as ED28b	, ED28y	 as ED28y ,");
            sb.AppendLine("	ED29b	 as ED29b	, ED29y	 as ED29y ,");
            sb.AppendLine("	ED30b	 as ED30b	, ED30y	 as ED30y ,");
            sb.AppendLine("	ED31b	 as ED31b	, ED31y	 as ED31y ");

            sb.AppendLine("  ");
            sb.AppendLine("   from (");
            sb.AppendFormat("    select '{0}' as vcMonth ,", mon);
            sb.AppendLine("   case when t2.vcPartsno is null then t1.vcPartsno  else t2.vcPartsno end as vcPartsno,");
            sb.AppendLine("    case when t2.vcDock is null then t1.vcDock else t2.vcDock end as vcDock");
            sb.AppendLine("   ,case when t2.vcCarType is null then t1.vcCarType else t2.vcCarType end as vcCarType");
            sb.AppendLine("  ,'0' as vcMonTotal,");
            sb.AppendFormat(" {0},", tmpT);
            sb.AppendFormat(" {0}", tmpE);
            sb.AppendFormat("   from (select * from {0} where montouch is not null) t1  ", TblName);
            sb.AppendFormat("    full join (select * from {0} where montouch is null ) t2", TblName);
            sb.AppendLine("    on t1.montouch = t2.vcMonth and t1.vcPartsno=t2.vcPartsno and t1.vcDock=t2.vcDock and t1.vcCarType=t2.vcCarType");
            sb.AppendFormat("     where (t1.montouch = '{0}' or (t2.vcMonth ='{1}' and t2.montouch is null))", mon, mon);
            sb.AppendLine("    ) tt1");
            sb.AppendFormat("   left join (select distinct vcProType,vcZB,vcPlant,vcEDFlag,vcPartsNo,vcDock,vcCarType,vcPartNameCN,vcHJ from tPlanPartInfo where vcEDFlag ='E' and vcMonth ='{0}' ) t3", mon);
            sb.AppendLine("     on t3.vcPartsNo=tt1.vcPartsNo and t3.vcDock = tt1.vcDock and t3.vcCarType = tt1.vcCarType ");
            sb.AppendLine("     left join ProRuleMst t4");
            sb.AppendLine("   on t4.vcPorType = t3.vcProType and t4.vcZB = t3.vcZB");
            sb.AppendLine(" left join (select vcData1 ,vcData2  from ConstMst where vcDataId='kbplant') t5");
            sb.AppendLine(" on t3.vcPlant = t5.vcData1 ");
            sb.AppendFormat(" where t3.vcPlant='{0}' and t3.vcEDFlag='E'", plant);
            try
            {
                dt = excute.ExcuteSqlWithSelectToDT(sb.ToString());
            }
            catch (Exception ex)
            {
                e = ex;
                dt = null;
                return dt;
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int a = 0;
                for (int j = 11; j < dt.Columns.Count; j++)
                {
                    a += Convert.ToInt32(dt.Rows[i][j].ToString().Length == 0 ? "0" : dt.Rows[i][j].ToString());
                }
                dt.Rows[i]["vcMonTotal"] = a;
            }
            return dt;
        }

        public DataTable getPartsInfo()
        {
            string sql = " select * from tPartInfoMaster ";
            return excute.ExcuteSqlWithSelectToDT(sql);
        }

        public string UpdateEDPlan(DataTable dt, string user)
        {
            string msg = "";
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(ComConnectionHelper.GetConnectionString());
            cmd.Connection.Open();
            cmd.Transaction = cmd.Connection.BeginTransaction();
            try
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i][0].ToString().Trim().Length == 0)
                    {
                        break;
                    }
                    string month = dt.Rows[i][0].ToString().Trim();
                    string parts = dt.Rows[i][1].ToString().Replace("-", "").Trim();
                    string dock = dt.Rows[i][2].ToString().Trim();
                    string cartype = dt.Rows[i][3].ToString().Trim();
                    int num = Convert.ToInt32(dt.Rows[i][4]);
                    string calendar0 = Convert.ToDateTime(dt.Rows[i][6].ToString()).ToString("yyyy-MM-dd");
                    string zhi0 = dt.Rows[i][7].ToString();
                    if (dt.Rows[i][7].ToString() == "白" || dt.Rows[i][7].ToString() == "白值")
                    {
                        zhi0 = "0";
                    }
                    else if (dt.Rows[i][7].ToString() == "夜" || dt.Rows[i][7].ToString() == "夜值")
                    {
                        zhi0 = "1";
                    }
                    else
                    {
                        zhi0 = "";
                    }

                    string calendar1 = Convert.ToDateTime(dt.Rows[i][8].ToString()).ToString("yyyy-MM-dd");
                    string zhi1 = dt.Rows[i][9].ToString();
                    if (dt.Rows[i][9].ToString() == "白" || dt.Rows[i][9].ToString() == "白值")
                    {
                        zhi1 = "0";
                    }
                    else if (dt.Rows[i][9].ToString() == "夜" || dt.Rows[i][9].ToString() == "夜值")
                    {
                        zhi1 = "1";
                    }
                    else
                    {
                        zhi1 = "";
                    }

                    string calendar2 = dt.Rows[i][10].ToString().Trim() == "" ? "" : Convert.ToDateTime(dt.Rows[i][10].ToString()).ToString("yyyy-MM-dd");
                    string zhi2 = dt.Rows[i][11].ToString();
                    if (dt.Rows[i][11].ToString() == "白" || dt.Rows[i][11].ToString() == "白值")
                    {
                        zhi2 = "0";
                    }
                    else if (dt.Rows[i][11].ToString() == "夜" || dt.Rows[i][11].ToString() == "夜值")
                    {
                        zhi2 = "1";
                    }
                    else
                    {
                        zhi2 = "";
                    }

                    string calendar3 = dt.Rows[i][12].ToString().Trim() == "" ? "" : Convert.ToDateTime(dt.Rows[i][12].ToString()).ToString("yyyy-MM-dd");
                    string zhi3 = dt.Rows[i][13].ToString();
                    if (dt.Rows[i][13].ToString() == "白" || dt.Rows[i][13].ToString() == "白值")
                    {
                        zhi3 = "0";
                    }
                    else if (dt.Rows[i][13].ToString() == "夜" || dt.Rows[i][13].ToString() == "夜值")
                    {
                        zhi3 = "1";
                    }
                    else
                    {
                        zhi3 = "";
                    }

                    string calendar4 = Convert.ToDateTime(dt.Rows[i][14].ToString()).ToString("yyyy-MM-dd");
                    string zhi4 = dt.Rows[i][15].ToString();
                    if (dt.Rows[i][15].ToString() == "白" || dt.Rows[i][15].ToString() == "白值")
                    {
                        zhi4 = "0";
                    }
                    else if (dt.Rows[i][15].ToString() == "夜" || dt.Rows[i][15].ToString() == "夜值")
                    {
                        zhi4 = "1";
                    }
                    else
                    {
                        zhi4 = "";
                    }
                    string orderno = dt.Rows[i][5].ToString().Trim();
                    if (orderno.Length != 10 && orderno.Length != 12)
                    {
                        msg = "品番：" + parts + ",受入：" + dock + ",订单号位数不正确应为10位或12位。";
                        cmd.Transaction.Rollback();
                        cmd.Connection.Close();
                        return msg;
                    }
                    string sqlExist = " select * from EDMonthPlanTMP where vcOrderNo = '" + orderno + "'  and vcPartsno='" + parts + "' and vcDock ='" + dock + "' ";
                    DataTable dtExist = new DataTable();
                    cmd.CommandText = sqlExist;
                    SqlDataAdapter apt = new SqlDataAdapter(cmd);
                    apt.Fill(dtExist);
                    if (dtExist.Rows.Count > 0)
                    {
                        msg = "品番：" + parts + ",受入：" + dock + ",订单号：" + orderno + "已经存在。";
                        cmd.Transaction.Rollback();
                        cmd.Connection.Close();
                        return msg;
                    }
                    dtExist = new DataTable();
                    string ssql = "select iQuantityPerContainer as srs , vcPartPlant,vcPartsNameCHN,vcCurrentPastCode,vcPorType,vcZB,vcQFflag from tPartInfoMaster where vcPartsNo ='" + parts + "' and vcDock ='" + dock + "'  and dTimeFrom<= '" + month + "-01" + "' and dTimeTo >= '" + month + "-01" + "' ";

                    cmd.CommandText = ssql;
                    apt.Fill(dtExist);

                    if (dtExist.Rows.Count > 0)
                    {
                        if (num % Convert.ToInt32(dtExist.Rows[0]["srs"]) != 0)
                        {
                            msg = "品番：" + parts + ",受入：" + dock + ",订单号：" + orderno + "数量应为收容数的倍数。";
                            cmd.Transaction.Rollback();
                            cmd.Connection.Close();
                            return msg;
                        }
                    }
                    else
                    {
                        //msg = "品番：" + parts + ",受入：" + dock + ",工厂：" + plant + ", 在品番基础表中不存在。";
                        msg = "品番：" + parts + ",受入：" + dock + ", 在品番基础表中不存在。";
                        cmd.Transaction.Rollback();
                        cmd.Connection.Close();
                        return msg;
                    }

                    ssql = " INSERT INTO EDMonthPlanTMP  ";
                    ssql += "  (vcMonth,vcPlant,vcPartsno,vcDock,vcCarType,vcNum,vcOrderNo,vcPro0Day,vcPro0Zhi,vcPro1Day";
                    ssql += "  ,vcPro1Zhi,vcPro2Day,vcPro2Zhi,vcPro3Day,vcPro3Zhi,vcPro4Day";
                    ssql += "  ,vcPro4Zhi,vcCreaterID,dCreatTime)";
                    ssql += "  VALUES(  '" + month + "' ";
                    ssql += "  , '" + dtExist.Rows[0]["vcPartPlant"].ToString() + "' ";
                    ssql += "  , '" + parts + "' ";
                    ssql += "  , '" + dock + "' ";
                    ssql += "  , '" + cartype + "' ";
                    ssql += "  , '" + num + "' ";
                    ssql += "  , '" + orderno + "' ";
                    ssql += "  , '" + calendar0 + "' ";
                    ssql += "  , '" + zhi0 + "' ";
                    ssql += "  , '" + calendar1 + "' ";
                    ssql += "  , '" + zhi1 + "' ";
                    ssql += "  , '" + calendar2 + "' ";
                    ssql += "  , '" + zhi2 + "' ";
                    ssql += "  , '" + calendar3 + "' ";
                    ssql += "  , '" + zhi3 + "' ";
                    ssql += "  , '" + calendar4 + "' ";
                    ssql += "  , '" + zhi4 + "' ";
                    ssql += "  , '" + user + "' ";
                    ssql += "  , getdate() ";
                    ssql += "  )";
                    cmd.CommandText = ssql;
                    cmd.ExecuteNonQuery();
                    //updatePartMst(cmd, plant, month);

                    StringBuilder sb = new StringBuilder();
                    sb.Length = 0;
                    sb.AppendFormat(" select * from tPlanPartInfo where vcMonth ='{0}' ", month);
                    sb.AppendFormat(" and  vcPartsNo ='{0}'", parts);
                    sb.AppendFormat(" and  vcCarType ='{0}'", cartype);
                    sb.AppendFormat(" and  vcDock ='{0}'", dock);
                    sb.AppendLine(" and  vcEDFlag ='E'");
                    cmd.CommandText = sb.ToString();
                    apt = new SqlDataAdapter(cmd);
                    DataTable tmpdata = new DataTable();
                    apt.Fill(tmpdata);
                    if (tmpdata.Rows.Count == 0)
                    {
                        sb.Length = 0;
                        sb.AppendLine(" insert into tPlanPartInfo([vcMonth],[vcPartsNo],[vcCarType],[vcDock],[vcEDFlag],[vcPlant],[vcPartNameCN],[vcHJ],[vcProType],[vcZB],[vcSRS],[vcQFflag]) ");
                        sb.AppendFormat(" VALUES ('{0}'", month);
                        sb.AppendFormat(" ,'{0}' ", parts);
                        sb.AppendFormat(" ,'{0}' ", cartype);
                        sb.AppendFormat(" ,'{0}' ", dock);
                        sb.AppendLine(" ,'E' ");
                        sb.AppendFormat(" ,'{0}' ", dtExist.Rows[0]["vcPartPlant"].ToString());
                        sb.AppendFormat(" ,'{0}' ", dtExist.Rows[0]["vcPartsNameCHN"].ToString());
                        sb.AppendFormat(" ,'{0}' ", dtExist.Rows[0]["vcCurrentPastCode"].ToString());
                        sb.AppendFormat(" ,'{0}' ", dtExist.Rows[0]["vcPorType"].ToString());
                        sb.AppendFormat(" ,'{0}' ", dtExist.Rows[0]["vcZB"].ToString());
                        sb.AppendFormat(" ,'{0}' ", dtExist.Rows[0]["srs"].ToString());
                        sb.AppendFormat(" ,'{0}' )", dtExist.Rows[0]["vcQFflag"].ToString());
                        cmd.CommandText = sb.ToString();
                        cmd.ExecuteNonQuery();
                    }
                    updatePartMst2(cmd, month);
                }
                cmd.Transaction.Commit();
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                cmd.Transaction.Rollback();
                cmd.Connection.Close();
                throw ex;
            }
            return msg;
        }

        #region 删除
        public void Del_Order(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(" DELETE FROM EDMonthPlanTMP where iAutoId in(   \r\n ");
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
    }
}
