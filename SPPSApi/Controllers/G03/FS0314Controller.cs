using System;
using System.Collections.Generic;
using System.Data;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace SPPSApi.Controllers.G03
{
    [Route("api/FS0314/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0314Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string FunctionID = "FS0314";
        FS0314_Logic fs0314_logic = new FS0314_Logic();

        public FS0314Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost]
        [EnableCors("any")]
        public string searchSupplier([FromBody]dynamic data)
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
            string supplierCode = dataForm.supplierCode == null ? "" : dataForm.supplierCode;
            string supplierName = dataForm.supplierName == null ? "" : dataForm.supplierName;

            try
            {
                DataTable dt = fs0314_logic.searchSupplier(supplierCode, supplierName);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dt;

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