using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
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
    [Route("api/FS0707/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0707Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0707_Logic FS0707_Logic = new FS0707_Logic();
        private readonly string FunctionID = "FS0707";

        public FS0707Controller(IWebHostEnvironment webHostEnvironment)
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

                List<Object> strPartPlant = new List<object>();
                List<Object> dataList_Project = ComFunction.convertAllToResult(FS0707_Logic.SearchKind(strPartPlant));//工程
                res.Add("optionProject", dataList_Project);


                List<Object> dataList_Kind = ComFunction.convertAllToResult(ComFunction.getTCode("C070"));//计划类别
                res.Add("optionKind", dataList_Kind);


                List<Object> dataList_Supplier = ComFunction.convertAllToResult(FS0707_Logic.SearchSupplier());//供应商
                res.Add("optionSupplier", dataList_Supplier);


                List<Object> dataList_PartPlant = ComFunction.convertAllToResult(FS0707_Logic.SearchPartPlant());//工厂
                res.Add("optionPartPlant", dataList_PartPlant);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M07UE0701", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "初始化失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 工厂下拉选择触发事件
        [HttpPost]
        [EnableCors("any")]
        public string selectApi([FromBody] dynamic data)
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

            List<Object> strPartPlant = new List<object>();
            strPartPlant = dataForm.selectVaule.ToObject<List<Object>>();

            try
            {
                Dictionary<string, object> res = new Dictionary<string, object>();
                List<Object> dataList_Project = ComFunction.convertAllToResult(FS0707_Logic.SearchKind(strPartPlant));//工程
                res.Add("optionProject", dataList_Project);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M07UE0702", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "初始化工厂下拉框失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }


        }

        #endregion

        #region 计算
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
            string strBegin = dataForm.dFromBegin;
            string strEnd = dataForm.dFromEnd;
            string strFromBeginBZ = dataForm.vcFromBeginBZ;
            string strFromEndBZ = dataForm.vcFromEndBZ;


            List<Object> strProject = new List<object>();
            strProject = dataForm.Project.ToObject<List<Object>>();
            string strKind = dataForm.Kind;
            List<Object> OrderState = new List<object>();
            OrderState = dataForm.SupplierCode.ToObject<List<Object>>();
            List<Object> OrderPartPlant = new List<object>();
            OrderPartPlant = dataForm.PartPlant.ToObject<List<Object>>();

            try
            {
                //判断当前时间不能超过31天
                TimeSpan ts1 = new TimeSpan(Convert.ToDateTime(strBegin).Ticks);
                TimeSpan ts2 = new TimeSpan(Convert.ToDateTime(strEnd).Ticks);
                TimeSpan ts = ts1.Subtract(ts2).Duration();
                int datediff = Convert.ToInt32(ts.Days.ToString());
                if (datediff >= 31)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "时间间隔超过31天！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }
                if (strKind == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请选择计划类别！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }
                if (OrderState.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请选择供应商！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                //计算开始生成结果
                #region 计算开始生成结果
                DataTable dt = FS0707_Logic.SearchCalculation(strBegin, strEnd, strProject, strKind, OrderState, OrderPartPlant);




                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //是否在一个月内
                    if (strBegin.Split("-")[1] == strEnd.Split("-")[1])
                    {
                        int count = Convert.ToInt32(strEnd.Split("-")[2]) - Convert.ToInt32(strBegin.Split("-")[2]) - 1;
                        for (int j = 0; j <= count + 1; j++)
                        {
                            if (strFromBeginBZ == "1" && j == 0)
                            {
                                dt.Rows[i]["vcD" + (Convert.ToInt32(strBegin.Split("-")[2]) + j).ToString() + "bFShow"] = "";
                                dt.Rows[i]["vcD" + (Convert.ToInt32(strBegin.Split("-")[2]) + j).ToString() + "yFShow"] = "true";
                            }
                            else if (strFromEndBZ == "0" && j == count + 1)
                            {

                                dt.Rows[i]["vcD" + (Convert.ToInt32(strBegin.Split("-")[2]) + j).ToString() + "bFShow"] = "true";
                                dt.Rows[i]["vcD" + (Convert.ToInt32(strBegin.Split("-")[2]) + j).ToString() + "yFShow"] = "";
                            }
                            else
                            {
                                dt.Rows[i]["vcD" + (Convert.ToInt32(strBegin.Split("-")[2]) + j).ToString() + "bFShow"] = "true";
                                dt.Rows[i]["vcD" + (Convert.ToInt32(strBegin.Split("-")[2]) + j).ToString() + "yFShow"] = "true";

                            }
                        }
                    }
                    else
                    {
                        DateTime dt1 = Convert.ToDateTime(strBegin).AddDays(1 - Convert.ToDateTime(strBegin).Day).AddMonths(1).AddDays(-1);
                        int ss = Convert.ToInt32(dt1.ToString().Split(" ")[0].Split("/")[2]) - Convert.ToInt32(strBegin.Split("-")[2]) - 1;
                        for (int z = 0; z <= ss + 1; z++)
                        {
                            if (strFromBeginBZ == "1" && z == 0)
                            {
                                dt.Rows[i]["vcD" + (Convert.ToInt32(strBegin.Split("-")[2]) + z).ToString() + "bFShow"] = "";
                                dt.Rows[i]["vcD" + (Convert.ToInt32(strBegin.Split("-")[2]) + z).ToString() + "yFShow"] = "true";
                            }
                            else
                            {

                                dt.Rows[i]["vcD" + (Convert.ToInt32(strBegin.Split("-")[2]) + z).ToString() + "bFShow"] = "true";
                                dt.Rows[i]["vcD" + (Convert.ToInt32(strBegin.Split("-")[2]) + z).ToString() + "yFShow"] = "true";

                            }
                        }
                        int cs = Convert.ToInt32(strEnd.Split("-")[2]);
                        for (int r = 0; r < cs; r++)
                        {
                            if (strFromEndBZ == "0" && r == cs - 1)
                            {
                                dt.Rows[i]["vcD" + (1 + r).ToString() + "bTShow"] = "true";
                                dt.Rows[i]["vcD" + (1 + r).ToString() + "yTShow"] = "";
                            }
                            else
                            {


                                dt.Rows[i]["vcD" + (1 + r).ToString() + "bTShow"] = "true";
                                dt.Rows[i]["vcD" + (1 + r).ToString() + "yTShow"] = "true";
                            }
                        }

                    }
                }
                #endregion
                //插入导出表
                string strErrorName = "";

                for (int s = 0; s < dt.Rows.Count; s++)
                {
                    if (string.IsNullOrEmpty(dt.Rows[s]["vcPackNo"].ToString())) {

                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = dt.Rows[s]["vcPartsno"].ToString()+ "没有维护完整！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }


                FS0707_Logic.Save_GS(dt, loginInfo.UserId, ref strErrorName, strBegin, strEnd);

                DataTable dt2 = FS0707_Logic.Search();
                for (int t = 0; t < dt2.Rows.Count; t++)
                {

                    dt2.Rows[t]["vcNum"] = (t + 1).ToString();
                }
                DtConverter dtConverter = new DtConverter();
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt2, dtConverter);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M07UE0703", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "计算失败";
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
            string strBegin = dataForm.dFromBegin;
            string strEnd = dataForm.dFromEnd;
            string strFromBeginBZ = dataForm.vcFromBeginBZ;
            string strFromEndBZ = dataForm.vcFromEndBZ;
            try
            {

                DataTable dt = FS0707_Logic.Search();
                if (dt.Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "没有可导出数据！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }
                for (int t = 0; t < dt.Rows.Count; t++)
                {

                    dt.Rows[t]["vcNum"] = (t + 1).ToString();
                }
                string resMsg = "";
                int count = 0;
                string[] headData = new string[0];
                string[] fieldsData = new string[0];
                if (strBegin.Split("-")[1] == strEnd.Split("-")[1])
                {
                    count = (Convert.ToInt32(strEnd.Split("-")[2]) - Convert.ToInt32(strBegin.Split("-")[2]) + 1) * 2;

                    if (strFromBeginBZ == "1")
                    {
                        count--;
                    }
                    else if (strFromEndBZ == "0")
                    {
                        count--;
                    }
                    headData = new string[count + 5];
                    fieldsData = new string[count + 5];
                    headData[0] = "序号";
                    headData[1] = "补给品番";
                    headData[2] = "GPS品番";
                    headData[3] = "包材厂家";
                    headData[4] = "纳入收容数";
                    fieldsData[0] = "vcNum";
                    fieldsData[1] = "vcPackNo";
                    fieldsData[2] = "vcPackGPSNo";
                    fieldsData[3] = "vcSupplierName";
                    fieldsData[4] = "iRelease";
                    int x = 1;
                    for (int i = 1; i <= 31; i++)
                    {

                        if (dt.Rows[0]["vcD" + i + "bFShow"].ToString() == "true")
                        {
                            headData[4 + x] = i + "日白值F";
                            fieldsData[4 + x] = "vcD" + i + "bF";
                            x++;

                        }
                        if (dt.Rows[0]["vcD" + i + "yFShow"].ToString() == "true")
                        {
                            headData[4 + x] = i + "日夜值F";
                            fieldsData[4 + x] = "vcD" + i + "yF";
                            x++;
                        }
                        if (dt.Rows[0]["vcD" + i + "bTShow"].ToString() == "true")
                        {
                            headData[4 + x] = i + "日白值T";
                            fieldsData[4 + x] = "vcD" + i + "bT";
                            x++;
                        }

                        if (dt.Rows[0]["vcD" + i + "yTShow"].ToString() == "true")
                        {
                            headData[4 + x] = i + "日夜值T";
                            fieldsData[4 + x] = "vcD" + i + "yT";
                            x++;

                        }
                    }


                }
                else
                {

                    DateTime dt1 = Convert.ToDateTime(strBegin).AddDays(1 - Convert.ToDateTime(strBegin).Day).AddMonths(1).AddDays(-1);
                    int ss = (Convert.ToInt32(dt1.ToString().Split(" ")[0].Split("/")[2]) - Convert.ToInt32(strBegin.Split("-")[2]) + 1) * 2;
                    int cs = Convert.ToInt32(strEnd.Split("-")[2]) * 2;
                    if (strFromBeginBZ == "1")
                    {
                        ss--;
                    }
                    else if (strFromEndBZ == "0")
                    {
                        cs--;
                    }
                    headData = new string[ss + 5 + cs];
                    fieldsData = new string[ss + 5 + cs];
                    headData[0] = "序号";
                    headData[1] = "补给品番";
                    headData[2] = "GPS品番";
                    headData[3] = "包材厂家";
                    headData[4] = "纳入收容数";
                    fieldsData[0] = "vcNum";
                    fieldsData[1] = "vcPackNo";
                    fieldsData[2] = "vcPackGPSNo";
                    fieldsData[3] = "vcSupplierName";
                    fieldsData[4] = "iRelease";
                    int x = 1;
                    for (int i = 1; i <= 31; i++)
                    {

                        if (dt.Rows[0]["vcD" + i + "bFShow"].ToString() == "true")
                        {
                            headData[4 + x] = i + "日白值F";
                            fieldsData[4 + x] = "vcD" + i + "bF";
                            x++;

                        }
                        if (dt.Rows[0]["vcD" + i + "yFShow"].ToString() == "true")
                        {
                            headData[4 + x] = i + "日夜值F";
                            fieldsData[4 + x] = "vcD" + i + "yF";
                            x++;
                        }
                        
                    }
                    for (int j = 1; j <= 31; j++) {

                        if (dt.Rows[0]["vcD" + j + "bTShow"].ToString() == "true")
                        {
                            headData[4 + x] = j + "日白值T";
                            fieldsData[4 + x] = "vcD" + j + "bT";
                            x++;
                        }

                        if (dt.Rows[0]["vcD" + j + "yTShow"].ToString() == "true")
                        {
                            headData[4 + x] = j + "日夜值T";
                            fieldsData[4 + x] = "vcD" + j + "yT";
                            x++;

                        }

                    }




                }
                string[] head = headData;
                string[] fields = fieldsData;

                string filepath = ComFunction.DataTableToExcel(head, fields, dt, _webHostEnvironment.ContentRootPath, loginInfo.UserId, "周度内饰导出", ref resMsg);
                if (filepath == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "导出生成文件失败";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                FS0707_Logic.Updateprint();
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = filepath;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M07UE0704", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        //导出列值应该显示名字
        //#region 保存
        //[HttpPost]
        //[EnableCors("any")]
        //public string saveApi([FromBody] dynamic data)
        //{
        //    //验证是否登录
        //    string strToken = Request.Headers["X-Token"];
        //    if (!isLogin(strToken))
        //    {
        //        return error_login();
        //    }
        //    LoginInfo loginInfo = getLoginByToken(strToken);
        //    //以下开始业务处理
        //    ApiResult apiResult = new ApiResult();
        //    try
        //    {
        //        dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
        //        JArray listInfo = dataForm.multipleSelection;
        //        List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
        //        bool hasFind = false;//是否找到需要新增或者修改的数据
        //        for (int i = 0; i < listInfoData.Count; i++)
        //        {
        //            bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
        //            bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
        //            if (bAddFlag == true)
        //            {//新增
        //                hasFind = true;
        //            }
        //            else if (bAddFlag == false && bModFlag == true)
        //            {//修改
        //                hasFind = true;
        //            }
        //            Regex regex = new System.Text.RegularExpressions.Regex("^(-?[0-9]*[.]*[0-9]{0,3})$");
        //            bool b = regex.IsMatch(listInfoData[i]["iRelease"].ToString());
        //            if (!b)
        //            {
        //                apiResult.code = ComConstant.ERROR_CODE;
        //                apiResult.data = "请填写正常的发住收容数格式！";
        //                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //            }
        //        }
        //        if (!hasFind)
        //        {
        //            apiResult.code = ComConstant.ERROR_CODE;
        //            apiResult.data = "最少有一个编辑行！";
        //            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //        }

        //        string strErrorPartId = "";
        //        FS0707_Logic.Save(loginInfo.UserId, ref strErrorPartId);
        //        if (strErrorPartId != "")
        //        {
        //            apiResult.code = ComConstant.ERROR_CODE;
        //            apiResult.data = "保存失败";
        //            apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
        //            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //        }
        //        apiResult.code = ComConstant.SUCCESS_CODE;
        //        apiResult.data = null;
        //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //    }
        //    catch (Exception ex)
        //    {
        //        ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0902", ex, loginInfo.UserId);
        //        apiResult.code = ComConstant.ERROR_CODE;
        //        apiResult.data = "保存失败";
        //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //    }
        //}
        //#endregion

        #region 发送
        [HttpPost]
        [EnableCors("any")]
        public string SendApi([FromBody] dynamic data)
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
                //dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                //JArray listInfo = dataForm.multipleSelection;
                //List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                string strBegin = dataForm.dFromBegin;
                string strEnd = dataForm.dFromEnd;
                string strFromBeginBZ = dataForm.vcFromBeginBZ;
                string strFromEndBZ = dataForm.vcFromEndBZ;


                List<Object> strProject = new List<object>();
                strProject = dataForm.Project.ToObject<List<Object>>();

                string strKind = dataForm.Kind;

                List<Object> OrderState = new List<object>();
                OrderState = dataForm.SupplierCode.ToObject<List<Object>>();
                List<Object> OrderPartPlant = new List<object>();
                OrderPartPlant = dataForm.PartPlant.ToObject<List<Object>>();
                if (OrderState.Count==0) {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请选择供应商！";
                    apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                string strErrorPartId = "";
                OrderState = dataForm.SupplierCode.ToObject<List<Object>>();
                DataTable dt = FS0707_Logic.Search();
                for (int t = 0; t < dt.Rows.Count; t++)
                {

                    dt.Rows[t]["vcNum"] = (t + 1).ToString();
                }
                if (dt.Rows.Count==0) {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请重新计算";
                    apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (dt.Rows[0]["vcIsorNoPrint"].ToString() != "1")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请导出再发送！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                FS0707_Logic.Save(dt,loginInfo.UserId, ref strErrorPartId,strBegin, strEnd, strFromBeginBZ, strFromEndBZ, strKind, OrderState);
                if (strErrorPartId != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "发送失败";
                    apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M07UE0705", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "发送失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion


    }
}
