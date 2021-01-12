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
    [Route("api/FS1210_ensure/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1210Controller_ensure : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        FS1210_Logic logic = new FS1210_Logic();
        private readonly string FunctionID = "FS1210";

        public FS1210Controller_ensure(IWebHostEnvironment webHostEnvironment)
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
                FS1209_Logic logic_1 = new FS1209_Logic();
                string[] userPorType = null;
                List<Object> dataList_PorType = ComFunction.convertAllToResult(logic_1.dllPorType(loginInfo.UserId, ref userPorType));
                res.Add("PorTypeSource", dataList_PorType);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}
