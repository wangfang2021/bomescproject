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
                List<Object> DyStateList = ComFunction.convertAllToResult(ComFunction.getTCode("C036"));//对应状态
                List<Object> HyStateList = ComFunction.convertAllToResult(ComFunction.getTCode("C037"));//合意状态
                DataTable dtOptionsList = fs0603_Logic.getFormOptions("");
                List<Object> CarModelForForm = ComFunction.convertAllToResult(fs0603_Logic.getSelectOptions(dtOptionsList, "vcCarModel_Name", "vcCarModel_Value"));//车种选项
                List<Object> HaoJiuForForm = ComFunction.convertAllToResult(fs0603_Logic.getSelectOptions(dtOptionsList, "vcHaoJiu_Name", "vcHaoJiu_Value"));//号旧区分选项
                List<Object> OrderingMethodForForm = ComFunction.convertAllToResult(fs0603_Logic.getSelectOptions(dtOptionsList, "vcOrderingMethod_Name", "vcOrderingMethod_Value"));//订货方式
                List<Object> OrderPlantForForm = ComFunction.convertAllToResult(fs0603_Logic.getSelectOptions(dtOptionsList, "vcOrderPlant_Name", "vcOrderPlant_Value"));//发注工厂选项
                List<Object> InOutForForm = ComFunction.convertAllToResult(fs0603_Logic.getSelectOptions(dtOptionsList, "vcInOut_Name", "vcInOut_Value"));//内外区分选项
                List<Object> SupplierIdForForm = ComFunction.convertAllToResult(fs0603_Logic.getSelectOptions(dtOptionsList, "vcSupplierId_Name", "vcSupplierId_Value"));//供应商编码选项
                List<Object> SupplierPlantForForm = ComFunction.convertAllToResult(fs0603_Logic.getSelectOptions(dtOptionsList, "vcSupplierPlant_Name", "vcSupplierPlant_Value"));//工区
                DateTime dNow = DateTime.Now.AddMonths(1);

                res.Add("DyStateList", DyStateList);
                res.Add("HyStateList", HyStateList);
                res.Add("CarModelForForm", CarModelForForm);
                res.Add("HaoJiuForForm", HaoJiuForForm);
                res.Add("OrderingMethodForForm", OrderingMethodForForm);
                res.Add("OrderPlantForForm", OrderPlantForForm);
                res.Add("InOutForForm", InOutForForm);
                res.Add("SupplierIdForForm", SupplierIdForForm);
                res.Add("SupplierPlantForForm", SupplierPlantForForm);
                res.Add("yearMonth", dNow.ToString("yyyy/MM"));
                DataTable dtTask = fs0602_Logic.getTankInfo(dNow.ToString("yyyyMM"));
                res.Add("tasknum", dtTask.Rows.Count);
                res.Add("taskok", dtTask.Select("vcHyState='2'").Length);
                res.Add("taskng", dtTask.Select("vcHyState='3'").Length);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
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

            string strYearMonth = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth + "/01").ToString("yyyyMM");
            string strDyState = dataForm.DyState == null ? "" : dataForm.DyState;
            string strHyState = dataForm.HyState == null ? "" : dataForm.HyState;
            string strPartId = dataForm.PartId == null ? "" : dataForm.PartId;
            string strCarModel = dataForm.CarModel == null ? "" : dataForm.CarModel;
            string strHaoJiu = dataForm.HaoJiu == null ? "" : dataForm.HaoJiu;
            string strOrderingMethod = dataForm.OrderingMethod == null ? "" : dataForm.OrderingMethod;
            string strOrderPlant = dataForm.OrderPlant == null ? "" : dataForm.OrderPlant;
            string strInOut = dataForm.InOut == null ? "" : dataForm.InOut;
            string strSupplierId = dataForm.SupplierId == null ? "" : dataForm.SupplierId;
            string strSupplierPlant = dataForm.SupplierPlant == null ? "" : dataForm.SupplierPlant;
            string strOverDue = dataForm.OverDue == null ? "" : dataForm.OverDue;
            string strDataState = dataForm.DataState == null ? "" : dataForm.DataState;

            try
            {
                DataTable dtInfo = fs0602_Logic.getSearchInfo(strYearMonth, strDyState, strHyState, strPartId, strCarModel,
                    strInOut, strOrderingMethod, strOrderPlant, strHaoJiu, strSupplierId, strSupplierPlant, strDataState, strOverDue);
                DataTable dtHJ = fs0602_Logic.getHeJiInfo(strYearMonth, strDyState, strHyState, strPartId, strCarModel,
                    strInOut, strOrderingMethod, strOrderPlant, strHaoJiu, strSupplierId, strSupplierPlant, strDataState, strOverDue);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("bModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("bAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("bSelectFlag", ConvertFieldType.BoolType, null);
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dtInfo, dtConverter);
                DataTable dtTask = fs0602_Logic.getTankInfo(strYearMonth);
                res.Add("tasknum", dtTask.Rows.Count);
                res.Add("taskok", dtTask.Select("vcHyState='2'").Length);
                res.Add("taskng", dtTask.Select("vcHyState='3'").Length);
                res.Add("tempList", dataList);
                res.Add("hejiList", dtHJ);
                res.Add("message", "not");
                DataTable dtMessage = fs0603_Logic.createTable("MES");
                string strReceiver = "APC06";
                fs0602_Logic.checkDBInfo(strYearMonth, loginInfo.UnitCode, strReceiver, ref dtMessage);
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                {
                    res.Remove("message");
                    res.Add("message", "yes");
                    res.Add("messagelist", ComFunction.convertAllToResult(dtMessage));
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06PE0201", ex, loginInfo.UserId);
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

            string strYearMonth = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth + "/01").ToString("yyyyMM");
            string strDyState = dataForm.DyState == null ? "" : dataForm.DyState;
            string strHyState = dataForm.HyState == null ? "" : dataForm.HyState;
            string strPartId = dataForm.PartId == null ? "" : dataForm.PartId;
            string strCarModel = dataForm.CarModel == null ? "" : dataForm.CarModel;
            string strHaoJiu = dataForm.HaoJiu == null ? "" : dataForm.HaoJiu;
            string strOrderingMethod = dataForm.OrderingMethod == null ? "" : dataForm.OrderingMethod;
            string strOrderPlant = dataForm.OrderPlant == null ? "" : dataForm.OrderPlant;
            string strInOut = dataForm.InOut == null ? "" : dataForm.InOut;
            string strSupplierId = dataForm.SupplierId == null ? "" : dataForm.SupplierId;
            string strSupplierPlant = dataForm.SupplierPlant == null ? "" : dataForm.SupplierPlant;
            string strOverDue = dataForm.OverDue == null ? "" : dataForm.OverDue;
            string strDataState = dataForm.DataState == null ? "" : dataForm.DataState;
            try
            {
                string strYearMonth_2 = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth).AddMonths(1).ToString("yyyyMM");
                string strYearMonth_3 = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth).AddMonths(2).ToString("yyyyMM");
                DataTable dataTable = fs0602_Logic.getSearchInfo(strYearMonth, strDyState, strHyState, strPartId, strCarModel,
                     strInOut, strOrderingMethod, strOrderPlant, strHaoJiu, strSupplierId, strSupplierPlant, strDataState, strOverDue);

                string[] fields = {"vcYearMonth","vcDyState_Name","vcHyState_Name","vcPart_id","vcCarfamilyCode","vcHaoJiu","vcOrderingMethod",
                    "vcOrderPlant","vcInOut",
                    "vcSupplierId","vcSupplierPlant","iPackingQty",
                    "iCbSOQN","decCbBdl","iCbSOQN1","iCbSOQN2",
                    "iTzhSOQN","iTzhSOQN1","iTzhSOQN2",
                    "iHySOQN","iHySOQN1","iHySOQN2",
                    "dExpectTime","dSReplyTime","vcOverDue","dHyTime"
                };

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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06PE0202", ex, loginInfo.UserId);
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
                string[] heads = { "对象年月", "品番", "错误消息" };
                string[] fields = { "vcYearMonth", "vcPart_id", "vcMessage" };
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06PE0203", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出履历失败";
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
                string strReceiver = "APC06";
                DataTable dtImport = fs0602_Logic.checkSaveInfo(dtMultiple, strYearMonth, strYearMonth1, strYearMonth2,
                     loginInfo.UserId, loginInfo.UnitCode, strReceiver, ref dtMessage);
                if (dtMessage != null && dtMessage.Rows.Count != 0)
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06PE0204", ex, loginInfo.UserId);
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
                string strDyState = dataForm.searchform.DyState == null ? "" : dataForm.searchform.DyState.ToString();
                string strHyState = dataForm.searchform.HyState == null ? "" : dataForm.searchform.HyState.ToString();
                string strPartId = dataForm.searchform.PartId == null ? "" : dataForm.searchform.PartId.ToString();
                string strCarModel = dataForm.searchform.CarModel == null ? "" : dataForm.searchform.CarModel.ToString();
                string strInOut = dataForm.searchform.InOut == null ? "" : dataForm.searchform.InOut.ToString();
                string strOrderingMethod = dataForm.searchform.OrderingMethod == null ? "" : dataForm.searchform.OrderingMethod.ToString();
                string strOrderPlant = dataForm.searchform.OrderPlant == null ? "" : dataForm.searchform.OrderPlant.ToString();
                string strHaoJiu = dataForm.searchform.HaoJiu == null ? "" : dataForm.searchform.HaoJiu.ToString();
                string strSupplierId = dataForm.searchform.SupplierId == null ? "" : dataForm.searchform.SupplierId.ToString();
                string strSupplierPlant = dataForm.searchform.SupplierPlant == null ? "" : dataForm.searchform.SupplierPlant.ToString();
                string strDataState = dataForm.searchform.DataState == null ? "" : dataForm.searchform.DataState.ToString();
                string strOverDue = dataForm.searchform.OverDue == null ? "" : dataForm.searchform.OverDue;
                string strExpectTime = dataForm.info;//期望回复日
                string strEmailBody = dataForm.mailboy;//邮件体
                string strReceiver = "APC06";

                JArray multipleInfo = dataForm.searchform.multipleSelection;
                List<Dictionary<string, Object>> multipleInfoData = multipleInfo.ToObject<List<Dictionary<string, Object>>>();
                DataTable dtMessage = fs0603_Logic.createTable("MES");

                if (strYearMonth == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "对象年月不能为空。";
                    dtMessage.Rows.Add(dataRow);
                }
                if (strExpectTime == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "期望回复日期不能为空。";
                    dtMessage.Rows.Add(dataRow);
                }
                if (strExpectTime != "" && Convert.ToDateTime(strExpectTime + " 23:59:59") < System.DateTime.Now)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "期望回复日期不能小于当前时间。";
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
                DataTable dtInfo = fs0602_Logic.getSearchInfo(strYearMonth, strDyState, strHyState, strPartId, strCarModel,
                   strInOut, strOrderingMethod, strOrderPlant, strHaoJiu, strSupplierId, strSupplierPlant, strDataState, strOverDue);
                DataTable dtImport = fs0602_Logic.checkopenplanInfo(multipleInfoData, dtInfo, strExpectTime,
                    loginInfo.UserId, loginInfo.UnitCode, strReceiver, ref dtMessage);
                if (dtMessage != null && dtMessage.Rows.Count != 0)
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
                string strTheme = "月度内示展开确认";
                DataTable dtToList = fs0602_Logic.getToList(dtImport, ref dtMessage);
                fs0602_Logic.sendEmailInfo(loginInfo.UserId, loginInfo.UserName, loginInfo.Email, strTheme, strEmailBody, dtToList, ref dtMessage);
                if (dtMessage.Rows.Count != 0)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "内示展开成功。";
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06PE0205", ex, loginInfo.UserId);
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
                string strDyState = dataForm.searchform.DyState == null ? "" : dataForm.searchform.DyState.ToString();
                string strHyState = dataForm.searchform.HyState == null ? "" : dataForm.searchform.HyState.ToString();
                string strPartId = dataForm.searchform.PartId == null ? "" : dataForm.searchform.PartId.ToString();
                string strCarModel = dataForm.searchform.CarModel == null ? "" : dataForm.searchform.CarModel.ToString();
                string strInOut = dataForm.searchform.InOut == null ? "" : dataForm.searchform.InOut.ToString();
                string strOrderingMethod = dataForm.searchform.OrderingMethod == null ? "" : dataForm.searchform.OrderingMethod.ToString();
                string strOrderPlant = dataForm.searchform.OrderPlant == null ? "" : dataForm.searchform.OrderPlant.ToString();
                string strHaoJiu = dataForm.searchform.HaoJiu == null ? "" : dataForm.searchform.HaoJiu.ToString();
                string strSupplierId = dataForm.searchform.SupplierId == null ? "" : dataForm.searchform.SupplierId.ToString();
                string strSupplierPlant = dataForm.searchform.SupplierPlant == null ? "" : dataForm.searchform.SupplierPlant.ToString();
                string strDataState = dataForm.searchform.DataState == null ? "" : dataForm.searchform.DataState.ToString();
                string strOverDue = dataForm.searchform.OverDue == null ? "" : dataForm.searchform.OverDue;

                string strExpectTime = dataForm.info;//期望回复日
                JArray checkedInfo = dataForm.searchform.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();

                DataTable dtMessage = fs0603_Logic.createTable("MES");
                DataTable dataTable = fs0602_Logic.getSearchInfo(strYearMonth, strDyState, strHyState, strPartId, strCarModel,
                   strInOut, strOrderingMethod, strOrderPlant, strHaoJiu, strSupplierId, strSupplierPlant, strDataState, strOverDue);
                string strReceiver = "APC06";
                DataTable dtImport = fs0602_Logic.checkreplyplanInfo(listInfoData, dataTable, strExpectTime,
                    loginInfo.UserId, loginInfo.UnitCode, strReceiver, ref dtMessage);
                if (dtImport == null && dtMessage != null && dtMessage.Rows.Count != 0)
                {
                    //弹出错误dtMessage
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                DataTable dtMessage1 = fs0603_Logic.createTable("MES");
                fs0602_Logic.replyPlan(dtImport, loginInfo.UserId, ref dtMessage1);
                if (dtMessage1 != null && dtMessage1.Rows.Count != 0)
                {
                    //弹出错误dtMessage
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage1;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "内士回复成功。";
                    dtMessage.Rows.InsertAt(dataRow, 0);
                    DataRow dataRow1 = dtMessage.NewRow();
                    dataRow1["vcMessage"] = "以下品番不能进行订购（原因请查看列表）";
                    dtMessage.Rows.InsertAt(dataRow1, 1);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06PE0206", ex, loginInfo.UserId);
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
                string strDyState = dataForm.searchform.DyState == null ? "" : dataForm.searchform.DyState.ToString();
                string strHyState = dataForm.searchform.HyState == null ? "" : dataForm.searchform.HyState.ToString();
                string strPartId = dataForm.searchform.PartId == null ? "" : dataForm.searchform.PartId.ToString();
                string strCarModel = dataForm.searchform.CarModel == null ? "" : dataForm.searchform.CarModel.ToString();
                string strInOut = dataForm.searchform.InOut == null ? "" : dataForm.searchform.InOut.ToString();
                string strOrderingMethod = dataForm.searchform.OrderingMethod == null ? "" : dataForm.searchform.OrderingMethod.ToString();
                string strOrderPlant = dataForm.searchform.OrderPlant == null ? "" : dataForm.searchform.OrderPlant.ToString();
                string strHaoJiu = dataForm.searchform.HaoJiu == null ? "" : dataForm.searchform.HaoJiu.ToString();
                string strSupplierId = dataForm.searchform.SupplierId == null ? "" : dataForm.searchform.SupplierId.ToString();
                string strSupplierPlant = dataForm.searchform.SupplierPlant == null ? "" : dataForm.searchform.SupplierPlant.ToString();
                string strDataState = dataForm.searchform.DataState == null ? "" : dataForm.searchform.DataState.ToString();
                string strOverDue = dataForm.searchform.OverDue == null ? "" : dataForm.searchform.OverDue;

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
                DataTable dataTable = fs0602_Logic.getSearchInfo(strYearMonth, "", "", "", "", "", "", "", "", "", "", "", "");
                if (dataTable.Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "该对象月没有有效的内示情报";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string strReceiver = "APC06";
                string strReturnym = fs0602_Logic.checkreturnplanInfo(listInfoData, dataTable, strExpectTime,
                     ref dtMessage);
                if (dtMessage != null && dtMessage.Rows.Count != 0)
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
                string strEmailBody = fs0602_Logic.setEmailBody();
                string strTheme = "月度内示回退";
                fs0602_Logic.sendEmailInfo_FTMS(loginInfo.UserId, loginInfo.UserName, loginInfo.Email, strTheme, strEmailBody, ref dtMessage);
                if (dtMessage.Rows.Count != 0)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "月度内示回退成功。";
                    dtMessage.Rows.InsertAt(dataRow, 0);
                    dtMessage = dtMessage.DefaultView.ToTable(true, "vcMessage");
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06PE0207", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "内示退回失败";
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
            string strExpectTime = dataForm.dExpectTime == null ? "" : dataForm.dExpectTime;
            string strFlag = dataForm.flag == null ? "" : dataForm.flag;
            try
            {
                String emailBody = fs0602_Logic.setEmailBody(strExpectTime, strFlag);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = emailBody;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06PE0208", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "邮件预览失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 下载报表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string exportRefApi([FromBody]dynamic data)
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
            try
            {
                string strDXYNum = dataForm.decDXYNum == null ? "" : dataForm.decDXYNum;
                string strNSYNum = dataForm.decNSYNum == null ? "" : dataForm.decNSYNum;
                string strNNSYNum = dataForm.decNNSYNum == null ? "" : dataForm.decNNSYNum;
                DateTime dNow = DateTime.Now.AddMonths(1);
                string strYearMonth = dNow.ToString("yyyyMM");
                DataTable dtMessage = fs0603_Logic.createTable("MES");
                if (strDXYNum == "" || strNSYNum == "" || strNNSYNum == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "稼动天数存在未填写的情况，请填写后再试。";
                    dtMessage.Rows.Add(dataRow);
                }
                if (strYearMonth == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "对象年月不能为空。";
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
                DataTable dtRef = fs0602_Logic.getExportRef(strYearMonth, strDXYNum, strNSYNum, strNNSYNum, ref dtMessage);
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                {
                    //弹出错误dtMessage
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string[] fields = {"vcExportDate","vcRefDate","vcMonth_dx","vcMonth_ns","vcMonth_nns","decDXYNum","decNSYNum","decNNSYNum",
                    "vcProject","decSOQ_dx","decSOQ_ns_before","decSOQ_ns","decSOQ_nns_before","decSOQ_nns","decNNA_dx","decNNA_ns","decNNA_nns"};

                string filepath = ComFunction.generateExcelWithXlt(dtRef, fields, _webHostEnvironment.ContentRootPath, "FTMS内示总结.xlsx", 1, 1, loginInfo.UserId, FunctionID);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06PE0209", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "报表下载失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
    }
}