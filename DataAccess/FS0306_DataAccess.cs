using System;
using System.Collections.Generic;
using Common;
using System.Data;
using System.Text;

namespace DataAccess
{
    public class FS0306_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        public DataTable searchApi(string vcPart_Id, string vcCarType, string vcState, string strUnitCode)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.Append(" SELECT a.*,b.vcName,'0' as vcModFlag,'0' as vcAddFlag FROM ( \r\n");
                sbr.Append(" SELECT iAutoId,vcPart_id,vcChange,vcCarTypeDesign,dJiuBegin,vcNum1,vcNum2,vcNum3,vcNum4,vcNum5,vcNum6,vcNum7,vcNum8,vcNum9,vcNum10 FROM TJiuTenYear  \r\n");
                sbr.Append(" WHERE 1=1 \r\n");
                if (!string.IsNullOrWhiteSpace(vcPart_Id))
                {
                    sbr.Append(" AND ISNULL(vcPart_id,'') LIKE '" + vcPart_Id.Trim() + "%' \r\n");
                }

                if (!string.IsNullOrWhiteSpace(vcCarType))
                {
                    sbr.Append(" AND ISNULL(vcCarTypeDesign,'') LIKE '" + vcCarType.Trim() + "%' \r\n");
                }
                //已完成
                if (vcState == "0")
                {
                    sbr.Append(" AND (ISNULL(vcNum1,'') <> '' AND ISNULL(vcNum2,'') <> '' AND ISNULL(vcNum3,'') <> '' AND ISNULL(vcNum4,'') <> '' AND ISNULL(vcNum1,'') <> '' AND ISNULL(vcNum5,'') <> '' AND ISNULL(vcNum6,'') <> '' AND ISNULL(vcNum7,'') <> '' AND ISNULL(vcNum8,'') <> '' AND ISNULL(vcNum9,'') <> '' AND ISNULL(vcNum10,'') <> '')");
                }
                //未完成
                else if (vcState == "1")
                {
                    sbr.Append(" AND (ISNULL(vcNum1,'') = '' OR ISNULL(vcNum2,'') = '' OR ISNULL(vcNum3,'') = '' OR ISNULL(vcNum4,'') = '' OR ISNULL(vcNum1,'') = '' OR ISNULL(vcNum5,'') = '' OR ISNULL(vcNum6,'') = '' OR ISNULL(vcNum7,'') = '' OR ISNULL(vcNum8,'') = '' OR ISNULL(vcNum9,'') = '' OR ISNULL(vcNum10,'') = '')");
                }

                sbr.Append(" ) a \r\n");
                sbr.Append(" LEFT JOIN (SELECT vcName,vcValue FROM TCode WHERE vcCodeId = 'C002' ) b ON a.vcChange = b.vcValue \r\n");

