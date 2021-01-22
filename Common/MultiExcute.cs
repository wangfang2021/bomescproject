using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Common
{
    public class MultiExcute
    {
        private SqlConnection Sqlcon { get; set; }

        public MultiExcute()
        {

        }

        public SqlConnection GetConnect(string SYTCode)
        {
            string sConnect = ComConnectionHelper.GetConnectionString_MainToUnit(SYTCode);
            Sqlcon = new SqlConnection(sConnect);
            return Sqlcon;
        }

        public SqlConnection GetConnect()
        {
            string sConnect = ComConnectionHelper.GetConnectionString();
            Sqlcon = new SqlConnection(sConnect);
            return Sqlcon;

        }
        public DataTable ExcuteSqlWithSelect(string SqlStr)
        {
            try
            {
                DataTable newDt = new DataTable();
                Sqlcon = GetConnect();
                Sqlcon.Open();
                using (SqlCommand cmd = new SqlCommand(SqlStr, Sqlcon))
                {
                    cmd.CommandTimeout = 0;
                    SqlDataReader dr = cmd.ExecuteReader();
                    newDt.Load(dr);
                }
                Sqlcon.Close();
                return newDt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == Sqlcon.State)
                {
                    Sqlcon.Close();
                }
            }
        }
        /// <summary>
        /// 普通检索
        /// </summary>
        /// <param name="SqlStr"></param>
        /// <returns></returns>
        public DataSet ExcuteSqlWithSelectToDS(string SqlStr)
        {
            try
            {
                DataSet newDS = new DataSet();
                Sqlcon = GetConnect();
                Sqlcon.Open();
                using (SqlCommand cmd = new SqlCommand(SqlStr, Sqlcon))
                {
                    cmd.CommandTimeout = 0;
                    SqlDataAdapter ada = new SqlDataAdapter(cmd);
                    ada.Fill(newDS);

                }
                Sqlcon.Close();
                return newDS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == Sqlcon.State)
                {
                    Sqlcon.Close();
                }
            }
        }

        /// <summary>
        /// 检索，带参数的
        /// </summary>
        /// <param name="SqlStr"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataSet ExcuteSqlWithSelectToDS(string SqlStr, SqlParameter[] parameters)
        {
            try
            {
                DataSet newDS = new DataSet();
                Sqlcon = GetConnect();
                Sqlcon.Open();
                using (SqlCommand cmd = new SqlCommand(SqlStr, Sqlcon))
                {
                    cmd.CommandTimeout = 0;
                    foreach (var item in parameters)
                    {
                        cmd.Parameters.Add(item);
                    }
                    SqlDataAdapter ada = new SqlDataAdapter(cmd);
                    ada.Fill(newDS);

                }
                Sqlcon.Close();
                return newDS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == Sqlcon.State)
                {
                    Sqlcon.Close();
                }
            }
        }

        public DataTable ExcuteSqlWithSelectToDT(string SqlStr, string SYTCode)
        {
            try
            {
                DataSet newDS = new DataSet();
                Sqlcon = GetConnect(SYTCode);
                Sqlcon.Open();
                using (SqlCommand cmd = new SqlCommand(SqlStr, Sqlcon))
                {
                    cmd.CommandTimeout = 0;
                    SqlDataAdapter ada = new SqlDataAdapter(cmd);
                    ada.Fill(newDS);

                }
                Sqlcon.Close();
                return newDS.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == Sqlcon.State)
                {
                    Sqlcon.Close();
                }
            }
        }

        /// <summary>
        /// 普通检索
        /// </summary>
        /// <param name="SqlStr"></param>
        /// <returns></returns>
        public DataTable ExcuteSqlWithSelectToDT(string SqlStr)
        {
            try
            {
                DataSet newDS = new DataSet();
                Sqlcon = GetConnect();
                Sqlcon.Open();
                using (SqlCommand cmd = new SqlCommand(SqlStr, Sqlcon))
                {
                    cmd.CommandTimeout = 0;
                    SqlDataAdapter ada = new SqlDataAdapter(cmd);
                    ada.Fill(newDS);

                }
                Sqlcon.Close();
                return newDS.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == Sqlcon.State)
                {
                    Sqlcon.Close();
                }
            }
        }

        /// <summary>
        /// 普通检索，带参数
        /// </summary>
        /// <param name="SqlStr"></param>
        /// <returns></returns>
        public DataTable ExcuteSqlWithSelectToDT(string SqlStr, SqlParameter[] parameters)
        {
            try
            {
                DataSet newDS = new DataSet();
                Sqlcon = GetConnect();
                Sqlcon.Open();
                using (SqlCommand cmd = new SqlCommand(SqlStr, Sqlcon))
                {
                    cmd.CommandTimeout = 0;
                    foreach (var item in parameters)
                    {
                        cmd.Parameters.Add(item);
                    }
                    SqlDataAdapter ada = new SqlDataAdapter(cmd);
                    ada.Fill(newDS);

                }
                Sqlcon.Close();
                return newDS.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == Sqlcon.State)
                {
                    Sqlcon.Close();
                }
            }
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="dt">删除的数据来源，只删除rowstate为delele的数据</param>
        ///  <param name="strName">存储过程名</param>
        /// <param name="DeletePara">存储过程参数</param>
        /// <returns></returns>
        public int BatchDelete(DataTable dt, string strName, SqlParameter[] DeletePara)
        {
            SqlTransaction st = null;
            ////返回值定义，默认为失败false
            //bool bIsOK = false;
            try
            {
                this.Sqlcon = Common.ComConnectionHelper.CreateSqlConnection();
                Sqlcon = GetConnect();
                Sqlcon.Open();
                st = this.Sqlcon.BeginTransaction();
                SqlDataAdapter da = new SqlDataAdapter();
                da.DeleteCommand = new SqlCommand();
                da.DeleteCommand.Connection = this.Sqlcon;
                da.DeleteCommand.CommandType = System.Data.CommandType.StoredProcedure;
                da.DeleteCommand.CommandText = strName;
                da.DeleteCommand.Transaction = st;
                //设定参数

                da.DeleteCommand.Parameters.AddRange(DeletePara);
                da.DeleteCommand.CommandTimeout = 0;

                int iResult = da.Update(dt);
                da.DeleteCommand.Dispose();
                da.Dispose();
                st.Commit();
                Sqlcon.Close();
                return iResult;

            }
            catch (System.Data.DBConcurrencyException)
            {
                return 0;
            }
            catch (Exception ex)
            {
                if (st != null)
                {
                    st.Rollback();
                }
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == Sqlcon.State)
                {
                    Sqlcon.Close();
                }
            }

        }

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="dt">更新的数据来源，只更新rowstate为modify的数据</param>
        ///  <param name="strName">存储过程名</param>
        /// <param name="DeletePara">存储过程参数</param>
        /// <returns></returns>
        public int BatchUpdate(DataTable dt, string strName, SqlParameter[] UpdatePara)
        {
            SqlTransaction st = null;
            ////返回值定义，默认为失败false
            //bool bIsOK = false;
            try
            {
                this.Sqlcon = Common.ComConnectionHelper.CreateSqlConnection();
                Sqlcon = GetConnect();
                Sqlcon.Open();
                st = this.Sqlcon.BeginTransaction();
                SqlDataAdapter da = new SqlDataAdapter();
                da.UpdateCommand = new SqlCommand();
                da.UpdateCommand.Connection = this.Sqlcon;
                da.UpdateCommand.CommandType = System.Data.CommandType.StoredProcedure;
                da.UpdateCommand.CommandText = strName;
                da.UpdateCommand.Transaction = st;
                //设定参数

                da.UpdateCommand.Parameters.AddRange(UpdatePara);
                da.UpdateCommand.CommandTimeout = 0;
                int iResult = da.Update(dt);
                da.UpdateCommand.Dispose();
                da.Dispose();
                st.Commit();
                Sqlcon.Close();
                return iResult;
            }
            catch (Exception ex)
            {
                if (st != null)
                {
                    st.Rollback();
                }
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == Sqlcon.State)
                {
                    Sqlcon.Close();
                }
            }
        }

        /// <summary>
        /// 根据sql返回第一行第一段的值
        /// </summary>
        /// <param name="strExeSql">要执行的sql语句</param>
        /// <returns></returns>
        public int ExecuteScalar(string strExeSql)
        {
            SqlConnection conn = Common.ComConnectionHelper.CreateSqlConnection();
            SqlCommand comm = new SqlCommand(strExeSql, conn);
            try
            {
                comm.CommandType = CommandType.Text;
                comm.CommandTimeout = 0;
                Common.ComConnectionHelper.OpenConection_SQL(ref conn);
                int count = (int)comm.ExecuteScalar();
                Common.ComConnectionHelper.CloseConnection_SQL(ref conn);
                return count;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == conn.State)
                {
                    conn.Close();
                }
            }
        }

        public int ExcuteSqlWithStringOper(string SqlStr, string strCode)
        {
            Int32 rowsAffected = 0;
            SqlTransaction st = null;
            SqlConnection conn = null;
            try
            {
                DataSet ds = new DataSet();

                conn = Common.ComConnectionHelper.CreateSqlConnection(strCode);
                
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = new SqlCommand();
                da.SelectCommand.Connection = conn;
                da.SelectCommand.CommandType = System.Data.CommandType.Text;
                da.SelectCommand.CommandTimeout = 0;
                da.SelectCommand.CommandText = SqlStr;
                da.SelectCommand.Connection.Open();
                st = conn.BeginTransaction();
                da.SelectCommand.Transaction = st;
                rowsAffected = da.SelectCommand.ExecuteNonQuery();
                st.Commit();
                da.SelectCommand.Connection.Close();
                da.SelectCommand.Dispose();
                da.Dispose();
            }
            catch (Exception ex)
            {
                if (st != null)
                {
                    st.Rollback();
                }
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == conn.State)
                {
                    conn.Close();
                }
            }
            return rowsAffected;
        }

        public int ExcuteSqlWithStringOper(string SqlStr)
        {
            Int32 rowsAffected = 0;
            SqlTransaction st = null;
            SqlConnection conn = null;
            try
            {
                DataSet ds = new DataSet();
                conn = Common.ComConnectionHelper.CreateSqlConnection();
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = new SqlCommand();
                da.SelectCommand.Connection = conn;
                da.SelectCommand.CommandType = System.Data.CommandType.Text;
                da.SelectCommand.CommandTimeout = 0;
                da.SelectCommand.CommandText = SqlStr;
                da.SelectCommand.Connection.Open();
                st = conn.BeginTransaction();
                da.SelectCommand.Transaction = st;
                rowsAffected = da.SelectCommand.ExecuteNonQuery();
                st.Commit();
                da.SelectCommand.Connection.Close();
                da.SelectCommand.Dispose();
                da.Dispose();


            }
            catch (Exception ex)
            {
                if (st != null)
                {
                    st.Rollback();
                }

                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == conn.State)
                {
                    conn.Close();
                }
            }
            return rowsAffected;
        }

        public int ExcuteSqlWithStringOper(string SqlStr, SqlParameter[] parameters)
        {
            Int32 rowsAffected = 0;
            SqlTransaction st = null;
            SqlConnection conn = null;
            try
            {
                DataSet ds = new DataSet();
                conn = Common.ComConnectionHelper.CreateSqlConnection();
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = new SqlCommand();
                da.SelectCommand.Connection = conn;
                da.SelectCommand.CommandType = System.Data.CommandType.Text;
                da.SelectCommand.CommandTimeout = 0;
                da.SelectCommand.CommandText = SqlStr;
                foreach (var item in parameters)
                {
                    da.SelectCommand.Parameters.Add(item);
                }
                da.SelectCommand.Connection.Open();
                st = conn.BeginTransaction();
                da.SelectCommand.Transaction = st;
                rowsAffected = da.SelectCommand.ExecuteNonQuery();
                st.Commit();
                da.SelectCommand.Connection.Close();
                da.SelectCommand.Dispose();
                da.Dispose();
            }
            catch (Exception ex)
            {
                if (st != null)
                {
                    st.Rollback();
                }

                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == conn.State)
                {
                    conn.Close();
                }
            }
            return rowsAffected;
        }

        public bool ExcuteSqlWithLogin(string SqlStr)
        {
            try
            {
                Int32 rowsAffected = 0;
                Sqlcon = GetConnect();
                Sqlcon.Open();
                using (SqlCommand cmd = new SqlCommand(SqlStr, Sqlcon))
                {
                    cmd.CommandTimeout = 0;
                    rowsAffected = cmd.ExecuteNonQuery();
                }
                Sqlcon.Close();
                if (rowsAffected != 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == Sqlcon.State)
                {
                    Sqlcon.Close();
                }
            }

        }

        /// <summary>
        /// 批量更新1个表
        /// 原因：方便检查更新是否全部成功
        /// </summary>
        /// <param name="Data">数据</param>
        /// <param name="InsertSP">插入时使用的存储过程名</param>
        /// <param name="InsertPara">插入时的参数</param>
        /// <param name="UpdateSP">更新时使用的存储过程名</param>
        /// <param name="UpdatePara">更新参数</param>
        /// <param name="DeleteSP">删除时使用的存储过程名</param>
        /// <param name="DeletePara">删除参数</param>
        /// <returns></returns>
        public int BatchUpdate(DataTable Data,
            string InsertSP, SqlParameter[] InsertPara,
            string UpdateSP, SqlParameter[] UpdatePara,
            string DeleteSP, SqlParameter[] DeletePara)
        {
            SqlCommand sqlInsertCmd = null;
            SqlCommand sqlUpdateCmd = null;
            SqlCommand sqlDeleteCmd = null;
            SqlTransaction sqlTran = null;

            Sqlcon = GetConnect();

            //更新的行数.
            int iUpdateRows = -1;

            try
            {
                //实例化插入Command
                if (InsertSP != null)
                {
                    //sqlInsertCmd=new SqlCommand(InsertSP,this.conn);

                    sqlInsertCmd = new SqlCommand(InsertSP, Sqlcon);
                    sqlInsertCmd.CommandType = CommandType.StoredProcedure;
                    sqlInsertCmd.CommandTimeout = 0;
                    //忽略任何返回参数或行以提高性能
                    sqlInsertCmd.UpdatedRowSource = UpdateRowSource.None;

                    //设定参数
                    foreach (SqlParameter oPara in InsertPara)
                    {
                        sqlInsertCmd.Parameters.Add(oPara);
                    }
                }

                //实例化更新Command
                if (UpdateSP != null)
                {

                    sqlUpdateCmd = new SqlCommand(UpdateSP, Sqlcon);
                    sqlUpdateCmd.CommandType = CommandType.StoredProcedure;
                    sqlUpdateCmd.CommandTimeout = 0;
                    //忽略任何返回参数或行以提高性能
                    sqlUpdateCmd.UpdatedRowSource = UpdateRowSource.None;

                    //设定参数
                    foreach (SqlParameter oPara in UpdatePara)
                    {
                        sqlUpdateCmd.Parameters.Add(oPara);
                    }
                }
                //实例化删除Cmmmand
                if (DeleteSP != null)
                {
                    sqlDeleteCmd = new SqlCommand(DeleteSP, Sqlcon);
                    sqlDeleteCmd.CommandType = CommandType.StoredProcedure;
                    sqlDeleteCmd.CommandTimeout = 0;
                    //忽略任何返回参数或行以提高性能
                    sqlDeleteCmd.UpdatedRowSource = UpdateRowSource.None;
                    //ADD END

                    //设定参数
                    foreach (SqlParameter oPara in DeletePara)
                    {
                        sqlDeleteCmd.Parameters.Add(oPara);
                    }
                }

                Sqlcon.Open();
                sqlTran = Sqlcon.BeginTransaction();

                //执行更新
                // Create a SqlDataAdapter, and dispose of it after we are done
                using (SqlDataAdapter dataAdapter = new SqlDataAdapter())
                {
                    // Set the data adapter commands
                    dataAdapter.UpdateCommand = sqlUpdateCmd;
                    dataAdapter.InsertCommand = sqlInsertCmd;
                    dataAdapter.DeleteCommand = sqlDeleteCmd;

                    //初始化Command对象事务
                    if (!object.Equals(dataAdapter.UpdateCommand, null))
                        dataAdapter.UpdateCommand.Transaction = sqlTran;
                    if (!object.Equals(dataAdapter.InsertCommand, null))
                        dataAdapter.InsertCommand.Transaction = sqlTran;
                    if (!object.Equals(dataAdapter.DeleteCommand, null))
                        dataAdapter.DeleteCommand.Transaction = sqlTran;

                    // Update the dataset changes in the data source
                    iUpdateRows = dataAdapter.Update(Data);

                    if (iUpdateRows != Data.Rows.Count)
                    {
                        int ErrorRows = Data.Rows.Count - iUpdateRows;
                        //有错误时回滚
                        sqlTran.Rollback();
                        throw new Exception("有 " + ErrorRows.ToString() + "没有更新成功！");
                    }

                    // Commit all the changes made to the DataSet
                    Data.AcceptChanges();
                }
                //提交事务
                sqlTran.Commit();
            }
            catch (Exception ex)
            {
                if (sqlTran != null)
                {
                    //回滚事务
                    sqlTran.Rollback();
                }

                throw ex;
            }
            finally
            {
                //关闭数据库连接
                if (ConnectionState.Open == Sqlcon.State)
                {
                    Sqlcon.Close();
                }
            }

            return iUpdateRows;
        }

        /// <summary>
        /// 追加此重载方法,用于使用SQL语句执行批量任务.
        /// </summary>
        /// <param name="Data">要更新的数据</param>
        /// <param name="InsertCMD">执行插入的Sqlcommand</param>
        /// <param name="UpdateCMD">执行修改的Sqlcommand</param>
        /// <param name="DeleteCMD">执行删除的Sqlcommand</param>
        /// <returns></returns>
        public int BatchUpdate(DataTable Data, SqlCommand InsertCMD, SqlCommand UpdateCMD, SqlCommand DeleteCMD)
        {
            //连接对象
            Sqlcon = GetConnect();

            //事务对象
            SqlTransaction Trans = null;

            //更新行数
            int iUpdateRows = -1;

            try
            {
                //插入
                if (InsertCMD != null)
                {
                    //指定命令使用连接
                    InsertCMD.Connection = Sqlcon;

                    //命令执行SQL语句
                    InsertCMD.CommandType = CommandType.Text;
                    InsertCMD.CommandTimeout = 0;

                    //忽略任何返回参数或行以提高性能
                    InsertCMD.UpdatedRowSource = UpdateRowSource.None;
                }

                //修改
                if (UpdateCMD != null)
                {
                    //指定命令使用连接
                    UpdateCMD.Connection = Sqlcon;

                    //命令执行SQL语句
                    UpdateCMD.CommandType = CommandType.Text;
                    UpdateCMD.CommandTimeout = 0;
                    //忽略任何返回参数或行以提高性能
                    UpdateCMD.UpdatedRowSource = UpdateRowSource.None;
                }

                //删除
                if (DeleteCMD != null)
                {
                    //指定命令使用连接
                    DeleteCMD.Connection = Sqlcon;

                    //命令执行SQL语句
                    DeleteCMD.CommandType = CommandType.Text;
                    DeleteCMD.CommandTimeout = 0;
                    //忽略任何返回参数或行以提高性能
                    DeleteCMD.UpdatedRowSource = UpdateRowSource.None;
                }

                //打开数据库连接
                //				this.ConnectDB();

                //打开数据库连接
                if (Sqlcon.State == ConnectionState.Closed)
                    Sqlcon.Open();

                //开启事务
                Trans = Sqlcon.BeginTransaction();

                //执行更新
                // Create a SqlDataAdapter, and dispose of it after we are done
                using (SqlDataAdapter dataAdapter = new SqlDataAdapter())
                {
                    // Set the data adapter commands
                    dataAdapter.UpdateCommand = UpdateCMD;
                    dataAdapter.InsertCommand = InsertCMD;
                    dataAdapter.DeleteCommand = DeleteCMD;

                    //命令不为空时,初始化命令的事务对象
                    if (!object.Equals(dataAdapter.UpdateCommand, null))
                        dataAdapter.UpdateCommand.Transaction = Trans;
                    if (!object.Equals(dataAdapter.InsertCommand, null))
                        dataAdapter.InsertCommand.Transaction = Trans;
                    if (!object.Equals(dataAdapter.DeleteCommand, null))
                        dataAdapter.DeleteCommand.Transaction = Trans;

                    // Update the dataset changes in the data source
                    iUpdateRows = dataAdapter.Update(Data);

                    //检查更新是否全部成功执行
                    if (iUpdateRows != Data.Rows.Count)
                    {
                        int ErrorRows = Data.Rows.Count - iUpdateRows;

                        //有错误时回滚
                        Trans.Rollback();
                        throw new Exception("有 " + ErrorRows.ToString() + "没有更新成功！");
                    }

                    // Commit all the changes made to the DataSet
                    Data.AcceptChanges();
                }

                //提交事务
                Trans.Commit();
            }
            catch (Exception ex)
            {
                if (Trans != null)
                {
                    Trans.Rollback();
                }
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == Sqlcon.State)
                {
                    Sqlcon.Close();
                }
            }

            //返回受影响行数
            return iUpdateRows;
        }

        /// <summary>
        /// 执行没有返回结果的存储过程
        /// </summary>
        /// <param name="SPName">存储过程名</param>
        /// <param name="Parameters">参数</param>
        /// <returns>受影响的行数</returns>
        public int ExecuteSPNoQuery(string SPName, SqlParameter[] Parameters)
        {
            try
            {
                //打开数据库连接
                Sqlcon = GetConnect();
                //执行存储过程

                return ExecuteNonQuery(Sqlcon, CommandType.StoredProcedure, SPName, Parameters);

            }
            catch (Exception ex)
            {
                //Throw Exception
                throw ex;
            }
            finally
            {
                //关闭数据库连接
                if (ConnectionState.Open == Sqlcon.State)
                {
                    Sqlcon.Close();
                }
            }
        }

        /// <summary>
        /// 执行没有返回结果的SQL
        /// </summary>
        /// <param name="strSql">SQL字符串</param>
        /// <returns>受影响的行数</returns>
        public int ExecuteSQLNoQuery(string strSql)
        {
            try
            {

                //打开数据库连接
                Sqlcon = GetConnect();

                if (Sqlcon == null) throw new ArgumentNullException("connection");

                // 创建命令
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = Sqlcon;
                cmd.CommandText = strSql;
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 0;
                if (Sqlcon.State == ConnectionState.Closed)
                {
                    Sqlcon.Open();
                }
                int retval = 0;

                // 执行
                retval = cmd.ExecuteNonQuery();
                return retval;
            }
            catch (Exception ex)
            {
                //Throw Exception
                throw ex;
            }
            finally
            {
                //关闭数据库连接
                if (ConnectionState.Open == Sqlcon.State)
                {
                    Sqlcon.Close();
                }
            }

        }

        /// <summary>
        /// 执行一个具有参数的数据库命令
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="commandType">命令类型 (stored procedure, text 等)</param>
        /// <param name="commandText">存储过程或sql语句</param>
        /// <param name="commandParameters">一组参数</param>
        /// <returns>当前命令所影响的行数</returns>
        public int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException("connection");

            // 创建命令
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = 0;
            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);
            int retval = 0;
            try
            {
                // 执行
                retval = cmd.ExecuteNonQuery();
            }
            catch (SqlException ep)
            {
                throw (ep);
            }
            // 清空命令连接
            cmd.Parameters.Clear();
            if (mustCloseConnection)
                connection.Close();
            return retval;
        }

        /// <summary>
        /// This method opens (if necessary) and assigns a connection, transaction, command type and parameters 
        /// to the provided command
        /// </summary>
        /// <param name="command">The SqlCommand to be prepared</param>
        /// <param name="connection">A valid SqlConnection, on which to execute this command</param>
        /// <param name="transaction">事务, or 'null'</param>
        /// <param name="commandType">命令类型 (stored procedure, text 等)</param>
        /// <param name="commandText">存储过程或sql语句</param>
        /// <param name="commandParameters">An array of SqlParameters to be associated with the command or 'null' if no parameters are required</param>
        /// <param name="mustCloseConnection"><c>true</c> if the connection was opened by the method, otherwose is false.</param>
        private static void PrepareCommand(SqlCommand command,
            SqlConnection connection,
            SqlTransaction transaction,
            CommandType commandType,
            string commandText,
            SqlParameter[] commandParameters,
            out bool mustCloseConnection)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");

            //added by tany for connection null check
            if (connection == null) throw new ArgumentNullException("connection");
            //added end

            // If the provided connection is not open, we will open it
            if (connection.State != ConnectionState.Open)
            {
                mustCloseConnection = true;
                connection.Open();
            }
            else
            {
                mustCloseConnection = false;
            }

            // Associate the connection with the command
            command.Connection = connection;

            // Set the command text (stored procedure name or SQL statement)
            command.CommandText = commandText;

            // If we were provided a transaction, assign it
            if (transaction != null)
            {
                if (transaction.Connection == null)
                    throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                command.Transaction = transaction;
            }

            // Set the command type
            command.CommandType = commandType;

            // Attach the command parameters if they are provided
            if (commandParameters != null)
            {
                AttachParameters(command, commandParameters);
            }
            //			return;
        }

        /// <summary>
        /// This method is used to attach array of SqlParameters to a SqlCommand.
        /// 
        /// This method will assign a value of DbNull to any parameter with a direction of
        /// InputOutput and a value of null.  
        /// 
        /// This behavior will prevent default values from being used, but
        /// this will be the less common case than an intended pure output parameter (derived as InputOutput)
        /// where the user provided no input value.
        /// </summary>
        /// <param name="command">The command to which the parameters will be added</param>
        /// <param name="commandParameters">An array of SqlParameters to be added to command</param>
        private static void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (commandParameters != null)
            {
                foreach (SqlParameter p in commandParameters)
                {
                    if (p != null)
                    {
                        // Check for derived output value with no value assigned
                        if ((p.Direction == ParameterDirection.InputOutput ||
                            p.Direction == ParameterDirection.Input) &&
                            (p.Value == null))
                        {
                            p.Value = DBNull.Value;
                        }
                        command.Parameters.Add(p);
                    }
                }
            }
        }

        #region 带事务,执行SQL语句,返回int
        /// <summary>
        /// 返回int
        /// </summary>
        public int CommonExcuteNonQuery(string strSQL)
        {
            SqlTransaction MyTran = null;
            SqlCommand cmd = null;
            try
            {
                DataTable newDt = new DataTable();

                Sqlcon = GetConnect();
                Sqlcon.Open();
                MyTran = Sqlcon.BeginTransaction();
                cmd = new SqlCommand(strSQL, Sqlcon);
                cmd.CommandTimeout = 0;
                cmd.Transaction = MyTran;

                Int32 rowsAffected = 0;
                rowsAffected = cmd.ExecuteNonQuery();
                MyTran.Commit();
                return rowsAffected;

            }
            catch (Exception ex)
            {
                if (MyTran != null)
                {
                    MyTran.Rollback();
                }
                throw ex;
            }
            finally
            {
                if (ConnectionState.Open == Sqlcon.State)
                {
                    Sqlcon.Close();
                    Sqlcon = null;
                }

                MyTran.Dispose();
                MyTran = null;

                cmd.Dispose();
                cmd = null;
            }
        }
        #endregion

    }
}
