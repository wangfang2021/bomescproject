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
    [Route("api/FS1210/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1210Controller : BaseController
    {
        FS1210_Logic logic = new FS1210_Logic();
        private readonly string FunctionID = "FS1210";
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FS1210Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

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
                FS1209_Logic logic_1 = new FS1209_Logic();
                DataTable dt = logic_1.dllPorPlant();
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
        [HttpPost]
        [EnableCors("any")]
        public string GetPorType()
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
                FS1209_Logic logic_1 = new FS1209_Logic();
                string[] userPorType = null;
                DataTable dt = logic_1.dllPorType(loginInfo.UserId, ref userPorType);
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
        #endregion

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
            string vcType_ = string.Empty;
            string vcKbOrderId = dataForm.vcKbOrderId;
            string vcTF = dataForm.vcTF;
            string vcFBZ = dataForm.vcFBZ;
            string vcTT = dataForm.vcTT;
            string vcTBZ = dataForm.vcTBZ;
            string vcGC = dataForm.vcGC;
            string vcPartsNo = dataForm.vcPartsNo;
            string vcPartsNo_ = string.Empty;
            string vcCarType = dataForm.vcCarType;
            string vcPlant = dataForm.vcPlant;

            vcType = vcType == null ? "" : vcType;
            vcKbOrderId = vcKbOrderId == null ? "" : vcKbOrderId;
            vcTF = vcTF == null ? "" : vcTF;
            vcFBZ = vcFBZ == null ? "" : vcFBZ;
            vcTT = vcTT == null ? "" : vcTT;
            vcTBZ = vcTBZ == null ? "" : vcTBZ;
            vcGC = vcGC == null ? "" : vcGC;
            vcPartsNo = vcPartsNo == null ? "" : vcPartsNo;
            vcCarType = vcCarType == null ? "" : vcCarType;
            vcPlant = vcPlant == null ? "" : vcPlant;
            try
            {
                switch (vcType)
                {
                    case "再发行":
                        vcType_ = "A";
                        break;
                    case "提前打印":
                        vcType_ = "B";
                        break;
                    case "延迟打印":
                        vcType_ = "C";
                        break;
                }
                FS1209_Logic logic_1 = new FS1209_Logic();
                string[] userPorType = null;
                DataTable dt1 = logic_1.dllPorType(loginInfo.UserId, ref userPorType);
                vcPartsNo_ = vcPartsNo.Replace("-", "").ToString();
                DataTable tb = logic.PrintData(vcKbOrderId, vcTF, vcFBZ, vcTT, vcTF, vcPartsNo_, vcCarType, vcGC, vcType_, vcPlant, dt1);
                List<Object> dataList = ComFunction.convertAllToResult(tb);
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

        #region 提交数据库
        /// <summary>
        /// 特殊打印录入-更新列表数据
        /// </summary>
        /// <param name="dt">列表数据集合</param>
        [HttpPost]
        [EnableCors("any")]
        public string InUpdeOldData([FromBody] dynamic data)
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
                apiResult.data = logic.InUpdeOldData(dt);
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0204", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "更新失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 特殊打印录入
        /// <summary>
        /// 特殊打印录入
        /// </summary>
        [HttpPost]
        [EnableCors("any")]
        public string SearchPrintTDB([FromBody] dynamic data)
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
            string vcPrint = dataForm.vcPrint;
            try
            {
                FS1209_Logic logic_1 = new FS1209_Logic();
                string[] userPorType = null;
                DataTable dt1 = logic_1.dllPorType(loginInfo.UserId, ref userPorType);
                DataTable tb = logic.SearchPrintTDB(vcPrint, userPorType);
                List<Object> dataList = ComFunction.convertAllToResult(tb);
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

        #region 特殊打印
        /// <summary>
        /// 特殊打印 检索条件获取列表数据 无传入参数
        /// </summary>
        [HttpPost]
        [EnableCors("any")]
        public string SearchPrintT()
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
                FS1209_Logic logic_1 = new FS1209_Logic();
                DataTable tb = logic.searchPrintT();
                List<Object> dataList = ComFunction.convertAllToResult(tb);
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

        #region 确认单再发行
        /// <summary>
        /// 确认单再发行
        /// </summary>
        [HttpPost]
        [EnableCors("any")]
        public string SearchRePrintKBQR([FromBody] dynamic data)
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
            string vcKbOrderId = dataForm.vcKbOrderId;
            string vcPlanPrintDate = dataForm.vcPlanPrintDate;
            string vcPlanProcDate = dataForm.vcPlanProcDate;
            string vcPrintDate = dataForm.vcPrintDate;
            string vcGC = dataForm.vcGC;
            string vcPlanPrintBZ = dataForm.vcPlanPrintBZ;
            string vcPlanProcBZ = dataForm.vcPlanProcBZ;
            vcKbOrderId = vcKbOrderId == null ? "" : vcKbOrderId;
            vcPlanPrintDate = vcPlanPrintDate == null ? "" : vcPlanPrintDate;
            vcPlanProcDate = vcPlanProcDate == null ? "" : vcPlanProcDate;
            vcPrintDate = vcPrintDate == null ? "" : vcPrintDate;
            vcGC = vcGC == null ? "" : vcGC;
            vcPlanPrintBZ = vcPlanPrintBZ == null ? "" : vcPlanPrintBZ;
            vcPlanProcBZ = vcPlanProcBZ == null ? "" : vcPlanProcBZ;
            try
            {
                DataTable tb = logic.SearchRePrintKBQR(vcKbOrderId, vcGC, vcPlanPrintDate, vcPlanPrintBZ, vcPlanProcDate, vcPlanProcBZ, vcPrintDate);
                List<Object> dataList = ComFunction.convertAllToResult(tb);
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
    }
}
