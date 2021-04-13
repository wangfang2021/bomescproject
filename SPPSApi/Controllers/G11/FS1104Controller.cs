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
                List<Object> PlantList = ComFunction.convertAllToResult(ComFunction.getTCode("C071"));

                res.Add("ReceiverList", ReceiverList);
                res.Add("PlantList", PlantList);
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

            string strPlant = dataForm.selectVaule.Plant == null ? "" : dataForm.selectVaule.Plant;
            string strReceiver = dataForm.selectVaule.Receiver == null ? "" : dataForm.selectVaule.Receiver;
            try
            {
                Dictionary<string, object> res = new Dictionary<string, object>();
                DataTable dtMessage = fS0603_Logic.createTable("MES");

                if (strPlant == "" || strReceiver == "")
                {
                    res.Add("CaseNoItem", "");
                }
                else
                {
                    string strCaseNo = fS1104_Logic.getCaseNoInfo(strPlant, strReceiver, "");
                    res.Add("CaseNoItem", strCaseNo.Substring(0, 4) + "-" + strCaseNo.Substring(4, 4));
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

            string strPlant = dataForm.selectVaule.Plant == null ? "" : dataForm.selectVaule.Plant;
            string strReceiver = dataForm.selectVaule.Receiver == null ? "" : dataForm.selectVaule.Receiver;
            string strLianFan = dataForm.selectVaule.LianFan == null ? "" : dataForm.selectVaule.LianFan;
            string strPrintNum = dataForm.selectVaule.PrintNum == null ? "" : dataForm.selectVaule.PrintNum;
            try
            {
                DataTable dtMessage = fS0603_Logic.createTable("MES");
                string imagefile_qr = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Image" + Path.DirectorySeparatorChar + "QRcodeImage" + Path.DirectorySeparatorChar;

                if (strPlant == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "发货方不能为空";
                    dtMessage.Rows.Add(dataRow);
                }
                if (strReceiver == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "收货方不能为空";
                    dtMessage.Rows.Add(dataRow);
                }
                if (strPrintNum == "" || strPrintNum == "0")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "打印数量不能为空(0)";
                    dtMessage.Rows.Add(dataRow);
                }
                if (Convert.ToInt32(strPrintNum) > 500)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "一次打印数量不能超过500";
                    dtMessage.Rows.Add(dataRow);
                }
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                fS1104_Logic.getPrintInfo(strPlant, strReceiver, strPrintNum, loginInfo.UserId, ref dtMessage);
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                {
                    //弹出错误dtMessage
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                #region 调用webApi打印
                DataTable dtPrinterInfo = fS0603_Logic.getPrinterInfo("箱号", loginInfo.UserId);
                if (dtPrinterInfo.Rows.Count != 0)
                {
                    string file_crv = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "CryReports" + Path.DirectorySeparatorChar;
                    var binding = new BasicHttpBinding();
                    //根据 WebService 的 URL 构建终端点对象
                    var endpoint = new EndpointAddress(dtPrinterInfo.Rows[0]["vcWebAPI"].ToString());
                    //创建调用接口的工厂，注意这里泛型只能传入接口
                    var factory = new ChannelFactory<WebServiceAPISoap>(binding, endpoint);
                    //从工厂获取具体的调用实例
                    var callClient = factory.CreateChannel();
                    setCRVPrintRequestBody Body = new setCRVPrintRequestBody();
                    Body.strScrpit = "select * from tPrintTemp_FS1104 where vcOperator='" + loginInfo.UserId + "' order by LinId";
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

                fS1104_Logic.setSaveInfo(loginInfo.UserId, ref dtMessage);
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

            string strPlant = dataForm.selectVaule.Plant == null ? "" : dataForm.selectVaule.Plant;
            string strReceiver = dataForm.selectVaule.Receiver == null ? "" : dataForm.selectVaule.Receiver;
            string strLianFan = dataForm.selectVaule.LianFan == null ? "" : dataForm.selectVaule.LianFan;
            string strPrintNum = dataForm.selectVaule.PrintNum == null ? "" : dataForm.selectVaule.PrintNum;
            try
            {
                DataTable dtMessage = fS0603_Logic.createTable("MES");
                string imagefile_qr = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Image" + Path.DirectorySeparatorChar + "QRcodeImage" + Path.DirectorySeparatorChar;

                if (strPlant == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "发货方不能为空";
                    dtMessage.Rows.Add(dataRow);
                }
                if (strReceiver == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "收货方不能为空";
                    dtMessage.Rows.Add(dataRow);
                }
                if (strLianFan == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "箱号不能为空";
                    dtMessage.Rows.Add(dataRow);
                }
                if (strPrintNum == "" || strPrintNum == "0")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "打印数量不能为空(0)";
                    dtMessage.Rows.Add(dataRow);
                }
                string strCastNo = fS1104_Logic.getCaseNoInfo(strPlant, strReceiver, Convert.ToString(Convert.ToInt32((strLianFan.Replace("-", "")))));
                if (strCastNo == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "所输入箱号未进行过发行，禁止再发行";
                    dtMessage.Rows.Add(dataRow);
                }
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                bool bResult = fS1104_Logic.getPrintInfo(strPlant, strReceiver, strCastNo, strPrintNum, loginInfo.UserId, ref dtMessage);
                if (!bResult)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                #region 调用webApi打印
                DataTable dtPrinterInfo = fS0603_Logic.getPrinterInfo("箱号", loginInfo.UserId);
                if (dtPrinterInfo.Rows.Count != 0)
                {
                    string file_crv = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "CryReports" + Path.DirectorySeparatorChar;
                    var binding = new BasicHttpBinding();
                    //根据 WebService 的 URL 构建终端点对象
                    var endpoint = new EndpointAddress(dtPrinterInfo.Rows[0]["vcWebAPI"].ToString());
                    //创建调用接口的工厂，注意这里泛型只能传入接口
                    var factory = new ChannelFactory<WebServiceAPISoap>(binding, endpoint);
                    //从工厂获取具体的调用实例
                    var callClient = factory.CreateChannel();
                    setCRVPrintRequestBody Body = new setCRVPrintRequestBody();
                    Body.strScrpit = "select * from tPrintTemp_FS1104 where vcOperator='" + loginInfo.UserId + "' order by LinId";
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

                fS1104_Logic.setSaveInfo(strCastNo, loginInfo.UserId, ref dtMessage);
                if (!bResult)
                {
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
