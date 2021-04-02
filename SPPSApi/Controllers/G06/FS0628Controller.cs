using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using Common;
using ICSharpCode.SharpZipLib.Zip;
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
                DataTable dtCarType = fs0628_Logic.GetCarType();

                List<Object> dataList_Supplier = ComFunction.convertToResult(dtSupplier, new string[] { "vcValue", "vcName" });
                List<Object> dataList_InjectionOrderNo = ComFunction.convertToResult(dtInjectionOrderNo, new string[] { "vcValue", "vcName" });
                List<Object> dataList_WorkArea = ComFunction.convertToResult(dtWorkArea, new string[] { "vcValue", "vcName" });
                List<Object> dataList_CarType = ComFunction.convertToResult(dtCarType, new string[] { "vcValue", "vcName" });

                res.Add("C000", dataList_C000);
                res.Add("CarType", dataList_CarType);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2802", ex, loginInfo.UserId);
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

            string strOrderHandleTime_from = "";
            string strOrderHandleTime_to = "";
            JArray listOrderHandleDate = dataForm.dOrderHandleDate;
            if (listOrderHandleDate != null && listOrderHandleDate.Count != 0)
            {
                strOrderHandleTime_from = listOrderHandleDate[0].ToString();
                strOrderHandleTime_to = listOrderHandleDate[1].ToString();
            }

            string vcIsExportFlag = dataForm.vcIsExportFlag == null ? "" : dataForm.vcIsExportFlag;
            //string dOrderHandleDate = dataForm.dOrderHandleDate == null ? "" : dataForm.dOrderHandleDate;
            string vcOrderNo = dataForm.vcOrderNo == null ? "" : dataForm.vcOrderNo;
            string vcPartNo = dataForm.vcPartNo == null ? "" : dataForm.vcPartNo;
            string vcInsideOutsideType = dataForm.vcInsideOutsideType == null ? "" : dataForm.vcInsideOutsideType;
            string vcNewOldFlag = dataForm.vcNewOldFlag == null ? "" : dataForm.vcNewOldFlag;
            string vcInjectionFactory = dataForm.vcInjectionFactory == null ? "" : dataForm.vcInjectionFactory;
            string vcSupplier_id = dataForm.vcSupplier_id == null ? "" : dataForm.vcSupplier_id;
            string vcWorkArea = dataForm.vcWorkArea == null ? "" : dataForm.vcWorkArea;
            string vcInjectionOrderNo = dataForm.vcInjectionOrderNo == null ? "" : dataForm.vcInjectionOrderNo;

            string strExpectReceiveleTime_from = "";
            string strExpectReceiveTime_to = "";
            JArray listExpectReceiveDate = dataForm.dExpectReceiveDate;
            if (listExpectReceiveDate != null && listExpectReceiveDate.Count != 0)
            {
                strExpectReceiveleTime_from = listExpectReceiveDate[0].ToString();
                strExpectReceiveTime_to = listExpectReceiveDate[1].ToString();
            }


            //string dExpectReceiveDate = dataForm.dExpectReceiveDate == null ? "" : dataForm.dExpectReceiveDate;
            string vcCarType= dataForm.vcCarType == null ? "" : dataForm.vcCarType;
            try
            {
                DataTable dt = fs0628_Logic.Search(vcIsExportFlag, strOrderHandleTime_from, strOrderHandleTime_to, vcOrderNo, vcPartNo, vcInsideOutsideType, vcNewOldFlag, vcInjectionFactory, vcSupplier_id, vcWorkArea, vcInjectionOrderNo, strExpectReceiveleTime_from, strExpectReceiveTime_to, vcCarType);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2803", ex, loginInfo.UserId);
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

            string strOrderHandleTime_from = "";
            string strOrderHandleTime_to = "";
            JArray listOrderHandleDate = dataForm.dOrderHandleDate;
            if (listOrderHandleDate != null && listOrderHandleDate.Count != 0)
            {
                strOrderHandleTime_from = listOrderHandleDate[0].ToString();
                strOrderHandleTime_to = listOrderHandleDate[1].ToString();
            }

            string vcIsExportFlag = dataForm.vcIsExportFlag == null ? "" : dataForm.vcIsExportFlag;
            //string dOrderHandleDate = dataForm.dOrderHandleDate == null ? "" : dataForm.dOrderHandleDate;
            string vcOrderNo = dataForm.vcOrderNo == null ? "" : dataForm.vcOrderNo;
            string vcPartNo = dataForm.vcPartNo == null ? "" : dataForm.vcPartNo;
            string vcInsideOutsideType = dataForm.vcInsideOutsideType == null ? "" : dataForm.vcInsideOutsideType;
            string vcNewOldFlag = dataForm.vcNewOldFlag == null ? "" : dataForm.vcNewOldFlag;
            string vcInjectionFactory = dataForm.vcInjectionFactory == null ? "" : dataForm.vcInjectionFactory;
            string vcSupplier_id = dataForm.vcSupplier_id == null ? "" : dataForm.vcSupplier_id;
            string vcWorkArea = dataForm.vcWorkArea == null ? "" : dataForm.vcWorkArea;
            string vcInjectionOrderNo = dataForm.vcInjectionOrderNo == null ? "" : dataForm.vcInjectionOrderNo;

            string strExpectReceiveleTime_from = "";
            string strExpectReceiveTime_to = "";
            JArray listExpectReceiveDate = dataForm.dExpectReceiveDate;
            if (listExpectReceiveDate != null && listExpectReceiveDate.Count != 0)
            {
                strExpectReceiveleTime_from = listExpectReceiveDate[0].ToString();
                strExpectReceiveTime_to = listExpectReceiveDate[1].ToString();
            }


            //string dExpectReceiveDate = dataForm.dExpectReceiveDate == null ? "" : dataForm.dExpectReceiveDate;
            string vcCarType = dataForm.vcCarType == null ? "" : dataForm.vcCarType;
            try
            {
                DataTable dt = fs0628_Logic.Search(vcIsExportFlag, strOrderHandleTime_from, strOrderHandleTime_to, vcOrderNo, vcPartNo, vcInsideOutsideType, vcNewOldFlag, vcInjectionFactory, vcSupplier_id, vcWorkArea, vcInjectionOrderNo, strExpectReceiveleTime_from, strExpectReceiveTime_to, vcCarType);
                string[] head = new string[] { };
                string[] field = new string[] { };
                //[vcPartNo], [dBeginDate], [dEndDate]
                //const tHeader = ["订单处理日", "订单号", "品番", "内外", "号旧区分", "发注工厂", "受入", "供应商代码", "工区", "出荷场代码", "车型编码", "订货数量", "预计纳期", "订单回数", "发注订单号", "备注"];
                //const filterVal = ["dOrderHandleDate", "vcOrderNo", "vcPartNo", "vcInsideOutsideType", "vcNewOldFlag", "vcInjectionFactory", "vcDock", "vcSupplier_id", "vcWorkArea", "vcCHCCode", "vcCarType", "vcOrderNum", "dExpectReceiveDate", "vcOderTimes", "vcInjectionOrderNo", "vcMemo"];

                head = new string[] { "iAutoId", "进度状态", "订单处理日", "订单号", "品番", "内外", "号旧区分", "发注工场", "受入", "供应商代码", "工区", "出荷场", "车型", "订货数量", "预计纳期", "订单回数", "发注订单号", "备注" };
                field = new string[] { "iAutoId", "vcIsExportFlag", "dOrderHandleDate", "vcOrderNo", "vcPartNo", "vcInsideOutsideType", "vcNewOldFlag", "vcInjectionFactory", "vcDock", "vcSupplier_id", "vcWorkArea", "vcCHCCode", "vcCarType", "vcOrderNum", "dExpectReceiveDate", "vcOderTimes", "vcInjectionOrderNo", "vcMemo" };
                string msg = string.Empty;
                //string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0309_Export.xlsx", 2, loginInfo.UserId, FunctionID);
                //string filepath = ComFunction.DataTableToExcel(head, field, dt, ".", loginInfo.UserId, FunctionID, ref msg);
                string filepath = ComFunction.generateExcelWithXlt(dt, field, _webHostEnvironment.ContentRootPath, "FS0628_Data.xlsx", 1, loginInfo.UserId, FunctionID, true);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2804", ex, loginInfo.UserId);
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
                    string[,] strField = new string[,] {{"订单处理日", "订单号", "品番", "内外", "号旧区分", "发注工场", "受入", "供应商代码", "工区", "出荷场", "车型", "订货数量", "预计纳期", "订单回数", "发注订单号", "备注"},
                                                {"dOrderHandleDate", "vcOrderNo", "vcPartNo", "vcInsideOutsideType", "vcNewOldFlag", "vcInjectionFactory", "vcDock", "vcSupplier_id", "vcWorkArea", "vcCHCCode", "vcCarType", "vcOrderNum", "dExpectReceiveDate", "vcOderTimes", "vcInjectionOrderNo", "vcMemo"},
                                                {"",FieldCheck.NumCharLLL,FieldCheck.NumCharLLL,"","","","",FieldCheck.NumCharLLL,"","","",FieldCheck.Float,FieldCheck.Date,FieldCheck.Num,FieldCheck.NumCharLLL,"" },
                                                {"0","50","12","100","20","100","20","4","50","50","50","20","0","20","50","500"},//最大长度设定,不校验最大长度用0
                                                {"0","0","12","1","1","1","0","4","0","0","0","1","0","0","0","0"},//最小长度设定,可以为空用0
                                                {"1","2","3","4","5","6","7","8","9","10","11","12","13","14","15","16"}//前台显示列号，从0开始计算,注意有选择框的是0
                         };
                    //需要判断时间区间先后关系的字段
                    string[,] strDateRegion = { };
                    string[,] strSpecialCheck = { //例子-变更事项字段，当它为新设时，号旧必须为号口，旧型开始、旧型结束、旧型持续开始必须为空
                        
                          };

                    List<Object> checkRes = ListChecker.validateList(listInfoData, strField, strDateRegion, strSpecialCheck, true, "FS0628");
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2805", ex, loginInfo.UserId);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2806", ex, loginInfo.UserId);
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
                JArray checkedInfo = dataForm;
                List<Dictionary<string, Object>> listInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();
                if (listInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选择一条数据！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (listInfoData[i]["vcIsExportFlag"].ToString() != "未导入")
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = listInfoData[i]["vcOrderNo"] + "的状态不正确,已经导入进度管理操作，不能进行重复操作！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (listInfoData[i]["vcInsideOutsideType"].ToString() != "外注")
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = listInfoData[i]["vcOrderNo"] + "的品番內制品,不能导入外采品！";
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2807", ex, loginInfo.UserId);
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
                DataTable dt = new DataTable();
                dt.Columns.Add("iAutoId");
                dt.Columns.Add("vcOrderNo");
                dt.Columns.Add("vcPartNo");
                dt.Columns.Add("vcInsideOutsideType");
                dt.Columns.Add("vcNewOldFlag");
                dt.Columns.Add("vcInjectionFactory");
                dt.Columns.Add("vcDock");
                dt.Columns.Add("vcSupplier_id");
                dt.Columns.Add("vcWorkArea");
                dt.Columns.Add("vcCHCCode");
                dt.Columns.Add("vcCarType");
                dt.Columns.Add("vcOrderNum");
                dt.Columns.Add("dExpectReceiveDate");
                dt.Columns.Add("vcOderTimes");

                for (int i = 0; i < listInfoData.Count; i++)
                {
                    DataRow dr = dt.NewRow();
                    dr["iAutoId"] = listInfoData[i]["iAutoId"] == null ? "" : listInfoData[i]["iAutoId"].ToString();
                    dr["vcOrderNo"] = listInfoData[i]["vcOrderNo"] == null ? "" : listInfoData[i]["vcOrderNo"].ToString();
                    dr["vcPartNo"] = listInfoData[i]["vcPartNo"] == null ? "" : listInfoData[i]["vcPartNo"].ToString();
                    dr["vcInsideOutsideType"] = listInfoData[i]["vcInsideOutsideType"] == null ? "" : listInfoData[i]["vcInsideOutsideType"].ToString();
                    dr["vcNewOldFlag"] = listInfoData[i]["vcNewOldFlag"] == null ? "" : listInfoData[i]["vcNewOldFlag"].ToString();
                    dr["vcInjectionFactory"] = listInfoData[i]["vcInjectionFactory"] == null ? "" : listInfoData[i]["vcInjectionFactory"].ToString();
                    dr["vcDock"] = listInfoData[i]["vcDock"] == null ? "" : listInfoData[i]["vcDock"].ToString();
                    dr["vcSupplier_id"] = listInfoData[i]["vcSupplier_id"] == null ? "" : listInfoData[i]["vcSupplier_id"].ToString();
                    dr["vcWorkArea"] = listInfoData[i]["vcWorkArea"] == null ? "" : listInfoData[i]["vcWorkArea"].ToString();
                    dr["vcCHCCode"] = listInfoData[i]["vcCHCCode"] == null ? "" : listInfoData[i]["vcCHCCode"].ToString();
                    dr["vcCarType"] = listInfoData[i]["vcCarType"] == null ? "" : listInfoData[i]["vcCarType"].ToString();
                    dr["vcOrderNum"] = listInfoData[i]["vcOrderNum"] == null ? "" : listInfoData[i]["vcOrderNum"].ToString();
                    dr["dExpectReceiveDate"] = listInfoData[i]["dExpectReceiveDate"] == null ? "" : listInfoData[i]["dExpectReceiveDate"].ToString();
                    dr["vcOderTimes"] = listInfoData[i]["vcOderTimes"] == null ? "" : listInfoData[i]["vcOderTimes"].ToString();
                    dt.Rows.Add(dr);
                }
                DataRow[] drArray = dt.Select("vcInsideOutsideType='外注'");
                if (drArray.Length==0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "待生成的数据无外注数据！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                DataTable dtWZ = drArray[0].Table.Clone(); // 复制DataRow的表结构
                string msg = string.Empty;
                foreach (DataRow dr in drArray)
                {
                    dtWZ.ImportRow(dr);
                }

                Dictionary<string, byte[]> filepathList = CreatKBS(dtWZ, loginInfo.UnitCode,loginInfo.UserId);

                if (filepathList.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "导出生成N-KBS文件失败";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string realPath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "FS0628_KBS" + Path.DirectorySeparatorChar;
                string zipName = "紧急订单_" + loginInfo.UserId + "_" + System.DateTime.Now.ToString("yyyyMMddhhmmss") + ".zip";
                if (filepathList.Count>1)
                {
                    using (FileStream zip = System.IO.File.Create(realPath+zipName))
                    {
                        using (ZipOutputStream zipStream = new ZipOutputStream(zip))
                        {
                            foreach (KeyValuePair<string, byte[]> kv in filepathList)
                            {
                                //压缩包内条目
                                ZipEntry entry = new ZipEntry(kv.Key);
                                //添加条目
                                zipStream.PutNextEntry(entry);
                                //设置压缩级别1~9
                                zipStream.SetLevel(5);
                                //写入
                                zipStream.Write(kv.Value, 0, kv.Value.Length);
                            }
                        }
                    }

                }
                //生成 发注订单号
                fs0628_Logic.creatInjectionOrderNo(dtWZ,loginInfo.UserId);

                apiResult.code = ComConstant.SUCCESS_CODE;
                if (filepathList.Count > 1)
                {
                    apiResult.data = zipName;
                } else
                {
                    foreach (KeyValuePair<string, byte[]> kv in filepathList)
                    { 
                        apiResult.data = kv.Key +".txt";
                    }
                }
                
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2808", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "生成N-KBS文件失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 创建压缩包
        /// </summary>
        /// <param name="zipName">压缩包名称（路径）</param>
        /// <param name="files">要压缩的文件，key-文件名，value-文件字节数组</param>
        private void CreateZipPackage(string zipName, Dictionary<string, byte[]> files)
        {
            using (FileStream zip = System.IO.File.Create(zipName))
            {
                using (ZipOutputStream zipStream = new ZipOutputStream(zip))
                {
                    foreach (KeyValuePair<string, byte[]> kv in files)
                    {
                        //压缩包内条目
                        ZipEntry entry = new ZipEntry(kv.Key);
                        //添加条目
                        zipStream.PutNextEntry(entry);
                        //设置压缩级别1~9
                        zipStream.SetLevel(5);
                        //写入
                        zipStream.Write(kv.Value, 0, kv.Value.Length);
                    }
                }
            }
        }

        private Dictionary<string, byte[]> CreatKBS(DataTable dtWZ, string career,string userId)
        {
            String realPath = string.Empty;
            //List<Object> fileList = new List<object>();
            Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();
            realPath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "FS0628_KBS" + Path.DirectorySeparatorChar;
            string[] columnArray = { "vcInjectionFactory" };
            DataView dtSelectView = dtWZ.DefaultView;
            DataTable dtSelect = dtSelectView.ToTable(true, columnArray);//去重后的dt 

            for (int i = 0; i < dtSelect.Rows.Count; i++)
            {
                string vcInjectionFactory = dtSelect.Rows[i]["vcInjectionFactory"].ToString();
                DataRow[] drArray = dtWZ.Select("vcInjectionFactory='" + vcInjectionFactory + "'");
                DataTable dtNewInjectionFactory = drArray[0].Table.Clone(); // 复制DataRow的表结构
                string msg = string.Empty;
                foreach (DataRow dr in drArray)
                {
                    dtNewInjectionFactory.ImportRow(dr);
                }

                string filename = vcInjectionFactory  + "_" + System.DateTime.Now.ToString("yyyyMMddhhmmss") + ".txt";
                String dir = getDir(filename); // /f/e/d/c/4/9/8/4
                String path = realPath + dir; //D:\\products\3/f/e/d/c/4/9/8/4

                if (!Directory.Exists(realPath))
                {
                    Directory.CreateDirectory(realPath);
                }

                if (!System.IO.File.Exists(realPath + filename))
                {
                    FileStream stream = System.IO.File.Create(realPath + filename);
                    stream.Close();
                    stream.Dispose();
                }
                using (StreamWriter writer = new StreamWriter(realPath + filename, true))
                {
                    for (int j = 0; j < dtNewInjectionFactory.Rows.Count; j++)
                    {
                        writer.Write(dtNewInjectionFactory.Rows[j]["vcDock"].ToString() + "\t");
                        writer.Write(dtNewInjectionFactory.Rows[j]["vcSupplier_id"].ToString() + "\t");
                        writer.Write(dtNewInjectionFactory.Rows[j]["vcWorkArea"].ToString() + "\t");
                        writer.Write(dtNewInjectionFactory.Rows[j]["vcCHCCode"].ToString() + "\t");
                        writer.Write(dtNewInjectionFactory.Rows[j]["dExpectReceiveDate"].ToString().Replace("/", "") + dtNewInjectionFactory.Rows[j]["vcOderTimes"].ToString()+ "\t\t");
                        writer.Write(dtNewInjectionFactory.Rows[j]["vcPartNo"].ToString() + "\t\t\t\t");
                        writer.WriteLine(dtNewInjectionFactory.Rows[j]["vcOrderNum"].ToString());
                    }
                    //清空缓冲区内容，并把缓冲区内容写入基础流 
                    writer.Flush();
                    //关闭写数据流 
                    writer.Close();

                }
                string key = filename.ToString().Trim().Substring(0, filename.ToString().Trim().LastIndexOf("."));
                files.Add(key, System.IO.File.ReadAllBytes(realPath + filename));
            }
            return files;
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

        #region 一括赋予
        [HttpPost]
        [EnableCors("any")]
        public string allInstallApi([FromBody] dynamic data)
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
                //以下开始业务处理
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                JArray listInfo = dataForm.parentFormSelectItem;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();

                string vcIntake = dataForm.allInstallForm.vcIntake == null ? "" : dataForm.allInstallForm.vcIntake;
                if (vcIntake.Length == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "一括赋予订单回数不能为空！,请确认！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (listInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请先选择数据，再进行一括赋予操作！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
               
                fs0628_Logic.allInstall(listInfoData, vcIntake, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2809", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "一括赋予失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}

