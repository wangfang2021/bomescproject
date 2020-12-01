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
            string strUserId = ComFunction.Decrypt(strToken);
            //以下开始业务处理
            ApiResult apiResult = new ApiResult();
            dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
            string vcFunctionID = dataForm.vcFunctionID;
            vcFunctionID = vcFunctionID == null ? "" : vcFunctionID;
            try
            {
                DataTable dt = fs0104_Logic.Search(vcFunctionID);
                List<Object> dataList = ComFunction.convertToResult(dt, new string[] { "vcFunctionID", "vcLogType", "vcMessage", "vcException", "vcTrack", "dCreateTime", "vcIp", "vcUserName" });
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0104", ex, strUserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}
