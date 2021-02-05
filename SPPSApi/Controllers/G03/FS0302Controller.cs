using System;
using System.Collections.Generic;
using System.Data;
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
    [Route("api/FS0302/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0302Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string FunctionID = "FS0302";
        FS0302_Logic fs0302_logic = new FS0302_Logic();

        public FS0302Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

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
                    DataTable dtAll = fs0302_logic.SearchApi(fileName);
                    initSearchCash(strSearchKey, dtAll);
                    dt = getSearchResultByCash(strSearchKey, iPage, iPageSize, ref pageTotal);
                }

                DtConverter dtConverter = new DtConverter();

                dtConverter.addField("selected", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("oldDuplicate", ConvertFieldType.BoolType, null);
                dtConverter.addField("newDuplicate", ConvertFieldType.BoolType, null);

                dtConverter.addField("dOldProjTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dNewProjTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dOperatorTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dHandleTime", ConvertFieldType.DateType, "yyyy/MM/dd");

                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                apiResult.field1 = pageTotal;//这块需要把总页数返回
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0201", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

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

                List<string> dataList_C006 = convertTCodeToResult(getTCode("C006"));//原单位
                List<string> dataList_C014 = convertTCodeToResult(getTCode("C014"));//完成状态
                List<string> dataList_C002 = convertTCodeToResult(getTCode("C002"));//变更事项


                res.Add("C006", dataList_C006);
                res.Add("C014", dataList_C014);
                res.Add("C002", dataList_C002);


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
                    string[,] strField = new string[,] {{"完成状态"     ,"原单位名"    ,"车型代码"   ,"变更事项(统合)" ,"备注" },
                                                        {"FinishState" ,"vcUnit"    ,"vcCarType","THChange"    ,"vcRemark"},
                                                        {""            ,""       ,FieldCheck.NumChar,""  ,""  },
                                                        {"0"           ,"0"      ,"10"       ,"0"           ,"0"  },//最大长度设定,不校验最大长度用0
                                                        {"0"           ,"1"      ,"1"        ,"1"           ,"0"  },//最小长度设定,可以为空用0
                                                        {"4"           ,"5"      ,"7"        ,"8"           ,"10" }//前台显示列号，从0开始计算,注意有选择框的是0
                    };
                    //需要判断时间区间先后关系的字段
                    string[,] strDateRegion = { };
                    string[,] strSpecialCheck = { };

                    List<Object> checkRes = ListChecker.validateList(listInfoData, strField, strDateRegion, strSpecialCheck, true, "FS0303");
                    if (checkRes != null)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = ListChecker.listToString(checkRes);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }

                string strErrorPartId = "";
                fs0302_logic.Save(listInfoData, loginInfo.UserId);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0202", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
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

            string fileName = dataForm.fileName;

            try
            {
                DataTable dt = fs0302_logic.SearchApi(fileName);
                string resMsg = "";
                string[] fields = { "vcSPINo", "vcPart_Id_old", "vcPart_Id_new", "FinishState", "vcUnit", "vcDiff", "vcCarType", "THChange", "vcDD", "vcRemark", "vcChange", "vcBJDiff", "vcDT", "vcPartName", "vcStartYearMonth", "vcFXDiff", "vcFXNo", "vcOldProj", "dOldProjTime", "vcNewProj", "dNewProjTime", "vcCZYD", "dHandleTime" };
                //string[] fields = { "vcSPINo", "vcPart_Id_old", "vcPart_Id_new", "FinishState", "vcUnit", "vcDiff", "vcCarType", "THChange", "vcDD", "vcRemark", "vcChange", "vcBJDiff", "vcDT", "vcPartName", "vcStartYearMonth", "vcFXDiff", "vcFXNo", "vcOldProj", "dOldProjTime", "vcNewProj", "dNewProjTime", "vcCZYD", "dHandleTime", "vcSheetName", "vcFileName" };

                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0302.xlsx", 1, loginInfo.UserId, FunctionID);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0203", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 织入原单位

        [HttpPost]
        [EnableCors("any")]
        public string weaveUnitApi([FromBody] dynamic data)
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

                if (listInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选中一条织入原单位！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                //开始数据验证
                if (listInfoData.Count != 0)
                {
                    string[,] strField = new string[,] {{"完成状态"     ,"原单位名" , "区分"    ,"车型代码"   ,"变更事项(统合)" ,"备注" },
                        {"FinishState" ,"vcUnit" ,"vcDiff"   ,"vcCarType","THChange"    ,"vcRemark"},
                        {""            ,""       ,FieldCheck.Num,FieldCheck.NumChar,""  ,""  },
                        {"0"           ,"0"      ,"0"        ,"10"       ,"0"           ,"0"  },//最大长度设定,不校验最大长度用0
                        {"0"           ,"1"      ,"0"        ,"1"        ,"1"           ,"0"  },//最小长度设定,可以为空用0
                        {"4"           ,"5"      ,"6"        ,"7"        ,"8"           ,"10" }//前台显示列号，从0开始计算,注意有选择框的是0
                    };
                    //需要判断时间区间先后关系的字段
                    string[,] strDateRegion = { };
                    string[,] strSpecialCheck = { };

                    List<Object> checkRes = ListChecker.validateList(listInfoData, strField, strDateRegion, strSpecialCheck, true, "FS0302");
                    if (checkRes != null)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = ListChecker.listToString(checkRes);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }

                string refmsg = "";
                fs0302_logic.weaveUnit(listInfoData, loginInfo.UserId, loginInfo.UnitCode, ref refmsg);

                if (!string.IsNullOrWhiteSpace(refmsg))
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = refmsg;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0204", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "织入原单位失败";
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

    }
}