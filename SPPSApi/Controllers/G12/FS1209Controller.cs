using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
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

namespace SPPSApi.Controllers.G12
{
    [Route("api/FS1209/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1209Controller : BaseController
    {
        FS1209_Logic logic = new FS1209_Logic();
        private readonly string FunctionID = "FS1209";
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FS1209Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        #region 检索
        /// <summary>
        /// 根据检索条件获取列表数据
        /// </summary>
        [HttpPost]
        [EnableCors("any")]
        public string GetRenders([FromBody] dynamic data)
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
            string vcType = dataForm.vcType;
            string vcPrintPartNo = dataForm.vcPrintPartNo;
            string vcLianFan = dataForm.vcLianFan;
            string vcPorPlant = dataForm.vcPorPlant;
            string vcKbOrderId = dataForm.vcKbOrderId;
            string vcPorType = dataForm.vcPorType;
            vcType = vcType == null ? "" : vcType;
            vcPrintPartNo = vcPrintPartNo == null ? "" : vcPrintPartNo;
            vcLianFan = vcLianFan == null ? "" : vcLianFan;
            vcPorPlant = vcPorPlant == null ? "" : vcPorPlant;
            vcKbOrderId = vcKbOrderId == null ? "" : vcKbOrderId;
            vcPorType = vcPorType == null ? "" : vcPorType;
            try
            {
                if ("PP".Equals(vcPorType))
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "没有生产部署权限，检索无数据";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                DataTable resutPrint;
                if (vcType == "3")
                {
                    resutPrint = logic.searchPrint(vcPrintPartNo, vcKbOrderId, vcLianFan, vcPorType, vcPorPlant, null);
                }
                else
                {
                    resutPrint = logic.searchPrint(vcPrintPartNo, vcType, vcKbOrderId, vcLianFan, vcPorType, vcPorPlant, null);
                }
                List<Object> dataList = ComFunction.convertAllToResult(resutPrint);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0204", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }

            #region 弃用
            //if (null == fS1209_ViewModel)
            //{
            //    throw new BaseException(new ResponseData(ErrorCode.PARAM_ERROR, "参数不能为空"));
            //}
            //if (ModelState.IsValid)
            //{
            //    if ("PP".Equals(fS1209_ViewModel.vcPorType))
            //    {
            //        throw new BaseException(new ResponseData(ErrorCode.PARAM_ERROR, "没有生产部署权限，检索无数据"));
            //    }
            //    FS1209_Logic fS1209_Logic = new FS1209_Logic();
            //    DataTable resutPrint;
            //    if (fS1209_ViewModel.vcType == "3")
            //    {
            //        resutPrint = fS1209_Logic.searchPrint(fS1209_ViewModel.vcPrintPartNo, fS1209_ViewModel.vcKbOrderId,
            //            fS1209_ViewModel.vcLianFan, fS1209_ViewModel.vcPorType, fS1209_ViewModel.vcPorPlant, fS1209_ViewModel.vcDtportype);
            //    }
            //    else
            //    {
            //        resutPrint = fS1209_Logic.searchPrint(fS1209_ViewModel.vcPrintPartNo, fS1209_ViewModel.vcType,
            //            fS1209_ViewModel.vcKbOrderId, fS1209_ViewModel.vcLianFan, fS1209_ViewModel.vcPorType,
            //            fS1209_ViewModel.vcPorPlant, fS1209_ViewModel.vcDtportype);
            //    }

            //    return new ResponseData(resutPrint);
            //}
            //else
            //{
            //    throw new BaseException(new ResponseData(ErrorCode.PARAM_ERROR, "请求数据格式错误"));
            //}
            #endregion
        }
        #endregion

        #region 绑定下拉框
        #region 绑定工厂
        [HttpPost]
        [EnableCors("any")]
        public string GetPorPlant()
        {
            //以下开始业务处理
            ApiResult apiResult = new ApiResult();
            try
            {
                DataTable dt = logic.dllPorPlant();
                List<Object> dataList = ComFunction.convertAllToResult(dt);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0204", ex, "");
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 绑定生产部署
        //[HttpPost]
        //[EnableCors("any")]
        //public string GetPorType()
        //{
        //    //以下开始业务处理
        //    ApiResult apiResult = new ApiResult();
        //    try
        //    {
        //        DataTable dt = logic.dllPorType(;
        //        List<Object> dataList = ComFunction.convertAllToResult(dt);
        //        apiResult.code = ComConstant.SUCCESS_CODE;
        //        apiResult.data = dataList;
        //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //    }
        //    catch (Exception ex)
        //    {
        //        ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0204", ex, "");
        //        apiResult.code = ComConstant.ERROR_CODE;
        //        apiResult.data = "检索失败";
        //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //    }
        //}
        #endregion
        #endregion
    }
}
