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
                List<Object> dataList_C006 = ComFunction.convertAllToResult(ComFunction.getTCode("C006"));//原单位
                List<Object> dataList_C028 = ComFunction.convertAllToResult(ComFunction.getTCode("C028"));//防锈
                List<Object> dataList_C038 = ComFunction.convertAllToResult(ComFunction.getTCode("C038"));//公式名

                Dictionary<string, object> row_All = new Dictionary<string, object>();//公式增加请选择
                row_All["vcName"] = "请选择";
                row_All["vcValue"] = "";
                dataList_C038.Insert(0,row_All);

                //设变履历是否下拉待确定
                List<Object> dataList_C005 = ComFunction.convertAllToResult(ComFunction.getTCode("C005"));//收货方

                int iTask = fs0309_Logic.getAllTask();

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
                if (dt != null && dt.Rows.Count >= 10000)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "本次检索数据条数超过10000,为避免浏览器内存溢出，请调整检索条件或进行数据导出。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
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
                string[] fields = { "iAutoId","vcChange_Name", "vcPart_id", "dUseBeginStr", "dUseEndStr", "vcProjectType_Name", "vcSupplier_id"
                ,"vcSupplier_Name","dProjectBeginStr","dProjectEndStr","vcHaoJiu_Name","dJiuBeginStr","dJiuEndStr","dJiuBeginSustainStr","vcPriceChangeInfo_Name"
                ,"vcPriceState_Name","dPriceStateDateStr","vcPriceGS_Name","decPriceOrigin","decPriceAfter","decPriceTNPWithTax","dPricebeginStr","dPriceEndStr"
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
                    string[,] strField = new string[,] {{"变更事项","品番","使用开始","使用结束","内外","供应商代码","供应商名称","开始","结束","号旧","TNP含税","价格开始","价格结束","收货方","所属原单位"},
                                                {"vcChange","vcPart_id","dUseBegin","dUseEnd","vcProjectType","vcSupplier_id","vcSupplier_Name","dProjectBegin","dProjectEnd","vcHaoJiu","decPriceTNPWithTax","dPricebegin","dPriceEnd","vcReceiver","vcOriginCompany"},
                                                {"",FieldCheck.NumChar,FieldCheck.Date,FieldCheck.Date,"","","",FieldCheck.Date,FieldCheck.Date,"","","","","","" },
                                                {"25","12","0","0","0","4","50","0","0","0","0","0","0","0","0"},//最大长度设定,不校验最大长度用0
                                                {"0","10","1","1","1","1","1","1","1","1","0","0","0","1","1"},//最小长度设定,可以为空用0
                                                {"1","2","3","4","5","6","7","8","9","10","20","21","22","31","32"}//前台显示列号，从0开始计算,注意有选择框的是0
                    };
                    //需要判断时间区间先后关系的字段
                    string[,] strDateRegion = { { "dUseBegin", "dUseEnd" }, { "dProjectBegin", "dProjectEnd" }, { "dJiuBegin", "dJiuEnd" }, { "dPricebegin", "dPriceEnd" } };
                    string[,] strSpecialCheck = {
                        //例子-变更事项字段，当它为新设时，号旧必须为号口，旧型开始、旧型结束、旧型持续开始必须为空
                        //vcChange=1时，vcHaoJiu如果为1，如果内容列不为空(H)，则内容必须为H，如果内容为空，则对具体内容不做判断
                        { "TNP含税","decPriceTNPWithTax", "有金额","", "价格开始","dPricebegin","1", "","" },
                        { "TNP含税","decPriceTNPWithTax", "有金额","", "价格结束","dPriceEnd","1", "","" }
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
                    apiResult.data = "保存失败，以下品番价格开始、价格结束区间存在重叠：<br/>" + strErrorPartId;
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
                int iTask = fs0309_Logic.getAllTask();
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = iTask;
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
                JArray checkedInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();

                if (listInfoData == null || listInfoData.Count <= 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "至少选择一条数据！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                DataTable dt = fs0309_Logic.getXiaoShouZhanKai(listInfoData);
                string[] fields = {  "iNum","vcPart_id","vcFaZhuPlant","dQieTi","vcPart_Name","vcChange_Name",
                        "vcPartId_Replace","decPriceTNPWithTax","iPackingQty","vcCarTypeDev","vcNote"
                    };

                //获取单号，生成单号
                int iDanhao=fs0309_Logic.getNewDanHao(loginInfo.UnitCode);
                if(iDanhao>99)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "连番大于99，不可再生成！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string strDanhao = "PIC-"+ loginInfo.UnitCode + "-"+DateTime.Now.ToString("yyMMdd")+"-"+ iDanhao.ToString("00");

                string filepath = fs0309_Logic.generateExcelWithXlt_XiaoShou(dt, fields, _webHostEnvironment.ContentRootPath, "FS0309_XiaoShou.xlsx", loginInfo.UserId, FunctionID, strDanhao);
                if (filepath == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "导出生成文件失败";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fs0309_Logic.updateXiaoShouZhanKaiState(listInfoData, strDanhao);//变更事项变空，状态改为PIC
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = filepath;
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

        #region 送信-调达-外注
        [HttpPost]
        [EnableCors("any")]
        public string sendDiaoDaApi_Wai([FromBody] dynamic data)
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

                 

                JArray checkedInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();
                string[] fields = {  "iNo","vcPart_id","vcCarTypeDev","vcPart_Name","vcSupplier_Name",
                        "vcSupplier_id","vcOE","vcPart_id_HK","vcHaoJiu_Name","vcStateFX","vcFXNO","vcChange_Name"
                    ,"dQieTiStr","field1","field2","field3"
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
                result.Columns.Add("dQieTiStr");//切替预定日
                result.Columns.Add("field1");//价格设定要望日
                result.Columns.Add("field2");//调达原价
                result.Columns.Add("field3");//调达部回答日

                for (int i = 0; i < listInfoData.Count; i++)
                {
                    DataRow row = result.NewRow();
                    string strProjectType= listInfoData[i]["vcProjectType"].ToString();
                    if (strProjectType != "1")//非外注排除掉
                        continue;

                    row["iNo"] = i + 1;
                    row["field1"] = "";
                    row["field2"] = "";
                    row["field3"] = "";
                    row["vcPart_id"] = listInfoData[i]["vcPart_id"];
                    row["vcCarTypeDev"] = listInfoData[i]["vcCarTypeDev"];
                    row["vcPart_Name"] = listInfoData[i]["vcPart_Name"];
                    row["vcSupplier_Name"] = listInfoData[i]["vcSupplier_Name"];
                    row["vcSupplier_id"] = listInfoData[i]["vcSupplier_id"];
                    row["vcOE"] = listInfoData[i]["vcOE_Name"];
                    row["vcPart_id_HK"] = listInfoData[i]["vcPart_id_HK"];
                    row["vcHaoJiu_Name"] = listInfoData[i]["vcHaoJiu_Name"];
                    row["vcStateFX"] = listInfoData[i]["vcStateFX"];
                    row["vcFXNO"] = listInfoData[i]["vcFXNO"];
                    row["vcChange"] = listInfoData[i]["vcChange"];
                    row["vcChange_Name"] = listInfoData[i]["vcChange_Name"];
                    if (row["vcChange"].ToString() == "1"||row["vcChange"].ToString() == "2")//新车新设 设变新设：使用开始时间
                    {
                        row["dQieTiStr"] = Convert.ToDateTime(listInfoData[i]["dUseBegin"]).ToString("yyyy/MM/dd");
                    }
                    else if (row["vcChange"].ToString() == "5")//旧型：旧型开始
                    {
                        row["dQieTiStr"] = Convert.ToDateTime(listInfoData[i]["dJiuBegin"]).ToString("yyyy/MM/dd");
                    }
                    else if (row["vcChange"].ToString() == "4")//废止：使用结束时间
                    {
                        row["dQieTiStr"] = Convert.ToDateTime(listInfoData[i]["dUseEnd"]).ToString("yyyy/MM/dd");
                    }
                    else if (row["vcChange"].ToString() == "6")//恢复现号：旧型结束+1天
                    {
                        DateTime temp_d = Convert.ToDateTime(listInfoData[i]["dJiuEnd"]).AddDays(1);
                        row["dQieTiStr"] = temp_d.ToString("yyyy/MM/dd");
                    }
                    else if (row["vcChange"].ToString() == "16")//复活：使用开始时间
                    {
                        row["dQieTiStr"] = Convert.ToDateTime(listInfoData[i]["dUseBegin"]).ToString("yyyy/MM/dd");
                    }
                    else if (row["vcChange"].ToString() == "7")//持续生产：旧型持续开始
                    {
                        row["dQieTiStr"] = Convert.ToDateTime(listInfoData[i]["dJiuBeginSustain"]).ToString("yyyy/MM/dd");
                    }
                    else if (row["vcChange"].ToString() == "15")//一括生产：使用结束时间
                    {
                        row["dQieTiStr"] = Convert.ToDateTime(listInfoData[i]["dUseEnd"]).ToString("yyyy/MM/dd");
                    }
                    else if (row["vcChange"].ToString() == "8")//工程变更新设：工程/供应商信息下的开始时间
                    {
                        row["dQieTiStr"] = Convert.ToDateTime(listInfoData[i]["dProjectBegin"]).ToString("yyyy/MM/dd");
                    }
                    else if (row["vcChange"].ToString() == "9") //工程变更废止：工程/供应商信息下的结束时间
                    {
                        row["dQieTiStr"] = Convert.ToDateTime(listInfoData[i]["dProjectEnd"]).ToString("yyyy/MM/dd");
                    }
                    else if (row["vcChange"].ToString() == "10")//供应商变更新设：工程/供应商信息下的开始时间
                    {
                        row["dQieTiStr"] = Convert.ToDateTime(listInfoData[i]["dProjectBegin"]).ToString("yyyy/MM/dd");
                    }
                    else if (row["vcChange"].ToString() == "11")//供应商变更废止：工程/供应商信息下的结束时间
                    {
                        row["dQieTiStr"] = Convert.ToDateTime(listInfoData[i]["dProjectEnd"]).ToString("yyyy/MM/dd");
                    }
                    else if (row["vcChange"].ToString() == "12")//包装工厂变更新设：工程/供应商信息下的开始时间
                    {
                        row["dQieTiStr"] = Convert.ToDateTime(listInfoData[i]["dProjectBegin"]).ToString("yyyy/MM/dd");
                    }
                    else if (row["vcChange"].ToString() == "13")//包装工厂变更废止：工程/供应商信息下的结束时间
                    {
                        row["dQieTiStr"] = Convert.ToDateTime(listInfoData[i]["dProjectEnd"]).ToString("yyyy/MM/dd");
                    }
                    else if (row["vcChange"].ToString() == "17")//防锈变更：=文本“即时切替”
                    {
                        row["dQieTiStr"] = "即时切替";
                    }
                    else if (row["vcChange"].ToString() == "3")//打切旧型：旧型开始时间
                    {
                        row["dQieTiStr"] = Convert.ToDateTime(listInfoData[i]["dJiuBegin"]).ToString("yyyy/MM/dd");
                    }
                    else if (row["vcChange"].ToString() == "14")//生产打切：品番使用结束时间
                    {
                        row["dQieTiStr"] = Convert.ToDateTime(listInfoData[i]["dUseEnd"]).ToString("yyyy/MM/dd");
                    }
                    result.Rows.Add(row);
                }
                if (result.Rows.Count == 0)//没有要导出的数据，直接返回
                {
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.data = "";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                DataTable dt10Year=fs0309_Logic.getOld_10_Year(listInfoData,"1");
                string[] fields10Year = {  "iNo","vcPart_id","vcCarTypeDev","vcPart_Name","vcSupplier_Name",
                        "vcSupplier_id","vcNum1","vcNum2","vcNum3","vcNum4","vcNum5","vcNum6"
                    ,"vcNum7","vcNum8","vcNum9","vcNum10"
                    };

                string filepath = fs0309_Logic.generateExcelWithXlt_Wai(result, dt10Year, fields, fields10Year, _webHostEnvironment.ContentRootPath, "FS0309_DiaoDa_Wai.xlsx", loginInfo.UserId, FunctionID);
                if (filepath == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "导出生成文件失败";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string strErr = "";
                fs0309_Logic.sendDiaoDaChangeState(listInfoData, ref strErr);
                if (strErr != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = strErr;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = filepath;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
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

        #region 送信-调达-内制
        [HttpPost]
        [EnableCors("any")]
        public string sendDiaoDaApi_Nei([FromBody] dynamic data)
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

 
                JArray checkedInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();
                string[] fields = {  "iNo","vcPart_id","vcCarTypeDev","vcPart_Name","vcSupplier_Name",
                        "vcSupplier_id","vcOE","vcPart_id_HK","vcHaoJiu_Name","dQieTiStr","field1","field2","field3"
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
                result.Columns.Add("dQieTiStr");//切替预定日
                result.Columns.Add("field1");//价格设定要望日
                result.Columns.Add("field2");//调达原价
                result.Columns.Add("field3");//调达部回答日


                List<string> carTypeList = new List<string>();//存放车型开发，根据车型给财务发邮件

                for (int i = 0; i < listInfoData.Count; i++)
                {
                    string strProjectType = listInfoData[i]["vcProjectType"].ToString();
                    if (strProjectType != "0")//非内制排除掉
                        continue;

                    DataRow row = result.NewRow();
                    row["iNo"] = i + 1;
                    row["field1"] = "";
                    row["field2"] = "";
                    row["field3"] = "";
                    row["vcPart_id"] = listInfoData[i]["vcPart_id"];
                    row["vcCarTypeDev"] = listInfoData[i]["vcCarTypeDev"];
                    string strCarTypeDev = listInfoData[i]["vcCarTypeDev"] == null ? "" : listInfoData[i]["vcCarTypeDev"].ToString();
                    carTypeList.Add(strCarTypeDev);//存储车型开发
                    row["vcPart_Name"] = listInfoData[i]["vcPart_Name"];
                    row["vcSupplier_Name"] = listInfoData[i]["vcSupplier_Name"];
                    row["vcSupplier_id"] = listInfoData[i]["vcSupplier_id"];
                    row["vcOE"] = listInfoData[i]["vcOE_Name"];
                    row["vcPart_id_HK"] = listInfoData[i]["vcPart_id_HK"];
                    row["vcHaoJiu_Name"] = listInfoData[i]["vcHaoJiu_Name"];
                    row["vcStateFX"] = listInfoData[i]["vcStateFX"];
                    row["vcFXNO"] = listInfoData[i]["vcFXNO"];
                    row["vcChange"] = listInfoData[i]["vcChange"];
                    row["vcChange_Name"] = listInfoData[i]["vcChange_Name"];
                    if (row["vcChange"].ToString() == "1"|| row["vcChange"].ToString() == "2")//新车新设 设变新设：使用开始时间
                    {
                        row["dQieTiStr"] = Convert.ToDateTime(listInfoData[i]["dUseBegin"]).ToString("yyyy/MM/dd");
                    }
                    else if (row["vcChange"].ToString() == "5")//旧型：旧型开始
                    {
                        row["dQieTiStr"] = Convert.ToDateTime(listInfoData[i]["dJiuBegin"]).ToString("yyyy/MM/dd");
                    }
                    else if (row["vcChange"].ToString() == "4")//废止：使用结束时间
                    {
                        row["dQieTiStr"] = Convert.ToDateTime(listInfoData[i]["dUseEnd"]).ToString("yyyy/MM/dd");
                    }
                    else if (row["vcChange"].ToString() == "6")//恢复现号：旧型结束+1天
                    {
                        DateTime temp_d = Convert.ToDateTime(listInfoData[i]["dJiuEnd"]).AddDays(1);
                        row["dQieTiStr"] = temp_d.ToString("yyyy/MM/dd");
                    }
                    else if (row["vcChange"].ToString() == "16")//复活：使用开始时间
                    {
                        row["dQieTiStr"] = Convert.ToDateTime(listInfoData[i]["dUseBegin"]).ToString("yyyy/MM/dd");
                    }
                    else if (row["vcChange"].ToString() == "7")//持续生产：旧型持续开始
                    {
                        row["dQieTiStr"] = Convert.ToDateTime(listInfoData[i]["dJiuBeginSustain"]).ToString("yyyy/MM/dd");
                    }
                    else if (row["vcChange"].ToString() == "15")//一括生产：使用结束时间
                    {
                        row["dQieTiStr"] = Convert.ToDateTime(listInfoData[i]["dUseEnd"]).ToString("yyyy/MM/dd");
                    }
                    else if (row["vcChange"].ToString() == "8")//工程变更新设：工程/供应商信息下的开始时间
                    {
                        row["dQieTiStr"] = Convert.ToDateTime(listInfoData[i]["dProjectBegin"]).ToString("yyyy/MM/dd");
                    }
                    else if (row["vcChange"].ToString() == "9") //工程变更废止：工程/供应商信息下的结束时间
                    {
                        row["dQieTiStr"] = Convert.ToDateTime(listInfoData[i]["dProjectEnd"]).ToString("yyyy/MM/dd");
                    }
                    else if (row["vcChange"].ToString() == "10")//供应商变更新设：工程/供应商信息下的开始时间
                    {
                        row["dQieTiStr"] = Convert.ToDateTime(listInfoData[i]["dProjectBegin"]).ToString("yyyy/MM/dd");
                    }
                    else if (row["vcChange"].ToString() == "11")//供应商变更废止：工程/供应商信息下的结束时间
                    {
                        row["dQieTiStr"] = Convert.ToDateTime(listInfoData[i]["dProjectEnd"]).ToString("yyyy/MM/dd");
                    }
                    else if (row["vcChange"].ToString() == "12")//包装工厂变更新设：工程/供应商信息下的开始时间
                    {
                        row["dQieTiStr"] = Convert.ToDateTime(listInfoData[i]["dProjectBegin"]).ToString("yyyy/MM/dd");
                    }
                    else if (row["vcChange"].ToString() == "13")//包装工厂变更废止：工程/供应商信息下的结束时间
                    {
                        row["dQieTiStr"] = Convert.ToDateTime(listInfoData[i]["dProjectEnd"]).ToString("yyyy/MM/dd");
                    }
                    else if (row["vcChange"].ToString() == "17")//防锈变更：=文本“即时切替”
                    {
                        row["dQieTiStr"] = "即时切替";
                    }
                    else if (row["vcChange"].ToString() == "3")//打切旧型：旧型开始时间
                    {
                        row["dQieTiStr"] = Convert.ToDateTime(listInfoData[i]["dJiuBegin"]).ToString("yyyy/MM/dd");
                    }
                    else if (row["vcChange"].ToString() == "14")//生产打切：品番使用结束时间
                    {
                        row["dQieTiStr"] = Convert.ToDateTime(listInfoData[i]["dUseEnd"]).ToString("yyyy/MM/dd");
                    }
                    result.Rows.Add(row);
                }
                if (result.Rows.Count == 0)//没有要导出的数据，直接返回
                {
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.data = "";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                DataTable dt10Year = fs0309_Logic.getOld_10_Year(listInfoData,"0");
                string[] fields10Year = {  "iNo","vcPart_id","vcCarTypeDev","vcPart_Name","vcSupplier_Name",
                        "vcSupplier_id","vcNum1","vcNum2","vcNum3","vcNum4","vcNum5","vcNum6"
                    ,"vcNum7","vcNum8","vcNum9","vcNum10"
                    };

                string filepath = fs0309_Logic.generateExcelWithXlt_Nei(result, dt10Year, fields, fields10Year, _webHostEnvironment.ContentRootPath, "FS0309_DiaoDa_Nei.xlsx", loginInfo.UserId, FunctionID);
                if (filepath == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "导出生成文件失败";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string strErr = "";
                #region 给财务发邮件

                List<string> carTypeListDistinct= carTypeList.Distinct().ToList();

                fs0309_Logic.sendEmailToCaiWu(loginInfo.Email, loginInfo.UnitName, ref strErr,carTypeListDistinct);
                if (strErr != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = strErr;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                #endregion

                #region 最后更改状态---理论上这一步不可能报错
                fs0309_Logic.sendDiaoDaChangeState(listInfoData, ref strErr);
                if (strErr != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = strErr;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                #endregion

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = filepath;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
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


        #region 公式选择时进行计算
        [HttpPost]
        [EnableCors("any")]
        public string gsChangeApi([FromBody]dynamic data)
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
                

                JArray checkedInfo = dataForm.gsChangeItem;
                List<Dictionary<string, Object>> listInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();



                if (listInfoData[0]["decPriceOrigin"]==null|| listInfoData[0]["decPriceOrigin"].ToString().Trim()==""|| listInfoData[0]["vcPriceGS"]==null|| listInfoData[0]["vcPriceGS"].ToString()=="")
                {
                    Dictionary<string, object> res_return = new Dictionary<string, object>();
                    res_return.Add("priceAfter", "");
                    res_return.Add("priceTNPWithTax", "");
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.data = res_return;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                string strPartId = listInfoData[0]["vcPart_id"].ToString();
                string strSupplier = listInfoData[0]["vcSupplier_id"].ToString();
                string strAutoId ="";
                if (listInfoData[0]["iAutoId"]!=null&& listInfoData[0]["iAutoId"].ToString()!="")
                {
                    strAutoId=listInfoData[0]["iAutoId"].ToString();
                }
                    
                string strGS = listInfoData[0]["vcPriceGS"].ToString();
                decimal decPriceOrigin = Convert.ToDecimal(listInfoData[0]["decPriceOrigin"]);
                string strReceiver = listInfoData[0]["vcReceiver"].ToString();

                if ((strGS == "2") &&!fs0309_Logic.getLastStateGsData(strPartId, strSupplier, strReceiver, strAutoId))
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "选择公式B的品番必须存在上个状态的价格信息！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (strGS!=""&& strGS!="3"&&!fs0309_Logic.isGsExist(strGS))
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "选择的公式没维护基础数据！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }


                DataTable dt=fs0309_Logic.getGSChangePrice(strPartId, strSupplier, strReceiver, strAutoId, strGS, decPriceOrigin);
                Dictionary<string, object> res = new Dictionary<string, object>();
                if (dt.Rows[0]["priceAfter"] == DBNull.Value)
                {
                    res.Add("priceAfter", "");
                    res.Add("priceTNPWithTax", "");
                }
                else
                {
                    decimal priceAfter = Convert.ToDecimal(dt.Rows[0]["priceAfter"]);
                    decimal priceTNPWithTax = Convert.ToDecimal(dt.Rows[0]["priceTNPWithTax"]);
                    res.Add("priceAfter", priceAfter);
                    res.Add("priceTNPWithTax", priceTNPWithTax);
                }
 
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0912", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "公式计算失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

    }
}
