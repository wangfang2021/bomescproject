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
    public class FS0609_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public void save(List<string> re, string varDxny, string vcFZGC, decimal TOTALWORKDAYS, string strUserId)
        {
            try
            {
                System.Data.SqlClient.SqlParameter[] parameters = {
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
                strSql.AppendLine(" DELETE TCalendar_PingZhun_Nei ");
                strSql.AppendLine(" WHERE TARGETMONTH=@varDxny; ");

                //再新增
                strSql.AppendLine(" INSERT INTO TCalendar_PingZhun_Nei( ");
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
                        strSql.AppendLine("'" + re[i] + "',");
                    else
                        strSql.AppendLine("NULL,");
                }

                strSql.AppendLine(" @TOTALWORKDAYS, ");
                strSql.AppendLine(" @DADDTIME, ");
                strSql.AppendLine(" @CUPDUSER); ");


                excute.ExcuteSqlWithStringOper(strSql.ToString(), parameters);
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
                strSql.AppendLine(" FROM TCalendar_PingZhun_Nei ");
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
                strSql.AppendLine(" FROM TCalendar_PingZhun_Nei ");

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

        public string CopyTo(string vcPlantFrom, List<string> vcPlantTo, List<string> vcMon, string strUserId)
        {
            try
            {
                string sql = string.Empty;
                for (int i = 0; i < vcMon.Count; i++)
                {
                    sql = "select iAutoId from TCalendar_PingZhun_Nei where vcFZGC='" + vcPlantFrom + "' and TARGETMONTH='" + vcMon[i] + "';";
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
                            sql += "delete from TCalendar_PingZhun_Nei where vcFZGC='" + vcPlantTo[i] + "' and TARGETMONTH='" + vcMon[j] + "';";
                            sql += "insert into TCalendar_PingZhun_Nei (" +
                                    "vcFZGC, TARGETMONTH, TARGETDAY1, TARGETDAY2, TARGETDAY3, TARGETDAY4, TARGETDAY5, TARGETDAY6, TARGETDAY7, TARGETDAY8, " +
                                    "TARGETDAY9, TARGETDAY10, TARGETDAY11, TARGETDAY12, TARGETDAY13, TARGETDAY14, TARGETDAY15, TARGETDAY16, TARGETDAY17, TARGETDAY18, " +
                                    "TARGETDAY19, TARGETDAY20, TARGETDAY21, TARGETDAY22, TARGETDAY23, TARGETDAY24, TARGETDAY25, TARGETDAY26, TARGETDAY27, TARGETDAY28, " +
                                    "TARGETDAY29, TARGETDAY30, TARGETDAY31, TOTALWORKDAYS, DADDTIME, CUPDUSER) " +
                                    "select '" + vcPlantTo[i] + "', '" + vcMon[j] + "', TARGETDAY1, TARGETDAY2, TARGETDAY3, TARGETDAY4, TARGETDAY5, TARGETDAY6, TARGETDAY7, TARGETDAY8, " +
                                    "TARGETDAY9, TARGETDAY10, TARGETDAY11, TARGETDAY12, TARGETDAY13, TARGETDAY14, TARGETDAY15, TARGETDAY16, TARGETDAY17, TARGETDAY18, " +
                                    "TARGETDAY19, TARGETDAY20, TARGETDAY21, TARGETDAY22, TARGETDAY23, TARGETDAY24, TARGETDAY25, TARGETDAY26, TARGETDAY27, TARGETDAY28, " +
                                    "TARGETDAY29, TARGETDAY30, TARGETDAY31, TOTALWORKDAYS, '" + DateTime.Now.ToString() + "','" + strUserId + "' from TCalendar_PingZhun_Nei " +
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
