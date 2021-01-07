using System;
using System.Collections.Generic;
using Common;
using System.Data;
using System.Text;
using Org.BouncyCastle.Crypto.Tls;

namespace DataAccess
{
    public class FS0307_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        //年限对象品番抽取
        public void extractPart(string strUserId, List<string> vcOriginCompany)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                string OriginCompany = "''";
                if (vcOriginCompany.Count > 0)
                {
                    OriginCompany = "";
                    foreach (string str in vcOriginCompany)
                    {
                        if (!string.IsNullOrWhiteSpace(OriginCompany))
                        {
                            OriginCompany += ",";
                        }

                        OriginCompany += "'" + str + "'";
                    }
                }

                sbr.Append("  DECLARE @Year VARCHAR(4);  \r\n");
                sbr.Append("  DECLARE @7Year VARCHAR(4);  \r\n");
                sbr.Append("  DECLARE @9Year VARCHAR(4);  \r\n");
                sbr.Append("  DECLARE @10Year VARCHAR(4);  \r\n");
                sbr.Append("  DECLARE @7B VARCHAR(10);  \r\n");
                sbr.Append("  DECLARE @7E VARCHAR(10);  \r\n");
                sbr.Append("  DECLARE @9B VARCHAR(10);  \r\n");
                sbr.Append("  DECLARE @9E VARCHAR(10);  \r\n");
                sbr.Append("  DECLARE @10E VARCHAR(10);  \r\n");
                sbr.Append("  SET @Year=SUBSTRING(CONVERT(VARCHAR, GETDATE(), 120), 1, 4);  \r\n");
                sbr.Append("  SET @7Year=@Year-7;  \r\n");
                sbr.Append("  SET @9Year=@Year-9;  \r\n");
                sbr.Append("  SET @10Year=@Year-10;  \r\n");
                sbr.Append("  SET @7B=@9Year+'/09/01';  \r\n");
                sbr.Append("  SET @7E=@7Year+'/08/31';  \r\n");
                sbr.Append("  SET @9B=@10Year+'/09/01';  \r\n");
                sbr.Append("  SET @9E=@9Year+'/08/31';  \r\n");
                sbr.Append("  SET @10E=@10Year+'/08/31';  \r\n");

                sbr.Append("  DELETE TOldYearManager WHERE vcYear = @Year AND vcOriginCompany in (" + OriginCompany + ") \r\n");
                sbr.Append("  IF EXISTS (SELECT *  \r\n");
                sbr.Append("             FROM tempdb.dbo.sysobjects  \r\n");
                sbr.Append("             WHERE id=OBJECT_ID(N'tempdb..#temp')AND type='U')  \r\n");
                sbr.Append("      DROP TABLE #temp;  \r\n");
                sbr.Append("  SELECT vcSupplier_id, vcPart_id, vcPartNameEn, vcInOutflag, vcNXQF, vcCarTypeDev, dJiuBegin,vcSYTCode,vcReceiver,vcOriginCompany  \r\n");
                sbr.Append("  INTO #temp  \r\n");
                sbr.Append("  FROM TUnit  \r\n");
                sbr.Append("  WHERE vcChange IN (SELECT vcValue FROM TCode WHERE vcName like'%旧型%' AND vcCodeId='C002') AND vcOriginCompany in (" + OriginCompany + ");  \r\n");

