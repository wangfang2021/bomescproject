using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SoqCompute
{
    public class ComputeMain
    {
        Compute_1 compute_1 = new Compute_1();
        Compute_2 compute_2 = new Compute_2();
        Compute_3toM compute_3toM = new Compute_3toM();
        Compute_MtoMax compute_MtoMax = new Compute_MtoMax();
        Compute_SpecialPart compute_SpecialPart = new Compute_SpecialPart();
        Compute_SpecialSupplier compute_SpecialSupplier = new Compute_SpecialSupplier();

        //根据月别soq和月别日历，进行平准化计算，返回二位数组(soq和日历在算法外已经根据业务需要取好了)
        /// <summary>
        /// 平准化入口方法，返回月别平准结果
        /// </summary>
        /// <param name="dtSoq">某月soq内容</param>
        /// <param name="dtCalendar">某月“平准”稼动日</param>
        /// <param name="dtSpecialSupplier">某月特殊厂家，没有给null</param>
        /// <param name="dtSpecialPart">某月特殊品番，没有给null</param>
        /// <returns></returns>
        public ArrayList getPinZhunList(DataTable dtSoq, DataTable dtCalendar, DataTable dtSpecialSupplier, DataTable dtSpecialPart)
        {
            decimal decTotalWorkDays = 0;//可能会有小数0.5出现													
            decTotalWorkDays = Convert.ToDecimal(dtCalendar.Rows[0]["TOTALWORKDAYS"]);
            ArrayList beginData_1 = new ArrayList();//1箱处理数据													
            ArrayList beginData_2 = new ArrayList();//2箱处理数据													
            ArrayList beginData_3 = new ArrayList();//3箱到稼动处理数据													
            ArrayList beginData_4 = new ArrayList();//大于稼动日箱数处理数据													
            getBeginData(dtSoq, decTotalWorkDays, ref beginData_1, ref beginData_2, ref beginData_3, ref beginData_4);
            ArrayList result = new ArrayList();//1箱处理数据											
            compute_1.pinZhun_1(ref beginData_1, dtCalendar, decTotalWorkDays);
            compute_2.pinZhun_2(ref beginData_2, dtCalendar, decTotalWorkDays);
            compute_3toM.pinZhun_3toM(ref beginData_3, dtCalendar);
            compute_MtoMax.pinZhun_MtoMax(ref beginData_4, dtCalendar, decTotalWorkDays, null);

            if(beginData_1.Count>0)
                result.AddRange(beginData_1);
            if (beginData_2.Count > 0)
                result.AddRange(beginData_2);
            if (beginData_3.Count > 0)
                result.AddRange(beginData_3);
            if (beginData_4.Count > 0)
                result.AddRange(beginData_4);

            if (dtSpecialSupplier!=null&& dtSpecialSupplier.Rows.Count>0)
                compute_SpecialSupplier.pinZhun_SpecialSupplier(ref result, dtCalendar, dtSpecialSupplier);//特殊厂家处理，注意跟品番先后顺序，先厂家，再品番
            if (dtSpecialPart != null && dtSpecialPart.Rows.Count > 0)
                compute_SpecialPart.pinZhun_SpecialPart(ref result, dtCalendar, dtSpecialPart);//特殊品番处理

            return result;
        }

        //根据soq以及类别，返回要处理的原始二维数组													
        private void getBeginData(DataTable dtSoq, decimal decTotalWorkDays, ref ArrayList beginData_1, ref ArrayList beginData_2, ref ArrayList beginData_3, ref ArrayList beginData_4)
        {
            for (int i = 0; i < dtSoq.Rows.Count; i++)
            {
                int iHyNum = Convert.ToInt32(dtSoq.Rows[i]["iHyNum"].ToString());//合意订单数量													
                int iQuantityPercontainer = Convert.ToInt32(dtSoq.Rows[i]["iQuantityPercontainer"].ToString());//收容数													
                int iBox = iHyNum / iQuantityPercontainer;//箱子数量													
                string strPart_id = dtSoq.Rows[i]["vcPart_id"].ToString();
                string[] temp = new string[37];//一行数据													
                temp[0] = strPart_id;
                temp[1] = iHyNum.ToString();
                temp[2] = iQuantityPercontainer.ToString();
                temp[3] = iBox.ToString();
                for (int j = 4; j < 35; j++)//初始化31天为0													
                    temp[j] = "0";
                temp[35] = iBox.ToString();//当前剩余分配箱子													
                if (iBox == 1)
                {
                    beginData_1.Add(temp);
                }
                else if (iBox == 2)
                {
                    beginData_2.Add(temp);
                }
                else if (iBox >= 3 && iBox <= decTotalWorkDays)
                {
                    beginData_3.Add(temp);
                }
                else if (iBox > decTotalWorkDays)
                {
                    beginData_4.Add(temp);
                }
            }
        }
    }
}
