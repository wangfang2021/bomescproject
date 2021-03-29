using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Logic;

namespace SPPSApi.Controllers.G06
{
    [Route("api/FS0616/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0616Controller : BaseController
    {
        FS0602_Logic fs0602_Logic = new FS0602_Logic();
        FS0603_Logic fs0603_Logic = new FS0603_Logic();
        FS0616_Logic fS0616_logic = new FS0616_Logic();
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string FunctionID = "FS0616";

        public FS0616Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        /// <summary>
        /// 页面初始化
        /// </summary>
        /// <returns></returns>
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
                DataTable dtOptionsList = fS0616_logic.getFormOptions();
                List<Object> OrderNoListForForm = ComFunction.convertAllToResult(fs0603_Logic.getSelectOptions(dtOptionsList, "vcOrderNo_Name", "vcOrderNo_Value"));//订单
                List<Object> HaoJiuForForm = ComFunction.convertAllToResult(fs0603_Logic.getSelectOptions(dtOptionsList, "vcHaoJiu_Name", "vcHaoJiu_Value"));//号旧
                List<Object> OrderPlantForForm = ComFunction.convertAllToResult(fs0603_Logic.getSelectOptions(dtOptionsList, "vcOrderPlant_Name", "vcOrderPlant_Value"));//发注工场
                List<Object> SupplierIdForForm = ComFunction.convertAllToResult(fs0603_Logic.getSelectOptions(dtOptionsList, "vcSupplierId_Name", "vcSupplierId_Value"));//供应商
                List<Object> SupplierPlantForForm = ComFunction.convertAllToResult(fs0603_Logic.getSelectOptions(dtOptionsList, "vcSupplierPlant_Name", "vcSupplierPlant_Value"));//工区
                List<Object> ReplyOverDateForForm = ComFunction.convertAllToResult(fs0603_Logic.getSelectOptions(dtOptionsList, "vcReplyOverDate_Name", "vcReplyOverDate_Value"));//回复截止日
                List<Object> OutPutDateForForm = ComFunction.convertAllToResult(fs0603_Logic.getSelectOptions(dtOptionsList, "vcOutPutDate_Name", "vcOutPutDate_Value"));//出荷日

                List<Object> StateList = ComFunction.convertAllToResult(ComFunction.getTCode("C056"));//状态

                res.Add("StateList", StateList);
                res.Add("OrderNoListForForm", OrderNoListForForm);
                res.Add("HaoJiuForForm", HaoJiuForForm);
                res.Add("OrderPlantForForm", OrderPlantForForm);
                res.Add("SupplierIdForForm", SupplierIdForForm);
                res.Add("SupplierPlantForForm", SupplierPlantForForm);
                res.Add("ReplyOverDateForForm", ReplyOverDateForForm);
                res.Add("OutPutDateForForm", OutPutDateForForm);

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
        /// <summary>
        /// 检索方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
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
            Dictionary<string, object> res = new Dictionary<string, object>();

            string strState = dataForm.State;
            List<Object> listOrderNo = dataForm.OrderNoList.ToObject<List<Object>>();
            string strPartId = dataForm.PartId == null ? "" : dataForm.PartId.ToString();
            string strHaoJiu = dataForm.HaoJiu == null ? "" : dataForm.HaoJiu.ToString();
            string strOrderPlant = dataForm.OrderPlant == null ? "" : dataForm.OrderPlant.ToString();
            string strSupplierId = dataForm.SupplierId == null ? "" : dataForm.SupplierId.ToString();
            string strSupplierPlant = dataForm.SupplierPlant == null ? "" : dataForm.SupplierPlant.ToString();
            string strReplyOverDate = dataForm.ReplyOverDate == null ? "" : dataForm.ReplyOverDate.ToString();
            string strOutPutDate = dataForm.OutPutDate == null ? "" : dataForm.OutPutDate.ToString();

            try
            {
                DataTable dataTable = fS0616_logic.getSearchInfo(strState, listOrderNo, strPartId, strHaoJiu, strOrderPlant, strSupplierId, strSupplierPlant, strReplyOverDate, strOutPutDate);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("bAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("bModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("bSelectFlag", ConvertFieldType.BoolType, null);
                List<Object> dataList = null;
                if (dataTable.Rows.Count > 10000)
                {
                    DataTable dtMessage = fs0603_Logic.createTable("MES");
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "本次检索数据条数超过10000,为避免浏览器内存溢出，请调整检索条件或进行数据导出。";
                    dtMessage.Rows.Add(dataRow);

                    DataTable table = dataTable.Clone();
                    dataList = ComFunction.convertAllToResultByConverter(table, dtConverter);
                    res.Add("tempList", dataList);
                    res.Add("messageList", dtMessage);
                    dtMessage = dtMessage.DefaultView.ToTable(true, "vcMessage");
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "message";
                    apiResult.data = res;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                else
                {
                    dataList = ComFunction.convertAllToResultByConverter(dataTable, dtConverter);
                    res.Add("tempList", dataList);
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.data = res;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 导出方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
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

            string strState = dataForm.State;
            List<Object> listOrderNo = dataForm.OrderNoList.ToObject<List<Object>>();
            string strPartId = dataForm.PartId == null ? "" : dataForm.PartId.ToString();
            string strHaoJiu = dataForm.HaoJiu == null ? "" : dataForm.HaoJiu.ToString();
            string strOrderPlant = dataForm.OrderPlant == null ? "" : dataForm.OrderPlant.ToString();
            string strSupplierId = dataForm.SupplierId == null ? "" : dataForm.SupplierId.ToString();
            string strSupplierPlant = dataForm.SupplierPlant == null ? "" : dataForm.SupplierPlant.ToString();
            string strReplyOverDate = dataForm.ReplyOverDate == null ? "" : dataForm.ReplyOverDate.ToString();
            string strOutPutDate = dataForm.OutPutDate == null ? "" : dataForm.OutPutDate.ToString();
            try
            {
                DataTable dataTable = fS0616_logic.getSearchInfo(strState, listOrderNo, strPartId, strHaoJiu, strOrderPlant, strSupplierId, strSupplierPlant, strReplyOverDate, strOutPutDate);

                string[] fields = {"vcState_name","vcOrderNo","vcPart_id","vcOrderPlant","vcHaoJiu","vcOESP","vcSupplierId","vcSupplierPlant",
                    "vcSupplierPlace","vcSufferIn","iPackingQty","iOrderQuantity","iDuiYingQuantity","decBoxQuantity","dDeliveryDate","dOutPutDate","dReplyOverDate","dSupReplyTime"
                };

                string filepath = ComFunction.generateExcelWithXlt(dataTable, fields, _webHostEnvironment.ContentRootPath, "FS0616_Export.xlsx", 1, loginInfo.UserId, FunctionID);
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
        /// <summary>
        /// 保存方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string saveApi([FromBody]dynamic data)
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
            Dictionary<string, object> res = new Dictionary<string, object>();
            try
            {
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                JArray listMultiple = (dataForm.selectmultiple).multipleSelection;
                List<Dictionary<string, Object>> listMultipleData = listMultiple.ToObject<List<Dictionary<string, Object>>>();
                JArray listInfo = dataForm.alltemp.list;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
                int icurrentPage = Convert.ToInt32(dataForm.currentPage == null ? "1" : dataForm.currentPage);
                int iorderby = 0;
                bool hasFind = false;//是否找到需要新增或者修改的数据
                for (int i = 0; i < listMultipleData.Count; i++)
                {
                    bool bModFlag = (bool)listMultipleData[i]["bModFlag"];//true可编辑,false不可编辑
                    if (bModFlag == true)
                        hasFind = true;//修改
                }
                if (!hasFind)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "info";
                    apiResult.data = "最少有一个编辑行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                DataTable dtMessage = fs0603_Logic.createTable("MES");
                DataTable dtMultiple = fS0616_logic.setMultipleData(listMultipleData, ref dtMessage);
                DataTable dtImport = fS0616_logic.checkSaveInfo(listInfoData, dtMultiple, ref dtMessage);
                if (dtMessage.Rows.Count != 0)
                {
                    dtMessage = dtMessage.DefaultView.ToTable(true, "vcMessage");
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fS0616_logic.setSaveInfo(dtImport, loginInfo.UserId, ref dtMessage);
                if (dtMessage.Rows.Count != 0)
                {
                    dtMessage = dtMessage.DefaultView.ToTable(true, "vcMessage");
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                res.Add("icurrentPage", icurrentPage);
                res.Add("iorderby", iorderby);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0902", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.type = "info";
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 回复纳期
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string replyorderApi([FromBody]dynamic data)
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

                string strState = dataForm.searchform.State;
                List<Object> listOrderNo = dataForm.searchform.OrderNoList.ToObject<List<Object>>();
                string strPartId = dataForm.PartId == null ? "" : dataForm.PartId.ToString();
                string strHaoJiu = dataForm.HaoJiu == null ? "" : dataForm.HaoJiu.ToString();
                string strOrderPlant = dataForm.OrderPlant == null ? "" : dataForm.OrderPlant.ToString();
                string strSupplierId = dataForm.SupplierId == null ? "" : dataForm.SupplierId.ToString();
                string strSupplierPlant = dataForm.SupplierPlant == null ? "" : dataForm.SupplierPlant.ToString();
                string strReplyOverDate = dataForm.ReplyOverDate == null ? "" : dataForm.ReplyOverDate.ToString();
                string strOutPutDate = dataForm.OutPutDate == null ? "" : dataForm.OutPutDate.ToString();

                JArray listMultiple = (dataForm.selectmultiple).multipleSelection;
                List<Dictionary<string, Object>> listMultipleData = listMultiple.ToObject<List<Dictionary<string, Object>>>();
                JArray listInfo = dataForm.alltemp.list;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();

                DataTable dtMessage = fs0603_Logic.createTable("MES");
                DataTable dtMultiple = fS0616_logic.setMultipleData(listMultipleData, ref dtMessage);
                DataTable dataTable = fS0616_logic.getSearchInfo(strState, listOrderNo, strPartId, strHaoJiu, strOrderPlant, strSupplierId, strSupplierPlant, strReplyOverDate, strOutPutDate);

                DataTable dtImport = fS0616_logic.checkReplyInfo(listInfoData, dtMultiple, dataTable, ref dtMessage);
                if (dtMessage.Rows.Count != 0)
                {
                    dtMessage = dtMessage.DefaultView.ToTable(true, "vcMessage");
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fS0616_logic.setReplyInfo(dtImport, loginInfo.UserId, ref dtMessage);
                if (dtMessage.Rows.Count != 0)
                {
                    dtMessage = dtMessage.DefaultView.ToTable(true, "vcMessage");
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M04UE0203", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 一括赋予出荷日
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string saveallInoutputApi([FromBody]dynamic data)
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

                string strState = dataForm.searchform.State;
                List<Object> listOrderNo = dataForm.searchform.OrderNoList.ToObject<List<Object>>();
                string strPartId = dataForm.PartId == null ? "" : dataForm.PartId.ToString();
                string strHaoJiu = dataForm.HaoJiu == null ? "" : dataForm.HaoJiu.ToString();
                string strOrderPlant = dataForm.OrderPlant == null ? "" : dataForm.OrderPlant.ToString();
                string strSupplierId = dataForm.SupplierId == null ? "" : dataForm.SupplierId.ToString();
                string strSupplierPlant = dataForm.SupplierPlant == null ? "" : dataForm.SupplierPlant.ToString();
                string strReplyOverDate = dataForm.ReplyOverDate == null ? "" : dataForm.ReplyOverDate.ToString();
                string strOutPutDate = dataForm.OutPutDate == null ? "" : dataForm.OutPutDate.ToString();
                string dOutPutDate = dataForm.info;

                JArray listMultiple = (dataForm.selectmultiple).multipleSelection;
                List<Dictionary<string, Object>> listMultipleData = listMultiple.ToObject<List<Dictionary<string, Object>>>();
                JArray listInfo = dataForm.alltemp.list;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
                DataTable dtMessage = fs0603_Logic.createTable("MES");

                if (dOutPutDate == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "出荷日不能为空";
                    dtMessage.Rows.Add(dataRow);
                }
                if (dOutPutDate != "")
                {
                    if (Convert.ToDateTime(dOutPutDate) < Convert.ToDateTime(System.DateTime.Now.ToString("yyyy-MM-dd")))
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = "出荷日不能小于当前时间";
                        dtMessage.Rows.Add(dataRow);
                    }
                }
                if (dtMessage.Rows.Count != 0)
                {
                    dtMessage = dtMessage.DefaultView.ToTable(true, "vcMessage");
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                DataTable dataTable = fS0616_logic.getSearchInfo(strState, listOrderNo, strPartId, strHaoJiu, strOrderPlant, strSupplierId, strSupplierPlant, strReplyOverDate, strOutPutDate);
                DataTable dtImport = fS0616_logic.checkOutputInfo(listMultipleData, dataTable, dOutPutDate, ref dtMessage);
                if (dtMessage.Rows.Count != 0)
                {
                    dtMessage = dtMessage.DefaultView.ToTable(true, "vcMessage");
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fS0616_logic.setOutputInfo(dtImport, loginInfo.UserId, ref dtMessage);
                if (dtMessage.Rows.Count != 0)
                {
                    dtMessage = dtMessage.DefaultView.ToTable(true, "vcMessage");
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M04UE0203", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 邮件预览
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string emailBodyApi([FromBody]dynamic data)
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
            string strReplyOverDate = dataForm.dReplyOverDate == null ? "" : dataForm.dReplyOverDate;
            try
            {
                String emailBody = fS0616_logic.setEmailBody(strReplyOverDate);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = emailBody;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0708", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "邮件预览失败" + ex.Message;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 发送纳期确认
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string saveopenorderApi([FromBody]dynamic data)
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

                string strState = dataForm.searchform.State;
                List<Object> listOrderNo = dataForm.searchform.OrderNoList.ToObject<List<Object>>();
                string strPartId = dataForm.PartId == null ? "" : dataForm.PartId.ToString();
                string strHaoJiu = dataForm.HaoJiu == null ? "" : dataForm.HaoJiu.ToString();
                string strOrderPlant = dataForm.OrderPlant == null ? "" : dataForm.OrderPlant.ToString();
                string strSupplierId = dataForm.SupplierId == null ? "" : dataForm.SupplierId.ToString();
                string strSupplierPlant = dataForm.SupplierPlant == null ? "" : dataForm.SupplierPlant.ToString();
                string strReplyOverDate = dataForm.ReplyOverDate == null ? "" : dataForm.ReplyOverDate.ToString();
                string strOutPutDate = dataForm.OutPutDate == null ? "" : dataForm.OutPutDate.ToString();
                string dReplyOverDate = dataForm.info;//期望回复日
                string strEmailBody = dataForm.mailboy;//邮件体
                JArray listMultiple = (dataForm.selectmultiple).multipleSelection;
                List<Dictionary<string, Object>> listMultipleData = listMultiple.ToObject<List<Dictionary<string, Object>>>();

                DataTable dtMessage = fs0603_Logic.createTable("MES");
                if (dReplyOverDate == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "回复截止日期不能为空。";
                    dtMessage.Rows.Add(dataRow);
                }
                if (dReplyOverDate != "" && Convert.ToDateTime(dReplyOverDate + " 23:59:59") < System.DateTime.Now)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "回复截止日期不能小于当前时间。";
                    dtMessage.Rows.Add(dataRow);
                }
                if (strEmailBody == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "邮件体不能为空(请点击邮件预览按钮)。";
                    dtMessage.Rows.Add(dataRow);
                }
                if (dtMessage.Rows.Count != 0)
                {
                    dtMessage = dtMessage.DefaultView.ToTable(true, "vcMessage");
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                DataTable dataTable = fS0616_logic.getSearchInfo(strState, listOrderNo, strPartId, strHaoJiu, strOrderPlant, strSupplierId, strSupplierPlant, strReplyOverDate, strOutPutDate);

                DataTable dtImport = fS0616_logic.checkOpenInfo(listMultipleData, dataTable, dReplyOverDate, ref dtMessage);
                if (dtMessage.Rows.Count != 0)
                {
                    dtMessage = dtMessage.DefaultView.ToTable(true, "vcMessage");
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fS0616_logic.setOpenInfo(dtImport, loginInfo.UserId, ref dtMessage);
                if (dtMessage.Rows.Count != 0)
                {
                    //弹出错误dtMessage
                    dtMessage = dtMessage.DefaultView.ToTable(true, "vcMessage");
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string strTheme = "紧急订单纳期确认";
                DataTable dtToList = fs0602_Logic.getToList(dtImport, ref dtMessage);
                fs0602_Logic.sendEmailInfo(loginInfo.UserId, loginInfo.UserName, loginInfo.Email, strTheme, strEmailBody, dtToList, ref dtMessage);
                if (dtMessage.Rows.Count != 0)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "紧急订单纳期展开成功。";
                    dtMessage.Rows.InsertAt(dataRow, 0);
                    dtMessage = dtMessage.DefaultView.ToTable(true, "vcMessage");
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M04UE0203", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 子页面初始化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string searchsubApi([FromBody]dynamic data)
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
                Dictionary<string, object> res = new Dictionary<string, object>();

                string strLinId = dataForm.LinId == null ? "" : dataForm.LinId;
                string strOrderNo = dataForm.vcOrderNo == null ? "" : dataForm.vcOrderNo;
                string strPart_id = dataForm.vcPart_id == null ? "" : dataForm.vcPart_id;
                string strSupplierId = dataForm.vcSupplierId == null ? "" : dataForm.vcSupplierId;
                string strStatus = "0";
                DataTable dataTable = fS0616_logic.getSearchSubInfo(strOrderNo, strPart_id, strSupplierId);
                if (dataTable != null && dataTable.Rows.Count != 0)
                {
                    strStatus = dataTable.Rows[0]["vcState"].ToString();
                }
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("bAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("bModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("bSelectFlag", ConvertFieldType.BoolType, null);
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dataTable, dtConverter);
                res.Add("tempList", dataList);
                res.Add("statusItem", strStatus);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M04UE0203", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 子页面提示
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string checksubApi([FromBody]dynamic data)
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
                //JArray listMultiple = (dataForm.selectmultiple).multipleSelection;
                //List<Dictionary<string, Object>> listMultipleData = listMultiple.ToObject<List<Dictionary<string, Object>>>();
                JArray listInfo = dataForm.alltemp.list;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
                //bool hasFind = false;//是否找到需要新增或者修改的数据
                //for (int i = 0; i < listMultipleData.Count; i++)
                //{
                //    bool bModFlag = (bool)listMultipleData[i]["bModFlag"];//true可编辑,false不可编辑
                //    if (bModFlag == true)
                //        hasFind = true;//修改
                //}
                //if (!hasFind)
                //{
                //    apiResult.code = ComConstant.ERROR_CODE;
                //    apiResult.type = "info";
                //    apiResult.data = "最少有一个编辑行！";
                //    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                //}
                DataTable dtMessage = fs0603_Logic.createTable("MES");
                //DataTable dtMultiple = fS0616_logic.setMultipleData(listMultipleData, ref dtMessage);
                DataTable dtImport = fS0616_logic.checksubSaveInfo(listInfoData, ref dtMessage);
                if (dtMessage.Rows.Count != 0)
                {
                    dtMessage = dtMessage.DefaultView.ToTable(true, "vcMessage");
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //判断整箱收容数
                bool bIsInteger = true;
                for (int i = 0; i < dtImport.Rows.Count; i++)
                {
                    string decBoxQuantity = dtImport.Rows[i]["decBoxQuantity"].ToString();
                    string strBoxColor = fS0616_logic.IsInteger(decBoxQuantity);
                    //decimal decBoxQuantity = Convert.ToDecimal(dtImport.Rows[i]["decBoxQuantity"].ToString());
                    if (strBoxColor=="1")
                    {
                        bIsInteger = false;
                        break;
                    }
                }
                if (bIsInteger)
                {
                    //直接保存
                    fS0616_logic.setSaveInfo(dtImport, loginInfo.UserId, ref dtMessage);
                    if (dtMessage.Rows.Count != 0)
                    {
                        dtMessage = dtMessage.DefaultView.ToTable(true, "vcMessage");
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.type = "list";
                        apiResult.data = dtMessage;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.type = "";
                    apiResult.data = null;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                else
                {
                    //提示
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.type = "BoxInfo";
                    apiResult.data = null;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0902", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.type = "info";
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 子页面保存
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string savesubApi([FromBody]dynamic data)
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
                //JArray listMultiple = (dataForm.selectmultiple).multipleSelection;
                //List<Dictionary<string, Object>> listMultipleData = listMultiple.ToObject<List<Dictionary<string, Object>>>();
                JArray listInfo = dataForm.alltemp.list;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
                //bool hasFind = false;//是否找到需要新增或者修改的数据
                //for (int i = 0; i < listMultipleData.Count; i++)
                //{
                //    bool bModFlag = (bool)listMultipleData[i]["bModFlag"];//true可编辑,false不可编辑
                //    if (bModFlag == true)
                //        hasFind = true;//修改
                //}
                //if (!hasFind)
                //{
                //    apiResult.code = ComConstant.ERROR_CODE;
                //    apiResult.type = "info";
                //    apiResult.data = "最少有一个编辑行！";
                //    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                //}
                DataTable dtMessage = fs0603_Logic.createTable("MES");
                //DataTable dtMultiple = fS0616_logic.setMultipleData(listMultipleData, ref dtMessage);
                DataTable dtImport = fS0616_logic.checksubSaveInfo(listInfoData, ref dtMessage);
                if (dtMessage.Rows.Count != 0)
                {
                    dtMessage = dtMessage.DefaultView.ToTable(true, "vcMessage");
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fS0616_logic.setSaveInfo(dtImport, loginInfo.UserId, ref dtMessage);
                if (dtMessage.Rows.Count != 0)
                {
                    dtMessage = dtMessage.DefaultView.ToTable(true, "vcMessage");
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0902", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.type = "info";
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 自动计算时间日期
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string checkTimeApi([FromBody]dynamic data)
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
            Dictionary<string, object> res = new Dictionary<string, object>();
            string strDeliveryDate = dataForm.dDeliveryDate == null ? "" : dataForm.dDeliveryDate;
            string strOutPutDate = dataForm.dOutPutDate == null ? "" : dataForm.dOutPutDate;
            try
            {
                int iValue = fS0616_logic.getManualCode();
                string dOutPutDate = fS0616_logic.getAddDate1(Convert.ToDateTime(strDeliveryDate), iValue * 1, false).ToString("yyyy/MM/dd");
                res.Add("OutPutDate", dOutPutDate);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0708", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "计算日期失败" + ex.Message;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }

    }
}