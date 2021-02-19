using System;
using System.Collections.Generic;
using System.Data;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SPPSApi.Controllers.G03
{
    [Route("api/FS0316/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0316Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string FunctionID = "FS0316";
        FS0316_Logic fs0316_logic = new FS0316_Logic();

        public FS0316Controller(IWebHostEnvironment webHostEnvironment)
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
                List<Object> dataList_C006 = ComFunction.convertAllToResult(ComFunction.getTCode("C006"));//工程
                res.Add("C049", dataList_C049);
                res.Add("C006", dataList_C006);
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


            JArray factoryArray = dataForm.factory;
            List<string> factory = factoryArray.ToObject<List<string>>();
            JArray projectArray = dataForm.project;
            List<string> project = projectArray.ToObject<List<string>>();

            string flag = dataForm.flag == null ? "" : dataForm.flag;
            string supplierId = dataForm.supplierId == null ? "" : dataForm.supplierId;

            try
            {
                DataTable dt = fs0316_logic.searchApi(flag, factory, supplierId, project);
                DtConverter dtConverter = new DtConverter();
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE1601", ex, loginInfo.UserId);
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

            JArray factoryArray = dataForm.factory;
            List<string> factory = factoryArray.ToObject<List<string>>();
            JArray projectArray = dataForm.project;
            List<string> project = projectArray.ToObject<List<string>>();

            string flag = dataForm.flag == null ? "" : dataForm.flag;
            string supplierId = dataForm.supplierId == null ? "" : dataForm.supplierId;

            try
            {
                DataTable dt = fs0316_logic.searchApi(flag, factory, supplierId, project);
                string filepath = "";
                string strMsg = "";
                if (flag == "0")
                {
                    string[] heads = { "工厂", "内外", "点数", "号旧", "点数", "OE=SP", "点数" };
                    string[] fields = { "vcOriginCompany", "vcInOutflag", "Total", "vcHaoJiu", "HJTotal", "vcOE", "OETotal" };
                    filepath = ComFunction.DataTableToExcel(heads, fields, dt, _webHostEnvironment.ContentRootPath, loginInfo.UserId, FunctionID, ref strMsg);

                }
                else if (flag == "1")
                {
                    string[] heads = { "供应商代码", "点数", "号旧", "点数" };
                    string[] fields = { "vcSupplier_id", "Total", "vcHaoJiu", "HJTotal" };
                    filepath = ComFunction.DataTableToExcel(heads, fields, dt, _webHostEnvironment.ContentRootPath, loginInfo.UserId, FunctionID, ref strMsg);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE1602", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}