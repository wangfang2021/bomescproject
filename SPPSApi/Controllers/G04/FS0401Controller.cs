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

                List<Object> dataList_C003 = ComFunction.convertAllToResult(ComFunction.getTCode("C003"));//内外
                List<Object> dataList_C047 = ComFunction.convertAllToResult(ComFunction.getTCode("C047"));//订货方式
                List<Object> dataList_CarType = ComFunction.convertAllToResult(fs0401_Logic.getCarType());//车种


                res.Add("C003", dataList_C003);
                res.Add("C047", dataList_C047);
                res.Add("CarType", dataList_CarType);

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
                DataTable dt = fs0401_Logic.searchApi(Part_id, TimeFrom, TimeTo, carType, InOut, DHFlag);
                DtConverter dtConverter = new DtConverter();

                dtConverter.addField("dFromTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dToTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dFromTimeQty", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dToTimeQty", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dDebugTime", ConvertFieldType.DateType, "yyyy/MM");

                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);

                if (dataList.Count > 10000)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "结果条数大于10000条,请添加条件检索或导出。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

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

            string Part_id = dataForm.Part_id == null ? "" : dataForm.Part_id;
            string TimeFrom = dataForm.TimeFrom == null ? "" : dataForm.TimeFrom;
            string TimeTo = dataForm.TimeTo == null ? "" : dataForm.TimeTo;
            string carType = dataForm.carType == null ? "" : dataForm.carType;
            string InOut = dataForm.InOut == null ? "" : dataForm.InOut;
            string DHFlag = dataForm.DHFlag == null ? "" : dataForm.DHFlag;

            try
            {

                DataTable dt = fs0401_Logic.searchApi(Part_id, TimeFrom, TimeTo, carType, InOut, DHFlag);
                string[] fields = { "vcPackingPlant", "vcPartIdDC", "dFromTime", "dToTime", "vcPartId_ReplaceDC", "vcCarfamilyCode", "vcInOut_name", "vcHaoJiu_name", "vcOrderingMethod_name", "iPackingQty", "BoxFromTime_ed", "BoxToTime_ed", "vcOldProduction_name", "dDebugTime" };
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0401.xlsx", 1, loginInfo.UserId, FunctionID, true);
                if (filepath == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "导出生成文件失败";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = filepath;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M04UE0102", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

    }
}