﻿using System;
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

namespace SPPSApi.Controllers.G03
{
    [Route("api/FS0304/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0304Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0304_Logic fs0304_Logic = new FS0304_Logic();
        private readonly string FunctionID = "FS0304";

        public FS0304Controller(IWebHostEnvironment webHostEnvironment)
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
                
                List<Object> dataList_C026 = ComFunction.convertAllToResult(ComFunction.getTCode("C026"));      //编辑行使用的生确进度
                List<Object> dataList_C002 = ComFunction.convertAllToResult(ComFunction.getTCode("C002"));      //变更事项
                List<Object> dataList_C003 = ComFunction.convertAllToResult(ComFunction.getTCode("C003"));      //内外                
                List<Object> dataList_C012 = ComFunction.convertAllToResult(ComFunction.getTCode("C012"));      //OE                
                List<Object> dataList_C015 = ComFunction.convertAllToResult(ComFunction.getTCode("C015"));      //省份                
                List<Object> dataList_C028 = ComFunction.convertAllToResult(ComFunction.getTCode("C028"));      //防锈指示
                List<Object> dataList_C029 = ComFunction.convertAllToResult(ComFunction.getTCode("C029"));      //对应可否确认结果
                List<Object> dataList_C030 = ComFunction.convertAllToResult(ComFunction.getTCode("C030"));      //防锈对应可否
                List<Object> dataList_C026_all = new List<object>();                                            //检索使用的生确进度

                foreach (var item in dataList_C026)
                {
                    dataList_C026_all.Add(item);
                }
                #region 在获取的TCode中添加新行
                DataTable dt = new DataTable();
                dt.Columns.Add("vcName");
                dt.Columns.Add("vcValue");
                DataRow dr = dt.NewRow();
                dr["vcName"] = "处理中";
                dr["vcValue"] = "5";
                dt.Rows.Add(dr);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Dictionary<string, object> row = new Dictionary<string, object>();
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        string colName = dt.Columns[j].ColumnName;
                        row[colName] = dt.Rows[i][colName];
                    }
                    row["iAPILineNo"] = dataList_C026_all.Count-1;
                    dataList_C026_all.Add(row);
                }
                #endregion

                res.Add("C026", dataList_C026);
                res.Add("C002", dataList_C002);
                res.Add("C003", dataList_C003);
                res.Add("C015", dataList_C015);
                res.Add("C012", dataList_C012);
                res.Add("C028", dataList_C028);
                res.Add("C029", dataList_C029);
                res.Add("C030", dataList_C030);
                res.Add("C026_all", dataList_C026_all);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0401", ex, loginInfo.UserId);
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

            string strSSDate = dataForm.dSSDate;                //实施日期
            string strJD = dataForm.vcJD;                       //生确进度
            string strInOutFlag = dataForm.vcInOutflag;         //内外
            string strSupplier_id = dataForm.vcSupplier_id;     //供应商
            string strCarType = dataForm.vcCarType;             //车种
            string strPart_id = dataForm.vcPart_id;             //品番
            string strIsDYJG = dataForm.vcIsDYJG;               //对应可否确认结果

