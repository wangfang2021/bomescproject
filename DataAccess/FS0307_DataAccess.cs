using System;
using System.Collections.Generic;
using Common;
using System.Data;
using System.Text;

namespace DataAccess
{
    public class FS0307_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        //获取抽取状态
        public DataTable getExtractState()
        {
            StringBuilder sbr = new StringBuilder();
            sbr.AppendLine("SELECT a.vcName,a.vcValue,CASE WHEN a.isFinish>0 THEN '已抽取' WHEN a.isFinish = 0 THEN '未抽取' END AS isFinish  FROM ");
            sbr.AppendLine("(");
            sbr.AppendLine("SELECT a.*,ISNULL(b.vcOriginCompany,'0') AS isFinish FROM ");
            sbr.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId = 'C006') a");
            sbr.AppendLine("LEFT JOIN ");
            sbr.AppendLine("(SELECT distinct vcOriginCompany FROM TOldYearManager WHERE vcYear = SUBSTRING(CONVERT(VARCHAR, GETDATE(), 120), 1, 4)) b ON a.vcValue = b.vcOriginCompany");
            sbr.AppendLine(") a");

            return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
        }

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
                sbr.Append("  WHERE vcChange IN (SELECT vcValue FROM TCode WHERE vcName like'%旧型%' AND vcCodeId='C002') AND vcOriginCompany in (" + OriginCompany + ") AND  dTimeFrom <= GETDATE() AND dTimeTo >= GETDATE() ;  \r\n");

                sbr.Append("  INSERT INTO TOldYearManager(vcYear, vcFinish,dFinishYMD, vcSupplier_id, vcPart_id, vcPartNameEn, vcInOutflag, vcCarTypeDev, dJiuBegin, vcRemark, vcOld10, vcOld9, vcOld7,vcNXQF, vcOperatorID, dOperatorTime,vcSYTCode,vcReceiver,vcOriginCompany)  \r\n");
                sbr.Append("  SELECT @Year AS vcYear, '0' AS vcFinish,GETDATE() AS dFinishYMD, vcSupplier_id, vcPart_id, vcPartNameEn, vcInOutflag, vcCarTypeDev, dJiuBegin, CASE vcNXQF WHEN '继续生产' THEN '往年持续生产' ELSE '' END AS vcRemark, vcOld10, vcOld9, vcOld7, vcNXQF,'" + strUserId + "' AS vcOperatorID, GETDATE() AS dOperatorTime,vcSYTCode,vcReceiver,vcOriginCompany  \r\n");
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

                excute.ExcuteSqlWithStringOper(sbr.ToString(), "TK");
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
                sql.Append(" UPDATE TOldYearManager SET vcFinish = '1',dFinishYMD = GETDATE(),vcOperatorID = '" + strUserId + "',dOperatorTime = GETDATE() WHERE iAuto_Id IN (  \r\n ");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (i != 0)
                        sql.Append(",");
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAuto_id"]);
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
        //检索
        public DataTable searchApi(string strYear, string FinishFlag)
        {
            try
            {
                //string OriginCompany = "";
                //if (origin.Count > 0)
                //{
                //    OriginCompany = "";
                //    foreach (string str in origin)
                //    {
                //        if (!string.IsNullOrWhiteSpace(OriginCompany))
                //        {
                //            OriginCompany += ",";
                //        }

                //        OriginCompany += "'" + str + "'";
                //    }
                //}
                StringBuilder sbr = new StringBuilder();

                sbr.Append(" SELECT a.iAuto_id,'0' AS selected,'0' as vcModFlag,'0' as vcAddFlag,a.vcYear,b.vcName AS vcFinish,a.dFinishYMD,e.vcName AS vcSYTCode,f.vcName AS vcReceiver,g.vcName AS vcOriginCompany,   \r\n");
                sbr.Append(" a.vcSupplier_id, a.vcPart_id, a.vcPartNameEn,d.vcName AS vcInOutflag, a.vcCarTypeDev,a.dJiuBegin, a.vcRemark, \r\n");
                sbr.Append("  case a.vcOld10 when '0' then '' when '1' then '●' else '' end as vcOld10, \r\n");
                sbr.Append("  case a.vcOld9 when '0' then '' when '1' then '●' else '' end as vcOld9, \r\n");
                sbr.Append("  case a.vcOld7 when '0' then '' when '1' then '●' else '' end as vcOld7, \r\n");
                sbr.Append("  c.vcName AS vcPM, a.vcNum1, a.vcNum2, a.vcNum3,  CAST((CAST((CASE isnull(A.vcNum1,'') WHEN '' THEN '0' ELSE A.vcNum1 END ) as decimal(18,2))+CAST((CASE isnull(A.vcNum2,'') WHEN '' THEN '0'  ELSE A.vcNum2 END ) as decimal(18,2))+CAST((CASE isnull(A.vcNum3,'') WHEN '' THEN '0'  ELSE A.vcNum3 END ) as decimal(18,2)))/3 AS decimal(18,2)) AS vcNumAvg, a.vcNXQF,  \r\n");
                sbr.Append(" a.dSSDate, a.vcDY, a.vcNum11, a.vcNum12, a.vcNum13, a.vcNum14, a.vcNum15, a.vcNum16, a.vcNum17, a.vcNum18, a.vcNum19, a.vcNum20, a.vcNum21 \r\n");
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
                //if (!string.IsNullOrWhiteSpace(SYT))
                //{
                //    sbr.Append(" AND a.vcSYTCode = '" + SYT + "' \r\n");
                //}
                //if (!string.IsNullOrWhiteSpace(Receiver))
                //{
                //    sbr.Append(" AND a.vcReceiver = '" + Receiver + "' \r\n");
                //}
                //if (!string.IsNullOrWhiteSpace(OriginCompany))
                //{
                //    sbr.Append(" AND a.vcOriginCompany in (" + OriginCompany + ") \r\n");
                //}

                return excute.ExcuteSqlWithSelectToDT(sbr.ToString(), "TK");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable getFile(List<int> list)
        {
            try
            {
                string idList = "";
                foreach (int i in list)
                {
                    if (!string.IsNullOrWhiteSpace(idList))
                    {
                        idList += ",";
                    }

                    idList += i.ToString();
                }

                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine("SELECT a.vcSupplier_id, a.vcPart_id, a.vcPartNameEn, a.vcCarTypeDev, a.dJiuBegin,");
                sbr.AppendLine(" CASE b.vcName WHEN '机能' THEN '1' WHEN '内外装' THEN '2' ELSE '' END AS vcPM, ");
                sbr.AppendLine("a.vcNum1, a.vcNum2, a.vcNum3,  CAST((CAST((CASE isnull(A.vcNum1,'') WHEN '' THEN '0' ELSE A.vcNum1 END ) as decimal(18,2))+CAST((CASE isnull(A.vcNum2,'') WHEN '' THEN '0'  ELSE A.vcNum2 END ) as decimal(18,2))+CAST((CASE isnull(A.vcNum3,'') WHEN '' THEN '0'  ELSE A.vcNum3 END ) as decimal(18,2)))/3 AS decimal(18,2)) AS vcNumAvg, ");
                sbr.AppendLine(" a.vcNXQF, a.dSSDate, a.vcNum11, a.vcNum12, a.vcNum13, a.vcNum14, a.vcNum15, a.vcNum16, a.vcNum17, a.vcNum18, a.vcNum19, a.vcNum20, a.vcNum21 ");
                sbr.AppendLine("FROM TOldYearManager a");
                sbr.AppendLine("     LEFT JOIN(SELECT vcName, vcValue FROM TCode WHERE vcCodeId='C099') b ON SUBSTRING(a.vcPart_id, 1, 5)=b.vcValue");
                sbr.AppendLine("WHERE a.iAuto_id IN (" + idList + ");");
                return excute.ExcuteSqlWithSelectToDT(sbr.ToString(), "TK");
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
                sql.Append("  delete TOldYearManager where iAuto_id in(   \r\n ");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (i != 0)
                        sql.Append(",");
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAuto_id"]);
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
        //保存
        #region 保存
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
                    sql.Append(" dSSDate = " + ComFunction.getSqlValue(dt.Rows[i]["dSSDate"], true) + ", \r\n");
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
                    sql.Append(" AND isnull(vcSYTCode,'') = '" + getValue("C016", dt.Rows[i]["vcSYTCode"].ToString()) + "' \r\n");
                    sql.Append(" AND isnull(vcOriginCompany,'') = '" + getValue("C006", dt.Rows[i]["vcOriginCompany"].ToString()) + "' \r\n");
                    sql.Append(" AND isnull(vcReceiver,'') = '" + getValue("C005", dt.Rows[i]["vcReceiver"].ToString()) + "' \r\n");
                    sql.Append(" AND isnull(vcInOutflag,'') = '" + getValue("C003", dt.Rows[i]["vcInOutflag"].ToString()) + "' \r\n");

                    if (!vcFinish.Equals("对象外"))
                    {
                        sql.Append(" AND vcFinish = '" + getValue("C024", vcFinish) + "' \r\n");
                    }


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
                    if (vcFinish.Equals("对象外"))
                    {
                        sql.Append(" vcFinish = '" + getValue("C024", vcFinish) + "', \r\n");
                        sql.Append(" dFinishYMD = GETDATE(), \r\n");
                    }
                    sql.Append(" vcNum1 = " + ComFunction.getSqlValue(list[i]["vcNum1"], false) + ", \r\n");
                    sql.Append(" vcNum2 = " + ComFunction.getSqlValue(list[i]["vcNum2"], false) + ", \r\n");
                    sql.Append(" vcNum3 = " + ComFunction.getSqlValue(list[i]["vcNum3"], false) + ", \r\n");
                    sql.Append(" vcNXQF = " + ComFunction.getSqlValue(list[i]["vcNXQF"], false) + ", \r\n");
                    sql.Append(" dSSDate = " + ComFunction.getSqlValue(list[i]["dSSDate"], true) + ", \r\n");
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
                    sql.Append(" iAuto_Id = " + iAutoId + " \r\n");


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

        //展开账票
        public void ZKZP(List<int> idList, string strUserId)
        {
            try
            {
                string id = "";
                for (int i = 0; i < idList.Count; i++)
                {
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        id += ",";
                    }

                    id += idList[i];

                }

                StringBuilder sbr = new StringBuilder();
                sbr.Append(" UPDATE TOldYearManager SET vcFinish = '3',dFinishYMD = GETDATE(),vcOperatorID = '" + strUserId + "',dOperatorTime = GETDATE() \r\n");
                sbr.Append(" WHERE iAuto_id in ( " + id + " ) \r\n");

                excute.ExcuteSqlWithStringOper(sbr.ToString(), "TK");
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

        //织入原单位
        public void InsertUnitApi(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAuto_id"]);
                    string vcNXQF = listInfoData[i]["vcNXQF"].ToString();

                    sbr.AppendLine("UPDATE TUnit SET ");
                    sbr.AppendLine("vcNXQF = " + ComFunction.getSqlValue(listInfoData[i]["vcNXQF"], false) + ", ");
                    sbr.AppendLine("vcNum11 = " + ComFunction.getSqlValue(listInfoData[i]["vcNum11"], false) + ", ");
                    sbr.AppendLine("vcNum12 = " + ComFunction.getSqlValue(listInfoData[i]["vcNum12"], false) + ", ");
                    sbr.AppendLine("vcNum13 = " + ComFunction.getSqlValue(listInfoData[i]["vcNum13"], false) + ", ");
                    sbr.AppendLine("vcNum14 = " + ComFunction.getSqlValue(listInfoData[i]["vcNum14"], false) + ", ");
                    sbr.AppendLine("vcNum15 = " + ComFunction.getSqlValue(listInfoData[i]["vcNum15"], false) + ", ");
                    sbr.AppendLine("vcChange = '8', ");
                    sbr.AppendLine("dSSDate = " + ComFunction.getSqlValue(listInfoData[i]["dSSDate"], true) + ",");
                    if (vcNXQF.Equals("一括生产") || vcNXQF.Equals("生产打切"))
                    {
                        sbr.AppendLine("vcDiff = '4', ");
                    }
                    sbr.AppendLine("vcMeno = isnull(vcMeno,'')+'年限区分;' , \r\n");
                    sbr.AppendLine("vcOperator = '" + strUserId + "', ");
                    sbr.AppendLine("dOperatorTime = GETDATE()");
                    sbr.AppendLine("WHERE ");
                    sbr.AppendLine("vcPart_id = " + ComFunction.getSqlValue(listInfoData[i]["vcPart_id"], false) + "");
                    sbr.AppendLine("AND vcReceiver = " + ComFunction.getSqlValue(getValue("C005", ObjToString(listInfoData[i]["vcReceiver"])), false) + " ");
                    sbr.AppendLine("AND vcSYTCode = " + ComFunction.getSqlValue(getValue("C016", ObjToString(listInfoData[i]["vcSYTCode"])), false) + " ");
                    sbr.AppendLine("AND vcOriginCompany = " + ComFunction.getSqlValue(getValue("C006", ObjToString(listInfoData[i]["vcOriginCompany"])), false) + " ");
                    sbr.AppendLine("AND vcSupplier_id = " + ComFunction.getSqlValue(listInfoData[i]["vcSupplier_id"], false) + " ");
                    sbr.AppendLine("AND vcCarTypeDev = " + ComFunction.getSqlValue(listInfoData[i]["vcCarTypeDev"], false) + " ");
                    sbr.AppendLine("AND  dTimeFrom <= GETDATE() AND dTimeTo >= GETDATE()");

                    sbr.AppendLine(" UPDATE TOldYearManager SET vcFinish = '4' WHERE iAuto_id = " + iAutoId + " ");

                }

                if (sbr.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(sbr.ToString(), "TK");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ObjToString(Object obj)
        {
            try
            {
                return obj.ToString();
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}