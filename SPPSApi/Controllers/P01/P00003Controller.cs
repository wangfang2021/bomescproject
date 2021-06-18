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

        #region 包装-登录到包装页面第二步-定时更新能率
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
                DataTable getCaseNo = P00003_Logic.getOperCaseNo(iP, strPointState,opearteId);
                if (getCaseNo.Rows.Count > 0)
                {
                    caseNo = getCaseNo.Rows[0]["vcCaseNo"].ToString();//全位
                    boxNo = getCaseNo.Rows[0]["vcBoxNo"].ToString(); //截位
                }
                DataTable dtCaseNoInfo = P00003_Logic.GetCaseNoInfo(caseNo);
                P00003_DataEntity.kanbanQuantity = dtCaseNoInfo.Rows[0]["kanbanQuantity"].ToString();
                apiResult.data = P00003_DataEntity;
                P00003_DataEntity.caseNo = caseNo;
                P00003_DataEntity.boxNo = boxNo;
                P00003_DataEntity.userName = userName;
                P00003_DataEntity.banZhi = banZhi;
                apiResult.data = P00003_DataEntity;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "获取用户信息失败!";
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
                //0.检验IP所属点位信息
                DataTable getPoint = P00001_Logic.GetPointNo(iP);
                if (getPoint.Rows.Count != 1)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "当前点位信息异常，请检查！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string pointType = getPoint.Rows[0][0].ToString() + getPoint.Rows[0][1].ToString();
                DataTable dtPrintName = P00001_Logic.GetPrintName(iP);
                string strPrinterName = "";
                if (dtPrintName.Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "该点位标签打印机未进行设置，请设置后重试。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                strPrinterName = dtPrintName.Rows[0]["vcPrinterName"].ToString();
                //1.获取看板信息
                DataTable dtKanBanInfo = P00003_Logic.GetKanBanInfo(partId, dock, kanbanOrderNo, kanbanSerial, scanTime);
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
                    if (strCheckStatus != "OK")
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "该看板未进行检查操作，请检查后再试。";
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
                //3.向前台返回必要数据
                P00003_DataEntity.packParts = dtPackList;//包材list
                P00003_DataEntity.caseQuantity = iQuantity_Fzx.ToString();//待装箱数
                P00003_DataEntity.packQuantity = iQuantity_Fzx.ToString();//待包装数
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
                P00003_DataEntity.kanbanQuantity = dtCaseNoInfo.Rows[0]["kanbanQuantity"].ToString();

                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "验证看板信息失败";
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

        #region  包装-包装装箱操作
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
                }
                DataTable dtPackWork = P00003_Logic.getPackInfo(partId, kanbanOrderNo, kanbanSerial, dock, packQuantity);
                P00003_Logic.setPackAndZxInfo(iP, pointType, strType, partId, kanbanOrderNo, kanbanSerial, dock, packQuantity, caseNo, boxno, scanTime, dtPackWork, opearteId);

                DataTable dtCaseNoInfo = P00003_Logic.GetCaseNoInfo(caseNo);
                P00003_DataEntity.kanbanQuantity = dtCaseNoInfo.Rows[0]["kanbanQuantity"].ToString();
                apiResult.data = P00003_DataEntity;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                #region  作废代码
                /*
                if (packQuantity == 0)

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
                    validateOpr1 = P00003_Logic.ValidateOpr2(partId, quantity, dock, kanbanOrderNo, kanbanSerial);                                                                                                //从作业实际取入荷数据
                    validateOpr = P00003_Logic.ValidateOpr1(partId, quantity, dock, kanbanOrderNo, kanbanSerial);
                    //从入出库履历取入荷数据
                    validateInv = P00003_Logic.ValidateInv(partId, quantity, dock, kanbanOrderNo, kanbanSerial);
                }
                else//其他类型的前工程为检查
                {
                    validateOpr1 = P00003_Logic.ValidateOpr3(partId, quantity, dock, kanbanOrderNo, kanbanSerial);                                                                                             //从作业实际取入荷数据
                    validateOpr = P00003_Logic.ValidateOpr(partId, quantity, dock, kanbanOrderNo, kanbanSerial);
                    //从入出库履历取入荷数据
                    validateInv = P00003_Logic.ValidateInv(partId, quantity, dock, kanbanOrderNo, kanbanSerial);
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
                        //更新作业履历 ==待包装==待装箱
                        int invResult = P00003_Logic.UpdateInv(partId, quantity, dock, kanbanOrderNo, kanbanSerial, scanTime, serverTime, opearteId);                                                                                                                    //更新入出库履历表
                        //插入包装实际表
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
                */
                #endregion
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "包装装箱失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

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

        #region  包装-包装不装箱操作
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
                string serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
                string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");//客户端IP地址
                //0.检验IP所属点位信息
                DataTable getPoint = P00001_Logic.GetPointNo(iP);
                if (getPoint.Rows.Count != 1)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "当前点位信息异常，请检查！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string pointType = getPoint.Rows[0][0].ToString() + getPoint.Rows[0][1].ToString();

                string checkType = dataForm.CheckType == null ? "" : dataForm.CheckType;//检查区分
                string caseNo = "";//箱号
                string formatDate = serverTime.Substring(0, 10).Replace("-", "");
                string packQuantity = quantity;//本次要包装数量
                //包装不装箱
                //1.插入作业实绩TOperateSJ
                //3.更新入出库履历TOperateSJ_InOutput
                //4.插入包材履历TPackWork
                //5.更新包材在库TPackZaiKu
                string strType = "包装不装箱";
                string boxno = caseNo;
                DataTable dtPackWork = P00003_Logic.getPackInfo(partId, kanbanOrderNo, kanbanSerial, dock, packQuantity);
                P00003_Logic.setPackAndZxInfo(iP, pointType, strType, partId, kanbanOrderNo, kanbanSerial, dock, packQuantity, caseNo, boxno, scanTime, dtPackWork, opearteId);

                DataTable dtCaseNoInfo = P00003_Logic.GetCaseNoInfo(caseNo);
                P00003_DataEntity.kanbanQuantity = dtCaseNoInfo.Rows[0]["kanbanQuantity"].ToString();
                apiResult.data = P00003_DataEntity;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                #region 作废代码
                /*
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
                    validateOpr = P00003_Logic.ValidateOpr1(partId, quantity, dock, kanbanOrderNo, kanbanSerial);
                    //从入出库履历取入荷数据
                    validateInv = P00003_Logic.ValidateInv(partId, quantity, dock, kanbanOrderNo, kanbanSerial);


                }
                else//其他类型的前工程为检查
                {
                    validateOpr = P00003_Logic.ValidateOpr(partId, quantity, dock, kanbanOrderNo, kanbanSerial);
                    //从入出库履历取入荷数据
                    validateInv = P00003_Logic.ValidateInv(partId, quantity, dock, kanbanOrderNo, kanbanSerial);
                }
                bool a = (validateOpr.Rows.Count == 1);
                bool b = (validateInv.Rows.Count == 1);
                bool c = (int.Parse(validateInv.Rows[0][4].ToString()) >= int.Parse(quantity));

                if (validateOpr.Rows.Count == 1 && validateInv.Rows.Count == 1
                  && (int.Parse(validateInv.Rows[0][3].ToString()) >= int.Parse(quantity)))
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
                    apiResult.data = P00003_DataEntity;
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
                */
                #endregion
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "包装不装箱失败";
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
