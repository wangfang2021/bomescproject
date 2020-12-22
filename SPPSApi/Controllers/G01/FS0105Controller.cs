using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
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
 


namespace SPPSApi.Controllers.G00
{
    [Route("api/FS0105/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0105Controller : BaseController
    {
        FS0105_Logic fs0105_Logic = new FS0105_Logic();
        private readonly string FunctionID = "FS0105";
        
        private readonly IWebHostEnvironment _webHostEnvironment;
        

        public FS0105Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        #region 绑定包装厂
        [HttpPost]
        [EnableCors("any")]
        public string bindConst()
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
                DataTable dt = fs0105_Logic.BindConst();
                List<Object> dataList = ComFunction.convertToResult(dt, new string[] { "vcCodeId", "vcCodeName" });
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0501", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "绑定常量列表失败";
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
            string vcCodeId = dataForm.vcCodeId == null ? "" : dataForm.vcCodeId;
           
            try
            {
                DataTable dt = fs0105_Logic.Search(vcCodeId);
                // TypeCode, TypeName, KeyCode, KeyValue, Sort, Memo
               
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0502", ex, loginInfo.UserId);
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
            string vcCodeId = dataForm.vcCodeId == null ? "" : dataForm.vcCodeId;
           
            try
            {
                DataTable dt = fs0105_Logic.Search(vcCodeId);
                string[] head = new string[] { };
                string[] field = new string[] { };
               

                head = new string[] { "常量区分代码", "常量区分名称", "Key键", "Key值", "备注" };
                field = new string[] { "vcCodeId", "vcCodeName", "vcValue", "vcName", "vcMeaning" };
                string msg = string.Empty;
                //string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0309_Export.xlsx", 2, loginInfo.UserId, FunctionID);
                string filepath = ComFunction.DataTableToExcel(head, field, dt, ".", loginInfo.UserId, FunctionID,ref msg);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0503", ex, loginInfo.UserId);
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
                #region 判断新增的数据本身是否有重复的 和数据库内部关键字是否有重复的

                //本身判重
                DataTable dtadd = new DataTable();
                //vcCodeId ,vcCodeName  ,vcValue ,vcName 
                dtadd.Columns.Add("vcCodeId");
                dtadd.Columns.Add("vcCodeName");
                dtadd.Columns.Add("vcValue");
                dtadd.Columns.Add("vcName");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (listInfoData[i]["vcAddFlag"].ToString().ToLower() == "true") {
                        DataRow dr = dtadd.NewRow();
                        dr["vcCodeId"] = listInfoData[i]["vcCodeId"].ToString();
                        dr["vcCodeName"] = listInfoData[i]["vcCodeName"].ToString();
                        dr["vcValue"] = listInfoData[i]["vcValue"].ToString();
                        dr["vcName"] = listInfoData[i]["vcName"].ToString();
                        dtadd.Rows.Add(dr);
                    }
                }
                
                if (dtadd.Rows.Count > 0)
                {
                    if(dtadd.Rows.Count>1)
                    { 
                        for (int i = 0; i < dtadd.Rows.Count; i++)
                        {
                            for (int j = i + 1; j < dtadd.Rows.Count; j++)
                            {
                                if (dtadd.Rows[i]["vcCodeId"].ToString() == dtadd.Rows[j]["vcCodeId"].ToString() &&
                                    dtadd.Rows[i]["vcValue"].ToString() == dtadd.Rows[j]["vcValue"].ToString())
                                {
                                    apiResult.code = ComConstant.ERROR_CODE;
                                    apiResult.data = "常量区代码" + dtadd.Rows[i]["vcCodeId"].ToString() + "、Key键" + dtadd.Rows[i]["vcValue"].ToString() + "存在重复项，请确认！";
                                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                                }
                            }
                        }
                    }
                    #region
                    //string[] columnArray = { "vcCodeId", "vcValue" };
                    //DataView dtaddView = dtadd.DefaultView;
                    //DataTable dtaddNew = dtaddView.ToTable(true, columnArray);
                    //if (dtadd.Rows.Count != dtaddNew.Rows.Count)
                    //{
                    //    apiResult.code = ComConstant.ERROR_CODE;
                    //    apiResult.data = "新增数据中有重复数据,请确认！";
                    //    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    //}
                    #endregion
                    //数据库验证  true  存在重复项
                    DataTable dt = fs0105_Logic.CheckDistinctByTable(dtadd);
                    if (dt.Rows.Count>0)
                    {
                        string errMsg = string.Empty;
                        for (int i=0;i<dt.Rows.Count;i++)
                        {
                            errMsg += "常量区代码" + dt.Rows[i]["vcCodeId"].ToString() + "、Key键" + dt.Rows[i]["vcValue"].ToString() + ",";
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
                    string[,] strField = new string[,] {{"常量区分代码","常量区分名称","Key键","Key值"},
                                                {"vcCodeId","vcCodeName","vcValue","vcName"},
                                                {"","","","" },
                                                {"50","100","40","200"},//最大长度设定,不校验最大长度用0
                                                {"1","1","1","1"},//最小长度设定,可以为空用0
                                                {"1","2","3","4"}//前台显示列号，从0开始计算,注意有选择框的是0
                    };
                    //需要判断时间区间先后关系的字段
                    string[,] strDateRegion = {  };
                    string[,] strSpecialCheck = { //例子-变更事项字段，当它为新设时，号旧必须为号口，旧型开始、旧型结束、旧型持续开始必须为空
                        
                    };



                    List<Object> checkRes = ListChecker.validateList(listInfoData, strField, strDateRegion, strSpecialCheck, true, "FS0105");
                    if (checkRes != null)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = checkRes;
                        apiResult.flag = Convert.ToInt32(ERROR_FLAG.单元格定位提示);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }

                string strErrorPartId = "";
                fs0105_Logic.Save(listInfoData, loginInfo.UserId, ref strErrorPartId);
                if (strErrorPartId != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "保存失败，以下常量区分代码和Key键区间存在重叠：<br/>" + strErrorPartId;
                    apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0503", ex, loginInfo.UserId);
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
                fs0105_Logic.Del(listInfoData, loginInfo.UserId);
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

    }
}
