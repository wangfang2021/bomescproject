using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
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
        /// 导出方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string exportApi([FromBody] dynamic data)
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
                string[] fields = { "vcPartId", "vcPartENName", "dFromTime", "dToTime", "vcCarfamilyCode", "vcOrderPlant", "vcSupplierId", "vcSupplierPlant", "vcInOut", "vcHaoJiu", "vcPackType", "vcCheckType", "vcSPISStatus" };

                string filepath = ComFunction.generateExcelWithXlt(dataTable, fields, _webHostEnvironment.ContentRootPath, "FS1401_Export.xlsx", 1, loginInfo.UserId, FunctionID);
                if (filepath == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "导出生成文件失败";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = filepath;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0904", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
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
                DataTable dtImport = fS1406_Logic.checkreplyInfo(checkedInfoData, loginInfo.UserId, loginInfo.UnitCode, ref dtMessage);
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

                string strApplyId = listInfoData[0]["vcApplyId"] == null ? "" : listInfoData[0]["vcApplyId"].ToString();
                string strSPISStatus = listInfoData[0]["vcSPISStatus"] == null ? "" : listInfoData[0]["vcSPISStatus"].ToString();
                string strPartId = listInfoData[0]["vcPartId"] == null ? "" : listInfoData[0]["vcPartId"].ToString();
                string strPartENName = listInfoData[0]["vcPartENName"] == null ? "" : listInfoData[0]["vcPartENName"].ToString();
                string strCarfamilyCode = listInfoData[0]["vcCarfamilyCode"] == null ? "" : listInfoData[0]["vcCarfamilyCode"].ToString();
                string strSupplierId = listInfoData[0]["vcSupplierId"] == null ? "" : listInfoData[0]["vcSupplierId"].ToString();
                string strPicUrl = listInfoData[0]["vcPicUrl"] == null ? "" : listInfoData[0]["vcPicUrl"].ToString();
                string strSPISUrl = listInfoData[0]["vcSPISUrl"] == null ? "" : listInfoData[0]["vcSPISUrl"].ToString();
                string strModItem = listInfoData[0]["vcModItem"] == null ? "" : listInfoData[0]["vcModItem"].ToString();

                string strApplyIdItem = strApplyId;
                string strSPISStatusItem = strSPISStatus;
                string strSPISTimeItem = System.DateTime.Now.ToString("yyyy/MM/dd");
                string strPartIdItem = strPartId;
                string strCarfamilyCodeItem = strCarfamilyCode;
                string strSupplierIdItem = strSupplierId;
                string strPartENNameItem = strPartENName;
                string strColourNoItem = string.Empty;
                string strColourCodeItem = string.Empty;
                string strColourNameItem = string.Empty;
                string strModItemItem = strModItem;
                string strPicUrlItem = strPicUrl;
                string strSPISUrlItem = strSPISUrl;
                string strModelItem = "NG";
                DataTable dtApply = fS1406_Logic.getSearchInfo(strApplyId);
                if (dtApply.Rows.Count != 0)
                {
                    strApplyIdItem = dtApply.Rows[0]["vcApplyId"].ToString();
                    strSPISTimeItem = dtApply.Rows[0]["dSPISTime"].ToString();
                    strColourNoItem = dtApply.Rows[0]["vcColourNo"].ToString();
                    strColourCodeItem = dtApply.Rows[0]["vcColourCode"].ToString();
                    strColourNameItem = dtApply.Rows[0]["vcColourName"].ToString();
                    strModItemItem = dtApply.Rows[0]["vcModItem"].ToString();
                    strPicUrlItem = dtApply.Rows[0]["vcPicUrl"].ToString();
                }
                if(strSPISStatusItem=="0"|| strSPISStatusItem == "1" || strSPISStatusItem == "4")
                    strModelItem = "OK";
                res.Add("ApplyIdItem", strApplyIdItem);
                res.Add("SPISStatusItem", strSPISStatusItem);
                res.Add("modelItem", strModelItem);
                res.Add("SPISTimeItem", strSPISTimeItem);
                res.Add("PartIdItem", strPartIdItem);
                res.Add("CarfamilyCodeItem", strCarfamilyCodeItem);
                res.Add("SupplierIdItem", strSupplierIdItem);
                res.Add("PartENNameItem", strPartENNameItem);
                res.Add("ColourNoItem", strColourNoItem);
                res.Add("ColourCodeItem", strColourCodeItem);
                res.Add("ColourNameItem", strColourNameItem);
                res.Add("PicUrlItem", strPicUrlItem);
                res.Add("ModItemItem", strModItemItem);
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
                string strModel = dataForm.model==null?"": dataForm.model.ToString();
                string strApplyId = dataForm.ApplyId == null?"": dataForm.ApplyId.ToString();
                string strSPISStatus = dataForm.SPISStatus == null?"": dataForm.SPISStatus.ToString();
                string strSPISTime = dataForm.SPISTime == null ? "" : dataForm.SPISTime.ToString();
                string strPartId = dataForm.PartId == null ? "" : dataForm.PartId.ToString();
                string strCarfamilyCode = dataForm.CarfamilyCode == null ? "" : dataForm.CarfamilyCode.ToString();
                string strSupplierId = dataForm.SupplierId == null ? "" : dataForm.SupplierId.ToString();
                string strPartENName = dataForm.PartENName == null ? "" : dataForm.PartENName.ToString();
                string strColourNo = dataForm.ColourNo == null ? "" : dataForm.ColourNo.ToString();
                string strColourCode = dataForm.ColourCode == null ? "" : dataForm.ColourCode.ToString();
                string strColourName = dataForm.ColourName == null ? "" : dataForm.ColourName.ToString();
                string strModItem = dataForm.ModItem == null ? "" : dataForm.ModItem.ToString();
                string strOperImage = dataForm.PicRoutes == null ? "" : dataForm.PicRoutes.ToString();
                string strDelImageRoutes = dataForm.DelPicRoutes == null ? "" : dataForm.DelPicRoutes.ToString();
                DataTable dtImport = fS0603_Logic.createTable("SPISApply");
                DataRow drImport = dtImport.NewRow();
                drImport["LinId"] = "";
                drImport["vcApplyId"] = strApplyId;
                drImport["dFromTime_SPIS"] = "";
                drImport["dToTime_SPIS"] = "";
                drImport["dSPISTime"] = strSPISTime;
                drImport["vcPartId"] = strPartId;
                drImport["vcCarfamilyCode"] = strCarfamilyCode;
                drImport["vcSupplierId"] = strSupplierId;
                drImport["vcPartENName"] = strPartENName;
                drImport["vcColourNo"] = strColourNo;
                drImport["vcColourCode"] = strColourCode;
                drImport["vcColourName"] = strColourName;
                drImport["vcModItem"] = strModItem;
                drImport["vcPicUrl"] = strOperImage;
                drImport["vcPDFUrl"] = string.Empty;
                drImport["vcSPISUrl"] = string.Empty;
                drImport["vcSupplier_1"] = string.Empty;
                drImport["vcSupplier_2"] = string.Empty;
                drImport["vcOperName"] = string.Empty;
                drImport["vcGM"] = string.Empty;
                drImport["vcSPISStatus"] = strSPISStatus;
                dtImport.Rows.Add(drImport);
                DataTable dtMessage = fS0603_Logic.createTable("MES");
                string strPath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "upload_spisapply" + Path.DirectorySeparatorChar + "apply" + Path.DirectorySeparatorChar;
                DataTable dtInfo = fS1406_Logic.checkSaveInfo(dtImport, strPath, loginInfo.UserId, ref dtMessage);
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                {
                    dtMessage = dtMessage.DefaultView.ToTable(true, "vcMessage");
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fS1406_Logic.setSaveInfo(dtImport, loginInfo.UserId, ref dtMessage);
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
