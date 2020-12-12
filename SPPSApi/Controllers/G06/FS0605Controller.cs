using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Text.RegularExpressions;
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

namespace SPPSApi.Controllers.G06
{
    [Route("api/FS0605/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0605Controller : BaseController
    {
        FS0605_Logic fs0605_Logic = new FS0605_Logic();
        private readonly string FunctionID = "FS0605";

        private readonly IWebHostEnvironment _webHostEnvironment;


        public FS0605Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        #region 绑定
        //[HttpPost]
        //[EnableCors("any")]
        //public string bindConst()
        //{
        //    //验证是否登录
        //    string strToken = Request.Headers["X-Token"];
        //    if (!isLogin(strToken))
        //    {
        //        return error_login();
        //    }
        //    LoginInfo loginInfo = getLoginByToken(strToken);
        //    //以下开始业务处理
        //    ApiResult apiResult = new ApiResult();
        //    try
        //    {
        //        DataTable dt = fs0605_Logic.BindConst();
        //        List<Object> dataList = ComFunction.convertToResult(dt, new string[] { "vcCodeId", "vcCodeName" });
        //        apiResult.code = ComConstant.SUCCESS_CODE;
        //        apiResult.data = dataList;
        //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //    }
        //    catch (Exception ex)
        //    {
        //        ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0501", ex, loginInfo.UserId);
        //        apiResult.code = ComConstant.ERROR_CODE;
        //        apiResult.data = "绑定常量列表失败";
        //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        //    }
        //}
        #endregion

        #region 检索
        [HttpPost]
        [EnableCors("any")]
        public string searchApi([FromBody] dynamic data)
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
            string vcWorkArea = dataForm.vcWorkArea == null ? "" : dataForm.vcWorkArea;
            string vcIsSureFlag = dataForm.vcIsSureFlag == null ? "" : dataForm.vcIsSureFlag;

            try
            {
                DataTable dt = fs0605_Logic.Search(vcSupplier_id, vcWorkArea, vcIsSureFlag);
                // TypeCode, TypeName, KeyCode, KeyValue, Sort, Memo
                List<Object> dataList = ComFunction.convertToResult(dt, new string[] { "vcSupplier_id", "vcWorkArea", "vcIsSureFlag", "vcLinkMan", "vcPhone","vcEmail",
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0501", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 保存
        [HttpPost]
        [EnableCors("any")]
        public string saveApi([FromBody] dynamic data)
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
                //vcSupplier_id, vcWorkArea, vcIsSureFlag, vcLinkMan, vcPhone, vcEmail, vcOperatorID, dOperatorTime
                dtmod.Columns.Add("vcSupplier_id");
                dtmod.Columns.Add("vcWorkArea");
                dtmod.Columns.Add("vcIsSureFlag");
                dtmod.Columns.Add("vcLinkMan");
                dtmod.Columns.Add("vcPhone");
                dtmod.Columns.Add("vcEmail");

                DataTable dtadd = dtmod.Clone();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bmodflag = (bool)listInfoData[i]["vcmodflag"];//false可编辑,true不可编辑
                    bool baddflag = (bool)listInfoData[i]["vcaddflag"];//false可编辑,true不可编辑
                    if (bmodflag == false && baddflag == false)
                    {//新增
                        DataRow dr = dtadd.NewRow();
                        dr["vcSupplier_id"] = listInfoData[i]["vcSupplier_id"].ToString();
                        dr["vcWorkArea"] = listInfoData[i]["vcWorkArea"].ToString();
                        dr["vcIsSureFlag"] = listInfoData[i]["vcIsSureFlag"].ToString();
                        dr["vcLinkMan"] = listInfoData[i]["vcLinkMan"].ToString();
                        dr["vcPhone"] = listInfoData[i]["vcPhone"].ToString();
                        dr["vcEmail"] = listInfoData[i]["vcEmail"].ToString();
                        dtadd.Rows.Add(dr);
                    }
                    else if (bmodflag == false && baddflag == true)
                    {//修改
                        DataRow dr = dtmod.NewRow();
                        dr["vcSupplier_id"] = listInfoData[i]["vcSupplier_id"].ToString();
                        dr["vcWorkArea"] = listInfoData[i]["vcWorkArea"].ToString();
                        dr["vcIsSureFlag"] = listInfoData[i]["vcIsSureFlag"].ToString();
                        dr["vcLinkMan"] = listInfoData[i]["vcLinkMan"].ToString();
                        dr["vcPhone"] = listInfoData[i]["vcPhone"].ToString();
                        dr["vcEmail"] = listInfoData[i]["vcEmail"].ToString();
                        dtmod.Rows.Add(dr);
                    }
                }
                if (dtadd.Rows.Count == 0 && dtmod.Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少有一个编辑行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //判断空
                for (int i = 0; i < dtadd.Rows.Count; i++)
                {
                    //[vcCodeId] ,[vcCodeName]  ,[vcValue] ,[vcName] ,[vcMeaning]
                    string vcSupplier_id = dtadd.Rows[i]["vcSupplier_id"].ToString();
                    string strvcWorkArea = dtadd.Rows[i]["vcWorkArea"].ToString();
                    string strvcIsSureFlag = dtadd.Rows[i]["vcIsSureFlag"].ToString();
                    string strvcEmail = dtadd.Rows[i]["vcEmail"].ToString();
                    if (vcSupplier_id.Length == 0|| vcSupplier_id.Trim().Length!=4)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "新增的数据供应商代码不能为空并且长度必须是四位,请确认！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (strvcWorkArea.Length == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "新增的工区不能为空,请确认！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (strvcIsSureFlag.Length == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "新增的生产能力&纳期确认不能为空,请确认！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (strvcEmail.Length>0)
                    {
                        //：("^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$")；
                        //^\\s*([A-Za-z0-9_-]+(\\.\\w+)*@(\\w+\\.)+\\w{2,5})\\s*$
                        Regex r = new Regex("^\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*$");
                        if (!r.IsMatch(strvcEmail))
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "新增的邮箱不是一个有效的邮箱地址,请确认！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                    }
                }
                for (int i = 0; i < dtmod.Rows.Count; i++)
                {
                    //[vcCodeId] ,[vcCodeName]  ,[vcValue] ,[vcName] ,[vcMeaning]
                    string strvcEmail = dtmod.Rows[i]["vcEmail"].ToString();
                    if (strvcEmail.Length > 0)
                    {
                        Regex r = new Regex("^\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*$");
                        if (!r.IsMatch(strvcEmail))
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "修改的邮箱不是一个有效的邮箱地址,请确认！";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                    }
                }
                //验证 是否有重复 数据
                string[] columnArray = { "vcSupplier_id", "vcWorkArea" };
                if (dtadd.Rows.Count > 0)
                {
                    DataView dtaddView = dtadd.DefaultView;
                    DataTable dtaddNew = dtaddView.ToTable(true, columnArray);
                    if (dtadd.Rows.Count != dtaddNew.Rows.Count)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "新增数据中有重复数据,请确认！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                //if (dtmod.Rows.Count>0)
                //{
                //    DataView dtmodView = dtmod.DefaultView;
                //    DataTable dtmodNew = dtmodView.ToTable(true, columnArray);
                //    if (dtmod.Rows.Count != dtmodNew.Rows.Count)
                //    {
                //        apiResult.code = ComConstant.ERROR_CODE;
                //        apiResult.data = "编辑的数据中在数据库中有重复数据,请确认！";
                //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                //    }
                //}
                //DataTable dtHuiZong= dtadd.Clone();//
                //if (dtadd.Rows.Count>0)
                //{
                //    dtHuiZong.Merge(dtadd);
                //}
                //if (dtmod.Rows.Count > 0)
                //{
                //    dtHuiZong.Merge(dtmod);
                //}

                //DataView dtAllView = dtHuiZong.DefaultView;
                //DataTable dtAllNew = dtAllView.ToTable(true, columnArray);
                //if(dtadd.Rows.Count+dtmod.Rows.Count!=dtAllNew.Rows.Count)
                //{
                //    apiResult.code = ComConstant.ERROR_CODE;
                //    apiResult.data = "保存数据中新增和编辑有重复数据,请确认！";
                //    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                //}
                //验证新增的里面是否有重复数据

                if (dtadd.Rows.Count > 0)
                {
                    Boolean isExistAddData = fs0605_Logic.isExistAddData(dtadd);
                    if (isExistAddData)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "新增的数据已存在数据库中,请确认！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                //验证修改的里面是否有重复数据
                //if (dtmod.Rows.Count>0)
                //{
                //    Boolean isExistModData = fs0105_Logic.isExistModData(dtmod);
                //    if (isExistModData)
                //    {
                //        apiResult.code = ComConstant.ERROR_CODE;
                //        apiResult.data = "编辑的数据中在数据库中有重复数据,请确认！";
                //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                //    }
                //}

                fs0605_Logic.Save(dtadd, dtmod, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0502", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
        #region 删除
        [HttpPost]
        [EnableCors("any")]
        public string deleteApi([FromBody] dynamic data)
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
                dtdel.Columns.Add("vcSupplier_id");
                dtdel.Columns.Add("vcWorkArea");

                for (int i = 0; i < checkedInfoData.Count; i++)
                {
                    DataRow dr = dtdel.NewRow();
                    dr["vcSupplier_id"] = checkedInfoData[i]["vcSupplier_id"].ToString();
                    dr["vcWorkArea"] = checkedInfoData[i]["vcWorkArea"].ToString();
                    dtdel.Rows.Add(dr);

                }
                if (dtdel.Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选择一行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fs0605_Logic.Del(dtdel, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0503", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}

