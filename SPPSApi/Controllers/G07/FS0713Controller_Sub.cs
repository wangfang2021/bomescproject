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
    [Route("api/FS0713_Sub/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0713_Controller_Sub : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0713_Logic FS0713_Logic = new FS0713_Logic();
        private readonly string FunctionID = "FS0713_Sub";

        public FS0713_Controller_Sub(IWebHostEnvironment webHostEnvironment)
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
                FS0701_Logic FS0701_Logic = new FS0701_Logic();
                List<Object> dataList_C023 = ComFunction.convertAllToResult(FS0701_Logic.SearchPackSpot(loginInfo.UserId));//包装场

                res.Add("C023", dataList_C023);
                List<Object> dataList_Supplier = ComFunction.convertAllToResult(FS0713_Logic.SearchSupplier());//供应商
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


        #region 在库计算页面初始化
        [HttpPost]
        [EnableCors("any")]
        public string pageloadZKApi()
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
                List<Object> dataList_Supplier = ComFunction.convertAllToResult(FS0713_Logic.SearchSupplier());//供应商
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


        #region 在库安全时段初始化
        [HttpPost]
        [EnableCors("any")]
        public string searchSaveTimeApi([FromBody] dynamic data)
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
                DataTable dt = FS0713_Logic.SearchSaveDate();
                DtConverter dtConverter = new DtConverter();
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


        #region 消耗计算
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


            string PackNo = dataForm.PackNO;//包材品番
            string PackGPSNo = dataForm.PackGPSNo;//GPS品番


            List<Object> strSupplierCode = new List<object>();

            if (dataForm.Supplier.ToObject<List<Object>>() == null)
            {
                strSupplierCode = new List<object>();
            }
            else
            {
                strSupplierCode = dataForm.Supplier.ToObject<List<Object>>();
            }

            string strRatio = dataForm.Ratio;
            string StrFrom = dataForm.dFrom;
            string StrTo = dataForm.dTo;
            string strJiSuanType = dataForm.JiSuanType;//计算种类：日，班制，时段
            string strXHJiSuanType = dataForm.XHJiSuanType;//消耗计算方式:平均，峰值
            string strSaveAdvice = dataForm.SaveAdvice;//建议安全在库计算方式 :方法一，方法二


            try
            {
                string strErrorPartId = "";
                DataTable dt = FS0713_Logic.SearchCalcuate(PackSpot, PackNo, PackGPSNo, strSupplierCode, strRatio, StrFrom, StrTo, strJiSuanType, strXHJiSuanType, strSaveAdvice, loginInfo.UserId, ref strErrorPartId);
                string strSearchKey = FunctionID + loginInfo.UserId;
                //放入缓存
                DataTable dt1 = null;
                initSearchCash(strSearchKey, dt);
                dt1 = getResultCashByKey(strSearchKey);
                if (strErrorPartId == "")
                {

                    strErrorPartId = "计算成功！";
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = strErrorPartId;
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

        #region 建议在库计算
        [HttpPost]
        [EnableCors("any")]
        public string searchApiJYZK([FromBody] dynamic data)
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
            string PackNo = dataForm.PackNO;//包材品番
            string PackGPSNo = dataForm.PackGPSNo;//GPS品番
            List<Object> strSupplierCode = new List<object>();

            if (dataForm.Supplier.ToObject<List<Object>>() == null)
            {
                strSupplierCode = new List<object>();
            }
            else
            {
                strSupplierCode = dataForm.Supplier.ToObject<List<Object>>();
            }

            string strRatio = dataForm.Ratio;
            string StrFrom = dataForm.dFrom;
            string StrTo = dataForm.dTo;
            string strJiSuanType = dataForm.JiSuanType;//计算种类：日，班制，时段
            string strXHJiSuanType = dataForm.XHJiSuanType;//消耗计算方式:平均，峰值
            string strSaveAdvice = dataForm.SaveAdvice;//建议安全在库计算方式 :方法一，方法二
            string strSearchKey = FunctionID + loginInfo.UserId;

            try
            {
                string strErrorPartId = "";
                DataTable dtJS = new DataTable();
                if (isExistSearchCash(strSearchKey))//缓存已经存在，则从缓存中获取
                {
                    dtJS = getResultCashByKey(strSearchKey);
                    FS0701_Logic FS0701_Logic = new FS0701_Logic();
                    DataTable dtPackBase = FS0701_Logic.Search(PackSpot, "", "", new List<object>(), "", "", "", "");
                    DataTable dt = FS0713_Logic.SearchJYZKCalcuate(PackSpot, PackNo, PackGPSNo, strSupplierCode, strRatio, StrFrom, StrTo, strJiSuanType, strXHJiSuanType, strSaveAdvice, loginInfo.UserId, ref strErrorPartId, dtJS, dtPackBase);
                   
                }
                else
                {
                    strErrorPartId = "请重新计算平均/峰值！";
                }
                
                //放入缓存
               // initSearchCash(strSearchKey, dt);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = strErrorPartId;
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
            List<Object> PackSpot = new List<object>();

            if (dataForm.PackSpot.ToObject<List<Object>>() == null)
            {
                PackSpot = new List<object>();
            }
            else
            {
                PackSpot = dataForm.PackSpot.ToObject<List<Object>>();
            }


            string PackNo = dataForm.PackNO;//包材品番
            string PackGPSNo = dataForm.PackGPSNo;//GPS品番


            List<Object> strSupplierCode = new List<object>();

            if (dataForm.Supplier.ToObject<List<Object>>() == null)
            {
                strSupplierCode = new List<object>();
            }
            else
            {
                strSupplierCode = dataForm.Supplier.ToObject<List<Object>>();
            }

            string strRatio = dataForm.Ratio;
            string StrFrom = dataForm.dFrom;
            string StrTo = dataForm.dTo;
            string strJiSuanType = dataForm.JiSuanType;//计算种类：日，班制，时段
            string strXHJiSuanType = dataForm.XHJiSuanType;//消耗计算方式:平均，峰值
            string strSaveAdvice = dataForm.SaveAdvice;//建议安全在库计算方式 :方法一，方法二
            string strSearchKey = FunctionID + loginInfo.UserId;
            try
            {

                DataTable dtJS = new DataTable();
                if (isExistSearchCash(strSearchKey))//缓存已经存在，则从缓存中获取
                {
                    dtJS = getResultCashByKey(strSearchKey);
                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "没有可导出数据！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }
                string resMsg = "";
                int ihead = 0;
                int ifields = 0;
                if (strXHJiSuanType == "平均")
                {
                    ihead = dtJS.Columns.Count - 1;
                    ifields = dtJS.Columns.Count - 1;
                }
                else
                {
                    ihead = dtJS.Columns.Count - 2;
                    ifields = dtJS.Columns.Count - 2;

                }
                string[] head = new string[ihead];
                string[] fields = new string[ifields];
                head[0] = "包装场";
                fields[0] = "vcPackSpot";
                head[1] = "包材品番";
                fields[1] = "vcPackNo";
                head[2] = "GPS品番";
                fields[2] = "vcPackGPSNo";
                head[3] = "建议在库";
                fields[3] = "dSaveZK";
                if (strXHJiSuanType == "平均")
                {
                    head[4] = "平均";
                    fields[4] = "vcAvg";
                    if (strJiSuanType == "时段")
                    {
                        for (int i = 1; i <= dtJS.Columns.Count - 6; i++)
                        {
                            head[4 + i] = dtJS.Columns[4 + i].ColumnName.Substring(5, 17);
                            fields[4 + i] = dtJS.Columns[4 + i].ColumnName;
                        }
                    }
                    else
                    {
                        for (int i = 1; i <= dtJS.Columns.Count - 6; i++)
                        {
                            head[4 + i] = dtJS.Columns[4 + i].ColumnName.Substring(5, 5);
                            fields[4 + i] = dtJS.Columns[4 + i].ColumnName;
                        }
                    }

                }
                else
                {
                    int count = 0;
                    if (strJiSuanType == "时段")
                    {
                        for (int i = 1; i <= dtJS.Columns.Count - 16; i++)
                        {
                            head[3 + i] = dtJS.Columns[5 + i].ColumnName.Substring(5, 17);
                            fields[3 + i] = dtJS.Columns[5 + i].ColumnName;
                            count++;
                        }
                    }
                    else
                    {
                        for (int i = 1; i <= dtJS.Columns.Count - 16; i++)
                        {
                            head[3 + i] = dtJS.Columns[5 + i].ColumnName.Substring(5, 5);
                            fields[3 + i] = dtJS.Columns[5 + i].ColumnName;
                            count++;
                        }

                    }

                    for (int j = 1; j <= 10; j++)
                    {
                        head[3 + count + j] = "Max" + j.ToString();
                        fields[3 + count + j] = "vcMax" + j.ToString();
                    }

                }
                string filepath = ComFunction.DataTableToExcel(head, fields, dtJS, _webHostEnvironment.ContentRootPath, loginInfo.UserId, FunctionID, ref resMsg);
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


        #region 保存
        [HttpPost]
        [EnableCors("any")]
        public string SaveTimeApi([FromBody] dynamic data)
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


            string dFrom1 = dataForm.dFrom1;
            string vcIsOrNoKTFrom1 = dataForm.vcIsOrNoKTFrom1 == "是" ? "1" : "0";
            string dTo1 = dataForm.dTo1;
            string vcIsOrNoKT1 = dataForm.vcIsOrNoKT1 == "是" ? "1" : "0";
            DateTime dF1 = new DateTime();
            DateTime dT1 = new DateTime();

            if (!string.IsNullOrEmpty(dFrom1) && !string.IsNullOrEmpty(dTo1))
            {
                dF1 = vcIsOrNoKTFrom1 == "1" ? Convert.ToDateTime(dFrom1).AddDays(1) : Convert.ToDateTime(dFrom1);
                dT1 = vcIsOrNoKT1 == "1" ? Convert.ToDateTime(dTo1).AddDays(1) : Convert.ToDateTime(dTo1);
                if (dF1 > Convert.ToDateTime(dT1))
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "第一行开始时间大于结束时间！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }
            }

            string dFrom2 = dataForm.dFrom2;
            string vcIsOrNoKTFrom2 = dataForm.vcIsOrNoKTFrom2 == "是" ? "1" : "0";
            string dTo2 = dataForm.dTo2;
            string vcIsOrNoKT2 = dataForm.vcIsOrNoKT2 == "是" ? "1" : "0";
            DateTime dF2 = new DateTime();
            DateTime dT2 = new DateTime();

            if (!string.IsNullOrEmpty(dFrom2) && !string.IsNullOrEmpty(dTo2))
            {
                dF2 = vcIsOrNoKTFrom2 == "1" ? Convert.ToDateTime(dFrom2).AddDays(1) : Convert.ToDateTime(dFrom2);
                dT2 = vcIsOrNoKT2 == "1" ? Convert.ToDateTime(dTo2).AddDays(1) : Convert.ToDateTime(dTo2);

                if (dF2 > dT2)
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "第二行开始时间大于结束时间！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }
                if (dF1 <= dT2 && dT1 >= dF2)
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "1-2有重叠时间！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
            }

            string dFrom3 = dataForm.dFrom3;
            string vcIsOrNoKTFrom3 = dataForm.vcIsOrNoKTFrom3 == "是" ? "1" : "0";
            string dTo3 = dataForm.dTo3;
            string vcIsOrNoKT3 = dataForm.vcIsOrNoKT3 == "是" ? "1" : "0";
            DateTime dF3 = new DateTime();
            DateTime dT3 = new DateTime();

            if (!string.IsNullOrEmpty(dFrom3) && !string.IsNullOrEmpty(dTo3))
            {
                dF3 = vcIsOrNoKTFrom3 == "1" ? Convert.ToDateTime(dFrom3).AddDays(1) : Convert.ToDateTime(dFrom3);
                dT3 = vcIsOrNoKT3 == "1" ? Convert.ToDateTime(dTo3).AddDays(1) : Convert.ToDateTime(dTo3);


                if (dF3 > dT3)
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "第三行开始时间大于结束时间！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }
                if (dF1 <= dT3 && dT1 >= dF3)
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "1-3有重叠时间！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (dF2 <= dT3 && dT2 >= dF3)
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "2-3有重叠时间！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
            }


            string dFrom4 = dataForm.dFrom4;
            string vcIsOrNoKTFrom4 = dataForm.vcIsOrNoKTFrom4 == "是" ? "1" : "0";
            string dTo4 = dataForm.dTo4;
            string vcIsOrNoKT4 = dataForm.vcIsOrNoKT4 == "是" ? "1" : "0";
            DateTime dF4 = new DateTime();
            DateTime dT4 = new DateTime();
            if (!string.IsNullOrEmpty(dFrom4) && !string.IsNullOrEmpty(dTo4))
            {
                dF4 = vcIsOrNoKTFrom4 == "1" ? Convert.ToDateTime(dFrom4).AddDays(1) : Convert.ToDateTime(dFrom4);
                dT4 = vcIsOrNoKT4 == "1" ? Convert.ToDateTime(dTo4).AddDays(1) : Convert.ToDateTime(dTo4);


                if (dF4 > dT4)
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "第四行开始时间大于结束时间！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }
                if (dF1 <= dT4 && dT1 >= dF4)
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "1-4有重叠时间！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (dF2 <= dT4 && dT2 >= dF4)
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "2-4有重叠时间！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (dF3 <= dT4 && dT3 >= dF4)
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "3-4有重叠时间！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
            }

            string dFrom5 = dataForm.dFrom5;
            string vcIsOrNoKTFrom5 = dataForm.vcIsOrNoKTFrom5 == "是" ? "1" : "0";
            string dTo5 = dataForm.dTo5;
            string vcIsOrNoKT5 = dataForm.vcIsOrNoKT5 == "是" ? "1" : "0";
            DateTime dF5 = new DateTime();
            DateTime dT5 = new DateTime();
            if (!string.IsNullOrEmpty(dFrom5) && !string.IsNullOrEmpty(dTo5))
            {
                dF5 = vcIsOrNoKTFrom5 == "1" ? Convert.ToDateTime(dFrom5).AddDays(1) : Convert.ToDateTime(dFrom5);
                dT5 = vcIsOrNoKT5 == "1" ? Convert.ToDateTime(dTo5).AddDays(1) : Convert.ToDateTime(dTo5);


                if (dF5 > dT5)
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "第五行开始时间大于结束时间！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }
                if (dF1 <= dT5 && dT1 >= dF5)
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "1-5有重叠时间！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (dF2 <= dT5 && dT2 >= dF5)
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "2-5有重叠时间！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (dF3 <= dT5 && dT3 >= dF5)
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "3-5有重叠时间！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (dF4 <= dT5 && dT4 >= dF5)
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "4-5有重叠时间！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
            }

            string dFrom6 = dataForm.dFrom6;
            string vcIsOrNoKTFrom6 = dataForm.vcIsOrNoKTFrom6 == "是" ? "1" : "0";
            string dTo6 = dataForm.dTo6;
            string vcIsOrNoKT6 = dataForm.vcIsOrNoKT6 == "是" ? "1" : "0";
            DateTime dF6 = new DateTime();
            DateTime dT6 = new DateTime();
            if (!string.IsNullOrEmpty(dFrom6) && !string.IsNullOrEmpty(dTo6))
            {
                dF6 = vcIsOrNoKTFrom6 == "1" ? Convert.ToDateTime(dFrom6).AddDays(1) : Convert.ToDateTime(dFrom6);
                dT6 = vcIsOrNoKT6 == "1" ? Convert.ToDateTime(dTo6).AddDays(1) : Convert.ToDateTime(dTo6);


                if (dF6 > dT6)
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "第六行开始时间大于结束时间！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }
                if (dF1 <= dT6 && dT1 >= dF6)
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "1-6有重叠时间！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (dF2 <= dT6 && dT2 >= dF6)
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "2-6有重叠时间！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (dF3 <= dT6 && dT3 >= dF6)
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "3-6有重叠时间！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (dF4 <= dT6 && dT4 >= dF6)
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "4-6有重叠时间！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (dF5 <= dT6 && dT5 >= dF6)
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "5-6有重叠时间！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
            }

            string dFrom7 = dataForm.dFrom7;
            string vcIsOrNoKTFrom7 = dataForm.vcIsOrNoKTFrom7 == "是" ? "1" : "0";
            string dTo7 = dataForm.dTo7;
            string vcIsOrNoKT7 = dataForm.vcIsOrNoKT7 == "是" ? "1" : "0";
            DateTime dF7 = new DateTime();
            DateTime dT7 = new DateTime();
            if (!string.IsNullOrEmpty(dFrom7) && !string.IsNullOrEmpty(dTo7))
            {
                dF7 = vcIsOrNoKTFrom7 == "1" ? Convert.ToDateTime(dFrom7).AddDays(1) : Convert.ToDateTime(dFrom7);
                dT7 = vcIsOrNoKT7 == "1" ? Convert.ToDateTime(dTo7).AddDays(1) : Convert.ToDateTime(dTo7);


                if (dF7 > dT7)
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "第七行开始时间大于结束时间！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }
                if (dF1 <= dT7 && dT1 >= dF7)
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "1-7有重叠时间！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (dF2 <= dT7 && dT2 >= dF7)
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "2-7有重叠时间！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (dF3 <= dT7 && dT3 >= dF7)
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "3-7有重叠时间！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (dF4 <= dT7 && dT4 >= dF7)
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "4-7有重叠时间！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (dF5 <= dT7 && dT5 >= dF7)
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "5-7有重叠时间！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (dF6 <= dT7 && dT6 >= dF7)
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "6-7有重叠时间！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
            }

            string dFrom8 = dataForm.dFrom8;
            string vcIsOrNoKTFrom8 = dataForm.vcIsOrNoKTFrom8 == "是" ? "1" : "0";
            string dTo8 = dataForm.dTo8;
            string vcIsOrNoKT8 = dataForm.vcIsOrNoKT8 == "是" ? "1" : "0";
            DateTime dF8 = new DateTime();
            DateTime dT8 = new DateTime();
            if (!string.IsNullOrEmpty(dFrom8) && !string.IsNullOrEmpty(dTo8))
            {
                dF8 = vcIsOrNoKTFrom8 == "1" ? Convert.ToDateTime(dFrom8).AddDays(1) : Convert.ToDateTime(dFrom8);
                dT8 = vcIsOrNoKT8 == "1" ? Convert.ToDateTime(dTo8).AddDays(1) : Convert.ToDateTime(dTo8);


                if (dF8 > dT8)
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "第八行开始时间大于结束时间！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }
                if (dF1 <= dT8 && dT1 >= dF8)
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "1-8有重叠时间！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (dF2 <= dT8 && dT2 >= dF8)
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "2-8有重叠时间！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (dF3 <= dT8 && dT3 >= dF8)
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "3-8有重叠时间！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (dF4 <= dT8 && dT4 >= dF8)
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "4-8有重叠时间！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (dF5 <= dT8 && dT5 >= dF8)
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "5-8有重叠时间！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (dF6 <= dT8 && dT6 >= dF8)
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "6-8有重叠时间！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (dF7 <= dT8 && dT7 >= dF8)
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "7-8有重叠时间！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
            }

            try
            {
                string strErrorPartId = "";
                FS0713_Logic.InsertSaveDate(dFrom1, vcIsOrNoKTFrom1, dTo1, vcIsOrNoKT1, dFrom2, vcIsOrNoKTFrom2, dTo2, vcIsOrNoKT2, dFrom3, vcIsOrNoKTFrom3, dTo3, vcIsOrNoKT3,
                    dFrom4, vcIsOrNoKTFrom4, dTo4, vcIsOrNoKT4, dFrom5, vcIsOrNoKTFrom5, dTo5, vcIsOrNoKT5, dFrom6, vcIsOrNoKTFrom6, dTo6, vcIsOrNoKT6,
                    dFrom7, vcIsOrNoKTFrom7, dTo7, vcIsOrNoKT7, dFrom8, vcIsOrNoKTFrom8, dTo8, vcIsOrNoKT8, loginInfo.UserId, ref strErrorPartId
                    );
                DataTable dt = FS0713_Logic.SearchSaveDate();
                DtConverter dtConverter = new DtConverter();
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




    }
}
