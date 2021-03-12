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
        public Dictionary<string, object> setLoadPage(Dictionary<string, object> res, string strPackingPlant, ref string type, ref int code)
        {
            try
            {
                DataTable dtBanZhi = getNowBZInfo(strPackingPlant);
                if (dtBanZhi == null || dtBanZhi.Rows.Count == 0)
                {
                    code = ComConstant.ERROR_CODE;
                    type = "e2";
                    return res;
                }
                string strHosDate = dtBanZhi.Rows[0]["dHosDate"].ToString();
                string strBanZhi = dtBanZhi.Rows[0]["vcBanZhi"].ToString();
                string strFromTime = dtBanZhi.Rows[0]["tFromTime"].ToString();
                string strToTime = dtBanZhi.Rows[0]["tToTime"].ToString();
                //数据列表表头
                string strBZ = strHosDate + "(" + strBanZhi + ")";
                res.Add("BZItem", strBZ);
                DataSet dataSet = getLoadData(strPackingPlant, strHosDate, strBanZhi);
                if (dataSet == null)
                {
                    code = ComConstant.ERROR_CODE;
                    type = "e3";
                    return res;
                }

                //页面条件
                string strPeopleNum = dataSet.Tables[0].Rows[0]["vcPeopleNum"].ToString();
                string strCycleTime = dataSet.Tables[0].Rows[0]["vcCycleTime"].ToString();
                string strWorkOverTime = dataSet.Tables[0].Rows[0]["vcWorkOverTime"].ToString();
                //数据列表
                DataTable dataTable = dataSet.Tables[1];
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("bSelectFlag", ConvertFieldType.BoolType, null);
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dataTable, dtConverter);

                //uuid
                //string uuid = dataSet.Tables[0].Rows[0]["uuid"].ToString();
                res.Add("PeopleNumItem", strPeopleNum);
                res.Add("CycleTimeItem", strCycleTime);
                res.Add("WorkOverTimeItem", strWorkOverTime);
                res.Add("dataList", dataList);
                //res.Add("uuidItem", uuid);
                code = ComConstant.SUCCESS_CODE;
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Dictionary<string, object> setTempPage(Dictionary<string, object> res, string strPackingPlant, string strHosDate, string strBanZhi, ref string type, ref int code)
        {
            try
            {
                string strBZ = strHosDate + "(" + strBanZhi + ")";
                res.Add("BZItem", strBZ);
                DataSet dataSet = getTempData(strPackingPlant, strHosDate, strBanZhi);
                if (dataSet == null)
                {
                    code = ComConstant.ERROR_CODE;
                    type = "e3";
                    return res;
                }

                //页面条件
                string strPeopleNum = dataSet.Tables[0].Rows[0]["vcPeopleNum"].ToString();
                string strCycleTime = dataSet.Tables[0].Rows[0]["vcCycleTime"].ToString();
                string strWorkOverTime = dataSet.Tables[0].Rows[0]["vcWorkOverTime"].ToString();
                //数据列表
                DataTable dataTable = dataSet.Tables[1];
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("bSelectFlag", ConvertFieldType.BoolType, null);
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dataTable, dtConverter);

                //uuid
                //string uuid = dataSet.Tables[0].Rows[0]["uuid"].ToString();
                res.Add("PeopleNumItem", strPeopleNum);
                res.Add("CycleTimeItem", strCycleTime);
                res.Add("WorkOverTimeItem", strWorkOverTime);
                res.Add("dataList", dataList);
                //res.Add("uuidItem", uuid);
                code = ComConstant.SUCCESS_CODE;
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getNowBZInfo(string strPackPlant)
        {
            return fs0811_DataAccess.getNowBZInfo(strPackPlant);
        }
        public DataSet getLoadData(string strPackPlant, string strHosDate, string strBanZhi)
        {
            //查询人员投入表数据
            DataSet dsInPutIntoOver = fs0811_DataAccess.searchInPutIntoOver(strPackPlant, strHosDate, strBanZhi);
            if (dsInPutIntoOver != null && dsInPutIntoOver.Tables[0].Rows.Count != 0)
            {
                //设置登录人情况
                dsInPutIntoOver = setPointStateInfo(dsInPutIntoOver, strPackPlant);
                return dsInPutIntoOver;
            }
            //查询人员投入表数据
            DataSet dsPackingPlan = fs0811_DataAccess.searchPackingPlan(strPackPlant, strHosDate, strBanZhi);
            if (dsInPutIntoOver != null && dsPackingPlan.Tables[1].Rows.Count != 0)
            {
                //设置登录人情况
                dsPackingPlan = setPointStateInfo(dsPackingPlan, strPackPlant);
                return dsPackingPlan;
            }
            return null;
        }
        public DataSet getTempData(string strPackPlant, string strHosDate, string strBanZhi)
        {
            //查询人员投入表数据
            DataSet dsInPutIntoOver = fs0811_DataAccess.searchInPutIntoOver_temp(strPackPlant, strHosDate, strBanZhi);
            if (dsInPutIntoOver != null && dsInPutIntoOver.Tables[0].Rows.Count != 0)
            {
                //设置登录人情况
                dsInPutIntoOver = setPointStateInfo(dsInPutIntoOver, strPackPlant);
                return dsInPutIntoOver;
            }
            return null;
        }
        public DataSet setPointStateInfo(DataSet dsInfo, string strPackPlant)
        {
            DataTable dtPointStateInfo = fs0811_DataAccess.getPointState(strPackPlant);
            DataTable dtMain = dsInfo.Tables[0];
            DataTable dtList = dsInfo.Tables[1];
            for (int i = 0; i < dtList.Rows.Count; i++)
            {
                decimal decSysLander = 0;
                decimal decDiffer = 0;
                decimal decInputPerson = Convert.ToDecimal(dtList.Rows[i]["decInputPerson"].ToString() == "" ? "0" : dtList.Rows[i]["decInputPerson"].ToString());
                string strBigPM = dtList.Rows[i]["vcBigPM"].ToString();
                DataRow[] drPointStateInfo = dtPointStateInfo.Select("vcBigPM='" + strBigPM + "'");
                if (drPointStateInfo.Length != 0)
                {
                    decSysLander = Convert.ToDecimal(drPointStateInfo[0]["decSysLander"].ToString());
                }
                decDiffer = decSysLander - decInputPerson;
                dsInfo.Tables[1].Rows[i]["decSysLander"] = decSysLander.ToString("#0.00");
                dsInfo.Tables[1].Rows[i]["decDiffer"] = decDiffer.ToString("#0.00");
            }

            return dsInfo;
        }

        public DataTable checkSaveData(List<Dictionary<string, Object>> listInfoData, string strPeopleNum, string strCycleTime, ref string strWorkOverTime, ref DataTable dtMessage)
        {
            try
            {
                DataTable dtImport = fS0603_Logic.createTable("Query811");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    DataRow drRowinfo = dtImport.NewRow();
                    drRowinfo["vcBigPM"] = listInfoData[i]["vcBigPM"] == null ? "" : listInfoData[i]["vcBigPM"].ToString();
                    drRowinfo["decPackNum"] = listInfoData[i]["decPackNum"] == null ? "" : listInfoData[i]["decPackNum"].ToString();
                    drRowinfo["decPlannedTime"] = listInfoData[i]["decPlannedTime"] == null ? "" : listInfoData[i]["decPlannedTime"].ToString();
                    drRowinfo["decPlannedPerson"] = listInfoData[i]["decPlannedPerson"] == null ? "" : listInfoData[i]["decPlannedPerson"].ToString();
                    drRowinfo["decInputPerson"] = listInfoData[i]["decInputPerson"] == null ? "" : listInfoData[i]["decInputPerson"].ToString();
                    drRowinfo["decInputTime"] = listInfoData[i]["decInputTime"] == null ? "" : listInfoData[i]["decInputTime"].ToString();
                    drRowinfo["decOverFlowTime"] = listInfoData[i]["decOverFlowTime"] == null ? "" : listInfoData[i]["decOverFlowTime"].ToString();
                    dtImport.Rows.Add(drRowinfo);
                }
                for (int i = 0; i < dtImport.Rows.Count; i++)
                {
                    string strBigPM = dtImport.Rows[i]["vcBigPM"].ToString();
                    string strInputPerson = dtImport.Rows[i]["decInputPerson"].ToString();
                    if (strBigPM == "")
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = "存在未匹配成功的品目信息";
                        dtMessage.Rows.Add(dataRow);
                    }
                    if (strInputPerson == "" || strInputPerson == "0")
                    {
                        DataRow dataRow = dtMessage.NewRow();
                        dataRow["vcMessage"] = string.Format("品目{0}的人员投入数录入有误)", strBigPM);
                        dtMessage.Rows.Add(dataRow);
                    }
                }
                if (dtMessage != null && dtMessage.Rows.Count != 0)
                    return null;
                //计算"投入人员"	"投入工时（小时）"	"过或不足（小时）"
                decimal decPeopleNum = Convert.ToDecimal(strPeopleNum);//最大包装持有人数C2
                decimal decPlannedPerson_sum = 0;//计划人数合计F22
                decimal decInputPerson_sum = 0;//投入人数合计
                for (int i = 0; i < dtImport.Rows.Count; i++)
                {
                    decimal decPlannedPerson = Convert.ToDecimal(dtImport.Rows[i]["decPlannedPerson"].ToString());
                    decimal decInputPerson = Convert.ToDecimal(dtImport.Rows[i]["decInputPerson"].ToString());
                    decimal decInputTime = decInputPerson * (Convert.ToDecimal(strCycleTime) / 60);
                    decimal decPlannedTime = Convert.ToDecimal(dtImport.Rows[i]["decPlannedTime"].ToString());
                    decimal decOverFlowTime = decInputTime - decPlannedTime;
                    //decPeopleNum = decPeopleNum + decInputPerson;
                    decPlannedPerson_sum = decPlannedPerson_sum + decPlannedPerson;
                    decInputPerson_sum = decInputPerson_sum + decInputPerson;
                    //登录人员获取并计算最后两列decSysLander、decDiffer

                    dtImport.Rows[i]["decInputPerson"] = decInputPerson.ToString("#0.00");
                    dtImport.Rows[i]["decInputTime"] = decInputTime.ToString("#0.00");
                    dtImport.Rows[i]["decOverFlowTime"] = decOverFlowTime.ToString("#0.00");
                    dtImport.Rows[i]["decSysLander"] = "0.00";
                    dtImport.Rows[i]["decDiffer"] = "0.00";
                }
                decimal decWorkOverTime = 0;//计划人均加班小时数
                if (decPlannedPerson_sum > decPeopleNum)
                {
                    decWorkOverTime = ((decPlannedPerson_sum - decInputPerson_sum) * (Convert.ToDecimal(strCycleTime)) * 60) / decPeopleNum;
                }
                strWorkOverTime = decWorkOverTime < 0 ? "0" : decWorkOverTime.ToString();
                return dtImport;
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
                DataTable dtSaveInfo = fs0811_DataAccess.searchPackingPlan_small(strPackPlant, strHosDate, strBanZhi);
                for (int i = 0; i < dtSaveInfo.Rows.Count; i++)
                {
                    string strBigPM = dtSaveInfo.Rows[i]["vcBigPM"].ToString();
                    DataRow[] drSaveInfo = dtImport.Select("vcBigPM='" + strBigPM + "'");
                    if (drSaveInfo.Length != 0)
                    {
                        dtSaveInfo.Rows[i]["decPlannedTime"] = drSaveInfo[0]["decPlannedTime"].ToString();
                        dtSaveInfo.Rows[i]["decPlannedPerson"] = drSaveInfo[0]["decPlannedPerson"].ToString();
                        dtSaveInfo.Rows[i]["decInputPerson"] = drSaveInfo[0]["decInputPerson"].ToString();
                        dtSaveInfo.Rows[i]["decInputTime"] = drSaveInfo[0]["decInputTime"].ToString();
                        dtSaveInfo.Rows[i]["decOverFlowTime"] = drSaveInfo[0]["decOverFlowTime"].ToString();
                    }
                }
                //保存临时表
                fs0811_DataAccess.setInPutIntoOverInfo_temp(strPackPlant, strHosDate, strBanZhi, dtSaveInfo,
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

    }
}
