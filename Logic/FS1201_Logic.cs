using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using DataAccess;
using System.Collections;
using System;
using System.Linq;
using System.Text;

namespace Logic
{
    public class FS1201_Logic
    {
        FS1201_DataAccess dataAccess = new FS1201_DataAccess();

        /// <summary>
        /// 类别下拉框
        /// </summary>
        public DataTable bindPartType()
        {
            return dataAccess.bindPartType();
        }
        /// <summary>
        /// 生产部署下拉框
        /// </summary>
        public DataTable bindProType()
        {
            return dataAccess.bindProType();
        }
        public DataTable bindProType(string Calendartype)
        {
            return dataAccess.bindProType(Calendartype);
        }

        /// <summary>
        /// 组别下拉框
        /// </summary>
        /// <param name="protype">生产部署</param>
        public DataTable bindZB(string protype)
        {
            return dataAccess.bindZB(protype);
        }

        public DataTable BindPlant(int type = 1)
        {
            return dataAccess.BindPlant(type);
        }

        /// <summary>
        /// 根据检索条件获取全年的稼动数据
        /// </summary>
        /// <param name="fS1201_ViewModel">检索条件</param>
        //public List<DataTable> getRenders(string vcYear, string vcMonth, string vcProType, string vcZB, string vcPlant, string cellcolor)
        //{
        //    List<DataTable> dataSets = new List<DataTable>();
        //    vcYear = DateTime.Now.Year.ToString();
        //    for (int month = 1; month <= 12; month++)
        //    {
        //        vcMonth = month.ToString();
        //        dataSets.Add(getDataTablesByMonth(vcYear, vcMonth, vcProType, vcZB, vcPlant, cellcolor));
        //    }
        //    return dataSets;
        //}

        public DataTable getRenders(string vcYear, string vcMonth, string vcProType, string vcZB, string vcPlant, string cellcolor)
        {
            DataTable dt = getDataTablesByMonth(vcYear, vcMonth, vcProType, vcZB, vcPlant, cellcolor);
            return dt;
        }

        private DataTable getDataTablesByMonth(string vcYear, string vcMonth, string vcProType, string vcZB, string vcPlant, string cellcolor)
        {
            DataTable dt;
            DataTable dt_calendar;
            DataTable dt_plan;
            DataRow dw;

            if (String.IsNullOrEmpty(cellcolor))
            {
                cellcolor = "#5DAD64";
            }
            dt_calendar = dataAccess.CalendarSource(vcYear, vcMonth);
            dt_plan = dataAccess.PlanSource(vcYear, vcMonth, vcProType, vcZB, vcPlant);
            string curNum;
            string search;
            string daySingle;
            dt = new DataTable();
            setDataTable(dt);
            for (int i = 0; i < dt_calendar.Rows.Count; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    dw = dt.NewRow();
                    curNum = dt_calendar.Rows[i][j].ToString().Trim();
                    if (String.IsNullOrEmpty(curNum))
                    {
                        dw["num"] = "0";
                        dt.Rows.Add(dw);
                        continue;
                    }
                    search = string.Format("dayall='D{0}b'", curNum);
                    dw["num"] = curNum;
                    dw["backColorLeft"] = "#ffffff";
                    dw["backColorRight"] = "#ffffff";
                    dw["left"] = "";
                    dw["leftValue"] = "";
                    dw["right"] = "";
                    dw["rightValue"] = "";
                    if (dt_plan.Rows.Count > 0 && dt_plan.Select(search).Length > 0)
                    {
                        daySingle = dt_plan.Select(search)[0]["daySingle"].ToString().Trim();
                        if (daySingle.Substring(0, 1) == "m" || daySingle.Substring(0, 1) == "M")
                        {
                            dw["backColorLeft"] = cellcolor;
                        }
                        else
                        {
                            dw["backColorLeft"] = cellcolor;
                        }
                        dw["left"] = daySingle.Substring(1, daySingle.Length - 1);
                        dw["leftValue"] = string.Format("~D{0}b", curNum);

                    }
                    search = string.Format("dayall='D{0}y'", curNum);
                    if (dt_plan.Rows.Count > 0 && dt_plan.Select(search).Length > 0)
                    {
                        daySingle = dt_plan.Select(search)[0]["daySingle"].ToString().Trim();
                        if (daySingle.Substring(0, 1) == "m" || daySingle.Substring(0, 1) == "M")
                        {
                            dw["backColorRight"] = cellcolor;
                        }
                        else
                        {
                            dw["backColorRight"] = cellcolor;
                        }
                        dw["right"] = daySingle.Substring(1, daySingle.Length - 1);
                        dw["rightValue"] = string.Format("~D{0}y", curNum);
                    }
                    dt.Rows.Add(dw);
                }
            }
            return dt;
        }


        private void setDataTable(DataTable dt)
        {
            dt.Columns.Add("num", Type.GetType("System.String"));
            dt.Columns.Add("backColorLeft", Type.GetType("System.String"));
            dt.Columns.Add("backColorRight", Type.GetType("System.String"));
            dt.Columns.Add("left", Type.GetType("System.String"));
            dt.Columns.Add("leftValue", Type.GetType("System.String"));
            dt.Columns.Add("right", Type.GetType("System.String"));
            dt.Columns.Add("rightValue", Type.GetType("System.String"));
        }

