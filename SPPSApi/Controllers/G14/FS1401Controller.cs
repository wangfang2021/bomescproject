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

namespace SPPSApi.Controllers.G14
{
    [Route("api/FS1401/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1401Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        FS1401_Logic fs1401_Logic = new FS1401_Logic();
        private readonly string FunctionID = "FS1401";

        public FS1401Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        /// <summary>
        /// 初始化方法
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string pageload_api()
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
                DataSet dataSet = fs1401_Logic.getDllOptionsList();
                List<Object> dataList_HJOptions = ComFunction.convertAllToResult(dataSet.Tables[0]);
                List<Object> dataList_InOutOptions = ComFunction.convertAllToResult(dataSet.Tables[1]);
                List<Object> dataList_PartAreaOptions = ComFunction.convertAllToResult(dataSet.Tables[2]);
                List<Object> dataList_SPISqufenOptions = ComFunction.convertAllToResult(dataSet.Tables[3]);
                List<Object> dataList_CheckPOptions = ComFunction.convertAllToResult(dataSet.Tables[4]);
                List<Object> dataList_SPISInPutOptions = ComFunction.convertAllToResult(dataSet.Tables[5]);

                res.Add("HJOptions", dataList_HJOptions);
                res.Add("InOutOptions", dataList_InOutOptions);
                res.Add("PartAreaOptions", dataList_PartAreaOptions);
                res.Add("SPISqufenOptions", dataList_SPISqufenOptions);
                res.Add("CheckPOptions", dataList_CheckPOptions);
                res.Add("SPISInPutOptions", dataList_SPISInPutOptions);
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
        /// <summary>
        /// 检索方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string search_api([FromBody] dynamic data)
        {
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            //以下开始业务处理
            ApiResult apiResult = new ApiResult();
            dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));//接收参数

            string strPartNo = dataForm.sPartNo;//textbox取值
            string strSuplier = dataForm.sSuplier;
            string strHJ = dataForm.HJOptions;//dropdownlist取值
            string strInOut = dataForm.InOutOptions;
            string strPartArea = dataForm.PartAreaOptions;
            string strSPISqufen = dataForm.SPISqufenOptions;
            string strCheckP = dataForm.CheckPOptions;
            string strTimeFrom = dataForm.sTimeFrom;
            string strTimeTo = dataForm.sTimeTo;
            string strCarFamily = dataForm.sCarFamily;
            string strSPISInPut = dataForm.SPISInPutOptions;
            string strcboxnow = dataForm.SPISInPutOptions;//checkbox取值
            string strcboxtom = dataForm.SPISInPutOptions;
            string strcboxyes = dataForm.SPISInPutOptions;

            try
            {
                DataTable dt = fs1401_Logic.getSearchInfo(strPartNo, strSuplier, strHJ, strInOut, strPartArea, strSPISqufen, strCheckP,
                    strTimeFrom, strTimeTo, strCarFamily, strSPISInPut, 
                    strcboxnow, strcboxtom, strcboxyes);

                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("dTimeFrom", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dTimeTo", ConvertFieldType.DateType, "yyyy/MM/dd");

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

        /// <summary>
        /// 导出方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string export_api([FromBody] dynamic data)
        {
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            //以下开始业务处理
            ApiResult apiResult = new ApiResult();
            dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));//接收参数

            string strPartNo = dataForm.sPartNo;//textbox取值
            string strSuplier = dataForm.sSuplier;
            string strHJ = dataForm.HJOptions;//dropdownlist取值
            string strInOut = dataForm.InOutOptions;
            string strPartArea = dataForm.PartAreaOptions;
            string strSPISqufen = dataForm.SPISqufenOptions;
            string strCheckP = dataForm.CheckPOptions;
            string strTimeFrom = dataForm.sTimeFrom;
            string strTimeTo = dataForm.sTimeTo;
            string strCarFamily = dataForm.sCarFamily;
            string strSPISInPut = dataForm.SPISInPutOptions;
            string strcboxnow = dataForm.SPISInPutOptions;//checkbox取值
            string strcboxtom = dataForm.SPISInPutOptions;
            string strcboxyes = dataForm.SPISInPutOptions;
            try
            {
                DataTable dt = fs1401_Logic.getSearchInfo(strPartNo, strSuplier, strHJ, strInOut, strPartArea, strSPISqufen, strCheckP,
                    strTimeFrom, strTimeTo, strCarFamily, strSPISInPut,
                    strcboxnow, strcboxtom, strcboxyes);
                string[] fields = { "dPart_No", "dPartsNameEn", "dTimeFrom", "dTimeTo", "dSupplierCode", "dPartArea",
                    "dCarFamilyCode","dSupplierArea","dPicUrl","dCheckQF","dInOut","dHJ","dPACKINGFLAG","dFlag"};
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS1401_Export.xlsx", 2, loginInfo.UserId, FunctionID);
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
        /// <summary>
        /// 保存方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string save_api([FromBody]dynamic data)
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
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();//复选框选择

                //bool hasFind = false;//是否找到需要新增或者修改的数据
                //for (int i = 0; i < listInfoData.Count; i++)
                //{
                //    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                //    bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                //    if (bAddFlag == true)
                //    {//新增
                //        hasFind = true;
                //    }
                //    else if (bAddFlag == false && bModFlag == true)
                //    {//修改
                //        hasFind = true;
                //    }
                //}
                //if (!hasFind)
                //{
                //    apiResult.code = ComConstant.ERROR_CODE;
                //    apiResult.data = "最少有一个编辑行！";
                //    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                //}
                ////开始数据验证
                //if (hasFind)
                //{
                //    string[,] strField = new string[,] {{"变更事项","品番","使用开始","使用结束","内外","供应商代码","供应商名称","开始","结束","号旧"},
                //                                {"vcChange","vcPart_id","dUseBegin","dUseEnd","vcProjectType","vcSupplier_id","vcSupplier_Name","dProjectBegin","dProjectEnd","vcHaoJiu"},
                //                                {"",FieldCheck.NumChar,FieldCheck.Date,FieldCheck.Date,"","","",FieldCheck.Date,FieldCheck.Date,"" },
                //                                {"25","12","0","0","0","4","50","0","0","0"},//最大长度设定,不校验最大长度用0
                //                                {"1","10","1","1","1","1","1","1","1","1"},//最小长度设定,可以为空用0
                //                                {"1","2","3","4","5","6","7","8","9","10"}//前台显示列号，从0开始计算,注意有选择框的是0
                //    };
                //    //需要判断时间区间先后关系的字段
                //    string[,] strDateRegion = { { "dUseBegin", "dUseEnd" }, { "dProjectBegin", "dProjectEnd" }, { "dJiuBegin", "dJiuEnd" }, { "dPricebegin", "dPriceEnd" } };
                //    string[,] strSpecialCheck = {
                //        //例子-变更事项字段，当它为新设时，号旧必须为号口，旧型开始、旧型结束、旧型持续开始必须为空
                //        //vcChange=1时，vcHaoJiu如果为1，如果内容列不为空(H)，则内容必须为H，如果内容为空，则对具体内容不做判断
                //        { "变更事项",
                //            "vcChange",//验证vcChange字段
                //            "新设"
                //            ,"1",//当vcChange=1时
                //            "号旧",
                //            "vcHaoJiu",//判断字段
                //            "1", //1:该字段不能为空 0:该字段必须为空
                //            "号口",
                //            "H" //该字段有值且验证标记为“1”，则vcHaoJiu必须等于H，该字段为空且验证标记为“1”,则该字段值填什么都行
                //        },
                //        { "变更事项","vcChange", "旧型","3", "号旧","vcHaoJiu","1", "旧型", "Q" },
                //        { "变更事项","vcChange", "新设","1", "旧型开始","dJiuBegin","0", "空","" },
                //        { "变更事项","vcChange", "新设","1", "旧型结束","dJiuEnd","0", "空","" },
                //        { "变更事项","vcChange", "新设","1", "旧型持续开始","dJiuBeginSustain","0", "空","" },
                //        { "变更事项","vcChange", "旧型","3", "旧型开始","dJiuBegin","1", "","" },
                //        { "变更事项","vcChange", "旧型","3", "旧型结束","dJiuEnd","1", "","" },
                //        { "变更事项","vcChange", "旧型","3", "旧型持续开始","dJiuBeginSustain","1", "","" }
                //    };



                //    List<Object> checkRes = ListChecker.validateList(listInfoData, strField, strDateRegion, strSpecialCheck, true, "FS0309");
                //    if (checkRes != null)
                //    {
                //        apiResult.code = ComConstant.ERROR_CODE;
                //        apiResult.data = checkRes;
                //        apiResult.flag = Convert.ToInt32(ERROR_FLAG.单元格定位提示);
                //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                //    }
                //}

                //string strErrorPartId = "";
                //fs0309_Logic.Save(listInfoData, loginInfo.UserId, ref strErrorPartId);
                //if (strErrorPartId != "")
                //{
                //    apiResult.code = ComConstant.ERROR_CODE;
                //    apiResult.data = "保存失败，以下品番使用开始、结束区间存在重叠：<br/>" + strErrorPartId;
                //    apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                //    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                //}
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
    }
}
