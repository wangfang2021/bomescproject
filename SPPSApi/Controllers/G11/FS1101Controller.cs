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

namespace SPPSApi.Controllers.G11
{
    [Route("api/FS1101/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1101Controller : BaseController
    {
        FS1101_Logic fS1101_Logic = new FS1101_Logic();
        private readonly string FunctionID = "FS1101";

        private readonly IWebHostEnvironment _webHostEnvironment;
        public FS1101Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        /// <summary>
        /// 查询方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string search_api([FromBody]dynamic data)
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
            string strDQuNo = dataForm.sDQuNo == null ? "" : dataForm.sDQuNo;
            string strTrolleyNo = dataForm.sTrolleyNo == null ? "" : dataForm.sTrolleyNo;
            string strPartId = dataForm.sPartId == null ? "" : dataForm.sPartId;
            string strOrderNo = dataForm.sOrderNo == null ? "" : dataForm.sOrderNo;
            string strLianFan = dataForm.sLianFan == null ? "" : dataForm.sLianFan;
            try
            {
                DataTable dt = fS1101_Logic.getSearchInfo(strDQuNo, strTrolleyNo, strPartId, strOrderNo, strLianFan);
                List<Object> dataList = ComFunction.convertAllToResult(dt);
                for (int i = 0; i < dataList.Count; i++)
                {
                    Dictionary<string, object> row = (Dictionary<string, object>)dataList[i];
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
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
        /// <summary>
        /// 印刷方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string print_api([FromBody]dynamic data)
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
            string strSaleno = dataForm.Saleno == null ? "" : dataForm.Saleno;
            try
            {
                string strFilesPath = fS1101_Logic.getPrintFile();
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = strFilesPath;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0204", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "印刷失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
    }
}