        /// <summary>
        /// 数据更新
        /// </summary>
        /// <param name="fS1201_ViewModel"></param>
        public void UpdateCalendar(string vcPlant, string vcGC, string vcZB, string vcYear, string vcMonth, string[] Udata)
        {
            dataAccess.UpdateCalendar(vcPlant, vcGC, vcZB, vcYear, vcMonth, Udata);
        }

        public void UpdateTable(DataTable dt, string Month)
        {
            dataAccess.UpdateTable(dt, Month);
        }

        public string checkExcel(string excelpath, ref DataTable dtre, DataTable dtTmplate)
        {
            string msg = "";
            //QMCommon.OfficeCommon.QMExcel oQMExcel = new QMCommon.OfficeCommon.QMExcel();
            ////string templatepath =System.Web.HttpContext.Current.Server
            ////FileInfo info = new FileInfo("~\\Template\\FS0110_Calendar.xlt");
            //DataTable dt = oQMExcel.GetExcelContentByOleDb(excelpath);//导入文件
            ////DataTable dtTmplate = oQMExcel.GetExcelContentByOleDb(info.FullName);//模板文件
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
                if (dt.Rows[0][i].ToString().Trim() != dtTmplate.Rows[0][i].ToString().Trim())
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
            dt_chk = dataAccess.getConst();//获取常量数据
            dt.Columns[0].ColumnName = "F1";
            for (int i = 1; i < dt.Rows.Count; i++)
            {
                string plant = dt.Rows[i][1].ToString().Trim();
                string calendartype = dt.Rows[i][0].ToString().Trim();
                string gc = dt.Rows[i][2].ToString().Trim();
                string zb = dt.Rows[i][3].ToString().Trim();
                //string chk = "";
                //空校验
                if (calendartype.Length <= 0 && zb.Length <= 0 && gc.Length <= 0 && plant.Length <= 0)
                    break;
                if (plant.Length <= 0)
                {
                    msg = "第" + (i + 2) + "行，工厂不能为空。";
                    return msg;
                }
                else
                {
                    if (dt_chk.Select("vcDataid ='KBPlant' and vcData2 ='" + plant + "'  ").Length <= 0)
                    {
                        msg = "第" + (i + 2) + "行，无效的工厂。";
                        return msg;
                    }
                }
                //存在性校验
                if (calendartype.Length <= 0)
                {
                    msg = "第" + (i + 2) + "行，无效的品番类别。";
                    return msg;
                }
                else
                {
                    if (calendartype.ToString().Trim() == "非指定")
                    {
                        if (gc.Length > 0 || zb.Length > 0)
                        {
                            msg = "第" + (i + 2) + "行，非指定类别的品番无生产部署和组别区分。";
                            return msg;
                        }
                    }
                    else if (calendartype.ToString().Trim() == "指定")
                    {
                        if (dt_chk.Select(" vcData1='" + gc + "' and vcData3 ='" + zb + "' and vcDataid='ProType' ").Length <= 0)
                        {
                            msg = "第" + (i + 2) + "行，无效的生产部署或组别。";
                            return msg;
                        }
                    }
                    else
                    {
                        msg = "第" + (i + 2) + "行，无效的品番类别。";
                    }
                }
                //重复性校验
                if (calendartype.ToString().Trim() == "非指定")
                {
                    if (dt.Select("F1='非指定' and  F2='" + plant + "'").Length > 1)
                    {
                        msg = "第" + (i + 2) + "行，存在相同的品番类别和工厂。";
                        return msg;
                    }
                }
                else if (calendartype.ToString().Trim() == "指定")
                {
                    if (dt.Select(" F1='指定' and  F3='" + gc + "' and F4='" + zb + "' ").Length > 1)
                    {
                        msg = "第" + (i + 2) + "行，存在相同生产部署和组别。";
                        return msg;
                    }
                }
                if (msg.Length > 0) return msg;
                //值校验      
                for (int j = 4; j < dt.Columns.Count; j++)
                {
                    if (calendartype == "指定")
                    {
                        //把M和N分为AB值，即分为MA、MB和NA、NB - 刘刚
                        if (dt.Rows[i][j].ToString().Trim() != "MA" && dt.Rows[i][j].ToString().Trim() != "MB" && dt.Rows[i][j].ToString().Trim() != "NA" && dt.Rows[i][j].ToString().Trim() != "NB" && dt.Rows[i][j].ToString().Trim() != "")
                        {
                            msg = "第" + (i + 2) + "行," + ExcelPos(j) + "列输入非法。";
                            return msg;
                        }
                    }
                    else if (calendartype == "非指定")
                    {
                        //把M分为AB值，即分为MA、MB - 刘刚
                        if (dt.Rows[i][j].ToString().Trim() != "MA" && dt.Rows[i][j].ToString().Trim() != "MB" && dt.Rows[i][j].ToString().Trim() != "")
                        {
                            msg = "第" + (i + 2) + "行," + ExcelPos(j) + "列输入非法。";
                            return msg;
                        }
                    }
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
    }
}