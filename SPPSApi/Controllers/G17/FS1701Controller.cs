using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.ServiceModel;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebServiceAPI;


namespace SPPSApi.Controllers.G17
{
    [Route("api/FS1701/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1701Controller : BaseController
    {
        FS1701_Logic fs1701_Logic = new FS1701_Logic();
        FS1702_Logic fs1702_Logic = new FS1702_Logic();
        private readonly string FunctionID = "FS1701";

        private readonly IWebHostEnvironment _webHostEnvironment;

        public FS1701Controller(IWebHostEnvironment webHostEnvironment)
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
                //if (loginInfo.Special == "财务用户")
                //    res.Add("caiWuBtnVisible", false);
                //else
                //    res.Add("caiWuBtnVisible", true);

                List<Object> dataList_C021 = ComFunction.convertAllToResult(ComFunction.getTCode("C021"));//是否
                res.Add("C021", dataList_C021);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M08UE0701", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "初始化失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 检索
        [HttpPost]
        [EnableCors("any")]
        public string searchApi([FromBody]dynamic data)
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
            string vcIsDQ = dataForm.vcIsDQ;
            string vcTicketNo = dataForm.vcTicketNo;
            string vcLJNo = dataForm.vcLJNo;
            string vcOldOrderNo = dataForm.vcOldOrderNo;

            try
            {
                DataTable dt = fs1701_Logic.Search(vcIsDQ, vcTicketNo, vcLJNo, vcOldOrderNo);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);

                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
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
            string vcIsDQ = dataForm.vcIsDQ;
            string vcTicketNo = dataForm.vcTicketNo;
            string vcLJNo = dataForm.vcLJNo;
            string vcOldOrderNo = dataForm.vcOldOrderNo;

            try
            {
                DataTable dt = fs1701_Logic.Search(vcIsDQ, vcTicketNo, vcLJNo, vcOldOrderNo);
                string[] heads = { "票号", "零件号码", "原订单号", "数量", "是否到齐" };
                string[] fields = { "vcTicketNo", "vcLJNo", "vcOldOrderNo", "iQuantity", "vcIsDQ" };
                string strMsg = "";
                string filepath = ComFunction.DataTableToExcel(heads, fields, dt, _webHostEnvironment.ContentRootPath, loginInfo.UserId, FunctionID, ref strMsg);
                if (strMsg != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = strMsg;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = filepath;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M08UE1002", ex, loginInfo.UserId);
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
                    #region 数据格式校验
                    string[,] strField = new string[,]
                    {
                        {"数量","是否到齐"},//中文字段名
                        {"iQuantity","vcIsDQ"},//英文字段名
                        {FieldCheck.Num,""},//数据类型校验
                        {"0","1"},//最大长度设定,不校验最大长度用0
                        {"1","0"},//最小长度设定,可以为空用0
                        {"4","5"},//前台显示列号，从0开始计算,注意有选择框的是0
                    };
                    List<Object> checkRes = ListChecker.validateList(listInfoData, strField, null, null, true, "FS1701");
                    if (checkRes != null)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = checkRes;
                        apiResult.flag = Convert.ToInt32(ERROR_FLAG.单元格定位提示);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    #endregion

                    #region 内容校验 
                    for(int i=0;i<listInfoData.Count;i++)
                    {
                        string vcIsDQ = listInfoData[i]["vcIsDQ"].ToString();
                        if (vcIsDQ != "" && Array.IndexOf(new string[] { "是", "否" }, vcIsDQ) == -1)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "[是否到齐]列只能填写是/否";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                    }
                    #endregion
                }
                fs1701_Logic.Save(listInfoData, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0203", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
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
                List<Dictionary<string, Object>> checkedInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();
                if (checkedInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选择一行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fs1701_Logic.Del(checkedInfoData, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0203", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 纳入看板打印
        [HttpPost]
        [EnableCors("any")]
        public string kbPrintApi([FromBody]dynamic data)
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
            JArray listInfo = dataForm.multipleSelection;
            List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
            try
            {
                DataTable dtMessage = fs1702_Logic.createTable("MES");
                if (listInfoData.Count != 0)
                {
                    for (int i = 0; i < listInfoData.Count; i++)
                    {
                        string vcLJNo = listInfoData[i]["vcLJNo"] == null ? "" : listInfoData[i]["vcLJNo"].ToString();
                        if (vcLJNo == "" )
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("零件号码{0}为空，无法打印", vcLJNo);
                            dtMessage.Rows.Add(dataRow);
                        }
                    }
                    if (dtMessage.Rows.Count != 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.type = "list";
                        apiResult.data = dtMessage;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    DataSet dsyushu = new DataSet();
                    bool bResult = fs1701_Logic.getPrintInfo_kb(listInfoData, loginInfo.UserId, ref dtMessage,ref dsyushu);
                    if (!bResult)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.type = "list";
                        apiResult.data = dtMessage;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    #region 调用webApi打印
                    FS0603_Logic fS0603_Logic = new FS0603_Logic();
                    string strPrinterName = fS0603_Logic.getPrinterName("FS1701", loginInfo.UserId);
                    //创建 HTTP 绑定对象
                    string file_crv = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "CryReports" + Path.DirectorySeparatorChar;
                    var binding = new BasicHttpBinding();
                    //根据 WebService 的 URL 构建终端点对象
                    var endpoint = new EndpointAddress(@"http://172.23.164.28/WebAPI/WebServiceAPI.asmx");
                    //创建调用接口的工厂，注意这里泛型只能传入接口
                    var factory = new ChannelFactory<WebServiceAPISoap>(binding, endpoint);
                    //从工厂获取具体的调用实例
                    var callClient = factory.CreateChannel();
                    setCRVPrintRequestBody Body = new setCRVPrintRequestBody();
                    Body.strCRVName = file_crv + "crv_FS1702_kb_main.rpt";
                    Body.strScrpit = "select * from tPrintTemp_FS1702_kb_main where vcOperator='" + loginInfo.UserId + "' ORDER BY LinId";
                    Body.strPrinterName = strPrinterName;
                    Body.sqlUserID = "sa";
                    Body.sqlPassword = "SPPS_Server2019";
                    Body.sqlCatalog = "SPPSdb";
                    Body.sqlSource = "172.23.180.116";
                    //调用具体的方法，这里是 HelloWorldAsync 方法
                    Task<setCRVPrintResponse> responseTask = callClient.setCRVPrintAsync(new setCRVPrintRequest(Body));
                    //获取结果
                    //setCRVPrintResponse response = responseTask.Result;
                    //if (response.Body.setCRVPrintResult != "打印成功")
                    //{
                    //    DataRow dataRow = dtMessage.NewRow();
                    //    dataRow["vcMessage"] = "打印失败，请联系管理员进行打印接口故障检查。";
                    //    dtMessage.Rows.Add(dataRow);
                    //}
                    if (dtMessage != null && dtMessage.Rows.Count != 0)
                    {
                        //弹出错误dtMessage
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.type = "list";
                        apiResult.data = dtMessage;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    #endregion
                    
                    //更新打印时间
                    fs1701_Logic.kbPrint(listInfoData, loginInfo.UserId,dsyushu);

                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.data = "打印成功";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "未选择有效的打印数据";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0204", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "生成印刷文件失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}
