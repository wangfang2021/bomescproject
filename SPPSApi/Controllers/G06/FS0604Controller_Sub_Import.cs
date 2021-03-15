﻿using System;
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
    [Route("api/FS0604_Sub_Import/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0604Controller_Sub_Import : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0604_Logic fs0604_Logic = new FS0604_Logic();
        private readonly string FunctionID = "FS0604";

        public FS0604Controller_Sub_Import(IWebHostEnvironment webHostEnvironment)
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
                string[,] headers = new string[,] {{"状态","包装工场","收货方","供应商代码","品番","要望纳期","要望收容数","收容数", "箱最大收容数", "箱种", "长(mm)", "宽(mm)", "高(mm)", "空箱重量(g)", "单品净重(g)","备注"},
                                                {"vcOperWay","vcPackingPlant","vcReceiver","vcSupplier_id","vcPartNo","dExpectDeliveryDate", "vcExpectIntake","vcIntake", "vcBoxMaxIntake", "vcBoxType", "vcLength", "vcWide", "vcHeight", "vcEmptyWeight", "vcUnitNetWeight","vcMemo"},
                                                {"","","",FieldCheck.NumCharLLL,FieldCheck.NumCharLLL,FieldCheck.Date,FieldCheck.Num, FieldCheck.Num,FieldCheck.Num,"",FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,""},
                                                {"20","20","10","4","12","0","0","20","20","20","50", "20", "20", "20", "20","500"},//最大长度设定,不校验最大长度用0
                                                {"1","1","1","1","1","0","0","0","0","0","0", "0", "0", "0", "0","0"}};//最小长度设定,可以为空用0
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
                             group r by new { r2 = r.Field<string>("vcPartNo"), r3 = r.Field<string>("vcPackingPlant"), r4 = r.Field<string>("vcReceiver"), r5 = r.Field<string>("vcSupplier_id") } into g
                             where g.Count() > 1
                             select g;
                if (result.Count() > 0)
                {
                    StringBuilder sbr = new StringBuilder();
                    sbr.Append("导入数据重复:<br/>");
                    foreach (var item in result)
                    {
                        sbr.Append("品番:" + item.Key.r2 + "包装工厂: " + item.Key.r3 + "收货方: " + item.Key.r4 + "供应商代码:" + item.Key.r5 + " < br/>");
                    }
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = sbr.ToString();
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //判断新增的数据四个主键是否在数存在重复
                for (int i=0;i< importDt.Rows.Count;i++)
                {
                    if (importDt.Rows[i]["vcOperWay"].ToString().Trim()=="新增")
                    {
                        if (fs0604_Logic.isCheckImportAddData(importDt.Rows[i]["vcPackingPlant"].ToString().Trim(), importDt.Rows[i]["vcReceiver"].ToString().Trim(), importDt.Rows[i]["vcSupplier_id"].ToString().Trim(), importDt.Rows[i]["vcPartNo"].ToString().Trim()))
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "新增状态的数据,包装工厂"+ importDt.Rows[i]["vcPackingPlant"].ToString().Trim()+",收货方"+ importDt.Rows[i]["vcReceiver"].ToString().Trim()+",供应商代码"+ importDt.Rows[i]["vcSupplier_id"].ToString().Trim()+"品番"+importDt.Rows[i]["vcPartNo"].ToString().Trim()+"数据库主键重复,不能新增！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                    }
                }
                fs0604_Logic.importSave(importDt, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "保存成功";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0505", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败" + ex.Message;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion



    }
}
