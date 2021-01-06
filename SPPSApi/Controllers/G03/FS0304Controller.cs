﻿using System;
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

namespace SPPSApi.Controllers.G03
{
    [Route("api/FS0304/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0304Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0304_Logic fs0304_Logic = new FS0304_Logic();
        private readonly string FunctionID = "FS0304";

        public FS0304Controller(IWebHostEnvironment webHostEnvironment)
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
                List<Object> dataList_C026 = ComFunction.convertAllToResult(ComFunction.getTCode("C026"));//生确进度
                List<Object> dataList_C002 = ComFunction.convertAllToResult(ComFunction.getTCode("C002"));//内外区分
                List<Object> dataList_C003 = ComFunction.convertAllToResult(ComFunction.getTCode("C003"));//生确进度
                List<Object> dataList_C012 = ComFunction.convertAllToResult(ComFunction.getTCode("C012"));//OE
                List<Object> dataList_C023 = ComFunction.convertAllToResult(ComFunction.getTCode("C023"));//包装厂
                List<Object> dataList_C098 = ComFunction.convertAllToResult(ComFunction.getTCode("C098"));//车种
                List<Object> dataList_C028 = ComFunction.convertAllToResult(ComFunction.getTCode("C028"));//防锈指示
                List<Object> dataList_C029 = ComFunction.convertAllToResult(ComFunction.getTCode("C029"));//对应结果可否确认结果
                List<Object> dataList_C030 = ComFunction.convertAllToResult(ComFunction.getTCode("C030"));//防锈对应可否

                res.Add("C026", dataList_C026);
                res.Add("C002", dataList_C002);
                res.Add("C003", dataList_C003);
                res.Add("C012", dataList_C012);
                res.Add("C023", dataList_C023);
                res.Add("C098", dataList_C098);
                res.Add("C028", dataList_C028);
                res.Add("C029", dataList_C029);
                res.Add("C030", dataList_C030);

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

            string strJD = dataForm.vcJD;
            string strInOutflag = dataForm.vcInOutflag;
            string strSupplier_id = dataForm.vcSupplier_id;
            string strCarType = dataForm.vcCarType;
            string strPart_id = dataForm.vcPart_id;
            string strIsDYJG = dataForm.vcIsDYJG;
            string strIsDYFX = dataForm.vcIsDYFX;

            try
            {
                DataTable dt = fs0304_Logic.Search(strJD, strInOutflag, strSupplier_id, strCarType, strPart_id, strIsDYJG, strIsDYFX);
                DtConverter dtConverter = new DtConverter();

                dtConverter.addField("selected", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("dSSDateMonth", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dSupplier_BJ", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dSupplier_HK", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dTFTM_BJ", ConvertFieldType.DateType, "yyyy/MM/dd");

                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0312", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 导出
        [HttpPost]
        [EnableCors("any")]
        public string exportapi([FromBody] dynamic data)
        {
            string strtoken = Request.Headers["x-token"];
            if (!isLogin(strtoken))
            {
                return error_login();
            }
            LoginInfo logininfo = getLoginByToken(strtoken);
            //以下开始业务处理
            ApiResult apiresult = new ApiResult();
            dynamic dataform = JsonConvert.DeserializeObject(Convert.ToString(data));

            string strJD = dataform.vcJD;
            string strInOutflag = dataform.strInOutflag;
            string strSupplier_id = dataform.vcSupplier_id;
            string strCarType = dataform.vcCarType;
            string strPart_id = dataform.vcPart_id;
            string strIsDYJG = dataform.vcIsDYJG;
            string strIsDYFX = dataform.vcIsDYFX;
            try
            {
                DataTable dt = fs0304_Logic.Search(strJD, strInOutflag, strSupplier_id, strCarType, strPart_id, strIsDYJG, strIsDYFX);
                string[] fields = { "dSSDateMonth", "vcJD_Name", "vcPart_id", "vcSPINo",
                                    "vcChange_Name", "vcCarType_Name","vcInOutflag_Name","vcPartName",
                                    "vcOE_Name","vcSupplier_id","vcFXDiff_Name","vcFXNo",
                                    "vcSumLater","vcIsDYJG_Name","vcIsDYFX_Name","vcYQorNG",
                                    "vcSCPlace_City","vcSCPlace_Province","vcCHPlace_City","vcCHPlace_Province",
                                    "vcPackFactory_Name","vcSCSPlace","dSupplier_BJ","dSupplier_HK",
                                    "dTFTM_BJ","vcZXBZDiff","vcZXBZNo"
                };
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "fs0304_export.xlsx", 2, logininfo.UserId, FunctionID);
                if (filepath == "")
                {
                    apiresult.code = ComConstant.ERROR_CODE;
                    apiresult.data = "导出生成文件失败";
                    return JsonConvert.SerializeObject(apiresult, Formatting.Indented, JSON_SETTING);
                }
                apiresult.code = ComConstant.SUCCESS_CODE;
                apiresult.data = filepath;
                return JsonConvert.SerializeObject(apiresult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "m03ue0904", ex, logininfo.UserId);
                apiresult.code = ComConstant.ERROR_CODE;
                apiresult.data = "导出失败";
                return JsonConvert.SerializeObject(apiresult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}