                sbr.Append("  INSERT INTO TOldYearManager(vcYear, vcFinish,dFinishYMD, vcSupplier_id, vcPart_id, vcPartNameEn, vcInOutflag, vcCarTypeDev, dJiuBegin, vcRemark, vcOld10, vcOld9, vcOld7, vcFlag,vcNXQF, vcOperatorID, dOperatorTime,vcSYTCode,vcReceiver,vcOriginCompany)  \r\n");
                sbr.Append("  SELECT @Year AS vcYear, '0' AS vcFinish,GETDATE() AS dFinishYMD, vcSupplier_id, vcPart_id, vcPartNameEn, vcInOutflag, vcCarTypeDev, dJiuBegin, CASE vcNXQF WHEN '继续生产' THEN '往年持续生产' ELSE '' END AS vcRemark, vcOld10, vcOld9, vcOld7, '0' AS vcFlag, vcNXQF,'" + strUserId + "' AS vcOperatorID, GETDATE() AS dOperatorTime,vcSYTCode,vcReceiver,vcOriginCompany  \r\n");
                sbr.Append("  FROM(SELECT vcSupplier_id, vcPart_id, vcPartNameEn, vcInOutflag, vcNXQF, vcCarTypeDev, dJiuBegin, '1' AS vcOld7, '0' AS vcOld9, '0' AS vcOld10,vcSYTCode,vcReceiver,vcOriginCompany  \r\n");
                sbr.Append("       FROM #temp  \r\n");
                sbr.Append("       WHERE CONVERT(VARCHAR(12), dJiuBegin, 111)>=@7B AND CONVERT(VARCHAR(12), dJiuBegin, 111)<=@7E  \r\n");
                sbr.Append("       UNION ALL  \r\n");
                sbr.Append("       SELECT vcSupplier_id, vcPart_id, vcPartNameEn, vcInOutflag, vcNXQF, vcCarTypeDev, dJiuBegin, '0' AS vcOld7, '1' AS vcOld9, '0' AS vcOld10,vcSYTCode,vcReceiver,vcOriginCompany  \r\n");
                sbr.Append("       FROM #temp  \r\n");
                sbr.Append("       WHERE CONVERT(VARCHAR(12), dJiuBegin, 111)>=@9B AND CONVERT(VARCHAR(12), dJiuBegin, 111)<=@9E  \r\n");
                sbr.Append("       UNION ALL  \r\n");
                sbr.Append("       SELECT vcSupplier_id, vcPart_id, vcPartNameEn, vcInOutflag, vcNXQF, vcCarTypeDev, dJiuBegin, '0' AS vcOld7, '0' AS vcOld9, '1' AS vcOld10,vcSYTCode,vcReceiver,vcOriginCompany  \r\n");
                sbr.Append("       FROM #temp \r\n");
                sbr.Append("       WHERE CONVERT(VARCHAR(12), dJiuBegin, 111)<=@10E  AND vcNXQF = '继续生产') a;  \r\n");
                sbr.Append("  IF EXISTS (SELECT *  \r\n");
                sbr.Append("             FROM tempdb.dbo.sysobjects  \r\n");
                sbr.Append("             WHERE id=OBJECT_ID(N'tempdb..#temp')AND type='U')  \r\n");
                sbr.Append("      DROP TABLE #temp;  \r\n");

