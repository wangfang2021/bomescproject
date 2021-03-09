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

namespace SPPSApi.Controllers.G05
{
    [Route("api/FS0504/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1306Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        FS0602_Logic fs0602_Logic = new FS0602_Logic();
        FS0603_Logic fs0603_Logic = new FS0603_Logic();
        FS0402_Logic fs0402_Logic = new FS0402_Logic();
        FS0501_Logic fs0501_Logic = new FS0501_Logic();
        private readonly string FunctionID = "FS0602";
        public FS1306Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        /// <summary>
        /// 检索方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
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

            string strYearMonth = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth + "/01").ToString("yyyyMM");
            string strDyState = dataForm.DyState;
            string strHyState = dataForm.HyState;
            string strPartId = dataForm.PartId;
            string strCarModel = dataForm.CarModel;
            string strInOut = dataForm.InOut;
            string strOrderingMethod = dataForm.OrderingMethod;
            string strOrderPlant = dataForm.OrderPlant;
            string strHaoJiu = dataForm.HaoJiu;
            string strSupplierId = dataForm.SupplierId;
            string strSupplierPlant = dataForm.SupplierPlant;
            string strDataState = dataForm.DataState;

            try
            {
                Dictionary<string, object> res = new Dictionary<string, object>();
                DataTable dataTable = fs0602_Logic.getSearchInfo(strYearMonth, strDyState, strHyState, strPartId, strCarModel,
                    strInOut, strOrderingMethod, strOrderPlant, strHaoJiu, strSupplierId, strSupplierPlant, strDataState);
                DataTable dtHJ = fs0602_Logic.getHeJiInfo(strYearMonth, strDyState, strHyState, strPartId, strCarModel,
                    strInOut, strOrderingMethod, strOrderPlant, strHaoJiu, strSupplierId, strSupplierPlant, strDataState);
                DtConverter dtConverter = new DtConverter(); 
                dtConverter.addField("bModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("bAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("bSelectFlag", ConvertFieldType.BoolType, null);
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dataTable, dtConverter);
                DataTable dtTask = fs0602_Logic.getSearchInfo(strYearMonth, "", "", "", "", "", "", "", "", "", "", "");
                res.Add("tasknum", dtTask.Rows.Count);
                res.Add("taskok", dtTask.Select("vcHyState='2'").Length);
                res.Add("taskng", dtTask.Select("vcHyState='3'").Length);
                res.Add("tempList", dataList);
                res.Add("hejiList", dtHJ);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 下载方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
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

            string strDyState = dataForm.DyState;
            string strHyState = dataForm.HyState;
            string strPartId = dataForm.PartId;
            string strCarModel = dataForm.CarModel;
            string strInOut = dataForm.InOut;
            string strOrderingMethod = dataForm.OrderingMethod;
            string strOrderPlant = dataForm.OrderPlant;
            string strHaoJiu = dataForm.HaoJiu;
            string strSupplierId = dataForm.SupplierId;
            string strSupplierPlant = dataForm.SupplierPlant;
            string strDataState = dataForm.DataState;
            try
            {
                string strYearMonth = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth).ToString("yyyyMM");
                string strYearMonth_2 = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth).AddMonths(1).ToString("yyyyMM");
                string strYearMonth_3 = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth).AddMonths(2).ToString("yyyyMM");
                DataTable dataTable = fs0602_Logic.getSearchInfo(strYearMonth, strDyState, strHyState, strPartId, strCarModel,
                     strInOut, strOrderingMethod, strOrderPlant, strHaoJiu, strSupplierId, strSupplierPlant, strDataState);

                string[] fields = {"vcYearMonth","vcDyState_Name","vcHyState_Name","vcPart_id","vcCarfamilyCode","vcHaoJiu","vcOrderingMethod",
                    "vcOrderPlant","vcInOut",
                    "vcSupplierId","vcSupplierPlant","iPackingQty",
                    "iCbSOQN","decCbBdl","iCbSOQN1","iCbSOQN2",
                    "iTzhSOQN","iTzhSOQN1","iTzhSOQN2",
                    "iHySOQN","iHySOQN1","iHySOQN2",
                    "dExpectTime","dSReplyTime","vcOverDue","dHyTime"
                };

                //string filepath = ComFunction.generateExcelWithXlt(dataTable, fields, _webHostEnvironment.ContentRootPath, "FS0602_Export.xlsx", 1, loginInfo.UserId, FunctionID);
                string filepath = fs0602_Logic.generateExcelWithXlt(dataTable, fields, _webHostEnvironment.ContentRootPath, "FS0602_Export.xlsx", 2, loginInfo.UserId, FunctionID, strYearMonth, strYearMonth_2, strYearMonth_3);
                if (filepath == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "导出生成文件失败";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = filepath;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0904", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
    }
}