﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
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

namespace SPPSApi.Controllers.G17
{
    [Route("api/FS1702/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1702Controller : BaseController
    {
        FS1702_Logic fs1702_Logic = new FS1702_Logic();
        private readonly string FunctionID = "FS1702";

        private readonly IWebHostEnvironment _webHostEnvironment;

        public FS1702Controller(IWebHostEnvironment webHostEnvironment)
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
                //if (loginInfo.Special == "财务用户")
                //    res.Add("caiWuBtnVisible", false);
                //else
                //    res.Add("caiWuBtnVisible", true);

                List<Object> dataList_Project = ComFunction.convertAllToResult(fs1702_Logic.getAllProject());//工程
                res.Add("optionProject", dataList_Project);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M08UE0701", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "初始化失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 检索
        [HttpPost]
        [EnableCors("any")]
        public string searchApi([FromBody]dynamic data)
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
            string vcProject = dataForm.vcProject;
            string dChuHeDateFrom = dataForm.dChuHeDateFrom;
            string dChuHeDateTo = dataForm.dChuHeDateTo;
            try
            {
                DataTable dt = fs1702_Logic.Search(vcProject, dChuHeDateFrom, dChuHeDateTo);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("dChuHeDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M08UE1001", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 确认单打印
        [HttpPost]
        [EnableCors("any")]
        [Obsolete]
        public string qrdPrintApi([FromBody]dynamic data)
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
                JArray checkedInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> checkedInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();
                if (checkedInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选择一行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //取出生成确认单的数据
                string[] fields = { "id", "vcPart_id", "vcBackPart_id", "iQuantity", "vcRemark"};
                string vcQueRenNo = checkedInfoData[0]["vcQueRenNo"].ToString();
                string vcProject = checkedInfoData[0]["vcProject"].ToString();
                string dChuHeDate = checkedInfoData[0]["dChuHeDate"].ToString();
                DataTable dt = fs1702_Logic.GetqrdInfo(vcProject, dChuHeDate);
                string strMsg = "";
                //生成excel
                string filepath = fs1702_Logic.generateExcelWithXlt(vcQueRenNo,dt, fields, _webHostEnvironment.ContentRootPath, "FS1702.xlsx", 0, 3, loginInfo.UserId, FunctionID);
                if (strMsg != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = strMsg;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //调打印方法（没做呢）

                //更新打印时间
                fs1702_Logic.qrdPrint(checkedInfoData, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M08UE1004", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 出荷看板打印
        [HttpPost]
        [EnableCors("any")]
        public string kbPrintApi([FromBody]dynamic data)
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
                JArray checkedInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> checkedInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();
                if (checkedInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选择一行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string vcQueRenNo = checkedInfoData[0]["vcQueRenNo"].ToString();
                string vcProject = checkedInfoData[0]["vcProject"].ToString();
                string dChuHeDate = checkedInfoData[0]["dChuHeDate"].ToString();
                for (int i = 0; i < checkedInfoData.Count; i++)
                {
                    if (vcQueRenNo.StartsWith("BJW"))
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "确认单号[" + vcQueRenNo + "]不能进行出荷看板打印！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                //取出生成看板数据
                DataTable dt = fs1702_Logic.getKBData(vcProject,dChuHeDate);
                //调用打印方法（还没做呢）

                //更新打印时间
                fs1702_Logic.kbPrint(checkedInfoData, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M08UE1004", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 出荷完了
        [HttpPost]
        [EnableCors("any")]
        public string chuheOKApi([FromBody]dynamic data)
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
                JArray checkedInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> checkedInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();
                if (checkedInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选择一行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                for (int i = 0; i < checkedInfoData.Count; i++)
                {
                    string vcQueRenNo = checkedInfoData[i]["vcQueRenNo"].ToString();
                    string QueRenPrintFlag= checkedInfoData[i]["QueRenPrintFlag"].ToString();
                    string KBPrintFlagg = checkedInfoData[i]["KBPrintFlag"].ToString();
                    if (vcQueRenNo.StartsWith("BJW"))
                    {
                        if(QueRenPrintFlag=="")
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "确认单号[" + vcQueRenNo + "]：先进行确认单打印！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                    }
                    else
                    {
                        if (QueRenPrintFlag == "" || KBPrintFlagg=="")
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "确认单号[" + vcQueRenNo + "]：先进行确认单和出荷看板打印！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                    }
                }
                fs1702_Logic.chuheOK(checkedInfoData, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M08UE1004", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}
