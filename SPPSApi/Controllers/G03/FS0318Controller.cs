using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace SPPSApi.Controllers.G03
{
    [Route("api/FS0318/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0318Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string FunctionID = "FS0318";
        FS0318_Logic fs0318_logic = new FS0318_Logic();

        public FS0318Controller(IWebHostEnvironment webHostEnvironment)
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
                //TODO 选择车型TCode
                List<Object> carType = ComFunction.convertAllToResult(fs0318_logic.getcarType());//车型
                res.Add("carType", carType);

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
           

            string carType = dataForm.carType;

            try
            {
                DataTable dt = fs0318_logic.search(carType);
                DtConverter dtConverter = new DtConverter();
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE1801", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }

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
            //以下开始业务处理
            ApiResult apiResult = new ApiResult();
            dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));

            string carType = dataForm.carType;


            try
            {
                DataTable dt = fs0318_logic.search(carType);

                string filepath = "";
                string strMsg = "";

                filepath = generateExcelWithXlt(dt, _webHostEnvironment.ContentRootPath, "FS0318.xlsx", carType);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE1702", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        public static string generateExcelWithXlt(DataTable dt, string rootPath, string xltName, string carType)
        {
            try
            {
                XSSFWorkbook xssfworkbook = new XSSFWorkbook();

                string XltPath = rootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Template" + Path.DirectorySeparatorChar + xltName;
                using (FileStream fs = System.IO.File.OpenRead(XltPath))
                {
                    xssfworkbook = new XSSFWorkbook(fs);
                    fs.Close();
                }

                ISheet sheet = xssfworkbook.GetSheetAt(0);
                if (dt.Rows.Count == 0)
                    return "";

                DataRow row = dt.Rows[0];

                sheet.GetRow(1).GetCell(0).SetCellValue(row["total"].ToString());
                sheet.GetRow(1).GetCell(1).SetCellValue(row["outSum"].ToString());
                sheet.GetRow(1).GetCell(2).SetCellValue(row["inSum"].ToString());
                sheet.GetRow(4).GetCell(1).SetCellValue(row["outSQ"].ToString());
                sheet.GetRow(4).GetCell(2).SetCellValue(row["outSQPer"].ToString());
                sheet.GetRow(4).GetCell(3).SetCellValue(row["inSQ"].ToString());
                sheet.GetRow(4).GetCell(4).SetCellValue(row["inSQPer"].ToString());
                sheet.GetRow(5).GetCell(1).SetCellValue(row["outPrice"].ToString());
                sheet.GetRow(5).GetCell(2).SetCellValue(row["outPricePer"].ToString());
                sheet.GetRow(5).GetCell(3).SetCellValue(row["inPrice"].ToString());
                sheet.GetRow(5).GetCell(4).SetCellValue(row["inPricePer"].ToString());
                sheet.GetRow(6).GetCell(1).SetCellValue(row["outHezi"].ToString());
                sheet.GetRow(6).GetCell(2).SetCellValue(row["outHeziPer"].ToString());
                sheet.GetRow(6).GetCell(3).SetCellValue(row["inHezi"].ToString());
                sheet.GetRow(6).GetCell(4).SetCellValue(row["inHeziPer"].ToString());
                sheet.GetRow(7).GetCell(1).SetCellValue(row["outPack"].ToString());
                sheet.GetRow(7).GetCell(2).SetCellValue(row["outPackPer"].ToString());
                sheet.GetRow(7).GetCell(3).SetCellValue(row["inPack"].ToString());
                sheet.GetRow(7).GetCell(4).SetCellValue(row["inPackPer"].ToString());


                string strFileName = carType + "_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + "_新车进度报表.xlsx";
                string fileSavePath = rootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Export" + Path.DirectorySeparatorChar;//文件临时目录，导入完成后 删除
                string path = fileSavePath + strFileName;
                using (FileStream fs = System.IO.File.OpenWrite(path))
                {
                    xssfworkbook.Write(fs);//向打开的这个xls文件中写入数据  
                    fs.Close();
                }
                return strFileName;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}