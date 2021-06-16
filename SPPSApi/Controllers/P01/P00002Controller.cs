using System;
using System.Collections;
using System.Data;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using DataEntity;
using Microsoft.AspNetCore.StaticFiles;
using System.IO;

namespace SPPSApi.Controllers.P01
{
    [Route("api/P00002/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class P00002Controller : BaseController
    {
        P00002_Logic P00002_Logic = new P00002_Logic();
        P00002_DataEntity P00002_DataEntity = new P00002_DataEntity();
        private readonly string FunctionID = "P00002";
        private readonly IWebHostEnvironment _webHostEnvironment;


        #region 根据入库单号荷数量判断是否全部检查完毕

        [HttpPost]
        [EnableCors("any")]
        public string GetInnoApi([FromBody] dynamic data)
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
                string scanTime = dataForm.ScanTime == null ? "" : dataForm.ScanTime;//客户端时间
                DataTable validateOpr1 = P00001_Logic.ValidateOpr1(inno);
                DataTable validateOpr2 = P00001_Logic.ValidateOpr2(inno);
                if (validateOpr1.Rows.Count == 1 && validateOpr2.Rows.Count == 0)
                {
                    string partId = validateOpr1.Rows[0]["vcPart_id"].ToString();
                    string kanbanOrderNo = validateOpr1.Rows[0]["vcKBOrderNo"].ToString();
                    string kanbanSerial = validateOpr1.Rows[0]["vcKBLFNo"].ToString();
                    string dock = validateOpr1.Rows[0]["vcSR"].ToString();
                    string supplierId = validateOpr1.Rows[0]["vcSupplier_id"].ToString();
                    string quantity = validateOpr1.Rows[0]["iQuantity"].ToString();
                    DataTable getCheckType = P00002_Logic.GetCheckType(partId, kanbanOrderNo, kanbanSerial, dock, scanTime, supplierId);
                    DataTable getSPIS = P00002_Logic.GetSPIS(partId, scanTime, supplierId);
                    if (getCheckType.Rows.Count == 1 && getSPIS.Rows.Count == 1)
                    {
                        string checkType = getCheckType.Rows[0]["vcCheckP"].ToString();
                        string tjsx = getCheckType.Rows[0]["vcTJSX"].ToString();
                        P00002_DataEntity.checkType = checkType;
                        P00002_DataEntity.tjsx = tjsx;
                        P00002_DataEntity.partId = partId;
                        P00002_DataEntity.dock = dock;
                        P00002_DataEntity.kanbanOrderNo = kanbanOrderNo;
                        P00002_DataEntity.kanbanSerial = kanbanSerial;
                        P00002_DataEntity.quantity = quantity;
                        apiResult.data = P00002_DataEntity;
                    }
                    else if (getCheckType.Rows.Count != 1)
                    {

                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "品番" + partId + "在检查区分表中没有有效数据";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    else if (getSPIS.Rows.Count != 1)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "品番" + partId + "在检查法中没有有效数据";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                    }
                }
                else if (validateOpr1.Rows.Count != 1)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "入库单号" + inno + "没有进行入荷作业";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }
                else if (validateOpr2.Rows.Count > 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "入库单号" + inno + "已经进行检查作业";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, "");
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "获取入库信息失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        }
        #endregion

        #region 根据入库单号获得数据

        [HttpPost]
        [EnableCors("any")]
        public string GetInputInfoApi([FromBody] dynamic data)
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
                DataTable getInnoData = P00002_Logic.GetInnoData(inno);
                if (getInnoData.Rows.Count == 1)
                {
                    #region 构造返回数据
                    P00002_DataEntity.partId = getInnoData.Rows[0]["vcPart_id"].ToString();
                    P00002_DataEntity.dock = getInnoData.Rows[0]["vcSR"].ToString();
                    P00002_DataEntity.quantity = getInnoData.Rows[0]["iQuantity"].ToString();
                    P00002_DataEntity.kanbanOrderNo = getInnoData.Rows[0]["vcKBOrderNo"].ToString();
                    P00002_DataEntity.kanbanSerial = getInnoData.Rows[0]["vcKBLFNo"].ToString();
                    apiResult.data = P00002_DataEntity;
                    #endregion
                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "入库单号" + inno + "在作业实绩情报表中没有或有多条入库数据,请检查!";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, "");
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "获取品番信息失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        }
        #endregion

        #region  获得用户信息
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
                P00002_DataEntity.userName = userName;
                P00002_DataEntity.banZhi = banZhi;
                apiResult.data = P00002_DataEntity;
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, "");
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "获取用户信息失败!";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
        }
        #endregion

