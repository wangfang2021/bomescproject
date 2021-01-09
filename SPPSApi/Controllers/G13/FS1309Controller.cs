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
                //处理初始化
                List<Object> PackPlantList = ComFunction.convertAllToResult(ComFunction.getTCode("C023"));//包装厂
                res.Add("PackPlantList", PackPlantList);

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
        /// <summary>
        /// 刷新方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string refreshApi([FromBody]dynamic data)
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

            string strPackPlant = dataForm.PackPlant == null ? "" : dataForm.PackPlant;
            try
            {
                if (strPackPlant == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请选择有效的包装厂";
                }
                else
                {
                    DataSet dataSet = fS1309_Logic.getSearchInfo(strPackPlant);
                    if (dataSet.Tables[0].Rows.Count != 0)
                    {
                        //班值信息
                        res.Add("BFromTimeItem", (dataSet.Tables[0].Select("vcBanZhi='白'")).Length == 0 ? "08:30" : (dataSet.Tables[0].Select("vcBanZhi='白'"))[0]["tFromTime"].ToString());
                        res.Add("BCrossItem", (dataSet.Tables[0].Select("vcBanZhi='白'")).Length == 0 ? "0" : (dataSet.Tables[0].Select("vcBanZhi='白'"))[0]["vcCross"].ToString());
                        res.Add("BToTimeItem", (dataSet.Tables[0].Select("vcBanZhi='白'")).Length == 0 ? "17:15" : (dataSet.Tables[0].Select("vcBanZhi='白'"))[0]["tToTime"].ToString());
                        res.Add("YFromTimeItem", (dataSet.Tables[0].Select("vcBanZhi='夜'")).Length == 0 ? "21:00" : (dataSet.Tables[0].Select("vcBanZhi='夜'"))[0]["tFromTime"].ToString());
                        res.Add("YCrossItem", (dataSet.Tables[0].Select("vcBanZhi='夜'")).Length == 0 ? "1" : (dataSet.Tables[0].Select("vcBanZhi='夜'"))[0]["vcCross"].ToString());
                        res.Add("YToTimeItem", (dataSet.Tables[0].Select("vcBanZhi='夜'")).Length == 0 ? "05:45" : (dataSet.Tables[0].Select("vcBanZhi='夜'"))[0]["tToTime"].ToString());

                    }
                    if (dataSet.Tables[1].Rows.Count != 0)
                    {
                        //显示信息
                        res.Add("PageClientNumItem", dataSet.Tables[1].Rows[0]["vcPageClientNum"].ToString()==""?"30": dataSet.Tables[1].Rows[0]["vcPageClientNum"].ToString());
                        res.Add("GZTTongjiFreItem", dataSet.Tables[1].Rows[0]["iGZTTongjiFre"].ToString()==""?"60": dataSet.Tables[1].Rows[0]["iGZTTongjiFre"].ToString());
                        res.Add("BZLTongjiFreItem", dataSet.Tables[1].Rows[0]["iBZLTongjiFre"].ToString()==""?"60": dataSet.Tables[1].Rows[0]["iBZLTongjiFre"].ToString());
                        res.Add("GZTZhuangTaiFreItem", dataSet.Tables[1].Rows[0]["iGZTZhuangTaiFre"].ToString()==""?"60": dataSet.Tables[1].Rows[0]["iGZTZhuangTaiFre"].ToString());
                        res.Add("GZTQieHuanFreItem", dataSet.Tables[1].Rows[0]["iGZTQieHuanFre"].ToString()==""?"60": dataSet.Tables[1].Rows[0]["iGZTQieHuanFre"].ToString());
                        res.Add("GZTShowTypeItem", dataSet.Tables[1].Rows[0]["iGZTShowType"].ToString()==""?"1" : dataSet.Tables[1].Rows[0]["iGZTShowType"].ToString());
                    }
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
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

            string strPackPlant = dataForm.PackPlant == null ? "" : dataForm.PackPlant;
            string strPageClientNum = dataForm.PageClientNum == null ? "" : dataForm.PageClientNum;
            string strGZTTongjiFre = dataForm.GZTTongjiFre == null ? "" : dataForm.GZTTongjiFre;
            string strBZLTongjiFre = dataForm.BZLTongjiFre == null ? "" : dataForm.BZLTongjiFre;
            string strGZTZhuangTaiFre = dataForm.GZTZhuangTaiFre == null ? "" : dataForm.GZTZhuangTaiFre;
            string strGZTQieHuanFre = dataForm.GZTQieHuanFre == null ? "" : dataForm.GZTQieHuanFre;
            string strGZTShowType = dataForm.GZTShowType == null ? "" : dataForm.GZTShowType;
            string strBFromTime = dataForm.BFromTime == null ? "" : dataForm.BFromTime;
            string strBCross = dataForm.BCross == null ? "" : dataForm.BCross;
            string strBToTime = dataForm.BToTime == null ? "" : dataForm.BToTime;
            string strYFromTime = dataForm.YFromTime == null ? "" : dataForm.YFromTime;
            string strYCross = dataForm.YCross == null ? "" : dataForm.YCross;
            string strYToTime = dataForm.YToTime == null ? "" : dataForm.YToTime;
            try
            {   //保存
                fS1309_Logic.setDisplayInfo(strPackPlant, strPageClientNum, strGZTTongjiFre, strBZLTongjiFre, strGZTZhuangTaiFre, strGZTQieHuanFre, strGZTShowType
                    , strBFromTime, strBCross, strBToTime, strYFromTime, strYCross, strYToTime, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "保存成功";
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

    }
}
