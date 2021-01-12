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

namespace SPPSApi.Controllers.G12
{
    [Route("api/FS1209/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1209Controller : BaseController
    {
        FS1209_Logic logic = new FS1209_Logic();
        private readonly string FunctionID = "FS1209";
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FS1209Controller(IWebHostEnvironment webHostEnvironment)
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
                List<Object> dataList_PlantSource = ComFunction.convertAllToResult(logic.dllPorPlant());
                string[] userPorType = null;
                List<Object> dataList_PorTypeSource = ComFunction.convertAllToResult(logic.dllPorType(loginInfo.UserId,ref userPorType));
                string printerName = logic.PrintMess(loginInfo.UserId);
                res.Add("dataList_PlantSource", dataList_PlantSource);
                res.Add("dataList_PorTypeSource", dataList_PorTypeSource);
                res.Add("printerName", printerName);
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
            string vcType = dataForm.vcType;
            string vcPrintPartNo = dataForm.vcPrintPartNo;
            string vcLianFan = dataForm.vcLianFan;
            string vcPorPlant = dataForm.vcPorPlant;
            string vcKbOrderId = dataForm.vcKbOrderId;
            string vcPorType = dataForm.vcPorType;
            vcType = vcType == null ? "" : vcType;
            vcPrintPartNo = vcPrintPartNo == null ? "" : vcPrintPartNo;
            vcLianFan = vcLianFan == null ? "" : vcLianFan;
            vcPorPlant = vcPorPlant == null ? "" : vcPorPlant;
            vcKbOrderId = vcKbOrderId == null ? "" : vcKbOrderId;
            vcPorType = vcPorType == null ? "" : vcPorType;
            try
            {
                string[] userPorType = null;
                DataTable dtportype = logic.dllPorType(loginInfo.UserId, ref userPorType);
                if ("PP".Equals(vcPorType))
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "没有生产部署权限，检索无数据";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                DataTable resutPrint;
                if (vcType == "3")
                {
                    resutPrint = logic.searchPrint(vcPrintPartNo, vcKbOrderId, vcLianFan, vcPorType, vcPorPlant, dtportype);
                }
                else
                {
                    resutPrint = logic.searchPrint(vcPrintPartNo, vcType, vcKbOrderId, vcLianFan, vcPorType, vcPorPlant, dtportype);
                }
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                List<Object> dataList = ComFunction.convertAllToResultByConverter(resutPrint, dtConverter);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
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
        #endregion

        #region 打印
        [HttpPost]
        [EnableCors("any")]
        //public string BtnPrintAll(DataTable dt, string vcType, string printerName)
        public string printdataApi([FromBody] dynamic data)
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
            string vcType = dataForm.vcType;
            string vcPrintPartNo = dataForm.vcPrintPartNo;
            string vcLianFan = dataForm.vcLianFan;
            string vcPorPlant = dataForm.vcPorPlant;
            string vcKbOrderId = dataForm.vcKbOrderId;
            string vcPorType = dataForm.vcPorType;    
            string printerName = dataForm.printerName;
            vcType = vcType == null ? "" : vcType;
            vcPrintPartNo = vcPrintPartNo == null ? "" : vcPrintPartNo;
            vcLianFan = vcLianFan == null ? "" : vcLianFan;
            vcPorPlant = vcPorPlant == null ? "" : vcPorPlant;
            vcKbOrderId = vcKbOrderId == null ? "" : vcKbOrderId;
            vcPorType = vcPorType == null ? "" : vcPorType;
            printerName = printerName == null ? "" : printerName;
            try
            {
                string[] userPorType = null;
                DataTable dtportype = logic.dllPorType(loginInfo.UserId, ref userPorType);
                if ("PP".Equals(vcPorType))
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "没有生产部署权限，检索无数据";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                DataTable printTable;
                if (vcType == "3")
                {
                    printTable = logic.searchPrint(vcPrintPartNo, vcKbOrderId, vcLianFan, vcPorType, vcPorPlant, dtportype);
                }
                else
                {
                    printTable = logic.searchPrint(vcPrintPartNo, vcType, vcKbOrderId, vcLianFan, vcPorType, vcPorPlant, dtportype);
                }
                if (null == printTable || null == vcType || null == printerName)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "参数不能为空";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (printTable.Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "无检索数据,无法打印";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //FS1209_Logic fS1209_Logic = new FS1209_Logic();
                string userid = loginInfo.UserId;
                DataTable dtPorType = new DataTable();
                //if (ActionContext.Request.Properties.ContainsKey("userId") && ActionContext.Request.Properties["userId"] != null)
                //{
                //    userid = ActionContext.Request.Properties["userId"].ToString();
                //}
                //else
                //{
                //    userid = "admin";
                //}
                try
                {
                    logic.BtnPrintAll(printTable, vcType, printerName, userid, ref dtPorType);
                }
                catch (System.Exception e)
                {
                    for (int i = 0; i < dtPorType.Rows.Count; i++)
                    {
                        //fS1209_Logic.DeleteprinterCREX(dtPorType.Rows[i]["vcPorType"].ToString(), dtPorType.Rows[i]["vcorderno"].ToString(), dtPorType.Rows[i]["vcComDate01"].ToString(), dtPorType.Rows[i]["vcBanZhi01"].ToString());
                    }
                    throw new Exception("看板打印异常:" + e.Message);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "打印成功";
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
        #endregion
    }
}