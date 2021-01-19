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
                        apiResult.data =string.Format("没找到{0}厂{1}月日历。",strPlant,vcDXYM);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    //取Soq数据
                    DataTable dtSoq_dxym = fs0610_Logic.GetSoq(strPlant, vcDXYM, "dxym");
                    DataTable dtSoq_nsym = fs0610_Logic.GetSoq(strPlant, vcDXYM, "nsym");
                    DataTable dtSoq_nnsym = fs0610_Logic.GetSoq(strPlant, vcDXYM, "nnsym");
                    if(dtSoq_dxym.Rows.Count==0 || dtSoq_nsym.Rows.Count==0 || dtSoq_nnsym.Rows.Count==0)
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
                    if(arrResult_DXYM.Count==0 || arrResult_NSYM.Count==0 || arrResult_NNSYM.Count==0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = string.Format("{0}厂{1}月平准化结果计算失败。", strPlant, vcDXYM);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    //将平准化结果更新DB
                    string vcCLYM = System.DateTime.Now.ToString("yyyyMM");
                    string vcNSYM = Convert.ToDateTime(vcDXYM.Substring(0, 4) + "-" + vcDXYM.Substring(4, 2) + "-01").ToString("yyyyMM");
                    string vcNNSYM= Convert.ToDateTime(vcNSYM.Substring(0, 4) + "-" + vcNSYM.Substring(4, 2) + "-01").ToString("yyyyMM");
                    fs0610_Logic.SaveResult(vcCLYM,vcDXYM,vcNSYM, vcNNSYM, strPlant, arrResult_DXYM, arrResult_NSYM, arrResult_NNSYM,loginInfo.UserId);

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
    }

}