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
    [Route("api/FS0402/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0402Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string FunctionID = "FS0402";
        FS0402_Logic fs0402_Logic = new FS0402_Logic();

        public FS0402Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }


        #region 获取对象年月
        [HttpPost]
        [EnableCors("any")]
        public string getDxnyApi()
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
                DateTime varDxny = DateTime.Now.AddMonths(1);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = varDxny;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0104", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
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

            string varDxny = dataForm.varDxny==null?"": Convert.ToDateTime(dataForm.varDxny).ToString("yyyy/MM");
            string varDyzt = dataForm.varDyzt==null?"": dataForm.varDyzt;
            string varHyzt = dataForm.varHyzt==null?"": dataForm.varHyzt;
            string PARTSNO = dataForm.PARTSNO == null ? "" : dataForm.PARTSNO;


            try
            {
                DataTable dt = fs0402_Logic.Search(varDxny, varDyzt, varHyzt, PARTSNO);
                List<Object> dataList = ComFunction.convertAllToResult(dt);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0104", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 承认
        [HttpPost]
        [EnableCors("any")]
        public string crApi([FromBody] dynamic data)
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
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));

                string varDxny = dataForm.varDxny == null ? "" : Convert.ToDateTime(dataForm.varDxny).ToString("yyyy/MM");
                string varDyzt = dataForm.varDyzt == null ? "" : dataForm.varDyzt;
                string varHyzt = dataForm.varHyzt == null ? "" : dataForm.varHyzt;
                string PARTSNO = dataForm.PARTSNO == null ? "" : dataForm.PARTSNO;

                int count=fs0402_Logic.Cr(varDxny, varDyzt, varHyzt, PARTSNO);

                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UI0103", null, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = count;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0201", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

    }
}