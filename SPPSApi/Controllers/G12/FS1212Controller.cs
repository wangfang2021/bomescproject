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
    [Route("api/FS1212/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1212Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS1212_Logic logic = new FS1212_Logic();
        private readonly string FunctionID = "FS1212";

        public FS1212Controller(IWebHostEnvironment webHostEnvironment)
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
                Dictionary<string, Object> res = new Dictionary<string, Object>();
                List<Object> dataList_ProTypeSource = ComFunction.convertAllToResult(logic.dllPorType(""));
                List<Object> dataList_ZBSource = ComFunction.convertAllToResult(logic.dllZB(""));
                List<Object> dataList_LBSource = ComFunction.convertAllToResult(logic.dllLB1(""));
                List<Object> dataList_PartFrequenceSource = ComFunction.convertAllToResult(logic.ddlPartFrequence());
                res.Add("ProTypeSource", dataList_ProTypeSource);
                res.Add("ZBSource", dataList_ZBSource);
                res.Add("LBSource", dataList_LBSource);
                res.Add("PartFrequenceSource", dataList_PartFrequenceSource);
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

            string vcPartsNo = dataForm.vcPartsNo;
            vcPartsNo = vcPartsNo == null ? "" : vcPartsNo.Replace("-", "").ToString();
            string vcPorType = dataForm.vcPorType;
            string vcCarFamilyCode = dataForm.vcCarFamilyCode;
            string vcZB = dataForm.vcZB;
            string vcLB = dataForm.vcLB;
            string vcPartFrequence = dataForm.vcPartFrequence;
            try
            {
                DataTable dt = logic.SearchPartData(vcPartsNo, vcCarFamilyCode, vcPorType, vcZB, vcLB, vcPartFrequence);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
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

        #region 保存
        [HttpPost]
        [EnableCors("any")]
        public string saveApi([FromBody] dynamic data)
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
                if (!hasFind)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少有一个编辑行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //开始数据验证
                //if (hasFind)
                //{
                //    string[,] strField = new string[,] {{"变更事项","品番","使用开始","使用结束","内外","供应商代码","供应商名称","开始","结束","号旧"},
                //                                {"vcChange","vcPart_id","dUseBegin","dUseEnd","vcProjectType","vcSupplier_id","vcSupplier_Name","dProjectBegin","dProjectEnd","vcHaoJiu"},
                //                                {"",FieldCheck.NumChar,FieldCheck.Date,FieldCheck.Date,"","","",FieldCheck.Date,FieldCheck.Date,"" },
                //                                {"25","12","0","0","0","4","50","0","0","0"},//最大长度设定,不校验最大长度用0
                //                                {"1","10","1","1","1","1","1","1","1","1"},//最小长度设定,可以为空用0
                //                                {"1","2","3","4","5","6","7","8","9","10"}//前台显示列号，从0开始计算,注意有选择框的是0
                //    };
                //    //需要判断时间区间先后关系的字段
                //    string[,] strDateRegion = { { "dUseBegin", "dUseEnd" }, { "dProjectBegin", "dProjectEnd" }, { "dJiuBegin", "dJiuEnd" }, { "dPricebegin", "dPriceEnd" } };
                //    string[,] strSpecialCheck = { //例子-变更事项字段，当它为新设时，号旧必须为号口，旧型开始、旧型结束、旧型持续开始必须为空
                //        { "变更事项","vcChange", "新设","1", "号旧","vcHaoJiu","1", "号口", "H" },
                //        { "变更事项","vcChange", "旧型","3", "号旧","vcHaoJiu","1", "旧型", "Q" },
                //        { "变更事项","vcChange", "新设","1", "旧型开始","dJiuBegin","0", "空","" },
                //        { "变更事项","vcChange", "新设","1", "旧型结束","dJiuEnd","0", "空","" },
                //        { "变更事项","vcChange", "新设","1", "旧型持续开始","dJiuBeginSustain","0", "空","" },
                //        { "变更事项","vcChange", "旧型","3", "旧型开始","dJiuBegin","1", "空","" },
                //        { "变更事项","vcChange", "旧型","3", "旧型结束","dJiuEnd","1", "空","" },
                //        { "变更事项","vcChange", "旧型","3", "旧型持续开始","dJiuBeginSustain","1", "空","" }
                //    };

                //    List<Object> checkRes = ListChecker.validateList(listInfoData, strField, strDateRegion, strSpecialCheck, true, "FS0309");
                //    if (checkRes != null)
                //    {
                //        apiResult.code = ComConstant.ERROR_CODE;
                //        apiResult.data = checkRes;
                //        apiResult.flag = Convert.ToInt32(ERROR_FLAG.单元格定位提示);
                //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                //    }
                //}
                string strErrorPartId = "";
                int j = logic.Save(listInfoData, loginInfo.UserId, ref strErrorPartId);
                if (j > 0)
                {
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.data = null;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败，以下品番使用开始、结束区间存在重叠：<br/>" + strErrorPartId;
                apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
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
        public string delApi([FromBody] dynamic data)
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
                logic.Del(listInfoData, loginInfo.UserId);
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

            string vcPartsNo = dataForm.vcPartsNo;
            vcPartsNo = vcPartsNo == null ? "" : vcPartsNo.Replace("-", "").ToString();
            string vcPorType = dataForm.vcPorType;
            string vcCarFamilyCode = dataForm.vcCarFamilyCode;
            string vcZB = dataForm.vcZB;
            string vcLB = dataForm.vcLB;
            string vcPartFrequence = dataForm.vcPartFrequence;
            try
            {
                DataTable dt = logic.SearchPartData(vcPartsNo, vcCarFamilyCode, vcPorType, vcZB, vcLB, vcPartFrequence);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dt.Rows[i]["vcPartsNo"] = dt.Rows[i]["vcPartsNo"].ToString().Replace("-", "");
                    if (dt.Rows[i]["vcPartPlant"] != null && dt.Rows[i]["vcPartPlant"].ToString().Trim() != "")
                        dt.Rows[i]["vcPartPlant"] = "#" + dt.Rows[i]["vcPartPlant"].ToString();
                    if (dt.Rows[i]["vcQFflag"] != null && dt.Rows[i]["vcQFflag"].ToString() == "1")
                        dt.Rows[i]["vcQFflag"] = "○";
                    else if (dt.Rows[i]["vcQFflag"] != null && dt.Rows[i]["vcQFflag"].ToString() == "2")
                        dt.Rows[i]["vcQFflag"] = "×";
                }
                string[] fields = { "vcPartsNo","dFromTime","dToTime","vcPartPlant","vcDock","vcCarFamilyCode","vcPartENName",
                                    "vcQFflag","iQuantityPerContainer","vcQJcontainer","vcPorType","vcZB","vcOrderingMethod","vcReceiver","vcSupplierId" };
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS1212_Export.xlsx", 1, loginInfo.UserId, FunctionID);
                if (filepath == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "导出生成文件失败";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
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

        #region 联动取得组别
        [HttpPost]
        [EnableCors("any")]
        public string getZB([FromBody] dynamic data)
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
            string vcPorType = dataForm.vcPorType == null ? "" : dataForm.vcPorType;
            try
            {
                Dictionary<string, Object> res = new Dictionary<string, Object>();
                DataTable dt = logic.dllZB(vcPorType);
                List<Object> dataList_ZB = ComFunction.convertAllToResult(dt);
                res.Add("ZBSource", dataList_ZB);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
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
    }
}
