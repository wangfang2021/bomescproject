using System;
using System.Text;
using System.Data;
using Common;
using System.Data.SqlClient;

namespace DataAccess
{
  public class P00002_DataAccess
  {
    private MultiExcute excute = new MultiExcute();

    public DataTable GetCheckType(string partId, string kanbanOrderNo, string kanbanSerial, string dock, string scanTime, string supplierId)
    {
      StringBuilder GetCheckTypeSql = new StringBuilder();
      GetCheckTypeSql.Append(" select vcCheckP,vcTJSX from tCheckQf where vcSupplierCode='" + supplierId + "' and  vcPartId='" + partId + "' and vcTimeFrom<='" + scanTime + "'and vcTimeTo>='" + scanTime + "'");
      SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();
      DataSet ds = new DataSet();
      try
      {
        ConnSql.Open();
        string strSQL = GetCheckTypeSql.ToString();
        SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
        da.Fill(ds);
        return ds.Tables[0];
      }
      catch (Exception ex)
      {
        throw ex;
      }
      finally
      {
        if (ConnectionState.Open == ConnSql.State)
        {
          ConnSql.Close();
        }
      }
    }

    public DataTable GetSPIS(string partId, string scanTime, string supplierId)
    {
      StringBuilder GetSPISSql = new StringBuilder();
      GetSPISSql.Append("select vcPicUrl from TSPISQf where vcSupplierCode='" + supplierId + "' and vcTimeFrom<='" + scanTime + "' and vcTimeTo>='" + scanTime + "' and vcPartId='" + partId + "'");
      SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();

      DataSet ds = new DataSet();
      try
      {
        ConnSql.Open();
        string strSQL = GetSPISSql.ToString();
        SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
        da.Fill(ds);
        return ds.Tables[0];
      }
      catch (Exception ex)
      {
        throw ex;
      }
      finally
      {
        if (ConnectionState.Open == ConnSql.State)
        {
          ConnSql.Close();
        }
      }
    }

    public DataTable GetInnoData(string inno)
    {
      StringBuilder GetInnoDataSql = new StringBuilder();

      GetInnoDataSql.Append("select vcPart_id,vcSR,iQuantity,vcKBOrderNo,vcKBLFNo from TOperateSJ where vcInputNo='" + inno + "' and vcZYType='S0'");
      SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();

      DataSet ds = new DataSet();
      try
      {
        ConnSql.Open();
        string strSQL = GetInnoDataSql.ToString();
        SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
        da.Fill(ds);
        return ds.Tables[0];
      }
      catch (Exception ex)
      {
        throw ex;
      }
      finally
      {
        if (ConnectionState.Open == ConnSql.State)
        {
          ConnSql.Close();
        }
      }
    }

