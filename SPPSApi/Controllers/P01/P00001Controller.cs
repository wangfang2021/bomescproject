using System;
using System.Data;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DataEntity;
using System.Text.RegularExpressions;


namespace SPPSApi.Controllers.P01
{
    [Route("api/P00001/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class P00001Controller : BaseController
    {

        P00001_DataEntity p00001_DataEntity = new P00001_DataEntity();
        private readonly string FunctionID = "P00001";
        private readonly IWebHostEnvironment _webHostEnvironment;
        ComFunction comFunction = new ComFunction();

        public P00001Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;

        }


        #region 获取权限
        [HttpPost]
        [EnableCors("any")]
        public string GetRoleInfoApi([FromBody] dynamic data)
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
                DataTable getUserRole = P00001_Logic.GetUserRole(opearteId);
                if (getUserRole.Rows.Count == 1)
                {
                    p00001_DataEntity.input = getUserRole.Rows[0]["vcInPut"].ToString();
                    p00001_DataEntity.check = getUserRole.Rows[0]["vcCheck"].ToString();
                    p00001_DataEntity.pack = getUserRole.Rows[0]["vcPack"].ToString();
                    p00001_DataEntity.output = getUserRole.Rows[0]["vcOutPut"].ToString();
                    apiResult.data = p00001_DataEntity;
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
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, "");
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "获取用户权限失败!";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }

            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        }
        #endregion

