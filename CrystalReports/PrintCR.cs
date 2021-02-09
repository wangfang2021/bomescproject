using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace CrystalReports
{
    public class PrintCR
    {
        public void printCrReport(DataTable dataTable, string printerName, string reportName, string left, string up)
        {

            string ServerName = "172.23.140.169";
            string DatabaseName = "SPPSdb";
            string UserID = "sa";
            string Password = "Sa123";
            ReportDocument rd = new ReportDocument();
            try
            {
                rd.Load(reportName);
                TableLogOnInfo logOnInfo = new TableLogOnInfo();
                logOnInfo.ConnectionInfo.ServerName = ServerName;
                logOnInfo.ConnectionInfo.DatabaseName = DatabaseName;
                logOnInfo.ConnectionInfo.UserID = UserID;
                logOnInfo.ConnectionInfo.Password = Password;
                PageMargins margins = rd.PrintOptions.PageMargins;
                margins.leftMargin = Int32.Parse(left);
                margins.bottomMargin = 0;
                margins.rightMargin = 0;
                margins.topMargin = Int32.Parse(up);
                rd.Database.Tables[0].ApplyLogOnInfo(logOnInfo);
                rd.PrintOptions.PrinterName = printerName;
                rd.PrintOptions.ApplyPageMargins(margins);
                rd.SetDataSource(dataTable);
                rd.PrintToPrinter(1, true, 0, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                rd.Close();
                rd.Dispose();
                if (dataTable.Rows.Count < 10)
                {
                    System.Threading.Thread.Sleep(5000);
                }

            }
        }
    }
}
