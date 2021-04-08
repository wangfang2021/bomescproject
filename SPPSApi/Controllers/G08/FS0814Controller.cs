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



namespace SPPSApi.Controllers.G08
{
    [Route("api/FS0814/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0814Controller : BaseController
    {
        FS0814_Logic fs0814_Logic = new FS0814_Logic();
        private readonly string FunctionID = "FS0814";

        private readonly IWebHostEnvironment _webHostEnvironment;

        public FS0814Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

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
            string strYearMonth = dataForm.vcYearMonth == null ? "" : dataForm.vcYearMonth;

            try
            {
                DataTable dt = fs0814_Logic.Search(strYearMonth);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0204", ex, loginInfo.UserId);
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
            string strYearMonth = dataForm.vcYearMonth == null ? "" : dataForm.vcYearMonth;

            try
            {
                DataTable dt = fs0814_Logic.Search(strYearMonth);
                string[] heads = { "对象年月", "白夜", "D1", "D2", "D3","D4","D5","D6","D7","D8","D9","D10",
                "D11","D12","D13","D14","D15","D16","D17","D18","D19","D20",
                "D21","D22","D23","D24","D25","D26","D27","D28","D29","D30","D31"};
                string[] fields = { "vcYearMonth", "vcType", "vcD1", "vcD2", "vcD3","vcD4","vcD5","vcD6","vcD7","vcD8","vcD9","vcD10", 
                "vcD11","vcD12","vcD13","vcD14","vcD15","vcD16","vcD17","vcD18","vcD19","vcD20",
                "vcD21","vcD22","vcD23","vcD24","vcD25","vcD26","vcD27","vcD28","vcD29","vcD30","vcD31"};
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
                    string[,] strField = new string[,] {
                    {"对象年月","白夜","D1","D2","D3","D4","D5","D6","D7","D8","D9","D10",
                    "D11","D12","D13","D14","D15","D16","D17","D18","D19","D20",
                    "D21","D22","D23","D24","D25","D26","D27","D28","D29","D30","D31"},
                    {"vcYearMonth","vcType","vcD1", "vcD2","vcD3","vcD4","vcD5","vcD6","vcD7","vcD8","vcD9","vcD10",
                    "vcD11","vcD12","vcD13","vcD14","vcD15","vcD16","vcD17","vcD18","vcD19","vcD20",
                    "vcD21","vcD22","vcD23","vcD24","vcD25","vcD26","vcD27","vcD28","vcD29","vcD30","vcD31"},
                    {FieldCheck.Num,"",FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,
                    FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,
                    FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char},
                    {"6","1","1","1","1","1","1","1","1","1","1","1"
                    ,"1","1","1","1","1","1","1","1","1","1"
                    ,"1","1","1","1","1","1","1","1","1","1","1"},//最大长度设定,不校验最大长度用0
                    {"1","1","0","0","0","0","0","0","0","0","0","0"
                    ,"0","0","0","0","0","0","0","0","0","0"
                    ,"0","0","0","0","0","0","0","0","0","0","0"},//最小长度设定,可以为空用0
                    {"1","2","3","4","5","6","7","8","9","10","11","12"
                    ,"13","14","15","16","17","18","19","20","21","22"
                    ,"23","24","25","26","27","28","29","30","31","32","33"}
                    };//位置
                    List<Object> checkRes = ListChecker.validateList(listInfoData, strField, null, null, true, "FS0814");
                    if (checkRes != null)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = checkRes;
                        apiResult.flag = Convert.ToInt32(ERROR_FLAG.单元格定位提示);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    #endregion
                }
                #region 文件内容校验
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    for(int j=1;j<=31;j++)
                    {
                        string day = listInfoData[i]["vcD"+j+""].ToString();
                        if (day != "" && day != null && Array.IndexOf(new string[] { "A", "B" }, day) == -1)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "[D"+j+"]列只能填写A/B";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                    }
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    if (bAddFlag == true)
                    {//新增 
                     //校验 对象年月+白夜 不能重复
                        string vcYearMonth = listInfoData[i]["vcYearMonth"].ToString();
                        string vcType = listInfoData[i]["vcType"].ToString();
                        if (vcType != "" && vcType != null && Array.IndexOf(new string[] { "白", "夜" }, vcType) == -1)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "[白夜]列只能填写白/夜";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        bool isRepeat = fs0814_Logic.RepeatCheck(vcYearMonth, vcType);
                        if (isRepeat)
                        {//有重复数据  
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = string.Format("保存失败，有重复数据[对象年月-白夜]：{0}-{1}",vcYearMonth,vcType);
                            apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                    }
                }
                #endregion

                string strErrorName = "";
                fs0814_Logic.Save(listInfoData, loginInfo.UserId,ref strErrorName);
                if(strErrorName!="")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "以下年月白夜出现相同班值情况：" + strErrorName;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
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
                fs0814_Logic.Del(checkedInfoData, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
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
    }
}
