using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;
using Common;

namespace Logic
{
    public class FS0811_Logic
    {
        FS0603_Logic fS0603_Logic = new FS0603_Logic();
        FS0811_DataAccess fs0811_DataAccess;

        public FS0811_Logic()
        {
            fs0811_DataAccess = new FS0811_DataAccess();
        }
        public Dictionary<string, object> setLoadPage(Dictionary<string, object> res, string strPackPlant, ref string type, ref int code)
        {
            try
            {
                DataTable dtBanZhi = getBanZhiTime(strPackPlant, "1");
                if (dtBanZhi == null || dtBanZhi.Rows.Count == 0)
                {
                    code = ComConstant.ERROR_CODE;
                    type = "e2";
                    return res;
                }
                string strHosDate = dtBanZhi.Rows[0]["dHosDate"].ToString();
                string strBanZhi = dtBanZhi.Rows[0]["vcBanZhi"].ToString();
                string strFromTime = dtBanZhi.Rows[0]["tToTime_bf"].ToString();
                string strToTime = dtBanZhi.Rows[0]["tToTime_nw"].ToString();
                //数据列表表头
                string strBZ = strHosDate + "(" + strBanZhi + ")";
                res.Add("BZItem", strBZ);

                //创建页面头部信息显示
                string strPeopleNum = "";
                string strCycleTime = "";
                string strWorkOverTime = "";
                int iCycleTime_mi = 450;
                DataTable dtPageInfo = getLoadData(strPackPlant, strHosDate, strBanZhi, iCycleTime_mi, ref strPeopleNum, ref strCycleTime, ref strWorkOverTime);
                if (dtPageInfo == null)
                {
                    code = ComConstant.ERROR_CODE;
                    type = "e3";
                    return res;
                }
                ///创建页面主题信息显示
                DataTable dataTable = dtPageInfo;
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("bSelectFlag", ConvertFieldType.BoolType, null);
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dataTable, dtConverter);

                res.Add("PeopleNumItem", strPeopleNum);
                res.Add("CycleTimeItem", strCycleTime);
                res.Add("WorkOverTimeItem", strWorkOverTime);
                res.Add("dataList", dataList);
                code = ComConstant.SUCCESS_CODE;
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Dictionary<string, object> setTempPage(Dictionary<string, object> res, string strPackPlant, string strHosDate, string strBanZhi, ref string type, ref int code)
        {
            try
            {
                string strBZ = strHosDate + "(" + strBanZhi + ")";
                res.Add("BZItem", strBZ);
                //创建页面头部信息显示
                string strPeopleNum = "";
                string strCycleTime = "";
                string strWorkOverTime = "";
                DataTable dtPageInfo = getTempData(strPackPlant, strHosDate, strBanZhi, ref strPeopleNum, ref strCycleTime, ref strWorkOverTime);
                if (dtPageInfo == null)
                {
                    code = ComConstant.ERROR_CODE;
                    type = "e3";
                    return res;
                }
                ///创建页面主题信息显示
                DataTable dataTable = dtPageInfo;
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("bSelectFlag", ConvertFieldType.BoolType, null);
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dataTable, dtConverter);
                res.Add("PeopleNumItem", strPeopleNum);
                res.Add("CycleTimeItem", strCycleTime);
                res.Add("WorkOverTimeItem", strWorkOverTime);
                res.Add("dataList", dataList);
                code = ComConstant.SUCCESS_CODE;
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getBanZhiTime(string strPackPlant, string strFlag)
        {
            return fs0811_DataAccess.getBanZhiTime(strPackPlant, strFlag);
        }
        public DataTable getLoadData(string strPackPlant, string strHosDate, string strBanZhi, int iCycleTime_mi,
          ref string strPeopleNum, ref string strCycleTime, ref string strWorkOverTime)
        {

            //创建页面大品目别绑定datatable
            DataTable dtPageInfo = fs0811_DataAccess.getPageDataInfo();
            //查询小品目别信息
            //1.查询人员投入
            DataSet dsInPutIntoOver_small = fs0811_DataAccess.getInPutIntoOver_small(strPackPlant, strHosDate, strBanZhi);
            if (dsInPutIntoOver_small != null && dsInPutIntoOver_small.Tables[0].Rows.Count != 0)
            {
                return setPageInfo(false, strPackPlant, strHosDate, strBanZhi, dsInPutIntoOver_small,
                    dtPageInfo, ref strPeopleNum, ref strCycleTime, ref strWorkOverTime);
            }
            //2.查询查询实行计划
            DataSet dsPackingPlan_summary = fs0811_DataAccess.getPackingPlan_summary(strPackPlant, strHosDate, strBanZhi, string.Empty, iCycleTime_mi);
            if (dsPackingPlan_summary != null && dsPackingPlan_summary.Tables[1].Rows.Count != 0)
            {
                return setPackingPlanInfo(strPackPlant, strHosDate, strBanZhi, dsPackingPlan_summary,
                    dtPageInfo, ref strPeopleNum, ref strCycleTime, ref strWorkOverTime);
            }
            return null;
        }
        public DataTable getTempData(string strPackPlant, string strHosDate, string strBanZhi, ref string strPeopleNum, ref string strCycleTime, ref string strWorkOverTime)
        {
            //创建页面大品目别绑定datatable
            DataTable dtPageInfo = fs0811_DataAccess.getPageDataInfo();
            //查询小品目别信息
            //1.查询人员投入
            DataSet dsInPutIntoOver_small_temp = fs0811_DataAccess.getInPutIntoOver_small_temp(strPackPlant, strHosDate, strBanZhi);
            if (dsInPutIntoOver_small_temp != null && dsInPutIntoOver_small_temp.Tables[0].Rows.Count != 0)
            {
                return setPageInfo(false, strPackPlant, strHosDate, strBanZhi, dsInPutIntoOver_small_temp,
                    dtPageInfo, ref strPeopleNum, ref strCycleTime, ref strWorkOverTime);
            }
            return null;
        }
        public bool checkTempData(string strPackPlant, string strHosDate, string strBanZhi)
        {
            DataSet dsInPutIntoOver_small_temp = fs0811_DataAccess.getInPutIntoOver_small_temp(strPackPlant, strHosDate, strBanZhi);
            if (!(dsInPutIntoOver_small_temp != null && dsInPutIntoOver_small_temp.Tables[0].Rows.Count != 0))
                return true;
            return false;
        }
        public DataTable setPageInfo(bool bQuery, string strPackPlant, string strHosDate, string strBanZhi, DataSet dsInPutIntoOver_small,
            DataTable dtPageInfo, ref string strPeopleNum, ref string strCycleTime, ref string strWorkOverTime)
        {
            try
            {
                DataTable dtHead = dsInPutIntoOver_small.Tables[0];
                strPeopleNum = dtHead.Rows[0]["decPeopleNum"].ToString();
                strCycleTime = dtHead.Rows[0]["decCycleTime_mi"].ToString();
                strWorkOverTime = dtHead.Rows[0]["decWorkOverTime"].ToString();
                DataTable dtBody = dsInPutIntoOver_small.Tables[1];
                for (int i = 0; i < dtPageInfo.Rows.Count; i++)
                {
                    string strBigPM = dtPageInfo.Rows[i]["vcBigPM"].ToString();
                    DataRow[] drBody = dtBody.Select("vcBigPM='" + strBigPM + "'");
                    dtPageInfo.Rows[i]["vcPackPlant"] = strPackPlant;
                    dtPageInfo.Rows[i]["dHosDate"] = strHosDate;
                    dtPageInfo.Rows[i]["vcBanZhi"] = strBanZhi;
                    if (drBody.Length != 0)
                    {
                        //dtPageInfo.Rows[i]["decPackNum"] = Convert.ToDecimal(drBody[0]["iPackNum_summary"].ToString()) < 0 ? "0" : drBody[0]["iPackNum_summary"].ToString();
                        //dtPageInfo.Rows[i]["decPlannedTime"] = Convert.ToDecimal(drBody[0]["decPlannedTime_summary"].ToString()) < 0 ? "0" : drBody[0]["decPlannedTime_summary"].ToString();
                        //dtPageInfo.Rows[i]["decPlannedPerson"] = Convert.ToDecimal(drBody[0]["decPlannedPerson_summary"].ToString()) < 0 ? "0" : drBody[0]["decPlannedPerson_summary"].ToString();
                        //dtPageInfo.Rows[i]["decInputPerson"] = Convert.ToDecimal(drBody[0]["decInputPerson_summary"].ToString()) < 0 ? "" : drBody[0]["decInputPerson_summary"].ToString();
                        //dtPageInfo.Rows[i]["decInputTime"] = Convert.ToDecimal(drBody[0]["decInputTime"].ToString()) < 0 ? "0.00" : drBody[0]["decInputTime"].ToString();
                        //dtPageInfo.Rows[i]["decOverFlowTime"] = Convert.ToDecimal(drBody[0]["decOverFlowTime"].ToString()) < 0 ? "0.00" : drBody[0]["decOverFlowTime"].ToString();

                        dtPageInfo.Rows[i]["decPackNum"] = drBody[0]["iPackNum_summary"].ToString();
                        dtPageInfo.Rows[i]["decPlannedTime"] = drBody[0]["decPlannedTime_summary"].ToString();
                        dtPageInfo.Rows[i]["decPlannedPerson"] = drBody[0]["decPlannedPerson_summary"].ToString();
                        dtPageInfo.Rows[i]["decInputPerson"] = drBody[0]["decInputPerson_summary"].ToString();
                        dtPageInfo.Rows[i]["decInputTime"] = drBody[0]["decInputTime"].ToString();
                        dtPageInfo.Rows[i]["decOverFlowTime"] = drBody[0]["decOverFlowTime"].ToString();
                    }
                }
                //计算登录人信息
                dtPageInfo = setPointStateInfo(dtPageInfo, strPackPlant);
                //以下为计算逻辑
                if (bQuery)
                {
                    //计算"投入人员"	"投入工时（小时）"	"过或不足（小时）"
                    decimal decPeopleNum = Convert.ToDecimal(strPeopleNum);//最大包装持有人数
                    decimal decPlannedPerson_sum = 0;//计划人数合计
                    decimal decInputPerson_sum = 0;//投入人数合计
                    for (int i = 0; i < dtPageInfo.Rows.Count; i++)
                    {
                        decimal decPlannedTime = Convert.ToDecimal(dtPageInfo.Rows[i]["decPlannedTime"].ToString());
                        decimal decPlannedPerson = Convert.ToDecimal(dtPageInfo.Rows[i]["decPlannedPerson"].ToString());
                        decimal decInputPerson = Convert.ToDecimal(dtPageInfo.Rows[i]["decInputPerson"].ToString());
                        decimal decInputTime = decInputPerson * (Convert.ToDecimal(strCycleTime) / 60);
                        decimal decOverFlowTime = decInputTime - decPlannedTime;

                        decPlannedPerson_sum = decPlannedPerson_sum + decPlannedPerson;
                        decInputPerson_sum = decInputPerson_sum + decInputPerson;
                        //登录人员获取并计算最后两列decSysLander、decDiffer

                        dtPageInfo.Rows[i]["decInputTime"] = decInputTime.ToString("#0.00");
                        dtPageInfo.Rows[i]["decOverFlowTime"] = decOverFlowTime.ToString("#0.00");
                    }
                    strWorkOverTime = ((decPlannedPerson_sum - decInputPerson_sum) * (Convert.ToDecimal(strCycleTime) / 60)).ToString("#0.00");
                }
                return dtPageInfo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable setPackingPlanInfo(string strPackPlant, string strHosDate, string strBanZhi, DataSet dsPackingPlan_summary,
            DataTable dtPageInfo, ref string strPeopleNum, ref string strCycleTime, ref string strWorkOverTime)
        {
            try
            {
                DataTable dtHead = dsPackingPlan_summary.Tables[0];
                DataTable dtBody = dsPackingPlan_summary.Tables[1];
                //完善小品目列表
                dtBody = setPutIntoOver_samll(dtPageInfo, dtBody, null);
                //获取大品目列表
                DataSet dsInPutIntoOver_small = new DataSet();
                dsInPutIntoOver_small.Tables.Add(dtHead.Copy());
                dsInPutIntoOver_small.Tables.Add(dtBody.Copy());
                dtPageInfo = setPageInfo(false, strPackPlant, strHosDate, strBanZhi, dsInPutIntoOver_small,
                    dtPageInfo, ref strPeopleNum, ref strCycleTime, ref strWorkOverTime);
                return dtPageInfo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable setPutIntoOver_samll(DataTable dtPageInfo, DataTable dtBody, DataTable dtImport)
        {
            try
            {
                //完善小品目列表
                for (int i = 0; i < dtPageInfo.Rows.Count; i++)
                {
                    string strBigPM = dtPageInfo.Rows[i]["vcBigPM"].ToString();
                    int iPackNum_summary = 0;
                    decimal decPlannedTime_summary = 0;
                    decimal decPlannedPerson_summary = 0;
                    DataRow[] drBody = dtBody.Select("vcBigPM='" + strBigPM + "'");
                    DataTable dtBody_clone = dtBody.Clone();
                    for (int oo = 0; oo < drBody.Length; oo++)
                    {
                        dtBody_clone.ImportRow(drBody[oo]);
                    }
                    for (int j = 0; j < dtBody_clone.Rows.Count; j++)
                    {
                        string strSmallPM = dtBody_clone.Rows[j]["vcSmallPM"].ToString();
                        iPackNum_summary += Convert.ToInt32(dtBody_clone.Rows[j]["iPackNum"].ToString());
                        decPlannedTime_summary += Convert.ToDecimal(dtBody_clone.Rows[j]["decPlannedTime"].ToString());
                        decPlannedPerson_summary += Convert.ToDecimal(dtBody_clone.Rows[j]["decPlannedPerson"].ToString());
                        foreach (DataRow dr in dtBody_clone.Rows)
                        {
                            dr["iPackNum_summary"] = iPackNum_summary;
                            dr["decPlannedTime_summary"] = decPlannedTime_summary;
                            dr["decPlannedPerson_summary"] = decPlannedPerson_summary;
                        }
                        if (dtImport != null)
                        {
                            DataRow[] drImport = dtImport.Select("vcBigPM='" + strBigPM + "'");
                            if (drImport.Length != 0)
                            {
                                dtBody_clone.Rows[j]["decInputPerson_summary"] = drImport[0]["decInputPerson"].ToString();
                            }
                        }
                    }
                    decimal decRatio = 0;
                    decimal decInputPerson_Ratio = 0;
                    for (int j = 0; j < dtBody_clone.Rows.Count; j++)
                    {
                        decimal decPlannedPerson_pro = Convert.ToDecimal(dtBody_clone.Rows[j]["decPlannedPerson"].ToString());
                        decimal decPlannedPerson_summary_pro = Convert.ToDecimal(dtBody_clone.Rows[j]["decPlannedPerson_summary"].ToString());
                        decimal decPlannedPerson_ratio = Convert.ToDecimal(Convert.ToDecimal("0").ToString("#0.0000"));
                        if (decPlannedPerson_summary_pro != 0)
                        { decPlannedPerson_ratio = Convert.ToDecimal(Convert.ToDecimal(decPlannedPerson_pro / decPlannedPerson_summary_pro).ToString("#0.0000")); }


                        decimal decInputPerson_summary_pro = Convert.ToDecimal(dtBody_clone.Rows[j]["decInputPerson_summary"].ToString() == "" ? "0" : dtBody_clone.Rows[j]["decInputPerson_summary"].ToString());
                        decimal decInputPerson_pro = Convert.ToDecimal(dtBody_clone.Rows[j]["decInputPerson"].ToString() == "" ? "0" : dtBody_clone.Rows[j]["decInputPerson"].ToString());
                        if (j != dtBody_clone.Rows.Count - 1)
                        {
                            decRatio += decPlannedPerson_ratio;
                            if (dtImport != null)
                            {
                                decInputPerson_pro = Convert.ToDecimal(Convert.ToDecimal(decInputPerson_summary_pro * decPlannedPerson_ratio).ToString("#0.0000"));
                                decInputPerson_Ratio += decInputPerson_pro;
                            }
                        }
                        else
                        {
                            decPlannedPerson_ratio = 1 - decRatio;
                            if (dtImport != null)
                            {
                                decInputPerson_pro = decInputPerson_summary_pro - decInputPerson_Ratio;
                            }
                        }
                        dtBody_clone.Rows[j]["decPlannedPerson_ratio"] = decPlannedPerson_ratio;
                        if (dtImport != null)
                        {
                            dtBody_clone.Rows[j]["decInputPerson"] = decInputPerson_pro;
                        }
                    }
                    for (int j = 0; j < dtBody_clone.Rows.Count; j++)
                    {
                        string strBigPM_check = dtBody_clone.Rows[j]["vcBigPM"].ToString();
                        string strSmallPM_check = dtBody_clone.Rows[j]["vcSmallPM"].ToString();
                        for (int jj = 0; jj < dtBody.Rows.Count; jj++)
                        {
                            if (dtBody.Rows[jj]["vcBigPM"].ToString() == strBigPM_check && dtBody.Rows[jj]["vcSmallPM"].ToString() == strSmallPM_check)
                            {
                                dtBody.Rows[jj]["iPackNum_summary"] = dtBody_clone.Rows[j]["iPackNum_summary"];
                                dtBody.Rows[jj]["decPlannedTime_summary"] = Convert.ToDecimal(dtBody_clone.Rows[j]["decPlannedTime_summary"]).ToString("#0.00");
                                dtBody.Rows[jj]["decPlannedPerson_summary"] = Convert.ToDecimal(dtBody_clone.Rows[j]["decPlannedPerson_summary"]).ToString("#0.00");
                                dtBody.Rows[jj]["decPlannedPerson_ratio"] = dtBody_clone.Rows[j]["decPlannedPerson_ratio"];
                                dtBody.Rows[jj]["decInputPerson_summary"] = Convert.ToDecimal(dtBody_clone.Rows[j]["decInputPerson_summary"]).ToString("#0.00");
                                dtBody.Rows[jj]["decInputPerson"] = dtBody_clone.Rows[j]["decInputPerson"];
                            }
                        }
                    }
                }
                return dtBody;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable setPointStateInfo(DataTable dtPageInfo, string strPackPlant)
        {
            DataTable dtPointStateInfo = fs0811_DataAccess.getPointState(strPackPlant);
            for (int i = 0; i < dtPageInfo.Rows.Count; i++)
            {
                decimal decSysLander = 0;
                decimal decDiffer = 0;
                decimal decInputPerson = Convert.ToDecimal(dtPageInfo.Rows[i]["decInputPerson"].ToString() == "" ? "0" : dtPageInfo.Rows[i]["decInputPerson"].ToString());
                string strBigPM = dtPageInfo.Rows[i]["vcBigPM"].ToString();
                DataRow[] drPointStateInfo = dtPointStateInfo.Select("vcBigPM='" + strBigPM + "'");
                if (drPointStateInfo.Length != 0)
                {
                    decSysLander = Convert.ToDecimal(drPointStateInfo[0]["decSysLander"].ToString());
                }
                decDiffer = decSysLander - decInputPerson;
                dtPageInfo.Rows[i]["decSysLander"] = decSysLander.ToString("#0.00");
                dtPageInfo.Rows[i]["decDiffer"] = decDiffer.ToString("#0.00");
            }
            return dtPageInfo;
        }
        public DataTable checkSaveData(List<Dictionary<string, Object>> listInfoData, string strPeopleNum, string strCycleTime, ref string strWorkOverTime, ref DataTable dtMessage)
        {
            try
            {
                DataTable dtImport = fS0603_Logic.createTable("Query811");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    DataRow drRowinfo = dtImport.NewRow();
                    drRowinfo["vcPackPlant"] = listInfoData[i]["vcPackPlant"] == null ? "" : listInfoData[i]["vcPackPlant"].ToString();
                    drRowinfo["dHosDate"] = listInfoData[i]["dHosDate"] == null ? "" : listInfoData[i]["dHosDate"].ToString();
                    drRowinfo["vcBanZhi"] = listInfoData[i]["vcBanZhi"] == null ? "" : listInfoData[i]["vcBanZhi"].ToString();
                    drRowinfo["vcBigPM"] = listInfoData[i]["vcBigPM"] == null ? "" : listInfoData[i]["vcBigPM"].ToString();
                    drRowinfo["decPackNum"] = listInfoData[i]["decPackNum"] == null ? "" : listInfoData[i]["decPackNum"].ToString();
                    drRowinfo["decPlannedTime"] = listInfoData[i]["decPlannedTime"] == null ? "" : listInfoData[i]["decPlannedTime"].ToString();
                    drRowinfo["decPlannedPerson"] = listInfoData[i]["decPlannedPerson"] == null ? "" : listInfoData[i]["decPlannedPerson"].ToString();
                    drRowinfo["decInputPerson"] = listInfoData[i]["decInputPerson"] == null ? "0" : listInfoData[i]["decInputPerson"].ToString();
                    drRowinfo["decInputTime"] = listInfoData[i]["decInputTime"] == null ? "" : listInfoData[i]["decInputTime"].ToString();
                    drRowinfo["decOverFlowTime"] = listInfoData[i]["decOverFlowTime"] == null ? "" : listInfoData[i]["decOverFlowTime"].ToString();
                    dtImport.Rows.Add(drRowinfo);
                }
                string strPackPlant = string.Empty;
                string strHosDate = string.Empty;
                string strBanZhi = string.Empty;
                for (int i = 0; i < dtImport.Rows.Count; i++)
                {
                    strPackPlant = dtImport.Rows[i]["vcPackPlant"].ToString();
                    strHosDate = dtImport.Rows[i]["dHosDate"].ToString();
                    strBanZhi = dtImport.Rows[i]["vcBanZhi"].ToString();
                    string strBigPM = dtImport.Rows[i]["vcBigPM"].ToString();
                    string strInputPerson = dtImport.Rows[i]["decInputPerson"].ToString();
                    if (strBigPM == "")
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = "存在未匹配成功的品目信息";
                        dtMessage.Rows.Add(dataRow);
                    }
                    else
                    {
                        if (strPackPlant == string.Empty || strHosDate == string.Empty || strBanZhi == string.Empty)
                        {
                            DataRow dataRow = dtMessage.NewRow();
                            dataRow["vcMessage"] = strBigPM + "班值信息为空，请确认";
                            dtMessage.Rows.Add(dataRow);
                        }
                    }
                    if (strInputPerson == "" || strInputPerson == "0")
                    {
                        dtImport.Rows[i]["decInputPerson"] = "0";
                    }
                }
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                    return null;
                //读取小品目别信息并用于计算
                int iCycleTime_mi = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(strCycleTime)).ToString());
                DataTable dtPageInfo = fs0811_DataAccess.getPageDataInfo();
                DataSet dsPackingPlan_summary = fs0811_DataAccess.getPackingPlan_summary(strPackPlant, strHosDate, strBanZhi, strPeopleNum, iCycleTime_mi);
                if (dsPackingPlan_summary != null && dsPackingPlan_summary.Tables[1].Rows.Count != 0)
                {
                    //创建页面大品目别绑定datatable
                    DataTable dtHead = dsPackingPlan_summary.Tables[0];
                    DataTable dtBody = dsPackingPlan_summary.Tables[1];
                    //完善小品目列表
                    dtBody = setPutIntoOver_samll(dtPageInfo, dtBody, dtImport);
                    DataSet dsPackingPlan_summary_clone = new DataSet();
                    dsPackingPlan_summary_clone.Tables.Add(dtHead.Copy());
                    dsPackingPlan_summary_clone.Tables.Add(dtBody.Copy());
                    //页面录入赋值
                    dtPageInfo = setPageInfo(true, strPackPlant, strHosDate, strBanZhi, dsPackingPlan_summary_clone,
                        dtPageInfo, ref strPeopleNum, ref strCycleTime, ref strWorkOverTime);
                    //完善dtBody数据
                    for (int i = 0; i < dtPageInfo.Rows.Count; i++)
                    {
                        string strBigPM = dtPageInfo.Rows[i]["vcBigPM"].ToString();
                        string decInputTime = dtPageInfo.Rows[i]["decInputTime"].ToString();
                        string decOverFlowTime = dtPageInfo.Rows[i]["decOverFlowTime"].ToString();
                        for (int ii = 0; ii < dtBody.Rows.Count; ii++)
                        {
                            if (strBigPM == dtBody.Rows[ii]["vcBigPM"].ToString())
                            {
                                dtBody.Rows[ii]["decInputTime"] = decInputTime;
                                dtBody.Rows[ii]["decOverFlowTime"] = decOverFlowTime;
                            }
                        }
                    }
                    return dtBody;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void queryData(string strPackPlant, string strHosDate, string strBanZhi, DataTable dtImport,
            string vcPeopleNum, string vcCycleTime, string vcWorkOverTime, string strOperId, ref DataTable dtMessage)
        {
            try
            {
                //保存临时表
                fs0811_DataAccess.setInPutIntoOver_small_temp(strPackPlant, strHosDate, strBanZhi, dtImport,
                 vcPeopleNum, vcCycleTime, vcWorkOverTime, strOperId, ref dtMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setInPutIntoOverInfo(string strPackPlant, string strHosDate, string strBanZhi, ref DataTable dtMessage)
        {
            fs0811_DataAccess.setInPutIntoOverInfo(strPackPlant, strHosDate, strBanZhi, ref dtMessage);
        }
        public DataTable getDayRef(ref DataTable dtMessage)
        {
            try
            {
                DataTable dtDayRef = new DataTable();
                return dtDayRef;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getMonthRef(ref DataTable dtMessage)
        {
            try
            {
                DataTable dtDayRef = new DataTable();
                return dtDayRef;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //============================================报表业务=============================================
        public DataTable getMonthPaper(string strPackPlant, string strMonth, ref DataTable dtMessage)
        {
            try
            {
                int iDateNum = Convert.ToDateTime(strMonth.Replace("/", "-") + "-01").AddMonths(1).AddDays(-1).Day;
                //创建日报表结构
                DataTable dtDailyPaper_M = fs0811_DataAccess.createMonthPaper();
                for (int iNum = 1; iNum <= iDateNum; iNum++)
                {
                    string strHosDate = strMonth + "/" + (100 + iNum).ToString().Substring(1, 2);

                    DataTable dtDailyPaper = getDailyPaper(strPackPlant, strHosDate, ref dtMessage);
                    if(dtDailyPaper_M.Rows.Count== dtDailyPaper.Rows.Count)
                    {
                        for (int i = 0; i < dtDailyPaper_M.Rows.Count; i++)
                        {
                            dtDailyPaper_M.Rows[i]["A"] = Convert.ToInt32(dtDailyPaper_M.Rows[i]["A"].ToString()) + Convert.ToInt32(dtDailyPaper.Rows[i]["A"].ToString());
                            dtDailyPaper_M.Rows[i]["B"] = Convert.ToInt32(dtDailyPaper_M.Rows[i]["B"].ToString()) + Convert.ToInt32(dtDailyPaper.Rows[i]["B"].ToString());
                            dtDailyPaper_M.Rows[i]["C"] = Convert.ToInt32(dtDailyPaper_M.Rows[i]["C"].ToString()) + Convert.ToInt32(dtDailyPaper.Rows[i]["C"].ToString());
                            dtDailyPaper_M.Rows[i]["D"] = Convert.ToInt32(dtDailyPaper_M.Rows[i]["D"].ToString()) + Convert.ToInt32(dtDailyPaper.Rows[i]["D"].ToString());
                            dtDailyPaper_M.Rows[i]["E"] = Convert.ToInt32(dtDailyPaper_M.Rows[i]["E"].ToString()) + Convert.ToInt32(dtDailyPaper.Rows[i]["E"].ToString());
                            dtDailyPaper_M.Rows[i]["F"] = Convert.ToDecimal(dtDailyPaper_M.Rows[i]["F"].ToString()) + Convert.ToDecimal(dtDailyPaper.Rows[i]["F"].ToString());
                            dtDailyPaper_M.Rows[i]["G"] = Convert.ToDecimal(dtDailyPaper_M.Rows[i]["G"].ToString()) + Convert.ToDecimal(dtDailyPaper.Rows[i]["G"].ToString());
                            dtDailyPaper_M.Rows[i]["H"] = Convert.ToDecimal(dtDailyPaper_M.Rows[i]["H"].ToString()) + Convert.ToDecimal(dtDailyPaper.Rows[i]["H"].ToString());
                            dtDailyPaper_M.Rows[i]["I"] = Convert.ToDecimal(dtDailyPaper_M.Rows[i]["I"].ToString()) + Convert.ToDecimal(dtDailyPaper.Rows[i]["I"].ToString());
                            dtDailyPaper_M.Rows[i]["L"] = Convert.ToDecimal(dtDailyPaper_M.Rows[i]["L"].ToString()) + Convert.ToDecimal(dtDailyPaper.Rows[i]["L"].ToString());
                            dtDailyPaper_M.Rows[i]["J"] = "0.00%";
                            if (Convert.ToDecimal(dtDailyPaper_M.Rows[i]["G"].ToString()) != 0)
                            {
                                dtDailyPaper_M.Rows[i]["J"] = (Convert.ToDecimal(dtDailyPaper_M.Rows[i]["I"].ToString()) * 100 / Convert.ToDecimal(dtDailyPaper_M.Rows[i]["G"].ToString())).ToString("#0.00") + "%";
                            }
                            dtDailyPaper_M.Rows[i]["K"] = "0.00%";
                            if (Convert.ToDecimal(dtDailyPaper_M.Rows[i]["H"].ToString()) != 0)
                            {
                                dtDailyPaper_M.Rows[i]["K"] = (Convert.ToDecimal(dtDailyPaper_M.Rows[i]["I"].ToString()) * 100 / Convert.ToDecimal(dtDailyPaper_M.Rows[i]["H"].ToString())).ToString("#0.00") + "%";
                            }
                        }
                    }
                }
                dtDailyPaper_M.Columns.Add("vcMonth");
                for (int i = 0; i < dtDailyPaper_M.Rows.Count; i++)
                {
                    dtDailyPaper_M.Rows[i]["vcMonth"] = strMonth;
                }

                return dtDailyPaper_M;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getDailyPaper(string strPackPlant, string strHosDate, ref DataTable dtMessage)
        {
            try
            {
                //创建日报表结构
                DataTable dtDailyPaper = fs0811_DataAccess.createDailyPaper();
                DataTable dtDailyPaper_T = dtDailyPaper.Clone();
                //获取当天出勤班值信息
                DataTable dtCalendarInfo = fs0811_DataAccess.getDailyCalendarInfo(strHosDate);
                foreach (DataRow drCalendarInfo in dtCalendarInfo.Rows)
                {
                    DataTable dtDailyPaper_A = fs0811_DataAccess.createDailyPaper();
                    //if (drCalendarInfo["vcDay"].ToString() == "A")
                    if (drCalendarInfo["vcDay"].ToString() != string.Empty)
                    {
                        string strBanZhi = drCalendarInfo["vcType"].ToString();
                        //获取品目别实绩数据
                        DataTable dtDailyList_00 = fs0811_DataAccess.getDailyList_00(strPackPlant, strHosDate, strBanZhi);
                        decimal decH_total = 0;

                        for (int i = 0; i < dtDailyList_00.Rows.Count; i++)
                        {
                            string strPartItem = dtDailyList_00.Rows[i]["vcPartItem"].ToString();
                            foreach (DataRow drDailyPaper_A in dtDailyPaper_A.Rows)
                            {
                                if (drDailyPaper_A["vcPartItem"].ToString() == strPartItem)
                                {
                                    drDailyPaper_A["A"] = dtDailyList_00.Rows[i]["A"].ToString();//包装计划
                                    drDailyPaper_A["B"] = dtDailyList_00.Rows[i]["B"].ToString();//实行计划
                                    drDailyPaper_A["C"] = dtDailyList_00.Rows[i]["C"].ToString();//入荷实绩
                                    drDailyPaper_A["D"] = dtDailyList_00.Rows[i]["D"].ToString();//包装实绩
                                    drDailyPaper_A["E"] = dtDailyList_00.Rows[i]["E"].ToString();//包装差额
                                    drDailyPaper_A["F"] = dtDailyList_00.Rows[i]["F"].ToString();//包装计划工时(H)
                                    drDailyPaper_A["G"] = dtDailyList_00.Rows[i]["G"].ToString();//实行计划工时(H)
                                    //品目别实绩在线工时SS
                                    drDailyPaper_A["H"] = 0;//实绩在线工时(H)
                                    decimal[] decOnline = getOperOnLineInfo(strPackPlant, strHosDate, strBanZhi, strPartItem);
                                    if (decOnline[0] == 0)
                                    {
                                        if (decOnline[1] != 0)
                                        {
                                            drDailyPaper_A["H"] = decOnline[1] / Convert.ToDecimal(3600.00);
                                        }
                                    }
                                    if (strPartItem != "合计")
                                    {
                                        decH_total = decH_total + Convert.ToDecimal(drDailyPaper_A["H"].ToString());
                                    }
                                    else
                                    {
                                        drDailyPaper_A["H"] = decH_total;
                                    }
                                    drDailyPaper_A["I"] = dtDailyList_00.Rows[i]["I"].ToString(); //实绩包装工时(H)
                                    drDailyPaper_A["J"] = "0.00%";//工时完成率
                                    if (Convert.ToDecimal(drDailyPaper_A["G"].ToString()) != 0)
                                    {
                                        drDailyPaper_A["J"] = (Convert.ToDecimal(drDailyPaper_A["I"].ToString()) * 100 / Convert.ToDecimal(drDailyPaper_A["G"].ToString())).ToString("#0.00") + "%";
                                    }
                                    drDailyPaper_A["K"] = "0.00%";//作业效率
                                    if (Convert.ToDecimal(drDailyPaper_A["H"].ToString()) != 0)
                                    {
                                        drDailyPaper_A["K"] = (Convert.ToDecimal(drDailyPaper_A["I"].ToString()) * 100 / Convert.ToDecimal(drDailyPaper_A["H"].ToString())).ToString("#0.00") + "%";
                                    }
                                    drDailyPaper_A["L"] = Convert.ToDecimal(drDailyPaper_A["G"].ToString()) - Convert.ToDecimal(drDailyPaper_A["I"].ToString());//作业工时差

                                    break;
                                }
                            }
                        }
                    }
                    //将处理后的表附加dtDailyPaper下
                    foreach (DataRow drDailyPaper_A in dtDailyPaper_A.Rows)
                    {
                        dtDailyPaper_T.ImportRow(drDailyPaper_A);
                    }
                }
                //统计合计值
                dtDailyPaper_T = getHeJiInfo(dtDailyPaper_T, dtDailyPaper, ref dtMessage);

                dtDailyPaper_T.Columns.Add("vcHosDate");
                for (int i = 0; i < dtDailyPaper_T.Rows.Count; i++)
                {
                    dtDailyPaper_T.Rows[i]["vcHosDate"] = strHosDate;
                }

                return dtDailyPaper_T;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public decimal[] getOperOnLineInfo(string strPackPlant, string strHosDate, string strBanZhi, string strPointName)
        {
            try
            {
                decimal[] vs = new decimal[2];
                //获取休息阶段、点位登录履历、操作人当日完成基准时间
                DataSet dsOperPointInfo = fs0811_DataAccess.getOperPointInfo(strPackPlant, strBanZhi, strHosDate, strPointName);
                if (dsOperPointInfo == null)
                {
                    vs[0] = -2;//点位信息获取失败--报错显示
                    return vs;
                }
                if (dsOperPointInfo.Tables[0].Rows.Count == 0)
                {
                    vs[0] = -3;//当值休息时间获取失败--报错显示
                    return vs;
                }
                if (dsOperPointInfo.Tables[1].Rows.Count == 0)
                {
                    vs[0] = 0;//当值点位履历获取失败--报错显示
                    vs[1] = 0;
                    return vs;
                }
                //点位在线有效时间（ss）
                decimal decOnLine = getOnLineDetails(dsOperPointInfo.Tables[1], dsOperPointInfo.Tables[0]);
                vs[0] = 0;
                vs[1] = decOnLine;
                return vs;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private decimal getOnLineDetails(DataTable dtPointDetails, DataTable dtRest)
        {
            try
            {
                DataTable dtPointDetails_Temp = dtPointDetails.Clone();
                dtPointDetails_Temp.Columns.Add("iOnLine");
                for (int i = 0; i < dtPointDetails.Rows.Count; i++)
                {
                    DataTable dtRest_Temp = dtRest.Clone();
                    string strHosDate = dtPointDetails.Rows[i]["dHosDate"].ToString();
                    string strPackPlant = dtPointDetails.Rows[i]["vcPackPlant"].ToString();
                    string strBanZhi = dtPointDetails.Rows[i]["vcBanZhi"].ToString();
                    string strPointNo = dtPointDetails.Rows[i]["vcPointNo"].ToString();
                    string strUUID = dtPointDetails.Rows[i]["UUID"].ToString();
                    string strEntryTime = dtPointDetails.Rows[i]["dEntryTime"].ToString();
                    string strDestroyTime = dtPointDetails.Rows[i]["dDestroyTime"].ToString();
                    if (Convert.ToDateTime(strEntryTime) >= Convert.ToDateTime(strDestroyTime))
                    {
                        DataRow drPointDetails_Temp = dtPointDetails_Temp.NewRow();
                        drPointDetails_Temp["dHosDate"] = strHosDate;
                        drPointDetails_Temp["vcPackPlant"] = strPackPlant;
                        drPointDetails_Temp["vcBanZhi"] = strBanZhi;
                        drPointDetails_Temp["vcPointNo"] = strPointNo;
                        drPointDetails_Temp["UUID"] = strUUID;
                        drPointDetails_Temp["dEntryTime"] = "1900-01-01";
                        drPointDetails_Temp["dDestroyTime"] = "1900-01-01";
                        dtPointDetails_Temp.Rows.Add(drPointDetails_Temp);
                    }
                    else
                    {
                        //判断开始结束是否是在休息范围内
                        DataRow[] drRest_00 = dtRest.Select("vcPackPlant='" + strPackPlant + "' and vcBanZhi='" + strBanZhi + "' and tBeforTime<='" + strEntryTime + "' and tLastTime>='" + strDestroyTime + "'");
                        if (drRest_00.Length != 0)
                        {
                            DataRow drPointDetails_Temp = dtPointDetails_Temp.NewRow();
                            drPointDetails_Temp["dHosDate"] = strHosDate;
                            drPointDetails_Temp["vcPackPlant"] = strPackPlant;
                            drPointDetails_Temp["vcBanZhi"] = strBanZhi;
                            drPointDetails_Temp["vcPointNo"] = strPointNo;
                            drPointDetails_Temp["UUID"] = strUUID;
                            drPointDetails_Temp["dEntryTime"] = "1900-01-01";
                            drPointDetails_Temp["dDestroyTime"] = "1900-01-01";
                            dtPointDetails_Temp.Rows.Add(drPointDetails_Temp);
                        }
                        else
                        {
                            //判断开始在休息时间之间
                            DataRow[] drRest_20 = dtRest.Select("vcPackPlant='" + strPackPlant + "' and vcBanZhi='" + strBanZhi + "' and tBeforTime<='" + strEntryTime + "' and tLastTime>='" + strEntryTime + "'");
                            if (drRest_20.Length != 0)
                            {
                                strEntryTime = drRest_20[0]["tLastTime"].ToString();
                            }
                            //判断开始在休息时间之前
                            DataRow[] drRest_10 = dtRest.Select("vcPackPlant='" + strPackPlant + "' and vcBanZhi='" + strBanZhi + "' and tBeforTime>='" + strEntryTime + "'");
                            if (drRest_10.Length != 0)
                            {
                                for (int j = 0; j < drRest_10.Length; j++)
                                {
                                    DataRow drRest_Temp = dtRest_Temp.NewRow();
                                    drRest_Temp["TANK"] = drRest_10[j]["TANK"];
                                    drRest_Temp["vcPackPlant"] = drRest_10[j]["vcPackPlant"];
                                    drRest_Temp["vcBanZhi"] = drRest_10[j]["vcBanZhi"];
                                    drRest_Temp["tBeforTime"] = drRest_10[j]["tBeforTime"];
                                    drRest_Temp["tLastTime"] = drRest_10[j]["tLastTime"];
                                    drRest_Temp["iMinute"] = drRest_10[j]["iMinute"];
                                    dtRest_Temp.Rows.Add(drRest_Temp);
                                }
                                dtRest_Temp.DefaultView.Sort = "TANK ASC";
                                dtRest_Temp = dtRest_Temp.DefaultView.ToTable();
                                int iTANK = Convert.ToInt32(dtRest_Temp.Rows[0]["TANK"].ToString());
                                //判断结束在休息时间中
                                DataRow[] drRest_11 = dtRest_Temp.Select("vcPackPlant='" + strPackPlant + "' and vcBanZhi='" + strBanZhi + "' and tBeforTime<='" + strDestroyTime + "' and tLastTime>='" + strDestroyTime + "'");
                                if (drRest_11.Length != 0)
                                {
                                    int iTANK_11 = Convert.ToInt32(drRest_11[drRest_11.Length - 1]["TANK"].ToString());
                                    for (int j = 0; j < iTANK_11 - iTANK + 1; j++)
                                    {
                                        string strEntryTime_11 = "";
                                        string strDestroyTime_11 = "";
                                        if (j == 0)
                                            strEntryTime_11 = strEntryTime;
                                        else
                                            strEntryTime_11 = dtRest_Temp.Rows[j - 1]["tLastTime"].ToString();

                                        if (j == 0)
                                            strDestroyTime_11 = dtRest_Temp.Rows[j]["tBeforTime"].ToString();
                                        //else
                                        //if (iTANK_11 - iTANK + 1 - 1 == j)
                                        //    strDestroyTime_11 = dtRest_Temp.Rows[j]["tBeforTime"].ToString();
                                        else
                                            strDestroyTime_11 = dtRest_Temp.Rows[j]["tBeforTime"].ToString();


                                        DataRow drPointDetails_Temp = dtPointDetails_Temp.NewRow();
                                        drPointDetails_Temp["dHosDate"] = strHosDate;
                                        drPointDetails_Temp["vcPackPlant"] = strPackPlant;
                                        drPointDetails_Temp["vcBanZhi"] = strBanZhi;
                                        drPointDetails_Temp["vcPointNo"] = strPointNo;
                                        drPointDetails_Temp["UUID"] = strUUID;
                                        drPointDetails_Temp["dEntryTime"] = strEntryTime_11;
                                        drPointDetails_Temp["dDestroyTime"] = strDestroyTime_11;
                                        dtPointDetails_Temp.Rows.Add(drPointDetails_Temp);
                                    }
                                }
                                else
                                {
                                    //判断结束在休息时间间
                                    DataRow[] drRest_12 = dtRest_Temp.Select("vcPackPlant='" + strPackPlant + "' and vcBanZhi='" + strBanZhi + "' and tLastTime<'" + strDestroyTime + "'");
                                    if (drRest_12.Length != 0)
                                    {
                                        for (int j = 0; j < drRest_12.Length; j++)
                                        {
                                            string strEntryTime_11 = "1900-01-01";
                                            string strDestroyTime_11 = "1900-01-01";
                                            if (j == 0)
                                            {
                                                strEntryTime_11 = strEntryTime;
                                                strDestroyTime_11 = drRest_12[j]["tBeforTime"].ToString();
                                                DataRow drPointDetails_Temp_1 = dtPointDetails_Temp.NewRow();
                                                drPointDetails_Temp_1["dHosDate"] = strHosDate;
                                                drPointDetails_Temp_1["vcPackPlant"] = strPackPlant;
                                                drPointDetails_Temp_1["vcBanZhi"] = strBanZhi;
                                                drPointDetails_Temp_1["vcPointNo"] = strPointNo;
                                                drPointDetails_Temp_1["UUID"] = strUUID;
                                                drPointDetails_Temp_1["dEntryTime"] = strEntryTime_11;
                                                drPointDetails_Temp_1["dDestroyTime"] = strDestroyTime_11;
                                                dtPointDetails_Temp.Rows.Add(drPointDetails_Temp_1);
                                            }
                                            if (j == drRest_12.Length - 1)
                                            {
                                                strEntryTime_11 = drRest_12[j]["tLastTime"].ToString();
                                                strDestroyTime_11 = strDestroyTime;
                                                DataRow drPointDetails_Temp_1 = dtPointDetails_Temp.NewRow();
                                                drPointDetails_Temp_1["dHosDate"] = strHosDate;
                                                drPointDetails_Temp_1["vcPackPlant"] = strPackPlant;
                                                drPointDetails_Temp_1["vcBanZhi"] = strBanZhi;
                                                drPointDetails_Temp_1["vcPointNo"] = strPointNo;
                                                drPointDetails_Temp_1["UUID"] = strUUID;
                                                drPointDetails_Temp_1["dEntryTime"] = strEntryTime_11;
                                                drPointDetails_Temp_1["dDestroyTime"] = strDestroyTime_11;
                                                dtPointDetails_Temp.Rows.Add(drPointDetails_Temp_1);
                                            }
                                            else
                                            {
                                                strEntryTime_11 = drRest_12[j]["tLastTime"].ToString();
                                                strDestroyTime_11 = drRest_12[j + 1]["tBeforTime"].ToString();
                                                DataRow drPointDetails_Temp_1 = dtPointDetails_Temp.NewRow();
                                                drPointDetails_Temp_1["dHosDate"] = strHosDate;
                                                drPointDetails_Temp_1["vcPackPlant"] = strPackPlant;
                                                drPointDetails_Temp_1["vcBanZhi"] = strBanZhi;
                                                drPointDetails_Temp_1["vcPointNo"] = strPointNo;
                                                drPointDetails_Temp_1["UUID"] = strUUID;
                                                drPointDetails_Temp_1["dEntryTime"] = strEntryTime_11;
                                                drPointDetails_Temp_1["dDestroyTime"] = strDestroyTime_11;
                                                dtPointDetails_Temp.Rows.Add(drPointDetails_Temp_1);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        DataRow drPointDetails_Temp = dtPointDetails_Temp.NewRow();
                                        drPointDetails_Temp["dHosDate"] = strHosDate;
                                        drPointDetails_Temp["vcPackPlant"] = strPackPlant;
                                        drPointDetails_Temp["vcBanZhi"] = strBanZhi;
                                        drPointDetails_Temp["vcPointNo"] = strPointNo;
                                        drPointDetails_Temp["UUID"] = strUUID;
                                        drPointDetails_Temp["dEntryTime"] = strEntryTime;
                                        drPointDetails_Temp["dDestroyTime"] = strDestroyTime;
                                        dtPointDetails_Temp.Rows.Add(drPointDetails_Temp);
                                    }
                                }
                            }

                        }
                    }
                }
                decimal decOnLine = 0;
                for (int i = 0; i < dtPointDetails_Temp.Rows.Count; i++)
                {
                    DateTime dEntryTime = Convert.ToDateTime(dtPointDetails_Temp.Rows[i]["dEntryTime"].ToString());
                    DateTime dDestroyTime = Convert.ToDateTime(dtPointDetails_Temp.Rows[i]["dDestroyTime"].ToString());
                    TimeSpan timeSpan = dDestroyTime.Subtract(dEntryTime);
                    double secInterval = timeSpan.TotalSeconds;
                    decOnLine = decOnLine + Convert.ToDecimal(secInterval);
                }
                return decOnLine;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getHeJiInfo(DataTable dtDailyPaper_T, DataTable dtDailyPaper, ref DataTable dtMessage)
        {
            try
            {
                foreach (DataRow drDailyPaper in dtDailyPaper.Rows)
                {
                    string strPartItem = drDailyPaper["vcPartItem"].ToString();
                    DataRow[] drDailyPaper_T = dtDailyPaper_T.Select("vcPartItem='" + strPartItem + "'");
                    int A = 0;//包装计划
                    int B = 0;//实行计划
                    int C = 0;//入荷实绩
                    int D = 0;//包装实绩
                    int E = 0;//包装差额
                    decimal F = 0;//包装计划工时(H)
                    decimal G = 0;//实行计划工时(H)
                    decimal H = 0;//实绩在线工时(H)
                    decimal I = 0;//实绩包装工时(H)
                    string J = "0.00%";//工时完成率
                    string K = "0.00%";//作业效率
                    decimal L = 0;//作业工时差
                    if (drDailyPaper_T.Length != 0)
                    {
                        for (int i = 0; i < drDailyPaper_T.Length; i++)
                        {
                            A = A + Convert.ToInt32(drDailyPaper_T[i]["A"].ToString());
                            B = B + Convert.ToInt32(drDailyPaper_T[i]["B"].ToString());
                            C = C + Convert.ToInt32(drDailyPaper_T[i]["C"].ToString());
                            D = D + Convert.ToInt32(drDailyPaper_T[i]["D"].ToString());
                            E = E + Convert.ToInt32(drDailyPaper_T[i]["E"].ToString());
                            F = F + Convert.ToDecimal(drDailyPaper_T[i]["F"].ToString());
                            G = G + Convert.ToDecimal(drDailyPaper_T[i]["G"].ToString());
                            H = H + Convert.ToDecimal(drDailyPaper_T[i]["H"].ToString());
                            I = I + Convert.ToDecimal(drDailyPaper_T[i]["I"].ToString());
                            L = L + Convert.ToDecimal(drDailyPaper_T[i]["L"].ToString());
                        }
                    }
                    if (Convert.ToDecimal(G) != 0)
                    {
                        J = (Convert.ToDecimal(I) * 100 / Convert.ToDecimal(G)).ToString("#0.00") + "%";
                    }
                    if (Convert.ToDecimal(H) != 0)
                    {
                        K = (Convert.ToDecimal(I) * 100 / Convert.ToDecimal(H)).ToString("#0.00") + "%";
                    }
                    drDailyPaper["A"] = A;
                    drDailyPaper["B"] = B;
                    drDailyPaper["C"] = C;
                    drDailyPaper["D"] = D;
                    drDailyPaper["E"] = E;
                    drDailyPaper["F"] = F;
                    drDailyPaper["G"] = G;
                    drDailyPaper["H"] = H;
                    drDailyPaper["I"] = I;
                    drDailyPaper["L"] = L;
                    drDailyPaper["J"] = J;
                    drDailyPaper["K"] = K;
                }
                //将处理后的表附加dtDailyPaper下
                foreach (DataRow drDailyPaper in dtDailyPaper.Rows)
                {
                    dtDailyPaper_T.ImportRow(drDailyPaper);
                }
                return dtDailyPaper_T;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
