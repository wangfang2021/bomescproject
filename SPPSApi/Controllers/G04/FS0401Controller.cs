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
    [Route("api/FS0401/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0401Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string FunctionID = "FS0401";
        FS0401_Logic fs0401_Logic = new FS0401_Logic();

        public FS0401Controller(IWebHostEnvironment webHostEnvironment)
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

                List<Object> dataList_C036 = ComFunction.convertAllToResult(ComFunction.getTCode("C036"));//月度订单对应状态
                List<Object> dataList_C037 = ComFunction.convertAllToResult(ComFunction.getTCode("C037"));//月度订单合意状态


                res.Add("C036", dataList_C036);
                res.Add("C037", dataList_C037);

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

            string strYearMonth = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth + "/01").ToString("yyyyMM");
            string strDyState = dataForm.DyState == null ? "" : dataForm.DyState;
            string strHyState = dataForm.HyState == null ? "" : dataForm.HyState;
            string strPart_id = dataForm.Part_id == null ? "" : dataForm.Part_id;


            try
            {
                //DataTable dt = fs0402_Logic.Search(strYearMonth, strDyState, strHyState, strPart_id);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("dHyTime", ConvertFieldType.DateType, "yyyy/MM/dd HH:mm:ss");
                //List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);
                apiResult.code = ComConstant.SUCCESS_CODE;
                //apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M04UE0202", ex, loginInfo.UserId);
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

            string strYearMonth = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth).ToString("yyyyMM");
            string strYearMonth_2 = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth).AddMonths(1).ToString("yyyyMM");
            string strYearMonth_3 = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth).AddMonths(2).ToString("yyyyMM");




            string strDyState = dataForm.DyState == null ? "" : dataForm.DyState;
            string strHyState = dataForm.HyState == null ? "" : dataForm.HyState;
            string strPart_id = dataForm.Part_id == null ? "" : dataForm.Part_id;

            try
            {
                //DataTable dt = fs0402_Logic.Search(strYearMonth, strDyState, strHyState, strPart_id);
                //string[] fields = { "vcYearMonth", "vcDyState_Name", "vcHyState_Name", "vcPart_id", "iCbSOQN", "decCbBdl"
                //,"iCbSOQN1","iCbSOQN2","iTzhSOQN","iTzhSOQN1","iTzhSOQN2","iHySOQN","iHySOQN1","iHySOQN2"
                //,"dHyTime"
                //};
                //string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0401.xlsx", 2, loginInfo.UserId, FunctionID, strYearMonth, strYearMonth_2, strYearMonth_3);
                //if (filepath == "")
                //{
                //    apiResult.code = ComConstant.ERROR_CODE;
                //    apiResult.data = "导出生成文件失败";
                //    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                //}
                //apiResult.code = ComConstant.SUCCESS_CODE;
                //apiResult.data = filepath;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M04UE0205", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

    }
}