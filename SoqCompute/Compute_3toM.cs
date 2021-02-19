using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;


namespace SoqCompute
{
    public class Compute_3toM : ComputeBase
	{
		//平准3箱~稼动日主方法							
		public void pinZhun_3toM(ref ArrayList beginData, DataTable dtCalendar)
		{
			Hashtable hash = new Hashtable();//记录每周的最后游标和半值处理标记
			int iWeekLength = getMaxWeek(dtCalendar);//剩余还有多少周要处理
			int iWeekLength_cursor = iWeekLength;
			string strNowDealWeek = "1";//从第一周开始
			for (int i = 0; i < beginData.Count; i++)
			{
				int iStartDay = -1;
				int iEndDay = -1;
				int iPositionDay = -1;//记录当前分配日期游标	
				bool halfFlag = false;//半值处理标记，遍历第一次设定false(数据不处理)，如果true才处理数据，处理完设定false		
				getMaxWeek(dtCalendar, strNowDealWeek, ref iStartDay, ref iEndDay);//获取指定周工作日区间范围						
				if (iStartDay == -1)//没找到，证明没有，比如第5周，此时周数++，但是品番不能变					
				{
					switch (strNowDealWeek)
					{
						case "1": strNowDealWeek = "3"; break;
						case "3": strNowDealWeek = "5"; break;
						case "5": strNowDealWeek = "2"; break;
						case "2": strNowDealWeek = "4"; break;
						case "4": strNowDealWeek = "1"; break;
					}
					i--;
					continue;
				}
				if (hash[strNowDealWeek] != null)
				{
					iPositionDay = (int)hash[strNowDealWeek];//最有天数游标
					halfFlag = (bool)hash[strNowDealWeek + "halfFlag"];//最后半值游标
				}

				string[] temp = (string[])beginData[i];
				pingZhunPart(ref temp, dtCalendar, ref halfFlag, iWeekLength_cursor, iStartDay, iEndDay, ref iPositionDay);
				hash[strNowDealWeek] = iPositionDay;//最有天数游标
				hash[strNowDealWeek + "halfFlag"] = halfFlag;//最后半值游标
				switch (strNowDealWeek)
				{
					case "1": strNowDealWeek = "3"; break;
					case "3": strNowDealWeek = "5"; break;
					case "5": strNowDealWeek = "2"; break;
					case "2": strNowDealWeek = "4"; break;
					case "4": strNowDealWeek = "1"; break;
				}
				int iBox_Last_PZ = Convert.ToInt32(temp[35]);//剩余需要平准化的箱数		
				if (iBox_Last_PZ > 0)//当前品番没分配完，继续下周分配
				{
					i--;
					iWeekLength_cursor--;
				}
				else//分完了，下一个品番
				{
					iWeekLength_cursor = iWeekLength;//处理周数还原
				}
			}
		}
 

		//平准一个品番，按照制定日期时间范围、起始日期进行每日分配，如果分配到最后一天没分完，则从第一个天数分配,iPositionDay=下一个要分配的天数							
		public void pingZhunPart(ref string[] temp, DataTable dtCalendar, ref bool halfFlag, int iWeekLength, int iStartDay, int iEndDay, ref int iPositionDay)
		{
			iPositionDay = iPositionDay == -1 ? iStartDay : iPositionDay;
			int iBox_Last_PZ = Convert.ToInt32(temp[35]);//剩余需要平准化的箱数							
			int iBox_week_Deal = Convert.ToInt32(Math.Ceiling((decimal)iBox_Last_PZ / (decimal)iWeekLength));//本周要处理=向上取整(该品番剩余没平准的总箱数/剩余总周数)							
			int iBox_week_finish = 0;//已经分配的数量							
			while (iBox_week_Deal != iBox_week_finish)
			{
				for (int i = iPositionDay; i <= iEndDay && iBox_week_Deal != iBox_week_finish; i++)//遍历当前周日期范围						
				{
					string strNowDay = dtCalendar.Rows[0]["TARGETDAY" + i] == System.DBNull.Value ? "0" : dtCalendar.Rows[0]["TARGETDAY" + i].ToString();
					if (strNowDay == "0")
					{
						if (i != iEndDay)
							iPositionDay = i + 1;
						else
							iPositionDay = iStartDay;
						continue;
					}
					else if (strNowDay.IndexOf('*') != -1 && !halfFlag)//半值不处理数据
					{
						if (i != iEndDay)
							iPositionDay = i + 1;
						else
							iPositionDay = iStartDay;
					}
					else if (strNowDay.IndexOf('*') != -1&& halfFlag)//半值处理数据
					{
						int ibox_temp = Convert.ToInt32(temp[i + 3]);
						temp[i + 3] = (ibox_temp + 1).ToString();
						iBox_week_finish++;
						if (i != iEndDay)
							iPositionDay = i + 1;
						else
							iPositionDay = iStartDay;
					}
					else if (strNowDay.IndexOf('*') == -1)
					{
						int ibox_temp = Convert.ToInt32(temp[i + 3]);
						temp[i + 3] = (ibox_temp + 1).ToString();
						iBox_week_finish++;
						if (i != iEndDay)
							iPositionDay = i + 1;
						else
							iPositionDay = iStartDay;
					}
				}
				//以下半值标记切换，有且只有下一个品番是第一位置时才会重置(有可能下个品番是一行中的某个位置，此时切换是不对的！)
				if(iPositionDay== iStartDay)
					halfFlag = halfFlag == true ? false : true;
			}
			temp[35] = (iBox_Last_PZ - iBox_week_finish).ToString();//=剩余平准箱子数-本周已经分配							
		}

	}
}
