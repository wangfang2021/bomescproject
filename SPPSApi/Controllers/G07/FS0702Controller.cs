using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SPPSApi.Controllers.G07
{
    [Route("api/FS0702/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0702Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0702_Logic FS0702_Logic = new FS0702_Logic();
        private readonly string FunctionID = "FS0702";

        public FS0702Controller(IWebHostEnvironment webHostEnvironment)
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
                Dictionary<string, object> res = new Dictionary<string, object>();

                List<Object> dataList_C023 = ComFunction.convertAllToResult(ComFunction.getTCode("C023"));//包装场
                res.Add("C023", dataList_C023);


                List<Object> dataList_C098 = ComFunction.convertAllToResult(ComFunction.getTCode("C098"));//车型
                res.Add("C098", dataList_C098);

                List<Object> dataList_C018 = ComFunction.convertAllToResult(ComFunction.getTCode("C018"));//收货方
                res.Add("C018", dataList_C018);

                List<Object> dataList_Supplier = ComFunction.convertAllToResult(FS0702_Logic.SearchSupplier());//供应商
                res.Add("optionSupplier", dataList_Supplier);

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

        #region 表格中收货方下拉列表加载
        [HttpPost]
        [EnableCors("any")]
        public string InitPageApi()
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

                List<Object> dataList_C018 = ComFunction.convertAllToResult(ComFunction.getTCode("C018"));//收货方
                res.Add("C018", dataList_C018);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M00UE0006", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "收货方下拉列表加载失败";
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

            string Note = dataForm.Note;
            string PackSpot = dataForm.PackSpot;
            string Shouhuofang = dataForm.Shouhuofang;
            string Pinfan = dataForm.PartsNo;
            string Car = dataForm.Car;
            string PackNO = dataForm.PackNO;
            string PackGPSNo = dataForm.PackGPSNo;
            string dtFromBegin = dataForm.dtFromBegin;
            string dtFromEnd = dataForm.dtFromEnd;
            string dtToBegin = dataForm.dtToBegin;
            string dtToEnd = dataForm.dtToEnd;
            try
            {
                DataTable dt = FS0702_Logic.Search(Note, PackSpot, Shouhuofang, Pinfan, Car, PackNO, PackGPSNo, dtFromBegin, dtFromEnd, dtToBegin, dtToEnd);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("dUseFrom", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dUseTo", ConvertFieldType.DateType, "yyyy/MM/dd");
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

        #region 导出纵版
        [HttpPost]
        [EnableCors("any")]
        public string exportApi_Z([FromBody] dynamic data)
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

            //dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
            //JArray listInfo = dataForm.multipleSelection;
            //List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
            string iautoID = "";
            //if (listInfoData.Count == 0)
            //{
            //    apiResult.code = ComConstant.ERROR_CODE;
            //    apiResult.data = "没有可导出数据！";
            //    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            //}
            //for (int i = 0; i < listInfoData.Count; i++)
            //{
            //    if (i == listInfoData.Count - 1)
            //    {
            //        iautoID = iautoID + listInfoData[i]["iAutoId"].ToString();

            //    }
            //    else
            //        iautoID = iautoID + listInfoData[i]["iAutoId"].ToString() + ",";
            //}
            string strNote = dataForm.Note;//变更事项
            string strPackSpot = dataForm.dataForm;//包装厂
            string strShouhuofang = dataForm.Shouhuofang;//收货方
            string strPartsNo = dataForm.PartsNo;//品番
            string strCar = dataForm.Car;//车型
            string strPackNO = dataForm.PackNO;//包装材品番
            string strPackGPSNo = dataForm.PackGPSNo;//GPS品番
            string strFromBegin = dataForm.dFromBegin;//From开始
            string strFromEnd = dataForm.dFromEnd;//From结束
            string strToBegin = dataForm.dToBegin;//To开始
            string strToEnd = dataForm.dToEnd;//To结束

            try
            {
                DataTable dt = FS0702_Logic.SearchEXZ(iautoID, strNote, strPackSpot, strShouhuofang, strPartsNo, strCar, strPackNO, strPackGPSNo, strFromBegin, strFromEnd, strToBegin, strToEnd);
                string[] fields = { "varChangedItem","vcPackSpot","vcShouhuofang","vcPartsNo","vcCar","dUsedFrom","dUsedTo","vcPackNo",
                    "vcPackGPSNo","dFrom","dTo","vcDistinguish","iBiYao"
                };
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0702_Export.xlsx", 1, loginInfo.UserId, FunctionID);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0904", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 导出横版
        [HttpPost]
        [EnableCors("any")]
        public string exportApi_H([FromBody] dynamic data)
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
            //ApiResult apiResult = new ApiResult();
            //dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
            //JArray listInfo = dataForm.multipleSelection;
            //if (listInfo == null)
            //{
            //    apiResult.code = ComConstant.ERROR_CODE;
            //    apiResult.data = "没有可导出数据！";
            //    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            //}
            //List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
            string iautoID = "";
            //for (int i = 0; i < listInfoData.Count; i++)
            //{
            //    if (i == listInfoData.Count - 1)
            //    {
            //        iautoID = iautoID + listInfoData[i]["iAutoId"].ToString();

            //    }
            //    else
            //        iautoID = iautoID + listInfoData[i]["iAutoId"].ToString() + ",";
            //}
            string strNote = dataForm.Note;//变更事项
            string strPackSpot = dataForm.dataForm;//包装厂
            string strShouhuofang = dataForm.Shouhuofang;//收货方
            string strPartsNo = dataForm.PartsNo;//品番
            string strCar = dataForm.Car;//车型
            string strPackNO = dataForm.PackNO;//包装材品番
            string strPackGPSNo = dataForm.PackGPSNo;//GPS品番
            string strFromBegin = dataForm.dFromBegin;//From开始
            string strFromEnd = dataForm.dFromEnd;//From结束
            string strToBegin = dataForm.dToBegin;//To开始
            string strToEnd = dataForm.dToEnd;//To结束

            try
            {
                DataTable dt = FS0702_Logic.SearchEXZ(iautoID, strNote, strPackSpot, strShouhuofang, strPartsNo, strCar, strPackNO, strPackGPSNo, strFromBegin, strFromEnd, strToBegin, strToEnd);
                
                DataTable dtcope = dt.Copy();
                dtcope.Clear();

                #region 处理导出数据
                int maxcolumn = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 1; j < dt.Rows.Count; j++)
                    {
                        if (dt.Rows[i]["vcPartsNo"].ToString() == dt.Rows[j]["vcPartsNo"].ToString())
                        {
                            if (maxcolumn < j)
                            {
                                dt.Columns.Add("vcPackNo" + j, typeof(System.String));
                                dt.Columns.Add("vcPackGPSNo" + j, typeof(System.String));
                                dt.Columns.Add("dFrom" + j, typeof(System.String));
                                dt.Columns.Add("dTo" + j, typeof(System.String));
                                dt.Columns.Add("vcDistinguish" + j, typeof(System.String));
                                dt.Columns.Add("iBiYao" + j, typeof(System.String));
                                dtcope.Columns.Add("vcPackNo" + j, typeof(System.String));
                                dtcope.Columns.Add("vcPackGPSNo" + j, typeof(System.String));
                                dtcope.Columns.Add("dFrom" + j, typeof(System.String));
                                dtcope.Columns.Add("dTo" + j, typeof(System.String));
                                dtcope.Columns.Add("vcDistinguish" + j, typeof(System.String));
                                dtcope.Columns.Add("iBiYao" + j, typeof(System.String));
                                dt.Rows[i]["vcPackNo" + j] = dt.Rows[j]["vcPackNo"].ToString();
                                dt.Rows[i]["vcPackGPSNo" + j] = dt.Rows[j]["vcPackGPSNo"].ToString();
                                dt.Rows[i]["dFrom" + j] = dt.Rows[j]["dFrom"].ToString();
                                dt.Rows[i]["dTo" + j] = dt.Rows[j]["dTo"].ToString();
                                dt.Rows[i]["vcDistinguish" + j] = dt.Rows[j]["vcDistinguish"].ToString();
                                dt.Rows[i]["iBiYao" + j] = dt.Rows[j]["iBiYao"].ToString();
                                dtcope.Rows.Add(dt.Rows[i]);
                                maxcolumn++;
                            }
                            else
                            {
                                dt.Rows[i]["vcPackNo" + j] = dt.Rows[j]["vcPackNo"].ToString();
                                dt.Rows[i]["vcPackGPSNo" + j] = dt.Rows[j]["vcPackGPSNo"].ToString();
                                dt.Rows[i]["dFrom" + j] = dt.Rows[j]["dFrom"].ToString();
                                dt.Rows[i]["dTo" + j] = dt.Rows[j]["dTo"].ToString();
                                dt.Rows[i]["vcDistinguish" + j] = dt.Rows[j]["vcDistinguish"].ToString();
                                dt.Rows[i]["iBiYao" + j] = dt.Rows[j]["iBiYao"].ToString();
                                dtcope.Rows.Add(dt.Rows[i]);
                            }
                        }
                        else
                        {
                            dtcope.Rows.Add(dt.Rows[i]);
                            for (int z=0;z<j;z++) {
                                dt.Rows.RemoveAt(z);
                            }
                            break;
                        }
                    }
                }
                #endregion

                string[] fields = { "varChangedItem","vcPackSpot","vcShouhuofang","vcPartsNo","vcCar","dUsedFrom","dUsedTo",
                    "vcPackNo1","vcPackGPSNo1","dFrom","dTo","vcDistinguish","iBiYao",
                    "vcPackNo2","vcPackGPSNo2","dFrom","dTo","vcDistinguish","iBiYao",
                    "vcPackNo3","vcPackGPSNo3","dFrom","dTo","vcDistinguish","iBiYao",
                    "vcPackNo4","vcPackGPSNo4","dFrom","dTo","vcDistinguish","iBiYao",
                    "vcPackNo5","vcPackGPSNo5","dFrom","dTo","vcDistinguish","iBiYao",
                    "vcPackNo6","vcPackGPSNo6","dFrom","dTo","vcDistinguish","iBiYao",
                    "vcPackNo7","vcPackGPSNo7","dFrom","dTo","vcDistinguish","iBiYao",
                    "vcPackNo8","vcPackGPSNo8","dFrom","dTo","vcDistinguish","iBiYao",
                    "vcPackNo9","vcPackGPSNo9","dFrom","dTo","vcDistinguish","iBiYao",
                    "vcPackNo10","vcPackGPSNo10","dFrom","dTo","vcDistinguish","iBiYao",
                };
                string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0702_Export_H.xlsx", 2, loginInfo.UserId, FunctionID);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0904", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
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
                    //判断品番是否存在
                    bool isok = true;
                   // bool isok = FS0702_Logic.CheckPartsNo(listInfoData[i]["vcShouhuofang"].ToString(), listInfoData[i]["vcPartsNo"].ToString(), listInfoData[i]["vcPackSpot"].ToString());
                    if (!isok)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "品番:" + listInfoData[i]["vcPartsNo"].ToString() + "有误！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (listInfoData[i]["dFrom"].ToString()==""|| listInfoData[i]["dTo"].ToString() == "") {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "请输入开始/结束时间！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                    }
                }
                if (!hasFind)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少有一个编辑行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                string strErrorPartId = "";
                FS0702_Logic.Save(listInfoData, loginInfo.UserId, ref strErrorPartId);
                if (strErrorPartId != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "保存失败";
                    apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0902", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
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
                FS0702_Logic.Del(listInfoData, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0903", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 同步
        [HttpPost]
        [EnableCors("any")]
        public string PFReplaceapi([FromBody] dynamic data)
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
                //JArray checkedInfo = dataForm.multipleSelection;
                //List<Dictionary<string, Object>> listInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();
                BatchProcess.FP0018 fp0018 = new BatchProcess.FP0018();

                if (!fp0018.main(loginInfo.UserId)) {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "更新失败";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0903", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "更新失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion


    }
}
