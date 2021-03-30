using Common;
using DataEntity;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Collections;
using System.IO;
using SoqCompute;

namespace SPPSApi.Controllers.G06
{
    [Route("api/FS0610/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0610Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string FunctionID = "FS0610";
        FS0610_Logic fs0610_Logic = new FS0610_Logic();

        public FS0610Controller(IWebHostEnvironment webHostEnvironment)
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

                List<Object> dataList_C000 = ComFunction.convertAllToResult(ComFunction.getTCode("C000"));//工厂
                res.Add("C000", dataList_C000);

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

        #region 发注工厂下拉框改变
        [HttpPost]
        [EnableCors("any")]
        public string FZGCChangeApi([FromBody] dynamic data)
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
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                string strCLYM = DateTime.Now.ToString("yyyyMM");
                //用户选择的发注工厂
                JArray vcPlants = dataForm.vcFZGC;
                List<string> plantList = null;
                try
                {
                    plantList = vcPlants.ToObject<List<string>>();
                }
                catch (Exception ex)
                {
                }
                if (plantList != null && plantList.Count > 0)
                {
                    for (int i = 0; i < plantList.Count; i++)
                    {
                        int iStep = 0;
                        string plant = plantList[i].ToString();
                        if (fs0610_Logic.isHaveSORReplyData(plant, strCLYM))
                            iStep = 1;
                        if (fs0610_Logic.isSCPlan(plant, strCLYM))
                            iStep = 2;
                        if (fs0610_Logic.isZhankai(plant, strCLYM))
                            iStep = 3;
                        res.Add("iStep_" + plant.ToString(), iStep);
                    }
                }

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
                string strYearMonth = dataForm.vcDXYM == null ? "" : Convert.ToDateTime(dataForm.vcDXYM + "/01").ToString("yyyyMM");
                string strYearMonth_2 = dataForm.vcDXYM == null ? "" : Convert.ToDateTime(dataForm.vcDXYM).AddMonths(1).ToString("yyyyMM");
                string strYearMonth_3 = dataForm.vcDXYM == null ? "" : Convert.ToDateTime(dataForm.vcDXYM).AddMonths(2).ToString("yyyyMM");
                string type = dataForm.type == null ? "" : dataForm.type;
                //用户选择的发注工厂
                JArray vcPlants = dataForm.vcFZGC;
                List<string> plantList = null;
                try
                {
                    plantList = vcPlants.ToObject<List<string>>();
                }
                catch (Exception ex)
                {
                }
                if (plantList == null || plantList.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请选择工厂。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //判断是否计算过
                if (type == "check" && fs0610_Logic.isCal(strYearMonth, plantList))
                {//已经计算了
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "message";
                    apiResult.data = "已经计算过";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                ComputeMain soqCompute = new ComputeMain();
                for (int i = 0; i < plantList.Count; i++)
                {
                    string strPlant = plantList[i];
                    bool find = fs0610_Logic.GetSoq(strPlant, strYearMonth, "dxym").Rows.Count > 0 ? true : false;
                    //bool find_2 = fs0610_Logic.GetSoq(strPlant, strYearMonth, "nsym").Rows.Count > 0 ? true : false;
                    //bool find_3 = fs0610_Logic.GetSoq(strPlant, strYearMonth, "nnsym").Rows.Count > 0 ? true : false;

                    if (find == false)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = string.Format("没找到{0}厂{1}月soq数据(内制)。", strPlant, strYearMonth);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }

                    //取日历数据-三个月
                    DataTable dtCalendar = fs0610_Logic.GetCalendar(strPlant, strYearMonth);
                    if (find && dtCalendar.Rows.Count == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = string.Format("没找到{0}厂{1}月日历。", strPlant, strYearMonth);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    DataTable dtCalendar_2 = fs0610_Logic.GetCalendar(strPlant, strYearMonth_2);
                    if (find && dtCalendar_2.Rows.Count == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = string.Format("没找到{0}厂{1}月日历。", strPlant, strYearMonth_2);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    DataTable dtCalendar_3 = fs0610_Logic.GetCalendar(strPlant, strYearMonth_3);
                    if (find && dtCalendar_3.Rows.Count == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = string.Format("没找到{0}厂{1}月日历。", strPlant, strYearMonth_3);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }

                    //取Soq数据-三个月
                    DataTable dtSoq_dxym = fs0610_Logic.GetSoqHy(strPlant, strYearMonth, "dxym");
                    DataTable dtSoq_nsym = fs0610_Logic.GetSoqHy(strPlant, strYearMonth, "nsym");
                    DataTable dtSoq_nnsym = fs0610_Logic.GetSoqHy(strPlant, strYearMonth, "nnsym");
                    if (find && dtSoq_dxym.Rows.Count == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = string.Format("没找到{0}厂{1}月soq数据(内制&已合意)。", strPlant, strYearMonth);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (find && dtSoq_nsym.Rows.Count == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = string.Format("没找到{0}厂{1}月soq数据(内制&已合意)。", strPlant, strYearMonth);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (find && dtSoq_nnsym.Rows.Count == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = string.Format("没找到{0}厂{1}月soq数据(内制&已合意)。", strPlant, strYearMonth);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }

                for (int i = 0; i < plantList.Count; i++)
                {
                    string strPlant = plantList[i];
                    bool find = fs0610_Logic.GetSoq(strPlant, strYearMonth, "dxym").Rows.Count > 0 ? true : false;
                    //bool find_2 = fs0610_Logic.GetSoq(strPlant, strYearMonth, "nsym").Rows.Count > 0 ? true : false;
                    //bool find_3 = fs0610_Logic.GetSoq(strPlant, strYearMonth, "nnsym").Rows.Count > 0 ? true : false;

                    //取日历数据-三个月
                    DataTable dtCalendar = fs0610_Logic.GetCalendar(strPlant, strYearMonth);
                    DataTable dtCalendar_2 = fs0610_Logic.GetCalendar(strPlant, strYearMonth_2);
                    DataTable dtCalendar_3 = fs0610_Logic.GetCalendar(strPlant, strYearMonth_3);

                    //取Soq数据-三个月
                    DataTable dtSoq_dxym = fs0610_Logic.GetSoqHy(strPlant, strYearMonth, "dxym");
                    DataTable dtSoq_nsym = fs0610_Logic.GetSoqHy(strPlant, strYearMonth, "nsym");
                    DataTable dtSoq_nnsym = fs0610_Logic.GetSoqHy(strPlant, strYearMonth, "nnsym");

                    //调公共方法得到平准化结果
                    //参数：dtCalendar(where工厂/对象月),dtSoQ(where工厂/对象月/计算月/内制或外注/已合意),dt特殊品番,dt特殊供应商
                    ArrayList arrResult_DXYM = new ArrayList();//dtCalendar,dtSoq_dxym,null,null
                    ArrayList arrResult_NSYM = new ArrayList();//dtCalendar,dtSoq_nsym,null,null
                    ArrayList arrResult_NNSYM = new ArrayList();//dtCalendar,dtSoq_nnsym,null,null
                    int iSub = fs0610_Logic.getPingZhunAddSubDay();
                    if (find)
                    {
                        arrResult_DXYM = soqCompute.getPinZhunList(dtSoq_dxym, dtCalendar, null, null, strYearMonth, iSub, "DXYM");
                        arrResult_NSYM = soqCompute.getPinZhunList(dtSoq_nsym, dtCalendar_2, null, null, strYearMonth_2, iSub, "NSYM");
                        arrResult_NNSYM = soqCompute.getPinZhunList(dtSoq_nnsym, dtCalendar_3, null, null, strYearMonth_3, iSub, "NNSYM");
                    }
                    //if (find_2)
                    //    arrResult_NSYM = soqCompute.getPinZhunList(dtSoq_nsym, dtCalendar_2, null, null);
                    //if (find_3)
                    //    arrResult_NNSYM = soqCompute.getPinZhunList(dtSoq_nnsym, dtCalendar_3, null, null);

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
                    fs0610_Logic.SaveResult(vcCLYM, strYearMonth, strYearMonth_2, strYearMonth_3, strPlant, arrResult_DXYM, arrResult_NSYM, arrResult_NNSYM, loginInfo.UserId, loginInfo.UnitCode);
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
                string strYearMonth = dataForm.vcDXYM == null ? "" : Convert.ToDateTime(dataForm.vcDXYM + "/01").ToString("yyyyMM");
                //用户选择的发注工厂
                JArray vcPlants = dataForm.vcFZGC;
                List<string> plantList = null;
                try
                {
                    plantList = vcPlants.ToObject<List<string>>();
                }
                catch (Exception ex)
                {
                }
                if (plantList == null || plantList.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请选择工厂。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                DataTable dt = fs0610_Logic.getZhankaiData(false, plantList);
                if (dt.Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "没有要展开的数据";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                fs0610_Logic.zk(loginInfo.UserId, plantList);
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
                string strYearMonth = dataForm.vcDXYM == null ? "" : Convert.ToDateTime(dataForm.vcDXYM + "/01").ToString("yyyyMM");
                string strYearMonth_2 = dataForm.vcDXYM == null ? "" : Convert.ToDateTime(dataForm.vcDXYM).AddMonths(1).ToString("yyyyMM");
                string strYearMonth_3 = dataForm.vcDXYM == null ? "" : Convert.ToDateTime(dataForm.vcDXYM).AddMonths(2).ToString("yyyyMM");
                //用户选择的发注工厂
                JArray vcPlants = dataForm.vcFZGC;
                List<string> plantList = null;
                try
                {
                    plantList = vcPlants.ToObject<List<string>>();
                }
                catch (Exception ex)
                {
                }
                if (plantList == null || plantList.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请选择工厂。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                DataTable dt = fs0610_Logic.search(strYearMonth, strYearMonth_2, strYearMonth_3, plantList);
                string[] fields = { "PartsNo", "发注工厂", "订货频度", "CFC", "OrdLot", "N Units"
                ,"N PCS","iD1","iD2","iD3","iD4","iD5","iD6","iD7","iD8","iD9","iD10","iD11","iD12","iD13","iD14"
                ,"iD15","iD16","iD17","iD18","iD19","iD20","iD21","iD22","iD23","iD24","iD25","iD26","iD27","iD28"
                ,"iD29","iD30","iD31","N+1 O/L","N+1 Units","N+1 PCS","N+2 O/L","N+2 Units","N+2 PCS"
                };
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0610_Download.xlsx", 1, loginInfo.UserId, FunctionID, true);
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

        #region 生成生产计划
        [HttpPost]
        [EnableCors("any")]
        public string createProPlan([FromBody] dynamic data)
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
            string msg = "";
            try
            {
                string vcDxny = dataForm.vcDXYM == null ? "" : Convert.ToDateTime(dataForm.vcDXYM + "/01").ToString("yyyyMM");
                object b = dataForm.vcFZGC;
                string[] vcFZGC = b.ToString().Replace("\r\n", "").Replace("\"", "").Replace("[", "").Replace("]", "").Replace(" ", "").Split(','); ;
                //生成生产计划  
                msg = fs0610_Logic.createProPlan(vcDxny, vcFZGC, loginInfo.UserId);
                if (msg != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = msg;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0201", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "生成失败" + msg;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 下载生产计划
        [HttpPost]
        [EnableCors("any")]
        public string downloadProPlan([FromBody] dynamic data)
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
                string vcDxny = dataForm.vcDXYM == null ? "" : Convert.ToDateTime(dataForm.vcDXYM + "/01").ToString("yyyyMM");
                object b = dataForm.vcFZGC;
                string[] vcFZGC = b.ToString().Replace("\r\n", "").Replace("\"", "").Replace("[", "").Replace("]", "").Replace(" ", "").Split(',');
                string[] heads = { "对象月","工厂","品番","受入","车型","紧急区分","工程1","工程2","工程3","工程4","供应商编码",
                    "工程号","工程名","号旧","月度总量",
                    "1日白","1日夜","2日白","2日夜","3日白","3日夜","4日白","4日夜","5日白","5日夜","6日白","6日夜","7日白","7日夜","8日白","8日夜","9日白","9日夜","10日白","10日夜",
                    "11日白","11日夜","12日白","12日夜","13日白","13日夜","14日白","14日夜","15日白","15日夜","16日白","16日夜","17日白","17日夜","18日白","18日夜","19日白","19日夜","20日白","20日夜",
                    "21日白","21日夜","22日白","22日夜","23日白","23日夜","24日白","24日夜","25日白","25日夜","26日白","26日夜","27日白","27日夜","28日白","28日夜","29日白","29日夜","30日白","30日夜","31日白","31日夜",
                    "1日白","1日夜","2日白","2日夜","3日白","3日夜","4日白","4日夜","5日白","5日夜","6日白","6日夜","7日白","7日夜","8日白","8日夜","9日白","9日夜","10日白","10日夜",
                    "11日白","11日夜","12日白","12日夜","13日白","13日夜","14日白","14日夜","15日白","15日夜","16日白","16日夜","17日白","17日夜","18日白","18日夜","19日白","19日夜","20日白","20日夜",
                    "21日白","21日夜","22日白","22日夜","23日白","23日夜","24日白","24日夜","25日白","25日夜","26日白","26日夜","27日白","27日夜","28日白","28日夜","29日白","29日夜","30日白","30日夜","31日白","31日夜",
                };
                string[] fields = { "vcMonth","vcPlant","vcPartsno","vcDock","vcCarType","vcEDflag","vcCalendar1","vcCalendar2","vcCalendar3","vcCalendar4","vcSupplier_id",
                    "vcProject1","vcProjectName","vcCurrentPastCode","vcMonTotal",
                    "TD1b","TD1y","TD2b","TD2y","TD3b","TD3y","TD4b","TD4y","TD5b","TD5y","TD6b","TD6y","TD7b","TD7y","TD8b","TD8y","TD9b","TD9y","TD10b","TD10y",
                    "TD11b","TD11y","TD12b","TD12y","TD13b","TD13y","TD14b","TD14y","TD15b","TD15y","TD16b","TD16y","TD17b","TD17y","TD18b","TD18y","TD19b","TD19y","TD20b","TD20y",
                    "TD21b","TD21y","TD22b","TD22y","TD23b","TD23y","TD24b","TD24y","TD25b","TD25y","TD26b","TD26y","TD27b","TD27y","TD28b","TD28y","TD29b","TD29y","TD30b","TD30y","TD31b","TD31y",
                    "ED1b","ED1y","ED2b","ED2y","ED3b","ED3y","ED4b","ED4y","ED5b","ED5y","ED6b","ED6y","ED7b","ED7y","ED8b","ED8y","ED9b","ED9y","ED10b","ED10y",
                    "ED11b","ED11y","ED12b","ED12y","ED13b","ED13y","ED14b","ED14y","ED15b","ED15y","ED16b","ED16y","ED17b","ED17y","ED18b","ED18y","ED19b","ED19y","ED20b","ED20y",
                    "ED21b","ED21y","ED22b","ED22y","ED23b","ED23y","ED24b","ED24y","ED25b","ED25y","ED26b","ED26y","ED27b","ED27y","ED28b","ED28y","ED29b","ED29y","ED30b","ED30y","ED31b","ED31y"
                };
                string filepath = "";
                string tbName = "";
                DataTable dt = new DataTable();
                for (int i = 0; i < vcFZGC.Length; i++)
                {
                    dt.Merge(fs0610_Logic.dowloadProPlan(vcDxny, vcFZGC[i], loginInfo.UserId));
                    tbName += "#" + vcFZGC[i];
                }
                if (dt != null && dt.Rows.Count > 0)
                    filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0610_PlanMake.xlsx", 1, loginInfo.UserId, FunctionID);
                //filepath += ComFunction.DataTableToExcel(heads, fields, dt, ".", tbName + "生产计划_", loginInfo.UserId, "", ref msg) + ";";
                if (filepath == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "未生成 " + tbName + " 厂生产计划，不能导出！";
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