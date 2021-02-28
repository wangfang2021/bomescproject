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
    public class FS1101_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        public DataTable getSearchInfo(string strPackMaterNo, string strTrolleyNo, string strPartId, string strOrderNo, string strLianFan)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select distinct a.iAutoId as LinId, a.vcLotid as vcPackMaterNo,a.[vcTrolleyNo] AS vcTrolleyNo,'1' as bSelectFlag ");
                //strSql.AppendLine("select a.iAutoId, a.vcInno as vcInPutOrderNo,a.vcLotid as vcPackMaterNo,a.[vcTrolleyNo] AS vcTrolleyNo,a.vcPackingpartsno as vcPackPartId  ");
                strSql.AppendLine("	from   ");
                //strSql.AppendLine("		,cast(dQty as varchar(50)) as iQty,vcPackingpartslocation as vcLocation,a.vcPackinggroup,'1' as bSelectFlag 	from   ");
                strSql.AppendLine("(select * from tpacklist  ");
                if (strTrolleyNo != "")
                {
                    strSql.AppendLine(" where iAutoId in (select MAX(iAutoId) as iAutoId from tpacklist where vcTrolleyNo='" + strTrolleyNo + "') ");
                }
                strSql.AppendLine(")a  ");
                strSql.AppendLine("left join  ");
                strSql.AppendLine("(select * from TOperateSJ where vcZYType='S0')b  ");
                strSql.AppendLine("on a.vcInno=b.vcInputNo  ");
                strSql.AppendLine("WHERE a.[dFirstPrintTime] IS NOT NULL   ");
                if (strPackMaterNo != "")
                {
                    strSql.AppendLine("  AND a.vcLotid like '%" + strPackMaterNo + "%' ");
                }
                if (strPartId != "")
                {
                    strSql.AppendLine("  AND b.vcPart_id like '" + strPartId + "%' ");
                }
                if (strOrderNo != "")
                {
                    strSql.AppendLine("  AND b.vcKBOrderNo='" + strOrderNo + "' ");
                }
                if (strLianFan != "")
                {
                    strSql.AppendLine("  AND b.vcKBLFNo='" + strLianFan + "'");
                }
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getSearchInfo(string strPackMaterNo)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select vcTrolleyNo,vcLotid,vcPackingpartsno,vcPackinggroup,cast(dQty as varchar(50)) as dQty,vcPackingpartslocation,convert(varchar(10),getdate(),23) as dPrintDate from tpacklist where vcLotid='"+ strPackMaterNo + "' order by [iAutoId]");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getPrintTemp(string strPage)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select top(1)* from [tPrintTemp_" + strPage + "]");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setPrintTemp(DataTable dtSub, string strOperId, ref DataTable dtMessage)
        {
            SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();

            sqlConnection.Open();
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            try
            {
                #region 写入数据库
                #region sqlCommand_deleteinfo
                SqlCommand sqlCommand_deleteinfo = sqlConnection.CreateCommand();
                sqlCommand_deleteinfo.Transaction = sqlTransaction;
                sqlCommand_deleteinfo.CommandType = CommandType.Text;
                StringBuilder strSql_deleteinfo = new StringBuilder();
                #region SQL and Parameters
                strSql_deleteinfo.AppendLine("DELETE from tPrintTemp_FS1101 where vcOperator='" + strOperId + "'");
                sqlCommand_deleteinfo.CommandText = strSql_deleteinfo.ToString();
                #endregion
                sqlCommand_deleteinfo.ExecuteNonQuery();
                #endregion

                #region sqlCommand_sub
                SqlCommand sqlCommand_sub = sqlConnection.CreateCommand();
                sqlCommand_sub.Transaction = sqlTransaction;
                sqlCommand_sub.CommandType = CommandType.Text;
                StringBuilder strSql_sub = new StringBuilder();

                #region SQL and Parameters
                strSql_sub.AppendLine("INSERT INTO [dbo].[tPrintTemp_FS1101]");
                strSql_sub.AppendLine("           ([UUID]");
                strSql_sub.AppendLine("           ,[vcOperator]");
                strSql_sub.AppendLine("           ,[dOperatorTime]");
                strSql_sub.AppendLine("           ,[vcTrolleyNo]");
                strSql_sub.AppendLine("           ,[vcLotid]");
                strSql_sub.AppendLine("           ,[vcPackingpartsno]");
                strSql_sub.AppendLine("           ,[vcPackinggroup]");
                strSql_sub.AppendLine("           ,[dQty]");
                strSql_sub.AppendLine("           ,[vcPackingpartslocation]");
                strSql_sub.AppendLine("           ,[dPrintDate]");
                strSql_sub.AppendLine("           ,[vcCodemage])");
                strSql_sub.AppendLine("     VALUES");
                strSql_sub.AppendLine("           (@UUID");
                strSql_sub.AppendLine("           ,'"+ strOperId + "'");
                strSql_sub.AppendLine("           ,GETDATE()");
                strSql_sub.AppendLine("           ,@vcTrolleyNo");
                strSql_sub.AppendLine("           ,@vcLotid");
                strSql_sub.AppendLine("           ,@vcPackingpartsno");
                strSql_sub.AppendLine("           ,@vcPackinggroup");
                strSql_sub.AppendLine("           ,@dQty");
                strSql_sub.AppendLine("           ,@vcPackingpartslocation");
                strSql_sub.AppendLine("           ,@dPrintDate");
                strSql_sub.AppendLine("           ,@vcCodemage)");
                sqlCommand_sub.CommandText = strSql_sub.ToString();
                sqlCommand_sub.Parameters.AddWithValue("@UUID", "");
                sqlCommand_sub.Parameters.AddWithValue("@vcTrolleyNo", "");
                sqlCommand_sub.Parameters.AddWithValue("@vcLotid", "");
                sqlCommand_sub.Parameters.AddWithValue("@vcPackingpartsno", "");
                sqlCommand_sub.Parameters.AddWithValue("@vcPackinggroup", "");
                sqlCommand_sub.Parameters.AddWithValue("@dQty", "");
                sqlCommand_sub.Parameters.AddWithValue("@vcPackingpartslocation", "");
                sqlCommand_sub.Parameters.AddWithValue("@dPrintDate", "");
                sqlCommand_sub.Parameters.Add("@vcCodemage", SqlDbType.Image);
                #endregion
                foreach (DataRow item in dtSub.Rows)
                {
                    #region Value
                    sqlCommand_sub.Parameters["@UUID"].Value = item["UUID"].ToString();
                    sqlCommand_sub.Parameters["@vcTrolleyNo"].Value = item["vcTrolleyNo"].ToString();
                    sqlCommand_sub.Parameters["@vcLotid"].Value = item["vcLotid"].ToString();
                    sqlCommand_sub.Parameters["@vcPackingpartsno"].Value = item["vcPackingpartsno"].ToString();
                    sqlCommand_sub.Parameters["@vcPackinggroup"].Value = item["vcPackinggroup"].ToString();
                    sqlCommand_sub.Parameters["@dQty"].Value = item["dQty"].ToString();
                    sqlCommand_sub.Parameters["@vcPackingpartslocation"].Value = item["vcPackingpartslocation"].ToString();
                    sqlCommand_sub.Parameters["@dPrintDate"].Value = item["dPrintDate"].ToString();
                    sqlCommand_sub.Parameters["@vcCodemage"].Value = item["vcCodemage"];
                    #endregion
                    sqlCommand_sub.ExecuteNonQuery();
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
                dataRow["vcMessage"] = "数据写入打印数据失败！";
                dtMessage.Rows.Add(dataRow);
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
