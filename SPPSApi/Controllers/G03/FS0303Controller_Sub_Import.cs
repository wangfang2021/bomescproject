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

namespace SPPSApi.Controllers.G03
{
    [Route("api/FS0303_Sub_Import/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0303Controller_Sub_Import : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0303_Logic FS0303_Logic = new FS0303_Logic();
        private readonly string FunctionID = "FS0303";

        public FS0303Controller_Sub_Import(IWebHostEnvironment webHostEnvironment)
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
                string[,] strField = new string[,] {{"同步数据"     ,"变更事项","设变号" ,"生确"     ,"区分"  ,"补给品番" ,"车型(技术)"  ,"车型(开发)"     ,"车名"         ,"使用开始"     ,"使用结束"     ,"切替实绩"     ,"SD"      ,"替代品番"     ,"英文品名"    ,"中文品名"    ,"号口工程","补给工程","内外"       ,"供应商代码"   ,"供应商名称"     ,"生产地"   ,"出荷地"   ,"包装工厂" ,"生产商名称","地址"       ,"开始"         ,"结束"         ,"OE=SP","品番(参考)" ,"号旧"    ,"旧型开始"     ,"旧型结束"     ,"旧型经年" ,"旧型年限生产区分","实施年限(年限)","特记"  ,"防錆"    ,"防錆指示书号","旧型1年","旧型2年","旧型3年","旧型4年","旧型5年","旧型6年","旧型7年","旧型8年","旧型9年","旧型10年","旧型11年","旧型12年","旧型13年","旧型14年","旧型15年","执行标准No","收货方"    ,"所属原单位"     },
                                                        {"dSyncTime"    ,"vcChange","vcSPINo","vcSQState","vcDiff","vcPart_id","vcCarTypeDev","vcCarTypeDesign","vcCarTypeName","dTimeFrom"    ,"dTimeTo"      ,"dTimeFromSJ"  ,"vcBJDiff","vcPartReplace","vcPartNameEn","vcPartNameCn","vcHKGC"  ,"vcBJGC"  ,"vcInOutflag","vcSupplier_id","vcSupplier_Name","vcSCPlace","vcCHPlace","vcSYTCode","vcSCSName" ,"vcSCSAdress","dGYSTimeFrom" ,"dGYSTimeTo"   ,"vcOE" ,"vcHKPart_id","vcHaoJiu","dJiuBegin"    ,"dJiuEnd"      ,"vcJiuYear","vcNXQF"          ,"dSSDateMonth"  ,"vcMeno","vcFXDiff","vcFXNo"      ,"vcNum1" ,"vcNum2" ,"vcNum3" ,"vcNum4" ,"vcNum5" ,"vcNum6" ,"vcNum7" ,"vcNum8" ,"vcNum9" ,"vcNum10" ,"vcNum11" ,"vcNum12" ,"vcNum13" ,"vcNum14" ,"vcNum15" ,"vcZXBZNo"  ,"vcReceiver","vcOriginCompany"},
                                                        {FieldCheck.Date,""        ,""       ,""         ,""      ,""         ,""            ,""               ,""             ,FieldCheck.Date,FieldCheck.Date,FieldCheck.Date,""        ,""             ,""            ,""            ,""        ,""        ,""           ,""             ,""               ,""         ,""         ,""         ,""          ,""           ,FieldCheck.Date,FieldCheck.Date,""     ,""           ,""        ,FieldCheck.Date,FieldCheck.Date,""         ,""                ,FieldCheck.Date ,""      ,""        ,""            ,""       ,""       ,""       ,""       ,""       ,""       ,""       ,""       ,""       ,""        ,""        ,""        ,""        ,""        ,""        ,""          ,""          ,""               },
                                                        {"0"            ,"1"       ,"20"     ,"1"        ,"1"     ,"12"       ,"4"           ,"4"              ,"4"            ,"0"            ,"0"            ,"0"            ,"4"       ,"12"           ,"100"         ,"100"         ,"20"      ,"20"      ,"1"          ,"4"            ,"100"            ,"20"       ,"20"       ,"1"        ,"100"       ,"100"        ,"0"            ,"0"            ,"1"    ,"12"         ,"1"       ,""             ,"0"            ,"4"        ,"20"              ,"0"             ,"200"   ,"2"       ,"12"          ,"4"      ,"4"      ,"4"      ,"4"      ,"4"      ,"4"      ,"4"      ,"4"      ,"4"      ,"4"       ,"4"       ,"4"       ,"4"       ,"4"       ,"4"       ,"50"        ,"10"        ,"1"              },//最大长度设定,不校验最大长度用0
                                                        {"1"            ,"1"      ,"1"       ,"1"        ,"1"     ,"1"        ,"1"           ,"1"              ,"20"           ,"1"            ,"1"            ,"1"            ,"1"       ,"1"            ,"1"           ,"1"           ,"1"       ,"1"       ,"1"          ,"1"            ,"1"              ,"1"        ,"1"        ,"1"        ,"1"         ,"1"          ,"1"            ,"1"            ,"1"    ,"1"          ,"1"       ,""             ,"1"            ,"1"        ,"1"               ,"1"             ,"1"     ,"1"       ,"1"           ,"1"      ,"1"      ,"1"      ,"1"      ,"1"      ,"1"      ,"1"      ,"1"      ,"1"      ,"1"       ,"1"       ,"1"       ,"1"       ,"1"       ,"1"       ,"1"         ,"1"         ,"1"              },//最小长度设定,可以为空用0
                                                        {"1"            ,"2"      ,"3"       ,"4"        ,"5"     ,"6"        ,"7"           ,"8"              ,"9"            ,"10"           ,"11"           ,"12"           ,"13"      ,"14"           ,"15"          ,"16"          ,"17"      ,"18"      ,"19"         ,"20"           ,"21"             ,"22"       ,"23"       ,"24"       ,"25"        ,"26"         ,"27"           ,"28"           ,"29"   ,"30"         ,"31"      ,"32"           ,"33"           ,"34"       ,"35"              ,"36"            ,"37"    ,"38"      ,"39"          ,"40"     ,"41"     ,"42"     ,"43"     ,"44"     ,"45"     ,"46"     ,"47"     ,"48"     ,"49"      ,"50"      ,"51"      ,"52"      ,"53"      ,"54"      ,"55"        ,"56"        ,"57"             }//前台显示列号，从0开始计算,注意有选择框的是0
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

                #region 校验重复性
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
                #endregion

                FS0303_Logic.importSave(importDt, loginInfo.UserId);
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
