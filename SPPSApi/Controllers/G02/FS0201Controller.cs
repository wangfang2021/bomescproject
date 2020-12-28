using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace SPPSApi.Controllers.G02
{

    [Route("api/FS0201/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0201Controller : BaseController
    {
        FS0201_Logic fs0201_logic = new FS0201_Logic();
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string FunctionID = "FS0201";

        public FS0201Controller(IWebHostEnvironment webHostEnvironment)
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
            string vcSPINo = dataForm.vcSPINo == null ? "" : dataForm.vcSPINo;
            string vcPart_Id = dataForm.vcPart_Id == null ? "" : dataForm.vcPart_Id;
            string vcCarType = dataForm.vcCarType == null ? "" : dataForm.vcCarType;
            string vcState = dataForm.vcState == null ? "" : dataForm.vcState;
            try
            {
                DataTable dt = fs0201_logic.searchApi(vcSPINo, vcPart_Id, vcCarType, vcState);


                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("dHandleTime", ConvertFieldType.DateType, "yyyy-MM-dd");
                dtConverter.addField("vcModFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("vcAddFlag", ConvertFieldType.BoolType, null);
                dtConverter.addField("PartError", ConvertFieldType.BoolType, null);
                dtConverter.addField("BJDiffError", ConvertFieldType.BoolType, null);
                dtConverter.addField("DTDiffError", ConvertFieldType.BoolType, null);
                dtConverter.addField("DTPartError", ConvertFieldType.BoolType, null);
                dtConverter.addField("PartNameError", ConvertFieldType.BoolType, null);
                dtConverter.addField("FXError", ConvertFieldType.BoolType, null);
                dtConverter.addField("FXNoError", ConvertFieldType.BoolType, null);
                dtConverter.addField("ChangeError", ConvertFieldType.BoolType, null);
                dtConverter.addField("NewProjError", ConvertFieldType.BoolType, null);

                List<Object> dataList = ComFunction.convertAllToResultByConverter(dt, dtConverter);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M02UE0201", ex, loginInfo.UserId);
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

            string vcSPINo = dataForm.vcSPINo == null ? "" : dataForm.vcSPINo;
            string vcPart_Id = dataForm.vcPart_Id == null ? "" : dataForm.vcPart_Id;
            string vcCarType = dataForm.vcCarType == null ? "" : dataForm.vcCarType;
            string vcState = dataForm.vcState == null ? "" : dataForm.vcState;
            try
            {
                DataTable dt = fs0201_logic.searchApi(vcSPINo, vcPart_Id, vcCarType, vcState);
                string resMsg = "";
                string[] head = { "SPI NO", "旧品番", "新品番", "補給区分(新)", "代替区分", "代替品番(新)", "品名", "品番実施時期(新/ｶﾗ)", "防錆区分", "防錆指示書№(新)", "変更事項", "旧工程", "工程実施時期旧/ﾏﾃﾞ", "新工程", "工程実施時期新/ｶﾗ", "工程参照引当(直上品番)(新)", "処理日", "シート名", "ファイル名" };
                string[] fields = { "vcSPINo", "vcPart_Id_old", "vcPart_Id_new", "vcBJDiff", "vcDTDiff", "vcPart_id_DT", "vcPartName", "vcStartYearMonth", "vcFXDiff", "vcFXNo", "vcChange", "vcOldProj", "vcOldProjTime", "vcNewProj", "vcNewProjTime", "vcCZYD", "dHandleTime", "vcSheetName", "vcFileName" };

                string filepath = ComFunction.DataTableToExcel(head, fields, dt, _webHostEnvironment.ContentRootPath, loginInfo.UserId, FunctionID, ref resMsg);
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
                //开始数据验证
                if (hasFind)
                {
                    string[,] strField = new string[,] {{"SPI NO","旧品番","新品番","補給区分(新)","代替区分","代替品番(新)","品名","品番実施時期(新/ｶﾗ)","防錆区分","防錆指示書№(新)","変更事項","旧工程","工程実施時期旧/ﾏﾃﾞ","新工程","工程実施時期新/ｶﾗ","工程参照引当(直上品番)(新)","処理日","シート名","ファイル名"},
                                                {"vcSPINo","vcPart_Id_old","vcPart_Id_new","vcBJDiff","vcDTDiff","vcPart_id_DT","vcPartName","vcStartYearMonth","vcFXDiff","vcFXNo","vcChange","vcOldProj","dOldProjTime","vcNewProj","dNewProjTime","vcCZYD","dHandleTime","vcSheetName","vcFileName"},
                                                {"","","","","","","","","","","","","","","","","","",""},
                                                {"0","12","12","0","0","0","0","0","0","0","0","0","7","0","7","0","0","0","0"},//最大长度设定,不校验最大长度用0
                                                {"1","0","0","1","1","0","1","0","1","0","1","0","0","0","0","0","0","0","0"},//最小长度设定,可以为空用0
                                                {"3","4","5","6","7","8","9","10","11","12","13","14","15","16","17","18","19","20","21"}//前台显示列号，从0开始计算,注意有选择框的是0
                    };
                    //需要判断时间区间先后关系的字段
                    string[,] strDateRegion = { };
                    string[,] strSpecialCheck = {
                        //例子-变更事项字段，当它为新设时，号旧必须为号口，旧型开始、旧型结束、旧型持续开始必须为空
                        //vcChange=1时，vcHaoJiu如果为1，如果内容列不为空(H)，则内容必须为H，如果内容为空，则对具体内容不做判断
                        { "旧品番","vcPart_Id_old", "空","", "新品番","vcPart_Id_new","1", "", "" },
                        { "新品番","vcPart_Id_new", "空","", "旧品番","vcPart_Id_old","1", "", "" },
                        { "代替区分","vcDTDiff", "HD","HD", "代替品番","vcPart_id_DT","1", "空", "" },
                        { "防錆区分","vcFXDiff", "R","R", "防錆指示書№(新)","vcFXNo","1", "空", "" },
                    };



                    List<Object> checkRes = ListChecker.validateList(listInfoData, strField, strDateRegion, strSpecialCheck, true, "FS0201");
                    if (checkRes != null)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = checkRes;
                        apiResult.flag = Convert.ToInt32(ERROR_FLAG.单元格定位提示);
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }

                string strErrorPartId = "";
                fs0201_logic.Save(listInfoData, loginInfo.UserId, ref strErrorPartId);

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
                List<Dictionary<string, Object>> listInfoData = checkedInfo.ToObject<List<Dictionary<string, Object>>>();
                if (listInfoData.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选择一条数据！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fs0201_logic.delSPI(listInfoData, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M02UE0100", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除SPI失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 传送

        [HttpPost]
        [EnableCors("any")]
        public string transferApi()
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
                bool flag = fs0201_logic.transferApi(loginInfo.UserId);
                if (flag == true)
                {
                    apiResult.code = ComConstant.SUCCESS_CODE;
                    apiResult.data = "传送成功";
                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "传送失败,含有状态为NG的数据";
                }

                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M02UE0201", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "传送失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }

        #endregion




    }


}