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