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

namespace SPPSApi.Controllers.G03
{
    [Route("api/FS1201_editCalendarData/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1201Controller_Sub_editCalendarData : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        FS1201_Logic fs1201_Logic = new FS1201_Logic();
        private readonly string FunctionID = "FS1201";

        public FS1201Controller_Sub_editCalendarData(IWebHostEnvironment webHostEnvironment)
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
            string vcYear = dataForm.vcYear;
            string vcPorType = dataForm.vcPorType;
            string vcZB = dataForm.vcZB;
            vcPartType = vcPartType == null ? "" : vcPartType;
            vcPlant = vcPlant == null ? "" : vcPlant;
            vcYear = (vcYear == null || vcYear.Length < 1) ? DateTime.Now.Year.ToString() : vcYear;
            vcPorType = vcPorType == null ? "" : vcPorType;
            vcZB = vcZB == null ? "" : vcZB;

            try
            {
                Dictionary<string, object> res = new Dictionary<string, object>();
                List<Object> calendarData1 = ComFunction.convertAllToResult(fs1201_Logic.getRenders(vcYear, "1", vcPorType, vcZB, vcPlant, "#5DAD64"));
                res.Add("calendarData1", calendarData1);
                res.Add("current_Year", vcYear);
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
