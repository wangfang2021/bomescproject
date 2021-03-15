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
    [Route("api/FS0604_Sub/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0604Controller_Sub : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0604_Logic fs0604_Logic = new FS0604_Logic();
        private readonly string FunctionID = "FS0604";

        public FS0604Controller_Sub(IWebHostEnvironment webHostEnvironment)
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
                String emailBody = fs0604_Logic.CreateEmailBody(date, flag, loginInfo.UnitCode, loginInfo.UserName);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = emailBody;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0408", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "生成邮件体失败" + ex.Message;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

       /* #region 账票展开
        [HttpPost]
        [EnableCors("any")]
        public string ZPFXApi([FromBody]dynamic data)
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
                string EmailBody = dataForm.emailBody.ToString();

                bool hasFind = false;//是否找到需要新增或者修改的数据
                if (listInfoData.Count > 0)
                {
                    hasFind = true;
                }
                if (!hasFind)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选择一条数据！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                string strMsg = "";
                string path = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Export" + Path.DirectorySeparatorChar;
                fs0307_Logic.ZKZP(listInfoData, loginInfo.UserId, EmailBody, _webHostEnvironment.ContentRootPath, ref strMsg, loginInfo.Email, loginInfo.UnitName, path);

                if (!strMsg.Equals("账票展开成功。"))
                {
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.data = strMsg;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "账票展开成功。";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0709", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "账票展开失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }

        #endregion*/

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
                string dExpectDeliveryDate = dataForm.date == null ? "" : dataForm.date;
                if (EmailBody.Length == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "邮件体内容不能为空！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (dExpectDeliveryDate.Length == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "回复纳期不能为空！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                bool hasFind = false;//是否找到需要新增或者修改的数据

                if (listInfoData.Count > 0)
                {
                    hasFind = true;
                }

                if (!hasFind)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选中一条数据！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (listInfoData[i]["vcState"].ToString() != "未发送")
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = listInfoData[i]["vcPartNo"] + "状态不正确,必须是未发送，才能进行荷姿展开操作！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (listInfoData[i]["vcExpectIntake"].ToString().Trim().Length == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "请填写品番" + listInfoData[i]["vcPartNo"] + "的要望收容数，才能进行荷姿展开操作！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                //构建 Datablt
                DataTable dt = new DataTable();
                //"dExportDate", "", "", "", ""
                dt.Columns.Add("iAutoId");
                dt.Columns.Add("vcSupplier_id");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    DataRow dr = dt.NewRow();
                    dr["iAutoId"] = listInfoData[i]["iAutoId"] == null ? "" : listInfoData[i]["iAutoId"].ToString();
                    dr["vcSupplier_id"] = listInfoData[i]["vcSupplier_id"] == null ? "" : listInfoData[i]["vcSupplier_id"].ToString();
                    dt.Rows.Add(dr);
                }
                string[] columnArray = { "vcSupplier_id" };
                DataView dtSelectView = dt.DefaultView;
                DataTable dtSelect = dtSelectView.ToTable(true, columnArray);//去重后的dt 
                bool bReault = true;
                FS0603_Logic fs0603_Logic = new FS0603_Logic();
                DataTable dtMessage = fs0603_Logic.createTable("MES");
                for (int i = 0; i < dtSelect.Rows.Count; i++)
                {
                    string strSupplier = dtSelect.Rows[i]["vcSupplier_id"].ToString();
                    DataTable dtCheckSupplier = fs0604_Logic.CheckEmail(strSupplier);
                    if (dtCheckSupplier.Rows.Count == 0)
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = "供应商" + strSupplier + "在供应商表中信息不存在，请维护信息及其邮箱";
                        dtMessage.Rows.Add(dataRow);
                        bReault = false;
                    }
                    else
                    {
                        bool isYouXiaoEmail = false;
                        for (int j = 0; j < dtCheckSupplier.Rows.Count; j++)
                        {
                            if (dtCheckSupplier.Rows[j]["vcEmail1"].ToString() != "" || dtCheckSupplier.Rows[j]["vcEmail2"].ToString() != "" || dtCheckSupplier.Rows[j]["vcEmail3"].ToString() != "")
                            {
                                isYouXiaoEmail = true;
                                break;
                            }
                        }
                        if (!isYouXiaoEmail)
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = "供应商" + strSupplier + "在供应商表中邮箱信息不存在，请维护邮箱";
                            dtMessage.Rows.Add(dataRow);
                            bReault = false;
                        }
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
                DataTable dtMessageEmail = fs0603_Logic.createTable("MES");
                bool bReaultEmail = true;
                for (int i = 0; i < dtSelect.Rows.Count; i++)
                {
                    string strSupplier = dtSelect.Rows[i]["vcSupplier_id"].ToString();
                    DataTable dtCheckSupplier = fs0604_Logic.CheckEmail(strSupplier);
                    DataRow[] drArray = dt.Select("vcSupplier_id='" + strSupplier + "' ");
                    DataTable dtNewSupplierand = drArray[0].Table.Clone(); // 复制DataRow的表结构
                    string msg = string.Empty;
                    foreach (DataRow dr in drArray)
                    {
                        dtNewSupplierand.ImportRow(dr);
                    }
                    fs0604_Logic.hZZK(dtNewSupplierand, dExpectDeliveryDate, loginInfo.UserId);
                    //收件人dt
                    DataTable receiverDt = new DataTable();
                    receiverDt.Columns.Add("address");
                    receiverDt.Columns.Add("displayName");
                    for (int j = 0; j < dtCheckSupplier.Rows.Count; j++)
                    {
                        if (dtCheckSupplier.Rows[j]["vcEmail1"].ToString().Length > 0)
                        {
                            DataRow dr = receiverDt.NewRow();
                            dr["address"] = dtCheckSupplier.Rows[j]["vcEmail1"].ToString();
                            dr["displayName"] = dtCheckSupplier.Rows[j]["vcLinkMan1"].ToString();
                            receiverDt.Rows.Add(dr);
                        }
                        if (dtCheckSupplier.Rows[j]["vcEmail2"].ToString().Length > 0)
                        {
                            DataRow dr = receiverDt.NewRow();
                            dr["address"] = dtCheckSupplier.Rows[j]["vcEmail2"].ToString();
                            dr["displayName"] = dtCheckSupplier.Rows[j]["vcLinkMan2"].ToString();
                            receiverDt.Rows.Add(dr);
                        }
                        if (dtCheckSupplier.Rows[j]["vcEmail3"].ToString().Length > 0)
                        {
                            DataRow dr = receiverDt.NewRow();
                            dr["address"] = dtCheckSupplier.Rows[j]["vcEmail3"].ToString();
                            dr["displayName"] = dtCheckSupplier.Rows[j]["vcLinkMan3"].ToString();
                            receiverDt.Rows.Add(dr);
                        }
                    }
                    
                    string result = "Success";
                    //邮件主题
                    string strSubject = "荷姿展开邮件";
                    result = ComFunction.SendEmailInfo(loginInfo.Email, loginInfo.UnitName, EmailBody, receiverDt, null, strSubject, "", false);
                    if (result == "Success")
                    {
                    }
                    else
                    {
                        DataRow dataRowEmail = dtMessageEmail.NewRow();
                        dataRowEmail["vcMessage"] = "供应商" + strSupplier + "发送邮件失败";
                        dtMessageEmail.Rows.Add(dataRowEmail);
                        bReaultEmail = false;
                    }
                }
                if (!bReaultEmail)
                {
                    //弹出错误dtMessage
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessageEmail;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0410", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "荷姿展开失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
       
    }
}