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

namespace SPPSApi.Controllers.G13
{
    [Route("api/FS1301/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1301Controller : BaseController
    {
        FS0603_Logic fS0603_Logic = new FS0603_Logic();
        FS1301_Logic fS1301_Logic = new FS1301_Logic();
        private readonly string FunctionID = "FS1301";

        private readonly IWebHostEnvironment _webHostEnvironment;
        public FS1301Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        /// <summary>
        /// 页面初始化
        /// </summary>
        /// <returns></returns>
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

                List<Object> PackingPlantList = ComFunction.convertAllToResult(ComFunction.getTCode("C023"));//包装厂
                List<Object> RolerList = ComFunction.convertAllToResult(fS0603_Logic.getCodeInfo("Role"));//角色Role
                res.Add("PackingPlantList", PackingPlantList);
                res.Add("RolerList", RolerList);

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
        /// <summary>
        /// 查询方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string searchApi([FromBody]dynamic data)
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

            string strPackingPlant = dataForm.PackingPlant == null ? "" : dataForm.PackingPlant;
            string strUser = dataForm.User == null ? "" : dataForm.User;
            string strRoler = dataForm.Roler == null ? "" : dataForm.Roler;
            try
            {
                DataTable dataTable = fS1301_Logic.getSearchInfo(strPackingPlant, strUser, strRoler);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("bInPut", ConvertFieldType.BoolType, null);
                dtConverter.addField("bInPutUnLock", ConvertFieldType.BoolType, null);
                dtConverter.addField("bCheck", ConvertFieldType.BoolType, null);
                dtConverter.addField("bCheckUnLock", ConvertFieldType.BoolType, null);
                dtConverter.addField("bPack", ConvertFieldType.BoolType, null);
                dtConverter.addField("bPackUnLock", ConvertFieldType.BoolType, null);
                dtConverter.addField("bOutPut", ConvertFieldType.BoolType, null);
                dtConverter.addField("bOutPutUnLock", ConvertFieldType.BoolType, null);
                dtConverter.addField("bSelectFlag", ConvertFieldType.BoolType, null);
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dataTable, dtConverter);

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
        /// 更新方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string saveApi([FromBody]dynamic data)
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
            try
            {
                dynamic temp = JsonConvert.DeserializeObject(Convert.ToString(data));
                JArray listInfo = temp.alltemp.list;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
                fS1301_Logic.saveDataInfo(listInfoData, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "更新成功";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0203", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "更新失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }

    }
}
