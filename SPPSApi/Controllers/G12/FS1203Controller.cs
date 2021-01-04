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
    [Route("api/FS1203/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1203Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        FS1203_Logic logic = new FS1203_Logic();
        private readonly string FunctionID = "FS1203";

        public FS1203Controller(IWebHostEnvironment webHostEnvironment)
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
                List<Object> dataList_PlanType = ComFunction.convertAllToResult(logic.getPlantype());
                List<Object> dataList_Plant = ComFunction.convertAllToResult(logic.bindplant());
                res.Add("dataList_PlanType", dataList_PlanType);
                res.Add("dataList_Plant", dataList_Plant);
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
            string strMon = dataForm.vcMon;
            string strPlant = dataForm.vcPlant;
            string strPlanType = dataForm.vcPlanType;
            string strType = dataForm.vcType;
            try
            {
                DataTable dt = logic.serchData(strMon, strPlanType, strType, strPlant, strPlant);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
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
            string strMon = dataForm.vcMon;
            string strPlant = dataForm.vcPlant;
            string strPlanType = dataForm.vcPlanType;
            string strType = dataForm.vcType;
            try
            {
                DataTable dt = logic.serchData(strMon, strPlanType, strType, strPlant, strPlant);
                string[] fields = { "vcMonth","vcPlant","vcPartsno","vcDock","vcCarType","vcEDflag","vcCalendar1","vcCalendar2","vcCalendar3","vcCalendar4",
                                    "vcPartsNameCHN","vcProject1","vcProjectName","vcCurrentPastCode","vcMonTotal",
                                    "TD1b","TD1y","TD2b","TD2y","TD3b","TD3y","TD4b","TD4y","TD5b","TD5y","TD6b","TD6y",
                                    "TD7b","TD7y","TD8b","TD8y","TD9b","TD9y","TD10b","TD10y","TD11b","TD11y","TD12b","TD12y",
                                    "TD13b","TD13y","TD14b","TD14y","TD15b","TD15y","TD16b","TD16y","TD17b","TD17y","TD18b","TD18y",
                                    "TD19b","TD19y","TD20b","TD20y","TD21b","TD21y","TD22b","TD22y","TD23b","TD23y","TD24b","TD24y",
                                    "TD25b","TD25y","TD26b","TD26y","TD27b","TD27y","TD28b","TD28y","TD29b","TD29y","TD30b","TD30y","TD31b","TD31y",
                                    "ED1b","ED1y","ED2b","ED2y","ED3b","ED3y","ED4b","ED4y","ED5b","ED5y","ED6b","ED6y",
                                    "ED7b","ED7y","ED8b","ED8y","ED9b","ED9y","ED10b","ED10y","ED11b","ED11y","ED12b","ED12y",
                                    "ED13b","ED13y","ED14b","ED14y","ED15b","ED15y","ED16b","ED16y","ED17b","ED17y","ED18b","ED18y",
                                    "ED19b","ED19y","ED20b","ED20y","ED21b","ED21y","ED22b","ED22y","ED23b","ED23y","ED24b","ED24y",
                                    "ED25b","ED25y","ED26b","ED26y","ED27b","ED27y","ED28b","ED28y","ED29b","ED29y","ED30b","ED30y","ED31b","ED31y"
                };
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS1203_PlanMake.xlsx", 1, loginInfo.UserId, FunctionID);
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
