using System;
using System.Collections.Generic;
using System.Data;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace SPPSApi.Controllers.G03
{
    [Route("api/FS0302/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0302Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string FunctionID = "FS0302";
        FS0302_Logic fs0302_logic = new FS0302_Logic();

        public FS0302Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        #region 初始化页面

        [HttpPost]
        [EnableCors("any")]
        public string PageLoad([FromBody]dynamic data)
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
            string fileName = dataForm.fileName == null ? "" : dataForm.fileName;

            try
            {
                Dictionary<string, object> res = new Dictionary<string, object>();

                List<Object> dataList_C014 = ComFunction.convertAllToResult(ComFunction.getTCode("C014"));//完成状态
                List<Object> dataList_C015 = ComFunction.convertAllToResult(ComFunction.getTCode("C015"));//变更事项


                res.Add("C014", dataList_C014);
                res.Add("C015", dataList_C015);



                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;

                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0201", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }

        #endregion


    }
}