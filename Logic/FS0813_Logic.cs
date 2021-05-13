using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;


namespace Logic
{
    public class FS0813_Logic
    {
        FS0813_DataAccess fs0813_DataAccess;

        public FS0813_Logic()
        {
            fs0813_DataAccess = new FS0813_DataAccess();
        }

        #region 检索
        public DataTable Search(string strSellNo, string strStartTime, string strEndTime,string strYinQuType,string strSHF,string strLabelID,string vcBanZhi,
            string vcQianFen,string vcBianCi)
        {
            return fs0813_DataAccess.Search(strSellNo, strStartTime, strEndTime, strYinQuType, strSHF, strLabelID, vcBanZhi, vcQianFen,vcBianCi);
        }
        #endregion

        #region 子画面初始化
        public DataTable initSubApi(string strSellNo)
        {
            return fs0813_DataAccess.initSubApi(strSellNo);
        }
        public DataTable initSubApi2(string strSellNo)
        {
            return fs0813_DataAccess.initSubApi2(strSellNo);
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs0813_DataAccess.Save(listInfoData, strUserId);
        }
        #endregion
    }
}
