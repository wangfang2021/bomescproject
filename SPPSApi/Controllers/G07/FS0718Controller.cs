using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using Common;
using ICSharpCode.SharpZipLib.Zip;
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

namespace SPPSApi.Controllers.G07
{
    [Route("api/FS0718/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0718Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0718_Logic FS0718_Logic = new FS0718_Logic();
        private readonly string FunctionID = "FS0718";

        public FS0718Controller(IWebHostEnvironment webHostEnvironment)
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
                DataTable DToptionC001 = new DataTable();
                DToptionC001.Columns.Add("vcName");
                DToptionC001.Columns.Add("vcValue");
                DataRow dr = DToptionC001.NewRow();
                dr["vcName"] = "已下载";
                dr["vcValue"] = "is not null";
                DToptionC001.Rows.Add(dr);
                dr = DToptionC001.NewRow();
                dr["vcName"] = "未下载";
                dr["vcValue"] = "is null";
                DToptionC001.Rows.Add(dr);
                List<Object> dataList_C001 = ComFunction.convertAllToResult(DToptionC001);
                res.Add("C001", dataList_C001);
                
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

            try
            {
                string strDownloadDiff = dataForm.strDownloadDiff;
                DataTable dt = FS0718_Logic.Search(strDownloadDiff,loginInfo.UserId);

                #region 周度内饰期间特殊处理
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["vcNSDiff"]!=null && dt.Rows[i]["vcNSDiff"].ToString() =="周度内饰")
                    {
                        string strNSQJ = dt.Rows[i]["vcNSQJ"].ToString();
                        string strBefore = strNSQJ.Split(' ')[0];
                        string strAfter = strNSQJ.Split(' ')[strNSQJ.Split(' ').Length - 1];

                        #region 开始时间和班值
                        string strBeforeTime = strBefore.Split('/')[0];
                        string strBeforeBZ = strBefore.Split('/')[strBefore.Split('/').Length - 1];
                        if (strBeforeBZ == "0")
                        {
                            strBeforeBZ = "白值";
                        }
                        else if (strBeforeBZ == "1")
                        {
                            strBeforeBZ = "夜值";
                        }
                        strBefore = strBeforeTime + strBeforeBZ;
                        #endregion

                        #region 结束时间班值
                        string strAfterTime = strAfter.Split('/')[0];
                        string strAfterBZ = strAfter.Split('/')[strAfter.Split('/').Length - 1];
                        if (strAfterBZ == "0")
                        {
                            strAfterBZ = "白班";
                        }
                        else if (strAfterBZ == "1") 
                        {
                            strAfterBZ = "夜值";
                        }
                        strAfter = strAfterTime + strAfterBZ;
                        #endregion

                        dt.Rows[i]["vcNSQJ"] = strBefore + " - " + strAfter;
                    }
                }
                #endregion

                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("dFaBuTime", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dFirstDownload", ConvertFieldType.DateType, "yyyy/MM/dd");
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

            try
            {
                JArray listInfo = dataForm.multipleSelection;
                if (listInfo.Count<=0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "至少选择一条数据！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();

                #region 定义两个List分别记录月度和周度的内饰数据
                List<Dictionary<string, Object>> list_NSMonth = new List<Dictionary<string, object>>();
                List<Dictionary<string, Object>> list_NSWeek = new List<Dictionary<string, object>>();
                #endregion

                #region 定义两个DataTable,用来接收月度和周度的查询结果

                #endregion
                #region 筛选用户所选数据
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (listInfoData[i]["vcType"] != null)
                    {
                        if (listInfoData[i]["vcType"].ToString() == "月度内饰")
                        {
                            list_NSMonth.Add(listInfoData[i]);
                        }
                        if (listInfoData[i]["vcType"].ToString() == "周度内饰")
                        {
                            list_NSWeek.Add(listInfoData[i]);
                        }
                    }
                }
                #endregion

                #region 对所选月度内饰进行处理
                if (list_NSMonth.Count > 0)
                {
                    for (int i = 0; i < list_NSMonth.Count; i++)
                    {
                        string strSupplier = list_NSMonth[i]["vcSupplier"].ToString();
                        string strYearMonth = list_NSMonth[i]["vcYearMonth"].ToString();
                        string strFaBuTime = list_NSMonth[i]["dFaBuTime"].ToString();
                        DataTable dt_Month = FS0718_Logic.Search_Month(strSupplier, strYearMonth, strFaBuTime);

                        string resMsg = "";
                        string[] head = { "对象月", "包装场",  "包装材品番",  "GPS品番", "供货商代码",   "供货商工区",   "供货商名称（中文）",   "纳入周期",    "纳入单位",    "当月合计必要数", "+1月合计必要数" ,   "+2月合计必要数",
                                          "1日" , "2日",  "3日",  "4日",  "5日",  "6日",  "7日",  "8日",  "9日",  "10日", "11日", "12日", "13日", "14日", "15日",
                                          "16日", "17日" ,"18日" ,"19日", "20日", "21日", "22日", "23日", "24日", "25日", "26日", "27日", "28日", "29日", "30日" ,"31日", "作成时间"};

                        string[] fields = { "vcYearMonth","vcPackSpot","vcPackNo","vcPackGPSNo","vcSupplierCode","vcSupplierWork","vcSupplierName",
                                            "vcCycle","iRelease","iDayNNum","iDayN1Num","iDayN2Num",
                                            "iDay1","iDay2","iDay3","iDay4","iDay5","iDay6","iDay7","iDay8","iDay9","iDay10",
                                            "iDay11","iDay12","iDay13","iDay14","iDay15","iDay16","iDay17","iDay18","iDay19","iDay20",
                                            "iDay21","iDay22","iDay23","iDay24","iDay25","iDay26","iDay27","iDay28","iDay29","iDay30",
                                            "iDay31","dZYTime"};
                        string filepath = ComFunction.DataTableToExcel(head, fields, dt_Month, _webHostEnvironment.ContentRootPath, loginInfo.UserId, "月度内饰书导出", ref resMsg);

                    }
                }
                #endregion

                #region 对所选周度内饰进行处理
                if (list_NSWeek.Count>0)
                {
                    for (int i = 0; i < list_NSWeek.Count; i++)
                    {
                        string strNSQJ = list_NSWeek[i]["vcNSQJ"].ToString();
                        string strBefore = strNSQJ.Split(' ')[0];
                        string strAfter = strNSQJ.Split(' ')[strNSQJ.Split(' ').Length-1];

                        string strBegin = dataForm.dFromBegin;
                        string strEnd = dataForm.dFromEnd;
                        string strFromBeginBZ = dataForm.vcFromBeginBZ;
                        string strFromEndBZ = dataForm.vcFromEndBZ;
                    }
                }
                #endregion
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "";
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

        /// <summary>
        /// 创建压缩包
        /// </summary>
        /// <param name="zipName">压缩包名称（路径）</param>
        /// <param name="files">要压缩的文件，key-文件名，value-文件字节数组</param>
        private void CreateZipPackage(string zipName, Dictionary<string, byte[]> files)
        {
            using (FileStream zip = System.IO.File.Create(zipName))
            {
                using (ZipOutputStream zipStream = new ZipOutputStream(zip))
                {
                    foreach (KeyValuePair<string, byte[]> kv in files)
                    {
                        //压缩包内条目
                        ZipEntry entry = new ZipEntry(kv.Key);
                        //添加条目
                        zipStream.PutNextEntry(entry);
                        //设置压缩级别1~9
                        zipStream.SetLevel(5);
                        //写入
                        zipStream.Write(kv.Value, 0, kv.Value.Length);
                    }
                }
            }
        }

    }
}
