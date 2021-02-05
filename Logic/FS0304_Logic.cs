using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS0304_Logic
    {
        FS0304_DataAccess fs0304_DataAccess;

        public FS0304_Logic()
        {
            fs0304_DataAccess = new FS0304_DataAccess();
        }

        #region 检索
        public DataTable Search(string strSSDateMonth, string strJD, string strPart_id, string strInOutFlag, string strIsDYJG, string strCarType, string strSupplier_id)
        {
            return fs0304_DataAccess.Search(strSSDateMonth, strJD, strPart_id, strInOutFlag, strIsDYJG, strCarType, strSupplier_id);
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            fs0304_DataAccess.Save(listInfoData, strUserId, ref strErrorPartId);
        }
        #endregion

        #region 退回
        public void Back(List<Dictionary<string, Object>> listInfoData, string strUserId,string strEmail,string strUserName, ref string strErrorPartId)
        {
            #region 更新生确进度表
            fs0304_DataAccess.Back(listInfoData, strUserId, ref strErrorPartId);
            #endregion
            
            #region 给供应商发邮件

            #endregion
        }
        #endregion

        #region 付与日期一括付与
        public void DateKFY(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId, string dTFTM_BJ)
        {
            fs0304_DataAccess.DateKFY(listInfoData, strUserId, ref strErrorPartId, dTFTM_BJ);
        }
        #endregion

        #region 织入原单位
        public void sendUnit(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            fs0304_DataAccess.sendUnit(listInfoData, strUserId, ref strErrorPartId);
        }
        #endregion
    }
}
