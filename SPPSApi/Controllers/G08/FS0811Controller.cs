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

namespace SPPSApi.Controllers.G08
{
    [Route("api/FS0811/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0811Controller : BaseController
    {
        FS0811_Logic fS0811_Logic = new FS0811_Logic();
        private readonly string FunctionID = "FS0811";

        private readonly IWebHostEnvironment _webHostEnvironment;
        public FS0811Controller(IWebHostEnvironment webHostEnvironment)
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
                List<Object> BanZhiList = ComFunction.convertAllToResult(ComFunction.getTCode("C031"));//班值
                //查询列表数据及输入框
                //tempList  PeopleNumItem   CycleTimeItem   ObjectiveItem   WorkOverTimeItem
                DataTable dtNowBanZhiInfo = fS0811_Logic.getNowBanZhiInfo();
                if (dtNowBanZhiInfo.Rows.Count != 0)
                {
                    string strHosDate = dtNowBanZhiInfo.Rows[0]["dHosDate"].ToString();
                    string strBanZhi = dtNowBanZhiInfo.Rows[0]["vcBanZhi"].ToString();
                    string strFromTime = dtNowBanZhiInfo.Rows[0]["tFromTime"].ToString();
                    string strToTime = dtNowBanZhiInfo.Rows[0]["tToTime"].ToString();
                    DataSet dsLoadPageData = fS0811_Logic.getLoadPageData(strHosDate, strBanZhi, strFromTime, strToTime);
                    if (dsLoadPageData != null)
                    {
                        DataTable dtFormList = dsLoadPageData.Tables[0];
                        DataTable dtTempList = dsLoadPageData.Tables[1];
                        if (dtFormList.Rows.Count == 0 || dtTempList.Rows.Count == 0)
                        {
                            res.Add("Mesflag", "warning");
                            res.Add("MessageInfo", "实行计划未生成，无法进行此页操作");
                            res.Add("BanZhiList", BanZhiList);
                        }
                        else
                        {
                            res.Add("Mesflag", "success");
                            res.Add("BanZhiList", BanZhiList);
                            res.Add("PeopleNumItem", dtFormList.Rows[0]["vcPeopleNum"].ToString());
                            res.Add("CycleTimeItem", dtFormList.Rows[0]["vcCycleTime"].ToString());
                            res.Add("ObjectiveItem", dtFormList.Rows[0]["vcObjective"].ToString());
                            res.Add("WorkOverTimeItem", dtFormList.Rows[0]["vcWorkOverTime"].ToString());
                            res.Add("uuidItem", dtFormList.Rows[0]["uuid"].ToString());
                            dtTempList.Columns.Add("vcFlag");
                            foreach (DataRow item in dtTempList.Rows)
                                item["vcFlag"] = "1";
                            res.Add("tempList", dtTempList);
                        }
                    }
                    else
                    {
                        res.Add("Mesflag", "warning");
                        res.Add("MessageInfo", "实行计划未生成，无法进行此页操作");
                        res.Add("BanZhiList", BanZhiList);
                    }
                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "班值信息确定异常，请联系管理员";
                }
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

            string strHosDate = dataForm.HosDate == null ? "" : dataForm.HosDate;
            string strBanZhi = dataForm.BanZhi == null ? "" : dataForm.BanZhi;
            Dictionary<string, object> res = new Dictionary<string, object>();
            try
            {
                DataSet dsSearchInfo = fS0811_Logic.getSearchInfo(strHosDate, strBanZhi);
                DataTable dtFormList = dsSearchInfo.Tables[0];
                DataTable dtTempList = dsSearchInfo.Tables[1];
                if (dtFormList.Rows.Count == 0 || dtTempList.Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "该时间未进行数据保存，无法进行此页操作";
                }
                else
                {
                    res.Add("PeopleNumItem", dtFormList.Rows[0]["vcPeopleNum"].ToString());
                    res.Add("CycleTimeItem", dtFormList.Rows[0]["vcCycleTime"].ToString());
                    res.Add("ObjectiveItem", dtFormList.Rows[0]["vcObjective"].ToString());
                    res.Add("WorkOverTimeItem", dtFormList.Rows[0]["vcWorkOverTime"].ToString());
                    res.Add("uuidItem", dtFormList.Rows[0]["uuid"].ToString());
                    res.Add("tempList", dtTempList);
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
        /// 计算方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors("any")]
        public string queryApi([FromBody]dynamic data)
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

            string strPeopleNum = dataForm.PeopleNum;
            string strCycleTime = dataForm.CycleTime;
            string strObjective = dataForm.Objective;
            string strWorkOverTime = dataForm.WorkOverTime;
            string struuid = dataForm.uuid;

            JArray listInfo = dataForm.list;
            List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
            try
            {
                if (listInfoData.Count != 0)
                {
                    //获取待打印的数据
                    //DataTable dataTable = fS0617_Logic.getPrintInfo(listInfoData);
                    //执行打印操作
                    //===========================================





                    //===========================================
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.data = "计算成功";
                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "未选择有效的打印数据";
                }
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0204", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "生成印刷文件失败";
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
            string strPeopleNum = dataForm.PeopleNum;
            string strCycleTime = dataForm.CycleTime;
            string strObjective = dataForm.Objective;
            string strWorkOverTime = dataForm.WorkOverTime;
            string struuid = dataForm.uuid;

            JArray listInfo = dataForm.list;
            List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
            try
            {
                if (listInfoData.Count != 0)
                {
                    //1.判断struuid是否为空，为空则提示需要先进行计算
                    //DataTable dataTable = fS0617_Logic.getPrintInfo(listInfoData);
                    //执行打印操作
                    //===========================================





                    //===========================================
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.data = "保存成功";
                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "未选择有效的打印数据";
                }
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0204", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "生成印刷文件失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
    }
}
