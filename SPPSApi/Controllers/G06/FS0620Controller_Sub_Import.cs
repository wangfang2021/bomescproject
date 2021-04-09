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
    [Route("api/FS0620_Sub_Import/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0620Controller_Sub_Import : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0620_Logic fs0620_Logic = new FS0620_Logic();
        private readonly string FunctionID = "FS0620";

        public FS0620Controller_Sub_Import(IWebHostEnvironment webHostEnvironment)
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
                string[,] headers = new string[,] {{"年计类型","收货方","包装工场", "对象年份", "品番", "发注工场", "内外", "供应商代码", "工区", "车型", "收容数", "1月", "2月", "3月", "4月", "5月", "6月", "7月", "8月", "9月", "10月", "11月", "12月", "N+1年预测", "N+2年预测"},
                                                { "vcType","vcReceiver","vcPackPlant", "vcTargetYear", "vcPartNo", "vcInjectionFactory", "vcInsideOutsideType", "vcSupplier_id", "vcWorkArea", "vcCarType", "vcAcceptNum", "vcJanuary", "vcFebruary", "vcMarch", "vcApril", "vcMay", "vcJune", "vcJuly", "vcAugust", "vcSeptember", "vcOctober", "vcNovember", "vcDecember", "vcNextOneYear", "vcNextTwoYear"},
                                                {"","","",FieldCheck.Num,FieldCheck.NumCharLLL,"","",FieldCheck.NumCharLLL,FieldCheck.NumCharLLL,FieldCheck.NumCharLLL,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num},
                                                {"100","50","50","4","12","100","100","4","1","50","30","30","30","30","30","30","30","30","30","30","30","30","30","30","30"},//最大长度设定,不校验最大长度用0
                                                {"1","1","1","4","12","1","1","1","1","4","1","0","0","0","0","0","0","0","0","0","0","0","0","0","0"}};//最小长度设定,可以为空用0
                DataTable importDt = new DataTable();
                DataTable dataTable = fs0620_Logic.createTable("fs0620");
                bool bReault = true;
                foreach (FileInfo info in theFolder.GetFiles())
                {
                    DataTable dt = fs0620_Logic.ExcelToDataTableFfs0620(info.FullName, "sheet1", headers, ref bReault, ref dataTable);
                    //DataTable dt = ComFunction.ExcelToDataTable(info.FullName, "sheet1", headers, ref strMsg);
                    //if (strMsg != "")
                    //{
                    //    ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                    //    apiResult.code = ComConstant.ERROR_CODE;
                    //    apiResult.data = "导入终止，文件" + info.Name + ":" + strMsg;
                    //    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    //}
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

               
                DataTable dtReceiver = ComFunction.getTCode("C018");
                DataTable dtpackPlant = ComFunction.getTCode("C017");
                DataTable dtfazhuPlant = fs0620_Logic.getTCode("C026");
                ArrayList list = new ArrayList();
                ArrayList listError = new ArrayList();
                ArrayList packlistError = new ArrayList();
                ArrayList fazhulistError = new ArrayList();
                for (int i=0;i< dtReceiver.Rows.Count;i++)
                {
                    list.Add(dtReceiver.Rows[i]["vcValue"]);
                }
                ArrayList packArrayLis = new ArrayList();
                for (int i = 0; i < dtpackPlant.Rows.Count; i++)
                {
                    packArrayLis.Add(dtpackPlant.Rows[i]["vcValue"]);
                }
                ArrayList fazhuArrayLis = new ArrayList();
                for (int i = 0; i < dtfazhuPlant.Rows.Count; i++)
                {
                    fazhuArrayLis.Add(dtfazhuPlant.Rows[i]["vcName"]);
                }

                for (int i = 0;i<importDt.Rows.Count;i++)
                {
                   
                    if (!list.Contains(importDt.Rows[i]["vcReceiver"].ToString().Trim()))
                    {
                        if (!listError.Contains(importDt.Rows[i]["vcReceiver"].ToString().Trim()))
                        {
                            listError.Add(importDt.Rows[i]["vcReceiver"].ToString().Trim());
                        }
                    }
                    if (!packArrayLis.Contains(importDt.Rows[i]["vcPackPlant"].ToString().Trim()))
                    {
                        if (!packlistError.Contains(importDt.Rows[i]["vcPackPlant"].ToString().Trim()))
                        {
                            packlistError.Add(importDt.Rows[i]["vcPackPlant"].ToString().Trim());
                        }
                    }
                    if (!fazhuArrayLis.Contains(importDt.Rows[i]["vcInjectionFactory"].ToString().Trim()))
                    {
                        if (!fazhulistError.Contains(importDt.Rows[i]["vcInjectionFactory"].ToString().Trim()))
                        {
                            fazhulistError.Add(importDt.Rows[i]["vcInjectionFactory"].ToString().Trim());
                        }
                    }
                }
                StringBuilder sbr = new StringBuilder();
                if (listError.Count>0)
                {
                    //sbr.Append("收货方:");
                    for (int i=0;i<listError.Count;i++)
                    {
                        //sbr.Append(listError[i]+"、");
                        DataRow dataRow = dataTable.NewRow();
                        dataRow["vcPartNo"] = "";
                        dataRow["vcMessage"] = "收货方" + listError[i]+ "不存在于数据字典收货方里面,请先追加再导入数据!";
                        dataTable.Rows.Add(dataRow);
                        bReault = false;
                    }
                    //sbr.Remove(sbr.Length - 1, 1);
                    //sbr.Append("不存在于数据字典收货方里面,请先追加再导入数据!<br/>");
                   
                }
                if (packlistError.Count > 0)
                {
                    //sbr.Append("包装工场:");
                    for (int i = 0; i < packlistError.Count; i++)
                    {
                        //sbr.Append(packlistError[i] + "、");
                        DataRow dataRow = dataTable.NewRow();
                        dataRow["vcPartNo"] = "";
                        dataRow["vcMessage"] = "包装工场" + packlistError[i] + "不存在于常量里面,请先追加再导入数据!";
                        dataTable.Rows.Add(dataRow);
                        bReault = false;
                    }
                    //sbr.Remove(sbr.Length - 1, 1);
                    //sbr.Append("不存在于常量里面,请先追加再导入数据!<br/>");
                }
                if (fazhulistError.Count > 0)
                {
                    //sbr.Append("发注工场:");
                    for (int i = 0; i < fazhulistError.Count; i++)
                    {
                        //sbr.Append(fazhulistError[i] + "、");
                        DataRow dataRow = dataTable.NewRow();
                        dataRow["vcPartNo"] = "";
                        dataRow["vcMessage"] = "发注工场" + fazhulistError[i] + "不存在于常量里面,请先追加再导入数据!";
                        dataTable.Rows.Add(dataRow);
                        bReault = false;
                    }
                    //sbr.Remove(sbr.Length - 1, 1);
                    //sbr.Append("不存在于常量里面,请先追加再导入数据!<br/>");
                }
                if (!bReault)
                {
                    //弹出错误dtMessage
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dataTable;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //if (sbr.Length>0)
                //{
                //    apiResult.code = ComConstant.ERROR_CODE;
                //    apiResult.data = sbr.ToString();
                //    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                //}
                fs0620_Logic.importSave(importDt, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "保存成功";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2011", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败" + ex.Message;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion



    }
}
