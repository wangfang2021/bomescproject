using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Logic;
namespace SPPSApi.Controllers.G02
{
    [Route("api/FS0202/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0202Controller : BaseController
    {
        FS0202_Logic fs0202_logic = new FS0202_Logic();
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string FunctionID = "FS0202";

        public FS0202Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost]
        [EnableCors("any")]
        public string searchHistory([FromBody]dynamic data)
        {
            fs0202_logic.getPartId("843W", "2320924060", "ABC");

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
            string filename = dataForm.filename == null ? "" : dataForm.filename;
            string TimeFrom = dataForm.timeFrom == null ? "" : dataForm.timeFrom;
            string TimeTo = dataForm.timeTo == null ? "" : dataForm.timeTo;

            try
            {
                DataTable dt = fs0202_logic.searchHistory(filename, TimeFrom, TimeTo);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("dOperatorTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;

                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M02UE0201", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
    }
}