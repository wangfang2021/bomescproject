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

namespace SPPSApi.Controllers.G12
{
    [Route("api/FS1207_Sub1/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1207Controller_Sub1 : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        FS1207_Logic logic = new FS1207_Logic();
        private readonly string FunctionID = "FS1207";

        public FS1207Controller_Sub1(IWebHostEnvironment webHostEnvironment)
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
                Dictionary<string, Object> res = new Dictionary<string, Object>();
                List<Object> dataList_SaleUserSource = ComFunction.convertAllToResult(logic.ddlSaleUser());
                res.Add("SaleUserSource", dataList_SaleUserSource);
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
            string vcMon = dataForm.vcMon;
            string vcPartsNo = dataForm.vcPartsNo;
            string vcYesOrNo = dataForm.vcYesOrNo;
            vcMon = vcMon == null ? "" : vcMon;
            vcPartsNo = vcPartsNo == null ? "" : vcPartsNo;
            vcYesOrNo = vcYesOrNo == null ? "" : vcYesOrNo;
            if (!string.IsNullOrEmpty(vcPartsNo))
            {
                vcPartsNo = vcPartsNo.Replace("-", "").ToString();
            }
            try
            {
                string _msg;
                DataTable dt = logic.GetFzjsRenders(vcMon, vcPartsNo, vcYesOrNo, out _msg);
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
            ApiResult apiResult = new ApiResult();
            //以下开始业务处理
            dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
            string vcMon = dataForm.vcMon;
            string vcPartsNo = dataForm.vcPartsNo;
            string vcYesOrNo = dataForm.vcYesOrNo;
            vcMon = vcMon == null ? "" : vcMon;
            vcPartsNo = vcPartsNo == null ? "" : vcPartsNo;
            vcYesOrNo = vcYesOrNo == null ? "" : vcYesOrNo;
            if (!string.IsNullOrEmpty(vcPartsNo))
            {
                vcPartsNo = vcPartsNo.Replace("-", "").ToString();
            }
            try
            {
                string _msg;
                DataTable dt = logic.GetFzjsRenders(vcMon, vcPartsNo, vcYesOrNo, out _msg);
                string[] fields = { "vcMonth", "vcPartsNo","iSRNum","Total","iXZNum","iBYNum",
                "iFZNum","syco","iCONum","iFlag","vcPartsNoFZ", "vcSource"
                };
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS1207_Sub1.xlsx", 1, loginInfo.UserId, FunctionID);
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
                    hasFind = true;
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
                    string[,] strField = new string[,] {{"修正数","发注数","本月CO数","对象月","品番"},
                                                {"iXZNum","iFZNum","iCONum","vcMonth","vcPartsNo"},
                                                {FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,"",""},
                                                {"10","10","10","20","20"},//最大长度设定,不校验最大长度用0
                                                {"1","1","1","1","0"},//最小长度设定,可以为空用0
                                                {"1","2","3","4","5"}//前台显示列号，从0开始计算,注意有选择框的是0
                    };
                    List<Object> checkRes = ListChecker.validateList(listInfoData, strField, null, null, true, "FS0309_Sub");
                    if (checkRes != null)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = checkRes;
                        apiResult.flag = Convert.ToInt32(ERROR_FLAG.单元格定位提示);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                string strErrorName = "";
                DataTable dt = ListToDataTable(listInfoData);
                string _msg = logic.UpdateFZJSEdit(dt, loginInfo.UserId);
                if (strErrorName != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "保存失败," + strErrorName;
                    apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0908", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 发注
        [HttpPost]
        [EnableCors("any")]
        public string UpdateFZJSEditFZ([FromBody] dynamic data)
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
            string vcType = dataForm.vcType;
            string vcOrder = dataForm.vcOrder;
            string vcSaleUser = dataForm.vcSaleUser;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(Convert.ToString(dataForm.temp));
            vcType = vcType == null ? "" : vcType;
            vcOrder = vcOrder == null ? "" : vcOrder;
            vcSaleUser = vcSaleUser == null ? "" : vcSaleUser;
            try
            {
                string exlName;
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = logic.UpdateFZJSEditFZ(dt, vcType, vcOrder, loginInfo.UserId, out exlName);
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0204", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 通用方法
        public DataTable ListToDataTable(List<Dictionary<string, Object>> listInfoData)
        {
            DataTable tb = new DataTable();
            if (listInfoData.Count > 0)
            {
                Dictionary<string, object> li = listInfoData[0];
                for (int i = 0; i < li.Count; i++)
                {
                    string colName = li.ToList()[i].Key;
                    string colType = li.ToList()[i].Value.GetType().Name;
                    tb.Columns.Add(new DataColumn(colName, li.ToList()[i].Value.GetType()));
                }
                foreach (Dictionary<string, object> li1 in listInfoData)
                {
                    DataRow r = tb.NewRow();
                    for (int j = 0; j < tb.Columns.Count; j++)
                        r[j] = li1[tb.Columns[j].ColumnName].ToString();
                    tb.Rows.Add(r);
                }
            }
            return tb;
        }
        #endregion
    }
}
