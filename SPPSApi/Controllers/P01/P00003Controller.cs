using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Threading;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DataEntity;


namespace SPPSApi.Controllers.P01
{
    [Route("api/P00003/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class P00003Controller : BaseController
    {

        private readonly string FunctionID = "P00003";
        P00003_DataEntity P00003_DataEntity = new P00003_DataEntity();
        P00003_Logic P00003_Logic = new P00003_Logic();
        ComFunction comFunction = new ComFunction();

        #region 获取包装打印机
        [HttpPost]
        [EnableCors("any")]
        public string GetPackPrinter([FromBody] dynamic data)
        {
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            string opearteId = loginInfo.UserId;
            ApiResult apiResult = new ApiResult();
            try
            {
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                string pointtype = dataForm.pointtype == null ? "" : dataForm.pointtype;//设备类型
                string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");
                DataTable dtPrintName = P00003_Logic.checkPrintName(iP, pointtype);
                bool bCheckPrint = false;
                if (pointtype == "COM" || pointtype == "PAD")//平板,一体机
                {
                    if (dtPrintName.Rows.Count != 2)
                        bCheckPrint = true;
                }
                if (bCheckPrint)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "该设备绑定打印机有误，请联系管理员设置。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "P03UE0001", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "验证包装打印机失败!";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 激活验证解锁权限
        [HttpPost]
        [EnableCors("any")]
        public string ValidateUserRoleApi([FromBody] dynamic data)
        {
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            string opearteId = loginInfo.UserId;

            ApiResult apiResult = new ApiResult();
            try
            {
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                string user = dataForm.User == null ? "" : dataForm.User;//入库指令书号
                DataTable getUserRole = P00003_Logic.GetUserRole(user);
                if (getUserRole.Rows.Count == 1)
                {
                    P00003_DataEntity.active = getUserRole.Rows[0][0].ToString();
                    apiResult.data = P00003_DataEntity;
                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "用户在权限表中不存在有效数据!";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "P03UE0002", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "验证解锁权限失败!";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 根据入库指示书判断是否可以包装,返回到前台验证看板
        [HttpPost]
        [EnableCors("any")]
        public string ValidateInvApi([FromBody] dynamic data)
        {
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            string opearteId = loginInfo.UserId;

            ApiResult apiResult = new ApiResult();
            try
            {
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                string inno = dataForm.Inno == null ? "" : dataForm.Inno;//入库指令书号
                string quantity = dataForm.Quantity == null ? "" : dataForm.Quantity;//数量
                string scanTime = dataForm.ScanTime == null ? "" : dataForm.ScanTime;//客户端时间
                DataTable getInputQuantity = P00001_Logic.GetInoutQuantity(inno);//获得入库数量
                DataTable getPackQuantity = P00001_Logic.GetPackQuantity(inno);
                int packSum = 0;
                if (getPackQuantity.Rows.Count > 0)
                {
                    for (int i = 0; i < getPackQuantity.Rows.Count; i++)
                    {
                        packSum += int.Parse(getPackQuantity.Rows[i][0].ToString());

                    }
                }
                if (getInputQuantity.Rows.Count == 1 && (int.Parse(getInputQuantity.Rows[0][4].ToString()) - packSum >= int.Parse(quantity)))
                {
                    //vcPart_id,vcKBOrderNo,vcKBLFNo,vcSR,iQuantity
                    P00003_DataEntity.partId = getInputQuantity.Rows[0][0].ToString();
                    P00003_DataEntity.kanbanOrderNo = getInputQuantity.Rows[0][1].ToString();
                    P00003_DataEntity.kanbanSerial = getInputQuantity.Rows[0][2].ToString();
                    P00003_DataEntity.dock = getInputQuantity.Rows[0][3].ToString();
                    apiResult.data = P00003_DataEntity;
                }
                else if (getInputQuantity.Rows.Count != 1)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "入库单号" + inno + "在作业实绩数据中不存在有效数据!";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "入库单号" + inno + "的数量超过待包装个数!";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "P03UE0003", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "验证入库指令书失败!";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region  包装-登录到包装页面第二步-绑定箱号
        [HttpPost]
        [EnableCors("any")]
        public string GetUserInfoApi([FromBody] dynamic data)
        {
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            string opearteId = loginInfo.UserId;
            string banZhi = loginInfo.BanZhi;
            string userName = loginInfo.UserName;
            ApiResult apiResult = new ApiResult();
            try
            {
                string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");//客户端IP地址
                                                                                                             //vcPointState 1:当前登录中  0：已经退出
                string strPointState = "1";
                string caseNo = "";
                string boxNo = "";
                DataTable getCaseNo = P00003_Logic.getOperCaseNo(iP, strPointState, opearteId);
                if (getCaseNo.Rows.Count > 0)
                {
                    caseNo = getCaseNo.Rows[0]["vcCaseNo"].ToString();//全位
                    boxNo = getCaseNo.Rows[0]["vcBoxNo"].ToString(); //截位
                }
                DataTable dtCaseNoInfo = P00003_Logic.GetCaseNoInfo(caseNo);
                string kanbanQuantity = "0";
                if (dtCaseNoInfo.Rows.Count > 0)
                    kanbanQuantity = dtCaseNoInfo.Rows[0]["kanbanQuantity"].ToString();
                P00003_DataEntity.kanbanQuantity = kanbanQuantity;
                P00003_DataEntity.caseNo = caseNo;
                P00003_DataEntity.boxNo = boxNo;
                P00003_DataEntity.userName = userName;
                P00003_DataEntity.banZhi = banZhi;
                apiResult.data = P00003_DataEntity;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "P03UE0004", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "绑定箱号失败!";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 包装-登录到包装页面第三步-定时更新能率
        [HttpPost]
        [EnableCors("any")]
        public string GetEffiApi([FromBody] dynamic data)
        {
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            string opearteId = loginInfo.UserId;

            ApiResult apiResult = new ApiResult();
            try
            {
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));

                string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");//客户端IP地址
                DataTable getPoint = P00003_Logic.GetPoint(iP);
                DataTable getStanTime = P00003_Logic.GetStanTime();
                if (getPoint.Rows.Count == 1 && getStanTime.Rows.Count == 1)
                {
                    string pointNo = getPoint.Rows[0][0].ToString();
                    decimal[] effi = P00003_Logic.getOperEfficacyInfo("H2", opearteId, pointNo);
                    if (!(effi[0] < 0))
                    {
                        P00003_Logic.UpdateEffi1(pointNo, effi[2]);
                        P00003_DataEntity.totalTime = effi[1].ToString();
                        P00003_DataEntity.effiEncy = (effi[2] * 100).ToString();
                        P00003_DataEntity.stanTime = getStanTime.Rows[0][0].ToString();
                        apiResult.data = P00003_DataEntity;
                    }
                }
            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "P03UE0005", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "能率更新失败!";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 返回到导航页面（包装）
        [HttpPost]
        [EnableCors("any")]
        public string ReturnPage([FromBody] dynamic data)
        {
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            string opearteId = loginInfo.UserId;
            ApiResult apiResult = new ApiResult();
            try
            {
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");

                //检验IP所属点位信息
                DataTable getPoint = P00001_Logic.GetPointNo(iP);
                if (getPoint.Rows.Count != 1)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "当前点位信息异常，请检查！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string pointType = getPoint.Rows[0][0].ToString() + getPoint.Rows[0][1].ToString();
                //更新点位在线履历
                P00001_Logic.setSysExit(iP, "包装-返回导航");
            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "P03UE0006", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "更改状态失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 重新登录（包装）
        [HttpPost]
        [EnableCors("any")]
        public string ExitSystem([FromBody] dynamic data)
        {
            /*
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
              return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            string opearteId = loginInfo.UserId;
            */
            ApiResult apiResult = new ApiResult();
            try
            {
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");

                //检验IP所属点位信息
                DataTable getPoint = P00001_Logic.GetPointNo(iP);
                if (getPoint.Rows.Count != 1)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "当前点位信息异常，请检查！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string pointType = getPoint.Rows[0][0].ToString() + getPoint.Rows[0][1].ToString();
                //更新点位在线履历
                P00001_Logic.setSysExit(iP, "包装-重新登录");
            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "P03UE0007", ex, "sys");
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "更改状态失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region  暂停运行
        [HttpPost]
        [EnableCors("any")]
        public string PauseApi([FromBody] dynamic data)
        {
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            string opearteId = loginInfo.UserId;

            ApiResult apiResult = new ApiResult();
            try
            {
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                string status = dataForm.Status == null ? "" : dataForm.Status;//状态
                string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");//客户端IP地址
                string stopTime = dataForm.StopTime == null ? "" : dataForm.StopTime;//暂停时间
                string serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
                string formatDate = serverTime.Substring(0, 10).Replace("-", "");
                DataTable getStatus2 = P00003_Logic.GetStatus2(iP, opearteId);
                if (getStatus2.Rows.Count == 1)
                {
                    string pointNo = getStatus2.Rows[0][1].ToString();
                    if ((status == "False" || status == "") && getStatus2.Rows[0][0].ToString() == "正常")//将状态修改为暂停
                    {
                        P00003_Logic.UpdateStatus4(pointNo, opearteId);
                    }
                    else if (status == "True" && getStatus2.Rows[0][0].ToString() == "暂停" && stopTime != "")//将状态修改为正常
                    {
                        P00003_Logic.UpdateStatus5(pointNo, opearteId);
                        P00003_Logic.UpdateEffi(formatDate, opearteId, stopTime);
                    }
                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "点位信息异常,请检查!";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "P03UE0008", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "暂停操作失败!";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region  验证时间
        public string GenTimeApi([FromBody] dynamic data)
        {
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            string opearteId = loginInfo.UserId;
            string userName = loginInfo.UserName;
            ApiResult apiResult = new ApiResult();
            try
            {
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                string time = dataForm.Time == null ? "" : dataForm.Time;//品番
                string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");//客户端IP地址
                string serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
                string formatDate = time.Substring(0, 10).Replace("-", "");
                DataTable getTime = P00003_Logic.GetTime(formatDate, opearteId);
                DataTable getCase = P00003_Logic.GetCase(opearteId);
                DataTable getBanZhi = P00003_Logic.GetBanZhi(serverTime);
                if (getCase.Rows.Count == 1)
                {
                    P00003_DataEntity.caseNo = getCase.Rows[0][0].ToString();
                }
                else
                {
                    P00003_DataEntity.caseNo = "";
                }
                string effiEncy = "0";
                if (getTime.Rows.Count == 0 && getBanZhi.Rows.Count == 1)
                {
                    string date = getBanZhi.Rows[0][0].ToString();
                    string banZhi = getBanZhi.Rows[0][1].ToString();
                    P00003_Logic.InsertFre(time, formatDate, effiEncy, opearteId, serverTime, iP, date, banZhi);
                    getTime = P00003_Logic.GetTime(formatDate, opearteId);
                    P00003_Logic.UpdateFre(time, serverTime, formatDate, opearteId);
                    P00003_DataEntity.totalTime = getTime.Rows[0][0].ToString();
                    P00003_DataEntity.freQuency = getTime.Rows[0][1].ToString();
                    P00003_DataEntity.effiEncy = getTime.Rows[0][2].ToString();
                    P00003_DataEntity.startTime = getTime.Rows[0][3].ToString();
                    P00003_DataEntity.packTotalTime = getTime.Rows[0][4].ToString();
                    P00003_DataEntity.userName = userName;
                    apiResult.data = P00003_DataEntity;
                }
                else if (getBanZhi.Rows.Count != 1)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "获取班值失败";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                else if (getTime.Rows.Count == 1)
                {
                    string totalTime = getTime.Rows[0][0].ToString();
                    string startTime = getTime.Rows[0][3].ToString();
                    P00003_Logic.UpdateFre(time, serverTime, formatDate, opearteId);
                    getTime = P00003_Logic.GetTime(formatDate, opearteId);
                    P00003_DataEntity.totalTime = getTime.Rows[0][0].ToString();
                    P00003_DataEntity.startTime = getTime.Rows[0][3].ToString();
                    P00003_DataEntity.freQuency = getTime.Rows[0][1].ToString();
                    P00003_DataEntity.effiEncy = getTime.Rows[0][2].ToString();
                    P00003_DataEntity.packTotalTime = getTime.Rows[0][4].ToString();
                    P00003_DataEntity.userName = userName;
                    apiResult.data = P00003_DataEntity;
                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "当前用户在效率表中存在多条数据,请检查!";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "P03UE0009", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "能率信息生成失败!";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 登出前更新信息
        public string ReloginApi([FromBody] dynamic data)
        {
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            string opearteId = loginInfo.UserId;
            ApiResult apiResult = new ApiResult();
            try
            {
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));

                string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");//客户端IP地址
                string serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
                P00003_Logic.UpdateCase1(opearteId, iP);
            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "P03UE0010", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "更新登出失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion



        #region 获得箱号使用信息 是否出荷,是否绑定信息
        public string GetCaseInfoApi([FromBody] dynamic data)
        {
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            string opearteId = loginInfo.UserId;
            ApiResult apiResult = new ApiResult();
            try
            {
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                string caseNo = dataForm.CaseNo == null ? "" : dataForm.CaseNo;//箱号
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "P03UE0011", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "获取箱号信息失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 登录到主页,将状态设置为已登录,插入状态到履历表中,将之前未做出货的箱号重新绑定，并显示最新的箱号
        //UpdateStatus2Api
        [HttpPost]
        [EnableCors("any")]
        public string UpdateStatus2Api([FromBody] dynamic data)
        {


            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            string opearteId = loginInfo.UserId;
            ApiResult apiResult = new ApiResult();

            try
            {
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");
                string uuid = System.Guid.NewGuid().ToString("N");//生成UUID
                string serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间

                DataTable getBanZhi = P00001_Logic.GetBanZhi(serverTime);
                DataTable getPoint = P00001_Logic.GetPoint2(iP);
                if (getPoint.Rows.Count == 1 && getBanZhi.Rows.Count == 1)
                {
                    string pointNo = getPoint.Rows[0][0].ToString();
                    string date = getBanZhi.Rows[0][0].ToString();
                    string banZhi = getBanZhi.Rows[0][1].ToString();
                    P00001_Logic.InsertDetail(date, banZhi, pointNo, uuid, serverTime, opearteId);
                    DataTable getStatus = P00001_Logic.GetPointStatus4(pointNo);
                    if (getStatus.Rows.Count == 1)
                    {
                        P00001_Logic.UpdateStatus4(pointNo, opearteId);
                        #region 将当前绑定，未打印装箱单的箱号重新绑定
                        DataTable getCase = P00001_Logic.GetCase(opearteId, iP);
                        if (getCase.Rows.Count > 0)
                        {
                            for (int i = 0; i < getCase.Rows.Count; i++)
                            {
                                string caseNo = getCase.Rows[i][0].ToString();
                                DataTable getCase1 = P00001_Logic.GetCase1(caseNo);
                                if (getCase1.Rows.Count == 0)//未打印装箱单
                                {
                                    P00001_Logic.UpdateCase(iP, serverTime, opearteId, caseNo);
                                }
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "点位信息异常,请检查!";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "当前点位表中不存在有效信息,请检查!";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "P03UE0012", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "绑定箱号失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion


        #region  包装-扫描箱号-验证
        public string ValidateCaseNoApi([FromBody] dynamic data)
        {
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            string opearteId = loginInfo.UserId;
            ApiResult apiResult = new ApiResult();
            try
            {
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                string caseNo = dataForm.CaseNo == null ? "" : dataForm.CaseNo;//箱号
                string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");//客户端IP地址
                string serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
                
                #region 记录日志-begin
                string path_begin = @"G:\ScanFile\Log\现场作业\包装_" + System.DateTime.Now.ToString("yyyyMMdd") + "_" + iP + ".txt";
                string log_begin = "作业员:" + opearteId
                    + "；作业时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    + "；作业内容:扫描箱号-验证(begin)"
                    + "；作业对象：" + caseNo + "";
                new P00003_Logic().WriteLog(log_begin, path_begin);
                #endregion

                DataTable dtCaseNoInfo = P00003_Logic.GetCaseNoInfo(caseNo);
                //0.检验IP所属点位信息
                DataTable getPoint = P00001_Logic.GetPointNo(iP);
                if (getPoint.Rows.Count != 1)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "当前点位信息异常，请检查！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string pointType = getPoint.Rows[0][0].ToString() + getPoint.Rows[0][1].ToString();
                //1.验证箱号是否已发行

                if (dtCaseNoInfo.Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "该箱号在补给系统中未进行过发行，请确认后再试。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //2.验证箱号是否已经发行过装箱单
                if (dtCaseNoInfo.Rows[0]["dBoxPrintTime"].ToString() != "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "该箱号已经进行过装箱单发行，请更换箱号后再试。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //3.验证箱号是否在正在非当前用户使用
                if (dtCaseNoInfo.Rows[0]["vcBoxNo"].ToString() != "")
                {
                    //判断使用人是否与当前用户一致
                    if (dtCaseNoInfo.Rows[0]["vcOperatorID"].ToString() != opearteId)
                    {
                        //判断该箱号是否在登录使用状态
                        //vcPointState 1:当前登录中  0：已经退出
                        if (dtCaseNoInfo.Rows[0]["vcPointState"].ToString() == "1")
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "该箱号正在【" + dtCaseNoInfo.Rows[0]["vcPointType"].ToString() + "】使用中，请更换箱号再试。";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                    }
                }
                //4.更新箱号信息表
                string strBoxNo = caseNo.Split('*')[1];
                string strCaseNo = caseNo;
                string strHostIp = iP;
                string strSheBeiNo = pointType;
                string strPointState = "1";
                string strOperatorID = opearteId;
                P00003_Logic.SetCaseNoInfo(strBoxNo, strCaseNo, strHostIp, strSheBeiNo, strPointState, strOperatorID);

                string kanbanQuantity = "0";
                if (dtCaseNoInfo.Rows.Count > 0)
                    kanbanQuantity = dtCaseNoInfo.Rows[0]["kanbanQuantity"].ToString();
                P00003_DataEntity.kanbanQuantity = kanbanQuantity;
                P00003_DataEntity.boxNo = strBoxNo;
                P00003_DataEntity.caseNo = strCaseNo;
                P00003_DataEntity.scanTime = serverTime;
                apiResult.data = P00003_DataEntity;

                #region 记录日志-end
                string path_end = @"G:\ScanFile\Log\现场作业\包装_" + System.DateTime.Now.ToString("yyyyMMdd") + "_" + iP + ".txt";
                string log_end = "作业员:" + opearteId
                    + "；作业时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    + "；作业内容:扫描箱号-验证(end)"
                    + "；作业对象：" + caseNo + "";
                new P00003_Logic().WriteLog(log_end, path_end);
                #endregion

                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "P03UE0013", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "箱号验证失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 包装-扫描看板-验证
        public string ValidateKanbanApi([FromBody] dynamic data)
        {
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            string opearteId = loginInfo.UserId;
            ApiResult apiResult = new ApiResult();
            try
            {
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                string partId = dataForm.PartId == null ? "" : dataForm.PartId;//品番
                string dock = dataForm.Dock == null ? "" : dataForm.Dock;//受入
                string kanbanOrderNo = dataForm.KanbanOrderNo == null ? "" : dataForm.KanbanOrderNo;//看板订单号
                string kanbanSerial = dataForm.KanbanSerial == null ? "" : dataForm.KanbanSerial;//看板连番
                string scanTime = dataForm.ScanTime == null ? "" : dataForm.ScanTime;//客户端时间
                string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");//客户端IP地址
                string serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
                string caseNo = dataForm.CaseNo == null ? "" : dataForm.CaseNo;//当前箱号
                                                                               //0.检验IP所属点位信息
                #region 记录日志-begin
                string path_begin = @"G:\ScanFile\Log\现场作业\包装_" + System.DateTime.Now.ToString("yyyyMMdd") + "_" + iP + ".txt";
                string log_begin = "作业员:" + opearteId
                    + "；作业时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    + "；作业内容:扫描看板-验证(begin)"
                    + "；作业对象：" + kanbanOrderNo + "|" + kanbanSerial + "|" + partId + "|" + dock + "";
                new P00003_Logic().WriteLog(log_begin, path_begin);
                #endregion

                DataTable getPoint = P00001_Logic.GetPointNo(iP);
                if (getPoint.Rows.Count != 1)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "当前点位信息异常，请检查！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string pointType = getPoint.Rows[0][0].ToString() + getPoint.Rows[0][1].ToString();
                string strKind = "LABEL PRINTER";
                string strKindCase = "CASE PRINTER";
                DataTable dtPrintName = P00003_Logic.GetPrintName(iP, strKind);
                DataTable dtCasePrintName = P00003_Logic.GetPrintName(iP, strKindCase);
                string strPrinterName = "";
                string strCasePrinterName = "";
                if (dtPrintName.Rows.Count == 0 || dtCasePrintName.Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "该点位标签打印机未进行设置，请设置后重试。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                strPrinterName = dtPrintName.Rows[0]["vcPrinterName"].ToString();
                strCasePrinterName = dtCasePrintName.Rows[0]["vcPrinterName"].ToString();
                //1.获取看板信息
                DataTable dtKanBanInfo = P00003_Logic.GetKanBanInfo(partId, kanbanOrderNo, kanbanSerial, dock, scanTime);
                //2.检验看板信息
                if (dtKanBanInfo.Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "该看板未进行入荷操作，请入荷后再试。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string strPart_id_InOut = dtKanBanInfo.Rows[0]["vcPart_id_InOut"].ToString();
                if (strPart_id_InOut == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "该看板入出库履历数据异常，请联系管理员确认后再试。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string strSmallPM = dtKanBanInfo.Rows[0]["vcSmallPM"].ToString();
                if (strSmallPM == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "该看板品番品目信息未维护，请维护后再试。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string strCheckP = dtKanBanInfo.Rows[0]["vcCheckP"].ToString();
                if (strCheckP == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "该看板没有检查区分信息，请维护后再试。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string strCheckStatus = dtKanBanInfo.Rows[0]["vcCheckStatus"].ToString();
                if (strCheckP == "抽检" || strCheckP == "全检")
                {
                    if (strCheckStatus == "")
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "该看板未进行检查作业，请检查作业后再试。";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (strCheckStatus == "NG")
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "该看板检查作业结果为NG，请修改后再试。";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                string strInvNo = dtKanBanInfo.Rows[0]["vcInputNo"].ToString();
                string strBZUnit = dtKanBanInfo.Rows[0]["vcBZUnit"].ToString();
                string strPicUrl = dtKanBanInfo.Rows[0]["vcPicUrl"].ToString();
                DataTable dtPackList = P00003_Logic.GetPackList(strInvNo);
                string strQuantity = dtKanBanInfo.Rows[0]["iQuantity"].ToString();
                string strQuantity_bz = dtKanBanInfo.Rows[0]["iQuantity_bz"].ToString();
                string strQuantity_zx = dtKanBanInfo.Rows[0]["iQuantity_zx"].ToString();
                int iQuantity_Fbz = Convert.ToInt32(strQuantity) - Convert.ToInt32(strQuantity_bz);
                int iQuantity_Fzx = Convert.ToInt32(strQuantity) - Convert.ToInt32(strQuantity_zx);
                if (iQuantity_Fzx <= 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "该看板已经全部完成装箱，请确认后再试。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (iQuantity_Fbz < 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "包装数量异常，请联系管理员处理异常。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //3.向前台返回必要数据
                P00003_DataEntity.packParts = dtPackList;//包材list
                P00003_DataEntity.caseQuantity = iQuantity_Fzx.ToString();//待装箱数
                P00003_DataEntity.packQuantity = iQuantity_Fbz.ToString();//待包装数
                P00003_DataEntity.PM = strSmallPM;//品目消息
                P00003_DataEntity.packingQuatity = strBZUnit;//包装单位
                P00003_DataEntity.checkType = strCheckP;
                P00003_DataEntity.partId = partId;
                P00003_DataEntity.kanbanOrderNo = kanbanOrderNo;
                P00003_DataEntity.kanbanSerial = kanbanSerial;
                P00003_DataEntity.kanban = kanbanOrderNo + " " + kanbanSerial;
                P00003_DataEntity.photo = strPicUrl;
                P00003_DataEntity.dock = dock;
                P00003_DataEntity.quantity = strQuantity;
                apiResult.data = P00003_DataEntity;
                //4.打印标签
                P00003_Logic.setPrintLable(iP, strInvNo, strPrinterName, opearteId);
                //5.获取箱号已装箱
                DataTable dtCaseNoInfo = P00003_Logic.GetCaseNoInfo(caseNo);
                string kanbanQuantity = "0";
                if (dtCaseNoInfo.Rows.Count > 0)
                    kanbanQuantity = dtCaseNoInfo.Rows[0]["kanbanQuantity"].ToString();
                P00003_DataEntity.kanbanQuantity = kanbanQuantity;
                P00003_DataEntity.scanTime = serverTime;
                apiResult.data = P00003_DataEntity;

                #region 记录日志-end
                string path_end = @"G:\ScanFile\Log\现场作业\包装_" + System.DateTime.Now.ToString("yyyyMMdd") + "_" + iP + ".txt";
                string log_end = "作业员:" + opearteId
                    + "；作业时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    + "；作业内容:扫描看板-验证(end)"
                    + "；作业对象：" + kanbanOrderNo + "|" + kanbanSerial + "|" + partId + "|" + dock + "";
                new P00003_Logic().WriteLog(log_end, path_end);
                #endregion

                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "P03UE0014", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "验证看板信息失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region  包装-包装装箱-执行
        public string PackWithEnchaseApi([FromBody] dynamic data)
        {
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            string opearteId = loginInfo.UserId;
            ApiResult apiResult = new ApiResult();

            try
            {
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                string partId = dataForm.PartId == null ? "" : dataForm.PartId;//品番
                string quantity = dataForm.Quantity == null ? "" : dataForm.Quantity;//本次要装箱数量
                string dock = dataForm.Dock == null ? "" : dataForm.Dock;//受入
                string kanbanOrderNo = dataForm.KanbanOrderNo == null ? "" : dataForm.KanbanOrderNo;//看板订单号
                string kanbanSerial = dataForm.KanbanSerial == null ? "" : dataForm.KanbanSerial;//看板连番
                string scanTime = dataForm.ScanTime == null ? "" : dataForm.ScanTime;//客户端时间
                if (scanTime == "")
                    scanTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
                string serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
                string timeStart = dataForm.TimeStart == null ? "" : dataForm.TimeStart;
                string timeEnd = dataForm.TimeEnd == null ? "" : dataForm.TimeEnd;
                string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");//客户端IP地址
                                                                                                             //0.检验IP所属点位信息
                DataTable getPoint = P00001_Logic.GetPointNo(iP);
                if (getPoint.Rows.Count != 1)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "当前点位信息异常，请检查！";
                    apiResult.type = "LS";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string pointType = getPoint.Rows[0][0].ToString() + getPoint.Rows[0][1].ToString();

                string checkType = dataForm.CheckType == null ? "" : dataForm.CheckType;//检查区分
                string caseNo = dataForm.CaseNo == null ? "" : dataForm.CaseNo;//箱号
                string formatDate = serverTime.Substring(0, 10).Replace("-", "");
                string packQuantity = dataForm.PackQuantity == null ? "" : dataForm.PackQuantity;//本次要包装数量
                //包装装箱
                //1.插入作业实绩TOperateSJ
                //2.插入装箱实绩TBoxMaster
                //3.更新入出库履历TOperateSJ_InOutput
                //4.插入包材履历TPackWork
                //5.更新包材在库TPackZaiKu
                string strType = "包装装箱";
                string boxno = caseNo;
                if (caseNo.Split('*').Length > 1)
                {
                    boxno = caseNo.Split('*')[1];
                }
                if (packQuantity == "0")
                {
                    strType = "只装箱";
                    packQuantity = quantity;
                }
                #region 记录日志-begin
                string path_begin = @"G:\ScanFile\Log\现场作业\包装_" + System.DateTime.Now.ToString("yyyyMMdd") + "_" + iP + ".txt";
                string log_begin = "作业员:" + opearteId
                    + "；作业时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    + "；作业内容:" + strType + "(begin)"
                    + "；作业对象：" + kanbanOrderNo + "|" + kanbanSerial + "|" + partId + "|" + dock + "";
                new P00003_Logic().WriteLog(log_begin, path_begin);
                #endregion

                DataTable dtPackWork = P00003_Logic.getPackInfo(partId, kanbanOrderNo, kanbanSerial, dock, packQuantity);
                P00003_Logic.setPackAndZxInfo(iP, pointType, strType, partId, kanbanOrderNo, kanbanSerial, dock, packQuantity, caseNo, boxno, scanTime, dtPackWork, opearteId);
                
                DataTable dtCaseNoInfo = P00003_Logic.GetCaseNoInfo(caseNo);
                string kanbanQuantity = "0";
                if (dtCaseNoInfo.Rows.Count > 0)
                    kanbanQuantity = dtCaseNoInfo.Rows[0]["kanbanQuantity"].ToString();
                P00003_DataEntity.kanbanQuantity = kanbanQuantity;
                apiResult.data = P00003_DataEntity;

                #region 记录日志-end
                string path_end = @"G:\ScanFile\Log\现场作业\包装_" + System.DateTime.Now.ToString("yyyyMMdd") + "_" + iP + ".txt";
                string log_end = "作业员:" + opearteId
                    + "；作业时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    + "；作业内容:" + strType + "(end)"
                    + "；作业对象：" + kanbanOrderNo + "|" + kanbanSerial + "|" + partId + "|" + dock + "";
                new P00003_Logic().WriteLog(log_end, path_end);
                #endregion
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "P03UE0015", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "包装装箱失败";
                apiResult.type = "LS";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region  包装-包装不装箱-执行
        public string PackWithoutEnchaseApi([FromBody] dynamic data)
        {
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            string opearteId = loginInfo.UserId;
            ApiResult apiResult = new ApiResult();
            try
            {
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                string partId = dataForm.PartId == null ? "" : dataForm.PartId;//品番
                string quantity = dataForm.Quantity == null ? "" : dataForm.Quantity;//本次要装箱数量
                string dock = dataForm.Dock == null ? "" : dataForm.Dock;//受入
                string kanbanOrderNo = dataForm.KanbanOrderNo == null ? "" : dataForm.KanbanOrderNo;//看板订单号
                string kanbanSerial = dataForm.KanbanSerial == null ? "" : dataForm.KanbanSerial;//看板连番
                string scanTime = dataForm.ScanTime == null ? "" : dataForm.ScanTime;//客户端时间
                if (scanTime == "")
                    scanTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
                string serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
                string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");//客户端IP地址
                //0.检验IP所属点位信息
                DataTable getPoint = P00001_Logic.GetPointNo(iP);
                if (getPoint.Rows.Count != 1)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "当前点位信息异常，请检查！";
                    apiResult.type = "LS";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string pointType = getPoint.Rows[0][0].ToString() + getPoint.Rows[0][1].ToString();

                string checkType = dataForm.CheckType == null ? "" : dataForm.CheckType;//检查区分
                string caseNo = "";//箱号
                string formatDate = serverTime.Substring(0, 10).Replace("-", "");
                string packQuantity = dataForm.PackQuantity == null ? "" : dataForm.PackQuantity;//本次要包装数量
                //string packQuantity = quantity;//本次要包装数量
                //包装不装箱
                //1.插入作业实绩TOperateSJ
                //3.更新入出库履历TOperateSJ_InOutput
                //4.插入包材履历TPackWork
                //5.更新包材在库TPackZaiKu
                if (Convert.ToInt32(packQuantity) < 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "包装数量异常，请联系管理员处理异常。";
                    apiResult.type = "LS";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string strType = "包装不装箱";
                string boxno = caseNo;

                #region 记录日志-begin
                string path_begin = @"G:\ScanFile\Log\现场作业\包装_" + System.DateTime.Now.ToString("yyyyMMdd") + "_" + iP + ".txt";
                string log_begin = "作业员:" + opearteId
                    + "；作业时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    + "；作业内容:" + strType + "(begin)"
                    + "；作业对象：" + kanbanOrderNo + "|" + kanbanSerial + "|" + partId + "|" + dock + "";
                new P00003_Logic().WriteLog(log_begin, path_begin);
                #endregion

                DataTable dtPackWork = P00003_Logic.getPackInfo(partId, kanbanOrderNo, kanbanSerial, dock, packQuantity);
                P00003_Logic.setPackAndZxInfo(iP, pointType, strType, partId, kanbanOrderNo, kanbanSerial, dock, packQuantity, caseNo, boxno, scanTime, dtPackWork, opearteId);
                
                DataTable dtCaseNoInfo = P00003_Logic.GetCaseNoInfo(caseNo);
                string kanbanQuantity = "0";
                if (dtCaseNoInfo.Rows.Count > 0)
                    kanbanQuantity = dtCaseNoInfo.Rows[0]["kanbanQuantity"].ToString();
                P00003_DataEntity.kanbanQuantity = kanbanQuantity;
                apiResult.data = P00003_DataEntity;

                #region 记录日志-end
                string path_end = @"G:\ScanFile\Log\现场作业\包装_" + System.DateTime.Now.ToString("yyyyMMdd") + "_" + iP + ".txt";
                string log_end = "作业员:" + opearteId
                    + "；作业时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    + "；作业内容:" + strType + "(end)"
                    + "；作业对象：" + kanbanOrderNo + "|" + kanbanSerial + "|" + partId + "|" + dock + "";
                new P00003_Logic().WriteLog(log_end, path_end);
                #endregion

                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "P03UE0016", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "包装不装箱失败";
                apiResult.type = "LS";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region  包装-换箱-加载箱号列表
        [HttpPost]
        [EnableCors("any")]
        public string GetCaseListApi([FromBody] dynamic data)
        {
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            string opearteId = loginInfo.UserId;
            ApiResult apiResult = new ApiResult();
            try
            {
                string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");//客户端IP地址
                string strPointState = "1";
                string serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
                //按照IP获取点位使用的箱号getOperCaseNo
                DataTable dtOperCaseNo = P00003_Logic.getOperCaseNo(iP, strPointState, opearteId);
                if (dtOperCaseNo.Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "该点位未获取到有效的箱号信息，请确认后再试。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                P00003_DataEntity.caseNo = string.Empty;
                P00003_DataEntity.caseNo1 = string.Empty;
                P00003_DataEntity.caseNo2 = string.Empty;
                P00003_DataEntity.caseNo3 = string.Empty;
                for (int i = 0; i < dtOperCaseNo.Rows.Count && i <= 3; i++)
                {
                    string strCaseNo = dtOperCaseNo.Rows[i]["vcCaseNo"].ToString();

                    if (i == 0)
                        P00003_DataEntity.caseNo = strCaseNo;

                    if (i == 1)
                        P00003_DataEntity.caseNo1 = strCaseNo;

                    if (i == 2)
                        P00003_DataEntity.caseNo2 = strCaseNo;

                    if (i == 3)
                        P00003_DataEntity.caseNo3 = strCaseNo;

                    apiResult.data = P00003_DataEntity;
                }
                //5.获取箱号已装箱
                DataTable dtCaseNoInfo = P00003_Logic.GetCaseNoInfo(P00003_DataEntity.caseNo);
                string kanbanQuantity = "0";
                if (dtCaseNoInfo.Rows.Count > 0)
                    kanbanQuantity = dtCaseNoInfo.Rows[0]["kanbanQuantity"].ToString();
                P00003_DataEntity.kanbanQuantity = kanbanQuantity;
                P00003_DataEntity.scanTime = serverTime;
                apiResult.data = P00003_DataEntity;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "P03UE0017", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "加载换箱箱号列表失败!";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 包装-换箱-执行
        [HttpPost]
        [EnableCors("any")]
        public string UpdateCaseTimeApi([FromBody] dynamic data)
        {
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            string opearteId = loginInfo.UserId;

            ApiResult apiResult = new ApiResult();
            try
            {
                string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");//客户端IP地址
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                string caseNo = dataForm.Case == null ? "" : dataForm.Case;//箱号
                string serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
                                                                                            //0.检验IP所属点位信息

                #region 记录日志-begin
                string path_begin = @"G:\ScanFile\Log\现场作业\包装_" + System.DateTime.Now.ToString("yyyyMMdd") + "_" + iP + ".txt";
                string log_begin = "作业员:" + opearteId
                    + "；作业时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    + "；作业内容:换箱执行(begin)"
                    + "；作业对象：" + caseNo + "";
                new P00003_Logic().WriteLog(log_begin, path_begin);
                #endregion

                DataTable getPoint = P00001_Logic.GetPointNo(iP);
                if (getPoint.Rows.Count != 1)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "当前点位信息异常，请检查！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string pointType = getPoint.Rows[0][0].ToString() + getPoint.Rows[0][1].ToString();
                //4.更新箱号信息表
                string strBoxNo = caseNo.Split('*')[1];
                string strCaseNo = caseNo;
                string strHostIp = iP;
                string strSheBeiNo = pointType;
                string strPointState = "1";
                string strOperatorID = opearteId;
                P00003_Logic.SetCaseNoInfo(strBoxNo, strCaseNo, strHostIp, strSheBeiNo, strPointState, strOperatorID);
                //5.获取箱号已装箱
                DataTable dtCaseNoInfo = P00003_Logic.GetCaseNoInfo(P00003_DataEntity.caseNo);
                string kanbanQuantity = "0";
                if (dtCaseNoInfo.Rows.Count > 0)
                    kanbanQuantity = dtCaseNoInfo.Rows[0]["kanbanQuantity"].ToString();
                P00003_DataEntity.kanbanQuantity = kanbanQuantity;
                P00003_DataEntity.scanTime = serverTime;
                apiResult.data = P00003_DataEntity;

                #region 记录日志-end
                string path_end = @"G:\ScanFile\Log\现场作业\包装_" + System.DateTime.Now.ToString("yyyyMMdd") + "_" + iP + ".txt";
                string log_end = "作业员:" + opearteId
                    + "；作业时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    + "；作业内容:换箱执行(end)"
                    + "；作业对象：" + caseNo + "";
                new P00003_Logic().WriteLog(log_end, path_end);
                #endregion

                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "P03UE0018", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "执行换箱操作失败!";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 包装-打印装箱单-执行
        public string PrintCaseListApi([FromBody] dynamic data)
        {
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            string opearteId = loginInfo.UserId;
            ApiResult apiResult = new ApiResult();
            try
            {
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                string caseNo = dataForm.CaseNo == null ? "" : dataForm.CaseNo;//箱号
                string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");//客户端IP地址
                string scanTime = dataForm.ScanTime == null ? "" : dataForm.ScanTime;//客户端时间
                if (scanTime == "")
                    scanTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
                string serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
                                                                                            //0.检验IP所属点位信息
                DataTable getPoint = P00001_Logic.GetPointNo(iP);
                if (getPoint.Rows.Count != 1)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "当前点位信息异常，请检查！";
                    apiResult.type = "LS";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string pointType = getPoint.Rows[0][0].ToString() + getPoint.Rows[0][1].ToString();
                #region 记录日志-begin
                string path_begin = @"G:\ScanFile\Log\现场作业\包装_" + System.DateTime.Now.ToString("yyyyMMdd") + "_" + iP + ".txt";
                string log_begin = "作业员:" + opearteId
                    + "；作业时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    + "；作业内容:" + "打印装箱单(begin)"
                    + "；作业对象：" + caseNo + "";
                new P00003_Logic().WriteLog(log_begin, path_begin);
                #endregion
                string strKind = "LABEL PRINTER";
                string strKindCase = "CASE PRINTER";
                DataTable dtPrintName = P00003_Logic.GetPrintName(iP, strKind);
                DataTable dtCasePrintName = P00003_Logic.GetPrintName(iP, strKindCase);
                string strPrinterName = "";
                string strCasePrinterName = "";
                if (dtPrintName.Rows.Count == 0 || dtCasePrintName.Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "该点位标签打印机未进行设置，请设置后重试。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                strPrinterName = dtPrintName.Rows[0]["vcPrinterName"].ToString();
                strCasePrinterName = dtCasePrintName.Rows[0]["vcPrinterName"].ToString();

                //获取箱号相关信息
                DataTable dtBoxMasterInfo = P00003_Logic.getBoxMasterInfo(caseNo, serverTime);
                if (dtBoxMasterInfo.Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "该箱号装箱数据为空，请确认后再试。";
                    apiResult.type = "LS";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //计算两个数量iTotalcnt看板数+iTotalpiece求和数
                int iTotalcnt = dtBoxMasterInfo.Rows.Count;
                int iTotalpiece = 0;
                for (int i = 0; i < dtBoxMasterInfo.Rows.Count; i++)
                {
                    iTotalpiece = iTotalpiece + Convert.ToInt32(dtBoxMasterInfo.Rows[i]["iQuantity_zx"].ToString());
                }
                for (int i = 0; i < dtBoxMasterInfo.Rows.Count; i++)
                {
                    dtBoxMasterInfo.Rows[i]["iTotalcnt"] = iTotalcnt;
                    dtBoxMasterInfo.Rows[i]["iTotalpiece"] = iTotalpiece;
                }
                //检验装箱数
                bool bDZX = false;
                for (int i = 0; i < dtBoxMasterInfo.Rows.Count; i++)
                {
                    if (Convert.ToInt32(dtBoxMasterInfo.Rows[i]["iQuantity_zx"].ToString()) > Convert.ToInt32(dtBoxMasterInfo.Rows[i]["iDZX"].ToString())
                                  || Convert.ToInt32(dtBoxMasterInfo.Rows[i]["iDBZ"].ToString()) < 0)
                    {
                        bDZX = true;
                    }
                }
                if (bDZX)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "装箱数据异常，请联系管理员处理。";
                    apiResult.type = "LS";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                for (int i = 0; i < dtBoxMasterInfo.Rows.Count; i++)
                {
                    if (Convert.ToInt32(dtBoxMasterInfo.Rows[i]["iDBZ"].ToString()) < 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "存在重复包装数据，请查询后删除再处理。";
                        apiResult.type = "LS";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                //获取表结构
                DataSet dsTable = P00003_Logic.getTableFromDB(serverTime);
                //作业实绩表
                DataTable dtOperateSJ_Temp = dsTable.Tables[0].Clone();
                //装箱单表
                DataTable dtCaseList_Temp = dsTable.Tables[1].Clone();
                string cpdName = "一汽丰田";
                string cpdAddress = "天津塘沽开发区第九大街";
                for (int i = 0; i < dtBoxMasterInfo.Rows.Count; i++)
                {
                    #region addrows
                    DataRow drOperateSJ_Temp = dtOperateSJ_Temp.NewRow();
                    drOperateSJ_Temp["vcZYType"] = dtBoxMasterInfo.Rows[i]["vcZYType"].ToString();
                    drOperateSJ_Temp["vcBZPlant"] = dtBoxMasterInfo.Rows[i]["vcBZPlant"].ToString();
                    drOperateSJ_Temp["vcInputNo"] = dtBoxMasterInfo.Rows[i]["vcInputNo"].ToString();
                    drOperateSJ_Temp["vcKBOrderNo"] = dtBoxMasterInfo.Rows[i]["vcKBOrderNo"].ToString();
                    drOperateSJ_Temp["vcKBLFNo"] = dtBoxMasterInfo.Rows[i]["vcKBLFNo"].ToString();
                    drOperateSJ_Temp["vcPart_id"] = dtBoxMasterInfo.Rows[i]["vcPart_id"].ToString();
                    drOperateSJ_Temp["vcIOType"] = dtBoxMasterInfo.Rows[i]["vcIOType"].ToString();
                    drOperateSJ_Temp["vcSupplier_id"] = dtBoxMasterInfo.Rows[i]["vcSupplier_id"].ToString();
                    drOperateSJ_Temp["vcSupplierGQ"] = dtBoxMasterInfo.Rows[i]["vcSupplierGQ"].ToString();
                    drOperateSJ_Temp["dStart"] = scanTime;
                    //drOperateSJ_Temp["dEnd"] = dtBoxMasterInfo.Rows[i]["vcLotid"].ToString();
                    drOperateSJ_Temp["iQuantity"] = dtBoxMasterInfo.Rows[i]["iQuantity_zx"].ToString();
                    drOperateSJ_Temp["vcBZUnit"] = dtBoxMasterInfo.Rows[i]["vcBZUnit"].ToString();
                    drOperateSJ_Temp["vcSHF"] = dtBoxMasterInfo.Rows[i]["vcSHF"].ToString();
                    drOperateSJ_Temp["vcSR"] = dtBoxMasterInfo.Rows[i]["vcSR"].ToString();
                    drOperateSJ_Temp["vcCaseNo"] = dtBoxMasterInfo.Rows[i]["vcCaseNo"].ToString();
                    drOperateSJ_Temp["vcBoxNo"] = dtBoxMasterInfo.Rows[i]["vcBoxNo"].ToString();
                    drOperateSJ_Temp["vcSheBeiNo"] = pointType;
                    drOperateSJ_Temp["vcCheckType"] = dtBoxMasterInfo.Rows[i]["vcCheckType"].ToString();
                    drOperateSJ_Temp["iCheckNum"] = dtBoxMasterInfo.Rows[i]["iCheckNum"].ToString();
                    drOperateSJ_Temp["vcCheckStatus"] = dtBoxMasterInfo.Rows[i]["vcCheckStatus"].ToString();
                    drOperateSJ_Temp["vcLabelStart"] = dtBoxMasterInfo.Rows[i]["vcLabelStart"].ToString();
                    drOperateSJ_Temp["vcLabelEnd"] = dtBoxMasterInfo.Rows[i]["vcLabelEnd"].ToString();
                    //drOperateSJ_Temp["vcUnlocker"] = dtBoxMasterInfo.Rows[i]["vcLotid"].ToString();
                    //drOperateSJ_Temp["dUnlockTime"] = dtBoxMasterInfo.Rows[i]["vcLotid"].ToString();
                    //drOperateSJ_Temp["vcSellNo"] = dtBoxMasterInfo.Rows[i]["vcLotid"].ToString();
                    drOperateSJ_Temp["vcOperatorID"] = opearteId;
                    //drOperateSJ_Temp["dOperatorTime"] = dtBoxMasterInfo.Rows[i]["vcLotid"].ToString();
                    drOperateSJ_Temp["vcHostIp"] = iP;
                    drOperateSJ_Temp["packingcondition"] = dtBoxMasterInfo.Rows[i]["packingcondition"].ToString();
                    drOperateSJ_Temp["vcPackingPlant"] = dtBoxMasterInfo.Rows[i]["vcPackingPlant"].ToString();
                    dtOperateSJ_Temp.Rows.Add(drOperateSJ_Temp);
                    #endregion

                    #region addrows
                    DataRow drCaseList_Temp = dtCaseList_Temp.NewRow();
                    drCaseList_Temp["vcCpdcode"] = dtBoxMasterInfo.Rows[i]["vcSHF"].ToString();
                    drCaseList_Temp["vcCpdname"] = cpdName;
                    drCaseList_Temp["vcCpdaddress"] = cpdAddress;
                    //drCaseList_Temp["vcCasenoIntact"] = dtBoxMasterInfo.Rows[i]["vcZYType"].ToString();
                    drCaseList_Temp["vcCaseno"] = dtBoxMasterInfo.Rows[i]["vcCaseNo"].ToString();
                    drCaseList_Temp["vcCasebarcode"] = dtBoxMasterInfo.Rows[i]["vcBoxNo"].ToString();
                    //drCaseList_Temp["iNo"] = dtBoxMasterInfo.Rows[i]["vcZYType"].ToString();
                    drCaseList_Temp["vcInno"] = dtBoxMasterInfo.Rows[i]["vcInputNo"].ToString();
                    drCaseList_Temp["vcPart_id"] = dtBoxMasterInfo.Rows[i]["vcPart_id"].ToString();
                    drCaseList_Temp["vcPartsname"] = dtBoxMasterInfo.Rows[i]["vcPartENName"].ToString();
                    drCaseList_Temp["iQty"] = dtBoxMasterInfo.Rows[i]["iQuantity_zx"].ToString();
                    drCaseList_Temp["iTotalcnt"] = dtBoxMasterInfo.Rows[i]["iTotalcnt"].ToString();
                    drCaseList_Temp["iTotalpiece"] = dtBoxMasterInfo.Rows[i]["iTotalpiece"].ToString();
                    drCaseList_Temp["vcPcname"] = pointType;
                    drCaseList_Temp["vcHostip"] = iP;
                    byte[] Qrcode = P00003_Logic.GenerateQRCode(dtBoxMasterInfo.Rows[i]["vcCaseNo"].ToString());//全号
                    drCaseList_Temp["iDatamatrixcode"] = Qrcode;
                    drCaseList_Temp["vcOperatorID"] = opearteId;
                    //drCaseList_Temp["dOperatorTime"] = dtBoxMasterInfo.Rows[i]["vcZYType"].ToString();
                    //drCaseList_Temp["dFirstPrintTime"] = dtBoxMasterInfo.Rows[i]["vcZYType"].ToString();
                    //drCaseList_Temp["dLatelyPrintTime"] = dtBoxMasterInfo.Rows[i]["vcZYType"].ToString();
                    drCaseList_Temp["vcLabelStart"] = dtBoxMasterInfo.Rows[i]["vcLabelStart"].ToString();
                    drCaseList_Temp["vcLabelEnd"] = dtBoxMasterInfo.Rows[i]["vcLabelEnd"].ToString();
                    dtCaseList_Temp.Rows.Add(drCaseList_Temp);
                    #endregion
                }
                //插入作业实绩表-装箱数据
                //更新入出库履历-待装箱+待出荷
                //插入装箱单表
                //更新箱号使用情况表
                //插入装箱单打印表
                string strBoxNo = caseNo.Split('*')[1];
                string strCaseNo = caseNo;
                P00003_Logic.setCastListInfo(dtOperateSJ_Temp, dtCaseList_Temp, iP, caseNo, strBoxNo, scanTime, opearteId, strCasePrinterName);
                #region 记录日志-end
                string path_end = @"G:\ScanFile\Log\现场作业\包装_" + System.DateTime.Now.ToString("yyyyMMdd") + "_" + iP + ".txt";
                string log_end = "作业员:" + opearteId
                    + "；作业时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    + "；作业内容:" + "打印装箱单(end)"
                    + "；作业对象：" + caseNo + "";
                new P00003_Logic().WriteLog(log_begin, path_begin);
                #endregion
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "P03UE0019", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "打印装箱单失败";
                apiResult.type = "LS";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region  获得包装数据
        public string GetCheckDataApi([FromBody] dynamic data)
        {
            string strToken = Request.Headers["X-Token"];
            if (!isLogin(strToken))
            {
                return error_login();
            }
            LoginInfo loginInfo = getLoginByToken(strToken);
            string opearteId = loginInfo.UserId;

            ApiResult apiResult = new ApiResult();
            try
            {
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                string partId = dataForm.PartId == null ? "" : dataForm.PartId;//品番
                string quantity = dataForm.Quantity == null ? "" : dataForm.Quantity;//数量
                string dock = dataForm.Dock == null ? "" : dataForm.Dock;//受入
                string kanbanOrderNo = dataForm.KanbanOrderNo == null ? "" : dataForm.KanbanOrderNo;//看板订单号
                string kanbanSerial = dataForm.KanbanSerial == null ? "" : dataForm.KanbanSerial;//看板连番
                string scanTime = dataForm.ScanTime == null ? "" : dataForm.ScanTime;//客户端时间
                if (scanTime == "")
                    scanTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
                DataTable getPackData = P00003_Logic.GetPackData(partId, scanTime);//从包材构成数据获得包材区分
                DataTable getPM = P00003_Logic.GetPM(dock, partId);

                DataTable getData = P00003_Logic.GetData(partId, dock, kanbanOrderNo, kanbanSerial);
                if (getPackData.Rows.Count > 0 && getPM.Rows.Count == 1 && getData.Rows.Count == 1)
                {

                    #region 构造返回数据
                    P00003_DataEntity.NZ = getPackData.Rows[0][1].ToString();
                    P00003_DataEntity.GZ = getPackData.Rows[1][1].ToString();
                    int sum = 0;
                    for (int i = 2; i < getPackData.Rows.Count; i++)
                    {

                        String j = getPackData.Rows[i][1].ToString();
                        sum += int.Parse(j);

                    }
                    P00003_DataEntity.other = sum.ToString();
                    P00003_DataEntity.PM = getPM.Rows[0][0].ToString();
                    P00003_DataEntity.DW = getData.Rows[0][0].ToString();
                    P00003_DataEntity.checkType = getData.Rows[0][1].ToString();
                    P00003_DataEntity.quantity = quantity.Replace("0", "");


                    apiResult.data = P00003_DataEntity;


                    #endregion
                }
                else if (getPackData.Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "品番" + partId + "在包材基础数据中没有对应数据,请检查!";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }
                else if (getPM.Rows.Count != 1)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "品番" + partId + "在品目信息表中没有有效或多条数据,请检查!";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }
                else if (getData.Rows.Count != 1)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "品番" + partId + "在作业实绩表中没有对应数据或有多条数据,请检查!";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "P03UE0020", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "获取包装数据失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion
    }

}
