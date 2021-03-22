using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Common;
using DataAccess;
using DataEntity;

namespace Logic
{
    public class FS0611_Logic
    {
        FS0611_DataAccess fs0611_DataAccess = new FS0611_DataAccess();

        #region 取日历
        public DataTable GetCalendar(string strPlant, string vcDXYM)
        {
            return fs0611_DataAccess.GetCalendar(strPlant, vcDXYM);
        }
        #endregion

        #region 取soq数据
        public DataTable GetSoq(string strPlant, string strYearMonth, string strType)
        {
            return fs0611_DataAccess.GetSoq(strPlant, strYearMonth, strType);
        }
        #endregion


        #region 取soq数据-已合意
        public DataTable GetSoqHy(string strPlant, string strYearMonth, string strType)
        {
            return fs0611_DataAccess.GetSoqHy(strPlant, strYearMonth, strType);
        }
        #endregion
        

        #region 取特殊厂家对应的品番
        public DataTable GetSpecialSupplier(string strPlant, string strDXYearMonth, string strYearMonth)
        {
            return fs0611_DataAccess.GetSpecialSupplier(strPlant, strDXYearMonth, strYearMonth);
        }
        #endregion


        #region 取特殊品番
        public DataTable GetSpecialPartId(string strPlant, string strDXYearMonth, string strYearMonth)
        {
            return fs0611_DataAccess.GetSpecialPartId(strPlant, strDXYearMonth, strYearMonth);
        }
        #endregion

        #region 更新平准化结果
        public void SaveResult(string strCLYM, string strDXYM, string strNSYM, string strNNSYM, string strPlant,
            ArrayList arrResult_DXYM, ArrayList arrResult_NSYM, ArrayList arrResult_NNSYM, string strUserId,string strUnit)
        {
            fs0611_DataAccess.SaveResult(strCLYM, strDXYM, strNSYM, strNNSYM, strPlant,
             arrResult_DXYM, arrResult_NSYM, arrResult_NNSYM, strUserId,strUnit);
        }
        #endregion

        #region 获取没有展开的数据
        public DataTable getZhankaiData(bool isZhankai)
        {
            return fs0611_DataAccess.getZhankaiData(isZhankai);
        }
        #endregion

        #region 展开SOQReply
        public int zk( string userId)
        {
            return fs0611_DataAccess.zk(userId);
        }
        #endregion

        #region 下载SOQReply（检索内容）
        public DataTable search(string strYearMonth, string strYearMonth_2, string strYearMonth_3)
        {
            return fs0611_DataAccess.search(strYearMonth,strYearMonth_2,strYearMonth_3);
        }
        #endregion

        #region 导入后保存
        public void importSave(DataTable dt, string strYearMonth, string strUserId)
        {
            fs0611_DataAccess.importSave(dt, strYearMonth, strUserId);
        }
        #endregion


        #region 获取平准化加减天数
        public int getPingZhunAddSubDay()
        {
            DataTable dt=fs0611_DataAccess.getPingZhunAddSubDay();
            if (dt == null || dt.Rows.Count == 0)
                return 0;
            return Convert.ToInt32(dt.Rows[0]["vcValue1"]);
        }
        #endregion
    }
}
