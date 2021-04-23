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

namespace SPPSApi.Controllers.G07
{
    [Route("api/FS0705/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0705Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0705_Logic fs0705_Logic = new FS0705_Logic();
        private readonly string FunctionID = "FS0705";

        public FS0705Controller(IWebHostEnvironment webHostEnvironment)
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

                bool IsUserDisabled = false;

                #region 获取当前用户能否使用计算过程检索和调整数据输入按钮
                IsUserDisabled = fs0705_Logic.getUserDisable(loginInfo.UserId);
                #endregion

                res.Add("btnUserDisabled", IsUserDisabled);

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

        #region 发注便次更新
        [HttpPost]
        [EnableCors("any")]
        public string searchFaZhuTimeApi([FromBody] dynamic data)
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
            string PackSpot = loginInfo.BaoZhuangPlace;
            try
            {
                ArrayList list = fs0705_Logic.SearchFaZhuTime(PackSpot);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = list;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M07UE0501", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "发注便次更新失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 发注数量计算
        [HttpPost]
        [EnableCors("any")]
        public string computeApi([FromBody] dynamic data)
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
            Dictionary<string, object> res = new Dictionary<string, object>();

            try
            {
                JArray listInfo = dataForm.multipleSelection;
                if (listInfo == null || listInfo.Count <= 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请先选择发注便次";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();

                List<string> lists = new List<string>();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    lists.Add(listInfoData[i]["strFaZhuID"].ToString());
                }
                string strFaZhuID = lists.Distinct().ToList()[0];


                /*
                 * 添加校验此次补给品番是否维护包材构成
                 * 是否维护：有包材构成数据，并且包材构成数据是有效的(开始时间<=当前时间 and 当前时间<=结束时间)
                 * 数据源：部品入库表，时间段取上次计算结束时间到(当前时间包材自动发注对应的入荷结束时间)
                 * 包材构成表：TPackItem
                 */
                #region 验证补给品番是否存在有效的包材构成
                string[] strArray = fs0705_Logic.getPackCheckDT(strFaZhuID,loginInfo.BaoZhuangPlace);

                string strErr1 = "";
                string strErr2 = "";

                if (strArray[0] != "")
                {
                    foreach (var item in strArray[0].Split(","))
                    {
                        strErr1 += item + "<br/>";
                    }
                    res.Add("errPart", "发注数量计算失败,以下品番无包材构成！"+ "<br/>" + strErr1);
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.flag = 1;
                    apiResult.data = res;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (strArray[1] != "")
                {
                    foreach (var item in strArray[1].Split(","))
                    {
                        strErr2 += item + "<br/>";
                    }
                    res.Add("errPart",  "发注数量计算失败,以下品番包材构成无效！"+ "<br/>" + strErr2);
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.flag = 1;
                    apiResult.data = res;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                #endregion

                fs0705_Logic.computer(strFaZhuID, loginInfo.UserId, loginInfo.BaoZhuangPlace);

                #region 计算完毕检索计算结果
                DataTable computeJGDT = fs0705_Logic.searchComputeJG(loginInfo.BaoZhuangPlace);

                if (computeJGDT.Rows.Count<=0)
                {
                    //计算成功，本次计算订购数量为0
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.flag = 2;
                    apiResult.data = "计算成功，本次计算订购数量为0";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                DtConverter dtConverter = new DtConverter();

                dtConverter.addField("dTimeStr", ConvertFieldType.DateType, "yyyy/MM/dd hh:mm:ss");
                List<Object> dataList = ComFunction.convertAllToResultByConverter(computeJGDT, dtConverter);
                #endregion

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M07UE0502", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "发注数量计算失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 生成发注数据
        public string sendFaZhuDataApi([FromBody] dynamic data)
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
                //获取最新的订单号
                string strOrderNo = fs0705_Logic.getNewOrderNo();
                //生成发注数据
                /*
                 * 查询出计算结果中订单号为空的数据(订单号为空，说明没有生成发注数据),并且计算出的订购数量大于0(等于0不需要订购)
                 */
                DataTable JGDT = fs0705_Logic.SCFZDataSearchComputeJG(loginInfo.BaoZhuangPlace);
                fs0705_Logic.SCFZData(JGDT, strOrderNo,loginInfo.UserId);


                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "生成发注数据成功";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M07UE0503", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "生成发注数据失败";
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
                DataTable dt = fs0705_Logic.searchComputeJG(loginInfo.BaoZhuangPlace);
                if (dt.Rows.Count<=0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "没有找到需要订购的品番信息";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string[] fields = { "vcFaZhuID","vcPackNo","vcPackGPSNo","iF_DingGou","vcBianCi","dTimeStr"};
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0705_Export.xlsx", 2, loginInfo.UserId, FunctionID);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M07UE0505", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}
