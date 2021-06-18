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
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;
using DataEntity;
using QRCoder;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

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




            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "验证解锁权限失败!";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }



            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
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




                if (getInputQuantity.Rows.Count == 1 && (int.Parse(getInputQuantity.Rows[0][4].ToString()) - packSum >= int.Parse(quantity))

                  )
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






            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "获取入库信息失败!";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }



            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        }
        #endregion

        #region 定时更新能率
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
                        int effiResult = P00003_Logic.UpdateEffi1(pointNo, effi[2]);
                        P00003_DataEntity.totalTime = effi[1].ToString();
                        P00003_DataEntity.effiEncy = (effi[2] * 100).ToString();
                        P00003_DataEntity.stanTime = getStanTime.Rows[0][0].ToString();
                        apiResult.data = P00003_DataEntity;
                    }

                }



            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "更改状态失败!";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }



            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
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
                        int statusResultUp = P00003_Logic.UpdateStatus4(pointNo, opearteId);





                    }
                    else if (status == "True" && getStatus2.Rows[0][0].ToString() == "暂停" && stopTime != "")//将状态修改为正常
                    {

                        int statusResultUp = P00003_Logic.UpdateStatus5(pointNo, opearteId);
                        int effiResultUp = P00003_Logic.UpdateEffi(formatDate, opearteId, stopTime);
                    }

                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "点位信息异常,请检查!";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);


                }

            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "更改状态失败!";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }



            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        }
        #endregion

        #region 更新箱号时间
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
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                string caseNo = dataForm.Case == null ? "" : dataForm.Case;//品番
                string serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
                int caseResultUp = P00003_Logic.UpdateCase2(caseNo, serverTime);


            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "获取用户信息失败!";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }



            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        }
        #endregion

        #region  获取箱号列表
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
                DataTable getCaseList = P00003_Logic.GetCaseList(opearteId);
                if (getCaseList.Rows.Count > 0 || getCaseList.Rows.Count > 4)
                {
                    if (getCaseList.Rows.Count == 1)
                    {
                        P00003_DataEntity.caseNo = getCaseList.Rows[0][0].ToString();
                        P00003_DataEntity.caseNo1 = "";
                        P00003_DataEntity.caseNo2 = "";
                        P00003_DataEntity.caseNo3 = "";
                        apiResult.data = P00003_DataEntity;


                    }
                    else if (getCaseList.Rows.Count == 2)
                    {

                        P00003_DataEntity.caseNo = getCaseList.Rows[0][0].ToString();
                        P00003_DataEntity.caseNo1 = getCaseList.Rows[1][0].ToString();
                        P00003_DataEntity.caseNo2 = "";
                        P00003_DataEntity.caseNo3 = "";
                        apiResult.data = P00003_DataEntity;
                    }
                    else if (getCaseList.Rows.Count == 3)
                    {

                        P00003_DataEntity.caseNo = getCaseList.Rows[0][0].ToString();
                        P00003_DataEntity.caseNo1 = getCaseList.Rows[1][0].ToString();
                        P00003_DataEntity.caseNo2 = getCaseList.Rows[2][0].ToString();
                        P00003_DataEntity.caseNo3 = "";
                        apiResult.data = P00003_DataEntity;
                    }
                    else if (getCaseList.Rows.Count == 4)
                    {

                        P00003_DataEntity.caseNo = getCaseList.Rows[0][0].ToString();
                        P00003_DataEntity.caseNo1 = getCaseList.Rows[1][0].ToString();
                        P00003_DataEntity.caseNo2 = getCaseList.Rows[2][0].ToString();
                        P00003_DataEntity.caseNo3 = getCaseList.Rows[3][0].ToString();
                        apiResult.data = P00003_DataEntity;
                    }





                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "当前箱号列表中没有有效的箱号!";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }



            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "获取用户信息失败!";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }



            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        }
        #endregion

        #region  获得用户信息,获取箱号
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
                DataTable getCaseNo = P00003_Logic.GetCaseNo(iP);

                if (getCaseNo.Rows.Count > 0)
                {
                    string caseNo = getCaseNo.Rows[0][0].ToString(); ;
                    DataTable validateCaseNo3 = P00003_Logic.ValidateCaseNo3(caseNo);
                    P00003_DataEntity.caseNo = caseNo;
                    P00003_DataEntity.kanbanQuantity = validateCaseNo3.Rows.Count.ToString();

                }
                else
                {


                    P00003_DataEntity.caseNo = "";

                }



                P00003_DataEntity.userName = userName;
                P00003_DataEntity.banZhi = banZhi;
                apiResult.data = P00003_DataEntity;



            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "获取用户信息失败!";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }



            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        }
        #endregion

        #region 销毁页面或退出系统时候更新总时间,重新登录需要将当前箱号状态设置为不可用,需要更改机器状态为未登录
        public string UpdateTimeApi([FromBody] dynamic data)
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
                string time = dataForm.Time == null ? "" : dataForm.Time;//品番
                string caseNo = dataForm.CaseNo == null ? "" : dataForm.CaseNo;//箱号
                string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");//客户端IP地址
                string serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
                string formatDate = time.Substring(0, 10).Replace("-", "");
                DataTable getTime = P00003_Logic.GetTime(formatDate, opearteId);
                DataTable getCase1 = P00003_Logic.GetCase1(caseNo);
                DataTable getStatus1 = P00003_Logic.GetStatus1(iP, opearteId);



                if (getTime.Rows.Count == 1 && getStatus1.Rows.Count == 1)
                {
                    #region 退出登录更新箱号状态
                    int caseResultUp = P00003_Logic.UpdateCase3(caseNo);




                    #endregion

                    #region 退出登录需要更改状态为未登录
                    string pointNo = getStatus1.Rows[0][0].ToString();
                    int statusResultUp = P00003_Logic.UpdateStatus3(pointNo, opearteId);






                    #endregion




                    //返回时间格式 2021/3/20 17:06:38
                    string startTime = getTime.Rows[0][3].ToString();
                    string totalTime1 = getTime.Rows[0][0].ToString();



                    string getYearData = startTime.Split(" ")[0];
                    string getTimeData = startTime.Split(" ")[1];

                    int beforeYear = int.Parse(getYearData.Split("/")[0]);
                    int beforeMonth = int.Parse(getYearData.Split("/")[1]);
                    int beforeDate = int.Parse(getYearData.Split("/")[2]);
                    int beforeHours = int.Parse(getTimeData.Split(":")[0]);
                    int beforeMinutes = int.Parse(getTimeData.Split(":")[1]);
                    int beforeSeconds = int.Parse(getTimeData.Split(":")[2]);


                    string getYearData1 = time.Split(" ")[0];
                    string getTimeData1 = time.Split(" ")[1];

                    int afterYear = int.Parse(getYearData1.Split("-")[0]);
                    int afterMonth = int.Parse(getYearData1.Split("-")[1]);
                    int afterDate = int.Parse(getYearData1.Split("-")[2]);

                    int afterHours = int.Parse(getTimeData1.Split(":")[0]);
                    int afterMinutes = int.Parse(getTimeData1.Split(":")[1]);
                    int afterSeconds = int.Parse(getTimeData1.Split(":")[2]);

                    int totalTime = (afterYear - beforeYear) * 365 * 24 * 60 * 60 +
                      (afterMonth - beforeMonth) * 30 * 24 * 60 * 60 +
                      (afterDate - beforeDate) * 24 * 60 * 60 +
                      (afterHours - beforeHours) * 60 * 60 + (afterMinutes - beforeMinutes) * 60 + (afterSeconds - beforeSeconds) + int.Parse(totalTime1);

                    if (totalTime >= 0)
                    {
                        int freResultUp = P00003_Logic.UpdateFre1(totalTime, opearteId, formatDate);







                    }
                    else if (getStatus1.Rows.Count != 1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "点位信息异常,请检查!";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                    }
                    else
                    {

                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "结束时间小于开始时间,请检查!";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);


                    }







                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "当前用户在效率表中不存在有效信息,请检查!";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);




                }

            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "更新时间失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }






            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
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
                    int freResultIn = P00003_Logic.InsertFre(time, formatDate, effiEncy, opearteId, serverTime, iP, date, banZhi);
                    getTime = P00003_Logic.GetTime(formatDate, opearteId);

                    int freResultUp = P00003_Logic.UpdateFre(time, serverTime, formatDate, opearteId);






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


                    int freResultUp = P00003_Logic.UpdateFre(time, serverTime, formatDate, opearteId);

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





            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "验证时间失败!";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }






            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        }
        #endregion

        #region 包装-扫描看板
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

                #region 验证看板信息
                //1
                DataTable validateSJ = P00003_Logic.ValidateSJ(partId, dock, kanbanOrderNo, kanbanSerial);//验证包装数量
                //2
                DataTable validateSJ1 = P00003_Logic.ValidateSJ2(partId, dock, kanbanOrderNo, kanbanSerial);//验证装箱数量
                //3
                DataTable validateInv1 = P00003_Logic.ValidateInv1(partId, kanbanOrderNo, kanbanSerial);
                //4
                DataTable getInno = P00003_Logic.GetQuantity(kanbanOrderNo, kanbanSerial, partId, dock);
                //5
                DataTable getInputQuantity = P00003_Logic.GetInputQuantity(kanbanOrderNo, kanbanSerial, partId, dock);
                //6
                DataTable validateData = P00003_Logic.ValidateData(partId, scanTime);
                //7
                DataTable validateCase = P00003_Logic.ValidateCase(partId, kanbanOrderNo, kanbanSerial, dock, caseNo);
                if (validateCase.Rows.Count > 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "箱号" + caseNo + "中已经存在品番" + partId + "数据，请更换箱号进行装箱";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //发生入荷+入荷数量+品番信息完整
                if (getInno.Rows.Count == 1 && getInputQuantity.Rows.Count == 1 && validateData.Rows.Count == 1)
                {
                    string inputNo = getInno.Rows[0][1].ToString();
                    string quantity = getInputQuantity.Rows[0][0].ToString();
                    //未发生过包装+未发生过装箱+入出履历数据完整
                    if (validateSJ.Rows.Count == 0 && validateSJ1.Rows.Count == 0 && validateInv1.Rows.Count == 1)//没有进行检查或包装看板,需要判断前工程
                    {
                        string supplier_id = validateData.Rows[0][1].ToString();
                        //获取检查区分
                        DataTable getCheckType = P00003_Logic.GetCheckType(partId, scanTime, supplier_id);//获得检查区分
                        if (getCheckType.Rows.Count == 1)
                        {
                            string checkType = getCheckType.Rows[0][0].ToString();
                            if (checkType == "免检")//免检品验证入库信息返回数据
                            {
                                //获取入荷信息
                                DataTable validateSJ2 = P00003_Logic.ValidateOpr1(partId, quantity, dock, kanbanOrderNo, kanbanSerial);//02为包装单位
                                //获取品目信息
                                DataTable getPM = P00003_Logic.GetPM(dock, partId);//00为品目
                                //获取品番收容数
                                DataTable validateData1 = P00003_Logic.ValidateData1(partId, scanTime);//00为收容数

                                if (validateSJ2.Rows.Count == 1 && getPM.Rows.Count == 1 && validateData1.Rows.Count == 1)
                                {
                                    string packingQuatity = validateSJ2.Rows[0][2].ToString();//包装单位
                                    string pinmu = getPM.Rows[0][0].ToString();//品目
                                    string packQuantity = validateData1.Rows[0][0].ToString();//收容数
                                    //获取使用的包材信息
                                    DataTable getPackInfo = P00003_Logic.GetPackInfo(partId, scanTime, packingQuatity, quantity);
                                    if (getPackInfo.Rows.Count > 0)
                                    {
                                        P00003_DataEntity.packParts = getPackInfo;//包材list
                                        P00003_DataEntity.caseQuantity = quantity;//装箱数
                                        P00003_DataEntity.packQuantity = quantity;//包装数
                                        P00003_DataEntity.PM = pinmu;//品目消息
                                        P00003_DataEntity.packingQuatity = packingQuatity;//包装单位
                                        P00003_DataEntity.checkType = checkType;
                                        P00003_DataEntity.partId = partId;
                                        P00003_DataEntity.kanbanOrderNo = kanbanOrderNo;
                                        P00003_DataEntity.kanbanSerial = kanbanSerial;
                                        P00003_DataEntity.kanban = kanbanOrderNo + " " + kanbanSerial;
                                        P00003_DataEntity.photo = "../../static/checkphoto/52119F292900.jpg";
                                        P00003_DataEntity.dock = dock;
                                        P00003_DataEntity.quantity = quantity;
                                        apiResult.data = P00003_DataEntity;

                                        #region 打印标签,根据入库单号进行区分
                                        //获取标签信息
                                        DataTable getLabel = P00003_Logic.GetLabel(inputNo);
                                        //获取打印机信息
                                        DataTable getPrintName = P00003_Logic.GetPrintName(iP);

                                        if (getLabel.Rows.Count > 0 && getPrintName.Rows.Count == 1)
                                        {
                                            if (getLabel.Rows[0][0].ToString() == "")
                                            {
                                                string printName = getPrintName.Rows[0][0].ToString();
                                                int lbResultIn = P00003_Logic.InsertTP1(iP, opearteId, serverTime, inputNo, printName);
                                            }
                                        }
                                        else if (getPrintName.Rows.Count != 1)
                                        {
                                            apiResult.code = ComConstant.ERROR_CODE;
                                            apiResult.data = "打印机表中没有有效的标签打印机,请联系管理员!";
                                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        apiResult.code = ComConstant.ERROR_CODE;
                                        apiResult.data = "品番" + partId + "包材构成数据中不存在有效数据,请检查";
                                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                                    }
                                }
                                else if (validateSJ2.Rows.Count != 1)
                                {
                                    apiResult.code = ComConstant.ERROR_CODE;
                                    apiResult.data = "品番" + partId + "不存在或存在多条入库数据,请检查";
                                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                                }
                                else if (getPM.Rows.Count != 1)
                                {
                                    apiResult.code = ComConstant.ERROR_CODE;
                                    apiResult.data = "品番" + partId + "获取不到对应品目信息,请检查";
                                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                                }
                                else if (validateData1.Rows.Count != 1)
                                {
                                    apiResult.code = ComConstant.ERROR_CODE;
                                    apiResult.data = "品番" + partId + "在当前日期不存在或存在多条有效数据,请检查";
                                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                                }
                            }
                            else//抽检或全检验证检查信息,必须为OK
                            {
                                //判断检查状态+包装单位
                                DataTable validateSJ3 = P00003_Logic.ValidateOpr(partId, quantity, dock, kanbanOrderNo, kanbanSerial);//02为包装单位
                                //获取品目信息
                                DataTable getPM = P00003_Logic.GetPM(dock, partId);//00为品目
                                //获取包装数量
                                DataTable validateData1 = P00003_Logic.ValidateData1(partId, scanTime);//00为收容数

                                if (validateSJ3.Rows.Count == 1 && getPM.Rows.Count == 1 && validateData1.Rows.Count == 1)
                                {
                                    string packingQuatity = validateSJ3.Rows[0][2].ToString();//包装单位
                                    string pinmu = getPM.Rows[0][0].ToString();//品目
                                    string packQuantity = validateData1.Rows[0][0].ToString();//收容数
                                    //获取包材信息
                                    DataTable getPackInfo = P00003_Logic.GetPackInfo(partId, scanTime, packingQuatity, quantity);//vcPackNo,vcDistinguish,iBiYao 
                                    if (getPackInfo.Rows.Count > 0)
                                    {
                                        P00003_DataEntity.packParts = getPackInfo;//包材list
                                        P00003_DataEntity.caseQuantity = quantity;//装箱数
                                        P00003_DataEntity.packQuantity = quantity;//包装数
                                        P00003_DataEntity.PM = pinmu;//品目消息
                                        P00003_DataEntity.packingQuatity = packingQuatity;//包装单位
                                        P00003_DataEntity.checkType = checkType;
                                        P00003_DataEntity.partId = partId;
                                        P00003_DataEntity.kanbanOrderNo = kanbanOrderNo;
                                        P00003_DataEntity.kanbanSerial = kanbanSerial;
                                        P00003_DataEntity.kanban = kanbanOrderNo + " " + kanbanSerial;
                                        P00003_DataEntity.photo = "../../static/checkphoto/52119F292900.jpg";
                                        P00003_DataEntity.dock = dock;
                                        P00003_DataEntity.quantity = quantity;
                                        #region 打印标签,根据入库单号进行区分
                                        DataTable getLabel = P00003_Logic.GetLabel(inputNo);
                                        DataTable getPrintName = P00003_Logic.GetPrintName(iP);
                                        if (getLabel.Rows.Count > 0 && getPrintName.Rows.Count == 1)
                                        {
                                            if (getLabel.Rows[0][0].ToString() == "")
                                            {
                                                string printName = getPrintName.Rows[0][0].ToString();
                                                int lbResultIn = P00003_Logic.InsertTP1(iP, opearteId, serverTime, inputNo, printName);
                                            }
                                        }
                                        else if (getPrintName.Rows.Count != 1)
                                        {
                                            apiResult.code = ComConstant.ERROR_CODE;
                                            apiResult.data = "打印机表中没有有效的标签打印机,请联系管理员!";
                                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                                        }
                                        #endregion
                                        apiResult.data = P00003_DataEntity;
                                    }
                                    else
                                    {
                                        apiResult.code = ComConstant.ERROR_CODE;
                                        apiResult.data = "品番" + partId + "包材构成数据中不存在有效数据,请检查";
                                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                                    }
                                }
                                else if (validateSJ3.Rows.Count != 1)
                                {
                                    apiResult.code = ComConstant.ERROR_CODE;
                                    apiResult.data = "品番" + partId + "不存在检查数据或检查结果为NG,请检查";
                                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                                }
                                else if (getPM.Rows.Count != 1)
                                {
                                    apiResult.code = ComConstant.ERROR_CODE;
                                    apiResult.data = "品番" + partId + "获取不到对应品目信息,请检查";
                                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                                }
                                else if (validateData1.Rows.Count != 1)
                                {
                                    apiResult.code = ComConstant.ERROR_CODE;
                                    apiResult.data = "品番" + partId + "在当前日期不存在或存在多条有效数据,请检查";
                                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                                }
                            }
                        }
                        else
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "品番" + partId + "在检查区分表不存在或在当前日期存在多条有效数据,请检查";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                    }
                    else if (validateSJ.Rows.Count > 0 && validateSJ1.Rows.Count == 0 && validateInv1.Rows.Count == 1)//进行了包装不装箱  包装数量为0 装箱数量为数量 不需要包装数量
                    {
                        string supplier_id = validateData.Rows[0][1].ToString();
                        #region 之前进行包装不装箱的数据
                        DataTable getCheckType = P00003_Logic.GetCheckType(partId, scanTime, supplier_id);//获得检查区分
                        if (getCheckType.Rows.Count == 1)
                        {
                            string checkType = getCheckType.Rows[0][0].ToString();
                            DataTable validateSJ2 = null;
                            if (checkType == "免检")
                            {
                                validateSJ2 = P00003_Logic.ValidateOpr5(partId, quantity, dock, kanbanOrderNo, kanbanSerial);
                            }
                            else
                            {
                                validateSJ2 = P00003_Logic.ValidateOpr4(partId, quantity, dock, kanbanOrderNo, kanbanSerial);//02为包装单位 08为已包装数量
                            }
                            if (validateSJ2.Rows.Count == 1 && validateInv1.Rows.Count == 1)
                            {
                                #region 进行包装不装箱之后包装数量为零,装箱数量为之前数量,包装的时候继续判断
                                DataTable getPM = P00003_Logic.GetPM(dock, partId);//00为品目
                                DataTable validateData1 = P00003_Logic.ValidateData1(partId, scanTime);//00为收容数

                                if (getPM.Rows.Count == 1 && validateData1.Rows.Count == 1)
                                {
                                    string packingQuatity = validateSJ2.Rows[0][2].ToString();//包装单位
                                    string pinmu = getPM.Rows[0][0].ToString();//品目
                                    string packQuantity = validateData1.Rows[0][0].ToString();//收容数

                                    DataTable getPackInfo = P00003_Logic.GetPackInfo(partId, scanTime, packingQuatity, quantity);//vcPackNo,vcDistinguish,iBiYao 
                                    if (getPackInfo.Rows.Count > 0)
                                    {
                                        P00003_DataEntity.packParts = getPackInfo;//包材list
                                        P00003_DataEntity.caseQuantity = quantity;//装箱数
                                        P00003_DataEntity.packQuantity = "0";//包装数
                                        P00003_DataEntity.PM = pinmu;//品目消息
                                        P00003_DataEntity.packingQuatity = packingQuatity;//包装单位
                                        P00003_DataEntity.checkType = checkType;
                                        P00003_DataEntity.partId = partId;
                                        P00003_DataEntity.kanbanOrderNo = kanbanOrderNo;
                                        P00003_DataEntity.kanbanSerial = kanbanSerial;
                                        P00003_DataEntity.kanban = kanbanOrderNo + " " + kanbanSerial;
                                        P00003_DataEntity.photo = "../../static/checkphoto/52119F292900.jpg";
                                        P00003_DataEntity.dock = dock;
                                        P00003_DataEntity.quantity = quantity;
                                        apiResult.data = P00003_DataEntity;
                                    }
                                    else
                                    {
                                        apiResult.code = ComConstant.ERROR_CODE;
                                        apiResult.data = "品番" + partId + "包材构成数据中不存在有效数据,请检查";
                                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                                    }
                                }
                                else if (getPM.Rows.Count != 1)
                                {
                                    apiResult.code = ComConstant.ERROR_CODE;
                                    apiResult.data = "品番" + partId + "获取不到对应品目信息,请检查";
                                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                                }
                                else if (validateData1.Rows.Count != 1)
                                {
                                    apiResult.code = ComConstant.ERROR_CODE;
                                    apiResult.data = "品番" + partId + "在当前日期不存在或存在多条有效数据,请检查";
                                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                                }
                                #endregion
                            }
                            else if (validateSJ2.Rows.Count != 1)
                            {
                                apiResult.code = ComConstant.ERROR_CODE;
                                apiResult.data = "品番" + partId + "在作业实绩表中不存在有效数据,请检查";
                                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                            }
                            else if (validateInv1.Rows.Count != 1)
                            {
                                apiResult.code = ComConstant.ERROR_CODE;
                                apiResult.data = "品番" + partId + "在入出库履历表中没有有效数据,请检查";
                                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                            }
                        }
                        else
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "品番" + partId + "在检查区分表中没有有效数据,请检查";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        #endregion
                    }
                    else if (validateSJ.Rows.Count > 0 && validateInv1.Rows.Count == 1 && validateSJ1.Rows.Count > 0)//进行了劈票
                    {
                        #region 进行了劈票,需要验证待出货数量
                        int packQuantity1 = 0;
                        int zxQuantity = 0;
                        string supplier_id = validateData.Rows[0][1].ToString();
                        for (int i = 0; i < validateSJ.Rows.Count; i++)
                        {
                            packQuantity1 += int.Parse(validateSJ.Rows[i][0].ToString());
                        }
                        for (int j = 0; j < validateSJ1.Rows.Count; j++)
                        {
                            zxQuantity += int.Parse(validateSJ1.Rows[j][0].ToString());
                        }
                        int dbz = int.Parse(validateInv1.Rows[0][0].ToString());
                        int dzx = int.Parse(validateInv1.Rows[0][1].ToString());
                        if ((int.Parse(quantity) - packQuantity1 <= dbz) || (int.Parse(quantity) - zxQuantity <= dzx))
                        {

                            #region  可以进行包装装箱作业
                            DataTable getCheckType = P00003_Logic.GetCheckType(partId, scanTime, supplier_id);//获得检查区分
                            if (getCheckType.Rows.Count == 1)
                            {
                                string checkType = getCheckType.Rows[0][0].ToString();
                                if (checkType == "免检")//免检品验证入库信息返回数据
                                {
                                    DataTable validateSJ2 = P00003_Logic.ValidateOpr1(partId, quantity, dock, kanbanOrderNo, kanbanSerial);//02为包装单位 08为已包装数量
                                    DataTable getPM = P00003_Logic.GetPM(dock, partId);//00为品目
                                    DataTable validateData1 = P00003_Logic.ValidateData1(partId, scanTime);//00为收容数

                                    if (validateSJ2.Rows.Count == 1 && getPM.Rows.Count == 1 && validateData1.Rows.Count == 1)
                                    {
                                        string packingQuatity = validateSJ2.Rows[0][2].ToString();//包装单位
                                        string quantity1 = validateSJ2.Rows[0][2].ToString();
                                        string pinmu = getPM.Rows[0][0].ToString();//品目
                                        string packQuantity = validateData1.Rows[0][0].ToString();//收容数


                                        DataTable getPackInfo = P00003_Logic.GetPackInfo(partId, scanTime, packingQuatity, quantity);//vcPackNo,vcDistinguish,iBiYao 
                                        if (getPackInfo.Rows.Count > 0)
                                        {

                                            P00003_DataEntity.packParts = getPackInfo;//包材list
                                            P00003_DataEntity.caseQuantity = (int.Parse(quantity) - zxQuantity).ToString();//装箱数
                                            P00003_DataEntity.packQuantity = (int.Parse(quantity) - packQuantity1).ToString();//包装数
                                            P00003_DataEntity.PM = pinmu;//品目消息
                                            P00003_DataEntity.packingQuatity = packingQuatity;//包装单位
                                            P00003_DataEntity.checkType = checkType;
                                            P00003_DataEntity.partId = partId;
                                            P00003_DataEntity.kanbanOrderNo = kanbanOrderNo;
                                            P00003_DataEntity.kanbanSerial = kanbanSerial;
                                            P00003_DataEntity.kanban = kanbanOrderNo + " " + kanbanSerial;
                                            P00003_DataEntity.photo = "../../static/checkphoto/52119F292900.jpg";
                                            P00003_DataEntity.dock = dock;
                                            apiResult.data = P00003_DataEntity;
                                            //插入数据
                                        }
                                        else
                                        {
                                            apiResult.code = ComConstant.ERROR_CODE;
                                            apiResult.data = "品番" + partId + "包材构成数据中不存在有效数据,请检查";
                                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                                        }
                                    }
                                    else if (validateSJ2.Rows.Count != 1)
                                    {
                                        apiResult.code = ComConstant.ERROR_CODE;
                                        apiResult.data = "品番" + partId + "不存在或存在多条入库数据,请检查";
                                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                                    }
                                    else if (getPM.Rows.Count != 1)
                                    {
                                        apiResult.code = ComConstant.ERROR_CODE;
                                        apiResult.data = "品番" + partId + "获取不到对应品目信息,请检查";
                                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                                    }
                                    else if (validateData1.Rows.Count != 1)
                                    {
                                        apiResult.code = ComConstant.ERROR_CODE;
                                        apiResult.data = "品番" + partId + "在当前日期不存在或存在多条有效数据,请检查";
                                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                                    }
                                }
                                else//抽检或全检验证检查信息,必须为OK
                                {
                                    DataTable validateSJ3 = P00003_Logic.ValidateOpr(partId, quantity, dock, kanbanOrderNo, kanbanSerial);//02为包装单位
                                    DataTable getPM = P00003_Logic.GetPM(dock, partId);//00为品目
                                    DataTable validateData1 = P00003_Logic.ValidateData1(partId, scanTime);//00为收容数
                                    if (validateSJ3.Rows.Count == 1 && getPM.Rows.Count == 1 && validateData1.Rows.Count == 1)
                                    {
                                        string packingQuatity = validateSJ3.Rows[0][2].ToString();//包装单位
                                        string pinmu = getPM.Rows[0][0].ToString();//品目
                                        string packQuantity = validateData1.Rows[0][0].ToString();//收容数
                                        DataTable getPackInfo = P00003_Logic.GetPackInfo(partId, scanTime, packingQuatity, quantity);//vcPackNo,vcDistinguish,iBiYao 
                                        if (getPackInfo.Rows.Count > 0)
                                        {
                                            P00003_DataEntity.packParts = getPackInfo;//包材list
                                            P00003_DataEntity.caseQuantity = (int.Parse(quantity) - zxQuantity).ToString();//装箱数
                                            P00003_DataEntity.packQuantity = (int.Parse(quantity) - packQuantity1).ToString();//包装数
                                            P00003_DataEntity.PM = pinmu;//品目消息
                                            P00003_DataEntity.packingQuatity = packingQuatity;//包装单位
                                            P00003_DataEntity.checkType = checkType;
                                            P00003_DataEntity.partId = partId;
                                            P00003_DataEntity.kanbanOrderNo = kanbanOrderNo;
                                            P00003_DataEntity.kanbanSerial = kanbanSerial;
                                            P00003_DataEntity.kanban = kanbanOrderNo + " " + kanbanSerial;
                                            P00003_DataEntity.photo = "../../static/checkphoto/52119F292900.jpg";
                                            P00003_DataEntity.dock = dock;
                                            //插入数据
                                            apiResult.data = P00003_DataEntity;
                                        }
                                        else
                                        {
                                            apiResult.code = ComConstant.ERROR_CODE;
                                            apiResult.data = "品番" + partId + "包材构成数据中不存在有效数据,请检查";
                                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                                        }
                                    }
                                    else if (validateSJ3.Rows.Count != 1)
                                    {
                                        apiResult.code = ComConstant.ERROR_CODE;
                                        apiResult.data = "品番" + partId + "不存在检查数据或检查结果为NG,请检查";
                                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                                    }
                                    else if (getPM.Rows.Count != 1)
                                    {
                                        apiResult.code = ComConstant.ERROR_CODE;
                                        apiResult.data = "品番" + partId + "获取不到对应品目信息,请检查";
                                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                                    }
                                    else if (validateData1.Rows.Count != 1)
                                    {
                                        apiResult.code = ComConstant.ERROR_CODE;
                                        apiResult.data = "品番" + partId + "在当前日期不存在或存在多条有效数据,请检查";
                                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                                    }
                                }
                            }
                            else
                            {
                                apiResult.code = ComConstant.ERROR_CODE;
                                apiResult.data = "品番" + partId + "在检查区分表中不存在有效数据,请检查";
                                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                            }
                        }
                        else
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "当前看板数量不符合设定的规则,请检查!";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        #endregion
                        #endregion
                    }
                    else
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "当前看板没有进行检查作业,请确认!";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    #endregion
                }
                else if (validateData.Rows.Count != 1)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "品番" + partId + "在品番基础数据中不存在有效数据";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "品番" + partId + "在作业实绩表中不存在有效数据";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "验证看板信息失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
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
                int caseResultUp = P00003_Logic.UpdateCase1(opearteId, iP);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "更新信息失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }






            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        }
        #endregion

        #region 打印装箱单
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
                DataTable getPoint = P00003_Logic.GetPoingNo(iP);                                                                                        //0605
                if (getPoint.Rows.Count != 1)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "当前点位信息异常，请检查！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }
                string pointType = getPoint.Rows[0][0].ToString() + getPoint.Rows[0][1].ToString();
                string serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
                DataTable getCase2 = P00003_Logic.GetCaseInfo(caseNo);
                DataTable getCase3 = P00003_Logic.GetCaseInfo1(caseNo);
                if (getCase2.Rows.Count > 0 && getCase3.Rows.Count == 0)
                {
                    //vcInstructionNo,vcPart_id,vcOrderNo,vcLianFanNo,iQuantity
                    for (int i = 0; i < getCase2.Rows.Count; i++)
                    {
                        string inputNo = getCase2.Rows[i][0].ToString();
                        string partId = getCase2.Rows[i][1].ToString();
                        string kanbanOrderNo = getCase2.Rows[i][2].ToString();
                        string kanbanSerial = getCase2.Rows[i][3].ToString();
                        string quantity = getCase2.Rows[i][4].ToString();
                        string dock = getCase2.Rows[i][5].ToString();
                        DataTable validateOpr = P00003_Logic.ValidateOpr1(partId, quantity, dock, kanbanOrderNo, kanbanSerial);
                        if (validateOpr.Rows.Count == 1)
                        {

                            string supplier_id = validateOpr.Rows[0][0].ToString();//供应商代码
                            string supplierGQ = validateOpr.Rows[0][1].ToString();//供应商工区
                            string bZUnit = validateOpr.Rows[0][2].ToString();//包装单位

                            string labelStart = validateOpr.Rows[0][4].ToString();//标签起
                            string labelEnd = validateOpr.Rows[0][5].ToString();//标签止
                            string inoutFlag = validateOpr.Rows[0][6].ToString();//内外区分
                            string checkStatus = "OK";//检查状态

                            //插入到箱号List,插入作业实绩,修改入出库履历
                            DataTable validateInv = P00003_Logic.ValidateInv(inputNo);
                            // DataTable validateOpr1 = P00003_Logic.ValidateSJ1(partId, dock, kanbanOrderNo, kanbanSerial);
                            DataTable getCheckType = P00003_Logic.GetCheckType(partId, serverTime, supplier_id);
                            DataTable getPartsName = P00003_Logic.GetPartsName(serverTime, partId);
                            if (getPartsName.Rows.Count == 1 && getCheckType.Rows.Count == 1 && validateInv.Rows.Count == 1 && int.Parse(quantity) <= (int.Parse(validateInv.Rows[0][0].ToString())))
                            {
                                //
                                string bzPlant = validateInv.Rows[0][2].ToString();
                                string sHF = validateInv.Rows[0][3].ToString();
                                string checkType = getCheckType.Rows[0][0].ToString();
                                string cpdName = "一汽丰田";
                                string cpdAddress = "天津塘沽开发区第九大街";
                                string partsName = getPartsName.Rows[0][0].ToString();
                                byte[] vs = P00003_Logic.GenerateQRCode(caseNo);
                                int sjResultIn = P00003_Logic.InsertSj(supplier_id, supplierGQ, bZUnit, checkType, labelStart, labelEnd, inoutFlag, checkStatus, bzPlant, inputNo, quantity, partId, kanbanOrderNo, kanbanSerial, dock, opearteId, serverTime, serverTime, iP, sHF, quantity, caseNo, pointType);//此处需要改动
                                int invResultUp = P00003_Logic.UpdateInv1(partId, kanbanOrderNo, kanbanSerial, quantity);
                                int caseResultIn = P00003_Logic.InsertCase(sHF, cpdName, cpdAddress, caseNo, inputNo, partId, quantity, partsName, opearteId, serverTime, iP, vs, labelStart, labelEnd);
                                int caseResultUp1 = P00003_Logic.UpdateCase5(iP, caseNo);






                            }
                            else if (getPartsName.Rows.Count != 1)
                            {

                                apiResult.code = ComConstant.ERROR_CODE;
                                apiResult.data = "品番" + partId + "在品番基础数据中没有有效数据!";
                                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);


                            }
                            else if (getCheckType.Rows.Count != 1)
                            {

                                apiResult.code = ComConstant.ERROR_CODE;
                                apiResult.data = "品番" + partId + "在检查区分表中没有有效数据!";
                                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);


                            }

                            else if (validateInv.Rows.Count != 1)
                            {
                                apiResult.code = ComConstant.ERROR_CODE;
                                apiResult.data = "品番" + partId + "在入出库履历表中没有有效数据!";
                                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);


                            }






                        }
                        else
                        {

                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "品番" + partId + "在作业实绩中没有有效数据!";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);



                        }


                    }

                    //循环结束为止
                    #region 循环结束之后需要更新打印时间,需要更新总的数量
                    DataTable getCaseList = P00003_Logic.GetCaseList(iP, caseNo);
                    string count = getCaseList.Rows[0][0].ToString();
                    if (int.Parse(count) > 0)
                    {
                        string sum = getCaseList.Rows[0][1].ToString();
                        int caseResultUp = P00003_Logic.UpdateCase(count, sum, caseNo);
                        int caResultIn = P00003_Logic.InsertTP(iP, opearteId, serverTime, caseNo);
                        int boxResultUp = P00003_Logic.UpdateBox(caseNo, serverTime);



                    }
                    else
                    {

                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "没有需要打印的数据!";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);


                    }



                    #endregion



                }
                else if (getCase3.Rows.Count > 0)
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "该箱号已经打印装箱单!";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);


                }
                else if (getCase2.Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "没有需要打印的数据!";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }

            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "打印装箱单失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }






            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
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


            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "获取箱号信息失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

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

            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, "");
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "更改状态失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }

            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        }
        #endregion

        #region  包装-验证箱号
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
                string strPointState = "0";
                string strOperatorID = opearteId;
                P00003_Logic.SetCaseNoInfo(strBoxNo, strCaseNo, strHostIp, strSheBeiNo, strPointState, strOperatorID);

                P00003_DataEntity.kanbanQuantity = dtCaseNoInfo.Rows[0]["kanbanQuantity"].ToString();
                apiResult.data = P00003_DataEntity;
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "箱号验证失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        }
        #endregion

        #region  包装装箱
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
                string quantity = dataForm.Quantity == null ? "" : dataForm.Quantity;//数量
                string dock = dataForm.Dock == null ? "" : dataForm.Dock;//受入
                string kanbanOrderNo = dataForm.KanbanOrderNo == null ? "" : dataForm.KanbanOrderNo;//看板订单号
                string kanbanSerial = dataForm.KanbanSerial == null ? "" : dataForm.KanbanSerial;//看板连番
                string scanTime = dataForm.ScanTime == null ? "" : dataForm.ScanTime;//客户端时间
                string serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
                string timeStart = dataForm.TimeStart == null ? "" : dataForm.TimeStart;
                string timeEnd = dataForm.TimeEnd == null ? "" : dataForm.TimeEnd;
                string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");//客户端IP地址
                DataTable getPoint = P00003_Logic.GetPoingNo(iP);
                if (getPoint.Rows.Count != 1)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "当前点位信息异常，请检查！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }
                string pointType = getPoint.Rows[0][0].ToString() + getPoint.Rows[0][1].ToString();


                string checkType = dataForm.CheckType == null ? "" : dataForm.CheckType;//检查区分
                string caseNo = dataForm.CaseNo == null ? "" : dataForm.CaseNo;//箱号
                string formatDate = serverTime.Substring(0, 10).Replace("-", "");
                string packQuantity = dataForm.PackQuantity == null ? "" : dataForm.PackQuantity;//包装数量
                DataTable validateOpr;//验证作业实绩
                DataTable validateOpr1;
                DataTable validateInv;//验证入出库履历
                if (checkType == "")//检查类型为空,无法判断前工程,报错退出
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "品番" + partId + "获得检查类型失败,请联系管理员!";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                else if (checkType == "免检")//免检品的前工程为入库
                {

                    // validateHtrst = P00003_Logic.ValidateHtrst2(partId, quantity, dock, kanbanOrderNo, kanbanSerial);//验证实绩情报表中有没有入荷数据
                    //validateHtrst1 = P00003_Logic.ValidateHtrst1(partId, quantity, dock, kanbanOrderNo, kanbanSerial);//验证实绩情报表中有没有包装数据
                    validateOpr1 = P00003_Logic.ValidateOpr2(partId, quantity, dock, kanbanOrderNo, kanbanSerial);                                                                                                //从作业实际取入荷数据
                    validateOpr = P00003_Logic.ValidateOpr1(partId, quantity, dock, kanbanOrderNo, kanbanSerial);
                    //从入出库履历取入荷数据
                    validateInv = P00003_Logic.ValidateInv(partId, quantity, dock, kanbanOrderNo, kanbanSerial);
                    //校验品番的有效性
                    // validateData = P00003_Logic.ValidateData(partId, scanTime, dock);


                }
                else//其他类型的前工程为检查

                {

                    // validateHtrst = P00003_Logic.ValidateHtrst(partId, quantity, dock, kanbanOrderNo, kanbanSerial);//验证实绩情报表中有没有检查数据
                    // validateHtrst1 = P00003_Logic.ValidateHtrst1(partId, quantity, dock, kanbanOrderNo, kanbanSerial);//验证实绩情报表中有没有包装数据
                    validateOpr1 = P00003_Logic.ValidateOpr3(partId, quantity, dock, kanbanOrderNo, kanbanSerial);                                                                                             //从作业实际取入荷数据
                    validateOpr = P00003_Logic.ValidateOpr(partId, quantity, dock, kanbanOrderNo, kanbanSerial);
                    //从入出库履历取入荷数据
                    validateInv = P00003_Logic.ValidateInv(partId, quantity, dock, kanbanOrderNo, kanbanSerial);
                    //校验品番的有效性
                    // validateData = P00003_Logic.ValidateData(partId, scanTime, dock);

                }
                DataTable getPartsName = P00003_Logic.GetPartsName(scanTime, partId);

                if (validateOpr.Rows.Count == 1 && validateInv.Rows.Count == 1 && getPartsName.Rows.Count == 1)
                {
                    if (packQuantity == "0")//只进行装箱作业

                    {
                        string bzPlant = validateInv.Rows[0][0].ToString();//包装场
                        string sHF = validateInv.Rows[0][1].ToString();//收货方
                        string inputNo = validateInv.Rows[0][2].ToString();//入库单号
                        string dBZ = validateInv.Rows[0][3].ToString();//待包装
                        string dZX = validateInv.Rows[0][4].ToString();//待装箱
                        string dCH = validateInv.Rows[0][5].ToString();//待出荷


                        string supplier_id = validateOpr.Rows[0][0].ToString();//供应商代码
                        string supplierGQ = validateOpr.Rows[0][1].ToString();//供应商工区
                        string bZUnit = validateOpr.Rows[0][2].ToString();//包装单位

                        string labelStart = validateOpr.Rows[0][4].ToString();//标签起
                        string labelEnd = validateOpr.Rows[0][5].ToString();//标签止
                        string inoutFlag = validateOpr.Rows[0][6].ToString();//内外区分
                        string checkStatus = validateOpr.Rows[0][7].ToString();//检查状态

                        string partsName = getPartsName.Rows[0][0].ToString();



                        // DataTable getTime = P00003_Logic.GetTime(formatDate, opearteId);
                        // P00003_DataEntity.packTotalTime = getTime.Rows[0][4].ToString();
                        // int qbResultIn = P00003_Logic.Insert(partId, quantity, dock, kanbanOrderNo, kanbanSerial, scanTime, iP, serverTime, trolleyNo, sHF, inputNo);//插入实绩情报表
                        // int invResult = P00003_Logic.UpdateInv(partId, quantity, dock, kanbanOrderNo, kanbanSerial, scanTime, serverTime, opearteId);                                                                                                                    //更新入出库履历表
                        //  int oprReusultIn = P00003_Logic.InsertOpr(bzPlant, inputNo, kanbanOrderNo, kanbanSerial, partId, inoutFlag, supplier_id, supplierGQ, scanTime, serverTime, quantity, bZUnit, sHF, dock, checkType, labelStart, labelEnd, checkStatus, opearteId, timeStart, timeEnd);//插入作业实际表
                        //  #region 打印标签,根据入库单号进行区分

                        // int lbResultIn = P00003_Logic.InsertTP1(iP, opearteId, serverTime, inputNo);










                        // #endregion









                        //int qbResultIn1 = P00003_Logic.Insert1(partId, quantity, dock, kanbanOrderNo, kanbanSerial, scanTime, iP, serverTime, trolleyNo, sHF, inputNo,caseNo);//插入实绩情报表
                        // int invResultUp = P00003_Logic.UpdateInv1(partId, kanbanOrderNo, kanbanSerial, dBZ, dCH, "", quantity);                                                                                                                 //更新入出库履历表
                        // byte[] vs = GenerateQRCode(caseNo);                                                                                                                                                                                                                       // int oprReusultIn1 = P00003_Logic.InsertOpr1(bzPlant, inputNo, kanbanOrderNo, kanbanSerial, partId, inoutFlag, supplier_id, supplierGQ, scanTime, serverTime, quantity, bZUnit, sHF, dock, checkType, labelStart, labelEnd, checkStatus, caseNo);//插入作业实际表
                        // int caseResultIn = P00003_Logic.InsertCase(sHF, cpdName, cpdAddress, caseNo, inputNo, partId, quantity, partsName, opearteId, serverTime, iP, vs, labelStart, labelEnd);
                        //int sjResultIn = P00003_Logic.InsertSj1(supplier_id, supplierGQ, bZUnit, checkType, labelStart, labelEnd, inoutFlag, checkStatus, bzPlant, inputNo, quantity, partId, kanbanOrderNo, kanbanSerial, dock, opearteId, scanTime, serverTime, iP, sHF,  caseNo);


                        // int sjResultIn = P00003_Logic.InsertSj(supplier_id, supplierGQ, bZUnit, checkType, labelStart, labelEnd, inoutFlag, checkStatus, bzPlant, inputNo, quantity, partId, kanbanOrderNo, kanbanSerial, dock, opearteId, scanTime, serverTime, iP, sHF, quantity, caseNo);//此处需要改动
                        //int invResultUp = P00003_Logic.UpdateInv1(partId, kanbanOrderNo, kanbanSerial, quantity);
                        DataTable getQuantity = P00003_Logic.GetQuantity(kanbanOrderNo, kanbanSerial, partId, dock);

                        if (getQuantity.Rows.Count == 1)
                        {
                            string rhQuantity = getQuantity.Rows[0][0].ToString();


                            int boxResultIn = P00003_Logic.InsertBox(caseNo, inputNo, partId, kanbanOrderNo, kanbanSerial, quantity, opearteId, scanTime, labelStart, labelEnd, rhQuantity, serverTime, dock);


                        }
                        else
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "品番" + partId + "在作业实绩表中不存在有效数据,请检查!";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);


                        }








                        DataTable validateCaseNo3 = P00003_Logic.ValidateCaseNo3(caseNo);
                        P00003_DataEntity.kanbanQuantity = validateCaseNo3.Rows.Count.ToString();

                        apiResult.data = P00003_DataEntity;










                    }
                    else
                    {
                        /*
                        #region   根据开始结束时间算出时间差

                        string getYearData = timeStart.Split(" ")[0];
                        string getTimeData = timeStart.Split(" ")[1];

                        int beforeYear = int.Parse(getYearData.Split("-")[0]);
                        int beforeMonth = int.Parse(getYearData.Split("-")[1]);
                        int beforeDate = int.Parse(getYearData.Split("-")[2]);
                        int beforeHours = int.Parse(getTimeData.Split(":")[0]);
                        int beforeMinutes = int.Parse(getTimeData.Split(":")[1]);
                        int beforeSeconds = int.Parse(getTimeData.Split(":")[2]);


                        string getYearData1 = timeEnd.Split(" ")[0];
                        string getTimeData1 = timeEnd.Split(" ")[1];

                        int afterYear = int.Parse(getYearData1.Split("-")[0]);
                        int afterMonth = int.Parse(getYearData1.Split("-")[1]);
                        int afterDate = int.Parse(getYearData1.Split("-")[2]);

                        int afterHours = int.Parse(getTimeData1.Split(":")[0]);
                        int afterMinutes = int.Parse(getTimeData1.Split(":")[1]);
                        int afterSeconds = int.Parse(getTimeData1.Split(":")[2]);


                        int totalTime = (afterYear - beforeYear) * 365 * 24 * 60 * 60 +
                          (afterMonth - beforeMonth) * 30 * 24 * 60 * 60 +
                          (afterDate - beforeDate) * 24 * 60 * 60 +
                          (afterHours - beforeHours) * 60 * 60 + (afterMinutes - beforeMinutes) * 60 + (afterSeconds - beforeSeconds);

                        int timeResultUp = P00003_Logic.UpdateTime(formatDate, totalTime, opearteId);


                        DataTable getTime = P00003_Logic.GetTime(formatDate, opearteId);
                        P00003_DataEntity.packTotalTime = getTime.Rows[0][4].ToString();


                        #endregion
                        */

                        string bzPlant = validateInv.Rows[0][0].ToString();//包装场
                        string sHF = validateInv.Rows[0][1].ToString();//收货方
                        string inputNo = validateInv.Rows[0][2].ToString();//入库单号
                        string dBZ = validateInv.Rows[0][3].ToString();//待包装
                        string dZX = validateInv.Rows[0][4].ToString();//待装箱
                        string dCH = validateInv.Rows[0][5].ToString();//待出荷


                        string supplier_id = validateOpr.Rows[0][0].ToString();//供应商代码
                        string supplierGQ = validateOpr.Rows[0][1].ToString();//供应商工区
                        string bZUnit = validateOpr.Rows[0][2].ToString();//包装单位

                        string labelStart = validateOpr.Rows[0][4].ToString();//标签起
                        string labelEnd = validateOpr.Rows[0][5].ToString();//标签止
                        string inoutFlag = validateOpr.Rows[0][6].ToString();//内外区分
                        string checkStatus = validateOpr.Rows[0][7].ToString();//检查状态

                        string partsName = getPartsName.Rows[0][0].ToString();

                        string cpdName = "一汽丰田";
                        string cpdAddress = "天津塘沽开发区第九大街";


                        // int qbResultIn = P00003_Logic.Insert(partId, quantity, dock, kanbanOrderNo, kanbanSerial, scanTime, iP, serverTime, trolleyNo, sHF, inputNo);//插入实绩情报表
                        int invResult = P00003_Logic.UpdateInv(partId, quantity, dock, kanbanOrderNo, kanbanSerial, scanTime, serverTime, opearteId);                                                                                                                    //更新入出库履历表
                        int oprReusultIn = P00003_Logic.InsertOpr(bzPlant, inputNo, kanbanOrderNo, kanbanSerial, partId, inoutFlag, supplier_id, supplierGQ, scanTime, serverTime, quantity, bZUnit, sHF, dock, checkType, labelStart, labelEnd, checkStatus, opearteId, timeStart, timeEnd, iP, pointType);//插入作业实际表
                        #region 对包材进行操作,消耗实绩,消减在库

                        DataTable getPack = P00003_Logic.GetPackData1(partId, serverTime);
                        if (getPack.Rows.Count > 0)
                        {

                            for (int i = 0; i < getPack.Rows.Count; i++)
                            {
                                string packNo = getPack.Rows[i][0].ToString();
                                string biYao = getPack.Rows[i][1].ToString();
                                string gpsNo = getPack.Rows[i][2].ToString();
                                DataTable getPackbase = P00003_Logic.GetPackBase(packNo, serverTime);
                                if (getPackbase.Rows.Count == 1)
                                {
                                    string packsupplier = getPackbase.Rows[0][0].ToString() + getPackbase.Rows[0][1].ToString();

                                    int packResultIn = P00003_Logic.InsertPackWork(packNo, gpsNo, packsupplier, bZUnit, biYao, opearteId, serverTime, quantity);
                                    DataTable getZaiKu = P00003_Logic.GetZaiKu(packNo, gpsNo, packsupplier);
                                    if (getZaiKu.Rows.Count == 0)
                                    {
                                        int zaikuResultIn = P00003_Logic.InsertZaiKu(packNo, gpsNo, packsupplier, opearteId, serverTime);

                                    }
                                    getZaiKu = P00003_Logic.GetZaiKu(packNo, gpsNo, packsupplier);

                                    if (getZaiKu.Rows.Count == 1)
                                    {
                                        int invResultUp = P00003_Logic.UpdateInv2(packNo, gpsNo, packsupplier, bZUnit, biYao, opearteId, serverTime, quantity);//修改包材2
                                    }
                                    else
                                    {
                                        apiResult.code = ComConstant.ERROR_CODE;
                                        apiResult.data = "包材品番" + packNo + "在在库表中不存在有效数据,请检查!";
                                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                                    }


                                }
                                else
                                {

                                    apiResult.code = ComConstant.ERROR_CODE;
                                    apiResult.data = "包材品番" + packNo + "在包材基础中不存在有效数据,请检查!";
                                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                                }




                            }





                        }
                        else
                        {

                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "品番" + partId + "在包材构成中不存在有效数据,请检查!";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);



                        }










                        #endregion





                        /*
                        #region 打印标签,根据入库单号进行区分

                        DataTable getLabel = P00003_Logic.GetLabel(inputNo);
                        if (getLabel.Rows.Count>0) { 
                        if (getLabel.Rows[0][0].ToString()=="") {
                          int lbResultIn = P00003_Logic.InsertTP1(iP, opearteId, serverTime, inputNo);
                        }
                        }
                        #endregion
                         */


                        DataTable getQuantity = P00003_Logic.GetQuantity(kanbanOrderNo, kanbanSerial, partId, dock);
                        if (getQuantity.Rows.Count == 1)
                        {
                            string rhQuantity = getQuantity.Rows[0][0].ToString();


                            int boxResultIn = P00003_Logic.InsertBox(caseNo, inputNo, partId, kanbanOrderNo, kanbanSerial, quantity, opearteId, scanTime, labelStart, labelEnd, rhQuantity, serverTime, dock);


                        }
                        else
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "品番" + partId + "在作业实绩表中不存在有效数据,请检查!";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);


                        }





                        //int qbResultIn1 = P00003_Logic.Insert1(partId, quantity, dock, kanbanOrderNo, kanbanSerial, scanTime, iP, serverTime, trolleyNo, sHF, inputNo,caseNo);//插入实绩情报表
                        // int invResultUp = P00003_Logic.UpdateInv1(partId, kanbanOrderNo, kanbanSerial, dBZ, dCH, "", quantity);                                                                                                                 //更新入出库履历表
                        // byte[] vs = GenerateQRCode(caseNo);                                                                                                                                                                                                                       // int oprReusultIn1 = P00003_Logic.InsertOpr1(bzPlant, inputNo, kanbanOrderNo, kanbanSerial, partId, inoutFlag, supplier_id, supplierGQ, scanTime, serverTime, quantity, bZUnit, sHF, dock, checkType, labelStart, labelEnd, checkStatus, caseNo);//插入作业实际表
                        //  int caseResultIn = P00003_Logic.InsertCase(sHF, cpdName, cpdAddress, caseNo, inputNo, partId, quantity, partsName, opearteId, serverTime, iP, vs, labelStart, labelEnd);
                        //int sjResultIn = P00003_Logic.InsertSj1(supplier_id, supplierGQ, bZUnit, checkType, labelStart, labelEnd, inoutFlag, checkStatus, bzPlant, inputNo, quantity, partId, kanbanOrderNo, kanbanSerial, dock, opearteId, scanTime, serverTime, iP, sHF,  caseNo);


                        // int sjResultIn = P00003_Logic.InsertSj(supplier_id, supplierGQ, bZUnit, checkType, labelStart, labelEnd, inoutFlag, checkStatus, bzPlant, inputNo, quantity, partId, kanbanOrderNo, kanbanSerial, dock, opearteId, scanTime, serverTime, iP, sHF, quantity, caseNo);//此处需要改动
                        //int invResultUp = P00003_Logic.UpdateInv1(partId, kanbanOrderNo, kanbanSerial, quantity);


                        DataTable validateCaseNo3 = P00003_Logic.ValidateCaseNo3(caseNo);
                        P00003_DataEntity.kanbanQuantity = validateCaseNo3.Rows.Count.ToString();

                        apiResult.data = P00003_DataEntity;










                        //int qbResultIn = P00003_Logic.Insert(partId, quantity, dock, kanbanOrderNo, kanbanSerial, scanTime, iP, serverTime,trolleyNo,sHF,inputNo);//插入实绩情报表
                        // int invResult = P00003_Logic.UpdateInv(partId,quantity,dock,kanbanOrderNo,kanbanSerial,scanTime,serverTime);                                                                                                                    //更新入出库履历表
                        //int oprReusultIn = P00003_Logic.InsertOpr(bzPlant, inputNo, kanbanOrderNo, kanbanSerial, partId, inoutFlag, supplier_id, supplierGQ, scanTime, serverTime, quantity, bZUnit, sHF, dock, checkType, labelStart, labelEnd,checkStatus);//插入作业实际表







                    }










                }




                else if (getPartsName.Rows.Count != 1)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "品番" + partId + "在品番基础数据里面没有有效或存在多条数据,请检查!";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }

                else if (validateOpr.Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "品番" + partId + "在作业实绩表中没有对应数据或检查结果为NG,请检查!";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                else if (validateInv.Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "品番" + partId + "在入出库履历中没有对应数据,请检查!";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }









            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "包装装箱失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }



            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        }
        private byte[] GenerateQRCode(string content)
        {
            var generator = new QRCodeGenerator();
            var codeData = generator.CreateQrCode(content, QRCodeGenerator.ECCLevel.M, true);

            QRCoder.QRCode qrcode = new QRCoder.QRCode(codeData);
            var bitmapImg = qrcode.GetGraphic(10, Color.Black, Color.White, false);
            using MemoryStream stream = new MemoryStream();
            bitmapImg.Save(stream, ImageFormat.Jpeg);
            return stream.GetBuffer();
        }
        #endregion

        #region  包装不装箱
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
                string quantity = dataForm.Quantity == null ? "" : dataForm.Quantity;//数量
                string dock = dataForm.Dock == null ? "" : dataForm.Dock;//受入
                string kanbanOrderNo = dataForm.KanbanOrderNo == null ? "" : dataForm.KanbanOrderNo;//看板订单号
                string kanbanSerial = dataForm.KanbanSerial == null ? "" : dataForm.KanbanSerial;//看板连番
                string scanTime = dataForm.ScanTime == null ? "" : dataForm.ScanTime;//客户端时间
                string serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
                string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");//客户端IP地址
                                                                                                             //0605
                DataTable getPoint = P00003_Logic.GetPoingNo(iP);                                                                                        //0605
                if (getPoint.Rows.Count != 1)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "当前点位信息异常，请检查！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }
                string pointType = getPoint.Rows[0][0].ToString() + getPoint.Rows[0][1].ToString();

                string checkType = dataForm.CheckType == null ? "" : dataForm.CheckType;
                string timeStart = dataForm.TimeStart == null ? "" : dataForm.TimeStart;
                string timeEnd = dataForm.TimeEnd == null ? "" : dataForm.TimeEnd;
                string freQuency = dataForm.FreQuency == null ? "" : dataForm.FreQuency;
                string effiency1 = dataForm.Effiency == null ? "" : dataForm.Effiency;//Effiency
                string effiency = effiency1.Replace("%", "");
                string formatParts = partId.Substring(0, 5);
                string formatDate = serverTime.Substring(0, 10).Replace("-", "");
                DataTable validateOpr;//验证作业实绩前工程
                                      // DataTable validateOpr1;//验证作业实绩当前工程
                DataTable validateInv;//验证入出库履历
                                      // DataTable validateTime = P00003_Logic.ValidateTime(partId, dock);
                                      //DataTable validateData;//验证品番基础数据
                                      // DataTable validateEffi = P00003_Logic.GetTime(formatDate, opearteId);


                if (checkType == "")//检查类型为空,无法判断前工程,报错退出,劈票的时候不能装箱
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "品番" + partId + "获得检查类型失败!";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                else if (checkType == "免检")//免检品的前工程为入库
                {





                    // validateHtrst = P00003_Logic.ValidateHtrst2(partId, quantity, dock, kanbanOrderNo, kanbanSerial);//验证实绩情报表中有没有入荷数据
                    // validateHtrst1 = P00003_Logic.ValidateHtrst1(partId, quantity, dock, kanbanOrderNo, kanbanSerial);//验证实绩情报表中有没有包装数据
                    // validateOpr1 = P00003_Logic.ValidateOpr2(partId, quantity, dock, kanbanOrderNo, kanbanSerial);                                                                                              //从作业实际取入荷数据
                    validateOpr = P00003_Logic.ValidateOpr1(partId, quantity, dock, kanbanOrderNo, kanbanSerial);
                    //从入出库履历取入荷数据
                    validateInv = P00003_Logic.ValidateInv(partId, quantity, dock, kanbanOrderNo, kanbanSerial);
                    //校验品番的有效性
                    // validateData = P00003_Logic.ValidateData(partId, scanTime, dock);


                }
                else//其他类型的前工程为检查

                {

                    // validateHtrst = P00003_Logic.ValidateHtrst(partId, quantity, dock, kanbanOrderNo, kanbanSerial);//验证实绩情报表中有没有检查数据
                    // validateHtrst1 = P00003_Logic.ValidateHtrst1(partId, quantity, dock, kanbanOrderNo, kanbanSerial);//验证实绩情报表中有没有包装数据
                    // validateOpr1 = P00003_Logic.ValidateOpr3(partId, quantity, dock, kanbanOrderNo, kanbanSerial);                                                                                                //从作业实际取入荷数据
                    validateOpr = P00003_Logic.ValidateOpr(partId, quantity, dock, kanbanOrderNo, kanbanSerial);
                    //从入出库履历取入荷数据
                    validateInv = P00003_Logic.ValidateInv(partId, quantity, dock, kanbanOrderNo, kanbanSerial);
                    //校验品番的有效性
                    //  validateData = P00003_Logic.ValidateData(partId, scanTime, dock);


                }
                bool a = (validateOpr.Rows.Count == 1);
                bool b = (validateInv.Rows.Count == 1);
                bool c = (int.Parse(validateInv.Rows[0][4].ToString()) >= int.Parse(quantity));


                if (validateOpr.Rows.Count == 1 && validateInv.Rows.Count == 1
                  && (int.Parse(validateInv.Rows[0][3].ToString()) >= int.Parse(quantity)))
                {

                    //int stanTotalTime = int.Parse(validateTime.Rows[0][0].ToString());
                    /*
                      #region   根据开始结束时间算出时间差

                      string getYearData = timeStart.Split(" ")[0];
                      string getTimeData = timeStart.Split(" ")[1];

                      int beforeYear = int.Parse(getYearData.Split("-")[0]);
                      int beforeMonth = int.Parse(getYearData.Split("-")[1]);
                      int beforeDate = int.Parse(getYearData.Split("-")[2]);
                      int beforeHours = int.Parse(getTimeData.Split(":")[0]);
                      int beforeMinutes = int.Parse(getTimeData.Split(":")[1]);
                      int beforeSeconds = int.Parse(getTimeData.Split(":")[2]);


                      string getYearData1 = timeEnd.Split(" ")[0];
                      string getTimeData1 = timeEnd.Split(" ")[1];

                      int afterYear = int.Parse(getYearData1.Split("-")[0]);
                      int afterMonth = int.Parse(getYearData1.Split("-")[1]);
                      int afterDate = int.Parse(getYearData1.Split("-")[2]);

                      int afterHours = int.Parse(getTimeData1.Split(":")[0]);
                      int afterMinutes = int.Parse(getTimeData1.Split(":")[1]);
                      int afterSeconds = int.Parse(getTimeData1.Split(":")[2]);


                      int totalTime = (afterYear - beforeYear) * 365 * 24 * 60 * 60 +
                        (afterMonth - beforeMonth) * 30 * 24 * 60 * 60 +
                        (afterDate - beforeDate) * 24 * 60 * 60 +
                        (afterHours - beforeHours) * 60 * 60 + (afterMinutes - beforeMinutes) * 60 + (afterSeconds - beforeSeconds);

                      int timeResultUp = P00003_Logic.UpdateTime(formatDate,totalTime,opearteId);



                      DataTable getTime = P00003_Logic.GetTime(formatDate, opearteId);
                      P00003_DataEntity.packTotalTime = getTime.Rows[0][4].ToString();


                      #endregion


                      */









                    string bzPlant = validateInv.Rows[0][0].ToString();//包装场
                    string sHF = validateInv.Rows[0][1].ToString();//收货方
                    string inputNo = validateInv.Rows[0][2].ToString();//入库单号
                    string dBZ = validateInv.Rows[0][3].ToString();//待包装
                    string dZX = validateInv.Rows[0][4].ToString();//待装箱
                    string dCH = validateInv.Rows[0][5].ToString();//待出荷



                    string supplier_id = validateOpr.Rows[0][0].ToString();//供应商代码
                    string supplierGQ = validateOpr.Rows[0][1].ToString();//供应商工区
                    string bZUnit = validateOpr.Rows[0][2].ToString();//包装单位

                    string labelStart = validateOpr.Rows[0][4].ToString();//标签起
                    string labelEnd = validateOpr.Rows[0][5].ToString();//标签止
                    string inoutFlag = validateOpr.Rows[0][6].ToString();//内外区分
                    string checkStatus = validateOpr.Rows[0][7].ToString();//检查状态


                    //int qbResultIn = P00003_Logic.Insert(partId, quantity, dock, kanbanOrderNo, kanbanSerial, scanTime, iP, serverTime, trolleyNo, sHF, inputNo);//插入实绩情报表
                    int invResult = P00003_Logic.UpdateInv(partId, quantity, dock, kanbanOrderNo, kanbanSerial, scanTime, serverTime, opearteId);                                                                                                                    //更新入出库履历表
                    int oprReusultIn = P00003_Logic.InsertOpr(bzPlant, inputNo, kanbanOrderNo, kanbanSerial, partId, inoutFlag, supplier_id, supplierGQ, scanTime, serverTime, quantity, bZUnit, sHF, dock, checkType, labelStart, labelEnd, checkStatus, opearteId, timeStart, timeEnd, iP, pointType);//插入作业实际表
                    #region 对包材进行操作,消耗实绩,消减在库

                    DataTable getPack = P00003_Logic.GetPackData1(partId, serverTime);
                    if (getPack.Rows.Count > 0)
                    {

                        for (int i = 0; i < getPack.Rows.Count; i++)
                        {
                            string packNo = getPack.Rows[i][0].ToString();
                            string biYao = getPack.Rows[i][1].ToString();
                            string gpsNo = getPack.Rows[i][2].ToString();
                            DataTable getPackbase = P00003_Logic.GetPackBase(packNo, serverTime);
                            if (getPackbase.Rows.Count == 1)
                            {
                                string packsupplier = getPackbase.Rows[0][0].ToString() + getPackbase.Rows[0][1].ToString();

                                int packResultIn = P00003_Logic.InsertPackWork(packNo, gpsNo, packsupplier, bZUnit, biYao, opearteId, serverTime, quantity);

                                DataTable getZaiKu = P00003_Logic.GetZaiKu(packNo, gpsNo, packsupplier);
                                if (getZaiKu.Rows.Count == 0)
                                {
                                    int zaikuResultIn = P00003_Logic.InsertZaiKu(packNo, gpsNo, packsupplier, opearteId, serverTime);

                                }
                                getZaiKu = P00003_Logic.GetZaiKu(packNo, gpsNo, packsupplier);

                                if (getZaiKu.Rows.Count == 1)
                                {
                                    int invResultUp = P00003_Logic.UpdateInv2(packNo, gpsNo, packsupplier, bZUnit, biYao, opearteId, serverTime, quantity);//修改包材2
                                }
                                else
                                {
                                    apiResult.code = ComConstant.ERROR_CODE;
                                    apiResult.data = "包材品番" + packNo + "在在库表中不存在有效数据,请检查!";
                                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                                }

                            }
                            else
                            {

                                apiResult.code = ComConstant.ERROR_CODE;
                                apiResult.data = "包材品番" + packNo + "在包材基础中不存在有效数据,请检查!";
                                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                            }




                        }





                    }
                    else
                    {

                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "品番" + partId + "在包材构成中不存在有效数据,请检查!";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);



                    }










                    #endregion





                    /*
                              #region 打印标签,根据入库单号进行区分
                              //用入库单号去查询标签表,如果第一次打印时间不为空进行插入打印标签
                              DataTable getLabel = P00003_Logic.GetLabel(inputNo);

                              if (getLabel.Rows.Count > 0)
                              {




                                if (getLabel.Rows[0][0].ToString() == "")
                                {
                                  int lbResultIn = P00003_Logic.InsertTP1(iP, opearteId, serverTime, inputNo);
                                }



                              }










                              #endregion
                    */




                    // P00003_DataEntity.freQuency = validateEffi.Rows[0][1].ToString();
                    //  P00003_DataEntity.effiEncy = (double.Parse(validateEffi.Rows[0][2].ToString()) * 100).ToString() + "%";
                    apiResult.data = P00003_DataEntity;

                    //int qbResultIn = P00003_Logic.Insert(partId, quantity, dock, kanbanOrderNo, kanbanSerial, scanTime, iP, serverTime,trolleyNo,sHF,inputNo);//插入实绩情报表
                    // int invResult = P00003_Logic.UpdateInv(partId,quantity,dock,kanbanOrderNo,kanbanSerial,scanTime,serverTime);                                                                                                                    //更新入出库履历表
                    //int oprReusultIn = P00003_Logic.InsertOpr(bzPlant, inputNo, kanbanOrderNo, kanbanSerial, partId, inoutFlag, supplier_id, supplierGQ, scanTime, serverTime, quantity, bZUnit, sHF, dock, checkType, labelStart, labelEnd,checkStatus);//插入作业实际表




                }

                else if (validateOpr.Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "品番" + partId + "在作业实绩表中没有对应数据或检查结果为NG,请检查!";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                else if (validateInv.Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "品番" + partId + "在入出库履历中没有对应数据,请检查!";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }













            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "包装失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }


            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
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

            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "获取包装数据失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

        }
        #endregion
    }

}
