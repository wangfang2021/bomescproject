using System;
using System.Collections.Generic;
using Common;
using System.Data;
using System.Text;

namespace DataAccess
{
    public class FS0308_DataAccess
    {
        private MultiExcute excute = new MultiExcute();


        public DataTable searchApi(string strYear, string Receiver)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();

                sbr.Append(" SELECT a.iAuto_id,'0' AS selected,'0' as vcModFlag,'0' as vcAddFlag,a.vcYear,b.vcName AS vcFinish,a.dFinishYMD,e.vcName AS vcSYTCode,f.vcName AS vcReceiver,g.vcName AS vcOriginCompany,   \r\n");
                sbr.Append(" a.vcSupplier_id, a.vcPart_id, a.vcPartNameEn,d.vcName AS vcInOutflag, a.vcCarTypeDev,a.dJiuBegin, a.vcRemark, \r\n");
                sbr.Append("  case a.vcOld10 when '0' then '' when '1' then '●' else '' end as vcOld10, \r\n");
                sbr.Append("  case a.vcOld9 when '0' then '' when '1' then '●' else '' end as vcOld9, \r\n");
                sbr.Append("  case a.vcOld7 when '0' then '' when '1' then '●' else '' end as vcOld7, \r\n");
                sbr.Append("  c.vcName AS vcPM, a.vcNum1, a.vcNum2, a.vcNum3, CAST((CAST((CASE isnull(A.vcNum1,'') WHEN '' THEN '0'  ELSE A.vcNum1 END ) as decimal(18,2))+CAST((CASE isnull(A.vcNum2,'') WHEN '' THEN '0'  ELSE A.vcNum2 END ) as decimal(18,2))+CAST((CASE isnull(A.vcNum3,'') WHEN '' THEN '0'  ELSE A.vcNum3 END ) as decimal(18,2)))/3 AS decimal(18,2)) AS vcNumAvg,a.vcNXQF,  \r\n");
                sbr.Append(" a.dSSDate, a.vcDY, a.vcNum11, a.vcNum12, a.vcNum13, a.vcNum14, a.vcNum15, a.vcNum16, a.vcNum17, a.vcNum18, a.vcNum19, a.vcNum20, a.vcNum21 \r\n");
                sbr.Append(" FROM TOldYearManager a \r\n");
                sbr.Append(" LEFT JOIN (SELECT vcName,vcValue FROM TCode WHERE vcCodeId = 'C024') b ON a.vcFinish = b.vcValue \r\n");
                sbr.Append(" LEFT JOIN (SELECT vcName,vcValue FROM TCode WHERE vcCodeId = 'C099') c ON SUBSTRING(a.vcPart_id,1,5) = c.vcValue \r\n");
                sbr.Append(" LEFT JOIN (SELECT vcName,vcValue FROM TCode WHERE vcCodeId = 'C003') d ON a.vcInOutflag = d.vcValue \r\n");
                sbr.Append(" LEFT JOIN (SELECT vcName,vcValue FROM TCode WHERE vcCodeId = 'C016') e ON a.vcSYTCode = e.vcValue \r\n");
                sbr.Append(" LEFT JOIN (SELECT vcName,vcValue FROM TCode WHERE vcCodeId = 'C005') f ON a.vcReceiver = f.vcValue \r\n");
                sbr.Append(" LEFT JOIN (SELECT vcName,vcValue FROM TCode WHERE vcCodeId = 'C006') g ON a.vcOriginCompany = g.vcValue \r\n");
                sbr.Append(" WHERE 1=1  \r\n");
                sbr.Append(" AND a.vcFinish in ('1','2','3','4') \r\n");
                sbr.Append(" AND a.vcReceiver = " + ComFunction.getSqlValue(getValue("C005", Receiver), false) + " \r\n");

                if (!string.IsNullOrWhiteSpace(strYear))
                {
                    sbr.Append(" AND a.vcYear = '" + strYear + "' \r\n");
                }


