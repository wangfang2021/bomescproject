using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Common;
using DataAccess;
using DataEntity;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Logic
{
    public class FS0610_Logic
    {
        FS0610_DataAccess fs0610_DataAccess = new FS0610_DataAccess();

        #region 取日历
        public DataTable GetCalendar(string strPlant, string vcDXYM)
        {
            return fs0610_DataAccess.GetCalendar(strPlant, vcDXYM);
        }
        #endregion

        #region 取soq数据
        public DataTable GetSoq(string strPlant, string strDXYM, string strType)
        {
            return fs0610_DataAccess.GetSoq(strPlant, strDXYM, strType);
        }
        #endregion

        #region 更新平准化结果
        public void SaveResult(string strCLYM, string strDXYM, string strNSYM, string strNNSYM, string strPlant,
            ArrayList arrResult_DXYM, ArrayList arrResult_NSYM, ArrayList arrResult_NNSYM, string strUserId)
        {
            fs0610_DataAccess.SaveResult(strCLYM, strDXYM, strNSYM, strNNSYM, strPlant,
             arrResult_DXYM, arrResult_NSYM, arrResult_NNSYM, strUserId);
        }
        #endregion

        #region 展开SOQReply
        public int zk(string varDxny, string userId)
        {
            return fs0610_DataAccess.zk(varDxny, userId);
        }
        #endregion

        #region 下载SOQReply（检索内容）
        public DataTable search(string varDxny)
        {
            return fs0610_DataAccess.search(varDxny);
        }
        #endregion

        #region 导入后保存
        public void importSave(DataTable dt, string varDxny)
        {
            fs0610_DataAccess.importSave(dt, varDxny);
        }
        #endregion

        #region 生产计划方法（王立伟）2020-01-21
        /// <summary>
        /// 生成生产计划
        /// </summary>
        /// <param name="vcDxny">年月</param>
        /// <param name="vcFZGC">工厂</param>
        /// <returns></returns>
        public string createProPlan(string vcDxny, string[] vcFZGC, string strUserId)
        {
            string msg = string.Empty;//方法返回消息

            this.Mon = Convert.ToInt32(vcDxny.Substring(4, 2));
            this.Year = Convert.ToInt32(vcDxny.Substring(0, 4));
            for (int k = 0; k < vcFZGC.Length; k++)
            {
                //判断SOQREPLY是否存在
                DataTable dt_info = fs0610_DataAccess.getSoqInfo(vcDxny, vcFZGC[k]);
                if (dt_info.Rows.Count == 0)
                {
                    return "请先导入SOQREPLY再生成计划！";
                }
                else
                {
                    for (int i = 0; i < dt_info.Rows.Count; i++)
                    {
                        if (dt_info.Rows[i]["kbsrs"].ToString().Length == 0)
                        {
                            return "品番：" + dt_info.Rows[i]["Partsno"].ToString() + "器具收容数未维护！";
                        }
                    }
                }
                //Pro1 生产日历--3
                //Pro0 看板打印
                //Pro2 涂装
                //Pro3 预留
                List<DataTable> dtpro1 = new List<DataTable>();
                List<DataTable> dtpro0 = new List<DataTable>();
                List<DataTable> dtpro2 = new List<DataTable>();
                List<DataTable> dtpro3 = new List<DataTable>();
                List<DataTable> dtpro4;

                //平准 Pro4 包装计划 --4   与生产的zhi  一一对应
                try
                {
                    dtpro4 = InitDataREP(dt_info, ref msg, ref dtpro0, ref dtpro1, ref dtpro2, ref dtpro3);
                }
                catch
                {
                    return "计算失败！";
                }
                if (dtpro4 == null || msg.Length > 0)
                {
                    return msg;
                }
                int tmpYear = Convert.ToInt32(vcDxny.Substring(0, 4));
                int tmpMonth = Convert.ToInt32(vcDxny.Substring(4, 2));
                string user = strUserId;
                try
                {
                    msg = fs0610_DataAccess.updateProPlan(dtpro0, dtpro1, dtpro2, dtpro3, dtpro4, dt_info, user, vcDxny, vcFZGC[k]); //更新到包装计划临时表
                }
                catch (Exception ex)
                {
                    return "生成计划失败！";
                }

                if (msg.Length == 0) //取得以上生成的临时表包装计划
                {
                    DataTable dt_temp = serchData(vcDxny, "Importpro", "", vcFZGC[k], "");
                    Exception ex = new Exception();
                    fs0610_DataAccess.updatePro(dt_temp, strUserId, vcDxny, ref ex, vcFZGC[k]);
                    if (msg.Length > 0)
                    {
                        return msg;
                    }
                    msg = "计划生成成功！";
                }
            }
            return msg;
        }


        #region 平准方法
        public static object lockflag = new object();
        public static DataTable dt_reality;
        public static DataTable dt_zhiInfo;
        public int Mon = 0;//月份
        public int Year = 2012;
        //public int srs = 1;
        //public int kbsrs = 1;
        //箱数为1标识
        public static int Pos_zhi = 1;
        //箱数为2标识
        public static int Pos_zhi_left = 1;
        public static int Pos_zhi_right = 1;
        //箱数为3~n标识
        public static int Maxzhounum = 0;
        public static int lastzhou_zhinum = 0;
        public static int Pos_zhou = 1;
        public static int Pos_zhou_zhi = 1;
        public static List<int> step_value = new List<int>();
        public static int step_Pos = 0;
        //箱数为n标识
        public static List<int> step_value2 = new List<int>();
        public static int step_Pos2 = 0;
        //箱数大于N
        public static DataTable dtchkNUM;

        /// <summary>
        /// 给数据表设置从D1b到D31y的列
        /// </summary>
        /// <param name="dt">原表</param>
        /// <returns>返回的设置了列的表</returns>
        public DataTable InitDataNum(DataTable dt)
        {
            dt = new DataTable();
            for (int i = 1; i < 32; i++)
            {
                DataColumn col1 = new DataColumn();
                col1.ColumnName = "D" + i + "b";
                col1.DataType = typeof(int);
                col1.DefaultValue = 0;
                dt.Columns.Add(col1);
                DataColumn col2 = new DataColumn();
                col2.ColumnName = "D" + i + "y";
                col2.DataType = typeof(int);
                col2.DefaultValue = 0;
                dt.Columns.Add(col2);
            }
            dt.Rows.Add(dt.NewRow());
            return dt;
        }

        public void AddNUM(ref DataTable dtchkNUM, DataTable dtsig, int srs)
        {
            for (int z = 0; z < dtsig.Rows.Count; z++)
            {
                dtchkNUM.Rows[0][dtsig.Rows[z]["days"].ToString().Trim()] = Convert.ToInt32(dtchkNUM.Rows[0][dtsig.Rows[z]["days"].ToString().Trim()]) + Convert.ToInt32(dtsig.Rows[z]["total"]) * srs;
            }
        }

        public List<DataTable> InitDataREP(DataTable dt, ref string msg, ref List<DataTable> dtPro0, ref List<DataTable> dtpro1, ref List<DataTable> dtPro2, ref List<DataTable> dtPro3)
        {
            int tmpmon = Mon == 0 ? DateTime.Now.Month : Mon;
            List<DataTable> dtall = new List<DataTable>();
            lock (lockflag)//锁定static变量
            {
                resetValue();
                string tmp_gc = "";
                string tmp_zb = "";
                string tmp_plant = "";
                int jjj = 0;
                dtchkNUM = InitDataNum(dtchkNUM);
                try
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        step_value = new List<int>();
                        step_value2 = new List<int>();
                        string plant = dt.Rows[i]["vcFZGC"].ToString();
                        string partsno = dt.Rows[i]["vcPart_id"].ToString();
                        string dock = dt.Rows[i]["vcDock"].ToString();
                        string cartype = dt.Rows[i]["vcCarFamilyCode"].ToString();
                        int num = Convert.ToInt32(dt.Rows[i]["num"]);
                        int srs = Convert.ToInt32(dt.Rows[i]["srs"]);
                        int kbsrs = Convert.ToInt32(dt.Rows[i]["kbsrs"]);
                        string gc = dt.Rows[i]["vcPorType"].ToString();
                        string zb = dt.Rows[i]["vcZB"].ToString();
                        string parttype = dt.Rows[i]["KBpartType"].ToString();
                        DataTable dtFinal_pack = new DataTable();
                        //if (tmp_gc != gc || tmp_zb != zb) //工程变更
                        //{
                        //    resetValue();
                        //    tmp_gc = gc;
                        //    tmp_zb = zb;
                        //}
                        if (tmp_plant != plant)//工厂变更
                        {
                            resetValue();
                            tmp_gc = gc;
                            tmp_zb = zb;
                            tmp_plant = plant;
                            dtchkNUM = InitDataNum(dtchkNUM);
                        }
                        else if (tmp_gc != gc && tmp_plant == plant)//工程变更
                        {
                            resetValue();
                            tmp_gc = gc;
                            tmp_zb = zb;
                            dtchkNUM = InitDataNum(dtchkNUM);
                        }
                        //--1由生产日历推包装日历 
                        //取当前部署的生产日历  vcCalendar1 的嫁动形态
                        string vcCalendar1 = dt.Rows[i]["vcCalendar1"].ToString();
                        string vcCalendar0 = dt.Rows[i]["vcCalendar0"].ToString();
                        if (vcCalendar1.Length <= 0 || vcCalendar0.Length <= 0)
                        {
                            msg = "工程：" + gc + ",组别：" + zb + ",生产条件未维护！";
                            return null;
                        }
                        string pro_gc = vcCalendar1.Split('-')[0];
                        string pro_zb = vcCalendar1.Split('-')[1];

                        //取当前部署的包装日历 vcCalendar4 的嫁动形态
                        string vcCalendar4 = dt.Rows[i]["vcCalendar4"].ToString();
                        if (vcCalendar4.Length <= 0)
                        {
                            msg = "工程：" + gc + ",组别：" + zb + ",生产条件未维护！";
                            return null;
                        }
                        string pack_gc = vcCalendar4.Split('-')[0];
                        string pack_zb = vcCalendar4.Split('-')[1];
                        //取两个日历的Lt
                        int lt1 = Convert.ToInt32(dt.Rows[i]["vcLT1"].ToString().Length == 0 ? "0" : dt.Rows[i]["vcLT1"].ToString());
                        int lt2 = Convert.ToInt32(dt.Rows[i]["vcLT2"].ToString().Length == 0 ? "0" : dt.Rows[i]["vcLT2"].ToString());
                        int lt3 = Convert.ToInt32(dt.Rows[i]["vcLT3"].ToString().Length == 0 ? "0" : dt.Rows[i]["vcLT3"].ToString());
                        int Lt = lt1 + lt2 + lt3;
                        //取生产日历 含 上月N+当月M
                        DataTable dtpro = new DataTable();
                        if (parttype == "1" || parttype == "非指定")
                        {
                            dtpro = fs0610_DataAccess.getCalendarSP(Year, Mon, pro_gc, pro_zb, Lt, plant);
                        }
                        else
                        {
                            dtpro = fs0610_DataAccess.getCalendar(Year, Mon, pro_gc, pro_zb, "N", "M", "1", plant);
                        }
                        // 2014-12-2 修正52值得问题 start
                        for (int m = 0; m < dtpro.Rows.Count; m++)
                        {
                            if (Convert.ToInt32(dtpro.Rows[m]["zhou"].ToString().Length > 0 ? dtpro.Rows[m]["zhou"].ToString() : "0") >= 5 && dtpro.Rows[m]["zhi"].ToString() != "9999")
                            {
                                dtpro.Rows[m]["zhou"] = 5;
                                ;
                            }
                        }
                        // 2014-12-2 修正52值得问题 end
                        DataTable dtpack = fs0610_DataAccess.getCalendar(Year, Mon, pack_gc, pack_zb, "M", "M", "", plant);
                        if (dtpro.Rows.Count <= 1 || dtpro.Select("vcYear='" + Year + "' and  vcMonth ='" + Mon.ToString("00") + "'").Length == 0)
                        {
                            msg = "对象月生产日历(" + pro_gc + "-" + pro_zb + ")不存在，请先维护生产日历再导入！";
                            return null;
                        }
                        if (dtpack.Rows.Count == 0)
                        {
                            msg = "与生产日历(" + pro_gc + "-" + pro_zb + ")相对应的包装日历(" + pack_gc + "-" + pack_zb + ")不存在，请先维护包装日历再导入！";
                            return null;
                        }

                        //获得最终排产的包装日历
                        dtFinal_pack = getFinalCalendar(dtpro, dtpack, Lt, ref msg);
                        if (msg.Length > 0)
                        {
                            return null;
                        }

                        //获取pro0的日历
                        string tmp0 = dt.Rows[i]["vcLT0"].ToString().Length == 0 ? "0" : dt.Rows[i]["vcLT0"].ToString();
                        int Lt0 = 0 - Convert.ToInt32(tmp0);
                        //string vcCalendar0 = dt.Rows[i]["vcCalendar0"].ToString();
                        string pro0_gc = vcCalendar0.Split('-')[0];
                        string pro0_zb = vcCalendar0.Split('-')[1];
                        DataTable dtpro0 = fs0610_DataAccess.getCalendar(Year, Mon, pro0_gc, pro0_zb, "M", "M", "", plant);
                        DataTable dtFinal_Pro0 = getFinalCalendar(dtpro, dtpro0, Lt0, ref msg);
                        if (msg.Length > 0)
                        {
                            return null;
                        }
                        //获取pro2的日历
                        string tmp1 = dt.Rows[i]["vcLT1"].ToString().Length == 0 ? "0" : dt.Rows[i]["vcLT1"].ToString();
                        int Lt2 = Convert.ToInt32(tmp1);
                        string vcCalendar2 = dt.Rows[i]["vcCalendar2"].ToString().Length == 0 ? dt.Rows[i]["vcCalendar0"].ToString() : dt.Rows[i]["vcCalendar2"].ToString();
                        string pro2_gc = vcCalendar2.Split('-')[0];
                        string pro2_zb = vcCalendar2.Split('-')[1];
                        DataTable dtpro2 = fs0610_DataAccess.getCalendar(Year, Mon, pro2_gc, pro2_zb, "M", "M", "", plant);
                        DataTable dtFinal_Pro2 = getFinalCalendar(dtpro, dtpro2, Lt2, ref msg);
                        if (msg.Length > 0)
                        {
                            return null;
                        }
                        //获取pro3的日历
                        string tmp2 = dt.Rows[i]["vcLT2"].ToString().Length == 0 ? "0" : dt.Rows[i]["vcLT2"].ToString();
                        int Lt3 = Convert.ToInt32(tmp1) + Convert.ToInt32(tmp2);
                        string vcCalendar3 = dt.Rows[i]["vcCalendar3"].ToString().Length == 0 ? dt.Rows[i]["vcCalendar0"].ToString() : dt.Rows[i]["vcCalendar3"].ToString();
                        string pro3_gc = vcCalendar0.Split('-')[0];
                        string pro3_zb = vcCalendar0.Split('-')[1];
                        DataTable dtpro3 = fs0610_DataAccess.getCalendar(Year, Mon, pro3_gc, pro3_zb, "M", "M", "", plant);
                        DataTable dtFinal_Pro3 = getFinalCalendar(dtpro, dtpro3, Lt3, ref msg);
                        if (msg.Length > 0)
                        {
                            return null;
                        }
                        //--2在包装日历上平准 
                        if (dtFinal_pack.Rows.Count <= 1)
                        {
                            msg = "与生产日历相匹配的包装日历不存在，请先维护包装日历再导入！";
                            return null;
                        }
                        dt_reality = dtFinal_pack.Select(" zhi<>'9999' ").CopyToDataTable();
                        dt_zhiInfo = dtFinal_pack.Select(" zhi='9999' ").CopyToDataTable();
                        lastzhou_zhinum = dt_reality.Rows.Count % 10;
                        Maxzhounum = dt_reality.Rows.Count / 10 + (lastzhou_zhinum != 0 ? 1 : 0);
                        // 2014-12-2 修正52值得问题 start
                        if (Maxzhounum > 5)
                        {
                            Maxzhounum = 5;
                        }
                        // 2014-12-2 修正52值得问题 end
                        stepCal_logic3(Maxzhounum);
                        stepCal_logicn(Maxzhounum);
                        int Minbeishu = getMinbeishu(srs, kbsrs);//算出最小公倍数
                        int kb = Minbeishu / srs;//每次分配的个数
                        int boxnum = (num / Minbeishu) + (num % Minbeishu != 0 ? 1 : 0);//分配的次数(含器具收容数)
                        int yushu = (num % Minbeishu) / srs;//余数 --最后一次分配的箱数
                        FS0610_Logic fspzlg = _pz(boxnum);
                        DataTable dtsig = new DataTable();
                        dtsig = dt_reality.DefaultView.ToTable();
                        bool isLgN = fspzlg.dt_result(ref dtsig, kb, yushu, boxnum, srs);//在包装计划上平准
                        dtall.Add(dtsig);
                        //平准后数量插入临时dt
                        if (!isLgN)
                        {
                            AddNUM(ref dtchkNUM, dtsig, srs);
                        }
                        //算出NPCS每值数量
                        for (int m = 0; m < dtsig.Rows.Count; m++)
                        {
                            dtsig.Rows[m]["total"] = Convert.ToInt32(dtsig.Rows[m]["total"]) * srs;
                        }
                        //将平准结果赋予其他计划
                        dtFinal_Pro0 = CopyOther(dtsig, dtFinal_Pro0);
                        dtFinal_Pro2 = CopyOther(dtsig, dtFinal_Pro2);
                        dtFinal_Pro3 = CopyOther(dtsig, dtFinal_Pro3);
                        dtpro = CopyOther(dtsig, dtpro);
                        dtpro1.Add(dtpro);
                        dtPro0.Add(dtFinal_Pro0);
                        dtPro3.Add(dtFinal_Pro3);
                        dtPro2.Add(dtFinal_Pro2);
                    }
                }
                catch (Exception ex)
                {
                    int aaa = jjj;
                    throw ex;
                }
            }
            return dtall;
        }
        public DataTable CopyOther(DataTable pack, DataTable other)
        {
            pack = pack.Select(" zhi<> '9999'").CopyToDataTable();
            other = other.Select(" zhi<> '9999'").CopyToDataTable();
            for (int i = 0; i < pack.Rows.Count; i++)
            {
                other.Rows[i]["total"] = pack.Rows[i]["total"];
            }
            return other;
        }
        public void stepCal_logic3(int maxzhounum)
        {
            switch (maxzhounum)
            {
                case 1:
                    step_value.Add(1);
                    break;
                case 2:
                    step_value.Add(1);
                    step_value.Add(2);
                    break;
                case 3:
                    step_value.Add(1);
                    step_value.Add(3);
                    step_value.Add(2);
                    break;
                case 4:
                    step_value.Add(1);
                    step_value.Add(3);
                    step_value.Add(2);
                    step_value.Add(4);
                    break;
                case 5:
                    step_value.Add(1);
                    step_value.Add(3);
                    step_value.Add(5);
                    step_value.Add(2);
                    step_value.Add(4);
                    break;
            }
        }
        public void stepCal_logicn(int maxzhounum)
        {
            switch (maxzhounum)
            {
                case 1:
                    step_value2.Add(1);
                    break;
                case 2:
                    step_value2.Add(2);
                    step_value2.Add(1);
                    break;
                case 3:
                    step_value2.Add(3);
                    step_value2.Add(2);
                    step_value2.Add(1);
                    break;
                case 4:
                    step_value2.Add(4);
                    step_value2.Add(3);
                    step_value2.Add(2);
                    step_value2.Add(1);
                    break;
                case 5:
                    step_value2.Add(5);
                    step_value2.Add(4);
                    step_value2.Add(3);
                    step_value2.Add(2);
                    step_value2.Add(1);
                    break;
            }
        }
        public virtual bool dt_result(ref DataTable dt, int kbsrs, int yushu, int boxnum, int srs)
        {
            return false;
        }

        public FS0610_Logic _pz(int totalreality)
        {
            FS0610_Logic lg = null;
            if (totalreality == 1)
            {
                lg = new PZlogic_1();
            }
            else if (totalreality == 2)
            {
                lg = new PZlogic_2();
            }
            else if (totalreality > 2 && totalreality <= Convert.ToInt32(dt_zhiInfo.Rows[0]["total"]))
            {
                lg = new PZlogic_3();
            }
            else if (totalreality > Convert.ToInt32(dt_zhiInfo.Rows[0]["total"]))
            {
                lg = new PZlogic_n();
            }
            return lg;
        }

        public void resetValue()
        {
            Pos_zhi = 1;
            Pos_zhi_left = 1;
            Pos_zhi_right = 1;
            Maxzhounum = 0;
            lastzhou_zhinum = 0;
            Pos_zhou = 1;
            Pos_zhou_zhi = 1;
            step_value = new List<int>();
            step_value2 = new List<int>();
            step_Pos = 0;
        }
        #endregion

        #region 根据生产日历和LT推其他日历
        public DataTable getFinalCalendar(DataTable dtpro, DataTable dtpack, int Lt, ref string msg)
        {
            DataTable dtFinal = dtpro.Copy();
            for (int i = 0; i < (dtpro.Rows.Count - 1); i++)
            {
                string proyear = dtFinal.Rows[i]["vcYear"].ToString();
                string promon = dtFinal.Rows[i]["vcMonth"].ToString();
                string days = dtFinal.Rows[i]["days"].ToString();
                string progc = dtFinal.Rows[i]["vcGC"].ToString();
                string prozb = dtFinal.Rows[i]["vcZB"].ToString();

                string packyear = dtpack.Rows[i]["vcYear"].ToString();
                string packmon = dtpack.Rows[i]["vcMonth"].ToString();
                string packgc = dtpack.Rows[i]["vcGC"].ToString();
                string packzb = dtpack.Rows[i]["vcZB"].ToString();
                DataRow[] dr = dtpack.Select("vcYear='" + proyear + "' and vcMonth='" + promon + "' and days='" + days + "' ");
                if (dr.Length == 0)
                {
                    msg = proyear + "-" + promon + "," + progc + "-" + prozb + "日历稼动而" + proyear + "-" + promon + "," + packgc + "-" + packzb + "日历非稼动。";
                    break;
                }
                //取得值数
                int zhi = Convert.ToInt32(dr[0]["zhi"]);
                //取加上Lt 后的dr
                DataRow[] dr2 = dtpack.Select("zhi='" + (zhi + Lt) + "' ");
                if (dr2.Length == 0)
                {
                    msg = proyear + "-" + promon + "," + progc + "-" + prozb + "日历维护错误，超出" + proyear + "-" + promon + "," + packgc + "-" + packzb + "日历最大值。";
                    break;
                }
                dtFinal.Rows[i]["days"] = dr2[0]["days"];
                dtFinal.Rows[i]["vcMonth"] = dr2[0]["vcMonth"];
                dtFinal.Rows[i]["vcYear"] = dr2[0]["vcYear"];
            }
            return dtFinal;
        }
        #endregion

        public int getMinbeishu(int m, int n)//求最小公倍数
        {
            int i = 0;
            for (i = m < n ? m : n; m % i != 0 || n % i != 0; i--) ;
            return i = m * n / i;
        }

        public DataTable serchData(string mon, string plan, string type, string plant, string plantname)//检索方法
        {
            DataTable dt = new DataTable();
            switch (plan)
            {
                case "1"://生产计划  值别
                    {
                        if (type == "0")//合算
                        {
                            dt = fs0610_DataAccess.getMonProALL2(mon, "MonthProdPlanTbl", "EDMonthProdPlanTbl", plant);
                            dt.TableName = "ALLPro1" + "-" + plantname;
                        }
                        if (type == "1")//月度
                        {
                            dt = fs0610_DataAccess.getMonPackPlanTMP(mon, "MonthProdPlanTbl", plant);
                            addEDflag(ref dt, "通常");
                            dt.TableName = "MonPro1" + "-" + plantname;
                        }
                        break;
                    }
                case "0"://包装计划  值别
                    {
                        if (type == "0")//合算
                        {
                            dt = fs0610_DataAccess.getMonProALL2(mon, "MonthPackPlanTbl", "EDMonthPackPlanTbl", plant);
                            dt = tranNZtoWZ(dt, mon);
                            dt.TableName = "ALLPro4" + "-" + plantname;
                        }
                        if (type == "1")//月度
                        {
                            dt = fs0610_DataAccess.getMonPackPlanTMP(mon, "MonthPackPlanTbl", plant);
                            dt = tranNZtoWZ(dt, mon);
                            addEDflag(ref dt, "通常");
                            dt.TableName = "MonPro4" + "-" + plantname;
                        }
                        break;
                    }
                case "2"://看板打印计划  值别
                    {
                        if (type == "0")//合算
                        {
                            dt = fs0610_DataAccess.getMonProALL2(mon, "MonthKanBanPlanTbl", "EDMonthKanBanPlanTbl", plant);
                            dt.TableName = "ALLPro0" + "-" + plantname;
                        }
                        if (type == "1")//月度
                        {
                            dt = fs0610_DataAccess.getMonPackPlanTMP(mon, "MonthKanBanPlanTbl", plant);
                            dt.TableName = "MonPro0" + "-" + plantname;
                            addEDflag(ref dt, "通常");
                        }
                        break;
                    }
                case "3"://丰铁看板涂装计划  值别
                    {
                        if (type == "0")//合算
                        {
                            dt = fs0610_DataAccess.getMonProALL2(mon, "MonthTZPlanTbl", "dbo.EDMonthTZPlanTbl", plant);
                            dt = tranNZtoWZ(dt, mon);
                            dt.TableName = "ALLPro2" + "-" + plantname;
                        }
                        if (type == "1")//月度
                        {
                            dt = fs0610_DataAccess.getMonPackPlanTMP(mon, "MonthTZPlanTbl", plant);
                            dt = tranNZtoWZ(dt, mon);
                            addEDflag(ref dt, "通常");
                        }
                        break;
                    }
                case "4"://P3计划  值别
                    {
                        if (type == "0")//合算
                        {
                            dt = fs0610_DataAccess.getMonProALL2(mon, "MonthP3PlanTbl", "EDMonthP3PlanTbl", plant);
                            dt = tranNZtoWZ(dt, mon);
                            dt.TableName = "ALLPro3" + "-" + plantname;
                        }
                        if (type == "1")//月度
                        {
                            dt = fs0610_DataAccess.getMonPackPlanTMP(mon, "MonthP3PlanTbl", plant);
                            dt = tranNZtoWZ(dt, mon);
                            addEDflag(ref dt, "通常");
                        }
                        break;
                    }
                case "Importpack":
                    {
                        dt = fs0610_DataAccess.getMonPackPlanTMPcur(mon, "MonthPackPlanTblTMP", plant);
                        addEDflag(ref dt, "通常"); ;
                        //   dt = tranNZtoWZ(dt);
                        break;
                    }
                case "Importpro":
                    {
                        dt = fs0610_DataAccess.getMonPackPlanTMPcur(mon, "MonthProPlanTblTMP", plant);
                        addEDflag(ref dt, "通常");
                        break;
                    }
            }
            dt.Columns["vcEDflag"].SetOrdinal(5);
            return dt;
        }

        public void addEDflag(ref DataTable dt, string flag)//加列
        {
            DataColumn col = new DataColumn();
            col.ColumnName = "vcEDflag";
            col.DataType = typeof(string);
            col.DefaultValue = flag;
            dt.Columns.Add(col);
        }

        public DataTable tranNZtoWZ(DataTable dtNZ, string mon)//内制品转换相应外注
        {
            DataTable dtWz = new DataTable();
            DataTable dtInfo = fs0610_DataAccess.getPartsno(mon);
            dtWz = dtNZ.Copy();
            foreach (var row in dtWz.AsEnumerable())
            {
                var isQF = from info in dtInfo.AsEnumerable()
                           where info.Field<string>("vcPartsNo") == row.Field<string>("vcPartsNo").Replace("-", "")
                           && info.Field<string>("vcQFflag") == "1" && info.Field<string>("vcDock") == row.Field<string>("vcDock")
                           select info;
                if (isQF.Count() > 0)
                {
                    var tmp = from info in dtInfo.AsEnumerable()
                              where info.Field<string>("vcPartsNo").Substring(0, 10) == row.Field<string>("vcPartsNo").Replace("-", "").Substring(0, 10)
                                  && info.Field<string>("vcInOutFlag") == "1"
                              select new
                              {
                                  o_partsno = info.Field<string>("vcPartsNo"),
                                  o_dock = info.Field<string>("vcDock")
                              };
                    if (tmp.Count() > 0)
                    {
                        string tmppartsno = tmp.ElementAt(0).o_partsno.ToString();
                        string partsno = tmppartsno.Insert(5, "-").Insert(11, "-");
                        string dock = tmp.ElementAt(0).o_dock.ToString();
                        row.SetField<string>("vcPartsNo", partsno);
                        row.SetField<string>("vcDock", dock);
                    }
                }
            }
            return dtWz;
        }

        public DataTable tranWztoNz(DataTable dtWz, string mon)//外注品转换相应内制品
        {
            DataTable dtNz = new DataTable();
            DataTable dtInfo = fs0610_DataAccess.getPartsno(mon);
            dtNz = dtWz.Copy();
            foreach (var row in dtNz.AsEnumerable())
            {
                var isWz = from info in dtInfo.AsEnumerable()
                           where info.Field<string>("vcPartsNo") == row.Field<string>("vcPartsNo").Replace("-", "")
                           && info.Field<string>("vcDock") == row.Field<string>("vcDock")
                           && info.Field<string>("vcInOutFlag") == "1"
                           select info;
                if (isWz.Count() > 0)
                {
                    var tmp = from info in dtInfo.AsEnumerable()
                              where info.Field<string>("vcPartsNo").Substring(0, 10) == row.Field<string>("vcPartsNo").Replace("-", "").Substring(0, 10)
                              && info.Field<string>("vcInOutFlag") == "0"
                              select new
                              {
                                  o_partsno = info.Field<string>("vcPartsNo"),
                                  o_dock = info.Field<string>("vcDock")
                              };
                    if (tmp.Count() > 0)
                    {
                        string tmppartsno = tmp.ElementAt(0).o_partsno.ToString();
                        string partsno = tmppartsno.Insert(5, "-").Insert(11, "-");
                        string dock = tmp.ElementAt(0).o_dock.ToString();
                        row.SetField<string>("vcPartsNo", partsno);
                        row.SetField<string>("vcDock", dock);
                    }
                }
            }
            return dtNz;
        }

        #endregion
    }

    #region 平准分支
    public class PZlogic_1 : FS0610_Logic
    {
        public override bool dt_result(ref DataTable dt, int kbsrs, int yushu, int boxnum, int srs)//kbsrs 分配的数 yushu 余数
        {
            if (Pos_zhi >= Convert.ToInt32(dt_zhiInfo.Rows[0]["zhou"]) + 1)
            {
                Pos_zhi = 1;
            }
            if (yushu > 0)
            {
                dt.Rows[Pos_zhi - 1]["total"] = Convert.ToInt32(dt.Rows[Pos_zhi - 1]["total"]) + yushu;
            }
            else
            {
                dt.Rows[Pos_zhi - 1]["total"] = Convert.ToInt32(dt.Rows[Pos_zhi - 1]["total"]) + kbsrs;
            }
            Pos_zhi++;
            return false;
            //return base.dt_result(ref dt);
        }
    }

    public class PZlogic_2 : FS0610_Logic
    {
        public override bool dt_result(ref DataTable dt, int kbsrs, int yushu, int boxnum, int srs)//kbsrs 分配的数 yushu 余数
        {
            Pos_zhi_left = Pos_zhi - 1;
            Pos_zhi_right = Convert.ToInt32(dt_zhiInfo.Rows[0]["zhou"]) + 1;
            if (Pos_zhi_left >= Convert.ToInt32(dt_zhiInfo.Rows[0]["zhou"]))
            {
                Pos_zhi_left = 1;
            }
            if (Pos_zhi_right > Convert.ToInt32(dt_zhiInfo.Rows[0]["total"]))
            {
                Pos_zhi_right = Convert.ToInt32(dt_zhiInfo.Rows[0]["zhou"]) + 1;
            }
            dt.Rows[Pos_zhi_left]["total"] = Convert.ToInt32(dt.Rows[Pos_zhi_left]["total"]) + kbsrs;
            dt.Rows[Pos_zhi_right - 1]["total"] = Convert.ToInt32(dt.Rows[Pos_zhi_right - 1]["total"]) + (yushu != 0 ? yushu : kbsrs);
            Pos_zhi_left++;
            Pos_zhi_right++;
            // return base.dt_result(ref dt);
            return false;
        }
    }

    public class PZlogic_3 : FS0610_Logic
    {
        public override bool dt_result(ref DataTable dt, int kbsrs, int yushu, int boxnum, int srs)//kbsrs 每次分配的看板数 yushu 余数  boxnum分配的次数
        {
            for (int i = 1; i <= boxnum; i++)
            {
                if (Pos_zhou_zhi > 10)
                {
                    Pos_zhou_zhi = 1;
                }
                if (step_Pos > step_value.Count - 1)
                {
                    step_Pos = 0;
                }
                if (step_Pos != step_value.Count - 1)
                {
                    int addPos = (step_value[step_Pos] - 1) * 10 + Pos_zhou_zhi - 1;

                    if (step_value[step_Pos] == step_value.Max() && Pos_zhou_zhi > lastzhou_zhinum)
                    {
                        step_Pos++;
                        addPos = (step_value[step_Pos] - 1) * 10 + Pos_zhou_zhi - 1;
                        if (i == boxnum && yushu > 0)
                        {
                            dt.Rows[addPos]["total"] = Convert.ToInt32(dt.Rows[addPos]["total"]) + yushu;
                            break;
                        }
                        else
                        {
                            dt.Rows[addPos]["total"] = Convert.ToInt32(dt.Rows[addPos]["total"]) + kbsrs;
                            step_Pos++;
                            Pos_zhou_zhi++;
                        }
                    }
                    else
                    {
                        if (i == boxnum && yushu > 0)
                        {
                            dt.Rows[addPos]["total"] = Convert.ToInt32(dt.Rows[addPos]["total"]) + yushu;
                            break;
                        }
                        else
                        {
                            dt.Rows[addPos]["total"] = Convert.ToInt32(dt.Rows[addPos]["total"]) + kbsrs;
                            step_Pos++;
                            Pos_zhou_zhi++;
                        }
                    }
                }
                else
                {
                    if (step_value[step_Pos] == step_value.Max() && Pos_zhou_zhi > lastzhou_zhinum)
                    {
                        step_Pos++;
                        if (step_Pos == step_value.Count)
                        {
                            Pos_zhou_zhi++;
                        }
                        if (step_Pos > step_value.Count - 1)
                        {
                            step_Pos = 0;
                        }

                        if (Pos_zhou_zhi > 10 || Pos_zhou_zhi > dt.Rows.Count)
                        {
                            Pos_zhou_zhi = 1;
                        }
                        int addPos = (step_value[step_Pos] - 1) * 10 + Pos_zhou_zhi - 1;
                        if (i == boxnum && yushu > 0)
                        {
                            dt.Rows[addPos]["total"] = Convert.ToInt32(dt.Rows[addPos]["total"]) + yushu;
                            break;
                        }
                        else
                        {
                            dt.Rows[addPos]["total"] = Convert.ToInt32(dt.Rows[addPos]["total"]) + kbsrs;
                            step_Pos++;
                            Pos_zhou_zhi++;
                        }
                    }
                    else
                    {
                        int addPos = (step_value[step_Pos] - 1) * 10 + Pos_zhou_zhi - 1;
                        if (i == boxnum && yushu > 0)
                        {
                            dt.Rows[addPos]["total"] = Convert.ToInt32(dt.Rows[addPos]["total"]) + yushu;
                            break;
                        }
                        else
                        {
                            dt.Rows[addPos]["total"] = Convert.ToInt32(dt.Rows[addPos]["total"]) + kbsrs;
                            step_Pos++;
                            Pos_zhou_zhi++;
                        }
                    }
                }
            }
            return false;
        }
    }

    public class PZlogic_n : FS0610_Logic
    {
        public override bool dt_result(ref DataTable dt, int kbsrs, int yushu, int boxnum, int srs)//kbsrs 每次分配的看板数 yushu 余数  boxnum分配的次数
        {

            int loop = boxnum % Convert.ToInt32(dt_zhiInfo.Rows[0]["total"]);
            int Pjnum = boxnum / Convert.ToInt32(dt_zhiInfo.Rows[0]["total"]);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (i == dt.Rows.Count - 1 && yushu != 0)
                {
                    dt.Rows[i]["total"] = ((Pjnum - 1) * kbsrs) + yushu + Convert.ToInt32(dt.Rows[i]["total"]);
                }
                else
                {
                    dt.Rows[i]["total"] = (Pjnum * kbsrs) + Convert.ToInt32(dt.Rows[i]["total"]);
                }
            }
            AddNUM(ref dtchkNUM, dt, srs);
            for (int i = 0; i < loop; i++)
            {
                if (step_Pos2 >= step_value2.Max())
                    step_Pos2 = 0;
                //DataTable tmp = dt.Select(" zhou='" + step_value2[step_Pos2] + "' and zhi<>'9999'", "total,zhi").CopyToDataTable();
                DataTable tmp = dt.Select(" zhou='" + step_value2[step_Pos2] + "' and zhi<>'9999'").CopyToDataTable();
                string[] colname = new string[tmp.Rows.Count];
                for (int j = 0; j < tmp.Rows.Count; j++)
                {
                    colname[j] = tmp.Rows[j]["days"].ToString();
                }
                DataTable dttmp = dtchkNUM.DefaultView.ToTable(false, colname);
                Dictionary<string, int> dic = new Dictionary<string, int>();
                for (int k = 0; k < dttmp.Columns.Count; k++)
                {
                    dic.Add(dttmp.Columns[k].ColumnName, Convert.ToInt32(dttmp.Rows[0][k]));
                }
                Dictionary<string, int> dicASC = dic.OrderBy(p => p.Value).ToDictionary(p => p.Key, p => p.Value);
                string minDays = dicASC.ElementAt(0).Key.ToString();
                DataRow[] mindr = tmp.Select("days ='" + minDays + "'");
                //取该周总数最小的值
                int updaterow = Convert.ToInt32(mindr[0]["zhi"]) - 1;
                if (i == loop && yushu > 0)
                {
                    dt.Rows[updaterow]["total"] = Convert.ToInt32(dt.Rows[updaterow]["total"]) + yushu;
                    dtchkNUM.Rows[0][dt.Rows[updaterow]["days"].ToString()] = Convert.ToInt32(dtchkNUM.Rows[0][dt.Rows[updaterow]["days"].ToString()]) + yushu * srs;
                    break;
                }
                else
                {
                    dt.Rows[updaterow]["total"] = Convert.ToInt32(dt.Rows[updaterow]["total"]) + kbsrs;
                    dtchkNUM.Rows[0][dt.Rows[updaterow]["days"].ToString()] = Convert.ToInt32(dtchkNUM.Rows[0][dt.Rows[updaterow]["days"].ToString()]) + kbsrs * srs;
                    step_Pos2++;
                }
            }
            return true;
        }
    }
    #endregion
}
