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
        public DataTable Search(string strSellNo, string strStartTime, string strEndTime,string strYinQuType)
        {
            return fs0813_DataAccess.Search(strSellNo, strStartTime, strEndTime, strYinQuType);
        }
        #endregion

        #region 子画面初始化
        public DataTable initSubApi(string strSellNo)
        {
            return fs0813_DataAccess.initSubApi(strSellNo);
        }
        #endregion

    }
}
