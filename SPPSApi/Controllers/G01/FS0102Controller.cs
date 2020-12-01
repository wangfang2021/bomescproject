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
    [Route("api/FS0102/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0102Controller : BaseController
    {
        FS0102_Logic fs0102_Logic = new FS0102_Logic();
        private readonly string FunctionID = "FS0102";
        
        private readonly IWebHostEnvironment _webHostEnvironment;
        

        public FS0102Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }



        #region 检索所有角色
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
            string roleName = dataForm.roleName;
            roleName = roleName == null ? "" : roleName;
            try
            {
                DataTable dt = fs0102_Logic.Search(roleName);
                List<Object> dataList = ComFunction.convertToResult(dt, new string[] { "vcRoleId", "vcRoleName", "vcOperatorName", "dOperatorTime" });
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M01UE0204", ex, loginInfo.UserId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "检索失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion


        #region 删除角色
        [HttpPost]
        [EnableCors("any")]
        public string deleteApi([FromBody]dynamic data)
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
            string strRoleId = dataForm.vcRoleId;
            string strRoleName = dataForm.vcRoleName;
            try
            {
                DataTable userList = fs0102_Logic.getRoleUserList(strRoleId);
                if (userList != null && userList.Rows.Count > 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "删除终止，选择的" + strRoleName + "角色有" + userList.Rows.Count + "个用户正在使用";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                ArrayList delList = new ArrayList();
                delList.Add(strRoleId);
                this.fs0102_Logic.Delete(delList);
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

        #region 初始化修改子画面
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
            string strRoleId = dataForm.vcRoleId;
            string strRoleName = dataForm.vcRoleName;
            string strType = dataForm.vcType;
            try
            {
                DataTable dt = null;
                if (strType=="create")
                    dt = fs0102_Logic.getRoleFunctionList("");
                else
                    dt = fs0102_Logic.getRoleFunctionList(strRoleId);
                
                List<Object> dataList = ComFunction.convertToResult(dt, new string[] { "vcChildFunID", "vcChildFunName", "vcRead", "vcFunctionType", "vcWrite" });
                for (int i = 0; i < dataList.Count; i++)
                {
                    //vcRead vcWrite字段需要从 0 1转换成false true
                    Dictionary<string, object> row = (Dictionary<string, object>)dataList[i];
                    row["vcRead"] = row["vcRead"].ToString() == "1"?true : false;
                    row["vcWrite"] = row["vcWrite"].ToString() == "1"?true : false;
                }
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = dataList;
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


        #region 修改角色
        [HttpPost]
        [EnableCors("any")]
        public string updateApi([FromBody]dynamic data)
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
                string strRoleId = dataForm.vcRoleId;
                string strRoleName = dataForm.vcRoleName;
                JArray listInfo = dataForm.functionlist;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();

                if (strRoleName.Trim() == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请输入角色名称！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                ArrayList functionList = new ArrayList();
                ArrayList rightList = new ArrayList();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    string strFunctionId = listInfoData[i]["vcChildFunID"].ToString();//机能编号
                    string strFunctionoName = listInfoData[i]["vcChildFunName"].ToString();//机能名称
                    bool isRead = (bool)listInfoData[i]["vcRead"];//是否可读
                    bool isWrite = (bool)listInfoData[i]["vcWrite"];//是否可写
                    if (!isRead && !isWrite)
                        continue;
                    functionList.Add(strFunctionId);
                    string strRight = "";
                    if (isWrite)
                        strRight = "11";
                    else
                        strRight = "10";
                    rightList.Add(strRight);
                }
                if (functionList.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选择一个机能！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fs0102_Logic.Update(strRoleId, strRoleName, functionList, rightList, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
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

        #region 新增角色
        [HttpPost]
        [EnableCors("any")]
        public string createApi([FromBody]dynamic data)
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
                string strRoleId = dataForm.vcRoleId;
                string strRoleName = dataForm.vcRoleName;
                JArray listInfo = dataForm.functionlist;
                List<Dictionary<string, Object>> listInfoData = listInfo.ToObject<List<Dictionary<string, Object>>>();

                if (strRoleName.Trim() == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "请输入角色名称！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                ArrayList functionList = new ArrayList();
                ArrayList rightList = new ArrayList();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    string strFunctionId = listInfoData[i]["vcChildFunID"].ToString();//机能编号
                    string strFunctionoName = listInfoData[i]["vcChildFunName"].ToString();//机能名称
                    bool isRead = (bool)listInfoData[i]["vcRead"];//是否可读
                    bool isWrite = (bool)listInfoData[i]["vcWrite"];//是否可写
                    if (!isRead && !isWrite)
                        continue;
                    functionList.Add(strFunctionId);
                    string strRight = "";
                    if (isWrite)
                        strRight = "11";
                    else
                        strRight = "10";
                    rightList.Add(strRight);
                }
                if (functionList.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "最少选择一个机能！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (fs0102_Logic.hasRoleName(strRoleName))
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "已存在该角色名称，请修改！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                fs0102_Logic.Insert(strRoleName, functionList, rightList, loginInfo.UserId);
                apiResult.code = ComConstant.SUCCESS_CODE;
                apiResult.data = null;
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

    }
}
