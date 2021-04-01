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
                //string[,] headers = new string[,] {{"状态","包装工场","收货方","供应商代码","品番","要望纳期","要望收容数","收容数", "箱最大收容数", "箱种", "长(mm)", "宽(mm)", "高(mm)", "空箱重量(g)", "单品净重(g)","备注"},
                //                                {"vcOperWay","vcPackingPlant","vcReceiver","vcSupplier_id","vcPartNo","dExpectDeliveryDate", "vcExpectIntake","vcIntake", "vcBoxMaxIntake", "vcBoxType", "vcLength", "vcWide", "vcHeight", "vcEmptyWeight", "vcUnitNetWeight","vcMemo"},
                //                                {"","","",FieldCheck.NumCharLLL,FieldCheck.NumCharLLL,FieldCheck.Date,FieldCheck.Num, FieldCheck.Num,FieldCheck.Num,"",FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,""},
                //                                {"20","20","10","4","12","0","0","20","20","20","50", "20", "20", "20", "20","500"},//最大长度设定,不校验最大长度用0
                //                                {"1","1","1","1","1","0","0","0","0","0","0", "0", "0", "0", "0","0"}};//最小长度设定,可以为空用0
                string[] fields = {"vcType",
                    "vcState", "dSendDate", "dExpectDeliveryDate", "dSynchronizationDate", "vcPackingPlant", "vcReceiver", "vcPartNo", "vcPartName", "vcCarType", "dUseStartDate", "vcOEOrSP", "vcSupplier_id", "vcWorkArea", "vcExpectIntake", "vcIntake", "vcBoxMaxIntake", "vcBoxType", "vcLength", "vcWide", "vcHeight", "vcEmptyWeight", "vcUnitNetWeight", "dReplyDate", "dAdmitDate", "dWeaveDate", "vcMemo"
                };
                //head = new string[] { "状态", "展开时间", "要望纳期", "同步时间", "包装工场", "收货方", "品番", "品名", "车型", "使用开始时间", "OE=SP", "供应商代码", "工区", "要望收容数", "收容数", "箱最大收容数", "箱种", "长(mm)", "宽(mm)", "高(mm)", "空箱重量(g)", "单品净重(g)", "回复时间", "承认时间", "原单位织入时间", "备注" };
                //field = new string[] { "vcState", "dSendDate", "dExpectDeliveryDate", "dSynchronizationDate", "vcPackingPlant", "vcReceiver", "vcPartNo", "vcPartName", "vcCarType", "dUseStartDate", "vcOEOrSP", "vcSupplier_id", "vcWorkArea", "vcExpectIntake", "vcIntake", "vcBoxMaxIntake", "vcBoxType", "vcLength", "vcWide", "vcHeight", "vcEmptyWeight", "vcUnitNetWeight", "dReplyDate", "dAdmitDate", "dWeaveDate", "vcMemo" };


                string[,] headers = new string[,] {{"操作类型","状态", "展开时间", "要望纳期", "同步时间", "包装工场", "收货方", "品番", "品名", "车型", "使用开始时间", "OE=SP", "供应商代码", "工区", "要望收容数", "收容数", "箱最大收容数", "箱种", "长(mm)", "宽(mm)", "高(mm)", "空箱重量(g)", "单品净重(g)", "回复时间", "承认时间", "原单位织入时间", "备注" },
                                                { "vcType","vcState", "dSendDate", "dExpectDeliveryDate", "dSynchronizationDate", "vcPackingPlant", "vcReceiver", "vcPartNo", "vcPartName", "vcCarType", "dUseStartDate", "vcOEOrSP", "vcSupplier_id", "vcWorkArea", "vcExpectIntake", "vcIntake", "vcBoxMaxIntake", "vcBoxType", "vcLength", "vcWide", "vcHeight", "vcEmptyWeight", "vcUnitNetWeight", "dReplyDate", "dAdmitDate", "dWeaveDate", "vcMemo"},
                                                {"","",FieldCheck.Date,FieldCheck.Date,FieldCheck.Date,"","",FieldCheck.NumCharLLL,"","",FieldCheck.Date,"",FieldCheck.NumCharLLL,"",FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.NumCharLLL,FieldCheck.Num, FieldCheck.Num,FieldCheck.Num,"","",FieldCheck.Date,FieldCheck.Date,FieldCheck.Date,""},
                                                {"20","0","0","0","0","20","10","12","0","50","0","0","4","50","20","20","20","50","20","20","20","20","20","0", "0", "0","500"},//最大长度设定,不校验最大长度用0
                                                {"1","0","0","0","0","1","1","12","0","0","0","0","4","0","0","0", "0", "0", "0", "0","0", "0", "0", "0","0", "0","0"}};//最小长度设定,可以为空用0


                DataTable importDt = new DataTable();
                DataTable dataTable = fs0604_Logic.createTable("fs0604");
                bool bReault = true;
                foreach (FileInfo info in theFolder.GetFiles())
                {
                    DataTable dt = fs0604_Logic.ExcelToDataTableFfs0604(info.FullName, "sheet1", headers, ref bReault,ref dataTable);
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
                
                var result = from r in importDt.AsEnumerable()
                             group r by new { r2 = r.Field<string>("vcPartNo"), r3 = r.Field<string>("vcPackingPlant"), r4 = r.Field<string>("vcReceiver"), r5 = r.Field<string>("vcSupplier_id") } into g
                             where g.Count() > 1
                             select g;
                if (result.Count() > 0)
                {
                    //StringBuilder sbr = new StringBuilder();
                    //sbr.Append("导入数据重复:<br/>");
                    foreach (var item in result)
                    {
                        DataRow dataRow = dataTable.NewRow();
                        dataRow["vcPartNo"] = item.Key.r2;
                        dataRow["vcMessage"] = "包装工场: " + item.Key.r3 + "收货方: " + item.Key.r4 + "供应商代码:" + item.Key.r5 + "导入数据重复";
                        dataTable.Rows.Add(dataRow);
                        bReault = false;
                        //sbr.Append("品番:" + item.Key.r2 + "包装工厂: " + item.Key.r3 + "收货方: " + item.Key.r4 + "供应商代码:" + item.Key.r5 + " < br/>");
                    }
                }
                //判断新增的数据四个主键是否在数存在重复
                for (int i=0;i< importDt.Rows.Count;i++)
                {
                    if (importDt.Rows[i]["vcType"].ToString().Trim()=="新增")
                    {
                        if (fs0604_Logic.isCheckImportAddData(importDt.Rows[i]["vcPackingPlant"].ToString().Trim(), importDt.Rows[i]["vcReceiver"].ToString().Trim(), importDt.Rows[i]["vcSupplier_id"].ToString().Trim(), importDt.Rows[i]["vcPartNo"].ToString().Trim()))
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
            string dSynchronizationDateFrom = dataForm.dSynchronizationDateFrom == null ? "" : dataForm.dSynchronizationDateFrom;
            string dSynchronizationDateTo = dataForm.dSynchronizationDateTo == null ? "" : dataForm.dSynchronizationDateTo;
            string dSynchronizationDate = dataForm.dSynchronizationDate == null ? "" : dataForm.dSynchronizationDate;
            string vcState = dataForm.vcState == null ? "" : dataForm.vcState;
            string vcPartNo = dataForm.vcPartNo == null ? "" : dataForm.vcPartNo;
            string vcSupplier_id = dataForm.vcSupplier_id == null ? "" : dataForm.vcSupplier_id;
            string vcWorkArea = dataForm.vcWorkArea == null ? "" : dataForm.vcWorkArea;
            string vcCarType = dataForm.vcCarType == null ? "" : dataForm.vcCarType;
            string dExpectDeliveryDate = dataForm.dExpectDeliveryDate == null ? "" : dataForm.dExpectDeliveryDate;
            string dSendDate = dataForm.dSendDate == null ? "" : dataForm.dSendDate;
            string vcOEOrSP = dataForm.vcOEOrSP == null ? "" : dataForm.vcOEOrSP;
            string vcBoxType = dataForm.vcBoxType == null ? "" : dataForm.vcBoxType;

            try
            {
                DataTable dt = fs0604_Logic.Search(dSynchronizationDateFrom, dSynchronizationDateTo, dSynchronizationDate, vcState, vcPartNo, vcSupplier_id, vcWorkArea, vcCarType, dExpectDeliveryDate, vcOEOrSP, vcBoxType, dSendDate);
                dt.Columns.Add("vcType");
                string[] fields = {"vcType",
                    "vcState", "dSendDate", "dExpectDeliveryDate", "dSynchronizationDate", "vcPackingPlant", "vcReceiver", "vcPartNo", "vcPartName", "vcCarType", "dUseStartDate", "vcOEOrSP", "vcSupplier_id", "vcWorkArea", "vcExpectIntake", "vcIntake", "vcBoxMaxIntake", "vcBoxType", "vcLength", "vcWide", "vcHeight", "vcEmptyWeight", "vcUnitNetWeight", "dReplyDate", "dAdmitDate", "dWeaveDate", "vcMemo"
                };
                //head = new string[] { "状态", "展开时间", "要望纳期", "同步时间", "包装工场", "收货方", "品番", "品名", "车型", "使用开始时间", "OE=SP", "供应商代码", "工区", "要望收容数", "收容数", "箱最大收容数", "箱种", "长(mm)", "宽(mm)", "高(mm)", "空箱重量(g)", "单品净重(g)", "回复时间", "承认时间", "原单位织入时间", "备注" };
                //field = new string[] { "vcState", "dSendDate", "dExpectDeliveryDate", "dSynchronizationDate", "vcPackingPlant", "vcReceiver", "vcPartNo", "vcPartName", "vcCarType", "dUseStartDate", "vcOEOrSP", "vcSupplier_id", "vcWorkArea", "vcExpectIntake", "vcIntake", "vcBoxMaxIntake", "vcBoxType", "vcLength", "vcWide", "vcHeight", "vcEmptyWeight", "vcUnitNetWeight", "dReplyDate", "dAdmitDate", "dWeaveDate", "vcMemo" };
                //string msg = string.Empty;

                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0604_Template.xlsx", 1, loginInfo.UserId, FunctionID);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0412", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        
    }
}
