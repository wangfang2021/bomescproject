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
    [Route("api/FS0628_Sub_Import/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0628Controller_Sub_Import : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0628_Logic fs0628_Logic = new FS0628_Logic();
        private readonly string FunctionID = "FS0628";

        public FS0628Controller_Sub_Import(IWebHostEnvironment webHostEnvironment)
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

                //const tHeader = ["导入时间", "车型", "品番", "品名", "内外", "供应商代码", "工区", "是否新规", "OE=SP", "受入", "号试数量", "号试目的", "订单预计发行日", "订单预计纳入日", "纳入便次", "实际纳入日", "结算订单号", "结算订单验收日期", "号试订单发送日期", "备注"];
                //const filterVal = ["dExportDate", "vcCarType", "vcPartNo", "vcPartName", "vcInsideOutsideType", "vcSupplier_id", "vcWorkArea", "vcIsNewRulesFlag", "vcOEOrSP", "vcDock", "vcNumber", "vcPurposes", "dOrderPurposesDate", "dOrderReceiveDate", "vcReceiveTimes", "dActualReceiveDate", "vcAccountOrderNo", "dAccountOrderReceiveDate", "dOrderSendDate", "vcMemo"];

                string[,] headers = new string[,] {{ "iAutoId", "进度状态", "订单处理日", "订单号", "品番", "内外", "号旧区分", "发注工场", "受入", "供应商代码", "工区", "出荷场", "车型", "订货数量", "预计纳期", "订单回数", "发注订单号", "备注"},
                                                { "iAutoId", "vcIsExportFlag", "dOrderHandleDate", "vcOrderNo", "vcPartNo", "vcInsideOutsideType", "vcNewOldFlag", "vcInjectionFactory", "vcDock", "vcSupplier_id", "vcWorkArea", "vcCHCCode", "vcCarType", "vcOrderNum", "dExpectReceiveDate", "vcOderTimes", "vcInjectionOrderNo", "vcMemo"},
                                                {"","",FieldCheck.Date,FieldCheck.NumCharLLL,FieldCheck.NumCharLLL,"","","","",FieldCheck.NumCharLLL,"","","",FieldCheck.Float,FieldCheck.Date,FieldCheck.Num,FieldCheck.NumCharLLL,""},
                                                {"0","0","0","50","12","100","20","100","20","4","50","50","50","20","0","20","50","500"},//最大长度设定,不校验最大长度用0
                                                {"0","0","0","0","1","1","1","1","0","1","0","0","0","1","0","0","0","0"}};//最小长度设定,可以为空用0
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

                //var result = from r in importDt.AsEnumerable()
                //             group r by new { r2 = r.Field<string>("vcGroupName"), r3 = r.Field<string>("vcPartNo")

                //             } into g
                //             where g.Count() > 1
                //             select g;
                //if (result.Count() > 0)
                //{
                //    StringBuilder sbr = new StringBuilder();
                //    sbr.Append("导入数据重复:<br/>");
                //    foreach (var item in result)
                //    {
                //        sbr.Append("组别:" + item.Key.r2 + " 品番:" + item.Key.r3 +"<br/>");
                //    }
                //    apiResult.code = ComConstant.ERROR_CODE;
                //    apiResult.data = sbr.ToString();
                //    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                //}
                //string[] groupResult =  importDt.AsEnumerable().Select(a=> a.Field<string>("vcGroupName")).Distinct().ToArray();
                ////验证是否存在不存在的组名 true 存在
                //DataTable dtGroup= fs0628_Logic.GetGroupNameDt();

                //string strGroupNames = string.Empty;
                //if (dtGroup.Rows.Count>0)
                //{
                //    for (int i=0;i< dtGroup.Rows.Count;i++)
                //    {
                //        if (i != 0) { strGroupNames += ","; }
                //        strGroupNames += dtGroup.Rows[i][0].ToString();
                //    }
                //}
                //if (strGroupNames.Length>0)
                //{
                //    string strErr = string.Empty;
                //    for (int i = 0; i < groupResult.Length; i++)
                //    {
                //        if (!strGroupNames.Contains(groupResult[i]))
                //        {
                //            strErr += groupResult[i] + "、";
                //        }
                //    }
                //    if (strErr.Length > 0)
                //    {
                //        strErr = strErr.Substring(0, strErr.LastIndexOf('、'));
                //        apiResult.code = ComConstant.ERROR_CODE;
                //        apiResult.data = strErr.ToString()+"不存在，请先追加组别,再导入数据";
                //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                //    }
                //}
                
                fs0628_Logic.importSave(importDt, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "保存成功";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2505", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败" + ex.Message;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion



    }
}
