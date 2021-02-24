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
    [Route("api/FS0312_Sub_Import/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0312Controller_Sub_Import : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0312_Logic FS0312_Logic = new FS0312_Logic();
        private readonly string FunctionID = "FS0312";

        public FS0312Controller_Sub_Import(IWebHostEnvironment webHostEnvironment)
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
                string[,] headers = new string[,] {{"品番"               ,"号旧"    ,"旧型开始时间" ,"供应商代码"   ,"供应商名称"     ,"车型(开发代码)" ,"品名"      ,"旧型1年"     ,"旧型2年"     ,"旧型3年"     ,"旧型4年"     ,"旧型5年"     ,"旧型6年"     ,"旧型7年"     ,"旧型8年"     ,"旧型9年"     ,"旧型10年"     ,"送信时间"     },
                                                   {"vcPart_id"          ,"vcHaoJiu","dJiuBegin"    ,"vcSupplier_id","vcSupplier_Name","vcCarTypeDesign","vcPartName","vcNum1"      ,"vcNum2"      ,"vcNum3"      ,"vcNum4"      ,"vcNum5"      ,"vcNum6"      ,"vcNum7"      ,"vcNum8"      ,"vcNum9"      ,"vcNum10"      ,"dSendTime"    },
                                                   {FieldCheck.NumCharLLL,""        ,FieldCheck.Date,""             ,"100"            ,""               ,""          ,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num,FieldCheck.Num ,FieldCheck.Date},
                                                   {"12"                 ,"0"       ,"0"            ,"4"            ,"50"             ,"4"              , "100"      ,"5"           ,"5"           ,"5"           ,"5"           ,"5"           ,"5"           ,"5"           ,"5"           ,"5"           ,"5"            ,"0"            },//最大长度设定,不校验最大长度用0
                                                   {"1"                  ,"0"       ,"1"            ,"1"            ,"1"              ,"1"              , "1"        ,"0"           ,"0"           ,"0"           ,"0"           ,"0"           ,"0"           ,"0"           ,"0"           ,"0"           ,"0"            ,"0"            }};//最小长度设定,可以为空用0
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

                    #region 根据输入的Name获取对应的Value值，并将获取的Value值添加到dt的后面

                    #region 先定义哪些列涉及Name转Value

                    List<FS0312_Logic.NameOrValue> lists = new List<FS0312_Logic.NameOrValue>();
                    lists.Add(new FS0312_Logic.NameOrValue() { strTitle = "号旧", strHeader = "vcHaoJiu", strCodeid = "C004", isNull = true });
                    #endregion

                    #region 更新table
                    string strErr = "";         //记录错误信息
                    dt = FS0312_Logic.ConverDT(dt, lists, ref strErr);
                    #endregion

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


                var result = from r in importDt.AsEnumerable()
                             group r by new { r2 = r.Field<string>("vcPart_id"), r3 = r.Field<string>("vcHaoJiu"), r4 = r.Field<string>("vcCarTypeDesign") } into g
                             where g.Count() > 1
                             select g;
                if (result.Count() > 0)
                {
                    StringBuilder sbr = new StringBuilder();
                    sbr.Append("导入数据重复:<br/>");
                    foreach (var item in result)
                    {
                        sbr.Append("品番:" + item.Key.r2 + " 号旧:" + item.Key.r3 + " 车型(开发代码):" + item.Key.r4 + "<br/>");
                    }
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = sbr.ToString();
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                string strErrorPartId = "";
                FS0312_Logic.importSave(importDt, loginInfo.UserId, ref strErrorPartId);
                if (strErrorPartId != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "保存失败，以下品番使用开始、结束区间存在重叠：<br/>" + strErrorPartId;
                    apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
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
