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

                List<Object> StateList = ComFunction.convertAllToResult(fs0603_Logic.getCodeInfo("NqState"));//状态
                List<Object> OrderNoList = ComFunction.convertAllToResult(fS0616_logic.getOrderNoList());//订单列表
                List<Object> OrderPlantList = ComFunction.convertAllToResult(ComFunction.getTCode("C000"));//发注工厂
                List<Object> InOutList = ComFunction.convertAllToResult(ComFunction.getTCode("C003"));//内外区分
                List<Object> HaoJiuList = ComFunction.convertAllToResult(ComFunction.getTCode("C004"));//号旧区分

                res.Add("StateList", StateList);
                res.Add("OrderNoList", OrderNoList);
                res.Add("OrderPlantList", OrderPlantList);
                res.Add("InOutList", InOutList);
                res.Add("HaoJiuList", HaoJiuList);

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

            string strDelete = dataForm.Delete;
            string strState = dataForm.State;
            List<Object> listOrderNo = dataForm.OrderNoList.ToObject<List<Object>>();
            string strPartId = dataForm.PartId;
            string strOrderPlant = dataForm.OrderPlant;
            string strInOut = dataForm.InOut;
            string strHaoJiu = dataForm.HaoJiu;
            string strSupplierId = dataForm.SupplierId;
            string strSupplierPlant = dataForm.SupplierPlant;

            try
            {
                DataTable dataTable = fS0616_logic.getSearchInfo(strDelete, strState, listOrderNo, strPartId, strOrderPlant, strInOut, strHaoJiu, strSupplierId, strSupplierPlant);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("bAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("bModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("bSelectFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("dReplyOverDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dDeliveryDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dOutPutDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dataTable, dtConverter);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
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

            string strDelete = dataForm.Delete;
            string strState = dataForm.State;
            List<Object> listOrderNo = dataForm.OrderNoList.ToObject<List<Object>>();
            string strPartId = dataForm.PartId;
            string strOrderPlant = dataForm.OrderPlant;
            string strInOut = dataForm.InOut;
            string strHaoJiu = dataForm.HaoJiu;
            string strSupplierId = dataForm.SupplierId;
            string strSupplierPlant = dataForm.SupplierPlant;
            try
            {
                DataTable dataTable = fS0616_logic.getSearchInfo(strDelete, strState, listOrderNo, strPartId, strOrderPlant, strInOut, strHaoJiu, strSupplierId, strSupplierPlant);

                string[] fields = {"vcState_name","dReplyOverDate","vcOrderNo","vcPart_id","vcOrderPlant","vcInOut","vcHaoJiu","vcOESP","vcSupplierId",
                    "vcSupplierPlant","vcSupplierPlace","vcSufferIn","iPackingQty","iOrderQuantity","iDuiYingQuantity","dDeliveryDate","dOutPutDate"
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
            try
            {
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                JArray listInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
                bool hasFind = false;//是否找到需要新增或者修改的数据
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["bModFlag"];//true可编辑,false不可编辑
                    bool bAddFlag = (bool)listInfoData[i]["bAddFlag"];//true可编辑,false不可编辑
                    if (bAddFlag == true)
                        hasFind = true;//新增
                    else if (bAddFlag == false && bModFlag == true)
                        hasFind = true;//修改
                }
                if (!hasFind)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "info";
                    apiResult.data = "最少有一个编辑行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //开始数据验证
                if (hasFind)
                {
                    #region 数据校验
                    //string[,] strField = new string[,] {{"同步时间","变更事项","包装工厂","补给品番","品名","车种代码","收货方","使用开始","使用结束","替代品番","内外区分","OE=SP","号旧区分","旧型年限生产区分","旧型开始时间","实施年月","供应商编号","供应商使用开始","供应商使用结束","供应商名称"},
                    //                            {"dSyncTime","vcChanges","vcPackingPlant","vcPartId","vcPartENName","vcCarfamilyCode","vcReceiver","dFromTime","dToTime","vcPartId_Replace","vcInOut","vcOESP","vcHaoJiu","vcOldProduction","dOldStartTime","dDebugTime","vcSupplierId","dSupplierFromTime","dSupplierToTime","vcSupplierName"},
                    //                            {FieldCheck.Date,FieldCheck.NumChar,FieldCheck.NumChar,FieldCheck.NumChar,FieldCheck.NumChar,FieldCheck.NumChar,FieldCheck.NumChar,FieldCheck.Date,FieldCheck.Date,FieldCheck.NumChar,FieldCheck.NumChar,FieldCheck.NumChar,FieldCheck.NumChar,FieldCheck.NumChar,FieldCheck.Date,FieldCheck.YearMonth,FieldCheck.NumChar,FieldCheck.Date,FieldCheck.Date,FieldCheck.NumChar},
                    //                            {"0","0","0","12","200","0","0","0","0","0","0","0","0","0","0","0","10","0","0","0"},//最大长度设定,不校验最大长度用0
                    //                            {"0","1","1","12","1","0","1","1","1","0","1","1","1","0","0","0","1","1","1","1"},//最小长度设定,可以为空用0
                    //                            {"1","2","3","4","5","6","7","8","9","10","11","12","13","14","15","16","17","18","19","20"}//前台显示列号，从0开始计算,注意有选择框的是0
                    //};
                    ////需要判断时间区间先后关系的字段
                    //string[,] strDateRegion = { { "dFromTime", "dToTime" }, { "dSupplierFromTime", "dSupplierToTime" } };
                    //string[,] strSpecialCheck = { };
                    //List<Object> checkRes = ListChecker.validateList(listInfoData, strField, strDateRegion, strSpecialCheck, true, "FS0603");
                    //if (checkRes != null)
                    //{
                    //    apiResult.code = ComConstant.ERROR_CODE;
                    //    apiResult.data = checkRes;
                    //    apiResult.flag = Convert.ToInt32(ERROR_FLAG.单元格定位提示);
                    //    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    //}
                    #endregion
                }
                DataTable dtMessage = fs0603_Logic.createTable("MES");
                //DataTable dataTable = fS0616_logic.getSearchInfo(strDelete, strState, strOrderNoList, strPartId, strOrderPlant, strInOut, strHaoJiu, strSupplierId, strSupplierPlant);
                DataTable dataTable = new DataTable();
                bool bReault = true;
                DataTable dtImport = fS0616_logic.checkSaveInfo(listInfoData, dataTable, ref bReault, ref dtMessage);
                if (!bReault)
                {
                    //弹出错误dtMessage
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fS0616_logic.setSaveInfo(dtImport, loginInfo.UserId, ref dtMessage);
                if (dtMessage.Rows.Count != 0)
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

                string strDelete = dataForm.Delete;
                string strState = dataForm.State;
                List<Object> listOrderNo = dataForm.OrderNoList.ToObject<List<Object>>();
                string strPartId = dataForm.PartId;
                string strOrderPlant = dataForm.OrderPlant;
                string strInOut = dataForm.InOut;
                string strHaoJiu = dataForm.HaoJiu;
                string strSupplierId = dataForm.SupplierId;
                string strSupplierPlant = dataForm.SupplierPlant;

                JArray checkedInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();
                DataTable dtMessage = fs0603_Logic.createTable("MES");
                DataTable dataTable = fS0616_logic.getSearchInfo(strDelete, strState, listOrderNo, strPartId, strOrderPlant, strInOut, strHaoJiu, strSupplierId, strSupplierPlant);
                bool bReault = true;
                DataTable dtImport = fS0616_logic.checkReplyInfo(listInfoData, dataTable, ref bReault, ref dtMessage);
                if (!bReault)
                {
                    //弹出错误dtMessage
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fS0616_logic.setReplyInfo(dtImport, loginInfo.UserId, ref dtMessage);
                if (dtMessage.Rows.Count != 0)
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M04UE0203", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = null;
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

                string strDelete = dataForm.Delete;
                string strState = dataForm.State;
                List<Object> listOrderNo = dataForm.OrderNoList.ToObject<List<Object>>();
                string strPartId = dataForm.PartId;
                string strOrderPlant = dataForm.OrderPlant;
                string strInOut = dataForm.InOut;
                string strHaoJiu = dataForm.HaoJiu;
                string strSupplierId = dataForm.SupplierId;
                string strSupplierPlant = dataForm.SupplierPlant;
                string strReplyOverDate = dataForm.info;//期望回复日

                JArray checkedInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();
                if (strReplyOverDate == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "info";
                    apiResult.data = "期望回复日期不能为空";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                DataTable dtMessage = fs0603_Logic.createTable("MES");
                DataTable dataTable = fS0616_logic.getSearchInfo(strDelete, strState, listOrderNo, strPartId, strOrderPlant, strInOut, strHaoJiu, strSupplierId, strSupplierPlant);
                bool bReault = true;
                DataTable dtImport = fS0616_logic.checkOpenInfo(listInfoData, dataTable, ref bReault, ref dtMessage);
                if (!bReault)
                {
                    //弹出错误dtMessage
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fS0616_logic.setOpenInfo(dtImport, loginInfo.UserId, ref dtMessage);
                if (dtMessage.Rows.Count != 0)
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M04UE0203", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "";
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

                string strDelete = dataForm.Delete;
                string strState = dataForm.State;
                List<Object> listOrderNo = dataForm.OrderNoList.ToObject<List<Object>>();
                string strPartId = dataForm.PartId;
                string strOrderPlant = dataForm.OrderPlant;
                string strInOut = dataForm.InOut;
                string strHaoJiu = dataForm.HaoJiu;
                string strSupplierId = dataForm.SupplierId;
                string strSupplierPlant = dataForm.SupplierPlant;
                string strOutPutDate = dataForm.info;//出荷日

                JArray checkedInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();
                if (strOutPutDate == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "info";
                    apiResult.data = "计划出荷日不能为空";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                DataTable dtMessage = fs0603_Logic.createTable("MES");
                DataTable dataTable = fS0616_logic.getSearchInfo(strDelete, strState, listOrderNo, strPartId, strOrderPlant, strInOut, strHaoJiu, strSupplierId, strSupplierPlant);
                bool bReault = true;
                DataTable dtImport = fS0616_logic.checkOutputInfo(listInfoData, dataTable, ref bReault, ref dtMessage);
                if (!bReault)
                {
                    //弹出错误dtMessage
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fS0616_logic.setOutputInfo(dtImport, loginInfo.UserId, ref dtMessage);
                if (dtMessage.Rows.Count != 0)
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M04UE0203", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 导出消息信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string exportmessageApi([FromBody]dynamic data)
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
                DataTable dataTable = fs0603_Logic.createTable("MES");

                for (int i = 0; i < listInfoData.Count; i++)
                {
                    DataRow dataRow = dataTable.NewRow();
                    dataRow["vcMessage"] = listInfoData[i]["vcMessage"].ToString();
                    dataTable.Rows.Add(dataRow);
                }
                string[] fields = { "vcMessage" };

                string filepath = ComFunction.generateExcelWithXlt(dataTable, fields, _webHostEnvironment.ContentRootPath, "MessageList.xlsx", 1, loginInfo.UserId, FunctionID);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0902", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
    }
}