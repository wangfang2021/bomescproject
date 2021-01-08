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

namespace SPPSApi.Controllers.G03
{
    [Route("api/FS0305/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0305Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0305_Logic fs0305_Logic = new FS0305_Logic();
        private readonly string FunctionID = "FS0305";

        public FS0305Controller(IWebHostEnvironment webHostEnvironment)
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
                List<Object> dataList_C026 = ComFunction.convertAllToResult(ComFunction.getTCode("C026"));//生确进度
                List<Object> dataList_C002 = ComFunction.convertAllToResult(ComFunction.getTCode("C002"));//内外区分
                List<Object> dataList_C003 = ComFunction.convertAllToResult(ComFunction.getTCode("C003"));//生确进度
                List<Object> dataList_C012 = ComFunction.convertAllToResult(ComFunction.getTCode("C012"));//OE
                List<Object> dataList_C016 = ComFunction.convertAllToResult(ComFunction.getTCode("C016"));//包装事业体
                List<Object> dataList_C098 = ComFunction.convertAllToResult(ComFunction.getTCode("C098"));//车种
                List<Object> dataList_C028 = ComFunction.convertAllToResult(ComFunction.getTCode("C028"));//防锈指示
                List<Object> dataList_C029 = ComFunction.convertAllToResult(ComFunction.getTCode("C029"));//对应结果可否确认结果
                List<Object> dataList_C030 = ComFunction.convertAllToResult(ComFunction.getTCode("C030"));//防锈对应可否

                res.Add("C026", dataList_C026);
                res.Add("C002", dataList_C002);
                res.Add("C003", dataList_C003);
                res.Add("C012", dataList_C012);
                res.Add("C016", dataList_C016);
                res.Add("C098", dataList_C098);
                res.Add("C028", dataList_C028);
                res.Add("C029", dataList_C029);
                res.Add("C030", dataList_C030);

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

            string strJD = dataForm.vcJD;
            string strInOutflag = dataForm.vcInOutflag;
            string strSupplier_id = dataForm.vcSupplier_id;
            string strCarType = dataForm.vcCarType;
            string strPart_id = dataForm.vcPart_id;
            string strIsDYJG = dataForm.vcIsDYJG;
            string strIsDYFX = dataForm.vcIsDYFX;

            try
            {
                DataTable dt = fs0305_Logic.Search(strJD, strInOutflag, strSupplier_id, strCarType, strPart_id);
                DtConverter dtConverter = new DtConverter();

                dtConverter.addField("selected", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("dSSDateMonth", ConvertFieldType.DateType, "yyyy/MM/dd");
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0312", ex, loginInfo.UserId);
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

            string strJD = dataform.vcJD;
            string strInOutflag = dataform.strInOutflag;
            string strSupplier_id = dataform.vcSupplier_id;
            string strCarType = dataform.vcCarType;
            string strPart_id = dataform.vcPart_id;
            string strIsDYJG = dataform.vcIsDYJG;
            string strIsDYFX = dataform.vcIsDYFX;
            try
            {
                DataTable dt = fs0305_Logic.Search(strJD, strInOutflag, strSupplier_id, strCarType, strPart_id);
                string[] fields = { "dSSDateMonth", "vcJD_Name", "vcPart_id", "vcSPINo",
                                    "vcChange_Name", "vcCarType_Name","vcInOutflag_Name","vcPartName",
                                    "vcOE_Name","vcSupplier_id","vcFXDiff_Name","vcFXNo",
                                    "vcSumLater","vcIsDYJG_Name","vcIsDYFX_Name","vcYQorNG",
                                    "vcSCPlace_City","vcSCPlace_Province","vcCHPlace_City","vcCHPlace_Province",
                                    "vcPackFactory_Name","vcSCSPlace","dSupplier_BJ","dSupplier_HK",
                                    "dTFTM_BJ","vcZXBZDiff","vcZXBZNo"
                };
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "fs0305_export.xlsx", 2, logininfo.UserId, FunctionID);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "m03ue0904", ex, logininfo.UserId);
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

                //if (checkRes != null)
                //{
                //    apiResult.code = ComConstant.ERROR_CODE;
                //    apiResult.data = checkRes;
                //    apiResult.flag = Convert.ToInt32(ERROR_FLAG.单元格定位提示);
                //    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                //}

                string strErrorPartId = "";
                fs0305_Logic.Send(listInfoData, loginInfo.UserId, ref strErrorPartId);
                if (strErrorPartId != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "保存失败，以下品番使用开始、结束区间存在重叠：<br/>" + strErrorPartId;
                    apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0312", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 对应可否一括付与
        [HttpPost]
        [EnableCors("any")]
        public string isDYJGApi([FromBody] dynamic data)
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
                string strIsDY = dataForm.vcIsDYJG;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();

                //开始数据验证

                //判断至少勾选了一条数据
                if (listInfoData.Count<=0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "未选中任何行";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                string[,] strField = new string[,] {{"实施日期"     ,"进度","补给品番" ,"设变号" ,"变更事项","车种"     ,"内外区分"   ,"品名"      ,"OE=SP","供应商代码"   ,"防锈指示","防锈指示书","旧型今后必要预测数","对应结果可否确认结果","防锈对应可否","延期说明/NG理由","生产地-市"     ,"生产地-省"         ,"出荷地-市"     ,"出荷地-省"         ,"包装工厂"     ,"生产商名称","生产商地址","供应商切替日期-补给","供应商切替日期-号口","TFTM调整日期-补给","执行标准区分","执行标准NO"},
                                                        {"dSSDateMonth" ,"vcJD","vcPart_id","vcSPINo","vcChange","vcCarType","vcInOutflag","vcPartName","vcOE" ,"vcSupplier_id","vcFXDiff","vcFXNo"    ,"vcSumLater"        ,"vcIsDYJG"            ,"vcIsDYFX"    ,"vcYQorNG"       ,"vcSCPlace_City","vcSCPlace_Province","vcCHPlace_City","vcCHPlace_Province","vcPackFactory","vcSCSName" ,"vcSCSPlace","dSupplier_BJ"       ,"dSupplier_HK"       ,"dTFTM_BJ"         ,"vcZXBZDiff"  ,"vcZXBZNo"  },
                                                        {FieldCheck.Date,""    ,""         ,""       ,""        ,""         ,""           ,""          ,""     ,""             ,""        ,""          ,""                  ,""                    ,""            ,""               ,""              ,""                  ,""              ,""                  ,""             ,""          ,""          ,FieldCheck.Date      ,FieldCheck.Date      ,FieldCheck.Date    ,""            ,""          },
                                                        {"0"            ,"1"   ,"12"       ,"20"     ,"1"       ,"4"        ,"1"          ,"100"       ,"1"    ,"4"            ,"1"       ,"12"        ,"20"                ,"1"                   ,"1"           ,"100"            ,"100"           ,"100"               ,"100"           ,"100"               ,"100"          ,"100"       ,"100"       ,"0"                  ,"0"                  ,"0"                ,"100"         ,"100"       },//最大长度设定,不校验最大长度用0
                                                        {"1"            ,"1"   ,"1"        ,"1"      ,"1"       ,"1"        ,"1"          ,"1"         ,"1"    ,"1"            ,"1"       ,"1"         ,"1"                 ,"1"                   ,"1"           ,"1"              ,"1"             ,"1"                 ,"1"             ,"1"                 ,"1"            ,"1"         ,"1"         ,"1"                  ,"1"                  ,"1"                ,"1"           ,"1"         },//最小长度设定,可以为空用0
                                                        {"1"            ,"2"   ,"3"        ,"4"      ,"5"       ,"6"        ,"7"          ,"8"         ,"9"    ,"10"           ,"11"      ,"12"        ,"13"                ,"14"                  ,"15"          ,"16"             ,"17"            ,"18"                ,"19"            ,"20"                ,"21"           ,"22"        ,"23"        ,"24"                 ,"25"                 ,"26"               ,"27"          ,"28"        }//前台显示列号，从0开始计算,注意有选择框的是0
                    };
                //需要判断时间区间先后关系的字段
                string[,] strDateRegion = null;
                string[,] strSpecialCheck = null;

                List<Object> checkRes = ListChecker.validateList(listInfoData, strField, strDateRegion, strSpecialCheck, true, "FS0305");
                if (checkRes != null)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = checkRes;
                    apiResult.flag = Convert.ToInt32(ERROR_FLAG.单元格定位提示);
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                string strErrorPartId = "";
                fs0305_Logic.IsDYJG(listInfoData, loginInfo.UserId, ref strErrorPartId , strIsDY);
                if (strErrorPartId != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "保存失败，以下品番使用开始、结束区间存在重叠：<br/>" + strErrorPartId;
                    apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0312", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 防锈区分一括付与
        [HttpPost]
        [EnableCors("any")]
        public string isDYFXApi([FromBody] dynamic data)
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
                string strIsDYFX = dataForm.vcIsDYFX;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();

                //开始数据验证

                //判断至少勾选了一条数据
                if (listInfoData.Count <= 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "未选中任何行";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                string[,] strField = new string[,] {{"实施日期"     ,"进度","补给品番" ,"设变号" ,"变更事项","车种"     ,"内外区分"   ,"品名"      ,"OE=SP","供应商代码"   ,"防锈指示","防锈指示书","旧型今后必要预测数","对应结果可否确认结果","防锈对应可否","延期说明/NG理由","生产地-市"     ,"生产地-省"         ,"出荷地-市"     ,"出荷地-省"         ,"包装工厂"     ,"生产商名称","生产商地址","供应商切替日期-补给","供应商切替日期-号口","TFTM调整日期-补给","执行标准区分","执行标准NO"},
                                                        {"dSSDateMonth" ,"vcJD","vcPart_id","vcSPINo","vcChange","vcCarType","vcInOutflag","vcPartName","vcOE" ,"vcSupplier_id","vcFXDiff","vcFXNo"    ,"vcSumLater"        ,"vcIsDYJG"            ,"vcIsDYFX"    ,"vcYQorNG"       ,"vcSCPlace_City","vcSCPlace_Province","vcCHPlace_City","vcCHPlace_Province","vcPackFactory","vcSCSName" ,"vcSCSPlace","dSupplier_BJ"       ,"dSupplier_HK"       ,"dTFTM_BJ"         ,"vcZXBZDiff"  ,"vcZXBZNo"  },
                                                        {FieldCheck.Date,""    ,""         ,""       ,""        ,""         ,""           ,""          ,""     ,""             ,""        ,""          ,""                  ,""                    ,""            ,""               ,""              ,""                  ,""              ,""                  ,""             ,""          ,""          ,FieldCheck.Date      ,FieldCheck.Date      ,FieldCheck.Date    ,""            ,""          },
                                                        {"0"            ,"1"   ,"12"       ,"20"     ,"1"       ,"4"        ,"1"          ,"100"       ,"1"    ,"4"            ,"1"       ,"12"        ,"20"                ,"1"                   ,"1"           ,"100"            ,"100"           ,"100"               ,"100"           ,"100"               ,"100"          ,"100"       ,"100"       ,"0"                  ,"0"                  ,"0"                ,"100"         ,"100"       },//最大长度设定,不校验最大长度用0
                                                        {"1"            ,"1"   ,"1"        ,"1"      ,"1"       ,"1"        ,"1"          ,"1"         ,"1"    ,"1"            ,"1"       ,"1"         ,"1"                 ,"1"                   ,"1"           ,"1"              ,"1"             ,"1"                 ,"1"             ,"1"                 ,"1"            ,"1"         ,"1"         ,"1"                  ,"1"                  ,"1"                ,"1"           ,"1"         },//最小长度设定,可以为空用0
                                                        {"1"            ,"2"   ,"3"        ,"4"      ,"5"       ,"6"        ,"7"          ,"8"         ,"9"    ,"10"           ,"11"      ,"12"        ,"13"                ,"14"                  ,"15"          ,"16"             ,"17"            ,"18"                ,"19"            ,"20"                ,"21"           ,"22"        ,"23"        ,"24"                 ,"25"                 ,"26"               ,"27"          ,"28"        }//前台显示列号，从0开始计算,注意有选择框的是0
                    };
                //需要判断时间区间先后关系的字段
                string[,] strDateRegion = null;
                string[,] strSpecialCheck = null;

                List<Object> checkRes = ListChecker.validateList(listInfoData, strField, strDateRegion, strSpecialCheck, true, "FS0305");
                if (checkRes != null)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = checkRes;
                    apiResult.flag = Convert.ToInt32(ERROR_FLAG.单元格定位提示);
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                string strErrorPartId = "";
                fs0305_Logic.IsDYFX(listInfoData, loginInfo.UserId, ref strErrorPartId, strIsDYFX);
                if (strErrorPartId != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "保存失败，以下品番使用开始、结束区间存在重叠：<br/>" + strErrorPartId;
                    apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0312", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

    }
}
