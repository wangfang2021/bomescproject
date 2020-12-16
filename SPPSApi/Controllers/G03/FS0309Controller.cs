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
    [Route("api/FS0309/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0309Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0309_Logic fs0309_Logic = new FS0309_Logic();
        private readonly string FunctionID = "FS0309";

        public FS0309Controller(IWebHostEnvironment webHostEnvironment)
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
                if (loginInfo.Special == "财务用户")
                    res.Add("caiWuBtnVisible", false);
                else
                    res.Add("caiWuBtnVisible", true);

                List<Object> dataList_C002 = ComFunction.convertAllToResult(ComFunction.getTCode("C002"));//变更事项
                List<Object> dataList_C004 = ComFunction.convertAllToResult(ComFunction.getTCode("C004"));//号旧区分
                List<Object> dataList_C003 = ComFunction.convertAllToResult(ComFunction.getTCode("C003"));//内外区分
                List<Object> dataList_C012 = ComFunction.convertAllToResult(ComFunction.getTCode("C012"));//OE=SP
                List<Object> dataList_C013 = ComFunction.convertAllToResult(ComFunction.getTCode("C013"));//状态
                //设变履历是否下拉待确定
                List<Object> dataList_C005 = ComFunction.convertAllToResult(ComFunction.getTCode("C005"));//收货方

                res.Add("C002", dataList_C002);
                res.Add("C004", dataList_C004);
                res.Add("C003", dataList_C003);
                res.Add("C012", dataList_C012);
                res.Add("C013", dataList_C013);
                res.Add("C005", dataList_C005);
                List<Object> dataList_GS = ComFunction.convertAllToResult(fs0309_Logic.getAllGS());//公式
                res.Add("optionGS", dataList_GS);

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

            string strChange = dataForm.Change;
            string strPart_id = dataForm.Part_id;
            string strOriginCompany = dataForm.OriginCompany;
            string strHaoJiu = dataForm.HaoJiu;
            string strProjectType = dataForm.ProjectType;
            if (loginInfo.Special == "财务用户")
                strProjectType = "内制";

            string strPriceChangeInfo = dataForm.PriceChangeInfo;
            string strCarTypeDev = dataForm.CarTypeDev;
            string strSupplier_id = dataForm.Supplier_id;
            string strReceiver = dataForm.Receiver;
            string strPriceState = dataForm.PriceState;

            try
            {
                DataTable dt = fs0309_Logic.Search(strChange, strPart_id, strOriginCompany, strHaoJiu
                    , strProjectType, strPriceChangeInfo, strCarTypeDev, strSupplier_id
                    , strReceiver, strPriceState
                    );
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("dUseBegin", ConvertFieldType.DateType, "yyyy-MM-dd");
                dtConverter.addField("dUseEnd", ConvertFieldType.DateType, "yyyy-MM-dd");
                dtConverter.addField("dProjectBegin", ConvertFieldType.DateType, "yyyy-MM-dd");
                dtConverter.addField("dProjectEnd", ConvertFieldType.DateType, "yyyy-MM-dd");
                dtConverter.addField("dJiuBegin", ConvertFieldType.DateType, "yyyy-MM-dd");
                dtConverter.addField("dJiuEnd", ConvertFieldType.DateType, "yyyy-MM-dd");
                dtConverter.addField("dJiuBeginSustain", ConvertFieldType.DateType, "yyyy-MM-dd");
                dtConverter.addField("dPriceSendDate", ConvertFieldType.DateType, "yyyy-MM-dd");
                dtConverter.addField("dPricebegin", ConvertFieldType.DateType, "yyyy-MM-dd");
                dtConverter.addField("dPriceEnd", ConvertFieldType.DateType, "yyyy-MM-dd");
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

            string strChange = dataForm.Change;
            string strPart_id = dataForm.Part_id;
            string strOriginCompany = dataForm.OriginCompany;
            string strHaoJiu = dataForm.HaoJiu;
            string strProjectType = dataForm.ProjectType;
            string strPriceChangeInfo = dataForm.PriceChangeInfo;
            string strCarTypeDev = dataForm.CarTypeDev;
            string strSupplier_id = dataForm.Supplier_id;
            string strReceiver = dataForm.Receiver;
            string strPriceState = dataForm.PriceState;
            try
            {
                DataTable dt = fs0309_Logic.Search(strChange, strPart_id, strOriginCompany, strHaoJiu
                   , strProjectType, strPriceChangeInfo, strCarTypeDev, strSupplier_id
                   , strReceiver, strPriceState
                   );
                string[] fields = { "vcChange", "vcPart_id", "dUseBegin", "dUseEnd", "vcProjectType", "vcSupplier_id"
                ,"vcSupplier_Name","dProjectBegin","dProjectEnd","vcHaoJiu","dJiuBegin","dJiuEnd","dJiuBeginSustain","vcPriceChangeInfo"
                ,"vcPriceState","dPriceSendDate","vcPriceGS","decPriceOrigin","decPriceAfter","decPriceTNPWithTax","dPricebegin","dPriceEnd"
                ,"vcCarTypeDev","vcCarTypeDesign","vcPart_Name","vcOE","vcPart_id_HK","vcStateFX","vcFXNO","vcSumLater","vcReceiver"
                ,"vcOriginCompany"
                };
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0309.xlsx", 2,loginInfo.UserId,FunctionID  );
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = filepath;
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


        #region 保存
        [HttpPost]
        [EnableCors("any")]
        public string saveApi([FromBody]dynamic data)
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
            try
            {
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                JArray listInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
                bool hasFind = false;//是否找到需要新增或者修改的数据
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    if (bAddFlag == true)
                    {//新增
                        hasFind = true;
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        hasFind = true;
                    }
                }
                //开始数据验证
                if (hasFind)
                {
                    string[,] strField = new string[,] {{"变更事项","品番","使用开始","使用结束","内外","供应商代码","供应商名称","开始","结束","号旧"},
                                                {"vcChange","vcPart_id","dUseBegin","dUseEnd","vcProjectType","vcSupplier_id","vcSupplier_Name","dProjectBegin","dProjectEnd","vcHaoJiu"},
                                                {"",FieldCheck.NumChar,FieldCheck.Date,FieldCheck.Date,"","","",FieldCheck.Date,FieldCheck.Date,"" },
                                                {"25","12","0","0","0","4","50","0","0","0"},//最大长度设定,不校验最大长度用0
                                                {"1","10","1","1","1","1","1","1","1","1"},//最小长度设定,可以为空用0
                                                {"1","2","3","4","5","6","7","8","9","10"}//前台显示列号，从0开始计算,注意有选择框的是0
                    };
                    List<Object> checkRes = ListChecker.validateList(listInfoData, strField, true);
                    if (checkRes != null)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = checkRes;
                        apiResult.flag = 1;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                if (!hasFind)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少有一个编辑行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fs0309_Logic.Save(listInfoData, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0902", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 删除
        [HttpPost]
        [EnableCors("any")]
        public string delApi([FromBody]dynamic data)
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
            try
            {
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                JArray checkedInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();
                if (listInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选择一条数据！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fs0309_Logic.Del(listInfoData, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0903", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

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
                string[,] headers = new string[,] {{"品番","使用开始","使用结束","变更履历", "公式选择", "原价","价格开始","价格结束"},
                                                {"vcPart_id", "dUseBegin", "dUseEnd", "vcPriceChangeInfo","vcPriceGS","decPriceOrigin","dPricebegin","dPriceEnd"},
                                                {FieldCheck.NumCharLLL,FieldCheck.Date,FieldCheck.Date,"","",FieldCheck.Decimal,FieldCheck.Date,FieldCheck.Date},
                                                {"12","0","0","50", "50", "0", "0", "0"},//最大长度设定,不校验最大长度用0
                                                {"1","1","1","0", "0", "0", "0", "0"}};//最小长度设定,可以为空用0
                DataTable importDt = new DataTable();
                foreach (FileInfo info in theFolder.GetFiles()) {
                    DataTable dt=ComFunction.ExcelToDataTable(info.FullName, "sheet1", headers, ref strMsg);
                    if (strMsg != "")
                    {
                        ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "导入终止，文件"+ info .Name+":"+ strMsg;
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
                             group r by new { r2 = r.Field<string>("vcPart_id"), r3 = r.Field<string>("dUseBegin"), r4 = r.Field<string>("dUseEnd") } into g
                             where g.Count() > 1
                             select g;
                if (result.Count() > 0)
                {
                    StringBuilder sbr = new StringBuilder();
                    sbr.Append("导入数据重复:<br/>");
                    foreach (var item in result)
                    {
                        sbr.Append("品番:"+item.Key.r2 + " 使用开始:" + item.Key.r3 + " 使用结束:" + item.Key.r4+ "<br/>");
                    }
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = sbr.ToString();
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }



                fs0309_Logic.importSave(importDt, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "保存成功";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComFunction.DeleteFolder(fileSavePath);//读取异常则，删除文件夹，全部重新上传
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0905", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败"+ ex.Message;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}
