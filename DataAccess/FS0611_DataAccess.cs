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
                //外注
                strSql.Append("  AND INOUTFLAG='1'");
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
                strSql.AppendLine(" TARGETMONTH=@TARGETMONTH ");
                //外注
                strSql.AppendLine(" AND INOUTFLAG='1'; ");



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


        #region 下载SOQReply（检索内容）
        public DataTable search(string varDxny)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                System.Data.SqlClient.SqlParameter[] parameters = {
                    new SqlParameter("@varDxny", SqlDbType.VarChar),
                };
                parameters[0].Value = varDxny;

                strSql.AppendLine(" SELECT tn.*,tn1.[N+1 O/L],tn1.[N+1 Units],tn1.[N+1 PCS], ");
                strSql.AppendLine(" tn2.[N+2 O/L],tn2.[N+2 Units],tn2.[N+2 PCS] ");
                strSql.AppendLine(" FROM ");

                strSql.AppendLine(" (SELECT ");
                strSql.AppendLine(" PARTSNO as 'PartsNo',");
                //发注工厂
                strSql.AppendLine(" iFZGC as '发注工厂',");
                //订货频度
                strSql.AppendLine(" varMakingOrderType as '订货频度',");
                strSql.AppendLine(" CARFAMILYCODE as 'CFC',");
                strSql.AppendLine(" QUANTITYPERCONTAINER as 'OrdLot',");
                strSql.AppendLine(" iUnits as 'N Units',");
                strSql.AppendLine(" iPCS as 'N PCS',");
                strSql.AppendLine(" D1,");
                strSql.AppendLine(" D2,");
                strSql.AppendLine(" D3,");
                strSql.AppendLine(" D4,");
                strSql.AppendLine(" D5,");
                strSql.AppendLine(" D6,");
                strSql.AppendLine(" D7,");
                strSql.AppendLine(" D8,");
                strSql.AppendLine(" D9,");
                strSql.AppendLine(" D10,");
                strSql.AppendLine(" D11,");
                strSql.AppendLine(" D12,");
                strSql.AppendLine(" D13,");
                strSql.AppendLine(" D14,");
                strSql.AppendLine(" D15,");
                strSql.AppendLine(" D16,");
                strSql.AppendLine(" D17,");
                strSql.AppendLine(" D18,");
                strSql.AppendLine(" D19,");
                strSql.AppendLine(" D20,");
                strSql.AppendLine(" D21,");
                strSql.AppendLine(" D22,");
                strSql.AppendLine(" D23,");
                strSql.AppendLine(" D24,");
                strSql.AppendLine(" D25,");
                strSql.AppendLine(" D26,");
                strSql.AppendLine(" D27,");
                strSql.AppendLine(" D28,");
                strSql.AppendLine(" D29,");
                strSql.AppendLine(" D30,");
                strSql.AppendLine(" D31");

                strSql.AppendLine(" FROM TSOQReply ");

                strSql.AppendLine(" WHERE TARGETMONTH=@varDxny ");
                //外注
                strSql.AppendLine(" AND INOUTFLAG='1' ");
                //对象月
                strSql.AppendLine(" AND iMonthFlag=0) tn ");

                strSql.AppendLine(" LEFT JOIN (");
                strSql.AppendLine(" SELECT PARTSNO, ");
                strSql.AppendLine(" QUANTITYPERCONTAINER as 'N+1 O/L',");
                strSql.AppendLine(" iUnits as 'N+1 Units',");
                strSql.AppendLine(" iPCS as 'N+1 PCS'");
                strSql.AppendLine(" FROM TSOQReply ");
                strSql.AppendLine(" WHERE TARGETMONTH=@varDxny ");
                //外注
                strSql.AppendLine(" AND INOUTFLAG='1' ");
                //内示月
                strSql.AppendLine(" AND iMonthFlag=1) tn1 ");

                strSql.AppendLine(" ON tn.PartsNo=tn1.PARTSNO ");


                strSql.AppendLine(" LEFT JOIN (");
                strSql.AppendLine(" SELECT PARTSNO, ");
                strSql.AppendLine(" QUANTITYPERCONTAINER as 'N+2 O/L',");
                strSql.AppendLine(" iUnits as 'N+2 Units',");
                strSql.AppendLine(" iPCS as 'N+2 PCS'");
                strSql.AppendLine(" FROM TSOQReply ");
                strSql.AppendLine(" WHERE TARGETMONTH=@varDxny ");
                //外注
                strSql.AppendLine(" AND INOUTFLAG='1' ");
                //内内示月
                strSql.AppendLine(" AND iMonthFlag=2) tn2 ");

                strSql.AppendLine(" ON tn.PartsNo=tn2.PARTSNO ");


                return excute.ExcuteSqlWithSelectToDT(strSql.ToString(), parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 导入后保存
        public void importSave(DataTable dt, string varDxny)
        {
            try
            {
                System.Data.SqlClient.SqlParameter[] parameters = {
                    new SqlParameter("@varDxny", SqlDbType.VarChar,10),
                };
                parameters[0].Value = varDxny;


                StringBuilder sql = new StringBuilder();

                for (int i = 0; i < dt.Rows.Count; i++) {
                    //如果为空则跳过
                    if (string.IsNullOrEmpty(dt.Rows[i]["PARTSNO"].ToString()))
                        continue;

                    //更新对象月数据。（内示与内内示月不更新）
                    sql.AppendLine("  UPDATE TSOQReply SET ");
                    for (int j = 1; j < 32; j++) {
                        sql.AppendLine(" D"+j+"=" + dt.Rows[i]["D"+j+""] + ",");
                    }
                    sql.AppendLine(" iPCS=" + dt.Rows[i]["iPCS"] + ",");
                    sql.AppendLine(" iUnits=" + dt.Rows[i]["iUnits"] + ",");
                    sql.AppendLine(" QUANTITYPERCONTAINER=" + dt.Rows[i]["QUANTITYPERCONTAINER"] );

                    sql.AppendLine(" WHERE PARTSNO='"+ dt.Rows[i]["PARTSNO"]+"'");
                    sql.AppendLine(" AND TARGETMONTH=@varDxny ");
                    //外注
                    sql.AppendLine(" AND INOUTFLAG='1' ");
                    //对象月
                    sql.AppendLine(" AND iMonthFlag=0; ");
                }
                
                excute.ExcuteSqlWithStringOper(sql.ToString(), parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}