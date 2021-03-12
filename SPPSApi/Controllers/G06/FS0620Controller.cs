using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace SPPSApi.Controllers.G06
{
    [Route("api/FS0620/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0620Controller : BaseController
    {
        FS0620_Logic fs0620_Logic = new FS0620_Logic();
        private readonly string FunctionID = "FS0620";

        private readonly IWebHostEnvironment _webHostEnvironment;


        public FS0620Controller(IWebHostEnvironment webHostEnvironment)
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
                List<Object> dataList_C018 = ComFunction.convertAllToResult(ComFunction.getTCode("C018"));//收货方
                List<Object> dataList_C000 = ComFunction.convertAllToResult(ComFunction.getTCode("C000"));//外注工厂
                List<Object> dataList_C003 = ComFunction.convertAllToResult(ComFunction.getTCode("C003"));//内外区分
                List<Object> dataList_C061 = ComFunction.convertAllToResult(ComFunction.getTCode("C061"));//内外区分
                DataTable SupplierWorkArea = fs0620_Logic.GetSupplierWorkArea();
                List<Object> dataList_SupplierWorkArea = ComFunction.convertToResult(SupplierWorkArea, new string[] { "vcValue", "vcName" });
                res.Add("C000", dataList_C000);
                res.Add("C003", dataList_C003);
                res.Add("C018", dataList_C018);
                res.Add("C061", dataList_C061);
                res.Add("SupplierWorkArea", dataList_SupplierWorkArea);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2001", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "初始化失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        //#region 绑定 发注工厂
        //[HttpPost]
        //[EnableCors("any")]
        //public string bindInjectionFactory()
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
        //        DataTable dt = fs0620_Logic.BindInjectionFactory();
        //        List<Object> dataList = ComFunction.convertToResult(dt, new string[] { "vcCodeId", "vcCodeName" });
        //        apiResult.code = ComConstant.SUCCESS_CODE;
        //        apiResult.data = dataList;
        //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //    }
        //    catch (Exception ex)
        //    {
        //        ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2001", ex, loginInfo.UserId);
        //        apiResult.code = ComConstant.ERROR_CODE;
        //        apiResult.data = "绑定发注工厂列表失败";
        //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //    }
        //}
        //#endregion

        //#region 绑定内外
        //[HttpPost]
        //[EnableCors("any")]
        //public string bindInsideOutsideType()
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
        //        DataTable dt = fs0620_Logic.BindInsideOutsideType();
        //        List<Object> dataList = ComFunction.convertToResult(dt, new string[] { "vcCodeId", "vcCodeName" });
        //        apiResult.code = ComConstant.SUCCESS_CODE;
        //        apiResult.data = dataList;
        //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //    }
        //    catch (Exception ex)
        //    {
        //        ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2002", ex, loginInfo.UserId);
        //        apiResult.code = ComConstant.ERROR_CODE;
        //        apiResult.data = "绑定内外列表失败";
        //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //    }
        //}
        //#endregion

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
            string dOperatorTime = dataForm.dOperatorTime == null ? "" : dataForm.dOperatorTime;
            string vcTargetYear = dataForm.vcTargetYear == null ? "" : dataForm.vcTargetYear;
            string vcPartNo = dataForm.vcPartNo == null ? "" : dataForm.vcPartNo;
            string vcInjectionFactory = dataForm.vcInjectionFactory == null ? "" : dataForm.vcInjectionFactory;
            string vcInsideOutsideType = dataForm.vcInsideOutsideType == null ? "" : dataForm.vcInsideOutsideType;
            string vcSupplier_id = dataForm.vcSupplier_id == null ? "" : dataForm.vcSupplier_id;
            string vcWorkArea = dataForm.vcWorkArea == null ? "" : dataForm.vcWorkArea;
            string vcCarType = dataForm.vcCarType == null ? "" : dataForm.vcCarType;
            try
            {
                DataTable dt = fs0620_Logic.Search(dOperatorTime,vcTargetYear, vcPartNo, vcInjectionFactory, vcInsideOutsideType, vcSupplier_id, vcWorkArea, vcCarType);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2001", ex, loginInfo.UserId);
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
            string dOperatorTime = dataForm.dOperatorTime == null ? "" : dataForm.dOperatorTime;
            string vcTargetYear = dataForm.vcTargetYear == null ? "" : dataForm.vcTargetYear;
            string vcPartNo = dataForm.vcPartNo == null ? "" : dataForm.vcPartNo;
            string vcInjectionFactory = dataForm.vcInjectionFactory == null ? "" : dataForm.vcInjectionFactory;
            string vcInsideOutsideType = dataForm.vcInsideOutsideType == null ? "" : dataForm.vcInsideOutsideType;
            string vcSupplierIdWorkArea = dataForm.vcSupplierIdWorkArea == null ? "" : dataForm.vcSupplierIdWorkArea;
            string vcType = dataForm.vcType == null ? "" : dataForm.vcType;
            string vcCarType = dataForm.vcCarType == null ? "" : dataForm.vcCarType;
            try
            {
                DataTable dt = fs0620_Logic.Search(dOperatorTime,vcTargetYear, vcPartNo, vcInjectionFactory, vcInsideOutsideType, vcSupplierIdWorkArea, vcType, vcCarType);
                string[] head = new string[] { };
                string[] field = new string[] { };
                //[vcPartNo], [dBeginDate], [dEndDate]
                head = new string[] { "导入时间","年计类型","收货方", "包装工厂", "对象年份", "品番", "发注工厂", "内外", "供应商代码", "工区", "车型", "收容数", "1月", "2月", "3月", "4月", "5月", "6月", "7月", "8月", "9月", "10月", "11月", "12月", "对象年合计", "N+1年预测", "N+2年预测" };
                field = new string[] { "dOperatorTime","vcType", "vcReceiver", "vcPackPlant", "vcTargetYear", "vcPartNo", "vcInjectionFactory", "vcInsideOutsideType","vcSupplier_id", "vcWorkArea",  "vcCarType", "vcAcceptNum", "vcJanuary", "vcFebruary", "vcMarch", "vcApril", "vcMay", "vcJune", "vcJuly", "vcAugust", "vcSeptember", "vcOctober", "vcNovember", "vcDecember", "vcSum", "vcNextOneYear", "vcNextTwoYear" };
                string msg = string.Empty; 
                string filepath = ComFunction.generateExcelWithXlt(dt, field, _webHostEnvironment.ContentRootPath, "FS0620_Data.xlsx", 1, loginInfo.UserId, FunctionID);
                //string filepath = ComFunction.DataTableToExcel(head, field, dt, ".", loginInfo.UserId, FunctionID, ref msg);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2003", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
        #region 删除操作
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

                //以下开始业务处理
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                JArray listInfo = dataForm;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
                if (listInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请先检索数据，再进行删除操作！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fs0620_Logic.del(listInfoData);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2008", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
        #region 年计邮件发送
        [HttpPost]
        [EnableCors("any")]
        public string sendMailApi([FromBody] dynamic data)
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

                //以下开始业务处理
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                JArray listInfo = dataForm;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
                if (listInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请先检索数据，再进行年检邮件电送！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                DataTable dt = new DataTable();
                dt.Columns.Add("dOperatorTime");
                dt.Columns.Add("vcPackPlant");
                dt.Columns.Add("vcTargetYear");
                dt.Columns.Add("vcPartNo");
                dt.Columns.Add("vcInjectionFactory");
                dt.Columns.Add("vcInsideOutsideType");
                dt.Columns.Add("vcSupplier_id");
                dt.Columns.Add("vcWorkArea");
                dt.Columns.Add("vcCarType");
                dt.Columns.Add("vcAcceptNum");
                dt.Columns.Add("vcJanuary");
                dt.Columns.Add("vcFebruary");
                dt.Columns.Add("vcMarch");
                dt.Columns.Add("vcApril");
                dt.Columns.Add("vcMay");
                dt.Columns.Add("vcJune");
                dt.Columns.Add("vcJuly");
                dt.Columns.Add("vcAugust");
                dt.Columns.Add("vcSeptember");
                dt.Columns.Add("vcOctober");
                dt.Columns.Add("vcNovember");
                dt.Columns.Add("vcDecember");
                dt.Columns.Add("vcSum");
                dt.Columns.Add("vcNextOneYear");
                dt.Columns.Add("vcNextTwoYear");
                for (int i=0;i<listInfoData.Count;i++)
                {
                    DataRow dr = dt.NewRow();
                    dr["dOperatorTime"] = listInfoData[i]["dOperatorTime"]==null?"": listInfoData[i]["dOperatorTime"].ToString();
                    dr["vcPackPlant"] = listInfoData[i]["vcPackPlant"] == null ? "" : listInfoData[i]["vcPackPlant"].ToString();
                    dr["vcTargetYear"] = listInfoData[i]["vcTargetYear"] == null ? "" : listInfoData[i]["vcTargetYear"].ToString();
                    dr["vcPartNo"] = listInfoData[i]["vcPartNo"] == null ? "" : listInfoData[i]["vcPartNo"].ToString();
                    dr["vcInjectionFactory"] = listInfoData[i]["vcInjectionFactory"] == null ? "" : listInfoData[i]["vcInjectionFactory"].ToString();
                    dr["vcInsideOutsideType"] = listInfoData[i]["vcInsideOutsideType"] == null ? "" : listInfoData[i]["vcInsideOutsideType"].ToString();
                    dr["vcSupplier_id"] = listInfoData[i]["vcSupplier_id"] == null ? "" : listInfoData[i]["vcSupplier_id"].ToString();
                    dr["vcWorkArea"] = listInfoData[i]["vcWorkArea"] == null ? "" : listInfoData[i]["vcWorkArea"].ToString();
                    dr["vcCarType"] = listInfoData[i]["vcCarType"] == null ? "" : listInfoData[i]["vcCarType"].ToString();
                    dr["vcAcceptNum"] = listInfoData[i]["vcAcceptNum"] == null ? "" : listInfoData[i]["vcAcceptNum"].ToString();
                    dr["vcJanuary"] = listInfoData[i]["vcJanuary"] == null ? "" : listInfoData[i]["vcJanuary"].ToString();
                    dr["vcMarch"] = listInfoData[i]["vcMarch"] == null ? "" : listInfoData[i]["vcMarch"].ToString(); 
                     dr["vcMay"] = listInfoData[i]["vcMay"] == null ? "" : listInfoData[i]["vcMay"].ToString();
                    dr["vcJune"] = listInfoData[i]["vcJune"] == null ? "" : listInfoData[i]["vcJune"].ToString();
                    dr["vcJuly"] = listInfoData[i]["vcJuly"] == null ? "" : listInfoData[i]["vcJuly"].ToString();
                    dr["vcAugust"] = listInfoData[i]["vcAugust"] == null ? "" : listInfoData[i]["vcAugust"].ToString();
                    dr["vcSeptember"] = listInfoData[i]["vcSeptember"] == null ? "" : listInfoData[i]["vcSeptember"].ToString();
                    dr["vcOctober"] = listInfoData[i]["vcOctober"] == null ? "" : listInfoData[i]["vcOctober"].ToString();
                    dr["vcNovember"] = listInfoData[i]["vcNovember"] == null ? "" : listInfoData[i]["vcNovember"].ToString();
                    dr["vcDecember"] = listInfoData[i]["vcDecember"] == null ? "" : listInfoData[i]["vcDecember"].ToString();
                    dr["vcSum"] = listInfoData[i]["vcSum"].ToString() == null ? "" : listInfoData[i]["vcSum"].ToString();
                    dr["vcNextOneYear"] = listInfoData[i]["vcNextOneYear"] == null ? "" : listInfoData[i]["vcNextOneYear"].ToString();
                    dr["vcNextTwoYear"] = listInfoData[i]["vcNextTwoYear"] == null ? "" : listInfoData[i]["vcNextTwoYear"].ToString();
                    dt.Rows.Add(dr);
                }
               
                string[] columnArray = { "vcSupplier_id", "vcWorkArea" };
                DataView dtSelectView = dt.DefaultView;
                DataTable dtSelect = dtSelectView.ToTable(true, columnArray);//去重后的dt 

                string logName = System.DateTime.Now.ToString("yyyyMMddHHmmss") + Guid.NewGuid().ToString("N");
                string logs = string.Empty;

                string[] head = new string[] { };
                string[] field = new string[] { };
                //[vcPartNo], [dBeginDate], [dEndDate]
                head = new string[] { "导入时间", "包装工厂", "对象年份", "品番", "发注工厂", "内外", "供应商代码", "工区", "车型", "收容数", "一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月", "对象年合计", "N+1年预测", "N+2年预测" };
                field = new string[] { "dOperatorTime", "vcPackPlant", "vcTargetYear", "vcPartNo", "vcInjectionFactory", "vcInsideOutsideType", "vcSupplier_id", "vcWorkArea", "vcCarType", "vcAcceptNum", "vcJanuary", "vcFebruary", "vcMarch", "vcApril", "vcMay", "vcJune", "vcJuly", "vcAugust", "vcSeptember", "vcOctober", "vcNovember", "vcDecember", "vcSum", "vcNextOneYear", "vcNextTwoYear" };
                string path = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Export" + Path.DirectorySeparatorChar;
                StringBuilder strErr = new StringBuilder();
                bool bReault = true;
                //FS0603_Logic fs0603_Logic = new FS0603_Logic();
                DataTable dtMessage = fs0620_Logic.createTable("email");
                for (int i=0;i<dtSelect.Rows.Count;i++)
                {
                    //组织制定供应商和工区的数据
                    string vcSupplier_id = dtSelect.Rows[i]["vcSupplier_id"].ToString();
                    string vcWorkArea = dtSelect.Rows[i]["vcWorkArea"].ToString();
                    DataRow[] drArray = dt.Select("vcSupplier_id='" + vcSupplier_id + "' and vcWorkArea='"+ vcWorkArea + "' ");
                    DataTable dtNewSupplierandWorkArea = drArray[0].Table.Clone(); // 复制DataRow的表结构
                    string msg = string.Empty;
                    foreach (DataRow dr in drArray)
                    {
                        dtNewSupplierandWorkArea.ImportRow(dr);
                    }
                    string strFileName = System.DateTime.Now.ToString("yyyyMMdd")+"_" + vcSupplier_id + "_" + vcWorkArea+"年计管理";
                    string filepath = ComFunction.DataTableToExcel(head, field, dtNewSupplierandWorkArea, ".", loginInfo.UserId, strFileName, ref msg);
                    if (filepath == "")
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcSupplier_id"] = vcSupplier_id;
                        dataRow["vcWorkArea"] = vcWorkArea;
                        dataRow["vcMessage"] = "供应商：" + vcSupplier_id + "工区" + vcWorkArea + "附件没有生产成功，不能发送邮件";
                        dtMessage.Rows.Add(dataRow);
                        bReault = false;
                        logs = System.DateTime.Now.ToString() + "供应商：" + vcSupplier_id + "工区" + vcWorkArea + "附件没有生产成功，不能发送邮件 \n";
                        writeLog(logs, logName, loginInfo.UnitCode, loginInfo.UserId);
                        strErr.Append(logs);
                        continue;
                    }
                    filepath = path + filepath;
                    logs = System.DateTime.Now.ToString() + "供应商："+ vcSupplier_id + "工区"+ vcWorkArea+ "开始查找邮箱 \n";
                    writeLog(logs, logName, loginInfo.UnitCode, loginInfo.UserId);
                    //获取供应商 工区的邮箱
                    DataTable dtEmail = fs0620_Logic.getEmail(vcSupplier_id, vcWorkArea);
                    if (dtEmail.Rows.Count==0)
                    {
                        logs = System.DateTime.Now.ToString() + "供应商：" + vcSupplier_id + "工区" + vcWorkArea + "没有维护邮箱，不能发送邮件 \n";
                        writeLog(logs, logName, loginInfo.UnitCode, loginInfo.UserId);
                        strErr.Append("供应商：" + vcSupplier_id + "工区" + vcWorkArea+ "缺少邮箱，不能发送!");
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcSupplier_id"] = vcSupplier_id;
                        dataRow["vcWorkArea"] = vcWorkArea;
                        dataRow["vcMessage"] = "供应商：" + vcSupplier_id + "工区" + vcWorkArea + "没有维护邮箱，不能发送邮件";
                        dtMessage.Rows.Add(dataRow);
                        bReault = false;
                        continue;
                    }
                    if (dtEmail.Rows.Count>1)
                    {
                        logs = System.DateTime.Now.ToString() + "供应商：" + vcSupplier_id + "工区" + vcWorkArea + "邮箱维护冗余有误，不能发送邮件 \n";
                        writeLog(logs, logName, loginInfo.UnitCode, loginInfo.UserId);
                        strErr.Append("供应商：" + vcSupplier_id + "工区" + vcWorkArea + "邮箱冗余,不能发送!");
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcSupplier_id"] = vcSupplier_id;
                        dataRow["vcWorkArea"] = vcWorkArea;
                        dataRow["vcMessage"] = "供应商：" + vcSupplier_id + "工区" + vcWorkArea + "邮箱维护冗余有误，不能发送邮件";
                        dtMessage.Rows.Add(dataRow);
                        bReault = false;
                        continue;
                    }
                    string strdisplayName1 = dtEmail.Rows[0]["vcLinkMan1"].ToString();
                    if (string.IsNullOrEmpty(strdisplayName1))
                    {
                        strdisplayName1 = "";
                    }
                    string strdisplayName2 = dtEmail.Rows[0]["vcLinkMan2"].ToString();
                    if (string.IsNullOrEmpty(strdisplayName2))
                    {
                        strdisplayName2 = "";
                    }
                    string strdisplayName3 = dtEmail.Rows[0]["vcLinkMan3"].ToString();
                    if (string.IsNullOrEmpty(strdisplayName3))
                    {
                        strdisplayName3 = "";
                    }
                    string strEmail1 = dtEmail.Rows[0]["vcEmail1"].ToString();
                    string strEmail2 = dtEmail.Rows[0]["vcEmail2"].ToString();
                    string strEmail3 = dtEmail.Rows[0]["vcEmail3"].ToString();
                    if (string.IsNullOrEmpty(strEmail1)&& string.IsNullOrEmpty(strEmail2)&&string.IsNullOrEmpty(strEmail3))
                    {
                        logs = System.DateTime.Now.ToString() + "供应商：" + vcSupplier_id + "工区" + vcWorkArea + "邮箱为空，不能发送邮件 \n";
                        writeLog(logs, logName, loginInfo.UnitCode, loginInfo.UserId);
                        strErr.Append("供应商：" + vcSupplier_id + "工区" + vcWorkArea + "邮箱为空,不能发送!");
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcSupplier_id"] = vcSupplier_id;
                        dataRow["vcWorkArea"] = vcWorkArea;
                        dataRow["vcMessage"] = "供应商：" + vcSupplier_id + "工区" + vcWorkArea + "邮箱为空，不能发送邮件";
                        dtMessage.Rows.Add(dataRow);
                        bReault = false;
                        continue;
                    }
                    //string[] emailArray = strEmail.Split(';');
                    //收件人dt
                    DataTable receiverDt = new DataTable();
                    receiverDt.Columns.Add("address");
                    receiverDt.Columns.Add("displayName");
                    //for (int j = 0; j < emailArray.Length; j++) {
                    //    if (emailArray[j].ToString().Length>0)
                    //    {
                    //        DataRow dr = receiverDt.NewRow();
                    //        dr["address"] = emailArray[j].ToString();
                    //        dr["displayName"] = strdisplayName;
                    //        receiverDt.Rows.Add(dr);
                    //    }
                    //}
                    if (strEmail1.Length>0)
                    {
                        DataRow dr = receiverDt.NewRow();
                        dr["address"] = strEmail1.ToString();
                        dr["displayName"] = strdisplayName1;
                        receiverDt.Rows.Add(dr);
                    }

                    if (strEmail2.Length > 0)
                    {
                        DataRow dr = receiverDt.NewRow();
                        dr["address"] = strEmail2.ToString();
                        dr["displayName"] = strdisplayName2;
                        receiverDt.Rows.Add(dr);
                    }
                    if (strEmail3.Length > 0)
                    {
                        DataRow dr = receiverDt.NewRow();
                        dr["address"] = strEmail3.ToString();
                        dr["displayName"] = strdisplayName3;
                        receiverDt.Rows.Add(dr);
                    }

                    //抄送人dt 通过Tcode 自己定义cCDt
                    DataTable cCDt = null;
                    DataTable dtCCEmail = fs0620_Logic.getCCEmail("C053");
                    if (dtCCEmail.Rows.Count>0)
                    {
                        cCDt = dtCCEmail;
                    }
                    //邮件主题
                    string strSubject = "供应商:" + vcSupplier_id + "工区:" + vcWorkArea + "_" + loginInfo.UnitCode + "年计管理信息";
                    //邮件内容
                    string strEmailBody = "";
                    strEmailBody += "<div style='font-family:宋体;font-size:12'>" + "各位供应商 殿：大家好<br /><br />";
                    strEmailBody += loginInfo.UnitCode+"补给  " + loginInfo.UserName+ "<br /><br />";
                    strEmailBody += "感谢大家一直以来对"+ loginInfo.UnitCode + "补给业务的协力！<br /><br />";
                    strEmailBody += "  此次邮件内容为事业体"+loginInfo.UnitCode+"年计管理信息，具体内容请查看附！<br /><br />";
                    //strEmailBody += "  请在1个工作日内将是否可以供货的确认结果邮件回复，以下是各个仓库对应的邮箱：<br />";
                    
                    string result= ComFunction.SendEmailInfo(loginInfo.Email, loginInfo.UnitName, strEmailBody, receiverDt, cCDt, strSubject, filepath, false);
                    if (result=="Success")
                    {
                        logs = System.DateTime.Now.ToString() + "供应商：" + vcSupplier_id + "工区" + vcWorkArea + "邮箱发送成功！\n";
                    }
                    else
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcSupplier_id"] = vcSupplier_id;
                        dataRow["vcWorkArea"] = vcWorkArea;
                        dataRow["vcMessage"] = "供应商：" + vcSupplier_id + "工区" + vcWorkArea + "邮箱发送失败，邮件发送公共方法未知原因！";
                        dtMessage.Rows.Add(dataRow);
                        bReault = false;
                        logs = System.DateTime.Now.ToString() + "供应商：" + vcSupplier_id + "工区" + vcWorkArea + "邮箱发送失败，邮件发送公共方法未知原因！\n";
                    }
                    writeLog(logs, logName, loginInfo.UnitCode, loginInfo.UserId);
                }
                if (!bReault)
                {
                    //弹出错误dtMessage
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0309_Export.xlsx", 2, loginInfo.UserId, FunctionID);
                //string filepath = ComFunction.DataTableToExcel(head, field, dt, ".", loginInfo.UserId, FunctionID, ref msg);
                //if (filepath == "")
                //{
                //    apiResult.code = ComConstant.ERROR_CODE;
                //    apiResult.data = "导出生成文件失败";
                //    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                //}

                apiResult.code = ComConstant.SUCCESS_CODE;
                if (strErr.Length > 0)
                {
                    apiResult.data = strErr.ToString()+",其余发送成功！";
                }
                else
                {
                    apiResult.data = "邮件发送成功！";
                }
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2004", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "邮件发送失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="logs"></param>
        /// <param name="logName"></param>
        /// <param name="career"></param>
        /// <param name="userId"></param>
        public void writeLog(string logs, string logName, string career, string userId)
        {
            string path0 = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Log" + Path.DirectorySeparatorChar + "Email" + Path.DirectorySeparatorChar + FunctionID;
            string filename = path0 + Path.DirectorySeparatorChar + career + "_" + userId + "_" + logName + ".txt";
            if (!Directory.Exists(path0))
            {
                Directory.CreateDirectory(path0);
            }

            if (!System.IO.File.Exists(filename))
            {
                FileStream stream = System.IO.File.Create(filename);
                stream.Close();
                stream.Dispose();
            }
            using (StreamWriter writer = new StreamWriter(filename, true))
            {
                writer.WriteLine(logs);
            }
        }
        #endregion
        //#region list-->dt
        //public DataTable ToDataTable<T>(IEnumerable<T> collection)
        //{
        //    var props = typeof(T).GetProperties();
        //    var dt = new DataTable();
        //    dt.Columns.AddRange(props.Select(p => new DataColumn(p.Name, p.PropertyType)).ToArray());
        //    if (collection.Count() > 0)
        //    {
        //        for (int i = 0; i < collection.Count(); i++)
        //        {
        //            ArrayList tempList = new ArrayList();
        //            foreach (PropertyInfo pi in props)
        //            {
        //                object obj = pi.GetValue(collection.ElementAt(i), null);
        //                tempList.Add(obj);
        //            }
        //            object[] array = tempList.ToArray();
        //            dt.LoadDataRow(array, true);
        //        }
        //    }
        //    return dt;
        //}
        //#endregion

        /// <summary>
        /// 导出消息信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string exportmessageApi([FromBody] dynamic data)
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
                List<Dictionary<string, Object>> listInfoData = dataForm.ToObject<List<Dictionary<string, Object>>>();
                //DataTable dataTable = fs0603_Logic.createTable("MES");
                //FS0404_Logic fs0404_Logic = new FS0404_Logic();
                DataTable dataTable = fs0620_Logic.createTable("email");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    DataRow dataRow = dataTable.NewRow();
                    dataRow["vcSupplier_id"] = listInfoData[i]["vcSupplier_id"].ToString();
                    dataRow["vcWorkArea"] = listInfoData[i]["vcWorkArea"].ToString();
                    dataRow["vcMessage"] = listInfoData[i]["vcMessage"].ToString();
                    dataTable.Rows.Add(dataRow);
                }

                string[] fields = { "vcSupplier_id", "vcWorkArea", "vcMessage" };
                string filepath = ComFunction.generateExcelWithXlt(dataTable, fields, _webHostEnvironment.ContentRootPath, "FS0620_MessageList.xlsx", 1, loginInfo.UserId, FunctionID);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0902", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }

        #region  报表导出
        [HttpPost]
        [EnableCors("any")]
        public string createReportApi([FromBody] dynamic data)
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
            string vcTargetYear = dataForm.vcTargetYear == null ? "" : dataForm.vcTargetYear;
            string vcType= dataForm.vcType == null ? "" : dataForm.vcType;
            if (vcTargetYear=="")
            {
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "对象年份为空，无法导出报表！";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            if (vcType == "")
            {
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "年计类型为空，无法导出报表！";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            string filepath = "";
            try
            {
                //获取内制  根据工厂动态生成Datatable
                DataTable dtPlant = fs0620_Logic.getPlant(vcTargetYear, vcType);
                if (dtPlant.Rows.Count==0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "未查询到"+vcTargetYear+"年年计管理数据，无法导出报表！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //DataSet ds = new DataSet();
                ArrayList arrayList = new ArrayList();
                for (int i=0;i<dtPlant.Rows.Count;i++)
                {
                    string plantCode = dtPlant.Rows[i]["vcInjectionFactory"].ToString(); //发注工厂
                    DataTable dt = fs0620_Logic.getDtByTargetYearAndPlant(vcTargetYear, plantCode, vcType);
                    arrayList.Add(dt);
                }
                // 外注
                DataTable dtWaiZhu = new DataTable();
                dtWaiZhu = fs0620_Logic.getWaiZhuDt(vcTargetYear, vcType);
                //最终合计
                DataTable dthuiZong = new DataTable();
                dthuiZong = fs0620_Logic.getHuiZongDt(vcTargetYear, vcType);

                #region 导出报表
                XSSFWorkbook hssfworkbook = new XSSFWorkbook();//用于创建xlsx
                string XltPath = "." + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Template" + Path.DirectorySeparatorChar + "FS0620_Report.xlsx";
                using (FileStream fs = System.IO.File.OpenRead(XltPath))
                {
                    hssfworkbook = new XSSFWorkbook(fs);
                    fs.Close();
                }
                ISheet mysheetHSSF = hssfworkbook.GetSheetAt(0);//获得sheet
                #region 设置单元格对齐方式
                //创建CellStyle  
                //ICellStyle styleGeneral = hssfworkbook.CreateCellStyle();
                //styleGeneral.Alignment = HorizontalAlignment.General;//【General】数字、时间默认：右对齐；BOOL：默认居中；字符串：默认左对齐  

                //ICellStyle styleLeft = hssfworkbook.CreateCellStyle();
                //styleLeft.Alignment = HorizontalAlignment.Left;//【Left】左对齐  

                //ICellStyle styleCenter = hssfworkbook.CreateCellStyle();
                //styleCenter.Alignment = HorizontalAlignment.Center;//【Center】居中  

                //ICellStyle styleRight = hssfworkbook.CreateCellStyle();
                //styleRight.Alignment = HorizontalAlignment.Right;//【Right】右对齐  

                //ICellStyle styleFill = hssfworkbook.CreateCellStyle();
                //styleFill.Alignment = HorizontalAlignment.Fill;//【Fill】填充  

                //ICellStyle styleJustify = hssfworkbook.CreateCellStyle();
                //styleJustify.Alignment = HorizontalAlignment.Justify;//【Justify】两端对齐[会自动换行]（主要针对英文）  
                //ICellStyle styleCenterSelection = hssfworkbook.CreateCellStyle();
                //styleCenterSelection.Alignment = HorizontalAlignment.CenterSelection;//【CenterSelection】跨列居中  

                //ICellStyle styleDistributed = hssfworkbook.CreateCellStyle();
                //styleDistributed.Alignment = HorizontalAlignment.Distributed;//【Distributed】分散对齐[会自动换行]
                #endregion
                #region 设置字体样式
                ICellStyle style1 = hssfworkbook.CreateCellStyle();//声明style1对象，设置Excel表格的样式 标题
                ICellStyle style2 = hssfworkbook.CreateCellStyle();//9号字体加粗 没有背景色 内制 外注字体
                ICellStyle style3 = hssfworkbook.CreateCellStyle();//9号字体不加粗 没有背景色 2019/12
                ICellStyle style4 = hssfworkbook.CreateCellStyle();//9号字体不加粗 深蓝蓝 工程区分
                ICellStyle style5 = hssfworkbook.CreateCellStyle();//9号字体不加粗 紫色 具体内容
                ICellStyle style6 = hssfworkbook.CreateCellStyle();//9号字体不加粗 结尾
                ICellStyle style7 = hssfworkbook.CreateCellStyle();//9号字体不加粗 以上
                IFont font = hssfworkbook.CreateFont();
                font.Color = IndexedColors.Black.Index;
                font.IsBold = true; ;
                font.FontHeightInPoints = 22;
                //font.FontName = "宋体";
                style1.SetFont(font);
                style1.Alignment = HorizontalAlignment.Center;//两端自动对齐（自动换行）
                style1.VerticalAlignment = VerticalAlignment.Center;
                

                IFont font2 = hssfworkbook.CreateFont();
                font2.Color = IndexedColors.Black.Index;
                font2.IsBold = true; ;
                font2.FontHeightInPoints = 16;
                //font.FontName = "宋体";
                style2.SetFont(font2);
                style2.Alignment = HorizontalAlignment.Center;
                style2.VerticalAlignment = VerticalAlignment.Center;
                

                IFont font3 = hssfworkbook.CreateFont();
                font3.Color = IndexedColors.Black.Index;
                font3.IsBold = false; ;
                font3.FontHeightInPoints = 11;
                //font.FontName = "宋体";
                style3.SetFont(font3);
                style3.Alignment = HorizontalAlignment.Right;
                style3.VerticalAlignment = VerticalAlignment.Center;
              

                IFont font4 = hssfworkbook.CreateFont();
                font4.Color = IndexedColors.Black.Index;
                font4.IsBold = false; ;
                font4.FontHeightInPoints = 11;
                //font.FontName = "宋体";
                style4.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;//灰色
                style4.FillPattern = FillPattern.SolidForeground;
                style4.SetFont(font4);
                style4.Alignment = HorizontalAlignment.Center;
                style4.VerticalAlignment = VerticalAlignment.Center;
                style4.BorderLeft = BorderStyle.Thin;
                style4.BorderRight = BorderStyle.Thin;
                style4.BorderTop = BorderStyle.Medium;

                IFont font5 = hssfworkbook.CreateFont();
                font5.Color = IndexedColors.Black.Index;
                font5.IsBold = false; ;
                font5.FontHeightInPoints = 12;
                //font.FontName = "宋体";
                //style5.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Lavender.Index;
                //style5.FillPattern = FillPattern.SolidForeground;
                style5.SetFont(font5);
                style5.Alignment = HorizontalAlignment.Center;
                //style5.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                style5.VerticalAlignment = VerticalAlignment.Center;
                style5.BorderLeft = BorderStyle.Thin;
                style5.BorderRight = BorderStyle.Thin;
                style5.BorderTop = BorderStyle.Thin;
                style5.BorderBottom = BorderStyle.Thin;

                IFont font6 = hssfworkbook.CreateFont();
                font6.Color = IndexedColors.Black.Index;
                font6.IsBold = false; ;
                font5.FontHeightInPoints = 12;
                //font6.FontName = "宋体";
                //style6.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightTurquoise.Index;
                //style6.FillPattern = FillPattern.SolidForeground;
                style6.SetFont(font6);
                style6.Alignment = HorizontalAlignment.Center;
                style6.VerticalAlignment = VerticalAlignment.Center;
                style6.BorderLeft = BorderStyle.Thin;
                style6.BorderRight = BorderStyle.Thin;
                style6.BorderTop = BorderStyle.Thin;
                style6.BorderBottom = BorderStyle.Medium;

                IFont font7 = hssfworkbook.CreateFont();
                font7.Color = IndexedColors.Black.Index;
                font7.IsBold = false; ;
                font7.FontHeightInPoints = 11;
                //font.FontName = "宋体";
                style7.SetFont(font3);
                style7.Alignment = HorizontalAlignment.Left;
                style7.VerticalAlignment = VerticalAlignment.Center;
                #endregion
                #region 设置列的宽度
                //mysheetHSSF.SetColumnWidth(0, 17 * 256); //设置第1列的列宽为17个字符
                //mysheetHSSF.SetColumnWidth(1, 5 * 256); //设置第2列的列宽为31个字符
                //mysheetHSSF.SetColumnWidth(2, 10 * 256); //设置第3列的列宽为10个字符
                //mysheetHSSF.SetColumnWidth(3, 10 * 256); //设置第4列的列宽为10个字符
                //mysheetHSSF.SetColumnWidth(4, 10 * 256); //设置第5列的列宽为10个字符
                //mysheetHSSF.SetColumnWidth(5, 10 * 256); //设置第6列的列宽为10个字符
                //mysheetHSSF.SetColumnWidth(6, 10 * 256); //设置第7列的列宽为10个字符
                //mysheetHSSF.SetColumnWidth(7, 10 * 256); //设置第8列的列宽为10个字符
                //mysheetHSSF.SetColumnWidth(8, 10 * 256); //设置第9列的列宽为10个字符
                //mysheetHSSF.SetColumnWidth(9, 10 * 256); //设置第10列的列宽为10个字符
                //mysheetHSSF.SetColumnWidth(10, 10 * 256); //设置第11列的列宽为10个字符
                //mysheetHSSF.SetColumnWidth(11, 10 * 256); //设置第12列的列宽为10个字符
                //mysheetHSSF.SetColumnWidth(12, 10 * 256); //设置第13列的列宽为10个字符
                //mysheetHSSF.SetColumnWidth(13, 17 * 256); //设置第14列的列宽为17个字符
                #endregion

                #region //设置第二行
                //2019/12 
                IRow SecondRowHSSF = mysheetHSSF.CreateRow(1);//创建row 行 从0开始
                SecondRowHSSF.CreateCell(16).SetCellValue(DateTime.Now.ToString("yyyy/MM"));
                #endregion
                #region //设置第三行 
                //设置第二行 统计项目  合并单元格【CellRangeAddress(开始行,结束行,开始列,结束列)】
                mysheetHSSF.AddMergedRegion(new CellRangeAddress(2, 2, 0, 16)); //合并单元格第二行从第1列到第2列
                IRow ThreeHSSF = mysheetHSSF.CreateRow(2); //添加第二行
                ThreeHSSF.Height = 34 * 20; //设置高度为50个点
                ThreeHSSF.CreateCell(0).SetCellValue(DateTime.Now.ToString("yyyy") + "年" + loginInfo.UnitCode + "补给部品当初年计");
                ThreeHSSF.GetCell(0).CellStyle = style1;//将CellStyle应用于具体单元格
                #endregion
                #region //设置第五行 内制开始
                //设置第二行 统计项目  合并单元格【CellRangeAddress(开始行,结束行,开始列,结束列)】
                mysheetHSSF.AddMergedRegion(new CellRangeAddress(4, 4, 0, 1));
                IRow FiveRowHSSF = mysheetHSSF.CreateRow(4); //设置第五行
                int nextRow = 6;
                string strYear2 = DateTime.Now.ToString("yyyy").Substring(2);
                string strNextOneYear = DateTime.Now.ToString("yyyy") + 1;
                string strNextTwoYear = DateTime.Now.ToString("yyyy") + 2;
                if (arrayList.Count > 0)
                {
                    FiveRowHSSF.Height = 34 * 20; //设置高度为50个点
                    FiveRowHSSF.CreateCell(0).SetCellValue("【内制】");
                    FiveRowHSSF.GetCell(0).CellStyle = style2;//将CellStyle应用于具体单元格
                    for (int i = 0; i < arrayList.Count; i++)
                    {
                        DataTable dtNeiZhi = (DataTable)arrayList[i];
                        //设置列
                        mysheetHSSF.AddMergedRegion(new CellRangeAddress(nextRow, nextRow, 0, 1));
                        IRow nextRowHSSFCol = mysheetHSSF.CreateRow(nextRow); //设置第五行
                        nextRowHSSFCol.Height = 34 * 20;
                        nextRowHSSFCol.CreateCell(0).SetCellValue("工程区分");
                        nextRowHSSFCol.GetCell(0).CellStyle = style4;
                        nextRowHSSFCol.CreateCell(1).SetCellValue("");
                        nextRowHSSFCol.GetCell(1).CellStyle = style4;
                        nextRowHSSFCol.CreateCell(2).SetCellValue(strYear2 + "年1月");
                        nextRowHSSFCol.GetCell(2).CellStyle = style4;
                        nextRowHSSFCol.CreateCell(3).SetCellValue(strYear2 + "年2月");
                        nextRowHSSFCol.GetCell(3).CellStyle = style4;
                        nextRowHSSFCol.CreateCell(4).SetCellValue(strYear2 + "年3月");
                        nextRowHSSFCol.GetCell(4).CellStyle = style4;
                        nextRowHSSFCol.CreateCell(5).SetCellValue(strYear2 + "年4月");
                        nextRowHSSFCol.GetCell(5).CellStyle = style4;
                        nextRowHSSFCol.CreateCell(6).SetCellValue(strYear2 + "年5月");
                        nextRowHSSFCol.GetCell(6).CellStyle = style4;
                        nextRowHSSFCol.CreateCell(7).SetCellValue(strYear2 + "年6月");
                        nextRowHSSFCol.GetCell(7).CellStyle = style4;
                        nextRowHSSFCol.CreateCell(8).SetCellValue(strYear2 + "年7月");
                        nextRowHSSFCol.GetCell(8).CellStyle = style4;
                        nextRowHSSFCol.CreateCell(9).SetCellValue(strYear2 + "年8月");
                        nextRowHSSFCol.GetCell(9).CellStyle = style4;
                        nextRowHSSFCol.CreateCell(10).SetCellValue(strYear2 + "年9月");
                        nextRowHSSFCol.GetCell(10).CellStyle = style4;
                        nextRowHSSFCol.CreateCell(11).SetCellValue(strYear2 + "年10月");
                        nextRowHSSFCol.GetCell(11).CellStyle = style4;
                        nextRowHSSFCol.CreateCell(12).SetCellValue(strYear2 + "年11月");
                        nextRowHSSFCol.GetCell(12).CellStyle = style4;
                        nextRowHSSFCol.CreateCell(13).SetCellValue(strYear2 + "年12月");
                        nextRowHSSFCol.GetCell(13).CellStyle = style4;
                        nextRowHSSFCol.CreateCell(14).SetCellValue(DateTime.Now.ToString("yyyy") + "年合计");
                        nextRowHSSFCol.GetCell(14).CellStyle = style4;
                        nextRowHSSFCol.CreateCell(15).SetCellValue(strNextOneYear + "年合计");
                        nextRowHSSFCol.GetCell(15).CellStyle = style4;
                        nextRowHSSFCol.CreateCell(16).SetCellValue(strNextTwoYear + "年合计");
                        nextRowHSSFCol.GetCell(16).CellStyle = style4;
                        nextRow++;
                        int heBingStartRow = nextRow;
                        int hebingEndRow = nextRow + (dtNeiZhi.Rows.Count - 1) - 1;
                        mysheetHSSF.AddMergedRegion(new CellRangeAddress(heBingStartRow, hebingEndRow, 0, 0));
                        for (var j = 0; j < dtNeiZhi.Rows.Count - 1; j++)
                        {
                            IRow nextRowHSSFRow = mysheetHSSF.CreateRow(nextRow);
                            nextRowHSSFRow.Height = 34 * 20;
                            if (heBingStartRow == nextRow)
                            {
                                string strPlant = dtNeiZhi.Rows[0]["vcInjectionFactory"].ToString();
                                nextRowHSSFRow.CreateCell(0).SetCellValue(strPlant);
                                nextRowHSSFRow.GetCell(0).CellStyle = style5;//将CellStyle应用于具体单元格 
                            }else
                            {
                                nextRowHSSFRow.CreateCell(0).SetCellValue("");
                                nextRowHSSFRow.GetCell(0).CellStyle = style5;
                            }
                            int colNum = 1;//用于计数
                            for (var k = 1; k < dtNeiZhi.Columns.Count; k++)
                            {
                                if (k == 1)
                                {
                                    nextRowHSSFRow.CreateCell(colNum).SetCellValue(" ○" + dtNeiZhi.Rows[j][k].ToString());
                                    //nextRowHSSFRow.CreateCell(colNum).SetCellValue(Convert.ToDouble(dtNeiZhi.Rows[j][k].ToString()));
                                    nextRowHSSFRow.GetCell(colNum).CellStyle = style5;
                                    colNum++;
                                }
                                else
                                {
                                    nextRowHSSFRow.CreateCell(colNum).SetCellValue(Convert.ToDouble(dtNeiZhi.Rows[j][k].ToString()));
                                    nextRowHSSFRow.GetCell(colNum).CellStyle = style5;
                                    colNum++;
                                }

                            }
                            nextRow++;
                        }
                        //        //创建销售数据最后一行
                        #region
                        mysheetHSSF.AddMergedRegion(new CellRangeAddress(nextRow, nextRow, 0, 1));
                        IRow nextRowHSSFLastRow = mysheetHSSF.CreateRow(nextRow);
                        nextRowHSSFLastRow.Height = 34 * 20;
                        nextRowHSSFLastRow.CreateCell(0).SetCellValue("合计");
                        nextRowHSSFLastRow.GetCell(0).CellStyle = style6;//将CellStyle应用于具体单元格 

                        nextRowHSSFLastRow.CreateCell(1).SetCellValue("");
                        nextRowHSSFLastRow.GetCell(1).CellStyle = style6;
                        for (var m = 2; m < dtNeiZhi.Columns.Count; m++)
                        {
                            nextRowHSSFLastRow.CreateCell(m).SetCellValue(Convert.ToDouble(dtNeiZhi.Rows[dtNeiZhi.Rows.Count - 1][m].ToString()));
                            nextRowHSSFLastRow.GetCell(m).CellStyle = style6;
                        }
                        #endregion
                        //每一个个四行
                        nextRow = nextRow + 4;
                    }
                }
                #endregion
                #region 设置外注
                if (dtWaiZhu.Rows.Count > 0)
                {
                    mysheetHSSF.AddMergedRegion(new CellRangeAddress(nextRow, nextRow, 0, 1));
                    IRow nextRowHSSFCol = mysheetHSSF.CreateRow(nextRow);
                    nextRowHSSFCol.Height = 34 * 20;
                    nextRowHSSFCol.CreateCell(0).SetCellValue("【外注】");
                    nextRowHSSFCol.GetCell(0).CellStyle = style2;//将CellStyle应用于具体单元格
                    nextRow++;
                    //设置列
                    mysheetHSSF.AddMergedRegion(new CellRangeAddress(nextRow, nextRow, 0, 1));
                    IRow nextRowHSSFColWaiZhu = mysheetHSSF.CreateRow(nextRow);
                    nextRowHSSFColWaiZhu.Height = 34 * 20;
                    nextRowHSSFColWaiZhu.CreateCell(0).SetCellValue("工程区分");
                    nextRowHSSFColWaiZhu.GetCell(0).CellStyle = style4;
                    nextRowHSSFColWaiZhu.CreateCell(1).SetCellValue("");
                    nextRowHSSFColWaiZhu.GetCell(1).CellStyle = style4;
                    nextRowHSSFColWaiZhu.CreateCell(2).SetCellValue(strYear2 + "年1月");
                    nextRowHSSFColWaiZhu.GetCell(2).CellStyle = style4;
                    nextRowHSSFColWaiZhu.CreateCell(3).SetCellValue(strYear2 + "年2月");
                    nextRowHSSFColWaiZhu.GetCell(3).CellStyle = style4;
                    nextRowHSSFColWaiZhu.CreateCell(4).SetCellValue(strYear2 + "年3月");
                    nextRowHSSFColWaiZhu.GetCell(4).CellStyle = style4;
                    nextRowHSSFColWaiZhu.CreateCell(5).SetCellValue(strYear2 + "年4月");
                    nextRowHSSFColWaiZhu.GetCell(5).CellStyle = style4;
                    nextRowHSSFColWaiZhu.CreateCell(6).SetCellValue(strYear2 + "年5月");
                    nextRowHSSFColWaiZhu.GetCell(6).CellStyle = style4;
                    nextRowHSSFColWaiZhu.CreateCell(7).SetCellValue(strYear2 + "年6月");
                    nextRowHSSFColWaiZhu.GetCell(7).CellStyle = style4;
                    nextRowHSSFColWaiZhu.CreateCell(8).SetCellValue(strYear2 + "年7月");
                    nextRowHSSFColWaiZhu.GetCell(8).CellStyle = style4;
                    nextRowHSSFColWaiZhu.CreateCell(9).SetCellValue(strYear2 + "年8月");
                    nextRowHSSFColWaiZhu.GetCell(9).CellStyle = style4;
                    nextRowHSSFColWaiZhu.CreateCell(10).SetCellValue(strYear2 + "年9月");
                    nextRowHSSFColWaiZhu.GetCell(10).CellStyle = style4;
                    nextRowHSSFColWaiZhu.CreateCell(11).SetCellValue(strYear2 + "年10月");
                    nextRowHSSFColWaiZhu.GetCell(11).CellStyle = style4;
                    nextRowHSSFColWaiZhu.CreateCell(12).SetCellValue(strYear2 + "年11月");
                    nextRowHSSFColWaiZhu.GetCell(12).CellStyle = style4;
                    nextRowHSSFColWaiZhu.CreateCell(13).SetCellValue(strYear2 + "年12月");
                    nextRowHSSFColWaiZhu.GetCell(13).CellStyle = style4;
                    nextRowHSSFColWaiZhu.CreateCell(14).SetCellValue(DateTime.Now.ToString("yyyy") + "年合计");
                    nextRowHSSFColWaiZhu.GetCell(14).CellStyle = style4;
                    nextRowHSSFColWaiZhu.CreateCell(15).SetCellValue(strNextOneYear + "年合计");
                    nextRowHSSFColWaiZhu.GetCell(15).CellStyle = style4;
                    nextRowHSSFColWaiZhu.CreateCell(16).SetCellValue(strNextTwoYear + "年合计");
                    nextRowHSSFColWaiZhu.GetCell(16).CellStyle = style4;
                    nextRow++;
                    int heBingStartRow = nextRow;
                    int hebingEndRow = nextRow + (dtWaiZhu.Rows.Count - 1) - 1;
                    mysheetHSSF.AddMergedRegion(new CellRangeAddress(heBingStartRow, hebingEndRow, 0, 0));
                    for (var j = 0; j < dtWaiZhu.Rows.Count - 1; j++)
                    {
                        IRow nextRowHSSFRow = mysheetHSSF.CreateRow(nextRow);
                        nextRowHSSFRow.Height = 34 * 20;
                        if (heBingStartRow == nextRow)
                        {
                            nextRowHSSFRow.CreateCell(0).SetCellValue("");
                            nextRowHSSFRow.GetCell(0).CellStyle = style5;//将CellStyle应用于具体单元格 
                        }
                        else
                        {
                            nextRowHSSFRow.CreateCell(0).SetCellValue("");
                            nextRowHSSFRow.GetCell(0).CellStyle = style5;
                        }
                        int colNum = 1;//用于计数
                        for (var k = 0; k < dtWaiZhu.Columns.Count; k++)
                        {
                            if (k != 1)
                            {
                                if (k == 0)
                                {
                                    nextRowHSSFRow.CreateCell(colNum).SetCellValue(dtWaiZhu.Rows[j][k].ToString());
                                    nextRowHSSFRow.GetCell(colNum).CellStyle = style5;
                                    colNum++;
                                }
                                else {
                                    nextRowHSSFRow.CreateCell(colNum).SetCellValue(Convert.ToDouble(dtWaiZhu.Rows[j][k].ToString()));
                                    nextRowHSSFRow.GetCell(colNum).CellStyle = style5;
                                    colNum++;
                                }    
                                
                            }
                        }
                        nextRow++;
                    }
                    //创建销售数据最后一行
                    #region
                    mysheetHSSF.AddMergedRegion(new CellRangeAddress(nextRow, nextRow, 0, 1));
                    IRow nextRowHSSFLastRow = mysheetHSSF.CreateRow(nextRow);
                    nextRowHSSFLastRow.Height = 34 * 20;
                    nextRowHSSFLastRow.CreateCell(0).SetCellValue("合计");
                    nextRowHSSFLastRow.GetCell(0).CellStyle = style6;//将CellStyle应用于具体单元格 

                    nextRowHSSFLastRow.CreateCell(1).SetCellValue("");
                    nextRowHSSFLastRow.GetCell(1).CellStyle = style6;
                    for (var m = 2; m < dtWaiZhu.Columns.Count; m++)
                    {
                        nextRowHSSFLastRow.CreateCell(m).SetCellValue(Convert.ToDouble(dtWaiZhu.Rows[dtWaiZhu.Rows.Count - 1][m].ToString()));
                        nextRowHSSFLastRow.GetCell(m).CellStyle = style6;
                    }
                    #endregion
                    //每一个个四行
                    nextRow = nextRow + 4;
                }

                #endregion
                #region 设置合计
                if (dthuiZong.Rows.Count > 0)
                {
                    mysheetHSSF.AddMergedRegion(new CellRangeAddress(nextRow, nextRow, 0, 1));
                    IRow nextRowHSSFCol = mysheetHSSF.CreateRow(nextRow);
                    nextRowHSSFCol.Height = 34 * 20;
                    nextRowHSSFCol.CreateCell(0).SetCellValue("【合计】");
                    nextRowHSSFCol.GetCell(0).CellStyle = style2;//将CellStyle应用于具体单元格
                    nextRow++;
                    //设置列
                    mysheetHSSF.AddMergedRegion(new CellRangeAddress(nextRow, nextRow, 0, 1));
                    IRow nextRowHSSFColWaiZhu = mysheetHSSF.CreateRow(nextRow);
                    nextRowHSSFColWaiZhu.Height = 34 * 20;
                    nextRowHSSFColWaiZhu.CreateCell(0).SetCellValue("工程区分");
                    nextRowHSSFColWaiZhu.GetCell(0).CellStyle = style4;
                    nextRowHSSFColWaiZhu.CreateCell(1).SetCellValue("");
                    nextRowHSSFColWaiZhu.GetCell(1).CellStyle = style4;
                    nextRowHSSFColWaiZhu.CreateCell(2).SetCellValue(strYear2 + "年1月");
                    nextRowHSSFColWaiZhu.GetCell(2).CellStyle = style4;
                    nextRowHSSFColWaiZhu.CreateCell(3).SetCellValue(strYear2 + "年2月");
                    nextRowHSSFColWaiZhu.GetCell(3).CellStyle = style4;
                    nextRowHSSFColWaiZhu.CreateCell(4).SetCellValue(strYear2 + "年3月");
                    nextRowHSSFColWaiZhu.GetCell(4).CellStyle = style4;
                    nextRowHSSFColWaiZhu.CreateCell(5).SetCellValue(strYear2 + "年4月");
                    nextRowHSSFColWaiZhu.GetCell(5).CellStyle = style4;
                    nextRowHSSFColWaiZhu.CreateCell(6).SetCellValue(strYear2 + "年5月");
                    nextRowHSSFColWaiZhu.GetCell(6).CellStyle = style4;
                    nextRowHSSFColWaiZhu.CreateCell(7).SetCellValue(strYear2 + "年6月");
                    nextRowHSSFColWaiZhu.GetCell(7).CellStyle = style4;
                    nextRowHSSFColWaiZhu.CreateCell(8).SetCellValue(strYear2 + "年7月");
                    nextRowHSSFColWaiZhu.GetCell(8).CellStyle = style4;
                    nextRowHSSFColWaiZhu.CreateCell(9).SetCellValue(strYear2 + "年8月");
                    nextRowHSSFColWaiZhu.GetCell(9).CellStyle = style4;
                    nextRowHSSFColWaiZhu.CreateCell(10).SetCellValue(strYear2 + "年9月");
                    nextRowHSSFColWaiZhu.GetCell(10).CellStyle = style4;
                    nextRowHSSFColWaiZhu.CreateCell(11).SetCellValue(strYear2 + "年10月");
                    nextRowHSSFColWaiZhu.GetCell(11).CellStyle = style4;
                    nextRowHSSFColWaiZhu.CreateCell(12).SetCellValue(strYear2 + "年11月");
                    nextRowHSSFColWaiZhu.GetCell(12).CellStyle = style4;
                    nextRowHSSFColWaiZhu.CreateCell(13).SetCellValue(strYear2 + "年12月");
                    nextRowHSSFColWaiZhu.GetCell(13).CellStyle = style4;
                    nextRowHSSFColWaiZhu.CreateCell(14).SetCellValue(DateTime.Now.ToString("yyyy") + "年合计");
                    nextRowHSSFColWaiZhu.GetCell(14).CellStyle = style4;
                    nextRowHSSFColWaiZhu.CreateCell(15).SetCellValue(strNextOneYear + "年合计");
                    nextRowHSSFColWaiZhu.GetCell(15).CellStyle = style4;
                    nextRowHSSFColWaiZhu.CreateCell(16).SetCellValue(strNextTwoYear + "年合计");
                    nextRowHSSFColWaiZhu.GetCell(16).CellStyle = style4;
                    nextRow++;

                    for (var j = 0; j < dthuiZong.Rows.Count; j++)
                    {
                        mysheetHSSF.AddMergedRegion(new CellRangeAddress(nextRow, nextRow, 0, 1));
                        IRow nextRowHSSFRow = mysheetHSSF.CreateRow(nextRow);
                        nextRowHSSFRow.Height = 34 * 20;
                        nextRowHSSFRow.CreateCell(0).SetCellValue("合计");
                        nextRowHSSFRow.GetCell(0).CellStyle = style5;//将CellStyle应用于具体单元格 
                        nextRowHSSFRow.CreateCell(1).SetCellValue("");
                        nextRowHSSFRow.GetCell(1).CellStyle = style5;//将CellStyle应用于具体单元格 
                        int colNum = 2;//用于计数
                        for (var k = 2; k < dthuiZong.Columns.Count; k++)
                        {
                            if (k != 1)
                            {
                                nextRowHSSFRow.CreateCell(colNum).SetCellValue(Convert.ToDouble(dthuiZong.Rows[j][k].ToString()));
                                nextRowHSSFRow.GetCell(colNum).CellStyle = style5;
                                colNum++;
                            }
                        }
                        nextRow++;
                    }
                    //创建销售数据最后一行
                    #region
                    mysheetHSSF.AddMergedRegion(new CellRangeAddress(nextRow, nextRow, 0, 1));
                    IRow nextRowHSSFLastRow = mysheetHSSF.CreateRow(nextRow);
                    nextRowHSSFLastRow.Height = 34 * 20;
                    nextRowHSSFLastRow.CreateCell(0).SetCellValue("稼动日");
                    nextRowHSSFLastRow.GetCell(0).CellStyle = style6;//将CellStyle应用于具体单元格 

                    nextRowHSSFLastRow.CreateCell(1).SetCellValue("");
                    nextRowHSSFLastRow.GetCell(1).CellStyle = style6;
                    for (var m = 2; m < dthuiZong.Columns.Count; m++)
                    {
                        nextRowHSSFLastRow.CreateCell(m).SetCellValue("");
                        nextRowHSSFLastRow.GetCell(m).CellStyle = style6;
                    }
                    
                    #endregion
                }
                //以上设置
                IRow nextRowYiShangHSSFCol = mysheetHSSF.CreateRow(nextRow + 2);
                //nextRowHSSFCol.Height = 34 * 20;
                nextRowYiShangHSSFCol.CreateCell(16).SetCellValue("以上");
                nextRowYiShangHSSFCol.GetCell(16).CellStyle = style7;//将CellStyle应用于具体单元格
                #endregion

                string rootPath = _webHostEnvironment.ContentRootPath;
                string strFunctionName = "FS0620_Export";

                string strFileName = strFunctionName + "_导出信息_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + loginInfo.UserId + ".xlsx";
                string fileSavePath = rootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Export" + Path.DirectorySeparatorChar;//文件临时目录，导入完成后 删除
                filepath = fileSavePath+strFileName;

                using (FileStream fs = System.IO.File.OpenWrite(filepath))
                {
                    hssfworkbook.Write(fs);//向打开的这个xls文件中写入数据  
                    fs.Close();
                }
                #endregion
                if (strFileName == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "导出生成文件失败";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = strFileName;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2005", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出报表失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            } 
        }
        #endregion
    }
}

