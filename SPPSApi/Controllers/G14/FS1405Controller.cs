using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebServiceAPI;

namespace SPPSApi.Controllers.G14
{
    [Route("api/FS1405/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1405Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        FS1405_Logic fS1405_Logic = new FS1405_Logic();
        FS0602_Logic fS0602_Logic = new FS0602_Logic();
        FS0603_Logic fS0603_Logic = new FS0603_Logic();
        FS1406_Logic fS1406_Logic = new FS1406_Logic();
        private readonly string FunctionID = "FS1405";

        public FS1405Controller(IWebHostEnvironment webHostEnvironment)
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
                List<Object> HaoJiuList = ComFunction.convertAllToResult(ComFunction.getTCode("C004"));//号旧区分
                List<Object> InOutList = ComFunction.convertAllToResult(ComFunction.getTCode("C003"));//内外区分
                List<Object> OrderPlantList = ComFunction.convertAllToResult(ComFunction.getTCode("C000"));//发注工场
                List<Object> SPISStatusList = ComFunction.convertAllToResult(ComFunction.getTCode("C067"));//ＳＰＩＳ状态

                res.Add("HaoJiuList", HaoJiuList);
                res.Add("InOutList", InOutList);
                res.Add("OrderPlantList", OrderPlantList);
                res.Add("SPISStatusList", SPISStatusList);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M14PE0500", ex, loginInfo.UserId);
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
            string strHaoJiu = dataForm.HaoJiu == null ? "" : dataForm.HaoJiu;
            string strInOut = dataForm.InOut == null ? "" : dataForm.InOut;
            string strOrderPlant = dataForm.OrderPlant == null ? "" : dataForm.OrderPlant;
            string strFrom = dataForm.From == null ? "" : dataForm.From;
            string strTo = dataForm.To == null ? "" : dataForm.To;
            string strCarModel = dataForm.CarModel == null ? "" : dataForm.CarModel;
            string strSPISStatus = dataForm.SPISStatus == null ? "" : dataForm.SPISStatus;
            List<Object> listTime = dataForm.TimeList.ToObject<List<Object>>();

            try
            {
                DataTable dataTable = fS1405_Logic.getSearchInfo(strPartId, strSupplierId, strHaoJiu, strInOut, strOrderPlant, strFrom, strTo, strCarModel, strSPISStatus, listTime);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M14PE0501", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 作成依赖发送方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string sendtoApi([FromBody]dynamic data)
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
                string strToTime_SPIS = dataForm.info;//品番SPIS生效
                string strEmailBody = dataForm.mailboy;//邮件体
                JArray checkedInfo = dataForm.selectmultiple.multipleSelection;
                List<Dictionary<string, Object>> checkedInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();

                DataTable dtMessage = fS0603_Logic.createTable("MES");
                if (strToTime_SPIS == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "品番SPIS生效日期不能为空。";
                    dtMessage.Rows.Add(dataRow);
                }
                if (strToTime_SPIS != "" && Convert.ToDateTime(strToTime_SPIS + " 23:59:59") < System.DateTime.Now)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "品番SPIS生效日期不能小于当前时间。";
                    dtMessage.Rows.Add(dataRow);
                }
                if (strEmailBody == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "邮件体不能为空(请点击邮件预览按钮)。";
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
                DataTable dtImport = fS0603_Logic.createTable("SPISApply");
                fS1405_Logic.checksendtoInfo(checkedInfoData, ref dtImport, strToTime_SPIS, loginInfo.UserId, loginInfo.UserName, ref dtMessage);
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string strTheme = "品番检查SPIS作成依赖";
                DataTable dtToList = fS1405_Logic.getToList(dtImport, ref dtMessage);
                fS1405_Logic.sendEmailInfo(loginInfo.UserId, loginInfo.UserName, loginInfo.Email, strTheme, strEmailBody, dtToList, ref dtMessage);
                if (dtMessage.Rows.Count != 0)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "依赖发送成功。";
                    dtMessage.Rows.InsertAt(dataRow, 0);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M14PE0502", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "依赖发送失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 承认方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string admitApi([FromBody]dynamic data)
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

                JArray checkedInfo = dataForm.selectmultiple.multipleSelection;
                List<Dictionary<string, Object>> checkedInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();

                DataTable dtMessage = fS0603_Logic.createTable("MES");
                DataTable dtImport = fS0603_Logic.createTable("SPISApply");
                DataTable dtApplyList = dtImport.Clone();
                DataTable dtSPISTime = fS0603_Logic.createTable("savFs1404");
                DataTable dtPDF_temp = fS1406_Logic.getTempDataInfo();
                fS1405_Logic.checkadmitInfo(checkedInfoData, ref dtImport, ref dtApplyList, ref dtPDF_temp, ref dtSPISTime, loginInfo.UserId, loginInfo.UserName, ref dtMessage);
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                {
                    //弹出错误dtMessage
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //处理图像
                //1.插入并打印
                DataTable dtPrinterInfo = fS0603_Logic.getPrinterInfo("SPIS生成", "");
                if (dtPrinterInfo.Rows.Count != 0)
                {
                    for (int i = 0; i < dtPDF_temp.Rows.Count; i++)
                    {
                        DataRow drPDF_temp = dtPDF_temp.Rows[i];
                        string sources_pdf = drPDF_temp["vcPDFPath"].ToString();
                        string sources_sips = drPDF_temp["vcSPISPath"].ToString();
                        fS1406_Logic.setCRVtoPdf(drPDF_temp, loginInfo.UserId, ref dtMessage);
                        #region 调用webApiPDF导出
                        //创建 HTTP 绑定对象
                        string file_crv = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "CryReports" + Path.DirectorySeparatorChar;
                        var binding = new BasicHttpBinding();
                        //根据 WebService 的 URL 构建终端点对象
                        var endpoint = new EndpointAddress(dtPrinterInfo.Rows[0]["vcWebAPI"].ToString());
                        //创建调用接口的工厂，注意这里泛型只能传入接口
                        var factory = new ChannelFactory<WebServiceAPISoap>(binding, endpoint);
                        //从工厂获取具体的调用实例
                        var callClient = factory.CreateChannel();
                        setCRVToIMGRequestBody Body = new setCRVToIMGRequestBody();
                        Body.strScrpit = "select * from tPrintTemp_FS1406 WHERE vcOperator='" + loginInfo.UserId + "' ORDER BY LinId";
                        Body.strPdfFileName = sources_pdf;
                        Body.strImgFileName = sources_sips;
                        Body.strCRVName = file_crv + dtPrinterInfo.Rows[0]["vcReports"].ToString();
                        Body.sqlUserID = dtPrinterInfo.Rows[0]["vcSqlUserID"].ToString();
                        Body.sqlPassword = dtPrinterInfo.Rows[0]["vcSqlPassword"].ToString();
                        Body.sqlCatalog = dtPrinterInfo.Rows[0]["vcSqlCatalog"].ToString();
                        Body.sqlSource = dtPrinterInfo.Rows[0]["vcSqlSource"].ToString();
                        //调用具体的方法，这里是 HelloWorldAsync 方法
                        Task<setCRVToIMGResponse> responseTask = callClient.setCRVToIMGAsync(new setCRVToIMGRequest(Body));
                        //获取结果
                        setCRVToIMGResponse response = responseTask.Result;
                        if (response.Body.setCRVToIMGResult != "导出成功")
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
                    ////2.PDF转SPIS图片
                    //fS1406_Logic.setPdftoImgs(dtApplyList, loginInfo.UserId, ref dtMessage);
                    //3.保存数据
                    fS1405_Logic.admitInfo(dtApplyList, dtSPISTime, loginInfo.UserId, ref dtMessage);//更新
                }
                else
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "没有接口信息，请联系管理员维护。";
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
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M14PE0503", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "承认失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 驳回方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string rejectApi([FromBody]dynamic data)
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
                JArray checkedInfo = dataForm.selectmultiple.multipleSelection;
                List<Dictionary<string, Object>> checkedInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();

                DataTable dtMessage = fS0603_Logic.createTable("MES");
                fS1405_Logic.checkrejectInfo(checkedInfoData, loginInfo.UserId, loginInfo.UserName, ref dtMessage);
                if (dtMessage != null && dtMessage.Rows.Count != 0)
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M14PE0504", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "驳回失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 邮件预览
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string emailBodyApi([FromBody]dynamic data)
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
            string strToTime = dataForm.ToTime_SPIS == null ? "" : dataForm.ToTime_SPIS;
            string strFlag = dataForm.flag == null ? "" : dataForm.flag;
            try
            {
                String emailBody = fS1405_Logic.setEmailBody(strToTime, strFlag);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = emailBody;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M14PE0505", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "邮件预览失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
    }
}
