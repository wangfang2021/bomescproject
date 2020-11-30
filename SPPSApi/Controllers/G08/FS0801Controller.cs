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
    [Route("api/FS0801/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0801Controller : BaseController
    {
        FS0801_Logic fs0801_Logic = new FS0801_Logic();
        private readonly string FunctionID = "FS0801";
        
        private readonly IWebHostEnvironment _webHostEnvironment;
        
        public FS0801Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        #region 绑定包装厂
        [HttpPost]
        [EnableCors("any")]
        public string bindPlant()
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
            //dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
            //string roleName = dataForm.roleName;
            //roleName = roleName == null ? "" : roleName;
            try
            {
                DataTable dt = fs0801_Logic.BindPlant();
                List<Object> dataList = ComFunction.convertToResult(dt, new string[] { "vcPlantCode", "vcPlantName"});
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0204", ex, strUserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

    }
}
