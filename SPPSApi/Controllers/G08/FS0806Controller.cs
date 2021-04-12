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
    [Route("api/FS0806/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0806Controller : BaseController
    {
        FS0806_Logic fs0806_Logic = new FS0806_Logic();
        private readonly string FunctionID = "FS0806";

        private readonly IWebHostEnvironment _webHostEnvironment;

        public FS0806Controller(IWebHostEnvironment webHostEnvironment)
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

                List<Object> dataList_C022 = ComFunction.convertAllToResult(ComFunction.getTCode("C022"));//作业类型区分
                List<Object> dataList_C023 = ComFunction.convertAllToResult(ComFunction.getTCode("C023"));//包装场
                res.Add("C022", dataList_C022);
                res.Add("C023", dataList_C023);

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

            string vcZYType = dataForm.vcZYType;
            string vcBZPlant = dataForm.vcBZPlant;
            string vcInputNo = dataForm.vcInputNo;
            string vcKBOrderNo = dataForm.vcKBOrderNo;
            string vcKBLFNo = dataForm.vcKBLFNo;
            string vcSellNo = dataForm.vcSellNo;
            string vcPart_id = dataForm.vcPart_id;
            string vcBoxNo = dataForm.vcBoxNo;
            string dStart = dataForm.dStart;
            string dEnd = dataForm.dEnd;
            string vcLabelNo = dataForm.vcLabelNo;
            string vcStatus = dataForm.vcStatus;

            try
            {
                DataTable dt = fs0806_Logic.Search(vcZYType, vcBZPlant, vcInputNo, vcKBOrderNo, vcKBLFNo, vcSellNo, vcPart_id, vcBoxNo, dStart, dEnd, vcLabelNo,vcStatus);

                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("dStart", ConvertFieldType.DateType, "yyyy/MM/dd HH:mm:ss");
                dtConverter.addField("dEnd", ConvertFieldType.DateType, "yyyy/MM/dd HH:mm:ss");

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

            string vcZYType = dataForm.vcZYType;
            string vcBZPlant = dataForm.vcBZPlant;
            string vcInputNo = dataForm.vcInputNo;
            string vcKBOrderNo = dataForm.vcKBOrderNo;
            string vcKBLFNo = dataForm.vcKBLFNo;
            string vcSellNo = dataForm.vcSellNo;
            string vcPart_id = dataForm.vcPart_id;
            string vcBoxNo = dataForm.vcBoxNo;
            string dStart = dataForm.dStart;
            string dEnd = dataForm.dEnd;
            string vcLabelNo = dataForm.vcLabelNo;
            string vcStatus = dataForm.vcStatus;

            try
            {
                DataTable dt = fs0806_Logic.Search(vcZYType, vcBZPlant, vcInputNo, vcKBOrderNo, vcKBLFNo, vcSellNo, vcPart_id, vcBoxNo, dStart, dEnd, vcLabelNo,vcStatus);
                string[] heads = { "作业类型区分","包装场","入库单号","看板订单号","看板连番","品番","内外区分","供应商","供应商工区","开始时间","结束时间",
                    "数量","包装单位","收货方","受入","箱号","设备编号","检查区分","检查个数","检查状态","员工编号","员工姓名","标签ID起","标签ID止","解锁员",
                    "解锁时间","销售单号"
                };
                string[] fields = { "vcZYTypeName","vcBZPlantName","vcInputNo","vcKBOrderNo","vcKBLFNo","vcPart_id","vcIOType","vcSupplier_id","vcSupplierGQ",
                    "dStart","dEnd","iQuantity","vcBZUnit","vcSHF","vcSR","vcBoxNo","vcSheBeiNo","vcCheckType","iCheckNum","vcCheckStatus","vcOperatorID",
                    "vcUserName","vcLabelStart","vcLabelEnd","vcUnlocker","dUnlockTime","vcSellNo"
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
                        {"数量"},//中文字段名
                        {"iQuantity"},//英文字段名
                        {FieldCheck.Num},//数据类型校验
                        {"0"},//最大长度设定,不校验最大长度用0
                        {"1"},//最小长度设定,可以为空用0
                        {"12"},//前台显示列号，从0开始计算,注意有选择框的是0
                    };
                    List<Object> checkRes = ListChecker.validateList(listInfoData, strField, null, null, true, "FS0806");
                    if (checkRes != null)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = checkRes;
                        apiResult.flag = Convert.ToInt32(ERROR_FLAG.单元格定位提示);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    #endregion
                }
                //校验 数量>0、数量<上一层数量
                for (int i = 0; i < listInfo.Count; i++)
                {
                    int iQuantity_input = Convert.ToInt32(listInfo[i]["iQuantity"].ToString());
                    string vcPart_id = listInfo[i]["vcPart_id"].ToString();
                    string vcKBOrderNo = listInfo[i]["vcKBOrderNo"].ToString();
                    string vcKBLFNo = listInfo[i]["vcKBLFNo"].ToString();
                    string vcSR = listInfo[i]["vcSR"].ToString();
                    string vcZYType = listInfo[i]["vcZYType"].ToString();
                    //校验 数量> 0
                    if (iQuantity_input == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "数量不能为0！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    //校验 录入数量<上一层数量
                    if (fs0806_Logic.isQuantityOK(vcPart_id, vcKBOrderNo, vcKBLFNo, vcSR, vcZYType, iQuantity_input) == false)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "不能大于上一层数量！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }

                fs0806_Logic.Save(listInfoData, loginInfo.UserId);
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
                fs0806_Logic.Del(checkedInfoData, loginInfo.UserId);
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

        #region 子画面初始化
        [HttpPost]
        [EnableCors("any")]
        public string initSubApi([FromBody]dynamic data)
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
            string vcPart_id = dataForm.vcPart_id;
            string vcKBOrderNo = dataForm.vcKBOrderNo;
            string vcKBLFNo = dataForm.vcKBLFNo;
            string vcSR = dataForm.vcSR;

            try
            {
                DataTable dt = fs0806_Logic.initSubApi(vcPart_id,vcKBOrderNo,vcKBLFNo,vcSR);

                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("dOperatorTime", ConvertFieldType.DateType, "yyyy/MM/dd HH:mm");

                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0204", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "子页面初始化失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}
