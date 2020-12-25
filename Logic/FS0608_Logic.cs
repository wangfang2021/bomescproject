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
            JObject data = new JObject {};

            DataTable re = fs0608_DataAccess.Search(varDxny, varFZGC);

            for (int i = 0; i < re.Rows.Count; i++)
            {
                string[] dayTempalte = new string[31];
                string[] weekTempalte = new string[31];
                for (int j = 0; j < re.Columns.Count - 1; j++)
                {
                    //单值
                    if (re.Rows[i][j].ToString().Contains("*"))
                    {
                        dayTempalte[j] = "单";
                    }
                    //休
                    else if (re.Rows[i][j].ToString() == "0")
                    {
                        dayTempalte[j] = "休";
                    }
                    //双值
                    else {
                        dayTempalte[j] = "双";
                    }
                    //周数（取第一位数字）
                    weekTempalte[j] = re.Rows[i][j].ToString()[0].ToString();
                }
                data["dayTypeValsN"+(i==0?"":i.ToString())] = new JArray(dayTempalte);
                data["weekTypeValsN" + (i == 0 ? "" : i.ToString())] = new JArray(weekTempalte);
                data["totalWorkDaysN" + (i == 0 ? "" : i.ToString())] = re.Rows[i]["TOTALWORKDAYS"].ToString();
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
                    default:
                        re.Add(weekTypeVals[i]);
                        break;
                }
            }

            fs0608_DataAccess.save(re, varDxny, varFZGC, TOTALWORKDAYS, strUserId);
        }

    }
}
