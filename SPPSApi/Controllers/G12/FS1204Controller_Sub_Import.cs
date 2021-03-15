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
    [Route("api/FS1204_Sub_Import/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1204Controller_Sub_Import : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        FS1204_Logic fs1204_Logic = new FS1204_Logic();
        private readonly string FunctionID = "FS1204";

        public FS1204Controller_Sub_Import(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        #region 导入之后点保存
        [HttpPost]
        [EnableCors("any")]
        public string importSaveApi([FromBody]dynamic data)
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
            string vcMon = dataForm.vcMon;
            vcMon = vcMon == null ? "" : vcMon;
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
                string strMsg = "";
                string[,] headers = new string[,] {{"类别", "工厂", "生产部署", "组别",
                        "1日（白）", "1日（夜）","2日（白）", "2日（夜）","3日（白）","3日（夜）","4日（白）", "4日（夜）","5日（白）", "5日（夜）",  "6日（白）",  "6日（夜）",
                        "7日（白）", "7日（夜）","8日（白）", "8日（夜）","9日（白）","9日（夜）","10日（白）","10日（夜）","11日（白）", "11日（夜）","12日（白）", "12日（夜）",
                        "13日（白）", "13日（夜）","14日（白）", "14日（夜）", "15日（白）", "15日（夜）","16日（白）", "16日（夜）","17日（白）", "17日（夜）","18日（白）", "18日（夜）",
                        "19日（白）", "19日（夜）","20日（白）", "20日（夜）","21日（白）", "21日（夜）","22日（白）", "22日（夜）","23日（白）", "23日（夜）","24日（白）", "24日（夜）",
                        "25日（白）", "25日（夜）","26日（白）", "26日（夜）","27日（白）", "27日（夜）","28日（白）", "28日（夜）","29日（白）", "29日（夜）", "30日（白）", "30日（夜）",
                        "31日（白）", "31日（夜）" },
                        { "vcPartType", "vcPlant", "vcGC", "vcZB",
                          "D1b", "D1y", "D2b", "D2y", "D3b", "D3y", "D4b", "D4y", "D5b", "D5y", "D6b", "D6y",
                          "D7b", "D7y", "D8b", "D8y", "D9b", "D9y", "D10b", "D10y", "D11b", "D11y", "D12b", "D12y",
                          "D13b", "D13y", "D14b", "D14y", "D15b", "D15y", "D16b", "D16y", "D17b", "D17y", "D18b", "D18y",
                          "D19b", "D19y", "D20b", "D20y", "D21b", "D21y", "D22b", "D22y", "D23b", "D23y", "D24b", "D24y",
                          "D25b", "D25y", "D26b", "D26y", "D27b", "D27y", "D28b", "D28y", "D29b", "D29y", "D30b", "D30y",
                          "D31b", "D31y"},
                        { "", "", "", "",
                         "","","","","","","","","","","","",
                         "","","","","","","","","","","","",
                         "","","","","","","","","","","","",
                         "","","","","","","","","","","","",
                         "","","","","","","","","","","","",
                         "","" },
                        { "0","2","20","3", 
                          "2","2","2","2","2","2","2","2","2","2","2","2",
                          "2","2","2","2","2","2","2","2","2","2","2","2",
                          "2","2","2","2","2","2","2","2","2","2","2","2",
                          "2","2","2","2","2","2","2","2","2","2","2","2",
                          "2","2","2","2","2","2","2","2","2","2","2","2",
                          "2","2"},//最大长度设定,不校验最大长度用0
                        { "1","0","0","0", 
                          "0","0","0","0","0","0","0","0","0","0","0","0",
                          "0","0","0","0","0","0","0","0","0","0","0","0",
                          "0","0","0","0","0","0","0","0","0","0","0","0",
                          "0","0","0","0","0","0","0","0","0","0","0","0",
                          "0","0","0","0","0","0","0","0","0","0","0","0",
                          "0","0"}//最小长度设定,可以为空用0
                };
                DataTable importDt = new DataTable();
                foreach (FileInfo info in theFolder.GetFiles())
                {
                    DataTable dt = ComFunction.ExcelToDataTable(info.FullName, "sheet1", headers, 2, ref strMsg);
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


                //var result = from r in importDt.AsEnumerable()
                //             group r by new { r2 = r.Field<string>("vcPart_id"), r3 = r.Field<string>("dUseBegin"), r4 = r.Field<string>("dUseEnd") } into g
                //             where g.Count() > 1
                //             select g;
                //if (result.Count() > 0)
                //{
                //    StringBuilder sbr = new StringBuilder();
                //    sbr.Append("导入数据重复:<br/>");
                //    foreach (var item in result)
                //    {
                //        sbr.Append("品番:" + item.Key.r2 + " 使用开始:" + item.Key.r3 + " 使用结束:" + item.Key.r4 + "<br/>");
                //    }
                //    apiResult.code = ComConstant.ERROR_CODE;
                //    apiResult.data = sbr.ToString();
                //    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                //}

                fs1204_Logic.UpdateTable(importDt, vcMon);//将导入的表格数据上传
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
