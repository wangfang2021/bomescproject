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
    [Route("api/FS0631/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0631Controller : BaseController
    {
        FS0631_Logic fs0631_Logic = new FS0631_Logic();
        private readonly string FunctionID = "FS0631";

        private readonly IWebHostEnvironment _webHostEnvironment;

        public FS0631Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        #region 页面初始化
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
                //if (loginInfo.Special == "财务用户")
                //    res.Add("caiWuBtnVisible", false);
                //else
                //    res.Add("caiWuBtnVisible", true);

                List<Object> dataList_C000 = ComFunction.convertAllToResult(fs0631_Logic.getTCode("C000",loginInfo.UnitCode));//工厂
                res.Add("C000", dataList_C000);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M08UE0701", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "初始化失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 检索
        [HttpPost]
        [EnableCors("any")]
        public string searchApi([FromBody]dynamic data)
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
            string strCLYM = dataForm.Process_YYYYMM;
            string strDXYM = dataForm.Start_date_for_daily_qty;
            string strPartNo = dataForm.Part_No;
            try
            {
                DataTable dt = fs0631_Logic.SearchNQCResult(strCLYM, strDXYM, strPartNo);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("Process_YYYYMM", ConvertFieldType.DateType, "yyyy/MM");
                dtConverter.addField("Start_date_for_daily_qty", ConvertFieldType.DateType, "yyyy/MM");

                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M08UE0702", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 导出
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
            string strCLYM = dataForm.Process_YYYYMM;
            string strDXYM = dataForm.Start_date_for_daily_qty;
            string strPartNo = dataForm.Part_No;
            try
            {
                DataTable dt = fs0631_Logic.SearchNQCResult(strCLYM, strDXYM, strPartNo);
                string[] heads = { "品番", "处理年月", "对象年月","工厂","D1","D2","D3","D4","D5","D6","D7","D8","D9","D10","D11","D12",
                    "D13","D14","D15","D16" ,"D17","D18","D19","D20","D21","D22","D23","D24","D25","D26","D27","D28","D29","D30","D31"
                };
                string[] fields = { "Part_No_Disp","vcCLYM","vcDXYM","Process_Factory","Daily_Qty_01","Daily_Qty_02","Daily_Qty_03","Daily_Qty_04",
                    "Daily_Qty_05","Daily_Qty_06","Daily_Qty_07","Daily_Qty_08","Daily_Qty_09","Daily_Qty_10","Daily_Qty_11","Daily_Qty_12",
                    "Daily_Qty_13","Daily_Qty_14","Daily_Qty_15","Daily_Qty_16","Daily_Qty_17","Daily_Qty_18","Daily_Qty_19","Daily_Qty_20",
                    "Daily_Qty_21","Daily_Qty_22","Daily_Qty_23","Daily_Qty_24","Daily_Qty_25","Daily_Qty_26","Daily_Qty_27","Daily_Qty_28",
                    "Daily_Qty_29","Daily_Qty_30","Daily_Qty_31"
                };
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
        #endregion

        #region 保存
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
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    if (bAddFlag == true)
                    {//新增
                        hasFind = true;
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        hasFind = true;
                    }
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
                    #region 数据格式校验
                    string[,] strField = new string[,]
                    {
                        {"品番", "处理年月", "对象年月","工厂","D1","D2","D3","D4",
                         "D5","D6","D7","D8","D9","D10","D11","D12",
                         "D13","D14","D15","D16" ,"D17","D18","D19","D20",
                         "D21","D22","D23","D24","D25","D26","D27","D28",
                         "D29","D30","D31"},//中文字段名
                        {"Part_No_Disp","Process_YYYYMM","Start_date_for_daily_qty","Process_Factory","Daily_Qty_01","Daily_Qty_02","Daily_Qty_03","Daily_Qty_04",
                         "Daily_Qty_05","Daily_Qty_06","Daily_Qty_07","Daily_Qty_08","Daily_Qty_09","Daily_Qty_10","Daily_Qty_11","Daily_Qty_12",
                         "Daily_Qty_13","Daily_Qty_14","Daily_Qty_15","Daily_Qty_16","Daily_Qty_17","Daily_Qty_18","Daily_Qty_19","Daily_Qty_20",
                         "Daily_Qty_21","Daily_Qty_22","Daily_Qty_23","Daily_Qty_24","Daily_Qty_25","Daily_Qty_26","Daily_Qty_27","Daily_Qty_28",
                         "Daily_Qty_29","Daily_Qty_30","Daily_Qty_31"},//英文字段名
                        {FieldCheck.NumChar,FieldCheck.Date,FieldCheck.Date,FieldCheck.NumChar,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num, FieldCheck.Num,
                         FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,
                         FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,
                         FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,
                         FieldCheck.Num,FieldCheck.Num,FieldCheck.Num},//数据类型校验
                        {"20","0","0","5","9","9","9","9",
                         "9","9","9","9","9","9","9","9",
                         "9","9","9","9","9","9","9","9",
                         "9","9","9","9","9","9","9","9",
                         "9","9","9"},//最大长度设定,不校验最大长度用0
                        {"14","1","1","1","0","0","0","0",
                         "0","0","0","0","0","0","0","0",
                         "0","0","0","0","0","0","0","0",
                         "0","0","0","0","0","0","0","0",
                         "0","0","0"},//最小长度设定,可以为空用0
                        {"1","2","3","4","5","6","7",
                         "8","9","10","11","12","13","14","15",
                         "16","17","18","19","20","21","22","23",
                         "24","25","26","27","28","29","30","31",
                         "32","33","34","35"},//前台显示列号，从0开始计算,注意有选择框的是0
                    };
                    List<Object> checkRes = ListChecker.validateList(listInfoData, strField, null, null, true, "FS0631");
                    if (checkRes != null)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = checkRes;
                        apiResult.flag = Convert.ToInt32(ERROR_FLAG.单元格定位提示);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    #endregion
                }
                fs0631_Logic.SaveNQCResult(listInfoData, loginInfo.UserId);
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
        #endregion

        #region 删除
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
                List<Dictionary<string, Object>> checkedInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();
                if (checkedInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选择一行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fs0631_Logic.DelNQCResult(checkedInfoData, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M08UE0705", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}