                return excute.ExcuteSqlWithSelectToDT(sbr.ToString(), "TK");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #region 保存
        public void importSave(DataTable dt, string receiver, string strUserId)
        {
            try
            {

                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < dt.Rows.Count; i++)
                {


                    sql.Append(" UPDATE TOldYearManager SET \r\n");
                    sql.Append(" vcNum1 = " + ComFunction.getSqlValue(dt.Rows[i]["vcNum1"], false) + ", \r\n");
                    sql.Append(" vcNum2 = " + ComFunction.getSqlValue(dt.Rows[i]["vcNum2"], false) + ", \r\n");
                    sql.Append(" vcNum3 = " + ComFunction.getSqlValue(dt.Rows[i]["vcNum3"], false) + ", \r\n");
                    sql.Append(" vcNXQF = " + ComFunction.getSqlValue(dt.Rows[i]["vcNXQF"], false) + ", \r\n");
                    sql.Append(" dSSDate = " + ComFunction.getSqlValue(dt.Rows[i]["dSSDate"], true) + ", \r\n");
                    sql.Append(" vcNum11=" + ComFunction.getSqlValue(dt.Rows[i]["vcNum11"], false) + ", \r\n");
                    sql.Append(" vcNum12=" + ComFunction.getSqlValue(dt.Rows[i]["vcNum12"], false) + ", \r\n");
                    sql.Append(" vcNum13=" + ComFunction.getSqlValue(dt.Rows[i]["vcNum13"], false) + ", \r\n");
                    sql.Append(" vcNum14=" + ComFunction.getSqlValue(dt.Rows[i]["vcNum14"], false) + ", \r\n");
                    sql.Append(" vcNum15=" + ComFunction.getSqlValue(dt.Rows[i]["vcNum15"], false) + ", \r\n");
                    sql.Append(" vcNum16=" + ComFunction.getSqlValue(dt.Rows[i]["vcNum16"], false) + ", \r\n");
                    sql.Append(" vcNum17=" + ComFunction.getSqlValue(dt.Rows[i]["vcNum17"], false) + ", \r\n");
                    sql.Append(" vcNum18=" + ComFunction.getSqlValue(dt.Rows[i]["vcNum18"], false) + ", \r\n");
                    sql.Append(" vcNum19=" + ComFunction.getSqlValue(dt.Rows[i]["vcNum19"], false) + ", \r\n");
                    sql.Append(" vcNum20=" + ComFunction.getSqlValue(dt.Rows[i]["vcNum20"], false) + ", \r\n");
                    sql.Append(" vcNum21=" + ComFunction.getSqlValue(dt.Rows[i]["vcNum21"], false) + ", \r\n");
                    sql.Append(" vcFinish = '2',");
                    sql.Append(" vcOperatorID='" + strUserId + "', \r\n");
                    sql.Append(" dOperatorTime = GETDATE() \r\n");
                    sql.Append(" WHERE \r\n");
                    sql.Append(" vcYear = " + ComFunction.getSqlValue(dt.Rows[i]["vcYear"], false) + " \r\n");
                    sql.Append(" AND vcPart_id = " + ComFunction.getSqlValue(dt.Rows[i]["vcPart_id"], false) + " \r\n");
                    sql.Append(" AND vcReceiver = '" + getValue("C005", receiver) + "' \r\n");
                    sql.Append(" AND vcFinish = '1' \r\n");
                }
                if (sql.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(sql.ToString(), "TK");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SaveApi(List<Dictionary<string, Object>> list, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < list.Count; i++)
                {
                    string vcFinish = list[i]["vcFinish"].ToString();
                    int iAutoId = Convert.ToInt32(list[i]["iAuto_id"]);
                    sql.Append(" UPDATE TOldYearManager SET \r\n");
                    sql.Append(" vcNum1 = " + ComFunction.getSqlValue(list[i]["vcNum1"], false) + ", \r\n");
                    sql.Append(" vcNum2 = " + ComFunction.getSqlValue(list[i]["vcNum2"], false) + ", \r\n");
                    sql.Append(" vcNum3 = " + ComFunction.getSqlValue(list[i]["vcNum3"], false) + ", \r\n");
                    sql.Append(" vcNXQF = " + ComFunction.getSqlValue(list[i]["vcNXQF"], false) + ", \r\n");
                    sql.Append(" dSSDate = " + ComFunction.getSqlValue(list[i]["dSSDate"], true) + ", \r\n");
                    sql.Append(" vcNum11=" + ComFunction.getSqlValue(list[i]["vcNum11"], false) + ", \r\n");
                    sql.Append(" vcNum12=" + ComFunction.getSqlValue(list[i]["vcNum12"], false) + ", \r\n");
                    sql.Append(" vcNum13=" + ComFunction.getSqlValue(list[i]["vcNum13"], false) + ", \r\n");
                    sql.Append(" vcNum14=" + ComFunction.getSqlValue(list[i]["vcNum14"], false) + ", \r\n");
                    sql.Append(" vcNum15=" + ComFunction.getSqlValue(list[i]["vcNum15"], false) + ", \r\n");
                    sql.Append(" vcNum16=" + ComFunction.getSqlValue(list[i]["vcNum16"], false) + ", \r\n");
                    sql.Append(" vcNum17=" + ComFunction.getSqlValue(list[i]["vcNum17"], false) + ", \r\n");
                    sql.Append(" vcNum18=" + ComFunction.getSqlValue(list[i]["vcNum18"], false) + ", \r\n");
                    sql.Append(" vcNum19=" + ComFunction.getSqlValue(list[i]["vcNum19"], false) + ", \r\n");
                    sql.Append(" vcNum20=" + ComFunction.getSqlValue(list[i]["vcNum20"], false) + ", \r\n");
                    sql.Append(" vcNum21=" + ComFunction.getSqlValue(list[i]["vcNum21"], false) + ", \r\n");
                    sql.Append(" vcFinish = '2',");
                    sql.Append(" vcOperatorID='" + strUserId + "', \r\n");
                    sql.Append(" dOperatorTime = GETDATE() \r\n");
                    sql.Append(" WHERE \r\n");
                    sql.Append(" iAuto_Id = " + iAutoId + " \r\n");
                    sql.Append(" AND vcFinish = '1' \r\n");


                }
                if (sql.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(sql.ToString(), "TK");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        //获取value值
        public string getValue(string strCodeId, string vcName)
        {
            try
            {
                DataTable dt = new DataTable();
                StringBuilder strSql = new StringBuilder();
                strSql.Append("   select vcName,vcValue from TCode where vcCodeId='" + strCodeId + "'     \n");
                dt = excute.ExcuteSqlWithSelectToDT(strSql.ToString(), "TK");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["vcName"].ToString().Equals(vcName))
                    {
                        return dt.Rows[i]["vcValue"].ToString();
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}