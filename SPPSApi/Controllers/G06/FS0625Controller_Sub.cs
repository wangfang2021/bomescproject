using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SPPSApi.Controllers.G06
{
    [Route("api/FS0625_Sub/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0625Controller_Sub : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0625_Logic fs0625_Logic = new FS0625_Logic();
        private readonly string FunctionID = "FS0625";

        public FS0625Controller_Sub(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }


        #region 获取邮件体
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
            string date = dataForm.date == null ? "" : dataForm.date;
            string flag = dataForm.flag == null ? "" : dataForm.flag;
            string vcColor = dataForm.vcColor == null ? "" : dataForm.vcColor;

            try
            {
                String emailBody = fs0625_Logic.CreateEmailBody(date, vcColor,flag, loginInfo.UnitCode, loginInfo.UnitName);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = emailBody;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2510", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "生成邮件体失败" + ex.Message;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion


        #region FTMS

        [HttpPost]
        [EnableCors("any")]
        public string FTMSApi([FromBody]dynamic data)
        {
            //验证是否登录
            return "";
        }
        #endregion
       
    }
}