        #region  检查-看板扫描
        [HttpPost]
        [EnableCors("any")]
        public string GetCheckApi([FromBody] dynamic data)
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
                //1.从前台接数据
                string partId = dataForm.PartId == null ? "" : dataForm.PartId;//品番
                string quantity = dataForm.Quantity == null ? "" : dataForm.Quantity;//数量
                string dock = dataForm.Dock == null ? "" : dataForm.Dock;//受入
                string kanbanOrderNo = dataForm.KanbanOrderNo == null ? "" : dataForm.KanbanOrderNo;//看板订单号
                string kanbanSerial = dataForm.KanbanSerial == null ? "" : dataForm.KanbanSerial;//看板连番
                string scanTime = dataForm.ScanTime == null ? "" : dataForm.ScanTime;//客户端时间
                //2.必要录入字段的校验
                DataTable dtCheckInfo = P00002_Logic.getCheckInfo(partId, kanbanOrderNo, kanbanSerial, dock, scanTime);
                //2.1 是否已经入荷
                if (dtCheckInfo.Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "该看板未进行过入荷，请确认。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //2.2 是否已经经过检查
                string strS1_check = dtCheckInfo.Rows[0]["vcS1_check"].ToString();
                if (strS1_check.Trim().Length != 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "该看板已经进行过检查，请确认。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //2.3 是否有检查基础数据
                string strPart_check = dtCheckInfo.Rows[0]["vcPart_check"].ToString();
                if (strPart_check.Trim().Length == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "该看板品番检查基础数据缺失，请确认。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //2.4 是否有检查区分
                string strCheckP = dtCheckInfo.Rows[0]["vcCheckP"].ToString();
                if (strCheckP.Trim().Length == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "该看板品番检查频度缺失，请确认。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //2.5 在检查区分为【抽检】时是否有抽检个数
                string strSpotQty = dtCheckInfo.Rows[0]["iSpotQty"].ToString();
                if (strCheckP == "抽检" && strSpotQty == "0")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "该看板品番抽检个数缺失，请确认。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //2.6 是否存在SPIS照片
                string strPicUrl = dtCheckInfo.Rows[0]["vcPicUrl"].ToString();
                if (strPicUrl.Trim().Length == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "该看板品番检查SPIS缺失，请确认。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //3.向前台返回数据
                string strQuantity = dtCheckInfo.Rows[0]["iQuantity"].ToString();
                string strTJSX = dtCheckInfo.Rows[0]["vcTJSX"].ToString();

                P00002_DataEntity.quantity = strQuantity;
                P00002_DataEntity.tjsx = strTJSX;
                P00002_DataEntity.checkType = strCheckP;
                P00002_DataEntity.spispath = strPicUrl;
                apiResult.data = P00002_DataEntity;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, "");
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "获取检查类型失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 检查-NG页面初始化
        [HttpPost]
        [EnableCors("any")]
        public string GetNGInfoApi([FromBody] dynamic data)
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
                //1.获取NG信息及责任部署列表
                DataSet dsNgInfo = P00002_Logic.getNgReasonInfo();
                //2.检验必要信息
                if (dsNgInfo == null || dsNgInfo.Tables[0].Rows.Count == 0 || dsNgInfo.Tables[1].Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "NG情报或责任部署情报未维护，请联系管理员维护后再试。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                ArrayList ngReasonList = new ArrayList();
                ArrayList ngBlameList = new ArrayList();
                for (int i = 0; i < dsNgInfo.Tables[0].Rows.Count; i++)
                {
                    ngReasonList.Add(dsNgInfo.Tables[0].Rows[i][0].ToString());
                }
                for (int j = 0; j < dsNgInfo.Tables[1].Rows.Count; j++)
                {
                    ngBlameList.Add(dsNgInfo.Tables[1].Rows[j][0].ToString());
                }
                //2.向前台返回数据
                P00002_DataEntity.ngBlame = ngBlameList;
                P00002_DataEntity.ngReason = ngReasonList;
                apiResult.data = P00002_DataEntity;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, "");
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "获取NG信息列表失败!";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 检查-NG录入上传
        [HttpPost]
        [EnableCors("any")]
        public string SendNGDataApi([FromBody] dynamic data)
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
                //1.从前台接数据and定义变量
                string partId = dataForm.PartId == null ? "" : dataForm.PartId;//品番
                string quantity = dataForm.Quantity == null ? "" : dataForm.Quantity;//数量
                string dock = dataForm.Dock == null ? "" : dataForm.Dock;//受入
                string kanbanOrderNo = dataForm.KanbanOrderNo == null ? "" : dataForm.KanbanOrderNo;//看板订单号
                string kanbanSerial = dataForm.KanbanSerial == null ? "" : dataForm.KanbanSerial;//看板连番
                string scanTime = dataForm.ScanTime == null ? "" : dataForm.ScanTime;//客户端时间
                string serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
                string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");//客户端IP地址
                string value = dataForm.value == null ? "" : dataForm.value;//检查结果
                string ngBlame = dataForm.NGBlame == null ? "" : dataForm.NGBlame;
                string ngReason = dataForm.NGReason == null ? "" : dataForm.NGReason;
                string ngQuantity = dataForm.NGQuantity == null ? "" : dataForm.NGQuantity;
                //2.获取点位IP并获得点位名称
                if (iP == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "系统异常请联系管理员或退出后重新登录再试。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                DataTable getPoint = P00001_Logic.GetPointNo(iP);
                if (getPoint.Rows.Count != 1)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "当前点位信息缺失，请联系管理员处理。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string pointType = getPoint.Rows[0][0].ToString() + getPoint.Rows[0][1].ToString();
                //3.对于单条上传数据要在每次进行服务器交互时进行看板校验
                DataTable dtCheckInfo = P00002_Logic.getCheckInfo(partId, kanbanOrderNo, kanbanSerial, dock, scanTime);
                //3.1 是否已经入荷
                if (dtCheckInfo.Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "该看板未进行过入荷，请确认。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //3.2 是否已经经过检查
                string strS1_check = dtCheckInfo.Rows[0]["vcS1_check"].ToString();
                if (strS1_check.Trim().Length != 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "该看板已经进行过检查，请确认。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //3.3 是否有检查基础数据
                string strPart_check = dtCheckInfo.Rows[0]["vcPart_check"].ToString();
                if (strPart_check.Trim().Length == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "该看板品番检查基础数据缺失，请确认。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //3.4 是否有检查区分
                string strCheckP = dtCheckInfo.Rows[0]["vcCheckP"].ToString();
                if (strCheckP.Trim().Length == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "该看板品番检查频度缺失，请确认。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //3.5 在检查区分为【抽检】时是否有抽检个数
                string strSpotQty = dtCheckInfo.Rows[0]["iSpotQty"].ToString();
                if (strCheckP == "抽检" && strSpotQty == "0")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "该看板品番抽检个数缺失，请确认。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                else
                {
                    if (strCheckP == "全检")
                        strSpotQty = quantity;
                    else
                        strSpotQty = "0";
                }
                //4.收集上传数据
                DataSet dsInfo_Temp = P00002_Logic.getTableInfoFromDB();
                DataTable dtInfo_SJ_Temp = dsInfo_Temp.Tables[0].Clone();
                DataTable dtInfo_NG_Temp = dsInfo_Temp.Tables[1].Clone();
                string strQuantity = dtCheckInfo.Rows[0]["iQuantity"].ToString();//总数量
                string packingSpot = dtCheckInfo.Rows[0]["vcBZPlant"].ToString();//包装场
                string inputNo = dtCheckInfo.Rows[0]["vcInputNo"].ToString();//入库单号
                string inOutFlag = dtCheckInfo.Rows[0]["vcIOType"].ToString();//内外区分
                string supplierId = dtCheckInfo.Rows[0]["vcSupplier_id"].ToString();//供应商代码
                string supplierPlant = dtCheckInfo.Rows[0]["vcSupplierGQ"].ToString();//供应商工区
                string packingQuantity = dtCheckInfo.Rows[0]["vcBZUnit"].ToString();//包装单位
                string cpdCompany = dtCheckInfo.Rows[0]["vcSHF"].ToString();//收货方
                string lblStart = dtCheckInfo.Rows[0]["vcLabelStart"].ToString();//标签开始
                string lblEnd = dtCheckInfo.Rows[0]["vcLabelEnd"].ToString();//标签结束

                #region addrows
                DataRow drInfo_SJ_Temp = dtInfo_SJ_Temp.NewRow();
                drInfo_SJ_Temp["vcZYType"] = "S1";
                drInfo_SJ_Temp["vcBZPlant"] = packingSpot;
                drInfo_SJ_Temp["vcInputNo"] = inputNo;
                drInfo_SJ_Temp["vcKBOrderNo"] = kanbanOrderNo;
                drInfo_SJ_Temp["vcKBLFNo"] = kanbanSerial;
                drInfo_SJ_Temp["vcPart_id"] = partId;
                drInfo_SJ_Temp["vcIOType"] = inOutFlag;
                drInfo_SJ_Temp["vcSupplier_id"] = supplierId;
                drInfo_SJ_Temp["vcSupplierGQ"] = supplierPlant;
                drInfo_SJ_Temp["dStart"] = scanTime;
                drInfo_SJ_Temp["dEnd"] = serverTime;
                drInfo_SJ_Temp["iQuantity"] = strQuantity;
                drInfo_SJ_Temp["vcBZUnit"] = packingQuantity;
                drInfo_SJ_Temp["vcSHF"] = cpdCompany;
                drInfo_SJ_Temp["vcSR"] = dock;
                drInfo_SJ_Temp["vcBoxNo"] = "";
                drInfo_SJ_Temp["vcSheBeiNo"] = pointType;
                drInfo_SJ_Temp["vcCheckType"] = strCheckP;
                drInfo_SJ_Temp["iCheckNum"] = strSpotQty;
                drInfo_SJ_Temp["vcCheckStatus"] = value;
                drInfo_SJ_Temp["vcLabelStart"] = lblStart;
                drInfo_SJ_Temp["vcLabelEnd"] = lblEnd;
                drInfo_SJ_Temp["vcUnlocker"] = "";
                drInfo_SJ_Temp["dUnlockTime"] = "";
                drInfo_SJ_Temp["vcSellNo"] = "";
                drInfo_SJ_Temp["vcOperatorID"] = opearteId;
                drInfo_SJ_Temp["dOperatorTime"] = System.DateTime.Now.ToString("yyyy-MM-dd");
                drInfo_SJ_Temp["vcHostIp"] = iP;
                drInfo_SJ_Temp["packingcondition"] = "0";
                drInfo_SJ_Temp["vcPackingPlant"] = "";
                dtInfo_SJ_Temp.Rows.Add(drInfo_SJ_Temp);
                #endregion

                #region addrows
                DataRow drInfo_NG_Temp = dtInfo_NG_Temp.NewRow();
                drInfo_NG_Temp["vcPart_id"] = partId;
                drInfo_NG_Temp["vcKBOrderNo"] = kanbanOrderNo;
                drInfo_NG_Temp["vcKBLFNo"] = kanbanSerial;
                drInfo_NG_Temp["vcSR"] = dock;
                drInfo_NG_Temp["iNGQuantity"] = ngQuantity;
                drInfo_NG_Temp["vcNGReason"] = ngReason;
                drInfo_NG_Temp["vcZRBS"] = ngBlame;
                drInfo_NG_Temp["vcOperatorID"] = opearteId;
                drInfo_NG_Temp["dOperatorTime"] = System.DateTime.Now.ToString("yyyy-MM-dd");
                dtInfo_NG_Temp.Rows.Add(drInfo_NG_Temp);
                #endregion
                //5.写入数据库
                bool bResult = P00002_Logic.setCheckInfo(dtInfo_SJ_Temp, dtInfo_NG_Temp);
                if (bResult)
                {
                    P00002_DataEntity.result = "NG操作成功";
                    apiResult.data = P00002_DataEntity;
                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "NG操作失败";
                }
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                //ComMessage.GetInstance().ProcessMessage(FunctionID, "", ex, "system");
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "NG操作失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 检查-OK上传
        [HttpPost]
        [EnableCors("any")]
        public string ValidateApi([FromBody] dynamic data)
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
                //1.从前台接数据and定义变量
                string partId = dataForm.PartId == null ? "" : dataForm.PartId;//品番
                string quantity = dataForm.Quantity == null ? "" : dataForm.Quantity;//数量
                string dock = dataForm.Dock == null ? "" : dataForm.Dock;//受入
                string kanbanOrderNo = dataForm.KanbanOrderNo == null ? "" : dataForm.KanbanOrderNo;//看板订单号
                string kanbanSerial = dataForm.KanbanSerial == null ? "" : dataForm.KanbanSerial;//看板连番
                string scanTime = dataForm.ScanTime == null ? "" : dataForm.ScanTime;//客户端时间
                string serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
                string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");//客户端IP地址
                string value = dataForm.value == null ? "" : dataForm.value;//检查结果
                //2.获取点位IP并获得点位名称
                if (iP == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "系统异常请联系管理员或退出后重新登录再试。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                DataTable getPoint = P00001_Logic.GetPointNo(iP);
                if (getPoint.Rows.Count != 1)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "当前点位信息缺失，请联系管理员处理。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string pointType = getPoint.Rows[0][0].ToString() + getPoint.Rows[0][1].ToString();
                //3.对于单条上传数据要在每次进行服务器交互时进行看板校验
                DataTable dtCheckInfo = P00002_Logic.getCheckInfo(partId, kanbanOrderNo, kanbanSerial, dock, scanTime);
                //3.1 是否已经入荷
                if (dtCheckInfo.Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "该看板未进行过入荷，请确认。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //3.2 是否已经经过检查
                string strS1_check = dtCheckInfo.Rows[0]["vcS1_check"].ToString();
                if (strS1_check.Trim().Length != 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "该看板已经进行过检查，请确认。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //3.3 是否有检查基础数据
                string strPart_check = dtCheckInfo.Rows[0]["vcPart_check"].ToString();
                if (strPart_check.Trim().Length == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "该看板品番检查基础数据缺失，请确认。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //3.4 是否有检查区分
                string strCheckP = dtCheckInfo.Rows[0]["vcCheckP"].ToString();
                if (strCheckP.Trim().Length == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "该看板品番检查频度缺失，请确认。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //3.5 在检查区分为【抽检】时是否有抽检个数
                string strSpotQty = dtCheckInfo.Rows[0]["iSpotQty"].ToString();
                if (strCheckP == "抽检" && strSpotQty == "0")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "该看板品番抽检个数缺失，请确认。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                else
                {
                    if (strCheckP == "全检")
                        strSpotQty = quantity;
                    else
                        strSpotQty = "0";
                }
                //4.收集上传数据
                DataSet dsInfo_Temp = P00002_Logic.getTableInfoFromDB();
                DataTable dtInfo_SJ_Temp = dsInfo_Temp.Tables[0].Clone();
                DataTable dtInfo_NG_Temp = dsInfo_Temp.Tables[1].Clone();
                string strQuantity = dtCheckInfo.Rows[0]["iQuantity"].ToString();//总数量
                string packingSpot = dtCheckInfo.Rows[0]["vcBZPlant"].ToString();//包装场
                string inputNo = dtCheckInfo.Rows[0]["vcInputNo"].ToString();//入库单号
                string inOutFlag = dtCheckInfo.Rows[0]["vcIOType"].ToString();//内外区分
                string supplierId = dtCheckInfo.Rows[0]["vcSupplier_id"].ToString();//供应商代码
                string supplierPlant = dtCheckInfo.Rows[0]["vcSupplierGQ"].ToString();//供应商工区
                string packingQuantity = dtCheckInfo.Rows[0]["vcBZUnit"].ToString();//包装单位
                string cpdCompany = dtCheckInfo.Rows[0]["vcSHF"].ToString();//收货方
                string lblStart = dtCheckInfo.Rows[0]["vcLabelStart"].ToString();//标签开始
                string lblEnd = dtCheckInfo.Rows[0]["vcLabelEnd"].ToString();//标签结束

                #region addrows
                DataRow drInfo_SJ_Temp = dtInfo_SJ_Temp.NewRow();
                drInfo_SJ_Temp["vcZYType"] = "S1";
                drInfo_SJ_Temp["vcBZPlant"] = packingSpot;
                drInfo_SJ_Temp["vcInputNo"] = inputNo;
                drInfo_SJ_Temp["vcKBOrderNo"] = kanbanOrderNo;
                drInfo_SJ_Temp["vcKBLFNo"] = kanbanSerial;
                drInfo_SJ_Temp["vcPart_id"] = partId;
                drInfo_SJ_Temp["vcIOType"] = inOutFlag;
                drInfo_SJ_Temp["vcSupplier_id"] = supplierId;
                drInfo_SJ_Temp["vcSupplierGQ"] = supplierPlant;
                drInfo_SJ_Temp["dStart"] = scanTime;
                drInfo_SJ_Temp["dEnd"] = serverTime;
                drInfo_SJ_Temp["iQuantity"] = strQuantity;
                drInfo_SJ_Temp["vcBZUnit"] = packingQuantity;
                drInfo_SJ_Temp["vcSHF"] = cpdCompany;
                drInfo_SJ_Temp["vcSR"] = dock;
                drInfo_SJ_Temp["vcBoxNo"] = "";
                drInfo_SJ_Temp["vcSheBeiNo"] = pointType;
                drInfo_SJ_Temp["vcCheckType"] = strCheckP;
                drInfo_SJ_Temp["iCheckNum"] = strSpotQty;
                drInfo_SJ_Temp["vcCheckStatus"] = value;
                drInfo_SJ_Temp["vcLabelStart"] = lblStart;
                drInfo_SJ_Temp["vcLabelEnd"] = lblEnd;
                drInfo_SJ_Temp["vcUnlocker"] = "";
                drInfo_SJ_Temp["dUnlockTime"] = DBNull.Value;
                drInfo_SJ_Temp["vcSellNo"] = "";
                drInfo_SJ_Temp["vcOperatorID"] = opearteId;
                drInfo_SJ_Temp["dOperatorTime"] = System.DateTime.Now.ToString("yyyy-MM-dd");
                drInfo_SJ_Temp["vcHostIp"] = iP;
                drInfo_SJ_Temp["packingcondition"] = "0";
                drInfo_SJ_Temp["vcPackingPlant"] = "";
                dtInfo_SJ_Temp.Rows.Add(drInfo_SJ_Temp);
                #endregion

                //5.写入数据库
                bool bResult = P00002_Logic.setCheckInfo(dtInfo_SJ_Temp, dtInfo_NG_Temp);
                if (bResult)
                {
                    P00002_DataEntity.result = "OK操作成功";
                    apiResult.data = P00002_DataEntity;
                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "OK操作失败";
                }
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "OK操作失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 检查-SPIS绑定URL
        [HttpGet]
        [EnableCors("any")]
        public IActionResult getSPISImageList(string path)
        {
            try
            {
                string fileSavePath = _webHostEnvironment.ContentRootPath + Path.DirectorySeparatorChar + "Doc" + Path.DirectorySeparatorChar + "Image" + Path.DirectorySeparatorChar + "PackingOper" + Path.DirectorySeparatorChar;
                var provider = new FileExtensionContentTypeProvider();
                FileInfo fileInfo = new FileInfo(fileSavePath + path);
                var ext = fileInfo.Extension;
                new FileExtensionContentTypeProvider().Mappings.TryGetValue(ext, out var contenttype);
                byte[] bt = System.IO.File.ReadAllBytes(fileSavePath + path);
                return File(bt, contenttype ?? "image/Jpeg", fileInfo.Name);
            }
            catch (Exception ex)
            {
                ContentResult result = new ContentResult();
                result.Content = "<script>alert('导出失败,没有找到要导出的图片！')</script>";
                result.ContentType = "text/html;charset=utf-8";
                ComMessage.GetInstance().ProcessMessage("getImage", "M00UE0008", ex, "system");
                return result;
            }
        }
        #endregion


    }

}

