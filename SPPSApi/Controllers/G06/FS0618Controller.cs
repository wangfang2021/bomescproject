using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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

namespace SPPSApi.Controllers.G06
{
    [Route("api/FS0618/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0618Controller : BaseController
    {
        FS0618_Logic fs0618_Logic = new FS0618_Logic();
        private readonly string FunctionID = "FS0618";

        private readonly IWebHostEnvironment _webHostEnvironment;


        public FS0618Controller(IWebHostEnvironment webHostEnvironment)
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
                List<Object> dataList_C018 = ComFunction.convertAllToResult(ComFunction.getTCode("C018"));//收货方
                List<Object> dataList_C045 = ComFunction.convertAllToResult(ComFunction.getTCode("C045"));//订单状态
                DataTable dt_Dock = fs0618_Logic.getDock();//荷姿状态
                DataTable dt_Supplier = fs0618_Logic.getSupplier();//荷姿状态
                DataTable dt_OrderType = fs0618_Logic.getOrderType();//
                List<Object> dataList_Dock = ComFunction.convertToResult(dt_Dock, new string[] { "vcValue", "vcName" });
                List<Object> dataList_Supplier = ComFunction.convertToResult(dt_Supplier, new string[] { "vcValue", "vcName" });
                List<Object> dataList_OrderType = ComFunction.convertToResult(dt_OrderType, new string[] { "vcValue", "vcName" });

                res.Add("C018", dataList_C018);
                res.Add("C045", dataList_C045);
                res.Add("Dock", dataList_Dock);
                res.Add("Supplier", dataList_Supplier);
                res.Add("OrderType", dataList_OrderType);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE1801", ex, loginInfo.UserId);
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

            string vcTargetYearMonth = dataForm.vcTargetYearMonth == null ? "" : dataForm.vcTargetYearMonth;
            string vcCpdcompany = dataForm.vcCpdcompany == null ? "" : dataForm.vcCpdcompany;
            string vcOrderNo = dataForm.vcOrderNo == null ? "" : dataForm.vcOrderNo;
            string vcDock = dataForm.vcDock == null ? "" : dataForm.vcDock;
            string vcPartNo = dataForm.vcPartNo == null ? "" : dataForm.vcPartNo;
            string vcOrderType = dataForm.vcOrderType == null ? "" : dataForm.vcOrderType;
            string vcSupplier_id = dataForm.vcSupplier_id == null ? "" : dataForm.vcSupplier_id;
            string dOrderExportDate = dataForm.dOrderExportDate == null ? "" : dataForm.dOrderExportDate;
            try
            {
                DataTable dt = fs0618_Logic.Search(vcTargetYearMonth, vcCpdcompany, vcOrderNo, vcDock, vcPartNo, vcOrderType, vcSupplier_id, dOrderExportDate);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("dOrderDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                //dtConverter.addField("vcTargetYearMonth", ConvertFieldType.DateType, "yyyy/MM");
                dtConverter.addField("dOrderExportDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE1802", ex, loginInfo.UserId);
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

            string vcTargetYearMonth = dataForm.vcTargetYearMonth == null ? "" : dataForm.vcTargetYearMonth;
            string vcCpdcompany = dataForm.vcCpdcompany == null ? "" : dataForm.vcCpdcompany;
            string vcOrderNo = dataForm.vcOrderNo == null ? "" : dataForm.vcOrderNo;
            string vcDock = dataForm.vcDock == null ? "" : dataForm.vcDock;
            string vcPartNo = dataForm.vcPartNo == null ? "" : dataForm.vcPartNo;
            string vcOrderType = dataForm.vcOrderType == null ? "" : dataForm.vcOrderType;
            string vcSupplier_id = dataForm.vcSupplier_id == null ? "" : dataForm.vcSupplier_id;
            string dOrderExportDate = dataForm.dOrderExportDate == null ? "" : dataForm.dOrderExportDate;
            try
            {
                DataTable dt = fs0618_Logic.Search(vcTargetYearMonth, vcCpdcompany, vcOrderNo, vcDock, vcPartNo, vcOrderType, vcSupplier_id, dOrderExportDate);
               
                string[] fields = { "vcPackingFactory", "vcTargetYearMonth", "vcDock", "vcCpdcompany", "vcOrderType", "vcOrderNo", "vcSeqno",
                                     "dOrderDate", "dOrderExportDate", "vcPartNo", "vcInsideOutsideType",
                                      "vcPlantQtyDaily1", "vcInputQtyDaily1", "vcResultQtyDaily1", "vcPlantQtyDaily2", "vcInputQtyDaily2",
                                     "vcResultQtyDaily2", "vcPlantQtyDaily3", "vcInputQtyDaily3", "vcResultQtyDaily3", "vcPlantQtyDaily4", "vcInputQtyDaily4",
                                     "vcResultQtyDaily4", "vcPlantQtyDaily5", "vcInputQtyDaily5", "vcResultQtyDaily5", "vcPlantQtyDaily6", "vcInputQtyDaily6",
                                     "vcResultQtyDaily6", "vcPlantQtyDaily7", "vcInputQtyDaily7", "vcResultQtyDaily7", "vcPlantQtyDaily8", "vcInputQtyDaily8",
                                     "vcResultQtyDaily8", "vcPlantQtyDaily9", "vcInputQtyDaily9", "vcResultQtyDaily9", "vcPlantQtyDaily10", "vcInputQtyDaily10",
                                     "vcResultQtyDaily10", "vcPlantQtyDaily11", "vcInputQtyDaily11", "vcResultQtyDaily11", "vcPlantQtyDaily12", "vcInputQtyDaily12",
                                     "vcResultQtyDaily12", "vcPlantQtyDaily13", "vcInputQtyDaily13", "vcResultQtyDaily13", "vcPlantQtyDaily14", "vcInputQtyDaily14",
                                     "vcResultQtyDaily14", "vcPlantQtyDaily15", "vcInputQtyDaily15", "vcResultQtyDaily15", "vcPlantQtyDaily16", "vcInputQtyDaily16",
                                     "vcResultQtyDaily16", "vcPlantQtyDaily17", "vcInputQtyDaily17", "vcResultQtyDaily17", "vcPlantQtyDaily18", "vcInputQtyDaily18",
                                     "vcResultQtyDaily18", "vcPlantQtyDaily19", "vcInputQtyDaily19", "vcResultQtyDaily19", "vcPlantQtyDaily20", "vcInputQtyDaily20",
                                     "vcResultQtyDaily20", "vcPlantQtyDaily21", "vcInputQtyDaily21", "vcResultQtyDaily21", "vcPlantQtyDaily22", "vcInputQtyDaily22",
                                     "vcResultQtyDaily22", "vcPlantQtyDaily23", "vcInputQtyDaily23", "vcResultQtyDaily23", "vcPlantQtyDaily24", "vcInputQtyDaily24",
                                     "vcResultQtyDaily24", "vcPlantQtyDaily25", "vcInputQtyDaily25", "vcResultQtyDaily25", "vcPlantQtyDaily26", "vcInputQtyDaily26",
                                     "vcResultQtyDaily26", "vcPlantQtyDaily27", "vcInputQtyDaily27", "vcResultQtyDaily27", "vcPlantQtyDaily28", "vcInputQtyDaily28",
                                     "vcResultQtyDaily28", "vcPlantQtyDaily29", "vcInputQtyDaily29", "vcResultQtyDaily29", "vcPlantQtyDaily30", "vcInputQtyDaily30",
                                     "vcResultQtyDaily30", "vcPlantQtyDaily31", "vcInputQtyDaily31", "vcResultQtyDaily31","vcPlantQtyTotal","vcInputQtyTotal",
                                     "vcResultQtyTotal", "vcCarType", "vcLastPartNo", "vcPackingSpot","vcTargetMonthFlag", "vcTargetMonthLast"
                };
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0618_Export.xlsx", 2, loginInfo.UserId, FunctionID);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE1803", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
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
                    bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    if (bAddFlag == true)
                    {//新增
                        hasFind = true;
                    }
                    else if (bAddFlag == false && bModFlag == true)
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
                //开始数据验证
                if (hasFind)
                {
                    string[,] strField = new string[,] {{
                            "包装工厂", "对象年月", "受入",   "收货方",  "订单区分", "订单编号", "连番",   "订单日期", "订单导入日期",   "品番",   "内外",   "供应商代码",
                            "订单数",  "到货数",  "发货数",  "订单数",  "到货数",  "发货数",  "订单数",  "到货数",  "发货数",  "订单数",  "到货数",  "发货数",
                            "订单数",  "到货数",  "发货数",  "订单数",  "到货数",  "发货数",  "订单数",  "到货数",  "发货数",  "订单数",  "到货数",  "发货数",
                            "订单数",  "到货数",  "发货数",  "订单数",  "到货数",  "发货数",  "订单数",  "到货数",  "发货数",  "订单数",  "到货数",  "发货数",
                            "订单数",  "到货数",  "发货数",  "订单数",  "到货数",  "发货数",  "订单数",  "到货数",  "发货数",  "订单数",  "到货数",  "发货数",
                            "订单数",  "到货数",  "发货数",  "订单数",  "到货数",  "发货数",  "订单数",  "到货数",  "发货数",  "订单数",  "到货数",  "发货数",
                            "订单数",  "到货数",  "发货数",  "订单数",  "到货数",  "发货数",  "订单数",  "到货数",  "发货数",  "订单数",  "到货数",  "发货数",
                            "订单数",  "到货数",  "发货数",  "订单数",  "到货数",  "发货数",  "订单数",  "到货数",  "发货数",  "订单数",  "到货数",  "发货数",
                            "订单数",  "到货数",  "发货数",  "订单数",  "到货数",  "发货数",  "订单数",   "到货数", "发货数",
                            "月总订单数","月总到货数","月总发货数","车型编码","原品番","包装场","对象月识别","对象月最终日"

                        },
                                                {"vcPackingFactory", "vcTargetYearMonth", "vcDock", "vcCpdcompany", "vcOrderType", "vcOrderNo", "vcSeqno",
                                     "dOrderDate", "dOrderExportDate", "vcPartNo", "vcInsideOutsideType",
                                     "vcSupplier_id", "vcPlantQtyDaily1", "vcInputQtyDaily1", "vcResultQtyDaily1", "vcPlantQtyDaily2", "vcInputQtyDaily2",
                                     "vcResultQtyDaily2", "vcPlantQtyDaily3", "vcInputQtyDaily3", "vcResultQtyDaily3", "vcPlantQtyDaily4", "vcInputQtyDaily4",
                                     "vcResultQtyDaily4", "vcPlantQtyDaily5", "vcInputQtyDaily5", "vcResultQtyDaily5", "vcPlantQtyDaily6", "vcInputQtyDaily6",
                                     "vcResultQtyDaily6", "vcPlantQtyDaily7", "vcInputQtyDaily7", "vcResultQtyDaily7", "vcPlantQtyDaily8", "vcInputQtyDaily8",
                                     "vcResultQtyDaily8", "vcPlantQtyDaily9", "vcInputQtyDaily9", "vcResultQtyDaily9", "vcPlantQtyDaily10", "vcInputQtyDaily10",
                                     "vcResultQtyDaily10", "vcPlantQtyDaily11", "vcInputQtyDaily11", "vcResultQtyDaily11", "vcPlantQtyDaily12", "vcInputQtyDaily12",
                                     "vcResultQtyDaily12", "vcPlantQtyDaily13", "vcInputQtyDaily13", "vcResultQtyDaily13", "vcPlantQtyDaily14", "vcInputQtyDaily14",
                                     "vcResultQtyDaily14", "vcPlantQtyDaily15", "vcInputQtyDaily15", "vcResultQtyDaily15", "vcPlantQtyDaily16", "vcInputQtyDaily16",
                                     "vcResultQtyDaily16", "vcPlantQtyDaily17", "vcInputQtyDaily17", "vcResultQtyDaily17", "vcPlantQtyDaily18", "vcInputQtyDaily18",
                                     "vcResultQtyDaily18", "vcPlantQtyDaily19", "vcInputQtyDaily19", "vcResultQtyDaily19", "vcPlantQtyDaily20", "vcInputQtyDaily20",
                                     "vcResultQtyDaily20", "vcPlantQtyDaily21", "vcInputQtyDaily21", "vcResultQtyDaily21", "vcPlantQtyDaily22", "vcInputQtyDaily22",
                                     "vcResultQtyDaily22", "vcPlantQtyDaily23", "vcInputQtyDaily23", "vcResultQtyDaily23", "vcPlantQtyDaily24", "vcInputQtyDaily24",
                                     "vcResultQtyDaily24", "vcPlantQtyDaily25", "vcInputQtyDaily25", "vcResultQtyDaily25", "vcPlantQtyDaily26", "vcInputQtyDaily26",
                                     "vcResultQtyDaily26", "vcPlantQtyDaily27", "vcInputQtyDaily27", "vcResultQtyDaily27", "vcPlantQtyDaily28", "vcInputQtyDaily28",
                                     "vcResultQtyDaily28", "vcPlantQtyDaily29", "vcInputQtyDaily29", "vcResultQtyDaily29", "vcPlantQtyDaily30", "vcInputQtyDaily30",
                                     "vcResultQtyDaily30", "vcPlantQtyDaily31", "vcInputQtyDaily31", "vcResultQtyDaily31","vcPlantQtyTotal","vcInputQtyTotal",
                                     "vcResultQtyTotal", "vcCarType", "vcLastPartNo", "vcPackingSpot","vcTargetMonthFlag", "vcTargetMonthLast"
                        },
                                                {"","","","","","","",FieldCheck.Date,FieldCheck.Date,"","","",
                        FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,
                        FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,
                        FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,
                        FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,
                        FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,
                        FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,
                        FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,
                        FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,
                        "","","","",FieldCheck.NumChar,"","",""
                        },
                                                {"100","20","20","20","20","100","20","0","0","12","100","4",
                        "20","20","20","20","20","20","20","20","20","20","20","20",
                        "20","20","20","20","20","20","20","20","20","20","20","20",
                        "20","20","20","20","20","20","20","20","20","20","20","20",
                        "20","20","20","20","20","20","20","20","20","20","20","20",
                        "20","20","20","20","20","20","20","20","20","20","20","20",
                        "20","20","20","20","20","20","20","20","20","20","20","20",
                        "20","20","20","20","20","20","20","20","20","20","20","20",
                        "20","20","20","20","20","20","20","20","20",
                        "20","20","20","50","12","50","20","20"
                        
                        },//最大长度设定,不校验最大长度用0
                                                {"1","1","1","1","1","1","1","1","1","1","1","1",
                        "0","0","0","0","0","0","0","0","0","0","0","0",
                        "0","0","0","0","0","0","0","0","0","0","0","0",
                        "0","0","0","0","0","0","0","0","0","0","0","0",
                        "0","0","0","0","0","0","0","0","0","0","0","0",
                        "0","0","0","0","0","0","0","0","0","0","0","0",
                        "0","0","0","0","0","0","0","0","0","0","0","0",
                        "0","0","0","0","0","0","0","0","0","0","0","0",
                        "0","0","0","0","0","0","0","0","0",
                        "0","0","0","0","0","0","0","0"
                        },//最小长度设定,可以为空用0
                                                {"1","2","3","4","5","6","7","8","9","10","11","12",
                        "13","14","15","16","17","18","19","20","21","22","23","24",
                        "25","26","27","28","29","30","31","32","33","34","35","36",
                        "37","38","39","40","41","42","43","44","45","46","47","48",
                        "49","50","51","52","53","54","55","56","57","58","59","60",
                        "61","62","63","64","65","66","67","68","69","70","71","72",
                        "73","74","75","76","77","78","79","80","81","82","83","84",
                        "85","86","87","88","89","90","91","92","93","94","95","96",
                        "97","98","99","100","101","102","103","104","105",
                         "106","107","108","109","110","111","112","113"

                        }//前台显示列号，从0开始计算,注意有选择框的是0
                         };
                    //需要判断时间区间先后关系的字段
                    string[,] strDateRegion = { };
                    string[,] strSpecialCheck = { //例子-变更事项字段，当它为新设时，号旧必须为号口，旧型开始、旧型结束、旧型持续开始必须为空
                        
                          };

                    List<Object> checkRes = ListChecker.validateList(listInfoData, strField, strDateRegion, strSpecialCheck, true, "FS0618");
                    if (checkRes != null)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = checkRes;
                        apiResult.flag = Convert.ToInt32(ERROR_FLAG.单元格定位提示);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }

                string strErrorPartId = "";
                fs0618_Logic.Save(listInfoData, loginInfo.UserId, ref strErrorPartId);
                if (strErrorPartId != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "保存失败，以下供应商代码存在重叠：<br/>" + strErrorPartId;
                    apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE1804", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }

        }
        #endregion
        #region 删除
        [HttpPost]
        [EnableCors("any")]
        public string deleteApi([FromBody] dynamic data)
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
                JArray checkedInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();
                if (listInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选择一条数据！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fs0618_Logic.Del(listInfoData, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE1805", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

       
    }
}

