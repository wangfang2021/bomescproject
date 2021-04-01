using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SPPSApi.Controllers.G06
{
    [Route("api/FS0621/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0621Controller : BaseController
    {
        FS0621_Logic fs0621_Logic = new FS0621_Logic();
        private readonly string FunctionID = "FS0621";

        private readonly IWebHostEnvironment _webHostEnvironment;


        public FS0621Controller(IWebHostEnvironment webHostEnvironment)
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

                DataTable dtSupplier = fs0621_Logic.GetSupplier();
                DataTable dtWorkArea = fs0621_Logic.GetWorkArea();
                DataTable dtCarType = fs0621_Logic.GetCarType();

                List<Object> dataList_Supplier = ComFunction.convertToResult(dtSupplier, new string[] { "vcValue", "vcName" });
                List<Object> dataList_WorkArea = ComFunction.convertToResult(dtWorkArea, new string[] { "vcValue", "vcName" });
                List<Object> dataList_CarType = ComFunction.convertToResult(dtCarType, new string[] { "vcValue", "vcName" });
                List<Object> dataList_C018 = ComFunction.convertAllToResult(ComFunction.getTCode("C018"));//收货方
                List<Object> dataList_C003 = ComFunction.convertAllToResult(ComFunction.getTCode("C003"));//内外区分

                res.Add("C018", dataList_C018);
                res.Add("C003", dataList_C003);
                res.Add("Supplier", dataList_Supplier);
                res.Add("CarType", dataList_CarType);
                res.Add("WorkArea", dataList_WorkArea);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2101", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "初始化失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
        [HttpPost]
        [EnableCors("any")]
        public string changeSupplieridApi([FromBody] dynamic data)
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
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                string supplierCode = dataForm.supplierCode == null ? "" : dataForm.supplierCode;
                Dictionary<string, object> res = new Dictionary<string, object>();
                DataTable dtWorkAreaBySupplier = fs0621_Logic.GetWorkAreaBySupplier(supplierCode);

                List<Object> dataList_WorkAreaBySupplier = ComFunction.convertToResult(dtWorkAreaBySupplier, new string[] { "vcValue", "vcName" });

                res.Add("WorkAreaBySupplier", dataList_WorkAreaBySupplier);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2102", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "选择供应商联动工区失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #region 检索
        [HttpPost]
        [EnableCors("any")]
        public string searchApi([FromBody] dynamic data)
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
            string vcConsignee = dataForm.vcConsignee == null ? "" : dataForm.vcConsignee;//收货方
            string vcTargetMonth = dataForm.vcTargetMonth == null ? "" : dataForm.vcTargetMonth;//内示年月
            string vcPartNo = dataForm.vcPartNo == null ? "" : dataForm.vcPartNo;
            string vcCarType = dataForm.vcCarType == null ? "" : dataForm.vcCarType;//车型编号
            string vcInsideOutsideType = dataForm.vcInsideOutsideType == null ? "" : dataForm.vcInsideOutsideType;//内外
            string vcSupplier_id = dataForm.vcSupplier_id == null ? "" : dataForm.vcSupplier_id;//供应商代码
            string vcWorkArea = dataForm.vcWorkArea == null ? "" : dataForm.vcWorkArea;//工区
            
            try
            {
                DataTable dt = fs0621_Logic.Search(vcConsignee, vcTargetMonth,vcPartNo, vcCarType, vcInsideOutsideType, vcSupplier_id, vcWorkArea);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                //dtConverter.addField("dInputDate", ConvertFieldType.DateType, "yyyy/MM/dd hh:mm:ss");
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2103", ex, loginInfo.UserId);
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
            string vcConsignee = dataForm.vcConsignee == null ? "" : dataForm.vcConsignee;//收货方
            string vcTargetMonth = dataForm.vcTargetMonth == null ? "" : dataForm.vcTargetMonth;//内示年月
            string vcPartNo = dataForm.vcPartNo == null ? "" : dataForm.vcPartNo;
            string vcCarType = dataForm.vcCarType == null ? "" : dataForm.vcCarType;//车型编号
            string vcInsideOutsideType = dataForm.vcInsideOutsideType == null ? "" : dataForm.vcInsideOutsideType;//内外
            string vcSupplier_id = dataForm.vcSupplier_id == null ? "" : dataForm.vcSupplier_id;//供应商代码
            string vcWorkArea = dataForm.vcWorkArea == null ? "" : dataForm.vcWorkArea;//工区
            try
            {
                DataTable dt = fs0621_Logic.Search(vcConsignee, vcTargetMonth, vcPartNo, vcCarType, vcInsideOutsideType, vcSupplier_id, vcWorkArea);
                string[] head = new string[] { };
                string[] field = new string[] { };
                //[vcPartNo], [dBeginDate], [dEndDate]

                head = new string[] { "收货方", "导入时间", "内示年月", "品番", "车型编号", "内外", "供应商代码", "工区", "受入", "月总发货数" };
                field = new string[] { "CPDCOMPANY", "dInputDate", "TARGETMONTH", "PARTSNO", "CARFAMCODE", "INOUTFLAG", "SUPPLIERCODE", "iSupplierPlant", "DOCK", "RESULTQTYTOTAL" };
                string msg = string.Empty;
                //string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0309_Export.xlsx", 2, loginInfo.UserId, FunctionID);
                //string filepath = ComFunction.DataTableToExcel(head, field, dt, ".", loginInfo.UserId, FunctionID, ref msg);
                string filepath = ComFunction.generateExcelWithXlt(dt, field, _webHostEnvironment.ContentRootPath, "FS0621_Data.xlsx", 1, loginInfo.UserId, FunctionID, true);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2104", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}

