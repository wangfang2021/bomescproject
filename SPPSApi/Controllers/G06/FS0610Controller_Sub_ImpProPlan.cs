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

namespace SPPSApi.Controllers.G06
{
    [Route("api/FS0610_Sub_ImpProPlan/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0610Controller_Sub_ImpProPlan : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0610_Logic fs0610_Logic = new FS0610_Logic();
        private readonly string FunctionID = "FS0610";

        public FS0610Controller_Sub_ImpProPlan(IWebHostEnvironment webHostEnvironment)
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
            string vcMon = DateTime.Now.AddMonths(1).ToString("yyyyMM");
            string[] vcFZGC = dataForm.vcFZGC.ToString().Replace("\r\n", "").Replace("\"", "").Replace("[", "").Replace("]", "").Replace(" ", "").Split(',');
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
                string[,] headers = new string[,] {{"对象月","工厂","品番","受入","车型","紧急区分","工程1","工程2","工程3","工程4",
                            "品名（中）","工程号","工程名","号旧","月度总量",
                            "1日白","1日夜","2日白","2日夜","3日白","3日夜","4日白","4日夜","5日白","5日夜","6日白","6日夜","7日白","7日夜","8日白","8日夜","9日白","9日夜","10日白","10日夜",
                            "11日白","11日夜","12日白","12日夜","13日白","13日夜","14日白","14日夜","15日白","15日夜","16日白","16日夜","17日白","17日夜","18日白","18日夜","19日白","19日夜","20日白","20日夜",
                            "21日白","21日夜","22日白","22日夜","23日白","23日夜","24日白","24日夜","25日白","25日夜","26日白","26日夜","27日白","27日夜","28日白","28日夜","29日白","29日夜","30日白","30日夜","31日白","31日夜",
                            "1日白 ","1日夜 ","2日白 ","2日夜 ","3日白 ","3日夜 ","4日白 ","4日夜 ","5日白 ","5日夜 ","6日白 ","6日夜 ","7日白 ","7日夜 ","8日白 ","8日夜 ","9日白 ","9日夜 ","10日白 ","10日夜 ",
                            "11日白 ","11日夜 ","12日白 ","12日夜 ","13日白 ","13日夜 ","14日白 ","14日夜 ","15日白 ","15日夜 ","16日白 ","16日夜 ","17日白 ","17日夜 ","18日白 ","18日夜 ","19日白 ","19日夜 ","20日白 ","20日夜 ",
                            "21日白 ","21日夜 ","22日白 ","22日夜 ","23日白 ","23日夜 ","24日白 ","24日夜 ","25日白 ","25日夜 ","26日白 ","26日夜 ","27日白 ","27日夜 ","28日白 ","28日夜 ","29日白 ","29日夜 ","30日白 ","30日夜 ","31日白 ","31日夜 "},
                           {"vcMonth","vcPlant","vcPartsno","vcDock","vcCarType","vcEDflag","vcCalendar1","vcCalendar2","vcCalendar3","vcCalendar4",
                            "vcPartsNameCHN","vcProject1","vcProjectName","vcCurrentPastCode","vcMonTotal",
                            "TD1b","TD1y","TD2b","TD2y","TD3b","TD3y","TD4b","TD4y","TD5b","TD5y","TD6b","TD6y","TD7b","TD7y","TD8b","TD8y","TD9b","TD9y","TD10b","TD10y",
                            "TD11b","TD11y","TD12b","TD12y","TD13b","TD13y","TD14b","TD14y","TD15b","TD15y","TD16b","TD16y","TD17b","TD17y","TD18b","TD18y","TD19b","TD19y","TD20b","TD20y",
                            "TD21b","TD21y","TD22b","TD22y","TD23b","TD23y","TD24b","TD24y","TD25b","TD25y","TD26b","TD26y","TD27b","TD27y","TD28b","TD28y","TD29b","TD29y","TD30b","TD30y","TD31b","TD31y",
                            "ED1b","ED1y","ED2b","ED2y","ED3b","ED3y","ED4b","ED4y","ED5b","ED5y","ED6b","ED6y","ED7b","ED7y","ED8b","ED8y","ED9b","ED9y","ED10b","ED10y",
                            "ED11b","ED11y","ED12b","ED12y","ED13b","ED13y","ED14b","ED14y","ED15b","ED15y","ED16b","ED16y","ED17b","ED17y","ED18b","ED18y","ED19b","ED19y","ED20b","ED20y",
                            "ED21b","ED21y","ED22b","ED22y","ED23b","ED23y","ED24b","ED24y","ED25b","ED25y","ED26b","ED26y","ED27b","ED27y","ED28b","ED28y","ED29b","ED29y","ED30b","ED30y","ED31b","ED31y"
                            },
                            {"","","","","","","","","","","","","","","",
                             FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,
                             FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,
                             FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,
                             FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,
                             FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,
                             FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,
                             FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,
                             FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,
                             FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,
                             FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,
                             FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,
                             FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num
                             },
                            {"7","2","14","4","4","10","20","20","20","20","500","20","7","2","10",
                             "10","10","10","10","10","10","10","10","10","10",
                             "10","10","10","10","10","10","10","10","10","10",
                             "10","10","10","10","10","10","10","10","10","10",
                             "10","10","10","10","10","10","10","10","10","10",
                             "10","10","10","10","10","10","10","10","10","10",
                             "10","10","10","10","10","10","10","10","10","10","10","10",
                              "10","10","10","10","10","10","10","10","10","10",
                             "10","10","10","10","10","10","10","10","10","10",
                             "10","10","10","10","10","10","10","10","10","10",
                             "10","10","10","10","10","10","10","10","10","10",
                             "10","10","10","10","10","10","10","10","10","10",
                             "10","10","10","10","10","10","10","10","10","10","10","10"
                            },//最大长度设定,不校验最大长度用0
                            {"7","2","0","0","0","0","0","0","0","0","0","0","0","0","0",
                             "0","0","0","0","0","0","0","0","0","0",
                             "0","0","0","0","0","0","0","0","0","0",
                             "0","0","0","0","0","0","0","0","0","0",
                             "0","0","0","0","0","0","0","0","0","0",
                             "0","0","0","0","0","0","0","0","0","0",
                             "0","0","0","0","0","0","0","0","0","0","0","0",
                             "0","0","0","0","0","0","0","0","0","0",
                             "0","0","0","0","0","0","0","0","0","0",
                             "0","0","0","0","0","0","0","0","0","0",
                             "0","0","0","0","0","0","0","0","0","0",
                             "0","0","0","0","0","0","0","0","0","0",
                             "0","0","0","0","0","0","0","0","0","0","0","0"
                            },//最小长度设定,可以为空用0
                            {"1","2","3","4","5","6","7","8","9","10","11","12","13","14","15",
                            "16","17","18","19","20","21","22","23","24","25",
                            "26","27","28","29","30","31","32","33","34","35",
                            "36","37","38","39","40","41","42","43","44","45",
                            "46","47","48","49","50","51","52","53","54","55",
                            "56","57","58","59","60","61","62","63","64","65",
                            "66","67","68","69","70","71","72","73","74","75","76","77",
                            "78","79","80","81","82","83","84","85","86","87",
                            "88","89","90","91","92","93","94","95","96","97",
                            "98","99","100","101","102","103","104","105","106","107",
                            "108","109","110","111","112","113","114","115","116","117",
                            "118","119","120","121","122","123","124","125","126","127",
                            "128","129","130","131","132","133","134","135","136","137","138","139"
                            }//前台显示列号，从0开始计算,注意有选择框的是0
                };
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
                             group r by new { r2 = r.Field<string>("vcMonth"), r3 = r.Field<string>("vcPlant"), r4 = r.Field<string>("vcPartsno"), r5 = r.Field<string>("vcDock") } into g
                             where g.Count() > 1
                             select g;
                if (result.Count() > 0)
                {
                    StringBuilder sbr = new StringBuilder();
                    sbr.Append("导入数据重复:<br/>");
                    foreach (var item in result)
                    {
                        sbr.Append("对象月:" + item.Key.r2 + "工厂:" + item.Key.r2 + "品番:" + item.Key.r2 + "受入:" + item.Key.r2 + "<br/>");
                    }
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = sbr.ToString();
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fs0610_Logic.TranColName(ref importDt);
                fs0610_Logic.PartsNoFomatTo12(ref importDt);//2013-4-18 10位品番后两位加00
                for (int i = 0; i < vcFZGC.Length; i++)
                {
                    strMsg = fs0610_Logic.checkExcelData_Pro(ref importDt, vcMon, vcFZGC[i]);//校验数据
                    if (strMsg.Length > 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = strMsg;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                Exception ex = new Exception();
                for (int i = 0; i < vcFZGC.Length; i++)
                {
                    strMsg = fs0610_Logic.updatePro(importDt, loginInfo.UserId, vcMon, ref ex, vcFZGC[i]);
                    if (strMsg.Length > 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = strMsg;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "上传生产计划成功";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE1102", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败" + ex.Message;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}
