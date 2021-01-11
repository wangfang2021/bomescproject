using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using Common;
using DataAccess;
using Microsoft.AspNetCore.Http;

namespace Logic
{
    public class FS0307_Logic
    {
        FS0307_DataAccess fs0307_dataAccess = new FS0307_DataAccess();

        #region 检索

        public DataTable searchApi(string strYear, string FinishFlag, string SYT, string Receiver, List<string> origin)
        {
            return fs0307_dataAccess.searchApi(strYear, FinishFlag, SYT, Receiver, origin);
        }

        #endregion

        #region 删除
        public void DelApi(List<Dictionary<string, Object>> listInfoData)
        {
            fs0307_dataAccess.DelApi(listInfoData);
        }
        #endregion

        #region 年限抽取

        public void extractPart(string strUserId, List<string> vcOriginCompany)
        {
            fs0307_dataAccess.extractPart(strUserId, vcOriginCompany);
        }

        #endregion

        #region FTMS

        public void FTMS(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs0307_dataAccess.FTMSCB(listInfoData, strUserId);
        }

        #endregion

        #region 展开账票

        public string CreateEmailBody(string date)
        {
            StringBuilder sbr = new StringBuilder();
            sbr.AppendLine("展开期限");
            sbr.AppendLine(date);
            return sbr.ToString();
        }

        public bool ZKZP(List<Dictionary<string, Object>> listInfoData, string strUserId, string emailBody)
        {
            try
            {
                bool isSuccess = true;
                //记录列表
                List<MailSend> list = new List<MailSend>();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAuto_id"]);
                    string supplierId = listInfoData[i]["vcSupplier_id"].ToString();
                    int index = -1;
                    for (int j = 0; j < list.Count; j++)
                    {
                        if (list[j].supplierId.Equals(supplierId))
                        {
                            index = j;
                            break;
                        }
                    }

                    if (index == -1)
                    {
                        list.Add(new MailSend(supplierId, iAutoId));
                    }
                    else
                    {
                        list[index].id.Add(iAutoId);
                    }
                }
                //对每个供应商发送邮件，成功则记录
                for (int i = 0; i < list.Count; i++)
                {
                    //TODO 生成附件

                    //TODO 发送邮件

                    //成功发送邮件,记录结果
                    fs0307_dataAccess.ZKZP(list[i].id, strUserId);
                }

                return isSuccess;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region MailClass

        public class MailSend
        {
            public MailSend(string supplierId, int id)
            {
                this.supplierId = supplierId;
                this.id = new List<int>() { id };
            }
            public string supplierId;
            public List<int> id;

        }

        #endregion

        #region 导入后保存

        public void importSave(DataTable dt, string strUserId)
        {
            fs0307_dataAccess.importSave(dt, strUserId);
        }

        #endregion

        #region 保存

        public void SaveApi(List<Dictionary<string, Object>> list, string strUserId)
        {
            fs0307_dataAccess.SaveApi(list, strUserId);
        }

        #endregion

        #region 织入原单位

        public void InsertUnitApi(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            fs0307_dataAccess.InsertUnitApi(listInfoData, strUserId);
        }

        #endregion
    }
}
