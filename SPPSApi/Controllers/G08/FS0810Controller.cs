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
                List<Object> dataList = ComFunction.convertAllToResult(dt);
                for (int i = 0; i < dataList.Count; i++)
                {
                    //vcRead vcWrite字段需要从 0 1转换成false true
                    Dictionary<string, object> row = (Dictionary<string, object>)dataList[i];
                    row["vcModFlag"] = row["vcModFlag"].ToString() == "1" ? true : false;
                    row["vcAddFlag"] = row["vcAddFlag"].ToString() == "1" ? true : false;
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
                JArray listInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
                DataTable dtPMSmall = fs0810_Logic.GetPMSmall();//取得品目信息维护表中信息
                int ieditRows = 0;
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bmodflag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool baddflag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    //标识说明
                    //默认  bmodflag:false  baddflag:false
                    //新增  bmodflag:true   baddflag:true
                    //修改  bmodflag:true   baddflag:false

                    if (bmodflag == true)
                    {//编辑行
                        ieditRows++;
                        string strSR = listInfoData[i]["vcSR"].ToString();
                        string strPartsNoBefore5 = listInfoData[i]["vcPartsNoBefore5"].ToString();
                        string strBCPartsNo = listInfoData[i]["vcBCPartsNo"].ToString();
                        string strSmallPM = listInfoData[i]["vcSmallPM"].ToString();
                        //校验1：受入号、品番前5位、包材品番、小品目不能为空
                        if (strSR == "" || strPartsNoBefore5 == "" || strBCPartsNo == "" || strSmallPM == "")
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "受入号、品番前5位、包材品番、小品目不能为空";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        //校验2：受入号、品番前5位、包材品番不能重复
                        if (baddflag)
                        {
                            DataRow[] drs = dtPMSmall.Select("vcSR='" + strSR + "' " + "and vcPartsNoBefore5='" + strPartsNoBefore5 + "' " + "and vcBCPartsNo='" + strBCPartsNo + "'  ");
                            if (drs.Length > 0)
                            {
                                apiResult.code = ComConstant.ERROR_CODE;
                                apiResult.data = string.Format("[受入号-品番前5位-包材品番]不能重复：{0}-{1}-{2}",
                                    strSR, strPartsNoBefore5, strBCPartsNo);
                                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                            }
                        }
                        //校验3：品番前5位+包材品番 在 包材品番表中是否存在
                    }
                }
                if (ieditRows == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选择一行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fs0810_Logic.Save(listInfoData, loginInfo.UserId);
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
                if (checkedInfoData.Count==0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选择一行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fs0810_Logic.Del(checkedInfoData, loginInfo.UserId);
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
                List<Object> dataList = ComFunction.convertAllToResult(dt);
                for (int i = 0; i < dataList.Count; i++)
                {
                    //vcRead vcWrite字段需要从 0 1转换成false true
                    Dictionary<string, object> row = (Dictionary<string, object>)dataList[i];
                    row["vcModFlag"] = row["vcModFlag"].ToString() == "1" ? true : false;
                    row["vcAddFlag"] = row["vcAddFlag"].ToString() == "1" ? true : false;
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
                JArray listInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
                DataTable dtBigPM = fs0810_Logic.GetBigPM();//取得关系表中所有大品目
                DataTable dtSmallPM = fs0810_Logic.GetSmallPM();//取得关系表中所有小品目
                int ieditRows = 0;
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bmodflag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool baddflag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    //标识说明
                    //默认  bmodflag:false  baddflag:false
                    //新增  bmodflag:true   baddflag:true
                    //修改  bmodflag:true   baddflag:false

                    if (bmodflag == true)
                    {//编辑行
                        ieditRows++;
                        string strBigPM = listInfoData[i]["vcBigPM"].ToString();
                        string strSmallPM = listInfoData[i]["vcSmallPM"].ToString();
                        string strBigPM_init = listInfoData[i]["vcBigPM_init"].ToString();
                        string strSmallPM_init = listInfoData[i]["vcSmallPM_init"].ToString();
                        //校验1：大品目、小品目不能为空
                        if (strBigPM == "" || strSmallPM == "")
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "大品目和小品目不能为空";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        //校验2：小品目不能重复
                        DataRow[] drsmallpm = dtSmallPM.Select("vcSmallPM='" + strSmallPM + "' and vcSmallPM<>'" + strSmallPM_init + "'  ");
                        if (drsmallpm.Length > 0)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "已经存在小品目：" + strSmallPM;
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                    }
                }
                if (ieditRows == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选择一行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fs0810_Logic.Save_pm(listInfoData, loginInfo.UserId);
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
                if (checkedInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选择一行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fs0810_Logic.Del_pm(checkedInfoData, loginInfo.UserId);
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

        #region 检索_基准时间
        [HttpPost]
        [EnableCors("any")]
        public string searchApi_standardtime([FromBody]dynamic data)
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
            string bigpm = dataForm.bigpm == null ? "" : dataForm.bigpm;
            string standardtime = dataForm.standardtime == null ? "" : dataForm.standardtime;
            try
            {
                DataTable dt = fs0810_Logic.Search_StandardTime(bigpm, standardtime);
                List<Object> dataList = ComFunction.convertAllToResult(dt);
                for (int i = 0; i < dataList.Count; i++)
                {
                    //vcRead vcWrite字段需要从 0 1转换成false true
                    Dictionary<string, object> row = (Dictionary<string, object>)dataList[i];
                    row["vcModFlag"] = row["vcModFlag"].ToString() == "1" ? true : false;
                    row["vcAddFlag"] = row["vcAddFlag"].ToString() == "1" ? true : false;
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

        #region 保存_基准时间
        [HttpPost]
        [EnableCors("any")]
        public string saveApi_standardtime([FromBody]dynamic data)
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
                JArray listInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
                DataTable dtStandardTime = fs0810_Logic.GetStandardTime();//取得大品目基准时间
                int ieditRows = 0;
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bmodflag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool baddflag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    //标识说明
                    //默认  bmodflag:false  baddflag:false
                    //新增  bmodflag:true   baddflag:true
                    //修改  bmodflag:true   baddflag:false                                                   

                    if (bmodflag == true)
                    {//编辑行
                        ieditRows++;
                        string strBigPM = listInfoData[i]["vcBigPM"].ToString();
                        string strStandardTime = listInfoData[i]["vcStandardTime"].ToString();

                        //校验1：大品目、基准时间不能为空
                        if (strBigPM == "" || strStandardTime == "")
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "大品目和基准时间不能为空";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        //校验2：大品目、基准时间不能重复
                        if (baddflag)
                        {
                            DataRow[] drs = dtStandardTime.Select("vcBigPM='" + strBigPM + "'  ");
                            if (drs.Length > 0)
                            {
                                apiResult.code = ComConstant.ERROR_CODE;
                                apiResult.data = string.Format("[大品目]不能重复：{0}", strBigPM);
                                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                            }
                        }
                    }
                }
                if (ieditRows == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选择一行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fs0810_Logic.Save_Standardtime(listInfoData, loginInfo.UserId);
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
        public string delApi_standardtime([FromBody]dynamic data)
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
                if (checkedInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选择一行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fs0810_Logic.Del_Standardtime(checkedInfoData, loginInfo.UserId);
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
