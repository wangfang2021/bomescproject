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
                string strPackingPlant = dtPackingPlant.Rows[0][0].ToString();
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
                string strObjective = data_queryinfo.Objective == null ? "" : data_queryinfo.Objective.ToString();
                string strWorkOverTime = data_queryinfo.WorkOverTime == null ? "" : data_queryinfo.WorkOverTime.ToString();

                JArray listInfo = data_rowinfo;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
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
                if (strCycleTime == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "作业时时间为空无法计算";
                    dtMessage.Rows.Add(dataRow);
                }
                if (strObjective == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "个人效率目标为空无法计算";
                    dtMessage.Rows.Add(dataRow);
                }
                if (listInfoData.Count == 0)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "品目列表或包装计划为空无法计算";
                    dtMessage.Rows.Add(dataRow);
                }
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    DataRow drRowinfo = dtRowinfo.NewRow();
                    drRowinfo["uuid"] = listInfoData[i]["uuid"] == null ? "" : listInfoData[i]["uuid"].ToString();
                    drRowinfo["vcPackingPlant"] = listInfoData[i]["vcPackingPlant"] == null ? "" : listInfoData[i]["vcPackingPlant"].ToString();
                    drRowinfo["dHosDate"] = listInfoData[i]["dHosDate"] == null ? "" : listInfoData[i]["dHosDate"].ToString();
                    drRowinfo["vcBanZhi"] = listInfoData[i]["vcBanZhi"] == null ? "" : listInfoData[i]["vcBanZhi"].ToString();
                    drRowinfo["LinId"] = listInfoData[i]["LinId"] == null ? "" : listInfoData[i]["LinId"].ToString();
                    drRowinfo["vcPartItem"] = listInfoData[i]["vcPartItem"] == null ? "" : listInfoData[i]["vcPartItem"].ToString();
                    drRowinfo["vcStandard"] = listInfoData[i]["vcStandard"] == null ? "" : listInfoData[i]["vcStandard"].ToString();
                    drRowinfo["decPackTotalNum"] = listInfoData[i]["decPackTotalNum"] == null ? "" : listInfoData[i]["decPackTotalNum"].ToString();
                    drRowinfo["decPlannedTime"] = listInfoData[i]["decPlannedTime"] == null ? "" : listInfoData[i]["decPlannedTime"].ToString();
                    drRowinfo["decPlannedPerson"] = listInfoData[i]["decPlannedPerson"] == null ? "" : listInfoData[i]["decPlannedPerson"].ToString();
                    drRowinfo["decInputPerson"] = listInfoData[i]["decInputPerson"] == null ? "" : listInfoData[i]["decInputPerson"].ToString();
                    drRowinfo["decInputTime"] = listInfoData[i]["decInputTime"] == null ? "" : listInfoData[i]["decInputTime"].ToString();
                    drRowinfo["decOverFlowTime"] = listInfoData[i]["decOverFlowTime"] == null ? "" : listInfoData[i]["decOverFlowTime"].ToString();
                    drRowinfo["decSysLander"] = listInfoData[i]["decSysLander"] == null ? "" : listInfoData[i]["decSysLander"].ToString();
                    drRowinfo["decDiffer"] = listInfoData[i]["decDiffer"] == null ? "" : listInfoData[i]["decDiffer"].ToString();
                    drRowinfo["bSelectFlag"] = "1";
                    dtRowinfo.Rows.Add(drRowinfo);
                }
                for (int i = 0; i < dtRowinfo.Rows.Count; i++)
                {
                    string strPartItem = dtRowinfo.Rows[i]["vcPartItem"].ToString();
                    string strInputPerson = dtRowinfo.Rows[i]["decInputPerson"].ToString();
                    if (strInputPerson == "" || strInputPerson == "0")
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("品目{0}的人员投入数录入有误)", strPartItem);
                        dtMessage.Rows.Add(dataRow);
                    }
                }
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                decimal decPeopleNum = 0;//最大包装持有人数C2
                decimal decPlannedPerson = 0;//计划人数F22
                for (int i = 0; i < dtRowinfo.Rows.Count; i++)
                {
                    dtRowinfo.Rows[i]["uuid"] = struuid;
                    dtRowinfo.Rows[i]["vcPackingPlant"] = strPackingPlant;
                    dtRowinfo.Rows[i]["dHosDate"] = strbz.Substring(0, 10);
                    dtRowinfo.Rows[i]["vcBanZhi"] = strbz.Substring(11, 1);
                    dtRowinfo.Rows[i]["LinId"] = dtRowinfo.Rows[i]["LinId"].ToString();

                    decimal decInputPerson = Convert.ToDecimal(dtRowinfo.Rows[i]["decInputPerson"].ToString());
                    decimal decInputTime = decInputPerson * (Convert.ToDecimal(strCycleTime) / 60);
                    decimal decPlannedTime = Convert.ToDecimal(dtRowinfo.Rows[i]["decPlannedTime"].ToString());
                    decimal decOverFlowTime = decInputTime - decPlannedTime;
                    decPeopleNum = decPeopleNum + decInputPerson;
                    decPlannedPerson = decPlannedPerson + Convert.ToDecimal(dtRowinfo.Rows[i]["decPlannedPerson"].ToString());
                    //登录人员获取并计算最后两列decSysLander、decDiffer

                    dtRowinfo.Rows[i]["decPlannedTime"] = decInputPerson;
                    dtRowinfo.Rows[i]["decInputTime"] = decInputTime.ToString("#0.00");
                    dtRowinfo.Rows[i]["decOverFlowTime"] = decOverFlowTime.ToString("#0.00");
                    dtRowinfo.Rows[i]["decSysLander"] = "0.00";
                    dtRowinfo.Rows[i]["decDiffer"] = decInputTime;
                }
                decimal decWorkOverTime = 0;//计划人均加班小时数
                if (decPlannedPerson > decPeopleNum)
                {
                    decWorkOverTime = ((decPlannedPerson - decPeopleNum) * (Convert.ToDecimal(strCycleTime) / 60) * 60) / decPeopleNum;
                }

                Dictionary<string, object> res = new Dictionary<string, object>();
                res.Add("PeopleNumItem", decPeopleNum.ToString("#0"));
                res.Add("CycleTimeItem", Convert.ToDecimal(strCycleTime).ToString("#0"));
                res.Add("ObjectiveItem", strObjective);
                res.Add("WorkOverTimeItem", decWorkOverTime.ToString("#0.00"));
                res.Add("dataList", dtRowinfo);
                res.Add("uuidItem", struuid);
                res.Add("BZItem", strbz);
                apiResult.code = ComConstant.SUCCESS_CODE;
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
                string strPeopleNum = data_queryinfo.PeopleNum == null ? "" : data_queryinfo.PeopleNum.ToString();
                string strCycleTime = data_queryinfo.CycleTime == null ? "" : data_queryinfo.CycleTime.ToString();
                string strObjective = data_queryinfo.Objective == null ? "" : data_queryinfo.Objective.ToString();
                string strWorkOverTime = data_queryinfo.WorkOverTime == null ? "" : data_queryinfo.WorkOverTime.ToString();

                JArray listInfo = data_rowinfo;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
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
                if (strCycleTime == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "作业时时间为空无法计算";
                    dtMessage.Rows.Add(dataRow);
                }
                if (strObjective == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "个人效率目标为空无法计算";
                    dtMessage.Rows.Add(dataRow);
                }
                if (listInfoData.Count == 0)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "品目列表或包装计划为空无法计算";
                    dtMessage.Rows.Add(dataRow);
                }
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    DataRow drRowinfo = dtRowinfo.NewRow();
                    drRowinfo["uuid"] = listInfoData[i]["uuid"] == null ? "" : listInfoData[i]["uuid"].ToString();
                    drRowinfo["vcPackingPlant"] = listInfoData[i]["vcPackingPlant"] == null ? "" : listInfoData[i]["vcPackingPlant"].ToString();
                    drRowinfo["dHosDate"] = listInfoData[i]["dHosDate"] == null ? "" : listInfoData[i]["dHosDate"].ToString();
                    drRowinfo["vcBanZhi"] = listInfoData[i]["vcBanZhi"] == null ? "" : listInfoData[i]["vcBanZhi"].ToString();
                    drRowinfo["LinId"] = listInfoData[i]["LinId"] == null ? "" : listInfoData[i]["LinId"].ToString();
                    drRowinfo["vcPartItem"] = listInfoData[i]["vcPartItem"] == null ? "" : listInfoData[i]["vcPartItem"].ToString();
                    drRowinfo["vcStandard"] = listInfoData[i]["vcStandard"] == null ? "" : listInfoData[i]["vcStandard"].ToString();
                    drRowinfo["decPackTotalNum"] = listInfoData[i]["decPackTotalNum"] == null ? "" : listInfoData[i]["decPackTotalNum"].ToString();
                    drRowinfo["decPlannedTime"] = listInfoData[i]["decPlannedTime"] == null ? "" : listInfoData[i]["decPlannedTime"].ToString();
                    drRowinfo["decPlannedPerson"] = listInfoData[i]["decPlannedPerson"] == null ? "" : listInfoData[i]["decPlannedPerson"].ToString();
                    drRowinfo["decInputPerson"] = listInfoData[i]["decInputPerson"] == null ? "" : listInfoData[i]["decInputPerson"].ToString();
                    drRowinfo["decInputTime"] = listInfoData[i]["decInputTime"] == null ? "" : listInfoData[i]["decInputTime"].ToString();
                    drRowinfo["decOverFlowTime"] = listInfoData[i]["decOverFlowTime"] == null ? "" : listInfoData[i]["decOverFlowTime"].ToString();
                    drRowinfo["decSysLander"] = listInfoData[i]["decSysLander"] == null ? "" : listInfoData[i]["decSysLander"].ToString();
                    drRowinfo["decDiffer"] = listInfoData[i]["decDiffer"] == null ? "" : listInfoData[i]["decDiffer"].ToString();
                    drRowinfo["bSelectFlag"] = "1";
                    dtRowinfo.Rows.Add(drRowinfo);
                }
                for (int i = 0; i < dtRowinfo.Rows.Count; i++)
                {
                    string strPartItem = dtRowinfo.Rows[i]["vcPartItem"].ToString();
                    string strInputPerson = dtRowinfo.Rows[i]["decInputPerson"].ToString();
                    if (strInputPerson == "" || strInputPerson == "0")
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("品目{0}的人员投入数录入有误)", strPartItem);
                        dtMessage.Rows.Add(dataRow);
                    }
                }
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                decimal decPeopleNum = 0;//最大包装持有人数C2
                decimal decPlannedPerson = 0;//计划人数F22
                for (int i = 0; i < dtRowinfo.Rows.Count; i++)
                {
                    dtRowinfo.Rows[i]["uuid"] = struuid;
                    dtRowinfo.Rows[i]["vcPackingPlant"] = strPackingPlant;
                    dtRowinfo.Rows[i]["dHosDate"] = strbz.Substring(0, 10);
                    dtRowinfo.Rows[i]["vcBanZhi"] = strbz.Substring(11, 1);
                    dtRowinfo.Rows[i]["LinId"] = dtRowinfo.Rows[i]["LinId"].ToString();

                    decimal decInputPerson = Convert.ToDecimal(dtRowinfo.Rows[i]["decInputPerson"].ToString());
                    decimal decInputTime = decInputPerson * (Convert.ToDecimal(strCycleTime) / 60);
                    decimal decPlannedTime = Convert.ToDecimal(dtRowinfo.Rows[i]["decPlannedTime"].ToString());
                    decimal decOverFlowTime = decInputTime - decPlannedTime;
                    decPeopleNum = decPeopleNum + decInputPerson;
                    decPlannedPerson = decPlannedPerson + Convert.ToDecimal(dtRowinfo.Rows[i]["decPlannedPerson"].ToString());
                    //登录人员获取并计算最后两列decSysLander、decDiffer

                    dtRowinfo.Rows[i]["decPlannedTime"] = decInputPerson;
                    dtRowinfo.Rows[i]["decInputTime"] = decInputTime.ToString("#0.00");
                    dtRowinfo.Rows[i]["decOverFlowTime"] = decOverFlowTime.ToString("#0.00");
                    dtRowinfo.Rows[i]["decSysLander"] = "0.00";
                    dtRowinfo.Rows[i]["decDiffer"] = decInputTime;
                }
                decimal decWorkOverTime = 0;//计划人均加班小时数
                if (decPlannedPerson > decPeopleNum)
                {
                    decWorkOverTime = ((decPlannedPerson - decPeopleNum) * (Convert.ToDecimal(strCycleTime) / 60) * 60) / decPeopleNum;
                }
                fS0811_Logic.setInPutIntoOverInfo(dtRowinfo,
            struuid, decPeopleNum.ToString("#0"), Convert.ToDecimal(strCycleTime).ToString("#0"), strObjective, decWorkOverTime.ToString("#0.00"),
            loginInfo.UserId, ref dtMessage);
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "info";
                    apiResult.data = "保存计算结果失败";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                Dictionary<string, object> res = new Dictionary<string, object>();
                res.Add("PeopleNumItem", decPeopleNum.ToString("#0"));
                res.Add("CycleTimeItem", Convert.ToDecimal(strCycleTime).ToString("#0"));
                res.Add("ObjectiveItem", strObjective);
                res.Add("WorkOverTimeItem", decWorkOverTime.ToString("#0.00"));
                res.Add("dataList", dtRowinfo);
                res.Add("uuidItem", struuid);
                res.Add("BZItem", strbz);
                apiResult.code = ComConstant.SUCCESS_CODE;
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
    }
}
