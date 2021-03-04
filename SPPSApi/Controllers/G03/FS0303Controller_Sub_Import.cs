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
                /*
                 * 时间：2020-01-27
                 * 修改人：董镇
                 * 修改内容：在导入时，如果只有品番也可以导入，所以去掉了品番以外的数据校验
                 */
                string strMsg = "";
                string[,] strField = new string[,] {{"ID"              ,"同步数据"     ,"变更事项","设变号" ,"生确"       ,"区分"  ,"补给品番"          ,"车型(设计)"  ,"车型(开发)"     ,"车名"         ,"使用开始"     ,"使用结束"     ,"切替实绩"     ,"SD"      ,"替代品番"     ,"英文品名"    ,"中文品名"    ,"号口工程","补给工程","内外"       ,"供应商代码"   ,"供应商名称"     ,"生产地"   ,"出荷地"   ,"包装工厂" ,"生产商名称","地址"       ,"开始"         ,"结束"         ,"OE=SP","品番(参考)" ,"号旧"    ,"旧型开始"     ,"旧型结束"     ,"旧型经年" ,"旧型年限生产区分","实施年限(年限)","特记"  ,"防锈"    ,"防锈指示书号","旧型1年","旧型2年","旧型3年","旧型4年","旧型5年","旧型6年","旧型7年","旧型8年","旧型9年","旧型10年","旧型11年","旧型12年","旧型13年","旧型14年","旧型15年","执行标准No","收货方"    ,"所属原单位"     ,"备注"     },
                                                    {"iAutoId"         ,"dSyncTime"    ,"vcChange","vcSPINo","vcSQContent","vcDiff","vcPart_id"         ,"vcCarTypeDev","vcCarTypeDesign","vcCarTypeName","dTimeFrom"    ,"dTimeTo"      ,"dTimeFromSJ"  ,"vcBJDiff","vcPartReplace","vcPartNameEn","vcPartNameCn","vcHKGC"  ,"vcBJGC"  ,"vcInOutflag","vcSupplier_id","vcSupplier_Name","vcSCPlace","vcCHPlace","vcSYTCode","vcSCSName" ,"vcSCSAdress","dGYSTimeFrom" ,"dGYSTimeTo"   ,"vcOE" ,"vcHKPart_id","vcHaoJiu","dJiuBegin"    ,"dJiuEnd"      ,"vcJiuYear","vcNXQF"          ,"dSSDate"       ,"vcMeno","vcFXDiff","vcFXNo"      ,"vcNum1" ,"vcNum2" ,"vcNum3" ,"vcNum4" ,"vcNum5" ,"vcNum6" ,"vcNum7" ,"vcNum8" ,"vcNum9" ,"vcNum10" ,"vcNum11" ,"vcNum12" ,"vcNum13" ,"vcNum14" ,"vcNum15" ,"vcZXBZNo"  ,"vcReceiver","vcOriginCompany","vcRemark"},
                                                    {FieldCheck.Num    ,FieldCheck.Date,""        ,""       ,""           ,""      ,""                  ,""            ,""               ,""             ,FieldCheck.Date,FieldCheck.Date,FieldCheck.Date,""        ,""             ,""            ,""            ,""        ,""        ,""           ,""             ,""               ,""         ,""         ,""         ,""          ,""           ,FieldCheck.Date,FieldCheck.Date,""     ,""           ,""        ,FieldCheck.Date,FieldCheck.Date,""         ,""                ,FieldCheck.Date ,""      ,""        ,""            ,""       ,""       ,""       ,""       ,""       ,""       ,""       ,""       ,""       ,""        ,""        ,""        ,""        ,""        ,""        ,""          ,""          ,""               ,""               },
                                                    {"0"               ,"0"            ,"0"       ,"20"     ,"0"          ,"1"     ,"14"                ,"4"           ,"4"              ,"10"           ,"0"            ,"0"            ,"0"            ,"4"       ,"14"           ,"100"         ,"100"         ,"50"      ,"50"      ,"0"          ,"4"            ,"100"            ,"20"       ,"20"       ,"0"        ,"100"       ,"100"        ,"0"            ,"0"            ,"0"    ,"14"         ,"0"       ,"0"            ,"0"            ,"4"        ,"20"              ,"0"             ,"200"   ,"2"       ,"12"          ,"4"      ,"4"      ,"4"      ,"4"      ,"4"      ,"4"      ,"4"      ,"4"      ,"4"      ,"4"       ,"4"       ,"4"       ,"4"       ,"4"       ,"4"       ,"100"        ,"10"        ,"0"              ,"0"              },//最大长度设定,不校验最大长度用0
                                                    {"0"               ,"0"            ,"0"       ,"0"      ,"0"          ,"0"     ,"11"                ,"0"           ,"0"              ,"0"            ,"0"            ,"0"            ,"0"            ,"0"       ,"0"            ,"0"           ,"0"           ,"0"       ,"0"       ,"0"          ,"0"            ,"0"              ,"0"        ,"0"        ,"0"        ,"0"         ,"0"          ,"0"            ,"0"            ,"0"    ,"0"          ,"0"       ,"0"            ,"0"            ,"0"        ,"0"               ,"0"             ,"0"     ,"0"       ,"0"           ,"0"      ,"0"      ,"0"      ,"0"      ,"0"      ,"0"      ,"0"      ,"0"      ,"0"      ,"0"       ,"0"       ,"0"       ,"0"       ,"0"       ,"0"       ,"0"         ,"1"         ,"1"              ,"0"              }//最小长度设定,可以为空用0
                                                    
                    };
                DataTable importDt = new DataTable();
                foreach (FileInfo info in theFolder.GetFiles())
                {
                    DataTable dt = ComFunction.ExcelToDataTable(info.FullName, "sheet1"     , strField, ref strMsg);

                    if (strMsg != "")
                    {
                        ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "导入终止，文件" + info.Name + ":" + strMsg;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }

                    #region 根据输入的Name获取对应的Value值，并将获取的Value值添加到dt的后面

                    #region 先定义哪些列涉及Name转Value

                    List<FS0303_Logic.NameOrValue> lists = new List<FS0303_Logic.NameOrValue>();
                    lists.Add(new FS0303_Logic.NameOrValue() { strTitle = "变更事项", strHeader = "vcChange", strCodeid = "C002", isNull = true });
                    lists.Add(new FS0303_Logic.NameOrValue() { strTitle = "内外", strHeader = "vcInOutflag", strCodeid = "C003", isNull = true });
                    lists.Add(new FS0303_Logic.NameOrValue() { strTitle = "号旧", strHeader = "vcHaoJiu", strCodeid = "C004", isNull = true });
                    lists.Add(new FS0303_Logic.NameOrValue() { strTitle = "收货方", strHeader = "vcReceiver", strCodeid = "C005", isNull = true });
                    lists.Add(new FS0303_Logic.NameOrValue() { strTitle = "所属原单位", strHeader = "vcOriginCompany", strCodeid = "C006", isNull = true });
                    lists.Add(new FS0303_Logic.NameOrValue() { strTitle = "OE=SP", strHeader = "vcOE", strCodeid = "C012", isNull = true });
                    lists.Add(new FS0303_Logic.NameOrValue() { strTitle = "包装工厂", strHeader = "vcSYTCode", strCodeid = "C016", isNull = true });
                    lists.Add(new FS0303_Logic.NameOrValue() { strTitle = "防锈", strHeader = "vcFXDiff", strCodeid = "C028", isNull = true });
                    #endregion

                    #region 更新table
                    string strErr = "";         //记录错误信息
                    dt = FS0303_Logic.ConverDT(dt, lists, ref strErr);
                    #endregion

                    #endregion

                    #region 根据输入的生确描述来判断生确状态
                    //1、添加生确状态列
                    dt.Columns.Add("vcSQState");
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        //2、获取生确描述
                        var varSQContent = dt.Rows[i]["vcSQState"];
                        //3、对生确描述进行判断
                        if (varSQContent == null)           
                        {   //3.1、如果为null，生确状态也设置位null
                            dt.Rows[i]["vcSQState"] = null;
                        }
                        else if (string.IsNullOrEmpty(varSQContent.ToString()))
                        {
                            //3.2、如果为空，生确状态设置为null
                            dt.Rows[i]["vcSQState"] = null;
                        }
                        else if (varSQContent.ToString().Contains("未确认"))
                        {
                            //文本包含未确认，生确状态设为为0
                            dt.Rows[i]["vcSQState"] = "0";
                        }
                        else if (varSQContent.ToString().Contains("确认中"))
                        {
                            //文本包含确认中，生确状态设为为1
                            dt.Rows[i]["vcSQState"] = "1";
                        }
                        else if (varSQContent.ToString().Contains("OK"))
                        {
                            //同上
                            dt.Rows[i]["vcSQState"] = "2";
                        }
                        else if (varSQContent.ToString().Contains("NG"))
                        {
                            //同上
                            dt.Rows[i]["vcSQState"] = "3";
                        }
                        else
                        {
                            //如果没有匹配到以上所有结果，消息提示
                            strErr += "第"+(i+2)+"行生确无法解析，请检查生确填写是否正确\n";
                        }
                    }

                    if (strErr!="")
                    {
                        ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = strErr;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }

                    #endregion

                    if (dt == null)
                    {
                        ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = strErr;
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
                string strErrorPartId="";
                FS0303_Logic.importSave(importDt, loginInfo.UserId,ref strErrorPartId);
                if (strErrorPartId != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "导入失败，以下品番重复：<br/>" + strErrorPartId;
                    apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "导入成功";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0905", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导入失败" + ex.Message;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

    }
}
