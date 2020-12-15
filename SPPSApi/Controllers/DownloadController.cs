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

namespace SPPSApi.Controllers
{
    //下载共通类
    [Route("api/Download/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class DownloadController : ControllerBase
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
    }
}
