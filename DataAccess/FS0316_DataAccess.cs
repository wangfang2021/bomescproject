using System;
using System.Collections.Generic;
using Common;
using System.Data;
using System.Text;
using Org.BouncyCastle.Asn1;

namespace DataAccess
{
    public class FS0316_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        public DataTable searchApi(string flag, List<string> Origin, string supplierId, List<string> project)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                if (flag == "0")
                {
                    sbr.AppendLine("---工厂别");
                    sbr.AppendLine("IF EXISTS (SELECT * ");
                    sbr.AppendLine("FROM tempdb.dbo.sysobjects ");
                    sbr.AppendLine("WHERE id=OBJECT_ID(N'tempdb..#temp')AND type='U')");
                    sbr.AppendLine("DROP TABLE #temp;");
                    sbr.AppendLine("");
                    sbr.AppendLine("SELECT iAutoId,vcInOutflag,vcHaoJiu,vcOE,vcOriginCompany INTO #temp FROM TUnit ");
                    sbr.AppendLine("WHERE 1=1");
                    if (Origin.Count > 0)
                    {
                        string tmp = "";
                        foreach (string s in Origin)
                        {
                            if (tmp.Length > 0)
                            {
                                tmp += ",";
                            }
                            tmp += "'" + s + "'";
                        }

                        sbr.AppendLine("AND vcOriginCompany IN (" + tmp + ") ");
                    }
                    sbr.AppendLine("AND dTimeFrom <= GETDATE()");
                    sbr.AppendLine("AND dTimeTo >= GETDATE()");
                    sbr.AppendLine("");
                    sbr.AppendLine("SELECT d.vcName AS vcOriginCompany,e.vcName AS vcInOutflag,ISNULL(a.Total,0) AS Total,f.vcName AS vcHaoJiu,ISNULL(b.Total,0) AS HJTotal,g.vcMeaning as vcOE,ISNULL(c.Total,0) AS OETotal from");
                    sbr.AppendLine("(");
                    sbr.AppendLine("	SELECT vcOriginCompany,vcInOutflag,COUNT(iAutoId) AS Total FROM #temp GROUP BY vcOriginCompany,vcInOutflag");
                    sbr.AppendLine(") a ");
                    sbr.AppendLine("LEFT JOIN");
                    sbr.AppendLine("(");
                    sbr.AppendLine("	SELECT vcOriginCompany,vcInOutflag,vcHaoJiu,vcOE,SUM(Total) AS Total FROM");
                    sbr.AppendLine("	(");
                    sbr.AppendLine("		SELECT vcOriginCompany,vcInOutflag,'H' AS vcHaoJiu,'0' AS vcOE,COUNT(iAutoId) AS Total FROM #temp WHERE vcHaoJiu = 'H' GROUP BY vcOriginCompany,vcInOutflag");
                    sbr.AppendLine("		UNION");
                    sbr.AppendLine("		SELECT DISTINCT vcOriginCompany,vcInOutflag,'H' AS vcHaoJiu,'0' AS vcOE,0 AS Total FROM #temp WHERE vcHaoJiu = 'H' GROUP BY vcOriginCompany,vcInOutflag");
                    sbr.AppendLine("		UNION ");
                    sbr.AppendLine("		SELECT vcOriginCompany,vcInOutflag,'Q' AS vcHaoJiu,'1' AS vcOE,COUNT(iAutoId) AS Total FROM #temp WHERE vcHaoJiu = 'Q' GROUP BY vcOriginCompany,vcInOutflag");
                    sbr.AppendLine("		UNION");
                    sbr.AppendLine("		SELECT DISTINCT vcOriginCompany,vcInOutflag,'Q' AS vcHaoJiu,'1' AS vcOE,0 AS Total FROM #temp WHERE vcHaoJiu = 'H' GROUP BY vcOriginCompany,vcInOutflag");
                    sbr.AppendLine("	) a GROUP BY vcOriginCompany,vcInOutflag,vcHaoJiu,vcOE");
                    sbr.AppendLine(") b ON a.vcOriginCompany = B.vcOriginCompany AND a.vcInOutflag = b.vcInOutflag");
                    sbr.AppendLine("LEFT JOIN");
                    sbr.AppendLine("(");
                    sbr.AppendLine("SELECT vcOriginCompany,vcInOutflag,vcHaoJiu,vcOE,SUM(Total) AS Total FROM");
                    sbr.AppendLine("	(");
                    sbr.AppendLine("	SELECT vcOriginCompany,vcInOutflag,'H' AS vcHaoJiu,'0' AS vcOE,COUNT(iAutoId) AS Total FROM #temp WHERE vcOE = '0' GROUP BY vcOriginCompany,vcInOutflag");
                    sbr.AppendLine("	UNION ");
                    sbr.AppendLine("	SELECT vcOriginCompany,vcInOutflag,'Q' AS vcHaoJiu,'1' AS vcOE,COUNT(iAutoId) AS Total FROM #temp WHERE vcOE = '1' GROUP BY vcOriginCompany,vcInOutflag");
                    sbr.AppendLine("	UNION");
                    sbr.AppendLine("	SELECT DISTINCT vcOriginCompany,vcInOutflag,'H' AS vcHaoJiu,'0' AS vcOE,0 AS Total FROM #temp WHERE vcHaoJiu = 'H' GROUP BY vcOriginCompany,vcInOutflag");
                    sbr.AppendLine("	UNION");
                    sbr.AppendLine("	SELECT DISTINCT vcOriginCompany,vcInOutflag,'Q' AS vcHaoJiu,'1' AS vcOE,0 AS Total FROM #temp WHERE vcHaoJiu = 'H' GROUP BY vcOriginCompany,vcInOutflag");
                    sbr.AppendLine(") a GROUP BY vcOriginCompany,vcInOutflag,vcHaoJiu,vcOE");
                    sbr.AppendLine(") c ON c.vcOriginCompany =b.vcOriginCompany AND c.vcInOutflag = b.vcInOutflag AND c.vcHaoJiu = b.vcHaoJiu AND c.vcOE = b.vcOE");
                    sbr.AppendLine("LEFT JOIN");
                    sbr.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId = 'C006') d ON a.vcOriginCompany = d.vcValue");
                    sbr.AppendLine("LEFT JOIN");
                    sbr.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId = 'C003') e ON a.vcInOutflag = e.vcValue");
                    sbr.AppendLine("LEFT JOIN");
                    sbr.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId = 'C004') f ON b.vcHaoJiu = f.vcValue");
                    sbr.AppendLine("LEFT JOIN");
                    sbr.AppendLine("(SELECT vcName,vcValue,vcMeaning FROM TCode WHERE vcCodeId = 'C012') g ON c.vcOE = g.vcValue");
                    sbr.AppendLine("ORDER BY a.vcOriginCompany,a.vcInOutflag,b.vcHaoJiu,c.vcOE");

                }
                else if (flag == "1")
                {
                    sbr.AppendLine("IF EXISTS (SELECT * ");
                    sbr.AppendLine("FROM tempdb.dbo.sysobjects ");
                    sbr.AppendLine("WHERE id=OBJECT_ID(N'tempdb..#temp')AND type='U')");
                    sbr.AppendLine("DROP TABLE #temp;");
                    sbr.AppendLine("");
                    sbr.AppendLine("SELECT iAutoId,vcSupplier_id,vcHaoJiu INTO #temp FROM TUnit ");
                    sbr.AppendLine("WHERE 1=1");
                    if (!string.IsNullOrWhiteSpace(supplierId))
                    {
                        sbr.AppendLine("AND vcSupplier_id = '" + supplierId + "'");
                    }
                    if (project.Count > 0)
                    {
                        string tmp = "";
                        foreach (string s in project)
                        {
                            if (tmp.Length > 0)
                            {
                                tmp += ",";
                            }
                            tmp += "'" + s + "'";
                        }
                        sbr.AppendLine("AND vcBJGC IN (" + tmp + ")");
                    }
                    sbr.AppendLine("AND dTimeFrom <= GETDATE()");
                    sbr.AppendLine("AND dTimeTo >= GETDATE()");
                    sbr.AppendLine("");
                    sbr.AppendLine("SELECT a.vcSupplier_id,a.Total,c.vcName AS vcHaoJiu,b.Total AS HJTotal FROM ");
                    sbr.AppendLine("(");
                    sbr.AppendLine("SELECT vcSupplier_id,COUNT(iAutoId) AS Total FROM #temp GROUP BY vcSupplier_id");
                    sbr.AppendLine(") a");
                    sbr.AppendLine("LEFT JOIN");
                    sbr.AppendLine("(");
                    sbr.AppendLine("	SELECT vcSupplier_id,vcHaoJiu,SUM(a.Total) AS Total FROM");
                    sbr.AppendLine("	(");
                    sbr.AppendLine("		SELECT vcSupplier_id,vcHaoJiu,COUNT(iAutoId) AS Total FROM #temp GROUP BY vcSupplier_id,vcHaoJiu");
                    sbr.AppendLine("		UNION");
                    sbr.AppendLine("		SELECT DISTINCT vcSupplier_id,'H' AS vcHaoJiu,0 AS Total FROM #temp");
                    sbr.AppendLine("		UNION");
                    sbr.AppendLine("		SELECT DISTINCT vcSupplier_id,'Q' AS vcHaoJiu,0 AS Total FROM #temp");
                    sbr.AppendLine("	) a GROUP BY vcSupplier_id,vcHaoJiu");
                    sbr.AppendLine(") b ON a.vcSupplier_id = b.vcSupplier_id");
                    sbr.AppendLine("LEFT JOIN");
                    sbr.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId = 'C004') c ON b.vcHaoJiu = c.vcValue");

                }

                if (sbr.Length > 0)
                {
                    return excute.ExcuteSqlWithSelectToDT(sbr.ToString(), "TK");
                }

                return null;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}