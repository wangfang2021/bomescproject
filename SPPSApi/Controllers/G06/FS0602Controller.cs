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
    [Route("api/FS0602/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0602Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        FS0602_Logic fs0602_Logic = new FS0602_Logic();
        FS0603_Logic fs0603_Logic = new FS0603_Logic();
        FS0402_Logic fs0402_Logic = new FS0402_Logic();
        FS0501_Logic fs0501_Logic = new FS0501_Logic();
        private readonly string FunctionID = "FS0602";
        public FS0602Controller(IWebHostEnvironment webHostEnvironment)
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

                List<Object> DyStateList = ComFunction.convertAllToResult(fs0402_Logic.getTCode("C036"));//对应状态
                List<Object> HyStateList = ComFunction.convertAllToResult(ComFunction.getTCode("C037"));//合意状态
                List<Object> InOutList = ComFunction.convertAllToResult(ComFunction.getTCode("C003"));//内外区分
                List<Object> OrderingMethodList = ComFunction.convertAllToResult(ComFunction.getTCode("C047"));//订货方式
                List<Object> OrderPlantList = ComFunction.convertAllToResult(ComFunction.getTCode("C000"));//发注工厂
                List<Object> HaoJiuList = ComFunction.convertAllToResult(ComFunction.getTCode("C004"));//号旧区分
                DateTime dNow = DateTime.Now.AddMonths(1);

                res.Add("DyStateList", DyStateList);
                res.Add("HyStateList", HyStateList);
                res.Add("InOutList", InOutList);
                res.Add("OrderingMethodList", OrderingMethodList);
                res.Add("OrderPlantList", OrderPlantList);
                res.Add("HaoJiuList", HaoJiuList);
                res.Add("yearMonth", dNow.ToString("yyyy/MM"));

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

            string strYearMonth = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth + "/01").ToString("yyyyMM");
            string strDyState = dataForm.DyState;
            string strHyState = dataForm.HyState;
            string strPartId = dataForm.PartId;
            string strCarModel = dataForm.CarModel;
            string strInOut = dataForm.InOut;
            string strOrderingMethod = dataForm.OrderingMethod;
            string strOrderPlant = dataForm.OrderPlant;
            string strHaoJiu = dataForm.HaoJiu;
            string strSupplierId = dataForm.SupplierId;
            string strSupplierPlant = dataForm.SupplierPlant;
            string strDataState = dataForm.DataState;

            try
            {
                DataTable dataTable = fs0602_Logic.getSearchInfo(strYearMonth, strDyState, strHyState, strPartId, strCarModel,
                    strInOut, strOrderingMethod, strOrderPlant, strHaoJiu, strSupplierId, strSupplierPlant, strDataState);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("bModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("bAddFlag", ConvertFieldType.BoolType, null);
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

            string strDyState = dataForm.DyState;
            string strHyState = dataForm.HyState;
            string strPartId = dataForm.PartId;
            string strCarModel = dataForm.CarModel;
            string strInOut = dataForm.InOut;
            string strOrderingMethod = dataForm.OrderingMethod;
            string strOrderPlant = dataForm.OrderPlant;
            string strHaoJiu = dataForm.HaoJiu;
            string strSupplierId = dataForm.SupplierId;
            string strSupplierPlant = dataForm.SupplierPlant;
            string strDataState = dataForm.DataState;
            try
            {
                string strYearMonth = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth).ToString("yyyyMM");
                string strYearMonth_2 = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth).AddMonths(1).ToString("yyyyMM");
                string strYearMonth_3 = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth).AddMonths(2).ToString("yyyyMM");
                DataTable dataTable = fs0602_Logic.getSearchInfo(strYearMonth, strDyState, strHyState, strPartId, strCarModel,
                     strInOut, strOrderingMethod, strOrderPlant, strHaoJiu, strSupplierId, strSupplierPlant, strDataState);

                string[] fields = {"vcYearMonth","vcDyState_Name","vcHyState_Name","vcPart_id","vcCarfamilyCode","vcHaoJiu","vcOrderingMethod",
                    "vcOrderPlant","vcInOut",
                    "vcSupplierId","vcSupplierPlant","iPackingQty",
                    "iCbSOQN","decCbBdl","iCbSOQN1","iCbSOQN2",
                    "iTzhSOQN","iTzhSOQN1","iTzhSOQN2",
                    "iHySOQN","iHySOQN1","iHySOQN2",
                    "dExpectTime","dSReplyTime","vcOverDue","dHyTime"
                };

                //string filepath = ComFunction.generateExcelWithXlt(dataTable, fields, _webHostEnvironment.ContentRootPath, "FS0602_Export.xlsx", 1, loginInfo.UserId, FunctionID);
                string filepath = fs0602_Logic.generateExcelWithXlt(dataTable, fields, _webHostEnvironment.ContentRootPath, "FS0602_Export.xlsx", 2, loginInfo.UserId, FunctionID, strYearMonth, strYearMonth_2, strYearMonth_3);
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
        /// 导出履历方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string exportresumeApi([FromBody] dynamic data)
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
                DataTable dt = fs0402_Logic.SearchHistory();
                string[] heads = { "年月", "错误消息" };
                string[] fields = { "vcYearMonth", "vcMessage" };
                string strMsg = "";
                string filepath = ComFunction.DataTableToExcel(heads, fields, dt, _webHostEnvironment.ContentRootPath, loginInfo.UserId, FunctionID, ref strMsg);
                if (strMsg != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = strMsg;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = filepath;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M04UE0205", ex, loginInfo.UserId);
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
                JArray listMultiple = (dataForm.selectmultiple).multipleSelection;
                List<Dictionary<string, Object>> listMultipleData = listMultiple.ToObject<List<Dictionary<string, Object>>>();
                JArray listInfo = dataForm.alltemp.list;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
                string strExpectTime = dataForm.ExpectTime == null ? "" : dataForm.ExpectTime.ToString();
                bool hasFind = false;//是否找到需要新增或者修改的数据
                List<string> lsYearMonth = new List<string>();
                for (int i = 0; i < listMultipleData.Count; i++)
                {
                    string ym = listMultipleData[i]["vcYearMonth"].ToString();
                    ym = ym.Substring(0, 4) + "-" + ym.Substring(4, 2) + "-01";

                    bool bModFlag = (bool)listMultipleData[i]["bModFlag"];//true可编辑,false不可编辑
                    bool bAddFlag = (bool)listMultipleData[i]["bAddFlag"];//true可编辑,false不可编辑
                    if (bAddFlag == true)
                        hasFind = true;//新增
                    else if (bAddFlag == false && bModFlag == true)
                    {
                        hasFind = true;//修改
                        if (!lsYearMonth.Contains(ym))
                        {
                            lsYearMonth.Add(ym);
                        }
                    }
                }
                if (!hasFind)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "info";
                    apiResult.data = "最少有一个编辑行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string strYearMonth = Convert.ToDateTime(lsYearMonth[0].ToString()).ToString("yyyyMM");
                string strYearMonth1 = Convert.ToDateTime(lsYearMonth[0].ToString()).AddMonths(1).ToString("yyyyMM");
                string strYearMonth2 = Convert.ToDateTime(lsYearMonth[0].ToString()).AddMonths(2).ToString("yyyyMM");
                DataTable dtMessage = fs0603_Logic.createTable("MES");
                DataTable dtMultiple = fs0603_Logic.createTable("SOQ602");
                dtMultiple = fs0602_Logic.setListInfo(listMultipleData, dtMultiple, strExpectTime);
                bool bReault = true;
                string strReceiver = "APC06";
                DataTable dtImport = fs0602_Logic.checkSaveInfo(dtMultiple, strYearMonth, strYearMonth1, strYearMonth2,
                     loginInfo.UserId, loginInfo.UnitCode, strReceiver, ref bReault, ref dtMessage);
                if (!bReault)
                {
                    //弹出错误dtMessage
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fs0602_Logic.setSaveInfo(strYearMonth, loginInfo.UserId, ref dtMessage);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M08UE0704", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 展开内示方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string openplanApi([FromBody]dynamic data)
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

                string strYearMonth = dataForm.searchform.YearMonth == null ? "" : Convert.ToDateTime(dataForm.searchform.YearMonth + "/01").ToString("yyyyMM");
                string strDyState = dataForm.searchform.DyState;
                string strHyState = dataForm.searchform.HyState;
                string strPartId = dataForm.searchform.PartId;
                string strCarModel = dataForm.searchform.CarModel;
                string strInOut = dataForm.searchform.InOut;
                string strOrderingMethod = dataForm.searchform.OrderingMethod;
                string strOrderPlant = dataForm.searchform.OrderPlant;
                string strHaoJiu = dataForm.searchform.HaoJiu;
                string strSupplierId = dataForm.searchform.SupplierId;
                string strSupplierPlant = dataForm.searchform.SupplierPlant;
                string strDataState = dataForm.searchform.DataState;

                string strExpectTime = dataForm.info;//期望回复日
                JArray checkedInfo = dataForm.searchform.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();

                if (strYearMonth == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "info";
                    apiResult.data = "对象年月不能为空";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (strExpectTime == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "info";
                    apiResult.data = "期望回复日期不能为空";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                DataTable dtMessage = fs0603_Logic.createTable("MES");
                DataTable dataTable = fs0602_Logic.getSearchInfo(strYearMonth, strDyState, strHyState, strPartId, strCarModel,
                   strInOut, strOrderingMethod, strOrderPlant, strHaoJiu, strSupplierId, strSupplierPlant, strDataState);
                bool bReault = true;
                string strReceiver = "APC06";
                DataTable dtImport = fs0602_Logic.checkopenplanInfo(listInfoData, dataTable, strExpectTime,
                    loginInfo.UserId, loginInfo.UnitCode, strReceiver, ref bReault, ref dtMessage);
                if (!bReault)
                {
                    //弹出错误dtMessage
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fs0602_Logic.openPlan(dtImport, loginInfo.UserId, ref dtMessage);
                if (dtMessage.Rows.Count != 0)
                {
                    //弹出错误dtMessage
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fs0602_Logic.sendMail(loginInfo, dtImport, ref dtMessage);
                if (dtMessage.Rows.Count != 0)
                {
                    //弹出错误dtMessage
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list->search";
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
                apiResult.data = "内示展开失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 回复内示方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string replyplanApi([FromBody]dynamic data)
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
                string strYearMonth = dataForm.searchform.YearMonth == null ? "" : Convert.ToDateTime(dataForm.searchform.YearMonth + "/01").ToString("yyyyMM");
                string strDyState = dataForm.searchform.DyState;
                string strHyState = dataForm.searchform.HyState;
                string strPartId = dataForm.searchform.PartId;
                string strCarModel = dataForm.searchform.CarModel;
                string strInOut = dataForm.searchform.InOut;
                string strOrderingMethod = dataForm.searchform.OrderingMethod;
                string strOrderPlant = dataForm.searchform.OrderPlant;
                string strHaoJiu = dataForm.searchform.HaoJiu;
                string strSupplierId = dataForm.searchform.SupplierId;
                string strSupplierPlant = dataForm.searchform.SupplierPlant;
                string strDataState = dataForm.searchform.DataState;

                string strExpectTime = dataForm.info;//期望回复日
                JArray checkedInfo = dataForm.searchform.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();

                DataTable dtMessage = fs0603_Logic.createTable("MES");
                DataTable dataTable = fs0602_Logic.getSearchInfo(strYearMonth, strDyState, strHyState, strPartId, strCarModel,
                   strInOut, strOrderingMethod, strOrderPlant, strHaoJiu, strSupplierId, strSupplierPlant, strDataState);
                bool bReault = true;
                string strReceiver = "APC06";
                DataTable dtImport = fs0602_Logic.checkreplyplanInfo(listInfoData, dataTable, strExpectTime,
                    loginInfo.UserId, loginInfo.UnitCode, strReceiver, ref bReault, ref dtMessage);
                if (!bReault)
                {
                    //弹出错误dtMessage
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fs0602_Logic.replyPlan(dtImport, loginInfo.UserId, ref dtMessage);
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
                apiResult.data = "内示回复失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }

        /// <summary>
        /// 回退内示方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string returnplanApi([FromBody]dynamic data)
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
                string strYearMonth = dataForm.searchform.YearMonth == null ? "" : Convert.ToDateTime(dataForm.searchform.YearMonth + "/01").ToString("yyyyMM");
                string strDyState = dataForm.searchform.DyState;
                string strHyState = dataForm.searchform.HyState;
                string strPartId = dataForm.searchform.PartId;
                string strCarModel = dataForm.searchform.CarModel;
                string strInOut = dataForm.searchform.InOut;
                string strOrderingMethod = dataForm.searchform.OrderingMethod;
                string strOrderPlant = dataForm.searchform.OrderPlant;
                string strHaoJiu = dataForm.searchform.HaoJiu;
                string strSupplierId = dataForm.searchform.SupplierId;
                string strSupplierPlant = dataForm.searchform.SupplierPlant;
                string strDataState = dataForm.searchform.DataState;

                string strExpectTime = dataForm.info;//期望回复日
                JArray checkedInfo = dataForm.searchform.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();

                if (strYearMonth == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "对象年月不能为空";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                DataTable dtMessage = fs0603_Logic.createTable("MES");
                DataTable dataTable = fs0602_Logic.getSearchInfo(strYearMonth, "", "", "", "",
                   "", "", "", "", "", "", "");
                if(dataTable.Rows.Count==0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "该对象月没有有效的内示情报";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                bool bReault = true;
                string strReceiver = "APC06";
                string strReturnym = fs0602_Logic.checkreturnplanInfo(listInfoData, dataTable, strExpectTime,
                    ref bReault, ref dtMessage);
                if (!bReault)
                {
                    //弹出错误dtMessage
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fs0602_Logic.replyPlan(strReturnym, loginInfo.UserId, ref dtMessage);
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
                apiResult.data = "内示回退失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
    }
}