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
    [Route("api/FS0627/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0627Controller : BaseController
    {
        FS0627_Logic fs0627_Logic = new FS0627_Logic();
        private readonly string FunctionID = "FS0627";

        private readonly IWebHostEnvironment _webHostEnvironment;


        public FS0627Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        #region 绑定 发注工厂直接从表中数据
        [HttpPost]
        [EnableCors("any")]
        public string bindInjectionFactoryApi()
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
                Dictionary<string, object> res = new Dictionary<string, object>();
                List<Object> dataList_C000 = ComFunction.convertAllToResult(ComFunction.getTCode("C000"));//发注工厂
                res.Add("C000", dataList_C000);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2101", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "绑定发注工厂列表失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 绑定内外
        [HttpPost]
        [EnableCors("any")]
        public string bindInsideOutsideType()
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
                DataTable dt = fs0627_Logic.BindInsideOutsideType();
                List<Object> dataList = ComFunction.convertToResult(dt, new string[] { "vcCodeId", "vcCodeName" });
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2102", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "绑定内外列表失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

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
            string vcInjectionFactory = dataForm.vcInjectionFactory == null ? "" : dataForm.vcInjectionFactory;
            string vcProject = dataForm.vcProject == null ? "" : dataForm.vcProject;
            string vcTargetYear = dataForm.vcTargetYear == null ? "" : dataForm.vcTargetYear;
            try
            {
                DataSet ds = fs0627_Logic.Search(vcInjectionFactory, vcProject, vcTargetYear);
                DataTable dtNum = ds.Tables[0];
                DataTable dtMonty = ds.Tables[1];
                // "vcPackPlant", "vcTargetYear", "vcPartNo", "vcInjectionFactory", "vcInsideOutsideType", "vcSupplier_id", "vcWorkArea", "vcCarType",
                List<Object> dataListNum = ComFunction.convertAllToResult(dtNum);
                List<Object> dataListMoney = ComFunction.convertAllToResult(dtMonty);
                var obbject = new object[] {
                    new { listNum=dataListNum},
                    new { listMoney=dataListMoney}
                };
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = obbject;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2703", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

    }
}

