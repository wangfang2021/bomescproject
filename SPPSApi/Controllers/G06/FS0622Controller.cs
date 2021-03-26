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
    [Route("api/FS0622/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0622Controller : BaseController
    {
        FS0622_Logic fs0622_Logic = new FS0622_Logic();
        private readonly string FunctionID = "FS0622";

        private readonly IWebHostEnvironment _webHostEnvironment;


        public FS0622Controller(IWebHostEnvironment webHostEnvironment)
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

                List<Object> dataList_Group = ComFunction.convertAllToResult(fs0622_Logic.GetGroupDt());

                res.Add("Group", dataList_Group);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2201", ex, loginInfo.UserId);
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
            string vcGroup = dataForm.vcGroup == null ? "" : dataForm.vcGroup;
            string vcPartNo = dataForm.vcPartNo == null ? "" : dataForm.vcPartNo;
            string vcFluctuationRange = dataForm.vcFluctuationRange == null ? "" : dataForm.vcFluctuationRange;
            try
            {
                DataTable dt = fs0622_Logic.Search(vcGroup, vcPartNo, vcFluctuationRange);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("dOperatorTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2202", ex, loginInfo.UserId);
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
            string vcGroup = dataForm.vcGroup == null ? "" : dataForm.vcGroup;
            string vcPartNo = dataForm.vcPartNo == null ? "" : dataForm.vcPartNo;
            string vcFluctuationRange = dataForm.vcFluctuationRange == null ? "" : dataForm.vcFluctuationRange;
            try
            {
                DataTable dt = fs0622_Logic.Search(vcGroup, vcPartNo, vcFluctuationRange);
                string[] head = new string[] { };
                string[] field = new string[] { };
                //[vcPartNo], [dBeginDate], [dEndDate]
                head = new string[] {  "品番","组别", "波动比例(%)","定义" };
                field = new string[] { "vcPartNo","vcGroupName",  "vcFluctuationRange", "vcDefinition" };
                string msg = string.Empty;
                //string filepath = ComFunction.DataTableToExcel(head, field, dt, ".", loginInfo.UserId, FunctionID, ref msg);
                string filepath = ComFunction.generateExcelWithXlt(dt, field, _webHostEnvironment.ContentRootPath, "FS0622_Data.xlsx", 1, loginInfo.UserId, FunctionID,true);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2203", ex, loginInfo.UserId);
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
                    string[,] strField = new string[,] {{"品番","组别",  },
                                                { "vcPartNo","vcGroupName", },
                                                {"",""},
                                                {"12","0"},//最大长度设定,不校验最大长度用0
                                                {"12","1"},//最小长度设定,可以为空用0
                                                {"1","2"}//前台显示列号，从0开始计算,注意有选择框的是0
                         };
                    //需要判断时间区间先后关系的字段
                    string[,] strDateRegion = { };
                    string[,] strSpecialCheck = { //例子-变更事项字段，当它为新设时，号旧必须为号口，旧型开始、旧型结束、旧型持续开始必须为空
                        
                          };

                    List<Object> checkRes = ListChecker.validateList(listInfoData, strField, strDateRegion, strSpecialCheck, true, "FS0622");
                    if (checkRes != null)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = checkRes;
                        apiResult.flag = Convert.ToInt32(ERROR_FLAG.单元格定位提示);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }

                string strErrorPartId = "";
                fs0622_Logic.Save(listInfoData, loginInfo.UserId, ref strErrorPartId);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2204", ex, loginInfo.UserId);
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
                fs0622_Logic.Del(listInfoData, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2205", ex, loginInfo.UserId);
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
            string vcGroupId = dataForm.vcGroupId == null ? "" : dataForm.vcGroupId;
            string vcFluctuationRange = dataForm.vcFluctuationRange == null ? "" : dataForm.vcFluctuationRange;
            try
            {
                if (vcGroupId.Length == 0 || vcFluctuationRange.Length == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "一括赋予组别、赋予值都不能为空,请确认！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                fs0622_Logic.allInstall(vcGroupId, vcFluctuationRange, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2206", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "一括赋予失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 新增组别
        [HttpPost]
        [EnableCors("any")]
        public string addZbConfirmApi([FromBody] dynamic data)
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
            string vcGroupName = dataForm.vcGroupName == null ? "" : dataForm.vcGroupName;
            string vcDefinition = dataForm.vcDefinition == null ? "" : dataForm.vcDefinition;
            try
            {
                if (vcGroupName.Length == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "组别不能为空,请确认！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //true  存在
                if (fs0622_Logic.CheckGroup(vcGroupName))
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "组别名称已经存在不能重复追加,请确认！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }
                fs0622_Logic.addZbConfirm(vcGroupName, vcDefinition, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2207", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "新增组别失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 新增波动比例
        [HttpPost]
        [EnableCors("any")]
        public string addConfirmApi([FromBody] dynamic data)
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
            string vcGroupId = dataForm.vcGroupId == null ? "" : dataForm.vcGroupId;
            string vcPartNo = dataForm.vcPartNo == null ? "" : dataForm.vcPartNo;
            string vcFluctuationRange = dataForm.vcFluctuationRange == null ? "" : dataForm.vcFluctuationRange;
            try
            {
                if (vcGroupId.Length == 0|| vcPartNo.Length == 0|| vcFluctuationRange.Length == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "组别、品番、波动比例不能为空,请确认！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //true  存在
                if (fs0622_Logic.CheckGroupbyGroupIdAndvcPartNo(vcGroupId, vcPartNo))
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "组别名称已经存在不能重复追加,请确认！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }

                fs0622_Logic.addConfirm(vcGroupId, vcPartNo, vcFluctuationRange, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2207", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "新增组别失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}

