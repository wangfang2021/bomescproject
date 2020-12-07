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
        public void Save(DataTable dtadd, DataTable dtmod, string strUserId)
        {
            fs0810_DataAccess.Save(dtadd, dtmod, strUserId);
        }
        #endregion

        #region 删除
        public void Del(DataTable dtdel, string strUserId)
        {
            fs0810_DataAccess.Del(dtdel, strUserId);
        }
        #endregion

        #region 检索_品目
        public DataTable Search_PM(string smallpm, string bigpm)
        {
            return fs0810_DataAccess.Search_PM(smallpm, bigpm);
        }
        #endregion

        #region 保存_品目
        public void Save_pm(DataTable dtadd, string strUserId)
        {
            fs0810_DataAccess.Save_pm(dtadd, strUserId);
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
        public void Del_pm(DataTable dtdel, string strUserId)
        {
            fs0810_DataAccess.Del_pm(dtdel, strUserId);
        }
        #endregion

        #region 检索_基准时间
        public DataTable Search_StandardTime(string bigpm, string standardtime)
        {
            return fs0810_DataAccess.Search_StandardTime(bigpm, standardtime);
        }
        #endregion

        #region 大品目、基准时间是否重复，返回true存在，false不存在
        public bool IsExit(string bigpm, string standardtime)
        {
            int rows = fs0810_DataAccess.GetStandardTime(bigpm, standardtime);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 保存_基准时间
        public void Save_Standardtime(DataTable dtadd, string strUserId)
        {
            fs0810_DataAccess.Save_Standardtime(dtadd, strUserId);
        }
        #endregion

        #region 删除_品目
        public void Del_Standardtime(DataTable dtdel, string strUserId)
        {
            fs0810_DataAccess.Del_Standardtime(dtdel, strUserId);
        }
        #endregion
    }
}
