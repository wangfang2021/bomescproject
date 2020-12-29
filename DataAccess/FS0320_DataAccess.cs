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

        public DataTable searchApi(string vcPart_id, string vcPartNameEn, string vcPartNameCn, string strSYT)
        {
            StringBuilder sbr = new StringBuilder();
            sbr.Append(" SELECT iAutoId,vcPart_id,vcPartNameEn,LEFT(vcPart_id,5) AS vcPart_id_short,vcPartNameCn,'0' as vcModFlag,'0' as vcAddFlag FROM Tunit \r\n ");
            sbr.Append(" WHERE dTimeFrom <= GETDATE() AND dTimeTo >= GETDATE()");
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
            if (!string.IsNullOrWhiteSpace(strSYT))
            {
                sbr.Append(" AND vcSYTCode = (SELECT vcValue FROM TCode WHERE vcCodeId = 'C016' AND vcName = '" + strSYT + "') ");
            }
            sbr.Append(" AND  vcChange IN(SELECT vcValue FROM Tcode WHERE vcCodeId = 'C002' AND vcName LIKE '%新设%') ");

            return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
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

                        sbr.Append(" UPDATE TUnit SET vcPartNameCn = " + ComFunction.getSqlValue(listInfoData[i]["vcPartNameCn"], false) + ",dOperatorTime = GETDATE(),vcOperator = '" + strUserId + "' WHERE iAutoId = " + iAutoId + " \r\n");

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


        public void importSave(DataTable dt, string strUserId, string strUnitCode)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sbr.Append(" UPDATE TUnit SET  \r\n ");
                    sbr.Append(" vcPartNameCn = " + ComFunction.getSqlValue(dt.Rows[i]["vcPartNameCn"], false) + ", ");
                    sbr.Append(" dOperatorTime = GETDATE(), ");
                    sbr.Append(" vcOperator = '" + strUserId + "' \r\n");
                    sbr.Append(" where  dTimeFrom <= GETDATE() AND dTimeTo >= GETDATE() ");
                    sbr.Append(" and vcSYTCode = (SELECT vcValue FROM TCode WHERE vcCodeId = 'C016' AND vcName = '" + strUnitCode + "') ");
                    sbr.Append(" and vcPart_id = " + ComFunction.getSqlValue(dt.Rows[i]["vcPart_id"], false) + "  \r\n");
                }
                if (sbr.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(sbr.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}