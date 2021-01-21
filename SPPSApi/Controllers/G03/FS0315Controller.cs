using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SPPSApi.Controllers.G03
{
    [Route("api/FS0315/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0315Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string FunctionID = "FS0315";
        FS0315_Logic fs0315_logic = new FS0315_Logic();

        public FS0315Controller(IWebHostEnvironment webHostEnvironment)
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

                List<Object> dataList_C049 = ComFunction.convertAllToResult(ComFunction.getTCode("C049"));//工程

                res.Add("C049", dataList_C049);
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
            JArray projectArray = dataForm.project;
            List<string> project = projectArray.ToObject<List<string>>();
            try
            {
                string startTime = dateRange.Count > 0 ? dateRange[0].Replace("-", "") : "";
                string endTime = dateRange.Count > 0 ? dateRange[1].Replace("-", "") : "";

                List<DataTable> list = fs0315_logic.searchApi(startTime, endTime, project);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = list;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE1501", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }

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

            JArray dateRangeArray = dataForm.dateRange;
            List<string> dateRange = dateRangeArray.ToObject<List<string>>();
            JArray projectArray = dataForm.project;
            List<string> project = projectArray.ToObject<List<string>>();
            string flag = dataForm.flag == null ? "" : dataForm.flag;
            try
            {
                string startTime = dateRange.Count > 0 ? dateRange[0].Replace("-", "") : "";
                string endTime = dateRange.Count > 0 ? dateRange[1].Replace("-", "") : "";

                List<DataTable> list = fs0315_logic.searchApi(startTime, endTime, project);
                string filepath = "";
                string strMsg = "";
                if (flag == "0")
                {
                    string[] heads = { "年", "月", "日", "新设", "废止", "旧型", "点数/回", "点数/月", };
                    string[] fields = { "Year", "Month", "Day", "iXS", "iFZ", "iJX", "idaySUM", "imonthSUM" };
                    filepath = ComFunction.DataTableToExcel(heads, fields, list[0], _webHostEnvironment.ContentRootPath, loginInfo.UserId, FunctionID, ref strMsg);

                }
                else if (flag == "1")
                {
                    string[] heads = { "时间", "新设", "废止", "旧型", "其他", "合计", };
                    string[] fields = { "Date", "iXS", "iFZ", "iJX", "iOther", "imonthSUM" };
                    filepath = ComFunction.DataTableToExcel(heads, fields, list[1], _webHostEnvironment.ContentRootPath, loginInfo.UserId, FunctionID, ref strMsg);
                }
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0103", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}