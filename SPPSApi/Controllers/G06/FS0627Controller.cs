using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace SPPSApi.Controllers.G06
{
    [Route("api/FS0627/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0627Controller : BaseController
    {
        FS0627_Logic fs0627_Logic = new FS0627_Logic();
        private readonly string FunctionID = "FS0627";

        private readonly IWebHostEnvironment _webHostEnvironment;


        public FS0627Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        #region 绑定 发注工厂直接从表中数据
        [HttpPost]
        [EnableCors("any")]
        public string bindInjectionFactoryApi()
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
                Dictionary<string, object> res = new Dictionary<string, object>();
                List<Object> dataList_C000 = ComFunction.convertAllToResult(ComFunction.getTCode("C000"));//发注工厂
                res.Add("C000", dataList_C000);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2101", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "绑定发注工厂列表失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

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
                //DataTable dtSupplier = fs0627_Logic.GetSupplier();

                DataTable dtSupplier = fs0627_Logic.GetSupplier(); ;
                List<Object> dataList_Supplier = ComFunction.convertToResult(dtSupplier, new string[] { "vcValue", "vcName" });
                List<Object> dataList_C000 = ComFunction.convertAllToResult(ComFunction.getTCode("C000"));//ERSP
              
                res.Add("Supplier", dataList_Supplier);
                res.Add("C000", dataList_C000);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2501", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "初始化失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
        #region 绑定内外
        [HttpPost]
        [EnableCors("any")]
        public string bindInsideOutsideType()
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
                DataTable dt = fs0627_Logic.BindInsideOutsideType();
                List<Object> dataList = ComFunction.convertToResult(dt, new string[] { "vcCodeId", "vcCodeName" });
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2102", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "绑定内外列表失败";
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
            string vcInjectionFactory = dataForm.vcInjectionFactory == null ? "" : dataForm.vcInjectionFactory;
            string vcProject = dataForm.vcProject == null ? "" : dataForm.vcProject;
            string vcTargetYear = dataForm.vcTargetYear == null ? "" : dataForm.vcTargetYear;
            try
            {
                DataSet ds = fs0627_Logic.Search(vcInjectionFactory, vcProject, vcTargetYear);
                DataTable dtNum = ds.Tables[0];
                DataTable dtMonty = ds.Tables[1];
                // "vcPackPlant", "vcTargetYear", "vcPartNo", "vcInjectionFactory", "vcInsideOutsideType", "vcSupplier_id", "vcWorkArea", "vcCarType",
                List<Object> dataListNum = ComFunction.convertAllToResult(dtNum);
                List<Object> dataListMoney = ComFunction.convertAllToResult(dtMonty);
                var obbject = new object[] {
                    new { listNum=dataListNum},
                    new { listMoney=dataListMoney}
                };
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = obbject;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2703", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 导出
        [HttpPost]
        [EnableCors("any")]
        public string createReportApi([FromBody] dynamic data)
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
            string vcInjectionFactory = dataForm.vcInjectionFactory == null ? "" : dataForm.vcInjectionFactory;
            string vcProject = dataForm.vcProject == null ? "" : dataForm.vcProject;
            string vcTargetYear = dataForm.vcTargetYear == null ? "" : dataForm.vcTargetYear;
            string filepath = "";
            try
            {
                DataSet ds = fs0627_Logic.Search(vcInjectionFactory, vcProject, vcTargetYear);
                DataTable dtNum = ds.Tables[0];
                DataTable dtMonty = ds.Tables[1];

                #region 导出报表
               

                XSSFWorkbook hssfworkbook = new XSSFWorkbook();//用于创建xlsx
                ISheet mysheetHSSF = hssfworkbook.CreateSheet("销售数据");//创建sheet名称 
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
                //设置第一种
                //IFont font = hssfworkbook.CreateFont();
                ////font.Boldweight = (Int16)FontBoldWeight.Bold;//原始字体
                ////【Tips】
                //// 1.Boldweight 要使用(Int16)FontBoldWeight 对应的数值 否则无效
                //font.Color = IndexedColors.Black.Index; //设置字体颜色
                ////font.FontHeight = 15;//设置字体高度【FontHeightInPoints也是设置字体高度，我还不知道有啥区别】
                ////font.FontName = "宋体";//设置字体
                ////font.IsBold = true;//是否加粗
                ////font.IsItalic = false;//是否斜体
                ////font.IsStrikeout = false;//是否加删除线
                ////font.TypeOffset = FontSuperScript.Sub;//设置脚本上的字体【Sub 下；Super 上】
                ////font.Underline = FontUnderlineType.Single;//下划线【Single一条线；Double两条线】
                ////创建CellStyle并加载字体
                //ICellStyle Fontstyle = hssfworkbook.CreateCellStyle();
                //Fontstyle.SetFont(font);
                ////设置第二种
                //IFont font1 = hssfworkbook.CreateFont();
                ////font.Boldweight = (Int16)FontBoldWeight.Bold;//原始字体
                ////【Tips】
                //// 1.Boldweight 要使用(Int16)FontBoldWeight 对应的数值 否则无效
                //font1.Color = IndexedColors.Black.Index; //设置字体颜色
                ////font1.FontHeight = 9;//设置字体高度【FontHeightInPoints也是设置字体高度，我还不知道有啥区别】
                ////font1.FontName = "宋体";//设置字体
                ////font1.IsBold = false;//是否加粗
                ////font1.IsItalic = false;//是否斜体
                ////font1.IsStrikeout = false;//是否加删除线
                ////font.TypeOffset = FontSuperScript.Sub;//设置脚本上的字体【Sub 下；Super 上】
                ////font.Underline = FontUnderlineType.Single;//下划线【Single一条线；Double两条线】
                ////创建CellStyle并加载字体
                //ICellStyle Fontstyle1 = hssfworkbook.CreateCellStyle();
                //Fontstyle1.SetFont(font1);
                ////设置第三种
                //IFont font2 = hssfworkbook.CreateFont();
                ////font.Boldweight = (Int16)FontBoldWeight.Bold;//原始字体
                ////【Tips】
                //// 1.Boldweight 要使用(Int16)FontBoldWeight 对应的数值 否则无效
                //font2.Color = IndexedColors.Black.Index; //设置字体颜色
                //font2.FontHeight = 9;//设置字体高度【FontHeightInPoints也是设置字体高度，我还不知道有啥区别】
                ////font2.FontName = "宋体";//设置字体
                ////font2.IsBold = true;//是否加粗
                ////font2.IsItalic = false;//是否斜体
                ////font2.IsStrikeout = false;//是否加删除线
                ////font.TypeOffset = FontSuperScript.Sub;//设置脚本上的字体【Sub 下；Super 上】
                ////font.Underline = FontUnderlineType.Single;//下划线【Single一条线；Double两条线】
                ////创建CellStyle并加载字体
                //ICellStyle Fontstyle2 = hssfworkbook.CreateCellStyle();
                //Fontstyle2.SetFont(font2);
                #endregion

                ICellStyle style1 = hssfworkbook.CreateCellStyle();//声明style1对象，设置Excel表格的样式
                ICellStyle style2 = hssfworkbook.CreateCellStyle();//9号字体加粗 没有背景色
                ICellStyle style3 = hssfworkbook.CreateCellStyle();//9号字体不加粗 没有背景色
                ICellStyle style4 = hssfworkbook.CreateCellStyle();//9号字体不加粗 深蓝蓝
                ICellStyle style5 = hssfworkbook.CreateCellStyle();//9号字体不加粗 紫色
                ICellStyle style6 = hssfworkbook.CreateCellStyle();//9号字体不加粗 LIGHT_TURQUOISE
                IFont font = hssfworkbook.CreateFont();
                font.Color = IndexedColors.Black.Index;
                font.IsBold = true; ;
                font.FontHeightInPoints = 15;
                //font.FontName = "宋体";
                style1.SetFont(font);
                style1.Alignment = HorizontalAlignment.Center;//两端自动对齐（自动换行）
                style1.VerticalAlignment = VerticalAlignment.Center;
                style1.BorderLeft = BorderStyle.Thin;
                style1.BorderRight = BorderStyle.Thin;
                style1.BorderTop = BorderStyle.Thin;

                IFont font2 = hssfworkbook.CreateFont();
                font2.Color = IndexedColors.Black.Index;
                font2.IsBold = true; ;
                font2.FontHeightInPoints = 9;
                //font.FontName = "宋体";
                style2.SetFont(font2);
                style2.Alignment = HorizontalAlignment.Center;
                style2.VerticalAlignment = VerticalAlignment.Center;
                style2.BorderLeft = BorderStyle.Thin;
                style2.BorderRight = BorderStyle.Thin;
                style2.BorderTop = BorderStyle.Thin;

                IFont font3 = hssfworkbook.CreateFont();
                font3.Color = IndexedColors.Black.Index;
                font3.IsBold = false; ;
                font3.FontHeightInPoints = 9;
                //font.FontName = "宋体";
                style3.SetFont(font3);
                style3.Alignment = HorizontalAlignment.Center;
                style3.VerticalAlignment = VerticalAlignment.Center;
                style3.BorderLeft = BorderStyle.Thin;
                style3.BorderRight = BorderStyle.Thin;
                style3.BorderTop = BorderStyle.Thin;

                IFont font4 = hssfworkbook.CreateFont();
                font4.Color = IndexedColors.Black.Index;
                font4.IsBold = false; ;
                font4.FontHeightInPoints = 9;
                //font.FontName = "宋体";
                style4.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Turquoise.Index;
                style4.FillPattern = FillPattern.SolidForeground;
                style4.SetFont(font4);
                style4.Alignment = HorizontalAlignment.Center;
                style4.VerticalAlignment = VerticalAlignment.Center;
                style4.BorderLeft = BorderStyle.Thin;
                style4.BorderRight = BorderStyle.Thin;
                style4.BorderTop = BorderStyle.Thin;

                IFont font5 = hssfworkbook.CreateFont();
                font5.Color = IndexedColors.Black.Index;
                font5.IsBold = false; ;
                font5.FontHeightInPoints = 9;
                //font.FontName = "宋体";
                style5.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Lavender.Index;
                style5.FillPattern = FillPattern.SolidForeground;
                style5.SetFont(font5);
                style5.Alignment = HorizontalAlignment.Center;
                style5.VerticalAlignment = VerticalAlignment.Center;
                style5.BorderLeft = BorderStyle.Thin;
                style5.BorderRight = BorderStyle.Thin;
                style5.BorderTop = BorderStyle.Thin;
                style5.BorderBottom = BorderStyle.Thin;

                IFont font6 = hssfworkbook.CreateFont();
                font6.Color = IndexedColors.Black.Index;
                font6.IsBold = false; ;
                font5.FontHeightInPoints = 9;
                //font6.FontName = "宋体";
                style6.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightTurquoise.Index;
                style6.FillPattern = FillPattern.SolidForeground;
                style6.SetFont(font6);
                style6.Alignment = HorizontalAlignment.Center;
                style6.VerticalAlignment = VerticalAlignment.Center;
                style6.BorderLeft = BorderStyle.Thin;
                style6.BorderRight = BorderStyle.Thin;
                style6.BorderTop = BorderStyle.Thin;
                #region 设置列的宽度
                mysheetHSSF.SetColumnWidth(0, 17 * 256); //设置第1列的列宽为17个字符
                mysheetHSSF.SetColumnWidth(1, 5 * 256); //设置第2列的列宽为31个字符
                mysheetHSSF.SetColumnWidth(2, 10 * 256); //设置第3列的列宽为10个字符
                mysheetHSSF.SetColumnWidth(3, 10 * 256); //设置第4列的列宽为10个字符
                mysheetHSSF.SetColumnWidth(4, 10 * 256); //设置第5列的列宽为10个字符
                mysheetHSSF.SetColumnWidth(5, 10 * 256); //设置第6列的列宽为10个字符
                mysheetHSSF.SetColumnWidth(6, 10 * 256); //设置第7列的列宽为10个字符
                mysheetHSSF.SetColumnWidth(7, 10 * 256); //设置第8列的列宽为10个字符
                mysheetHSSF.SetColumnWidth(8, 10 * 256); //设置第9列的列宽为10个字符
                mysheetHSSF.SetColumnWidth(9, 10 * 256); //设置第10列的列宽为10个字符
                mysheetHSSF.SetColumnWidth(10, 10 * 256); //设置第11列的列宽为10个字符
                mysheetHSSF.SetColumnWidth(11, 10 * 256); //设置第12列的列宽为10个字符
                mysheetHSSF.SetColumnWidth(12, 10 * 256); //设置第13列的列宽为10个字符
                mysheetHSSF.SetColumnWidth(13, 17 * 256); //设置第14列的列宽为17个字符
                #endregion

                #region //设置第一行

                IRow FirstrowHSSF = mysheetHSSF.CreateRow(0);//创建row 行 从0开始

                FirstrowHSSF.Height = 50 * 20; //设置高度为50个点
                FirstrowHSSF.CreateCell(0).SetCellValue("销售数据用");
                FirstrowHSSF.GetCell(0).CellStyle = style1;//将CellStyle应用于具体单元格 
                #endregion
                #region //设置第二行 
                //设置第二行 统计项目  合并单元格【CellRangeAddress(开始行,结束行,开始列,结束列)】
                mysheetHSSF.AddMergedRegion(new CellRangeAddress(1, 1, 0, 1)); //合并单元格第二行从第1列到第2列
                IRow SecondRowHSSF = mysheetHSSF.CreateRow(1); //添加第二行
                SecondRowHSSF.CreateCell(0).SetCellValue("统计项目");
                SecondRowHSSF.GetCell(0).CellStyle = style3;//将CellStyle应用于具体单元格 
                SecondRowHSSF.CreateCell(1).SetCellValue("");
                SecondRowHSSF.GetCell(1).CellStyle = style3;//将CellStyle应用于具体单元格 
                SecondRowHSSF.CreateCell(2).SetCellValue("1月");
                SecondRowHSSF.GetCell(2).CellStyle = style3;

                SecondRowHSSF.CreateCell(3).SetCellValue("2月");
                SecondRowHSSF.GetCell(3).CellStyle = style3;

                SecondRowHSSF.CreateCell(4).SetCellValue("3月");
                SecondRowHSSF.GetCell(4).CellStyle = style3;

                SecondRowHSSF.CreateCell(5).SetCellValue("4月");
                SecondRowHSSF.GetCell(5).CellStyle = style3;

                SecondRowHSSF.CreateCell(6).SetCellValue("5月");
                SecondRowHSSF.GetCell(6).CellStyle = style3;

                SecondRowHSSF.CreateCell(7).SetCellValue("6月");
                SecondRowHSSF.GetCell(7).CellStyle = style3;

                SecondRowHSSF.CreateCell(8).SetCellValue("7月");
                SecondRowHSSF.GetCell(8).CellStyle = style3;

                SecondRowHSSF.CreateCell(9).SetCellValue("8月");
                SecondRowHSSF.GetCell(9).CellStyle = style3;

                SecondRowHSSF.CreateCell(10).SetCellValue("9月");
                SecondRowHSSF.GetCell(10).CellStyle = style3;

                SecondRowHSSF.CreateCell(11).SetCellValue("10月");
                SecondRowHSSF.GetCell(11).CellStyle = style3;

                SecondRowHSSF.CreateCell(12).SetCellValue("11月");
                SecondRowHSSF.GetCell(12).CellStyle = style3;


                SecondRowHSSF.CreateCell(13).SetCellValue("12月");
                SecondRowHSSF.GetCell(13).CellStyle = style3;

                SecondRowHSSF.CreateCell(14).SetCellValue("合计");
                SecondRowHSSF.GetCell(14).CellStyle = style3;
                #endregion

                #region //设置第三行 
                //设置第二行 统计项目  合并单元格【CellRangeAddress(开始行,结束行,开始列,结束列)】
                IRow ThreeRowHSSF = mysheetHSSF.CreateRow(2); //添加第二行
                ThreeRowHSSF.CreateCell(0).SetCellValue("1)销售额(万元）");
                ThreeRowHSSF.GetCell(0).CellStyle = style4;//将CellStyle应用于具体单元格 

                ThreeRowHSSF.CreateCell(1).SetCellValue("");
                ThreeRowHSSF.GetCell(1).CellStyle = style4;
                // 金额数赋值
                int lastMontyRow = dtMonty.Rows.Count - 1;
                for (var i = 3; i < dtMonty.Columns.Count; i++)
                {
                    ThreeRowHSSF.CreateCell(i - 1).SetCellValue(dtMonty.Rows[lastMontyRow][i].ToString());
                    ThreeRowHSSF.GetCell(i - 1).CellStyle = style4;
                }

                #endregion
                #region //设置第四行 
                //设置第四行  统计项目  合并单元格【CellRangeAddress(开始行,结束行,开始列,结束列)】
                IRow FourRowHSSF = mysheetHSSF.CreateRow(3); //添加第二行
                FourRowHSSF.CreateCell(0).SetCellValue("4)销售数量（件）");
                FourRowHSSF.GetCell(0).CellStyle = style5;//将CellStyle应用于具体单元格 

                FourRowHSSF.CreateCell(1).SetCellValue("");
                FourRowHSSF.GetCell(1).CellStyle = style5;
                // 金额数赋值
                int lastNumRow = dtNum.Rows.Count - 1;
                for (var i = 3; i < dtNum.Columns.Count; i++)
                {
                    FourRowHSSF.CreateCell(i - 1).SetCellValue(dtNum.Rows[lastNumRow][i].ToString());
                    FourRowHSSF.GetCell(i - 1).CellStyle = style5;
                }

                #endregion

                #region 设置第五行
                IRow FiveRowHSSF = mysheetHSSF.CreateRow(4);
                #endregion
                #region 设置第六行
                IRow SixRowHSSF = mysheetHSSF.CreateRow(5);
                SixRowHSSF.CreateCell(0).SetCellValue("需求统计用");
                #endregion
                #region 设置第七行
                IRow SevenRowHSSF = mysheetHSSF.CreateRow(6);
                SevenRowHSSF.CreateCell(0).SetCellValue("销售数量（件)");
                #endregion
                #region 设置第八行 存在列的合并单元格
                int heBingStartRow = 7;
                int hebingEndRow = 8;
                int nextRow = 7;
                bool isNeedNumCol = true;//是否需要设置列头
                if (dtNum.Rows.Count > 0)
                {
                    string strPlantName = dtNum.Rows[0]["vcInjectionFactory"].ToString();
                    int j = 0;
                    for (int i = 1; i < dtNum.Rows.Count; i++)
                    {
                        if (dtNum.Rows[i]["vcInjectionFactory"].ToString() != strPlantName)
                        {
                            if (heBingStartRow != hebingEndRow)
                            {
                                mysheetHSSF.AddMergedRegion(new CellRangeAddress(heBingStartRow, hebingEndRow, 0, 0));
                            }
                            if (isNeedNumCol)
                            {
                                #region 追加列头
                                IRow EightRowHSSF = mysheetHSSF.CreateRow(nextRow);
                                EightRowHSSF.CreateCell(0).SetCellValue(strPlantName);
                                EightRowHSSF.GetCell(0).CellStyle = style6;//将CellStyle应用于具体单元格 
                                EightRowHSSF.CreateCell(1).SetCellValue("工程");
                                EightRowHSSF.GetCell(1).CellStyle = style6;//将CellStyle应用于具体单元格 

                                EightRowHSSF.CreateCell(2).SetCellValue("1月");
                                EightRowHSSF.GetCell(2).CellStyle = style6;//将CellStyle应用于具体单元格 

                                EightRowHSSF.CreateCell(3).SetCellValue("2月");
                                EightRowHSSF.GetCell(3).CellStyle = style6;//将CellStyle应用于具体单元格 

                                EightRowHSSF.CreateCell(4).SetCellValue("3月");
                                EightRowHSSF.GetCell(4).CellStyle = style6;//将CellStyle应用于具体单元格 

                                EightRowHSSF.CreateCell(5).SetCellValue("4月");
                                EightRowHSSF.GetCell(5).CellStyle = style6;//将CellStyle应用于具体单元格 

                                EightRowHSSF.CreateCell(6).SetCellValue("5月");
                                EightRowHSSF.GetCell(6).CellStyle = style6;//将CellStyle应用于具体单元格 

                                EightRowHSSF.CreateCell(7).SetCellValue("6月");
                                EightRowHSSF.GetCell(7).CellStyle = style6;//将CellStyle应用于具体单元格 

                                EightRowHSSF.CreateCell(8).SetCellValue("7月");
                                EightRowHSSF.GetCell(8).CellStyle = style6;//将CellStyle应用于具体单元格 

                                EightRowHSSF.CreateCell(9).SetCellValue("8月");
                                EightRowHSSF.GetCell(9).CellStyle = style6;//将CellStyle应用于具体单元格 

                                EightRowHSSF.CreateCell(10).SetCellValue("9月");
                                EightRowHSSF.GetCell(10).CellStyle = style6;//将CellStyle应用于具体单元格 

                                EightRowHSSF.CreateCell(11).SetCellValue("10月");
                                EightRowHSSF.GetCell(11).CellStyle = style6;//将CellStyle应用于具体单元格 

                                EightRowHSSF.CreateCell(12).SetCellValue("11月");
                                EightRowHSSF.GetCell(12).CellStyle = style6;//将CellStyle应用于具体单元格 

                                EightRowHSSF.CreateCell(13).SetCellValue("12月");
                                EightRowHSSF.GetCell(13).CellStyle = style6;//将CellStyle应用于具体单元格 

                                EightRowHSSF.CreateCell(14).SetCellValue("合计");
                                EightRowHSSF.GetCell(14).CellStyle = style6;//将CellStyle应用于具体单元格 
                                #endregion
                                nextRow++;
                            }
                            for ( ; j < i; j++)//行
                            {
                                IRow NextRowHSSF = mysheetHSSF.CreateRow(nextRow);
                                if (heBingStartRow==nextRow)
                                {
                                    NextRowHSSF.CreateCell(0).SetCellValue(strPlantName);
                                    NextRowHSSF.GetCell(0).CellStyle = style6;//将CellStyle应用于具体单元格 
                                }
                                int colNum = 1;//用于计数
                                for (var k = 1; k < dtNum.Columns.Count; k++)
                                {
                                    if (k != 2)//k=2是 年份 去掉
                                    {
                                        NextRowHSSF.CreateCell(colNum).SetCellValue(dtNum.Rows[j][k].ToString());
                                        NextRowHSSF.GetCell(colNum).CellStyle = style6;
                                        colNum++;
                                    }
                                }
                                nextRow++;
                            }
                            strPlantName = dtNum.Rows[i]["vcInjectionFactory"].ToString();
                            isNeedNumCol = false;
                            heBingStartRow = hebingEndRow+1;
                            hebingEndRow++;
                            j = i;
                        }
                        else
                        {
                            hebingEndRow++;
                                //IRow NextRowHSSF = mysheetHSSF.CreateRow(nextRow);
                                //int colNum = 1;//用于计数
                                //for (var k = 1; k < dtNum.Columns.Count; k++)
                                //{
                                //    if (k != 2)//k=2是 年份 去掉
                                //    {
                                //        NextRowHSSF.CreateCell(colNum).SetCellValue(dtNum.Rows[i][k].ToString());
                                //        NextRowHSSF.GetCell(colNum).CellStyle = styleCenter;
                                //        NextRowHSSF.GetCell(colNum).CellStyle = Fontstyle1;
                                //    }
                                //    colNum++;
                                //}
                                //nextRow++;
                        }
                    }

                    //创建销售数据最后一行
                    #region
                    IRow NineRowHSSF = mysheetHSSF.CreateRow(nextRow);
                    NineRowHSSF.CreateCell(0).SetCellValue("4)销售数量（件）");
                    NineRowHSSF.GetCell(0).CellStyle = style5;//将CellStyle应用于具体单元格 

                    NineRowHSSF.CreateCell(1).SetCellValue("");
                    NineRowHSSF.CreateCell(1).CellStyle = style2;
                    for (var i = 3; i < dtNum.Columns.Count; i++)
                    {
                        NineRowHSSF.CreateCell(i - 1).SetCellValue(dtNum.Rows[dtNum.Rows.Count - 1][i].ToString());
                        NineRowHSSF.GetCell(i - 1).CellStyle = style2;
                    }
                    nextRow ++;
                    #endregion
                }
                else
                {
                    #region  不存在数据 就只创建列头 
                    IRow EightRowHSSF = mysheetHSSF.CreateRow(nextRow);
                    EightRowHSSF.CreateCell(0).SetCellValue("");
                    EightRowHSSF.GetCell(0).CellStyle = style2;//将CellStyle应用于具体单元格 

                    EightRowHSSF.CreateCell(1).SetCellValue("工程");
                    EightRowHSSF.GetCell(1).CellStyle = style2;//将CellStyle应用于具体单元格 

                    EightRowHSSF.CreateCell(2).SetCellValue("1月");
                    EightRowHSSF.GetCell(2).CellStyle = style2;//将CellStyle应用于具体单元格 

                    EightRowHSSF.CreateCell(3).SetCellValue("2月");
                    EightRowHSSF.GetCell(3).CellStyle = style2;//将CellStyle应用于具体单元格 

                    EightRowHSSF.CreateCell(4).SetCellValue("3月");
                    EightRowHSSF.GetCell(4).CellStyle = style2;//将CellStyle应用于具体单元格 

                    EightRowHSSF.CreateCell(5).SetCellValue("4月");
                    EightRowHSSF.GetCell(5).CellStyle = style2;//将CellStyle应用于具体单元格 

                    EightRowHSSF.CreateCell(6).SetCellValue("5月");
                    EightRowHSSF.GetCell(6).CellStyle = style2;//将CellStyle应用于具体单元格 

                    EightRowHSSF.CreateCell(7).SetCellValue("6月");
                    EightRowHSSF.GetCell(7).CellStyle = style2;//将CellStyle应用于具体单元格 

                    EightRowHSSF.CreateCell(8).SetCellValue("7月");
                    EightRowHSSF.GetCell(8).CellStyle = style2;//将CellStyle应用于具体单元格 

                    EightRowHSSF.CreateCell(9).SetCellValue("8月");
                    EightRowHSSF.GetCell(9).CellStyle = style2;//将CellStyle应用于具体单元格 

                    EightRowHSSF.CreateCell(10).SetCellValue("9月");
                    EightRowHSSF.GetCell(10).CellStyle = style2;//将CellStyle应用于具体单元格 

                    EightRowHSSF.CreateCell(11).SetCellValue("10月");
                    EightRowHSSF.GetCell(11).CellStyle = style2;//将CellStyle应用于具体单元格 

                    EightRowHSSF.CreateCell(12).SetCellValue("11月");
                    EightRowHSSF.GetCell(12).CellStyle = style2;//将CellStyle应用于具体单元格 

                    EightRowHSSF.CreateCell(13).SetCellValue("12月");
                    EightRowHSSF.GetCell(13).CellStyle = style2;//将CellStyle应用于具体单元格 

                    EightRowHSSF.CreateCell(14).SetCellValue("合计");
                    EightRowHSSF.GetCell(14).CellStyle = style2;//将CellStyle应用于具体单元格 
                    nextRow++;
                    //下一行
                    IRow NineRowHSSF = mysheetHSSF.CreateRow(nextRow);
                    NineRowHSSF.CreateCell(0).SetCellValue("4)销售数量（件）");
                    NineRowHSSF.GetCell(0).CellStyle = style2;//将CellStyle应用于具体单元格 
                    #endregion
                    nextRow ++;
                }

                // 写 销售额(万元）
                #region 设置第十行
                IRow MoneySellNameRowHSSF = mysheetHSSF.CreateRow(nextRow);
                MoneySellNameRowHSSF.CreateCell(0).SetCellValue("销售额(万元）");
                //MoneySellNameRowHSSF.GetCell(0).CellStyle = style5;//将CellStyle应用于具体单元格 
                nextRow++;
                #endregion
                #region 销售万元明细
                heBingStartRow = nextRow;
                hebingEndRow = nextRow+1;
                isNeedNumCol = true;
                if (dtMonty.Rows.Count > 0)
                {
                    string strPlantName = dtMonty.Rows[0]["vcInjectionFactory"].ToString();
                    int j = 0;
                    for (int i = 1; i < dtMonty.Rows.Count; i++)
                    {
                        if (dtMonty.Rows[i]["vcInjectionFactory"].ToString() != strPlantName)
                        {
                            if (heBingStartRow!=hebingEndRow)
                            {
                                mysheetHSSF.AddMergedRegion(new CellRangeAddress(heBingStartRow, hebingEndRow, 0, 0));
                            }
                            
                            if (isNeedNumCol)
                            {
                                #region 追加列头
                                IRow NextRowHSSF = mysheetHSSF.CreateRow(nextRow);
                                NextRowHSSF.CreateCell(0).SetCellValue(strPlantName);
                                NextRowHSSF.GetCell(0).CellStyle = style6;//将CellStyle应用于具体单元格 
                                NextRowHSSF.CreateCell(1).SetCellValue("工程");
                                NextRowHSSF.GetCell(1).CellStyle = style6;//将CellStyle应用于具体单元格 

                                NextRowHSSF.CreateCell(2).SetCellValue("1月");
                                NextRowHSSF.GetCell(2).CellStyle = style6;//将CellStyle应用于具体单元格 

                                NextRowHSSF.CreateCell(3).SetCellValue("2月");
                                NextRowHSSF.GetCell(3).CellStyle = style6;//将CellStyle应用于具体单元格 

                                NextRowHSSF.CreateCell(4).SetCellValue("3月");
                                NextRowHSSF.GetCell(4).CellStyle = style6;//将CellStyle应用于具体单元格 

                                NextRowHSSF.CreateCell(5).SetCellValue("4月");
                                NextRowHSSF.GetCell(5).CellStyle = style6;//将CellStyle应用于具体单元格 

                                NextRowHSSF.CreateCell(6).SetCellValue("5月");
                                NextRowHSSF.GetCell(6).CellStyle = style6;//将CellStyle应用于具体单元格 

                                NextRowHSSF.CreateCell(7).SetCellValue("6月");
                                NextRowHSSF.GetCell(7).CellStyle = style6;//将CellStyle应用于具体单元格 

                                NextRowHSSF.CreateCell(8).SetCellValue("7月");
                                NextRowHSSF.GetCell(8).CellStyle = style6;//将CellStyle应用于具体单元格 

                                NextRowHSSF.CreateCell(9).SetCellValue("8月");
                                NextRowHSSF.GetCell(9).CellStyle = style6;//将CellStyle应用于具体单元格 

                                NextRowHSSF.CreateCell(10).SetCellValue("9月");
                                NextRowHSSF.GetCell(10).CellStyle = style6;//将CellStyle应用于具体单元格 

                                NextRowHSSF.CreateCell(11).SetCellValue("10月");
                                NextRowHSSF.GetCell(11).CellStyle = style6;//将CellStyle应用于具体单元格 

                                NextRowHSSF.CreateCell(12).SetCellValue("11月");
                                NextRowHSSF.GetCell(12).CellStyle = style6;//将CellStyle应用于具体单元格 

                                NextRowHSSF.CreateCell(13).SetCellValue("12月");
                                NextRowHSSF.GetCell(13).CellStyle = style6;//将CellStyle应用于具体单元格 

                                NextRowHSSF.CreateCell(14).SetCellValue("合计");
                                NextRowHSSF.GetCell(14).CellStyle = style6;//将CellStyle应用于具体单元格 
                                #endregion
                                nextRow++;

                            }
                            for (; j < i; j++)//行
                            {
                                IRow NextRowHSSF = mysheetHSSF.CreateRow(nextRow);
                                if (heBingStartRow == nextRow)
                                {
                                    NextRowHSSF.CreateCell(0).SetCellValue(strPlantName);
                                    NextRowHSSF.GetCell(0).CellStyle = style6;//将CellStyle应用于具体单元格 
                                }
                                int colNum = 1;//用于计数
                                for (var k = 1; k < dtMonty.Columns.Count; k++)
                                {
                                    if (k != 2)//k=2是 年份 去掉
                                    {
                                        NextRowHSSF.CreateCell(colNum).SetCellValue(dtMonty.Rows[j][k].ToString());
                                        NextRowHSSF.GetCell(colNum).CellStyle = style6;
                                        colNum++;
                                    }
                                }
                                nextRow++;
                            }
                            strPlantName = dtNum.Rows[i]["vcInjectionFactory"].ToString();
                            isNeedNumCol = false;
                            heBingStartRow = hebingEndRow + 1;
                            hebingEndRow++;
                            j = i;
                        }
                        else
                        {
                            hebingEndRow++;
                            //IRow NextRowHSSF = mysheetHSSF.CreateRow(nextRow);
                            //int colNum = 1;//用于计数
                            //for (var k = 1; k < dtMonty.Columns.Count; k++)
                            //{
                            //    if (k != 2)//k=2是 年份 去掉
                            //    {
                            //        NextRowHSSF.CreateCell(colNum).SetCellValue(dtMonty.Rows[i][k].ToString());
                            //        NextRowHSSF.GetCell(colNum).CellStyle = styleCenter;
                            //        NextRowHSSF.GetCell(colNum).CellStyle = Fontstyle1;
                            //    }
                            //    colNum++;
                            //}
                            //nextRow++;
                        }
                    }

                    //创建销售数据最后一行
                    #region
                    IRow NineRowHSSF = mysheetHSSF.CreateRow(nextRow);
                    NineRowHSSF.CreateCell(0).SetCellValue("1)销售额(万元）");
                    NineRowHSSF.GetCell(0).CellStyle = style4;//将CellStyle应用于具体单元格 

                    NineRowHSSF.CreateCell(1).SetCellValue("");
                    NineRowHSSF.CreateCell(1).CellStyle = style2;
                    for (var i = 3; i < dtMonty.Columns.Count; i++)
                    {
                        NineRowHSSF.CreateCell(i - 1).SetCellValue(dtMonty.Rows[dtMonty.Rows.Count - 1][i].ToString());
                        NineRowHSSF.GetCell(i - 1).CellStyle = style2;
                    }
                    nextRow++;
                    #endregion
                }
                else
                {
                    #region  不存在数据 就只创建列头 
                    IRow EightRowHSSF = mysheetHSSF.CreateRow(nextRow);
                    EightRowHSSF.CreateCell(0).SetCellValue("");
                    EightRowHSSF.GetCell(0).CellStyle = style2;//将CellStyle应用于具体单元格 

                    EightRowHSSF.CreateCell(1).SetCellValue("工程");
                    EightRowHSSF.GetCell(1).CellStyle = style2;//将CellStyle应用于具体单元格 

                    EightRowHSSF.CreateCell(2).SetCellValue("1月");
                    EightRowHSSF.GetCell(2).CellStyle = style2;//将CellStyle应用于具体单元格 

                    EightRowHSSF.CreateCell(3).SetCellValue("2月");
                    EightRowHSSF.GetCell(3).CellStyle = style2;//将CellStyle应用于具体单元格 

                    EightRowHSSF.CreateCell(4).SetCellValue("3月");
                    EightRowHSSF.GetCell(4).CellStyle = style2;//将CellStyle应用于具体单元格 

                    EightRowHSSF.CreateCell(5).SetCellValue("4月");
                    EightRowHSSF.GetCell(5).CellStyle = style2;//将CellStyle应用于具体单元格 

                    EightRowHSSF.CreateCell(6).SetCellValue("5月");
                    EightRowHSSF.GetCell(6).CellStyle = style2;//将CellStyle应用于具体单元格 

                    EightRowHSSF.CreateCell(7).SetCellValue("6月");
                    EightRowHSSF.GetCell(7).CellStyle = style2;//将CellStyle应用于具体单元格 

                    EightRowHSSF.CreateCell(8).SetCellValue("7月");
                    EightRowHSSF.GetCell(8).CellStyle = style2;//将CellStyle应用于具体单元格 

                    EightRowHSSF.CreateCell(9).SetCellValue("8月");
                    EightRowHSSF.GetCell(9).CellStyle = style2;//将CellStyle应用于具体单元格 

                    EightRowHSSF.CreateCell(10).SetCellValue("9月");
                    EightRowHSSF.GetCell(10).CellStyle = style2;//将CellStyle应用于具体单元格 

                    EightRowHSSF.CreateCell(11).SetCellValue("10月");
                    EightRowHSSF.GetCell(11).CellStyle = style2;//将CellStyle应用于具体单元格 

                    EightRowHSSF.CreateCell(12).SetCellValue("11月");
                    EightRowHSSF.GetCell(12).CellStyle = style2;//将CellStyle应用于具体单元格 

                    EightRowHSSF.CreateCell(13).SetCellValue("12月");
                    EightRowHSSF.GetCell(13).CellStyle = style2;//将CellStyle应用于具体单元格 

                    EightRowHSSF.CreateCell(14).SetCellValue("合计");
                    EightRowHSSF.GetCell(14).CellStyle = style2;//将CellStyle应用于具体单元格 
                    nextRow++;
                    //下一行
                    IRow NineRowHSSF = mysheetHSSF.CreateRow(nextRow);
                    NineRowHSSF.CreateCell(0).SetCellValue("1)销售额(万元）");
                    NineRowHSSF.GetCell(0).CellStyle = style4;//将CellStyle应用于具体单元格 
                    #endregion
                    nextRow++;
                }
                #endregion
                #endregion


                string rootPath = _webHostEnvironment.ContentRootPath;
                string strFunctionName = "FS0627_Export";
                string strFileName = strFunctionName + "_导出信息_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + loginInfo.UserId + ".xlsx";
                string fileSavePath = rootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Export" + Path.DirectorySeparatorChar;//文件临时目录，导入完成后 删除
                filepath = fileSavePath+strFileName;

                using (FileStream file = new FileStream(filepath, FileMode.Create))
                {
                    hssfworkbook.Write(file);//向打开的这个xls文件中写入数据  
                    file.Close();
                }
                #endregion
                #region delete
                //输出的文件名称
                //string fileName = "故障码信息" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-ffff") + ".xls";
                ////把Excel转为流，输出
                ////创建文件流
                //System.IO.MemoryStream bookStream = new System.IO.MemoryStream();
                ////将工作薄写入文件流
                //excelBook.Write(bookStream);

                ////输出之前调用Seek（偏移量，游标位置) 把0位置指定为开始位置
                //bookStream.Seek(0, System.IO.SeekOrigin.Begin);
                ////Stream对象,文件类型,文件名称
                //return File(bookStream, "application/vnd.ms-excel", fileName);
                #endregion

                if (strFileName == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "导出生成文件失败";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = strFileName;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2704", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出报表失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            } 
          
        }
        #endregion
    }
}

