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
                                     "vcResultQtyDaily30", "vcPlantQtyDaily31", "vcInputQtyDaily31", "vcResultQtyDaily31","vcPlantQtyDailySum","vcInputQtyDailySum",
                                     "vcResultQtyDailySum", "vcCarType", "vcLastPartNo", "vcPackingSpot","vcTargetMonthFlag", "vcTargetMonthLast"
                };
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0618_Export.xlsx", 2, loginInfo.UserId, FunctionID,true);
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
                                     "vcResultQtyDaily30", "vcPlantQtyDaily31", "vcInputQtyDaily31", "vcResultQtyDaily31","vcPlantQtyDailySum","vcInputQtyDailySum",
                                     "vcResultQtyDailySum", "vcCarType", "vcLastPartNo", "vcPackingSpot","vcTargetMonthFlag", "vcTargetMonthLast"
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
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    if (bAddFlag == true)
                    {//新增
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        #region
                        int vcInputQtyDaily1 = listInfoData[i]["vcInputQtyDaily1"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcInputQtyDaily1"].ToString());
                        int vcPlantQtyDaily1 = listInfoData[i]["vcPlantQtyDaily1"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcPlantQtyDaily1"].ToString());
                        int vcResultQtyDaily1 = listInfoData[i]["vcResultQtyDaily1"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcResultQtyDaily1"].ToString());

                        int vcInputQtyDaily2 = listInfoData[i]["vcInputQtyDaily2"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcInputQtyDaily2"].ToString());
                        int vcPlantQtyDaily2 = listInfoData[i]["vcPlantQtyDaily2"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcPlantQtyDaily2"].ToString());
                        int vcResultQtyDaily2 = listInfoData[i]["vcResultQtyDaily2"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcResultQtyDaily2"].ToString());

                        int vcInputQtyDaily3 = listInfoData[i]["vcInputQtyDaily3"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcInputQtyDaily3"].ToString());
                        int vcPlantQtyDaily3 = listInfoData[i]["vcPlantQtyDaily3"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcPlantQtyDaily3"].ToString());
                        int vcResultQtyDaily3 = listInfoData[i]["vcResultQtyDaily3"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcResultQtyDaily3"].ToString());

                        int vcInputQtyDaily4 = listInfoData[i]["vcInputQtyDaily4"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcInputQtyDaily4"].ToString());
                        int vcPlantQtyDaily4 = listInfoData[i]["vcPlantQtyDaily4"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcPlantQtyDaily4"].ToString());
                        int vcResultQtyDaily4 = listInfoData[i]["vcResultQtyDaily4"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcResultQtyDaily4"].ToString());

                        int vcInputQtyDaily5 = listInfoData[i]["vcInputQtyDaily5"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcInputQtyDaily5"].ToString());
                        int vcPlantQtyDaily5 = listInfoData[i]["vcPlantQtyDaily5"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcPlantQtyDaily5"].ToString());
                        int vcResultQtyDaily5 = listInfoData[i]["vcResultQtyDaily5"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcResultQtyDaily5"].ToString());

                        int vcInputQtyDaily6 = listInfoData[i]["vcInputQtyDaily6"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcInputQtyDaily6"].ToString());
                        int vcPlantQtyDaily6 = listInfoData[i]["vcPlantQtyDaily6"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcPlantQtyDaily6"].ToString());
                        int vcResultQtyDaily6 = listInfoData[i]["vcResultQtyDaily6"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcResultQtyDaily6"].ToString());

                        int vcInputQtyDaily7 = listInfoData[i]["vcInputQtyDaily7"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcInputQtyDaily7"].ToString());
                        int vcPlantQtyDaily7 = listInfoData[i]["vcPlantQtyDaily7"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcPlantQtyDaily7"].ToString());
                        int vcResultQtyDaily7 = listInfoData[i]["vcResultQtyDaily7"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcResultQtyDaily7"].ToString());

                        int vcInputQtyDaily8 = listInfoData[i]["vcInputQtyDaily8"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcInputQtyDaily8"].ToString());
                        int vcPlantQtyDaily8 = listInfoData[i]["vcPlantQtyDaily8"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcPlantQtyDaily8"].ToString());
                        int vcResultQtyDaily8 = listInfoData[i]["vcResultQtyDaily8"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcResultQtyDaily8"].ToString());

                        int vcInputQtyDaily9 = listInfoData[i]["vcInputQtyDaily9"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcInputQtyDaily9"].ToString());
                        int vcPlantQtyDaily9 = listInfoData[i]["vcPlantQtyDaily9"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcPlantQtyDaily9"].ToString());
                        int vcResultQtyDaily9 = listInfoData[i]["vcResultQtyDaily9"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcResultQtyDaily9"].ToString());

                        int vcInputQtyDaily10 = listInfoData[i]["vcInputQtyDaily10"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcInputQtyDaily10"].ToString());
                        int vcPlantQtyDaily10 = listInfoData[i]["vcPlantQtyDaily10"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcPlantQtyDaily10"].ToString());
                        int vcResultQtyDaily10 = listInfoData[i]["vcResultQtyDaily10"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcResultQtyDaily10"].ToString());

                        int vcInputQtyDaily11 = listInfoData[i]["vcInputQtyDaily11"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcInputQtyDaily11"].ToString());
                        int vcPlantQtyDaily11 = listInfoData[i]["vcPlantQtyDaily11"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcPlantQtyDaily11"].ToString());
                        int vcResultQtyDaily11 = listInfoData[i]["vcResultQtyDaily11"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcResultQtyDaily11"].ToString());

                        int vcInputQtyDaily12 = listInfoData[i]["vcInputQtyDaily12"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcInputQtyDaily12"].ToString());
                        int vcPlantQtyDaily12 = listInfoData[i]["vcPlantQtyDaily12"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcPlantQtyDaily12"].ToString());
                        int vcResultQtyDaily12 = listInfoData[i]["vcResultQtyDaily12"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcResultQtyDaily12"].ToString());

                        int vcInputQtyDaily13 = listInfoData[i]["vcInputQtyDaily13"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcInputQtyDaily13"].ToString());
                        int vcPlantQtyDaily13 = listInfoData[i]["vcPlantQtyDaily13"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcPlantQtyDaily13"].ToString());
                        int vcResultQtyDaily13 = listInfoData[i]["vcResultQtyDaily13"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcResultQtyDaily13"].ToString());

                        int vcInputQtyDaily14 = listInfoData[i]["vcInputQtyDaily14"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcInputQtyDaily14"].ToString());
                        int vcPlantQtyDaily14 = listInfoData[i]["vcPlantQtyDaily14"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcPlantQtyDaily14"].ToString());
                        int vcResultQtyDaily14 = listInfoData[i]["vcResultQtyDaily14"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcResultQtyDaily14"].ToString());

                        int vcInputQtyDaily15 = listInfoData[i]["vcInputQtyDaily15"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcInputQtyDaily15"].ToString());
                        int vcPlantQtyDaily15 = listInfoData[i]["vcPlantQtyDaily15"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcPlantQtyDaily15"].ToString());
                        int vcResultQtyDaily15 = listInfoData[i]["vcResultQtyDaily15"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcResultQtyDaily15"].ToString());

                        int vcInputQtyDaily16 = listInfoData[i]["vcInputQtyDaily16"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcInputQtyDaily16"].ToString());
                        int vcPlantQtyDaily16 = listInfoData[i]["vcPlantQtyDaily16"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcPlantQtyDaily16"].ToString());
                        int vcResultQtyDaily16 = listInfoData[i]["vcResultQtyDaily16"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcResultQtyDaily16"].ToString());

                        int vcInputQtyDaily17 = listInfoData[i]["vcInputQtyDaily17"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcInputQtyDaily17"].ToString());
                        int vcPlantQtyDaily17 = listInfoData[i]["vcPlantQtyDaily17"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcPlantQtyDaily17"].ToString());
                        int vcResultQtyDaily17 = listInfoData[i]["vcResultQtyDaily17"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcResultQtyDaily17"].ToString());

                        int vcInputQtyDaily18 = listInfoData[i]["vcInputQtyDaily18"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcInputQtyDaily18"].ToString());
                        int vcPlantQtyDaily18 = listInfoData[i]["vcPlantQtyDaily18"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcPlantQtyDaily18"].ToString());
                        int vcResultQtyDaily18 = listInfoData[i]["vcResultQtyDaily18"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcResultQtyDaily18"].ToString());

                        int vcInputQtyDaily19 = listInfoData[i]["vcInputQtyDaily19"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcInputQtyDaily19"].ToString());
                        int vcPlantQtyDaily19 = listInfoData[i]["vcPlantQtyDaily19"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcPlantQtyDaily19"].ToString());
                        int vcResultQtyDaily19 = listInfoData[i]["vcResultQtyDaily19"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcResultQtyDaily19"].ToString());

                        int vcInputQtyDaily20 = listInfoData[i]["vcInputQtyDaily20"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcInputQtyDaily20"].ToString());
                        int vcPlantQtyDaily20 = listInfoData[i]["vcPlantQtyDaily20"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcPlantQtyDaily20"].ToString());
                        int vcResultQtyDaily20 = listInfoData[i]["vcResultQtyDaily20"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcResultQtyDaily20"].ToString());

                        int vcInputQtyDaily21 = listInfoData[i]["vcInputQtyDaily21"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcInputQtyDaily21"].ToString());
                        int vcPlantQtyDaily21 = listInfoData[i]["vcPlantQtyDaily21"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcPlantQtyDaily21"].ToString());
                        int vcResultQtyDaily21 = listInfoData[i]["vcResultQtyDaily21"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcResultQtyDaily21"].ToString());

                        int vcInputQtyDaily22 = listInfoData[i]["vcInputQtyDaily22"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcInputQtyDaily22"].ToString());
                        int vcPlantQtyDaily22 = listInfoData[i]["vcPlantQtyDaily22"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcPlantQtyDaily22"].ToString());
                        int vcResultQtyDaily22 = listInfoData[i]["vcResultQtyDaily22"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcResultQtyDaily22"].ToString());

                        int vcInputQtyDaily23 = listInfoData[i]["vcInputQtyDaily23"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcInputQtyDaily23"].ToString());
                        int vcPlantQtyDaily23 = listInfoData[i]["vcPlantQtyDaily23"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcPlantQtyDaily23"].ToString());
                        int vcResultQtyDaily23 = listInfoData[i]["vcResultQtyDaily23"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcResultQtyDaily23"].ToString());
                        int vcInputQtyDaily24 = listInfoData[i]["vcInputQtyDaily24"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcInputQtyDaily24"].ToString());
                        int vcPlantQtyDaily24 = listInfoData[i]["vcPlantQtyDaily24"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcPlantQtyDaily24"].ToString());
                        int vcResultQtyDaily24 = listInfoData[i]["vcResultQtyDaily24"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcResultQtyDaily24"].ToString());

                        int vcInputQtyDaily25 = listInfoData[i]["vcInputQtyDaily25"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcInputQtyDaily25"].ToString());
                        int vcPlantQtyDaily25 = listInfoData[i]["vcPlantQtyDaily25"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcPlantQtyDaily25"].ToString());
                        int vcResultQtyDaily25 = listInfoData[i]["vcResultQtyDaily25"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcResultQtyDaily25"].ToString());
                        int vcInputQtyDaily26 = listInfoData[i]["vcInputQtyDaily26"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcInputQtyDaily26"].ToString());
                        int vcPlantQtyDaily26 = listInfoData[i]["vcPlantQtyDaily26"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcPlantQtyDaily26"].ToString());
                        int vcResultQtyDaily26 = listInfoData[i]["vcResultQtyDaily26"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcResultQtyDaily26"].ToString());

                        int vcInputQtyDaily27 = listInfoData[i]["vcInputQtyDaily27"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcInputQtyDaily27"].ToString());
                        int vcPlantQtyDaily27 = listInfoData[i]["vcPlantQtyDaily27"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcPlantQtyDaily27"].ToString());
                        int vcResultQtyDaily27 = listInfoData[i]["vcResultQtyDaily27"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcResultQtyDaily27"].ToString());
                        int vcInputQtyDaily28 = listInfoData[i]["vcInputQtyDaily28"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcInputQtyDaily28"].ToString());
                        int vcPlantQtyDaily28 = listInfoData[i]["vcPlantQtyDaily28"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcPlantQtyDaily28"].ToString());
                        int vcResultQtyDaily28 = listInfoData[i]["vcResultQtyDaily28"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcResultQtyDaily28"].ToString());

                        int vcInputQtyDaily29 = listInfoData[i]["vcInputQtyDaily29"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcInputQtyDaily29"].ToString());
                        int vcPlantQtyDaily29 = listInfoData[i]["vcPlantQtyDaily29"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcPlantQtyDaily29"].ToString());
                        int vcResultQtyDaily29 = listInfoData[i]["vcResultQtyDaily29"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcResultQtyDaily29"].ToString());
                        int vcInputQtyDaily30 = listInfoData[i]["vcInputQtyDaily30"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcInputQtyDaily30"].ToString());
                        int vcPlantQtyDaily30 = listInfoData[i]["vcPlantQtyDaily30"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcPlantQtyDaily30"].ToString());
                        int vcResultQtyDaily30 = listInfoData[i]["vcResultQtyDaily30"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcResultQtyDaily30"].ToString());
                        int vcInputQtyDaily31 = listInfoData[i]["vcInputQtyDaily31"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcInputQtyDaily31"].ToString());
                        int vcPlantQtyDaily31 = listInfoData[i]["vcPlantQtyDaily31"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcPlantQtyDaily31"].ToString());
                        int vcResultQtyDaily31 = listInfoData[i]["vcResultQtyDaily31"].ToString() == "" ? 0 : int.Parse(listInfoData[i]["vcResultQtyDaily31"].ToString());
                        #endregion

                        #region
                        if (vcInputQtyDaily1 > vcPlantQtyDaily1 || vcResultQtyDaily1>vcPlantQtyDaily1)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "编辑的订单号"+ listInfoData[i]["vcOrderNo"]+"对应的品番"+ listInfoData[i]["vcPartNo"]+ "日期为1日数量维护有误,发货数≤到货数≤订货数！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        if (vcInputQtyDaily2 > vcPlantQtyDaily2 || vcResultQtyDaily2 > vcPlantQtyDaily2)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "编辑的订单号" + listInfoData[i]["vcOrderNo"] + "对应的品番" + listInfoData[i]["vcPartNo"] + "日期为2日数量维护有误,发货数≤到货数≤订货数！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        if (vcInputQtyDaily3 > vcPlantQtyDaily3 || vcResultQtyDaily3 > vcPlantQtyDaily3)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "编辑的订单号" + listInfoData[i]["vcOrderNo"] + "对应的品番" + listInfoData[i]["vcPartNo"] + "日期为3日数量维护有误,发货数≤到货数≤订货数！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        if (vcInputQtyDaily4 > vcPlantQtyDaily4 || vcResultQtyDaily4 > vcPlantQtyDaily4)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "编辑的订单号" + listInfoData[i]["vcOrderNo"] + "对应的品番" + listInfoData[i]["vcPartNo"] + "日期为4日数量维护有误,发货数≤到货数≤订货数！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        if (vcInputQtyDaily5 > vcPlantQtyDaily5 || vcResultQtyDaily5 > vcPlantQtyDaily5)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "编辑的订单号" + listInfoData[i]["vcOrderNo"] + "对应的品番" + listInfoData[i]["vcPartNo"] + "日期为5日数量维护有误,发货数≤到货数≤订货数！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        if (vcInputQtyDaily6 > vcPlantQtyDaily6 || vcResultQtyDaily6 > vcPlantQtyDaily6)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "编辑的订单号" + listInfoData[i]["vcOrderNo"] + "对应的品番" + listInfoData[i]["vcPartNo"] + "日期为6日数量维护有误,发货数≤到货数≤订货数！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        if (vcInputQtyDaily7 > vcPlantQtyDaily7 || vcResultQtyDaily7 > vcPlantQtyDaily7)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "编辑的订单号" + listInfoData[i]["vcOrderNo"] + "对应的品番" + listInfoData[i]["vcPartNo"] + "日期为7日数量维护有误,发货数≤到货数≤订货数！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        if (vcInputQtyDaily8 > vcPlantQtyDaily8 || vcResultQtyDaily8 > vcPlantQtyDaily8)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "编辑的订单号" + listInfoData[i]["vcOrderNo"] + "对应的品番" + listInfoData[i]["vcPartNo"] + "日期为8日数量维护有误,发货数≤到货数≤订货数！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        if (vcInputQtyDaily9 > vcPlantQtyDaily9 || vcResultQtyDaily9 > vcPlantQtyDaily9)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "编辑的订单号" + listInfoData[i]["vcOrderNo"] + "对应的品番" + listInfoData[i]["vcPartNo"] + "日期为9日数量维护有误,发货数≤到货数≤订货数！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        if (vcInputQtyDaily10 > vcPlantQtyDaily10 || vcResultQtyDaily10 > vcPlantQtyDaily10)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "编辑的订单号" + listInfoData[i]["vcOrderNo"] + "对应的品番" + listInfoData[i]["vcPartNo"] + "日期为10日数量维护有误,发货数≤到货数≤订货数！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        if (vcInputQtyDaily11 > vcPlantQtyDaily11 || vcResultQtyDaily11 > vcPlantQtyDaily11)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "编辑的订单号" + listInfoData[i]["vcOrderNo"] + "对应的品番" + listInfoData[i]["vcPartNo"] + "日期为11日数量维护有误,发货数≤到货数≤订货数！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        if (vcInputQtyDaily12 > vcPlantQtyDaily12 || vcResultQtyDaily12 > vcPlantQtyDaily12)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "编辑的订单号" + listInfoData[i]["vcOrderNo"] + "对应的品番" + listInfoData[i]["vcPartNo"] + "日期为12日数量维护有误,发货数≤到货数≤订货数！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        if (vcInputQtyDaily13 > vcPlantQtyDaily13 || vcResultQtyDaily13 > vcPlantQtyDaily13)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "编辑的订单号" + listInfoData[i]["vcOrderNo"] + "对应的品番" + listInfoData[i]["vcPartNo"] + "日期为13日数量维护有误,发货数≤到货数≤订货数！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        if (vcInputQtyDaily14 > vcPlantQtyDaily14 || vcResultQtyDaily14 > vcPlantQtyDaily14)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "编辑的订单号" + listInfoData[i]["vcOrderNo"] + "对应的品番" + listInfoData[i]["vcPartNo"] + "日期为14日数量维护有误,发货数≤到货数≤订货数！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        if (vcInputQtyDaily15 > vcPlantQtyDaily15 || vcResultQtyDaily15 > vcPlantQtyDaily15)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "编辑的订单号" + listInfoData[i]["vcOrderNo"] + "对应的品番" + listInfoData[i]["vcPartNo"] + "日期为15日数量维护有误,发货数≤到货数≤订货数！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        if (vcInputQtyDaily16 > vcPlantQtyDaily16 || vcResultQtyDaily16 > vcPlantQtyDaily16)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "编辑的订单号" + listInfoData[i]["vcOrderNo"] + "对应的品番" + listInfoData[i]["vcPartNo"] + "日期为16日数量维护有误,发货数≤到货数≤订货数！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        if (vcInputQtyDaily17 > vcPlantQtyDaily17 || vcResultQtyDaily17 > vcPlantQtyDaily17)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "编辑的订单号" + listInfoData[i]["vcOrderNo"] + "对应的品番" + listInfoData[i]["vcPartNo"] + "日期为17日数量维护有误,发货数≤到货数≤订货数！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        if (vcInputQtyDaily18 > vcPlantQtyDaily18 || vcResultQtyDaily18 > vcPlantQtyDaily18)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "编辑的订单号" + listInfoData[i]["vcOrderNo"] + "对应的品番" + listInfoData[i]["vcPartNo"] + "日期为18日数量维护有误,发货数≤到货数≤订货数！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        if (vcInputQtyDaily19 > vcPlantQtyDaily19 || vcResultQtyDaily19 > vcPlantQtyDaily19)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "编辑的订单号" + listInfoData[i]["vcOrderNo"] + "对应的品番" + listInfoData[i]["vcPartNo"] + "日期为19日数量维护有误,发货数≤到货数≤订货数！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        if (vcInputQtyDaily20 > vcPlantQtyDaily20 || vcResultQtyDaily20 > vcPlantQtyDaily20)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "编辑的订单号" + listInfoData[i]["vcOrderNo"] + "对应的品番" + listInfoData[i]["vcPartNo"] + "日期为20日数量维护有误,发货数≤到货数≤订货数！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        if (vcInputQtyDaily21 > vcPlantQtyDaily21 || vcResultQtyDaily21 > vcPlantQtyDaily21)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "编辑的订单号" + listInfoData[i]["vcOrderNo"] + "对应的品番" + listInfoData[i]["vcPartNo"] + "日期为21日数量维护有误,发货数≤到货数≤订货数！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        if (vcInputQtyDaily22 > vcPlantQtyDaily22 || vcResultQtyDaily22 > vcPlantQtyDaily22)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "编辑的订单号" + listInfoData[i]["vcOrderNo"] + "对应的品番" + listInfoData[i]["vcPartNo"] + "日期为22日数量维护有误,发货数≤到货数≤订货数！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        if (vcInputQtyDaily23 > vcPlantQtyDaily23 || vcResultQtyDaily23 > vcPlantQtyDaily23)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "编辑的订单号" + listInfoData[i]["vcOrderNo"] + "对应的品番" + listInfoData[i]["vcPartNo"] + "日期为23日数量维护有误,发货数≤到货数≤订货数！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        if (vcInputQtyDaily24 > vcPlantQtyDaily24 || vcResultQtyDaily24 > vcPlantQtyDaily24)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "编辑的订单号" + listInfoData[i]["vcOrderNo"] + "对应的品番" + listInfoData[i]["vcPartNo"] + "日期为24日数量维护有误,发货数≤到货数≤订货数！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        if (vcInputQtyDaily25 > vcPlantQtyDaily25 || vcResultQtyDaily25 > vcPlantQtyDaily25)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "编辑的订单号" + listInfoData[i]["vcOrderNo"] + "对应的品番" + listInfoData[i]["vcPartNo"] + "日期为25日数量维护有误,发货数≤到货数≤订货数！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        if (vcInputQtyDaily26 > vcPlantQtyDaily26 || vcResultQtyDaily26 > vcPlantQtyDaily26)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "编辑的订单号" + listInfoData[i]["vcOrderNo"] + "对应的品番" + listInfoData[i]["vcPartNo"] + "日期为26日数量维护有误,发货数≤到货数≤订货数！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        if (vcInputQtyDaily27 > vcPlantQtyDaily27 || vcResultQtyDaily27 > vcPlantQtyDaily27)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "编辑的订单号" + listInfoData[i]["vcOrderNo"] + "对应的品番" + listInfoData[i]["vcPartNo"] + "日期为27日数量维护有误,发货数≤到货数≤订货数！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        if (vcInputQtyDaily28 > vcPlantQtyDaily28 || vcResultQtyDaily28 > vcPlantQtyDaily28)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "编辑的订单号" + listInfoData[i]["vcOrderNo"] + "对应的品番" + listInfoData[i]["vcPartNo"] + "日期为28日数量维护有误,发货数≤到货数≤订货数！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        if (vcInputQtyDaily29 > vcPlantQtyDaily29 || vcResultQtyDaily29 > vcPlantQtyDaily29)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "编辑的订单号" + listInfoData[i]["vcOrderNo"] + "对应的品番" + listInfoData[i]["vcPartNo"] + "日期为29日数量维护有误,发货数≤到货数≤订货数！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        if (vcInputQtyDaily30 > vcPlantQtyDaily30 || vcResultQtyDaily30 > vcPlantQtyDaily30)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "编辑的订单号" + listInfoData[i]["vcOrderNo"] + "对应的品番" + listInfoData[i]["vcPartNo"] + "日期为30日数量维护有误,发货数≤到货数≤订货数！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        if (vcInputQtyDaily31 > vcPlantQtyDaily31 || vcResultQtyDaily31 > vcPlantQtyDaily31)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "编辑的订单号" + listInfoData[i]["vcOrderNo"] + "对应的品番" + listInfoData[i]["vcPartNo"] + "日期为31日数量维护有误,发货数≤到货数≤订货数！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        
                        #endregion
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
                for (int i=0;i<listInfoData.Count;i++)
                { 
                    if(Convert.ToDecimal(listInfoData[i]["vcInputQtyDailySum"])>0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "月总到货数大于0,不能删除！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (Convert.ToDecimal(listInfoData[i]["vcResultQtyDailySum"]) > 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "月总发货数大于0,不能删除！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
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

