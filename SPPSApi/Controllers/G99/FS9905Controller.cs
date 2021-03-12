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

namespace SPPSApi.Controllers.G99
{
    [Route("api/FS9905/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS9905Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS9905_Logic fs9905_Logic = new FS9905_Logic();
        private readonly string FunctionID = "FS9905";

        public FS9905Controller(IWebHostEnvironment webHostEnvironment)
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
                List<Object> dataList_C040 = ComFunction.convertAllToResult(ComFunction.getTCode("C040"));//生确进度
                List<Object> dataList_C002 = ComFunction.convertAllToResult(ComFunction.getTCode("C002"));//变更事项
                List<Object> dataList_C003 = ComFunction.convertAllToResult(ComFunction.getTCode("C003"));//内外区分
                List<Object> dataList_C012 = ComFunction.convertAllToResult(ComFunction.getTCode("C012"));//OE
                List<Object> dataList_C015 = ComFunction.convertAllToResult(ComFunction.getTCode("C015"));//省
                List<Object> dataList_C016 = ComFunction.convertAllToResult(ComFunction.getTCode("C016"));//包装事业体
                List<Object> dataList_C028 = ComFunction.convertAllToResult(ComFunction.getTCode("C028"));//防锈指示
                List<Object> dataList_C029 = ComFunction.convertAllToResult(ComFunction.getTCode("C029"));//对应可否确认结果
                List<Object> dataList_C030 = ComFunction.convertAllToResult(ComFunction.getTCode("C030"));//防锈对应可否

                List<Object> dataList_C040_all = new List<object>();

                foreach (var item in dataList_C040)
                {
                    dataList_C040_all.Add(item);
                }
                #region 在获取的TCode中添加新行
                DataTable dt = new DataTable();
                dt.Columns.Add("vcName");
                dt.Columns.Add("vcValue");
                DataRow dr = dt.NewRow();
                dr["vcName"] = "处理中";
                dr["vcValue"] = "4";
                dt.Rows.Add(dr);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Dictionary<string, object> row = new Dictionary<string, object>();
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        string colName = dt.Columns[j].ColumnName;
                        row[colName] = dt.Rows[i][colName];
                    }
                    row["iAPILineNo"] = dataList_C040.Count-1;
                    dataList_C040_all.Add(row);
                }
                #endregion

                res.Add("C040", dataList_C040);
                res.Add("C002", dataList_C002);
                res.Add("C003", dataList_C003);
                res.Add("C015", dataList_C015);
                res.Add("C012", dataList_C012);
                res.Add("C016", dataList_C016);
                res.Add("C028", dataList_C028);
                res.Add("C029", dataList_C029);
                res.Add("C030", dataList_C030);
                res.Add("C040_all", dataList_C040_all);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M99UE0501", ex, loginInfo.UserId);
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

            string strJD = dataForm.vcJD;
            string strInOutflag = dataForm.vcInOutflag;
            string strSupplier_id = dataForm.vcSupplier_id;
            string strCarType = dataForm.vcCarType;
            string strPart_id = dataForm.vcPart_id;
            string strIsDYJG = dataForm.vcIsDYJG;
            string strIsDYFX = dataForm.vcIsDYFX;

            try
            {
                #region 拿到统括库中的所有供应商生确单
                DataTable dt = fs9905_Logic.Search(strJD, strInOutflag, strSupplier_id, strCarType, strPart_id,loginInfo.UserId);
                #endregion

                if (dt!=null && dt.Rows.Count>0)
                {
                    #region 去现地库取出供应商可否编辑的数据
                    List<string> SupplierLists = new List<string>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i]["vcSupplier_id"] == null)
                        {
                            apiResult.code = ComConstant.SUCCESS_CODE;
                            apiResult.data = "检索错误，出现品番无供应商情况";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        SupplierLists.Add(dt.Rows[i]["vcSupplier_id"].ToString());
                    }
                    DataTable SupplierDT = fs9905_Logic.SearchSupplierEditDT(SupplierLists.Distinct().ToList());
                    #endregion

