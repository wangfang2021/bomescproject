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

namespace SPPSApi.Controllers.G01
{
    [Route("api/FS0101_Sub_Import/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0101Controller_Sub_Import : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0101_Logic fs0101_Logic = new FS0101_Logic();
        private readonly string FunctionID = "FS0101";

        public FS0101Controller_Sub_Import(IWebHostEnvironment webHostEnvironment)
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
            JArray fileNameList = dataForm.fileNameList;

            Dictionary<string, object> res = new Dictionary<string, object>();
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
                string[,] strField = new string[,] {{"工厂"       ,"班值"    ,"包装场"          ,"用户编号","用户名称"  ,"用户密码"  ,"角色"      ,"邮箱地址","特殊权限" ,"停用"  },
                                                   {"vcPlantCode","vcBanZhi","vcBaoZhuangPlace","vcUserID","vcUserName","vcPassWord","vcUserRole","vcEmail" ,"vcSpecial","vcStop"},
                                                   {""           ,""        ,""                ,""        ,""          ,""          ,""          ,""        ,""         ,""      },
                                                   {"50"         ,"10"      ,"10"              ,"10"      ,"20"        ,"100"       ,"0"         ,"100"     ,"50"       ,"1"     },//最大长度设定,不校验最大长度用0
                                                   {"0"          ,"0"       ,"0"               ,"1"       ,"1"         ,"1"         ,"1"         ,"0"       ,"0"        ,"0"     }//最小长度设定,可以为空用0
                };

                DataTable importDt = new DataTable();
                foreach (FileInfo info in theFolder.GetFiles())
                {
                    DataTable dt = ComFunction.ExcelToDataTable(info.FullName, "sheet1", strField, ref strMsg);

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

                //记录所有的错误信息
                string strErrAll = "";
                
                #region 从数据库中取出需要验证的数据
                //工厂    dt列     vcPlantCode,vcPlantName
                DataTable dt1 = fs0101_Logic.getPlantList(loginInfo.UnitCode);

                //班值    dt列     key,label
                DataTable dt2 = fs0101_Logic.getBanZhi();

                //包装场   dt列     key,label
                DataTable dt3 = fs0101_Logic.getBaoZhuangPlace();

                //用户编号  dt列     vcUserID
                DataTable dt4 = fs0101_Logic.getUserID();

                //用户角色  dt列     key,label
                DataTable dt7 = fs0101_Logic.getAllRole();

                //特殊权限  dt列     label,value
                DataTable dt9 = fs0101_Logic.getAllSpecial();
                #endregion

                #region 校验所有数据
                for (int i = 0; i < importDt.Rows.Count; i++)
                {
                    #region 错误信息
                    //工厂错误信息
                    string strErr1 = "";

                    //班值错误信息
                    string strErr2 = "";

                    //包装场错误信息
                    string strErr3 = "";

                    //用户编号错误信息
                    string strErr4 = "";

                    //用户名称错误信息
                    string strErr5 = "";

                    //用户密码错误信息
                    string strErr6 = "";

                    //角色错误信息
                    string strErr7 = "";

                    //邮箱地址错误信息
                    string strErr8 = "";

                    //特殊权限错误信息
                    string strErr9 = "";

                    #endregion

                    //工厂信息
                    string str1 = importDt.Rows[i]["vcPlantCode"] == null ? "" : importDt.Rows[i]["vcPlantCode"].ToString();

                    //班值信息
                    string str2 = importDt.Rows[i]["vcBanZhi"] == null ? "" : importDt.Rows[i]["vcBanZhi"].ToString();

                    //包装场信息
                    string str3 = importDt.Rows[i]["vcBaoZhuangPlace"] == null ? "" : importDt.Rows[i]["vcBaoZhuangPlace"].ToString();

                    //用户编号信息
                    string str4 = importDt.Rows[i]["vcUserID"] == null ? "" : importDt.Rows[i]["vcUserID"].ToString();

                    //用户名称信息
                    string str5 = importDt.Rows[i]["vcUserName"] == null ? "" : importDt.Rows[i]["vcUserName"].ToString();

                    //用户密码信息
                    string str6 = importDt.Rows[i]["vcPassWord"] == null ? "" : importDt.Rows[i]["vcPassWord"].ToString();

                    //角色信息
                    string str7 = importDt.Rows[i]["vcUserRole"] == null ? "" : importDt.Rows[i]["vcUserRole"].ToString();

                    //邮箱地址信息
                    string str8 = importDt.Rows[i]["vcEmail"] == null ? "" : importDt.Rows[i]["vcEmail"].ToString();

                    //特殊权限信息
                    string str9 = importDt.Rows[i]["vcSpecial"] == null ? "" : importDt.Rows[i]["vcSpecial"].ToString();

                    //停用信息
                    string str10 = importDt.Rows[i]["vcStop"] == null ? "" : importDt.Rows[i]["vcStop"].ToString();

                    #region 校验必填的字段

                    #region 校验工厂,并将name值转换为value值
                    str1 = str1.Trim();
                    string str1Conver = "";
                    if (str1.Length > 0)  //不为空
                    {
                        if (str1.Substring(str1.Length - 1) == ";") //最后一个字符是';'
                        {
                            str1 = str1.Substring(0, str1.Length - 1);      //去';'
                        }
                        string[] str1s = str1.Split(';');
                        for (int j = 0; j < str1s.Length; j++)
                        {
                            if (dt1.Select("vcPlantName = '" + str1s[j] + "'").Length <= 0)
                            {
                                strErr1 += "第" + (i + 2) + "行工厂'" + str1s[j] + "'不存在;";
                            }
                            else
                            {
                                if (str1Conver.Length>0)
                                {
                                    str1Conver += ",";
                                }
                                str1Conver += dt1.Select("vcPlantName = '" + str1s[j] + "'")[0]["vcPlantCode"].ToString();
                            }
                        }
                        importDt.Rows[i]["vcPlantCode"] = str1Conver;
                    }
                    #endregion

                    #region 校验班值,并将name转换为value
                    str2 = str2.Trim();
                    string str2Conver = "";
                    if (str2.Length > 0)  //不为空
                    {
                        if (dt2.Select("label = '" + str2 + "'").Length <= 0)
                        {
                            strErr2 += "第" + (i + 2) + "行班值'" + str2 + "'不存在;";
                        }
                        else
                        {
                            str2Conver = dt2.Select("label = '" + str2 + "'")[0]["key"].ToString();
                            importDt.Rows[i]["vcBanZhi"] = str2Conver;
                        }
                    }
                    #endregion

                    #region 校验包装场,并将name转换为value
                    str3 = str3.Trim();
                    if (str3.Length > 0)  //不为空
                    {
                        if (dt3.Select("label = '" + str3 + "'").Length <= 0)
                        {
                            strErr3 += "第" + (i + 2) + "行包装场'" + str3 + "'不存在;";
                        }
                        else
                        {
                            importDt.Rows[i]["vcBaoZhuangPlace"] = dt3.Select("label = '" + str3 + "'")[0]["key"].ToString();
                        }
                    }
                    #endregion

                    #region 校验用户编号
                    str4 = str4.Trim();
                    if (str4.Length > 0)  //不为空
                    {
                        if (importDt.Select("vcUserID = '" + str4 + "'").Length > 1)
                        {
                            //errorUserIdLists.Add((i + 2).ToString(), str4);
                        }

                        if (dt4.Select("vcUserID = '" + str4 + "'").Length > 0)
                        {
                            strErr4 += "第" + (i + 2) + "行用户编号'" + str4 + "'已存在;";
                        }

                    }
                    #endregion

                    #region 校验角色
                    str7 = str7.Trim();
                    if (str7.Length > 0)  //不为空
                    {
                        if (str7.Substring(str7.Length - 1) == ";") //最后一个字符是';'
                        {
                            str7 = str7.Substring(0, str7.Length - 1);      //去';'
                        }
                        string[] str7s = str7.Split(';');
                        for (int j = 0; j < str7s.Length; j++)
                        {
                            if (dt7.Select("label = '" + str7s[j] + "'").Length <= 0)
                            {
                                strErr7 += "第" + (i + 2) + "行角色'" + str7s[j] + "'不存在;";
                            }
                        }
                    }
                    #endregion

                    #region 校验特殊权限
                    str9 = str9.Trim();
                    if (str9.Length > 0)  //不为空
                    {
                        if (dt9.Select("label = '" + str9 + "'").Length <= 0)
                        {
                            strErr9 += "第" + (i + 2) + "行特殊权限'" + str9 + "'不存在;";
                        }
                    }
                    #endregion

                    #endregion

                    if ((strErr1 + strErr2 + strErr3 + strErr4 + strErr5 + strErr6 + strErr7 + strErr8 + strErr9).Length>0)
                    {
                        strErrAll += strErr1 + strErr2 + strErr3 + strErr4 + strErr5 + strErr6 + strErr7 + strErr8 + strErr9+"|";
                    }
                }
                #endregion

                strErrAll = strErrAll.Substring(0, strErrAll.Length - 1);       //去掉最后一个'|'

                if (strErrAll.Replace("|","")!="")
                {

                    string strReturn = "";
                    for (int i = 0; i < strErrAll.Split('|').Length; i++)
                    {
                        if (strErrAll.Split('|')[i].Trim().Length>0)
                        {
                            strReturn += strErrAll.Split('|')[i] + "<br/>";
                        }
                    }
                    res.Add("strErr", "<br/>" + strReturn);
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.flag = 1;
                    apiResult.data = res;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                fs0101_Logic.ImportAddUsers(loginInfo.UnitCode, importDt, loginInfo.PlatForm, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "导入成功";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0103", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导入失败" + ex.Message;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

    }
}
