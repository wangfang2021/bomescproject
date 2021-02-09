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
                DataSet dataSet = getLoadData(strPackingPlant, strHosDate, strBanZhi, strFromTime, strToTime);
                if (dataSet == null)
                {
                    code = ComConstant.ERROR_CODE;
                    type = "e3";
                    return res;
                }

                //页面条件
                string strPeopleNum = dataSet.Tables[0].Rows[0]["vcPeopleNum"].ToString();
                string strCycleTime = dataSet.Tables[0].Rows[0]["vcCycleTime"].ToString();
                string strObjective = dataSet.Tables[0].Rows[0]["vcObjective"].ToString();
                string strWorkOverTime = dataSet.Tables[0].Rows[0]["vcWorkOverTime"].ToString();
                //数据列表
                DataTable dataTable = dataSet.Tables[1];
                DtConverter dtConverter = new DtConverter();
                dtConverter.addField("bSelectFlag", ConvertFieldType.BoolType, null);
                List<Object> dataList = ComFunction.convertAllToResultByConverter(dataTable, dtConverter);

                //uuid
                string uuid = dataSet.Tables[0].Rows[0]["uuid"].ToString();
                res.Add("PeopleNumItem", strPeopleNum);
                res.Add("CycleTimeItem", strCycleTime);
                res.Add("ObjectiveItem", strObjective);
                res.Add("WorkOverTimeItem", strWorkOverTime);
                res.Add("dataList", dataList);
                res.Add("uuidItem", uuid);
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
        public DataSet getLoadData(string strPackPlant, string strHosDate, string strBanZhi, string strFromTime, string strToTime)
        {
            DataSet dataSet = new DataSet();
            dataSet = fs0811_DataAccess.getSearchInfo(strPackPlant, strHosDate, strBanZhi);
            if (dataSet != null && dataSet.Tables[1].Rows.Count != 0)
                return dataSet;
            else
            {
                dataSet = fs0811_DataAccess.getPackingPlanInfo(strPackPlant, strHosDate, strBanZhi);
                if (dataSet != null && dataSet.Tables[1].Rows.Count != 0)
                {
                    decimal decCycleTime = Convert.ToDecimal(dataSet.Tables[0].Rows[0]["vcCycleTime"].ToString());
                    for (int i = 0; i < dataSet.Tables[1].Rows.Count; i++)
                    {
                        decimal decStandard = Convert.ToDecimal(dataSet.Tables[1].Rows[i]["vcStandard"].ToString());
                        decimal decPackTotalNum = Convert.ToDecimal(dataSet.Tables[1].Rows[i]["decPackTotalNum"].ToString());
                        decimal decPlannedTime = (decStandard * decPackTotalNum) / 3600;
                        decimal decPlannedPerson = (decPlannedTime * 60) / decCycleTime;
                        dataSet.Tables[1].Rows[i]["decPlannedTime"] = decPlannedTime.ToString("#0.00");
                        dataSet.Tables[1].Rows[i]["decPlannedPerson"] = decPlannedPerson.ToString("#0.00");
                    }
                    return dataSet;
                }
                else
                    return null;
            }
        }
        public void setInPutIntoOverInfo(DataTable dtRowinfo,
            string uuid, string vcPeopleNum, string vcCycleTime, string vcObjective, string vcWorkOverTime,
            string strOperId, ref DataTable dtMessage)
        {
            fs0811_DataAccess.setInPutIntoOverInfo(dtRowinfo,
            uuid, vcPeopleNum, vcCycleTime, vcObjective, vcWorkOverTime,
            strOperId, ref dtMessage);
        }
    }
}
