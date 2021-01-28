using Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace SPPSApi.Controllers
{
    public class BaseController : ControllerBase
    {
        private static readonly MemoryCache memoryCache_Login = new MemoryCache(new MemoryCacheOptions());//全局登录缓存
        private static readonly MemoryCache memoryCache_Search = new MemoryCache(new MemoryCacheOptions());//全局检索缓存
        public JsonSerializerSettings JSON_SETTING = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };//JSON返回自动忽略NULL字段
        public class ApiResult
        {
            public int code;
            public Object data;
            public int flag;//个别画面对于返回值类型特别判断用
            public string type;//返回值类型
            public int field1;//预留1
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




            using (var entry = memoryCache_Login.CreateEntry(token))
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
            memoryCache_Login.TryGetValue(token, out temp);
            if (temp != null && temp.ToString() != "")
            {//缓存重置
                memoryCache_Login.Remove(token);
                using (var entry = memoryCache_Login.CreateEntry(token))
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
            memoryCache_Login.Remove(tokenKey);
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


        #region 初始化检索缓存
        public void initSearchCash(string key, DataTable dt)
        {
            if (key == null || key.Trim() == "")
                return ;
            using(var entry = memoryCache_Search.CreateEntry(key))
            {
                entry.Value = dt;
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(ComConstant.TIME_OUT_MINUTES));
            }
        }
        #endregion

        #region 判断检索缓存是否存在
        public bool isExistSearchCash(string key)
        {
            if (key == null || key.Trim() == "")
                return false;
            Object temp = null;
            memoryCache_Search.TryGetValue(key, out temp);
            if (temp != null && temp.ToString() != "")
            {
                return true;
            }
            else
                return false;
        }
        #endregion

        #region 从缓存中获取检索结果
        /// <summary>
        /// 从缓存中获取检索结果
        /// </summary>
        /// <param name="key">检索key</param>
        /// <param name="page">起始页码，从0开始</param>
        /// <param name="pageSize">每页长度</param>
        /// <returns></returns>
        public DataTable getSearchResultByCash(string key,int page,int pageSize,ref int pageTotal)
        {
            if (key == null || key.Trim() == "")
                return null;
            Object temp = null;
            memoryCache_Search.TryGetValue(key, out temp);
            if (temp != null && temp.ToString() != "")
            {//缓存重置
                memoryCache_Login.Remove(key);
                using (var entry = memoryCache_Login.CreateEntry(key))
                {
                    entry.Value = temp;
                    entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(ComConstant.TIME_OUT_MINUTES));
                }
                DataTable dt = (DataTable)temp;

                int rowTotal=dt.Rows.Count;//总行数
                pageTotal = (rowTotal / pageSize) + 1;//总页数
                DataTable result = dt.Clone();
                for (int i = page*pageSize; i < (page+1) *pageSize&&i<dt.Rows.Count; i++)
                {
                    result.Rows.Add(dt.Rows[i].ItemArray);
                }
                return result;
            }
            else
            {
                return null;
            }
        }
        #endregion
    }

    public enum ERROR_FLAG
    {
        单元格定位提示 = 1,
        弹窗提示 = 2
    }
}
