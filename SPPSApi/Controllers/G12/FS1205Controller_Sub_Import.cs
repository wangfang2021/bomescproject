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
    [Route("api/FS1205_Sub_Import/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1205Controller_Sub_Import : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        FS1205_Logic fs1205_Logic = new FS1205_Logic();
        private readonly string FunctionID = "FS1205";
        public FS1205Controller_Sub_Import(IWebHostEnvironment webHostEnvironment)
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
                string[,] headers = new string[,] {{"对象月", "对象周", "工场", "工程", "组别", "品番", "收容数",
                        "1日白", "1日夜","2日白", "2日夜","3日白","3日夜","4日白", "4日夜","5日白", "5日夜",  "6日白",  "6日夜",
                        "7日白", "7日夜","8日白", "8日夜","9日白","9日夜","10日白","10日夜","11日白", "11日夜","12日白", "12日夜",
                        "13日白", "13日夜","14日白", "14日夜", "15日白", "15日夜","16日白", "16日夜","17日白", "17日夜","18日白", "18日夜",
                        "19日白", "19日夜","20日白", "20日夜","21日白", "21日夜","22日白", "22日夜","23日白", "23日夜","24日白", "24日夜",
                        "25日白", "25日夜","26日白", "26日夜","27日白", "27日夜","28日白", "28日夜","29日白", "29日夜", "30日白", "30日夜",
                        "31日白", "31日夜","周总量(内示)",
                        "1日白", "1日夜","2日白", "2日夜","3日白","3日夜","4日白", "4日夜","5日白", "5日夜",  "6日白",  "6日夜",
                        "7日白", "7日夜","8日白", "8日夜","9日白","9日夜","10日白","10日夜","11日白", "11日夜","12日白", "12日夜",
                        "13日白", "13日夜","14日白", "14日夜", "15日白", "15日夜","16日白", "16日夜","17日白", "17日夜","18日白", "18日夜",
                        "19日白", "19日夜","20日白", "20日夜","21日白", "21日夜","22日白", "22日夜","23日白", "23日夜","24日白", "24日夜",
                        "25日白", "25日夜","26日白", "26日夜","27日白", "27日夜","28日白", "28日夜","29日白", "29日夜", "30日白", "30日夜",
                        "31日白", "31日夜","周总量(平准)"
                        },
                        { "vcMonth", "vcWeek", "vcPlant", "vcGC", "vcZB", "vcPartsno", "vcQuantityPerContainer",
                        "vcD1b", "vcD1y", "vcD2b", "vcD2y", "vcD3b", "vcD3y", "vcD4b", "vcD4y", "vcD5b", "vcD5y", "vcD6b", "vcD6y",
                        "vcD7b", "vcD7y", "vcD8b", "vcD8y", "vcD9b", "vcD9y", "vcD10b", "vcD10y", "vcD11b", "vcD11y", "vcD12b", "vcD12y",
                        "vcD13b", "vcD13y", "vcD14b", "vcD14y", "vcD15b", "vcD15y", "vcD16b", "vcD16y", "vcD17b", "vcD17y", "vcD18b", "vcD18y",
                        "vcD19b", "vcD19y", "vcD20b", "vcD20y", "vcD21b", "vcD21y", "vcD22b", "vcD22y", "vcD23b", "vcD23y", "vcD24b", "vcD24y",
                        "vcD25b", "vcD25y", "vcD26b", "vcD26y", "vcD27b", "vcD27y", "vcD28b", "vcD28y", "vcD29b", "vcD29y", "vcD30b", "vcD30y",
                        "vcD31b", "vcD31y", "vcWeekTotal",
                        "vcLevelD1b", "vcLevelD1y", "vcLevelD2b", "vcLevelD2y", "vcLevelD3b", "vcLevelD3y", "vcLevelD4b", "vcLevelD4y", "vcLevelD5b", "vcLevelD5y", "vcLevelD6b", "vcLevelD6y", 
                        "vcLevelD7b", "vcLevelD7y", "vcLevelD8b", "vcLevelD8y", "vcLevelD9b", "vcLevelD9y", "vcLevelD10b", "vcLevelD10y", "vcLevelD11b", "vcLevelD11y", "vcLevelD12b", "vcLevelD12y", 
                        "vcLevelD13b", "vcLevelD13y", "vcLevelD14b", "vcLevelD14y", "vcLevelD15b", "vcLevelD15y", "vcLevelD16b", "vcLevelD16y", "vcLevelD17b", "vcLevelD17y", "vcLevelD18b", "vcLevelD18y", 
                        "vcLevelD19b", "vcLevelD19y", "vcLevelD20b", "vcLevelD20y", "vcLevelD21b", "vcLevelD21y", "vcLevelD22b", "vcLevelD22y", "vcLevelD23b", "vcLevelD23y", "vcLevelD24b", "vcLevelD24y", 
                        "vcLevelD25b", "vcLevelD25y", "vcLevelD26b", "vcLevelD26y", "vcLevelD27b", "vcLevelD27y", "vcLevelD28b", "vcLevelD28y", "vcLevelD29b", "vcLevelD29y", "vcLevelD30b", "vcLevelD30y", 
                        "vcLevelD31b", "vcLevelD31y", "vcLevelWeekTotal" },
                        { FieldCheck.FNum, FieldCheck.Num, FieldCheck.Num, FieldCheck.NumCharLLL, FieldCheck.NumCharLLL, FieldCheck.NumChar, FieldCheck.Num,
                         FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,
                         FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,
                         FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,
                         FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,
                         FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,
                         FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,
                         FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,
                         FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,
                         FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,
                         FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,
                         FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,
                         FieldCheck.Num,FieldCheck.Num,FieldCheck.Num
                        },
                        { "7","1","2","20","3","12","8",
                          "8","8","8","8","8","8","8","8","8","8","8","8",
                          "8","8","8","8","8","8","8","8","8","8","8","8",
                          "8","8","8","8","8","8","8","8","8","8","8","8",
                          "8","8","8","8","8","8","8","8","8","8","8","8",
                          "8","8","8","8","8","8","8","8","8","8","8","8",
                          "8","8","10",
                          "8","8","8","8","8","8","8","8","8","8","8","8",
                          "8","8","8","8","8","8","8","8","8","8","8","8",
                          "8","8","8","8","8","8","8","8","8","8","8","8",
                          "8","8","8","8","8","8","8","8","8","8","8","8",
                          "8","8","8","8","8","8","8","8","8","8","8","8",
                          "8","8","10"
                        },//最大长度设定,不校验最大长度用0
                        { "7","1","1","1","1","12","0",
                          "0","0","0","0","0","0","0","0","0","0","0","0",
                          "0","0","0","0","0","0","0","0","0","0","0","0",
                          "0","0","0","0","0","0","0","0","0","0","0","0",
                          "0","0","0","0","0","0","0","0","0","0","0","0",
                          "0","0","0","0","0","0","0","0","0","0","0","0",
                          "0","0","0",
                           "0","0","0","0","0","0","0","0","0","0","0","0",
                          "0","0","0","0","0","0","0","0","0","0","0","0",
                          "0","0","0","0","0","0","0","0","0","0","0","0",
                          "0","0","0","0","0","0","0","0","0","0","0","0",
                          "0","0","0","0","0","0","0","0","0","0","0","0",
                          "0","0","0"
                        }//最小长度设定,可以为空用0
                };
                DataTable importDt = new DataTable();
                foreach (FileInfo info in theFolder.GetFiles())
                {
                    DataTable dt = ComFunction.ImportExcel(info.FullName, "sheet1", headers, ref strMsg);
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
                    string msg = fs1205_Logic.TXTCheckDataSchedule(importDt);
                    if (msg.Length > 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = msg;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    else
                    {
                        string Month = importDt.Rows[0]["vcMonth"].ToString();//获取数据源中的对象月
                        string Week = importDt.Rows[0]["vcWeek"].ToString();  //获取数据源中的对象周
                        string Plant = importDt.Rows[0]["vcPlant"].ToString();//获取数据源中的工厂
                        importDt.PrimaryKey = new DataColumn[]
                        {
                            importDt.Columns["vcMonth"],
                            importDt.Columns["vcWeek"],
                            importDt.Columns["vcPlant"],
                            importDt.Columns["vcGC"],
                            importDt.Columns["vcZB"],
                            importDt.Columns["vcPartsno"]
                        };
                        fs1205_Logic.TXTUpdateTableSchedule(importDt, Month, Week, Plant);
                    }
                }
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
