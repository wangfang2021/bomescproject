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
    [Route("api/FS0801_Sub/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0801Controller_Sub : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0801_Logic fs0801_Logic = new FS0801_Logic();
        private readonly string FunctionID = "FS0801";

        public FS0801Controller_Sub(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }


        #region 导入之后点保存
        [HttpPost]
        [EnableCors("any")]
        public string importApi([FromBody]dynamic data)
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
                string[,] headers = new string[,] {{"品番","使用开始","使用结束","收货方","供应商","包装厂","包装场","小品目","包装区分","包装单位","入荷区分"},
                                                {"vcPart_id", "dTimeFrom","dTimeTo","vcReceiver","vcSupplierId","vcPackingPlant","vcBZPlant","vcSmallPM","vcBZQF","vcBZUnit","vcRHQF"},
                                                {FieldCheck.NumChar,FieldCheck.Date,FieldCheck.Date,FieldCheck.NumChar,FieldCheck.NumChar,FieldCheck.NumChar,"","","","",""},//数据类型校验
                                                {"12","0","0","25","25","25","25","25","25","25","25"},//最大长度设定,不校验最大长度用0
                                                {"1","0","0","1","1","1","0","0","0","0","0"}};//最小长度设定,可以为空用0
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
                    string vcBZPlant = importDt.Rows[i]["vcBZPlant"].ToString();
                    if (vcBZPlant!="" && Array.IndexOf(new string[] { "H1", "H2" }, vcBZPlant) == -1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "[包装场]列只能填写H1/H2";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    string vcBZQF = importDt.Rows[i]["vcBZQF"].ToString();
                    if (vcBZQF != "" && Array.IndexOf(new string[] { "已包装", "未包装" }, vcBZQF) == -1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "[包装区分]列只能填写已包装/未包装";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    string vcRHQF = importDt.Rows[i]["vcRHQF"].ToString();
                    if (vcRHQF != "" && Array.IndexOf(new string[] { "可入荷", "不可入荷" }, vcRHQF) == -1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "[入荷区分]列只能填写可入荷/不可入荷";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                #endregion

                var result = from r in importDt.AsEnumerable()
                             group r by new { r1 = r.Field<string>("vcPart_id"), r2 = r.Field<string>("vcPackingPlant"), r3 = r.Field<string>("vcReceiver"), r4 = r.Field<string>("vcSupplierId") } into g
                             where g.Count() > 1
                             select g;
                if (result.Count() > 0)
                {
                    StringBuilder sbr = new StringBuilder();
                    sbr.Append("导入数据重复:<br/>");
                    foreach (var item in result)
                    {
                        sbr.Append("品番:" + item.Key.r1 + " 包装厂:" + item.Key.r2 + " 收货方:" + item.Key.r3 + "供应商: " + item.Key.r4 + "<br/>");
                    }
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = sbr.ToString();
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //校验列表项



                fs0801_Logic.import(importDt, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "保存成功";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M08UE0106", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导入失败 " + ex.Message;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion



    }
}