                    #region 在DT中添加供应商可否编辑判断字段列，根据获取到的供应商可否编辑信息判断供应商可否编辑(可编辑：1  不可编辑：0)
                    dt.Columns.Add("vcSupplierEditFlag");
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string supplier_id = dt.Rows[i]["vcSupplier_id"].ToString();
                        DataRow[] dataRows = SupplierDT.Select("vcValue1='" + supplier_id + "'");
                        if (dataRows.Count() > 0)
                        {
                            if (dataRows[0]["vcValue2"] != null && dataRows[0]["vcValue2"].ToString() == "是")
                            {
                                dt.Rows[i]["vcSupplierEditFlag"] = "1";
                            }
                            else
                            {
                                dt.Rows[i]["vcSupplierEditFlag"] = "0";
                            }
                        }
                        else
                        {
                            dt.Rows[i]["vcSupplierEditFlag"] = "0";
                        }
                    }
                    #endregion
                
                }
                DtConverter dtConverter = new DtConverter();

                dtConverter.addField("selected", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcSupplierEditFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcSupplierEditFlag", ConvertFieldType.BoolType, null);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M99UE0502", ex, loginInfo.UserId);
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
                DataTable dt = fs9905_Logic.Search(loginInfo.UserId);

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region 去现地库取出供应商可否编辑的数据
                    List<string> SupplierLists = new List<string>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i]["vcSupplier_id"] == null)
                        {
                            apiResult.code = ComConstant.SUCCESS_CODE;
                            apiResult.data = "检索错误，出现品番无供应商情况";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        SupplierLists.Add(dt.Rows[i]["vcSupplier_id"].ToString());
                    }
                    DataTable SupplierDT = fs9905_Logic.SearchSupplierEditDT(SupplierLists.Distinct().ToList());
                    #endregion

                    #region 在DT中添加供应商可否编辑判断字段列，根据获取到的供应商可否编辑信息判断供应商可否编辑(可编辑：1  不可编辑：0)
                    dt.Columns.Add("vcSupplierEditFlag");
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string supplier_id = dt.Rows[i]["vcSupplier_id"].ToString();
                        DataRow[] dataRows = SupplierDT.Select("vcValue1='" + supplier_id + "'");
                        if (dataRows.Count() > 0)
                        {
                            if (dataRows[0]["vcValue2"] != null && dataRows[0]["vcValue2"].ToString() == "是")
                            {
                                dt.Rows[i]["vcSupplierEditFlag"] = "1";
                            }
                            else
                            {
                                dt.Rows[i]["vcSupplierEditFlag"] = "0";
                            }
                        }
                        else
                        {
                            dt.Rows[i]["vcSupplierEditFlag"] = "0";
                        }
                    }
                    #endregion
                }

                DtConverter dtConverter = new DtConverter();

                dtConverter.addField("selected", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcSCSNameModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcSCSPlaceModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcSupplierEditFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcSupplierEditFlag", ConvertFieldType.BoolType, null);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M99UE0502", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 检索退回履历
        [HttpPost]
        [EnableCors("any")]
        public string searchTHListApi([FromBody] dynamic data)
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

            string strGUID = dataForm.GUID;

            try
            {
                DataTable dt = fs9905_Logic.SearchTHList(strGUID);
                DtConverter dtConverter = new DtConverter();

                dtConverter.addField("dTHTime", ConvertFieldType.DateType,"yyyyMMdd");
                dtConverter.addField("dOperatorTime", ConvertFieldType.DateType,"yyyyMMdd");

                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M99UE0502", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
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
                bool hasFind = false;//是否找到需要新增或者修改的数据
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    
                    if (bModFlag == true)
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
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    int byteLength = System.Text.Encoding.Default.GetBytes(listInfoData[i]["vcYQorNG"].ToString()).Length;
                    //延期说明超过了600字节，提示错误
                    if (byteLength > 600)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "保存失败，延期说明文本超长";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                string strErr = "";
                fs9905_Logic.Save(listInfoData, loginInfo.UserId, ref strErr);
                if (strErr != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = strErr;
                    apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M99UE0503", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
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

            string strJD = dataform.vcJD;
            string strInOutflag = dataform.strInOutflag;
            string strSupplier_id = dataform.vcSupplier_id;
            string strCarType = dataform.vcCarType;
            string strPart_id = dataform.vcPart_id;
            string strIsDYJG = dataform.vcIsDYJG;
            string strIsDYFX = dataform.vcIsDYFX;
            try
            {
                DataTable dt = fs9905_Logic.Search(strJD, strInOutflag, strSupplier_id, strCarType, strPart_id,logininfo.UserId);
                string[] fields = { "vcPart_id", "dSSDate", "vcJD_Name", "vcSPINo",
                                    "vcChange_Name", "vcCarType","vcInOutflag_Name","vcPartName",
                                    "vcOE_Name","vcSupplier_id","vcFXDiff_Name","vcFXNo",
                                    "vcSumLater","vcIsDYJG_Name","vcIsDYFX_Name","vcNotDY",
                                    "vcYQorNG","vcTH","vcSCPlace_City","vcSCPlace_Province",
                                    "vcCHPlace_City","vcCHPlace_Province","vcSCSName","vcSCSPlace",
                                    "dSupplier_BJ","dSupplier_HK","dTFTM_BJ","vcZXBZDiff","vcZXBZNo","vcNum1",
                                    "vcNum2","vcNum3","vcNum4","vcNum5","vcNum6","vcNum7","vcNum8","vcNum9",
                                    "vcNum10","vcNum11","vcNum12","vcNum13","vcNum14","vcNum15"
                };
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS9905_Export.xlsx", 2, logininfo.UserId, FunctionID);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M99UE0504", ex, logininfo.UserId);
                apiresult.code = ComConstant.ERROR_CODE;
                apiresult.data = "导出失败";
                return JsonConvert.SerializeObject(apiresult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 生确回复
        [HttpPost]
        [EnableCors("any")]
        public string sendApi([FromBody] dynamic data)
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
                    apiResult.data = "未选择任何行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                
                #region 数据校验
                //开始数据验证
                string[,] strField = new string[,] {{"对应可否确认结果","防锈对应可否"},
                                                    {"vcIsDYJG"        ,"vcIsDYFX"    },
                                                    {""                ,""            },
                                                    {"1"               ,"1"           },//最大长度设定,不校验最大长度用0
                                                    {"1"               ,"1"           },//最小长度设定,可以为空用0
                                                    {"14"              ,"15"          }//前台显示列号，从0开始计算,注意有选择框的是0
                    };
                //需要判断时间区间先后关系的字段
                string[,] strDateRegion = null;

                /*
                 * 执行标准区分为CCC时，执行标准No必填
                 */
                /*                         验证vcChange字段     当vcChange = 1时     判断字段    1:该字段不能为空 0:该字段必须为空      该字段有值且验证标记为“1”，则vcHaoJiu必须等于H，该字段为空且验证标记为“1”,则该字段值填什么都行    */
                string[,] strSpecialCheck = {
                         { "执行标准区分"    ,"vcZXBZDiff","CCC"       ,"CCC","执行标准NO"         ,"vcZXBZNo"          ,"1",                      "","" }
                        ,{ "对应可否确认结果","vcIsDYJG"  ,"不可对应"  ,"2"  ,"对应不可理由"       ,"vcNotDY"           ,"1",                      "","" }
                        };

                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (listInfoData[i]["vcIsDYJG"].ToString()=="1")
                    {
                        string strChange = listInfoData[i]["vcChange"].ToString();
                        if (strChange=="1" || strChange =="2" || strChange =="10" || strChange =="8" || strChange =="12")
                        {
                            if (
                                  listInfoData[i]["vcSCPlace_City"]==null || listInfoData[i]["vcSCPlace_City"].ToString()==""
                                || listInfoData[i]["vcSCPlace_Province"] == null || listInfoData[i]["vcSCPlace_Province"].ToString() == ""
                                || listInfoData[i]["vcCHPlace_City"] == null || listInfoData[i]["vcCHPlace_City"].ToString() == ""
                                || listInfoData[i]["vcCHPlace_Province"] == null || listInfoData[i]["vcCHPlace_Province"].ToString() == ""
                                || listInfoData[i]["dSupplier_BJ"] == null || listInfoData[i]["dSupplier_BJ"].ToString() == ""
                                || listInfoData[i]["dSupplier_HK"] == null || listInfoData[i]["dSupplier_HK"].ToString() == ""
                                )
                            {
                                apiResult.code = ComConstant.ERROR_CODE;
                                apiResult.data = "生确回复失败！当对应可否确认结果为可对应、设变为新设时，生产地-市、生产地-省、出荷地-市、出荷地-省、补给日期、号口日期必须填写";
                                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                            }
                        }
                        else
                        {
                            if (  
                                  listInfoData[i]["dSupplier_BJ"] == null || listInfoData[i]["dSupplier_BJ"].ToString() == ""
                                || listInfoData[i]["dSupplier_HK"] == null || listInfoData[i]["dSupplier_HK"].ToString() == ""
                                )
                            {
                                apiResult.code = ComConstant.ERROR_CODE;
                                apiResult.data = "生确回复失败！当对应可否确认结果为可对应时，补给日期、号口日期必须填写";
                                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                            }
                        }
                    }
                    else if(listInfoData[i]["vcIsDYJG"].ToString() == "2" || listInfoData[i]["vcIsDYJG"].ToString()=="2")
                    {
                        if (listInfoData[i]["vcNotDY"]==null || listInfoData[i]["vcNotDY"].ToString()=="")
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "生确回复失败！当对应可否确认结果或者防锈对应可否为不可对应时，不可对应理由必须填写";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                    }
                    if (listInfoData[i]["vcJD"].ToString()=="2")
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "生确回复失败！已回复生确不可再次回复";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (
                          listInfoData[i]["vcSCSName"]==null || listInfoData[i]["vcSCSName"].ToString()==""
                        || listInfoData[i]["vcSCSPlace"] == null || listInfoData[i]["vcSCSPlace"].ToString() == ""
                        )
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "生确回复失败！生产商名称和生产商地址必须填写";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }

                List<Object> checkRes = ListChecker.validateList(listInfoData, strField, strDateRegion, strSpecialCheck, true, "FS9905");
                if (checkRes != null)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = checkRes;
                    apiResult.flag = Convert.ToInt32(ERROR_FLAG.单元格定位提示);
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                #endregion

                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (listInfoData[i]["vcJD"].ToString() == "2")
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "已回复的生确单不可重复回复";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }

                    string strChange = listInfoData[i]["vcChange"] == null ? "" : listInfoData[i]["vcChange"].ToString();
                    string strSCPlace_City = listInfoData[i]["vcSCPlace_City"] == null ? "" : listInfoData[i]["vcSCPlace_City"].ToString();
                    string strSCPlace_Province = listInfoData[i]["vcSCPlace_Province"] == null ? "" : listInfoData[i]["vcSCPlace_Province"].ToString();
                    string strCHPlace_City = listInfoData[i]["vcCHPlace_City"] == null ? "" : listInfoData[i]["vcCHPlace_City"].ToString();
                    string strCHPlace_Province = listInfoData[i]["vcCHPlace_Province"] == null ? "" : listInfoData[i]["vcCHPlace_Province"].ToString();
                    if ((strChange == "1" || strChange == "2" || strChange == "3" || strChange == "4") && (string.IsNullOrEmpty(strSCPlace_City) || string.IsNullOrEmpty(strCHPlace_City)))
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "生确回复时，变更事项为新车新设、设变新设、打切旧型、设变旧型时，生产地与出荷地不能为空！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }

                string strErr = "";
                fs9905_Logic.Send(listInfoData, loginInfo.UserId, ref strErr);
                if (strErr != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data =  strErr;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M99UE0505", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 延期说明
        [HttpPost]
        [EnableCors("any")]
        public string sendYQApi([FromBody] dynamic data)
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

                if (listInfoData.Count <= 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "未选择任何行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                #region 数据校验
                //延期说明不能为空
                string[,] strField = new string[,] {{"延期说明"},
                                                    {"vcYQorNG"        },
                                                    {""                },
                                                    {"0"               },//最大长度设定,不校验最大长度用0
                                                    {"1"               },//最小长度设定,可以为空用0
                                                    {"17"              }//前台显示列号，从0开始计算,注意有选择框的是0
                    };
                string[,] strDateRegion = null;
                string[,] strSpecialCheck = null;

                List<Object> checkRes = ListChecker.validateList(listInfoData, strField, strDateRegion, strSpecialCheck, true, "FS9905");
                if (checkRes != null)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = checkRes;
                    apiResult.flag = Convert.ToInt32(ERROR_FLAG.单元格定位提示);
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                #endregion

                string strErr = "";
                fs9905_Logic.SendYQ(listInfoData, loginInfo.UserId, ref strErr);
                if (strErr != "")
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M99UE0505", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "延期说明发送失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 一括付与
        [HttpPost]
        [EnableCors("any")]
        public string setFYApi([FromBody] dynamic data)
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
                string strSupplier_BJ = dataForm.dSupplier_BJ;
                string strSupplier_HK = dataForm.dSupplier_HK;
                string strSCPlace_City = dataForm.vcSCPlace_City;
                string strSCPlace_Province = dataForm.vcSCPlace_Province;
                string strCHPlace_City = dataForm.vcCHPlace_City;
                string strCHPlace_Province = dataForm.vcCHPlace_Province;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();

                //判断至少勾选了一条数据
                if (listInfoData.Count<=0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "未选中任何行";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                //如果都为null，就不能进行一括付与
                if (string.IsNullOrEmpty(strSupplier_BJ) && string.IsNullOrEmpty(strSupplier_HK) && string.IsNullOrEmpty(strSCPlace_City) && string.IsNullOrEmpty(strSCPlace_Province) && string.IsNullOrEmpty(strCHPlace_City) && string.IsNullOrEmpty(strCHPlace_Province))
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "未填写任何信息，一括付与失败！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                string strErr = "";
                //已回复数据不进行一括付与，需要剔除
                fs9905_Logic.SetFY(listInfoData, strSupplier_BJ, strSupplier_HK, strSCPlace_City, strSCPlace_Province, strCHPlace_City, strCHPlace_Province, loginInfo.UserId, ref strErr);
                if (strErr != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = strErr;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "一括付与成功";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M99UE0506", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 下载PDF文件
        [HttpPost]
        [EnableCors("any")]
        public string downloadPDFApi([FromBody] dynamic data)
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
            try
            { 
                string strFileName = dataform.fileName;
                string fileSavePath = strFileName + ".pdf";
                apiresult.code = ComConstant.SUCCESS_CODE;
                apiresult.data = fileSavePath;
                return JsonConvert.SerializeObject(apiresult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M99UE0504", ex, logininfo.UserId);
                apiresult.code = ComConstant.ERROR_CODE;
                apiresult.data = "导出失败";
                return JsonConvert.SerializeObject(apiresult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}
