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
    public class FS0608_Logic
    {
        FS0608_DataAccess fs0608_DataAccess;

        public FS0608_Logic()
        {
            fs0608_DataAccess = new FS0608_DataAccess();

        }

        public JObject Search(DateTime varDxny, string varFZGC)
        {
            JObject data = new JObject { };

            DataTable re = fs0608_DataAccess.Search(varDxny, varFZGC);

            string[] dxnyArray = new string[]
                { varDxny.ToString("yyyyMM"),
                  varDxny.AddMonths(1).ToString("yyyyMM"),
                  varDxny.AddMonths(2).ToString("yyyyMM")
                };

            for (int i = 0; i < dxnyArray.Length; i++)
            {
                for (int j = 0; j < re.Rows.Count; j++)
                {
                    if (dxnyArray[i] == re.Rows[j]["TARGETMONTH"].ToString())
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
                        data["dayTypeValsN" + (i == 0 ? "" : i.ToString())] = new JArray(dayTempalte);
                        data["weekTypeValsN" + (i == 0 ? "" : i.ToString())] = new JArray(weekTempalte);
                        data["totalWorkDaysN" + (i == 0 ? "" : i.ToString())] = re.Rows[j]["TOTALWORKDAYS"].ToString();
                    }
                }
            }


            return data;
        }

        public void save(List<string> dayTypeVals, List<string> weekTypeVals, string varDxny, string varFZGC, decimal TOTALWORKDAYS, string strUserId)
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
                        re.Add(null);
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
                    re.Add(null);
                }
            }


            fs0608_DataAccess.save(re, varDxny, varFZGC, TOTALWORKDAYS, strUserId);
        }

        public string CopyTo(string vcPlantFrom, List<string> vcPlantTo, List<string> vcMon, string strUserId)
        {
            return fs0608_DataAccess.CopyTo(vcPlantFrom, vcPlantTo, vcMon, strUserId);
        }
    }
}
