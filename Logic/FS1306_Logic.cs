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
                DataTable dtBanZhi = fs0811_DataAccess.getNowBZInfo(strPackingPlant);
                string strHosDate = dtBanZhi.Rows[0]["dHosDate"].ToString();
                string strBanZhi = dtBanZhi.Rows[0]["vcBanZhi"].ToString();
                string strFromTime = dtBanZhi.Rows[0]["tFromTime"].ToString();
                string strToTime = dtBanZhi.Rows[0]["tToTime"].ToString();
                string strBZFromTime = strHosDate + " " + strFromTime;
                strOverTime_plan = "0.00";
                strOverTime_now = "0.00";
                DataTable dtList = fS0603_Logic.createTable("KanBan1306");

                DataTable dtDataInfo = fs1306_DataAccess.getDataInfo(strPackingPlant, strHosDate, strBanZhi, strBZFromTime, ref dtMessage);
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
                dtList = setInfo(dtDataInfo, dtList, ref strOverTime_plan, ref strOverTime_now);
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
                dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["T"] = (decR_sum * 100 / decS_sum).ToString("#.00");
                dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AI"] = (decR_sum * 100 / decS_sum).ToString("#.00");
            }
            if (decI_sum != 0)
            {
                dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["U"] = (decR_sum * 100 / (decI_sum * 60)).ToString("#.00");
                dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["AJ"] = (decR_sum * 100 / (decI_sum * 60)).ToString("#.00");
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
            strOverTime_plan = Convert.ToDecimal(dtDataInfo.Rows[0]["D"].ToString() == "" ? "0" : dtDataInfo.Rows[0]["D"].ToString()).ToString("#.00");
            decimal decPeopleNum = Convert.ToDecimal(dtDataInfo.Rows[0]["C"].ToString() == "" ? "0" : dtDataInfo.Rows[0]["C"].ToString());
            decimal decHeJ_X = Convert.ToDecimal(dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["X"].ToString() == "" ? "0" : dtDataInfo.Rows[dtDataInfo.Rows.Count - 1]["X"].ToString());
            if (decHeJ_X > 0)
            {
                if (decPeopleNum != 0)
                    strOverTime_now = Convert.ToDecimal(decHeJ_X / decPeopleNum).ToString("#.00");
            }

            return dtList;
        }

    }
}
