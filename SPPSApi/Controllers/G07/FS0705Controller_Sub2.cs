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

namespace SPPSApi.Controllers.G07
{
    [Route("api/FS0705_Sub2/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0705Controller_Sub2 : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0705_Logic fs0705_Logic = new FS0705_Logic();
        private readonly string FunctionID = "FS0705";

        public FS0705Controller_Sub2(IWebHostEnvironment webHostEnvironment)
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
                Dictionary<string, object> res = new Dictionary<string, object>();
                FS0701_Logic fS0701_Logic = new FS0701_Logic();
                List<Object> dataList_C001 = ComFunction.convertAllToResult(fS0701_Logic.SearchFZLJ());//发注逻辑

                res.Add("C001", dataList_C001);
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

        #region 检索
        [HttpPost]
        [EnableCors("any")]
        public string searchApi([FromBody] dynamic data)
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
            string strPackNo = dataForm.vcPackNo;           //包装材品番
            string strPackGPSNo = dataForm.vcPackGPSNo;     //GPS品番
            string strFaZhuID = dataForm.vcFaZhuID;         //发注逻辑
            string strTimeFrom = dataForm.dTimeFrom;        //T/C(From)
            string strTimeTo = dataForm.dTimeTo;            //T/C(To)

            try
            {
                DataTable dt = fs0705_Logic.searchPackCompute(strPackNo, strPackGPSNo, strFaZhuID,strTimeFrom, strTimeTo);

                Dictionary<string, object> res = new Dictionary<string, object>();
                int dtCount = 0;
                dtCount = dt == null ? 0 : dt.Rows.Count;
                string strMessage = "共检索到"+dtCount+"条数据";
                res.Add("strMessage", strMessage);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M07UE0506", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "计算过程检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 导出
        [HttpPost]
        [EnableCors("any")]
        public string exportApi([FromBody] dynamic data)
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
                string strPackNo = dataForm.vcPackNo;           //包装材品番
                string strPackGPSNo = dataForm.vcPackGPSNo;     //GPS品番
                string strFaZhuID = dataForm.vcFaZhuID;         //发注逻辑
                string strTimeFrom = dataForm.dTimeFrom;        //T/C(From)
                string strTimeTo = dataForm.dTimeTo;            //T/C(To)

                DataTable dt = fs0705_Logic.searchPackCompute(strPackNo,strPackGPSNo,strFaZhuID,strTimeFrom,strTimeTo);

                if (dt==null || dt.Rows.Count<=0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "导出失败,未检索到任何数据";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                string[] fields = { "vcBianCi","vcPackNo", "vcPackGPSNo","dTimeStr", "iA_SRS", "iB_LastShengYu", "iC_LiLun", "iD_TiaoZheng"
                ,"iE_JinJi","iF_DingGou","iG_ShengYu"
                };
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0705_Export2.xlsx", 2, loginInfo.UserId, FunctionID);
                if (filepath == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "导出失败";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = filepath;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M07UE0504", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}
