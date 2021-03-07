using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;

namespace SPPSApi.Controllers.G05
{
    [Route("api/FS0502/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0502Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string FunctionID = "FS0502";
        FS0502_Logic fs0502_Logic = new FS0502_Logic();

        public FS0502Controller(IWebHostEnvironment webHostEnvironment)
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
                List<Object> dataList_C056 = ComFunction.convertAllToResult(fs0502_Logic.getTCode("C056"));//状态
                List<Object> dataList_OrderNo = ComFunction.convertAllToResult(fs0502_Logic.getOrderNo(loginInfo.UserId));//订单号
                res.Add("C056", dataList_C056);
                res.Add("OrderNo", dataList_OrderNo);
                res.Add("vcSupplier_id", loginInfo.UserId);

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

        #region 查看子画面初始化
        [HttpPost]
        [EnableCors("any")]
        public string initSubApi([FromBody]dynamic data)
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
            string iAutoId = dataForm.iAutoId;

            try
            {
                DataTable dt = fs0502_Logic.initSubApi(iAutoId);

                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("dSendTime", ConvertFieldType.DateType, "yyyy/MM/dd HH:mm");
                dtConverter.addField("dSupReplyTime", ConvertFieldType.DateType, "yyyy/MM/dd HH:mm");

                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0204", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "子页面初始化失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 检索数据
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

            string vcSupplier_id = dataForm.vcSupplier_id == null ? "" : dataForm.vcSupplier_id;
            string vcStatus = dataForm.vcStatus == null ? "" : dataForm.vcStatus;
            string vcOrderNo = dataForm.vcOrderNo == null?"": dataForm.vcOrderNo;
            string vcPart_id = dataForm.vcPart_id == null ? "" : dataForm.vcPart_id;
            string vcDelete= dataForm.vcDelete == null ? "" : dataForm.vcDelete;

            try
            {
                Dictionary<string, object> res = new Dictionary<string, object>();
                DataTable dt = fs0502_Logic.Search(vcSupplier_id, vcStatus, vcOrderNo, vcPart_id, vcDelete);
                int dhfNum = dt.Rows.Count;//待回复条数
                string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                DataRow[] drs = dt.Select(" '"+now+"' > dReplyOverDate ");
                int yyqNum = drs.Length;//已逾期条数

                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("dReplyOverDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("dDeliveryDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);

                res.Add("dataList", dataList);
                res.Add("dhfNum", dhfNum);
                res.Add("yyqNum", yyqNum);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M04UE0202", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 分批纳入子画面检索数据
        [HttpPost]
        [EnableCors("any")]
        public string searchSubApi([FromBody] dynamic data)
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

            string iAutoId = dataForm.iAutoId == null ? "" : dataForm.iAutoId;
            string vcOrderNo= dataForm.vcOrderNo == null ? "" : dataForm.vcOrderNo;
            string vcPart_id= dataForm.vcPart_id == null ? "" : dataForm.vcPart_id;
            string vcSupplier_id= dataForm.vcSupplier_id == null ? "" : dataForm.vcSupplier_id;

            try
            {
                DataTable dt = fs0502_Logic.SearchSub(vcOrderNo,vcPart_id,vcSupplier_id);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("dDeliveryDate", ConvertFieldType.DateType, "yyyy/MM/dd");
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M04UE0202", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 导出
        [HttpPost]
        [EnableCors("any")]
        public string exportApi([FromBody] dynamic data)
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

            string vcSupplier_id = dataForm.vcSupplier_id == null ? "" : dataForm.vcSupplier_id;
            string vcStatus = dataForm.vcStatus == null ? "" : dataForm.vcStatus;
            string vcOrderNo = dataForm.vcOrderNo == null ? "" : dataForm.vcOrderNo;
            string vcPart_id = dataForm.vcPart_id == null ? "" : dataForm.vcPart_id;
            string vcDelete = dataForm.vcDelete == null ? "" : dataForm.vcDelete;

            try
            {
                DataTable dt = fs0502_Logic.Search(vcSupplier_id, vcStatus, vcOrderNo, vcPart_id,vcDelete);
                string[] heads = { "状态", "订单编号", "品番", "供应商代码","工区","回复截至日期","收容数(个)","订货总数(个)","可对应数量(个)","箱数","纳期"};
                string[] fields = { "vcStatusName","vcOrderNo","vcPart_id","vcSupplier_id","vcGQ","dReplyOverDate","iPackingQty","iOrderQuantity",
                    "iDuiYingQuantity","decBoxes","dDeliveryDate"};
                string strMsg = "";
                string filepath = ComFunction.DataTableToExcel(heads, fields, dt, _webHostEnvironment.ContentRootPath, loginInfo.UserId, FunctionID, ref strMsg);
                if (strMsg != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = strMsg;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = filepath;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M08UE0703", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "导出失败";
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
                string vcSupplier_id = dataForm.vcSupplier_id == null ? "" : dataForm.vcSupplier_id;
                JArray listInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
                bool hasFind = false;//是否找到需要新增或者修改的数据
                List<string> lsYearMonth = new List<string>();
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
                string strMsg_status = "";
                string strMsg_null = "";
                if (!hasFind)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少有一个编辑行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                
                //开始数据验证
                if (hasFind)
                {
                    #region 数据格式校验
                    string[,] strField = new string[,]
                    {
                        {"可对应数量(个)","纳期"},//中文字段名
                        {"iDuiYingQuantity","dDeliveryDate"},//英文字段名
                        {FieldCheck.Num,FieldCheck.Date},//数据类型校验
                        {"0","0"},//最大长度设定,不校验最大长度用0
                        {"1","1"},//最小长度设定,可以为空用0
                        {"9","11"},//前台显示列号，从0开始计算,注意有选择框的是0
                    };
                    List<Object> checkRes = ListChecker.validateList(listInfoData, strField, null, null, true, "FS0502");
                    if (checkRes != null)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = checkRes;
                        apiResult.flag = Convert.ToInt32(ERROR_FLAG.单元格定位提示);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    #endregion

                    if (fs0502_Logic.IsDQR(listInfoData, ref strMsg_status, ref strMsg_null, "save"))
                    {//全是可操作的数据
                        //继续向下执行
                    }
                    else
                    {//有不可以操作的数据
                        apiResult.code = ComConstant.ERROR_CODE;
                        if (strMsg_status.Length > 0)
                        {
                            apiResult.data = "以下品番不可操作：" + strMsg_status;
                        }
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                string strErrorPartId = "";
                string infopart = "";
                fs0502_Logic.Save(listInfoData, loginInfo.UserId, ref strErrorPartId,"","","",vcSupplier_id,ref infopart);
                if (strErrorPartId != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "保存失败，以下品番可对应数量大于订货总数：<br/>" + strErrorPartId;
                    apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (infopart != "")
                {
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.type = "information";
                    apiResult.data = "请按收容数调整：<br/>" + infopart;
                }
                else
                {
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.data = null;
                }
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M08UE0704", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 分批纳入子画面保存
        [HttpPost]
        [EnableCors("any")]
        public string saveSubApi([FromBody]dynamic data)
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
                string vcSupplier_id = dataForm.vcSupplier_id == null ? "" : dataForm.vcSupplier_id;
                string vcOrderNo = dataForm.vcOrderNo == null ? "" : dataForm.vcOrderNo;
                string vcPart_id = dataForm.vcPart_id == null ? "" : dataForm.vcPart_id;
                string iAutoId = dataForm.iAutoId == null ? "" : dataForm.iAutoId;
                //JArray listInfo = dataForm.multipleSelection;
                //List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();
                dynamic dSubform = dataForm.subform.list;//维护页面数据列表
                List<Dictionary<string, Object>> listSubInfo = dSubform.ToObject<List<Dictionary<string, Object>>>();
                bool hasFind = false;//是否找到需要新增或者修改的数据
                List<string> lsYearMonth = new List<string>();
                //for (int i = 0; i < listInfoData.Count; i++)
                //{
                //    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                //    bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                //    if (bAddFlag == true)
                //    {//新增
                //        hasFind = true;
                //    }
                //    else if (bAddFlag == false && bModFlag == true)
                //    {//修改
                //        hasFind = true;
                //    }
                //}
                //string strMsg = "";
                //if (!hasFind)
                //{
                //    apiResult.code = ComConstant.ERROR_CODE;
                //    apiResult.data = "最少有一个编辑行！";
                //    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                //}
                //else
                //{
                //    if (fs0502_Logic.IsDQR(listInfoData, ref strMsg, "save"))
                //    {//全是可操作的数据
                //        //继续向下执行
                //    }
                //    else
                //    {//有不可以操作的数据
                //        apiResult.code = ComConstant.ERROR_CODE;
                //        apiResult.data = "以下品番不可操作：" + strMsg;
                //        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                //    }
                //}
                //开始数据验证
                hasFind = true;
                if (hasFind)
                {
                    #region 数据格式校验
                    string[,] strField = new string[,]
                    {
                        {"可对应数量(个)","纳期"},//中文字段名
                        {"iDuiYingQuantity","dDeliveryDate"},//英文字段名
                        {FieldCheck.Num,FieldCheck.Date},//数据类型校验
                        {"0","0"},//最大长度设定,不校验最大长度用0
                        {"1","1"},//最小长度设定,可以为空用0
                        {"0","2"},//前台显示列号，从0开始计算,注意有选择框的是0
                    };
                    List<Object> checkRes = ListChecker.validateList(listSubInfo, strField, null, null, true, "FS0502_Sub");
                    if (checkRes != null)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = checkRes;
                        apiResult.flag = Convert.ToInt32(ERROR_FLAG.单元格定位提示);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    #endregion
                }

                string strErrorPartId = "";
                string infopart = "";
                fs0502_Logic.Save(listSubInfo, loginInfo.UserId, ref strErrorPartId, iAutoId, vcPart_id,vcOrderNo,vcSupplier_id,ref infopart);
                if (strErrorPartId != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "保存失败，以下品番可对应数量大于订货总数：<br/>" + strErrorPartId;
                    apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (infopart!="")
                {
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.type = "information";
                    apiResult.data = "请按收容数调整：<br/>" + infopart;
                }
                else
                {
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.data = null;
                }
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M08UE0704", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 回复纳期
        [HttpPost]
        [EnableCors("any")]
        public string okApi([FromBody] dynamic data)
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
                string vcSupplier_id = dataForm.vcSupplier_id == null ? "" : dataForm.vcSupplier_id;
                string vcStatus = dataForm.vcStatus == null ? "" : dataForm.vcStatus;
                string vcOrderNo = dataForm.vcOrderNo == null ? "" : dataForm.vcOrderNo;
                string vcPart_id = dataForm.vcPart_id == null ? "" : dataForm.vcPart_id;
                JArray checkedInfo = dataForm.multipleSelection;
                List<Dictionary<string, Object>> listInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();

                if (vcSupplier_id == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "供应商不能为空";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                int count = 0;//影响行数，没啥用
                string strMsg_status = "";
                string strMsg_null = "";
                if (listInfoData.Count != 0)//选中了数据操作
                {
                    if (fs0502_Logic.IsDQR( listInfoData, ref strMsg_status,ref strMsg_null, "submit"))
                    {//全是可操作的数据
                        // 执行提交操作：按所选数据提交
                        fs0502_Logic.ok( listInfoData, loginInfo.UserId);
                    }
                    else
                    {//有不可以操作的数据
                        apiResult.code = ComConstant.ERROR_CODE;
                        if(strMsg_status.Length>0)
                        {
                            apiResult.data = "以下品番不可操作：" + strMsg_status;
                        }
                        if (strMsg_null.Length > 0)
                        {
                            apiResult.data = "以下品番没填写对应数量/纳期：" + strMsg_null;
                        }
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                else//按检索条件
                {
                    if (fs0502_Logic.IsDQR(vcSupplier_id, vcStatus, vcOrderNo, vcPart_id, ref strMsg_status, ref strMsg_null))
                    {//全是可操作的数据
                        //执行提交操作：按检索条件提交
                        fs0502_Logic.ok(vcSupplier_id, vcStatus, vcOrderNo, vcPart_id, loginInfo.UserId);
                    }
                    else
                    {//有不可以操作的数据
                        apiResult.code = ComConstant.ERROR_CODE;
                        if (strMsg_status.Length > 0)
                        {
                            apiResult.data = "以下品番不可操作：" + strMsg_status;
                        }
                        if (strMsg_null.Length > 0)
                        {
                            apiResult.data = "以下品番没填写对应数量/纳期：" + strMsg_null;
                        }
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = count;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M04UE0203", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 分批纳入子画面删除  不用
        [HttpPost]
        [EnableCors("any")]
        public string delSubApi([FromBody]dynamic data)
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
                string strErrorPartId = "";
                fs0502_Logic.DelSub(listInfoData, loginInfo.UserId,ref strErrorPartId);
                if (strErrorPartId != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "保存失败，以下品番可对应数量与订货总数不匹配：<br/>" + strErrorPartId;
                    apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M08UE1008", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

    }
}