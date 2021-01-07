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

namespace SPPSApi.Controllers.G03
{
    [Route("api/FS0303_Sub_Import/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0303Controller_Sub_Import : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0303_Logic FS0303_Logic = new FS0303_Logic();
        private readonly string FunctionID = "FS0303";

        public FS0303Controller_Sub_Import(IWebHostEnvironment webHostEnvironment)
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
                string[,] headers = new string[,] {{"变更事项","设变号" , "生确"    ,"区分"  ,"补给品番" ,"车型(设计)"  ,"车型(开发)"     ,"使用开始"     ,"使用结束"     ,"SD"      ,"英文品名"    ,"OE=SP","号旧"    ,"防錆"    ,"收货方"    ,"所属单位"},
                                                   {"vcChange","vcSPINo","vcSQState","vcDiff","vcPart_id","vcCarTypeDev","vcCarTypeDesign","dTimeFrom"    ,"dTimeTo"      ,"vcBJDiff","vcPartNameEn","vcOE" ,"vcHaoJiu","vcFXDiff","vcReceiver","vcOriginCompany"},
                                                   {""        ,""       ,""         ,""      ,""         ,""            ,""               ,FieldCheck.Date,FieldCheck.Date,""        ,""            ,""     ,""        ,""        ,""          ,FieldCheck.Date},
                                                   {"1"       ,"20"     ,"1"        ,"1"     ,"12"       ,"4"           ,"4"              ,"0"            ,"0"            ,"4"       ,"100"         ,"1"    ,"1"       ,"2"       ,"10"        ,"0",},//最大长度设定,不校验最大长度用0
                                                   {"1"       ,"1"      ,"1"        ,"1"     , "1"       ,"1"           , "1"             ,"1"            ,"1"            ,"1"       ,"1"           ,"1"    ,"1"       ,"1"       ,"1"         ,"1"},//最小长度设定,可以为空用0
                                                   {"1"       ,"2"      ,"3"        ,"4"     , "5"       ,"6"           , "7"             ,"8"            ,"9"            ,"10"      ,"11"          ,"12"   ,"13"      ,"14"      ,"15"        ,"16"}//前台显示列号，从0开始计算,注意有选择框的是0
                    };
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

                #region 校验重复性
                //var result = from r in importDt.AsEnumerable()
                //             group r by new { r2 = r.Field<string>("vcPart_id"), r3 = r.Field<string>("dUseBegin"), r4 = r.Field<string>("dUseEnd") } into g
                //             where g.Count() > 1
                //             select g;
                //if (result.Count() > 0)
                //{
                //    StringBuilder sbr = new StringBuilder();
                //    sbr.Append("导入数据重复:<br/>");
                //    foreach (var item in result)
                //    {
                //        sbr.Append("品番:" + item.Key.r2 + " 使用开始:" + item.Key.r3 + " 使用结束:" + item.Key.r4 + "<br/>");
                //    }
                //    apiResult.code = ComConstant.ERROR_CODE;
                //    apiResult.data = sbr.ToString();
                //    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                //}
                #endregion

                FS0303_Logic.importSave(importDt, loginInfo.UserId);
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
