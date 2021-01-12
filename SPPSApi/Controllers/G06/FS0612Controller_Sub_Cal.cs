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



namespace SPPSApi.Controllers.G06
{
    [Route("api/FS0612_Sub_Cal/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0612_Sub_CalController : BaseController
    {
        FS0612_Logic fs0612_Logic = new FS0612_Logic();
        private readonly string FunctionID = "FS0612";

        private readonly IWebHostEnvironment _webHostEnvironment;

        public FS0612_Sub_CalController(IWebHostEnvironment webHostEnvironment)
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
                //if (loginInfo.Special == "财务用户")
                //    res.Add("caiWuBtnVisible", false);
                //else
                //    res.Add("caiWuBtnVisible", true);

                List<Object> dataList_C000 = ComFunction.convertAllToResult(ComFunction.getTCode("C000"));//工厂
                res.Add("C000", dataList_C000);

                string vcCLYM = System.DateTime.Now.ToString("yyyy-MM-dd");
                res.Add("vcCLYM", vcCLYM);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M08UE0701", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "初始化失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 确定
        [HttpPost]
        [EnableCors("any")]
        public string OKApi([FromBody] dynamic data)
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
                JArray vcPlants = null;
                List<string> plantList = null;
                try
                {
                    vcPlants = dataForm.vcPlant;
                    plantList = vcPlants.ToObject<List<string>>();
                }
                catch (Exception ex)
                {

                }
                string vcCLYM = dataForm.vcCLYM == null ? "" : dataForm.vcCLYM;
                vcCLYM = vcCLYM == "" ? "" : vcCLYM.Replace("-", "").Substring(0, 6);
                if (string.IsNullOrEmpty(vcCLYM))
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "处理年月不能为空。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (plantList == null || plantList.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "工厂不能为空。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //记录请求时间
                fs0612_Logic.CreateView(vcCLYM, plantList, loginInfo.UserId);

                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UI0103", null, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0201", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "请求失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

    }
}
