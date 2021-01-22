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
    [Route("api/FS0606/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0606Controller : BaseController
    {
        FS0606_Logic fs0606_Logic = new FS0606_Logic();
        private readonly string FunctionID = "FS0606";

        private readonly IWebHostEnvironment _webHostEnvironment;


        public FS0606Controller(IWebHostEnvironment webHostEnvironment)
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
        //        DataTable dt = fs0606_Logic.BindConst();
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
            string vcPartNo = dataForm.vcPartNo == null ? "" : dataForm.vcPartNo;
            try
            {
                DataTable dt = fs0606_Logic.Search(vcPartNo);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("dBeginDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dEndDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0601", ex, loginInfo.UserId);
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
            string vcPartNo = dataForm.vcPartNo == null ? "" : dataForm.vcPartNo;

            try
            {
                DataTable dt = fs0606_Logic.Search(vcPartNo);
                string[] head = new string[] { };
                string[] field = new string[] { };
                //[vcPartNo], [dBeginDate], [dEndDate]
                head = new string[] { "品番", "开始日", "结束日"};
                field = new string[] { "vcPartNo", "dBeginDate", "dEndDate" };
                string msg = string.Empty;
                //string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0309_Export.xlsx", 2, loginInfo.UserId, FunctionID);
                string filepath = ComFunction.DataTableToExcel(head, field, dt, ".", loginInfo.UserId, FunctionID, ref msg);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0602", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
        #region
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
                                string dBeginDate = listInfoData[i]["dBeginDate"].ToString().Replace("-", "").Substring(0, 6);//判断是不是同一个月
                                string dEndDate = listInfoData[i]["dEndDate"].ToString().Replace("-", "").Substring(0, 6);//判断是不是同一个月
                                if (dBeginDate != dEndDate)
                                {
                                    apiResult.code = ComConstant.ERROR_CODE;
                                    apiResult.data = "新增品番：" + listInfoData[i]["vcPartNo"] + "开始日和结束日必须同一个月！";
                                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                                }
                            }
                            else if (bAddFlag == false && bModFlag == true)
                            {//修改
                                hasFind = true;
                                string dBeginDate = listInfoData[i]["dBeginDate"].ToString().Replace("-", "").Substring(0, 6);//判断是不是同一个月
                                string dEndDate = listInfoData[i]["dEndDate"].ToString().Replace("-", "").Substring(0, 6);//判断是不是同一个月
                                if (dBeginDate != dEndDate)
                                {
                                    apiResult.code = ComConstant.ERROR_CODE;
                                    apiResult.data = "修改品番：" + listInfoData[i]["vcPartNo"] + "开始日和结束日必须同一个月！";
                                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                                }
                            }
                        }
                        if (!hasFind)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "最少有一个编辑行！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        #region 判断新增的数据本身是否有重复的 和数据库内部关键字是否有重复的
                        //本身判重
                        DataTable dtadd = new DataTable();
                        dtadd.Columns.Add("vcPartNo");
                        for (int i = 0; i < listInfoData.Count; i++)
                        {
                            if (listInfoData[i]["vcAddFlag"].ToString().ToLower() == "true")
                            {
                                DataRow dr = dtadd.NewRow();
                                dr["vcPartNo"] = listInfoData[i]["vcPartNo"].ToString();
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
                                        if (dtadd.Rows[i]["vcPartNo"].ToString() == dtadd.Rows[j]["vcPartNo"].ToString() )
                                        {
                                            apiResult.code = ComConstant.ERROR_CODE;
                                            apiResult.data = "品番" + dtadd.Rows[i]["vcPartNo"].ToString() +  "存在重复项，请确认！";
                                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                                        }
                                    }
                                }
                            }
                            
                            //数据库验证  true  存在重复项
                            DataTable dt = fs0606_Logic.CheckDistinctByTable(dtadd);
                            if (dt.Rows.Count > 0)
                            {
                                string errMsg = string.Empty;
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    errMsg += "品番" + dt.Rows[i]["vcPartNo"].ToString() + ",";
                                }
                                errMsg.Substring(0, errMsg.LastIndexOf(","));
                                apiResult.code = ComConstant.ERROR_CODE;
                                apiResult.data = errMsg + "存在重复项，请确认！";
                                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                            }
                        }

                        #endregion

                        //开始数据验证
                        if (hasFind)
                        {
                            string[,] strField = new string[,] {{"vcPartNo","开始日","结束日"},
                                                {"vcPartNo", "dBeginDate", "dEndDate"},
                                                {"","","" },
                                                {"12","0","0"},//最大长度设定,不校验最大长度用0
                                                {"1","1","1"},//最小长度设定,可以为空用0
                                                {"1","2","3"}//前台显示列号，从0开始计算,注意有选择框的是0
                         };
                            //需要判断时间区间先后关系的字段
                            string[,] strDateRegion = { };
                            string[,] strSpecialCheck = { //例子-变更事项字段，当它为新设时，号旧必须为号口，旧型开始、旧型结束、旧型持续开始必须为空
                        
                          };

                            List<Object> checkRes = ListChecker.validateList(listInfoData, strField, strDateRegion, strSpecialCheck, true, "FS0605");
                            if (checkRes != null)
                            {
                                apiResult.code = ComConstant.ERROR_CODE;
                                apiResult.data = checkRes;
                                apiResult.flag = Convert.ToInt32(ERROR_FLAG.单元格定位提示);
                                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                            }
                        }
                        
                        string strErrorPartId = "";
                        fs0606_Logic.Save(listInfoData, loginInfo.UserId, ref strErrorPartId);
                        if (strErrorPartId != "")
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "保存失败，以下品番存在重叠：<br/>" + strErrorPartId;
                            apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        apiResult.code = ComConstant.SUCCESS_CODE;
                        apiResult.data = null;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    catch (Exception ex)
                    {
                        ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0603", ex, loginInfo.UserId);
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
                fs0606_Logic.Del(listInfoData, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0604", ex, loginInfo.UserId);
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
                if (dBeginDate.Length==0|| dEndDate.Length == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "一括赋予开始日、结束日都不能为空,请确认！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (listInfoData.Count==0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选择一条数据,进行一括赋予，请确认！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                fs0606_Logic.allInstall (listInfoData,Convert.ToDateTime(dBeginDate), Convert.ToDateTime(dEndDate), loginInfo.UserId);
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

