using System;
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

namespace SPPSApi.Controllers.G08
{
    [Route("api/FS0811/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0811Controller : BaseController
    {
        FS0603_Logic fS0603_Logic = new FS0603_Logic();
        FS0811_Logic fS0811_Logic = new FS0811_Logic();
        private readonly string FunctionID = "FS0811";

        private readonly IWebHostEnvironment _webHostEnvironment;
        public FS0811Controller(IWebHostEnvironment webHostEnvironment)
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
                //获取登录人包装厂
                DataTable dtPackingPlant = ComFunction.getTCode("C023");
                if (dtPackingPlant == null || dtPackingPlant.Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "e1";
                    apiResult.data = null;
                    //apiResult.data = "包装厂信息不全请维护";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //包装厂下拉
                List<Object> PackingPlantList = ComFunction.convertAllToResult(dtPackingPlant);//包装厂
                res.Add("PackingPlantList", PackingPlantList);
                string strPackingPlant = dtPackingPlant.Rows[0]["vcValue"].ToString();
                int code = 0;
                string type = "";
                res = fS0811_Logic.setLoadPage(res, strPackingPlant, ref type, ref code);
                apiResult.code = code;
                apiResult.type = type;
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
        /// 下拉框触发方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string selectApi([FromBody]dynamic data)
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

            string strPackingPlant = dataForm.selectVaule == null ? "" : dataForm.selectVaule;
            try
            {
                Dictionary<string, object> res = new Dictionary<string, object>();
                int code = 0;
                string type = "";
                res = fS0811_Logic.setLoadPage(res, strPackingPlant, ref type, ref code);
                apiResult.code = code;
                apiResult.type = type;
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
        /// 查询方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string searchApi([FromBody]dynamic data)
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

            string strHosDate = dataForm.HosDate == null ? "" : dataForm.HosDate;
            string strBanZhi = dataForm.BanZhi == null ? "" : dataForm.BanZhi;
            Dictionary<string, object> res = new Dictionary<string, object>();
            try
            {

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0204", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 计算方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string queryApi([FromBody]dynamic data)
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
                dynamic data_hidinfo = dataForm.hidinfo;
                dynamic data_queryinfo = dataForm.queryinfo;
                dynamic data_rowinfo = dataForm.rowinfo;

                string strbz = data_hidinfo.bz == null ? "" : data_hidinfo.bz.ToString();
                string struuid = data_hidinfo.uuid == null ? "" : data_hidinfo.uuid.ToString();

                string strPackingPlant = data_queryinfo.PackingPlant == null ? "" : data_queryinfo.PackingPlant.ToString();
                string strPeopleNum = data_queryinfo.PeopleNum == null ? "" : data_queryinfo.PeopleNum.ToString();
                string strCycleTime = data_queryinfo.CycleTime == null ? "" : data_queryinfo.CycleTime.ToString();
                //string strObjective = data_queryinfo.Objective == null ? "" : data_queryinfo.Objective.ToString();
                string strWorkOverTime = data_queryinfo.WorkOverTime == null ? "" : data_queryinfo.WorkOverTime.ToString();

                JArray listInfo = data_rowinfo;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
                DataTable dtMessage = fS0603_Logic.createTable("MES");


                if (strbz == "" || strbz == "9999-12-31(白)")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "班值信息为空无法计算";
                    dtMessage.Rows.Add(dataRow);
                }
                if (strPackingPlant == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "包装厂信息为空无法计算";
                    dtMessage.Rows.Add(dataRow);

                }
                if (strPeopleNum == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "最大包装持有人数为空无法计算";
                    dtMessage.Rows.Add(dataRow);

                }
                if (strCycleTime == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "作业时时间为空无法计算";
                    dtMessage.Rows.Add(dataRow);
                }
                if (listInfoData.Count == 0)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "品目列表或实行计划为空无法计算";
                    dtMessage.Rows.Add(dataRow);
                }
                DataTable dtImport = fS0811_Logic.checkSaveData(listInfoData, strPeopleNum, strCycleTime, ref strWorkOverTime, ref dtMessage);
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fS0811_Logic.queryData(strPackingPlant, strbz.Substring(0, 10), strbz.Substring(11, 1), dtImport, strPeopleNum, strCycleTime, strWorkOverTime, loginInfo.UserId, ref dtMessage);
                //获取计算后结果
                Dictionary<string, object> res = new Dictionary<string, object>();
                int code = 0;
                string type = "";
                res = fS0811_Logic.setTempPage(res, strPackingPlant, strbz.Substring(0, 10), strbz.Substring(11, 1), ref type, ref code);
                apiResult.code = code;
                apiResult.type = type;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0204", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "计算失败";
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
            dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
            try
            {
                dynamic data_hidinfo = dataForm.hidinfo;
                dynamic data_queryinfo = dataForm.queryinfo;
                dynamic data_rowinfo = dataForm.rowinfo;

                string strbz = data_hidinfo.bz == null ? "" : data_hidinfo.bz.ToString();
                string struuid = data_hidinfo.uuid == null ? "" : data_hidinfo.uuid.ToString();

                string strPackingPlant = data_queryinfo.PackingPlant == null ? "" : data_queryinfo.PackingPlant.ToString();

                DataTable dtMessage = fS0603_Logic.createTable("MES");
                DataTable dtRowinfo = fS0603_Logic.createTable("Query811");

                if (strbz == "" || strbz == "9999-12-31(白)")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "班值信息为空无法计算";
                    dtMessage.Rows.Add(dataRow);
                }
                if (strPackingPlant == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "包装厂信息为空无法计算";
                    dtMessage.Rows.Add(dataRow);

                }
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                DataSet dsSaveInfo = fS0811_Logic.getTempData(strPackingPlant, strbz.Substring(0, 10), strbz.Substring(11, 1));
                if (dsSaveInfo == null)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "没有可供保存的数据，请先进行计算";
                    dtMessage.Rows.Add(dataRow);

                }
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fS0811_Logic.setInPutIntoOverInfo(strPackingPlant, strbz.Substring(0, 10), strbz.Substring(11, 1), ref dtMessage);
                Dictionary<string, object> res = new Dictionary<string, object>();
                int code = 0;
                string type = "";
                res = fS0811_Logic.setLoadPage(res, strPackingPlant, ref type, ref code);
                apiResult.code = code;
                apiResult.type = type;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0204", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }

        /// <summary>
        /// 日报
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string exportdayApi([FromBody]dynamic data)
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

                DataTable dtMessage = fS0603_Logic.createTable("MES");
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                {
                    //弹出错误dtMessage
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                DataTable dtDayRef = fS0811_Logic.getDayRef(ref dtMessage);
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                {
                    //弹出错误dtMessage
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string[] fields = {"vcExportDate","vcRefDate","vcMonth_dx","vcMonth_ns","vcMonth_nns","decDXYNum","decNSYNum","decNNSYNum",
                    "vcProject","decSOQ_dx","decSOQ_ns_before","decSOQ_ns","decSOQ_nns_before","decSOQ_nns","decNNA_dx","decNNA_ns","decNNA_nns"};

                string filepath = ComFunction.generateExcelWithXlt(dtDayRef, fields, _webHostEnvironment.ContentRootPath, "FTMS内示总结.xlsx", 1, 1, loginInfo.UserId, FunctionID);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0708", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出报表失败" + ex.Message;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 月报
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string exportmonthApi([FromBody]dynamic data)
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

                DataTable dtMessage = fS0603_Logic.createTable("MES");
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                {
                    //弹出错误dtMessage
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                DataTable dtMonthRef = fS0811_Logic.getMonthRef(ref dtMessage);
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                {
                    //弹出错误dtMessage
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string[] fields = {"vcExportDate","vcRefDate","vcMonth_dx","vcMonth_ns","vcMonth_nns","decDXYNum","decNSYNum","decNNSYNum",
                    "vcProject","decSOQ_dx","decSOQ_ns_before","decSOQ_ns","decSOQ_nns_before","decSOQ_nns","decNNA_dx","decNNA_ns","decNNA_nns"};

                string filepath = ComFunction.generateExcelWithXlt(dtMonthRef, fields, _webHostEnvironment.ContentRootPath, "FTMS内示总结.xlsx", 1, 1, loginInfo.UserId, FunctionID);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0708", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出报表失败" + ex.Message;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
    }
}
