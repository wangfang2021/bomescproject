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
            string strUserId = ComFunction.Decrypt(strToken);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0204", ex, strUserId);
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
            string strUserId = ComFunction.Decrypt(strToken);
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
                    bool baddflag= (bool)listInfoData[i]["vcaddflag"];//false可编辑,true不可编辑
                    if (bmodflag==false && baddflag==false)
                    {//新增
                        DataRow dr = dtadd.NewRow();
                        dr["vcSR"] = listInfoData[i]["vcSR"].ToString();
                        dr["vcPartsNoBefore5"] = listInfoData[i]["vcPartsNoBefore5"].ToString();
                        dr["vcBCPartsNo"] = listInfoData[i]["vcBCPartsNo"].ToString();
                        dr["vcSmallPM"] = listInfoData[i]["vcSmallPM"].ToString();
                        dtadd.Rows.Add(dr);
                    }
                    else if(bmodflag==false && baddflag==true)
                    {//修改
                        DataRow dr = dtmod.NewRow();
                        dr["vcSR"] = listInfoData[i]["vcSR"].ToString();
                        dr["vcPartsNoBefore5"] = listInfoData[i]["vcPartsNoBefore5"].ToString();
                        dr["vcBCPartsNo"] = listInfoData[i]["vcBCPartsNo"].ToString();
                        dr["vcSmallPM"] = listInfoData[i]["vcSmallPM"].ToString();
                        dtmod.Rows.Add(dr);
                    }
                }
                if (dtadd.Rows.Count == 0 && dtmod.Rows.Count==0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少有一个编辑行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fs0810_Logic.Save(dtadd,dtmod, strUserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0203", ex, strUserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}
