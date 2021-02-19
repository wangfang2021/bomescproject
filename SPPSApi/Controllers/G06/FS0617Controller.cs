using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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

                List<Object> PlantAreaList = ComFunction.convertAllToResult(ComFunction.getTCode("C017"));//工区
                List<Object> OrderPlantList = ComFunction.convertAllToResult(ComFunction.getTCode("C000"));//发注工厂
                List<Object> CarModelList = ComFunction.convertAllToResult(ComFunction.getTCode("C098"));//车种
                List<Object> ReceiverList = ComFunction.convertAllToResult(fS0603_Logic.getCodeInfo("Receiver"));//收货方
                List<Object> SupplierList = ComFunction.convertAllToResult(fS0603_Logic.getCodeInfo("Supplier"));//供应商
                res.Add("PlantAreaList", PlantAreaList);
                res.Add("OrderPlantList", OrderPlantList);
                res.Add("CarModelList", CarModelList);
                res.Add("ReceiverList", ReceiverList);
                res.Add("SupplierList", SupplierList);

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

            string strPlantArea = dataForm.PlantArea == null ? "" : dataForm.PlantArea;
            string strOrderPlant = dataForm.OrderPlant == null ? "" : dataForm.OrderPlant;
            string strPartId = dataForm.PartId == null ? "" : dataForm.PartId;
            string strCarModel = dataForm.CarModel == null ? "" : dataForm.CarModel;
            string strReceiver = dataForm.Receiver == null ? "" : dataForm.Receiver;
            string strSupplier = dataForm.Supplier == null ? "" : dataForm.Supplier;
            try
            {
                DataTable dataTable = fS0617_Logic.getSearchInfo(strPlantArea, strOrderPlant, strPartId, strCarModel, strReceiver, strSupplier);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("dFromTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dToTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dFrontProjectTime", ConvertFieldType.DateType, "yyyy/MM/dd HH:mm");
                dtConverter.addField("dShipmentTime", ConvertFieldType.DateType, "yyyy/MM/dd HH:mm");
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dataTable, dtConverter);

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
                if (listInfoData.Count != 0)
                {
                    //获取待打印的数据
                    //DataTable dataTable = fS0617_Logic.getPrintInfo(listInfoData);
                    //执行打印操作
                    //===========================================
                    DataTable dtMessage = fS0603_Logic.createTable("MES");
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "错误测试";
                    dtMessage.Rows.Add(dataRow);

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);




                    //===========================================
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
