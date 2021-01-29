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
                //dtConverter.addField("dExpectTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                //dtConverter.addField("dSReplyTime", ConvertFieldType.DateType, "yyyy/MM/dd HH:mm:ss");
                //dtConverter.addField("dHyTime", ConvertFieldType.DateType, "yyyy/MM/dd HH:mm:ss");
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
                JArray listInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
                bool hasFind = false;//是否找到需要新增或者修改的数据
                List<string> lsYearMonth = new List<string>();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["bModFlag"];//true可编辑,false不可编辑
                    bool bAddFlag = (bool)listInfoData[i]["bAddFlag"];//true可编辑,false不可编辑
                    string ym = listInfoData[i]["vcYearMonth"].ToString();
                    ym = ym.Substring(0, 4) + "-" + ym.Substring(4, 2) + "-01";
                    if (bAddFlag == true)
                    {//新增
                        hasFind = true;
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        hasFind = true;
                        if (!lsYearMonth.Contains(ym))
                        {
                            lsYearMonth.Add(ym);
                        }
                    }
                }
                string strYearMonth = "";
                string strYearMonth_2 = "";
                string strYearMonth_3 = "";
                string strMsg = "";
                if (!hasFind)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少有一个编辑行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                else
                {
                    strYearMonth = Convert.ToDateTime(lsYearMonth[0].ToString()).ToString("yyyyMM");
                    strYearMonth_2 = Convert.ToDateTime(lsYearMonth[0].ToString()).AddMonths(1).ToString("yyyyMM");
                    strYearMonth_3 = Convert.ToDateTime(lsYearMonth[0].ToString()).AddMonths(2).ToString("yyyyMM");
                    if (fs0602_Logic.IsDQR(strYearMonth, listInfoData, ref strMsg, "save"))
                    {//全是可操作的数据
                        //继续向下执行
                    }
                    else
                    {//有不可以操作的数据
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "以下品番不可操作：" + strMsg;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                //开始数据验证
                if (hasFind)
                {
                    #region 数据格式校验
                    string[,] strField = new string[,]
                    {
                        {"N月","N+1月","N+2月"},//中文字段名
                        {"iTzhSOQN","iTzhSOQN1","iTzhSOQN2"},//英文字段名
                        {FieldCheck.Num,FieldCheck.Num,FieldCheck.Num},//数据类型校验
                        {"0","0","0"},//最大长度设定,不校验最大长度用0
                        {"1","1","1"},//最小长度设定,可以为空用0
                        {"9","10","11"},//前台显示列号，从0开始计算,注意有选择框的是0
                    };
                    List<Object> checkRes = ListChecker.validateList(listInfoData, strField, null, null, true, "FS0602");
                    if (checkRes != null)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = checkRes;
                        apiResult.flag = Convert.ToInt32(ERROR_FLAG.单元格定位提示);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    #endregion
                }

                List<string> errMessageList = new List<string>();//记录导入错误消息
                fs0602_Logic.SaveCheck(listInfoData, loginInfo.UserId, strYearMonth, strYearMonth_2, strYearMonth_3, ref errMessageList, loginInfo.UnitCode);
                if (errMessageList.Count > 0)
                {
                    fs0501_Logic.importHistory(strYearMonth, errMessageList, loginInfo.UserId);
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "发现问题数据，导入终止，请查看导入履历。";
                    apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fs0602_Logic.importSave(strYearMonth, loginInfo.UserId, loginInfo.UnitCode);
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

                string strYearMonth = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth + "/01").ToString("yyyyMM");
                JArray checkedInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();

                if (strYearMonth == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "对象年月不能为空";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string strMsg = "";
                if (listInfoData.Count == 0)//选中了数据操作
                {
                    listInfoData = null;
                }
                fs0602_Logic.replyPlan(listInfoData, dataForm, loginInfo.UserId, ref strMsg);
                if (strMsg == "")
                {
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.data = null;
                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = strMsg;
                }
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M04UE0203", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "";
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

                string strYearMonth = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth + "/01").ToString("yyyyMM");
                string strExpectTime = dataForm.ExpectTime == null ? "" : Convert.ToDateTime(dataForm.ExpectTime).ToString("yyyy-MM-dd");
                JArray checkedInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();

                if (strYearMonth == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "对象年月不能为空";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (strExpectTime == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "纳期不能为空";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string strMsg = "";
                if (listInfoData.Count == 0)//选中了数据操作
                {
                    listInfoData = null;
                }
                fs0602_Logic.openPlan(listInfoData, dataForm, strExpectTime, loginInfo, loginInfo.UserId, ref strMsg);
                if (strMsg == "")
                {
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.data = null;
                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = strMsg;
                }
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M04UE0203", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "";
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

                string strYearMonth = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth + "/01").ToString("yyyyMM");

                if (strYearMonth == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "对象年月不能为空";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string strMsg = "";
                fs0602_Logic.returnPlan(dataForm, loginInfo.UserId, ref strMsg);
                if (strMsg == "")
                {
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.data = null;
                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = strMsg;
                }
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M04UE0203", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 错误信息下载
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string errMsgDownApi([FromBody] dynamic data)
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
            try
            {
                DataTable dt = fs0501_Logic.SearchHistory(strYearMonth, loginInfo.UserId);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M08UE0703", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
    }
}