﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Text.RegularExpressions;
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
    [Route("api/FS0605/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0605Controller : BaseController
    {
        FS0605_Logic fs0605_Logic = new FS0605_Logic();
        private readonly string FunctionID = "FS0605";

        private readonly IWebHostEnvironment _webHostEnvironment;


        public FS0605Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        #region 绑定
        //[HttpPost]
        //[EnableCors("any")]
        //public string bindConst()
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
        //        DataTable dt = fs0605_Logic.BindConst();
        //        List<Object> dataList = ComFunction.convertToResult(dt, new string[] { "vcCodeId", "vcCodeName" });
        //        apiResult.code = ComConstant.SUCCESS_CODE;
        //        apiResult.data = dataList;
        //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //    }
        //    catch (Exception ex)
        //    {
        //        ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0501", ex, loginInfo.UserId);
        //        apiResult.code = ComConstant.ERROR_CODE;
        //        apiResult.data = "绑定常量列表失败";
        //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //    }
        //}
        #endregion

        #region 检索
        [HttpPost]
        [EnableCors("any")]
        public string searchApi([FromBody] dynamic data)
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
            string vcSupplier_id = dataForm.vcSupplier_id == null ? "" : dataForm.vcSupplier_id;
            string vcWorkArea = dataForm.vcWorkArea == null ? "" : dataForm.vcWorkArea;
            string vcIsSureFlag = dataForm.vcIsSureFlag == null ? "" : dataForm.vcIsSureFlag;

            try
            {
                DataTable dt = fs0605_Logic.Search(vcSupplier_id, vcWorkArea, vcIsSureFlag);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0501", ex, loginInfo.UserId);
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
            string vcSupplier_id = dataForm.vcSupplier_id == null ? "" : dataForm.vcSupplier_id;
            string vcWorkArea = dataForm.vcWorkArea == null ? "" : dataForm.vcWorkArea;
            string vcIsSureFlag = dataForm.vcIsSureFlag == null ? "" : dataForm.vcIsSureFlag;

            try
            {
                DataTable dt = fs0605_Logic.Search(vcSupplier_id, vcWorkArea, vcIsSureFlag);
                string[] head = new string[] { };
                string[] field = new string[] { };

                head = new string[] { "供应商代码", "工区", "生产能力&纳期确认", "联系人1", "电话1", "邮箱1", "联系人2", "电话2", "邮箱2", "联系人3", "电话3", "邮箱3" };
                field = new string[] { "vcSupplier_id", "vcWorkArea", "vcIsSureFlag", "vcLinkMan1", "vcPhone1", "vcEmail1", "vcLinkMan2", "vcPhone2", "vcEmail2", "vcLinkMan3", "vcPhone3", "vcEmail3" };
                string msg = string.Empty;
                //string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0309_Export.xlsx", 2, loginInfo.UserId, FunctionID);
                string filepath = ComFunction.DataTableToExcel(head, field, dt, ".", loginInfo.UserId, FunctionID, ref msg);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0502", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

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
                }
                if (!hasFind)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少有一个编辑行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                #region 判断新增的数据本身是否有重复的 和数据库内部关键字是否有重复的
                //本身判重
                DataTable dtadd = new DataTable();
                dtadd.Columns.Add("vcSupplier_id");
                dtadd.Columns.Add("vcWorkArea");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (listInfoData[i]["vcAddFlag"].ToString().ToLower() == "true")
                    {
                        DataRow dr = dtadd.NewRow();
                        dr["vcSupplier_id"] = listInfoData[i]["vcSupplier_id"].ToString();
                        dr["vcWorkArea"] = listInfoData[i]["vcWorkArea"].ToString();
                        dtadd.Rows.Add(dr);
                    }
                }

                if (dtadd.Rows.Count > 0)
                {
                    if (dtadd.Rows.Count > 1)
                    {
                        for (int i = 0; i < dtadd.Rows.Count; i++)
                        {
                            for (int j = i + 1; j < dtadd.Rows.Count; j++)
                            {
                                if (dtadd.Rows[i]["vcSupplier_id"].ToString() == dtadd.Rows[j]["vcSupplier_id"].ToString() &&
                                    dtadd.Rows[i]["vcWorkArea"].ToString() == dtadd.Rows[j]["vcWorkArea"].ToString())
                                {
                                    apiResult.code = ComConstant.ERROR_CODE;
                                    apiResult.data = "供应商代码" + dtadd.Rows[i]["vcSupplier_id"].ToString() + "、工区" + dtadd.Rows[i]["vcWorkArea"].ToString() + "存在重复项，请确认！";
                                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                                }
                            }
                        }
                    }
                    #region
                    //string[] columnArray = { "vcCodeId", "vcValue" };
                    //DataView dtaddView = dtadd.DefaultView;
                    //DataTable dtaddNew = dtaddView.ToTable(true, columnArray);
                    //if (dtadd.Rows.Count != dtaddNew.Rows.Count)
                    //{
                    //    apiResult.code = ComConstant.ERROR_CODE;
                    //    apiResult.data = "新增数据中有重复数据,请确认！";
                    //    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    //}
                    #endregion
                    for (int m=0; m < dtadd.Rows.Count; m++)
                    {
                        if (dtadd.Rows[m]["vcSupplier_id"].ToString()=="")
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "供应商代码" + dtadd.Rows[m]["vcSupplier_id"].ToString() + "不允许为空，请确认！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        if (dtadd.Rows[m]["vcWorkArea"].ToString() == "")
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "工区不能为空，如未确定，请写无！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                    }
                    //数据库验证  true  存在重复项
                    DataTable dt = fs0605_Logic.CheckDistinctByTable(dtadd);
                    if (dt.Rows.Count > 0)
                    {
                        string errMsg = string.Empty;
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            errMsg += "供应商代码" + dt.Rows[i]["vcSupplier_id"].ToString() + "、工区" + dt.Rows[i]["vcWorkArea"].ToString() + ",";
                        }
                        errMsg.Substring(0, errMsg.LastIndexOf(","));
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = errMsg + "存在重复项，请确认！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }

                #endregion

                //开始数据验证
                if (hasFind)
                {
                    string[,] strField = new string[,] {{"供应商代码","工区","生产能力&纳期确认", "联系人1", "电话1","邮箱1", "联系人2", "电话2","邮箱2", "联系人3", "电话3","邮箱3"},
                                                {"vcSupplier_id", "vcWorkArea", "vcIsSureFlag", "vcLinkMan1", "vcPhone1", "vcEmail1", "vcLinkMan2", "vcPhone2", "vcEmail2", "vcLinkMan3", "vcPhone3", "vcEmail3"},
                                                {"","","","","","","","","","","","" },
                                                {"4","50","1","100","100","100","100","100","100","100","100","100"},//最大长度设定,不校验最大长度用0
                                                {"1","1","1","0","0","0","0","0","0","0","0","0"},//最小长度设定,可以为空用0
                                                {"1","2","3","4","5","6","7","8","9","10","11","12"}//前台显示列号，从0开始计算,注意有选择框的是0
                    };
                    //需要判断时间区间先后关系的字段
                    string[,] strDateRegion = { };
                    string[,] strSpecialCheck = { //例子-变更事项字段，当它为新设时，号旧必须为号口，旧型开始、旧型结束、旧型持续开始必须为空
                        
                    };



                    List<Object> checkRes = ListChecker.validateList(listInfoData, strField, strDateRegion, strSpecialCheck, true, "FS0605");
                    if (checkRes != null)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = checkRes;
                        apiResult.flag = Convert.ToInt32(ERROR_FLAG.单元格定位提示);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                #region delete 
                //                string strErrorPartId = "";
                //                fs0105_Logic.Save(listInfoData, loginInfo.UserId, ref strErrorPartId);
                //                if (strErrorPartId != "")
                //                {
                //                    apiResult.code = ComConstant.ERROR_CODE;
                //                    apiResult.data = "保存失败，以下常量区分代码和Key键区间存在重叠：<br/>" + strErrorPartId;
                //                    apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                //                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                //                }
                //                apiResult.code = ComConstant.SUCCESS_CODE;
                //                apiResult.data = null;
                //                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);


                //                DataTable dtmod = new DataTable();
                //                vcSupplier_id, vcWorkArea, vcIsSureFlag, vcLinkMan, vcPhone, vcEmail, vcOperatorID, dOperatorTime
                //                dtmod.Columns.Add("vcSupplier_id");
                //                dtmod.Columns.Add("vcWorkArea");
                //                dtmod.Columns.Add("vcIsSureFlag");
                //                dtmod.Columns.Add("vcLinkMan");
                //                dtmod.Columns.Add("vcPhone");
                //                dtmod.Columns.Add("vcEmail");

                //                DataTable dtadd = dtmod.Clone();
                //                for (int i = 0; i < listInfoData.Count; i++)
                //                {
                //                    bool bmodflag = (bool)listInfoData[i]["vcmodflag"];//false可编辑,true不可编辑
                //                    bool baddflag = (bool)listInfoData[i]["vcaddflag"];//false可编辑,true不可编辑
                //                    if (bmodflag == false && baddflag == false)
                //                    {//新增
                //                        DataRow dr = dtadd.NewRow();
                //                        dr["vcSupplier_id"] = listInfoData[i]["vcSupplier_id"].ToString();
                //                        dr["vcWorkArea"] = listInfoData[i]["vcWorkArea"].ToString();
                //                        dr["vcIsSureFlag"] = listInfoData[i]["vcIsSureFlag"].ToString();
                //                        dr["vcLinkMan"] = listInfoData[i]["vcLinkMan"].ToString();
                //                        dr["vcPhone"] = listInfoData[i]["vcPhone"].ToString();
                //                        dr["vcEmail"] = listInfoData[i]["vcEmail"].ToString();
                //                        dtadd.Rows.Add(dr);
                //                    }
                //                    else if (bmodflag == false && baddflag == true)
                //                    {//修改
                //                        DataRow dr = dtmod.NewRow();
                //                        dr["vcSupplier_id"] = listInfoData[i]["vcSupplier_id"].ToString();
                //                        dr["vcWorkArea"] = listInfoData[i]["vcWorkArea"].ToString();
                //                        dr["vcIsSureFlag"] = listInfoData[i]["vcIsSureFlag"].ToString();
                //                        dr["vcLinkMan"] = listInfoData[i]["vcLinkMan"].ToString();
                //                        dr["vcPhone"] = listInfoData[i]["vcPhone"].ToString();
                //                        dr["vcEmail"] = listInfoData[i]["vcEmail"].ToString();
                //                        dtmod.Rows.Add(dr);
                //                    }
                //                }
                //                if (dtadd.Rows.Count == 0 && dtmod.Rows.Count == 0)
                //                {
                //                    apiResult.code = ComConstant.ERROR_CODE;
                //                    apiResult.data = "最少有一个编辑行！";
                //                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                //                }
                //                判断空
                //                for (int i = 0; i < dtadd.Rows.Count; i++)
                //                {
                //                    [vcCodeId] ,[vcCodeName]  ,[vcValue] ,[vcName] ,[vcMeaning]
                //                    string vcSupplier_id = dtadd.Rows[i]["vcSupplier_id"].ToString();
                //                    string strvcWorkArea = dtadd.Rows[i]["vcWorkArea"].ToString();
                //                    string strvcIsSureFlag = dtadd.Rows[i]["vcIsSureFlag"].ToString();
                //                    string strvcEmail = dtadd.Rows[i]["vcEmail"].ToString();
                //                    if (vcSupplier_id.Length == 0 || vcSupplier_id.Trim().Length != 4)
                //                    {
                //                        apiResult.code = ComConstant.ERROR_CODE;
                //                        apiResult.data = "新增的数据供应商代码不能为空并且长度必须是四位,请确认！";
                //                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                //                    }
                //                    if (strvcWorkArea.Length == 0)
                //                    {
                //                        apiResult.code = ComConstant.ERROR_CODE;
                //                        apiResult.data = "新增的工区不能为空,请确认！";
                //                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                //                    }
                //                    if (strvcIsSureFlag.Length == 0)
                //                    {
                //                        apiResult.code = ComConstant.ERROR_CODE;
                //                        apiResult.data = "新增的生产能力&纳期确认不能为空,请确认！";
                //                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                //                    }
                //                    if (strvcEmail.Length > 0)
                //                    {
                //                        ：("^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$")；
                //                        ^\\s * ([A - Za - z0 - 9_ -] + (\\.\\w +)*@(\\w +\\.)+\\w{ 2,5})\\s *$
                //                        Regex r = new Regex("^\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*$");
                //                if (!r.IsMatch(strvcEmail))
                //                {
                //                    apiResult.code = ComConstant.ERROR_CODE;
                //                    apiResult.data = "新增的邮箱不是一个有效的邮箱地址,请确认！";
                //                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                //                }
                //            }
                //                }
                //                for (int i = 0; i<dtmod.Rows.Count; i++)
                //                {
                //                    [vcCodeId] ,[vcCodeName]  ,[vcValue] ,[vcName] ,[vcMeaning]
                //        string strvcEmail = dtmod.Rows[i]["vcEmail"].ToString();
                //                    if (strvcEmail.Length > 0)
                //                    {
                //                        Regex r = new Regex("^\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*$");
                //                        if (!r.IsMatch(strvcEmail))
                //                        {
                //                            apiResult.code = ComConstant.ERROR_CODE;
                //                            apiResult.data = "修改的邮箱不是一个有效的邮箱地址,请确认！";
                //                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                //                        }
                //}
                //                }
                //                验证 是否有重复 数据
                //                string[] columnArray = { "vcSupplier_id", "vcWorkArea" };
                //if (dtadd.Rows.Count > 0)
                //{
                //    DataView dtaddView = dtadd.DefaultView;
                //    DataTable dtaddNew = dtaddView.ToTable(true, columnArray);
                //    if (dtadd.Rows.Count != dtaddNew.Rows.Count)
                //    {
                //        apiResult.code = ComConstant.ERROR_CODE;
                //        apiResult.data = "新增数据中有重复数据,请确认！";
                //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                //    }
                //}
                //if (dtmod.Rows.Count > 0)
                //{
                //    DataView dtmodView = dtmod.DefaultView;
                //    DataTable dtmodNew = dtmodView.ToTable(true, columnArray);
                //    if (dtmod.Rows.Count != dtmodNew.Rows.Count)
                //    {
                //        apiResult.code = ComConstant.ERROR_CODE;
                //        apiResult.data = "编辑的数据中在数据库中有重复数据,请确认！";
                //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                //    }
                //}
                //DataTable dtHuiZong = dtadd.Clone();//
                //if (dtadd.Rows.Count > 0)
                //{
                //    dtHuiZong.Merge(dtadd);
                //}
                //if (dtmod.Rows.Count > 0)
                //{
                //    dtHuiZong.Merge(dtmod);
                //}

                //DataView dtAllView = dtHuiZong.DefaultView;
                //DataTable dtAllNew = dtAllView.ToTable(true, columnArray);
                //if (dtadd.Rows.Count + dtmod.Rows.Count != dtAllNew.Rows.Count)
                //{
                //    apiResult.code = ComConstant.ERROR_CODE;
                //    apiResult.data = "保存数据中新增和编辑有重复数据,请确认！";
                //    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                //}
                //验证新增的里面是否有重复数据

                //                if (dtadd.Rows.Count > 0)
                //{
                //    Boolean isExistAddData = fs0605_Logic.isExistAddData(dtadd);
                //    if (isExistAddData)
                //    {
                //        apiResult.code = ComConstant.ERROR_CODE;
                //        apiResult.data = "新增的数据已存在数据库中,请确认！";
                //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                //    }
                //}
                //验证修改的里面是否有重复数据
                //                if (dtmod.Rows.Count > 0)
                //{
                //    Boolean isExistModData = fs0105_Logic.isExistModData(dtmod);
                //    if (isExistModData)
                //    {
                //        apiResult.code = ComConstant.ERROR_CODE;
                //        apiResult.data = "编辑的数据中在数据库中有重复数据,请确认！";
                //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                //    }
                //}

                //fs0605_Logic.Save(dtadd, dtmod, loginInfo.UserId);
                //apiResult.code = ComConstant.SUCCESS_CODE;
                //apiResult.data = null;
                //return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                #endregion
                string strErrorPartId = "";
                fs0605_Logic.Save(listInfoData, loginInfo.UserId, ref strErrorPartId);
                if (strErrorPartId != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "保存失败，以下常量区分代码和Key键区间存在重叠：<br/>" + strErrorPartId;
                    apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0503", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
        #region 删除
        [HttpPost]
        [EnableCors("any")]
        public string deleteApi([FromBody] dynamic data)
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
                fs0605_Logic.Del(listInfoData, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0504", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}

