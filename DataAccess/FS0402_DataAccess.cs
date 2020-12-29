using System;
using Common;
using System.Data;
using System.Text;
using System.Data.SqlClient;

namespace DataAccess
{
    public class FS0402_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 按检索条件检索,返回dt
        public DataTable Search(string varDxny, string varDyzt, string varHyzt,string PARTSNO)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                System.Data.SqlClient.SqlParameter[] parameters = {
                    new SqlParameter("@varDxny", SqlDbType.VarChar,10),
                };
                parameters[0].Value = varDxny;


                strSql.Append("SELECT iAutoId");
                strSql.Append("      ,varDxny");
                strSql.Append("      ,varDyzt");
                strSql.Append("      ,varHyzt");
                strSql.Append("      ,PARTSNO");
                strSql.Append("      ,iCbSOQN");
                strSql.Append("      ,decCbBdl");
                strSql.Append("      ,iCbSOQN1");
                strSql.Append("      ,iCbSOQN2");
                strSql.Append("      ,iTzhSOQN");
                strSql.Append("      ,iTzhSOQN1");
                strSql.Append("      ,iTzhSOQN2");
                strSql.Append("      ,iHySOQN");
                strSql.Append("      ,iHySOQN1");
                strSql.Append("      ,iHySOQN2");
                strSql.Append("      ,dHyTime");
                strSql.Append("  FROM TSoq");
                strSql.Append("  WHERE 1=1");


