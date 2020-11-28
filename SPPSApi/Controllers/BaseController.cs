using Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography;

namespace SPPSApi.Controllers
{
    public class BaseController : ControllerBase
    {
        private static readonly MemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());//全局缓存
        public JsonSerializerSettings JSON_SETTING = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };//JSON返回自动忽略NULL字段
        public class ApiResult
        {
            public int code;
            public Object data;
        }

        public class ApiToken
        {
            public string token;
        }
        #region 用户登录后，新增token
        public string createToken(string strUserId)
        {
            string token = ComFunction.Encrypt(strUserId);
            using (var entry = memoryCache.CreateEntry(token))
            {
                entry.Value = token;
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(ComConstant.TIME_OUT_MINUTES));
            }
            return token;
        }
        #endregion

        #region 验证用户是否登录
        public bool isLogin(string token)
        {
            if (token == null || token.Trim() == "")
                return false;
            Object temp = null;
            memoryCache.TryGetValue(token,out temp);
            if (temp != null && temp.ToString() != "")
            {//缓存重置
                memoryCache.Remove(token);
                using (var entry = memoryCache.CreateEntry(token))
                {
                    entry.Value = token;
                    entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(ComConstant.TIME_OUT_MINUTES));
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 清除token
        public void clearToken(string tokenKey)
        {
            memoryCache.Remove(tokenKey);
        }
        #endregion

        #region 定义没登录返回信息
        public string error_login()
        {
            ApiResult apiResult = new ApiResult();
            apiResult.code = ComConstant.TIME_OUT_CODE;
            apiResult.data = "没有登录或者登陆超时";
            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        }
        #endregion
    }
}
