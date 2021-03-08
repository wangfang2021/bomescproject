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

namespace SPPSApi.Controllers.G06
{
    [Route("api/FS0603/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0603Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        FS0603_Logic fs0603_Logic = new FS0603_Logic();
        private readonly string FunctionID = "FS0603";
        public FS0603Controller(IWebHostEnvironment webHostEnvironment)
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
                DataTable dtOptionsList = fs0603_Logic.getFormOptions("");
                List<Object> CarModelForForm = ComFunction.convertAllToResult(fs0603_Logic.getSelectOptions(dtOptionsList, "vcCarModel_Name", "vcCarModel_Value"));//车种选项
                List<Object> ReceiverForForm = ComFunction.convertAllToResult(fs0603_Logic.getSelectOptions(dtOptionsList, "vcReceiver_Name", "vcReceiver_Value"));//收货方选项
                List<Object> InOutForForm = ComFunction.convertAllToResult(fs0603_Logic.getSelectOptions(dtOptionsList, "vcInOut_Name", "vcInOut_Value"));//内外区分选项
                List<Object> SupplierIdForForm = ComFunction.convertAllToResult(fs0603_Logic.getSelectOptions(dtOptionsList, "vcSupplierId_Name", "vcSupplierId_Value"));//供应商编码选项
                List<Object> SupplierPlantForForm = ComFunction.convertAllToResult(fs0603_Logic.getSelectOptions(dtOptionsList, "vcSupplierPlant_Name", "vcSupplierPlant_Value"));//工区
                List<Object> FromTimeForForm = ComFunction.convertAllToResult(fs0603_Logic.getSelectOptions(dtOptionsList, "vcFromTime_Name", "vcFromTime_Value"));//开始使用选项
                List<Object> ToTimeForForm = ComFunction.convertAllToResult(fs0603_Logic.getSelectOptions(dtOptionsList, "vcToTime_Name", "vcToTime_Value"));//结束使用选项
                List<Object> HaoJiuForForm = ComFunction.convertAllToResult(fs0603_Logic.getSelectOptions(dtOptionsList, "vcHaoJiu_Name", "vcHaoJiu_Value"));//号旧区分选项
                List<Object> OrderPlantForForm = ComFunction.convertAllToResult(fs0603_Logic.getSelectOptions(dtOptionsList, "vcOrderPlant_Name", "vcOrderPlant_Value"));//发注工厂选项
                List<Object> SufferInForForm = ComFunction.convertAllToResult(fs0603_Logic.getSelectOptions(dtOptionsList, "vcSufferIn_Name", "vcSufferIn_Value"));//受入选项
                List<Object> SupplierPackingForForm = ComFunction.convertAllToResult(fs0603_Logic.getSelectOptions(dtOptionsList, "vcSupplierPacking_Name", "vcSupplierPacking_Value"));//供应商包装选项
                List<Object> OldProductionForForm = ComFunction.convertAllToResult(fs0603_Logic.getSelectOptions(dtOptionsList, "vcOldProduction_Name", "vcOldProduction_Value"));//旧型年限生产区分选项
                List<Object> DebugTimeForForm = ComFunction.convertAllToResult(fs0603_Logic.getSelectOptions(dtOptionsList, "vcDebugTime_Name", "vcDebugTime_Value"));//实施年月选项
                List<Object> BoxTypeForForm = ComFunction.convertAllToResult(fs0603_Logic.getSelectOptions(dtOptionsList, "vcBoxType_Name", "vcBoxType_Value"));//箱种

                List<Object> ChangesList = ComFunction.convertAllToResult(ComFunction.getTCode("C002"));//变更事项
                List<Object> PackingPlantList = ComFunction.convertAllToResult(ComFunction.getTCode("C017"));//包装工厂
                List<Object> ReceiverList = ComFunction.convertAllToResult(fs0603_Logic.getCodeInfo("Receiver"));//收货方
                List<Object> InOutList = ComFunction.convertAllToResult(ComFunction.getTCode("C003"));//内外区分
                List<Object> OESPList = ComFunction.convertAllToResult(ComFunction.getTCode("C012"));//OE=SP
                List<Object> HaoJiuList = ComFunction.convertAllToResult(ComFunction.getTCode("C004"));//号旧区分
                List<Object> OldProductionList = ComFunction.convertAllToResult(ComFunction.getTCode("C024"));//旧型年限生产区分
                List<Object> BillTypeList = ComFunction.convertAllToResult(ComFunction.getTCode("C007"));//单据区分
                List<Object> OrderingMethodList = ComFunction.convertAllToResult(ComFunction.getTCode("C047"));//订货方式
                List<Object> MandOrderList = ComFunction.convertAllToResult(ComFunction.getTCode("C048"));//强制订货
                List<Object> SupplierPackingList = ComFunction.convertAllToResult(ComFunction.getTCode("C059"));//供应商包装
                List<Object> OrderPlantList = ComFunction.convertAllToResult(ComFunction.getTCode("C000"));//发注工厂

                res.Add("CarModelForForm", CarModelForForm);
                res.Add("ReceiverForForm", ReceiverForForm);
                res.Add("InOutForForm", InOutForForm);
                res.Add("SupplierIdForForm", SupplierIdForForm);
                res.Add("SupplierPlantForForm", SupplierPlantForForm);
                res.Add("FromTimeForForm", FromTimeForForm);
                res.Add("ToTimeForForm", ToTimeForForm);
                res.Add("HaoJiuForForm", HaoJiuForForm);
                res.Add("OrderPlantForForm", OrderPlantForForm);
                res.Add("SufferInForForm", SufferInForForm);
                res.Add("SupplierPackingForForm", SupplierPackingForForm);
                res.Add("OldProductionForForm", OldProductionForForm);
                res.Add("DebugTimeForForm", DebugTimeForForm);
                res.Add("BoxTypeForForm", BoxTypeForForm);

                res.Add("ChangesList", ChangesList);
                res.Add("PackingPlantList", PackingPlantList);
                res.Add("ReceiverList", ReceiverList);
                res.Add("InOutList", InOutList);
                res.Add("OESPList", OESPList);
                res.Add("HaoJiuList", HaoJiuList);
                res.Add("OldProductionList", OldProductionList);
                res.Add("BillTypeList", BillTypeList);
                res.Add("OrderingMethodList", OrderingMethodList);
                res.Add("MandOrderList", MandOrderList);
                res.Add("SupplierPackingList", SupplierPackingList);
                res.Add("OrderPlantList", OrderPlantList);
                DataTable dttaskNum = fs0603_Logic.gettaskNum();
                string taskoutNum = "0";
                string taskinNum = "0";
                if (!(dttaskNum == null || dttaskNum.Rows.Count == 0))
                {
                    taskoutNum = dttaskNum.Select("vcInOut='1'").Length.ToString();
                    taskinNum = dttaskNum.Select("vcInOut='0'").Length.ToString();
                }
                res.Add("taskoutNum", taskoutNum);
                res.Add("taskinNum", taskinNum);
                res.Add("synclist", fs0603_Logic.getSyncInfo());
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
            Dictionary<string, object> res = new Dictionary<string, object>();

            string strSyncTime = dataForm.SyncTime;
            string strPartId = dataForm.PartId;
            string strCarModel = dataForm.CarModel;
            string strReceiver = dataForm.Receiver;
            string strInOut = dataForm.InOut;
            string strHaoJiu = dataForm.HaoJiu;
            string strSupplierId = dataForm.SupplierId;
            string strSupplierPlant = dataForm.SupplierPlant;
            string strOrderPlant = dataForm.OrderPlant;
            string strFromTime = dataForm.FromTime;
            string strToTime = dataForm.ToTime;
            string strBoxType = dataForm.BoxType;
            string strSufferIn = dataForm.SufferIn;
            string strSupplierPacking = dataForm.SupplierPacking;
            string strOldProduction = dataForm.OldProduction;
            string strDebugTime = dataForm.DebugTime;
            try
            {
                DataTable dataTable = fs0603_Logic.getSearchInfo(strSyncTime, strPartId, strCarModel, strReceiver, strInOut, strHaoJiu, strSupplierId, strSupplierPlant,
                    strOrderPlant, strFromTime, strToTime, strBoxType, strSufferIn, strSupplierPacking, strOldProduction, strDebugTime, "", false);

                DataTable dttaskNum = fs0603_Logic.gettaskNum();
                string taskoutNum = "0";
                string taskinNum = "0";
                if (!(dttaskNum == null || dttaskNum.Rows.Count == 0))
                {
                    taskoutNum = dttaskNum.Select("vcInOut='1'").Length.ToString();
                    taskinNum = dttaskNum.Select("vcInOut='0'").Length.ToString();
                }
                res.Add("taskoutNum", taskoutNum);
                res.Add("taskinNum", taskinNum);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("bModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("bAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("bSelectFlag", ConvertFieldType.BoolType, null);
                List<Object> dataList = null;
                if (dataTable.Rows.Count > 10000)
                {
                    DataTable dtMessage = fs0603_Logic.createTable("MES");
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "本次检索数据条数超过10000,为避免浏览器内存溢出，请调整检索条件或进行数据导出。";
                    dtMessage.Rows.Add(dataRow);

                    DataTable table = dataTable.Clone();
                    dataList = ComFunction.convertAllToResultByConverter(table, dtConverter);
                    res.Add("tempList", dataList);
                    res.Add("messageList", dtMessage);
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "message";
                    apiResult.data = res;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                else
                {
                    dataList = ComFunction.convertAllToResultByConverter(dataTable, dtConverter);
                    res.Add("tempList", dataList);
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.data = res;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

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

            string strSyncTime = dataForm.SyncTime;
            string strPartId = dataForm.PartId;
            string strCarModel = dataForm.CarModel;
            string strReceiver = dataForm.Receiver;
            string strInOut = dataForm.InOut;
            string strHaoJiu = dataForm.HaoJiu;
            string strSupplierId = dataForm.SupplierId;
            string strSupplierPlant = dataForm.SupplierPlant;
            string strOrderPlant = dataForm.OrderPlant;
            string strFromTime = dataForm.FromTime;
            string strToTime = dataForm.ToTime;
            string strBoxType = dataForm.BoxType;
            string strSufferIn = dataForm.SufferIn;
            string strSupplierPacking = dataForm.SupplierPacking;
            string strOldProduction = dataForm.OldProduction;
            string strDebugTime = dataForm.DebugTime;
            try
            {
                DataTable dtMessage = fs0603_Logic.createTable("MES");
                DataTable dtMainInfo = fs0603_Logic.getSearchInfo(strSyncTime, strPartId, strCarModel, strReceiver, strInOut, strHaoJiu, strSupplierId, strSupplierPlant,
                    strOrderPlant, strFromTime, strToTime, strBoxType, strSufferIn, strSupplierPacking, strOldProduction, strDebugTime, "", false);
                DataTable dtSPInfo = fs0603_Logic.getSubInfo("SupplierPlantEdit", "", strPartId, strReceiver, strSupplierId, "");
                DataTable dtPQInfo = fs0603_Logic.getSubInfo("PackingQtyEdit", "", strPartId, strReceiver, strSupplierId, "");
                DataTable dtSIInfo = fs0603_Logic.getSubInfo("SufferInEdit", "", strPartId, strReceiver, strSupplierId, "");
                DataTable dtOPInfo = fs0603_Logic.getSubInfo("OrderPlantEdit", "", "", "", strSupplierId, "");

                DataTable dtExport = fs0603_Logic.setExportInfo(dtMainInfo, dtSPInfo, dtPQInfo, dtSIInfo, dtOPInfo, ref dtMessage);


                string[] fields = {"dSyncTime","vcChanges_name","vcPackingPlant_name","vcPartId","vcPartENName","vcCarfamilyCode","vcReceiver","dFromTime","dToTime",
"vcPartId_Replace","vcInOut_name","vcOESP_name","vcHaoJiu_name","vcOldProduction_name","dDebugTime","vcSupplierId","dSupplierFromTime",
"dSupplierToTime","vcSupplierName","vcSupplierPlant","dSupplierPlantFromTime","dSupplierPlantToTime","vcSupplierPlace","iPackingQty",
"vcBoxType","iLength","iWidth","iHeight","iVolume","dBoxFromTime","dBoxToTime","vcSufferIn","dSufferInFromTime","dSufferInToTime",
"vcOrderPlant_name","dOrderPlantFromTime","dOrderPlantToTime","vcInteriorProject","vcPassProject","vcFrontProject","dFrontProjectTime",
"dShipmentTime","vcPartImage","vcBillType_name","vcRemark1","vcRemark2","vcOrderingMethod_name","vcMandOrder_name","vcSupplierPacking_name"
};

                string filepath = ComFunction.generateExcelWithXlt(dtExport, fields, _webHostEnvironment.ContentRootPath, "FS0603_Export.xlsx", 1, loginInfo.UserId, FunctionID);
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
                JArray listInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
                bool hasFind = false;//是否找到需要新增或者修改的数据
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["bModFlag"];//true可编辑,false不可编辑
                    bool bAddFlag = (bool)listInfoData[i]["bAddFlag"];//true可编辑,false不可编辑
                    if (bAddFlag == true)
                        hasFind = true;//新增
                    else if (bAddFlag == false && bModFlag == true)
                        hasFind = true;//修改
                }
                if (!hasFind)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "info";
                    apiResult.data = "最少有一个编辑行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //开始数据验证
                DataTable dtMessage = fs0603_Logic.createTable("MES");
                bool bReault = true;
                DataTable dtImport = fs0603_Logic.checkDataInfo(listInfoData, ref bReault, ref dtMessage);
                if (!bReault)
                {
                    //弹出错误dtMessage
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fs0603_Logic.setSPInfo(dtImport, loginInfo.UserId, ref dtMessage);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0902", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.type = "info";
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 删除方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string delApi([FromBody]dynamic data)
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
                List<Dictionary<string, Object>> listInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();
                if (listInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选择一条数据！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fs0603_Logic.deleteInfo(listInfoData, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0903", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 子页面初始化（工区SupplierPlantEdit；收容数PackingQtyEdit；受入SufferInEdit；发注工厂OrderPlantEdit）
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string editpageloadApi([FromBody] dynamic data)
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
            try
            {
                string strEditType = dataForm.type;//维护类型
                dynamic dRowinfo = dataForm.rowinfo;//维护数据信息

                string strPackingPlant = dRowinfo.vcPackingPlant == null ? "" : dRowinfo.vcPackingPlant.ToString().Trim();
                string strPackingPlant_name = dRowinfo.vcPackingPlant_name == null ? "" : dRowinfo.vcPackingPlant_name.ToString().Trim();
                string strPartId = dRowinfo.vcPartId == null ? "" : dRowinfo.vcPartId.ToString().ToUpper().Trim();
                string strReceiver = dRowinfo.vcReceiver == null ? "" : dRowinfo.vcReceiver.ToString().Trim();
                string strSupplierId = dRowinfo.vcSupplierId == null ? "" : dRowinfo.vcSupplierId.ToString().Trim();

                string strFromTime = dRowinfo.dFromTime == null ? "" : dRowinfo.dFromTime.ToString().Trim();
                string strToTime = dRowinfo.dToTime == null ? "" : dRowinfo.dToTime.ToString().Trim();


                DataTable dtMessage = fs0603_Logic.createTable("MES");

                if (strPackingPlant == "" || strPartId == "" || strReceiver == "" || strSupplierId == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "原单位4Key未维护完整不能进行手配维护";
                    dtMessage.Rows.Add(dataRow);
                }
                if (strFromTime == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "原单位开始使用时间未维护完整不能进行手配维护";
                    dtMessage.Rows.Add(dataRow);
                }
                else
                    strFromTime = Convert.ToDateTime(strFromTime).ToString("yyyy-MM-dd");
                if (strToTime == "")
                    strToTime = "9999-12-31";
                else
                    strToTime = Convert.ToDateTime(strToTime).ToString("yyyy-MM-dd");
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                {
                    //弹出错误dtMessage
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                bool bAddFlag = (bool)dRowinfo.bAddFlag;//true可编辑,false不可编辑
                if (bAddFlag == true)
                {
                    DataTable dtSPInfo = fs0603_Logic.getSearchInfo("", strPartId, "", strReceiver, "", "", strSupplierId, "", "", "", "", "", "", "", "", "", strPackingPlant, true);
                    if (dtSPInfo.Rows.Count != 0)
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = "追加的原单位信息4key重复，禁止操作";
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
                //获取包装工厂名称
                if (strPackingPlant_name == "")
                {
                    DataTable dtPackingPlant = ComFunction.getTCode("C017");
                    DataRow[] drPackingPlant = dtPackingPlant.Select("vcValue='" + strPackingPlant + "'");
                    if (drPackingPlant.Length != 0)
                        strPackingPlant_name = drPackingPlant[0]["vcName"].ToString();
                }
                if (strEditType == "SupplierPlantEdit")
                {
                    string SupplierPlant_ed = dRowinfo.SupplierPlant_ed == null ? "" : dRowinfo.SupplierPlant_ed.ToString();
                    string SupplierPlantLinId_ed = dRowinfo.SupplierPlantLinId_ed == null ? "" : dRowinfo.SupplierPlantLinId_ed.ToString();
                    string SupplierPlantFromTime_ed = dRowinfo.SupplierPlantFromTime_ed == null ? "" : dRowinfo.SupplierPlantFromTime_ed.ToString();
                    string SupplierPlantToTime_ed = dRowinfo.SupplierPlantToTime_ed == null ? "" : dRowinfo.SupplierPlantToTime_ed.ToString();

                    DataTable dataTable = fs0603_Logic.getEditLoadInfo(strEditType, strPackingPlant, strPartId, strReceiver, strSupplierId, "");
                    if (SupplierPlantLinId_ed != "" && dataTable.Select("LinId='" + SupplierPlantLinId_ed + "'").Length == 0)//数据库中不是最新有效期履历
                    {
                        DataRow dataRow = dataTable.NewRow();
                        dataRow["LinId"] = SupplierPlantLinId_ed;
                        dataRow["dFromTime"] = SupplierPlantFromTime_ed;
                        dataRow["dToTime"] = SupplierPlantToTime_ed;
                        dataRow["vcSupplierPlant"] = SupplierPlant_ed;
                        dataRow["bAddFlag"] = "1";
                        dataRow["vcBgColor"] = "";
                        dataTable.Rows.InsertAt(dataRow, 0);//插入到第一行
                    }
                    DtConverter dtConverter = new DtConverter();
                    dtConverter.addField("bAddFlag", ConvertFieldType.BoolType, null);
                    dtConverter.addField("dFromTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                    dtConverter.addField("dToTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                    List<Object> dataList = ComFunction.convertAllToResultByConverter(dataTable, dtConverter);
                    res.Add("packingPlantItem", strPackingPlant);
                    res.Add("packingPlant_nameItem", strPackingPlant_name);
                    res.Add("partIdItem", strPartId);
                    res.Add("receiverItem", strReceiver);
                    res.Add("supplierIdItem", strSupplierId);
                    res.Add("fromtimeItem", strFromTime);
                    res.Add("totimeItem", strToTime);
                    res.Add("tempList", dataList);
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.data = res;

                }
                if (strEditType == "PackingQtyEdit")
                {
                    string iPackingQty = dRowinfo.iPackingQty == null ? "" : dRowinfo.iPackingQty.ToString();
                    string vcBoxType = dRowinfo.vcBoxType == null ? "" : dRowinfo.vcBoxType.ToString();
                    string iLength = dRowinfo.iLength == null ? "" : dRowinfo.iLength.ToString();
                    string iWidth = dRowinfo.iWidth == null ? "" : dRowinfo.iWidth.ToString();
                    string iHeight = dRowinfo.iHeight == null ? "" : dRowinfo.iHeight.ToString();
                    string iVolume = dRowinfo.iVolume == null ? "" : dRowinfo.iVolume.ToString();

                    string BoxPackingQty_ed = dRowinfo.BoxPackingQty_ed == null ? "" : dRowinfo.BoxPackingQty_ed.ToString();
                    string BoxLinId_ed = dRowinfo.BoxLinId_ed == null ? "" : dRowinfo.BoxLinId_ed.ToString();
                    string BoxFromTime_ed = dRowinfo.BoxFromTime_ed == null ? "" : dRowinfo.BoxFromTime_ed.ToString();
                    string BoxToTime_ed = dRowinfo.BoxToTime_ed == null ? "" : dRowinfo.BoxToTime_ed.ToString();
                    string BoxType_ed = dRowinfo.BoxType_ed == null ? "" : dRowinfo.BoxType_ed.ToString();
                    string BoxLength_ed = dRowinfo.BoxLength_ed == null ? "" : dRowinfo.BoxLength_ed.ToString();
                    string BoxWidth_ed = dRowinfo.BoxWidth_ed == null ? "" : dRowinfo.BoxWidth_ed.ToString();
                    string BoxHeight_ed = dRowinfo.BoxHeight_ed == null ? "" : dRowinfo.BoxHeight_ed.ToString();
                    string BoxVolume_ed = dRowinfo.BoxVolume_ed == null ? "" : dRowinfo.BoxVolume_ed.ToString();

                    DataTable dataTable = fs0603_Logic.getEditLoadInfo(strEditType, strPackingPlant, strPartId, strReceiver, strSupplierId, "");
                    if (BoxPackingQty_ed != "" && dataTable.Select("LinId='" + BoxLinId_ed + "'").Length == 0)//数据库中不是最新有效期履历
                    {
                        DataRow dataRow = dataTable.NewRow();
                        dataRow["LinId"] = BoxLinId_ed;
                        dataRow["iPackingQty"] = BoxPackingQty_ed;
                        dataRow["dFromTime"] = BoxFromTime_ed;
                        dataRow["dToTime"] = BoxToTime_ed;
                        dataRow["vcBoxType"] = BoxType_ed;
                        dataRow["iLength"] = BoxLength_ed;
                        dataRow["iWidth"] = BoxWidth_ed;
                        dataRow["iHeight"] = BoxHeight_ed;
                        dataRow["iVolume"] = BoxVolume_ed;
                        dataRow["bAddFlag"] = "1";
                        dataRow["vcBgColor"] = "";
                        dataTable.Rows.InsertAt(dataRow, 0);//插入到第一行
                    }
                    DtConverter dtConverter = new DtConverter();
                    dtConverter.addField("bAddFlag", ConvertFieldType.BoolType, null);
                    dtConverter.addField("dFromTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                    dtConverter.addField("dToTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                    List<Object> dataList = ComFunction.convertAllToResultByConverter(dataTable, dtConverter);
                    res.Add("packingPlantItem", strPackingPlant);
                    res.Add("packingPlant_nameItem", strPackingPlant_name);
                    res.Add("partIdItem", strPartId);
                    res.Add("receiverItem", strReceiver);
                    res.Add("supplierIdItem", strSupplierId);
                    res.Add("fromtimeItem", strFromTime);
                    res.Add("totimeItem", strToTime);
                    res.Add("tempList", dataList);
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.data = res;

                }
                if (strEditType == "SufferInEdit")
                {
                    string SufferIn_ed = dRowinfo.SufferIn_ed == null ? "" : dRowinfo.SufferIn_ed.ToString();
                    string SufferInLinId_ed = dRowinfo.SufferInLinId_ed == null ? "" : dRowinfo.SufferInLinId_ed.ToString();
                    string SufferInFromTime_ed = dRowinfo.SufferInFromTime_ed == null ? "" : dRowinfo.SufferInFromTime_ed.ToString();
                    string SufferInToTime_ed = dRowinfo.SufferInToTime_ed == null ? "" : dRowinfo.SufferInToTime_ed.ToString();

                    DataTable dataTable = fs0603_Logic.getEditLoadInfo(strEditType, strPackingPlant, strPartId, strReceiver, strSupplierId, "");
                    if (SufferInLinId_ed != "" && dataTable.Select("LinId='" + SufferInLinId_ed + "'").Length == 0)//数据库中不是最新有效期履历
                    {
                        DataRow dataRow = dataTable.NewRow();
                        dataRow["LinId"] = SufferInLinId_ed;
                        dataRow["dFromTime"] = SufferInFromTime_ed;
                        dataRow["dToTime"] = SufferInToTime_ed;
                        dataRow["vcSufferIn"] = SufferIn_ed;
                        dataRow["bAddFlag"] = "1";
                        dataRow["vcBgColor"] = "";
                        dataTable.Rows.InsertAt(dataRow, 0);//插入到第一行
                    }
                    DtConverter dtConverter = new DtConverter();
                    dtConverter.addField("bAddFlag", ConvertFieldType.BoolType, null);
                    dtConverter.addField("dFromTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                    dtConverter.addField("dToTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                    List<Object> dataList = ComFunction.convertAllToResultByConverter(dataTable, dtConverter);
                    res.Add("packingPlantItem", strPackingPlant);
                    res.Add("packingPlant_nameItem", strPackingPlant_name);
                    res.Add("partIdItem", strPartId);
                    res.Add("receiverItem", strReceiver);
                    res.Add("supplierIdItem", strSupplierId);
                    res.Add("fromtimeItem", strFromTime);
                    res.Add("totimeItem", strToTime);
                    res.Add("tempList", dataList);
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.data = res;

                }
                if (strEditType == "OrderPlantEdit")
                {
                    string strSupplierPlant = dRowinfo.vcSupplierPlant == null ? "" : dRowinfo.vcSupplierPlant.ToString().Trim();
                    string OrderPlant_ed = dRowinfo.OrderPlant_ed == null ? "" : dRowinfo.OrderPlant_ed.ToString();
                    string OrderPlantLinId_ed = dRowinfo.OrderPlantLinId_ed == null ? "" : dRowinfo.OrderPlantLinId_ed.ToString();
                    string OrderPlantFromTime_ed = dRowinfo.OrderPlantFromTime_ed == null ? "" : dRowinfo.OrderPlantFromTime_ed.ToString();
                    string OrderPlantToTime_ed = dRowinfo.OrderPlantToTime_ed == null ? "" : dRowinfo.OrderPlantToTime_ed.ToString();

                    DataTable dataTable = fs0603_Logic.getEditLoadInfo(strEditType, "", "", "", strSupplierId, strSupplierPlant);
                    //if (OrderPlantLinId_ed != "" && dataTable.Select("LinId='" + OrderPlantLinId_ed + "'").Length == 0)//数据库中不是最新有效期履历
                    //{
                    //    DataRow dataRow = dataTable.NewRow();
                    //    dataRow["LinId"] = OrderPlantLinId_ed;
                    //    dataRow["dFromTime"] = OrderPlantFromTime_ed;
                    //    dataRow["dToTime"] = OrderPlantToTime_ed;
                    //    dataRow["vcOrderPlant"] = OrderPlant_ed;
                    //    dataRow["bAddFlag"] = "1";
                    //    dataRow["vcBgColor"] = "";
                    //    dataTable.Rows.InsertAt(dataRow, 0);//插入到第一行
                    //}
                    DtConverter dtConverter = new DtConverter();
                    dtConverter.addField("bAddFlag", ConvertFieldType.BoolType, null);
                    dtConverter.addField("dFromTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                    dtConverter.addField("dToTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                    List<Object> dataList = ComFunction.convertAllToResultByConverter(dataTable, dtConverter);
                    res.Add("packingPlantItem", strPackingPlant);
                    res.Add("packingPlant_nameItem", strPackingPlant_name);
                    res.Add("partIdItem", strPartId);
                    res.Add("receiverItem", strReceiver);
                    res.Add("supplierIdItem", strSupplierId);
                    res.Add("fromtimeItem", strFromTime);
                    res.Add("totimeItem", strToTime);
                    res.Add("tempList", dataList);
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.data = res;

                }
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
        /// 重置子页面（工区SupplierPlantEdit；收容数PackingQtyEdit；受入SufferInEdit；发注工厂OrderPlantEdit）
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string resetpageloadApi([FromBody] dynamic data)
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
            try
            {
                string strEditType = dataForm.type;//维护类型
                string strPackingPlant_fixed = dataForm.mainform.PackingPlant;
                string strPartId_fixed = dataForm.mainform.PartId;
                string strReceiver_fixed = dataForm.mainform.Receiver;
                string strSupplierId_fixed = dataForm.mainform.SupplierId;
                string strFromTime_fixed = dataForm.mainform.FromTime;
                string strToTime_fixed = dataForm.mainform.ToTime;
                DataTable dataTable = fs0603_Logic.getEditLoadInfo(strEditType, strPackingPlant_fixed, strPartId_fixed, strReceiver_fixed, strSupplierId_fixed, "");
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("bAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("dFromTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dToTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dataTable, dtConverter);
                res.Add("tempList", dataList);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "重置失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 维护弹出信息保存（工区SupplierPlantEdit；收容数PackingQtyEdit；受入SufferInEdit；发注工厂OrderPlantEdit）
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string saveeditpageApi([FromBody] dynamic data)
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
            try
            {
                dynamic dSubform = dataForm.subform.list;
                List<Dictionary<string, Object>> listSubInfo = dSubform.ToObject<List<Dictionary<string, Object>>>();

                string strEditType = dataForm.type;//维护类型
                string strPackingPlant_fixed = dataForm.mainform.PackingPlant;
                string strPartId_fixed = dataForm.mainform.PartId;
                string strReceiver_fixed = dataForm.mainform.Receiver;
                string strSupplierId_fixed = dataForm.mainform.SupplierId;
                string strFromTime_fixed = dataForm.mainform.FromTime;
                string strToTime_fixed = dataForm.mainform.ToTime;
                DataTable dtMessage = fs0603_Logic.createTable("MES");

                if (strEditType == "SupplierPlantEdit")
                {
                    res.Add("PackingPlant_fixed", strPackingPlant_fixed);
                    res.Add("PartId_fixed", strPartId_fixed);
                    res.Add("Receiver_fixed", strReceiver_fixed);
                    res.Add("SupplierId_fixed", strSupplierId_fixed);
                    if (listSubInfo.Count == 0)//不存任何有效期履历
                    {
                        res.Add("SupplierPlant", "");
                        res.Add("SupplierPlant_ed", "");
                        res.Add("SupplierPlantLinId_ed", "");
                        res.Add("SupplierPlantFromTime_ed", "");
                        res.Add("SupplierPlantToTime_ed", "");
                        apiResult.code = ComConstant.SUCCESS_CODE;
                        apiResult.data = res;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    else
                    {
                        //只取第一行数据（每次只能追加一条数据有效期）

                        string strLinId = listSubInfo[0]["LinId"].ToString();
                        string strSupplierPlant = listSubInfo[0]["vcSupplierPlant"].ToString();
                        string dFromTime = listSubInfo[0]["dFromTime"] == null || listSubInfo[0]["dFromTime"].ToString() == "" ? Convert.ToDateTime(strFromTime_fixed).ToString("yyyy/MM/dd") : listSubInfo[0]["dFromTime"].ToString();
                        string dToTime = listSubInfo[0]["dToTime"] == null || listSubInfo[0]["dToTime"].ToString() == "" ? Convert.ToDateTime(strToTime_fixed).ToString("yyyy/MM/dd") : listSubInfo[0]["dToTime"].ToString();
                        if (strSupplierPlant == "")
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = "未维护工区信息，无法继续保存。";
                            dtMessage.Rows.Add(dataRow);
                        }
                        if (dFromTime == "" || dToTime == "")
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = "未维护开始使用时间或结束使用时间信息，无法继续保存。";
                            dtMessage.Rows.Add(dataRow);
                        }
                        if (dFromTime != "" && dToTime != "" && Convert.ToDateTime(dFromTime) > Convert.ToDateTime(dToTime))
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = "所维护的使用有效期开始时间大于结束时间，无法继续保存。";
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
                        DataTable dtCheckTime = fs0603_Logic.getEditLoadInfo(strEditType, strPackingPlant_fixed, strPartId_fixed, strReceiver_fixed, strSupplierId_fixed, "");
                        if (Convert.ToInt32(strLinId) == -1)
                        {
                            if (dtCheckTime.Rows.Count > 0)
                            {
                                string strSupplierPlantFromTime_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["dFromTime"].ToString();
                                if (Convert.ToDateTime(strSupplierPlantFromTime_before) >= Convert.ToDateTime(dFromTime))
                                {
                                    //DataRow dataRow = dtMessage.NewRow();
                                    //dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报【工区有效期】维护有误(当前有效的开始使用时间小于维护的开始使用时间)", i + 1);
                                    //dtMessage.Rows.Add(dataRow);
                                }
                                dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["dToTime"] = Convert.ToDateTime(dFromTime).AddDays(-1);
                            }
                            //判断当前生效的数据
                            DataRow dataRow = dtCheckTime.NewRow();
                            dataRow["LinId"] = strLinId;
                            dataRow["dFromTime"] = dFromTime;
                            dataRow["dToTime"] = dToTime;
                            dataRow["vcSupplierPlant"] = strSupplierPlant;
                            dataRow["bAddFlag"] = "1";
                            dataRow["vcBgColor"] = "";
                            dtCheckTime.Rows.InsertAt(dataRow, 0);//插入到第一行
                        }
                        res.Add("SupplierPlant", "");
                        foreach (DataRow item in dtCheckTime.Rows)
                        {
                            if (Convert.ToDateTime(item["dFromTime"].ToString()) <= Convert.ToDateTime(System.DateTime.Now.ToString("yyyy-MM-dd"))
                                && Convert.ToDateTime(item["dToTime"].ToString()) >= Convert.ToDateTime(System.DateTime.Now.ToString("yyyy-MM-dd")))
                            {
                                res.Remove("SupplierPlant");
                                res.Add("SupplierPlant", item["vcSupplierPlant"].ToString());
                                break;
                            }
                        }
                        res.Add("SupplierPlant_ed", strSupplierPlant);
                        res.Add("SupplierPlantLinId_ed", strLinId);
                        res.Add("SupplierPlantFromTime_ed", dFromTime);
                        res.Add("SupplierPlantToTime_ed", dToTime);
                        apiResult.code = ComConstant.SUCCESS_CODE;
                        apiResult.data = res;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                if (strEditType == "PackingQtyEdit")
                {
                    res.Add("PackingPlant_fixed", strPackingPlant_fixed);
                    res.Add("PartId_fixed", strPartId_fixed);
                    res.Add("Receiver_fixed", strReceiver_fixed);
                    res.Add("SupplierId_fixed", strSupplierId_fixed);
                    if (listSubInfo.Count == 0)//不存任何有效期履历
                    {
                        res.Add("iPackingQty", "");
                        res.Add("vcBoxType", "");
                        res.Add("iLength", "");
                        res.Add("iWidth", "");
                        res.Add("iHeight", "");
                        res.Add("iVolume", "");
                        res.Add("BoxPackingQty_ed", "");
                        res.Add("BoxLinId_ed", "");
                        res.Add("BoxFromTime_ed", "");
                        res.Add("BoxToTime_ed", "");
                        res.Add("BoxType_ed", "");
                        res.Add("BoxLength_ed", "");
                        res.Add("BoxWidth_ed", "");
                        res.Add("BoxHeight_ed", "");
                        res.Add("BoxVolume_ed", "");
                        apiResult.code = ComConstant.SUCCESS_CODE;
                        apiResult.data = res;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    else
                    {
                        //只取第一行数据（每次只能追加一条数据有效期）
                        string strLinId = listSubInfo[0]["LinId"].ToString();
                        string strPackingQty = listSubInfo[0]["iPackingQty"].ToString();
                        string dFromTime = listSubInfo[0]["dFromTime"] == null || listSubInfo[0]["dFromTime"].ToString() == "" ? Convert.ToDateTime(strFromTime_fixed).ToString("yyyy/MM/dd") : listSubInfo[0]["dFromTime"].ToString();
                        string dToTime = listSubInfo[0]["dToTime"] == null || listSubInfo[0]["dToTime"].ToString() == "" ? Convert.ToDateTime(strToTime_fixed).ToString("yyyy/MM/dd") : listSubInfo[0]["dToTime"].ToString();
                        string strBoxType = listSubInfo[0]["vcBoxType"].ToString();
                        string strLength = listSubInfo[0]["iLength"].ToString();
                        string strWidth = listSubInfo[0]["iWidth"].ToString();
                        string strHeight = listSubInfo[0]["iHeight"].ToString();
                        if (strPackingQty == "" || strBoxType == "" || strLength == "" || strWidth == "" || strHeight == "")
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = "未维护收容数或者箱种信息，无法继续保存。";
                            dtMessage.Rows.Add(dataRow);
                        }
                        if (strBoxType.Length < 2)
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = "请填写完整箱种信息(至少两位)，无法继续保存。";
                            dtMessage.Rows.Add(dataRow);
                        }
                        if (dFromTime == "" || dToTime == "")
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = "未维护开始使用时间或结束使用时间信息，无法继续保存。";
                            dtMessage.Rows.Add(dataRow);
                        }
                        if (dFromTime != "" && dToTime != "" && Convert.ToDateTime(dFromTime) > Convert.ToDateTime(dToTime))
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = "所维护的使用有效期开始时间大于结束时间，无法继续保存。";
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
                        string strVolume = ((Convert.ToInt32(listSubInfo[0]["iLength"].ToString()) * Convert.ToInt32(listSubInfo[0]["iWidth"].ToString()) * Convert.ToInt32(listSubInfo[0]["iHeight"].ToString())) / 1000000000.0000).ToString("#.0000");
                        DataTable dtCheckTime = fs0603_Logic.getEditLoadInfo(strEditType, strPackingPlant_fixed, strPartId_fixed, strReceiver_fixed, strSupplierId_fixed, "");
                        if (Convert.ToInt32(strLinId) == -1)
                        {
                            if (dtCheckTime.Rows.Count > 0)
                            {
                                string strPackingQtyFromTime_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["dFromTime"].ToString();
                                if (Convert.ToDateTime(strPackingQtyFromTime_before) >= Convert.ToDateTime(dFromTime))
                                {
                                    //DataRow dataRow = dtMessage.NewRow();
                                    //dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报【工区有效期】维护有误(当前有效的开始使用时间小于维护的开始使用时间)", i + 1);
                                    //dtMessage.Rows.Add(dataRow);
                                }
                                dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["dToTime"] = Convert.ToDateTime(dFromTime).AddDays(-1);
                            }
                            //判断当前生效的数据
                            DataRow dataRow = dtCheckTime.NewRow();
                            dataRow["LinId"] = strLinId;
                            dataRow["iPackingQty"] = strPackingQty;
                            dataRow["vcBoxType"] = strBoxType;
                            dataRow["iLength"] = strLength;
                            dataRow["iWidth"] = strWidth;
                            dataRow["iHeight"] = strHeight;
                            dataRow["iVolume"] = strVolume;
                            dataRow["dFromTime"] = dFromTime;
                            dataRow["dToTime"] = dToTime;
                            dataRow["bAddFlag"] = "1";
                            dataRow["vcBgColor"] = "";
                            dtCheckTime.Rows.InsertAt(dataRow, 0);//插入到第一行
                        }
                        res.Add("iPackingQty", "");
                        res.Add("vcBoxType", "");
                        res.Add("iLength", "");
                        res.Add("iWidth", "");
                        res.Add("iHeight", "");
                        res.Add("iVolume", "");
                        foreach (DataRow item in dtCheckTime.Rows)
                        {
                            if (Convert.ToDateTime(item["dFromTime"].ToString()) <= Convert.ToDateTime(System.DateTime.Now.ToString("yyyy-MM-dd"))
                                && Convert.ToDateTime(item["dToTime"].ToString()) >= Convert.ToDateTime(System.DateTime.Now.ToString("yyyy-MM-dd")))
                            {
                                //当前有效的
                                res.Remove("iPackingQty");
                                res.Add("iPackingQty", item["iPackingQty"].ToString());
                                res.Remove("vcBoxType");
                                res.Add("vcBoxType", item["vcBoxType"].ToString());
                                res.Remove("iLength");
                                res.Add("iLength", item["iLength"].ToString());
                                res.Remove("iWidth");
                                res.Add("iWidth", item["iWidth"].ToString());
                                res.Remove("iHeight");
                                res.Add("iHeight", item["iHeight"].ToString());
                                res.Remove("iVolume");
                                res.Add("iVolume", item["iVolume"].ToString());
                                break;
                            }
                        }
                        res.Add("BoxPackingQty_ed", strPackingQty);
                        res.Add("BoxLinId_ed", strLinId);
                        res.Add("BoxFromTime_ed", dFromTime);
                        res.Add("BoxToTime_ed", dToTime);
                        res.Add("BoxType_ed", strBoxType);
                        res.Add("BoxLength_ed", strLength);
                        res.Add("BoxWidth_ed", strWidth);
                        res.Add("BoxHeight_ed", strHeight);
                        res.Add("BoxVolume_ed", strVolume);
                        apiResult.code = ComConstant.SUCCESS_CODE;
                        apiResult.data = res;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                if (strEditType == "SufferInEdit")
                {
                    res.Add("PackingPlant_fixed", strPackingPlant_fixed);
                    res.Add("PartId_fixed", strPartId_fixed);
                    res.Add("Receiver_fixed", strReceiver_fixed);
                    res.Add("SupplierId_fixed", strSupplierId_fixed);
                    if (listSubInfo.Count == 0)//不存任何有效期履历
                    {
                        res.Add("SufferIn", "");
                        res.Add("SufferIn_ed", "");
                        res.Add("SufferInLinId_ed", "");
                        res.Add("SufferInFromTime_ed", "");
                        res.Add("SufferInToTime_ed", "");
                        apiResult.code = ComConstant.SUCCESS_CODE;
                        apiResult.data = res;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    else
                    {
                        //只取第一行数据（每次只能追加一条数据有效期）
                        string strLinId = listSubInfo[0]["LinId"].ToString();
                        string strSufferIn = listSubInfo[0]["vcSufferIn"].ToString();
                        string dFromTime = listSubInfo[0]["dFromTime"] == null || listSubInfo[0]["dFromTime"].ToString() == "" ? Convert.ToDateTime(strFromTime_fixed).ToString("yyyy/MM/dd") : listSubInfo[0]["dFromTime"].ToString();
                        string dToTime = listSubInfo[0]["dToTime"] == null || listSubInfo[0]["dToTime"].ToString() == "" ? Convert.ToDateTime(strToTime_fixed).ToString("yyyy/MM/dd") : listSubInfo[0]["dToTime"].ToString();
                        if (strSufferIn == "")
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = "未维护受入信息，无法继续保存。";
                            dtMessage.Rows.Add(dataRow);
                        }
                        if (dFromTime == "" || dToTime == "")
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = "未维护开始使用时间或结束使用时间信息，无法继续保存。";
                            dtMessage.Rows.Add(dataRow);
                        }
                        if (dFromTime != "" && dToTime != "" && Convert.ToDateTime(dFromTime) > Convert.ToDateTime(dToTime))
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = "所维护的使用有效期开始时间大于结束时间，无法继续保存。";
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
                        DataTable dtCheckTime = fs0603_Logic.getEditLoadInfo(strEditType, strPackingPlant_fixed, strPartId_fixed, strReceiver_fixed, strSupplierId_fixed, "");
                        if (Convert.ToInt32(strLinId) == -1)
                        {
                            if (dtCheckTime.Rows.Count > 0)
                            {
                                string strSufferInFromTime_before = dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["dFromTime"].ToString();
                                if (Convert.ToDateTime(strSufferInFromTime_before) >= Convert.ToDateTime(dFromTime))
                                {
                                    //DataRow dataRow = dtMessage.NewRow();
                                    //dataRow["vcMessage"] = string.Format("第{0}行【" + strType + "】情报【工区有效期】维护有误(当前有效的开始使用时间小于维护的开始使用时间)", i + 1);
                                    //dtMessage.Rows.Add(dataRow);
                                }
                                dtCheckTime.Rows[dtCheckTime.Rows.Count - 1]["dToTime"] = Convert.ToDateTime(dFromTime).AddDays(-1);
                            }
                            //判断当前生效的数据
                            DataRow dataRow = dtCheckTime.NewRow();
                            dataRow["LinId"] = strLinId;
                            dataRow["dFromTime"] = dFromTime;
                            dataRow["dToTime"] = dToTime;
                            dataRow["vcSufferIn"] = strSufferIn;
                            dataRow["bAddFlag"] = "1";
                            dataRow["vcBgColor"] = "";
                            dtCheckTime.Rows.InsertAt(dataRow, 0);//插入到第一行
                        }
                        res.Add("SufferIn", "");
                        foreach (DataRow item in dtCheckTime.Rows)
                        {
                            if (Convert.ToDateTime(item["dFromTime"].ToString()) <= Convert.ToDateTime(System.DateTime.Now.ToString("yyyy-MM-dd"))
                                && Convert.ToDateTime(item["dToTime"].ToString()) >= Convert.ToDateTime(System.DateTime.Now.ToString("yyyy-MM-dd")))
                            {
                                //当前有效的
                                res.Remove("SufferIn");
                                res.Add("SufferIn", item["vcSufferIn"].ToString());
                                break;
                            }
                        }
                        res.Add("SufferIn_ed", strSufferIn);
                        res.Add("SufferInLinId_ed", strLinId);
                        res.Add("SufferInFromTime_ed", dFromTime);
                        res.Add("SufferInToTime_ed", dToTime);
                        apiResult.code = ComConstant.SUCCESS_CODE;
                        apiResult.data = res;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                if (strEditType == "OrderPlantEdit")
                {

                }
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
        /// 履历下载
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string exportoperhistoryApi([FromBody]dynamic data)
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

            string strFromTime = dataForm.searchform.FromTime == null ? "" : dataForm.searchform.FromTime;
            string strToTime = dataForm.searchform.ToTime == null ? "" : dataForm.searchform.ToTime;
            try
            {
                DataTable dtMessage = fs0603_Logic.createTable("MES");
                if (strFromTime == "" || strToTime == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "请输入下载的起止时间";
                    dtMessage.Rows.Add(dataRow);
                }
                DataTable dataTable = fs0603_Logic.searchOperHistory(strFromTime, strToTime, loginInfo.UserId);
                if (dataTable.Rows.Count == 0)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "没有可供下载的履历信息";
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
                string[] fields = { "vcPackingPlant", "vcPartId", "vcReceiver", "vcSupplierId", "vcAction", "dOperatorTime" };
                string filepath = ComFunction.generateExcelWithXlt(dataTable, fields, _webHostEnvironment.ContentRootPath, "FS0603_OperHistory.xlsx", 1, loginInfo.UserId, FunctionID);
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
        /// 下载数据模板
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string downloadTplApi([FromBody] dynamic data)
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

            string strSyncTime = dataForm.SyncTime;
            string strPartId = dataForm.PartId;
            string strCarModel = dataForm.CarModel;
            string strReceiver = dataForm.Receiver;
            string strInOut = dataForm.InOut;
            string strHaoJiu = dataForm.HaoJiu;
            string strSupplierId = dataForm.SupplierId;
            string strSupplierPlant = dataForm.SupplierPlant;
            string strOrderPlant = dataForm.OrderPlant;
            string strFromTime = dataForm.FromTime;
            string strToTime = dataForm.ToTime;
            string strBoxType = dataForm.BoxType;
            string strSufferIn = dataForm.SufferIn;
            string strSupplierPacking = dataForm.SupplierPacking;
            string strOldProduction = dataForm.OldProduction;
            string strDebugTime = dataForm.DebugTime;
            try
            {
                DataTable dataTable = fs0603_Logic.getSearchInfo(strSyncTime, strPartId, strCarModel, strReceiver, strInOut, strHaoJiu, strSupplierId, strSupplierPlant,
                    strOrderPlant, strFromTime, strToTime, strBoxType, strSufferIn, strSupplierPacking, strOldProduction, strDebugTime, "", false);
                dataTable.Columns.Add("vcType");
                string[] fields = {"vcType","dSyncTime","vcChanges_name","vcPackingPlant","vcPartId","vcPartENName","vcCarfamilyCode",
                    "vcReceiver","dFromTime","dToTime","vcPartId_Replace","vcInOut_name","vcOESP_name","vcHaoJiu_name",
                    "vcOldProduction_name","dDebugTime","vcSupplierId","dSupplierFromTime","dSupplierToTime","vcSupplierName",
                    "SupplierPlant_ed","SupplierPlantFromTime_ed","SupplierPlantToTime_ed",
                    "vcSupplierPlace",
                    "BoxPackingQty_ed","BoxFromTime_ed","BoxToTime_ed","BoxType_ed","BoxLength_ed","BoxWidth_ed","BoxHeight_ed","BoxVolume_ed",
                    "SufferIn_ed","SufferInFromTime_ed","SufferInToTime_ed",
                    "vcOrderPlant_name",
                    "vcInteriorProject","vcPassProject","vcFrontProject","dFrontProjectTime","dShipmentTime",
                    "vcPartImage","vcBillType_name","vcRemark1","vcRemark2",
                    "vcOrderingMethod_name","vcMandOrder_name","vcSupplierPacking_name"
                };

                string filepath = ComFunction.generateExcelWithXlt(dataTable, fields, _webHostEnvironment.ContentRootPath, "FS0603_Template.xlsx", 1, loginInfo.UserId, FunctionID);
                if (filepath == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "导出模板文件失败";
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
        /// 批量维护
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string importSaveApi([FromBody]dynamic data)
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
            JArray fileNameList = dataForm.fileNameList;
            string hashCode = dataForm.hashCode;
            string fileSavePath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "upload" + Path.DirectorySeparatorChar + hashCode + Path.DirectorySeparatorChar;
            try
            {
                if (!Directory.Exists(fileSavePath))
                {
                    ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "没有要导入的文件，请重新上传！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                DirectoryInfo theFolder = new DirectoryInfo(fileSavePath);
                string strMsg = "";
                #region  string[,] headers = new string[,] {}
                string[,] headers = new string[,] {
                    {"操作类型","同步时间","变更事项","包装工厂","补给品番","品名(英文)","车种代码",
                        "收货方","品番-使用开始","品番-使用结束","替代品番",
                        "内外区分","OE=SP","号旧区分","旧型年限生产区分","实施年月(年限)",
                        "供应商编号","供应商使用开始","供应商使用结束","供应商名称",
                        "工区","工区-使用开始","工区-使用结束","出荷场",
                        "收容数","收容数-使用开始","收容数-使用结束","箱种","长(mm)","宽(mm)","高(mm)","体积(m³)",
                        "受入","受入-使用开始","受入-使用结束",
                        "发注工场",
                        "内制工程","通过工程","前工程","前工程通过时间","自工程出货时间","照片",
                        "单据区分","备注1","备注2","订货方式","强制订货","供应商包装"},
                    {"vcType","dSyncTime","vcChanges_name","vcPackingPlant","vcPartId","vcPartENName","vcCarfamilyCode",
                        "vcReceiver","dFromTime","dToTime","vcPartId_Replace",
                        "vcInOut_name","vcOESP_name","vcHaoJiu_name","vcOldProduction_name","dDebugTime",
                        "vcSupplierId","dSupplierFromTime","dSupplierToTime","vcSupplierName",
                        "SupplierPlant_ed","SupplierPlantFromTime_ed","SupplierPlantToTime_ed","vcSupplierPlace",
                        "BoxPackingQty_ed","BoxFromTime_ed","BoxToTime_ed","BoxType_ed","BoxLength_ed","BoxWidth_ed","BoxHeight_ed","BoxVolume_ed",
                        "SufferIn_ed","SufferInFromTime_ed","SufferInToTime_ed",
                        "vcOrderPlant_name",
                        "vcInteriorProject","vcPassProject","vcFrontProject","dFrontProjectTime","dShipmentTime","vcPartImage",
                        "vcBillType_name","vcRemark1","vcRemark2","vcOrderingMethod_name","vcMandOrder_name","vcSupplierPacking_name"},
                    {"","","","","","","",
                        "","","","",
                        "","","","","",
                        "","","","",
                        FieldCheck.NumCharLLL,FieldCheck.Date,FieldCheck.Date,"",
                        FieldCheck.Num,FieldCheck.Date,FieldCheck.Date,FieldCheck.NumCharLLL,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,"",
                        FieldCheck.NumCharLLL,FieldCheck.Date,FieldCheck.Date,
                        "",
                        "","","",FieldCheck.Date,FieldCheck.Date,"",
                        "","","","","",""},
                    {"0","0","0","0","0","0","0",
                        "0","0","0","0",
                        "0","0","0","0","0",
                        "0","0","0","0",
                        "1","0","0","0",
                        "0","0","0","0","0","0","0","0",
                        "0","0","0",
                        "0",
                        "0","0","0","0","0","0",
                        "0","0","0","0","0","0"},//最大长度设定,不校验最大长度用0
                    {"0","0","0","0","0","0","0",
                        "0","0","0","0",
                        "0","0","0","0","0",
                        "0","0","0","0",
                        "1","0","0","0",
                        "0","0","0","0","0","0","0","0",
                        "0","0","0",
                        "0",
                        "0","0","0","0","0","0",
                        "0","0","0","0","0","0"}};//最小长度设定,可以为空用0
                #endregion
                DataTable importDt = new DataTable();
                DataTable dtMessage = fs0603_Logic.createTable("MES");
                bool bReault = true;
                foreach (FileInfo info in theFolder.GetFiles())
                {
                    DataTable dt = ComFunction.ExcelToDataTableformRows(info.FullName, "sheet1", headers, 2, 2, ref strMsg);
                    if (strMsg != "")
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = "读取导入文件" + info.Name + "出错！";
                        dtMessage.Rows.Add(dataRow);
                        //弹出错误dtMessage
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.type = "list";
                        apiResult.data = dtMessage;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    DataTable dtImport = fs0603_Logic.checkFileInfo(dt, headers, 2, 3, ref bReault, ref dtMessage);
                    if (!bReault)
                    {
                        //弹出错误dtMessage
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.type = "list";
                        apiResult.data = dtMessage;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    fs0603_Logic.setSPInfo(dtImport, loginInfo.UserId, ref dtMessage);
                    if (dtMessage.Rows.Count != 0)
                    {
                        //弹出错误dtMessage
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.type = "list";
                        apiResult.data = dtMessage;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "导入成功";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0905", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.type = "info";
                apiResult.data = "导入失败" + ex.Message;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            //finally
            //{
            //    ComFunction.DeleteFolder(fileSavePath);//读取数据后删除文件夹
            //}
        }
        /// <summary>
        /// 导出消息信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string exportmessageApi([FromBody]dynamic data)
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
                List<Dictionary<string, Object>> listInfoData = dataForm.ToObject<List<Dictionary<string, Object>>>();
                DataTable dataTable = fs0603_Logic.createTable("MES");

                for (int i = 0; i < listInfoData.Count; i++)
                {
                    DataRow dataRow = dataTable.NewRow();
                    dataRow["vcMessage"] = listInfoData[i]["vcMessage"].ToString();
                    dataTable.Rows.Add(dataRow);
                }
                string[] fields = { "vcMessage" };

                string filepath = ComFunction.generateExcelWithXlt(dataTable, fields, _webHostEnvironment.ContentRootPath, "MessageList.xlsx", 1, loginInfo.UserId, FunctionID);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0902", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
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
        public string exportsyncApi([FromBody] dynamic data)
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

            try
            {
                DataTable dtExport = fs0603_Logic.getSyncTable();
                string[] fields = {"dSyncTime","vcSyncMessage"
                };

                string filepath = ComFunction.generateExcelWithXlt(dtExport, fields, _webHostEnvironment.ContentRootPath, "Sync_Info.xlsx", 1, loginInfo.UserId, FunctionID);
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
        /// 我了解了方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string readsyncApi([FromBody]dynamic data)
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
                JArray listInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
                if (listInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "info";
                    apiResult.data = "最少选择一条数据！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                DataTable dtMessage = fs0603_Logic.createTable("MES");
                DataTable dtImport = fs0603_Logic.createTable("SyncData");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    DataRow drImport = dtImport.NewRow();
                    drImport["dSyncTime"] = listInfoData[i]["dSyncTime"].ToString();
                    drImport["vcChanges"] = listInfoData[i]["vcChanges"].ToString();
                    dtImport.Rows.Add(drImport);
                }
                fs0603_Logic.setSyncInfo(dtImport, loginInfo.UserId, ref dtMessage);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0902", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.type = "info";
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
    }
}
