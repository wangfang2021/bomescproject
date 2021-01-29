using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SoqCompute
{
    public class Compute_Special : ComputeBase
    {
        Compute_MtoMax compute_MtoMax = new Compute_MtoMax();

        //平准特殊共通			
        public void pinZhun_Special(ref ArrayList beginData, DataTable dtCalendar, DataTable dtSpecial, string strSpecial)
        {
            for (int i = 0; i < dtSpecial.Rows.Count; i++)
            {
                string strPart_id = dtSpecial.Rows[i]["vcPartId"].ToString();
                string[] temp = clearDelete_PZ(ref beginData, strPart_id);//beginData把要特别处理的品番平准结果清空删除，并返回这条数据			
                if (temp == null)//品番没找到，处理下一个			
                    continue;
                DataTable dtCalendar_temp = dtCalendar.Clone();//创建临时dtCalendar_temp			
                                                               //根据品番的处理时间区间，把dtCalendar在区间范围外的处理成非稼动，然后赋值给dtCalendar_temp			
                int startDay = Convert.ToDateTime(dtSpecial.Rows[i]["dBeginDate"]).Day;
                int endDay = Convert.ToDateTime(dtSpecial.Rows[i]["dEndDate"]).Day;
                DataRow row = dtCalendar_temp.NewRow();
                decimal decTotalWorkDays = 0;//合计稼动日重算			
                for (int j = 1; j <= 31; j++)//初始化31天为0			
                {
                    if (j < startDay || j > endDay)
                        row["TARGETDAY" + j] = "0";
                    else
                    {
                        row["TARGETDAY" + j] = dtCalendar.Rows[0]["TARGETDAY" + j];
                        if (row["TARGETDAY" + j].ToString().IndexOf('*') != -1)//半值加0.5天		
                            decTotalWorkDays = decTotalWorkDays + 0.5m;
                        else if (row["TARGETDAY" + j].ToString() != "0")//全值加1天		
                            decTotalWorkDays = decTotalWorkDays + 1.0m;
                    }
                }
                dtCalendar_temp.Rows.Add(row);
                //合计稼动日计算decTotalWorkDays			
                ArrayList beginData_temp = new ArrayList();
                beginData_temp.Add(temp);
                compute_MtoMax.pinZhun_MtoMax(ref beginData_temp, dtCalendar_temp, decTotalWorkDays, strSpecial, beginData);//计算并设定特别处理			
                beginData.AddRange(beginData_temp);
            }
        }
    }
}
