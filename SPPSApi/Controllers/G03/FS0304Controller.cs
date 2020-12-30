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

        //FS0304_Logic FS0304_Logic = new FS0304_Logic();
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
                List<Object> dataList_C003 = ComFunction.convertAllToResult(ComFunction.getTCode("C003"));//内外区分
                List<object> dataList_JD = new List<object> { "已联络", "已回复", "已退回", "已织入原单位" };
                
                res.Add("C004", dataList_C003);
                res.Add("JD", dataList_JD);

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

        //#region 检索
        //[HttpPost]
        //[EnableCors("any")]
        //public string searchApi([FromBody] dynamic data)
        //{
        //    string strToken = Request.Headers["X-Token"];
        //    if (!isLogin(strToken))
        //    {
        //        return error_login();
        //    }
        //    LoginInfo loginInfo = getLoginByToken(strToken);
        //    //以下开始业务处理
        //    ApiResult apiResult = new ApiResult();
        //    dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));

        //    string strPart_id = dataForm.vcPart_id;
        //    string strSupplier_id = dataForm.vcSupplier_id;

        //    try
        //    {
        //        DataTable dt = FS0304_Logic.Search(strPart_id, strSupplier_id);
        //        DtConverter dtConverter = new DtConverter();

        //        dtConverter.addField("selected", ConvertFieldType.BoolType, null);
        //        dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
        //        dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
        //        dtConverter.addField("dJiuBegin", ConvertFieldType.DateType, "yyyy/MM/dd");
        //        dtConverter.addField("dSendTime", ConvertFieldType.DateType, "yyyy/MM/dd");
        //        dtConverter.addField("dOperatorTime", ConvertFieldType.DateType, "yyyy/MM/dd");

        //        List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);

        //        apiResult.code = ComConstant.SUCCESS_CODE;
        //        apiResult.data = dataList;
        //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //    }
        //    catch (Exception ex)
        //    {
        //        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0312", ex, loginInfo.UserId);
        //        apiResult.code = ComConstant.ERROR_CODE;
        //        apiResult.data = "检索失败";
        //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //    }
        //}
        //#endregion

        //#region 保存
        //[HttpPost]
        //[EnableCors("any")]
        //public string saveApi([FromBody] dynamic data)
        //{
        //    //验证是否登录
        //    string strToken = Request.Headers["X-Token"];
        //    if (!isLogin(strToken))
        //    {
        //        return error_login();
        //    }
        //    LoginInfo loginInfo = getLoginByToken(strToken);
        //    //以下开始业务处理
        //    ApiResult apiResult = new ApiResult();
        //    try
        //    {
        //        dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
        //        JArray listInfo = dataForm.multipleSelection;
        //        List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
        //        bool hasFind = false;//是否找到需要新增或者修改的数据
        //        for (int i = 0; i < listInfoData.Count; i++)
        //        {
        //            bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
        //            bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
        //            if (bAddFlag == true)
        //            {//新增
        //                hasFind = true;
        //            }
        //            else if (bAddFlag == false && bModFlag == true)
        //            {//修改
        //                hasFind = true;
        //            }
        //        }
        //        if (!hasFind)
        //        {
        //            apiResult.code = ComConstant.ERROR_CODE;
        //            apiResult.data = "最少有一个编辑行！";
        //            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //        }
        //        //开始数据验证
        //        if (hasFind)
        //        {
        //            string[,] strField = new string[,] {{"品番"     ,"号旧"    ,"旧型开始时间" ,"供应商代码"   ,"供应商名称"     ,"车型(开发代码)"  ,"品名"      ,"旧型今后必要数(合计)","受入"      ,"送信时间"     },
        //                                                {"vcPart_id","vcHaoJiu","dJiuBegin"    ,"vcSupplier_id","vcSupplier_Name","vcCarTypeDesign" ,"vcPartName","vcSumLater"          ,"vcInput_No","dSendTime"    },
        //                                                {""         ,""        ,FieldCheck.Date,""             ,""               ,""                ,""          ,""                    ,""          ,FieldCheck.Date},
        //                                                {"12"       ,"1"       ,"0"            ,"4"            ,"100"            ,"4"               ,"100"       ,"20"                  ,"2"         ,"0"            },//最大长度设定,不校验最大长度用0
        //                                                {"1"        ,"1"       ,"1"            ,"1"            , "1"             ,"1"               , "1"        ,"1"                   ,"1"         ,"1"            },//最小长度设定,可以为空用0
        //                                                {"1"        ,"2"       ,"3"            ,"4"            , "5"             ,"6"               , "7"        ,"8"                   ,"9"         ,"10"           }//前台显示列号，从0开始计算,注意有选择框的是0
        //            };
        //            //需要判断时间区间先后关系的字段
        //            string[,] strDateRegion = null;
        //            string[,] strSpecialCheck = null;

        //            List<Object> checkRes = ListChecker.validateList(listInfoData, strField, strDateRegion, strSpecialCheck, true, "FS0304");
        //            if (checkRes != null)
        //            {
        //                apiResult.code = ComConstant.ERROR_CODE;
        //                apiResult.data = checkRes;
        //                apiResult.flag = Convert.ToInt32(ERROR_FLAG.单元格定位提示);
        //                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //            }
        //        }

        //        string strErrorPartId = "";
        //        FS0304_Logic.Save(listInfoData, loginInfo.UserId, ref strErrorPartId);
        //        if (strErrorPartId != "")
        //        {
        //            apiResult.code = ComConstant.ERROR_CODE;
        //            apiResult.data = "保存失败，以下品番使用开始、结束区间存在重叠：<br/>" + strErrorPartId;
        //            apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
        //            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //        }
        //        apiResult.code = ComConstant.SUCCESS_CODE;
        //        apiResult.data = null;
        //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //    }
        //    catch (Exception ex)
        //    {
        //        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0312", ex, loginInfo.UserId);
        //        apiResult.code = ComConstant.ERROR_CODE;
        //        apiResult.data = "保存失败";
        //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //    }
        //}
        //#endregion

        //#region 删除
        //[HttpPost]
        //[EnableCors("any")]
        //public string delApi([FromBody] dynamic data)
        //{
        //    //验证是否登录
        //    string strToken = Request.Headers["X-Token"];
        //    if (!isLogin(strToken))
        //    {
        //        return error_login();
        //    }
        //    LoginInfo loginInfo = getLoginByToken(strToken);
        //    //以下开始业务处理
        //    ApiResult apiResult = new ApiResult();
        //    try
        //    {
        //        dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
        //        JArray checkedInfo = dataForm.multipleSelection;
        //        List<Dictionary<string, Object>> listInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();
        //        if (listInfoData.Count == 0)
        //        {
        //            apiResult.code = ComConstant.ERROR_CODE;
        //            apiResult.data = "最少选择一条数据！";
        //            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //        }
        //        FS0304_Logic.Del(listInfoData, loginInfo.UserId);
        //        apiResult.code = ComConstant.SUCCESS_CODE;
        //        apiResult.data = null;
        //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //    }
        //    catch (Exception ex)
        //    {
        //        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0312", ex, loginInfo.UserId);
        //        apiResult.code = ComConstant.ERROR_CODE;
        //        apiResult.data = "删除失败";
        //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //    }
        //}
        //#endregion

        //#region 导出
        //[HttpPost]
        //[EnableCors("any")]
        //public string exportApi([FromBody] dynamic data)
        //{
        //    string strToken = Request.Headers["X-Token"];
        //    if (!isLogin(strToken))
        //    {
        //        return error_login();
        //    }
        //    LoginInfo loginInfo = getLoginByToken(strToken);
        //    //以下开始业务处理
        //    ApiResult apiResult = new ApiResult();
        //    dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));

        //    string strPart_id = dataForm.vcPart_id;
        //    string strSupplier_id = dataForm.vcSupplier_id;
        //    try
        //    {
        //        DataTable dt = FS0304_Logic.Search(strPart_id, strSupplier_id);
        //        string[] fields = { "vcPart_id", "vcHaoJiu", "dJiuBegin", "vcSupplier_id", "vcSupplier_Name", "vcCarTypeDesign"
        //        ,"vcPartName","vcSumLater","vcInput_No","dSendTime"
        //        };
        //        string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0304_Export.xlsx", 2, loginInfo.UserId, FunctionID);
        //        if (filepath == "")
        //        {
        //            apiResult.code = ComConstant.ERROR_CODE;
        //            apiResult.data = "导出生成文件失败";
        //            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //        }
        //        apiResult.code = ComConstant.SUCCESS_CODE;
        //        apiResult.data = filepath;
        //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //    }
        //    catch (Exception ex)
        //    {
        //        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0904", ex, loginInfo.UserId);
        //        apiResult.code = ComConstant.ERROR_CODE;
        //        apiResult.data = "导出失败";
        //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //    }
        //}
        //#endregion

        //#region 调达送信
        //[HttpPost]
        //[EnableCors("any")]
        //public string sendMailApi([FromBody] dynamic data)
        //{
        //    //验证是否登录
        //    string strToken = Request.Headers["X-Token"];
        //    if (!isLogin(strToken))
        //    {
        //        return error_login();
        //    }
        //    LoginInfo loginInfo = getLoginByToken(strToken);
        //    //以下开始业务处理
        //    ApiResult apiResult = new ApiResult();
        //    try
        //    {
        //        dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
        //        Object multipleSelection = dataForm.multipleSelection;
        //        if (multipleSelection == null)//如果没有选中数据，那么就是按检索条件发送
        //        {


        //        }
        //        else
        //        {


        //        }
        //        //发送邮件
        //        //发件人邮箱，对方邮箱，邮件标题、内容、附件需要确认
        //        apiResult.code = ComConstant.SUCCESS_CODE;
        //        apiResult.data = null;
        //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //    }
        //    catch (Exception ex)
        //    {
        //        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0906", ex, loginInfo.UserId);
        //        apiResult.code = ComConstant.ERROR_CODE;
        //        apiResult.data = "销售展开失败";
        //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //    }
        //}
        //#endregion


    }
}
