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
    public class FS0304_Logic
    {
        FS0304_DataAccess fs0304_DataAccess;

        public FS0304_Logic()
        {
            fs0304_DataAccess = new FS0304_DataAccess();
        }

        #region 检索
        public DataTable Search(string strSSDate, string strJD, string strPart_id, string strInOutFlag, string strIsDYJG, string strCarType, string strSupplier_id, string strUserOriginCompany,string strUserID)
        {
            return fs0304_DataAccess.Search(strSSDate, strJD, strPart_id, strInOutFlag, strIsDYJG, strCarType, strSupplier_id,strUserOriginCompany,strUserID);
        }
        #endregion

        #region 初始化检索
        public DataTable Search(string strUserID)
        {
            DataTable userOriginCompanyDT = fs0304_DataAccess.getUserOriginCompany(strUserID);
            if (userOriginCompanyDT==null || userOriginCompanyDT.Rows.Count<=0 || userOriginCompanyDT.Rows[0][0]==null || userOriginCompanyDT.Rows[0][0].ToString()=="")
            {
                return null;
            }

            return fs0304_DataAccess.Search(userOriginCompanyDT.Rows[0][0].ToString());
        }
        #endregion

        #region 织入原单位前校验品番在原单位表中是否存在
        public DataTable getPartidExistsInUnit(List<Dictionary<string, Object>> listInfoData,string strUserId,ref string strErr)
        {
            return fs0304_DataAccess.getPartidExistsInUnit(listInfoData, strUserId, ref strErr);
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs0304_DataAccess.Save(listInfoData, strUserId);
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

            #region 获取登陆人的邮件模板(邮件标题和邮件内容)，未找到：返回错误提示
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
                var dateTime = DateTime.Now.ToString("yyyy年MM月dd日");
                strContent = strContent.Replace("##yearmonth##", dateTime);
            }
            #endregion

            //再向供应商发邮件
            StringBuilder strEmailBody = new StringBuilder();
            for (int i = 0; i < listInfoData.Count; i++)
            {
                string strSupplier_id = listInfoData[i]["vcSupplier_id"].ToString();
                DataTable receiverDt = getSupplierEmail(strSupplier_id);
                if (receiverDt == null)
                {
                    strErr += "退回成功，但未找到 '" + strSupplier_id + "' 供应商邮箱地址 ";
                    return;
                }
                ComFunction.SendEmailInfo(strEmail, strUserName, strContent, receiverDt, null, strTitle, "", false);
            }
            #endregion
        }
        #endregion

        #region 付与日期一括付与
        public void DateKFY(List<Dictionary<string, Object>> listInfoData, string strUserId,string dTFTM_BJ)
        {
            fs0304_DataAccess.DateKFY(listInfoData, strUserId, dTFTM_BJ);
        }
        #endregion

        #region 织入原单位
        public void sendUnit(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErr)
        {
            #region 校验所选品番在原单位中是否存在，如果不存在，提示并报错
            DataTable dt = getPartidExistsInUnit(listInfoData, strUserId, ref strErr);
            if (strErr!="")
            {
                return;
            }
            if (dt!=null && dt.Rows.Count>0)
            {
                strErr = "织入失败！以下品番在原单位表中不存在：";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    strErr += dt.Rows[i]["vcPart_id"].ToString()+" ";
                }
                return;
            }
            #endregion
            else
            {
                #region 校验所选品番的TFTM调整时间是否大于等于原单位品番开始时间
                dt = getCheckFromDate(listInfoData, strUserId, ref strErr);
                if (strErr!="")
                {
                    return;
                }
                if (dt!=null && dt.Rows.Count>0)
                {
                    strErr = "织入失败！以下品番TFTM调整时间小于原单位品番开始时间：";
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        strErr += dt.Rows[i]["vcPart_id"].ToString() + " ";
                    }
                    return;
                }
                #endregion

                #region 校验所选品番的TFTM调整时间大于品番开始时间并且TFTM调整时间大于等于原单位工程开始时间
                dt = getCheckGYSFromDate(listInfoData, strUserId, ref strErr);
                if (strErr!="")
                {
                    return;
                }
                if (dt!=null && dt.Rows.Count>0)
                {
                    strErr = "织入失败！以下品番TFTM调整时间小于原单位品番开始时间或小于原单位供应商开始时间：";
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        strErr += dt.Rows[i]["vcPart_id"].ToString() + " ";
                    }
                    return;
                }
                #endregion
            }
            
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

        #region 变更事项为设变-废止、供应商变更-废止、工程变更-废止、包装工厂变更-废止的数据，验证生确TFTM调整时间是否大于等于原单位品番开始时间
        public DataTable getCheckFromDate(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErr)
        {
            return fs0304_DataAccess.getCheckFromDate(listInfoData, strUserId, ref strErr);
        }
        #endregion

        #region 变更事项为设变-废止、供应商变更-废止、工程变更-废止、包装工厂变更-废止的数据，验证生确TFTM调整时间是否大于等于原单位品番开始时间
        public DataTable getCheckGYSFromDate(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErr)
        {
            return fs0304_DataAccess.getCheckGYSFromDate(listInfoData, strUserId, ref strErr);
        }
        #endregion

        #region 获取当前登陆用户担当的原单位
        public DataTable getUserOriginCompany(string strUserID)
        {
            DataTable dt = fs0304_DataAccess.getUserOriginCompany(strUserID);
            if (dt==null || dt.Rows.Count<=0 || dt.Rows[0][0] == null || dt.Rows[0][0].ToString()=="")
            {
                return null;
            }
            string strUserOriginCompany = dt.Rows[0][0].ToString();

            #region 担当原单位字符串格式化
            strUserOriginCompany = strUserOriginCompany.Replace("，",",");           //如果字符串中存在中文字符，则转换为英文字符
            if (strUserOriginCompany.Substring(strUserOriginCompany.Length-1,1)==",")//去掉结尾的逗号
            {
                strUserOriginCompany = strUserOriginCompany.Substring(0, strUserOriginCompany.Length - 1);
            }
            if (strUserOriginCompany.Substring(0, 1) == ",")//去掉开头的逗号
            {
                strUserOriginCompany = strUserOriginCompany.Substring(1);
            }
            #endregion

            DataTable returnDt = new DataTable();
            returnDt.Columns.Add("vcName");
            returnDt.Columns.Add("vcValue");

            for (int i = 0; i < strUserOriginCompany.Split(",").Length; i++)
            {
                DataRow dr = returnDt.NewRow();
                dr["vcName"] = strUserOriginCompany.Split(",")[i];
                dr["vcValue"] = strUserOriginCompany.Split(",")[i];
                returnDt.Rows.Add(dr);
            }

            return returnDt;
        }
        #endregion
    }
}
