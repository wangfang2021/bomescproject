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

namespace SPPSApi.Controllers.G14
{
    [Route("api/FS1402/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1402Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        FS1402_Logic fS1402_Logic = new FS1402_Logic();
        FS0603_Logic fs0603_Logic = new FS0603_Logic();
        private readonly string FunctionID = "FS1402";

        public FS1402Controller(IWebHostEnvironment webHostEnvironment)
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

            string strCheckType = dataForm.CheckType == null ? "" : dataForm.CheckType;
            string strPartId = dataForm.PartId == null ? "" : dataForm.PartId;
            string strSupplierId = dataForm.SupplierId == null ? "" : dataForm.SupplierId;
            string strSupplierPlant = dataForm.SupplierPlant == null ? "" : dataForm.SupplierPlant;
            try
            {
                DataTable dataTable = fS1402_Logic.getSearchInfo(strCheckType, strPartId, strSupplierId, strSupplierPlant);
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

            string strCheckType = dataForm.CheckType == null ? "" : dataForm.CheckType;
            string strPartId = dataForm.PartId == null ? "" : dataForm.PartId;
            string strSupplierId = dataForm.SupplierId == null ? "" : dataForm.SupplierId;
            string strSupplierPlant = dataForm.SupplierPlant == null ? "" : dataForm.SupplierPlant;
            try
            {
                DataTable dataTable = fS1402_Logic.getSearchInfo(strCheckType, strPartId, strSupplierId, strSupplierPlant);
                string[] fields = { "LinId", "vcPartId", "dFromTime", "dToTime", "vcCarfamilyCode", "vcSupplierId", "vcSupplierPlant", "vcCheckP", "vcChangeRea", "vcTJSX", "vcOperator", "dOperatorTime" };
                string filepath = ComFunction.generateExcelWithXlt(dataTable, fields, _webHostEnvironment.ContentRootPath, "FS1402_Export.xlsx", 1, loginInfo.UserId, FunctionID);
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
            JArray listMultiple = dataForm.multipleSelection;
            List<Dictionary<string, Object>> listMultipleData = listMultiple.ToObject<List<Dictionary<string, Object>>>();
            try
            {
                string strPartId = listMultipleData[0]["vcPartId"] == null ? "" : listMultipleData[0]["vcPartId"].ToString();
                string strFromTime = listMultipleData[0]["dFromTime"] == null ? "" : listMultipleData[0]["dFromTime"].ToString();
                string strToTime = listMultipleData[0]["dToTime"] == null ? "" : listMultipleData[0]["dToTime"].ToString();
                string strSupplierId = listMultipleData[0]["vcSupplierId"] == null ? "" : listMultipleData[0]["vcSupplierId"].ToString();
                string strSupplierPlant = listMultipleData[0]["vcSupplierPlant"] == null ? "" : listMultipleData[0]["vcSupplierPlant"].ToString();
                string strCheckP = listMultipleData[0]["vcCheckP"] == null ? "" : listMultipleData[0]["vcCheckP"].ToString();
                string strChangeRea = listMultipleData[0]["vcChangeRea"] == null ? "" : listMultipleData[0]["vcChangeRea"].ToString();
                string strTJSX = listMultipleData[0]["vcTJSX"] == null ? "" : listMultipleData[0]["vcTJSX"].ToString();

                res.Add("modelItem", "mod");
                res.Add("PartIdItem", strPartId);
                res.Add("SupplierIdItem", strSupplierId);
                res.Add("SupplierPlantItem", strSupplierPlant);
                res.Add("FromTimeItem", strFromTime);
                res.Add("ToTimeItem", strToTime);
                res.Add("CheckTypeItem", strCheckP);
                res.Add("TeJiItem", strTJSX);
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
        /// 保存方法
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
                string strtype = dataForm.type;
                string strmodel = dataForm.model;
                string strPartId = dataForm.PartId;
                string strSupplierId = dataForm.SupplierId;
                string strSupplierPlant = dataForm.SupplierPlant;
                string strFromTime = dataForm.FromTime;
                string strToTime = dataForm.ToTime;
                string strCheckType = dataForm.CheckType;
                string strTeJi = dataForm.TeJi;
                string strChangeReason = dataForm.ChangeReason;
                DataTable dtMessage = fs0603_Logic.createTable("MES");
                bool bReault = true;
                DataTable dtImport = fS1402_Logic.checkSaveInfo(dataForm, ref bReault, ref dtMessage);
                if(!bReault)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "数据读取失败";
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
                bool bResult = true;
                //bool bResult = fS1402_Logic.setDataInfo(strType, strInfo, strPartNo, strFromTime, strToTime, strSupplierCode, strSupplierPlant, strCarFamilyCode, strCheckQf, strTeJi, strChangeReason, loginInfo.UserId, ref strMsg);
                if (!bResult)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = null;
                    //apiResult.data = strMsg.ToString();
                }
                else
                {
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.data = "保存成功";
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
                string strMsg = "";
                DirectoryInfo theFolder = new DirectoryInfo(fileSavePath);
                string[,] headers = new string[,] {{"品番","使用开始","使用结束","供应商编码", "供应商工区", "车种代码","检查区分","变更理由","特记事项"},
                                                {"vcPartId", "vcTimeFrom", "vcTimeTo", "vcSupplierCode","vcSupplierPlant","vcCarfamilyCode","vcCheckP","vcChangeRea","vcTJSX"},
                                                {FieldCheck.NumCharLLL,FieldCheck.Date,FieldCheck.Date,"","","","","",""},
                                                {"12","10","10","5", "1", "0", "0", "0", "0"},//最大长度设定,不校验最大长度用0
                                                {"12","10","10","0", "0", "0", "0", "0", "0"}};//最小长度设定,可以为空用0

                bool bResult = fS1402_Logic.ImportFile(theFolder, fileSavePath, "sheet1", headers, true, loginInfo.UserId, ref strMsg);
                if (!bResult)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = strMsg.ToString();
                }
                else
                {
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.data = "保存成功";
                }
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


    }
}
