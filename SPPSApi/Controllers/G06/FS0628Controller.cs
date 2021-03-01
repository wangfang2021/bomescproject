﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SPPSApi.Controllers.G06
{
    [Route("api/FS0628/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0628Controller : BaseController
    {
        FS0628_Logic fs0628_Logic = new FS0628_Logic();
        private readonly string FunctionID = "FS0628";

        private readonly IWebHostEnvironment _webHostEnvironment;


        public FS0628Controller(IWebHostEnvironment webHostEnvironment)
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
                List<Object> dataList_C000 = ComFunction.convertAllToResult(ComFunction.getTCode("C000"));//发注工厂
                List<Object> dataList_C003 = ComFunction.convertAllToResult(ComFunction.getTCode("C003"));//内外区分
                List<Object> dataList_C004 = ComFunction.convertAllToResult(ComFunction.getTCode("C004"));//号旧区分

                DataTable dtSupplier = fs0628_Logic.GetSupplier();
                DataTable dtWorkArea = fs0628_Logic.GetWorkArea();
                DataTable dtInjectionOrderNo = fs0628_Logic.GetInjectionOrderNo();
               
                List<Object> dataList_Supplier = ComFunction.convertToResult(dtSupplier, new string[] { "vcValue", "vcName" });
                List<Object> dataList_InjectionOrderNo = ComFunction.convertToResult(dtInjectionOrderNo, new string[] { "vcValue", "vcName" });
                List<Object> dataList_WorkArea = ComFunction.convertToResult(dtWorkArea, new string[] { "vcValue", "vcName" });

                res.Add("C000", dataList_C000);
                res.Add("C003", dataList_C003);
                res.Add("C004", dataList_C004);
                res.Add("Supplier", dataList_Supplier);
                res.Add("InjectionOrderNo", dataList_InjectionOrderNo);
                res.Add("WorkArea", dataList_WorkArea);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2801", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "初始化失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
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
                DataTable dtWorkAreaBySupplier = fs0628_Logic.GetWorkAreaBySupplier(supplierCode);

                List<Object> dataList_WorkAreaBySupplier = ComFunction.convertToResult(dtWorkAreaBySupplier, new string[] { "vcValue", "vcName" });

                res.Add("WorkAreaBySupplier", dataList_WorkAreaBySupplier);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0410", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "选择供应商联动工区失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
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
            string vcIsExportFlag = dataForm.vcIsExportFlag == null ? "" : dataForm.vcIsExportFlag;
            string dOrderHandleDate = dataForm.dOrderHandleDate == null ? "" : dataForm.dOrderHandleDate;
            string vcOrderNo = dataForm.vcOrderNo == null ? "" : dataForm.vcOrderNo;
            string vcPartNo = dataForm.vcPartNo == null ? "" : dataForm.vcPartNo;
            string vcInsideOutsideType = dataForm.vcInsideOutsideType == null ? "" : dataForm.vcInsideOutsideType;
            string vcNewOldFlag = dataForm.vcNewOldFlag == null ? "" : dataForm.vcNewOldFlag;
            string vcInjectionFactory = dataForm.vcInjectionFactory == null ? "" : dataForm.vcInjectionFactory;
            string vcSupplier_id = dataForm.vcSupplier_id == null ? "" : dataForm.vcSupplier_id;
            string vcWorkArea = dataForm.vcWorkArea == null ? "" : dataForm.vcWorkArea;
            string vcInjectionOrderNo = dataForm.vcInjectionOrderNo == null ? "" : dataForm.vcInjectionOrderNo;
            string dExpectReceiveDate = dataForm.dExpectReceiveDate == null ? "" : dataForm.dExpectReceiveDate;
            try
            {
                DataTable dt = fs0628_Logic.Search(vcIsExportFlag, dOrderHandleDate,vcOrderNo, vcPartNo, vcInsideOutsideType, vcNewOldFlag, vcInjectionFactory, vcSupplier_id, vcWorkArea, vcInjectionOrderNo, dExpectReceiveDate);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("dExpectReceiveDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dOrderHandleDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2802", ex, loginInfo.UserId);
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
            string vcIsExportFlag = dataForm.vcIsExportFlag == null ? "" : dataForm.vcIsExportFlag;
            string dOrderHandleDate = dataForm.dOrderHandleDate == null ? "" : dataForm.dOrderHandleDate;
            string vcOrderNo = dataForm.vcOrderNo == null ? "" : dataForm.vcOrderNo;
            string vcPartNo = dataForm.vcPartNo == null ? "" : dataForm.vcPartNo;
            string vcInsideOutsideType = dataForm.vcInsideOutsideType == null ? "" : dataForm.vcInsideOutsideType;
            string vcNewOldFlag = dataForm.vcNewOldFlag == null ? "" : dataForm.vcNewOldFlag;
            string vcInjectionFactory = dataForm.vcInjectionFactory == null ? "" : dataForm.vcInjectionFactory;
            string vcSupplier_id = dataForm.vcSupplier_id == null ? "" : dataForm.vcSupplier_id;
            string vcWorkArea = dataForm.vcWorkArea == null ? "" : dataForm.vcWorkArea;
            string vcInjectionOrderNo = dataForm.vcInjectionOrderNo == null ? "" : dataForm.vcInjectionOrderNo;
            string dExpectReceiveDate = dataForm.dExpectReceiveDate == null ? "" : dataForm.dExpectReceiveDate;
            try
            {
                DataTable dt = fs0628_Logic.Search(vcIsExportFlag, dOrderHandleDate, vcOrderNo, vcPartNo, vcInsideOutsideType, vcNewOldFlag, vcInjectionFactory, vcSupplier_id, vcWorkArea, vcInjectionOrderNo, dExpectReceiveDate);
                string[] head = new string[] { };
                string[] field = new string[] { };
                //[vcPartNo], [dBeginDate], [dEndDate]
                //const tHeader = ["订单处理日", "订单号", "品番", "内外", "号旧区分", "发注工厂", "受入", "供应商代码", "工区", "出荷场代码", "车型编码", "订货数量", "预计纳期", "订单回数", "发注订单号", "备注"];
                //const filterVal = ["dOrderHandleDate", "vcOrderNo", "vcPartNo", "vcInsideOutsideType", "vcNewOldFlag", "vcInjectionFactory", "vcDock", "vcSupplier_id", "vcWorkArea", "vcCHCCode", "vcCarType", "vcOrderNum", "dExpectReceiveDate", "vcOderTimes", "vcInjectionOrderNo", "vcMemo"];

                head = new string[] { "iAutoId", "进度状态管理", "订单处理日", "订单号", "品番", "内外", "号旧区分", "发注工场", "受入", "供应商代码", "工区", "出荷场", "车型编码", "订货数量", "预计纳期", "订单回数", "发注订单号", "备注" };
                field = new string[] { "iAutoId", "vcIsExportFlag", "dOrderHandleDate", "vcOrderNo", "vcPartNo", "vcInsideOutsideType", "vcNewOldFlag", "vcInjectionFactory", "vcDock", "vcSupplier_id", "vcWorkArea", "vcCHCCode", "vcCarType", "vcOrderNum", "dExpectReceiveDate", "vcOderTimes", "vcInjectionOrderNo", "vcMemo" };
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2803", ex, loginInfo.UserId);
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

                //开始数据验证
                if (hasFind)
                {
                    string[,] strField = new string[,] {{"订单处理日", "订单号", "品番", "内外", "号旧区分", "发注工厂", "受入", "供应商代码", "工区", "出荷场代码", "车型编码", "订货数量", "预计纳期", "订单回数", "发注订单号", "备注"},
                                                {"dOrderHandleDate", "vcOrderNo", "vcPartNo", "vcInsideOutsideType", "vcNewOldFlag", "vcInjectionFactory", "vcDock", "vcSupplier_id", "vcWorkArea", "vcCHCCode", "vcCarType", "vcOrderNum", "dExpectReceiveDate", "vcOderTimes", "vcInjectionOrderNo", "vcMemo"},
                                                {"",FieldCheck.NumCharLLL,FieldCheck.NumCharLLL,"","","","",FieldCheck.NumCharLLL,"","","",FieldCheck.Float,FieldCheck.Date,FieldCheck.Num,FieldCheck.NumCharLLL,"" },
                                                {"0","50","12","100","20","100","20","4","50","50","50","20","0","20","50","500"},//最大长度设定,不校验最大长度用0
                                                {"1","1","1","1","1","1","0","1","0","0","0","1","0","0","0","0"},//最小长度设定,可以为空用0
                                                {"1","2","3","4","5","6","7","8","9","10","11","12","13","14","15","16"}//前台显示列号，从0开始计算,注意有选择框的是0
                         };
                    //需要判断时间区间先后关系的字段
                    string[,] strDateRegion = { };
                    string[,] strSpecialCheck = { //例子-变更事项字段，当它为新设时，号旧必须为号口，旧型开始、旧型结束、旧型持续开始必须为空
                        
                          };

                    List<Object> checkRes = ListChecker.validateList(listInfoData, strField, strDateRegion, strSpecialCheck, true, "FS0607");
                    if (checkRes != null)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = checkRes;
                        apiResult.flag = Convert.ToInt32(ERROR_FLAG.单元格定位提示);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }

                string strErrorPartId = "";
                fs0628_Logic.Save(listInfoData, loginInfo.UserId, ref strErrorPartId);
                if (strErrorPartId != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "保存失败，以下供应商代码存在重叠：<br/>" + strErrorPartId;
                    apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2804", ex, loginInfo.UserId);
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
                fs0628_Logic.Del(listInfoData, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2805", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 导入进度管理 
        [HttpPost]
        [EnableCors("any")]
        public string importProgressApi([FromBody] dynamic data)
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
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (listInfoData[i]["vcIsExportFlag"].ToString() != "0")
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = listInfoData[i]["vcOrderNo"] + "的状态不正确,已经导入进度管理操作，不能进行重复操作！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                fs0628_Logic.ImportProgress(listInfoData, loginInfo.UserId,loginInfo.UnitCode);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2806", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导入进度管理操作失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region creatKBSApi
        [HttpPost]
        [EnableCors("any")]
        public string creatKBSApi([FromBody] dynamic data)
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
            JArray listInfo = dataForm.parentFormSelectItem;
            List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
            try
            {
                if (listInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请先进行数据检索，再进行生成N-KBS文件操作！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string filepath = CreatKBS(listInfoData,loginInfo.UnitCode,loginInfo.UserId);
               
                if (filepath == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "导出生成N-KBS文件失败";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = filepath;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2807", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "生成N-KBS文件失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }

        private string CreatKBS(List<Dictionary<string, object>> listInfoData,string career,string userId)
        {
            String realPath = string.Empty;
            realPath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "FS0628_KBS" + Path.DirectorySeparatorChar;
            string filename = "紧急订单_" + userId+"_" + System.DateTime.Now.ToString("yyyyMMddhhmmss") + ".txt";
            String dir = getDir(filename); // /f/e/d/c/4/9/8/4
            String path = realPath + dir; //D:\\products\3/f/e/d/c/4/9/8/4
            
            if (!Directory.Exists(realPath))
            {
                Directory.CreateDirectory(realPath);
            }

            if (!System.IO.File.Exists(realPath+filename))
            {
                FileStream stream = System.IO.File.Create(realPath + filename);
                stream.Close();
                stream.Dispose();
            }
            using (StreamWriter writer = new StreamWriter(realPath + filename, true))
            {
                for (int i=0;i< listInfoData.Count;i++)
                {
                    if (listInfoData[i]["vcInsideOutsideType"].ToString()=="外注")
                    {
                        writer.Write(listInfoData[i]["vcDock"].ToString()+ "\t");
                        writer.Write(listInfoData[i]["vcSupplier_id"].ToString() + "\t");
                        writer.Write(listInfoData[i]["vcWorkArea"].ToString() + "\t");
                        writer.Write(listInfoData[i]["vcCHCCode"].ToString() + "\t");
                        writer.Write(listInfoData[i]["dExpectReceiveDate"].ToString().Replace("/","")+ listInfoData[i]["vcOderTimes"].ToString()+"E1" + "\t\t");
                        writer.Write(listInfoData[i]["vcPartNo"].ToString() + "\t\t\t\t");
                        writer.WriteLine(listInfoData[i]["vcOrderNum"].ToString());
                    }
                }
                //清空缓冲区内容，并把缓冲区内容写入基础流 
                writer.Flush();
                //关闭写数据流 
                writer.Close();

            }
            return filename;
        }
        public String getDir(String name)
        {
            //任意一个对象都有一个hash码   131313213
            int i = name.GetHashCode();
            //将hash码转成16禁止的字符串
            //String hex = Integer.toHexString(i);
            string hex = String.Format("{0:X}", i);
            int j = hex.Length;
            for (int k = 0; k < 8 - j; k++)
            {
                hex = "0" + hex;
            }
            return Path.DirectorySeparatorChar + hex.Substring(0, 1) + Path.DirectorySeparatorChar + hex.Substring(1, 1) + Path.DirectorySeparatorChar + hex.Substring(2, 1) + Path.DirectorySeparatorChar + hex.Substring(3, 1) + Path.DirectorySeparatorChar + hex.Substring(4, 1) + Path.DirectorySeparatorChar + hex.Substring(5, 1) + Path.DirectorySeparatorChar + hex.Substring(6, 1) + Path.DirectorySeparatorChar + hex.Substring(7, 1);
        }
        #endregion

        #region 下载-txt,下载后会自动删除文件
        [HttpGet]
        [EnableCors("any")]
        public IActionResult downloadTxtApi(string path)
        {
            try
            {
                string fileSavePath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "FS0628_KBS" + Path.DirectorySeparatorChar;//模板目录，读取模板供用户下载
                var provider = new FileExtensionContentTypeProvider();
                FileInfo fileInfo = new FileInfo(fileSavePath + path);
                var ext = fileInfo.Extension;
                new FileExtensionContentTypeProvider().Mappings.TryGetValue(ext, out var contenttype);
                byte[] bt = System.IO.File.ReadAllBytes(fileSavePath + path);
                return File(bt, contenttype ?? "application/octet-stream", fileInfo.Name);
            }
            catch (Exception ex)
            {
                ContentResult result = new ContentResult();
                result.Content = "<script>alert('下载生成N-KBS文件失败,没有找到生成的文件！')</script>";
                result.ContentType = "text/html;charset=utf-8";
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2808", ex, "system");
                return result;
            }
        }
        #endregion
    }
}

