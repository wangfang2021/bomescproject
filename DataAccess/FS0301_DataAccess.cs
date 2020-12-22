using System;
using Common;
using System.Data;
using System.Text;
using System.Collections.Generic;
using Org.BouncyCastle.Crypto;

namespace DataAccess
{
    public class FS0301_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        public DataTable getList(string iState, string dOperatorTime)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.Append(" SELECT iAutoId,'0' as vcModFlag,vcFileNameTJ,case vcState WHEN 0 THEN '未开封' WHEN 1 THEN '已开封' WHEN 2 THEN '已完成' END AS State ,vcRemark,dOperatorTime FROM TSBFile \r\n");
                sbr.Append(" WHERE 1=1  \r\n");
                if (!string.IsNullOrWhiteSpace(iState))
                {
                    sbr.Append(" AND vcState = '" + iState + "' \r\n");
                }
                if (!string.IsNullOrWhiteSpace(dOperatorTime))
                {
                    sbr.Append(" AND dOperatorTime = '" + dOperatorTime + "' \r\n");
                }
                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void updateState(string fileName)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.Append("UPDATE TSBFile SET vcState = 1 WHERE vcState = 0 AND vcFileNameTJ = '" + fileName + "' \r\n");
                excute.ExcuteSqlWithStringOper(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    if (bModFlag == true)
                    {//修改
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                        string vcRemark = listInfoData[i]["vcRemark"].ToString();

                        sbr.Append(" UPDATE TSBFile SET vcRemark = '" + vcRemark + "' WHERE iAutoId = " + iAutoId + " \r\n");

                    }
                    excute.ExcuteSqlWithStringOper(sbr.ToString());
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("-->") != -1)
                {//主动判断抛出的异常
                    int startIndex = ex.Message.IndexOf("-->");
                    int endIndex = ex.Message.LastIndexOf("<--");
                    strErrorPartId = ex.Message.Substring(startIndex + 3, endIndex - startIndex - 3);
                }
                else
                    throw ex;
            }
        }
        #endregion
    }
}