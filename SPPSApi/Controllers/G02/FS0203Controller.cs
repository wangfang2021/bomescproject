using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace SPPSApi.Controllers.G02
{
    [Route("api/FS0203/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0203Controller : BaseController
    {
        FS0203_Logic fs0203_logic = new FS0203_Logic();
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string FunctionID = "FS0203";

        public FS0203Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost]
        [EnableCors("any")]
        public string searchApi([FromBody]dynamic data)
        {
            //验证是否登录
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);

            //以下开始业务处理
            ApiResult apiResult = new ApiResult();
            dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
            int flag = dataForm.fileType;
            string UploadTime = dataForm.uploadDate == null ? "" : dataForm.uploadDate;
            try
            {
                DataTable dt = fs0203_logic.searchApi(flag, UploadTime);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("dOperatorTime", ConvertFieldType.DateType, "yyyy-MM-dd");

                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M02UE0301", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }

        #region 导出
        [HttpPost]
        [EnableCors("any")]
        public string exportApi([FromBody] dynamic data)
        {
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            //以下开始业务处理
            ApiResult apiResult = new ApiResult();
            dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));

            int flag = dataForm.fileType;
            string UploadTime = dataForm.uploadDate == null ? "" : dataForm.uploadDate;
            try
            {
                DataTable dt = fs0203_logic.searchApi(flag, UploadTime);
                string resMsg = "";
                string[] head = { "文件名", "上传人", "上传时间" };
                string[] fields = { "vcFileName", "vcOperatorID", "dOperatorTime" };

                string filepath = ComFunction.DataTableToExcel(head, fields, dt, _webHostEnvironment.ContentRootPath, loginInfo.UserId, FunctionID, ref resMsg);
                if (filepath == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "导出生成文件失败";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = filepath;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M02UE0302", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion


        #region SPRL导出
        [HttpPost]
        [EnableCors("any")]
        public string SPRLexportApi([FromBody] dynamic data)
        {
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            //以下开始业务处理
            ApiResult apiResult = new ApiResult();
            dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));

            string carType = dataForm.carType == null ? "" : dataForm.carType;
            try
            {
                string environment = Environment.OSVersion.ToString().ToLower();
                if (!environment.Contains("windows"))
                {
                    string realPath = ComFunction.HttpDownload(@"Doc\Export\", carType + ".xlsx", _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Export");
                    string filepath = System.IO.Path.GetFileName(realPath);
                    
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.data = filepath;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                else
                {
                    //string realPath = ComFunction.FtpDownload("TTCC/SPRL", _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Export", carType + ".xlsx");
                    ////转存下载
                    //string realPath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" +
                    //                  Path.DirectorySeparatorChar + "TTCC" + Path.DirectorySeparatorChar + "SPRL" + Path.DirectorySeparatorChar + carType + ".xlsx";

                    //string filepath = "";
                    //if (System.IO.File.Exists(realPath))
                    //{
                    //    filepath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" +
                    //               Path.DirectorySeparatorChar + "Export" + Path.DirectorySeparatorChar + "SPRL" + Path.DirectorySeparatorChar;

                    //    if (Directory.Exists(filepath))
                    //    {
                    //        ComFunction.DeleteFolder(filepath);
                    //    }

                    //    Directory.CreateDirectory(filepath);

                    //    filepath = filepath + System.IO.Path.GetFileName(realPath);
                    //    System.IO.File.Copy(realPath, filepath, true);
                    //    filepath = "SPRL" + Path.DirectorySeparatorChar + System.IO.Path.GetFileName(realPath);
                    }




                //string realPath = ComFunction.FtpDownload("TTCC/SPRL", _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Export", carType + ".xlsx");
                ////转存下载
                //string realPath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" +
                //                  Path.DirectorySeparatorChar + "TTCC" + Path.DirectorySeparatorChar + "SPRL" + Path.DirectorySeparatorChar + carType + ".xlsx";

                //string filepath = "";
                //if (System.IO.File.Exists(realPath))
                //{
                //    filepath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" +
                //               Path.DirectorySeparatorChar + "Export" + Path.DirectorySeparatorChar + "SPRL" + Path.DirectorySeparatorChar;

                //    if (Directory.Exists(filepath))
                //    {
                //        ComFunction.DeleteFolder(filepath);
                //    }

                //    Directory.CreateDirectory(filepath);

                //    filepath = filepath + System.IO.Path.GetFileName(realPath);
                //    System.IO.File.Copy(realPath, filepath, true);
                //    filepath = "SPRL" + Path.DirectorySeparatorChar + System.IO.Path.GetFileName(realPath);





                apiResult.code = ComConstant.SUCCESS_CODE;
                //apiResult.data = filepath;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M02UE0304", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion


    }
}