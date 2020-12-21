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

            string varDxny = dataForm.varDxny == null ? "" : Convert.ToDateTime(dataForm.varDxny).ToString("yyyy/MM");
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
                                                {"PARTSNO", "iCbSOQN", "iCbSOQN1", "iCbSOQN2"},
                                                {FieldCheck.NumCharLLL,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num},
                                                {"0","0","0","0"},//最大长度设定,不校验最大长度用0
                                                {"0","0","0","0"}};//最小长度设定,可以为空用0

                //用于存储错误信息的表
                DataTable errorMessage = new DataTable();
                errorMessage.Columns.Add("错误类型");
                errorMessage.Columns.Add("品番");
                errorMessage.Columns.Add("年月");
                errorMessage.Columns.Add("错误信息");


                //检验文件中的对象年月是否与在页面上选择的对象年月一致+获取列
                foreach (FileInfo info in theFolder.GetFiles())
                {
                    using (FileStream fs = new FileStream(info.FullName, FileMode.Open, FileAccess.Read))
                    {
                        IWorkbook workbook = null;

                        if (info.FullName.IndexOf(".xlsx") > 0 || info.FullName.IndexOf(".xlsm") > 0) // 2007版本
                            workbook = new XSSFWorkbook(fs);
                        else if (info.FullName.IndexOf(".xls") > 0) // 2003版本
                            workbook = new HSSFWorkbook(fs);

                        IRow firstRow = workbook.GetSheetAt(0).GetRow(0);
                        ICell varDxnyCell = firstRow.GetCell(1);
                        string varDxny_File = varDxnyCell.StringCellValue;

                        //若文件中的对象年月与页面输入的不相符，则报错
                        if (varDxny_File != varDxny) {
                            errorMessage.Rows.Add("文件错误","","", "文件中的对象年月与页面输入的对象年月不相符！");

                            //生成错误文件
                            string generateError = "";
                            string[] errorHeader = { "错误类型", "品番", "年月", "错误信息" };
                            string path=ComFunction.DataTableToExcel(errorHeader, errorHeader, errorMessage, _webHostEnvironment.ContentRootPath,"SOQ导入错误信息", loginInfo.UserId, FunctionID, ref generateError);

                            //在SOQ导入履历表中新增错误信息数据
                            fs0402_Logic.importHistory(varDxny, info.Name,1, path, loginInfo.UserId);

                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "文件中的对象年月与页面输入的对象年月不相符！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }

                        //获取列
                        headers[0, 1] = firstRow.GetCell(1).StringCellValue;
                        headers[0, 2] = firstRow.GetCell(2).StringCellValue;
                        headers[0, 3] = firstRow.GetCell(3).StringCellValue;
                    }
                }

                string strMsg = "";
                DataTable importDt = new DataTable();
                foreach (FileInfo info in theFolder.GetFiles())
                {
                    DataTable dt = ComFunction.ExcelToDataTable(info.FullName, "sheet1", headers, ref strMsg);
                    if (strMsg != "")
                    {
                        ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "导入终止，文件" + info.Name + ":" + strMsg;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (importDt.Columns.Count == 0)
                        importDt = dt.Clone();
                    if (dt.Rows.Count == 0)
                    {
                        ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "导入终止，文件" + info.Name + "没有要导入的数据";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    foreach (DataRow row in dt.Rows)
                    {
                        importDt.ImportRow(row);
                    }
                }
                ComFunction.DeleteFolder(fileSavePath);//读取数据后删除文件夹


                var result = from r in importDt.AsEnumerable()
                             group r by new { r2 = r.Field<string>("PARTSNO") } into g
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
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = sbr.ToString();
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }




                fs0402_Logic.importSave(importDt, loginInfo.UserId, varDxny);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "保存成功";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0905", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败" + ex.Message;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion



    }
}
