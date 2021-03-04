using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SPPSApi.Controllers.G13
{
    [Route("api/FS1310/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1310Controller : BaseController
    {
        FS1310_Logic fS1310_Logic = new FS1310_Logic();
        FS0603_Logic fS0603_Logic = new FS0603_Logic();
        private readonly string FunctionID = "FS1310";

        private readonly IWebHostEnvironment _webHostEnvironment;
        public FS1310Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        /// <summary>
        /// 页面初始化
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
                List<Object> PackPlantList = ComFunction.convertAllToResult(ComFunction.getTCode("C023"));
                List<Object> PinMuList = ComFunction.convertAllToResult(fS0603_Logic.getCodeInfo("TPMRelation"));//品目
                res.Add("PackPlantList", PackPlantList);
                res.Add("PinMuList", PinMuList);

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
        /// 查询方法
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

            string strPackPlant = dataForm.PackPlant == null ? "" : dataForm.PackPlant;
            string strPinMu = dataForm.PinMu == null ? "" : dataForm.PinMu;
            string strPartId = dataForm.PartId == null ? "" : dataForm.PartId;
            string strOperImage = dataForm.OperImage == null ? "" : dataForm.OperImage;
            try
            {
                DataTable dataTable = fS1310_Logic.getSearchInfo(strPackPlant, strPinMu, strPartId, strOperImage);
                DtConverter dtConverter = new DtConverter();
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

            string strPackPlant = dataForm.PackPlant == null ? "" : dataForm.PackPlant;
            string strPinMu = dataForm.PinMu == null ? "" : dataForm.PinMu;
            string strPartId = dataForm.PartId == null ? "" : dataForm.PartId;
            string strOperImage = dataForm.OperImage == null ? "" : dataForm.OperImage;
            try
            {
                DataTable dataTable = fS1310_Logic.getSearchInfo(strPackPlant, strPinMu, strPartId, strOperImage);
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    dataTable.Rows[i]["LinId"] = (i + 1).ToString();
                    if (dataTable.Rows[i]["vcOperImage"].ToString() == "/暂无图像.jpg")
                        dataTable.Rows[i]["vcOperImage"] = "未导入";
                    else
                        dataTable.Rows[i]["vcOperImage"] = "已导入";
                }
                string[] fields = { "vcPackPlant_name", "vcPartId", "vcPinMu", "vcOperImage", "vcOperator", "vcOperatorTime" };
                string filepath = ComFunction.generateExcelWithXlt(dataTable, fields, _webHostEnvironment.ContentRootPath, "FS1310_Export.xlsx", 1, loginInfo.UserId, FunctionID);
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
        public string importApi([FromBody]dynamic data)
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
                foreach (FileInfo info in theFolder.GetFiles())
                {
                    string strRarPatch = info.FullName;
                }
                string strMsg = "";
                //==================
                //==================
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
        /// 子页面初始化
        /// </summary>
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
            dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
            JArray listInfo = dataForm.multipleSelection;
            List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
            try
            {
                DataTable dtMessage = fS0603_Logic.createTable("MES");
                if (listInfoData.Count != 1)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "请选中一条数据进行编辑，请确认";
                    dtMessage.Rows.Add(dataRow);
                }
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                {
                    //弹出错误dtMessage
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                string strPackPlant = listInfoData[0]["vcPackPlant"] == null ? "" : listInfoData[0]["vcPackPlant"].ToString();
                string strPartId = listInfoData[0]["vcPartId"] == null ? "" : listInfoData[0]["vcPartId"].ToString();
                string strPinMu = listInfoData[0]["vcPinMu"] == null ? "" : listInfoData[0]["vcPinMu"].ToString();
                string strOperImage = listInfoData[0]["vcOperImage"] == null ? "" : listInfoData[0]["vcOperImage"].ToString();
                res.Add("PackPlantItem", strPackPlant);
                res.Add("PartIdItem", strPartId);
                res.Add("PinMuItem", strPinMu);
                res.Add("OperImageItem", strOperImage == "/暂无图像.jpg" ? "" : strOperImage);
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
        /// 保存方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string editApi([FromBody]dynamic data)
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
                string strPinMu = dataForm.PinMu;
                string strPackPlant = dataForm.PackPlant;
                string strOperImage = dataForm.OperImage;
                string strDelImageRoutes = dataForm.DelImageRoutes;
                fS1310_Logic.setPackOperImage(strPartId, strPinMu, strPackPlant, strOperImage, loginInfo.UserId);
                //保存图片
                if (strDelImageRoutes.Length > 0)
                {
                    if (strDelImageRoutes.LastIndexOf(",") > 0)
                    {
                        string[] images = dataForm.DelImageRoutes.Split(",");
                        for (int i = 0; i < images.Length; i++)
                        {
                            String realPath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Image" + Path.DirectorySeparatorChar + "PackingOper";
                            if (System.IO.File.Exists(realPath + images[i]))
                            {
                                System.IO.File.Delete(realPath + images[i]);
                            }
                        }
                    }
                    else
                    {
                        String realPath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Image" + Path.DirectorySeparatorChar + "PackingOper";
                        if (System.IO.File.Exists(realPath + strDelImageRoutes))
                        {
                            System.IO.File.Delete(realPath + strDelImageRoutes);
                        }
                    }
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
        /// <summary>
        /// 删除方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string delApi([FromBody]dynamic data)
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
                if (listInfoData.Count != 0)
                {
                    fS1310_Logic.deleteInfo(listInfoData);
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.data = null;
                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请选择要删除的数据";
                }
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
        /// 解压缩
        /// </summary>
        /// <param name="unRarPatch">解压路径</param>
        /// <param name="rarPatch">压缩包路路径</param>
        /// <param name="rarName">文件名</param>
        //public void unCompressRAR(string unRarPatch, string rarPatch, string rarName)
        //{
        //    try
        //    {
        //        string the_rar;
        //        RegistryKey the_Reg;
        //        object the_Obj;
        //        string the_Info;

        //        the_Reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\WinRAR.exe");
        //        the_Obj = the_Reg.GetValue("");
        //        the_rar = the_Obj.ToString();
        //        the_Reg.Close();

        //        if (Directory.Exists(unRarPatch) == false)
        //        {
        //            Directory.CreateDirectory(unRarPatch);
        //        }
        //        the_Info = "x  -y  " + rarName + " " + unRarPatch;
        //        ProcessStartInfo the_StartInfo = new ProcessStartInfo();
        //        the_StartInfo.FileName = the_rar;
        //        the_StartInfo.Arguments = the_Info;
        //        the_StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        //        the_StartInfo.WorkingDirectory = rarPatch;//获取压缩包路径

        //        Process the_Process = new Process();
        //        the_Process.StartInfo = the_StartInfo;
        //        the_Process.Start();
        //        the_Process.WaitForExit();
        //        the_Process.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


    }
}
