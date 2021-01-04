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
    public class FS1204_Logic
    {
        FS1204_DataAccess dataAccess = new FS1204_DataAccess();

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
            string searchStr = "1,2,3,4,5";
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
                        if (searchStr.Contains(daySingle))
                        {
                            dw["backColorLeft"] = cellcolor;
                        }
                        else
                        {
                            dw["backColorLeft"] = "#ff9bff";
                        }
                        dw["left"] = daySingle;
                        dw["leftValue"] = string.Format("~D{0}b", curNum);

                    }
                    search = string.Format("dayall='D{0}y'", curNum);
                    if (dt_plan.Rows.Count > 0 && dt_plan.Select(search).Length > 0)
                    {
                        daySingle = dt_plan.Select(search)[0]["daySingle"].ToString().Trim();
                        if (searchStr.Contains(daySingle))
                        {
                            dw["backColorRight"] = cellcolor;
                        }
                        else
                        {
                            dw["backColorRight"] = "#ff9bff";
                        }
                        dw["right"] = daySingle;
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



    }
}