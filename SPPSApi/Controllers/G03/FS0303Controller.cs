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
    [Route("api/FS0303/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0303Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0303_Logic fs0303_Logic = new FS0303_Logic();
        private readonly string FunctionID = "FS0303";

        public FS0303Controller(IWebHostEnvironment webHostEnvironment)
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

                List<string> dataList_C002 = convertTCodeToResult(getTCode("C035"));//变更事项
                List<string> dataList_C003 = convertTCodeToResult(getTCode("C003"));//内外区分
                List<string> dataList_C004 = convertTCodeToResult(getTCode("C004"));//号旧区分
                List<string> dataList_C005 = convertTCodeToResult(getTCode("C005"));//收货方
                List<string> dataList_C006 = convertTCodeToResult(getTCode("C006"));//原单位
                List<string> dataList_C012 = convertTCodeToResult(getTCode("C012"));//OE=SP
                List<string> dataList_C016 = convertTCodeToResult(getTCode("C016"));//包装事业体
                List<string> dataList_C019 = convertTCodeToResult(getTCode("C019"));//生确

                res.Add("C002", dataList_C002);
                res.Add("C003", dataList_C003);
                res.Add("C004", dataList_C004);
                res.Add("C005", dataList_C005);
                res.Add("C006", dataList_C006);
                res.Add("C012", dataList_C012);
                res.Add("C016", dataList_C016);
                res.Add("C019", dataList_C019);
                
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

        #region 检索（分页缓存）
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

            string strIsShowAll = dataForm.isShowAll;
            string strOriginCompany = dataForm.OriginCompany;


            string strSearchKey = dataForm.searchKey;
            int iPage = dataForm.page == null ? 0 : dataForm.page;
            int iPageSize = dataForm.pageSize;


            try
            {
                DataTable dt = null;
                int pageTotal = 0;//总页数
                if (isExistSearchCash(strSearchKey))//缓存已经存在，则从缓存中获取
                {
                    dt = getSearchResultByCash(strSearchKey, iPage, iPageSize,ref pageTotal);
                }
                else
                {
                    DataTable dtAll = fs0303_Logic.Search(strIsShowAll, strOriginCompany);
                    initSearchCash(strSearchKey, dtAll);
                    dt = getSearchResultByCash(strSearchKey, iPage, iPageSize, ref pageTotal);
                }
                
                DtConverter dtConverter = new DtConverter();

                dtConverter.addField("selected", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("dSyncTime",ConvertFieldType.DateType,"yyyy/MM/dd");
                dtConverter.addField("dTimeFrom", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dTimeTo", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dTimeFromSJ", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dGYSTimeFrom", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dGYSTimeTo", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dJiuBegin", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dJiuEnd", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dSSDateMonth", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dOperatorTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                apiResult.field1 = pageTotal;//这块需要把总页数返回
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0301", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
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
                    string[,] strField = new string[,] {{"同步数据"     ,"变更事项"     ,"设变号" ,"生确"          ,"区分"  ,"补给品番" ,"车型(设计)"  ,"车型(开发)"     ,"车名"         ,"使用开始"     ,"使用结束"     ,"切替实绩"     ,"SD"      ,"替代品番"     ,"英文品名"    ,"中文品名"    ,"号口工程","补给工程","内外"            ,"供应商代码"   ,"供应商名称"     ,"生产地"   ,"出荷地"   ,"包装工厂"      ,"生产商名称","地址"       ,"开始"         ,"结束"         ,"OE=SP"    ,"品番(参考)" ,"号旧"         ,"旧型开始"     ,"旧型结束"     ,"旧型经年" ,"旧型年限生产区分","实施年限(年限)","特记"  ,"防錆"    ,"防錆指示书号","旧型1年","旧型2年","旧型3年","旧型4年","旧型5年","旧型6年","旧型7年","旧型8年","旧型9年","旧型10年","旧型11年","旧型12年","旧型13年","旧型14年","旧型15年","执行标准No","收货方"         ,"所属原单位"          },
                                                        {"dSyncTime"    ,"vcChange_Name","vcSPINo","vcSQState_Name","vcDiff","vcPart_id","vcCarTypeDev","vcCarTypeDesign","vcCarTypeName","dTimeFrom"    ,"dTimeTo"      ,"dTimeFromSJ"  ,"vcBJDiff","vcPartReplace","vcPartNameEn","vcPartNameCn","vcHKGC"  ,"vcBJGC"  ,"vcInOutflag_Name","vcSupplier_id","vcSupplier_Name","vcSCPlace","vcCHPlace","vcSYTCode_Name","vcSCSName" ,"vcSCSAdress","dGYSTimeFrom" ,"dGYSTimeTo"   ,"vcOE_Name","vcHKPart_id","vcHaoJiu_Name","dJiuBegin"    ,"dJiuEnd"      ,"vcJiuYear","vcNXQF"          ,"dSSDate"       ,"vcMeno","vcFXDiff","vcFXNo"      ,"vcNum1" ,"vcNum2" ,"vcNum3" ,"vcNum4" ,"vcNum5" ,"vcNum6" ,"vcNum7" ,"vcNum8" ,"vcNum9" ,"vcNum10" ,"vcNum11" ,"vcNum12" ,"vcNum13" ,"vcNum14" ,"vcNum15" ,"vcZXBZNo"  ,"vcReceiver_Name","vcOriginCompany_Name"},
                                                        {FieldCheck.Date,""             ,""       ,""              ,""      ,""         ,""            ,""               ,""             ,FieldCheck.Date,FieldCheck.Date,FieldCheck.Date,""        ,""             ,""            ,""            ,""        ,""        ,""                ,""             ,""               ,""         ,""         ,""              ,""          ,""           ,FieldCheck.Date,FieldCheck.Date,""         ,""           ,""             ,FieldCheck.Date,FieldCheck.Date,""         ,""                ,FieldCheck.Date ,""      ,""        ,""            ,""       ,""       ,""       ,""       ,""       ,""       ,""       ,""       ,""       ,""        ,""        ,""        ,""        ,""        ,""        ,""          ,""               ,""                    },
                                                        {"0"            ,"0"            ,"20"     ,"0"             ,"0"     ,"12"       ,"4"           ,"4"              ,"4"            ,"0"            ,"0"            ,"0"            ,"4"       ,"12"           ,"100"         ,"100"         ,"20"      ,"20"      ,"0"               ,"4"            ,"100"            ,"20"       ,"20"       ,"0"             ,"100"       ,"100"        ,"0"            ,"0"            ,"0"        ,"12"         ,"0"            ,"0"            ,"0"            ,"4"        ,"20"              ,"0"             ,"200"   ,"2"       ,"12"          ,"4"      ,"4"      ,"4"      ,"4"      ,"4"      ,"4"      ,"4"      ,"4"      ,"4"      ,"4"       ,"4"       ,"4"       ,"4"       ,"4"       ,"4"       ,"50"        ,"0"              ,"0"                   },//最大长度设定,不校验最大长度用0
                                                        {"0"            ,"1"            ,"1"      ,"1"             ,"1"     ,"1"        ,"1"           ,"1"              ,"0"            ,"1"            ,"1"            ,"0"            ,"1"       ,"0"            ,"1"           ,"0"           ,"0"       ,"0"       ,"1"               ,"1"            ,"0"              ,"0"        ,"0"        ,"1"             ,"0"         ,"0"          ,"0"            ,"0"            ,"1"        ,"0"          ,"1"            ,"1"            ,"1"            ,"0"        ,"0"               ,"0"             ,"0"     ,"1"       ,"0"           ,"0"      ,"0"      ,"0"      ,"0"      ,"0"      ,"0"      ,"0"      ,"0"      ,"0"      ,"0"       ,"0"       ,"0"       ,"0"       ,"0"       ,"0"       ,"0"         ,"1"              ,"1"                   },//最小长度设定,可以为空用0
                                                        {"1"            ,"2"            ,"3"      ,"4"             ,"5"     ,"6"        ,"7"           ,"8"              ,"9"            ,"10"           ,"11"           ,"12"           ,"13"      ,"14"           ,"15"          ,"16"          ,"17"      ,"18"      ,"19"              ,"20"           ,"21"             ,"22"       ,"23"       ,"24"            ,"25"        ,"26"         ,"27"           ,"28"           ,"29"       ,"30"         ,"31"           ,"32"           ,"33"           ,"34"       ,"35"              ,"36"            ,"37"    ,"38"      ,"39"          ,"40"     ,"41"     ,"42"     ,"43"     ,"44"     ,"45"     ,"46"     ,"47"     ,"48"     ,"49"      ,"50"      ,"51"      ,"52"      ,"53"      ,"54"      ,"55"        ,"56"             ,"57"                  }
                    };
                    //需要判断时间区间先后关系的字段
                    string[,] strDateRegion = { { "dTimeFrom", "dTimeTo" }, { "dGYSTimeFrom", "dGYSTimeTo" }, { "dJiuBegin", "dJiuEnd" } };
                    string[,] strSpecialCheck = {
                        //例子-变更事项字段，当它为新设时，号旧必须为号口，旧型开始、旧型结束、旧型持续开始必须为空
                        //vcChange=1时，vcHaoJiu如果为1，如果内容列不为空(H)，则内容必须为H，如果内容为空，则对具体内容不做判断
                        { "号旧",
                            "vcHaoJiu_Name",//验证vcHaoJiu字段
                            "旧型", //填验证值对应的中文名
                            "旧型",//填验证值，当vcHaoJiu=Q时
                            "旧型开始",
                            "dJiuBegin",//判断字段
                            "1", //1:该字段不能为空 0:该字段必须为空
                            "",
                            ""
                        },{"防錆","vcFXDiff","R","R","防錆指示书号","vcFXNo","1","","" }
                    };

                    List<Object> checkRes = ListChecker.validateList(listInfoData, strField, strDateRegion, strSpecialCheck, true, "FS0303");
                    if (checkRes != null)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = ListChecker.listToString(checkRes);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }

                    #region 根据输入的Name获取对应的Value值，并将获取的Value值添加到dt的后面

                    #region 先定义哪些列涉及Name转Value

                    List<FS0303_Logic.NameOrValue> lists = new List<FS0303_Logic.NameOrValue>();
                    lists.Add(new FS0303_Logic.NameOrValue() { strTitle = "变更事项", strHeader = "vcChange", strCodeid = "C002", isNull = false });
                    lists.Add(new FS0303_Logic.NameOrValue() { strTitle = "内外", strHeader = "vcInOutflag", strCodeid = "C003", isNull = true });
                    lists.Add(new FS0303_Logic.NameOrValue() { strTitle = "号旧", strHeader = "vcHaoJiu", strCodeid = "C004", isNull = false });
                    lists.Add(new FS0303_Logic.NameOrValue() { strTitle = "收货方", strHeader = "vcReceiver", strCodeid = "C005", isNull = false });
                    lists.Add(new FS0303_Logic.NameOrValue() { strTitle = "所属原单位", strHeader = "vcOriginCompany", strCodeid = "C006", isNull = false });
                    lists.Add(new FS0303_Logic.NameOrValue() { strTitle = "OE=SP", strHeader = "vcOE", strCodeid = "C012", isNull = true });
                    lists.Add(new FS0303_Logic.NameOrValue() { strTitle = "包装工厂", strHeader = "vcSYTCode", strCodeid = "C016", isNull = true });
                    lists.Add(new FS0303_Logic.NameOrValue() { strTitle = "生确", strHeader = "vcSQState", strCodeid = "C019", isNull = false });
                    #endregion

                    #region 更新table
                    string strErr = "";         //记录错误信息
                    listInfoData = fs0303_Logic.ConverList(listInfoData, lists, ref strErr);
                    #endregion

                    #endregion

                    if (listInfoData == null)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = strErr;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }

                string strErrorPartId = "";
                fs0303_Logic.Save(listInfoData, loginInfo.UserId, ref strErrorPartId);
                if (strErrorPartId != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "保存失败，以下品番使用开始、结束区间存在重叠：<br/>" + strErrorPartId;
                    apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0302", ex, loginInfo.UserId);
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
                fs0303_Logic.Del(listInfoData, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0303", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除失败";
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

            string strIsShowAll = dataForm.isShowAll;
            string strOriginCompany = dataForm.OriginCompany;

            try
            {
                /*2020-01-04*/
                DataTable dt = fs0303_Logic.Search(strIsShowAll, strOriginCompany);
                string[] fields = { "dSyncTime", "vcChange_Name", "vcSPINo", "vcSQState_Name", "vcDiff"
                                    ,"vcPart_id","vcCarTypeDev","vcCarTypeDesign","vcCarTypeName"
                                    ,"dTimeFrom","dTimeTo","dTimeFromSJ","vcBJDiff","vcPartReplace"
                                    ,"vcPartNameEn","vcPartNameCn","vcHKGC","vcBJGC","vcInOutflag_Name"
                                    ,"vcSupplier_id","vcSupplier_Name","vcSCPlace","vcCHPlace"
                                    ,"vcSYTCode_Name","vcSCSName","vcSCSAdress","dGYSTimeFrom","dGYSTimeTo"
                                    ,"vcOE_Name","vcHKPart_id","vcHaoJiu_Name","dJiuBegin","dJiuEnd","vcJiuYear"
                                    ,"vcNXQF","dSSDate","vcMeno","vcFXDiff","vcFXNo","vcNum1"
                                    ,"vcNum2","vcNum3","vcNum4","vcNum5","vcNum6","vcNum7","vcNum8"
                                    ,"vcNum9","vcNum10","vcNum11","vcNum12","vcNum13","vcNum14","vcNum15"
                                    ,"vcZXBZNo","vcReceiver_Name","vcOriginCompany_Name"
                };
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0303_Export.xlsx", 1, loginInfo.UserId, FunctionID);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0304", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion


        #region 生确单发行
        [HttpPost]
        [EnableCors("any")]
        public string sqSendApi([FromBody] dynamic data)
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
                JArray checkedInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();
                if (listInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选择一条数据！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }


                string strSqDate = "";
                strSqDate = dataForm.dNqDate;
                if (string.IsNullOrEmpty(strSqDate))
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请先选择纳期日期";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                string strErr = "";

                for (int i = 0; i < listInfoData.Count; i++)
                {
                    string strSQState = listInfoData[i]["vcSQState_Name"].ToString();
                    if (strSQState!="未确认")
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "只可发行未确认的信息";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }

                fs0303_Logic.sqSend(listInfoData, strSqDate, loginInfo.UserId,loginInfo.Email,loginInfo.UserName,ref strErr);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0305", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "生确单发行失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 获取数据字典--只取名字
        public static DataTable getTCode(string strCodeId)
        {
            try
            {
                MultiExcute excute = new MultiExcute();
                System.Data.DataTable dt = new System.Data.DataTable();
                StringBuilder strSql = new StringBuilder();
                strSql.Append("   select vcName from TCode where vcCodeId='" + strCodeId + "'     \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        public static List<string> convertTCodeToResult(DataTable dt)
        {
            List<string> res = new List<string>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                res.Add(dt.Rows[i]["vcName"].ToString());
            }
            return res;
        }

        #region 数据同步
        [HttpPost]
        [EnableCors("any")]
        public string dataSyncApi([FromBody] dynamic data)
        {
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            //以下开始业务处理
            ApiResult apiResult = new ApiResult();
            string strMessage = "";
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

                #region 判断主key是否都不为null
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    //品番不能为null
                    if (listInfoData[i]["vcPart_id"] ==null|| string.IsNullOrEmpty(listInfoData[i]["vcPart_id"].ToString()))
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "第"+(i+1)+ "行品番不能为空，数据同步失败！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    //包装工厂不能为null
                    if (listInfoData[i]["vcSYTCode"] ==null||string.IsNullOrEmpty(listInfoData[i]["vcSYTCode"].ToString()))
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "第" + (i + 1) + "行的包装工厂不能为空，数据同步失败！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    //收货方时间不能为null
                    if (listInfoData[i]["vcReceiver"] ==null||string.IsNullOrEmpty(listInfoData[i]["vcReceiver"].ToString()))
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "第" + (i + 1) + "行的收货方不能为空，数据同步失败！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    //供应商代码不能为null
                    if (listInfoData[i]["vcSupplier_id"] == null|| string.IsNullOrEmpty(listInfoData[i]["vcSupplier_id"].ToString()))
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "第" + (i + 1) + "行的供应商代码不能为空，数据同步失败！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                #endregion

                fs0303_Logic.dataSync(listInfoData, loginInfo.UserId, ref strMessage);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = strMessage;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0305", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data =strMessage;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion


    }
}
