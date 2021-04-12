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
using Newtonsoft.Json;

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

        #region 下载PDF文件
        [HttpGet]
        [EnableCors("any")]
        public IActionResult downloadPDFApi(string path)
        {
            try
            {
                string fileSavePath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "PDF" + Path.DirectorySeparatorChar;//文件临时目录，导入完成后 删除
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

        #region 下载ZIP文件
        [HttpGet]
        [EnableCors("any")]
        public IActionResult downloadTagZIPApi(string path)
        {
            try
            {
                string fileSavePath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "TagZIP" + Path.DirectorySeparatorChar;//文件临时目录，导入完成后 删除
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

        [HttpGet]
        [EnableCors("any")]
        public IActionResult getImageList(string path)
        {
            try
            {
                string fileSavePath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Image" + Path.DirectorySeparatorChar + "PackingOper" + Path.DirectorySeparatorChar;
                var provider = new FileExtensionContentTypeProvider();
                FileInfo fileInfo = new FileInfo(fileSavePath + path);
                var ext = fileInfo.Extension;
                new FileExtensionContentTypeProvider().Mappings.TryGetValue(ext, out var contenttype);
                byte[] bt = System.IO.File.ReadAllBytes(fileSavePath + path);
                return File(bt, contenttype ?? "image/Jpeg", fileInfo.Name);
            }
            catch (Exception ex)
            {
                ContentResult result = new ContentResult();
                result.Content = "<script>alert('导出失败,没有找到要导出的图片！')</script>";
                result.ContentType = "text/html;charset=utf-8";
                ComMessage.GetInstance().ProcessMessage("getImage", "M00UE0008", ex, "system");
                return result;
            }
        }
        [HttpPost]
        public string uploadImagetospisApi(IFormFile file)
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                string token = Request.Form["token"].ToString();
                string hashCode = Request.Form["hashCode"].ToString();
                if (!isLogin(token))
                {
                    return "error";
                }
                LoginInfo loginInfo = getLoginByToken(token);
                //以下开始业务处理
                var filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Replace("\"", ""); // 原文件名（包括路径）
                var extName = filename.Substring(filename.LastIndexOf('.') + 1).Replace("\"", "");// 扩展名
                string ImageType = ".jpg,.png,.gif,.bmp,.jpeg";
                //判断上传格式是否合法
                if (ImageType.IndexOf(extName.ToLower()) <= 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "图片格式必须是jpg|png|gif|bmp|jpeg,请确认上传图片格式!";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string oldFileName = filename;

                // 获取到要保存文件的名称 
                String newFileName = getUUIDName(oldFileName);

                //获取到当前项目下products/3下的真实路径
                //D:\tomcat\tomcat71_sz07\webapps\store_v5\products\3
                String realPath = string.Empty;
                if (Directory.Exists(ComConstant.strImagePath))
                {
                    realPath = ComConstant.strImagePath;
                }
                else
                {
                    realPath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "upload_spis" + Path.DirectorySeparatorChar + "spis" + Path.DirectorySeparatorChar;
                }
                String path = realPath; 
                string fileSavePath = path;
                if (!Directory.Exists(fileSavePath))
                {
                    Directory.CreateDirectory(fileSavePath);
                }

                DirectoryInfo theFolder = new DirectoryInfo(fileSavePath);

                filename = fileSavePath + newFileName; // 新文件名（包括路径）
                using (FileStream fs = System.IO.File.Create(filename)) // 创建新文件
                {
                    file.CopyTo(fs);// 复制文件
                    fs.Flush();// 清空缓冲区数据
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = newFileName;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "图片上传失败!";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        [HttpPost]
        public string uploadsharpcompressApi(IFormFile file)
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                //deleteOldDir();//删除过期文件夹
                string token = Request.Form["token"].ToString();
                string hashCode = Request.Form["hashCode"].ToString();
                if (!isLogin(token))
                {
                    return "error";
                }
                LoginInfo loginInfo = getLoginByToken(token);
                //以下开始业务处理
                var filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Replace("\"", ""); // 原文件名（包括路径）
                var extName = filename.Substring(filename.LastIndexOf('.') + 1).Replace("\"", "");// 扩展名
                //string ImageType = ".rar,.zip,.7z";
                ////判断上传格式是否合法
                //if (ImageType.IndexOf(extName.ToLower()) <= 0)
                //{
                //    apiResult.code = ComConstant.ERROR_CODE;
                //    apiResult.data = "格式必须是rar|zip|7z,请确认上传文件格式!";
                //    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                //}

                string shortfilename = filename;

                string fileSavePath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "upload" + Path.DirectorySeparatorChar + hashCode + Path.DirectorySeparatorChar;
                if (!Directory.Exists(fileSavePath))
                {
                    Directory.CreateDirectory(fileSavePath);
                }



                DirectoryInfo theFolder = new DirectoryInfo(fileSavePath);


                filename = fileSavePath + theFolder.GetFiles().Length + "_" + shortfilename; // 新文件名（包括路径）
                using (FileStream fs = System.IO.File.Create(filename)) // 创建新文件
                {
                    file.CopyTo(fs);// 复制文件
                    fs.Flush();// 清空缓冲区数据
                }
                return shortfilename;
            }
            catch (Exception ex)
            {
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "文件上传失败!";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        [HttpPost]
        public string uploadImagetoapplyspisApi(IFormFile file)
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                string token = Request.Form["token"].ToString();
                string hashCode = Request.Form["hashCode"].ToString();
                if (!isLogin(token))
                {
                    return "error";
                }
                LoginInfo loginInfo = getLoginByToken(token);
                //以下开始业务处理
                var filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Replace("\"", ""); // 原文件名（包括路径）
                var extName = filename.Substring(filename.LastIndexOf('.') + 1).Replace("\"", "");// 扩展名
                string ImageType = ".jpg,.png,.gif,.bmp,.jpeg";
                //判断上传格式是否合法
                if (ImageType.IndexOf(extName.ToLower()) <= 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "图片格式必须是jpg|png|gif|bmp|jpeg,请确认上传图片格式!";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string oldFileName = filename;

                // 获取到要保存文件的名称 
                String newFileName = getUUIDName(oldFileName);

                //获取到当前项目下products/3下的真实路径
                //D:\tomcat\tomcat71_sz07\webapps\store_v5\products\3
                String realPath = string.Empty;
                if (Directory.Exists(ComConstant.strImagePath))
                {
                    realPath = ComConstant.strImagePath;
                }
                else
                {
                    realPath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "upload_spisapply" + Path.DirectorySeparatorChar + "apply" + Path.DirectorySeparatorChar;
                }
                String path = realPath;
                string fileSavePath = path;
                if (!Directory.Exists(fileSavePath))
                {
                    Directory.CreateDirectory(fileSavePath);
                }

                DirectoryInfo theFolder = new DirectoryInfo(fileSavePath);

                filename = fileSavePath + newFileName; // 新文件名（包括路径）
                using (FileStream fs = System.IO.File.Create(filename)) // 创建新文件
                {
                    file.CopyTo(fs);// 复制文件
                    fs.Flush();// 清空缓冲区数据
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = newFileName;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "图片上传失败!";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        [HttpGet]
        [EnableCors("any")]
        public IActionResult getImagetospisList(string path)
        {
            try
            {
                string fileSavePath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Image" + Path.DirectorySeparatorChar + "SPISImage" + Path.DirectorySeparatorChar;
                var provider = new FileExtensionContentTypeProvider();
                FileInfo fileInfo = new FileInfo(fileSavePath + path);
                var ext = fileInfo.Extension;
                new FileExtensionContentTypeProvider().Mappings.TryGetValue(ext, out var contenttype);
                byte[] bt = System.IO.File.ReadAllBytes(fileSavePath + path);
                return File(bt, contenttype ?? "image/Jpeg", fileInfo.Name);
            }
            catch (Exception ex)
            {
                ContentResult result = new ContentResult();
                result.Content = "<script>alert('导出失败,没有找到要导出的图片！')</script>";
                result.ContentType = "text/html;charset=utf-8";
                ComMessage.GetInstance().ProcessMessage("getImage", "M00UE0008", ex, "system");
                return result;
            }
        }
        [HttpGet]
        [EnableCors("any")]
        public IActionResult getImagetopackList(string path)
        {
            try
            {
                string fileSavePath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Image" + Path.DirectorySeparatorChar + "PackingOper" + Path.DirectorySeparatorChar;
                var provider = new FileExtensionContentTypeProvider();
                FileInfo fileInfo = new FileInfo(fileSavePath + path);
                var ext = fileInfo.Extension;
                new FileExtensionContentTypeProvider().Mappings.TryGetValue(ext, out var contenttype);
                byte[] bt = System.IO.File.ReadAllBytes(fileSavePath + path);
                return File(bt, contenttype ?? "image/Jpeg", fileInfo.Name);
            }
            catch (Exception ex)
            {
                ContentResult result = new ContentResult();
                result.Content = "<script>alert('导出失败,没有找到要导出的图片！')</script>";
                result.ContentType = "text/html;charset=utf-8";
                ComMessage.GetInstance().ProcessMessage("getImage", "M00UE0008", ex, "system");
                return result;
            }
        }
        [HttpGet]
        [EnableCors("any")]
        public IActionResult getImagetopicList(string path)
        {
            try
            {
                string fileSavePath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Image" + Path.DirectorySeparatorChar + "SPISPic" + Path.DirectorySeparatorChar;
                var provider = new FileExtensionContentTypeProvider();
                FileInfo fileInfo = new FileInfo(fileSavePath + path);
                var ext = fileInfo.Extension;
                new FileExtensionContentTypeProvider().Mappings.TryGetValue(ext, out var contenttype);
                byte[] bt = System.IO.File.ReadAllBytes(fileSavePath + path);
                return File(bt, contenttype ?? "image/Jpeg", fileInfo.Name);
            }
            catch (Exception ex)
            {
                ContentResult result = new ContentResult();
                result.Content = "<script>alert('导出失败,没有找到要导出的图片！')</script>";
                result.ContentType = "text/html;charset=utf-8";
                ComMessage.GetInstance().ProcessMessage("getImage", "M00UE0008", ex, "system");
                return result;
            }
        }
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

                string fileSavePath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "upload" + Path.DirectorySeparatorChar + hashCode + Path.DirectorySeparatorChar;
                if (!Directory.Exists(fileSavePath))
                {
                    Directory.CreateDirectory(fileSavePath);
                }



                DirectoryInfo theFolder = new DirectoryInfo(fileSavePath);


                filename = fileSavePath + theFolder.GetFiles().Length + "_" + shortfilename; // 新文件名（包括路径）
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
                string strTime = strName.Substring(0, strName.IndexOf('_'));
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

        #region
        [HttpPost]
        public string uploadImageApi(IFormFile file)
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                string token = Request.Form["token"].ToString();
                string hashCode = Request.Form["hashCode"].ToString();
                if (!isLogin(token))
                {
                    return "error";
                }
                LoginInfo loginInfo = getLoginByToken(token);
                //以下开始业务处理
                var filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Replace("\"", ""); // 原文件名（包括路径）
                var extName = filename.Substring(filename.LastIndexOf('.') + 1).Replace("\"", "");// 扩展名
                string ImageType = ".jpg,.png,.gif,.bmp,.jpeg";
                //判断上传格式是否合法
                if (ImageType.IndexOf(extName.ToLower()) <= 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "图片格式必须是jpg|png|gif|bmp|jpeg,请确认上传图片格式!";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string oldFileName = filename;

                // 获取到要保存文件的名称 
                String newFileName = getUUIDName(oldFileName);

                //获取到当前项目下products/3下的真实路径
                //D:\tomcat\tomcat71_sz07\webapps\store_v5\products\3
                String realPath = string.Empty;
                if (Directory.Exists(ComConstant.strImagePath))
                {
                    realPath = ComConstant.strImagePath;
                }
                else
                {
                    realPath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar+ "Doc" + Path.DirectorySeparatorChar+ "Image" + Path.DirectorySeparatorChar + "HeZiImages";
                }
                String dir = getDir(newFileName); // /f/e/d/c/4/9/8/4
                String path = realPath + dir; //D:\\products\3/f/e/d/c/4/9/8/4
                string fileSavePath = path;
                //string fileSavePath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "upload" + Path.DirectorySeparatorChar + hashCode + Path.DirectorySeparatorChar;
                if (!Directory.Exists(fileSavePath))
                {
                    Directory.CreateDirectory(fileSavePath);
                }

                DirectoryInfo theFolder = new DirectoryInfo(fileSavePath);

                filename = fileSavePath + newFileName; // 新文件名（包括路径）
                using (FileStream fs = System.IO.File.Create(filename)) // 创建新文件
                {
                    file.CopyTo(fs);// 复制文件
                    fs.Flush();// 清空缓冲区数据
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dir + newFileName;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "图片上传失败!";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /**
	    * 获取随机名称
	    * @param realName 真实名称
	    * @return uuid
	    */
        public static String getUUIDName(String realName)
        {
            //realname  可能是  1.jpg   也可能是  1
            //获取后缀名
            int index = realName.LastIndexOf(".");
            if (index == -1)
            {
                return Guid.NewGuid().ToString().Replace("-", "").ToUpper();
            }
            else
            {
                return Guid.NewGuid().ToString().Replace("-", "").ToUpper() + realName.Substring(index);
            }
        }
        /**
     * 获取文件目录
     * @param name 文件名称
     * @return 目录
     */
        public static String getDir(String name)
        {
            //任意一个对象都有一个hash码   131313213
            int i = name.GetHashCode();
            //将hash码转成16禁止的字符串
            //String hex = Integer.toHexString(i);
            string hex = String.Format("{0:X}", i);
            int j = hex.Length;
            for (int k = 0; k < 8 - j; k++)
            {
                hex = "0" + hex;
            }
            return Path.DirectorySeparatorChar + hex.Substring(0, 1) + Path.DirectorySeparatorChar + hex.Substring(1, 1) + Path.DirectorySeparatorChar + hex.Substring(2, 1) + Path.DirectorySeparatorChar + hex.Substring(3, 1) + Path.DirectorySeparatorChar + hex.Substring(4, 1) + Path.DirectorySeparatorChar + hex.Substring(5, 1) + Path.DirectorySeparatorChar + hex.Substring(6, 1) + Path.DirectorySeparatorChar + hex.Substring(7, 1);
        }
        #endregion

        #region 下载NQC错误信息文件
        [HttpGet]
        [EnableCors("any")]
        public IActionResult downloadNQCErrMsgApi(string path)
        {
            try
            {
                string fileSavePath = ComConstant.strNQCErrMsgPath;
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

        #region 下载Image
        [HttpGet]
        [EnableCors("any")]
        public IActionResult getImageApi(string path)
        {
            try
            {
                string fileSavePath =  _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar +"Doc" + Path.DirectorySeparatorChar + "Image" + Path.DirectorySeparatorChar + "HeZiImages";//文件临时目录，导入完成后 删除
                var provider = new FileExtensionContentTypeProvider();
                FileInfo fileInfo = new FileInfo(fileSavePath + path);
                var ext = fileInfo.Extension;
                new FileExtensionContentTypeProvider().Mappings.TryGetValue(ext, out var contenttype);
                byte[] bt = System.IO.File.ReadAllBytes(fileSavePath + path);
                //if (fileInfo.Exists)
                //    fileInfo.Delete();
                return File(bt, contenttype ?? "image/Jpeg", fileInfo.Name);
            }
            catch (Exception ex)
            {
                ContentResult result = new ContentResult();
                result.Content = "<script>alert('导出失败,没有找到要导出的图片！')</script>";
                result.ContentType = "text/html;charset=utf-8";
                ComMessage.GetInstance().ProcessMessage("getImage", "M00UE0008", ex, "system");
                return result;
            }
        }
        #endregion

        #region 上传订单API
        [HttpPost]
        public string uploadOrderApi(IFormFile file)
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                string token = Request.Form["token"].ToString();
                string hashCode = Request.Form["hashCode"].ToString();
                if (!isLogin(token))
                {
                    return "error";
                }
                LoginInfo loginInfo = getLoginByToken(token);
                //以下开始业务处理
                var filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Replace("\"", ""); // 原文件名（包括路径）
                var lastFileName = filename;
                var extName = filename.Substring(filename.LastIndexOf('.') + 1).Replace("\"", "");// 扩展名
                string ImageType = ".txt";
                //判断上传格式是否合法
                if (ImageType.IndexOf(extName.ToLower()) <= 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "上传文件必须是txt文件,请确认上传文件格式!";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string oldFileName = filename;

                // 获取到要保存文件的名称 
                String newFileName = getUUIDName(oldFileName);

                //获取到当前项目下products/3下的真实路径
                //D:\tomcat\tomcat71_sz07\webapps\store_v5\products\3
                String realPath = string.Empty;
                if (Directory.Exists(ComConstant.strOrderPath))
                {
                    realPath = ComConstant.strOrderPath;
                }
                else
                {
                    realPath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "orders";
                }
                String dir = getDir(newFileName); // /f/e/d/c/4/9/8/4
                String path = realPath + dir; //D:\\products\3/f/e/d/c/4/9/8/4
                string fileSavePath = path;
                //string fileSavePath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "upload" + Path.DirectorySeparatorChar + hashCode + Path.DirectorySeparatorChar;
                if (!Directory.Exists(fileSavePath))
                {
                    Directory.CreateDirectory(fileSavePath);
                }

                DirectoryInfo theFolder = new DirectoryInfo(fileSavePath);

                filename = fileSavePath + newFileName; // 新文件名（包括路径）
                using (FileStream fs = System.IO.File.Create(filename)) // 创建新文件
                {
                    file.CopyTo(fs);// 复制文件
                    fs.Flush();// 清空缓冲区数据
                }
                Dictionary<string, object> res = new Dictionary<string, object>();
                res.Add("fileName", lastFileName);
                res.Add("filePath", dir + newFileName);
                
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "文件上传失败!";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region DMZ上传文件
        [HttpPost]
        [RequestSizeLimit(100_000_000)]
        public string uploadDMZApi(IFormFile file, string name, string dir)
        {
    //        调用例子
    //        ComFunction.HttpUploadFile(_webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar
    //+ "Doc" + Path.DirectorySeparatorChar
    //+ "Export" + Path.DirectorySeparatorChar
    //+ "02.切替文件-0412.rar"
    //, "02.切替文件-0412.rar"
    //, "Doc\\Export\\");

            ApiResult apiResult = new ApiResult();
            try
            {
                string filename = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + dir + name; // 新文件名（包括路径）
                FileInfo fileInfo = new FileInfo(filename);
                if (fileInfo.Exists)
                    fileInfo.Delete();
                using (FileStream fs = System.IO.File.Create(filename)) // 创建新文件
                {
                    file.CopyTo(fs);// 复制文件
                    fs.Flush();// 清空缓冲区数据
                }
                return "";
            }
            catch (Exception ex)
            {
                Console.WriteLine("文件上传失败! " + ex.Message);
                return "error";
            }
        }
        #endregion

        #region DMZ下载API
        public IActionResult downloadDMZApi(string name, string dir)
        {
            //调用例子
            //ComFunction.HttpDownload("Doc\\Export\\",
            //   "02.切替文件-0412.rar",
            //   _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar
            //   + "Doc" + Path.DirectorySeparatorChar
            //   + "PDF" + Path.DirectorySeparatorChar);
            try
            {
                string fileSavePath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar ;//文件临时目录，导入完成后 删除
                var provider = new FileExtensionContentTypeProvider();
                FileInfo fileInfo = new FileInfo(fileSavePath + dir+ name);
                var ext = fileInfo.Extension;
                new FileExtensionContentTypeProvider().Mappings.TryGetValue(ext, out var contenttype);
                byte[] bt = System.IO.File.ReadAllBytes(fileSavePath + dir + name);
                return File(bt, contenttype ?? "application/octet-stream", fileInfo.Name);
            }
            catch (Exception ex)
            {
                Console.WriteLine("文件下载失败! " + ex.Message);
                ContentResult result = new ContentResult();
                result.Content = "error";
                result.ContentType = "text/html;charset=utf-8";
                Console.WriteLine("文件下载失败! " + ex.Message);
                return result;
            }
        }
        #endregion
    }
}
