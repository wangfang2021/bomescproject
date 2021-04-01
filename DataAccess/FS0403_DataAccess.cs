using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using System.Data;
using System.Drawing;
using System.Security.Policy;
using System.Text;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace DataAccess
{
    public class FS0403_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 检索

        public DataTable searchApi(string changeNo, string state, string orderNo)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine("SELECT * FROM");
                sbr.AppendLine("(");
                sbr.AppendLine("SELECT CAST(vcDXDate AS DATETIME) AS vcDXDate,vcOrderNo,vcChangeNo,MAX(dFileUpload) AS dFileUpload,CASE ISNULL(vcOrderNo,'') WHEN '' THEN '订单待上传' ELSE '订单已上传' END AS state  FROM TSoqDayChange GROUP BY vcDXDate,vcOrderNo,vcChangeNo");
                sbr.AppendLine(") a");
                sbr.AppendLine("WHERE 1=1 ");
                if (!string.IsNullOrWhiteSpace(state))
                {
                    sbr.AppendLine("AND a.state = '" + state + "'");
                }
                if (!string.IsNullOrWhiteSpace(changeNo))
                {
                    sbr.AppendLine("AND a.vcChangeNo LIKE '" + changeNo + "%'");
                }
                if (!string.IsNullOrWhiteSpace(orderNo))
                {
                    sbr.AppendLine("AND a.vcOrderNo LIKE '" + orderNo + "%'");
                }

                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable downLoadApi(string vcChangeNo)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine("SELECT vcDXDate, vcOrderNo, vcChangeNo, vcPart_Id, iQuantityBefore, iQuantityNow, dFileUpload, vcOperatorID, dOperatorTime  FROM TSoqDayChange WHERE vcChangeNo = '" + vcChangeNo + "'");
                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 导入d+n日次变更

        #region 计算n个工作日各工厂稼动日
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
                        string state = ObjToString(list[i].day[tmp]);
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

        #endregion

        #region 获取品番当月每天的订单数量

        public Hashtable getCount(Hashtable ht, string inOut)
        {
            try
            {
                Hashtable res = new Hashtable();
                string choose = "";
                foreach (string htKey in ht.Keys)
                {
                    string plant = htKey;
                    string DXYM = ht[htKey].ToString().Substring(0, 6);
                    DateTime t2;
                    //DateTime.TryParseExact(DXYM + "01", "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out t2);
                    t2 =
                        DateTime.Parse(DXYM.Substring(0, 4) + "-" + DXYM.Substring(4, 2) + "-01");
                    t2 = t2.AddMonths(-1);
                    string CLYM = t2.ToString("yyyyMM");
                    string tmp = " (vcDXYM = '" + DXYM + "' AND vcFZGC = '" + plant + "' AND vcCLYM = '" + CLYM + "') ";
                    if (!string.IsNullOrWhiteSpace(choose))
                    {
                        choose = choose + "OR" + tmp;
                    }
                    else
                    {
                        choose = tmp;
                    }
                }

                if (!string.IsNullOrWhiteSpace(choose))
                {
                    choose = "AND (" + choose + ")";
                }

                StringBuilder sbr = new StringBuilder();

                //TODO 应修改日度订货次数
                //sbr.AppendLine("SELECT * FROM TSoqReply WHERE vcMakingOrderType = '3' " + choose);
                sbr.AppendLine("SELECT * FROM TSoqReply WHERE 1=1 and vcMakingOrderType = '3' AND vcInOutFlag = '" + inOut + "' " + choose);
                DataTable dt = excute.ExcuteSqlWithSelectToDT(sbr.ToString());


                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string vcFZGC = dt.Rows[i]["vcFZGC"].ToString();
                    string vcPart_id = dt.Rows[i]["vcPart_id"].ToString();
                    string day = ht[vcFZGC].ToString().Substring(6, 2);

                    DataRow[] rows = dt.Select("vcFZGC = '" + vcFZGC + "' and vcPart_id = '" + vcPart_id + "'");
                    if (rows.Length > 0)
                    {
                        PartNode node = new PartNode();
                        node.DXR = ht[vcFZGC].ToString();
                        if (!string.IsNullOrWhiteSpace(rows[0]["iD" + day.TrimStart('0')].ToString()))
                        {
                            node.quantity = Convert.ToInt32(rows[0]["iD" + day.TrimStart('0')].ToString());
                        }

                        node.iSRS = Convert.ToInt32(rows[0]["iQuantityPercontainer"]);
                        res.Add(vcPart_id, node);
                    }
                }


                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public class PartNode
        {
            public PartNode()
            {
                this.quantity = 0;
            }

            public int quantity;
            public int iSRS;
            public string DXR;
        }

        #endregion

        #region 获取波动率

        public Hashtable getFluctuate()
        {
            try
            {
                Hashtable ht = new Hashtable();
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine("SELECT vcPartNo,vcFluctuationRange FROM TDaysChangeOrdersBaseData a left join TGroup b on a.vcGroupId = b.iAutoId");
                DataTable dt = excute.ExcuteSqlWithSelectToDT(sbr.ToString());

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string vcPartNo = dt.Rows[i]["vcPartNo"].ToString();
                    if (!ht.Contains(vcPartNo))
                    {
                        ht.Add(vcPartNo, getFloat(dt.Rows[i]["vcFluctuationRange"].ToString()));
                    }
                }

                return ht;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 修改soqReply并导入履历表

        public void ChangeSoq(List<PartIDNode> list, string strUserId)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine("DECLARE @now DATETIME");
                sbr.AppendLine("SET @now = GETDATE()");

                for (int i = 0; i < list.Count; i++)
                {
                    string CLYM = DateTime.Parse(list[i].DXR.Substring(0, 4) + "/" + list[i].DXR.Substring(4, 2) + "/01").AddMonths(-1).ToString("yyyyMM");

                    sbr.AppendLine("INSERT INTO TSoqDayChange(vcDXDate,vcChangeNo, vcPart_Id, iQuantityBefore, iQuantityNow, dFileUpload, vcOperatorID, dOperatorTime)");
                    sbr.AppendLine("VALUES('" + list[i].DXR + "','" + list[i].ChangeNo + "','" + list[i].partId + "'," + list[i].iQuantityBefore + "," + list[i].IQuantityNow + ",@now,'" + strUserId + "'  ,GETDATE());");
                    sbr.AppendLine("UPDATE TSoqReply SET iD" + Convert.ToInt32(list[i].DXR.Substring(6, 2)) + " = " + list[i].IQuantityNow + " ,vcOperatorID = '" + strUserId + "' ,dOperatorTime = GETDATE() WHERE vcDXYM = '" + list[i].DXR.Substring(0, 6) + "' AND vcPart_id = '" + list[i].partId + "' AND vcCLYM = '" + CLYM + "';");
                    sbr.AppendLine("UPDATE TSoqReply SET iPartNums = ISNULL(iD1,0)+ISNULL(iD2,0)+ISNULL(iD3,0)+ISNULL(iD4,0)+ISNULL(iD5,0)+ISNULL(iD6,0)+ISNULL(iD7,0)+ISNULL(iD8,0)+ISNULL(iD9,0)+ISNULL(iD10,0)+ISNULL(iD11,0)+ISNULL(iD12,0)+ISNULL(iD13,0)+ISNULL(iD14,0)+ISNULL(iD15,0)+ISNULL(iD16,0)+ISNULL(iD17,0)+ISNULL(iD18,0)+ISNULL(iD19,0)+ISNULL(iD20,0)+ISNULL(iD21,0)+ISNULL(iD22,0)+ISNULL(iD23,0)+ISNULL(iD24,0)+ISNULL(iD25,0)+ISNULL(iD26,0)+ISNULL(iD27,0)+ISNULL(iD28,0)+ISNULL(iD29,0)+ISNULL(iD30,0)+ISNULL(iD31,0)");
                    sbr.AppendLine(",vcOperatorID = '" + strUserId + "' ,dOperatorTime = GETDATE() WHERE vcDXYM = '" + list[i].DXR.Substring(0, 6) + "' AND vcPart_id = '" + list[i].partId + "' AND vcCLYM = '" + CLYM + "';");
                    sbr.AppendLine("UPDATE TSoqReply SET iBoxes = CEILING(iPartNums*1.0/iQuantityPercontainer*1.0),vcOperatorID = '" + strUserId + "' ,dOperatorTime = GETDATE()");
                    sbr.AppendLine("WHERE vcDXYM = '" + list[i].DXR.Substring(0, 6) + "' AND vcPart_id = '" + list[i].partId + "' AND vcCLYM = '" + CLYM + "';");
                }

                excute.ExcuteSqlWithStringOper(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region string To Float

        public Decimal getFloat(string str)
        {
            try
            {
                return Convert.ToDecimal(str);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        #endregion

        #region 品番完整类

        public class PartIDNode
        {
            public string partId;//品番
            public int IQuantityNow;//修改后数量
            public int iQuantityBefore;//修改前数量
            public decimal allowPercent;//允许波动率
            public string DXR;//对象日
            public string ChangeNo;//变更号
            public bool flag;//是否允许存在
            public string message;//错误记录
            //public decimal realPercent;//变更波动率
            public int iSRS;
            public PartIDNode(string partId, int excelQuantity, int soqQuantity, string allowPercent, string DXR, string ChangeNo, int iSRS)
            {
                try
                {
                    this.partId = partId;
                    this.flag = true;
                    this.DXR = DXR;
                    this.iSRS = iSRS;
                    this.IQuantityNow = excelQuantity;
                    this.message = "";
                    if (soqQuantity == -1 || string.IsNullOrWhiteSpace(DXR))
                    {
                        this.message += "对象日Soq不存在";
                        this.flag = false;
                    }

                    if (string.IsNullOrWhiteSpace(allowPercent))
                    {
                        this.message += "波动率不存在";
                        this.flag = false;
                    }

                    this.iQuantityBefore = soqQuantity;
                    this.allowPercent = ObjToDecimal(allowPercent);
                    this.ChangeNo = ChangeNo;

                    //波动率范围
                    int max = 0;
                    int min = 0;
                    //TODO 当日订货数量为0是否要置为1？
                    //iQuantityBefore = iQuantityBefore == 0 ? 1 : iQuantityBefore;

                    max = (int)Math.Floor(iQuantityBefore * (1 + this.allowPercent / 100));
                    min = (int)Math.Ceiling(iQuantityBefore * (1 - this.allowPercent / 100));
                    min = min < 0 ? 0 : min;

                    if (IQuantityNow > max)
                    {
                        IQuantityNow = max;
                    }
                    else if (IQuantityNow < min)
                    {
                        IQuantityNow = min;
                    }

                    int tmp = IQuantityNow % iSRS;
                    if (tmp > 0)
                    {
                        IQuantityNow = ((IQuantityNow / iSRS) + 1) * iSRS;
                    }
                }
                catch (Exception ex)
                {
                    this.partId = partId;
                    this.message = "数据错误";
                    this.flag = false;
                }

            }

        }

        public static int ObjToInt(Object obj)
        {
            try
            {
                return Convert.ToInt32(obj);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public static decimal ObjToDecimal(Object obj)
        {
            try
            {
                return Convert.ToDecimal(obj);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        #endregion

        #endregion

        #region 查看是否可以上传变更

        public bool isUpload()
        {
            try
            {
                DateTime t2;

                //DateTime.TryParseExact(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 10:00:00", "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out t2);
                t2 = DateTime.Parse(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 10:00:00");
                if (DateTime.Now <= t2)
                {
                    StringBuilder sbr = new StringBuilder();
                    string time = DateTime.Now.ToString("yyyyMMdd");
                    sbr.AppendLine("SELECT * WHERE vcChangeNo = '" + time + "'");
                    DataTable dt = excute.ExcuteSqlWithSelectToDT(sbr.ToString());
                    if (dt.Rows.Count > 0)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        public DataTable getModify(DateTime DXR)
        {
            try
            {
                DataTable dt = getCalendar(DXR);
                int count = getCountDay();
                DataRow[] rowIn = dt.Select("Flag = '0'");
                DataRow[] rowOut = dt.Select("Flag = '1'");

                DataTable dtIn = ToDataTable(rowIn);
                DataTable dtOut = ToDataTable(rowOut);

                Hashtable hsIntmp = getDay(dtIn, DXR, count);
                Hashtable hsOuttmp = getDay(dtOut, DXR, count);

                Hashtable hsIn = new Hashtable();
                Hashtable hsOut = new Hashtable();

                foreach (string dayOutKey in hsOuttmp.Keys)
                {
                    hsOut.Add(dayOutKey, hsOuttmp["2"]);
                    hsIn.Add(dayOutKey, hsOuttmp["2"]);

                }
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
                    sbr.AppendLine("SELECT vcPart_id,vcDXYM,iD" + day + " AS DayNum,'" + day + "' as DXR  FROM TSoqReply");
                    sbr.AppendLine("WHERE vcInOutFlag = '0' AND vcMakingOrderType in (" + getTypeMethod("D") + ")");
                    sbr.AppendLine("AND vcDXYM = '" + ym + "' AND vcFZGC = '" + key + "'");
                }

                foreach (string key in hsOut.Keys)
                {
                    string YMD = hsOut[key].ToString();
                    string ym = YMD.Substring(0, 6);
                    string day = YMD.Substring(6, 2);
                    string CLYMD = DateTime.Parse(ym.Substring(0, 4) + "-" + ym.Substring(4, 2) + "-01").AddMonths(-1).ToString("yyyyMM");
                    if (day[0] == '0')
                        day = day[1].ToString();
                    if (sbr.Length > 0)
                    {
                        sbr.AppendLine("union all");
                    }
                    sbr.AppendLine("SELECT vcPart_id,vcDXYM,iD" + day + " AS DayNum,'" + day + "' as DXR  FROM TSoqReply");
                    sbr.AppendLine("WHERE vcInOutFlag = '1' AND vcMakingOrderType in (" + getTypeMethod("D") + ")");
                    sbr.AppendLine("AND vcDXYM = '" + ym + "' AND vcFZGC = '" + key + "' AND vcCLYM = '" + CLYMD + "'");
                }

                if (sbr.Length > 0)
                {
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine("select vcPart_id,vcDXYM,ISNULL(DayNum,0) AS DayNum,DXR  from (");
                    sql.AppendLine(sbr.ToString());
                    sql.AppendLine(") a");

                    return excute.ExcuteSqlWithSelectToDT(sql.ToString());
                }
                return new DataTable();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool isSame(DataTable dtSQL, DataTable dtFile)
        {
            return true;
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

        public string ObjToString(Object obj)
        {
            try
            {
                return obj.ToString();
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}