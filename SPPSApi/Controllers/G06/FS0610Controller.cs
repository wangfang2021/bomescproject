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


        #region 获取对象年月
        [HttpPost]
        [EnableCors("any")]
        public string getDxnyApi()
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
                string vcDXYM = DateTime.Now.AddMonths(1).ToString("yyyy/MM");

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = vcDXYM;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0104", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
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
                string vcDXYM = dataForm.vcDXYM == null ? "" : Convert.ToDateTime(dataForm.vcDXYM).ToString("yyyyMM");
                //用户选择的发注工厂
                JArray vcPlants = dataForm.vcPlant;
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
                for (int i = 0; i < plantList.Count; i++)
                {
                    string strPlant = plantList[i];
                    //取日历数据
                    DataTable dtCalendar = fs0610_Logic.GetCalendar(strPlant, vcDXYM);
                    if (dtCalendar.Rows.Count == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = string.Format("没找到{0}厂{1}月日历。", strPlant, vcDXYM);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    //取Soq数据
                    DataTable dtSoq_dxym = fs0610_Logic.GetSoq(strPlant, vcDXYM, "dxym");
                    DataTable dtSoq_nsym = fs0610_Logic.GetSoq(strPlant, vcDXYM, "nsym");
                    DataTable dtSoq_nnsym = fs0610_Logic.GetSoq(strPlant, vcDXYM, "nnsym");
                    if (dtSoq_dxym.Rows.Count == 0 || dtSoq_nsym.Rows.Count == 0 || dtSoq_nnsym.Rows.Count == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = string.Format("没找到{0}厂{1}月soq数据(内制&已合意)。", strPlant, vcDXYM);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    //调公共方法得到平准化结果
                    //参数：dtCalendar(where工厂/对象月),dtSoQ(where工厂/对象月/计算月/内制或外注/已合意),dt特殊品番,dt特殊供应商
                    ArrayList arrResult_DXYM = new ArrayList();//dtCalendar,dtSoq_dxym,null,null
                    ArrayList arrResult_NSYM = new ArrayList();//dtCalendar,dtSoq_nsym,null,null
                    ArrayList arrResult_NNSYM = new ArrayList();//dtCalendar,dtSoq_nnsym,null,null
                    if (arrResult_DXYM.Count == 0 || arrResult_NSYM.Count == 0 || arrResult_NNSYM.Count == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = string.Format("{0}厂{1}月平准化结果计算失败。", strPlant, vcDXYM);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    //将平准化结果更新DB
                    string vcCLYM = System.DateTime.Now.ToString("yyyyMM");
                    string vcNSYM = Convert.ToDateTime(vcDXYM.Substring(0, 4) + "-" + vcDXYM.Substring(4, 2) + "-01").ToString("yyyyMM");
                    string vcNNSYM = Convert.ToDateTime(vcNSYM.Substring(0, 4) + "-" + vcNSYM.Substring(4, 2) + "-01").ToString("yyyyMM");
                    fs0610_Logic.SaveResult(vcCLYM, vcDXYM, vcNSYM, vcNNSYM, strPlant, arrResult_DXYM, arrResult_NSYM, arrResult_NNSYM, loginInfo.UserId);

                }

                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UI0103", null, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0201", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "请求失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 展开SOQReply   张德勤写的，还需要修改
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

                string varDxny = dataForm.varDxny == null ? "" : Convert.ToDateTime(dataForm.varDxny).ToString("yyyy/MM");

                fs0610_Logic.zk(varDxny, loginInfo.UserId);

                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UI0103", null, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                //apiResult.data = count;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0201", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 下载SOQReply  张德勤写的，还需要修改
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

                string varDxny = dataForm.varDxny == null ? "" : Convert.ToDateTime(dataForm.varDxny).ToString("yyyy/MM");

                DataTable dt = fs0610_Logic.search(varDxny);
                string[] fields = { "PartsNo", "发注工厂", "订货频度", "CFC", "OrdLot", "N Units"
                ,"N PCS","D1","D2","D3","D4","D5","D6","D7","D8","D9","D10","D11","D12","D13","D14"
                ,"D15","D16","D17","D18","D19","D20","D21","D22","D23","D24","D25","D26","D27","D28"
                ,"D29","D30","D31","N+1 O/L","N+1 Units","N+1 PCS","N+2 O/L","N+2 Units","N+2 PCS"
                };
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0610_Download.xlsx", 1, loginInfo.UserId, FunctionID);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0201", ex, loginInfo.UserId);
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
            try
            {
                string vcDxny = DateTime.Now.AddMonths(1).ToString("yyyyMM");
                object b = dataForm.vcFZGC;
                string[] vcFZGC = b.ToString().Replace("\r\n", "").Replace("\"", "").Replace("[", "").Replace("]", "").Replace(" ", "").Split(','); ;
                //生成生产计划  
                string msg = fs0610_Logic.createProPlan(vcDxny, vcFZGC, loginInfo.UserId);
                if (msg != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = msg;
                    apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "生成成功。";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0201", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "生成失败";
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
                string vcDxny = DateTime.Now.AddMonths(1).ToString("yyyyMM");
                object b = dataForm.vcFZGC;
                string[] vcFZGC = b.ToString().Replace("\r\n", "").Replace("\"", "").Replace("[", "").Replace("]", "").Replace(" ", "").Split(',');
                string[] fields = { "vcMonth","vcPlant","vcPartsno","vcDock","vcCarType","vcCalendar1","vcCalendar2","vcCalendar3","vcCalendar4",
                    "vcPartsNameCHN","vcProject1","vcProjectName","vcCurrentPastCode","vcMonTotal","vcCalendar1","vcCalendar2","vcCalendar3","vcCalendar4",
                    "TD1b","TD1y","TD2b","TD2y","TD3b","TD3y","TD4b","TD4y","TD5b","TD5y","TD6b","TD6y","TD7b","TD7y","TD8b","TD8y","TD9b","TD9y","TD10b","TD10y",
                    "TD11b","TD11y","TD12b","TD12y","TD13b","TD13y","TD14b","TD14y","TD15b","TD15y","TD16b","TD16y","TD17b","TD17y","TD18b","TD18y","TD19b","TD19y","TD20b","TD20y",
                    "TD21b","TD21y","TD22b","TD22y","TD23b","TD23y","TD24b","TD24y","TD25b","TD25y","TD26b","TD26y","TD27b","TD27y","TD28b","TD28y","TD29b","TD29y","TD30b","TD30y","TD31b","TD31y",
                    "ED1b","ED1y","ED2b","ED2y","ED3b","ED3y","ED4b","ED4y","ED5b","ED5y","ED6b","ED6y","ED7b","ED7y","ED8b","ED8y","ED9b","ED9y","ED10b","ED10y",
                    "ED11b","ED11y","ED12b","ED12y","ED13b","ED13y","ED14b","ED14y","ED15b","ED15y","ED16b","ED16y","ED17b","ED17y","ED18b","ED18y","ED19b","ED19y","ED20b","ED20y",
                    "ED21b","ED21y","ED22b","ED22y","ED23b","ED23y","ED24b","ED24y","ED25b","ED25y","ED26b","ED26y","ED27b","ED27y","ED28b","ED28y","ED29b","ED29y","ED30b","ED30y","ED31b","ED31y"
                };
                string filepath = "";
                for (int i = 0; i < vcFZGC.Length; i++)
                {
                    DataTable dt = fs0610_Logic.dowloadProPlan(vcDxny, vcFZGC[i], loginInfo.UserId);
                    filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0610_PlanMake.xlsx", 1, loginInfo.UserId, FunctionID);
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

        #region 上传生产计划
        [HttpPost]
        [EnableCors("any")]
        public string uploadProPlan([FromBody] dynamic data)
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



                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UI0103", null, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0201", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "请求失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }

}