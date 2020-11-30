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
            string strUserId = ComFunction.Decrypt(strToken);
            //以下开始业务处理
            ApiResult apiResult = new ApiResult();
            dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
            string vcUnitID = dataForm.vcUnitID;
            string vcUserID = dataForm.vcUserID;
            string vcUserName = dataForm.vcUserName;

            vcUnitID = vcUnitID == null ? "" : vcUnitID;
            vcUserID = vcUserID == null ? "" : vcUserID;
            vcUserName = vcUserName == null ? "" : vcUserName;

            try
            {
                DataTable dt = fs0101_Logic.Search(vcUnitID, vcUserID, vcUserName);
                List<Object> dataList = ComFunction.convertToResult(dt, new string[] { "vcUserID", "vcUserName", "vcUnitName", "vcPlantCode", "vcRoleName", "vcStop", "vcSpecial" });
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0104", ex, strUserId);
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
            string strUserId = ComFunction.Decrypt(strToken);
            //以下开始业务处理
            ApiResult apiResult = new ApiResult();
            dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
            string vcUserID = dataForm.vcUserID;
            string strType = dataForm.vcType;
            try
            {
                DataTable dt_AllRole = fs0101_Logic.getAllRole();//获取所有可选择的角色
                DataTable dt_MyRole = fs0101_Logic.getRoleList(vcUserID);//获取所有可选择的角色
                DataTable dt_Special = fs0101_Logic.getAllSpecial();//获取所有特别角色

                List<Object> list_AllRole = ComFunction.convertToResult(dt_AllRole, new string[] { "key", "label" });//返回所有角色
                List<Object> list_MyRole = ComFunction.convertToResult(dt_MyRole, "vcRoleID");//返回用户所拥有的角色
                List<Object> list_AllSpecialRole = ComFunction.convertToResult(dt_Special, new string[] { "label", "value" });//返回所有特别角色

                Dictionary<string, object> res = new Dictionary<string, object>();
                res.Add("allRole", list_AllRole);
                res.Add("myRole", list_MyRole);
                res.Add("allSpecialRole", list_AllSpecialRole);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = res;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M00UE0006", ex, strUserId);
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
            string strUserId = ComFunction.Decrypt(strToken);
            //以下开始业务处理
            ApiResult apiResult = new ApiResult();
            try
            {
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                string vcUnitID = dataForm.vcUnitID;
                string vcUserID = dataForm.vcUserID;
                string vcUserName = dataForm.vcUserName;
                string vcPassWord = dataForm.vcPassWord;
                //JArray listInfo = dataForm.functionlist;
                //List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();

                if (vcUserID.Trim() == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请输入用户编号！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                fs0101_Logic.Insert(vcUserID,vcUserName,vcPassWord,"1",vcUnitID,null,null,null,"0",null,"");
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0201", ex, strUserId);
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
            string strUserId = ComFunction.Decrypt(strToken);
            //以下开始业务处理
            ApiResult apiResult = new ApiResult();
            try
            {
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                string vcUnitID = dataForm.vcUnitID;
                string vcUserID = dataForm.vcUserID;
                string vcUserName = dataForm.vcUserName;
                string vcPassWord = dataForm.vcPassWord;

                fs0101_Logic.Update(vcUserID, vcUserName, vcPassWord, "1", vcUnitID, null, null,null,null,"0",null);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0203", ex, strUserId);
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
            string strUserId = ComFunction.Decrypt(strToken);
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0202", ex, strUserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion


    }
}
