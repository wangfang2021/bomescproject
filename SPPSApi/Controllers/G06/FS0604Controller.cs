using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Text;
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
    [Route("api/FS0604/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0604Controller : BaseController
    {
        FS0604_Logic fs0604_Logic = new FS0604_Logic();
        private readonly string FunctionID = "FS0604";

        private readonly IWebHostEnvironment _webHostEnvironment;


        public FS0604Controller(IWebHostEnvironment webHostEnvironment)
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
                DataTable dtSupplier = fs0604_Logic.GetSupplier();
                DataTable dtCarType = fs0604_Logic.GetCarType();
                DataTable dtExpectDeliveryDate = fs0604_Logic.GetExpectDeliveryDate();
                DataTable dtWorkArea = fs0604_Logic.GetWorkArea();
                DataTable dt = fs0604_Logic.GetBoxType();//箱种的
                DataTable task = fs0604_Logic.GetTaskNum();//未发送的数据
                DataTable task1 = fs0604_Logic.GetTaskNum1();//待回复 含退回
                DataTable task2 = fs0604_Logic.GetTaskNum2();// 含退回
                DataTable dSendDate = fs0604_Logic.dSendDate();
                List<Object> dataList_BoxType = ComFunction.convertToResult(dt, new string[] { "vcValue", "vcName" });
                List<Object> dataList_CarType = ComFunction.convertToResult(dtCarType, new string[] { "vcValue", "vcName" });
                List<Object> dataList_Supplier = ComFunction.convertToResult(dtSupplier, new string[] { "vcValue", "vcName" });
                List<Object> dataList_ExpectDeliveryDate = ComFunction.convertToResult(dtExpectDeliveryDate, new string[] { "vcValue", "vcName" });
                List<Object> dataList_WorkArea = ComFunction.convertToResult(dtWorkArea, new string[] { "vcValue", "vcName" });
                List<Object> dataList_SendDate = ComFunction.convertToResult(dSendDate, new string[] { "vcValue", "vcName" });

                List<Object> dataList_C033 = ComFunction.convertAllToResult(ComFunction.getTCode("C033"));//荷姿状态

                res.Add("CarType", dataList_CarType);
                res.Add("BoxType", dataList_BoxType);
                res.Add("C033", dataList_C033);
                res.Add("Supplier", dataList_Supplier);
                res.Add("ExpectDeliveryDate", dataList_ExpectDeliveryDate);
                res.Add("SendDate", dataList_SendDate);
                res.Add("WorkArea", dataList_WorkArea);
                res.Add("taskNum", task.Rows.Count);
                res.Add("taskNum1", task1.Rows.Count);
                res.Add("taskNum2", task2.Rows.Count);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0401", ex, loginInfo.UserId);
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
                DataTable dtWorkAreaBySupplier = fs0604_Logic.GetWorkAreaBySupplier(supplierCode);
              
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
            string dSynchronizationDateFrom = dataForm.dSynchronizationDateFrom == null ? "" : dataForm.dSynchronizationDateFrom;
            string dSynchronizationDateTo = dataForm.dSynchronizationDateTo == null ? "" : dataForm.dSynchronizationDateTo;
            string dSynchronizationDate = dataForm.dSynchronizationDate == null ? "" : dataForm.dSynchronizationDate;
            string vcState = dataForm.vcState == null ? "" : dataForm.vcState;
            string vcPartNo = dataForm.vcPartNo == null ? "" : dataForm.vcPartNo;
            string vcSupplier_id = dataForm.vcSupplier_id == null ? "" : dataForm.vcSupplier_id;
            string vcWorkArea = dataForm.vcWorkArea == null ? "" : dataForm.vcWorkArea;
            string vcCarType = dataForm.vcCarType == null ? "" : dataForm.vcCarType;
            string dExpectDeliveryDate = dataForm.dExpectDeliveryDate == null ? "" : dataForm.dExpectDeliveryDate;
            string dSendDate = dataForm.dSendDate == null ? "" : dataForm.dSendDate;
            string vcOEOrSP = dataForm.vcOEOrSP == null ? "" : dataForm.vcOEOrSP;
            string vcBoxType = dataForm.vcBoxType == null ? "" : dataForm.vcBoxType; 

            try
            {
                DataTable dt = fs0604_Logic.Search(dSynchronizationDateFrom, dSynchronizationDateTo,dSynchronizationDate, vcState, vcPartNo, vcSupplier_id, vcWorkArea, vcCarType, dExpectDeliveryDate, vcOEOrSP, vcBoxType, dSendDate);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("dSynchronizationDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dExpectDeliveryDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dUseStartDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dUserEndDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dOrderReceiveDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dSendDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dReplyDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dAdmitDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dWeaveDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0402", ex, loginInfo.UserId);
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
            string dSynchronizationDateFrom = dataForm.dSynchronizationDateFrom == null ? "" : dataForm.dSynchronizationDateFrom;
            string dSynchronizationDateTo = dataForm.dSynchronizationDateTo == null ? "" : dataForm.dSynchronizationDateTo;
            string dSynchronizationDate = dataForm.dSynchronizationDate == null ? "" : dataForm.dSynchronizationDate;
            string vcState = dataForm.vcState == null ? "" : dataForm.vcState;
            string vcPartNo = dataForm.vcPartNo == null ? "" : dataForm.vcPartNo;
            string vcSupplier_id = dataForm.vcSupplier_id == null ? "" : dataForm.vcSupplier_id;
            string vcWorkArea = dataForm.vcWorkArea == null ? "" : dataForm.vcWorkArea;
            string vcCarType = dataForm.vcCarType == null ? "" : dataForm.vcCarType;
            string dExpectDeliveryDate = dataForm.dExpectDeliveryDate == null ? "" : dataForm.dExpectDeliveryDate;
            string vcOEOrSP = dataForm.vcOEOrSP == null ? "" : dataForm.vcOEOrSP;
            string vcBoxType = dataForm.vcBoxType == null ? "" : dataForm.vcBoxType;
            string dSendDate = dataForm.dSendDate == null ? "" : dataForm.dSendDate;

            try
            {
                DataTable dt = fs0604_Logic.Search(dSynchronizationDateFrom, dSynchronizationDateTo, dSynchronizationDate, vcState, vcPartNo, vcSupplier_id, vcWorkArea, vcCarType, dExpectDeliveryDate, vcOEOrSP, vcBoxType, dSendDate);
                string[] head = new string[] { };
                string[] field = new string[] { };
                //[vcPartNo], [dBeginDate], [dEndDate]"使用结束时间", "dUserEndDate",
                //状态 - 展开时间 - 要望纳期 - 同步时间 - 包装工场 - 收货方 - 品番 - 品名 
                //- 车型 - 使用开始时间 - OE = SP - 供应商代码 - 工区 - 要望收容数 - 收容数 
                //- 箱最大收容数 - 箱种 - 长宽高 - 空箱重量 - 单品净重 - 照片 - 回复时间 - 承认时间 - 原单位织入时间 - 备注
                head = new string[] { "状态","展开时间","要望纳期","同步时间", "包装工场", "收货方",   "品番", "品名", "车型", "使用开始时间",  "OE=SP", "供应商代码", "工区",  "要望收容数", "收容数", "箱最大收容数", "箱种", "长(mm)", "宽(mm)", "高(mm)", "空箱重量(g)", "单品净重(g)",  "回复时间", "承认时间", "原单位织入时间", "备注" };
                field = new string[] {  "vcState","dSendDate","dExpectDeliveryDate", "dSynchronizationDate","vcPackingPlant", "vcReceiver",  "vcPartNo", "vcPartName", "vcCarType", "dUseStartDate", "vcOEOrSP", "vcSupplier_id", "vcWorkArea", "vcExpectIntake", "vcIntake", "vcBoxMaxIntake", "vcBoxType", "vcLength", "vcWide", "vcHeight", "vcEmptyWeight", "vcUnitNetWeight",  "dReplyDate", "dAdmitDate", "dWeaveDate", "vcMemo" };
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0403", ex, loginInfo.UserId);
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
                string msg = string.Empty;
                //开始数据验证
                if (hasFind)
                {
                    string[,] strField = new string[,] {{"状态","展开时间","要望纳期","同步时间",  "品番","品名", "车型", "使用开始时间",  "OE=SP", "供应商代码", "工区",  "要望收容数", "收容数", "箱最大收容数", "箱种", "长(mm)", "宽(mm)", "高(mm)", "空箱重量(g)", "单品净重(g)", "照片","回复时间","承认时间","原单位织入时间","备注"},
                                                {"vcState","dSendDate", "dExpectDeliveryDate", "dSynchronizationDate",  "vcPartNo","vcPartName", "vcCarType", "dUseStartDate", "vcOEOrSP", "vcSupplier_id", "vcWorkArea",  "vcExpectIntake", "vcIntake", "vcBoxMaxIntake", "vcBoxType", "vcLength", "vcWide", "vcHeight", "vcEmptyWeight", "vcUnitNetWeight","vcImageRoutes", "dReplyDate", "dAdmitDate", "dWeaveDate", "vcMemo"},
                                                {"","","","",FieldCheck.NumChar,"","","","",FieldCheck.NumChar,"","","","",FieldCheck.NumChar, "","","","","","","","","",""},
                                                {"5","0","0","0","12","200","50","0","0","4","50","20","20","20","50","20","20","20","20","20","0","0","0","0","500"},//最大长度设定,不校验最大长度用0
                                                {"1","0","0","0","1","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0"},//最小长度设定,可以为空用0
                                                {"1","2","3","4","5","6","7","8","9","10","11","12","13","14","15","16","17","18","19","20","21","22","23","24","25"}//前台显示列号，从0开始计算,注意有选择框的是0
                         };
                    //需要判断时间区间先后关系的字段
                    string[,] strDateRegion = { };
                    string[,] strSpecialCheck = { //例子-变更事项字段，当它为新设时，号旧必须为号口，旧型开始、旧型结束、旧型持续开始必须为空
                        
                          };

                    List<Object> checkRes = ListChecker.validateList(listInfoData, strField, strDateRegion, strSpecialCheck, true, "FS0604");
                    if (checkRes != null)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = checkRes;
                        apiResult.flag = Convert.ToInt32(ERROR_FLAG.单元格定位提示);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                // 追加新增品番的是否存在基础信息
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    if (bAddFlag == true)
                    {//新增
                        string vcPartNo = listInfoData[i]["vcPartNo"].ToString();
                        DataTable dtExist = fs0604_Logic.checkIsExistByPartNo(vcPartNo);
                        if (dtExist.Rows.Count==0) {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "新增的品番" + vcPartNo+"原单位表不存在!";
                            apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                    }
                }
                string strErrorPartId = "";
                fs0604_Logic.Save(listInfoData, loginInfo.UserId, ref strErrorPartId);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0404", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }

        }
        #endregion
        #region 荷姿展开
        [HttpPost]
        [EnableCors("any")]
        public string hZZKApi([FromBody] dynamic data)
        {
            //验证是否登录
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            ApiResult apiResult = new ApiResult();
            dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
            JArray listInfo = dataForm.parentFormSelectItem.multipleSelection;
            List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
            string dExpectDeliveryDate = dataForm.hzzkForm.dExpectDeliveryDate == null ? "" : dataForm.hzzkForm.dExpectDeliveryDate;
            try
            {
                //以下开始业务处理
               
                if (dExpectDeliveryDate.Length == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "一括赋予要望纳期不能全为空！,请确认！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (listInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请先进行数据检索，再进行一括赋予操作！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (listInfoData[i]["vcState"].ToString() != "未发送")
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = listInfoData[i]["vcPartNo"] + "状态不正确,必须是未发送，才能进行荷姿展开操作！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (listInfoData[i]["vcExpectIntake"].ToString().Trim().Length==0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "请填写品番"+listInfoData[i]["vcPartNo"] + "的要望收容数，才能进行荷姿展开操作！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                //构建 Datablt
                DataTable dt = new DataTable();
                //"dExportDate", "", "", "", ""
                dt.Columns.Add("iAutoId");
                dt.Columns.Add("vcSupplier_id");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    DataRow dr = dt.NewRow();
                    dr["iAutoId"] = listInfoData[i]["iAutoId"] == null ? "" : listInfoData[i]["iAutoId"].ToString();
                    dr["vcSupplier_id"] = listInfoData[i]["vcSupplier_id"] == null ? "" : listInfoData[i]["vcSupplier_id"].ToString();
                    dt.Rows.Add(dr);
                }
                string[] columnArray = { "vcSupplier_id" };
                DataView dtSelectView = dt.DefaultView;
                DataTable dtSelect = dtSelectView.ToTable(true, columnArray);//去重后的dt 
                bool bReault = true;
                FS0603_Logic fs0603_Logic = new FS0603_Logic();
                DataTable dtMessage = fs0603_Logic.createTable("MES");
                for (int i=0;i<dtSelect.Rows.Count;i++)
                {
                    string strSupplier = dtSelect.Rows[i]["vcSupplier_id"].ToString();
                    DataTable dtCheckSupplier =  fs0604_Logic.CheckEmail(strSupplier);
                    if (dtCheckSupplier.Rows.Count == 0)
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = "供应商" + strSupplier + "在供应商表中信息不存在，请维护信息及其邮箱";
                        dtMessage.Rows.Add(dataRow);
                        bReault = false;
                    }
                    else
                    {
                        bool isYouXiaoEmail = false;
                        for (int j=0;j<dtCheckSupplier.Rows.Count;j++)
                        {
                            if (dtCheckSupplier.Rows[j]["vcEmail1"].ToString()!=""|| dtCheckSupplier.Rows[j]["vcEmail2"].ToString() != ""|| dtCheckSupplier.Rows[j]["vcEmail3"].ToString() != "")
                            {
                                isYouXiaoEmail = true;
                                break;
                            }
                        }
                        if (!isYouXiaoEmail)
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = "供应商" + strSupplier + "在供应商表中邮箱信息不存在，请维护邮箱";
                            dtMessage.Rows.Add(dataRow);
                            bReault = false;
                        }
                    }
                }
                if (!bReault)
                {
                    //弹出错误dtMessage
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                bool bReaultEmail = true;
                DataTable dtMessageEmail = fs0603_Logic.createTable("MES");
                for (int i = 0; i < dtSelect.Rows.Count; i++)
                {
                    string strSupplier = dtSelect.Rows[i]["vcSupplier_id"].ToString();
                    DataTable dtCheckSupplier = fs0604_Logic.CheckEmail(strSupplier);
                    DataRow[] drArray = dt.Select("vcSupplier_id='" + strSupplier + "' ");
                    DataTable dtNewSupplierand = drArray[0].Table.Clone(); // 复制DataRow的表结构
                    string msg = string.Empty;
                    foreach (DataRow dr in drArray)
                    {
                        dtNewSupplierand.ImportRow(dr);
                    }
                    fs0604_Logic.hZZK(dtNewSupplierand, dExpectDeliveryDate, loginInfo.UserId);
                    //收件人dt
                    DataTable receiverDt = new DataTable();
                    receiverDt.Columns.Add("address");
                    receiverDt.Columns.Add("displayName");
                    for (int j = 0; j < dtCheckSupplier.Rows.Count; j++)
                    {
                        if (dtCheckSupplier.Rows[j]["vcEmail1"].ToString().Length > 0)
                        {
                            DataRow dr = receiverDt.NewRow();
                            dr["address"] = dtCheckSupplier.Rows[j]["vcEmail1"].ToString();
                            dr["displayName"] = dtCheckSupplier.Rows[j]["vcLinkMan1"].ToString();
                            receiverDt.Rows.Add(dr);
                        }
                        if (dtCheckSupplier.Rows[j]["vcEmail2"].ToString().Length > 0)
                        {
                            DataRow dr = receiverDt.NewRow();
                            dr["address"] = dtCheckSupplier.Rows[j]["vcEmail2"].ToString();
                            dr["displayName"] = dtCheckSupplier.Rows[j]["vcLinkMan2"].ToString();
                            receiverDt.Rows.Add(dr);
                        }
                        if (dtCheckSupplier.Rows[j]["vcEmail3"].ToString().Length > 0)
                        {
                            DataRow dr = receiverDt.NewRow();
                            dr["address"] = dtCheckSupplier.Rows[j]["vcEmail3"].ToString();
                            dr["displayName"] = dtCheckSupplier.Rows[j]["vcLinkMan3"].ToString();
                            receiverDt.Rows.Add(dr);
                        }
                    }
                    //邮件主题
                    string strSubject = "荷姿展开邮件";
                    //邮件内容
                    string strEmailBody = "";
                    strEmailBody += "<div style='font-family:宋体;font-size:12'>各位供应商 殿：大家好 <br /><br />";
                    strEmailBody += loginInfo.UnitCode +"补给 " + loginInfo.UserName + "<br />";
                    strEmailBody += "  感谢大家一直以来对"+ loginInfo.UnitCode + "补给业务的协力！<br /><br />";
                    strEmailBody += "  一丰补给管理系统】上传了贵司新设补给品荷姿确认，拜托贵司进行检讨，<br />";
                    strEmailBody += "  一丰补给管理系统】上传了贵司新设补给品荷姿确认，拜托贵司进行检讨，<br />";
                    strEmailBody += "  请填写完整后，于<span style='font-family:宋体;font-size:12;color:red'>" + dExpectDeliveryDate + "日前在系统上给予回复</span>，谢谢！<br /><br /></div>";
                    string result = "Success";
                    result = ComFunction.SendEmailInfo(loginInfo.Email, loginInfo.UnitName, strEmailBody, receiverDt, null, strSubject, "", false);
                    if (result == "Success")
                    {
                    }
                    else
                    {
                        DataRow dataRowEmail = dtMessageEmail.NewRow();
                        dataRowEmail["vcMessage"] = "供应商" + strSupplier + "发送邮件失败";
                        dtMessageEmail.Rows.Add(dataRowEmail);
                        bReaultEmail = false;
                    }
                }
                if (!bReaultEmail)
                {
                    //弹出错误dtMessage
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessageEmail;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0405", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "荷姿展开操作失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 承认
        [HttpPost]
        [EnableCors("any")]
        public string admitApi([FromBody] dynamic data)
        {
            //验证是否登录
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            ApiResult apiResult = new ApiResult();
            try
            {
                //以下开始业务处理
                //以下开始业务处理

                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                JArray listInfo = dataForm;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();

                if (listInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请先选中数据，再进行承认操作！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (listInfoData[i]["vcState"].ToString() != "已回复")
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = listInfoData[i]["vcPartNo"] + "状态不正确,必须是已回复，才能进行承认操作！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                fs0604_Logic.admit(listInfoData, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0406", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "承认操作失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
        #region 退回
        [HttpPost]
        [EnableCors("any")]
        public string returnHandleApi([FromBody] dynamic data)
        {
            //验证是否登录
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            ApiResult apiResult = new ApiResult();
            try
            {
                //以下开始业务处理
                //以下开始业务处理

                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                JArray listInfo = dataForm;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();

                if (listInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请先选中数据，再进行承认操作！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (listInfoData[i]["vcState"].ToString() != "已回复")
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = listInfoData[i]["vcPartNo"] + "状态不正确,必须是已回复，才能进行退回操作！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                fs0604_Logic.returnHandle(listInfoData, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0407", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "退回操作失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 织入原单位
        [HttpPost]
        [EnableCors("any")]
        public string weaveHandleApi([FromBody] dynamic data)
        {
            //验证是否登录
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            ApiResult apiResult = new ApiResult();
            try
            {
                //以下开始业务处理
                //以下开始业务处理

                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                JArray listInfo = dataForm;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();

                if (listInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请先选中数据，再进行织入操作！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //织入操作的前提是已承认状态
                //vcValue vcName 0   未发送1 待回复2   已回复3 已承认4   退回5 已织入
                for (var i=0;i<listInfoData.Count;i++)
                {
                    if (listInfoData[i]["vcState"].ToString() != "已承认")
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = listInfoData[i]["vcPartNo"] + "状态不正确,必须是已承认，才能进行织入操作！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                FS0603_Logic fs0603_Logic = new FS0603_Logic();
                DataTable dtMessage = fs0603_Logic.createTable("MES");
                bool bReault = true;
                fs0604_Logic.weaveHandle(listInfoData, loginInfo.UserId, ref bReault, ref dtMessage);
                if (!bReault)
                {
                    //弹出错误dtMessage
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0408", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "织入操作失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
        #region 手动织入原单位
        [HttpPost]
        [EnableCors("any")]
        public string sdweaveHandleApi([FromBody] dynamic data)
        {
            //验证是否登录
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            ApiResult apiResult = new ApiResult();
            try
            {
                //以下开始业务处理
                //以下开始业务处理

                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                JArray listInfo = dataForm;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();

                if (listInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请先选中数据，再进行手动织入操作！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //织入操作的前提是已承认状态
                //vcValue vcName 0   未发送1 待回复2   已回复3 已承认4   退回5 已织入
                //for (var i = 0; i < listInfoData.Count; i++)
                //{
                //    if (listInfoData[i]["vcState"].ToString() != "已承认")
                //    {
                //        apiResult.code = ComConstant.ERROR_CODE;
                //        apiResult.data = listInfoData[i]["vcPartNo"] + "状态不正确,必须是已承认，才能进行织入操作！";
                //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                //    }
                //}
                fs0604_Logic.sdweaveHandle(listInfoData, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0408", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "织入操作失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
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
                if (vcIntake.Length == 0 )
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "一括赋予要望收容数不能全为空！,请确认！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (listInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请先进行数据检索，再进行一括赋予操作！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                for (var i = 0; i < listInfoData.Count; i++)
                {
                    if (listInfoData[i]["vcState"].ToString() != "未发送")
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = listInfoData[i]["vcPartNo"] + "状态不正确,必须是未发送，才能进行一括赋予操作！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                fs0604_Logic.allInstall(listInfoData, vcIntake, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0409", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "一括赋予失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
        
       
    }
}

