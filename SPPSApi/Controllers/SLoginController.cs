using System;
using System.Collections.Generic;
using System.Data;
using Common;
using DataEntity;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace WebApplication5.Controllers
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
                DataTable dt = slogin_Logic.LonginState(strUserId, strPassWord);
                if (dt != null && dt.Rows.Count > 0)
                {
                    string strTokenValue = createToken(strUserId);
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
        [HttpGet]
        public string getInfoApi(string token)
        {
            //以下开始业务处理
            ApiResult apiResult = new ApiResult();
            string strUserId = ComFunction.Decrypt(token);
            try
            {
                DataTable dt = slogin_Logic.getUserInfo(strUserId);
                if (dt != null && dt.Rows.Count > 0)
                {
                    Dictionary<string, object> res = new Dictionary<string, object>();
                    res.Add("name", dt.Rows[0]["vcUserName"].ToString());
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.data = res;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                else
                {
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
        [HttpGet]
        public string getRoutes(string token)
        {
            //验证是否登录
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            //以下开始业务处理
            string strUserId = ComFunction.Decrypt(strToken);

            ApiResult apiResult = new ApiResult();
            try
            {
                List<SLogin_DataEntity.Node> list = slogin_Logic.GetRouter(strUserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = list;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M00UE0008", ex, strUserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "获取路由失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion


        #region 用户退出，清除token
        [HttpGet]
        public string logOutApi(string token)
        {
            clearToken(token);
            ApiResult apiResult = new ApiResult();
            apiResult.code = ComConstant.SUCCESS_CODE;
            apiResult.data = "success";
            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        }
        #endregion



    }
}