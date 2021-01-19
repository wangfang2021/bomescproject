using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Common;
using DataAccess;
using DataEntity;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Logic
{
    public class FS0610_Logic
    {
        FS0610_DataAccess fs0610_DataAccess = new FS0610_DataAccess();

        #region 取日历
        public DataTable GetCalendar(string strPlant,string vcDXYM)
        {
            return fs0610_DataAccess.GetCalendar(strPlant, vcDXYM);
        }
        #endregion

        #region 取soq数据
        public DataTable GetSoq(string strPlant,string strDXYM,string strType)
        {
            return fs0610_DataAccess.GetSoq(strPlant,strDXYM,strType);
        }
        #endregion

        #region 更新平准化结果
        public void SaveResult(string strCLYM,string strDXYM,string strNSYM,string strNNSYM, string strPlant,
            ArrayList arrResult_DXYM, ArrayList arrResult_NSYM, ArrayList arrResult_NNSYM,string strUserId)
        {
            fs0610_DataAccess.SaveResult(strCLYM, strDXYM, strNSYM, strNNSYM, strPlant,
             arrResult_DXYM,  arrResult_NSYM,  arrResult_NNSYM, strUserId);
        }
        #endregion

        #region 展开SOQReply
        public int zk(string varDxny, string userId)
        {
            return fs0610_DataAccess.zk(varDxny, userId);
        }
        #endregion

        #region 下载SOQReply（检索内容）
        public DataTable search(string varDxny)
        {
            return fs0610_DataAccess.search(varDxny);
        }
        #endregion

        #region 导入后保存
        public void importSave(DataTable dt, string varDxny)
        {
            fs0610_DataAccess.importSave(dt, varDxny);
        }
        #endregion
    }
}
