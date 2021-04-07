using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace SPPSApi.Controllers.G02
{

    [Route("api/FS0203_Sub_Import/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0203Controller_Sub_import : BaseController
    {
        FS0203_Logic fs0203_logic = new FS0203_Logic();
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string FunctionID = "FS0203";

        public FS0203Controller_Sub_import(IWebHostEnvironment webHostEnvironment)
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
            string flag = dataForm.flag;
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
                //0 部品导入
                if (flag == "0")
                {
                    foreach (FileInfo info in theFolder.GetFiles())
                    {
                        List<Hashtable> list = fs0203_logic.GetPartFromFile(info.FullName);
                        if (list.Count > 0)
                        {
                            fs0203_logic.importPartList(list, info.Name, loginInfo.UserId);
                            SaveFile(fileSavePath, "PartList");
                        }
                    }

                }
                //1 SPRL导入
                if (flag == "1")
                {
                    string strMsg = "";

                    string[,] headers = new string[,] {{"工場区分","補給区分","品番－類別","品名","設変ＮＯカラ","適用期間カラ","防錆区分","防錆指示書ＮＯ","補給出荷場所"},
                        {"vcPlant","vcBJDiff","vcPart_Id_new","vcPartName","vcSPINo","vcStartYearMonth","vcFXDiff","vcFXNo","vcNewProj"},
                        {"","","","","","","","",""},
                        {"0","0","0","0","0","0","0","0","0"},//最大长度设定,不校验最大长度用0
                        {"0","1","1","0","1","0","0","0","0"},//最小长度设定,可以为空用0
                    };

                    DataTable importDt = new DataTable();
                    DataTable sprlList = fs0203_logic.getSPRLList();
                    foreach (FileInfo info in theFolder.GetFiles())
                    {
                        DataRow[] rows = sprlList.Select("vcFileName = '" + info.Name + "'");
                        if (rows.Length > 0)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "导入失败,文件" + info.Name + "已上传过无法再次上传。";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
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
                        fs0203_logic.importSPRL(importDt, info.Name, loginInfo.UserId);
                        SaveFile(fileSavePath, "SPRL");
                    }
                    ComFunction.DeleteFolder(fileSavePath);//读取数据后删除文件夹

                }

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "导入成功";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M02UE0303", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导入失败" + ex.Message;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 导入
        [HttpPost]
        public string importApi(IFormFile file)
        {
            deleteOldDir();//删除过期文件夹
            string token = Request.Form["token"].ToString();
            string hashCode = Request.Form["hashCode"].ToString();
            if (!isLogin(token))
            {
                return "error";
            }
            LoginInfo loginInfo = getLoginByToken(token);
            string fileSavePath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "upload" + Path.DirectorySeparatorChar + hashCode + Path.DirectorySeparatorChar;
            //以下开始业务处理
            try
            {
                var filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Replace("\"", ""); // 原文件名（包括路径）
                var extName = filename.Substring(filename.LastIndexOf('.')).Replace("\"", "");// 扩展名
                string shortfilename = filename;

                if (!Directory.Exists(fileSavePath))
                {
                    Directory.CreateDirectory(fileSavePath);
                }


                DirectoryInfo theFolder = new DirectoryInfo(fileSavePath);


                filename = fileSavePath + shortfilename; // 新文件名（包括路径）
                using (FileStream fs = System.IO.File.Create(filename)) // 创建新文件
                {
                    file.CopyTo(fs);// 复制文件
                    fs.Flush();// 清空缓冲区数据
                }
                return shortfilename;
            }
            catch (Exception ex)
            {
                ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                return "error";
            }
        }
        #endregion

        #region 删除过期文件夹
        public void deleteOldDir()
        {
            string fileSavePath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "upload" + Path.DirectorySeparatorChar;
            if (!Directory.Exists(fileSavePath))
            {
                Directory.CreateDirectory(fileSavePath);
            }
            DirectoryInfo theFolder = new DirectoryInfo(fileSavePath);
            List<string> dellist = new List<string>(); ;
            foreach (DirectoryInfo info in theFolder.GetDirectories())
            {
                string strName = info.Name;
                if (strName.IndexOf('_') == -1)
                    continue;
                string strTime = strName.Substring(0, strName.IndexOf('_'));
                DateTime time = DateTime.Parse(strTime);
                TimeSpan span = DateTime.Now - time;
                if (span.Days > 1)
                { //超过1天的冗余文件删除
                    dellist.Add(info.FullName);
                }
            }
            while (dellist.Count > 0)
            {
                ComFunction.DeleteFolder(dellist[0]);
                dellist.RemoveAt(0);
            }
        }
        #endregion

        #region 保存文件

        public void SaveFile(string filePath, string Type)
        {
            try
            {

                DirectoryInfo theFolder = new DirectoryInfo(filePath);

                foreach (FileInfo info in theFolder.GetFiles())
                {
                    ComFunction.FtpUpload("TTCC" + Path.DirectorySeparatorChar + Type, info.FullName);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion


    }


}