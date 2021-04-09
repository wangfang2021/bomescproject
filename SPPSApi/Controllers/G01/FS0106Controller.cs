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
 


namespace SPPSApi.Controllers.G00
{
    [Route("api/FS0106/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0106Controller : BaseController
    {
        FS0106_Logic fs0106_Logic = new FS0106_Logic();
        private readonly string FunctionID = "FS0106";
        
        private readonly IWebHostEnvironment _webHostEnvironment;
        

        public FS0106Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        #region 绑定包装厂
        [HttpPost]
        [EnableCors("any")]
        public string bindConst()
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
                DataTable dt = fs0106_Logic.BindConst();
                List<Object> dataList = ComFunction.convertToResult(dt, new string[] { "vcCodeId", "vcCodeName" });
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0601", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "绑定常量列表失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
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
            string vcCodeId = dataForm.vcCodeId == null ? "" : dataForm.vcCodeId;
           
            try
            {
                if (vcCodeId.Length==0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请选择常量区分，再检索数据！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                DataSet ds = fs0106_Logic.Search(vcCodeId);
                DataTable dtContent = ds.Tables[0];
                DataTable dtColum = ds.Tables[1];
                if (dtColum==null||dtColum.Rows.Count==0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "该常量区分的数据列头没有维护，无法检索数据！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                DataTable dt = new DataTable();
                dt.Columns.Add("prop");
                dt.Columns.Add("label"); 
                
                // TypeCode, TypeName, KeyCode, KeyValue, Sort, Memo
                //构建检索的主体数据
                DataRow[] drArray = dtContent.Select("vcIsColum='1'");
                DataTable dtNewColum = drArray[0].Table.Clone(); // 复制DataRow的表结构
                foreach (DataRow dr in drArray)
                {
                    dtNewColum.ImportRow(dr);
                }
                //构建检索体
                DataRow[] drArrayContent = dtContent.Select("vcIsColum<>'1'");
                DataTable dtContentNew = dtContent.Clone(); // 复制DataRow的表结构
                foreach (DataRow dr in drArrayContent)
                {
                    dtContentNew.ImportRow(dr);
                }

                for (int i = 0; i < dtNewColum.Rows.Count; i++)
                {
                    for (int j= dtNewColum.Columns.Count-1; j>0; j--) {
                        if (dtNewColum.Rows[i][j] == null|| dtNewColum.Rows[i][j].ToString() == "")//删除此列
                        {
                            dtContentNew.Columns.RemoveAt(j);
                        }
                    }
                }
                //构建列头
                for (int i = 0; i < dtColum.Rows.Count; i++)
                {
                    for (int j = 0; j < dtColum.Columns.Count; j++)
                    {
                        if (dtColum.Rows[i][j]!=null&&dtColum.Rows[i][j].ToString()!="")
                        {
                            DataRow dr = dt.NewRow();
                            dr["prop"] = dtColum.Columns[j];
                            dr["label"] = dtColum.Rows[i][j].ToString();
                            dt.Rows.Add(dr);
                        }
                    }
                }
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);

                List<Object> dataList = ComFunction.convertAllToResultByConverter(dtContentNew, dtConverter);
                List<Object> colist = ComFunction.convertAllToResult(dt);
                var obbject = new object[] {
                    new { list=dataList},
                    new { colist=colist}
                };
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = obbject;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0602", ex, loginInfo.UserId);
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
            string vcCodeId = dataForm.vcCodeId == null ? "" : dataForm.vcCodeId;
           
            try
            {
                DataTable dt = null; 
               // DataSet dt = fs0106_Logic.Search(vcCodeId);
                
                if (vcCodeId == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请选择常量区分，否则无法导出数据！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                DataSet ds = fs0106_Logic.Search(vcCodeId);
                DataTable dtContent = ds.Tables[0];
                DataTable dtColum = ds.Tables[1];
                // TypeCode, TypeName, KeyCode, KeyValue, Sort, Memo
                //构建检索的主体数据
                DataRow[] drArray = dtContent.Select("vcIsColum='1'");
                DataTable dtNewColum = drArray[0].Table.Clone(); // 复制DataRow的表结构
                foreach (DataRow dr in drArray)
                {
                    dtNewColum.ImportRow(dr);
                }
                //构建检索体
                DataRow[] drArrayContent = dtContent.Select("vcIsColum<>'1'");
                DataTable dtContentNew = dtContent.Clone(); // 复制DataRow的表结构
                foreach (DataRow dr in drArrayContent)
                {
                    dtContentNew.ImportRow(dr);
                }

                for (int i = 0; i < dtNewColum.Rows.Count; i++)
                {
                    for (int j = dtNewColum.Columns.Count - 1; j > 0; j--)
                    {
                        if (dtNewColum.Rows[i][j] == null || dtNewColum.Rows[i][j].ToString() == "")//删除此列
                        {
                            dtNewColum.Columns.RemoveAt(j);
                            dtContentNew.Columns.RemoveAt(j);
                        }
                    }
                }
                
                dtNewColum.Columns.Remove("iAutoId");
                dtNewColum.Columns.Remove("vcIsColum");
                dtNewColum.Columns.Remove("vcModFlag");
                dtNewColum.Columns.Remove("vcAddFlag");
                dtNewColum.Columns.Remove("vcOperatorID");
                dtNewColum.Columns.Remove("dOperatorTime");

                dtContentNew.Columns.Remove("iAutoId");
                dtContentNew.Columns.Remove("vcIsColum");
                dtContentNew.Columns.Remove("vcModFlag");
                dtContentNew.Columns.Remove("vcAddFlag");
                dtContentNew.Columns.Remove("vcOperatorID");
                dtContentNew.Columns.Remove("dOperatorTime");
                string[] head = new string[dtNewColum.Columns.Count];
                string[] field = new string[dtNewColum.Columns.Count];
                for (int m=0;m< dtNewColum.Rows.Count;m++)
                {
                    dtNewColum.Rows[0][0] = "常量代码";
                    dtNewColum.Rows[0][1] = "常量名称";
                }
                for (int i=0;i< dtNewColum.Columns.Count;i++)
                {
                    head.SetValue(dtNewColum.Rows[0][i], i);
                }
                for (int i = 0; i < dtContentNew.Columns.Count; i++)
                {
                    field.SetValue(dtContentNew.Columns[i].ToString(), i);
                }
                //head = new string[] { "常量区分代码", "常量区分名称", "Key键", "Key值", "备注" };
                //field = new string[] { "vcCodeId", "vcCodeName", "vcValue", "vcName", "vcMeaning" };
                string msg = string.Empty;
                //string filepath = ComFunction.generateExcelWithXlt(dt, fields, _webHostEnvironment.ContentRootPath, "FS0309_Export.xlsx", 2, loginInfo.UserId, FunctionID);
                string filepath = ComFunction.DataTableToExcel(head, field, dtContentNew, ".", loginInfo.UserId, FunctionID,ref msg);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0603", ex, loginInfo.UserId);
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
                string vcCodeId = string.Empty;
                string vcCodeName = string.Empty;
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
                vcCodeId = listInfoData[0]["vcCodeId"].ToString();
                vcCodeName = listInfoData[0]["vcCodeName"].ToString();
                //开始数据验证
                if (hasFind)
                {
                    string[,] strField = new string[,] {{"常量代码","常量名称"},
                                                {"vcCodeId","vcCodeName"},
                                                {"",""},
                                                {"50","300"},//最大长度设定,不校验最大长度用0
                                                {"1","1"},//最小长度设定,可以为空用0
                                                {"1","2"}//前台显示列号，从0开始计算,注意有选择框的是0
                    };
                    //需要判断时间区间先后关系的字段
                    string[,] strDateRegion = {  };
                    string[,] strSpecialCheck = { //例子-变更事项字段，当它为新设时，号旧必须为号口，旧型开始、旧型结束、旧型持续开始必须为空
                        
                    };

                    List<Object> checkRes = ListChecker.validateList(listInfoData, strField, strDateRegion, strSpecialCheck, true, "FS0106");
                    if (checkRes != null)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = checkRes;
                        apiResult.flag = Convert.ToInt32(ERROR_FLAG.单元格定位提示);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                //构建DataTable  
                DataTable dt = new DataTable();
                dt.Columns.Add("iAutoId");
                dt.Columns.Add("vcCodeId");
                dt.Columns.Add("vcCodeName");
                dt.Columns.Add("vcValue1");
                dt.Columns.Add("vcValue2");
                dt.Columns.Add("vcValue3");
                dt.Columns.Add("vcValue4");
                dt.Columns.Add("vcValue5");
                dt.Columns.Add("vcValue6");
                dt.Columns.Add("vcValue7");
                dt.Columns.Add("vcValue8");
                dt.Columns.Add("vcValue9");
                dt.Columns.Add("vcValue10");
                dt.Columns.Add("vcModFlag");
                dt.Columns.Add("vcAddFlag");
                dt.Columns.Add("iAPILineNo");

                for (int i = 0; i < listInfoData.Count; i++)
                {
                    List<string> test = new List<string>(listInfoData[i].Keys);//新增中间表数据
                    DataRow dr = dt.NewRow();
                    foreach (string key in test)
                    {
                        for (int j=0;j<dt.Columns.Count;j++)
                        { 
                            if(dt.Columns[j].ToString()==key)
                            {
                                dr[key] = listInfoData[i][key].ToString();
                                break;
                            }
                        }
                    }
                    dt.Rows.Add(dr);
                }
                //验证新维护的
                string[] columnArray = { "vcCodeId", "vcCodeName", "vcAddFlag" };
                DataView dtSelectView = dt.DefaultView;
                DataTable dtSelect = dtSelectView.ToTable(true, columnArray);//去重后的dt
                if (dtSelect.Rows.Count>0)
                {
                    for (int i=0;i<dtSelect.Rows.Count;i++)
                    {
                        if (dtSelect.Rows[i]["vcAddFlag"].ToString()=="True")
                        {
                            string vcCodeIdStr = dtSelect.Rows[i]["vcCodeId"].ToString();
                            string vcCodeNameStr = dtSelect.Rows[i]["vcCodeName"].ToString();
                            //验证新增的 是否存在
                            if (!fs0106_Logic.isExist(vcCodeIdStr, vcCodeNameStr))
                            {
                                apiResult.code = ComConstant.ERROR_CODE;
                                apiResult.data = "新增的常量代码"+vcCodeIdStr+",常量名称"+vcCodeName+"不存在！,无法新增，请联系管理员维护相应的列头数据！";
                                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                            }
                        }
                    }
                }

                string strErrorPartId = "";
                fs0106_Logic.Save(dt,vcCodeId,vcCodeName, loginInfo.UserId, ref strErrorPartId);
                if (strErrorPartId != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "保存失败，" + strErrorPartId;
                    apiResult.flag = Convert.ToInt32(ERROR_FLAG.弹窗提示);
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0604", ex, loginInfo.UserId);
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
                fs0106_Logic.Del(listInfoData, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0605", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

    }
}
