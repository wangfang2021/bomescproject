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
using Microsoft.VisualBasic;
using Newtonsoft.Json;

namespace SPPSApi.Controllers
{
    //下载共通类
    [Route("api/Time/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class TimeController : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public TimeController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost]
        [EnableCors("any")]
        public string TargetMonthApi()
        {
            ////验证是否登录
            //string strToken = Request.Headers["X-Token"];
            //if (!isLogin(strToken))
            //{
            //    return error_login();
            //}
            //LoginInfo loginInfo = getLoginByToken(strToken);

            //以下开始业务处理
            ApiResult apiResult = new ApiResult();


            try
            {
                string targetTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).ToString("yyyy/MM");
                targetTime = targetTime.Replace('-', '/');
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = targetTime;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "时间获取失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }


    }
}
