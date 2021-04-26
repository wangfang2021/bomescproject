﻿using System;
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

namespace SPPSApi.Controllers.G07
{
    [Route("api/FS0704_Sub_Import/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0704Controller_Sub_Import : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0704_Logic FS0704_Logic = new FS0704_Logic();
        private readonly string FunctionID = "FS0704";

        public FS0704Controller_Sub_Import(IWebHostEnvironment webHostEnvironment)
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




                string[,] headers = new string[,] {{"逻辑名称","部品入荷日期(起)","部品入荷时刻(起)","部品入荷日期(止)","部品入荷时刻(止)", "发注作业日期(起)", "发注作业时刻(起)","发注作业日期(止)"
                                                    ,"发注作业时刻(止)","包材纳期日期(起)","包材纳期时刻(起)","包材纳期日期(止)","包材纳期时刻(止)","包材纳入便次名称","包装厂","TC(FROM)","TC(TO)"},
                                                {"vcFaZhuID","vcRuHeFromDay","dRuHeFromTime","vcRuHeToDay","druHeToTime","vcFaZhuFromDay","dFaZhuFromTime",
                                                 "vcFaZhuToDay","dFaZhuToTime","vcNaQiFromDay","dNaQiFromTime","vcNaQiToDay","dNaQiToTime","vcBianCi","vcPackSpot","dFrom","dTo"},
                                                {"","",FieldCheck.Date,"",FieldCheck.Date,"",FieldCheck.Date,"",FieldCheck.Date,"",FieldCheck.Date,"","","","",FieldCheck.Date,FieldCheck.Date},
                                                {"0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0" },
                                                {"0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0" }
                                                   };//最小长度设定,可以为空用0
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

                #region  检查本身导入文件时间重叠行
                DataTable dtcope = importDt.Copy();
                dtcope.Clear();
                for (int i = 0; i < importDt.Rows.Count; i++)
                {
                    DataRow[] dr = importDt.Select("vcFaZhuID='" + importDt.Rows[i]["vcFaZhuID"].ToString() + "'");
                    //dtcope.Rows.Add(dr);
                    i = dr.Length;
                    int j = dr.Length - 1;
                    while (j > 0)
                    {
                        //DateTime dRuHeFromDay = DateTime.ParseExact(dr[j]["dRuHeFromTime"].ToString(), "HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                        //DateTime dRuHeToDay = DateTime.ParseExact(dr[j]["druHeToTime"].ToString(), "HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                        //DateTime dFaZhuFromDay = DateTime.ParseExact(dr[j]["dFaZhuFromTime"].ToString(), "HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                        //DateTime dFaZhuToDay = DateTime.ParseExact(dr[j]["dFaZhuToTime"].ToString(), "HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                        //DateTime dNaQiFromDay = DateTime.ParseExact(dr[j]["dNaQiFromTime"].ToString(), "HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                        //DateTime dNaQiToDay = DateTime.ParseExact(dr[j]["dNaQiToTime"].ToString(), "HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);


                        DateTime dRuHeFromDay = DateTime.Parse(dr[j]["dRuHeFromTime"].ToString()).AddDays(Convert.ToInt32(dr[j]["vcRuHeFromDay"].ToString()));
                        DateTime dRuHeToDay = DateTime.Parse(dr[j]["druHeToTime"].ToString()).AddDays(Convert.ToInt32(dr[j]["vcRuHeToDay"].ToString()));
                        DateTime dFaZhuFromDay = DateTime.Parse(dr[j]["dFaZhuFromTime"].ToString()).AddDays(Convert.ToInt32(dr[j]["vcFaZhuFromDay"].ToString()));
                        DateTime dFaZhuToDay = DateTime.Parse(dr[j]["dFaZhuToTime"].ToString()).AddDays(Convert.ToInt32(dr[j]["vcFaZhuToDay"].ToString()));
                        DateTime dNaQiFromDay = DateTime.Parse(dr[j]["dNaQiFromTime"].ToString()).AddDays(Convert.ToInt32(dr[j]["vcNaQiFromDay"].ToString()));
                        DateTime dNaQiToDay = DateTime.Parse(dr[j]["dNaQiToTime"].ToString()).AddDays(Convert.ToInt32(dr[j]["vcNaQiToDay"].ToString()));



                        DateTime dFrom = DateTime.Parse(dr[j]["dFrom"].ToString());
                        DateTime dTo = DateTime.Parse(dr[j]["dTo"].ToString());
                        if (dFrom > dTo)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = dr[j]["vcFaZhuID"].ToString() + "TC(FROM)大于TC(TO)！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }


                        //判断入荷早于发注早于纳期
                        if (dRuHeToDay < dRuHeFromDay)
                        {

                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = dr[j]["vcFaZhuID"].ToString() + "部品入荷起始时间不能大于终止时间！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        if (dFaZhuToDay < dFaZhuFromDay)
                        {

                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = dr[j]["vcFaZhuID"].ToString() + "发注作业起始时间不能大于终止时间！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        if (dNaQiToDay < dNaQiFromDay)
                        {

                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = dr[j]["vcFaZhuID"].ToString() + "包材纳期起始时间不能大于终止时间！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        if (dRuHeToDay > dFaZhuFromDay || dRuHeToDay > dNaQiFromDay)
                        {

                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = dr[j]["vcFaZhuID"].ToString() + "部品入荷时间不能大于发注作业时间和纳期时间！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        for (int z = 0; z <= j - 1; z++)
                        {
                            //DateTime dRuHeFromDay1 = DateTime.ParseExact(dr[z]["dRuHeFromTime"].ToString(), "HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                            //DateTime dRuHeToDay1 = DateTime.ParseExact(dr[z]["druHeToTime"].ToString(), "HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                            //DateTime dFaZhuFromDay1 = DateTime.ParseExact(dr[z]["dFaZhuFromTime"].ToString(), "HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                            //DateTime dFaZhuToDay1 = DateTime.ParseExact(dr[z]["dFaZhuToTime"].ToString(), "HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                            //DateTime dNaQiFromDay1 = DateTime.ParseExact(dr[z]["dNaQiFromTime"].ToString(), "HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                            //DateTime dNaQiToDay1 = DateTime.ParseExact(dr[z]["dNaQiToTime"].ToString(), "HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);

                            DateTime dRuHeFromDay1 = DateTime.Parse(dr[z]["dRuHeFromTime"].ToString()).AddDays(Convert.ToInt32(dr[z]["vcRuHeFromDay"].ToString()));
                            DateTime dRuHeToDay1 = DateTime.Parse(dr[z]["druHeToTime"].ToString()).AddDays(Convert.ToInt32(dr[z]["vcRuHeToDay"].ToString()));
                            DateTime dFaZhuFromDay1 = DateTime.Parse(dr[z]["dFaZhuFromTime"].ToString()).AddDays(Convert.ToInt32(dr[z]["vcFaZhuFromDay"].ToString()));
                            DateTime dFaZhuToDay1 = DateTime.Parse(dr[z]["dFaZhuToTime"].ToString()).AddDays(Convert.ToInt32(dr[z]["vcFaZhuToDay"].ToString()));
                            DateTime dNaQiFromDay1 = DateTime.Parse(dr[z]["dNaQiFromTime"].ToString()).AddDays(Convert.ToInt32(dr[z]["vcNaQiFromDay"].ToString()));
                            DateTime dNaQiToDay1 = DateTime.Parse(dr[z]["dNaQiToTime"].ToString()).AddDays(Convert.ToInt32(dr[z]["vcNaQiToDay"].ToString()));


                            if ((dRuHeFromDay <= dRuHeFromDay1 && dRuHeFromDay1 <= dRuHeToDay) || (dRuHeFromDay <= dRuHeToDay1 && dRuHeToDay1 <= dRuHeToDay))
                            {
                                apiResult.code = ComConstant.ERROR_CODE;
                                apiResult.data = dr[0]["vcFaZhuID"].ToString() + "导入文件部品入荷时间有重叠";
                                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                            }
                            else if ((dFaZhuFromDay <= dFaZhuFromDay1 && dFaZhuFromDay1 <= dFaZhuToDay) || (dFaZhuFromDay <= dFaZhuToDay1 && dFaZhuToDay1 <= dFaZhuToDay))
                            {
                                apiResult.code = ComConstant.ERROR_CODE;
                                apiResult.data = dr[0]["vcFaZhuID"].ToString().ToString() + "导入文件发注作业时间有重叠";
                                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                            }
                            else if ((dNaQiFromDay <= dNaQiFromDay1 && dNaQiFromDay1 <= dNaQiToDay) || (dNaQiFromDay <= dNaQiToDay1 && dNaQiToDay1 <= dNaQiToDay))
                            {
                                apiResult.code = ComConstant.ERROR_CODE;
                                apiResult.data = dr[0]["vcFaZhuID"].ToString() + "导入文件部品入荷时间有重叠";
                                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                            }
                        }
                        j--;
                    }
                }

                #endregion

                //判断对应逻辑下是否有重叠时间
                #region 判断对应逻辑下是否有重叠时间
                DataTable dtLJtime = FS0704_Logic.SearchLJTime("", "");
                for (int a = 0; a < importDt.Rows.Count; a++)
                {
                    DataRow[] dr = dtLJtime.Select("vcFaZhuID='" + importDt.Rows[a]["vcFaZhuID"].ToString() + "' vcBianCi<>'" + importDt.Rows[a]["vcBianCi"].ToString() + "'");
                    int count = dtcope.Rows.Count - 1;
                    a = dtLJtime.Rows.Count;
                    while (count > 0)
                    {
                        DateTime dRuHeFromDay = DateTime.ParseExact(importDt.Rows[a]["dRuHeFromTime"].ToString().Split(" ")[1], "HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                        DateTime dRuHeToDay = DateTime.ParseExact(importDt.Rows[a]["druHeToTime"].ToString().Split(" ")[1], "HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                        DateTime dFaZhuFromDay = DateTime.ParseExact(importDt.Rows[a]["dFaZhuFromTime"].ToString().Split(" ")[1], "HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                        DateTime dFaZhuToDay = DateTime.ParseExact(importDt.Rows[a]["dFaZhuToTime"].ToString().Split(" ")[1], "HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                        DateTime dNaQiFromDay = DateTime.ParseExact(importDt.Rows[a]["dNaQiFromTime"].ToString().Split(" ")[1], "HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                        DateTime dNaQiToDay = DateTime.ParseExact(importDt.Rows[a]["dNaQiToTime"].ToString().Split(" ")[1], "HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                      
                        
                        
                        
                        
                        for (int z = 0; z <= count - 1; z++)
                        {
                            DateTime dRuHeFromDay1 = DateTime.ParseExact(dr[z]["dRuHeFromTime"].ToString(), "HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                            DateTime dRuHeToDay1 = DateTime.ParseExact(dr[z]["druHeToTime"].ToString(), "HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                            DateTime dFaZhuFromDay1 = DateTime.ParseExact(dr[z]["dFaZhuFromTime"].ToString(), "HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                            DateTime dFaZhuToDay1 = DateTime.ParseExact(dr[z]["dFaZhuToTime"].ToString(), "HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                            DateTime dNaQiFromDay1 = DateTime.ParseExact(dr[z]["dNaQiFromTime"].ToString(), "HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                            DateTime dNaQiToDay1 = DateTime.ParseExact(dr[z]["dNaQiToTime"].ToString(), "HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                            if ((dRuHeFromDay <= dRuHeFromDay1 && dRuHeFromDay1 <= dRuHeToDay) || (dRuHeFromDay <= dRuHeToDay1 && dRuHeToDay1 <= dRuHeToDay))
                            {
                                apiResult.code = ComConstant.ERROR_CODE;
                                apiResult.data = dr[0]["vcFaZhuID"].ToString() + "部品入荷时间有重叠";
                                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                            }
                            else if ((dFaZhuFromDay <= dFaZhuFromDay1 && dFaZhuFromDay1 <= dFaZhuToDay) || (dFaZhuFromDay <= dFaZhuToDay1 && dFaZhuToDay1 <= dFaZhuToDay))
                            {
                                apiResult.code = ComConstant.ERROR_CODE;
                                apiResult.data = dr[0]["vcFaZhuID"].ToString().ToString() + "发注作业时间有重叠";
                                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                            }
                            else if ((dNaQiFromDay <= dNaQiFromDay1 && dNaQiFromDay1 <= dNaQiToDay) || (dNaQiFromDay <= dNaQiToDay1 && dNaQiToDay1 <= dNaQiToDay))
                            {
                                apiResult.code = ComConstant.ERROR_CODE;
                                apiResult.data = dr[0]["vcFaZhuID"].ToString() + "部品入荷时间有重叠";
                                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                            }
                        }
                        count--;
                    }

                }
                #endregion


                FS0704_Logic.importSave(importDt, loginInfo.UserId);
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
