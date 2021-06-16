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
    [Route("api/FS0705/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0705Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0705_Logic fs0705_Logic = new FS0705_Logic();
        private readonly string FunctionID = "FS0705";

        public FS0705Controller(IWebHostEnvironment webHostEnvironment)
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

                bool IsUserDisabled = false;

                #region 获取当前用户能否使用计算过程检索和调整数据输入按钮
                IsUserDisabled = fs0705_Logic.getUserDisable(loginInfo.UserId);
                #endregion

                res.Add("btnUserDisabled", IsUserDisabled);

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

        #region 发注便次更新
        [HttpPost]
        [EnableCors("any")]
        public string searchFaZhuTimeApi([FromBody] dynamic data)
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
            string PackSpot = loginInfo.BaoZhuangPlace;
            try
            {
                ArrayList list = fs0705_Logic.SearchFaZhuTime(PackSpot);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = list;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M07UE0501", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "发注便次更新失败:"+ex.Message;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 发注数量计算
        [HttpPost]
        [EnableCors("any")]
        public string computeApi([FromBody] dynamic data)
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
            Dictionary<string, object> res = new Dictionary<string, object>();

            try
            {
                JArray listInfo = dataForm.multipleSelection;
                if (listInfo == null || listInfo.Count <= 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请先选择发注便次";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();


                string strRuHeToTime = listInfoData[listInfoData.Count - 1]["dRuheToDate"].ToString();
                string strFaZhuID = listInfoData[listInfoData.Count - 1]["strFaZhuID"].ToString();
                string strBianCi = listInfoData[listInfoData.Count - 1]["strBCName"].ToString();
                string strNaQiDate = listInfoData[listInfoData.Count - 1]["dNaqiToDate"].ToString();

                /*
                 * 添加校验此次补给品番是否维护包材构成
                 * 是否维护：有包材构成数据，并且包材构成数据是有效的(开始时间<=当前时间 and 当前时间<=结束时间)
                 * 数据源：部品入库表，时间段取上次计算结束时间到(当前时间包材自动发注对应的入荷结束时间)
                 * 包材构成表：TPackItem
                 */


                #region 校验当前计算的发注逻辑下是否存在有效包材构成
                /*
                 * 修改时间：2021-5-10
                 * 修改人：董镇
                 * 注：注意包材品番的有效时间
                 */
                DataTable CheckFaZhuIDPackBase =  fs0705_Logic.getFaZhuIdPackCheckDT(strFaZhuID, loginInfo.BaoZhuangPlace);
                if (CheckFaZhuIDPackBase==null || CheckFaZhuIDPackBase.Rows.Count<=0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "无对应该逻辑的包材品番";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                #endregion

                #region 检验是否存在当前计算时间段内有部品品番无有效包材构成，如果有则发邮件提示
                DataTable dt = fs0705_Logic.getInvalidPackNo(strFaZhuID, loginInfo.BaoZhuangPlace, strRuHeToTime);
                if (dt!=null && dt.Rows.Count>0)
                {
                    #region 获取发件人名称、发件人邮箱、邮件主题、邮件模板、收件人DT、生成EXCEL附件，并记录错误信息
                    DataTable EmailInformation = fs0705_Logic.getEmailInformation();

                    string strErrMsg = "";
                    string strUserName = "";
                    string strUserEmail = "";
                    string strSubject = "";
                    string strEmailBody = "";
                    DataTable receiverDT = new DataTable();
                    string fileName = "";
                    if (EmailInformation!=null && EmailInformation.Rows.Count>0)
                    {
                        
                        strUserName = EmailInformation.Rows[0]["strUserName"] != null ? EmailInformation.Rows[0]["strUserName"].ToString() : "";
                        if (string.IsNullOrEmpty(strUserName))
                        {
                            strErrMsg += "发件人名称获取失败 ";
                        }
                        strUserEmail = EmailInformation.Rows[0]["strUserEmail"] != null ? EmailInformation.Rows[0]["strUserEmail"].ToString() : "";
                        if (string.IsNullOrEmpty(strUserEmail))
                        {
                            strErrMsg += "发件人邮箱获取失败 ";
                        }
                        strSubject = EmailInformation.Rows[0]["strSubject"] != null ? EmailInformation.Rows[0]["strSubject"].ToString() : "";
                        if (string.IsNullOrEmpty(strSubject))
                        {
                            strErrMsg += "邮件主题获取失败 ";
                        }
                        strEmailBody = EmailInformation.Rows[0]["strEmailBody"] != null ? EmailInformation.Rows[0]["strEmailBody"].ToString() : "";
                        if (string.IsNullOrEmpty(strEmailBody))
                        {
                            strErrMsg += "邮件模板获取失败 ";
                        }
                        string address = EmailInformation.Rows[0]["receiverEmail"] != null ? EmailInformation.Rows[0]["receiverEmail"].ToString() : "";
                        if (string.IsNullOrEmpty(address))
                        {
                            strErrMsg += "收件人邮箱获取失败 ";
                        }
                        string displayName = EmailInformation.Rows[0]["receiverName"] != null ? EmailInformation.Rows[0]["receiverName"].ToString() : "";
                        if (string.IsNullOrEmpty(displayName))
                        {
                            strErrMsg += "收件人姓名获取失败 ";
                        }
                        //这里校验邮箱格式是否正确
                        if (!fs0705_Logic.CheckEmailFormat(address, displayName))
                        {
                            strErrMsg += "收件人信息有误 ";
                        }
                        else
                        {
                            receiverDT.Columns.Add("address");
                            receiverDT.Columns.Add("displayName");
                            DataRow dr = receiverDT.NewRow();
                            dr["address"] = EmailInformation.Rows[0]["receiverEmail"] != null ? EmailInformation.Rows[0]["receiverEmail"].ToString().Trim() : "";
                            dr["displayName"] = EmailInformation.Rows[0]["receiverName"] != null ? EmailInformation.Rows[0]["receiverName"].ToString().Trim() : "";
                            receiverDT.Rows.Add(dr);
                        }
                        //生成附件
                        string[] head = { "品番", "数量" };
                        string[] field = { "vcPart_id", "iQuantity" };
                        string strMsg = "";
                        fileName = ComFunction.DataTableToExcel(head, field, dt, _webHostEnvironment.ContentRootPath, loginInfo.UserId, FunctionID, ref strMsg);
                        fileName = _webHostEnvironment.ContentRootPath + "Doc" + Path.DirectorySeparatorChar + "Export" + Path.DirectorySeparatorChar + fileName;
                        if (strMsg != "") //导出文件出错
                        {
                            strErrMsg += "导出生成附件失败：" + strMsg;
                        }
                    }
                    #endregion

                    #region 如果存在错误，则记录错误信息到日志表，无错误则执行发送邮件
                    if (strErrMsg != "")      //有错误,记录错误
                    {
                        MultiExcute me;
                        me = new MultiExcute();
                        System.Data.SqlClient.SqlParameter[] parameters = {
                                    new System.Data.SqlClient.SqlParameter("@vcMessage",SqlDbType.NVarChar),
                                    new System.Data.SqlClient.SqlParameter("@vcException",SqlDbType.NVarChar),
                                    new System.Data.SqlClient.SqlParameter("@vcTrack",SqlDbType.NVarChar)
                                };
                        parameters[0].Value = strErrMsg;
                        parameters[1].Value = "";
                        parameters[2].Value = "";
                        string strSql = "insert into SLog(UUID,vcFunctionID,vcLogType,vcUserID,vcMessage,vcException,vcTrack,dCreateTime) values(newid(),"
                                                                    + "'FS0705',"
                                                                    + "'E','"
                                                                    + loginInfo.UserId + "',"
                                                                    + "@vcMessage,"
                                                                    + "@vcException,"
                                                                    + "@vcTrack,"
                                                                    + "CONVERT(varchar, GETDATE(),120))";
                        me.ExcuteSqlWithStringOper(strSql, parameters);
                    }
                    else
                    {
                        ComFunction.SendEmailInfo(strUserEmail, strUserName, strEmailBody, receiverDT, null, strSubject, fileName, false);
                    }
                    #endregion
                }
                #endregion

                fs0705_Logic.computer(strFaZhuID, loginInfo.UserId, loginInfo.BaoZhuangPlace,strRuHeToTime,strBianCi,strNaQiDate);

                #region 计算完毕检索计算结果
                DataTable computeJGDT = fs0705_Logic.searchComputeJG(loginInfo.BaoZhuangPlace, strBianCi);

                if (computeJGDT.Rows.Count<=0)
                {
                    //计算成功，本次计算订购数量为0
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.flag = 2;
                    apiResult.data = "计算成功，本次计算订购数量为0";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                DtConverter dtConverter = new DtConverter();

                dtConverter.addField("dTimeStr", ConvertFieldType.DateType, "yyyy/MM/dd hh:mm:ss");
                List<Object> dataList = ComFunction.convertAllToResultByConverter(computeJGDT, dtConverter);
                #endregion

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M07UE0502", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "发注数量计算失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 承认订购数量为0的计算结果

        public string admitEmptyDataApi([FromBody] dynamic data)
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
                /*
                 * 查询出最后一次计算结果
                 */
                DataTable JGDT = fs0705_Logic.admitEmptyDataSearch(loginInfo.BaoZhuangPlace);

                if (JGDT == null || JGDT.Rows.Count <= 0)       //为查询到任何数据，可能性：从未计算过
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请先进行计算";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                //承认计算结果为0
                fs0705_Logic.AdmitEmptyData(JGDT, loginInfo.UserId);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M07UE0503", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "生成发注数据失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }

        #endregion

        #region 生成发注数据
        public string sendFaZhuDataApi([FromBody] dynamic data)
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
                //获取最新的订单号
                string strOrderNo = fs0705_Logic.getNewOrderNo();
                
                /*
                 * 查询出最后一次计算结果中订单号为空的数据(订单号为空，说明没有生成发注数据),并且计算出的订购数量大于0(等于0不需要订购),并且，去掉MBC的包材品番
                 */
                DataTable JGDT = fs0705_Logic.SCFZDataSearchComputeJG(loginInfo.BaoZhuangPlace);

                if (JGDT==null||JGDT.Rows.Count<=0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "没有找到需要订购的品番信息"; 
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                //生成发注数据
                fs0705_Logic.SCFZData(JGDT, strOrderNo,loginInfo.UserId);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "生成发注数据成功";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M07UE0503", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "生成发注数据失败";
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

            try
            {
                DataTable dt = fs0705_Logic.exportSearchJG(loginInfo.BaoZhuangPlace);
                if (dt.Rows.Count<=0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "没有找到需要订购的品番信息";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string[] fields = { "vcBianCi", "vcFaZhuID","vcPackNo","vcPackGPSNo","iF_DingGou","vcBianCi","dTimeStr"};
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0705_Export.xlsx", 2, loginInfo.UserId, FunctionID);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M07UE0505", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion


    }
}
