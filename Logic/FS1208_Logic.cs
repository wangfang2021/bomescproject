/*******************************************************************
* 	项目名称			:	TPCS								   	
* 	模块名称			:	紧急计划生成					
* 	创建者			    :	GAOLEI								
* 	创建日期			:	2020/09/2							
* 	类名			    :	FS1208_Logic					    
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
    public class FS1208_Logic
    {
        FS1208_DataAccess dataAccess = new FS1208_DataAccess();

        /// <summary>
        /// 紧急订单导入
        /// </summary>
        /// <param name="file"></param>
        /// <param name="userId"></param>
        /// <returns>msg</returns>
        //public string EDPlanFileImport(HttpPostedFile file, string userId)
        //{
        //    FileInfo file_im = new FileInfo(file.FileName);
        //    if (file_im.Extension != ".xls" && file_im.Extension != ".xlt" && file_im.Extension != ".xlsx")
        //    {
        //        return "文件格式不正确！";
        //    }
        //    QMExcel oQMExcel = new QMExcel();
        //    string path;
        //    string tepath = HttpContext.Current.Server.MapPath("~/Templates/FS1208_EDPlanImport.xlt");
        //    if (file_im.Extension == ".xls" || file_im.Extension == ".xlt")
        //    {
        //        path = HttpContext.Current.Server.MapPath("~/Temps/FS1208Im.xls");
        //    }
        //    else
        //    {
        //        path = HttpContext.Current.Server.MapPath("~/Temps/FS1208Im.xlsx");
        //    }
        //    if (File.Exists(path))
        //    {
        //        File.Delete(path);
        //    }
        //    file.SaveAs(path);
        //    DataTable dtTmplate = oQMExcel.GetExcelContentByOleDb(tepath);//导入Excel文件到DataTable
        //    DataTable dt = new DataTable();
        //    string msg = checkExcel(path, ref dt, dtTmplate);//检验导入的数据
        //    if (msg.Length > 0)
        //    {
        //        return msg;
        //    }
        //    msg = UpdateEDPlan(dt, userId);//将导入的表格数据上传

        //    return msg;
        //}

        /// <summary>
        /// 计划导出
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="exlName"></param>
        //public string PlanFileExport(DataTable dt, string exlName)
        //{
        //    string msg = null;
        //    try
        //    {
        //        if (dt != null && dt.Rows.Count > 0)
        //        {
        //            if(dt.Columns.Contains("iFlag"))
        //            {
        //                dt.Columns.Remove("iFlag");
        //            }
        //            if (dt.Columns.Contains("vcPartsNoFZ"))
        //            {
        //                dt.Columns.Remove("vcPartsNoFZ");
        //            }
        //            if (dt.Columns.Contains("vcSource"))
        //            {
        //                dt.Columns.Remove("vcSource");
        //            }
        //            QMCommon.OfficeCommon.QMExcel oQMExcel = new QMCommon.OfficeCommon.QMExcel();
        //            string tmplatePath = System.Web.HttpContext.Current.Server.MapPath("~/Templates/FS1208_EDPlanExprot.xlt");
        //            string path = System.Web.HttpContext.Current.Server.MapPath("~/Temps/" + exlName);
        //            oQMExcel.ExportFromTemplate(dt, tmplatePath, path, 2, 1, true);

        //            dt.Dispose();
        //        }
        //        else
        //        {
        //            msg = "无检索数据,无法导出数据";
        //        }

        //    }
        //    catch (WebException ex)
        //    {
        //        LogHelper.ErrorLog("发注计算导出异常：" + ex.Message);
        //        msg = "导出失败！";
        //    }
        //    return msg;
        //}

        public DataTable getEDPlanInfo(string Mon, string Partsno, string cartype, string dock, string type, string pro, string zhi, string day, string order)
        {
            return dataAccess.getEDPlanInfo(Mon, Partsno, cartype, dock, type, pro, zhi, day, order);
        }

        public string UpdateTable(DataTable dt, string user)
        {
            return dataAccess.UpdateTable(dt, user);
        }

        public string UpdatePlan(string mon, string user)
        //public string UpdatePlan(string mon,string user,string plant)
        {
            return dataAccess.UpdatePlan(mon, user);
        }
        public DataTable serchData(string mon, string plant, string plan, ref Exception e, string plantname)
        {
            DataTable dt = new DataTable();
            FS1203_Logic fS1203_Logic = new FS1203_Logic();
            switch (plan)
            {
                case "0":   //包装计划
                    dt = dataAccess.getSearch(mon, plant, "EDMonthPackPlanTbl", ref e);
                    dt = fS1203_Logic.tranNZtoWZ(dt, mon);
                    dt.TableName = "P4" + "-" + plantname;
                    break;
                case "1":   //生产计划
                    dt = dataAccess.getSearch(mon, plant, "EDMonthProdPlanTbl", ref e);
                    dt.TableName = "P1" + "-" + plantname;
                    break;
                case "2":   //看板打印计划
                    dt = dataAccess.getSearch(mon, plant, "EDMonthKanBanPlanTbl", ref e);
                    dt.TableName = "P0" + "-" + plantname;
                    break;
                case "3":   //涂装计划
                    dt = dataAccess.getSearch(mon, plant, "EDMonthTZPlanTbl", ref e);
                    dt = fS1203_Logic.tranNZtoWZ(dt, mon);
                    dt.TableName = "P2" + "-" + plantname;
                    break;
                case "4":   //P3计划
                    dt = dataAccess.getSearch(mon, plant, "EDMonthP3PlanTbl", ref e);
                    dt = fS1203_Logic.tranNZtoWZ(dt, mon);
                    dt.TableName = "P3" + "-" + plantname;
                    break;
            }
            return dt;
        }
        public string getPlanName(string n, out string plantname)
        {
            string re = "";
            string[] a = n.Split('-');
            string tablename = "";
            plantname = "";
            if (a.Length > 0)
            {
                tablename = a[0].ToString();
            }
            if (a.Length > 1)
            {
                plantname = a[1].ToString();
            }
            if (a.Length == 0)
            {
                tablename = n;
            }
            switch (tablename)
            {
                case "P0": re = "工程0紧急计划"; break;
                case "P1": re = "工程1紧急计划"; break;
                case "P2": re = "工程2紧急计划"; break;
                case "P3": re = "工程3紧急计划"; break;
                case "P4": re = "工程4紧急计划"; break;
            }
            return re;
        }
        public DataTable GetPlant()
        {
            return dataAccess.GetPlant();
        }
        public DataTable GetPlanType()
        {
            return dataAccess.GetPlanType();
        }

        public string checkExcel(string excelpath, ref DataTable dtre, DataTable dtTmplate)
        {
            string msg = "";
            //QMCommon.OfficeCommon.QMExcel oQMExcel = new QMCommon.OfficeCommon.QMExcel();
            //DataTable dt = oQMExcel.GetExcelContentByOleDb(excelpath);//导入文件
            //msg = checkExcelHeadpos(dt, dtTmplate);//校验模板
            //if (msg.Length > 0) return msg;
            //msg = checkExcelData(dt);//校验数据
            //dtre = dt;
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
        public string checkExcelData(DataTable dt)
        {
            string msg = "";
            DataTable dt_chk = new DataTable();
            dt_chk = dataAccess.getPartsInfo();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string partsno = dt.Rows[i][1].ToString().Replace("-", "").Trim();
                string dock = dt.Rows[i][2].ToString().Trim();
                string cartype = dt.Rows[i][3].ToString().Trim();
                if (partsno.Length == 0 && dock.Length == 0 && cartype.Length == 0) return msg;
                if (dt_chk.Select(" vcPartsNo ='" + partsno + "' and vcCarFamilyCode='" + cartype + "' and vcDock='" + dock + "' ").Length == 0)
                {
                    msg = "行：" + (i + 2) + " 品番基础信息不存在。";//20181009除了Datatable第一行行号是0开始之外，导入的模板文件还有一个标题行，因此不能i+1，应该i+2
                    return msg;
                }

                string month = dt.Rows[i][0].ToString().Trim();
                if (month.Length > 0)
                {
                    try
                    {
                        DateTime tmp = Convert.ToDateTime(month);
                    }
                    catch
                    {
                        msg = "行：" + (i + 1) + " 对象月信息输入非法。";
                        return msg;
                    }
                }
                else
                {
                    msg = "行：" + (i + 1) + " 对象月信息不能为空。";
                    return msg;
                }
                string carlender0 = dt.Rows[i][6].ToString().Trim();
                if (carlender0.Length > 0)
                {
                    try
                    {
                        DateTime tmp = Convert.ToDateTime(carlender0);
                    }
                    catch
                    {
                        msg = "行：" + (i + 1) + " 工程0日期信息输入非法。";
                        return msg;
                    }
                }
                string carlender1 = dt.Rows[i][8].ToString().Trim();
                if (carlender1.Length > 0)
                {
                    try
                    {
                        DateTime tmp = Convert.ToDateTime(carlender1);
                    }
                    catch
                    {
                        msg = "行：" + (i + 1) + " 工程1日期信息输入非法。";
                        return msg;
                    }
                }
                string carlender2 = dt.Rows[i][10].ToString().Trim();
                if (carlender2.Length > 0)
                {
                    try
                    {
                        DateTime tmp = Convert.ToDateTime(carlender2);
                    }
                    catch
                    {
                        msg = "行：" + (i + 1) + " 工程2日期信息输入非法。";
                        return msg;
                    }
                }
                string carlender3 = dt.Rows[i][12].ToString().Trim();
                if (carlender3.Length > 0)
                {
                    try
                    {
                        DateTime tmp = Convert.ToDateTime(carlender3);
                    }
                    catch
                    {
                        msg = "行：" + (i + 1) + " 工程3日期信息输入非法。";
                        return msg;
                    }
                }
                string carlender4 = dt.Rows[i][14].ToString().Trim();
                if (carlender4.Length > 0)
                {
                    try
                    {
                        DateTime tmp = Convert.ToDateTime(carlender4);
                    }
                    catch
                    {
                        msg = "行：" + (i + 1) + " 工程4日期信息输入非法。";
                        return msg;
                    }
                }

                string zhi0 = dt.Rows[i][7].ToString().Trim();
                string zhi1 = dt.Rows[i][9].ToString().Trim();
                string zhi2 = dt.Rows[i][11].ToString().Trim();
                string zhi3 = dt.Rows[i][13].ToString().Trim();
                string zhi4 = dt.Rows[i][15].ToString().Trim();
                if (zhi0 != "白" && zhi0 != "白值" && zhi0 != "夜" && zhi0 != "夜值" && zhi0.Length > 0)
                {
                    msg = "行：" + (i + 1) + " 工程0值别信息输入非法。";
                    return msg;
                }
                if (zhi1 != "白" && zhi1 != "白值" && zhi1 != "夜" && zhi1 != "夜值" && zhi1.Length > 0)
                {
                    msg = "行：" + (i + 1) + " 工程1值别信息输入非法。";
                    return msg;
                }
                if (zhi2 != "白" && zhi2 != "白值" && zhi2 != "夜" && zhi2 != "夜值" && zhi2.Length > 0)
                {
                    msg = "行：" + (i + 1) + " 工程2值别信息输入非法。";
                    return msg;
                }
                if (zhi3 != "白" && zhi3 != "白值" && zhi3 != "夜" && zhi3 != "夜值" && zhi3.Length > 0)
                {
                    msg = "行：" + (i + 1) + " 工程3值别信息输入非法。";
                    return msg;
                }
                if (zhi4 != "白" && zhi4 != "白值" && zhi4 != "夜" && zhi4 != "夜值" && zhi4.Length > 0)
                {
                    msg = "行：" + (i + 1) + " 工程4值别信息输入非法。";
                    return msg;
                }
                if ((carlender0.Length == 0 && zhi0.Length > 0) || (carlender0.Length > 0 && zhi0.Length == 0))
                {
                    msg = "行：" + (i + 1) + " 工程0信息输入非法。";
                    return msg;
                }
                if ((carlender1.Length == 0 && zhi1.Length > 0) || (carlender1.Length > 0 && zhi1.Length == 0))
                {
                    msg = "行：" + (i + 1) + " 工程1信息输入非法。";
                    return msg;
                }
                if ((carlender2.Length == 0 && zhi2.Length > 0) || (carlender2.Length > 0 && zhi2.Length == 0))
                {
                    msg = "行：" + (i + 1) + " 工程2信息输入非法。";
                    return msg;
                }
                if ((carlender3.Length == 0 && zhi3.Length > 0) || (carlender3.Length > 0 && zhi3.Length == 0))
                {
                    msg = "行：" + (i + 1) + " 工程3信息输入非法。";
                    return msg;
                }
                if ((carlender4.Length == 0 && zhi4.Length > 0) || (carlender4.Length > 0 && zhi4.Length == 0))
                {
                    msg = "行：" + (i + 1) + " 工程4信息输入非法。";
                    return msg;
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
            A.Add("V");
            A.Add("W");
            A.Add("X");
            A.Add("Y");
            A.Add("Z");
            if (i < 26) re = A[i];
            if (i >= 26) re = A[(i / 26) - 1] + A[i % 26];
            return re;
        }

        public string UpdateEDPlan(DataTable dt, string user)
        {
            return dataAccess.UpdateEDPlan(dt, user);
        }

        //public string PartsChange(FS1208_ViewModel fS1208_ViewModel)
        //{
        //    return dataAccess.PartsChange(fS1208_ViewModel);
        //}


        #region 子画面
        #region 删除
        public void Del_Order(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            dataAccess.Del_Order(listInfoData, strUserId);
        }
        #endregion
        #endregion
    }
}