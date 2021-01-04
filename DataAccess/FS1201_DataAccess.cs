using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Common;
using System.Data.SqlClient;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace DataAccess
{
    public class FS1201_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        /// <summary>
        /// 获取当前年月的日历
        /// </summary>
        /// <param name="year">当前年</param>
        /// <param name="month">当前月</param>
        public DataTable CalendarSource(string year, string month)
        {
            string dateTime = string.Format("{0}-{1}-1", year, month);
            string ssql = "";
            ssql += " DECLARE @DATE DATETIME ";
            ssql += " SET @DATE='" + dateTime + "'";
            ssql += " DECLARE @Start DATETIME,@End DATETIME";
            ssql += " DECLARE @Index INT";
            ssql += " SET @Start =DATEADD(MONTH,DATEDIFF(MONTH,0,@Date),0)";
            ssql += " SET @End=DATEADD(MONTH,1,@Start)";
            ssql += " SET @Index=DATEDIFF(DAY,-1,@Start)%7-1;";
            ssql += " SET @Start=DATEADD(mm,DATEDIFF(mm,0,@Date),0) ";
            ssql += " SET @End= DATEADD(mm,1,@Start)-1";
            ssql += " SET @Index=DATEDIFF(day,0,@Start)%7";
            ssql += " ;WITH";
            ssql += " temp(date,row,col) AS";
            ssql += "     (";
            ssql += "         SELECT date=1,row=@Index/7+1,col=@Index%7+1";
            ssql += " UNION";
            ssql += " ALL";
            ssql += " SELECT date=date+1,row=(@Index+date)/7+1,col=(@Index+date)%7+1";
            ssql += " FROM";
            ssql += " temp";
            ssql += " WHERE date <=";
            ssql += " DATEDIFF(DAY,@Start,@End)";
            ssql += "     )";
            ssql += "     SELECT ISNULL(CONVERT(CHAR(2),[1]),null) AS MON,";
            ssql += " ISNULL(CONVERT(CHAR(2),[2]),null) AS TUE,";
            ssql += " ISNULL(CONVERT(CHAR(2),[3]),null) AS WED,";
            ssql += " ISNULL(CONVERT(CHAR(2),[4]),null) AS THU,";
            ssql += " ISNULL(CONVERT(CHAR(2),[5]),null) AS FRI,";
            ssql += " ISNULL(CONVERT(CHAR(2),[6]),null) AS SAT,";
            ssql += " ISNULL(CONVERT(CHAR(2),[7]),null) AS SUN";
            ssql += "     FROM";
            ssql += " temp";
            ssql += "     PIVOT";
            ssql += "     (    ";
            ssql += "         MAX(date) FOR col IN ([1],[2],[3],[4],[5],[6],[7])";
            ssql += "     ) AS B";
            try
            {
                DataSet dataSet = excute.ExcuteSqlWithSelectToDS(ssql);
                return dataSet.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取当前年月的稼动数据
        /// </summary>
        /// <param name="fS1201_ViewModel"></param>
        public DataTable PlanSource(string vcYear, string vcMonth, string vcProType, string vcZB, string vcPlant)
        {
            if (vcProType == "" || vcZB == "" || vcProType == null || vcZB == null)
                return new DataTable();
            if (vcMonth == null || vcMonth.ToString() == "")
                vcMonth = DateTime.Now.Month.ToString().Replace("'", "");
            DateTime mon1 = Convert.ToDateTime((vcYear + "-" + vcMonth + "-1").Replace("'", ""));
            double daynum = (mon1.AddMonths(1) - mon1).TotalDays;
            string sqltmp = "";
            for (double i = 1; i < daynum + 1; i++)
            {
                if (i == daynum)
                    sqltmp += "D" + i + "b,D" + i + "y";
                else sqltmp += "D" + i + "b,D" + i + "y,";
            }
            StringBuilder sql = new StringBuilder();
            string tableName = getTableName(vcMonth);
            SqlParameter[] paras = new SqlParameter[4];
            sql.AppendLine(" select vcYear,vcMonth,vcGC,vcZB,dayall,daySingle from " + tableName);
            sql.AppendLine(" unpivot( daySingle for dayall in (" + sqltmp);
            sql.AppendLine(" )) P where len(daysingle)>0 ");

            if (vcYear != null && vcYear != "")
            {
                sql.AppendLine(" and vcYear='" + vcYear + "'");
            }
            if (vcProType != null && vcProType != "")
            {
                sql.AppendLine(" and vcGC='" + vcProType + "'");
            }
            if (vcZB != null && vcZB != "")
            {
                sql.AppendLine(" and vcZB='" + vcZB + "'");
            }
            if (vcPlant != null && vcPlant != "")
            {
                sql.AppendLine(" and vcPlant='" + vcPlant + "'");
            }
            try
            {
                DataTable dt = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string getTableName(string month)
        {
            if (month == null || month.ToString() == "")
                month = DateTime.Now.Month.ToString();
            if (month.ToString() == "1" || month.ToString() == "01")
                return "tJanuary";
            else if (month.ToString() == "2" || month.ToString() == "02")
                return "tFebruary";
            else if (month.ToString() == "3" || month.ToString() == "03")
                return "tMarch";
            else if (month.ToString() == "4" || month.ToString() == "04")
                return "tApril";
            else if (month.ToString() == "5" || month.ToString() == "05")
                return "tMay";
            else if (month.ToString() == "6" || month.ToString() == "06")
                return "tJune";
            else if (month.ToString() == "7" || month.ToString() == "07")
                return "tJuly";
            else if (month.ToString() == "8" || month.ToString() == "08")
                return "tAugust";
            else if (month.ToString() == "9" || month.ToString() == "09")
                return "tSeptember";
            else if (month.ToString() == "10")
                return "tOctober";
            else if (month.ToString() == "11")
                return "tNovember";
            else if (month.ToString() == "12")
                return "tDecember";
            else return "";
        }

        /// <summary>
        /// 获取类别列表
        /// </summary>
        public DataTable bindPartType()
        {
            string ssql = "select '' as vcValue, '' as vcName union all select vcData1, vcData2 from ConstMst where vcDataId='KBpartType'";
            return excute.ExcuteSqlWithSelectToDT(ssql);
        }

        /// <summary>
        /// 生产部署列表
        /// </summary>
        public DataTable bindProType()
        {
            string ssql = "select '' as vcValue,'' as vcName union all select distinct vcData1, vcData1 from ConstMst where vcDataId='ProType' ";
            return excute.ExcuteSqlWithSelectToDT(ssql);
        }

        /// <summary>
        /// 根据生产部署获取组别列表
        /// </summary>
        /// <param name="protype">生产部署</param>
        public DataTable bindZB(string protype)
        {
            string ssql = " select '' as vcValue,'' as vcName union all select distinct vcData3,vcData3 from ConstMst where vcDataID='ProType'" +
                " and vcData1='" + protype + "' order by vcName ";
            return excute.ExcuteSqlWithSelectToDT(ssql);
        }

        public DataTable bindProType(string Calendartype)
        {
            string ssql = " select '' as vcGC union all ";
            ssql += " select t1.vcData1 as vcGC from ";
            ssql += " (select distinct vcData1, vcDataID from ConstMst where vcDataID='ProType') t1 ";
            ssql += " left join ";
            ssql += " (select vcData1, vcData10 from ConstMst) t2 ";
            ssql += "  on t1.vcDataID = t2.vcData10 ";
            ssql += " ";
            ssql += " where t2.vcData1 = '" + Calendartype + "' order by vcData1";
            try
            {
                return excute.ExcuteSqlWithSelectToDT(ssql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable BindPlant(int type = 1)
        {
            string sql;
            if (type == 1)
            {
                sql = " select '' as vcValue,'' as vcName union all select distinct vcData1,vcData2 from ConstMst where vcDataID='KBPlant' ";
            }
            else
            {
                sql = " select vcData1 as vcValue,vcData2 as vcName from ConstMst where vcDataId='KBPlant' ";
            }
            return excute.ExcuteSqlWithSelectToDT(sql);
        }


        public void UpdateTable(DataTable dtrel, string Month)
        {
            string year = Month.Split('-')[0];
            string month = Month.Split('-')[1];
            try
            {
                string sql = "select distinct vcData1,vcData2 from ConstMst where vcDataID='KBPlant' ";
                DataSet dataSet = excute.ExcuteSqlWithSelectToDS(sql);
                DataTable dtplant = dataSet.Tables[0];
                string UpTable = MonSQL(month);//获取数字月份对应的数据库表名
                SqlCommand cmd = new SqlCommand(" select * from " + UpTable, new SqlConnection(ComConnectionHelper.GetConnectionString()));
                SqlDataAdapter ss = new SqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                ss.Fill(dt);
                DataTable dtupdate = dt.Clone();
                for (int j = 0; j < dtrel.Rows.Count; j++)
                {
                    int num = 0;
                    if (dtrel.Rows[j][0].ToString().Trim().Length == 0 && dtrel.Rows[j][1].ToString().Trim().Length == 0 && dtrel.Rows[j][2].ToString().Trim().Length == 0)
                        continue;
                    dtupdate.Rows.Add(dtupdate.NewRow());
                    dtupdate.Rows[j][1] = dtplant.Select("vcData2='" + dtrel.Rows[j][1].ToString() + "'")[0]["vcData1"].ToString();
                    dtupdate.Rows[j][2] = year;
                    dtupdate.Rows[j][3] = month;
                    if (dtrel.Rows[j][2].ToString().Trim().Length == 0)
                    {
                        dtrel.Rows[j][2] = dtrel.Rows[j][0];
                        dtrel.Rows[j][3] = dtrel.Rows[j][1];
                    }
                    for (int i = 4; i < dtupdate.Columns.Count - 1; i++)
                    {

                        dtupdate.Rows[j][i] = dtrel.Rows[j][i - 2].ToString().Trim();
                        if (Regex.IsMatch(dtrel.Rows[j][i - 2].ToString().Trim(), "(M|N)(A|B)") && i > 5)//刘刚
                            num++;
                    }
                    dtupdate.Rows[j]["total"] = num;
                }
                dtupdate.PrimaryKey = new DataColumn[] { dtupdate.Columns["vcYear"], dtupdate.Columns["vcMonth"], dtupdate.Columns["vcGC"], dtupdate.Columns["vcZB"] };
                for (int i = 0; i < dtupdate.Rows.Count; i++)
                {
                    string a0 = dtupdate.Rows[i]["vcYear"].ToString();
                    string a1 = dtupdate.Rows[i]["vcMonth"].ToString();
                    string a2 = dtupdate.Rows[i]["vcGC"].ToString();
                    string a3 = dtupdate.Rows[i]["vcZB"].ToString();
                    DataRow[] dr = dt.Select(" vcYear ='" + a0 + "' and vcMonth='" + a1 + "' and vcGC='" + a2 + "' and vcZB='" + a3 + "' ");
                    if (dr.Length > 0)
                    {
                        for (int j = 1; j < dt.Columns.Count; j++)
                        {
                            dr[0][j] = dtupdate.Rows[i][j];
                        }
                    }
                    else
                    {
                        dt.Rows.Add(dt.NewRow());
                        for (int j = 1; j < dt.Columns.Count; j++)
                        {
                            dt.Rows[dt.Rows.Count - 1][j] = dtupdate.Rows[i][j];
                        }
                    }
                }
                SqlCommandBuilder cmdBud = new SqlCommandBuilder(ss);
                ss.UpdateCommand = cmdBud.GetUpdateCommand();
                ss.Update(dt);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getConst()
        {
            string ssql = "SELECT vcDataId, vcDataName, vcData1, vcData2, vcData3, vcData4, vcData5, vcData6, vcData7, vcData8," +
                " vcData9, vcData10, vcCreateUserId, dCreateTime, vcUpdateUserId, dUpdateTime FROM ConstMst" +
                " where vcDataid='ProType' or vcDataid='CalendarType' or vcDataid='KBPlant' ";
            return excute.ExcuteSqlWithSelectToDT(ssql);
        }

        public void UpdateCalendar(string plant, string gc, string zb, string year, string month, string[] data)
        {
            try
            {
                string UpTable = MonSQL(month);
                SqlCommand cmd = new SqlCommand(" select * from " + UpTable, new SqlConnection(ComConnectionHelper.GetConnectionString()));
                SqlDataAdapter ss = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                ss.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    if (dt.Select(" vcYear='" + year + "' and vcMonth='" + month + "' and vcGC='" + gc + "' and vcZB='" + zb + "' ").Length > 0)
                        dt = dt.Select(" vcYear='" + year + "' and vcMonth='" + month + "' and vcGC='" + gc + "' and vcZB='" + zb + "' ").CopyToDataTable();
                    else
                    {
                        dt.Rows.Add(dt.NewRow());
                    }
                }
                else dt.Rows.Add(dt.NewRow());
                for (int i = 1; i < dt.Columns.Count; i++)
                {
                    dt.Rows[dt.Rows.Count - 1][i] = DBNull.Value;
                }
                SqlCommandBuilder cmdBud = new SqlCommandBuilder(ss);
                dt.Rows[dt.Rows.Count - 1]["vcPlant"] = plant;
                dt.Rows[dt.Rows.Count - 1]["vcYear"] = year;
                dt.Rows[dt.Rows.Count - 1]["vcMonth"] = month;
                dt.Rows[dt.Rows.Count - 1]["vcGC"] = gc;
                dt.Rows[dt.Rows.Count - 1]["vcZB"] = zb;
                dt.Rows[dt.Rows.Count - 1]["total"] = data.Length - 1;
                for (int i = 0; i < data.Length - 1; i++)
                {
                    string col = data[i].Split('~')[1].ToString().Trim();
                    string value = data[i].Split('~')[0].ToString().Trim();
                    byte[] b = System.Text.Encoding.UTF8.GetBytes(value);
                    dt.Rows[dt.Rows.Count - 1][col] = value;
                }
                dt.PrimaryKey = new DataColumn[] { dt.Columns["vcYear"], dt.Columns["vcMonth"], dt.Columns["vcGC"], dt.Columns["vcZB"] }; //设置主键
                ss.Update(dt);
                dt.AcceptChanges();
                ss.Dispose();
                cmdBud.Dispose();
            }
            catch(Exception ex)
            {
                throw ex;
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
    }
}
