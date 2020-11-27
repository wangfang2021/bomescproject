using System;
using System.Collections.Generic;
using System.IO;

using System.Net.Http.Headers;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
 

namespace WebApplication5.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class ExcelController : BaseController
    {
        SLogin_Logic slogin_Logic = new SLogin_Logic();


        private readonly IWebHostEnvironment _webHostEnvironment;
        public ExcelController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }


        //        export const asyncRoutes = [
        //  {
        //    path: '/permission',
        //    component: Layout,
        //    redirect: '/permission/page',
        //    alwaysShow: true, // will always show the root menu
        //    name: 'Permission',
        //    meta: {
        //      title: 'permission',
        //      icon: 'lock',
        //      roles: ['admin', 'editor'] // you can set roles in root nav
        //    },
        //    children: [
        //      {
        //        path: 'page',
        //        component: () => import('@/views/permission/page'),
        //        name: 'PagePermission',
        //        meta: {
        //          title: 'pagePermission',
        //          roles: ['admin'] // or you can only set roles in sub nav
        //}
        //      },
        public class RouteMeta
        {
            public string title;
            public string icon;
            public string[] roles;
            public bool noCache;
        }
        public class AsyncRoutes
        {
            public string path;
            public string component;
            public string redirect;
            public bool alwaysShow;
            public string name;
            public bool hidden;
            public int sort;
            public string pid;
            public RouteMeta meta;
            public List<AsyncRoutes> children;
        }
 
        [HttpGet]
        public string getRoutes(string token)
        {
            //AsyncRoutes asyncRoutes = new AsyncRoutes();
            //asyncRoutes.path = "/permission";
            //asyncRoutes.component = "Layout";
            //asyncRoutes.redirect = "/permission/page";
            //asyncRoutes.alwaysShow = true;
            //asyncRoutes.name = "Permission";
            //asyncRoutes.hidden = false;
            //RouteMeta meta = new RouteMeta();
            //meta.title = "permission";
            //meta.icon = "lock";
            //meta.roles = new string[] { "admin", "editor" };
            //asyncRoutes.meta = meta;


            //AsyncRoutes asyncRoutes_child1 = new AsyncRoutes();
            //asyncRoutes_child1.path = "page";
            //asyncRoutes_child1.component = "permission/page";
            //asyncRoutes_child1.name = "PagePermission";
            //asyncRoutes_child1.hidden = false;

            //RouteMeta meta_child1 = new RouteMeta();
            //meta_child1.title = "pagePermission";
            //meta_child1.roles = new string[] { "admin" , "editor" };
            //asyncRoutes_child1.meta = meta_child1;

            //AsyncRoutes asyncRoutes_child2 = new AsyncRoutes();
            //asyncRoutes_child2.path = "directive";
            //asyncRoutes_child2.component = "permission/directive";
            //asyncRoutes_child2.name = "DirectivePermission";
            //asyncRoutes_child2.hidden = false;

            //RouteMeta meta_child2 = new RouteMeta();
            //meta_child2.title = "directivePermission";
            ////meta_child2.roles = new string[] { "admin" };
            //asyncRoutes_child2.meta = meta_child2;

            //AsyncRoutes asyncRoutes_child3 = new AsyncRoutes();
            //asyncRoutes_child3.path = "role";
            //asyncRoutes_child3.component = "permission/role";
            //asyncRoutes_child3.name = "RolePermission";
            //asyncRoutes_child3.hidden = false;
           

            //RouteMeta meta_child3 = new RouteMeta();
            //meta_child3.title = "rolePermission";
            //meta_child3.roles = new string[] { "admin", "editor" };
            //asyncRoutes_child3.meta = meta_child3;

            //asyncRoutes.children = new List<AsyncRoutes>();
            //asyncRoutes.children.Add(asyncRoutes_child1);
            //asyncRoutes.children.Add(asyncRoutes_child2);
            //asyncRoutes.children.Add(asyncRoutes_child3);


            AsyncRoutes asyncRoutes2 = new AsyncRoutes();
            asyncRoutes2.path = "/icon";
            asyncRoutes2.component = "Layout";
            asyncRoutes2.hidden = false;

            AsyncRoutes asyncRoutes2_child1 = new AsyncRoutes();
            asyncRoutes2_child1.path = "index";
            asyncRoutes2_child1.component = "icons/index";
            asyncRoutes2_child1.name = "Icons";
            asyncRoutes2_child1.hidden = false;

            RouteMeta meta2_child1 = new RouteMeta();
            meta2_child1.title = "icons";
            meta2_child1.icon = "icon";
            meta2_child1.noCache = true;

            asyncRoutes2.children= new List<AsyncRoutes>();
            asyncRoutes2.children.Add(asyncRoutes2_child1);


            AsyncRoutes asyncRoutes3 = new AsyncRoutes();
            asyncRoutes3.path = "/G01";
            asyncRoutes3.component = "Layout";
            asyncRoutes3.alwaysShow = false;
            asyncRoutes3.name = "G01";
            asyncRoutes3.hidden = false;
            RouteMeta meta3 = new RouteMeta();
            meta3.title = "系统管理";
            meta3.icon = "lock";
            meta3.roles = new string[] { "admin", "editor" };
            asyncRoutes3.meta = meta3;



            AsyncRoutes asyncRoutes3_child1 = new AsyncRoutes();
            asyncRoutes3_child1.path = "/FS0101";
            asyncRoutes3_child1.component = "G00/FS0101";
            asyncRoutes3_child1.name = "FS0101";
            asyncRoutes3_child1.hidden = false;

            RouteMeta meta3_child1 = new RouteMeta();
            meta3_child1.title = "用户管理";
            meta3_child1.roles = new string[] { "admin", "editor" };
            asyncRoutes3_child1.meta = meta3_child1;


            AsyncRoutes asyncRoutes3_child2 = new AsyncRoutes();
            asyncRoutes3_child2.path = "/FS0102";
            asyncRoutes3_child2.component = "G00/FS0102";
            asyncRoutes3_child2.name = "FS0102";
            asyncRoutes3_child2.hidden = false;

            RouteMeta meta3_child2 = new RouteMeta();
            meta3_child2.title = "角色管理";
            meta3_child2.roles = new string[] { "admin", "editor" };
            asyncRoutes3_child2.meta = meta3_child2;

            AsyncRoutes asyncRoutes3_child3 = new AsyncRoutes();
            asyncRoutes3_child3.path = "/FS0103";
            asyncRoutes3_child3.component = "G00/FS0103";
            asyncRoutes3_child3.name = "FS0103";
            asyncRoutes3_child3.hidden = false;

            RouteMeta meta3_child3 = new RouteMeta();
            meta3_child3.title = "日志查询";
            meta3_child3.roles = new string[] { "admin", "editor" };
            asyncRoutes3_child3.meta = meta3_child3;
 


            asyncRoutes3.children = new List<AsyncRoutes>();
            asyncRoutes3.children.Add(asyncRoutes3_child1);
            asyncRoutes3.children.Add(asyncRoutes3_child2);
            asyncRoutes3.children.Add(asyncRoutes3_child3);





            List<Object> dataList = new List<Object>();
           // dataList.Add(asyncRoutes);
            dataList.Add(asyncRoutes2);
            dataList.Add(asyncRoutes3);

            ApiResult apiResult = new ApiResult();
            apiResult.code = 20000;
            apiResult.data = dataList;


            var JsonSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JsonSetting);
        }


        [HttpGet]

        public string addUser(string strName, int iAge)
        {
            try
            {
                Console.WriteLine("strName " + strName);
                Console.WriteLine("iAge " + iAge);
                return "ok";
            }
            catch (Exception ex)
            {
                return "error";
            }
        }


        [HttpPost]

        public string Impt(IFormFile fileinput)
        {
            try
            {
                var filename = ContentDispositionHeaderValue.Parse(fileinput.ContentDisposition).FileName; // 原文件名（包括路径）
                var extName = filename.Substring(filename.LastIndexOf('.')).Replace("\"", "");// 扩展名
                string shortfilename = $"{Guid.NewGuid()}{extName}";// 新文件名
                
                string fileSavePath = _webHostEnvironment.ContentRootPath +  Path.DirectorySeparatorChar+"upload"+ Path.DirectorySeparatorChar;//文件临时目录，导入完成后 删除
                filename = fileSavePath + shortfilename; // 新文件名（包括路径）
                Console.WriteLine("fileSavePath " + fileSavePath);
                Console.WriteLine("Directory.Exists " + !Directory.Exists(fileSavePath));

                if (!Directory.Exists(fileSavePath))
                {
                    Directory.CreateDirectory(fileSavePath);
                }
                using (FileStream fs = System.IO.File.Create(filename)) // 创建新文件
                {
                    fileinput.CopyTo(fs);// 复制文件
                    fs.Flush();// 清空缓冲区数据
                    //根据 filename 【文件服务器磁盘路径】可对文件进行业务操作
                }
                //处理完成后，删除上传的文件

                //if (System.IO.File.Exists(filename))
                //{
                //    System.IO.File.Delete(filename);
                //}
                return "ok";
            }
            catch (Exception ex)
            {
                return "error";
            }
        }
        private static readonly string[] Summaries = new[]
{
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

 
        [HttpGet]
        public IEnumerable<Job> getAllCar(string strName,int iAge)
        {
            Job job1 = new Job();
            job1.strName = "啊";
            job1.strCls = "哈哈";
            job1.iAge = 1;

            Job job2 = new Job();
            job2.strName = "嗯嗯";
            job2.strCls = "呵呵";
            job2.iAge = 3;

            List<Job> list = new List<Job>();
            list.Add(job1);
            list.Add(job2);

            return list.ToArray();
             
        }
    }

    public class Job {
        public string strName { get; set; }
        public string strCls { get; set; }
        public int iAge { get; set; }
    }
}