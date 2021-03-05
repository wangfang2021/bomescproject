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

namespace SPPSApi.Controllers.G06
{
    [Route("api/FS0608_Sub/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0608Controller_Sub : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        FS0608_Logic logic = new FS0608_Logic();
        private readonly string FunctionID = "FS0608";

        public FS0608Controller_Sub(IWebHostEnvironment webHostEnvironment)
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
                Dictionary<string, Object> res = new Dictionary<string, Object>();
                //发注工厂
                List<Object> dataList_PlantSource = ComFunction.convertAllToResult(ComFunction.getTCode("C000"));
                res.Add("dataList_PlantSource", dataList_PlantSource);
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

        #region 保存数据
        [HttpPost]
        [EnableCors("any")]
        public string saveApi([FromBody] dynamic data)
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
            string vcPlantFrom = dataForm.vcPlantFrom; //源工厂
            List<string> vcPlantTo = dataForm.vcPlantTo.ToObject<List<string>>(); //目标工厂
            List<string> vcMon = new List<string>();//源年月
            vcMon.Add(Convert.ToDateTime(dataForm.vcMonFrom1).ToString("yyyyMM"));
            vcMon.Add(Convert.ToDateTime(dataForm.vcMonFrom2).ToString("yyyyMM"));
            vcMon.Add(Convert.ToDateTime(dataForm.vcMonFrom3).ToString("yyyyMM"));
            try
            {
                string msg = logic.CopyTo(vcPlantFrom, vcPlantTo, vcMon, loginInfo.UserId);
                if (msg.Length > 0)
                {
                    apiResult.data = msg;
                    apiResult.code = ComConstant.ERROR_CODE;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.data = "复制工作日历成功！";
                apiResult.code = ComConstant.SUCCESS_CODE;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0104", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "复制失败！";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}
