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

        #region 受入号+品番前5位+包材品番 不能重复
        public bool RepeatCheck(string vcSR, string vcPartsNoBefore5, string vcBCPartsNo)
        {
            int num = fs0810_DataAccess.RepeatCheck(vcSR, vcPartsNoBefore5, vcBCPartsNo);
            if (num > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion 

        #region 小品目 不能重复
        public bool RepeatCheckSmall(string vcSmallPM,string strMode,string strAutoId)
        {
            int num = fs0810_DataAccess.RepeatCheckSmall(vcSmallPM, strMode, strAutoId);
            if (num > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion 

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId)
        {
            fs0810_DataAccess.importSave(dt, strUserId);
        }
        #endregion

        #region 大品目 不能重复
        public bool RepeatCheckStandardTime(string vcBigPM)
        {
            int num = fs0810_DataAccess.RepeatCheckStandardTime(vcBigPM);
            if (num > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion 

        #region 导入后保存-基准时间
        public void importSave_StandardTime(DataTable dt, string strUserId)
        {
            fs0810_DataAccess.importSave_StandardTime(dt, strUserId);
        }
        #endregion

    }
}
