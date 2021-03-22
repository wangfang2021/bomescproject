using Common;
using DataEntity;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SoqCompute;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace SPPSApi.Controllers.G06
{
    [Route("api/FS0611/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0611Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string FunctionID = "FS0611";
        FS0611_Logic fs0611_Logic = new FS0611_Logic();

        public FS0611Controller(IWebHostEnvironment webHostEnvironment)
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
                 
                DateTime dNow = DateTime.Now.AddMonths(1);
                res.Add("yearMonth", dNow.ToString("yyyy/MM"));
                int iStep = 0;
                DataTable dt = fs0611_Logic.getZhankaiData(false);
                if (dt.Rows.Count != 0)
                {
                    iStep = 1;
                }
                DataTable dt2 = fs0611_Logic.getZhankaiData(true);
                if (dt2.Rows.Count != 0)
                {
                    iStep = 2;
                }
                res.Add("iStep", iStep);
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

        #region 生成SOQReply
        [HttpPost]
        [EnableCors("any")]
        public string createApi([FromBody] dynamic data)
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
                string strYearMonth_2 = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth).AddMonths(1).ToString("yyyyMM");
                string strYearMonth_3 = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth).AddMonths(2).ToString("yyyyMM");

                DataTable dt_C000 =ComFunction.getTCode("C000");
                ComputeMain soqCompute = new ComputeMain();
                for (int i = 0; i < dt_C000.Rows.Count; i++)
                {
                    string strPlant = dt_C000.Rows[i]["vcValue"].ToString();

                    //取特殊厂家--可以为空
                    DataTable dtSpecialSupplier = fs0611_Logic.GetSpecialSupplier(strPlant, strYearMonth, strYearMonth);
                    DataTable dtSpecialSupplier_2 = fs0611_Logic.GetSpecialSupplier(strPlant, strYearMonth, strYearMonth_2);
                    DataTable dtSpecialSupplier_3 = fs0611_Logic.GetSpecialSupplier(strPlant, strYearMonth, strYearMonth_3);

                    //取特殊品番--可以为空，特殊品番只取一个，只要平准化结果有这个品番，就特别处理
                    DataTable dtSpecialPart = fs0611_Logic.GetSpecialPartId(strPlant, strYearMonth, strYearMonth);
                    DataTable dtSpecialPart_2 = fs0611_Logic.GetSpecialPartId(strPlant, strYearMonth, strYearMonth_2);
                    DataTable dtSpecialPart_3 = fs0611_Logic.GetSpecialPartId(strPlant, strYearMonth, strYearMonth_3);

                    bool find   = fs0611_Logic.GetSoq(strPlant, strYearMonth, "dxym").Rows.Count > 0 ? true : false;
                    bool find_2 = fs0611_Logic.GetSoq(strPlant, strYearMonth, "nsym").Rows.Count > 0 ? true : false;
                    bool find_3 = fs0611_Logic.GetSoq(strPlant, strYearMonth, "nnsym").Rows.Count > 0 ? true : false;


                    //取日历数据-三个月
                    DataTable dtCalendar = fs0611_Logic.GetCalendar(strPlant, strYearMonth);
                    if (find&&dtCalendar.Rows.Count == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = string.Format("没找到{0}厂{1}月日历。", strPlant, strYearMonth);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    DataTable dtCalendar_2 = fs0611_Logic.GetCalendar(strPlant, strYearMonth_2);
                    if (find_2&&dtCalendar_2.Rows.Count == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = string.Format("没找到{0}厂{1}月日历。", strPlant, strYearMonth_2);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    DataTable dtCalendar_3 = fs0611_Logic.GetCalendar(strPlant, strYearMonth_3);
                    if (find_3&&dtCalendar_3.Rows.Count == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = string.Format("没找到{0}厂{1}月日历。", strPlant, strYearMonth_3);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }

                    //取Soq数据-三个月
                    DataTable dtSoq_dxym = fs0611_Logic.GetSoqHy(strPlant, strYearMonth, "dxym");
                    DataTable dtSoq_nsym = fs0611_Logic.GetSoqHy(strPlant, strYearMonth, "nsym");
                    DataTable dtSoq_nnsym = fs0611_Logic.GetSoqHy(strPlant, strYearMonth, "nnsym");
                    if (find && dtSoq_dxym.Rows.Count == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = string.Format("没找到{0}厂{1}月soq数据(内制&已合意)。", strPlant, strYearMonth);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (find_2 && dtSoq_nsym.Rows.Count == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = string.Format("没找到{0}厂{1}月soq数据(内制&已合意)。", strPlant, strYearMonth);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (find_3 && dtSoq_nnsym.Rows.Count == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = string.Format("没找到{0}厂{1}月soq数据(内制&已合意)。", strPlant, strYearMonth);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }

                    //验证特殊品番特殊厂家维护的区间，是否有稼动日
                    string strCheckSpecial = "";
                    string strCheckSpecial_2 = "";
                    string strCheckSpecial_3 = "";

                    strCheckSpecial = ComputeUtil.checkSpecial(strPlant, dtCalendar, dtSpecialSupplier, dtSpecialPart);
                    strCheckSpecial_2 = ComputeUtil.checkSpecial(strPlant, dtCalendar_2, dtSpecialSupplier_2, dtSpecialPart_2);
                    strCheckSpecial_3 = ComputeUtil.checkSpecial(strPlant, dtCalendar_3, dtSpecialSupplier_3, dtSpecialPart_3);
                    if (strCheckSpecial != null|| strCheckSpecial_2!=null || strCheckSpecial_3 !=null)
                    {
                        string err = "";
                        err = strCheckSpecial == null ? err : err + strCheckSpecial;
                        err = strCheckSpecial_2 == null ? err: err + strCheckSpecial_2;
                        err = strCheckSpecial_3 == null ? err: err + strCheckSpecial_3;

                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = err;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
               
                }

 
                for (int i = 0; i < dt_C000.Rows.Count; i++)
                {
                    string strPlant = dt_C000.Rows[i]["vcValue"].ToString();
                    //取特殊厂家--可以为空
                    DataTable dtSpecialSupplier = fs0611_Logic.GetSpecialSupplier(strPlant, strYearMonth, strYearMonth);
                    DataTable dtSpecialSupplier_2 = fs0611_Logic.GetSpecialSupplier(strPlant, strYearMonth, strYearMonth_2);
                    DataTable dtSpecialSupplier_3 = fs0611_Logic.GetSpecialSupplier(strPlant, strYearMonth, strYearMonth_3);

                    //取特殊品番--可以为空，特殊品番只取一个，只要平准化结果有这个品番，就特别处理
                    DataTable dtSpecialPart = fs0611_Logic.GetSpecialPartId(strPlant, strYearMonth, strYearMonth);
                    DataTable dtSpecialPart_2 = fs0611_Logic.GetSpecialPartId(strPlant, strYearMonth, strYearMonth_2);
                    DataTable dtSpecialPart_3 = fs0611_Logic.GetSpecialPartId(strPlant, strYearMonth, strYearMonth_3);


                    bool find = fs0611_Logic.GetSoq(strPlant, strYearMonth, "dxym").Rows.Count > 0 ? true : false;
                    bool find_2 = fs0611_Logic.GetSoq(strPlant, strYearMonth, "nsym").Rows.Count > 0 ? true : false;
                    bool find_3 = fs0611_Logic.GetSoq(strPlant, strYearMonth, "nnsym").Rows.Count > 0 ? true : false;


                    //取日历数据-三个月
                    DataTable dtCalendar = fs0611_Logic.GetCalendar(strPlant, strYearMonth);
                    DataTable dtCalendar_2 = fs0611_Logic.GetCalendar(strPlant, strYearMonth_2);
                    DataTable dtCalendar_3 = fs0611_Logic.GetCalendar(strPlant, strYearMonth_3);

                    //取Soq数据-三个月
                    DataTable dtSoq_dxym = fs0611_Logic.GetSoqHy(strPlant, strYearMonth, "dxym");
                    DataTable dtSoq_nsym = fs0611_Logic.GetSoqHy(strPlant, strYearMonth, "nsym");
                    DataTable dtSoq_nnsym = fs0611_Logic.GetSoqHy(strPlant, strYearMonth, "nnsym");

                    //调公共方法得到平准化结果
                    //参数：dtCalendar(where工厂/对象月),dtSoQ(where工厂/对象月/计算月/内制或外注/已合意),dt特殊品番,dt特殊供应商
                    ArrayList arrResult_DXYM = new ArrayList();//dtCalendar,dtSoq_dxym,null,null
                    ArrayList arrResult_NSYM = new ArrayList();//dtCalendar,dtSoq_nsym,null,null
                    ArrayList arrResult_NNSYM = new ArrayList();//dtCalendar,dtSoq_nnsym,null,null

                    if (!find && !find_2 && !find_3)
                        continue;//如果该厂没有要处理的数据，则跳过

                    int iSub=fs0611_Logic.getPingZhunAddSubDay();

                    if (find)
                        arrResult_DXYM=soqCompute.getPinZhunList(dtSoq_dxym, dtCalendar, dtSpecialSupplier, dtSpecialPart, strYearMonth, iSub);
                    if (find_2)
                        arrResult_NSYM =soqCompute.getPinZhunList(dtSoq_nsym, dtCalendar_2, dtSpecialSupplier_2, dtSpecialPart_2, strYearMonth_2, iSub);
                    if (find_3)
                        arrResult_NNSYM =soqCompute.getPinZhunList(dtSoq_nnsym, dtCalendar_3, dtSpecialSupplier_3, dtSpecialPart_3, strYearMonth_3, iSub);

                    if (arrResult_DXYM.Count == 0 || arrResult_NSYM.Count == 0 || arrResult_NNSYM.Count == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = string.Format("{0}厂{1}月平准化结果计算失败。", strPlant, strYearMonth);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    //将平准化结果更新DB
                    string vcCLYM = System.DateTime.Now.ToString("yyyyMM");
                    string vcNSYM = Convert.ToDateTime(strYearMonth.Substring(0, 4) + "-" + strYearMonth.Substring(4, 2) + "-01").ToString("yyyyMM");
                    string vcNNSYM = Convert.ToDateTime(vcNSYM.Substring(0, 4) + "-" + vcNSYM.Substring(4, 2) + "-01").ToString("yyyyMM");
                    fs0611_Logic.SaveResult(vcCLYM, strYearMonth, strYearMonth_2, strYearMonth_3, strPlant, arrResult_DXYM, arrResult_NSYM, arrResult_NNSYM, loginInfo.UserId,loginInfo.UnitCode);
                }

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "生成SOQ Reply成功";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE1101", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "生成SOQReply失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 展开SOQReply
        [HttpPost]
        [EnableCors("any")]
        public string zkApi([FromBody] dynamic data)
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

                DataTable dt=fs0611_Logic.getZhankaiData(false);
                if (dt.Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "没有要展开的数据";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                fs0611_Logic.zk(loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                //apiResult.data = count;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE1104", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 下载SOQReply
        [HttpPost]
        [EnableCors("any")]
        public string downloadApi([FromBody] dynamic data)
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
                string strYearMonth_2 = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth).AddMonths(1).ToString("yyyyMM");
                string strYearMonth_3 = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth).AddMonths(2).ToString("yyyyMM");


                DataTable dt = fs0611_Logic.search(strYearMonth, strYearMonth_2, strYearMonth_3);
                string[] fields = { "PartsNo", "发注工厂", "订货方式", "CFC", "OrdLot", "N Units"
                ,"N PCS","iD1","iD2","iD3","iD4","iD5","iD6","iD7","iD8","iD9","iD10","iD11","iD12","iD13","iD14"
                ,"iD15","iD16","iD17","iD18","iD19","iD20","iD21","iD22","iD23","iD24","iD25","iD26","iD27","iD28"
                ,"iD29","iD30","iD31","N+1 O/L","N+1 Units","N+1 PCS","N+2 O/L","N+2 Units","N+2 PCS"
                };
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0611_Download.xlsx", 1, loginInfo.UserId, FunctionID,true);
                
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE1103", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }

}