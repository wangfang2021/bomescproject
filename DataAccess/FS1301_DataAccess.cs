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
                strSql.AppendLine("select distinct t2.LinId as LinId ");
                strSql.AppendLine(",t1.vcPackingPlant as vcPackingPlant ");
                strSql.AppendLine(",t1.vcPackingPlant_name as vcPackingPlant_name ");
                strSql.AppendLine(",t1.vcUserId as vcUserId ");
                strSql.AppendLine(",t1.vcUserName as vcUserName ");
                strSql.AppendLine(",t1.vcRoleName as vcRoleName ");
                strSql.AppendLine(",isnull(t2.vcInPut,'0') as bInPut ");
                strSql.AppendLine(",isnull(t2.vcInPutUnLock,'0') as bInPutUnLock ");
                strSql.AppendLine(",isnull(t2.vcCheck,'0') as bCheck ");
                strSql.AppendLine(",isnull(t2.vcCheckUnLock,'0') as bCheckUnLock ");
                strSql.AppendLine(",isnull(t2.vcPack,'0') as bPack ");
                strSql.AppendLine(",isnull(t2.vcPackUnLock,'0') as bPackUnLock ");
                strSql.AppendLine(",isnull(t2.vcOutPut,'0') as bOutPut ");
                strSql.AppendLine(",isnull(t2.vcOutPutUnLock,'0') as bOutPutUnLock");
                strSql.AppendLine(",'1' as bSelectFlag   from ");
                strSql.AppendLine("(select DISTINCT A.vcBaoZhuangPlace AS vcPackingPlant");
                strSql.AppendLine("		,D.vcName AS vcPackingPlant_name");
                strSql.AppendLine("		,A.vcUserID");
                strSql.AppendLine("		,A.vcUserName");
                strSql.AppendLine("		,C.vcRoleName from ");
                strSql.AppendLine("(SELECT * FROM SUser WHERE isnull(vcStop,'0')<>'1'");
                if (strPackingPlant != "")
                {
                    strSql.AppendLine("and ISNULL(vcBaoZhuangPlace,'')='"+ strPackingPlant + "'");
                }
                if (strUser != "")
                {
                    strSql.AppendLine("and vcUserID='"+ strUser + "'");
                }
                strSql.AppendLine(")A");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT * FROM SUserRole)B");
                strSql.AppendLine("ON A.vcUserID=B.vcUserId");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcUserId");
                strSql.AppendLine("		,vcRoleName = stuff((");
                strSql.AppendLine("SELECT ',' + vcRoleName");
                strSql.AppendLine("FROM ");
                strSql.AppendLine("(SELECT vcUserId,vcRoleName FROM ");
                strSql.AppendLine("(SELECT * FROM SUserRole)T1");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT * FROM SRole)T2");
                strSql.AppendLine("ON T1.vcRoleID=T2.vcRoleID )t");
                strSql.AppendLine("WHERE t.vcUserId = a.vcUserId FOR XML path('')), 1, 1, '')");
                strSql.AppendLine("FROM ");
                strSql.AppendLine("(SELECT vcUserId,vcRoleName FROM ");
                strSql.AppendLine("(SELECT * FROM SUserRole)T1");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT * FROM SRole)T2");
                strSql.AppendLine("ON T1.vcRoleID=T2.vcRoleID ) a");
                strSql.AppendLine("GROUP BY vcUserId)C");
                strSql.AppendLine("ON A.vcUserID=C.vcUserId");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C023')D");
                strSql.AppendLine("ON A.vcBaoZhuangPlace=D.vcValue");
                strSql.AppendLine("WHERE 1=1");
                if (strRoler != "")
                {
                    strSql.AppendLine("AND B.vcRoleID='" + strRoler + "'");
                }
                strSql.AppendLine(")t1");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select * from tPointPower)t2");
                strSql.AppendLine("on t1.vcPackingPlant=t2.vcPlant and t1.vcUserID=t2.vcUserId");
                strSql.AppendLine("order by t1.vcPackingPlant,t1.vcRoleName,t1.vcUserID");
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
                sqlDataAdapter.InsertCommand.CommandText += "   if(@LinId='')";
                sqlDataAdapter.InsertCommand.CommandText += "   begin";
                sqlDataAdapter.InsertCommand.CommandText += "   INSERT INTO [dbo].[TPointPower]";
                sqlDataAdapter.InsertCommand.CommandText += "              ([vcUserId]";
                sqlDataAdapter.InsertCommand.CommandText += "              ,[vcPlant]";
                sqlDataAdapter.InsertCommand.CommandText += "              ,[vcInPut]";
                sqlDataAdapter.InsertCommand.CommandText += "              ,[vcInPutUnLock]";
                sqlDataAdapter.InsertCommand.CommandText += "              ,[vcCheck]";
                sqlDataAdapter.InsertCommand.CommandText += "              ,[vcCheckUnLock]";
                sqlDataAdapter.InsertCommand.CommandText += "              ,[vcPack]";
                sqlDataAdapter.InsertCommand.CommandText += "              ,[vcPackUnLock]";
                sqlDataAdapter.InsertCommand.CommandText += "              ,[vcOutPut]";
                sqlDataAdapter.InsertCommand.CommandText += "              ,[vcOutPutUnLock]";
                sqlDataAdapter.InsertCommand.CommandText += "              ,[vcOperatorID]";
                sqlDataAdapter.InsertCommand.CommandText += "              ,[dOperatorTime])";
                sqlDataAdapter.InsertCommand.CommandText += "        VALUES";
                sqlDataAdapter.InsertCommand.CommandText += "              (@vcUserId";
                sqlDataAdapter.InsertCommand.CommandText += "              ,@vcPlant";
                sqlDataAdapter.InsertCommand.CommandText += "              ,@vcInPut";
                sqlDataAdapter.InsertCommand.CommandText += "              ,@vcInPutUnLock";
                sqlDataAdapter.InsertCommand.CommandText += "              ,@vcCheck";
                sqlDataAdapter.InsertCommand.CommandText += "              ,@vcCheckUnLock";
                sqlDataAdapter.InsertCommand.CommandText += "              ,@vcPack";
                sqlDataAdapter.InsertCommand.CommandText += "              ,@vcPackUnLock";
                sqlDataAdapter.InsertCommand.CommandText += "              ,@vcOutPut";
                sqlDataAdapter.InsertCommand.CommandText += "              ,@vcOutPutUnLock";
                sqlDataAdapter.InsertCommand.CommandText += "              ,'"+ strOperId + "'";
                sqlDataAdapter.InsertCommand.CommandText += "              ,GETDATE())";
                sqlDataAdapter.InsertCommand.CommandText += "   end";
                sqlDataAdapter.InsertCommand.CommandText += "   else";
                sqlDataAdapter.InsertCommand.CommandText += "   begin";
                sqlDataAdapter.InsertCommand.CommandText += "   UPDATE [dbo].[TPointPower]";
                sqlDataAdapter.InsertCommand.CommandText += "      SET [vcInPut] = @vcInPut";
                sqlDataAdapter.InsertCommand.CommandText += "         ,[vcInPutUnLock] = @vcInPutUnLock";
                sqlDataAdapter.InsertCommand.CommandText += "         ,[vcCheck] = @vcCheck";
                sqlDataAdapter.InsertCommand.CommandText += "         ,[vcCheckUnLock] = @vcCheckUnLock";
                sqlDataAdapter.InsertCommand.CommandText += "         ,[vcPack] = @vcPack";
                sqlDataAdapter.InsertCommand.CommandText += "         ,[vcPackUnLock] = @vcPackUnLock";
                sqlDataAdapter.InsertCommand.CommandText += "         ,[vcOutPut] = @vcOutPut";
                sqlDataAdapter.InsertCommand.CommandText += "         ,[vcOutPutUnLock] = @vcOutPutUnLock";
                sqlDataAdapter.InsertCommand.CommandText += "         ,[vcOperatorID] ='"+ strOperId + "'";
                sqlDataAdapter.InsertCommand.CommandText += "         ,[dOperatorTime] =getdate()";
                sqlDataAdapter.InsertCommand.CommandText += "    WHERE [LinId]=@LinId";
                sqlDataAdapter.InsertCommand.CommandText += "   end";
                sqlDataAdapter.InsertCommand.Parameters.Add("@LinId", SqlDbType.VarChar, 18, "LinId");
                sqlDataAdapter.InsertCommand.Parameters.Add("@vcUserId", SqlDbType.VarChar, 20, "vcUserId");
                sqlDataAdapter.InsertCommand.Parameters.Add("@vcPlant", SqlDbType.VarChar, 20, "vcPlant");
                sqlDataAdapter.InsertCommand.Parameters.Add("@vcInPut", SqlDbType.VarChar, 1, "vcInPut");
                sqlDataAdapter.InsertCommand.Parameters.Add("@vcInPutUnLock", SqlDbType.VarChar, 1, "vcInPutUnLock");
                sqlDataAdapter.InsertCommand.Parameters.Add("@vcCheck", SqlDbType.VarChar, 1, "vcCheck");
                sqlDataAdapter.InsertCommand.Parameters.Add("@vcCheckUnLock", SqlDbType.VarChar, 1, "vcCheckUnLock");
                sqlDataAdapter.InsertCommand.Parameters.Add("@vcPack", SqlDbType.VarChar, 1, "vcPack");
                sqlDataAdapter.InsertCommand.Parameters.Add("@vcPackUnLock", SqlDbType.VarChar, 1, "vcPackUnLock");
                sqlDataAdapter.InsertCommand.Parameters.Add("@vcOutPut", SqlDbType.VarChar, 1, "vcOutPut");
                sqlDataAdapter.InsertCommand.Parameters.Add("@vcOutPutUnLock", SqlDbType.VarChar, 1, "vcOutPutUnLock");
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
