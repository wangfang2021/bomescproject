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



namespace SPPSApi.Controllers.G08
{
    [Route("api/FS0813/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0813Controller : BaseController
    {
        FS0813_Logic fs0813_Logic = new FS0813_Logic();
        private readonly string FunctionID = "FS0813";

        private readonly IWebHostEnvironment _webHostEnvironment;

        public FS0813Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        #region 检索
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
            string strSellNo = dataForm.vcSellNo == null ? "" : dataForm.vcSellNo;
            string strStartTime = dataForm.StartTime == null ? "" : dataForm.StartTime;
            string strEndTime= dataForm.EndTime == null ? "" : dataForm.EndTime;

            try
            {
                DataTable dt = fs0813_Logic.Search(strSellNo, strStartTime, strEndTime);

                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("dOperatorTime", ConvertFieldType.DateType, "yyyy-MM-dd HH:mm");

                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);
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
        #endregion

        #region 子画面初始化
        [HttpPost]
        [EnableCors("any")]
        public string initSubApi([FromBody]dynamic data)
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
            string strSellNo = dataForm.vcSellNo == null ? "" : dataForm.vcSellNo;

            try
            {
                DataTable dt = fs0813_Logic.initSubApi(strSellNo);

                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("dOperatorTime", ConvertFieldType.DateType, "yyyy-MM-dd HH:mm");

                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0204", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "子页面初始化失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

    }
}
