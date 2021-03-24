using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebServiceAPI;

namespace SPPSApi.Controllers.G08
{
    [Route("api/FS0805/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0805Controller : BaseController
    {
        FS0603_Logic fS0603_Logic = new FS0603_Logic();
        FS0805_Logic fS0805_Logic = new FS0805_Logic();
        private readonly string FunctionID = "FS0805";

        private readonly IWebHostEnvironment _webHostEnvironment;
        public FS0805Controller(IWebHostEnvironment webHostEnvironment)
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
                //无初始化交互
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
        /// 查询方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string searchApi([FromBody]dynamic data)
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
            Dictionary<string, object> res = new Dictionary<string, object>();

            string strSellNo = dataForm.SellNo == null ? "" : dataForm.SellNo;
            try
            {
                DataTable dtMessage = fS0603_Logic.createTable("MES");
                if (strSellNo == string.Empty)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = string.Format("销售单号不能为空");
                    dtMessage.Rows.Add(dataRow);
                }
                if (dtMessage.Rows.Count != 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                DataTable dataTable = fS0805_Logic.getSearchInfo(strSellNo);
                if (dataTable.Rows.Count == 0)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = string.Format("没有可供打印的数据，请确认销售单号");
                    dtMessage.Rows.Add(dataRow);
                }
                if (dtMessage.Rows.Count != 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("bSelectFlag", ConvertFieldType.BoolType, null);
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dataTable, dtConverter);
                res.Add("tempList", dataList);
                res.Add("SellNoItem", strSellNo);
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
        /// 印刷方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string printApi([FromBody]dynamic data)
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
            Dictionary<string, object> res = new Dictionary<string, object>();

            string strSellNo = dataForm.sellNo == null ? "" : dataForm.sellNo;
            try
            {
                DataTable dtMessage = fS0603_Logic.createTable("MES");
                if (strSellNo == string.Empty)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = string.Format("打印前请进行销售单查询");
                    dtMessage.Rows.Add(dataRow);
                }
                if (dtMessage.Rows.Count != 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                bool bResult = fS0805_Logic.getPrintInfo(strSellNo, loginInfo.UserId, ref dtMessage);
                if (!bResult)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                DataTable dtExport = fS0805_Logic.getPrintInfo(loginInfo.UserId);
                if (dtExport.Rows.Count != 0)
                {

                    //序号赋值
                    for (int j = 0; j < dtExport.Rows.Count; j++)
                    {
                        dtExport.Rows[j]["LinId"] = j + 1;
                    }
                    //导出到excel中
                    string[] fields = { "LinId", "vcPartsno", "vcOrderno", "vcSeqno", "vcInvoiceno", "vcPartsnamechn", "vcPartsnameen", "vcShippingqty", "vcCaseno", "vcCostwithtaxes", "vcPrice" };
                    string filepath = fS0805_Logic.generateExcelWithXlt(dtExport, fields, _webHostEnvironment.ContentRootPath, "FS0805_Print.xlsx", 0, 4, loginInfo.UserId, FunctionID);
                    #region 调用webApi打印
                    string strPrinterName = fS0603_Logic.getPrinterName("FS1101", loginInfo.UserId);
                    //创建 HTTP 绑定对象
                    var binding = new BasicHttpBinding();
                    //根据 WebService 的 URL 构建终端点对象
                    var endpoint = new EndpointAddress(@"http://172.23.238.179/WebAPI/WebServiceAPI.asmx");
                    //创建调用接口的工厂，注意这里泛型只能传入接口
                    var factory = new ChannelFactory<WebServiceAPISoap>(binding, endpoint);
                    //从工厂获取具体的调用实例
                    var callClient = factory.CreateChannel();
                    setExcelPrintRequestBody Body = new setExcelPrintRequestBody();
                    Body.strDiskFileName = filepath;
                    Body.strOperID = loginInfo.UserId;
                    Body.strPrinterName = strPrinterName;
                    //调用具体的方法，这里是 HelloWorldAsync 方法
                    Task<setExcelPrintResponse> responseTask = callClient.setExcelPrintAsync(new setExcelPrintRequest(Body));
                    //获取结果
                    setExcelPrintResponse response = responseTask.Result;
                    if (response.Body.setExcelPrintResult != "打印成功")
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = "打印失败，请联系管理员进行打印接口故障检查。";
                        dtMessage.Rows.Add(dataRow);
                    }
                    #endregion
                }
                else
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "出货明细书数据异常";
                    dtMessage.Rows.Add(dataRow);
                }
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                {
                    //弹出错误dtMessage
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
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
