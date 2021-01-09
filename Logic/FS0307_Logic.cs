using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Common;
using DataAccess;

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

        public void ZKZP(List<Dictionary<string, Object>> listInfoData, string strUserId, string emailBody)
        {
            try
            {
                //邮件体

                for (int i = 0; i < listInfoData.Count; i++)
                {
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                    //发送邮件,带附件？

                    //成功记录
                    fs0307_dataAccess.ZKZP(iAutoId, strUserId);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
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
