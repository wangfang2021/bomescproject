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

        #region 下载日志文件
        [HttpPost]
        [EnableCors("any")]
        public string downloadLogApi([FromBody] dynamic data)
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
                //判断是否选择了日期，如果没选，提示并返回
                if (dataForm.strDateTime==null || dataForm.strDateTime=="")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请选择一个日期";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                //所选择的年
                string strDateTime = dataForm.strDateTime;
                string strYear = Convert.ToDateTime(strDateTime).ToString("yyyy");
                string strDay = Convert.ToDateTime(strDateTime).ToString("yyyyMMdd");

                
                string path_Year = strYear + System.IO.Path.DirectorySeparatorChar;
                //所选天的文件路径
                string path_Day = path_Year + strDay + ".txt";

                //判断文件是否存在
                bool fileExists = System.IO.File.Exists(_webHostEnvironment.ContentRootPath + System.IO.Path.DirectorySeparatorChar + "Doc" + System.IO.Path.DirectorySeparatorChar + "Log" + System.IO.Path.DirectorySeparatorChar + path_Day);

                if (!fileExists)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "所选日期无记录日志";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = path_Day;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0503", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}
