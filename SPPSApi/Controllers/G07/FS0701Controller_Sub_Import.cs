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
    [Route("api/FS0701_Sub_Import/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0701Controller_Sub_Import : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0701_Logic fs0701_Logic = new FS0701_Logic();
        private readonly string FunctionID = "FS0701";

        public FS0701Controller_Sub_Import(IWebHostEnvironment webHostEnvironment)
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
                string[,] headers = new string[,] {{"是否修改","指定标识","包装场","包装材品番","GPS品番","开始时间", "结束时间", "品名","供应商","供应商代码","发注收容数","资材订购批量","循环","段取区分","场所","规格","发注逻辑"},
                                                {"vcIsorNo","iAutoId","vcPackSpot","vcPackNo","vcPackGPSNo","dPackFrom","dPackTo","vcParstName","vcSupplierName",
                                                 "vcSupplierCode","iRelease","iZCRelease","vcCycle","vcDistinguish","vcPackLocation","vcFormat","vcReleaseName"},
                                                {"",FieldCheck.Num,FieldCheck.NumCharLLL,FieldCheck.NumCharLLL,FieldCheck.NumCharLLL,FieldCheck.Date,FieldCheck.Date,"","","","",FieldCheck.Num,"","","","",""},
                                                {"50","0","50","50","50","0","0","0","0","0","0","0","0","0","0","0","0" },
                                                {"0","0","1","1","1","0","0","0","0","0","0","0","0","0","0","0","0" }
                                               };//最小长度设定,可以为空用0
                DataTable importDt = new DataTable();
                foreach (FileInfo info in theFolder.GetFiles())
                {
                    DataTable dt = ComFunction.ExcelToDataTable(info.FullName, "", headers, ref strMsg);
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
                        if (row[0].ToString() != "")
                        {
                            importDt.ImportRow(row);

                        }

                    }
                }
                ComFunction.DeleteFolder(fileSavePath);//读取数据后删除文件夹




                #region 导入限制
                //查找包装厂
                DataTable dtPS = fs0701_Logic.SearchPackSpot(loginInfo.UserId);
                //资材校验
                DataTable dtMaps = fs0701_Logic.searchmaps();


                for (int i = 0; i < importDt.Rows.Count; i++)
                {
                    if (dtPS.Select("vcValue='" + importDt.Rows[i]["vcPackSpot"].ToString() + "'").Length == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "导入失败:第" + (i + 2) + "行,包装场维护错误！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    DateTime dt1;
                    bool time = DateTime.TryParseExact(Convert.ToDateTime(importDt.Rows[i]["dPackFrom"].ToString()).ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dt1);
                    if (!time)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "导入失败:第" + (i + 2) + "行,开始时间维护错误！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    DateTime dt2;
                    bool time1 = DateTime.TryParseExact(Convert.ToDateTime(importDt.Rows[i]["dPackTo"].ToString()).ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dt2);
                    if (!time1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "导入失败:第" + (i + 2) + "行,结束时间维护错误！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (dt1 > dt2)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "导入失败:第" + (i + 2) + "行,开始时间大于结束时间！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    Regex regex = new System.Text.RegularExpressions.Regex("^(-?[0-9]*[.]*[0-9]{0,3})$");
                    bool b = regex.IsMatch(importDt.Rows[i]["iRelease"].ToString());
                    if (!b)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "导入失败:第" + (i + 2) + "请填写正常的发住收容数格式！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (importDt.Rows[i]["iRelease"].ToString() == "0")
                    {

                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "导入失败:第" + (i + 2) + "收容数不能是'0'！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (!string.IsNullOrEmpty(importDt.Rows[i]["iRelease"].ToString()) && !string.IsNullOrEmpty(importDt.Rows[i]["iZCRelease"].ToString()))
                    {

                        if (Convert.ToInt32(importDt.Rows[i]["iRelease"]) % Convert.ToInt32(importDt.Rows[i]["iZCRelease"]) != 0)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "导入失败:第" + (i + 2) + "行收容数不是导入数据中订购批量的整数倍！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                    }
                    DataRow[] dr1 = dtMaps.Select("PART_NO='" + importDt.Rows[i]["vcPackGPSNo"] + "'");
                    if (dr1.Length > 0)
                    {

                        if (!string.IsNullOrEmpty(importDt.Rows[i]["iRelease"].ToString()))
                        {
                            if (Convert.ToInt32(importDt.Rows[i]["iRelease"]) % Convert.ToInt32(dr1[0]["ORDER_LOT"].ToString()) != 0)
                            {
                                apiResult.code = ComConstant.ERROR_CODE;
                                apiResult.data = "导入失败:第" + (i + 2) + "行收容数不是资材数据中订购批量的整数倍！";
                                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                            }
                        }
                        if (!string.IsNullOrEmpty(importDt.Rows[i]["vcSupplierName"].ToString()))
                        {
                            if (importDt.Rows[i]["vcSupplierCode"].ToString() == dr1[0]["SUPPLIER_CODE"].ToString())
                            {
                                if (importDt.Rows[i]["vcSupplierName"].ToString() != dr1[0]["SUPPLIER_NAME"].ToString())
                                {
                                    apiResult.code = ComConstant.ERROR_CODE;
                                    apiResult.data = "导入失败:第" + (i + 2) + "行维护的供应商名称与资材数据不匹配！";
                                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                                }
                            }

                        }
                    }




                    //if (importDt.Rows[i]["vcIsorNo"].ToString() == "修改")
                    //{
                    string strAutoId = importDt.Rows[i]["iAutoId"].ToString() == "" ? "0" : importDt.Rows[i]["iAutoId"].ToString();
                    int iAutoId = Convert.ToInt32(strAutoId);
                    DataTable dtcheckTime = fs0701_Logic.searchcheckTime(importDt.Rows[i]["vcPackSpot"].ToString(), importDt.Rows[i]["vcPackNo"].ToString(), importDt.Rows[i]["dPackFrom"].ToString().Split(' ')[0], importDt.Rows[i]["dPackTo"].ToString().Split(' ')[0], iAutoId);
                    if (dtcheckTime.Rows.Count > 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "导入失败:第" + (i + 2) + "品番有维护重复有效时间！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    DataRow[] dr = importDt.Select("vcPackNo='" + importDt.Rows[i]["vcPackNo"].ToString() + "'and iAutoId<>'" + iAutoId.ToString() + "' and  dPackFrom<='" + importDt.Rows[i]["dPackTo"].ToString().Split(' ')[0] + "' and  dPackTo>='" + importDt.Rows[i]["dPackFrom"].ToString().Split(' ')[0] + "'");

                    if (dr.Length >= 1)
                    {

                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "导入失败:第" + (i + 2) + "导入文件品番有维护重复有效时间！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    //}

                }
                #endregion

                //var result = from r in importDt.AsEnumerable()
                //             group r by new { r2 = r.Field<string>("vcPackSpot"), r3 = r.Field<string>("vcPackNo"), r4 = r.Field<string>("vcPackGPSNo") } into g
                //             where g.Count() > 1
                //             select g;
                //if (result.Count() > 0)
                //{
                //    StringBuilder sbr = new StringBuilder();
                //    sbr.Append("导入数据重复:<br/>");
                //    foreach (var item in result)
                //    {
                //        sbr.Append("包装场:" + item.Key.r2 + " 品番:" + item.Key.r3 + " GPS品番:" + item.Key.r4 + "<br/>");
                //    }
                //    apiResult.code = ComConstant.ERROR_CODE;
                //    apiResult.data = sbr.ToString();
                //    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                //}




                fs0701_Logic.importSave(importDt, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "保存成功";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M07UE0108", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导入保存失败" + ex.Message;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion



    }
}
