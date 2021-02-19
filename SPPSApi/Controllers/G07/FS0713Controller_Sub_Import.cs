using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
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

namespace SPPSApi.Controllers.G07
{
    [Route("api/FS0713_Sub_Import/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0713Controller_Sub_Import : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0713_Logic fs0713_Logic = new FS0713_Logic();
        private readonly string FunctionID = "FS0713";

        public FS0713Controller_Sub_Import(IWebHostEnvironment webHostEnvironment)
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
                string[,] headers = new string[,] {{"包装场","包装材品番","GPS品番","供应商","平均消耗","建议安全在库","安全在库","看板循环"},
                                                {"vcPackSpot","vcPackNo","vcPackGPSNo","vcSupplierCode","vcAvgUse","vcadviceZK","vcSaveZK","vcKBcycle"},
                                                {FieldCheck.NumCharLLL,FieldCheck.NumCharLLL,FieldCheck.NumCharLLL,"","","","",FieldCheck.Num},
                                                {"50","50","50","0","0","0","0","0" },
                                                {"1","1","1","0","0","0","0","0" }
                                               };//最小长度设定,可以为空用0
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

                #region 导入限制
                //查找包装厂
                DataTable dtPS = fs0713_Logic.SearchPackSpot();
                for (int i = 0; i < importDt.Rows.Count; i++)
                {
                    if (dtPS.Select("vcValue='" + importDt.Rows[i]["vcPackSpot"].ToString() + "'").Length == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "导入失败:第" + i + "行,包装场维护错误！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    Regex regex = new System.Text.RegularExpressions.Regex("^(-?[0-9]*[.]*[0-9]{0,3})$");
                    bool b = regex.IsMatch(importDt.Rows[i]["vcKBcycle"].ToString());
                    if (!b)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "导入失败:第" + i + "行,发注收容数维护错误！"; 
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                #endregion

                var result = from r in importDt.AsEnumerable()
                             group r by new { r2 = r.Field<string>("vcPackSpot"), r3 = r.Field<string>("vcPackNo"), r4 = r.Field<string>("vcPackGPSNo") } into g
                             where g.Count() > 1
                             select g;
                if (result.Count() > 0)
                {
                    StringBuilder sbr = new StringBuilder();
                    sbr.Append("导入数据重复:<br/>");
                    foreach (var item in result)
                    {
                        sbr.Append("品番:" + item.Key.r2 + " 使用开始:" + item.Key.r3 + " 使用结束:" + item.Key.r4 + "<br/>");
                    }
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = sbr.ToString();
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }




                fs0713_Logic.importSave(importDt, loginInfo.UserId);
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
