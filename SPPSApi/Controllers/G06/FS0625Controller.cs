using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
    [Route("api/FS0625/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0625Controller : BaseController
    {
        FS0625_Logic fs0625_Logic = new FS0625_Logic();
        private readonly string FunctionID = "FS0625";

        private readonly IWebHostEnvironment _webHostEnvironment;


        public FS0625Controller(IWebHostEnvironment webHostEnvironment)
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
                DataTable dt = fs0625_Logic.GetPurposes();//号试目的
                List<Object> dataList_Purposes = ComFunction.convertToResult(dt, new string[] { "vcCodeId", "vcCodeName" });

                List<Object> dataList_C003 = ComFunction.convertAllToResult(ComFunction.getTCode("C003"));//内外区分

                res.Add("Purposes", dataList_Purposes);
                res.Add("C003", dataList_C003);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2501", ex, loginInfo.UserId);
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

            string dExportDate = dataForm.dExportDate == null ? "" : dataForm.dExportDate;
            string vcCarType = dataForm.vcCarType == null ? "" : dataForm.vcCarType;
            string vcPartNo = dataForm.vcPartNo == null ? "" : dataForm.vcPartNo;
            string vcInsideOutsideType = dataForm.vcInsideOutsideType == null ? "" : dataForm.vcInsideOutsideType;
            string vcSupplier_id = dataForm.vcSupplier_id == null ? "" : dataForm.vcSupplier_id;
            string vcWorkArea = dataForm.vcWorkArea == null ? "" : dataForm.vcWorkArea;
            string vcIsNewRulesFlag = dataForm.vcIsNewRulesFlag == null ? "" : dataForm.vcIsNewRulesFlag;
            string vcPurposes = dataForm.vcPurposes == null ? "" : dataForm.vcPurposes;

            try
            {
                DataTable dt = fs0625_Logic.Search(dExportDate, vcCarType, vcPartNo, vcInsideOutsideType, vcSupplier_id, vcWorkArea, vcIsNewRulesFlag, vcPurposes);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("dExportDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dOrderPurposesDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dOrderReceiveDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dActualReceiveDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dAccountOrderReceiveDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dOrderSendDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dOperatorTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2502", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
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

                //const tHeader = ["导入时间", "车型", "品番", "品名", "内外", "供应商代码", "工区", "是否新规", "OE=SP", "受入", "号试数量", "号试目的", "订单预计发行日", "订单预计纳入日", "纳入便次", "实际纳入日", "结算订单号", "结算订单验收日期", "号试订单验收日期", "备注"];
                //const filterVal = ["dExportDate", "vcCarType", "vcPartNo", "vcPartName", "vcInsideOutsideType", "vcSupplier_id", "vcWorkArea", "vcIsNewRulesFlag", "vcOEOrSP", "vcDock", "vcNumber", "vcPurposes", "dOrderPurposesDate", "dOrderReceiveDate", "vcReceiveTimes", "dActualReceiveDate", "vcAccountOrderNo", "dAccountOrderReceiveDate", "dOrderSendDate", "vcMemo"];

                //开始数据验证
                if (hasFind)
                {
                    string[,] strField = new string[,] {{"导入时间", "车型", "品番", "品名", "内外", "供应商代码", "工区", "是否新规", "OE=SP", "受入", "号试数量", "号试目的", "订单预计发行日", "订单预计纳入日", "纳入便次", "实际纳入日", "结算订单号", "结算订单验收日期", "号试订单验收日期", "备注"},
                                                {"dExportDate", "vcCarType", "vcPartNo", "vcPartName", "vcInsideOutsideType", "vcSupplier_id", "vcWorkArea", "vcIsNewRulesFlag", "vcOEOrSP", "vcDock", "vcNumber", "vcPurposes", "dOrderPurposesDate", "dOrderReceiveDate", "vcReceiveTimes", "dActualReceiveDate", "vcAccountOrderNo", "dAccountOrderReceiveDate", "dOrderSendDate", "vcMemo"},
                                                {"","","","","","","","","","","","","","","","" ,"","","",""},
                                                {"0","50","12","200","100","4","50","50","200","20","20","300","0","0","20","0","50","0","0","500"},//最大长度设定,不校验最大长度用0
                                                {"0","0","1","1","0","1","1","0","0","0","1","0","0","0","0","0","0","0","0","0"},//最小长度设定,可以为空用0
                                                {"1","2","3","4","5","6","7","8","9","10","11","12","13","14","15","16","17","18","19","20"}//前台显示列号，从0开始计算,注意有选择框的是0
                         };
                    //需要判断时间区间先后关系的字段
                    string[,] strDateRegion = { };
                    string[,] strSpecialCheck = { //例子-变更事项字段，当它为新设时，号旧必须为号口，旧型开始、旧型结束、旧型持续开始必须为空
                        
                          };

                    List<Object> checkRes = ListChecker.validateList(listInfoData, strField, strDateRegion, strSpecialCheck, true, "FS0625");
                    if (checkRes != null)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = checkRes;
                        apiResult.flag = Convert.ToInt32(ERROR_FLAG.单元格定位提示);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }

                string strErrorPartId = "";
                fs0625_Logic.Save(listInfoData, loginInfo.UserId, ref strErrorPartId);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2503", ex, loginInfo.UserId);
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
                fs0625_Logic.Del(listInfoData, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2504", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除失败";
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
            //以下开始业务处理
            ApiResult apiResult = new ApiResult();
            dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
            JArray listInfo = dataForm.parentFormSelectItem;
            List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();

            string dAccountOrderReceiveDate = dataForm.allInstallForm.dAccountOrderReceiveDate == null ? "" : dataForm.allInstallForm.dAccountOrderReceiveDate;
            string vcAccountOrderNo = dataForm.allInstallForm.vcAccountOrderNo == null ? "" : dataForm.allInstallForm.vcAccountOrderNo;
            try
            {
                if (dAccountOrderReceiveDate.Length == 0 && vcAccountOrderNo.Length == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "一括赋予结算订单号和结算订单验收日期不能全为空！,请确认！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (listInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请先进行数据检索，再进行一括赋予操作！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                fs0625_Logic.allInstall(listInfoData, dAccountOrderReceiveDate, vcAccountOrderNo, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2606", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "一括赋予失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 号试订单邮件发送
        [HttpPost]
        [EnableCors("any")]
        public string sendMailApi([FromBody] dynamic data)
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
                JArray listInfo = dataForm;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
                if (listInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请先检索数据，再进行年检邮件电送！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                DataTable dt = new DataTable();
                //"dExportDate", "", "", "", ""
                dt.Columns.Add("dExportDate");
                dt.Columns.Add("vcCarType");
                dt.Columns.Add("vcPartNo");
                dt.Columns.Add("vcPartName");
                dt.Columns.Add("vcInsideOutsideType");
                dt.Columns.Add("vcSupplier_id");
                dt.Columns.Add("vcWorkArea");
                dt.Columns.Add("vcIsNewRulesFlag");
                dt.Columns.Add("vcOEOrSP");
                dt.Columns.Add("vcDock");
                dt.Columns.Add("vcNumber"); 
                dt.Columns.Add("vcPurposes");
                dt.Columns.Add("dOrderPurposesDate");
                dt.Columns.Add("dOrderReceiveDate");
                dt.Columns.Add("vcReceiveTimes");
                dt.Columns.Add("dActualReceiveDate"); 
                dt.Columns.Add("vcAccountOrderNo");
                dt.Columns.Add("dAccountOrderReceiveDate");
                dt.Columns.Add("vcMemo");
              
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    DataRow dr = dt.NewRow();
                    dr["dExportDate"] = listInfoData[i]["dExportDate"] == null ? "" : listInfoData[i]["dExportDate"].ToString();
                    dr["vcCarType"] = listInfoData[i]["vcCarType"] == null ? "" : listInfoData[i]["vcCarType"].ToString();
                    dr["vcPartNo"] = listInfoData[i]["vcPartNo"] == null ? "" : listInfoData[i]["vcPartNo"].ToString();
                    dr["vcPartName"] = listInfoData[i]["vcPartName"] == null ? "" : listInfoData[i]["vcPartName"].ToString();
                    
                    string vcInsideOutsideType = listInfoData[i]["vcInsideOutsideType"] == null ? "" : listInfoData[i]["vcInsideOutsideType"].ToString();
                    if (vcInsideOutsideType=="1")
                    {
                        dr["vcInsideOutsideType"] = "外注";
                    } else if (vcInsideOutsideType == "0")
                    {
                        dr["vcInsideOutsideType"] = "内制";
                    } else
                    {
                        dr["vcInsideOutsideType"] = "";
                    }
                    dr["vcSupplier_id"] = listInfoData[i]["vcSupplier_id"] == null ? "" : listInfoData[i]["vcSupplier_id"].ToString();
                    dr["vcWorkArea"] = listInfoData[i]["vcWorkArea"] == null ? "" : listInfoData[i]["vcWorkArea"].ToString();
                    string vcIsNewRulesFlag = listInfoData[i]["vcIsNewRulesFlag"] == null ? "" : listInfoData[i]["vcIsNewRulesFlag"].ToString();
                    if (vcIsNewRulesFlag == "1")
                    {
                        dr["vcIsNewRulesFlag"] = "是";
                    }
                    else if (vcIsNewRulesFlag == "0")
                    {
                        dr["vcIsNewRulesFlag"] = "否";
                    }
                    else
                    {
                        dr["vcIsNewRulesFlag"] = "";
                    }

                    dr["vcOEOrSP"] = listInfoData[i]["vcOEOrSP"] == null ? "" : listInfoData[i]["vcOEOrSP"].ToString();
                    dr["vcDock"] = listInfoData[i]["vcDock"] == null ? "" : listInfoData[i]["vcDock"].ToString();
                    dr["vcNumber"] = listInfoData[i]["vcNumber"] == null ? "" : listInfoData[i]["vcNumber"].ToString();
                    dr["vcPurposes"] = listInfoData[i]["vcPurposes"] == null ? "" : listInfoData[i]["vcPurposes"].ToString();
                    dr["dOrderPurposesDate"] = listInfoData[i]["dOrderPurposesDate"] == null ? "" : listInfoData[i]["dOrderPurposesDate"].ToString();
                    dr["dOrderReceiveDate"] = listInfoData[i]["dOrderReceiveDate"] == null ? "" : listInfoData[i]["dOrderReceiveDate"].ToString();
                    dr["vcReceiveTimes"] = listInfoData[i]["vcReceiveTimes"] == null ? "" : listInfoData[i]["vcReceiveTimes"].ToString();
                    dr["dActualReceiveDate"] = listInfoData[i]["dActualReceiveDate"] == null ? "" : listInfoData[i]["dActualReceiveDate"].ToString();
                    dr["vcAccountOrderNo"] = listInfoData[i]["vcAccountOrderNo"] == null ? "" : listInfoData[i]["vcAccountOrderNo"].ToString();
                    dr["dAccountOrderReceiveDate"] = listInfoData[i]["dAccountOrderReceiveDate"] == null ? "" : listInfoData[i]["dAccountOrderReceiveDate"].ToString();
                    dr["vcMemo"] = listInfoData[i]["vcMemo"] == null ? "" : listInfoData[i]["vcMemo"].ToString();
                    dt.Rows.Add(dr);
                }

                string[] columnArray = { "vcSupplier_id", "vcWorkArea" };
                DataView dtSelectView = dt.DefaultView;
                DataTable dtSelect = dtSelectView.ToTable(true, columnArray);//去重后的dt 

                string logName = System.DateTime.Now.ToString("yyyyMMddHHmmss") + Guid.NewGuid().ToString("N");
                string logs = string.Empty;

                string[] head = new string[] { };
                string[] field = new string[] { };
                //[vcPartNo], [dBeginDate], [dEndDate]
                //const tHeader = [];
                //const filterVal = [,   ];

                head = new string[] { "导入时间", "车型", "品番", "品名", "内外", "供应商代码", "工区", "是否新规", "OE=SP", "受入", "号试数量", "号试目的", "订单预计发行日", "订单预计纳入日", "纳入便次", "实际纳入日", "结算订单号", "结算订单验收日期", "备注" };
                field = new string[] { "dExportDate", "vcCarType", "vcPartNo", "vcPartName", "vcInsideOutsideType", "vcSupplier_id", "vcWorkArea", "vcIsNewRulesFlag", "vcOEOrSP", "vcDock", "vcNumber", "vcPurposes", "dOrderPurposesDate", "dOrderReceiveDate", "vcReceiveTimes", "dActualReceiveDate", "vcAccountOrderNo", "dAccountOrderReceiveDate",  "vcMemo" };
                string path = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Export" + Path.DirectorySeparatorChar;
                StringBuilder strErr = new StringBuilder();
                for (int i = 0; i < dtSelect.Rows.Count; i++)
                {
                    //组织制定供应商和工区的数据
                    string vcSupplier_id = dtSelect.Rows[i]["vcSupplier_id"].ToString();
                    string vcWorkArea = dtSelect.Rows[i]["vcWorkArea"].ToString();
                    DataRow[] drArray = dt.Select("vcSupplier_id='" + vcSupplier_id + "' and vcWorkArea='" + vcWorkArea + "' ");
                    DataTable dtNewSupplierandWorkArea = drArray[0].Table.Clone(); // 复制DataRow的表结构
                    string msg = string.Empty;
                    foreach (DataRow dr in drArray)
                    {
                        dtNewSupplierandWorkArea.ImportRow(dr);
                    }
                    string strFileName = System.DateTime.Now.ToString("yyyyMMdd") + "_" + vcSupplier_id + "_" + vcWorkArea + "号试订单";
                    string filepath = ComFunction.DataTableToExcel(head, field, dtNewSupplierandWorkArea, ".", loginInfo.UserId, strFileName, ref msg);
                    if (filepath == "")
                    {
                        logs = System.DateTime.Now.ToString() + "供应商：" + vcSupplier_id + "工区" + vcWorkArea + "附件没有生产成功，不能发送邮件 \n";
                        writeLog(logs, logName, loginInfo.UnitCode, loginInfo.UserId);
                        strErr.Append(logs);
                        continue;
                    }
                    filepath = path + filepath;
                    logs = System.DateTime.Now.ToString() + "供应商：" + vcSupplier_id + "工区" + vcWorkArea + "开始查找邮箱 \n";
                    writeLog(logs, logName, loginInfo.UnitCode, loginInfo.UserId);
                    //获取供应商 工区的邮箱
                    DataTable dtEmail = fs0625_Logic.getEmail(vcSupplier_id, vcWorkArea);
                    if (dtEmail.Rows.Count == 0)
                    {
                        logs = System.DateTime.Now.ToString() + "供应商：" + vcSupplier_id + "工区" + vcWorkArea + "没有维护邮箱，不能发送邮件 \n";
                        writeLog(logs, logName, loginInfo.UnitCode, loginInfo.UserId);
                        strErr.Append("供应商：" + vcSupplier_id + "工区" + vcWorkArea + "缺少邮箱，不能发送!");
                        continue;
                    }
                    if (dtEmail.Rows.Count > 1)
                    {
                        logs = System.DateTime.Now.ToString() + "供应商：" + vcSupplier_id + "工区" + vcWorkArea + "邮箱维护冗余有误，不能发送邮件 \n";
                        writeLog(logs, logName, loginInfo.UnitCode, loginInfo.UserId);
                        strErr.Append("供应商：" + vcSupplier_id + "工区" + vcWorkArea + "邮箱冗余,不能发送!");
                        continue;
                    }
                    string strdisplayName = dtEmail.Rows[0]["vcLinkMan"].ToString();
                    if (string.IsNullOrEmpty(strdisplayName))
                    {
                        strdisplayName = "";
                    }
                    string strEmail = dtEmail.Rows[0]["vcEmail"].ToString();
                    if (strEmail == "" || string.IsNullOrEmpty(strEmail))
                    {
                        logs = System.DateTime.Now.ToString() + "供应商：" + vcSupplier_id + "工区" + vcWorkArea + "邮箱为空，不能发送邮件 \n";
                        writeLog(logs, logName, loginInfo.UnitCode, loginInfo.UserId);
                        strErr.Append("供应商：" + vcSupplier_id + "工区" + vcWorkArea + "邮箱为空,不能发送!");
                        continue;
                    }
                    string[] emailArray = strEmail.Split(';');
                    //收件人dt
                    DataTable receiverDt = new DataTable();
                    receiverDt.Columns.Add("address");
                    receiverDt.Columns.Add("displayName");
                    for (int j = 0; j < emailArray.Length; j++)
                    {
                        if (emailArray[j].ToString().Length > 0)
                        {
                            DataRow dr = receiverDt.NewRow();
                            dr["address"] = emailArray[j].ToString();
                            dr["displayName"] = strdisplayName;
                            receiverDt.Rows.Add(dr);
                        }
                    }
                    //抄送人dt 通过Tcode 自己定义cCDt
                    DataTable cCDt = null;
                    DataTable dtCCEmail = fs0625_Logic.getCCEmail("C054");
                    if (dtCCEmail.Rows.Count > 0)
                    {
                        cCDt = dtCCEmail;
                    }
                    //邮件主题
                    string strSubject = "供应商:" + vcSupplier_id + "工区:" + vcWorkArea + "_" + loginInfo.UnitCode + "号试订单信息";
                    //邮件内容
                    string strEmailBody = "";
                    strEmailBody += "<div style='font-family:宋体;font-size:12'>" + "供应商" + vcSupplier_id + " <br /><br />";
                    strEmailBody += "  您好 " + strdisplayName + "<br /><br />";
                    strEmailBody += "  此次邮件内容为事业体" + loginInfo.UnitCode + "号试订单信息，具体内容请查看附！<br /><br />";
                    //strEmailBody += "  请在1个工作日内将是否可以供货的确认结果邮件回复，以下是各个仓库对应的邮箱：<br />";

                    string result = ComFunction.SendEmailInfo(loginInfo.Email, loginInfo.UnitName, strEmailBody, receiverDt, cCDt, strSubject, filepath, false);
                    if (result == "Success")
                    {
                        logs = System.DateTime.Now.ToString() + "供应商：" + vcSupplier_id + "工区" + vcWorkArea + "邮箱发送成功！\n";
                    }
                    else
                    {
                        logs = System.DateTime.Now.ToString() + "供应商：" + vcSupplier_id + "工区" + vcWorkArea + "邮箱发送失败，邮件发送公共方法未知原因！\n";
                    }
                    writeLog(logs, logName, loginInfo.UnitCode, loginInfo.UserId);
                }
                //string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0309_Export.xlsx", 2, loginInfo.UserId, FunctionID);
                //string filepath = ComFunction.DataTableToExcel(head, field, dt, ".", loginInfo.UserId, FunctionID, ref msg);
                //if (filepath == "")
                //{
                //    apiResult.code = ComConstant.ERROR_CODE;
                //    apiResult.data = "导出生成文件失败";
                //    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                //}

                apiResult.code = ComConstant.SUCCESS_CODE;
                if (strErr.Length > 0)
                {
                    apiResult.data = strErr.ToString() + ",其余发送成功！";
                }
                else
                {
                    apiResult.data = "邮件发送成功！";
                }
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0409", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "邮件发送失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="logs"></param>
        /// <param name="logName"></param>
        /// <param name="career"></param>
        /// <param name="userId"></param>
        public void writeLog(string logs, string logName, string career, string userId)
        {
            string path0 = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Log" + Path.DirectorySeparatorChar + "Email" + Path.DirectorySeparatorChar + FunctionID;
            string filename = path0 + Path.DirectorySeparatorChar + career + "_" + userId + "_" + logName + ".txt";
            if (!Directory.Exists(path0))
            {
                Directory.CreateDirectory(path0);
            }

            if (!System.IO.File.Exists(filename))
            {
                FileStream stream = System.IO.File.Create(filename);
                stream.Close();
                stream.Dispose();
            }
            using (StreamWriter writer = new StreamWriter(filename, true))
            {
                writer.WriteLine(logs);
            }
        }
        #endregion
    }
}

