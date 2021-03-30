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
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace SPPSApi.Controllers.G04
{
    [Route("api/FS0402_Sub_Import/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0402Controller_Sub_Import : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0402_Logic fs0402_Logic = new FS0402_Logic();
        private readonly string FunctionID = "FS0402";

        public FS0402Controller_Sub_Import(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }


        #region 导入之后点保存
        [HttpPost]
        [EnableCors("any")]
        public string importSaveApi([FromBody] dynamic data)
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

            if (dataForm.YearMonth == null || dataForm.YearMonth == "")
            {
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "请选择对象年月。";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }

            string strYearMonth = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth).ToString("yyyyMM");
            string strYearMonth_2 = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth).AddMonths(1).ToString("yyyyMM");
            string strYearMonth_3 = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth).AddMonths(2).ToString("yyyyMM");

            string strMonth = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth).ToString("MM");
            string strMonth_2 = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth).AddMonths(1).ToString("MM");
            string strMonth_3 = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth).AddMonths(2).ToString("MM");

            //校验：导入后不能再导入
            if (fs0402_Logic.isRepeatImport(strYearMonth))
            {
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "对象年月："+strYearMonth+"曾经导入过，不能再导入。";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }

            JArray fileNameList = dataForm.fileNameList;
            string hashCode = dataForm.hashCode;
            string fileSavePath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "upload" + Path.DirectorySeparatorChar + hashCode + Path.DirectorySeparatorChar;
            try
            {
                if (!Directory.Exists(fileSavePath))
                {
                    ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "没有要导入的文件，请重新上传！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                DirectoryInfo theFolder = new DirectoryInfo(fileSavePath);
                int iMonth_column = Convert.ToInt32(strYearMonth.Substring(4, 2));//对象月
                int iMonth_2_column = Convert.ToInt32(strYearMonth_2.Substring(4, 2));//内示月
                int iMonth_3_column = Convert.ToInt32(strYearMonth_3.Substring(4, 2));//内内示月
                string[,] headers = new string[,] {{"品番", iMonth_column + "月", iMonth_2_column + "月", iMonth_3_column + "月"},
                                                {"vcPart_id", "iCbSOQN", "iCbSOQN1", "iCbSOQN2"},
                                                {FieldCheck.NumCharLLL,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num},
                                                {"12","0","0","0"},//最大长度设定,不校验最大长度用0
                                                {"1","1","1","1"}};//最小长度设定,可以为空用0
                //记录导入错误消息 (string,string) -- (品番,错误消息)
                Dictionary<string, string> errMessageDict = new Dictionary<string, string>(new RepeatDictionaryComparer());
                string strMsg = "";
                DataTable importDt = new DataTable();
                foreach (FileInfo info in theFolder.GetFiles())
                {
                    DataTable dt = fs0402_Logic.ExcelToDataTable(info.FullName, "sheet1", headers, ref strMsg);
                    if (strMsg != "")
                    {
                        ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = strMsg;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (importDt.Columns.Count == 0)
                        importDt = dt.Clone();
                    if (dt.Rows.Count == 0 || dt.Rows.Count == 1)
                    {
                        ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "没有要导入的数据";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }

                    //验证对象月是否是选择的月份
                    string strMonth_import = dt.Rows[0][1] == System.DBNull.Value ? "0" : dt.Rows[0][1].ToString().Replace("月", "");
                    string strMonth_import_2 = dt.Rows[0][2] == System.DBNull.Value ? "0" : dt.Rows[0][2].ToString().Replace("月", "");
                    string strMonth_import_3 = dt.Rows[0][3] == System.DBNull.Value ? "0" : dt.Rows[0][3].ToString().Replace("月", "");

                    int iMonth = Convert.ToInt32(strMonth);
                    int iMonth_2 = Convert.ToInt32(strMonth_2);
                    int iMonth_3 = Convert.ToInt32(strMonth_3);

                    if (iMonth != Convert.ToInt32(strMonth_import))
                    {
                        //errMessageDict.Add("", "文件中的对象月" + strMonth_import + "与页面输入的对象年月" + iMonth + "不相符！");
                        ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "B列标题行不正确，应该是" + iMonth + "月";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (iMonth_2 != Convert.ToInt32(strMonth_import_2))
                    {
                        //errMessageDict.Add("","文件中的内示月" + strMonth_import_2 + "与页面输入的对象年月" + iMonth_2 + "不相符！");
                        ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "C列标题行不正确，应该是" + iMonth_2 + "月";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (iMonth_3 != Convert.ToInt32(strMonth_import_3))
                    {
                        //errMessageDict.Add("","文件中的内内示月" + strMonth_import_3 + "与页面输入的对象年月" + iMonth_3 + "不相符！");
                        ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "D列标题行不正确，应该是" + iMonth_3 + "月";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    for (int i = 1; i < dt.Rows.Count; i++)//跳过列头
                    {
                        DataRow row = dt.Rows[i];
                        importDt.ImportRow(row);
                    }
                }
                ComFunction.DeleteFolder(fileSavePath);//读取数据后删除文件夹
                List<string> errMonthList = new List<string>();//记录年月错误的品番

                //check:品番行数是否唯一
                var result = from r in importDt.AsEnumerable()
                             group r by new { r2 = r.Field<string>("vcPart_id") } into g
                             where g.Count() > 1
                             select g;
                if (result.Count() > 0)
                {
                    //StringBuilder sbr = new StringBuilder();
                    //sbr.Append("导入数据重复:<br/>");
                    foreach (var item in result)
                    {
                        //sbr.Append("品番:" + item.Key.r2 + "<br/>");
                        errMessageDict.Add(item.Key.r2, "导入数据重复");
                    }
                    //errMessageList.Add(sbr.ToString());
                }
                //导入批量校验
                fs0402_Logic.importCheck(importDt, loginInfo.UserId, strYearMonth, strYearMonth_2, strYearMonth_3, ref errMessageDict, loginInfo.UnitCode);
                if (errMessageDict.Count > 0)
                {
                    fs0402_Logic.importHistory(strYearMonth, errMessageDict);
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "发现问题数据，导入终止，请查看导入履历。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string strResult = "";
                fs0402_Logic.importSave(importDt, loginInfo.UserId, strYearMonth, strYearMonth_2, strYearMonth_3, loginInfo.UnitCode);
                strResult = "导入成功\\";
                //导入成功发送邮件给TFTM担当
                string strMailMsg = "";
                SendEmail(loginInfo.UserId, loginInfo.UserName, strYearMonth, strYearMonth_2, strYearMonth_3, ref strMailMsg);
                if (strMailMsg != "")
                {
                    strResult += "发送邮件失败";
                }
                else
                {
                    strResult += "发送邮件成功";
                }

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = strResult;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M04UE0204", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导入失败" + ex.Message;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        public void SendEmail(string strSendUserId, string strSendUserName, string strYearMonth, string strYearMonth_2, string strYearMonth_3, ref string strMsg)
        {
            string strSendEmail = fs0402_Logic.getEmail(strSendUserId);
            if (strSendEmail == "")
            {
                strMsg = string.Format("用户{0}没有配置邮箱。", strSendUserName);
                return;
            }
            string strContent = string.Format("FTMS已导入SOQ数据({0}、{1}、{2})。", strYearMonth, strYearMonth_2, strYearMonth_3);
            DataTable receiverDt = fs0402_Logic.getReciveEmail();
            if (receiverDt.Rows.Count == 0)
            {
                strMsg = string.Format("tcode(C050)中没有找到收件人信息。");
                return;
            }
            string strTitle = "FTMS已导入SOQ数据成功";
            string response = ComFunction.SendEmailInfo(strSendEmail, strSendUserName, strContent, receiverDt, null, strTitle, "", false);
            if (response == "Error")
            {
                strMsg = string.Format("邮件发送失败。");
                return;
            }
        }

        #region 导出
        [HttpPost]
        [EnableCors("any")]
        public string exportModuleApi([FromBody] dynamic data)
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
            if (dataForm.YearMonth == null || dataForm.YearMonth == "")
            {
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "请选择对象年月。";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }

            string strYearMonth = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth).ToString("yyyyMM");
            string strYearMonth_2 = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth).AddMonths(1).ToString("yyyyMM");
            string strYearMonth_3 = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth).AddMonths(2).ToString("yyyyMM");

            try
            {
                string filepath = fs0402_Logic.generateExcelWithXlt_Module(_webHostEnvironment.ContentRootPath, "FS0402.xlsx", loginInfo.UserId, FunctionID, strYearMonth, strYearMonth_2, strYearMonth_3);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M04UE0205", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

    }

    //为了让dictionary<string,string>（品番,错误消息）的key可以重复，因为一个品番可以有多条错误信息数据
    public class RepeatDictionaryComparer:IEqualityComparer<string>
    {
        public bool Equals(string x,string y)
        {
            return x != y;
        }
        public int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }
    }
}
