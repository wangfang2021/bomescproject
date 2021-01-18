using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SPPSApi.Controllers.G04
{
    [Route("api/FS0404/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0404Controller : BaseController
    {
        FS0404_Logic fs0404_Logic = new FS0404_Logic();
        private readonly string FunctionID = "FS0404";

        private readonly IWebHostEnvironment _webHostEnvironment;


        public FS0404Controller(IWebHostEnvironment webHostEnvironment)
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

                List<Object> dataList_C045 = ComFunction.convertAllToResult(ComFunction.getTCode("C045"));//荷姿状态
                List<Object> dataList_C046 = ComFunction.convertAllToResult(ComFunction.getTCode("C046"));//荷姿状态

                res.Add("C045", dataList_C045);
                res.Add("C046", dataList_C046);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M04UE0401", ex, loginInfo.UserId);
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
            string vcOrderState = dataForm.vcOrderState == null ? "" : dataForm.vcOrderState;
            string vcOrderNo = dataForm.vcOrderNo == null ? "" : dataForm.vcOrderNo;
            string dTargetDate = dataForm.dTargetDate == null ? "" : dataForm.dTargetDate;
            string vcOrderType = dataForm.vcOrderType == null ? "" : dataForm.vcOrderType;
            try
            {
                DataTable dt = fs0404_Logic.Search(vcOrderState, vcOrderNo, dTargetDate, vcOrderType,loginInfo.UserId);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("dUploadDate", ConvertFieldType.DateType, "yyyy-MM-dd HH:mm:ss");
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M04UE0402", ex, loginInfo.UserId);
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
            string vcOrderState = dataForm.vcOrderState == null ? "" : dataForm.vcOrderState;
            string vcOrderNo = dataForm.vcOrderNo == null ? "" : dataForm.vcOrderNo;
            string dTargetDate = dataForm.dTargetDate == null ? "" : dataForm.dTargetDate;
            string vcOrderType = dataForm.vcOrderType == null ? "" : dataForm.vcOrderType;
            try
            {
                DataTable dt = fs0404_Logic.Search(vcOrderState, vcOrderNo, dTargetDate, vcOrderType, loginInfo.UserId);
                string[] head = new string[] { };
                string[] field = new string[] { };
                //[vcOrderNo], [dTargetDate], [vcOrderType] ,vcOrderState,[dUploadDate],[vcMemo]
                head = new string[] { "订单编号", "对象年月日", "订单类型", "状态", "备注", "上传时间" };

                field = new string[] { "vcOrderNo", "dTargetDate", "vcOrderType", "vcOrderState", "vcMemo", "dUploadDate" };
                string msg = string.Empty;
                //string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0309_Export.xlsx", 2, loginInfo.UserId, FunctionID);
                string filepath = ComFunction.DataTableToExcel(head, field, dt, ".", loginInfo.UserId, FunctionID, ref msg);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M04UE0403", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 上传确定
        [HttpPost]
        [EnableCors("any")]
        public string updateOKApi([FromBody] dynamic data)
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
            string vcOrderType = dataForm.vcOrderType == null ? "" : dataForm.vcOrderType;
            string dTargetDate = dataForm.dTargetDate == null ? "" : dataForm.dTargetDate;
            string lastOrderNo = dataForm.lastOrdderNo == null ? "" : dataForm.lastOrdderNo;
            string newOrderNo = dataForm.newOrderNo == null ? "" : dataForm.newOrderNo;
            string vcMemo = dataForm.vcMemo == null ? "" : dataForm.vcMemo;
            string dTargetWeek = dataForm.dTargetWeek == null ? "" : dataForm.dTargetWeek;
            List<Dictionary<string, Object>> fileList = dataForm.file.ToObject<List<Dictionary<string, Object>>>();
            List<Dictionary<string, Object>> fileDelList = dataForm.fileDel.ToObject<List<Dictionary<string, Object>>>();
            try
            {
                if (vcOrderType == "0")
                {
                    dTargetDate = dTargetDate.Replace("-", "");
                } else if (vcOrderType == "1")
                {
                    dTargetDate = dTargetDate.Replace("-", "").Substring(0,4);
                }
                else if (vcOrderType == "2") {
                    dTargetDate = dTargetDate.Replace("-", "").Substring(0, 6);
                } else
                { 
                
                }
                    #region 判断
                    #endregion
                if (lastOrderNo.Length >0)
                {
                    if (newOrderNo.Length==0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "请确认是否在修正订单，修正订单必须输入原订单号和修正订单号,请确认！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                //订单类型	0	日度
                //订单类型	1	周度
                //订单类型	2	月度
                //订单类型	3	紧急
                #region
                
                if (vcOrderType=="0")
                {
                    if (dTargetDate.Length==0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "订单类型为日度时，对象年月(日)不能为空！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (fileList.Count>1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "订单类型为日度时，每次仅能上传一个文件,请确认！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    for (int i = 0; i < fileList.Count; i++)
                    {
                        if (fileList[i]["fileName"].ToString().Substring(0, 2).ToUpper() != "RD")
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "文件名不匹配日度订单命名规则，请以RD开始命名 '";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                    }
                }
                if (vcOrderType == "1")
                {
                    if (dTargetDate.Length == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "订单类型为周度时，对象周的对象年月不能为空！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (dTargetWeek.Length == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "订单类型为周度时，对象周不能为空！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (fileList.Count > 1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "订单类型为周度时，每次仅能上传一个文件,请确认！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    for (int i = 0; i < fileList.Count; i++)
                    {
                        if (fileList[i]["fileName"].ToString().Substring(0, 2).ToUpper() != "ZD")
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "文件名不匹配周度订单命名规则，请以ZD开始命名 '";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                    }
                }
                if (vcOrderType == "2")
                {
                    if (dTargetDate.Length == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "订单类型为月度时，对象年月(日)不能为空！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (lastOrderNo.Length > 0 && newOrderNo.Length > 0)
                    {
                        if (fileList.Count != 1)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "订单类型为月度时，修正订单每次仅上传一个文件,请确认！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                    }
                    else {
                        if (fileList.Count != 2)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "订单类型为月度时，每次仅必须上传两个文件,请确认！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                    }
                    
                    for (int i = 0; i < fileList.Count; i++)
                    {
                        if (fileList[i]["fileName"].ToString().Substring(0, 2).ToUpper() != "YD")
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "文件名不匹配月度订单命名规则，请以YD开始命名 '";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                    }
                }
                if (vcOrderType == "3")
                {
                    if (fileList.Count > 1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "订单类型为紧急时，每次仅能上传一个文件,请确认！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    for (int i = 0; i < fileList.Count; i++)
                    {
                        if (fileList[i]["fileName"].ToString().Substring(0, 2).ToUpper() != "JJ")
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "文件名不匹配紧急订单命名规则，请以JJ开始命名 '";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                    }
                }
                #endregion
                //判断是修正 还是新增
                if (lastOrderNo.Length > 0 && newOrderNo.Length > 0)
                {
                    //判定原订单号是否存在
                    DataTable dt = fs0404_Logic.isCheckByOrderNo(lastOrderNo);
                    if (dt.Rows.Count==0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "请确认是否在修正订单，修正订单的原订单号不存在,请确认！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
           
                    fs0404_Logic.updateBylastOrderNo(vcOrderType, dTargetDate, dTargetWeek, lastOrderNo, newOrderNo, vcMemo, fileList,loginInfo.UserId);
                    String realPath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "orders";
                    if (System.IO.File.Exists(realPath + dt.Rows[0]["vcFilePath"]))
                    {
                        System.IO.File.Delete(realPath + dt.Rows[0]["vcFilePath"]);
                    }
                }
                else
                {
                    fs0404_Logic.addOrderNo(vcOrderType, dTargetDate, dTargetWeek, lastOrderNo, newOrderNo, vcMemo, fileList, loginInfo.UserId);
                }

                //删除误操作的上传订单
                if (fileDelList.Count > 0)
                {
                    for (int i = 0; i < fileDelList.Count; i++) {
                        String realPath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "orders";
                        string filePath = fileDelList[i]["filePath"].ToString();
                        if(filePath.Length>0)
                        {
                            if (System.IO.File.Exists(realPath + filePath))
                            {
                                System.IO.File.Delete(realPath + filePath);
                            }
                        }
                    }
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M04UE0404", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "上传订单失败！";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

    }
}

