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
    [Route("api/FS0309_Sub_Import/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0309Controller_Sub_Import : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0309_Logic fs0309_Logic = new FS0309_Logic();
        private readonly string FunctionID = "FS0309";

        public FS0309Controller_Sub_Import(IWebHostEnvironment webHostEnvironment)
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
                string[,] headers = new string[,] {{"ID","品番","使用开始","使用结束","变更履历", "公式选择", "原价","参考值","TNP含税","价格开始","价格结束"},
                                                {"iAutoId","vcPart_id", "dUseBegin", "dUseEnd", "vcPriceChangeInfo","vcPriceGS","decPriceOrigin","decPriceAfter","decPriceTNPWithTax","dPricebegin","dPriceEnd"},
                                                {"",FieldCheck.NumCharLLL,FieldCheck.Date,FieldCheck.Date,"","",FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Decimal,FieldCheck.Date,FieldCheck.Date},
                                                {"0","12","0","0","50", "50", "0","0","0", "0", "0"},//最大长度设定,不校验最大长度用0
                                                {"1","1","1","1","0", "0", "0","0","0", "0", "0"}};//最小长度设定,可以为空用0
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

                for (int i = 0; i < importDt.Rows.Count; i++)
                {
                    string strAutoId = importDt.Rows[i]["iAutoId"] == System.DBNull.Value ? "" : importDt.Rows[i]["iAutoId"].ToString();
                    string strPart_id = importDt.Rows[i]["vcPart_id"] == System.DBNull.Value ? "" : importDt.Rows[i]["vcPart_id"].ToString();
                    string strPriceTNPWithTax = importDt.Rows[i]["decPriceTNPWithTax"]==System.DBNull.Value?"": importDt.Rows[i]["decPriceTNPWithTax"].ToString();
                    string strPricebegin = importDt.Rows[i]["dPricebegin"] == System.DBNull.Value ? "" : importDt.Rows[i]["dPricebegin"].ToString();
                    string strPriceEnd = importDt.Rows[i]["dPriceEnd"] == System.DBNull.Value ? "" : importDt.Rows[i]["dPriceEnd"].ToString();
                    if (strPriceTNPWithTax != "" && (strPricebegin == "" || strPriceEnd == ""))
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "ID:" + strAutoId +" 品番:"+ strPart_id + " TNP含税不为空时，价格开始、价格结束不能为空！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (strPricebegin != "" && strPriceEnd != "")
                    {
                        DateTime dbegin = DateTime.Parse(strPricebegin);
                        DateTime dend = DateTime.Parse(strPriceEnd);
                        if (dbegin > dend)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "ID:" + strAutoId + " 品番:" + strPart_id + " 价格开始时间不能晚于价格结束时间！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                    }
                }



                FS0309_Logic fS0309_Logic = new FS0309_Logic();

                List<FS0309_Logic.NameOrValue> lists = new List<FS0309_Logic.NameOrValue>();
                lists.Add(new FS0309_Logic.NameOrValue() { strTitle = "公式选择", strHeader = "vcPriceGS", strCodeid = "C038", isNull = true });
                string strErr = "";         //记录错误信息
                importDt = fS0309_Logic.ConverDT(importDt, lists, ref strErr);

                string strErrorPartId = "";
                fs0309_Logic.importSave(importDt, loginInfo.UserId, ref strErrorPartId);
                if (strErrorPartId != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "保存失败，以下品番价格开始、价格结束区间存在重叠：<br/>" + strErrorPartId;
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
