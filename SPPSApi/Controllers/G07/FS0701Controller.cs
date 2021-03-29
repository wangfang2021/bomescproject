using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
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
    [Route("api/FS0701/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0701Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0701_Logic FS0701_Logic = new FS0701_Logic();
        private readonly string FunctionID = "FS0701";

        public FS0701Controller(IWebHostEnvironment webHostEnvironment)
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

                List<Object> dataList_Supplier = ComFunction.convertAllToResult(FS0701_Logic.SearchSupplier());//供应商
                res.Add("optionSupplier", dataList_Supplier);

                List<Object> ReleaseName = ComFunction.convertAllToResult(FS0701_Logic.SearchFZLJ());//发注逻辑
                res.Add("optionReleaseName", ReleaseName);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "FS0701", ex, loginInfo.UserId);
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

            List<Object> PackSpot = new List<object>();

            if (dataForm.PackSpot.ToObject<List<Object>>() == null)
            {
                PackSpot = new List<object>();
            }
            else
            {
                PackSpot = dataForm.PackSpot.ToObject<List<Object>>();
            }

            string PackNo = dataForm.PackNo;//包材品番
            string PackGPSNo = dataForm.PackGPSNo;//GPS品番

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


            string dFromB = dataForm.dtKaishiBegin;
            string dFromE = dataForm.dtKaishiEnd;
            string dToB = dataForm.dtJieshuBegin;
            string dToE = dataForm.dtJieshuEnd;

            try
            {
                DataTable dt = FS0701_Logic.Search(PackSpot, PackNo, PackGPSNo, strSupplierCode, dFromB, dFromE, dToB, dToE);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "FS0701", ex, loginInfo.UserId);
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


            List<Object> PackSpot = new List<object>();

            if (dataForm.PackSpot.ToObject<List<Object>>() == null)
            {
                PackSpot = new List<object>();
            }
            else
            {
                PackSpot = dataForm.PackSpot.ToObject<List<Object>>();
            }

            string PackNo = dataForm.PackNo;//包材品番
            string PackGPSNo = dataForm.PackGPSNo;//GPS品番

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


            string dFromB = dataForm.dtKaishiBegin;
            string dFromE = dataForm.dtKaishiEnd;
            string dToB = dataForm.dtJieshuBegin;
            string dToE = dataForm.dtJieshuEnd;


            try
            {
                DataTable dt = FS0701_Logic.Search(PackSpot, PackNo, PackGPSNo, strSupplierCode, dFromB, dFromE, dToB, dToE);
             


                if (dt.Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "没有可导出数据！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }
                string resMsg = "";
                string[] head = { "包装场", "包装材品番", "GPS品番", "开始时间,", "结束时间", "品名", "供应商", "供应商代码", "发注收容数", "资材订购批量", "循环", "段取区分", "场所", "规格", "发注逻辑" };
                string[] fields = { "vcPackSpot","vcPackNo","vcPackGPSNo","dPackFrom","dPackTo","vcParstName","vcSupplierName",
                    "vcSupplierCode","iRelease","iZCRelease","vcCycle","vcDistinguish","vcPackLocation","vcFormat","vcReleaseName"
                };
                string filepath = ComFunction.DataTableToExcel(head, fields, dt, _webHostEnvironment.ContentRootPath, loginInfo.UserId, "包材基础数据导出", ref resMsg);
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

        #region 导出导入模板
        [HttpPost]
        [EnableCors("any")]
        public string exportApi_import([FromBody] dynamic data)
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

            if (dataForm.PackSpot.ToObject<List<Object>>() == null)
            {
                PackSpot = new List<object>();
            }
            else
            {
                PackSpot = dataForm.PackSpot.ToObject<List<Object>>();
            }

            string PackNo = dataForm.PackNo;//包材品番
            string PackGPSNo = dataForm.PackGPSNo;//GPS品番

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


            string dFromB = dataForm.dtKaishiBegin;
            string dFromE = dataForm.dtKaishiEnd;
            string dToB = dataForm.dtJieshuBegin;
            string dToE = dataForm.dtJieshuEnd;


            try
            {
                DataTable dt = FS0701_Logic.Search(PackSpot, PackNo, PackGPSNo, strSupplierCode, dFromB, dFromE, dToB, dToE);

            

                string resMsg = "";
                string[] head = { "是否修改","指定标识", "包装场", "包装材品番", "GPS品番", "开始时间", "结束时间", "品名", "供应商", "供应商代码", "发注收容数", "资材订购批量", "循环", "段取区分", "场所", "规格", "发注逻辑" };
                string[] fields = {"vcIsorNo", "iAutoId","vcPackSpot","vcPackNo","vcPackGPSNo","dPackFrom","dPackTo","vcParstName","vcSupplierName",
                    "vcSupplierCode","iRelease","iZCRelease","vcCycle","vcDistinguish","vcPackLocation","vcFormat","vcReleaseName"
                };
                string filepath = ComFunction.DataTableToExcel(head, fields, dt, _webHostEnvironment.ContentRootPath, loginInfo.UserId, "包材基础数据导出", ref resMsg);
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
                   
                    int iAutoId = listInfoData[i]["iAutoId"]==""?0: Convert.ToInt32(listInfoData[i]["iAutoId"]);
                    DataTable dtcheckTime = FS0701_Logic.searchcheckTime(listInfoData[i]["vcPackSpot"].ToString(),listInfoData[i]["vcPackNo"].ToString(), listInfoData[i]["dPackFrom"].ToString().Split(' ')[0], listInfoData[i]["dPackTo"].ToString().Split(' ')[0], iAutoId);
                    if (dtcheckTime.Rows.Count > 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "品番有维护重复有效时间！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }


                    if (listInfoData[i]["vcPackLocation"].ToString().Length > 20)
                    {

                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "超过20位！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    Regex regex = new System.Text.RegularExpressions.Regex("^(-?[0-9]*[.]*[0-9]{0,3})$");
                    bool b = regex.IsMatch(listInfoData[i]["iRelease"].ToString());
                    if (!b)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "请填写正常的发住收容数格式！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (Convert.ToInt32(listInfoData[i]["iRelease"]) % Convert.ToInt32(listInfoData[i]["iZCRelease"]) != 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "收容数不是订购批量的整数倍！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }

                }
                if (!hasFind)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少有一个编辑行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                string strErrorPartId = "";
                FS0701_Logic.Save(listInfoData, loginInfo.UserId, ref strErrorPartId);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "FS0701", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion


        #region 更新
        [HttpPost]
        [EnableCors("any")]
        public string updateApi([FromBody] dynamic data)
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
                BatchProcess.FP0005 fp0005 = new BatchProcess.FP0005();


                if (!fp0005.main(loginInfo.UserId))
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "更新失败";
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
                apiResult.data = "更新失败";
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
                FS0701_Logic.Del(listInfoData, loginInfo.UserId);
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
