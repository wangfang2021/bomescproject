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
    [Route("api/FS1204_Sub_editCalendarData/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1204Controller_Sub_editCalendarData : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        FS1204_Logic fs1204_Logic = new FS1204_Logic();
        private readonly string FunctionID = "FS1204";

        public FS1204Controller_Sub_editCalendarData(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
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
            string vcPartType = dataForm.vcPartType;
            string vcPlant = dataForm.vcPlant;
            string vcYearMonth = dataForm.vcMonth;
            string vcPorType = dataForm.vcPorType;
            string vcZB = dataForm.vcZB;
            vcPartType = vcPartType == null ? "" : vcPartType;
            vcPlant = vcPlant == null ? "" : vcPlant;
            vcYearMonth = (vcYearMonth == null || vcYearMonth.Length < 1) ? DateTime.Now.Year.ToString() : vcYearMonth;
            string vcYear = vcYearMonth.Split('-')[0];
            string vcMonth = vcYearMonth.Split('-')[1];
            vcPorType = vcPorType == null ? "" : vcPorType;
            vcZB = vcZB == null ? "" : vcZB;
            try
            {
                Dictionary<string, object> res = new Dictionary<string, object>();
                List<Object> calendarData = ComFunction.convertAllToResult(fs1204_Logic.getRenders(vcYear, vcMonth, vcPorType, vcZB, vcPlant, "#5DAD64"));
                res.Add("calendarData", calendarData);
                res.Add("current_Year", vcMonth);
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

        /// <summary>
        /// 更新稼动日数据
        /// </summary>
        [HttpPost]
        [EnableCors("any")]
        public string UpdateCalendar([FromBody] dynamic data)
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
                string vcPartType = dataForm.vcPartType;
                string vcPlant = dataForm.vcPlant;
                string vcYearMonth = dataForm.vcMonth;
                string vcPorType = dataForm.vcPorType;
                string vcZB = dataForm.vcZB;
                vcPartType = vcPartType == null ? "" : vcPartType;
                vcPlant = vcPlant == null ? "" : vcPlant;
                vcYearMonth = (vcYearMonth == null || vcYearMonth.Length < 1) ? DateTime.Now.Year.ToString() : vcYearMonth;
                string vcYear = vcYearMonth.Split('-')[0];
                string vcMonth = vcYearMonth.Split('-')[1].Length == 1 ? "0" + vcYearMonth.Split('-')[1] : vcYearMonth.Split('-')[1];
                vcPorType = vcPorType == null ? "" : vcPorType;
                vcZB = vcZB == null ? "" : vcZB;

                byte[] rep = { 194, 160 };
                string Add_data = dataForm.vcData;
                string[] Udata = Add_data.Replace(System.Text.Encoding.GetEncoding("UTF-8").GetString(rep), "").Split('|');

                fs1204_Logic.UpdateCalendar(vcPlant, vcPorType, vcZB, vcYear, vcMonth, Udata);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "更新成功";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0902", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "更新失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
    }
}
