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

namespace SPPSApi.Controllers.G14
{
    [Route("api/FS1406/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1406Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        FS1406_Logic fS1406_Logic = new FS1406_Logic();
        FS0603_Logic fS0603_Logic = new FS0603_Logic();
        private readonly string FunctionID = "FS1406";

        public FS1406Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        /// <summary>
        /// 初始化方法
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
                List<Object> OrderPlantList = ComFunction.convertAllToResult(ComFunction.getTCode("C000"));//发注工场
                List<Object> SPISStatusList = ComFunction.convertAllToResult(ComFunction.getTCode("C067"));//ＳＰＩＳ状态

                res.Add("OrderPlantList", OrderPlantList);
                res.Add("SPISStatusList", SPISStatusList);
                res.Add("SupplierIdItem", loginInfo.UserId);

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
        /// 检索方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string searchApi([FromBody] dynamic data)
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

            string strPartId = dataForm.PartId == null ? "" : dataForm.PartId;
            string strSupplierId = dataForm.SupplierId == null ? "" : dataForm.SupplierId;
            string strCarModel = dataForm.CarModel == null ? "" : dataForm.CarModel;
            string strOrderPlant = dataForm.OrderPlant == null ? "" : dataForm.OrderPlant;
            string strSPISStatus = dataForm.SPISStatus == null ? "" : dataForm.SPISStatus;

            try
            {
                DataTable dataTable = fS1406_Logic.getSearchInfo(strPartId, strSupplierId, strOrderPlant, strCarModel, strSPISStatus);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("bSPISupload", ConvertFieldType.BoolType, null);
                dtConverter.addField("bAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("bModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("bSelectFlag", ConvertFieldType.BoolType, null);
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dataTable, dtConverter);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 回复SPIS
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string replyApi([FromBody]dynamic data)
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

                DataTable dtMessage = fS0603_Logic.createTable("MES");
                DataTable dtImport = fS1406_Logic.checkreplyInfo(checkedInfoData, loginInfo.UserId, loginInfo.UserName, ref dtMessage);
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                {
                    //弹出错误dtMessage
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fS1406_Logic.replyInfo(dtImport, loginInfo.UserId, ref dtMessage);
                if (dtMessage.Rows.Count != 0)
                {
                    //弹出错误dtMessage
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M04UE0203", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "回复SPIS失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 子页面初始化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string subloadApi([FromBody]dynamic data)
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
            try
            {
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                JArray listInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();

                string strSPISStatus = listInfoData[0]["vcSPISStatus"] == null ? "" : listInfoData[0]["vcSPISStatus"].ToString();
                string strApplyId = listInfoData[0]["vcApplyId"] == null ? "" : listInfoData[0]["vcApplyId"].ToString();
                string strFromTime_SPIS = listInfoData[0]["dFromTime_SPIS"] == null ? "" : listInfoData[0]["dFromTime_SPIS"].ToString();
                string strToTime_SPIS = listInfoData[0]["dToTime_SPIS"] == null ? "" : listInfoData[0]["dToTime_SPIS"].ToString();
                string strSPISTime = listInfoData[0]["dSPISTime"] == null ? "" : listInfoData[0]["dSPISTime"].ToString();
                string strPartId = listInfoData[0]["vcPartId"] == null ? "" : listInfoData[0]["vcPartId"].ToString();
                string strPartENName = listInfoData[0]["vcPartENName"] == null ? "" : listInfoData[0]["vcPartENName"].ToString();
                string strCarfamilyCode = listInfoData[0]["vcCarfamilyCode"] == null ? "" : listInfoData[0]["vcCarfamilyCode"].ToString();
                string strSupplierId = listInfoData[0]["vcSupplierId"] == null ? "" : listInfoData[0]["vcSupplierId"].ToString();
                string strSupplierName = listInfoData[0]["vcSupplierName"] == null ? "" : listInfoData[0]["vcSupplierName"].ToString();
                string strColourNo = listInfoData[0]["vcColourNo"] == null ? "" : listInfoData[0]["vcColourNo"].ToString();
                string strColourCode = listInfoData[0]["vcColourCode"] == null ? "" : listInfoData[0]["vcColourCode"].ToString();
                string strColourName = listInfoData[0]["vcColourName"] == null ? "" : listInfoData[0]["vcColourName"].ToString();
                string strModItem = listInfoData[0]["vcModItem"] == null ? "" : listInfoData[0]["vcModItem"].ToString();

                string strPicUrl = listInfoData[0]["vcPICUrl"] == null ? "" : listInfoData[0]["vcPICUrl"].ToString();
                string strSPISUrl = listInfoData[0]["vcSPISUrl"] == null ? "" : listInfoData[0]["vcSPISUrl"].ToString();

                string model = "NG";
                if (strSPISStatus == "0" || strSPISStatus == "1" || strSPISStatus == "4")
                    model = "OK";
                res.Add("SPISStatusItem", strSPISStatus);
                res.Add("ApplyIdItem", strApplyId);
                res.Add("FromTime_SPISItem", strFromTime_SPIS);
                res.Add("ToTime_SPISItem", strToTime_SPIS);
                res.Add("SPISTimeItem", strSPISTime == "" ? System.DateTime.Now.ToString("yyyy-MM-dd") : strSPISTime);
                res.Add("PartIdItem", strPartId);
                res.Add("PartENNameItem", strPartENName);
                res.Add("CarfamilyCodeItem", strCarfamilyCode);
                res.Add("SupplierIdItem", strSupplierId);
                res.Add("SupplierNameItem", strSupplierName);
                res.Add("ColourNoItem", strColourNo);
                res.Add("ColourCodeItem", strColourCode);
                res.Add("ColourNameItem", strColourName);
                res.Add("PicUrlItem", strPicUrl);
                res.Add("ModItemItem", strModItem);
                res.Add("model", model);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0204", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "初始化失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 保存方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string saveApi([FromBody]dynamic data)
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
                DataTable dtMessage = fS0603_Logic.createTable("MES");
                DataTable dtImport = fS1406_Logic.setInfoList(dataForm);
                DataTable dtApplyList = dtImport.Clone();
                string strPath_temp = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "upload_spisapply" + Path.DirectorySeparatorChar + "apply" + Path.DirectorySeparatorChar;
                string strPath_pic = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Image" + Path.DirectorySeparatorChar + "SPISPic" + Path.DirectorySeparatorChar;
                string strPath_pdf = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Image" + Path.DirectorySeparatorChar + "SPISPdf" + Path.DirectorySeparatorChar;
                string strPath_sips = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Image" + Path.DirectorySeparatorChar + "SPISImage" + Path.DirectorySeparatorChar;
                DataTable dtPDF_temp = fS1406_Logic.getTempDataInfo();
                fS1406_Logic.checkSaveInfo(dtImport, ref dtApplyList, ref dtPDF_temp, strPath_temp, strPath_pic, strPath_pdf, strPath_sips, loginInfo.UserId, ref dtMessage);
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                {
                    dtMessage = dtMessage.DefaultView.ToTable(true, "vcMessage");
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //处理图像
                //1.插入并打印
                for (int i = 0; i < dtPDF_temp.Rows.Count; i++)
                {
                    DataRow drPDF_temp = dtPDF_temp.Rows[i];
                    string sources_pdf = drPDF_temp["vcPDFPath"].ToString();
                    fS1406_Logic.setCRVtoPdf(drPDF_temp, loginInfo.UserId, ref dtMessage);
                    #region 调用webApiPDF导出
                    //创建 HTTP 绑定对象
                    string file_crv = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "CryReports" + Path.DirectorySeparatorChar;
                    var binding = new BasicHttpBinding();
                    //根据 WebService 的 URL 构建终端点对象
                    var endpoint = new EndpointAddress(@"http://172.23.238.179/WebAPI/WebServiceAPI.asmx");
                    //创建调用接口的工厂，注意这里泛型只能传入接口
                    var factory = new ChannelFactory<WebServiceAPISoap>(binding, endpoint);
                    //从工厂获取具体的调用实例
                    var callClient = factory.CreateChannel();
                    setCRVToPDFRequestBody Body = new setCRVToPDFRequestBody();
                    Body.strCRVName = file_crv + "crv_FS1406.rpt";
                    Body.strTableName = "tPrintTemp_FS1406";
                    Body.strOperID = loginInfo.UserId;
                    Body.strDiskFileName = sources_pdf;
                    Body.sqlUserID = "sa";
                    Body.sqlPassword = "SPPS_Server2019";
                    Body.sqlCatalog = "SPPSdb";
                    Body.sqlSource = "172.23.180.116";
                    //调用具体的方法，这里是 HelloWorldAsync 方法
                    Task<setCRVToPDFResponse> responseTask = callClient.setCRVToPDFAsync(new setCRVToPDFRequest(Body));
                    //获取结果
                    setCRVToPDFResponse response = responseTask.Result;
                    if (response.Body.setCRVToPDFResult != "导出成功")
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = "打印失败，请联系管理员进行打印接口故障检查。";
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
                }
                //2.PDF转SPIS图片
                fS1406_Logic.setPdftoImgs(dtApplyList, loginInfo.UserId, ref dtMessage);
                //3.保存数据
                fS1406_Logic.setSaveInfo(dtApplyList, loginInfo.UserId, ref dtMessage);
                if (dtMessage.Rows.Count != 0)
                {
                    //弹出错误dtMessage
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M04UE0203", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存SPIS失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }

    }
}
