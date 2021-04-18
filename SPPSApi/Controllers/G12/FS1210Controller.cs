using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using System.Threading;
using Common;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using Logic;

namespace SPPSApi.Controllers.G12
{
    [Route("api/FS1210/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1210Controller : BaseController
    {
        FS1209_Logic logic_09 = new FS1209_Logic();
        FS1210_Logic logic = new FS1210_Logic();
        PrinterCR print = new PrinterCR();
        string gud = "";
        string ls_fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + Guid.NewGuid().ToString().Replace("-", "") + ".png";
        private readonly string FunctionID = "FS1210";
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FS1210Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        #region 页面初始化Api
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
                FS1209_Logic logic_1 = new FS1209_Logic();
                List<object> dataList_PorPlant = ComFunction.convertAllToResult(ComFunction.getTCode("C000"));
                string RolePorType = logic_1.getRoleTip(loginInfo.UserId);
                DataTable dtportype = logic_1.dllPorType(RolePorType.Split('*'));
                List<object> dataList_PorType = ComFunction.convertAllToResult(dtportype);
                Dictionary<string, object> res = new Dictionary<string, object>();
                res.Add("dataList_PorPlant", dataList_PorPlant);
                res.Add("dataList_PorType", dataList_PorType);
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

        #region 检索Api
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
            try
            {
                #region 检索
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                string vcKbOrderId = dataForm.vcKbOrderId == null ? "" : dataForm.vcKbOrderId;
                string vcTF = dataForm.vcTF == null ? "" : dataForm.vcTF;
                string vcFBZ = dataForm.vcFBZ == null ? "" : dataForm.vcFBZ;
                string vcTT = dataForm.vcTT == null ? "" : dataForm.vcTT;
                string vcTBZ = dataForm.vcTBZ == null ? "" : dataForm.vcTBZ;
                string vcPartsNo = dataForm.vcPartsNo == null ? "" : dataForm.vcPartsNo;
                string vcPartsNo_ = vcPartsNo.Replace("-", "");
                string vcCarType = dataForm.vcCarType == null ? "" : dataForm.vcCarType;
                string vcGC = dataForm.vcGC == null ? "" : dataForm.vcGC;
                string vcType = dataForm.vcType == null ? "" : dataForm.vcType;
                string vcPlant = dataForm.vcPlant == null ? "" : dataForm.vcPlant;
                DataTable DataPorType = getDataPorType(loginInfo.UserId);
                DataTable dt = getData(vcKbOrderId, vcTF, vcFBZ, vcTT, vcTBZ, vcPartsNo, vcCarType, vcGC, vcType, vcPlant, DataPorType);
                #endregion

                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                List<object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 打印Api
        [HttpPost]
        [EnableCors("any")]
        public string printApi([FromBody] dynamic data)
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
            string vcType = dataForm.vcType == null ? "" : dataForm.vcType;
            JArray checkedInfo = dataForm._multipleSelection;
            List<Dictionary<string, object>> listInfoData = checkedInfo.ToObject<List<Dictionary<string, object>>>();
            if (vcType == "A")
            {
                if (listInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请选择要打印的数据行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
            }
            else
            {
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "请选择全部打印按钮进行打印！";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }

            DataTable dtPorType = new DataTable();
            string msg = string.Empty;
            try
            {
                string tmplatePath = "\\Template\\FS160170.xlt";//看板投放确认单Excel模板
                string ls_fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + Guid.NewGuid().ToString().Replace("-", "") + ".png";
                string strPrinterName = logic.PrintMess(loginInfo.UserId);//获取打印机
                string vcFlagZ = "";
                string picnull = _webHostEnvironment.ContentRootPath + "Doc\\Image\\SPPartImage\\picnull.JPG";
                byte[] vcPhotoPath = print.PhotoToArray("", picnull);//照片初始化
                DataTable dtPrintCR = new DataTable();
                DataTable dtPrintCRLone = print.searchTBCreate();//获取表testprinterCR结构，为打印数据填充提供DataTable
                DataTable dtPrint = dtPrintCRLone.Clone();//创建看板打印DataTable
                DataTable exdt = logic.CreatDataTable();//创建看板投放确认单Excel打印DataTable
                bool check = true;
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    string vcSupplierCode = ""; string vcSupplierPlant = ""; string vcCpdCompany = ""; string vcPartsNameEN = ""; string vcPartsNameCHN = "";
                    string vcLogisticRoute = ""; string iQuantityPerContainer = "";
                    string vcProject01 = ""; string vcComDate01 = ""; string vcBanZhi01 = ""; string vcAB01 = "";
                    string vcProject02 = ""; string vcComDate02 = ""; string vcBanZhi02 = ""; string vcAB02 = "";
                    string vcProject03 = ""; string vcComDate03 = ""; string vcBanZhi03 = ""; string vcAB03 = "";
                    string vcProject04 = ""; string vcComDate04 = ""; string vcBanZhi04 = ""; string vcAB04 = "";
                    string vcRemark1 = ""; string vcRemark2 = "";
                    string PorType = ""; string vcKBorser = "";
                    string vcComDate00 = ""; string vcBanZhi00 = "";
                    DataTable dtKANB = new DataTable();
                    #region 整理数据
                    string vcPartsNo = listInfoData[i]["vcPartsNo"].ToString(); //品番
                    string vcDock = listInfoData[i]["vcDock"].ToString(); //受入
                    string vcCarFamilyCode = listInfoData[i]["vcCarType"].ToString(); //车型
                    string vcEDflag = listInfoData[i]["jinjiqufen"].ToString(); //紧急区分
                    if (vcEDflag == "通常")
                    {
                        vcEDflag = "S";
                    }
                    else if (vcEDflag == "紧急")
                    {
                        vcEDflag = "E";
                    }
                    string vcKBorderno = listInfoData[i]["vcKBorderno"].ToString(); //看板订单号
                    string vcKBSerial = listInfoData[i]["vcKBSerial"].ToString(); //连番
                    string vcPlanMonth = listInfoData[i]["vcPlanMonth"].ToString();
                    string vcNo = listInfoData[i]["iNo"].ToString();
                    string vcPorType = listInfoData[i]["vcPorType"].ToString();

                    //标记再打或者提前延迟打印区分是否打印Excel表
                    #region 整理数据 判断打印类型
                    check = logic.IfPrintKB(vcNo);
                    if (check)//已经打印过 属于再发行
                    {
                        DataTable QFED = logic.QFED00QuFen(vcPartsNo, vcDock, vcKBorderno, vcKBSerial, vcPlanMonth, vcNo);
                        if (QFED.Rows.Count != 0)
                        {
                            if (QFED.Rows[0]["iBaiJianFlag"].ToString() == "1" && vcPartsNo.Substring(10, 2) != "ED")//秦丰非ED再发行
                            {
                                dtKANB = logic.rePrintDataED(vcPartsNo, vcDock, vcPlanMonth, vcKBorderno, vcKBSerial, vcNo, vcCarFamilyCode);
                            }
                            else//秦丰ED再发行//非秦丰再发行
                            {
                                dtKANB = logic.rePrintData(vcPartsNo, vcDock, vcPlanMonth, vcKBorderno, vcKBSerial, vcNo);
                            }
                        }
                        vcFlagZ = "Z";
                    }
                    else//未打印过非再发行
                    {
                        dtKANB = logic.GetPrintFZData(vcPartsNo, vcDock, vcKBorderno, vcKBSerial, vcPlanMonth, vcNo);
                        vcFlagZ = "TY";
                    }
                    #endregion
                    if (dtKANB.Rows.Count != 0)
                    {
                        #region 取非空数据
                        vcPhotoPath = print.PhotoToArray(dtKANB.Rows[0]["vcPhotoPath"].ToString(), picnull);//图片二进制流
                        vcSupplierCode = dtKANB.Rows[0]["vcSupplierCode"].ToString();//供应商
                        vcSupplierPlant = dtKANB.Rows[0]["vcSupplierPlant"].ToString();//供应商工区
                        vcCpdCompany = dtKANB.Rows[0]["vcCpdCompany"].ToString();//收货方
                        vcPartsNameEN = dtKANB.Rows[0]["vcPartsNameEN"].ToString();//中英文品名
                        vcPartsNameCHN = dtKANB.Rows[0]["vcPartsNameCHN"].ToString();
                        vcLogisticRoute = dtKANB.Rows[0]["vcLogisticRoute"].ToString();//路径
                        iQuantityPerContainer = dtKANB.Rows[0]["iQuantityPerContainer"].ToString();//收容数
                                                                                                   //工程、完成日、班值
                        vcComDate00 = dtKANB.Rows[0]["vcComDate00"].ToString();
                        vcBanZhi00 = dtKANB.Rows[0]["vcBanZhi00"].ToString();
                        vcProject01 = dtKANB.Rows[0]["vcProject01"].ToString();
                        vcComDate01 = dtKANB.Rows[0]["vcComDate01"].ToString();
                        vcBanZhi01 = dtKANB.Rows[0]["vcBanZhi01"].ToString();
                        vcAB01 = dtKANB.Rows[0]["vcAB01"].ToString();//20181010添加AB值信息 - 李兴旺
                        vcProject02 = dtKANB.Rows[0]["vcProject02"].ToString();
                        vcComDate02 = dtKANB.Rows[0]["vcComDate02"].ToString();
                        vcBanZhi02 = dtKANB.Rows[0]["vcBanZhi02"].ToString();
                        vcAB02 = dtKANB.Rows[0]["vcAB02"].ToString();//20181010添加AB值信息 - 李兴旺
                        vcProject03 = dtKANB.Rows[0]["vcProject03"].ToString();
                        vcComDate03 = dtKANB.Rows[0]["vcComDate03"].ToString();
                        vcBanZhi03 = dtKANB.Rows[0]["vcBanZhi03"].ToString();
                        vcAB03 = dtKANB.Rows[0]["vcAB03"].ToString();//20181010添加AB值信息 - 李兴旺
                        vcProject04 = dtKANB.Rows[0]["vcProject04"].ToString();
                        vcComDate04 = dtKANB.Rows[0]["vcComDate04"].ToString();
                        vcBanZhi04 = dtKANB.Rows[0]["vcBanZhi04"].ToString();
                        vcAB04 = dtKANB.Rows[0]["vcAB04"].ToString();//20181010添加AB值信息 - 李兴旺
                        vcRemark1 = dtKANB.Rows[0]["vcRemark1"].ToString();//特记事项
                        vcRemark2 = dtKANB.Rows[0]["vcRemark2"].ToString();
                        PorType = vcPorType;
                        vcKBorser = vcKBorderno + vcDock;
                        #endregion
                        string QuantityPerContainerFED = logic_09.resQuantityPerContainer(vcPartsNo, vcDock, vcPlanMonth);
                        if (QuantityPerContainerFED != iQuantityPerContainer)
                        {
                            DataTable dtKBhistory = logic.dtKBSerial_history(vcPartsNo, vcDock, vcKBorderno, vcKBSerial);
                            for (int a = 0; a < dtKBhistory.Rows.Count; a++)
                            {
                                gud = Guid.NewGuid().ToString("N");
                                iQuantityPerContainer = (Convert.ToInt32(QuantityPerContainerFED) > Convert.ToInt32(iQuantityPerContainer)) ? Convert.ToString((Convert.ToInt32(iQuantityPerContainer))) : Convert.ToString((Convert.ToInt32(QuantityPerContainerFED)));
                                dtPrint = dtPrintRE(vcSupplierCode, vcCpdCompany, vcCarFamilyCode, vcPartsNo, vcPartsNameEN, vcPartsNameCHN, vcLogisticRoute, iQuantityPerContainer, vcProject01, vcComDate01, vcBanZhi01, vcProject02, vcComDate02, vcBanZhi02, vcProject03, vcComDate03, vcBanZhi03, vcProject04, vcComDate04, vcBanZhi04, vcRemark1, vcRemark2, dtKBhistory.Rows[a]["vcKBSerial"].ToString(), vcPhotoPath, vcDock, vcKBorser, gud, vcKBorderno, PorType, vcEDflag, vcSupplierPlant, dtPrint, vcComDate00, vcBanZhi00, vcPlanMonth, vcAB01, vcAB02, vcAB03, vcAB04);
                                if (vcFlagZ == "TY")
                                {
                                    DataRow exrow = exdt.NewRow();
                                    exrow[0] = vcPartsNo;
                                    exrow[1] = vcCarFamilyCode;
                                    exrow[2] = vcPartsNameCHN;
                                    exrow[3] = vcProject01;
                                    exrow[4] = vcComDate01;
                                    exrow[5] = vcBanZhi01;
                                    exrow[6] = "1";//计数
                                    exrow[7] = PorType;
                                    exrow[8] = vcKBorderno;
                                    exrow[9] = dtKBhistory.Rows[a]["vcKBSerial"].ToString();
                                    exrow[10] = vcComDate00;
                                    exrow[11] = vcBanZhi00;
                                    exdt.Rows.Add(exrow);
                                }
                            }
                        }
                        else
                        {
                            gud = Guid.NewGuid().ToString("N");
                            dtPrint = dtPrintRE(vcSupplierCode, vcCpdCompany, vcCarFamilyCode, vcPartsNo, vcPartsNameEN, vcPartsNameCHN, vcLogisticRoute, iQuantityPerContainer, vcProject01, vcComDate01, vcBanZhi01, vcProject02, vcComDate02, vcBanZhi02, vcProject03, vcComDate03, vcBanZhi03, vcProject04, vcComDate04, vcBanZhi04, vcRemark1, vcRemark2, vcKBSerial, vcPhotoPath, vcDock, vcKBorser, gud, vcKBorderno, PorType, vcEDflag, vcSupplierPlant, dtPrint, vcComDate00, vcBanZhi00, vcPlanMonth, vcAB01, vcAB02, vcAB03, vcAB04);
                            if (vcFlagZ == "TY")
                            {
                                DataRow exrow = exdt.NewRow();
                                exrow[0] = vcPartsNo;
                                exrow[1] = vcCarFamilyCode;
                                exrow[2] = vcPartsNameCHN;
                                exrow[3] = vcProject01;
                                exrow[4] = vcComDate01;
                                exrow[5] = vcBanZhi01;
                                exrow[6] = "1";//计数
                                exrow[7] = PorType;
                                exrow[8] = vcKBorderno;
                                exrow[9] = vcKBSerial;
                                exrow[10] = vcComDate00;
                                exrow[11] = vcBanZhi00;
                                exdt.Rows.Add(exrow);
                            }
                        }
                    }
                    else
                    {
                        #region 打印空数据
                        gud = Guid.NewGuid().ToString("N");
                        dtPrint = dtPrintRE(vcSupplierCode, vcCpdCompany, vcCarFamilyCode, vcPartsNo, vcPartsNameEN, vcPartsNameCHN, vcLogisticRoute, iQuantityPerContainer, vcProject01, vcComDate01, vcBanZhi01, vcProject02, vcComDate02, vcBanZhi02, vcProject03, vcComDate03, vcBanZhi03, vcProject04, vcComDate04, vcBanZhi04, vcRemark1, vcRemark2, vcKBSerial, vcPhotoPath, vcDock, vcKBorser, gud, vcKBorderno, PorType, vcEDflag, vcSupplierPlant, dtPrint, vcComDate00, vcBanZhi00, vcPlanMonth, vcAB01, vcAB02, vcAB03, vcAB04);
                        if (vcFlagZ == "TY")
                        {
                            DataRow exrow = exdt.NewRow();
                            exrow[0] = vcPartsNo;
                            exrow[1] = vcCarFamilyCode;
                            exrow[2] = vcPartsNameCHN;
                            exrow[3] = vcProject01;
                            exrow[4] = vcComDate01;
                            exrow[5] = vcBanZhi01;
                            exrow[6] = "1";//计数
                            exrow[7] = PorType;
                            exrow[8] = vcKBorderno;
                            exrow[9] = vcKBSerial;
                            exrow[10] = vcComDate00;
                            exrow[11] = vcBanZhi00;
                            exdt.Rows.Add(exrow);
                        }
                        #endregion
                    }
                    #endregion
                }
                #region 打印处理
                dtPrint = print.orderDataTable(dtPrint);//排序
                logic.insertTableCR(dtPrint);//插入打印临时子表
                logic.insertTableExcel00(exdt);//插入看板确认单Excel
                                               //DataTable dtPorType = print.searchPorType00();//取生产部署
                                               //六项数据
                dtPorType = logic.QueryGroup(dtPrint);//用订单号 生产部署 生产日期 生产班值分组,修改不在数据库中取值
                print.insertTableCRMain00(dtPrint, dtPorType);//插入打印临时主表
                //string printDay = "";
                //logic_09.KanBIfPrintDay();//获取班值信息
                string reportPath = "CrReport.rpt";
                string strLoginId = loginInfo.UserId;
                for (int z = 0; z < dtPorType.Rows.Count; z++)
                {
                    //DataTable exdtt = exdt.Clone();
                    DataTable exdttt = new DataTable();
                    DataTable exdthj = new DataTable();
                    //exdttt.Clear();
                    string vcPorType = dtPorType.Rows[z]["vcPorType"].ToString();
                    string vcorderno = dtPorType.Rows[z]["vcorderno"].ToString();
                    string vcComDate01 = dtPorType.Rows[z]["vcComDate01"].ToString();
                    string vcBanZhi01 = dtPorType.Rows[z]["vcBanZhi01"].ToString();
                    string vcComDate00 = dtPorType.Rows[z]["vcComDate00"].ToString();
                    string vcBanZhi00 = dtPorType.Rows[z]["vcBanZhi00"].ToString();

                    //bool retb = print.printCr(reportPath, vcPorType, vcorderno, vcComDate01, vcBanZhi01, vcComDate00, vcBanZhi00, strLoginId, strPrinterName);//打印水晶报表
                    msg = print.printCr(reportPath, vcPorType, vcorderno, vcComDate01, vcBanZhi01, vcComDate00, vcBanZhi00, strLoginId, strPrinterName);//打印水晶报表
                    if (msg == "打印成功")
                    {
                        DataSet ds = logic.PrintExcel(vcPorType, vcorderno, vcComDate01, vcBanZhi01, vcComDate00, vcBanZhi00);
                        exdttt = ds.Tables[0];
                        if (exdttt.Rows.Count != 0)
                        {
                            #region Excel打印
                            exdthj = ds.Tables[1];
                            for (int p = 0; p < exdttt.Rows.Count; p++)
                            {
                                exdttt.Rows[p]["no"] = p + 1;
                            }
                            int dsRowsCount = exdttt.Rows.Count;
                            int dsRow = dsRowsCount / 43;
                            int dsrows = dsRowsCount % 43;
                            //总页数
                            int pagetotle = 0;
                            //页数
                            int pageno = 0;
                            if (dsRow != 0)
                            {
                                pagetotle = ((dsrows + exdthj.Rows.Count + 3) / 43) == 0 ? (dsRow + 1) : (dsRow + 1 + (((exdthj.Rows.Count + 3) / 43) == 0 ? 1 : ((exdthj.Rows.Count + 3) / 43) + 1));
                            }
                            else
                            {
                                pagetotle = ((dsRowsCount + exdthj.Rows.Count + 3) / 43) == 0 ? 1 : (1 + (((exdthj.Rows.Count + 3) / 43) == 0 ? 1 : ((exdthj.Rows.Count + 3) / 43) + 1));
                            }
                            if (dsRow > 0)
                            {
                                DataTable inTable = exdttt.Clone();
                                for (int i = 0; i < exdttt.Rows.Count; i++)
                                {
                                    DataRow dr = exdttt.Rows[i];
                                    DataRow add = inTable.NewRow();
                                    add.ItemArray = dr.ItemArray;
                                    inTable.Rows.Add(add);
                                    if (inTable.Rows.Count >= 43 || exdttt.Rows.Count - 1 == i)
                                    {
                                        pageno = pageno + 1;
                                        string pageB = "0";
                                        if (inTable.Rows.Count <= 43)
                                        {
                                            pageB = inTable.Rows.Count + exdthj.Rows.Count + 3 <= 43 ? "1" : "2";
                                        }
                                        //打印传值有：订单号、生产部署、计划打印日期 printIme、计划打印班值 printDay 、计划生产日期 、计划生产班值、页码
                                        //exprint.PrintTemplateFromDataTable(inTable, exdthj, tmplatePath, vcorderno, vcPorType, strLoginId, vcComDate00, vcBanZhi00 == "白" ? "白值" : "夜值", vcComDate01, vcBanZhi01 == "白" ? "白值" : "夜值", strPrinterName, Convert.ToString(pagetotle), Convert.ToString(pageno), pageB);

                                        #region 创建打印临时表并打印 WebService
                                        FS1209_Logic lg = new FS1209_Logic();
                                        string exdthj_tmp = string.Empty;
                                        string inTable_tmp = string.Empty;
                                        string exdthj_msg = lg.CreateTempTable(exdthj, "FS1209_Excel_", out exdthj_tmp);//创建打印临时表  
                                        string inTable_msg = lg.CreateTempTable(exdttt, "FS1209_Excel_", out inTable_tmp);//创建打印临时表                 
                                        if (inTable_msg.Length == 0)
                                        {
                                            try
                                            {
                                                BasicHttpBinding binding = new BasicHttpBinding();
                                                binding.CloseTimeout = TimeSpan.MaxValue;
                                                binding.OpenTimeout = TimeSpan.MaxValue;
                                                binding.ReceiveTimeout = TimeSpan.MaxValue;
                                                binding.SendTimeout = TimeSpan.MaxValue;
                                                EndpointAddress address = new EndpointAddress("http://localhost:8089/PrintTable.asmx");
                                                PrintCR.PrintTableSoapClient client = new PrintCR.PrintTableSoapClient(binding, address);
                                                msg = client.PrintExcel_Confirmation(inTable_tmp, exdthj_tmp, tmplatePath, vcorderno, vcPorType, strLoginId, vcComDate00, vcBanZhi00 == "白" ? "白值" : "夜值", vcComDate01, vcBanZhi01 == "白" ? "白值" : "夜值", strPrinterName, Convert.ToString(pagetotle), Convert.ToString(pageno), pageB);
                                            }
                                            catch (Exception ex)
                                            {
                                                msg = "打印看板确认单失败！";
                                                throw ex;
                                            }
                                            finally
                                            {
                                                lg.DropTempTable(inTable_tmp);//删除打印临时表
                                                lg.DropTempTable(exdthj_tmp);//删除打印临时表
                                            }
                                        }
                                        #endregion
                                        inTable = exdt.Clone();
                                    }
                                }
                            }
                            else
                            {
                                string pageB = "0";
                                pageno = pageno + 1;
                                pageB = exdttt.Rows.Count + exdthj.Rows.Count + 3 <= 43 ? "1" : "2";
                                //打印传值有：订单号、生产部署、计划打印日期 printIme、计划打印班值 printDay 、计划生产日期 、计划生产班值、页码
                                //exprint.PrintTemplateFromDataTable(exdttt, exdthj, tmplatePath, vcorderno, vcPorType, strLoginId, vcComDate00, vcBanZhi00 == "白" ? "白值" : "夜值", vcComDate01, vcBanZhi01 == "白" ? "白值" : "夜值", strPrinterName, Convert.ToString(pagetotle), Convert.ToString(pageno), pageB);
                                #region 创建打印临时表并打印 WebService
                                FS1209_Logic lg = new FS1209_Logic();
                                string exdttt_tmp = string.Empty;
                                string exdthj_tmp = string.Empty;
                                string exdttt_msg = lg.CreateTempTable(exdttt, "FS1209_Excel_", out exdttt_tmp);//创建打印临时表
                                string exdthj_msg = lg.CreateTempTable(exdthj, "FS1209_Excel_", out exdthj_tmp);//创建打印临时表                
                                if (exdthj_msg.Length == 0)
                                {
                                    try
                                    {
                                        BasicHttpBinding binding = new BasicHttpBinding();
                                        binding.CloseTimeout = TimeSpan.MaxValue;
                                        binding.OpenTimeout = TimeSpan.MaxValue;
                                        binding.ReceiveTimeout = TimeSpan.MaxValue;
                                        binding.SendTimeout = TimeSpan.MaxValue;
                                        EndpointAddress address = new EndpointAddress("http://localhost:8089/PrintTable.asmx");
                                        PrintCR.PrintTableSoapClient client = new PrintCR.PrintTableSoapClient(binding, address);
                                        exdthj_msg = client.PrintExcel_Confirmation(exdttt_tmp, exdthj_tmp, tmplatePath, vcorderno, vcPorType, strLoginId, vcComDate00, vcBanZhi00 == "白" ? "白值" : "夜值", vcComDate01, vcBanZhi01 == "白" ? "白值" : "夜值", strPrinterName, Convert.ToString(pagetotle), Convert.ToString(pageno), pageB);
                                    }
                                    catch (Exception ex)
                                    {
                                        msg = "打印看板确认单失败！";
                                        throw ex;
                                    }
                                    finally
                                    {
                                        lg.DropTempTable(exdttt_tmp);//删除打印临时表
                                        lg.DropTempTable(exdthj_tmp);//删除打印临时表
                                    }
                                }
                                lg.DropTempTable(exdttt_tmp);//删除打印临时表
                                lg.DropTempTable(exdthj_tmp);//删除打印临时表
                                #endregion
                            }
                            #endregion
                        }
                        //删除看板打印的临时文件
                        logic_09.DeleteprinterCREX(vcPorType, vcorderno, vcComDate01, vcBanZhi01);
                    }
                }
                #endregion
                if (!(check))
                {
                    logic.UpdatePrintKANB(dtPrint);//更新看板打印表
                }
                if (msg == "打印成功")
                {
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.data = msg;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = msg;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
            }
            catch (Exception ex)
            {
                if (dtPorType != null)
                {
                    for (int i = 0; i < dtPorType.Rows.Count; i++)
                    {
                        logic_09.DeleteprinterCREX(dtPorType.Rows[i]["vcPorType"].ToString(), dtPorType.Rows[i]["vcorderno"].ToString(), dtPorType.Rows[i]["vcComDate01"].ToString(), dtPorType.Rows[i]["vcBanZhi01"].ToString());
                    }
                }
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = msg;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 全部打印Api
        [HttpPost]
        [EnableCors("any")]
        public string printAllApi([FromBody] dynamic data)
        {
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            ApiResult apiResult = new ApiResult();
            dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));

            DataTable dtPorType = new DataTable();
            try
            {
                #region 检索
                string vcKbOrderId = dataForm.vcKbOrderId == null ? "" : dataForm.vcKbOrderId;
                string vcTF = dataForm.vcTF == null ? "" : dataForm.vcTF;
                string vcFBZ = dataForm.vcFBZ == null ? "" : dataForm.vcFBZ;
                string vcTT = dataForm.vcTT == null ? "" : dataForm.vcTT;
                string vcTBZ = dataForm.vcTBZ == null ? "" : dataForm.vcTBZ;
                string vcPartsNo = dataForm.vcPartsNo == null ? "" : dataForm.vcPartsNo;
                string vcPartsNo_ = vcPartsNo.Replace("-", "");
                string vcCarType = dataForm.vcCarTyp == null ? "" : dataForm.vcCarTypee;
                string vcGC = dataForm.vcGC == null ? "" : dataForm.vcGC;
                string vcType = dataForm.vcType == null ? "" : dataForm.vcType;
                string vcPlant = dataForm.vcPlant == null ? "" : dataForm.vcPlant;
                DataTable DataPorType = getDataPorType(loginInfo.UserId);
                DataTable dt17 = getData(vcKbOrderId, vcTF, vcFBZ, vcTT, vcTBZ, vcPartsNo, vcCarType, vcGC, vcType, vcPlant, DataPorType);
                #endregion

                if ((dt17.Rows.Count == 0) || (dt17.Rows[0][0].ToString() == "_1"))
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "无可打印数据,请确认！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                string tmplatePath = "\\Template\\FS160170.xlt";//看板投放确认单Excel模板
                string ls_fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + Guid.NewGuid().ToString().Replace("-", "") + ".png";
                string strPrinterName = logic.PrintMess(loginInfo.UserId);//获取打印机
                string vcFlagZ = "";

                string picnull = _webHostEnvironment.ContentRootPath + "Doc\\Image\\SPPartImage\\picnull.JPG";
                byte[] vcPhotoPath = print.PhotoToArray("", picnull);
                DataTable dtPrintCR = new DataTable();
                DataTable dtPrintCRLone = print.searchTBCreate();//获得数据库表结构
                DataTable dtPrint = dtPrintCRLone.Clone();
                DataTable exdt = logic.CreatDataTable();
                bool check = true;
                //string QFlag = "2";
                print.TurnCate();
                for (int i = 0; i < dt17.Rows.Count; i++)
                {
                    string vcSupplierCode = ""; string vcSupplierPlant = ""; string vcCpdCompany = ""; string vcPartsNameEN = ""; string vcPartsNameCHN = "";
                    string vcLogisticRoute = ""; string iQuantityPerContainer = "";
                    string vcProject01 = ""; string vcComDate01 = ""; string vcBanZhi01 = ""; string vcAB01 = "";//20181010添加AB值信息 - 李兴旺
                    string vcProject02 = ""; string vcComDate02 = ""; string vcBanZhi02 = ""; string vcAB02 = "";//20181010添加AB值信息 - 李兴旺
                    string vcProject03 = ""; string vcComDate03 = ""; string vcBanZhi03 = ""; string vcAB03 = "";//20181010添加AB值信息 - 李兴旺
                    string vcProject04 = ""; string vcComDate04 = ""; string vcBanZhi04 = ""; string vcAB04 = "";//20181010添加AB值信息 - 李兴旺
                    string vcRemark1 = ""; string vcRemark2 = "";
                    string PorType = ""; string vcKBorser = "";
                    string vcComDate00 = ""; string vcBanZhi00 = "";
                    DataTable dtKANB = new DataTable();
                    #region 整理数据
                    vcPartsNo = dt17.Rows[i]["vcPartsNo"].ToString().Replace("-", "").ToString();//品番
                    string vcDock = dt17.Rows[i]["vcDock"].ToString();//受入
                    string vcCarFamilyCode = dt17.Rows[i]["vcCarType"].ToString();//车型
                    string vcEDflag = dt17.Rows[i]["jinjiqufen"].ToString();//紧急区分
                    if (vcEDflag == "通常")
                    {
                        vcEDflag = "S";
                    }
                    else if (vcEDflag == "紧急")
                    {
                        vcEDflag = "E";
                    }
                    else
                    {
                        vcEDflag = " ";
                    }
                    string vcKBorderno = dt17.Rows[i]["vcKBorderno"].ToString(); //看板订单号
                    string vcKBSerial = dt17.Rows[i]["vcKBSerial"].ToString();//连番
                    string vcPlanMonth = "";
                    string vcNo = "";
                    string vcPorType = "";
                    if (dt17.Columns.IndexOf("vcPlanMonth") >= 0)
                    {
                        vcPlanMonth = dt17.Rows[i]["vcPlanMonth"].ToString();
                    }
                    if (dt17.Columns.IndexOf("iNo") >= 0)
                    {
                        vcNo = dt17.Rows[i]["iNo"].ToString();
                    }
                    if (dt17.Columns.IndexOf("vcPorType") >= 0)
                    {
                        vcPorType = dt17.Rows[i]["vcPorType"].ToString();
                    }
                    //标记再打或者提前延迟打印区分是否打印Excel表
                    #region 整理数据 判断打印类型
                    check = logic.IfPrintKB(vcNo);
                    if (check)//已经打印过 属于再发行
                    {
                        DataTable QFED = logic.QFED00QuFen(vcPartsNo, vcDock, vcKBorderno, vcKBSerial, vcPlanMonth, vcNo);
                        if (QFED.Rows.Count != 0)
                        {
                            if (QFED.Rows[0]["iBaiJianFlag"].ToString() == "1" && vcPartsNo.Substring(10, 2) != "ED")//秦丰非ED再发行
                            {
                                dtKANB = logic.rePrintDataED(vcPartsNo, vcDock, vcPlanMonth, vcKBorderno, vcKBSerial, vcNo, vcCarFamilyCode);
                            }
                            else//秦丰ED再发行//非秦丰再发行
                            {
                                dtKANB = logic.rePrintData(vcPartsNo, vcDock, vcPlanMonth, vcKBorderno, vcKBSerial, vcNo);
                            }
                        }
                        vcFlagZ = "Z";
                    }
                    else//未打印过非再发行
                    {
                        dtKANB = logic.GetPrintFZData(vcPartsNo, vcDock, vcKBorderno, vcKBSerial, vcPlanMonth, vcNo);
                        vcFlagZ = "TY";
                    }
                    #endregion
                    if (dtKANB.Rows.Count != 0)
                    {
                        #region 取非空数据
                        vcPhotoPath = print.PhotoToArray(dtKANB.Rows[0]["vcPhotoPath"].ToString(), picnull);//图片二进制流
                        vcSupplierCode = dtKANB.Rows[0]["vcSupplierCode"].ToString();//供应商
                        vcSupplierPlant = dtKANB.Rows[0]["vcSupplierPlant"].ToString();//供应商工区
                        vcCpdCompany = dtKANB.Rows[0]["vcCpdCompany"].ToString();//收货方
                        vcPartsNameEN = dtKANB.Rows[0]["vcPartsNameEN"].ToString();//中英文品名
                        vcPartsNameCHN = dtKANB.Rows[0]["vcPartsNameCHN"].ToString();
                        vcLogisticRoute = dtKANB.Rows[0]["vcLogisticRoute"].ToString();//路径
                        iQuantityPerContainer = dtKANB.Rows[0]["iQuantityPerContainer"].ToString();//收容数
                        //工程、完成日、班值
                        vcComDate00 = dtKANB.Rows[0]["vcComDate00"].ToString();
                        vcBanZhi00 = dtKANB.Rows[0]["vcBanZhi00"].ToString();
                        vcProject01 = dtKANB.Rows[0]["vcProject01"].ToString();
                        vcComDate01 = dtKANB.Rows[0]["vcComDate01"].ToString();
                        vcBanZhi01 = dtKANB.Rows[0]["vcBanZhi01"].ToString();
                        vcAB01 = dtKANB.Rows[0]["vcAB01"].ToString();//20181010添加AB值信息 - 李兴旺
                        vcProject02 = dtKANB.Rows[0]["vcProject02"].ToString();
                        vcComDate02 = dtKANB.Rows[0]["vcComDate02"].ToString();
                        vcBanZhi02 = dtKANB.Rows[0]["vcBanZhi02"].ToString();
                        vcAB02 = dtKANB.Rows[0]["vcAB02"].ToString();//20181010添加AB值信息 - 李兴旺
                        vcProject03 = dtKANB.Rows[0]["vcProject03"].ToString();
                        vcComDate03 = dtKANB.Rows[0]["vcComDate03"].ToString();
                        vcBanZhi03 = dtKANB.Rows[0]["vcBanZhi03"].ToString();
                        vcAB03 = dtKANB.Rows[0]["vcAB03"].ToString();//20181010添加AB值信息 - 李兴旺
                        vcProject04 = dtKANB.Rows[0]["vcProject04"].ToString();
                        vcComDate04 = dtKANB.Rows[0]["vcComDate04"].ToString();
                        vcBanZhi04 = dtKANB.Rows[0]["vcBanZhi04"].ToString();
                        vcAB04 = dtKANB.Rows[0]["vcAB04"].ToString();//20181010添加AB值信息 - 李兴旺
                        vcRemark1 = dtKANB.Rows[0]["vcRemark1"].ToString();//特记事项
                        vcRemark2 = dtKANB.Rows[0]["vcRemark2"].ToString();
                        PorType = vcPorType;
                        vcKBorser = vcKBorderno + vcDock;
                        #endregion
                        string QuantityPerContainerFED = logic_09.resQuantityPerContainer(vcPartsNo, vcDock, vcPlanMonth);
                        if (QuantityPerContainerFED != iQuantityPerContainer)
                        {
                            DataTable dtKBhistory = logic.dtKBSerial_history(vcPartsNo, vcDock, vcKBorderno, vcKBSerial);
                            for (int a = 0; a < dtKBhistory.Rows.Count; a++)
                            {
                                gud = Guid.NewGuid().ToString("N");
                                iQuantityPerContainer = (Convert.ToInt32(QuantityPerContainerFED) > Convert.ToInt32(iQuantityPerContainer)) ? Convert.ToString((Convert.ToInt32(iQuantityPerContainer))) : Convert.ToString((Convert.ToInt32(QuantityPerContainerFED)));
                                dtPrint = dtPrintRE(vcSupplierCode, vcCpdCompany, vcCarFamilyCode, vcPartsNo, vcPartsNameEN, vcPartsNameCHN, vcLogisticRoute, iQuantityPerContainer, vcProject01, vcComDate01, vcBanZhi01, vcProject02, vcComDate02, vcBanZhi02, vcProject03, vcComDate03, vcBanZhi03, vcProject04, vcComDate04, vcBanZhi04, vcRemark1, vcRemark2, dtKBhistory.Rows[a]["vcKBSerial"].ToString(), vcPhotoPath, vcDock, vcKBorser, gud, vcKBorderno, PorType, vcEDflag, vcSupplierPlant, dtPrint, vcComDate00, vcBanZhi00, vcPlanMonth, vcAB01, vcAB02, vcAB03, vcAB04);
                                if (vcFlagZ == "TY")
                                {
                                    DataRow exrow = exdt.NewRow();
                                    exrow[0] = vcPartsNo;
                                    exrow[1] = vcCarFamilyCode;
                                    exrow[2] = vcPartsNameCHN;
                                    exrow[3] = vcProject01;
                                    exrow[4] = vcComDate01;
                                    exrow[5] = vcBanZhi01;
                                    exrow[6] = "1";//计数
                                    exrow[7] = PorType;
                                    exrow[8] = vcKBorderno;
                                    exrow[9] = dtKBhistory.Rows[a]["vcKBSerial"].ToString();
                                    exrow[10] = vcComDate00;
                                    exrow[11] = vcBanZhi00;
                                    exdt.Rows.Add(exrow);
                                }
                            }
                        }
                        else
                        {
                            gud = Guid.NewGuid().ToString("N");
                            dtPrint = dtPrintRE(vcSupplierCode, vcCpdCompany, vcCarFamilyCode, vcPartsNo, vcPartsNameEN, vcPartsNameCHN, vcLogisticRoute, iQuantityPerContainer, vcProject01, vcComDate01, vcBanZhi01, vcProject02, vcComDate02, vcBanZhi02, vcProject03, vcComDate03, vcBanZhi03, vcProject04, vcComDate04, vcBanZhi04, vcRemark1, vcRemark2, vcKBSerial, vcPhotoPath, vcDock, vcKBorser, gud, vcKBorderno, PorType, vcEDflag, vcSupplierPlant, dtPrint, vcComDate00, vcBanZhi00, vcPlanMonth, vcAB01, vcAB02, vcAB03, vcAB04);
                            if (vcFlagZ == "TY")
                            {
                                DataRow exrow = exdt.NewRow();
                                exrow[0] = vcPartsNo;
                                exrow[1] = vcCarFamilyCode;
                                exrow[2] = vcPartsNameCHN;
                                exrow[3] = vcProject01;
                                exrow[4] = vcComDate01;
                                exrow[5] = vcBanZhi01;
                                exrow[6] = "1";//计数
                                exrow[7] = PorType;
                                exrow[8] = vcKBorderno;
                                exrow[9] = vcKBSerial;
                                exrow[10] = vcComDate00;
                                exrow[11] = vcBanZhi00;
                                exdt.Rows.Add(exrow);
                            }
                        }
                    }
                    else
                    {
                        gud = Guid.NewGuid().ToString("N");
                        dtPrint = dtPrintRE(vcSupplierCode, vcCpdCompany, vcCarFamilyCode, vcPartsNo, vcPartsNameEN, vcPartsNameCHN, vcLogisticRoute, iQuantityPerContainer, vcProject01, vcComDate01, vcBanZhi01, vcProject02, vcComDate02, vcBanZhi02, vcProject03, vcComDate03, vcBanZhi03, vcProject04, vcComDate04, vcBanZhi04, vcRemark1, vcRemark2, vcKBSerial, vcPhotoPath, vcDock, vcKBorser, gud, vcKBorderno, PorType, vcEDflag, vcSupplierPlant, dtPrint, vcComDate00, vcBanZhi00, vcPlanMonth, vcAB01, vcAB02, vcAB03, vcAB04);
                        if (vcFlagZ == "TY")
                        {
                            DataRow exrow = exdt.NewRow();
                            exrow[0] = vcPartsNo;
                            exrow[1] = vcCarFamilyCode;
                            exrow[2] = vcPartsNameCHN;
                            exrow[3] = vcProject01;
                            exrow[4] = vcComDate01;
                            exrow[5] = vcBanZhi01;
                            exrow[6] = "1";//计数
                            exrow[7] = PorType;
                            exrow[8] = vcKBorderno;
                            exrow[9] = vcKBSerial;
                            exrow[10] = vcComDate00;
                            exrow[11] = vcBanZhi00;
                            exdt.Rows.Add(exrow);
                        }
                    }
                    #endregion
                }
                #region 打印操作
                dtPrint = print.orderDataTable(dtPrint);//排序
                logic.insertTableCR(dtPrint);//插入打印临时子表
                logic.insertTableExcel00(exdt);//插入看板确认单Excel
                //DataTable dtPorType = print.searchPorType00();//取生产部署
                dtPorType = logic.QueryGroup(dtPrint);//用订单号 生产部署 生产日期 生产班值分组,修改不在数据库中取值
                print.insertTableCRMain00(dtPrint, dtPorType);//插入打印临时主表
                //string printDay = "";
                //logic_09.KanBIfPrintDay();//获取班值信息
                string reportPath = "CrReport.rpt";
                string strLoginId = loginInfo.UserId;
                for (int z = 0; z < dtPorType.Rows.Count; z++)
                {
                    //DataTable exdtt = exdt.Clone();
                    DataTable exdttt = new DataTable();
                    DataTable exdthj = new DataTable();
                    //exdttt.Clear();
                    string vcPorType = dtPorType.Rows[z]["vcPorType"].ToString();
                    string vcorderno = dtPorType.Rows[z]["vcorderno"].ToString();
                    string vcComDate01 = dtPorType.Rows[z]["vcComDate01"].ToString();
                    string vcBanZhi01 = dtPorType.Rows[z]["vcBanZhi01"].ToString();
                    string vcComDate00 = dtPorType.Rows[z]["vcComDate00"].ToString();
                    string vcBanZhi00 = dtPorType.Rows[z]["vcBanZhi00"].ToString();
                    string msg = print.printCr(reportPath, vcPorType, vcorderno, vcComDate01, vcBanZhi01, vcComDate00, vcBanZhi00, strLoginId, strPrinterName);//打印水晶报表

                    //数据库取出Excel的数据进行打印
                    //dtPorType.Rows[z]["vcPorType"].ToString(), dtPorType.Rows[z]["vcorderno"].ToString(), dtPorType.Rows[z]["vcComDate01"].ToString(), dtPorType.Rows[z]["vcBanZhi01"].ToString()
                    DataSet ds = logic.PrintExcel(vcPorType, vcorderno, vcComDate01, vcBanZhi01, vcComDate00, vcBanZhi00);
                    exdttt = ds.Tables[0];
                    if (exdttt.Rows.Count != 0)
                    {
                        #region Excel打印
                        exdthj = ds.Tables[1];
                        for (int p = 0; p < exdttt.Rows.Count; p++)
                        {
                            exdttt.Rows[p]["no"] = p + 1;
                        }
                        int dsRowsCount = exdttt.Rows.Count;
                        int dsRow = dsRowsCount / 43;
                        int dsrows = dsRowsCount % 43;
                        //总页数
                        int pagetotle = 0;
                        //页数
                        int pageno = 0;
                        if (dsRow != 0)
                        {
                            pagetotle = ((dsrows + exdthj.Rows.Count + 3) / 43) == 0 ? (dsRow + 1) : (dsRow + 1 + (((exdthj.Rows.Count + 3) / 43) == 0 ? 1 : ((exdthj.Rows.Count + 3) / 43) + 1));
                        }
                        else
                        {
                            pagetotle = ((dsRowsCount + exdthj.Rows.Count + 3) / 43) == 0 ? 1 : (1 + (((exdthj.Rows.Count + 3) / 43) == 0 ? 1 : ((exdthj.Rows.Count + 3) / 43) + 1));
                        }
                        if (dsRow > 0)
                        {
                            DataTable inTable = exdttt.Clone();
                            for (int i = 0; i < exdttt.Rows.Count; i++)
                            {
                                DataRow dr = exdttt.Rows[i];
                                DataRow add = inTable.NewRow();
                                add.ItemArray = dr.ItemArray;
                                inTable.Rows.Add(add);
                                if (inTable.Rows.Count >= 43 || exdttt.Rows.Count - 1 == i)
                                {
                                    pageno = pageno + 1;
                                    string pageB = "0";
                                    if (inTable.Rows.Count < 43)
                                    {
                                        pageB = inTable.Rows.Count + exdthj.Rows.Count + 3 <= 43 ? "1" : "2";
                                    }
                                    //打印传值有：订单号、生产部署、计划打印日期 printIme、计划打印班值 printDay 、计划生产日期 、计划生产班值、页码
                                    //exprint.PrintTemplateFromDataTable(inTable, exdthj, tmplatePath, vcorderno, vcPorType, strLoginId, vcComDate00, vcBanZhi00 == "白" ? "白值" : "夜值", vcComDate01, vcBanZhi01 == "白" ? "白值" : "夜值", strPrinterName, Convert.ToString(pagetotle), Convert.ToString(pageno), pageB);
                                    #region 创建打印临时表并打印 WebService
                                    FS1209_Logic lg = new FS1209_Logic();
                                    string exdthj_tmp = string.Empty;
                                    string inTable_tmp = string.Empty;
                                    string exdthj_msg = lg.CreateTempTable(exdthj, "FS1209_Excel_", out exdthj_tmp);//创建打印临时表  
                                    string inTable_msg = lg.CreateTempTable(exdttt, "FS1209_Excel_", out inTable_tmp);//创建打印临时表                 
                                    if (inTable_msg.Length == 0)
                                    {
                                        try
                                        {
                                            BasicHttpBinding binding = new BasicHttpBinding();
                                            binding.CloseTimeout = TimeSpan.MaxValue;
                                            binding.OpenTimeout = TimeSpan.MaxValue;
                                            binding.ReceiveTimeout = TimeSpan.MaxValue;
                                            binding.SendTimeout = TimeSpan.MaxValue;
                                            EndpointAddress address = new EndpointAddress("http://localhost:8089/PrintTable.asmx");
                                            PrintCR.PrintTableSoapClient client = new PrintCR.PrintTableSoapClient(binding, address);
                                            msg = client.PrintExcel_Confirmation(inTable_tmp, exdthj_tmp, tmplatePath, vcorderno, vcPorType, strLoginId, vcComDate00, vcBanZhi00 == "白" ? "白值" : "夜值", vcComDate01, vcBanZhi01 == "白" ? "白值" : "夜值", strPrinterName, Convert.ToString(pagetotle), Convert.ToString(pageno), pageB);
                                        }
                                        catch (Exception ex)
                                        {
                                            msg = "打印看板确认单失败！";
                                            throw ex;
                                        }
                                        finally
                                        {
                                            lg.DropTempTable(inTable_tmp);//删除打印临时表
                                            lg.DropTempTable(exdthj_tmp);//删除打印临时表
                                        }
                                    }
                                    #endregion

                                    if (vcType != "再发行")
                                    {
                                        DataTable checkorderno = new DataTable();
                                        checkorderno = logic_09.check(vcorderno, vcPorType);
                                        int rows;
                                        rows = checkorderno.Rows.Count;
                                        //向tKanBanQrTbl表中插入数据
                                        if (rows == 0)
                                        {
                                            //将testprinterExcel表中数据存入到testprinterExcel1中
                                            logic_09.InsertInto(vcorderno, vcPorType);
                                            logic_09.InsertDate(vcorderno, vcPorType, vcComDate00, vcBanZhi00 == "白" ? "0" : "1", vcComDate01, vcBanZhi01 == "白" ? "0" : "1");
                                        }
                                    }
                                    inTable = exdt.Clone();
                                }
                            }
                        }
                        else
                        {
                            string pageB = "0";
                            pageno = pageno + 1;
                            pageB = exdttt.Rows.Count + exdthj.Rows.Count + 3 <= 43 ? "1" : "2";
                            //打印传值有：订单号、生产部署、计划打印日期 printIme、计划打印班值 printDay 、计划生产日期 、计划生产班值、页码
                            //exprint.PrintTemplateFromDataTable(exdttt, exdthj, tmplatePath, vcorderno, vcPorType, strLoginId, vcComDate00, vcBanZhi00 == "白" ? "白值" : "夜值", vcComDate01, vcBanZhi01 == "白" ? "白值" : "夜值", strPrinterName, Convert.ToString(pagetotle), Convert.ToString(pageno), pageB);
                            #region 创建打印临时表并打印 WebService
                            FS1209_Logic lg = new FS1209_Logic();
                            string exdttt_tmp = string.Empty;
                            string exdthj_tmp = string.Empty;
                            string exdttt_msg = lg.CreateTempTable(exdttt, "FS1209_Excel_", out exdttt_tmp);//创建打印临时表
                            string exdthj_msg = lg.CreateTempTable(exdthj, "FS1209_Excel_", out exdthj_tmp);//创建打印临时表                
                            if (exdthj_msg.Length == 0)
                            {
                                try
                                {
                                    BasicHttpBinding binding = new BasicHttpBinding();
                                    binding.CloseTimeout = TimeSpan.MaxValue;
                                    binding.OpenTimeout = TimeSpan.MaxValue;
                                    binding.ReceiveTimeout = TimeSpan.MaxValue;
                                    binding.SendTimeout = TimeSpan.MaxValue;
                                    EndpointAddress address = new EndpointAddress("http://localhost:8089/PrintTable.asmx");
                                    PrintCR.PrintTableSoapClient client = new PrintCR.PrintTableSoapClient(binding, address);
                                    exdthj_msg = client.PrintExcel_Confirmation(exdttt_tmp, exdthj_tmp, tmplatePath, vcorderno, vcPorType, strLoginId, vcComDate00, vcBanZhi00 == "白" ? "白值" : "夜值", vcComDate01, vcBanZhi01 == "白" ? "白值" : "夜值", strPrinterName, Convert.ToString(pagetotle), Convert.ToString(pageno), pageB);
                                }
                                catch (Exception ex)
                                {
                                    msg = "打印看板确认单失败！";
                                    throw ex;
                                }
                                finally
                                {
                                    lg.DropTempTable(exdttt_tmp);//删除打印临时表
                                    lg.DropTempTable(exdthj_tmp);//删除打印临时表
                                }
                            }
                            #endregion

                            if (vcType != "再发行")
                            {
                                DataTable checkorderno = new DataTable();
                                checkorderno = logic_09.check(vcorderno, vcPorType);
                                int rows;
                                rows = checkorderno.Rows.Count;
                                //向tKanBanQrTbl表中插入数据
                                if (rows == 0)
                                {
                                    //将testprinterExcel表中数据存入到testprinterExcel1中
                                    logic_09.InsertInto(vcorderno, vcPorType);
                                    logic_09.InsertDate(vcorderno, vcPorType, vcComDate00, vcBanZhi00 == "白" ? "0" : "1", vcComDate01, vcBanZhi01 == "白" ? "0" : "1");
                                }
                            }
                        }
                        #endregion
                    }
                    //删除看板打印的临时文件
                    logic_09.DeleteprinterCREX(vcPorType, vcorderno, vcComDate01, vcBanZhi01);
                }
                #endregion
                if (!(check))
                {
                    logic.UpdatePrintKANB(dtPrint);//更新看板打印表
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "打印成功";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                if (dtPorType != null)
                {
                    for (int i = 0; i < dtPorType.Rows.Count; i++)
                    {
                        logic_09.DeleteprinterCREX(dtPorType.Rows[i]["vcPorType"].ToString(), dtPorType.Rows[i]["vcorderno"].ToString(), dtPorType.Rows[i]["vcComDate01"].ToString(), dtPorType.Rows[i]["vcBanZhi01"].ToString());
                    }
                }
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "打印失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 非ED追加打印Api
        [HttpPost]
        [EnableCors("any")]
        public string printEDApi([FromBody] dynamic data)
        {
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            ApiResult apiResult = new ApiResult();
            dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
            string vcType = dataForm.vcType == null ? "" : dataForm.vcType;
            JArray checkedInfo = dataForm._multipleSelection;
            List<Dictionary<string, object>> listInfoData = checkedInfo.ToObject<List<Dictionary<string, object>>>();
            if (vcType == "A")
            {
                if (listInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请选择要打印的数据行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
            }
            else
            {
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "请选择全部打印按钮进行打印！";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }

            DataTable DataPorType = getDataPorType(loginInfo.UserId);
            DataTable tbThrow = new DataTable();
            string msg = string.Empty;
            try
            {
                string ls_fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + Guid.NewGuid().ToString().Replace("-", "") + ".png";
                string strPrinterName = logic.PrintMess(loginInfo.UserId);//获取打印机
                string picnull = _webHostEnvironment.ContentRootPath + "Doc\\Image\\SPPartImage\\picnull.JPG";
                byte[] vcPhotoPath = print.PhotoToArray("", picnull);
                DataTable dtPrintCR = new DataTable();
                DataTable dtPrintCRLone = print.searchTBCreate();//获得数据库表结构
                DataTable dtPrint = dtPrintCRLone.Clone();
                bool check = true;
                //string QFlag = "2";
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (listInfoData[i]["iNo"].ToString() != "")
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "选择的行不属于该打印类别，请选择后点击打印。";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    } 
                    #region 整理数据
                    string vcPartsNo = listInfoData[i]["vcPartsNo"].ToString().Replace("-", "");//品番
                    string vcSupplierCode = ""; string vcSupplierPlant = ""; string vcCpdCompany = ""; string vcPartsNameEN = ""; string vcPartsNameCHN = "";
                    string vcLogisticRoute = ""; string iQuantityPerContainer = "0";
                    string vcProject01 = ""; string vcComDate01 = ""; string vcBanZhi01 = "";
                    string vcProject02 = ""; string vcComDate02 = ""; string vcBanZhi02 = "";
                    string vcProject03 = ""; string vcComDate03 = ""; string vcBanZhi03 = "";
                    string vcProject04 = ""; string vcComDate04 = ""; string vcBanZhi04 = "";
                    string vcRemark1 = ""; string vcRemark2 = "";
                    string PorType = ""; string vcKBorser = "";

                    DataTable dtKANB = new DataTable();
                    string vcDock = listInfoData[i]["vcDock"].ToString();//受入
                    string vcKBorderno = listInfoData[i]["vcKBorderno"].ToString();//看板订单号
                    string vcKBSerial = listInfoData[i]["vcKBSerial"].ToString();//连番

                    string vcCarFamilyCode = listInfoData[i]["vcCarType"].ToString();//车型
                    string vcEDflag = listInfoData[i]["jinjiqufen"].ToString();//紧急区分
                    if (vcEDflag == "通常") vcEDflag = "S";
                    else if (vcEDflag == "紧急") vcEDflag = "E";
                    else vcEDflag = " ";

                    string vcPlanMonth = "";
                    string vcNo = "";
                    string vcPorType ="";

                    DataTable dtrusut = logic.seaKBnoser(vcKBorderno, vcKBSerial, vcPartsNo, vcDock);
                    DataTable dtr = logic.seaKBSerial_history(vcKBorderno, vcKBSerial, vcPartsNo, vcDock);
                    if (dtrusut.Rows.Count != 0)
                    {
                        vcPlanMonth = dtrusut.Rows[0]["vcPlanMonth"].ToString();
                        vcNo = dtrusut.Rows[0]["iNo"].ToString();
                        vcPorType = dtrusut.Rows[0]["vcPorType"].ToString();
                    }
                    else if(dtr.Rows.Count != 0)
                    {
                        DataTable dtrusutno = logic.seaKBnoser(vcKBorderno, dtr.Rows[0]["vcKBSerialBefore"].ToString(), vcPartsNo, vcDock);
                        vcPlanMonth = dtrusutno.Rows[0]["vcPlanMonth"].ToString();
                        vcNo = dtrusutno.Rows[0]["iNo"].ToString();
                        vcPorType = dtrusutno.Rows[0]["vcPorType"].ToString();
                    }
                    string vcKBSerialup = logic.dtKBSerialUP(vcPartsNo, vcDock, vcKBorderno, vcKBSerial);//获取打印的ED连番
                    string QuantityPerContainerFED = logic.dtMasteriQuantity(vcPartsNo, vcDock, vcPlanMonth);
                    //dtKANB = print.searchPrintKANBALL(vcPartsNo, vcDock, vcKBorderno, vcKBSerialup);
                    DataTable QFED = logic.QFED00QuFen(vcPartsNo, vcDock, vcKBorderno, vcKBSerial, vcPlanMonth, vcNo);
                    if (QFED.Rows.Count != 0)
                    {
                        if (QFED.Rows[0]["iBaiJianFlag"].ToString() == "1" && vcPartsNo.Substring(10, 2) != "ED")//秦丰非ED再发行
                        {
                            dtKANB = logic.rePrintDataED(vcPartsNo, vcDock, vcPlanMonth, vcKBorderno, vcKBSerialup, vcNo, vcCarFamilyCode);
                        }
                    }
                    if (dtKANB.Rows.Count != 0)
                    {
                        #region 取非空数据
                        vcPhotoPath = print.PhotoToArray(dtKANB.Rows[0]["vcPhotoPath"].ToString(), picnull);//图片二进制流
                        vcSupplierCode = dtKANB.Rows[0]["vcSupplierCode"].ToString();//供应商
                        vcSupplierPlant = dtKANB.Rows[0]["vcSupplierPlant"].ToString();//供应商工区
                        vcCpdCompany = dtKANB.Rows[0]["vcCpdCompany"].ToString();//收货方
                        vcPartsNameEN = dtKANB.Rows[0]["vcPartsNameEN"].ToString();//中英文品名
                        vcPartsNameCHN = dtKANB.Rows[0]["vcPartsNameCHN"].ToString();
                        vcLogisticRoute = dtKANB.Rows[0]["vcLogisticRoute"].ToString();//路径
                        iQuantityPerContainer = dtKANB.Rows[0]["iQuantityPerContainer"].ToString();//收容数
                        vcProject01 = dtKANB.Rows[0]["vcProject01"].ToString();//工程、完成日、班值
                        vcComDate01 = dtKANB.Rows[0]["vcComDate01"].ToString();
                        vcBanZhi01 = dtKANB.Rows[0]["vcBanZhi01"].ToString();
                        vcProject02 = dtKANB.Rows[0]["vcProject02"].ToString();
                        vcComDate02 = dtKANB.Rows[0]["vcComDate02"].ToString();
                        vcBanZhi02 = dtKANB.Rows[0]["vcBanZhi02"].ToString();
                        vcProject03 = dtKANB.Rows[0]["vcProject03"].ToString();
                        vcComDate03 = dtKANB.Rows[0]["vcComDate03"].ToString();
                        vcBanZhi03 = dtKANB.Rows[0]["vcBanZhi03"].ToString();
                        vcProject04 = dtKANB.Rows[0]["vcProject04"].ToString();
                        vcComDate04 = dtKANB.Rows[0]["vcComDate04"].ToString();
                        vcBanZhi04 = dtKANB.Rows[0]["vcBanZhi04"].ToString();
                        vcRemark1 = dtKANB.Rows[0]["vcRemark1"].ToString();//特记事项
                        vcRemark2 = dtKANB.Rows[0]["vcRemark2"].ToString();
                        PorType = vcPorType;
                        vcKBorser = vcKBorderno + vcDock;
                        #endregion
                    }
                    gud = Guid.NewGuid().ToString("N");
                    string reCode = print.reCode(vcSupplierCode, vcSupplierPlant, vcDock, vcPartsNo, iQuantityPerContainer, vcKBSerial, vcEDflag, vcKBorderno);
                    byte[] vcQRCodeImge = print.GenGenerateQRCode(msg, reCode);
                    #region
                    DataRow row = dtPrint.NewRow();
                    row[0] = vcSupplierCode.ToUpper();
                    row[1] = vcCpdCompany.ToUpper();
                    row[2] = vcCarFamilyCode.ToUpper();
                    row[3] = vcPartsNo;
                    row[4] = vcPartsNameEN;
                    row[5] = vcPartsNameCHN;
                    row[6] = vcLogisticRoute;
                    row[7] = (Convert.ToInt32(QuantityPerContainerFED) > Convert.ToInt32(iQuantityPerContainer)) ? iQuantityPerContainer : QuantityPerContainerFED;
                    row[8] = vcProject01;
                    row[9] = vcComDate01;
                    row[10] = vcBanZhi01;
                    row[11] = vcProject02;
                    row[12] = vcComDate02;
                    row[13] = vcBanZhi02;
                    row[14] = vcProject03;
                    row[15] = vcComDate03;
                    row[16] = vcBanZhi03;
                    row[17] = vcProject04;
                    row[18] = vcComDate04;
                    row[19] = vcBanZhi04;
                    row[20] = vcRemark1;
                    row[21] = vcRemark2;
                    row[22] = vcKBSerial;
                    row[23] = vcPhotoPath;
                    row[24] = vcQRCodeImge;
                    row[25] = vcDock;//标记类型
                    row[26] = vcKBorser.ToUpper();
                    row[27] = gud;
                    row[28] = vcKBorderno.ToUpper();
                    row[29] = PorType;
                    row[30] = vcEDflag == "E" ? "紧急" : "";
                    #endregion
                    dtPrint.Rows.Add(row);
                    #endregion
                }
                dtPrint = print.orderDataTable(dtPrint);//排序
                print.insertTableCR(dtPrint);//插入打印临时子表
                //DataTable dtPorType = print.searchPorType();//取生产部署
                DataTable dtPorType = logic.QueryGroup(dtPrint);//用订单号 生产部署 生产日期 生产班值分组,修改不在数据库中取值
                tbThrow = dtPorType;
                print.insertTableCRMain(dtPrint, dtPorType);//插入打印临时主表
                string printDay = logic_09.KanBIfPrintDay();//获取班值信息
                string reportPath = "CrReport.rpt";
                string strLoginId = loginInfo.UserId;
                for (int z = 0; z < dtPorType.Rows.Count; z++)
                {
                    msg = print.printCr(reportPath, dtPorType.Rows[z]["vcPorType"].ToString(), "", "", "", "", "", strLoginId, strPrinterName);//打印水晶报表
                    //删除看板打印的临时文件
                    logic_09.DeleteprinterCREX1(dtPorType.Rows[z]["vcPorType"].ToString(), dtPorType.Rows[z]["vcorderno"].ToString(), dtPorType.Rows[z]["vcComDate01"].ToString(), dtPorType.Rows[z]["vcBanZhi01"].ToString());
                }
                if (!(check))
                {
                    logic.UpdatePrintKANB(dtPrint);//更新看板打印表
                }
                if (msg == "打印成功")
                {
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = msg;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
            }
            catch (Exception ex)
            {
                if (tbThrow != null)
                {
                    for (int i = 0; i < tbThrow.Rows.Count; i++)
                    {
                        logic_09.DeleteprinterCREX1(tbThrow.Rows[i]["vcPorType"].ToString(), tbThrow.Rows[i]["vcorderno"].ToString(), tbThrow.Rows[i]["vcComDate01"].ToString(), tbThrow.Rows[i]["vcBanZhi01"].ToString());
                    }
                }
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "打印失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion


        #region 提交数据库
        /// <summary>
        /// 特殊打印录入-更新列表数据
        /// </summary>
        /// <param name="dt">列表数据集合</param>
        [HttpPost]
        [EnableCors("any")]
        public string InUpdeOldData([FromBody] dynamic data)
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
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(Convert.ToString(dataForm));
            try
            {
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = logic.InUpdeOldData(dt);
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0204", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "更新失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 特殊打印录入
        /// <summary>
        /// 特殊打印录入
        /// </summary>
        [HttpPost]
        [EnableCors("any")]
        public string SearchPrintTDB([FromBody] dynamic data)
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
            string vcPrint = dataForm.vcPrint;
            try
            {
                FS1209_Logic logic_1 = new FS1209_Logic();
                string RolePorType = logic_1.getRoleTip(loginInfo.UserId);
                DataTable dt1 = logic_1.dllPorType(RolePorType.Split('*'));
                DataTable tb = logic.SearchPrintTDB(vcPrint, RolePorType.Split('*'));
                List<object> dataList = ComFunction.convertAllToResult(tb);
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
        #endregion

        #region 特殊打印
        /// <summary>
        /// 特殊打印 检索条件获取列表数据 无传入参数
        /// </summary>
        [HttpPost]
        [EnableCors("any")]
        public string SearchPrintT()
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
                FS1209_Logic logic_1 = new FS1209_Logic();
                DataTable tb = logic.searchPrintT();
                List<object> dataList = ComFunction.convertAllToResult(tb);
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
        #endregion

        #region 确认单再发行
        /// <summary>
        /// 确认单再发行
        /// </summary>
        [HttpPost]
        [EnableCors("any")]
        public string SearchRePrintKBQR([FromBody] dynamic data)
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
            string vcKbOrderId = dataForm.vcKbOrderId;
            string vcPlanPrintDate = dataForm.vcPlanPrintDate;
            string vcPlanProcDate = dataForm.vcPlanProcDate;
            string vcPrintDate = dataForm.vcPrintDate;
            string vcGC = dataForm.vcGC;
            string vcPlanPrintBZ = dataForm.vcPlanPrintBZ;
            string vcPlanProcBZ = dataForm.vcPlanProcBZ;
            vcKbOrderId = vcKbOrderId == null ? "" : vcKbOrderId;
            vcPlanPrintDate = vcPlanPrintDate == null ? "" : vcPlanPrintDate;
            vcPlanProcDate = vcPlanProcDate == null ? "" : vcPlanProcDate;
            vcPrintDate = vcPrintDate == null ? "" : vcPrintDate;
            vcGC = vcGC == null ? "" : vcGC;
            vcPlanPrintBZ = vcPlanPrintBZ == null ? "" : vcPlanPrintBZ;
            vcPlanProcBZ = vcPlanProcBZ == null ? "" : vcPlanProcBZ;
            try
            {
                DataTable tb = logic.SearchRePrintKBQR(vcKbOrderId, vcGC, vcPlanPrintDate, vcPlanPrintBZ, vcPlanProcDate, vcPlanProcBZ, vcPrintDate);
                List<object> dataList = ComFunction.convertAllToResult(tb);
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
        #endregion


        #region 填充数据
        private DataTable dtPrintRE(string vcSupplierCode, string vcCpdCompany, string vcCarFamilyCode, string vcPartsNo, string vcPartsNameEN, string vcPartsNameCHN, string vcLogisticRoute,
           string iQuantityPerContainer, string vcProject01, string vcComDate01, string vcBanZhi01, string vcProject02, string vcComDate02, string vcBanZhi02, string vcProject03,
           string vcComDate03, string vcBanZhi03, string vcProject04, string vcComDate04, string vcBanZhi04, string vcRemark1, string vcRemark2, string vcKBSerial, byte[] vcPhotoPath,
           string vcDock, string vcKBorser, string i, string vcKBorderno, string PorType, string vcEDflag, string vcSupplierPlant, DataTable dtPrint, string vcComDate00, string vcBanZhi00, string vcPlanMonth,
           string vcAB01, string vcAB02, string vcAB03, string vcAB04)//20181010添加AB值信息 - 李兴旺
        {
            //string ls_savePath = "\\QRCodeImages\\" + ls_fileName;
            string reCode = print.reCode(vcSupplierCode, vcSupplierPlant, vcDock, vcPartsNo, iQuantityPerContainer, vcKBSerial, vcEDflag, vcKBorderno);
            byte[] vcQRCodeImge = print.GenGenerateQRCode("", reCode);
            DataRow row = dtPrint.NewRow();
            row[0] = vcSupplierCode.ToUpper();
            row[1] = vcCpdCompany.ToUpper();
            row[2] = vcCarFamilyCode.ToUpper();
            row[3] = vcPartsNo;
            row[4] = vcPartsNameEN;
            row[5] = vcPartsNameCHN;
            row[6] = vcLogisticRoute;
            row[7] = iQuantityPerContainer;
            row[8] = vcProject01;
            row[9] = vcComDate01;
            row[10] = vcBanZhi01;
            row[11] = vcProject02;
            row[12] = vcComDate02;
            row[13] = vcBanZhi02;
            row[14] = vcProject03;
            row[15] = vcComDate03;
            row[16] = vcBanZhi03;
            row[17] = vcProject04;
            row[18] = vcComDate04;
            row[19] = vcBanZhi04;
            row[20] = vcRemark1;
            row[21] = vcRemark2;
            row[22] = vcKBSerial;
            row[23] = vcPhotoPath;
            row[24] = vcQRCodeImge;
            row[25] = vcDock;//标记类型
            row[26] = vcKBorser.ToUpper();
            row[27] = i;
            row[28] = vcKBorderno.ToUpper();
            row[29] = PorType;
            row[30] = vcEDflag == "E" ? "紧急" : "";
            row[31] = vcComDate00;
            row[32] = vcBanZhi00;
            row[34] = vcPlanMonth;
            row[35] = vcAB01;//20181010添加AB值 - 李兴旺
            row[36] = vcAB02;//20181010添加AB值 - 李兴旺
            row[37] = vcAB03;//20181010添加AB值 - 李兴旺
            row[38] = vcAB04;//20181010添加AB值 - 李兴旺
            dtPrint.Rows.Add(row);
            return dtPrint;
        }
        #endregion

        #region 数据表紧急区分设置
        public DataTable returnED(DataTable dt)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i]["jinjiqufen"].ToString() == "S")
                {
                    dt.Rows[i]["jinjiqufen"] = "通常";
                }
                if (dt.Rows[i]["jinjiqufen"].ToString() == "E")
                {
                    dt.Rows[i]["jinjiqufen"] = "紧急";
                }
            }
            return dt;
        }
        #endregion

        #region 检索方法
        public DataTable getData(string vcKbOrderId, string vcTF, string vcFBZ, string vcTT, string vcTFZ, string vcPartsNo, string vcCarType, string vcGC, string vcType, string vcplant, DataTable dtflag)
        {
            FS1210_Logic logic = new FS1210_Logic();
            DataTable returndata = logic.PrintData(vcKbOrderId, vcTF, vcFBZ, vcTT, vcTFZ, vcPartsNo, vcCarType, vcGC, vcType, vcplant, dtflag);

            if (returndata == null || returndata.Rows.Count == 0)
            {
            }
            else
            {
                returndata = returnED(returndata);
            }
            return returndata;
        }

        public DataTable getDataPorType(string vcUserId)
        {
            FS1209_Logic lg = new FS1209_Logic();
            string RolePorType = lg.getRoleTip(vcUserId);
            DataTable tb = lg.dllPorType(RolePorType.Split('*'));
            return tb;
        }
        #endregion


        #region 自动获取数据
        [HttpPost]
        [EnableCors("any")]
        public string textChangeApi([FromBody] dynamic data)
        {
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            ApiResult apiResult = new ApiResult();
            dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
            string vcPartsNo = dataForm.vcPartsNo == null ? "" : dataForm.vcPartsNo;
            string vcDock = dataForm.vcDock == null ? "" : dataForm.vcDock;
            string vcKBorderno = dataForm.vcKBorderno == null ? "" : dataForm.vcKBorderno;
            string vcKBSerial = dataForm.vcKBSerial == null ? "" : dataForm.vcKBSerial;

            string strReturn;
            try
            {
                if (vcPartsNo != "" && vcDock != "" && vcKBorderno != "" && vcKBSerial != "")
                {
                    DataTable dtrusut = logic.seaKBnoser(vcKBorderno, vcKBSerial, vcPartsNo, vcDock);
                    DataTable dtr = logic.seaKBSerial_history(vcKBorderno, vcKBSerial, vcPartsNo, vcDock);
                    if (dtrusut.Rows.Count != 0)
                    {
                        strReturn = dtrusut.Rows[0]["vcCarType"].ToString();
                        strReturn = strReturn + "?" + dtrusut.Rows[0]["vcTips"].ToString();
                        string vcEDflag = "";
                        if (dtrusut.Rows[0]["vcEDflag"].ToString() == "S")
                        {
                            vcEDflag = "通常";
                        }
                        else if (dtrusut.Rows[0]["vcEDflag"].ToString() == "E")
                        {
                            vcEDflag = "紧急";
                        }
                        strReturn = strReturn + "?" + vcEDflag;
                    }
                    else if (dtr.Rows.Count != 0)
                    {
                        DataTable dtrusutno = logic.seaKBnoser(vcKBorderno, dtr.Rows[0]["vcKBSerialBefore"].ToString(), vcPartsNo, vcDock);
                        strReturn = dtrusut.Rows[0]["vcCarType"].ToString();
                        strReturn = strReturn + "?" + dtrusut.Rows[0]["vcTips"].ToString();
                        string vcEDflag = "";
                        if (dtrusutno.Rows[0]["vcEDflag"].ToString() == "S")
                        {
                            vcEDflag = "通常";
                        }
                        else
                            if (dtrusutno.Rows[0]["vcEDflag"].ToString() == "E")
                        {
                            vcEDflag = "紧急";
                        }
                        strReturn = strReturn + "?" + vcEDflag;
                    }
                    else
                    {
                        strReturn = "";
                        strReturn = strReturn + "?" + "";
                        strReturn = strReturn + "?" + "";
                    }
                }
                else
                {
                    strReturn = "";
                    strReturn = strReturn + "?" + "";
                    strReturn = strReturn + "?" + "";
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = strReturn;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}