    public DataTable getCheckInfo(string partId, string kanbanOrderNo, string kanbanSerial, string dock, string scanTime)
    {
    
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("select a.vcPackingPlant");
        stringBuilder.AppendLine(",a.vcPart_id");
        stringBuilder.AppendLine(",a.vcSupplier_id");
        stringBuilder.AppendLine(",a.vcBZPlant");
        stringBuilder.AppendLine(",a.vcInputNo");
        stringBuilder.AppendLine(",a.vcIOType");
        stringBuilder.AppendLine(",a.vcSupplierGQ");
        stringBuilder.AppendLine(",a.vcBZUnit");
        stringBuilder.AppendLine(",a.vcSHF");
        stringBuilder.AppendLine(",a.iQuantity");
        stringBuilder.AppendLine(",a.vcLabelStart");
        stringBuilder.AppendLine(",a.vcLabelEnd");
        stringBuilder.AppendLine(",b.vcPart_id as vcS1_check");
        stringBuilder.AppendLine(",c.vcPartId as vcPart_check");
        stringBuilder.AppendLine(",d.vcCheckP");
        stringBuilder.AppendLine(",d.vcTJSX");
        stringBuilder.AppendLine(",e.vcPicUrl");
        stringBuilder.AppendLine(",isnull(f.iSpotQty,0) as iSpotQty");
        stringBuilder.AppendLine("from ");
        stringBuilder.AppendLine("(select * from TOperateSJ ");
        stringBuilder.AppendLine("where vcPart_id='" + partId + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "' and vcSR='" + dock + "' and vcZYType='S0')a");
        stringBuilder.AppendLine("left join");
        stringBuilder.AppendLine("(select* from TOperateSJ ");
        stringBuilder.AppendLine("where vcPart_id='" + partId + "' and vcKBOrderNo='" + kanbanOrderNo + "' and vcKBLFNo='" + kanbanSerial + "' and vcSR='" + dock + "' and vcZYType='S1')b");
        stringBuilder.AppendLine("on a.vcPart_id=b.vcPart_id and a.vcKBOrderNo=b.vcKBOrderNo and a.vcKBLFNo=b.vcKBLFNo and a.vcSR=b.vcSR");
        stringBuilder.AppendLine("left join");
        stringBuilder.AppendLine("(select * from tCheckMethod_Master where vcPartId='" + partId + "' and dFromTime<='" + scanTime + "' and dToTime>='" + scanTime + "')c");
        stringBuilder.AppendLine("on a.vcPart_id=c.vcPartId and a.vcSupplier_id=c.vcSupplierId");
        stringBuilder.AppendLine("left join");
        stringBuilder.AppendLine("(SELECT * FROM [tCheckQf] ");
        stringBuilder.AppendLine("where [vcTimeFrom]<='" + scanTime + "'  and [vcTimeTo]>='" + scanTime + "' )d");
        stringBuilder.AppendLine("ON c.vcPartId=d.vcPartId AND c.vcSupplierId=d.[vcSupplierCode]");
        stringBuilder.AppendLine("left join");
        stringBuilder.AppendLine("(SELECT * FROM TSPISQf ");
        stringBuilder.AppendLine("where [vcTimeFrom]<='" + scanTime + "'  and [vcTimeTo]>='" + scanTime + "' )e");
        stringBuilder.AppendLine("ON c.vcPartId=e.vcPartId AND c.vcSupplierId=e.[vcSupplierCode]");
        stringBuilder.AppendLine("left join");
        stringBuilder.AppendLine("(select * from tCheckSpot)f");
        stringBuilder.AppendLine("on a.iQuantity=f.iPackingQty");
      SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();

      DataSet ds = new DataSet();
      try
      {
        ConnSql.Open();
        string strSQL = stringBuilder.ToString();
        SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
        da.Fill(ds);
        return ds.Tables[0];
      }
      catch (Exception ex)
      {
        throw ex;
      }
      finally
      {
        if (ConnectionState.Open == ConnSql.State)
        {
          ConnSql.Close();
        }
      }


    }

    public DataSet getNgReasonInfo()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("select vcNgReason from TNgReason");
      stringBuilder.AppendLine("select vcblame from TNG_Blame");
      SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();

      DataSet ds = new DataSet();
      try
      {
        ConnSql.Open();
        string strSQL = stringBuilder.ToString();
        SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
        da.Fill(ds);
        return ds;
      }
      catch (Exception ex)
      {
        throw ex;
      }
      finally
      {
        if (ConnectionState.Open == ConnSql.State)
        {
          ConnSql.Close();
        }
      }



    }
    public DataSet getTableInfoFromDB()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("--查询操作实际表结构");
      stringBuilder.AppendLine("SELECT TOP (1)*  FROM [TOperateSJ]");
      stringBuilder.AppendLine("--查询检查NG表结构");
      stringBuilder.AppendLine("SELECT TOP (1)*  FROM [TOperateSJ_NG]");
      SqlConnection ConnSql = Common.ComConnectionHelper.CreateSqlConnection();

