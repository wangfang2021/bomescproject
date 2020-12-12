using Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;

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
            public int flag;//个别画面对于返回值类型特别判断用
        }

        public class ApiToken
        {
            public string token;
        }
        #region 用户登录后，新增token
        public string createToken(LoginInfo info)
        {
            byte[] bytedata = ComFunction.objectToBytes(info);
            SymmetricAlgorithm des = Aes.Create();
            des.Key = Encoding.UTF8.GetBytes(ComConstant.SERVER_KEY);
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;
            ICryptoTransform cryptoTransform = des.CreateEncryptor();
            byte[] res = cryptoTransform.TransformFinalBlock(bytedata, 0, bytedata.Length);
            string token = Convert.ToBase64String(res);

            byte[] res22 = Convert.FromBase64String(token);




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
            memoryCache.TryGetValue(token, out temp);
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

        #region 根据token获取用户所有信息
        public LoginInfo getLoginByToken(string token)
        {
            byte[] source = Convert.FromBase64String(token);
            SymmetricAlgorithm des = Aes.Create();
            des.Key = Encoding.UTF8.GetBytes(ComConstant.SERVER_KEY);
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;
            ICryptoTransform cryptoTransform = des.CreateDecryptor();
            byte[] res = cryptoTransform.TransformFinalBlock(source, 0, source.Length);
            LoginInfo info = (LoginInfo)ComFunction.bytesToObject(res);
            return info;
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
