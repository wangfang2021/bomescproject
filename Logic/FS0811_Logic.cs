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
            DataSet dsPackingPlan_summary = fs0811_DataAccess.getPackingPlan_summary(strPackPlant, strHosDate, strBanZhi, iCycleTime_mi);
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
                        dtPageInfo.Rows[i]["decPackNum"] = Convert.ToDecimal(drBody[0]["iPackNum_summary"].ToString()) < 0 ? "0" : drBody[0]["iPackNum_summary"].ToString();
                        dtPageInfo.Rows[i]["decPlannedTime"] = Convert.ToDecimal(drBody[0]["decPlannedTime_summary"].ToString()) < 0 ? "0" : drBody[0]["decPlannedTime_summary"].ToString();
                        dtPageInfo.Rows[i]["decPlannedPerson"] = Convert.ToDecimal(drBody[0]["decPlannedPerson_summary"].ToString()) < 0 ? "0" : drBody[0]["decPlannedPerson_summary"].ToString();
                        dtPageInfo.Rows[i]["decInputPerson"] = Convert.ToDecimal(drBody[0]["decInputPerson_summary"].ToString()) < 0 ? "" : drBody[0]["decInputPerson_summary"].ToString();
                        dtPageInfo.Rows[i]["decInputTime"] = Convert.ToDecimal(drBody[0]["decInputTime"].ToString()) < 0 ? "0.00" : drBody[0]["decInputTime"].ToString();
                        dtPageInfo.Rows[i]["decOverFlowTime"] = Convert.ToDecimal(drBody[0]["decOverFlowTime"].ToString()) < 0 ? "0.00" : drBody[0]["decOverFlowTime"].ToString();
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
                    decimal decWorkOverTime = Convert.ToDecimal(((decPlannedPerson_sum - decInputPerson_sum) * (Convert.ToDecimal(strCycleTime) / 60)).ToString("#0.00"));
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
                            dr["decPlannedTime_summary"] = decPlannedTime_summary.ToString("#0.00");
                            dr["decPlannedPerson_summary"] = decPlannedPerson_summary.ToString("#0.00");
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
                        decimal decPlannedPerson_ratio = Convert.ToDecimal(Convert.ToDecimal(decPlannedPerson_pro / decPlannedPerson_summary_pro).ToString("#0.0000"));

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
                                dtBody.Rows[jj]["decPlannedTime_summary"] = dtBody_clone.Rows[j]["decPlannedTime_summary"];
                                dtBody.Rows[jj]["decPlannedPerson_summary"] = dtBody_clone.Rows[j]["decPlannedPerson_summary"];
                                dtBody.Rows[jj]["decPlannedPerson_ratio"] = dtBody_clone.Rows[j]["decPlannedPerson_ratio"];
                                dtBody.Rows[jj]["decInputPerson_summary"] = dtBody_clone.Rows[j]["decInputPerson_summary"];
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
                DataSet dsPackingPlan_summary = fs0811_DataAccess.getPackingPlan_summary(strPackPlant, strHosDate, strBanZhi, iCycleTime_mi);
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
    }
}
