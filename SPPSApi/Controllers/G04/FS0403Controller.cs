using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace SPPSApi.Controllers.G04
{
    [Route("api/FS0403/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0403Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string FunctionID = "FS0403";
        FS0403_Logic fs0403_Logic = new FS0403_Logic();

        public FS0403Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }


        #region 检索数据
        [HttpPost]
        [EnableCors("any")]
        public string searchApi([FromBody] dynamic data)
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

            string changeNo = dataForm.changeNo == null ? "" : dataForm.changeNo;
            string state = dataForm.state == null ? "" : dataForm.state;
            string orderNo = dataForm.orderNo == null ? "" : dataForm.orderNo;


            try
            {
                DataTable dt = fs0403_Logic.searchApi(changeNo, state, orderNo);
                DtConverter dtConverter = new DtConverter();

                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M04UE0101", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion



    }
}