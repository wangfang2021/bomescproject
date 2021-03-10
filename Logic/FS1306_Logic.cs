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
        public DataTable getDataInfo(string strPackingPlant, ref DataTable dtMessage)
        {
            try
            {
                DataTable dtBanZhi = fs0811_DataAccess.getNowBZInfo(strPackingPlant);
                string strHosDate = dtBanZhi.Rows[0]["dHosDate"].ToString();
                string strBanZhi = dtBanZhi.Rows[0]["vcBanZhi"].ToString();
                string strFromTime = dtBanZhi.Rows[0]["tFromTime"].ToString();
                string strToTime = dtBanZhi.Rows[0]["tToTime"].ToString();
                string strBZFromTime = strHosDate + " " + strFromTime;
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
                return dtList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DataTable setHeJ(DataTable dtDataInfo)
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



            return dtDataInfo;
        }

    }
}
