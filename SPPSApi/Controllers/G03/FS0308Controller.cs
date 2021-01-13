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
    [Route("api/FS0308/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0308Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string FunctionID = "FS0308";
        FS0308_Logic fs0308_logic = new FS0308_Logic();

        public FS0308Controller(IWebHostEnvironment webHostEnvironment)
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

                List<Object> dataList_C006 = ComFunction.convertAllToResult(ComFunction.getTCode("C006"));//原单位
                List<Object> dataList_C016 = ComFunction.convertAllToResult(ComFunction.getTCode("C016"));//包装事业体
                List<string> dataList_C024_Excel = convertTCodeToResult(getTCode("C024"));//变更事项


                res.Add("C006", dataList_C006);
                res.Add("C016", dataList_C016);
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

            string strYear = dataForm.vcYear == null ? "" : dataForm.vcYear;

            if (!string.IsNullOrWhiteSpace(strYear))
            {
                strYear = Convert.ToDateTime(strYear).AddHours(8).ToString("yyyy");
            }

            string SYT = dataForm.vcSYTCode == null ? "" : dataForm.vcSYTCode;


            string Receiver = loginInfo.UnitCode;

            JArray listInfo = dataForm.vcOriginCompany;
            List<string> origin = listInfo.ToObject<List<string>>();
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
                    DataTable dtAll = fs0308_logic.searchApi(strYear, SYT, Receiver, origin);
                    initSearchCash(strSearchKey, dtAll);
                    dt = getSearchResultByCash(strSearchKey, iPage, iPageSize, ref pageTotal);
                }

                DtConverter dtConverter = new DtConverter();

                dtConverter.addField("selected", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("dTimeFrom", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dJiuBegin", ConvertFieldType.DateType, "yyyy/MM/dd");

                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                apiResult.field1 = pageTotal;//这块需要把总页数返回
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0802", ex, loginInfo.UserId);
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

            string strYear = dataForm.strYear == null ? "" : dataForm.strYear;
            if (!string.IsNullOrWhiteSpace(strYear))
            {
                strYear = Convert.ToDateTime(strYear).AddHours(8).ToString("yyyy");
            }
            string SYT = dataForm.vcSYTCode == null ? "" : dataForm.vcSYTCode;

            string Receiver = loginInfo.UnitCode;
            JArray listInfo = dataForm.vcOriginCompany;
            List<string> origin = listInfo.ToObject<List<string>>();
            try
            {
                DataTable dt = fs0308_logic.searchApi(strYear, SYT, Receiver, origin);

                string[] fields = { "vcYear", "vcSupplier_id", "vcPart_id", "vcPartNameEn", "vcInOutflag", "vcCarTypeDev", "dJiuBegin", "vcRemark", "vcOld10", "vcOld9", "vcOld7", "vcPM", "vcNum1", "vcNum2", "vcNum3", "vcNXQF", "dTimeFrom", "vcDY", "vcNum11", "vcNum12", "vcNum13", "vcNum14", "vcNum15", "vcNum16", "vcNum17", "vcNum18", "vcNum19", "vcNum20", "vcNum21", "vcSYTCode", "vcReceiver", "vcOriginCompany" };
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0308.xlsx", 1, loginInfo.UserId, FunctionID);
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
                bool hasFind = false;//是否找到需要新增或者修改的数据
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    if (bModFlag == true)
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
                    string[,] strField = new string[,] {{"进度","1年","2年","3年","年限区分","实施时间","对应可否","11年","12年","13年","14年","15年","16年","17年","18年","19年","20年","21年"},
                                                {"vcFinish","vcNum1","vcNum2","vcNum3","vcNXQF","dTimeFrom","vcDY","vcNum11","vcNum12","vcNum13","vcNum14","vcNum15","vcNum16","vcNum17","vcNum18","vcNum19","vcNum20","vcNum21"},
                                                {"",FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,"",FieldCheck.Date,"",FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num },
                                                {"0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0"},//最大长度设定,不校验最大长度用0
                                                {"1","0","0","0","0","0","1","0","0","0","0","0","0","0","0","0","0","0"},//最小长度设定,可以为空用0
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

                fs0308_logic.SaveApi(listInfoData, loginInfo.UserId);

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


    }
}