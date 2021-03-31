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
    [Route("api/FS0703/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0703Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0703_Logic FS0703_Logic = new FS0703_Logic();
        private readonly string FunctionID = "FS0703";

        public FS0703Controller(IWebHostEnvironment webHostEnvironment)
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

                List<Object> dataList_C023 = ComFunction.convertAllToResult(ComFunction.getTCode("C023"));//包装场

                res.Add("C023", dataList_C023);
                FS0701_Logic FS0701_Logic = new FS0701_Logic();

                List<Object> dataList_Supplier = ComFunction.convertAllToResult(FS0701_Logic.SearchSupplier());//供应商
                res.Add("optionSupplier", dataList_Supplier);

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

        #region 计算并插入
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

            List<Object> PackSpot = new List<object>();

            if (dataForm.PackSpot.ToObject<List<Object>>() == null)
            {
                PackSpot = new List<object>();
            }
            else
            {
                PackSpot = dataForm.PackSpot.ToObject<List<Object>>();
            }

            string PackFrom = dataForm.dFrom;//对象年月

            List<Object> SupplierCodeList = new List<object>();

            SupplierCodeList = dataForm.vcSupplierCode.ToObject<List<Object>>();
            string strSearchKey = FunctionID + loginInfo.UserId;

            if (PackSpot.Count == 0 || string.IsNullOrEmpty(PackFrom) || SupplierCodeList == null)
            {
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "请填写必要项！";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }

            try
            {

                DataTable dt = FS0703_Logic.Calculation(PackSpot, PackFrom, SupplierCodeList);
                string strErrorPartId = "";
                if (dt.Rows.Count > 0)
                {
                    FS0703_Logic.Save_GS(dt, loginInfo.UserId, ref strErrorPartId);
                }

                //DataTable dtcheck = FS0703_Logic.SearchCheck();

                DataTable dtcope = dt.Copy();
                DataTable dtException = new DataTable();
                dtException.Columns.Add("vcpart_id", Type.GetType("System.String"));
                dtException.Columns.Add("vcException", Type.GetType("System.String"));

                dtcope.TableName = "123";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (string.IsNullOrEmpty(dt.Rows[i]["vcPackNo"].ToString()))
                    {
                        dtcope = new DataTable();
                        DataRow drImport = dtException.NewRow();
                        drImport["vcpart_id"] = dt.Rows[i]["vcpart_id"].ToString();
                        drImport["vcException"] = "没有维护包材品番信息！";
                    }
                    if (string.IsNullOrEmpty(dt.Rows[i]["dUsedFrom"].ToString())|| string.IsNullOrEmpty(dt.Rows[i]["dUsedTo"].ToString()))
                    {
                        dtcope = new DataTable();
                        DataRow drImport = dtException.NewRow();
                        drImport["vcpart_id"] = dt.Rows[i]["vcpart_id"].ToString();
                        drImport["vcException"] = "没有维护包材有效周期！";
                    }
                    else if (!(Convert.ToDateTime(dt.Rows[i]["dUsedFrom"]) < DateTime.Now) && !(DateTime.Now < Convert.ToDateTime(dt.Rows[i]["dUsedTo"])))
                    {
                        dtcope = new DataTable();
                        DataRow drImport = dtException.NewRow();
                        drImport["vcpart_id"] = dt.Rows[i]["vcpart_id"].ToString();
                        drImport["vcException"] = "此包材失效！";
                    }
                }
                FS0703_Logic.InsertCheck(dtException, loginInfo.UserId);

                //放入缓存
                initSearchCash(strSearchKey, dtcope);
                DataTable dtE = FS0703_Logic.SearchExceptionCK();
                //List<Object> dataList = ComFunction.convertAllToResult(dtE);
                Dictionary<string, object> res = new Dictionary<string, object>();
                if (dtE.Rows.Count>0) {

                    initSearchCash("Exception", dtE);
                }
                List<Object> vcException = ComFunction.convertAllToResult(dtE);//
                res.Add("strException", vcException);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "计算失败";
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


            List<Object> PackSpot = new List<object>();

            if (dataForm.PackSpot.ToObject<List<Object>>() == null)
            {
                PackSpot = new List<object>();
            }
            else
            {
                PackSpot = dataForm.PackSpot.ToObject<List<Object>>();
            }
            //包装厂


            string PackFrom = dataForm.dFrom;//对象年月
            List<Object> SupplierCodeList = new List<object>();

            SupplierCodeList = dataForm.vcSupplierCode.ToObject<List<Object>>();
            string strSearchKey = FunctionID + loginInfo.UserId;

            try
            {
                DataTable dt = new DataTable();
                //dt = FS0703_Logic.Search(PackSpot, PackFrom, SupplierCodeList);
                if (isExistSearchCash(strSearchKey))//缓存已经存在，则从缓存中获取
                {
                    dt = getResultCashByKey(strSearchKey);
                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "没有可导出数据！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                if (dt.Rows.Count == 0)
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "没有可导出数据！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }
                string[] fields = { "vcYearMonth","vcPackNo","vcPackSpot","vcSupplierCode","vcSupplierWork","vcSupplierName","vcSupplierPack",
                    "vcCycle","iRelease","iDayNNum","iDayN1Num","iDayN2Num",
                    "iDay1","iDay2","iDay3","iDay4","iDay5","iDay6","iDay7","iDay8","iDay9","iDay10",
                    "iDay11","iDay12","iDay13","iDay14","iDay15","iDay16","iDay17","iDay18","iDay19","iDay20",
                    "iDay21","iDay22","iDay23","iDay24","iDay25","iDay26","iDay27","iDay28","iDay29","iDay30",
                    "iDay31","dZYTime"
                };
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0703_Export.xlsx", 1, loginInfo.UserId, "内示书计算导出");
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

        #region 错误品番导出
        [HttpPost]
        [EnableCors("any")]
        public string exportApiPF([FromBody] dynamic data)
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
                DataTable dt = new DataTable();
                if (isExistSearchCash("Exception"))//缓存已经存在，则从缓存中获取
                {
                    dt = FS0703_Logic.SearchException();
                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请重新计算！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                if (dt.Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "没有可导出数据！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }
                string[] fields = { "vcValue","vcName"
                };
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0703_Exception.xlsx", 1, loginInfo.UserId, "月度内示");
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

        #region 发送
        [HttpPost]
        [EnableCors("any")]
        public string SendApi([FromBody] dynamic data)
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
                JArray listInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();

                List<Object> PackSpot = new List<object>();

                if (dataForm.PackSpot.ToObject<List<Object>>() == null)
                {
                    PackSpot = new List<object>();
                }
                else
                {
                    PackSpot = dataForm.PackSpot.ToObject<List<Object>>();
                }
                //包装厂


                string PackFrom = dataForm.dFrom;//对象年月
                List<Object> SupplierCodeList = new List<object>();

                SupplierCodeList = dataForm.vcSupplierCode.ToObject<List<Object>>();


                string strErrorPartId = "";
                DataTable dt = new DataTable();
                string strSearchKey = FunctionID + loginInfo.UserId;
                if (isExistSearchCash(strSearchKey))//缓存已经存在，则从缓存中获取
                {
                    dt = getResultCashByKey(strSearchKey);
                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "没有可发送数据！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                if (dt.Rows.Count == 0)
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "没有可发送数据！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }

                FS0703_Logic.Save(dt, loginInfo.UserId, ref strErrorPartId,PackFrom, SupplierCodeList);
                if (strErrorPartId != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "发送失败";
                    apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0902", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "发送失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion









    }
}
