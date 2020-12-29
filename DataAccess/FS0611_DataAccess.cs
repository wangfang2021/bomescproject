using System;
using Common;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using DataEntity;

namespace DataAccess
{
    public class FS0611_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 获取品番箱数
        public DataTable getParts(string varDxny, string iFZGC, int parts_Count)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                System.Data.SqlClient.SqlParameter[] parameters = {
                    new SqlParameter("@varDxny", SqlDbType.VarChar),
                    new SqlParameter("@iFZGC", SqlDbType.VarChar),
                    new SqlParameter("@unitCount", SqlDbType.Int),
                };
                parameters[0].Value = varDxny;
                parameters[1].Value = iFZGC;
                parameters[2].Value = parts_Count;


                strSql.Append("SELECT iAutoId");
                strSql.Append("      ,PARTSNO");
                //发注工厂列
                strSql.Append("      ,iFZGC");
                //订货频度列
                strSql.Append("      ,varMakingOrderType");
                //内外区分列
                strSql.Append("      ,INOUTFLAG");
                //车型编号列
                strSql.Append("      ,CARFAMILYCODE");
                //收容数列
                strSql.Append("      ,QUANTITYPERCONTAINER");
                //箱数列
                strSql.Append("      ,@unitCount as 'iUnits'");
                //品番数列
                strSql.Append("      ,iHySOQN as 'iPCS'");


                strSql.Append("  FROM TSoq");
                strSql.Append("  WHERE varDxny=@varDxny");
                strSql.Append("  AND iFZGC=@iFZGC");
                strSql.Append("  AND iHySOQN/QUANTITYPERCONTAINER=@unitCount");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString(), parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region 获取稼动日历数据
        public DataTable getJdrlData(string varDxny, string varFZGC)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                System.Data.SqlClient.SqlParameter[] parameters = {
                    new SqlParameter("@varDxny", SqlDbType.VarChar),
                    new SqlParameter("@varFZGC", SqlDbType.VarChar),
                };
                parameters[0].Value = varDxny;
                parameters[1].Value = varFZGC;

                strSql.AppendLine(" SELECT ");
                strSql.AppendLine(" TARGETDAY1,");
                strSql.AppendLine(" TARGETDAY2,");
                strSql.AppendLine(" TARGETDAY3,");
                strSql.AppendLine(" TARGETDAY4,");
                strSql.AppendLine(" TARGETDAY5,");
                strSql.AppendLine(" TARGETDAY6,");
                strSql.AppendLine(" TARGETDAY7,");
                strSql.AppendLine(" TARGETDAY8,");
                strSql.AppendLine(" TARGETDAY9,");
                strSql.AppendLine(" TARGETDAY10,");
                strSql.AppendLine(" TARGETDAY11,");
                strSql.AppendLine(" TARGETDAY12,");
                strSql.AppendLine(" TARGETDAY13,");
                strSql.AppendLine(" TARGETDAY14,");
                strSql.AppendLine(" TARGETDAY15,");
                strSql.AppendLine(" TARGETDAY16,");
                strSql.AppendLine(" TARGETDAY17,");
                strSql.AppendLine(" TARGETDAY18,");
                strSql.AppendLine(" TARGETDAY19,");
                strSql.AppendLine(" TARGETDAY20,");
                strSql.AppendLine(" TARGETDAY21,");
                strSql.AppendLine(" TARGETDAY22,");
                strSql.AppendLine(" TARGETDAY23,");
                strSql.AppendLine(" TARGETDAY24,");
                strSql.AppendLine(" TARGETDAY25,");
                strSql.AppendLine(" TARGETDAY26,");
                strSql.AppendLine(" TARGETDAY27,");
                strSql.AppendLine(" TARGETDAY28,");
                strSql.AppendLine(" TARGETDAY29,");
                strSql.AppendLine(" TARGETDAY30,");
                strSql.AppendLine(" TARGETDAY31,");
                strSql.AppendLine(" TOTALWORKDAYS");

                strSql.AppendLine(" FROM SP_M_SOQCLDAR ");

                strSql.AppendLine(" WHERE TARGETMONTH=@varDxny ");

