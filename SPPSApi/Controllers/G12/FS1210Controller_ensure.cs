using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ServiceModel;

namespace SPPSApi.Controllers.G12
{
    [Route("api/FS1210_ensure/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1210Controller_ensure : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        FS1210_Logic logic = new FS1210_Logic();
        private readonly string FunctionID = "FS1210";

        public FS1210Controller_ensure(IWebHostEnvironment webHostEnvironment)
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
                Dictionary<string, Object> res = new Dictionary<string, Object>();
                FS1209_Logic logic_1 = new FS1209_Logic();
                string RolePorType = logic_1.getRoleTip(loginInfo.UserId);
                DataTable dt1 = logic_1.dllPorType(RolePorType.Split('*'));
                List<Object> dataList_PorType = ComFunction.convertAllToResult(dt1);
                res.Add("PorTypeSource", dataList_PorType);
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
            ApiResult apiResult = new ApiResult();
            dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
            string vcKbOrderId = dataForm.vcKbOrderId == null ? "" : dataForm.vcKbOrderId;
            string vcPlanPrintDate = dataForm.vcPlanPrintDate == null ? "" : dataForm.vcPlanPrintDate;
            string vcPlanProcDate = dataForm.vcPlanProcDate == null ? "" : dataForm.vcPlanProcDate;
            string vcPrintDate = dataForm.vcPrintDate == null ? "" : dataForm.vcPrintDate;
            string vcGC = dataForm.vcGC == null ? "" : dataForm.vcGC;
            string vcPlanPrintBZ = dataForm.vcPlanPrintBZ == null ? "" : dataForm.vcPlanPrintBZ;
            string vcPlanProcBZ = dataForm.vcPlanProcBZ == null ? "" : dataForm.vcPlanProcBZ;
            try
            {
                DataTable tb = logic.SearchRePrintKBQR(vcKbOrderId, vcGC, vcPlanPrintDate, vcPlanPrintBZ, vcPlanProcDate, vcPlanProcBZ, vcPrintDate);
                List<object> dataList = ComFunction.convertAllToResult(tb);
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

        #region 打印
        [HttpPost]
        [EnableCors("any")]
        public string printApi([FromBody] dynamic data)
        {
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            ApiResult apiResult = new ApiResult();
            dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
            JArray checkedInfo = dataForm._multipleSelection;
            List<Dictionary<string, object>> listInfoData = checkedInfo.ToObject<List<Dictionary<string, object>>>();
            if (listInfoData.Count == 0)
            {
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "最少选择一条数据！";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            string msg = "";
            DataTable dtPorType = new DataTable();
            try
            {
                string vcKbOrderId = dataForm.vcKbOrderId == null ? "" : dataForm.vcKbOrderId;
                string vcPlanPrintDate = dataForm.vcPlanPrintDate == null ? "" : dataForm.vcPlanPrintDate;
                string vcPlanProcDate = dataForm.vcPlanProcDate == null ? "" : dataForm.vcPlanProcDate;
                string vcPrintDate = dataForm.vcPrintDate == null ? "" : dataForm.vcPrintDate;
                string vcGC = dataForm.vcGC == null ? "" : dataForm.vcGC;
                string vcPlanPrintBZ = dataForm.vcPlanPrintBZ == null ? "" : dataForm.vcPlanPrintBZ;
                string vcPlanProcBZ = dataForm.vcPlanProcBZ == null ? "" : dataForm.vcPlanProcBZ;
                string strLoginId = loginInfo.UserId;
                string strPrinterName = logic.PrintMess(loginInfo.UserId);//获取打印机
                string tmplatePath = "\\Template\\FS160170.xlt";//看板投放确认单Excel模板

                for (int z = 0; z < listInfoData.Count; z++)
                {
                    DataTable exdttt = new DataTable();
                    DataTable exdthj = new DataTable();
                    vcGC = listInfoData[z]["vcGC"].ToString();
                    vcKbOrderId = listInfoData[z]["vcOrderNo"].ToString();
                    vcPlanPrintDate = listInfoData[z]["vcPlanPrintDate"].ToString();
                    vcPlanPrintBZ = listInfoData[z]["vcPlanPrintBZ"].ToString();
                    if (vcPlanPrintBZ == "白值") { vcPlanPrintBZ = "0"; }
                    if (vcPlanPrintBZ == "夜值") { vcPlanPrintBZ = "1"; }
                    vcPlanProcDate = listInfoData[z]["vcPlanProcDate"].ToString();
                    vcPlanProcBZ = listInfoData[z]["vcPlanProcBZ"].ToString();
                    if (vcPlanProcBZ == "白值") { vcPlanProcBZ = "0"; }
                    if (vcPlanProcBZ == "夜值") { vcPlanProcBZ = "1"; }
                    vcPrintDate = listInfoData[z]["vcPrintDate"].ToString();

                    DataTable exdt = new DataTable();
                    DataTable dtHis = new DataTable();
                    exdt = logic.CreatDataTable();//创建ExcelDataTable
                    dtHis = logic.CreatDataTableHis();//创建连番DataTable

                    //数据库取出Excel的数据进行打印
                    DataSet ds = logic.aPrintExcel(vcGC, vcKbOrderId, vcPlanPrintDate, vcPlanPrintBZ == "0" ? "白" : "夜", vcPlanProcDate, vcPlanProcBZ == "0" ? "白" : "夜");

                    exdttt = ds.Tables[0];
                    exdthj = ds.Tables[1];
                    for (int p = 0; p < exdttt.Rows.Count; p++)
                    {
                        exdttt.Rows[p]["no"] = p + 1;
                    }
                    int dsRowsCount = exdttt.Rows.Count;
                    int dsRow = dsRowsCount / 43;
                    int dsrows = dsRowsCount % 43;
                    //总页数
                    int pagetotle = 0;
                    //页数.
                    int pageno = 0;
                    if (dsRow != 0)
                    {
                        pagetotle = ((dsrows + exdthj.Rows.Count + 3) / 43) == 0 ? (dsRow + 1) : (dsRow + 1 + (((exdthj.Rows.Count + 3) / 43) == 0 ? 1 : ((exdthj.Rows.Count + 3) / 43) + 1));
                    }
                    else
                    {
                        pagetotle = ((dsRowsCount + exdthj.Rows.Count + 3) / 43) == 0 ? 1 : (1 + (((exdthj.Rows.Count + 3) / 43) == 0 ? 1 : ((exdthj.Rows.Count + 3) / 43) + 1));
                    }

                    if (dsRow > 0)
                    {
                        DataTable inTable = exdttt.Clone();

                        for (int i = 0; i < exdttt.Rows.Count; i++)
                        {
                            DataRow dr = exdttt.Rows[i];
                            DataRow add = inTable.NewRow();
                            add.ItemArray = dr.ItemArray;
                            inTable.Rows.Add(add);
                            if (inTable.Rows.Count >= 43 || exdttt.Rows.Count - 1 == i)
                            {
                                pageno = pageno + 1;
                                string pageB = "0";
                                if (inTable.Rows.Count < 43)
                                {
                                    pageB = inTable.Rows.Count + exdthj.Rows.Count + 3 <= 43 ? "1" : "2";
                                }
                                //打印传值有：订单号、生产部署、计划打印日期 printIme、计划打印班值 printDay 、计划生产日期 、计划生产班值、页码
                                //exprint.PrintTemplateFromDataTable1(inTable, exdthj, tmplatePath, vcKbOrderId, vcGC, strLoginId, vcPlanPrintDate, vcPlanPrintBZ == "0" ? "白值" : "夜值", vcPlanProcDate, vcPlanProcBZ == "0" ? "白值" : "夜值", strPrinterName, Convert.ToString(pagetotle), Convert.ToString(pageno), pageB, vcPrintDate);

                                #region 创建打印临时表并打印 WebService
                                FS1209_Logic lg = new FS1209_Logic();
                                string exdthj_tmp = string.Empty;
                                string inTable_tmp = string.Empty;
                                string exdthj_msg = lg.CreateTempTable(exdthj, "FS1209_Excel_", out exdthj_tmp);//创建打印临时表  
                                string inTable_msg = lg.CreateTempTable(exdttt, "FS1209_Excel_", out inTable_tmp);//创建打印临时表                 
                                if (inTable_msg.Length == 0)
                                {
                                    try
                                    {
                                        BasicHttpBinding binding = new BasicHttpBinding();
                                        binding.CloseTimeout = TimeSpan.MaxValue;
                                        binding.OpenTimeout = TimeSpan.MaxValue;
                                        binding.ReceiveTimeout = TimeSpan.MaxValue;
                                        binding.SendTimeout = TimeSpan.MaxValue;
                                        SqlConnectionStringBuilder cn = new SqlConnectionStringBuilder(ComConnectionHelper.GetConnectionString());
                                        EndpointAddress address = new EndpointAddress("http://" + cn.DataSource + ":25012/PrintTable.asmx"); 
                                        PrintCR.PrintTableSoapClient client = new PrintCR.PrintTableSoapClient(binding, address);
                                        msg = client.PrintExcel_Confirmation_PrintDate(inTable_tmp, exdthj_tmp, tmplatePath, vcKbOrderId, vcGC, strLoginId, vcPlanPrintDate, vcPlanPrintBZ == "0" ? "白值" : "夜值", vcPlanProcDate, vcPlanProcBZ == "0" ? "白值" : "夜值", strPrinterName, Convert.ToString(pagetotle), Convert.ToString(pageno), pageB, vcPrintDate);

                                    }
                                    catch (Exception ex)
                                    {
                                        msg = "打印看板确认单失败！";
                                        throw ex;
                                    }
                                    finally
                                    {
                                        lg.DropTempTable(inTable_tmp);//删除打印临时表
                                        lg.DropTempTable(exdthj_tmp);//删除打印临时表
                                    }
                                }
                                #endregion


                                inTable = exdt.Clone();
                            }
                        }
                    }
                    else
                    {
                        string pageB = "0";
                        pageno = pageno + 1;
                        pageB = exdttt.Rows.Count + exdthj.Rows.Count + 3 <= 43 ? "1" : "2";

                        //打印传值有：订单号、生产部署、计划打印日期 printIme、计划打印班值 printDay 、计划生产日期 、计划生产班值、页码
                        //exprint.PrintTemplateFromDataTable1(exdttt, exdthj, tmplatePath, vcKbOrderId, vcGC, strLoginId, vcPlanPrintDate, vcPlanPrintBZ == "0" ? "白值" : "夜值", vcPlanProcDate, vcPlanProcBZ == "0" ? "白值" : "夜值", strPrinterName, Convert.ToString(pagetotle), Convert.ToString(pageno), pageB, vcPrintDate);
                        #region 创建打印临时表并打印 WebService
                        FS1209_Logic lg = new FS1209_Logic();
                        string exdttt_tmp = string.Empty;
                        string exdthj_tmp = string.Empty;
                        string exdttt_msg = lg.CreateTempTable(exdttt, "FS1209_Excel_", out exdttt_tmp);//创建打印临时表
                        string exdthj_msg = lg.CreateTempTable(exdthj, "FS1209_Excel_", out exdthj_tmp);//创建打印临时表                
                        if (exdthj_msg.Length == 0)
                        {
                            try
                            {
                                BasicHttpBinding binding = new BasicHttpBinding();
                                binding.CloseTimeout = TimeSpan.MaxValue;
                                binding.OpenTimeout = TimeSpan.MaxValue;
                                binding.ReceiveTimeout = TimeSpan.MaxValue;
                                binding.SendTimeout = TimeSpan.MaxValue;
                                SqlConnectionStringBuilder cn = new SqlConnectionStringBuilder(ComConnectionHelper.GetConnectionString());
                                EndpointAddress address = new EndpointAddress("http://" + cn.DataSource + ":25012/PrintTable.asmx");
                                PrintCR.PrintTableSoapClient client = new PrintCR.PrintTableSoapClient(binding, address);
                                exdthj_msg = client.PrintExcel_Confirmation_PrintDate(exdttt_tmp, exdthj_tmp, tmplatePath, vcKbOrderId, vcGC, strLoginId, vcPlanPrintDate, vcPlanPrintBZ == "0" ? "白值" : "夜值", vcPlanProcDate, vcPlanProcBZ == "0" ? "白值" : "夜值", strPrinterName, Convert.ToString(pagetotle), Convert.ToString(pageno), pageB, vcPrintDate);
                            }
                            catch (Exception ex)
                            {
                                msg = "打印看板确认单失败！";
                                throw ex;
                            }
                            finally
                            {
                                lg.DropTempTable(exdttt_tmp);//删除打印临时表
                                lg.DropTempTable(exdthj_tmp);//删除打印临时表
                            }
                        }
                        lg.DropTempTable(exdttt_tmp);//删除打印临时表
                        lg.DropTempTable(exdthj_tmp);//删除打印临时表
                        #endregion
                    }
                }
                if (msg == "")
                {
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.data = "打印成功";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = msg;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
            }
            catch (Exception ex)
            {
                //if (dtPorType != null)
                //{
                //    for (int i = 0; i < dtPorType.Rows.Count; i++)
                //    {
                //        ogic_09.DeleteprinterCREX(dtPorType.Rows[i]["vcPorType"].ToString(), dtPorType.Rows[i]["vcorderno"].ToString(), dtPorType.Rows[i]["vcComDate01"].ToString(), dtPorType.Rows[i]["vcBanZhi01"].ToString());
                //    }
                //}
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = msg;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}
