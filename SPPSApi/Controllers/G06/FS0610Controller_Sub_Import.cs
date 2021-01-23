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
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace SPPSApi.Controllers.G04
{
    [Route("api/FS0610_Sub_Import/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0610Controller_Sub_Import : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0610_Logic fs0610_Logic = new FS0610_Logic();
        private readonly string FunctionID = "FS0610";

        public FS0610Controller_Sub_Import(IWebHostEnvironment webHostEnvironment)
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

            DateTime dNow = DateTime.Now.AddMonths(1);
            string strYearMonth = dNow.ToString("yyyyMM");

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
                string[,] headers = new string[,] {{"PartsNo", "发注工厂", "订货频度", "CFC", "OrdLot", "N Units"
                ,"N PCS","D1","D2","D3","D4","D5","D6","D7","D8","D9","D10","D11","D12","D13","D14"
                ,"D15","D16","D17","D18","D19","D20","D21","D22","D23","D24","D25","D26","D27","D28"
                ,"D29","D30","D31","N+1 O/L","N+1 Units","N+1 PCS","N+2 O/L","N+2 Units","N+2 PCS"},
                                                {"vcPart_id", "vcFZGC", "vcMakingOrderType", "vcCarType", "iQuantityPercontainer", "iBoxes"
                ,"iPartNums","iD1","iD2","iD3","iD4","iD5","iD6","iD7","iD8","iD9","iD10","iD11","iD12","iD13","iD14"
                ,"iD15","iD16","iD17","iD18","iD19","iD20","iD21","iD22","iD23","iD24","iD25","iD26","iD27","iD28"
                ,"iD29","iD30","iD31","vcCarType2","iBoxes2","iPartNums2","vcCarType3","iBoxes3","iPartNums3"},
                                                {FieldCheck.NumCharLLL,FieldCheck.NumChar,FieldCheck.NumChar,FieldCheck.NumChar,FieldCheck.Num,FieldCheck.Num,
                FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,
                FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,
                FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num},
                                                {"0","0","0","0","0","0","0","0","0","0","0","0","0",
                "0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0",
                "0","0","0","0","0","0","0","0","0","0"},//最大长度设定,不校验最大长度用0
                                                {"0","0","0","0","0","0","0","0","0","0","0","0","0",
                "0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0",
                "0","0","0","0","0","0","0","0","0","0"}};//最小长度设定,可以为空用0

                string strMsg = "";
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
                             group r by new { r2 = r.Field<string>("vcPart_id") } into g
                             where g.Count() > 1
                             select g;
                if (result.Count() > 0)
                {
                    StringBuilder sbr = new StringBuilder();
                    sbr.Append("导入数据重复:<br/>");
                    foreach (var item in result)
                    {
                        sbr.Append("品番:" + item.Key.r2 + "<br/>");
                    }
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = sbr.ToString();
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                //验证修改后每日平准箱数是否跟总箱数一致
                StringBuilder errPart = new StringBuilder();
                for (int i = 0; i < importDt.Rows.Count; i++)
                {
                    string strPart_id = importDt.Rows[i]["vcPart_id"].ToString();
                    string strTemp = importDt.Rows[i]["iBoxes"] == System.DBNull.Value ? "" : importDt.Rows[i]["iBoxes"].ToString();
                    int iBoxes = strTemp.Trim() == "" ? 0 : Convert.ToInt32(strTemp.Trim());
                    int iCheck = 0;
                    for (int j = 1; j < 32; j++)
                    {
                        string strIDTemp = importDt.Rows[i]["iD" + j] == System.DBNull.Value ? "" : importDt.Rows[i]["iD" + j].ToString();
                        int iD = strIDTemp.Trim() == "" ? 0 : Convert.ToInt32(strIDTemp.Trim());
                        iCheck = iCheck + iD;
                    }
                    if (iBoxes != iCheck)
                        errPart.Append(strPart_id + ",");
                }
                if (errPart.Length > 0)
                {
                    StringBuilder sbr = new StringBuilder();
                    sbr.Append("以下品番修改后每日平准箱数跟总箱数不一致:<br/>");
                    sbr.Append(errPart);
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = sbr.ToString();
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                fs0610_Logic.importSave(importDt, strYearMonth, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "保存成功";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE1102", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败" + ex.Message;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}
