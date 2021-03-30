using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Text.RegularExpressions;
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

namespace SPPSApi.Controllers.G06
{
    [Route("api/FS0607/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0607Controller : BaseController
    {
        FS0607_Logic fs0607_Logic = new FS0607_Logic();
        private readonly string FunctionID = "FS0607";

        private readonly IWebHostEnvironment _webHostEnvironment;


        public FS0607Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        #region 绑定
        //[HttpPost]
        //[EnableCors("any")]
        //public string bindConst()
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
        //    try
        //    {
        //        DataTable dt = fs0607_Logic.BindConst();
        //        List<Object> dataList = ComFunction.convertToResult(dt, new string[] { "vcCodeId", "vcCodeName" });
        //        apiResult.code = ComConstant.SUCCESS_CODE;
        //        apiResult.data = dataList;
        //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //    }
        //    catch (Exception ex)
        //    {
        //        ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0501", ex, loginInfo.UserId);
        //        apiResult.code = ComConstant.ERROR_CODE;
        //        apiResult.data = "绑定常量列表失败";
        //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //    }
        //}
        #endregion

        #region 检索
        [HttpPost]
        [EnableCors("any")]
        public string searchApi([FromBody] dynamic data)
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
            string vcSupplier_id = dataForm.vcSupplier_id == null ? "" : dataForm.vcSupplier_id;
            string vcWorkArea = dataForm.vcWorkArea == null ? "" : dataForm.vcWorkArea;
            string dBeginDate = dataForm.dBeginDate == null ? "" : dataForm.dBeginDate;
            string dEndDate = dataForm.dEndDate == null ? "" : dataForm.dEndDate;
            string vcMemo = dataForm.vcMemo == null ? "" : dataForm.vcMemo;
            try
            {
                DataTable dt = fs0607_Logic.Search(vcSupplier_id, vcWorkArea, dBeginDate,dEndDate,vcMemo);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0701", ex, loginInfo.UserId);
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
            string vcSupplier_id = dataForm.vcSupplier_id == null ? "" : dataForm.vcSupplier_id;
            string vcWorkArea = dataForm.vcWorkArea == null ? "" : dataForm.vcWorkArea;
            string dBeginDate = dataForm.dBeginDate == null ? "" : dataForm.dBeginDate;
            string dEndDate = dataForm.dEndDate == null ? "" : dataForm.dEndDate;
            string vcMemo = dataForm.vcMemo == null ? "" : dataForm.vcMemo;
            try
            {
                DataTable dt = fs0607_Logic.Search(vcSupplier_id, vcWorkArea, dBeginDate, dEndDate, vcMemo);
                string[] head = new string[] { };
                string[] field = new string[] { };
                //[vcPartNo], [dBeginDate], [dEndDate]
                head = new string[] { "供应商代码", "工区", "开始日", "结束日", "备注" };
                field = new string[] { "vcSupplier_id", "vcWorkArea", "dBeginDate", "dEndDate","vcMemo" };
                string msg = string.Empty;
                //string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0309_Export.xlsx", 2, loginInfo.UserId, FunctionID);
                //string filepath = ComFunction.DataTableToExcel(head, field, dt, ".", loginInfo.UserId, FunctionID, ref msg);
                string filepath = ComFunction.generateExcelWithXlt(dt, field, _webHostEnvironment.ContentRootPath, "FS0607_Data.xlsx", 1, loginInfo.UserId, FunctionID, true);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0702", ex, loginInfo.UserId);
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
                    string[,] strField = new string[,] {{"供应商代码", "工区", "开始日", "结束日","备注"},
                                                {"vcSupplier_id", "vcWorkArea", "dBeginDate", "dEndDate","vcMemo"},
                                                {"","","","","" },
                                                {"4","50","0","0","500"},//最大长度设定,不校验最大长度用0
                                                {"4","1","1","1","0"},//最小长度设定,可以为空用0
                                                {"1","2","3","4","5"}//前台显示列号，从0开始计算,注意有选择框的是0
                         };
                    //需要判断时间区间先后关系的字段
                    string[,] strDateRegion = { { "dBeginDate", "dEndDate" } };
                    string[,] strSpecialCheck = { //例子-变更事项字段，当它为新设时，号旧必须为号口，旧型开始、旧型结束、旧型持续开始必须为空
                        
                          };

                    List<Object> checkRes = ListChecker.validateList(listInfoData, strField, strDateRegion, strSpecialCheck, true, "FS0607");
                    if (checkRes != null)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = checkRes;
                        apiResult.flag = Convert.ToInt32(ERROR_FLAG.单元格定位提示);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    if (bAddFlag == true)
                    {//新增
                        string dBeginDate = listInfoData[i]["dBeginDate"].ToString().Replace("/", "").Substring(0, 6);//判断是不是同一个月
                        string dEndDate = listInfoData[i]["dEndDate"].ToString().Replace("/", "").Substring(0, 6);//判断是不是同一个月
                        if (dBeginDate != dEndDate)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "新增供应商：" + listInfoData[i]["vcSupplier_id"] + "工区：" + listInfoData[i]["vcWorkArea"] + "纳入开始日和纳入结束日必须同一个月！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        string dBeginDate = listInfoData[i]["dBeginDate"].ToString().Replace("/", "").Substring(0, 6);//判断是不是同一个月
                        string dEndDate = listInfoData[i]["dEndDate"].ToString().Replace("/", "").Substring(0, 6);//判断是不是同一个月
                        if (dBeginDate != dEndDate)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "修改供应商：" + listInfoData[i]["vcSupplier_id"] + "工区：" + listInfoData[i]["vcWorkArea"] + "纳入开始日和纳入结束日必须同一个月！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                    }
                }
                #region 判断新增的数据本身是否有重复的 和数据库内部关键字是否有重复的
                //本身判重 "iAutoId", "vcSupplier_id", "vcWorkArea", "dBeginDate", "dEndDate", "vcOperatorID", "dOperatorTime"
                DataTable dtadd = new DataTable();
                dtadd.Columns.Add("vcSupplier_id");
                dtadd.Columns.Add("vcWorkArea");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (listInfoData[i]["vcAddFlag"].ToString().ToLower() == "true")
                    {
                        DataRow dr = dtadd.NewRow();
                        dr["vcSupplier_id"] = listInfoData[i]["vcSupplier_id"].ToString();
                        dr["vcWorkArea"] = listInfoData[i]["vcWorkArea"].ToString();
                        dtadd.Rows.Add(dr);
                    }
                }

                if (dtadd.Rows.Count > 0)
                {
                    if (dtadd.Rows.Count > 1)
                    {
                        for (int i = 0; i < dtadd.Rows.Count; i++)
                        {
                            for (int j = i + 1; j < dtadd.Rows.Count; j++)
                            {
                                if (dtadd.Rows[i]["vcSupplier_id"].ToString() == dtadd.Rows[j]["vcSupplier_id"].ToString() &&
                                    dtadd.Rows[i]["vcWorkArea"].ToString() == dtadd.Rows[j]["vcWorkArea"].ToString())
                                {
                                    apiResult.code = ComConstant.ERROR_CODE;
                                    apiResult.data = "供应商" + dtadd.Rows[i]["vcSupplier_id"].ToString() + "、工区" + dtadd.Rows[i]["vcSupplier_id"].ToString() + "存在重复项，请确认！";
                                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                                }
                            }
                        }
                    }

                    //数据库验证  true  存在重复项
                    DataTable dt = fs0607_Logic.CheckDistinctByTable(dtadd);
                    if (dt.Rows.Count > 0)
                    {
                        string errMsg = string.Empty;
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            errMsg += "供应商" + dt.Rows[i]["vcSupplier_id"].ToString() + "、工区" + dt.Rows[i]["vcWorkArea"].ToString() + ",";
                        }
                        errMsg.Substring(0, errMsg.LastIndexOf(","));
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = errMsg + "存在重复项，请确认！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }

                #endregion
                string strErrorPartId = "";
                fs0607_Logic.Save(listInfoData, loginInfo.UserId, ref strErrorPartId);
                if (strErrorPartId != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "保存失败，以下供应商代码存在重叠：<br/>" + strErrorPartId;
                    apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0703", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }

        }
        #endregion
        #region 删除
        [HttpPost]
        [EnableCors("any")]
        public string deleteApi([FromBody] dynamic data)
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
                fs0607_Logic.Del(listInfoData, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0704", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 一括赋予
        [HttpPost]
        [EnableCors("any")]
        public string allInstallApi([FromBody] dynamic data)
        {
            //验证是否登录
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            //以下开始业务处理
            //以下开始业务处理
            ApiResult apiResult = new ApiResult();
            dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
            JArray listInfo = dataForm.parentFormSelectItem.multipleSelection;
            List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
            string dBeginDate = dataForm.allInstallForm.dBeginDate == null ? "" : dataForm.allInstallForm.dBeginDate;
            string dEndDate = dataForm.allInstallForm.dEndDate == null ? "" : dataForm.allInstallForm.dEndDate;
            try
            {
                if (dBeginDate.Length == 0 || dEndDate.Length == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "一括赋予纳入日-开始、纳入日-结束都不能为空,请确认！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (dBeginDate.Replace("/", "").Replace("-", "").Substring(0, 6) != dEndDate.Replace("/", "").Replace("-", "").Substring(0, 6))
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "纳入日-开始和纳入日-结束必须同一个月！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (int.Parse(dBeginDate.Replace("/", "").Replace("-", "")) > int.Parse(dEndDate.Replace("/", "").Replace("-", "").ToString()))
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "纳入日-开始必须小于纳入日-结束！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (listInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选择一条数据,进行一括赋予，请确认！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                fs0607_Logic.allInstall(listInfoData, Convert.ToDateTime(dBeginDate), Convert.ToDateTime(dEndDate), loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0605", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "一括赋予失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}

