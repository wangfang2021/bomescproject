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
using ICSharpCode.SharpZipLib.Zip;
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

namespace SPPSApi.Controllers.G14
{
    [Route("api/FS1404/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1404Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        FS1404_Logic fS1404_Logic = new FS1404_Logic();
        FS0603_Logic fs0603_Logic = new FS0603_Logic();
        private readonly string FunctionID = "FS1404";

        public FS1404Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        /// <summary>
        /// 初始化方法
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string pageloadApi()
        {
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            //以下开始业务处理
            ApiResult apiResult = new ApiResult();
            try
            {
                Dictionary<string, object> res = new Dictionary<string, object>();
                //处理初始化

                //string zipFileName = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "TagZIP" + Path.DirectorySeparatorChar + "20210311098765432100.zip";
                //string filePath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "TagTempFile";
                //fS1404_Logic.zip(zipFileName, filePath);
                //string zipFileName = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "upload_spis" + Path.DirectorySeparatorChar + "spis";
                //string filePath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "TagTempFile";
                //fS1404_Logic.CopyFolder(zipFileName, filePath);


                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M00UE0006", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "初始化失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 检索方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string searchApi([FromBody]dynamic data)
        {
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            //以下开始业务处理
            ApiResult apiResult = new ApiResult();
            dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));

            string strPartId = dataForm.PartId == null ? "" : dataForm.PartId;
            string strSupplierId = dataForm.SupplierId == null ? "" : dataForm.SupplierId;
            try
            {
                DataTable dataTable = fS1404_Logic.getSearchInfo(strPartId, strSupplierId);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("bAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("bModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("bSelectFlag", ConvertFieldType.BoolType, null);
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dataTable, dtConverter);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0204", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 导出方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string exportApi([FromBody] dynamic data)
        {
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            //以下开始业务处理
            ApiResult apiResult = new ApiResult();
            dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));

            string strPartId = dataForm.PartId == null ? "" : dataForm.PartId;
            string strSupplierId = dataForm.SupplierId == null ? "" : dataForm.SupplierId;
            string strSupplierPlant = dataForm.SupplierPlant == null ? "" : dataForm.SupplierPlant;
            try
            {
                DataTable dataTable = fS1404_Logic.getSearchInfo(strPartId, strSupplierId);
                string[] fields = { "vcPartId", "dFromTime", "dToTime", "vcSupplierId", "vcPicUrl", "vcChangeRea", "vcOperator", "dOperatorTime" };
                string filepath = ComFunction.generateExcelWithXlt(dataTable, fields, _webHostEnvironment.ContentRootPath, "FS1404_Export.xlsx", 1, loginInfo.UserId, FunctionID);
                if (filepath == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "导出生成文件失败";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = filepath;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0904", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 子页面初始化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string subloadApi([FromBody]dynamic data)
        {
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            //以下开始业务处理
            ApiResult apiResult = new ApiResult();
            Dictionary<string, object> res = new Dictionary<string, object>();
            try
            {
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                JArray listInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();

                string strPartId = listInfoData[0]["vcPartId"] == null ? "" : listInfoData[0]["vcPartId"].ToString();
                string strFromTime = listInfoData[0]["dFromTime"] == null ? "" : listInfoData[0]["dFromTime"].ToString();
                string strToTime = listInfoData[0]["dToTime"] == null ? "" : listInfoData[0]["dToTime"].ToString();
                string strSupplierId = listInfoData[0]["vcSupplierId"] == null ? "" : listInfoData[0]["vcSupplierId"].ToString();
                string strSupplierPlant = listInfoData[0]["vcSupplierPlant"] == null ? "" : listInfoData[0]["vcSupplierPlant"].ToString();
                string strPicUrl = listInfoData[0]["vcPicUrl"] == null ? "" : listInfoData[0]["vcPicUrl"].ToString();
                string strChangeRea = listInfoData[0]["vcChangeRea"] == null ? "" : listInfoData[0]["vcChangeRea"].ToString();

                res.Add("modelItem", "mod");
                res.Add("PartIdItem", strPartId);
                res.Add("SupplierIdItem", strSupplierId);
                res.Add("FromTimeItem", strFromTime.Replace("-", "/"));
                res.Add("ToTimeItem", strToTime.Replace("-", "/"));
                res.Add("PicUrlItem", strPicUrl);
                res.Add("ChangeReasonItem", strChangeRea);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0204", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "初始化失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 新增方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string saveApi([FromBody]dynamic data)
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
            try
            {
                string strModel = dataForm.model;
                string strPartId = dataForm.PartId;
                string strSupplierId = dataForm.SupplierId;
                string strFromTime = dataForm.FromTime;
                string strToTime = dataForm.ToTime;
                string strChangeReason = dataForm.ChangeReason;
                string strOperImage = dataForm.PicRoutes;
                string strDelImageRoutes = dataForm.DelPicRoutes;

                DataTable dtImport = fs0603_Logic.createTable("savFs1404");
                DataRow drImport = dtImport.NewRow();
                drImport["LinId"] = "";
                drImport["vcPartId"] = strPartId;
                drImport["dFromTime"] = strFromTime != "" ? Convert.ToDateTime(strFromTime).ToString("yyyy-MM-dd") : "";
                drImport["dToTime"] = strToTime != "" ? Convert.ToDateTime(strToTime).ToString("yyyy-MM-dd") : "";
                drImport["vcSupplierId"] = strSupplierId;
                drImport["vcPicUrl"] = strOperImage;
                drImport["vcChangeRea"] = strChangeReason;
                drImport["vcType"] = "add";
                dtImport.Rows.Add(drImport);
                DataTable dtMessage = fs0603_Logic.createTable("MES");
                string strPath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "upload_spis" + Path.DirectorySeparatorChar + "spis" + Path.DirectorySeparatorChar;
                DataTable dtInfo = fS1404_Logic.checkSaveInfo(dtImport, strPath, ref dtMessage);
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                {
                    dtMessage = dtMessage.DefaultView.ToTable(true, "vcMessage");
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fS1404_Logic.setSaveInfo(dtImport, loginInfo.UserId, ref dtMessage);
                //保存图片
                for (int i = 0; i < dtImport.Rows.Count; i++)
                {
                    if (dtImport.Rows[i]["vcType"].ToString() == "add")
                    {
                        string sources = strPath + dtImport.Rows[i]["vcPicUrl"].ToString();
                        string dest = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Image" + Path.DirectorySeparatorChar + "SPISImage" + Path.DirectorySeparatorChar + dtImport.Rows[i]["vcPicUrlUUID"].ToString();
                        fS1404_Logic.CopyFile(sources, dest);
                    }
                }
                #region 删除照片
                //if (strDelImageRoutes.Length > 0)
                //{
                //    if (strDelImageRoutes.LastIndexOf(",") > 0)
                //    {
                //        string[] images = dataForm.DelImageRoutes.Split(",");
                //        for (int i = 0; i < images.Length; i++)
                //        {
                //            String realPath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Image" + Path.DirectorySeparatorChar + "SPISImage" + Path.DirectorySeparatorChar;
                //            if (System.IO.File.Exists(realPath + images[i]))
                //            {
                //                System.IO.File.Delete(realPath + images[i]);
                //            }
                //        }
                //    }
                //    else
                //    {
                //        String realPath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Image" + Path.DirectorySeparatorChar + "SPISImage" + Path.DirectorySeparatorChar;
                //        if (System.IO.File.Exists(realPath + strDelImageRoutes))
                //        {
                //            System.IO.File.Delete(realPath + strDelImageRoutes);
                //        }
                //    }
                //}
                #endregion
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "保存成功";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0204", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "生成印刷文件失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 下载数据模板
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string downloadTplApi([FromBody] dynamic data)
        {
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            //以下开始业务处理
            ApiResult apiResult = new ApiResult();
            dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));

            try
            {
                DataTable dataTable = fS1404_Logic.getSearchInfo("模板", "");
                string[] fields = { "vcPartId", "dFromTime", "dToTime", "vcSupplierId", "vcPicUrl", "vcChangeRea", "vcOperator", "dOperatorTime" };
                string filepath = ComFunction.generateExcelWithXlt(dataTable, fields, _webHostEnvironment.ContentRootPath, "FS1404_Export.xlsx", 1, loginInfo.UserId, FunctionID);
                if (filepath == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "导出生成文件失败";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = filepath;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0904", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 导入方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
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
                DataTable dtMessage = fs0603_Logic.createTable("MES");
                DirectoryInfo theFolder = new DirectoryInfo(fileSavePath);
                string[,] headers = new string[,] {{"品番","使用开始","使用结束","供应商代码", "SPIS照片路径","变更理由"},
                                                {"vcPartId", "dFromTime", "dToTime", "vcSupplierId","vcPicUrl","vcChangeRea"},
                                                {FieldCheck.NumCharLLL,FieldCheck.Date,FieldCheck.Date,"","",""},
                                                {"12","10","10","5", "0", "0"},//最大长度设定,不校验最大长度用0
                                                {"12","10","10","0", "0", "0"}};//最小长度设定,可以为空用0
                foreach (FileInfo info in theFolder.GetFiles())
                {
                    string ssss = info.FullName;
                }
                DataTable dtImport = fS1404_Logic.ImportFile(theFolder, fileSavePath, "sheet1", headers, true, loginInfo.UserId, ref dtMessage);
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string strPath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "upload_spis" + Path.DirectorySeparatorChar + "spis" + Path.DirectorySeparatorChar;
                DataTable dtInfo = fS1404_Logic.checkSaveInfo(dtImport, strPath, ref dtMessage);
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fS1404_Logic.setSaveInfo(dtImport, loginInfo.UserId, ref dtMessage);
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
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
        /// <summary>
        /// 下载照片压缩包
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string downspisApi([FromBody] dynamic data)
        {
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            //以下开始业务处理
            ApiResult apiResult = new ApiResult();
            dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
            string strSupplierId = dataForm.SupplierId == null ? "" : dataForm.SupplierId;
            List<Object> listTime = dataForm.TimeList.ToObject<List<Object>>();
            Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();
            string destpic = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Image" + Path.DirectorySeparatorChar + "SPISImage" + Path.DirectorySeparatorChar;
            string zipName = "SPIS_" + strSupplierId + "_" + System.DateTime.Now.ToString("yyyyMMddhhmmss") + ".zip";
            try
            {
                DataTable dtMessage = fs0603_Logic.createTable("MES");
                DataTable dtInfo = fS1404_Logic.getSearchsubInfo(strSupplierId, listTime);
                if(dtInfo.Rows.Count==0)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "未查询出任何数据，请检查条件。";
                    dtMessage.Rows.Add(dataRow);
                }
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                for (int i = 0; i < dtInfo.Rows.Count; i++)
                {
                    string key = dtInfo.Rows[i]["vcPicUrl"].ToString();
                    files.Add(key, System.IO.File.ReadAllBytes(destpic + key));
                }
                if (files.Count > 1)
                {
                    string destZip = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Export" + Path.DirectorySeparatorChar;
                    using (FileStream zip = System.IO.File.Create(destZip + zipName))
                    {
                        using (ZipOutputStream zipStream = new ZipOutputStream(zip))
                        {
                            foreach (KeyValuePair<string, byte[]> kv in files)
                            {
                                //压缩包内条目
                                ZipEntry entry = new ZipEntry(kv.Key);
                                //添加条目
                                zipStream.PutNextEntry(entry);
                                //设置压缩级别1~9
                                zipStream.SetLevel(5);
                                //写入
                                zipStream.Write(kv.Value, 0, kv.Value.Length);
                            }
                        }
                    }
                }
                if (zipName == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "导出生成文件失败";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = zipName;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0904", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }

    }
}
