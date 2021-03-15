using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Logic;
/// <summary>
/// NQC内外合状态获取_EKANBAN
/// </summary>
namespace BatchProcess
{
    public class FP0021
    {
        private MultiExcute excute = new MultiExcute();

        #region 主方法
        public bool main(string strUserId, string plant)
        {
            string PageId = "FP0021";
            try
            {
                //批处理开始
                ComMessage.GetInstance().ProcessMessage(PageId, "M00PI2100", null, strUserId);
                bool flag = isExist();
                if (!flag)
                {
                    getModify(DateTime.Now, strUserId);

                }

                //王方方法
                {

                    FS0612_Logic fs0612_Logic = new FS0612_Logic();
                    FS0630_Logic fs0630_Logic = new FS0630_Logic();

                    string vcCLYM = DateTime.Now.ToString("yyyyMM");
                    string vcDXYM = DateTime.Now.AddMonths(1).ToString("yyyyMM");
                    string vcNSYM = DateTime.Now.AddMonths(2).ToString("yyyyMM");
                    string vcNNSYM = DateTime.Now.AddMonths(3).ToString("yyyyMM");

                    DataTable dtNQCResult = fs0612_Logic.dtNQCReceive(vcCLYM);

                    DataRow[] drs_dxny_a = dtNQCResult.Select("Process_Factory='TFTM" + plant + "' and Start_date_for_daily_qty like '" + vcDXYM + "%'  ");
                    DataRow[] drs_nsny_a = dtNQCResult.Select("Process_Factory='TFTM" + plant + "' and Start_date_for_daily_qty like '" + vcNSYM + "%'  ");
                    DataRow[] drs_nnsny_a = dtNQCResult.Select("Process_Factory='TFTM" + plant + "' and Start_date_for_daily_qty like '" + vcNNSYM + "%'  ");

                    if (drs_dxny_a.Length == 0)
                    {
                        ComMessage.GetInstance().ProcessMessage(PageId, string.Format("工厂{0},对象年月{1}内制结果未处理。", plant, vcDXYM), null, strUserId);
                        return true;
                    }
                    if (drs_nsny_a.Length == 0)
                    {
                        ComMessage.GetInstance().ProcessMessage(PageId, string.Format("工厂{0},对象年月{1}内制结果未处理。", plant, vcNSYM), null, strUserId);
                        return true;
                    }
                    if (drs_nnsny_a.Length == 0)
                    {
                        ComMessage.GetInstance().ProcessMessage(PageId, string.Format("工厂{0},对象年月{1}内制结果未处理。", plant, vcNNSYM), null, strUserId);
                        return true;
                    }

                    DataTable dtSOQReply = fs0630_Logic.GetSOQReply(vcCLYM, "1");
                    DataRow[] drs_dxny_b = dtSOQReply.Select("vcFZGC='" + plant + "' and vcDXYM='" + vcDXYM + "'  ");
                    DataRow[] drs_nsny_b = dtSOQReply.Select("vcFZGC='" + plant + "' and vcDXYM='" + vcNSYM + "'  ");
                    DataRow[] drs_nnsny_b = dtSOQReply.Select("vcFZGC='" + plant + "' and vcDXYM='" + vcNNSYM + "'  ");
                    if (drs_dxny_b.Length == 0)
                    {
                        ComMessage.GetInstance().ProcessMessage(PageId, string.Format("工厂{0},对象年月{1}外注平准化数据没有展开。", plant, vcDXYM), null, strUserId);
                        return true;
                    }
                    if (drs_nsny_b.Length == 0)
                    {
                        ComMessage.GetInstance().ProcessMessage(PageId, string.Format("工厂{0},内示年月{1}外注平准化数据没有展开。", plant.ToString(), vcNSYM), null, strUserId);
                        return true;
                    }
                    if (drs_nnsny_b.Length == 0)
                    {
                        ComMessage.GetInstance().ProcessMessage(PageId, string.Format("工厂{0},内示年月{1}外注平准化数据没有展开。", plant.ToString(), vcNNSYM), null, strUserId);
                        return true;
                    }

                    //判断最大次数合算能不能再次请求，如果请求了但是没有回复则不能再次请求
                    DataTable dtMaxCLResult = fs0612_Logic.GetMaxCLResult2(vcCLYM);

                    DataRow[] drs_dxny_c = dtMaxCLResult.Select("vcPlant='" + plant + "' and vcStatus='已请求'  ");
                    if (drs_dxny_c.Length > 0)
                    {
                        ComMessage.GetInstance().ProcessMessage(PageId, string.Format("工厂{0}不能再次请求。", plant), null, strUserId);
                        return true;
                    }

                    List<string> plantList = null;

                    plantList.Add(plant);

                    fs0612_Logic.CreateView2(vcCLYM, plantList, strUserId);
                }


                //批处理结束
                ComMessage.GetInstance().ProcessMessage(PageId, "M00PI2101", null, strUserId);
                return true;
            }
            catch (Exception ex)
            {
                //批处理异常结束
                ComMessage.GetInstance().ProcessMessage(PageId, "M00PE2102", null, strUserId);
                throw ex;
            }
        }
        #endregion

