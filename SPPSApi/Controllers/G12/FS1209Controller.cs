using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using System.Threading;
using Common;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using Logic;

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
                string RolePorType = logic.getRoleTip(loginInfo.UserId);
                DataTable dtportype = logic.dllPorType(RolePorType.Split('*'));
                List<Object> dataList_PorTypeSource = ComFunction.convertAllToResult(dtportype);

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
            string vcType = dataForm.vcType == null ? "" : dataForm.vcType;
            string vcPrintPartNo = dataForm.vcPrintPartNo == null ? "" : dataForm.vcPrintPartNo;
            string vcLianFan = dataForm.vcLianFan == null ? "" : dataForm.vcLianFan;
            string vcPorPlant = dataForm.vcPorPlant == null ? "" : dataForm.vcPorPlant;
            string vcKbOrderId = dataForm.vcKbOrderId == null ? "" : dataForm.vcKbOrderId;
            string vcPorType = dataForm.vcPorType == null ? "" : dataForm.vcPorType;
            try
            {
                DataTable resutPrint = logic.Search(vcType, vcPrintPartNo, vcLianFan, vcPorPlant, vcKbOrderId, vcPorType, loginInfo.UserId);
                DtConverter dtConverter = new DtConverter();
                //dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                //dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
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
        public string printdataApi([FromBody] dynamic data)
        {
            #region 
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
            string vcType = dataForm.vcType == null ? "" : dataForm.vcType;
            string vcPrintPartNo = dataForm.vcPrintPartNo == null ? "" : dataForm.vcPrintPartNo;
            string vcLianFan = dataForm.vcLianFan == null ? "" : dataForm.vcLianFan;
            string vcPorPlant = dataForm.vcPorPlant == null ? "" : dataForm.vcPorPlant;
            string vcKbOrderId = dataForm.vcKbOrderId == null ? "" : dataForm.vcKbOrderId;
            string vcPorType = dataForm.vcPorType == null ? "" : dataForm.vcPorType;
            string printerName = logic.PrintMess(loginInfo.UserId);
            try
            {
                //先检索
                DataTable printTable = logic.Search(vcType, vcPrintPartNo, vcLianFan, vcPorPlant, vcKbOrderId, vcPorType, loginInfo.UserId);
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

                string s = logic.BtnPrintAll(printTable, vcType, "", printerName, loginInfo.UserId);

                //BasicHttpBinding binding = new BasicHttpBinding();
                //binding.CloseTimeout = TimeSpan.MaxValue;
                //binding.OpenTimeout = TimeSpan.MaxValue;
                //binding.ReceiveTimeout = TimeSpan.MaxValue;
                //binding.SendTimeout = TimeSpan.MaxValue;
                //EndpointAddress address = new EndpointAddress("http://localhost:63480/PrintTable.asmx");
                //PrintTableSoapClient client = new PrintTableSoapClient(binding, address);
                //Task<PrinterResponse> responseTask = client.PrinterAsync("test20210205", "\\\\172.23.129.181\\刷卡打印机黑白", "C:\\inetpub\\SPPSPrint\\Test.rpt", "172.23.140.169", "SPPSdb", "sa", "Sa123");
                //PrinterResponse response = responseTask.Result;
                //// 获取HelloWorld方法的返回值
                //return response.Body.PrinterResult;

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
            #endregion
        }
        #endregion
    }
}