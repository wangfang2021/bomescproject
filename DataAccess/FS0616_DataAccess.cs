using System;
using Common;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using DataEntity;
using System.Collections.Generic;

namespace DataAccess
{
    public class FS0616_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        public DataTable getFormOptions()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select distinct isnull(t1.vcOrderNo,'--') as vcOrderNo_Value");
                strSql.AppendLine("		,isnull(t1.vcOrderNo,'--') as vcOrderNo_Name");
                strSql.AppendLine("		,isnull(t1.vcHaoJiu,'--') as vcHaoJiu_Value");
                strSql.AppendLine("		,isnull(t3.vcName,'--') as vcHaoJiu_Name");
                strSql.AppendLine("		,isnull(t1.vcOrderPlant,'--') as vcOrderPlant_Value");
                strSql.AppendLine("		,isnull(t4.vcName,'--') as vcOrderPlant_Name");
                strSql.AppendLine("		,isnull(t1.vcSupplier_id,'--') as vcSupplierId_Value");
                strSql.AppendLine("		,isnull(t1.vcSupplier_id,'--') as vcSupplierId_Name");
                strSql.AppendLine("		,isnull(t1.vcGQ,'--') as vcSupplierPlant_Value");
                strSql.AppendLine("		,isnull(t1.vcGQ,'--') as vcSupplierPlant_Name");
                strSql.AppendLine("		,isnull(CONVERT(VARCHAR(10),t1.dReplyOverDate,111),'--') as vcReplyOverDate_Value");
                strSql.AppendLine("		,isnull(CONVERT(VARCHAR(10),t1.dReplyOverDate,111),'--') as vcReplyOverDate_Name");
                strSql.AppendLine("		,isnull(CONVERT(VARCHAR(10),t2.dOutPutDate,111),'--') as vcOutPutDate_Value");
                strSql.AppendLine("		,isnull(CONVERT(VARCHAR(10),t2.dOutPutDate,111),'--') as vcOutPutDate_Name");
                strSql.AppendLine("		from ");
                strSql.AppendLine("(select * from TUrgentOrder");
                strSql.AppendLine("where vcShowFlag='1'");
                strSql.AppendLine(")t1");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select * from VI_UrgentOrder_OperHistory where cast(isnull(iDuiYingQuantity,0) as decimal(16,2))<>0)t2");
                strSql.AppendLine("ON T1.vcOrderNo=T2.vcOrderNo AND T1.vcPart_id=T2.vcPart_id AND T1.vcSupplier_id=T2.vcSupplier_id");
                strSql.AppendLine("left join");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C004')t3");
                strSql.AppendLine("on t1.vcHaoJiu=t3.vcValue");
                strSql.AppendLine("left join");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C000')t4");
                strSql.AppendLine("on t1.vcOrderPlant=t4.vcValue");
                strSql.AppendLine("left join");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C012')t5");
                strSql.AppendLine("on t1.vcOESP=t5.vcValue");
                strSql.AppendLine("left join");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C056')t6");
                strSql.AppendLine("on t1.vcStatus=t6.vcValue");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getSearchInfo(string strState, string strOrderNoList, string strPartId, string strHaoJiu, string strOrderPlant, string strSupplierId, string strSupplierPlant, string strReplyOverDate, string strOutPutDate)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select t1.iAutoId as LinId,t2.iAutoId as LinId_sub");
                strSql.AppendLine("		,t6.vcName as vcState_name");
                strSql.AppendLine("		,t1.vcStatus as vcState");
                strSql.AppendLine("		,t1.vcOrderNo as vcOrderNo");
                strSql.AppendLine("		,t1.vcPart_id as vcPart_id");
                strSql.AppendLine("		,t4.vcName as vcOrderPlant");
                strSql.AppendLine("		,t3.vcName as vcHaoJiu");
                strSql.AppendLine("		,t5.vcName as vcOESP");
                strSql.AppendLine("		,t1.vcSupplier_id as vcSupplierId");
                strSql.AppendLine("		,t1.vcGQ as vcSupplierPlant");
                strSql.AppendLine("		,t1.vcChuHePlant as vcSupplierPlace");
                strSql.AppendLine("		,t1.vcSufferIn as vcSufferIn");
                strSql.AppendLine("		,t1.iPackingQty as iPackingQty");
                strSql.AppendLine("		,t1.iOrderQuantity as iOrderQuantity");
                strSql.AppendLine("		,t2.iDuiYingQuantity as iDuiYingQuantity");
                strSql.AppendLine("		,t2.decBoxQuantity as decBoxQuantity");
                strSql.AppendLine("		,CONVERT(VARCHAR(10),t2.dDeliveryDate,111) as dDeliveryDate");
                strSql.AppendLine("		,CONVERT(VARCHAR(10),t2.dOutPutDate,111) as dOutPutDate");
                strSql.AppendLine("		,CONVERT(VARCHAR(10),t1.dReplyOverDate,111) as dReplyOverDate");
                strSql.AppendLine("		,CONVERT(VARCHAR(10),t1.dSupReplyTime,111) as dSupReplyTime ");
                strSql.AppendLine("		,CASE WHEN t2.decBoxQuantity IS NULL THEN '0' ELSE CASE WHEN CAST(t2.decBoxQuantity AS INT)=t2.decBoxQuantity THEN '0' ELSE '1' END END AS vcBoxColor");
                strSql.AppendLine("		,CASE WHEN t7.iDuiYingSum IS NULL THEN '0' ELSE CASE WHEN CAST(t7.iDuiYingSum AS INT)=CAST(t1.iOrderQuantity as int) THEN '0' ELSE '1' END END AS vcDuiYingColor");
                strSql.AppendLine("		,'' as vcBgColor,'0' as bModFlag,'0' as bAddFlag");
                strSql.AppendLine("		,CASE WHEN t1.vcStatus='3' THEN '0' ELSE '1' END as bSelectFlag");
                strSql.AppendLine("		from ");
                strSql.AppendLine("(select * from TUrgentOrder");
                strSql.AppendLine("where vcShowFlag='1'");
                if (strState != "")
                {
                    strSql.AppendLine("AND [vcStatus]='" + strState + "'");
                }
                if (strOrderNoList != "")
                {
                    strSql.AppendLine("AND [vcOrderNo] IN (" + strOrderNoList + ")");
                }
                if (strPartId != "")
                {
                    strSql.AppendLine("AND [vcPart_id] LIKE '" + strPartId + "%'");
                }
                if (strHaoJiu != "")
                {
                    if (strHaoJiu == "--")
                    {
                        strSql.AppendLine("AND isnull(vcHaoJiu,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("AND [vcHaoJiu]='" + strHaoJiu + "'");
                    }
                }
                if (strOrderPlant != "")
                {
                    if (strOrderPlant == "--")
                    {
                        strSql.AppendLine("AND isnull(vcOrderPlant,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("AND [vcOrderPlant]='" + strOrderPlant + "'");
                    }
                }
                if (strSupplierId != "")
                {
                    if (strSupplierId == "--")
                    {
                        strSql.AppendLine("AND isnull(vcSupplier_id,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("AND [vcSupplier_id]='" + strSupplierId + "'");
                    }
                }
                if (strSupplierPlant != "")
                {
                    if (strSupplierPlant == "--")
                    {
                        strSql.AppendLine("AND isnull(vcGQ,'')=''");
                    }
                    else
                    {
                        strSql.AppendLine("AND [vcGQ]='" + strSupplierPlant + "'");
                    }
                }
                if (strReplyOverDate != "")
                {
                    if (strReplyOverDate == "--")
                    {
                        strSql.AppendLine("AND dReplyOverDate is null");
                    }
                    else
                    {
                        strSql.AppendLine("AND CONVERT(VARCHAR(10),dReplyOverDate,111)='" + strReplyOverDate + "'");
                    }
                }
                strSql.AppendLine(")t1");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select * from VI_UrgentOrder_OperHistory where cast(isnull(iDuiYingQuantity,0) as decimal(16,2))<>0 )t2");
                strSql.AppendLine("ON T1.vcOrderNo=T2.vcOrderNo AND T1.vcPart_id=T2.vcPart_id AND T1.vcSupplier_id=T2.vcSupplier_id");
                strSql.AppendLine("left join");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C004')t3");
                strSql.AppendLine("on t1.vcHaoJiu=t3.vcValue");
                strSql.AppendLine("left join");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C000')t4");
                strSql.AppendLine("on t1.vcOrderPlant=t4.vcValue");
                strSql.AppendLine("left join");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C012')t5");
                strSql.AppendLine("on t1.vcOESP=t5.vcValue");
                strSql.AppendLine("left join");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C056')t6");
                strSql.AppendLine("on t1.vcStatus=t6.vcValue");
                strSql.AppendLine("left join");
                strSql.AppendLine("(select vcOrderNo,vcPart_id,vcSupplier_id,sum(CAST(iDuiYingQuantity as int)) as iDuiYingSum from VI_UrgentOrder_OperHistory where cast(isnull(iDuiYingQuantity,0) as decimal(16,2))<>0");
                strSql.AppendLine("group by vcOrderNo,vcPart_id,vcSupplier_id)t7");
                strSql.AppendLine("ON T1.vcOrderNo=t7.vcOrderNo AND T1.vcPart_id=t7.vcPart_id AND T1.vcSupplier_id=t7.vcSupplier_id");
                if (strOutPutDate != "")
                {
                    if (strOutPutDate == "--")
                    {
                        strSql.AppendLine("AND t2.dOutPutDate is null");
                    }
                    else
                    {
                        strSql.AppendLine("AND CONVERT(VARCHAR(10),t2.dOutPutDate,111)='" + strOutPutDate + "'");
                    }
                }
                strSql.AppendLine("ORDER BY T1.vcOrderNo,t1.iAutoId,t2.iAutoId");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int getManualCode()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select vcValue1 from TOutCode where vcIsColum=0 and vcCodeId='C011'");
                DataTable dataTable = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                if(dataTable.Rows.Count!=0)
                {
                    return Convert.ToInt32(dataTable.Rows[0]["vcValue1"].ToString());
                }
                return 0;
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
                string strNow = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
                SqlCommand sqlCommand_modinfo = sqlConnection.CreateCommand();
                sqlCommand_modinfo.Transaction = sqlTransaction;
                sqlCommand_modinfo.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo = new StringBuilder();
                strSql_modinfo.AppendLine("UPDATE TUrgentOrder SET vcSaveFlag='1',vcSaveer='" + strOperId + "',dSaveTime='" + strNow + "' WHERE [vcOrderNo]=@vcOrderNo AND vcPart_id=@vcPart_id AND vcSupplier_id=@vcSupplier_id");
                strSql_modinfo.AppendLine("INSERT INTO [dbo].[TUrgentOrder_OperHistory]");
                strSql_modinfo.AppendLine("           ([vcOrderNo]");
                strSql_modinfo.AppendLine("           ,[vcPart_id]");
                strSql_modinfo.AppendLine("           ,[vcSupplier_id]");
                strSql_modinfo.AppendLine("           ,[iDuiYingQuantity]");
                strSql_modinfo.AppendLine("           ,[decBoxQuantity]");
                strSql_modinfo.AppendLine("           ,[dDeliveryDate]");
                strSql_modinfo.AppendLine("           ,[dOutPutDate]");
                strSql_modinfo.AppendLine("           ,[vcInputType]");
                strSql_modinfo.AppendLine("           ,[vcOperatorID]");
                strSql_modinfo.AppendLine("           ,[dOperatorTime])");
                strSql_modinfo.AppendLine("     VALUES");
                strSql_modinfo.AppendLine("           (@vcOrderNo");
                strSql_modinfo.AppendLine("           ,@vcPart_id");
                strSql_modinfo.AppendLine("           ,@vcSupplier_id");
                strSql_modinfo.AppendLine("           ,@iDuiYingQuantity");
                strSql_modinfo.AppendLine("           ,@decBoxQuantity");
                strSql_modinfo.AppendLine("           ,CASE WHEN @dDeliveryDate='' THEN NULL ELSE @dDeliveryDate END");
                strSql_modinfo.AppendLine("           ,CASE WHEN @dOutPutDate='' THEN NULL ELSE @dOutPutDate END");
                strSql_modinfo.AppendLine("           ,@vcInputType");
                strSql_modinfo.AppendLine("           ,'" + strOperId + "'");
                strSql_modinfo.AppendLine("           ,'" + strNow + "')");
                sqlCommand_modinfo.CommandText = strSql_modinfo.ToString();
                sqlCommand_modinfo.Parameters.AddWithValue("@vcOrderNo", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcPart_id", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcSupplier_id", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@iDuiYingQuantity", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@decBoxQuantity", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@dDeliveryDate", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@dOutPutDate", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcInputType", "");
                foreach (DataRow item in dtImport.Rows)
                {
                    sqlCommand_modinfo.Parameters["@vcOrderNo"].Value = item["vcOrderNo"].ToString();
                    sqlCommand_modinfo.Parameters["@vcPart_id"].Value = item["vcPart_id"].ToString();
                    sqlCommand_modinfo.Parameters["@vcSupplier_id"].Value = item["vcSupplierId"].ToString();
                    sqlCommand_modinfo.Parameters["@iDuiYingQuantity"].Value = item["iDuiYingQuantity"].ToString();
                    sqlCommand_modinfo.Parameters["@decBoxQuantity"].Value = item["decBoxQuantity"].ToString();
                    sqlCommand_modinfo.Parameters["@dDeliveryDate"].Value = item["dDeliveryDate"].ToString();
                    sqlCommand_modinfo.Parameters["@dOutPutDate"].Value = item["dOutPutDate"].ToString();
                    sqlCommand_modinfo.Parameters["@vcInputType"].Value = item["vcInputType"].ToString();
                    sqlCommand_modinfo.ExecuteNonQuery();
                }
                //提交事务
                sqlTransaction.Commit();
                sqlConnection.Close();

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

        public void setReplyInfo(DataTable dtImport, string strOperId, ref DataTable dtMessage)
        {
            SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();

            sqlConnection.Open();
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            try
            {
                string strNow = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
                SqlCommand sqlCommand_modinfo = sqlConnection.CreateCommand();
                sqlCommand_modinfo.Transaction = sqlTransaction;
                sqlCommand_modinfo.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo = new StringBuilder();
                strSql_modinfo.AppendLine("UPDATE TUrgentOrder SET vcStatus='3',vcSaveFlag='1',vcSaveer='" + strOperId + "',dSaveTime='" + strNow + "',[vcReplyer]='" + strOperId + "',[dReplyTime]='"+ " + strNow + " + "' WHERE [vcOrderNo]=@vcOrderNo AND vcPart_id=@vcPart_id AND vcSupplier_id=@vcSupplier_id");
                strSql_modinfo.AppendLine("INSERT INTO [dbo].[TUrgentOrder_OperHistory]");
                strSql_modinfo.AppendLine("           ([vcOrderNo]");
                strSql_modinfo.AppendLine("           ,[vcPart_id]");
                strSql_modinfo.AppendLine("           ,[vcSupplier_id]");
                strSql_modinfo.AppendLine("           ,[iDuiYingQuantity]");
                strSql_modinfo.AppendLine("           ,[decBoxQuantity]");
                strSql_modinfo.AppendLine("           ,[dDeliveryDate]");
                strSql_modinfo.AppendLine("           ,[dOutPutDate]");
                strSql_modinfo.AppendLine("           ,[vcInputType]");
                strSql_modinfo.AppendLine("           ,[vcOperatorID]");
                strSql_modinfo.AppendLine("           ,[dOperatorTime])");
                strSql_modinfo.AppendLine("     VALUES");
                strSql_modinfo.AppendLine("           (@vcOrderNo");
                strSql_modinfo.AppendLine("           ,@vcPart_id");
                strSql_modinfo.AppendLine("           ,@vcSupplier_id");
                strSql_modinfo.AppendLine("           ,@iDuiYingQuantity");
                strSql_modinfo.AppendLine("           ,@decBoxQuantity");
                strSql_modinfo.AppendLine("           ,CASE WHEN @dDeliveryDate='' THEN NULL ELSE @dDeliveryDate END");
                strSql_modinfo.AppendLine("           ,CASE WHEN @dOutPutDate='' THEN NULL ELSE @dOutPutDate END");
                strSql_modinfo.AppendLine("           ,@vcInputType");
                strSql_modinfo.AppendLine("           ,'" + strOperId + "'");
                strSql_modinfo.AppendLine("           ,'" + strNow + "')");
                sqlCommand_modinfo.CommandText = strSql_modinfo.ToString();
                sqlCommand_modinfo.Parameters.AddWithValue("@vcOrderNo", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcPart_id", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcSupplier_id", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@iDuiYingQuantity", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@decBoxQuantity", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@dDeliveryDate", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@dOutPutDate", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcInputType", "");
                foreach (DataRow item in dtImport.Rows)
                {
                    sqlCommand_modinfo.Parameters["@vcOrderNo"].Value = item["vcOrderNo"].ToString();
                    sqlCommand_modinfo.Parameters["@vcPart_id"].Value = item["vcPart_id"].ToString();
                    sqlCommand_modinfo.Parameters["@vcSupplier_id"].Value = item["vcSupplierId"].ToString();
                    sqlCommand_modinfo.Parameters["@iDuiYingQuantity"].Value = item["iDuiYingQuantity"].ToString();
                    sqlCommand_modinfo.Parameters["@decBoxQuantity"].Value = item["decBoxQuantity"].ToString();
                    sqlCommand_modinfo.Parameters["@dDeliveryDate"].Value = item["dDeliveryDate"].ToString();
                    sqlCommand_modinfo.Parameters["@dOutPutDate"].Value = item["dOutPutDate"].ToString();
                    sqlCommand_modinfo.Parameters["@vcInputType"].Value = item["vcInputType"].ToString();
                    sqlCommand_modinfo.ExecuteNonQuery();
                }
                //提交事务
                sqlTransaction.Commit();
                sqlConnection.Close();

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

        public void setOpenInfo(DataTable dtImport, string strOperId, ref DataTable dtMessage)
        {
            SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();

            sqlConnection.Open();
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            try
            {
                SqlCommand sqlCommand_modinfo = sqlConnection.CreateCommand();
                sqlCommand_modinfo.Transaction = sqlTransaction;
                sqlCommand_modinfo.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo = new StringBuilder();
                strSql_modinfo.AppendLine("UPDATE [TUrgentOrder] SET  vcStatus='1',[dReplyOverDate]=@dReplyOverDate ,[vcSender]='" + strOperId + "',[dSendTime]=GETDATE() WHERE [vcOrderNo]=@vcOrderNo AND [vcPart_id]=@vcPart_id AND [vcSupplier_id]=@vcSupplier_id");
                sqlCommand_modinfo.CommandText = strSql_modinfo.ToString();
                sqlCommand_modinfo.Parameters.AddWithValue("@dReplyOverDate", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcOrderNo", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcPart_id", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcSupplier_id", "");
                foreach (DataRow item in dtImport.Rows)
                {
                    sqlCommand_modinfo.Parameters["@dReplyOverDate"].Value = item["dReplyOverDate"].ToString();
                    sqlCommand_modinfo.Parameters["@vcOrderNo"].Value = item["vcOrderNo"].ToString();
                    sqlCommand_modinfo.Parameters["@vcPart_id"].Value = item["vcPart_id"].ToString();
                    sqlCommand_modinfo.Parameters["@vcSupplier_id"].Value = item["vcSupplierId"].ToString();
                    sqlCommand_modinfo.ExecuteNonQuery();
                }
                //提交事务
                sqlTransaction.Commit();
                sqlConnection.Close();

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

        public void setOutputInfo(DataTable dtImport, string strOperId, ref DataTable dtMessage)
        {
            SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();

            sqlConnection.Open();
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            try
            {
                string strNow = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
                SqlCommand sqlCommand_modinfo = sqlConnection.CreateCommand();
                sqlCommand_modinfo.Transaction = sqlTransaction;
                sqlCommand_modinfo.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo = new StringBuilder();
                strSql_modinfo.AppendLine("UPDATE TUrgentOrder SET vcSaveFlag='1',vcSaveer='" + strOperId + "',dSaveTime='" + strNow + "' WHERE [vcOrderNo]=@vcOrderNo AND vcPart_id=@vcPart_id AND vcSupplier_id=@vcSupplier_id");
                strSql_modinfo.AppendLine("UPDATE TUrgentOrder_OperHistory SET [dOutPutDate] = CASE WHEN @dOutPutDate='' THEN NULL ELSE @dOutPutDate END,[vcOperatorID] = '" + strOperId + "',[dOperatorTime] = '" + strNow + "'");
                strSql_modinfo.AppendLine("WHERE [iAutoId]=@LinId");
                sqlCommand_modinfo.CommandText = strSql_modinfo.ToString();
                sqlCommand_modinfo.Parameters.AddWithValue("@LinId", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcOrderNo", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcPart_id", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcSupplier_id", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@dOutPutDate", "");
                foreach (DataRow item in dtImport.Rows)
                {
                    sqlCommand_modinfo.Parameters["@LinId"].Value = item["LinId"].ToString();
                    sqlCommand_modinfo.Parameters["@vcOrderNo"].Value = item["vcOrderNo"].ToString();
                    sqlCommand_modinfo.Parameters["@vcPart_id"].Value = item["vcPart_id"].ToString();
                    sqlCommand_modinfo.Parameters["@vcSupplier_id"].Value = item["vcSupplierId"].ToString();
                    sqlCommand_modinfo.Parameters["@dOutPutDate"].Value = item["dOutPutDate"].ToString();
                    sqlCommand_modinfo.ExecuteNonQuery();
                }
                //提交事务
                sqlTransaction.Commit();
                sqlConnection.Close();

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
    }
}