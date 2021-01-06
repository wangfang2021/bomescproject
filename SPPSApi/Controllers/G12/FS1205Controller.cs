using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

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

        #region 生成订单
        [HttpPost]
        [EnableCors("any")]
        public string csvFileExport([FromBody] dynamic data)
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
            string msg;
            try
            {
                DataTable dtSource = new DataTable();
                msg = fS1205_Logic.TXTCSVInfoTableMaker(vcMonth, vcWeek, vcPlant, ref dtSource); //检索CSV用的数据，包装计划为基础;
                if (msg.Length > 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = msg;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                else
                {
                    DataTable dtCol = dtSource.Clone();
                    if (dtSource.Rows.Count > 0)
                    {
                        StringBuilder sb = new StringBuilder();
                        //获取列名信息（不用）
                        //string[] strColumnNames = dtSource.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToArray();
                        //sb.AppendLine(string.Join(",", strColumnNames));
                        //获取每行数据信息
                        foreach (DataRow dtRow in dtSource.Rows)
                        {
                            for (int j = 0; j < dtCol.Columns.Count; j++)
                            {
                                if (j != dtCol.Columns.Count - 1)
                                {
                                    sb.Append(dtRow[j].ToString() + ",");
                                }
                                else
                                {
                                    sb.Append(dtRow[j].ToString());
                                    sb.AppendLine();
                                }
                            }
                        }
                        //文件名：EMERGENCY_4_201810_20181025133627
                        string name = "";
                        switch (vcPlant)
                        {
                            case "1": name = "0"; break;
                            case "2": name = "4"; break;
                            case "3": name = "8"; break;
                        }
                        string CSVName = "EMERGENCY_" + name + "_" + vcMonth.Replace("-", "") + "_" + vcWeek + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                        string path = System.IO.Directory.GetCurrentDirectory() + "\\Doc\\Export\\" + CSVName + ".csv";
                        string pathupdate = CSVName + ".csv";
                        //生成CSV文件
                        System.IO.File.WriteAllText(path, sb.ToString());
                        if (System.IO.File.Exists(path))
                        {
                            //string strtmp = this.gvMonPlan.Rows[0].Cells[0].Text;
                            //if (strtmp == "初始化状态没有数据，请进行检索！")
                            //{
                            //}
                            //下载窗口
                            //ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "window.open('../tmp/" + pathupdate + "');", true);
                            //ShowMessage("导出CSV文件成功: (" + CSVName + ")！", MessageType.Information);
                            apiResult.code = ComConstant.SUCCESS_CODE;
                            apiResult.data = path;
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        else
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = msg;
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                    }
                    else
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "无可导出的数据！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                return "";
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

        #region 导出计划
        [HttpPost]
        [EnableCors("any")]
        public string planFileExport([FromBody] dynamic data)
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
                DataTable dtSource = fS1205_Logic.TXTExportPlan(vcMonth, vcWeek, vcPlant, vcType); //检索数据
                if (dtSource.Rows.Count > 0)
                {
                    fS1205_Logic.PartsNoFomatTo10(ref dtSource);
                    string name = "";
                    switch (vcType)
                    {
                        case "0": name = "包装周度计划"; break;
                        case "2": name = "看板打印周度计划"; break;
                        case "1": name = "生产周度计划"; break;
                        case "3": name = "涂装周度计划"; break;
                        case "4": name = "工程3周度计划"; break;
                    }
                    string[] fields = { "vcMonth", "vcPlant", "vcPartsno", "vcDock", "vcCarType", "vcCalendar1","vcCalendar2","vcCalendar3","vcCalendar4"
                        ,"vcPartsNameCHN","vcProject1","vcProjectName","vcCurrentPastCode","vcMonTotal"
                        ,"TD1b","TD1y","TD2b","TD2y","TD3b","TD3y","TD4b","TD4y","TD5b","TD5y","TD6b","TD6y"
                        ,"TD7b","TD7y","TD8b","TD8y","TD9b","TD9y","TD10b","TD10y","TD11b","TD11y","TD12b","TD12y"
                        ,"TD13b","TD13y","TD14b","TD14y","TD15b","TD15y","TD16b","TD16y","TD17b","TD17y","TD18b","TD18y"
                        ,"TD19b","TD19y","TD20b","TD20y","TD21b","TD21y","TD22b","TD22y","TD23b","TD23y","TD24b","TD24y"
                        ,"TD25b","TD25y","TD26b","TD26y","TD27b","TD27y","TD28b","TD28y","TD29b","TD29y","TD30b","TD30y"
                        ,"TD31b","TD31y"
                        ,"ED1b","ED1y","ED2b","ED2y","ED3b","ED3y","ED4b","ED4y","ED5b","ED5y","ED6b","ED6y"
                        ,"ED7b","ED7y","ED8b","ED8y","ED9b","ED9y","ED10b","ED10y","ED11b","ED11y","ED12b","ED12y"
                        ,"ED13b","ED13y","ED14b","ED14y","ED15b","ED15y","ED16b","ED16y","ED17b","ED17y","ED18b","ED18y"
                        ,"ED19b","ED19y","ED20b","ED20y","ED21b","ED21y","ED22b","ED22y","ED23b","ED23y","ED24b","ED24y"
                        ,"ED25b","ED25y","ED26b","ED26y","ED27b","ED27y","ED28b","ED28y","ED29b","ED29y","ED30b","ED30y"
                        ,"ED31b","ED31y"
                    };
                    string filepath = ComFunction.generateExcelWithXlt(dtSource, fields, _webHostEnvironment.ContentRootPath, "FS1205_PlanMake.xlsx", 1, loginInfo.UserId, FunctionID);
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
                else
                {
                    //ShowMessage("无可导出的数据！", MessageType.Information);
                    //InitGrid();
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "无可导出的数据！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}
