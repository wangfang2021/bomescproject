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

namespace SPPSApi.Controllers.G08
{
    [Route("api/FS0807_Sub_Import/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0807Controller_Sub_Import : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0807_Logic fs0807_Logic = new FS0807_Logic();
        private readonly string FunctionID = "FS0807";

        public FS0807Controller_Sub_Import(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
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

            try
            {
                string filepath = fs0807_Logic.generateExcelWithXlt_Module(_webHostEnvironment.ContentRootPath, "FS0807.xlsx", loginInfo.UserId, FunctionID);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M08UE0706", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "下载模板失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

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
                    {"工区","品番","开始时间","结束时间","车种","收货方","供应商","包装场","受入",
                     "背番号","收容数","品名(英文)","品名(中文)","自工程","通过工程","前工程",
                     "前工程通过时间","自工程发货时间","照片","备注1","备注2"},
                    {"vcGQ", "vcPart_id","dTimeFrom","dTimeTo","vcCarType","vcSHF","vcSupplier_id","vcBZPlant","vcSR",
                     "vcKanBanNo","iContainerQuantity","vcPartNameEn","vcPartNameCn","vcInProcess","vcTGProcess","vcPreProcess",
                     "vcPreProcessPassTime","vcInProcessSendTime","vcPhotoPath","vcRemark1","vcRemark2"},
                    {"",FieldCheck.NumChar,FieldCheck.Date,FieldCheck.Date,"","","","","",
                     "",FieldCheck.Num,"","","","","",
                     "","","","",""},
                    {"10","12","0","0","10","25","25","25","2",
                     "5","0","20","20","1","10","10",
                     "4","4","100","25","25"},//最大长度设定,不校验最大长度用0
                    {"0","1","1","1","0","1","0","0","0",
                     "0","1","0","0","0","0","0",
                     "0","0","0","0","0"}};//最小长度设定,可以为空用0
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
                             group r by new { r1 = r.Field<string>("vcPart_id"), r2 = r.Field<string>("dTimeFrom"), r3 = r.Field<string>("vcSHF") } into g
                             where g.Count() > 1
                             select g;
                if (result.Count() > 0)
                {
                    StringBuilder sbr = new StringBuilder();
                    sbr.Append("导入数据重复:<br/>");
                    foreach (var item in result)
                    {
                        sbr.Append("品番:" + item.Key.r1 + " 开始时间:" + item.Key.r2 + " 收货方:" + item.Key.r3 + "<br/>");
                    }
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = sbr.ToString();
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                DataTable dt_gq = fs0807_Logic.getName("gq");//工区
                DataTable dt_shf = fs0807_Logic.getName("shf");//收货方
                DataTable dt_supplier = fs0807_Logic.getName("supplier");//供应商
                DataTable dt_bzplant = fs0807_Logic.getName("bzplant");//包装场
                for (int i = 0; i < importDt.Rows.Count; i++)
                {
                    //工区内容校验
                    DataRow[] drs = dt_gq.Select("vcValue='" + importDt.Rows[i]["vcGQ"].ToString() + "' ");
                    if (importDt.Rows[i]["vcGQ"].ToString()!="" && drs.Length == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "第" + (i + 2) + "行工区：" + importDt.Rows[i]["vcGQ"].ToString() + " 在系统中不存在。";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    //else
                    //    importDt.Rows[i]["vcGQ"] = drs[0]["vcValue"].ToString();
                    //收货方内容校验
                    drs = dt_shf.Select("vcValue='" + importDt.Rows[i]["vcSHF"].ToString() + "' ");
                    if (drs.Length == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "第" + (i + 2) + "行收货方：" + importDt.Rows[i]["vcSHF"].ToString() + " 在系统中不存在。";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    //else
                    //    importDt.Rows[i]["vcSHF"] = drs[0]["vcValue"].ToString();
                    //供应商内容校验
                    drs = dt_supplier.Select("vcValue='" + importDt.Rows[i]["vcSupplier_id"].ToString() + "' ");
                    if (importDt.Rows[i]["vcSupplier_id"].ToString()!="" && drs.Length == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "第" + (i + 2) + "行供应商：" + importDt.Rows[i]["vcSupplier_id"].ToString() + " 在系统中不存在。";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    //else
                    //    importDt.Rows[i]["vcSupplier_id"] = drs[0]["vcValue"].ToString();
                    //包装场内容校验
                    drs = dt_bzplant.Select("vcValue='" + importDt.Rows[i]["vcBZPlant"].ToString() + "' ");
                    if (importDt.Rows[i]["vcBZPlant"].ToString()!="" && drs.Length == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "第" + (i + 2) + "行包装场：" + importDt.Rows[i]["vcBZPlant"].ToString() + " 在系统中不存在。";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    //else
                    //    importDt.Rows[i]["vcBZPlant"] = drs[0]["vcValue"].ToString();
                    //判断时间区间先后关系
                    string strValue_start = importDt.Rows[i]["dTimeFrom"].ToString();
                    string strValue_end = importDt.Rows[i]["dTimeTo"].ToString();
                    if (strValue_start != "" && strValue_end != "")
                    {
                        DateTime dStart = DateTime.Parse(strValue_start);
                        DateTime dEnd = DateTime.Parse(strValue_end);
                        if (dStart > dEnd)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "第" + (i + 2) + "行时间区间必须满足起<止。";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                    }
                }
                
                string strErrorName = "";
                fs0807_Logic.importSave(importDt, loginInfo.UserId,ref strErrorName);
                if(strErrorName!="")
                {//品番日期区间有重复
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "以下品番区间有重复："+strErrorName;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "保存成功";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M08UE0707", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导入失败" + ex.Message;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

    }


}
