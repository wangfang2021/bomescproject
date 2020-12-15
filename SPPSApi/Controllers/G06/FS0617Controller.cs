using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
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

namespace SPPSApi.Controllers.G06
{
    [Route("api/FS0617/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0617Controller : BaseController
    {
        FS0617_Logic fS0617_Logic = new FS0617_Logic();
        private readonly string FunctionID = "FS0617";

        private readonly IWebHostEnvironment _webHostEnvironment;
        public FS0617Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        /// <summary>
        /// 绑定工区信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string bindplant()
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
                DataTable dt = fS0617_Logic.getPlantInfo();
                List<Object> dataList = ComFunction.convertAllToResult(dt);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0204", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "绑定工区失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 绑定车种信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string bindcartype()
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
                DataTable dt = fS0617_Logic.getCarTypeInfo();
                List<Object> dataList = ComFunction.convertAllToResult(dt);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0204", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "绑定车种失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 绑定收货方信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string bindreparty()
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
                DataTable dt = fS0617_Logic.getRePartyInfo();
                List<Object> dataList = ComFunction.convertAllToResult(dt);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0204", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "绑定收货方失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 绑定供应商信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string bindsuparty()
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
                DataTable dt = fS0617_Logic.getSuPartyInfo();
                List<Object> dataList = ComFunction.convertAllToResult(dt);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0204", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "绑定供应商失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 查询方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string search_api([FromBody]dynamic data)
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
            string strplant = dataForm.plant == null ? "" : dataForm.plant;
            string strpartid = dataForm.partid == null ? "" : dataForm.partid;
            string strcartype = dataForm.cartype == null ? "" : dataForm.cartype;
            string strreparty = dataForm.reparty == null ? "" : dataForm.reparty;
            string strsuparty = dataForm.suparty == null ? "" : dataForm.suparty;
            try
            {
                DataTable dt = fS0617_Logic.getSearchInfo(strplant, strpartid, strcartype, strreparty, strsuparty);
                List<Object> dataList = ComFunction.convertAllToResult(dt);
                for (int i = 0; i < dataList.Count; i++)
                {
                    Dictionary<string, object> row = (Dictionary<string, object>)dataList[i];
                    row["eableflag"] = row["eableflag"].ToString() == "1" ? true : false;
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0204", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 印刷方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string print_api([FromBody]dynamic data)
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
            string strplant = dataForm.plant == null ? "" : dataForm.plant;
            string strpartid = dataForm.partid == null ? "" : dataForm.partid;
            string strcartype = dataForm.cartype == null ? "" : dataForm.cartype;
            string strreparty = dataForm.reparty == null ? "" : dataForm.reparty;
            string strsuparty = dataForm.suparty == null ? "" : dataForm.suparty;
            try
            {
                string strFilesPath = "file:///E:/20200305%E6%A1%8C%E9%9D%A2/9.%E7%89%A9%E6%B5%81%E7%89%A9%E8%B5%84%E7%AE%A1%E7%90%86%E7%B3%BB%E7%BB%9F.pdf";
                //string strFilesPath = fS0617_Logic.getPrintFile(strplant, strpartid, strcartype, strreparty, strsuparty);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = strFilesPath;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0204", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "生成印刷文件失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
    }
}
