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

        //年限对象品番抽取
        public void extractPart(string strUserId)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();

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
                sbr.Append("  IF EXISTS (SELECT *  \r\n");
                sbr.Append("             FROM tempdb.dbo.sysobjects  \r\n");
                sbr.Append("             WHERE id=OBJECT_ID(N'tempdb..#temp')AND type='U')  \r\n");
                sbr.Append("      DROP TABLE #temp;  \r\n");
                sbr.Append("  SELECT vcSupplier_id, vcPart_id, vcPartNameEn, vcInOutflag, vcNXQF, vcCarTypeDev, dJiuBegin  \r\n");
                sbr.Append("  INTO #temp  \r\n");
                sbr.Append("  FROM TUnit  \r\n");
                sbr.Append("  WHERE vcChange=(SELECT vcValue FROM TCode WHERE vcName='旧型' AND vcCodeId='C002');  \r\n");
                sbr.Append("  INSERT INTO TOldYearManager(vcYear, vcFinish, vcSupplier_id, vcPart_id, vcPartNameEn, vcInOutflag, vcCarTypeDev, dJiuBegin, vcRemark, vcOld10, vcOld9, vcOld7, vcFlag, vcOperatorID, dOperatorTime)  \r\n");
                sbr.Append("  SELECT @Year AS vcYear, '0' AS vcFinish, vcSupplier_id, vcPart_id, vcPartNameEn, vcInOutflag, vcCarTypeDev, dJiuBegin, CASE vcNXQF WHEN '1' THEN '往年持续生产' ELSE '' END AS vcRemark, vcOld10, vcOld9, vcOld7, '0' AS vcFlag, '" + strUserId + "' AS vcOperatorID, GETDATE() AS dOperatorTime  \r\n");
                sbr.Append("  FROM(SELECT vcSupplier_id, vcPart_id, vcPartNameEn, vcInOutflag, vcNXQF, vcCarTypeDev, dJiuBegin, '1' AS vcOld7, '0' AS vcOld9, '0' AS vcOld10  \r\n");
                sbr.Append("       FROM #temp  \r\n");
                sbr.Append("       WHERE CONVERT(VARCHAR(12), dJiuBegin, 111)>=@7B AND CONVERT(VARCHAR(12), dJiuBegin, 111)<=@7E  \r\n");
                sbr.Append("       UNION ALL  \r\n");
                sbr.Append("       SELECT vcSupplier_id, vcPart_id, vcPartNameEn, vcInOutflag, vcNXQF, vcCarTypeDev, dJiuBegin, '0' AS vcOld7, '1' AS vcOld9, '0' AS vcOld10  \r\n");
                sbr.Append("       FROM #temp  \r\n");
                sbr.Append("       WHERE CONVERT(VARCHAR(12), dJiuBegin, 111)>=@9B AND CONVERT(VARCHAR(12), dJiuBegin, 111)<=@9E  \r\n");
                sbr.Append("       UNION ALL  \r\n");
                sbr.Append("       SELECT vcSupplier_id, vcPart_id, vcPartNameEn, vcInOutflag, vcNXQF, vcCarTypeDev, dJiuBegin, '0' AS vcOld7, '0' AS vcOld9, '1' AS vcOld10  \r\n");
                sbr.Append("       FROM #temp  \r\n");
                sbr.Append("       WHERE CONVERT(VARCHAR(12), dJiuBegin, 111)<=@10E) a;  \r\n");
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
                sql.Append(" UPDATE TOldYearManager SET vcFlag = '1',vcOperatorID = '" + strUserId + "',dOperatorTime = GETDATE() WHERE iAutoId IN (  \r\n ");
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
        public DataTable searchApi(string strYear, string FinishFlag)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();

                sbr.Append(" SELECT a.vcYear,b.vcName AS vcFinish, a.vcSupplier_id, a.vcPart_id, a.vcPartNameEn,d.vcName AS vcInOutflag, a.vcCarTypeDev, \r\n");
                sbr.Append(" a.dJiuBegin, a.vcRemark, a.vcOld10, a.vcOld9, a.vcOld7,c.vcName AS vcPM, a.vcNum1, a.vcNum2, a.vcNum3, a.vcYearDiff, \r\n");
                sbr.Append(" a.dTimeFrom, a.vcDY, a.vcNum11, a.vcNum12, a.vcNum13, a.vcNum14, a.vcNum15, a.vcNum16, a.vcNum17, a.vcNum18, a.vcNum19, a.vcNum20, a.vcNum21,'0' as vcModFlag \r\n");
                sbr.Append(" FROM TOldYearManager a \r\n");
                sbr.Append(" LEFT JOIN (SELECT vcName,vcValue FROM TCode WHERE vcCodeId = 'C024') b ON a.vcFinish = b.vcValue \r\n");
                sbr.Append(" LEFT JOIN (SELECT vcName,vcValue FROM TCode WHERE vcCodeId = 'C099') c ON SUBSTRING(a.vcPart_id,1,5) = c.vcValue \r\n");
                sbr.Append(" LEFT JOIN (SELECT vcName,vcValue FROM TCode WHERE vcCodeId = 'C003') d ON a.vcInOutflag = d.vcValue \r\n");
                sbr.Append(" WHERE a.vcYear = '" + strYear + "'  \r\n");
                sbr.Append(" AND a.vcFinish = '" + FinishFlag + "' \r\n");


                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}