                excute.ExcuteSqlWithStringOper(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //FTMS层别
        public void FTMSCB(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(" UPDATE TOldYearManager SET vcFinish = '1',dFinishYMD = GETDATE(),vcOperatorID = '" + strUserId + "',dOperatorTime = GETDATE() WHERE iAutoId IN (  \r\n ");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (i != 0)
                        sql.Append(",");
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                    sql.Append(iAutoId);
                }
                sql.Append("  )   \r\n ");
                excute.ExcuteSqlWithStringOper(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //检索
        public DataTable searchApi(string strYear, string FinishFlag, string SYT, string Receiver, List<string> origin)
        {
            try
            {
                string OriginCompany = "";
                if (origin.Count > 0)
                {
                    OriginCompany = "";
                    foreach (string str in origin)
                    {
                        if (!string.IsNullOrWhiteSpace(OriginCompany))
                        {
                            OriginCompany += ",";
                        }

                        OriginCompany += "'" + str + "'";
                    }
                }
                StringBuilder sbr = new StringBuilder();

                sbr.Append(" SELECT a.vcYear,b.vcName AS vcFinish,a.dFinishYMD,e.vcName AS vcSYTCode,f.vcName AS vcReceiver,g.vcName AS vcOriginCompany,  a.vcSupplier_id, a.vcPart_id, a.vcPartNameEn,d.vcName AS vcInOutflag, a.vcCarTypeDev, \r\n");
                sbr.Append(" a.dJiuBegin, a.vcRemark, a.vcOld10, a.vcOld9, a.vcOld7,c.vcName AS vcPM, a.vcNum1, a.vcNum2, a.vcNum3,(isnull(a.vcNum1,0)+isnull(a.vcNum2,0)+isnull(a.vcNum3,0))/3 as vcNumAvg, a.vcNXQF, \r\n");
                sbr.Append(" a.dTimeFrom, a.vcDY, a.vcNum11, a.vcNum12, a.vcNum13, a.vcNum14, a.vcNum15, a.vcNum16, a.vcNum17, a.vcNum18, a.vcNum19, a.vcNum20, a.vcNum21,'0' as vcModFlag \r\n");
                sbr.Append(" FROM TOldYearManager a \r\n");
                sbr.Append(" LEFT JOIN (SELECT vcName,vcValue FROM TCode WHERE vcCodeId = 'C024') b ON a.vcFinish = b.vcValue \r\n");
                sbr.Append(" LEFT JOIN (SELECT vcName,vcValue FROM TCode WHERE vcCodeId = 'C099') c ON SUBSTRING(a.vcPart_id,1,5) = c.vcValue \r\n");
                sbr.Append(" LEFT JOIN (SELECT vcName,vcValue FROM TCode WHERE vcCodeId = 'C003') d ON a.vcInOutflag = d.vcValue \r\n");
                sbr.Append(" LEFT JOIN (SELECT vcName,vcValue FROM TCode WHERE vcCodeId = 'C016') e ON a.vcSYTCode = e.vcValue \r\n");
                sbr.Append(" LEFT JOIN (SELECT vcName,vcValue FROM TCode WHERE vcCodeId = 'C005') f ON a.vcReceiver = f.vcValue \r\n");
                sbr.Append(" LEFT JOIN (SELECT vcName,vcValue FROM TCode WHERE vcCodeId = 'C006') g ON a.vcOriginCompany = g.vcValue \r\n");
                sbr.Append(" WHERE 1=1  \r\n");
                if (!string.IsNullOrWhiteSpace(strYear))
                {
                    sbr.Append(" AND a.vcYear = '" + strYear + "' \r\n");
                }
                if (!string.IsNullOrWhiteSpace(FinishFlag))
                {
                    sbr.Append(" AND a.vcFinish = '" + FinishFlag + "' \r\n");
                }
                if (!string.IsNullOrWhiteSpace(SYT))
                {
                    sbr.Append(" AND a.vcSYTCode = '" + SYT + "' \r\n");
                }
                if (!string.IsNullOrWhiteSpace(Receiver))
                {
                    sbr.Append(" AND a.vcReceiver = '" + Receiver + "' \r\n");
                }
                if (!string.IsNullOrWhiteSpace(OriginCompany))
                {
                    sbr.Append(" AND a.vcOriginCompany in (" + OriginCompany + ") \r\n");
                }

                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //删除
        public void DelApi(List<Dictionary<string, Object>> listInfoData)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("  delete TOldYearManager where iAutoId in(   \r\n ");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (i != 0)
                        sql.Append(",");
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                    sql.Append(iAutoId);
                }
                sql.Append("  )   \r\n ");
                excute.ExcuteSqlWithStringOper(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //保存
        #region 导入后保存
        public void importSave(DataTable dt, string strUserId)
        {
            try
            {

                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string vcFinish = dt.Rows[i]["vcFinish"].ToString();

                    sql.Append(" UPDATE TOldYearManager SET \r\n");
                    if (vcFinish.Equals("对象外"))
                    {
                        sql.Append(" vcFinish = '" + getValue("C024", vcFinish) + "', \r\n");
                        sql.Append(" dFinishYMD = GETDATE(), \r\n");
                    }
                    sql.Append(" vcNum1 = " + ComFunction.getSqlValue(dt.Rows[i]["vcNum1"], false) + ", \r\n");
                    sql.Append(" vcNum2 = " + ComFunction.getSqlValue(dt.Rows[i]["vcNum2"], false) + ", \r\n");
                    sql.Append(" vcNum3 = " + ComFunction.getSqlValue(dt.Rows[i]["vcNum3"], false) + ", \r\n");
                    sql.Append(" vcNXQF = " + ComFunction.getSqlValue(dt.Rows[i]["vcNXQF"], false) + ", \r\n");
                    sql.Append(" dTimeFrom = " + ComFunction.getSqlValue(dt.Rows[i]["dTimeFrom"], true) + ", \r\n");
                    sql.Append(" vcDY = " + ComFunction.getSqlValue(dt.Rows[i]["vcDY"], false) + ", \r\n");
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
                    sql.Append(" vcOperatorID='" + strUserId + "', \r\n");
                    sql.Append(" dOperatorTime = GETDATE() \r\n");
                    sql.Append(" WHERE \r\n");
                    sql.Append(" vcYear = " + ComFunction.getSqlValue(dt.Rows[i]["vcYear"], false) + " \r\n");
                    sql.Append(" AND vcPart_id = " + ComFunction.getSqlValue(dt.Rows[i]["vcPart_id"], false) + " \r\n");
                    sql.Append(" AND vcCarTypeDev = " + ComFunction.getSqlValue(dt.Rows[i]["vcCarTypeDev"], false) + " \r\n");
                    sql.Append(" AND vcSupplier_id =" + ComFunction.getSqlValue(dt.Rows[i]["vcSupplier_id"], false) + " \r\n");
                    sql.Append(" AND vcSYTCode = '" + getValue("C016", dt.Rows[i]["vcSYTCode"].ToString()) + "' \r\n");
                    sql.Append(" AND vcOriginCompany = '" + getValue("C006", dt.Rows[i]["vcOriginCompany"].ToString()) + "' \r\n");
                    sql.Append(" AND vcReceiver = '" + getValue("C005", dt.Rows[i]["vcReceiver"].ToString()) + "' \r\n");
                    sql.Append(" AND vcInOutflag = '" + getValue("C003", dt.Rows[i]["vcInOutflag"].ToString()) + "' \r\n");
                    if (!vcFinish.Equals("对象外"))
                    {
                        sql.Append(" AND vcFinish = '" + getValue("C024", vcFinish) + "' \r\n");
                    }


                }
                if (sql.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(sql.ToString());
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
                    int iAutoId = Convert.ToInt32(list[i]["iAutoId"]);
                    sql.Append(" UPDATE TOldYearManager SET \r\n");
                    if (vcFinish.Equals("对象外"))
                    {
                        sql.Append(" vcFinish = '" + getValue("C024", vcFinish) + "', \r\n");
                        sql.Append(" dFinishYMD = GETDATE(), \r\n");
                    }
                    sql.Append(" vcNum1 = " + ComFunction.getSqlValue(list[i]["vcNum1"], false) + ", \r\n");
                    sql.Append(" vcNum2 = " + ComFunction.getSqlValue(list[i]["vcNum2"], false) + ", \r\n");
                    sql.Append(" vcNum3 = " + ComFunction.getSqlValue(list[i]["vcNum3"], false) + ", \r\n");
                    sql.Append(" vcNXQF = " + ComFunction.getSqlValue(list[i]["vcNXQF"], false) + ", \r\n");
                    sql.Append(" dTimeFrom = " + ComFunction.getSqlValue(list[i]["dTimeFrom"], true) + ", \r\n");
                    sql.Append(" vcDY = " + ComFunction.getSqlValue(list[i]["vcDY"], false) + ", \r\n");
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
                    sql.Append(" vcOperatorID='" + strUserId + "', \r\n");
                    sql.Append(" dOperatorTime = GETDATE() \r\n");
                    sql.Append(" WHERE \r\n");
                    sql.Append(" iAutoId = " + iAutoId + " \r\n");


                }
                if (sql.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(sql.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        //展开账票
        public void ZKZP(int id, string strUserId)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.Append(" UPDATE TOldYearManager SET vcFinish = '3',dFinishYMD = GETDATE(),vcOperatorID = '" + strUserId + "',dOperatorTime = GETDATE() \r\n");
                sbr.Append(" WHERE iAuto_id = " + id + " \r\n");

                excute.ExcuteSqlWithStringOper(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //获取value值
        public string getValue(string strCodeId, string vcName)
        {
            try
            {
                DataTable dt = new DataTable();
                StringBuilder strSql = new StringBuilder();
                strSql.Append("   select vcName,vcValue from TCode where vcCodeId='" + strCodeId + "'     \n");
                dt = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
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

        //织入原单位
        public void InsertUnitApi(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                    string vcYear = listInfoData[i]["vcYear"].ToString();
                    string vcNXQF = listInfoData[i]["vcNXQF"].ToString();
                    string vcNum11 = listInfoData[i]["vcNum11"].ToString();
                    string vcNum12 = listInfoData[i]["vcNum12"].ToString();
                    string vcNum13 = listInfoData[i]["vcNum13"].ToString();
                    string vcNum14 = listInfoData[i]["vcNum14"].ToString();
                    string vcNum15 = listInfoData[i]["vcNum15"].ToString();
                    string vcReceiver = listInfoData[i]["vcReceiver"].ToString();
                    string vcSYTCode = listInfoData[i]["vcSYTCode"].ToString();
                    string vcOriginCompany = listInfoData[i]["vcOriginCompany"].ToString();
                    string vcSupplier_id = listInfoData[i]["vcSupplier_id"].ToString();
                    string vcPart_id = listInfoData[i]["vcPart_id"].ToString();
                    string vcCarTypeDev = listInfoData[i]["vcCarTypeDev"].ToString();

                    sbr.AppendLine("UPDATE TUnit SET ");
                    sbr.AppendLine("vcNXQF = '" + vcNXQF + "', ");
                    sbr.AppendLine("vcNum11 = '" + vcNum11 + "', ");
                    sbr.AppendLine("vcNum12 = '" + vcNum12 + "', ");
                    sbr.AppendLine("vcNum13 = '" + vcNum13 + "', ");
                    sbr.AppendLine("vcNum14 = '" + vcNum14 + "', ");
                    sbr.AppendLine("vcNum15 = '" + vcNum15 + "', ");
                    sbr.AppendLine("vcChange = '8', ");
                    if (vcNXQF.Equals("一括生产") || vcNXQF.Equals("生产打切"))
                    {
                        sbr.AppendLine("vcDiff = '4', ");
                    }
                    sbr.AppendLine("vcOperator = '" + strUserId + "', ");
                    sbr.AppendLine("dOperatorTime = GETDATE()");
                    sbr.AppendLine("WHERE ");
                    sbr.AppendLine("vcPart_id = '" + vcPart_id + "' ");
                    sbr.AppendLine("AND vcReceiver = '" + vcReceiver + "' ");
                    sbr.AppendLine("AND vcSYTCode = '" + vcSYTCode + "' ");
                    sbr.AppendLine("AND vcOriginCompany = '" + vcOriginCompany + "' ");
                    sbr.AppendLine("AND vcSupplier_id = '" + vcSupplier_id + "' ");
                    sbr.AppendLine("AND vcCarTypeDev = '" + vcCarTypeDev + "' ");
                    sbr.AppendLine("AND dTimeTo = " + ComFunction.getSqlValue(listInfoData[i]["dTimeFrom"].ToString(), true) + " ");

                    sbr.AppendLine(" UPDATE TOldYearManager SET vcFinish = '4' WHERE iAuto_id = " + iAutoId + " ");

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