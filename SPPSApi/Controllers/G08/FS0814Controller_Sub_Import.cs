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

namespace SPPSApi.Controllers.G08
{
    [Route("api/FS0814_Sub_Import/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0814Controller_Sub_Import : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0814_Logic fs0814_Logic = new FS0814_Logic();
        private readonly string FunctionID = "FS0814";

        public FS0814Controller_Sub_Import(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }


        #region 导入之后点保存
        [HttpPost]
        [EnableCors("any")]
        public string importSaveApi([FromBody]dynamic data)
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
                string strMsg = "";
                string[,] headers = new string[,] {
                    {"对象年月","白夜","D1","D2","D3","D4","D5","D6","D7","D8","D9","D10",
                    "D11","D12","D13","D14","D15","D16","D17","D18","D19","D20",
                    "D21","D22","D23","D24","D25","D26","D27","D28","D29","D30","D31"},
                    {"vcYearMonth", "vcType", "vcD1", "vcD2","vcD3","vcD4","vcD5","vcD6","vcD7","vcD8","vcD9","vcD10",
                    "vcD11","vcD12","vcD13","vcD14","vcD15","vcD16","vcD17","vcD18","vcD19","vcD20",
                    "vcD21","vcD22","vcD23","vcD24","vcD25","vcD26","vcD27","vcD28","vcD29","vcD30","vcD31"},
                    {FieldCheck.Num,"",FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,
                    FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,
                    FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char,FieldCheck.Char},
                    {"6","1","1","1","1","1","1","1","1","1","1","1"
                    ,"1","1","1","1","1","1","1","1","1","1"
                    ,"1","1","1","1","1","1","1","1","1","1","1"},//最大长度设定,不校验最大长度用0
                    {"1","1","0","0","0","0","0","0","0","0","0","0"
                    ,"0","0","0","0","0","0","0","0","0","0"
                    ,"0","0","0","0","0","0","0","0","0","0","0"}};//最小长度设定,可以为空用0
                DataTable importDt = new DataTable();
                foreach (FileInfo info in theFolder.GetFiles())
                {
                    DataTable dt = ComFunction.ExcelToDataTable(info.FullName, "sheet1", headers, ref strMsg);
                    if (strMsg != "")
                    {
                        ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "导入终止，文件" + info.Name + ":" + strMsg;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (importDt.Columns.Count == 0)
                        importDt = dt.Clone();
                    if (dt.Rows.Count == 0)
                    {
                        ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "导入终止，文件" + info.Name + "没有要导入的数据";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    foreach (DataRow row in dt.Rows)
                    {
                        importDt.ImportRow(row);
                    }
                }
                ComFunction.DeleteFolder(fileSavePath);//读取数据后删除文件夹

                #region 文件内容校验
                for (int i = 0; i < importDt.Rows.Count; i++)
                {
                    string vcType = importDt.Rows[i]["vcType"].ToString();
                    if (Array.IndexOf(new string[] { "白", "夜" }, vcType) == -1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "[白夜]列只能填写白/夜";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    string vcD1 = importDt.Rows[i]["vcD1"].ToString();
                    if (vcD1!="" && Array.IndexOf(new string[] { "A","B"},vcD1)==-1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "[D1]列只能填写A/B";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    string vcD2 = importDt.Rows[i]["vcD2"].ToString();
                    if (vcD2 != "" && Array.IndexOf(new string[] { "A", "B" }, vcD2) == -1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "[D2]列只能填写A/B";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    string vcD3 = importDt.Rows[i]["vcD3"].ToString();
                    if (vcD3 != "" && Array.IndexOf(new string[] { "A", "B" }, vcD3) == -1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "[D3]列只能填写A/B";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    string vcD4 = importDt.Rows[i]["vcD4"].ToString();
                    if (vcD4 != "" && Array.IndexOf(new string[] { "A", "B" }, vcD4) == -1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "[D4]列只能填写A/B";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    string vcD5 = importDt.Rows[i]["vcD5"].ToString();
                    if (vcD5 != "" && Array.IndexOf(new string[] { "A", "B" }, vcD5) == -1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "[D5]列只能填写A/B";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    string vcD6 = importDt.Rows[i]["vcD6"].ToString();
                    if (vcD6 != "" && Array.IndexOf(new string[] { "A", "B" }, vcD6) == -1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "[D6]列只能填写A/B";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    string vcD7 = importDt.Rows[i]["vcD7"].ToString();
                    if (vcD7 != "" && Array.IndexOf(new string[] { "A", "B" }, vcD7) == -1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "[D7]列只能填写A/B";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    string vcD8 = importDt.Rows[i]["vcD8"].ToString();
                    if (vcD8 != "" && Array.IndexOf(new string[] { "A", "B" }, vcD8) == -1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "[D8]列只能填写A/B";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    string vcD9 = importDt.Rows[i]["vcD9"].ToString();
                    if (vcD9 != "" && Array.IndexOf(new string[] { "A", "B" }, vcD9) == -1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "[D9]列只能填写A/B";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    string vcD10 = importDt.Rows[i]["vcD10"].ToString();
                    if (vcD10 != "" && Array.IndexOf(new string[] { "A", "B" }, vcD10) == -1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "[D10]列只能填写A/B";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    string vcD11 = importDt.Rows[i]["vcD11"].ToString();
                    if (vcD11 != "" && Array.IndexOf(new string[] { "A", "B" }, vcD11) == -1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "[D11]列只能填写A/B";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    string vcD12 = importDt.Rows[i]["vcD12"].ToString();
                    if (vcD12 != "" && Array.IndexOf(new string[] { "A", "B" }, vcD12) == -1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "[D12]列只能填写A/B";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    string vcD13 = importDt.Rows[i]["vcD13"].ToString();
                    if (vcD13 != "" && Array.IndexOf(new string[] { "A", "B" }, vcD13) == -1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "[D13]列只能填写A/B";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    string vcD14 = importDt.Rows[i]["vcD14"].ToString();
                    if (vcD14 != "" && Array.IndexOf(new string[] { "A", "B" }, vcD14) == -1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "[D14]列只能填写A/B";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    string vcD15 = importDt.Rows[i]["vcD15"].ToString();
                    if (vcD15 != "" && Array.IndexOf(new string[] { "A", "B" }, vcD15) == -1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "[D15]列只能填写A/B";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    string vcD16 = importDt.Rows[i]["vcD16"].ToString();
                    if (vcD16 != "" && Array.IndexOf(new string[] { "A", "B" }, vcD16) == -1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "[D16]列只能填写A/B";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    string vcD17 = importDt.Rows[i]["vcD17"].ToString();
                    if (vcD17 != "" && Array.IndexOf(new string[] { "A", "B" }, vcD17) == -1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "[D17]列只能填写A/B";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    string vcD18 = importDt.Rows[i]["vcD18"].ToString();
                    if (vcD18 != "" && Array.IndexOf(new string[] { "A", "B" }, vcD18) == -1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "[D18]列只能填写A/B";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    string vcD19 = importDt.Rows[i]["vcD19"].ToString();
                    if (vcD19 != "" && Array.IndexOf(new string[] { "A", "B" }, vcD19) == -1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "[D19]列只能填写A/B";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    string vcD20 = importDt.Rows[i]["vcD20"].ToString();
                    if (vcD20 != "" && Array.IndexOf(new string[] { "A", "B" }, vcD20) == -1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "[D20]列只能填写A/B";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    string vcD21 = importDt.Rows[i]["vcD21"].ToString();
                    if (vcD21 != "" && Array.IndexOf(new string[] { "A", "B" }, vcD21) == -1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "[D21]列只能填写A/B";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    string vcD22 = importDt.Rows[i]["vcD22"].ToString();
                    if (vcD22 != "" && Array.IndexOf(new string[] { "A", "B" }, vcD22) == -1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "[D22]列只能填写A/B";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    string vcD23 = importDt.Rows[i]["vcD23"].ToString();
                    if (vcD23 != "" && Array.IndexOf(new string[] { "A", "B" }, vcD23) == -1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "[D23]列只能填写A/B";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    string vcD24 = importDt.Rows[i]["vcD24"].ToString();
                    if (vcD24 != "" && Array.IndexOf(new string[] { "A", "B" }, vcD24) == -1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "[D24]列只能填写A/B";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    string vcD25 = importDt.Rows[i]["vcD25"].ToString();
                    if (vcD25 != "" && Array.IndexOf(new string[] { "A", "B" }, vcD25) == -1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "[D25]列只能填写A/B";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    string vcD26 = importDt.Rows[i]["vcD26"].ToString();
                    if (vcD26 != "" && Array.IndexOf(new string[] { "A", "B" }, vcD26) == -1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "[D26]列只能填写A/B";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    string vcD27 = importDt.Rows[i]["vcD27"].ToString();
                    if (vcD27 != "" && Array.IndexOf(new string[] { "A", "B" }, vcD27) == -1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "[D27]列只能填写A/B";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    string vcD28 = importDt.Rows[i]["vcD28"].ToString();
                    if (vcD28 != "" && Array.IndexOf(new string[] { "A", "B" }, vcD28) == -1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "[D28]列只能填写A/B";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    string vcD29 = importDt.Rows[i]["vcD29"].ToString();
                    if (vcD29 != "" && Array.IndexOf(new string[] { "A", "B" }, vcD29) == -1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "[D29]列只能填写A/B";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    string vcD30 = importDt.Rows[i]["vcD30"].ToString();
                    if (vcD30 != "" && Array.IndexOf(new string[] { "A", "B" }, vcD30) == -1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "[D30]列只能填写A/B";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    string vcD31 = importDt.Rows[i]["vcD31"].ToString();
                    if (vcD31 != "" && Array.IndexOf(new string[] { "A", "B" }, vcD31) == -1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "[D31]列只能填写A/B";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                #endregion

                var result = from r in importDt.AsEnumerable()
                             group r by new { r2 = r.Field<string>("vcYearMonth"), r3 = r.Field<string>("vcType") } into g
                             where g.Count() > 1
                             select g;
                if (result.Count() > 0)
                {
                    StringBuilder sbr = new StringBuilder();
                    sbr.Append("导入数据重复:<br/>");
                    foreach (var item in result)
                    {
                        sbr.Append("年月:" + item.Key.r2 + " 白夜:" + item.Key.r3 + "<br/>");
                    }
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = sbr.ToString();
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                fs0814_Logic.importSave_Sub(importDt, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "保存成功";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0905", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败" + ex.Message;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion



    }
}
