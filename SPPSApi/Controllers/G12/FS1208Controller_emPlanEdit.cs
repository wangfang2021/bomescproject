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

namespace SPPSApi.Controllers.G12
{
    [Route("api/FS1208_emPlanEdit/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1208Controller_emPlanEdit : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        FS1208_Logic logic = new FS1208_Logic();
        private readonly string FunctionID = "FS1208";

        public FS1208Controller_emPlanEdit(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

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
            string vcMon = dataForm.vcMon;
            string vcPartsNo = dataForm.vcPartsNo;
            string vcCarType = dataForm.vcCarType;
            string vcDock = dataForm.vcDock;
            string vcType = dataForm.vcType;
            string vcPro = dataForm.vcPro;
            string vcZhi = dataForm.vcZhi;
            string vcDay = dataForm.vcDay;
            string vcOrder = dataForm.vcOrder;
            vcMon = vcMon == null ? "" : vcMon;
            vcPartsNo = vcPartsNo == null ? "" : vcPartsNo;
            vcCarType = vcCarType == null ? "" : vcCarType;
            vcDock = vcDock == null ? "" : vcDock;
            vcType = vcType == null ? "" : vcType;
            vcPro = vcPro == null ? "" : vcPro;
            vcZhi = vcZhi == null ? "" : vcZhi;
            vcDay = vcDay == null ? "" : vcDay;
            vcOrder = vcOrder == null ? "" : vcOrder;
            try
            {
                Exception ex = new Exception();
                DataTable dt = logic.getEDPlanInfo(vcMon, vcPartsNo, vcCarType, vcDock, vcType, vcPro, vcZhi, vcDay, vcOrder);
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
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
                logic.Del_Order(listInfoData, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0909", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除公式失败";
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
                    hasFind = true;
                }
                if (!hasFind)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少有一个编辑行！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //开始数据验证
                if (hasFind)
                {
                    string[,] strField = new string[,] {{"iAutoId","对象月","工厂","品番","受入","车型","数量","看板订单号","工程0日期","工程0值别","工程1日期","工程1值别","工程2日期","工程2值别","工程3日期","工程3值别","工程4日期","工程4值别","状态"},
                                                {"iAutoId","vcMonth","vcPlant","vcPartsno","vcDock","vcCarType","vcNum","vcOrderNo","vcPro0Day","vcPro0Zhi","vcPro1Day","vcPro1Zhi","vcPro2Day","vcPro2Zhi","vcPro3Day","vcPro3Zhi","vcPro4Day","vcPro4Zhi","vcState"},
                                                {"","","","","","",FieldCheck.Num,"",FieldCheck.Date,"",FieldCheck.Date,"",FieldCheck.Date,"",FieldCheck.Date,"",FieldCheck.Date,"",""},
                                                {"0","7","7","12","2","5","10","12","10","2","10","2","10","2","10","2","10","2","3"},//最大长度设定,不校验最大长度用0
                                                {"0","7","1","12","2","1","1","1","0","0","0","0","0","0","0","0","0","0","2"},//最小长度设定,可以为空用0
                                                {"1","2","3","4","5","6","7","8","9","10","11","12","13","14","15","16","17","18","19"}//前台显示列号，从0开始计算,注意有选择框的是0
                    };

                }
                DataTable tb = ListToDataTable(listInfoData);
                if (tb != null && tb.Rows.Count > 0)
                {
                    string strMsg = logic.UpdateTable(tb, loginInfo.UserId);
                    if (strMsg != "")
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = strMsg;
                        apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.data = strMsg;
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "无数据更新！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0908", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "保存失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 更新到计划
        [HttpPost]
        [EnableCors("any")]
        public string updatePlan([FromBody] dynamic data)
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
            string strMon = dataForm.vcMon;
            try
            {
                string strMsg = "";
                strMsg = logic.UpdatePlan(strMon, loginInfo.UserId);
                if (strMsg != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = strMsg;
                    apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "计划削减成功！";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "计划削减失败！";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 通用方法
        public DataTable ListToDataTable(List<Dictionary<string, Object>> listInfoData)
        {
            try
            {
                DataTable tb = new DataTable();
                if (listInfoData.Count > 0)
                {
                    Dictionary<string, object> li = listInfoData[0];
                    for (int i = 0; i < li.Count; i++)
                    {
                        string colName = li.ToList()[i].Key;
                        Type colType = li.ToList()[i].Value == null ? typeof(System.String) : li.ToList()[i].Value.GetType();
                        tb.Columns.Add(new DataColumn(colName, colType));
                    }
                    foreach (Dictionary<string, object> li1 in listInfoData)
                    {
                        if ((li1["vcModFlag"] != null && Convert.ToBoolean(li1["vcModFlag"]) == true && li1["vcAddFlag"] != null && Convert.ToBoolean(li1["vcAddFlag"]) == false)
                            ||
                            (li1["vcModFlag"] != null && Convert.ToBoolean(li1["vcModFlag"]) == true && li1["vcAddFlag"] != null && Convert.ToBoolean(li1["vcAddFlag"]) == true))
                        {
                            DataRow r = tb.NewRow();
                            for (int j = 0; j < tb.Columns.Count; j++)
                            {
                                r[j] = li1[tb.Columns[j].ColumnName] == null ? "" : li1[tb.Columns[j].ColumnName].ToString();
                            }
                            tb.Rows.Add(r);
                        }
                    }
                }
                return tb;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