                if (!string.IsNullOrEmpty(varDxny)) {
                    strSql.Append(" AND varDxny=@varDxny");
                }
                //strSql.Append("");
                //strSql.Append("");
                //strSql.Append("");
                //strSql.Append("");
                //strSql.Append("");
                //strSql.Append("");
                //strSql.Append("");
                //strSql.Append("");
                //strSql.Append("");
                //strSql.Append("");
                //strSql.Append("");
                //if (strUnitCode.Trim() != "")
                //    strSql.Append("   and a.vcUnitCode='" + strUnitCode.Trim() + "'        \n");
                //if (strUserId.Trim() != "")
                //    strSql.Append("   and a.vcUserID like '%" + strUserId.Trim() + "%'        \n");
                //if (strUserName.Trim() != "")
                //    strSql.Append("   and a.vcUserName like '%'+@vcUserName+'%'        \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString(), parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId,string varDxny)
        {
            try
            {
                System.Data.SqlClient.SqlParameter[] parameters = {
                    new SqlParameter("@varDxny", SqlDbType.VarChar,10),
                };
                parameters[0].Value = varDxny;

                DateTime now = DateTime.Now;

                StringBuilder sql = new StringBuilder();

                //1、先插入
                sql.AppendLine("  INSERT INTO TSoq( ");
                sql.AppendLine("varDxny,");
                sql.AppendLine("varDyzt,");
                sql.AppendLine("varHyzt,");
                sql.AppendLine("PARTSNO,");
                sql.AppendLine("iCbSOQN,");
                sql.AppendLine("iCbSOQN1,");
                sql.AppendLine("iCbSOQN2,");
                sql.AppendLine("dDrTime)");

                sql.AppendLine("VALUES");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sql.AppendLine("('"+ varDxny + "',");
                    sql.AppendLine("'0',");
                    sql.AppendLine("'0',");
                    sql.AppendLine("'" + dt.Rows[i]["PARTSNO"] + "',");
                    sql.AppendLine("'" + dt.Rows[i]["iCbSOQN"] + "',");
                    sql.AppendLine("'" + dt.Rows[i]["iCbSOQN1"] + "',");
                    sql.AppendLine("'" + dt.Rows[i]["iCbSOQN2"] + "',");
                    sql.AppendLine("'" + now + "')");

                    if (i < dt.Rows.Count - 1) {
                        sql.Append(",");
                    }
                }

                sql.Append(";");

                //2、再更新关联数据
                sql.AppendLine("  UPDATE TSoq SET ");
                sql.AppendLine("CARFAMILYCODE=SP_M_SITEM.CARFAMILYCODE,");
                sql.AppendLine("CURRENTPASTCODE=SP_M_SITEM.CURRENTPASTCODE,");
                sql.AppendLine("varMakingOrderType=SP_M_SITEM.vcPackingFactory,");
                sql.AppendLine("iFZGC=SP_M_SITEM.iFZGC,");
                sql.AppendLine("INOUTFLAG=SP_M_SITEM.INOUTFLAG,");
                sql.AppendLine("SUPPLIERCODE=SP_M_SITEM.SUPPLIERCODE,");
                sql.AppendLine("iSupplierPlant=SP_M_SITEM.iSupplierPlant,");
                sql.AppendLine("QUANTITYPERCONTAINER=SP_M_SITEM.QUANTITYPERCONTAINER");
                //波动率需要用内示月-当前数据来计算。
                //sql.AppendLine("decCbBdl=)");
                sql.AppendLine(" FROM TSoq ");
                
                sql.AppendLine(" LEFT JOIN SP_M_SITEM ");
                //按照逻辑，需要按照品番、包装工厂、TC来连表查询
                sql.AppendLine(" ON SP_M_SITEM.PARTSNO=TSoq.PARTSNO ");


                sql.AppendLine("WHERE TSoq.varDxny=@varDxny;");

                //在SOQprocess表中插入状态
                sql.AppendLine("DELETE TSOQProcess WHERE varDxny=@varDxny;");
                sql.AppendLine("INSERT INTO TSOQProcess(INOUTFLAG,varDxny,iStatus) ");
                sql.AppendLine("VALUES('0',@varDxny,0),");
                sql.AppendLine("('1',@varDxny,0);");

                if (sql.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(sql.ToString(), parameters);
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 向SOQ导入履历中新增数据
        public int importHistory(string varDxny, string varFileName, int iState, string varErrorUrl, string strUserId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                System.Data.SqlClient.SqlParameter[] parameters = {
                    new SqlParameter("@varDxny", SqlDbType.VarChar),
                    new SqlParameter("@varFileName", SqlDbType.VarChar),
                    new SqlParameter("@iState", SqlDbType.Int),
                    new SqlParameter("@varErrorUrl", SqlDbType.VarChar),
                    new SqlParameter("@varInputUserID", SqlDbType.VarChar),
                    new SqlParameter("@dInputTime", SqlDbType.VarChar),
                };
                parameters[0].Value = varDxny;
                parameters[1].Value = varFileName;
                parameters[2].Value = iState;
                parameters[3].Value = varErrorUrl;
                parameters[4].Value = strUserId;
                parameters[5].Value = DateTime.Now;

                //先删除本对象月再新增
                strSql.AppendLine(" DELETE FROM TSoqInput ");
                strSql.AppendLine(" WHERE varDxny=@varDxny; ");

                strSql.AppendLine(" INSERT INTO TSoqInput ");
                strSql.AppendLine("      (varDxny,");
                strSql.AppendLine("      varFileName,");
                strSql.AppendLine("      iState,");
                strSql.AppendLine("      varErrorUrl,");
                strSql.AppendLine("      dInputTime,");
                strSql.AppendLine("      varInputUserID)");
                strSql.AppendLine("      VALUES(@varDxny,");
                strSql.AppendLine("      @varFileName,");
                strSql.AppendLine("      @iState,");
                strSql.AppendLine("      @varErrorUrl,");
                strSql.AppendLine("      @dInputTime,");
                strSql.AppendLine("      @varInputUserID);");

                return excute.ExcuteSqlWithStringOper(strSql.ToString(), parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 承认。将合意后SOQ数据复制到合意SOQ，并改变合意状态，赋予合意时间
        public int Cr(string varDxny, string varDyzt, string varHyzt, string PARTSNO)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                System.Data.SqlClient.SqlParameter[] parameters = {
                    new SqlParameter("@varDxny", SqlDbType.VarChar,10),
                    new SqlParameter("@varDyzt", SqlDbType.VarChar,2),
                    new SqlParameter("@varHyzt", SqlDbType.VarChar,2),
                    new SqlParameter("@PARTSNO", SqlDbType.VarChar,50),
                    new SqlParameter("@dHyTime", SqlDbType.VarChar,50),
                };
                parameters[0].Value = varDxny;
                parameters[1].Value = varDyzt;
                parameters[2].Value = varHyzt;
                parameters[3].Value = PARTSNO;
                parameters[4].Value = DateTime.Now;

                strSql.AppendLine(" UPDATE TSoq SET ");
                strSql.AppendLine("      iHySOQN=iTzhSOQN,");
                strSql.AppendLine("      iHySOQN1=iTzhSOQN1,");
                strSql.AppendLine("      iHySOQN2=iTzhSOQN2,");
                strSql.AppendLine("      varHyzt='2', ");
                strSql.AppendLine("      dHyTime=@dHyTime ");
                strSql.AppendLine(" WHERE 1=1 ");
                //筛选条件：对象年月
                if (!string.IsNullOrEmpty(varDxny))
                {
                    strSql.AppendLine(" AND varDxny=@varDxny ");
                }
                //筛选条件：对应状态
                if (!string.IsNullOrEmpty(varDyzt))
                {
                    strSql.AppendLine(" AND varDyzt=@varDyzt ");
                }
                //筛选条件：合意状态
                if (!string.IsNullOrEmpty(varHyzt))
                {
                    strSql.AppendLine(" AND varHyzt=@varHyzt ");
                }
                //筛选条件：品番
                if (!string.IsNullOrEmpty(PARTSNO))
                {
                    strSql.AppendLine(" AND PARTSNO like '%'+@PARTSNO+'%' ");
                }
                
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