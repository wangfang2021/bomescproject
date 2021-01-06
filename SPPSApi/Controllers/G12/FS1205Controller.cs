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

namespace SPPSApi.Controllers.G12
{
    [Route("api/FS1205/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1205Controller : BaseController
    {
        FS1205_Logic fS1205_Logic = new FS1205_Logic();
        private readonly string FunctionID = "FS1205";
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FS1205Controller(IWebHostEnvironment webHostEnvironment)
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
                List<Object> dataList_PlantSource = ComFunction.convertAllToResult(fS1205_Logic.bindplant());
                List<Object> dataList_TypeSource = ComFunction.convertAllToResult(fS1205_Logic.getPlantype());
                res.Add("PlantSource", dataList_PlantSource);
                res.Add("TypeSource", dataList_TypeSource);
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
            string vcMonth = dataForm.vcMonth;
            string vcWeek = dataForm.vcWeek;
            string vcPlant = dataForm.vcPlant;
            string vcType = dataForm.vcType;

            try
            {
                DataTable dt = fS1205_Logic.TXTSearchWeekLevelSchedule(vcMonth, vcWeek, vcPlant);
                DtConverter dtConverter = new DtConverter();
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

        #region 更新到数据库
        [HttpPost]
        [EnableCors("any")]
        public string TXTUpdateTableDetermine([FromBody] dynamic data)
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
            string vcMonth = dataForm.vcMonth;
            string vcWeek = dataForm.vcWeek;
            string vcPlant = dataForm.vcPlant;
            try
            {
                DataTable dt = fS1205_Logic.TXTSearchWeekLevelSchedule(vcMonth, vcWeek, vcPlant);
                dt.Columns.Remove("iAutoId");
                if (dt.Rows.Count > 0)
                {
                    string Month = dt.Rows[0]["vcMonth"].ToString();//获取数据源中的对象月
                    string Week = dt.Rows[0]["vcWeek"].ToString();//获取数据源中的对象周
                    string Plant = dt.Rows[0]["vcPlant"].ToString();//获取数据源中的工厂
                    //检查数据正确性
                    string msg = fS1205_Logic.TXTCheckDataSchedule(dt);
                    if (msg.Length > 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = msg;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    else
                    {
                        dt.PrimaryKey = new DataColumn[]
                        {
                            dt.Columns["vcMonth"],
                            dt.Columns["vcWeek"],
                            dt.Columns["vcPlant"],
                            dt.Columns["vcGC"],
                            dt.Columns["vcZB"],
                            dt.Columns["vcPartsno"]
                        };
                        fS1205_Logic.TXTUpdateTableSchedule(dt, Month, Week, Plant);
                        //最终提示信息
                        //更新成功重新获取数据源
                        apiResult.code = ComConstant.SUCCESS_CODE;
                        apiResult.data = "更新成功";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "无可更新的数据请先检索！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
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
