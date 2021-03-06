using Common;
using DataEntity;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;

namespace SPPSApi.Controllers.G06
{
    [Route("api/FS0608/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0608Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string FunctionID = "FS0608";
        FS0608_Logic fs0608_Logic = new FS0608_Logic();

        public FS0608Controller(IWebHostEnvironment webHostEnvironment)
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

                //获取对象年月
                DateTime varDxny = DateTime.Now.AddMonths(1);

                //发注工厂
                List<Object> dataList_C000 = ComFunction.convertAllToResult(ComFunction.getTCode("C000"));

                res.Add("C000", dataList_C000);
                res.Add("varDxny", varDxny);

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

            DateTime varDxny = dataForm.varDxny == null ? "" : Convert.ToDateTime(dataForm.varDxny);
            string varFZGC = dataForm.varFZGC.value == null ? "" : dataForm.varFZGC.value;

            try
            {
                JObject dt = fs0608_Logic.Search(varDxny, varFZGC);
                //List<Object> dataList = ComFunction.convertAllToResult(dt);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dt;
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

        #region 保存数据
        [HttpPost]
        [EnableCors("any")]
        public string saveApi([FromBody] dynamic data)
        {

            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                //return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            //以下开始业务处理
            ApiResult apiResult = new ApiResult();
            dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));

            //单、双、非
            List<string> dayTypeVals = dataForm.dayTypeVals.ToObject<List<string>>();

            //属于第几周（0、1、2、3、4）
            List<string> weekTypeVals = dataForm.weekTypeVals.ToObject<List<string>>();

            //对象年月
            DateTime Dxny = dataForm.varDxny == null ? "" : Convert.ToDateTime(dataForm.varDxny);
            string varDxny = Dxny.ToString("yyyyMM");
            //发注工厂
            string varFZGC = dataForm.varFZGC.value == null ? "" : dataForm.varFZGC.value;
            //总稼动日
            decimal TOTALWORKDAYS = dataForm.TOTALWORKDAYS == null ? 0 : Convert.ToDecimal(dataForm.TOTALWORKDAYS);

            try
            {
                fs0608_Logic.save(dayTypeVals, weekTypeVals, varDxny, varFZGC, TOTALWORKDAYS, loginInfo.UserId);

                apiResult.code = ComConstant.SUCCESS_CODE;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0104", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}