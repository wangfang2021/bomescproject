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
    [Route("api/FS0630_Sub_Cal/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0630_Sub_CalController : BaseController
    {
        FS0630_Logic fs0630_Logic = new FS0630_Logic();
        private readonly string FunctionID = "FS0630";

        private readonly IWebHostEnvironment _webHostEnvironment;

        public FS0630_Sub_CalController(IWebHostEnvironment webHostEnvironment)
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
                string vcDXYM = System.DateTime.Now.AddMonths(1).ToString("yyyy-MM-dd");
                string vcNSYM = System.DateTime.Now.AddMonths(2).ToString("yyyy-MM-dd");
                string vcNNSYM = System.DateTime.Now.AddMonths(3).ToString("yyyy-MM-dd");
                res.Add("vcCLYM", vcCLYM);
                res.Add("vcDXYM", vcDXYM);
                res.Add("vcNSYM", vcNSYM);
                res.Add("vcNNSYM", vcNNSYM);

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
                string vcDXYM = dataForm.vcDXYM == null ? "" : dataForm.vcDXYM;
                string vcNSYM = dataForm.vcNSYM == null ? "" : dataForm.vcNSYM;
                string vcNNSYM = dataForm.vcNNSYM == null ? "" : dataForm.vcNNSYM;

                vcCLYM = vcCLYM == "" ? "" : vcCLYM.Replace("-", "").Substring(0, 6);
                vcDXYM = vcDXYM == "" ? "" : vcDXYM.Replace("-", "").Substring(0, 6);
                vcNSYM = vcNSYM == "" ? "" : vcNSYM.Replace("-", "").Substring(0, 6);
                vcNNSYM = vcNNSYM == "" ? "" : vcNNSYM.Replace("-", "").Substring(0, 6);

                List<string> lsdxym = new List<string>();

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
                if (string.IsNullOrEmpty(vcDXYM))
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "对象年月不能为空。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //之前说内示年月和内内示年月不能为空，但是global系统要求：1个处理月必须对应3个对象月的数据
                if (string.IsNullOrEmpty(vcNSYM))
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "内示年月不能为空。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (string.IsNullOrEmpty(vcNNSYM))
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "内内示年月不能为空。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //对象年月=处理年月+1
                string vcDXYM_temp = Convert.ToDateTime(vcCLYM.Substring(0, 4) + "-" + vcCLYM.Substring(4, 2) + "-" + "01").AddMonths(1).ToString("yyyyMM");
                if (vcDXYM != vcDXYM_temp)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = string.Format("对象年月只能是{0}。", vcDXYM_temp);
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                lsdxym.Add(vcDXYM);
                //内示年月=对象年月+1
                if (string.IsNullOrEmpty(vcNSYM) == false)
                {
                    string vcNSYM_temp = Convert.ToDateTime(vcDXYM.Substring(0, 4) + "-" + vcDXYM.Substring(4, 2) + "-" + "01").AddMonths(1).ToString("yyyyMM");
                    if (vcNSYM != vcNSYM_temp)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = string.Format("内示年月只能是{0}。", vcNSYM_temp);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    lsdxym.Add(vcNSYM);
                }
                //内内示年月=内示年月+1
                if (string.IsNullOrEmpty(vcNNSYM) == false)
                {
                    if (string.IsNullOrEmpty(vcNSYM) == false)
                    {
                        string vcNNSYM_temp = Convert.ToDateTime(vcNSYM.Substring(0, 4) + "-" + vcNSYM.Substring(4, 2) + "-" + "01").AddMonths(1).ToString("yyyyMM");
                        if (vcNNSYM != vcNNSYM_temp)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = string.Format("内内示年月只能是{0}。", vcNNSYM_temp);
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        lsdxym.Add(vcNNSYM);
                    }
                    else
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = string.Format("内示年月不能为空。");
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                //记录请求时间
                fs0630_Logic.CreateView(vcCLYM, plantList, lsdxym, loginInfo.UserId);

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
