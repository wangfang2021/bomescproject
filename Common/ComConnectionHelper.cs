 
using System.Data.SqlClient;
using System;

namespace Common
{
    /// <summary>
    /// ComSqlHelper 的摘要说明。
    /// </summary>
    public class ComConnectionHelper
    {
        #region 打开数据库连接SQL
        public static void OpenConection_SQL(ref SqlConnection conn_sql)
        {
            try
            {
                if (conn_sql.State == System.Data.ConnectionState.Closed)
                {
                    conn_sql.Open();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 关闭数据库连接SQL
        public static void CloseConnection_SQL(ref SqlConnection conn_sql)
        {
            try
            {
                if (conn_sql.State == System.Data.ConnectionState.Open)
                {
                    conn_sql.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion 


        #region 获取数据库连接字符串 
        public static string GetConnectionString()
        {
            return "Persist Security Info = False; User ID = sa; Password = Sa123; Initial Catalog = SPPSdb; Data Source =172.23.140.169";
        }
        #endregion

        #region 获取数据库连接字符串_NQC
        public static string GetConnectionString_NQC()
        {
            return "Persist Security Info = False; User ID = guest; Password = Sa123; Initial Catalog = NQCdb; Data Source =172.23.140.169";
        }
        #endregion


        #region 创建数据连接,连接状态为Closed--FIFS
        public static SqlConnection CreateSqlConnection()
        {
            try
            {
                SqlConnection conn_sql = new SqlConnection();
                conn_sql.ConnectionString = GetConnectionString();
                CloseConnection_SQL(ref conn_sql);
                return conn_sql;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 创建数据连接,连接状态为Closed--NQC
        public static SqlConnection CreateConnection_NQC()
        {
            try
            {
                SqlConnection conn_sql = new SqlConnection();
                conn_sql.ConnectionString = GetConnectionString_NQC();
                CloseConnection_SQL(ref conn_sql);
                return conn_sql;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


    }
}