                return excute.ExcuteSqlWithSelectToDT(sbr.ToString(), "TK");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("  delete TJiuTenYear where iAutoId in(   \r\n ");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (i != 0)
                        sql.Append(",");
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                    sql.Append(iAutoId);
                }
                sql.Append("  )   \r\n ");
                excute.ExcuteSqlWithStringOper(sql.ToString(), "TK");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    string vcIsLock = listInfoData[i]["vcIsLock"].ToString();//true可编辑,false不可编辑
                    if (bModFlag == true && vcIsLock.Equals("0"))
                    {//修改
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);

                        sbr.Append(" UPDATE TJiuTenYear SET vcNum1= " + ComFunction.getSqlValue(listInfoData[i]["vcNum1"], false) + ", \r\n");
                        sbr.Append(" vcNum2= " + ComFunction.getSqlValue(listInfoData[i]["vcNum2"], false) + ", \r\n");
                        sbr.Append(" vcNum3= " + ComFunction.getSqlValue(listInfoData[i]["vcNum3"], false) + ", \r\n");
                        sbr.Append(" vcNum4= " + ComFunction.getSqlValue(listInfoData[i]["vcNum4"], false) + ", \r\n");
                        sbr.Append(" vcNum5= " + ComFunction.getSqlValue(listInfoData[i]["vcNum5"], false) + ", \r\n");
                        sbr.Append(" vcNum6= " + ComFunction.getSqlValue(listInfoData[i]["vcNum6"], false) + ", \r\n");
                        sbr.Append(" vcNum7= " + ComFunction.getSqlValue(listInfoData[i]["vcNum7"], false) + ", \r\n");
                        sbr.Append(" vcNum8= " + ComFunction.getSqlValue(listInfoData[i]["vcNum8"], false) + ", \r\n");
                        sbr.Append(" vcNum9= " + ComFunction.getSqlValue(listInfoData[i]["vcNum9"], false) + ", \r\n");
                        sbr.Append(" vcNum10=" + ComFunction.getSqlValue(listInfoData[i]["vcNum10"], false) + " \r\n");
                        sbr.Append(" vcIsLock='1' \r\n");
                        sbr.Append(" WHERE iAutoId = " + iAutoId + " \r\n");


                        sbr.Append(" UPDATE TUnit SET vcNum1= " + ComFunction.getSqlValue(listInfoData[i]["vcNum1"], false) + ", \r\n");
                        sbr.Append(" vcNum2= " + ComFunction.getSqlValue(listInfoData[i]["vcNum2"], false) + ", \r\n");
                        sbr.Append(" vcNum3= " + ComFunction.getSqlValue(listInfoData[i]["vcNum3"], false) + ", \r\n");
                        sbr.Append(" vcNum4= " + ComFunction.getSqlValue(listInfoData[i]["vcNum4"], false) + ", \r\n");
                        sbr.Append(" vcNum5= " + ComFunction.getSqlValue(listInfoData[i]["vcNum5"], false) + ", \r\n");
                        sbr.Append(" vcNum6= " + ComFunction.getSqlValue(listInfoData[i]["vcNum6"], false) + ", \r\n");
                        sbr.Append(" vcNum7= " + ComFunction.getSqlValue(listInfoData[i]["vcNum7"], false) + ", \r\n");
                        sbr.Append(" vcNum8= " + ComFunction.getSqlValue(listInfoData[i]["vcNum8"], false) + ", \r\n");
                        sbr.Append(" vcNum9= " + ComFunction.getSqlValue(listInfoData[i]["vcNum9"], false) + ", \r\n");
                        sbr.Append(" vcNum10=" + ComFunction.getSqlValue(listInfoData[i]["vcNum10"], false) + " \r\n");
                        sbr.Append(" WHERE vcPart_id = " + ComFunction.getSqlValue(listInfoData[i]["vcPart_id"], false) + " \r\n");
                    }
                    excute.ExcuteSqlWithStringOper(sbr.ToString(), "TK");
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

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId)
        {
            try
            {

                StringBuilder sbr = new StringBuilder();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sbr.Append("UPDATE TJiuTenYear SET  \r\n");
                    sbr.Append("vcIsLock= '2', ");
                    sbr.Append("vcNum1= " + ComFunction.getSqlValue(dt.Rows[i]["vcNum1"], false) + ", ");
                    sbr.Append("vcNum2= " + ComFunction.getSqlValue(dt.Rows[i]["vcNum2"], false) + ", ");
                    sbr.Append("vcNum3= " + ComFunction.getSqlValue(dt.Rows[i]["vcNum3"], false) + ", ");
                    sbr.Append("vcNum4= " + ComFunction.getSqlValue(dt.Rows[i]["vcNum4"], false) + ", ");
                    sbr.Append("vcNum5= " + ComFunction.getSqlValue(dt.Rows[i]["vcNum5"], false) + ", ");
                    sbr.Append("vcNum6= " + ComFunction.getSqlValue(dt.Rows[i]["vcNum6"], false) + ", ");
                    sbr.Append("vcNum7= " + ComFunction.getSqlValue(dt.Rows[i]["vcNum7"], false) + ", ");
                    sbr.Append("vcNum8= " + ComFunction.getSqlValue(dt.Rows[i]["vcNum8"], false) + ", ");
                    sbr.Append("vcNum9= " + ComFunction.getSqlValue(dt.Rows[i]["vcNum9"], false) + ", ");
                    sbr.Append("vcNum10=" + ComFunction.getSqlValue(dt.Rows[i]["vcNum10"], false) + "  \r\n");
                    sbr.Append("WHERE vcChange = (SELECT vcValue FROM TCode WHERE vcCodeId = 'C002' AND vcName LIKE '%旧型%' AND vcName = " + ComFunction.getSqlValue(dt.Rows[i]["vcChange"], false) + ") ");
                    sbr.Append("AND vcPart_id = " + ComFunction.getSqlValue(dt.Rows[i]["vcPart_id"], false) + " ");
                    sbr.Append("AND vcCarTypeDesign = " + ComFunction.getSqlValue(dt.Rows[i]["vcCarTypeDesign"], false) + " ");
                    sbr.Append("AND dJiuBegin = " + ComFunction.getSqlValue(dt.Rows[i]["dJiuBegin"], true) + " ");
                    sbr.Append("AND vcIsLock = '0' ");
                }
                if (sbr.Length > 0)
                {
                    sbr.Append("UPDATE TUnit SET");
                    sbr.Append("vcNum1  = b.vcNum1 ,");
                    sbr.Append("vcNum2  = b.vcNum2 ,");
                    sbr.Append("vcNum3  = b.vcNum3 ,");
                    sbr.Append("vcNum4  = b.vcNum4 ,");
                    sbr.Append("vcNum5  = b.vcNum5 ,");
                    sbr.Append("vcNum6  = b.vcNum6 ,");
                    sbr.Append("vcNum7  = b.vcNum7 ,");
                    sbr.Append("vcNum8  = b.vcNum8 ,");
                    sbr.Append("vcNum9  = b.vcNum9 ,");
                    sbr.Append("vcNum10 = b.vcNum10");
                    sbr.Append("FROM  ");
                    sbr.Append("TUnit a");
                    sbr.Append("LEFT JOIN ");
                    sbr.Append("(SELECT vcPart_id,vcNum1,vcNum2,vcNum3,vcNum4,vcNum5,vcNum6,vcNum7,vcNum8,vcNum9,vcNum10,vcIsLock FROM TJiuTenYear) b ON a.vcPart_id = b.vcPart_id");
                    sbr.Append("WHERE b.vcIsLock = '2'");
                    sbr.Append("UPDATE TJiuTenYear SET vcIsLock = '1' WHERE vcIsLock = '2' \r\n");

                    excute.ExcuteSqlWithStringOper(sbr.ToString(), "TK");
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