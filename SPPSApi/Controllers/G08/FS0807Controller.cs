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
    [Route("api/FS0807/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0807Controller : BaseController
    {
        FS0807_Logic fs0807_Logic = new FS0807_Logic();
        private readonly string FunctionID = "FS0807";

        private readonly IWebHostEnvironment _webHostEnvironment;

        public FS0807Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        #region 绑定工区
        [HttpPost]
        [EnableCors("any")]
        public string bindGQ()
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
                DataTable dt = fs0807_Logic.bindGQ();
                List<Object> dataList = ComFunction.convertAllToResult(dt);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0204", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "绑定工区失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 绑定供应商
        [HttpPost]
        [EnableCors("any")]
        public string bindSupplier()
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
                DataTable dt = fs0807_Logic.bindSupplier();
                List<Object> dataList = ComFunction.convertAllToResult(dt);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0204", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "绑定供应商失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 绑定收货方
        [HttpPost]
        [EnableCors("any")]
        public string bindSHF()
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
                DataTable dt = fs0807_Logic.bindSHF();
                List<Object> dataList = ComFunction.convertAllToResult(dt);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0204", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "绑定收货方失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

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
            string vcGQ = dataForm.vcGQ == null ? "" : dataForm.vcGQ;
            string vcSupplier_id = dataForm.vcSupplier_id == null ? "" : dataForm.vcSupplier_id;
            string vcSHF = dataForm.vcSHF == null ? "" : dataForm.vcSHF;
            string vcPart_id = dataForm.vcPart_id == null ? "" : dataForm.vcPart_id;
            string vcCarType = dataForm.vcCarType == null ? "" : dataForm.vcCarType;
            string vcTimeFrom = dataForm.vcTimeFrom == null ? "" : dataForm.vcTimeFrom;
            string vcTimeTo = dataForm.vcTimeTo == null ? "" : dataForm.vcTimeTo;

            try
            {
                DataTable dt = fs0807_Logic.Search(vcGQ, vcSupplier_id, vcSHF, vcPart_id, vcCarType, vcTimeFrom, vcTimeTo);

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
                DataTable dt = fs0807_Logic.GetPartsInfo();
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

                        string strPart_id = listInfoData[i]["vcPart_id"].ToString();
                        string strTimeFrom = listInfoData[i]["vcTimeFrom"].ToString();
                        string strTimeTo = listInfoData[i]["vcTimeTo"].ToString();
                        string strSHF = listInfoData[i]["vcSHF"].ToString();
                        string strQuantity = listInfoData[i]["iContainerQuantity"].ToString();
                        //校验 非空校验
                        if (strPart_id=="")
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = string.Format("品番不能为空：{0}",
                                strPart_id);
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        if (strTimeFrom == "")
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = string.Format("品番{0}开始时间不能为空",
                                strPart_id);
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        if (strSHF == "")
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = string.Format("品番{0}发货方不能为空",
                                strPart_id);
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        if (strQuantity == "")
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = string.Format("品番{0}收容数不能为空",
                                strPart_id);
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }

                        //校验 开始时间不能大于结束时间
                        if (string.Compare(strTimeFrom, strTimeTo) >= 0)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = string.Format("开始时间大于结束时间：{0}-{1}",
                                strTimeFrom, strTimeTo);
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        DataRow[] drs = null;
                        //校验 品番+开始时间+收货方 不能重复
                        if (baddflag)
                        {
                            drs = dt.Select("vcPart_id='" + strPart_id + "' and vcTimeFrom='" + strTimeFrom + "' and vcSHF='" + strSHF + "' ");
                            if (drs.Length > 0)
                            {
                                apiResult.code = ComConstant.ERROR_CODE;
                                apiResult.data = string.Format("[品番-开始时间-收货方]不能重复：{0}-{1}-{2}",
                                    strPart_id, strTimeFrom, strSHF);
                                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                            }
                        }
                        //校验 品番日期区间不能重复
                        if (baddflag)
                        {
                            drs = dt.Select("vcPart_id='" + strPart_id + "' and vcSHF='" + strSHF + "' " +
                                "and (('" + strTimeFrom + "'>=vcTimeFrom and '" + strTimeFrom + "'<= vcTimeTo) or ('" + strTimeTo + "'>= vcTimeFrom and '" + strTimeTo + "'<= vcTimeTo)  )  ");
                        }
                        else
                        {
                            string strAutoId = listInfoData[i]["iAutoId"].ToString();
                            drs = dt.Select("vcPart_id='" + strPart_id + "' and vcSHF='" + strSHF + "' " +
                                "and (('" + strTimeFrom + "' >= vcTimeFrom and '" + strTimeFrom + "'<= vcTimeTo) or ('" + strTimeTo + "' >= vcTimeFrom and '" + strTimeTo + "'<= vcTimeTo)  ) " +
                                "and iAutoId<>'" + strAutoId + "' ");
                        }
                        if (drs.Length > 0)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = string.Format("[品番-收货方]日期区间有交叉：{0}-{1}",
                                strPart_id, strSHF);
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
                fs0807_Logic.Save(listInfoData, loginInfo.UserId);
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
                fs0807_Logic.Del(checkedInfoData, loginInfo.UserId);
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
