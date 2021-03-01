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
    [Route("api/FS0607_Sub_Import/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0607Controller_Sub_Import : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0607_Logic fs0607_Logic = new FS0607_Logic();
        private readonly string FunctionID = "FS0607";

        public FS0607Controller_Sub_Import(IWebHostEnvironment webHostEnvironment)
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
                string[,] headers = new string[,] {{"供应商代码","工区", "纳入日-开始", "纳入日-结束","备注"},
                                                { "vcSupplier_id","vcWorkArea", "dBeginDate", "dEndDate","vcMemo"},
                                                {FieldCheck.NumCharLLL,"",FieldCheck.Date,FieldCheck.Date,""},
                                                {"4","50","0","0","500"},//最大长度设定,不校验最大长度用0
                                                {"4","1","1","1","0"}};//最小长度设定,可以为空用0
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


                var result = from r in importDt.AsEnumerable()
                             group r by new { r2 = r.Field<string>("vcSupplier_id"), r3 = r.Field<string>("vcWorkArea") } into g
                             where g.Count() > 1
                             select g;
                if (result.Count() > 0)
                {
                    StringBuilder sbr = new StringBuilder();
                    sbr.Append("导入数据重复:<br/>");
                    foreach (var item in result)
                    {
                        sbr.Append("供应商代码:" + item.Key.r2 + " 工区:" + item.Key.r3 + "<br/>");
                    }
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = sbr.ToString();
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                StringBuilder sbr1 = new StringBuilder();
                for (int i = 0; i < importDt.Rows.Count; i++)
                {
                    string vcSupplier_id = importDt.Rows[i]["vcSupplier_id"].ToString();
                    string vcWorkArea = importDt.Rows[i]["vcWorkArea"].ToString();
                    int BeginDate = int.Parse(importDt.Rows[i]["dBeginDate"].ToString().Replace("-", "").Replace("/", ""));
                    int EndDate = int.Parse(importDt.Rows[i]["dEndDate"].ToString().Replace("-", "").Replace("/", ""));
                    if (BeginDate.ToString().Substring(0, 6) != EndDate.ToString().Substring(0, 6))
                    {
                        sbr1.Append("供应商代码:" + vcSupplier_id + " 工区:"+ vcWorkArea + "的纳入日-开始日期纳入日-结束日期必须同一个月<br/>");
                    }
                    if (BeginDate > EndDate)
                    {
                        sbr1.Append("供应商代码:" + vcSupplier_id + " 工区:" + vcWorkArea + "的纳入日-开始日期必须小于 纳入日-结束日期 <br/>");
                    }
                }
                if (sbr1.Length > 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = sbr1.ToString();
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fs0607_Logic.importSave(importDt, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "保存成功";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0605", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败" + ex.Message;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion



    }
}
