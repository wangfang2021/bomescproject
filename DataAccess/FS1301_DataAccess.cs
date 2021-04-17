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

        public void importSave(DataTable dt, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("DELETE FROM [dbo].[TPointPowerTmp] where  vcOperatorID='" + strUserId + "' \n");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string vcUserId = dt.Rows[i]["vcUserId"] == System.DBNull.Value ? "" : dt.Rows[i]["vcUserId"].ToString().Trim();
                    string vcPlant = dt.Rows[i]["vcPackingPlant"] == System.DBNull.Value ? "" : dt.Rows[i]["vcPackingPlant"].ToString().Trim();
                    #region
                    string vcInPut = dt.Rows[i]["bInPut"] == System.DBNull.Value ? "" : dt.Rows[i]["bInPut"].ToString().Trim();
                    if (vcInPut == "√")
                    {
                        vcInPut = "1";
                    } else
                    {
                        vcInPut = "0";
                    }
                    string vcInPutUnLock = dt.Rows[i]["bInPutUnLock"] == System.DBNull.Value ? "" : dt.Rows[i]["bInPutUnLock"].ToString().Trim();
                    if (vcInPutUnLock == "√")
                    {
                        vcInPutUnLock = "1";
                    }
                    else
                    {
                        vcInPutUnLock = "0";
                    }
                    #endregion
                    #region
                    string vcCheck = dt.Rows[i]["bCheck"] == System.DBNull.Value ? "" : dt.Rows[i]["bCheck"].ToString().Trim();
                    if (vcCheck == "√")
                    {
                        vcCheck = "1";
                    }
                    else
                    {
                        vcCheck = "0";
                    }
                    string vcCheckUnLock = dt.Rows[i]["bCheckUnLock"] == System.DBNull.Value ? "" : dt.Rows[i]["bCheckUnLock"].ToString().Trim();
                    if (vcCheckUnLock == "√")
                    {
                        vcCheckUnLock = "1";
                    }
                    else
                    {
                        vcCheckUnLock = "0";
                    }
                    #endregion
                    #region
                    string vcPack = dt.Rows[i]["bPack"] == System.DBNull.Value ? "" : dt.Rows[i]["bPack"].ToString().Trim();
                    if (vcPack == "√")
                    {
                        vcPack = "1";
                    }
                    else
                    {
                        vcPack = "0";
                    }
                    string vcPackUnLock = dt.Rows[i]["bPackUnLock"] == System.DBNull.Value ? "" : dt.Rows[i]["bPackUnLock"].ToString().Trim();
                    if (vcPackUnLock == "√")
                    {
                        vcPackUnLock = "1";
                    }
                    else
                    {
                        vcPackUnLock = "0";
                    }
                    #endregion
                    #region
                    string vcOutPut = dt.Rows[i]["bOutPut"] == System.DBNull.Value ? "" : dt.Rows[i]["bOutPut"].ToString().Trim();
                    if (vcOutPut == "√")
                    {
                        vcOutPut = "1";
                    }
                    else
                    {
                        vcOutPut = "0";
                    }
                    string vcOutPutUnLock = dt.Rows[i]["bOutPutUnLock"] == System.DBNull.Value ? "" : dt.Rows[i]["bOutPutUnLock"].ToString().Trim();
                    if (vcOutPutUnLock == "√")
                    {
                        vcOutPutUnLock = "1";
                    }
                    else
                    {
                        vcOutPutUnLock = "0";
                    }
                    #endregion

                    sql.Append("   INSERT INTO [dbo].[TPointPowerTmp]   \n");
                    sql.Append("              (vcUserId, vcPlant, vcInPut, vcInPutUnLock, vcCheck, vcCheckUnLock,   \n");
                    sql.Append("              vcPack, vcPackUnLock, vcOutPut, vcOutPutUnLock, vcOperatorID, dOperatorTime    \n");
                    sql.Append("             ) values    \n");
                    sql.Append("   		  ( " + getSqlValue(vcUserId, true) + "," + getSqlValue(vcPlant, true) + "," + getSqlValue(vcInPut, true) + "," + getSqlValue(vcInPutUnLock, true) + ",  \n");
                    sql.Append("    " + getSqlValue(vcCheck, true) + "," + getSqlValue(vcCheckUnLock, true) + "," + getSqlValue(vcPack, true) + ",   \n");
                    sql.Append("     " + getSqlValue(vcPackUnLock, true) + "," + getSqlValue(vcOutPut, true) + "," + getSqlValue(vcOutPutUnLock, true) + ",   \n");
                    sql.Append("   '" + strUserId + "' \n");
                    sql.Append("     ,getdate())  \n");
                }
               
                //更新
                sql.Append("   update b set b.vcInPut=a.vcInPut,b.vcInPutUnLock=a.vcInPutUnLock,   \n");
                sql.Append("   b.vcCheck=a.vcCheck,b.vcCheckUnLock=a.vcCheckUnLock,   \n");
                sql.Append("   b.vcPack =a.vcPack,b.vcPackUnLock=a.vcPackUnLock,   \n");
                sql.Append("   b.vcOutPut=a.vcOutPut,b.vcOutPutUnLock=a.vcOutPutUnLock,   \n");
                sql.Append("   b.dOperatorTime=GETDATE()   \n");
                sql.Append("   from   \n");
                sql.Append("   (   \n");
                sql.Append("   select * from [TPointPowerTmp] where [vcOperatorID]='"+ strUserId + "'   \n");
                sql.Append("   ) a   \n");
                sql.Append("   left join (select * from TPointPower)b    \n");
                sql.Append("   on a.vcUserId=b.vcUserId and a.vcPlant=b.vcPlant   \n");
                sql.Append("   where b.vcUserId is not null   \n");
                sql.Append("   ;   \n");
                
                sql.Append("   INSERT INTO [dbo].[TPointPower]   \n");
                sql.Append("              (vcUserId, vcPlant, vcInPut, vcInPutUnLock, vcCheck, vcCheckUnLock, vcPack, \n");
                sql.Append("               vcPackUnLock, vcOutPut, vcOutPutUnLock, vcOperatorID, dOperatorTime    \n");
                sql.Append("             )    \n");
                sql.Append("    select a.vcUserId, a.vcPlant, a.vcInPut, a.vcInPutUnLock, a.vcCheck, a.vcCheckUnLock,       \n");
                sql.Append("    a.vcPack, a.vcPackUnLock, a.vcOutPut, a.vcOutPutUnLock, a.vcOperatorID, a.dOperatorTime       \n");
                sql.Append("    from      \n");
                sql.Append("    (      \n");
                sql.Append("    select * from [TPointPowerTmp] where [vcOperatorID]='"+ strUserId + "'      \n");
                sql.Append("    ) a      \n");
                sql.Append("    left join (select * from TPointPower)b       \n");
                sql.Append("    on a.vcUserId=b.vcUserId and a.vcPlant=b.vcPlant      \n");
                sql.Append("    where b.vcUserId is null      \n");
                sql.Append("      \n");
                if (sql.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(sql.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #region 返回insert语句值
        /// <summary>
        /// 返回insert语句值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="isObject">如果insert时间、金额或者其他对象类型数据，为true</param>
        /// <returns></returns>
        private string getSqlValue(Object obj, bool isObject)
        {
            if (obj == null)
                return "null";
            else if (obj.ToString().Trim() == "" && isObject)
                return "null";
            else
                return "'" + obj.ToString() + "'";
        }
        #endregion
        public DataTable isCheckUserId(string userId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                
                strSql.AppendLine(" select * from SUser where vcUserID='"+ userId + "'");
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
