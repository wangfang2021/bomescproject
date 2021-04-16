using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.Util;
using Org.BouncyCastle.Utilities.Encoders;

namespace SPPSApi.Controllers.G05
{
    [Route("api/FS0503/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0503Controller : BaseController
    {
        FS0503_Logic fs0503_Logic = new FS0503_Logic();
        private readonly string FunctionID = "FS0503";

        private readonly IWebHostEnvironment _webHostEnvironment;


        public FS0503Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        #region 页面初始化
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
                DataTable task = fs0503_Logic.GetTaskNum(loginInfo.UserId);//待回复的数据
                DataTable task1 = fs0503_Logic.GetTaskNum1(loginInfo.UserId);//待回复的数据
                DataTable WorkArea = fs0503_Logic.GetWorkArea(loginInfo.UserId);
                DataTable dtCarType = fs0503_Logic.GetCarType(loginInfo.UserId);
                DataTable dtExpectDeliveryDate = fs0503_Logic.GetExpectDeliveryDate(loginInfo.UserId);
                DataTable dtSendDate = fs0503_Logic.GetSendDate(loginInfo.UserId);
                DataTable dtReplyDate = fs0503_Logic.GetReplyDate(loginInfo.UserId);

                List<Object> dataList_C034 = ComFunction.convertAllToResult(ComFunction.getTCode("C034"));//荷姿状态
                List<Object> dataList_WorkAreae = ComFunction.convertToResult(WorkArea, new string[] { "vcValue", "vcName" });
                List<Object> dataList_CarType = ComFunction.convertToResult(dtCarType, new string[] { "vcValue", "vcName" });
                List<Object> dataList_ExpectDeliveryDate = ComFunction.convertToResult(dtExpectDeliveryDate, new string[] { "vcValue", "vcName" });
                List<Object> dataList_SendDate = ComFunction.convertToResult(dtSendDate, new string[] { "vcValue", "vcName" });
                List<Object> dataList_ReplyDate = ComFunction.convertToResult(dtReplyDate, new string[] { "vcValue", "vcName" });
                res.Add("C034", dataList_C034);
                res.Add("taskNum", task.Rows.Count);
                res.Add("taskNum1", task1.Rows.Count);
                res.Add("WorkArea", dataList_WorkAreae);
                res.Add("ExpectDeliveryDate", dataList_ExpectDeliveryDate);
                res.Add("SendDate", dataList_SendDate);
                res.Add("ReplyDate", dataList_ReplyDate);
                res.Add("CarType", dataList_CarType);
                res.Add("userId", loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M05UE0301", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "初始化失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 检索
        [HttpPost]
        [EnableCors("any")]
        public string searchApi([FromBody] dynamic data)
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
            string vcSupplier_id = loginInfo.UserId;
            string vcWorkArea = dataForm.vcWorkArea == null ? "" : dataForm.vcWorkArea;
            string vcState = dataForm.vcState == null ? "" : dataForm.vcState;
            string vcPartNo = dataForm.vcPartNo == null ? "" : dataForm.vcPartNo;

            string vcCarType = dataForm.vcCarType == null ? "" : dataForm.vcCarType;
            string dExpectDeliveryDate = dataForm.dExpectDeliveryDate == null ? "" : dataForm.dExpectDeliveryDate;
            string dSendDate = dataForm.dSendDate == null ? "" : dataForm.dSendDate;
            string dReplyDate = dataForm.dReplyDate == null ? "" : dataForm.dReplyDate;

            try
            {

                DataTable dt = fs0503_Logic.Search(vcSupplier_id, vcWorkArea, vcState, vcPartNo, vcCarType, dExpectDeliveryDate,loginInfo.UserId, dSendDate, dReplyDate);
                #region delete 

                //if (dt.Rows.Count > 0)
                //{
                //    for (int i = 0; i < dt.Rows.Count; i++)
                //    {
                //        var vcImageRoutes = dt.Rows[i]["vcImageRoutes"].ToString();
                //        if (vcImageRoutes.Length > 0)
                //        {
                //            //"data:image/"+file.getCrfileExtention()+";base64,"+getImageStrFromPath(rootPath+file.getCrfileServerPath())
                //            StringBuilder strBuild = new StringBuilder();
                //            string[] images = vcImageRoutes.Split(",");

                //            for (var j = 0; j < images.Length; j++)
                //            {
                //                string imageType = images[j].Substring(images[j].LastIndexOf('.') + 1).Replace("\"", "");
                //                //string baseStr = "data:image/" + imageType + ";base64,"+getBaseApi(images[j])+",";
                //                string baseStr = getBaseApi(images[j]) + "?";
                //                strBuild.Append(baseStr);
                //            }
                //            dt.Rows[i]["vcImageRoutes"] = strBuild.Remove(strBuild.Length - 1, 1).ToString();
                //        }
                //    }
                //}
                #endregion
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("dSynchronizationDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dExpectDeliveryDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dOrderReceiveDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dUseStartDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dUserEndDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dSendDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dReplyDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dAdmitDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dWeaveDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M05UE0302", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 导出
        [HttpPost]
        [EnableCors("any")]
        public string exportApi([FromBody] dynamic data)
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
            string vcSupplier_id = loginInfo.UserId;
            string vcWorkArea = dataForm.vcWorkArea == null ? "" : dataForm.vcWorkArea;
            string vcState = dataForm.vcState == null ? "" : dataForm.vcState;
            string vcPartNo = dataForm.vcPartNo == null ? "" : dataForm.vcPartNo;

            string vcCarType = dataForm.vcCarType == null ? "" : dataForm.vcCarType;
            string dExpectDeliveryDate = dataForm.dExpectDeliveryDate == null ? "" : dataForm.dExpectDeliveryDate;
            string dSendDate = dataForm.dSendDate == null ? "" : dataForm.dSendDate;
            string dReplyDate = dataForm.dReplyDate == null ? "" : dataForm.dReplyDate;
            try
            {
                DataTable dt = fs0503_Logic.Search(vcSupplier_id, vcWorkArea, vcState, vcPartNo, vcCarType, dExpectDeliveryDate, loginInfo.UserId, dSendDate, dReplyDate);
                string[] head = new string[] { };
                string[] field = new string[] { };
                //[vcPartNo], [dBeginDate], [dEndDate]"使用结束时间",
                //表格列排序：状态 - 展开时间 - 要望纳期 - 包装工厂 - 品番 - 品名 - 车型 - 使用开始 - OE = SP - 供应商代码 - 工区 - 要望收容数 - 收容数 - 箱最大收容数 - 箱种 - 长宽高 - 空箱重量 - 单品净重 - 照片 - 备注
                head = new string[] { "状态", "展开时间", "要望纳期", "包装工场", "收货方" , "品番",  "品名", "车型", "使用开始时间", "OE=SP", "供应商代码", "工区", "要望收容数", "收容数", "箱最大收容数", "箱种", "长(mm)", "宽(mm)", "高(mm)", "空箱重量(g)", "单品净重(g)",  "回复时间", "承认时间", "原单位织入时间", "备注" };
                field = new string[] { "vcState", "dSendDate","dExpectDeliveryDate", "vcPackingPlant", "vcReceiver", "vcPartNo",  "vcPartName", "vcCarType","dUseStartDate", "vcOEOrSP", "vcSupplier_id", "vcWorkArea", "vcExpectIntake", "vcIntake", "vcBoxMaxIntake", "vcBoxType", "vcLength", "vcWide", "vcHeight", "vcEmptyWeight", "vcUnitNetWeight",  "dReplyDate", "dAdmitDate", "dWeaveDate", "vcMemo" };
                string msg = string.Empty;
                //string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0309_Export.xlsx", 2, loginInfo.UserId, FunctionID);
                //string filepath = ComFunction.DataTableToExcel(head, field, dt, ".", loginInfo.UserId, FunctionID, ref msg);
                string filepath = ComFunction.generateExcelWithXlt(dt, field, _webHostEnvironment.ContentRootPath, "FS0503_Data.xlsx", 1, loginInfo.UserId, FunctionID, true);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M05UE0303", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion


        #region 通过选中数据获取出荷信息
        [HttpPost]
        [EnableCors("any")]
        public string getChuHeBySelectApi([FromBody] dynamic data)
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
            JArray listInfo = dataForm.multipleSelection;
            List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();

            try
            {
                if (listInfoData.Count != 1)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请选中一条数据进行编辑，请确认！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                String strIAutoId = listInfoData[0]["iAutoId"].ToString();
                DataTable dt = fs0503_Logic.SearchByIAutoId(strIAutoId);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("dSynchronizationDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dExpectDeliveryDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dOrderReceiveDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dSendDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dReplyDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dAdmitDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dWeaveDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M05UE0304", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "编辑查询失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 编辑对话框确定按钮
        [HttpPost]
        [EnableCors("any")]
        public string editOkApi([FromBody] dynamic data)
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
            string vcBoxType = dataForm.vcBoxType == null ? "" : dataForm.vcBoxType;
            try
            {
                Regex r = new Regex("^[a-zA-Z0-9]+$");
                if (vcBoxType.Length>0) { 
                    if (!r.IsMatch(vcBoxType))
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "箱种仅支持英数格式！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                string vcImageRoutes = dataForm.vcImageRoutes;
                if (vcImageRoutes.Length>0)
                {
                    //Console.WriteLine("开始保存照片");
                    //Console.WriteLine(vcImageRoutes);
                    string hashCode = dataForm.hashCode;
                    //Console.WriteLine(hashCode);
                    string fileSavePath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "upload" + Path.DirectorySeparatorChar + hashCode + Path.DirectorySeparatorChar;
                    //string filePath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Image" + Path.DirectorySeparatorChar + "HeZiImages";
                    //Console.WriteLine(fileSavePath);
                    SaveFile(fileSavePath);
                }
                if (fs0503_Logic.editOk(dataForm, loginInfo.UserId))
                {
                    //有误需要删除的冗余图片
                    //string vcDelImageRoutes = dataForm.vcDelImageRoutes;
                    //if (vcDelImageRoutes.Length > 0)
                    //{
                    //    if (vcDelImageRoutes.LastIndexOf(",") > 0)
                    //    {
                    //        string[] imsges = vcDelImageRoutes.Split(",");
                    //        for (int i = 0; i < imsges.Length; i++)
                    //        {
                    //            String realPath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Image" + Path.DirectorySeparatorChar + "HeZiImages";
                    //            if (System.IO.File.Exists(realPath + imsges[i]))
                    //            {
                    //                System.IO.File.Delete(realPath + imsges[i]);
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        String realPath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Image" + Path.DirectorySeparatorChar + "HeZiImages";
                    //        if (System.IO.File.Exists(realPath + vcDelImageRoutes))
                    //        {
                    //            System.IO.File.Delete(realPath + vcDelImageRoutes);
                    //        }
                    //    }
                        
                    //}
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "保存成功！";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M05UE0305", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
        #region 保存文件

        public void SaveFile(string filePath)
        {
            try
            {

                DirectoryInfo theFolder = new DirectoryInfo(filePath);
                string environment = Environment.OSVersion.ToString().ToLower();
                //Console.WriteLine("进入保存方法");
                if (!environment.Contains("windows"))
                {
                    //Console.WriteLine("linux");
                    foreach (FileInfo info in theFolder.GetFiles())
                    {
                        //Console.WriteLine("linux正式保存");
                        ComFunction.HttpUploadFile(info.FullName, info.Name, @"Doc\Image\HeZiImages\");
                        //Console.WriteLine("linux结束保存");
                    }
                }
                else
                {
                    //Console.WriteLine("windows");
                    //转存下载
                    foreach (FileInfo info in theFolder.GetFiles())
                    {
                        //Console.WriteLine("windows正式保存");
                        string realPath = _webHostEnvironment.ContentRootPath + @"\Doc\Image\HeZiImages\"  + info.Name;
                        System.IO.File.Copy(info.FullName, realPath, true);
                        //Console.WriteLine("windows正式保存");
                    }

                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex);
                throw ex;
            }
        }

        #endregion
        [HttpPost]
        [EnableCors("any")]
        public IActionResult getBaseApi1(string path)
        {

            //string strPath1 = "D:\\CDIST接口数据备份_G\\FP0033.jpg";
            var width = 0;
            string appPath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Images";
            var errorImage = appPath + "404.png";//没有找到图片
            var imgPath = string.IsNullOrEmpty(path) ? errorImage : appPath + path;
            //获取图片的返回类型
            var contentTypDict = new Dictionary<string, string> {
                {"jpg","image/jpeg"},
                {"jpeg","image/jpeg"},
                {"jpe","image/jpeg"},
                {"png","image/png"},
                {"gif","image/gif"},
                {"ico","image/x-ico"},
                {"tif","image/tiff"},
                {"tiff","image/tiff"},
                {"fax","image/fax"},
                {"wbmp","image//vnd.wap.wbmp"},
                {"rp","image/vnd.rn-realpix"}
            };
            var contentTypeStr = "image/jpeg";
            var imgTypeSplit = path.Split('.');
            var imgType = imgTypeSplit[imgTypeSplit.Length - 1].ToLower();
            //未知的图片类型
            if (!contentTypDict.ContainsKey(imgType))
            {
                imgPath = errorImage;
            }
            else
            {
                contentTypeStr = contentTypDict[imgType];
            }
            //图片不存在
            if (!new FileInfo(imgPath).Exists)
            {
                imgPath = errorImage;
            }
            //原图
            if (width <= 0)
            {
                using (var sw = new FileStream(imgPath, FileMode.Open,FileAccess.ReadWrite))
                {
                    var bytes = new byte[sw.Length];
                    sw.Read(bytes, 0, bytes.Length);
                    sw.Close();
                    return new FileContentResult(bytes, contentTypeStr);
                }
            }
            //缩小图片
            using (var imgBmp = new Bitmap(imgPath))
            {
                //找到新尺寸
                var oWidth = imgBmp.Width;
                var oHeight = imgBmp.Height;
                var height = oHeight;
                if (width > oWidth)
                {
                    width = oWidth;
                }
                else
                {
                    height = width * oHeight / oWidth;
                }
                var newImg = new Bitmap(imgBmp, width, height);
                newImg.SetResolution(72, 72);
                var ms = new MemoryStream();
                newImg.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                var bytes = ms.GetBuffer();
                ms.Close();
                return new FileContentResult(bytes, contentTypeStr);
            }

        }

        [HttpGet]
        [EnableCors("any")]
        public IActionResult getBaseApi(string path)
        {

            try
            {
                Console.WriteLine("111");
                string fileSavePath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Images";//文件临时目录，导入完成后 删除
                var provider = new FileExtensionContentTypeProvider();
                FileInfo fileInfo = new FileInfo(fileSavePath + path);
                var ext = fileInfo.Extension;
                new FileExtensionContentTypeProvider().Mappings.TryGetValue(ext, out var contenttype);
                byte[] bt = System.IO.File.ReadAllBytes(fileSavePath + path);
                if (fileInfo.Exists)
                    fileInfo.Delete();
                return File(bt, contenttype ?? "image/Jpeg", fileInfo.Name);
            }
            catch (Exception ex)
            {
                ContentResult result = new ContentResult();
                result.Content = "<script>alert('导出失败,没有找到要导出的文件！')</script>";
                result.ContentType = "text/html;charset=utf-8";
                ComMessage.GetInstance().ProcessMessage("download", "M00UE0007", ex, "system");
                return result;
            }

        }

        [HttpPost]
        [EnableCors("any")]
        public string replyApi([FromBody] dynamic data)
        {
            //验证是否登录
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            ApiResult apiResult = new ApiResult();
            try
            {
                //以下开始业务处理
                //以下开始业务处理

                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                JArray listInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();

                if (listInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请先选中数据，再进行回复操作！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                DataTable dataTable = fs0503_Logic.createTable("fs0503");
                bool bReault = true;
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (listInfoData[i]["vcState"].ToString() != "待回复" && listInfoData[i]["vcState"].ToString() != "退回"&& listInfoData[i]["vcState"].ToString() != "待回复(已填写)" && listInfoData[i]["vcState"].ToString() != "退回(已填写)")
                    {
                        DataRow dataRow = dataTable.NewRow();
                        dataRow["vcPartNo"] = listInfoData[i]["vcPartNo"].ToString();
                        dataRow["vcMessage"] = "状态不正确,必须是待回复(已填写)或退回(已填写)，才能进行回复操作！";
                        dataTable.Rows.Add(dataRow);
                        bReault = false;
                        //apiResult.code = ComConstant.ERROR_CODE;
                        //apiResult.data = listInfoData[i]["vcPartNo"] + "状态不正确,必须是待回复(已填写)或退回(已填写)，才能进行回复操作！";
                        //return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    string vcImageRoutes = listInfoData[i]["vcImageRoutes"] == null ? "" : listInfoData[i]["vcImageRoutes"].ToString();
                    if (vcImageRoutes.Length==0)
                    {
                        DataRow dataRow = dataTable.NewRow();
                        dataRow["vcPartNo"] = listInfoData[i]["vcPartNo"].ToString();
                        dataRow["vcMessage"] = "尚未编辑上传图片,不能回复！";
                        dataTable.Rows.Add(dataRow);
                        bReault = false;
                        //apiResult.code = ComConstant.ERROR_CODE;
                        //apiResult.data = listInfoData[i]["vcPartNo"] + "尚未编辑上传图片,不能回复！";
                        //return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    string vcWorkArea = listInfoData[i]["vcWorkArea"] == null ? "" : listInfoData[i]["vcWorkArea"].ToString();
                    if (vcWorkArea.Length == 0)
                    {
                        DataRow dataRow = dataTable.NewRow();
                        dataRow["vcPartNo"] = listInfoData[i]["vcPartNo"].ToString();
                        dataRow["vcMessage"] = "工区不能为空！";
                        dataTable.Rows.Add(dataRow);
                        bReault = false;
                    }
                    string vcIntake = listInfoData[i]["vcIntake"] == null ? "" : listInfoData[i]["vcIntake"].ToString();
                    if (vcIntake.Length == 0)
                    {
                        DataRow dataRow = dataTable.NewRow();
                        dataRow["vcPartNo"] = listInfoData[i]["vcPartNo"].ToString();
                        dataRow["vcMessage"] = "收容数不能为空！";
                        dataTable.Rows.Add(dataRow);
                        bReault = false;
                    }
                    string vcBoxMaxIntake = listInfoData[i]["vcBoxMaxIntake"] == null ? "" : listInfoData[i]["vcBoxMaxIntake"].ToString();
                    if (vcBoxMaxIntake.Length == 0)
                    {
                        DataRow dataRow = dataTable.NewRow();
                        dataRow["vcPartNo"] = listInfoData[i]["vcPartNo"].ToString();
                        dataRow["vcMessage"] = "箱最大收容数不能为空！";
                        dataTable.Rows.Add(dataRow);
                        bReault = false;
                    }
                    string vcBoxType = listInfoData[i]["vcBoxType"] == null ? "" : listInfoData[i]["vcBoxType"].ToString();
                    if (vcBoxType.Length == 0)
                    {
                        DataRow dataRow = dataTable.NewRow();
                        dataRow["vcPartNo"] = listInfoData[i]["vcPartNo"].ToString();
                        dataRow["vcMessage"] = "箱种不能为空！";
                        dataTable.Rows.Add(dataRow);
                        bReault = false;
                    }
                    string vcLength = listInfoData[i]["vcLength"] == null ? "" : listInfoData[i]["vcLength"].ToString();
                    if (vcLength.Length == 0)
                    {
                        DataRow dataRow = dataTable.NewRow();
                        dataRow["vcPartNo"] = listInfoData[i]["vcPartNo"].ToString();
                        dataRow["vcMessage"] = "长(mm)不能为空！";
                        dataTable.Rows.Add(dataRow);
                        bReault = false;
                    }
                    string vcWide = listInfoData[i]["vcWide"] == null ? "" : listInfoData[i]["vcWide"].ToString();
                    if (vcWide.Length == 0)
                    {
                        DataRow dataRow = dataTable.NewRow();
                        dataRow["vcPartNo"] = listInfoData[i]["vcPartNo"].ToString();
                        dataRow["vcMessage"] = "宽(mm)不能为空！";
                        dataTable.Rows.Add(dataRow);
                        bReault = false;
                    }
                    string vcHeight = listInfoData[i]["vcHeight"] == null ? "" : listInfoData[i]["vcHeight"].ToString();
                    if (vcHeight.Length == 0)
                    {
                        DataRow dataRow = dataTable.NewRow();
                        dataRow["vcPartNo"] = listInfoData[i]["vcPartNo"].ToString();
                        dataRow["vcMessage"] = "高(mm)不能为空！";
                        dataTable.Rows.Add(dataRow);
                        bReault = false;
                    }
                    string vcEmptyWeight = listInfoData[i]["vcEmptyWeight"] == null ? "" : listInfoData[i]["vcEmptyWeight"].ToString();
                    if (vcEmptyWeight.Length == 0)
                    {
                        DataRow dataRow = dataTable.NewRow();
                        dataRow["vcPartNo"] = listInfoData[i]["vcPartNo"].ToString();
                        dataRow["vcMessage"] = "空箱重量(g)不能为空！";
                        dataTable.Rows.Add(dataRow);
                        bReault = false;
                    }
                    string vcUnitNetWeight = listInfoData[i]["vcUnitNetWeight"] == null ? "" : listInfoData[i]["vcUnitNetWeight"].ToString();
                    if (vcUnitNetWeight.Length == 0)
                    {
                        DataRow dataRow = dataTable.NewRow();
                        dataRow["vcPartNo"] = listInfoData[i]["vcPartNo"].ToString();
                        dataRow["vcMessage"] = "单品净重(g)不能为空！";
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
                fs0503_Logic.reply(listInfoData, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M05UE0306", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "回复失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }


        /// <summary>
        /// 导出消息信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string exportmessageApi([FromBody] dynamic data)
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
            try
            {
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                List<Dictionary<string, Object>> listInfoData = dataForm.ToObject<List<Dictionary<string, Object>>>();
                //DataTable dataTable = fs0603_Logic.createTable("MES");
                //FS0404_Logic fs0404_Logic = new FS0404_Logic();
                DataTable dataTable = fs0503_Logic.createTable("fs0503");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    DataRow dataRow = dataTable.NewRow();
                    dataRow["vcPartNo"] = listInfoData[i]["vcPartNo"].ToString();
                    dataRow["vcMessage"] = listInfoData[i]["vcMessage"].ToString();
                    dataTable.Rows.Add(dataRow);
                }

                string[] fields = { "vcPartNo", "vcMessage" };
                string filepath = ComFunction.generateExcelWithXlt(dataTable, fields, _webHostEnvironment.ContentRootPath, "FS0503_MessageList.xlsx", 1, loginInfo.UserId, FunctionID);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M05UE0307", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出错误列表失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
    }
}

