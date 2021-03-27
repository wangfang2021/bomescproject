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
    [Route("api/FS1207_Sub1/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1207Controller_Sub1 : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        FS1207_Logic logic = new FS1207_Logic();

        private readonly string FunctionID = "FS1207";

        public FS1207Controller_Sub1(IWebHostEnvironment webHostEnvironment)
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
                List<Object> dataList_SaleUserSource = ComFunction.convertAllToResult(logic.ddlSaleUser());
                res.Add("SaleUserSource", dataList_SaleUserSource);
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

        #region 未发注检索
        [HttpPost]
        [EnableCors("any")]
        public string searchFZJSApi([FromBody] dynamic data)
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
            string vcMon = dataForm.vcMon == null ? "" : dataForm.vcMon;
            string vcPartsNo = dataForm.vcPartsNo == null ? "" : dataForm.vcPartsNo;
            if (!string.IsNullOrEmpty(vcPartsNo))
            {
                vcPartsNo = vcPartsNo.Replace("-", "").ToString();
            }
            try
            {
                DataTable dtNoExict = logic.NoExict(vcMon);
                if (dtNoExict.Rows.Count > 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "品番" + dtNoExict.Rows[0]["vcPartsNo"] + "未在基础数据中维护";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                DataTable dt = logic.searchFZJS(vcMon, vcPartsNo);
                if (dt != null && dt.Rows.Count > 0)
                {
                    DtConverter dtConverter = new DtConverter();
                    dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                    dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                    List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.data = dataList;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
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

        #region 已发注检索
        [HttpPost]
        [EnableCors("any")]
        public string searchFZFinshApi([FromBody] dynamic data)
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
            string vcMon = dataForm.vcMon == null ? "" : dataForm.vcMon;
            string vcPartsNo = dataForm.vcPartsNo == null ? "" : dataForm.vcPartsNo;
            if (!string.IsNullOrEmpty(vcPartsNo))
            {
                vcPartsNo = vcPartsNo.Replace("-", "").ToString();
            }
            try
            {
                DataTable dt = logic.searchFZFinsh(vcMon, vcPartsNo);
                if (dt != null && dt.Rows.Count > 0)
                {
                    DtConverter dtConverter = new DtConverter();
                    dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                    dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                    List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.data = dataList;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
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
            ApiResult apiResult = new ApiResult();
            //以下开始业务处理
            dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
            string vcMon = dataForm.vcMon;
            string vcPartsNo = dataForm.vcPartsNo;
            string vcYesOrNo = dataForm.vcYesOrNo;
            vcMon = vcMon == null ? "" : vcMon;
            vcPartsNo = vcPartsNo == null ? "" : vcPartsNo;
            vcYesOrNo = vcYesOrNo == null ? "" : vcYesOrNo;
            if (!string.IsNullOrEmpty(vcPartsNo))
            {
                vcPartsNo = vcPartsNo.Replace("-", "").ToString();
            }
            try
            {
                string _msg;
                DataTable dt = logic.GetFzjsRenders(vcMon, vcPartsNo, vcYesOrNo, out _msg);
                string[] fields = { "vcMonth", "vcPartsNo","iSRNum","Total","iXZNum","iBYNum",
                "iFZNum","syco","iCONum"
                };
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS1207_Sub1_Export.xlsx", 1, loginInfo.UserId, FunctionID);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0904", ex, loginInfo.UserId);
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
                    hasFind = true;
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
                    string[,] strField = new string[,] {{"修正数","发注数","本月CO数","对象月","品番"},
                                                {"iXZNum","iFZNum","iCONum","vcMonth","vcPartsNo"},
                                                {FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,"",""},
                                                {"10","10","10","20","20"},//最大长度设定,不校验最大长度用0
                                                {"1","1","1","1","0"},//最小长度设定,可以为空用0
                                                {"1","2","3","4","5"}//前台显示列号，从0开始计算,注意有选择框的是0
                    };
                    List<Object> checkRes = ListChecker.validateList(listInfoData, strField, null, null, true, "FS1207_Sub1");
                    if (checkRes != null)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = checkRes;
                        apiResult.flag = Convert.ToInt32(ERROR_FLAG.单元格定位提示);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                string strErrorName = "";
                DataTable dt = ListToDataTable(listInfoData);
                strErrorName = logic.UpdateFZJSEdit(dt, loginInfo.UserId);
                if (strErrorName != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "保存失败," + strErrorName;
                    apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0908", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 发注
        [HttpPost]
        [EnableCors("any")]
        public string UpdateFZJSEditFZ([FromBody] dynamic data)
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
            string type = dataForm.vcType == null ? "" : dataForm.vcType;
            string orderlb = dataForm.vcOrder == null ? "" : dataForm.vcOrder;
            string vcSaleUser = dataForm.vcSaleUser == null ? "" : dataForm.vcSaleUser;
            string vcMon = dataForm.vcMon == null ? "" : dataForm.vcMon;
            string vcPartsNo = dataForm.vcPartsNo == null ? "" : dataForm.vcPartsNo;
            if (!string.IsNullOrEmpty(vcPartsNo))
                vcPartsNo = vcPartsNo.Replace("-", "").ToString();

            string UserId = loginInfo.UserId;

            DataTable dtNoExict = logic.NoExict(vcMon);
            if (dtNoExict.Rows.Count > 0)
            {
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "品番" + dtNoExict.Rows[0]["vcPartsNo"] + "未在基础数据中维护";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            DataTable dtSSPtmp = logic.searchFZJS(vcMon, vcPartsNo);
            string msg = string.Empty;
            try
            {
                string exlName = "";
                string date = DateTime.Now.ToString("yyyy年M月d日");
                string date2 = DateTime.Now.ToString("yy-M");

                DataTable dtHeader = new DataTable();//表头信息
                dtHeader.Columns.Add("vcDate");//日期
                dtHeader.Columns.Add("vcSalerName");//高新
                dtHeader.Columns.Add("vcSalerPhone");//高新电话
                dtHeader.Columns.Add("vcSalerEMail");//高新邮箱
                dtHeader.Columns.Add("vcOrderNo");//订单编号（不带TM2）
                dtHeader.Columns.Add("vcItemTotal");//项目总数
                dtHeader.Columns.Add("vcOrderQtyTotal");//订货总件数
                dtHeader.Columns.Add("vcUserName");//刘家成
                dtHeader.Columns.Add("vcUserPhone");//刘家成电话
                dtHeader.Columns.Add("vcUserEMail");//刘家成邮箱
                dtHeader.Columns.Add("vcOrderType");//订单类别
                dtHeader.Columns.Add("vcChuanZhen");//丰田传真
                dtHeader.Columns.Add("vcChuanZhenSale"); //销售公司传真

                DataTable dtDetail = new DataTable();//数据信息
                dtDetail.Columns.Add("TASSCODE");//经销商代码
                dtDetail.Columns.Add("B");//B
                dtDetail.Columns.Add("C");//C
                dtDetail.Columns.Add("OrderDate");//订购日期
                dtDetail.Columns.Add("E");//E
                dtDetail.Columns.Add("F");//F
                dtDetail.Columns.Add("OrderNo");//订单编号（带TM2）
                dtDetail.Columns.Add("H");//H
                dtDetail.Columns.Add("I");//I
                dtDetail.Columns.Add("TYPE");//订单类型
                dtDetail.Columns.Add("ItemNo");//项目号（0000）
                dtDetail.Columns.Add("L");//L
                dtDetail.Columns.Add("PartNo");//零件编号即发注品番
                dtDetail.Columns.Add("N");//N
                dtDetail.Columns.Add("O");//O
                dtDetail.Columns.Add("P");//P
                dtDetail.Columns.Add("OrderQty", typeof(int));//订购数量
                dtDetail.Columns.Add("PToPCust");//代订客户

                if (dtSSPtmp.Rows.Count == 0 || dtSSPtmp == null)
                {
                    return "无发注数据，请进行检索检索数据！";
                }
                DataTable dtSSP = new DataTable();
                dtSSP = dtSSPtmp.Clone();
                DataRow[] rows = dtSSPtmp.Select("vcSource='" + type + "' and iFZNum>=0 ");
                if (rows.Length > 0)
                {
                    foreach (DataRow row in rows)
                    {
                        dtSSP.ImportRow(row);
                    }
                }
                else
                {
                    return "无发注数据！";
                }

                //获取销售公司信息

                DataTable dtSaleuser = logic.GetSaleuser(UserId);
                //获取发注担当信息
                DataTable dtUser = logic.GetUser(UserId);
                if (dtUser.Rows.Count == 0)
                {
                    return "不是发注担当，不能进行发注！";
                }

                //订单号作成：
                string Year = date2.Split('-')[0];
                string Month = date2.Split('-')[1];
                string OrderType = string.Empty;
                string TYPE = string.Empty;
                int OrderQtyTotal = 0;//订货总件数

                if (Month == "10")
                {
                    Month = "J";
                }
                else if (Month == "11")
                {
                    Month = "Q";
                }
                else if (Month == "12")
                {
                    Month = "K";
                }
                switch (type)
                {
                    case "JSP": OrderType = "01"; break;
                    case "MSP": OrderType = "02"; break;
                }
                switch (orderlb)
                {
                    case "S/O": TYPE = "S"; break;
                    case "E/O": TYPE = "E"; break;
                    case "F/O": TYPE = "F"; break;
                }
                string vcOrderNo = Year + Month + OrderType;//表头用
                string OrderNo = "TM2" + vcOrderNo;//数据用

                //将信息写到模板中 单元格必须留一行空行(dtSSP中已有发注品番、数量、类别）
                //数据信息：

                for (int i = 0; i < dtSSP.Rows.Count; i++)
                {
                    if (dtSSP.Rows[i]["iFZNum"].ToString() != "0")//去除发注数为0
                    {
                        int ItemNo = i + 1;
                        DataRow drDetail = dtDetail.NewRow();
                        drDetail["TASSCODE"] = "TFTM2";
                        drDetail["OrderDate"] = DateTime.Now.ToString("yyyyMMdd");
                        drDetail["OrderNo"] = OrderNo;
                        drDetail["TYPE"] = TYPE;
                        drDetail["ItemNo"] = ItemNo.ToString("0000");
                        drDetail["PartNo"] = dtSSP.Rows[i]["vcPartsNoFZ"].ToString();
                        drDetail["OrderQty"] = Convert.ToInt32(dtSSP.Rows[i]["iFZNum"].ToString());
                        OrderQtyTotal = OrderQtyTotal + Convert.ToInt32(dtSSP.Rows[i]["iFZNum"].ToString());//总件数累加
                        dtDetail.Rows.Add(drDetail);
                    }
                }

                //排序
                DataView dv = dtDetail.DefaultView;
                dv.Sort = "ItemNo";
                dtDetail = dv.ToTable();
                //添加空行
                //DataRow drDNull = dtDetail.NewRow();
                //dtDetail.Rows.Add(drDNull);
                //表头信息：

                DataRow drHeader = dtHeader.NewRow();
                drHeader["vcDate"] = date;
                drHeader["vcSalerName"] = dtSaleuser.Rows[0]["name"].ToString();
                drHeader["vcSalerPhone"] = dtSaleuser.Rows[0]["pphone"].ToString();
                drHeader["vcSalerEMail"] = dtSaleuser.Rows[0]["email"].ToString();
                drHeader["vcChuanZhenSale"] = dtSaleuser.Rows[0]["ChuanZhenSale"].ToString();
                drHeader["vcOrderNo"] = vcOrderNo;
                drHeader["vcItemTotal"] = (dtDetail.Rows.Count).ToString();//多一空行，算总项目数时要减掉
                drHeader["vcOrderQtyTotal"] = OrderQtyTotal.ToString();
                drHeader["vcUserName"] = dtUser.Rows[0]["Username"].ToString();
                drHeader["vcUserPhone"] = dtUser.Rows[0]["UserPhone"].ToString();
                drHeader["vcUserEMail"] = dtUser.Rows[0]["Useremail"].ToString();
                drHeader["vcOrderType"] = orderlb;
                drHeader["vcChuanZhen"] = dtUser.Rows[0]["ChuanZhen"].ToString();
                dtHeader.Rows.Add(drHeader);
                //更新tSSP表中 iFZFlg='1',iXZNum ,iFZNum,iCONum
                //201903  修改发注数为0，更新余数
                msg = logic.updSSP(dtSSP, UserId);
                //end
                if (msg.Length <= 0)
                {
                    if (type == "JSP")
                    {
                        exlName = "发注订货单" + System.DateTime.Now.ToString("yyyyMMdd") + "";
                    }
                    else
                    {
                        exlName = "发注订货单" + System.DateTime.Now.ToString("yyyyMMdd") + "B";
                    }
                    if (dtDetail.Rows.Count == 0)
                    {
                        msg = "无发注数据！";
                    }
                    else
                    {
                        msg = "发注成功！";
                        exlName = exlName + ".xls";
                        //QMExcel oQMExcel = new QMExcel();
                        //string tmplatePath = System.Web.HttpContext.Current.Server.MapPath("~/Templates/FS1207_SSP.xlt");//模板路径
                        //string path = System.Web.HttpContext.Current.Server.MapPath("~/Temps/" + exlName);//文件路径
                        //oQMExcel.ExportFromTemplate(dtHeader, dtDetail, tmplatePath, path, 19, 1, false);
                    }
                    dtSSP.Dispose();
                }
                else
                {
                    msg = "导出失败：";
                }
                if (msg == "")
                {
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.data = null;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = msg;
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

        #region 通用方法
        public DataTable ListToDataTable(List<Dictionary<string, Object>> listInfoData)
        {
            DataTable tb = new DataTable();
            if (listInfoData.Count > 0)
            {
                Dictionary<string, object> li = listInfoData[0];
                for (int i = 0; i < li.Count; i++)
                {
                    string colName = li.ToList()[i].Key;
                    string colType = li.ToList()[i].Value.GetType().Name;
                    tb.Columns.Add(new DataColumn(colName, li.ToList()[i].Value.GetType()));
                }
                foreach (Dictionary<string, object> li1 in listInfoData)
                {
                    DataRow r = tb.NewRow();
                    for (int j = 0; j < tb.Columns.Count; j++)
                        r[j] = li1[tb.Columns[j].ColumnName].ToString();
                    tb.Rows.Add(r);
                }
            }
            return tb;
        }
        #endregion
    }
}
