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
        /// 绑定工厂信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string bindPlant()
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
                DataTable dt = fS1301_Logic.getPlantInfo();
                List<Object> dataList = ComFunction.convertAllToResult(dt);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0204", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "绑定工厂失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        /// <summary>
        /// 绑定角色信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string bindRoler()
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
                DataTable dt = fS1301_Logic.getRolerInfo();
                List<Object> dataList = ComFunction.convertAllToResult(dt);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0204", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "绑定角色失败";
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
        public string search_api([FromBody]dynamic data)
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
            string strPlant = dataForm.Plant == null ? "" : dataForm.Plant;
            string strUser = dataForm.User == null ? "" : dataForm.User;
            string strRoler = dataForm.Roler == null ? "" : dataForm.Roler;
            try
            {
                DataTable dt = fS1301_Logic.getSearchInfo(strPlant, strUser, strRoler);
                List<Object> dataList = ComFunction.convertAllToResult(dt);
                for (int i = 0; i < dataList.Count; i++)
                {
                    Dictionary<string, object> row = (Dictionary<string, object>)dataList[i];
                    row["eableflag"] = row["eableflag"].ToString() == "1" ? true : false;
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
        /// 保存方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string save_api([FromBody]dynamic data)
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
                JArray listInfo = dataForm.list;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
                DataTable dt = new DataTable();
                dt.Columns.Add("vcPart_id");
                dt.Columns.Add("dTimeFrom");
                dt.Columns.Add("dTimeTo");
                dt.Columns.Add("vcBZPlant");
                dt.Columns.Add("vcBZQF");
                dt.Columns.Add("vcBZUnit");
                dt.Columns.Add("vcRHQF");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bflag = (bool)listInfoData[i]["vcflag"];//编辑标识,取false的
                    if (bflag == false)
                    {
                        DataRow dr = dt.NewRow();
                        dr["vcPart_id"] = listInfoData[i]["vcPart_id"].ToString();
                        dr["dTimeFrom"] = listInfoData[i]["dTimeFrom"].ToString();
                        dr["dTimeTo"] = listInfoData[i]["dTimeTo"].ToString();
                        dr["vcBZPlant"] = listInfoData[i]["vcBZPlant"].ToString();
                        dr["vcBZQF"] = listInfoData[i]["vcBZQF"].ToString();
                        dr["vcBZUnit"] = listInfoData[i]["vcBZUnit"].ToString();
                        dr["vcRHQF"] = listInfoData[i]["vcRHQF"].ToString();
                        dt.Rows.Add(dr);
                    }
                }
                if (dt.Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选择一个编辑行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fS1301_Logic.Save(dt, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0203", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }

    }
}
