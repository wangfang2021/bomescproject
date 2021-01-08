﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SPPSApi.Controllers.G03
{
    [Route("api/FS0307/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0307Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string FunctionID = "FS0307";
        FS0307_Logic fs0307_logic = new FS0307_Logic();

        public FS0307Controller(IWebHostEnvironment webHostEnvironment)
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

                List<Object> dataList_C005 = ComFunction.convertAllToResult(ComFunction.getTCode("C005"));//收货方
                List<Object> dataList_C006 = ComFunction.convertAllToResult(ComFunction.getTCode("C006"));//原单位
                List<Object> dataList_C016 = ComFunction.convertAllToResult(ComFunction.getTCode("C016"));//包装事业体
                List<Object> dataList_C024 = ComFunction.convertAllToResult(ComFunction.getTCode("C024"));//包装事业体

                res.Add("C005", dataList_C005);
                res.Add("C006", dataList_C006);
                res.Add("C016", dataList_C016);
                res.Add("C024", dataList_C024);

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

        #region 年限抽取

        [HttpPost]
        [EnableCors("any")]
        public string extractApi([FromBody]dynamic data)
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
            JArray listInfo = dataForm.vcOriginCompany;
            List<string> vcOriginCompany = listInfo.ToObject<List<string>>();
            try
            {
                fs0307_logic.extractPart(loginInfo.UserId, vcOriginCompany);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0703", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "年限抽取失败";
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

            string strYear = dataForm.strYear == null ? "" : dataForm.strYear;
            string FinishFlag = dataForm.FinishFlag == null ? "" : dataForm.FinishFlag; ;
            string SYT = dataForm.vcSYTCode == null ? "" : dataForm.vcSYTCode;
            string Receiver = dataForm.vcReceiver == null ? "" : dataForm.vcReceiver;
            JArray listInfo = dataForm.vcOriginCompany;
            List<string> origin = listInfo.ToObject<List<string>>();

            try
            {
                DataTable dt = fs0307_logic.searchApi(strYear, FinishFlag, SYT, Receiver, origin);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcOld10", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcOld9", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcOld7", ConvertFieldType.BoolType, null);

                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0701", ex, loginInfo.UserId);
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

            string strYear = dataForm.strYear;
            string FinishFlag = dataForm.FinishFlag;
            string SYT = dataForm.vcSYTCode;
            string Receiver = dataForm.vcReceiver;
            JArray listInfo = dataForm.vcOriginCompany;
            List<string> origin = listInfo.ToObject<List<string>>();
            try
            {
                DataTable dt = fs0307_logic.searchApi(strYear, FinishFlag, SYT, Receiver, origin);

                string[] fields = { "vcYear", "vcFinish", "dFinishYMD", "vcSupplier_id", "vcSYTCode", "vcReceiver", "vcOriginCompany", "vcPart_id", "vcPartNameEn", "vcInOutflag", "vcCarTypeDev", "dJiuBegin", "vcRemark", "vcOld10", "vcOld9", "vcOld7", "vcPM", "vcNum1", "vcNum2", "vcNum3", "vcNXQF", "dTimeFrom", "vcDY", "vcNum11", "vcNum12", "vcNum13", "vcNum14", "vcNum15", "vcNum16", "vcNum17", "vcNum18", "vcNum19", "vcNum20", "vcNum21" };
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0307.xlsx", 1, loginInfo.UserId, FunctionID);
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

        #region FTMS

        [HttpPost]
        [EnableCors("any")]
        public string FTMSApi([FromBody]dynamic data)
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

                if (listInfoData.Count > 0)
                {
                    hasFind = true;
                }

                if (!hasFind)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选中一条数据！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                fs0307_logic.FTMS(listInfoData, loginInfo.UserId);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0703", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "FTMS层别失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 保存



        #endregion





    }
}