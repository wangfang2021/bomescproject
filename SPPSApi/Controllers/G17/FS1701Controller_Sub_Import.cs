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

namespace SPPSApi.Controllers.G17
{
    [Route("api/FS1701_Sub_Import/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1701Controller_Sub_Import : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS1701_Logic fs1701_Logic = new FS1701_Logic();
        private readonly string FunctionID = "FS1701";

        public FS1701Controller_Sub_Import(IWebHostEnvironment webHostEnvironment)
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
            string type = dataForm.type == null ? "" : dataForm.type;
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
                string[,] headers = new string[,] {{"票号","零件号码","原订单号","数量","是否到齐"},
                                                {"vcTicketNo", "vcLJNo", "vcOldOrderNo", "iQuantity","vcIsDQ"},
                                                {FieldCheck.NumChar,FieldCheck.NumChar,FieldCheck.NumChar,FieldCheck.Num,""},
                                                {"50","50","50","0","25"},//最大长度设定,不校验最大长度用0
                                                {"1","1","1","1","0"}};//最小长度设定,可以为空用0
                //DataTable importDt = new DataTable();
                DataSet ds = new DataSet();
                foreach (FileInfo info in theFolder.GetFiles())
                {
                    //DataTable dt = ComFunction.ExcelToDataTable(info.FullName, "sheet1", headers, ref strMsg);
                    string strTickNo = "";
                    DataTable dt = fs1701_Logic.ExcelToDataTable(info.FullName, "sheet1", ref strMsg,ref strTickNo);
                    if (strMsg != "")
                    {
                        ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "导入终止，文件" + info.Name + ":" + strMsg;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    //if (importDt.Columns.Count == 0)
                    //    importDt = dt.Clone();
                    if (dt.Rows.Count == 0)
                    {
                        ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "导入终止，文件" + info.Name + "没有要导入的数据";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    //判断 票号 是否导入过，如果导入过提示是否继续导入，如果是则导入，否返回
                    if (type == "check" && fs1701_Logic.isImport(strTickNo))
                    {//已经计算了
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.type = "message";
                        apiResult.data = "已经导入过";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }

                    dt.TableName = strTickNo;
                    ds.Tables.Add(dt);
                    //foreach (DataRow row in dt.Rows)
                    //{
                    //    importDt.ImportRow(row);
                    //}
                }
                ComFunction.DeleteFolder(fileSavePath);//读取数据后删除文件夹

                #region not use
                //for (int i = 0; i < importDt.Rows.Count; i++)
                //{
                //    string vcIsDQ = importDt.Rows[i]["vcIsDQ"].ToString();
                //    if (vcIsDQ != "" && Array.IndexOf(new string[] { "是", "否" }, vcIsDQ) == -1)
                //    {
                //        apiResult.code = ComConstant.ERROR_CODE;
                //        apiResult.data = "[是否到齐]列只能填写是/否";
                //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                //    }
                //}

                //var result = from r in importDt.AsEnumerable()
                //             group r by new
                //             {
                //                 r2 = r.Field<string>("vcTicketNo"),
                //                 r3 = r.Field<string>("vcLJNo"),
                //                 r4 = r.Field<string>("vcOldOrderNo")
                //             } into g
                //             where g.Count() > 1
                //             select g;
                //if (result.Count() > 0)
                //{
                //    StringBuilder sbr = new StringBuilder();
                //    sbr.Append("导入数据重复:<br/>");
                //    foreach (var item in result)
                //    {
                //        string str = string.Format("票号：{0}、零件号：{1}、原订单号：{2} <br/>", item.Key.r2, item.Key.r3, item.Key.r4);
                //        sbr.Append(str);
                //    }
                //    apiResult.code = ComConstant.ERROR_CODE;
                //    apiResult.data = sbr.ToString();
                //    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                //}
                #endregion

                #region 文件内容校验
                for(int i=0;i<ds.Tables.Count;i++)
                {
                    DataTable dt = ds.Tables[i];
                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        string vcType = dt.Rows[j]["vcIsDQ"].ToString();
                        if (vcType!="" && Array.IndexOf(new string[] {"是", "否" }, vcType) == -1)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "[是否到齐]列只能填写是/否";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                    }
                }
                #endregion

                fs1701_Logic.importSave_Sub(ds, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "保存成功";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M17UE0107", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导入失败" + ex.Message;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}
