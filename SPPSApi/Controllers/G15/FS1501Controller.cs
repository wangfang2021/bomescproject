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



namespace SPPSApi.Controllers.G15
{
    [Route("api/FS1501/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1501Controller : BaseController
    {
        FS1501_Logic fs1501_Logic = new FS1501_Logic();
        private readonly string FunctionID = "FS1501";

        private readonly IWebHostEnvironment _webHostEnvironment;

        public FS1501Controller(IWebHostEnvironment webHostEnvironment)
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
            string vcSupplier_id = dataForm.vcSupplier_id == null ? "" : dataForm.vcSupplier_id;
            string vcGQ = dataForm.vcGQ == null ? "" : dataForm.vcGQ;
            string vcSR = dataForm.vcSR == null ? "" : dataForm.vcSR;
            string vcOrderNo = dataForm.vcOrderNo == null ? "" : dataForm.vcOrderNo;
            string vcNRBianCi = dataForm.vcNRBianCi == null ? "" : dataForm.vcNRBianCi;
            string vcNRBJSK = dataForm.vcNRBJSK == null ? "" : dataForm.vcNRBJSK;

            try
            {
                DataTable dt = fs1501_Logic.Search(vcSupplier_id, vcGQ, vcSR, vcOrderNo, vcNRBianCi, vcNRBJSK);

                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);

                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);
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
                DataTable dt = fs1501_Logic.GetNRBJSKBianCiInfo();
                int ieditRows = 0;
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool baddflag = (bool)listInfoData[i]["vcAddFlag"];//编辑标识,取false的
                    bool bmodflag = (bool)listInfoData[i]["vcModFlag"];//编辑标识,取false的
                    //标识说明
                    //默认  bmodflag:false  baddflag:false
                    //新增  bmodflag:true   baddflag:true
                    //修改  bmodflag:true   baddflag:false
                    if (bmodflag == true)
                    {
                        ieditRows++;

                        string vcSupplier_id = listInfoData[i]["vcSupplier_id"].ToString();
                        string vcGQ = listInfoData[i]["vcGQ"].ToString();
                        string vcSR = listInfoData[i]["vcSR"].ToString();
                        string vcOrderNo = listInfoData[i]["vcOrderNo"].ToString();
                        //校验 非空校验
                        if (vcSupplier_id == "")
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = string.Format("供应商代码不能为空");
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        if (vcGQ == "")
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = string.Format("工区不能为空");
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        if (vcSR == "")
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = string.Format("受入不能为空");
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        if (vcOrderNo == "")
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = string.Format("订单号不能为空");
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        DataRow[] drs = null;
                        //校验 供应商代码+工区+受入 不能重复
                        if (baddflag)
                        {
                            drs = dt.Select("vcSupplier_id='" + vcSupplier_id + "' and vcGQ='" + vcGQ + "' and vcSR='" + vcSR + "' ");
                            if (drs.Length > 0)
                            {
                                apiResult.code = ComConstant.ERROR_CODE;
                                apiResult.data = string.Format("[供应商代码-工区-受入]不能重复：{0}-{1}-{2}",
                                    vcSupplier_id, vcGQ, vcSR);
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
                fs1501_Logic.Save(listInfoData, loginInfo.UserId);
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
                if (checkedInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选择一行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fs1501_Logic.Del(checkedInfoData, loginInfo.UserId);
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
