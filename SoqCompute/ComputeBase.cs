using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SoqCompute
{
    public class ComputeBase
    {
		//获取月半稼动日是哪一天		
		public int getHalfDay(DataTable dtCalendar, decimal decTotalWorkDays)
		{
			int halfDay = Convert.ToInt32(Math.Ceiling(decTotalWorkDays / 2.0m));//((月总稼动/2)上取整)天		
			int iFindDay = 0;//处理到第几个稼动日		
			for (int i = 1; i <= 31 && iFindDay <= halfDay; i++)//遍历31天		
			{
				string strNowDay = dtCalendar.Rows[0]["TARGETDAY" + i] == System.DBNull.Value ? "0" : dtCalendar.Rows[0]["TARGETDAY" + i].ToString();
				if (strNowDay == "0")
					continue;
				else
				{
					iFindDay++;
					if (iFindDay == halfDay)
						return i;
				}
			}
			return -1;//如果稼动日没维护或者全维护成0，会走到这		
		}


		//获取月别稼动最大周数		
		public int getMaxWeek(DataTable dtCalendar)
		{
			string res = "";
			for (int i = 1; i <= 31; i++)//遍历31天		
			{
				string strNowDay = dtCalendar.Rows[0]["TARGETDAY" + i] == System.DBNull.Value ? "0" : dtCalendar.Rows[0]["TARGETDAY" + i].ToString();
				if (strNowDay == "0")
					continue;
				else
				{
					res = strNowDay;//记录最后稼动的周数	
				}
			}
			res = res.Replace("*", "");
			return Convert.ToInt32(res);
		}

		//返回稼动日第几周的起始天数和结束天数		
		public void getMaxWeek(DataTable dtCalendar, string week, ref int iStartDay, ref int iEndDay)
		{
			bool findStart = false;//是否找到这周第一天		
			bool findEnd = false;//是否找到这周最后一天		
			string res = "";
			for (int i = 1; i <= 31; i++)//遍历31天		
			{
				string strNowDay = dtCalendar.Rows[0]["TARGETDAY" + i] == System.DBNull.Value ? "0" : dtCalendar.Rows[0]["TARGETDAY" + i].ToString();
				if (strNowDay.StartsWith(week))
				{
					if (!findStart)
					{
						iStartDay = i;
						findStart = true;
					}
					else
					{
						iEndDay = i;
						findEnd = true;
					}
				}
			}
			if (!findEnd)
				iEndDay = iStartDay;
		}


		//返回某周分配总数最少的一天，如果有多个天都是最小的，应该按照从左到右返回第一天		
		public int getWeekMinDay(DataTable dtCalendar, string week, ArrayList beginData, ArrayList beforeTotalList, ref Hashtable hash)
		{
			int iStartDay = -1;
			int iEndDay = -1;
			getMaxWeek(dtCalendar, week, ref iStartDay, ref iEndDay);
			if (iStartDay == -1)
				return -1;//没找到		
			if (hash == null)
			{
				hash = new Hashtable();
				for (int i = 0; i < beforeTotalList.Count; i++)//遍历处理 [3~稼动日/特殊厂家] "之前" 的订单总数
				{
					string[] temp = (string[])beforeTotalList[i];
					for (int j = iStartDay; j <= iEndDay; j++)//遍历当前周日期范围	
					{
						string strNowDay = dtCalendar.Rows[0]["TARGETDAY" + j] == System.DBNull.Value ? "0" : dtCalendar.Rows[0]["TARGETDAY" + j].ToString();
						if (strNowDay == "0")//如果是非稼动日，则不应该纳入判断最小范围内
							continue;
						int iBox = Convert.ToInt32(temp[j + 3]);//这个地方+3，因为i从1开始，且稼动日左边有4个位置存其他信息	
						int iQuantityPercontainer = Convert.ToInt32(temp[2]);//收容数
						if (strNowDay.IndexOf('*') != -1)
							iBox = iBox * 2;
						//以下存储要匹配收容数，最后按照订单数进行比较
						if (hash[j] == null)
							hash[j] = iBox ;
						else
							hash[j] = Convert.ToInt32(hash[j]) + iBox ;
					}
				}
				for (int i = 0; i < beginData.Count; i++)
				{
					string[] temp = (string[])beginData[i];
					for (int j = iStartDay; j <= iEndDay; j++)//遍历当前周日期范围	
					{
						string strNowDay = dtCalendar.Rows[0]["TARGETDAY" + j] == System.DBNull.Value ? "0" : dtCalendar.Rows[0]["TARGETDAY" + j].ToString();
						if (strNowDay == "0")//如果是非稼动日，则不应该纳入判断最小范围内
							continue;
						int iBox = Convert.ToInt32(temp[j + 3]);//这个地方+3，因为i从1开始，且稼动日左边有4个位置存其他信息	
						int iQuantityPercontainer = Convert.ToInt32(temp[2]);//收容数
						if (strNowDay.IndexOf('*') != -1)
							iBox = iBox * 2;
						//以下存储要匹配收容数，最后按照订单数进行比较
						if (hash[j] == null)
							hash[j] = iBox ;
						else
							hash[j] = Convert.ToInt32(hash[j]) + iBox ;
					}
				}
			}
			
			int max = 999999999;
			int iResult = 0;
			for (int i = iStartDay; i <= iEndDay; i++)//遍历当前周日期范围		
			{
				if (hash[i] == null)
					continue;
				if ((int)hash[i] < max)
				{
					max = (int)hash[i];//由于是从左到右判断的，所以如果有多个最小值，肯定最左边的会返回
					iResult = i;
				}
			}
			hash[iResult] = Convert.ToInt32(hash[iResult]) + 1;//返回最小的一天，后面由于需要对品番箱数+1，所以缓存对应的天数总箱数也得+1
			return iResult;
		}

		//清空一个品番平准化结果		
		public string[] clearDelete_PZ(ref ArrayList beginData, string strPart_id)
		{
			for (int i = 0; i < beginData.Count; i++)
			{
				string[] temp = (string[])beginData[i];
				string strPart_id_temp = temp[0];
				if (strPart_id_temp == strPart_id)
				{
					for (int j = 4; j < 35; j++)//初始化31天为0
						temp[j] = "0";
					temp[35] = temp[3];//重置当前剩余分配箱子
					beginData.RemoveAt(i);
					return temp;
				}
			}
			return null;
		}

		

	}
}
