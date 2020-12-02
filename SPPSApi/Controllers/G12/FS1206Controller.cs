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

namespace SPPSApi.Controllers.G12
{
    [Route("api/FS1206/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1206Controller : BaseController
    {
        FS1206_Logic logic = new FS1206_Logic();
        private readonly string FunctionID = "FS1206";
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FS1206Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        #region 点击检索按钮获取列表数据
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
            string strUserId = ComFunction.Decrypt(strToken);
            //以下开始业务处理
            ApiResult apiResult = new ApiResult();
            dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
            string vcPartsNo = dataForm.vcPartsNo;
            vcPartsNo = vcPartsNo == null ? "" : vcPartsNo;
            if (!string.IsNullOrEmpty(vcPartsNo))
            {
                vcPartsNo = vcPartsNo.Replace("-", "").ToString();
            }
            string vcMon = DateTime.Now.ToString("yyyy-MM");
            try
            {
                DataTable dt = logic.Search(vcPartsNo, vcMon);
                List<Object> dataList = ComFunction.convertToResult(dt, new string[] { "vcPartsNo", "vcPartsNoFZ", "vcSource", "vcFactory", "vcBF", "iSRNum", "iCONum" });
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

        [HttpPost]
        [EnableCors("any")]
        public string InUpdeOldData([FromBody] dynamic data)
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
            try
            {
                DataTable tb = JsonConvert.DeserializeObject<DataTable>(Convert.ToString(data));
                string msg = logic.InUpdeOldData(tb, strUserId);
                if (!string.IsNullOrEmpty(msg))
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "保存失败";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "保存成功";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0202", ex, strUserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }

        }
    }
}
