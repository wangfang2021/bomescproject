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


                sbr.AppendLine("SELECT a.vcSupplier_id, a.total, ISNULL(b.total, '0') AS overNum, LTRIM(CONVERT(NUMERIC(9, 2), CAST(ISNULL(b.total, '0') AS INT)* 100.0 / CAST(a.total AS INT)))+'%' AS per");
                sbr.AppendLine("FROM(SELECT vcSupplier_id, COUNT(iAutoId) AS total");
                sbr.AppendLine("     FROM TSQJD");
                sbr.AppendLine("	 WHERE 1=1");
                if (!string.IsNullOrWhiteSpace(startTime))
                {
                    sbr.AppendLine("AND dNQDate>='" + startTime + "'");
                }
                if (!string.IsNullOrWhiteSpace(endTime))
                {
                    sbr.AppendLine("AND dNQDate <= '" + endTime + "'");
                }
                sbr.AppendLine("     GROUP BY vcSupplier_id");
                sbr.AppendLine("	 ) a");
                sbr.AppendLine("    LEFT JOIN(SELECT vcSupplier_id, COUNT(iAutoId) AS total");
                sbr.AppendLine("              FROM TSQJD");
                sbr.AppendLine("              WHERE ISNULL(vcYQorNG, '')='' ");
                sbr.AppendLine("			  AND((vcJD='1' AND dNqDate<GETDATE())OR(vcJD='2' AND dNqDate<dHFDate))");
                if (!string.IsNullOrWhiteSpace(startTime))
                {
                    sbr.AppendLine("AND dNQDate>='" + startTime + "'");
                }
                if (!string.IsNullOrWhiteSpace(endTime))
                {
                    sbr.AppendLine("AND dNQDate <= '" + endTime + "'");
                }
                sbr.AppendLine("              GROUP BY vcSupplier_id) b ON a.vcSupplier_id=b.vcSupplier_id");
                if (!string.IsNullOrWhiteSpace(supplierId))
                {
                    sbr.AppendLine("Where a.vcSupplier_id like '" + supplierId + "%'");
                }





                return excute.ExcuteSqlWithSelectToDT(sbr.ToString(), "TK");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}