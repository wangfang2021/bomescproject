using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SPPSApi.Controllers.G13
{
    [Route("api/FS1306/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1306Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        FS0603_Logic fS0603_Logic = new FS0603_Logic();
        FS1306_Logic fS1306_Logic = new FS1306_Logic();
        private readonly string FunctionID = "FS1306";
        public FS1306Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        
        [HttpPost]
        [EnableCors("any")]
        public string pageloadApi([FromBody] dynamic data)
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

            try
            {
                Dictionary<string, object> res = new Dictionary<string, object>();
                DataTable dtMessage = fS0603_Logic.createTable("MES");
                //获取登录人包装厂
                DataTable dtPackingPlant = ComFunction.getTCode("C023");
                if (dtPackingPlant == null || dtPackingPlant.Rows.Count == 0)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "包装厂信息为空，请维护";
                    dtMessage.Rows.Add(dataRow);
                }
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //包装厂下拉
                List<Object> PackingPlantList = ComFunction.convertAllToResult(dtPackingPlant);//包装厂
                res.Add("PackingPlantList", PackingPlantList);
                string strPackingPlant = dtPackingPlant.Rows[0]["vcValue"].ToString();
                string strOverTime_plan = "0.00";
                string strOverTime_now = "0.00";
                fS1306_Logic.getOperEfficacyInfo("H2", "000000", "10");
                DataTable dtDataInfo = fS1306_Logic.getDataInfo(strPackingPlant, ref strOverTime_plan, ref strOverTime_now, ref dtMessage);
                DtConverter dtConverter = new DtConverter();
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dtDataInfo, dtConverter);
                res.Add("tempList", dataList);
                res.Add("OverTime_plan", strOverTime_plan);
                res.Add("OverTime_now", strOverTime_now);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M13PE0600", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "初始化失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }

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
            string strPackingPlant = dataForm.selectVaule == null ? "" : dataForm.selectVaule;

            try
            {
                Dictionary<string, object> res = new Dictionary<string, object>();
                DataTable dtMessage = fS0603_Logic.createTable("MES");
                //获取登录人包装厂
                string strOverTime_plan = "0.00";
                string strOverTime_now = "0.00";

                DataTable dtDataInfo = fS1306_Logic.getDataInfo(strPackingPlant, ref strOverTime_plan, ref strOverTime_now, ref dtMessage);
                DtConverter dtConverter = new DtConverter();
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dtDataInfo, dtConverter);
                res.Add("tempList", dataList);
                res.Add("OverTime_plan", strOverTime_plan);
                res.Add("OverTime_now", strOverTime_now);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M13PE0601", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "页面加载失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
    }
}