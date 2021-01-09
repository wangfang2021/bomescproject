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
        public void sqSend(List<Dictionary<string, Object>> listInfoData,string strSqDate, string strUserId,string strEmail,string strUserName,ref string strErr)
        {
            //1、更新原单位纳期 2、更新生确单
            fs0303_DataAccess.sqSend(listInfoData, strSqDate, strUserId);

            DataTable dtSetting=getEmailSetting(strUserId);
            string strTitle = "";//邮件标题
            string strContent = "";//邮件内容
            if (dtSetting == null || dtSetting.Rows.Count == 0)
            {
                strErr = "数据发送成功，但用户"+ strUserId + "邮件内容没配置，邮件发送终止！";
                return;
            }
            else
            {
                strTitle = dtSetting.Rows[0]["vcTitle"].ToString();
                strContent = dtSetting.Rows[0]["vcContent"].ToString();
                strContent = strContent.Replace("##yearmonth##", strSqDate);
            }
            //再向供应商发邮件
            StringBuilder strEmailBody = new StringBuilder();
            for (int i = 0; i < listInfoData.Count; i++)
            {
                string strSupplier_id = listInfoData[i]["vcSupplier_id"].ToString();
                DataTable receiverDt = getSupplierEmail(strSupplier_id);
                ComFunction.SendEmailInfo(strEmail, strUserName, strContent, receiverDt, null, strTitle, "", false);
            }
        }
        #endregion

        #region 根据供应商获取邮箱地址
        public DataTable getSupplierEmail(string strSupplierId) {
            DataTable dt= fs0303_DataAccess.getSupplierEmail(strSupplierId);
            if (dt == null || dt.Rows.Count == 0)
                return null;
            else
                return dt;
        }
        #endregion

        #region 获取发件人的邮件内容配置
        public DataTable getEmailSetting(string strUserId)
        {
            DataTable dt = fs0303_DataAccess.getEmailSetting(strUserId);
            if (dt == null || dt.Rows.Count == 0)
                return null;
            else
                return dt;
        }
        #endregion


    }
}
