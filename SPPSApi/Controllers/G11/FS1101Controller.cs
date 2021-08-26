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
    [Route("api/FS1101/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1101Controller : BaseController
    {
        FS0603_Logic fS0603_Logic = new FS0603_Logic();
        FS1101_Logic fS1101_Logic = new FS1101_Logic();
        private readonly string FunctionID = "FS1101";

        private readonly IWebHostEnvironment _webHostEnvironment;
        public FS1101Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        /// <summary>
        /// 重启bat
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string restartLWApi()
        {
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            //以下开始业务处理
            ApiResult apiResult = new ApiResult();
            DataTable dtMessage = fS0603_Logic.createTable("MES");
            try
            {
                Dictionary<string, object> res = new Dictionary<string, object>();
                DataTable dataTable = fS1101_Logic.getBatInfo("1");
                if (dataTable.Rows.Count == 0|| dataTable.Rows[0]["vcBatPath"].ToString()=="")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "未维护打印服务地址";
                    dtMessage.Rows.Add(dataRow);
                }
                if (dtMessage.Rows.Count != 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                System.Diagnostics.Process.Start(dataTable.Rows[0]["vcBatPath"].ToString());
                //System.Diagnostics.Process.Start(@"C:\Users\Administrator\Desktop\laowu\打印程序\close.bat");
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06PE0200", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "初始化失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        [HttpPost]
        [EnableCors("any")]
        public string restartIISApi()
        {
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            //以下开始业务处理
            ApiResult apiResult = new ApiResult();
            DataTable dtMessage = fS0603_Logic.createTable("MES");
            try
            {
                Dictionary<string, object> res = new Dictionary<string, object>();
                DataTable dataTable = fS1101_Logic.getBatInfo("2");
                if (dataTable.Rows.Count == 0 || dataTable.Rows[0]["vcBatPath"].ToString() == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "未维护打印服务地址";
                    dtMessage.Rows.Add(dataRow);
                }
                if (dtMessage.Rows.Count != 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                System.Diagnostics.Process.Start(dataTable.Rows[0]["vcBatPath"].ToString());
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06PE0200", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "初始化失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
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
                //
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M11PE0100", ex, loginInfo.UserId);
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

            string strPackMaterNo = dataForm.PackMaterNo == null ? "" : dataForm.PackMaterNo;
            string strTrolleyNo = dataForm.TrolleyNo == null ? "" : dataForm.TrolleyNo;
            string strPartId = dataForm.PartId == null ? "" : dataForm.PartId;
            string strOrderNo = dataForm.OrderNo == null ? "" : dataForm.OrderNo;
            string strLianFan = dataForm.LianFan == null ? "" : dataForm.LianFan;
            try
            {
                DataTable dataTable = fS1101_Logic.getSearchInfo(strPackMaterNo, strTrolleyNo, strPartId, strOrderNo, strLianFan);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("bSelectFlag", ConvertFieldType.BoolType, null);
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dataTable, dtConverter);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M11PE0101", ex, loginInfo.UserId);
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
                        //string strLind = listInfoData[i]["LinId"] == null ? "" : listInfoData[i]["LinId"].ToString();
                        string strPackMaterNo = listInfoData[i]["vcPackMaterNo"] == null ? "" : listInfoData[i]["vcPackMaterNo"].ToString();
                        if (strPackMaterNo == "")
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("断取指示书号{0}为空，无法打印", strPackMaterNo);
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
                    bool bResult = fS1101_Logic.getPrintInfo(listInfoData, loginInfo.UserId, ref dtMessage);
                    if (!bResult)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.type = "list";
                        apiResult.data = dtMessage;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    //DataSet dsPrintInfo = fS1101_Logic.getPrintInfo(loginInfo.UserId);
                    //if (dsPrintInfo.Tables[0].Rows.Count != 0)
                    //{
                    //    string strPrinterName = fS0603_Logic.getPrinterName("FS1101", loginInfo.UserId);
                    //    for (int i = 0; i < dsPrintInfo.Tables[0].Rows.Count; i++)
                    //    {
                    //        string strLotid = dsPrintInfo.Tables[0].Rows[i]["vcLotid"].ToString();
                    //        DataTable dtExport = dsPrintInfo.Tables[1].Clone();
                    //        DataRow[] drPrintInfo = dsPrintInfo.Tables[1].Select("vcLotid='" + strLotid + "'");
                    //        for (int j = 0; j < drPrintInfo.Length; j++)
                    //        {
                    //            dtExport.ImportRow(drPrintInfo[j]);
                    //        }
                    //        //序号赋值
                    //        for (int j = 0; j < dtExport.Rows.Count; j++)
                    //        {
                    //            dtExport.Rows[j]["LinId"] = j + 1;
                    //        }
                    //        //导出到excel中
                    //        string[] fields = { "LinId", "vcPackingpartsno", "vcPackinggroup", "dQty", "vcPackingpartslocation", "vcCodemage" };
                    //        string filepath = fS1101_Logic.generateExcelWithXlt(dtExport, fields, _webHostEnvironment.ContentRootPath, "FS1101_Print.xlsx", 0, 3, loginInfo.UserId, FunctionID);
                    //        #region 调用webApi打印
                    //        //创建 HTTP 绑定对象
                    //        var binding = new BasicHttpBinding();
                    //        //根据 WebService 的 URL 构建终端点对象
                    //        var endpoint = new EndpointAddress(@"http://172.23.238.179/WebAPI/WebServiceAPI.asmx");
                    //        //创建调用接口的工厂，注意这里泛型只能传入接口
                    //        var factory = new ChannelFactory<WebServiceAPISoap>(binding, endpoint);
                    //        //从工厂获取具体的调用实例
                    //        var callClient = factory.CreateChannel();
                    //        setExcelPrintRequestBody Body = new setExcelPrintRequestBody();
                    //        Body.strDiskFileName = filepath;
                    //        Body.strOperID = loginInfo.UserId;
                    //        Body.strPrinterName = strPrinterName;
                    //        //调用具体的方法，这里是 HelloWorldAsync 方法
                    //        Task<setExcelPrintResponse> responseTask = callClient.setExcelPrintAsync(new setExcelPrintRequest(Body));
                    //        //获取结果
                    //        setExcelPrintResponse response = responseTask.Result;
                    //        if (response.Body.setExcelPrintResult != "打印成功")
                    //        {
                    //            DataRow dataRow = dtMessage.NewRow();
                    //            dataRow["vcMessage"] = "打印失败，请联系管理员进行打印接口故障检查。";
                    //            dtMessage.Rows.Add(dataRow);
                    //        }
                    //        #endregion
                    //    }
                    //}
                    //else
                    //{
                    //    DataRow dataRow = dtMessage.NewRow();
                    //    dataRow["vcMessage"] = "断取指示书数据异常";
                    //    dtMessage.Rows.Add(dataRow);
                    //}
                    #region 调用webApi打印
                    DataTable dtPrinterInfo = fS0603_Logic.getPrinterInfo("段取指示书", loginInfo.UserId);
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
                        Body.strScrpit = "select * from tPrintTemp_FS1101 where vcOperator='" + loginInfo.UserId + "' ORDER BY CAST(vcRows AS INT)";
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M11PE0102", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "打印失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
    }
}
