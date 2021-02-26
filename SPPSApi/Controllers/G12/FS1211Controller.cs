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
    [Route("api/FS1211/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1211Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS1211_Logic logic = new FS1211_Logic();
        private readonly string FunctionID = "FS1211";

        public FS1211Controller(IWebHostEnvironment webHostEnvironment)
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
            string vcPlant = dataForm.vcPlant == null ? "" : dataForm.vcPlant;
            string vcPorType = dataForm.vcPorType == null ? "" : dataForm.vcPorType;
            string vcMon = dataForm.vcMon == null ? "" : dataForm.vcMon;
            string vcTF = dataForm.vcTF == null ? "" : dataForm.vcTF;
            string vcTO = dataForm.vcTO == null ? "" : dataForm.vcTO;
            string vcType = dataForm.vcType == null ? "" : dataForm.vcType;
            string vcPartsno = dataForm.vcPartsno == null ? "" : dataForm.vcPartsno;
            string vcDock = dataForm.vcDock == null ? "" : dataForm.vcDock;
            string vcOrder = dataForm.vcOrder == null ? "" : dataForm.vcOrder;
            string vcSerial = dataForm.vcSerial == null ? "" : dataForm.vcSerial;
            string vcED = dataForm.vcED == null ? "" : dataForm.vcED;
            try
            {
                if ("PP".Equals(vcPorType))
                    return "没有生产部署权限，检索无数据";
                DataTable dt = new DataTable();
                if (vcType == "0")
                {
                    dt = logic.SearchData(vcMon, vcTF, vcTO, vcType, vcPartsno, vcDock, vcPorType, vcOrder, vcPlant, vcED);
                }
                if (vcType == "1")
                {
                    dt = logic.SearchItemData(vcMon, vcTF, vcTO, vcType, vcPartsno, vcDock, vcSerial, vcOrder, vcPorType, vcPlant, vcED);
                    if (dt.Rows.Count > 0)
                    {
                        if (dt.Columns.Contains("otype")) dt.Columns.Remove("otype");
                        if (dt.Columns.Contains("vcEDflag")) dt.Columns["vcEDflag"].SetOrdinal(3);
                        if (dt.Columns.Contains("chkFlag")) dt.Columns.Remove("chkFlag");
                    }
                }
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
        public string exportApi1([FromBody] dynamic data)
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
            string vcPorType = dataForm.vcPorType;
            string vcMon = dataForm.vcMon;
            string vcTF = dataForm.vcTF;
            string vcTO = dataForm.vcTO;
            string vcType = dataForm.vcType;
            string vcPartsno = dataForm.vcPartsno;
            string vcDock = dataForm.vcDock;
            string vcOrder = dataForm.vcOrder;
            string vcSerial = dataForm.vcSerial;
            string vcED = dataForm.vcED;
            try
            {
                DataTable dt = logic.SearchData(vcMon, vcTF, vcTO, vcType, vcPartsno, vcDock, vcPorType, vcOrder, vcPlant, vcED);
                string[] fields = { "vcMonth",
                     "vcPartsNo",
                     "vcCarType",
                     "vcDock",
                     "vcEDflag",
                     "vcMonTotal",
                     "vcActTotal",
                     "TD1b",
                     "TD1bA",
                     "TD1y",
                     "TD1yA",
                     "TD2b",
                     "TD2bA",
                     "TD2y",
                     "TD2yA",
                     "TD3b",
                     "TD3bA",
                     "TD3y",
                     "TD3yA",
                     "TD4b",
                     "TD4bA",
                     "TD4y",
                     "TD4yA",
                     "TD5b",
                     "TD5bA",
                     "TD5y",
                     "TD5yA",
                     "TD6b",
                     "TD6bA",
                     "TD6y",
                     "TD6yA",
                     "TD7b",
                     "TD7bA",
                     "TD7y",
                     "TD7yA",
                     "TD8b",
                     "TD8bA",
                     "TD8y",
                     "TD8yA",
                     "TD9b",
                     "TD9bA",
                     "TD9y",
                     "TD9yA",
                     "TD10b",
                     "TD10bA",
                     "TD10y",
                     "TD10yA",
                     "TD11b",
                     "TD11bA",
                     "TD11y",
                     "TD11yA",
                     "TD12b",
                     "TD12bA",
                     "TD12y",
                     "TD12yA",
                     "TD13b",
                     "TD13bA",
                     "TD13y",
                     "TD13yA",
                     "TD14b",
                     "TD14bA",
                     "TD14y",
                     "TD14yA",
                     "TD15b",
                     "TD15bA",
                     "TD15y",
                     "TD15yA",
                     "TD16b",
                     "TD16bA",
                     "TD16y",
                     "TD16yA",
                     "TD17b",
                     "TD17bA",
                     "TD17y",
                     "TD17yA",
                     "TD18b",
                     "TD18bA",
                     "TD18y",
                     "TD18yA",
                     "TD19b",
                     "TD19bA",
                     "TD19y",
                     "TD19yA",
                     "TD20b",
                     "TD20bA",
                     "TD20y",
                     "TD20yA",
                     "TD21b",
                     "TD21bA",
                     "TD21y",
                     "TD21yA",
                     "TD22b",
                     "TD22bA",
                     "TD22y",
                     "TD22yA",
                     "TD23b",
                     "TD23bA",
                     "TD23y",
                     "TD23yA",
                     "TD24b",
                     "TD24bA",
                     "TD24y",
                     "TD24yA",
                     "TD25b",
                     "TD25bA",
                     "TD25y",
                     "TD25yA",
                     "TD26b",
                     "TD26bA",
                     "TD26y",
                     "TD26yA",
                     "TD27b",
                     "TD27bA",
                     "TD27y",
                     "TD27yA",
                     "TD28b",
                     "TD28bA",
                     "TD28y",
                     "TD28yA",
                     "TD29b",
                     "TD29bA",
                     "TD29y",
                     "TD29yA",
                     "TD30b",
                     "TD30bA",
                     "TD30y",
                     "TD30yA",
                     "TD31b",
                     "TD31bA",
                     "TD31y",
                     "TD31yA",

                     "ED1b",
                     "ED1bA",
                     "ED1y",
                     "ED1yA",
                     "ED2b",
                     "ED2bA",
                     "ED2y",
                     "ED2yA",
                     "ED3b",
                     "ED3bA",
                     "ED3y",
                     "ED3yA",
                     "ED4b",
                     "ED4bA",
                     "ED4y",
                     "ED4yA",
                     "ED5b",
                     "ED5bA",
                     "ED5y",
                     "ED5yA",
                     "ED6b",
                     "ED6bA",
                     "ED6y",
                     "ED6yA",
                     "ED7b",
                     "ED7bA",
                     "ED7y",
                     "ED7yA",
                     "ED8b",
                     "ED8bA",
                     "ED8y",
                     "ED8yA",
                     "ED9b",
                     "ED9bA",
                     "ED9y",
                     "ED9yA",
                     "ED10b",
                     "ED10bA",
                     "ED10y",
                     "ED10yA",
                     "ED11b",
                     "ED11bA",
                     "ED11y",
                     "ED11yA",
                     "ED12b",
                     "ED12bA",
                     "ED12y",
                     "ED12yA",
                     "ED13b",
                     "ED13bA",
                     "ED13y",
                     "ED13yA",
                     "ED14b",
                     "ED14bA",
                     "ED14y",
                     "ED14yA",
                     "ED15b",
                     "ED15bA",
                     "ED15y",
                     "ED15yA",
                     "ED16b",
                     "ED16bA",
                     "ED16y",
                     "ED16yA",
                     "ED17b",
                     "ED17bA",
                     "ED17y",
                     "ED17yA",
                     "ED18b",
                     "ED18bA",
                     "ED18y",
                     "ED18yA",
                     "ED19b",
                     "ED19bA",
                     "ED19y",
                     "ED19yA",
                     "ED20b",
                     "ED20bA",
                     "ED20y",
                     "ED20yA",
                     "ED21b",
                     "ED21bA",
                     "ED21y",
                     "ED21yA",
                     "ED22b",
                     "ED22bA",
                     "ED22y",
                     "ED22yA",
                     "ED23b",
                     "ED23bA",
                     "ED23y",
                     "ED23yA",
                     "ED24b",
                     "ED24bA",
                     "ED24y",
                     "ED24yA",
                     "ED25b",
                     "ED25bA",
                     "ED25y",
                     "ED25yA",
                     "ED26b",
                     "ED26bA",
                     "ED26y",
                     "ED26yA",
                     "ED27b",
                     "ED27bA",
                     "ED27y",
                     "ED27yA",
                     "ED28b",
                     "ED28bA",
                     "ED28y",
                     "ED28yA",
                     "ED29b",
                     "ED29bA",
                     "ED29y",
                     "ED29yA",
                     "ED30b",
                     "ED30bA",
                     "ED30y",
                     "ED30yA",
                     "ED31b",
                     "ED31bA",
                     "ED31y",
                     "ED31yA"
                 };
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS1211_ALLExport.xlsx", 1, loginInfo.UserId, FunctionID);
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
        [HttpPost]
        [EnableCors("any")]
        public string exportApi2([FromBody] dynamic data)
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
            string vcPorType = dataForm.vcPorType;
            string vcMon = dataForm.vcMon;
            string vcTF = dataForm.vcTF;
            string vcTO = dataForm.vcTO;
            string vcType = dataForm.vcType;
            string vcPartsno = dataForm.vcPartsno;
            string vcDock = dataForm.vcDock;
            string vcOrder = dataForm.vcOrder;
            string vcSerial = dataForm.vcSerial;
            string vcED = dataForm.vcED;
            try
            {
                DataTable dt = logic.SearchItemData(vcMon, vcTF, vcTO, vcType, vcPartsno, vcDock, vcSerial, vcOrder, vcPorType, vcPlant, vcED);
                if (dt.Rows.Count > 0)
                {
                    if (dt.Columns.Contains("otype")) dt.Columns.Remove("otype");
                    if (dt.Columns.Contains("vcEDflag")) dt.Columns["vcEDflag"].SetOrdinal(3);
                    if (dt.Columns.Contains("chkFlag")) dt.Columns.Remove("chkFlag");
                }
                string[] fields = { "partsno","cpdcompany","dock","vcEDflag","inno","quantity","kanbanorderno","kanbanserial","packingcondition","packingspot",
                                    "scandatetimeht","htuser","htno","daddtime","cupduser"
                };
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS1211_ItemExport.xlsx", 1, loginInfo.UserId, FunctionID);
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
