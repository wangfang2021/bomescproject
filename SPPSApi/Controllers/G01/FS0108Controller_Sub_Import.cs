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
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace SPPSApi.Controllers.G01
{
    [Route("api/FS0108_Sub_Import/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0108Controller_Sub_Import : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0108_Logic fs0108_Logic = new FS0108_Logic();
        private readonly string FunctionID = "FS0108";

        public FS0108Controller_Sub_Import(IWebHostEnvironment webHostEnvironment)
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
                //string[,] headers = new string[,] {{"状态","包装工场","收货方","供应商代码","品番","要望纳期","要望收容数","收容数", "箱最大收容数", "箱种", "长(mm)", "宽(mm)", "高(mm)", "空箱重量(g)", "单品净重(g)","备注"},
                //                                {"vcOperWay","vcPackingPlant","vcReceiver","vcSupplier_id","vcPartNo","dExpectDeliveryDate", "vcExpectIntake","vcIntake", "vcBoxMaxIntake", "vcBoxType", "vcLength", "vcWide", "vcHeight", "vcEmptyWeight", "vcUnitNetWeight","vcMemo"},
                //                                {"","","",FieldCheck.NumCharLLL,FieldCheck.NumCharLLL,FieldCheck.Date,FieldCheck.Num, FieldCheck.Num,FieldCheck.Num,"",FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,""},
                //                                {"20","20","10","4","12","0","0","20","20","20","50", "20", "20", "20", "20","500"},//最大长度设定,不校验最大长度用0
                //                                {"1","1","1","1","1","0","0","0","0","0","0", "0", "0", "0", "0","0"}};//最小长度设定,可以为空用0
                string[] fields = {"vcType","iAutoId",
                    "vcValue1", "vcValue2", "vcValue3", "vcValue4", "vcValue5"
                };
                //head = new string[] { "状态", "展开时间", "要望纳期", "同步时间", "包装工场", "收货方", "品番", "品名", "车型", "使用开始时间", "OE=SP", "供应商代码", "工区", "要望收容数", "收容数", "箱最大收容数", "箱种", "长(mm)", "宽(mm)", "高(mm)", "空箱重量(g)", "单品净重(g)", "回复时间", "承认时间", "原单位织入时间", "备注" };
                //field = new string[] { "vcState", "dSendDate", "dExpectDeliveryDate", "dSynchronizationDate", "vcPackingPlant", "vcReceiver", "vcPartNo", "vcPartName", "vcCarType", "dUseStartDate", "vcOEOrSP", "vcSupplier_id", "vcWorkArea", "vcExpectIntake", "vcIntake", "vcBoxMaxIntake", "vcBoxType", "vcLength", "vcWide", "vcHeight", "vcEmptyWeight", "vcUnitNetWeight", "dReplyDate", "dAdmitDate", "dWeaveDate", "vcMemo" };


                string[,] headers = new string[,] {{"操作类型","iAutoId", "供应商编码", "工区", "开始时间", "结束时间", "发注工场" },
                                                { "vcType","iAutoId",  "vcValue1", "vcValue2", "vcValue3", "vcValue4", "vcValue5"},
                                                {"","",FieldCheck.NumChar,FieldCheck.NumChar,FieldCheck.Date,FieldCheck.Date,FieldCheck.NumCharLLL},
                                                {"20","0","4","4","10","10","10"},//最大长度设定,不校验最大长度用0
                                                {"1","0","4","1","1","1","1"}};//最小长度设定,可以为空用0


                DataTable importDt = new DataTable();
                DataTable dataTable = fs0108_Logic.createTable("fs0108");
                bool bReault = true;
                foreach (FileInfo info in theFolder.GetFiles())
                {
                    DataTable dt = fs0108_Logic.ExcelToDataTableFfs0108(info.FullName, "sheet1", headers, ref bReault,ref dataTable);
                    
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
                DataRow[] drArrayAdd = importDt.Select("vcType='新增' ");
                DataTable dtadd = drArrayAdd[0].Table.Clone(); // 复制DataRow的表结构
                foreach (DataRow dr in drArrayAdd)
                {
                    dtadd.ImportRow(dr);
                }
                DataRow[] drArrayMod = importDt.Select("vcType='修改' ");
                DataTable dtamody = drArrayMod[0].Table.Clone(); // 复制DataRow的表结构
                foreach (DataRow dr in drArrayMod)
                {
                    dtamody.ImportRow(dr);
                }
                
                //判断新增的数据四个主键是否在数存在重复
                for (int i=0;i< importDt.Rows.Count;i++)
                {
                    if (importDt.Rows[i]["vcType"].ToString().Trim()=="新增")
                    {
                        if (fs0108_Logic.isCheckImportAddData(importDt.Rows[i]["vcPackingPlant"].ToString().Trim(), importDt.Rows[i]["vcReceiver"].ToString().Trim(), importDt.Rows[i]["vcSupplier_id"].ToString().Trim(), importDt.Rows[i]["vcPartNo"].ToString().Trim()))
                        {
                            DataRow dataRow = dataTable.NewRow();
                            dataRow["vcPartNo"] = importDt.Rows[i]["vcPartNo"].ToString().Trim();
                            dataRow["vcMessage"] = "新增状态的数据,包装工场" + importDt.Rows[i]["vcPackingPlant"].ToString().Trim() + ",收货方" + importDt.Rows[i]["vcReceiver"].ToString().Trim() + ",供应商代码" + importDt.Rows[i]["vcSupplier_id"].ToString().Trim() +  "数据库主键重复,不能新增！";
                            dataTable.Rows.Add(dataRow);
                            bReault = false;
                        }
                    }
                }
                if (!bReault)
                {
                    //弹出错误dtMessage
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dataTable;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fs0108_Logic.importSave(importDt, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "保存成功";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                if (ex.Message == "“a”附近有语法错误。\r\n字符串 '   \n      \n' 后的引号不完整。")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "数据里面有单引号,报错!";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "数据里面有非法符号,报错!";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0416", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "批量维护导入失败" + ex.Message;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 下载模板数据
        [HttpPost]
        [EnableCors("any")]
        public string downloadTplApi([FromBody] dynamic data)
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
            string vcValue1 = dataForm.vcValue1 == null ? "" : dataForm.vcValue1;
            string vcValue2 = dataForm.vcValue2 == null ? "" : dataForm.vcValue2;
            string vcValue5 = dataForm.vcValue5 == null ? "" : dataForm.vcValue5;
            try
            {
                DataTable dt = fs0108_Logic.Search(vcValue1, vcValue2, vcValue5);
                dt.Columns.Add("vcType");
                string[] fields = {"vcType","iAutoId",
                    "vcValue1", "vcValue2", "vcValue3", "vcValue4", "vcValue5"
                };
                //head = new string[] { "状态", "展开时间", "要望纳期", "同步时间", "包装工场", "收货方", "品番", "品名", "车型", "使用开始时间", "OE=SP", "供应商代码", "工区", "要望收容数", "收容数", "箱最大收容数", "箱种", "长(mm)", "宽(mm)", "高(mm)", "空箱重量(g)", "单品净重(g)", "回复时间", "承认时间", "原单位织入时间", "备注" };
                //field = new string[] { "vcState", "dSendDate", "dExpectDeliveryDate", "dSynchronizationDate", "vcPackingPlant", "vcReceiver", "vcPartNo", "vcPartName", "vcCarType", "dUseStartDate", "vcOEOrSP", "vcSupplier_id", "vcWorkArea", "vcExpectIntake", "vcIntake", "vcBoxMaxIntake", "vcBoxType", "vcLength", "vcWide", "vcHeight", "vcEmptyWeight", "vcUnitNetWeight", "dReplyDate", "dAdmitDate", "dWeaveDate", "vcMemo" };
                //string msg = string.Empty;

                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0108_Template.xlsx", 1, loginInfo.UserId, FunctionID);
                if (filepath == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "导出模板文件失败";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = filepath;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0817", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "批量维护下载模板失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        
    }
}
