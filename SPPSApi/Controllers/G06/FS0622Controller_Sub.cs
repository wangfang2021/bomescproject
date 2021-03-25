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

namespace SPPSApi.Controllers.G06
{
    [Route("api/FS0622_Sub/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0622Controller_Sub : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0622_Logic fs0622_Logic = new FS0622_Logic();
        private readonly string FunctionID = "FS0622";

        public FS0622Controller_Sub(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

 
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
            //dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
            //string strBegin = dataForm.dBegin;
            //string strEnd = dataForm.dEnd;
 
            try
            {
                DataTable dt = fs0622_Logic.Search_Sub();
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2221", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索订单区分失败";
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
                #region 判断新增的数据本身是否有重复的 和数据库内部关键字是否有重复的
                //本身判重 "iAutoId", "vcSupplier_id", "vcWorkArea", "dBeginDate", "dEndDate", "vcOperatorID", "dOperatorTime"
                DataTable dtadd = new DataTable();
                dtadd.Columns.Add("vcGroupName"); 
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (listInfoData[i]["vcAddFlag"].ToString().ToLower() == "true")
                    {
                        DataRow dr = dtadd.NewRow();
                        dr["vcGroupName"] = listInfoData[i]["vcGroupName"].ToString();
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
                                if (dtadd.Rows[i]["vcGroupName"].ToString().Trim() == dtadd.Rows[j]["vcGroupName"].ToString().Trim())
                                {
                                    apiResult.code = ComConstant.ERROR_CODE;
                                    apiResult.data = "组别" + dtadd.Rows[i]["vcGroupName"].ToString() + "存在重复项，请确认！";
                                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                                }
                            }
                        }
                    }

                    //数据库验证  true  存在重复项
                    DataTable dt = fs0622_Logic.CheckDistinctByTable(dtadd);
                    if (dt.Rows.Count > 0)
                    {
                        string errMsg = string.Empty;
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            errMsg += "组别" + dt.Rows[i]["vcGroupName"].ToString() + ",";
                        }
                        errMsg.Substring(0, errMsg.LastIndexOf(","));
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = errMsg + "存在重复项，请确认！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }

                #endregion
                //开始数据验证vcGroupName,vcDefinition,vcFluctuationRange
                if (hasFind)
                {
                    string[,] strField = new string[,] {{"组别","波动比例(%)","定义"},
                                                {"vcGroupName","vcFluctuationRange","vcDefinition"},
                                                {"","","",},
                                                {"100","18","500"},//最大长度设定,不校验最大长度用0
                                                {"1","1","0"},//最小长度设定,可以为空用0
                                                {"1","2","3"}//前台显示列号，从0开始计算,注意有选择框的是0
                    };
                    //需要判断时间区间先后关系的字段
                    string[,] strDateRegion = { };

                    List<Object> checkRes = ListChecker.validateList(listInfoData, strField, strDateRegion,null,true,"FS0622_Sub");

                    if (checkRes != null)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = checkRes;
                        apiResult.flag = Convert.ToInt32(ERROR_FLAG.单元格定位提示);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }

                string strErrorName = "";
                fs0622_Logic.Save_Sub(listInfoData, loginInfo.UserId,ref strErrorName);
                if (strErrorName != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "保存失败，以下订单区分存在重叠：<br/>"+ strErrorName;
                    apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2302", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存订单区分失败";
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
                //验证该组别下面是否存在品番
                DataTable dtPartNo = fs0622_Logic.GetSubPartByGroup(listInfoData);
                fs0622_Logic.Del_Sub(listInfoData, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06E2303", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除订单区分失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

    }
}
