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
    [Route("api/FS1301/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1301Controller : BaseController
    {
        FS1301_Logic fS1301_Logic = new FS1301_Logic();
        private readonly string FunctionID = "FS1301";

        private readonly IWebHostEnvironment _webHostEnvironment;
        public FS1301Controller(IWebHostEnvironment webHostEnvironment)
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
                List<Object> PlantList = ComFunction.convertAllToResult(ComFunction.getTCode("C000"));//工厂vcValue   vcName
                List<Object> RolerList = ComFunction.convertAllToResult(fS1301_Logic.getRolerInfo());//角色vcValue   vcName
                res.Add("PlantList", PlantList);
                res.Add("RolerList", RolerList);

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
        /// 查询方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string searchApi([FromBody]dynamic data)
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
            string strPlant = dataForm.Plant == null ? "" : dataForm.Plant;
            string strUserId = dataForm.UserId == null ? "" : dataForm.UserId;
            string strRoler = dataForm.Roler == null ? "" : dataForm.Roler;
            try
            {
                DataTable dt = fS1301_Logic.getSearchInfo(strPlant, strUserId, strRoler);
                List<Object> dataList = ComFunction.convertToResult(dt, new string[] { "LinId", "vcUserId", "vcUserName", "vcPlant", "bChecker", "bUnLockChecker", "bPacker", "bUnLockPacker" });
                for (int i = 0; i < dataList.Count; i++)
                {
                    //vcRead vcWrite字段需要从 0 1转换成false true
                    Dictionary<string, object> row = (Dictionary<string, object>)dataList[i];
                    row["bChecker"] = row["bChecker"].ToString() == "1" ? true : false;
                    row["bUnLockChecker"] = row["bUnLockChecker"].ToString() == "1" ? true : false;
                    row["bPacker"] = row["bPacker"].ToString() == "1" ? true : false;
                    row["bUnLockPacker"] = row["bUnLockPacker"].ToString() == "1" ? true : false;
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
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
        /// 更新方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
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
                dynamic temp = JsonConvert.DeserializeObject(Convert.ToString(data));
                JArray listInfo = temp.list;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
                bool bResult= fS1301_Logic.saveDataInfo(listInfoData, loginInfo.UserId);
                if(bResult)
                {
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.data = null;
                }else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "更新失败";
                }
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0203", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "更新失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }

    }
}
