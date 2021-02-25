using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SoqCompute
{
    public class ComputeUtil
    {
		//验证特殊品番特殊厂家维护区间是否有稼动
		public static string checkSpecial(string strPlant,DataTable dtCalendar, DataTable dtSpecialSupplier, DataTable dtSpecialPart)
		{
			for (int i = 0; i < dtSpecialSupplier.Rows.Count; i++)
			{
				string strPart_id = dtSpecialSupplier.Rows[i]["vcPartId"].ToString();
				string strSupplier_id = dtSpecialSupplier.Rows[i]["vcSupplier_id"].ToString();

				int startDay = Convert.ToDateTime(dtSpecialSupplier.Rows[i]["dBeginDate"]).Day;
				int endDay = Convert.ToDateTime(dtSpecialSupplier.Rows[i]["dEndDate"]).Day;

				decimal decTotalWorkDays = 0;//合计稼动日重算			
				for (int j = 1; j <= 31; j++)//初始化31天为0			
				{
					if (j < startDay || j > endDay)
						;
					else
					{
						if (dtCalendar.Rows[0]["TARGETDAY" + j].ToString().IndexOf('*') != -1)//半值加0.5天		
							decTotalWorkDays = decTotalWorkDays + 0.5m;
						else if (dtCalendar.Rows[0]["TARGETDAY" + j].ToString() != "0")//全值加1天		
							decTotalWorkDays = decTotalWorkDays + 1.0m;
					}
				}
				if (decTotalWorkDays == 0)
					return strPlant+"厂 特殊厂家" + strSupplier_id + "区间" + startDay + "日到" + endDay + "日没有稼动，请确认！";
			}
			for (int i = 0; i < dtSpecialPart.Rows.Count; i++)
			{
				string strPart_id = dtSpecialPart.Rows[i]["vcPartId"].ToString();
				int startDay = Convert.ToDateTime(dtSpecialPart.Rows[i]["dBeginDate"]).Day;
				int endDay = Convert.ToDateTime(dtSpecialPart.Rows[i]["dEndDate"]).Day;

				decimal decTotalWorkDays = 0;//合计稼动日重算			
				for (int j = 1; j <= 31; j++)//初始化31天为0			
				{
					if (j < startDay || j > endDay)
						;
					else
					{
						if (dtCalendar.Rows[0]["TARGETDAY" + j].ToString().IndexOf('*') != -1)//半值加0.5天		
							decTotalWorkDays = decTotalWorkDays + 0.5m;
						else if (dtCalendar.Rows[0]["TARGETDAY" + j].ToString() != "0")//全值加1天		
							decTotalWorkDays = decTotalWorkDays + 1.0m;
					}
				}
				if (decTotalWorkDays == 0)
					return strPlant + "厂 特殊品番" + strPart_id + "区间" + startDay + "日到" + endDay + "日没有稼动，请确认！";
			}
			return null;
		}
	}
}
