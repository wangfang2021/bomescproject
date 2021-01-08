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
    [Route("api/FS0623/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0623Controller : BaseController
    {
        FS0623_Logic fs0623_Logic = new FS0623_Logic();
        private readonly string FunctionID = "FS0623";

        private readonly IWebHostEnvironment _webHostEnvironment;


        public FS0623Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

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
            //dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
            //string vcTargetYear = dataForm.vcTargetYear == null ? "" : dataForm.vcTargetYear;
            //string vcPartNo = dataForm.vcPartNo == null ? "" : dataForm.vcPartNo;
            //string vcInjectionFactory = dataForm.vcInjectionFactory == null ? "" : dataForm.vcInjectionFactory;
            //string vcInsideOutsideType = dataForm.vcInsideOutsideType == null ? "" : dataForm.vcInsideOutsideType;
            //string vcSupplier_id = dataForm.vcSupplier_id == null ? "" : dataForm.vcSupplier_id;
            //string vcWorkArea = dataForm.vcWorkArea == null ? "" : dataForm.vcWorkArea;
            //string vcCarType = dataForm.vcCarType == null ? "" : dataForm.vcCarType;
            try
            {
                DataSet ds = fs0623_Logic.Search();
                DataTable dt1 = ds.Tables[0];//订货方试表
                //计算一下订货方式的列数
                int numDt1 = dt1.Columns.Count;
                DataTable dt2 = ds.Tables[1];//订单区分
                DataTable dt3 = ds.Tables[2];//中间表
                DataTable dt4 = ds.Tables[3];//订单区分页面显示
                DtConverter dtConverter = new DtConverter();
                //重构新的dt
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    dt1.Columns.Add(dt2.Rows[i]["vcOrderDifferentiation"].ToString());
                    dtConverter.addField(dt2.Rows[i]["vcOrderDifferentiation"].ToString(), ConvertFieldType.BoolType, null);

                }
                //定义dt1的数据
                for (int i= 0;i<dt1.Rows.Count;i++)
                {
                    for (int j = numDt1; j < dt1.Columns.Count; j++)
                    {
                        dt1.Rows[i][j] = "0";
                    }
                }

                for (int i = 0; i < dt1.Rows.Count; i++) {

                    string strDsfs = dt1.Rows[i]["dhfs"].ToString();
                    for (int j = 0; j < dt3.Rows.Count; j++) {
                        //中间表的订货方式是否等于strDsfs
                        if (dt3.Rows[j]["vcOrderGoods"].ToString()==strDsfs)
                        {
                            string strOrderDifferentiation = dt3.Rows[j]["vcOrderDifferentiation"].ToString();
                            dt1.Rows[i][strOrderDifferentiation] = "1";
                            //
                            //string strOrderDifferentiation = dt3.Rows[j]["vcOrderDifferentiation"].ToString();
                            //for (int g = 0; g < dt2.Columns.Count; g++) {
                            //    if (dt2.Columns[g].ColumnName== strOrderDifferentiation)
                            //    { 
                            //        dt2.Rows[]
                            //    }
                            //}
                        }
                    }
                }


               
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("show", ConvertFieldType.BoolType, null);
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt1, dtConverter);
                List<Object> ddlxList = ComFunction.convertAllToResult(dt4);
                var obbject = new object[] {
                    new { list=dataList},
                    new { ddlxList=ddlxList}
                };
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = obbject;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2304", ex, loginInfo.UserId);
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
                JArray listInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
                bool hasFind = false;//是否找到需要新增或者修改的数据
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    if (bAddFlag == true)
                    {//新增
                        hasFind = true;
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        hasFind = true;
                    }
                }
                if (!hasFind)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少有一个编辑行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                #region 判断新增的数据本身是否有重复的 和数据库内部关键字是否有重复的
                //本身判重 "iAutoId", "vcSupplier_id", "vcWorkArea", "dBeginDate", "dEndDate", "vcOperatorID", "dOperatorTime"
                DataTable dtadd = new DataTable();
                dtadd.Columns.Add("vcOrderGoods");//新增的订单方式
                DataTable dtaddZJB = new DataTable();//新增中间表数据
                dtaddZJB.Columns.Add("vcOrderGoods");
                dtaddZJB.Columns.Add("vcOrderDifferentiation");

                DataTable dtamodify = new DataTable();//修改的订单方式
                dtamodify.Columns.Add("vcOrderGoods");//新增的订单方式
                DataTable dtamodifyZJB = new DataTable();//新增中间表数据
                dtamodifyZJB.Columns.Add("vcOrderGoods");
                dtamodifyZJB.Columns.Add("vcOrderDifferentiation");
                //获取订单区分
                String strOrderDifferentiation = fs0623_Logic.GetOrderDifferentiation();


                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (listInfoData[i]["vcAddFlag"].ToString().ToLower() == "true")
                    {
                        DataRow dr = dtadd.NewRow();
                        dr["vcOrderGoods"] = listInfoData[i]["dhfs"].ToString();
                        dtadd.Rows.Add(dr);
                        List<string> test = new List<string>(listInfoData[i].Keys);//新增中间表数据
                        
                        foreach (string key in test)
                        {
                            if (strOrderDifferentiation.Contains(key) && listInfoData[i][key].ToString().ToLower() == "true")
                            {
                                DataRow drZJB = dtaddZJB.NewRow();
                                drZJB["vcOrderGoods"] = listInfoData[i]["dhfs"].ToString();
                                drZJB["vcOrderDifferentiation"] = key;
                                dtaddZJB.Rows.Add(drZJB);
                            }
                        }
                    }
                    else
                    {
                        DataRow drModify = dtamodify.NewRow();
                        drModify["vcOrderGoods"] = listInfoData[i]["dhfs"].ToString();
                        dtamodify.Rows.Add(drModify);
                        List<string> test = new List<string>(listInfoData[i].Keys);//新增中间表数据
                        
                        foreach (string key in test)
                        {
                            if (strOrderDifferentiation.Contains(key) && listInfoData[i][key].ToString().ToLower() == "true")
                            {
                                DataRow drModfiyZJB = dtamodifyZJB.NewRow();
                                drModfiyZJB["vcOrderGoods"] = listInfoData[i]["dhfs"].ToString();
                                drModfiyZJB["vcOrderDifferentiation"] = key;
                                dtamodifyZJB.Rows.Add(drModfiyZJB);
                            }
                        }
                    }
                }

                if (dtadd.Rows.Count > 0)
                {
                    if (dtadd.Rows.Count > 1)
                    {
                        for (int i = 0; i < dtadd.Rows.Count; i++)
                        {
                            for (int j = i + 1; j < dtadd.Rows.Count; j++)
                            {
                                if (dtadd.Rows[i]["vcOrderGoods"].ToString() == dtadd.Rows[j]["vcOrderGoods"].ToString())
                                {
                                    apiResult.code = ComConstant.ERROR_CODE;
                                    apiResult.data = "订货方式" + dtadd.Rows[i]["vcOrderGoods"].ToString() + "存在重复项，请确认！";
                                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                                }
                            }
                        }
                    }

                    //数据库验证  true  存在重复项
                    DataTable dt = fs0623_Logic.CheckDistinctByTableOrderGoods(dtadd);
                    if (dt.Rows.Count > 0)
                    {
                        string errMsg = string.Empty;
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            errMsg += "订货方式" + dt.Rows[i]["vcOrderGoods"].ToString() + ",";
                        }
                        errMsg.Substring(0, errMsg.LastIndexOf(","));
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = errMsg + "存在重复项，请确认！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }

                #endregion
                //开始数据验证
                if (hasFind)
                {
                    string[,] strField = new string[,] {{"订货方式"},
                                                {"dhfs"},
                                                {""},
                                                {"200"},//最大长度设定,不校验最大长度用0
                                                {"1"},//最小长度设定,可以为空用0
                                                {"1"}//前台显示列号，从0开始计算,注意有选择框的是0
                    };
                    //需要判断时间区间先后关系的字段
                    string[,] strDateRegion = { };

                    List<Object> checkRes = ListChecker.validateList(listInfoData, strField, strDateRegion, null, true, "FS0623");



                    if (checkRes != null)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = checkRes;
                        apiResult.flag = Convert.ToInt32(ERROR_FLAG.单元格定位提示);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }

                //string strErrorName = "";
                //首先增加订单方式
                if (dtadd.Rows.Count > 0)
                {
                    if (fs0623_Logic.AddOrderGoods(dtadd, loginInfo.UserId))
                    {
                        //增加新增中间表数据
                        if (dtaddZJB.Rows.Count > 0)
                        {
                            fs0623_Logic.AddOrderGoodsAndDifferentiation(dtaddZJB, loginInfo.UserId);
                        }
                    }
                }
                if (dtamodify.Rows.Count > 0)
                {
                    fs0623_Logic.UpdateOrderGoodsAndDifferentiation(dtamodify, dtamodifyZJB, loginInfo.UserId);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06UE2305", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存订单区分失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 删除
        [HttpPost]
        [EnableCors("any")]
        public string delApi([FromBody] dynamic data)
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
                List<Dictionary<string, Object>> listInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();
                if (listInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选择一条数据！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fs0623_Logic.Del(listInfoData, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M06E2303", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除订单区分失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }
}

