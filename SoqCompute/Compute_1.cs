using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SoqCompute
{
    public class Compute_1: ComputeBase
	{
		public void pinZhun_1(ref ArrayList beginData, DataTable dtCalendar, decimal decTotalWorkDays)
		{
			int halfDay = Convert.ToInt32(Math.Ceiling(decTotalWorkDays / 2.0m));//((月总稼动/2)上取整)天			
			bool halfFlag = false;//半值处理标记，遍历第一次设定true(数据不处理)，如果true才处理数据，处理完设定false			
			int iNextIndex = 0;//beginData中下一个要处理的数据游标			
			int halfDay_Num = getHalfDay(dtCalendar, decTotalWorkDays);//半值稼动日到哪一天			
			while (iNextIndex < beginData.Count)
			{
				for (int i = 1; i <= halfDay_Num && iNextIndex < beginData.Count; i++)//遍历31天，且当前beginData处理的游标iNextIndex小于数据总长度，处理日期限定半个稼动日范围内		
				{
					string strNowDay = dtCalendar.Rows[0]["TARGETDAY" + i] == System.DBNull.Value ? "0" : dtCalendar.Rows[0]["TARGETDAY" + i].ToString();
					if (strNowDay == "0")
						continue;
					else if (strNowDay.IndexOf('*') != -1)//半值不能处理第一次	
					{
						if (halfFlag == false)//第一次不处理，游标不变	
							halfFlag = true;
						else
						{
							string[] temp = (string[])beginData[iNextIndex];
							temp[i + 3] = "1";//这个地方+3，因为i从1开始，且稼动日左边有4个位置存其他信息
							iNextIndex++;
							halfFlag = false;
						}
					}
					else if (strNowDay.IndexOf('*') == -1)
					{
						string[] temp = (string[])beginData[iNextIndex];
						temp[i + 3] = "1";//这个地方+3，因为i从1开始，且稼动日左边有4个位置存其他信息
						iNextIndex++;
					}
				}
			}
		}

	}
}
