
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

        #region 获取数据库连接字符串 主获取子
        public static string GetConnectionString_MainToUnit(string strUnitCode)
        {
            switch (strUnitCode)
            {
                case "TFTM":
                    return "Persist Security Info = False; User ID = sa; Password = SPPS_Server2019; Initial Catalog = SPPSdb; Data Source =172.23.180.116";
                case "SFTM":
                    return "Persist Security Info = False; User ID = sa; Password = Sa123; Initial Catalog = xxx; Data Source =xxx";
                case "SFTMCF":
                    return "Persist Security Info = False; User ID = sa; Password = Sa123; Initial Catalog = xxx; Data Source =xxx";
                case "TK":
                    //return "Persist Security Info = False; User ID = sa; Password = Sa123; Initial Catalog = SPPSdb_TK; Data Source =172.23.140.169";
                    return "Persist Security Info = False; User ID = sa; Password = SPPS_Server2019; Initial Catalog = SPPSdb_TK; Data Source =172.23.180.115";
                default:
                    return "Persist Security Info = False; User ID = sa; Password = Sa123; Initial Catalog = xxx; Data Source =xxx";
            }
            //switch (strUnitCode)
            //{
            //    case "TFTM":
            //        return "Persist Security Info = False; User ID = sa; Password = Sa123; Initial Catalog = SPPSdb_TEST; Data Source =.";
            //    case "SFTM":
            //        return "Persist Security Info = False; User ID = sa; Password = Sa123; Initial Catalog = SPPSdb_SFTM_TEST; Data Source =.";
            //    case "SFTMCF":
            //        return "Persist Security Info = False; User ID = sa; Password = Sa123; Initial Catalog = SPPSdb_SFTMCF_TEST; Data Source =.";
            //    case "TK":
            //        return "Persist Security Info = False; User ID = sa; Password = Sa123; Initial Catalog = SPPSdb_TK_TEST; Data Source =.";
            //    default:
            //        return "Persist Security Info = False; User ID = sa; Password = Sa123; Initial Catalog = SPPSdb_TEST; Data Source =.";
            //}
        }
        #endregion

        #region 获取DMZ从现地服务器同步文件的地址，文件统一由该地址下的Doc进行存储
        public static string GetFileUploadHost()
        {
            return "http://172.23.180.116:XXXX";
        }
        #endregion

        #region 获取数据库连接字符串 
        public static string GetConnectionString()
        {
            return "Persist Security Info = False; User ID = fqm; Password = SPPS_Server2019; Initial Catalog = SPPSdb_Trail_wjw; Data Source =172.23.180.116";
            //return "Persist Security Info = False; User ID = sa; Password = Sa123; Initial Catalog = SPPSdb; Data Source =172.23.140.169";
            //return "Persist Security Info = False; User ID = sa; Password = Sa123456; Initial Catalog = SPPSdb; Data Source =.";
            //return "Persist Security Info = False; User ID = sa; Password = Server2008; Initial Catalog = SPPSdb001; Data Source =TJQM-FY\\SQLEXPRESS";
        }
        #endregion

        #region 获取数据库连接字符串_NQC
        public static string GetConnectionString_NQC()
        {
            return "Persist Security Info = False; User ID = sa; Password = SPPS_Server2019; Initial Catalog = SPPS-IF; Data Source =172.23.180.116";
        }
        #endregion

        #region 获取数据库连接字符串_MAPS
        public static string GetConnectionString_MAPS()
        {
            //临时
            return "Persist Security Info = False; User ID = sa; Password = maps0426; Initial Catalog = MAPS_20210205; Data Source =172.23.129.80";
        }
        #endregion

        #region 创建数据连接，连接状态为Closed--FIFS
        /// <summary>
        /// 根据不同的代码创建数据连接，连接状态为Closed--FIFS
        /// </summary>
        /// <param name="strCode">数据库连接代码</param>
        /// <param name="_bool">true:主数据库获取子数据库 false:反之</param>
        /// <returns></returns>
        public static SqlConnection CreateSqlConnection(string strCode)
        {
            try
            {
                SqlConnection conn_sql = new SqlConnection();
                conn_sql.ConnectionString = GetConnectionString_MainToUnit(strCode);
                CloseConnection_SQL(ref conn_sql);
                return conn_sql;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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

        #region 创建数据连接,连接状态为Closed--MAPS
        public static SqlConnection CreateConnection_MAPS()
        {
            try
            {
                SqlConnection conn_sql = new SqlConnection();
                conn_sql.ConnectionString = GetConnectionString_MAPS();
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