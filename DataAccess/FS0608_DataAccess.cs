using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Common;
using System.Data.SqlClient;
using System.Collections;

namespace DataAccess
{
    public class FS0608_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public string save(List<string> re, string varDxny, string vcFZGC, decimal TOTALWORKDAYS, string strUserId)
        {
            try
            {
                #region 初次保存
                string msg = "";
                int days = 0;
                SqlParameter[] parameters = {
                    new SqlParameter("@varDxny", SqlDbType.VarChar),
                    new SqlParameter("@vcFZGC", SqlDbType.VarChar),
                    new SqlParameter("@TOTALWORKDAYS", SqlDbType.Decimal),
                    new SqlParameter("@DADDTIME", SqlDbType.DateTime),
                    new SqlParameter("@CUPDUSER", SqlDbType.VarChar),
                };
                parameters[0].Value = varDxny;
                parameters[1].Value = vcFZGC;
                parameters[2].Value = TOTALWORKDAYS;
                parameters[3].Value = DateTime.Now;
                parameters[4].Value = strUserId;
                StringBuilder strSql = new StringBuilder();

                //先删除
                strSql.AppendLine(" DELETE TCalendar_PingZhun_Wai ");
                strSql.AppendLine(" WHERE TARGETMONTH=@varDxny and vcFZGC='" + vcFZGC + "'; ");

                //再新增
                strSql.AppendLine(" INSERT INTO TCalendar_PingZhun_Wai( ");
                strSql.AppendLine(" vcFZGC, ");
                strSql.AppendLine(" TARGETMONTH, ");
                strSql.AppendLine(" TARGETDAY1, ");
                strSql.AppendLine(" TARGETDAY2, ");
                strSql.AppendLine(" TARGETDAY3, ");
                strSql.AppendLine(" TARGETDAY4, ");
                strSql.AppendLine(" TARGETDAY5, ");
                strSql.AppendLine(" TARGETDAY6, ");
                strSql.AppendLine(" TARGETDAY7, ");
                strSql.AppendLine(" TARGETDAY8, ");
                strSql.AppendLine(" TARGETDAY9, ");
                strSql.AppendLine(" TARGETDAY10, ");
                strSql.AppendLine(" TARGETDAY11, ");
                strSql.AppendLine(" TARGETDAY12, ");
                strSql.AppendLine(" TARGETDAY13, ");
                strSql.AppendLine(" TARGETDAY14, ");
                strSql.AppendLine(" TARGETDAY15, ");
                strSql.AppendLine(" TARGETDAY16, ");
                strSql.AppendLine(" TARGETDAY17, ");
                strSql.AppendLine(" TARGETDAY18, ");
                strSql.AppendLine(" TARGETDAY19, ");
                strSql.AppendLine(" TARGETDAY20, ");
                strSql.AppendLine(" TARGETDAY21, ");
                strSql.AppendLine(" TARGETDAY22, ");
                strSql.AppendLine(" TARGETDAY23, ");
                strSql.AppendLine(" TARGETDAY24, ");
                strSql.AppendLine(" TARGETDAY25, ");
                strSql.AppendLine(" TARGETDAY26, ");
                strSql.AppendLine(" TARGETDAY27, ");
                strSql.AppendLine(" TARGETDAY28, ");
                strSql.AppendLine(" TARGETDAY29, ");
                strSql.AppendLine(" TARGETDAY30, ");
                strSql.AppendLine(" TARGETDAY31, ");
                strSql.AppendLine(" TOTALWORKDAYS, ");
                strSql.AppendLine(" DADDTIME, ");
                strSql.AppendLine(" CUPDUSER) ");
                strSql.AppendLine(" VALUES( ");
                strSql.AppendLine(" @vcFZGC, ");
                strSql.AppendLine(" @varDxny, ");
                for (int i = 0; i < 31; i++)
                {
                    if (re.Count > i && !string.IsNullOrEmpty(re[i]))
                    {
                        strSql.AppendLine("'" + re[i] + "',");
                        if (re[i].ToString() != "0")
                            days++;
                    }
                    else
                    {
                        try
                        {
                            DateTime t = Convert.ToDateTime(varDxny.Substring(0, 4) + "-" + varDxny.Substring(4, 2) + "-" + (i + 1).ToString());
                            return "请进行分配！";
                        }
                        catch
                        {
                            strSql.AppendLine("NULL,");
                        }
                    }
                }
                strSql.AppendLine(" @TOTALWORKDAYS, ");
                strSql.AppendLine(" @DADDTIME, ");
                strSql.AppendLine(" @CUPDUSER); ");
                if (days < 3)
                    return "最少必须维护3天";
                excute.ExcuteSqlWithStringOper(strSql.ToString(), parameters);
                #endregion

                #region 重新平准化 不对弃用
                //DataTable ckdt = excute.ExcuteSqlWithSelectToDT("select * from TCalendar_PingZhun_Wai where vcFZGC='" + vcFZGC + "' and TARGETMONTH='" + varDxny + "'");
                //#region 准备判断数据
                //double[] weeks = { 0, 0, 0, 0 };
                //for (int i = 0; i < 31; i++)
                //{
                //    if (ckdt.Rows[0][i + 3].ToString() == "1")
                //    {
                //        weeks[0] = weeks[0] + 1;
                //    }
                //    else if (ckdt.Rows[0][i + 3].ToString() == "1*")
                //    {
                //        weeks[0] = weeks[0] + 0.5;
                //    }
                //    else if (ckdt.Rows[0][i + 3].ToString() == "2")
                //    {
                //        weeks[1] = weeks[1] + 1;
                //    }
                //    else if (ckdt.Rows[0][i + 3].ToString() == "2*")
                //    {
                //        weeks[1] = weeks[1] + 0.5;
                //    }
                //    else if (ckdt.Rows[0][i + 3].ToString() == "3")
                //    {
                //        weeks[2] = weeks[2] + 1;
                //    }
                //    else if (ckdt.Rows[0][i].ToString() == "3*")
                //    {
                //        weeks[2] = weeks[2] + 0.5;
                //    }
                //    else if (ckdt.Rows[0][i + 3].ToString() == "4")
                //    {
                //        weeks[3] = weeks[3] + 1;
                //    }
                //    else if (ckdt.Rows[0][i + 3].ToString() == "4*")
                //    {
                //        weeks[3] = weeks[3] + 0.5;
                //    }
                //}
                //#endregion
                //Array.Sort(weeks);
                //if (weeks[3] - weeks[0] > 1) //判断存在差值大于1的，则重新分配
                //{
                //    double totals = 0;
                //    double avg_int = 0;
                //    double avg_left = 0;
                //    for (int i = 1; i <= 31; i++)
                //    {
                //        if (ckdt.Rows[0]["TARGETDAY" + i.ToString()].ToString().Contains('*'))
                //            totals = totals + 0.5;
                //        else if (!ckdt.Rows[0]["TARGETDAY" + i.ToString()].ToString().Contains('0'))
                //        {
                //            if (i != 31)
                //            {
                //                totals = totals + 1;
                //            }
                //        }
                //    }
                //    avg_int = Math.Floor(totals / 4);
                //    avg_left = totals % 4;

                //    DataTable dt = new DataTable();
                //    dt.Columns.Add("c1");

                //    for (int i = 1; i < 5; i++)
                //    {
                //        for (int k = 0; k <= avg_int; k++)
                //        {
                //            DataRow r = dt.NewRow();
                //            if (k < avg_int)
                //                r[0] = i.ToString();
                //            else
                //            {
                //                if (avg_left == 0.5)
                //                {
                //                    r[0] = i.ToString() + "*";
                //                    avg_left = 0;
                //                }
                //                else if (avg_left > 0.5)
                //                {
                //                    r[0] = i.ToString();
                //                    avg_left--;
                //                }
                //                else
                //                {
                //                    break;
                //                }
                //            }
                //            dt.Rows.Add(r);
                //        }
                //    }

                //    for (int i = 1; i <= 31; i++)
                //    {
                //        if (!ckdt.Rows[0]["TARGETDAY" + i.ToString()].ToString().Contains('0'))
                //        {
                //            if (dt.Rows.Count > 0)
                //            {
                //                ckdt.Rows[0]["TARGETDAY" + i.ToString()] = dt.Rows[0][0];
                //                dt.Rows[0].Delete();
                //            }
                //            else
                //            {
                //                ckdt.Rows[0]["TARGETDAY" + i.ToString()] = "0";
                //            }
                //        }
                //    }
                //    string sql = "update TCalendar_PingZhun_Wai set ";
                //    for (int y = 1; y <= 31; y++)
                //    {
                //        sql += "TARGETDAY" + y.ToString() + "='" + ckdt.Rows[0]["TARGETDAY" + y.ToString()].ToString() + "',";
                //    }
                //    sql = sql.TrimEnd(',');
                //    sql += " where vcFZGC='" + vcFZGC + "' and TARGETMONTH='" + varDxny + "'";
                //    excute.ExecuteSQLNoQuery(sql);
                //}
                #endregion


                #region 重新分配1
                DataTable ckdt = excute.ExcuteSqlWithSelectToDT("select * from TCalendar_PingZhun_Wai where vcFZGC='" + vcFZGC + "' and TARGETMONTH='" + varDxny + "'");
                double totals = 0;
                double avg_int = 0;
                double avg_left = 0;

                #region 准备判断数据
                double[] weeks = { 0, 0, 0, 0 };
                double[] weeks_pre = { 0, 0, 0, 0 };
                for (int i = 0; i < 31; i++)
                {
                    if (ckdt.Rows[0][i + 3].ToString() == "1")
                        weeks[0] = weeks[0] + 2;
                    else if (ckdt.Rows[0][i + 3].ToString() == "1*")
                        weeks[0] = weeks[0] + 1;
                    else if (ckdt.Rows[0][i + 3].ToString() == "2")
                        weeks[1] = weeks[1] + 2;
                    else if (ckdt.Rows[0][i + 3].ToString() == "2*")
                        weeks[1] = weeks[1] + 1;
                    else if (ckdt.Rows[0][i + 3].ToString() == "3")
                        weeks[2] = weeks[2] + 2;
                    else if (ckdt.Rows[0][i + 3].ToString() == "3*")
                        weeks[2] = weeks[2] + 1;
                    else if (ckdt.Rows[0][i + 3].ToString() == "4")
                        weeks[3] = weeks[3] + 2;
                    else if (ckdt.Rows[0][i + 3].ToString() == "4*")
                        weeks[3] = weeks[3] + 1;
                }
                for (int i = 0; i < 4; i++)
                {
                    weeks_pre[i] = weeks[i];
                }
                Array.Sort(weeks);
                if (weeks[3] - weeks[0] >= 2) //判断存在差值大于2值的，则重新分配
                {
                    for (int i = 1; i <= 31; i++)
                    {
                        if (ckdt.Rows[0]["TARGETDAY" + i.ToString()].ToString().Contains('*'))
                            totals = totals + 1;
                        else if (!ckdt.Rows[0]["TARGETDAY" + i.ToString()].ToString().Contains('0'))
                            totals = totals + 2;
                    }
                    avg_int = Math.Floor(totals / 4);
                    avg_left = totals % 4;
                    double[] avg_i = { 0, 0, 0, 0 };
                    switch (avg_left)
                    {
                        case 0:
                            avg_i[0] = avg_int;
                            avg_i[1] = avg_int + avg_i[0];
                            avg_i[2] = avg_int + avg_i[1];
                            avg_i[3] = avg_int + avg_i[2];
                            break;
                        case 1:
                            avg_i[0] = avg_int + 1;
                            avg_i[1] = avg_int + avg_i[0];
                            avg_i[2] = avg_int + avg_i[1];
                            avg_i[3] = avg_int + avg_i[2];
                            break;
                        case 2:
                            avg_i[0] = avg_int + 1;
                            avg_i[1] = avg_int + avg_i[0];
                            avg_i[2] = avg_int + avg_i[1] + 1;
                            avg_i[3] = avg_int + avg_i[2];
                            break;
                        case 3:
                            avg_i[0] = avg_int + 1;
                            avg_i[1] = avg_int + avg_i[0] + 1;
                            avg_i[2] = avg_int + avg_i[1] + 1;
                            avg_i[3] = avg_int + avg_i[2];
                            break;
                    }
                    List<int> re_tm = new List<int>();
                    List<string> re_fp = new List<string>();
                    for (int i = 0; i < re.Count; i++)
                    {
                        if (re[i] != "0")
                        {
                            re_tm.Add(1);
                        }
                        else
                        {
                            re_tm.Add(0);
                        }
                        re_fp.Add("0");
                    }
                    int flag = 0;

                    for (int i = 0; i < re.Count; i++)
                    {
                        if (re[i] != "0")
                        {
                            if (!re[i].Contains("*"))
                            {
                                flag = flag + re_tm[i] + 1;
                                if (flag <= avg_i[0])
                                {
                                    re_fp[i] = "1";
                                }
                                else if (flag <= avg_i[1] && flag > avg_i[0])
                                {
                                    if (weeks[0] != 0)
                                    {
                                        if (flag - avg_i[0] == 1 && flag - weeks[0] <= 2)
                                        {
                                            re_fp[i] = "1";
                                        }
                                        else
                                        {
                                            re_fp[i] = "2";
                                        }
                                    }
                                    else
                                    {
                                        re_fp[i] = re[i];
                                    }
                                }
                                else if (flag <= avg_i[2] && flag > avg_i[1])
                                {
                                    if (weeks[0] != 0)
                                    {
                                        if (flag - avg_i[1] == 1 && flag - (weeks[0] + avg_int) <= 2)
                                        {
                                            re_fp[i] = "2";
                                        }
                                        else
                                        {
                                            re_fp[i] = "3";
                                        }
                                    }
                                    else
                                    {
                                        re_fp[i] = re[i];
                                    }
                                }
                                else if (flag <= avg_i[3] && flag > avg_i[2])
                                {
                                    if (weeks[0] != 0)
                                    {
                                        if (flag - avg_i[2] == 1 && flag - (weeks[0] + avg_int * 2) <= 2)
                                        {
                                            re_fp[i] = "3";
                                        }
                                        else
                                        {
                                            re_fp[i] = "4";
                                        }
                                    }
                                    else
                                    {
                                        re_fp[i] = re[i];
                                    }
                                }
                            }
                            else
                            {
                                flag = flag + re_tm[i];
                                if (flag <= avg_i[0])
                                {
                                    if (weeks[0] != 0)
                                    {
                                        re_fp[i] = "1*";
                                    }
                                    else
                                    {
                                        re_fp[i] = re[i];
                                    }
                                }
                                else if (flag <= avg_i[1] && flag > avg_i[0])
                                {
                                    if (weeks[0] != 0)
                                    {
                                        re_fp[i] = "2*";
                                    }
                                    else
                                    {
                                        re_fp[i] = re[i];
                                    }
                                }
                                else if (flag <= avg_i[2] && flag > avg_i[1])
                                {
                                    if (weeks[0] != 0)
                                    {
                                        re_fp[i] = "3*";
                                    }
                                    else
                                    {
                                        re_fp[i] = re[i];
                                    }
                                }
                                else if (flag <= avg_i[3] && flag > avg_i[2])
                                {
                                    if (weeks[0] != 0)
                                    {
                                        re_fp[i] = "4*";
                                    }
                                    else
                                    {
                                        re_fp[i] = re[i];
                                    }
                                }
                            }
                        }
                    }

                    string sql = "update TCalendar_PingZhun_Wai set ";
                    for (int i = 0; i < 31; i++)
                    {
                        sql += "TARGETDAY" + (i + 1).ToString() + "='" + re_fp[i] + "',";
                    }
                    sql = sql.TrimEnd(',');
                    sql += " where vcFZGC='" + vcFZGC + "' and TARGETMONTH='" + varDxny + "'";
                    excute.ExecuteSQLNoQuery(sql);
                }
                #endregion
                #endregion

                //#region 重新分配2（上半月与下半月不能大于2值）
                //DataTable dt2 = excute.ExcuteSqlWithSelectToDT("select * from TCalendar_PingZhun_Wai where vcFZGC='" + vcFZGC + "' and TARGETMONTH='" + varDxny + "'");

                //int mon_up = 0; //上半月
                //int mon_down = 0; //下半月
                //for (int i = 0; i < 31; i++)
                //{
                //    if (dt2.Rows[0][i + 3].ToString() == "1*" || dt2.Rows[0][i + 3].ToString() == "2*")
                //    {
                //        mon_up = mon_up + 1;
                //    }
                //    else if (dt2.Rows[0][i + 3].ToString() == "1" || dt2.Rows[0][i + 3].ToString() == "2")
                //    {
                //        mon_up = mon_up + 2;
                //    }
                //    else if (dt2.Rows[0][i + 3].ToString() == "3*" || dt2.Rows[0][i + 3].ToString() == "4*")
                //    {
                //        mon_down = mon_down + 1;
                //    }
                //    else if (dt2.Rows[0][i + 3].ToString() == "3" || dt2.Rows[0][i + 3].ToString() == "4")
                //    {
                //        mon_down = mon_down + 2;
                //    }
                //}
                //if (mon_down - mon_up > 2)
                //{

                //}
                //#endregion

                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 检索数据
        /// </summary>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        public DataTable Search(DateTime varDxny, string vcFZGC)
        {
            try
            {
                System.Data.SqlClient.SqlParameter[] parameters = {
                    new SqlParameter("@vcFZGC", SqlDbType.VarChar),
                };
                parameters[0].Value = vcFZGC;

                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("  SELECT ");
                strSql.AppendLine(" TARGETDAY1, ");
                strSql.AppendLine(" TARGETDAY2, ");
                strSql.AppendLine(" TARGETDAY3, ");
                strSql.AppendLine(" TARGETDAY4, ");
                strSql.AppendLine(" TARGETDAY5, ");
                strSql.AppendLine(" TARGETDAY6, ");
                strSql.AppendLine(" TARGETDAY7, ");
                strSql.AppendLine(" TARGETDAY8, ");
                strSql.AppendLine(" TARGETDAY9, ");
                strSql.AppendLine(" TARGETDAY10, ");
                strSql.AppendLine(" TARGETDAY11, ");
                strSql.AppendLine(" TARGETDAY12, ");
                strSql.AppendLine(" TARGETDAY13, ");
                strSql.AppendLine(" TARGETDAY14, ");
                strSql.AppendLine(" TARGETDAY15, ");
                strSql.AppendLine(" TARGETDAY16, ");
                strSql.AppendLine(" TARGETDAY17, ");
                strSql.AppendLine(" TARGETDAY18, ");
                strSql.AppendLine(" TARGETDAY19, ");
                strSql.AppendLine(" TARGETDAY20, ");
                strSql.AppendLine(" TARGETDAY21, ");
                strSql.AppendLine(" TARGETDAY22, ");
                strSql.AppendLine(" TARGETDAY23, ");
                strSql.AppendLine(" TARGETDAY24, ");
                strSql.AppendLine(" TARGETDAY25, ");
                strSql.AppendLine(" TARGETDAY26, ");
                strSql.AppendLine(" TARGETDAY27, ");
                strSql.AppendLine(" TARGETDAY28, ");
                strSql.AppendLine(" TARGETDAY29, ");
                strSql.AppendLine(" TARGETDAY30, ");
                strSql.AppendLine(" TARGETDAY31, ");
                strSql.AppendLine(" TARGETMONTH, ");
                strSql.AppendLine(" TOTALWORKDAYS ");
                strSql.AppendLine(" FROM TCalendar_PingZhun_Wai ");
                strSql.AppendLine(" WHERE vcFZGC=@vcFZGC ");
                strSql.AppendLine(string.Format(" AND TARGETMONTH in ('{0}','{1}','{2}')", varDxny.ToString("yyyyMM"), varDxny.AddMonths(1).ToString("yyyyMM"), varDxny.AddMonths(2).ToString("yyyyMM")));
                strSql.AppendLine(" ORDER BY TARGETMONTH; ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString(), parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable SearchData(DateTime varDxny, string vcFZGC)
        {
            try
            {
                System.Data.SqlClient.SqlParameter[] parameters = {
                    new SqlParameter("@vcFZGC", SqlDbType.VarChar),
                };
                parameters[0].Value = vcFZGC;
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("  SELECT ");
                strSql.AppendLine(" TARGETDAY1, ");
                strSql.AppendLine(" TARGETDAY2, ");
                strSql.AppendLine(" TARGETDAY3, ");
                strSql.AppendLine(" TARGETDAY4, ");
                strSql.AppendLine(" TARGETDAY5, ");
                strSql.AppendLine(" TARGETDAY6, ");
                strSql.AppendLine(" TARGETDAY7, ");
                strSql.AppendLine(" TARGETDAY8, ");
                strSql.AppendLine(" TARGETDAY9, ");
                strSql.AppendLine(" TARGETDAY10, ");
                strSql.AppendLine(" TARGETDAY11, ");
                strSql.AppendLine(" TARGETDAY12, ");
                strSql.AppendLine(" TARGETDAY13, ");
                strSql.AppendLine(" TARGETDAY14, ");
                strSql.AppendLine(" TARGETDAY15, ");
                strSql.AppendLine(" TARGETDAY16, ");
                strSql.AppendLine(" TARGETDAY17, ");
                strSql.AppendLine(" TARGETDAY18, ");
                strSql.AppendLine(" TARGETDAY19, ");
                strSql.AppendLine(" TARGETDAY20, ");
                strSql.AppendLine(" TARGETDAY21, ");
                strSql.AppendLine(" TARGETDAY22, ");
                strSql.AppendLine(" TARGETDAY23, ");
                strSql.AppendLine(" TARGETDAY24, ");
                strSql.AppendLine(" TARGETDAY25, ");
                strSql.AppendLine(" TARGETDAY26, ");
                strSql.AppendLine(" TARGETDAY27, ");
                strSql.AppendLine(" TARGETDAY28, ");
                strSql.AppendLine(" TARGETDAY29, ");
                strSql.AppendLine(" TARGETDAY30, ");
                strSql.AppendLine(" TARGETDAY31, ");
                strSql.AppendLine(" TARGETMONTH, ");
                strSql.AppendLine(" TOTALWORKDAYS ");
                strSql.AppendLine(" FROM TCalendar_PingZhun_Wai ");
                strSql.AppendLine(" WHERE vcFZGC=@vcFZGC ");
                strSql.AppendLine(string.Format(" AND TARGETMONTH in ('{0}')", varDxny.ToString("yyyyMM")));
                strSql.AppendLine(" ORDER BY TARGETMONTH; ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString(), parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string CopyTo(string vcPlantFrom, List<string> vcPlantTo, List<string> vcMon, string strUserId)
        {
            try
            {
                string sql = string.Empty;
                for (int i = 0; i < vcMon.Count; i++)
                {
                    sql = "select iAutoId from TCalendar_PingZhun_Wai where vcFZGC='" + vcPlantFrom + "' and TARGETMONTH='" + vcMon[i] + "';";
                    DataTable ckb1 = excute.ExcuteSqlWithSelectToDT(sql);
                    if (!(ckb1 != null && ckb1.Rows.Count > 0))
                        return "所复制的" + vcPlantFrom + "工厂、" + vcMon[i] + "月日历不存在！";
                }
                sql = "";
                for (int i = 0; i < vcPlantTo.Count; i++)
                {
                    if (vcPlantTo[i] != vcPlantFrom)//源工厂与目标工厂不相同，即可复制
                    {
                        for (int j = 0; j < vcMon.Count; j++)
                        {
                            sql += "delete from TCalendar_PingZhun_Wai where vcFZGC='" + vcPlantTo[i] + "' and TARGETMONTH='" + vcMon[j] + "';";
                            sql += "insert into TCalendar_PingZhun_Wai (" +
                                    "vcFZGC, TARGETMONTH, TARGETDAY1, TARGETDAY2, TARGETDAY3, TARGETDAY4, TARGETDAY5, TARGETDAY6, TARGETDAY7, TARGETDAY8, " +
                                    "TARGETDAY9, TARGETDAY10, TARGETDAY11, TARGETDAY12, TARGETDAY13, TARGETDAY14, TARGETDAY15, TARGETDAY16, TARGETDAY17, TARGETDAY18, " +
                                    "TARGETDAY19, TARGETDAY20, TARGETDAY21, TARGETDAY22, TARGETDAY23, TARGETDAY24, TARGETDAY25, TARGETDAY26, TARGETDAY27, TARGETDAY28, " +
                                    "TARGETDAY29, TARGETDAY30, TARGETDAY31, TOTALWORKDAYS, DADDTIME, CUPDUSER) " +
                                    "select '" + vcPlantTo[i] + "', '" + vcMon[j] + "', TARGETDAY1, TARGETDAY2, TARGETDAY3, TARGETDAY4, TARGETDAY5, TARGETDAY6, TARGETDAY7, TARGETDAY8, " +
                                    "TARGETDAY9, TARGETDAY10, TARGETDAY11, TARGETDAY12, TARGETDAY13, TARGETDAY14, TARGETDAY15, TARGETDAY16, TARGETDAY17, TARGETDAY18, " +
                                    "TARGETDAY19, TARGETDAY20, TARGETDAY21, TARGETDAY22, TARGETDAY23, TARGETDAY24, TARGETDAY25, TARGETDAY26, TARGETDAY27, TARGETDAY28, " +
                                    "TARGETDAY29, TARGETDAY30, TARGETDAY31, TOTALWORKDAYS, '" + DateTime.Now.ToString() + "','" + strUserId + "' from TCalendar_PingZhun_Wai " +
                                    "where TARGETMONTH = '" + vcMon[j] + "' and vcFZGC = '" + vcPlantFrom + "';";
                        }
                    }
                }
                if (sql == "")
                {
                    return "不能从同一工厂复制到同一工厂！";
                }
                using (SqlConnection conn = new SqlConnection(ComConnectionHelper.GetConnectionString()))
                {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();
                    SqlTransaction trans = conn.BeginTransaction();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = sql;
                    cmd.Transaction = trans;
                    int inums;
                    try
                    {
                        inums = cmd.ExecuteNonQuery();
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                    return inums > 0 ? "" : "复制工作日历失败！";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
