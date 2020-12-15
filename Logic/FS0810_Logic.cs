using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0810_Logic
    {
        FS0810_DataAccess fs0810_DataAccess;

        public FS0810_Logic()
        {
            fs0810_DataAccess = new FS0810_DataAccess();
        }

        #region 检索
        public DataTable Search(string smallpm, string sr, string pfbefore5)
        {
            return fs0810_DataAccess.Search(smallpm, sr, pfbefore5);
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs0810_DataAccess.Save(listInfoData, strUserId);
        }
        #endregion

        #region 删除
        public void Del(List<Dictionary<string, Object>> checkedInfoData, string strUserId)
        {
            fs0810_DataAccess.Del(checkedInfoData, strUserId);
        }
        #endregion

        #region 检索_品目
        public DataTable Search_PM(string smallpm, string bigpm)
        {
            return fs0810_DataAccess.Search_PM(smallpm, bigpm);
        }
        #endregion

        #region 保存_品目
        public void Save_pm(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs0810_DataAccess.Save_pm(listInfoData, strUserId);
        }
        #endregion

        #region 取得关系表中所有大品目
        public DataTable GetBigPM()
        {
            return fs0810_DataAccess.GetBigPM();
        }
        #endregion

        #region 取得关系表中所有小品目
        public DataTable GetSmallPM()
        {
            return fs0810_DataAccess.GetSmallPM();
        }
        #endregion

        #region 删除_品目
        public void Del_pm(List<Dictionary<string, Object>> checkedInfoData, string strUserId)
        {
            fs0810_DataAccess.Del_pm(checkedInfoData, strUserId);
        }
        #endregion

        #region 检索_基准时间
        public DataTable Search_StandardTime(string bigpm, string standardtime)
        {
            return fs0810_DataAccess.Search_StandardTime(bigpm, standardtime);
        }
        #endregion

        #region 大品目、基准时间是否重复，返回true存在，false不存在
        public DataTable GetStandardTime()
        {
            return fs0810_DataAccess.GetStandardTime();
        }
        #endregion

        #region 保存_基准时间
        public void Save_Standardtime(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs0810_DataAccess.Save_Standardtime(listInfoData, strUserId);
        }
        #endregion

        #region 删除_品目
        public void Del_Standardtime(List<Dictionary<string, Object>> checkedInfoData, string strUserId)
        {
            fs0810_DataAccess.Del_Standardtime(checkedInfoData, strUserId);
        }
        #endregion

        #region 取得品目信息维护表中信息
        public DataTable GetPMSmall()
        {
            return fs0810_DataAccess.GetPMSmall();
        }
        #endregion


    }
}
