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

        #region 初始化下拉框
        #region 绑定区分
        /// <summary>
        /// 绑定区分
        /// </summary>
        [HttpPost]
        [EnableCors("any")]
        public string BindClass()
        {
            //验证是否登录
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            ApiResult apiResult = new ApiResult();
            try
            {
                DataTable dt = logic.getPlant();
                List<Object> dataList = ComFunction.convertToResult(dt, new string[] { "vcData2" });
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

        #region 绑定销售员
        /// <summary>
        /// 绑定销售员
        /// </summary>
        [HttpPost]
        [EnableCors("any")]
        public string SaleUserGetLoad()
        {
            //验证是否登录
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            ApiResult apiResult = new ApiResult();
            try
            {
                DataTable dt = logic.ddlSaleUser();
                List<Object> dataList = ComFunction.convertToResult(dt, new string[] { "vcData3" });
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
        #endregion

        #region 发注计算
        #region 检索
        /// <summary>
        /// 发注计算-检索数据
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
        #endregion

        #region 发注
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
                string exlName;
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = logic.UpdateFZJSEditFZ(dt, vcType, vcOrder, loginInfo.UserId, out exlName);
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

        #region 保存
        /// <summary>
        /// 发注计算-保存
        /// </summary>
        [HttpPost]
        [EnableCors("any")]
        public string UpdateFZJSEdit([FromBody] dynamic data)
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
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(Convert.ToString(dataForm));
            try
            {
                apiResult.code = ComConstant.SUCCESS_CODE;
                string _msg = logic.UpdateFZJSEdit(dt, loginInfo.UserId);
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

        #endregion

        #region 追加发注
        #region 检索
        [HttpPost]
        [EnableCors("any")]
        public string GetZJFzRenders([FromBody] dynamic data)
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
                DataTable dt = logic.GetZJFzRenders(vcMon, vcPartsNo, vcYesOrNo, out _msg);
                List<Object> dataList = ComFunction.convertToResult(dt, new string[] { "vcMonth", "vcPartsNo", "iFZNum", "vcPartsNoFZ", "vcSource", "iFlag" });
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

        #region 保存
        /// <summary>
        /// 追加发注-保存
        /// </summary>
        [HttpPost]
        [EnableCors("any")]
        public string UpdateZJFZEdit([FromBody] dynamic data)
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
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(Convert.ToString(dataForm));
            try
            {
                apiResult.code = ComConstant.SUCCESS_CODE;
                string _msg = logic.updateAddFZ(dt, loginInfo.UserId);
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

        #region 发注
        [HttpPost]
        [EnableCors("any")]
        public string UpdateZJFZEditFZ([FromBody] dynamic data)
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
                string exlName;
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = logic.UpdateFZJSEditFZ(dt, vcType, vcOrder, loginInfo.UserId, out exlName);
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

        #endregion

    }
}
