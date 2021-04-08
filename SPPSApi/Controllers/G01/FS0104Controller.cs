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
 


namespace SPPSApi.Controllers.G00
{
    [Route("api/FS0104/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0104Controller : BaseController
    {
        FS0104_Logic fs0104_Logic = new FS0104_Logic();
        private readonly string FunctionID = "FS0104";
        
        private readonly IWebHostEnvironment _webHostEnvironment;
        

        public FS0104Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }



        #region 检索所有角色
        [HttpPost]
        [EnableCors("any")]
        public string searchApi([FromBody]dynamic data)
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
            string vcFunctionID = dataForm.vcFunctionID;
            string vcLogType = dataForm.vcLogType;
            string vcTimeFrom = dataForm.vcTimeFrom;
            string vcTimeTo = dataForm.vcTimeTo;
            vcFunctionID = vcFunctionID == null ? "" : vcFunctionID;
            vcLogType = vcLogType == null ? "" : vcLogType;
            vcTimeFrom = vcTimeFrom == null ? "" : vcTimeFrom;
            vcTimeTo = vcTimeTo == null ? "" : vcTimeTo;
            try
            {
                DataTable dt = fs0104_Logic.Search(vcFunctionID, vcLogType, vcTimeFrom, vcTimeTo);
                List<Object> dataList = ComFunction.convertToResult(dt, new string[] { "vcFunctionID", "vcLogType", "vcMessage", "vcException", "vcTrack", "dCreateTime", "vcIp", "vcUserName" });
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0104", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 下载后台日志文件
        [HttpPost]
        [EnableCors("any")]
        public string downloadLLoadApi([FromBody] dynamic data)
        {
            string strtoken = Request.Headers["x-token"];
            if (!isLogin(strtoken))
            {
                return error_login();
            }
            LoginInfo logininfo = getLoginByToken(strtoken);
            //以下开始业务处理
            ApiResult apiresult = new ApiResult();
            dynamic dataform = JsonConvert.DeserializeObject(Convert.ToString(data));
            try
            {
                string strFileName = dataform.fileName;
                strFileName = "-" + strFileName.Substring(strFileName.Length - 5);
                string fileSavePath = strFileName + ".pdf";
                apiresult.code = ComConstant.SUCCESS_CODE;
                apiresult.data = fileSavePath;
                return JsonConvert.SerializeObject(apiresult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0408", ex, logininfo.UserId);
                apiresult.code = ComConstant.ERROR_CODE;
                apiresult.data = "导出失败";
                return JsonConvert.SerializeObject(apiresult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}
