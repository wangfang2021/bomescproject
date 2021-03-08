﻿using System;
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
    [Route("api/FS0703/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0703Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0703_Logic FS0703_Logic = new FS0703_Logic();
        private readonly string FunctionID = "FS0703";

        public FS0703Controller(IWebHostEnvironment webHostEnvironment)
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
                FS0701_Logic FS0701_Logic = new FS0701_Logic();

                List<Object> dataList_Supplier = ComFunction.convertAllToResult(FS0701_Logic.SearchSupplier());//供应商
                res.Add("optionSupplier", dataList_Supplier);

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

        #region 计算并插入
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

            string PackSpot = dataForm.PackSpot;//包装厂
            string PackFrom = dataForm.dFrom;//对象年月

            List<Object> SupplierCodeList = new List<object>();

            SupplierCodeList = dataForm.vcSupplierCode.ToObject<List<Object>>();


            if (string.IsNullOrEmpty(PackSpot) || string.IsNullOrEmpty(PackFrom) || SupplierCodeList == null)
            {
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "请填写必要项！";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }

            try
            {

                DataTable dt = FS0703_Logic.Calculation(PackSpot, PackFrom, SupplierCodeList);
                string strErrorPartId = "";
                if (dt.Rows.Count > 0)
                {
                    FS0703_Logic.Save_GS(dt, loginInfo.UserId, ref strErrorPartId);
                }

                DataTable dtcheck = FS0703_Logic.SearchCheck();

                for (int i = 0; i < dtcheck.Rows.Count; i++)
                {
                    if (string.IsNullOrEmpty(dtcheck.Rows[i]["vcPackNo"].ToString()))
                    {
                        FS0703_Logic.InsertCheck(dtcheck.Rows[i]["vcpart_id"].ToString(), loginInfo.UserId, dtcheck.Rows[i]["vcPackNo"].ToString() + "没有维护包材品番信息");
                    }
                    if (string.IsNullOrEmpty(dtcheck.Rows[i]["vcCycle"].ToString()))
                    {
                        FS0703_Logic.InsertCheck(dtcheck.Rows[i]["vcpart_id"].ToString(), loginInfo.UserId, dtcheck.Rows[i]["vcPackNo"].ToString() + "没有维护纳入周期");
                    }
                    if (string.IsNullOrEmpty(dtcheck.Rows[i]["iRelease"].ToString()))
                    {
                        FS0703_Logic.InsertCheck(dtcheck.Rows[i]["vcpart_id"].ToString(), loginInfo.UserId, dtcheck.Rows[i]["vcPackNo"].ToString() + "没有维护纳入单位");
                    }

                }
                //DataTable dtE = FS0703_Logic.SearchExceptionCK();
                //List<Object> dataList = ComFunction.convertAllToResult(dtE);
                Dictionary<string, object> res = new Dictionary<string, object>();
                List<Object> vcException = ComFunction.convertAllToResult(FS0703_Logic.SearchExceptionCK());//
                res.Add("strException", vcException);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "计算失败";
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


            string PackSpot = dataForm.PackSpot;//包装厂
            string PackFrom = dataForm.dFrom;//对象年月
            List<Object> SupplierCodeList = new List<object>();

            SupplierCodeList = dataForm.vcSupplierCode.ToObject<List<Object>>();


            try
            {
                DataTable dt = new DataTable();
                dt=FS0703_Logic.Search(PackSpot, PackFrom, SupplierCodeList);

                if (dt.Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "没有可导出数据！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }
                string[] fields = { "vcYearMonth","vcPackNo","vcPackSpot","vcSupplierCode","vcSupplierWork","vcSupplierName","vcSupplierPack",
                    "vcCycle","iRelease","iDayNNum","iDayN1Num","iDayN2Num",
                    "iDay1","iDay2","iDay3","iDay4","iDay5","iDay6","iDay7","iDay8","iDay9","iDay10",
                    "iDay11","iDay12","iDay13","iDay14","iDay15","iDay16","iDay17","iDay18","iDay19","iDay20",
                    "iDay21","iDay22","iDay23","iDay24","iDay25","iDay26","iDay27","iDay28","iDay29","iDay30",
                    "iDay31","dZYTime"
                };
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0703_Export.xlsx", 1, loginInfo.UserId, "内示书计算导出");
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

        #region 错误品番导出
        [HttpPost]
        [EnableCors("any")]
        public string exportApiPF([FromBody] dynamic data)
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
                DataTable dt = FS0703_Logic.SearchException();

                if (dt.Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "没有可导出数据！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }
                string[] fields = { "vcPart_id","vcException"
                };
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0703_Exception.xlsx", 1, loginInfo.UserId, "月度内示");
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

        #region 发送
        [HttpPost]
        [EnableCors("any")]
        public string SendApi([FromBody] dynamic data)
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
                //FS0703_Logic.Save(listInfoData, loginInfo.UserId, ref strErrorPartId);
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









    }
}
