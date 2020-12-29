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

namespace SPPSApi.Controllers.G06
{
    [Route("api/FS0617/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0617Controller : BaseController
    {
        FS0617_Logic fS0617_Logic = new FS0617_Logic();
        private readonly string FunctionID = "FS0617";

        private readonly IWebHostEnvironment _webHostEnvironment;
        public FS0617Controller(IWebHostEnvironment webHostEnvironment)
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
                List<Object> PlantAreaList = ComFunction.convertAllToResult(ComFunction.getTCode("C017"));//工区vcValue   vcName
                List<Object> PlantList = ComFunction.convertAllToResult(ComFunction.getTCode("C000"));//工厂vcValue   vcName
                List<Object> CarTypeList = ComFunction.convertAllToResult(ComFunction.getTCode("C098"));//车种vcValue   vcName
                List<Object> RePartyList = ComFunction.convertAllToResult(ComFunction.getTCode("C005"));//收货方vcValue   vcName
                List<Object> SupartyList = ComFunction.convertAllToResult(fS0617_Logic.getSuPartyInfo());//供应商vcValue   vcName
                res.Add("PlantAreaList", PlantAreaList);
                res.Add("PlantList", PlantList);
                res.Add("CarTypeList", CarTypeList);
                res.Add("RePartyList", RePartyList);
                res.Add("SupartyList", SupartyList);

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

            string strPlantArea = dataForm.PlantArea == null ? "" : dataForm.PlantArea;
            string strPlant = dataForm.Plant == null ? "" : dataForm.Plant;
            string strPartId = dataForm.PartId == null ? "" : dataForm.PartId;
            string strCarType = dataForm.CarType == null ? "" : dataForm.CarType;
            string strReParty = dataForm.ReParty == null ? "" : dataForm.ReParty;
            string strSuparty = dataForm.Suparty == null ? "" : dataForm.Suparty;
            try
            {
                DataTable dt = fS0617_Logic.getSearchInfo(strPlantArea, strPlant, strPartId, strCarType, strReParty, strSuparty);
                DtConverter dtConverter = new DtConverter();
                //dtConverter.addField("vcFromTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                //dtConverter.addField("vcToTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);
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
        public string printApi([FromBody]dynamic data)
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
            string strplant = dataForm.Enum == null ? "" : dataForm.Enum;
            string strpartid = dataForm.vcPlantArea == null ? "" : dataForm.vcPlantArea;
            string strcartype = dataForm.vcFZPlant == null ? "" : dataForm.vcFZPlant;
            string strreparty = dataForm.vcPartId == null ? "" : dataForm.vcPartId;
            string strsuparty = dataForm.vcFromTime == null ? "" : dataForm.vcFromTime;
            try
            {
                string strFilesPath = "file:///E:/20200305%E6%A1%8C%E9%9D%A2/9.%E7%89%A9%E6%B5%81%E7%89%A9%E8%B5%84%E7%AE%A1%E7%90%86%E7%B3%BB%E7%BB%9F.pdf";
                //string strFilesPath = fS0617_Logic.getPrintFile(strplant, strpartid, strcartype, strreparty, strsuparty);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = strFilesPath;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0204", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "生成印刷文件失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
    }
}
