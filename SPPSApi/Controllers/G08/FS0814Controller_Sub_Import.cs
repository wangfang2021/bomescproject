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
                    {"6","1","0","0","0","0","0","0","0","0","0","0"
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
                    for (int j = 1; j <= 31; j++)
                    {
                        string day = importDt.Rows[i]["vcD" + j + ""].ToString();
                        if (day != "" && day != null && Array.IndexOf(new string[] { "A", "B" }, day) == -1)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "[D" + j + "]列只能填写A/B";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
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
                string strErrorName = "";
                fs0814_Logic.importSave_Sub(importDt, loginInfo.UserId,ref strErrorName);
                if(strErrorName!="")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "以下年月白夜出现相同班值情况："+strErrorName;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
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
