using System;
using System.Collections.Generic;
using Common;
using System.Data;
using System.Text;

namespace DataAccess
{
    public class FS0315_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 生成报表

        public List<DataTable> searchApi(string startTime, string endTime, List<string> Type, string vcOriginCompany)
        {
            try
            {
                List<DataTable> list = new List<DataTable>();
                StringBuilder sbr = new StringBuilder();

                sbr.AppendLine("---日别");
                sbr.AppendLine("IF EXISTS (SELECT * ");
                sbr.AppendLine("FROM tempdb.dbo.sysobjects ");
                sbr.AppendLine("WHERE id=OBJECT_ID(N'tempdb..#temp')AND type='U')");
                sbr.AppendLine("DROP TABLE #temp;");
                sbr.AppendLine("SELECT SUBSTRING(vcFileNameTJ,7,4) AS Year,SUBSTRING(vcFileNameTJ,11,2) AS Month,SUBSTRING(vcFileNameTJ,13,2) AS Day,iAutoId,vcChange INTO #temp FROM TSBManager ");
                sbr.AppendLine("WHERE vcType = '0' ");
                if (!string.IsNullOrWhiteSpace(startTime))
                {
                    sbr.AppendLine(" AND '" + startTime + "'<=SUBSTRING(vcFileNameTJ,7,8)");
                }

                if (!string.IsNullOrWhiteSpace(vcOriginCompany))
                {
                    sbr.AppendLine(" AND vcOriginCompany = '" + vcOriginCompany + "'");
                }
                if (!string.IsNullOrWhiteSpace(endTime))
                {
                    sbr.AppendLine(" AND '" + endTime + "' >= SUBSTRING(vcFileNameTJ,7,8)");
                }
                if (Type.Count > 0)
                {
                    string tmp = "";
                    foreach (string s in Type)
                    {
                        if (tmp.Length > 0)
                        {
                            tmp += ",";
                        }
                        tmp += "'" + s + "'";
                    }
                    sbr.AppendLine("AND SUBSTRING(vcFileNameTJ,1,2) in (" + tmp + ")");
                }

                sbr.AppendLine("");
                sbr.AppendLine("SELECT a.Year,a.Month,a.Day,ISNULL(d.NUM,0) AS iXS,ISNULL(e.NUM,0) AS iFZ,ISNULL(f.NUM,0) AS iJX,(ISNULL(b.NUM,0)-ISNULL(d.NUM,0)-ISNULL(e.NUM,0)-ISNULL(f.NUM,0)) AS iOther,ISNULL(b.NUM,0) AS idaySUM,ISNULL(c.NUM,0) AS imonthSUM");
                sbr.AppendLine("from");
                sbr.AppendLine("(SELECT distinct Year,Month,Day FROM #temp) a");
                sbr.AppendLine("LEFT JOIN ");
                sbr.AppendLine("(SELECT Year,Month,Day,COUNT(iAutoId) AS NUM FROM #temp GROUP BY Year,Month,Day) b ON a.Year = b.Year AND a.Month = b.Month AND a.Day = b.Day");
                sbr.AppendLine("LEFT JOIN ");
                sbr.AppendLine("(SELECT Year,Month,COUNT(iAutoId) AS NUM FROM #temp GROUP BY Year,Month) c ON a.Year = c.Year AND a.Month = c.Month");
                sbr.AppendLine("LEFT JOIN");
                sbr.AppendLine("(SELECT Year,Month,Day,COUNT(iAutoId) AS NUM FROM #temp WHERE vcChange = '补给新设' GROUP BY Year,Month,Day) d ON a.Year = d.Year AND a.Month = d.Month AND a.Day = d.Day");
                sbr.AppendLine("LEFT JOIN");
                sbr.AppendLine("(SELECT Year,Month,Day,COUNT(iAutoId) AS NUM FROM #temp WHERE vcChange = '补给废止' GROUP BY Year,Month,Day) e ON a.Year = e.Year AND a.Month = e.Month AND a.Day = e.Day");
                sbr.AppendLine("LEFT JOIN");
                sbr.AppendLine("(SELECT Year,Month,Day,COUNT(iAutoId) AS NUM FROM #temp WHERE vcChange = '旧型' GROUP BY Year,Month,Day) f ON a.Year = f.Year AND a.Month = f.Month AND a.Day = f.Day");

                if (sbr.Length > 0)
                {
                    list.Add(excute.ExcuteSqlWithSelectToDT(sbr.ToString()));
                }

                sbr.Length = 0;

                sbr.AppendLine("IF EXISTS (SELECT * ");
                sbr.AppendLine("FROM tempdb.dbo.sysobjects ");
                sbr.AppendLine("WHERE id=OBJECT_ID(N'tempdb..#temp')AND type='U')");
                sbr.AppendLine("DROP TABLE #temp;");
                sbr.AppendLine("SELECT SUBSTRING(vcFileNameTJ,7,4) AS Year,SUBSTRING(vcFileNameTJ,11,2) AS Month,iAutoId,vcChange INTO #temp FROM TSBManager ");
                sbr.AppendLine("WHERE vcType = '0' ");
                if (!string.IsNullOrWhiteSpace(startTime))
                {
                    sbr.AppendLine(" AND '" + startTime + "'<=SUBSTRING(vcFileNameTJ,7,8)");
                }
                if (!string.IsNullOrWhiteSpace(vcOriginCompany))
                {
                    sbr.AppendLine(" AND vcOriginCompany = '" + vcOriginCompany + "'");
                }
                if (!string.IsNullOrWhiteSpace(endTime))
                {
                    sbr.AppendLine(" AND '" + endTime + "' >= SUBSTRING(vcFileNameTJ,7,8)");
                }
                if (Type.Count > 0)
                {
                    string tmp = "";
                    foreach (string s in Type)
                    {
                        if (tmp.Length > 0)
                        {
                            tmp += ",";
                        }
                        tmp += "'" + s + "'";
                    }
                    sbr.AppendLine("AND SUBSTRING(vcFileNameTJ,1,2) in (" + tmp + ")");
                }
                sbr.AppendLine("");
                sbr.AppendLine("SELECT a.Year+'/'+a.Month as Date,ISNULL(c.NUM,0) AS iXS,ISNULL(d.NUM,0) AS iFZ,ISNULL(e.NUM,0) AS iJX,(ISNULL(b.NUM,0)-ISNULL(c.NUM,0)-ISNULL(d.NUM,0)-ISNULL(e.NUM,0)) AS iOther,ISNULL(b.NUM,0) AS imonthSUM");
                sbr.AppendLine("from");
                sbr.AppendLine("(SELECT distinct Year,Month FROM #temp) a");
                sbr.AppendLine("LEFT JOIN ");
                sbr.AppendLine("(SELECT Year,Month,COUNT(iAutoId) AS NUM FROM #temp GROUP BY Year,Month) b ON a.Year = b.Year AND a.Month = b.Month ");
                sbr.AppendLine("LEFT JOIN");
                sbr.AppendLine("(SELECT Year,Month,COUNT(iAutoId) AS NUM FROM #temp WHERE vcChange = '补给新设' GROUP BY Year,Month) c ON a.Year = c.Year AND a.Month = c.Month");
                sbr.AppendLine("LEFT JOIN");
                sbr.AppendLine("(SELECT Year,Month,COUNT(iAutoId) AS NUM FROM #temp WHERE vcChange = '补给废止' GROUP BY Year,Month) d ON a.Year = d.Year AND a.Month = d.Month");
                sbr.AppendLine("LEFT JOIN");
                sbr.AppendLine("(SELECT Year,Month,COUNT(iAutoId) AS NUM FROM #temp WHERE vcChange = '旧型' GROUP BY Year,Month) e ON a.Year = e.Year AND a.Month = e.Month");

                if (sbr.Length > 0)
                {
                    list.Add(excute.ExcuteSqlWithSelectToDT(sbr.ToString(), "TK"));
                }

                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}