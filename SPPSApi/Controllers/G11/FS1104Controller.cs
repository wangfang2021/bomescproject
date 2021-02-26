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
                List<Object> ReceiverList = ComFunction.convertAllToResult(fS0603_Logic.getCodeInfo("Receiver"));//收货方
                List<Object> OrderPlantList = ComFunction.convertAllToResult(ComFunction.getTCode("C000"));//发注工厂
                List<Object> PackingPlantList = ComFunction.convertAllToResult(ComFunction.getTCode("C017"));//包装厂

                res.Add("ReceiverList", ReceiverList);
                res.Add("OrderPlantList", OrderPlantList);
                res.Add("PackingPlantList", PackingPlantList);

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
        public string selectApi([FromBody]dynamic data)
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

            string strOrderPlant = dataForm.selectVaule.OrderPlant == null ? "" : dataForm.selectVaule.OrderPlant;
            string strReceiver = dataForm.selectVaule.Receiver == null ? "" : dataForm.selectVaule.Receiver;
            string strPackingPlant = dataForm.selectVaule.PackingPlant == null ? "" : dataForm.selectVaule.PackingPlant;
            try
            {
                Dictionary<string, object> res = new Dictionary<string, object>();
                DataTable dtMessage = fS0603_Logic.createTable("MES");

                if (strOrderPlant == "" || strReceiver == "" || strPackingPlant == "")
                {
                    res.Add("LianFanItem", "");
                }
                else
                {
                    string strCaseLianFanNo = fS1104_Logic.getCaseNoInfo(strOrderPlant, strReceiver, strPackingPlant, "");
                    res.Add("LianFanItem", strCaseLianFanNo);
                }
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

            string strOrderPlant = dataForm.OrderPlant == null ? "" : dataForm.OrderPlant;
            string strReceiver = dataForm.Receiver == null ? "" : dataForm.Receiver;
            string strPackingPlant = dataForm.PackingPlant == null ? "" : dataForm.PackingPlant;
            string strLianFan = dataForm.LianFan == null ? "" : dataForm.LianFan;
            string strPrintNum = dataForm.PrintNum == null ? "" : dataForm.PrintNum;
            string strPrintCopy = dataForm.PrintCopy == null ? "" : dataForm.PrintCopy;
            try
            {
                DataTable dtMessage = fS0603_Logic.createTable("MES");

                if (strOrderPlant == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "发注工厂不能为空";
                    dtMessage.Rows.Add(dataRow);
                }
                if (strReceiver == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "收货方不能为空";
                    dtMessage.Rows.Add(dataRow);
                }
                if (strPackingPlant == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "包装工不能为空";
                    dtMessage.Rows.Add(dataRow);
                }
                if (strLianFan == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "连番不能为空";
                    dtMessage.Rows.Add(dataRow);
                }
                if (strPrintNum == "" || strPrintNum == "0")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "打印数量不能为空(0)";
                    dtMessage.Rows.Add(dataRow);
                }
                if (strPrintCopy == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "份数不能为空";
                    dtMessage.Rows.Add(dataRow);
                }
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                DataTable dataTable = fS1104_Logic.setPrintInfo(strOrderPlant, strReceiver, strPackingPlant, strLianFan, strPrintNum, "", strPrintCopy);
                //======================打印方法=======================

                //=====================================================
                fS1104_Logic.setSaveInfo(dataTable, ref dtMessage);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "打印成功";
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
        public string secondprintApi([FromBody]dynamic data)
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
                DataTable dtMessage = fS0603_Logic.createTable("MES");
                if (strCaseNo == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "箱号不能为空";
                    dtMessage.Rows.Add(dataRow);
                }
                if (strPrintCopy == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "份数不能为空";
                    dtMessage.Rows.Add(dataRow);
                }
                //======================打印方法=======================


                //=====================================================
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "打印成功";
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
