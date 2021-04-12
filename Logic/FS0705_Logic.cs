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

        #region 获取所有当前时间段内所有品番的包材信息和有效信息
        public string[] getPackCheckDT(string strFaZhuID,string strPackSpot)
        {
            DataTable packCheckDT = fs0705_DataAccess.getPackCheckDT(strFaZhuID, strPackSpot);
            string[] strArray = new string[2];
            //记录无包材构成的品番
            string strErr1 = "";
            //记录包材构成无效的品番
            string strErr2 = "";

            //取到本次计算时段的所有品番(不重复)
            List<string> partLists = new List<string>();
            for (int i = 0; i < packCheckDT.Rows.Count; i++)
            {
                partLists.Add(packCheckDT.Rows[i]["vcPart_id"].ToString());
            }
             partLists = partLists.Distinct().ToList();

            for (int i = 0; i < partLists.Count; i++)
            {
                DataRow[] drs = packCheckDT.Select("vcPart_id='" + partLists[i]+ "' and vcPartsNo is null");
                if (drs.Length>0)
                {
                    strErr1 += partLists[i]+" ";
                }
                else
                {
                    drs = packCheckDT.Select("vcPart_id='" + partLists[i] + "' and dUsedFrom<'" + DateTime.Now.ToString() + "' and '" + DateTime.Now.ToString() + "'<dUsedTo");
                    if (drs.Length<=0)
                    {
                        strErr2 += partLists[i]+" ";
                    }
                }
            }

            strArray[0] = strErr1;
            strArray[1] = strErr2;
            return strArray;
        }
        #endregion

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

                        for (int j = 0; j < dtStandard.Rows.Count; j++)
                        {
                            string strX_WorkType = "";//发注时间白or夜
                            string strY_WorkType = "";//入荷时间白or夜
                            bool isOver12 = false;//入荷时间是否夜班，且是12点以后

                            DateTime dX = Convert.ToDateTime(dEnd_Index.ToString("yyyy-MM-dd ") + dtStandard.Rows[j]["dFaZhuFromTime"].ToString());//发注起始时间
                            DateTime dY = Convert.ToDateTime(dEnd_Index.ToString("yyyy-MM-dd ") + dtStandard.Rows[j]["dRuHeFromTime"].ToString());//入荷起始时间
                            if (dX > DateTime.Now)
                                continue;//如果发注作业时间超过系统当前时间，则跳过
                            
                            if (!isJiaDong(strPackSpot, dX,ref strX_WorkType))//如果不是稼动时间，那么继续判断下一个
                                continue;

                            isJiaDong(strPackSpot, dY, ref strY_WorkType);//入荷起白夜班获取

                            //if (strX_WorkType == "夜" && dY.Hour < 12)//如果发注作业是夜班，且入荷时间是12点之前，证明是夜班的第二天凌晨
                            //    isOver12 = true;


                            int iRuHeFromDay = Convert.ToInt32(dtStandard.Rows[j]["vcRuHeFromDay"].ToString());//部品入荷起加减数
                            string strBianCi = dtStandard.Rows[j]["vcBianCi"].ToString();//便次名后缀
                            string strRuHeFromTime = dtStandard.Rows[j]["dRuHeFromTime"].ToString(); //入荷起时间
                            int iNaQiFromDay = Convert.ToInt32(dtStandard.Rows[j]["vcNaQiFromDay"].ToString());//纳期起加减数
                            

                            if (strX_WorkType == "白")
                            {
                                if (strY_WorkType == "白")
                                    dY = Convert.ToDateTime(dX).AddDays(iRuHeFromDay);
                                else//入荷是夜班
                                    dY = Convert.ToDateTime(dX).AddDays(iRuHeFromDay+1);
                            }
                            else
                            { //夜班
                                if (dX.Hour >= 12 && dX.Hour < 24)
                                {
                                    dY = Convert.ToDateTime(dX).AddDays(iRuHeFromDay);
                                }
                                else if (dX.Hour >= 0 && dX.Hour < 12 && dY.Hour >= 12 && dY.Hour < 24)
                                {
                                    dY = Convert.ToDateTime(dX).AddDays(iRuHeFromDay-1);
                                }
                                else if (dX.Hour >= 0 && dX.Hour < 12 && dY.Hour >= 0 && dY.Hour < 12)
                                {
                                    dY = Convert.ToDateTime(dX).AddDays(iRuHeFromDay);
                                }
                            }

                            DateTime dTempRuhe = Convert.ToDateTime(dY.ToString("yyyy-MM-dd") + " " + strRuHeFromTime);
                            if (dTempRuhe > dEnd&& dTempRuhe < DateTime.Now)//如果得到的入荷时间起大于最后获取入库时间，且小于当前时间，那么证明需要获取
                            {
                                BcTask task = new BcTask();
                                if (strX_WorkType == "白")
                                {
                                    task.strBCName = dEnd_Index.AddDays(iNaQiFromDay).ToString("yyyy-MM-dd") + " " + strBianCi;
                                }
                                else
                                { //夜班
                                    if (dX.Hour>12)//当天的夜班
                                    {
                                        task.strBCName = dEnd_Index.AddDays(iNaQiFromDay).ToString("yyyy-MM-dd") + " " + strBianCi;
                                    }
                                    else //昨天的夜班
                                    {
                                        task.strBCName = dEnd_Index.AddDays(iNaQiFromDay - 1).ToString("yyyy-MM-dd") + " " + strBianCi;
                                    }
                                }
                                
                                task.strFaZhuID = strFaZhuID;
                                result.Add(task);
                            }
                        }
                        dEnd_Index = dEnd_Index.AddDays(1);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

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
        public void computer(string strFaZhuID, string strUserID, string strPackSpot)
        {
            fs0705_DataAccess.computer(strFaZhuID, strUserID, strPackSpot);
        }
        #endregion

        #region 检索计算结果
        public DataTable searchComputeJG(string strPackSpot)
        {
            DataTable dt = fs0705_DataAccess.searchComputeJG(strPackSpot);
            DataTable returnDT = dt.Clone();
            DataRow[] drs = dt.Select("iF_DingGou > 0");
            for (int i = 0; i < drs.Length; i++)
            {
                returnDT.ImportRow(drs[i]);
            }
            return returnDT;
        }
        #endregion

        #region 导出用检索计算结果
        public DataTable exportSearchJG() 
        {
            return fs0705_DataAccess.searchComputeJGAll();
        }
        #endregion

        #region 生成发注数据的检索计算结果（舍弃订购数量为NULL或者为0的数据）
        public DataTable SCFZDataSearchComputeJG(string strPackSpot)
        {
            DataTable dt = fs0705_DataAccess.searchComputeJG(strPackSpot);
            DataTable returnDT = dt.Clone();
            DataRow[] drs = dt.Select("iF_DingGou > 0");
            for (int i = 0; i < drs.Length; i++)
            {
                returnDT.ImportRow(drs[i]);
            }
            return returnDT;
        }
        #endregion

        #region 检索计算结果
        public DataTable searchComputeJGAll()
        {
            return fs0705_DataAccess.searchComputeJGAll();
        }
        #endregion

        #region 生成订单数据
        public void SCFZData(DataTable dt,string strOrderNo)
        {
            fs0705_DataAccess.SCFZData(dt,strOrderNo);
        }
        #endregion

        #region 获取新订单号
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
                    return getNewOrderNo(dt.Rows[0][0].ToString());
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
            if (strBanZhiLast == "双值" || strBanZhiLast == "夜")
            {
                switch (strBanZhi)
                {
                    case "非稼动":
                        checkList.Add(getBZTime(strPackPlant, "夜", dtLast.ToString("yyyy-MM-dd")));
                        ; break;
                    case "白":
                        checkList.Add(getBZTime(strPackPlant, "夜", dtLast.ToString("yyyy-MM-dd")));
                        checkList.Add(getBZTime(strPackPlant, "白", dT.ToString("yyyy-MM-dd")));
                        ; break;
                    case "夜":
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
            else if (strBanZhiLast == "白" || strBanZhiLast == "非稼动")
            {
                switch (strBanZhi)
                {
                    case "非稼动":
                        ; break;
                    case "白":
                        checkList.Add(getBZTime(strPackPlant, "白", dT.ToString("yyyy-MM-dd")));
                        ; break;
                    case "夜":
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
                if (dT > bZTime.dStart && dT < bZTime.dEnd)
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
        public DataTable search_Sub(string strPackNo, string strPackGPSNo, string strFrom, string strTo, string strType)
        {
            return fs0705_DataAccess.search_Sub(strPackNo, strPackGPSNo, strFrom, strTo, strType);
        }
        #endregion

        #region 调整数据输入-保存
        public void Save_Sub(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs0705_DataAccess.save_Sub(listInfoData, strUserId);
        }
        #endregion

        #region 调整数据输入-导入后保存
        public void importSave_Sub(DataTable dt, string strUserId)
        {
            fs0705_DataAccess.importSave_Sub(dt, strUserId);
        }
        #endregion

        #region 获取品番是否满足保存条件
        public DataTable getIsSave(List<Dictionary<string, Object>> listInfoData) 
        {
            return fs0705_DataAccess.getIsSave(listInfoData);
        }
        #endregion

        #region 获取品番是否满足保存条件
        public DataTable getIsSave(DataTable dt)
        {
            return fs0705_DataAccess.getIsSave(dt);
        }
        #endregion



    }
    public class BcTask
    {
        public string strFaZhuID;//发注逻辑
        public string strBCName;//便次名称
        public DateTime dDate;//便次对应的稼动日，注意不是自然日，比如2021-3-24 2:00:00的夜班，dDate算2021-3-23
    }

    public class BZTime
    {
        public string strBaiYeType;
        public DateTime dStart;
        public DateTime dEnd;
    }
}

