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
        ZIPHelper zIPHelper = new ZIPHelper();
        FileHelper fileHelper = new FileHelper();
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

                #region 格式化DataTable
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["dFirstDownload"]!=null && dt.Rows[i]["dFirstDownload"].ToString().Trim()!="")
                    {
                        dt.Rows[i]["vcNSState"] = "已下载";
                    }
                    else
                    {
                        dt.Rows[i]["vcNSState"] = "未下载";
                    }
                    if (dt.Rows[i]["vcNSDiff"]!=null && dt.Rows[i]["vcNSDiff"].ToString() =="周度内示")
                    {
                        dt.Rows[i]["vcYearMonth"] = "—";
                        string strNSQJ = dt.Rows[i]["vcNSQJ"].ToString();
                        string strBefore = strNSQJ.Split(' ')[0];
                        string strAfter = strNSQJ.Split(' ')[1];
                        string strPlan = strNSQJ.Split(' ')[2];

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

                        dt.Rows[i]["vcNSQJ"] = strBefore + " " + strAfter+" "+strPlan;
                    }
                    if (dt.Rows[i]["vcNSDiff"] != null && dt.Rows[i]["vcNSDiff"].ToString() == "月度内示")
                    {
                        dt.Rows[i]["vcNSQJ"] = "—";
                    }
                }
                #endregion

                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("dFaBuTime", ConvertFieldType.DateType, "yyyy/MM/dd HH:mm:ss");
                dtConverter.addField("dFirstDownload", ConvertFieldType.DateType, "yyyy/MM/dd HH:mm:ss");
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M07UE1801", ex, loginInfo.UserId);
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

                #region 定义两个List分别记录月度和周度的内示数据
                List<Dictionary<string, Object>> list_NSMonth = new List<Dictionary<string, object>>();
                List<Dictionary<string, Object>> list_NSWeek = new List<Dictionary<string, object>>();
                #endregion

                #region 筛选用户所选数据
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (listInfoData[i]["vcNSDiff"] != null)
                    {
                        if (listInfoData[i]["vcNSDiff"].ToString() == "月度内示")
                        {
                            list_NSMonth.Add(listInfoData[i]);
                        }
                        if (listInfoData[i]["vcNSDiff"].ToString() == "周度内示")
                        {
                            list_NSWeek.Add(listInfoData[i]);
                        }
                    }
                }
                #endregion

                #region 对所选月度内示进行处理
                ComFunction.ConsoleWriteLine("开始对所选月度内示进行处理");
                if (list_NSMonth.Count > 0)
                {
                    for (int i = 0; i < list_NSMonth.Count; i++)
                    {
                        string strSupplier = list_NSMonth[i]["vcSupplier"].ToString();
                        string strYearMonth = list_NSMonth[i]["vcYearMonth"].ToString().Replace("-","");
                        string strFaBuTime = Convert.ToDateTime(list_NSMonth[i]["dFaBuTime"].ToString()).ToString();
                        DataTable dt_Month = FS0718_Logic.Search_Month(strSupplier, strYearMonth, strFaBuTime);
                        ComFunction.ConsoleWriteLine("检索出月度内示数据条数(DataTable.rows)"+dt_Month.Rows.Count);
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
                        string strfileName = "月度内示书_"+strYearMonth+"_"+i+"_";
                        string filepath = ComFunction.DataTableToExcel(head, fields, dt_Month, _webHostEnvironment.ContentRootPath, loginInfo.UserId, strfileName, ref resMsg);

                        string strFilePath_All = _webHostEnvironment.ContentRootPath + "Doc" + Path.DirectorySeparatorChar + "Export" + Path.DirectorySeparatorChar + filepath;

                        if (string.IsNullOrEmpty(filepath))
                        {
                            ComFunction.ConsoleWriteLine("error文件生成失败！");
                        }
                        else
                        {
                            ComFunction.ConsoleWriteLine("##文件生成成功！文件名："+filepath);
                            ComFunction.ConsoleWriteLine("##文件所在完整路径：" + strFilePath_All);
                        }
                        
                        if (filepath == "")
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "导出生成文件失败";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        string strMessage = "##校验文件是否存在：";
                        if (System.IO.File.Exists(strFilePath_All))
                        {
                            strMessage += "true";
                        }
                        else
                        {
                            strMessage += "False";
                        }
                        ComFunction.ConsoleWriteLine(strMessage);
                        zIPHelper.addFiles(filepath, _webHostEnvironment.ContentRootPath + "Doc" + Path.DirectorySeparatorChar + "Export" + Path.DirectorySeparatorChar);
                    }
                }
                #endregion

                #region 对所选周度内示进行处理
                if (list_NSWeek.Count>0)
                {
                    for (int i = 0; i < list_NSWeek.Count; i++)
                    {
                        string strNSQJ = list_NSWeek[i]["vcNSQJ"].ToString();

                        string strBefore = strNSQJ.Split(' ')[0];
                        string strAfter = strNSQJ.Split(' ')[1];

                        string strBeforeTime = strBefore.Substring(0,strBefore.Length-2);
                        string strBeforeBZ = strBefore.Substring(strBefore.Length-2,2);

                        string strAfterTime = strAfter.Substring(0,strAfter.Length-2);
                        string strAfterBZ = strAfter.Substring(strAfter.Length-2,2);

                        if (strBeforeBZ == "白值")
                        {
                            strBeforeBZ = "0";
                        }
                        else if (strBeforeBZ=="夜值")
                        {
                            strBeforeBZ = "1";
                        }
                        if (strAfterBZ == "白值")
                        {
                            strAfterBZ = "0";
                        }
                        else if (strAfterBZ == "夜值")
                        {
                            strAfterBZ = "1";
                        }

                        string strBegin = strBeforeTime;
                        string strEnd = strAfterTime;
                        string strFromBeginBZ = strBeforeBZ;
                        string strFromEndBZ = strAfterBZ;

                        string strSearch_SupplierCode = list_NSWeek[i]["vcSupplier"].ToString();
                        string strSearch_dFaBuTime = list_NSWeek[i]["dFaBuTime"].ToString().Replace("/","-");
                        DataTable dt = FS0718_Logic.Search_Week(strSearch_SupplierCode,strSearch_dFaBuTime);
                        for (int t = 0; t < dt.Rows.Count; t++)
                        {
                            dt.Rows[t]["vcNum"] = (t + 1).ToString();
                        }
                        string resMsg = "";
                        int count = 0;
                        string[] headData = new string[0];
                        string[] fieldsData = new string[0];
                        if (strBegin.Split("-")[1] == strEnd.Split("-")[1])
                        {
                            count = (Convert.ToInt32(strEnd.Split("-")[2]) - Convert.ToInt32(strBegin.Split("-")[2]) + 1) * 2;

                            if (strFromBeginBZ == "1")
                            {
                                count--;
                            }
                            else if (strFromEndBZ == "0")
                            {
                                count--;
                            }
                            headData = new string[count + 5];
                            fieldsData = new string[count + 5];
                            headData[0] = "序号";
                            headData[1] = "补给品番";
                            headData[2] = "GPS品番";
                            headData[3] = "包材厂家";
                            headData[4] = "纳入收容数";
                            fieldsData[0] = "vcNum";
                            fieldsData[1] = "vcPackNo";
                            fieldsData[2] = "vcPackGPSNo";
                            fieldsData[3] = "vcSupplierName";
                            fieldsData[4] = "iRelease";
                            int x = 1;
                            for (int j = 1; j <= 31; j++)
                            {
                                if (dt.Rows[0]["vcD" + j + "bFShow"].ToString() == "true")
                                {
                                    headData[4 + x] = j + "日白值F";
                                    fieldsData[4 + x] = "vcD" + j + "bF";
                                    x++;
                                }
                                if (dt.Rows[0]["vcD" + j + "yFShow"].ToString() == "true")
                                {
                                    headData[4 + x] = j + "日夜值F";
                                    fieldsData[4 + x] = "vcD" + j + "yF";
                                    x++;
                                }
                                if (dt.Rows[0]["vcD" + j + "bTShow"].ToString() == "true")
                                {
                                    headData[4 + x] = j + "日白值T";
                                    fieldsData[4 + x] = "vcD" + j + "bT";
                                    x++;
                                }

                                if (dt.Rows[0]["vcD" + j + "yTShow"].ToString() == "true")
                                {
                                    headData[4 + x] = j + "日夜值T";
                                    fieldsData[4 + x] = "vcD" + j + "yT";
                                    x++;
                                }
                            }
                        }
                        else
                        {
                            DateTime dt1 = Convert.ToDateTime(strBegin).AddDays(1 - Convert.ToDateTime(strBegin).Day).AddMonths(1).AddDays(-1);
                            int ss = (Convert.ToInt32(dt1.ToString().Split(" ")[0].Split("-")[2]) - Convert.ToInt32(strBegin.Split("-")[2]) + 1) * 2;
                            int cs = Convert.ToInt32(strEnd.Split("-")[2]) * 2;
                            if (strFromBeginBZ == "1")
                            {
                                ss--;
                            }
                            else if (strFromBeginBZ == "0")
                            {
                                cs--;
                            }
                            headData = new string[ss + 5 + cs];
                            fieldsData = new string[ss + 5 + cs];
                            headData[0] = "序号";
                            headData[1] = "补给品番";
                            headData[2] = "GPS品番";
                            headData[3] = "包材厂家";
                            headData[4] = "纳入收容数";
                            fieldsData[0] = "vcNum";
                            fieldsData[1] = "vcPackNo";
                            fieldsData[2] = "vcPackGPSNo";
                            fieldsData[3] = "vcSupplierName";
                            fieldsData[4] = "iRelease";
                            int x = 1;
                            for (int j = 1; j <= 31; j++)
                            {

                                if (dt.Rows[0]["vcD" + j + "bFShow"].ToString() == "true")
                                {
                                    headData[4 + x] = j + "日白值F";
                                    fieldsData[4 + x] = "vcD" + j + "bF";
                                    x++;
                                }
                                if (dt.Rows[0]["vcD" + j + "yFShow"].ToString() == "true")
                                {
                                    headData[4 + x] = j + "日夜值F";
                                    fieldsData[4 + x] = "vcD" + j + "yF";
                                    x++;
                                }
                                if (dt.Rows[0]["vcD" + j + "bTShow"].ToString() == "true")
                                {
                                    headData[4 + x] = j + "日白值T";
                                    fieldsData[4 + x] = "vcD" + j + "bT";
                                    x++;
                                }

                                if (dt.Rows[0]["vcD" + j + "yTShow"].ToString() == "true")
                                {
                                    headData[4 + x] = j + "日夜值T";
                                    fieldsData[4 + x] = "vcD" + j + "yT";
                                    x++;
                                }
                            }
                        }
                        string[] head = headData;
                        string[] fields = fieldsData;

                        if (strBeforeBZ=="0")
                        {
                            strBeforeBZ = "白值";
                        }
                        else if(strBeforeBZ=="1")
                        {
                            strBeforeBZ = "夜值";
                        }
                        if (strAfterBZ=="0")
                        {
                            strAfterBZ = "白值";
                        }
                        else if (strAfterBZ=="1")
                        {
                            strAfterBZ = "夜值";
                        }
                        string strfileName = "周度内示书_" + strBeforeTime+strBeforeBZ+" - "+strAfterTime+strAfterBZ + "_" + i + "_";
                        string filepath = ComFunction.DataTableToExcel(head, fields, dt, _webHostEnvironment.ContentRootPath, loginInfo.UserId, strfileName, ref resMsg);
                        if (filepath == "")
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "导出生成文件失败";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        zIPHelper.addFiles(filepath, _webHostEnvironment.ContentRootPath + "Doc" + Path.DirectorySeparatorChar + "Export" + Path.DirectorySeparatorChar);
                    }
                }
                #endregion

                #region 更新包材内示检索表
                FS0718_Logic.updateSearchTable(listInfoData, loginInfo.UserId);
                #endregion


                zIPHelper.strZIPFileName = "内示下载" + DateTime.Now.ToString("yyyyMMdd") + ".zip";
                zIPHelper.strZIPFilePath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Export" + Path.DirectorySeparatorChar;
                
                zIPHelper.createZIPFile();

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = zIPHelper.strZIPFileName;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M07UE1802", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

    }

    #region 压缩包帮助类
    /// <summary>
    /// 压缩包帮助类
    /// </summary>
    public class ZIPHelper
    {
        /// <summary>
        /// 压缩包文件名
        /// </summary>
        public string strZIPFileName { get; set; }

        /// <summary>
        /// 压缩包文件路径
        /// </summary>
        public string strZIPFilePath { get; set; }

        /// <summary>
        /// 文件集合：文件名 — 文件二进制流
        /// </summary>
        public Dictionary<string, byte[]> files { get; set; }

        /// <summary>
        /// 实例化函数
        /// </summary>
        public ZIPHelper()
        {
            this.strZIPFileName = "";
            this.strZIPFilePath = "";
            this.files = new Dictionary<string, byte[]>();
        }

        /// <summary>
        /// 设置文件对应文件二进制流List
        /// </summary>
        /// <param name="fileLists"></param>
        public void setFiles(List<FileHelper> fileLists)
        {
            foreach (var item in fileLists)
            {
                string strFilePath_All = item.strFilePath + Path.DirectorySeparatorChar + item.strFileName;
                files.Add(item.strFileName, File.ReadAllBytes(strFilePath_All));
                if (File.Exists(strFilePath_All))
                {
                    File.Delete(strFilePath_All);
                }
            }
        }

        public void addFiles(string strFileName, string strFilePath)
        {
            ComFunction.ConsoleWriteLine("##进入添加到压缩包方法");
            string strFilePath_All = strFilePath + strFileName;
            ComFunction.ConsoleWriteLine("##传入的strFileName：" + strFileName);
            ComFunction.ConsoleWriteLine("##传入的strFilePath：" + strFilePath);
            string strMessage = "**校验文件是否存在：";
            if (File.Exists(strFilePath_All))
            {
                strMessage += "true";
            }
            else
            {
                strMessage += "False";
            }
            ComFunction.ConsoleWriteLine(strMessage);
            try
            {
                ComFunction.ConsoleWriteLine("**开始添加文件到ZIP：");
                files.Add(strFileName, File.ReadAllBytes(strFilePath_All));
                ComFunction.ConsoleWriteLine("**添加成功！添加文件完整路径："+strFilePath_All);
            }
            catch (Exception ex)
            {
                ComFunction.ConsoleWriteLine("**添加失败，异常消息："+ex.Message);
            }

            //if (File.Exists(strFilePath_All))
            //{
            //    File.Delete(strFilePath_All);
            //}
        }

        /// <summary>
        /// 创建压缩包文件
        /// </summary>
        public void createZIPFile()
        {
            using (FileStream zip = File.Create(strZIPFilePath + Path.DirectorySeparatorChar + strZIPFileName))
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
    #endregion

    #region 文件帮助类
    /// <summary>
    /// 文件帮助类
    /// </summary>
    public class FileHelper
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string strFileName { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string strFilePath { get; set; }


        /// <summary>
        /// 无参实例化
        /// </summary>
        public FileHelper()
        {
            strFileName = "";
            strFilePath = "";
        }
    }
    #endregion


}
