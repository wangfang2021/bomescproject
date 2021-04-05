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
    public class FS1310_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        public DataTable getSearchInfo(string strPackPlant, string strPinMu, string strPartId, string strOperImage)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select a.iAutoId as LinId");
                strSql.AppendLine("		,a.vcBZPlant as vcPackPlant");
                strSql.AppendLine("		,d.vcName as vcPackPlant_name");
                strSql.AppendLine("		,a.vcPart_id as vcPartId");
                strSql.AppendLine("		,b.vcBigPM as vcPinMu");
                strSql.AppendLine("		,c.vcPicUrl as vcOperImage");
                strSql.AppendLine("		,e.vcUserName as vcOperator");
                strSql.AppendLine("		,Convert(varchar(10),c.dOperatorTime,111) as dOperatorTime");
                strSql.AppendLine("		,'0' as bModFlag,'0' as bAddFlag");
                strSql.AppendLine("		,'1' as bSelectFlag,'' as vcBgColor from ");
                strSql.AppendLine("(select * from TPackageMaster)a");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select * from TPMRelation)b");
                strSql.AppendLine("on a.vcSmallPM=b.vcSmallPM");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select * from TPackOperImage)c");
                strSql.AppendLine("on a.vcPart_id=c.vcPartId and a.vcBZPlant=c.vcPlant");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select * from TCode where vcCodeId='C023')d");
                strSql.AppendLine("on a.vcBZPlant=d.vcValue");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select * from SUser)e");
                strSql.AppendLine("on c.vcOperatorID=e.vcUserID");
                strSql.AppendLine("where 1=1");
                if (strPackPlant != "")
                {
                    strSql.AppendLine("and a.vcBZPlant='" + strPackPlant + "' ");
                }
                if (strPinMu != "")
                {
                    strSql.AppendLine("and b.vcBigPM='" + strPinMu + "'");
                }
                if (strPartId != "")
                {
                    strSql.AppendLine("and a.vcPart_id like '" + strPartId + "%'");
                }
                if (strOperImage != "")
                {
                    if (strOperImage == "0")
                    {
                        strSql.AppendLine("and c.vcPicUrl is null");
                    }
                    else
                    {
                        strSql.AppendLine("and c.vcPicUrl is not null");
                    }
                }
                strSql.AppendLine("order by a.vcBZPlant,c.vcPartId desc");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setSaveInfo(DataTable dtImport, string strOperId, ref DataTable dtMessage)
        {
            SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();

            sqlConnection.Open();
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            try
            {
                #region 写入数据库
                #region sqlCommand_addinfo
                SqlCommand sqlCommand_addinfo = sqlConnection.CreateCommand();
                sqlCommand_addinfo.Transaction = sqlTransaction;
                sqlCommand_addinfo.CommandType = CommandType.Text;
                StringBuilder strSql_addinfo = new StringBuilder();

                #region SQL and Parameters
                strSql_addinfo.AppendLine("IF((SELECT COUNT(*) FROM [TPackOperImage] WHERE [vcPlant]=@vcPlant AND [vcPartId]=@vcPartId)>0)");
                strSql_addinfo.AppendLine("BEGIN");
                strSql_addinfo.AppendLine("UPDATE [dbo].[TPackOperImage] SET  [vcPicUrl]=@vcPicUrlUUID,[vcOperatorID]='" + strOperId + "',[dOperatorTime]=GETDATE() WHERE [vcPlant]=@vcPlant AND [vcPartId]=@vcPartId");
                strSql_addinfo.AppendLine("END");
                strSql_addinfo.AppendLine("ELSE ");
                strSql_addinfo.AppendLine("BEGIN");
                strSql_addinfo.AppendLine("INSERT INTO [dbo].[TPackOperImage]");
                strSql_addinfo.AppendLine("           ([vcPlant]");
                strSql_addinfo.AppendLine("           ,[vcPartId]");
                strSql_addinfo.AppendLine("           ,[vcPicUrl_small]");
                strSql_addinfo.AppendLine("           ,[vcPicUrl]");
                strSql_addinfo.AppendLine("           ,[vcNote]");
                strSql_addinfo.AppendLine("           ,[vcOperatorID]");
                strSql_addinfo.AppendLine("           ,[dOperatorTime])");
                strSql_addinfo.AppendLine("     VALUES");
                strSql_addinfo.AppendLine("           (@vcPlant");
                strSql_addinfo.AppendLine("           ,@vcPartId");
                strSql_addinfo.AppendLine("           ,NULL");
                strSql_addinfo.AppendLine("           ,@vcPicUrlUUID");
                strSql_addinfo.AppendLine("           ,@vcNote");
                strSql_addinfo.AppendLine("           ,'" + strOperId + "'");
                strSql_addinfo.AppendLine("           ,GETDATE())");
                strSql_addinfo.AppendLine("END");
                sqlCommand_addinfo.CommandText = strSql_addinfo.ToString();
                sqlCommand_addinfo.Parameters.AddWithValue("@vcPlant", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcPartId", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcPicUrlUUID", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcNote", "");
                #endregion
                foreach (DataRow item in dtImport.Rows)
                {
                    #region Value
                    sqlCommand_addinfo.Parameters["@vcPlant"].Value = item["vcPlant"].ToString();
                    sqlCommand_addinfo.Parameters["@vcPartId"].Value = item["vcPartId"].ToString();
                    sqlCommand_addinfo.Parameters["@vcPicUrlUUID"].Value = item["vcPicUrlUUID"].ToString();
                    sqlCommand_addinfo.Parameters["@vcNote"].Value = item["vcNote"].ToString();
                    #endregion
                    sqlCommand_addinfo.ExecuteNonQuery();
                }
                #endregion
                //提交事务
                sqlTransaction.Commit();
                sqlConnection.Close();
                #endregion

            }
            catch (Exception ex)
            {
                DataRow dataRow = dtMessage.NewRow();
                dataRow["vcMessage"] = "数据写入数据库失败！";
                dtMessage.Rows.Add(dataRow);
                //回滚事务
                if (sqlTransaction != null && sqlConnection != null)
                {
                    sqlTransaction.Rollback();
                    sqlConnection.Close();
                }
            }
        }
        public void deleteInfo(List<Dictionary<string, Object>> listInfoData)
        {
            SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();

            sqlConnection.Open();
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            try
            {
                #region 写入数据库
                //启动事务
                SqlCommand sqlCommand = sqlConnection.CreateCommand();//主表
                sqlCommand.Transaction = sqlTransaction;
                sqlCommand.CommandType = CommandType.Text;
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("delete from TPackOperImage where [vcPartId] =@vcPartId and [vcPlant]=@vcPackPlant");
                sqlCommand.CommandText = stringBuilder.ToString();

                sqlCommand.Parameters.AddWithValue("@vcPackPlant", "");
                sqlCommand.Parameters.AddWithValue("@vcPartId", "");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    sqlCommand.Parameters["@vcPackPlant"].Value = listInfoData[i]["vcPackPlant"].ToString();
                    sqlCommand.Parameters["@vcPartId"].Value = listInfoData[i]["vcPartId"].ToString();
                    sqlCommand.ExecuteNonQuery();
                }
                //提交事务
                sqlTransaction.Commit();
                sqlConnection.Close();
                #endregion

            }
            catch (Exception ex)
            {
                //回滚事务
                if (sqlTransaction != null && sqlConnection != null)
                {
                    sqlTransaction.Rollback();
                    sqlConnection.Close();
                }
            }
        }
        
    }
}
