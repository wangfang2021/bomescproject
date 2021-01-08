using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;
using Common;


namespace Logic
{
    public class FS0303_Logic
    {
        FS0303_DataAccess fs0303_DataAccess;

        public FS0303_Logic()
        {
            fs0303_DataAccess = new FS0303_DataAccess();
        }

        #region 检索
        public DataTable Search(string strIsShowAll,string strOriginCompany)
        {
            return fs0303_DataAccess.Search(strIsShowAll, strOriginCompany);
        }
        #endregion

        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs0303_DataAccess.Del(listInfoData, strUserId);
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            fs0303_DataAccess.Save(listInfoData, strUserId, ref strErrorPartId);
        }
        #endregion

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId)
        {
            fs0303_DataAccess.importSave(dt, strUserId);
        }
        #endregion

        #region 生确单发行
        public void sqSend(List<Dictionary<string, Object>> listInfoData, string strUserId,string strEmail,string strUserName)
        {
            //先更新生确单
            fs0303_DataAccess.sqSend(listInfoData, strUserId);
            //再向供应商发邮件




            //ComFunction.SendEmailInfo()


        }
        #endregion
    }
}
