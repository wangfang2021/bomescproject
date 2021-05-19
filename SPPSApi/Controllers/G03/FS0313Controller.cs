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

namespace SPPSApi.Controllers.G03
{
    [Route("api/FS0313/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0313Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0313_Logic fs0313_Logic = new FS0313_Logic();
        private readonly string FunctionID = "FS0313";

        public FS0313Controller(IWebHostEnvironment webHostEnvironment)
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

                List<Object> dataList_C002 = ComFunction.convertAllToResult(ComFunction.getTCode("C002"));//变更事项
                Dictionary<string, object> row = new Dictionary<string, object>();
                row["vcName"] = "空";
                row["vcValue"] = "空";
                dataList_C002.Add(row);
                Dictionary<string, object> row2 = new Dictionary<string, object>();
                row2["vcName"] = "处理中";
                row2["vcValue"] = "处理中";
                dataList_C002.Add(row2);

                List<Object> dataList_C004 = ComFunction.convertAllToResult(ComFunction.getTCode("C004"));//号旧区分
                List<Object> dataList_C003 = ComFunction.convertAllToResult(ComFunction.getTCode("C003"));//内外区分
                List<Object> dataList_C012 = ComFunction.convertAllToResult(ComFunction.getTCode("C012"));//OE=SP
                List<Object> dataList_C013 = ComFunction.convertAllToResult(ComFunction.getTCode("C013"));//状态
                for(int i=0;i< dataList_C013.Count; i++)
                {
                    Dictionary<string, object> item = (Dictionary<string, object>)dataList_C013[i];
                    if (item["vcName"].ToString() == "待处理")
                    {
                        dataList_C013.RemoveAt(i);
                        break;
                    }
                }


                List<Object> dataList_C006 = ComFunction.convertAllToResult(ComFunction.getTCode("C006"));//原单位
                List<Object> dataList_C028 = ComFunction.convertAllToResult(ComFunction.getTCode("C028"));//防锈
                List<Object> dataList_C038 = ComFunction.convertAllToResult(ComFunction.getTCode("C038"));//公式名

                Dictionary<string, object> row_All = new Dictionary<string, object>();//公式增加请选择
                row_All["vcName"] = "请选择";
                row_All["vcValue"] = "";
                dataList_C038.Insert(0,row_All);

                //设变履历是否下拉待确定
                List<Object> dataList_C005 = ComFunction.convertAllToResult(ComFunction.getTCode("C005"));//收货方

                int iTask = fs0313_Logic.getAllTask();

                res.Add("C002", dataList_C002);
                res.Add("C004", dataList_C004);
                res.Add("C003", dataList_C003);
                res.Add("C012", dataList_C012);
                res.Add("C013", dataList_C013);
                res.Add("C005", dataList_C005);
                res.Add("C006", dataList_C006);
                res.Add("C028", dataList_C028);
                res.Add("C038", dataList_C038);
                res.Add("taskNum", iTask);

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

            string strChange = dataForm.Change;
            string strPart_id = dataForm.Part_id;
            string strOriginCompany = dataForm.OriginCompany;
            string strHaoJiu = dataForm.HaoJiu;

            string strPriceChangeInfo = dataForm.PriceChangeInfo;
            string strCarTypeDev = dataForm.CarTypeDev;
            string strSupplier_id = dataForm.Supplier_id;
            string strReceiver = dataForm.Receiver;
            string strPriceState = dataForm.PriceState;
            string strMaxNum = dataForm.iMaxNum;

            try
            {
                DataTable dt = fs0313_Logic.Search(strMaxNum,strChange, strPart_id, strOriginCompany, strHaoJiu
                    , strPriceChangeInfo, strCarTypeDev, strSupplier_id
                    , strReceiver, strPriceState
                    );
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

            string strChange = dataForm.Change;
            string strPart_id = dataForm.Part_id;
            string strOriginCompany = dataForm.OriginCompany;
            string strHaoJiu = dataForm.HaoJiu;
            string strPriceChangeInfo = dataForm.PriceChangeInfo;
            string strCarTypeDev = dataForm.CarTypeDev;
            string strSupplier_id = dataForm.Supplier_id;
            string strReceiver = dataForm.Receiver;
            string strPriceState = dataForm.PriceState;
            try
            {
                DataTable dt = fs0313_Logic.Search("",strChange, strPart_id, strOriginCompany, strHaoJiu
                   , strPriceChangeInfo, strCarTypeDev, strSupplier_id
                   , strReceiver, strPriceState
                   );
                string[] fields = { "iAutoId","vcChange_Name", "vcPart_id", "dUseBeginStr", "dUseEndStr", "vcProjectType_Name", "vcSupplier_id"
                ,"vcSupplier_Name","dProjectBeginStr","dProjectEndStr","vcHaoJiu_Name","dJiuBeginStr","dJiuEndStr","dJiuBeginSustainStr","vcPriceChangeInfo_Name"
                ,"vcPriceState_Name","dPriceStateDateStr","vcPriceGS_Name","decPriceOrigin_CW","decPriceAfter","decPriceTNPWithTax","dPricebeginStr","dPriceEndStr"
                ,"vcCarTypeDev","vcCarTypeDesign","vcPart_Name","vcOE_Name","vcPart_id_HK","vcStateFX","vcFXNO","vcSumLater","vcReceiver_Name"
                ,"vcOriginCompany_Name"
                };
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0309_Export.xlsx", 1,loginInfo.UserId,FunctionID  );
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE1304", ex, loginInfo.UserId);
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
                    string[,] strField = new string[,] {{"原价"},
                                                {"decPriceOrigin_CW"},
                                                {FieldCheck.Decimal},
                                                {"0"},//最大长度设定,不校验最大长度用0
                                                {"1"},//最小长度设定,可以为空用0
                                                {"18"}//前台显示列号，从0开始计算,注意有选择框的是0
                    };
                    List<Object> checkRes = ListChecker.validateList(listInfoData, strField, null, null,true, "FS0313");
                    if (checkRes != null)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = checkRes;
                        apiResult.flag = Convert.ToInt32(ERROR_FLAG.单元格定位提示);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    #region 验证品番状态是否可操作
                    for (int i = 0; i < listInfoData.Count; i++)
                    {
                        string strPart_id = listInfoData[i]["vcPart_id"].ToString();
                        string strPriceState = listInfoData[i]["vcPriceState"].ToString();
                        if (strPriceState.ToString()!= "1")
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "品番"+ strPart_id+ "状态不是“已送信”，不能再编辑！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                    }
                    #endregion
                }

                fs0313_Logic.SaveCaiWu(listInfoData, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE1302", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 财务回复
        [HttpPost]
        [EnableCors("any")]
        public string OKCaiWuApi([FromBody]dynamic data)
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
                if (listInfoData == null || listInfoData.Count <= 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "至少选择一条数据！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string[,] strField = new string[,] {{"原价"},
                                                {"decPriceOrigin_CW"},
                                                {FieldCheck.Decimal},
                                                {"0"},//最大长度设定,不校验最大长度用0
                                                {"1"},//最小长度设定,可以为空用0
                                                {"18"}//前台显示列号，从0开始计算,注意有选择框的是0
                    };
                List<Object> checkRes = ListChecker.validateList(listInfoData, strField, null, null, true, "FS0313");
                if (checkRes != null)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = checkRes;
                    apiResult.flag = Convert.ToInt32(ERROR_FLAG.单元格定位提示);
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                #region 验证品番状态是否可操作
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    string strPart_id = listInfoData[i]["vcPart_id"].ToString();
                    string strPriceState = listInfoData[i]["vcPriceState"].ToString();
                    if (strPriceState.ToString() != "1")
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "品番" + strPart_id + "状态不是“已送信”，不能再回复！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                #endregion

                fs0313_Logic.OKCaiWu(listInfoData, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE1303", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "回复失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion



        #region 获取待办任务
        [HttpPost]
        [EnableCors("any")]
        public string taskApi()
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
                int iTask = fs0313_Logic.getAllTask();
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = iTask;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE1310", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "获取待办失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

      
    }
}
