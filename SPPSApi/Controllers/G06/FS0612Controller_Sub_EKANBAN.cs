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
    [Route("api/FS0612_Sub_EKANBAN/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0612_Sub_EKANBANController : BaseController
    {
        FS0612_Logic fs0612_Logic = new FS0612_Logic();
        FS0630_Logic fs0630_Logic = new FS0630_Logic();
        private readonly string FunctionID = "FS0612";

        private readonly IWebHostEnvironment _webHostEnvironment;

        public FS0612_Sub_EKANBANController(IWebHostEnvironment webHostEnvironment)
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
                //判断内制结果是否已处理完
                DataTable dtNQCResult = fs0612_Logic.dtNQCReceive(vcCLYM);
                for (int i = 0; i < plantList.Count; i++)
                {
                    DataRow[] drs_dxny = dtNQCResult.Select("Process_Factory='TFTM" + plantList[i].ToString() + "' and Start_date_for_daily_qty like '" + vcDXYM + "%'  ");
                    DataRow[] drs_nsny = dtNQCResult.Select("Process_Factory='TFTM" + plantList[i].ToString() + "' and Start_date_for_daily_qty like '" + vcNSYM + "%'  ");
                    DataRow[] drs_nnsny = dtNQCResult.Select("Process_Factory='TFTM" + plantList[i].ToString() + "' and Start_date_for_daily_qty like '" + vcNNSYM + "%'  ");
                    if (drs_dxny.Length == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = string.Format("工厂{0},对象年月{1}内制结果未处理。", plantList[i].ToString(), vcDXYM);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (drs_nsny.Length == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = string.Format("工厂{0},内示年月{1}内制结果未处理。", plantList[i].ToString(), vcNSYM);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (drs_nnsny.Length == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = string.Format("工厂{0},内内示年月{1}内制结果未处理。", plantList[i].ToString(), vcNNSYM);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                //判断外注平准化数据是否展开
                DataTable dtSOQReply = fs0630_Logic.GetSOQReply(vcCLYM,"1");
                for (int i = 0; i < plantList.Count; i++)
                {
                    DataRow[] drs_dxny = dtSOQReply.Select("vcFZGC='" + plantList[i].ToString() + "' and vcDXYM='" + vcDXYM + "'  ");
                    DataRow[] drs_nsny = dtSOQReply.Select("vcFZGC='" + plantList[i].ToString() + "' and vcDXYM='" + vcNSYM + "'  ");
                    DataRow[] drs_nnsny = dtSOQReply.Select("vcFZGC='" + plantList[i].ToString() + "' and vcDXYM='" + vcNNSYM + "'  ");
                    if (drs_dxny.Length == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = string.Format("工厂{0},对象年月{1}外注平准化数据没有展开。", plantList[i].ToString(), vcDXYM);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (drs_nsny.Length == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = string.Format("工厂{0},内示年月{1}外注平准化数据没有展开。", plantList[i].ToString(), vcNSYM);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (drs_nnsny.Length == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = string.Format("工厂{0},内内示年月{1}外注平准化数据没有展开。", plantList[i].ToString(), vcNNSYM);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                //判断最大次数合算能不能再次请求，如果请求了但是没有回复则不能再次请求
                DataTable dtMaxCLResult = fs0612_Logic.GetMaxCLResult2(vcCLYM);
                for (int i = 0; i < plantList.Count; i++)
                {
                    string strPlant = plantList[i];
                    DataRow[] drs_dxny = dtMaxCLResult.Select("vcPlant='" + strPlant + "' and vcStatus='已请求'  ");
                    if (drs_dxny.Length > 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = string.Format("工厂{0}不能再次请求。", strPlant);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                //记录请求时间
                fs0612_Logic.CreateView2(vcCLYM, plantList, loginInfo.UserId);

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
