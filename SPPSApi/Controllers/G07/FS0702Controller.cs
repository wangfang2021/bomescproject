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


                List<Object> dataList_C069 = ComFunction.convertAllToResult(ComFunction.getTCode("C069"));//
                res.Add("C069", dataList_C069);

                List<Object> dataList_C098 = ComFunction.convertAllToResult(ComFunction.getTCode("C098"));//
                res.Add("C098", dataList_C098);

                List<Object> dataList_C018 = ComFunction.convertAllToResult(ComFunction.getTCode("C018"));//收货方
                res.Add("C018", dataList_C018);

                List<Object> dataList_Supplier = ComFunction.convertAllToResult(FS0702_Logic.SearchSupplier());//供应商
                res.Add("optionSupplier", dataList_Supplier);

                List<Object> dataList_Note = ComFunction.convertAllToResult(FS0702_Logic.SearchNote());//变更事项
                res.Add("optionNote", dataList_Note);

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


            List<Object> Note = new List<object>();

            if (dataForm.Note.ToObject<List<Object>>() == null)
            {
                Note = new List<object>();
            }
            else
            {
                Note = dataForm.Note.ToObject<List<Object>>();
            }

            List<Object> PackSpot = new List<object>();

            if (dataForm.PackSpot.ToObject<List<Object>>() == null)
            {
                PackSpot = new List<object>();
            }
            else
            {
                PackSpot = dataForm.PackSpot.ToObject<List<Object>>();
            }


            List<Object> Shouhuofang = new List<object>();
            if (dataForm.Shouhuofang.ToObject<List<Object>>() == null)
            {
                Shouhuofang = new List<object>();
            }
            else
            {
                Shouhuofang = dataForm.Shouhuofang.ToObject<List<Object>>();
            }

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
            string iautoID = "";
            //变更事项
            List<Object> strNote = new List<object>();

            if (dataForm.Note.ToObject<List<Object>>() == null)
            {
                strNote = new List<object>();
            }
            else
            {
                strNote = dataForm.Note.ToObject<List<Object>>();
            }

            List<Object> strPackSpot = new List<object>();//包装厂

            if (dataForm.PackSpot.ToObject<List<Object>>() == null)
            {
                strPackSpot = new List<object>();
            }
            else
            {
                strPackSpot = dataForm.PackSpot.ToObject<List<Object>>();
            }
           // 收货方
            List<Object> Shouhuofang = new List<object>();
            if (dataForm.Shouhuofang.ToObject<List<Object>>() == null)
            {
                Shouhuofang = new List<object>();
            }
            else
            {
                Shouhuofang = dataForm.Shouhuofang.ToObject<List<Object>>();
            }
            



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
                DataTable dt = FS0702_Logic.SearchEXZ(iautoID, strNote, strPackSpot, Shouhuofang, strPartsNo, strCar, strPackNO, strPackGPSNo, strFromBegin, strFromEnd, strToBegin, strToEnd);
                string resMsg = "";
                string[] head = { "变更事项", "包装场", "收货方", "品番,", "车型", "开始时间", "结束时间", "包材品番", "GPS品番", "开始时间", "结束时间", "包装材区分", "必要数"};

                string[] fields = { "varChangedItem","vcPackSpot","vcShouhuofangID","vcPartsNo","vcCar","dUsedFrom","dUsedTo","vcPackNo",
                    "vcPackGPSNo","dFrom","dTo","vcDistinguish","iBiYao"
                };

                string filepath = ComFunction.DataTableToExcel(head, fields, dt, _webHostEnvironment.ContentRootPath, loginInfo.UserId, "包材基础数据导出", ref resMsg);
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


        #region 导出导入模板
        [HttpPost]
        [EnableCors("any")]
        public string exportApi_import([FromBody] dynamic data)
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


            //变更事项
            List<Object> strNote = new List<object>();

            if (dataForm.Note.ToObject<List<Object>>() == null)
            {
                strNote = new List<object>();
            }
            else
            {
                strNote = dataForm.Note.ToObject<List<Object>>();
            }

            List<Object> strPackSpot = new List<object>();//包装厂

            if (dataForm.PackSpot.ToObject<List<Object>>() == null)
            {
                strPackSpot = new List<object>();
            }
            else
            {
                strPackSpot = dataForm.PackSpot.ToObject<List<Object>>();
            }
            // 收货方
            List<Object> Shouhuofang = new List<object>();
            if (dataForm.Shouhuofang.ToObject<List<Object>>() == null)
            {
                Shouhuofang = new List<object>();
            }
            else
            {
                Shouhuofang = dataForm.Shouhuofang.ToObject<List<Object>>();
            }




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
                DataTable dt = FS0702_Logic.SearchEXZ("", strNote, strPackSpot, Shouhuofang, strPartsNo, strCar, strPackNO, strPackGPSNo, strFromBegin, strFromEnd, strToBegin, strToEnd);
                string resMsg = "";
                string[] head = {"导入状态","对应标识", "变更事项", "包装场", "收货方", "品番,", "车型", "开始时间(部品)", "结束时间(部品)", "包材品番", "GPS品番", "开始时间", "结束时间", "包装材区分", "必要数" };

                string[] fields = {"vcIsorNo","iAutoId", "varChangedItem","vcPackSpot","vcShouhuofangID","vcPartsNo","vcCar","dUsedFrom","dUsedTo","vcPackNo",
                    "vcPackGPSNo","dFrom","dTo","vcDistinguish","iBiYao"
                };
                string filepath = ComFunction.DataTableToExcel(head, fields, dt, _webHostEnvironment.ContentRootPath, loginInfo.UserId, "包材基础数据导出", ref resMsg);
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
            //变更事项
            List<Object> strNote = new List<object>();

            if (dataForm.Note.ToObject<List<Object>>() == null)
            {
                strNote = new List<object>();
            }
            else
            {
                strNote = dataForm.Note.ToObject<List<Object>>();
            }

            List<Object> strPackSpot = new List<object>();//包装厂

            if (dataForm.PackSpot.ToObject<List<Object>>() == null)
            {
                strPackSpot = new List<object>();
            }
            else
            {
                strPackSpot = dataForm.PackSpot.ToObject<List<Object>>();
            }
            // 收货方
            List<Object> Shouhuofang = new List<object>();
            if (dataForm.Shouhuofang.ToObject<List<Object>>() == null)
            {
                Shouhuofang = new List<object>();
            }
            else
            {
                Shouhuofang = dataForm.Shouhuofang.ToObject<List<Object>>();
            }




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
                DataTable dt = FS0702_Logic.SearchEXZ(iautoID, strNote, strPackSpot, Shouhuofang, strPartsNo, strCar, strPackNO, strPackGPSNo, strFromBegin, strFromEnd, strToBegin, strToEnd);

                DataTable dtcope = dt.Copy();
                dtcope.Clear();

                #region 处理导出数据
                DataTable dt_EX = new DataTable();
                dt_EX.Columns.Add("varChangedItem", Type.GetType("System.String"));
                dt_EX.Columns.Add("vcPackSpot", Type.GetType("System.String"));
                dt_EX.Columns.Add("vcPackSpot", Type.GetType("System.String"));







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

                    bool isok = FS0702_Logic.CheckPartsNo(listInfoData[i]["vcShouhuofangID"].ToString(), listInfoData[i]["vcPartsNo"].ToString());
                    if (!isok)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "品番:" + listInfoData[i]["vcPartsNo"].ToString() + "不存在！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (listInfoData[i]["dFrom"].ToString() == "" || listInfoData[i]["dTo"].ToString() == "")
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "请输入开始/结束时间！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                    }
                    int iAutoId = listInfoData[i]["iAutoId"] == "" ? 0 : Convert.ToInt32(listInfoData[i]["iAutoId"]);
                    DataTable dtcheckTime = FS0702_Logic.searchcheckTime(listInfoData[i]["vcPackSpot"].ToString(), listInfoData[i]["vcPartsNo"].ToString(), 
                        listInfoData[i]["vcPackNo"].ToString(), listInfoData[i]["dFrom"].ToString(), 
                        listInfoData[i]["dTo"].ToString(), iAutoId,listInfoData[i]["vcShouhuofangID"].ToString());
                    if (dtcheckTime.Rows.Count > 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "品番有维护重复有效时间！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }

                }
                if (!hasFind)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少有一个编辑行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                List<Object> strSupplierCode = new List<object>();
                FS0701_Logic FS0701_Logic = new FS0701_Logic();
                DataTable dt = FS0702_Logic.Search_1();
                DataTable dt1 = FS0701_Logic.Search_1();
                string strErrorPartId = "";
                FS0702_Logic.Save(listInfoData, loginInfo.UserId, ref strErrorPartId, dt, dt1);
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
                DataTable dtcheck = new DataTable();
                //check
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    dtcheck = FS0702_Logic.checkSOQ(listInfoData[i]["vcPartsNo"].ToString());

                    if (dtcheck.Rows.Count <2)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = dtcheck.Rows[i]["vcPart_id"] + "存在上游不可删除！";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
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

                if (!fp0018.main(loginInfo.UserId))
                {

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
