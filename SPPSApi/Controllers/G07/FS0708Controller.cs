using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
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

namespace SPPSApi.Controllers.G07
{
    [Route("api/FS0708/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0708Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0708_Logic FS0708_Logic = new FS0708_Logic();
        private readonly string FunctionID = "FS0708";

        public FS0708Controller(IWebHostEnvironment webHostEnvironment)
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

                List<Object> dataList_C023 = ComFunction.convertAllToResult(ComFunction.getTCode("C023"));//包装场
                res.Add("C023", dataList_C023);

                List<Object> dataList_Supplier = ComFunction.convertAllToResult(FS0708_Logic.SearchSupplier());//供应商
                res.Add("optionSupplier", dataList_Supplier);

                List<Object> dataList_OrderStateData = ComFunction.convertAllToResult(ComFunction.getTCode("C063"));//纳入状态
                res.Add("OrderStateData", dataList_OrderStateData);



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

        #region 检索
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


            string PackSpot = dataForm.PackSpot;
            string OrderFrom = dataForm.OrderFrom;
            string OrderTo = dataForm.OrderTo;
            string PackNo = dataForm.PackNo;
            string PackGPSNo = dataForm.PackGPSNo;
            string Type = dataForm.Type;
            List<Object> OrderState = new List<object>();

            OrderState = dataForm.OrderStateData.ToObject<List<Object>>();

            string IsQianPin = dataForm.IsQianPin;
            string SupplierName = dataForm.SupplierName;
            string ZuCode = dataForm.ZuCode;
            string dFaZhuFrom = dataForm.dFaZhuFrom;
            string dFaZhuTo = dataForm.dFaZhuTo;
            string dNaQiFrom = dataForm.dNaQiFrom;
            string dNaQiTo = dataForm.dNaQiTo;
            string dNaRuFrom = dataForm.dNaRuFrom;
            string dNaRuTo = dataForm.dNaRuTo;


            try
            {
                DataTable dt = FS0708_Logic.Search(PackSpot, PackNo, PackGPSNo, OrderFrom, OrderTo, Type, OrderState, IsQianPin, SupplierName, ZuCode, dFaZhuFrom, dFaZhuTo, dNaQiFrom, dNaQiTo, dNaRuFrom, dNaRuTo);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("dUseBegin", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dUseEnd", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dProjectBegin", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dProjectEnd", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dJiuBegin", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dJiuEnd", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dJiuBeginSustain", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dPriceStateDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dPricebegin", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dPriceEnd", ConvertFieldType.DateType, "yyyy/MM/dd");
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);

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

            string PackSpot = dataForm.PackSpot;
            string OrderFrom = dataForm.OrderFrom;
            string OrderTo = dataForm.OrderTo;
            string PackNo = dataForm.PackNo;
            string PackGPSNo = dataForm.PackGPSNo;
            string Type = dataForm.Type;
            List<Object> OrderState = dataForm.OrderStateData.ToObject<List<Object>>();
            string IsQianPin = dataForm.IsQianPin;
            string SupplierName = dataForm.SupplierName;
            string ZuCode = dataForm.ZuCode;
            string dFaZhuFrom = dataForm.dFaZhuFrom;
            string dFaZhuTo = dataForm.dFaZhuTo;
            string dNaQiFrom = dataForm.dNaQiFrom;
            string dNaQiTo = dataForm.dNaQiTo;
            string dNaRuFrom = dataForm.dNaRuFrom;
            string dNaRuTo = dataForm.dNaRuTo;

            try
            {
                DataTable dt = FS0708_Logic.Search(PackSpot, PackNo, PackGPSNo, OrderFrom, OrderTo, Type, OrderState, IsQianPin, SupplierName, ZuCode, dFaZhuFrom, dFaZhuTo, dNaQiFrom, dNaQiTo, dNaRuFrom, dNaRuTo);

                if (dt.Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "没有可导出数据！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }
                string[] fields = { "vcPackSpot","vcOrderNo","vcPackNo","vcPackGPSNo", "iOrderNumber","iSJNumber","vcType",
                    "dFaZhuTime","dNaRuYuDing","vcNaRuBianCi","dNaRuShiJi","vcState","vcPackSupplierID","vcPackSupplierName",
                    "vcFeiYongName","vcZuCode"
                };
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0708_Export.xlsx", 1, loginInfo.UserId, "包装材基础数据");
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
        #endregion
        //导出列值应该显示名字

        #region 保存
        [HttpPost]
        [EnableCors("any")]
        public string saveApi([FromBody] dynamic data)
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
                    Regex regex = new System.Text.RegularExpressions.Regex("^(-?[0-9]*[.]*[0-9]{0,3})$");
                    bool b = regex.IsMatch(listInfoData[i]["iRelease"].ToString());
                    if (!b)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "请填写正常的发住收容数格式！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                if (!hasFind)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少有一个编辑行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                string strErrorPartId = "";
                FS0708_Logic.Save(listInfoData, loginInfo.UserId, ref strErrorPartId);
                if (strErrorPartId != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "保存失败";
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
        #endregion

        #region 删除
        [HttpPost]
        [EnableCors("any")]
        public string delApi([FromBody] dynamic data)
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
                FS0708_Logic.Del(listInfoData, loginInfo.UserId);
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
        #endregion


    }
}
