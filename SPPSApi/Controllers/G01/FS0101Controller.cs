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
    [Route("api/FS0101/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0101Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        FS0101_Logic fs0101_Logic = new FS0101_Logic();
        private readonly string FunctionID = "FS0101";

        public FS0101Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        #region 检索所有用户
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

            string vcUnitID = loginInfo.UnitCode;
            string vcUserID = dataForm.vcUserID;
            string vcUserName = dataForm.vcUserName;

            vcUnitID = vcUnitID == null ? "" : vcUnitID;
            vcUserID = vcUserID == null ? "" : vcUserID;
            vcUserName = vcUserName == null ? "" : vcUserName;

            try
            {
                DataTable dt = fs0101_Logic.Search(vcUnitID, vcUserID, vcUserName);
                List<Object> dataList = ComFunction.convertToResult(dt, new string[] { "vcUserID", "vcUserName", "vcUnitName", "vcPlantCode", "vcRoleName", "vcStop", "vcSpecial","vcBanZhi", "vcBaoZhuangPlace","vcBanZhi_Name", "vcBaoZhuangPlace_Name", "vcEmail" });
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0104", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 初始化修改子画面
        [HttpPost]
        [EnableCors("any")]
        public string initSubApi([FromBody] dynamic data)
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
            string vcUserID = dataForm.vcUserID;
            JArray vcPlants = dataForm.vcPlants;
            List<string> plantList = vcPlants.ToObject<List<string>>();

            try
            {
                DataTable dt_AllRole = fs0101_Logic.getAllRole();//获取所有可选择的角色
                DataTable dt_MyRole = fs0101_Logic.getRoleList(vcUserID);//获取所有可选择的角色
                DataTable dt_Special = fs0101_Logic.getAllSpecial();//获取所有特别角色
                DataTable dt_PlantAll = fs0101_Logic.getPlantList(loginInfo.UnitCode);//获取工厂列表

                List<Object> list_AllRole = ComFunction.convertToResult(dt_AllRole, new string[] { "key", "label" });//返回所有角色
                List<Object> list_MyRole = ComFunction.convertToResult(dt_MyRole, "vcRoleID");//返回用户所拥有的角色
                List<Object> list_AllSpecialRole = ComFunction.convertToResult(dt_Special, new string[] { "label", "value" });//返回所有特别角色
                List<Object> list_AllPlant = ComFunction.convertToResult(dt_PlantAll, new string[] { "vcPlantCode", "vcPlantName" });//返回所有工厂代码

                List<Object> dataList_C001 = ComFunction.convertAllToResult(ComFunction.getTCode("C001"));//包装场
                List<Object> dataList_C010 = ComFunction.convertAllToResult(ComFunction.getTCode("C010"));//班值
 

                Dictionary<string, object> res = new Dictionary<string, object>();
                res.Add("allRole", list_AllRole);
                res.Add("myRole", list_MyRole);
                res.Add("allSpecialRole", list_AllSpecialRole);
                res.Add("plantAll", list_AllPlant);
                res.Add("plantMyList", plantList);
                res.Add("unitCode", loginInfo.UnitCode);
                //2021-2-7追加 班值 包装场
                res.Add("baoZhuangPlaceOptions", dataList_C001);
                res.Add("banzhiOptions", dataList_C010);

                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M00UE0006", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "初始化修改子画面失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 新增用户
        [HttpPost]
        [EnableCors("any")]
        public string createApi([FromBody] dynamic data)
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
                string vcUnitID = dataForm.vcUnitID;
                string vcUserID = dataForm.vcUserID;
                string vcUserName = dataForm.vcUserName;
                string vcPassWord = dataForm.vcPassWord;
                JArray vcPlants = dataForm.vcPlants;
                List<string> plantList = vcPlants.ToObject<List<string>>();
                string vcEmail = dataForm.vcEmail;
                string vcStop = dataForm.vcStop == "true" ? "1" : "0";
                string vcSpecial = dataForm.vcSpecial;
                JArray listInfo = dataForm.ddlRoleValue;

                string vcBanZhi = dataForm.vcBanZhi;
                string vcBaoZhuangPlace = dataForm.vcBaoZhuangPlace;

                List<string> roleList = listInfo.ToObject<List<string>>();

                if (vcUnitID.Trim() == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "事业体代码不能为空！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (vcUserID.Trim() == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请输入用户编号！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (vcUserName.Trim() == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请输入用户名称！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (vcPassWord.Trim() == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请输入用户密码！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (roleList.Count== 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选择一个角色！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (fs0101_Logic.hasUserId(vcUserID))
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "已存在该用户代码，请修改！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                fs0101_Logic.Insert(vcUserID, vcUserName, ComFunction.encodePwd(vcPassWord)
                         , fs0101_Logic.getStrByList(plantList), vcUnitID, roleList, loginInfo.UserId, vcEmail, vcStop, vcSpecial, vcBanZhi, vcBaoZhuangPlace, loginInfo.PlatForm);

                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UI0103", null, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0201", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "新增失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 修改用户
        [HttpPost]
        [EnableCors("any")]
        public string updateApi([FromBody] dynamic data)
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
                string vcUnitID = dataForm.vcUnitID;
                string vcUserID = dataForm.vcUserID;
                string vcUserName = dataForm.vcUserName;
                string vcPassWord = dataForm.vcPassWord;
                JArray vcPlants = dataForm.vcPlants;
                List<string> plantList = vcPlants.ToObject<List<string>>();

                string vcEmail = dataForm.vcEmail;
                string vcStop = dataForm.vcStop=="true"?"1":"0";
                string vcSpecial = dataForm.vcSpecial;

                string vcBanZhi = dataForm.vcBanZhi;
                string vcBaoZhuangPlace = dataForm.vcBaoZhuangPlace;

                JArray listInfo = dataForm.ddlRoleValue;
                List<string> roleList = listInfo.ToObject<List<string>>();

                if (vcUnitID.Trim() == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "事业体代码不能为空！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (vcUserID.Trim() == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请输入用户编号！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (vcUserName.Trim() == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请输入用户名称！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (roleList.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选择一个角色！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                 

                fs0101_Logic.Update(vcUserID, vcUserName, ComFunction.encodePwd(vcPassWord)
                        , fs0101_Logic.getStrByList(plantList), vcUnitID, roleList, loginInfo.UserId, vcEmail, vcStop, vcSpecial, vcBanZhi, vcBaoZhuangPlace, loginInfo.PlatForm);
                ComMessage.GetInstance().ProcessMessage(FunctionID,  "M01UI0104", null, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0203", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "修改失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 删除用户
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
            dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
            string vcUserID = dataForm.vcUserID;
            try
            {
                ArrayList delList = new ArrayList();
                delList.Add(vcUserID);
                this.fs0101_Logic.Delete(delList);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = "删除成功";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0202", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion


    }
}
