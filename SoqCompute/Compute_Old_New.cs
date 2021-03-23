using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SoqCompute
{
    public class Compute_Old_New : ComputeBase
	{
		//对于新设或者废止品番，找出不应该有的天数箱数(前n天或者后n天)，按照找最低箱数稼动日，从左到右平准的算法去分配	
		public void pinZhun_Old_New(ref ArrayList beginData, DataTable dtCalendar,string strYearMonth,int iSub)
		{
			if (iSub <= 0)
				return;
			for (int i = 0; i < beginData.Count; i++)
			{
				string[] temp = (string[])beginData[i];
				//string strPart_id=temp[0] ;
				//if (strPart_id == "8158002D6200")
				//{
				//	int a = 0;
				//	a = 1;
				//}
				string strFromTime = temp[37];
				string strToTime = temp[38];
				int iFromTime_Check = -1;//起始时间校验，不为-1则判定需要校验
				int iToTime_Check = -1;//结束时间校验，不为-1则判定需要校验

				if (strFromTime != ""&& strYearMonth== Convert.ToDateTime(strFromTime).ToString("yyyyMM"))//起始时间不为空，且跟当前计算是一个月
				{
					iFromTime_Check = Convert.ToDateTime(strFromTime).Day;
				}
				if (strToTime != "" && strYearMonth == Convert.ToDateTime(strToTime).ToString("yyyyMM"))//结束时间不为空，且跟当前计算是一个月
				{
					iToTime_Check = Convert.ToDateTime(strToTime).Day;
				}
				if (iFromTime_Check == -1 && iToTime_Check == -1)//没有要处理的品番，继续
					continue;
				DataTable dtCam = getCalendar(dtCalendar, iFromTime_Check, iToTime_Check, iSub);//修正后的稼动日
				//循环品番品准结果，没有稼动日的箱数清空，计入剩余没有分配数量中
				clearDay(ref temp, dtCam);
				//按照找最少箱数的算法(最少箱数相等，从左到右)进行分配
				pinZhun(ref temp, dtCam);
 
			}
		}

		/// <summary>
		/// 获取品番加3减3之后的稼动日
		/// </summary>
		/// <param name="dtCalendar"></param>
		/// <returns></returns>
		public DataTable getCalendar(DataTable dtCalendar,int iFromTime_Check, int iToTime_Check, int iSub)
		{
			DataTable tempDt = dtCalendar.Copy();
			if (iFromTime_Check != -1)
			{
				int findDayFrom = 0;//起始时间找到多少个稼动日
				for (int i = 1; i <= 31; i++)//遍历当前周日期范围	
				{
					string strNowDay = tempDt.Rows[0]["TARGETDAY" + i] == System.DBNull.Value ? "0" : tempDt.Rows[0]["TARGETDAY" + i].ToString();
					if (i < iFromTime_Check)//价格有效期之“前”的稼动日，清空
					{
						tempDt.Rows[0]["TARGETDAY" + i] = "0";
					}
					else if (strNowDay != "0" && i >= iFromTime_Check && findDayFrom < iSub)//价格有效期开始，找到总稼动日数不够设定iSub数
					{
						tempDt.Rows[0]["TARGETDAY" + i] = "0";
						findDayFrom++;
					}
				}
			}
			
			if(iToTime_Check!=-1)
			{
				int findDayEnd = 0;//结束时间找到多少个稼动日
				for (int i = 31; i > 0; i--)//遍历当前周日期范围	
				{
					string strNowDay = tempDt.Rows[0]["TARGETDAY" + i] == System.DBNull.Value ? "0" : tempDt.Rows[0]["TARGETDAY" + i].ToString();
					if (i > iToTime_Check)//价格有效期之“后”的稼动日，清空
					{
						tempDt.Rows[0]["TARGETDAY" + i] = "0";
					}
					else if (strNowDay != "0" && i <= iToTime_Check && findDayEnd < iSub)//价格有效期开始，找到总稼动日数不够设定iSub数
					{
						tempDt.Rows[0]["TARGETDAY" + i] = "0";
						findDayEnd++;
					}
				}
			}
			return tempDt;
		}

		//循环品番品准结果，没有稼动日的箱数清空，计入剩余没有分配数量中
		public void clearDay(ref string[] temp, DataTable dtCalendar )
		{
			//先对品番35位存储的剩余未分配清空（前面算法有的没清）
			temp[35] = "0";
			for (int i = 1; i <= 31; i++)//遍历当前周日期范围	
			{
				string strNowDay = dtCalendar.Rows[0]["TARGETDAY" + i] == System.DBNull.Value ? "0" : dtCalendar.Rows[0]["TARGETDAY" + i].ToString();
				if (temp[i + 3] != "0" && strNowDay == "0")
				{
					int iBox_Last_PZ = Convert.ToInt32(temp[35]);//剩余需要平准化的箱数	
					iBox_Last_PZ = iBox_Last_PZ + Convert.ToInt32(temp[i + 3]);
					temp[35] = iBox_Last_PZ.ToString();//更新剩余平准箱子数	
					temp[i + 3] = "0";
				}
			}
		}
		//按照调整后的剩余箱数，进行最少箱数的算法(最少箱数相等，从左到右)进行分配
		public void pinZhun(ref string[] temp, DataTable dtCam)
		{
			int iBox_Last_PZ = Convert.ToInt32(temp[35]);//剩余需要平准化的箱数	
			if (iBox_Last_PZ <= 0)
				return;

			while(iBox_Last_PZ>0)
			{
				int iMinDay = 0;//最小箱数的日期
				int iMinBox = 9999999;
				for (int i = 1; i <= 31; i++)//遍历当前周日期范围	
				{
					string strNowDay = dtCam.Rows[0]["TARGETDAY" + i] == System.DBNull.Value ? "0" : dtCam.Rows[0]["TARGETDAY" + i].ToString();
					if (strNowDay != "0"&&Convert.ToInt32(temp[i + 3])< iMinBox)
					{
						iMinDay = i;
						iMinBox = Convert.ToInt32(temp[i + 3]);
					}
				}
				int iTemp = Convert.ToInt32(temp[iMinDay + 3]);
				temp[iMinDay + 3] = (iTemp + 1).ToString();
				iBox_Last_PZ = iBox_Last_PZ - 1;
			}
			temp[35] = "0";
		}




	}
}
