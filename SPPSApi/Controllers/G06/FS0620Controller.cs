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
using NPOI.SS.UserModel;
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

                List<Object> dataList_C000 = ComFunction.convertAllToResult(ComFunction.getTCode("C000"));//外注工厂
                List<Object> dataList_C003 = ComFunction.convertAllToResult(ComFunction.getTCode("C003"));//内外区分

                res.Add("C000", dataList_C000);
                res.Add("C003", dataList_C003);

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
            string vcTargetYear = dataForm.vcTargetYear == null ? "" : dataForm.vcTargetYear;
            string vcPartNo = dataForm.vcPartNo == null ? "" : dataForm.vcPartNo;
            string vcInjectionFactory = dataForm.vcInjectionFactory == null ? "" : dataForm.vcInjectionFactory;
            string vcInsideOutsideType = dataForm.vcInsideOutsideType == null ? "" : dataForm.vcInsideOutsideType;
            string vcSupplier_id = dataForm.vcSupplier_id == null ? "" : dataForm.vcSupplier_id;
            string vcWorkArea = dataForm.vcWorkArea == null ? "" : dataForm.vcWorkArea;
            string vcCarType = dataForm.vcCarType == null ? "" : dataForm.vcCarType;
            try
            {
                DataTable dt = fs0620_Logic.Search(vcTargetYear, vcPartNo, vcInjectionFactory, vcInsideOutsideType, vcSupplier_id, vcWorkArea, vcCarType);
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
            string vcTargetYear = dataForm.vcTargetYear == null ? "" : dataForm.vcTargetYear;
            string vcPartNo = dataForm.vcPartNo == null ? "" : dataForm.vcPartNo;
            string vcInjectionFactory = dataForm.vcInjectionFactory == null ? "" : dataForm.vcInjectionFactory;
            string vcInsideOutsideType = dataForm.vcInsideOutsideType == null ? "" : dataForm.vcInsideOutsideType;
            string vcSupplier_id = dataForm.vcSupplier_id == null ? "" : dataForm.vcSupplier_id;
            string vcWorkArea = dataForm.vcWorkArea == null ? "" : dataForm.vcWorkArea;
            string vcCarType = dataForm.vcCarType == null ? "" : dataForm.vcCarType;
            try
            {
                DataTable dt = fs0620_Logic.Search(vcTargetYear, vcPartNo, vcInjectionFactory, vcInsideOutsideType, vcSupplier_id, vcWorkArea, vcCarType);
                string[] head = new string[] { };
                string[] field = new string[] { };
                //[vcPartNo], [dBeginDate], [dEndDate]
                head = new string[] { "导入时间", "包装工厂", "对象年份", "品番", "发注工厂", "内外", "供应商代码", "工区", "车型", "收容数", "一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月", "对象年合计", "N+1年预测", "N+2年预测" };
                field = new string[] { "dOperatorTime", "vcPackPlant", "vcTargetYear", "vcPartNo", "vcInjectionFactory", "vcInsideOutsideType", "vcSupplier_id", "vcWorkArea", "vcCarType", "vcAcceptNum", "vcJanuary", "vcFebruary", "vcMarch", "vcApril", "vcMay", "vcJune", "vcJuly", "vcAugust", "vcSeptember", "vcOctober", "vcNovember", "vcDecember", "vcSum", "vcNextOneYear", "vcNextTwoYear" };
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2003", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
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
                        continue;
                    }
                    if (dtEmail.Rows.Count>1)
                    {
                        logs = System.DateTime.Now.ToString() + "供应商：" + vcSupplier_id + "工区" + vcWorkArea + "邮箱维护冗余有误，不能发送邮件 \n";
                        writeLog(logs, logName, loginInfo.UnitCode, loginInfo.UserId);
                        strErr.Append("供应商：" + vcSupplier_id + "工区" + vcWorkArea + "邮箱冗余,不能发送!");
                        continue;
                    }
                    string strdisplayName = dtEmail.Rows[0]["vcLinkMan"].ToString();
                    if (string.IsNullOrEmpty(strdisplayName))
                    {
                        strdisplayName = "";
                    }
                    string strEmail = dtEmail.Rows[0]["vcEmail"].ToString();
                    if (strEmail == "" || string.IsNullOrEmpty(strEmail))
                    {
                        logs = System.DateTime.Now.ToString() + "供应商：" + vcSupplier_id + "工区" + vcWorkArea + "邮箱为空，不能发送邮件 \n";
                        writeLog(logs, logName, loginInfo.UnitCode, loginInfo.UserId);
                        strErr.Append("供应商：" + vcSupplier_id + "工区" + vcWorkArea + "邮箱为空,不能发送!");
                        continue;
                    }
                    string[] emailArray = strEmail.Split(';');
                    //收件人dt
                    DataTable receiverDt = new DataTable();
                    receiverDt.Columns.Add("address");
                    receiverDt.Columns.Add("displayName");
                    for (int j = 0; j < emailArray.Length; j++) {
                        if (emailArray[j].ToString().Length>0)
                        {
                            DataRow dr = receiverDt.NewRow();
                            dr["address"] = emailArray[j].ToString();
                            dr["displayName"] = strdisplayName;
                            receiverDt.Rows.Add(dr);
                        }
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
                    strEmailBody += "<div style='font-family:宋体;font-size:12'>" + "供应商" + vcSupplier_id + " <br /><br />";
                    strEmailBody += "  您好 " + strdisplayName + "<br /><br />";
                    strEmailBody += "  此次邮件内容为事业体"+loginInfo.UnitCode+"年计管理信息，具体内容请查看附！<br /><br />";
                    //strEmailBody += "  请在1个工作日内将是否可以供货的确认结果邮件回复，以下是各个仓库对应的邮箱：<br />";
                    
                    string result= ComFunction.SendEmailInfo(loginInfo.Email, loginInfo.UnitName, strEmailBody, receiverDt, cCDt, strSubject, filepath, false);
                    if (result=="Success")
                    {
                        logs = System.DateTime.Now.ToString() + "供应商：" + vcSupplier_id + "工区" + vcWorkArea + "邮箱发送成功！\n";
                    }
                    else
                    {
                        logs = System.DateTime.Now.ToString() + "供应商：" + vcSupplier_id + "工区" + vcWorkArea + "邮箱发送失败，邮件发送公共方法未知原因！\n";
                    }
                    writeLog(logs, logName, loginInfo.UnitCode, loginInfo.UserId);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0409", ex, loginInfo.UserId);
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

    }
}

