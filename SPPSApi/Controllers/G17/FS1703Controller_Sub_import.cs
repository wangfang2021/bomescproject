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

namespace SPPSApi.Controllers.G17
{
    [Route("api/FS1703_Sub_import/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1703Controller_Sub_import : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS1703_Logic fs1703_Logic = new FS1703_Logic();
        private readonly string FunctionID = "FS1703";

        public FS1703Controller_Sub_import(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        #region 导入之后点保存
        //[HttpPost]
        //[EnableCors("any")]
        //public string importSaveApi([FromBody]dynamic data)
        //{
        //    //验证是否登录
        //    string strToken = Request.Headers["X-Token"];
        //    if (!isLogin(strToken))
        //    {
        //        return error_login();
        //    }
        //    LoginInfo loginInfo = getLoginByToken(strToken);
        //    //以下开始业务处理
        //    ApiResult apiResult = new ApiResult();
        //    dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
        //    JArray fileNameList = dataForm.fileNameList;
        //    string hashCode = dataForm.hashCode;
        //    string fileSavePath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "upload" + Path.DirectorySeparatorChar + hashCode + Path.DirectorySeparatorChar;
        //    try
        //    {
        //        if (!Directory.Exists(fileSavePath))
        //        {
        //            ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
        //            apiResult.code = ComConstant.ERROR_CODE;
        //            apiResult.data = "没有要导入的文件，请重新上传！";
        //            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //        }
        //        DirectoryInfo theFolder = new DirectoryInfo(fileSavePath);
        //        string strMsg = "";
        //        string[,] headers = new string[,] {{"纳入品番","所番地","应有数量","实际数量"},
        //                                        {"vcPart_id","vcPlace" ,"iSystemQuantity", "iRealQuantity"},
        //                                        {FieldCheck.NumCharL,"",FieldCheck.Num,FieldCheck.Num},
        //                                        {"500","500","500","5000","25"},//最大长度设定,不校验最大长度用0
        //                                        {"1","0","0","0","1"}};//最小长度设定,可以为空用0
        //        DataTable importDt = new DataTable();
        //        foreach (FileInfo info in theFolder.GetFiles())
        //        {
        //            DataTable dt = ComFunction.ExcelToDataTable(info.FullName, "sheet1", headers, ref strMsg);
        //            if (strMsg != "")
        //            {
        //                ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
        //                apiResult.code = ComConstant.ERROR_CODE;
        //                apiResult.data = "导入终止，文件" + info.Name + ":" + strMsg;
        //                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //            }
        //            if (importDt.Columns.Count == 0)
        //                importDt = dt.Clone();
        //            if (dt.Rows.Count == 0)
        //            {
        //                ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
        //                apiResult.code = ComConstant.ERROR_CODE;
        //                apiResult.data = "导入终止，文件" + info.Name + "没有要导入的数据";
        //                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //            }
        //            foreach (DataRow row in dt.Rows)
        //            {
        //                importDt.ImportRow(row);
        //            }
        //        }
        //        ComFunction.DeleteFolder(fileSavePath);//读取数据后删除文件夹

        //        #region 导入文件处理，按逗号拆分成多条数据

        //        DataTable dtBoss = importDt.Clone();
        //        for (int i = 0; i < importDt.Rows.Count; i++)
        //        {
        //            string vcSR = importDt.Rows[i]["vcSR"].ToString();
        //            string vcSupplier_id = importDt.Rows[i]["vcSupplier_id"].ToString();
        //            string vcPartsNoBefore5 = importDt.Rows[i]["vcPartsNoBefore5"].ToString();
        //            string vcBCPartsNo = importDt.Rows[i]["vcBCPartsNo"].ToString();
        //            string vcSmallPM = importDt.Rows[i]["vcSmallPM"].ToString();

        //            List<string> list_sr = new List<string>();
        //            List<string> list_supplier = new List<string>();
        //            List<string> list_part = new List<string>();
        //            List<string> list_bcpart = new List<string>();

        //            if (vcSR == "")
        //                list_sr.Add("");
        //            else if (!vcSR.Contains(","))
        //                list_sr.Add(vcSR);
        //            else if (vcSR.Contains(","))
        //                list_sr = vcSR.Split(',').ToList<string>();

        //            if (vcSupplier_id == "")
        //                list_supplier.Add("");
        //            else if (!vcSupplier_id.Contains(","))
        //                list_supplier.Add(vcSupplier_id);
        //            else if (vcSupplier_id.Contains(","))
        //                list_supplier = vcSupplier_id.Split(',').ToList<string>();

        //            if (vcPartsNoBefore5 == "")
        //                list_part.Add("");
        //            else if (!vcPartsNoBefore5.Contains(","))
        //                list_part.Add(vcPartsNoBefore5);
        //            else if (vcPartsNoBefore5.Contains(","))
        //                list_part = vcPartsNoBefore5.Split(',').ToList<string>();

        //            if (vcBCPartsNo == "")
        //                list_bcpart.Add("");
        //            else if (!vcBCPartsNo.Contains(","))
        //                list_bcpart.Add(vcBCPartsNo);
        //            else if (vcBCPartsNo.Contains(","))
        //                list_bcpart = vcBCPartsNo.Split(',').ToList<string>();

        //            var res = from sr in list_sr
        //                      from supplier in list_supplier
        //                      from part in list_part
        //                      from bcpart in list_bcpart
        //                      select new { sr, supplier, part, bcpart };
        //            if (res.Count() > 0)
        //            {
        //                foreach (var item in res)
        //                {
        //                    DataRow dr = dtBoss.NewRow();
        //                    string sr = item.sr;
        //                    string supplier = item.supplier;
        //                    string part = item.part;
        //                    string bcpart = item.bcpart;

        //                    //这4个字段不能同时为空(受入号，厂家代码，品番前5位，包材品番)
        //                    if (sr == "" && supplier == "" && part == "" && bcpart == "")
        //                    {
        //                        StringBuilder sbr = new StringBuilder();
        //                        sbr.Append("[受入号、厂家代码、品番前5位、包材品番]不能同时为空<br/>");
        //                        apiResult.code = ComConstant.ERROR_CODE;
        //                        apiResult.data = sbr.ToString();
        //                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //                    }

        //                    dr["vcSR"] = sr;
        //                    dr["vcSupplier_id"] = supplier;
        //                    dr["vcPartsNoBefore5"] = part;
        //                    dr["vcBCPartsNo"] = bcpart;
        //                    dr["vcSmallPM"] = vcSmallPM;
        //                    dtBoss.Rows.Add(dr);
        //                }
        //            }
        //        }
        //        #endregion

        //        var result = from r in dtBoss.AsEnumerable()
        //                     group r by new { r2 = r.Field<string>("vcSR"), r3 = r.Field<string>("vcSupplier_id"), r4 = r.Field<string>("vcPartsNoBefore5"), r5 = r.Field<string>("vcBCPartsNo") } into g
        //                     where g.Count() > 1
        //                     select g;
        //        if (result.Count() > 0)
        //        {
        //            StringBuilder sbr = new StringBuilder();
        //            sbr.Append("导入数据重复:<br/>");
        //            foreach (var item in result)
        //            {
        //                sbr.Append("受入号:" + item.Key.r2 + " 厂家代码:" + item.Key.r3 + " 品番前5位:" + item.Key.r4 + " 包材品番:" + item.Key.r5 + "<br/>");
        //            }
        //            apiResult.code = ComConstant.ERROR_CODE;
        //            apiResult.data = sbr.ToString();
        //            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //        }

        //        fs0810_Logic.importSave_Sub(dtBoss, loginInfo.UserId);
        //        apiResult.code = ComConstant.SUCCESS_CODE;
        //        apiResult.data = "保存成功";
        //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //    }
        //    catch (Exception ex)
        //    {
        //        ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
        //        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0905", ex, loginInfo.UserId);
        //        apiResult.code = ComConstant.ERROR_CODE;
        //        apiResult.data = "保存失败" + ex.Message;
        //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //    }
        //}
        #endregion

    }


}
