using Common;
using DataEntity;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace SPPSApi.Controllers.G06
{
    [Route("api/FS0602/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0602Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string FunctionID = "FS0602";
        FS0602_Logic fs0602_Logic = new FS0602_Logic();

        public FS0602Controller(IWebHostEnvironment webHostEnvironment)
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
                DateTime varDxny = DateTime.Now.AddMonths(1);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = varDxny;
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

        #region 检索数据
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

            FS0602_DataEntity searchForm = new FS0602_DataEntity();

            searchForm.varDxny = dataForm.varDxny == null ? "" : Convert.ToDateTime(dataForm.varDxny).ToString("yyyyMM");
            searchForm.varDyzt = dataForm.varDyzt == null ? "" : dataForm.varDyzt;
            searchForm.varHyzt = dataForm.varHyzt == null ? "" : dataForm.varHyzt;
            searchForm.PARTSNO = dataForm.PARTSNO == null ? "" : dataForm.PARTSNO;
            searchForm.CARFAMILYCODE = dataForm.CARFAMILYCODE == null ? "" : dataForm.CARFAMILYCODE;
            searchForm.CURRENTPASTCODE = dataForm.CURRENTPASTCODE == null ? "" : dataForm.CURRENTPASTCODE;
            searchForm.varMakingOrderType = dataForm.varMakingOrderType == null ? "" : dataForm.varMakingOrderType;

            if (dataForm.iFZGC.Count == 0 || dataForm.iFZGC == null)
            {
                searchForm.iFZGC = "";
            }
            else
            {
                string temp = "";
                for (int i = 0; i < dataForm.iFZGC.Count; i++)
                {
                    if (i == dataForm.iFZGC.Count - 1)
                        temp += dataForm.iFZGC[i];
                    else
                        temp += dataForm.iFZGC[i] + ",";
                }
                searchForm.iFZGC = temp;
            }

            //searchForm.iFZGC = dataForm.iFZGC.Length == 0 ? "" :  dataForm.iFZGC;
            searchForm.INOUTFLAG = dataForm.INOUTFLAG == null ? "" : dataForm.INOUTFLAG;
            searchForm.SUPPLIERCODE = dataForm.SUPPLIERCODE == null ? "" : dataForm.SUPPLIERCODE;
            searchForm.iSupplierPlant = dataForm.iSupplierPlant == null ? null : Convert.ToInt32(dataForm.iSupplierPlant);


            try
            {
                DataTable dt = fs0602_Logic.Search(searchForm);
                List<Object> dataList = ComFunction.convertAllToResult(dt);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
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

        #region 展开内示（向供应商展开内示）
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

                FS0602_DataEntity searchForm = new FS0602_DataEntity();

                searchForm.varDxny = dataForm.varDxny == null ? "" : Convert.ToDateTime(dataForm.varDxny).ToString("yyyyMM");
                searchForm.varDyzt = dataForm.varDyzt == null ? "" : dataForm.varDyzt;
                searchForm.varHyzt = dataForm.varHyzt == null ? "" : dataForm.varHyzt;
                searchForm.PARTSNO = dataForm.PARTSNO == null ? "" : dataForm.PARTSNO;
                searchForm.CARFAMILYCODE = dataForm.CARFAMILYCODE == null ? "" : dataForm.CARFAMILYCODE;
                searchForm.CURRENTPASTCODE = dataForm.CURRENTPASTCODE == null ? "" : dataForm.CURRENTPASTCODE;
                searchForm.varMakingOrderType = dataForm.varMakingOrderType == null ? "" : dataForm.varMakingOrderType;

                if (dataForm.iFZGC.Count == 0 || dataForm.iFZGC == null)
                {
                    searchForm.iFZGC = "";
                }
                else
                {
                    string temp = "";
                    for (int i = 0; i < dataForm.iFZGC.Count; i++)
                    {
                        if (i == dataForm.iFZGC.Count - 1)
                            temp += dataForm.iFZGC[i];
                        else
                            temp += dataForm.iFZGC[i] + ",";
                    }
                    searchForm.iFZGC = temp;
                }

                //searchForm.iFZGC = dataForm.iFZGC.Length == 0 ? "" :  dataForm.iFZGC;
                searchForm.INOUTFLAG = dataForm.INOUTFLAG == null ? "" : dataForm.INOUTFLAG;
                searchForm.SUPPLIERCODE = dataForm.SUPPLIERCODE == null ? "" : dataForm.SUPPLIERCODE;
                searchForm.iSupplierPlant = dataForm.iSupplierPlant == null ? null : Convert.ToInt32(dataForm.iSupplierPlant);

                int count = fs0602_Logic.Zk(searchForm);

                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UI0103", null, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = count;
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

        #region 回复内示（向销售公司回复内示）
        [HttpPost]
        [EnableCors("any")]
        public string hfApi([FromBody] dynamic data)
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

                FS0602_DataEntity searchForm = new FS0602_DataEntity();

                searchForm.varDxny = dataForm.varDxny == null ? "" : Convert.ToDateTime(dataForm.varDxny).ToString("yyyyMM");
                searchForm.varDyzt = dataForm.varDyzt == null ? "" : dataForm.varDyzt;
                searchForm.varHyzt = dataForm.varHyzt == null ? "" : dataForm.varHyzt;
                searchForm.PARTSNO = dataForm.PARTSNO == null ? "" : dataForm.PARTSNO;
                searchForm.CARFAMILYCODE = dataForm.CARFAMILYCODE == null ? "" : dataForm.CARFAMILYCODE;
                searchForm.CURRENTPASTCODE = dataForm.CURRENTPASTCODE == null ? "" : dataForm.CURRENTPASTCODE;
                searchForm.varMakingOrderType = dataForm.varMakingOrderType == null ? "" : dataForm.varMakingOrderType;

                if (dataForm.iFZGC.Count == 0 || dataForm.iFZGC == null)
                {
                    searchForm.iFZGC = "";
                }
                else
                {
                    string temp = "";
                    for (int i = 0; i < dataForm.iFZGC.Count; i++)
                    {
                        if (i == dataForm.iFZGC.Count - 1)
                            temp += dataForm.iFZGC[i];
                        else
                            temp += dataForm.iFZGC[i] + ",";
                    }
                    searchForm.iFZGC = temp;
                }

                searchForm.INOUTFLAG = dataForm.INOUTFLAG == null ? "" : dataForm.INOUTFLAG;
                searchForm.SUPPLIERCODE = dataForm.SUPPLIERCODE == null ? "" : dataForm.SUPPLIERCODE;
                searchForm.iSupplierPlant = dataForm.iSupplierPlant == null ? null : Convert.ToInt32(dataForm.iSupplierPlant);

                int count = fs0602_Logic.Hf(searchForm);

                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UI0103", null, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = count;
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

        #region 退回内示（删除对象年月内示，然后在soq履历表中改变状态为已退回）
        [HttpPost]
        [EnableCors("any")]
        public string thnsApi([FromBody] dynamic data)
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

                FS0602_DataEntity searchForm = new FS0602_DataEntity();

                searchForm.varDxny = dataForm.varDxny == null ? "" : Convert.ToDateTime(dataForm.varDxny).ToString("yyyyMM");
                
                int count = fs0602_Logic.thns(searchForm);

                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UI0103", null, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = count;
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
    }
}