      DataSet ds = new DataSet();
      try
      {
        ConnSql.Open();
        string strSQL = stringBuilder.ToString();
        SqlDataAdapter da = new SqlDataAdapter(strSQL, ConnSql);
        da.Fill(ds);
        return ds;
      }
      catch (Exception ex)
      {
        throw ex;
      }
      finally
      {
        if (ConnectionState.Open == ConnSql.State)
        {
          ConnSql.Close();
        }
      }
    }
    public bool setCheckInfo(DataTable dtInfo_SJ_Temp, DataTable dtInfo_NG_Temp)
    {
      SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();
      sqlConnection.Open();
      SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
      try
      {
        #region 写入数据库

        #region 2.sqlCommand_modinfo
        SqlCommand sqlCommand_modinfo = sqlConnection.CreateCommand();
        sqlCommand_modinfo.Transaction = sqlTransaction;
        sqlCommand_modinfo.CommandType = CommandType.Text;
        StringBuilder strSql_modinfo = new StringBuilder();

        #region SQL and Parameters
        strSql_modinfo.AppendLine("INSERT INTO [dbo].[TOperateSJ]");
        strSql_modinfo.AppendLine("           ([vcZYType]");
        strSql_modinfo.AppendLine("           ,[vcBZPlant]");
        strSql_modinfo.AppendLine("           ,[vcInputNo]");
        strSql_modinfo.AppendLine("           ,[vcKBOrderNo]");
        strSql_modinfo.AppendLine("           ,[vcKBLFNo]");
        strSql_modinfo.AppendLine("           ,[vcPart_id]");
        strSql_modinfo.AppendLine("           ,[vcIOType]");
        strSql_modinfo.AppendLine("           ,[vcSupplier_id]");
        strSql_modinfo.AppendLine("           ,[vcSupplierGQ]");
        strSql_modinfo.AppendLine("           ,[dStart]");
        strSql_modinfo.AppendLine("           ,[dEnd]");
        strSql_modinfo.AppendLine("           ,[iQuantity]");
        strSql_modinfo.AppendLine("           ,[vcBZUnit]");
        strSql_modinfo.AppendLine("           ,[vcSHF]");
        strSql_modinfo.AppendLine("           ,[vcSR]");
        strSql_modinfo.AppendLine("           ,[vcBoxNo]");
        strSql_modinfo.AppendLine("           ,[vcSheBeiNo]");
        strSql_modinfo.AppendLine("           ,[vcCheckType]");
        strSql_modinfo.AppendLine("           ,[iCheckNum]");
        strSql_modinfo.AppendLine("           ,[vcCheckStatus]");
        strSql_modinfo.AppendLine("           ,[vcLabelStart]");
        strSql_modinfo.AppendLine("           ,[vcLabelEnd]");
        strSql_modinfo.AppendLine("           ,[vcUnlocker]");
        strSql_modinfo.AppendLine("           ,[dUnlockTime]");
        strSql_modinfo.AppendLine("           ,[vcSellNo]");
        strSql_modinfo.AppendLine("           ,[vcOperatorID]");
        strSql_modinfo.AppendLine("           ,[dOperatorTime]");
        strSql_modinfo.AppendLine("           ,[vcHostIp]");
        strSql_modinfo.AppendLine("           ,[packingcondition]");
        strSql_modinfo.AppendLine("           ,[vcPackingPlant])");
        strSql_modinfo.AppendLine("     VALUES");
        strSql_modinfo.AppendLine("           (@vcZYType");
        strSql_modinfo.AppendLine("           ,@vcBZPlant");
        strSql_modinfo.AppendLine("           ,@vcInputNo");
        strSql_modinfo.AppendLine("           ,@vcKBOrderNo");
        strSql_modinfo.AppendLine("           ,@vcKBLFNo");
        strSql_modinfo.AppendLine("           ,@vcPart_id");
        strSql_modinfo.AppendLine("           ,@vcIOType");
        strSql_modinfo.AppendLine("           ,@vcSupplier_id");
        strSql_modinfo.AppendLine("           ,@vcSupplierGQ");
        strSql_modinfo.AppendLine("           ,@dStart");
        strSql_modinfo.AppendLine("           ,@dEnd");
        strSql_modinfo.AppendLine("           ,@iQuantity");
        strSql_modinfo.AppendLine("           ,@vcBZUnit");
        strSql_modinfo.AppendLine("           ,@vcSHF");
        strSql_modinfo.AppendLine("           ,@vcSR");
        strSql_modinfo.AppendLine("           ,@vcBoxNo");
        strSql_modinfo.AppendLine("           ,@vcSheBeiNo");
        strSql_modinfo.AppendLine("           ,@vcCheckType");
        strSql_modinfo.AppendLine("           ,@iCheckNum");
        strSql_modinfo.AppendLine("           ,@vcCheckStatus");
        strSql_modinfo.AppendLine("           ,@vcLabelStart");
        strSql_modinfo.AppendLine("           ,@vcLabelEnd");
        strSql_modinfo.AppendLine("           ,@vcUnlocker");
        strSql_modinfo.AppendLine("           ,null");
        strSql_modinfo.AppendLine("           ,@vcSellNo");
        strSql_modinfo.AppendLine("           ,@vcOperatorID");
        strSql_modinfo.AppendLine("           ,GETDATE()");
        strSql_modinfo.AppendLine("           ,@vcHostIp");
        strSql_modinfo.AppendLine("           ,@packingcondition");
        strSql_modinfo.AppendLine("           ,@vcPackingPlant)");
        sqlCommand_modinfo.CommandText = strSql_modinfo.ToString();
        sqlCommand_modinfo.Parameters.AddWithValue("@vcZYType", "");
        sqlCommand_modinfo.Parameters.AddWithValue("@vcBZPlant", "");
        sqlCommand_modinfo.Parameters.AddWithValue("@vcInputNo", "");
        sqlCommand_modinfo.Parameters.AddWithValue("@vcKBOrderNo", "");
        sqlCommand_modinfo.Parameters.AddWithValue("@vcKBLFNo", "");
        sqlCommand_modinfo.Parameters.AddWithValue("@vcPart_id", "");
        sqlCommand_modinfo.Parameters.AddWithValue("@vcIOType", "");
        sqlCommand_modinfo.Parameters.AddWithValue("@vcSupplier_id", "");
        sqlCommand_modinfo.Parameters.AddWithValue("@vcSupplierGQ", "");
        sqlCommand_modinfo.Parameters.AddWithValue("@dStart", "");
        sqlCommand_modinfo.Parameters.AddWithValue("@dEnd", "");
        sqlCommand_modinfo.Parameters.AddWithValue("@iQuantity", "");
        sqlCommand_modinfo.Parameters.AddWithValue("@vcBZUnit", "");
        sqlCommand_modinfo.Parameters.AddWithValue("@vcSHF", "");
        sqlCommand_modinfo.Parameters.AddWithValue("@vcSR", "");
        sqlCommand_modinfo.Parameters.AddWithValue("@vcBoxNo", "");
        sqlCommand_modinfo.Parameters.AddWithValue("@vcSheBeiNo", "");
        sqlCommand_modinfo.Parameters.AddWithValue("@vcCheckType", "");
        sqlCommand_modinfo.Parameters.AddWithValue("@iCheckNum", "");
        sqlCommand_modinfo.Parameters.AddWithValue("@vcCheckStatus", "");
        sqlCommand_modinfo.Parameters.AddWithValue("@vcLabelStart", "");
        sqlCommand_modinfo.Parameters.AddWithValue("@vcLabelEnd", "");
        sqlCommand_modinfo.Parameters.AddWithValue("@vcUnlocker", "");
        // sqlCommand_modinfo.Parameters.AddWithValue("@dUnlockTime", "");
        sqlCommand_modinfo.Parameters.AddWithValue("@vcSellNo", "");
        sqlCommand_modinfo.Parameters.AddWithValue("@vcOperatorID", "");
        sqlCommand_modinfo.Parameters.AddWithValue("@vcHostIp", "");
        sqlCommand_modinfo.Parameters.AddWithValue("@packingcondition", "");
        sqlCommand_modinfo.Parameters.AddWithValue("@vcPackingPlant", "");
        #endregion
        foreach (DataRow item in dtInfo_SJ_Temp.Rows)
        {
          #region Value
          sqlCommand_modinfo.Parameters["@vcZYType"].Value = item["vcZYType"].ToString();
          sqlCommand_modinfo.Parameters["@vcBZPlant"].Value = item["vcBZPlant"].ToString();
          sqlCommand_modinfo.Parameters["@vcInputNo"].Value = item["vcInputNo"].ToString();
          sqlCommand_modinfo.Parameters["@vcKBOrderNo"].Value = item["vcKBOrderNo"].ToString();
          sqlCommand_modinfo.Parameters["@vcKBLFNo"].Value = item["vcKBLFNo"].ToString();
          sqlCommand_modinfo.Parameters["@vcPart_id"].Value = item["vcPart_id"].ToString();
          sqlCommand_modinfo.Parameters["@vcIOType"].Value = item["vcIOType"].ToString();
          sqlCommand_modinfo.Parameters["@vcSupplier_id"].Value = item["vcSupplier_id"].ToString();
          sqlCommand_modinfo.Parameters["@vcSupplierGQ"].Value = item["vcSupplierGQ"].ToString();
          sqlCommand_modinfo.Parameters["@dStart"].Value = item["dStart"].ToString();
          sqlCommand_modinfo.Parameters["@dEnd"].Value = item["dEnd"].ToString();
          sqlCommand_modinfo.Parameters["@iQuantity"].Value = item["iQuantity"].ToString();
          sqlCommand_modinfo.Parameters["@vcBZUnit"].Value = item["vcBZUnit"].ToString();
          sqlCommand_modinfo.Parameters["@vcSHF"].Value = item["vcSHF"].ToString();
          sqlCommand_modinfo.Parameters["@vcSR"].Value = item["vcSR"].ToString();
          sqlCommand_modinfo.Parameters["@vcBoxNo"].Value = item["vcBoxNo"].ToString();
          sqlCommand_modinfo.Parameters["@vcSheBeiNo"].Value = item["vcSheBeiNo"].ToString();
          sqlCommand_modinfo.Parameters["@vcCheckType"].Value = item["vcCheckType"].ToString();
          sqlCommand_modinfo.Parameters["@iCheckNum"].Value = item["iCheckNum"].ToString();
          sqlCommand_modinfo.Parameters["@vcCheckStatus"].Value = item["vcCheckStatus"].ToString();
          sqlCommand_modinfo.Parameters["@vcLabelStart"].Value = item["vcLabelStart"].ToString();
          sqlCommand_modinfo.Parameters["@vcLabelEnd"].Value = item["vcLabelEnd"].ToString();
          sqlCommand_modinfo.Parameters["@vcUnlocker"].Value = item["vcUnlocker"].ToString();
          // sqlCommand_modinfo.Parameters["@dUnlockTime"].Value = item["dUnlockTime"].ToString();
          sqlCommand_modinfo.Parameters["@vcSellNo"].Value = item["vcSellNo"].ToString();
          sqlCommand_modinfo.Parameters["@vcOperatorID"].Value = item["vcOperatorID"].ToString();
          sqlCommand_modinfo.Parameters["@vcHostIp"].Value = item["vcHostIp"].ToString();
          sqlCommand_modinfo.Parameters["@packingcondition"].Value = item["packingcondition"].ToString();
          sqlCommand_modinfo.Parameters["@vcPackingPlant"].Value = item["vcPackingPlant"].ToString();
          #endregion
          sqlCommand_modinfo.ExecuteNonQuery();
        }
        #endregion

        #region 4.sqlCommand_modinfo_sp_add
        SqlCommand sqlCommand_modinfo_sp_add = sqlConnection.CreateCommand();
        sqlCommand_modinfo_sp_add.Transaction = sqlTransaction;
        sqlCommand_modinfo_sp_add.CommandType = CommandType.Text;
        StringBuilder strSql_modinfo_sp_add = new StringBuilder();

        #region SQL and Parameters
        strSql_modinfo_sp_add.AppendLine("INSERT INTO [dbo].[TOperateSJ_NG]");
        strSql_modinfo_sp_add.AppendLine("           ([vcPart_id]");
        strSql_modinfo_sp_add.AppendLine("           ,[vcKBOrderNo]");
        strSql_modinfo_sp_add.AppendLine("           ,[vcKBLFNo]");
        strSql_modinfo_sp_add.AppendLine("           ,[vcSR]");
        strSql_modinfo_sp_add.AppendLine("           ,[iNGQuantity]");
        strSql_modinfo_sp_add.AppendLine("           ,[vcNGReason]");
        strSql_modinfo_sp_add.AppendLine("           ,[vcZRBS]");
        strSql_modinfo_sp_add.AppendLine("           ,[vcOperatorID]");
        strSql_modinfo_sp_add.AppendLine("           ,[dOperatorTime])");
        strSql_modinfo_sp_add.AppendLine("     VALUES");
        strSql_modinfo_sp_add.AppendLine("           (@vcPart_id");
        strSql_modinfo_sp_add.AppendLine("           ,@vcKBOrderNo");
        strSql_modinfo_sp_add.AppendLine("           ,@vcKBLFNo");
        strSql_modinfo_sp_add.AppendLine("           ,@vcSR");
        strSql_modinfo_sp_add.AppendLine("           ,@iNGQuantity");
        strSql_modinfo_sp_add.AppendLine("           ,@vcNGReason");
        strSql_modinfo_sp_add.AppendLine("           ,@vcZRBS");
        strSql_modinfo_sp_add.AppendLine("           ,@vcOperatorID");
        strSql_modinfo_sp_add.AppendLine("           ,GETDATE())");
        sqlCommand_modinfo_sp_add.CommandText = strSql_modinfo_sp_add.ToString();
        sqlCommand_modinfo_sp_add.Parameters.AddWithValue("@vcPart_id", "");
        sqlCommand_modinfo_sp_add.Parameters.AddWithValue("@vcKBOrderNo", "");
        sqlCommand_modinfo_sp_add.Parameters.AddWithValue("@vcKBLFNo", "");
        sqlCommand_modinfo_sp_add.Parameters.AddWithValue("@vcSR", "");
        sqlCommand_modinfo_sp_add.Parameters.AddWithValue("@iNGQuantity", "");
        sqlCommand_modinfo_sp_add.Parameters.AddWithValue("@vcNGReason", "");
        sqlCommand_modinfo_sp_add.Parameters.AddWithValue("@vcZRBS", "");
        sqlCommand_modinfo_sp_add.Parameters.AddWithValue("@vcOperatorID", "");
        #endregion
        foreach (DataRow item in dtInfo_NG_Temp.Rows)
        {
          #region Value
          sqlCommand_modinfo_sp_add.Parameters["@vcPart_id"].Value = item["vcPart_id"].ToString();
          sqlCommand_modinfo_sp_add.Parameters["@vcKBOrderNo"].Value = item["vcKBOrderNo"].ToString();
          sqlCommand_modinfo_sp_add.Parameters["@vcKBLFNo"].Value = item["vcKBLFNo"].ToString();
          sqlCommand_modinfo_sp_add.Parameters["@vcSR"].Value = item["vcSR"].ToString();
          sqlCommand_modinfo_sp_add.Parameters["@iNGQuantity"].Value = item["iNGQuantity"].ToString();
          sqlCommand_modinfo_sp_add.Parameters["@vcNGReason"].Value = item["vcNGReason"].ToString();
          sqlCommand_modinfo_sp_add.Parameters["@vcZRBS"].Value = item["vcZRBS"].ToString();
          sqlCommand_modinfo_sp_add.Parameters["@vcOperatorID"].Value = item["vcOperatorID"].ToString();
          #endregion
          sqlCommand_modinfo_sp_add.ExecuteNonQuery();
        }
        #endregion

        //提交事务
        sqlTransaction.Commit();
        sqlConnection.Close();
        return true;
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
        return false;
      }
    }

  }
}
