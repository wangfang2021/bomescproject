using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Common;
using DataAccess;
using DataEntity;

namespace Logic
{
    public class FS0611_Logic
    {
        FS0611_DataAccess fs0611_DataAccess = new FS0611_DataAccess();

        #region 生成SOQReply
        public int create(string varDxny, string userId)
        {
            try
            {
                //从soq表中获取soq，然后获取稼动日历，再生成平准化结果
                //#1厂
                //获取稼动日历数据
                DataTable calendarRe = fs0611_DataAccess.getJdrlData(varDxny, "1");

                //计算月前半稼动日大小
                decimal halfWorkDaysCount = Math.Ceiling(Convert.ToDecimal(calendarRe.Rows[0]["TOTALWORKDAYS"].ToString()) / 2);


                //获取品番箱数为1的所有soq品番
                DataTable re = fs0611_DataAccess.getParts(varDxny, "1", 1);


                //开始平准
                //存储前半月稼动的日子
                DataTable pzRe = new DataTable();

                pzRe.Columns.Add("PARTSNO");

                //给结果表新增列。只有前半稼动日的列
                int index = Convert.ToInt32(halfWorkDaysCount);
                for (int i = 0; i < 31 && index > 0; i++)
                {
                    //休息日跳过
                    if (calendarRe.Rows[0]["TARGETDAY" + (i + 1)].ToString() == "0")
                        continue;
                    else
                    {
                        pzRe.Columns.Add("D" + (i + 1));
                        index = index - 1;
                    }
                }

                //发注工厂列
                pzRe.Columns.Add("iFZGC");
                //订货频度列
                pzRe.Columns.Add("varMakingOrderType");
                //内外区分列
                pzRe.Columns.Add("INOUTFLAG");
                //车型编号列
                pzRe.Columns.Add("CARFAMILYCODE");
                //收容数列
                pzRe.Columns.Add("QUANTITYPERCONTAINER");
                //箱数列
                pzRe.Columns.Add("iUnits");
                //品番数列
                pzRe.Columns.Add("iPCS");


                int dayIndex = 0;
                int roundIndex = 0;
                for (int i = 0; i < re.Rows.Count; i++)
                {
                    pzRe.Rows.Add();
                    pzRe.Rows[i]["PARTSNO"] = re.Rows[i]["PARTSNO"].ToString();

                    //第一轮且单值，则跳过该日的排班
                    while (roundIndex == 0 && calendarRe.Rows[0][pzRe.Columns[dayIndex + 1].ToString().Replace("D", "TARGETDAY")].ToString().Length > 1)
                    {
                        dayIndex++;
                        if (dayIndex > halfWorkDaysCount - 1)
                        {
                            roundIndex++;
                            dayIndex = 0;
                        }
                    }
                    pzRe.Rows[i][dayIndex + 1] = 1;

                    //发注工厂列
                    pzRe.Rows[i]["iFZGC"] = re.Rows[i]["iFZGC"].ToString();
                    //订货频度列
                    pzRe.Rows[i]["varMakingOrderType"] = re.Rows[i]["varMakingOrderType"].ToString();
                    //内外区分列
                    pzRe.Rows[i]["INOUTFLAG"] = re.Rows[i]["INOUTFLAG"].ToString();
                    //车型编号列
                    pzRe.Rows[i]["CARFAMILYCODE"] = re.Rows[i]["CARFAMILYCODE"].ToString();
                    //收容数列
                    pzRe.Rows[i]["QUANTITYPERCONTAINER"] = re.Rows[i]["QUANTITYPERCONTAINER"].ToString();
                    //箱数列
                    pzRe.Rows[i]["iUnits"] = re.Rows[i]["iUnits"].ToString();
                    //品番数列
                    pzRe.Rows[i]["iPCS"] = re.Rows[i]["iPCS"].ToString();

                    dayIndex++;
                    if (dayIndex > halfWorkDaysCount - 1)
                    {
                        roundIndex++;
                        dayIndex = 0;
                    }
                }


                if (pzRe.Rows.Count > 0)
                    return fs0611_DataAccess.save(pzRe, userId, varDxny, 0);
                else
                    return 0;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region 展开SOQReply
        public int zk(string varDxny, string userId)
        {
            return fs0611_DataAccess.zk(varDxny, userId);
        }
        #endregion

        #region 下载SOQReply（检索内容）
        public DataTable search(string varDxny)
        {
            return fs0611_DataAccess.search(varDxny);
        }
        #endregion

        #region 导入后保存
        public void importSave(DataTable dt, string varDxny)
        {
            fs0611_DataAccess.importSave(dt, varDxny);
        }
        #endregion
    }
}
