using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
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
    [Route("api/FS0704/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0704Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0704_Logic FS0704_Logic = new FS0704_Logic();
        private readonly string FunctionID = "FS0704";

        public FS0704Controller(IWebHostEnvironment webHostEnvironment)
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



                List<Object> dataList_C023 = ComFunction.convertAllToResult(ComFunction.getTCode("C023"));//包装场

                res.Add("C023", dataList_C023);

                List<Object> dataList_C042 = ComFunction.convertAllToResult(ComFunction.getTCode("C042"));//发注逻辑

                res.Add("C042", dataList_C042);

                List<Object> dataList_Supplier = ComFunction.convertAllToResult(FS0704_Logic.SearchSupplier());//供应商
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
            //以下开始业务处理
            ApiResult apiResult = new ApiResult();
            dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));

            string PackSpot = dataForm.PackSpot;
            string FaZhu = dataForm.FaZhu;
            string dFromB = dataForm.dFrom;
            string dFromE = dataForm.dFrom;
            string dToB = dataForm.dToB;
            string dToE = dataForm.dToE;

            try
            {
                DataTable dt = FS0704_Logic.Search(PackSpot, FaZhu, dFromB, dFromE, dToB, dToE);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("dUseBegin", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dUseEnd", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dProjectBegin", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dProjectEnd", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dJiuBegin", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dJiuEnd", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dJiuBeginSustain", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dPriceStateDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dPricebegin", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dPriceEnd", ConvertFieldType.DateType, "yyyy/MM/dd");
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


            string PackSpot = dataForm.PackSpot;
            string FaZhu = dataForm.PackNo;
            string dFromB = dataForm.dFrom;
            string dFromE = dataForm.dFrom;
            string dToB = dataForm.dToB;
            string dToE = dataForm.dToE;

            try
            {
                DataTable dt = FS0704_Logic.Search(PackSpot, FaZhu, dFromB, dFromE, dToB, dToE);
                string[] fields = { "vcFaZhuID","vcRuHeFromDay","dRuHeFromTime","vcRuHeToDay","druHeToTime","vcFaZhuFromDay","dFaZhuFromTime","vcFaZhuToDay","dFaZhuToTime","vcNaQiFromDay",
                    "dNaQiFromTime","vcNaQiToDay","dNaQiToTime","vcBianCi","vcPackSpot","dFrom","dTo"
                };
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0704_Export.xlsx", 3, loginInfo.UserId, FunctionID);
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
        //导出列值应该显示名字

        #region 保存
        [HttpPost]
        [EnableCors("any")]
        public string saveApi([FromBody] dynamic data)
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
                JArray listInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
                bool hasFind = false;//是否找到需要新增或者修改的数据
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    if (bAddFlag == true)
                    {//新增
                        hasFind = true;
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        hasFind = true;
                    }

                    //判断对应逻辑下是否有重叠时间
                    #region 判断对应逻辑下是否有重叠时间
                    DataTable dtLJtime = FS0704_Logic.SearchLJTime(listInfoData[i]["vcFaZhu"].ToString(), listInfoData[i]["iAutoId"].ToString());
                    DateTime dRuHeFromDay = DateTime.ParseExact(listInfoData[i]["dRuHeFromTime"].ToString(), "HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                    DateTime dRuHeToDay = DateTime.ParseExact(listInfoData[i]["druHeToTime"].ToString(), "HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                    DateTime dFaZhuFromDay = DateTime.ParseExact(listInfoData[i]["dFaZhuFromTime"].ToString(), "HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                    DateTime dFaZhuToDay = DateTime.ParseExact(listInfoData[i]["dFaZhuToTime"].ToString(), "HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                    DateTime dNaQiFromDay = DateTime.ParseExact(listInfoData[i]["dNaQiFromTime"].ToString(), "HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                    DateTime dNaQiToDay = DateTime.ParseExact(listInfoData[i]["dNaQiToTime"].ToString(), "HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                   
                    for (int j = 0; j < dtLJtime.Rows.Count; j++)
                    {
                        
                        DateTime dRuHeFromDay1 = DateTime.ParseExact(dtLJtime.Rows[j]["dRuHeFromTime"].ToString(), "HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                        DateTime dRuHeToDay1 = DateTime.ParseExact(dtLJtime.Rows[j]["druHeToTime"].ToString(), "HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                        DateTime dFaZhuFromDay1 = DateTime.ParseExact(dtLJtime.Rows[j]["dFaZhuFromTime"].ToString(), "HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                        DateTime dFaZhuToDay1 = DateTime.ParseExact(dtLJtime.Rows[j]["dFaZhuToTime"].ToString(), "HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                        DateTime dNaQiFromDay1 = DateTime.ParseExact(dtLJtime.Rows[j]["dNaQiFromTime"].ToString(), "HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                        DateTime dNaQiToDay1 = DateTime.ParseExact(dtLJtime.Rows[j]["dNaQiToTime"].ToString(), "HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                      

                        if (!(dRuHeFromDay < dRuHeFromDay1 && dRuHeToDay < dRuHeFromDay1) || !(dRuHeFromDay > dRuHeToDay1 && dRuHeToDay > dRuHeToDay1))
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = listInfoData[i]["vcFaZhu"].ToString()+"部品入荷时间有重叠";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        if (!(dFaZhuFromDay < dFaZhuFromDay1 && dFaZhuToDay < dFaZhuFromDay1) || !(dFaZhuFromDay > dFaZhuToDay1 && dFaZhuToDay > dFaZhuToDay1))
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = listInfoData[i]["vcFaZhu"].ToString() + "发注作业时间有重叠";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        if (!(dNaQiFromDay < dNaQiFromDay1 && dNaQiToDay < dNaQiFromDay1) || !(dNaQiFromDay > dNaQiToDay1 && dNaQiToDay > dNaQiToDay1))
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = listInfoData[i]["vcFaZhu"].ToString() + "部品入荷时间有重叠";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                    }
                    #endregion

                }
                if (!hasFind)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少有一个编辑行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                string strErrorPartId = "";
                FS0704_Logic.Save(listInfoData, loginInfo.UserId, ref strErrorPartId);
                if (strErrorPartId != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "保存失败";
                    apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0902", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 删除
        [HttpPost]
        [EnableCors("any")]
        public string delApi([FromBody] dynamic data)
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
                List<Dictionary<string, Object>> listInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();
                if (listInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选择一条数据！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                FS0704_Logic.Del(listInfoData, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0903", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion


    }
}
