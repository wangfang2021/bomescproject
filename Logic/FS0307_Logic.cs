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

        public DataTable searchApi(string strYear, string FinishFlag)
        {
            return fs0307_dataAccess.searchApi(strYear, FinishFlag);
        }

        #endregion

        #region 年限抽取

        public void extractPart(string strUserId)
        {
            fs0307_dataAccess.extractPart(strUserId);
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
    }
}
