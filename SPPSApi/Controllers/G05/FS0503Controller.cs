using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.Util;
using Org.BouncyCastle.Utilities.Encoders;

namespace SPPSApi.Controllers.G05
{
    [Route("api/FS0503/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0503Controller : BaseController
    {
        FS0503_Logic fs0503_Logic = new FS0503_Logic();
        private readonly string FunctionID = "FS0503";

        private readonly IWebHostEnvironment _webHostEnvironment;


        public FS0503Controller(IWebHostEnvironment webHostEnvironment)
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
                DataTable task = fs0503_Logic.GetTaskNum();//待回复的数据

                List<Object> dataList_C033 = ComFunction.convertAllToResult(ComFunction.getTCode("C034"));//荷姿状态

                res.Add("C034", dataList_C033);
                res.Add("taskNum", task.Rows.Count);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M05UE0301", ex, loginInfo.UserId);
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
            string vcSupplier_id = dataForm.vcSupplier_id == null ? "" : dataForm.vcSupplier_id;
            string vcWorkArea = dataForm.vcWorkArea == null ? "" : dataForm.vcWorkArea;
            string vcState = dataForm.vcState == null ? "" : dataForm.vcState;
            string vcPartNo = dataForm.vcPartNo == null ? "" : dataForm.vcPartNo;

            string vcCarType = dataForm.vcCarType == null ? "" : dataForm.vcCarType;
            string dExpectDeliveryDate = dataForm.dExpectDeliveryDate == null ? "" : dataForm.dExpectDeliveryDate;

            try
            {
                DataTable dt = fs0503_Logic.Search(vcSupplier_id, vcWorkArea, vcState, vcPartNo, vcCarType, dExpectDeliveryDate);
                //if (dt.Rows.Count > 0)
                //{
                //    for (int i = 0; i < dt.Rows.Count; i++)
                //    {
                //        var vcImageRoutes = dt.Rows[i]["vcImageRoutes"].ToString();
                //        if (vcImageRoutes.Length > 0)
                //        {
                //            //"data:image/"+file.getCrfileExtention()+";base64,"+getImageStrFromPath(rootPath+file.getCrfileServerPath())
                //            StringBuilder strBuild = new StringBuilder();
                //            string[] images = vcImageRoutes.Split(",");

                //            for (var j = 0; j < images.Length; j++)
                //            {
                //                string imageType = images[j].Substring(images[j].LastIndexOf('.') + 1).Replace("\"", "");
                //                //string baseStr = "data:image/" + imageType + ";base64,"+getBaseApi(images[j])+",";
                //                string baseStr = getBaseApi(images[j]) + "?";
                //                strBuild.Append(baseStr);
                //            }
                //            dt.Rows[i]["vcImageRoutes"] = strBuild.Remove(strBuild.Length - 1, 1).ToString();
                //        }
                //    }
                //}
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("dSynchronizationDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dExpectDeliveryDate", ConvertFieldType.DateType, "yyyy/MM/dd");
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M05UE0302", ex, loginInfo.UserId);
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

            string dSynchronizationDate = dataForm.dSynchronizationDate == null ? "" : dataForm.dSynchronizationDate;
            string vcState = dataForm.vcState == null ? "" : dataForm.vcState;
            string vcPartNo = dataForm.vcPartNo == null ? "" : dataForm.vcPartNo;
            string vcSupplier_id = dataForm.vcSupplier_id == null ? "" : dataForm.vcSupplier_id;
            string vcWorkArea = dataForm.vcWorkArea == null ? "" : dataForm.vcWorkArea;
            string vcCarType = dataForm.vcCarType == null ? "" : dataForm.vcCarType;
            string dExpectDeliveryDate = dataForm.dExpectDeliveryDate == null ? "" : dataForm.dExpectDeliveryDate;
            string vcOEOrSP = dataForm.vcOEOrSP == null ? "" : dataForm.vcOEOrSP;
            string vcBoxType = dataForm.vcBoxType == null ? "" : dataForm.vcBoxType;

            try
            {
                DataTable dt = fs0503_Logic.Search(vcSupplier_id, vcWorkArea, vcState, vcPartNo, vcCarType, dExpectDeliveryDate);
                string[] head = new string[] { };
                string[] field = new string[] { };
                //[vcPartNo], [dBeginDate], [dEndDate]
                head = new string[] { "同步时间", "状态", "品番", "使用开始时间", "品名", "车型", "OE=SP", "供应商代码", "工区", "要望纳期", "要望收容数", "收容数", "箱最大收容数", "箱种", "长(mm)", "宽(mm)", "高(mm)", "空箱重量(g)", "单品净重(g)", "发送时间", "回复时间", "承认时间", "原单位织入时间", "备注" };
                field = new string[] { "dSynchronizationDate", "vcState", "vcPartNo", "dUseStartDate", "vcPartName", "vcCarType", "vcOEOrSP", "vcSupplier_id", "vcWorkArea", "dExpectDeliveryDate", "vcExpectIntake", "vcIntake", "vcBoxMaxIntake", "vcBoxType", "vcLength", "vcWide", "vcHeight", "vcEmptyWeight", "vcUnitNetWeight", "dSendDate", "dReplyDate", "dAdmitDate", "dWeaveDate", "vcMemo" };
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M05UE0303", ex, loginInfo.UserId);
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

                //const tHeader = ["导入时间", "车型", "品番", "品名", "内外", "供应商代码", "工区", "是否新规", "OE=SP", "受入", "号试数量", "号试目的", "订单预计发行日", "订单预计纳入日", "纳入便次", "实际纳入日", "结算订单号", "结算订单验收日期", "号试订单验收日期", "备注"];
                //const filterVal = ["dExportDate", "vcCarType", "vcPartNo", "vcPartName", "vcInsideOutsideType", "vcSupplier_id", "vcWorkArea", "vcIsNewRulesFlag", "vcOEOrSP", "vcDock", "vcNumber", "vcPurposes", "dOrderPurposesDate", "dOrderReceiveDate", "vcReceiveTimes", "dActualReceiveDate", "vcAccountOrderNo", "dAccountOrderReceiveDate", "dOrderSendDate", "vcMemo"];
                //开始数据验证
                if (hasFind)
                {
                    string[,] strField = new string[,] {{"同步时间", "状态", "品番", "使用开始时间", "品名", "车型", "OE=SP", "供应商代码", "工区", "要望纳期", "要望收容数", "收容数", "箱最大收容数", "箱种", "长(mm)", "宽(mm)", "高(mm)", "空箱重量(g)", "单品净重(g)", "照片","发送时间","回复时间","承认时间","原单位织入时间","备注"},
                                                {"dSynchronizationDate", "vcState", "vcPartNo", "dUseStartDate", "vcPartName", "vcCarType", "vcOEOrSP", "vcSupplier_id", "vcWorkArea", "dExpectDeliveryDate", "vcExpectIntake", "vcIntake", "vcBoxMaxIntake", "vcBoxType", "vcLength", "vcWide", "vcHeight", "vcEmptyWeight", "vcUnitNetWeight","vcImageRoutes","dSendDate", "dReplyDate", "dAdmitDate", "dWeaveDate", "vcMemo"},
                                                {"","","","","","","","","",FieldCheck.Date,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,"",FieldCheck.Decimal, FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,"","","","","",""},
                                                {"0","5","12","0","200","50","200","4","50","0","20","20","20","50","20","20","20","20","20","500","0","0","0","0","500"},//最大长度设定,不校验最大长度用0
                                                {"0","1","1","0","0","0","0","1","0","0","0","0","0","0","0","0","0","0","0","1","0","0","0","0","0"},//最小长度设定,可以为空用0
                                                {"1","2","3","4","5","6","7","8","9","10","11","12","13","14","15","16","17","18","19","20","21","22","23","24","25"}//前台显示列号，从0开始计算,注意有选择框的是0
                         };
                    //需要判断时间区间先后关系的字段
                    string[,] strDateRegion = { };
                    string[,] strSpecialCheck = { //例子-变更事项字段，当它为新设时，号旧必须为号口，旧型开始、旧型结束、旧型持续开始必须为空
                        
                          };

                    List<Object> checkRes = ListChecker.validateList(listInfoData, strField, strDateRegion, strSpecialCheck, true, "FS0503");
                    if (checkRes != null)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = checkRes;
                        apiResult.flag = Convert.ToInt32(ERROR_FLAG.单元格定位提示);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }

                string strErrorPartId = "";
                fs0503_Logic.Save(listInfoData, loginInfo.UserId, ref strErrorPartId);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M05UE0304", ex, loginInfo.UserId);
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
            try
            {
                //以下开始业务处理
                //以下开始业务处理

                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                JArray listInfo = dataForm.parentFormSelectItem;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();

                string dExpectDeliveryDate = dataForm.allInstallForm.dExpectDeliveryDate == null ? "" : dataForm.allInstallForm.dExpectDeliveryDate;

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
                fs0503_Logic.hZZK(listInfoData, dExpectDeliveryDate, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M05UE0305", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除失败";
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
                JArray listInfo = dataForm.parentFormSelectItem;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();

                string dExpectDeliveryDate = dataForm.allInstallForm.dExpectDeliveryDate == null ? "" : dataForm.allInstallForm.dExpectDeliveryDate;

                if (listInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请先选中数据，再进行承认操作！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fs0503_Logic.admit(listInfoData, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0405", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除失败";
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
                JArray listInfo = dataForm.parentFormSelectItem;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();

                string dExpectDeliveryDate = dataForm.allInstallForm.dExpectDeliveryDate == null ? "" : dataForm.allInstallForm.dExpectDeliveryDate;

                if (listInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请先选中数据，再进行承认操作！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fs0503_Logic.returnHandle(listInfoData, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0406", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除失败";
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
                JArray listInfo = dataForm.parentFormSelectItem;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();

                string dExpectDeliveryDate = dataForm.allInstallForm.dExpectDeliveryDate == null ? "" : dataForm.allInstallForm.dExpectDeliveryDate;

                if (listInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请先选中数据，再进行承认操作！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fs0503_Logic.weaveHandle(listInfoData, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0407", ex, loginInfo.UserId);
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
                    apiResult.data = "一括赋予收容数不能全为空！,请确认！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (listInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请先进行数据检索，再进行一括赋予操作！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                fs0503_Logic.allInstall(listInfoData, vcIntake, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0406", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "一括赋予失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 通过选中数据获取出荷信息
        [HttpPost]
        [EnableCors("any")]
        public string getChuHeBySelectApi([FromBody] dynamic data)
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
            JArray listInfo = dataForm.multipleSelection;
            List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();

            try
            {
                if (listInfoData.Count != 1)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请选中一条数据进行编辑，请确认！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                String strIAutoId = listInfoData[0]["iAutoId"].ToString();
                DataTable dt = fs0503_Logic.SearchByIAutoId(strIAutoId);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("dSynchronizationDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dExpectDeliveryDate", ConvertFieldType.DateType, "yyyy/MM/dd");
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M05UE0303", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "编辑查询失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 编辑对话框确定按钮
        [HttpPost]
        [EnableCors("any")]
        public string editOkApi([FromBody] dynamic data)
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

            try
            {
                //
                if (fs0503_Logic.editOk(dataForm, loginInfo.UserId))
                {
                    //有误需要删除的冗余图片
                    string vcDelImageRoutes = dataForm.vcDelImageRoutes;
                    if (vcDelImageRoutes.Length > 0)
                    {
                        string[] imsges = dataForm.vcDelImageRoutes.Split(",");
                        for (int i = 0; i < imsges.Length; i++)
                        {
                            String realPath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Images";
                            if (System.IO.File.Exists(realPath + imsges[i]))
                            {
                                System.IO.File.Delete(realPath + imsges[i]);
                            }
                        }
                    }
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "保存成功！";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M05UE0304", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        [HttpPost]
        [EnableCors("any")]
        public IActionResult getBaseApi1(string path)
        {

            //string strPath1 = "D:\\CDIST接口数据备份_G\\FP0033.jpg";
            var width = 0;
            string appPath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Images";
            var errorImage = appPath + "404.png";//没有找到图片
            var imgPath = string.IsNullOrEmpty(path) ? errorImage : appPath + path;
            //获取图片的返回类型
            var contentTypDict = new Dictionary<string, string> {
                {"jpg","image/jpeg"},
                {"jpeg","image/jpeg"},
                {"jpe","image/jpeg"},
                {"png","image/png"},
                {"gif","image/gif"},
                {"ico","image/x-ico"},
                {"tif","image/tiff"},
                {"tiff","image/tiff"},
                {"fax","image/fax"},
                {"wbmp","image//vnd.wap.wbmp"},
                {"rp","image/vnd.rn-realpix"}
            };
            var contentTypeStr = "image/jpeg";
            var imgTypeSplit = path.Split('.');
            var imgType = imgTypeSplit[imgTypeSplit.Length - 1].ToLower();
            //未知的图片类型
            if (!contentTypDict.ContainsKey(imgType))
            {
                imgPath = errorImage;
            }
            else
            {
                contentTypeStr = contentTypDict[imgType];
            }
            //图片不存在
            if (!new FileInfo(imgPath).Exists)
            {
                imgPath = errorImage;
            }
            //原图
            if (width <= 0)
            {
                using (var sw = new FileStream(imgPath, FileMode.Open,FileAccess.ReadWrite))
                {
                    var bytes = new byte[sw.Length];
                    sw.Read(bytes, 0, bytes.Length);
                    sw.Close();
                    return new FileContentResult(bytes, contentTypeStr);
                }
            }
            //缩小图片
            using (var imgBmp = new Bitmap(imgPath))
            {
                //找到新尺寸
                var oWidth = imgBmp.Width;
                var oHeight = imgBmp.Height;
                var height = oHeight;
                if (width > oWidth)
                {
                    width = oWidth;
                }
                else
                {
                    height = width * oHeight / oWidth;
                }
                var newImg = new Bitmap(imgBmp, width, height);
                newImg.SetResolution(72, 72);
                var ms = new MemoryStream();
                newImg.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                var bytes = ms.GetBuffer();
                ms.Close();
                return new FileContentResult(bytes, contentTypeStr);
            }

        }

        [HttpGet]
        [EnableCors("any")]
        public IActionResult getBaseApi(string path)
        {

            try
            {
                Console.WriteLine("111");
                string fileSavePath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Images";//文件临时目录，导入完成后 删除
                var provider = new FileExtensionContentTypeProvider();
                FileInfo fileInfo = new FileInfo(fileSavePath + path);
                var ext = fileInfo.Extension;
                new FileExtensionContentTypeProvider().Mappings.TryGetValue(ext, out var contenttype);
                byte[] bt = System.IO.File.ReadAllBytes(fileSavePath + path);
                if (fileInfo.Exists)
                    fileInfo.Delete();
                return File(bt, contenttype ?? "image/Jpeg", fileInfo.Name);
            }
            catch (Exception ex)
            {
                ContentResult result = new ContentResult();
                result.Content = "<script>alert('导出失败,没有找到要导出的文件！')</script>";
                result.ContentType = "text/html;charset=utf-8";
                ComMessage.GetInstance().ProcessMessage("download", "M00UE0007", ex, "system");
                return result;
            }

        }
    }
}