            try
            {
                DataTable dt = fs0304_Logic.Search(strSSDate, strJD, strPart_id, strInOutFlag, strIsDYJG, strCarType, strSupplier_id);
                DtConverter dtConverter = new DtConverter();

                dtConverter.addField("selected", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("dSSDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dSupplier_BJ", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dSupplier_HK", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dTFTM_BJ", ConvertFieldType.DateType, "yyyy/MM/dd");

                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0402", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 初始化检索
        [HttpPost]
        [EnableCors("any")]
        public string loadSearchApi([FromBody] dynamic data)
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
                DataTable dt = fs0304_Logic.Search();
                DtConverter dtConverter = new DtConverter();

                dtConverter.addField("selected", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("dSSDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dSupplier_BJ", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dSupplier_HK", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dTFTM_BJ", ConvertFieldType.DateType, "yyyy/MM/dd");

                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0402", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 导出
        [HttpPost]
        [EnableCors("any")]
        public string exportapi([FromBody] dynamic data)
        {
            string strtoken = Request.Headers["x-token"];
            if (!isLogin(strtoken))
            {
                return error_login();
            }
            LoginInfo logininfo = getLoginByToken(strtoken);
            //以下开始业务处理
            ApiResult apiresult = new ApiResult();
            dynamic dataform = JsonConvert.DeserializeObject(Convert.ToString(data));

            string strSSDate = dataform.dSSDate;                //实施日期
            string strJD = dataform.vcJD;                       //进度
            string strInOutFlag = dataform.vcInOutflag;         //内外
            string strSupplier_id = dataform.vcSupplier_id;     //供应商
            string strCarType = dataform.vcCarType;             //车种
            string strPart_id = dataform.vcPart_id;             //品番
            string strIsDYJG = dataform.vcIsDYJG;               //对应可否确认结果

            try
            {
                DataTable dt = fs0304_Logic.Search(strSSDate, strJD, strPart_id, strInOutFlag, strIsDYJG, strCarType, strSupplier_id);
                string[] fields = { "dSSDate", "vcJD_Name", "vcPart_id", "vcSPINo",
                                    "vcChange_Name", "vcCarType_Name","vcInOutflag_Name","vcPartName",
                                    "vcOE_Name","vcSupplier_id","vcFXDiff_Name","vcFXNo",
                                    "vcSumLater","vcIsDYJG_Name","vcIsDYFX_Name","vcYQorNG",
                                    "vcSCPlace_City","vcSCPlace_Province","vcCHPlace_City","vcCHPlace_Province",
                                    "vcSYTCode_Name","vcSCSPlace","dSupplier_BJ","dSupplier_HK",
                                    "dTFTM_BJ","vcZXBZDiff","vcZXBZNo","vcNum1",
                                    "vcNum2","vcNum3","vcNum4","vcNum5","vcNum6","vcNum7","vcNum8","vcNum9",
                                    "vcNum10","vcNum11","vcNum12","vcNum13","vcNum14","vcNum15"
                };
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "fs0304_export.xlsx", 2, logininfo.UserId, FunctionID);
                if (filepath == "")
                {
                    apiresult.code = ComConstant.ERROR_CODE;
                    apiresult.data = "导出生成文件失败";
                    return JsonConvert.SerializeObject(apiresult, Formatting.Indented, JSON_SETTING);
                }
                apiresult.code = ComConstant.SUCCESS_CODE;
                apiresult.data = filepath;
                return JsonConvert.SerializeObject(apiresult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0403", ex, logininfo.UserId);
                apiresult.code = ComConstant.ERROR_CODE;
                apiresult.data = "导出失败";
                return JsonConvert.SerializeObject(apiresult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 退回
        [HttpPost]
        [EnableCors("any")]
        public string backApi([FromBody] dynamic data)
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

                //判断至少勾选了一条数据
                if (listInfoData.Count <= 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "未选中任何行";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                //只有已回复的才可以退回
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    string strJD = listInfoData[i]["vcJD"].ToString();
                    if (strJD!="2")
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "所选对象包含未回复品番";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }

                string strTH = dataForm.vcTH;

                string strErr = "";
                fs0304_Logic.Back(listInfoData, loginInfo.UserId,strTH,loginInfo.Email,loginInfo.UserName, ref strErr);
                if (strErr != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = strErr;
                    apiResult.flag = 99;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0404", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "退回失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 保存
        [HttpPost]
        [EnableCors("any")]
        public string saveApi([FromBody] dynamic data)
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

                JArray factoryArray = dataForm.select_ZXBZNo;
                List<string> select_ZXBZNo = factoryArray.ToObject<List<string>>();


                bool hasFind = false;//是否找到需要新增或者修改的数据
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    
                    if ( bModFlag == true)
                    {//修改
                        hasFind = true;
                    }
                }
                if (!hasFind)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少有一个编辑行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //只有已回复的才可以保存
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    string strJD = listInfoData[i]["vcJD"].ToString();
                    if (strJD != "2")
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "所选对象包含未回复品番";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }

                fs0304_Logic.Save(listInfoData, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0405", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 删除
        [HttpPost]
        [EnableCors("any")]
        public string delApi([FromBody] dynamic data)
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
                
                if (listInfoData.Count<=0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选择一条数据！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                fs0304_Logic.del(listInfoData, loginInfo.UserId);
                
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0406", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 织入原单位 
        [HttpPost]
        [EnableCors("any")]
        public string sendUnitApi([FromBody] dynamic data)
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

                if (listInfoData.Count<=0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "未选中任何行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                /*
                 * 织入原单位时，只有生确进度为已回复的才可以织入原单位
                 */
                Hashtable hash = new Hashtable();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (listInfoData[i]["vcJD"].ToString()=="1")
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "所选对象进度状态为已联络，不可织入！";
                        apiResult.data = "已联络信息不可织入";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (listInfoData[i]["vcJD"].ToString()=="3")
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "所选对象进度状态为已退回，不可织入！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (listInfoData[i]["vcJD"].ToString() == "4")
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "不可重复织入";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (listInfoData[i]["vcIsDYJG"].ToString()=="1" && listInfoData[i]["vcIsDYFX"].ToString() == "1" && (listInfoData[i]["dTFTM_BJ"]==null || listInfoData[i]["dTFTM_BJ"].ToString()==""))
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "TFTM调整日期必填";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }

                    string strPart_id = listInfoData[i]["vcPart_id"].ToString();
                    string strSupplier_id = listInfoData[i]["vcSupplier_id"].ToString();
                    string tmp = strPart_id + strSupplier_id;
                    if (hash[tmp] == null)
                        hash[tmp] = tmp;
                    else//曾经添加过，肯定是选择重复了
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "不可选择重复的数据";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }

                string strErrorPartId = "";
                string strErr = "";
                fs0304_Logic.sendUnit(listInfoData, loginInfo.UserId, ref strErr);
                if (strErrorPartId != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "保存失败，以下品番TFTM调整时间小于品番开始时间：<br/>" + strErrorPartId;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (strErr!="")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = strErr;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0407", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 一括付与
        [HttpPost]
        [EnableCors("any")]
        public string dateKFYApi([FromBody] dynamic data)
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
                string dTFTM_BJ = dataForm.dTFTM_BJ;

                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();

                //开始数据验证

                //判断至少勾选了一条数据
                if (listInfoData.Count <= 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "未选中任何行";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //判断是否选择了一个正确的日期
                if (string.IsNullOrEmpty(dTFTM_BJ))
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "未选择付与日期";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //只针对已回复数据进行一括付与
                fs0304_Logic.DateKFY(listInfoData, loginInfo.UserId, dTFTM_BJ);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0408", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}
