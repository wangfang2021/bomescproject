using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Logic;

namespace SPPSApi.Controllers.G06
{
    [Route("api/FS0614/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0614Controller : BaseController
    {
        FS0614_Logic fs0614_logic = new FS0614_Logic();
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string FunctionID = "FS0614";

        public FS0614Controller(IWebHostEnvironment webHostEnvironment)
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

                List<Object> dataList_C044 = ComFunction.convertAllToResult(ComFunction.getTCode("C044")); //收货方
                List<Object> dataList_C045 = ComFunction.convertAllToResult(fs0614_logic.getType()); //原单位

                res.Add("C044", dataList_C044);
                res.Add("C045", dataList_C045);
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
            string orderState = dataForm.orderState == null ? "" : dataForm.orderState;
            string targetYM = dataForm.targetYM == null ? "" : dataForm.targetYM;
            targetYM = targetYM.Replace("-", "");
            string orderNo = dataForm.orderNo == null ? "" : dataForm.orderNo;
            string orderType = dataForm.orderType == null ? "" : dataForm.orderType;
            string dUpload = dataForm.dUpload == null ? "" : dataForm.dUpload;
            string memo = dataForm.memo == null ? "" : dataForm.memo;
            try
            {
                DataTable dt = fs0614_logic.searchApi(orderState, targetYM, orderNo, orderType, dUpload, memo);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("dUploadDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dCreateDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;

                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M02UE0201", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }

        #endregion

        #region 生成订单
        [HttpPost]
        [EnableCors("any")]
        public string createApi([FromBody]dynamic data)
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
                JArray listInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
                bool hasFind = false;//是否找到需要新增或者修改的数据
                if (listInfoData.Count > 0)
                {
                    hasFind = true;
                }
                if (!hasFind)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选中一行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                string refMsg = "";
                //开始数据验证
                if (hasFind)
                {
                    for (int i = 0; i < listInfoData.Count; i++)
                    {
                        if ("周度订单".Equals(listInfoData[i]["vcOrderType"].ToString()))
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.flag = 1;
                            apiResult.data = "周度订单不能进行生成";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }

                        if ("撤销".Equals(listInfoData[i]["vcOrderState"].ToString()))
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.flag = 1;
                            apiResult.data = "存在已撤销的订单不能进行生成操作";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                    }

                    bool flag = fs0614_logic.checkType(listInfoData, ref refMsg);
                    if (!flag)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = refMsg;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    string realPath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" +
                                      Path.DirectorySeparatorChar + "orders";

                    DataTable dtMessage = new DataTable();
                    dtMessage.Columns.Add("vcOrder");
                    dtMessage.Columns.Add("vcPartNo");
                    dtMessage.Columns.Add("vcMessage");
                    bool result = true;
                    bool resflag = fs0614_logic.CreateOrder(listInfoData, realPath, loginInfo.UserId, loginInfo.UnitCode, ref result, ref dtMessage);


                    if (dtMessage.Rows.Count > 0)
                    {
                        //弹出错误dtMessage
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.type = "list";
                        apiResult.data = dtMessage;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }

                }

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0705", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }

        #endregion

        #region 撤销订单
        [HttpPost]
        [EnableCors("any")]
        public string cancelApi([FromBody]dynamic data)
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
                JArray listInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
                bool hasFind = false;//是否找到需要新增或者修改的数据
                if (listInfoData.Count > 0)
                {
                    hasFind = true;
                }
                if (!hasFind)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选中一行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                string refMsg = "";
                //开始数据验证
                if (hasFind)
                {
                    for (int i = 0; i < listInfoData.Count; i++)
                    {
                        if ("已做成".Equals(listInfoData[i]["vcOrderState"].ToString()))
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.flag = 1;
                            apiResult.data = "存在已做成的订单不能进行撤销操作";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                    }

                    fs0614_logic.cancelOrder(listInfoData, loginInfo.UserId);
                }

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0705", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "撤销失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }

        #endregion

        #region 导出

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
            string fileName = dataForm.fileName;

            try
            {
                string realPath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" +
                                  Path.DirectorySeparatorChar + "orders";
                string filepath = fs0614_logic.getPath(fileName);
                if (!string.IsNullOrWhiteSpace(filepath))
                {
                    string fileSavePath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" +
                                          Path.DirectorySeparatorChar + "Export" + Path.DirectorySeparatorChar +
                                          "Order"; //文件临时目录，导入完成后 删除
                    string tmp = fileSavePath + Path.DirectorySeparatorChar;
                    if (Directory.Exists(tmp))
                    {
                        ComFunction.DeleteFolder(tmp);
                    }

                    string filepathTmp = realPath + filepath;
                    string filePathFinally = fileSavePath + filepath;

                    string strPath = Path.GetDirectoryName(filePathFinally);
                    if (!Directory.Exists(strPath))
                    {
                        Directory.CreateDirectory(strPath);
                    }

                    if (System.IO.File.Exists(filepathTmp))
                    {
                        System.IO.File.Copy(filepathTmp, filePathFinally, true);
                        filepath = "Order" + Path.DirectorySeparatorChar + filepath;
                    }
                    else
                    {
                        filepath = "";
                    }
                }
                else
                {
                    filepath = "";
                }

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = filepath;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE1403", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "下载文件失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }

        #endregion

        #region 文件夹方法


        #region 获取文件路径

        public string getPath(string fileName)
        {
            return "";
        }

        #endregion

        #endregion
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
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("vcOrder");
                dataTable.Columns.Add("vcPartNo");
                dataTable.Columns.Add("vcMessage");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    DataRow dataRow = dataTable.NewRow();
                    dataRow["vcOrder"] = listInfoData[i]["vcOrder"].ToString();
                    dataRow["vcPartNo"] = listInfoData[i]["vcPartNo"].ToString();
                    dataRow["vcMessage"] = listInfoData[i]["vcMessage"].ToString();
                    dataTable.Rows.Add(dataRow);
                }
                string[] head = { "订单号", "品番", "错误信息" };
                string[] fields = { "vcOrder", "vcPartNo", "vcMessage" };
                string refMsg = "";
                string filepath = ComFunction.DataTableToExcel(head, fields, dataTable, _webHostEnvironment.ContentRootPath, loginInfo.UserId, FunctionID, ref refMsg);

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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0902", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
    }
}