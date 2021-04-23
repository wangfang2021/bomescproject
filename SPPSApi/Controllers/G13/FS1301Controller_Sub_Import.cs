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
    [Route("api/FS1301_Sub_Import/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1301Controller_Sub_Import : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS1301_Logic fs1301_Logic = new FS1301_Logic();
        private readonly string FunctionID = "FS1301";

        public FS1301Controller_Sub_Import(IWebHostEnvironment webHostEnvironment)
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
                string[,] headers = new string[,] {{"包装工厂","员工编号", "员工姓名", "用户角色", "现场入荷作业", "现场入荷解锁", "现场检查作业", "现场检查解锁", "现场包装作业", "现场包装解锁", "现场出荷作业", "现场出荷解锁" },
                                                { "vcPackingPlant", "vcUserId", "vcUserName", "vcRoleName", "bInPut", "bInPutUnLock", "bCheck", "bCheckUnLock", "bPack", "bPackUnLock", "bOutPut", "bOutPutUnLock"},
                                                {FieldCheck.NumChar,FieldCheck.NumChar,"","","","","","","","","","",},
                                                {"10","10","20","20","0","0","0","0","0","0","0","0"},//最大长度设定,不校验最大长度用0
                                                {"1","1","0","0","0","0","0","0","0","0","0","0"}};//最小长度设定,可以为空用0
                
                DataTable importDt = new DataTable();
                foreach (FileInfo info in theFolder.GetFiles())
                {
                    DataTable dt = ComFunction.ExcelToDataTableformRows(info.FullName, "sheet1", headers, 2, 2, ref strMsg);
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
                             group r by new { r2 = r.Field<string>("vcPackingPlant"), r3 = r.Field<string>("vcUserId") } into g
                             where g.Count() > 1
                             select g;
                if (result.Count() > 0)
                {
                    StringBuilder sbr = new StringBuilder();
                    sbr.Append("导入数据重复:<br/>");
                    foreach (var item in result)
                    {
                        sbr.Append("包装工厂:" + item.Key.r2 + " 员工编号:" + item.Key.r3 + "<br/>");
                    }
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = sbr.ToString();
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //判断新增的数据四个主键是否在数存在重复
                StringBuilder sbrUserId = new StringBuilder();
                bool isExist = true;
                sbrUserId.Append("员工编号:<br/>");
                for (int i=0;i< importDt.Rows.Count;i++)
                {
                    DataTable dt = fs1301_Logic.isCheckUserId(importDt.Rows[i]["vcUserId"].ToString().Trim());
                    if (dt.Rows.Count==0)
                    {
                        isExist = false;
                        sbrUserId.Append("员工编号:" + importDt.Rows[i]["vcUserId"].ToString().Trim() + "不存在员工信息表中"+"<br/>");
                    }
                }
                if (!isExist)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = sbrUserId.ToString();
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fs1301_Logic.importSave(importDt, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "保存成功";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M13PE0103", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "批量维护失败";
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
            string strPackingPlant = dataForm.searchForm.PackingPlant == null ? "" : dataForm.searchForm.PackingPlant;
            string strUser = dataForm.searchForm.User == null ? "" : dataForm.searchForm.User;
            string strRoler = dataForm.searchForm.Roler == null ? "" : dataForm.searchForm.Roler;
            try
            {
                DataTable dt = fs1301_Logic.getSearchInfo(strPackingPlant, strUser, strRoler);
                for (int i=0;i<dt.Rows.Count;i++)
                {
                    if (dt.Rows[i]["bInPut"].ToString()=="1")
                    {
                        dt.Rows[i]["bInPut"] = "√";
                    }
                    else
                    {
                        dt.Rows[i]["bInPut"] = "×";
                    }
                    if (dt.Rows[i]["bInPutUnLock"].ToString() == "1")
                    {
                        dt.Rows[i]["bInPutUnLock"] = "√";
                    }
                    else
                    {
                        dt.Rows[i]["bInPutUnLock"] = "×";
                    }
                    if (dt.Rows[i]["bCheck"].ToString() == "1")
                    {
                        dt.Rows[i]["bCheck"] = "√";
                    }
                    else
                    {
                        dt.Rows[i]["bCheck"] = "×";
                    }
                    if (dt.Rows[i]["bCheckUnLock"].ToString() == "1")
                    {
                        dt.Rows[i]["bCheckUnLock"] = "√";
                    }
                    else
                    {
                        dt.Rows[i]["bCheckUnLock"] = "×";
                    }
                    if (dt.Rows[i]["bPack"].ToString() == "1")
                    {
                        dt.Rows[i]["bPack"] = "√";
                    }
                    else
                    {
                        dt.Rows[i]["bPack"] = "×";
                    }
                    if (dt.Rows[i]["bPackUnLock"].ToString() == "1")
                    {
                        dt.Rows[i]["bPackUnLock"] = "√";
                    }
                    else
                    {
                        dt.Rows[i]["bPackUnLock"] = "×";
                    }
                    if (dt.Rows[i]["bOutPut"].ToString() == "1")
                    {
                        dt.Rows[i]["bOutPut"] = "√";
                    }
                    else
                    {
                        dt.Rows[i]["bOutPut"] = "×";
                    }
                    if (dt.Rows[i]["bOutPutUnLock"].ToString() == "1")
                    {
                        dt.Rows[i]["bOutPutUnLock"] = "√";
                    }
                    else
                    {
                        dt.Rows[i]["bOutPutUnLock"] = "×";
                    }
                }
                string[] fields = {
                    "vcPackingPlant", "vcUserId", "vcUserName", "vcRoleName", "bInPut", "bInPutUnLock", "bCheck", "bCheckUnLock", "bPack", "bPackUnLock", "bOutPut", "bOutPutUnLock"
                };

                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS1301_Template.xlsx", 1, loginInfo.UserId, FunctionID);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M13PE0104", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "下载模板失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        
    }
}
