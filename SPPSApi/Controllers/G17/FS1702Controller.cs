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

namespace SPPSApi.Controllers.G17
{
    [Route("api/FS1702/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1702Controller : BaseController
    {
        FS1702_Logic fs1702_Logic = new FS1702_Logic();
        private readonly string FunctionID = "FS1702";

        private readonly IWebHostEnvironment _webHostEnvironment;

        public FS1702Controller(IWebHostEnvironment webHostEnvironment)
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

                List<Object> dataList_Project = ComFunction.convertAllToResult(fs1702_Logic.getAllProject());//工程
                res.Add("optionProject", dataList_Project);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M17UE0201", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "初始化失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 检索
        [HttpPost]
        [EnableCors("any")]
        public string searchApi([FromBody]dynamic data)
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
            string vcProject = dataForm.vcProject;
            string dChuHeDateFrom = dataForm.dChuHeDateFrom;
            string dChuHeDateTo = dataForm.dChuHeDateTo;
            try
            {
                DataTable dt = fs1702_Logic.Search(vcProject, dChuHeDateFrom, dChuHeDateTo);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("dChuHeDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M17UE0202", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 确认单打印 not use
        //[HttpPost]
        //[EnableCors("any")]
        //[Obsolete]
        //public string qrdPrintApi([FromBody]dynamic data)
        //{
        //    //验证是否登录
        //    string strToken = Request.Headers["X-Token"];
        //    if (!isLogin(strToken))
        //    {
        //        return error_login();
        //    }
        //    LoginInfo loginInfo = getLoginByToken(strToken);
        //    //以下开始业务处理
        //    ApiResult apiResult = new ApiResult();
        //    try
        //    {
        //        dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
        //        JArray checkedInfo = dataForm.multipleSelection;
        //        List<Dictionary<string, Object>> checkedInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();
        //        if (checkedInfoData.Count == 0)
        //        {
        //            apiResult.code = ComConstant.ERROR_CODE;
        //            apiResult.data = "最少选择一行！";
        //            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //        }
        //        //取出生成确认单的数据
        //        string[] fields = { "id", "vcPart_id", "vcBackPart_id", "iQuantity", "vcRemark"};
        //        string vcQueRenNo = checkedInfoData[0]["vcQueRenNo"].ToString();
        //        string vcProject = checkedInfoData[0]["vcProject"].ToString();
        //        string dChuHeDate = checkedInfoData[0]["dChuHeDate"].ToString();
        //        DataTable dt = fs1702_Logic.GetqrdInfo(vcProject, dChuHeDate);
        //        string strMsg = "";
        //        //生成excel
        //        string filepath = fs1702_Logic.generateExcelWithXlt(vcQueRenNo,dt, fields, _webHostEnvironment.ContentRootPath, "FS1702.xlsx", 0, 3, loginInfo.UserId, FunctionID);
        //        if (strMsg != "")
        //        {
        //            apiResult.code = ComConstant.ERROR_CODE;
        //            apiResult.data = strMsg;
        //            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //        }
        //        //调打印方法（没做呢）

        //        //更新打印时间
        //        fs1702_Logic.qrdPrint(checkedInfoData, loginInfo.UserId);
        //        apiResult.code = ComConstant.SUCCESS_CODE;
        //        apiResult.data = null;
        //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //    }
        //    catch (Exception ex)
        //    {
        //        ComMessage.GetInstance().ProcessMessage(FunctionID, "M08UE1004", ex, loginInfo.UserId);
        //        apiResult.code = ComConstant.ERROR_CODE;
        //        apiResult.data = "删除失败";
        //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //    }
        //}
        #endregion

        #region 确认单打印
        [HttpPost]
        [EnableCors("any")]
        public string qrdPrintApi([FromBody]dynamic data)
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
                DataTable dtMessage = fs1702_Logic.createTable("MES");
                if (listInfoData.Count != 0)
                {
                    for (int i = 0; i < listInfoData.Count; i++)
                    {
                        string vcProject = listInfoData[i]["vcProject"] == null ? "" : listInfoData[i]["vcProject"].ToString();
                        string dChuHeDate = listInfoData[i]["dChuHeDate"] == null ? "" : listInfoData[i]["dChuHeDate"].ToString();
                        if (vcProject == "" || dChuHeDate == "")
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("工程{0}或出荷日期{1}为空，无法打印", vcProject, dChuHeDate);
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
                    DataTable dtBJW = new DataTable();
                    DataTable dtBJWHistory = new DataTable();
                    DataTable dtSub = new DataTable();
                    bool bResult = fs1702_Logic.getPrintInfo(listInfoData, loginInfo.UserId, ref dtMessage, ref dtBJW, ref dtBJWHistory, ref dtSub);
                    if (!bResult)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.type = "list";
                        apiResult.data = dtMessage;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    #region 调用webApi打印 not use
                    //FS0603_Logic fS0603_Logic = new FS0603_Logic();
                    //string strPrinterName = fS0603_Logic.getPrinterName("FS1702", loginInfo.UserId);
                    ////创建 HTTP 绑定对象
                    //string file_crv = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "CryReports" + Path.DirectorySeparatorChar;
                    //var binding = new BasicHttpBinding();
                    ////根据 WebService 的 URL 构建终端点对象
                    //var endpoint = new EndpointAddress(@"http://172.23.238.171/WebAPI/WebServiceAPI.asmx");
                    ////创建调用接口的工厂，注意这里泛型只能传入接口
                    //var factory = new ChannelFactory<WebServiceAPISoap>(binding, endpoint);
                    ////从工厂获取具体的调用实例
                    //var callClient = factory.CreateChannel();
                    //setCRVPrintRequestBody Body = new setCRVPrintRequestBody();
                    //Body.strCRVName = file_crv + "crv_FS1702_qrd.rpt";
                    //Body.strScrpit = "select * from tPrintTemp_FS1702 where vcOperator='" + loginInfo.UserId + "' ORDER BY CAST(id AS INT)";
                    //Body.strPrinterName = strPrinterName;
                    //Body.sqlUserID = "sa";
                    //Body.sqlPassword = "SPPS_Server2019";
                    //Body.sqlCatalog = "SPPSdb";
                    //Body.sqlSource = "172.23.180.116";
                    ////调用具体的方法，这里是 HelloWorldAsync 方法
                    //Task<setCRVPrintResponse> responseTask = callClient.setCRVPrintAsync(new setCRVPrintRequest(Body));
                    ////获取结果
                    //setCRVPrintResponse response = responseTask.Result;
                    //if (response.Body.setCRVPrintResult != "打印成功")
                    //{
                    //    DataRow dataRow = dtMessage.NewRow();
                    //    dataRow["vcMessage"] = "打印失败，请联系管理员进行打印接口故障检查。";
                    //    dtMessage.Rows.Add(dataRow);
                    //}
                    //if (dtMessage != null && dtMessage.Rows.Count != 0)
                    //{
                    //    //弹出错误dtMessage
                    //    apiResult.code = ComConstant.ERROR_CODE;
                    //    apiResult.type = "list";
                    //    apiResult.data = dtMessage;
                    //    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    //}
                    #endregion

                    #region 调用webApi打印
                    FS0603_Logic fS0603_Logic = new FS0603_Logic();
                    DataTable dtPrinterInfo = fS0603_Logic.getPrinterInfo("确认单", loginInfo.UserId);
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
                        Body.strScrpit = "select cast(ROW_NUMBER() over(order by UUID) as int) as id,vcOperator,dOperatorTime,vcQueRenNo,vcPart_id,vcBackPart_id,iQuantity from tPrintTemp_FS1702 where vcOperator='" + loginInfo.UserId + "' and iQuantity>0 ORDER BY UUID";
                        Body.strCRVName = file_crv + dtPrinterInfo.Rows[0]["vcReports"].ToString();
                        Body.strPrinterName = dtPrinterInfo.Rows[0]["vcPrinter"].ToString();
                        Body.sqlUserID = dtPrinterInfo.Rows[0]["vcSqlUserID"].ToString();
                        Body.sqlPassword = dtPrinterInfo.Rows[0]["vcSqlPassword"].ToString();
                        Body.sqlCatalog = dtPrinterInfo.Rows[0]["vcSqlCatalog"].ToString();
                        Body.sqlSource = dtPrinterInfo.Rows[0]["vcSqlSource"].ToString();
                        for (int i = 0; i < 2; i++)
                        {//确认单一式2份，调用2次
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

                    //更新打印时间
                    fs1702_Logic.qrdPrint(listInfoData, loginInfo.UserId, dtBJW, dtBJWHistory, dtSub);

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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M17UE0203", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "确认单打印失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 出荷看板打印 not use 
        //[HttpPost]
        //[EnableCors("any")]
        //public string kbPrintApi([FromBody]dynamic data)
        //{
        //    //验证是否登录
        //    string strToken = Request.Headers["X-Token"];
        //    if (!isLogin(strToken))
        //    {
        //        return error_login();
        //    }
        //    LoginInfo loginInfo = getLoginByToken(strToken);
        //    //以下开始业务处理
        //    ApiResult apiResult = new ApiResult();
        //    try
        //    {
        //        dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
        //        JArray checkedInfo = dataForm.multipleSelection;
        //        List<Dictionary<string, Object>> checkedInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();
        //        if (checkedInfoData.Count == 0)
        //        {
        //            apiResult.code = ComConstant.ERROR_CODE;
        //            apiResult.data = "最少选择一行！";
        //            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //        }
        //        string vcQueRenNo = checkedInfoData[0]["vcQueRenNo"].ToString();
        //        string vcProject = checkedInfoData[0]["vcProject"].ToString();
        //        string dChuHeDate = checkedInfoData[0]["dChuHeDate"].ToString();
        //        for (int i = 0; i < checkedInfoData.Count; i++)
        //        {
        //            if (vcQueRenNo.StartsWith("BJW"))
        //            {
        //                apiResult.code = ComConstant.ERROR_CODE;
        //                apiResult.data = "确认单号[" + vcQueRenNo + "]不能进行出荷看板打印！";
        //                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //            }
        //        }
        //        //取出生成看板数据
        //        DataTable dt = fs1702_Logic.getKBData(vcProject,dChuHeDate);
        //        //调用打印方法（还没做呢）

        //        //更新打印时间
        //        fs1702_Logic.kbPrint(checkedInfoData, loginInfo.UserId);
        //        apiResult.code = ComConstant.SUCCESS_CODE;
        //        apiResult.data = null;
        //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //    }
        //    catch (Exception ex)
        //    {
        //        ComMessage.GetInstance().ProcessMessage(FunctionID, "M08UE1004", ex, loginInfo.UserId);
        //        apiResult.code = ComConstant.ERROR_CODE;
        //        apiResult.data = "删除失败";
        //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //    }
        //}
        #endregion

        #region 出荷看板打印
        [HttpPost]
        [EnableCors("any")]
        public string kbPrintApi([FromBody]dynamic data)
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
                DataTable dtMessage = fs1702_Logic.createTable("MES");
                if (listInfoData.Count != 0)
                {
                    for (int i = 0; i < listInfoData.Count; i++)
                    {
                        string vcProject = listInfoData[i]["vcProject"] == null ? "" : listInfoData[i]["vcProject"].ToString();
                        string dChuHeDate = listInfoData[i]["dChuHeDate"] == null ? "" : listInfoData[i]["dChuHeDate"].ToString();
                        string vcQueRenNo = listInfoData[i]["vcQueRenNo"] == null ? "" : listInfoData[i]["vcQueRenNo"].ToString();
                        if (vcProject == "" || dChuHeDate == "")
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("工程{0}或出荷日期{1}为空，无法打印", vcProject, dChuHeDate);
                            dtMessage.Rows.Add(dataRow);
                        }
                        if (vcQueRenNo.StartsWith("BJW"))
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("确认单号{0}不能进行出荷看板打印！", vcQueRenNo);
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
                    bool bResult = fs1702_Logic.getPrintInfo_kb(listInfoData, loginInfo.UserId, ref dtMessage);
                    if (!bResult)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.type = "list";
                        apiResult.data = dtMessage;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    #region 调用webApi打印 not use
                    //FS0603_Logic fS0603_Logic = new FS0603_Logic();
                    //string strPrinterName = fS0603_Logic.getPrinterName("FS1702", loginInfo.UserId);
                    ////创建 HTTP 绑定对象
                    //string file_crv = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "CryReports" + Path.DirectorySeparatorChar;
                    //var binding = new BasicHttpBinding();
                    ////根据 WebService 的 URL 构建终端点对象
                    //var endpoint = new EndpointAddress(@"http://172.23.238.171/WebAPI/WebServiceAPI.asmx");
                    ////创建调用接口的工厂，注意这里泛型只能传入接口
                    //var factory = new ChannelFactory<WebServiceAPISoap>(binding, endpoint);
                    ////从工厂获取具体的调用实例
                    //var callClient = factory.CreateChannel();
                    //setCRVPrintRequestBody Body = new setCRVPrintRequestBody();
                    //Body.strCRVName = file_crv + "crv_FS1702_kb_main.rpt";
                    //Body.strScrpit = "select * from tPrintTemp_FS1702_kb_main where vcOperator='" + loginInfo.UserId + "' ORDER BY LinId";
                    //Body.strPrinterName = strPrinterName;
                    //Body.sqlUserID = "sa";
                    //Body.sqlPassword = "SPPS_Server2019";
                    //Body.sqlCatalog = "SPPSdb";
                    //Body.sqlSource = "172.23.180.116";
                    ////调用具体的方法，这里是 HelloWorldAsync 方法
                    //Task<setCRVPrintResponse> responseTask = callClient.setCRVPrintAsync(new setCRVPrintRequest(Body));
                    ////获取结果
                    //setCRVPrintResponse response = responseTask.Result;
                    //if (response.Body.setCRVPrintResult != "打印成功")
                    //{
                    //    DataRow dataRow = dtMessage.NewRow();
                    //    dataRow["vcMessage"] = "打印失败，请联系管理员进行打印接口故障检查。";
                    //    dtMessage.Rows.Add(dataRow);
                    //}
                    //if (dtMessage != null && dtMessage.Rows.Count != 0)
                    //{
                    //    //弹出错误dtMessage
                    //    apiResult.code = ComConstant.ERROR_CODE;
                    //    apiResult.type = "list";
                    //    apiResult.data = dtMessage;
                    //    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    //}
                    #endregion

                    #region 调用webApi打印
                    FS0603_Logic fS0603_Logic = new FS0603_Logic();
                    DataTable dtPrinterInfo = fS0603_Logic.getPrinterInfo("出荷看板", loginInfo.UserId);
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
                        Body.strScrpit = "select * from tPrintTemp_FS1702_kb_main where vcOperator='" + loginInfo.UserId + "' ORDER BY LinId";
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

                    //更新打印时间
                    fs1702_Logic.kbPrint(listInfoData, loginInfo.UserId);

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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M17UE0204", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "出荷看板打印失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 出荷完了
        [HttpPost]
        [EnableCors("any")]
        public string chuheOKApi([FromBody]dynamic data)
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
                JArray checkedInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> checkedInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();
                if (checkedInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选择一行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                for (int i = 0; i < checkedInfoData.Count; i++)
                {
                    string vcQueRenNo = checkedInfoData[i]["vcQueRenNo"].ToString();
                    string QueRenPrintFlag = checkedInfoData[i]["QueRenPrintFlag"].ToString();
                    string KBPrintFlagg = checkedInfoData[i]["KBPrintFlag"].ToString();
                    if (vcQueRenNo.StartsWith("BJW"))
                    {
                        if (QueRenPrintFlag == "")
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "确认单号[" + vcQueRenNo + "]：先进行确认单打印！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                    }
                    else
                    {
                        if (QueRenPrintFlag == "" || KBPrintFlagg == "")
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "确认单号[" + vcQueRenNo + "]：先进行确认单和出荷看板打印！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                    }
                }
                fs1702_Logic.chuheOK(checkedInfoData, loginInfo.UserId);//更新在库还没写呢
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M17UE0205", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "出荷完了失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}
