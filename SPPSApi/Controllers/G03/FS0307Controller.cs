using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SPPSApi.Controllers.G03
{
    [Route("api/FS0307/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0307Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string FunctionID = "FS0307";
        FS0307_Logic fs0307_logic = new FS0307_Logic();

        public FS0307Controller(IWebHostEnvironment webHostEnvironment)
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

                List<Object> dataList_C024 = ComFunction.convertAllToResult(ComFunction.getTCode("C024"));//旧型年限进度
                //List<string> dataList_C006_Excel = convertTCodeToResult(getTCode("C006"));//原单位
                List<string> dataList_C006_Excel = fs0307_logic.OriginCompanys(loginInfo.UserId);//原单位
                //List<string> dataList_C006_Excel = convertTCodeToResult(fs0307_logic.getCompany(loginInfo.PlantCode));//原单位
                List<string> dataList_C005_Excel = convertTCodeToResult(getTCode("C005"));//收货方
                List<string> dataList_C016_Excel = convertTCodeToResult(getTCode("C016"));//包装事业体
                List<string> dataList_C024_Excel = convertTCodeToResult(getTCode("C024"));//旧型年限进度
                List<string> dataList_C003_Excel = convertTCodeToResult(getTCode("C003"));//内外
                res.Add("C024", dataList_C024);
                res.Add("C003_E", dataList_C003_Excel);
                res.Add("C006_E", dataList_C006_Excel);
                res.Add("C005_E", dataList_C005_Excel);
                res.Add("C016_E", dataList_C016_Excel);
                res.Add("C024_E", dataList_C024_Excel);
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

        //#region 年限抽取

        //[HttpPost]
        //[EnableCors("any")]
        //public string extractApi([FromBody]dynamic data)
        //{
        //    //验证是否登录
        //    string strToken = Request.Headers["X-Token"];
        //    if (!isLogin(strToken))
        //    {
        //        return error_login();
        //    }
        //    LoginInfo loginInfo = getLoginByToken(strToken);
        //    //以下开始业务处理
        //    ApiResult apiResult = new ApiResult();
        //    dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
        //    JArray listInfo = dataForm.vcOriginCompany;
        //    List<string> vcOriginCompany = listInfo.ToObject<List<string>>();
        //    try
        //    {
        //        fs0307_logic.extractPart(loginInfo.UserId, vcOriginCompany);

        //        apiResult.code = ComConstant.SUCCESS_CODE;
        //        apiResult.data = null;
        //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //    }
        //    catch (Exception ex)
        //    {
        //        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0701", ex, loginInfo.UserId);
        //        apiResult.code = ComConstant.ERROR_CODE;
        //        apiResult.data = "年限抽取失败";
        //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //    }
        //}
        //#endregion

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
            string strSearchKey = dataForm.searchKey;
            int iPage = dataForm.page == null ? 0 : dataForm.page;
            int iPageSize = dataForm.pageSize;
            string fileName = dataForm.fileName;
            string strYear = dataForm.vcYear == null ? "" : dataForm.vcYear;
            if (!string.IsNullOrWhiteSpace(strYear))
            {
                strYear = Convert.ToDateTime(strYear).AddHours(8).ToString("yyyy");
            }
            string FinishFlag = dataForm.vcFinish == null ? "" : dataForm.vcFinish; ;

            try
            {
                DataTable dt = null;
                int pageTotal = 0;//总页数
                if (isExistSearchCash(strSearchKey))//缓存已经存在，则从缓存中获取
                {
                    dt = getSearchResultByCash(strSearchKey, iPage, iPageSize, ref pageTotal);
                }
                else
                {
                    DataTable dtAll = fs0307_logic.searchApi(strYear, FinishFlag, loginInfo.UserId);
                    initSearchCash(strSearchKey, dtAll);
                    dt = getSearchResultByCash(strSearchKey, iPage, iPageSize, ref pageTotal);
                }

                DtConverter dtConverter = new DtConverter();

                dtConverter.addField("selected", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("dSSDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dFinishYMD", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dJiuBegin", ConvertFieldType.DateType, "yyyy/MM/dd");

                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                apiResult.field1 = pageTotal;//这块需要把总页数返回
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0702", ex, loginInfo.UserId);
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

            string strYear = dataForm.strYear;
            string FinishFlag = dataForm.FinishFlag;

            try
            {
                DataTable dt = fs0307_logic.searchApi(strYear, FinishFlag, loginInfo.UserId);

                string[] fields = { "vcYear", "vcFinish", "dFinishYMD1", "vcSupplier_id", "vcSYTCode", "vcReceiver", "vcOriginCompany", "vcPart_id", "vcPartNameEn", "vcInOutflag", "vcCarTypeDev", "dJiuBegin1", "vcRemark", "vcOld10", "vcOld9", "vcOld7", "vcPM", "vcNum1", "vcNum2", "vcNum3", "vcNumAvg", "vcNXQF", "dSSDate1", "vcDY", "vcNum11", "vcNum12", "vcNum13", "vcNum14", "vcNum15", "vcNum16", "vcNum17", "vcNum18", "vcNum19", "vcNum20", "vcNum21" };
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0307.xlsx", 1, loginInfo.UserId, FunctionID);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0703", ex, loginInfo.UserId);
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

                OADateConvert(ref listInfoData);

                bool hasFind = false;//是否找到需要新增或者修改的数据
                bool bModFlag = false;
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
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
                    string[,] strField = new string[,] {{"进度","进度完成时间","厂家编码","品番","品名","内外区分","车种","旧型开始时间","备注","1年","2年","3年","年限区分","实施时间","对应可否","11年","12年","13年","14年","15年","16年","17年","18年","19年","20年","21年","原单位","包装事业体","收货方","平均"},
                                                {"vcFinish","dFinishYMD","vcSupplier_id","vcPart_id","vcPartNameEn","vcInOutflag","vcCarTypeDev","dJiuBegin","vcRemark","vcNum1","vcNum2","vcNum3","vcNXQF","dSSDate","vcDY","vcNum11","vcNum12","vcNum13","vcNum14","vcNum15","vcNum16","vcNum17","vcNum18","vcNum19","vcNum20","vcNum21","vcOriginCompany","vcSYTCode","vcReceiver","vcNumAvg"},
                                                {"",FieldCheck.Date,"",FieldCheck.NumCharL,"","",FieldCheck.NumChar,FieldCheck.Date,"",FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,"",FieldCheck.Date,"",FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,"","","",FieldCheck.NumChar },
                                                {"0","0","4","14","0","0","4","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0"},//最大长度设定,不校验最大长度用0
                                                {"1","0","4","10","1","1","4","1","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","1","1","1","0"},//最小长度设定,可以为空用0
                                                {"2","3","4","5","6","7","8","9","10","14","15","16","18","19","20","21","22","23","24","25","26","27","28","29","30","31","32","33","34","17"}//前台显示列号，从0开始计算,注意有选择框的是0
                    };
                    //需要判断时间区间先后关系的字段
                    string[,] strDateRegion = { };
                    string[,] strSpecialCheck = { };



                    List<Object> checkRes = ListChecker.validateList(listInfoData, strField, strDateRegion, strSpecialCheck, true, "FS0307");
                    if (checkRes != null)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = ListChecker.listToString(checkRes);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }

                string Msg = "";
                fs0307_logic.SaveApi(listInfoData, loginInfo.UserId, ref Msg);

                if (!string.IsNullOrWhiteSpace(Msg))
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = Msg;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0705", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }

        #endregion

        #region 织入原单位
        [HttpPost]
        [EnableCors("any")]
        public string InsertUnitApi([FromBody]dynamic data)
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

                OADateConvert(ref listInfoData);

                bool hasFind = false;//是否找到需要新增或者修改的数据
                if (listInfoData.Count > 0)
                {
                    hasFind = true;
                }
                if (!hasFind)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选择一条数据！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //开始数据验证
                if (hasFind)
                {
                    string[,] strField = new string[,] {{"进度","1年","2年","3年","年限区分","实施时间","对应可否","11年","12年","13年","14年","15年","16年","17年","18年","19年","20年","21年"},
                                                {"vcFinish","vcNum1","vcNum2","vcNum3","vcNXQF","dSSDate","vcDY","vcNum11","vcNum12","vcNum13","vcNum14","vcNum15","vcNum16","vcNum17","vcNum18","vcNum19","vcNum20","vcNum21"},
                                                {"",FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,"",FieldCheck.Date,"",FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num },
                                                {"0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0"},//最大长度设定,不校验最大长度用0
                                                {"1","1","1","1","1","1","1","1","1","1","1","1","1","1","1","1","1","1"},//最小长度设定,可以为空用0
                                                {"2","14","15","16","18","19","20","21","22","23","24","25","26","27","28","29","30","31"}//前台显示列号，从0开始计算,注意有选择框的是0
                    };
                    //需要判断时间区间先后关系的字段
                    string[,] strDateRegion = { };
                    string[,] strSpecialCheck = { };



                    List<Object> checkRes = ListChecker.validateList(listInfoData, strField, strDateRegion, strSpecialCheck, true, "FS0307");
                    if (checkRes != null)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = ListChecker.listToString(checkRes);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }

                string Msg = "";
                fs0307_logic.InsertUnitApi(listInfoData, loginInfo.UserId, ref Msg);

                if (!string.IsNullOrWhiteSpace(Msg))
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = Msg;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0706", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
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
                strSql.Append("   select vcName from TCode where vcCodeId='" + strCodeId + "'  ORDER BY vcMeaning      \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString(), "TK");
            }
            catch (Exception ex)
            {
                throw ex;
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
                fs0307_logic.DelApi(listInfoData);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0707", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
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

        #region SpreadJs传过来的日期格式OADate格式转文本格式
        public void OADateConvert(ref List<Dictionary<string, Object>> listInfoData)
        {
            for (int i = 0; i < listInfoData.Count; i++)
            {
                Dictionary<string, Object> dic = (Dictionary<string, Object>)listInfoData[i];
                for (int j = 0; j < dic.Count; j++)
                {
                    var item = dic.ElementAt(j);
                    if (item.Value != null && item.Value.ToString().IndexOf("OADate(") != -1)
                    {
                        string strTemp = item.Value.ToString().Substring(item.Value.ToString().IndexOf("OADate(") + 7, item.Value.ToString().Length - item.Value.ToString().IndexOf("OADate(") - 7 - 1 - 1);
                        DateTime d = System.DateTime.FromOADate(Convert.ToInt32(strTemp));
                        dic[item.Key] = d.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }
            }
        }
        #endregion
    }
}