using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;

namespace SPPSApi.Controllers.G04
{
    [Route("api/FS0402/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0402Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string FunctionID = "FS0402";
        FS0402_Logic fs0402_Logic = new FS0402_Logic();
        FS0603_Logic fs0603_Logic = new FS0603_Logic();
        public FS0402Controller(IWebHostEnvironment webHostEnvironment)
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
                List<Object> dataList_C036 = ComFunction.convertAllToResult(fs0402_Logic.getTCode("C036"));//月度订单对应状态
                List<Object> dataList_C075 = ComFunction.convertAllToResult(ComFunction.getTCode("C075"));//月度订单合意状态

                res.Add("C036", dataList_C036);
                res.Add("C075", dataList_C075);
                DateTime dNow = DateTime.Now.AddMonths(1);
                res.Add("yearMonth", dNow.ToString("yyyy/MM"));

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M04UE0201", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "初始化失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        //#region 获取对象年月 格式YYYY/MM
        //[HttpPost]
        //[EnableCors("any")]
        //public string getYearMonthApi()
        //{

        //    string strToken = Request.Headers["X-Token"];
        //    if (!isLogin(strToken))
        //    {
        //        return error_login();
        //    }
        //    LoginInfo loginInfo = getLoginByToken(strToken);
        //    //以下开始业务处理
        //    ApiResult apiResult = new ApiResult();
            
        //    try
        //    {
        //        DateTime dNow = DateTime.Now.AddMonths(1);

        //        apiResult.code = ComConstant.SUCCESS_CODE;
        //        apiResult.data = dNow.ToString("yyyy/MM");
        //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //    }
        //    catch (Exception ex)
        //    {
        //        ComMessage.GetInstance().ProcessMessage(FunctionID, "M04UE0201", ex, loginInfo.UserId);
        //        apiResult.code = ComConstant.ERROR_CODE;
        //        apiResult.data = "检索失败";
        //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //    }
        //}
        //#endregion

        #region 检索数据
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
            Dictionary<string, object> res = new Dictionary<string, object>();
            //以下开始业务处理
            ApiResult apiResult = new ApiResult();
            dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));

            string strYearMonth = dataForm.YearMonth==null?"": Convert.ToDateTime(dataForm.YearMonth+"/01").ToString("yyyyMM");
            string strDyState = dataForm.DyState == null?"": dataForm.DyState;
            string strHyState = dataForm.HyState == null?"": dataForm.HyState;
            string strPart_id = dataForm.Part_id == null ? "" : dataForm.Part_id;


            try
            {
                DataSet ds = fs0402_Logic.Search(strYearMonth, strDyState, strHyState, strPart_id);
                DataTable dt = ds.Tables[0];
                DataTable dt_heji = ds.Tables[1];
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("dHyTime", ConvertFieldType.DateType, "yyyy/MM/dd HH:mm:ss");
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);
                List<Object> hejiList = ComFunction.convertAllToResult(dt_heji);

