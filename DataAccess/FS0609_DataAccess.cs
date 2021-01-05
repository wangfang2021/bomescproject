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
        public void save(List<string> re, string varDxny, string varFZGC, decimal TOTALWORKDAYS, string strUserId)
        {
            try
            {
                System.Data.SqlClient.SqlParameter[] parameters = {
                    new SqlParameter("@varDxny", SqlDbType.VarChar),
                    new SqlParameter("@varFZGC", SqlDbType.VarChar),
                    new SqlParameter("@TOTALWORKDAYS", SqlDbType.Decimal),
                    new SqlParameter("@DADDTIME", SqlDbType.DateTime),
                    new SqlParameter("@CUPDUSER", SqlDbType.VarChar),
                };
                parameters[0].Value = varDxny;
                parameters[1].Value = varFZGC;
                parameters[2].Value = TOTALWORKDAYS;
                parameters[3].Value = DateTime.Now;
                parameters[4].Value = strUserId;

                StringBuilder strSql = new StringBuilder();

                //先删除
                strSql.AppendLine(" DELETE SP_M_SOQCLDAR_N ");
                strSql.AppendLine(" WHERE TARGETMONTH=@varDxny; ");

                //再新增
                strSql.AppendLine(" INSERT INTO SP_M_SOQCLDAR_N( ");
                strSql.AppendLine(" varFZGC, ");
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
                strSql.AppendLine(" @varFZGC, ");
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
        public DataTable Search(DateTime varDxny, string varFZGC)
        {
            try
            {
                System.Data.SqlClient.SqlParameter[] parameters = {
                    new SqlParameter("@varFZGC", SqlDbType.VarChar),
                };
                parameters[0].Value = varFZGC;

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
                strSql.AppendLine(" FROM SP_M_SOQCLDAR_N ");

                strSql.AppendLine(" WHERE varFZGC=@varFZGC ");
                strSql.AppendLine(string.Format(" AND TARGETMONTH in ('{0}','{1}','{2}')", varDxny.ToString("yyyy/MM"), varDxny.AddMonths(1).ToString("yyyy/MM"), varDxny.AddMonths(2).ToString("yyyy/MM")));

                strSql.AppendLine(" ORDER BY TARGETMONTH; ");



                return excute.ExcuteSqlWithSelectToDT(strSql.ToString(), parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
