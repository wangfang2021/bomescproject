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
 


        #region 发注便次更新
        public DataTable SearchFaZhuTime(string PackSpot)
        {
            try
            {
                //一、取目前系统中需要计算的“逻辑名称”对应的入荷时间“起”
                DataTable dtFanWei = fs0705_DataAccess.SearchFaZhuTime();
                for (int i = 0; i < dtFanWei.Rows.Count; i++)
                {
                    string strPackSpot = dtFanWei.Rows[i]["vcPackSpot"].ToString();
                    string strFaZhuID = dtFanWei.Rows[i]["vcFaZhuID"].ToString();
                    string strEnd = dtFanWei.Rows[i]["dEnd"]==DBNull.Value?"":Convert.ToDateTime(dtFanWei.Rows[i]["dEnd"]).ToString("yyyy-MM-dd HH:mm:ss");
                    if (strEnd == "")//一次都没有计算过，只算当天的
                    {

                    }
                    else 
                    {
                        DateTime dEnd = DateTime.Parse(strEnd);
                        while (((TimeSpan)(DateTime.Now - dEnd)).Days>=0)
                        {
                            string strBanZhi = getPackBanZhi(dEnd.ToString("yyyy-MM"),dEnd.Day);
                            if (strBanZhi == "无稼动")
                            {
                                string strBanZhiStart=getBanZhiStart(strPackSpot);
                                dEnd = Convert.ToDateTime(dEnd.AddDays(1).ToString("yyyy-MM-dd") +" "+ strBanZhiStart);
                            }
                            else if (strBanZhi == "双值")
                            { 
                            
                            }
                            else if (strBanZhi == "白值")
                            {

                            }
                            else if (strBanZhi == "夜值")
                            {

                            }
                        }
                    }
                }

      




                return dtFanWei;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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

        #region 取工厂对应白班的起始时间
        public string getBanZhiStart(string strPackPlant)
        {
            DataTable dt = fs0705_DataAccess.getBanZhiStart(strPackPlant);
            if (dt == null || dt.Rows.Count == 0)
                return "";
            else
                return dt.Rows[0]["tFromTime"].ToString();
        }
        #endregion


    }
}
