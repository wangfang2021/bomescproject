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

namespace SPPSApi.Controllers.G07
{
    [Route("api/FS0702_Sub_Import/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0702Controller_Sub_Import : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0702_Logic FS0702_Logic = new FS0702_Logic();
        private readonly string FunctionID = "FS0702";

        public FS0702Controller_Sub_Import(IWebHostEnvironment webHostEnvironment)
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
                string[,] headers = new string[,] {{"导入状态","对应标识", "变更事项", "包装场", "收货方", "品番,", "车型", "开始时间(部品)", "结束时间(部品)", "包材品番", "GPS品番", "开始时间", "结束时间", "包装材区分", "必要数"},
                                                {"vcIsorNo","iAutoId", "varChangedItem","vcPackSpot","vcShouhuofangID","vcPartsNo","vcCar","dUsedFrom","dUsedTo","vcPackNo","vcPackGPSNo","dFrom","dTo","vcDistinguish","iBiYao"},
                                                {"",FieldCheck.Num,"","","","","",FieldCheck.Date,FieldCheck.Date,"","",FieldCheck.Date,FieldCheck.Date,"",FieldCheck.Num},
                                                {"50","0","50","50","50","50","0","0","0","0","0","0","0","0","0"},
                                                {"0","0","0","0","0","0","0","0","0","0","0","0","0","0","0"}
                                               };//最小长度设定,可以为空用0
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

                        if (row[0] != "")
                        {
                            importDt.ImportRow(row);

                        }
                    }
                }
                ComFunction.DeleteFolder(fileSavePath);//读取数据后删除文件夹

                for (int i = 0; i < importDt.Rows.Count; i++)
                {
                    bool isok = FS0702_Logic.CheckPartsNo(importDt.Rows[i]["vcShouhuofangID"].ToString(), importDt.Rows[i]["vcPartsNo"].ToString());
                    if (!isok)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "第" + (i + 1) + "行的" + "品番:" + importDt.Rows[i]["vcPartsNo"].ToString() + "不存在！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (importDt.Rows[i]["vcIsorNo"].ToString() == "修改")
                    {
                        string strAutoId = importDt.Rows[i]["iAutoId"].ToString() == "" ? "0" : importDt.Rows[i]["iAutoId"].ToString();
                        int iAutoId = Convert.ToInt32(strAutoId);
                        DataTable dtcheckTime = FS0702_Logic.searchcheckTime(importDt.Rows[i]["vcPackSpot"].ToString(), importDt.Rows[i]["vcPartsNo"].ToString(),
                            importDt.Rows[i]["vcPackNo"].ToString(), importDt.Rows[i]["dFrom"].ToString(),
                            importDt.Rows[i]["dTo"].ToString(), iAutoId, importDt.Rows[i]["vcShouhuofangID"].ToString());
                        if (dtcheckTime.Rows.Count > 0)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "品番有维护重复有效时间！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        string sql = "vcPartsNo ='" + importDt.Rows[i]["vcPartsNo"].ToString() + "'  and iAutoId<>'" + iAutoId + "' ";
                        if (!string.IsNullOrEmpty(importDt.Rows[i]["vcPackSpot"].ToString()))
                        {
                            sql = sql + "and vcPackSpot='" + importDt.Rows[i]["vcPackSpot"].ToString() + "'";
                        }
                        if (!string.IsNullOrEmpty(importDt.Rows[i]["vcPackSpot"].ToString()))
                        {
                            sql = sql + "and vcPackNo='" + importDt.Rows[i]["vcPackNo"].ToString() + "'";
                        }
                        if (!string.IsNullOrEmpty(importDt.Rows[i]["vcShouhuofangID"].ToString()))
                        {
                            sql = sql + "and vcShouhuofangID='" + importDt.Rows[i]["vcShouhuofangID"].ToString() + "'";
                        }
                        sql = sql + "and dFrom<='" + importDt.Rows[i]["dTo"].ToString() + "' and dTo>='"+ importDt.Rows[i]["dFrom"].ToString() + "'";
                        DataRow[] dr = importDt.Select(sql);
                        if (dr.Length >= 1)
                        {

                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "导入文件品番有维护重复有效时间！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                    }




                }
                List<Object> strSupplierCode = new List<object>();
                FS0701_Logic FS0701_Logic = new FS0701_Logic();
                DataTable dtPackBase = FS0701_Logic.Search(new List<object>(), "", "", strSupplierCode, "", "", "", "");
                DataTable dtPackitem = FS0702_Logic.Search(new List<object>(), new List<object>(), new List<object>(), "", "", "", "", "", "", "", "");

                FS0702_Logic.importSave(importDt, loginInfo.UserId, dtPackBase, dtPackitem);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "保存成功";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0905", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败" + ex.Message;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion



    }
}
