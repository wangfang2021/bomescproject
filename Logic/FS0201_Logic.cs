using System;
using Microsoft.Office.Interop.Excel;
using Common;
namespace Logic
{
    public class FS0201_Logic
    {
        FS0201_Logic fs0201_DataAccess = new FS0201_Logic();

        public void ChangePath(string path)
        {
            Application app = new Application();
            app.Visible = false;
            string filePath = @"D:\FS0201.xlsm";
            string a = @path;
            Workbook wb = app.Workbooks.Open(filePath);
            try
            {
                object objRtn = new object();
                ComFunction.RunExcelMacro(app, "getPath", new Object[] { a }, out objRtn);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (wb != null)
                {
                    wb.Save();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(wb);
                    app.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(app);

                }
            }


        }
        public static void CreateList()
        {
            Application app = new Application();
            app.Visible = false;
            string filePath = @"D:\TestList.xlsm";

            Workbook wb = app.Workbooks.Open(filePath);
            try
            {
                object objRtn = new object();
                ComFunction.RunExcelMacro(app, "MakeList", new Object[] { }, out objRtn);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (wb != null)
                {
                    wb.Save();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(wb);
                    app.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(app);

                }
            }


        }
    }
}