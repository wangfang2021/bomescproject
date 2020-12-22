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
                if ("PP".Equals(vcPorType))
                    return "没有生产部署权限，检索无数据";
                DataTable dt;
                if (vcType == "0")
                {
                    dt = logic.SearchData(vcMon, vcTF, vcTO, vcType, vcPartsno, vcDock, vcPorType, vcOrder, vcPlant, vcED);
                }
                else
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
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);


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

    }
}