                //发注工厂
                strSql.AppendLine(" AND varFZGC=@varFZGC ");


                return excute.ExcuteSqlWithSelectToDT(strSql.ToString(), parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region 将平准结果存储进数据库
        public int save(DataTable saveTable, string userId,string varDxny,int iMonthFlag)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                System.Data.SqlClient.SqlParameter[] parameters = {
                    new SqlParameter("@varCreater", SqlDbType.VarChar),
                    new SqlParameter("@dCreateTime", SqlDbType.DateTime),
                    new SqlParameter("@TARGETMONTH", SqlDbType.VarChar),
                    new SqlParameter("@iMonthFlag", SqlDbType.Int),
                };

                parameters[0].Value = userId;
                parameters[1].Value = DateTime.Now;
                parameters[2].Value = varDxny;
                parameters[3].Value = iMonthFlag;

                //先删除，后插入
                strSql.AppendLine(" DELETE TSOQReply ");
                strSql.AppendLine(" WHERE ");
                strSql.AppendLine(" TARGETMONTH=@TARGETMONTH; ");


                strSql.AppendLine(" INSERT INTO TSOQReply( ");
                for (int i = 0; i < saveTable.Columns.Count; i++)
                {
                    strSql.AppendLine(saveTable.Columns[i] + ",");
                }

                strSql.AppendLine(" TARGETMONTH, ");
                strSql.AppendLine(" iMonthFlag, ");
                strSql.AppendLine(" varCreater, ");
                strSql.AppendLine(" dCreateTime) ");


                strSql.AppendLine(" VALUES");
                for (int i = 0; i < saveTable.Rows.Count; i++)
                {
                    for (int j = 0; j < saveTable.Columns.Count; j++)
                    {
                        string re ;

                        string ColumnName = saveTable.Columns[j].ColumnName.ToString();
                        int days;
                        if (ColumnName[0] == 'D' && int.TryParse(ColumnName.Replace('D', ' '), out days))
                        {
                            re = string.IsNullOrEmpty(saveTable.Rows[i][j].ToString()) ? "0" : "'" + saveTable.Rows[i][j] + "'";
                        }
                        else {
                            re = string.IsNullOrEmpty(saveTable.Rows[i][j].ToString()) ? "NULL" : "'" + saveTable.Rows[i][j] + "'";
                        }

                        if (j == 0)
                        {
                            strSql.AppendLine("(" + re + ",");
                        }
                        else
                        {
                            strSql.AppendLine("" + re + ",");
                        }
                    }

                    strSql.AppendLine(" @TARGETMONTH, ");
                    strSql.AppendLine(" @iMonthFlag, ");
                    strSql.AppendLine(" @varCreater, ");

                    if (i == saveTable.Rows.Count - 1)
                        strSql.AppendLine(" @dCreateTime) ");
                    else
                        strSql.AppendLine(" @dCreateTime), ");
                }

                strSql.Append(" ; ");


                return excute.ExcuteSqlWithStringOper(strSql.ToString(),parameters);
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
            try
            {
                StringBuilder strSql = new StringBuilder();
                System.Data.SqlClient.SqlParameter[] parameters = {
                    new SqlParameter("@varDxny", SqlDbType.VarChar),
                    new SqlParameter("@varZhanKai", SqlDbType.VarChar),
                    new SqlParameter("@dZhanKaiTime", SqlDbType.VarChar),
                };

                parameters[0].Value = varDxny;
                parameters[1].Value = userId;
                parameters[2].Value = DateTime.Now;

                strSql.AppendLine(" UPDATE TSOQProcess ");
                strSql.AppendLine(" SET iStatus=1, ");
                strSql.AppendLine(" varZhanKai=@varZhanKai, ");
                strSql.AppendLine(" dZhanKaiTime=@dZhanKaiTime ");
                strSql.AppendLine(" WHERE ");
                strSql.AppendLine(" varDxny=@varDxny ");
                //1:外注
                strSql.AppendLine(" AND INOUTFLAG=1; ");

                return excute.ExcuteSqlWithStringOper(strSql.ToString(), parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }
}