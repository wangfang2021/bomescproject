using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SPPSApi.Controllers.G00
{
    [Route("api/FS0100/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0100Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0100_Logic fs0100_Logic = new FS0100_Logic();
        private readonly string FunctionID = "FS0100";

        public FS0100Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        #region 页面初始化
        [HttpPost]
        [EnableCors("any")]
        public string pageloadApi()
        {
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            //以下开始业务处理
            ApiResult apiResult = new ApiResult();
            try
            {
                Dictionary<string, object> res = new Dictionary<string, object>();

                string strUserID = loginInfo.UserId;
                string strUserName = loginInfo.UserName;

                res.Add("strUserID", strUserID);
                res.Add("strUserName", strUserName);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M99UE0501", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "初始化失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 保存
        [HttpPost]
        [EnableCors("any")]
        public string saveApi([FromBody] dynamic data)
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
            try
            {
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));

                string strUserID = dataForm.strUserID;
                string strUserName = dataForm.strUserName;
                string strOldPwd = dataForm.strOldPwd;
                string strNewPwd = dataForm.strNewPwd;
                string strNewPwd2 = dataForm.strNewPwd2;

                if (strOldPwd.Trim() == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请输入旧密码";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (strNewPwd.Trim() == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请输入新密码";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (strNewPwd.Trim().Length < 8)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "新密码不能小于8位";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (strNewPwd2.Trim() == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请再输入一遍新密码";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (strNewPwd.Trim() != strNewPwd2.Trim())
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "新密码两次输入不一致";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                
                if (!CheckSecurity(strNewPwd2.Trim()))
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "创建密码强度过低,新密码最少长度8位，包括至少1个大写字母，1个小写字母，1个数字";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (fs0100_Logic.checkPwd(loginInfo.UserId, ComFunction.encodePwd(strOldPwd.Trim())))//判断密码是否正确
                {//正确

                    fs0100_Logic.changePwd(loginInfo.UserId, ComFunction.encodePwd(strNewPwd2.Trim()));

                    ComMessage.GetInstance().ProcessMessage(FunctionID, "M00UI1001",null, loginInfo.UserId);
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.data = null;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                else
                {//不正确 
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "老密码输入不正确，请修改！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M00UE1001", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "修改失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion




        #region 验证密码强度

        #endregion
        private bool CheckSecurity(string pwd)
        {
            string pattern = @"(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9]).{8,}";
            bool isMatch = Regex.IsMatch(pwd, pattern);
            return isMatch;
        }
    }
}
