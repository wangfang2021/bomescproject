using System;
using System.Data;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DataEntity;
using System.Linq;
using Microsoft.AspNetCore.Hosting;

namespace SPPSApi.Controllers.P01
{
    [Route("api/P00004/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class P00004Controller : BaseController
    {
        P00004_Logic P00004_Logic = new P00004_Logic();
        P00004_DataEntity P00004_DataEntity = new P00004_DataEntity();
        private readonly string FunctionID = "P00004";
        private readonly IWebHostEnvironment _webHostEnvironment;

        #region 出荷-初始化-尝试绑定Dock与叉车
        public string ValidateUserApi([FromBody] dynamic data)
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
                DataTable getDockInfo = P00004_Logic.GetDockInfo(opearteId);
                if (getDockInfo.Rows.Count == 1)
                {
                    P00004_DataEntity.fork = getDockInfo.Rows[0][0].ToString();
                    P00004_DataEntity.dockSell = getDockInfo.Rows[0][1].ToString();
                    P00004_DataEntity.getDockResult = "yes";
                    apiResult.data = P00004_DataEntity;

                }
                else if (getDockInfo.Rows.Count == 0)
                {
                    P00004_DataEntity.getDockResult = "no";
                    apiResult.data = P00004_DataEntity;
                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "当前用户绑定多个DOCK,请检查!";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "获取信息失败!";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }


            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

        }
        #endregion

        #region 出荷-绑定DOCK与叉车
        public string BindDockApi([FromBody] dynamic data)
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
                string dock = dataForm.Dock == null ? "" : dataForm.Dock;
                string fork = dataForm.Fork == null ? "" : dataForm.Fork;
                string scanTime = dataForm.ScanTime == null ? "" : dataForm.ScanTime;
                //vcFlag 0:绑定中  1：已解绑
                string strFlag = "0";
                string strType = "绑定";
                DataTable dtDockAndFork = P00004_Logic.getDockAndForkInfo(dock, fork, strFlag);
                if (dtDockAndFork.Rows.Count > 1)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "该叉车号绑定DOCK异常，请联系管理员处理。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (dtDockAndFork.Rows.Count == 1)
                {
                    string strDock = dtDockAndFork.Rows[0]["vcDockSell"].ToString();
                    if (strDock != fork)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "叉车号:" + fork + "已经绑定" + strDock + "";
                    }
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //插入新的叉车绑定关系
                P00004_Logic.setDockAndForkInfo(strType, dock, fork, strFlag, opearteId);
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, "system");
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "叉车与DOCK绑定失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 出荷-解绑DOCK与叉车
        public string DelBindApi([FromBody] dynamic data)
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
                string fork = dataForm.Fork == null ? "" : dataForm.Fork;
                string dock = dataForm.Dock == null ? "" : dataForm.Dock;
                string serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
                DataTable getData = P00004_Logic.GetData(dock, fork);
                //解绑之前需要判断当前叉车是否为dock的最后一辆
                DataSet dsCheckDock = P00004_Logic.checkDockAndForkInfo(dock, fork);
                if (dsCheckDock == null || dsCheckDock.Tables[0] == null || dsCheckDock.Tables[0].Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "该叉车号绑定异常，请联系管理员处理。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                DataTable dtDock = dsCheckDock.Tables[0];
                DataTable dtShip = dsCheckDock.Tables[1];
                if (dtDock.Rows.Count == 1 && dtShip.Rows.Count > 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "叉车号：" + dock + "为" + dock + "的最后一台叉车,禁止解绑。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string strFlag = "1";
                string strType = "解绑";
                P00004_Logic.setDockAndForkInfo(strType, dock, fork, strFlag, opearteId);
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "叉车与DOCK解绑失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region  出荷-查询箱号履历
        public string GetShipDataApi([FromBody] dynamic data)
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
                string dock = dataForm.Dock == null ? "" : dataForm.Dock;
                //以DOCK为单位查询箱号
                DataTable getShipData = P00004_Logic.getBoxList(dock, "");
                P00004_DataEntity.sum = getShipData.Rows.Count.ToString();
                P00004_DataEntity.caseNo = getShipData;
                apiResult.data = P00004_DataEntity;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "获取出货数据失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 出荷-删除箱号
        public string DelCaseApi([FromBody] dynamic data)
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
                string caseNo = dataForm.CaseNo == null ? "" : dataForm.CaseNo;
                P00004_Logic.delBoxList(caseNo, "", "");
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "", ex, "system");
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "删除箱号失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region  出荷-数据上传
        public string SendSellDataApi([FromBody] dynamic data)
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
                string truckNo = dataForm.TruckNo == null ? "" : dataForm.TruckNo;//卡车号
                string bian = dataForm.Bian == null ? "" : dataForm.Bian;//便次：成型--外注钣金
                string bPQuantity = dataForm.BPQuantity == null ? "" : dataForm.BPQuantity;//BP器具数
                string cBQuantity = dataForm.CBQuantity == null ? "" : dataForm.CBQuantity;//CB器具数
                string hUQuantity = dataForm.HUQuantity == null ? "" : dataForm.HUQuantity;//HU器具数
                string hUQuantity1 = dataForm.HUQuantity1 == null ? "" : dataForm.HUQuantity1;//2HU器具数
                string pCQuantity = dataForm.PCQuantity == null ? "" : dataForm.PCQuantity;//PC器具数
                string dockSell = dataForm.Dock == null ? "" : dataForm.Dock;//DOCK
                string qianFen = dataForm.QianFen == null ? "" : dataForm.QianFen;//铅封号
                string scanTime = dataForm.ScanTime == null ? "" : dataForm.ScanTime;//客户端时间
                string serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString();//服务端时间
                string iP = Request.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "");//客户端IP地址
                string strYinQuType = "";
                if (bian == "1")
                    strYinQuType = "cx";
                if (bian == "2")
                    strYinQuType = "wz";
                if (strYinQuType == "")
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "引取类型为空禁止操作，请检查！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //0.检验IP所属点位信息
                DataTable getPoint = P00001_Logic.GetPointNo(iP);
                if (getPoint.Rows.Count != 1)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "当前点位信息异常，请检查！";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                string pointType = getPoint.Rows[0][0].ToString() + getPoint.Rows[0][1].ToString();

                //1.验证DOCK中箱数与器具数一致
                DataTable dtBoxList = P00004_Logic.getBoxList(dockSell, "");
                int caseSum = int.Parse(bPQuantity) + int.Parse(cBQuantity) + int.Parse(hUQuantity) + int.Parse(hUQuantity1) + int.Parse(pCQuantity);
                if (dtBoxList.Rows.Count != caseSum)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "填写的器具数与DOCK总箱数不一致(箱数：" + dtBoxList.Rows.Count + "；器具数：" + caseSum + ")，请修改后重试。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                DataTable dtTime = P00004_Logic.getBanZhiTime(loginInfo.BaoZhuangPlace, "1");
                if (dtTime.Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "班值信息获取失败，请重试。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //2.对于DOCK下箱号中的部品进行必要的出荷验证
                string strFlag = "0";//未完成出荷的箱號
                DataSet dsDockInfo = P00004_Logic.getDockInfo(dockSell, "", strFlag, loginInfo.UnitCode);
                if (dsDockInfo == null || dsDockInfo.Tables[0].Rows.Count == 0 || dsDockInfo.Tables[1].Rows.Count == 0)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "装箱数据异常，确保出荷数据准确，请联系管理员处理后再试[A]。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                DataTable dtDockInfo = dsDockInfo.Tables[0];
                if (dtDockInfo.Rows.Count != dsDockInfo.Tables[1].Rows.Count)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "装箱数据异常，确保出荷数据准确，请联系管理员处理后再试[B]。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                for (int i = 0; i < dtDockInfo.Rows.Count; i++)
                {
                    string strForkNo = dtDockInfo.Rows[i]["vcForkNo"].ToString();
                    string strCaseNo = dtDockInfo.Rows[i]["vcCaseNo"].ToString();
                    string strBoxNo = dtDockInfo.Rows[i]["vcBoxNo"].ToString();
                    string strPart_id = dtDockInfo.Rows[i]["vcPart_id"].ToString();
                    //2.1   无有效的品番基础信息
                    string strPartENName = dtDockInfo.Rows[i]["vcPartENName"].ToString();
                    if (strPartENName == string.Empty)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "箱号：" + strBoxNo + "中的品番：" + strPart_id + "无有效的品番基础信息，请联系情报担当处理后再试。";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    //2.2   无有效的单价信息
                    string strPriceTNPWithTax = dtDockInfo.Rows[i]["decPriceTNPWithTax"].ToString();
                    if (strPriceTNPWithTax == string.Empty)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "箱号：" + strBoxNo + "中的品番：" + strPart_id + "无有效的单价信息，请联系情报担当处理后再试。";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    //2.3   未进行装箱处理
                    string strFlag_zx = dtDockInfo.Rows[i]["vcFlag_zx"].ToString();
                    if (strFlag_zx == string.Empty)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "箱号：" + strBoxNo + "中的品番：" + strPart_id + "未进行装箱处理，请装箱后后再试。";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    //2.4   已进行出荷处理
                    string strFlag_ck = dtDockInfo.Rows[i]["vcFlag_ck"].ToString();
                    if (strFlag_ck != string.Empty)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "箱号：" + strBoxNo + "已进行出荷处理，请删除箱号后再试。";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    //2.5   待出荷量不足
                    int iQuantity_bcck = Convert.ToInt32(dtDockInfo.Rows[i]["iQuantity_bcck"].ToString());
                    int iQuantity_dck = Convert.ToInt32(dtDockInfo.Rows[i]["iQuantity_dck"].ToString());
                    if (iQuantity_dck < iQuantity_bcck)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "箱号：" + strBoxNo + "中的品番：" + strPart_id + "待出荷量不足，请联系管理员后再试。";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                    //2.6   订单剩余量不足
                    int iQuantity_kck = Convert.ToInt32(dtDockInfo.Rows[i]["iQuantity_kck"].ToString());
                    if (iQuantity_kck < iQuantity_bcck)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "箱号：" + strBoxNo + "中的品番：" + strPart_id + "订单剩余量不足，请联系管理员后再试。";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }

                #region //2.7 本次出荷总量是否与订单总量匹配
                DataTable dtQuery = new DataTable();
                dtQuery.Columns.Add("vcDockSell");
                dtQuery.Columns.Add("vcSHF");
                dtQuery.Columns.Add("vcPart_id");
                dtQuery.Columns.Add("iQuantity_kck");
                dtQuery.Columns.Add("iQuantity_bcck_sum");
                var query = from t in dtDockInfo.AsEnumerable()
                            group t by new
                            {
                                t1 = t.Field<string>("vcPart_id"),
                                t2 = t.Field<int>("iQuantity_kck"),
                                t3 = t.Field<string>("vcDockSell"),
                                t4 = t.Field<string>("vcSHF")
                            } into m
                            select new
                            {
                                Part_id = m.Key.t1,
                                Quantity_kck = m.Key.t2,
                                DockSell = m.Key.t3,
                                SHF = m.Key.t4,
                                rowSum = m.Sum(m => m.Field<int>("iQuantity_bcck"))
                            };
                if (query.ToList().Count > 0)
                {
                    query.ToList().ForEach(q =>
                    {
                        DataRow drQuery = dtQuery.NewRow();
                        drQuery["vcDockSell"] = q.DockSell;
                        drQuery["vcSHF"] = q.SHF;
                        drQuery["vcPart_id"] = q.Part_id;
                        drQuery["iQuantity_kck"] = q.Quantity_kck;
                        drQuery["iQuantity_bcck_sum"] = q.rowSum;
                        dtQuery.Rows.Add(drQuery);
                    });
                }
                for (int i = 0; i < dtQuery.Rows.Count; i++)
                {
                    string strDockSell = dtQuery.Rows[i]["vcDockSell"].ToString();
                    string strPart_id = dtQuery.Rows[i]["vcPart_id"].ToString();
                    string strSHF = dtQuery.Rows[i]["vcSHF"].ToString();
                    int iQuantity_kck = Convert.ToInt32(dtQuery.Rows[i]["iQuantity_kck"].ToString());
                    int iQuantity_bcck_sum = Convert.ToInt32(dtQuery.Rows[i]["iQuantity_bcck_sum"].ToString());
                    if (iQuantity_bcck_sum > iQuantity_kck)
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "收货方：" + strSHF + "；DOCK：" + strDockSell + "中的品番：" + strPart_id + "出荷总量大于订单余量，请联系管理员后再试。";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                #endregion

                //3.插入到出荷临时表中
                //以DOCK进行清空
                //插入临时表
                P00004_Logic.setOutPut_Temp(dockSell, "", strFlag, loginInfo.UnitCode, iP, opearteId);
                //4.取出本次出荷临时表中的待出荷数据
                DataSet dsTableFromDB = P00004_Logic.getTableInfoFromDB();
                //5.进行数据整理，对于订单数据进行循环消减订单并进行出荷品番拆分
                //TOperateSJ
                //TOperateSJ_InOutput
                //SP_M_ORD
                //TSell
                //TShipList
                DataTable dtOperateSJ_Temp = dsTableFromDB.Tables[0].Clone();
                DataTable dtOperateSJ_InOutput_Temp = dsTableFromDB.Tables[1].Clone();
                DataTable dtShipList_Temp = dsTableFromDB.Tables[2].Clone();
                DataTable dtSell_Temp = dsTableFromDB.Tables[3].Clone();
                DataTable dtSell_Tool_Temp = dsTableFromDB.Tables[4].Clone();
                DataTable dtSell_Sum_Temp = dsTableFromDB.Tables[5].Clone();
                DataTable dtOrder_Temp = dsTableFromDB.Tables[6].Clone();
                //获取订单信息表
                DataTable dtOrderInfo = dsTableFromDB.Tables[7];
                foreach (DataRow drQuery in dtQuery.Rows)
                {
                    DataTable dtDockInfo_clone = dtDockInfo.Clone();
                    string strPart_id = drQuery["vcPart_id"].ToString();
                    string strSHF = drQuery["vcSHF"].ToString();
                    DataRow[] drDockInfo = dtDockInfo.Select("vcPart_id='" + strPart_id + "' and vcSHF='" + strSHF + "'");
                    for (int i = 0; i < drDockInfo.Length; i++)
                    {
                        dtDockInfo_clone.ImportRow(drDockInfo[i]);
                    }
                    DataRow[] drOrderInfo = dtOrderInfo.Select("vcPartNo='" + strPart_id + "' and vcCpdcompany='" + strSHF + "'");
                    DataTable dtOrderInfo_check = dtOrderInfo.Clone();
                    for (int j = 0; j < drOrderInfo.Length; j++)
                    {
                        dtOrderInfo_check.ImportRow(drOrderInfo[j]);
                    }
                    for (int i = 0; i < dtDockInfo_clone.Rows.Count; i++)
                    {
                        #region addrow-dtOperateSJ
                        DataRow drOperateSJ_Temp = dtOperateSJ_Temp.NewRow();
                        drOperateSJ_Temp["vcZYType"] = "S4";
                        drOperateSJ_Temp["vcBZPlant"] = dtDockInfo_clone.Rows[i]["vcBZPlant"].ToString();
                        drOperateSJ_Temp["vcInputNo"] = dtDockInfo_clone.Rows[i]["vcInputNo"].ToString();
                        drOperateSJ_Temp["vcKBOrderNo"] = dtDockInfo_clone.Rows[i]["vcKBOrderNo"].ToString();
                        drOperateSJ_Temp["vcKBLFNo"] = dtDockInfo_clone.Rows[i]["vcKBLFNo"].ToString();
                        drOperateSJ_Temp["vcPart_id"] = dtDockInfo_clone.Rows[i]["vcPart_id"].ToString();
                        drOperateSJ_Temp["vcIOType"] = dtDockInfo_clone.Rows[i]["vcIOType"].ToString();
                        drOperateSJ_Temp["vcSupplier_id"] = dtDockInfo_clone.Rows[i]["vcSupplier_id"].ToString();
                        drOperateSJ_Temp["vcSupplierGQ"] = dtDockInfo_clone.Rows[i]["vcSupplierGQ"].ToString();
                        drOperateSJ_Temp["dStart"] = scanTime;
                        //drOperateSJ_Temp["dEnd"] = dtBoxMasterInfo.Rows[i]["vcLotid"].ToString();
                        drOperateSJ_Temp["iQuantity"] = dtDockInfo_clone.Rows[i]["iQuantity_bcck"].ToString();
                        drOperateSJ_Temp["vcBZUnit"] = dtDockInfo_clone.Rows[i]["vcBZUnit"].ToString();
                        drOperateSJ_Temp["vcSHF"] = dtDockInfo_clone.Rows[i]["vcSHF"].ToString();
                        drOperateSJ_Temp["vcSR"] = dtDockInfo_clone.Rows[i]["vcSR"].ToString();
                        drOperateSJ_Temp["vcCaseNo"] = dtDockInfo_clone.Rows[i]["vcCaseNo"].ToString();
                        drOperateSJ_Temp["vcBoxNo"] = dtDockInfo_clone.Rows[i]["vcBoxNo"].ToString();
                        drOperateSJ_Temp["vcSheBeiNo"] = pointType;
                        drOperateSJ_Temp["vcCheckType"] = dtDockInfo_clone.Rows[i]["vcCheckType"].ToString();
                        drOperateSJ_Temp["iCheckNum"] = dtDockInfo_clone.Rows[i]["iCheckNum"].ToString();
                        drOperateSJ_Temp["vcCheckStatus"] = dtDockInfo_clone.Rows[i]["vcCheckStatus"].ToString();
                        drOperateSJ_Temp["vcLabelStart"] = dtDockInfo_clone.Rows[i]["vcLabelStart"].ToString();
                        drOperateSJ_Temp["vcLabelEnd"] = dtDockInfo_clone.Rows[i]["vcLabelEnd"].ToString();
                        //drOperateSJ_Temp["vcUnlocker"] = dtBoxMasterInfo.Rows[i]["vcLotid"].ToString();
                        //drOperateSJ_Temp["dUnlockTime"] = dtBoxMasterInfo.Rows[i]["vcLotid"].ToString();
                        //drOperateSJ_Temp["vcSellNo"] = dtBoxMasterInfo.Rows[i]["vcLotid"].ToString();
                        drOperateSJ_Temp["vcOperatorID"] = opearteId;
                        //drOperateSJ_Temp["dOperatorTime"] = dtBoxMasterInfo.Rows[i]["vcLotid"].ToString();
                        drOperateSJ_Temp["vcHostIp"] = iP;
                        drOperateSJ_Temp["packingcondition"] = dtDockInfo_clone.Rows[i]["packingcondition"].ToString();
                        drOperateSJ_Temp["vcPackingPlant"] = dtDockInfo_clone.Rows[i]["vcPackingPlant"].ToString();
                        dtOperateSJ_Temp.Rows.Add(drOperateSJ_Temp);
                        #endregion

                        #region addrow-dtOperateSJ_InOutput
                        DataRow drOperateSJ_InOutput_Temp = dtOperateSJ_InOutput_Temp.NewRow();
                        drOperateSJ_InOutput_Temp["vcBZPlant"] = dtDockInfo_clone.Rows[i]["vcBZPlant"].ToString();
                        drOperateSJ_InOutput_Temp["vcSHF"] = dtDockInfo_clone.Rows[i]["vcSHF"].ToString();
                        drOperateSJ_InOutput_Temp["vcInputNo"] = dtDockInfo_clone.Rows[i]["vcInputNo"].ToString();
                        drOperateSJ_InOutput_Temp["vcKBOrderNo"] = dtDockInfo_clone.Rows[i]["vcKBOrderNo"].ToString();
                        drOperateSJ_InOutput_Temp["vcKBLFNo"] = dtDockInfo_clone.Rows[i]["vcKBLFNo"].ToString();
                        drOperateSJ_InOutput_Temp["vcPart_id"] = dtDockInfo_clone.Rows[i]["vcPart_id"].ToString();
                        drOperateSJ_InOutput_Temp["vcSR"] = dtDockInfo_clone.Rows[i]["vcSR"].ToString();
                        //drOperateSJ_InOutput_Temp["iQuantity"] = dtDockInfo_clone.Rows[i][""].ToString();
                        //drOperateSJ_InOutput_Temp["iDBZ"] = dtDockInfo_clone.Rows[i][""].ToString();
                        //drOperateSJ_InOutput_Temp["iDZX"] = dtDockInfo_clone.Rows[i][""].ToString();
                        drOperateSJ_InOutput_Temp["iDCH"] = dtDockInfo_clone.Rows[i]["iQuantity_bcck"].ToString();
                        //drOperateSJ_InOutput_Temp["dInDate"] = dtDockInfo_clone.Rows[i][""].ToString();
                        drOperateSJ_InOutput_Temp["vcOperatorID"] = opearteId;
                        //drOperateSJ_InOutput_Temp["dOperatorTime"] = dtDockInfo_clone.Rows[i][""].ToString();
                        dtOperateSJ_InOutput_Temp.Rows.Add(drOperateSJ_InOutput_Temp);
                        #endregion

                        int iSumQuantity = Convert.ToInt32(dtDockInfo_clone.Rows[i]["iQuantity_bcck"].ToString());
                        for (int j = 0; j < dtOrderInfo_check.Rows.Count; j++)
                        {
                            int iResultQtyDailySum = 0;
                            string iAutoId = dtOrderInfo_check.Rows[j]["iAutoId"].ToString();
                            string targetMonth = dtOrderInfo_check.Rows[j]["vcTargetYearMonth"].ToString();
                            string orderType = dtOrderInfo_check.Rows[j]["vcOrderType"].ToString();
                            string orderNo = dtOrderInfo_check.Rows[j]["vcOrderNo"].ToString();
                            string seqNo = dtOrderInfo_check.Rows[j]["vcSeqno"].ToString();
                            #region 减订单
                            #region getorder
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
                            int[] array = { d1, d2, d3, d4, d5, d6, d7, d8, d9, d10, d11, d12, d13, d14, d15, d16, d17, d18, d19, d20, d21, d22, d23, d24, d25, d26, d27, d28, d29, d30, d31 };
                            int[] newarray = new int[31];
                            #endregion
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
                                dtOrderInfo_check.Rows[j][l + 7 + 1] = Convert.ToInt32(dtOrderInfo_check.Rows[j][l + 7 + 1]) - array[l];
                            }
                            #endregion
                            for (int k = 0; k < newarray.Length; k++)
                            {
                                iResultQtyDailySum += newarray[k];
                            }

                            #region addrows-dtOrder
                            DataRow drOrder_Temp = dtOrder_Temp.NewRow();
                            drOrder_Temp["iAutoId"] = iAutoId;
                            drOrder_Temp["vcResultQtyDaily1"] = newarray[0];
                            drOrder_Temp["vcResultQtyDaily2"] = newarray[1];
                            drOrder_Temp["vcResultQtyDaily3"] = newarray[2];
                            drOrder_Temp["vcResultQtyDaily4"] = newarray[3];
                            drOrder_Temp["vcResultQtyDaily5"] = newarray[4];
                            drOrder_Temp["vcResultQtyDaily6"] = newarray[5];
                            drOrder_Temp["vcResultQtyDaily7"] = newarray[6];
                            drOrder_Temp["vcResultQtyDaily8"] = newarray[7];
                            drOrder_Temp["vcResultQtyDaily9"] = newarray[8];
                            drOrder_Temp["vcResultQtyDaily10"] = newarray[9];
                            drOrder_Temp["vcResultQtyDaily11"] = newarray[10];
                            drOrder_Temp["vcResultQtyDaily12"] = newarray[11];
                            drOrder_Temp["vcResultQtyDaily13"] = newarray[12];
                            drOrder_Temp["vcResultQtyDaily14"] = newarray[13];
                            drOrder_Temp["vcResultQtyDaily15"] = newarray[14];
                            drOrder_Temp["vcResultQtyDaily16"] = newarray[15];
                            drOrder_Temp["vcResultQtyDaily17"] = newarray[16];
                            drOrder_Temp["vcResultQtyDaily18"] = newarray[17];
                            drOrder_Temp["vcResultQtyDaily19"] = newarray[18];
                            drOrder_Temp["vcResultQtyDaily20"] = newarray[19];
                            drOrder_Temp["vcResultQtyDaily21"] = newarray[20];
                            drOrder_Temp["vcResultQtyDaily22"] = newarray[21];
                            drOrder_Temp["vcResultQtyDaily23"] = newarray[22];
                            drOrder_Temp["vcResultQtyDaily24"] = newarray[23];
                            drOrder_Temp["vcResultQtyDaily25"] = newarray[24];
                            drOrder_Temp["vcResultQtyDaily26"] = newarray[25];
                            drOrder_Temp["vcResultQtyDaily27"] = newarray[26];
                            drOrder_Temp["vcResultQtyDaily28"] = newarray[27];
                            drOrder_Temp["vcResultQtyDaily29"] = newarray[28];
                            drOrder_Temp["vcResultQtyDaily30"] = newarray[29];
                            drOrder_Temp["vcResultQtyDaily31"] = newarray[30];
                            drOrder_Temp["vcResultQtyDailySum"] = iResultQtyDailySum;
                            dtOrder_Temp.Rows.Add(drOrder_Temp);
                            #endregion

                            #region addrow-dtSell
                            DataRow drSell_Temp = dtSell_Temp.NewRow();
                            //drSell_Temp["dHosDate"] = dtDockInfo_clone.Rows[i][""].ToString();
                            //drSell_Temp["vcBanZhi"] = dtDockInfo_clone.Rows[i][""].ToString();
                            //drSell_Temp["vcBianCi"] = dtDockInfo_clone.Rows[i][""].ToString();
                            //drSell_Temp["vcSellNo"] = dtDockInfo_clone.Rows[i][""].ToString();
                            drSell_Temp["vcTruckNo"] = truckNo;
                            drSell_Temp["vcSHF"] = dtDockInfo_clone.Rows[i]["vcSHF"].ToString();
                            drSell_Temp["vcPart_id"] = dtDockInfo_clone.Rows[i]["vcPart_id"].ToString();
                            drSell_Temp["vcOrderNo"] = orderNo;
                            drSell_Temp["vcLianFanNo"] = seqNo;
                            //drSell_Temp["vcInvoiceNo"] = dtDockInfo_clone.Rows[i][""].ToString();//截位销售号
                            drSell_Temp["vcCaseNo"] = dtDockInfo_clone.Rows[i]["vcCaseNo"].ToString();
                            drSell_Temp["vcBoxNo"] = dtDockInfo_clone.Rows[i]["vcBoxNo"].ToString();
                            drSell_Temp["vcPartsNameEN"] = dtDockInfo_clone.Rows[i]["vcPartENName"].ToString();
                            drSell_Temp["iQuantity"] = iResultQtyDailySum;
                            drSell_Temp["decPriceWithTax"] = dtDockInfo_clone.Rows[i]["decPriceTNPWithTax"].ToString();
                            drSell_Temp["decMoney"] = (iResultQtyDailySum * Convert.ToDecimal(dtDockInfo_clone.Rows[i]["decPriceTNPWithTax"].ToString())).ToString("#0.00");
                            drSell_Temp["vcOperatorID"] = opearteId;
                            //drSell_Temp["dOperatorTime"] = dtDockInfo_clone.Rows[i][""].ToString();
                            drSell_Temp["vcYinQuType"] = strYinQuType;
                            //drSell_Temp["vcSender"] = dtDockInfo_clone.Rows[i][""].ToString();
                            drSell_Temp["vcLabelStart"] = dtDockInfo_clone.Rows[i]["vcLabelStart"].ToString();
                            drSell_Temp["vcLabelEnd"] = dtDockInfo_clone.Rows[i]["vcLabelEnd"].ToString();
                            drSell_Temp["vcSupplier_id"] = dtDockInfo_clone.Rows[i]["vcSupplier_id"].ToString();
                            dtSell_Temp.Rows.Add(drSell_Temp);
                            #endregion

                            #region addrow-dtShipList
                            DataRow drShipList_Temp = dtShipList_Temp.NewRow();
                            drShipList_Temp["vcPackingspot"] = dtDockInfo_clone.Rows[i]["vcBZPlant"].ToString();
                            drShipList_Temp["vcSupplier"] = dtDockInfo_clone.Rows[i]["vcSupplier_id"].ToString();
                            drShipList_Temp["vcCpdcompany"] = dtDockInfo_clone.Rows[i]["vcSHF"].ToString();
                            //drShipList_Temp["vcControlno"] = dtDockInfo_clone.Rows[i][""].ToString();//销售单号
                            drShipList_Temp["vcPart_id"] = dtDockInfo_clone.Rows[i]["vcPart_id"].ToString();
                            drShipList_Temp["vcOrderno"] = orderNo;
                            drShipList_Temp["vcSeqno"] = seqNo;
                            //drShipList_Temp["vcInvoiceno"] = dtDockInfo_clone.Rows[i][""].ToString();
                            drShipList_Temp["vcPartsnamechn"] = dtDockInfo_clone.Rows[i]["vcPartNameCn"].ToString();
                            drShipList_Temp["vcPartsnameen"] = dtDockInfo_clone.Rows[i]["vcPartENName"].ToString();
                            drShipList_Temp["vcShippingqty"] = iResultQtyDailySum;
                            drShipList_Temp["vcCostwithtaxes"] = dtDockInfo_clone.Rows[i]["decPriceTNPWithTax"].ToString();
                            drShipList_Temp["vcPrice"] = (iResultQtyDailySum * Convert.ToDecimal(dtDockInfo_clone.Rows[i]["decPriceTNPWithTax"].ToString())).ToString("#0.00");
                            drShipList_Temp["iNocount"] = 0;
                            drShipList_Temp["vcCaseNo"] = dtDockInfo_clone.Rows[i]["vcCaseNo"].ToString();
                            drShipList_Temp["vcBoxNo"] = dtDockInfo_clone.Rows[i]["vcBoxNo"].ToString();
                            //drShipList_Temp["vcProgramfrom"] = dtDockInfo_clone.Rows[i][""].ToString();
                            //drShipList_Temp["vcComputernm"] = dtDockInfo_clone.Rows[i][""].ToString();
                            //drShipList_Temp["vcPackcode"] = dtDockInfo_clone.Rows[i][""].ToString();
                            //drShipList_Temp["vcCompany"] = dtDockInfo_clone.Rows[i][""].ToString();
                            drShipList_Temp["vcHostip"] = iP;
                            drShipList_Temp["vcOperatorID"] = opearteId;
                            //drShipList_Temp["dOperatorTime"] = dtDockInfo_clone.Rows[i][""].ToString();
                            //drShipList_Temp["dFirstPrintTime"] = dtDockInfo_clone.Rows[i][""].ToString();
                            //drShipList_Temp["dLatelyPrintTime"] = dtDockInfo_clone.Rows[i][""].ToString();
                            dtShipList_Temp.Rows.Add(drShipList_Temp);
                            #endregion

                            if (iSumQuantity == 0)
                                break;
                        }
                    }
                }

                #region addrow-dtSell_Sum
                DataRow drSell_Sum_Temp = dtSell_Sum_Temp.NewRow();
                //drSell_Sum_Temp["vcDate"] = dtDockInfo_clone.Rows[i][""].ToString();
                //drSell_Sum_Temp["vcBanZhi"] = dtDockInfo_clone.Rows[i][""].ToString();
                //drSell_Sum_Temp["vcBianCi"] = dtDockInfo_clone.Rows[i][""].ToString();
                //drSell_Sum_Temp["vcSellNo"] = dtDockInfo_clone.Rows[i][""].ToString();
                drSell_Sum_Temp["vcTruckNo"] = truckNo;
                drSell_Sum_Temp["iToolQuantity"] = caseSum;
                drSell_Sum_Temp["vcYinQuType"] = strYinQuType;
                drSell_Sum_Temp["vcOperatorID"] = opearteId;
                //drSell_Sum_Temp["dOperatorTime"] = dtDockInfo_clone.Rows[i][""].ToString();
                drSell_Sum_Temp["vcQianFen"] = qianFen;
                //drSell_Sum_Temp["vcLabelStart"] = dtDockInfo_clone.Rows[i][""].ToString();
                //drSell_Sum_Temp["vcLabelEnd"] = dtDockInfo_clone.Rows[i][""].ToString();
                drSell_Sum_Temp["vcSender"] = opearteId;
                dtSell_Sum_Temp.Rows.Add(drSell_Sum_Temp);
                #endregion

                #region addrow-dtSell_Tool
                DataRow drSell_Tool_Temp_PC = dtSell_Tool_Temp.NewRow();
                //drSell_Tool_Temp_PC["vcSellNo"] = dtDockInfo_clone.Rows[i][""].ToString();
                drSell_Tool_Temp_PC["vcToolName"] = "金属支柱";
                drSell_Tool_Temp_PC["vcToolCode"] = "PC";
                drSell_Tool_Temp_PC["vcToolColor"] = "红";
                drSell_Tool_Temp_PC["iToolQuantity"] = int.Parse(pCQuantity);
                drSell_Tool_Temp_PC["vcOperatorID"] = opearteId;
                //drSell_Tool_Temp_PC["dOperatorTime"] = dtDockInfo_clone.Rows[i][""].ToString();
                drSell_Tool_Temp_PC["vcYinQuType"] = strYinQuType;
                dtSell_Tool_Temp.Rows.Add(drSell_Tool_Temp_PC);

                DataRow drSell_Tool_Temp_BP = dtSell_Tool_Temp.NewRow();
                //drSell_Tool_Temp_BP["vcSellNo"] = dtDockInfo_clone.Rows[i][""].ToString();
                drSell_Tool_Temp_BP["vcToolName"] = "木托盘";
                drSell_Tool_Temp_BP["vcToolCode"] = "BP";
                drSell_Tool_Temp_BP["vcToolColor"] = "灰白";
                drSell_Tool_Temp_BP["iToolQuantity"] = int.Parse(bPQuantity);
                drSell_Tool_Temp_BP["vcOperatorID"] = opearteId;
                //drSell_Tool_Temp_BP["dOperatorTime"] = dtDockInfo_clone.Rows[i][""].ToString();
                drSell_Tool_Temp_BP["vcYinQuType"] = strYinQuType;
                dtSell_Tool_Temp.Rows.Add(drSell_Tool_Temp_BP);

                DataRow drSell_Tool_Temp_HU = dtSell_Tool_Temp.NewRow();
                //drSell_Tool_Temp_HU["vcSellNo"] = dtDockInfo_clone.Rows[i][""].ToString();
                drSell_Tool_Temp_HU["vcToolName"] = "金属绿筐";
                drSell_Tool_Temp_HU["vcToolCode"] = "HU";
                drSell_Tool_Temp_HU["vcToolColor"] = "绿";
                drSell_Tool_Temp_HU["iToolQuantity"] = int.Parse(hUQuantity1);
                drSell_Tool_Temp_HU["vcOperatorID"] = opearteId;
                //drSell_Tool_Temp_HU["dOperatorTime"] = dtDockInfo_clone.Rows[i][""].ToString();
                drSell_Tool_Temp_HU["vcYinQuType"] = strYinQuType;
                dtSell_Tool_Temp.Rows.Add(drSell_Tool_Temp_HU);

                DataRow drSell_Tool_Temp_CB = dtSell_Tool_Temp.NewRow();
                //drSell_Tool_Temp_CB["vcSellNo"] = dtDockInfo_clone.Rows[i][""].ToString();
                drSell_Tool_Temp_CB["vcToolName"] = "金属支柱";
                drSell_Tool_Temp_CB["vcToolCode"] = "CB";
                drSell_Tool_Temp_CB["vcToolColor"] = "蓝";
                drSell_Tool_Temp_CB["iToolQuantity"] = int.Parse(cBQuantity);
                drSell_Tool_Temp_CB["vcOperatorID"] = opearteId;
                //drSell_Tool_Temp_CB["dOperatorTime"] = dtDockInfo_clone.Rows[i][""].ToString();
                drSell_Tool_Temp_CB["vcYinQuType"] = strYinQuType;
                dtSell_Tool_Temp.Rows.Add(drSell_Tool_Temp_CB);

                DataRow drSell_Tool_Temp_2HU = dtSell_Tool_Temp.NewRow();
                //drSell_Tool_Temp_2HU["vcSellNo"] = dtDockInfo_clone.Rows[i][""].ToString();
                drSell_Tool_Temp_2HU["vcToolName"] = "金属2HU箱";
                drSell_Tool_Temp_2HU["vcToolCode"] = "2HU";
                drSell_Tool_Temp_2HU["vcToolColor"] = "";
                drSell_Tool_Temp_2HU["iToolQuantity"] = int.Parse(hUQuantity);
                drSell_Tool_Temp_2HU["vcOperatorID"] = opearteId;
                //drSell_Tool_Temp_2HU["dOperatorTime"] = dtDockInfo_clone.Rows[i][""].ToString();
                drSell_Tool_Temp_2HU["vcYinQuType"] = strYinQuType;
                dtSell_Tool_Temp.Rows.Add(drSell_Tool_Temp_2HU);
                #endregion

                //6.生成销售单号及便次号并整理数据，对于5.进行销售单号及便次号赋值
                //获取班值信息
                //TSell_Tool-销售单号
                //TSell_Sum-销售单号+便次号
                //TSell-赋值-销售单号+便次号
                //TShipList-赋值-销售单号
                //TOperateSJ-赋值-销售单号
                string strHosDate = dtTime.Rows[0]["dHosDate"].ToString();
                string strBanZhi = dtTime.Rows[0]["vcBanZhi"].ToString();
                string formatDate = serverTime.Substring(0, 10);
                string formatDate1 = serverTime.Substring(0, 10).Replace("-", "");
                string tmpString = "SHPH2";
                string tmpString1 = "";
                if (strYinQuType == "cx")//生成销售单
                {
                    tmpString1 = "SHPCX";
                }
                if (strYinQuType == "wz")
                {
                    tmpString1 = "SHPWZ";
                }
                DataTable dtXSNo = P00004_Logic.setSeqNo(tmpString, 1, formatDate, strHosDate, strBanZhi, "销售单号");
                DataTable dtBSNo = P00004_Logic.setSeqNo(tmpString1, 1, formatDate, strHosDate, strBanZhi, "便次号");
                string strXSNo = "XS" + formatDate1 + "0" + dtXSNo.Rows[0]["vcSeqNo"].ToString().PadLeft(3, '0');
                string strBSNo = dtBSNo.Rows[0]["vcSeqNo"].ToString();
                #region 销售单+便次赋值
                for (int i = 0; i < dtSell_Tool_Temp.Rows.Count; i++)
                {
                    dtSell_Tool_Temp.Rows[i]["vcSellNo"] = strXSNo;
                }
                for (int i = 0; i < dtSell_Sum_Temp.Rows.Count; i++)
                {
                    dtSell_Sum_Temp.Rows[i]["vcDate"] = strHosDate;
                    dtSell_Sum_Temp.Rows[i]["vcBanZhi"] = strBanZhi;
                    dtSell_Sum_Temp.Rows[i]["vcBianCi"] = strBSNo;
                    dtSell_Sum_Temp.Rows[i]["vcSellNo"] = strXSNo;
                }
                for (int i = 0; i < dtSell_Temp.Rows.Count; i++)
                {
                    dtSell_Temp.Rows[i]["dHosDate"] = strHosDate;
                    dtSell_Temp.Rows[i]["vcBanZhi"] = strBanZhi;
                    dtSell_Temp.Rows[i]["vcBianCi"] = strBSNo;
                    dtSell_Temp.Rows[i]["vcSellNo"] = strXSNo;
                    dtSell_Temp.Rows[i]["vcInvoiceNo"] = strXSNo.Substring(4);
                }
                for (int i = 0; i < dtShipList_Temp.Rows.Count; i++)
                {
                    dtShipList_Temp.Rows[i]["vcControlno"] = strXSNo;
                    dtShipList_Temp.Rows[i]["vcInvoiceno"] = strXSNo.Substring(4);
                }
                for (int i = 0; i < dtOperateSJ_Temp.Rows.Count; i++)
                {
                    dtOperateSJ_Temp.Rows[i]["vcSellNo"] = strXSNo;
                }
                #endregion
                //7.影响数据库后台
                //插入更新5.6.
                //按照DOCK更新TShip_Temp为已出荷
                //按照DOCK解绑TSell_DockCar
                //插入TPrint_Temp
                //删除出荷临时表
                bool bRet = P00004_Logic.setCastListInfo(dtOperateSJ_Temp, dtOperateSJ_InOutput_Temp, dtOrder_Temp, dtSell_Temp, dtShipList_Temp, dtSell_Sum_Temp, dtSell_Tool_Temp, iP, strXSNo, dockSell, opearteId);
                if (!bRet)
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "数据写入数据库失败，请联系管理员后再试。";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                //8.生成csv文件到指定目录

                //9.发送带csv附件的邮件

                #region 作废
                /*
                string seqNo = "";
                string formatDate = serverTime.Substring(0, 10);
                string formatDate1 = serverTime.Substring(0, 10).Replace("-", "");
                string sellNo = "";
                string bianCi = "";





                if (caseSum == caseSum1)
                {
                    for (int i = 0; i < getCaseSum.Rows.Count; i++)
                    {
                        string caseNo = getCaseSum.Rows[i][0].ToString().Split("*")[1];
                        //2.拿到箱号之后取出入库指令号和数量,验证信息无误后放到临时表中,验证临时表中是否有相关信息
                        DataTable getSellInfo = P00004_Logic.GetSellInfo(caseNo);//装箱单数据
                        DataTable getSellInfo1 = P00004_Logic.GetSellInfo1(caseNo);//出荷实绩数据S4
                        DataTable getSellInfo2 = P00004_Logic.GetSellInfo2(caseNo);//吴凡使用的QB临时表
                        if (getSellInfo.Rows.Count > 0 && getSellInfo1.Rows.Count == 0)
                        {
                            for (int j = 0; j < getSellInfo.Rows.Count; j++)
                            {
                                string inno = getSellInfo.Rows[j][0].ToString();//入库单号
                                string quantity = getSellInfo.Rows[j][1].ToString();//数量
                                DataTable getQBData = P00004_Logic.GetQBData(inno);//吴凡使用的QB临时表获取S0入荷临时数据

                                if (getQBData.Rows.Count == 1)
                                {
                                    string trolley = getQBData.Rows[0][0].ToString(); //台车号
                                    string partId = getQBData.Rows[0][1].ToString();//品番
                                    string cpdCompany = getQBData.Rows[0][2].ToString();//收货方
                                    string dock = getQBData.Rows[0][3].ToString();//受入
                                    string kanbanOrderNo = getQBData.Rows[0][4].ToString();//看板订单号
                                    string kanbanSerial = getQBData.Rows[0][5].ToString();//看板连番
                                    string packingSpot = getQBData.Rows[0][6].ToString();//包装场
                                    string packingQuatity = getQBData.Rows[0][7].ToString();//包装单位
                                    string lblStart = getQBData.Rows[0][8].ToString();//标签开始
                                    string lblEnd = getQBData.Rows[0][9].ToString();//标签结束
                                    string supplierId = getQBData.Rows[0][10].ToString();//供应商代码
                                    string supplierPlant = getQBData.Rows[0][11].ToString();//供应商工区
                                    string lotId = getQBData.Rows[0][12].ToString();//段取指示号
                                    string checkType = getQBData.Rows[0][13].ToString();//检查区分
                                    string inoutFlag = getQBData.Rows[0][14].ToString();//内外区分

                                    DataTable validateData = P00004_Logic.ValidateData(partId, scanTime, dock);//获取收货方
                                    DataTable validateInv = P00004_Logic.ValidateInv(partId, kanbanOrderNo, kanbanSerial);//入出库履历获取待出荷数量
                                    DataTable validatePrice = P00004_Logic.ValidatePrice(partId, scanTime, cpdCompany, supplierId);//验证是否有单价
                                    #region 验证订单
                                    DataTable validateOrd1 = P00004_Logic.ValidateOrd1(partId);//验证当前有可消减的订单
                                    DataTable getCount = P00004_Logic.GetCount(partId);//吴凡使用的QB临时表获取本次已经插入到QB表的出荷数
                                    #endregion




                                    if (validateData.Rows.Count == 1
                                        && validatePrice.Rows.Count == 1
                                        && validateInv.Rows.Count == 1
                                        && (int.Parse(validateInv.Rows[0][0].ToString()) >= int.Parse(quantity))
                                        && (int.Parse(validateOrd1.Rows[0][0].ToString()) - int.Parse(getCount.Rows[0][0].ToString())
                                        >= int.Parse(quantity)))
                                    {
                                        //插入到QB临时表
                                        int qbResultIn = P00004_Logic.Insert(trolley, partId, quantity, dock, kanbanOrderNo, kanbanSerial, scanTime, iP, serverTime, cpdCompany, inno, opearteId, packingSpot, packingQuatity, lblStart, lblEnd, supplierId, supplierPlant, lotId, inoutFlag, checkType, caseNo, dockSell);//插入实绩情报表

                                    }
                                    else if (validateData.Rows.Count != 1)
                                    {
                                        P00004_Logic.DelData(dockSell);
                                        apiResult.code = ComConstant.ERROR_CODE;
                                        apiResult.data = "箱号" + caseNo + "中品番" + partId + "在品番基础数据中没有有效数据";//
                                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                                    }
                                    else if (validatePrice.Rows.Count != 1)
                                    {
                                        P00004_Logic.DelData(dockSell);
                                        apiResult.code = ComConstant.ERROR_CODE;
                                        apiResult.data = "箱号" + caseNo + "中品番" + partId + "在销售单价表中没有有效数据";//
                                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                                    }
                                    else if (validateInv.Rows.Count != 1)
                                    {
                                        P00004_Logic.DelData(dockSell);
                                        apiResult.code = ComConstant.ERROR_CODE;
                                        apiResult.data = "箱号" + caseNo + "中品番" + partId + "在入出库履历表中没有有效数据";
                                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                                    }
                                    else if (int.Parse(validateInv.Rows[0][0].ToString()) < int.Parse(quantity))
                                    {
                                        P00004_Logic.DelData(dockSell);
                                        apiResult.code = ComConstant.ERROR_CODE;
                                        apiResult.data = "箱号" + caseNo + "中品番" + partId + "在入出库履历中的待出荷数小于当前数量";
                                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                                    }
                                    else if ((int.Parse(validateInv.Rows[0][0].ToString()) < int.Parse(quantity)) && (int.Parse(validateOrd1.Rows[0][0].ToString()) - int.Parse(getCount.Rows[0][0].ToString()) >= int.Parse(quantity)))
                                    {
                                        P00004_Logic.DelData(dockSell);
                                        apiResult.code = ComConstant.ERROR_CODE;
                                        apiResult.data = "箱号" + caseNo + "中品番" + partId + "在订单表中的出库数小于当前数量";
                                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                                    }
                                }
                                else
                                {
                                    P00004_Logic.DelData(dockSell);
                                    apiResult.code = ComConstant.ERROR_CODE;
                                    apiResult.data = "箱号" + caseNo + "中入库指令书" + inno + "在实绩情报表中有多条数据!";//
                                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                                }
                            }
                        }
                        else if (getSellInfo1.Rows.Count > 0 || getSellInfo2.Rows.Count > 0)
                        {
                            P00004_Logic.DelData(dockSell);
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "箱号" + caseNo + "已经出货,请检查!";//
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        else if (getSellInfo.Rows.Count == 0)
                        {
                            P00004_Logic.DelData(dockSell);
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "没有需要更新的数据!";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                    }
                }
                else
                {

                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "填写的箱数与实绩不匹配,当前" + dockSell + "共有" + caseSum1 + "箱";//
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

                }

                string bianCiSeqNo = "";

                //全部插入到实绩情报表之后执行更新操作
                DataTable getQBData1 = P00004_Logic.GetQBData1(dockSell);//从QB中获取已经插入的临时出荷数据
                DataTable getBanZhi = P00004_Logic.GetBanZhi(serverTime);//获取班值信息
                string date = "";
                string banzhi = "";
                if (getBanZhi.Rows.Count == 1)
                {
                    date = getBanZhi.Rows[0][0].ToString();
                    banzhi = getBanZhi.Rows[0][1].ToString();
                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "获取班值失败!";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                if (bian == "1")
                {
                    bianCi = "cx";
                }
                else if (bian == "2")
                {
                    bianCi = "wz";
                }
                if (bianCi == "cx")//生成销售单
                {
                    string tmpString = "SHPH2";
                    DataTable getSeqNo = P00004_Logic.GetSeqNo(tmpString, formatDate);
                    if (getSeqNo.Rows.Count == 0)
                    {
                        int seqResultIn = P00004_Logic.InsertSeqNo(tmpString, formatDate);
                    }
                    getSeqNo = P00004_Logic.GetSeqNo(tmpString, formatDate);
                    seqNo = getSeqNo.Rows[0][0].ToString();
                    int seqNoNew = int.Parse(seqNo) + 1;
                    int seqResultUp = P00004_Logic.UpdateSeqNo(seqNoNew, formatDate, tmpString);
                    sellNo = "XS" + formatDate1 + "0" + seqNo.PadLeft(3, '0');
                    int toolResult = P00004_Logic.InsertTool(sellNo, opearteId, scanTime, hUQuantity, hUQuantity1, bPQuantity, pCQuantity, cBQuantity, bianCi);
                }
                else if (bianCi == "wz")
                {
                    string tmpString = "SHPH2";
                    DataTable getSeqNo = P00004_Logic.GetSeqNo(tmpString, formatDate);
                    if (getSeqNo.Rows.Count == 0)
                    {
                        int seqResultIn = P00004_Logic.InsertSeqNo(tmpString, formatDate);
                    }
                    getSeqNo = P00004_Logic.GetSeqNo(tmpString, formatDate);
                    seqNo = getSeqNo.Rows[0][0].ToString();
                    int seqNoNew = int.Parse(seqNo) + 1;
                    int seqResultUp = P00004_Logic.UpdateSeqNo(seqNoNew, formatDate, tmpString);
                    sellNo = "XS" + formatDate1 + "0" + seqNo.PadLeft(3, '0');
                    int toolResult = P00004_Logic.InsertTool(sellNo, opearteId, scanTime, hUQuantity, hUQuantity1, bPQuantity, pCQuantity, cBQuantity, bianCi);
                }
                if (bianCi == "cx")
                {
                    string tmpString = "SHPCX";
                    DataTable getSeqNo = P00004_Logic.GetSeqNo(tmpString, formatDate);
                    if (getSeqNo.Rows.Count == 0)
                    {
                        int seqResultIn = P00004_Logic.InsertSeqNo(tmpString, formatDate);
                    }
                    getSeqNo = P00004_Logic.GetSeqNo(tmpString, formatDate);
                    bianCiSeqNo = getSeqNo.Rows[0][0].ToString();
                    int seqNoNew = int.Parse(bianCiSeqNo) + 1;
                    int seqResultUp = P00004_Logic.UpdateSeqNo(seqNoNew, formatDate, tmpString);
                    int sumResultIn = P00004_Logic.InsertSum(bianCiSeqNo, sellNo, truckNo, caseSum, bianCi, opearteId, serverTime, date, banzhi, qianFen);
                }
                else if (bianCi == "wz")
                {
                    string tmpString = "SHPWZ";
                    DataTable getSeqNo = P00004_Logic.GetSeqNo(tmpString, formatDate);
                    if (getSeqNo.Rows.Count == 0)
                    {
                        int seqResultIn = P00004_Logic.InsertSeqNo(tmpString, formatDate);
                    }
                    getSeqNo = P00004_Logic.GetSeqNo(tmpString, formatDate);
                    bianCiSeqNo = getSeqNo.Rows[0][0].ToString();
                    int seqNoNew = int.Parse(bianCiSeqNo) + 1;
                    int seqResultUp = P00004_Logic.UpdateSeqNo(seqNoNew, formatDate, tmpString);
                    int sumResultIn = P00004_Logic.InsertSum(bianCiSeqNo, sellNo, truckNo, caseSum, bianCi, opearteId, serverTime, date, banzhi, qianFen);
                }
                if (getQBData1.Rows.Count > 0)
                {
                    for (int j = 0; j < getQBData1.Rows.Count; j++)
                    {
                        string trolley = getQBData1.Rows[j][0].ToString(); //台车号

                        string partId = getQBData1.Rows[j][1].ToString();//品番
                        string cpdCompany = getQBData1.Rows[j][2].ToString();//收货方
                        string dock = getQBData1.Rows[j][3].ToString();//受入

                        string kanbanOrderNo = getQBData1.Rows[j][4].ToString();//看板订单号
                        string kanbanSerial = getQBData1.Rows[j][5].ToString();//看板连番

                        string packingSpot = getQBData1.Rows[j][6].ToString();//包装场
                        string packingQuatity = getQBData1.Rows[j][7].ToString();//包装单位
                        string lblStart = getQBData1.Rows[j][8].ToString();//标签开始
                        string lblEnd = getQBData1.Rows[j][9].ToString();//标签结束
                        string supplierId = getQBData1.Rows[j][10].ToString();//供应商代码
                        string supplierPlant = getQBData1.Rows[j][11].ToString();//供应商工区
                        string lotId = getQBData1.Rows[j][12].ToString();//段取指示号
                        string checkType = getQBData1.Rows[j][13].ToString();//检查区分
                        string inoutFlag = getQBData1.Rows[j][14].ToString();//内外区分
                        string caseNo = getQBData1.Rows[j][15].ToString();//箱号
                        string inputNo = getQBData1.Rows[j][16].ToString();//入库单号
                        DataTable getOprData = P00004_Logic.GetOprData(caseNo, inputNo);//验证作业实绩-装箱
                        DataTable getPrice = P00004_Logic.ValidatePrice(partId, scanTime, cpdCompany, supplierId);//验证master--价格
                        DataTable validateData = P00004_Logic.ValidateData(partId, scanTime, dock);//获取品名信息
                        DataTable validateInv = P00004_Logic.ValidateInv(partId, kanbanOrderNo, kanbanSerial);//获取入出库履历-待出荷数量
                        DataTable validateOrd1 = P00004_Logic.ValidateOrd1(partId);//获取可消减订单数量
                        DataTable validateOrd = P00004_Logic.ValiateOrd2(partId);//获取订单明细
                        DataTable getCount = P00004_Logic.GetCount(partId);//获取QB表中待出荷明细
                        //DataTable getPartsName = P00004_Logic.GetPartsName1(partId,scanTime);

                        if (getOprData.Rows.Count > 0 && validateData.Rows.Count == 1 && validateInv.Rows.Count == 1)
                        {
                            string quantity = getOprData.Rows[0][0].ToString();//装箱数量
                            string checkStatus = getOprData.Rows[0][1].ToString();//检查结果
                            string partsNameEn = validateData.Rows[0][0].ToString();
                            string partsNameCn = validateData.Rows[0][1].ToString();
                            string price = getPrice.Rows[0][0].ToString();
                            string invoiceNo = sellNo.Substring(4, 10);
                            string dCH = validateInv.Rows[0][0].ToString();




                            if (int.Parse(dCH) >= int.Parse(quantity)
                                && (int.Parse(validateInv.Rows[0][0].ToString()) >= int.Parse(quantity))
                                && (int.Parse(validateOrd1.Rows[0][0].ToString()) - int.Parse(getCount.Rows[0][0].ToString()) >= int.Parse(quantity)))
                            {
                                //作业实绩表
                                int sjReultIn = P00004_Logic.InsertSj(packingSpot, inputNo, kanbanOrderNo, kanbanSerial, partId, inoutFlag, supplierId, supplierPlant, scanTime, serverTime, quantity, packingQuatity, cpdCompany, dock, checkType, lblStart, lblEnd, opearteId, checkStatus, caseNo, sellNo, iP, pointType);
                                //销售表
                                int sellResultIn = P00004_Logic.InsertSell(bianCiSeqNo, sellNo, truckNo, cpdCompany, partId, kanbanOrderNo, kanbanSerial, invoiceNo, caseNo, partsNameEn, quantity, bianCi, opearteId, scanTime, supplierId, lblStart, lblEnd, price);
                                //入出库履历
                                int invResultUp = P00004_Logic.UpdateInv1(partId, kanbanOrderNo, kanbanSerial, quantity);
                                //发货明细书
                                int shpResulIn = P00004_Logic.InsertShip1(cpdCompany, packingSpot, sellNo, partId, invoiceNo, bianCiSeqNo, quantity, caseNo, partsNameEn, opearteId, iP, partsNameCn, price, serverTime, supplierId);

                                int SumQuantity = int.Parse(quantity);
                                int newSum = 0;
                                #region  更新订单表
                                for (int o = 0; o < validateOrd.Rows.Count; o++)
                                {
                                    string targetMonth = validateOrd.Rows[o][0].ToString();
                                    string orderType = validateOrd.Rows[o][1].ToString();
                                    string orderNo = validateOrd.Rows[o][2].ToString();
                                    string seqNo1 = validateOrd.Rows[o][3].ToString();
                                    int d1 = int.Parse(validateOrd.Rows[o][4].ToString());
                                    int d2 = int.Parse(validateOrd.Rows[o][5].ToString());
                                    int d3 = int.Parse(validateOrd.Rows[o][6].ToString());
                                    int d4 = int.Parse(validateOrd.Rows[o][7].ToString());
                                    int d5 = int.Parse(validateOrd.Rows[o][8].ToString());
                                    int d6 = int.Parse(validateOrd.Rows[o][9].ToString());
                                    int d7 = int.Parse(validateOrd.Rows[o][10].ToString());
                                    int d8 = int.Parse(validateOrd.Rows[o][11].ToString());
                                    int d9 = int.Parse(validateOrd.Rows[o][12].ToString());
                                    int d10 = int.Parse(validateOrd.Rows[o][13].ToString());
                                    int d11 = int.Parse(validateOrd.Rows[o][14].ToString());
                                    int d12 = int.Parse(validateOrd.Rows[o][15].ToString());
                                    int d13 = int.Parse(validateOrd.Rows[o][16].ToString());
                                    int d14 = int.Parse(validateOrd.Rows[o][17].ToString());
                                    int d15 = int.Parse(validateOrd.Rows[o][18].ToString());
                                    int d16 = int.Parse(validateOrd.Rows[o][19].ToString());
                                    int d17 = int.Parse(validateOrd.Rows[o][20].ToString());
                                    int d18 = int.Parse(validateOrd.Rows[o][21].ToString());
                                    int d19 = int.Parse(validateOrd.Rows[o][22].ToString());
                                    int d20 = int.Parse(validateOrd.Rows[o][23].ToString());
                                    int d21 = int.Parse(validateOrd.Rows[o][24].ToString());
                                    int d22 = int.Parse(validateOrd.Rows[o][25].ToString());
                                    int d23 = int.Parse(validateOrd.Rows[o][26].ToString());
                                    int d24 = int.Parse(validateOrd.Rows[o][27].ToString());
                                    int d25 = int.Parse(validateOrd.Rows[o][28].ToString());
                                    int d26 = int.Parse(validateOrd.Rows[o][29].ToString());
                                    int d27 = int.Parse(validateOrd.Rows[o][30].ToString());
                                    int d28 = int.Parse(validateOrd.Rows[o][31].ToString());
                                    int d29 = int.Parse(validateOrd.Rows[o][32].ToString());
                                    int d30 = int.Parse(validateOrd.Rows[o][33].ToString());
                                    int d31 = int.Parse(validateOrd.Rows[o][34].ToString());
                                    int[] array = {d1, d2, d3, d4, d5, d6, d7, d8, d9, d10, d11, d12, d13, d14, d15, d16, d17, d18, d19, d20, d21,
              d22,d23,d24,d25,d26,d27,d28,d29,d30,d31 };
                                    int[] newarray = new int[31];
                                    for (int l = 0; l < array.Length; l++)
                                    {
                                        if (SumQuantity - array[l] > 0)
                                        {
                                            newarray[l] = array[l];
                                            SumQuantity = SumQuantity - array[l];

                                        }
                                        else
                                        {
                                            newarray[l] = SumQuantity;
                                            SumQuantity = 0;

                                        }
                                    }
                                    for (int k = 0; k < newarray.Length; k++)
                                    {
                                        newSum += newarray[k];
                                    }
                                    int updateOrd = P00004_Logic.UpdateOrd(targetMonth, orderNo, seqNo1, newarray[0], newarray[1], newarray[2], newarray[3], newarray[4], newarray[5], newarray[6], newarray[7], newarray[8], newarray[9], newarray[10],
                                      newarray[11], newarray[12], newarray[13], newarray[14], newarray[15], newarray[16], newarray[17], newarray[18], newarray[19], newarray[20], newarray[21], newarray[22], newarray[23],
                                      newarray[24], newarray[25], newarray[26], newarray[27], newarray[28], newarray[29], newarray[30], newSum, partId);
                                    newSum = 0;
                                }
                                #endregion
                            }
                            else if ((int.Parse(validateInv.Rows[0][0].ToString()) < int.Parse(quantity)) && (int.Parse(validateOrd1.Rows[0][0].ToString()) - int.Parse(getCount.Rows[0][0].ToString()) >= int.Parse(quantity)))
                            {
                                apiResult.code = ComConstant.ERROR_CODE;
                                apiResult.data = "箱号" + caseNo + "中品番" + partId + "在订单表中的出库数小于当前数量";
                                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                            }
                            else
                            {
                                apiResult.code = ComConstant.ERROR_CODE;
                                apiResult.data = "箱号" + caseNo + "中品番" + partId + "入出库履历中的待出荷数量大于当前数量";
                                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                            }
                        }
                        else if (getOprData.Rows.Count == 0)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "箱号" + caseNo + "中品番" + partId + "在作业作业实绩表中没有有效的装箱数据!";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }

                        else if (validateData.Rows.Count != 1)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "箱号" + caseNo + "中品番" + partId + "在品番基础数据表中没有有效的装箱数据!";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                        else if (validateInv.Rows.Count != 1)
                        {
                            apiResult.code = ComConstant.ERROR_CODE;
                            apiResult.data = "箱号" + caseNo + "中品番" + partId + "在入出库履历表中没有有效数据";
                            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                        }
                    }
                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "没有需要更新的数据";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                */
                #endregion

                #region 做成CSV文件
                DataTable dtSellInfo = P00004_Logic.getSellInfo(strXSNo);
                string path = "";
                if (dtSellInfo.Rows.Count > 0)
                {
                    string[] head = new string[] { };
                    string[] field = new string[] { };
                    string msg = string.Empty;
                    head = new string[] { "inv_no", "inv_date", "part_no", "part_name", "case_no", "ord_no", "item_no", "dlr_no", "qty", "price" };
                    field = new string[] { "inv_no", "inv_date", "part_no", "part_name", "case_no", "ord_no", "item_no", "dlr_no", "qty", "price" };
                    path = P00004_Logic.DataTableToExcel(head, field, dtSellInfo, _webHostEnvironment.ContentRootPath, opearteId, "P00001", ref msg, strXSNo);
                }
                #endregion

                #region 发送邮件到销售公司
                DataSet dsSumandToolOfSell = P00004_Logic.getSumandToolOfSell(strXSNo, strYinQuType);
                DataTable getDataInfo = dsSumandToolOfSell.Tables[0];
                DataTable getSellInfo3 = dsSumandToolOfSell.Tables[1];
                //DataTable getDataInfo = P00004_Logic.GetDataInfo(sellNo, bianCi);

                //DataTable getSellInfo3 = P00004_Logic.GetSellInfo3(sellNo, bianCi);

                if (getDataInfo.Rows.Count == 1 && getSellInfo3.Rows.Count > 0)
                {

                    string bianci = getDataInfo.Rows[0][0].ToString();
                    string truckNo1 = getDataInfo.Rows[0][1].ToString();
                    string toolSum = getDataInfo.Rows[0][2].ToString();
                    string yinQuType = getDataInfo.Rows[0][3].ToString();
                    string banZhi = getDataInfo.Rows[0][4].ToString();
                    string date1 = getDataInfo.Rows[0][5].ToString();
                    string info = "";
                    string yinQuType1 = "";
                    if (yinQuType == "cx")
                    {
                        yinQuType1 = "成型";
                    }
                    else
                    {
                        yinQuType1 = "外注加钣金";
                    }

                    for (int i = 0; i < getSellInfo3.Rows.Count; i++)
                    {
                        info = info + getSellInfo3.Rows[i][0].ToString() + ":" + getSellInfo3.Rows[i][1].ToString() + "<br>";
                    }
                    //string path1 = @"C:\Users\Administrator\Desktop\laowu 0531修改\FILE\" + path;
                    string path1 = path;
                    string mail = "fqm_wufan@tftm.com.cn";
                    string userName = "laowu";

                    string emailBody = "FTMS各位相关同事:<br>大家好!<br>附件为销售数据,请查收!<br>发货日期:" + date1 + "<br>发货班值:" + strBanZhi + "<br>便次区分:" + yinQuType1 + "第" + bianci + "便<br>引取车牌照号:" + truckNo1 + "<br>合计数量" + toolSum + "个<br>器具明细:" + info + "<br>收货时请及时确认数量<br>以上";
                    string subject = "发货";
                    DataTable getCode = P00004_Logic.GetCode();
                    if (getCode.Rows.Count > 0)
                    {
                        ComFunction.SendEmailInfo(mail, userName, emailBody, getCode, getCode, subject, path1, true);
                    }
                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "发货信息不存在,无法发送邮件";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                #endregion
                P00004_DataEntity.shipResult = "发货成功";
                apiResult.data = P00004_DataEntity;
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "发货失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
        }
        #endregion

        #region 出荷-便次信息查询
        public string GetSellDataApi([FromBody] dynamic data)
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
                string timeFrom = dataForm.Timefrom == null ? "" : dataForm.Timefrom;
                string timeEnd = dataForm.Timeend == null ? "" : dataForm.Timeend;
                string type = dataForm.Type == null ? "" : dataForm.Type;
                string time = dataForm.Time == null ? "" : dataForm.Time;
                //根据客户端时间判断班值
                DataTable getBanZhi = P00004_Logic.GetBanZhi(time);
                if (getBanZhi.Rows.Count == 1)
                {
                    string date = getBanZhi.Rows[0][0].ToString();
                    string banZhi = getBanZhi.Rows[0][1].ToString();
                    DataTable GetSellData = P00004_Logic.GetSellData(timeFrom, timeEnd, type, date, banZhi);
                    if (GetSellData.Rows.Count > 0)
                    {
                        P00004_DataEntity.sellList = GetSellData;
                        apiResult.data = P00004_DataEntity;
                    }
                    else
                    {
                        apiResult.code = ComConstant.ERROR_CODE;
                        apiResult.data = "当前班值没有出货数据,请检查!";
                        return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                    }
                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "获取班值失败!";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "获取出货便次明细失败";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }



        }
        #endregion

        #region  出荷-便次信息查询-器具明细查看
        public string GetToolInfoApi([FromBody] dynamic data)
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
                string sellNo = dataForm.SellNo == null ? "" : dataForm.SellNo;

                DataTable getToolInfo = P00004_Logic.GetToolInfo(sellNo);
                if (getToolInfo.Rows.Count == 5)
                {
                    P00004_DataEntity.HUQuantity = getToolInfo.Rows[0][1].ToString();
                    P00004_DataEntity.BPQuantity = getToolInfo.Rows[1][1].ToString();
                    P00004_DataEntity.CBQuantity = getToolInfo.Rows[2][1].ToString();
                    P00004_DataEntity.HUQuantity1 = getToolInfo.Rows[3][1].ToString();
                    P00004_DataEntity.PCQuantity = getToolInfo.Rows[4][1].ToString();

                    apiResult.data = P00004_DataEntity;




                }
                else
                {
                    apiResult.code = ComConstant.ERROR_CODE;
                    apiResult.data = "没有找到对应销售单号的器具信息!";
                    return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
                }


            }
            catch (Exception ex)
            {
                ComMessage.GetInstance().ProcessMessage(FunctionID, "M03UE0901", ex, opearteId);
                apiResult.code = ComConstant.ERROR_CODE;
                apiResult.data = "获取器具信息失败!";
                return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);
            }


            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, JSON_SETTING);

        }
        #endregion        
    }

}
