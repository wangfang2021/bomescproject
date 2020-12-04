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
    [Route("api/FS0810/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0810Controller : BaseController
    {
        FS0810_Logic fs0810_Logic = new FS0810_Logic();
        private readonly string FunctionID = "FS0810";

        private readonly IWebHostEnvironment _webHostEnvironment;

        public FS0810Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        #region 检索
        [HttpPost]
        [EnableCors("any")]
        public string searchApi([FromBody]dynamic data)
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
            string smallpm = dataForm.smallpm == null ? "" : dataForm.smallpm;
            string sr = dataForm.sr == null ? "" : dataForm.sr;
            string pfbefore5 = dataForm.pfbefore5 == null ? "" : dataForm.pfbefore5;
            try
            {
                DataTable dt = fs0810_Logic.Search(smallpm, sr, pfbefore5);
                List<Object> dataList = ComFunction.convertToResult(dt, new string[] { "vcSR", "vcPartsNoBefore5", "vcBCPartsNo", "vcSmallPM", "vcBigPM",
                    "vcmodflag","vcaddflag"});
                for (int i = 0; i < dataList.Count; i++)
                {
                    //vcRead vcWrite字段需要从 0 1转换成false true
                    Dictionary<string, object> row = (Dictionary<string, object>)dataList[i];
                    row["vcmodflag"] = row["vcmodflag"].ToString() == "1" ? true : false;
                    row["vcaddflag"] = row["vcaddflag"].ToString() == "1" ? true : false;
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
        #endregion

        #region 保存
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
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                JArray listInfo = dataForm.list;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
                DataTable dtmod = new DataTable();
                dtmod.Columns.Add("vcSR");
                dtmod.Columns.Add("vcPartsNoBefore5");
                dtmod.Columns.Add("vcBCPartsNo");
                dtmod.Columns.Add("vcSmallPM");
                DataTable dtadd = dtmod.Clone();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bmodflag = (bool)listInfoData[i]["vcmodflag"];//false可编辑,true不可编辑
                    bool baddflag = (bool)listInfoData[i]["vcaddflag"];//false可编辑,true不可编辑
                    if (bmodflag == false && baddflag == false)
                    {//新增
                        DataRow dr = dtadd.NewRow();
                        dr["vcSR"] = listInfoData[i]["vcSR"].ToString();
                        dr["vcPartsNoBefore5"] = listInfoData[i]["vcPartsNoBefore5"].ToString();
                        dr["vcBCPartsNo"] = listInfoData[i]["vcBCPartsNo"].ToString();
                        dr["vcSmallPM"] = listInfoData[i]["vcSmallPM"].ToString();
                        dtadd.Rows.Add(dr);
                    }
                    else if (bmodflag == false && baddflag == true)
                    {//修改
                        DataRow dr = dtmod.NewRow();
                        dr["vcSR"] = listInfoData[i]["vcSR"].ToString();
                        dr["vcPartsNoBefore5"] = listInfoData[i]["vcPartsNoBefore5"].ToString();
                        dr["vcBCPartsNo"] = listInfoData[i]["vcBCPartsNo"].ToString();
                        dr["vcSmallPM"] = listInfoData[i]["vcSmallPM"].ToString();
                        dtmod.Rows.Add(dr);
                    }
                }
                if (dtadd.Rows.Count == 0 && dtmod.Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少有一个编辑行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fs0810_Logic.Save(dtadd, dtmod, loginInfo.UserId);
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
        #endregion

        #region 删除
        [HttpPost]
        [EnableCors("any")]
        public string delApi([FromBody]dynamic data)
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
                List<Dictionary<string, Object>> checkedInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();
                DataTable dtdel = new DataTable();
                dtdel.Columns.Add("vcSR");
                dtdel.Columns.Add("vcPartsNoBefore5");
                dtdel.Columns.Add("vcBCPartsNo");
                for (int i = 0; i < checkedInfoData.Count; i++)
                {
                    DataRow dr = dtdel.NewRow();
                    dr["vcSR"] = checkedInfoData[i]["vcSR"].ToString();
                    dr["vcPartsNoBefore5"] = checkedInfoData[i]["vcPartsNoBefore5"].ToString();
                    dr["vcBCPartsNo"] = checkedInfoData[i]["vcBCPartsNo"].ToString();
                    dtdel.Rows.Add(dr);

                }
                if (dtdel.Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选择一行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fs0810_Logic.Del(dtdel, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0203", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 检索_品目
        [HttpPost]
        [EnableCors("any")]
        public string searchApi_pm([FromBody]dynamic data)
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
            string smallpm = dataForm.smallpm == null ? "" : dataForm.smallpm;
            string bigpm = dataForm.bigpm == null ? "" : dataForm.bigpm;
            try
            {
                DataTable dt = fs0810_Logic.Search_PM(smallpm, bigpm);
                List<Object> dataList = ComFunction.convertToResult(dt, new string[] { "vcBigPM", "vcSmallPM", "vcBigPM_init", "vcSmallPM_init", "vcaddflag", "vcmodflag" });
                for (int i = 0; i < dataList.Count; i++)
                {
                    //vcRead vcWrite字段需要从 0 1转换成false true
                    Dictionary<string, object> row = (Dictionary<string, object>)dataList[i];
                    row["vcmodflag"] = row["vcmodflag"].ToString() == "1" ? true : false;
                    row["vcaddflag"] = row["vcaddflag"].ToString() == "1" ? true : false;
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
        #endregion

        #region 保存_品目
        [HttpPost]
        [EnableCors("any")]
        public string saveApi_pm([FromBody]dynamic data)
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
                JArray listInfo = dataForm.functionlist;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
                DataTable dtBigPM = fs0810_Logic.GetBigPM();//取得关系表中所有大品目
                DataTable dtSmallPM = fs0810_Logic.GetSmallPM();//取得关系表中所有小品目
                DataTable dt = new DataTable();
                dt.Columns.Add("vcBigPM");
                dt.Columns.Add("vcSmallPM");
                dt.Columns.Add("vcBigPM_init");
                dt.Columns.Add("vcSmallPM_init");
                dt.Columns.Add("vcflag");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bmodflag = (bool)listInfoData[i]["vcmodflag"];//false可编辑,true不可编辑
                    bool baddflag = (bool)listInfoData[i]["vcaddflag"];//false可编辑,true不可编辑
                    //标识说明
                    //默认  bmodflag:true    baddflag:true
                    //新增  bmodflag:false   baddflag:false
                    //修改  bmodflag:false   baddflag:true

                    if (bmodflag == false)
                    {//编辑行
                        DataRow dr = dt.NewRow();
                        dr["vcBigPM"] = listInfoData[i]["vcBigPM"].ToString().Trim();
                        dr["vcSmallPM"] = listInfoData[i]["vcSmallPM"].ToString().Trim();
                        dr["vcBigPM_init"] = listInfoData[i]["vcBigPM_init"].ToString().Trim();
                        dr["vcSmallPM_init"] = listInfoData[i]["vcSmallPM_init"].ToString().Trim();
                        if (bmodflag == false && baddflag == false)
                        {//新增
                            dr["vcflag"] = "add";
                        }
                        else if (bmodflag == false && baddflag == true)
                        {//修改
                            dr["vcflag"] = "mod";
                        }

                        //校验
                        //大品目、小品目不能为空
                        if(dr["vcBigPM"].ToString()=="" || dr["vcSmallPM"].ToString() == "")
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "大品目和小品目不能为空";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        //小品目不能重复
                        DataRow[] drsmallpm = dtSmallPM.Select("vcSmallPM='" + dr["vcSmallPM"].ToString() + "' and vcSmallPM<>'" + dr["vcSmallPM_init"].ToString() + "'  ");
                        if (drsmallpm.Length > 0)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "已经存在小品目：" + dr["vcSmallPM"].ToString();
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }

                        dt.Rows.Add(dr);
                    }
                }
                if (dt.Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少有一个编辑行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fs0810_Logic.Save_pm(dt, loginInfo.UserId);
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
        #endregion

        #region 删除_品目
        [HttpPost]
        [EnableCors("any")]
        public string delApi_pm([FromBody]dynamic data)
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
                List<Dictionary<string, Object>> checkedInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();
                DataTable dtdel = new DataTable();
                dtdel.Columns.Add("vcBigPM");
                dtdel.Columns.Add("vcSmallPM");
                for (int i = 0; i < checkedInfoData.Count; i++)
                {
                    DataRow dr = dtdel.NewRow();
                    dr["vcBigPM"] = checkedInfoData[i]["vcBigPM"].ToString();
                    dr["vcSmallPM"] = checkedInfoData[i]["vcSmallPM"].ToString();
                    dtdel.Rows.Add(dr);

                }
                if (dtdel.Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选择一行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fs0810_Logic.Del_pm(dtdel, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0203", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}
