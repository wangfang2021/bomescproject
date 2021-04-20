using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using System.Collections;

namespace Logic
{
    public class FS1306_Logic
    {
        FS1306_DataAccess fs1306_DataAccess;
        FS0811_DataAccess fs0811_DataAccess;
        FS0603_Logic fS0603_Logic = new FS0603_Logic();

        public FS1306_Logic()
        {
            fs1306_DataAccess = new FS1306_DataAccess();
            fs0811_DataAccess = new FS0811_DataAccess();
        }
        public DataTable getDataInfo(string strPackingPlant, ref string strOverTime_plan, ref string strOverTime_now, ref DataTable dtMessage)
        {
            try
            {
                #region //获取统计时间
                string strHosDate = string.Empty;
                string strBanZhi = string.Empty;
                DateTime dFromTime = Convert.ToDateTime(System.DateTime.Now.Year.ToString() + "-" + System.DateTime.Now.Month.ToString() + "-" + System.DateTime.Now.AddDays(1).Day.ToString() + " 00:00:00");
                DateTime dToTime = Convert.ToDateTime(System.DateTime.Now.Year.ToString() + "-" + System.DateTime.Now.Month.ToString() + "-" + System.DateTime.Now.AddDays(1).Day.ToString() + " 00:00:00");
                string strBZFromTime = "";
                decimal decRest = 0;

                DataTable dtTime = fs0811_DataAccess.getBanZhiTime(strPackingPlant, "2");
                if (dtTime.Rows.Count != 0)
                {
                    strHosDate = dtTime.Rows[0]["dHosDate"].ToString();
                    strBanZhi = dtTime.Rows[0]["vcBanZhi"].ToString();
                    dFromTime = Convert.ToDateTime(dtTime.Rows[0]["tFromTime_nw"].ToString());
                    dToTime = Convert.ToDateTime(dtTime.Rows[0]["tToTime_nw"].ToString());
                    decRest = Convert.ToDecimal(dtTime.Rows[0]["iRest"].ToString());
                    strBZFromTime = dFromTime.ToString();
                }
                #endregion
                #region //创建显示数据列表、计划人均加班时间、现时点加班时间
                strOverTime_plan = "0.00";
                strOverTime_now = "0.00";
                DataTable dtList = fS0603_Logic.createTable("KanBan1306");
                #endregion
                #region //获取值别人员统计和Web共用
                DataTable dtDataInfo = fs1306_DataAccess.getDataInfo(strPackingPlant, strHosDate, strBanZhi, decRest, strBZFromTime, ref dtMessage);
                if (dtDataInfo == null || dtDataInfo.Rows.Count == 0)
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "看板数据异常，无法显示";
                    dtMessage.Rows.Add(dataRow);
                    return dtList;
                }
                if (dtDataInfo.Rows[0]["C"].ToString() == "")
                {
                    DataRow dataRow = dtMessage.NewRow();
                    dataRow["vcMessage"] = "每值人员投入未进行计算，显示数据为空";
                    dtMessage.Rows.Add(dataRow);
                    return dtList;
                }
                #endregion

                #region //构建大屏显示数据集合
                dtList = setDataInfo(dtDataInfo, dtList, ref strOverTime_plan, ref strOverTime_now);
                #endregion
                return dtList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable setInfo(DataTable dtDataInfo, DataTable dtList, ref string strOverTime_plan, ref string strOverTime_now)
        {
            for (int i = 0; i < dtDataInfo.Rows.Count; i++)
            {
                dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["H"] = Convert.ToDecimal(dtDataInfo.Rows[i]["H"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["H"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["H"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["H"].ToString());
                dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["I"] = Convert.ToDecimal(dtDataInfo.Rows[i]["I"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["I"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["I"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["I"].ToString());
                dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["J"] = Convert.ToDecimal(dtDataInfo.Rows[i]["J"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["J"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["J"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["J"].ToString());
                dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["K"] = Convert.ToDecimal(dtDataInfo.Rows[i]["K"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["K"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["K"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["K"].ToString());
                dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["L"] = Convert.ToDecimal(dtDataInfo.Rows[i]["L"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["L"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["L"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["L"].ToString());
                dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["M"] = Convert.ToDecimal(dtDataInfo.Rows[i]["M"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["M"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["M"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["M"].ToString());
                dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["N"] = Convert.ToDecimal(dtDataInfo.Rows[i]["N"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["N"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["N"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["N"].ToString());
                dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["O"] = Convert.ToDecimal(dtDataInfo.Rows[i]["O"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["O"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["O"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["O"].ToString());
                dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["P"] = Convert.ToDecimal(dtDataInfo.Rows[i]["P"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["P"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["P"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["P"].ToString());
                dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["Q"] = Convert.ToDecimal(dtDataInfo.Rows[i]["Q"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["Q"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["Q"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["Q"].ToString());
                dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["R"] = Convert.ToDecimal(dtDataInfo.Rows[i]["R"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["R"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["R"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["R"].ToString());
                dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["S"] = Convert.ToDecimal(dtDataInfo.Rows[i]["S"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["S"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["S"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["S"].ToString());
                dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["V"] = Convert.ToDecimal(dtDataInfo.Rows[i]["V"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["V"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["V"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["V"].ToString());
                dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["W"] = Convert.ToDecimal(dtDataInfo.Rows[i]["W"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["W"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["W"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["W"].ToString());
                dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["X"] = Convert.ToDecimal(dtDataInfo.Rows[i]["X"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["X"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["X"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["X"].ToString());
                dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AA"] = Convert.ToDecimal(dtDataInfo.Rows[i]["AA"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["AA"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AA"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AA"].ToString());
                dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AB"] = Convert.ToDecimal(dtDataInfo.Rows[i]["AB"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["AB"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AB"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AB"].ToString());
                dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AC"] = Convert.ToDecimal(dtDataInfo.Rows[i]["AC"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["AC"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AC"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AC"].ToString());
                dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AD"] = Convert.ToDecimal(dtDataInfo.Rows[i]["AD"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["AD"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AD"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AD"].ToString());
                dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AE"] = Convert.ToDecimal(dtDataInfo.Rows[i]["AE"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["AE"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AE"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AE"].ToString());
                dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AF"] = Convert.ToDecimal(dtDataInfo.Rows[i]["AF"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["AF"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AF"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AF"].ToString());
                dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AG"] = Convert.ToDecimal(dtDataInfo.Rows[i]["AG"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["AG"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AG"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AG"].ToString());
                dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AH"] = Convert.ToDecimal(dtDataInfo.Rows[i]["AH"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["AH"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AH"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AH"].ToString());
            }
            decimal decR_sum = Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["R"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["R"].ToString());
            decimal decS_sum = Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["S"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["S"].ToString());
            decimal decI_sum = Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["I"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["I"].ToString());
            dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["T"] = "0.00";
            dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AI"] = "0.00";
            if (decS_sum != 0)
            {
                dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["T"] = (decR_sum * 100 / decS_sum).ToString("#0.00");
                dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AI"] = (decR_sum * 100 / decS_sum).ToString("#0.00");
            }
            if (decI_sum != 0)
            {
                dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["U"] = (decR_sum * 100 / (decI_sum * 60)).ToString("#0.00");
                dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AJ"] = (decR_sum * 100 / (decI_sum * 60)).ToString("#0.00");
            }
            #region 赋值
            dtList.Rows[0]["vcDeng"] = dtDataInfo.Rows[0]["AA"].ToString();
            dtList.Rows[0]["vcZhongxw"] = dtDataInfo.Rows[1]["AA"].ToString();
            dtList.Rows[0]["vcDaw"] = dtDataInfo.Rows[2]["AA"].ToString();
            dtList.Rows[0]["vcBol"] = dtDataInfo.Rows[3]["AA"].ToString();
            dtList.Rows[0]["vcBanj"] = dtDataInfo.Rows[4]["AA"].ToString();
            dtList.Rows[0]["vcChengx"] = dtDataInfo.Rows[5]["AA"].ToString();
            dtList.Rows[0]["vcHej"] = dtDataInfo.Rows[6]["AA"].ToString();

            dtList.Rows[1]["vcDeng"] = dtDataInfo.Rows[0]["AB"].ToString();
            dtList.Rows[1]["vcZhongxw"] = dtDataInfo.Rows[1]["AB"].ToString();
            dtList.Rows[1]["vcDaw"] = dtDataInfo.Rows[2]["AB"].ToString();
            dtList.Rows[1]["vcBol"] = dtDataInfo.Rows[3]["AB"].ToString();
            dtList.Rows[1]["vcBanj"] = dtDataInfo.Rows[4]["AB"].ToString();
            dtList.Rows[1]["vcChengx"] = dtDataInfo.Rows[5]["AB"].ToString();
            dtList.Rows[1]["vcHej"] = dtDataInfo.Rows[6]["AB"].ToString();

            dtList.Rows[2]["vcDeng"] = dtDataInfo.Rows[0]["AC"].ToString();
            dtList.Rows[2]["vcZhongxw"] = dtDataInfo.Rows[1]["AC"].ToString();
            dtList.Rows[2]["vcDaw"] = dtDataInfo.Rows[2]["AC"].ToString();
            dtList.Rows[2]["vcBol"] = dtDataInfo.Rows[3]["AC"].ToString();
            dtList.Rows[2]["vcBanj"] = dtDataInfo.Rows[4]["AC"].ToString();
            dtList.Rows[2]["vcChengx"] = dtDataInfo.Rows[5]["AC"].ToString();
            dtList.Rows[2]["vcHej"] = dtDataInfo.Rows[6]["AC"].ToString();

            dtList.Rows[3]["vcDeng"] = dtDataInfo.Rows[0]["AD"].ToString();
            dtList.Rows[3]["vcZhongxw"] = dtDataInfo.Rows[1]["AD"].ToString();
            dtList.Rows[3]["vcDaw"] = dtDataInfo.Rows[2]["AD"].ToString();
            dtList.Rows[3]["vcBol"] = dtDataInfo.Rows[3]["AD"].ToString();
            dtList.Rows[3]["vcBanj"] = dtDataInfo.Rows[4]["AD"].ToString();
            dtList.Rows[3]["vcChengx"] = dtDataInfo.Rows[5]["AD"].ToString();
            dtList.Rows[3]["vcHej"] = dtDataInfo.Rows[6]["AD"].ToString();

            dtList.Rows[4]["vcDeng"] = dtDataInfo.Rows[0]["AE"].ToString();
            dtList.Rows[4]["vcZhongxw"] = dtDataInfo.Rows[1]["AE"].ToString();
            dtList.Rows[4]["vcDaw"] = dtDataInfo.Rows[2]["AE"].ToString();
            dtList.Rows[4]["vcBol"] = dtDataInfo.Rows[3]["AE"].ToString();
            dtList.Rows[4]["vcBanj"] = dtDataInfo.Rows[4]["AE"].ToString();
            dtList.Rows[4]["vcChengx"] = dtDataInfo.Rows[5]["AE"].ToString();
            dtList.Rows[4]["vcHej"] = dtDataInfo.Rows[6]["AE"].ToString();

            dtList.Rows[5]["vcDeng"] = dtDataInfo.Rows[0]["AF"].ToString();
            dtList.Rows[5]["vcZhongxw"] = dtDataInfo.Rows[1]["AF"].ToString();
            dtList.Rows[5]["vcDaw"] = dtDataInfo.Rows[2]["AF"].ToString();
            dtList.Rows[5]["vcBol"] = dtDataInfo.Rows[3]["AF"].ToString();
            dtList.Rows[5]["vcBanj"] = dtDataInfo.Rows[4]["AF"].ToString();
            dtList.Rows[5]["vcChengx"] = dtDataInfo.Rows[5]["AF"].ToString();
            dtList.Rows[5]["vcHej"] = dtDataInfo.Rows[6]["AF"].ToString();

            dtList.Rows[6]["vcDeng"] = dtDataInfo.Rows[0]["AG"].ToString();
            dtList.Rows[6]["vcZhongxw"] = dtDataInfo.Rows[1]["AG"].ToString();
            dtList.Rows[6]["vcDaw"] = dtDataInfo.Rows[2]["AG"].ToString();
            dtList.Rows[6]["vcBol"] = dtDataInfo.Rows[3]["AG"].ToString();
            dtList.Rows[6]["vcBanj"] = dtDataInfo.Rows[4]["AG"].ToString();
            dtList.Rows[6]["vcChengx"] = dtDataInfo.Rows[5]["AG"].ToString();
            dtList.Rows[6]["vcHej"] = dtDataInfo.Rows[6]["AG"].ToString();

            dtList.Rows[7]["vcDeng"] = dtDataInfo.Rows[0]["AH"].ToString();
            dtList.Rows[7]["vcZhongxw"] = dtDataInfo.Rows[1]["AH"].ToString();
            dtList.Rows[7]["vcDaw"] = dtDataInfo.Rows[2]["AH"].ToString();
            dtList.Rows[7]["vcBol"] = dtDataInfo.Rows[3]["AH"].ToString();
            dtList.Rows[7]["vcBanj"] = dtDataInfo.Rows[4]["AH"].ToString();
            dtList.Rows[7]["vcChengx"] = dtDataInfo.Rows[5]["AH"].ToString();
            dtList.Rows[7]["vcHej"] = dtDataInfo.Rows[6]["AH"].ToString();

            dtList.Rows[8]["vcDeng"] = dtDataInfo.Rows[0]["AI"].ToString() + "%";
            dtList.Rows[8]["vcZhongxw"] = dtDataInfo.Rows[1]["AI"].ToString() + "%";
            dtList.Rows[8]["vcDaw"] = dtDataInfo.Rows[2]["AI"].ToString() + "%";
            dtList.Rows[8]["vcBol"] = dtDataInfo.Rows[3]["AI"].ToString() + "%";
            dtList.Rows[8]["vcBanj"] = dtDataInfo.Rows[4]["AI"].ToString() + "%";
            dtList.Rows[8]["vcChengx"] = dtDataInfo.Rows[5]["AI"].ToString() + "%";
            dtList.Rows[8]["vcHej"] = dtDataInfo.Rows[6]["AI"].ToString() + "%";

            dtList.Rows[9]["vcDeng"] = dtDataInfo.Rows[0]["AJ"].ToString() + "%";
            dtList.Rows[9]["vcZhongxw"] = dtDataInfo.Rows[1]["AJ"].ToString() + "%";
            dtList.Rows[9]["vcDaw"] = dtDataInfo.Rows[2]["AJ"].ToString() + "%";
            dtList.Rows[9]["vcBol"] = dtDataInfo.Rows[3]["AJ"].ToString() + "%";
            dtList.Rows[9]["vcBanj"] = dtDataInfo.Rows[4]["AJ"].ToString() + "%";
            dtList.Rows[9]["vcChengx"] = dtDataInfo.Rows[5]["AJ"].ToString() + "%";
            dtList.Rows[9]["vcHej"] = dtDataInfo.Rows[6]["AJ"].ToString() + "%";
            #endregion
            strOverTime_plan = Convert.ToDecimal(dtDataInfo.Rows[0]["D"].ToString() == "" ? "0" : dtDataInfo.Rows[0]["D"].ToString()).ToString("#0.00");
            decimal decPeopleNum = Convert.ToDecimal(dtDataInfo.Rows[0]["C"].ToString() == "" ? "0" : dtDataInfo.Rows[0]["C"].ToString());
            decimal decHeJ_X = Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["X"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["X"].ToString());
            if (decHeJ_X > 0)
            {
                if (decPeopleNum != 0)
                    strOverTime_now = Convert.ToDecimal(decHeJ_X / decPeopleNum).ToString("#0.00");
            }

            return dtList;
        }
        public DataTable setDataInfo(DataTable dtDataInfo, DataTable dtList, ref string strOverTime_plan, ref string strOverTime_now)
        {
            try
            {
                for (int i = 0; i < dtDataInfo.Rows.Count - 1; i++)
                {
                    dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["H"] = Convert.ToDecimal(dtDataInfo.Rows[i]["H"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["H"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["H"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["H"].ToString());
                    dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["I"] = Convert.ToDecimal(dtDataInfo.Rows[i]["I"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["I"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["I"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["I"].ToString());
                    dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["J"] = Convert.ToDecimal(dtDataInfo.Rows[i]["J"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["J"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["J"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["J"].ToString());
                    dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["K"] = Convert.ToDecimal(dtDataInfo.Rows[i]["K"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["K"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["K"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["K"].ToString());
                    dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["L"] = Convert.ToDecimal(dtDataInfo.Rows[i]["L"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["L"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["L"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["L"].ToString());
                    dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["M"] = Convert.ToDecimal(dtDataInfo.Rows[i]["M"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["M"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["M"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["M"].ToString());
                    dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["N"] = Convert.ToDecimal(dtDataInfo.Rows[i]["N"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["N"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["N"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["N"].ToString());
                    dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["O"] = Convert.ToDecimal(dtDataInfo.Rows[i]["O"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["O"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["O"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["O"].ToString());
                    dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["P"] = Convert.ToDecimal(dtDataInfo.Rows[i]["P"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["P"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["P"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["P"].ToString());
                    dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["Q"] = Convert.ToDecimal(dtDataInfo.Rows[i]["Q"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["Q"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["Q"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["Q"].ToString());
                    dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["R"] = Convert.ToDecimal(dtDataInfo.Rows[i]["R"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["R"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["R"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["R"].ToString());
                    dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["S"] = Convert.ToDecimal(dtDataInfo.Rows[i]["S"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["S"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["S"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["S"].ToString());
                    dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["V"] = Convert.ToDecimal(dtDataInfo.Rows[i]["V"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["V"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["V"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["V"].ToString());
                    dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["W"] = Convert.ToDecimal(dtDataInfo.Rows[i]["W"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["W"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["W"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["W"].ToString());
                    dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["X"] = Convert.ToDecimal(dtDataInfo.Rows[i]["X"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["X"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["X"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["X"].ToString());
                    dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AA"] = Convert.ToDecimal(dtDataInfo.Rows[i]["AA"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["AA"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AA"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AA"].ToString());
                    dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AB"] = Convert.ToDecimal(dtDataInfo.Rows[i]["AB"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["AB"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AB"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AB"].ToString());
                    dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AC"] = Convert.ToDecimal(dtDataInfo.Rows[i]["AC"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["AC"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AC"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AC"].ToString());
                    dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AD"] = Convert.ToDecimal(dtDataInfo.Rows[i]["AD"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["AD"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AD"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AD"].ToString());
                    dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AE"] = Convert.ToDecimal(dtDataInfo.Rows[i]["AE"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["AE"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AE"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AE"].ToString());
                    dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AF"] = Convert.ToDecimal(dtDataInfo.Rows[i]["AF"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["AF"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AF"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AF"].ToString());
                    dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AG"] = Convert.ToDecimal(dtDataInfo.Rows[i]["AG"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["AG"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AG"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AG"].ToString());
                    dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AH"] = Convert.ToDecimal(dtDataInfo.Rows[i]["AH"].ToString() == "" ? "0" : dtDataInfo.Rows[i]["AH"].ToString()) + Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AH"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AH"].ToString());
                }
                decimal decR_sum = Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["R"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["R"].ToString());
                decimal decS_sum = Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["S"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["S"].ToString());
                decimal decI_sum = Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["I"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["I"].ToString());
                dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["T"] = "0.00";
                dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AI"] = "0.00";
                if (decS_sum != 0)
                {
                    dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["T"] = (decR_sum * 100 / decS_sum).ToString("#0.00");
                    dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AI"] = (decR_sum * 100 / decS_sum).ToString("#0.00");
                }
                if (decI_sum != 0)
                {
                    dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["U"] = (decR_sum * 100 / (decI_sum * 60)).ToString("#0.00");
                    dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AJ"] = (decR_sum * 100 / (decI_sum * 60)).ToString("#0.00");
                }
                #region 赋值
                dtList.Rows[0]["vcDeng"] = Math.Ceiling(Convert.ToDecimal(dtDataInfo.Rows[0]["AA"].ToString())).ToString();
                dtList.Rows[0]["vcZhongxw"] = Math.Ceiling(Convert.ToDecimal(dtDataInfo.Rows[1]["AA"].ToString())).ToString();
                dtList.Rows[0]["vcDaw"] = Math.Ceiling(Convert.ToDecimal(dtDataInfo.Rows[2]["AA"].ToString())).ToString();
                dtList.Rows[0]["vcBol"] = Math.Ceiling(Convert.ToDecimal(dtDataInfo.Rows[3]["AA"].ToString())).ToString();
                dtList.Rows[0]["vcBanj"] = Math.Ceiling(Convert.ToDecimal(dtDataInfo.Rows[4]["AA"].ToString())).ToString();
                dtList.Rows[0]["vcChengx"] = Math.Ceiling(Convert.ToDecimal(dtDataInfo.Rows[5]["AA"].ToString())).ToString();
                dtList.Rows[0]["vcHej"] = Math.Ceiling(Convert.ToDecimal(dtDataInfo.Rows[6]["AA"].ToString())).ToString();

                dtList.Rows[1]["vcDeng"] = dtDataInfo.Rows[0]["AB"].ToString();
                dtList.Rows[1]["vcZhongxw"] = dtDataInfo.Rows[1]["AB"].ToString();
                dtList.Rows[1]["vcDaw"] = dtDataInfo.Rows[2]["AB"].ToString();
                dtList.Rows[1]["vcBol"] = dtDataInfo.Rows[3]["AB"].ToString();
                dtList.Rows[1]["vcBanj"] = dtDataInfo.Rows[4]["AB"].ToString();
                dtList.Rows[1]["vcChengx"] = dtDataInfo.Rows[5]["AB"].ToString();
                dtList.Rows[1]["vcHej"] = dtDataInfo.Rows[6]["AB"].ToString();

                dtList.Rows[2]["vcDeng"] = dtDataInfo.Rows[0]["AC"].ToString();
                dtList.Rows[2]["vcZhongxw"] = dtDataInfo.Rows[1]["AC"].ToString();
                dtList.Rows[2]["vcDaw"] = dtDataInfo.Rows[2]["AC"].ToString();
                dtList.Rows[2]["vcBol"] = dtDataInfo.Rows[3]["AC"].ToString();
                dtList.Rows[2]["vcBanj"] = dtDataInfo.Rows[4]["AC"].ToString();
                dtList.Rows[2]["vcChengx"] = dtDataInfo.Rows[5]["AC"].ToString();
                dtList.Rows[2]["vcHej"] = dtDataInfo.Rows[6]["AC"].ToString();

                dtList.Rows[3]["vcDeng"] = dtDataInfo.Rows[0]["AD"].ToString();
                dtList.Rows[3]["vcZhongxw"] = dtDataInfo.Rows[1]["AD"].ToString();
                dtList.Rows[3]["vcDaw"] = dtDataInfo.Rows[2]["AD"].ToString();
                dtList.Rows[3]["vcBol"] = dtDataInfo.Rows[3]["AD"].ToString();
                dtList.Rows[3]["vcBanj"] = dtDataInfo.Rows[4]["AD"].ToString();
                dtList.Rows[3]["vcChengx"] = dtDataInfo.Rows[5]["AD"].ToString();
                dtList.Rows[3]["vcHej"] = dtDataInfo.Rows[6]["AD"].ToString();

                dtList.Rows[4]["vcDeng"] = Math.Ceiling(Convert.ToDecimal(dtDataInfo.Rows[0]["AE"].ToString())).ToString();
                dtList.Rows[4]["vcZhongxw"] = Math.Ceiling(Convert.ToDecimal(dtDataInfo.Rows[1]["AE"].ToString())).ToString();
                dtList.Rows[4]["vcDaw"] = Math.Ceiling(Convert.ToDecimal(dtDataInfo.Rows[2]["AE"].ToString())).ToString();
                dtList.Rows[4]["vcBol"] = Math.Ceiling(Convert.ToDecimal(dtDataInfo.Rows[3]["AE"].ToString())).ToString();
                dtList.Rows[4]["vcBanj"] = Math.Ceiling(Convert.ToDecimal(dtDataInfo.Rows[4]["AE"].ToString())).ToString();
                dtList.Rows[4]["vcChengx"] = Math.Ceiling(Convert.ToDecimal(dtDataInfo.Rows[5]["AE"].ToString())).ToString();
                dtList.Rows[4]["vcHej"] = Math.Ceiling(Convert.ToDecimal(dtDataInfo.Rows[6]["AE"].ToString())).ToString();

                dtList.Rows[5]["vcDeng"] = Math.Ceiling(Convert.ToDecimal(dtDataInfo.Rows[0]["AF"].ToString())).ToString();
                dtList.Rows[5]["vcZhongxw"] = Math.Ceiling(Convert.ToDecimal(dtDataInfo.Rows[1]["AF"].ToString())).ToString();
                dtList.Rows[5]["vcDaw"] = Math.Ceiling(Convert.ToDecimal(dtDataInfo.Rows[2]["AF"].ToString())).ToString();
                dtList.Rows[5]["vcBol"] = Math.Ceiling(Convert.ToDecimal(dtDataInfo.Rows[3]["AF"].ToString())).ToString();
                dtList.Rows[5]["vcBanj"] = Math.Ceiling(Convert.ToDecimal(dtDataInfo.Rows[4]["AF"].ToString())).ToString();
                dtList.Rows[5]["vcChengx"] = Math.Ceiling(Convert.ToDecimal(dtDataInfo.Rows[5]["AF"].ToString())).ToString();
                dtList.Rows[5]["vcHej"] = Math.Ceiling(Convert.ToDecimal(dtDataInfo.Rows[6]["AF"].ToString())).ToString();

                dtList.Rows[6]["vcDeng"] = Math.Ceiling(Convert.ToDecimal(dtDataInfo.Rows[0]["AG"].ToString())).ToString();
                dtList.Rows[6]["vcZhongxw"] = Math.Ceiling(Convert.ToDecimal(dtDataInfo.Rows[1]["AG"].ToString())).ToString();
                dtList.Rows[6]["vcDaw"] = Math.Ceiling(Convert.ToDecimal(dtDataInfo.Rows[2]["AG"].ToString())).ToString();
                dtList.Rows[6]["vcBol"] = Math.Ceiling(Convert.ToDecimal(dtDataInfo.Rows[3]["AG"].ToString())).ToString();
                dtList.Rows[6]["vcBanj"] = Math.Ceiling(Convert.ToDecimal(dtDataInfo.Rows[4]["AG"].ToString())).ToString();
                dtList.Rows[6]["vcChengx"] = Math.Ceiling(Convert.ToDecimal(dtDataInfo.Rows[5]["AG"].ToString())).ToString();
                dtList.Rows[6]["vcHej"] = Math.Ceiling(Convert.ToDecimal(dtDataInfo.Rows[6]["AG"].ToString())).ToString();

                dtList.Rows[7]["vcDeng"] = Math.Ceiling(Convert.ToDecimal(dtDataInfo.Rows[0]["AH"].ToString())).ToString();
                dtList.Rows[7]["vcZhongxw"] = Math.Ceiling(Convert.ToDecimal(dtDataInfo.Rows[1]["AH"].ToString())).ToString();
                dtList.Rows[7]["vcDaw"] = Math.Ceiling(Convert.ToDecimal(dtDataInfo.Rows[2]["AH"].ToString())).ToString();
                dtList.Rows[7]["vcBol"] = Math.Ceiling(Convert.ToDecimal(dtDataInfo.Rows[3]["AH"].ToString())).ToString();
                dtList.Rows[7]["vcBanj"] = Math.Ceiling(Convert.ToDecimal(dtDataInfo.Rows[4]["AH"].ToString())).ToString();
                dtList.Rows[7]["vcChengx"] = Math.Ceiling(Convert.ToDecimal(dtDataInfo.Rows[5]["AH"].ToString())).ToString();
                dtList.Rows[7]["vcHej"] = Math.Ceiling(Convert.ToDecimal(dtDataInfo.Rows[6]["AH"].ToString())).ToString();

                dtList.Rows[8]["vcDeng"] = dtDataInfo.Rows[0]["AI"].ToString() + "%";
                dtList.Rows[8]["vcZhongxw"] = dtDataInfo.Rows[1]["AI"].ToString() + "%";
                dtList.Rows[8]["vcDaw"] = dtDataInfo.Rows[2]["AI"].ToString() + "%";
                dtList.Rows[8]["vcBol"] = dtDataInfo.Rows[3]["AI"].ToString() + "%";
                dtList.Rows[8]["vcBanj"] = dtDataInfo.Rows[4]["AI"].ToString() + "%";
                dtList.Rows[8]["vcChengx"] = dtDataInfo.Rows[5]["AI"].ToString() + "%";
                dtList.Rows[8]["vcHej"] = dtDataInfo.Rows[6]["AI"].ToString() + "%";

                dtList.Rows[9]["vcDeng"] = dtDataInfo.Rows[0]["AJ"].ToString() + "%";
                dtList.Rows[9]["vcZhongxw"] = dtDataInfo.Rows[1]["AJ"].ToString() + "%";
                dtList.Rows[9]["vcDaw"] = dtDataInfo.Rows[2]["AJ"].ToString() + "%";
                dtList.Rows[9]["vcBol"] = dtDataInfo.Rows[3]["AJ"].ToString() + "%";
                dtList.Rows[9]["vcBanj"] = dtDataInfo.Rows[4]["AJ"].ToString() + "%";
                dtList.Rows[9]["vcChengx"] = dtDataInfo.Rows[5]["AJ"].ToString() + "%";
                dtList.Rows[9]["vcHej"] = dtDataInfo.Rows[6]["AJ"].ToString() + "%";
                #endregion
                strOverTime_plan = Convert.ToDecimal(dtDataInfo.Rows[0]["D"].ToString() == "" ? "0" : dtDataInfo.Rows[0]["D"].ToString()).ToString("#0.00");
                decimal decPeopleNum = Convert.ToDecimal(dtDataInfo.Rows[0]["C"].ToString() == "" ? "0" : dtDataInfo.Rows[0]["C"].ToString());
                decimal decHeJ_X = Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["X"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["X"].ToString());
                if (decHeJ_X > 0)
                {
                    if (decPeopleNum != 0)
                        strOverTime_now = Convert.ToDecimal(decHeJ_X / (decPeopleNum * 60)).ToString("#0.00");
                }

                return dtList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public decimal getOperEfficacyInfo(string strPackPlant, string strOperater, string strPointNo)
        {
            try
            {
                //获取当前班值信息
                DataTable dtBanZhiInfo = fs1306_DataAccess.getBanZhiTime(strPackPlant, "2");
                if (dtBanZhiInfo == null || dtBanZhiInfo.Rows.Count == 0)
                    return -1;//班值信息为空--报错显示
                string strHosDate = dtBanZhiInfo.Rows[0]["dHosDate"].ToString();
                string strBanZhi = dtBanZhiInfo.Rows[0]["vcBanZhi"].ToString();
                string strFromTime_nw = dtBanZhiInfo.Rows[0]["tFromTime_nw"].ToString();

                //获取休息阶段、点位登录履历、操作人当日完成基准时间
                DataSet dsOperPointInfo = fs1306_DataAccess.getOperPointInfo(strPackPlant, strBanZhi, strHosDate, strOperater, strFromTime_nw);
                if (dsOperPointInfo == null)
                    return -2;//点位信息获取失败--报错显示
                if (dsOperPointInfo.Tables[0].Rows.Count == 0)
                    return -3;//当值休息时间获取失败--报错显示
                if (dsOperPointInfo.Tables[1].Rows.Count == 0)
                    return -4;//当值点位履历获取失败--报错显示
                if (dsOperPointInfo.Tables[2].Rows.Count == 0)
                    return -5;//操作人当日完成基准时间获取失败--报错显示

                //点位完成基准时间（ss）
                decimal decOperStandard = Convert.ToDecimal(dsOperPointInfo.Tables[2].Rows[0]["decOperStandard"].ToString());
                //点位在线有效时间（ss）
                decimal decOnLine = getOnLineDetails(dsOperPointInfo.Tables[1], dsOperPointInfo.Tables[0]);
                decimal decOperEfficacy = 0;
                if (decOnLine > 0)
                    decOperEfficacy = decOperStandard / decOnLine;
                return decOperEfficacy;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public decimal getOnLineDetails(DataTable dtPointDetails, DataTable dtRest)
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
                    if (Convert.ToDateTime(strEntryTime) <= Convert.ToDateTime(strDestroyTime))
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
    }
}
