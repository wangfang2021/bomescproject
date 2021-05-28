using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;
using Newtonsoft.Json.Linq;

namespace Logic
{
    public class FS0609_Logic
    {
        FS0609_DataAccess fs0609_DataAccess;

        public FS0609_Logic()
        {
            fs0609_DataAccess = new FS0609_DataAccess();

        }

        public JObject Search(DateTime varDxny, string varFZGC)
        {
            JObject data = new JObject { };
            string vcYM = varDxny.ToString("yyyyMM");
            string vcYM1 = varDxny.AddMonths(1).ToString("yyyyMM");
            string vcYM2 = varDxny.AddMonths(2).ToString("yyyyMM");

            DataTable re = fs0609_DataAccess.SearchData(varDxny, varFZGC);
            DataTable re1 = fs0609_DataAccess.SearchData(varDxny.AddMonths(1), varFZGC);
            DataTable re2 = fs0609_DataAccess.SearchData(varDxny.AddMonths(2), varFZGC);

            #region 第1个月
            if (re != null && re.Rows.Count > 0)
            {
                for (int j = 0; j < re.Rows.Count; j++)
                {
                    if (vcYM == re.Rows[j]["TARGETMONTH"].ToString())
                    {
                        string[] dayTempalte = new string[31];
                        string[] weekTempalte = new string[31];
                        for (int k = 0; k < re.Columns.Count - 2; k++)
                        {
                            if (!string.IsNullOrEmpty(re.Rows[j][k].ToString()))
                            {
                                //单值
                                if (re.Rows[j][k].ToString().Contains("*"))
                                {
                                    dayTempalte[k] = "单";
                                }
                                //休
                                else if (re.Rows[j][k].ToString() == "0")
                                {
                                    dayTempalte[k] = "休";
                                }
                                //双值
                                else
                                {
                                    dayTempalte[k] = "双";
                                }
                                //周数（取第一位数字）
                                weekTempalte[k] = re.Rows[j][k].ToString()[0].ToString();
                            }
                        }
                        data["dayTypeValsN"] = new JArray(dayTempalte);
                        data["weekTypeValsN"] = new JArray(weekTempalte);
                        data["totalWorkDaysN"] = re.Rows[j]["TOTALWORKDAYS"].ToString();
                    }
                }
            }
            else
            {
                string[] dayTempalte = new string[31];
                string[] weekTempalte = new string[31];
                for (int k = 1; k < 32; k++)
                {
                    string currentDate = vcYM.Substring(0, 4) + "-" + vcYM.Substring(4, 2) + "-" + k.ToString();
                    try
                    {
                        DateTime cd = Convert.ToDateTime(currentDate);
                        if (cd.DayOfWeek == DayOfWeek.Saturday || cd.DayOfWeek == DayOfWeek.Sunday)
                        {
                            dayTempalte[k - 1] = "休";
                        }
                        else
                        {
                            dayTempalte[k - 1] = "双";
                        }
                    }
                    catch
                    {
                        dayTempalte[k - 1] = "0";
                    }
                    //周数（取第一位数字）
                    weekTempalte[k - 1] = "";
                }
                data["dayTypeValsN"] = new JArray(dayTempalte);
                data["weekTypeValsN"] = new JArray(weekTempalte);
                data["totalWorkDaysN"] = "";
            }
            #endregion

            #region 第2个月
            if (re1 != null && re1.Rows.Count > 0)
            {
                for (int j = 0; j < re1.Rows.Count; j++)
                {
                    if (vcYM1 == re1.Rows[j]["TARGETMONTH"].ToString())
                    {
                        string[] dayTempalte = new string[31];
                        string[] weekTempalte = new string[31];
                        for (int k = 0; k < re1.Columns.Count - 2; k++)
                        {
                            if (!string.IsNullOrEmpty(re1.Rows[j][k].ToString()))
                            {
                                //单值
                                if (re1.Rows[j][k].ToString().Contains("*"))
                                {
                                    dayTempalte[k] = "单";
                                }
                                //休
                                else if (re1.Rows[j][k].ToString() == "0")
                                {
                                    dayTempalte[k] = "休";
                                }
                                //双值
                                else
                                {
                                    dayTempalte[k] = "双";
                                }
                                //周数（取第一位数字）
                                weekTempalte[k] = re1.Rows[j][k].ToString()[0].ToString();
                            }
                        }
                        data["dayTypeValsN1"] = new JArray(dayTempalte);
                        data["weekTypeValsN1"] = new JArray(weekTempalte);
                        data["totalWorkDaysN1"] = re1.Rows[j]["TOTALWORKDAYS"].ToString();
                    }
                }
            }
            else
            {
                string[] dayTempalte = new string[31];
                string[] weekTempalte = new string[31];
                for (int k = 1; k < 32; k++)
                {
                    string currentDate = vcYM1.Substring(0, 4) + "-" + vcYM1.Substring(4, 2) + "-" + k.ToString();
                    try
                    {
                        DateTime cd = Convert.ToDateTime(currentDate);
                        if (cd.DayOfWeek == DayOfWeek.Saturday || cd.DayOfWeek == DayOfWeek.Sunday)
                        {
                            dayTempalte[k - 1] = "休";
                        }
                        else
                        {
                            dayTempalte[k - 1] = "双";
                        }
                    }
                    catch
                    {
                        dayTempalte[k - 1] = "0";
                    }
                    //周数（取第一位数字）
                    weekTempalte[k - 1] = "";
                }
                data["dayTypeValsN1"] = new JArray(dayTempalte);
                data["weekTypeValsN1"] = new JArray(weekTempalte);
                data["totalWorkDaysN1"] = "";
            }
            #endregion

            #region 第3个月
            if (re2 != null && re2.Rows.Count > 0)
            {
                for (int j = 0; j < re2.Rows.Count; j++)
                {
                    if (vcYM2 == re2.Rows[j]["TARGETMONTH"].ToString())
                    {
                        string[] dayTempalte = new string[31];
                        string[] weekTempalte = new string[31];
                        for (int k = 0; k < re2.Columns.Count - 2; k++)
                        {
                            if (!string.IsNullOrEmpty(re2.Rows[j][k].ToString()))
                            {
                                //单值
                                if (re2.Rows[j][k].ToString().Contains("*"))
                                {
                                    dayTempalte[k] = "单";
                                }
                                //休
                                else if (re2.Rows[j][k].ToString() == "0")
                                {
                                    dayTempalte[k] = "休";
                                }
                                //双值
                                else
                                {
                                    dayTempalte[k] = "双";
                                }
                                //周数（取第一位数字）
                                weekTempalte[k] = re2.Rows[j][k].ToString()[0].ToString();
                            }
                        }
                        data["dayTypeValsN2"] = new JArray(dayTempalte);
                        data["weekTypeValsN2"] = new JArray(weekTempalte);
                        data["totalWorkDaysN2"] = re2.Rows[j]["TOTALWORKDAYS"].ToString();
                    }
                }
            }
            else
            {
                string[] dayTempalte = new string[31];
                string[] weekTempalte = new string[31];
                for (int k = 1; k < 32; k++)
                {
                    string currentDate = vcYM2.Substring(0, 4) + "-" + vcYM2.Substring(4, 2) + "-" + k.ToString();
                    try
                    {
                        DateTime cd = Convert.ToDateTime(currentDate);
                        if (cd.DayOfWeek == DayOfWeek.Saturday || cd.DayOfWeek == DayOfWeek.Sunday)
                        {
                            dayTempalte[k - 1] = "休";
                        }
                        else
                        {
                            dayTempalte[k - 1] = "双";
                        }
                    }
                    catch
                    {
                        dayTempalte[k - 1] = "0";
                    }
                    //周数（取第一位数字）
                    weekTempalte[k - 1] = "";
                }
                data["dayTypeValsN2"] = new JArray(dayTempalte);
                data["weekTypeValsN2"] = new JArray(weekTempalte);
                data["totalWorkDaysN2"] = "";
            }
            #endregion

            return data;
        }

        public string save(List<string> dayTypeVals, List<string> weekTypeVals, string varDxny, string varFZGC, decimal TOTALWORKDAYS, string strUserId)
        {
            //1表示第1周，*表示单值
            List<string> re = new List<string>();

            for (int i = 0; i < dayTypeVals.Count; i++)
            {
                switch (dayTypeVals[i])
                {
                    case "单":
                        re.Add(weekTypeVals[i] + "*");
                        break;
                    case null:
                        re.Add("0");
                        break;
                    default:
                        re.Add(weekTypeVals[i]);
                        break;
                } 
            }

            //如果不够31，则补齐
            if (re.Count < 31)
            {
                for (int i = 0; i < 31 - re.Count; i++)
                {
                    re.Add("0");
                }
            }
            return fs0609_DataAccess.save(re, varDxny, varFZGC, TOTALWORKDAYS, strUserId);
        }

        public string CopyTo(string vcPlantFrom, List<string> vcPlantTo, List<string> vcMon, string strUserId)
        {
            return fs0609_DataAccess.CopyTo(vcPlantFrom, vcPlantTo, vcMon, strUserId);
        }
    }
}
