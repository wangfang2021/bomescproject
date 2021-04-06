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
    [Route("api/FS0705_Sub/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0705Controller_Sub : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0705_Logic fs0705_Logic = new FS0705_Logic();
        private readonly string FunctionID = "FS0705";

        public FS0705Controller_Sub(IWebHostEnvironment webHostEnvironment)
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

                DataTable optionC001DT = new DataTable();
                optionC001DT.Columns.Add("vcName");
                optionC001DT.Columns.Add("vcValue");
                DataRow dr = optionC001DT.NewRow();
                dr["vcName"] = "全部";
                dr["vcValue"] = "0";
                optionC001DT.Rows.Add(dr);
                dr = optionC001DT.NewRow();
                dr["vcName"] = "调增";
                dr["vcValue"] = "1";
                optionC001DT.Rows.Add(dr);
                dr = optionC001DT.NewRow();
                dr["vcName"] = "调减";
                dr["vcValue"] = "2";
                optionC001DT.Rows.Add(dr);

                List<Object> dataList_C001 = ComFunction.convertAllToResult(optionC001DT);//调整类型

                res.Add("C001", dataList_C001);

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
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            //以下开始业务处理
            ApiResult apiResult = new ApiResult();
            dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
            string strPackNo = dataForm.vcPackNo;
            string strPackGPSNo = dataForm.vcPackGPSNo;
            string strTimeFrom = dataForm.dTimeFrom;
            string strTimeTo = dataForm.dTimeTo;
            string strType = dataForm.vcType;

            try
            {
                DataTable dt = fs0705_Logic.search_Sub(strPackNo, strPackGPSNo, strTimeFrom, strTimeTo, strType);

                DtConverter dtConverter = new DtConverter();

                dtConverter.addField("selected", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("dTime", ConvertFieldType.DateType, "yyyy/MM/dd HH:mm:ss");

                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M07UE0504", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "计算过程检索失败";
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

            string strPackNo = dataForm.vcPackNo;
            string strPackGPSNo = dataForm.vcPackGPSNo;
            string strTimeFrom = dataForm.dTimeFrom;
            string strTimeTo = dataForm.dTimeTo;
            string strType = dataForm.vcType;

            try
            {
                DataTable dt = fs0705_Logic.search_Sub(strPackNo,strPackGPSNo,strTimeFrom,strTimeTo,strType);
                #region 根据调整类型的Value生成调整类型的Name列，导出使用调整类型Name列
                dt.Columns.Add("vcType_Name");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["vcType"].ToString()=="1")
                    {
                        dt.Rows[i]["vcType_Name"] = "调增";
                    }
                    if (dt.Rows[i]["vcType"].ToString()=="2")
                    {
                        dt.Rows[i]["vcType_Name"] = "调减";
                    }
                }
                #endregion

                string[] fields = { "vcPackNo", "vcPackGPSNo", "iNumber", "vcType_Name", "dTime", "vcReason"};
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0705_Export_Sub.xlsx", 2, loginInfo.UserId, FunctionID);
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

        #region 更新(保存)
        [HttpPost]
        [EnableCors("any")]
        public string saveApi([FromBody] dynamic data)
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
                JArray listInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
                bool hasFind = false;//是否找到需要新增或者修改的数据
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    if (bAddFlag == true)
                    {//新增
                        hasFind = true;
                    }
                }
                if (!hasFind)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少有一个编辑行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                #region 校验输入数据合法性
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (listInfoData[i]["vcPackGPSNo"]==null || listInfoData[i]["vcPackGPSNo"].ToString().Trim()=="")
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "GPS品番不能为空";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (listInfoData[i]["iNumber"]==null  || listInfoData[i]["iNumber"].ToString().Trim()=="")
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "调整数量不能为空";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (listInfoData[i]["vcReason"]==null || listInfoData[i]["vcReason"].ToString().Trim()=="")
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "调整原因不能为空";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                #endregion

                #region 验证数据是否满足保存条件
                DataTable dt = fs0705_Logic.getIsSave(listInfoData);
                string str1 = "";       //记录哪些GPS品番在包材基础数据表中不存在
                string str2 = "";       //记录哪些GPS品番在包材基础数据表中已废止
                string str3 = "";       //记录哪些GPS品番在包材基础数据表中不属于自动发注品番
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (dt.Select("IsGPSNo='"+listInfoData[i]["vcPackGPSNo"] +"'").Length<=0)
                    {
                        str1 += dt.Rows[i]["vcPackGPSNo"].ToString() + " ";
                    }
                    else
                    {
                        DataRow[] drs = dt.Select("dPackFrom<='"+DateTime.Now.ToString()+"' and  dPackTo>='"+DateTime.Now.ToString()+"'");
                        if (drs.Length<=0)
                        {
                            str2 += listInfoData[i]["vcPackGPSNo"].ToString();
                        }
                        else
                        {
                            if (drs[0]["vcReleaseName"]==null || drs[0]["vcReleaseName"].ToString()=="")
                            {
                                str3 += listInfoData[i]["vcPackGPSNo"].ToString();
                            }
                        }
                    }
                }

                if (str1!="")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "保存失败,以下GPS品番"+str1+"在包材基础信息中不存在";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (str2 != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "保存失败,以下GPS品番" + str1 + "在当前时间范围无效";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (str3 != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "保存失败,以下GPS品番" + str1 + "不是自动发注品番";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                #endregion

                fs0705_Logic.Save_Sub(listInfoData, loginInfo.UserId);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M07UE0505", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

    }
}
