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
        //public void UpdateCalendar(FS1201_ViewModel fS1201_ViewModel)
        //{
        //    byte[] rep = { 194, 160 };
        //    string Add_data = fS1201_ViewModel.vcData;
        //    string[] Udata = Add_data.Replace(System.Text.Encoding.GetEncoding("UTF-8").GetString(rep), "").Split('|');

        //    dataAccess.UpdateCalendar(plant, gc, zb, year, month, Udata);
        //}
    }
}