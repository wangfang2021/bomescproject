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
    public class FS1301_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        public DataTable getSearchInfo(string strPackingPlant, string strUser, string strRoler)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("SELECT distinct a.LinId as LinId ");
                strSql.AppendLine(",a.vcPlant as vcPackingPlant ");
                strSql.AppendLine(",e.vcName as vcPackingPlant_name ");
                strSql.AppendLine(",a.vcUserId as vcUserId ");
                strSql.AppendLine(",b.vcUserName as vcUserName ");
                strSql.AppendLine(",isnull(a.vcInPut,'0') as bInPut ");
                strSql.AppendLine(",isnull(a.vcInPutUnLock,'0') as bInPutUnLock ");
                strSql.AppendLine(",isnull(a.vcCheck,'0') as bCheck ");
                strSql.AppendLine(",isnull(a.vcCheckUnLock,'0') as bCheckUnLock ");
                strSql.AppendLine(",isnull(a.vcPack,'0') as bPack ");
                strSql.AppendLine(",isnull(a.vcPackUnLock,'0') as bPackUnLock ");
                strSql.AppendLine(",isnull(a.vcOutPut,'0') as bOutPut ");
                strSql.AppendLine(",isnull(a.vcOutPutUnLock,'0') as bOutPutUnLock");
                strSql.AppendLine(",'1' as bSelectFlag  ");
                strSql.AppendLine("FROM  ");
                strSql.AppendLine("(select * from tPointPower)A ");
                strSql.AppendLine("LEFT JOIN ");
                strSql.AppendLine("(SELECT * FROM SUser)B ");
                strSql.AppendLine("ON A.vcUserId=B.vcUserID ");
                strSql.AppendLine("LEFT JOIN ");
                strSql.AppendLine("(SELECT * FROM SUserRole)C ");
                strSql.AppendLine("ON A.vcUserId=C.vcUserId ");
                strSql.AppendLine("LEFT JOIN ");
                strSql.AppendLine("(SELECT * FROM SRole)D ");
                strSql.AppendLine("ON C.vcRoleID=D.vcRoleID ");
                strSql.AppendLine(" LEFT JOIN ");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C017')E ");
                strSql.AppendLine("ON A.vcPlant=E.vcValue ");
                strSql.AppendLine("where 1=1 ");
                if (strPackingPlant != "")
                {
                    strSql.AppendLine("and a.vcPlant like '" + strPackingPlant + "'");
                }
                if (strUser != "")
                {
                    strSql.AppendLine("and a.vcUserId='"+ strUser + "'");
                }
                if (strRoler != "")
                {
                    strSql.AppendLine("and c.vcRoleID='"+ strRoler + "'");
                }
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void saveDataInfo(DataTable dataTable, string strOperId)
        {
            SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();
            Common.ComConnectionHelper.OpenConection_SQL(ref sqlConnection);
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            try
            {
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                sqlDataAdapter.InsertCommand = new SqlCommand();
                sqlDataAdapter.InsertCommand.Connection = sqlConnection;
                sqlDataAdapter.InsertCommand.CommandType = CommandType.Text;
                sqlDataAdapter.InsertCommand.CommandText += "UPDATE [dbo].[TPointPower]";
                sqlDataAdapter.InsertCommand.CommandText += "   SET [vcInPut] = @vcInPut";
                sqlDataAdapter.InsertCommand.CommandText += "      ,[vcInPutUnLock] = @vcInPutUnLock";
                sqlDataAdapter.InsertCommand.CommandText += "      ,[vcCheck] = @vcCheck";
                sqlDataAdapter.InsertCommand.CommandText += "      ,[vcCheckUnLock] = @vcCheckUnLock";
                sqlDataAdapter.InsertCommand.CommandText += "      ,[vcPack] = @vcPack";
                sqlDataAdapter.InsertCommand.CommandText += "      ,[vcPackUnLock] = @vcPackUnLock";
                sqlDataAdapter.InsertCommand.CommandText += "      ,[vcOutPut] = @vcOutPut";
                sqlDataAdapter.InsertCommand.CommandText += "      ,[vcOutPutUnLock] = @vcOutPutUnLock";
                sqlDataAdapter.InsertCommand.CommandText += "      ,[vcOperatorID] ='"+ strOperId + "'";
                sqlDataAdapter.InsertCommand.CommandText += "      ,[dOperatorTime] =getdate()";
                sqlDataAdapter.InsertCommand.CommandText += " WHERE [LinId]=@LinId";
                sqlDataAdapter.InsertCommand.Parameters.Add("@vcInPut", SqlDbType.VarChar, 1, "vcInPut");
                sqlDataAdapter.InsertCommand.Parameters.Add("@vcInPutUnLock", SqlDbType.VarChar, 1, "vcInPutUnLock");
                sqlDataAdapter.InsertCommand.Parameters.Add("@vcCheck", SqlDbType.VarChar, 1, "vcCheck");
                sqlDataAdapter.InsertCommand.Parameters.Add("@vcCheckUnLock", SqlDbType.VarChar, 1, "vcCheckUnLock");
                sqlDataAdapter.InsertCommand.Parameters.Add("@vcPack", SqlDbType.VarChar, 1, "vcPack");
                sqlDataAdapter.InsertCommand.Parameters.Add("@vcPackUnLock", SqlDbType.VarChar, 1, "vcPackUnLock");
                sqlDataAdapter.InsertCommand.Parameters.Add("@vcOutPut", SqlDbType.VarChar, 1, "vcOutPut");
                sqlDataAdapter.InsertCommand.Parameters.Add("@vcOutPutUnLock", SqlDbType.VarChar, 1, "vcOutPutUnLock");
                sqlDataAdapter.InsertCommand.Parameters.Add("@LinId", SqlDbType.VarChar, 18, "LinId");
                sqlDataAdapter.InsertCommand.Transaction = sqlTransaction;
                sqlDataAdapter.Update(dataTable);
                sqlTransaction.Commit();
                Common.ComConnectionHelper.CloseConnection_SQL(ref sqlConnection);
            }
            catch (Exception ex)
            {
                sqlTransaction.Rollback();
                Common.ComConnectionHelper.CloseConnection_SQL(ref sqlConnection);
                throw ex;
            }
        }
    }
}
