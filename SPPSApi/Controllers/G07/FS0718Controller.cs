using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
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
    [Route("api/FS0718/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0718Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0718_Logic FS0718_Logic = new FS0718_Logic();
        private readonly string FunctionID = "FS0718";

        public FS0718Controller(IWebHostEnvironment webHostEnvironment)
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
                DataTable DToptionC001 = new DataTable();
                DToptionC001.Columns.Add("vcName");
                DToptionC001.Columns.Add("vcValue");
                DataRow dr = DToptionC001.NewRow();
                dr["vcName"] = "已下载";
                dr["vcValue"] = "is not null";
                DToptionC001.Rows.Add(dr);
                dr = DToptionC001.NewRow();
                dr["vcName"] = "未下载";
                dr["vcValue"] = "is null";
                DToptionC001.Rows.Add(dr);
                List<Object> dataList_C001 = ComFunction.convertAllToResult(DToptionC001);
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

            try
            {
                string strDownloadDiff = dataForm.strDownloadDiff;
                DataTable dt = FS0718_Logic.Search(strDownloadDiff,loginInfo.UserId);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("dFaBuTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dFirstDownload", ConvertFieldType.DateType, "yyyy/MM/dd");
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
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
                JArray listInfo = dataForm.multipleSelection;
                if (listInfo.Count<=0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "至少选择一条数据！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();

                string strSupplier = dataForm.strSupplier;
                string strYearMonth = dataForm.strYearMonth;
                string strNSDiff = dataForm.strNSDiff;
                string strNSQJ = dataForm.strNSQJ;
                string strNSState = dataForm.strNSState;
                string dFaBuTime = dataForm.strFaBuTime;
                string dFirstDownload = dataForm.dFirstDownload;

                DataTable dt = FS0718_Logic.Search();
                string[] fields = { "iAutoId","vcChange_Name", "vcPart_id", "dUseBeginStr", "dUseEndStr", "vcProjectType_Name", "vcSupplier_id"
                ,"vcSupplier_Name","dProjectBeginStr","dProjectEndStr","vcHaoJiu_Name","dJiuBeginStr","dJiuEndStr","dJiuBeginSustainStr","vcPriceChangeInfo_Name"
                ,"vcPriceState_Name","dPriceStateDateStr","vcPriceGS_Name","decPriceOrigin","decPriceAfter","decPriceTNPWithTax","dPricebeginStr","dPriceEndStr"
                ,"vcCarTypeDev","vcCarTypeDesign","vcPart_Name","vcOE_Name","vcPart_id_HK","vcStateFX","vcFXNO","vcSumLater","vcReceiver_Name"
                ,"vcOriginCompany_Name"
                };
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0309_Export.xlsx", 2, loginInfo.UserId, FunctionID);
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
        #endregion
    }
}
