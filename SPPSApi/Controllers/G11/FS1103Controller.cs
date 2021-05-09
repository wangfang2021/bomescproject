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
    [Route("api/FS1103/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1103Controller : BaseController
    {
        FS0603_Logic fS0603_Logic = new FS0603_Logic();
        FS1103_Logic fS1103_Logic = new FS1103_Logic();
        private readonly string FunctionID = "FS1103";

        private readonly IWebHostEnvironment _webHostEnvironment;
        public FS1103Controller(IWebHostEnvironment webHostEnvironment)
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
                DataTable dtOptionsList = fS0603_Logic.getFormOptions("");
                List<Object> ReceiverList = ComFunction.convertAllToResult(fS0603_Logic.getSelectOptions(dtOptionsList, "vcReceiver_Name", "vcReceiver_Value"));//收货方选项

                res.Add("ReceiverList", ReceiverList);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M11PE0300", ex, loginInfo.UserId);
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
            string strOrderNo = dataForm.OrderNo == null ? "" : dataForm.OrderNo;
            string strInPutOrderNo = dataForm.InPutOrderNo == null ? "" : dataForm.InPutOrderNo;
            string strPartId = dataForm.PartId == null ? "" : dataForm.PartId;
            string strLianFan = dataForm.LianFan == null ? "" : dataForm.LianFan;
            try
            {
                DataTable dataTable = fS1103_Logic.getSearchInfo(strReceiver, strOrderNo, strInPutOrderNo, strPartId, strLianFan);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("bInPutOrder", ConvertFieldType.BoolType, null);
                dtConverter.addField("bTag", ConvertFieldType.BoolType, null);
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dataTable, dtConverter);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M11PE0301", ex, loginInfo.UserId);
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
            JArray listInfo = dataForm.list;
            List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
            try
            {
                DataTable dtMessage = fS0603_Logic.createTable("MES");
                if (listInfoData.Count != 0)
                {

                    //获取待打印的数据
                    fS1103_Logic.getPrintInfo(listInfoData, loginInfo.UserId, ref dtMessage);
                    if (dtMessage.Rows.Count != 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.type = "list";
                        apiResult.data = dtMessage;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    #region 调用webApi打印
                    string file_crv = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "CryReports" + Path.DirectorySeparatorChar;
                    if (fS1103_Logic.getTempInfo(loginInfo.UserId, "input").Rows.Count != 0)
                    {
                        DataTable dtPrinterInfo = fS0603_Logic.getPrinterInfo("指令书", loginInfo.UserId);
                        if (dtPrinterInfo.Rows.Count != 0)
                        {
                            var binding = new BasicHttpBinding();
                            //根据 WebService 的 URL 构建终端点对象
                            var endpoint = new EndpointAddress(dtPrinterInfo.Rows[0]["vcWebAPI"].ToString());
                            //创建调用接口的工厂，注意这里泛型只能传入接口
                            var factory = new ChannelFactory<WebServiceAPISoap>(binding, endpoint);
                            //从工厂获取具体的调用实例
                            var callClient = factory.CreateChannel();
                            setCRVPrintRequestBody Body = new setCRVPrintRequestBody();
                            Body.strScrpit = "SELECT * from tPrintTemp_input_FS1103 where vcOperatorID='" + loginInfo.UserId + "' order by vcInno";
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
                                dataRow["vcMessage"] = "入库指令书打印失败，请联系管理员进行打印接口故障检查。";
                                dtMessage.Rows.Add(dataRow);
                            }
                        }
                        else
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = "没有指令书打印机信息，请联系管理员维护。";
                            dtMessage.Rows.Add(dataRow);
                        }
                    }
                    if (fS1103_Logic.getTempInfo(loginInfo.UserId, "tag").Rows.Count != 0)
                    {
                        DataTable dtPrinterInfo = fS0603_Logic.getPrinterInfo("标签", loginInfo.UserId);
                        if (dtPrinterInfo.Rows.Count != 0)
                        {
                            var binding = new BasicHttpBinding();
                            //根据 WebService 的 URL 构建终端点对象
                            var endpoint = new EndpointAddress(dtPrinterInfo.Rows[0]["vcWebAPI"].ToString());
                            //创建调用接口的工厂，注意这里泛型只能传入接口
                            var factory = new ChannelFactory<WebServiceAPISoap>(binding, endpoint);
                            //从工厂获取具体的调用实例
                            var callClient = factory.CreateChannel();
                            setCRVPrintRequestBody Body = new setCRVPrintRequestBody();
                            Body.strScrpit = "SELECT * from tPrintTemp_tag_FS1103 where vcOperatorID='" + loginInfo.UserId + "' order by vcInno,vcPrintcount";
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
                                dataRow["vcMessage"] = "标签打印失败，请联系管理员进行打印接口故障检查。";
                                dtMessage.Rows.Add(dataRow);
                            }
                        }
                        else
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = "没有标签打印机信息，请联系管理员维护。";
                            dtMessage.Rows.Add(dataRow);
                        }
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
                }
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M11PE0302", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "打印失败";
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
            string strPartId = dataForm.selectVaule.PartId == null ? "" : dataForm.selectVaule.PartId;
            string strPrintNum = dataForm.selectVaule.PrintNum == null ? "" : dataForm.selectVaule.PrintNum;
            try
            {
                DataTable dtMessage = fS0603_Logic.createTable("MES");
                string imagefile_qr = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Image" + Path.DirectorySeparatorChar + "QRcodeImage" + Path.DirectorySeparatorChar;
                if (strPartId == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "品番不能为空";
                    dtMessage.Rows.Add(dataRow);
                }
                if (strPrintNum == "" || strPrintNum == "0")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "打印数量不能为空(0)";
                    dtMessage.Rows.Add(dataRow);
                }
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fS1103_Logic.getPrintInfo(strPartId, strPrintNum, loginInfo.UserId, ref dtMessage);
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                #region 调用webApi打印
                string file_crv = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "CryReports" + Path.DirectorySeparatorChar;
                if (fS1103_Logic.getTempInfo(loginInfo.UserId, "tag").Rows.Count != 0)
                {
                    DataTable dtPrinterInfo = fS0603_Logic.getPrinterInfo("标签", loginInfo.UserId);
                    if (dtPrinterInfo.Rows.Count != 0)
                    {
                        var binding = new BasicHttpBinding();
                        //根据 WebService 的 URL 构建终端点对象
                        var endpoint = new EndpointAddress(dtPrinterInfo.Rows[0]["vcWebAPI"].ToString());
                        //创建调用接口的工厂，注意这里泛型只能传入接口
                        var factory = new ChannelFactory<WebServiceAPISoap>(binding, endpoint);
                        //从工厂获取具体的调用实例
                        var callClient = factory.CreateChannel();
                        setCRVPrintRequestBody Body = new setCRVPrintRequestBody();
                        Body.strScrpit = "SELECT * from tPrintTemp_tag_FS1103 where vcOperatorID='" + loginInfo.UserId + "' order by vcInno,vcPrintcount";
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
                            dataRow["vcMessage"] = "标签打印失败，请联系管理员进行打印接口故障检查。";
                            dtMessage.Rows.Add(dataRow);
                        }
                    }
                    else
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = "没有标签打印机信息，请联系管理员维护。";
                        dtMessage.Rows.Add(dataRow);
                    }
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
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M11PE0303", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "打印失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
    }
}
