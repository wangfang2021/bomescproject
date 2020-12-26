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
    [Route("api/FS0814/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0814Controller : BaseController
    {
        FS0814_Logic fs0814_Logic = new FS0814_Logic();
        private readonly string FunctionID = "FS0814";

        private readonly IWebHostEnvironment _webHostEnvironment;

        public FS0814Controller(IWebHostEnvironment webHostEnvironment)
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
            string strYearMonth = dataForm.vcYearMonth == null ? "" : dataForm.vcYearMonth;

            try
            {
                DataTable dt = fs0814_Logic.Search(strYearMonth);
                List<Object> dataList = ComFunction.convertAllToResult(dt);
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

        #region 导出
        [HttpPost]
        [EnableCors("any")]
        public string exportApi([FromBody] dynamic data)
        {
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            //以下开始业务处理
            ApiResult apiResult = new ApiResult();
            dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
            string strYearMonth = dataForm.vcYearMonth == null ? "" : dataForm.vcYearMonth;

            try
            {
                DataTable dt = fs0814_Logic.Search(strYearMonth);
                string[] heads = { "对象年月", "白夜", "D1", "D2", "D3","D4","D5","D6","D7","D8","D9","D10",
                "D11","D12","D13","D14","D15","D16","D17","D18","D19","D20",
                "D21","D22","D23","D24","D25","D26","D27","D28","D29","D30","D31"};
                string[] fields = { "vcYearMonth", "vcType", "vcD1", "vcD2", "vcD3","vcD4","vcD5","vcD6","vcD7","vcD8","vcD9","vcD10", 
                "vcD11","vcD12","vcD13","vcD14","vcD15","vcD16","vcD17","vcD18","vcD19","vcD20",
                "vcD21","vcD22","vcD23","vcD24","vcD25","vcD26","vcD27","vcD28","vcD29","vcD30","vcD31"};
                string strMsg = "";
                string filepath = ComFunction.DataTableToExcel(heads, fields, dt, _webHostEnvironment.ContentRootPath, loginInfo.UserId, FunctionID, ref strMsg);
                if (strMsg != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = strMsg;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = filepath;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M08UE1002", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}
