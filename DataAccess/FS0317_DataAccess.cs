using System;
using Common;
using System.Data;
using System.Text;

namespace DataAccess
{
    public class FS0317_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        public DataTable searchApi(string startTime, string endTime, string supplierId)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine("IF EXISTS (SELECT * ");
                sbr.AppendLine("FROM tempdb.dbo.sysobjects ");
                sbr.AppendLine("WHERE id=OBJECT_ID(N'tempdb..#temp')AND type='U')");
                sbr.AppendLine("DROP TABLE #temp;");
                sbr.AppendLine("");
                sbr.AppendLine("SELECT vcSupplier_id,vcSQState,dNQDate,vcPart_id INTO #temp FROM TUnit WHERE vcInOutflag = '1'");
                if (!string.IsNullOrWhiteSpace(startTime))
                {
                    sbr.AppendLine("AND dNQDate>='" + startTime + "'");
                }
                if (!string.IsNullOrWhiteSpace(endTime))
                {
                    sbr.AppendLine("AND dNQDate <= '" + endTime + "'");
                }
                if (!string.IsNullOrWhiteSpace(supplierId))
                {
                    sbr.AppendLine("AND vcSupplier_id = '" + supplierId + "'");
                }
                sbr.AppendLine("SELECT a.vcSupplier_id,a.total,ISNULL(b.total,'0') AS overNum,LTRIM(CONVERT(NUMERIC(9,2),CAST(ISNULL(b.total,'0') AS int)*100.0/CAST(a.total AS int)))+'%' AS per FROM ");
                sbr.AppendLine("(");
                sbr.AppendLine("SELECT vcSupplier_id,COUNT(*) AS total FROM #temp GROUP BY vcSupplier_id");
                sbr.AppendLine(") a");
                sbr.AppendLine("LEFT JOIN");
                sbr.AppendLine("(");
                sbr.AppendLine("SELECT vcSupplier_id,COUNT(*) AS total FROM #temp WHERE dNQDate < GETDATE() AND isnull(vcSQState,'') <> '2' GROUP BY vcSupplier_id");
                sbr.AppendLine(") b ON a.vcSupplier_id = b.vcSupplier_id");

                return excute.ExcuteSqlWithSelectToDT(sbr.ToString(), "TK");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}