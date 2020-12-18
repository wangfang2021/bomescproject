using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;

namespace SPPSApi.Controllers
{
    //下载共通类
    [Route("api/Download/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class DownloadController : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public DownloadController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        #region 下载-Doc/Export,下载后会自动删除文件
        [HttpGet]
        [EnableCors("any")]
        public IActionResult downloadApi(string path)
        {
            try
            {
                string fileSavePath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Export" + Path.DirectorySeparatorChar;//文件临时目录，导入完成后 删除
                var provider = new FileExtensionContentTypeProvider();
                FileInfo fileInfo = new FileInfo(fileSavePath + path);
                var ext = fileInfo.Extension;
                new FileExtensionContentTypeProvider().Mappings.TryGetValue(ext, out var contenttype);
                byte[] bt = System.IO.File.ReadAllBytes(fileSavePath + path);
                if (fileInfo.Exists)
                    fileInfo.Delete();
                return File(bt, contenttype ?? "application/octet-stream", fileInfo.Name);
            }
            catch (Exception ex)
            {
                ContentResult result = new ContentResult();
                result.Content = "<script>alert('导出失败,没有找到要导出的文件！')</script>";
                result.ContentType = "text/html;charset=utf-8";
                ComMessage.GetInstance().ProcessMessage("download", "M00UE0007", ex, "system");
                return result;
            }
        }
        #endregion

        #region 下载-Doc/Export,下载后会自动删除文件
        [HttpGet]
        [EnableCors("any")]
        public IActionResult downloadTplApi(string path)
        {
            try
            {
                string fileSavePath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Template" + Path.DirectorySeparatorChar;//模板目录，读取模板供用户下载
                var provider = new FileExtensionContentTypeProvider();
                FileInfo fileInfo = new FileInfo(fileSavePath + path);
                var ext = fileInfo.Extension;
                new FileExtensionContentTypeProvider().Mappings.TryGetValue(ext, out var contenttype);
                byte[] bt = System.IO.File.ReadAllBytes(fileSavePath + path);
                return File(bt, contenttype ?? "application/octet-stream", fileInfo.Name);
            }
            catch (Exception ex)
            {
                ContentResult result = new ContentResult();
                result.Content = "<script>alert('导出失败,没有找到要导出的文件！')</script>";
                result.ContentType = "text/html;charset=utf-8";
                ComMessage.GetInstance().ProcessMessage("download", "M00UE0007", ex, "system");
                return result;
            }
        }
        #endregion


        #region 导入
        [HttpPost]
        public string importApi(IFormFile file)
        {
            try
            {
                deleteOldDir();//删除过期文件夹
                string token = Request.Form["token"].ToString();
                string hashCode = Request.Form["hashCode"].ToString();
                if (!isLogin(token))
                {
                    return "error";
                }
                LoginInfo loginInfo = getLoginByToken(token);
                //以下开始业务处理

                var filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Replace("\"", ""); // 原文件名（包括路径）
                var extName = filename.Substring(filename.LastIndexOf('.')).Replace("\"", "");// 扩展名
                //string shortfilename = $"{Guid.NewGuid()}{extName}";// 新文件名
                string shortfilename = filename;

                string fileSavePath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "upload" + Path.DirectorySeparatorChar+ hashCode + Path.DirectorySeparatorChar;
                if (!Directory.Exists(fileSavePath))
                {
                    Directory.CreateDirectory(fileSavePath);
                }


                 
                DirectoryInfo theFolder = new DirectoryInfo(fileSavePath);
 

                filename = fileSavePath + theFolder.GetFiles().Length+"_" + shortfilename; // 新文件名（包括路径）
                using (FileStream fs = System.IO.File.Create(filename)) // 创建新文件
                {
                    file.CopyTo(fs);// 复制文件
                    fs.Flush();// 清空缓冲区数据
                }
                return shortfilename;
            }
            catch (Exception ex)
            {
                return "error";
            }
        }
        #endregion


        #region 删除过期文件夹
        public void deleteOldDir()
        {
            string fileSavePath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "upload" + Path.DirectorySeparatorChar;
            if (!Directory.Exists(fileSavePath))
            {
                Directory.CreateDirectory(fileSavePath);
            }
            DirectoryInfo theFolder = new DirectoryInfo(fileSavePath);
            List<string> dellist = new List<string>(); ;
            foreach (DirectoryInfo info in theFolder.GetDirectories())
            {
                string strName = info.Name;
                if (strName.IndexOf('_') == -1)
                    continue;
                string strTime =  strName.Substring(0, strName.IndexOf('_'));
                DateTime time = DateTime.Parse(strTime);
                TimeSpan span = DateTime.Now - time;
                if (span.Days > 1)
                { //超过1天的冗余文件删除
                    dellist.Add(info.FullName);
                }
            }
            while (dellist.Count > 0)
            {
                ComFunction.DeleteFolder(dellist[0]);
                dellist.RemoveAt(0);
            }
        }
        #endregion
    }
}
