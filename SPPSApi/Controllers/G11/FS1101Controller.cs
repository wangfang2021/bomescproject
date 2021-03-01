using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
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

namespace SPPSApi.Controllers.G11
{
    [Route("api/FS1101/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1101Controller : BaseController
    {
        FS0603_Logic fS0603_Logic = new FS0603_Logic();
        FS1101_Logic fS1101_Logic = new FS1101_Logic();
        private readonly string FunctionID = "FS1101";

        private readonly IWebHostEnvironment _webHostEnvironment;
        public FS1101Controller(IWebHostEnvironment webHostEnvironment)
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
                //
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

            string strPackMaterNo = dataForm.PackMaterNo == null ? "" : dataForm.PackMaterNo;
            string strTrolleyNo = dataForm.TrolleyNo == null ? "" : dataForm.TrolleyNo;
            string strPartId = dataForm.PartId == null ? "" : dataForm.PartId;
            string strOrderNo = dataForm.OrderNo == null ? "" : dataForm.OrderNo;
            string strLianFan = dataForm.LianFan == null ? "" : dataForm.LianFan;
            try
            {
                DataTable dataTable = fS1101_Logic.getSearchInfo(strPackMaterNo, strTrolleyNo, strPartId, strOrderNo, strLianFan);
                DtConverter dtConverter = new DtConverter();
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
        /// 印刷方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string printApi([FromBody]dynamic data)
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
                DataTable dtMessage = fS0603_Logic.createTable("MES");
                if (listInfoData.Count != 0)
                {
                    for (int i = 0; i < listInfoData.Count; i++)
                    {
                        string strLind = listInfoData[i]["LinId"] == null ? "" : listInfoData[i]["LinId"].ToString();
                        string strPackMaterNo = listInfoData[i]["vcPackMaterNo"] == null ? "" : listInfoData[i]["vcPackMaterNo"].ToString();
                        if (strPackMaterNo == "")
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = string.Format("断取指示书号{0}为空，无法打印", strPackMaterNo);
                            dtMessage.Rows.Add(dataRow);
                        }
                    }
                        if (dtMessage.Rows.Count != 0)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.type = "list";
                            apiResult.data = dtMessage;
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        bool bResult = fS1101_Logic.getPrintInfo(listInfoData, loginInfo.UserId, ref dtMessage);
                        if (!bResult)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.type = "list";
                            apiResult.data = dtMessage;
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }

                        string strPrintName = "";//打印机
                        string strReportName = "fs1101_exl";//Excel报表模板
                        string strPrintData = "";//数据表
                        if (!bResult)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.type = "list";
                            apiResult.data = dtMessage;
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        apiResult.code = ComConstant.SUCCESS_CODE;
                        apiResult.data = "打印成功";

                        ////调用WEBSERVICE接口打印
                        //bool bResult = true;
                        //if(!bResult)
                        //{
                        //    DataRow dataRow = dtMessage.NewRow();
                        //    dataRow["vcMessage"] = string.Format("断取指示书号{0}打印失败", strPackMaterNo);
                        //    dtMessage.Rows.Add(dataRow);
                        //}else
                        //{
                        //    DataRow dataRow = dtMessage.NewRow();
                        //    dataRow["vcMessage"] = string.Format("断取指示书号{0}打印成功", strPackMaterNo);
                        //    dtMessage.Rows.Add(dataRow);
                        //}
                    
                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "未选择有效的打印数据";
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
    }
}
