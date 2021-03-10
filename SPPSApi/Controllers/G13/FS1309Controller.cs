using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Threading;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SPPSApi.Controllers.G13
{
    [Route("api/FS1309/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1309Controller : BaseController
    {
        FS0603_Logic fs0603_Logic = new FS0603_Logic();
        FS1309_Logic fS1309_Logic = new FS1309_Logic();
        private readonly string FunctionID = "FS1309";

        private readonly IWebHostEnvironment _webHostEnvironment;
        public FS1309Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        /// <summary>
        /// 页面初始化
        /// </summary>
        /// <returns></returns>
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
                //获取登录人包装厂
                DataTable dtPackingPlant = ComFunction.getTCode("C023");
                if (dtPackingPlant == null || dtPackingPlant.Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "e1";
                    apiResult.data = null;
                    //apiResult.data = "包装厂信息不全请维护";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //包装厂下拉
                List<Object> PackingPlantList = ComFunction.convertAllToResult(dtPackingPlant);//包装厂
                res.Add("PackingPlantList", PackingPlantList);
                string strPackingPlant = dtPackingPlant.Rows[0]["vcValue"].ToString();
                int code = 0;
                string type = "";
                res = fS1309_Logic.setLoadPage(res, strPackingPlant, ref type, ref code);
                apiResult.code = code;
                apiResult.type = type;
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
        /// <summary>
        /// 刷新方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string selectApi([FromBody]dynamic data)
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

            string strPackingPlant = dataForm.selectVaule == null ? "" : dataForm.selectVaule;
            try
            {
                Dictionary<string, object> res = new Dictionary<string, object>();
                int code = 0;
                string type = "";
                res = fS1309_Logic.setLoadPage(res, strPackingPlant, ref type, ref code);
                apiResult.code = code;
                apiResult.type = type;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0204", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }

        /// <summary>
        /// 保存方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string saveApi([FromBody]dynamic data)
        {
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            //以下开始业务处理
            ApiResult apiResult = new ApiResult();
            Dictionary<string, object> res = new Dictionary<string, object>();
            dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));

            string strPackingPlant = dataForm.PackingPlant == null ? "" : dataForm.PackingPlant;
            string strPageClientNum = dataForm.PageClientNum == null ? "" : dataForm.PageClientNum;
            string strGZTTongjiFre = dataForm.GZTTongjiFre == null ? "" : dataForm.GZTTongjiFre;
            string strBZLTongjiFre = dataForm.BZLTongjiFre == null ? "" : dataForm.BZLTongjiFre;
            string strGZTZhuangTaiFre = dataForm.GZTZhuangTaiFre == null ? "" : dataForm.GZTZhuangTaiFre;
            string strGZTQieHuanFre = dataForm.GZTQieHuanFre == null ? "" : dataForm.GZTQieHuanFre;
            string strGZTShowType = dataForm.GZTShowType == null ? "" : dataForm.GZTShowType;
            string strObjective = dataForm.Objective == null ? "" : dataForm.Objective;
            string strBFromTime = dataForm.BFromTime == null ? "" : dataForm.BFromTime;
            string strBCross = dataForm.BCross == null ? "" : dataForm.BCross;
            string strBToTime = dataForm.BToTime == null ? "" : dataForm.BToTime;
            string strYFromTime = dataForm.YFromTime == null ? "" : dataForm.YFromTime;
            string strYCross = dataForm.YCross == null ? "" : dataForm.YCross;
            string strYToTime = dataForm.YToTime == null ? "" : dataForm.YToTime;
            try
            {
                DataTable dtMessage = fs0603_Logic.createTable("MES");
                if (strPackingPlant == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "包装工厂不能为空。";
                    dtMessage.Rows.Add(dataRow);
                }
                if (strGZTTongjiFre == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "工作台统计刷新不能为空。";
                    dtMessage.Rows.Add(dataRow);
                }
                else
                {
                    if (Convert.ToInt32(strGZTTongjiFre) < 60)
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = "工作台统计刷新时间不能小于60s。";
                        dtMessage.Rows.Add(dataRow);

                    }
                }
                if (strBZLTongjiFre == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "包装量统计刷新不能为空。";
                    dtMessage.Rows.Add(dataRow);
                }
                else
                {
                    if (Convert.ToInt32(strBZLTongjiFre) < 60)
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = "包装量统计刷新时间不能小于60s。";
                        dtMessage.Rows.Add(dataRow);

                    }
                }
                if (strGZTZhuangTaiFre == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "工作台状态刷新不能为空。";
                    dtMessage.Rows.Add(dataRow);
                }
                else
                {
                    if (Convert.ToInt32(strGZTZhuangTaiFre) < 60)
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = "工作台状态刷新时间不能小于60s。";
                        dtMessage.Rows.Add(dataRow);

                    }
                }
                if (strGZTQieHuanFre == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "工作台状态切换不能为空。";
                    dtMessage.Rows.Add(dataRow);
                }
                else
                {
                    if (Convert.ToInt32(strGZTQieHuanFre) < 60)
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = "工作台状态切换时间不能小于60s。";
                        dtMessage.Rows.Add(dataRow);

                    }
                }
                if (strObjective == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "现场个人效率目标不能为空。";
                    dtMessage.Rows.Add(dataRow);
                }
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                {
                    //弹出错误dtMessage
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.type = "list";
                    apiResult.data = dtMessage;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fS1309_Logic.setDisplayInfo(strPackingPlant, strPageClientNum, strGZTTongjiFre, strBZLTongjiFre, strGZTZhuangTaiFre, strGZTQieHuanFre, strGZTShowType, strObjective
                    , strBFromTime, strBCross, strBToTime, strYFromTime, strYCross, strYToTime, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "保存成功";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0204", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }

    }
}
