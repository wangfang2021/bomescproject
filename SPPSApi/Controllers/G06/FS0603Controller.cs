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

                List<Object> ReceiverList = ComFunction.convertAllToResult(fs0603_Logic.getCodeInfo("Receiver"));//收货方
                List<Object> InOutList = ComFunction.convertAllToResult(ComFunction.getTCode("C003"));//内外区分
                List<Object> HaoJiuList = ComFunction.convertAllToResult(ComFunction.getTCode("C004"));//号旧区分
                List<Object> OrderPlantList = ComFunction.convertAllToResult(ComFunction.getTCode("C000"));//发注工厂
                List<Object> ChangesList = ComFunction.convertAllToResult(ComFunction.getTCode("C002"));//变更事项
                List<Object> PackingPlantList = ComFunction.convertAllToResult(ComFunction.getTCode("C017"));//包装工厂
                List<Object> OESPList = ComFunction.convertAllToResult(ComFunction.getTCode("C012"));//OE=SP
                List<Object> OldProductionList = ComFunction.convertAllToResult(ComFunction.getTCode("C024"));//旧型年限生产区分
                List<Object> BillTypeList = ComFunction.convertAllToResult(ComFunction.getTCode("C007"));//单据区分
                List<Object> OrderingMethodList = ComFunction.convertAllToResult(ComFunction.getTCode("C047"));//订货方式
                List<Object> MandOrderList = ComFunction.convertAllToResult(ComFunction.getTCode("C048"));//强制订货

                res.Add("ReceiverList", ReceiverList);
                res.Add("InOutList", InOutList);
                res.Add("HaoJiuList", HaoJiuList);
                res.Add("OrderPlantList", OrderPlantList);
                res.Add("ChangesList", ChangesList);
                res.Add("PackingPlantList", PackingPlantList);
                res.Add("OESPList", OESPList);
                res.Add("OldProductionList", OldProductionList);
                res.Add("BillTypeList", BillTypeList);
                res.Add("OrderingMethodList", OrderingMethodList);
                res.Add("MandOrderList", MandOrderList);

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

            string strSyncTime = dataForm.SyncTime;
            string strPartId = dataForm.PartId;
            string strCarModel = dataForm.CarModel;
            string strReceiver = dataForm.Receiver;
            string strInOut = dataForm.InOut;
            string strSupplierId = dataForm.SupplierId;
            string strSupplierPlant = dataForm.SupplierPlant;
            string strFromTime = dataForm.FromTime;
            string strToTime = dataForm.ToTime;
            string strHaoJiu = dataForm.HaoJiu;
            string strOrderPlant = dataForm.OrderPlant;

            try
            {
                DataTable dataTable = fs0603_Logic.getSearchInfo(strSyncTime, strPartId, strCarModel, strReceiver, strInOut,
                    strSupplierId, strSupplierPlant, strFromTime, strToTime, strHaoJiu, strOrderPlant);
                DtConverter dtConverter = new DtConverter();


                dtConverter.addField("bModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("bAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("dFromTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dToTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dOldStartTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dDebugTime", ConvertFieldType.DateType, "yyyy/MM");
                dtConverter.addField("dSupplierFromTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dSupplierToTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dFrontProjectTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dShipmentTime", ConvertFieldType.DateType, "yyyy/MM/dd");
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

            string strSyncTime = dataForm.SyncTime;
            string strPartId = dataForm.PartId;
            string strCarModel = dataForm.CarModel;
            string strReceiver = dataForm.Receiver;
            string strInOut = dataForm.InOut;
            string strSupplierId = dataForm.SupplierId;
            string strSupplierPlant = dataForm.SupplierPlant;
            string strFromTime = dataForm.FromTime;
            string strToTime = dataForm.ToTime;
            string strHaoJiu = dataForm.HaoJiu;
            string strOrderPlant = dataForm.OrderPlant;
            try
            {
                DataTable dataTable = fs0603_Logic.getSearchInfo(strSyncTime, strPartId, strCarModel, strReceiver, strInOut,
                   strSupplierId, strSupplierPlant, strFromTime, strToTime, strHaoJiu, strOrderPlant);

                string[] fields = {"dSyncTime","vcChanges_name","vcPackingPlant_name","vcPartId","vcPartENName","vcCarfamilyCode","vcReceiver_name","dFromTime","dToTime",
                    "vcPartId_Replace","vcInOut_name","vcOESP_name","vcHaoJiu_name","vcOldProduction_name","dOldStartTime","dDebugTime","vcSupplierId",
                    "dSupplierFromTime","dSupplierToTime","vcSupplierName","vcSupplierPlant","vcSupplierPlace",
                    "iPackingQty","vcBoxType","iLength","iWidth","iHeight","iVolume","vcSufferIn","vcOrderPlant_name",
                    "vcInteriorProject","vcPassProject","vcFrontProject","dFrontProjectTime","dShipmentTime","vcPartImage","vcBillType_name",
                    "vcRemark1","vcRemark2","vcOrderingMethod_name","vcMandOrder_name"
                };

                string filepath = ComFunction.generateExcelWithXlt(dataTable, fields, _webHostEnvironment.ContentRootPath, "FS0603_Export.xlsx", 1, loginInfo.UserId, FunctionID);
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
                if (hasFind)
                {
                    #region 数据校验
                    //string[,] strField = new string[,] {{"同步时间","变更事项","包装工厂","补给品番","品名","车种代码","收货方","使用开始","使用结束","替代品番","内外区分","OE=SP","号旧区分","旧型年限生产区分","旧型开始时间","实施年月","供应商编号","供应商使用开始","供应商使用结束","供应商名称"},
                    //                            {"dSyncTime","vcChanges","vcPackingPlant","vcPartId","vcPartENName","vcCarfamilyCode","vcReceiver","dFromTime","dToTime","vcPartId_Replace","vcInOut","vcOESP","vcHaoJiu","vcOldProduction","dOldStartTime","dDebugTime","vcSupplierId","dSupplierFromTime","dSupplierToTime","vcSupplierName"},
                    //                            {FieldCheck.Date,FieldCheck.NumChar,FieldCheck.NumChar,FieldCheck.NumChar,FieldCheck.NumChar,FieldCheck.NumChar,FieldCheck.NumChar,FieldCheck.Date,FieldCheck.Date,FieldCheck.NumChar,FieldCheck.NumChar,FieldCheck.NumChar,FieldCheck.NumChar,FieldCheck.NumChar,FieldCheck.Date,FieldCheck.YearMonth,FieldCheck.NumChar,FieldCheck.Date,FieldCheck.Date,FieldCheck.NumChar},
                    //                            {"0","0","0","12","200","0","0","0","0","0","0","0","0","0","0","0","10","0","0","0"},//最大长度设定,不校验最大长度用0
                    //                            {"0","1","1","12","1","0","1","1","1","0","1","1","1","0","0","0","1","1","1","1"},//最小长度设定,可以为空用0
                    //                            {"1","2","3","4","5","6","7","8","9","10","11","12","13","14","15","16","17","18","19","20"}//前台显示列号，从0开始计算,注意有选择框的是0
                    //};
                    ////需要判断时间区间先后关系的字段
                    //string[,] strDateRegion = { { "dFromTime", "dToTime" }, { "dSupplierFromTime", "dSupplierToTime" } };
                    //string[,] strSpecialCheck = { };
                    //List<Object> checkRes = ListChecker.validateList(listInfoData, strField, strDateRegion, strSpecialCheck, true, "FS0603");
                    //if (checkRes != null)
                    //{
                    //    apiResult.code = ComConstant.ERROR_CODE;
                    //    apiResult.data = checkRes;
                    //    apiResult.flag = Convert.ToInt32(ERROR_FLAG.单元格定位提示);
                    //    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    //}
                    #endregion
                }
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

                string strLinId = dRowinfo.LinId == null ? "" : dRowinfo.LinId.ToString();//维护数据LinId
                string strPackingPlant = dRowinfo.vcPackingPlant == null ? "" : dRowinfo.vcPackingPlant.ToString();//包装工厂value
                string strPackingPlant_name = dRowinfo.vcPackingPlant_name == null ? "" : dRowinfo.vcPackingPlant_name.ToString();//包装工厂name
                string strPartId = dRowinfo.vcPartId == null ? "" : dRowinfo.vcPartId.ToString().ToUpper();//品番
                string strReceiver = dRowinfo.vcReceiver == null ? "" : dRowinfo.vcReceiver.ToString();//收货方value
                string strReceiver_name = dRowinfo.vcReceiver_name == null ? "" : dRowinfo.vcReceiver_name.ToString();// 收货方name
                string strSupplierId = dRowinfo.vcSupplierId == null ? "" : dRowinfo.vcSupplierId.ToString();// 供应商
                if (strEditType == "SupplierPlantEdit")
                {
                    string strSupplierPlant = dRowinfo.vcSupplierPlant == "待维护" ? "" : dRowinfo.vcSupplierPlant.ToString();// 供应商

                    string SupplierPlant_ed = dRowinfo.SupplierPlant_ed == null ? "" : dRowinfo.SupplierPlant_ed.ToString();
                    string SupplierPlantLinId_ed = dRowinfo.SupplierPlantLinId_ed == null ? "" : dRowinfo.SupplierPlantLinId_ed.ToString();
                    string SupplierPlantFromTime_ed = dRowinfo.SupplierPlantFromTime_ed == null ? "" : dRowinfo.SupplierPlantFromTime_ed.ToString();
                    string SupplierPlantToTime_ed = dRowinfo.SupplierPlantToTime_ed == null ? "" : dRowinfo.SupplierPlantToTime_ed.ToString();

                    DataTable dataTable = fs0603_Logic.getEditLoadInfo(strEditType, strPackingPlant, strPartId, strReceiver, strSupplierId, strSupplierPlant);
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
                    res.Add("partIdItem", strPartId);
                    res.Add("receiverItem", strReceiver);
                    res.Add("supplierIdItem", strSupplierId);
                    res.Add("supplierPlantItem", strSupplierPlant);
                    res.Add("tempList", dataList);
                    //这些作为子页面固定值
                    res.Add("packingPlant_nameItem", strPackingPlant_name);
                    res.Add("receiver_nameItem", strReceiver_name);
                    res.Add("linid_mainItem", strLinId);
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.data = res;

                }
                if (strEditType == "PackingQtyEdit")
                {
                    string strSupplierPlant = dRowinfo.vcSupplierPlant == "待维护" ? "" : dRowinfo.vcSupplierPlant.ToString();// 供应商
                    if (strSupplierPlant == "")
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "未维护工区信息，请先维护工区。";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    //string SupplierPlant_ed = dRowinfo.SupplierPlant_ed == null ? "" : dRowinfo.SupplierPlant_ed.ToString();
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

                    DataTable dataTable = fs0603_Logic.getEditLoadInfo(strEditType, strPackingPlant, strPartId, strReceiver, strSupplierId, strSupplierPlant);
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
                    res.Add("partIdItem", strPartId);
                    res.Add("receiverItem", strReceiver);
                    res.Add("supplierIdItem", strSupplierId);
                    res.Add("supplierPlantItem", strSupplierPlant);
                    res.Add("tempList", dataList);
                    //这些作为子页面固定值
                    res.Add("packingPlant_nameItem", strPackingPlant_name);
                    res.Add("receiver_nameItem", strReceiver_name);
                    res.Add("linid_mainItem", strLinId);
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.data = res;

                }
                if (strEditType == "SufferInEdit")
                {
                    string strSupplierPlant = dRowinfo.vcSupplierPlant == "待维护" ? "" : dRowinfo.vcSupplierPlant.ToString();// 供应商

                    string SufferIn_ed = dRowinfo.SufferIn_ed == null ? "" : dRowinfo.SufferIn_ed.ToString();
                    string SufferInLinId_ed = dRowinfo.SufferInLinId_ed == null ? "" : dRowinfo.SufferInLinId_ed.ToString();
                    string SufferInFromTime_ed = dRowinfo.SufferInFromTime_ed == null ? "" : dRowinfo.SufferInFromTime_ed.ToString();
                    string SufferInToTime_ed = dRowinfo.SufferInToTime_ed == null ? "" : dRowinfo.SufferInToTime_ed.ToString();

                    DataTable dataTable = fs0603_Logic.getEditLoadInfo(strEditType, strPackingPlant, strPartId, strReceiver, strSupplierId, strSupplierPlant);
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
                    res.Add("partIdItem", strPartId);
                    res.Add("receiverItem", strReceiver);
                    res.Add("supplierIdItem", strSupplierId);
                    res.Add("supplierPlantItem", strSupplierPlant);
                    res.Add("tempList", dataList);
                    //这些作为子页面固定值
                    res.Add("packingPlant_nameItem", strPackingPlant_name);
                    res.Add("receiver_nameItem", strReceiver_name);
                    res.Add("linid_mainItem", strLinId);
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.data = res;

                }
                if (strEditType == "OrderPlantEdit")
                {
                    string strSupplierPlant = dRowinfo.vcSupplierPlant == "待维护" ? "" : dRowinfo.vcSupplierPlant.ToString();// 供应商

                    string OrderPlant_ed = dRowinfo.OrderPlant_ed == null ? "" : dRowinfo.OrderPlant_ed.ToString();
                    string OrderPlantLinId_ed = dRowinfo.OrderPlantLinId_ed == null ? "" : dRowinfo.OrderPlantLinId_ed.ToString();
                    string OrderPlantFromTime_ed = dRowinfo.OrderPlantFromTime_ed == null ? "" : dRowinfo.OrderPlantFromTime_ed.ToString();
                    string OrderPlantToTime_ed = dRowinfo.OrderPlantToTime_ed == null ? "" : dRowinfo.OrderPlantToTime_ed.ToString();

                    DataTable dataTable = fs0603_Logic.getEditLoadInfo(strEditType, strPackingPlant, strPartId, strReceiver, strSupplierId, strSupplierPlant);
                    if (OrderPlantLinId_ed != "" && dataTable.Select("LinId='" + OrderPlantLinId_ed + "'").Length == 0)//数据库中不是最新有效期履历
                    {
                        DataRow dataRow = dataTable.NewRow();
                        dataRow["LinId"] = OrderPlantLinId_ed;
                        dataRow["dFromTime"] = OrderPlantFromTime_ed;
                        dataRow["dToTime"] = OrderPlantToTime_ed;
                        dataRow["vcOrderPlant"] = OrderPlant_ed;
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
                    res.Add("partIdItem", strPartId);
                    res.Add("receiverItem", strReceiver);
                    res.Add("supplierIdItem", strSupplierId);
                    res.Add("supplierPlantItem", strSupplierPlant);
                    res.Add("tempList", dataList);
                    //这些作为子页面固定值
                    res.Add("packingPlant_nameItem", strPackingPlant_name);
                    res.Add("receiver_nameItem", strReceiver_name);
                    res.Add("linid_mainItem", strLinId);
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
                dynamic dRowinfo = dataForm.subform;//维护数据信息

                string strLinId = dRowinfo.LinId_main == null ? "" : dRowinfo.LinId_main.ToString();//维护数据LinId
                string strPackingPlant = dRowinfo.PackingPlant == null ? "" : dRowinfo.PackingPlant.ToString();//维护数据LinId
                string strPartId = dRowinfo.PartId == null ? "" : dRowinfo.PartId.ToString().ToUpper();//维护数据LinId
                string strReceiver = dRowinfo.Receiver == null ? "" : dRowinfo.Receiver.ToString();//维护数据LinId
                string strSupplierId = dRowinfo.SupplierId == null ? "" : dRowinfo.SupplierId.ToString();//维护数据LinId
                string strSupplierPlant = dRowinfo.SupplierPlant == "待维护" ? "" : dRowinfo.SupplierPlant.ToString();//维护数据LinId
                #region 
                //if (strEditType == "SupplierPlantEdit")
                //{
                //    DataTable dataTable = fs0603_Logic.getEditLoadInfo(strEditType, strPackingPlant, strPartId, strReceiver, strSupplierId, strSupplierPlant);
                //    DtConverter dtConverter = new DtConverter();
                //    dtConverter.addField("bAddFlag", ConvertFieldType.BoolType, null);
                //    dtConverter.addField("dFromTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                //    dtConverter.addField("dToTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                //    List<Object> dataList = ComFunction.convertAllToResultByConverter(dataTable, dtConverter);

                //    res.Add("packingPlantItem", strPackingPlant);
                //    res.Add("partIdItem", strPartId);
                //    res.Add("receiverItem", strReceiver);
                //    res.Add("supplierIdItem", strSupplierId);
                //    res.Add("supplierPlantItem", strSupplierPlant);
                //    res.Add("tempList", dataList);
                //    //这些作为子页面固定值
                //    res.Add("linid_mainItem", strLinId);
                //    apiResult.code = ComConstant.SUCCESS_CODE;
                //    apiResult.data = res;
                //}
                //if (strEditType == "PackingQtyEdit")
                //{
                //    DataTable dataTable = fs0603_Logic.getEditLoadInfo(strEditType, strPackingPlant, strPartId, strReceiver, strSupplierId, strSupplierPlant);
                //    DtConverter dtConverter = new DtConverter();
                //    dtConverter.addField("bAddFlag", ConvertFieldType.BoolType, null);
                //    dtConverter.addField("dFromTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                //    dtConverter.addField("dToTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                //    List<Object> dataList = ComFunction.convertAllToResultByConverter(dataTable, dtConverter);

                //    res.Add("packingPlantItem", strPackingPlant);
                //    res.Add("partIdItem", strPartId);
                //    res.Add("receiverItem", strReceiver);
                //    res.Add("supplierIdItem", strSupplierId);
                //    res.Add("supplierPlantItem", strSupplierPlant);
                //    res.Add("tempList", dataList);
                //    //这些作为子页面固定值
                //    res.Add("linid_mainItem", strLinId);
                //    apiResult.code = ComConstant.SUCCESS_CODE;
                //    apiResult.data = res;
                //}
                //if (strEditType == "SufferInEdit")
                //{
                //    DataTable dataTable = fs0603_Logic.getEditLoadInfo(strEditType, strPackingPlant, strPartId, strReceiver, strSupplierId, strSupplierPlant);
                //    DtConverter dtConverter = new DtConverter();
                //    dtConverter.addField("bAddFlag", ConvertFieldType.BoolType, null);
                //    dtConverter.addField("dFromTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                //    dtConverter.addField("dToTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                //    List<Object> dataList = ComFunction.convertAllToResultByConverter(dataTable, dtConverter);

                //    res.Add("packingPlantItem", strPackingPlant);
                //    res.Add("partIdItem", strPartId);
                //    res.Add("receiverItem", strReceiver);
                //    res.Add("supplierIdItem", strSupplierId);
                //    res.Add("supplierPlantItem", strSupplierPlant);
                //    res.Add("tempList", dataList);
                //    //这些作为子页面固定值
                //    res.Add("linid_mainItem", strLinId);
                //    apiResult.code = ComConstant.SUCCESS_CODE;
                //    apiResult.data = res;
                //}
                //if (strEditType == "OrderPlantEdit")
                //{
                //    DataTable dataTable = fs0603_Logic.getEditLoadInfo(strEditType, strPackingPlant, strPartId, strReceiver, strSupplierId, strSupplierPlant);
                //    DtConverter dtConverter = new DtConverter();
                //    dtConverter.addField("bAddFlag", ConvertFieldType.BoolType, null);
                //    dtConverter.addField("dFromTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                //    dtConverter.addField("dToTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                //    List<Object> dataList = ComFunction.convertAllToResultByConverter(dataTable, dtConverter);

                //    res.Add("packingPlantItem", strPackingPlant);
                //    res.Add("partIdItem", strPartId);
                //    res.Add("receiverItem", strReceiver);
                //    res.Add("supplierIdItem", strSupplierId);
                //    res.Add("supplierPlantItem", strSupplierPlant);
                //    res.Add("tempList", dataList);
                //    //这些作为子页面固定值
                //    res.Add("linid_mainItem", strLinId);
                //    apiResult.code = ComConstant.SUCCESS_CODE;
                //    apiResult.data = res;
                //}
                #endregion
                DataTable dataTable = fs0603_Logic.getEditLoadInfo(strEditType, strPackingPlant, strPartId, strReceiver, strSupplierId, strSupplierPlant);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("bAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("dFromTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dToTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dataTable, dtConverter);

                res.Add("packingPlantItem", strPackingPlant);
                res.Add("partIdItem", strPartId);
                res.Add("receiverItem", strReceiver);
                res.Add("supplierIdItem", strSupplierId);
                res.Add("supplierPlantItem", strSupplierPlant);
                res.Add("tempList", dataList);
                //这些作为子页面固定值
                res.Add("linid_mainItem", strLinId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
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
                string strEditType = dataForm.type;//维护类型
                dynamic dSubform = dataForm.subform.list;//维护页面数据列表
                List<Dictionary<string, Object>> listSubInfo = dSubform.ToObject<List<Dictionary<string, Object>>>();

                string LinId_main = dataForm.subform.LinId_main;//隐藏维护数据信息
                string PackingPlant = dataForm.subform.PackingPlant;
                string PartId = dataForm.subform.PartId;
                string Receiver = dataForm.subform.Receiver;
                string SupplierId = dataForm.subform.SupplierId;
                string SupplierPlant = dataForm.subform.SupplierPlant;
                if (strEditType == "SupplierPlantEdit")
                {
                    if (listSubInfo.Count == 0)//不存任何有效期履历
                    {
                        res.Add("SupplierPlant", "待维护");
                        res.Add("SupplierPlant_ed", "");
                        res.Add("SupplierPlantLinId_ed", "");
                        res.Add("SupplierPlantFromTime_ed", "");
                        res.Add("SupplierPlantToTime_ed", "");
                        res.Add("LinId_mainItem", LinId_main);
                        apiResult.code = ComConstant.SUCCESS_CODE;
                        apiResult.data = res;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    else
                    {
                        //只取第一行数据（每次只能追加一条数据有效期）
                        string strLinId = listSubInfo[0]["LinId"].ToString();
                        string strSupplierPlant = listSubInfo[0]["vcSupplierPlant"].ToString();
                        string dFromTime = listSubInfo[0]["dFromTime"].ToString();
                        string dToTime = listSubInfo[0]["dToTime"].ToString();
                        DataTable dataTable = fs0603_Logic.getEditLoadInfo(strEditType, PackingPlant, PartId, Receiver, SupplierId, SupplierPlant);
                        if (dataTable.Rows.Count > 0 && Convert.ToInt32(strLinId) < 0)
                        {
                            if (Convert.ToDateTime(dataTable.Rows[dataTable.Rows.Count - 1]["dFromTime"].ToString()) >= Convert.ToDateTime(dFromTime))
                            {
                                //报错：不符合上一个当前有效的开始使用时间小于维护的开始使用时间
                            }
                            dataTable.Rows[dataTable.Rows.Count - 1]["dToTime"] = Convert.ToDateTime(dFromTime).AddDays(-1);
                        }
                        if (Convert.ToInt32(strLinId) < 0)
                        {
                            //判断当前生效的数据
                            DataRow dataRow = dataTable.NewRow();
                            dataRow["LinId"] = strLinId;
                            dataRow["dFromTime"] = dFromTime;
                            dataRow["dToTime"] = dToTime;
                            dataRow["vcSupplierPlant"] = strSupplierPlant;
                            dataRow["bAddFlag"] = "1";
                            dataRow["vcBgColor"] = "";
                            dataTable.Rows.InsertAt(dataRow, 0);//插入到第一行
                        }
                        res.Add("SupplierPlant", "待维护");
                        foreach (DataRow item in dataTable.Rows)
                        {
                            if (Convert.ToDateTime(item["dFromTime"].ToString()) <= Convert.ToDateTime(System.DateTime.Now.ToString("yyyy-MM-dd"))
                                && Convert.ToDateTime(item["dToTime"].ToString()) >= Convert.ToDateTime(System.DateTime.Now.ToString("yyyy-MM-dd")))
                            {
                                //当前有效的
                                res.Remove("SupplierPlant");
                                res.Add("SupplierPlant", item["vcSupplierPlant"].ToString());
                                break;
                            }
                        }
                        res.Add("SupplierPlant_ed", strSupplierPlant);
                        res.Add("SupplierPlantLinId_ed", strLinId);
                        res.Add("SupplierPlantFromTime_ed", dFromTime);
                        res.Add("SupplierPlantToTime_ed", dToTime);
                        res.Add("LinId_mainItem", LinId_main);
                        apiResult.code = ComConstant.SUCCESS_CODE;
                        apiResult.data = res;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                if (strEditType == "PackingQtyEdit")
                {
                    if (listSubInfo.Count == 0)//不存任何有效期履历
                    {
                        res.Add("iPackingQty", "待维护");
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
                        res.Add("LinId_mainItem", LinId_main);
                        apiResult.code = ComConstant.SUCCESS_CODE;
                        apiResult.data = res;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    else
                    {
                        //只取第一行数据（每次只能追加一条数据有效期）
                        string strLinId = listSubInfo[0]["LinId"].ToString();
                        string strPackingQty = listSubInfo[0]["iPackingQty"].ToString();
                        string strBoxType = listSubInfo[0]["vcBoxType"].ToString();
                        string strLength = listSubInfo[0]["iLength"].ToString();
                        string strWidth = listSubInfo[0]["iWidth"].ToString();
                        string strHeight = listSubInfo[0]["iHeight"].ToString();
                        string strVolume = (Convert.ToInt32(listSubInfo[0]["iLength"].ToString()) * Convert.ToInt32(listSubInfo[0]["iWidth"].ToString()) * Convert.ToInt32(listSubInfo[0]["iHeight"].ToString())).ToString();
                        string dFromTime = listSubInfo[0]["dFromTime"].ToString();
                        string dToTime = listSubInfo[0]["dToTime"].ToString();
                        DataTable dataTable = fs0603_Logic.getEditLoadInfo(strEditType, PackingPlant, PartId, Receiver, SupplierId, SupplierPlant);
                        if (dataTable.Rows.Count > 0 && Convert.ToInt32(strLinId) < 0)
                        {
                            if (Convert.ToDateTime(dataTable.Rows[dataTable.Rows.Count - 1]["dFromTime"].ToString()) >= Convert.ToDateTime(dFromTime))
                            {
                                //报错：不符合上一个当前有效的开始使用时间小于维护的开始使用时间
                            }
                            dataTable.Rows[dataTable.Rows.Count - 1]["dToTime"] = Convert.ToDateTime(dFromTime).AddDays(-1);
                        }
                        if (Convert.ToInt32(strLinId) < 0)
                        {
                            //判断当前生效的数据
                            DataRow dataRow = dataTable.NewRow();
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
                            dataTable.Rows.InsertAt(dataRow, 0);//插入到第一行
                        }
                        res.Add("iPackingQty", "待维护");
                        res.Add("vcBoxType", "");
                        res.Add("iLength", "");
                        res.Add("iWidth", "");
                        res.Add("iHeight", "");
                        res.Add("iVolume", "");
                        foreach (DataRow item in dataTable.Rows)
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
                        res.Add("LinId_mainItem", LinId_main);
                        apiResult.code = ComConstant.SUCCESS_CODE;
                        apiResult.data = res;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                if (strEditType == "SufferInEdit")
                {
                    if (listSubInfo.Count == 0)//不存任何有效期履历
                    {
                        res.Add("SufferIn", "待维护");
                        res.Add("SufferIn_ed", "");
                        res.Add("SufferInLinId_ed", "");
                        res.Add("SufferInFromTime_ed", "");
                        res.Add("SufferInToTime_ed", "");
                        res.Add("LinId_mainItem", LinId_main);
                        apiResult.code = ComConstant.SUCCESS_CODE;
                        apiResult.data = res;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    else
                    {
                        //只取第一行数据（每次只能追加一条数据有效期）
                        string strLinId = listSubInfo[0]["LinId"].ToString();
                        string strSufferIn = listSubInfo[0]["vcSufferIn"].ToString();
                        string dFromTime = listSubInfo[0]["dFromTime"].ToString();
                        string dToTime = listSubInfo[0]["dToTime"].ToString();
                        DataTable dataTable = fs0603_Logic.getEditLoadInfo(strEditType, PackingPlant, PartId, Receiver, SupplierId, SupplierPlant);
                        if (dataTable.Rows.Count > 0 && Convert.ToInt32(strLinId) < 0)
                        {
                            if (Convert.ToDateTime(dataTable.Rows[dataTable.Rows.Count - 1]["dFromTime"].ToString()) >= Convert.ToDateTime(dFromTime))
                            {
                                //报错：不符合上一个当前有效的开始使用时间小于维护的开始使用时间
                            }
                            dataTable.Rows[dataTable.Rows.Count - 1]["dToTime"] = Convert.ToDateTime(dFromTime).AddDays(-1);
                        }
                        if (Convert.ToInt32(strLinId) < 0)
                        {
                            //判断当前生效的数据
                            DataRow dataRow = dataTable.NewRow();
                            dataRow["LinId"] = strLinId;
                            dataRow["dFromTime"] = dFromTime;
                            dataRow["dToTime"] = dToTime;
                            dataRow["vcSufferIn"] = strSufferIn;
                            dataRow["bAddFlag"] = "1";
                            dataRow["vcBgColor"] = "";
                            dataTable.Rows.InsertAt(dataRow, 0);//插入到第一行
                        }
                        res.Add("SufferIn", "待维护");
                        foreach (DataRow item in dataTable.Rows)
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
                        res.Add("LinId_mainItem", LinId_main);
                        apiResult.code = ComConstant.SUCCESS_CODE;
                        apiResult.data = res;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                if (strEditType == "OrderPlantEdit")
                {
                    //DataTable dtOrderPlant = ComFunction.getTCode("C000");
                    if (listSubInfo.Count == 0)//不存任何有效期履历
                    {
                        res.Add("OrderPlant", "待维护");
                        res.Add("OrderPlant_ed", "");
                        res.Add("OrderPlantLinId_ed", "");
                        res.Add("OrderPlantFromTime_ed", "");
                        res.Add("OrderPlantToTime_ed", "");
                        res.Add("LinId_mainItem", LinId_main);
                        apiResult.code = ComConstant.SUCCESS_CODE;
                        apiResult.data = res;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    else
                    {
                        //只取第一行数据（每次只能追加一条数据有效期）
                        string strLinId = listSubInfo[0]["LinId"].ToString();
                        string strOrderPlant = listSubInfo[0]["vcOrderPlant"].ToString();
                        string dFromTime = listSubInfo[0]["dFromTime"].ToString();
                        string dToTime = listSubInfo[0]["dToTime"].ToString();
                        DataTable dataTable = fs0603_Logic.getEditLoadInfo(strEditType, PackingPlant, PartId, Receiver, SupplierId, SupplierPlant);
                        if (dataTable.Rows.Count > 0 && Convert.ToInt32(strLinId) < 0)
                        {
                            if (Convert.ToDateTime(dataTable.Rows[dataTable.Rows.Count - 1]["dFromTime"].ToString()) >= Convert.ToDateTime(dFromTime))
                            {
                                //报错：不符合上一个当前有效的开始使用时间小于维护的开始使用时间
                            }
                            dataTable.Rows[dataTable.Rows.Count - 1]["dToTime"] = Convert.ToDateTime(dFromTime).AddDays(-1);
                        }
                        if (Convert.ToInt32(strLinId) < 0)
                        {
                            //判断当前生效的数据
                            DataRow dataRow = dataTable.NewRow();
                            dataRow["LinId"] = strLinId;
                            dataRow["dFromTime"] = dFromTime;
                            dataRow["dToTime"] = dToTime;
                            dataRow["vcOrderPlant"] = strOrderPlant;
                            dataRow["bAddFlag"] = "1";
                            dataRow["vcBgColor"] = "";
                            dataTable.Rows.InsertAt(dataRow, 0);//插入到第一行
                        }
                        res.Add("OrderPlant", "待维护");
                        foreach (DataRow item in dataTable.Rows)
                        {
                            if (Convert.ToDateTime(item["dFromTime"].ToString()) <= Convert.ToDateTime(System.DateTime.Now.ToString("yyyy-MM-dd"))
                                && Convert.ToDateTime(item["dToTime"].ToString()) >= Convert.ToDateTime(System.DateTime.Now.ToString("yyyy-MM-dd")))
                            {
                                //当前有效的
                                res.Remove("OrderPlant");
                                res.Add("OrderPlant", item["vcOrderPlant"].ToString());
                                break;
                            }
                        }
                        res.Add("OrderPlant_ed", strOrderPlant);
                        res.Add("OrderPlantLinId_ed", strLinId);
                        res.Add("OrderPlantFromTime_ed", dFromTime);
                        res.Add("OrderPlantToTime_ed", dToTime);
                        res.Add("LinId_mainItem", LinId_main);
                        apiResult.code = ComConstant.SUCCESS_CODE;
                        apiResult.data = res;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
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
        /// 一括赋予有效期
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string saveallinApi([FromBody]dynamic data)
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

                dynamic dsearchform = dataForm.searchform;//检索条件
                dynamic dallininfo = dataForm.allininfo;//allin条件

                string strSyncTime = dsearchform.SyncTime;
                string strPartId = dsearchform.PartId;
                string strCarModel = dsearchform.CarModel;
                string strReceiver = dsearchform.Receiver;
                string strInOut = dsearchform.InOut;
                string strSupplierId = dsearchform.SupplierId;
                string strSupplierPlant = dsearchform.SupplierPlant;
                string strFromTime = dsearchform.FromTime;
                string strToTime = dsearchform.ToTime;
                string strHaoJiu = dsearchform.HaoJiu;
                string strOrderPlant = dsearchform.OrderPlant;

                string dFromTime = dallininfo.FromTime == null ? "" : dallininfo.FromTime;
                string dToTime = dallininfo.ToTime == null ? "" : dallininfo.ToTime;
                if (dFromTime == "" || dToTime == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "有效期不能为空";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                DateTime FromTime = Convert.ToDateTime(dFromTime);
                DateTime ToTime = Convert.ToDateTime(dToTime);
                if (FromTime > ToTime)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "品番有效期的开始时间不能大于结束时间";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                DataTable dataTable = fs0603_Logic.getSearchInfo(strSyncTime, strPartId, strCarModel, strReceiver, strInOut,
                    strSupplierId, strSupplierPlant, strFromTime, strToTime, strHaoJiu, strOrderPlant);

                string strErrorPartId = "";
                fs0603_Logic.setAllInTime(dataTable, FromTime, ToTime, loginInfo.UserId, ref strErrorPartId,
                    strSyncTime, strPartId, strCarModel, strReceiver, strInOut,
                    strSupplierId, strSupplierPlant, strFromTime, strToTime, strHaoJiu, strOrderPlant);
                if (strErrorPartId != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "保存失败，以下品番使用开始、结束区间存在重叠：<br/>" + strErrorPartId;
                    apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
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
                apiResult.data = "保存失败";
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

            string strSyncTime = string.Empty;
            string strPartId = string.Empty;
            string strCarModel = string.Empty;
            string strReceiver = string.Empty;
            string strInOut = string.Empty;
            string strSupplierId = string.Empty;
            string strSupplierPlant = string.Empty;
            string strFromTime = string.Empty;
            string strToTime = string.Empty;
            string strHaoJiu = string.Empty;
            string strOrderPlant = string.Empty;
            try
            {
                DataTable dataTable = fs0603_Logic.getSearchInfo(strSyncTime, strPartId, strCarModel, strReceiver, strInOut,
                   strSupplierId, strSupplierPlant, strFromTime, strToTime, strHaoJiu, strOrderPlant);

                string[] fields = {"dSyncTime","vcChanges_name","vcPackingPlant","vcPackingPlant_name","vcPartId","vcPartENName","vcCarfamilyCode",
                    "vcReceiver","vcReceiver_name","dFromTime","dToTime","vcPartId_Replace","vcInOut_name","vcOESP_name","vcHaoJiu_name",
                    "vcOldProduction_name","dOldStartTime","dDebugTime","vcSupplierId","dSupplierFromTime","dSupplierToTime","vcSupplierName",
                    "SupplierPlant_ed","SupplierPlantFromTime_ed","SupplierPlantToTime_ed",
                    "vcSupplierPlace",
                    "BoxPackingQty_ed","BoxFromTime_ed","BoxToTime_ed","BoxType_ed","BoxLength_ed","BoxWidth_ed","BoxHeight_ed","BoxVolume_ed",
                    "SufferIn_ed","SufferInFromTime_ed","SufferInToTime_ed",
                    "vcOrderPlant_name","OrderPlantFromTime_ed","OrderPlantToTime_ed",
                    "vcInteriorProject","vcPassProject","vcFrontProject","dFrontProjectTime","dShipmentTime",
                    "vcBillType_name","vcRemark1","vcRemark2",
                    "vcOrderingMethod_name","vcMandOrder_name"
                };

                string filepath = ComFunction.generateExcelWithXlt(dataTable, fields, _webHostEnvironment.ContentRootPath, "FS0603_Template.xlsx", 2, loginInfo.UserId, FunctionID);
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
                    {"同步时间","变更事项","包装工厂","补给品番","品名(英文)","车种代码",
                        "收货方","品番-使用开始","品番-使用结束","替代品番",
                        "内外区分","OE=SP","号旧区分","旧型年限生产区分","旧型开始时间","实施年月(年限)",
                        "供应商编号","供应商使用开始","供应商使用结束","供应商名称",
                        "工区","工区-使用开始","工区-使用结束",
                        "供应商出荷地",
                        "补给收容数","收容数-使用开始","收容数-使用结束","箱种","长（mm)","宽（mm)","高（mm)","体积（mm³)",
                        "受入","受入-使用开始","受入-使用结束",
                        "发注工厂","发注工厂-使用开始","发注工厂-使用结束",
                        "内制工程","通过工程","前工程","前工程通过时间","自工程出货时间",
                        "单据区分","备注1","备注2","订货方式","强制订货"},
                    {"dSyncTime","vcChanges_name","vcPackingPlant","vcPartId","vcPartENName","vcCarfamilyCode",
                        "vcReceiver","dFromTime","dToTime","vcPartId_Replace",
                        "vcInOut_name","vcOESP_name","vcHaoJiu_name","vcOldProduction_name","dOldStartTime","dDebugTime",
                        "vcSupplierId","dSupplierFromTime","dSupplierToTime","vcSupplierName",
                        "SupplierPlant_ed","SupplierPlantFromTime_ed","SupplierPlantToTime_ed",
                        "vcSupplierPlace",
                        "BoxPackingQty_ed","BoxFromTime_ed","BoxToTime_ed","BoxType_ed","BoxLength_ed","BoxWidth_ed","BoxHeight_ed","BoxVolume_ed",
                        "SufferIn_ed","SufferInFromTime_ed","SufferInToTime_ed",
                        "vcOrderPlant_name","OrderPlantFromTime_ed","OrderPlantToTime_ed",
                        "vcInteriorProject","vcPassProject","vcFrontProject","dFrontProjectTime","dShipmentTime",
                        "vcBillType_name","vcRemark1","vcRemark2","vcOrderingMethod_name","vcMandOrder_name"},
                    {"","","","","","",
                        "","","","",
                        "","","","","","",
                        "","","","",
                        FieldCheck.NumCharLLL,FieldCheck.Date,FieldCheck.Date,
                        "",
                        FieldCheck.Num,FieldCheck.Date,FieldCheck.Date,FieldCheck.NumCharLLL,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,"",
                        FieldCheck.NumCharLLL,FieldCheck.Date,FieldCheck.Date,
                        "",FieldCheck.Date,FieldCheck.Date,
                        "","","",FieldCheck.Date,FieldCheck.Date,
                        "","","","",""},
                    {"0","0","0","0","0","0",
                        "0","0","0","0",
                        "0","0","0","0","0","0",
                        "0","0","0","0",
                        "1","0","0",
                        "50",
                        "5","0","0","5","5","5","5","0",
                        "2","0","0",
                        "20","0","0",
                        "50","50","50","0","0",
                        "10","100","100","10","10"},//最大长度设定,不校验最大长度用0
                    {"0","0","0","0","0","0",
                        "0","0","0","0",
                        "0","0","0","0","0","0",
                        "0","0","0","0",
                        "1","1","1",
                        "0",
                        "1","1","1","1","1","1","1","0",
                        "1","1","1",
                        "1","1","1",
                        "0","0","0","0","0",
                        "1","0","0","1","1"}};//最小长度设定,可以为空用0
                #endregion
                DataTable importDt = new DataTable();
                DataTable dtMessage = fs0603_Logic.createTable("MES");
                bool bReault = true;
                foreach (FileInfo info in theFolder.GetFiles())
                {
                    DataTable dt = ComFunction.ExcelToDataTableformRows(info.FullName, "sheet1", headers, 2, 3, ref strMsg);
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
                apiResult.data = "保存成功";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0905", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.type = "info";
                apiResult.data = "保存失败" + ex.Message;
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
    }
}
