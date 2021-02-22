using System;
using System.Collections.Generic;
using Common;
using System.Data;
using System.Text;

namespace DataAccess
{
    public class FS0320_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        public DataTable searchApi(string vcPart_id, string vcPartNameEn, string vcPartNameCn, string vcState)
        {
            StringBuilder sbr = new StringBuilder();
            sbr.Append(" SELECT iAutoId,vcPart_id,vcPartNameEn,LEFT(vcPart_id,5) AS vcPart_id_short,vcPartNameCn,'0' as vcModFlag,'0' as vcAddFlag,vcIsLock FROM TPartNameCN \r\n ");
            sbr.Append(" WHERE 1=1  ");
            if (!string.IsNullOrWhiteSpace(vcState))
            {
                sbr.Append(" AND vcIsLock = '" + vcState + "'");
            }
            //TODO 有效时间是否需要
            if (!string.IsNullOrWhiteSpace(vcPart_id))
            {
                sbr.Append(" AND ISNULL(vcPart_id,'') LIKE '" + vcPart_id + "%' ");
            }
            if (!string.IsNullOrWhiteSpace(vcPartNameEn))
            {
                sbr.Append(" AND ISNULL(vcPartNameEn,'') LIKE '" + vcPartNameEn + "%' ");
            }
            if (!string.IsNullOrWhiteSpace(vcPartNameCn))
            {
                sbr.Append(" AND ISNULL(vcPartNameCn,'') LIKE '" + vcPartNameCn + "%' ");
            }

            sbr.Append(" ORDER BY vcIsLock ");
            return excute.ExcuteSqlWithSelectToDT(sbr.ToString(), "TK");
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
                    string flag = listInfoData[i]["vcIsLock"].ToString();
                    string vcPart_id = listInfoData[i]["vcPart_id"].ToString();
                    if (bModFlag == true && flag.Equals("0"))
                    {//修改
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);

                        sbr.Append(" UPDATE TPartNameCN SET vcPartNameCn = " + ComFunction.getSqlValue(listInfoData[i]["vcPartNameCn"], false) + ",dOperatorTime = GETDATE(),vcOperator = '" + strUserId + "',vcIsLock = '1' WHERE iAutoId = " + iAutoId + "  \r\n");
                        sbr.Append(" UPDATE TUnit SET vcPartNameCn = " + ComFunction.getSqlValue(listInfoData[i]["vcPartNameCn"], false) + ",vcOperator = '" + strUserId + "',dOperatorTime = GETDATE() ");
                        sbr.Append(" WHERE vcPart_id = " + ComFunction.getSqlValue(listInfoData[i]["vcPart_id"], false) + "\r\n");
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(strErrorPartId))
                        {
                            strErrorPartId += ",";
                        }

                        strErrorPartId += vcPart_id;
                    }

                    if (sbr.Length > 0)
                    {
                        excute.ExcuteSqlWithStringOper(sbr.ToString(), "TK");
                    }
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


        public void importSave(DataTable dt, string strUserId, string strUnitCode)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sbr.Append(" UPDATE TPartNameCN SET  \r\n ");
                    sbr.Append(" vcPartNameCn = " + ComFunction.getSqlValue(dt.Rows[i]["vcPartNameCn"], false) + ", ");
                    sbr.Append(" dOperatorTime = GETDATE(), ");
                    sbr.Append(" vcOperator = '" + strUserId + "',vcIsLock = '2' \r\n");
                    sbr.Append(" where vcPart_id = " + ComFunction.getSqlValue(dt.Rows[i]["vcPart_id"], false) + " AND vcIsLock = '0'  \r\n");
                }
                if (sbr.Length > 0)
                {
                    sbr.Append(" UPDATE TUnit SET vcPartNameCn = b.vcPartNameCn from  ");
                    sbr.Append(" TUnit a");
                    sbr.Append(" LEFT JOIN ");
                    sbr.Append(" (SELECT vcPart_id,vcPartNameCn,vcIsLock FROM TPartNameCN) b ON a.vcPart_id = b.vcPart_id");
                    sbr.Append(" WHERE b.vcIsLock = '2'");
                    sbr.Append(" UPDATE TPartNameCN SET vcIsLock = '1' WHERE vcIsLock = '2'");

                    excute.ExcuteSqlWithStringOper(sbr.ToString(), "TK");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}