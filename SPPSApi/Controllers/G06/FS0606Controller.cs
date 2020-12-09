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
    [Route("api/FS0606/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0606Controller : BaseController
    {
        FS0606_Logic fs0606_Logic = new FS0606_Logic();
        private readonly string FunctionID = "FS0606";

        private readonly IWebHostEnvironment _webHostEnvironment;


        public FS0606Controller(IWebHostEnvironment webHostEnvironment)
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
        //        DataTable dt = fs0606_Logic.BindConst();
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
            string vcPartNo = dataForm.vcPartNo == null ? "" : dataForm.vcPartNo;
            try
            {
                DataTable dt = fs0606_Logic.Search(vcPartNo);
                // vcPartNo, dBeginDate, dEndDate, vcOperatorID, dOpertatorTime
                List<Object> dataList = ComFunction.convertToResult(dt, new string[] { "vcPartNo", "dBeginDate", "dEndDate", 
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0601", ex, loginInfo.UserId);
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
                // vcPartNo, dBeginDate, dEndDate, vcOperatorID, dOpertatorTime
                dtmod.Columns.Add("vcPartNo");
                dtmod.Columns.Add("dBeginDate");
                dtmod.Columns.Add("dEndDate");

                DataTable dtadd = dtmod.Clone();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bmodflag = (bool)listInfoData[i]["vcmodflag"];//false可编辑,true不可编辑
                    bool baddflag = (bool)listInfoData[i]["vcaddflag"];//false可编辑,true不可编辑
                    if (bmodflag == false && baddflag == false)
                    {//新增
                        DataRow dr = dtadd.NewRow();
                        dr["vcPartNo"] = listInfoData[i]["vcPartNo"].ToString();
                        dr["dBeginDate"] = listInfoData[i]["dBeginDate"].ToString();
                        dr["dEndDate"] = listInfoData[i]["dEndDate"].ToString();
                       
                        dtadd.Rows.Add(dr);
                    }
                    else if (bmodflag == false && baddflag == true)
                    {//修改
                        DataRow dr = dtmod.NewRow();
                        dr["vcPartNo"] = listInfoData[i]["vcPartNo"].ToString();
                        dr["dBeginDate"] = listInfoData[i]["dBeginDate"].ToString();
                        dr["dEndDate"] = listInfoData[i]["dEndDate"].ToString();
                      
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
                    // vcPartNo, dBeginDate, dEndDate, vcOperatorID, dOpertatorTime
                    string vcPartNo = dtadd.Rows[i]["vcPartNo"].ToString();
                    string dBeginDate = dtadd.Rows[i]["dBeginDate"].ToString();
                    string dEndDate = dtadd.Rows[i]["dEndDate"].ToString();
                    
                    if (vcPartNo.Trim().Length!=12)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "新增的品番长度必须是12位,请确认！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (dBeginDate.Length == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "新增的开始日不能为空,请确认！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (dEndDate.Length == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "新增的结束日不能为空,请确认！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                for (int i = 0; i < dtmod.Rows.Count; i++)
                {
                    // vcPartNo, dBeginDate, dEndDate, vcOperatorID, dOpertatorTime
                    string dBeginDate = dtmod.Rows[i]["dBeginDate"].ToString();
                    string dEndDate = dtmod.Rows[i]["dEndDate"].ToString();
                    
                    if (dBeginDate.Length == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "新增的开始日不能为空,请确认！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (dEndDate.Length == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "新增的结束日不能为空,请确认！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                //验证 是否有重复 数据
                string[] columnArray = { "vcPartNo"};
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
                    Boolean isExistAddData = fs0606_Logic.isExistAddData(dtadd);
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

                fs0606_Logic.Save(dtadd, dtmod, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0602", ex, loginInfo.UserId);
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
                fs0606_Logic.Del(dtdel, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0603", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 一括赋予
        [HttpPost]
        [EnableCors("any")]
        public string allInstallApi([FromBody] dynamic data)
        {
            //验证是否登录
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            //以下开始业务处理
            //以下开始业务处理
            ApiResult apiResult = new ApiResult();
            dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
            string dBeginDate = dataForm.dBeginDate == null ? "" : dataForm.dBeginDate;
            string dEndDate = dataForm.dEndDate == null ? "" : dataForm.dEndDate;
            try
            {
                if (dataForm.length==0|| dBeginDate.Length == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "一括赋予开始日、结束日都不能为空,请确认！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                fs0606_Logic.allInstall (Convert.ToDateTime(dBeginDate), Convert.ToDateTime(dEndDate), loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE0604", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "一括赋予失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}

