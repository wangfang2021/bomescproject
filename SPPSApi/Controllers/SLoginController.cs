using System;
using System.Collections.Generic;
using System.Data;
using Common;
using DataEntity;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace SPPSApi.Controllers
{
    [Route("api/login/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class SLoginController : BaseController
    {
        SLogin_Logic slogin_Logic = new SLogin_Logic();
        private readonly string FunctionID = "Default";
        #region 登录验证
        [HttpPost]
        public string Login([FromBody]dynamic data)
        {
            //以下开始业务处理2222
            ApiResult apiResult = new ApiResult();
            dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));

            string strUserId = dataForm.username == null ? "" : dataForm.username;
            string strPassword = dataForm.password == null ? "" : dataForm.password;
            Console.WriteLine("用户登录:" + strUserId);
            string strPassWord = ComFunction.encodePwd(strPassword.Trim());
            try
            {
                LoginInfo info = slogin_Logic.LonginState(strUserId, strPassWord);
                if (info != null)
                {
                    string strTokenValue = createToken(info);
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    ApiToken token = new ApiToken();
                    token.token = strTokenValue;
                    apiResult.data = token;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "用户名或密码错误";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M00UE0008", ex, strUserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "登录失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 获取用户信息
        [HttpPost]
        public string getInfoApi()
        {
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            //以下开始业务处理
            ApiResult apiResult = new ApiResult();
            string strUserId = "";
            try
            {
                LoginInfo loginInfo = getLoginByToken(strToken);
                if (loginInfo != null)
                {
                    strUserId = loginInfo.UserId;
                    Dictionary<string, object> res = new Dictionary<string, object>();
                    res.Add("UserId", loginInfo.UserId);
                    res.Add("UserName", loginInfo.UserName);
                    res.Add("UnitCode", loginInfo.UnitCode);
                    res.Add("UnitName", loginInfo.UnitName);
                    res.Add("PlantCode", loginInfo.PlantCode);
                    res.Add("Email", loginInfo.Email);
                    res.Add("IsAdmin", loginInfo.IsAdmin);
                    res.Add("Ip", loginInfo.Ip);
                    res.Add("IsTest", loginInfo.IsTest);
                    res.Add("Special", loginInfo.Special);
                    res.Add("PlatForm", loginInfo.PlatForm);
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.data = res;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                else
                {//这块不走数据库，除了用户改加密串导致解析失败，要不不可能获取不到数据
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "没有个人信息";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M00UE0008", ex, strUserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "获取个人信息失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 获取路由
        [HttpPost]
        public string getRoutes()
        {
            //验证是否登录
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            //以下开始业务处理
            LoginInfo loginInfo = getLoginByToken(strToken);

            ApiResult apiResult = new ApiResult();
            try
            {
                List<SLogin_DataEntity.Node> list = slogin_Logic.GetRouter(loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = list;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M00UE0008", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "获取路由失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion


        #region 用户退出，清除token
        [HttpPost]
        public string logOutApi()
        {
            //验证是否登录
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            clearToken(strToken);
            ApiResult apiResult = new ApiResult();
            apiResult.code = ComConstant.SUCCESS_CODE;
            apiResult.data = "success";
            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        }
        #endregion



    }
}