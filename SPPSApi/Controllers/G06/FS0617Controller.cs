using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
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

namespace SPPSApi.Controllers.G06
{
    [Route("api/FS0617/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0617Controller : BaseController
    {
        FS0603_Logic fS0603_Logic = new FS0603_Logic();
        FS0617_Logic fS0617_Logic = new FS0617_Logic();
        private readonly string FunctionID = "FS0617";

        private readonly IWebHostEnvironment _webHostEnvironment;
        public FS0617Controller(IWebHostEnvironment webHostEnvironment)
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
                DataTable dtOptionsList = fS0603_Logic.getFormOptions("0");
                List<Object> OrderPlantForForm = ComFunction.convertAllToResult(fS0603_Logic.getSelectOptions(dtOptionsList, "vcOrderPlant_Name", "vcOrderPlant_Value"));//发注工厂选项
                List<Object> CarModelForForm = ComFunction.convertAllToResult(fS0603_Logic.getSelectOptions(dtOptionsList, "vcCarModel_Name", "vcCarModel_Value"));//车种选项
                List<Object> ReceiverForForm = ComFunction.convertAllToResult(fS0603_Logic.getSelectOptions(dtOptionsList, "vcReceiver_Name", "vcReceiver_Value"));//收货方选项
                List<Object> SupplierIdForForm = ComFunction.convertAllToResult(fS0603_Logic.getSelectOptions(dtOptionsList, "vcSupplierId_Name", "vcSupplierId_Value"));//供应商编码选项
                res.Add("OrderPlantForForm", OrderPlantForForm);
                res.Add("CarModelForForm", CarModelForForm);
                res.Add("ReceiverForForm", ReceiverForForm);
                res.Add("SupplierIdForForm", SupplierIdForForm);
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

            string strOrderPlant = dataForm.OrderPlant == null ? "" : dataForm.OrderPlant;
            string strPartId = dataForm.PartId == null ? "" : dataForm.PartId;
            string strCarModel = dataForm.CarModel == null ? "" : dataForm.CarModel;
            string strReceiver = dataForm.Receiver == null ? "" : dataForm.Receiver;
            string strSupplier = dataForm.Supplier == null ? "" : dataForm.Supplier;
            try
            {
                DataTable dataTable = fS0617_Logic.getSearchInfo(strOrderPlant, strPartId, strCarModel, strReceiver, strSupplier);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("bModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("bAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("bSelectFlag", ConvertFieldType.BoolType, null);
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dataTable, dtConverter);
                res.Add("tempList", dataList);
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
            JArray listInfo = dataForm.multipleSelection;
            List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
            try
            {
                string imagefile_sp = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Image" + Path.DirectorySeparatorChar + "SPPartImage" + Path.DirectorySeparatorChar;
                string imagefile_qr = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Image" + Path.DirectorySeparatorChar + "QRcodeImage" + Path.DirectorySeparatorChar;
                string strPrintName = "";//打印机
                string strReportName = "fs0617_cry.rpt";//水晶报表模板
                string strPrintData = "tPrintTemp_main_FS0617";//数据表
                DataTable dtMessage = fS0603_Logic.createTable("MES");
                if (listInfoData.Count != 0)
                {
                    bool bResult = fS0617_Logic.getPrintInfo(listInfoData, imagefile_sp, imagefile_qr, loginInfo.UserId, ref dtMessage);
                    if(!bResult)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.type = "list";
                        apiResult.data = dtMessage;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    //引进打印调用
                    //主表    tPrintTemp_main_FS0617



                    if(!bResult)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.type = "list";
                        apiResult.data = dtMessage;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0204", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "生成印刷文件失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
    }
}
