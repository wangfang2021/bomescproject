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
    [Route("api/FS0620_Sub/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0620Controller_Sub : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0620_Logic fs0620_Logic = new FS0620_Logic();
        private readonly string FunctionID = "FS0620";

        public FS0620Controller_Sub(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }


        #region 获取邮件体
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
            string date = dataForm.date == null ? "" : dataForm.date;
            string flag = dataForm.flag == null ? "" : dataForm.flag;


            try
            {
                String emailBody = fs0620_Logic.CreateEmailBody(date, flag, loginInfo.UnitCode, loginInfo.UserName);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = emailBody;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2009", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "生成邮件体失败" + ex.Message;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region FTMS

        [HttpPost]
        [EnableCors("any")]
        public string FTMSApi([FromBody]dynamic data)
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
                JArray listInfo = dataForm.parentFormSelectItem.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
                string EmailBody = dataForm.parentFormSelectItem.emailBody.ToString();
                string vcTargetYear = dataForm.date == null ? "" : dataForm.date;
                if (EmailBody.Length == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "邮件体内容不能为空！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (vcTargetYear.Length == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请先选择对象年,再进行供应商别年计发送操作！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (listInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请先检索数据，再进行年检邮件电送！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                DataTable dt = new DataTable(); 
                dt.Columns.Add("iAutoId");
                dt.Columns.Add("dOperatorTime");
                dt.Columns.Add("vcType");
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
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    DataRow dr = dt.NewRow();
                    dr["iAutoId"] = listInfoData[i]["iAutoId"] == null ? "" : listInfoData[i]["iAutoId"].ToString();
                    dr["dOperatorTime"] = listInfoData[i]["dOperatorTime"] == null ? "" : listInfoData[i]["dOperatorTime"].ToString();
                    dr["vcType"] = listInfoData[i]["vcType"] == null ? "" : listInfoData[i]["vcType"].ToString();
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
                head = new string[] { "导入时间", "年计类型", "包装工场", "对象年份", "品番", "发注工场", "内外", "供应商代码", "工区", "车型", "收容数", "一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月", "对象年合计", "N+1年预测", "N+2年预测" };
                field = new string[] { "dOperatorTime", "vcType", "vcPackPlant", "vcTargetYear", "vcPartNo", "vcInjectionFactory", "vcInsideOutsideType", "vcSupplier_id", "vcWorkArea", "vcCarType", "vcAcceptNum", "vcJanuary", "vcFebruary", "vcMarch", "vcApril", "vcMay", "vcJune", "vcJuly", "vcAugust", "vcSeptember", "vcOctober", "vcNovember", "vcDecember", "vcSum", "vcNextOneYear", "vcNextTwoYear" };
                string path = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Export" + Path.DirectorySeparatorChar;
                StringBuilder strErr = new StringBuilder();
                bool bReault = true;
                //FS0603_Logic fs0603_Logic = new FS0603_Logic();
                DataTable dtMessage = fs0620_Logic.createTable("email");
                for (int i = 0; i < dtSelect.Rows.Count; i++)
                {
                    //组织制定供应商和工区的数据
                    string vcSupplier_id = dtSelect.Rows[i]["vcSupplier_id"].ToString();
                    string vcWorkArea = dtSelect.Rows[i]["vcWorkArea"].ToString();
                    DataRow[] drArray = dt.Select("vcSupplier_id='" + vcSupplier_id + "' and vcWorkArea='" + vcWorkArea + "' ");
                    DataTable dtNewSupplierandWorkArea = drArray[0].Table.Clone(); // 复制DataRow的表结构
                    string msg = string.Empty;
                    foreach (DataRow dr in drArray)
                    {
                        dtNewSupplierandWorkArea.ImportRow(dr);
                    }
                    string strFileName = vcTargetYear + "年补给年计_" + vcSupplier_id + vcWorkArea;
                    string filepath = ComFunction.DataTableToExcel(head, field, dtNewSupplierandWorkArea, ".", loginInfo.UserId, strFileName, ref msg);
                    if (filepath == "")
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcSupplier_id"] = vcSupplier_id;
                        dataRow["vcWorkArea"] = vcWorkArea;
                        dataRow["vcMessage"] = "附件没有生产成功，不能发送邮件";
                        dtMessage.Rows.Add(dataRow);
                        bReault = false;
                        logs = System.DateTime.Now.ToString() + "供应商：" + vcSupplier_id + "工区" + vcWorkArea + "附件没有生产成功，不能发送邮件 \n";
                        strErr.Append(logs);
                        continue;
                    }
                    filepath = path + filepath;
                    string fileSavePath = path + "【展开】" + vcTargetYear + "年" + loginInfo.UnitCode + "补给年计(" + vcSupplier_id + "-" + vcWorkArea + ")_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                    System.IO.File.Copy(filepath, fileSavePath, true);
                    //获取供应商 工区的邮箱
                    DataTable dtEmail = fs0620_Logic.getEmail(vcSupplier_id, vcWorkArea);
                    if (dtEmail.Rows.Count == 0)
                    {
                        logs = System.DateTime.Now.ToString() + "供应商：" + vcSupplier_id + "工区" + vcWorkArea + "没有维护邮箱，不能发送邮件 \n";
                        strErr.Append("供应商：" + vcSupplier_id + "工区" + vcWorkArea + "缺少邮箱，不能发送!");
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcSupplier_id"] = vcSupplier_id;
                        dataRow["vcWorkArea"] = vcWorkArea;
                        dataRow["vcMessage"] = "没有维护邮箱，不能发送邮件";
                        dtMessage.Rows.Add(dataRow);
                        bReault = false;
                        continue;
                    }
                    if (dtEmail.Rows.Count > 1)
                    {
                        logs = System.DateTime.Now.ToString() + "供应商：" + vcSupplier_id + "工区" + vcWorkArea + "邮箱维护冗余有误，不能发送邮件 \n";
                        strErr.Append("供应商：" + vcSupplier_id + "工区" + vcWorkArea + "邮箱冗余,不能发送!");
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcSupplier_id"] = vcSupplier_id;
                        dataRow["vcWorkArea"] = vcWorkArea;
                        dataRow["vcMessage"] = "邮箱维护冗余有误，不能发送邮件";
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
                    if (string.IsNullOrEmpty(strEmail1) && string.IsNullOrEmpty(strEmail2) && string.IsNullOrEmpty(strEmail3))
                    {
                        strErr.Append("供应商：" + vcSupplier_id + "工区" + vcWorkArea + "邮箱为空,不能发送!");
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcSupplier_id"] = vcSupplier_id;
                        dataRow["vcWorkArea"] = vcWorkArea;
                        dataRow["vcMessage"] = "邮箱为空，不能发送邮件";
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
                    if (strEmail1.Length > 0)
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
                    if (dtCCEmail.Rows.Count > 0)
                    {
                        cCDt = dtCCEmail;
                    }
                    //邮件主题 【展开】2021年TFTM补给年计（2730-0）
                    string strSubject = "【展开】"  + vcTargetYear + "年"+loginInfo.UnitCode + "补给年计(" + vcSupplier_id +"-" +vcWorkArea + ")";

                    string result = ComFunction.SendEmailInfo(loginInfo.Email, loginInfo.UnitName, EmailBody, receiverDt, cCDt, strSubject, fileSavePath, false);
                    if (result == "Success")
                    {
                        fs0620_Logic.updateEmailState(dtNewSupplierandWorkArea);
                    }
                    else
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcSupplier_id"] = vcSupplier_id;
                        dataRow["vcWorkArea"] = vcWorkArea;
                        dataRow["vcMessage"] = "邮箱发送失败，邮件发送公共方法未知原因！";
                        dtMessage.Rows.Add(dataRow);
                        bReault = false;
                    }
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
                    apiResult.data = strErr.ToString() + ",其余发送成功！";
                }
                else
                {
                    apiResult.data = "邮件发送成功！";
                }
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2010", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "邮件发送失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }

        }
        #endregion
       
    }
}