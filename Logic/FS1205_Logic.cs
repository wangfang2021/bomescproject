using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using DataAccess;
using Common;
using System.Collections;
using System.Linq;
using System.Text;

namespace Logic
{
    public class FS1205_Logic
    {
        FS1205_DataAccess fs_da = new FS1205_DataAccess();
        private MultiExcute excute = new MultiExcute();

        #region 导入Excel文件数据校验 - 李兴旺整理

        public string checkExcelHeadpos(DataTable dt, DataTable dtTmplate)
        {
            string msg = "";
            if (dt.Columns.Count != dtTmplate.Columns.Count)
            {
                return msg = "使用模板错误！";
            }
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (dt.Columns[i].ColumnName.ToString().Trim() != dtTmplate.Columns[i].ColumnName.ToString().Trim())
                {
                    if (ExcelPos(i) != "error")
                        return msg = "模板" + ExcelPos(i) + "列错误！";
                }
            }
            return msg;
        }

        public string ExcelPos(int i)//取得列位置
        {
            string re = "error";
            List<string> A = new List<string>();
            A.Add("A");
            A.Add("B");
            A.Add("C");
            A.Add("D");
            A.Add("E");
            A.Add("F");
            A.Add("G");
            A.Add("H");
            A.Add("I");
            A.Add("J");
            A.Add("K");
            A.Add("L");
            A.Add("M");
            A.Add("N");
            A.Add("O");
            A.Add("P");
            A.Add("Q");
            A.Add("R");
            A.Add("S");
            A.Add("T");
            A.Add("U");
            A.Add("V");
            A.Add("W");
            A.Add("X");
            A.Add("Y");
            A.Add("Z");
            if (i < 26) re = A[i];
            if (i >= 26) re = A[(i / 26) - 1] + A[i % 26];
            return re;
        }
        #endregion

        #region 初始化计划类别 - 李兴旺整理
        public DataTable getPlantype()
        {
            string ssql = " select '<-请选择->' as planType ,'' as value union all select planType,value from dbo.sPlanType where enable ='1'";
            DataTable dt = excute.ExcuteSqlWithSelectToDT(ssql);
            return dt;
        }
        #endregion

        #region 获取品番信息 - 李兴旺整理
        public DataTable getPartsno(string mon)
        {
            string tmpmon = mon + "-01";
            string ssql = "select vcPartsNo,vcDock,vcInOutFlag,vcQFflag from tPartInfoMaster where dTimeFrom <='" + tmpmon + "' and dTimeTo>='" + tmpmon + "' ";
            return excute.ExcuteSqlWithSelectToDT(ssql);
        }
        #endregion

        #region 导出计划相关 - 李兴旺
        //导出计划 - 加列
        public static void addEDflag(ref DataTable dt, string flag)
        {
            DataColumn col = new DataColumn();
            col.ColumnName = "vcEDflag";
            col.DataType = typeof(string);
            col.DefaultValue = flag;
            dt.Columns.Add(col);
        }

        //导出计划 - 内制品转换相应外注

