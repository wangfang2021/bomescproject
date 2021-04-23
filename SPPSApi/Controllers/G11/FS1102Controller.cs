using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
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

namespace SPPSApi.Controllers.G11
{
    [Route("api/FS1102/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1102Controller : BaseController
    {
        FS0603_Logic fS0603_Logic = new FS0603_Logic();
        FS1102_Logic fS1102_Logic = new FS1102_Logic();
        FS0617_Logic fS0617_Logic = new FS0617_Logic();
        private readonly string FunctionID = "FS1102";

        private readonly IWebHostEnvironment _webHostEnvironment;
        public FS1102Controller(IWebHostEnvironment webHostEnvironment)
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
                res.Add("ReceiverList", ReceiverList);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M11PE0200", ex, loginInfo.UserId);
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
            Dictionary<string, object> res = new Dictionary<string, object>();
            dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));

            string strReceiver = dataForm.Receiver == null ? "" : dataForm.Receiver;
            string strCaseNo = dataForm.CaseNo == null ? "" : dataForm.CaseNo;
            string strTagId = dataForm.TagId == null ? "" : dataForm.TagId;
            try
            {
                DataTable dtMessage = fS0603_Logic.createTable("MES");
                if (strReceiver == "" && strCaseNo == "" && strTagId == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "请输入至少一个检索条件";
                    dtMessage.Rows.Add(dataRow);
                }
                if (dtMessage.Rows.Count != 0)
                {
                    //弹出错误dtMessage
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                DataTable dataTable = fS1102_Logic.getSearchInfo(strReceiver, strCaseNo, strTagId);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("bSelectFlag", ConvertFieldType.BoolType, null);
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dataTable, dtConverter);

                res.Add("tempList", dataList);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M11PE0201", ex, loginInfo.UserId);
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
            JArray listInfo = dataForm.multipleSelection;
            List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
            try
            {
                DataTable dtMessage = fS0603_Logic.createTable("MES");
                if (listInfoData.Count != 0)
                {
                    for (int i = 0; i < listInfoData.Count; i++)
                    {
                        string strReceiver = listInfoData[0]["vcReceiver"] == null ? "" : listInfoData[0]["vcReceiver"].ToString();
                        string strCaseNo = listInfoData[0]["vcCaseNo"] == null ? "" : listInfoData[0]["vcCaseNo"].ToString();
                        if (strCaseNo == "" || strReceiver == "")
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("收货方或装箱单号{0}为空，无法打印", strCaseNo);
                            dtMessage.Rows.Add(dataRow);
                        }
                    }
                    if (dtMessage.Rows.Count != 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.type = "list";
                        apiResult.data = dtMessage;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    bool bResult = fS1102_Logic.getPrintInfo(listInfoData, loginInfo.UserId, ref dtMessage);
                    if (!bResult)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.type = "list";
                        apiResult.data = dtMessage;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    #region 调用webApi打印
                    DataTable dtPrinterInfo = fS0603_Logic.getPrinterInfo("装箱单", loginInfo.UserId);
                    if (dtPrinterInfo.Rows.Count != 0)
                    {
                        //创建 HTTP 绑定对象
                        string file_crv = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "CryReports" + Path.DirectorySeparatorChar;
                        var binding = new BasicHttpBinding();
                        //根据 WebService 的 URL 构建终端点对象
                        var endpoint = new EndpointAddress(dtPrinterInfo.Rows[0]["vcWebAPI"].ToString());
                        //创建调用接口的工厂，注意这里泛型只能传入接口
                        var factory = new ChannelFactory<WebServiceAPISoap>(binding, endpoint);
                        //从工厂获取具体的调用实例
                        var callClient = factory.CreateChannel();
                        setCRVPrintRequestBody Body = new setCRVPrintRequestBody();
                        Body.strScrpit = "select * from tPrintTemp_FS1102 where vcOperator='" + loginInfo.UserId + "' ORDER BY CAST(vcNo AS INT)";
                        Body.strCRVName = file_crv + dtPrinterInfo.Rows[0]["vcReports"].ToString();
                        Body.strPrinterName = dtPrinterInfo.Rows[0]["vcPrinter"].ToString();
                        Body.sqlUserID = dtPrinterInfo.Rows[0]["vcSqlUserID"].ToString();
                        Body.sqlPassword = dtPrinterInfo.Rows[0]["vcSqlPassword"].ToString();
                        Body.sqlCatalog = dtPrinterInfo.Rows[0]["vcSqlCatalog"].ToString();
                        Body.sqlSource = dtPrinterInfo.Rows[0]["vcSqlSource"].ToString();
                        //调用具体的方法，这里是 HelloWorldAsync 方法
                        Task<setCRVPrintResponse> responseTask = callClient.setCRVPrintAsync(new setCRVPrintRequest(Body));
                        //获取结果
                        setCRVPrintResponse response = responseTask.Result;
                        if (response.Body.setCRVPrintResult != "打印成功")
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = "打印失败，请联系管理员进行打印接口故障检查。";
                            dtMessage.Rows.Add(dataRow);
                        }
                    }
                    else
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = "没有打印机信息，请联系管理员维护。";
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
                    #endregion
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.data = "打印成功";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "未选择有效的打印数据";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M11PE0202", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "打印失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
    }
}
