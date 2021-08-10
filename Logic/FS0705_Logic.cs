using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS0705_Logic
    {
        FS0705_DataAccess fs0705_DataAccess;

        public FS0705_Logic()
        {
            fs0705_DataAccess = new FS0705_DataAccess();
        }
 

        #region 按检索条件检索,
        public DataTable Search_TiaoZheng(string PackGPSNo, string PackNo, string TiaoZhengType, string dFromB, string dToE)
        {
            return fs0705_DataAccess.Search_TiaoZheng(PackGPSNo, PackNo, TiaoZhengType, dFromB, dToE);
        }
        #endregion

 

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId)
        {
            fs0705_DataAccess.importSave(dt, strUserId);
        }
        #endregion

 

        #region 
        public DataTable getSupplier()
        {
            return fs0705_DataAccess.getSupplier();
        }
        #endregion

        #region 获取所有当前时间段内所有品番的包材信息和有效信息,增加校验当前时段是否有包材品番入库
        public DataTable getInvalidPackNo(string strFaZhuID,string strPackSpot,string strEnd)
        {
            return fs0705_DataAccess.getInvalidPackNo(strFaZhuID, strPackSpot, strEnd);
        }
        #endregion

        #region 获取发邮件需要准备的数据
        public DataTable getEmailInformation()
        {
            return fs0705_DataAccess.getEmailInformation();
        }
        #endregion

        public DataTable getFaZhuIdPackCheckDT(string strFaZhuID,string strPackSpot)
        {
            return fs0705_DataAccess.getFaZhuIdPackCheckDT(strFaZhuID, strPackSpot);
        }

        #region 发注便次更新
        public ArrayList SearchFaZhuTime(string strPackSpot)
        {
            try
            {
                //一、取目前系统中需要计算的“逻辑名称”对应的入荷时间“起”
                DataTable dtFanWei = fs0705_DataAccess.SearchFaZhuLastTime(strPackSpot);
                ArrayList result = new ArrayList();

                for (int i = 0; i < dtFanWei.Rows.Count; i++)
                {
                    string strFaZhuID = dtFanWei.Rows[i]["vcFaZhuID"].ToString();
                    DataTable dtStandard = fs0705_DataAccess.getFaZhuTime(strFaZhuID,strPackSpot);//时间区间标准
                    string strEnd = dtFanWei.Rows[i]["dEnd"]==DBNull.Value?"":Convert.ToDateTime(dtFanWei.Rows[i]["dEnd"]).ToString("yyyy-MM-dd HH:mm:ss");
                    DateTime dEnd = new DateTime();
                    if (strEnd != "")
                        dEnd = DateTime.Parse(strEnd);//最后成功获取时间止
                    else
                        dEnd = DateTime.Parse("1900-1-1");//没获取直接给最早时间

                    DateTime dEnd_Index = new DateTime();//最后成功获取时间止-遍历用
                    if (strEnd != "")
                        dEnd_Index = dEnd;
                    else
                        dEnd_Index = DateTime.Now;
                    while (DateDiff(dEnd_Index, DateTime.Now) >= 0)
                    {
                        string strBanZhi = getPackBanZhi(dEnd_Index.ToString("yyyy-MM"), dEnd_Index.Day);

                        if (dEnd_Index.ToString("yyyy-MM-dd") == "2021-07-31" && strFaZhuID == "外注纸箱")
                        {
                            int temp = 1;
                        }
                        for (int j = 0; j < dtStandard.Rows.Count; j++)
                        {
                            string strX_WorkType = "";//发注时间白or夜
                            string strY_WorkType = "";//入荷起时间白or夜
                            string strY_To_WorkType = "";//入荷止时间白or夜
                            bool isOver12 = false;//入荷时间是否夜班，且是12点以后

                            DateTime dX = Convert.ToDateTime(dEnd_Index.ToString("yyyy-MM-dd ") + dtStandard.Rows[j]["dFaZhuFromTime"].ToString());//发注起始时间
                            DateTime dY = Convert.ToDateTime(dEnd_Index.ToString("yyyy-MM-dd ") + dtStandard.Rows[j]["dRuHeFromTime"].ToString());//入荷起始时间
                            DateTime dY_To = Convert.ToDateTime(dEnd_Index.ToString("yyyy-MM-dd ") + dtStandard.Rows[j]["dRuHeToTime"].ToString());//入荷结束时间
                            DateTime dN = Convert.ToDateTime(dEnd_Index.ToString("yyyy-MM-dd ") + dtStandard.Rows[j]["dNaQiFromTime"].ToString());//纳期起时间
                            DateTime dN_To = Convert.ToDateTime(dEnd_Index.ToString("yyyy-MM-dd ") + dtStandard.Rows[j]["dNaQiToTime"].ToString());//纳期止时间

                            
                            if (dX > DateTime.Now)
                                continue;//如果发注作业时间超过系统当前时间，则跳过
                            
                            if (!isJiaDong(strPackSpot, dX,ref strX_WorkType))//如果不是稼动时间，那么继续判断下一个
                                continue;

                            isJiaDong(strPackSpot, dY, ref strY_WorkType);//入荷起白夜班获取

                            isJiaDong(strPackSpot, dY_To, ref strY_To_WorkType);//入荷止白夜班获取

  

                            int iRuHeFromDay = Convert.ToInt32(dtStandard.Rows[j]["vcRuHeFromDay"].ToString());//部品入荷起加减数
                            int iRuHeToDay = Convert.ToInt32(dtStandard.Rows[j]["vcRuHeToDay"].ToString());//部品入荷止加减数
                            string strBianCi = dtStandard.Rows[j]["vcBianCi"].ToString();//便次名后缀
                            string strRuHeFromTime = dtStandard.Rows[j]["dRuHeFromTime"].ToString(); //入荷起时间
                            string strRuHeToTime = dtStandard.Rows[j]["dRuHeToTime"].ToString(); //入荷止时间
                            int iNaQiFromDay = Convert.ToInt32(dtStandard.Rows[j]["vcNaQiFromDay"].ToString());//纳期起加减数
                            string strNaQiFromTime = dtStandard.Rows[j]["dNaQiFromTime"].ToString(); //包材纳期起时间
                            string strNaQiToTime = dtStandard.Rows[j]["dNaQiToTime"].ToString(); //包材纳期止时间

                            if (strX_WorkType == "白")
                            {
                                //if (strY_WorkType == "白")
                                //    dY = Convert.ToDateTime(dX).AddDays(iRuHeFromDay);
                                //else//入荷是夜班
                                //    dY = Convert.ToDateTime(dX).AddDays(iRuHeFromDay+1);

                                //if (strY_To_WorkType == "白")
                                //    dY_To = Convert.ToDateTime(dX).AddDays(iRuHeToDay);
                                //else//入荷止是夜班
                                //    dY_To = Convert.ToDateTime(dX).AddDays(iRuHeToDay + 1);

                                if (strY_WorkType == "白")
                                    dY = addSubDay(dX,iRuHeFromDay);
                                else//入荷是夜班
                                    dY = addSubDay(dX, iRuHeFromDay + 1);

                                if (strY_To_WorkType == "白")
                                    dY_To = addSubDay(dX, iRuHeToDay);
                                else//入荷止是夜班
                                    dY_To = addSubDay(dX, iRuHeToDay + 1);
                            }
                            else
                            { //夜班
                                //if (dX.Hour >= 12 && dX.Hour < 24)
                                //{
                                //    dY = Convert.ToDateTime(dX).AddDays(iRuHeFromDay);
                                //}
                                //else if (dX.Hour >= 0 && dX.Hour < 12 && dY.Hour >= 12 && dY.Hour < 24)
                                //{
                                //    dY = Convert.ToDateTime(dX).AddDays(iRuHeFromDay-1);
                                //}
                                //else if (dX.Hour >= 0 && dX.Hour < 12 && dY.Hour >= 0 && dY.Hour < 12)
                                //{
                                //    dY = Convert.ToDateTime(dX).AddDays(iRuHeFromDay);
                                //}

                                //if (dX.Hour >= 12 && dX.Hour < 24)
                                //{
                                //    dY_To = Convert.ToDateTime(dX).AddDays(iRuHeToDay);
                                //}
                                //else if (dX.Hour >= 0 && dX.Hour < 12 && dY_To.Hour >= 12 && dY_To.Hour < 24)
                                //{
                                //    dY_To = Convert.ToDateTime(dX).AddDays(iRuHeToDay - 1);
                                //}
                                //else if (dX.Hour >= 0 && dX.Hour < 12 && dY_To.Hour >= 0 && dY_To.Hour < 12)
                                //{
                                //    dY_To = Convert.ToDateTime(dX).AddDays(iRuHeToDay);
                                //}
                                if (dX.Hour >= 12 && dX.Hour < 24)
                                {
                                    dY = addSubDay(dX, iRuHeFromDay);
                                }
                                else if (dX.Hour >= 0 && dX.Hour < 12 && dY.Hour >= 12 && dY.Hour < 24)
                                {
                                    dY = addSubDay(dX, iRuHeFromDay-1);
                                }
                                else if (dX.Hour >= 0 && dX.Hour < 12 && dY.Hour >= 0 && dY.Hour < 12)
                                {
                                    dY = addSubDay(dX, iRuHeFromDay);
                                }

                                if (dX.Hour >= 12 && dX.Hour < 24)
                                {
                                    dY_To = addSubDay(dX, iRuHeToDay);
                                }
                                else if (dX.Hour >= 0 && dX.Hour < 12 && dY_To.Hour >= 12 && dY_To.Hour < 24)
                                {
                                    dY_To = addSubDay(dX, iRuHeToDay-1);
                                }
                                else if (dX.Hour >= 0 && dX.Hour < 12 && dY_To.Hour >= 0 && dY_To.Hour < 12)
                                {
                                    dY_To = addSubDay(dX, iRuHeToDay);
                                }
                            }

                            DateTime dTempRuhe = Convert.ToDateTime(dY.ToString("yyyy-MM-dd") + " " + strRuHeFromTime);
                            dY_To = Convert.ToDateTime(dY_To.ToString("yyyy-MM-dd") + " " + strRuHeToTime);
                            if (dTempRuhe > dEnd&& dTempRuhe < DateTime.Now)//如果得到的入荷时间起大于最后获取入库时间，且小于当前时间，那么证明需要获取
                            {
                                //if (dTempRuhe.ToString("yyyy/MM/dd HH:mm:ss") == "2021/06/03 16:00:00")
                                //{
                                //    int a =1;
                                //    a = 2;
                                //}


                                BcTask task = new BcTask();
                                
                                DateTime dTimeTemp_NaQi = new DateTime();//计算具体纳期时间
                                
                                int iAddDay = dN.Hour < dX.Hour ? 1 : 0;//如果纳期起比发注起要小，证明跨天了
                                if (strX_WorkType == "白")
                                {
                                    //dTimeTemp_NaQi= dEnd_Index.AddDays(iNaQiFromDay + iAddDay);
                                    dTimeTemp_NaQi = addSubDay(dEnd_Index, iNaQiFromDay );
                                    //dTimeTemp_NaQi = dEnd_Index.AddDays(iAddDay);
                                    dTimeTemp_NaQi = Convert.ToDateTime(dTimeTemp_NaQi.ToString("yyyy-MM-dd") + " " + strNaQiFromTime);
                                }
                                else
                                { //夜班
                                    if (dX.Hour>12)//当天的夜班
                                    {
                                        //dTimeTemp_NaQi = dEnd_Index.AddDays(iNaQiFromDay + iAddDay);

                                        dTimeTemp_NaQi = addSubDay(dEnd_Index, iNaQiFromDay);
                                        if(iAddDay==1)
                                            dTimeTemp_NaQi = dTimeTemp_NaQi.AddDays(iAddDay);
                                        dTimeTemp_NaQi = Convert.ToDateTime(dTimeTemp_NaQi.ToString("yyyy-MM-dd") + " " + strNaQiFromTime);
                                    }
                                    else //昨天的夜班(即凌晨)
                                    {

                                        //dTimeTemp_NaQi = dEnd_Index.AddDays(iNaQiFromDay + iAddDay);

                                        ///begin 这块之所以取白班区间，是因为不能判断哪天是否是白班，因为这时候纳期是哪天还不确定///
                                        DataTable dt = fs0705_DataAccess.getBanZhi(strPackSpot, "白");
                                        DateTime dBaiStart = Convert.ToDateTime(dEnd_Index.ToString("yyyy-MM-dd") + " " + dt.Rows[0]["tFromTime"].ToString());
                                        DateTime dBaiEnd = Convert.ToDateTime(dEnd_Index.ToString("yyyy-MM-dd") + " " + dt.Rows[0]["tToTime"].ToString());
                                        DateTime temp= Convert.ToDateTime(dEnd_Index.ToString("yyyy-MM-dd") + " " + strNaQiFromTime);
                                        ///end///
                                        //if (temp >= dBaiStart && temp <= dBaiEnd)//如果纳期起是白班，这种用户会在纳期维护+1，实际上还是当天，则需要减去-1
                                        //{
                                        //    //dTimeTemp_NaQi = dTimeTemp_NaQi.AddDays(-1);
                                        //    dTimeTemp_NaQi = addSubDay(dTimeTemp_NaQi, -1);
                                        //}


                                        dTimeTemp_NaQi = Convert.ToDateTime(dEnd_Index.ToString("yyyy-MM-dd") + " " + strNaQiFromTime);
                                        //dTimeTemp_NaQi = dEnd_Index.AddDays(iAddDay);

                                        if (temp < dBaiStart)//纳期小于白班起，需要-1
                                        {
                                            //dTimeTemp_NaQi = dTimeTemp_NaQi.AddDays(-1);
                                            dTimeTemp_NaQi = addSubDay(dTimeTemp_NaQi, iNaQiFromDay  );
                                        }
                                        else
                                        {//大于等于白班，证明当前天就是当前工作日，这种时候用户会维护纳期是1，但实际上不加也不减，所以应该-1
                                            dTimeTemp_NaQi = addSubDay(dTimeTemp_NaQi, iNaQiFromDay -1  );
                                        }
                                        
                                    }
                                }

                                string strWorkType = "";
                                bool hasFind = false;//是否找到对应便次
                                if (isJiaDong(strPackSpot, dTimeTemp_NaQi, ref strWorkType))
                                {
                                    hasFind = true;
                                    task.dRuheToDate = dY_To;
                                    if (iAddDay == 1 && strWorkType == "夜" && dTimeTemp_NaQi.Hour >= 0 && dTimeTemp_NaQi.Hour < 12)
                                        dTimeTemp_NaQi = dTimeTemp_NaQi.AddDays(-1);//如果是稼动日，且是夜班，纳入时间在早上，那么工作日=纳期-1
                                    else if (strWorkType == "夜" && dX.Hour <= 12)
                                    {
                                        dTimeTemp_NaQi = dTimeTemp_NaQi.AddDays(-1);
                                    }
                                }
                                else
                                { //如果不是稼动日，则往后找一天最早班值的第一便次(距离白or夜起始时间最近的便次，即第一便次)
                                    task.dRuheToDate = dY_To;
                                    int findIndex = 0;
                                    int MAX_FIND = 30;//最多向后找30天
                                    while (true)//找纳期是否稼动，如果不是稼动，则往后找一天最早班值的第一便次
                                    {
                                        dTimeTemp_NaQi = dTimeTemp_NaQi.AddDays(1);
                                        findIndex++;

                                        if (findIndex > MAX_FIND)//向后找，也没找到维护的稼动日，则报错
                                            throw new Exception("包材纳期在向后找稼动日时，向后找超过" + MAX_FIND + "天都没找到稼动日，最后查找日期" + dTimeTemp_NaQi.ToString("yyyy-MM-dd"));
                                        string strBanZhiLast = getPackBanZhi(dTimeTemp_NaQi.ToString("yyyy-MM"), dTimeTemp_NaQi.Day);
                                        
                                        if (strBanZhiLast == "双值" || strBanZhiLast == "白值")
                                        {
                                            DataRow dRow = getFirstBianCi(strPackSpot, dtStandard, "白", dTimeTemp_NaQi);
                                            strBianCi = dRow["vcBianCi"].ToString();//便次名后缀
                                            strNaQiToTime = dRow["dNaQiToTime"].ToString();
                                            //dY_To = Convert.ToDateTime(dTimeTemp_NaQi.ToString("yyyy-MM-dd ") + dRow["dRuHeToTime"].ToString());//入荷结束时间
                                            //task.dRuheToDate = dY_To;
                                            //这种往后找的便次，有且只有：当前时间>=后找到的便次对应发注作业起，用户才能看见
                                            //DateTime dX_temp = Convert.ToDateTime(dTimeTemp_NaQi.ToString("yyyy-MM-dd ") + dRow["dFaZhuFromTime"].ToString());//发注起始时间
                                            //if(DateTime.Now>=dX_temp)
                                                hasFind = true;
                                            break;
                                        }
                                        else if (strBanZhiLast == "夜值")
                                        {
                                            DataRow dRow = getFirstBianCi(strPackSpot, dtStandard, "夜", dTimeTemp_NaQi);
                                            strBianCi = dRow["vcBianCi"].ToString();//便次名后缀
                                            strNaQiToTime = dRow["dNaQiToTime"].ToString();
                                            //dY_To = Convert.ToDateTime(dTimeTemp_NaQi.ToString("yyyy-MM-dd ") + dRow["dRuHeToTime"].ToString());//入荷结束时间
                                            //task.dRuheToDate = dY_To;
                                            //这种往后找的便次，有且只有：当前时间>=后找到的便次对应发注作业起，用户才能看见
                                            //DateTime dX_temp = Convert.ToDateTime(dTimeTemp_NaQi.ToString("yyyy-MM-dd ") + dRow["dFaZhuFromTime"].ToString());//发注起始时间
                                            //if (DateTime.Now >= dX_temp)
                                            hasFind = true;
                                            break;
                                        }
                                        else if (strBanZhiLast == "非稼动")
                                        {
                                            
                                        }
                                    }
                                }
                                if (hasFind)
                                    dEnd = dY_To;
                                else
                                    continue;
                                DateTime dTimeTemp_NaQi_To = new DateTime();//计算具体纳期截至时间
                                dTimeTemp_NaQi_To = Convert.ToDateTime(dTimeTemp_NaQi.ToString("yyyy-MM-dd") + " " + strNaQiToTime);

                                task.strBCName = dTimeTemp_NaQi.ToString("yyyy-MM-dd") + " " + strBianCi;
                                if (task.strBCName == "2021-06-08 外注纸箱 白值1便")
                                {
                                    int a = 0;
                                    a = 1;
                                }
                                task.dNaqiToDate = dTimeTemp_NaQi_To;
                                task.strFaZhuID = strFaZhuID;
                                result.Add(task);
                            }
                        }
                        dEnd_Index = dEnd_Index.AddDays(1);
                    }
                }
                ComputeCompare computeCompare = new ComputeCompare();
                result.Sort(computeCompare);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 从当前时间向前或者向后找指定的工作日，返回指定日期
        public DateTime addSubDay(DateTime dIndexDay, int iAddDays)
        {
            int findIndex = 0;//找的次数，最长不能大于找30天
            int MAX_FIND = 30;//最多向后找30天
            int iTurn = 0;//每一回合向前或者向后找一天
            if (iAddDays > 0)
                iTurn = 1;//大于0，则向后找稼动日
            else if (iAddDays < 0)
                iTurn = -1;//大于0，则往之前找稼动日
            else if (iAddDays == 0)
                return dIndexDay;

            while (true)//找纳期是否稼动，如果不是稼动，则往后找一天最早班值的第一便次
            {
                dIndexDay = dIndexDay.AddDays(iTurn);
                findIndex++;

                if (findIndex > MAX_FIND)//向后找，也没找到维护的稼动日，则报错
                    throw new Exception("包材纳期在向后找稼动日时，向后找超过" + MAX_FIND + "天都没找到稼动日，最后查找日期"+ dIndexDay.ToString("yyyy-MM-dd"));
                string strBanZhiLast = getPackBanZhi(dIndexDay.ToString("yyyy-MM"), dIndexDay.Day);
                if (strBanZhiLast == "双值" || strBanZhiLast == "白值")
                {
                    iAddDays = iAddDays - iTurn;
                }
                else if (strBanZhiLast == "夜值")
                {
                    iAddDays = iAddDays - iTurn;
                }
                else if (strBanZhiLast == "非稼动")
                {
                    //不做任何加减
                }
                if (iAddDays == 0)
                    break;
            }
            return dIndexDay;
        }
        #endregion


        /// <summary>
        /// 取第一个便次,小时差距离白夜起最近的，即为第一便次
        /// </summary>
        /// <param name="dtStandard"></param>
        public DataRow getFirstBianCi(string strPackSpot, DataTable dtStandard, string strBanZhi, DateTime dT)
        {
            DataRow result=null;
            int MIN = 9999;
            BZTime bz = getBZTime(strPackSpot, strBanZhi, dT.ToString("yyyy-MM-dd"));
            for (int i = 0; i < dtStandard.Rows.Count; i++)
            {
                DateTime dN = Convert.ToDateTime(dT.ToString("yyyy-MM-dd ") + dtStandard.Rows[i]["dNaQiFromTime"].ToString());//纳期起时间
                if (dN >= bz.dStart && dN <= bz.dEnd) {
                    if (dN.Hour - bz.dStart.Hour < MIN)
                    {
                        MIN = dN.Hour - bz.dStart.Hour;
                        result = dtStandard.Rows[i];
                    }
                }
            }
            return result;
        }

        public class ComputeCompare : IComparer
        {
            public int Compare(object x, object y)
            {
                BcTask temp_x = (BcTask)x;
                BcTask temp_y = (BcTask)y;
                if (string.Compare(temp_x.strFaZhuID, temp_y.strFaZhuID, true) == 1)
                    return 1;
                else if (string.Compare(temp_y.strFaZhuID, temp_x.strFaZhuID, true) == 1)
                    return -1;
                else
                {
                    if (string.Compare(temp_x.strBCName, temp_y.strBCName, true) == 1)
                        return 1;
                    if (string.Compare(temp_y.strBCName, temp_x.strBCName, true) == 1)
                        return -1;
                    return 0;
                }
            }
        }


        #region 日期相减得到天数
        public int DateDiff(DateTime dStartDate, DateTime dEndDate)
        {
            DateTime start = Convert.ToDateTime(dStartDate.ToShortDateString());
            DateTime end = Convert.ToDateTime(dEndDate.ToShortDateString());
            TimeSpan sp = end.Subtract(start);
            return sp.Days;
        }
        #endregion

        #region 取班值
        public string getPackBanZhi(string strYearMonth,int iDay)
        {
            DataTable dt = fs0705_DataAccess.getPackBanZhi(strYearMonth, iDay);
            if (dt == null || dt.Rows.Count == 0)
                return "";
            else
                return dt.Rows[0]["vcBanZhi"].ToString();
        }
        #endregion

        #region 获取用户计算过程检索和调整数据输入按钮的权限
        public bool getUserDisable(string strUserId)
        {
            DataTable dt = fs0705_DataAccess.getUserDisabledDT();
            if (dt.Select("userID=" + strUserId).Length>0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region 发注数量计算
        public void computer(string strFaZhuID, string strUserID, string strPackSpot,string strRuHeToTime,string strBianCi,string strNaQiDate)
        {
            fs0705_DataAccess.computer(strFaZhuID, strUserID, strPackSpot,strRuHeToTime,strBianCi,strNaQiDate);
        }
        #endregion

        #region 检索计算结果
        public DataTable searchComputeJG(string strPackSpot,string strBianCi)
        {
            DataTable dt = fs0705_DataAccess.searchComputeJG(strPackSpot,strBianCi);
            DataTable returnDT = dt.Clone();
            DataRow[] drs = dt.Select("iF_DingGou > 0");
            for (int i = 0; i < drs.Length; i++)
            {
                returnDT.ImportRow(drs[i]);
            }
            return returnDT;
        }
        #endregion

        #region 导出最后一次计算且有需要订购的计算数据
        public DataTable exportSearchJG(string strPackSpot)
        {
            return fs0705_DataAccess.exportSearchJG(strPackSpot);
        }
        #endregion

        #region 生成发注数据的检索计算结果（舍弃订购数量为NULL或者为0的数据）
        public DataTable SCFZDataSearchComputeJG(string strPackSpot)
        {
            DataTable dt = fs0705_DataAccess.SCFZDataSearchComputeJG(strPackSpot);

            //如果找到订单号不为空的数据，则说明上次进行过发注了，返回null
            int count = dt.Select("vcOrderNo is not null").Length;
            return count <= 0 ? dt : null;

        }
        #endregion

        #region 承认订购数量为0的计算结果用的检索
        public DataTable admitEmptyDataSearch(string strPackSpot)
        {
            return fs0705_DataAccess.admitEmptyDataSearch(strPackSpot);
        }
        #endregion

        #region 承认订购数量为0的计算结果用的检索
        public void AdmitEmptyData(DataTable dt, string strUserID)
        {
            fs0705_DataAccess.AdmitEmptyData(dt, strUserID);
        }
        #endregion

        #region 生成订单数据
        public void SCFZData(DataTable dt,string strOrderNo,string strUserid)
        {
            fs0705_DataAccess.SCFZData(dt,strOrderNo,strUserid);
        }
        #endregion

        #region 订单发注表中最后一个订单号
        public string getNewOrderNo()
        {
            DataTable dt = fs0705_DataAccess.getMAXOrderNo();
            if (dt == null || dt.Rows.Count == 0)
            {
                return "AA00000";
            }
            else
            {
                if (dt.Rows[0][0] == DBNull.Value || dt.Rows[0][0].ToString() == "")
                    return "AA00000";
                else
                {
                    return dt.Rows[0][0].ToString();
                }
            }
        }
        #endregion

        #region 获取新订单号
        public string getNewOrderNo(string strOrderNo)
        {

            string strOrderNoMAx = "";
            string strOrderNo_1 = strOrderNo.Substring(0, 1);
            string strOrderNo_2 = strOrderNo.Substring(1, 1);
            string strOrderNo_3 = strOrderNo.Substring(2, 5);
            if (strOrderNo_3 == "99999")
            {
                int iOrderNo_2 = (int)Convert.ToByte(strOrderNo_2[0]);

                if (strOrderNo_2 == "Z")
                {
                    int iOrderNo_1 = (int)Convert.ToByte(strOrderNo_1[0]);
                    strOrderNo_1 = Convert.ToChar(iOrderNo_1 + 1).ToString();
                    strOrderNo_2 = "A";
                    strOrderNo_3 = "00001";
                }
                else
                {
                    strOrderNo_3 = "00001";
                    strOrderNo_2 = Convert.ToChar(iOrderNo_2 + 1).ToString();
                }
            }
            else
            {
                int iOrderNo_3 = (int)Convert.ToInt32(strOrderNo_3) + 1;
                strOrderNo_3 = iOrderNo_3.ToString().PadLeft(5, '0');
            }
            strOrderNoMAx = strOrderNo_1 + strOrderNo_2 + strOrderNo_3;

            return strOrderNoMAx;

        }
        #endregion

        #region 返回某个日期是否应该是稼动
        public bool isJiaDong (string strPackPlant, DateTime dT,ref string strBaiYeType)
        {
            ArrayList checkList = new ArrayList();//当前日期稼动作业区间
            string strBanZhi = getPackBanZhi(dT.ToString("yyyy-MM"), dT.Day);
            DateTime dtLast = dT.AddDays(-1);
            string strBanZhiLast = getPackBanZhi(dtLast.ToString("yyyy-MM"), dtLast.Day);
            if (strBanZhiLast == "双值" || strBanZhiLast == "夜值")
            {
                switch (strBanZhi)
                {
                    case "非稼动":
                        checkList.Add(getBZTime(strPackPlant, "夜", dtLast.ToString("yyyy-MM-dd")));
                        ; break;
                    case "白值":
                        checkList.Add(getBZTime(strPackPlant, "夜", dtLast.ToString("yyyy-MM-dd")));
                        checkList.Add(getBZTime(strPackPlant, "白", dT.ToString("yyyy-MM-dd")));
                        ; break;
                    case "夜值":
                        checkList.Add(getBZTime(strPackPlant, "夜", dtLast.ToString("yyyy-MM-dd")));
                        checkList.Add(getBZTime(strPackPlant, "夜", dT.ToString("yyyy-MM-dd")));
                        ; break;
                    case "双值":
                        checkList.Add(getBZTime(strPackPlant, "夜", dtLast.ToString("yyyy-MM-dd")));
                        checkList.Add(getBZTime(strPackPlant, "白", dT.ToString("yyyy-MM-dd")));
                        checkList.Add(getBZTime(strPackPlant, "夜", dT.ToString("yyyy-MM-dd")));
                        ; break;
                }
            }
            else if (strBanZhiLast == "白值" || strBanZhiLast == "非稼动")
            {
                switch (strBanZhi)
                {
                    case "非稼动":
                        ; break;
                    case "白值":
                        checkList.Add(getBZTime(strPackPlant, "白", dT.ToString("yyyy-MM-dd")));
                        ; break;
                    case "夜值":
                        checkList.Add(getBZTime(strPackPlant, "夜", dT.ToString("yyyy-MM-dd")));
                        ; break;
                    case "双值":
                        checkList.Add(getBZTime(strPackPlant, "白", dT.ToString("yyyy-MM-dd")));
                        checkList.Add(getBZTime(strPackPlant, "夜", dT.ToString("yyyy-MM-dd")));
                        ; break;
                }
            }
            for (int i = 0; i < checkList.Count; i++)
            {
                BZTime bZTime = (BZTime)checkList[i];
                if (dT >= bZTime.dStart && dT <= bZTime.dEnd)
                {
                    strBaiYeType = bZTime.strBaiYeType;
                    return true;
                }
            }
            return false;
        }
        #endregion


        #region 取班值起始时间和结束时间
        /// <summary>
        /// 取班值起始时间和结束时间
        /// </summary>
        /// <param name="strPackPlant">工厂</param>
        /// <param name="strBanZhi">班值 白/夜</param>
        /// <param name="strYearMonthDay">2021-3-25</param>
        /// <returns></returns>
        public BZTime getBZTime(string strPackPlant, string strBaiYeType, string strYearMonthDay)
        {
            string strBanZhiStart = "";
            string strBanZhiEnd = "";
            DataTable dt = fs0705_DataAccess.getBanZhi(strPackPlant, strBaiYeType);
            strBanZhiStart = dt.Rows[0]["tFromTime"].ToString();
            strBanZhiEnd = dt.Rows[0]["tToTime"].ToString();
            DateTime dStart = Convert.ToDateTime(strYearMonthDay +" "+ strBanZhiStart);
            DateTime dEnd = Convert.ToDateTime(strYearMonthDay + " " + strBanZhiEnd);
            if (strBaiYeType == "夜")
                dEnd = dEnd.AddDays(1);//夜班的结束是下一天
            BZTime bZTime = new BZTime();
            bZTime.dStart = dStart;
            bZTime.dEnd = dEnd;
            bZTime.strBaiYeType = strBaiYeType;
            return bZTime;
        }
        #endregion


        #region 取工厂对应白班的起始时间
        public string getBanZhiStart(string strPackPlant,string strBanZhi)
        {
            DataTable dt = fs0705_DataAccess.getBanZhi(strPackPlant, strBanZhi);
            if (dt == null || dt.Rows.Count == 0)
                return "";
            else
                return dt.Rows[0]["tFromTime"].ToString();
        }
        #endregion

        #region 取工厂对应白班的起始时间
        public string getBanZhiEnd(string strPackPlant, string strBanZhi)
        {
            DataTable dt = fs0705_DataAccess.getBanZhi(strPackPlant, strBanZhi);
            if (dt == null || dt.Rows.Count == 0)
                return "";
            else
                return dt.Rows[0]["tToTime"].ToString();
        }
        #endregion




        #region 调整数据输入-检索
        public DataTable search_Sub(string strPackNo, string strPackGPSNo, string strFrom, string strTo, string strType,string strPackSpot)
        {
            return fs0705_DataAccess.search_Sub(strPackNo, strPackGPSNo, strFrom, strTo, strType,strPackSpot);
        }
        #endregion

        #region 调整数据输入-保存
        public void Save_Sub(List<Dictionary<string, Object>> listInfoData, string strUserId,string strPackSport)
        {
            fs0705_DataAccess.save_Sub(listInfoData, strUserId,strPackSport);
        }
        #endregion

        #region 调整数据输入-导入后保存
        public void importSave_Sub(DataTable dt, string strUserId,string strPackSpot)
        {
            fs0705_DataAccess.importSave_Sub(dt, strUserId,strPackSpot);
        }
        #endregion

        #region 获取品番是否满足保存条件
        public DataTable getIsSave(List<Dictionary<string, Object>> listInfoData,string strPackSpot) 
        {
            return fs0705_DataAccess.getIsSave(listInfoData,strPackSpot);
        }
        #endregion

        #region 获取品番是否满足保存条件
        public DataTable getIsSave(DataTable dt,string strPackSpot)
        {
            return fs0705_DataAccess.getIsSave(dt,strPackSpot);
        }
        #endregion

        #region 计算过程检索
        public DataTable searchPackCompute(string strPackNo, string strPackGPSNo,string strFaZhuID, string strFrom, string strTo,string strPackSpot)
        {
            return fs0705_DataAccess.searchPackCompute(strPackNo, strPackGPSNo, strFaZhuID, strFrom, strTo,strPackSpot);
        }
        #endregion

        #region 校验邮箱格式是否正确
        /// <summary>
        /// 校验收件人信息能否被添加
        /// </summary>
        /// <param name="strEmail"></param>
        /// <returns>true：可以添加，反之则false</returns>
        public bool CheckEmailFormat(object strEmail, object disPlayName)
        {
            try
            {
                new System.Net.Mail.MailAddress(strEmail.ToString().Trim(), disPlayName.ToString().Trim(), Encoding.UTF8);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
    public class BcTask
    {
        public string strFaZhuID;//发注逻辑
        public string strBCName;//便次名称
        public DateTime dDate;//便次对应的稼动日，注意不是自然日，比如2021-3-24 2:00:00的夜班，dDate算2021-3-23
        public DateTime dRuheToDate;
        public DateTime dNaqiToDate;
    }



    public class BZTime
    {
        public string strBaiYeType;
        public DateTime dStart;
        public DateTime dEnd;
    }
}

