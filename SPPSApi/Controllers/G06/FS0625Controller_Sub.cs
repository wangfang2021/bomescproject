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
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace SPPSApi.Controllers.G06
{
    [Route("api/FS0625_Sub/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0625Controller_Sub : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0625_Logic fs0625_Logic = new FS0625_Logic();
        private readonly string FunctionID = "FS0625";

        public FS0625Controller_Sub(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }


        #region 获取邮件体
        [HttpPost]
        [EnableCors("any")]
        public string searchApi([FromBody]dynamic data)
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
            string date = dataForm.date == null ? "" : dataForm.date;
            string flag = dataForm.flag == null ? "" : dataForm.flag;
            string vcColor = dataForm.vcColor == null ? "" : dataForm.vcColor;
            string vcCarType = dataForm.vcCarType == null ? "" : dataForm.vcCarType;
            try
            {
                String emailBody = fs0625_Logic.CreateEmailBody(date, vcColor,flag, loginInfo.UnitCode, loginInfo.UserName, vcCarType);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = emailBody;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2510", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "生成邮件体失败" + ex.Message;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion


        #region FTMS

        [HttpPost]
        [EnableCors("any")]
        public string FTMSApi([FromBody] dynamic data)
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

                //以下开始业务处理
                
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                JArray listInfo = dataForm.parentFormSelectItem.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
                string EmailBody = dataForm.parentFormSelectItem.emailBody.ToString();
                string dExpectDeliveryDate = dataForm.date == null ? "" : dataForm.date;
                string vcColor = dataForm.vcColor == null ? "" : dataForm.vcColor;
                string vcCarTypeChuShi = dataForm.vcCarType == null ? "" : dataForm.vcCarType;
                if (EmailBody.Length==0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "邮件体内容不能为空！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (dExpectDeliveryDate.Length==0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "回复纳期不能为空！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (vcColor.Length == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "账票颜色不能为空！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (listInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请先检索数据，再进行年检邮件电送！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                #region 组织数据
               
                DataTable dt = new DataTable();
                //"dExportDate", "", "", "", ""
                dt.Columns.Add("dExportDate");
                dt.Columns.Add("vcCarType");
                dt.Columns.Add("vcPartNo");
                dt.Columns.Add("vcPartName");
                dt.Columns.Add("vcInsideOutsideType");
                dt.Columns.Add("vcSupplier_id");
                dt.Columns.Add("vcSupplier_name");
                dt.Columns.Add("vcWorkArea");
                dt.Columns.Add("vcIsNewRulesFlag");
                dt.Columns.Add("vcOEOrSP");
                dt.Columns.Add("vcDock");
                dt.Columns.Add("vcNumber");
                dt.Columns.Add("vcPurposes");
                dt.Columns.Add("dOrderPurposesDate");
                dt.Columns.Add("dOrderReceiveDate");
                dt.Columns.Add("vcReceiveTimes");
                dt.Columns.Add("dActualReceiveDate");
                dt.Columns.Add("vcAccountOrderNo");
                dt.Columns.Add("dAccountOrderReceiveDate");
                dt.Columns.Add("vcMemo");

                for (int i = 0; i < listInfoData.Count; i++)
                {
                    DataRow dr = dt.NewRow();
                    dr["dExportDate"] = listInfoData[i]["dExportDate"] == null ? "" : listInfoData[i]["dExportDate"].ToString();
                    dr["vcCarType"] = listInfoData[i]["vcCarType"] == null ? "" : listInfoData[i]["vcCarType"].ToString();
                    dr["vcPartNo"] = listInfoData[i]["vcPartNo"] == null ? "" : listInfoData[i]["vcPartNo"].ToString();
                    dr["vcPartName"] = listInfoData[i]["vcPartName"] == null ? "" : listInfoData[i]["vcPartName"].ToString();

                    string vcInsideOutsideType = listInfoData[i]["vcInsideOutsideType"] == null ? "" : listInfoData[i]["vcInsideOutsideType"].ToString();
                    if (vcInsideOutsideType == "1")
                    {
                        dr["vcInsideOutsideType"] = "外注";
                    }
                    else if (vcInsideOutsideType == "0")
                    {
                        dr["vcInsideOutsideType"] = "内制";
                    }
                    else
                    {
                        dr["vcInsideOutsideType"] = "";
                    }
                    dr["vcSupplier_id"] = listInfoData[i]["vcSupplier_id"] == null ? "" : listInfoData[i]["vcSupplier_id"].ToString();
                    dr["vcSupplier_name"] = listInfoData[i]["vcSupplier_name"] == null ? "" : listInfoData[i]["vcSupplier_name"].ToString();
                    dr["vcWorkArea"] = listInfoData[i]["vcWorkArea"] == null ? "" : listInfoData[i]["vcWorkArea"].ToString();
                    string vcIsNewRulesFlag = listInfoData[i]["vcIsNewRulesFlag"] == null ? "" : listInfoData[i]["vcIsNewRulesFlag"].ToString();
                    if (vcIsNewRulesFlag == "1")
                    {
                        dr["vcIsNewRulesFlag"] = "是";
                    }
                    else if (vcIsNewRulesFlag == "0")
                    {
                        dr["vcIsNewRulesFlag"] = "否";
                    }
                    else
                    {
                        dr["vcIsNewRulesFlag"] = "";
                    }

                    dr["vcOEOrSP"] = listInfoData[i]["vcOEOrSP"] == null ? "" : listInfoData[i]["vcOEOrSP"].ToString();
                    dr["vcDock"] = listInfoData[i]["vcDock"] == null ? "" : listInfoData[i]["vcDock"].ToString();
                    dr["vcNumber"] = listInfoData[i]["vcNumber"] == null ? "" : listInfoData[i]["vcNumber"].ToString();
                    dr["vcPurposes"] = listInfoData[i]["vcPurposes"] == null ? "" : listInfoData[i]["vcPurposes"].ToString();
                    dr["dOrderPurposesDate"] = listInfoData[i]["dOrderPurposesDate"] == null ? "" : listInfoData[i]["dOrderPurposesDate"].ToString();
                    dr["dOrderReceiveDate"] = listInfoData[i]["dOrderReceiveDate"] == null ? "" : listInfoData[i]["dOrderReceiveDate"].ToString();
                    dr["vcReceiveTimes"] = listInfoData[i]["vcReceiveTimes"] == null ? "" : listInfoData[i]["vcReceiveTimes"].ToString();
                    dr["dActualReceiveDate"] = listInfoData[i]["dActualReceiveDate"] == null ? "" : listInfoData[i]["dActualReceiveDate"].ToString();
                    dr["vcAccountOrderNo"] = listInfoData[i]["vcAccountOrderNo"] == null ? "" : listInfoData[i]["vcAccountOrderNo"].ToString();
                    dr["dAccountOrderReceiveDate"] = listInfoData[i]["dAccountOrderReceiveDate"] == null ? "" : listInfoData[i]["dAccountOrderReceiveDate"].ToString();
                    dr["vcMemo"] = listInfoData[i]["vcMemo"] == null ? "" : listInfoData[i]["vcMemo"].ToString();
                    dt.Rows.Add(dr);
                }
                #endregion
                //Console.WriteLine("FS0625 Data获取成功");
                string[] columnArray = { "vcSupplier_id", "vcSupplier_name" };
                DataView dtSelectView = dt.DefaultView;
                DataTable dtSelect = dtSelectView.ToTable(true, columnArray);//去重后的dt 

                string logName = System.DateTime.Now.ToString("yyyyMMddHHmmss") + Guid.NewGuid().ToString("N");
                string logs = string.Empty;

                string[] head = new string[] { };
                string[] field = new string[] { };
                //[vcPartNo], [dBeginDate], [dEndDate]
                //const tHeader = [];
                //const filterVal = [,   ];

                head = new string[] { "导入时间", "车型", "品番", "品名", "内外", "供应商代码", "工区", "是否新规", "OE=SP", "受入", "号试数量", "号试目的", "订单预计发行日", "订单预计纳入日", "纳入便次", "实际纳入日", "结算订单号", "结算订单验收日期", "备注" };
                field = new string[] { "dExportDate", "vcCarType", "vcPartNo", "vcPartName", "vcInsideOutsideType", "vcSupplier_id", "vcWorkArea", "vcIsNewRulesFlag", "vcOEOrSP", "vcDock", "vcNumber", "vcPurposes", "dOrderPurposesDate", "dOrderReceiveDate", "vcReceiveTimes", "dActualReceiveDate", "vcAccountOrderNo", "dAccountOrderReceiveDate", "vcMemo" };
                string path = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Export" + Path.DirectorySeparatorChar;
                StringBuilder strErr = new StringBuilder();
                DataTable dtMessage = fs0625_Logic.createTable("supplier");
                bool bReault = true;
                for (int i = 0; i < dtSelect.Rows.Count; i++)
                {
                    //Console.WriteLine("FS0625 开始查找邮箱");
                    //组织制定供应商和工区的数据
                    string[] strFilePathArray = new string[3];
                    string vcSupplier_id = dtSelect.Rows[i]["vcSupplier_id"].ToString();
                    string vcSupplier_name = dtSelect.Rows[i]["vcSupplier_name"].ToString();
                    //string vcWorkArea = dtSelect.Rows[i]["vcWorkArea"].ToString();
                    DataRow[] drArray = dt.Select("vcSupplier_id='" + vcSupplier_id + "'  ");//and vcWorkArea='" + vcWorkArea + "'
                    DataTable dtNewSupplierandWorkArea = drArray[0].Table.Clone(); // 复制DataRow的表结构
                    string msg = string.Empty;
                    //Console.WriteLine(vcSupplier_id+":"+ vcSupplier_name);
                    foreach (DataRow dr in drArray)
                    {
                        dtNewSupplierandWorkArea.ImportRow(dr);
                    }
                    //Console.WriteLine("已经组织好当前供应商数据");
                    //获取供应商 工区的邮箱
                    //DataTable dtEmail = fs0625_Logic.getEmail(vcSupplier_id, vcWorkArea);
                    DataTable dtEmail = fs0625_Logic.getEmail(vcSupplier_id);
                    if (dtEmail.Rows.Count == 0)
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcSupplier"] = vcSupplier_id;
                        dataRow["vcMessage"] = "没有维护供应商，不能发送邮件";
                        dtMessage.Rows.Add(dataRow);
                        bReault = false;
                        continue;
                    }
                    //Console.WriteLine(vcSupplier_id+ "没有维护供应商，不能发送邮件");
                    DataTable receiverDt = new DataTable();
                    receiverDt.Columns.Add("address");
                    receiverDt.Columns.Add("displayName");

                    for (int m = 0; m < dtEmail.Rows.Count; m++)
                    {
                        string strdisplayName1 = dtEmail.Rows[m]["vcLinkMan1"].ToString();
                        if (string.IsNullOrEmpty(strdisplayName1))
                        {
                            strdisplayName1 = "";
                        }
                        string strdisplayName2 = dtEmail.Rows[m]["vcLinkMan2"].ToString();
                        if (string.IsNullOrEmpty(strdisplayName2))
                        {
                            strdisplayName2 = "";
                        }
                        string strdisplayName3 = dtEmail.Rows[m]["vcLinkMan3"].ToString();
                        if (string.IsNullOrEmpty(strdisplayName3))
                        {
                            strdisplayName3 = "";
                        }
                        string strEmail1 = dtEmail.Rows[m]["vcEmail1"].ToString();
                        string strEmail2 = dtEmail.Rows[m]["vcEmail2"].ToString();
                        string strEmail3 = dtEmail.Rows[m]["vcEmail3"].ToString();
                        if (strEmail1.Length > 0)
                        {
                            DataRow dr = receiverDt.NewRow();
                            dr["address"] = strEmail1.ToString();
                            dr["displayName"] = strdisplayName1;
                            receiverDt.Rows.Add(dr);
                        }

                        if (strEmail2.Length > 0)
                        {
                            DataRow dr = receiverDt.NewRow();
                            dr["address"] = strEmail2.ToString();
                            dr["displayName"] = strdisplayName2;
                            receiverDt.Rows.Add(dr);
                        }
                        if (strEmail3.Length > 0)
                        {
                            DataRow dr = receiverDt.NewRow();
                            dr["address"] = strEmail3.ToString();
                            dr["displayName"] = strdisplayName3;
                            receiverDt.Rows.Add(dr);
                        }
                    }
                    //Console.WriteLine(vcSupplier_id + "三个邮箱联系人");
                    if (receiverDt.Rows.Count == 0)
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcSupplier"] = vcSupplier_id;
                        dataRow["vcMessage"] = "维护的邮箱为空,不能发送邮件!";
                        dtMessage.Rows.Add(dataRow);
                        bReault = false;
                        continue;
                    }
                    #region 第一个附件 号试看板标签
                    XSSFWorkbook hssfworkbook = new XSSFWorkbook();//用于创建xlsx
                    string XltPath = "." + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Template" + Path.DirectorySeparatorChar + "FS0625_HSKanBanBiaoQian.xlsx";
                    using (FileStream fs = System.IO.File.OpenRead(XltPath))
                    {
                        hssfworkbook = new XSSFWorkbook(fs);
                        fs.Close();
                    }
                    ISheet mysheetHSSF = hssfworkbook.GetSheetAt(0);//获得sheet
                    #region 第一个附件的样式
                    #region 设置单元格对齐方式
                    //创建CellStyle  
                    //ICellStyle styleGeneral = hssfworkbook.CreateCellStyle();
                    //styleGeneral.Alignment = HorizontalAlignment.General;//【General】数字、时间默认：右对齐；BOOL：默认居中；字符串：默认左对齐  

                    //ICellStyle styleLeft = hssfworkbook.CreateCellStyle();
                    //styleLeft.Alignment = HorizontalAlignment.Left;//【Left】左对齐  

                    //ICellStyle styleCenter = hssfworkbook.CreateCellStyle();
                    //styleCenter.Alignment = HorizontalAlignment.Center;//【Center】居中  

                    //ICellStyle styleRight = hssfworkbook.CreateCellStyle();
                    //styleRight.Alignment = HorizontalAlignment.Right;//【Right】右对齐  

                    //ICellStyle styleFill = hssfworkbook.CreateCellStyle();
                    //styleFill.Alignment = HorizontalAlignment.Fill;//【Fill】填充  

                    //ICellStyle styleJustify = hssfworkbook.CreateCellStyle();
                    //styleJustify.Alignment = HorizontalAlignment.Justify;//【Justify】两端对齐[会自动换行]（主要针对英文）  
                    //ICellStyle styleCenterSelection = hssfworkbook.CreateCellStyle();
                    //styleCenterSelection.Alignment = HorizontalAlignment.CenterSelection;//【CenterSelection】跨列居中  

                    //ICellStyle styleDistributed = hssfworkbook.CreateCellStyle();
                    //styleDistributed.Alignment = HorizontalAlignment.Distributed;//【Distributed】分散对齐[会自动换行]
                    #endregion
                    #region 设置字体样式
                    ICellStyle style1 = hssfworkbook.CreateCellStyle();//声明style1对象，设置Excel表格的样式 标题 供应商番号
                    ICellStyle style2 = hssfworkbook.CreateCellStyle();//9号字体加粗 没有背景色 内制 外注字体 供应商番号具体
                    ICellStyle style3 = hssfworkbook.CreateCellStyle();//9号字体不加粗 没有背景色 2019/12 所番地
                    ICellStyle style4 = hssfworkbook.CreateCellStyle();//9号字体不加粗 深蓝蓝 工程区分  使用目的左边
                    ICellStyle style5 = hssfworkbook.CreateCellStyle();//9号字体不加粗 紫色 具体内容  供应商名称
                    ICellStyle style6 = hssfworkbook.CreateCellStyle();//9号字体不加粗 结尾 供应商名称 右边
                    ICellStyle style7 = hssfworkbook.CreateCellStyle();//9号字体不加粗 以上 所番地下方
                    ICellStyle style8 = hssfworkbook.CreateCellStyle();//9号字体不加粗 以上 youwenti
                    ICellStyle style9 = hssfworkbook.CreateCellStyle();//9号字体不加粗 以上  
                    ICellStyle style91 = hssfworkbook.CreateCellStyle();//9号字体不加粗 以上  品名
                    ICellStyle style10 = hssfworkbook.CreateCellStyle();//9号字体不加粗 以上  品番
                    ICellStyle style111 = hssfworkbook.CreateCellStyle();//9号字体不加粗 以上  色番右边边的
                    ICellStyle style112 = hssfworkbook.CreateCellStyle();//9号字体不加粗 以上  色番左边边边的
                    ICellStyle style11 = hssfworkbook.CreateCellStyle();//9号字体不加粗 以上  色番下边的
                    ICellStyle style12 = hssfworkbook.CreateCellStyle();//9号字体不加粗 以上  收入低右边
                    ICellStyle style13 = hssfworkbook.CreateCellStyle();//9号字体不加粗 以上  交货日期下边
                    ICellStyle style14 = hssfworkbook.CreateCellStyle();//9号字体不加粗 以上  被番号下边
                    ICellStyle style15 = hssfworkbook.CreateCellStyle();//9号字体不加粗 以上  本箱子应装个数
                    ICellStyle style16 = hssfworkbook.CreateCellStyle();//9号字体不加粗 以上  看板连番号下边

                    ICellStyle style17 = hssfworkbook.CreateCellStyle();//9号字体不加粗 以上  供应商番号右边 合并空白
                    ICellStyle style18 = hssfworkbook.CreateCellStyle();//9号字体不加粗 以上  供应商号下边
                    ICellStyle style19 = hssfworkbook.CreateCellStyle();//9号字体不加粗 以上  品名下边
                    ICellStyle style20 = hssfworkbook.CreateCellStyle();//9号字体不加粗 以上  最后一个格子

                    ICellStyle style21 = hssfworkbook.CreateCellStyle();//品番拆后下边的
                    ICellStyle style22 = hssfworkbook.CreateCellStyle();//交货周期下方右边品番拆后下边的
                    ICellStyle style23 = hssfworkbook.CreateCellStyle();//本箱子应装个数右边品番拆后下边的

                    ICellStyle style24 = hssfworkbook.CreateCellStyle();//注意字体
                    ICellStyle style25 = hssfworkbook.CreateCellStyle();//红色的补给课
                    ICellStyle style26 = hssfworkbook.CreateCellStyle();//分割线

                    ICellStyle style27 = hssfworkbook.CreateCellStyle();//分割线
                    ICellStyle style28 = hssfworkbook.CreateCellStyle();//分割线
                    ICellStyle style29 = hssfworkbook.CreateCellStyle();//
                    ICellStyle style6PartNo = hssfworkbook.CreateCellStyle();//

                    ICellStyle style71 = hssfworkbook.CreateCellStyle();//分割线
                    #region

                    IFont font = hssfworkbook.CreateFont();
                    font.Color = IndexedColors.Black.Index;
                    font.IsBold = true; ;
                    font.FontHeightInPoints = 14;
                    //font.FontName = "宋体";
                    style1.SetFont(font);
                    style1.Alignment = HorizontalAlignment.Center;//两端自动对齐（自动换行）
                    style1.VerticalAlignment = VerticalAlignment.Center;
                    style1.BorderLeft = BorderStyle.Medium;
                    //style1.BorderRight = BorderStyle.Thin;
                    style1.BorderTop = BorderStyle.Medium;
                    IFont font15 = hssfworkbook.CreateFont();
                    font15.Color = IndexedColors.Black.Index;
                    font15.IsBold = true; ;
                    font15.FontHeightInPoints = 18;

                    IFont font2 = hssfworkbook.CreateFont();
                    font2.Color = IndexedColors.Black.Index;
                    font2.IsBold = true; ;
                    font2.FontHeightInPoints = 16;
                    //font.FontName = "宋体";
                    style2.SetFont(font2);
                    style2.Alignment = HorizontalAlignment.Center;
                    style2.VerticalAlignment = VerticalAlignment.Center;
                    style2.BorderLeft = BorderStyle.Thin;
                    //style2.BorderRight = BorderStyle.Thin;
                    style2.BorderTop = BorderStyle.Thin;

                    style3.SetFont(font);
                    style3.Alignment = HorizontalAlignment.Center;
                    style3.VerticalAlignment = VerticalAlignment.Center;
                    style3.BorderLeft = BorderStyle.Thin;
                    //style1.BorderRight = BorderStyle.Thin;
                    style3.BorderTop = BorderStyle.Medium;

                    IFont font4 = hssfworkbook.CreateFont();
                    font4.Color = IndexedColors.Black.Index;
                    font4.IsBold = false; ;
                    font4.FontHeightInPoints = 14;
                    //font.FontName = "宋体";
                    style4.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;//灰色
                    style4.FillPattern = FillPattern.SolidForeground;
                    style4.SetFont(font4);
                    style4.Alignment = HorizontalAlignment.Center;
                    style4.VerticalAlignment = VerticalAlignment.Center;
                    //style4.BorderLeft = BorderStyle.Thin;
                    style4.BorderRight = BorderStyle.Medium;
                    style4.BorderTop = BorderStyle.Medium;

                    style5.SetFont(font);
                    style5.Alignment = HorizontalAlignment.Center;
                    style5.VerticalAlignment = VerticalAlignment.Center;
                    style5.BorderLeft = BorderStyle.Medium;
                    style5.BorderTop = BorderStyle.Dotted;

                    IFont font6 = hssfworkbook.CreateFont();
                    font6.Color = IndexedColors.Black.Index;
                    font6.IsBold = true; ;
                    font6.FontHeightInPoints = 14;
                    //font6.FontName = "宋体";
                    //style6.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightTurquoise.Index;
                    //style6.FillPattern = FillPattern.SolidForeground;
                    style6.SetFont(font6);
                    style6.Alignment = HorizontalAlignment.Center;
                    style6.VerticalAlignment = VerticalAlignment.Center;
                    style6.BorderLeft = BorderStyle.Thin;
                    //style6.BorderRight = BorderStyle.Thin;
                    style6.BorderTop = BorderStyle.Thin;
                    //style6.BorderBottom = BorderStyle.Medium;

                    IFont font6PartNo = hssfworkbook.CreateFont();
                    font6PartNo.Color = IndexedColors.Black.Index;
                    font6PartNo.IsBold = true; ;
                    font6PartNo.FontHeightInPoints = 24;

                    style6PartNo.SetFont(font6PartNo);
                    style6PartNo.Alignment = HorizontalAlignment.Center;
                    style6PartNo.VerticalAlignment = VerticalAlignment.Center;
                    style6PartNo.BorderLeft = BorderStyle.Thin;
                    style6PartNo.BorderTop = BorderStyle.Thin;
                    style6PartNo.Alignment = HorizontalAlignment.Center;
                    style6PartNo.VerticalAlignment = VerticalAlignment.Center;
                    style6PartNo.BorderLeft = BorderStyle.Thin;
                    style6PartNo.BorderTop = BorderStyle.Thin;

                    
                    style7.BorderTop = BorderStyle.Dotted;
                    style7.BorderLeft = BorderStyle.Thin;
                    style7.BorderBottom = BorderStyle.Thin;
                    //style7.BorderDiagonalLineStyle = BorderStyle.Thin;
                    //style7.BorderDiagonal = BorderDiagonal.Both;
                    //style7.BorderDiagonalColor = IndexedColors.Black.Index;
                    style71.SetFont(font15);
                    style71.BorderTop = BorderStyle.Dotted;
                    style71.BorderLeft = BorderStyle.Thin;
                    style71.BorderBottom = BorderStyle.Medium;
                    //style71.BorderDiagonalLineStyle = BorderStyle.Thin;
                    //style71.BorderDiagonal = BorderDiagonal.Both;
                    //style71.BorderDiagonalColor = IndexedColors.Black.Index;

                    //style6.BorderBottom = BorderStyle.Medium;

                    IFont font8 = hssfworkbook.CreateFont();
                    font8.Color = IndexedColors.Black.Index;
                    font8.IsBold = true; ;
                    font8.FontHeightInPoints = 14;
                    style8.SetFont(font8);
                    //style8.Alignment = HorizontalAlignment.Right;
                    //style8.VerticalAlignment = VerticalAlignment.Center;
                    //style8.BorderLeft = BorderStyle.Thin;
                    //style8.BorderTop = BorderStyle.Dotted;
                    style8.Alignment = HorizontalAlignment.Right;
                    style8.VerticalAlignment = VerticalAlignment.Center;
                    style8.BorderLeft = BorderStyle.Thin;
                    //style6.BorderRight = BorderStyle.Thin;
                    style8.BorderTop = BorderStyle.Dotted;

                    IFont font9 = hssfworkbook.CreateFont();
                    font9.Color = IndexedColors.Black.Index;
                    font9.IsBold = true; ;
                    font9.FontHeightInPoints = 14;
                    style9.SetFont(font9);
                    style9.Alignment = HorizontalAlignment.Center;
                    style9.VerticalAlignment = VerticalAlignment.Center;
                    style9.BorderLeft = BorderStyle.Thin;
                    style9.BorderRight = BorderStyle.Medium;
                    style9.BorderTop = BorderStyle.Dotted;

                    IFont font91 = hssfworkbook.CreateFont();
                    font91.Color = IndexedColors.Black.Index;
                    font91.IsBold = false; ;
                    font91.FontHeightInPoints = 16;
                    
                    style91.SetFont(font91);
                    style91.Alignment = HorizontalAlignment.Center;
                    style91.VerticalAlignment = VerticalAlignment.Center;
                    style91.BorderLeft = BorderStyle.Thin;
                    style91.BorderTop = BorderStyle.Dotted;

                    IFont font10 = hssfworkbook.CreateFont();
                    font10.Color = IndexedColors.Black.Index;
                    font10.IsBold = true; ;
                    font10.FontHeightInPoints = 14;
                    style10.SetFont(font10);
                    style10.Alignment = HorizontalAlignment.Center;
                    style10.VerticalAlignment = VerticalAlignment.Center;
                    style10.BorderLeft = BorderStyle.Medium;
                    style10.BorderTop = BorderStyle.Thin;

                    IFont font111 = hssfworkbook.CreateFont();
                    font111.Color = IndexedColors.Black.Index;
                    font111.IsBold = false; ;
                    font111.FontHeightInPoints = 16;
                    //font.FontName = "宋体";
                    style111.SetFont(font111);
                    style111.Alignment = HorizontalAlignment.Left;
                    style111.VerticalAlignment = VerticalAlignment.Center;
                    //style111.BorderLeft = BorderStyle.Thin;
                    style111.BorderRight = BorderStyle.Medium;
                    style111.BorderTop = BorderStyle.Thin;
                    //style111.BorderDiagonal = BorderDiagonal.Both;

                    IFont font112 = hssfworkbook.CreateFont();
                    font112.Color = IndexedColors.Black.Index;
                    font112.IsBold = true; ;
                    font112.FontHeightInPoints = 14;
                    //font.FontName = "宋体";
                    style112.SetFont(font112);
                    style112.Alignment = HorizontalAlignment.Center;
                    style112.VerticalAlignment = VerticalAlignment.Center;
                    style112.BorderLeft = BorderStyle.Thin;
                    //style112.BorderRight = BorderStyle.Thin;
                    style112.BorderTop = BorderStyle.Thin;

                    //IFont font11 = hssfworkbook.CreateFont();
                    //font11.Color = IndexedColors.Black.Index;
                    //font11.IsBold = false; ;
                    //font11.FontHeightInPoints = 16;
                    //font.FontName = "宋体";
                    style11.BorderTop = BorderStyle.Dotted;
                    style11.BorderLeft = BorderStyle.Thin;
                    style11.BorderRight = BorderStyle.Medium;
                    //style11.BorderBottom = BorderStyle.Thin;
                   /* style11.BorderDiagonalLineStyle = BorderStyle.Thin;
                    style11.BorderDiagonal = BorderDiagonal.Both;
                    style11.BorderDiagonalColor = IndexedColors.Black.Index;*/
                    //style11.SetFont(font11);
                    //style11.Alignment = HorizontalAlignment.Left;
                    //style11.VerticalAlignment = VerticalAlignment.Center;
                    //style11.BorderLeft = BorderStyle.Thin;
                    //style11.BorderRight = BorderStyle.Medium;
                    //style11.BorderTop = BorderStyle.Dotted;
                    //style11.BorderDiagonal = BorderDiagonal.Both;
                    //style6.BorderBottom = BorderStyle.Medium;

                    IFont font12 = hssfworkbook.CreateFont();
                    font12.Color = IndexedColors.Black.Index;
                    font12.IsBold = false; ;
                    font12.FontHeightInPoints = 18;
                    style12.SetFont(font12);
                    style12.Alignment = HorizontalAlignment.Center;
                    style12.VerticalAlignment = VerticalAlignment.Center;
                    style12.BorderLeft = BorderStyle.Thin;
                    style12.BorderRight = BorderStyle.Medium;
                    style12.BorderTop = BorderStyle.Thin;

                    IFont font13 = hssfworkbook.CreateFont();
                    font13.Color = IndexedColors.Black.Index;
                    font13.IsBold = false; ;
                    font13.FontHeightInPoints = 14;
                    //font.FontName = "宋体";
                    //style13.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;//灰色
                    //style13.FillPattern = FillPattern.SolidForeground;
                    style13.SetFont(font13);
                    style13.Alignment = HorizontalAlignment.Center;
                    style13.VerticalAlignment = VerticalAlignment.Center;
                    style13.BorderLeft = BorderStyle.Medium;
                    style13.BorderBottom = BorderStyle.Medium;
                    style13.BorderTop = BorderStyle.Dotted;

                    style14.Alignment = HorizontalAlignment.Center;
                    style14.VerticalAlignment = VerticalAlignment.Center;
                    style14.BorderLeft = BorderStyle.Thin;
                    style14.BorderBottom = BorderStyle.Medium;
                    //style14.BorderDiagonal = BorderDiagonal.Both;
                    style14.BorderTop = BorderStyle.Dotted;

                   
                    style15.SetFont(font15);
                    style15.Alignment = HorizontalAlignment.Center;
                    style15.VerticalAlignment = VerticalAlignment.Center;
                    style15.BorderLeft = BorderStyle.Thin;
                    style15.BorderBottom = BorderStyle.Medium;
                    style15.BorderTop = BorderStyle.Dotted;

                    style16.SetFont(font15);
                    style16.Alignment = HorizontalAlignment.Center;
                    style16.VerticalAlignment = VerticalAlignment.Center;
                    style16.BorderLeft = BorderStyle.Thin;
                    //style16.BorderRight = BorderStyle.Medium;
                    style16.BorderBottom = BorderStyle.Medium;
                    style16.BorderTop = BorderStyle.Dotted;

                    //style13.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;//灰色
                    //style13.FillPattern = FillPattern.SolidForeground;

                    style17.BorderTop = BorderStyle.Medium;
                    style18.BorderTop = BorderStyle.Thin;
                    style19.BorderTop = BorderStyle.Dotted;
                    style22.BorderTop = BorderStyle.Dotted;
                    style22.BorderBottom = BorderStyle.Medium;


                    IFont font20 = hssfworkbook.CreateFont();
                    font20.Color = IndexedColors.Black.Index;
                    font20.IsBold = false; ;
                    font20.FontHeightInPoints = 16;
                    //font.FontName = "宋体";
                    //style13.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;//灰色
                    //style13.FillPattern = FillPattern.SolidForeground;
                    style20.SetFont(font20);
                    style20.Alignment = HorizontalAlignment.Center;
                    style20.VerticalAlignment = VerticalAlignment.Center;
                    //style20.BorderLeft = BorderStyle.Thin;
                    style20.BorderRight = BorderStyle.Medium;
                    style20.BorderBottom = BorderStyle.Medium;
                    style20.BorderTop = BorderStyle.Dotted;

                    style21.BorderRight = BorderStyle.Thin;
                    style21.BorderLeft = BorderStyle.Medium;

                    IFont font24 = hssfworkbook.CreateFont();
                    font24.Color = IndexedColors.Black.Index;
                    font24.IsBold = false; ;
                    font24.FontHeightInPoints = 11;
                    //font.FontName = "宋体";
                    style24.SetFont(font24);
                    style24.Alignment = HorizontalAlignment.Left;
                    style24.VerticalAlignment = VerticalAlignment.Center;

                    IFont font25 = hssfworkbook.CreateFont();
                    font25.Color = IndexedColors.Black.Index;
                    font25.IsBold = false; ;
                    font25.FontHeightInPoints = 11;
                    //font.FontName = "宋体";
                    style25.SetFont(font25);
                    style25.Alignment = HorizontalAlignment.Right;
                    style25.VerticalAlignment = VerticalAlignment.Center;

                    style26.BorderBottom = BorderStyle.MediumDashed;


                    IFont font27 = hssfworkbook.CreateFont();
                    font27.Color = IndexedColors.Black.Index;
                    font27.IsBold = true; ;
                    font27.FontHeightInPoints = 16;
                    //font.FontName = "宋体";
                    style27.SetFont(font27);
                    style27.Alignment = HorizontalAlignment.Center;
                    style27.VerticalAlignment = VerticalAlignment.Center;
                    style27.BorderLeft = BorderStyle.Thin;
                    //style2.BorderRight = BorderStyle.Thin;
                    style27.BorderTop = BorderStyle.Dotted;

                    style28.BorderRight = BorderStyle.Medium;
                    style28.BorderTop = BorderStyle.Dotted;

                    IFont font29 = hssfworkbook.CreateFont();
                    font29.Color = IndexedColors.Black.Index;
                    font29.IsBold = true; ;
                    font29.FontHeightInPoints = 14;
                    //font.FontName = "宋体";
                    style29.SetFont(font29);
                    style29.Alignment = HorizontalAlignment.Center;
                    style29.VerticalAlignment = VerticalAlignment.Center;
                    style29.BorderLeft = BorderStyle.Thin;
                    style29.BorderTop = BorderStyle.Thin;
                    #endregion
                    #endregion
                    #region 设置列的宽度
                    //mysheetHSSF.SetColumnWidth(0, 17 * 256); //设置第1列的列宽为17个字符
                    //mysheetHSSF.SetColumnWidth(1, 5 * 256); //设置第2列的列宽为31个字符
                    //mysheetHSSF.SetColumnWidth(2, 10 * 256); //设置第3列的列宽为10个字符
                    //mysheetHSSF.SetColumnWidth(3, 10 * 256); //设置第4列的列宽为10个字符
                    //mysheetHSSF.SetColumnWidth(4, 10 * 256); //设置第5列的列宽为10个字符
                    //mysheetHSSF.SetColumnWidth(5, 10 * 256); //设置第6列的列宽为10个字符
                    //mysheetHSSF.SetColumnWidth(6, 10 * 256); //设置第7列的列宽为10个字符
                    //mysheetHSSF.SetColumnWidth(7, 10 * 256); //设置第8列的列宽为10个字符
                    //mysheetHSSF.SetColumnWidth(8, 10 * 256); //设置第9列的列宽为10个字符
                    //mysheetHSSF.SetColumnWidth(9, 10 * 256); //设置第10列的列宽为10个字符
                    //mysheetHSSF.SetColumnWidth(10, 10 * 256); //设置第11列的列宽为10个字符
                    //mysheetHSSF.SetColumnWidth(11, 10 * 256); //设置第12列的列宽为10个字符
                    //mysheetHSSF.SetColumnWidth(12, 10 * 256); //设置第13列的列宽为10个字符
                    //mysheetHSSF.SetColumnWidth(13, 17 * 256); //设置第14列的列宽为17个字符
                    #endregion
                    #endregion
                    //填写数据
                    int nextRow = 0;
                    for (int irow = 0; irow < dtNewSupplierandWorkArea.Rows.Count; irow++)
                    {
                        int haoShiNum = int.Parse(dtNewSupplierandWorkArea.Rows[i]["vcNumber"].ToString());
                        for (int k = 1; k <= haoShiNum; k++)
                        {
                            #region 模板第一行
                            mysheetHSSF.AddMergedRegion(new CellRangeAddress(nextRow, nextRow, 2, 5));
                            mysheetHSSF.AddMergedRegion(new CellRangeAddress(nextRow, nextRow, 7, 8));
                            IRow nextRowHSSFCol = mysheetHSSF.CreateRow(nextRow); //设置第0行
                            nextRowHSSFCol.Height = 25 * 20;
                            nextRowHSSFCol.CreateCell(1).SetCellValue("供应商番号");
                            nextRowHSSFCol.GetCell(1).CellStyle = style1;

                            nextRowHSSFCol.CreateCell(2).SetCellValue(dtNewSupplierandWorkArea.Rows[irow]["vcSupplier_id"].ToString());
                            nextRowHSSFCol.GetCell(2).CellStyle = style2;
                            nextRowHSSFCol.CreateCell(3).SetCellValue("");
                            nextRowHSSFCol.GetCell(3).CellStyle = style17;
                            nextRowHSSFCol.CreateCell(4).SetCellValue("");
                            nextRowHSSFCol.GetCell(4).CellStyle = style17;
                            nextRowHSSFCol.CreateCell(5).SetCellValue("");
                            nextRowHSSFCol.GetCell(5).CellStyle = style17;

                            nextRowHSSFCol.CreateCell(6).SetCellValue("所番地");
                            nextRowHSSFCol.GetCell(6).CellStyle = style3;
                            nextRowHSSFCol.CreateCell(7).SetCellValue("使用目的");
                            nextRowHSSFCol.GetCell(7).CellStyle = style3;
                            nextRowHSSFCol.CreateCell(8).SetCellValue("");
                            nextRowHSSFCol.GetCell(8).CellStyle = style4;
                            nextRow++;
                            #endregion
                            #region 模板第2行
                            mysheetHSSF.AddMergedRegion(new CellRangeAddress(nextRow, nextRow, 2, 5));
                            IRow next2RowHSSFCol = mysheetHSSF.CreateRow(nextRow); //设置第0行
                            next2RowHSSFCol.Height = 34 * 20;
                            next2RowHSSFCol.CreateCell(1).SetCellValue("供应商名称");
                            next2RowHSSFCol.GetCell(1).CellStyle = style5;

                            next2RowHSSFCol.CreateCell(2).SetCellValue(dtNewSupplierandWorkArea.Rows[irow]["vcSupplier_name"].ToString());
                            next2RowHSSFCol.GetCell(2).CellStyle = style6;
                            next2RowHSSFCol.CreateCell(3).SetCellValue("");
                            next2RowHSSFCol.GetCell(3).CellStyle = style18;
                            next2RowHSSFCol.CreateCell(4).SetCellValue("");
                            next2RowHSSFCol.GetCell(4).CellStyle = style18;
                            next2RowHSSFCol.CreateCell(5).SetCellValue("");
                            next2RowHSSFCol.GetCell(5).CellStyle = style18;

                            next2RowHSSFCol.CreateCell(6).SetCellValue("");
                            next2RowHSSFCol.GetCell(6).CellStyle = style7;
                            mysheetHSSF.AddMergedRegion(new CellRangeAddress(nextRow, nextRow, 7, 8));
                            next2RowHSSFCol.CreateCell(7).SetCellValue(dtNewSupplierandWorkArea.Rows[irow]["vcCarType"].ToString() + "补给号试");
                            next2RowHSSFCol.GetCell(7).CellStyle = style27;
                            //next2RowHSSFCol.CreateCell(7).SetCellValue(dtNewSupplierandWorkArea.Rows[irow]["vcCarType"].ToString());
                            //next2RowHSSFCol.GetCell(7).CellStyle = style27;
                            next2RowHSSFCol.CreateCell(8).SetCellValue("");
                            next2RowHSSFCol.GetCell(8).CellStyle = style9;
                            nextRow++;
                            #endregion
                            #region 模板第3 4行
                            mysheetHSSF.AddMergedRegion(new CellRangeAddress(nextRow, nextRow + 1, 1, 1));
                            mysheetHSSF.AddMergedRegion(new CellRangeAddress(nextRow, nextRow + 1, 2, 6));
                            mysheetHSSF.AddMergedRegion(new CellRangeAddress(nextRow, nextRow, 7, 8));
                            IRow next3RowHSSFCol = mysheetHSSF.CreateRow(nextRow); //设置第0行
                            next3RowHSSFCol.Height = (short)(17.5 * 20);
                            next3RowHSSFCol.CreateCell(1).SetCellValue("品番");
                            next3RowHSSFCol.GetCell(1).CellStyle = style10;

                            next3RowHSSFCol.CreateCell(2).SetCellValue(dtNewSupplierandWorkArea.Rows[irow]["vcPartNo"].ToString());
                            next3RowHSSFCol.GetCell(2).CellStyle = style6PartNo;
                            next3RowHSSFCol.CreateCell(3).SetCellValue("");
                            next3RowHSSFCol.GetCell(3).CellStyle = style18;
                            next3RowHSSFCol.CreateCell(4).SetCellValue("");
                            next3RowHSSFCol.GetCell(4).CellStyle = style18;
                            next3RowHSSFCol.CreateCell(5).SetCellValue("");
                            next3RowHSSFCol.GetCell(5).CellStyle = style18;
                            next3RowHSSFCol.CreateCell(6).SetCellValue("");
                            next3RowHSSFCol.GetCell(6).CellStyle = style7;

                            next3RowHSSFCol.CreateCell(7).SetCellValue("色番");
                            next3RowHSSFCol.GetCell(7).CellStyle = style112;
                            next3RowHSSFCol.CreateCell(8).SetCellValue("");
                            next3RowHSSFCol.GetCell(8).CellStyle = style111;
                            nextRow++;
                            mysheetHSSF.AddMergedRegion(new CellRangeAddress(nextRow, nextRow, 7, 8));
                            IRow next4RowHSSFCol = mysheetHSSF.CreateRow(nextRow); //设置第0行
                            next4RowHSSFCol.Height = (short)(35.5 * 20);
                            next4RowHSSFCol.CreateCell(1).SetCellValue("");
                            next4RowHSSFCol.GetCell(1).CellStyle = style21;
                            next4RowHSSFCol.CreateCell(2).SetCellValue("");
                            next4RowHSSFCol.CreateCell(3).SetCellValue("");
                            next4RowHSSFCol.CreateCell(4).SetCellValue("");
                            next4RowHSSFCol.CreateCell(5).SetCellValue("");
                            next4RowHSSFCol.CreateCell(6).SetCellValue("");
                            next4RowHSSFCol.CreateCell(7).SetCellValue("");
                            next4RowHSSFCol.GetCell(7).CellStyle = style8;
                            next4RowHSSFCol.CreateCell(8).SetCellValue("");
                            next4RowHSSFCol.GetCell(8).CellStyle = style28;
                            nextRow++;
                            #endregion
                            #region 模板第5行
                            mysheetHSSF.AddMergedRegion(new CellRangeAddress(nextRow, nextRow, 2, 6));
                            IRow next5RowHSSFCol = mysheetHSSF.CreateRow(nextRow); //设置第0行
                            next5RowHSSFCol.Height = 50 * 20;
                            next5RowHSSFCol.CreateCell(1).SetCellValue("品名");
                            next5RowHSSFCol.GetCell(1).CellStyle = style5;

                            next5RowHSSFCol.CreateCell(2).SetCellValue(dtNewSupplierandWorkArea.Rows[irow]["vcPartName"].ToString());
                            next5RowHSSFCol.GetCell(2).CellStyle = style91;
                            next5RowHSSFCol.CreateCell(3).SetCellValue("");
                            next5RowHSSFCol.GetCell(3).CellStyle = style19;
                            next5RowHSSFCol.CreateCell(4).SetCellValue("");
                            next5RowHSSFCol.GetCell(4).CellStyle = style19;
                            next5RowHSSFCol.CreateCell(5).SetCellValue("");
                            next5RowHSSFCol.GetCell(5).CellStyle = style19;
                            next5RowHSSFCol.CreateCell(6).SetCellValue("");
                            next5RowHSSFCol.GetCell(6).CellStyle = style19;

                            next5RowHSSFCol.CreateCell(7).SetCellValue("受入场");
                            next5RowHSSFCol.GetCell(7).CellStyle = style2;
                            next5RowHSSFCol.CreateCell(8).SetCellValue(dtNewSupplierandWorkArea.Rows[irow]["vcDock"].ToString());
                            next5RowHSSFCol.GetCell(8).CellStyle = style12;
                            nextRow++;
                            #endregion
                            #region 模板第6行
                            mysheetHSSF.AddMergedRegion(new CellRangeAddress(nextRow, nextRow, 1, 2));
                            mysheetHSSF.AddMergedRegion(new CellRangeAddress(nextRow, nextRow, 5, 6));
                            mysheetHSSF.AddMergedRegion(new CellRangeAddress(nextRow, nextRow, 7, 8));
                            IRow next6RowHSSFCol = mysheetHSSF.CreateRow(nextRow); //设置第0行
                            next6RowHSSFCol.Height = (short)(22.5 * 20);
                            next6RowHSSFCol.CreateCell(1).SetCellValue("交货日期提示");
                            next6RowHSSFCol.GetCell(1).CellStyle = style10;

                            next6RowHSSFCol.CreateCell(2).SetCellValue("");
                            next6RowHSSFCol.GetCell(2).CellStyle = style18;
                            next6RowHSSFCol.CreateCell(3).SetCellValue("背番号");
                            next6RowHSSFCol.GetCell(3).CellStyle = style112;
                            next6RowHSSFCol.CreateCell(4).SetCellValue("收容数");
                            next6RowHSSFCol.GetCell(4).CellStyle = style112;
                            next6RowHSSFCol.CreateCell(5).SetCellValue("本箱应装个数");
                            next6RowHSSFCol.GetCell(5).CellStyle = style112;
                            next6RowHSSFCol.CreateCell(6).SetCellValue("");
                            next6RowHSSFCol.GetCell(6).CellStyle = style18;

                            next6RowHSSFCol.CreateCell(7).SetCellValue("连番");
                            next6RowHSSFCol.GetCell(7).CellStyle = style6;
                            next6RowHSSFCol.CreateCell(8).SetCellValue("");
                            next6RowHSSFCol.GetCell(8).CellStyle = style111;
                            nextRow++;
                            #endregion
                            #region 模板第7行
                            mysheetHSSF.AddMergedRegion(new CellRangeAddress(nextRow, nextRow, 1, 2));
                            mysheetHSSF.AddMergedRegion(new CellRangeAddress(nextRow, nextRow, 5, 6));
                            mysheetHSSF.AddMergedRegion(new CellRangeAddress(nextRow, nextRow, 7, 8));
                            IRow next7RowHSSFCol = mysheetHSSF.CreateRow(nextRow); //设置第0行
                            next7RowHSSFCol.Height = (short)(45.5 * 20);
                            next7RowHSSFCol.CreateCell(1).SetCellValue(dtNewSupplierandWorkArea.Rows[irow]["dOrderReceiveDate"].ToString());
                            next7RowHSSFCol.GetCell(1).CellStyle = style13;
                            next7RowHSSFCol.CreateCell(2).SetCellValue("");
                            next7RowHSSFCol.GetCell(2).CellStyle = style22;

                            next7RowHSSFCol.CreateCell(3).SetCellValue("");
                            next7RowHSSFCol.GetCell(3).CellStyle = style71;
                            next7RowHSSFCol.CreateCell(4).SetCellValue("1");
                            next7RowHSSFCol.GetCell(4).CellStyle = style15;
                            next7RowHSSFCol.CreateCell(5).SetCellValue("1");//dtNewSupplierandWorkArea.Rows[irow]["vcNumber"].ToString()
                            next7RowHSSFCol.GetCell(5).CellStyle = style15;
                            next7RowHSSFCol.CreateCell(6).SetCellValue("");
                            next7RowHSSFCol.GetCell(6).CellStyle = style22;

                            next7RowHSSFCol.CreateCell(7).SetCellValue(k);
                            next7RowHSSFCol.GetCell(7).CellStyle = style16;
                            next7RowHSSFCol.CreateCell(8).SetCellValue("");
                            next7RowHSSFCol.GetCell(8).CellStyle = style20;
                            nextRow++;
                            #endregion
                            #region 模板第8行
                            mysheetHSSF.AddMergedRegion(new CellRangeAddress(nextRow, nextRow, 1, 4));
                            mysheetHSSF.AddMergedRegion(new CellRangeAddress(nextRow, nextRow, 6, 8));
                            IRow next8RowHSSFCol = mysheetHSSF.CreateRow(nextRow); //设置第0行
                            next8RowHSSFCol.Height = 39 * 20;
                            next8RowHSSFCol.CreateCell(1).SetCellValue("注：每一箱都要贴此标签,且装箱内容要和此标签一致");
                            next8RowHSSFCol.GetCell(1).CellStyle = style24;
                            next8RowHSSFCol.CreateCell(2).SetCellValue("");
                            next8RowHSSFCol.CreateCell(3).SetCellValue("");
                            next8RowHSSFCol.CreateCell(4).SetCellValue("");
                            next8RowHSSFCol.CreateCell(5).SetCellValue("");
                            next8RowHSSFCol.CreateCell(6).SetCellValue("");
                            //next8RowHSSFCol.GetCell(6).CellStyle = style25;
                            next8RowHSSFCol.CreateCell(7).SetCellValue("");
                            next8RowHSSFCol.CreateCell(8).SetCellValue("");
                            nextRow++;
                            #endregion
                            #region 分割线
                            mysheetHSSF.AddMergedRegion(new CellRangeAddress(nextRow, nextRow, 1, 8));
                            IRow next9RowHSSFCol = mysheetHSSF.CreateRow(nextRow); //设置第0行
                            next8RowHSSFCol.Height = 40 * 20;
                            next9RowHSSFCol.CreateCell(1).SetCellValue("");
                            next9RowHSSFCol.GetCell(1).CellStyle = style26;
                            next9RowHSSFCol.CreateCell(2).SetCellValue("");
                            next9RowHSSFCol.GetCell(2).CellStyle = style26;
                            next9RowHSSFCol.CreateCell(3).SetCellValue("");
                            next9RowHSSFCol.GetCell(3).CellStyle = style26;
                            next9RowHSSFCol.CreateCell(4).SetCellValue("");
                            next9RowHSSFCol.GetCell(4).CellStyle = style26;
                            next9RowHSSFCol.CreateCell(5).SetCellValue("");
                            next9RowHSSFCol.GetCell(5).CellStyle = style26;
                            next9RowHSSFCol.CreateCell(6).SetCellValue("");
                            next9RowHSSFCol.GetCell(6).CellStyle = style26;
                            next9RowHSSFCol.CreateCell(7).SetCellValue("");
                            next9RowHSSFCol.GetCell(7).CellStyle = style26;
                            next9RowHSSFCol.CreateCell(8).SetCellValue("");
                            next9RowHSSFCol.GetCell(8).CellStyle = style26;
                            nextRow++;
                            nextRow++;
                            #endregion

                        }
                    }
                    //CellRangeAddress c = CellRangeAddress.ValueOf("A1:I9");

                    //mysheetHSSF.SetAutoFilter(c);
                    //设置打印区域

                    //hssfworkbook.SetPrintArea(sheetnum, int first row, int last row, int first col, int last col);

                    int firstRow = 0;
                    int lastRow = 0;
                    for (int l=0;l< dtNewSupplierandWorkArea.Rows.Count;l++)
                    {
                        lastRow += int.Parse(dtNewSupplierandWorkArea.Rows[i]["vcNumber"].ToString())*10;
                    }
                    //int lastRow = dtNewSupplierandWorkArea.Rows.Count * 10;
                    hssfworkbook.SetPrintArea(0, 0, 8, firstRow, lastRow);
                    string rootPath = _webHostEnvironment.ContentRootPath;
                    string strFunctionName = "FS0625_号试看板标签_" + vcSupplier_id;

                    string strFileName = strFunctionName + "_导出信息_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + loginInfo.UserId + ".xlsx";
                    string fileSavePath = rootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Export" + Path.DirectorySeparatorChar;//文件临时目录，导入完成后 删除
                    string filepathHaoShiKanBan = fileSavePath + strFileName;

                    using (FileStream fs = System.IO.File.OpenWrite(filepathHaoShiKanBan))
                    {
                        hssfworkbook.Write(fs);//向打开的这个xls文件中写入数据  
                        fs.Close();
                    }
                    if (strFileName != "")
                    {
                        strFilePathArray[0] = filepathHaoShiKanBan;
                    }
                    else
                    {
                        strErr.Append(logs);
                    }
                    #endregion
                    //Console.WriteLine(vcSupplier_id + "第一个附件结束");
                    //Console.WriteLine(vcSupplier_id + "第二个附件开始");
                    #region  第二个附件
                    string[] columnArray2 = { "vcCarType", "vcDock" };
                    DataView dtSelectView2 = dtNewSupplierandWorkArea.DefaultView;
                    DataTable dtSelect2 = dtSelectView2.ToTable(true, columnArray2);//去重后的dt 

                    XSSFWorkbook hshdworkbook = new XSSFWorkbook();//用于创建xlsx 号试货剁
                    string XltPath4hshd = "." + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Template" + Path.DirectorySeparatorChar + "FS0625_HSHuoDuoBiaoQian.xlsx";
                    using (FileStream fs1 = System.IO.File.OpenRead(XltPath4hshd))
                    {
                        hshdworkbook = new XSSFWorkbook(fs1);
                        fs1.Close();
                    }
                    ISheet mysheetHSHD = hshdworkbook.GetSheetAt(0);//获得sheet

                    if (dtSelect2.Rows.Count > 1)
                    {
                        mysheetHSHD.ShiftRows(1, 3, dtSelect2.Rows.Count * 17, true, false);
                    }

                    #region 第二个附件的样式
                    Dictionary<string, object> res = new Dictionary<string, object>();
                    #region 设置字体样式
                    ICellStyle hdstyle1 = hshdworkbook.CreateCellStyle();//声明style1对象，设置Excel表格的样式 标题 车型 左边
                    IFont hdfont1 = hshdworkbook.CreateFont();
                    hdfont1.Color = IndexedColors.Black.Index;
                    hdfont1.IsBold = true; ;
                    hdfont1.FontHeightInPoints = 90;
                    hdstyle1.SetFont(hdfont1);
                    hdstyle1.Alignment = HorizontalAlignment.Center;
                    hdstyle1.VerticalAlignment = VerticalAlignment.Center;
                    //hdstyle1.BorderLeft = BorderStyle.Medium;
                    //hdstyle1.BorderTop = BorderStyle.Medium;

                    ICellStyle hdstyle2 = hshdworkbook.CreateCellStyle();//9号字体加粗 没有背景色 车型上边
                    //hdstyle2.BorderTop = BorderStyle.Medium;

                    ICellStyle hdstyle3 = hshdworkbook.CreateCellStyle();//9号字体不加粗 号试补给品右边
                    //hdstyle3.BorderTop = BorderStyle.Medium;
                    //hdstyle3.BorderRight = BorderStyle.Medium;

                    ICellStyle hdstyle4 = hshdworkbook.CreateCellStyle();//9号字体不加粗 号试补给品
                    IFont hdfont4 = hshdworkbook.CreateFont();
                    hdfont4.Color = IndexedColors.Black.Index;
                    hdfont4.IsBold = false; ;
                    hdfont4.FontHeightInPoints = 90;
                    hdstyle4.SetFont(hdfont4);
                    hdstyle4.Alignment = HorizontalAlignment.Center;
                    hdstyle4.VerticalAlignment = VerticalAlignment.Center;
                    //hdstyle4.BorderLeft = BorderStyle.Medium;

                    //ICellStyle hdstyle5 = hshdworkbook.CreateCellStyle();//9号字体不加粗 号试补给品右边
                    //hdstyle5.BorderRight = BorderStyle.Medium;

                    ICellStyle hdstyle6 = hshdworkbook.CreateCellStyle();//9号字体不加粗 号试补给品右边
                    //hdstyle6.BorderRight = BorderStyle.Medium;

                    ICellStyle hdstyle7 = hshdworkbook.CreateCellStyle();//9号字体不加粗 号试补给品右边
                    //hdstyle7.BorderLeft = BorderStyle.Medium;

                    ICellStyle hdstyle8 = hshdworkbook.CreateCellStyle();//9号字体不加粗 号试补给品右边
                    IFont hdfont8 = hshdworkbook.CreateFont();
                    hdfont8.Color = IndexedColors.Black.Index;
                    hdfont8.IsBold = true; ;
                    hdfont8.FontHeightInPoints = 120;
                    hdstyle8.SetFont(hdfont8);
                    hdstyle8.Alignment = HorizontalAlignment.Center;
                    hdstyle8.VerticalAlignment = VerticalAlignment.Center;
                    //hdstyle8.BorderLeft = BorderStyle.Medium;

                    ICellStyle hdstyle9 = hshdworkbook.CreateCellStyle();//9号字体不加粗 号试补给品右边
                    IFont hdfont9 = hshdworkbook.CreateFont();
                    hdfont9.Color = IndexedColors.Black.Index;
                    hdfont9.IsBold = false; ;
                    hdfont9.FontHeightInPoints = 18;
                    hdstyle9.SetFont(hdfont9);
                    hdstyle9.VerticalAlignment = VerticalAlignment.Center;
                    hdstyle9.Alignment = HorizontalAlignment.Center;
                    //hdstyle9.BorderLeft = BorderStyle.Medium;

                    ICellStyle hdstyle10 = hshdworkbook.CreateCellStyle();//9号字体不加粗 号试补给品右边
                    IFont hdfont10 = hshdworkbook.CreateFont();
                    hdfont10.Color = IndexedColors.Black.Index;
                    hdfont10.IsBold = false; ;
                    hdfont10.FontHeightInPoints = 12;
                    hdstyle10.SetFont(hdfont10);
                    hdstyle10.VerticalAlignment = VerticalAlignment.Center;
                    hdstyle10.Alignment = HorizontalAlignment.Center;
                    //hdstyle10.BorderLeft = BorderStyle.Medium;

                    ICellStyle hdstyle11 = hshdworkbook.CreateCellStyle();//9号字体不加粗 号试补给品右边
                    //hdstyle11.BorderBottom = BorderStyle.Medium;
                    //hdstyle11.BorderLeft = BorderStyle.Medium;

                    ICellStyle hdstyle12 = hshdworkbook.CreateCellStyle();//9号字体不加粗 号试补给品右边
                    //hdstyle12.BorderBottom = BorderStyle.Medium;

                    ICellStyle hdstyle13 = hshdworkbook.CreateCellStyle();//9号字体不加粗 号试补给品右边
                    //hdstyle13.BorderBottom = BorderStyle.Medium;
                    //hdstyle13.BorderRight = BorderStyle.Medium;

                    //ICellStyle hdstyle14 = hshdworkbook.CreateCellStyle();//9号字体不加粗 号试补给品右边
                    //hdstyle14.BorderLeft = BorderStyle.Medium;

                    //ICellStyle hdstyle15 = hshdworkbook.CreateCellStyle();//9号字体不加粗 号试补给品右边
                    //hdstyle15.BorderRight = BorderStyle.Medium;
                    #endregion
                    int nextRow2 = 0;
                    DataTable dsHHD = fs0625_Logic.getHSHD("C057");
                    string vcShowStr1 = string.Empty;
                    string vcShowStr2 = string.Empty;
                    if (dsHHD.Rows.Count > 0)
                    {
                        vcShowStr1 = dsHHD.Rows[0]["vcName"].ToString();
                        vcShowStr2 = dsHHD.Rows[1]["vcName"].ToString();
                    }

                    for (int irow = 0; irow < dtSelect2.Rows.Count; irow++)
                    {
                        string vcCarType = dtSelect2.Rows[irow]["vcCarType"].ToString();
                        string vcDock = dtSelect2.Rows[irow]["vcDock"].ToString();
                        #region 模板第一行
                        IRow nextRowHSSFCol2 = mysheetHSHD.CreateRow(nextRow2); //设置第0行
                        nextRowHSSFCol2.Height = 145 * 20;
                        nextRowHSSFCol2.CreateCell(0).SetCellValue(vcCarType.ToString());


                        mysheetHSHD.AddMergedRegion(new CellRangeAddress(nextRow2, nextRow2, 0, 8));
                        nextRowHSSFCol2.GetCell(0).CellStyle = hdstyle1;
                        nextRowHSSFCol2.CreateCell(1).SetCellValue("");

                        nextRowHSSFCol2.GetCell(1).CellStyle = hdstyle2;
                        nextRowHSSFCol2.CreateCell(2).SetCellValue("");
                        nextRowHSSFCol2.GetCell(2).CellStyle = hdstyle2;
                        nextRowHSSFCol2.CreateCell(3).SetCellValue("");
                        nextRowHSSFCol2.GetCell(3).CellStyle = hdstyle2;
                        nextRowHSSFCol2.CreateCell(4).SetCellValue("");
                        nextRowHSSFCol2.GetCell(4).CellStyle = hdstyle2;
                        nextRowHSSFCol2.CreateCell(5).SetCellValue("");
                        nextRowHSSFCol2.GetCell(5).CellStyle = hdstyle2;
                        nextRowHSSFCol2.CreateCell(6).SetCellValue("");
                        nextRowHSSFCol2.GetCell(6).CellStyle = hdstyle2;
                        nextRowHSSFCol2.CreateCell(7).SetCellValue("");
                        nextRowHSSFCol2.GetCell(7).CellStyle = hdstyle2;
                        nextRowHSSFCol2.CreateCell(8).SetCellValue("");

                        nextRowHSSFCol2.GetCell(8).CellStyle = hdstyle3;
                        nextRow2++;
                        #endregion
                        #region 2
                        IRow nextRowHSSFCol2s = mysheetHSHD.CreateRow(nextRow2); //设置第0行
                        mysheetHSHD.AddMergedRegion(new CellRangeAddress(nextRow2, nextRow2, 0, 8));
                        nextRowHSSFCol2s.Height = 85 * 20;
                        ICell cell2 = nextRowHSSFCol2s.CreateCell(0);

                        cell2.SetCellValue("号试");

                        nextRowHSSFCol2s.GetCell(0).CellStyle = hdstyle4;
                        nextRowHSSFCol2s.CreateCell(1).SetCellValue("");
                        nextRowHSSFCol2s.CreateCell(2).SetCellValue("");
                        nextRowHSSFCol2s.CreateCell(3).SetCellValue("");
                        nextRowHSSFCol2s.CreateCell(4).SetCellValue("");
                        nextRowHSSFCol2s.CreateCell(5).SetCellValue("");
                        nextRowHSSFCol2s.CreateCell(6).SetCellValue("");
                        nextRowHSSFCol2s.CreateCell(7).SetCellValue("");
                        nextRowHSSFCol2s.CreateCell(8).SetCellValue("");

                        nextRowHSSFCol2s.GetCell(8).CellStyle = hdstyle6;
                        nextRow2++;

                        IRow nextRowHSSFCol2b = mysheetHSHD.CreateRow(nextRow2); //设置第0行
                        mysheetHSHD.AddMergedRegion(new CellRangeAddress(nextRow2, nextRow2, 0, 8));
                        nextRowHSSFCol2b.Height = 109 * 20;
                        nextRowHSSFCol2b.CreateCell(0).SetCellValue("补给品");
                        nextRowHSSFCol2b.GetCell(0).CellStyle = hdstyle4;
                        nextRowHSSFCol2b.CreateCell(1).SetCellValue("");
                        nextRowHSSFCol2b.CreateCell(2).SetCellValue("");
                        nextRowHSSFCol2b.CreateCell(3).SetCellValue("");
                        nextRowHSSFCol2b.CreateCell(4).SetCellValue("");
                        nextRowHSSFCol2b.CreateCell(5).SetCellValue("");
                        nextRowHSSFCol2b.CreateCell(6).SetCellValue("");
                        nextRowHSSFCol2b.CreateCell(7).SetCellValue("");
                        nextRowHSSFCol2b.CreateCell(8).SetCellValue("");

                        nextRowHSSFCol2b.GetCell(8).CellStyle = hdstyle6;
                        nextRow2++;
                        #endregion
                        #region 第三行

                        IRow nextRowHSSFCol2_3 = mysheetHSHD.CreateRow(nextRow2); //设置第0行
                        mysheetHSHD.AddMergedRegion(new CellRangeAddress(nextRow2, nextRow2, 0, 8));
                        nextRowHSSFCol2_3.Height = 56 * 20;
                        nextRowHSSFCol2_3.CreateCell(0).SetCellValue("");

                        nextRowHSSFCol2_3.GetCell(0).CellStyle = hdstyle7;
                        nextRowHSSFCol2_3.CreateCell(1).SetCellValue("");
                        nextRowHSSFCol2_3.CreateCell(2).SetCellValue("");
                        nextRowHSSFCol2_3.CreateCell(3).SetCellValue("");
                        nextRowHSSFCol2_3.CreateCell(4).SetCellValue("");
                        nextRowHSSFCol2_3.CreateCell(5).SetCellValue("");
                        nextRowHSSFCol2_3.CreateCell(6).SetCellValue("");
                        nextRowHSSFCol2_3.CreateCell(7).SetCellValue("");
                        nextRowHSSFCol2_3.CreateCell(8).SetCellValue("");
                        nextRowHSSFCol2_3.GetCell(8).CellStyle = hdstyle6;
                        nextRow2++;
                        #endregion
                        #region 第四行
                        mysheetHSHD.AddMergedRegion(new CellRangeAddress(nextRow2, nextRow2, 0, 8));
                        IRow nextRowHSSFCol2_4 = mysheetHSHD.CreateRow(nextRow2); //设置第0行
                        nextRowHSSFCol2_4.Height = 154 * 20;
                        nextRowHSSFCol2_4.CreateCell(0).SetCellValue(vcDock);

                        nextRowHSSFCol2_4.GetCell(0).CellStyle = hdstyle8;
                        nextRowHSSFCol2_4.CreateCell(1).SetCellValue("");
                        nextRowHSSFCol2_4.CreateCell(2).SetCellValue("");
                        nextRowHSSFCol2_4.CreateCell(3).SetCellValue("");
                        nextRowHSSFCol2_4.CreateCell(4).SetCellValue("");
                        nextRowHSSFCol2_4.CreateCell(5).SetCellValue("");
                        nextRowHSSFCol2_4.CreateCell(6).SetCellValue("");
                        nextRowHSSFCol2_4.CreateCell(7).SetCellValue("");
                        nextRowHSSFCol2_4.CreateCell(8).SetCellValue("");
                        nextRowHSSFCol2_4.GetCell(8).CellStyle = hdstyle6;
                        nextRow2++;
                        #endregion
                        #region 第五行

                        IRow nextRowHSSFCol2_5 = mysheetHSHD.CreateRow(nextRow2); //设置第0行
                        mysheetHSHD.AddMergedRegion(new CellRangeAddress(nextRow2, nextRow2, 0, 8));
                        nextRowHSSFCol2_5.Height = 20 * 20;
                        nextRowHSSFCol2_5.CreateCell(0).SetCellValue(vcShowStr1);

                        nextRowHSSFCol2_5.GetCell(0).CellStyle = hdstyle9;
                        nextRowHSSFCol2_5.CreateCell(1).SetCellValue("");
                        nextRowHSSFCol2_5.CreateCell(2).SetCellValue("");
                        nextRowHSSFCol2_5.CreateCell(3).SetCellValue("");
                        nextRowHSSFCol2_5.CreateCell(4).SetCellValue("");
                        nextRowHSSFCol2_5.CreateCell(5).SetCellValue("");
                        nextRowHSSFCol2_5.CreateCell(6).SetCellValue("");
                        nextRowHSSFCol2_5.CreateCell(7).SetCellValue("");
                        nextRowHSSFCol2_5.CreateCell(8).SetCellValue("");
                        nextRowHSSFCol2_5.GetCell(8).CellStyle = hdstyle6;
                        nextRow2++;
                        #endregion
                        #region 6 
                        IRow nextRowHSSFCol2_6 = mysheetHSHD.CreateRow(nextRow2); //设置第0行
                        mysheetHSHD.AddMergedRegion(new CellRangeAddress(nextRow2, nextRow2, 0, 8));
                        nextRowHSSFCol2_6.CreateCell(0).SetCellValue("");
                        nextRowHSSFCol2_6.GetCell(0).CellStyle = hdstyle7;
                        nextRowHSSFCol2_6.CreateCell(1).SetCellValue("");
                        nextRowHSSFCol2_6.CreateCell(2).SetCellValue("");
                        nextRowHSSFCol2_6.CreateCell(3).SetCellValue("");
                        nextRowHSSFCol2_6.CreateCell(4).SetCellValue("");
                        nextRowHSSFCol2_6.CreateCell(5).SetCellValue("");
                        nextRowHSSFCol2_6.CreateCell(6).SetCellValue("");
                        nextRowHSSFCol2_6.CreateCell(7).SetCellValue("");
                        nextRowHSSFCol2_6.CreateCell(8).SetCellValue("");
                        nextRowHSSFCol2_6.GetCell(8).CellStyle = hdstyle6;
                        nextRow2++;
                        #endregion
                        #region 7 
                        IRow nextRowHSSFCol2_7 = mysheetHSHD.CreateRow(nextRow2); //设置第0行
                        mysheetHSHD.AddMergedRegion(new CellRangeAddress(nextRow2, nextRow2, 0, 8));

                        nextRowHSSFCol2_7.CreateCell(0).SetCellValue("");
                        nextRowHSSFCol2_7.GetCell(0).CellStyle = hdstyle7;
                        nextRowHSSFCol2_7.CreateCell(1).SetCellValue("");
                        nextRowHSSFCol2_7.CreateCell(2).SetCellValue("");
                        nextRowHSSFCol2_7.CreateCell(3).SetCellValue("");
                        nextRowHSSFCol2_7.CreateCell(4).SetCellValue("");
                        nextRowHSSFCol2_7.CreateCell(5).SetCellValue("");
                        nextRowHSSFCol2_7.CreateCell(6).SetCellValue("");
                        nextRowHSSFCol2_7.CreateCell(7).SetCellValue("");
                        nextRowHSSFCol2_7.CreateCell(8).SetCellValue("");
                        nextRowHSSFCol2_7.GetCell(8).CellStyle = hdstyle6;
                        nextRow2++;
                        #endregion
                        #region 8 
                        IRow nextRowHSSFCol2_8 = mysheetHSHD.CreateRow(nextRow2); //设置第0行
                        mysheetHSHD.AddMergedRegion(new CellRangeAddress(nextRow2, nextRow2, 0, 8));

                        nextRowHSSFCol2_8.CreateCell(0).SetCellValue("");
                        nextRowHSSFCol2_8.GetCell(0).CellStyle = hdstyle7;
                        nextRowHSSFCol2_8.CreateCell(1).SetCellValue("");
                        nextRowHSSFCol2_8.CreateCell(2).SetCellValue("");
                        nextRowHSSFCol2_8.CreateCell(3).SetCellValue("");
                        nextRowHSSFCol2_8.CreateCell(4).SetCellValue("");
                        nextRowHSSFCol2_8.CreateCell(5).SetCellValue("");
                        nextRowHSSFCol2_8.CreateCell(6).SetCellValue("");
                        nextRowHSSFCol2_8.CreateCell(7).SetCellValue("");
                        nextRowHSSFCol2_8.CreateCell(8).SetCellValue("");
                        nextRowHSSFCol2_8.GetCell(8).CellStyle = hdstyle6;
                        nextRow2++;
                        #endregion
                        #region 9

                        IRow nextRowHSSFCol2_9 = mysheetHSHD.CreateRow(nextRow2); //设置第0行
                        mysheetHSHD.AddMergedRegion(new CellRangeAddress(nextRow2, nextRow2, 0, 8));
                        nextRowHSSFCol2_9.Height = 25 * 20;
                        nextRowHSSFCol2_9.CreateCell(0).SetCellValue(vcShowStr2);

                        nextRowHSSFCol2_9.GetCell(0).CellStyle = hdstyle10;
                        nextRowHSSFCol2_9.CreateCell(1).SetCellValue("");
                        nextRowHSSFCol2_9.CreateCell(2).SetCellValue("");
                        nextRowHSSFCol2_9.CreateCell(3).SetCellValue("");
                        nextRowHSSFCol2_9.CreateCell(4).SetCellValue("");
                        nextRowHSSFCol2_9.CreateCell(5).SetCellValue("");
                        nextRowHSSFCol2_9.CreateCell(6).SetCellValue("");
                        nextRowHSSFCol2_9.CreateCell(7).SetCellValue("");
                        nextRowHSSFCol2_9.CreateCell(8).SetCellValue("");
                        nextRowHSSFCol2_9.GetCell(8).CellStyle = hdstyle6;
                        nextRow2++;
                        #endregion
                        #region 10
                        IRow nextRowHSSFCol2_10 = mysheetHSHD.CreateRow(nextRow2); //设置第0行
                        mysheetHSHD.AddMergedRegion(new CellRangeAddress(nextRow2, nextRow2, 0, 8));
                        nextRowHSSFCol2_10.CreateCell(0).SetCellValue("");
                        nextRowHSSFCol2_10.GetCell(0).CellStyle = hdstyle7;
                        nextRowHSSFCol2_10.CreateCell(1).SetCellValue("");
                        nextRowHSSFCol2_10.CreateCell(2).SetCellValue("");
                        nextRowHSSFCol2_10.CreateCell(3).SetCellValue("");
                        nextRowHSSFCol2_10.CreateCell(4).SetCellValue("");
                        nextRowHSSFCol2_10.CreateCell(5).SetCellValue("");
                        nextRowHSSFCol2_10.CreateCell(6).SetCellValue("");
                        nextRowHSSFCol2_10.CreateCell(7).SetCellValue("");
                        nextRowHSSFCol2_10.CreateCell(8).SetCellValue("");
                        nextRowHSSFCol2_10.GetCell(8).CellStyle = hdstyle6;
                        nextRow2++;
                        #endregion
                        #region 11
                        IRow nextRowHSSFCol2_11 = mysheetHSHD.CreateRow(nextRow2); //设置第0行
                        mysheetHSHD.AddMergedRegion(new CellRangeAddress(nextRow2, nextRow2, 0, 8));
                        nextRowHSSFCol2_11.CreateCell(0).SetCellValue(" ");

                        nextRowHSSFCol2_11.GetCell(0).CellStyle = hdstyle7;
                        nextRowHSSFCol2_11.CreateCell(1).SetCellValue("");
                        nextRowHSSFCol2_11.CreateCell(2).SetCellValue("");
                        nextRowHSSFCol2_11.CreateCell(3).SetCellValue("");
                        nextRowHSSFCol2_11.CreateCell(4).SetCellValue("");
                        nextRowHSSFCol2_11.CreateCell(5).SetCellValue("");
                        nextRowHSSFCol2_11.CreateCell(6).SetCellValue("");
                        nextRowHSSFCol2_11.CreateCell(7).SetCellValue("");
                        nextRowHSSFCol2_11.CreateCell(8).SetCellValue(" ");

                        nextRowHSSFCol2_11.GetCell(8).CellStyle = hdstyle6;
                        nextRow2++;
                        #endregion
                        #region 12
                        IRow nextRowHSSFCol2_12 = mysheetHSHD.CreateRow(nextRow2); //设置第0行
                        mysheetHSHD.AddMergedRegion(new CellRangeAddress(nextRow2, nextRow2, 0, 8));
                        nextRowHSSFCol2_12.CreateCell(0).SetCellValue("");

                        nextRowHSSFCol2_12.GetCell(0).CellStyle = hdstyle11;
                        nextRowHSSFCol2_12.CreateCell(1).SetCellValue("");

                        nextRowHSSFCol2_12.GetCell(1).CellStyle = hdstyle12;
                        nextRowHSSFCol2_12.CreateCell(2).SetCellValue("");
                        nextRowHSSFCol2_12.GetCell(2).CellStyle = hdstyle12;
                        nextRowHSSFCol2_12.CreateCell(3).SetCellValue("");
                        nextRowHSSFCol2_12.GetCell(3).CellStyle = hdstyle12;
                        nextRowHSSFCol2_12.CreateCell(4).SetCellValue("");
                        nextRowHSSFCol2_12.GetCell(4).CellStyle = hdstyle12;
                        nextRowHSSFCol2_12.CreateCell(5).SetCellValue("");
                        nextRowHSSFCol2_12.GetCell(5).CellStyle = hdstyle12;
                        nextRowHSSFCol2_12.CreateCell(6).SetCellValue("");
                        nextRowHSSFCol2_12.GetCell(6).CellStyle = hdstyle12;
                        nextRowHSSFCol2_12.CreateCell(7).SetCellValue("");
                        nextRowHSSFCol2_12.GetCell(7).CellStyle = hdstyle12;
                        nextRowHSSFCol2_12.CreateCell(8).SetCellValue("");
                        #endregion

                        nextRowHSSFCol2_12.GetCell(8).CellStyle = hdstyle13;
                        nextRow2++;
                        IRow nextRowHSSFCol2_13 = mysheetHSHD.CreateRow(nextRow2); //设置第0行
                        nextRow2++;
                        IRow nextRowHSSFCol2_14 = mysheetHSHD.CreateRow(nextRow2); //设置第0行
                        nextRow2++;
                        IRow nextRowHSSFCol2_15 = mysheetHSHD.CreateRow(nextRow2); //设置第0行
                        nextRow2++;
                        IRow nextRowHSSFCol2_16 = mysheetHSHD.CreateRow(nextRow2); //设置第0行
                        nextRow2++;
                        IRow nextRowHSSFCol2_17 = mysheetHSHD.CreateRow(nextRow2); //设置第0行
                        nextRow2++;
                        IDrawing drawing = (XSSFDrawing)mysheetHSHD.CreateDrawingPatriarch();
                        string ciclyPath = "." + Path.DirectorySeparatorChar + "Images" + Path.DirectorySeparatorChar + "FS0625cicly.png";
                        byte[] buff = System.IO.File.ReadAllBytes(ciclyPath);
                        int pic = hshdworkbook.AddPicture(buff, XSSFWorkbook.PICTURE_TYPE_PNG);
                        XSSFClientAnchor anchor = new XSSFClientAnchor(250, 0, 250, 0, 2, 3 + (irow * 18), 7, 7 + (irow * 18));
                        //anchor.AnchorType =  AnchorType.MoveAndResize;
                        drawing.CreatePicture(anchor, pic);
                    }

                    //设置打印区域

                    //hshdworkbook.SetPrintArea(sheetnum, int first row, int last row, int first col, int last col);

                    int firstRowhd = 0;
                    int lastRowhd = dtSelect2.Rows.Count * 18;
                    hshdworkbook.SetPrintArea(0, 0, 8, firstRowhd, lastRowhd);
                    string strHDFunctionName = "FS0625_号试货垛标签_" + vcSupplier_id;

                    string strHDFileName = strHDFunctionName + "_导出信息_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + loginInfo.UserId + ".xlsx";
                    string filepathHaoDuoKanBan = fileSavePath + strHDFileName;

                    using (FileStream fs1 = System.IO.File.Create(filepathHaoDuoKanBan))
                    {
                        hshdworkbook.Write(fs1);//向打开的这个xls文件中写入数据  
                        fs1.Close();
                    }
                    if (strHDFileName != "")
                    {
                        strFilePathArray[1] = filepathHaoDuoKanBan;
                    }
                    else
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcSupplier"] = vcSupplier_id;
                        dataRow["vcMessage"] = "号试货垛标签生成失败!";
                        dtMessage.Rows.Add(dataRow);
                        bReault = false;
                    }

                    #endregion

                    #endregion
                    //Console.WriteLine(vcSupplier_id + "第二个附件结束");
                    //Console.WriteLine(vcSupplier_id + "第三个附件开始");
                    #region 第三个附件
                    string[] columnArray3 = { "vcCarType" };
                    DataView dtSelectView3 = dtNewSupplierandWorkArea.DefaultView;
                    DataTable dtSelect3 = dtSelectView3.ToTable(true, columnArray3);//去重后的dt 
                    XSSFWorkbook hsorderworkbook = null;

                    string XltHSOrderPath = rootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Template" + Path.DirectorySeparatorChar + "FS0625_HSOrder.xlsx";
                    using (FileStream fs = System.IO.File.OpenRead(XltHSOrderPath))
                    {
                        hsorderworkbook = new XSSFWorkbook(fs);
                        fs.Close();
                    }

                    ISheet sheetTemp = hsorderworkbook.GetSheet("template");

                    IWorkbook dest = hsorderworkbook;
                    for (int m = 0; m < dtSelect3.Rows.Count; m++)
                    {
                        string strCarType = dtSelect3.Rows[m]["vcCarType"].ToString();
                        //ISheet creatSheet= hsorderworkbook.CreateSheet(strCarType);
                        sheetTemp.CopyTo(dest, strCarType, true, true);
                        //sheetTemp.CopySheet("sssd", true);
                    }
                    int startRowIndex = 7;

                    //sheet.GetRow(1).GetCell(27).SetCellValue(DateTime.Now.ToString("yyyy/MM/dd"));
                    #region 字体样式
                    ICellStyle hsOrderstyle1 = hsorderworkbook.CreateCellStyle();//声明style1对象，设置Excel表格的样式 标题 车型 左边
                    IFont hsOrderfont1 = hsorderworkbook.CreateFont();
                    hsOrderfont1.Color = IndexedColors.Black.Index;
                    hsOrderfont1.IsBold = false; ;
                    hsOrderfont1.FontHeightInPoints = 12;
                    hsOrderstyle1.SetFont(hsOrderfont1);
                    hsOrderstyle1.BorderTop = BorderStyle.Thin;
                    hsOrderstyle1.BorderLeft = BorderStyle.Thin;
                    hsOrderstyle1.BorderBottom = BorderStyle.Thin;

                    ICellStyle hsOrderstyle1R = hsorderworkbook.CreateCellStyle();//声明style1对象，设置Excel表格的样式 标题 车型 左边
                    IFont hsOrderfont1R = hsorderworkbook.CreateFont();
                    hsOrderfont1R.Color = IndexedColors.Black.Index;
                    hsOrderfont1R.IsBold = false; ;
                    hsOrderfont1R.FontHeightInPoints = 12;
                    hsOrderstyle1R.SetFont(hsOrderfont1R);
                    hsOrderstyle1R.Alignment = HorizontalAlignment.Right;//两端自动对齐（自动换行）
                    hsOrderstyle1R.VerticalAlignment = VerticalAlignment.Center;

                    ICellStyle hsOrderstyle2 = hsorderworkbook.CreateCellStyle();//声明style1对象，设置Excel表格的样式 标题 车型 左边
                    hsOrderstyle2.BorderLeft = BorderStyle.Thin;
                    hsOrderstyle2.BorderBottom = BorderStyle.Thin;
                    hsOrderstyle2.Alignment = HorizontalAlignment.Center;//两端自动对齐（自动换行）
                    hsOrderstyle2.VerticalAlignment = VerticalAlignment.Center;
                    ICellStyle hsOrderstyle3 = hsorderworkbook.CreateCellStyle();//声明style1对象，设置Excel表格的样式 标题 车型 左边
                    hsOrderstyle3.BorderLeft = BorderStyle.Thin;
                    hsOrderstyle3.BorderBottom = BorderStyle.Thin;
                    hsOrderstyle3.BorderRight = BorderStyle.Thin;
                    hsOrderstyle3.Alignment = HorizontalAlignment.Center;//两端自动对齐（自动换行）
                    hsOrderstyle3.VerticalAlignment = VerticalAlignment.Center;

                    ICellStyle hsOrderstyle5 = hsorderworkbook.CreateCellStyle();//声明style1对象，设置Excel表格的样式 标题 车型 左边
                    IFont hsOrdersFont5 = hsorderworkbook.CreateFont();
                    hsOrdersFont5.Color = IndexedColors.Black.Index;
                    hsOrdersFont5.IsBold = false; ;
                    hsOrdersFont5.FontHeightInPoints = 12;
                    hsOrderstyle5.SetFont(hsOrdersFont5);

                    ICellStyle hsOrderstyle4 = hsorderworkbook.CreateCellStyle();//声明style1对象，设置Excel表格的样式 标题 车型 左边
                    hsOrderstyle4.BorderLeft = BorderStyle.Thin;
                    hsOrderstyle4.BorderBottom = BorderStyle.Thin;
                    hsOrderstyle4.Alignment = HorizontalAlignment.Left;//两端自动对齐（自动换行）
                    hsOrderstyle4.VerticalAlignment = VerticalAlignment.Center;

                    #endregion
                    for (int m = 0; m < dtSelect3.Rows.Count; m++)
                    {
                        string strCarType = dtSelect3.Rows[m]["vcCarType"].ToString();
                        DataRow[] drArrayCarType = dtNewSupplierandWorkArea.Select("vcCarType='" + strCarType + "' ");
                        DataTable dtNewCarType = drArrayCarType[0].Table.Clone(); // 复制DataRow的表结构
                        foreach (DataRow dr in drArrayCarType)
                        {
                            dtNewCarType.ImportRow(dr);
                        }
                        ISheet sheetOrder = hsorderworkbook.GetSheet(strCarType);
                        sheetOrder.GetRow(1).GetCell(0).SetCellValue("厂家编码:" + vcSupplier_id);//厂家名称vcSupplier_name
                        sheetOrder.GetRow(1).GetCell(0).CellStyle = hsOrderstyle1;
                        sheetOrder.GetRow(1).GetCell(3).SetCellValue("一汽丰田" + strCarType + "补给品号试订单");//一汽丰田84* B补给品号试订单
                        //sheetOrder.GetRow(1).GetCell(0).CellStyle = hsOrderstyle1;
                        sheetOrder.GetRow(1).GetCell(8).SetCellValue(DateTime.Now.ToString("yyyy/MM/dd"));
                        sheetOrder.GetRow(1).GetCell(8).CellStyle = hsOrderstyle1R;
                        //sheetOrder.GetRow(1).GetCell(0).SetCellValue("厂家编码:" + vcSupplier_id);//厂家代码
                        //sheetOrder.GetRow(1).GetCell(0).CellStyle = hsOrderstyle1;

                        sheetOrder.GetRow(9 + dtNewCarType.Rows.Count).GetCell(0).SetCellValue("1）此订单采购的" + strCarType + "号试用补给品，请单独托盘出荷，贴付补给品号试用货垛标签");//厂家代码
                        sheetOrder.GetRow(9 + dtNewCarType.Rows.Count).GetCell(0).CellStyle = hsOrderstyle5;
                        if (dtNewCarType.Rows.Count > 1)
                        {
                            sheetOrder.ShiftRows(startRowIndex, sheetOrder.LastRowNum, dtNewCarType.Rows.Count - 1, true, false);
                            for (int k = startRowIndex; k < startRowIndex + dtNewCarType.Rows.Count - 1; k++)
                            {
                                var rowInsert = sheetOrder.CreateRow(k);

                                for (int col = 0; col <= 8; col++)
                                {
                                    var cellInsert = rowInsert.CreateCell(col);
                                    if (col == 8)
                                    {
                                        cellInsert.CellStyle = hsOrderstyle3;
                                    } else if (col == 2) {
                                        cellInsert.CellStyle = hsOrderstyle4;
                                    }
                                    else
                                    {
                                        cellInsert.CellStyle = hsOrderstyle2;
                                    }
                                }
                            }
                        }
                        //插入数据
                        for (int p = 0; p < dtNewCarType.Rows.Count; p++)
                        {
                            string strPartNo = dtNewCarType.Rows[p]["vcPartNo"].ToString();
                            string strPartNanem = dtNewCarType.Rows[p]["vcPartName"].ToString();
                            string strDock = dtNewCarType.Rows[p]["vcDock"].ToString();
                            string strNum = dtNewCarType.Rows[p]["vcNumber"].ToString();
                            string strNaruRi = dtNewCarType.Rows[p]["dOrderPurposesDate"].ToString();
                            string strChuHeRi = dtNewCarType.Rows[p]["dOrderReceiveDate"].ToString();
                            IRow row = sheetOrder.GetRow(p + startRowIndex);
                            row.Height = 660;
                            row.GetCell(0).SetCellValue(p + 1);
                            row.GetCell(1).SetCellValue(strPartNo);
                            row.GetCell(2).SetCellValue(strPartNanem);
                            row.GetCell(3).SetCellValue(vcSupplier_name);
                            row.GetCell(4).SetCellValue(vcSupplier_id);
                            row.GetCell(5).SetCellValue(strDock);
                            row.GetCell(6).SetCellValue(strNum);
                            row.GetCell(7).SetCellValue(strNaruRi);
                            row.GetCell(8).SetCellValue(strChuHeRi);
                        }
                        IDrawing drawing = (XSSFDrawing)sheetOrder.CreateDrawingPatriarch();
                        string pubiPath = "." + Path.DirectorySeparatorChar + "Images" + Path.DirectorySeparatorChar + "FS0625bujipin.png";
                        byte[] buff = System.IO.File.ReadAllBytes(pubiPath);
                        int pic = hsorderworkbook.AddPicture(buff, XSSFWorkbook.PICTURE_TYPE_PNG);
                        XSSFClientAnchor anchor = new XSSFClientAnchor(1000000, 0, 0, 0, 8, 3, 9, 5);
                        drawing.CreatePicture(anchor, pic);

                        int firstRowHSOrderArea = 0;
                        int lastRowHSorderArea = dtNewCarType.Rows.Count + 14;
                        int IndexSheet = hsorderworkbook.GetSheetIndex(strCarType);
                        hsorderworkbook.SetPrintArea(IndexSheet, 0, 9, firstRowHSOrderArea, lastRowHSorderArea);
                    }
                    hsorderworkbook.RemoveSheetAt(0);

                    string strOrderFunctionName = "FS0625_号试品订单_" + vcSupplier_id;

                    string strOrderFileName = strOrderFunctionName + "_导出信息_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + loginInfo.UserId + ".xlsx";
                    string filepathHSOrderKanBan = fileSavePath + strOrderFileName;

                    using (FileStream fs3 = System.IO.File.Create(filepathHSOrderKanBan))
                    {
                        hsorderworkbook.Write(fs3);//向打开的这个xls文件中写入数据  
                        fs3.Close();
                    }
                    if (strOrderFileName != "")
                    {
                        strFilePathArray[2] = filepathHSOrderKanBan;
                    }
                    else
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcSupplier"] = vcSupplier_id;
                        dataRow["vcMessage"] = "号试品订单生成失败!";
                        dtMessage.Rows.Add(dataRow);
                        bReault = false;
                    }
                    #endregion
                    //Console.WriteLine(vcSupplier_id + "第三个附件结束");
                    //Console.WriteLine(vcSupplier_id + "第三个附件结束");
                    //抄送人dt 通过Tcode 自己定义cCDt
                    DataTable cCDt = null;
                    DataTable dtCCEmail = fs0625_Logic.getCCEmail("C054");
                    if (dtCCEmail.Rows.Count > 0)
                    {
                        cCDt = dtCCEmail;
                    }
                    //Console.WriteLine(vcSupplier_id + "获取抄送者");
                    //邮件主题
                    string strSubject = vcCarTypeChuShi+ "车型 补给品 号试订单发注(重要！)";
                    //邮件内容
                    //strEmailBody += "  请在1个工作日内将是否可以供货的确认结果邮件回复，以下是各个仓库对应的邮箱：<br />";
                    string result = "Success";
                    //Console.WriteLine(vcSupplier_id + "邮件发送开始");
                    result = ComFunction.SendEmailInfo(loginInfo.Email, loginInfo.UnitName, EmailBody, receiverDt, cCDt, strSubject, strFilePathArray, false);
                    if (result == "Success")
                    {
                    }
                    else
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcSupplier"] = vcSupplier_id;
                        dataRow["vcMessage"] = "邮件发送失败，邮件发送公共方法未知原因！";
                        dtMessage.Rows.Add(dataRow);
                        bReault = false;
                    }
                    //Console.WriteLine(vcSupplier_id + "邮件发送结束");
                    //Console.WriteLine(vcSupplier_id + "下一轮开始");
                }
                if (!bReault)
                {
                    //弹出错误dtMessage
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "邮件发送成功！";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                Console.WriteLine("catch报错"+ex);
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2511", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "邮件发送失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

    }
}