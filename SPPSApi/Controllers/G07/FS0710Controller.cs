using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
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
using WebServiceAPI;

namespace SPPSApi.Controllers.G07
{
    [Route("api/FS0710/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0710Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0710_Logic FS0710_Logic = new FS0710_Logic();
        private readonly string FunctionID = "FS0710";

        public FS0710Controller(IWebHostEnvironment webHostEnvironment)
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

                List<Object> dataList_Supplier = ComFunction.convertAllToResult(FS0710_Logic.SearchSupplier());//供应商
                res.Add("optionSupplier", dataList_Supplier);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M07UE1001", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "初始化失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 纳入统计
        [HttpPost]
        [EnableCors("any")]
        public string searchApi_NaRu([FromBody] dynamic data)
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

            PackSpot = dataForm.PackSpot.ToObject<List<Object>>();

            if (dataForm.PackSpot.ToObject<List<Object>>() == null)
            {
                PackSpot = new List<object>();
            }
            else
            {
                PackSpot = dataForm.PackSpot.ToObject<List<Object>>();
            }


            //供应商
            List<Object> strSupplierCode = new List<object>();
            if (dataForm.SupplierCode.ToObject<List<Object>>() == null)
            {
                strSupplierCode = new List<object>();
            }
            else
            {
                strSupplierCode = dataForm.SupplierCode.ToObject<List<Object>>();
            }
            string dFrom = dataForm.dFrom;
            string dTo = dataForm.dTo;
            string reason = "";
            try
            {
                DataTable dt = FS0710_Logic.Search_NR(PackSpot, strSupplierCode, dFrom, dTo);
                //插入临时表
                string strErrorPartId = "";

                if (dt.Rows.Count == 0)
                {

                    strErrorPartId = "纳入统计0条！";
                }
                else
                {

                    //插入纳入统计表
                    FS0710_Logic.Save_NR(dt, ref strErrorPartId, loginInfo.UserId);
                    if (strErrorPartId == "")
                    {
                        strErrorPartId = "纳入统计计算成功！";
                        reason = "纳入统计计算成功";

                    }
                    //Crystal
                    for (int i = 0; i < strSupplierCode.Count; i++)
                    {
                        DataTable drc = FS0710_Logic.SearchNRCaystal(strSupplierCode[i].ToString(), PackSpot[0].ToString());
                        if (drc.Rows.Count > 0)
                        {
                            //插入待生成数据
                            FS0710_Logic.InsertCaystal(drc, PackSpot[0].ToString(), strSupplierCode[i].ToString(), dFrom, dTo);
                            #region 调用webApi打印
                            FS0603_Logic fS0603_Logic = new FS0603_Logic();
                            DataTable dtPrinterInfo = fS0603_Logic.getPrinterInfo("包材内示统计", "");
                            if (dtPrinterInfo.Rows.Count != 0)
                            {
                                //创建 HTTP 绑定对象
                                string file_crv = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "CryReports" + Path.DirectorySeparatorChar;
                                var binding = new BasicHttpBinding();
                                //根据 WebService 的 URL 构建终端点对象
                                var endpoint = new EndpointAddress(dtPrinterInfo.Rows[0]["vcWebAPI"].ToString());
                                //创建调用接口的工厂，注意这里泛型只能传入接口
                                var factory = new ChannelFactory<WebServiceAPISoap>(binding, endpoint);
                                //从工厂获取具体的调用实例
                                var callClient = factory.CreateChannel();
                                setCRVToPDFRequestBody Body = new setCRVToPDFRequestBody();
                                Body.strScrpit = "select * from TPackNRTJ_Caystal ";
                                Body.strDiskFileName = "D:/3.启明系统开发/62.2021年补给管理运用平台/WebAPI/WebAPI/内示" + DateTime.Now.ToString("yyyyMMddHHssmm") + ".PDF";
                                Body.strCRVName = file_crv + dtPrinterInfo.Rows[0]["vcReports"].ToString();
                                Body.sqlUserID = dtPrinterInfo.Rows[0]["vcSqlUserID"].ToString();
                                Body.sqlPassword = dtPrinterInfo.Rows[0]["vcSqlPassword"].ToString();
                                Body.sqlCatalog = dtPrinterInfo.Rows[0]["vcSqlCatalog"].ToString();
                                Body.sqlSource = dtPrinterInfo.Rows[0]["vcSqlSource"].ToString();
                                //调用具体的方法，这里是 HelloWorldAsync 方法
                                Task<setCRVToPDFResponse> responseTask = callClient.setCRVToPDFAsync(new setCRVToPDFRequest(Body));
                                //获取结果
                                setCRVToPDFResponse response = responseTask.Result;
                                if (response.Body.setCRVToPDFResult != "导出成功")
                                {
                                    if (reason == "纳入统计计算成功")
                                    {

                                        strErrorPartId = "生成PDF失败,纳入统计计算成功";
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                
                                strErrorPartId = "没有接口信息，请联系管理员维护。";
                            }
                            #endregion
                        }

                    }
                }

                DataTable dt1 = new DataTable();
                dt1.Columns.Add("vcName", Type.GetType("System.String"));
                DataRow dr = dt1.NewRow();
                dr["vcName"] = strErrorPartId;
                dt1.Rows.Add(dr);
                List<Object> vcException = ComFunction.convertAllToResult(dt1);//
                Dictionary<string, object> res = new Dictionary<string, object>();
                res.Add("strException", vcException);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M07UE1002", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "纳入统计生成失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 纳入统计导出
        [HttpPost]
        [EnableCors("any")]
        public string searchApi_NaRu_export([FromBody] dynamic data)
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
                string resMsg = "";
                DataTable dt = FS0710_Logic.Search_NaRu_export();
                if (dt.Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "没有可导出数据！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }
                string[] head = { "供应商", "供应商名称", "GPS品番", "品名", "规格", "数量", "单位", "备注", "费用负担" };
                string[] fields = { "vcSupplieCode", "vcSupplieName", "vcPackGPSNo", "vcParstName", "vcFormat", "isjNum", "vcUnit", "Memo", "vcCostID" };

                string filepath = ComFunction.DataTableToExcel(head, fields, dt, _webHostEnvironment.ContentRootPath, loginInfo.UserId, FunctionID, ref resMsg);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M07UE1003", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 订单统计
        [HttpPost]
        [EnableCors("any")]
        public string searchApi_DingDan([FromBody] dynamic data)
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
            PackSpot = dataForm.PackSpot.ToObject<List<Object>>();
            //供应商
            List<Object> strSupplierCode = new List<object>();
            strSupplierCode = dataForm.SupplierCode.ToObject<List<Object>>();
            string dFrom = dataForm.dFrom;
            string dTo = dataForm.dTo;
            try
            {
                DataTable dt = FS0710_Logic.Search_DD(PackSpot, strSupplierCode, dFrom, dTo);
                //插入临时表
                string strErrorPartId = "";
                if (dt.Rows.Count == 0)
                {

                    strErrorPartId = "订单统计0条！";
                }
                else
                {
                    FS0710_Logic.Save_DD(dt, ref strErrorPartId);
                    if (strErrorPartId == "")
                    {
                        strErrorPartId = "订单统计计算成功！";
                    }
                }
                DataTable dt1 = new DataTable();
                dt1.Columns.Add("vcName", Type.GetType("System.String"));
                DataRow dr = dt1.NewRow();
                dr["vcName"] = strErrorPartId;
                dt1.Rows.Add(dr);
                List<Object> vcException = ComFunction.convertAllToResult(dt1);//
                Dictionary<string, object> res = new Dictionary<string, object>();
                res.Add("strException", vcException);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M07UE1004", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "计算失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 订单统计导出
        [HttpPost]
        [EnableCors("any")]
        public string searchApi_DingDan_export([FromBody] dynamic data)
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
                string resMsg = "";
                DataTable dt = FS0710_Logic.Search_DingDan_export();
                if (dt.Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "没有可导出数据！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }
                string[] head = { "供应商", "订单号", "GPS品番", "包材品番", "到货预定日", "到货预订数", "到货日", "到货数" };
                string[] fields = { "vcSupplieCode", "vcOrderNo", "vcPackGPSNo", "vcPackNo", "dNaRuYuDing", "iOrderNumber", "dNaRuShiJi", "iSJNumber" };

                string filepath = ComFunction.DataTableToExcel(head, fields, dt, _webHostEnvironment.ContentRootPath, loginInfo.UserId, FunctionID, ref resMsg);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M07UE1005", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "订单统计导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

    }
}
