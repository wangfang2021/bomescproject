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
using NPOI.XSSF.UserModel;

namespace SPPSApi.Controllers.G04
{
    [Route("api/FS0402_Sub_Import/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0402Controller_Sub_Import : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0402_Logic fs0402_Logic = new FS0402_Logic();
        private readonly string FunctionID = "FS0402";

        public FS0402Controller_Sub_Import(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }


        #region 导入之后点保存
        [HttpPost]
        [EnableCors("any")]
        public string importSaveApi([FromBody] dynamic data)
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

            string strYearMonth = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth).ToString("yyyyMM");
            string strYearMonth_2 = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth).AddMonths(1).ToString("yyyyMM");
            string strYearMonth_3 = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth).AddMonths(2).ToString("yyyyMM");


            string strMonth = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth).ToString("MM");
            string strMonth_2 = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth).AddMonths(1).ToString("MM");
            string strMonth_3 = dataForm.YearMonth == null ? "" : Convert.ToDateTime(dataForm.YearMonth).AddMonths(2).ToString("MM");


            JArray fileNameList = dataForm.fileNameList;
            string hashCode = dataForm.hashCode;
            string fileSavePath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "upload" + Path.DirectorySeparatorChar + hashCode + Path.DirectorySeparatorChar;
            try
            {
                if (!Directory.Exists(fileSavePath))
                {
                    ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "没有要导入的文件，请重新上传！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                DirectoryInfo theFolder = new DirectoryInfo(fileSavePath);
                string[,] headers = new string[,] {{"品番", "", "", ""},
                                                {"vcPart_id", "iCbSOQN", "iCbSOQN1", "iCbSOQN2"},
                                                {FieldCheck.NumCharLLL,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num},
                                                {"12","0","0","0"},//最大长度设定,不校验最大长度用0
                                                {"1","1","1","1"}};//最小长度设定,可以为空用0

                //用于存储错误信息的表
                //DataTable errorMessage = new DataTable();
                //errorMessage.Columns.Add("错误类型");
                //errorMessage.Columns.Add("品番");
                //errorMessage.Columns.Add("年月");
                //errorMessage.Columns.Add("错误信息");


                ////检验文件中的对象年月是否与在页面上选择的对象年月一致+获取列
                //foreach (FileInfo info in theFolder.GetFiles())
                //{
                //    using (FileStream fs = new FileStream(info.FullName, FileMode.Open, FileAccess.Read))
                //    {
                //        IWorkbook workbook = null;

                //        if (info.FullName.IndexOf(".xlsx") > 0 || info.FullName.IndexOf(".xlsm") > 0) // 2007版本
                //            workbook = new XSSFWorkbook(fs);
                //        else if (info.FullName.IndexOf(".xls") > 0) // 2003版本
                //            workbook = new HSSFWorkbook(fs);

                //        IRow firstRow = workbook.GetSheetAt(0).GetRow(0);
                //        ICell varDxnyCell = firstRow.GetCell(1);
                //        string varDxny_File = varDxnyCell.StringCellValue;

                //        //若文件中的对象年月与页面输入的不相符，则报错
                //        if (varDxny_File != strYearMonth) {
                //            errorMessage.Rows.Add("文件错误","","", "文件中的对象年月与页面输入的对象年月不相符！");

                //            //生成错误文件
                //            string generateError = "";
                //            string[] errorHeader = { "错误类型", "品番", "年月", "错误信息" };
                //            string path=ComFunction.DataTableToExcel(errorHeader, errorHeader, errorMessage, _webHostEnvironment.ContentRootPath,"SOQ导入错误信息", loginInfo.UserId, FunctionID, ref generateError);

                //            //在SOQ导入履历表中新增错误信息数据
                //            fs0402_Logic.importHistory(varDxny, info.Name,1, path, loginInfo.UserId);

                //            apiResult.code = ComConstant.ERROR_CODE;
                //            apiResult.data = "文件中的对象年月与页面输入的对象年月不相符！";
                //            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                //        }

                //        //获取列
                //        headers[0, 1] = firstRow.GetCell(1).StringCellValue;
                //        headers[0, 2] = firstRow.GetCell(2).StringCellValue;
                //        headers[0, 3] = firstRow.GetCell(3).StringCellValue;
                //    }
                //}
                List<string> errMessageList = new List<string>();//记录导入错误消息

                string strMsg = "";
                DataTable importDt = new DataTable();
                foreach (FileInfo info in theFolder.GetFiles())
                {
                    DataTable dt = fs0402_Logic.ExcelToDataTable(info.FullName, "sheet1", headers, ref strMsg);
                    if (strMsg != "")
                    {
                        ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "导入终止，文件" + info.Name + ":" + strMsg;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (importDt.Columns.Count == 0)
                        importDt = dt.Clone();
                    if (dt.Rows.Count == 0|| dt.Rows.Count == 1)
                    {
                        ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "导入终止，文件" + info.Name + "没有要导入的数据";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    //验证对象月是否是选择的月份
                    string strMonth_import = dt.Rows[0][1] == System.DBNull.Value ? "0" : dt.Rows[0][1].ToString().Replace("月","");
                    string strMonth_import_2 = dt.Rows[0][2] == System.DBNull.Value ? "0" : dt.Rows[0][2].ToString().Replace("月", "");
                    string strMonth_import_3 = dt.Rows[0][3] == System.DBNull.Value ? "0" : dt.Rows[0][3].ToString().Replace("月", "");

 
                    int iMonth = Convert.ToInt32(strMonth);
                    int iMonth_2 = Convert.ToInt32(strMonth_2);
                    int iMonth_3 = Convert.ToInt32(strMonth_3);


                    if (iMonth != Convert.ToInt32(strMonth_import))
                    {
                        errMessageList.Add("文件中的对象月" + strMonth_import + "与页面输入的对象年月" + iMonth + "不相符！");
                    }
                    if (iMonth_2 != Convert.ToInt32(strMonth_import_2))
                    {
                        errMessageList.Add("文件中的内示月" + strMonth_import_2 + "与页面输入的对象年月" + iMonth_2 + "不相符！");
                    }
                    if (iMonth_3 != Convert.ToInt32(strMonth_import_3))
                    {
                        errMessageList.Add("文件中的内内示月" + strMonth_import_3 + "与页面输入的对象年月" + iMonth_3 + "不相符！");
                    }
                    for ( int i=1;i< dt.Rows.Count;i++)//跳过列头
                    {
                        DataRow row = dt.Rows[i];
                        importDt.ImportRow(row);
                    }
                }
                ComFunction.DeleteFolder(fileSavePath);//读取数据后删除文件夹
 

                List<string> errMonthList = new List<string>();//记录年月错误的品番
                //check:品番行数是否唯一
                var result = from r in importDt.AsEnumerable()
                             group r by new { r2 = r.Field<string>("vcPart_id") } into g
                             where g.Count() > 1
                             select g;
                if (result.Count() > 0)
                {
                    StringBuilder sbr = new StringBuilder();
                    sbr.Append("导入数据重复:<br/>");
                    foreach (var item in result)
                    {
                        sbr.Append("品番:" + item.Key.r2 + "<br/>");
                    }
                    errMessageList.Add(sbr.ToString());
                }
                //check:三个月品番数量不能同时为0


                //   是否为TFTM品番（包装工厂）                           SP_M_SITEM    是否有改品番
                //    N、N + 1、N + 2月品番有效性                         SP_M_SITEM    TIMEFROM  TIMETO   ，品番在时间区间内有数据
                //    是否有价格，且在有效期内                            TPrice  dUseBegin    dUseEnd ，品番在时间区间内有数据
                //    手配中是否有受入、收容数、发注工厂
                //    收容数整倍数                                        SP_M_SITEM QUANTITYPERCONTAINER
                //   if一括生产 校验： 对象月 > 实施年月时间 不能订货
                //  如果是强制订货，没有价格也可以定。





                if (errMessageList.Count > 0)
                {
                    fs0402_Logic.importHistory(strYearMonth, errMessageList);
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "发现问题数据，导入终止，请查看导入履历。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }


                fs0402_Logic.importSave(importDt, loginInfo.UserId, strYearMonth);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "保存成功";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M04UE0204", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败" + ex.Message;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion



    }
}
