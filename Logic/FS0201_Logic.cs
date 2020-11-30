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
            string filePath = @"D:\TestList.xlsm";
            string a = @path;
            Workbook wb = app.Workbooks.Open(filePath);
            try
            {
                object objRtn = new object();
                RunExcelMacro(app, "getPath", new Object[] { a }, out objRtn);
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
                RunExcelMacro(app, "MakeList", new Object[] { }, out objRtn);
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

        public static void RunExcelMacro(Microsoft.Office.Interop.Excel.Application app, string macroName, object[] parameters, out object rtnValue)
        {
            // 根据参数组是否为空，准备参数组对象
            object[] paraObjects;
            if (parameters == null)
                paraObjects = new object[] { macroName };
            else
            {
                int paraLength = parameters.Length;
                paraObjects = new object[paraLength + 1];
                paraObjects[0] = macroName;
                for (int i = 0; i < paraLength; i++)
                    paraObjects[i + 1] = parameters[i];
            }
            rtnValue = RunMacro(app, paraObjects);
        }

        private static object RunMacro(object app, object[] oRunArgs)
        {
            object objRtn;     // 声明一个返回对象

            // 反射方式执行宏
            objRtn = app.GetType().InvokeMember("Run", System.Reflection.BindingFlags.Default | System.Reflection.BindingFlags.InvokeMethod, null, app, oRunArgs);

            return objRtn;
        }
    }
}