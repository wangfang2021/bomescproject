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

namespace SPPSApi.Controllers.G03
{
    [Route("api/FS0405/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0405Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0405_Logic fs0405_Logic = new FS0405_Logic();
        private readonly string FunctionID = "FS0405";

        public FS0405Controller(IWebHostEnvironment webHostEnvironment)
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
                
                List<Object> dataList_C003 = ComFunction.convertAllToResult(ComFunction.getTCode("C003"));//内外区分
                res.Add("C003", dataList_C003);
                
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

            string strDXDataMonth = dataForm.strDXDataMonth;
            string strInOutFlag = dataForm.strInOutFlag;

            try
            {
                DataTable dt = fs0405_Logic.Search(strDXDataMonth, strInOutFlag);

                DtConverter dtConverter = new DtConverter();

                dtConverter.addField("dZhanKaiTime", ConvertFieldType.DateType, "yyyy/MM/dd HH:mm:ss");

                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0312", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 下载导出
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
            try
            {
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                JArray listInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();

                if (listInfo==null || listInfoData.Count<=0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请先选择一条数据";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                #region 校验所选数据的状态必须为可下载

                string strZKState = listInfoData[0]["vcZhanKaiState"].ToString();
                if (strZKState!="可下载")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "所选数据不可下载！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                #endregion

                string strDXYM = listInfoData[0]["vcDXYM"].ToString();
                strDXYM = strDXYM.Insert(4, "/");
                strDXYM = strDXYM.Insert(strDXYM.Length, "/01");
                string strInOutFlag = listInfoData[0]["vcInOutFlag"].ToString();
                string strYearMonth = Convert.ToDateTime(strDXYM).ToString("yyyyMM");
                string strYearMonth_2 = Convert.ToDateTime(strDXYM).AddMonths(1).ToString("yyyyMM");
                string strYearMonth_3 = Convert.ToDateTime(strDXYM).AddMonths(2).ToString("yyyyMM");

                DataTable dt = fs0405_Logic.exportSearch(strYearMonth, strYearMonth_2, strYearMonth_3,strInOutFlag);

                #region 对品番进行特殊处理，如果品番长度为12，并且后两位是“00”，则去掉“00”
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string strPartsNo = dt.Rows[i]["PartsNo"].ToString();
                    if (strPartsNo.Length==12 && (strPartsNo.Substring(strPartsNo.Length-2)=="00"))
                    {
                        dt.Rows[i]["PartsNo"] = strPartsNo.Substring(0, strPartsNo.Length - 2);
                    }
                }
                #endregion

                string[] ExcelHeader = { "PartsNo", "发注工厂", "订货方式", "CFC", "OrdLot", "N Units"
                ,"N PCS","iD1","iD2","iD3","iD4","iD5","iD6","iD7","iD8","iD9","iD10","iD11","iD12","iD13","iD14"
                ,"iD15","iD16","iD17","iD18","iD19","iD20","iD21","iD22","iD23","iD24","iD25","iD26","iD27","iD28"
                ,"iD29","iD30","iD31","N+1 O/L","N+1 Units","N+1 PCS","N+2 O/L","N+2 Units","N+2 PCS" };

                //string[] DataTableHeader = { "PartsNo", "发注工厂", "订货方式", "CFC", "OrdLot", "N Units"
                //,"N PCS","iD1","iD2","iD3","iD4","iD5","iD6","iD7","iD8","iD9","iD10","iD11","iD12","iD13","iD14"
                //,"iD15","iD16","iD17","iD18","iD19","iD20","iD21","iD22","iD23","iD24","iD25","iD26","iD27","iD28"
                //,"iD29","iD30","iD31","N+1 O/L","N+1 Units","N+1 PCS","N+2 O/L","N+2 Units","N+2 PCS"
                //};

                string strFileName = "SOQREP_" + "0_" + strYearMonth + "_" + strInOutFlag + "_" + DateTime.Now.ToString("yyyy")+DateTime.Now.ToString("MM")+DateTime.Now.ToString("dd")+DateTime.Now.ToString("HH")+DateTime.Now.ToString("mm")+DateTime.Now.ToString("ss")+".xlsx";

                string RetMsg = "";
                string filepath = fs0405_Logic.generateExcelWithXlt(dt, ExcelHeader, _webHostEnvironment.ContentRootPath, "FS0405.xlsx", 1, loginInfo.UserId, strFileName, true);

                if (RetMsg != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = RetMsg;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                if (string.IsNullOrEmpty(filepath))
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "下载失败";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = filepath;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0312", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

    }
}
