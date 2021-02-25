/*******************************************************************
* 	项目名称			:	TPCS								   	
* 	模块名称			:	纳入计划进度管理					
* 	创建者			    :	GAOLEI								
* 	创建日期			:	2020/09/12							
* 	类名			    :	FS1211_Logic					    
* 	修改者			    :						
* 	修改时间			:						
* 	修改内容			:											
* 					
* 	(C)2020-TJQM INFORMATION TECHNOLOGY CO.,LTD All Rights Reserved.
*******************************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using DataAccess;
using Common;
using System.Collections;
using System.Linq;

namespace Logic
{
    public class FS1211_Logic
    {
        FS1211_DataAccess dataAccess = new FS1211_DataAccess();
        /// <summary>
        /// 明细导出
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="exlName"></param>
        public string PartFileExport(DataTable dt, string exlName)
        {
            string msg = null;
            //try
            //{
            //    if (dt != null && dt.Rows.Count > 0)
            //    {
            //        QMCommon.OfficeCommon.QMExcel oQMExcel = new QMCommon.OfficeCommon.QMExcel();

            //        string tmplatePath = System.Web.HttpContext.Current.Server.MapPath("~/Templates/FS1211_PartList.xlt");
            //        string path = System.Web.HttpContext.Current.Server.MapPath("~/Temps/" + exlName);
            //        oQMExcel.ExportFromTemplate(dt, tmplatePath, path, 2, 1, true);

            //        dt.Dispose();
            //    }
            //    else
            //    {
            //        msg = "初始化状态没有数据，请进行检索！";
            //    }

            //}
            //catch (WebException ex)
            //{
            //    LogHelper.ErrorLog("纳入计划进度管理导出异常：" + ex.Message);
            //    msg = "导出失败！";
            //}
            return msg;
        }

        /// <summary>
        /// 计划导出
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="exlName"></param>
        public string FileExport(DataTable dt, string exlName, string type)
        {
            string msg = null;
            //try
            //{
            //    if (dt != null && dt.Rows.Count > 0)
            //    {
            //        QMCommon.OfficeCommon.QMExcel oQMExcel = new QMCommon.OfficeCommon.QMExcel();
            //        string templateName;
            //        if ("ALL".Equals(type))
            //        {
            //            templateName = "FS1211_ALLExport.xlt";
            //        }
            //        else
            //        {
            //            templateName = "FS1211_ItemExport.xlt";
            //        }
            //        string tmplatePath = System.Web.HttpContext.Current.Server.MapPath("~/Templates/" + templateName);
            //        string path = System.Web.HttpContext.Current.Server.MapPath("~/Temps/" + exlName);
            //        oQMExcel.ExportFromTemplate(dt, tmplatePath, path, 2, 1, true);

            //        dt.Dispose();
            //    }
            //    else
            //    {
            //        msg = "无导出数据,请确认条件!";
            //    }

            //}
            //catch (WebException ex)
            //{
            //    LogHelper.ErrorLog("纳入计划进度管理导出异常：" + ex.Message);
            //    msg = "导出失败！";
            //}
            return msg;
        }

        public DataTable plantsource()
        {
            return dataAccess.getPlantSource();
        }

        public DataTable protypesource()
        {
            return dataAccess.getProtypeSource();
        }

        public DataTable SearchData(string vcMon, string vcTF, string vcTO, string vcType, string vcPartsno, string vcDock, string vcPorType, string vcOrder, string plant, string ed)
        {
            DataTable dt = new DataTable();
            if (vcType == "0")
            {
                //--1 获取计划
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(ComConnectionHelper.GetConnectionString());
                cmd.CommandTimeout = 0;
                SqlDataAdapter apt = new SqlDataAdapter(cmd);
                DataTable dtNZ = new DataTable();
                //dtNZ = getMonProALL2(vcMon, "MonthPackPlanTbl", "EDMonthPackPlanTbl", cmd, apt);
                DataTable dtS = dataAccess.getMonPackPlanTMP(vcMon, "MonthPackPlanTbl", cmd, apt, plant);//通常计划
                if (vcPorType.Length > 0)
                {
                    dtS = dtS.Select("bushu ='" + vcPorType + "'").Length > 0 ? dtS.Select("bushu ='" + vcPorType + "'").CopyToDataTable() : dtS.Clone();
                }
                DataTable dtE = dataAccess.getMonPlanED(vcMon, "EDMonthPackPlanTbl", cmd, apt, plant);//紧急计划
                if (vcPorType.Length > 0)
                {
                    dtE = dtE.Select("bushu ='" + vcPorType + "'").Length > 0 ? dtE.Select("bushu ='" + vcPorType + "'").CopyToDataTable() : dtE.Clone();
                }
                int flag = dtS.Columns.Count;

                //--2 补给系统中opr 表中 vcMon 的数据
                DataTable dtEDOrder = dataAccess.getEDOrder(vcMon, cmd, apt);//获取紧急订单基础信息                
                DataSet ds = dataAccess.getOPRData(vcMon, vcTF, vcTO, vcPartsno, vcDock, dtEDOrder);//从补给获取入库数据
                DataTable dtOPR = ds.Tables[0];// 获取月度入库信息。
                DataTable dtED = ds.Tables[1]; // 获取紧急入库信息。

                //--3 合并
                #region 月度计划纳入计算
                dtNZ = dtS.Copy();
                DataColumn Actotal = new DataColumn();
                Actotal.ColumnName = "vcActTotal";
                Actotal.DataType = typeof(int);
                Actotal.DefaultValue = 0;
                dtNZ.Columns.Add(Actotal);
                for (int i = 1; i <= 31; i++)
                {
                    DataColumn d1 = new DataColumn();
                    d1.ColumnName = "TD" + i + "bA";
                    d1.DataType = typeof(int);
                    // d1.DefaultValue = 0;
                    dtNZ.Columns.Add(d1);
                    DataColumn d2 = new DataColumn();
                    d2.ColumnName = "TD" + i + "yA";
                    d2.DataType = typeof(int);
                    // d2.DefaultValue = 0;
                    dtNZ.Columns.Add(d2);
                }
                for (int i = 1; i <= 31; i++)
                {
                    DataColumn d1 = new DataColumn();
                    d1.ColumnName = "ED" + i + "bA";
                    d1.DataType = typeof(int);
                    // d1.DefaultValue = 0;
                    dtNZ.Columns.Add(d1);
                    DataColumn d2 = new DataColumn();
                    d2.ColumnName = "ED" + i + "yA";
                    d2.DataType = typeof(int);
                    // d2.DefaultValue = 0;
                    dtNZ.Columns.Add(d2);
                }
                for (int i = 0; i < dtOPR.Rows.Count; i++)//月度计划纳入计算
                {
                    string partsno = dtOPR.Rows[i]["partsno"].ToString();
                    string dock = dtOPR.Rows[i]["dock"].ToString();
                    string order = dtOPR.Rows[i]["kanbanorderno"].ToString().Trim();
                    string serial = dtOPR.Rows[i]["dock"].ToString();
                    string ED = "S";
                    string type = dtOPR.Rows[i]["Otype"].ToString();
                    //判断该订单号属于哪个月的计划
                    if (type == "T0")//内制
                    {
                        cmd.CommandText = " select vcPlanMonth from tKanbanPrintTbl where vcKBorderno ='" + order + "' and vcPartsNo='" + partsno + "' and vcDock ='" + dock + "'";
                    }
                    else if (type == "T1")//外注
                    {
                        cmd.CommandText = "   select top(1) vcPlanMonth from tKanbanPrintTbl t1";
                        cmd.CommandText += "  left join (select * from tPartInfoMaster where vcInOutFlag ='1') t2";
                        cmd.CommandText += "  on SUBSTRING(t1.vcPartsNo,0,11) = SUBSTRING(t2.vcPartsNo,0,11)";
                        cmd.CommandText += "  where t2.vcPartsNo='" + partsno + "' and t2.vcDock ='" + dock + "' and t1.vcKBorderno ='" + order + "' and t2.dTimeFrom<= '" + vcMon + "-01" + "' and t2.dTimeTo >= '" + vcMon + "-01" + "' ";
                    }
                    DataTable tmp = new DataTable();
                    apt.Fill(tmp);
                    if (tmp.Rows.Count == 0) continue;
                    else if (tmp.Rows.Count > 0 && tmp.Rows[0]["vcPlanMonth"].ToString().Trim() != vcMon)
                        continue;
                    else
                    {

                        string tmpMon = order.Substring(0, 6);
                        string tmpDay = Convert.ToInt32(order.Substring(6, 2)).ToString();
                        if (tmpMon == vcMon.Replace("-", ""))
                        {
                            tmpMon = "ED";
                        }
                        else
                        {
                            tmpMon = "TD";
                        }
                        string zhi = order.Substring(8, 2);
                        if (zhi == "01")
                        {
                            zhi = "b";
                        }
                        else
                        {
                            zhi = "y";
                        }
                        string colName = tmpMon + tmpDay + zhi + "A";
                        DataRow[] dr = dtNZ.Select("vcPartsNo ='" + dtOPR.Rows[i]["partsno"].ToString() + "' and vcDock='" + dtOPR.Rows[i]["dock"].ToString() + "' and EDflag ='" + ED + "' ");
                        if (dr.Length == 1)
                        {
                            dr[0][colName] = Convert.ToInt32(dr[0][colName].ToString().Length == 0 ? "0" : dr[0][colName].ToString()) + Convert.ToInt32(dtOPR.Rows[i]["num"]);
                            dr[0]["vcActTotal"] = Convert.ToInt32(dr[0]["vcActTotal"]) + Convert.ToInt32(dtOPR.Rows[i]["num"]);
                        }
                    }
                }
                dtS = dtNZ.Copy();
                #endregion

                #region 紧急计划纳入计算
                dtNZ = dtE.Copy();//紧急计划纳入计算。
                DataColumn col = new DataColumn();
                col.DataType = typeof(int);
                col.DefaultValue = 0;
                col.ColumnName = "vcActTotal";
                dtNZ.Columns.Add(col);
                for (int i = 1; i <= 31; i++)
                {
                    DataColumn d1 = new DataColumn();
                    d1.ColumnName = "TD" + i + "bA";
                    d1.DataType = typeof(int);
                    // d1.DefaultValue = 0;
                    dtNZ.Columns.Add(d1);
                    DataColumn d2 = new DataColumn();
                    d2.ColumnName = "TD" + i + "yA";
                    d2.DataType = typeof(int);
                    // d2.DefaultValue = 0;
                    dtNZ.Columns.Add(d2);
                }
                for (int i = 1; i <= 31; i++)
                {
                    DataColumn d1 = new DataColumn();
                    d1.ColumnName = "ED" + i + "bA";
                    d1.DataType = typeof(int);
                    // d1.DefaultValue = 0;
                    dtNZ.Columns.Add(d1);
                    DataColumn d2 = new DataColumn();
                    d2.ColumnName = "ED" + i + "yA";
                    d2.DataType = typeof(int);
                    // d2.DefaultValue = 0;
                    dtNZ.Columns.Add(d2);
                }
                for (int i = 0; i < dtED.Rows.Count; i++)
                {
                    string partsno = dtED.Rows[i]["partsno"].ToString();
                    string dock = dtED.Rows[i]["dock"].ToString();
                    string order = dtED.Rows[i]["kanbanorderno"].ToString().Trim();
                    string serial = dtED.Rows[i]["kanbanserial"].ToString();
                    string ED = "E";
                    DataTable dttmp = new DataTable();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "FS0180PlanInfo";
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@vcPartsNo", SqlDbType.VarChar, 12).Value = partsno;
                    cmd.Parameters.Add("@vcDock", SqlDbType.VarChar, 2).Value = dock;
                    cmd.Parameters.Add("@vcKBorderno", SqlDbType.VarChar, 12).Value = order;
                    cmd.Parameters.Add("@vcKBSerial", SqlDbType.VarChar, 4).Value = serial;
                    apt = new SqlDataAdapter(cmd);
                    apt.Fill(dttmp);
                    if (dttmp.Rows.Count == 0) continue;
                    else if (dttmp.Rows.Count > 0 && dttmp.Rows[0]["vcPlanMonth"].ToString().Trim() != vcMon)
                        continue;
                    else
                    {
                        string tmpMon = Convert.ToDateTime(dttmp.Rows[0]["vcComDate04"]).ToString("yyyy-MM");
                        string day = Convert.ToDateTime(dttmp.Rows[0]["vcComDate04"]).Day.ToString();
                        string zhi = dttmp.Rows[0]["vcBanZhi04"].ToString() == "0" ? "b" : "y";
                        if (tmpMon == vcMon)
                        {
                            tmpMon = "ED";
                        }
                        else
                        {
                            tmpMon = "TD";
                        }
                        string colname = tmpMon + day + zhi + "A";
                        DataRow[] dr = dtNZ.Select("vcPartsNo ='" + dtED.Rows[i]["partsno"].ToString() + "' and vcDock='" + dtED.Rows[i]["dock"].ToString() + "' and EDflag ='" + ED + "' ");
                        if (dr.Length == 1)
                        {
                            dr[0][colname] = Convert.ToInt32(dr[0][colname].ToString().Length == 0 ? "0" : dr[0][colname].ToString()) + Convert.ToInt32(dtED.Rows[i]["num"]);
                            dr[0]["vcActTotal"] = Convert.ToInt32(dr[0]["vcActTotal"]) + Convert.ToInt32(dtED.Rows[i]["num"]);
                        }
                    }
                }
                dtE = dtNZ.Copy();
                #endregion

                addEDflag(ref dtS, "通常");
                addEDflag(ref dtE, "紧急");
                dtNZ = MergeDataTable(dtS, dtE);
                if (ed.Length > 0)
                {
                    dtNZ = dtNZ.Select("vcEDflag ='" + ed + "'").Length > 0 ? dtNZ.Select("vcEDflag ='" + ed + "'").CopyToDataTable() : dtNZ.Clone();
                }
                for (int k = 0; k < dtNZ.Rows.Count; k++)
                {
                    DataRow dr2 = dtNZ.Rows[k];
                    for (int j = 13; j < flag; j++)
                    {
                        if (dtNZ.Columns[j].ColumnName.Substring(dtNZ.Columns[j].ColumnName.Length - 1, 1) != "A" && dr2[j].ToString().Length > 0 && dr2[dtNZ.Columns[j].ColumnName + "A"].ToString().Length == 0)
                        {
                            dr2[dtNZ.Columns[j].ColumnName + "A"] = 0;
                        }
                    }
                }
                #region dt的所有列
                dt = dtNZ.DefaultView.ToTable(false, "vcMonth",
                     "vcPartsNo",
                     "vcCarType",
                     "vcDock",
                     "vcEDflag",
                     "vcMonTotal",
                     "vcActTotal",
                     "TD1b",
                     "TD1bA",
                     "TD1y",
                     "TD1yA",
                     "TD2b",
                     "TD2bA",
                     "TD2y",
                     "TD2yA",
                     "TD3b",
                     "TD3bA",
                     "TD3y",
                     "TD3yA",
                     "TD4b",
                     "TD4bA",
                     "TD4y",
                     "TD4yA",
                     "TD5b",
                     "TD5bA",
                     "TD5y",
                     "TD5yA",
                     "TD6b",
                     "TD6bA",
                     "TD6y",
                     "TD6yA",
                     "TD7b",
                     "TD7bA",
                     "TD7y",
                     "TD7yA",
                     "TD8b",
                     "TD8bA",
                     "TD8y",
                     "TD8yA",
                     "TD9b",
                     "TD9bA",
                     "TD9y",
                     "TD9yA",
                     "TD10b",
                     "TD10bA",
                     "TD10y",
                     "TD10yA",
                     "TD11b",
                     "TD11bA",
                     "TD11y",
                     "TD11yA",
                     "TD12b",
                     "TD12bA",
                     "TD12y",
                     "TD12yA",
                     "TD13b",
                     "TD13bA",
                     "TD13y",
                     "TD13yA",
                     "TD14b",
                     "TD14bA",
                     "TD14y",
                     "TD14yA",
                     "TD15b",
                     "TD15bA",
                     "TD15y",
                     "TD15yA",
                     "TD16b",
                     "TD16bA",
                     "TD16y",
                     "TD16yA",
                     "TD17b",
                     "TD17bA",
                     "TD17y",
                     "TD17yA",
                     "TD18b",
                     "TD18bA",
                     "TD18y",
                     "TD18yA",
                     "TD19b",
                     "TD19bA",
                     "TD19y",
                     "TD19yA",
                     "TD20b",
                     "TD20bA",
                     "TD20y",
                     "TD20yA",
                     "TD21b",
                     "TD21bA",
                     "TD21y",
                     "TD21yA",
                     "TD22b",
                     "TD22bA",
                     "TD22y",
                     "TD22yA",
                     "TD23b",
                     "TD23bA",
                     "TD23y",
                     "TD23yA",
                     "TD24b",
                     "TD24bA",
                     "TD24y",
                     "TD24yA",
                     "TD25b",
                     "TD25bA",
                     "TD25y",
                     "TD25yA",
                     "TD26b",
                     "TD26bA",
                     "TD26y",
                     "TD26yA",
                     "TD27b",
                     "TD27bA",
                     "TD27y",
                     "TD27yA",
                     "TD28b",
                     "TD28bA",
                     "TD28y",
                     "TD28yA",
                     "TD29b",
                     "TD29bA",
                     "TD29y",
                     "TD29yA",
                     "TD30b",
                     "TD30bA",
                     "TD30y",
                     "TD30yA",
                     "TD31b",
                     "TD31bA",
                     "TD31y",
                     "TD31yA",

                     "ED1b",
                     "ED1bA",
                     "ED1y",
                     "ED1yA",
                     "ED2b",
                     "ED2bA",
                     "ED2y",
                     "ED2yA",
                     "ED3b",
                     "ED3bA",
                     "ED3y",
                     "ED3yA",
                     "ED4b",
                     "ED4bA",
                     "ED4y",
                     "ED4yA",
                     "ED5b",
                     "ED5bA",
                     "ED5y",
                     "ED5yA",
                     "ED6b",
                     "ED6bA",
                     "ED6y",
                     "ED6yA",
                     "ED7b",
                     "ED7bA",
                     "ED7y",
                     "ED7yA",
                     "ED8b",
                     "ED8bA",
                     "ED8y",
                     "ED8yA",
                     "ED9b",
                     "ED9bA",
                     "ED9y",
                     "ED9yA",
                     "ED10b",
                     "ED10bA",
                     "ED10y",
                     "ED10yA",
                     "ED11b",
                     "ED11bA",
                     "ED11y",
                     "ED11yA",
                     "ED12b",
                     "ED12bA",
                     "ED12y",
                     "ED12yA",
                     "ED13b",
                     "ED13bA",
                     "ED13y",
                     "ED13yA",
                     "ED14b",
                     "ED14bA",
                     "ED14y",
                     "ED14yA",
                     "ED15b",
                     "ED15bA",
                     "ED15y",
                     "ED15yA",
                     "ED16b",
                     "ED16bA",
                     "ED16y",
                     "ED16yA",
                     "ED17b",
                     "ED17bA",
                     "ED17y",
                     "ED17yA",
                     "ED18b",
                     "ED18bA",
                     "ED18y",
                     "ED18yA",
                     "ED19b",
                     "ED19bA",
                     "ED19y",
                     "ED19yA",
                     "ED20b",
                     "ED20bA",
                     "ED20y",
                     "ED20yA",
                     "ED21b",
                     "ED21bA",
                     "ED21y",
                     "ED21yA",
                     "ED22b",
                     "ED22bA",
                     "ED22y",
                     "ED22yA",
                     "ED23b",
                     "ED23bA",
                     "ED23y",
                     "ED23yA",
                     "ED24b",
                     "ED24bA",
                     "ED24y",
                     "ED24yA",
                     "ED25b",
                     "ED25bA",
                     "ED25y",
                     "ED25yA",
                     "ED26b",
                     "ED26bA",
                     "ED26y",
                     "ED26yA",
                     "ED27b",
                     "ED27bA",
                     "ED27y",
                     "ED27yA",
                     "ED28b",
                     "ED28bA",
                     "ED28y",
                     "ED28yA",
                     "ED29b",
                     "ED29bA",
                     "ED29y",
                     "ED29yA",
                     "ED30b",
                     "ED30bA",
                     "ED30y",
                     "ED30yA",
                     "ED31b",
                     "ED31bA",
                     "ED31y",
                     "ED31yA");
                #endregion
            }

            string tmpslt = " 1=1 ";
            if (vcPartsno.Length > 0)
            {
                tmpslt += " and  vcPartsNo like '%" + vcPartsno + "%'";
            }
            if (vcDock.Length > 0)
            {
                tmpslt += " and vcDock='" + vcDock + "'";
            }
            DataRow[] drs = dt.Select(tmpslt);
            if (drs.Length > 0)
                dt = drs.CopyToDataTable();
            else dt = dt.Clone();
            return dt;
        }

        public static void addEDflag(ref DataTable dt, string flag)
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

        public DataTable SearchItemData(string vcMon, string vcTF, string vcTO, string vcType, string vcPartsno, string vcDock, string vcSerial, string vcOrder, string vcProType, string plant, string ed)
        {
            return dataAccess.SearchItemData(vcMon, vcTF, vcTO, vcType, vcPartsno, vcDock, vcSerial, vcOrder, vcProType, plant, ed);
        }

        public DataTable getPartListCount(string mon, string partNo, string plant, string GC, string KbOrderId, string packdiv, string PlanProductionDateFrom, string PlanProductionBZFrom, string PlanPackDateFrom, string PlanPackBZFrom, string PlanProductionDateTo, string PlanProductionBZTo, string PlanPackDateTo, string PlanPackBZTo, string PlanProductionAB, string PlanPackAB, ref string msg)
        {
            return dataAccess.getPartListCount(mon, partNo, plant, GC, KbOrderId, packdiv, PlanProductionDateFrom, PlanProductionBZFrom, PlanPackDateFrom, PlanPackBZFrom, PlanProductionDateTo, PlanProductionBZTo, PlanPackDateTo, PlanPackBZTo, PlanProductionAB, PlanPackAB, ref msg);
        }

        public void deletetKanbanPrintTbl(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            dataAccess.deletetKanbanPrintTbl(listInfoData, strUserId);
        }
    }
}