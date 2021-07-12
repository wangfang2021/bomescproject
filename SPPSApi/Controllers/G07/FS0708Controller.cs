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

namespace SPPSApi.Controllers.G07
{
    [Route("api/FS0708/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0708Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0708_Logic FS0708_Logic = new FS0708_Logic();
        private readonly string FunctionID = "FS0708";

        public FS0708Controller(IWebHostEnvironment webHostEnvironment)
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
                FS0701_Logic FS0701_Logic = new FS0701_Logic();
                List<Object> dataList_C023 = ComFunction.convertAllToResult(FS0701_Logic.SearchPackSpot(loginInfo.UserId));//包装场
                res.Add("C023", dataList_C023);

                List<Object> dataList_Supplier = ComFunction.convertAllToResult(FS0708_Logic.SearchSupplier());//供应商
                res.Add("optionSupplier", dataList_Supplier);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M07UE0801", ex, loginInfo.UserId);
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

            List<Object> PackSpot = new List<object>();

            if (dataForm.PackSpot.ToObject<List<Object>>() == null)
            {
                PackSpot = new List<object>();
            }
            else
            {
                PackSpot = dataForm.PackSpot.ToObject<List<Object>>();
            }
            string OrderFrom = dataForm.OrderFrom;
            string OrderTo = dataForm.OrderTo;
            string PackNo = dataForm.PackNO;
            string PackGPSNo = dataForm.PackGPSNo;
            List<Object> Type = new List<object>();
            if (dataForm.Type.ToObject<List<Object>>() == null)
            {
                Type = new List<object>();
            }
            else
            {
                Type = dataForm.Type.ToObject<List<Object>>();
            }
            List<Object> OrderState = new List<object>();

            if (dataForm.OrderState.ToObject<List<Object>>() == null)
            {
                OrderState = new List<object>();
            }
            else
            {
                OrderState = dataForm.OrderState.ToObject<List<Object>>();
            }

            string IsQianPin = dataForm.IsQianPin;
            List<Object> strSupplierCode = new List<object>();

            if (dataForm.SupplierName.ToObject<List<Object>>() == null)
            {
                strSupplierCode = new List<object>();
            }
            else
            {
                strSupplierCode = dataForm.SupplierName.ToObject<List<Object>>();
            }
            string ZuCode = dataForm.ZuCode;
            string dFaZhuFrom = dataForm.dFaZhuFrom;
            string dFaZhuTo = dataForm.dFaZhuTo;
            string dNaQiFrom = dataForm.dNaQiFrom;
            string dNaQiTo = dataForm.dNaQiTo;
            string dNaRuFrom = dataForm.dNaRuFrom;
            string dNaRuTo = dataForm.dNaRuTo;


            try
            {
                BatchProcess.FP0015 fp0015 = new BatchProcess.FP0015();

                if (!fp0015.main(loginInfo.UserId))
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "同步更新失败";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                DataTable dt = FS0708_Logic.Search(PackSpot, PackNo, PackGPSNo, OrderFrom, OrderTo, Type, OrderState, IsQianPin, strSupplierCode, ZuCode, dFaZhuFrom, dFaZhuTo, dNaQiFrom, dNaQiTo, dNaRuFrom, dNaRuTo);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("dFaZhuTime", ConvertFieldType.DateType, "yyyy-MM-dd HH:mm:ss");
                dtConverter.addField("dNaRuYuDing", ConvertFieldType.DateType, "yyyy-MM-dd HH:mm:ss");
                dtConverter.addField("dNaRuShiJi", ConvertFieldType.DateType, "yyyy-MM-dd HH:mm:ss");
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M07UE0802", ex, loginInfo.UserId);
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
            List<Object> PackSpot = new List<object>();

            if (dataForm.PackSpot.ToObject<List<Object>>() == null)
            {
                PackSpot = new List<object>();
            }
            else
            {
                PackSpot = dataForm.PackSpot.ToObject<List<Object>>();
            }
            string OrderFrom = dataForm.OrderFrom;
            string OrderTo = dataForm.OrderTo;
            string PackNo = dataForm.PackNo;
            string PackGPSNo = dataForm.PackGPSNo;
            List<Object> Type = new List<object>();
            if (dataForm.Type.ToObject<List<Object>>() == null)
            {
                Type = new List<object>();
            }
            else
            {
                Type = dataForm.Type.ToObject<List<Object>>();
            }
            List<Object> OrderState = new List<object>();

            if (dataForm.OrderState.ToObject<List<Object>>() == null)
            {
                OrderState = new List<object>();
            }
            else
            {
                OrderState = dataForm.OrderState.ToObject<List<Object>>();
            }

            string IsQianPin = dataForm.IsQianPin;
            List<Object> strSupplierCode = new List<object>();

            if (dataForm.SupplierName.ToObject<List<Object>>() == null)
            {
                strSupplierCode = new List<object>();
            }
            else
            {
                strSupplierCode = dataForm.SupplierName.ToObject<List<Object>>();
            }
            string ZuCode = dataForm.ZuCode;
            string dFaZhuFrom = dataForm.dFaZhuFrom;
            string dFaZhuTo = dataForm.dFaZhuTo;
            string dNaQiFrom = dataForm.dNaQiFrom;
            string dNaQiTo = dataForm.dNaQiTo;
            string dNaRuFrom = dataForm.dNaRuFrom;
            string dNaRuTo = dataForm.dNaRuTo;

            try
            {
                DataTable dt = FS0708_Logic.Search(PackSpot, PackNo, PackGPSNo, OrderFrom, OrderTo, Type, OrderState, IsQianPin, strSupplierCode, ZuCode, dFaZhuFrom, dFaZhuTo, dNaQiFrom, dNaQiTo, dNaRuFrom, dNaRuTo);

                if (dt.Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "没有可导出数据！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }
                string resMsg = "";
                string[] head = {"包装场","订单号","包材品番","GPS品番","订购数量","实纳数量","发注类型","发注时间","预定纳入时间","纳入便次","实际纳入时间","纳入状态","供应商代码","费用负担部署","仓库代码" };

                string[] fields = { "vcPackSpot","vcOrderNo","vcPackNo","vcPackGPSNo", "iOrderNumber","iSJNumber","vcType",
                    "dFaZhuTime","dNaRuYuDing","vcNaRuBianCi","dNaRuShiJi","vcState","vcPackSupplierID",
                    "vcFeiYongID","vcCangKuID"
                };
                string filepath = ComFunction.DataTableToExcel(head, fields, dt, _webHostEnvironment.ContentRootPath, loginInfo.UserId, "包材订单导出", ref resMsg);

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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M07UE0803", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
        //导出列值应该显示名字

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
                    Regex regex = new System.Text.RegularExpressions.Regex("^(-?[0-9]*[.]*[0-9]{0,3})$");
                    bool b = regex.IsMatch(listInfoData[i]["iRelease"].ToString());
                    if (!b)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "请填写正常的发住收容数格式！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                if (!hasFind)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少有一个编辑行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                string strErrorPartId = "";
                FS0708_Logic.Save(listInfoData, loginInfo.UserId, ref strErrorPartId);
                if (strErrorPartId != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "保存失败";
                    apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M07UE0804", ex, loginInfo.UserId);
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
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (listInfoData[i]["vcType"].ToString() == "自动发注"|| listInfoData[i]["dFaZhuTime"]!=null)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "不可删除此数据！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }

                FS0708_Logic.Del(listInfoData, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M07UE0805", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion


    }
}
