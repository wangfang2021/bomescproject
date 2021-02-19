using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SoqCompute
{
    public class Compute_MtoMax : ComputeBase
	{
		//平准大于稼动日主方法	
		public void pinZhun_MtoMax(ref ArrayList beginData, DataTable dtCalendar, decimal decTotalWorkDays, string strSpecial, ArrayList beforeTotalList)
		{
			int iMaxWeek = getMaxWeek(dtCalendar);//一共多少周要处理	
			for (int i = 0; i < beginData.Count; i++)
			{
				string[] temp = (string[])beginData[i];
				//执行平均分配
				pingZhun_MtoMax_everyDay(ref temp, dtCalendar, decTotalWorkDays);
				if (strSpecial != null)//特殊厂家、特殊品番处理
					temp[36] = strSpecial;
			}
			//剩余箱子按照"品番内部"5-4-3-2-1顺序，按顺序分配到每周最少的稼动日中
			int nowWeek = 5;//按照5-4-3-2-1进行处理
			for (int i = 0; i < beginData.Count; i++)
			{
				string[] temp = (string[])beginData[i];
				int iBox_Last_PZ = Convert.ToInt32(temp[35]);//剩余需要平准化的箱数
				if (iBox_Last_PZ == 0)
					continue;//没有剩余的箱子要分配，跳过
				//返回某周分配总数最少的一天
				int iMinDay = getWeekMinDay(dtCalendar, nowWeek.ToString(), beginData, beforeTotalList);
				if (iMinDay == -1)//有可能这周不存在，则遍历的品番不变，周数--
				{
					i--;
					nowWeek--;
					nowWeek = nowWeek == 0 ? 5 : nowWeek;
					continue;
				}
				int iBox = Convert.ToInt32(temp[iMinDay + 3]);//取到这天的箱子数
				temp[iMinDay + 3] = (iBox + 1).ToString();
				temp[35] = (iBox_Last_PZ - 1).ToString();
				if (iBox_Last_PZ - 1 > 0)//这个品番没处理完
					i--;
				if (iBox_Last_PZ == 1)//如果这是最后一个箱子，那么应该重置周数=5
					nowWeek = 5;
				else
				{
					nowWeek--;
					nowWeek = nowWeek == 0 ? 5 : nowWeek;//到1时再减变成0，此时需要设定下次从5开始
				}

			}
		}
		//平准平均分配每一天	
		public void pingZhun_MtoMax_everyDay(ref string[] temp, DataTable dtCalendar, decimal decTotalWorkDays)
		{
			int iBoxTotal = Convert.ToInt32(temp[3]);//该品番总箱数	
			int iBoxDay = (int)((decimal)iBoxTotal / decTotalWorkDays);//平均每日箱数=(总箱数/总稼动日)向下取整	
			int iBox_Last_PZ = Convert.ToInt32(temp[35]);//剩余需要平准化的箱数	
			for (int i = 1; i <= 31; i++)//遍历当前周日期范围	
			{
				string strNowDay = dtCalendar.Rows[0]["TARGETDAY" + i] == System.DBNull.Value ? "0" : dtCalendar.Rows[0]["TARGETDAY" + i].ToString();
				if (strNowDay != "0" && strNowDay.IndexOf('*') != -1)//1值时，分配数量=(平均数量/2)向下取整
				{
					int iBox_half = iBoxDay / 2;
					temp[i + 3] = iBox_half.ToString();//这个地方+3，因为i从1开始，且稼动日左边有4个位置存其他信息
					iBox_Last_PZ = iBox_Last_PZ - iBox_half;
				}
				else if (strNowDay != "0" && strNowDay.IndexOf('*') == -1)
				{
					temp[i + 3] = iBoxDay.ToString();//这个地方+3，因为i从1开始，且稼动日左边有4个位置存其他信息
					iBox_Last_PZ = iBox_Last_PZ - iBoxDay;
				}
			}
			temp[35] = iBox_Last_PZ.ToString();//更新剩余平准箱子数	
		}

	}
}
