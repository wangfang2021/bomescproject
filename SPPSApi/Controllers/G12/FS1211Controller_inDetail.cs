using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SPPSApi.Controllers.G12
{
    [Route("api/FS1211_inDetail/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1211Controller_inDetail : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        FS1211_Logic logic = new FS1211_Logic();
        private readonly string FunctionID = "FS1211Controller_inDetail";
        public FS1211Controller_inDetail(IWebHostEnvironment webHostEnvironment)
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
                FS1211_Logic fS1211_Logic = new FS1211_Logic();
                Dictionary<string, Object> res = new Dictionary<string, Object>();
                List<Object> dataList_PlantSource = ComFunction.convertAllToResult(fS1211_Logic.plantsource());
                List<Object> dataList_ProtypeSource = ComFunction.convertAllToResult(fS1211_Logic.protypesource());
                res.Add("PlantSource", dataList_PlantSource);
                res.Add("ProtypeSource", dataList_ProtypeSource);

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
            string vcPlant = dataForm.vcPlant;
            string vcPartNo = dataForm.vcPartNo;
            string vcGC = dataForm.vcGC;
            string vcPlanProductionDateFrom = dataForm.vcPlanProductionDateFrom;
            string vcPlanProductionBZFrom = dataForm.vcPlanProductionBZFrom;
            string vcPlanProductionDateTo = dataForm.vcPlanProductionDateTo;
            string vcPlanProductionBZTo = dataForm.vcPlanProductionBZTo;
            string vcPlanProductAB = dataForm.vcPlanProductAB;
            string vcMon = dataForm.vcMon;
            string vcKbOrderId = dataForm.vcKbOrderId;
            string vcPackdiv = dataForm.vcPackdiv;
            string vcPlanPackDateFrom = dataForm.vcPlanPackDateFrom;
            string vcPlantPackBZFrom = dataForm.vcPlantPackBZFrom;
            string vcPlanPackDateTo = dataForm.vcPlanPackDateTo;
            string vcPlantPackBZTo = dataForm.vcPlantPackBZTo;
            string vcPlanPackAB = dataForm.vcPlanPackAB;
            try
            {
                string msg = string.Empty;
                DataTable dt = logic.getPartListCount(vcMon, vcPartNo, vcPlant, vcGC, vcKbOrderId,
                    vcPackdiv, vcPlanProductionDateFrom, vcPlanProductionBZFrom,
                    vcPlanPackDateFrom, vcPlantPackBZFrom, vcPlanProductionDateTo, vcPlanProductionBZTo,
                    vcPlanPackDateTo, vcPlantPackBZTo, vcPlanProductAB, vcPlanPackAB, ref msg);
                DtConverter dtConverter = new DtConverter();
                //dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);
                if (msg != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = msg;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
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

        #region 删除
        [HttpPost]
        [EnableCors("any")]
        public string DeletetKanbanPrintTbl([FromBody] dynamic data)
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
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                JArray checkedInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();
                if (listInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选择一条数据！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                logic.deletetKanbanPrintTbl(listInfoData, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0909", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除失败";
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
            string vcPlant = dataForm.vcPlant;
            string vcPartNo = dataForm.vcPartNo;
            string vcGC = dataForm.vcGC;
            string vcPlanProductionDateFrom = dataForm.vcPlanProductionDateFrom;
            string vcPlanProductionBZFrom = dataForm.vcPlanProductionBZFrom;
            string vcPlanProductionDateTo = dataForm.vcPlanProductionDateTo;
            string vcPlanProductionBZTo = dataForm.vcPlanProductionBZTo;
            string vcPlanProductAB = dataForm.vcPlanProductAB;
            string vcMon = dataForm.vcMon;
            string vcKbOrderId = dataForm.vcKbOrderId;
            string vcPackdiv = dataForm.vcPackdiv;
            string vcPlanPackDateFrom = dataForm.vcPlanPackDateFrom;
            string vcPlantPackBZFrom = dataForm.vcPlantPackBZFrom;
            string vcPlanPackDateTo = dataForm.vcPlanPackDateTo;
            string vcPlantPackBZTo = dataForm.vcPlantPackBZTo;
            string vcPlanPackAB = dataForm.vcPlanPackAB;
            try
            {
                string msg = string.Empty;
                DataTable dt = logic.getPartListCount(vcMon, vcPartNo, vcPlant, vcGC, vcKbOrderId,
                    vcPackdiv, vcPlanProductionDateFrom, vcPlanProductionBZFrom,
                    vcPlanPackDateFrom, vcPlantPackBZFrom, vcPlanProductionDateTo, vcPlanProductionBZTo,
                    vcPlanPackDateTo, vcPlantPackBZTo, vcPlanProductAB, vcPlanPackAB, ref msg);
                if (msg != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = msg;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string[] fields = { "vcMonth","vcGC","vcPlant","vcPartNo","vcDock","vcOrderNo","vcSerial","vcRealPrintTime","vcPlanPrintAB","vcRealPrintTime",
                                    "vcPlanProductionDate","vcPlanProductionAB","vcPlanProcDate","vcPlanProcAB","vcRealProcTime"
                };
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS1211_PartList.xlsx", 1, loginInfo.UserId, FunctionID);
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
    }
}
