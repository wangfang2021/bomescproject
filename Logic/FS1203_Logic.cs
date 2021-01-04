/*******************************************************************
* 	项目名称			:	TPCS								   	
* 	模块名称			:	部署组别生产条件维护					
* 	创建者			    :	GAOLEI								
* 	创建日期			:	2020/08/24							
* 	类名			    :	FS1202_Logic					    
* 	修改者			    :						
* 	修改时间			:						
* 	修改内容			:											
* 					
* 	(C)2020-TJQM INFORMATION TECHNOLOGY CO.,LTD All Rights Reserved.
*******************************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS1203_Logic
    {
        FS1203_DataAccess dataAccess = new FS1203_DataAccess();

        public DataTable getPartsno(string mon)
        {
            return dataAccess.getPartsno(mon);
        }
        public DataTable tranNZtoWZ(DataTable dtNZ, string mon)//内制品转换相应外注
        {
            DataTable dtWz = new DataTable();
            DataTable dtInfo = getPartsno(mon);
            dtWz = dtNZ.Copy();
            foreach (DataRow row_wz in dtWz.Rows)
            {
                string s1 = row_wz["vcPartsNo"].ToString().Replace("-", "");
                string s2 = row_wz["vcDock"].ToString();
                DataRow[] isQF = dtInfo.Select("vcPartsNo='" + s1 + "' and vcQFflag='1' and vcDock='" + s2 + "'");
                if (isQF.Length > 0)
                {
                    DataRow[] tmp = dtInfo.Select("substring(vcPartsNo,1,10)=substring( '" + s1 + "', 1, 10) and vcInOutFlag=1");
                    if (tmp.Length > 0)
                    {
                        string tmppartsno = tmp[0]["vcPartsNo"].ToString();
                        string partsno = tmppartsno.Insert(5, "-").Insert(11, "-");
                        string dock = tmp[0]["vcDock"].ToString();
                        row_wz["vcPartsNo"] = partsno;
                        row_wz["vcDock"] = dock;
                    }
                }
            }
            return dtWz;
        }
        public DataTable tranWztoNz(DataTable dtWz, string mon)//外注品转换相应内制品
        {
            DataTable dtNz = new DataTable();
            DataTable dtInfo = getPartsno(mon);
            dtNz = dtWz.Copy();
            //foreach (var row in dtNz.AsEnumerable())
            //{
            //    var isWz = from info in dtInfo.AsEnumerable()
            //               where info.Field<string>("vcPartsNo") == row.Field<string>("vcPartsNo").Replace("-", "")
            //               && info.Field<string>("vcDock") == row.Field<string>("vcDock")
            //               && info.Field<string>("vcInOutFlag") == "1"
            //               select info;
            //    if (isWz.Count() > 0)
            //    {
            //        var tmp = from info in dtInfo.AsEnumerable()
            //                  where info.Field<string>("vcPartsNo").Substring(0, 10) == row.Field<string>("vcPartsNo").Replace("-", "").Substring(0, 10)
            //                  && info.Field<string>("vcInOutFlag") == "0"
            //                  select new
            //                  {
            //                      o_partsno = info.Field<string>("vcPartsNo"),
            //                      o_dock = info.Field<string>("vcDock")
            //                  };
            //        if (tmp.Count() > 0)
            //        {
            //            string tmppartsno = tmp.ElementAt(0).o_partsno.ToString();
            //            string partsno = tmppartsno.Insert(5, "-").Insert(11, "-");
            //            string dock = tmp.ElementAt(0).o_dock.ToString();
            //            row.SetField<string>("vcPartsNo", partsno);
            //            row.SetField<string>("vcDock", dock);
            //        }
            //    }
            //}
            return dtNz;
        }

        public DataTable getPlantype()
        {
            return dataAccess.getPlantype();
        }
        public DataTable bindplant()
        {
            return dataAccess.bindplant();
        }

        public DataTable serchData(string mon, string plan, string type, string plant, string plantname)//检索方法
        {
            DataTable dt = new DataTable();
            switch (plan)
            {
                case "1"://生产计划  值别
                    {
                        if (type == "0")//合算
                        {
                            dt = dataAccess.getMonProALL2(mon, "MonthProdPlanTbl", "EDMonthProdPlanTbl", plant);
                            dt.TableName = "ALLPro1" + "-" + plantname;
                        }
                        if (type == "1")//月度
                        {
                            dt = dataAccess.getMonPackPlanTMP(mon, "MonthProdPlanTbl", plant);
                            addEDflag(ref dt, "通常");
                            dt.TableName = "MonPro1" + "-" + plantname;
                        }
                        break;
                    }
                case "0"://包装计划  值别
                    {
                        if (type == "0")//合算
                        {
                            dt = dataAccess.getMonProALL2(mon, "MonthPackPlanTbl", "EDMonthPackPlanTbl", plant);
                            dt = tranNZtoWZ(dt, mon);
                            dt.TableName = "ALLPro4" + "-" + plantname;
                        }
                        if (type == "1")//月度
                        {
                            dt = dataAccess.getMonPackPlanTMP(mon, "MonthPackPlanTbl", plant);
                            dt = tranNZtoWZ(dt, mon);
                            addEDflag(ref dt, "通常");
                            dt.TableName = "MonPro4" + "-" + plantname;
                        }
                        break;
                    }
                case "2"://看板打印计划  值别
                    {
                        if (type == "0")//合算
                        {
                            dt = dataAccess.getMonProALL2(mon, "MonthKanBanPlanTbl", "EDMonthKanBanPlanTbl", plant);
                            dt.TableName = "ALLPro0" + "-" + plantname;
                        }
                        if (type == "1")//月度
                        {
                            dt = dataAccess.getMonPackPlanTMP(mon, "MonthKanBanPlanTbl", plant);
                            dt.TableName = "MonPro0" + "-" + plantname;
                            addEDflag(ref dt, "通常");
                        }
                        break;
                    }
                case "3"://丰铁看板涂装计划  值别
                    {
                        if (type == "0")//合算
                        {
                            dt = dataAccess.getMonProALL2(mon, "MonthTZPlanTbl", "dbo.EDMonthTZPlanTbl", plant);
                            dt = tranNZtoWZ(dt, mon);
                            dt.TableName = "ALLPro2" + "-" + plantname;
                        }
                        if (type == "1")//月度
                        {
                            dt = dataAccess.getMonPackPlanTMP(mon, "MonthTZPlanTbl", plant);
                            dt = tranNZtoWZ(dt, mon);
                            addEDflag(ref dt, "通常");
                        }
                        break;
                    }
                case "4"://P3计划  值别
                    {
                        if (type == "0")//合算
                        {
                            dt = dataAccess.getMonProALL2(mon, "MonthP3PlanTbl", "EDMonthP3PlanTbl", plant);
                            dt = tranNZtoWZ(dt, mon);
                            dt.TableName = "ALLPro3" + "-" + plantname;
                        }
                        if (type == "1")//月度
                        {
                            dt = dataAccess.getMonPackPlanTMP(mon, "MonthP3PlanTbl", plant);
                            dt = tranNZtoWZ(dt, mon);
                            addEDflag(ref dt, "通常");
                        }
                        break;
                    }
                case "Importpack":
                    {
                        dt = dataAccess.getMonPackPlanTMPcur(mon, "MonthPackPlanTblTMP", plant);
                        addEDflag(ref dt, "通常"); ;
                        //   dt = tranNZtoWZ(dt);
                        break;
                    }
                case "Importpro":
                    {
                        dt = dataAccess.getMonPackPlanTMPcur(mon, "MonthProPlanTblTMP", plant);
                        addEDflag(ref dt, "通常");
                        break;
                    }
            }
            dt.Columns["vcEDflag"].SetOrdinal(5);
            return dt;
        }

        public void addEDflag(ref DataTable dt, string flag)//加列
        {
            DataColumn col = new DataColumn();
            col.ColumnName = "vcEDflag";
            col.DataType = typeof(string);
            col.DefaultValue = flag;
            dt.Columns.Add(col);
        }


        /// <summary>
        /// 弹出的编辑界面点击检索按钮获取列表数据
        /// </summary>
        /// <param name="mon"></param>
        public DataTable getCutPlan(string mon)
        {
            return dataAccess.getCutPlan(mon);
        }
        #region 删除
        public void Del_Plan(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            dataAccess.Del_Plan(listInfoData, strUserId);
        }
        #endregion

    }
}