        public DataTable tranNZtoWZ(DataTable dtNZ, string mon)
        {
            DataTable dtWz = new DataTable();
            DataTable dtInfo = getPartsno(mon);
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

        //导出计划 - 外注品转换相应内制品
        public DataTable tranWztoNz(DataTable dtWz, string mon)
        {
            DataTable dtNz = new DataTable();
            DataTable dtInfo = getPartsno(mon);
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

        //导出计划 - 按计划类别的检索方法

        public DataTable serchData(string strMonth, string strWeek, string strPlant, string strPlanValue)
        {
            DataTable dt = new DataTable();
            //根据计划类型和对象周，找相应的列

            switch (strPlanValue)
            {
                case "1"://生产计划  值别
                    {
                        dt = fs_da.getMonPackPlanTMP(strMonth, "WeekProdPlanTbl", strPlant);
                        addEDflag(ref dt, "通常");
                        dt.TableName = "WeekPro1" + "-#" + strPlant;
                        break;
                    }
                case "0"://包装计划  值别
                    {
                        dt = fs_da.getMonPackPlanTMP(strMonth, "WeekPackPlanTbl", strPlant);
                        dt = tranNZtoWZ(dt, strMonth);
                        addEDflag(ref dt, "通常");
                        dt.TableName = "WeekPro4" + "-#" + strPlant;
                        break;
                    }
                case "2"://看板打印计划  值别
                    {
                        dt = fs_da.getMonPackPlanTMP(strMonth, "WeekKanBanPlanTbl", strPlant);
                        addEDflag(ref dt, "通常");
                        dt.TableName = "WeekPro0" + "-#" + strPlant;
                        break;
                    }
                case "3"://丰铁看板涂装计划  值别
                    {
                        dt = fs_da.getMonPackPlanTMP(strMonth, "WeekTZPlanTbl", strPlant);
                        dt = tranNZtoWZ(dt, strMonth);
                        addEDflag(ref dt, "通常");
                        dt.TableName = "WeekPro2" + "-#" + strPlant;
                        break;
                    }
                case "4"://P3计划  值别
                    {
                        dt = fs_da.getMonPackPlanTMP(strMonth, "WeekP3PlanTbl", strPlant);
                        dt = tranNZtoWZ(dt, strMonth);
                        addEDflag(ref dt, "通常");
                        dt.TableName = "WeekPro3" + "-#" + strPlant;
                        break;
                    }
            }
            dt.Columns["vcEDflag"].SetOrdinal(5);
            return dt;
        }
        #endregion

        #region 转换成10位品番-去00
        //转换成10位品番-去00
        public static DataTable PartsNoFomatTo10(ref DataTable dt)
        {
            foreach (var row in dt.AsEnumerable())
            {
                string part = row.Field<string>("vcPartsno").ToString().Trim();
                if (part.Substring(part.Length - 2, 2) == "00" && part.Replace("-", "").Trim().Length == 12)
                {
                    row.SetField<string>("vcPartsno", part.Substring(0, part.Length - 3).ToString());
                }
            }
            return dt;
        }
        #endregion

        #region 转换成12位品番-加00
        //转换成12位品番-加00
        public static DataTable PartsNoFomatTo12(ref DataTable dt)
        {
            try
            {
                int a = 0;
                foreach (var row in dt.AsEnumerable())
                {
                    string part = row.Field<string>("vcPartsno").ToString().Replace("-", "").Trim();
                    if (part == null)
                    {
                        int aa = 0;
                    }
                    if (part.Length == 10)
                    {
                        row.SetField<string>("vcPartsno", part.Insert(5, "-") + "-00");
                    }
                    a++;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        #endregion

        #region 查看该月该场是否有计划
        /// <summary>
        /// 查看该月该场是否有计划
        /// </summary>
        /// <param name="txtMon"></param>
        /// <param name="plant"></param>
        /// <returns></returns>
        public string checkPlanExist(string txtMon, string plant)
        {
            return fs_da.checkPlanExist(txtMon, plant);
        }
        #endregion

        #region 根据计划类别导出相关计划 - 李兴旺
        /// <summary>
        /// 根据计划类别导出相关计划
        /// </summary>
        /// <param name="strMonth">对象月</param>
        /// <param name="strWeek">对象周</param>
        /// <param name="strPlant">厂区</param>
        /// <param name="strPlanValue">计划类别</param>
        /// <returns>检索结果</returns>
        public DataTable TXTExportPlan(string strMonth, string strWeek, string strPlant, string strPlanValue)
        {
            DataTable dt = serchData(strMonth, strWeek, strPlant, strPlanValue);
            //对检索结果进行处理：
            //1、除对象周（若跨月则跨月）外所有数值清空；2、总数为该周的总数
            //获取对象周对应的列名（目前暂时不用，以后加不加不好说）

            //int iLT0 = 0; int iLT1 = 0; int iLT2 = 0; int iLT3 = 0; int iLT4 = 0;
            //TXTWeekCreatOrderNoLT(strMonth, strWeek, strPlant, ref iLT0, ref iLT1, ref iLT2, ref iLT3, ref iLT4);
            //switch (strPlanValue)
            //{
            //    case "0"://包装周度计划4
            //        {
            //        } break;
            //    case "2"://看板打印周度计划0
            //        {
            //        } break;
            //    case "1"://生产周度计划1
            //        {
            //        } break;
            //    case "3"://涂装周度计划2
            //        {
            //        } break;
            //    case "4"://工程3周度计划3
            //        {
            //        } break;
            //}
            return dt;
        }
        #endregion

        #region 检索CSV文件用的数据 - 李兴旺
        /// <summary>
        /// 检索CSV文件用的数据
        /// </summary>
        /// <param name="strMonth">对象月</param>
        /// <param name="strWeek">对象周</param>
        /// <param name="strPlant">厂区</param>
        /// <param name="dtCSV">CSV文件数据源</param>
        /// <returns>提示信息</returns>
        public string TXTCSVInfoTableMaker(string strMonth, string strWeek, string strPlant, ref DataTable dtCSV)
        {
            string msg = string.Empty;
            try
            {
                InitCSVDataTable(dtCSV);//初始化CSV信息表
                DataTable dtPercentage = fs_da.getWeekLevelPercentage(strMonth, strWeek, strPlant);//获取周计划变动幅度管理表信息
                DataTable dtSchedule = fs_da.getWeekLevelSchedule(strMonth, strWeek, strPlant);//获取周度订单平准化管理表信息
                DataTable dtWeekPackPlan = fs_da.getWeekPackPlan(strMonth, strPlant);//获取周度包装计划管理表信息
                if (dtPercentage.Rows.Count <= 0)
                {
                    msg = "对象月为" + strMonth + "，第" + strWeek + "周的厂区为" + strPlant + "的周计划变动幅度管理表信息不存在！";
                }
                if (dtSchedule.Rows.Count <= 0)
                {
                    msg = "对象月为" + strMonth + "，第" + strWeek + "周的厂区为" + strPlant + "的周度订单平准化管理表信息不存在！";
                }
                if (dtWeekPackPlan.Rows.Count <= 0)
                {
                    msg = "对象月为" + strMonth + "，厂区为" + strPlant + "的周度包装计划信息不存在！";
                }
                if (msg.Length <= 0)
                {
                    //if (dtPercentage.Rows.Count == dtWeekPackPlan.Rows.Count)
                    //{
                    //}
                    //else
                    //{
                    //    msg = "周计划变动幅度管理表与周度包装计划管理表信息数量不一致！";
                    //}
                    for (int i = 0; i < dtPercentage.Rows.Count; i++)//以周计划变动幅度管理表信息为基准
                    {
                        #region 生成CSV数据循环体

                        //数据准备
                        string _vcMonth = dtPercentage.Rows[i]["vcMonth"].ToString();
                        string _vcPartsno = dtPercentage.Rows[i]["vcPartsno"].ToString();
                        string _vcCarType = dtPercentage.Rows[i]["vcCSVCarFamilyCode"].ToString();
                        string strSelect = "vcMonth='" + _vcMonth + "' and vcPartsno='" + _vcPartsno + "' and vcCarType='" + _vcCarType + "'";
                        DataTable dtRow = dtWeekPackPlan.Select(strSelect).CopyToDataTable();//按对象月、品番、车型筛选周度包装计划管理表信息
                        if (dtRow.Rows.Count > 1)
                        {
                            msg = "品番：" + _vcPartsno + "在对象月为" + _vcMonth + "厂区为" + strPlant + "的周度包装计划中信息不唯一！";
                            break;
                        }
                        else if (dtRow.Rows.Count <= 0)
                        {
                            msg = "品番：" + _vcPartsno + "在对象月为" + _vcMonth + "，第" + strWeek + "周的厂区为" + strPlant + "的周度包装计划中信息不存在！";
                            break;
                        }
                        string _vcCalendar1 = dtRow.Rows[0]["vcProject1"].ToString();
                        int iWeekDays = 0;//对象周天数

                        string _strWeekColumns = TXTWeekColumnName(strPlant, strMonth, _vcCalendar1, strWeek, ref iWeekDays).Replace("D", "vcD");//对象周列名

                        string[] _Week = _strWeekColumns.Split(',');
                        //一周的第一天一定是白值b，结尾可能是b可能是y
                        string by = _Week[_Week.Length - 1].Substring(_Week[_Week.Length - 1].Length - 1);
                        if (by == "b")
                        {
                            string NewItem = _Week[_Week.Length - 1].Substring(0, _Week[_Week.Length - 1].Length - 1) + "y";
                            string[] val = { NewItem };//单元素数组

                            _Week = _Week.Concat(val).ToArray();//单元素数组与原数组连接至尾部
                        }
                        //添加行信息

                        DataRow dr = dtCSV.NewRow();
                        //更新日别部分
                        for (int k = 0; k < _Week.Length; k = k + 2)
                        {
                            //空数据置0（白班夜班都要算）

                            if (dtRow.Rows[0][_Week[k]].ToString() == string.Empty)//白班
                            {
                                dtRow.Rows[0][_Week[k]] = "0";
                            }
                            if (dtRow.Rows[0][_Week[k + 1]].ToString() == string.Empty)//夜班
                            {
                                dtRow.Rows[0][_Week[k + 1]] = "0";
                            }
                            string Item = (Convert.ToInt32(dtRow.Rows[0][_Week[k]].ToString()) + Convert.ToInt32(dtRow.Rows[0][_Week[k + 1]].ToString())).ToString();//日别的数值是白班与夜班的和

                            string ItemName = _Week[k].Substring(0, _Week[k].Length - 1);
                            dr[ItemName] = Item;
                        }
                        //日别部分数据格式化

                        for (int j = 8; j < dr.ItemArray.Length; j++)
                        {
                            if (dr[j].ToString() == string.Empty)
                            {
                                dr[j] = "000000";
                            }
                            else
                            {
                                dr[j] = Convert.ToInt32(dr[j].ToString()).ToString("000000");//六位数字
                            }
                        }
                        dr["vcMonth"] = dtPercentage.Rows[i]["vcMonth"].ToString().Replace("-", "");//CSV文件中的对象月没有横线

                        dr["vcCSVFlag"] = dtPercentage.Rows[i]["vcCSVFlag"];
                        dr["vcOrderNo"] = dtPercentage.Rows[i]["vcOrderNo"];
                        dr["vcCSVItemNo"] = dtPercentage.Rows[i]["vcCSVItemNo"];
                        dr["vcCSVOrderDate"] = dtPercentage.Rows[i]["vcCSVOrderDate"];
                        dr["vcPartsno"] = dtPercentage.Rows[i]["vcPartsno"];
                        dr["vcCSVCarFamilyCode"] = dtPercentage.Rows[i]["vcCSVCarFamilyCode"];
                        dr["vcCSVCpdCompany"] = dtPercentage.Rows[i]["vcCSVCpdCompany"];
                        dtCSV.Rows.Add(dr);
                        #endregion
                    }
                    //按vcCSVItemNo排序
                    DataView dv = dtCSV.DefaultView;
                    dv.Sort = "vcCSVItemNo";
                    dtCSV = dv.ToTable();
                }
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }
            return msg;
        }
        #endregion

        #region 初始化CSV信息表 - 李兴旺
        /// <summary>
        /// 初始化CSV信息表
        /// </summary>
        /// <param name="dt">空数据表</param>
        private void InitCSVDataTable(DataTable dt)
        {
            dt.Columns.Add("vcMonth");//对象月
            dt.Columns.Add("vcCSVFlag");//TXT的标记
            dt.Columns.Add("vcOrderNo");//TXT订单号
            dt.Columns.Add("vcCSVItemNo");//TXT行号
            dt.Columns.Add("vcCSVOrderDate");//TXT发货日期
            dt.Columns.Add("vcPartsno");//品番
            dt.Columns.Add("vcCSVCarFamilyCode");//车型
            dt.Columns.Add("vcCSVCpdCompany");//TXT供应商
            dt.Columns.Add("vcD1");//1~31号
            dt.Columns.Add("vcD2");
            dt.Columns.Add("vcD3");
            dt.Columns.Add("vcD4");
            dt.Columns.Add("vcD5");
            dt.Columns.Add("vcD6");
            dt.Columns.Add("vcD7");
            dt.Columns.Add("vcD8");
            dt.Columns.Add("vcD9");
            dt.Columns.Add("vcD10");
            dt.Columns.Add("vcD11");
            dt.Columns.Add("vcD12");
            dt.Columns.Add("vcD13");
            dt.Columns.Add("vcD14");
            dt.Columns.Add("vcD15");
            dt.Columns.Add("vcD16");
            dt.Columns.Add("vcD17");
            dt.Columns.Add("vcD18");
            dt.Columns.Add("vcD19");
            dt.Columns.Add("vcD20");
            dt.Columns.Add("vcD21");
            dt.Columns.Add("vcD22");
            dt.Columns.Add("vcD23");
            dt.Columns.Add("vcD24");
            dt.Columns.Add("vcD25");
            dt.Columns.Add("vcD26");
            dt.Columns.Add("vcD27");
            dt.Columns.Add("vcD28");
            dt.Columns.Add("vcD29");
            dt.Columns.Add("vcD30");
            dt.Columns.Add("vcD31");
        }
        #endregion

        #region DDL获取并绑定厂区数据 - 李兴旺整理
        public DataTable plantConst()
        {
            string ssql = " select vcData1,vcData2 from dbo.ConstMst where vcDataId ='KBPlant' ";
            return excute.ExcuteSqlWithSelectToDT(ssql);
        }
        #endregion

        #region 获取对象月周数 - 李兴旺
        /// <summary>
        /// 获取对象月周数
        /// </summary>
        /// <param name="strMonth">对象月</param>
        /// <param name="strPlant">工厂</param>
        /// <returns></returns>
        public int TXTWeekCount(string strMonth, string strPlant)
        {
            DataTable dtResult = new DataTable();
            int iWeekCount = 0;
            string vcYear = strMonth.Split('-')[0];
            string vcMonth = strMonth.Split('-')[1];
            string ssql = " select TOP(1) vcWeekCount from WeekCalendarTbl where vcYear='" + vcYear + "' and vcMonth='" + vcMonth + "' and vcPlant='" + strPlant + "' order by vcWeekCount desc ";//添加厂区，并按照周数降序排序
            dtResult = excute.ExcuteSqlWithSelectToDT(ssql);
            if (dtResult.Rows.Count > 0)
            {
                iWeekCount = Convert.ToInt32(dtResult.Rows[0]["vcWeekCount"].ToString());
            }
            else
            {
                iWeekCount = -1;
            }
            return iWeekCount;
        }
        #endregion

        #region 获取周计划变动幅度管理表结构 - 李兴旺
        /// <summary>
        /// 获取周计划变动幅度管理表结构
        /// </summary>
        /// <returns>空表</returns>
        public DataTable TXTCloneWeekLevelPercentage()
        {
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("select * from WeekLevelPercentage where 1=2");//这样一定不会搜到数据，一定是行数为0的表
            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
        }
        #endregion

        #region 获取周度订单平准化管理表结构 - 李兴旺

        /// <summary>
        /// 获取周度订单平准化管理表结构
        /// </summary>
        /// <returns>空表</returns>
        public DataTable TXTCloneWeekLevelSchedule()
        {
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("select * from WeekLevelSchedule where 1=2");//这样一定不会搜到数据，一定是行数为0的表
            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());//获取WeekLevelSchedule表结构

        }
        #endregion

        #region 获取周度计划管理表结构 - 李兴旺

        /// <summary>
        /// 获取周度计划管理表结构，因5个工程的计划表结构完全相同，故只用一个方法即可。

        /// </summary>
        /// <returns>空表</returns>
        public DataTable TXTCloneWeekPlanTbl()
        {
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("select * from WeekPackPlanTbl where 1=2");//这样一定不会搜到数据，一定是行数为0的表
            return excute.ExcuteSqlWithSelectToDT(strSQL.ToString());//获取WeekLevelSchedule表结构

        }
        #endregion

        #region 将从TXT文件导入到的品番数据变成12位 - 李兴旺
        /// <summary>
        /// 将从TXT文件导入到的品番数据变成12位
        /// </summary>
        /// <param name="strPartsNo"></param>
        /// <returns></returns>
        public string TXTPartsNoFomatTo12(string strPartsNo)
        {
            string Partsno = string.Empty;
            if (strPartsNo.Length == 10)
            {
                //如果导入的品番位数为10，则在末尾加00
                Partsno = strPartsNo + "00";
            }
            return Partsno;
        }
        #endregion

        #region 查询周订单TXT文件中的本周本品番订货数量 - 李兴旺
        /// <summary>
        /// 查询周订单TXT文件中的本周本品番订货数量
        /// </summary>
        /// <param name="strMonth">对象月</param>
        /// <param name="strWeek">对象周</param>
        /// <param name="strPlant">厂区</param>
        /// <param name="strGC">工程部署</param>
        /// <param name="strZB">组别</param>
        /// <param name="strPartsNo">品番</param>
        /// <returns>订货数量</returns>
        public int TXTFindOrderingCount(string strMonth, string strWeek, string strPlant, string strGC, string strZB, string strPartsNo)
        {
            int _iResult = 0;
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine(" select vcWeekOrderingCount from WeekLevelPercentage where vcMonth='" + strMonth + "' and vcWeek='" + strWeek + "' and vcPlant='" + strPlant + "' and vcGC='" + strGC + "' and vcZB='" + strZB + "' and vcPartsno = '" + strPartsNo + "' ");
            dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            if (dt.Rows.Count == 1)
            {
                _iResult = Convert.ToInt32(dt.Rows[0]["vcWeekOrderingCount"].ToString());
            }
            else if (dt.Rows.Count > 1)
            {
                _iResult = -1;//结果不唯一
            }
            else
            {
                _iResult = 0;//没有数据
            }
            return _iResult;
        }
        #endregion

        #region 根据品番查询该品番的生产部署和组别 - 李兴旺
        /// <summary>
        /// 根据品番查询该品番的生产部署和组别
        /// </summary>
        /// <param name="strPartsNo">品番</param>
        /// <param name="strMonth">对象月</param>
        /// <param name="strGC">生产部署</param>
        /// <param name="strZB">组别</param>
        public void TXTFindGCAndZB(string strPartsNo, string strMonth, ref string strGC, ref string strZB)
        {
            //string Today = DateTime.Now.Date.ToString("yyyy-MM-dd");
            string Today = strMonth + "-01";
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine(" select vcPorType, vcZB from tPartInfoMaster where vcPartsNo='" + strPartsNo + "' and dTimeFrom<='" + Today + "' and dTimeTo>='" + Today + "' ");
            dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            //确定唯一的一行

            if (dt.Rows.Count == 1)
            {
                strGC = dt.Rows[0]["vcPorType"].ToString();
                strZB = dt.Rows[0]["vcZB"].ToString();
            }
        }
        #endregion

        #region 判定品番是周度还是月度 - 李兴旺

        /// <summary>
        /// 判定品番是周度还是月度

        /// </summary>
        /// <param name="strPartsNo">品番</param>
        /// <param name="strMonth">对象月</param>
        /// <returns>周度为true，月度为false</returns>
        public bool TXTIsPartFrequence(string strPartsNo, string strMonth)
        {
            //string Today = DateTime.Now.Date.ToString("yyyy-MM-dd");
            string Today = strMonth + "-01";
            string PartFrequence = string.Empty;
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine(" select vcPartFrequence from tPartInfoMaster where vcPartsNo='" + strPartsNo + "' and dTimeFrom<='" + Today + "' and dTimeTo>='" + Today + "' ");
            dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            if (dt.Rows.Count > 0)
            {
                //不论结果是否唯一，品番对应的频度都是一样的
                PartFrequence = dt.Rows[0]["vcPartFrequence"].ToString();
                if (PartFrequence == "周度")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 查询品番的厂区 - 李兴旺

        /// <summary>
        /// 查询品番的厂区

        /// </summary>
        /// <param name="strPartsNo">品番</param>
        /// <param name="strMonth">对象月</param>
        /// <returns>厂区</returns>
        public string TXTFindPartPlant(string strPartsNo, string strMonth)
        {
            string _vcPlant = string.Empty;
            //string Today = DateTime.Now.Date.ToString("yyyy-MM-dd");
            string Today = strMonth + "-01";
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine(" select vcPartPlant from tPartInfoMaster where vcPartsNo='" + strPartsNo + "' and dTimeFrom<='" + Today + "' and dTimeTo>='" + Today + "' ");
            dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            if (dt.Rows.Count > 0)
            {
                //不论结果是否唯一，品番对应的厂区都是一样的
                _vcPlant = dt.Rows[0]["vcPartPlant"].ToString();
            }
            return _vcPlant;
        }
        #endregion

        #region 获取品番的看板收容数 - 李兴旺

        /// <summary>
        /// 获取品番的看板收容数
        /// </summary>
        /// <param name="strPartsNo">品番</param>
        /// <param name="strMonth">对象月</param>
        /// <returns>看板收容数</returns>
        public int TXTQuantity(string strPartsNo, string strMonth)
        {
            int Quantity = 0;
            //string Today = DateTime.Now.Date.ToString("yyyy-MM-dd");
            string Today = strMonth + "-01";
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine(" select iQuantityPerContainer from tPartInfoMaster where vcPartsNo='" + strPartsNo + "' and dTimeFrom<='" + Today + "' and dTimeTo>='" + Today + "' ");
            dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            if (dt.Rows.Count > 0)//有数据

            {
                Quantity = Convert.ToInt32(dt.Rows[0]["iQuantityPerContainer"].ToString());
            }
            return Quantity;
        }
        #endregion

        #region 查询品番的车型 - 李兴旺

        /// <summary>
        /// 查询品番的车型

        /// </summary>
        /// <param name="strPartsNo">品番</param>
        /// <param name="strMonth">对象月</param>
        /// <returns>车型</returns>
        public string TXTFindCarType(string strPartsNo, string strMonth)
        {
            string _strCarType = string.Empty;
            //string Today = DateTime.Now.Date.ToString("yyyy-MM-dd");
            string Today = strMonth + "-01";
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine(" select vcCarFamilyCode from tPartInfoMaster where vcPartsNo='" + strPartsNo + "' and dTimeFrom<='" + Today + "' and dTimeTo>='" + Today + "' ");
            dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            if (dt.Rows.Count > 0)
            {
                //不论结果是否唯一，品番对应的车型都是固定的

                _strCarType = dt.Rows[0]["vcCarFamilyCode"].ToString();
            }
            return _strCarType;
        }
        #endregion

        #region 查询品番的受入 - 李兴旺

        /// <summary>
        /// 查询品番的受入

        /// </summary>
        /// <param name="strPartsNo">品番</param>
        /// <param name="strMonth">对象月</param>
        /// <returns>受入</returns>
        public string TXTFindDock(string strPartsNo, string strMonth)
        {
            string _strDock = string.Empty;
            //string Today = DateTime.Now.Date.ToString("yyyy-MM-dd");
            string Today = strMonth + "-01";
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine(" select vcDock from tPartInfoMaster where vcPartsNo='" + strPartsNo + "' and dTimeFrom<='" + Today + "' and dTimeTo>='" + Today + "' ");
            dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            if (dt.Rows.Count > 0)
            {
                //不论结果是否唯一，品番对应的车型都是固定的

                _strDock = dt.Rows[0]["vcDock"].ToString();
            }
            return _strDock;
        }
        #endregion

        #region 根据生产部署和组别查询工程1 - 李兴旺

        /// <summary>
        /// 根据生产部署和组别查询工程1
        /// </summary>
        /// <param name="vcPorType">生产部署</param>
        /// <param name="vcZB">组别</param>
        /// <returns>工程1</returns>
        public string TXTFindProject1(string vcPorType, string vcZB)
        {
            string vcCalendar1 = string.Empty;
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine(" select vcCalendar1 from ProRuleMst where vcPorType='" + vcPorType + "' and vcZB='" + vcZB + "' ");
            dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            vcCalendar1 = dt.Rows[0]["vcCalendar1"].ToString();
            return vcCalendar1;
        }
        #endregion

        #region 根据生产部署和组别查询工程1名称 - 李兴旺

        /// <summary>
        /// 根据生产部署和组别查询工程1名称
        /// </summary>
        /// <param name="vcPorType">生产部署</param>
        /// <param name="vcZB">组别</param>
        /// <returns>工程1名称</returns>
        public string TXTFindProName1(string vcPorType, string vcZB)
        {
            string vcProName1 = string.Empty;
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine(" select vcProName1 from ProRuleMst where vcPorType='" + vcPorType + "' and vcZB='" + vcZB + "' ");
            dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            vcProName1 = dt.Rows[0]["vcProName1"].ToString();
            return vcProName1;
        }
        #endregion

        #region 根据对象月、对象周、工程1、品番获取该品番在月度包装计划里的数据 - 李兴旺

        /// <summary>
        /// 根据对象月、对象周、工程1、品番获取该品番在月度包装计划里的数据

        /// </summary>
        /// <param name="strPlant">厂区</param>
        /// <param name="strMonth">对象月</param>
        /// <param name="strWeek">对象周</param>
        /// <param name="vcCalendar1">工程1</param>
        /// <param name="strPartsNo">品番</param>
        /// <returns>结果列表</returns>
        public DataTable TXTWeekCalendar(string strPlant, string strMonth, string strWeek, string vcCalendar1, string strPartsNo)
        {
            DataTable dt = new DataTable();//查找该品番对应的周稼动日历用
            DataTable dtResult = TXTCloneWeekLevelPercentage();//存放结果
            string vcYear = strMonth.Split('-')[0];
            string vcMonth = strMonth.Split('-')[1];
            string vcGC = vcCalendar1.Split('-')[0];
            string vcZB = vcCalendar1.Split('-')[1];
            string vcMonTotal = string.Empty;//月度内示总量
            string vcTotal = string.Empty;//实际订货数量合计
                                          //string[] strWeekColumnsName = new string[5];//存放五周的列名

            //string[] strWeekColumnsNameMonth = new string[5];//存放五周的月度包装计划列名

            //int[] iWeekNum = new int[5];//存放五周的天数

            int[] iWeekMonthTotal = new int[5];//存放该品番各周的最终实际订货数，默认是内示计划（月度包装计划）周合计

            string strColumnsNameTMP1 = string.Empty;
            string strColumnsNameTMP2 = string.Empty;
            string strColumnsNameTMP3 = string.Empty;
            string strColumnsNameTMP4 = string.Empty;
            string strColumnsNameTMP5 = string.Empty;
            string strColumnsName1 = string.Empty;//第1周所有列名

            string strColumnsName2 = string.Empty;//第2周所有列名

            string strColumnsName3 = string.Empty;//第3周所有列名

            string strColumnsName4 = string.Empty;//第4周所有列名

            string strColumnsName5 = string.Empty;//第5周所有列名

            string strColumnsNameMonth1 = string.Empty;//第1周所有列对应的月度包装计划列名

            string strColumnsNameMonth2 = string.Empty;//第2周所有列对应的月度包装计划列名

            string strColumnsNameMonth3 = string.Empty;//第3周所有列对应的月度包装计划列名

            string strColumnsNameMonth4 = string.Empty;//第4周所有列对应的月度包装计划列名

            string strColumnsNameMonth5 = string.Empty;//第5周所有列对应的月度包装计划列名

            int iWeek1 = 0;//第1周天数

            int iWeek2 = 0;//第2周天数

            int iWeek3 = 0;//第3周天数

            int iWeek4 = 0;//第4周天数

            int iWeek5 = 0;//第5周天数

            int iWeek1MonthTotal = 0;//内示计划（月度包装计划）中第1周订货总数
            int iWeek2MonthTotal = 0;//内示计划（月度包装计划）中第2周订货总数
            int iWeek3MonthTotal = 0;//内示计划（月度包装计划）中第3周订货总数
            int iWeek4MonthTotal = 0;//内示计划（月度包装计划）中第4周订货总数
            int iWeek5MonthTotal = 0;//内示计划（月度包装计划）中第5周订货总数
            string strSQLMonthPackPlanWeek1 = string.Empty;//查询该品番月度包装计划第1周SQL文

            string strSQLMonthPackPlanWeek2 = string.Empty;//查询该品番月度包装计划第2周SQL文

            string strSQLMonthPackPlanWeek3 = string.Empty;//查询该品番月度包装计划第3周SQL文

            string strSQLMonthPackPlanWeek4 = string.Empty;//查询该品番月度包装计划第4周SQL文

            string strSQLMonthPackPlanWeek5 = string.Empty;//查询该品番月度包装计划第5周SQL文

            StringBuilder strSQL = new StringBuilder();
            //查找该品番对应的周稼动日历

            //vcPlant='" + strPlant + "' and 
            strSQL.AppendLine(" select * from WeekCalendarTbl where vcPlant='" + strPlant + "' and vcYear='" + vcYear + "' and vcMonth='" + vcMonth + "' and vcGC='" + vcGC + "' and vcZB='" + vcZB + "'");
            //确定唯一的一行数据，包括iAutoID
            dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            //WeekCalendarTbl表的第6列（0开始），小于列数-2结束，

            for (int i = 6; i < dt.Columns.Count - 2; i++)
            {
                //根据所选对象周查询该月该周的天数和对应列名
                switch (dt.Rows[0][i].ToString())
                {
                    case "1":
                        {
                            iWeek1++;
                            strColumnsNameTMP1 += dt.Columns[i].ColumnName + ",";
                        }; break;
                    case "2":
                        {
                            iWeek2++;
                            strColumnsNameTMP2 += dt.Columns[i].ColumnName + ",";
                        }; break;
                    case "3":
                        {
                            iWeek3++;
                            strColumnsNameTMP3 += dt.Columns[i].ColumnName + ",";
                        }; break;
                    case "4":
                        {
                            iWeek4++;
                            strColumnsNameTMP4 += dt.Columns[i].ColumnName + ",";
                        }; break;
                    case "5":
                        {
                            iWeek5++;
                            strColumnsNameTMP5 += dt.Columns[i].ColumnName + ",";
                        }; break;
                }
            }
            //将各周天数存起来
            //iWeekNum[0] = iWeek1;
            //iWeekNum[1] = iWeek2;
            //iWeekNum[2] = iWeek3;
            //iWeekNum[3] = iWeek4;
            //iWeekNum[4] = iWeek5;
            //稼动日数>0，对应的字符串一定不为空
            //获取每周的周合计
            if (iWeek1 > 0)
            {
                strColumnsName1 = strColumnsNameTMP1.Substring(0, strColumnsNameTMP1.Length - 1);
                strColumnsNameMonth1 = strColumnsName1.Replace("D", "vcD");
                strSQLMonthPackPlanWeek1 = "select vcMonTotal," + strColumnsNameMonth1 + " from MonthPackPlanTbl where vcMonth='" + strMonth + "' and vcPartsno='" + strPartsNo + "' and vcProject1='" + vcCalendar1 + "' and (montouch='' or montouch is null) ";
                DataTable dt1 = excute.ExcuteSqlWithSelectToDT(strSQLMonthPackPlanWeek1);
                if (dt1.Rows.Count <= 0)//月度包装计划里没有数据时
                {
                    vcMonTotal = "0";
                    iWeekMonthTotal[0] = 0;
                }
                else
                {
                    vcMonTotal = dt1.Rows[0]["vcMonTotal"].ToString();
                    for (int i = 1; i < dt1.Columns.Count; i++)
                    {
                        if (dt1.Rows[0][i].ToString() != string.Empty)
                        {
                            iWeek1MonthTotal = iWeek1MonthTotal + Convert.ToInt32(dt1.Rows[0][i].ToString());
                        }
                    }
                    iWeekMonthTotal[0] = iWeek1MonthTotal;
                }
            }
            if (iWeek2 > 0)
            {
                strColumnsName2 = strColumnsNameTMP2.Substring(0, strColumnsNameTMP2.Length - 1);
                strColumnsNameMonth2 = strColumnsName2.Replace("D", "vcD");
                strSQLMonthPackPlanWeek2 = "select vcMonTotal," + strColumnsNameMonth2 + " from MonthPackPlanTbl where vcMonth='" + strMonth + "' and vcPartsno='" + strPartsNo + "' and vcProject1='" + vcCalendar1 + "' and (montouch='' or montouch is null) ";
                DataTable dt2 = excute.ExcuteSqlWithSelectToDT(strSQLMonthPackPlanWeek2);
                if (dt2.Rows.Count <= 0)
                {
                    vcMonTotal = "0";
                    iWeekMonthTotal[1] = 0;
                }
                else
                {
                    vcMonTotal = dt2.Rows[0]["vcMonTotal"].ToString();
                    for (int i = 1; i < dt2.Columns.Count; i++)
                    {
                        if (dt2.Rows[0][i].ToString() != string.Empty)
                        {
                            iWeek2MonthTotal = iWeek2MonthTotal + Convert.ToInt32(dt2.Rows[0][i].ToString());
                        }
                    }
                    iWeekMonthTotal[1] = iWeek2MonthTotal;
                }
            }
            if (iWeek3 > 0)
            {
                strColumnsName3 = strColumnsNameTMP3.Substring(0, strColumnsNameTMP3.Length - 1);
                strColumnsNameMonth3 = strColumnsName3.Replace("D", "vcD");
                strSQLMonthPackPlanWeek3 = "select vcMonTotal," + strColumnsNameMonth3 + " from MonthPackPlanTbl where vcMonth='" + strMonth + "' and vcPartsno='" + strPartsNo + "' and vcProject1='" + vcCalendar1 + "' and (montouch='' or montouch is null) ";
                DataTable dt3 = excute.ExcuteSqlWithSelectToDT(strSQLMonthPackPlanWeek3);
                if (dt3.Rows.Count <= 0)
                {
                    vcMonTotal = "0";
                    iWeekMonthTotal[2] = 0;
                }
                else
                {
                    vcMonTotal = dt3.Rows[0]["vcMonTotal"].ToString();
                    for (int i = 1; i < dt3.Columns.Count; i++)
                    {
                        if (dt3.Rows[0][i].ToString() != string.Empty)
                        {
                            iWeek3MonthTotal = iWeek3MonthTotal + Convert.ToInt32(dt3.Rows[0][i].ToString());
                        }
                    }
                    iWeekMonthTotal[2] = iWeek3MonthTotal;
                }
            }
            if (iWeek4 > 0)
            {
                strColumnsName4 = strColumnsNameTMP4.Substring(0, strColumnsNameTMP4.Length - 1);
                strColumnsNameMonth4 = strColumnsName4.Replace("D", "vcD");
                strSQLMonthPackPlanWeek4 = "select vcMonTotal," + strColumnsNameMonth4 + " from MonthPackPlanTbl where vcMonth='" + strMonth + "' and vcPartsno='" + strPartsNo + "' and vcProject1='" + vcCalendar1 + "' and (montouch='' or montouch is null) ";
                DataTable dt4 = excute.ExcuteSqlWithSelectToDT(strSQLMonthPackPlanWeek4);
                if (dt4.Rows.Count <= 0)
                {
                    vcMonTotal = "0";
                    iWeekMonthTotal[3] = 0;
                }
                else
                {
                    vcMonTotal = dt4.Rows[0]["vcMonTotal"].ToString();
                    for (int i = 1; i < dt4.Columns.Count; i++)
                    {
                        if (dt4.Rows[0][i].ToString() != string.Empty)
                        {
                            iWeek4MonthTotal = iWeek4MonthTotal + Convert.ToInt32(dt4.Rows[0][i].ToString());
                        }
                    }
                    iWeekMonthTotal[3] = iWeek4MonthTotal;
                }
            }
            if (iWeek5 > 0)
            {
                strColumnsName5 = strColumnsNameTMP5.Substring(0, strColumnsNameTMP5.Length - 1);
                strColumnsNameMonth5 = strColumnsName5.Replace("D", "vcD");
                strSQLMonthPackPlanWeek5 = "select vcMonTotal," + strColumnsNameMonth5 + " from MonthPackPlanTbl where vcMonth='" + strMonth + "' and vcPartsno='" + strPartsNo + "' and vcProject1='" + vcCalendar1 + "' and (montouch='' or montouch is null) ";
                DataTable dt5 = excute.ExcuteSqlWithSelectToDT(strSQLMonthPackPlanWeek5);
                if (dt5.Rows.Count <= 0)
                {
                    vcMonTotal = "0";
                    iWeekMonthTotal[4] = 0;
                }
                else
                {
                    vcMonTotal = dt5.Rows[0]["vcMonTotal"].ToString();
                    for (int i = 1; i < dt5.Columns.Count; i++)
                    {
                        iWeek5MonthTotal = iWeek5MonthTotal + Convert.ToInt32(dt5.Rows[0][i].ToString());
                    }
                    iWeekMonthTotal[4] = iWeek5MonthTotal;
                }
            }
            //如果该品番各周的订单确认（订单确认后肯定会平准化）结果已经有了，则每周的订货总数由内示总数更新为实际订货总数
            for (int i = 1; i <= 5; i++)//第1周到第5周

            {
                if (TXTExistInWeekLevelSchedule(strMonth, i.ToString(), strPartsNo, strPlant) > 0)
                {
                    //大于0就是有记录

                    string ColumnName = string.Empty;//对应周的实际订货数量的列名

                    switch (i.ToString())
                    {
                        case "1": ColumnName = "vc1stWeekTotal"; break;
                        case "2": ColumnName = "vc2ndWeekTotal"; break;
                        case "3": ColumnName = "vc3rdWeekTotal"; break;
                        case "4": ColumnName = "vc4thWeekTotal"; break;
                        case "5": ColumnName = "vc5thWeekTotal"; break;
                    }
                    string SQLTMP = " select " + ColumnName + " from WeekLevelPercentage where vcMonth='" + strMonth + "' and vcWeek='" + i.ToString() + "' and vcPartsno='" + strPartsNo + "' and vcPlant='" + strPlant + "' ";
                    DataTable dtTMP = excute.ExcuteSqlWithSelectToDT(SQLTMP);
                    iWeekMonthTotal[i - 1] = Convert.ToInt32(dtTMP.Rows[0][ColumnName].ToString());
                }
            }
            int iTotal = 0;
            for (int i = 0; i < 5; i++)
            {
                iTotal = iTotal + iWeekMonthTotal[i];
            }
            vcTotal = iTotal.ToString();
            //该品番在月度包装计划中的记录
            string SQLTMP2 = "select * from MonthPackPlanTbl where vcMonth='" + strMonth + "' and vcPartsno='" + strPartsNo + "' and vcProject1='" + vcCalendar1 + "' and (montouch='' or montouch is null) ";
            DataTable dtMonthPackPlan = excute.ExcuteSqlWithSelectToDT(SQLTMP2);
            if (dtMonthPackPlan.Rows.Count <= 0)//没有月度计划时，相当于真紧急订单

            {
                DataRow drMonthPackPlan = dtMonthPackPlan.NewRow();
                drMonthPackPlan["vcMonth"] = strMonth;
                drMonthPackPlan["vcPartsno"] = strPartsNo;
                drMonthPackPlan["vcProject1"] = vcCalendar1;
                drMonthPackPlan["vcMonTotal"] = "0";//月总数为0
                for (int i = 7; i < dtMonthPackPlan.Columns.Count - 4; i++)
                {
                    drMonthPackPlan[i] = "0";//该月每天的计划数为0
                }
                dtMonthPackPlan.Rows.Add(drMonthPackPlan);
            }
            //新建行，准备插入数据，按照对应的周列插入数据
            DataRow dr = dtResult.NewRow();
            dr["vcMonTotal"] = vcMonTotal;
            switch (strWeek)
            {
                case "1":
                    {
                        for (int i = 0; i < iWeek1; i++)
                        {
                            dr[strColumnsName1.Split(',')[i]] = dtMonthPackPlan.Rows[0][strColumnsNameMonth1.Split(',')[i]];
                        }
                        dr["vcWeekTotal"] = iWeek1MonthTotal.ToString();
                    }
                    break;
                case "2":
                    {
                        for (int i = 0; i < iWeek2; i++)
                        {
                            dr[strColumnsName2.Split(',')[i]] = dtMonthPackPlan.Rows[0][strColumnsNameMonth2.Split(',')[i]];
                        }
                        dr["vcWeekTotal"] = iWeek2MonthTotal.ToString();
                    }
                    break;
                case "3":
                    {
                        for (int i = 0; i < iWeek3; i++)
                        {
                            dr[strColumnsName3.Split(',')[i]] = dtMonthPackPlan.Rows[0][strColumnsNameMonth3.Split(',')[i]];
                        }
                        dr["vcWeekTotal"] = iWeek3MonthTotal.ToString();
                    }
                    break;
                case "4":
                    {
                        for (int i = 0; i < iWeek4; i++)
                        {
                            dr[strColumnsName4.Split(',')[i]] = dtMonthPackPlan.Rows[0][strColumnsNameMonth4.Split(',')[i]];
                        }
                        dr["vcWeekTotal"] = iWeek4MonthTotal.ToString();
                    }
                    break;
                case "5":
                    {
                        for (int i = 0; i < iWeek5; i++)
                        {
                            dr[strColumnsName5.Split(',')[i]] = dtMonthPackPlan.Rows[0][strColumnsNameMonth5.Split(',')[i]];
                        }
                        dr["vcWeekTotal"] = iWeek5MonthTotal.ToString();
                    }
                    break;
            }
            dr["vc1stWeekTotal"] = iWeekMonthTotal[0].ToString();
            dr["vc2ndWeekTotal"] = iWeekMonthTotal[1].ToString();
            dr["vc3rdWeekTotal"] = iWeekMonthTotal[2].ToString();
            dr["vc4thWeekTotal"] = iWeekMonthTotal[3].ToString();
            dr["vc5thWeekTotal"] = iWeekMonthTotal[4].ToString();
            dr["vcTotal"] = vcTotal;
            dtResult.Rows.Add(dr);
            return dtResult;
        }
        #endregion

        #region 获取该品番前N-1周的实际总数 - 李兴旺

        /// <summary>
        /// 获取该品番前N-1周的实际总数
        /// </summary>
        /// <param name="strMonth">对象月</param>
        /// <param name="strWeek">对象周</param>
        /// <param name="strPartsNo">品番</param>
        /// <param name="strPlant">厂区</param>
        /// <returns>前N-1周的实际总数</returns>
        public string TXTRealTotal(string strMonth, string strWeek, string strPartsNo, string strPlant)
        {
            string strRealTotal = string.Empty;
            int[] iWeekMonthTotal = new int[5];
            if (strWeek == "1")
            {
                //如果是第一周，总数设为0
                return "0";
            }
            else
            {
                //不是第一周，是第N周，则计算第一周到第N-1周的总数
                int iWeekNum = Convert.ToInt32(strWeek);//当前的第N周

                for (int i = 1; i <= iWeekNum - 1; i++)//第1周到第N-1周

                {
                    if (TXTExistInWeekLevelPercentage(strMonth, i.ToString(), strPartsNo, strPlant) > 0)
                    {
                        //大于0就是有记录

                        string ColumnName = string.Empty;//对应周的实际订货数量的列名

                        switch (i.ToString())
                        {
                            case "1": ColumnName = "vc1stWeekTotal"; break;
                            case "2": ColumnName = "vc2ndWeekTotal"; break;
                            case "3": ColumnName = "vc3rdWeekTotal"; break;
                            case "4": ColumnName = "vc4thWeekTotal"; break;
                            case "5": ColumnName = "vc5thWeekTotal"; break;
                        }
                        string SQLTMP = " select " + ColumnName + " from WeekLevelPercentage where vcMonth='" + strMonth + "' and vcWeek='" + i.ToString() + "' and vcPartsno='" + strPartsNo + "' and vcPlant='" + strPlant + "' ";
                        DataTable dtTMP = excute.ExcuteSqlWithSelectToDT(SQLTMP);
                        iWeekMonthTotal[i - 1] = Convert.ToInt32(dtTMP.Rows[0][ColumnName].ToString());
                    }
                }
                int iTotal = 0;
                for (int i = 0; i < 5; i++)
                {
                    iTotal = iTotal + iWeekMonthTotal[i];
                }
                strRealTotal = iTotal.ToString();
                return strRealTotal;
            }
        }
        #endregion

        #region 根据对象月和工程1查询该月稼动数（没用上） - 李兴旺

        /// <summary>
        /// 根据对象月和工程1查询该月稼动数

        /// </summary>
        /// <param name="strPlant">厂区</param>
        /// <param name="strMonth">对象月</param>
        /// <param name="strProject1">工程1</param>
        /// <returns>稼动数（字符串形式）</returns>
        public string TXTWeekMonthTotal(string strPlant, string strMonth, string strProject1)
        {
            string vcTotal = string.Empty;
            string vcYear = strMonth.Split('-')[0];
            string vcMonth = strMonth.Split('-')[1];
            string vcGC = strProject1.Split('-')[0];
            string vcZB = strProject1.Split('-')[1];
            DataTable dt = new DataTable();//查找该品番对应的周稼动日历用
            StringBuilder strSQL = new StringBuilder();
            //查找该品番对应的周稼动日历

            //vcPlant='" + strPlant + "' and 
            strSQL.AppendLine(" select * from WeekCalendarTbl where vcPlant='" + strPlant + "' and vcYear='" + vcYear + "' and vcMonth='" + vcMonth + "' and vcGC='" + vcGC + "' and vcZB='" + vcZB + "'");
            //确定唯一的一行数据，包括iAutoID
            dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            vcTotal = dt.Rows[0]["vcTotal"].ToString();
            return vcTotal;
        }
        #endregion

        #region 根据对象月和工程1，查询对象周对应的列名和天数 - 李兴旺

        /// <summary>
        /// 根据对象月和工程1，查询对象周对应的列名和天数
        /// </summary>
        /// <param name="strPlant">厂区</param>
        /// <param name="strMonth">对象月</param>
        /// <param name="strProject1">工程1</param>
        /// <param name="strWeek">对象周</param>
        /// <returns>对象周对应的列名</returns>
        public string TXTWeekColumnName(string strPlant, string strMonth, string strProject1, string strWeek, ref int iWeekNum)
        {
            string strWeekColumnName = string.Empty;
            string strColumnsNameTMP = string.Empty;
            string vcYear = strMonth.Split('-')[0];
            string vcMonth = strMonth.Split('-')[1];
            string vcGC = strProject1.Split('-')[0];
            string vcZB = strProject1.Split('-')[1];
            iWeekNum = 0;
            DataTable dt = new DataTable();//查找该品番对应的周稼动日历用
            StringBuilder strSQL = new StringBuilder();
            //查找该品番对应的周稼动日历

            //vcPlant='" + strPlant + "' and 
            strSQL.AppendLine(" select * from WeekCalendarTbl where vcPlant='" + strPlant + "' and vcYear='" + vcYear + "' and vcMonth='" + vcMonth + "' and vcGC='" + vcGC + "' and vcZB='" + vcZB + "'");
            //确定唯一的一行数据，包括iAutoID
            dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            //WeekCalendarTbl表的第6列（0开始），小于列数-2结束，

            for (int i = 6; i < dt.Columns.Count - 2; i++)
            {
                //根据所选对象周查询该月该周的天数和对应列名
                if (dt.Rows[0][i].ToString() == strWeek)
                {
                    switch (strWeek)
                    {
                        case "1":
                            {
                                iWeekNum++;
                                strColumnsNameTMP += dt.Columns[i].ColumnName + ",";
                            }; break;
                        case "2":
                            {
                                iWeekNum++;
                                strColumnsNameTMP += dt.Columns[i].ColumnName + ",";
                            }; break;
                        case "3":
                            {
                                iWeekNum++;
                                strColumnsNameTMP += dt.Columns[i].ColumnName + ",";
                            }; break;
                        case "4":
                            {
                                iWeekNum++;
                                strColumnsNameTMP += dt.Columns[i].ColumnName + ",";
                            }; break;
                        case "5":
                            {
                                iWeekNum++;
                                strColumnsNameTMP += dt.Columns[i].ColumnName + ",";
                            }; break;
                    }
                }
            }
            //稼动日数>0，对应的字符串一定不为空
            if (iWeekNum > 0)
            {
                strWeekColumnName = strColumnsNameTMP.Substring(0, strColumnsNameTMP.Length - 1);
            }
            return strWeekColumnName;
        }
        #endregion

        #region 根据对象月和对象周，查询对象周在WeekLevelPercentage表对应的列名 - 李兴旺

        /// <summary>
        /// 根据对象月和对象周，查询对象周在WeekLevelPercentage表对应的列名
        /// </summary>
        /// <param name="strMonth">对象月</param>
        /// <param name="strWeek">对象周</param>
        /// <returns>对应的列名</returns>
        public string TXTWeekColumnName(string strMonth, string strWeek)
        {
            string strWeekColumnName = string.Empty;
            string strColumnsNameTMP = string.Empty;
            int iWeekNum = 0;
            DataTable dt = new DataTable();//查找WeekLevelPercentage
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine(" select top(1) * from WeekLevelPercentage where vcMonth='" + strMonth + "' and vcWeek='" + strWeek + "' ");
            dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            //WeekLevelPercentage表的第9列（0开始），小于列数-9结束，

            for (int i = 9; i < dt.Columns.Count - 9; i++)
            {
                //所选对象周对应的列下有数据，不是该周的一定没有

                if (dt.Rows[0][i].ToString() != string.Empty)
                {
                    iWeekNum++;
                    strColumnsNameTMP += dt.Columns[i].ColumnName + ",";
                }
            }
            //稼动日数>0，对应的字符串一定不为空
            if (iWeekNum > 0)
            {
                strWeekColumnName = strColumnsNameTMP.Substring(0, strColumnsNameTMP.Length - 1);
            }
            return strWeekColumnName;
        }
        #endregion

        #region 确定某品番在某月某周在周计划变动幅度管理表是否有记录 - 李兴旺

        /// <summary>
        /// 确定某品番在某月某周在周计划变动幅度管理表是否有记录
        /// </summary>
        /// <param name="strMonth">对象月</param>
        /// <param name="strWeek">对象周</param>
        /// <param name="strPartsNo">品番</param>
        /// <param name="strPlant">厂区</param>
        /// <returns>记录的行数</returns>
        public int TXTExistInWeekLevelPercentage(string strMonth, string strWeek, string strPartsNo, string strPlant)
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine(" select * from WeekLevelPercentage where vcMonth='" + strMonth + "' and vcWeek='" + strWeek + "' and vcPartsno='" + strPartsNo + "' and vcPlant='" + strPlant + "' ");
            dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            return dt.Rows.Count;
        }
        #endregion

        #region 确定某品番在某月某周在周度订单平准化管理表是否有记录 - 李兴旺

        /// <summary>
        /// 确定某品番在某月某周在周度订单平准化管理表是否有记录
        /// </summary>
        /// <param name="strMonth">对象月</param>
        /// <param name="strWeek">对象周</param>
        /// <param name="strPartsNo">品番</param>
        /// <param name="strPlant">厂区</param>
        /// <returns>记录的行数</returns>
        public int TXTExistInWeekLevelSchedule(string strMonth, string strWeek, string strPartsNo, string strPlant)
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine(" select * from WeekLevelSchedule where vcMonth='" + strMonth + "' and vcWeek='" + strWeek + "' and vcPartsno='" + strPartsNo + "' and vcPlant='" + strPlant + "' ");
            dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            return dt.Rows.Count;
        }
        #endregion

        #region 将数据更新到周计划变动幅度管理表WeekLevelPercentage - 李兴旺

        /// <summary>
        /// 将数据更新到周计划变动幅度管理表WeekLevelPercentage
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="strMonth">对象月</param>
        /// <param name="strOrderNo">订单号</param>
        /// <param name="strWeek">对象周</param>
        /// <param name="strPlant">厂区</param>
        public void TXTUpdateTableDetermine(DataTable dt, string strMonth, string strOrderNo, string strWeek, string strPlant)
        {
            string strSQLSearch = " select * from WeekLevelPercentage ";
            StringBuilder strSQL = new StringBuilder();
            //删周计划变动幅度管理表

            strSQL.AppendLine(" delete from WeekLevelPercentage where vcMonth='" + strMonth + "' and vcOrderNo='" + strOrderNo + "' and vcWeek='" + strWeek + "' and vcPlant='" + strPlant + "'; ");
            //删周度订单平准化管理表

            strSQL.AppendLine(" delete from WeekLevelSchedule where vcMonth='" + strMonth + "' and vcWeek='" + strWeek + "' and vcPlant='" + strPlant + "'; ");
            //删计划（暂无）

            strSQL.AppendLine("");
            string strDeleteSQL = strSQL.ToString();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(ComConnectionHelper.GetConnectionString());
            cmd.CommandTimeout = 0;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = strDeleteSQL;
            cmd.Connection.Open();
            cmd.Transaction = cmd.Connection.BeginTransaction();
            try
            {
                //先执行删除

                cmd.ExecuteNonQuery();
                //再改为检索

                cmd.CommandText = strSQLSearch;
                SqlDataAdapter apt = new SqlDataAdapter(cmd);
                DataTable dtupdate = new DataTable();
                apt.Fill(dtupdate);
                for (int k = 0; k < dt.Rows.Count; k++)
                {
                    DataRow dr = dtupdate.NewRow();
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        dr[i] = dt.Rows[k][i];
                    }
                    dtupdate.Rows.Add(dr);
                }
                SqlCommandBuilder sb = new SqlCommandBuilder(apt);
                apt.Update(dtupdate);
                apt.Dispose();
                sb.Dispose();
                cmd.Transaction.Commit();
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                cmd.Transaction.Rollback();
                cmd.Connection.Close();
                throw ex;
            }
        }
        #endregion

        #region 根据对象月和对象周检索周计划变动幅度管理表WeekLevelPercentage - 李兴旺

        /// <summary>
        /// 根据对象月和对象周检索周计划变动幅度管理表WeekLevelPercentage
        /// </summary>
        /// <param name="strMonth">对象月</param>
        /// <param name="strWeek">对象周</param>
        /// <returns>检索结果数据表</returns>
        public DataTable TXTSearchWeekLevelPercentage(string strMonth, string strWeek, string strPlant)
        {
            DataTable dt = new DataTable();
            DataTable dtResult = new DataTable();
            StringBuilder strSQL1 = new StringBuilder();
            StringBuilder strSQL2 = new StringBuilder();
            //先检索确定有没有数据
            strSQL1.AppendLine(" select * from WeekLevelPercentage where vcMonth='" + strMonth + "' and vcWeek='" + strWeek + "' and vcPlant='" + strPlant + "' order by vcFlag ");//【201812103】把NG放前面

            dt = excute.ExcuteSqlWithSelectToDT(strSQL1.ToString());
            return dt;
            //if (dt.Rows.Count > 0)
            //{
            //    //有数据

            //    string strWeekColumnName = TXTWeekColumnName(strMonth, strWeek);
            //    strSQL2.AppendLine(" select * ");
            //    strSQL2.AppendLine(" select vcFlag,vcMonth,vcOrderNo,vcWeek,vcPlant,vcGC,vcZB,vcPartsno,vcMonTotal," + strWeekColumnName + ",");
            //    strSQL2.AppendLine("vcWeekTotal,vcWeekOrderingCount,vcWeekLevelPercentage,vc1stWeekTotal,vc2ndWeekTotal,vc3rdWeekTotal,vc4thWeekTotal,vc5thWeekTotal,vcTotal ");
            //    strSQL2.AppendLine("from WeekLevelPercentage where vcMonth='" + strMonth + "' and vcWeek='" + strWeek + "' ");
            //    dtResult = excute.ExcuteSqlWithSelectToDT(strSQL2.ToString());
            //    TXTChangeColumnName(dtResult);//修改列名
            //    return dtResult;
            //}
            //else
            //{
            //    //没有数据，直接返回空表

            //    TXTChangeColumnName(dt);//修改列名
            //    return dt;
            //}
        }
        #endregion

        #region 修改带波动判定列的表的列名（没用上） - 李兴旺

        /// <summary>
        /// 修改带波动判定列的表的列名

        /// </summary>
        /// <param name="dtSource">数据源</param>
        public void TXTChangeColumnName(DataTable dtSource)
        {
            dtSource.Columns[0].ColumnName = "波动判定";
            dtSource.Columns[1].ColumnName = "对象月";
            dtSource.Columns[2].ColumnName = "订单号";
            dtSource.Columns[3].ColumnName = "对象周";
            dtSource.Columns[4].ColumnName = "工厂";
            dtSource.Columns[5].ColumnName = "工程";
            dtSource.Columns[6].ColumnName = "组别";
            dtSource.Columns[7].ColumnName = "品番";
            dtSource.Columns[8].ColumnName = "月度内示总量";
            //比原数据库表多了第一列波动判定列，所以从第9列开始

            for (int i = 9; i < dtSource.Columns.Count - 9; i++)
            {
                string strOldName = dtSource.Columns[i].ColumnName;
                string strNewName1 = strOldName.Substring(1, strOldName.Length - 2);//获取数字
                string strNewName2 = strOldName.Substring(strOldName.Length - 1) == "b" ? "日白" : "日夜";
                dtSource.Columns[i].ColumnName = strNewName1 + strNewName2;
            }
            dtSource.Columns[dtSource.Columns.Count - 9].ColumnName = "周内示总量";
            dtSource.Columns[dtSource.Columns.Count - 8].ColumnName = "周订货数量";
            dtSource.Columns[dtSource.Columns.Count - 7].ColumnName = "变动幅度";
            dtSource.Columns[dtSource.Columns.Count - 6].ColumnName = "1W";
            dtSource.Columns[dtSource.Columns.Count - 5].ColumnName = "2W";
            dtSource.Columns[dtSource.Columns.Count - 4].ColumnName = "3W";
            dtSource.Columns[dtSource.Columns.Count - 3].ColumnName = "4W";
            dtSource.Columns[dtSource.Columns.Count - 2].ColumnName = "5W";
            dtSource.Columns[dtSource.Columns.Count - 1].ColumnName = "合计";
        }
        #endregion

        #region 将中文列名的周度订单平准化管理表（Excel导入所致）转换为同数据库一致的英文列名 - 李兴旺

        /// <summary>
        /// 将中文列名的周度订单平准化管理表（Excel导入所致）转换为同数据库一致的英文列名
        /// </summary>
        /// <param name="dtSource">数据源</param>
        /// <returns>提示信息</returns>
        public string TXTChangeColumnNameSchedule(DataTable dtSource)
        {
            string _msg = string.Empty;
            DataTable dt = TXTCloneWeekLevelSchedule();
            if (dtSource.Columns.Count != dt.Columns.Count)
            {
                _msg = "导入数据表的列数与数据库列数不一致！";
            }
            else
            {
                //如果数据源列名不是数据库同列名，则修改，否则不做任何操作
                if (dtSource.Columns[0].ColumnName != "vcMonth")
                {
                    //主键部分
                    dtSource.Columns[0].ColumnName = "vcMonth";
                    dtSource.Columns[1].ColumnName = "vcWeek";
                    dtSource.Columns[2].ColumnName = "vcPlant";
                    dtSource.Columns[3].ColumnName = "vcGC";
                    dtSource.Columns[4].ColumnName = "vcZB";
                    dtSource.Columns[5].ColumnName = "vcPartsno";
                    dtSource.Columns[6].ColumnName = "vcQuantityPerContainer";
                    //内示部分
                    for (int i = 7; i < dtSource.Columns.Count - 64; i++)
                    {
                        string strOldName = dtSource.Columns[i].ColumnName;
                        string strNewName1 = strOldName.Substring(0, strOldName.Length - 2);//获取数字
                        string strNewName2 = strOldName.Substring(strOldName.Length - 2) == "日白" ? "b" : "y";
                        dtSource.Columns[i].ColumnName = "vcD" + strNewName1 + strNewName2;
                    }
                    dtSource.Columns[69].ColumnName = "vcWeekTotal";
                    //平准化部分，由于Excel中平准化部分的列名与内示部分相同，在导入后，平准化部分自动在原列名后加了一个1
                    for (int i = 70; i < dtSource.Columns.Count - 1; i++)
                    {
                        string strOldName = dtSource.Columns[i].ColumnName;
                        string strNewName1 = strOldName.Substring(0, strOldName.Length - 3);//获取数字
                        string strNewName2 = strOldName.Substring(strOldName.Length - 3) == "日白1" ? "b" : "y";
                        dtSource.Columns[i].ColumnName = "vcLevelD" + strNewName1 + strNewName2;
                    }
                    dtSource.Columns[dtSource.Columns.Count - 1].ColumnName = "vcLevelWeekTotal";
                }
            }
            return _msg;
        }
        #endregion

        #region 用厂区、对象月、对象周、品番、部署、组别获取该品番在月度包装计划里该对象周中有稼动的天数和列名 - 李兴旺

        /// <summary>
        /// 用厂区、对象月、对象周、品番、部署、组别获取该品番在月度包装计划里该对象周中有稼动的天数和列名
        /// </summary>
        /// <param name="strPlant">厂区</param>
        /// <param name="strMonth">对象月</param>
        /// <param name="strWeek">对象周</param>
        /// <param name="strPartsNo">品番</param>
        /// <param name="strGC">部署</param>
        /// <param name="strZB">组别</param>
        /// <param name="strWorkDaysColumnName">列名</param>
        /// <returns>稼动日天数</returns>
        public int TXTWorkDaysCount(string strPlant, string strMonth, string strWeek, string strPartsNo, string strGC, string strZB, ref string[] strWorkDaysColumnName)
        {
            int iDayNum = 0;//该对象周稼动日天数

            string vcCalendar1 = TXTFindProject1(strGC, strZB);//获取该品番工程1
            int iWeekDays = 0;//该对象周天数
            string strMonthPackPlanWeekColumnName = TXTWeekColumnName(strPlant, strMonth, vcCalendar1, strWeek, ref iWeekDays).Replace("D", "vcD");//根据对象月和工程1，查询对象周对应的列名和天数
            string strSQLMonthPackPlanWeek = "select " + strMonthPackPlanWeekColumnName + " from MonthPackPlanTbl where vcMonth='" + strMonth + "' and vcPartsno='" + strPartsNo + "' and vcProject1='" + vcCalendar1 + "' and (montouch='' or montouch is null) ";
            DataTable dt1 = excute.ExcuteSqlWithSelectToDT(strSQLMonthPackPlanWeek);//确定的1行数据

            if (dt1.Rows.Count == 1)
            {
                string[] TMP = new string[iWeekDays];
                for (int i = 0; i < dt1.Columns.Count; i++)
                {
                    if (dt1.Rows[0][i].ToString() != string.Empty)//先找不为空的
                    {
                        if (dt1.Rows[0][i].ToString() != "0")//再找不为0的

                        {
                            iDayNum++;
                            TMP[i] = dt1.Columns[i].ColumnName;
                        }
                    }
                }
                if (iDayNum > 0)//大于0，则表示有稼动日
                {
                    int Index = 0;
                    strWorkDaysColumnName = new string[iDayNum];
                    for (int n = 0; n < iWeekDays; n++)
                    {
                        if (TMP[n] != null)
                        {
                            strWorkDaysColumnName[Index] = TMP[n];
                            Index++;
                        }
                    }
                }
                return iDayNum;
            }
            else if (dt1.Rows.Count > 1)
            {
                return -2;//大于1，该品番在月度包装计划里不止一个结果

            }
            else
            {
                return 0;//小于1，该品番在月度包装计划里不存在，直接是0
            }
        }
        #endregion

        #region 用周计划变动幅度管理表确定的相关数据进行平准化并更新到周度订单平准化管理表中 - 李兴旺

        /// <summary>
        /// 用周计划变动幅度管理表确定的相关数据进行平准化并更新到周度订单平准化管理表中
        /// </summary>
        /// <param name="dtUpdate">周计划变动幅度管理表确定的相关数据</param>
        /// <returns>返回信息</returns>
        public string TXTInsertToWeekLevelSchedule(DataTable dtUpdate)
        {
            string msg = string.Empty;
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine("select * from WeekLevelSchedule where 1=2");//这样一定不会搜到数据，一定是行数为0的表
            DataTable dtResult = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());//获取WeekLevelSchedule表结构

            for (int i = 0; i < dtUpdate.Rows.Count; i++)
            {
                #region 按照周计划变动幅度管理表行数的循环体，每一行进行平准化操作并填入dtResult
                DataRow dr = dtResult.NewRow();
                //主键部分
                string strMonth = dtUpdate.Rows[i]["vcMonth"].ToString();
                string strWeek = dtUpdate.Rows[i]["vcWeek"].ToString();
                string strGC = dtUpdate.Rows[i]["vcGC"].ToString();
                string strZB = dtUpdate.Rows[i]["vcZB"].ToString();
                string strPlant = dtUpdate.Rows[i]["vcPlant"].ToString();
                string vcCalendar1 = TXTFindProject1(strGC, strZB);//获取该品番工程1
                int iWeekDays = 0;//该对象周天数
                string strUpdateColumnName = TXTWeekColumnName(strPlant, strMonth, vcCalendar1, strWeek, ref iWeekDays);//dtUpdate表中对象周的列名
                string strWeekColumnName = TXTWeekColumnName(strPlant, strMonth, vcCalendar1, strWeek, ref iWeekDays).Replace("D", "vcD");//根据对象月和工程1，查询内示计划部分对象周对应的列名和天数
                string strLevelWeekColumnName = TXTWeekColumnName(strPlant, strMonth, vcCalendar1, strWeek, ref iWeekDays).Replace("D", "vcLevelD");//对应平准化部分的对象周列名

                dr["vcMonth"] = dtUpdate.Rows[i]["vcMonth"].ToString();
                dr["vcWeek"] = dtUpdate.Rows[i]["vcWeek"].ToString();
                dr["vcPlant"] = dtUpdate.Rows[i]["vcPlant"].ToString();
                dr["vcGC"] = dtUpdate.Rows[i]["vcGC"].ToString();
                dr["vcZB"] = dtUpdate.Rows[i]["vcZB"].ToString();
                dr["vcPartsno"] = dtUpdate.Rows[i]["vcPartsno"].ToString();
                //获取品番看板收容数

                int iQuantity = TXTQuantity(dtUpdate.Rows[i]["vcPartsno"].ToString(), dtUpdate.Rows[i]["vcMonth"].ToString());
                dr["vcQuantityPerContainer"] = iQuantity.ToString();
                //获取内示计划，更新WeekLevelSchedule内示部分对应周的内容
                for (int j = 0; j < iWeekDays; j++)
                {
                    //dr[j] = dtUpdate.Rows[i][j + 2].ToString();
                    dr[strWeekColumnName.Split(',')[j]] = dtUpdate.Rows[i][strUpdateColumnName.Split(',')[j]].ToString();
                }
                dr["vcWeekTotal"] = dtUpdate.Rows[i]["vcWeekTotal"].ToString();
                //平准化部分

                string[] strWorkDays = null;//该品番有稼动日（不为0）的列名（vcD）

                //该品番在本对象周的稼动日天数
                int iWorkDays = TXTWorkDaysCount(dtUpdate.Rows[i]["vcPlant"].ToString(), dtUpdate.Rows[i]["vcMonth"].ToString(), dtUpdate.Rows[i]["vcWeek"].ToString(), dtUpdate.Rows[i]["vcPartsno"].ToString(), dtUpdate.Rows[i]["vcGC"].ToString(), dtUpdate.Rows[i]["vcZB"].ToString(), ref strWorkDays);
                if (iWorkDays == -2)
                {
                    msg = "品番：" + dtUpdate.Rows[i]["vcPartsno"].ToString() + "在该月度包装计划里不止一个结果";
                    break;
                }
                //else if (iWorkDays == -1)
                //{
                //    msg = "品番：" + dtUpdate.Rows[i]["vcPartsno"].ToString() + "在该月度包装计划里不存在";
                //    break;
                //}
                else if (iWorkDays == 0)
                {
                    //稼动日天数为0，表示该周没计划，则把该周的订单数放在该周第一值内
                    string WeekTotal = dtUpdate.Rows[i]["vcWeekTotal"].ToString();//本行纯验证用
                    int iWeekOrderingCount = Convert.ToInt32(dtUpdate.Rows[i]["vcWeekOrderingCount"].ToString());//最终确认的周订货总数
                    for (int c = 0; c < iWeekDays; c++)
                    {
                        dr[strLevelWeekColumnName.Split(',')[c]] = "0";//先全部置0
                    }
                    dr[strLevelWeekColumnName.Split(',')[0]] = iWeekOrderingCount.ToString();//把该周的订单数放在该周第一值内
                    dr["vcLevelWeekTotal"] = iWeekOrderingCount.ToString();//最终确认的周订货总数
                }
                else
                {
                    //正常情况
                    int iWeekOrderingCount = Convert.ToInt32(dtUpdate.Rows[i]["vcWeekOrderingCount"].ToString());//最终确认的周订货总数
                                                                                                                 //可以不用判定最终的周订货总数是否和看板收容数整除，这个在数据导入时就判定了。

                    int iPackage = iWeekOrderingCount / iQuantity;//每周要包的份数

                    int m = iPackage / iWorkDays;//份数下限 利用整型相除的商只有整数部分，不会四舍五入

                    int n = m + 1;//份数上限
                    int x = n * iWorkDays - iPackage;//下限的稼动日数

                    int y = iWorkDays - x;//上限的稼动日数

                    string[] strResult = new string[iWorkDays];//存放平准化结果

                    for (int a = 0; a < y; a++)
                    {
                        //先分配上限的稼动日数
                        strResult[a] = (n * iQuantity).ToString();
                    }
                    for (int b = y; b < iWorkDays; b++)
                    {
                        //再分配下限的稼动日数
                        strResult[b] = (m * iQuantity).ToString();
                    }
                    for (int c = 0; c < iWeekDays; c++)
                    {
                        dr[strLevelWeekColumnName.Split(',')[c]] = "0";//先全部置0
                    }
                    for (int Index = 0; Index < iWorkDays; Index++)
                    {
                        dr[strWorkDays[Index].Replace("vcD", "vcLevelD")] = strResult[Index];//再把不是0的部分更新

                    }
                    dr["vcLevelWeekTotal"] = iWeekOrderingCount.ToString();//最终确认的周订货总数
                }
                dtResult.Rows.Add(dr);
                #endregion
            }
            //消息为空时，更新数据
            if (msg.Length <= 0)
            {
                //dtResult更新到WeekLevelSchedule表中
                dtResult.PrimaryKey = new DataColumn[]
                {
                    dtResult.Columns["vcMonth"],
                    dtResult.Columns["vcWeek"],
                    dtResult.Columns["vcPlant"],
                    dtResult.Columns["vcGC"],
                    dtResult.Columns["vcZB"],
                    dtResult.Columns["vcPartsno"]
                };
                string strMonth2 = dtResult.Rows[0]["vcMonth"].ToString();
                string strWeek2 = dtResult.Rows[0]["vcWeek"].ToString();
                string strPlant2 = dtResult.Rows[0]["vcPlant"].ToString();
                StringBuilder strSQL2 = new StringBuilder();
                //删周度订单平准化管理表

                strSQL2.AppendLine(" delete from WeekLevelSchedule where vcMonth='" + strMonth2 + "' and vcWeek='" + strWeek2 + "' and vcPlant='" + strPlant2 + "'; ");
                //删计划（暂无）

                strSQL2.AppendLine("");
                string strDeleteSQL = strSQL2.ToString();
                string strSQLSearch = " select * from WeekLevelSchedule ";
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = new SqlConnection(ComConnectionHelper.GetConnectionString());
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = strDeleteSQL;
                cmd.Connection.Open();
                cmd.Transaction = cmd.Connection.BeginTransaction();
                try
                {
                    //先执行删除

                    cmd.ExecuteNonQuery();
                    //再改为检索

                    cmd.CommandText = strSQLSearch;
                    SqlDataAdapter apt = new SqlDataAdapter(cmd);
                    DataTable dtupdate = new DataTable();
                    apt.Fill(dtupdate);
                    for (int k = 0; k < dtResult.Rows.Count; k++)
                    {
                        DataRow dr = dtupdate.NewRow();
                        for (int i = 0; i < dtResult.Columns.Count; i++)
                        {
                            dr[i] = dtResult.Rows[k][i];
                        }
                        dtupdate.Rows.Add(dr);
                    }
                    SqlCommandBuilder sb = new SqlCommandBuilder(apt);
                    apt.Update(dtupdate);
                    apt.Dispose();
                    sb.Dispose();
                    cmd.Transaction.Commit();
                    cmd.Connection.Close();
                }
                catch (Exception ex)
                {
                    cmd.Transaction.Rollback();
                    cmd.Connection.Close();
                    throw ex;
                }
            }
            return msg;
        }
        #endregion

        #region 用对象月、对象周、工厂检索周度订单平准化管理表WeekLevelSchedule - 李兴旺

        /// <summary>
        /// 用对象月、对象周、工厂检索周度订单平准化管理表WeekLevelSchedule
        /// </summary>
        /// <param name="strMonth">对象月</param>
        /// <param name="strWeek">对象周</param>
        /// <param name="strPlant">工厂</param>
        /// <returns>检索结果数据表</returns>
        public DataTable TXTSearchWeekLevelSchedule(string strMonth, string strWeek, string strPlant)
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL1 = new StringBuilder();
            strSQL1.AppendLine(" select * from WeekLevelSchedule where vcMonth='" + strMonth + "' and vcWeek='" + strWeek + "' and vcPlant='" + strPlant + "' ");
            dt = excute.ExcuteSqlWithSelectToDT(strSQL1.ToString());
            return dt;
        }
        #endregion

        #region 检查导入或更新的平准化数据正确性 - 李兴旺

        /// <summary>
        /// 检查导入或更新的平准化数据正确性

        /// </summary>
        /// <param name="dt">数据源</param>
        /// <returns>提示信息</returns>
        public string TXTCheckDataSchedule(DataTable dt)
        {
            string _msg = string.Empty;
            _msg = TXTChangeColumnNameSchedule(dt);//先把列名改成与数据库一致的
            if (_msg.Length > 0)
            {
                return _msg;
            }
            else
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    #region 循环体 检测数据

                    //数据准备
                    int _iSum = 0;//平准化部分各列数字求和

                    int _iSumWeek = 0;//对象周各列数字求和

                    string _strMonth = dt.Rows[i]["vcMonth"].ToString().Trim();
                    string _strWeek = dt.Rows[i]["vcWeek"].ToString().Trim();
                    string _strPlant = dt.Rows[i]["vcPlant"].ToString().Trim();
                    string _strGC = dt.Rows[i]["vcGC"].ToString().Trim();
                    string _strZB = dt.Rows[i]["vcZB"].ToString().Trim();
                    string _strPartsNo = dt.Rows[i]["vcPartsno"].ToString().Trim();
                    string _vcCalendar1 = TXTFindProject1(_strGC, _strZB);//获取该品番工程1
                    int _iWeekOrderingCount = TXTFindOrderingCount(_strMonth, _strWeek, _strPlant, _strGC, _strZB, _strPartsNo);
                    //订单订货数数据校验

                    if (_iWeekOrderingCount == -1)
                    {
                        //数据不唯一
                        _msg = "数据中除标题外第" + (i + 1).ToString() + "行" + NumberToText(_strWeek) + "的品番：" + _strPartsNo + "TXT订单数据不唯一！";
                        break;
                    }
                    else if (_iWeekOrderingCount == 0)
                    {
                        //数据不唯一
                        _msg = "数据中除标题外第" + (i + 1).ToString() + "行" + NumberToText(_strWeek) + "的品番：" + _strPartsNo + "TXT订单数据不存在！";
                        break;
                    }
                    int _iWeekDays = 0;//该对象周天数
                    string _strLevelWeekColumnName = TXTWeekColumnName(_strPlant, _strMonth, _vcCalendar1, _strWeek, ref _iWeekDays).Replace("D", "vcLevelD");//对应平准化部分的对象周列名

                    string[] _ColumnName = _strLevelWeekColumnName.Split(',');//对象周对应的平准化部分的列名清单
                    int _vcLevelWeekTotal = 0;
                    //开始检测部分

                    //【导入Excel文件】【页面修改】平准化部分的列数据是否都是数字（空字符串是允许的）
                    for (int j = 70; j < dt.Columns.Count - 1; j++)
                    {
                        if (TXTIsInt(dt.Rows[i][j].ToString().Trim()) == false)
                        {
                            string strOldName = dt.Columns[j].ColumnName;
                            string strNewName1 = strOldName.Substring(8, strOldName.Length - 9);//获取数字
                            string strNewName2 = strOldName.Substring(strOldName.Length - 1) == "b" ? "日白" : "日夜";
                            string _NewName = strNewName1 + strNewName2;
                            //对象周对应列的数据有的不是数字

                            _msg = "数据中除标题外第" + (i + 1).ToString() + "行" + NumberToText(_strWeek) + "的" + _NewName + "列的数据不是数字！";
                            break;
                        }
                    }
                    if (_msg.Length > 0)
                    {
                        //每通过二重循环检测一项内容，就检测一遍msg，如果msg有内容，则跳出最外层循环
                        break;
                    }
                    //【导入Excel文件】周总量(平准)列数据校验和获取
                    if (dt.Rows[i]["vcLevelWeekTotal"].ToString().Trim() == string.Empty)
                    {
                        //对象周周总量(平准)列数据为空

                        _msg = "数据中除标题外第" + (i + 1).ToString() + "行" + NumberToText(_strWeek) + "的周总量(平准)列的数据为空！";
                        break;
                    }
                    else
                    {
                        if (TXTIsInt(dt.Rows[i]["vcLevelWeekTotal"].ToString().Trim()) == false)
                        {
                            //对象周周总量(平准)列数据不是数字

                            _msg = "数据中除标题外第" + (i + 1).ToString() + "行" + NumberToText(_strWeek) + "的周总量(平准)列的数据不是数字！";
                            break;
                        }
                        else
                        {
                            _vcLevelWeekTotal = Convert.ToInt32(dt.Rows[i]["vcLevelWeekTotal"].ToString().Trim());//获取本周订货总量
                        }
                    }
                    //【导入Excel文件】本周订货数量与TXT订单中的数量是否一致

                    if (_iWeekOrderingCount != _vcLevelWeekTotal)
                    {
                        //导入Excel文件中的本周订货数量与TXT订单中的数量不一致

                        _msg = "数据中除标题外第" + (i + 1).ToString() + "行" + NumberToText(_strWeek) + "的订货总数与TXT订单的订货总数不一致！";
                        break;
                    }
                    //【导入Excel文件】【页面修改】针对对应周的数据进行空校验和求和

                    for (int k = 0; k < _iWeekDays; k++)
                    {
                        if (dt.Rows[i][_ColumnName[k]].ToString().Trim() == string.Empty)
                        {
                            string strOldName = _ColumnName[k];
                            string strNewName1 = strOldName.Substring(8, strOldName.Length - 9);//获取数字
                            string strNewName2 = strOldName.Substring(strOldName.Length - 1) == "b" ? "日白" : "日夜";
                            string _NewName = strNewName1 + strNewName2;
                            //对象周对应列的数据不能为空

                            _msg = "数据中除标题外第" + (i + 1).ToString() + "行" + NumberToText(_strWeek) + "的" + _NewName + "列的数据不能为空！";
                            break;
                        }
                        else
                        {
                            //对象周对应列的所有数字之和

                            _iSumWeek += Convert.ToInt32(dt.Rows[i][_ColumnName[k]].ToString().Trim());
                        }
                    }
                    if (_msg.Length > 0)
                    {
                        //每通过二重循环检测一项内容，就检测一遍msg，如果msg有内容，则跳出最外层循环
                        break;
                    }
                    //【导入Excel文件】平准化部分，一行的所有数据之和，对象周以外的列是否有数字校验
                    for (int j = 70; j < dt.Columns.Count - 1; j++)
                    {
                        if (dt.Rows[i][j].ToString().Trim() != string.Empty)
                        {
                            _iSum += Convert.ToInt32(dt.Rows[i][j].ToString().Trim());
                        }
                    }
                    if (_iSum != _iSumWeek)
                    {
                        //两个和不相等，说明对象周以外的列还有数字，不允许
                        _msg = "数据中除标题外第" + (i + 1).ToString() + "行含有" + NumberToText(_strWeek) + "以外列的数据！";
                        break;
                    }
                    //【导入Excel文件】【页面修改】与收容数整除校验

                    int _iQuantity = Convert.ToInt32(dt.Rows[i]["vcQuantityPerContainer"].ToString());//获取收容数

                    for (int k = 0; k < _iWeekDays; k++)
                    {
                        if (Convert.ToInt32(dt.Rows[i][_ColumnName[k]].ToString()) % _iQuantity != 0)
                        {
                            string strOldName = _ColumnName[k];
                            string strNewName1 = strOldName.Substring(8, strOldName.Length - 9);//获取数字
                            string strNewName2 = strOldName.Substring(strOldName.Length - 1) == "b" ? "日白" : "日夜";
                            string _NewName = strNewName1 + strNewName2;
                            //对象周对应列有数字不能被收容数整除

                            _msg = "数据中除标题外第" + (i + 1).ToString() + "行" + NumberToText(_strWeek) + "的" + _NewName + "列的数字不能被收容数整除！";
                            break;
                        }
                    }
                    if (_msg.Length > 0)
                    {
                        //每通过二重循环检测一项内容，就检测一遍msg，如果msg有内容，则跳出最外层循环
                        break;
                    }
                    //【导入Excel文件】【页面修改】对象周所有数字之和与本周订货总量校验
                    if (_iSumWeek != _vcLevelWeekTotal)
                    {
                        //对象周对应列的所有数字之和与本周订货总量不相等

                        _msg = "数据中除标题外第" + (i + 1).ToString() + "行" + NumberToText(_strWeek) + "所有数字之和与本周订货总量不相等！";
                        break;
                    }
                    #endregion
                }
                return _msg;
            }
        }
        #endregion

        #region 校验导入的Excel数据 - 李兴旺

        /// <summary>
        /// 校验导入的Excel数据
        /// </summary>
        /// <param name="excelpath">Excel导入路径</param>
        /// <param name="dtre">最终要更新数据库的数据源，即数据结果</param>
        /// <param name="dtTmplate">模板里导出的数据源</param>
        /// <returns>提示信息</returns>
        public string TXTCheckExcel(string excelpath, ref DataTable dtre, DataTable dtTmplate)
        {
            string msg = string.Empty;
            //QMCommon.OfficeCommon.QMExcel oQMExcel = new QMCommon.OfficeCommon.QMExcel();
            //DataTable dt = oQMExcel.GetExcelContentByOleDb(excelpath);//导入文件
            //msg = checkExcelHeadpos(dt, dtTmplate);//校验模板
            //if (msg.Length > 0) return msg;
            //msg = TXTCheckDataSchedule(dt);//校验数据
            //dtre = dt;
            return msg;
        }
        #endregion

        #region 将数据更新到周度订单平准化管理表WeekLevelSchedule - 李兴旺

        /// <summary>
        /// 将数据更新到周度订单平准化管理表WeekLevelSchedule
        /// </summary>
        /// <param name="dtSource">数据源</param>
        /// <param name="strMonth">对象月</param>
        /// <param name="strWeek">对象周</param>
        /// <param name="strPlant">工厂</param>
        public void TXTUpdateTableSchedule(DataTable dtSource, string strMonth, string strWeek, string strPlant)
        {
            string strSQLSearch = " select * from WeekLevelSchedule ";
            StringBuilder strSQL = new StringBuilder();
            //删周度订单平准化管理表

            strSQL.AppendLine(" delete from WeekLevelSchedule where vcMonth='" + strMonth + "' and vcWeek='" + strWeek + "' and vcPlant='" + strPlant + "'; ");
            //删计划（暂无）

            strSQL.AppendLine("");
            string strDeleteSQL = strSQL.ToString();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(ComConnectionHelper.GetConnectionString());
            cmd.CommandTimeout = 0;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = strDeleteSQL;
            cmd.Connection.Open();
            cmd.Transaction = cmd.Connection.BeginTransaction();
            try
            {
                //先执行删除

                cmd.ExecuteNonQuery();
                //再改为检索

                cmd.CommandText = strSQLSearch;
                SqlDataAdapter apt = new SqlDataAdapter(cmd);
                DataTable dtupdate = new DataTable();
                apt.Fill(dtupdate);
                for (int k = 0; k < dtSource.Rows.Count; k++)
                {
                    DataRow dr = dtupdate.NewRow();
                    for (int i = 0; i < dtSource.Columns.Count; i++)
                    {
                        dr[i] = dtSource.Rows[k][i];
                    }
                    dtupdate.Rows.Add(dr);
                }
                SqlCommandBuilder sb = new SqlCommandBuilder(apt);
                apt.Update(dtupdate);
                apt.Dispose();
                sb.Dispose();
                cmd.Transaction.Commit();
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                cmd.Transaction.Rollback();
                cmd.Connection.Close();
                throw ex;
            }
        }
        #endregion

        #region 将周度订单平准化管理表平准化部分数据源更新到各个计划中 - 李兴旺

        /// <summary>
        /// 将周度订单平准化管理表平准化部分数据源更新到各个计划中

        /// </summary>
        /// <param name="dt">周度订单平准化管理表</param>
        /// <param name="strMonth">对象月</param>
        /// <param name="strWeek">对象周</param>
        /// <param name="strPlant">厂区</param>
        /// <param name="strUser">登录用户</param>
        /// <returns>提示信息</returns>
        public string TXTScheduleToPlan(DataTable dt, string strMonth, string strWeek, string strPlant, string strUser)
        {
            string _msg = string.Empty;
            DataTable dtWeekPackPlan = TXTCloneWeekPlanTbl();//周度包装计划表

            DataTable dtWeekKanBanPlan = TXTCloneWeekPlanTbl();//周度看板打印计划表

            DataTable dtWeekProdPlan = TXTCloneWeekPlanTbl();//周度生产计划表

            DataTable dtWeekTZPlan = TXTCloneWeekPlanTbl();//周度涂装计划表

            DataTable dtWeekP3Plan = TXTCloneWeekPlanTbl();//周度P3计划表

            //先在数据库的周度包装计划管理表中找相应对象月、厂区的数据（有没有都要找）
            //string strSQL4 = "select * from WeekPackPlanTbl where vcMonth='" + strMonth + "' and (montouch='' or montouch is null) and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + strPlant + "' and vcPartsNo = WeekPackPlanTbl.vcPartsno and dTimeFrom <= '" + strMonth + "-01' and dTimeTo >= '" + strMonth + "-01'); ";
            string strSQL4 = "select * from WeekPackPlanTbl where vcMonth='" + strMonth + "' and (vcMonTotal <> '' or vcMonTotal is not null) ";
            strSQL4 += "and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + strPlant + "' and vcPartsNo = WeekPackPlanTbl.vcPartsno ";
            strSQL4 += "and dTimeFrom <= '" + strMonth + "-01' and dTimeTo >= '" + strMonth + "-01') ";
            strSQL4 += "union all ";
            strSQL4 += "select * from WeekPackPlanTbl where montouch = '" + strMonth + "' ";
            strSQL4 += "and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + strPlant + "' and vcPartsNo = WeekPackPlanTbl.vcPartsno ";
            strSQL4 += "and dTimeFrom <= '" + strMonth + "-01' and dTimeTo >= '" + strMonth + "-01') ";
            DataTable dtTMP4 = excute.ExcuteSqlWithSelectToDT(strSQL4);
            //先根据平准化结果更新周度包装计划数据表格
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                #region 生成周度包装计划循环体

                //数据准备
                string _strGC = dt.Rows[i]["vcGC"].ToString().Trim();
                string _strZB = dt.Rows[i]["vcZB"].ToString().Trim();
                string _strPartsNo = dt.Rows[i]["vcPartsno"].ToString().Trim();
                string _strPlant = dt.Rows[i]["vcPlant"].ToString().Trim();
                string _vcCalendar1 = TXTFindProject1(_strGC, _strZB);//获取该品番工程1
                string _vcProName1 = TXTFindProName1(_strGC, _strZB);//获取该品番工程1名称
                int _iWeekDays = 0;//对象周天数

                string _strPlanColumnName = TXTWeekColumnName(_strPlant, strMonth, _vcCalendar1, strWeek, ref _iWeekDays).Replace("D", "vcD");//计划管理表对象周列名
                string[] _PlanColumnName = _strPlanColumnName.Split(',');//计划管理表对象周列名清单
                string _strLevelWeekColumnName = TXTWeekColumnName(_strPlant, strMonth, _vcCalendar1, strWeek, ref _iWeekDays).Replace("D", "vcLevelD");//对应平准化部分的对象周列名

                string[] _ColumnName = _strLevelWeekColumnName.Split(',');//对象周对应的平准化部分的列名清单
                string strSQLMonthPackPlan = "select * from MonthPackPlanTbl where vcMonth='" + strMonth + "' and vcPartsno='" + _strPartsNo + "' and vcProject1='" + _vcCalendar1 + "' and (vcMonTotal <> '' or vcMonTotal is not null) ";
                DataTable dtTMPMonthPackPlan = excute.ExcuteSqlWithSelectToDT(strSQLMonthPackPlan);
                //在月度包装计划中数据不存在的情况取消了

                //if (dtTMPMonthPackPlan.Rows.Count <= 0)
                //{
                //    _msg = "品番：" + _strPartsNo + "在月度包装计划中不存在！";
                //    break;
                //}
                if (dtTMPMonthPackPlan.Rows.Count > 1)
                {
                    _msg = "品番：" + _strPartsNo + "在月度包装计划中数据不唯一！";
                    break;
                }
                //周度包装计划数据不存在时，用生成表格方式生成包装计划。。

                if (dtTMP4.Rows.Count <= 0)
                {
                    //_msg = "品番：" + _strPartsNo + "在第一周还没有数据，请先维护第一周的数据！";
                    //break;
                    DataRow dr = dtWeekPackPlan.NewRow();//新建行

                    dr["vcMonth"] = strMonth;//对象月

                    dr["vcPartsno"] = _strPartsNo;//品番
                    dr["vcDock"] = TXTFindDock(_strPartsNo, strMonth);//受入：单独方法检索

                    dr["vcCarType"] = TXTFindCarType(_strPartsNo, strMonth);//车型：单独方法检索

                    dr["vcProject1"] = _vcCalendar1;//工程1
                    dr["vcProjectName"] = _vcProName1;//工程1名称：部署组别检索

                    dr["vcMonTotal"] = dt.Rows[i]["vcLevelWeekTotal"];//总数为第一周订单总数
                    dr["montouch"] = DBNull.Value;//空值

                    dr["DADDTIME"] = DateTime.Now;
                    dr["DUPDTIME"] = DateTime.Now;
                    dr["CUPDUSER"] = strUser;
                    for (int k = 0; k < _iWeekDays; k++)
                    {
                        dr[_PlanColumnName[k]] = dt.Rows[i][_ColumnName[k]];//将周度订单平准化管理表中平准化部分的数据复制到周度包装计划表中

                    }
                    dtWeekPackPlan.Rows.Add(dr);
                }
                else
                {
                    string strSelect4 = "vcMonth='" + strMonth + "' and vcPartsno='" + _strPartsNo + "' and vcProject1='" + _vcCalendar1 + "' and (vcMonTotal is not null)";
                    DataRow[] dtRow4 = dtTMP4.Select(strSelect4);//筛选出要更新的那一行

                    if (dtRow4.Length > 1)
                    {
                        _msg = "品番：" + _strPartsNo + "在周度包装计划管理表中数据不唯一！";
                        break;
                    }
                    else if (dtRow4.Length == 1)
                    {
                        //开始给这一行更新数据

                        //int _Total1 = Convert.ToInt32(dtRow4[0]["vcMonTotal"].ToString());//周度包装计划表里的总数
                        //int _Total2 = Convert.ToInt32(dt.Rows[i]["vcLevelWeekTotal"].ToString());//周度订单平准化管理表中平准化部分的订单总数
                        //dtRow4[0]["vcMonTotal"] = _Total1 + _Total2;//总数为累加的和

                        dtRow4[0]["DUPDTIME"] = DateTime.Now;//添加时间不用改，只改更新时间
                        dtRow4[0]["CUPDUSER"] = strUser;
                        for (int k = 0; k < _iWeekDays; k++)
                        {
                            dtRow4[0][_PlanColumnName[k]] = dt.Rows[i][_ColumnName[k]];//将周度订单平准化管理表中平准化部分的数据复制到周度包装计划表中

                        }
                        //更新完主体数据之后再求和
                        dtRow4[0]["vcMonTotal"] = TXTWeekPackPlanMonTotal(dtRow4[0]);
                        dtWeekPackPlan.Rows.Add(dtRow4[0].ItemArray);//把更新好的数据行放到结果表中
                    }
                    else if (dtRow4.Length == 0)//行为0，即本周有此品番订单，而上周没有，需要新建一行

                    {
                        DataRow dr = dtWeekPackPlan.NewRow();//新建行

                        dr["vcMonth"] = strMonth;//对象月

                        dr["vcPartsno"] = _strPartsNo;//品番
                        dr["vcDock"] = TXTFindDock(_strPartsNo, strMonth);//受入：单独方法检索

                        dr["vcCarType"] = TXTFindCarType(_strPartsNo, strMonth);//车型：单独方法检索

                        dr["vcProject1"] = _vcCalendar1;//工程1
                        dr["vcProjectName"] = _vcProName1;//工程1名称：部署组别检索

                        dr["vcMonTotal"] = dt.Rows[i]["vcLevelWeekTotal"];//总数为第一周订单总数
                        dr["montouch"] = DBNull.Value;//空值

                        dr["DADDTIME"] = DateTime.Now;
                        dr["DUPDTIME"] = DateTime.Now;
                        dr["CUPDUSER"] = strUser;
                        for (int k = 0; k < _iWeekDays; k++)
                        {
                            dr[_PlanColumnName[k]] = dt.Rows[i][_ColumnName[k]];//将周度订单平准化管理表中平准化部分的数据复制到周度包装计划表中

                        }
                        dtWeekPackPlan.Rows.Add(dr);
                    }
                }
                #endregion
            }
            //周度包装计划表格更新完毕，开始更新P3、涂装、生产、看板打印计划表
            //0：看板打印；1：生产；2：涂装；3：P3
            //看板打印计划
            string strSQL0 = "select * from WeekKanBanPlanTbl where vcMonth='" + strMonth + "' and (vcMonTotal <> '' or vcMonTotal is not null) ";
            strSQL0 += "and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + strPlant + "' and vcPartsNo = WeekKanBanPlanTbl.vcPartsno ";
            strSQL0 += "and dTimeFrom <= '" + strMonth + "-01' and dTimeTo >= '" + strMonth + "-01') ";
            strSQL0 += "union all ";
            strSQL0 += "select * from WeekKanBanPlanTbl where montouch = '" + strMonth + "' ";
            strSQL0 += "and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + strPlant + "' and vcPartsNo = WeekKanBanPlanTbl.vcPartsno ";
            strSQL0 += "and dTimeFrom <= '" + strMonth + "-01' and dTimeTo >= '" + strMonth + "-01') ";
            //生产计划
            string strSQL1 = "select * from WeekProdPlanTbl where vcMonth='" + strMonth + "' and (vcMonTotal <> '' or vcMonTotal is not null) ";
            strSQL1 += "and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + strPlant + "' and vcPartsNo = WeekProdPlanTbl.vcPartsno ";
            strSQL1 += "and dTimeFrom <= '" + strMonth + "-01' and dTimeTo >= '" + strMonth + "-01') ";
            strSQL1 += "union all ";
            strSQL1 += "select * from WeekProdPlanTbl where montouch = '" + strMonth + "' ";
            strSQL1 += "and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + strPlant + "' and vcPartsNo = WeekProdPlanTbl.vcPartsno ";
            strSQL1 += "and dTimeFrom <= '" + strMonth + "-01' and dTimeTo >= '" + strMonth + "-01') ";
            //涂装计划
            string strSQL2 = "select * from WeekTZPlanTbl where vcMonth='" + strMonth + "' and (vcMonTotal <> '' or vcMonTotal is not null) ";
            strSQL2 += "and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + strPlant + "' and vcPartsNo = WeekTZPlanTbl.vcPartsno ";
            strSQL2 += "and dTimeFrom <= '" + strMonth + "-01' and dTimeTo >= '" + strMonth + "-01') ";
            strSQL2 += "union all ";
            strSQL2 += "select * from WeekTZPlanTbl where montouch = '" + strMonth + "' ";
            strSQL2 += "and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + strPlant + "' and vcPartsNo = WeekTZPlanTbl.vcPartsno ";
            strSQL2 += "and dTimeFrom <= '" + strMonth + "-01' and dTimeTo >= '" + strMonth + "-01') ";
            //P3计划
            string strSQL3 = "select * from WeekP3PlanTbl where vcMonth='" + strMonth + "' and (vcMonTotal <> '' or vcMonTotal is not null) ";
            strSQL3 += "and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + strPlant + "' and vcPartsNo = WeekP3PlanTbl.vcPartsno ";
            strSQL3 += "and dTimeFrom <= '" + strMonth + "-01' and dTimeTo >= '" + strMonth + "-01') ";
            strSQL3 += "union all ";
            strSQL3 += "select * from WeekP3PlanTbl where montouch = '" + strMonth + "' ";
            strSQL3 += "and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + strPlant + "' and vcPartsNo = WeekP3PlanTbl.vcPartsno ";
            strSQL3 += "and dTimeFrom <= '" + strMonth + "-01' and dTimeTo >= '" + strMonth + "-01') ";
            DataTable dtTMP0 = excute.ExcuteSqlWithSelectToDT(strSQL0);//工程0：看板打印

            DataTable dtTMP1 = excute.ExcuteSqlWithSelectToDT(strSQL1);//工程1：生产

            DataTable dtTMP2 = excute.ExcuteSqlWithSelectToDT(strSQL2);//工程2：涂装

            DataTable dtTMP3 = excute.ExcuteSqlWithSelectToDT(strSQL3);//工程3：预留P3
            for (int Index = 0; Index < dtWeekPackPlan.Rows.Count; Index++)
            {
                #region 生成其他计划循环体

                //数据准备
                string _pPartsNo = dtWeekPackPlan.Rows[Index]["vcPartsno"].ToString();
                string _pGC = string.Empty;//部署
                string _pZB = string.Empty;//组别
                TXTFindGCAndZB(_pPartsNo, strMonth, ref _pGC, ref _pZB);
                string _pCalendar1 = TXTFindProject1(_pGC, _pZB);//获取该品番工程1                
                DataTable dtLT = TXTFindLT(_pGC, _pZB);//一定是唯一的一行数据

                if (dtLT.Rows.Count <= 0)
                {
                    _msg = "部署：" + _pGC + " 和组别：" + _pZB + " 不存在对应的生产条件，请及时维护！";
                    break;
                }
                string _pLT0 = dtLT.Rows[0]["vcLT0"].ToString();//工程0：看板打印

                string _pLT1 = dtLT.Rows[0]["vcLT1"].ToString();//工程1：生产

                string _pLT2 = dtLT.Rows[0]["vcLT2"].ToString();//工程2：涂装

                string _pLT3 = dtLT.Rows[0]["vcLT3"].ToString();//工程3：预留P3
                string _pLT4 = dtLT.Rows[0]["vcLT4"].ToString();//工程4：包装

                //对象周列名

                int _pWeekDays = 0;//对象周天数

                string _pPlanColumnName = TXTWeekColumnName(strPlant, strMonth, _pCalendar1, strWeek, ref _pWeekDays).Replace("D", "vcD");//计划管理表对象周列名
                string[] _PlanColumnName = _pPlanColumnName.Split(',');//计划管理表对象周列名清单
                                                                       //_pLT2和_pLT3需要空校验，为空时则不作任何操作

                //1生产计划
                int iProd = Convert.ToInt32(_pLT1);
                //数据不存在或对象周为第一周时，用生成表格方式生成其他计划。下同。

                if (dtTMP1.Rows.Count <= 0 || strWeek == "1")
                {
                    TXTMakePlanTableRow(iProd, dtWeekPackPlan.Rows[Index], ref dtWeekProdPlan, _pWeekDays, _PlanColumnName, strMonth, _pCalendar1, strPlant);
                }
                else
                {
                    string strSelect1 = "vcMonth='" + strMonth + "' and vcPartsno='" + _pPartsNo + "' and vcProject1='" + _pCalendar1 + "' and (vcMonTotal is not null)";
                    DataRow[] dtRow1 = dtTMP1.Select(strSelect1);//筛选出要更新的那一行

                    if (dtRow1.Length > 1)
                    {
                        _msg = "品番：" + _pPartsNo + "在周度生产计划管理表中数据不唯一！";
                        break;
                    }
                    else if (dtRow1.Length == 1)
                    {
                        //开始给这一行更新数据

                        //“总数为累加的和”可以直接从包装计划获取
                        TXTMakeRowInPlanTable(iProd, dtWeekPackPlan.Rows[Index], ref dtRow1[0], _pWeekDays, _PlanColumnName, strMonth, _pCalendar1, strPlant);
                        dtWeekProdPlan.Rows.Add(dtRow1[0].ItemArray);//把更新好的数据行放到结果表中
                    }
                    else if (dtRow1.Length == 0)//行为0，即本周有此品番订单，而上周没有，需要新建一行，下同
                    {
                        TXTMakePlanTableRow(iProd, dtWeekPackPlan.Rows[Index], ref dtWeekProdPlan, _pWeekDays, _PlanColumnName, strMonth, _pCalendar1, strPlant);
                    }
                }
                //2涂装计划
                if (_pLT2 != string.Empty)
                {
                    int iTZ = Convert.ToInt32(_pLT2);
                    if (dtTMP2.Rows.Count <= 0 || strWeek == "1")
                    {
                        TXTMakePlanTableRow(iTZ, dtWeekPackPlan.Rows[Index], ref dtWeekTZPlan, _pWeekDays, _PlanColumnName, strMonth, _pCalendar1, strPlant);
                    }
                    else
                    {
                        string strSelect2 = "vcMonth='" + strMonth + "' and vcPartsno='" + _pPartsNo + "' and vcProject1='" + _pCalendar1 + "' and (vcMonTotal is not null)";
                        DataRow[] dtRow2 = dtTMP2.Select(strSelect2);//筛选出要更新的那一行

                        if (dtRow2.Length > 1)
                        {
                            _msg = "品番：" + _pPartsNo + "在周度涂装计划管理表中数据不唯一！";
                            break;
                        }
                        else if (dtRow2.Length == 1)
                        {
                            //开始给这一行更新数据

                            //“总数为累加的和”可以直接从包装计划获取
                            TXTMakeRowInPlanTable(iTZ, dtWeekPackPlan.Rows[Index], ref dtRow2[0], _pWeekDays, _PlanColumnName, strMonth, _pCalendar1, strPlant);
                            dtWeekTZPlan.Rows.Add(dtRow2[0].ItemArray);//把更新好的数据行放到结果表中
                        }
                        else if (dtRow2.Length == 0)
                        {
                            TXTMakePlanTableRow(iTZ, dtWeekPackPlan.Rows[Index], ref dtWeekTZPlan, _pWeekDays, _PlanColumnName, strMonth, _pCalendar1, strPlant);
                        }
                    }
                }
                //3P3计划
                if (_pLT3 != string.Empty)
                {
                    int iP3 = Convert.ToInt32(_pLT3);
                    if (dtTMP3.Rows.Count <= 0 || strWeek == "1")
                    {
                        TXTMakePlanTableRow(iP3, dtWeekPackPlan.Rows[Index], ref dtWeekP3Plan, _pWeekDays, _PlanColumnName, strMonth, _pCalendar1, strPlant);
                    }
                    else
                    {
                        string strSelect3 = "vcMonth='" + strMonth + "' and vcPartsno='" + _pPartsNo + "' and vcProject1='" + _pCalendar1 + "' and (vcMonTotal is not null)";
                        DataRow[] dtRow3 = dtTMP3.Select(strSelect3);//筛选出要更新的那一行

                        if (dtRow3.Length > 1)
                        {
                            _msg = "品番：" + _pPartsNo + "在周度P3计划管理表中数据不唯一！";
                            break;
                        }
                        else if (dtRow3.Length == 1)
                        {
                            //开始给这一行更新数据

                            //“总数为累加的和”可以直接从包装计划获取
                            TXTMakeRowInPlanTable(iP3, dtWeekPackPlan.Rows[Index], ref dtRow3[0], _pWeekDays, _PlanColumnName, strMonth, _pCalendar1, strPlant);
                            dtWeekP3Plan.Rows.Add(dtRow3[0].ItemArray);//把更新好的数据行放到结果表中
                        }
                        else if (dtRow3.Length == 0)
                        {
                            TXTMakePlanTableRow(iP3, dtWeekPackPlan.Rows[Index], ref dtWeekP3Plan, _pWeekDays, _PlanColumnName, strMonth, _pCalendar1, strPlant);
                        }
                    }
                }
                //0看板打印计划
                int iKanBan = Convert.ToInt32(_pLT0);
                if (dtTMP0.Rows.Count <= 0 || strWeek == "1")
                {
                    TXTMakePlanTableRow(iKanBan, dtWeekPackPlan.Rows[Index], ref dtWeekKanBanPlan, _pWeekDays, _PlanColumnName, strMonth, _pCalendar1, strPlant);
                }
                else
                {
                    string strSelect0 = "vcMonth='" + strMonth + "' and vcPartsno='" + _pPartsNo + "' and vcProject1='" + _pCalendar1 + "' and (vcMonTotal is not null)";
                    DataRow[] dtRow0 = dtTMP0.Select(strSelect0);//筛选出要更新的那一行

                    if (dtRow0.Length > 1)
                    {
                        _msg = "品番：" + _pPartsNo + "在周度看板打印计划管理表中数据不唯一！";
                        break;
                    }
                    else if (dtRow0.Length == 1)
                    {
                        //开始给这一行更新数据

                        //“总数为累加的和”可以直接从包装计划获取
                        TXTMakeRowInPlanTable(iKanBan, dtWeekPackPlan.Rows[Index], ref dtRow0[0], _pWeekDays, _PlanColumnName, strMonth, _pCalendar1, strPlant);
                        dtWeekKanBanPlan.Rows.Add(dtRow0[0].ItemArray);//把更新好的数据行放到结果表中
                    }
                    else if (dtRow0.Length == 0)
                    {
                        TXTMakePlanTableRow(iKanBan, dtWeekPackPlan.Rows[Index], ref dtWeekKanBanPlan, _pWeekDays, _PlanColumnName, strMonth, _pCalendar1, strPlant);
                    }
                }
                #endregion
            }
            #region 消息为空时，更新包装计划数据
            if (_msg.Length <= 0)
            {
                #region 旧的更新计划方式作废
                //不能用先删后插入的方式更新，会造成数据丢失
                //StringBuilder strSQL = new StringBuilder();
                ////删计划 周度包装计划不跨月，不涉及上个月的问题

                //strSQL.AppendLine("delete from WeekPackPlanTbl where vcMonth='" + strMonth + "' and (vcMonTotal <> '' or vcMonTotal is not null) and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + strPlant + "' and vcPartsNo = WeekPackPlanTbl.vcPartsno and dTimeFrom <= '" + strMonth + "-01' and dTimeTo >= '" + strMonth + "-01'); ");
                //strSQL.AppendLine("delete from WeekProdPlanTbl where vcMonth='" + strMonth + "' and (vcMonTotal <> '' or vcMonTotal is not null) and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + strPlant + "' and vcPartsNo = WeekProdPlanTbl.vcPartsno and dTimeFrom <= '" + strMonth + "-01' and dTimeTo >= '" + strMonth + "-01'); ");
                //strSQL.AppendLine("delete from WeekTZPlanTbl where vcMonth='" + strMonth + "' and (vcMonTotal <> '' or vcMonTotal is not null) and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + strPlant + "' and vcPartsNo = WeekTZPlanTbl.vcPartsno and dTimeFrom <= '" + strMonth + "-01' and dTimeTo >= '" + strMonth + "-01'); ");
                //strSQL.AppendLine("delete from WeekP3PlanTbl where vcMonth='" + strMonth + "' and (vcMonTotal <> '' or vcMonTotal is not null) and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + strPlant + "' and vcPartsNo = WeekP3PlanTbl.vcPartsno and dTimeFrom <= '" + strMonth + "-01' and dTimeTo >= '" + strMonth + "-01'); ");
                //strSQL.AppendLine("delete from WeekKanBanPlanTbl where vcMonth='" + strMonth + "' and (vcMonTotal <> '' or vcMonTotal is not null) and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + strPlant + "' and vcPartsNo = WeekKanBanPlanTbl.vcPartsno and dTimeFrom <= '" + strMonth + "-01' and dTimeTo >= '" + strMonth + "-01'); ");
                ////如果是第一周，其他计划会涉及跨月问题

                //if (strWeek == "1")
                //{
                //    strSQL.AppendLine("delete from WeekProdPlanTbl where montouch = '" + strMonth + "' and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + strPlant + "' and vcPartsNo = WeekProdPlanTbl.vcPartsno and dTimeFrom <= '" + strMonth + "-01' and dTimeTo >= '" + strMonth + "-01'); ");
                //    strSQL.AppendLine("delete from WeekTZPlanTbl where montouch = '" + strMonth + "' and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + strPlant + "' and vcPartsNo = WeekTZPlanTbl.vcPartsno and dTimeFrom <= '" + strMonth + "-01' and dTimeTo >= '" + strMonth + "-01'); ");
                //    strSQL.AppendLine("delete from WeekP3PlanTbl where montouch = '" + strMonth + "' and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + strPlant + "' and vcPartsNo = WeekP3PlanTbl.vcPartsno and dTimeFrom <= '" + strMonth + "-01' and dTimeTo >= '" + strMonth + "-01'); ");
                //    strSQL.AppendLine("delete from WeekKanBanPlanTbl where montouch = '" + strMonth + "' and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + strPlant + "' and vcPartsNo = WeekKanBanPlanTbl.vcPartsno and dTimeFrom <= '" + strMonth + "-01' and dTimeTo >= '" + strMonth + "-01'); ");
                //    //如果是第一周，需要清理一遍打印数据

                //    strSQL.AppendLine("delete from tKanbanPrintTbl where vcPlanMonth = '" + strMonth + "' and exists (select vcPartsNo from tPartInfoMaster where vcPartPlant ='" + strPlant + "' and vcPartFrequence = '周度' and vcPartsNo = tKanbanPrintTbl.vcPartsno and dTimeFrom <= '" + strMonth + "-01' and dTimeTo >= '" + strMonth + "-01'); ");
                //}
                //string strDeleteSQL = strSQL.ToString();
                //string strSQLSearch0 = " select * from WeekKanBanPlanTbl ";
                //string strSQLSearch1 = " select * from WeekProdPlanTbl ";
                //string strSQLSearch2 = " select * from WeekTZPlanTbl ";
                //string strSQLSearch3 = " select * from WeekP3PlanTbl ";
                //string strSQLSearch4 = " select * from WeekPackPlanTbl ";
                //DataTable dtupdate0 = new DataTable();//WeekKanBanPlanTbl
                //DataTable dtupdate1 = new DataTable();//WeekProdPlanTbl
                //DataTable dtupdate2 = new DataTable();//WeekTZPlanTbl
                //DataTable dtupdate3 = new DataTable();//WeekP3PlanTbl
                //DataTable dtupdate4 = new DataTable();//WeekPackPlanTbl
                //SqlCommand cmd = new SqlCommand();
                //cmd.Connection = new SqlConnection(ComConnectionHelper.GetConnectionString());
                //cmd.CommandTimeout = 0;
                //cmd.CommandType = CommandType.Text;
                //cmd.CommandText = strDeleteSQL;
                //cmd.Connection.Open();
                //cmd.Transaction = cmd.Connection.BeginTransaction();
                //try
                //{
                //    //先执行删除

                //    cmd.ExecuteNonQuery();
                //    //再改为检索

                //    cmd.CommandText = strSQLSearch4;
                //    SqlDataAdapter apt = new SqlDataAdapter(cmd);
                //    apt.Fill(dtupdate4);
                //    for (int k = 0; k < dtWeekPackPlan.Rows.Count; k++)
                //    {
                //        DataRow dr = dtupdate4.NewRow();
                //        for (int i = 0; i < dtWeekPackPlan.Columns.Count; i++)
                //        {
                //            dr[i] = dtWeekPackPlan.Rows[k][i];
                //        }
                //        dtupdate4.Rows.Add(dr);
                //    }
                //    SqlCommandBuilder sb = new SqlCommandBuilder(apt);
                //    apt.Update(dtupdate4);                    
                //    apt.Dispose();
                //    sb.Dispose();
                //    cmd.Transaction.Commit();
                //    cmd.Connection.Close();
                //}
                //catch (Exception ex)
                //{
                //    cmd.Transaction.Rollback();
                //    cmd.Connection.Close();
                //    throw ex;
                //}
                //更新其他计划
                //Delete语句已执行，则更新方法不必重新delete。

                //TXTUpdateOtherPlan(strSQLSearch0, dtWeekKanBanPlan);//看板打印计划
                //TXTUpdateOtherPlan(strSQLSearch1, dtWeekProdPlan);//生产计划
                //if (dtWeekTZPlan.Rows.Count > 0)
                //{
                //    TXTUpdateOtherPlan(strSQLSearch2, dtWeekTZPlan);//涂装计划
                //}
                //if (dtWeekP3Plan.Rows.Count > 0)
                //{
                //    TXTUpdateOtherPlan(strSQLSearch3, dtWeekP3Plan);//P3计划
                //}
                #endregion
                //更新包装计划
                _msg = TXTUpdatePlan(dtWeekPackPlan, strMonth, strPlant, strUser, "WeekPackPlanTbl");
                //更新其他计划
                _msg = TXTUpdatePlan(dtWeekKanBanPlan, strMonth, strPlant, strUser, "WeekKanBanPlanTbl");//看板打印计划
                _msg = TXTUpdatePlan(dtWeekProdPlan, strMonth, strPlant, strUser, "WeekProdPlanTbl");//生产计划
                if (dtWeekTZPlan.Rows.Count > 0)
                {
                    _msg = TXTUpdatePlan(dtWeekTZPlan, strMonth, strPlant, strUser, "WeekTZPlanTbl");//涂装计划
                }
                if (dtWeekP3Plan.Rows.Count > 0)
                {
                    _msg = TXTUpdatePlan(dtWeekP3Plan, strMonth, strPlant, strUser, "WeekP3PlanTbl");//P3计划
                }
                TXTUpdatePlanMST(strMonth, strPlant);//更新到计划品番数据表
                                                     //生成打印数据
                _msg = TXTCreatOrderNo(strUser, strMonth, strPlant, strWeek);
            }
            #endregion
            return _msg;
        }
        #endregion

        #region 更新到计划品番数据表 - 李兴旺整理

        /// <summary>
        /// 更新到计划品番数据表
        /// </summary>
        /// <param name="mon">对象月</param>
        /// <param name="plant">厂区</param>
        /// <returns></returns>
        private string TXTUpdatePlanMST(string mon, string plant)
        {
            string msg = string.Empty;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(ComConnectionHelper.GetConnectionString());
            cmd.CommandTimeout = 0;
            cmd.Connection.Open();
            cmd.Transaction = cmd.Connection.BeginTransaction();
            SqlDataAdapter apt = new SqlDataAdapter(cmd);
            try
            {
                string tmpmon = mon + "-01";
                StringBuilder sb = new StringBuilder();
                sb.Length = 0;
                sb.AppendLine(" insert into tPlanPartInfo ");
                sb.AppendFormat(" select '{0}' as vcMonth, t1.*,'S' as vcEDFlag,t2.vcPartPlant , ", mon);
                sb.AppendLine(" t2.vcPartsNameCHN,t2.vcCurrentPastCode,t2.vcPorType , t2.vcZB,t2.iQuantityPerContainer,t2.vcQFflag from (");
                sb.AppendLine(" select distinct vcPartsno ,vcCarType,vcDock from WeekPackPlanTbl ");//周度包装计划
                sb.AppendFormat(" where montouch ='{0}' or (vcMonth ='{1}' and montouch is null)", mon, mon);
                sb.AppendLine(" ) t1");
                sb.AppendLine(" left join dbo.tPartInfoMaster t2");
                sb.AppendLine(" on t1.vcPartsno = t2.vcPartsNo  and t1.vcDock = t2.vcDock ");
                sb.AppendFormat(" where t2.vcPartPlant ='{0}'  and t2.dTimeFrom <='{1}' and t2.dTimeTo>='{2}'", plant, tmpmon, tmpmon);

                cmd.CommandText = sb.ToString();
                cmd.ExecuteNonQuery();

                cmd.Transaction.Commit();
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                msg = "更新失败。" + ex.ToString();
                if (cmd.Connection.State == ConnectionState.Open)
                {
                    cmd.Transaction.Rollback();
                    cmd.Connection.Close();
                }
            }
            return msg;
        }
        #endregion

        #region 将数据插入或更新到周度计划表中

        /// <summary>
        /// 将数据插入或更新到周度计划表中

        /// </summary>
        /// <param name="dtSource">数据源</param>
        /// <param name="strMonth">对象月</param>
        /// <param name="strPlant">厂区</param>
        /// <param name="strUser">登录用户</param>
        /// <param name="strTableName">周度计划表名</param>
        /// <returns>提示信息</returns>
        public string TXTUpdatePlan(DataTable dtSource, string strMonth, string strPlant, string strUser, string strTableName)
        {
            string msg = string.Empty;
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = new SqlConnection(ComConnectionHelper.GetConnectionString());
                cmd.CommandTimeout = 0;
                cmd.Connection.Open();
                cmd.Transaction = cmd.Connection.BeginTransaction();
                SqlDataAdapter apt = new SqlDataAdapter(cmd);
                for (int i = 0; i < dtSource.Rows.Count; i++)
                {
                    string vcMonth = dtSource.Rows[i]["vcMonth"].ToString();
                    string vcPartsno = dtSource.Rows[i]["vcPartsno"].ToString();
                    string vcDock = dtSource.Rows[i]["vcDock"].ToString();
                    string vcCarType = dtSource.Rows[i]["vcCarType"].ToString();
                    string vcProject1 = dtSource.Rows[i]["vcProject1"].ToString();
                    string strSelect = "select * from " + strTableName + " where vcMonth='" + vcMonth + "' and vcPartsno='" + vcPartsno + "' and vcDock='" + vcDock + "' and vcCarType='" + vcCarType + "' and vcProject1='" + vcProject1 + "' ";
                    strSelect += "union all ";
                    strSelect += "select * from " + strTableName + " where montouch = '" + vcMonth + "' and vcPartsno='" + vcPartsno + "' and vcDock='" + vcDock + "' and vcCarType='" + vcCarType + "' and vcProject1='" + vcProject1 + "' ";
                    cmd.CommandText = strSelect;
                    //int iRow = cmd.ExecuteNonQuery();
                    DataTable dt = new DataTable();
                    apt.Fill(dt);
                    if (dt.Rows.Count <= 0)
                    {
                        //数据不存在，用插入语句

                        StringBuilder sb = new StringBuilder();
                        sb.Length = 0;
                        #region 插入语句
                        sb.AppendLine("insert into " + strTableName + " (vcMonth,vcPartsno,vcDock,vcCarType,vcProject1,vcProjectName,vcMonTotal,vcD1b,vcD1y,vcD2b,vcD2y,vcD3b,vcD3y,vcD4b,vcD4y,vcD5b,vcD5y,vcD6b,vcD6y,vcD7b,vcD7y,vcD8b,vcD8y,vcD9b,vcD9y,vcD10b,vcD10y,vcD11b,vcD11y,vcD12b,vcD12y,vcD13b,vcD13y,vcD14b,vcD14y,vcD15b,vcD15y,vcD16b,vcD16y,vcD17b,vcD17y,vcD18b,vcD18y,vcD19b,vcD19y,vcD20b,vcD20y,vcD21b,vcD21y,vcD22b,vcD22y,vcD23b,vcD23y,vcD24b,vcD24y,vcD25b,vcD25y,vcD26b,vcD26y,vcD27b,vcD27y,vcD28b,vcD28y,vcD29b,vcD29y,vcD30b,vcD30y,vcD31b,vcD31y,montouch,DADDTIME,DUPDTIME,CUPDUSER) ");
                        sb.AppendLine("  values (");
                        sb.AppendFormat("'{0}'", dtSource.Rows[i]["vcMonth"]);
                        sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcPartsno"]);
                        sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcDock"]);
                        sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcCarType"]);
                        sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcProject1"]);
                        sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcProjectName"]);
                        if (dtSource.Rows[i]["vcMonTotal"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcMonTotal"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        //-------------------------
                        if (dtSource.Rows[i]["vcD1b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD1b"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        if (dtSource.Rows[i]["vcD1y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD1y"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        //-------------------------
                        if (dtSource.Rows[i]["vcD2b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD2b"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        if (dtSource.Rows[i]["vcD2y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD2y"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        //-------------------------
                        if (dtSource.Rows[i]["vcD3b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD3b"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        if (dtSource.Rows[i]["vcD3y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD3y"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        //-------------------------
                        if (dtSource.Rows[i]["vcD4b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD4b"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        if (dtSource.Rows[i]["vcD4y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD4y"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        //-------------------------
                        if (dtSource.Rows[i]["vcD5b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD5b"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        if (dtSource.Rows[i]["vcD5y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD5y"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        //-------------------------
                        if (dtSource.Rows[i]["vcD6b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD6b"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        if (dtSource.Rows[i]["vcD6y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD6y"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        //-------------------------
                        if (dtSource.Rows[i]["vcD7b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD7b"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        if (dtSource.Rows[i]["vcD7y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD7y"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        //-------------------------
                        if (dtSource.Rows[i]["vcD8b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD8b"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        if (dtSource.Rows[i]["vcD8y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD8y"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        //-------------------------
                        if (dtSource.Rows[i]["vcD9b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD9b"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        if (dtSource.Rows[i]["vcD9y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD9y"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        //-------------------------
                        if (dtSource.Rows[i]["vcD10b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD10b"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        if (dtSource.Rows[i]["vcD10y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD10y"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        //-------------------------
                        if (dtSource.Rows[i]["vcD11b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD11b"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        if (dtSource.Rows[i]["vcD11y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD11y"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        //-------------------------
                        if (dtSource.Rows[i]["vcD12b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD12b"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        if (dtSource.Rows[i]["vcD12y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD12y"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        //-------------------------
                        if (dtSource.Rows[i]["vcD13b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD13b"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        if (dtSource.Rows[i]["vcD13y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD13y"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        //-------------------------
                        if (dtSource.Rows[i]["vcD14b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD14b"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        if (dtSource.Rows[i]["vcD14y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD14y"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        //-------------------------
                        if (dtSource.Rows[i]["vcD15b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD15b"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        if (dtSource.Rows[i]["vcD15y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD15y"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        //-------------------------
                        if (dtSource.Rows[i]["vcD16b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD16b"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        if (dtSource.Rows[i]["vcD16y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD16y"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        //-------------------------
                        if (dtSource.Rows[i]["vcD17b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD17b"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        if (dtSource.Rows[i]["vcD17y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD17y"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        //-------------------------
                        if (dtSource.Rows[i]["vcD18b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD18b"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        if (dtSource.Rows[i]["vcD18y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD18y"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        //-------------------------
                        if (dtSource.Rows[i]["vcD19b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD19b"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        if (dtSource.Rows[i]["vcD19y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD19y"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        //-------------------------
                        if (dtSource.Rows[i]["vcD20b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD20b"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        if (dtSource.Rows[i]["vcD20y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD20y"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        //-------------------------
                        if (dtSource.Rows[i]["vcD21b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD21b"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        if (dtSource.Rows[i]["vcD21y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD21y"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        //-------------------------
                        if (dtSource.Rows[i]["vcD22b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD22b"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        if (dtSource.Rows[i]["vcD22y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD22y"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        //-------------------------
                        if (dtSource.Rows[i]["vcD23b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD23b"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        if (dtSource.Rows[i]["vcD23y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD23y"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        //-------------------------
                        if (dtSource.Rows[i]["vcD24b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD24b"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        if (dtSource.Rows[i]["vcD24y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD24y"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        //-------------------------
                        if (dtSource.Rows[i]["vcD25b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD25b"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        if (dtSource.Rows[i]["vcD25y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD25y"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        //-------------------------
                        if (dtSource.Rows[i]["vcD26b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD26b"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        if (dtSource.Rows[i]["vcD26y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD26y"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        //-------------------------
                        if (dtSource.Rows[i]["vcD27b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD27b"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        if (dtSource.Rows[i]["vcD27y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD27y"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        //-------------------------
                        if (dtSource.Rows[i]["vcD28b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD28b"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        if (dtSource.Rows[i]["vcD28y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD28y"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        //-------------------------
                        if (dtSource.Rows[i]["vcD29b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD29b"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        if (dtSource.Rows[i]["vcD29y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD29y"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        //-------------------------
                        if (dtSource.Rows[i]["vcD30b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD30b"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        if (dtSource.Rows[i]["vcD30y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD30y"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        //-------------------------
                        if (dtSource.Rows[i]["vcD31b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD31b"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        if (dtSource.Rows[i]["vcD31y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["vcD31y"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        //-------------------------
                        if (dtSource.Rows[i]["montouch"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(",'{0}'", dtSource.Rows[i]["montouch"]);
                        }
                        else
                        {
                            sb.AppendLine(",null");
                        }
                        sb.AppendFormat(",'{0}'", dtSource.Rows[i]["DADDTIME"]);
                        sb.AppendFormat(",'{0}'", dtSource.Rows[i]["DUPDTIME"]);
                        sb.AppendFormat(",'{0}'", dtSource.Rows[i]["CUPDUSER"]);
                        sb.AppendLine(") ");
                        #endregion
                        cmd.CommandText = sb.ToString();
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        //数据存在，用更新语句
                        StringBuilder sb = new StringBuilder();
                        sb.Length = 0;
                        #region 更新语句
                        sb.AppendLine("update " + strTableName + " set ");
                        sb.AppendFormat("  vcProjectName='{0}'", dtSource.Rows[i]["vcProjectName"].ToString());
                        if (dtSource.Rows[i]["vcMonTotal"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcMonTotal='{0}'", dtSource.Rows[i]["vcMonTotal"]);
                        }
                        if (dtSource.Rows[i]["vcD1b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD1b='{0}'", dtSource.Rows[i]["vcD1b"]);
                        }
                        if (dtSource.Rows[i]["vcD1y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD1y='{0}'", dtSource.Rows[i]["vcD1y"]);
                        }

                        if (dtSource.Rows[i]["vcD2b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD2b='{0}'", dtSource.Rows[i]["vcD2b"]);
                        }
                        if (dtSource.Rows[i]["vcD2y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD2y='{0}'", dtSource.Rows[i]["vcD2y"]);
                        }

                        if (dtSource.Rows[i]["vcD3b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD3b='{0}'", dtSource.Rows[i]["vcD3b"]);
                        }
                        if (dtSource.Rows[i]["vcD3y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD3y='{0}'", dtSource.Rows[i]["vcD3y"]);
                        }

                        if (dtSource.Rows[i]["vcD4b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD4b='{0}'", dtSource.Rows[i]["vcD4b"]);
                        }
                        if (dtSource.Rows[i]["vcD4y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD4y='{0}'", dtSource.Rows[i]["vcD4y"]);
                        }

                        if (dtSource.Rows[i]["vcD5b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD5b='{0}'", dtSource.Rows[i]["vcD5b"]);
                        }
                        if (dtSource.Rows[i]["vcD5y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD5y='{0}'", dtSource.Rows[i]["vcD5y"]);
                        }

                        if (dtSource.Rows[i]["vcD6b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD6b='{0}'", dtSource.Rows[i]["vcD6b"]);
                        }
                        if (dtSource.Rows[i]["vcD6y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD6y='{0}'", dtSource.Rows[i]["vcD6y"]);
                        }

                        if (dtSource.Rows[i]["vcD7b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD7b='{0}'", dtSource.Rows[i]["vcD7b"]);
                        }
                        if (dtSource.Rows[i]["vcD7y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD7y='{0}'", dtSource.Rows[i]["vcD7y"]);
                        }

                        if (dtSource.Rows[i]["vcD8b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD8b='{0}'", dtSource.Rows[i]["vcD8b"]);
                        }
                        if (dtSource.Rows[i]["vcD8y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD8y='{0}'", dtSource.Rows[i]["vcD8y"]);
                        }

                        if (dtSource.Rows[i]["vcD9b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD9b='{0}'", dtSource.Rows[i]["vcD9b"]);
                        }
                        if (dtSource.Rows[i]["vcD9y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD9y='{0}'", dtSource.Rows[i]["vcD9y"]);
                        }

                        if (dtSource.Rows[i]["vcD10b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD10b='{0}'", dtSource.Rows[i]["vcD10b"]);
                        }
                        if (dtSource.Rows[i]["vcD10y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD10y='{0}'", dtSource.Rows[i]["vcD10y"]);
                        }

                        if (dtSource.Rows[i]["vcD11b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD11b='{0}'", dtSource.Rows[i]["vcD11b"]);
                        }
                        if (dtSource.Rows[i]["vcD11y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD11y='{0}'", dtSource.Rows[i]["vcD11y"]);
                        }

                        if (dtSource.Rows[i]["vcD12b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD12b='{0}'", dtSource.Rows[i]["vcD12b"]);
                        }
                        if (dtSource.Rows[i]["vcD12y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD12y='{0}'", dtSource.Rows[i]["vcD12y"]);
                        }

                        if (dtSource.Rows[i]["vcD13b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD13b='{0}'", dtSource.Rows[i]["vcD13b"]);
                        }
                        if (dtSource.Rows[i]["vcD13y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD13y='{0}'", dtSource.Rows[i]["vcD13y"]);
                        }

                        if (dtSource.Rows[i]["vcD14b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD14b='{0}'", dtSource.Rows[i]["vcD14b"]);
                        }
                        if (dtSource.Rows[i]["vcD14y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD14y='{0}'", dtSource.Rows[i]["vcD14y"]);
                        }

                        if (dtSource.Rows[i]["vcD15b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD15b='{0}'", dtSource.Rows[i]["vcD15b"]);
                        }
                        if (dtSource.Rows[i]["vcD15y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD15y='{0}'", dtSource.Rows[i]["vcD15y"]);
                        }

                        if (dtSource.Rows[i]["vcD16b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD16b='{0}'", dtSource.Rows[i]["vcD16b"]);
                        }
                        if (dtSource.Rows[i]["vcD16y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD16y='{0}'", dtSource.Rows[i]["vcD16y"]);
                        }

                        if (dtSource.Rows[i]["vcD17b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD17b='{0}'", dtSource.Rows[i]["vcD17b"]);
                        }
                        if (dtSource.Rows[i]["vcD17y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD17y='{0}'", dtSource.Rows[i]["vcD17y"]);
                        }

                        if (dtSource.Rows[i]["vcD18b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD18b='{0}'", dtSource.Rows[i]["vcD18b"]);
                        }
                        if (dtSource.Rows[i]["vcD18y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD18y='{0}'", dtSource.Rows[i]["vcD18y"]);
                        }

                        if (dtSource.Rows[i]["vcD19b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD19b='{0}'", dtSource.Rows[i]["vcD19b"]);
                        }
                        if (dtSource.Rows[i]["vcD19y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD19y='{0}'", dtSource.Rows[i]["vcD19y"]);
                        }

                        if (dtSource.Rows[i]["vcD20b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD20b='{0}'", dtSource.Rows[i]["vcD20b"]);
                        }
                        if (dtSource.Rows[i]["vcD20y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD20y='{0}'", dtSource.Rows[i]["vcD20y"]);
                        }

                        if (dtSource.Rows[i]["vcD21b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD21b='{0}'", dtSource.Rows[i]["vcD21b"]);
                        }
                        if (dtSource.Rows[i]["vcD21y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD21y='{0}'", dtSource.Rows[i]["vcD21y"]);
                        }

                        if (dtSource.Rows[i]["vcD22b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD22b='{0}'", dtSource.Rows[i]["vcD22b"]);
                        }
                        if (dtSource.Rows[i]["vcD22y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD22y='{0}'", dtSource.Rows[i]["vcD22y"]);
                        }

                        if (dtSource.Rows[i]["vcD23b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD23b='{0}'", dtSource.Rows[i]["vcD23b"]);
                        }
                        if (dtSource.Rows[i]["vcD23y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD23y='{0}'", dtSource.Rows[i]["vcD23y"]);
                        }

                        if (dtSource.Rows[i]["vcD24b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD24b='{0}'", dtSource.Rows[i]["vcD24b"]);
                        }
                        if (dtSource.Rows[i]["vcD24y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD24y='{0}'", dtSource.Rows[i]["vcD24y"]);
                        }

                        if (dtSource.Rows[i]["vcD25b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD25b='{0}'", dtSource.Rows[i]["vcD25b"]);
                        }
                        if (dtSource.Rows[i]["vcD25y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD25y='{0}'", dtSource.Rows[i]["vcD25y"]);
                        }

                        if (dtSource.Rows[i]["vcD26b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD26b='{0}'", dtSource.Rows[i]["vcD26b"]);
                        }
                        if (dtSource.Rows[i]["vcD26y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD26y='{0}'", dtSource.Rows[i]["vcD26y"]);
                        }

                        if (dtSource.Rows[i]["vcD27b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD27b='{0}'", dtSource.Rows[i]["vcD27b"]);
                        }
                        if (dtSource.Rows[i]["vcD27y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD27y='{0}'", dtSource.Rows[i]["vcD27y"]);
                        }

                        if (dtSource.Rows[i]["vcD28b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD28b='{0}'", dtSource.Rows[i]["vcD28b"]);
                        }
                        if (dtSource.Rows[i]["vcD28y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD28y='{0}'", dtSource.Rows[i]["vcD28y"]);
                        }

                        if (dtSource.Rows[i]["vcD29b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD29b='{0}'", dtSource.Rows[i]["vcD29b"]);
                        }
                        if (dtSource.Rows[i]["vcD29y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD29y='{0}'", dtSource.Rows[i]["vcD29y"]);
                        }

                        if (dtSource.Rows[i]["vcD30b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD30b='{0}'", dtSource.Rows[i]["vcD30b"]);
                        }
                        if (dtSource.Rows[i]["vcD30y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD30y='{0}'", dtSource.Rows[i]["vcD30y"]);
                        }

                        if (dtSource.Rows[i]["vcD31b"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD31b='{0}'", dtSource.Rows[i]["vcD31b"]);
                        }
                        if (dtSource.Rows[i]["vcD31y"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,vcD31y='{0}'", dtSource.Rows[i]["vcD31y"]);
                        }

                        if (dtSource.Rows[i]["montouch"].ToString() != string.Empty)
                        {
                            sb.AppendFormat(" ,montouch='{0}'", dtSource.Rows[i]["montouch"]);
                        }
                        sb.AppendFormat(" ,DUPDTIME='{0}'", dtSource.Rows[i]["DUPDTIME"]);
                        sb.AppendFormat(" ,CUPDUSER='{0}'", dtSource.Rows[i]["CUPDUSER"]);
                        sb.AppendFormat(" where vcMonth='{0}' ", dtSource.Rows[i]["vcMonth"]);
                        sb.AppendFormat(" and vcPartsno='{0}' ", dtSource.Rows[i]["vcPartsno"]);
                        sb.AppendFormat(" and vcDock='{0}' ", dtSource.Rows[i]["vcDock"]);
                        sb.AppendFormat(" and vcCarType='{0}' ", dtSource.Rows[i]["vcCarType"]);
                        sb.AppendFormat(" and vcProject1='{0}' ", dtSource.Rows[i]["vcProject1"]);
                        #endregion
                        cmd.CommandText = sb.ToString();
                        cmd.ExecuteNonQuery();
                    }
                }
                cmd.Transaction.Commit();
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                msg = "更新失败。" + ex.ToString();
                if (cmd.Connection.State == ConnectionState.Open)
                {
                    cmd.Transaction.Rollback();
                    cmd.Connection.Close();
                }
            }
            return msg;
        }
        #endregion

        #region 生成打印数据 - 李兴旺整理

        public string TXTCreatOrderNo(string user, string mon, string plant, string week)
        {
            string msg = "";
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(ComConnectionHelper.GetConnectionString());
            cmd.CommandTimeout = 0;
            cmd.Connection.Open();
            cmd.Transaction = cmd.Connection.BeginTransaction();
            SqlDataAdapter apt = new SqlDataAdapter(cmd);
            try
            {
                //生成打印数据
                msg = CreatOrderNo(cmd, mon, apt, user, plant, week);//2018-2-26增加AB值 - Malcolm.L 刘刚
                if (msg.Length > 0)
                {
                    cmd.Transaction.Rollback();
                    cmd.Connection.Close();
                    return msg;
                }
                cmd.Transaction.Commit();
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                msg = "打印数据生成失败！" + ex.ToString();
                if (cmd.Transaction != null) cmd.Transaction.Rollback();
                cmd.Connection.Close();
            }
            return msg;
        }

        //生成看板打印数据
        public string CreatOrderNo(SqlCommand cmd, string mon, SqlDataAdapter apt, string user, string plant, string week)
        {
            //不同计划对应的列名考虑LT
            string msg = "";
            int iWeekDays = 0;
            string tmp = TXTWeekCreatOrderNoColumns(mon, week, plant, ref iWeekDays);
            //string tmp = "";
            //for (int i = 1; i < 32; i++)
            //{
            //    if (i == 31)
            //        tmp += "vcD" + i + "b,	vcD" + i + "y";
            //    else tmp += "vcD" + i + "b,	vcD" + i + "y,";
            //}
            int iLT0 = 0; int iLT1 = 0; int iLT2 = 0; int iLT3 = 0; int iLT4 = 0;
            TXTWeekCreatOrderNoLT(mon, week, plant, ref iLT0, ref iLT1, ref iLT2, ref iLT3, ref iLT4);

            StringBuilder sb = new StringBuilder();
            sb.Length = 0;
            sb.AppendLine(" select distinct vcMonth,allTotal, daysig from ( ");
            sb.AppendLine("   select distinct vcMonth,allTotal, daysig,vcPartsno ,vcDock from (");
            sb.AppendLine("   select vcMonth, vcPartsno,vcDock,vcCartype,sigTotal , allTotal from WeekPackPlanTbl");
            sb.AppendFormat("    unpivot( sigTotal for allTotal in( {0} ", tmp);
            sb.AppendFormat("  )) P where LEN(sigTotal)>0 and montouch ='{0}'", mon);
            sb.AppendLine("    union all ");
            sb.AppendLine("    select vcMonth,vcPartsno,vcDock,vcCartype,sigTotal , allTotal from WeekPackPlanTbl  ");
            sb.AppendFormat("    unpivot( sigTotal for allTotal in( {0} ", tmp);
            sb.AppendFormat("  )) P where LEN(sigTotal)>0 and vcMonth ='{0}'", mon);
            sb.AppendLine("    ) t1");
            sb.AppendLine("    left join (");
            sb.AppendFormat("    select daysig , dayN from sPlanConst unpivot ( daysig for dayN in( {0}", tmp);
            sb.AppendLine("     )) P ) t2 ");
            sb.AppendLine("     on t1.allTotal = t2.dayN");
            sb.AppendLine(" ) tall ");
            sb.AppendLine(" left join dbo.tPartInfoMaster tinfo on tall.vcPartsno = tinfo.vcPartsNo and tall.vcDock = tinfo.vcDock  and   tinfo.dTimeFrom<= '" + mon + "-01" + "' and tinfo.dTimeTo >= '" + mon + "-01" + "'");
            sb.AppendFormat(" where tinfo.vcPartPlant ='{0}' ", plant);
            sb.AppendLine("    order by vcMonth , allTotal");
            cmd.CommandText = sb.ToString();
            DataTable DayType = new DataTable();
            apt.Fill(DayType);
            cmd.CommandText = TXTGetPlanSearch(mon, "WeekKanBanPlanTbl", week, plant, iLT0, "1");//周度看板计划
            DataTable pro0 = new DataTable();
            apt.Fill(pro0);
            cmd.CommandText = TXTGetPlanSearch(mon, "WeekProdPlanTbl", week, plant, iLT1, "");//周度生产计划
            DataTable pro1 = new DataTable();
            apt.Fill(pro1);
            cmd.CommandText = TXTGetPlanSearch(mon, "WeekTZPlanTbl", week, plant, iLT2, "");//周度涂装计划
            DataTable pro2 = new DataTable();
            apt.Fill(pro2);
            cmd.CommandText = TXTGetPlanSearch(mon, "WeekP3PlanTbl", week, plant, iLT3, "");//周度P3计划
            DataTable pro3 = new DataTable();
            apt.Fill(pro3);
            cmd.CommandText = TXTGetPlanSearch(mon, "WeekPackPlanTbl", week, plant, iLT4, "");//周度包装计划
            DataTable pro4 = new DataTable();
            apt.Fill(pro4);
            //------------------优化start
            cmd.CommandText = " select top(1)* from tKanbanPrintTbl ";
            DataTable BulkInsert = new DataTable();
            apt.Fill(BulkInsert);
            BulkInsert = BulkInsert.Clone();
            BulkInsert.Columns.Add("bushu");
            BulkInsert.Columns.Add("dayin");
            BulkInsert.Columns.Add("shengchan");
            string partsql = " select vcPartsno,vcDock,vcCarFamilyCode ,t1.iQuantityPerContainer,t1.vcPorType,t1.vcZB,t2.vcProName0,t2.vcProName1,t2.vcProName2,t2.vcProName3,t2.vcProName4,t2.vcCalendar0,t2.vcCalendar1,t2.vcCalendar2,t2.vcCalendar3,t2.vcCalendar4  from dbo.tPartInfoMaster t1";
            partsql += " left join dbo.ProRuleMst t2 on t1.vcPorType=t2.vcPorType and t1.vcZB = t2.vcZB ";
            partsql += "  where exists (select vcPartsno from WeekPackPlanTbl where (vcMonth='" + mon + "' or montouch ='" + mon + "') and vcPartsno = t1.vcPartsno  )  and t1.dTimeFrom<= '" + mon + "-01" + "' and t1.dTimeTo >= '" + mon + "-01" + "'  ";
            cmd.CommandText = partsql;
            DataTable dtcalendarname = new DataTable();
            apt.Fill(dtcalendarname);
            DataTable dt_calendarname = new DataTable();
            //------------------优化end
            for (int i = 0; i < DayType.Rows.Count; i++)
            {
                DataRow[] dr = pro4.Select(" vcMonth ='" + DayType.Rows[i]["vcMonth"].ToString() + "' and allTotal ='" + DayType.Rows[i]["allTotal"].ToString() + "' ");
                int OrderStart = 0000;
                string tmp_part = "";
                string tmp_dock = "";
                for (int j = 0; j < dr.Length; j++)
                {
                    string vcPartsno = dr[j]["vcPartsno"].ToString();
                    string vcDock = dr[j]["vcDock"].ToString();
                    string vcCartype = dr[j]["vcCartype"].ToString();
                    string flag = dr[j]["flag"].ToString();
                    #region 生成看板打印数据
                    dt_calendarname = dtcalendarname.Select("vcPartsno='" + vcPartsno + "'  and vcDock ='" + vcDock + "' and vcCarFamilyCode ='" + vcCartype + "'   ").CopyToDataTable();
                    string srs = dt_calendarname.Rows[0]["iQuantityPerContainer"].ToString().Trim();
                    int k = Convert.ToInt32(dr[j]["sigTotal"]) / Convert.ToInt32(srs);
                    if (tmp_part != vcPartsno || tmp_dock != vcDock)
                    {
                        tmp_part = vcPartsno;
                        tmp_dock = vcDock;
                        OrderStart = 0000;
                    }
                    if (k == 0)
                        continue;
                    //pro0
                    DataRow[] dr0 = pro0.Select(" vcPartsno='" + vcPartsno + "' and vcDock='" + vcDock + "' and vcCartype='" + vcCartype + "' and flag='" + flag + "' ");
                    string day00 = dr0[0]["daysig"].ToString().Split('-')[0];
                    string vcBanZhi00 = dr0[0]["daysig"].ToString().Split('-')[1];
                    string vcComDate00 = dr0[0]["vcMonth"].ToString() + "-" + day00;
                    string zhi00 = vcBanZhi00 == "白" ? "0" : "1";
                    //2018-2-26 Malcolm.L 刘刚 获取工程0的A/B班值

                    string vcAB00 = "";// ICalendar2.getABClass(vcComDate00, zhi00, plant, dt_calendarname.Rows[0]["vcCalendar0"].ToString().Trim());
                    string by0 = vcBanZhi00 == "白" ? "01" : "02";
                    string vcProject00 = dt_calendarname.Rows[0]["vcProName0"].ToString();
                    //pro1
                    DataRow[] dr1 = pro1.Select(" vcPartsno='" + vcPartsno + "' and vcDock='" + vcDock + "' and vcCartype='" + vcCartype + "' and flag='" + flag + "' ");
                    string day01 = dr1[0]["daysig"].ToString().Split('-')[0];
                    string vcBanZhi01 = dr1[0]["daysig"].ToString().Split('-')[1];
                    string vcComDate01 = dr1[0]["vcMonth"].ToString() + "-" + day01;
                    string zhi01 = vcBanZhi01 == "白" ? "0" : "1";
                    //2018-2-26 Malcolm.L 刘刚 获取工程1的A/B班值

                    string vcAB01 = "";// ICalendar2.getABClass(vcComDate01, zhi01, plant, dt_calendarname.Rows[0]["vcCalendar1"].ToString().Trim());
                    string by1 = vcBanZhi01 == "白" ? "01" : "02";
                    string vcProject01 = dt_calendarname.Rows[0]["vcProName1"].ToString();
                    //pro2
                    DataRow[] dr2 = pro2.Select(" vcPartsno='" + vcPartsno + "' and vcDock='" + vcDock + "' and vcCartype='" + vcCartype + "' and flag='" + flag + "' ");
                    string day02 = "";
                    string vcBanZhi02 = "";
                    string vcComDate02 = "";
                    string zhi02 = "";
                    string vcAB02 = "";//2018-2-26 Malcolm.L 刘刚 获取工程2的A/B班值

                    string by2 = "";
                    string vcProject02 = "";
                    if (dr2.Length > 0)
                    {
                        day02 = dr2[0]["daysig"].ToString().Split('-')[0];
                        vcBanZhi02 = dr2[0]["daysig"].ToString().Split('-')[1];
                        vcComDate02 = dr2[0]["vcMonth"].ToString() + "-" + day02;
                        zhi02 = vcBanZhi02 == "白" ? "0" : "1";
                        //2018-2-26 Malcolm.L 刘刚 获取工程2的A/B班值

                        vcAB02 = "";// ICalendar2.getABClass(vcComDate02, zhi02, plant, dt_calendarname.Rows[0]["vcCalendar2"].ToString().Trim());
                        by2 = vcBanZhi02 == "白" ? "01" : "02";
                        vcProject02 = dt_calendarname.Rows[0]["vcProName2"].ToString();
                    }
                    //pro3
                    DataRow[] dr3 = pro3.Select(" vcPartsno='" + vcPartsno + "' and vcDock='" + vcDock + "' and vcCartype='" + vcCartype + "' and flag='" + flag + "' ");
                    string day03 = "";
                    string vcBanZhi03 = "";
                    string vcComDate03 = "";
                    string zhi03 = "";
                    string vcAB03 = "";//2018-2-26 Malcolm.L 刘刚 获取工程3的A/B班值

                    string by3 = "";
                    string vcProject03 = "";
                    if (dr3.Length > 0)
                    {
                        day03 = dr3[0]["daysig"].ToString().Split('-')[0];
                        vcBanZhi03 = dr3[0]["daysig"].ToString().Split('-')[1];
                        vcComDate03 = dr3[0]["vcMonth"].ToString() + "-" + day03;
                        zhi03 = vcBanZhi03 == "白" ? "0" : "1";
                        //2018-2-26 Malcolm.L 刘刚 获取工程3的A/B班值

                        vcAB03 = "";// ICalendar2.getABClass(vcComDate03, zhi03, plant, dt_calendarname.Rows[0]["vcCalendar3"].ToString().Trim());
                        by3 = vcBanZhi03 == "白" ? "01" : "02";
                        vcProject03 = dt_calendarname.Rows[0]["vcProName3"].ToString();
                    }
                    //pro4
                    string day04 = dr[j]["daysig"].ToString().Split('-')[0];
                    string vcBanZhi04 = dr[j]["daysig"].ToString().Split('-')[1];//白/夜  白是0 夜是1
                    string vcComDate04 = dr[j]["vcMonth"].ToString() + "-" + day04;
                    string zhi04 = vcBanZhi04 == "白" ? "0" : "1";
                    //2018-2-26 Malcolm.L 刘刚 获取工程0的A/B班值

                    string vcAB04 = "";// ICalendar2.getABClass(vcComDate04, zhi04, plant, dt_calendarname.Rows[0]["vcCalendar4"].ToString().Trim());
                    string by04 = vcBanZhi04 == "白" ? "01" : "02";
                    string vcProject04 = dt_calendarname.Rows[0]["vcProName4"].ToString();
                    for (int n = 0; n < k; n++)
                    {
                        OrderStart++;
                        string orderSerial = "";
                        string orderNo = vcComDate04.Replace("-", "") + by04;//订单号

                        //优化start
                        DataRow drInsert = BulkInsert.NewRow();
                        drInsert["iNo"] = DBNull.Value;
                        drInsert["vcTips"] = DBNull.Value;
                        drInsert["vcPrintflag"] = DBNull.Value;
                        drInsert["vcPrintTime"] = DBNull.Value;
                        drInsert["vcKBType"] = DBNull.Value;
                        drInsert["dUpdateTime"] = DBNull.Value;
                        drInsert["vcUpdater"] = DBNull.Value;
                        drInsert["vcPrintSpec"] = DBNull.Value;
                        drInsert["vcPrintflagED"] = DBNull.Value;
                        drInsert["vcPrintTimeED"] = DBNull.Value;

                        drInsert["vcQuantityPerContainer"] = srs;
                        drInsert["vcPartsNo"] = dr[j]["vcPartsno"].ToString();
                        drInsert["vcDock"] = dr[j]["vcDock"].ToString();
                        drInsert["vcCarType"] = dr[j]["vcCartype"].ToString();
                        drInsert["vcEDflag"] = 'S';
                        drInsert["vcKBorderno"] = orderNo;
                        drInsert["vcKBSerial"] = orderSerial;
                        drInsert["vcProject00"] = vcProject00;
                        drInsert["vcProject01"] = vcProject01;
                        drInsert["vcProject02"] = vcProject02;
                        drInsert["vcProject03"] = vcProject03;
                        drInsert["vcProject04"] = vcProject04;
                        drInsert["vcComDate00"] = vcComDate00;
                        drInsert["vcComDate01"] = vcComDate01;
                        drInsert["vcComDate02"] = vcComDate02;
                        drInsert["vcComDate03"] = vcComDate03;
                        drInsert["vcComDate04"] = vcComDate04;
                        drInsert["vcBanZhi00"] = zhi00;
                        drInsert["vcBanZhi01"] = zhi01;
                        drInsert["vcBanZhi02"] = zhi02;
                        drInsert["vcBanZhi03"] = zhi03;
                        drInsert["vcBanZhi04"] = zhi04;
                        drInsert["vcAB00"] = vcAB00;//
                        drInsert["vcAB01"] = vcAB01;//
                        drInsert["vcAB02"] = vcAB02;//
                        drInsert["vcAB03"] = vcAB03;//
                        drInsert["vcAB04"] = vcAB04;//
                        drInsert["vcCreater"] = user;
                        drInsert["dCreatTime"] = DateTime.Now;
                        drInsert["vcPlanMonth"] = mon;

                        drInsert["bushu"] = dt_calendarname.Rows[0]["vcPorType"].ToString();//部署
                        drInsert["dayin"] = vcComDate00 + zhi00;//打印
                        drInsert["shengchan"] = vcComDate01 + zhi01;//生产

                        BulkInsert.Rows.Add(drInsert);
                        //优化end
                    }
                    #endregion
                }
            }
            //分组
            var query = from t in BulkInsert.AsEnumerable()
                        group t by new
                        {
                            t1 = t.Field<string>("vcKBorderno"),
                            t2 = t.Field<string>("bushu"),
                            t3 = t.Field<string>("dayin"),
                            t4 = t.Field<string>("shengchan")
                        }
                            into m
                        select m;
            //分组排连番

            for (int m = 0; m < query.Count(); m++)
            {
                DataTable dt = query.ElementAt(m).CopyToDataTable();
                //按工位排序

                DataView dv = dt.DefaultView;
                dv.Sort = "vcProject01";
                dt = dv.ToTable();
                string serial = "0000";
                for (int n = 0; n < dt.Rows.Count; n++)
                {
                    serial = (Convert.ToInt32(serial) + 1).ToString("0000");
                    dt.Rows[n]["vcKBSerial"] = serial;
                    //检查该连番下是否存在数据 存在时：
                    string ssql = "select * from tKanbanPrintTbl t1 left join tPartInfoMaster t2 on t1.vcPartsNo = t2.vcPartsNo and t1.vcDock  = t2.vcDock ";
                    ssql += "  where t1.vcEDflag ='S' and t1.vcKBorderno='" + dt.Rows[n]["vcKBorderno"].ToString() + "' ";
                    ssql += " and t2.vcPorType = '" + dt.Rows[n]["bushu"].ToString() + "' and vcComDate00 ='" + dt.Rows[n]["vcComDate00"].ToString() + "' and vcBanZhi00='" + dt.Rows[n]["vcBanZhi00"].ToString() + "' ";
                    ssql += " and vcComDate01 ='" + dt.Rows[n]["vcComDate01"].ToString() + "' and vcBanZhi01 ='" + dt.Rows[n]["vcBanZhi01"].ToString() + "' ";
                    ssql += " and t2.vcPartFrequence = '周度' ";

                    //ssql += "and dTimeFrom>=convert(varchar(10),GETDATE(),23) and dTimeTo<=convert(varchar(10),GETDATE(),23)  ";//改造是月度品番，这里是周度品番
                    //20180930上面SQL文最后一行增加对周度月度品番的判断，因为SQL文是按照部署、工程0和工程1的日期和值别检索数据，
                    //会包括周度品番，而看板打印数据不包含周度品番，且其余月度品番的看板序列号也会发生变化，则造成覆盖范围不符的情况

                    //因此需要清空旧有的（若导入对象月为七月，则包括六月底到八月初的）看板打印数据 - 李兴旺

                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = ssql;
                    apt = new SqlDataAdapter(cmd);
                    DataTable dttmp = new DataTable();
                    apt.Fill(dttmp);
                    if (dttmp.Rows.Count > 0)//如果存在
                    {
                        DataRow[] existKB = dttmp.Select("vcPartsNo = '" + dt.Rows[n]["vcPartsNo"].ToString().Trim() + "' and vcDock ='" + dt.Rows[n]["vcDock"].ToString().Trim() + "' and vcKBSerial ='" + serial + "'");
                        if (existKB.Count() > 0)//1、与该信息相符 
                        {
                            dt.Rows[n].Delete();
                            n--;
                            continue;
                        }
                        else//2、与该信息不符

                        {
                            msg = "生产计划调整与覆盖范围不符，请重新调整后再导入。";
                            return msg;
                        }
                    }
                    else//插入
                    {

                    }
                }
                if (dt.Rows.Count == 0) continue;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "InsertKanBanPrint";
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@vcPartsNo", SqlDbType.VarChar, 12, "vcPartsNo");
                cmd.Parameters.Add("@vcDock", SqlDbType.VarChar, 2, "vcDock");
                cmd.Parameters.Add("@vcCarType", SqlDbType.VarChar, 4, "vcCarType");
                cmd.Parameters.Add("@vcEDflag", SqlDbType.VarChar, 2, "vcEDflag");
                cmd.Parameters.Add("@vcKBorderno", SqlDbType.VarChar, 12, "vcKBorderno");
                cmd.Parameters.Add("@vcKBSerial", SqlDbType.VarChar, 4, "vcKBSerial");
                cmd.Parameters.Add("@vcProject00", SqlDbType.VarChar, 20, "vcProject00");
                cmd.Parameters.Add("@vcProject01", SqlDbType.VarChar, 20, "vcProject01");
                cmd.Parameters.Add("@vcProject02", SqlDbType.VarChar, 20, "vcProject02");
                cmd.Parameters.Add("@vcProject03", SqlDbType.VarChar, 20, "vcProject03");
                cmd.Parameters.Add("@vcProject04", SqlDbType.VarChar, 20, "vcProject04");
                cmd.Parameters.Add("@vcComDate00", SqlDbType.VarChar, 10, "vcComDate00");
                cmd.Parameters.Add("@vcComDate01", SqlDbType.VarChar, 10, "vcComDate01");
                cmd.Parameters.Add("@vcComDate02", SqlDbType.VarChar, 10, "vcComDate02");
                cmd.Parameters.Add("@vcComDate03", SqlDbType.VarChar, 10, "vcComDate03");
                cmd.Parameters.Add("@vcComDate04", SqlDbType.VarChar, 10, "vcComDate04");
                cmd.Parameters.Add("@vcBanZhi00", SqlDbType.VarChar, 1, "vcBanZhi00");
                cmd.Parameters.Add("@vcBanZhi01", SqlDbType.VarChar, 1, "vcBanZhi01");
                cmd.Parameters.Add("@vcBanZhi02", SqlDbType.VarChar, 1, "vcBanZhi02");
                cmd.Parameters.Add("@vcBanZhi03", SqlDbType.VarChar, 1, "vcBanZhi03");
                cmd.Parameters.Add("@vcBanZhi04", SqlDbType.VarChar, 1, "vcBanZhi04");
                cmd.Parameters.Add("@vcAB00", SqlDbType.VarChar, 1, "vcAB00");//
                cmd.Parameters.Add("@vcAB01", SqlDbType.VarChar, 1, "vcAB01");//
                cmd.Parameters.Add("@vcAB02", SqlDbType.VarChar, 1, "vcAB02");//
                cmd.Parameters.Add("@vcAB03", SqlDbType.VarChar, 1, "vcAB03");//
                cmd.Parameters.Add("@vcAB04", SqlDbType.VarChar, 1, "vcAB04");//
                cmd.Parameters.Add("@vcCreater", SqlDbType.VarChar, 10, "vcCreater");
                cmd.Parameters.Add("@vcPlanMonth", SqlDbType.VarChar, 7, "vcPlanMonth");
                cmd.Parameters.Add("@vcQuantityPerContainer", SqlDbType.VarChar, 7, "vcQuantityPerContainer");
                apt = new SqlDataAdapter();
                apt.InsertCommand = cmd;
                apt.Update(dt);
            }
            return msg;
        }

        //获取对象周列名（生成打印数据用）
        public string TXTWeekCreatOrderNoColumns(string strMonth, string strWeek, string strPlant, ref int iWeekDays)
        {
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine(" select * from WeekPackPlanTbl where vcMonth='" + strMonth + "' ");
            DataTable dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            string _pPartsNo = dt.Rows[0]["vcPartsno"].ToString();
            string _pGC = string.Empty;//部署
            string _pZB = string.Empty;//组别
            TXTFindGCAndZB(_pPartsNo, strMonth, ref _pGC, ref _pZB);
            string _pCalendar1 = TXTFindProject1(_pGC, _pZB);//获取该品番工程1
                                                             //int _pWeekDays = 0;//对象周天数

            string _pPlanColumnName = TXTWeekColumnName(strPlant, strMonth, _pCalendar1, strWeek, ref iWeekDays).Replace("D", "vcD");//计划管理表对象周列名
            return _pPlanColumnName;
        }

        //获取LT逻辑（生成打印数据用）

        public void TXTWeekCreatOrderNoLT(string strMonth, string strWeek, string strPlant, ref int LT0, ref int LT1, ref int LT2, ref int LT3, ref int LT4)
        {
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine(" select * from WeekPackPlanTbl where vcMonth='" + strMonth + "' ");
            DataTable dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            string _pPartsNo = dt.Rows[0]["vcPartsno"].ToString();
            string _pGC = string.Empty;//部署
            string _pZB = string.Empty;//组别
            TXTFindGCAndZB(_pPartsNo, strMonth, ref _pGC, ref _pZB);
            DataTable dtLT = TXTFindLT(_pGC, _pZB);//一定是唯一的一行数据

            string _pLT0 = dtLT.Rows[0]["vcLT0"].ToString();//工程0：看板打印

            string _pLT1 = dtLT.Rows[0]["vcLT1"].ToString();//工程1：生产

            string _pLT2 = dtLT.Rows[0]["vcLT2"].ToString();//工程2：涂装

            string _pLT3 = dtLT.Rows[0]["vcLT3"].ToString();//工程3：预留P3
            string _pLT4 = dtLT.Rows[0]["vcLT4"].ToString();//工程4：包装

            if (_pLT0 != string.Empty)
            {
                LT0 = Convert.ToInt32(_pLT0);
            }
            else
            {
                LT0 = 0;
            }
            if (_pLT1 != string.Empty)
            {
                LT1 = Convert.ToInt32(_pLT1);
            }
            else
            {
                LT1 = 0;
            }
            if (_pLT2 != string.Empty)
            {
                LT2 = Convert.ToInt32(_pLT2);
            }
            else
            {
                LT2 = 0;
            }
            if (_pLT3 != string.Empty)
            {
                LT3 = Convert.ToInt32(_pLT3);
            }
            else
            {
                LT3 = 0;
            }
            if (_pLT4 != string.Empty)
            {
                LT4 = Convert.ToInt32(_pLT4);
            }
            else
            {
                LT4 = 0;
            }
        }

        //根据LT逻辑检索对应计划表SQL文
        //2019.2 增加flg标志，flg="1"看板打印计划
        public string TXTGetPlanSearch(string strMonth, string TableName, string strWeek, string strPlant, int iLT, string flg)
        {
            //数据准备
            string Year = strMonth.Split('-')[0];//年

            string Month = strMonth.Split('-')[1];//月

            string LastYear = (Convert.ToInt32(Year) - 1).ToString();//上一年

            string LastMonth = (Convert.ToInt32(Month) - 1).ToString("00");//上个月

            string strLastYear = LastYear + "-12";//上一年的对象月

            string strLastMonth = Year + "-" + LastMonth;//上个对象月


            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine(" select * from WeekPackPlanTbl where vcMonth='" + strMonth + "' ");
            DataTable dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            string _pPartsNo = dt.Rows[0]["vcPartsno"].ToString();
            string _pGC = string.Empty;//部署
            string _pZB = string.Empty;//组别
            TXTFindGCAndZB(_pPartsNo, strMonth, ref _pGC, ref _pZB);
            string _pCalendar1 = TXTFindProject1(_pGC, _pZB);//获取该品番工程1
            int _iWeekDays = 0;//当前周的天数
            string strCThisWeek = TXTWeekCreatOrderNoColumns(strMonth, strWeek, strPlant, ref _iWeekDays);//当前周所有稼动日对应的列名

            string[] sThisWeek = strCThisWeek.Split(',');//当前周所有稼动日对应的列名

            string[] sThisMonth = TXTFindMonthWorkDaysColumns(strPlant, Year, Month, _pCalendar1);//当前月所有稼动日对应的列名

            string[] NewWeekDays = new string[_iWeekDays];//存放最终结果

            string tmpColumns = string.Empty;
            string tm1 = string.Empty;
            string tm2 = string.Empty;
            int IndexThisWeek = sThisMonth.ToList().IndexOf(sThisWeek[0]);//当前周第一天在当前月所有稼动日中的索引
            if (iLT == 0)
            {
                tmpColumns = strCThisWeek;
            }
            else
            {
                //先验证当前LT逻辑是否跨月或跨年

                //int IndexThisWeek = sThisMonth.ToList().IndexOf(sThisWeek[0]);//当前周第一天在当前月所有稼动日中的索引
                if (IndexThisWeek - iLT < 0)//小于0则表示跨月或跨年
                {
                    //跨月跨年的都是第一周

                    if (Month == "01")//如果当前月是1月，上个月则是上一年12月

                    {
                        string[] sLastYear = TXTFindWeekLastColumn(strPlant, LastYear, "12", _pCalendar1);//上一年12月最后一周所有稼动日对应的列名

                        string strTMP1 = string.Join(",", sLastYear);//列表转逗号连接的字符串
                        string strTMPColumns1 = strTMP1 + "," + strCThisWeek;//去年12月稼动日逗号连接当前周

                        string[] strC1 = strTMPColumns1.Split(',');//形成新的字符串列表

                        for (int i = 0; i < _iWeekDays; i++)
                        {
                            NewWeekDays[i] = strC1[strC1.ToList().IndexOf(sThisWeek[i]) - iLT];
                        }
                    }
                    else
                    {
                        string[] sLastMonth = TXTFindWeekLastColumn(strPlant, Year, LastMonth, _pCalendar1);//上个月最后一周所有稼动日对应的列名

                        string strTMP2 = string.Join(",", sLastMonth);//列表转逗号连接的字符串
                        string strTMPColumns2 = strTMP2 + "," + strCThisWeek;//上月稼动日逗号连接当前周

                        string[] strC2 = strTMPColumns2.Split(',');//形成新的字符串列表

                        for (int i = 0; i < _iWeekDays; i++)
                        {
                            NewWeekDays[i] = strC2[strC2.ToList().IndexOf(sThisWeek[i]) - iLT];
                        }
                    }
                }
                else//不小于0则表示不跨月，用本月所有稼动日列名即可
                {
                    for (int i = 0; i < _iWeekDays; i++)
                    {
                        NewWeekDays[i] = sThisMonth[sThisMonth.ToList().IndexOf(sThisWeek[i]) - iLT];
                    }

                }
                tmpColumns = string.Join(",", NewWeekDays);
                //2019.2 对应跨月时间久业务，去除多余数据
                for (int n = 0; n < NewWeekDays.Length; n++)
                {
                    if (n >= iLT)
                    {
                        tm2 += NewWeekDays[n] + ",";//本月打印日期
                    }
                    else
                    {
                        tm1 += NewWeekDays[n] + ","; //上月打印日期
                    }
                }
                tm1 = tm1.TrimEnd(',');
                tm2 = tm2.TrimEnd(',');
                //tmpColumns = tm1 + tm2;



            }
            //for (int i = 1; i < 32; i++)
            //{
            //    if (i == 31)
            //        tmpColumns += "vcD" + i + "b,	vcD" + i + "y";
            //    else tmpColumns += "vcD" + i + "b,	vcD" + i + "y,";
            //}

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("    select Tall.vcMonth,Tall.vcPartsno,Tall.vcDock,Tall.vcCarType,Tall.sigTotal ,Tall.allTotal ,Tall.daysig ,ROW_NUMBER() over(partition by Tall.vcPartsno, Tall.vcDock,Tall.vcCartype order by Tall.vcMonth,Tall.daysig,Tall.vcPartsno,Tall.vcDock,Tall.vcCartype) as flag from (");
            sb.AppendLine("   select t1.*,t2.daysig from (");

            if (flg == "1")//如果是看板打印计划
            {
                if (flg == "1" && IndexThisWeek - iLT < 0)//如果是看板打印计划且跨月
                {
                    sb.AppendFormat("   select vcMonth, vcPartsno,vcDock,vcCartype,sigTotal , allTotal from {0}", TableName);
                    sb.AppendFormat("    unpivot( sigTotal for allTotal in( {0} ", tm1);
                    sb.AppendFormat("  )) P where LEN(sigTotal)>0 and montouch ='{0}'", strMonth);
                    sb.AppendLine("    union all ");
                }
                else //看板打印计划不跨月
                {
                }
            }

            else
            {
                sb.AppendFormat("   select vcMonth, vcPartsno,vcDock,vcCartype,sigTotal , allTotal from {0}", TableName);
                sb.AppendFormat("    unpivot( sigTotal for allTotal in( {0} ", tmpColumns);
                sb.AppendFormat("  )) P where LEN(sigTotal)>0 and montouch ='{0}'", strMonth);
                sb.AppendLine("    union all ");
            }

            sb.AppendFormat("    select vcMonth,vcPartsno,vcDock,vcCartype,sigTotal , allTotal from {0}  ", TableName);
            if (flg == "1" && IndexThisWeek - iLT < 0)
            {
                sb.AppendFormat("    unpivot( sigTotal for allTotal in( {0} ", tm2);
            }
            else
            {
                sb.AppendFormat("    unpivot( sigTotal for allTotal in( {0} ", tmpColumns);
            }
            sb.AppendFormat("  )) P where LEN(sigTotal)>0 and vcMonth ='{0}'", strMonth);
            sb.AppendLine("    ) t1");
            sb.AppendLine("    left join (");
            sb.AppendFormat("    select daysig , dayN from sPlanConst unpivot ( daysig for dayN in( {0}", tmpColumns);
            sb.AppendLine("     )) P ) t2 ");
            sb.AppendLine("     on t1.allTotal = t2.dayN");
            sb.AppendLine("    ) Tall ");
            sb.AppendLine("  left join dbo.tPartInfoMaster Tinfo on Tall.vcPartsno = Tinfo.vcPartsNo and Tall.vcDock = Tinfo.vcDock and Tall.vcCarType = Tinfo.vcCarFamilyCode  and   Tinfo.dTimeFrom<= '" + strMonth + "-01" + "' and Tinfo.dTimeTo >= '" + strMonth + "-01" + "' ");
            sb.AppendFormat("  where Tinfo.vcPartPlant ='{0}'", strPlant);
            sb.AppendLine(" order by vcMonth ,daysig,vcPartsno,vcDock,vcCartype");
            return sb.ToString();
        }
        #endregion

        #region 根据检索SQL文和数据源表格更新数据库（不包含删除操作）（作废） - 李兴旺

        /// <summary>
        /// 根据检索SQL文和数据源表格更新数据库（不包含删除操作）（作废）

        /// </summary>
        /// <param name="strSQL">检索SQL文</param>
        /// <param name="dtSource">数据源表格</param>
        public void TXTUpdateOtherPlan(string strSQL, DataTable dtSource)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(ComConnectionHelper.GetConnectionString());
            cmd.CommandTimeout = 0;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = strSQL;
            cmd.Connection.Open();
            cmd.Transaction = cmd.Connection.BeginTransaction();
            try
            {
                SqlDataAdapter apt = new SqlDataAdapter(cmd);
                DataTable dtupdate = new DataTable();
                apt.Fill(dtupdate);
                for (int k = 0; k < dtSource.Rows.Count; k++)
                {
                    DataRow dr = dtupdate.NewRow();
                    for (int i = 0; i < dtSource.Columns.Count; i++)
                    {
                        dr[i] = dtSource.Rows[k][i];
                    }
                    dtupdate.Rows.Add(dr);
                }
                SqlCommandBuilder sb = new SqlCommandBuilder(apt);
                apt.Update(dtupdate);
                apt.Dispose();
                sb.Dispose();
                cmd.Transaction.Commit();
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                cmd.Transaction.Rollback();
                cmd.Connection.Close();
                throw ex;
            }
        }
        #endregion

        #region 根据包装计划的数据行和LT规则，生成其他计划的数据行（原计划表初为空时） - 李兴旺

        /// <summary>
        /// 根据包装计划的数据行和LT规则，生成其他计划的数据行（原计划表初为空时）

        /// </summary>
        /// <param name="iLT">不同计划对应的LT规则</param>
        /// <param name="drFromRow">从包装计划获取的数据行</param>
        /// <param name="dtToTable">更新到的对应计划的数据表</param>
        /// <param name="iWeekDays">对象周天数</param>
        /// <param name="PlanColumnName">对象周对应的列名</param>
        /// <param name="strMonth">对象月</param>
        /// <param name="strCalendar1">工程1</param>
        /// <param name="strPlant">厂区</param>
        public void TXTMakePlanTableRow(int iLT, DataRow drFromRow, ref DataTable dtToTable, int iWeekDays, string[] PlanColumnName, string strMonth, string strCalendar1, string strPlant)
        {
            string Year = strMonth.Split('-')[0];//年

            string Month = strMonth.Split('-')[1];//月

            string LastYear = (Convert.ToInt32(Year) - 1).ToString();//上一年

            int iLastMonth = Convert.ToInt32(Month) - 1;//上个月

            string LastMonth = iLastMonth.ToString("00");
            string strLastYear = LastYear + "-12";//上一年的对象月

            string strLastMonth = Year + "-" + LastMonth;//上个对象月

            //先给dtToTable生成两个数据行

            DataRow _drThisMonth = dtToTable.NewRow();//这个月计划的行

            DataRow _drLastMonth = dtToTable.NewRow();//上个月计划的行

            //开始给其他计划数据行赋值

            _drThisMonth["vcMonth"] = drFromRow["vcMonth"];
            _drThisMonth["vcPartsno"] = drFromRow["vcPartsno"];
            _drThisMonth["vcDock"] = drFromRow["vcDock"];
            _drThisMonth["vcCarType"] = drFromRow["vcCarType"];
            _drThisMonth["vcProject1"] = drFromRow["vcProject1"];
            _drThisMonth["vcProjectName"] = drFromRow["vcProjectName"];
            _drThisMonth["vcMonTotal"] = drFromRow["vcMonTotal"];
            _drThisMonth["DADDTIME"] = DateTime.Now;
            _drThisMonth["DUPDTIME"] = DateTime.Now;
            _drThisMonth["CUPDUSER"] = drFromRow["CUPDUSER"];
            //给对应对象周的列赋值

            //值是0也没关系，该赋值的赋值

            for (int i = 0; i < iWeekDays; i++)
            {
                string[] _WorkDaysThisMonth = TXTFindMonthWorkDaysColumns(strPlant, Year, Month, strCalendar1);//当前月所有稼动日列名
                                                                                                               //int Index = dtToTable.Columns.IndexOf(PlanColumnName[i]);//对象周各列对应的索引号

                int IndexThisMonth = _WorkDaysThisMonth.ToList().IndexOf(PlanColumnName[i]);//对象周这一天在当前月所有稼动日中的索引
                                                                                            //（逻辑有误，没有顾及到非稼动日）编号0开始，第7列到第68列是计划内容，对应索引号减去LT规则，小于第一周的索引号的话，则表示要在上个月最后一周开始添加数据

                //string FirstWeekFirstDay = TXTFindWeekFirstColumn(strPlant, Year, Month, strCalendar1);
                //string LastWeekLastDay = string.Empty;
                //利用计划管理表之间的结构完全相同，获取索引号用ToTable的即可

                //int iFirstWeekFirstDay = dtToTable.Columns.IndexOf(FirstWeekFirstDay);
                //int iLastWeekLastDay = 0;
                //（逻辑有误，没有顾及到非稼动日）减去LT规则后小于第一周第一天的索引号，则需要写入上个月（还有可能上一年）的最后的周中
                //if (Index - iLT < iFirstWeekFirstDay)
                //该天在所有稼动日列表中的索引减去LT规则，小于0的一定在上个月

                if (IndexThisMonth - iLT < 0)
                {
                    _drLastMonth["vcPartsno"] = drFromRow["vcPartsno"];
                    _drLastMonth["vcDock"] = drFromRow["vcDock"];
                    _drLastMonth["vcCarType"] = drFromRow["vcCarType"];
                    _drLastMonth["vcProject1"] = drFromRow["vcProject1"];
                    _drLastMonth["vcProjectName"] = drFromRow["vcProjectName"];
                    if (Month == "01")//如果当前月是1月，上个月则是上一年12月

                    {
                        _drLastMonth["vcMonth"] = strLastYear;
                        //LastWeekLastDay = TXTFindWeekLastColumn(strPlant, LastYear, "12", strCalendar1);
                        //iLastWeekLastDay = dtToTable.Columns.IndexOf(LastWeekLastDay);
                        string[] _WorkDaysLastYear = TXTFindMonthWorkDaysColumns(strPlant, LastYear, "12", strCalendar1);
                        int _Lenth1 = _WorkDaysLastYear.Length;
                        //不能用两个字符串数组拼接的方法，因为有重复元素

                        //按照负数的具体值即可

                        _drLastMonth[_WorkDaysLastYear[_Lenth1 + (IndexThisMonth - iLT)]] = drFromRow[PlanColumnName[i]];
                    }
                    else//不是1月，只需上个月即可

                    {
                        _drLastMonth["vcMonth"] = strLastMonth;
                        //LastWeekLastDay = TXTFindWeekLastColumn(strPlant, Year, LastMonth, strCalendar1);
                        //iLastWeekLastDay = dtToTable.Columns.IndexOf(LastWeekLastDay);
                        string[] _WorkDaysLastMonth = TXTFindMonthWorkDaysColumns(strPlant, Year, LastMonth, strCalendar1);
                        int _Lenth2 = _WorkDaysLastMonth.Length;
                        _drLastMonth[_WorkDaysLastMonth[_Lenth2 + (IndexThisMonth - iLT)]] = drFromRow[PlanColumnName[i]];
                    }
                    _drLastMonth["montouch"] = strMonth;
                    _drLastMonth["DADDTIME"] = DateTime.Now;
                    _drLastMonth["DUPDTIME"] = DateTime.Now;
                    _drLastMonth["CUPDUSER"] = drFromRow["CUPDUSER"];
                }
                else//不在上个月，则按照LT规则和对应的周稼动日填入数据（可能会跨周）

                {
                    _drThisMonth[_WorkDaysThisMonth[IndexThisMonth - iLT]] = drFromRow[PlanColumnName[i]];
                }
            }
            if (TXTIsNullRow(_drThisMonth) == true)
            {
                //有数据则插入
                dtToTable.Rows.Add(_drThisMonth);
            }
            if (TXTIsNullRow(_drLastMonth) == true)
            {
                //有数据则插入
                dtToTable.Rows.Add(_drLastMonth);
            }
        }
        #endregion

        #region 根据包装计划的数据行和LT规则，生成其他计划的数据行（原计划表不为空时） - 李兴旺

        /// <summary>
        /// 根据包装计划的数据行和LT规则，生成其他计划的数据行（原计划表不为空时）

        /// </summary>
        /// <param name="iLT">不同计划对应的LT规则</param>
        /// <param name="drFromRow">从包装计划获取的数据行</param>
        /// <param name="dtToRow">更新到的对应计划的数据行</param>
        /// <param name="iWeekDays">对象周天数</param>
        /// <param name="PlanColumnName">对象周对应的列名</param>
        /// <param name="strMonth">对象月</param>
        /// <param name="strCalendar1">工程1</param>
        /// <param name="strPlant">厂区</param>
        public void TXTMakeRowInPlanTable(int iLT, DataRow drFromRow, ref DataRow drToRow, int iWeekDays, string[] PlanColumnName, string strMonth, string strCalendar1, string strPlant)
        {
            //原计划表不为空，那当前周肯定不是第一周了，不会有转上个月甚至上一年的问题。

            //有些数值不用更新，只需更新总数、UPDTime、对应周数值

            string Year = strMonth.Split('-')[0];//年

            string Month = strMonth.Split('-')[1];//月

            //更新一部分数值

            drToRow["vcMonTotal"] = drFromRow["vcMonTotal"];
            drToRow["DUPDTIME"] = DateTime.Now;
            //给对应对象周的列赋值

            //值是0也没关系，该赋值的赋值

            for (int i = 0; i < iWeekDays; i++)
            {
                string[] _WorkDaysThisMonth = TXTFindMonthWorkDaysColumns(strPlant, Year, Month, strCalendar1);//当前月所有稼动日列名
                int IndexThisMonth = _WorkDaysThisMonth.ToList().IndexOf(PlanColumnName[i]);//对象周这一天在当前月所有稼动日中的索引
                if (IndexThisMonth - iLT >= 0)
                {
                    drToRow[_WorkDaysThisMonth[IndexThisMonth - iLT]] = drFromRow[PlanColumnName[i]];
                }
            }
        }
        #endregion

        #region 计算周度包装计划里当前的总数 - 李兴旺

        /// <summary>
        /// 计算周度包装计划里当前的总数
        /// </summary>
        /// <param name="dr">周度包装计划当前更新的数据行</param>
        /// <returns>总数</returns>
        public int TXTWeekPackPlanMonTotal(DataRow dr)
        {
            int iTotal = 0;
            //只计算1~31号部分

            for (int i = 7; i < dr.ItemArray.Length - 4; i++)
            {
                if (dr[i].ToString() != string.Empty)
                {
                    iTotal += Convert.ToInt32(dr[i].ToString());
                }
            }
            return iTotal;
        }
        #endregion

        #region 判断数据行是否有数据 - 李兴旺

        /// <summary>
        /// 判断数据行是否有数据
        /// </summary>
        /// <param name="dr">数据行</param>
        /// <returns>true为有数据，false为没数据</returns>
        public bool TXTIsNullRow(DataRow dr)
        {
            if (dr != null)
            {
                for (int i = 0; i < dr.ItemArray.Length; i++)
                {
                    if (dr[i].ToString() != string.Empty)
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 查找对象月第1周第一天的列名（没用上） - 李兴旺

        /// <summary>
        /// 查找对象月第1周第一天的列名
        /// </summary>
        /// <param name="vcPlant">厂区</param>
        /// <param name="vcYear">年</param>
        /// <param name="vcMonth">月</param>
        /// <param name="strProject1">工程1</param>
        /// <returns>对象月第1周第一天的列名</returns>
        public string TXTFindWeekFirstColumn(string vcPlant, string vcYear, string vcMonth, string strProject1)
        {
            string vcGC = strProject1.Split('-')[0];
            string vcZB = strProject1.Split('-')[1];
            string ColumnName = string.Empty;
            int Index = 0;
            DataTable dt = new DataTable();//查找该品番对应的周稼动日历用
            StringBuilder strSQL = new StringBuilder();
            //查找该品番对应的周稼动日历

            //vcPlant='" + vcPlant + "' and 
            strSQL.AppendLine(" select * from WeekCalendarTbl where vcPlant='" + vcPlant + "' and vcYear='" + vcYear + "' and vcMonth='" + vcMonth + "' and vcGC='" + vcGC + "' and vcZB='" + vcZB + "' ");
            //确定唯一的一行数据，包括iAutoID
            dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            for (int i = 6; i < dt.Columns.Count - 2; i++)
            {
                if (dt.Rows[0][i].ToString() != string.Empty)
                {
                    if (dt.Rows[0][i].ToString() == "1")
                    {
                        Index = i;//获取第一个值是1的那列的索引
                        break;
                    }
                }
            }
            ColumnName = dt.Columns[Index].ColumnName;//根据索引找列名

            return ColumnName;
        }
        #endregion

        #region 查找对象月最后一周的列名 - 李兴旺

        /// <summary>
        /// 查找对象月最后一周的列名
        /// </summary>
        /// <param name="vcPlant">厂区</param>
        /// <param name="vcYear">年</param>
        /// <param name="vcMonth">月</param>
        /// <param name="strProject1">工程1</param>
        /// <returns>最后一周列名列表</returns>
        public string[] TXTFindWeekLastColumn(string vcPlant, string vcYear, string vcMonth, string strProject1)
        {
            string strMonth = vcYear + "-" + vcMonth;
            string vcGC = strProject1.Split('-')[0];
            string vcZB = strProject1.Split('-')[1];
            string ColumnName = string.Empty;
            int iWeekDays = 0;
            DataTable dt = new DataTable();//查找该品番对应的周稼动日历用
            StringBuilder strSQL = new StringBuilder();
            //查找该品番对应的周稼动日历

            //vcPlant='" + vcPlant + "' and 
            strSQL.AppendLine(" select * from WeekCalendarTbl where vcPlant='" + vcPlant + "' and vcYear='" + vcYear + "' and vcMonth='" + vcMonth + "' and vcGC='" + vcGC + "' and vcZB='" + vcZB + "' ");
            //确定唯一的一行数据，包括iAutoID
            dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            string LastWeek = dt.Rows[0]["vcWeekCount"].ToString();//本月一共有几周
            ColumnName = TXTWeekColumnName(vcPlant, strMonth, strProject1, LastWeek, ref iWeekDays).Replace("D", "vcD");
            return ColumnName.Split(',');
        }
        #endregion

        #region 根据部署和组别查找该品番对应的LT规则 - 李兴旺

        /// <summary>
        /// 根据部署和组别查找该品番对应的LT规则
        /// </summary>
        /// <param name="strGC">部署</param>
        /// <param name="strZB">组别</param>
        /// <returns>LT0~4的结果数据表</returns>
        public DataTable TXTFindLT(string strGC, string strZB)
        {
            DataTable dt = new DataTable();
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendLine(" SELECT vcLT0,vcLT1,vcLT2,vcLT3,vcLT4 FROM ProRuleMst where vcPorType='" + strGC + "' and vcZB='" + strZB + "' ");
            dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            return dt;
        }
        #endregion

        #region 获取对象月所有稼动日的列名 - 李兴旺

        /// <summary>
        /// 获取对象月所有稼动日的列名

        /// </summary>
        /// <param name="vcPlant">厂区</param>
        /// <param name="vcYear">年</param>
        /// <param name="vcMonth">月</param>
        /// <param name="strProject1">工程1</param>
        /// <returns>列名列表</returns>
        public string[] TXTFindMonthWorkDaysColumns(string vcPlant, string vcYear, string vcMonth, string strProject1)
        {
            string strWeekColumnName = string.Empty;
            string strColumnsNameTMP = string.Empty;
            string vcGC = strProject1.Split('-')[0];
            string vcZB = strProject1.Split('-')[1];
            DataTable dt = new DataTable();//查找该品番对应的周稼动日历用
            StringBuilder strSQL = new StringBuilder();
            //查找该品番对应的周稼动日历

            //vcPlant='" + vcPlant + "' and 
            strSQL.AppendLine(" select * from WeekCalendarTbl where vcPlant='" + vcPlant + "' and vcYear='" + vcYear + "' and vcMonth='" + vcMonth + "' and vcGC='" + vcGC + "' and vcZB='" + vcZB + "' ");
            //确定唯一的一行数据，包括iAutoID
            dt = excute.ExcuteSqlWithSelectToDT(strSQL.ToString());
            for (int i = 6; i < dt.Columns.Count - 2; i++)
            {
                if (dt.Rows[0][i].ToString() != string.Empty)//1号~31号中不为空的即为稼动日

                {
                    strColumnsNameTMP += dt.Columns[i].ColumnName + ",";
                }
            }
            strWeekColumnName = strColumnsNameTMP.Substring(0, strColumnsNameTMP.Length - 1).Replace("D", "vcD");
            string[] MonthWorkDaysColumns = strWeekColumnName.Split(',');
            return MonthWorkDaysColumns;
        }
        #endregion

        #region 数字与周数文字对应关系 - 李兴旺

        /// <summary>
        /// 数字与周数文字对应关系

        /// </summary>
        /// <param name="iWeekNum">周数数字</param>
        /// <returns>周数文字</returns>
        private string NumberToText(string iWeekNum)
        {
            string strWeekNum = string.Empty;
            switch (iWeekNum)
            {
                case "1": strWeekNum = "第一周"; break;
                case "2": strWeekNum = "第二周"; break;
                case "3": strWeekNum = "第三周"; break;
                case "4": strWeekNum = "第四周"; break;
                case "5": strWeekNum = "第五周"; break;
                default: strWeekNum = ""; break;
            }
            return strWeekNum;
        }
        #endregion

        #region 检测字符串是否都是数字 - 李兴旺

        /// <summary>
        /// 检测字符串是否都是数字
        /// </summary>
        /// <param name="str">被测字符串</param>
        /// <returns>TRUE为真，都是数字或空字符串；FALSE为假，有不是数字的字符</returns>
        public bool TXTIsInt(string str)
        {
            int iResult = -1;
            //空字符串除外
            if (str != string.Empty)
            {
                try
                {
                    iResult = Convert.ToInt32(str);
                    return true;
                }
                catch
                {
                    //不能转换为数字，则抛出异常

                    return false;
                }
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region 判断品番在当前对象月的月度包装计划中是否有数据（没用上） - 李兴旺

        /// <summary>
        /// 判断品番在当前对象月的月度包装计划中是否有数据

        /// </summary>
        /// <param name="strMonth">对象月</param>
        /// <param name="vcCalendar1">工程1</param>
        /// <param name="strPartsNo">品番</param>
        /// <returns>有数据为TRUE，没数据为FALSE</returns>
        public bool TXTCheckMonthPlanExist(string strMonth, string vcCalendar1, string strPartsNo)
        {
            string SQLTMP2 = "select * from MonthPackPlanTbl where vcMonth='" + strMonth + "' and vcPartsno='" + strPartsNo + "' and vcProject1='" + vcCalendar1 + "' and (montouch='' or montouch is null) ";
            DataTable dtMonthPackPlan = excute.ExcuteSqlWithSelectToDT(SQLTMP2);
            if (dtMonthPackPlan.Rows.Count > 0)//大于0，有数据
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 初始化工厂DDL - 李兴旺整理

        /// <summary>
        /// 初始化工厂DDL
        /// </summary>
        /// <returns></returns>
        public DataTable bindplant()
        {
            return fs_da.bindplant();
        }
        #endregion

        #region 求最小公倍数
        /// <summary>
        /// 求最小公倍数
        /// </summary>
        /// <param name="m">第一个数</param>
        /// <param name="n">第二个数</param>
        /// <returns>最小公倍数</returns>
        public int getMinbeishu(int m, int n)
        {
            int i = 0;
            for (i = m < n ? m : n; m % i != 0 || n % i != 0; i--) ;
            return i = m * n / i;
        }
        #endregion
    }
}