﻿using System;
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
 


namespace SPPSApi.Controllers.G00
{
    [Route("api/FS0108/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0108Controller : BaseController
    {
        FS0108_Logic fs0108_Logic = new FS0108_Logic();
        private readonly string FunctionID = "FS0108";
        
        private readonly IWebHostEnvironment _webHostEnvironment;
        

        public FS0108Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        #region 
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
                DataTable dtSupplier = fs0108_Logic.GetSupplier();
                DataTable dtWorkArea = fs0108_Logic.GetWorkArea();

                List<Object> dataList_Supplier = ComFunction.convertToResult(dtSupplier, new string[] { "vcValue", "vcName" });
                List<Object> dataList_WorkArea = ComFunction.convertToResult(dtWorkArea, new string[] { "vcValue", "vcName" });

                List<Object> dtFZGC = ComFunction.convertAllToResult(ComFunction.getTCode("C000"));//内外区分

                res.Add("Supplier", dataList_Supplier);
                res.Add("WorkArea", dataList_WorkArea);
                res.Add("C000", dtFZGC);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0801", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "初始化失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        [HttpPost]
        [EnableCors("any")]
        public string changeSupplieridApi([FromBody] dynamic data)
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
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                string supplierCode = dataForm.supplierCode == null ? "" : dataForm.supplierCode;
                Dictionary<string, object> res = new Dictionary<string, object>();
                DataTable dtWorkAreaBySupplier = fs0108_Logic.GetWorkAreaBySupplier(supplierCode);

                List<Object> dataList_WorkAreaBySupplier = ComFunction.convertToResult(dtWorkAreaBySupplier, new string[] { "vcValue", "vcName" });

                res.Add("WorkAreaBySupplier", dataList_WorkAreaBySupplier);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0802", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "选择供应商联动工区失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
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
            string vcValue1 = dataForm.vcValue1 == null ? "" : dataForm.vcValue1;
            string vcValue2 = dataForm.vcValue2 == null ? "" : dataForm.vcValue2;
            string vcValue5 = dataForm.vcValue5 == null ? "" : dataForm.vcValue5;

            try
            {
                DataTable dt = fs0108_Logic.Search( vcValue1, vcValue2, vcValue5);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0803", ex, loginInfo.UserId);
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
            string vcValue1 = dataForm.vcValue1 == null ? "" : dataForm.vcValue1;
            string vcValue2 = dataForm.vcValue2 == null ? "" : dataForm.vcValue2;
            string vcValue5 = dataForm.vcValue5 == null ? "" : dataForm.vcValue5;

            try
            {
                DataTable dt = fs0108_Logic.Search( vcValue1, vcValue2, vcValue5);
                string[] head = new string[] { };
                string[] field = new string[] { };
               

                head = new string[] { "供应商代码", "工区", "发注工场", "开始时间", "结束时间" };
                field = new string[] { "vcValue1", "vcValue2", "vcValue5", "vcValue3", "vcValue4" };
                string msg = string.Empty;
                //string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0309_Export.xlsx", 2, loginInfo.UserId, FunctionID);
                string filepath = ComFunction.generateExcelWithXlt(dt, field, _webHostEnvironment.ContentRootPath, "FS0108_Data.xlsx", 1, loginInfo.UserId, FunctionID, true);
                //string filepath = ComFunction.DataTableToExcel(head, field, dt, ".", loginInfo.UserId, FunctionID,ref msg);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0804", ex, loginInfo.UserId);
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
                DataTable dataTable = fs0108_Logic.createTable("fs0108");
                bool bReault = true;
                //开始数据验证
                if (hasFind)
                {
                    string[,] strField = new string[,] {{"供应商代码", "工区", "发注工场","开始时间", "结束时间" },
                                                {"vcValue1", "vcValue2","vcValue5" ,"vcValue3", "vcValue4"},
                                                {FieldCheck.NumChar,FieldCheck.NumChar,"",FieldCheck.Date,FieldCheck.Date },
                                                {"4","100","500","500","500"},//最大长度设定,不校验最大长度用0
                                                {"4","1","1","1","1"},//最小长度设定,可以为空用0
                                                {"1","2","3","4","5"}//前台显示列号，从0开始计算,注意有选择框的是0
                    };
                    //需要判断时间区间先后关系的字段
                    string[,] strDateRegion = { { "vcValue3", "vcValue4" } };
                    string[,] strSpecialCheck = { //例子-变更事项字段，当它为新设时，号旧必须为号口，旧型开始、旧型结束、旧型持续开始必须为空
                        
                    };

                    List<Object> checkRes = ListChecker.validateList(listInfoData, strField, strDateRegion, strSpecialCheck, true, "FS0108");
                    if (checkRes != null)
                    {
                        for (int i = 0; i < checkRes.Count; i++)
                        {
                            string[] elements = ((DriverRes)checkRes[i]).element.Split("_");
                            string index = elements[1].Substring(elements[1].IndexOf('d') + 1);
                            string vcSupplier = "";
                            string vcWorkArea = "";
                            string vcFzgc = "";
                            for (int m = 0; m < listInfoData.Count; m++)
                            {
                                if (listInfoData[m]["iAPILineNo"].ToString() == index)
                                {
                                    vcSupplier = listInfoData[m]["vcValue1"] == null ? "" : listInfoData[m]["vcValue1"].ToString();
                                    vcWorkArea = listInfoData[m]["vcValue2"] == null ? "" : listInfoData[m]["vcValue2"].ToString();
                                    vcFzgc = listInfoData[m]["vcValue5"] == null ? "" : listInfoData[m]["vcValue5"].ToString();
                                    break;
                                }
                            }
                            DataRow dataRow = dataTable.NewRow();
                            dataRow["vcSupplier"] = vcSupplier;
                            dataRow["vcWorkArea"] = vcWorkArea;
                            dataRow["vcFzgc"] = vcFzgc; 
                            dataRow["vcMessage"] = ((DriverRes)checkRes[i]).popover.title + ((DriverRes)checkRes[i]).popover.description;
                            dataTable.Rows.Add(dataRow);
                            bReault = false;
                        }
                    }
                }
                #region 追加自己特殊的校验
                //本身判重
                DataTable dtadd = new DataTable();
                //vcCodeId ,vcCodeName  ,vcValue ,vcName 
                dtadd.Columns.Add("vcValue1");
                dtadd.Columns.Add("vcValue2");
                dtadd.Columns.Add("vcValue3");
                dtadd.Columns.Add("vcValue4");
                dtadd.Columns.Add("vcValue5");
                DataTable dtamody = new DataTable();
                dtamody.Columns.Add("vcValue1");
                dtamody.Columns.Add("vcValue2");
                dtamody.Columns.Add("vcValue3");
                dtamody.Columns.Add("vcValue4");
                dtamody.Columns.Add("vcValue5");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (listInfoData[i]["vcAddFlag"].ToString().ToLower() == "true")
                    {
                        DataRow dr = dtadd.NewRow();
                        dr["vcValue1"] = listInfoData[i]["vcValue1"].ToString();
                        dr["vcValue2"] = listInfoData[i]["vcValue2"].ToString();
                        dr["vcValue3"] = listInfoData[i]["vcValue3"].ToString();
                        dr["vcValue4"] = listInfoData[i]["vcValue4"].ToString();
                        dr["vcValue5"] = listInfoData[i]["vcValue5"].ToString();
                        dtadd.Rows.Add(dr);
                    }
                    else if ((bool)listInfoData[i]["vcAddFlag"] == false && (bool)listInfoData[i]["vcModFlag"] == true)
                    {//修改
                        DataRow dr = dtamody.NewRow();
                        dr["vcValue1"] = listInfoData[i]["vcValue1"].ToString();
                        dr["vcValue2"] = listInfoData[i]["vcValue2"].ToString();
                        dr["vcValue3"] = listInfoData[i]["vcValue3"].ToString();
                        dr["vcValue4"] = listInfoData[i]["vcValue4"].ToString();
                        dr["vcValue5"] = listInfoData[i]["vcValue5"].ToString();
                        dtamody.Rows.Add(dr);
                    }
                }
                for (int i=0;i< dtadd.Rows.Count;i++)
                {
                    string vcSupplier = dtadd.Rows[i]["vcValue1"].ToString();
                    string vcWorkArea = dtadd.Rows[i]["vcValue2"].ToString();
                    string vcFzgc = dtadd.Rows[i]["vcValue5"].ToString();
                    string vcStart = dtadd.Rows[i]["vcValue3"].ToString();
                    string vcEnd = dtadd.Rows[i]["vcValue4"].ToString();
                    for (int j= i + 1; j<dtadd.Rows.Count;j++) {
                        string vcSupplier1 = dtadd.Rows[j]["vcValue1"].ToString();
                        string vcWorkArea1 = dtadd.Rows[j]["vcValue2"].ToString();
                        string vcFzgc1 = dtadd.Rows[j]["vcValue5"].ToString();
                        string vcStart1 = dtadd.Rows[j]["vcValue3"].ToString();
                        string vcEnd1 = dtadd.Rows[j]["vcValue4"].ToString();
                        if (vcSupplier== vcSupplier1&& vcWorkArea== vcWorkArea1) {
                            if (Convert.ToInt32( vcStart.Replace("/","").Replace("-","")) > Convert.ToInt32(vcEnd1.Replace("/", "").Replace("-", "")) || Convert.ToInt32(vcEnd.Replace("/", "").Replace("-", "")) < Convert.ToInt32(vcStart1.Replace("/", "").Replace("-", "")))
                            {
                            }
                            else
                            {
                                DataRow dataRow = dataTable.NewRow();
                                dataRow["vcSupplier"] = vcSupplier;
                                dataRow["vcWorkArea"] = vcWorkArea;
                                dataRow["vcFzgc"] = vcFzgc;
                                dataRow["vcMessage"] = "新增的数据时间区间出现重叠";
                                dataTable.Rows.Add(dataRow);
                                bReault = false;
                            }
                        }
                    }
                    for (int j = 0; j < dtamody.Rows.Count; j++)
                    {
                        string vcSupplier1 = dtamody.Rows[j]["vcValue1"].ToString();
                        string vcWorkArea1 = dtamody.Rows[j]["vcValue2"].ToString();
                        string vcFzgc1 = dtamody.Rows[j]["vcValue5"].ToString();
                        string vcStart1 = dtamody.Rows[j]["vcValue3"].ToString();
                        string vcEnd1 = dtamody.Rows[j]["vcValue4"].ToString();
                        if (vcSupplier == vcSupplier1 && vcWorkArea == vcWorkArea1)
                        {
                            if (Convert.ToInt32(vcStart.Replace("/", "").Replace("-", "")) > Convert.ToInt32(vcEnd1.Replace("/", "").Replace("-", "")) || Convert.ToInt32(vcEnd.Replace("/", "").Replace("-", "")) < Convert.ToInt32(vcStart1.Replace("/", "").Replace("-", "")))
                            {
                            }
                            else
                            {
                                DataRow dataRow = dataTable.NewRow();
                                dataRow["vcSupplier"] = vcSupplier;
                                dataRow["vcWorkArea"] = vcWorkArea;
                                dataRow["vcFzgc"] = vcFzgc;
                                dataRow["vcMessage"] = "新增的数据与修改的数据时间区间出现重叠";
                                dataTable.Rows.Add(dataRow);
                                bReault = false;
                            }
                        }
                    }
                }
                //修改数据之间是否出现重叠校验
                for (int i = 0; i < dtamody.Rows.Count; i++)
                {
                    string vcSupplier = dtamody.Rows[i]["vcValue1"].ToString();
                    string vcWorkArea = dtamody.Rows[i]["vcValue2"].ToString();
                    string vcFzgc = dtamody.Rows[i]["vcValue5"].ToString();
                    string vcStart = dtamody.Rows[i]["vcValue3"].ToString();
                    string vcEnd = dtamody.Rows[i]["vcValue4"].ToString();
                    for (int j = i+1; j < dtamody.Rows.Count; j++)
                    {
                        string vcSupplier1 = dtamody.Rows[j]["vcValue1"].ToString();
                        string vcWorkArea1 = dtamody.Rows[j]["vcValue2"].ToString();
                        string vcFzgc1 = dtamody.Rows[j]["vcValue5"].ToString();
                        string vcStart1 = dtamody.Rows[j]["vcValue3"].ToString();
                        string vcEnd1 = dtamody.Rows[j]["vcValue4"].ToString();
                        if (vcSupplier == vcSupplier1 && vcWorkArea == vcWorkArea1)
                        {
                            if (Convert.ToInt32(vcStart.Replace("/", "").Replace("-", "")) > Convert.ToInt32(vcEnd1.Replace("/", "").Replace("-", "")) || Convert.ToInt32(vcEnd.Replace("/", "").Replace("-", "")) < Convert.ToInt32(vcStart1.Replace("/", "").Replace("-", "")))
                            {
                            }
                            else
                            {
                                DataRow dataRow = dataTable.NewRow();
                                dataRow["vcSupplier"] = vcSupplier;
                                dataRow["vcWorkArea"] = vcWorkArea;
                                dataRow["vcFzgc"] = vcFzgc;
                                dataRow["vcMessage"] = "修改的数据时间区间出现重叠";
                                dataTable.Rows.Add(dataRow);
                                bReault = false;
                            }
                        }
                    }
                }
                //验证新增与修改跟数据库里面是否重叠
                string strInAutoIds = string.Empty;
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if ((bool)listInfoData[i]["vcAddFlag"] == false && (bool)listInfoData[i]["vcModFlag"] == true)
                    {
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                        strInAutoIds += iAutoId + ",";
                    }
                    
                }
                if (strInAutoIds.Length>0)
                {
                    strInAutoIds = strInAutoIds.Substring(0, strInAutoIds.Length - 1);
                }
                for (int i = 0; i < dtadd.Rows.Count; i++)
                {
                    string vcSupplier = dtadd.Rows[i]["vcValue1"].ToString();
                    string vcWorkArea = dtadd.Rows[i]["vcValue2"].ToString();
                    string vcStart = dtadd.Rows[i]["vcValue3"].ToString();
                    string vcFzgc = dtadd.Rows[i]["vcValue5"].ToString();
                    string vcEnd = dtadd.Rows[i]["vcValue4"].ToString();
                    DataTable dtCheck = fs0108_Logic.checkData(vcSupplier, vcWorkArea, vcStart, vcEnd, strInAutoIds);
                    if (dtCheck.Rows.Count > 0)
                    {
                        DataRow dataRow = dataTable.NewRow();
                        dataRow["vcSupplier"] = vcSupplier;
                        dataRow["vcWorkArea"] = vcWorkArea;
                        dataRow["vcFzgc"] = vcFzgc;
                        dataRow["vcMessage"] = "新增的数据时间区间出现重叠";
                        dataTable.Rows.Add(dataRow);
                        bReault = false;
                    }
                }
                for (int i = 0; i < dtamody.Rows.Count; i++)
                {
                    string vcSupplier = dtamody.Rows[i]["vcValue1"].ToString();
                    string vcWorkArea = dtamody.Rows[i]["vcValue2"].ToString();
                    string vcStart = dtamody.Rows[i]["vcValue3"].ToString();
                    string vcFzgc = dtamody.Rows[i]["vcValue5"].ToString();
                    string vcEnd = dtamody.Rows[i]["vcValue4"].ToString();
                    DataTable dtCheck = fs0108_Logic.checkData(vcSupplier, vcWorkArea, vcStart, vcEnd, strInAutoIds);
                    if (dtCheck.Rows.Count > 0)
                    {
                        DataRow dataRow = dataTable.NewRow();
                        dataRow["vcSupplier"] = vcSupplier;
                        dataRow["vcWorkArea"] = vcWorkArea;
                        dataRow["vcFzgc"] = vcFzgc;
                        dataRow["vcMessage"] = "修改的数据时间区间出现重叠";
                        dataTable.Rows.Add(dataRow);
                        bReault = false;
                    }
                }


                #endregion
                if (!bReault)
                {
                    //弹出错误dtMessage
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dataTable;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string strErrorPartId = "";
                fs0108_Logic.Save(listInfoData, loginInfo.UserId, ref strErrorPartId);
                if (strErrorPartId != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "保存失败!";
                    apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0804", ex, loginInfo.UserId);
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
                fs0108_Logic.Del(listInfoData, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0705", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
        /// <summary>
        /// 导出消息信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string exportmessageApi([FromBody] dynamic data)
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
                List<Dictionary<string, Object>> listInfoData = dataForm.ToObject<List<Dictionary<string, Object>>>();
                //DataTable dataTable = fs0603_Logic.createTable("MES");
                //FS0404_Logic fs0404_Logic = new FS0404_Logic();
                DataTable dataTable = fs0108_Logic.createTable("fs0108");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    DataRow dataRow = dataTable.NewRow();
                    dataRow["vcSupplier"] = listInfoData[i]["vcSupplier"].ToString();
                    dataRow["vcWorkArea"] = listInfoData[i]["vcWorkArea"].ToString();
                    dataRow["vcFzgc"] = listInfoData[i]["vcFzgc"].ToString();
                    dataRow["vcMessage"] = listInfoData[i]["vcMessage"].ToString();
                    dataTable.Rows.Add(dataRow);
                }

                string[] fields = { "vcSupplier", "vcWorkArea", "vcMessage" };
                string filepath = ComFunction.generateExcelWithXlt(dataTable, fields, _webHostEnvironment.ContentRootPath, "FS0108_MessageList.xlsx", 1, loginInfo.UserId, FunctionID);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0706", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出错误信息失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
    }
}