        #region  根据IP获取看板总数
        [HttpPost]
        [EnableCors("any")]
        public string GetKanBanSumApi([FromBody] dynamic data)
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
                DataTable getKanBanSum = P00001_Logic.GetKanBanSum(iP);
                p00001_DataEntity.kanbanSum = int.Parse(getKanBanSum.Rows[0]["sum"].ToString());
                apiResult.data = p00001_DataEntity;
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, "");
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "获取看板数量失败!";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }

            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        }
        #endregion

        #region 生成台车顺番号
        [HttpPost]
        [EnableCors("any")]
        public string GenLotId2Api([FromBody] dynamic data)
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
                string trolley = dataForm.Trolley == null ? "" : dataForm.Trolley;//台车号
                                                                                  //将台车号和台车顺番记录到数据库中
                string tmpString = "TROH2";
                string serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
                string formatServerTime = serverTime.Substring(0, 10).Replace("-", "");//格式化号口时间
                DataTable getTroSeq = P00001_Logic.setSeqNo(tmpString, 1, formatServerTime);
                string seqNo = getTroSeq.Rows[0][0].ToString().PadLeft(6, '0');//台车连番
                P00001_Logic.InsertTrolley1(seqNo, trolley, iP, opearteId, serverTime);
                p00001_DataEntity.trolleySeqNo = seqNo;
                apiResult.data = p00001_DataEntity;
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, "");
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "生成台车顺番号失败!";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }

            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        }
        #endregion

        #region 进入画面之后寻找最后一条未入库数据,如果有赋值
        [HttpPost]
        [EnableCors("any")]
        public string GetTrolletInfoApi([FromBody] dynamic data)
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
                //先找最近扫描的台车,如果被删除,都不显示,如果台车里面的看板被删除,保留台车号
                DataTable getTrolley = P00001_Logic.GetTrolley(iP);
                if (getTrolley.Rows.Count > 0)
                {
                    string staus = getTrolley.Rows[0]["vcStatus"].ToString();
                    string trolley = getTrolley.Rows[0]["vcTrolleyNo"].ToString();
                    string trolleySeqNo = getTrolley.Rows[0]["vcTrolleySeqNo"].ToString();
                    if (staus == "0")
                    {
                        DataTable getKanBan = P00001_Logic.GetKanBan(iP, trolley, trolleySeqNo);
                        if (getKanBan.Rows.Count > 0)
                        {
                            string kanbanStatus = getKanBan.Rows[0]["vcReflectFlag"].ToString();
                            DataTable getInfoData = P00001_Logic.GetInfoData(iP);
                            if (getInfoData.Rows.Count == 1)
                            {
                                if (kanbanStatus == "0")
                                {
                                    p00001_DataEntity.partId = getInfoData.Rows[0]["vcPart_id"].ToString();
                                    p00001_DataEntity.kanbanOrderNo = getInfoData.Rows[0]["vcKBOrderNo"].ToString();
                                    p00001_DataEntity.kanbanSerial = getInfoData.Rows[0]["vcKBLFNo"].ToString();
                                    p00001_DataEntity.quantity = getInfoData.Rows[0]["iQuantity"].ToString();
                                    p00001_DataEntity.dock = getInfoData.Rows[0]["vcSR"].ToString();
                                    p00001_DataEntity.trolley1 = getInfoData.Rows[0]["vcTrolleyNo"].ToString();
                                    p00001_DataEntity.trolleySeqNo = getInfoData.Rows[0]["vcTrolleySeqNo"].ToString();
                                    apiResult.data = p00001_DataEntity;
                                }
                                else if (kanbanStatus == "4")
                                {
                                    p00001_DataEntity.trolley1 = getInfoData.Rows[0]["vcTrolleyNo"].ToString();
                                    p00001_DataEntity.trolleySeqNo = getInfoData.Rows[0]["vcTrolleySeqNo"].ToString();
                                    apiResult.data = p00001_DataEntity;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, "");
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "获取看板明细失败!";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }

            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        }
        #endregion

        #region 系统登录（主页面）
        [HttpPost]
        [EnableCors("any")]
        public string ValidateLoginApi([FromBody] dynamic data)
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
                //ip插入位置
                string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");
                //验证独占登录
                if (pointtype != "PDA")
                {
                    DataTable dtPointState = P00001_Logic.GetPointState(opearteId);
                    if (dtPointState.Rows.Count > 0)
                    {
                        if (dtPointState.Rows[0]["vcPointIp"].ToString() != iP)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "账号已经在" + dtPointState.Rows[0]["vcPointType"].ToString() + dtPointState.Rows[0]["vcPointNo"].ToString() + "登录";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                    }
                }
                //验证打印机
                DataTable dtPrintName = P00001_Logic.checkPrintName(iP, pointtype);
                bool bCheckPrint = false;
                if (pointtype == "PDA")//扫描枪
                {
                    if (dtPrintName.Rows.Count != 2)
                        bCheckPrint = true;
                }
                if (pointtype == "COM")//一体机
                {
                    if (dtPrintName.Rows.Count != 2)
                        bCheckPrint = true;
                }
                if (pointtype == "PAD")//PAD
                {
                    if (dtPrintName.Rows.Count != 3)
                        bCheckPrint = true;
                }
                if (bCheckPrint)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "该设备绑定打印机有误，请联系管理员设置。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //以IP更新箱号
                P00001_Logic.setCaseState(iP);
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, "");
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "验证账号失败!";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 系统重新登录（所页面）
        [HttpPost]
        [EnableCors("any")]
        public string UpdateStatus3Api([FromBody] dynamic data)
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
                //更新登录状态
                //更新点位在线履历
                //更新箱号占用信息
                P00001_Logic.setSysExit(iP,"主页面、检查、出荷-重新登录");
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

        #region 登录到包装页面第一步-插入操作时间履历
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
                    string pointNo = getPoint.Rows[0]["vcPointNo"].ToString();
                    string date = getBanZhi.Rows[0]["dHosDate"].ToString();
                    string banZhi = getBanZhi.Rows[0]["vcBanZhi"].ToString();
                    P00001_Logic.InsertDetail(date, banZhi, pointNo, uuid, serverTime, opearteId);
                    DataTable getStatus = P00001_Logic.GetPointStatus4(pointNo);
                    if (getStatus.Rows.Count == 1)
                    {
                        P00001_Logic.UpdateStatus4(pointNo, opearteId);
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

        #region 获取包装单位
        [HttpPost]
        [EnableCors("any")]
        public string ValidateQuantity1Api([FromBody] dynamic data)
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
                string partId = dataForm.PartId == null ? "" : dataForm.PartId;//品番
                string dock = dataForm.Dock == null ? "" : dataForm.Dock;//受入                                                                        //ScanTime
                string scanTime = dataForm.ScanTime == null ? "" : dataForm.ScanTime;//客户端时间
                DataTable getQuantity = P00001_Logic.GetQuantity(partId, scanTime, dock);
                if (getQuantity.Rows.Count == 1)
                {
                    p00001_DataEntity.packingQuantity = getQuantity.Rows[0]["vcBZUnit"].ToString();
                    apiResult.data = p00001_DataEntity;

                }
                else
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "品番" + partId + "在包装基础数据中没有有效数据!";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, "");
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "获取包装单位失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }

            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        }
        #endregion





        #region 退出应用需要更改状态为未用
        [HttpPost]
        [EnableCors("any")]
        public string UpdateStatus1Api([FromBody] dynamic data)
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");
                DataTable getPoint = P00001_Logic.GetPoint2(iP);
                if (getPoint.Rows.Count == 1)
                {
                    string pointNo = getPoint.Rows[0]["vcPointNo"].ToString();
                    DataTable getStatus = P00001_Logic.GetPointStatus4(pointNo);
                    if (getStatus.Rows.Count == 1)
                    {
                        P00001_Logic.UpdateStatus3(pointNo);
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

        #region 登录后更新状态为正常
        [HttpPost]
        [EnableCors("any")]
        public string UpdateStatusApi([FromBody] dynamic data)
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
                DataTable getPointStatus = P00001_Logic.GetPointStatus1(opearteId, iP);
                if (getPointStatus.Rows.Count == 1)
                {
                    string pointNo = getPointStatus.Rows[0]["vcPointNo"].ToString();
                    P00001_Logic.UpdateStatus(opearteId, iP, pointNo);
                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "点位信息异常!";
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

        #region  进入系统之后更改点位表状态为未登录
        [HttpPost]
        [EnableCors("any")]
        public string ValidateUserApi1([FromBody] dynamic data)
        {

            ApiResult apiResult = new ApiResult();
            try
            {
                dynamic dataForm = JsonConvert.DeserializeObject(Convert.ToString(data));
                string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");//客户端IP地址
                                                                                                             //获取当前IP的点位号  获取当前IP的登录信息
                DataTable getPoint = P00001_Logic.GetPoint2(iP);
                if (getPoint.Rows.Count == 1)
                {
                    string pointNo = getPoint.Rows[0]["vcPointNo"].ToString();
                    DataTable getStatus = P00001_Logic.GetPointStatus4(pointNo);
                    if (getStatus.Rows.Count == 0)
                    {
                        int poiResultIn = P00001_Logic.InsertPoint1(pointNo);
                    }
                    else if (getStatus.Rows.Count == 1)
                    {
                        P00001_Logic.UpdatePoint1(pointNo);
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

        #region 获取台车号
        [HttpPost]
        [EnableCors("any")]
        public string GetTrolleyApi([FromBody] dynamic data)
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
                DataTable getTrolley = P00001_Logic.GetTrolley(opearteId, iP);
                if (getTrolley.Rows.Count > 0)
                {
                    string trolley = getTrolley.Rows[0]["vcTrolleyNo"].ToString();
                    DataTable validateQB = P00001_Logic.ValidateQB(trolley);
                    if (validateQB.Rows.Count > 0)
                    {
                        p00001_DataEntity.trolley1 = getTrolley.Rows[0]["vcTrolleyNo"].ToString();
                        p00001_DataEntity.lotId = getTrolley.Rows[0]["vcLotid"].ToString();
                        apiResult.data = p00001_DataEntity;
                    }
                }
                else
                {
                    p00001_DataEntity.delRsult = "clear";
                    apiResult.data = p00001_DataEntity;
                }
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "获取台车号失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        }
        #endregion

        #region  删除看板
        [HttpPost]
        [EnableCors("any")]
        public string DelKanbanApi([FromBody] dynamic data)
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
                string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");//客户端IP地址
                P00001_Logic.DeleteKanban(partId, kanbanOrderNo, kanbanSerial, dock, iP);
                DataTable getSeqNo = P00001_Logic.GetSeqNo1(iP, kanbanOrderNo, kanbanSerial, dock, partId);
                string trolleySeqNo1 = getSeqNo.Rows[0]["vcTrolleySeqNo"].ToString();
                string trolley1 = getSeqNo.Rows[0]["vcTrolleyNo"].ToString();

                DataTable getData = P00001_Logic.GetData(trolley1, trolleySeqNo1, iP);
                DataTable getData1 = P00001_Logic.GetData1(trolley1, trolleySeqNo1, iP);
                if (getData.Rows[0]["vcReflectFlag"].ToString() == "4" && int.Parse(getData1.Rows[0]["sum"].ToString()) > 0)
                {

                    //if()
                    DataTable getInfoData = P00001_Logic.GetInfoData(iP);
                    p00001_DataEntity.trolley1 = getInfoData.Rows[0]["vcTrolleyNo"].ToString();
                    p00001_DataEntity.trolleySeqNo = getInfoData.Rows[0]["vcTrolleySeqNo"].ToString();
                    apiResult.data = p00001_DataEntity;
                }
                else if (getData.Rows[0]["vcReflectFlag"].ToString() == "4" && int.Parse(getData1.Rows[0]["sum"].ToString()) == 0)
                {
                    DataTable getSeqNo1 = P00001_Logic.GetSeqNo2(iP, kanbanOrderNo, kanbanSerial, dock, partId);
                    string trolleySeqNo = getSeqNo1.Rows[0]["vcTrolleySeqNo"].ToString();
                    string trolley = getSeqNo1.Rows[0]["vcTrolleyNo"].ToString();
                    P00001_Logic.UpdateTrolley3(trolley, trolleySeqNo, iP);
                }
                else if (getData.Rows[0]["vcReflectFlag"].ToString() == "0")
                {
                    DataTable getInfoData = P00001_Logic.GetInfoData(iP);
                    p00001_DataEntity.partId = getInfoData.Rows[0]["vcPart_id"].ToString();
                    p00001_DataEntity.kanbanOrderNo = getInfoData.Rows[0]["vcKBOrderNo"].ToString();
                    p00001_DataEntity.kanbanSerial = getInfoData.Rows[0]["vcKBLFNo"].ToString();
                    p00001_DataEntity.quantity = getInfoData.Rows[0]["iQuantity"].ToString();
                    p00001_DataEntity.dock = getInfoData.Rows[0]["vcSR"].ToString();
                    p00001_DataEntity.trolley1 = getInfoData.Rows[0]["vcTrolleyNo"].ToString();
                    p00001_DataEntity.trolleySeqNo = getInfoData.Rows[0]["vcTrolleySeqNo"].ToString();
                    apiResult.data = p00001_DataEntity;
                }
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除看板失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }

            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        }







        #endregion

        #region 获得看板明细
        [HttpPost]
        [EnableCors("any")]
        public string GetTrolleyInfoApi([FromBody] dynamic data)
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
                string trolley = dataForm.Trolley == null ? "" : dataForm.Trolley;//台车号
                string lotId = dataForm.LotId == null ? "" : dataForm.LotId;//段取指示号
                string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");//客户端IP地址
                DataTable getTrolleyInfo = P00001_Logic.GetTrolleyInfo(trolley, iP, lotId);
                if (getTrolleyInfo.Rows.Count > 0)
                {
                    p00001_DataEntity.kanbanSum1 = getTrolleyInfo.Rows.Count.ToString();
                    p00001_DataEntity.kanban = getTrolleyInfo;
                    apiResult.data = p00001_DataEntity;
                }
                else
                {
                    p00001_DataEntity.kanban = null;
                    p00001_DataEntity.kanbanSum1 = "0";
                    apiResult.data = p00001_DataEntity;
                }
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "", ex, "system");
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "获取看板明细失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }

            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        }
        #endregion

        #region 生成段取指示号
        [HttpPost]
        [EnableCors("any")]
        public string GenLotIdApi([FromBody] dynamic data)
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
                                                                                                             //Trolley
                string trolley = dataForm.Trolley == null ? "" : dataForm.Trolley;//品番
                #region 在进行之前更新当前用户名的台车号状态
                //int trolleyResultUp1 = P00001_Logic.UpdateTrolley1(iP,opearteId);
                #endregion
                string tmpString = "PAC";
                string packingSpot = "H2";
                string serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
                string formatServerTime = serverTime.Substring(0, 10).Replace("-", "");//格式化号口时间
                DataTable pacSeqNodt = P00001_Logic.ValidateSeqNo(packingSpot, formatServerTime, tmpString);//判断连番表里是否有当天的段取指示书连番
                DataTable getTrolley = P00001_Logic.ValidateTrolley(trolley, opearteId, iP);
                if (pacSeqNodt.Rows.Count == 0)
                {
                    P00001_Logic.InsertSeqNo(packingSpot, formatServerTime, tmpString);//如果当天没有插入信息
                    pacSeqNodt = P00001_Logic.ValidateSeqNo(packingSpot, formatServerTime, tmpString);
                }
                string pacSeqNo = pacSeqNodt.Rows[0][2].ToString();//取得包材段取连番号
                string lotId = formatServerTime.Substring(2, 6).Trim() + "-" + packingSpot.Substring(1, 1).Trim() + "-" + pacSeqNo.PadLeft(5, '0').Trim();
                if (getTrolley.Rows.Count == 0)//如果没有进行插入
                {
                    P00001_Logic.InsertTrolley(trolley, opearteId, serverTime, iP, lotId);
                }
                else
                {
                    P00001_Logic.UpdateTrolley(trolley, opearteId, serverTime, iP, lotId);
                }
                int pacSeqNoNew = int.Parse(pacSeqNo) + 1;
                int pacResultUp = P00001_Logic.UpdateSeqNo(packingSpot, formatServerTime, pacSeqNoNew, tmpString);//更新连番表
                p00001_DataEntity.lotId = lotId;
                apiResult.data = p00001_DataEntity;
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "生成段取指示号失败!";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        }
        #endregion

        #region 验证重复台车
        [HttpPost]
        [EnableCors("any")]
        public string GenLotId1Api([FromBody] dynamic data)
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
                                                                                                             //Trolley
                string trolley = dataForm.Trolley == null ? "" : dataForm.Trolley;//品番
                DataTable GetTrolleyInfo = P00001_Logic.GetTrolleyInfo1(trolley, iP, opearteId);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "验证重复台车失败!";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }

            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        }
        #endregion

        #region  更新段取指示号
        [HttpPost]
        [EnableCors("any")]
        public string UpdateLotIdApi([FromBody] dynamic data)
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
                string trolley = dataForm.Trolley == null ? "" : dataForm.Trolley;//台车号
                DataTable getPackingSpot = P00001_Logic.GetPackingSpot(iP);
                if (getPackingSpot.Rows.Count == 1)
                {
                    string packingSpot = getPackingSpot.Rows[0]["vcBZPlant"].ToString();
                    string tmpString = "PAC";
                    string serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
                    string formatServerTime = serverTime.Substring(0, 10).Replace("-", "");//格式化号口时间
                    DataTable pacSeqNodt = P00001_Logic.ValidateSeqNo(packingSpot, formatServerTime, tmpString);//判断连番表里是否有当天的段取指示书连番
                    if (pacSeqNodt.Rows.Count == 0)
                    {
                        P00001_Logic.InsertSeqNo(packingSpot, formatServerTime, tmpString);//如果当天没有插入信息
                        pacSeqNodt = P00001_Logic.ValidateSeqNo(packingSpot, formatServerTime, tmpString);
                    }
                    string pacSeqNo = pacSeqNodt.Rows[0][2].ToString();//取得包材段取连番号
                    string lotId = formatServerTime.Substring(2, 6).Trim() + "-" + packingSpot.Substring(1, 1).Trim() + "-" + pacSeqNo.PadLeft(5, '0').Trim();
                    int pacSeqNoNew = int.Parse(pacSeqNo) + 1;
                    int pacResultUp = P00001_Logic.UpdateSeqNo(packingSpot, formatServerTime, pacSeqNoNew, tmpString);//更新连番表
                    P00001_Logic.UpdateQB(lotId, iP, trolley);
                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "包装场在实绩情报表不存在或存在多个包装场,请检查!";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "更新段取指示号失败!";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }

            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        }


        #endregion

        #region  删除台车
        [HttpPost]
        [EnableCors("any")]
        public string DelTrolleyApi([FromBody] dynamic data)
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
                string trolley = dataForm.Trolley == null ? "" : dataForm.Trolley;//台车号
                string lotId = dataForm.LotId == null ? "" : dataForm.LotId;//台车号
                string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");//客户端IP地址
                P00001_Logic.DeleteTrolley(trolley, iP, lotId);
                P00001_Logic.UpdateTrolley1(iP, opearteId, trolley, lotId);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除台车失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }

            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        }
        #endregion

        #region  获得数据
        [HttpPost]
        [EnableCors("any")]
        public string GetDataApi([FromBody] dynamic data)
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
                                                                                                             //DataTable getSum = P00001_Logic.GetSum(iP);
                DataTable getSum = P00001_Logic.GetSum(iP);
                if (getSum.Rows.Count > 0)
                {
                    int trolleySum = getSum.Rows.Count;
                    int kanbanSum = 0;
                    for (int i = 0; i < getSum.Rows.Count; i++)
                    {
                        string kanbanQuantity = getSum.Rows[i][1].ToString();
                        kanbanSum += int.Parse(kanbanQuantity);
                        p00001_DataEntity.trolleySum = trolleySum;
                        p00001_DataEntity.kanbanSum = kanbanSum;
                        p00001_DataEntity.trolley = getSum;
                        apiResult.data = p00001_DataEntity;
                    }
                }
                else
                {
                    p00001_DataEntity.trolley = null;
                    apiResult.data = p00001_DataEntity;
                }
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "获取数据失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        }
        #endregion

        #region  入荷-上传   
        [HttpPost]
        [EnableCors("any")]
        public string SendInputDataApi([FromBody] dynamic data)
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
                //1.从前台接数据and定义变量
                string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");//客户端IP地址
                string serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
                string formatServerTime = serverTime.Substring(0, 10).Replace("-", "");//格式化号口时间
                string tmpString = "PACH2";//断取连番
                string tmpString1 = "LBLH2";//标签连番
                string tmpString2 = "INOH2";//入库指令书
                                            //2.必要录入字段的校验
                                            //2.1 IP作为后续操作的依据不能为空
                if (iP == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "系统异常请联系管理员或退出后重新登录再试。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //2.2 检验IP所属点位信息
                //Console.WriteLine("IP:" + iP + " 操作:检验IP所属点位信息begin");
                DataTable getPoint = P00001_Logic.GetPointNo(iP);
                //Console.WriteLine("IP:" + iP + " 操作:检验IP所属点位信息end");
                if (getPoint.Rows.Count != 1)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "当前点位信息异常，请检查！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string pointType = getPoint.Rows[0][0].ToString() + getPoint.Rows[0][1].ToString();

                //3 获取本次要处理的待入荷数据DataSet(台车连番+明细)
                //t0:按照ip查询全部数据
                //t1:按照ip查询全部数据的台车连番
                //t2:按照ip查询全部数据标签总个数
                //t3：查询断取指示书表结构
                //t4：查询标签表结构
                //t5：查询订单表结构
                //t6:查询档次入库品番合计数量
                //t7:查询入库指令书表结构
                //DataTable getLotInfo = P00001_Logic.getInputInfoFromDB(iP, serverTime);
                //Console.WriteLine("IP:" + iP + " 操作:取本次要处理的待入荷数据begin");
                DataSet dsInPutQBInfo = P00001_Logic.getInputInfoFromDB(iP, serverTime);
                //Console.WriteLine("IP:" + iP + " 操作:取本次要处理的待入荷数据end");
                if (dsInPutQBInfo == null)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "没有可上传的数据请确认";
                    apiResult.type = "empty";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (dsInPutQBInfo.Tables[0].Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "没有可上传的数据请确认";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                DataTable dtPrintName = P00001_Logic.GetPrintName(iP);
                string strPrinterName = "";
                if (dtPrintName.Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "该点位标签打印机未进行设置，请设置后重试。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                strPrinterName = dtPrintName.Rows[0]["vcPrinterName"].ToString();
                #region //7 再次校验扫描时的数据正确性（应该先于3，4，5操作进行）
                //Console.WriteLine("IP:" + iP + " 操作:再次校验扫描时的数据正确性begin");
                for (int i = 0; i < dsInPutQBInfo.Tables[0].Rows.Count; i++)
                {
                    string partId = dsInPutQBInfo.Tables[0].Rows[i]["vcPart_id"].ToString();
                    string kanbanOrderNo = dsInPutQBInfo.Tables[0].Rows[i]["vcKBOrderNo"].ToString();
                    string kanbanSerial = dsInPutQBInfo.Tables[0].Rows[i]["vcKBLFNo"].ToString();
                    string dock = dsInPutQBInfo.Tables[0].Rows[i]["vcSR"].ToString();
                    DataSet dsCheckDb = P00001_Logic.getCheckQBandSJInfo(partId, kanbanOrderNo, kanbanSerial, dock, "H2", Convert.ToDateTime(serverTime).ToString("yyyy-MM-dd"), "SJ");
                    //7.1 扫描校验+校验TOperateSJ表是否存在
                    if (dsCheckDb.Tables[0].Rows.Count > 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "看板订单号" + kanbanOrderNo + "看板连番" + kanbanSerial + "品番" + partId + "已经入库";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (dsCheckDb.Tables[1].Rows.Count != 1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "品番" + partId + "在包装基础数据表中没有有效数据";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                    }
                    if (dsCheckDb.Tables[2].Rows.Count != 1 || dsCheckDb.Tables[3].Rows.Count != 1 || dsCheckDb.Tables[4].Rows.Count != 1)
                    {

                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "品番" + partId + "在品番基础数据表中没有有效数据";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (dsCheckDb.Tables[5].Rows.Count == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "品番" + partId + "在包材构成数据表中没有有效数据";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (dsCheckDb.Tables[6].Rows.Count == 0)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "品番" + partId + "在标签信息表中没有有效数据";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    if (dsCheckDb.Tables[9].Rows.Count != 1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "品番" + partId + "在价格中没有有效数据";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                //Console.WriteLine("IP:" + iP + " 操作:再次校验扫描时的数据正确性end");

                #endregion

                #region //4 生成段取信息
                //Console.WriteLine("IP:" + iP + " 操作:生成段取信息begin");
                //4.1 断取顺番更新==锁定顺番占用
                DataTable dtTroSeq = P00001_Logic.setSeqNo(tmpString, dsInPutQBInfo.Tables[1].Rows.Count, formatServerTime);
                //4.2 断取号生成并更新TOperatorQB（需要一起更新）
                string seqNoTrolley = dtTroSeq.Rows[0][0].ToString().PadLeft(5, '0');//断取连番
                P00001_Logic.setLotToOperatorQB(dsInPutQBInfo.Tables[1], iP, formatServerTime, seqNoTrolley);
                //Console.WriteLine("IP:" + iP + " 操作:生成段取信息end");
                #endregion

                #region //5 生成标签
                //5.1 标签顺番更新==锁定顺番占用
                DataTable dtTagSeq = P00001_Logic.setSeqNo(tmpString1, Convert.ToInt32(dsInPutQBInfo.Tables[2].Rows[0]["iTagNum"].ToString()), formatServerTime);
                //5.2 标签号生成并更新TOperatorQB（需要一起更新）
                string strTagSeqNo = dtTagSeq.Rows[0][0].ToString().PadLeft(5, '0');//标签连番
                P00001_Logic.setTagToOperatorQB(dsInPutQBInfo.Tables[0], iP, serverTime, strTagSeqNo);
                #endregion

                #region //6 生成指令书号
                //6.1 指令书号顺番更新==锁定顺番占用
                DataTable dtInvSeq = P00001_Logic.setSeqNo(tmpString2, dsInPutQBInfo.Tables[0].Rows.Count, formatServerTime);
                //6.2 指令书号生成并更新TOperatorQB（需要一起更新）
                string invSeqNo = dtInvSeq.Rows[0][0].ToString().PadLeft(5, '0');
                P00001_Logic.setInvToOperatorQB(dsInPutQBInfo.Tables[0], iP, serverTime, invSeqNo);
                #endregion

                #region //8.创建断取信息数据结构DataTable
                //8.1 查询
                DataTable dtPackList_Temp = dsInPutQBInfo.Tables[3].Clone();
                DataTable dtPackInfo = P00001_Logic.getPackInfo(iP, Convert.ToDateTime(serverTime).ToString("yyyy-MM-dd"));
                //8.2 往dtPackList_Temp中插入
                for (int j = 0; j < dtPackInfo.Rows.Count; j++)
                {
                    #region addrows
                    DataRow drPackList_Temp = dtPackList_Temp.NewRow();
                    drPackList_Temp["vcLotid"] = dtPackInfo.Rows[j]["vcLotid"].ToString();
                    //drPackList_Temp["iNo"] = dtPackInfo.Rows[j][""].ToString();
                    drPackList_Temp["vcPackingpartsno"] = dtPackInfo.Rows[j]["vcPackNo"].ToString();
                    //0613修改包材品番获取
                    drPackList_Temp["vcPackinggroup"] = dtPackInfo.Rows[j]["vcPackinggroup"].ToString();
                    drPackList_Temp["vcDistinguish"] = dtPackInfo.Rows[j]["vcDistinguish"].ToString();
                    drPackList_Temp["vcInno"] = dtPackInfo.Rows[j]["vcInno"].ToString();
                    drPackList_Temp["dQty"] = dtPackInfo.Rows[j]["dQty"].ToString();
                    drPackList_Temp["vcPackingpartslocation"] = dtPackInfo.Rows[j]["vcPackingpartslocation"].ToString();
                    //drPackList_Temp["dDaddtime"] = dtPackInfo.Rows[j]["dDaddtime"].ToString();
                    drPackList_Temp["vcPcname"] = dtPackInfo.Rows[j]["vcPcname"].ToString();
                    drPackList_Temp["vcHostip"] = iP;
                    drPackList_Temp["vcOperatorID"] = opearteId;
                    drPackList_Temp["dOperatorTime"] = System.DateTime.Now.ToString("yyyy-MM-dd");
                    drPackList_Temp["vcTrolleyNo"] = dtPackInfo.Rows[j]["vcTrolleyNo"].ToString();
                    //drPackList_Temp["dFirstPrintTime"] = dtPackInfo.Rows[j][""].ToString();
                    //drPackList_Temp["dLatelyPrintTime"] = dtPackInfo.Rows[j][""].ToString();
                    drPackList_Temp["vcLabelStart"] = dtPackInfo.Rows[j]["vcLabelStart"].ToString();
                    drPackList_Temp["vcLabelEnd"] = dtPackInfo.Rows[j]["vcLabelEnd"].ToString();
                    dtPackList_Temp.Rows.Add(drPackList_Temp);
                    #endregion
                }
                #endregion

                #region //9 创建标签信息数据结构DataTable
                //9.1 查询
                DataTable dtLabelList_Temp = dsInPutQBInfo.Tables[4].Clone();
                DataTable dtLabelInfo = P00001_Logic.getLabelInfo(iP, Convert.ToDateTime(serverTime).ToString("yyyy-MM-dd"));
                //9.2 往dtLabelList_Temp中插入
                for (int j = 0; j < dtLabelInfo.Rows.Count; j++)
                {
                    string strBZQF = dtLabelInfo.Rows[j]["vcBZQF"].ToString();
                    if (strBZQF != "1")
                    {
                        string lblStart = dtLabelInfo.Rows[j]["vcLabelStart"].ToString();
                        string lblEnd = dtLabelInfo.Rows[j]["vcLabelEnd"].ToString();
                        string partId = dtLabelInfo.Rows[j]["vcPart_id"].ToString();
                        string date = lblStart.Substring(0, 6);//标签首
                        int front = int.Parse(lblStart.Substring(6, 5));//开始
                        int back = int.Parse(lblEnd.Substring(6, 5));//结束
                        for (int a = front; a <= back; a++)
                        {
                            string strPrintcount = partId + date + a.ToString().PadLeft(5, '0');
                            string strLabel = "*" + strPrintcount + "*";
                            string strContent = "https://wx-m.ftms.com.cn/carowner/part?tabindex=3&tracingcode=" + strPrintcount;
                            string strPrintcount1 = partId + date + a.ToString().PadLeft(5, '0') + "B";
                            string strLabel1 = "*" + strPrintcount1 + "*";
                            string strContent1 = "https://wx-m.ftms.com.cn/carowner/part?tabindex=3&tracingcode=" + strPrintcount1 + "B";
                            byte[] Qrcode = P00001_Logic.GenerateQRCode(strContent);
                            byte[] Qrcode1 = P00001_Logic.GenerateQRCode(strContent1);
                            #region addrows
                            DataRow drLabelList_Temp = dtLabelList_Temp.NewRow();
                            drLabelList_Temp["vcPartsnameen"] = dtLabelInfo.Rows[j]["vcPartENName"].ToString();
                            drLabelList_Temp["vcPart_id"] = dtLabelInfo.Rows[j]["vcPart_id"].ToString();
                            drLabelList_Temp["vcPart_id1"] = dtLabelInfo.Rows[j]["vcPart_id1"].ToString();
                            drLabelList_Temp["vcInno"] = dtLabelInfo.Rows[j]["vcInputNo"].ToString();
                            drLabelList_Temp["vcCpdcompany"] = dtLabelInfo.Rows[j]["vcCpdCompany"].ToString();
                            drLabelList_Temp["vcLabel"] = strLabel;
                            drLabelList_Temp["vcLabel1"] = strLabel1;
                            drLabelList_Temp["vcGetnum"] = dtLabelInfo.Rows[j]["vcGetnum"].ToString();
                            drLabelList_Temp["iDateprintflg"] = dtLabelInfo.Rows[j]["iDateprintflg"].ToString();
                            drLabelList_Temp["vcComputernm"] = dtLabelInfo.Rows[j]["vcComputernm"].ToString();
                            drLabelList_Temp["vcPrindate"] = dtLabelInfo.Rows[j]["vcPrindate"].ToString();
                            drLabelList_Temp["iQrcode"] = Qrcode;
                            drLabelList_Temp["iQrcode1"] = Qrcode1;
                            drLabelList_Temp["vcPrintcount"] = strPrintcount;
                            drLabelList_Temp["vcPrintcount1"] = strPrintcount1;
                            drLabelList_Temp["vcPartnamechineese"] = dtLabelInfo.Rows[j]["vcPartnamechineese"].ToString();
                            drLabelList_Temp["vcSuppliername"] = dtLabelInfo.Rows[j]["vcSuppliername"].ToString();
                            drLabelList_Temp["vcSupplieraddress"] = dtLabelInfo.Rows[j]["vcSupplieraddress"].ToString();
                            drLabelList_Temp["vcExecutestandard"] = dtLabelInfo.Rows[j]["vcExecutestandard"].ToString();
                            drLabelList_Temp["vcCartype"] = dtLabelInfo.Rows[j]["vcCartype"].ToString();
                            drLabelList_Temp["vcHostip"] = iP;
                            drLabelList_Temp["vcOperatorID"] = opearteId;
                            //drLabelList_Temp["dOperatorTime"] = dtLabelInfo.Rows[j][""].ToString();
                            //drLabelList_Temp["dFirstPrintTime"] = dtLabelInfo.Rows[j][""].ToString();
                            //drLabelList_Temp["dLatelyPrintTime"] = dtLabelInfo.Rows[j][""].ToString();
                            dtLabelList_Temp.Rows.Add(drLabelList_Temp);
                            #endregion
                        }
                    }
                }
                #endregion

                #region //10 创建订单信息数据结构DataTable
                //10.1 查询
                DataTable dtOrder_Temp = dsInPutQBInfo.Tables[5].Clone();
                DataTable dtOrderInfo = P00001_Logic.getOrderInfo(iP, Convert.ToDateTime(serverTime).ToString("yyyy-MM-dd"));
                for (int i = 0; i < dsInPutQBInfo.Tables[6].Rows.Count; i++)
                {
                    string strPart_id = dsInPutQBInfo.Tables[6].Rows[i]["vcPart_id"].ToString();
                    string strCpdCompany = dsInPutQBInfo.Tables[6].Rows[i]["vcCpdCompany"].ToString();
                    string strSR = dsInPutQBInfo.Tables[6].Rows[i]["vcSR"].ToString();
                    int iSumQuantity = Convert.ToInt32(dsInPutQBInfo.Tables[6].Rows[i]["iSumQuantity"].ToString());
                    DataRow[] drOrderInfo = dtOrderInfo.Select("vcPartNo='" + strPart_id + "' and vcCpdcompany='" + strCpdCompany + "' and vcDock='" + strSR + "'");
                    DataTable dtOrderInfo_check = dtOrderInfo.Clone();
                    for (int j = 0; j < drOrderInfo.Length; j++)
                    {
                        dtOrderInfo_check.ImportRow(drOrderInfo[j]);
                    }
                    for (int j = 0; j < dtOrderInfo_check.Rows.Count; j++)
                    {
                        int inputQtyDailySum = 0;
                        #region 减订单
                        string iAutoId = dtOrderInfo_check.Rows[j]["iAutoId"].ToString();
                        string targetMonth = dtOrderInfo_check.Rows[j]["vcTargetYearMonth"].ToString();
                        string orderType = dtOrderInfo_check.Rows[j]["vcOrderType"].ToString();
                        string orderNo = dtOrderInfo_check.Rows[j]["vcOrderNo"].ToString();
                        string seqNo = dtOrderInfo_check.Rows[j]["vcSeqno"].ToString();
                        int d1 = int.Parse(dtOrderInfo_check.Rows[j]["day1"].ToString());
                        int d2 = int.Parse(dtOrderInfo_check.Rows[j]["day2"].ToString());
                        int d3 = int.Parse(dtOrderInfo_check.Rows[j]["day3"].ToString());
                        int d4 = int.Parse(dtOrderInfo_check.Rows[j]["day4"].ToString());
                        int d5 = int.Parse(dtOrderInfo_check.Rows[j]["day5"].ToString());
                        int d6 = int.Parse(dtOrderInfo_check.Rows[j]["day6"].ToString());
                        int d7 = int.Parse(dtOrderInfo_check.Rows[j]["day7"].ToString());
                        int d8 = int.Parse(dtOrderInfo_check.Rows[j]["day8"].ToString());
                        int d9 = int.Parse(dtOrderInfo_check.Rows[j]["day9"].ToString());
                        int d10 = int.Parse(dtOrderInfo_check.Rows[j]["day10"].ToString());
                        int d11 = int.Parse(dtOrderInfo_check.Rows[j]["day11"].ToString());
                        int d12 = int.Parse(dtOrderInfo_check.Rows[j]["day12"].ToString());
                        int d13 = int.Parse(dtOrderInfo_check.Rows[j]["day13"].ToString());
                        int d14 = int.Parse(dtOrderInfo_check.Rows[j]["day14"].ToString());
                        int d15 = int.Parse(dtOrderInfo_check.Rows[j]["day15"].ToString());
                        int d16 = int.Parse(dtOrderInfo_check.Rows[j]["day16"].ToString());
                        int d17 = int.Parse(dtOrderInfo_check.Rows[j]["day17"].ToString());
                        int d18 = int.Parse(dtOrderInfo_check.Rows[j]["day18"].ToString());
                        int d19 = int.Parse(dtOrderInfo_check.Rows[j]["day19"].ToString());
                        int d20 = int.Parse(dtOrderInfo_check.Rows[j]["day20"].ToString());
                        int d21 = int.Parse(dtOrderInfo_check.Rows[j]["day21"].ToString());
                        int d22 = int.Parse(dtOrderInfo_check.Rows[j]["day22"].ToString());
                        int d23 = int.Parse(dtOrderInfo_check.Rows[j]["day23"].ToString());
                        int d24 = int.Parse(dtOrderInfo_check.Rows[j]["day24"].ToString());
                        int d25 = int.Parse(dtOrderInfo_check.Rows[j]["day25"].ToString());
                        int d26 = int.Parse(dtOrderInfo_check.Rows[j]["day26"].ToString());
                        int d27 = int.Parse(dtOrderInfo_check.Rows[j]["day27"].ToString());
                        int d28 = int.Parse(dtOrderInfo_check.Rows[j]["day28"].ToString());
                        int d29 = int.Parse(dtOrderInfo_check.Rows[j]["day29"].ToString());
                        int d30 = int.Parse(dtOrderInfo_check.Rows[j]["day30"].ToString());
                        int d31 = int.Parse(dtOrderInfo_check.Rows[j]["day31"].ToString());
                        int[] array = {d1, d2, d3, d4, d5, d6, d7, d8, d9, d10, d11, d12, d13, d14, d15, d16, d17, d18, d19, d20, d21,
              d22,d23,d24,d25,d26,d27,d28,d29,d30,d31 };
                        int[] newarray = new int[31];
                        for (int l = 0; l < array.Length; l++)
                        {
                            if (iSumQuantity - array[l] > 0)
                            {
                                newarray[l] = array[l];
                                iSumQuantity = iSumQuantity - array[l];

                            }
                            else
                            {
                                newarray[l] = iSumQuantity;
                                iSumQuantity = 0;

                            }
                        }
                        #endregion
                        for (int k = 0; k < newarray.Length; k++)
                        {
                            inputQtyDailySum += newarray[k];
                        }
                        #region addrows
                        DataRow drOrder_Temp = dtOrder_Temp.NewRow();
                        drOrder_Temp["iAutoId"] = iAutoId;
                        drOrder_Temp["vcInputQtyDaily1"] = newarray[0];
                        drOrder_Temp["vcInputQtyDaily2"] = newarray[1];
                        drOrder_Temp["vcInputQtyDaily3"] = newarray[2];
                        drOrder_Temp["vcInputQtyDaily4"] = newarray[3];
                        drOrder_Temp["vcInputQtyDaily5"] = newarray[4];
                        drOrder_Temp["vcInputQtyDaily6"] = newarray[5];
                        drOrder_Temp["vcInputQtyDaily7"] = newarray[6];
                        drOrder_Temp["vcInputQtyDaily8"] = newarray[7];
                        drOrder_Temp["vcInputQtyDaily9"] = newarray[8];
                        drOrder_Temp["vcInputQtyDaily10"] = newarray[9];
                        drOrder_Temp["vcInputQtyDaily11"] = newarray[10];
                        drOrder_Temp["vcInputQtyDaily12"] = newarray[11];
                        drOrder_Temp["vcInputQtyDaily13"] = newarray[12];
                        drOrder_Temp["vcInputQtyDaily14"] = newarray[13];
                        drOrder_Temp["vcInputQtyDaily15"] = newarray[14];
                        drOrder_Temp["vcInputQtyDaily16"] = newarray[15];
                        drOrder_Temp["vcInputQtyDaily17"] = newarray[16];
                        drOrder_Temp["vcInputQtyDaily18"] = newarray[17];
                        drOrder_Temp["vcInputQtyDaily19"] = newarray[18];
                        drOrder_Temp["vcInputQtyDaily20"] = newarray[19];
                        drOrder_Temp["vcInputQtyDaily21"] = newarray[20];
                        drOrder_Temp["vcInputQtyDaily22"] = newarray[21];
                        drOrder_Temp["vcInputQtyDaily23"] = newarray[22];
                        drOrder_Temp["vcInputQtyDaily24"] = newarray[23];
                        drOrder_Temp["vcInputQtyDaily25"] = newarray[24];
                        drOrder_Temp["vcInputQtyDaily26"] = newarray[25];
                        drOrder_Temp["vcInputQtyDaily27"] = newarray[26];
                        drOrder_Temp["vcInputQtyDaily28"] = newarray[27];
                        drOrder_Temp["vcInputQtyDaily29"] = newarray[28];
                        drOrder_Temp["vcInputQtyDaily30"] = newarray[29];
                        drOrder_Temp["vcInputQtyDaily31"] = newarray[30];
                        drOrder_Temp["vcInputQtyDailySum"] = inputQtyDailySum;
                        dtOrder_Temp.Rows.Add(drOrder_Temp);
                        #endregion
                        if (iSumQuantity == 0)
                            break;
                    }
                }
                #endregion

                #region //11 创建入库指令书数据结构DataTable
                DataTable dtInv_Temp = dsInPutQBInfo.Tables[7].Clone();
                DataTable dtInvInfo = P00001_Logic.getInvInfo(iP, Convert.ToDateTime(serverTime).ToString("yyyy-MM-dd"));
                for (int i = 0; i < dsInPutQBInfo.Tables[0].Rows.Count; i++)
                {
                    string strPart_Id = dtInvInfo.Rows[i]["vcPart_id"].ToString();
                    string strInno = dtInvInfo.Rows[i]["vcInputNo"].ToString();
                    string strBZPlant = dtInvInfo.Rows[i]["vcBZPlant"].ToString();
                    string strPartsnamechn = dtInvInfo.Rows[i]["vcPartENName"].ToString();
                    string strCpdCompany = dtInvInfo.Rows[i]["vcCpdCompany"].ToString();
                    string strSR = dtInvInfo.Rows[i]["vcSR"].ToString();
                    string strQuantity = dtInvInfo.Rows[i]["iQuantity"].ToString();
                    string strBZUnit = dtInvInfo.Rows[i]["vcBZUnit"].ToString();
                    DataRow[] drPackInfo = dtPackInfo.Select("vcPart_id='" + strPart_Id + "' and vcCpdCompany='" + strCpdCompany + "' and vcSR='" + strSR + "'");
                    DataTable dtPackInfo_check = dtPackInfo.Clone();
                    for (int j = 0; j < drPackInfo.Length; j++)
                    {
                        dtPackInfo_check.ImportRow(drPackInfo[j]);
                    }
                    #region addrows
                    DataRow drInv_Temp = dtInv_Temp.NewRow();
                    drInv_Temp["vcNo"] = "";
                    drInv_Temp["vcData"] = "包装场" + strBZPlant;
                    drInv_Temp["vcPrintdate"] = System.DateTime.Now.ToString("yyyy-MM-dd");
                    drInv_Temp["vcInno"] = strInno;
                    drInv_Temp["vcPart_Id"] = strPart_Id;
                    drInv_Temp["vcPartsnamechn"] = strPartsnamechn;
                    drInv_Temp["vcPartslocation"] = "";
                    drInv_Temp["vcInputnum"] = strQuantity;
                    drInv_Temp["vcPackingquantity"] = strBZUnit;
                    for (int j = 0; j < dtPackInfo_check.Rows.Count; j++)
                    {
                        if (j <= 9)
                        {
                            string distinguish = dtPackInfo_check.Rows[j]["vcDistinguish"].ToString();
                            string packLocation = dtPackInfo_check.Rows[j]["vcPackLocation"].ToString();
                            string packNo = dtPackInfo_check.Rows[j]["vcPackNo"].ToString();
                            string quantity1 = dtPackInfo_check.Rows[j]["dQty"].ToString();
                            drInv_Temp[10 + (j * 4) + 0] = distinguish;
                            drInv_Temp[10 + (j * 4) + 1] = packLocation;
                            drInv_Temp[10 + (j * 4) + 2] = packNo;
                            drInv_Temp[10 + (j * 4) + 3] = quantity1;
                        }
                    }
                    string strPartsnoandnum = strPart_Id + strQuantity.PadLeft(5, '0') + strCpdCompany + strInno;
                    byte[] Qrcode = P00001_Logic.GenerateQRCode(strPartsnoandnum);
                    drInv_Temp["vcPartsnoandnum"] = strPartsnoandnum;
                    drInv_Temp["vcLabel"] = strPartsnoandnum;
                    drInv_Temp["vcComputernm"] = "";
                    drInv_Temp["vcCpdcompany"] = strCpdCompany;
                    drInv_Temp["vcPlantcode"] = "";
                    drInv_Temp["vcCompanyname"] = "";
                    drInv_Temp["vcPlantname"] = "";
                    drInv_Temp["iQrcode"] = Qrcode;
                    drInv_Temp["vcOperatorID"] = opearteId;
                    dtInv_Temp.Rows.Add(drInv_Temp);
                    #endregion
                }
                #endregion


                //12.以上数据收集完毕进行数据库更新插入(一个事wu)
                //插入作业实绩表--ok
                //更新作业实绩临时表--ok
                //插入作业履历表--ok
                //更新台车占用表--ok
                //插入断取信息表---UUID---更新首次打印时间--ok
                //插入标签信息表---UUID---更新首次打印时间--ok
                //消减订单表
                //插入指定书信息表---UUID---更新首次打印时间--ok

                //14.触发打印
                //插入打印临时表-断取
                //插入打印临时表-标签
                //插入打印临时表-指定书
                bool bResult = P00001_Logic.setInputInfo(iP, pointType, strPrinterName, dtPackList_Temp, dtLabelList_Temp, dtInv_Temp, dtOrder_Temp, opearteId);
                if (!bResult)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "数据上传失败，请联系管理员或者重新登录再试。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //13.以上12全部OK 构内XML发送

                #region 生成构内XML文件
                //if (inoutFlag == "1" && kanBan != "")
                //{
                //    DataTable getPointNo = P00001_Logic.GetPointNo(iP);
                //    if (getPointNo.Rows.Count == 1)
                //    {
                //        string name = getPointNo.Rows[0][1].ToString().PadLeft(5, '0');
                //        string formatTime = serverTime.Replace("-", "").Replace(":", "").Replace(" ", "");
                //        DataEntity.P00001_DataEntity.ScanData data2 = P00001_Logic.CutScanData(kanBan);
                //        P00001_Logic.SaveXml(data2, serverTime, name, formatTime);
                //    }
                //}
                #endregion

                p00001_DataEntity.result = "入库成功";
                apiResult.data = p00001_DataEntity;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "更新入库数据失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 入荷-扫描
        [HttpPost]
        [EnableCors("any")]
        public string ValidateInApi([FromBody] dynamic data)
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


                #region //1.从前台接数据
                string trolley = dataForm.Trolley == null ? "" : dataForm.Trolley;//台车号
                string partId = dataForm.PartId == null ? "" : dataForm.PartId;//品番
                string quantity1 = dataForm.Quantity == null ? "" : dataForm.Quantity;//数量
                string quantity = quantity1.PadLeft(5, '0');
                string dock = dataForm.Dock == null ? "" : dataForm.Dock;//受入
                string kanbanOrderNo = dataForm.KanbanOrderNo == null ? "" : dataForm.KanbanOrderNo;//看板订单号
                string kanbanSerial = dataForm.KanbanSerial == null ? "" : dataForm.KanbanSerial;//看板连番
                string scanTime = dataForm.ScanTime == null ? "" : dataForm.ScanTime;//客户端时间
                string serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
                string kanBan = dataForm.KanBan == null ? "" : dataForm.KanBan;//段取指示号
                string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");//客户端IP地址
                string trolleySeqNo = dataForm.TrolleySeqNo == null ? "" : dataForm.TrolleySeqNo;//台车顺番号
                string reg = @"^[0-9]+$";
                string reg1 = @"^[a-z0-9A-Z]+$";
                string packingSpot = "H2";
                #endregion

                #region //2.必要录入字段的校验
                //2.1台车号校验
                if (scanTime == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "系统异常请联系管理员或退出后重新登录再试。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (trolleySeqNo.Length != 6)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "台车号不符合规则请确认。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //2.2看板信息格式不正确校验
                bool bKanBan = partId.Length == 12
                    && Regex.IsMatch(partId, reg1)
                    && quantity.Length == 5
                    && Regex.IsMatch(quantity, reg)
                    && dock.Length == 2
                    && (kanbanOrderNo.Length == 10 || kanbanOrderNo.Length == 12)
                    && Regex.IsMatch(kanbanOrderNo, reg1)
                    && kanbanSerial.Length == 4
                    && Regex.IsMatch(kanbanSerial, reg);
                if (!bKanBan)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "看板信息有误，请确认看板是否正确。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                #endregion

                #region //3.进行DB数据验证获取
                //3.1 尽量减少数据库连接
                DataSet dsCheckDb = P00001_Logic.getCheckQBandSJInfo(partId, kanbanOrderNo, kanbanSerial, dock, packingSpot, scanTime, "QB");
                //dsCheckDb.Tables[0] 实绩情报表，需要为空  validateQB
                //dsCheckDb.Tables[1] 现场包装基础数据，需要唯一  validateQF
                //dsCheckDb.Tables[2]  手配基础数据，需要唯一 validateData
                //dsCheckDb.Tables[3]  手配基础数据，需要唯一 validateData1
                //dsCheckDb.Tables[4]  手配基础数据，需要唯一 getInOutFlag
                //dsCheckDb.Tables[5]  包材构成数据，不能为空 getPackItem
                //dsCheckDb.Tables[6]  标签信息表，需要唯一  getTagInfo
                //dsCheckDb.Tables[7]  订单表中的当前余量  validateOrd
                //dsCheckDb.Tables[8]  实绩情报表中的当前品番已经入库数量 getCount
                //dsCheckDb.Tables[9]  价格
                //0611
                if (dsCheckDb.Tables[0].Rows.Count > 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "看板订单号" + kanbanOrderNo + "看板连番" + kanbanSerial + "品番" + partId + "已经扫描";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (dsCheckDb.Tables[1].Rows.Count != 1)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "品番" + partId + "在包装基础数据表中没有有效数据";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }
                if (dsCheckDb.Tables[2].Rows.Count != 1 || dsCheckDb.Tables[3].Rows.Count != 1 || dsCheckDb.Tables[4].Rows.Count != 1)
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "品番" + partId + "在品番基础数据表中没有有效数据";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (dsCheckDb.Tables[5].Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "品番" + partId + "在包材构成数据表中没有有效数据";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (dsCheckDb.Tables[6].Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "品番" + partId + "在标签信息表中没有有效数据";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (dsCheckDb.Tables[9].Rows.Count != 1)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "品番" + partId + "在价格中没有有效数据";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                int sum = int.Parse(dsCheckDb.Tables[7].Rows[0]["sum"].ToString());//当前订单表中的余量
                int sum1 = int.Parse(dsCheckDb.Tables[8].Rows[0]["sum"].ToString());
                string sHF = dsCheckDb.Tables[2].Rows[0]["vcReceiver"].ToString();
                string inoutFlag = dsCheckDb.Tables[4].Rows[0]["vcInOut"].ToString();
                string bzQf = dsCheckDb.Tables[1].Rows[0]["vcBZQF"].ToString();//包装区分
                string rhQf = dsCheckDb.Tables[1].Rows[0]["vcRHQF"].ToString();//入荷区分
                string packingQuatity = dsCheckDb.Tables[1].Rows[0]["vcBZUnit"].ToString();//包装单位
                string packQuantity = dsCheckDb.Tables[3].Rows[0]["iPackingQty"].ToString();//收容数
                string supplierId = dsCheckDb.Tables[3].Rows[0]["vcSupplierId"].ToString();
                string supplierPlant = dsCheckDb.Tables[3].Rows[0]["vcSupplierPlant"].ToString();
                string cpdCompany = dsCheckDb.Tables[2].Rows[0]["vcReceiver"].ToString();//收货方
                string lblSart = "";
                string lblEnd = "";
                string inno = "";
                string tmpString = "INO";//临时字符串
                string tmpString1 = "LBL";
                string formatServerTime = serverTime.Substring(0, 10).Replace("-", "");//格式化号口时间

                #region 验证包材基础数据
                for (int j = 0; j < dsCheckDb.Tables[5].Rows.Count; j++)
                {
                    string packNo = dsCheckDb.Tables[5].Rows[j]["vcPackNo"].ToString();//包材品番
                    string nessNo = dsCheckDb.Tables[5].Rows[j]["iBiYao"].ToString();//必要数     
                    if (dsCheckDb.Tables[5].Rows[j]["vcPackLocation"].ToString() == "")
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "包材品番" + packNo + "在包材基础数据中没有有效数据,请检查!";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                #endregion
                if (sum - sum1 <= 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "品番" + partId + "在订单基础数据中待入库数小于当前数量,请检查!";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }

                if (int.Parse(packingQuatity) == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "品番" + partId + "在包装基础数据中不存在有效的包装单位,请检查!";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (rhQf != "1")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "品番" + partId + "在包装基础数据中为不可入荷,请检查!";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }
                if (int.Parse(quantity) % int.Parse(packingQuatity) != 0)
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "品番" + partId + "的收容数不是包装单位的整数倍，请检查!";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                #endregion
                P00001_Logic.Insert(trolley, partId, quantity, dock, kanbanOrderNo, kanbanSerial, scanTime, iP, serverTime, cpdCompany, inno, opearteId, packingSpot, packingQuatity, lblSart, lblEnd, supplierId, supplierPlant, trolleySeqNo, inoutFlag, kanBan);//插入实绩情报表
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }

            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "扫描入荷看板失败!";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
    }
}
#endregion
