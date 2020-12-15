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
    [Route("api/FS1208/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1208Controller : BaseController
    {
        FS1208_Logic logic = new FS1208_Logic();
        private readonly string FunctionID = "FS1208";
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FS1208Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        #region 绑定下拉框
        #region 绑定工厂
        [HttpPost]
        [EnableCors("any")]
        public string GetPlants()
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
                DataTable dt = logic.GetPlant();
                List<Object> dataList = ComFunction.convertToResult(dt, new string[] { "vcData1", "vcData2" });
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
        #region 绑定类别
        [HttpPost]
        [EnableCors("any")]
        public string GetPlanType()
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
                DataTable dt = logic.GetPlanType();
                List<Object> dataList = ComFunction.convertToResult(dt, new string[] { "planType", "value" });
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

        #region 检索
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
            string vcPlant = dataForm.vcPlant;
            string vcType = dataForm.vcType;
            vcMon = vcMon == null ? "" : vcMon;
            vcPlant = vcPlant == null ? "" : vcPlant;
            vcType = vcType == null ? "" : vcType;
            try
            {
                Exception ex = new Exception();
                DataTable dt = logic.serchData(vcMon, vcPlant, vcType, ref ex, "");
                List<Object> dataList = ComFunction.convertAllToResult(dt);
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


        #region 子页面
        #region 检索
        /// <summary>
        /// 紧急计划编辑-根据检索条件获取列表数据
        /// </summary>
        /// <param name="fS1208_ViewModel">检索条件</param>
        [HttpPost]
        [EnableCors("any")]
        //public string GetEditRenders(string vcMon, string vcPartsNo, string vcCarType, string vcDock, string vcType, 
        //                             string vcPro, string vcZhi, string vcDay, string vcOrder)
        public string GetEditRenders([FromBody] dynamic data)
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
            string vcCarType = dataForm.vcCarType;
            string vcDock = dataForm.vcDock;
            string vcType = dataForm.vcType;
            string vcPro = dataForm.vcPro;
            string vcZhi = dataForm.vcZhi;
            string vcDay = dataForm.vcDay;
            string vcOrder = dataForm.vcOrder;
            vcMon = vcMon == null ? "" : vcMon;
            vcPartsNo = vcPartsNo == null ? "" : vcPartsNo;
            vcCarType = vcCarType == null ? "" : vcCarType;
            vcDock = vcDock == null ? "" : vcDock;
            vcType = vcType == null ? "" : vcType;
            vcPro = vcPro == null ? "" : vcPro;
            vcZhi = vcZhi == null ? "" : vcZhi;
            vcDay = vcDay == null ? "" : vcDay;
            vcOrder = vcOrder == null ? "" : vcOrder;
            try
            {
                Exception ex = new Exception();
                DataTable dt = logic.getEDPlanInfo(vcMon, vcPartsNo, vcCarType, vcDock, vcType, vcPro, vcZhi, vcDay, vcOrder);
                List<Object> dataList = ComFunction.convertAllToResult(dt);
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
        /// 紧急计划编辑-保存按钮
        /// </summary>
        /// <param name="dt">更新集合</param>
        [HttpPost]
        [EnableCors("any")]
        public string UpdateTable([FromBody] dynamic data)
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
                apiResult.data = logic.UpdateTable(dt, loginInfo.UserId);
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

        #region 生成紧急计划
        /// <summary>
        /// 紧急计划编辑-生成紧急计划
        /// </summary>
        /// <param name="mon">对象月</param>
        [HttpPost]
        [EnableCors("any")]
        public string UpdatePlan([FromBody] dynamic data)
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
            try
            {
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = logic.UpdatePlan(vcMon, loginInfo.UserId);
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
