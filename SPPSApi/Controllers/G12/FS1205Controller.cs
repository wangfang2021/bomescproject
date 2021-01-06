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

        #region 日程别更新
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

        #region 日程别导出
        [HttpPost]
        [EnableCors("any")]
        public string FileExport([FromBody] dynamic data)
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
                string[] fields = { "vcMonth", "vcWeek", "vcPlant", "vcGC", "vcZB", "vcPartsno", "vcQuantityPerContainer", "vcD1b", "vcD1y", "vcD2b", "vcD2y", "vcD3b", "vcD3y", "vcD4b", "vcD4y", "vcD5b", "vcD5y", "vcD6b", "vcD6y", "vcD7b", "vcD7y", "vcD8b", "vcD8y", "vcD9b", "vcD9y", "vcD10b", "vcD10y", "vcD11b", "vcD11y", "vcD12b", "vcD12y", "vcD13b", "vcD13y", "vcD14b", "vcD14y", "vcD15b", "vcD15y", "vcD16b", "vcD16y", "vcD17b", "vcD17y", "vcD18b", "vcD18y", "vcD19b", "vcD19y", "vcD20b", "vcD20y", "vcD21b", "vcD21y", "vcD22b", "vcD22y", "vcD23b", "vcD23y", "vcD24b", "vcD24y", "vcD25b", "vcD25y", "vcD26b", "vcD26y", "vcD27b", "vcD27y", "vcD28b", "vcD28y", "vcD29b", "vcD29y", "vcD30b", "vcD30y", "vcD31b", "vcD31y", "vcWeekTotal", "vcLevelD1b", "vcLevelD1y", "vcLevelD2b", "vcLevelD2y", "vcLevelD3b", "vcLevelD3y", "vcLevelD4b", "vcLevelD4y", "vcLevelD5b", "vcLevelD5y", "vcLevelD6b", "vcLevelD6y", "vcLevelD7b", "vcLevelD7y", "vcLevelD8b", "vcLevelD8y", "vcLevelD9b", "vcLevelD9y", "vcLevelD10b", "vcLevelD10y", "vcLevelD11b", "vcLevelD11y", "vcLevelD12b", "vcLevelD12y", "vcLevelD13b", "vcLevelD13y", "vcLevelD14b", "vcLevelD14y", "vcLevelD15b", "vcLevelD15y", "vcLevelD16b", "vcLevelD16y", "vcLevelD17b", "vcLevelD17y", "vcLevelD18b", "vcLevelD18y", "vcLevelD19b", "vcLevelD19y", "vcLevelD20b", "vcLevelD20y", "vcLevelD21b", "vcLevelD21y", "vcLevelD22b", "vcLevelD22y", "vcLevelD23b", "vcLevelD23y", "vcLevelD24b", "vcLevelD24y", "vcLevelD25b", "vcLevelD25y", "vcLevelD26b", "vcLevelD26y", "vcLevelD27b", "vcLevelD27y", "vcLevelD28b", "vcLevelD28y", "vcLevelD29b", "vcLevelD29y", "vcLevelD30b", "vcLevelD30y", "vcLevelD31b", "vcLevelD31y", "vcLevelWeekTotal"
                };
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS1205_Export.xlsx", 1, loginInfo.UserId, FunctionID);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 生成计划
        [HttpPost]
        [EnableCors("any")]
        public string txtScheduleToPlan([FromBody] dynamic data)
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
            string msg = "";
            try
            {
                DataTable dt = fS1205_Logic.TXTSearchWeekLevelSchedule(vcMonth, vcWeek, vcPlant);
                if (dt != null && dt.Rows.Count > 0)
                {
                    //生成计划
                    msg = fS1205_Logic.TXTScheduleToPlan(dt, vcMonth, vcWeek, vcPlant, loginInfo.UserId);
                    if (msg.Length > 0)
                    {
                        //大于0，意思是数据中有错误
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = msg;
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    else
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "生成计划成功！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "未检索数据不能生成！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "生成计划失败！";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}
