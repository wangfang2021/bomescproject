using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;


namespace SoqCompute
{
    public class Compute_2 : ComputeBase
	{
		public void pinZhun_2(ref ArrayList beginData, DataTable dtCalendar, decimal decTotalWorkDays)
		{
			int halfDay = Convert.ToInt32(Math.Ceiling(decTotalWorkDays / 2.0m));//((月总稼动/2)上取整)天			
			bool halfFlag = false;//半值处理标记，日期遍历结束重置true，只有true时半值才处理			
			int iNextIndex = 0;//beginData中下一个要处理的数据游标			
			int halfDay_Num = getHalfDay(dtCalendar, decTotalWorkDays);//半值稼动日到哪一天			
																	   //处理第一箱			
			while (iNextIndex < beginData.Count)
			{
				for (int i = 1; i <= halfDay_Num && iNextIndex < beginData.Count; i++)//遍历前半月，且当前beginData处理的游标iNextIndex小于数据总长度		
				{
					string strNowDay = dtCalendar.Rows[0]["TARGETDAY" + i] == System.DBNull.Value ? "0" : dtCalendar.Rows[0]["TARGETDAY" + i].ToString();
					if (strNowDay == "0")
						continue;
					else if (strNowDay.IndexOf('*') != -1&& halfFlag)//半值间隔处理
					{
						string[] temp = (string[])beginData[iNextIndex];
						temp[i + 3] = "1";//这个地方+3，因为i从1开始，且稼动日左边有4个位置存其他信息
						iNextIndex++;
					}
					else if (strNowDay.IndexOf('*') == -1)
					{
						string[] temp = (string[])beginData[iNextIndex];
						temp[i + 3] = "1";//这个地方+3，因为i从1开始，且稼动日左边有4个位置存其他信息
						iNextIndex++;
					}
				}
				halfFlag = halfFlag == true ? false : true;
			}
			iNextIndex = 0;//重置0			
			halfFlag = false;//重置半值处理标记			
							 //处理第二箱			
			while (iNextIndex < beginData.Count)
			{
				for (int i = halfDay_Num + 1; i <= 31 && iNextIndex < beginData.Count; i++)//遍历后半月，且当前beginData处理的游标iNextIndex小于数据总长度		
				{
					string strNowDay = dtCalendar.Rows[0]["TARGETDAY" + i] == System.DBNull.Value ? "0" : dtCalendar.Rows[0]["TARGETDAY" + i].ToString();
					if (strNowDay == "0")
						continue;
					else if (strNowDay.IndexOf('*') != -1 && halfFlag)//半值间隔处理
					{
						string[] temp = (string[])beginData[iNextIndex];
						temp[i + 3] = "1";//这个地方+3，因为i从1开始，且稼动日左边有4个位置存其他信息
						iNextIndex++;
					}
					else if (strNowDay.IndexOf('*') == -1)
					{
						string[] temp = (string[])beginData[iNextIndex];
						temp[i + 3] = "1";//这个地方+3，因为i从1开始，且稼动日左边有4个位置存其他信息
						iNextIndex++;
					}
				}
				halfFlag = halfFlag == true ? false : true;
			}
		}

	}
}
