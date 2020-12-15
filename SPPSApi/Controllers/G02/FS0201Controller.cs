using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace SPPSApi.Controllers.G02
{

    [Route("api/FS0201/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0201Controller : BaseController
    {
        FS0201_Logic fs0201_logic = new FS0201_Logic();
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string FunctionID = "FS0201";

        public FS0201Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }


        [HttpPost]
        [EnableCors("any")]
        public string searchSPI([FromBody]dynamic data)
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
            string vcSPINo = dataForm.vcSPINo == null ? "" : dataForm.vcSPINo;
            string vcPart_Id = dataForm.vcPart_Id == null ? "" : dataForm.vcPart_Id;
            string vcCarType = dataForm.vcCarType == null ? "" : dataForm.vcCarType;
            string vcState = dataForm.vcState == null ? "" : dataForm.vcCarType;
            try
            {
                DataTable dt = fs0201_logic.searchSPI(vcSPINo, vcPart_Id, vcCarType, vcState);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("dHandleTime", ConvertFieldType.DateType, "yyyy-MM-dd");
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


        [HttpPost]
        [EnableCors("any")]
        public string importSPI()
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
            string path = Directory.GetCurrentDirectory() + @"\Doc\import\FS0201";
            try
            {
                fs0201_logic.AddSPI(path, loginInfo.UserId);
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