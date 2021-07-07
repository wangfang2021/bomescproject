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

namespace SPPSApi.Controllers.G12
{
    [Route("api/FS1210_spPrint/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1210Controller_spPrint : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        FS1209_Logic logic09 = new FS1209_Logic();
        FS1210_Logic logic = new FS1210_Logic();
        PrinterCR print = new PrinterCR();
        private readonly string FunctionID = "FS1210";

        public FS1210Controller_spPrint(IWebHostEnvironment webHostEnvironment)
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
                Dictionary<string, Object> res = new Dictionary<string, Object>();
                FS1209_Logic logic_1 = new FS1209_Logic();
                string RolePorType = logic_1.getRoleTip(loginInfo.UserId);
                DataTable dtportype = logic_1.dllPorType(RolePorType.Split('*'));
                List<Object> dataList_PorType = ComFunction.convertAllToResult(dtportype);

                res.Add("PorTypeSource", dataList_PorType);
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
            string vcPrint = dataForm.vcPrint;
            try
            {
                string RolePorType = logic09.getRoleTip(loginInfo.UserId);
                string[] str2;
                str2 = RolePorType.Split('*');
                DataTable dt = logic.SearchPrintTDB(vcPrint, str2);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);
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
                JArray checkedInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();
                if (listInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选择一条数据！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                logic.Del(listInfoData);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0903", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除失败";
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
                    string[,] strField = new string[,] {
                                                {"品番","受入","生产部署","收容数","车型","紧急区分","看板订单号","连番","备注",
                                                 "完成日01","班值01","完成日02","班值02","完成日03","班值03","完成日04","班值04" },
                                                {"vcPartsNo", "vcDock", "vcPorType", "vcQuantityPerContainer", "vcCarType", "vcEDflag","vcKBorderno","vcKBSerial","vcTips",
                                                 "vcComDate01","vcBanZhi01","vcComDate02","vcBanZhi02","vcComDate03","vcBanZhi03","vcComDate04","vcBanZhi04" },
                                                {"","","","","","","","","","","","","","","","","" },
                                                {"12","2","4","8","4","2","12","4","100","10","1","10","1","10","1","10","1"},//最大长度设定,不校验最大长度用0
                                                {"12","2","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0"},//最小长度设定,可以为空用0
                                                {"1","2","3","4","5","6","7","8","9","10","11","12","13","14","15","16","17"}//前台显示列号，从0开始计算,注意有选择框的是0
                    };
                    List<Object> checkRes = ListChecker.validateList(listInfoData, strField, null, null, true, "FS1210_spInsert");
                    if (checkRes != null)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = checkRes;
                        apiResult.flag = Convert.ToInt32(ERROR_FLAG.单元格定位提示);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                string strErrorPartId = "";
                DataTable dt = ListToDataTable(listInfoData);
                strErrorPartId = logic.InUpdeOldData(dt);
                if (strErrorPartId != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "保存失败：" + strErrorPartId;
                    apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0902", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 打印
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
            string vcPrint = dataForm.vcPrint;
            DataTable dtPorType = new DataTable();
            JArray checkedInfo = dataForm._multipleSelection;
            List<Dictionary<string, Object>> listInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();
            if (listInfoData.Count == 0)
            {
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "最少选择一条数据！";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            try
            {
                string msg = string.Empty;
                string strPrinterName = logic.PrintMess(loginInfo.UserId);//获取打印机
                string RolePorType = logic09.getRoleTip(loginInfo.UserId);
                string[] str2;
                str2 = RolePorType.Split('*');

                DataTable dt = logic.SearchPrintTDB(vcPrint, str2);
                DataTable dtPrintCR = new DataTable();
                DataTable dtPrintCRLone = print.searchTBCreateCRExcep();//获得数据库表结构
                DataTable dtPrint = dtPrintCRLone.Clone();
                //print.TurnCate();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    #region 整理数据
                    string vcPartsNo = listInfoData[i]["vcPartsNo"].ToString();//品番
                    string vcDock = listInfoData[i]["vcDock"].ToString();//受入
                    string vcQuantityPerContainer = listInfoData[i]["vcQuantityPerContainer"].ToString();//收容数
                    string vcCarFamilyCode = listInfoData[i]["vcCarType"].ToString();//车型
                    string vcEDflag = listInfoData[i]["vcEDflag"].ToString();//紧急区分
                    string vcKBorderno = listInfoData[i]["vcKBorderno"].ToString();//看板订单号
                    string vcKBSerial = listInfoData[i]["vcKBSerial"].ToString();//连番
                    string vcTips = listInfoData[i]["vcTips"].ToString();//备注
                    string vcComDate01 = listInfoData[i]["vcComDate01"].ToString();//完成日、班值
                    string vcBanZhi01 = listInfoData[i]["vcBanZhi01"].ToString();
                    string vcComDate02 = listInfoData[i]["vcComDate02"].ToString();
                    string vcBanZhi02 = listInfoData[i]["vcBanZhi02"].ToString();
                    string vcComDate03 = listInfoData[i]["vcComDate03"].ToString();
                    string vcBanZhi03 = listInfoData[i]["vcBanZhi03"].ToString();
                    string vcComDate04 = listInfoData[i]["vcComDate04"].ToString();
                    string vcBanZhi04 = listInfoData[i]["vcBanZhi04"].ToString();

                    string vcSupplierCode = String.Empty;
                    string vcSupplierPlant = String.Empty;
                    string vcCpdCompany = String.Empty;
                    string vcPartsNameEN = String.Empty;
                    string vcPartsNameCHN = String.Empty;
                    string vcLogisticRoute = String.Empty;
                    string vcProject01 = String.Empty;
                    string vcProject02 = String.Empty;
                    string vcProject03 = String.Empty;
                    string vcProject04 = String.Empty;
                    string vcRemark1 = String.Empty;
                    string vcRemark2 = String.Empty;


                    DataTable dtKANB = print.searchProRuleMst(vcPartsNo, vcDock);
                    string vcmage = "";//图片
                    string picnull = _webHostEnvironment.ContentRootPath + "Doc\\Image\\SPPartImage\\picnull.JPG";
                    byte[] vcPhotoPath = print.PhotoToArray(vcmage, picnull);//图片二进制流
                    if (dtKANB.Rows.Count != 0)
                    {
                        //检索打印看板的其他字段 
                        //订单号、紧急区分searchPrintKANB
                        vcmage = dtKANB.Rows[0]["vcPhotoPath"].ToString();//图片
                        vcPhotoPath = print.PhotoToArray(vcmage, picnull);//图片二进制流
                        vcSupplierCode = dtKANB.Rows[0]["vcSupplierCode"].ToString();//供应商
                        vcSupplierPlant = dtKANB.Rows[0]["vcSupplierPlant"].ToString();//供应商工区
                        vcCpdCompany = dtKANB.Rows[0]["vcCpdCompany"].ToString();//收货方
                        vcPartsNameEN = dtKANB.Rows[0]["vcPartsNameEN"].ToString();//中英文品名
                        vcPartsNameCHN = dtKANB.Rows[0]["vcPartsNameCHN"].ToString();
                        vcLogisticRoute = dtKANB.Rows[0]["vcLogisticRoute"].ToString();//路径
                        vcProject01 = dtKANB.Rows[0]["vcProName1"].ToString();//工程
                        vcProject02 = dtKANB.Rows[0]["vcProName2"].ToString();
                        vcProject03 = dtKANB.Rows[0]["vcProName3"].ToString();
                        vcProject04 = dtKANB.Rows[0]["vcProName4"].ToString();
                        vcRemark1 = dtKANB.Rows[0]["vcRemark1"].ToString();//特记事项
                        vcRemark2 = dtKANB.Rows[0]["vcRemark2"].ToString();
                    }
                    string vcKBorser = vcKBorderno + vcDock;
                    DataRow row = dtPrint.NewRow();
                    #region
                    row[0] = vcSupplierCode.ToUpper();
                    row[1] = vcCpdCompany.ToUpper();
                    row[2] = vcCarFamilyCode;
                    row[3] = (vcPartsNo.Length == 12 ? (vcPartsNo.Substring(0, 5) + "-" + vcPartsNo.Substring(5, 5) + "-" + vcPartsNo.Substring(10, 2)) : (vcPartsNo.Substring(0, 5) + "-" + vcPartsNo.Substring(5, 5))).ToUpper();
                    row[4] = vcPartsNameEN;
                    row[5] = vcPartsNameCHN;
                    row[6] = vcLogisticRoute;
                    row[7] = vcQuantityPerContainer;
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
                    row[24] = vcTips;
                    row[25] = vcDock;
                    row[26] = vcKBorser.ToUpper();
                    row[27] = Convert.ToString(i + 1).ToString();
                    row[28] = vcKBorderno.ToUpper();
                    row[29] = "QW";
                    row[30] = vcEDflag == "E" ? "紧急" : "";
                    #endregion
                    dtPrint.Rows.Add(row);
                    #endregion
                }
                dtPrint = print.orderDataTable(dtPrint);//排序
                print.insertTableCRExcep(dtPrint);//插入打印临时子表
                                                  //DataTable dtPorType = print.searchPorTypeExcep();//取生产部署
                dtPorType = QueryGroup(dtPrint);//用订单号 生产部署 生产日期 生产班值分组,修改不在数据库中取值
                print.insertTableCRMainEND(dtPrint, dtPorType);//插入打印临时主表
                string reportPath = "CryReportEX.rpt";
                string strLoginId = loginInfo.UserId;
                for (int z = 0; z < dtPorType.Rows.Count; z++)
                {
                    //bool retb = print.printCr(reportPath, dtPorType.Rows[z]["vcPorType"].ToString(), "", "", "", "", "", strLoginId, strPrinterName);//打印水晶报表                                                                                  //删除看板打印的临时文件
                    msg = print.printCr(reportPath, dtPorType.Rows[z]["vcPorType"].ToString(), "", "", "", "", "", strLoginId, strPrinterName);//打印水晶报表
                    logic09.DeleteprinterCREX2(dtPorType.Rows[z]["vcPorType"].ToString(), dtPorType.Rows[z]["vcorderno"].ToString(), dtPorType.Rows[z]["vcComDate01"].ToString(), dtPorType.Rows[z]["vcBanZhi01"].ToString());
                }
                print.UpdatePrintKANBCRExcep(dtPrint);
                if (msg == "打印成功")
                {
                    apiResult.code = ComConstant.SUCCESS_CODE;
                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                }
                apiResult.data = msg;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                if (dtPorType != null)
                {
                    for (int i = 0; i < dtPorType.Rows.Count; i++)
                    {
                        logic09.DeleteprinterCREX2(dtPorType.Rows[i]["vcPorType"].ToString(), dtPorType.Rows[i]["vcorderno"].ToString(), dtPorType.Rows[i]["vcComDate01"].ToString(), dtPorType.Rows[i]["vcBanZhi01"].ToString());
                    }
                }
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "打印失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 通用方法
        public DataTable ListToDataTable(List<Dictionary<string, Object>> listInfoData)
        {
            try
            {
                DataTable tb = new DataTable();
                if (listInfoData.Count > 0)
                {
                    Dictionary<string, object> li = listInfoData[0];
                    for (int i = 0; i < li.Count - 1; i++)
                    {
                        string colName = li.ToList()[i].Key;
                        tb.Columns.Add(new DataColumn(colName, typeof(string)));
                    }
                    foreach (Dictionary<string, object> li1 in listInfoData)
                    {
                        DataRow r = tb.NewRow();
                        for (int j = 0; j < tb.Columns.Count; j++)
                            r[j] = (li1[tb.Columns[j].ColumnName] == null) ? null : li1[tb.Columns[j].ColumnName].ToString();
                        tb.Rows.Add(r);
                    }
                }
                return tb;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region DataTable分组
        public DataTable QueryGroup(DataTable dt)
        {
            int a = dt.Rows.Count;
            DataTable dtPorType = new DataTable("dtPorType");
            DataColumn dc1 = new DataColumn("vcorderno", Type.GetType("System.String"));
            DataColumn dc2 = new DataColumn("vcPorType", Type.GetType("System.String"));
            DataColumn dc3 = new DataColumn("vcComDate01", Type.GetType("System.String"));
            DataColumn dc4 = new DataColumn("vcBanZhi01", Type.GetType("System.String"));
            dtPorType.Columns.Add(dc1);
            dtPorType.Columns.Add(dc2);
            dtPorType.Columns.Add(dc3);
            dtPorType.Columns.Add(dc4);
            var query = from t in dt.AsEnumerable()
                        group t by new { t1 = t.Field<string>("vcorderno"), t2 = t.Field<string>("vcPorType"), t3 = t.Field<string>("vcComDate01"), t4 = t.Field<string>("vcBanZhi01") } into m
                        select new
                        {
                            vcorderno = m.Key.t1,
                            vcPorType = m.Key.t2,
                            vcComDate01 = m.Key.t3,
                            vcBanZhi01 = m.Key.t4
                        };
            foreach (var item in query.ToList())
            {
                DataRow dr = dtPorType.NewRow();
                dr["vcorderno"] = item.vcorderno;
                dr["vcPorType"] = item.vcPorType;
                dr["vcComDate01"] = item.vcComDate01; ;
                dr["vcBanZhi01"] = item.vcBanZhi01;
                dtPorType.Rows.Add(dr);
            }
            return dtPorType;
        }
        #endregion
        #endregion
    }
}
