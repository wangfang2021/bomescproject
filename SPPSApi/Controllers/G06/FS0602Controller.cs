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

            try
            {
                DataTable dataTable = fs0602_Logic.getSearchInfo(strYearMonth, strDyState, strHyState, strPartId, strCarModel,
                    strInOut, strOrderingMethod, strOrderPlant, strHaoJiu, strSupplierId, strSupplierPlant);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("bModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("bAddFlag", ConvertFieldType.BoolType, null);
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
            try
            {
                string strYearMonth = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth).ToString("yyyyMM");
                string strYearMonth_2 = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth).AddMonths(1).ToString("yyyyMM");
                string strYearMonth_3 = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth).AddMonths(2).ToString("yyyyMM");
                DataTable dataTable = fs0602_Logic.getSearchInfo(strYearMonth, strDyState, strHyState, strPartId, strCarModel,
                     strInOut, strOrderingMethod, strOrderPlant, strHaoJiu, strSupplierId, strSupplierPlant);

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
                    if (bModFlag == true)
                        hasFind = true;//修改
                }
                if (!hasFind)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
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

                string strErrorPartId = "";
                //fs0603_Logic.setSPInfo(listInfoData, loginInfo.UserId, ref strErrorPartId);
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
                fs0602_Logic.openPlan(listInfoData, dataForm, strExpectTime, loginInfo.UserId, ref strMsg);
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
    }
}