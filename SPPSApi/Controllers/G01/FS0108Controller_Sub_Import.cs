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
                    "vcValue1", "vcValue2", "vcValue5", "vcValue3", "vcValue4"
                };
                //head = new string[] { "状态", "展开时间", "要望纳期", "同步时间", "包装工场", "收货方", "品番", "品名", "车型", "使用开始时间", "OE=SP", "供应商代码", "工区", "要望收容数", "收容数", "箱最大收容数", "箱种", "长(mm)", "宽(mm)", "高(mm)", "空箱重量(g)", "单品净重(g)", "回复时间", "承认时间", "原单位织入时间", "备注" };
                //field = new string[] { "vcState", "dSendDate", "dExpectDeliveryDate", "dSynchronizationDate", "vcPackingPlant", "vcReceiver", "vcPartNo", "vcPartName", "vcCarType", "dUseStartDate", "vcOEOrSP", "vcSupplier_id", "vcWorkArea", "vcExpectIntake", "vcIntake", "vcBoxMaxIntake", "vcBoxType", "vcLength", "vcWide", "vcHeight", "vcEmptyWeight", "vcUnitNetWeight", "dReplyDate", "dAdmitDate", "dWeaveDate", "vcMemo" };


                string[,] headers = new string[,] {{"操作类型","iAutoId", "供应商编码", "工区","发注工场", "开始时间", "结束时间" },
                                                {"vcType","iAutoId",  "vcValue1", "vcValue2",  "vcValue5","vcValue3", "vcValue4"},
                                                {"","",FieldCheck.NumChar,FieldCheck.NumChar,"",FieldCheck.Date,FieldCheck.Date},
                                                {"20","0","4","4","500","500","500"},//最大长度设定,不校验最大长度用0
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
                DataTable dtadd = new DataTable();
                if (drArrayAdd.Length>0)
                {
                    dtadd = drArrayAdd[0].Table.Clone(); // 复制DataRow的表结构
                    foreach (DataRow dr in drArrayAdd)
                    {
                        dtadd.ImportRow(dr);
                    }
                }
                DataTable dtamody = new DataTable();
                DataRow[] drArrayMod = importDt.Select("vcType='修改' ");
                if (drArrayMod.Length > 0)
                {
                    dtamody = drArrayMod[0].Table.Clone(); // 复制DataRow的表结构
                    foreach (DataRow dr in drArrayMod)
                    {
                        dtamody.ImportRow(dr);
                    }
                }
                for (int i = 0; i < dtadd.Rows.Count; i++)
                {
                    string vcSupplier = dtadd.Rows[i]["vcValue1"].ToString();
                    string vcWorkArea = dtadd.Rows[i]["vcValue2"].ToString();
                    string vcFzgc = dtadd.Rows[i]["vcValue5"].ToString();
                    string vcStart = Convert.ToDateTime(dtadd.Rows[i]["vcValue3"].ToString()).ToString("yyyy/MM/dd").Replace("/", "-").ToString();
                    string vcEnd = Convert.ToDateTime(dtadd.Rows[i]["vcValue4"].ToString()).ToString("yyyy/MM/dd").Replace("/", "-").ToString();
                    for (int j = i + 1; j < dtadd.Rows.Count; j++)
                    {
                        string vcSupplier1 = dtadd.Rows[j]["vcValue1"].ToString();
                        string vcWorkArea1 = dtadd.Rows[j]["vcValue2"].ToString();
                        string vcFzgc1 = dtadd.Rows[j]["vcValue5"].ToString();
                        string vcStart1 = Convert.ToDateTime(dtadd.Rows[j]["vcValue3"].ToString()).ToString("yyyy/MM/dd").Replace("/", "-").ToString();
                        string vcEnd1 = Convert.ToDateTime(dtadd.Rows[j]["vcValue4"].ToString()).ToString("yyyy/MM/dd").Replace("/", "-").ToString();
                        if (vcSupplier == vcSupplier1 && vcWorkArea == vcWorkArea1)
                        {
                            if (Convert.ToInt32(vcStart.Replace("/", "").Replace("-", "")) > Convert.ToInt32(vcEnd1.Replace("/", "").Replace("-", "")) || Convert.ToInt32(vcEnd.Replace("/", "").Replace("-", "")) < Convert.ToInt32(vcStart1.Replace("/", "").Replace("-", "")))
                            {
                            }
                            else
                            {
                                DataRow dataRow = dataTable.NewRow();
                                dataRow["vcSupplier"] = vcSupplier;
                                dataRow["vcWorkArea"] = vcWorkArea;
                                dataRow["vcFzgc"] = vcFzgc;
                                dataRow["vcMessage"] = "新增的数据时间区间出现重叠";
                                dataTable.Rows.Add(dataRow);
                                bReault = false;
                            }
                        }
                    }
                    for (int j = 0; j < dtamody.Rows.Count; j++)
                    {
                        string vcSupplier1 = dtamody.Rows[j]["vcValue1"].ToString();
                        string vcWorkArea1 = dtamody.Rows[j]["vcValue2"].ToString();
                        string vcFzgc1 = dtamody.Rows[j]["vcValue5"].ToString();
                        string vcStart1 = Convert.ToDateTime(dtamody.Rows[j]["vcValue3"].ToString()).ToString("yyyy/MM/dd").Replace("/", "-").ToString();
                        string vcEnd1 = Convert.ToDateTime(dtamody.Rows[j]["vcValue4"].ToString()).ToString("yyyy/MM/dd").Replace("/", "-").ToString();
                        if (vcSupplier == vcSupplier1 && vcWorkArea == vcWorkArea1)
                        {
                            if (Convert.ToInt32(vcStart.Replace("/", "").Replace("-", "")) > Convert.ToInt32(vcEnd1.Replace("/", "").Replace("-", "")) || Convert.ToInt32(vcEnd.Replace("/", "").Replace("-", "")) < Convert.ToInt32(vcStart1.Replace("/", "").Replace("-", "")))
                            {
                            }
                            else
                            {
                                DataRow dataRow = dataTable.NewRow();
                                dataRow["vcSupplier"] = vcSupplier;
                                dataRow["vcWorkArea"] = vcWorkArea;
                                dataRow["vcFzgc"] = vcFzgc;
                                dataRow["vcMessage"] = "新增的数据与修改的数据时间区间出现重叠";
                                dataTable.Rows.Add(dataRow);
                                bReault = false;
                            }
                        }
                    }
                }
                //修改数据之间是否出现重叠校验
                for (int i = 0; i < dtamody.Rows.Count; i++)
                {
                    string vcSupplier = dtamody.Rows[i]["vcValue1"].ToString();
                    string vcWorkArea = dtamody.Rows[i]["vcValue2"].ToString();
                    string vcFzgc = dtamody.Rows[i]["vcValue5"].ToString();
                    string vcStart = Convert.ToDateTime(dtamody.Rows[i]["vcValue3"].ToString()).ToString("yyyy/MM/dd").Replace("/", "-").ToString();
                    string vcEnd = Convert.ToDateTime(dtamody.Rows[i]["vcValue4"].ToString()).ToString("yyyy/MM/dd").Replace("/", "-").ToString();
                    for (int j = i + 1; j < dtamody.Rows.Count; j++)
                    {
                        string vcSupplier1 = dtamody.Rows[j]["vcValue1"].ToString();
                        string vcWorkArea1 = dtamody.Rows[j]["vcValue2"].ToString();
                        string vcFzgc1 = dtamody.Rows[j]["vcValue5"].ToString();
                        string vcStart1 = Convert.ToDateTime(dtamody.Rows[j]["vcValue3"].ToString()).ToString("yyyy/MM/dd").Replace("/", "-").ToString();
                        string vcEnd1 = Convert.ToDateTime(dtamody.Rows[j]["vcValue4"].ToString()).ToString("yyyy/MM/dd").Replace("/", "-").ToString();
                        if (vcSupplier == vcSupplier1 && vcWorkArea == vcWorkArea1)
                        {
                            if (Convert.ToInt32(vcStart.Replace("/", "").Replace("-", "")) > Convert.ToInt32(vcEnd1.Replace("/", "").Replace("-", "")) || Convert.ToInt32(vcEnd.Replace("/", "").Replace("-", "")) < Convert.ToInt32(vcStart1.Replace("/", "").Replace("-", "")))
                            {
                            }
                            else
                            {
                                DataRow dataRow = dataTable.NewRow();
                                dataRow["vcSupplier"] = vcSupplier;
                                dataRow["vcWorkArea"] = vcWorkArea;
                                dataRow["vcFzgc"] = vcFzgc;
                                dataRow["vcMessage"] = "修改的数据时间区间出现重叠";
                                dataTable.Rows.Add(dataRow);
                                bReault = false;
                            }
                        }
                    }
                }
                //验证新增与修改跟数据库里面是否重叠
                string strInAutoIds = string.Empty;
                for (int i = 0; i < importDt.Rows.Count; i++)
                {
                    if ( importDt.Rows[i]["vcType"].ToString() == "修改")
                    {
                        if (importDt.Rows[i]["iAutoId"].ToString().Length == 0)
                        {
                            string vcSupplier = importDt.Rows[i]["vcValue1"].ToString();
                            string vcWorkArea = importDt.Rows[i]["vcValue2"].ToString();
                            DataRow dataRow = dataTable.NewRow();
                            dataRow["vcSupplier"] = vcSupplier;
                            dataRow["vcWorkArea"] = vcWorkArea;
                            dataRow["vcFzgc"] = "";
                            dataRow["vcMessage"] = "修改状态的数据iAutoId不能为空";
                            dataTable.Rows.Add(dataRow);
                            bReault = false;
                        }
                        else
                        {
                            int iAutoId = Convert.ToInt32(importDt.Rows[i]["iAutoId"]);
                            strInAutoIds += iAutoId + ",";
                        }
                        
                    }
                }
                if (strInAutoIds.Length>0)
                {
                    strInAutoIds = strInAutoIds.Substring(0, strInAutoIds.Length - 1);
                    
                }
                for (int i = 0; i < dtadd.Rows.Count; i++)
                {
                    string vcSupplier = dtadd.Rows[i]["vcValue1"].ToString();
                    string vcWorkArea = dtadd.Rows[i]["vcValue2"].ToString();
                    string vcStart = Convert.ToDateTime(dtadd.Rows[i]["vcValue3"].ToString()).ToString("yyyy/MM/dd").Replace("/", "-").ToString();
                    string vcFzgc = dtadd.Rows[i]["vcValue5"].ToString();
                    string vcEnd = Convert.ToDateTime(dtadd.Rows[i]["vcValue4"].ToString()).ToString("yyyy/MM/dd").Replace("/", "-").ToString();
                    DataTable dtCheck = fs0108_Logic.checkData(vcSupplier, vcWorkArea, vcStart, vcEnd, strInAutoIds);
                    if (dtCheck.Rows.Count > 0)
                    {
                        DataRow dataRow = dataTable.NewRow();
                        dataRow["vcSupplier"] = vcSupplier;
                        dataRow["vcWorkArea"] = vcWorkArea;
                        dataRow["vcFzgc"] = vcFzgc;
                        dataRow["vcMessage"] = "新增的数据时间区间出现重叠";
                        dataTable.Rows.Add(dataRow);
                        bReault = false;
                    }
                }
                for (int i = 0; i < dtamody.Rows.Count; i++)
                {
                    string vcSupplier = dtamody.Rows[i]["vcValue1"].ToString();
                    string vcWorkArea = dtamody.Rows[i]["vcValue2"].ToString();
                    string vcStart = Convert.ToDateTime(dtamody.Rows[i]["vcValue3"].ToString()).ToString("yyyy/MM/dd").Replace("/", "-").ToString();
                    string vcFzgc = dtamody.Rows[i]["vcValue5"].ToString();
                    string vcEnd = Convert.ToDateTime(dtamody.Rows[i]["vcValue4"].ToString()).ToString("yyyy/MM/dd").Replace("/", "-").ToString();
                    DataTable dtCheck = fs0108_Logic.checkData(vcSupplier, vcWorkArea, vcStart, vcEnd, strInAutoIds);
                    if (dtCheck.Rows.Count > 0)
                    {
                        DataRow dataRow = dataTable.NewRow();
                        dataRow["vcSupplier"] = vcSupplier;
                        dataRow["vcWorkArea"] = vcWorkArea;
                        dataRow["vcFzgc"] = vcFzgc;
                        dataRow["vcMessage"] = "修改的数据时间区间出现重叠";
                        dataTable.Rows.Add(dataRow);
                        bReault = false;
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
                     "vcValue1", "vcValue2", "vcValue5", "vcValue3", "vcValue4"
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
