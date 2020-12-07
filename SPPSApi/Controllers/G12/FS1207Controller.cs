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
    [Route("api/FS1207/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1207Controller : BaseController
    {
        FS1207_Logic logic = new FS1207_Logic();
        private readonly string FunctionID = "FS1207";
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FS1207Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        #region 检索列表
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
            string vcMon = dataForm.vcMon;
            string vcType = dataForm.vcType;
            string vcPartsNo = dataForm.vcPartsNo;
            vcMon = vcMon == null ? "" : vcMon;
            vcType = vcType == null ? "" : vcType;
            vcPartsNo = vcPartsNo == null ? "" : vcPartsNo;
            if (!string.IsNullOrEmpty(vcPartsNo))
            {
                vcPartsNo = vcPartsNo.Replace("-", "").ToString();
            }
            try
            {
                DataTable dt = logic.search(vcMon, vcType, vcPartsNo);
                List<Object> dataList = ComFunction.convertToResult(dt, new string[] { "vcMonth", "vcPartsNo", "vcClass", "vcProject", "vcDock", "Total",
                "D1","D2","D3","D4","D5","D6","D7","D8","D9","D10","D11","D12","D13","D14","D15","D16","D17","D18","D19","D20","D21","D22","D23",
                "D24","D25","D26","D27","D28","D29","D30","D31"});
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
        }
        #endregion

        /// <summary>
        /// 发注计算-根据检索条件获取列表数据
        /// </summary>
        /// <param name="fS1207_Fzjs_ViewModel">检索条件</param>
        [HttpPost]
        [EnableCors("any")]
        public string GetFzjsRenders([FromBody] dynamic data)
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
            string vcMon = dataForm.vcMon;
            string vcPartsNo = dataForm.vcPartsNo;
            string vcYesOrNo = dataForm.vcYesOrNo;
            vcMon = vcMon == null ? "" : vcMon;
            vcPartsNo = vcPartsNo == null ? "" : vcPartsNo;
            vcYesOrNo = vcYesOrNo == null ? "" : vcYesOrNo;

            if (!string.IsNullOrEmpty(vcPartsNo))
            {
                vcPartsNo = vcPartsNo.Replace("-", "").ToString();
            }
            try
            {
                string _msg;
                DataTable dt = logic.GetFzjsRenders(vcMon, vcPartsNo, vcYesOrNo, out _msg);
                List<Object> dataList = ComFunction.convertToResult(dt, new string[] { "vcMonth", "vcPartsNo","iSRNum","Total","iXZNum","iBYNum",
                "iFZNum","syco","iCONum","iFlag","vcPartsNoFZ", "vcSource"});
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
        }

        [HttpPost]
        [EnableCors("any")]
        public string UpdateFZJSEditFZ([FromBody] dynamic data)
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
            string vcOrder = dataForm.vcOrder;
            string vcSaleUser = dataForm.vcSaleUser;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(Convert.ToString(dataForm.temp));
            vcType = vcType == null ? "" : vcType;
            vcOrder = vcOrder == null ? "" : vcOrder;
            vcSaleUser = vcSaleUser == null ? "" : vcSaleUser;

            try
            {
                string _msg = logic.UpdateFZJSEdit(dt, loginInfo.UserId);
                List<Object> dataList = ComFunction.convertToResult(dt, new string[] { "vcMonth", "vcPartsNo","iSRNum","Total","iXZNum","iBYNum",
                "iFZNum","syco","iCONum","iFlag","vcPartsNoFZ", "vcSource"});
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
        }
    }
}
