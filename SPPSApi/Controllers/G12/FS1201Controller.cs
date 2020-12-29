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

namespace SPPSApi.Controllers.G12
{
    [Route("api/FS1201/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1201Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        FS1201_Logic logic = new FS1201_Logic();
        private readonly string FunctionID = "FS1201";

        public FS1201Controller(IWebHostEnvironment webHostEnvironment)
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
                List<Object> ddlPartType = ComFunction.convertAllToResult(logic.bindPartType());
                List<Object> ddlPlant = ComFunction.convertAllToResult(logic.BindPlant());
                List<Object> ddlPorType = ComFunction.convertAllToResult(logic.bindProType());
                res.Add("ddlPartType", ddlPartType);
                res.Add("ddlPlant", ddlPlant);
                res.Add("ddlPorType", ddlPorType);

                //List<Object> calendarData1 = ComFunction.convertAllToResult(logic.getRenders("2015", "1", "非指定", "", "2", ""));
                //res.Add("calendarData1", calendarData1);
                //List<Object> calendarData2 = ComFunction.convertAllToResult(logic.getRenders("2015", "2", "非指定", "", "2", ""));
                //res.Add("calendarData2", calendarData2);
                //List<Object> calendarData3 = ComFunction.convertAllToResult(logic.getRenders("2015", "3", "非指定", "", "2", ""));
                //res.Add("calendarData3", calendarData3);
                //List<Object> calendarData4 = ComFunction.convertAllToResult(logic.getRenders("2015", "4", "非指定", "", "2", ""));
                //res.Add("calendarData4", calendarData4);
                //List<Object> calendarData5 = ComFunction.convertAllToResult(logic.getRenders("2015", "5", "非指定", "", "2", ""));
                //res.Add("calendarData5", calendarData5);
                //List<Object> calendarData6 = ComFunction.convertAllToResult(logic.getRenders("2015", "6", "非指定", "", "2", ""));
                //res.Add("calendarData6", calendarData6);
                //List<Object> calendarData7 = ComFunction.convertAllToResult(logic.getRenders("2015", "7", "非指定", "", "2", ""));
                //res.Add("calendarData7", calendarData7);
                //List<Object> calendarData8 = ComFunction.convertAllToResult(logic.getRenders("2015", "8", "非指定", "", "2", ""));
                //res.Add("calendarData8", calendarData8);
                //List<Object> calendarData9 = ComFunction.convertAllToResult(logic.getRenders("2015", "9", "非指定", "", "2", ""));
                //res.Add("calendarData9", calendarData9);
                //List<Object> calendarData10 = ComFunction.convertAllToResult(logic.getRenders("2015", "10", "非指定", "", "2", ""));
                //res.Add("calendarData10", calendarData10);
                //List<Object> calendarData11 = ComFunction.convertAllToResult(logic.getRenders("2015", "11", "非指定", "", "2", ""));
                //res.Add("calendarData11", calendarData11);
                //List<Object> calendarData12 = ComFunction.convertAllToResult(logic.getRenders("2015", "12", "非指定", "", "2", ""));
                //res.Add("calendarData12", calendarData12);

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
        #endregion

        #region 组别联动
        [HttpPost]
        [EnableCors("any")]
        public string changeZBApi([FromBody] dynamic data)
        {
            ApiResult apiResult = new ApiResult();
            dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
            string vcPorType = dataForm.vcPorType;
            vcPorType = vcPorType == null ? "" : vcPorType;
            try
            {
                Dictionary<string, object> res = new Dictionary<string, object>();
                List<Object> ddlZB = ComFunction.convertAllToResult(logic.bindZB(vcPorType));
                res.Add("ddlZB", ddlZB);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M00UE0006", ex, "");
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
            string vcPartType = dataForm.vcPartType;
            string vcPlant = dataForm.vcPlant; 
            string vcYear = dataForm.vcYear;
            string vcPorType = dataForm.vcPorType;
            string vcZB = dataForm.vcZB;
            vcPartType = vcPartType == null ? "" : vcPartType;
            vcPlant = vcPlant == null ? "" : vcPlant;
            vcYear = vcYear == null ? "" : vcYear;
            vcPorType = vcPorType == null ? "" : vcPorType;  
            vcZB = vcZB == null ? "" : vcZB;

            try
            {
                Dictionary<string, object> res = new Dictionary<string, object>();
                List<Object> calendarData1 = ComFunction.convertAllToResult(logic.getRenders(vcYear, "1", vcPorType, vcZB,vcPlant,"#5DAD64"));
                res.Add("calendarData1", calendarData1);
                List<Object> calendarData2 = ComFunction.convertAllToResult(logic.getRenders(vcYear, "2", vcPorType, vcZB,vcPlant,"#5DAD64"));
                res.Add("calendarData2", calendarData2);
                List<Object> calendarData3 = ComFunction.convertAllToResult(logic.getRenders(vcYear, "3", vcPorType, vcZB,vcPlant,"#5DAD64"));
                res.Add("calendarData3", calendarData3);
                List<Object> calendarData4 = ComFunction.convertAllToResult(logic.getRenders(vcYear, "4", vcPorType, vcZB,vcPlant,"#5DAD64"));
                res.Add("calendarData4", calendarData4);
                List<Object> calendarData5 = ComFunction.convertAllToResult(logic.getRenders(vcYear, "5", vcPorType, vcZB,vcPlant,"#5DAD64"));
                res.Add("calendarData5", calendarData5);
                List<Object> calendarData6 = ComFunction.convertAllToResult(logic.getRenders(vcYear, "6", vcPorType, vcZB,vcPlant,"#5DAD64"));
                res.Add("calendarData6", calendarData6);
                List<Object> calendarData7 = ComFunction.convertAllToResult(logic.getRenders(vcYear, "7", vcPorType, vcZB,vcPlant,"#5DAD64"));
                res.Add("calendarData7", calendarData7);
                List<Object> calendarData8 = ComFunction.convertAllToResult(logic.getRenders(vcYear, "8", vcPorType, vcZB,vcPlant,"#5DAD64"));
                res.Add("calendarData8", calendarData8);
                List<Object> calendarData9 = ComFunction.convertAllToResult(logic.getRenders(vcYear, "9", vcPorType, vcZB,vcPlant,"#5DAD64"));
                res.Add("calendarData9", calendarData9);
                List<Object> calendarData10 = ComFunction.convertAllToResult(logic.getRenders(vcYear, "10", vcPorType, vcZB,vcPlant,"#5DAD64"));
                res.Add("calendarData10", calendarData10);
                List<Object> calendarData11 = ComFunction.convertAllToResult(logic.getRenders(vcYear, "11", vcPorType, vcZB,vcPlant,"#5DAD64"));
                res.Add("calendarData11", calendarData11);
                List<Object> calendarData12 = ComFunction.convertAllToResult(logic.getRenders(vcYear, "12", vcPorType, vcZB,vcPlant,"#5DAD64"));
                res.Add("calendarData12", calendarData12);
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
        #endregion
    }
}