        #region 是否需要执行生成工作

        public bool isExist()
        {
            StringBuilder sbr = new StringBuilder();
            sbr.AppendLine("SELECT * FROM dbo.TSoqDayChange WHERE vcChangeNo =  '" + DateTime.Now.ToString("yyyyMMdd") + "'");
            DataTable dt = excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            return dt.Rows.Count > 0;
        }

        #endregion

        #region 生成change数据
        public void getModify(DateTime DXR, string userId)
        {
            try
            {
                DataTable dt = getCalendar(DXR);
                int count = getCountDay();
                DataRow[] rowIn = dt.Select("Flag = '0'");
                DataRow[] rowOut = dt.Select("Flag = '1'");

                DataTable dtIn = ToDataTable(rowIn);
                DataTable dtOut = ToDataTable(rowOut);

                Hashtable hsIn = getDay(dtIn, DXR, count);
                Hashtable hsOut = getDay(dtOut, DXR, count);
                StringBuilder sbr = new StringBuilder();


                foreach (string key in hsIn.Keys)
                {
                    string YMD = hsIn[key].ToString();
                    string ym = YMD.Substring(0, 6);
                    string day = YMD.Substring(6, 2);
                    if (day[0] == '0')
                        day = day[1].ToString();
                    if (sbr.Length > 0)
                    {
                        sbr.AppendLine("union all");
                    }
                    sbr.AppendLine("SELECT vcPart_id,vcDXYM,iD" + day + " AS DayNum,'" + YMD + "' as DXR  FROM TSoqReply");
                    sbr.AppendLine("WHERE vcInOutFlag = '0' AND vcMakingOrderType in (" + getTypeMethod("D") + ")");
                    sbr.AppendLine("AND vcDXYM = '" + ym + "' AND vcFZGC = '" + key + "'");
                }

                foreach (string key in hsOut.Keys)
                {
                    string YMD = hsOut[key].ToString();
                    string ym = YMD.Substring(0, 6);
                    string day = YMD.Substring(6, 2);
                    if (day[0] == '0')
                        day = day[1].ToString();
                    if (sbr.Length > 0)
                    {
                        sbr.AppendLine("union all");
                    }
                    sbr.AppendLine("SELECT vcPart_id,vcDXYM,iD" + day + " AS DayNum,'" + YMD + "' as DXR  FROM TSoqReply");
                    sbr.AppendLine("WHERE vcInOutFlag = '1' AND vcMakingOrderType in (" + getTypeMethod("D") + ")");
                    sbr.AppendLine("AND vcDXYM = '" + ym + "' AND vcFZGC = '" + key + "'");
                }

                if (sbr.Length > 0)
                {
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine("select DXR,'" + DateTime.Now.ToString("yyyyMMdd") + "' as vcChangeNo,vcPart_id,ISNULL(DayNum,0) AS iQuantityBefore,ISNULL(DayNum,0) AS iQuantityNow,GETDATE() AS dFileUpload,'" + userId + "' AS vcOperatorID,GETDATE() AS dOperatorTime  from (");
                    sql.AppendLine(sbr.ToString());
                    sql.AppendLine(") a");

                    StringBuilder res = new StringBuilder();
                    res.AppendLine("INSERT INTO dbo.TSoqDayChange(vcDXDate, vcChangeNo, vcPart_Id, iQuantityBefore, iQuantityNow, dFileUpload, vcOperatorID, dOperatorTime)");
                    res.AppendLine(sql.ToString());
                    excute.ExcuteSqlWithStringOper(res.ToString());
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable getCalendar(DateTime DXR)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine("DECLARE @YM VARCHAR(6)");
                sbr.AppendLine("DECLARE @YM1 VARCHAR(6)");
                sbr.AppendLine("DECLARE @YM2 VARCHAR(6)");
                sbr.AppendLine("SET @YM = CONVERT(VARCHAR(6),DATEADD(m,0,'" + DXR + "'),112)");
                sbr.AppendLine("SET @YM1 = CONVERT(VARCHAR(6),DATEADD(m,1,'" + DXR + "'),112)");
                sbr.AppendLine("SET @YM2 = CONVERT(VARCHAR(6),DATEADD(m,2,'" + DXR + "'),112)");
                //sbr.AppendLine("SELECT * FROM TCalendar_PingZhun_Nei WHERE TARGETMONTH IN (@YM,@YM1,@YM2)");
                sbr.AppendLine("SELECT * FROM (");
                sbr.AppendLine("SELECT *,'0' AS Flag FROM TCalendar_PingZhun_Nei WHERE TARGETMONTH IN (@YM,@YM1,@YM2)");
                sbr.AppendLine("UNION all");
                sbr.AppendLine("SELECT *,'1' AS Flag FROM TCalendar_PingZhun_Wai WHERE TARGETMONTH IN (@YM,@YM1,@YM2)");
                sbr.AppendLine(") a");
                sbr.AppendLine("ORDER BY vcFZGC,TARGETMONTH");
                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int getCountDay()
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine("SELECT vcValue1 FROM TOutCode WHERE vcCodeId = 'C031' AND vcIsColum = '0'");
                DataTable dt = excute.ExcuteSqlWithSelectToDT(sbr.ToString());
                return Convert.ToInt32(dt.Rows[0]["vcValue1"].ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Hashtable getDay(DataTable dt, DateTime temp, int dayCount)
        {
            try
            {
                Hashtable hashtable = new Hashtable();
                List<Node> list = new List<Node>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string month = dt.Rows[i]["TARGETMONTH"].ToString();
                    string day1 = dt.Rows[i]["TARGETDAY1"].ToString();
                    string day2 = dt.Rows[i]["TARGETDAY2"].ToString();
                    string day3 = dt.Rows[i]["TARGETDAY3"].ToString();
                    string day4 = dt.Rows[i]["TARGETDAY4"].ToString();
                    string day5 = dt.Rows[i]["TARGETDAY5"].ToString();
                    string day6 = dt.Rows[i]["TARGETDAY6"].ToString();
                    string day7 = dt.Rows[i]["TARGETDAY7"].ToString();
                    string day8 = dt.Rows[i]["TARGETDAY8"].ToString();
                    string day9 = dt.Rows[i]["TARGETDAY9"].ToString();
                    string day10 = dt.Rows[i]["TARGETDAY10"].ToString();
                    string day11 = dt.Rows[i]["TARGETDAY11"].ToString();
                    string day12 = dt.Rows[i]["TARGETDAY12"].ToString();
                    string day13 = dt.Rows[i]["TARGETDAY13"].ToString();
                    string day14 = dt.Rows[i]["TARGETDAY14"].ToString();
                    string day15 = dt.Rows[i]["TARGETDAY15"].ToString();
                    string day16 = dt.Rows[i]["TARGETDAY16"].ToString();
                    string day17 = dt.Rows[i]["TARGETDAY17"].ToString();
                    string day18 = dt.Rows[i]["TARGETDAY18"].ToString();
                    string day19 = dt.Rows[i]["TARGETDAY19"].ToString();
                    string day20 = dt.Rows[i]["TARGETDAY20"].ToString();
                    string day21 = dt.Rows[i]["TARGETDAY21"].ToString();
                    string day22 = dt.Rows[i]["TARGETDAY22"].ToString();
                    string day23 = dt.Rows[i]["TARGETDAY23"].ToString();
                    string day24 = dt.Rows[i]["TARGETDAY24"].ToString();
                    string day25 = dt.Rows[i]["TARGETDAY25"].ToString();
                    string day26 = dt.Rows[i]["TARGETDAY26"].ToString();
                    string day27 = dt.Rows[i]["TARGETDAY27"].ToString();
                    string day28 = dt.Rows[i]["TARGETDAY28"].ToString();
                    string day29 = dt.Rows[i]["TARGETDAY29"].ToString();
                    string day30 = dt.Rows[i]["TARGETDAY30"].ToString();
                    string day31 = dt.Rows[i]["TARGETDAY31"].ToString();
                    Hashtable hs = new Hashtable();
                    hs.Add(month + "01", day1);
                    hs.Add(month + "02", day2);
                    hs.Add(month + "03", day3);
                    hs.Add(month + "04", day4);
                    hs.Add(month + "05", day5);
                    hs.Add(month + "06", day6);
                    hs.Add(month + "07", day7);
                    hs.Add(month + "08", day8);
                    hs.Add(month + "09", day9);
                    hs.Add(month + "10", day10);
                    hs.Add(month + "11", day11);
                    hs.Add(month + "12", day12);
                    hs.Add(month + "13", day13);
                    hs.Add(month + "14", day14);
                    hs.Add(month + "15", day15);
                    hs.Add(month + "16", day16);
                    hs.Add(month + "17", day17);
                    hs.Add(month + "18", day18);
                    hs.Add(month + "19", day19);
                    hs.Add(month + "20", day20);
                    hs.Add(month + "21", day21);
                    hs.Add(month + "22", day22);
                    hs.Add(month + "23", day23);
                    hs.Add(month + "24", day24);
                    hs.Add(month + "25", day25);
                    hs.Add(month + "26", day26);
                    hs.Add(month + "27", day27);
                    hs.Add(month + "28", day28);
                    hs.Add(month + "29", day29);
                    hs.Add(month + "30", day30);
                    hs.Add(month + "31", day31);

                    string plant = dt.Rows[i]["vcFZGC"].ToString();
                    bool flag = false;
                    for (int j = 0; j < list.Count; j++)
                    {
                        if (list[j].Exist(plant))
                        {
                            list[j].addRange(hs);
                            flag = true;
                            break;
                        }
                    }

                    if (!flag)
                    {
                        list.Add(new Node(plant, hs));
                    }
                }

                for (int i = 0; i < list.Count; i++)
                {
                    DateTime time = temp;
                    string key = list[i].plant;
                    string value = "";
                    int count = 0;
                    while (count < dayCount)
                    {
                        time = time.AddDays(1);
                        string tmp = time.ToString("yyyyMMdd");
                        string state = list[i].day[tmp].ToString();
                        if (!state.Equals("0") && !string.IsNullOrWhiteSpace(state))
                        {
                            count++;
                        }
                    }

                    string resTime = time.ToString("yyyyMMdd");
                    hashtable.Add(key, resTime);
                }


                return hashtable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public class Node
        {
            public Node(string plant, Hashtable hs)
            {
                this.plant = plant;
                this.day = hs;
            }

            public bool Exist(string plant)
            {
                if (this.plant.Equals(plant))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public void addRange(Hashtable hs)
            {
                foreach (string key in hs.Keys)
                {
                    this.day.Add(key, hs[key]);
                }
            }
            public string plant;
            public Hashtable day;
        }
        public string getTypeMethod(string vcType)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine("SELECT vcValue FROM TCode WHERE iAutoId IN(");
                sbr.AppendLine("SELECT vcOrderGoodsId FROM TOrderGoodsAndDifferentiation WHERE vcOrderDifferentiationId in");
                sbr.AppendLine("(SELECT iAutoId FROM TOrderDifferentiation WHERE vcOrderInitials = '" + vcType + "')) ");

                string res = "";

                DataTable dt = excute.ExcuteSqlWithSelectToDT(sbr.ToString());
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(res))
                        {
                            res += ",";
                        }
                        res += "'" + dt.Rows[i]["vcValue"].ToString() + "'";
                    }
                }

                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        public DataTable ToDataTable(DataRow[] rows)
        {
            if (rows == null || rows.Length == 0)
                return null;
            DataTable tmp = rows[0].Table.Clone();
            foreach (DataRow dataRow in rows)
            {
                tmp.Rows.Add(dataRow.ItemArray);
            }

            return tmp;
        }
    }
}
