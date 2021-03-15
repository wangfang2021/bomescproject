using System;
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



namespace SPPSApi.Controllers.G08
{
    [Route("api/FS0807/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0807Controller : BaseController
    {
        FS0807_Logic fs0807_Logic = new FS0807_Logic();
        private readonly string FunctionID = "FS0807";

        private readonly IWebHostEnvironment _webHostEnvironment;

        public FS0807Controller(IWebHostEnvironment webHostEnvironment)
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

                List<Object> dataList_C017 = ComFunction.convertAllToResult(ComFunction.getTCode("C017"));//工区
                List<Object> dataList_C018 = ComFunction.convertAllToResult(ComFunction.getTCode("C018"));//收货方
                List<Object> dataList_C023 = ComFunction.convertAllToResult(ComFunction.getTCode("C023"));//包装场
                res.Add("C017", dataList_C017);
                res.Add("C018", dataList_C018);
                res.Add("C023", dataList_C023);

                List<Object> dataList_Supplier = ComFunction.convertAllToResult(fs0807_Logic.getAllSupplier());//供应商
                res.Add("optionSupplier", dataList_Supplier);

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

            string vcGQ = dataForm.vcGQ;
            string vcSupplier_id = dataForm.vcSupplier_id;
            string vcSHF = dataForm.vcSHF;
            string vcPart_id = dataForm.vcPart_id;
            string vcCarType = dataForm.vcCarType;
            string vcTimeFrom = dataForm.dTimeFrom;
            string vcTimeTo = dataForm.dTimeTo;

            try
            {
                DataTable dt = fs0807_Logic.Search(vcGQ, vcSupplier_id, vcSHF, vcPart_id, vcCarType, vcTimeFrom, vcTimeTo);

                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("dTimeFrom", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dTimeTo", ConvertFieldType.DateType, "yyyy/MM/dd");

                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M08UE0702", ex, loginInfo.UserId);
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

            string vcGQ = dataForm.vcGQ;
            string vcSupplier_id = dataForm.vcSupplier_id;
            string vcSHF = dataForm.vcSHF;
            string vcPart_id = dataForm.vcPart_id;
            string vcCarType = dataForm.vcCarType;
            string vcTimeFrom = dataForm.dTimeFrom;
            string vcTimeTo = dataForm.dTimeTo;
            try
            {
                DataTable dt = fs0807_Logic.Search(vcGQ, vcSupplier_id, vcSHF, vcPart_id, vcCarType, vcTimeFrom, vcTimeTo);
                string[] heads = { "工区", "品番", "开始时间", "结束时间","车种","收货方","供应商","包装场","受入","背番号",
                    "收容数","品名(英文)","品名(中文)","自工程" ,"通过工程","前工程","前工程通过时间",
                    "自工程发货时间","照片","备注1","备注2"
                };
                string[] fields = { "vcGQ","vcPart_id","dTimeFrom","dTimeTo","vcCarType","vcSHF","vcSupplier_id","vcBZPlant","vcSR","vcKanBanNo",
                    "iContainerQuantity","vcPartNameEn","vcPartNameCn","vcInProcess","vcTGProcess","vcPreProcess","vcPreProcessPassTime",
                    "vcInProcessSendTime","vcPhotoPath","vcRemark1","vcRemark2"
                };
                string strMsg = "";
                string filepath = ComFunction.DataTableToExcel(heads, fields, dt, _webHostEnvironment.ContentRootPath, loginInfo.UserId, FunctionID, ref strMsg);
                if (strMsg != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = strMsg;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = filepath;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M08UE0703", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 保存
        [HttpPost]
        [EnableCors("any")]
        public string saveApi([FromBody]dynamic data)
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
                }
                if (!hasFind)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少有一个编辑行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //开始数据验证
                if (hasFind)
                {
                    #region 数据格式校验
                    string[,] strField = new string[,]
                    {
                        {"工区","品番","开始时间","结束时间","车种","收货方","供应商",
                         "包装场","受入","背番号","收容数","品名(英文)","品名(中文)","自工程",
                         "通过工程","前工程","前工程通过时间","自工程发货时间","备注1","备注2"},//中文字段名
                        {"vcGQ","vcPart_id","dTimeFrom","dTimeTo","vcCarType","vcSHF","vcSupplier_id",
                         "vcBZPlant","vcSR","vcKanBanNo","iContainerQuantity","vcPartNameEn","vcPartNameCn","vcInProcess",
                         "vcTGProcess","vcPreProcess","vcPreProcessPassTime","vcInProcessSendTime","vcRemark1","vcRemark2"},//英文字段名
                        {FieldCheck.NumChar,FieldCheck.NumChar,FieldCheck.Date,FieldCheck.Date,FieldCheck.NumChar,FieldCheck.NumChar, FieldCheck.NumChar,
                         FieldCheck.NumChar,FieldCheck.NumChar,FieldCheck.NumChar,FieldCheck.Num,"","",FieldCheck.Num,
                         "","","","","",""},//数据类型校验
                        {"20","12","0","0","4","5","4",
                         "2","2","5","0","20","20","1",
                         "10","5","2","2","25","25"},//最大长度设定,不校验最大长度用0
                        {"1","10","1","1","0","1","0",
                         "0","0","0","1","0","0","0",
                         "0","0","0","0","0","0"},//最小长度设定,可以为空用0
                        {"1","2","3","4","5","6","7",
                         "8","9","10","11","12","13","14",
                         "15","16","17","18","20","21"},//前台显示列号，从0开始计算,注意有选择框的是0
                    };
                    //需要判断时间区间先后关系的字段
                    string[,] strDateRegion = { { "dTimeFrom", "dTimeTo" } };

                    List<Object> checkRes = ListChecker.validateList(listInfoData, strField, strDateRegion, null, true, "FS0807");
                    if (checkRes != null)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = checkRes;
                        apiResult.flag = Convert.ToInt32(ERROR_FLAG.单元格定位提示);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    #endregion

                    #region 与DB交互校验
                    for (int i = 0; i < listInfoData.Count; i++)
                    {
                        bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                        bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                        string strPart_id = listInfoData[i]["vcPart_id"].ToString();
                        string strTimeFrom = listInfoData[i]["dTimeFrom"].ToString();
                        string strTimeTo = listInfoData[i]["dTimeTo"].ToString();
                        string strSHF = listInfoData[i]["vcSHF"].ToString();
                        if (bAddFlag == true)
                        {//新增 
                            //校验 品番+开始时间+收货方 不能重复
                            bool isRepeat = fs0807_Logic.RepeatCheck(strPart_id, strTimeFrom, strSHF);
                            if (isRepeat)
                            {//有重复数据  
                                apiResult.code = ComConstant.ERROR_CODE;
                                apiResult.data = string.Format("保存失败，有重复数据[品番-开始时间-收货方]：{0}-{1}-{2}",
                                strPart_id, strTimeFrom, strSHF);
                                apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                            }
                            //校验 品番日期区间不能重复
                            isRepeat = fs0807_Logic.DateRegionCheck(strPart_id,strSHF,strTimeFrom,strTimeTo,"new","");
                            if (isRepeat)
                            {//区间有重复  
                                apiResult.code = ComConstant.ERROR_CODE;
                                apiResult.data = string.Format("保存失败，以下品番使用开始、结束区间存在重叠：{0}",strPart_id);
                                apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                            }
                        }
                        else if (bAddFlag == false && bModFlag == true)
                        {//修改
                            //校验 品番日期区间不能重复
                            string strAutoId = listInfoData[i]["iAutoId"].ToString();
                            bool isRepeat = fs0807_Logic.DateRegionCheck(strPart_id, strSHF, strTimeFrom, strTimeTo, "mod", strAutoId);
                            if (isRepeat)
                            {//区间有重复  
                                apiResult.code = ComConstant.ERROR_CODE;
                                apiResult.data = string.Format("保存失败，以下品番使用开始、结束区间存在重叠：{0}", strPart_id);
                                apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                            }
                        }
                    }     
                    #endregion
                }
                fs0807_Logic.Save(listInfoData, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M08UE0704", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 删除
        [HttpPost]
        [EnableCors("any")]
        public string delApi([FromBody]dynamic data)
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
                fs0807_Logic.Del(checkedInfoData, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M08UE0705", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}
