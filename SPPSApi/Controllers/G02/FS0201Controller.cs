using System;
using System.IO;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Office.Interop.Excel;

namespace SPPSApi.Controllers.G02
{
    [Route("api/[controller]/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0201Controller : BaseController
    {
        //FS0201_Logic fs0201_logic = new FS0201_Logic();
        private readonly IWebHostEnvironment _webHostEnvironment;
        public FS0201Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost]
        [EnableCors("any")]
        public void test()
        {
            string path = Directory.GetCurrentDirectory() + @"\Doc\import";
            try
            {
                getTTCCData(path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        public static void getTTCCData(string path)
        {
            try
            {
                if (Directory.GetFiles(path).Length == 0)
                {
                    Console.WriteLine("没有该路径");
                    return;
                }
                ChangePath(path);
                CreateList();
                //DataTable dt = ReadExcel();
                //return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void ChangePath(string path)
        {
            Console.WriteLine("开始修改文件路径");
            Application app = new Application();
            app.Visible = false;
            string filePath = Directory.GetCurrentDirectory() + @"\Doc\template\TestList.xlsm";
            string a = @path;
            Workbook wb = app.Workbooks.Open(filePath);
            try
            {
                object objRtn = new object();
                ComFunction.RunExcelMacro(app, "getPath", new Object[] { a }, out objRtn);
            }
            catch (Exception ex)
            {
                Console.WriteLine("修改文件路径失败，原因：" + ex.Message);

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
                Console.WriteLine("修改文件路径结束");
            }


        }
        public static void CreateList()
        {
            Console.WriteLine("读取文件生成新文件");

            Application app = new Application();
            app.Visible = false;
            string filePath = Directory.GetCurrentDirectory() + @"\Doc\template\TestList.xlsm";

            Workbook wb = app.Workbooks.Open(filePath);
            try
            {
                object objRtn = new object();
                ComFunction.RunExcelMacro(app, "MakeList", new Object[] { }, out objRtn);
            }
            catch (Exception ex)
            {
                Console.WriteLine("读取失败，原因：" + ex);

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
                Console.WriteLine("读取结束");
            }
        }

    }
}