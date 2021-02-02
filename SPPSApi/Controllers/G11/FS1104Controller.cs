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

namespace SPPSApi.Controllers.G11
{
    [Route("api/FS1104/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1104Controller : BaseController
    {
        FS0603_Logic fS0603_Logic = new FS0603_Logic();
        FS1104_Logic fS1104_Logic = new FS1104_Logic();
        FS0617_Logic fS0617_Logic = new FS0617_Logic();
        private readonly string FunctionID = "FS1104";

        private readonly IWebHostEnvironment _webHostEnvironment;
        public FS1104Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        /// <summary>
        /// 页面初始化
        /// </summary>
        /// <returns></returns>
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
                //处理初始化
                List<Object> PlantList = ComFunction.convertAllToResult(ComFunction.getTCode("C000"));//发货方
                List<Object> RePartyList = ComFunction.convertAllToResult(fS0603_Logic.getCodeInfo("Receiver"));//客户代码
                List<Object> PackPlantList = ComFunction.convertAllToResult(ComFunction.getTCode("C023"));//包装厂
                res.Add("PlantList", PlantList);
                res.Add("RePartyList", RePartyList);
                res.Add("PackPlantList", PackPlantList);

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
        /// <summary>
        /// 刷新方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string refreshApi([FromBody]dynamic data)
        {
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            //以下开始业务处理
            ApiResult apiResult = new ApiResult();
            Dictionary<string, object> res = new Dictionary<string, object>();
            dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));

            string strPlant = dataForm.Plant == null ? "" : dataForm.Plant;
            string strReParty = dataForm.ReParty == null ? "" : dataForm.ReParty;
            string strPackPlant = dataForm.PackPlant == null ? "" : dataForm.PackPlant;
            try
            {
                if (strPlant == "" || strReParty == "" || strPackPlant == "")
                {
                    res.Add("CaseLianFanNoItem", "");
                }
                else
                {
                    res.Add("CaseLianFanNoItem", fS1104_Logic.getCaseLianFanInfo(strPlant, strReParty, strPackPlant));
                }
                res.Add("PlantItem", strPlant);
                res.Add("RePartyItem", strReParty);
                res.Add("PackPlantItem", strPackPlant);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0204", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 首次印刷方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string firstprintApi([FromBody]dynamic data)
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

            string strPlant = dataForm.Plant == null ? "" : dataForm.Plant;
            string strReParty = dataForm.ReParty == null ? "" : dataForm.ReParty;
            string strPackPlant = dataForm.PackPlant == null ? "" : dataForm.PackPlant;
            string strCaseLianFanNo = dataForm.CaseLianFanNo == null ? "" : dataForm.CaseLianFanNo;
            string strPrintNum = dataForm.PrintNum == null ? "" : dataForm.PrintNum;
            string strPrintCopy = dataForm.PrintCopy == null ? "" : dataForm.PrintCopy;
            try
            {
                //判断是否可
                strCaseLianFanNo = fS1104_Logic.getCaseLianFanInfo(strPlant, strReParty, strPackPlant);
                if (strPlant == "" || strReParty == "" || strPackPlant == "" || strCaseLianFanNo == "")
                {
                    //报错:箱号信息不完整
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "箱号信息不完整";
                }
                else
                {
                    if (strPrintNum == "" || strPrintNum == "0")
                    {
                        //报错:打印数量不能小于1
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "打印数量不能小于1";
                    }
                    else
                    {
                        if (strPrintCopy == "")
                        {
                            //报错:请选择打印份数
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "请选择打印份数";

                        }
                        else
                        {
                            DataTable dataPrint = fS1104_Logic.setPrintInfo(strPlant, strReParty, strPackPlant, strCaseLianFanNo, strPrintNum, "", strPrintCopy);
                            //打印

                            apiResult.code = ComConstant.SUCCESS_CODE;
                            apiResult.data = "打印成功";
                        }
                    }
                }
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0204", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "生成印刷文件失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 再发印刷方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string resetprintApi([FromBody]dynamic data)
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
            string strCaseNo = dataForm.CaseNo == null ? "" : dataForm.CaseNo;
            string strPrintCopy = dataForm.PrintCopy == null ? "" : dataForm.PrintCopy;
            try
            {
                if (strCaseNo == "")
                {
                    //报错:箱号信息不能为空
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "箱号信息不能为空";
                }
                else
                {
                    DataTable dataPrint = fS1104_Logic.setPrintInfo("", "", "", "", "", strCaseNo, strPrintCopy);
                    if (dataPrint.Rows.Count == 0)
                    {
                        //报错:无效箱号不能打印
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "无效箱号不能打印";
                    }
                    else
                    {
                        //打印

                        apiResult.code = ComConstant.SUCCESS_CODE;
                        apiResult.data = "打印成功";
                    }
                }
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0204", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "生成印刷文件失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
    }
}
