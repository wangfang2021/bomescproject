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

namespace SPPSApi.Controllers.G15
{
    [Route("api/FS1502/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1502Controller : BaseController
    {
        FS1502_Logic fs1502_Logic = new FS1502_Logic();
        FS0810_Logic fs0810_Logic = new FS0810_Logic();
        private readonly string FunctionID = "FS1502";

        private readonly IWebHostEnvironment _webHostEnvironment;

        public FS1502Controller(IWebHostEnvironment webHostEnvironment)
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

                List<Object> dataList_C003 = ComFunction.convertAllToResult(fs0810_Logic.getTCode("C003"));//大品目
                res.Add("C003", dataList_C003);

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
            string dBZDate = dataForm.dBZDate == null ? "" : dataForm.dBZDate;
            string vcBigPM = dataForm.vcBigPM == null ? "" : dataForm.vcBigPM;
            try
            {
                if (dBZDate == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请选择包装日期！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                DataTable dt = fs1502_Logic.Search(dBZDate, vcBigPM);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);

                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M08UE1001", ex, loginInfo.UserId);
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
            string dBZDate = dataForm.dBZDate == null ? "" : dataForm.dBZDate;
            string vcBigPM = dataForm.vcBigPM == null ? "" : dataForm.vcBigPM;
            try
            {
                DataTable dt = fs1502_Logic.Search(dBZDate, vcBigPM);
                string[] fields = { "vcBigPM","vcSmallPM", "iBZPlan_Day", "iBZPlan_Night", "iBZPlan_Heji", "iEmergencyOrder", "iLJBZRemain"
                ,"iPlanTZ","iSSPlan_Day","iSSPlan_Night","iSSPlan_Heji"
                };
                string filepath = fs1502_Logic.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS1502_Export.xlsx", 2, loginInfo.UserId, FunctionID);

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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M08UE1002", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 报表
        [HttpPost]
        [EnableCors("any")]
        public string reportApi([FromBody] dynamic data)
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
            string dBZDate = dataForm.dBZDate == null ? "" : dataForm.dBZDate;
            try
            {
                //DataTable dt = fs1502_Logic.Search_report(dBZDate);
                //string[] fields = { "kind", "dt", "vcBigPM", "heji"};
                //string filepath = fs1502_Logic.generateExcelWithXlt_report(dt, fields, _webHostEnvironment.ContentRootPath, "FS1502_Report.xls", 1, loginInfo.UserId, FunctionID);
                DataTable dt = fs1502_Logic.Search_report(dBZDate);
                string[] heads = { "类别", "大品目", "小品目", "1日", "2日", "3日", "4日", "5日", "6日", "7日", "8日", "9日", "10日",
                "11日","12日","13日","14日","15日","16日","17日","18日","19日","20日","21日","22日","23日","24日","25日","26日","27日","28日","29日","30日","31日"};
                string[] fields = { "vcKind", "vcBigPM", "vcSmallPM", "iD1", "iD2", "iD3", "iD4", "iD5", "iD6", "iD7", "iD8", "iD9", "iD10",
                "iD11","iD12","iD13","iD14","iD15","iD16","iD17","iD18","iD19","iD20","iD21","iD22","iD23","iD24","iD25","iD26","iD27","iD28","iD29","iD30","iD31"};
                string strMsg = "";
                string filepath = ComFunction.DataTableToExcel(heads, fields, dt, _webHostEnvironment.ContentRootPath, loginInfo.UserId, FunctionID, ref strMsg);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M08UE1002", ex, loginInfo.UserId);
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
                        {"累积包装残","计划调整"},//中文字段名
                        {"iLJBZRemain","iPlanTZ"},//英文字段名
                        {FieldCheck.Num,FieldCheck.Num},//数据类型校验
                        {"0","0"},//最大长度设定,不校验最大长度用0
                        {"1","1"},//最小长度设定,可以为空用0
                        {"7","8"},//前台显示列号，从0开始计算,注意有选择框的是0
                    };
                    List<Object> checkRes = ListChecker.validateList(listInfoData, strField, null, null, true, "FS1502");
                    if (checkRes != null)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = checkRes;
                        apiResult.flag = Convert.ToInt32(ERROR_FLAG.单元格定位提示);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    #endregion
                }

                for (int i = 0; i < listInfoData.Count; i++)
                {
                    string dPackDate = Convert.ToDateTime(listInfoData[i]["dPackDate"].ToString()).ToString("yyyy-MM-dd");
                    string iLJBZRemain = listInfoData[i]["iLJBZRemain"] == null ? "0" : listInfoData[i]["iLJBZRemain"].ToString();
                    string iLJBZRemain_old = listInfoData[i]["iLJBZRemain_old"] == null ? "0": listInfoData[i]["iLJBZRemain_old"].ToString();
                    string time = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                    if (iLJBZRemain != iLJBZRemain_old && dPackDate != time)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "只有修改" + time + "的累计包装残！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }

                fs1502_Logic.Save(listInfoData, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M08UE1003", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 人变化
        [HttpPost]
        [EnableCors("any")]
        public string personchangeApi([FromBody]dynamic data)
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
                Dictionary<string, object> res = new Dictionary<string, object>();
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                string radio = dataForm.radio == null ? "" : dataForm.radio;
                JArray listInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();

                #region 根据人&小时计算
                string person = dataForm.person == null || dataForm.person == "" ? "0" : dataForm.person;
                string hour = dataForm.hour == null || dataForm.hour == "" ? "0" : dataForm.hour;

                decimal decperson = Convert.ToDecimal(person);
                decimal dechour = Convert.ToDecimal(hour);

                decimal min = decperson * 450 + decperson * (dechour * 60);
                #endregion

                #region 计算下面-上面的值
                string strMsg = "";
                decimal checkedvalue = 0;
                decimal resultvalue = fs1502_Logic.result(listInfoData, radio, min, ref strMsg,ref checkedvalue);
                if (strMsg != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = strMsg;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                #endregion

                res.Add("min", min.ToString());
                res.Add("resultvalue", resultvalue.ToString());
                res.Add("checkedvalue", checkedvalue.ToString());

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M08UE1004", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 小时变化
        [HttpPost]
        [EnableCors("any")]
        public string hourchangeApi([FromBody]dynamic data)
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
                Dictionary<string, object> res = new Dictionary<string, object>();
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                string radio = dataForm.radio == null ? "" : dataForm.radio;
                JArray listInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();

                #region 根据人&小时计算
                string person = dataForm.person == null || dataForm.person == "" ? "0" : dataForm.person;
                string hour = dataForm.hour == null || dataForm.hour == "" ? "0" : dataForm.hour;

                decimal decperson = Convert.ToDecimal(person);
                decimal dechour = Convert.ToDecimal(hour);

                decimal min = decperson * 450 + decperson * (dechour * 60);
                #endregion

                #region 计算下面-上面的值
                string strMsg = "";
                decimal checkedvalue = 0;
                decimal resultvalue = fs1502_Logic.result(listInfoData, radio, min, ref strMsg,ref checkedvalue);
                if (strMsg != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = strMsg;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                #endregion

                res.Add("min", min.ToString());
                res.Add("resultvalue", resultvalue.ToString());
                res.Add("checkedvalue", checkedvalue.ToString());

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M08UE1004", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 白夜变化
        [HttpPost]
        [EnableCors("any")]
        public string radiochangeApi([FromBody]dynamic data)
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
                Dictionary<string, object> res = new Dictionary<string, object>();
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                string radio = dataForm.radio == null ? "" : dataForm.radio;
                string minute = (dataForm.min == null || dataForm.min == "" )? "0" :dataForm.min;
                int iminute = Convert.ToInt32(minute);
                JArray listInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
                string strMsg = "";
                decimal checkedvalue = 0;
                decimal resultvalue = fs1502_Logic.result(listInfoData, radio, iminute, ref strMsg,ref checkedvalue);
                if (strMsg != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = strMsg;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                res.Add("resultvalue", resultvalue);
                res.Add("checkedvalue", checkedvalue);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M08UE1004", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 计算
        [HttpPost]
        [EnableCors("any")]
        public string calApi([FromBody]dynamic data)
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
                string dBZDate = dataForm.dBZDate == null ? "" : dataForm.dBZDate;
                if (dBZDate == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请选择包装日期！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fs1502_Logic.Cal(dBZDate, loginInfo.UserId, loginInfo.UnitCode);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M08UE1003", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}