                res.Add("dataList", dataList);
                res.Add("hejiList", dt_heji);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M04UE0202", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 承认
        [HttpPost]
        [EnableCors("any")]
        public string okApi([FromBody] dynamic data)
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
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));

                string strYearMonth = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth + "/01").ToString("yyyyMM");
                string strDyState = dataForm.DyState == null ? "" : dataForm.DyState;
                string strHyState = dataForm.HyState == null ? "" : dataForm.HyState;
                string strPart_id = dataForm.Part_id == null ? "" : dataForm.Part_id;
                JArray checkedInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();

                if (strYearMonth == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "对象年月不能为空";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                int count = 0;//影响行数，没啥用
                string strMsg = "";
                DataTable dtMessage = fs0603_Logic.createTable("MES");
                if (listInfoData.Count != 0)//选中了数据操作
                {
                    if(fs0402_Logic.IsDQR(strYearMonth, listInfoData,ref strMsg, ref dtMessage))
                    {//全是待确认的
                        count = fs0402_Logic.ok(strYearMonth, listInfoData, loginInfo.UserId);
                    }
                    else
                    {//有不是待确认的数据
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.type = "list";
                        apiResult.data = dtMessage;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                else//按检索条件
                {
                    if (fs0402_Logic.IsDQR(strYearMonth, strDyState, strHyState, strPart_id, ref strMsg, ref dtMessage))
                    {//全是待确认的
                        count = fs0402_Logic.ok(strYearMonth, strDyState, strHyState, strPart_id, loginInfo.UserId);
                    }
                    else 
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.type = "list";
                        apiResult.data = dtMessage;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = count;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M04UE0203", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "承认失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 退回
        [HttpPost]
        [EnableCors("any")]
        public string ngApi([FromBody] dynamic data)
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
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));

                string strYearMonth = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth + "/01").ToString("yyyyMM");
                string strDyState = dataForm.DyState == null ? "" : dataForm.DyState;
                string strHyState = dataForm.HyState == null ? "" : dataForm.HyState;
                string strPart_id = dataForm.Part_id == null ? "" : dataForm.Part_id;
                JArray checkedInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();

                if (strYearMonth == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "对象年月不能为空";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //合意状态是待确认，才能退回


                int count = 0;//影响行数，没啥用
                string strMsg = "";
                DataTable dtMessage = fs0603_Logic.createTable("MES");
                if (listInfoData.Count != 0)//选中了数据操作
                {
                    if (fs0402_Logic.IsDQR(strYearMonth, listInfoData, ref strMsg, ref dtMessage))
                    {//全是待确认的
                        count = fs0402_Logic.ng(strYearMonth, listInfoData, loginInfo.UserId);
                    }
                    else
                    {//有不是待确认的数据
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.type = "list";
                        apiResult.data = dtMessage;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                else//按检索条件
                {
                    if (fs0402_Logic.IsDQR(strYearMonth, strDyState, strHyState, strPart_id, ref strMsg, ref dtMessage))
                    {//全是待确认的
                        count = fs0402_Logic.ng(strYearMonth, strDyState, strHyState, strPart_id, loginInfo.UserId);
                    }
                    else
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.type = "list";
                        apiResult.data = dtMessage;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = count;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M04UE0204", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "退回失败";
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

            string strYearMonth = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth).ToString("yyyyMM");
            string strYearMonth_2 = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth).AddMonths(1).ToString("yyyyMM");
            string strYearMonth_3 = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth).AddMonths(2).ToString("yyyyMM");


           

            string strDyState = dataForm.DyState == null ? "" : dataForm.DyState;
            string strHyState = dataForm.HyState == null ? "" : dataForm.HyState;
            string strPart_id = dataForm.Part_id == null ? "" : dataForm.Part_id;

            try
            {
                DataTable dt = fs0402_Logic.Search(strYearMonth, strDyState, strHyState, strPart_id).Tables[0];
                string[] fields = { "vcYearMonth", "vcDyState_Name", "vcHyState_Name", "vcPart_id_export", "iCbSOQN", "decCbBdl"
                ,"iCbSOQN1","iCbSOQN2","iTzhSOQN","iTzhSOQN1","iTzhSOQN2","iHySOQN","iHySOQN1","iHySOQN2"
                ,"dHyTime"
                };
                string filepath = fs0402_Logic.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0402_Export.xlsx", 2, loginInfo.UserId, FunctionID, strYearMonth, strYearMonth_2, strYearMonth_3);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M04UE0205", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 导入履历导出
        [HttpPost]
        [EnableCors("any")]
        public string exportApi_history([FromBody] dynamic data)
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
                DataTable dt = fs0402_Logic.SearchHistory();
                string[] heads = { "对象年月","品番", "错误消息"};
                string[] fields = { "vcYearMonth", "vcPart_id_export", "vcMessage" };
                string strMsg = "";
                string filepath = ComFunction.DataTableToExcel(heads, fields, dt, _webHostEnvironment.ContentRootPath, loginInfo.UserId, FunctionID, ref strMsg);
                if (strMsg != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = strMsg;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = filepath;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M04UE0206", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "履历导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

    }
}