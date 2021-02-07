﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;
using Common;

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
        public DataTable Search(string strSSDate, string strJD, string strPart_id, string strInOutFlag, string strIsDYJG, string strCarType, string strSupplier_id)
        {
            return fs0304_DataAccess.Search(strSSDate, strJD, strPart_id, strInOutFlag, strIsDYJG, strCarType, strSupplier_id);
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            fs0304_DataAccess.Save(listInfoData, strUserId, ref strErrorPartId);
        }
        #endregion

        #region 删除
        public void del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs0304_DataAccess.Del(listInfoData, strUserId);
        }
        #endregion

        #region 退回
        public void Back(List<Dictionary<string, Object>> listInfoData, string strUserId,string strTH, string strEmail,string strUserName, ref string strErr)
        {
            #region 更新生确进度表
            fs0304_DataAccess.Back(listInfoData, strUserId,strTH);
            #endregion

            #region 给供应商发邮件
            DataTable dtSetting = getEmailSetting(strUserId);
            string strTitle = "";//邮件标题
            string strContent = "";//邮件内容
            if (dtSetting == null || dtSetting.Rows.Count == 0)
            {
                strErr = "退回成功，但用户" + strUserId + "邮件内容没配置，邮件发送终止！";
                return;
            }
            else
            {
                strTitle = dtSetting.Rows[0]["vcTitle"].ToString();
                strContent = dtSetting.Rows[0]["vcContent"].ToString();
                var dateTime = DateTime.Now.ToString("yyyy年MM月");
                strContent = strContent.Replace("##yearmonth##", dateTime);
                /*这里的年月要进行特殊处理，发邮件时年月的格式要变成‘YYYY年MM月‘格式*/
                /*但是生确单发行时的格式是’YYYY-MM‘*/
            }
            //再向供应商发邮件
            StringBuilder strEmailBody = new StringBuilder();
            for (int i = 0; i < listInfoData.Count; i++)
            {
                string strSupplier_id = listInfoData[i]["vcSupplier_id"].ToString();
                DataTable receiverDt = getSupplierEmail(strSupplier_id);
                if (receiverDt == null)
                {
                    strErr += "未找到 '" + strSupplier_id + "' 供应商邮件信息";
                    return;
                }
                ComFunction.SendEmailInfo(strEmail, strUserName, strContent, receiverDt, null, strTitle, "", false);
            }
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
        public void sendUnit(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErr)
        {
            fs0304_DataAccess.sendUnit(listInfoData, strUserId, ref strErr);
        }
        #endregion

        #region 根据供应商获取邮箱地址
        public DataTable getSupplierEmail(string strSupplierId)
        {
            DataTable dt = fs0304_DataAccess.getSupplierEmail(strSupplierId);
            if (dt == null || dt.Rows.Count == 0)
                return null;
            else
                return dt;
        }
        #endregion

        #region 获取发件人的邮件内容配置
        public DataTable getEmailSetting(string strUserId)
        {
            DataTable dt = fs0304_DataAccess.getEmailSetting(strUserId);
            if (dt == null || dt.Rows.Count == 0)
                return null;
            else
                return dt;
        }
        #endregion
    }
}
