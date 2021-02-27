using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;

namespace SPPSApi.Controllers.G05
{
    [Route("api/FS0501/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0501Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string FunctionID = "FS0501";
        FS0501_Logic fs0501_Logic = new FS0501_Logic();

        public FS0501Controller(IWebHostEnvironment webHostEnvironment)
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
                List<Object> dataList_C036 = ComFunction.convertAllToResult(fs0501_Logic.getTCode("C036"));//对应状态
                List<Object> dataList_C052 = ComFunction.convertAllToResult(ComFunction.getTCode("C052"));//操作状态
                List<Object> dataList_WorkList = ComFunction.convertAllToResult(fs0501_Logic.getWorkArea(loginInfo.UserId));//供应商工区
                res.Add("C036", dataList_C036);
                res.Add("C052", dataList_C052);
                res.Add("WorkArea", dataList_WorkList);
                DateTime dNow = DateTime.Now.AddMonths(1);
                res.Add("yearMonth", dNow.ToString("yyyy/MM"));
                res.Add("vcSupplier_id", loginInfo.UserId);

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
        #endregion

        #region 检索数据
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

            string strYearMonth = dataForm.YearMonth==null?"": Convert.ToDateTime(dataForm.YearMonth+"/01").ToString("yyyyMM");
            string strSupplier_id = dataForm.vcSupplier_id == null ? "" : dataForm.vcSupplier_id;
            string strPart_id = dataForm.Part_id == null ? "" : dataForm.Part_id;
            string strDyState = dataForm.DyState == null?"": dataForm.DyState;
            string strOperState= dataForm.OperateState == null ? "" : dataForm.OperateState;
            string strWorkArea= dataForm.vcWorkArea == null ? "" : dataForm.vcWorkArea;

            try
            {
                Dictionary<string, object> res = new Dictionary<string, object>();
                int num = 0;
                DataTable dt = fs0501_Logic.Search(strYearMonth, strSupplier_id, strPart_id, strDyState, strOperState,strWorkArea,ref num);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("dExpectTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dOpenTime", ConvertFieldType.DateType, "yyyy/MM/dd HH:mm:ss");
                dtConverter.addField("dSReplyTime", ConvertFieldType.DateType, "yyyy/MM/dd HH:mm:ss");
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);

                res.Add("dataList", dataList);
                res.Add("isShowError", num > 0 ? true : false);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;

                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M04UE0202", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 提交
        [HttpPost]
        [EnableCors("any")]
        public string okApi([FromBody] dynamic data)
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
                string strSupplier_id = dataForm.vcSupplier_id == null ? "" : dataForm.vcSupplier_id;
                string strPart_id = dataForm.Part_id == null ? "" : dataForm.Part_id;
                string strDyState = dataForm.DyState == null ? "" : dataForm.DyState;
                string strOperState = dataForm.OperateState == null ? "" : dataForm.OperateState;
                string strWorkArea = dataForm.vcWorkArea == null ? "" : dataForm.vcWorkArea;
                JArray checkedInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();

                if (strYearMonth == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "对象年月不能为空";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                int count = 0;//影响行数，没啥用
                string strMsg = "";
                if (listInfoData.Count != 0)//选中了数据操作
                {
                    if (fs0501_Logic.IsDQR(strYearMonth,strSupplier_id, listInfoData, ref strMsg,"submit"))
                    {//全是可操作的数据
                        // 执行提交操作：按所选数据提交
                        fs0501_Logic.ok(strYearMonth, listInfoData, loginInfo.UserId);
                    }
                    else
                    {//有不可以操作的数据
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "以下品番不可操作：" + strMsg;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                else//按检索条件
                {
                    if (fs0501_Logic.IsDQR(strYearMonth, strSupplier_id, strPart_id, strDyState, strOperState,strWorkArea, ref strMsg))
                    {//全是可操作的数据
                        //执行提交操作：按检索条件提交
                        fs0501_Logic.ok(strYearMonth, strSupplier_id, strPart_id, strDyState, strOperState,strWorkArea, loginInfo.UserId);
                    }
                    else
                    {//有不可以操作的数据
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "以下品番不可操作：" + strMsg;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = count;
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

            //string strYearMonth = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth).ToString("yyyyMM");
            string strYearMonth_2 = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth).AddMonths(1).ToString("yyyyMM");
            string strYearMonth_3 = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth).AddMonths(2).ToString("yyyyMM");

            string strYearMonth = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth + "/01").ToString("yyyyMM");
            string strSupplier_id = dataForm.vcSupplier_id == null ? "" : dataForm.vcSupplier_id;
            string strPart_id = dataForm.Part_id == null ? "" : dataForm.Part_id;
            string strDyState = dataForm.DyState == null ? "" : dataForm.DyState;
            string strOperState = dataForm.OperateState == null ? "" : dataForm.OperateState;
            string strWorkArea = dataForm.vcWorkArea == null ? "" : dataForm.vcWorkArea;

            try
            {
                int num = 0;
                DataTable dt = fs0501_Logic.Search(strYearMonth, strSupplier_id, strPart_id, strDyState, strOperState, strWorkArea,ref num);
                string[] fields = { "vcYearMonth", "dExpectTime", "vcDyState_Name", "vcPart_id", "iQuantityPercontainer", "iCbSOQN"
                ,"iCbSOQN1","iCbSOQN2","iTzhSOQN","iTzhSOQN1","iTzhSOQN2","dOpenTime","dSReplyTime"
                };
                string filepath = fs0501_Logic.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0501_Export.xlsx", 2, loginInfo.UserId, FunctionID, strYearMonth, strYearMonth_2, strYearMonth_3);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M04UE0205", ex, loginInfo.UserId);
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
                string strSupplier_id = dataForm.vcSupplier_id == null ? "" : dataForm.vcSupplier_id;
                JArray listInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
                bool hasFind = false;//是否找到需要新增或者修改的数据
                List<string> lsYearMonth = new List<string>();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    string ym = listInfoData[i]["vcYearMonth"].ToString();
                    ym = ym.Substring(0, 4) + "-" + ym.Substring(4, 2) + "-01";
                    if (bAddFlag == true)
                    {//新增
                        hasFind = true;
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        hasFind = true;
                        if(!lsYearMonth.Contains(ym))
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
                    if (fs0501_Logic.IsDQR(strYearMonth,strSupplier_id, listInfoData, ref strMsg,"save"))
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
                    List<Object> checkRes = ListChecker.validateList(listInfoData, strField, null, null, true, "FS0501");
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
                fs0501_Logic.SaveCheck(listInfoData, loginInfo.UserId, strYearMonth, strYearMonth_2, strYearMonth_3, ref errMessageList, loginInfo.UnitCode);
                if (errMessageList.Count > 0)
                {
                    fs0501_Logic.importHistory(strYearMonth, errMessageList, loginInfo.UserId);
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "发现问题数据，导入终止，请查看导入履历。";
                    apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fs0501_Logic.importSave(strYearMonth, loginInfo.UserId, loginInfo.UnitCode);
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

        #region 错误信息下载
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
                DataTable dt = fs0501_Logic.SearchHistory(strYearMonth,loginInfo.UserId);
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
        #endregion

    }
}