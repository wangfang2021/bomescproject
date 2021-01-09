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
        public DataTable getRolerInfo()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select vcRoleID as vcValue,vcRoleName as vcName from SRole");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getSearchInfo(string strPlant, string strUser, string strRoler)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine(" SELECT distinct a.LinId as LinId,a.vcUserId as vcUserId,b.vcUserName as vcUserName,a.vcPlant as vcPlant");
                strSql.AppendLine(" ,isnull(a.vcChecker,'0') as bChecker");
                strSql.AppendLine(" ,isnull(a.vcUnLockChecker,'0') as bUnLockChecker");
                strSql.AppendLine(" ,isnull(a.vcPacker,'0') as bPacker");
                strSql.AppendLine(" ,isnull(a.vcUnLockPacker,'0') as bUnLockPacker FROM ");
                strSql.AppendLine(" (select LinId,vcUserId,vcPlant,vcChecker,vcUnLockChecker,vcPacker,vcUnLockPacker from tPointPower)A");
                strSql.AppendLine(" LEFT JOIN");
                strSql.AppendLine(" (SELECT * FROM SUser)B");
                strSql.AppendLine(" ON A.vcUserId=B.vcUserID");
                strSql.AppendLine(" LEFT JOIN");
                strSql.AppendLine(" (SELECT * FROM SUserRole)C");
                strSql.AppendLine(" ON A.vcUserId=C.vcUserId");
                strSql.AppendLine(" LEFT JOIN");
                strSql.AppendLine(" (SELECT * FROM SRole)D");
                strSql.AppendLine(" ON C.vcRoleID=D.vcRoleID");
                strSql.AppendLine(" where 1=1 ");
                if (strPlant != "")
                {
                    strSql.AppendLine("and a.vcPlant like '%"+ strPlant + "%'");
                }
                if (strUser != "")
                {
                    strSql.AppendLine("and A.vcUserId='"+ strUser + "'");
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
        public bool saveDataInfo(DataTable dataTable, string strOperId)
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
                sqlDataAdapter.InsertCommand.CommandText = "";
                sqlDataAdapter.InsertCommand.CommandText += "UPDATE [dbo].[TPointPower]";
                sqlDataAdapter.InsertCommand.CommandText += "   SET [vcChecker] = @vcChecker";
                sqlDataAdapter.InsertCommand.CommandText += "      ,[vcUnLockChecker] = @vcUnLockChecker";
                sqlDataAdapter.InsertCommand.CommandText += "      ,[vcPacker] = @vcPacker";
                sqlDataAdapter.InsertCommand.CommandText += "      ,[vcUnLockPacker] = @vcUnLockPacker";
                sqlDataAdapter.InsertCommand.CommandText += "      ,[vcOperatorID] ='"+ strOperId + "'";
                sqlDataAdapter.InsertCommand.CommandText += "      ,[dOperatorTime] =getdate()";
                sqlDataAdapter.InsertCommand.CommandText += " WHERE [LinId]=@LinId";
                sqlDataAdapter.InsertCommand.Parameters.Add("@vcChecker", SqlDbType.VarChar, 1, "vcChecker");
                sqlDataAdapter.InsertCommand.Parameters.Add("@vcUnLockChecker", SqlDbType.VarChar, 1, "vcUnLockChecker");
                sqlDataAdapter.InsertCommand.Parameters.Add("@vcPacker", SqlDbType.VarChar, 1, "vcPacker");
                sqlDataAdapter.InsertCommand.Parameters.Add("@vcUnLockPacker", SqlDbType.VarChar, 1, "vcUnLockPacker");
                sqlDataAdapter.InsertCommand.Parameters.Add("@LinId", SqlDbType.VarChar, 18, "LinId");
                sqlDataAdapter.InsertCommand.Transaction = sqlTransaction;
                sqlDataAdapter.Update(dataTable);
                sqlTransaction.Commit();
                Common.ComConnectionHelper.CloseConnection_SQL(ref sqlConnection);
                return true;
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
