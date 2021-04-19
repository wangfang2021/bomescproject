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
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace SPPSApi.Controllers.G04
{
    [Route("api/FS0406_Sub/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0406Controller_Sub : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0406_Logic fs0406_Logic = new FS0406_Logic();
        private readonly string FunctionID = "FS0406";

        public FS0406Controller_Sub(IWebHostEnvironment webHostEnvironment)
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

                List<Object> dataList_C005 = ComFunction.convertAllToResult(ComFunction.getTCode("C005"));//收货方


                res.Add("C005", dataList_C005);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M00UE0006", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "初始化失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion



        #region 生成

        [HttpPost]
        [EnableCors("any")]
        public string createApi([FromBody] dynamic data)
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

            JArray dateRangeArray = dataForm.CKdateRange;
            string startTime = dateRangeArray.Count > 0 ? dateRangeArray[0].ToString().Replace("-", "") : "";
            string endTime = dateRangeArray.Count > 0 ? dateRangeArray[1].ToString().Replace("-", "") : "";
            string Receiver = dataForm.Receiver == null ? "" : dataForm.Receiver;
            bool RKCheck = dataForm.RKCheck;
            bool CKCheck = dataForm.CKCheck;
            string RKDate = dataForm.RKDate == null ? "" : dataForm.RKDate;

            try
            {
                string msg = "";
                fs0406_Logic.createInfo(Receiver, RKCheck, RKDate, CKCheck, startTime, endTime, loginInfo.UserId, ref msg);

                if (!string.IsNullOrWhiteSpace(msg))
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = msg;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "生成成功";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M04UE0606", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "生成失败。";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion 

    }
}
