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
    [Route("api/FS0309/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0309Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0309_Logic fs0309_Logic = new FS0309_Logic();
        private readonly string FunctionID = "FS0309";

        public FS0309Controller(IWebHostEnvironment webHostEnvironment)
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
                if (loginInfo.Special == "财务用户")
                    res.Add("caiWuBtnVisible", false);
                else
                    res.Add("caiWuBtnVisible", true);

                List<Object> dataList_C002 = ComFunction.convertAllToResult(ComFunction.getTCode("C002"));//变更事项
                List<Object> dataList_C004 = ComFunction.convertAllToResult(ComFunction.getTCode("C004"));//号旧区分
                List<Object> dataList_C003 = ComFunction.convertAllToResult(ComFunction.getTCode("C003"));//内外区分
                List<Object> dataList_C012 = ComFunction.convertAllToResult(ComFunction.getTCode("C012"));//OE=SP
                List<Object> dataList_C013 = ComFunction.convertAllToResult(ComFunction.getTCode("C013"));//状态
                List<Object> dataList_C006 = ComFunction.convertAllToResult(ComFunction.getTCode("C006"));//原单位
                List<Object> dataList_C038 = ComFunction.convertAllToResult(ComFunction.getTCode("C038"));//公式名
                //设变履历是否下拉待确定
                List<Object> dataList_C005 = ComFunction.convertAllToResult(ComFunction.getTCode("C005"));//收货方

                DataTable task=fs0309_Logic.getAllTask();

                res.Add("C002", dataList_C002);
                res.Add("C004", dataList_C004);
                res.Add("C003", dataList_C003);
                res.Add("C012", dataList_C012);
                res.Add("C013", dataList_C013);
                res.Add("C005", dataList_C005);
                res.Add("C006", dataList_C006);
                res.Add("C038", dataList_C038);
                res.Add("taskNum", task.Rows.Count);

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
            string strProjectType = dataForm.ProjectType;
            if (loginInfo.Special == "财务用户")
                strProjectType = "内制";

            string strPriceChangeInfo = dataForm.PriceChangeInfo;
            string strCarTypeDev = dataForm.CarTypeDev;
            string strSupplier_id = dataForm.Supplier_id;
            string strReceiver = dataForm.Receiver;
            string strPriceState = dataForm.PriceState;

            try
            {
                DataTable dt = fs0309_Logic.Search(strChange, strPart_id, strOriginCompany, strHaoJiu
                    , strProjectType, strPriceChangeInfo, strCarTypeDev, strSupplier_id
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
            string strProjectType = dataForm.ProjectType;
            string strPriceChangeInfo = dataForm.PriceChangeInfo;
            string strCarTypeDev = dataForm.CarTypeDev;
            string strSupplier_id = dataForm.Supplier_id;
            string strReceiver = dataForm.Receiver;
            string strPriceState = dataForm.PriceState;
            try
            {
                DataTable dt = fs0309_Logic.Search(strChange, strPart_id, strOriginCompany, strHaoJiu
                   , strProjectType, strPriceChangeInfo, strCarTypeDev, strSupplier_id
                   , strReceiver, strPriceState
                   );
                string[] fields = { "vcChange_Name", "vcPart_id", "dUseBegin", "dUseEnd", "vcProjectType_Name", "vcSupplier_id"
                ,"vcSupplier_Name","dProjectBegin","dProjectEnd","vcHaoJiu_Name","dJiuBegin","dJiuEnd","dJiuBeginSustain","vcPriceChangeInfo"
                ,"vcPriceState_Name","dPriceStateDate","vcPriceGS","decPriceOrigin","decPriceAfter","decPriceTNPWithTax","dPricebegin","dPriceEnd"
                ,"vcCarTypeDev","vcCarTypeDesign","vcPart_Name","vcOE_Name","vcPart_id_HK","vcStateFX","vcFXNO","vcSumLater","vcReceiver_Name"
                ,"vcOriginCompany_Name"
                };
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0309_Export.xlsx", 2,loginInfo.UserId,FunctionID  );
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
                    string[,] strField = new string[,] {{"变更事项","品番","使用开始","使用结束","内外","供应商代码","供应商名称","开始","结束","号旧"},
                                                {"vcChange","vcPart_id","dUseBegin","dUseEnd","vcProjectType","vcSupplier_id","vcSupplier_Name","dProjectBegin","dProjectEnd","vcHaoJiu"},
                                                {"",FieldCheck.NumChar,FieldCheck.Date,FieldCheck.Date,"","","",FieldCheck.Date,FieldCheck.Date,"" },
                                                {"25","12","0","0","0","4","50","0","0","0"},//最大长度设定,不校验最大长度用0
                                                {"1","10","1","1","1","1","1","1","1","1"},//最小长度设定,可以为空用0
                                                {"1","2","3","4","5","6","7","8","9","10"}//前台显示列号，从0开始计算,注意有选择框的是0
                    };
                    //需要判断时间区间先后关系的字段
                    string[,] strDateRegion = { { "dUseBegin", "dUseEnd" }, { "dProjectBegin", "dProjectEnd" }, { "dJiuBegin", "dJiuEnd" }, { "dPricebegin", "dPriceEnd" } };
                    string[,] strSpecialCheck = {
                        //例子-变更事项字段，当它为新设时，号旧必须为号口，旧型开始、旧型结束、旧型持续开始必须为空
                        //vcChange=1时，vcHaoJiu如果为1，如果内容列不为空(H)，则内容必须为H，如果内容为空，则对具体内容不做判断
                        { "变更事项",
                            "vcChange",//验证vcChange字段
                            "新设"
                            ,"1",//当vcChange=1时
                            "号旧",
                            "vcHaoJiu",//判断字段
                            "1", //1:该字段不能为空 0:该字段必须为空
                            "号口",
                            "H" //该字段有值且验证标记为“1”，则vcHaoJiu必须等于H，该字段为空且验证标记为“1”,则该字段值填什么都行
                        },
                        { "变更事项","vcChange", "旧型","3", "号旧","vcHaoJiu","1", "旧型", "Q" },
                        { "变更事项","vcChange", "新设","1", "旧型开始","dJiuBegin","0", "空","" },
                        { "变更事项","vcChange", "新设","1", "旧型结束","dJiuEnd","0", "空","" },
                        { "变更事项","vcChange", "新设","1", "旧型持续开始","dJiuBeginSustain","0", "空","" },
                        { "变更事项","vcChange", "旧型","3", "旧型开始","dJiuBegin","1", "","" },
                        { "变更事项","vcChange", "旧型","3", "旧型结束","dJiuEnd","1", "","" },
                        { "变更事项","vcChange", "旧型","3", "旧型持续开始","dJiuBeginSustain","1", "","" }
                    };



                    List<Object> checkRes = ListChecker.validateList(listInfoData, strField, strDateRegion, strSpecialCheck,true, "FS0309");
                    if (checkRes != null)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = checkRes;
                        apiResult.flag = Convert.ToInt32(ERROR_FLAG.单元格定位提示);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }

                string strErrorPartId = "";
                fs0309_Logic.Save(listInfoData, loginInfo.UserId,ref strErrorPartId);
                if (strErrorPartId != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "保存失败，以下品番使用开始、结束区间存在重叠：<br/>"+ strErrorPartId;
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
                List<Dictionary<string, Object>> listInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();
                if (listInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选择一条数据！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fs0309_Logic.Del(listInfoData, loginInfo.UserId);
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
                DataTable task = fs0309_Logic.getAllTask();
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = task.Rows.Count;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0910", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "获取待办失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 销售展开
        [HttpPost]
        [EnableCors("any")]
        public string sendMailApi([FromBody]dynamic data)
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
                Object multipleSelection=dataForm.multipleSelection;
                if (multipleSelection == null)//如果没有选中数据，那么就是按检索条件发送
                {
                    string strChange = dataForm.Change;
                    string strPart_id = dataForm.Part_id;
                    string strOriginCompany = dataForm.OriginCompany;
                    string strHaoJiu = dataForm.HaoJiu;
                    string strProjectType = dataForm.ProjectType;
                    if (loginInfo.Special == "财务用户")
                        strProjectType = "内制";
                    string strPriceChangeInfo = dataForm.PriceChangeInfo;
                    string strCarTypeDev = dataForm.CarTypeDev;
                    string strSupplier_id = dataForm.Supplier_id;
                    string strReceiver = dataForm.Receiver;
                    string strPriceState = dataForm.PriceState;
                    string strErr = "";
                    var temp = fs0309_Logic.sendMail(strChange, strPart_id, strOriginCompany, strHaoJiu, strProjectType, strPriceChangeInfo, strCarTypeDev, strSupplier_id, strReceiver, strPriceState, ref strErr);
                    if (strErr!="")
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = strErr;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                else
                {
                    JArray checkedInfo = dataForm.multipleSelection;
                    List<Dictionary<string, Object>> listInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();

                    string strErr = "";
                    string strAutoId = dataForm.iAutoId;
                    fs0309_Logic.sendMail(listInfoData,ref strErr);
                    if (strErr!="")
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = strErr;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                //发送邮件
                //发件人邮箱，对方邮箱，邮件标题、内容、附件需要确认
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0906", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "销售展开失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 10万测试
        [HttpPost]
        [EnableCors("any")]
        public string test10WApi()
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
                DataTable dt = fs0309_Logic.test10W();
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("selected", ConvertFieldType.BoolType, null);


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

        #region 送信-调达
        [HttpPost]
        [EnableCors("any")]
        public string sendDiaoDaApi([FromBody] dynamic data)
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
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                Object multipleSelection = dataForm.multipleSelection;

                //切替预定日计算逻辑
                //新设：使用开始时间
                //旧型：旧型开始
                //废止：使用结束时间
                //恢复现号：旧型结束+1天
                //复活：使用开始时间
                //持续生产：旧型持续开始
                //一括生产：使用结束时间
                //工程变更新设：工程/供应商信息下的开始时间
                //工程变更废止：工程/供应商信息下的结束时间
                //供应商变更新设：工程/供应商信息下的开始时间
                //供应商变更废止：工程/供应商信息下的开始时间
                //包装工厂变更新设：工程/供应商信息下的开始时间
                //包装工厂变更废止：工程/供应商信息下的开始时间
                //防锈变更：=文本“即时切替”

                if (multipleSelection == null)//如果没有选中数据，那么就是按检索条件发送
                {
                    string strChange = dataForm.Change;
                    string strPart_id = dataForm.Part_id;
                    string strOriginCompany = dataForm.OriginCompany;
                    string strHaoJiu = dataForm.HaoJiu;
                    string strProjectType = dataForm.ProjectType;
                    if (loginInfo.Special == "财务用户")
                        strProjectType = "内制";
                    string strPriceChangeInfo = dataForm.PriceChangeInfo;
                    string strCarTypeDev = dataForm.CarTypeDev;
                    string strSupplier_id = dataForm.Supplier_id;
                    string strReceiver = dataForm.Receiver;
                    string strPriceState = dataForm.PriceState;
 
                    DataTable dt = fs0309_Logic.Search(strChange, strPart_id, strOriginCompany, strHaoJiu
                      , strProjectType, strPriceChangeInfo, strCarTypeDev, strSupplier_id
                      , strReceiver, strPriceState
                      );
                    string[] fields = {  "iNo","vcPart_id","vcCarTypeDev","vcPart_Name","vcSupplier_Name",
                        "vcSupplier_id","vcOE","vcPart_id_HK","vcHaoJiu_Name","vcStateFX","vcFXNO","vcChange_Name"
                    ,"dQieTi","field1","field2","field3"
                    };
                    DataTable result = dt.Clone();
                    result.Columns.Add("iNo");//序号
                    result.Columns.Add("dQieTi");//切替预定日
                    result.Columns.Add("field1");//价格设定要望日
                    result.Columns.Add("field2");//调达原价
                    result.Columns.Add("field3");//调达部回答日

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow row=result.NewRow();
                        row["iNo"] = i+1;
                        row["field1"] = "";
                        row["field2"] = "";
                        row["field3"] = "";
                        row["vcPart_id"] = dt.Rows[i]["vcPart_id"];
                        row["vcCarTypeDev"] = dt.Rows[i]["vcCarTypeDev"];
                        row["vcPart_Name"] = dt.Rows[i]["vcPart_Name"];
                        row["vcSupplier_Name"] = dt.Rows[i]["vcSupplier_Name"];
                        row["vcSupplier_id"] = dt.Rows[i]["vcSupplier_id"];
                        row["vcOE"] = dt.Rows[i]["vcOE"];
                        row["vcPart_id_HK"] = dt.Rows[i]["vcPart_id_HK"];
                        row["vcHaoJiu_Name"] = dt.Rows[i]["vcHaoJiu_Name"];
                        row["vcStateFX"] = dt.Rows[i]["vcStateFX"];
                        row["vcFXNO"] = dt.Rows[i]["vcFXNO"];
                        row["vcChange"] = dt.Rows[i]["vcChange"];
                        row["vcChange_Name"] = dt.Rows[i]["vcChange_Name"];
                        if (row["vcChange"].ToString() == "1")//新设：使用开始时间
                        {
                            row["dQieTi"] = dt.Rows[i]["dUseBegin"]; 
                        }
                        else if (row["vcChange"].ToString() == "5")//旧型：旧型开始
                        {
                            row["dQieTi"] = dt.Rows[i]["dJiuBegin"];
                        }
                        else if (row["vcChange"].ToString() == "4")//废止：使用结束时间
                        {
                            row["dQieTi"] = dt.Rows[i]["dUseEnd"];
                        }
                        else if (row["vcChange"].ToString() == "6")//恢复现号：旧型结束+1天
                        {
                            DateTime temp_d = Convert.ToDateTime(dt.Rows[i]["dJiuEnd"]).AddDays(1);
                            row["dQieTi"] = temp_d;
                        }
                        else if (row["vcChange"].ToString() == "16")//复活：使用开始时间
                        {
                            row["dQieTi"] = dt.Rows[i]["dUseBegin"];
                        }
                        else if (row["vcChange"].ToString() == "7")//持续生产：旧型持续开始
                        {
                            row["dQieTi"] = dt.Rows[i]["dJiuBeginSustain"];
                        }
                        else if (row["vcChange"].ToString() == "15")//一括生产：使用结束时间
                        {
                            row["dQieTi"] = dt.Rows[i]["dUseEnd"];
                        }
                        else if (row["vcChange"].ToString() == "8")//工程变更新设：工程/供应商信息下的开始时间
                        {
                            row["dQieTi"] = dt.Rows[i]["dProjectBegin"];
                        }
                        else if (row["vcChange"].ToString() == "9") //工程变更废止：工程/供应商信息下的结束时间
                        {
                            row["dQieTi"] = dt.Rows[i]["dProjectEnd"];
                        }
                        else if (row["vcChange"].ToString() == "10")//供应商变更新设：工程/供应商信息下的开始时间
                        {
                            row["dQieTi"] = dt.Rows[i]["dProjectBegin"];
                        }
                        else if (row["vcChange"].ToString() == "11")//供应商变更废止：工程/供应商信息下的开始时间
                        {
                            row["dQieTi"] = dt.Rows[i]["dProjectEnd"];
                        }
                        else if (row["vcChange"].ToString() == "12")//包装工厂变更新设：工程/供应商信息下的开始时间
                        {
                            row["dQieTi"] = dt.Rows[i]["dProjectBegin"];
                        }
                        else if (row["vcChange"].ToString() == "13")//包装工厂变更废止：工程/供应商信息下的开始时间
                        {
                            row["dQieTi"] = dt.Rows[i]["dProjectEnd"];
                        }
                        else if (row["vcChange"].ToString() == "17")//防锈变更：=文本“即时切替”
                        {
                            row["dQieTi"] = "即时切替";
                        }
                        result.Rows.Add(row);
                    }

                    string filepath = fs0309_Logic.generateExcelWithXlt(result, fields, _webHostEnvironment.ContentRootPath, "FS0309_DiaoDa.xlsx", 8, loginInfo.UserId, FunctionID);
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
                else
                {
                    JArray checkedInfo = dataForm.multipleSelection;
                    List<Dictionary<string, Object>> listInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();
                    string[] fields = {  "iNo","vcPart_id","vcCarTypeDev","vcPart_Name","vcSupplier_Name",
                        "vcSupplier_id","vcOE","vcPart_id_HK","vcHaoJiu_Name","vcStateFX","vcFXNO","vcChange_Name"
                    ,"dQieTi","field1","field2","field3"
                    };
                    DataTable result = new DataTable();
                    result.Columns.Add("iNo");//序号
                    result.Columns.Add("vcPart_id");
                    result.Columns.Add("vcCarTypeDev");
                    result.Columns.Add("vcPart_Name");
                    result.Columns.Add("vcSupplier_Name");
                    result.Columns.Add("vcSupplier_id");
                    result.Columns.Add("vcOE");
                    result.Columns.Add("vcPart_id_HK");
                    result.Columns.Add("vcHaoJiu_Name");
                    result.Columns.Add("vcStateFX");
                    result.Columns.Add("vcFXNO");
                    result.Columns.Add("vcChange");
                    result.Columns.Add("vcChange_Name");
                    result.Columns.Add("dQieTi");//切替预定日
                    result.Columns.Add("field1");//价格设定要望日
                    result.Columns.Add("field2");//调达原价
                    result.Columns.Add("field3");//调达部回答日

                    for (int i = 0; i < listInfoData.Count; i++)
                    {
                        DataRow row = result.NewRow();
                        row["iNo"] = i + 1;
                        row["field1"] = "";
                        row["field2"] = "";
                        row["field3"] = "";
                        row["vcPart_id"] = listInfoData[i]["vcPart_id"];
                        row["vcCarTypeDev"] = listInfoData[i]["vcCarTypeDev"];
                        row["vcPart_Name"] = listInfoData[i]["vcPart_Name"];
                        row["vcSupplier_Name"] = listInfoData[i]["vcSupplier_Name"];
                        row["vcSupplier_id"] = listInfoData[i]["vcSupplier_id"];
                        row["vcOE"] = listInfoData[i]["vcOE"];
                        row["vcPart_id_HK"] = listInfoData[i]["vcPart_id_HK"];
                        row["vcHaoJiu_Name"] = listInfoData[i]["vcHaoJiu_Name"];
                        row["vcStateFX"] = listInfoData[i]["vcStateFX"];
                        row["vcFXNO"] = listInfoData[i]["vcFXNO"];
                        row["vcChange"] = listInfoData[i]["vcChange"];
                        row["vcChange_Name"] = listInfoData[i]["vcChange_Name"];
                        if (row["vcChange"].ToString() == "1")//新设：使用开始时间
                        {
                            row["dQieTi"] = listInfoData[i]["dUseBegin"];
                        }
                        else if (row["vcChange"].ToString() == "5")//旧型：旧型开始
                        {
                            row["dQieTi"] = listInfoData[i]["dJiuBegin"];
                        }
                        else if (row["vcChange"].ToString() == "4")//废止：使用结束时间
                        {
                            row["dQieTi"] = listInfoData[i]["dUseEnd"];
                        }
                        else if (row["vcChange"].ToString() == "6")//恢复现号：旧型结束+1天
                        {
                            DateTime temp_d = Convert.ToDateTime(listInfoData[i]["dJiuEnd"]).AddDays(1);
                            row["dQieTi"] = temp_d;
                        }
                        else if (row["vcChange"].ToString() == "16")//复活：使用开始时间
                        {
                            row["dQieTi"] = listInfoData[i]["dUseBegin"];
                        }
                        else if (row["vcChange"].ToString() == "7")//持续生产：旧型持续开始
                        {
                            row["dQieTi"] = listInfoData[i]["dJiuBeginSustain"];
                        }
                        else if (row["vcChange"].ToString() == "15")//一括生产：使用结束时间
                        {
                            row["dQieTi"] = listInfoData[i]["dUseEnd"];
                        }
                        else if (row["vcChange"].ToString() == "8")//工程变更新设：工程/供应商信息下的开始时间
                        {
                            row["dQieTi"] = listInfoData[i]["dProjectBegin"];
                        }
                        else if (row["vcChange"].ToString() == "9") //工程变更废止：工程/供应商信息下的结束时间
                        {
                            row["dQieTi"] = listInfoData[i]["dProjectEnd"];
                        }
                        else if (row["vcChange"].ToString() == "10")//供应商变更新设：工程/供应商信息下的开始时间
                        {
                            row["dQieTi"] = listInfoData[i]["dProjectBegin"];
                        }
                        else if (row["vcChange"].ToString() == "11")//供应商变更废止：工程/供应商信息下的开始时间
                        {
                            row["dQieTi"] = listInfoData[i]["dProjectEnd"];
                        }
                        else if (row["vcChange"].ToString() == "12")//包装工厂变更新设：工程/供应商信息下的开始时间
                        {
                            row["dQieTi"] = listInfoData[i]["dProjectBegin"];
                        }
                        else if (row["vcChange"].ToString() == "13")//包装工厂变更废止：工程/供应商信息下的开始时间
                        {
                            row["dQieTi"] = listInfoData[i]["dProjectEnd"];
                        }
                        else if (row["vcChange"].ToString() == "17")//防锈变更：=文本“即时切替”
                        {
                            row["dQieTi"] = "即时切替";
                        }
                        result.Rows.Add(row);
                    }

                    string filepath = fs0309_Logic.generateExcelWithXlt(result, fields, _webHostEnvironment.ContentRootPath, "FS0309_DiaoDa.xlsx", 8, loginInfo.UserId, FunctionID);
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
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0911", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "送信失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}
