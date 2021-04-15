using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
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
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace SPPSApi.Controllers.G06
{
    [Route("api/FS0629/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0629Controller : BaseController
    {
        FS0629_Logic fs0629_Logic = new FS0629_Logic();
        private readonly string FunctionID = "FS0629";

        private readonly IWebHostEnvironment _webHostEnvironment;


        public FS0629Controller(IWebHostEnvironment webHostEnvironment)
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
                
                List<Object> dataList_C000 = ComFunction.convertAllToResult(ComFunction.getTCode("C000"));//发注工场
                List<Object> dataList_C018 = ComFunction.convertAllToResult(ComFunction.getTCode("C018"));//收货方

                res.Add("C000", dataList_C000);
                res.Add("C018", dataList_C018);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2901", ex, loginInfo.UserId);
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
            string vcTargetMonth = dataForm.vcTargetMonth == null ? "" : dataForm.vcTargetMonth;
            string vcConsignee = dataForm.vcConsignee == null ? "" : dataForm.vcConsignee;
            string vcInjectionFactory = dataForm.vcInjectionFactory == null ? "" : dataForm.vcInjectionFactory;
            string vcLastTargetMonth = string.Empty;
            if (vcTargetMonth.Length == 0)
            {
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "请输入对象年月！";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            if (vcTargetMonth.Length == 0)
            {
                vcTargetMonth = DateTime.Now.ToString("yyyyMM");
                vcLastTargetMonth = DateTime.Now.AddMonths(-1).ToString("yyyyMM");
            }
            else
            {
                vcLastTargetMonth = Convert.ToDateTime(vcTargetMonth).AddMonths(-1).ToString("yyyyMM");
                vcTargetMonth = vcTargetMonth.Replace("/","");
            }
            try
            {
                DataSet ds = fs0629_Logic.Search(vcConsignee, vcInjectionFactory, vcTargetMonth, vcLastTargetMonth);
                DataTable dt = ds.Tables[0];
                DataTable dtPlantSum = ds.Tables[1];
                DataTable dtSum = ds.Tables[2];
                if (dt.Rows.Count==0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "检索的对象年月没有数据!";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                

                string[] columnConsigneeArray = { "收货方" };
                DataView dtConsigneeSelectView = dt.DefaultView;
                DataTable dtConsigneeSelect = dtConsigneeSelectView.ToTable(true, columnConsigneeArray);//去重后的dt 
                DataTable dtNew = dt.Clone();//复制表结构 页面显示的 
                for (int m=0;m<dtConsigneeSelect.Rows.Count;m++)
                {
                    string strConsignee = dtConsigneeSelect.Rows[m]["收货方"].ToString();
                    DataRow[] drArray = dt.Select("收货方='" + strConsignee + "' ");
                    DataTable dt1 = drArray[0].Table.Clone();
                    foreach (DataRow dr in drArray)
                    {
                        dt1.ImportRow(dr);
                    }

                    string[] columnArray = { "收货方", "工场" };
                    DataView dtSelectView = dt1.DefaultView;
                    DataTable dtSelect = dtSelectView.ToTable(true, columnArray);//去重后的dt1 
                    
                    for (int i = 0; i < dtSelect.Rows.Count; i++)
                    {
                        string strConsigneeChild = dtSelect.Rows[i]["收货方"].ToString();
                        string strPlantChild = dtSelect.Rows[i]["工场"].ToString();
                        DataRow[] drArrayChild = dt.Select("收货方='" + strConsigneeChild + "' and 工场='" + strPlantChild + "' ");
                        string msg = string.Empty;
                        foreach (DataRow dr in drArrayChild)
                        {
                            dtNew.ImportRow(dr);
                        }
                        DataRow[] drArrayPlantSum = dtPlantSum.Select("收货方='" + strConsigneeChild + "' and 工场='" + strPlantChild + "' ");
                        foreach (DataRow dr in drArrayPlantSum)
                        {
                            dtNew.ImportRow(dr);
                        }
                    }
                    DataRow[] drArraySum = dtSum.Select("收货方='" + strConsignee + "' ");
                    foreach (DataRow dr in drArraySum)
                    {
                        dr["收货方"] = "合计";
                        dtNew.ImportRow(dr);
                    }
                }
                for (int m = 0; m < dtNew.Rows.Count; m++)
                {
                    string strYuDuDD = dtNew.Rows[m]["月度订单"].ToString() == "" ? "0" : dtNew.Rows[m]["月度订单"].ToString();
                    string strJinJiDD = dtNew.Rows[m]["紧急订单"].ToString() == "" ? "0" : dtNew.Rows[m]["紧急订单"].ToString();
                    string strYuDuDDNRSJ = dtNew.Rows[m]["月度订单纳入实绩"].ToString() == "" ? "0" : dtNew.Rows[m]["月度订单纳入实绩"].ToString();
                    string strJinJiDDNRSJ = dtNew.Rows[m]["紧急订单纳入实绩"].ToString() == "" ? "0" : dtNew.Rows[m]["紧急订单纳入实绩"].ToString();
                    string strYuDuDDCHSJ = dtNew.Rows[m]["月度订单出荷实绩"].ToString() == "" ? "0" : dtNew.Rows[m]["月度订单出荷实绩"].ToString();
                    string strJinJiDDCHSJ = dtNew.Rows[m]["紧急订单出荷实绩"].ToString() == "" ? "0" : dtNew.Rows[m]["紧急订单出荷实绩"].ToString();

                    string strLastYuDuDD = dtNew.Rows[m]["lastYuDuOrder"].ToString() == "" ? "0" : dtNew.Rows[m]["lastYuDuOrder"].ToString();
                    string strLastJinJiDD = dtNew.Rows[m]["lastJinJiOrder"].ToString() == "" ? "0" : dtNew.Rows[m]["lastJinJiOrder"].ToString();
                    string strLastYuDuDDNRSJ = dtNew.Rows[m]["lastYuDuNRSJ"].ToString() == "" ? "0" : dtNew.Rows[m]["lastYuDuNRSJ"].ToString();
                    string strLastJinJiDDNRSJ = dtNew.Rows[m]["lastJinJiNRSJ"].ToString() == "" ? "0" : dtNew.Rows[m]["lastJinJiNRSJ"].ToString();
                    string strLastYuDuDDCHSJ = dtNew.Rows[m]["lastYuDuCHSJ"].ToString() == "" ? "0" : dtNew.Rows[m]["lastYuDuCHSJ"].ToString();
                    string strLastJinJiDDCHSJ = dtNew.Rows[m]["lastJinJiCHSJ"].ToString() == "" ? "0" : dtNew.Rows[m]["lastJinJiCHSJ"].ToString();

                    if (Convert.ToDecimal(strYuDuDD) + Convert.ToDecimal(strJinJiDD) ==0)
                    {
                        dtNew.Rows[m]["纳入率(B/A)"] = "0.00%";
                        dtNew.Rows[m]["出荷率(D/A)"] = "0.00%";
                    }
                    else
                    {
                        decimal inputNR = ((Convert.ToDecimal(strYuDuDDNRSJ)+ Convert.ToDecimal(strJinJiDDNRSJ)) / (Convert.ToDecimal(strYuDuDD) + Convert.ToDecimal(strJinJiDD)))*100;
                        dtNew.Rows[m]["纳入率(B/A)"] = inputNR==0? "0.00%" : inputNR.RoundFirstSignificantDigit().ToString()+"%";
                        decimal inputCH = ((Convert.ToDecimal(strYuDuDDCHSJ) + Convert.ToDecimal(strJinJiDDCHSJ)) / (Convert.ToDecimal(strYuDuDD) + Convert.ToDecimal(strJinJiDD))) * 100;
                        dtNew.Rows[m]["出荷率(D/A)"] = inputCH == 0 ? "0.00%" : inputCH.RoundFirstSignificantDigit().ToString() + "%";
                    }
                    if (Convert.ToDecimal(strLastYuDuDD) + Convert.ToDecimal(strLastJinJiDD) == 0)
                    {
                        dtNew.Rows[m]["前月纳入率"] = "0.00%";
                        dtNew.Rows[m]["前月出荷率"] = "0.00%";
                    }
                    else
                    {
                        decimal inputLastNR = ((Convert.ToDecimal(strLastYuDuDDNRSJ) + Convert.ToDecimal(strLastJinJiDDNRSJ)) / (Convert.ToDecimal(strLastYuDuDD) + Convert.ToDecimal(strLastJinJiDD))) * 100;
                        dtNew.Rows[m]["前月纳入率"] = inputLastNR == 0 ? "0.00%" : inputLastNR.RoundFirstSignificantDigit().ToString() + "%";
                        decimal inputLastCH = ((Convert.ToDecimal(strLastYuDuDDCHSJ) + Convert.ToDecimal(strLastJinJiDDCHSJ)) / (Convert.ToDecimal(strLastYuDuDD) + Convert.ToDecimal(strLastJinJiDD))) * 100;
                        dtNew.Rows[m]["前月出荷率"] = inputLastCH == 0 ? "0.00%" : inputLastCH.RoundFirstSignificantDigit().ToString() + "%";
                    }
                }

                DtConverter dtConverter = new DtConverter();
                
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dtNew, dtConverter);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2902", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        

        #region 导出报表
        [HttpPost]
        [EnableCors("any")]
        public string createReportApi([FromBody] dynamic data)
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
            string vcTargetMonth = dataForm.vcTargetMonth == null ? "" : dataForm.vcTargetMonth;
            string vcConsignee = dataForm.vcConsignee == null ? "" : dataForm.vcConsignee;
            string vcInjectionFactory = dataForm.vcInjectionFactory == null ? "" : dataForm.vcInjectionFactory;
            string vcLastTargetMonth = string.Empty;
            if (vcTargetMonth.Length == 0)
            {
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "请输入对象年月！";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            if (vcTargetMonth.Length == 0)
            {
                vcTargetMonth = DateTime.Now.ToString("yyyyMM");
                vcLastTargetMonth = DateTime.Now.AddMonths(-1).ToString("yyyyMM");
            }
            else
            {
                vcLastTargetMonth = Convert.ToDateTime(vcTargetMonth).AddMonths(-1).ToString("yyyyMM");
                vcTargetMonth = vcTargetMonth.Replace("/", "");
            }
            try
            {
                DataTable dtXiData = fs0629_Logic.getDataChuRuKuByTargetMonth(vcTargetMonth);
                DataSet ds = fs0629_Logic.Search(vcConsignee, vcInjectionFactory, vcTargetMonth, vcLastTargetMonth);
                DataTable dt = ds.Tables[0];
                DataTable dtPlantSum = ds.Tables[1];
                DataTable dtSum = ds.Tables[2];
                DataTable dtSumReport = dtSum.Copy();
                if (dt.Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "检索的对象年月没有数据!";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                string[] columnConsigneeArray = { "收货方" };
                DataView dtConsigneeSelectView = dt.DefaultView;
                DataTable dtConsigneeSelect = dtConsigneeSelectView.ToTable(true, columnConsigneeArray);//去重后的dt 
                DataTable dtNew = dt.Clone();//复制表结构 页面显示的 
                for (int m = 0; m < dtConsigneeSelect.Rows.Count; m++)
                {
                    string strConsignee = dtConsigneeSelect.Rows[m]["收货方"].ToString();
                    DataRow[] drArray = dt.Select("收货方='" + strConsignee + "' ");
                    DataTable dt1 = drArray[0].Table.Clone();
                    foreach (DataRow dr in drArray)
                    {
                        dt1.ImportRow(dr);
                    }

                    string[] columnArray = { "收货方", "工场" };
                    DataView dtSelectView = dt1.DefaultView;
                    DataTable dtSelect = dtSelectView.ToTable(true, columnArray);//去重后的dt1 

                    for (int i = 0; i < dtSelect.Rows.Count; i++)
                    {
                        string strConsigneeChild = dtSelect.Rows[i]["收货方"].ToString();
                        string strPlantChild = dtSelect.Rows[i]["工场"].ToString();
                        DataRow[] drArrayChild = dt.Select("收货方='" + strConsigneeChild + "' and 工场='" + strPlantChild + "' ");
                        string msg = string.Empty;
                        foreach (DataRow dr in drArrayChild)
                        {
                            dtNew.ImportRow(dr);
                        }
                        DataRow[] drArrayPlantSum = dtPlantSum.Select("收货方='" + strConsigneeChild + "' and 工场='" + strPlantChild + "' ");
                        foreach (DataRow dr in drArrayPlantSum)
                        {
                            dtNew.ImportRow(dr);
                        }
                    }
                    DataRow[] drArraySum = dtSum.Select("收货方='" + strConsignee + "' ");
                    foreach (DataRow dr in drArraySum)
                    {
                        dr["收货方"] = "合计";
                        dtNew.ImportRow(dr);
                    }
                }
                for (int m = 0; m < dtNew.Rows.Count; m++)
                {
                    string strYuDuDD = dtNew.Rows[m]["月度订单"].ToString() == "" ? "0" : dtNew.Rows[m]["月度订单"].ToString();
                    string strJinJiDD = dtNew.Rows[m]["紧急订单"].ToString() == "" ? "0" : dtNew.Rows[m]["紧急订单"].ToString();
                    string strYuDuDDNRSJ = dtNew.Rows[m]["月度订单纳入实绩"].ToString() == "" ? "0" : dtNew.Rows[m]["月度订单纳入实绩"].ToString();
                    string strJinJiDDNRSJ = dtNew.Rows[m]["紧急订单纳入实绩"].ToString() == "" ? "0" : dtNew.Rows[m]["紧急订单纳入实绩"].ToString();
                    string strYuDuDDCHSJ = dtNew.Rows[m]["月度订单出荷实绩"].ToString() == "" ? "0" : dtNew.Rows[m]["月度订单出荷实绩"].ToString();
                    string strJinJiDDCHSJ = dtNew.Rows[m]["紧急订单出荷实绩"].ToString() == "" ? "0" : dtNew.Rows[m]["紧急订单出荷实绩"].ToString();

                    string strLastYuDuDD = dtNew.Rows[m]["lastYuDuOrder"].ToString() == "" ? "0" : dtNew.Rows[m]["lastYuDuOrder"].ToString();
                    string strLastJinJiDD = dtNew.Rows[m]["lastJinJiOrder"].ToString() == "" ? "0" : dtNew.Rows[m]["lastJinJiOrder"].ToString();
                    string strLastYuDuDDNRSJ = dtNew.Rows[m]["lastYuDuNRSJ"].ToString() == "" ? "0" : dtNew.Rows[m]["lastYuDuNRSJ"].ToString();
                    string strLastJinJiDDNRSJ = dtNew.Rows[m]["lastJinJiNRSJ"].ToString() == "" ? "0" : dtNew.Rows[m]["lastJinJiNRSJ"].ToString();
                    string strLastYuDuDDCHSJ = dtNew.Rows[m]["lastYuDuCHSJ"].ToString() == "" ? "0" : dtNew.Rows[m]["lastYuDuCHSJ"].ToString();
                    string strLastJinJiDDCHSJ = dtNew.Rows[m]["lastJinJiCHSJ"].ToString() == "" ? "0" : dtNew.Rows[m]["lastJinJiCHSJ"].ToString();

                    if (Convert.ToDecimal(strYuDuDD) + Convert.ToDecimal(strJinJiDD) == 0)
                    {
                        dtNew.Rows[m]["纳入率(B/A)"] = "0.00%";
                        dtNew.Rows[m]["出荷率(D/A)"] = "0.00%";
                    }
                    else
                    {
                        decimal inputNR = ((Convert.ToDecimal(strYuDuDDNRSJ) + Convert.ToDecimal(strJinJiDDNRSJ)) / (Convert.ToDecimal(strYuDuDD) + Convert.ToDecimal(strJinJiDD))) * 100;
                        dtNew.Rows[m]["纳入率(B/A)"] = inputNR == 0 ? "0.00%" : inputNR.RoundFirstSignificantDigit().ToString() + "%";
                        decimal inputCH = ((Convert.ToDecimal(strYuDuDDCHSJ) + Convert.ToDecimal(strJinJiDDCHSJ)) / (Convert.ToDecimal(strYuDuDD) + Convert.ToDecimal(strJinJiDD))) * 100;
                        dtNew.Rows[m]["出荷率(D/A)"] = inputCH == 0 ? "0.00%" : inputCH.RoundFirstSignificantDigit().ToString() + "%";
                    }
                    if (Convert.ToDecimal(strLastYuDuDD) + Convert.ToDecimal(strLastJinJiDD) == 0)
                    {
                        dtNew.Rows[m]["前月纳入率"] = "0.00%";
                        dtNew.Rows[m]["前月出荷率"] = "0.00%";
                    }
                    else
                    {
                        decimal inputLastNR = ((Convert.ToDecimal(strLastYuDuDDNRSJ) + Convert.ToDecimal(strLastJinJiDDNRSJ)) / (Convert.ToDecimal(strLastYuDuDD) + Convert.ToDecimal(strLastJinJiDD))) * 100; 
                        dtNew.Rows[m]["前月纳入率"] = inputLastNR == 0 ? "0.00%" : inputLastNR.RoundFirstSignificantDigit().ToString() + "%";
                        decimal inputLastCH = ((Convert.ToDecimal(strLastYuDuDDCHSJ) + Convert.ToDecimal(strLastJinJiDDCHSJ)) / (Convert.ToDecimal(strLastYuDuDD) + Convert.ToDecimal(strLastJinJiDD))) * 100;
                        dtNew.Rows[m]["前月出荷率"] = inputLastCH == 0 ? "0.00%" : inputLastCH.RoundFirstSignificantDigit().ToString() + "%";
                    }
                }

                for (int m = 0; m < dtSumReport.Rows.Count; m++)
                {
                    string strYuDuDD = dtSumReport.Rows[m]["月度订单"].ToString() == "" ? "0" : dtSumReport.Rows[m]["月度订单"].ToString();
                    string strJinJiDD = dtSumReport.Rows[m]["紧急订单"].ToString() == "" ? "0" : dtSumReport.Rows[m]["紧急订单"].ToString();
                    string strYuDuDDNRSJ = dtSumReport.Rows[m]["月度订单纳入实绩"].ToString() == "" ? "0" : dtSumReport.Rows[m]["月度订单纳入实绩"].ToString();
                    string strJinJiDDNRSJ = dtSumReport.Rows[m]["紧急订单纳入实绩"].ToString() == "" ? "0" : dtSumReport.Rows[m]["紧急订单纳入实绩"].ToString();
                    string strYuDuDDCHSJ = dtSumReport.Rows[m]["月度订单出荷实绩"].ToString() == "" ? "0" : dtSumReport.Rows[m]["月度订单出荷实绩"].ToString();
                    string strJinJiDDCHSJ = dtSumReport.Rows[m]["紧急订单出荷实绩"].ToString() == "" ? "0" : dtSumReport.Rows[m]["紧急订单出荷实绩"].ToString();

                    string strLastYuDuDD = dtSumReport.Rows[m]["lastYuDuOrder"].ToString() == "" ? "0" : dtSumReport.Rows[m]["lastYuDuOrder"].ToString();
                    string strLastJinJiDD = dtSumReport.Rows[m]["lastJinJiOrder"].ToString() == "" ? "0" : dtSumReport.Rows[m]["lastJinJiOrder"].ToString();
                    string strLastYuDuDDNRSJ = dtSumReport.Rows[m]["lastYuDuNRSJ"].ToString() == "" ? "0" : dtSumReport.Rows[m]["lastYuDuNRSJ"].ToString();
                    string strLastJinJiDDNRSJ = dtSumReport.Rows[m]["lastJinJiNRSJ"].ToString() == "" ? "0" : dtSumReport.Rows[m]["lastJinJiNRSJ"].ToString();
                    string strLastYuDuDDCHSJ = dtSumReport.Rows[m]["lastYuDuCHSJ"].ToString() == "" ? "0" : dtSumReport.Rows[m]["lastYuDuCHSJ"].ToString();
                    string strLastJinJiDDCHSJ = dtSumReport.Rows[m]["lastJinJiCHSJ"].ToString() == "" ? "0" : dtSumReport.Rows[m]["lastJinJiCHSJ"].ToString();

                    if (Convert.ToDecimal(strYuDuDD) + Convert.ToDecimal(strJinJiDD) == 0)
                    {
                        dtSumReport.Rows[m]["纳入率(B/A)"] = "0.00%";
                        dtSumReport.Rows[m]["出荷率(D/A)"] = "0.00%";
                    }
                    else
                    {
                        decimal inputNR = ((Convert.ToDecimal(strYuDuDDNRSJ) + Convert.ToDecimal(strJinJiDDNRSJ)) / (Convert.ToDecimal(strYuDuDD) + Convert.ToDecimal(strJinJiDD))) * 100;
                        dtSumReport.Rows[m]["纳入率(B/A)"] = inputNR == 0 ? "0.00%" : inputNR.RoundFirstSignificantDigit().ToString() + "%";
                        decimal inputCH = ((Convert.ToDecimal(strYuDuDDCHSJ) + Convert.ToDecimal(strJinJiDDCHSJ)) / (Convert.ToDecimal(strYuDuDD) + Convert.ToDecimal(strJinJiDD))) * 100;
                        dtSumReport.Rows[m]["出荷率(D/A)"] = inputCH == 0 ? "0.00%" : inputCH.RoundFirstSignificantDigit().ToString() + "%";
                    }
                    if (Convert.ToDecimal(strLastYuDuDD) + Convert.ToDecimal(strLastJinJiDD) == 0)
                    {
                        dtSumReport.Rows[m]["前月纳入率"] = "0.00%";
                        dtSumReport.Rows[m]["前月出荷率"] = "0.00%";
                    }
                    else
                    {
                        decimal inputLastNR = ((Convert.ToDecimal(strLastYuDuDDNRSJ) + Convert.ToDecimal(strLastJinJiDDNRSJ)) / (Convert.ToDecimal(strLastYuDuDD) + Convert.ToDecimal(strLastJinJiDD))) * 100;
                        dtSumReport.Rows[m]["前月纳入率"] = inputLastNR == 0 ? "0.00%" : inputLastNR.RoundFirstSignificantDigit().ToString() + "%";
                        decimal inputLastCH = ((Convert.ToDecimal(strLastYuDuDDCHSJ) + Convert.ToDecimal(strLastJinJiDDCHSJ)) / (Convert.ToDecimal(strLastYuDuDD) + Convert.ToDecimal(strLastJinJiDD))) * 100;
                        dtSumReport.Rows[m]["前月出荷率"] = inputLastCH == 0 ? "0.00%" : inputLastCH.RoundFirstSignificantDigit().ToString() + "%";
                    }
                }

                DataSet dsQianPin = fs0629_Logic.GetQianPin(vcConsignee, vcInjectionFactory, vcTargetMonth);
                DataTable dtQianPin = dsQianPin.Tables[0];
                DataTable dtQianPinSum = dsQianPin.Tables[1];

                string[] columnConsigneeArrayQianPin = {"vcReceiver" };
                DataView dtConsigneeSelectViewQianPin = dtQianPin.DefaultView;
                DataTable dtConsigneeSelectQianPin = dtConsigneeSelectViewQianPin.ToTable(true, columnConsigneeArrayQianPin);//去重后的dt 
                DataTable dtNewQianPin = dtQianPin.Clone();//复制表结构 页面显示的 
                for (int m = 0; m < dtConsigneeSelectQianPin.Rows.Count; m++)
                {
                    string strConsigneeQianPin = dtConsigneeSelectQianPin.Rows[m]["vcReceiver"].ToString();
                    DataRow[] drArrayQianPin = dtQianPin.Select("vcReceiver='" + strConsigneeQianPin + "' ");
                    DataTable dt1QianPin = drArrayQianPin[0].Table.Clone();
                    foreach (DataRow dr in drArrayQianPin)
                    {
                        dt1QianPin.ImportRow(dr);
                    }

                    string[] columnArrayQianPin = { "vcReceiver", "vcOrderPlant" };
                    DataView dtSelectViewQianPin = dt1QianPin.DefaultView;
                    DataTable dtSelectQianPin = dtSelectViewQianPin.ToTable(true, columnArrayQianPin);//去重后的dt1 

                    for (int i = 0; i < dtSelectQianPin.Rows.Count; i++)
                    {
                        string strConsigneeChildQianPin = dtSelectQianPin.Rows[i]["vcReceiver"].ToString();
                        string strPlantChildQianPin = dtSelectQianPin.Rows[i]["vcOrderPlant"].ToString();
                        DataRow[] drArrayChildQianPin = dtQianPin.Select("vcReceiver='" + strConsigneeChildQianPin + "' and vcOrderPlant='" + strPlantChildQianPin + "' ");
                        string msg = string.Empty;
                        foreach (DataRow dr in drArrayChildQianPin)
                        {
                            dtNewQianPin.ImportRow(dr);
                        }
                        DataRow[] drArrayPlantSumQianPin = dtQianPinSum.Select("vcReceiver='" + strConsigneeChildQianPin + "' and vcOrderPlant='" + strPlantChildQianPin + "' ");
                        foreach (DataRow dr in drArrayPlantSumQianPin)
                        {
                            dtNewQianPin.ImportRow(dr);
                        }
                    }
                }


                #region 导出报表
                XSSFWorkbook hssfworkbook = new XSSFWorkbook();//用于创建xlsx
                string XltPath = "." + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Template" + Path.DirectorySeparatorChar + "FS0629_reportData.xlsx";
                using (FileStream fs = System.IO.File.OpenRead(XltPath))
                {
                    hssfworkbook = new XSSFWorkbook(fs);
                    fs.Close();
                }
               
                #region 设置字体样式
                ICellStyle style1 = hssfworkbook.CreateCellStyle();//声明style1对象，设置Excel表格的样式 标题
                ICellStyle style2 = hssfworkbook.CreateCellStyle();//nian月日 补给
                ICellStyle style3 = hssfworkbook.CreateCellStyle();// 11 号字体 1-左边
                ICellStyle style4 = hssfworkbook.CreateCellStyle();//11 -2
                ICellStyle style5 = hssfworkbook.CreateCellStyle();// 11 号字体 11个单元格
                ICellStyle style6 = hssfworkbook.CreateCellStyle();//11 号字体 16个单元格
                ICellStyle style7 = hssfworkbook.CreateCellStyle();//9号字体不加粗 以上
                ICellStyle style8 = hssfworkbook.CreateCellStyle();//12号字体
                ICellStyle style9 = hssfworkbook.CreateCellStyle();//11号加粗字体
                ICellStyle style10 = hssfworkbook.CreateCellStyle();//最后合计去掉样式

                ICellStyle style11 = hssfworkbook.CreateCellStyle();//绿色
                ICellStyle style12 = hssfworkbook.CreateCellStyle();//第一个表单最后一个格子
                ICellStyle style13 = hssfworkbook.CreateCellStyle();//第一个表单最后一个格子
                IFont font = hssfworkbook.CreateFont();
                font.Color = IndexedColors.Black.Index;
                font.IsBold = false; ;
                font.FontHeightInPoints = 18;
                //font.FontName = "宋体";
                style1.SetFont(font);
                style1.Alignment = HorizontalAlignment.Left;//两端自动对齐（自动换行）
                style1.VerticalAlignment = VerticalAlignment.Center;


                IFont font2 = hssfworkbook.CreateFont();
                font2.Color = IndexedColors.Black.Index;
                font2.IsBold = false; ;
                font2.FontHeightInPoints = 12;
                //font.FontName = "宋体";
                style2.SetFont(font2);
                style2.Alignment = HorizontalAlignment.Right;
                style2.VerticalAlignment = VerticalAlignment.Center;


                IFont font3 = hssfworkbook.CreateFont();
                font3.Color = IndexedColors.Black.Index;
                font3.IsBold = false; ;
                font3.FontHeightInPoints = 11;
                style3.SetFont(font3);
                style3.Alignment = HorizontalAlignment.Center;
                style3.VerticalAlignment = VerticalAlignment.Center;
                style3.BorderLeft = BorderStyle.Medium;
                style3.BorderBottom = BorderStyle.Thin;

                IFont font4 = hssfworkbook.CreateFont();
                font4.Color = IndexedColors.Black.Index;
                font4.IsBold = false; ;
                font4.FontHeightInPoints = 11;
                style4.SetFont(font4);
                style4.Alignment = HorizontalAlignment.Center;
                style4.VerticalAlignment = VerticalAlignment.Center;
                style4.BorderLeft = BorderStyle.Thin;
                style4.BorderBottom = BorderStyle.Thin;

                IFont font5 = hssfworkbook.CreateFont();
                font5.Color = IndexedColors.Black.Index;
                font5.IsBold = false; ;
                font5.FontHeightInPoints = 11;
                style5.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightTurquoise.Index;
                style5.FillPattern = FillPattern.SolidForeground;
                style5.SetFont(font5);
                style5.Alignment = HorizontalAlignment.Center;
                style5.VerticalAlignment = VerticalAlignment.Center;
                style5.BorderLeft = BorderStyle.Medium;
                style5.BorderRight = BorderStyle.Thin;
                style5.BorderBottom = BorderStyle.Thin;

                IFont font6 = hssfworkbook.CreateFont();
                font6.Color = IndexedColors.Black.Index;
                font6.IsBold = false; ;
                font6.FontHeightInPoints = 11;
                style6.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightTurquoise.Index;
                style6.FillPattern = FillPattern.SolidForeground;
                style6.SetFont(font6);
                style6.Alignment = HorizontalAlignment.Center;
                style6.VerticalAlignment = VerticalAlignment.Center;
                style6.BorderLeft = BorderStyle.Medium;
                style6.BorderRight = BorderStyle.Medium;
                style6.BorderBottom = BorderStyle.Thin;

                
                style7.BorderLeft = BorderStyle.Thin;
                style7.BorderRight = BorderStyle.Thin;
                style7.BorderBottom = BorderStyle.Thin;

                IFont font8 = hssfworkbook.CreateFont();
                font8.Color = IndexedColors.Black.Index;
                font8.IsBold = false; ;
                font8.FontHeightInPoints = 12;
                style8.SetFont(font8);
                style8.Alignment = HorizontalAlignment.Center;//两端自动对齐（自动换行）
                style8.VerticalAlignment = VerticalAlignment.Center;

                IFont font9 = hssfworkbook.CreateFont();
                font9.Color = IndexedColors.Black.Index;
                font9.IsBold = true; ;
                font9.FontHeightInPoints = 11;
                style9.SetFont(font9);
                style9.Alignment = HorizontalAlignment.Center;//两端自动对齐（自动换行）
                style9.VerticalAlignment = VerticalAlignment.Center;

                style10.BorderLeft = BorderStyle.None;
                style10.BorderBottom = BorderStyle.None;
                
                style11.SetFont(font9);
                style11.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Lime.Index;
                style11.FillPattern = FillPattern.SolidForeground;
                style11.Alignment = HorizontalAlignment.Center;//两端自动对齐（自动换行）
                style11.VerticalAlignment = VerticalAlignment.Center;
                style11.BorderLeft = BorderStyle.Thin;
                style11.BorderBottom = BorderStyle.Medium;

                style12.SetFont(font9);
                style12.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Lime.Index;
                style12.FillPattern = FillPattern.SolidForeground;
                style12.Alignment = HorizontalAlignment.Center;//两端自动对齐（自动换行）
                style12.VerticalAlignment = VerticalAlignment.Center;
                style12.BorderLeft = BorderStyle.Thin;
                style12.BorderRight = BorderStyle.Medium;
                style12.BorderBottom = BorderStyle.Medium;
                style13.BorderBottom = BorderStyle.Medium;

                #endregion
                #region 设置列的宽度
                //mysheetHSSF.SetColumnWidth(0, 17 * 256); //设置第1列的列宽为17个字符
                //mysheetHSSF.SetColumnWidth(1, 5 * 256); //设置第2列的列宽为31个字符
                //mysheetHSSF.SetColumnWidth(2, 10 * 256); //设置第3列的列宽为10个字符
                //mysheetHSSF.SetColumnWidth(3, 10 * 256); //设置第4列的列宽为10个字符
                //mysheetHSSF.SetColumnWidth(4, 10 * 256); //设置第5列的列宽为10个字符
                //mysheetHSSF.SetColumnWidth(5, 10 * 256); //设置第6列的列宽为10个字符
                //mysheetHSSF.SetColumnWidth(6, 10 * 256); //设置第7列的列宽为10个字符
                //mysheetHSSF.SetColumnWidth(7, 10 * 256); //设置第8列的列宽为10个字符
                //mysheetHSSF.SetColumnWidth(8, 10 * 256); //设置第9列的列宽为10个字符
                //mysheetHSSF.SetColumnWidth(9, 10 * 256); //设置第10列的列宽为10个字符
                //mysheetHSSF.SetColumnWidth(10, 10 * 256); //设置第11列的列宽为10个字符
                //mysheetHSSF.SetColumnWidth(11, 10 * 256); //设置第12列的列宽为10个字符
                //mysheetHSSF.SetColumnWidth(12, 10 * 256); //设置第13列的列宽为10个字符
                //mysheetHSSF.SetColumnWidth(13, 17 * 256); //设置第14列的列宽为17个字符
                #endregion

                #region  设置标题
                ISheet sheetOrder = hssfworkbook.GetSheet("data");
                sheetOrder.GetRow(0).GetCell(0).SetCellValue("【"+vcTargetMonth.Replace("/","").Substring(0,4)+"年"+ vcTargetMonth.Replace("/", "").Substring(4, 2) + "月】补给品纳入 & 出荷总结");//标题名称
                sheetOrder.GetRow(0).GetCell(0).CellStyle = style1;

                string strDate = DateTime.Now.ToString("yyyy年MM月dd日");
                sheetOrder.GetRow(0).GetCell(15).SetCellValue(strDate);//标题名称
                sheetOrder.GetRow(0).GetCell(15).CellStyle = style2;

                #endregion
                #region 创建行
                int startRowIndexFor2 = 7;
                if (dtNewQianPin.Rows.Count > 1)
                {
                    sheetOrder.ShiftRows(startRowIndexFor2, sheetOrder.LastRowNum, dtNewQianPin.Rows.Count - 1, true, false);
                    for (int k = startRowIndexFor2; k < startRowIndexFor2 + dtNewQianPin.Rows.Count - 1; k++)
                    {
                        var rowInsert = sheetOrder.CreateRow(k);
                        rowInsert.Height = 34 * 20;

                        for (int col = 0; col < 17; col++)
                        {
                            var cellInsert = rowInsert.CreateCell(col);
                            if (col == 0)
                            {
                                cellInsert.CellStyle = style3;
                            }
                            else if (col == 16)
                            {
                                cellInsert.CellStyle = style7;
                            }
                            else
                            {
                                cellInsert.CellStyle = style4;
                            }
                        }
                    }
                }
                int startRowIndex = 4;
                if (dtNew.Rows.Count > 1)
                {
                    sheetOrder.ShiftRows(startRowIndex, sheetOrder.LastRowNum, dtNew.Rows.Count - 1, true, false);
                    for (int k = startRowIndex; k < startRowIndex + dtNew.Rows.Count - 1; k++)
                    {
                        var rowInsert = sheetOrder.CreateRow(k);
                        rowInsert.Height = 34 * 20;

                        for (int col = 0; col < 17; col++)
                        {
                            var cellInsert = rowInsert.CreateCell(col);
                            if (col == 0||col==7||col==10||col==15)
                            {
                                cellInsert.CellStyle = style3;
                            }
                            else if(col==11)
                            {
                                cellInsert.CellStyle = style5;
                            }
                            else if (col==16)
                            {
                                cellInsert.CellStyle = style6;
                            }
                            else 
                            {
                                cellInsert.CellStyle = style4;
                            }
                        }
                    }
                }
                #endregion

                #region 填充数据
                #region//纳入出荷 统计
                int startRowNum = 3;//开始填写数据
                int endRowNum = 0;
                int nextRowNum = startRowNum;
                for (int m = 0; m < dtConsigneeSelect.Rows.Count; m++)
                {
                    string strConsignee = dtConsigneeSelect.Rows[m]["收货方"].ToString();
                    DataRow[] drArray = dtNew.Select("收货方='" + strConsignee + "' ");
                    DataTable dt1 = drArray[0].Table.Clone();
                    foreach (DataRow dr in drArray)
                    {
                        dt1.ImportRow(dr);
                    }
                    //设置第二行 统计项目  合并单元格【CellRangeAddress(开始行,结束行,开始列,结束列)】
                    sheetOrder.AddMergedRegion(new CellRangeAddress(nextRowNum, nextRowNum+ dt1.Rows.Count-1, 0, 0)); //合并单元格第二行从第1列到第2列
                    sheetOrder.GetRow(nextRowNum).GetCell(0).SetCellValue(strConsignee);//收货方
                    //sheetOrder.GetRow(nextRowNum).GetCell(0).CellStyle = style8;

                    string[] columnArray = { "收货方", "工场" };
                    DataView dtSelectView = dt1.DefaultView;
                    DataTable dtSelect = dtSelectView.ToTable(true, columnArray);//去重后的dt1 

                    for (int i = 0; i < dtSelect.Rows.Count; i++)
                    {
                        string strConsigneeChild = dtSelect.Rows[i]["收货方"].ToString();
                        string strPlantChild = dtSelect.Rows[i]["工场"].ToString();
                        DataRow[] drArrayChild = dtNew.Select("收货方='" + strConsigneeChild + "' and 工场='" + strPlantChild + "' ");
                        DataTable dt4plant = drArrayChild[0].Table.Clone();
                        foreach (DataRow dr in drArrayChild)
                        {
                            dt4plant.ImportRow(dr);
                        }
                        //工场
                        sheetOrder.AddMergedRegion(new CellRangeAddress(nextRowNum, nextRowNum + drArrayChild.Length-1, 1, 1)); //合并单元格第二行从第1列到第2列
                        sheetOrder.GetRow(nextRowNum).GetCell(1).SetCellValue(strPlantChild);//工场
                        //sheetOrder.GetRow(nextRowNum).GetCell(1).CellStyle = style8;

                        DataRow[] drArrayChild4NeiZhi = dt4plant.Select("工程<>'外注' and 工程<>'合计' ");
                        if (drArrayChild4NeiZhi.Length>0)
                        {
                            if (drArrayChild4NeiZhi.Length > 1)
                            {
                                sheetOrder.AddMergedRegion(new CellRangeAddress(nextRowNum, nextRowNum + drArrayChild4NeiZhi.Length - 1, 2, 2)); //合并单元格第二行从第1列到第2列
                            }
                            sheetOrder.GetRow(nextRowNum).GetCell(2).SetCellValue("內制");//工场
                            //sheetOrder.GetRow(nextRowNum).GetCell(2).CellStyle = style8;
                            foreach (DataRow dr in drArrayChild4NeiZhi)
                            {
                                //行赋值
                                sheetOrder.GetRow(nextRowNum).GetCell(3).SetCellValue(dr["工程"].ToString());//工场
                                //sheetOrder.GetRow(nextRowNum).GetCell(3).CellStyle = style8;
                                sheetOrder.GetRow(nextRowNum).GetCell(4).SetCellValue(Convert.ToDouble(dr["月度订单"].ToString()));
                                //sheetOrder.GetRow(nextRowNum).GetCell(4).CellStyle = style8;
                                sheetOrder.GetRow(nextRowNum).GetCell(5).SetCellValue(Convert.ToDouble(dr["紧急订单"].ToString()));
                                //sheetOrder.GetRow(nextRowNum).GetCell(5).CellStyle = style8;
                                sheetOrder.GetRow(nextRowNum).GetCell(6).SetCellValue(Convert.ToDouble(dr["应纳合计(A)"].ToString()));
                                //sheetOrder.GetRow(nextRowNum).GetCell(6).CellStyle = style8;
                                sheetOrder.GetRow(nextRowNum).GetCell(7).SetCellValue(Convert.ToDouble(dr["月度订单纳入实绩"].ToString()));
                                //sheetOrder.GetRow(nextRowNum).GetCell(7).CellStyle = style8;
                                sheetOrder.GetRow(nextRowNum).GetCell(8).SetCellValue(Convert.ToDouble(dr["紧急订单纳入实绩"].ToString()));
                                //sheetOrder.GetRow(nextRowNum).GetCell(8).CellStyle = style8;
                                sheetOrder.GetRow(nextRowNum).GetCell(9).SetCellValue(Convert.ToDouble(dr["纳入合计(B)"].ToString()));
                                //sheetOrder.GetRow(nextRowNum).GetCell(9).CellStyle = style8;
                                sheetOrder.GetRow(nextRowNum).GetCell(10).SetCellValue(dr["纳入率(B/A)"].ToString());
                                //sheetOrder.GetRow(nextRowNum).GetCell(10).CellStyle = style8;
                                sheetOrder.GetRow(nextRowNum).GetCell(11).SetCellValue(dr["前月纳入率"].ToString());
                                //sheetOrder.GetRow(nextRowNum).GetCell(11).CellStyle = style8;
                                sheetOrder.GetRow(nextRowNum).GetCell(12).SetCellValue(Convert.ToDouble(dr["月度订单出荷实绩"].ToString()));
                                //sheetOrder.GetRow(nextRowNum).GetCell(12).CellStyle = style8;
                                sheetOrder.GetRow(nextRowNum).GetCell(13).SetCellValue(Convert.ToDouble(dr["紧急订单出荷实绩"].ToString()));
                                //sheetOrder.GetRow(nextRowNum).GetCell(13).CellStyle = style8;
                                sheetOrder.GetRow(nextRowNum).GetCell(14).SetCellValue(Convert.ToDouble(dr["出荷合计(D)"].ToString()));
                                //sheetOrder.GetRow(nextRowNum).GetCell(14).CellStyle = style8;
                                sheetOrder.GetRow(nextRowNum).GetCell(15).SetCellValue(dr["出荷率(D/A)"].ToString());
                                //sheetOrder.GetRow(nextRowNum).GetCell(15).CellStyle = style8;
                                sheetOrder.GetRow(nextRowNum).GetCell(16).SetCellValue(dr["前月出荷率"].ToString());
                                //sheetOrder.GetRow(nextRowNum).GetCell(16).CellStyle = style8;
                                nextRowNum++;
                            }
                        }
                        //外注
                        DataRow[] drArrayChild4WaiZhu = dt4plant.Select("工程='外注'");
                        if (drArrayChild4WaiZhu.Length > 0)
                        {
                            sheetOrder.AddMergedRegion(new CellRangeAddress(nextRowNum, nextRowNum, 2, 3)); //合并单元格第二行从第1列到第2列
                            sheetOrder.GetRow(nextRowNum).GetCell(2).SetCellValue("外注");//工场
                            //sheetOrder.GetRow(nextRowNum).GetCell(2).CellStyle = style8;
                            foreach (DataRow dr in drArrayChild4WaiZhu)
                            {
                                //行赋值
                                sheetOrder.GetRow(nextRowNum).GetCell(4).SetCellValue(Convert.ToDouble(dr["月度订单"].ToString()));
                                //sheetOrder.GetRow(nextRowNum).GetCell(4).CellStyle = style8;
                                sheetOrder.GetRow(nextRowNum).GetCell(5).SetCellValue(Convert.ToDouble(dr["紧急订单"].ToString()));
                                //sheetOrder.GetRow(nextRowNum).GetCell(5).CellStyle = style8;
                                sheetOrder.GetRow(nextRowNum).GetCell(6).SetCellValue(Convert.ToDouble(dr["应纳合计(A)"].ToString()));
                                //sheetOrder.GetRow(nextRowNum).GetCell(6).CellStyle = style8;
                                sheetOrder.GetRow(nextRowNum).GetCell(7).SetCellValue(Convert.ToDouble(dr["月度订单纳入实绩"].ToString()));
                                //sheetOrder.GetRow(nextRowNum).GetCell(7).CellStyle = style8;
                                sheetOrder.GetRow(nextRowNum).GetCell(8).SetCellValue(Convert.ToDouble(dr["紧急订单纳入实绩"].ToString()));
                                //sheetOrder.GetRow(nextRowNum).GetCell(8).CellStyle = style8;
                                sheetOrder.GetRow(nextRowNum).GetCell(9).SetCellValue(Convert.ToDouble(dr["纳入合计(B)"].ToString()));
                                //sheetOrder.GetRow(nextRowNum).GetCell(9).CellStyle = style8;
                                sheetOrder.GetRow(nextRowNum).GetCell(10).SetCellValue(dr["纳入率(B/A)"].ToString());
                                //sheetOrder.GetRow(nextRowNum).GetCell(10).CellStyle = style8;
                                sheetOrder.GetRow(nextRowNum).GetCell(11).SetCellValue(dr["前月纳入率"].ToString());
                                //sheetOrder.GetRow(nextRowNum).GetCell(11).CellStyle = style8;
                                sheetOrder.GetRow(nextRowNum).GetCell(12).SetCellValue(Convert.ToDouble(dr["月度订单出荷实绩"].ToString()));
                                //sheetOrder.GetRow(nextRowNum).GetCell(12).CellStyle = style8;
                                sheetOrder.GetRow(nextRowNum).GetCell(13).SetCellValue(Convert.ToDouble(dr["紧急订单出荷实绩"].ToString()));
                                //sheetOrder.GetRow(nextRowNum).GetCell(13).CellStyle = style8;
                                sheetOrder.GetRow(nextRowNum).GetCell(14).SetCellValue(Convert.ToDouble(dr["出荷合计(D)"].ToString()));
                                //sheetOrder.GetRow(nextRowNum).GetCell(14).CellStyle = style8;
                                sheetOrder.GetRow(nextRowNum).GetCell(15).SetCellValue(dr["出荷率(D/A)"].ToString());
                                //sheetOrder.GetRow(nextRowNum).GetCell(15).CellStyle = style8;
                                sheetOrder.GetRow(nextRowNum).GetCell(16).SetCellValue(dr["前月出荷率"].ToString());
                                //sheetOrder.GetRow(nextRowNum).GetCell(16).CellStyle = style8;
                                nextRowNum++;
                            }
                        }
                        //合计
                        DataRow[] drArrayChild4HeJi = dt4plant.Select("工程='合计'");
                        if (drArrayChild4HeJi.Length > 0)
                        {
                            sheetOrder.AddMergedRegion(new CellRangeAddress(nextRowNum, nextRowNum, 2, 3)); //合并单元格第二行从第1列到第2列
                            sheetOrder.GetRow(nextRowNum).GetCell(2).SetCellValue("合计");//工场
                            //sheetOrder.GetRow(nextRowNum).GetCell(2).CellStyle = style8;
                            foreach (DataRow dr in drArrayChild4HeJi)
                            {
                                //行赋值
                                sheetOrder.GetRow(nextRowNum).GetCell(4).SetCellValue(Convert.ToDouble(dr["月度订单"].ToString()));
                                //sheetOrder.GetRow(nextRowNum).GetCell(4).CellStyle = style9;
                                sheetOrder.GetRow(nextRowNum).GetCell(5).SetCellValue(Convert.ToDouble(dr["紧急订单"].ToString()));
                                //sheetOrder.GetRow(nextRowNum).GetCell(5).CellStyle = style9;
                                sheetOrder.GetRow(nextRowNum).GetCell(6).SetCellValue(Convert.ToDouble(dr["应纳合计(A)"].ToString()));
                                //sheetOrder.GetRow(nextRowNum).GetCell(6).CellStyle = style9;
                                sheetOrder.GetRow(nextRowNum).GetCell(7).SetCellValue(Convert.ToDouble(dr["月度订单纳入实绩"].ToString()));
                                //sheetOrder.GetRow(nextRowNum).GetCell(7).CellStyle = style9;
                                sheetOrder.GetRow(nextRowNum).GetCell(8).SetCellValue(Convert.ToDouble(dr["紧急订单纳入实绩"].ToString()));
                                //sheetOrder.GetRow(nextRowNum).GetCell(8).CellStyle = style9;
                                sheetOrder.GetRow(nextRowNum).GetCell(9).SetCellValue(Convert.ToDouble(dr["纳入合计(B)"].ToString()));
                                //sheetOrder.GetRow(nextRowNum).GetCell(9).CellStyle = style9;
                                sheetOrder.GetRow(nextRowNum).GetCell(10).SetCellValue(dr["纳入率(B/A)"].ToString());
                                //sheetOrder.GetRow(nextRowNum).GetCell(10).CellStyle = style9;
                                sheetOrder.GetRow(nextRowNum).GetCell(11).SetCellValue(dr["前月纳入率"].ToString());
                                //sheetOrder.GetRow(nextRowNum).GetCell(11).CellStyle = style9;
                                sheetOrder.GetRow(nextRowNum).GetCell(12).SetCellValue(Convert.ToDouble(dr["月度订单出荷实绩"].ToString()));
                                //sheetOrder.GetRow(nextRowNum).GetCell(12).CellStyle = style9;
                                sheetOrder.GetRow(nextRowNum).GetCell(13).SetCellValue(Convert.ToDouble(dr["紧急订单出荷实绩"].ToString()));
                                //sheetOrder.GetRow(nextRowNum).GetCell(13).CellStyle = style9;
                                sheetOrder.GetRow(nextRowNum).GetCell(14).SetCellValue(Convert.ToDouble(dr["出荷合计(D)"].ToString()));
                                //sheetOrder.GetRow(nextRowNum).GetCell(14).CellStyle = style9;
                                sheetOrder.GetRow(nextRowNum).GetCell(15).SetCellValue(dr["出荷率(D/A)"].ToString());
                                //sheetOrder.GetRow(nextRowNum).GetCell(15).CellStyle = style9;
                                sheetOrder.GetRow(nextRowNum).GetCell(16).SetCellValue(dr["前月出荷率"].ToString());
                                //sheetOrder.GetRow(nextRowNum).GetCell(16).CellStyle = style9;
                                nextRowNum++;
                            }
                        }
                    }
                    DataRow[] drArraySum = dtSumReport.Select("收货方='" + strConsignee + "' ");
                    foreach (DataRow dr in drArraySum)
                    {
                        sheetOrder.GetRow(nextRowNum).GetCell(0).SetCellValue("");
                        sheetOrder.GetRow(nextRowNum).GetCell(0).CellStyle = style10;
                        sheetOrder.AddMergedRegion(new CellRangeAddress(nextRowNum, nextRowNum, 1, 3)); //合并单元格第二行从第1列到第2列
                        sheetOrder.GetRow(nextRowNum).GetCell(1).SetCellValue("合计");//工场
                        sheetOrder.GetRow(nextRowNum).GetCell(1).CellStyle = style11;
                        sheetOrder.GetRow(nextRowNum).GetCell(2).CellStyle = style13;
                        sheetOrder.GetRow(nextRowNum).GetCell(3).CellStyle = style13;
                        sheetOrder.GetRow(nextRowNum).GetCell(4).SetCellValue(Convert.ToDouble(dr["月度订单"].ToString()));
                        sheetOrder.GetRow(nextRowNum).GetCell(4).CellStyle = style11;
                        sheetOrder.GetRow(nextRowNum).GetCell(5).SetCellValue(Convert.ToDouble(dr["紧急订单"].ToString()));
                        sheetOrder.GetRow(nextRowNum).GetCell(5).CellStyle = style11;
                        sheetOrder.GetRow(nextRowNum).GetCell(6).SetCellValue(Convert.ToDouble(dr["应纳合计(A)"].ToString()));
                        sheetOrder.GetRow(nextRowNum).GetCell(6).CellStyle = style11;
                        sheetOrder.GetRow(nextRowNum).GetCell(7).SetCellValue(Convert.ToDouble(dr["月度订单纳入实绩"].ToString()));
                        sheetOrder.GetRow(nextRowNum).GetCell(7).CellStyle = style11;
                        sheetOrder.GetRow(nextRowNum).GetCell(8).SetCellValue(Convert.ToDouble(dr["紧急订单纳入实绩"].ToString()));
                        sheetOrder.GetRow(nextRowNum).GetCell(8).CellStyle = style11;
                        sheetOrder.GetRow(nextRowNum).GetCell(9).SetCellValue(Convert.ToDouble(dr["纳入合计(B)"].ToString()));
                        sheetOrder.GetRow(nextRowNum).GetCell(9).CellStyle = style11;
                        sheetOrder.GetRow(nextRowNum).GetCell(10).SetCellValue(dr["纳入率(B/A)"].ToString());
                        sheetOrder.GetRow(nextRowNum).GetCell(10).CellStyle = style11;
                        sheetOrder.GetRow(nextRowNum).GetCell(11).SetCellValue(dr["前月纳入率"].ToString());
                        sheetOrder.GetRow(nextRowNum).GetCell(11).CellStyle = style11;
                        sheetOrder.GetRow(nextRowNum).GetCell(12).SetCellValue(Convert.ToDouble(dr["月度订单出荷实绩"].ToString()));
                        sheetOrder.GetRow(nextRowNum).GetCell(12).CellStyle = style11;
                        sheetOrder.GetRow(nextRowNum).GetCell(13).SetCellValue(Convert.ToDouble(dr["紧急订单出荷实绩"].ToString()));
                        sheetOrder.GetRow(nextRowNum).GetCell(13).CellStyle = style11;
                        sheetOrder.GetRow(nextRowNum).GetCell(14).SetCellValue(Convert.ToDouble(dr["出荷合计(D)"].ToString()));
                        sheetOrder.GetRow(nextRowNum).GetCell(14).CellStyle = style11;
                        sheetOrder.GetRow(nextRowNum).GetCell(15).SetCellValue(dr["出荷率(D/A)"].ToString());
                        sheetOrder.GetRow(nextRowNum).GetCell(15).CellStyle = style11;
                        sheetOrder.GetRow(nextRowNum).GetCell(16).SetCellValue(dr["前月出荷率"].ToString());
                        sheetOrder.GetRow(nextRowNum).GetCell(16).CellStyle = style12;
                        nextRowNum++;
                    }
                }
                #endregion

                #region
                nextRowNum = nextRowNum + 3;
                for (int m = 0; m < dtConsigneeSelectQianPin.Rows.Count; m++)
                {
                    string strConsigneeQianPin = dtConsigneeSelectQianPin.Rows[m]["vcReceiver"].ToString();
                    DataRow[] drArrayQianPin = dtNewQianPin.Select("vcReceiver='" + strConsigneeQianPin + "' ");
                    DataTable dt1QianPin = drArrayQianPin[0].Table.Clone();
                    foreach (DataRow dr in drArrayQianPin)
                    {
                        dt1QianPin.ImportRow(dr);
                    }
                    //收货方
                    sheetOrder.AddMergedRegion(new CellRangeAddress(nextRowNum, nextRowNum + drArrayQianPin.Length-1, 0, 0)); //合并单元格第二行从第1列到第2列
                    sheetOrder.GetRow(nextRowNum).GetCell(0).SetCellValue(strConsigneeQianPin);//收货方
                    //sheetOrder.GetRow(nextRowNum).GetCell(0).CellStyle = style8;

                    string[] columnArrayQianPin = { "vcReceiver", "vcOrderPlant" };
                    DataView dtSelectViewQianPin = dt1QianPin.DefaultView;
                    DataTable dtSelectQianPin = dtSelectViewQianPin.ToTable(true, columnArrayQianPin);//去重后的dt1 

                    for (int i = 0; i < dtSelectQianPin.Rows.Count; i++)
                    {
                        string strConsigneeChildQianPin = dtSelectQianPin.Rows[i]["vcReceiver"].ToString();
                        string strPlantChildQianPin = dtSelectQianPin.Rows[i]["vcOrderPlant"].ToString();
                        DataRow[] drArrayChildQianPin = dtNewQianPin.Select("vcReceiver='" + strConsigneeChildQianPin + "' and vcOrderPlant='" + strPlantChildQianPin + "' ");

                        DataTable dtChildQianPin4Class = drArrayChildQianPin[0].Table.Clone();
                        //发注工场
                        sheetOrder.AddMergedRegion(new CellRangeAddress(nextRowNum, nextRowNum + drArrayChildQianPin.Length-2, 1, 2)); //合并单元格第二行从第1列到第2列
                        sheetOrder.GetRow(nextRowNum).GetCell(1).SetCellValue(strPlantChildQianPin);
                        //sheetOrder.GetRow(nextRowNum).GetCell(1).CellStyle = style8;


                        string msg = string.Empty;
                        foreach (DataRow dr in drArrayChildQianPin)
                        {
                            dtChildQianPin4Class.ImportRow(dr);
                        }
                        //纳入
                        DataRow[] drArrayChildQianPin4Class4Naru = dtChildQianPin4Class.Select("vcClassType='纳入' ");
                        if (drArrayChildQianPin4Class4Naru.Length>0)
                        {
                            if (drArrayChildQianPin4Class4Naru.Length > 1)
                            {
                                sheetOrder.AddMergedRegion(new CellRangeAddress(nextRowNum, nextRowNum + drArrayChildQianPin4Class4Naru.Length - 1, 3, 3)); //合并单元格第二行从第1列到第2列
                            }
                            
                            sheetOrder.GetRow(nextRowNum).GetCell(3).SetCellValue("纳入");
                            //sheetOrder.GetRow(nextRowNum).GetCell(1).CellStyle = style8;

                            foreach (DataRow dr in drArrayChildQianPin4Class4Naru)
                            {
                                sheetOrder.AddMergedRegion(new CellRangeAddress(nextRowNum, nextRowNum, 4, 5)); //合并单元格第二行从第1列到第2列
                                sheetOrder.GetRow(nextRowNum).GetCell(4).SetCellValue(dr["vcSupplierId"].ToString());
                                //sheetOrder.GetRow(nextRowNum).GetCell(4).CellStyle = style11;
                                sheetOrder.GetRow(nextRowNum).GetCell(6).SetCellValue(Convert.ToDouble(dr["partNum"].ToString()));
                                //sheetOrder.GetRow(nextRowNum).GetCell(6).CellStyle = style11;
                                sheetOrder.GetRow(nextRowNum).GetCell(7).SetCellValue(Convert.ToDouble(dr["qianPinSum"].ToString()));
                                //sheetOrder.GetRow(nextRowNum).GetCell(7).CellStyle = style11;
                                sheetOrder.AddMergedRegion(new CellRangeAddress(nextRowNum, nextRowNum, 8, 11));
                                sheetOrder.AddMergedRegion(new CellRangeAddress(nextRowNum, nextRowNum, 13, 15));
                                nextRowNum++;
                            }
                        }

                        DataRow[] drArrayChildQianPin4Class4chuhe = dtChildQianPin4Class.Select("vcClassType='出荷' ");
                        if (drArrayChildQianPin4Class4chuhe.Length > 0)
                        {
                            if (drArrayChildQianPin4Class4chuhe.Length > 1)
                            {
                                sheetOrder.AddMergedRegion(new CellRangeAddress(nextRowNum, nextRowNum + drArrayChildQianPin4Class4chuhe.Length - 1, 3, 3)); //合并单元格第二行从第1列到第2列
                            }
                            sheetOrder.GetRow(nextRowNum).GetCell(3).SetCellValue("出荷");
                            //sheetOrder.GetRow(nextRowNum).GetCell(1).CellStyle = style8;

                            foreach (DataRow dr in drArrayChildQianPin4Class4chuhe)
                            {
                                sheetOrder.AddMergedRegion(new CellRangeAddress(nextRowNum, nextRowNum, 4, 5)); //合并单元格第二行从第1列到第2列
                                sheetOrder.GetRow(nextRowNum).GetCell(4).SetCellValue(dr["vcSupplierId"].ToString());
                                //sheetOrder.GetRow(nextRowNum).GetCell(4).CellStyle = style11;
                                sheetOrder.GetRow(nextRowNum).GetCell(6).SetCellValue(Convert.ToDouble(dr["partNum"].ToString()));
                                //sheetOrder.GetRow(nextRowNum).GetCell(6).CellStyle = style11;
                                sheetOrder.GetRow(nextRowNum).GetCell(7).SetCellValue(Convert.ToDouble(dr["qianPinSum"].ToString()));
                                //sheetOrder.GetRow(nextRowNum).GetCell(7).CellStyle = style11;
                                sheetOrder.AddMergedRegion(new CellRangeAddress(nextRowNum, nextRowNum, 8, 11));
                                sheetOrder.AddMergedRegion(new CellRangeAddress(nextRowNum, nextRowNum, 13, 15));
                                nextRowNum++;
                            }
                        }
                        DataRow[] drArrayChildQianPin4Class4heji = dtChildQianPin4Class.Select("vcClassType='合计' ");
                        if (drArrayChildQianPin4Class4heji.Length > 0)
                        {
                            sheetOrder.AddMergedRegion(new CellRangeAddress(nextRowNum, nextRowNum , 1, 5)); //合并单元格第二行从第1列到第2列
                            sheetOrder.GetRow(nextRowNum).GetCell(1).SetCellValue("合计");
                            //sheetOrder.GetRow(nextRowNum).GetCell(1).CellStyle = style8;

                            foreach (DataRow dr in drArrayChildQianPin4Class4heji)
                            {
                                sheetOrder.GetRow(nextRowNum).GetCell(6).SetCellValue(Convert.ToDouble(dr["partNum"].ToString()));
                                //sheetOrder.GetRow(nextRowNum).GetCell(6).CellStyle = style11;
                                sheetOrder.GetRow(nextRowNum).GetCell(7).SetCellValue(Convert.ToDouble(dr["qianPinSum"].ToString()));
                                //sheetOrder.GetRow(nextRowNum).GetCell(7).CellStyle = style11;
                                sheetOrder.AddMergedRegion(new CellRangeAddress(nextRowNum, nextRowNum, 8, 11));
                                sheetOrder.AddMergedRegion(new CellRangeAddress(nextRowNum, nextRowNum, 13, 15));
                                nextRowNum++;
                            }
                        }
                    }
                }
                #endregion
                #endregion

                #region 插入具体的数据
                ISheet sheet = hssfworkbook.GetSheet("orderData");
                string[] field = { "vcPackingFactory", "vcTargetYearMonth", "vcDock", "vcCpdcompany", "vcOrderType", "vcOrderNo", "vcSeqno",
                                     "dOrderDate", "dOrderExportDate", "vcPartNo", "vcInsideOutsideType",
                                      "vcPlantQtyDaily1", "vcInputQtyDaily1", "vcResultQtyDaily1", "vcPlantQtyDaily2", "vcInputQtyDaily2",
                                     "vcResultQtyDaily2", "vcPlantQtyDaily3", "vcInputQtyDaily3", "vcResultQtyDaily3", "vcPlantQtyDaily4", "vcInputQtyDaily4",
                                     "vcResultQtyDaily4", "vcPlantQtyDaily5", "vcInputQtyDaily5", "vcResultQtyDaily5", "vcPlantQtyDaily6", "vcInputQtyDaily6",
                                     "vcResultQtyDaily6", "vcPlantQtyDaily7", "vcInputQtyDaily7", "vcResultQtyDaily7", "vcPlantQtyDaily8", "vcInputQtyDaily8",
                                     "vcResultQtyDaily8", "vcPlantQtyDaily9", "vcInputQtyDaily9", "vcResultQtyDaily9", "vcPlantQtyDaily10", "vcInputQtyDaily10",
                                     "vcResultQtyDaily10", "vcPlantQtyDaily11", "vcInputQtyDaily11", "vcResultQtyDaily11", "vcPlantQtyDaily12", "vcInputQtyDaily12",
                                     "vcResultQtyDaily12", "vcPlantQtyDaily13", "vcInputQtyDaily13", "vcResultQtyDaily13", "vcPlantQtyDaily14", "vcInputQtyDaily14",
                                     "vcResultQtyDaily14", "vcPlantQtyDaily15", "vcInputQtyDaily15", "vcResultQtyDaily15", "vcPlantQtyDaily16", "vcInputQtyDaily16",
                                     "vcResultQtyDaily16", "vcPlantQtyDaily17", "vcInputQtyDaily17", "vcResultQtyDaily17", "vcPlantQtyDaily18", "vcInputQtyDaily18",
                                     "vcResultQtyDaily18", "vcPlantQtyDaily19", "vcInputQtyDaily19", "vcResultQtyDaily19", "vcPlantQtyDaily20", "vcInputQtyDaily20",
                                     "vcResultQtyDaily20", "vcPlantQtyDaily21", "vcInputQtyDaily21", "vcResultQtyDaily21", "vcPlantQtyDaily22", "vcInputQtyDaily22",
                                     "vcResultQtyDaily22", "vcPlantQtyDaily23", "vcInputQtyDaily23", "vcResultQtyDaily23", "vcPlantQtyDaily24", "vcInputQtyDaily24",
                                     "vcResultQtyDaily24", "vcPlantQtyDaily25", "vcInputQtyDaily25", "vcResultQtyDaily25", "vcPlantQtyDaily26", "vcInputQtyDaily26",
                                     "vcResultQtyDaily26", "vcPlantQtyDaily27", "vcInputQtyDaily27", "vcResultQtyDaily27", "vcPlantQtyDaily28", "vcInputQtyDaily28",
                                     "vcResultQtyDaily28", "vcPlantQtyDaily29", "vcInputQtyDaily29", "vcResultQtyDaily29", "vcPlantQtyDaily30", "vcInputQtyDaily30",
                                     "vcResultQtyDaily30", "vcPlantQtyDaily31", "vcInputQtyDaily31", "vcResultQtyDaily31","vcPlantQtyDailySum","vcInputQtyDailySum",
                                     "vcResultQtyDailySum", "vcCarType", "vcLastPartNo", "vcPackingSpot","vcTargetMonthFlag", "vcTargetMonthLast"
                };
                int startRow = 2;
                for (int i = 0; i < dtXiData.Rows.Count; i++)
                {
                    
                    IRow row = sheet.CreateRow(startRow + i);
                    for (int j = 0; j < field.Length; j++)
                    {
                        Type type = dtXiData.Columns[field[j]].DataType;
                        ICell cell = row.CreateCell(j);
                        if (type == Type.GetType("System.Decimal"))
                        {
                            if (dtXiData.Rows[i][field[j]].ToString().Trim() != "")
                                cell.SetCellValue(Convert.ToDouble(dtXiData.Rows[i][field[j]].ToString()));
                        }
                        else if (type == Type.GetType("System.Int32"))
                        {
                            if (dtXiData.Rows[i][field[j]].ToString().Trim() != "")
                                cell.SetCellValue(Convert.ToInt32(dtXiData.Rows[i][field[j]].ToString()));
                        }
                        else if (type == Type.GetType("System.Int16"))
                        {
                            if (dtXiData.Rows[i][field[j]].ToString().Trim() != "")
                                cell.SetCellValue(Convert.ToInt16(dtXiData.Rows[i][field[j]].ToString()));
                        }
                        else if (type == Type.GetType("System.Int64"))
                        {
                            if (dtXiData.Rows[i][field[j]].ToString().Trim() != "")
                                cell.SetCellValue(Convert.ToInt64(dtXiData.Rows[i][field[j]].ToString()));
                        }
                        else
                        {
                            cell.SetCellValue(dtXiData.Rows[i][field[j]].ToString());
                        }
                    }
                }

                #endregion

                string rootPath = _webHostEnvironment.ContentRootPath;
                string strFunctionName = "FS0629_Export";

                string strFileName = strFunctionName + "_导出信息_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + loginInfo.UserId + ".xlsx";
                string fileSavePath = rootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Export" + Path.DirectorySeparatorChar;//文件临时目录，导入完成后 删除
                string filepath = fileSavePath + strFileName;

                using (FileStream fs = System.IO.File.OpenWrite(filepath))
                {
                    hssfworkbook.Write(fs);//向打开的这个xls文件中写入数据  
                    fs.Close();
                }
                #endregion
                if (strFileName == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "导出生成文件失败";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = strFileName;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2903", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出报表失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

    }
}

