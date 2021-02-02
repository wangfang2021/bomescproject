using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using System.Data;
using System.Security.Policy;
using System.Text;

namespace DataAccess
{
    public class FS0403_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 计算5个工作日各工厂日期
        class Node
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

        public DataTable getCalendar()
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine("DECLARE @YM VARCHAR(6)");
                sbr.AppendLine("DECLARE @YM1 VARCHAR(6)");
                sbr.AppendLine("SET @YM = CONVERT(VARCHAR(6),GETDATE(),112)");
                sbr.AppendLine("SET @YM1 = CONVERT(VARCHAR(6),DATEADD(m,1,GETDATE()),112)");
                sbr.AppendLine("SELECT * FROM TCalendar_PingZhun_Nei WHERE TARGETMONTH IN (@YM,@YM1)");
                sbr.AppendLine("ORDER BY vcFZGC,TARGETMONTH");
                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Hashtable getDay(DataTable dt)
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
                    DateTime time = DateTime.Now;
                    string key = list[i].plant;
                    string value = "";
                    int count = 0;
                    while (count < 5)
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

        #endregion

        #region 获取品番当月每天的订单数量

        public Hashtable getCount(Hashtable ht)
        {
            try
            {
                Hashtable res = new Hashtable();
                string choose = "";
                foreach (string htKey in ht.Keys)
                {
                    string plant = htKey;
                    string DXYM = ht[htKey].ToString().Substring(0, 6);
                    string tmp = " (vcDXYM = '" + DXYM + "' AND vcFZGC = '" + plant + "') ";
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
                sbr.AppendLine("SELECT * FROM TSoqReply WHERE 1=1 " + choose);
                DataTable dt = excute.ExcuteSqlWithSelectToDT(sbr.ToString());


                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string vcFZGC = dt.Rows[i]["vcFZGC"].ToString();
                    string vcPart_id = dt.Rows[i]["vcPart_id"].ToString();
                    string YM = ht[vcFZGC].ToString().Substring(0, 6);
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
                sbr.AppendLine("SELECT vcGroupId,vcPartNo,vcFluctuationRange FROM TDaysChangeOrdersBaseData");
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
    }
}