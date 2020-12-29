using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Text.RegularExpressions;
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

namespace SPPSApi.Controllers.G06
{
    [Route("api/FS0623/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0623Controller : BaseController
    {
        FS0623_Logic fs0623_Logic = new FS0623_Logic();
        private readonly string FunctionID = "FS0623";

        private readonly IWebHostEnvironment _webHostEnvironment;


        public FS0623Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        #region 检索
        [HttpPost]
        [EnableCors("any")]
        public string searchApi([FromBody] dynamic data)
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
            string vcTargetYear = dataForm.vcTargetYear == null ? "" : dataForm.vcTargetYear;
            string vcPartNo = dataForm.vcPartNo == null ? "" : dataForm.vcPartNo;
            string vcInjectionFactory = dataForm.vcInjectionFactory == null ? "" : dataForm.vcInjectionFactory;
            string vcInsideOutsideType = dataForm.vcInsideOutsideType == null ? "" : dataForm.vcInsideOutsideType;
            string vcSupplier_id = dataForm.vcSupplier_id == null ? "" : dataForm.vcSupplier_id;
            string vcWorkArea = dataForm.vcWorkArea == null ? "" : dataForm.vcWorkArea;
            string vcCarType = dataForm.vcCarType == null ? "" : dataForm.vcCarType;
            try
            {
                DataTable dt = fs0623_Logic.Search(vcTargetYear, vcPartNo, vcInjectionFactory, vcInsideOutsideType, vcSupplier_id, vcWorkArea, vcCarType);
                // "vcPackPlant", "vcTargetYear", "vcPartNo", "vcInjectionFactory", "vcInsideOutsideType", "vcSupplier_id", "vcWorkArea", "vcCarType",
                List<Object> dataList = ComFunction.convertToResult(dt, new string[] { "iAutoId","vcDock","vcCarType", "dBeginDate", "dEndDate", 
                "vcmodflag","vcaddflag"});
                for (int i = 0; i < dataList.Count; i++)
                {
                    //vcRead vcWrite字段需要从 0 1转换成false true
                    Dictionary<string, object> row = (Dictionary<string, object>)dataList[i];
                    row["vcmodflag"] = row["vcmodflag"].ToString() == "1" ? true : false;
                    row["vcaddflag"] = row["vcaddflag"].ToString() == "1" ? true : false;
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2103", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

    }
}

