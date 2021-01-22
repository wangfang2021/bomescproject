using System;
using System.Collections.Generic;
using System.Data;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SPPSApi.Controllers.G03
{
    [Route("api/FS0317/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0317Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string FunctionID = "FS0317";
        FS0317_Logic fs0317_logic = new FS0317_Logic();

        public FS0317Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

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
            JArray dateRangeArray = dataForm.dateRange;
            List<string> dateRange = dateRangeArray.ToObject<List<string>>();

            string supplierId = dataForm.supplierId == null ? "" : dataForm.supplierId;

            try
            {
                string startTime = dateRange.Count > 0 ? dateRange[0].Replace("-", "") : "";
                string endTime = dateRange.Count > 0 ? dateRange[1].Replace("-", "") : "";

                DataTable dt = fs0317_logic.searchApi(startTime, endTime, supplierId);
                DtConverter dtConverter = new DtConverter();
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE1601", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
    }
}