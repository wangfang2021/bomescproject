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

                List<Object> dataList_C003 = ComFunction.convertAllToResult(ComFunction.getTCode("C003"));//内外
                List<Object> dataList_C047 = ComFunction.convertAllToResult(ComFunction.getTCode("C047"));//订货方式


                res.Add("C003", dataList_C003);
                res.Add("C047", dataList_C047);

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

            string Part_id = dataForm.Part_id == null ? "" : dataForm.Part_id;
            string TimeFrom = dataForm.TimeFrom == null ? "" : dataForm.TimeFrom;
            string TimeTo = dataForm.TimeTo == null ? "" : dataForm.TimeTo;
            string carType = dataForm.carType == null ? "" : dataForm.carType;
            string InOut = dataForm.InOut == null ? "" : dataForm.InOut;
            string DHFlag = dataForm.DHFlag == null ? "" : dataForm.DHFlag;


            try
            {
                //List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);
                List<Object> dataList = new List<object>();
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