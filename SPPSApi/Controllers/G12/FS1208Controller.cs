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
                Dictionary<string, Object> res = new Dictionary<string, Object>();
                List<Object> dataList_PlantSource = ComFunction.convertAllToResult(ComFunction.getTCode("C000"));
                List<Object> dataList_PlanSource = ComFunction.convertAllToResult(logic.GetPlanType());
                res.Add("PlantSource", dataList_PlantSource);
                res.Add("PlanSource", dataList_PlanSource);
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

        #region 检索
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
                DtConverter dtConverter = new DtConverter();
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, loginInfo.UserId);
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
                string[] fields = { "vcMonth", "vcPlant", "vcPartsno", "vcDock", "vcCarType", "vcCalendar1", "vcCalendar2", "vcCalendar3", "vcCalendar4",
                "vcPartsNameCHN","vcCurrentPastCode","vcMonTotal",
                "TD1b","TD1y","TD2b","TD2y","TD3b","TD3y","TD4b","TD4y","TD5b","TD5y","TD6b","TD6y","TD7b","TD7y","TD8b","TD8y","TD9b","TD9y","TD10b","TD10y",
                "TD11b","TD11y","TD12b","TD12y","TD13b","TD13y","TD14b","TD14y","TD15b","TD15y","TD16b","TD16y","TD17b","TD17y","TD18b","TD18y","TD19b","TD19y",
                "TD20b","TD20y","TD21b","TD21y","TD22b","TD22y","TD23b","TD23y","TD24b","TD24y","TD25b","TD25y","TD26b","TD26y","TD27b","TD27y","TD28b","TD28y",
                "TD29b","TD29y","TD30b","TD30y","TD31b","TD31y",
                "ED1b","ED1y","ED2b","ED2y","ED3b","ED3y","ED4b","ED4y","ED5b","ED5y","ED6b","ED6y","ED7b","ED7y","ED8b","ED8y","ED9b","ED9y","ED10b","ED10y",
                "ED11b","ED11y","ED12b","ED12y","ED13b","ED13y","ED14b","ED14y","ED15b","ED15y","ED16b","ED16y","ED17b","ED17y","ED18b","ED18y","ED19b","ED19y",
                "ED20b","ED20y","ED21b","ED21y","ED22b","ED22y","ED23b","ED23y","ED24b","ED24y","ED25b","ED25y","ED26b","ED26y","ED27b","ED27y","ED28b","ED28y",
                "ED29b","ED29y","ED30b","ED30y","ED31b","ED31y"
                };
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS1208_EDPlanExprot.xlsx", 1, loginInfo.UserId, FunctionID);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0904", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
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
