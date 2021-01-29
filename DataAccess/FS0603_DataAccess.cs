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
    public class FS0603_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        public DataTable getCodeInfo(string strType)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                if (strType == "Receiver")//收货方
                {
                    strSql.AppendLine("select vcValue,vcName from TCustomerInfo where vcDisting='C' and vcDisable='0'");
                }
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getSearchInfo(string strSyncTime, string strPartId, string strCarModel, string strReceiver, string strInOut,
                    string strSupplierId, string strSupplierPlant, string strFromTime, string strToTime, string strHaoJiu, string strOrderPlant)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("SELECT T1.LinId,Convert(varchar(10),T1.dSyncTime,23) as dSyncTime,");
                strSql.AppendLine("T1.vcChanges,T11.vcName as vcChanges_name,T1.vcPackingPlant,T12.vcName as vcPackingPlant_name,");
                strSql.AppendLine("T1.vcPartId,T1.vcPartENName,T1.vcCarfamilyCode,T1.vcReceiver,T7.vcName AS vcReceiver_name,");
                strSql.AppendLine("Convert(varchar(10),T1.dFromTime,23) as dFromTime,Convert(varchar(10),T1.dToTime,23) as dToTime,");
                strSql.AppendLine("T1.vcPartId_Replace,T1.vcInOut,t8.vcName as vcInOut_name,T1.vcOESP,T13.vcName as vcOESP_name,");
                strSql.AppendLine("T1.vcHaoJiu,T9.vcName as vcHaoJiu_name,T1.vcOldProduction,T14.vcName as vcOldProduction_name,");
                strSql.AppendLine("Convert(varchar(10),T1.dOldStartTime,23) as dOldStartTime,CONVERT(VARCHAR(7),T1.dDebugTime,23) AS dDebugTime,");
                strSql.AppendLine("T1.vcSupplierId,Convert(varchar(10),T1.dSupplierFromTime,23) as dSupplierFromTime,");
                strSql.AppendLine("Convert(varchar(10),T1.dSupplierToTime,23) as dSupplierToTime,T1.vcSupplierName,");
                strSql.AppendLine("T2.vcSupplierPlant,");
                strSql.AppendLine("");
                strSql.AppendLine("T2.vcSupplierPlant as SupplierPlant_ed,");
                strSql.AppendLine("T2.LinId as SupplierPlantLinId_ed,");
                strSql.AppendLine("Convert(varchar(10),T2.dFromTime,23) as SupplierPlantFromTime_ed,");
                strSql.AppendLine("Convert(varchar(10),t2.dToTime,23) as SupplierPlantToTime_ed,");
                strSql.AppendLine("");
                strSql.AppendLine("T1.vcSupplierPlace,");
                strSql.AppendLine("");
                strSql.AppendLine("CAST(T3.iPackingQty AS varchar(10)) AS iPackingQty,");
                strSql.AppendLine("T3.vcBoxType,T3.iLength,T3.iWidth,T3.iHeight,T3.iVolume,");
                strSql.AppendLine("");
                strSql.AppendLine("CAST(T3.iPackingQty AS varchar(10)) AS BoxPackingQty_ed,");
                strSql.AppendLine("T3.LinId as BoxLinId_ed,");
                strSql.AppendLine("Convert(varchar(10),T3.dFromTime,23) as BoxFromTime_ed,");
                strSql.AppendLine("Convert(varchar(10),T3.dToTime,23) as BoxToTime_ed,");
                strSql.AppendLine("T3.vcBoxType as BoxType_ed,T3.iLength as BoxLength_ed,T3.iWidth as BoxWidth_ed,T3.iHeight as BoxHeight_ed,T3.iVolume as BoxVolume_ed,");
                strSql.AppendLine("");
                strSql.AppendLine("T4.vcSufferIn,");
                strSql.AppendLine("");
                strSql.AppendLine("T4.vcSufferIn as SufferIn_ed,");
                strSql.AppendLine("T4.LinId as SufferInLinId_ed,");
                strSql.AppendLine("Convert(varchar(10),T4.dFromTime,23) as SufferInFromTime_ed,");
                strSql.AppendLine("Convert(varchar(10),T4.dToTime,23) as SufferInToTime_ed,");
                strSql.AppendLine("");
                strSql.AppendLine("T5.vcOrderPlant,");
                strSql.AppendLine("");
                strSql.AppendLine("T5.vcOrderPlant as OrderPlant_ed,");
                strSql.AppendLine("T5.LinId as OrderPlantLinId_ed,");
                strSql.AppendLine("Convert(varchar(10),T5.dFromTime,23) as OrderPlantFromTime_ed,");
                strSql.AppendLine("Convert(varchar(10),T5.dToTime,23) as OrderPlantToTime_ed,");
                strSql.AppendLine("");
                strSql.AppendLine("T10.vcName as vcOrderPlant_name,T1.vcInteriorProject,T1.vcPassProject,T1.vcFrontProject,");
                strSql.AppendLine("Convert(varchar(10),T1.dFrontProjectTime,23) as dFrontProjectTime,Convert(varchar(10),T1.dShipmentTime,23) as dShipmentTime,");
                strSql.AppendLine("T6.vcImagePath AS vcPartImage,T1.vcBillType,T15.vcName as vcBillType_name,T1.vcRemark1,T1.vcRemark2,");
                strSql.AppendLine("T1.vcOrderingMethod,T16.vcName as vcOrderingMethod_name,T1.vcMandOrder,T17.vcName as vcMandOrder_name,");
                strSql.AppendLine("'0' as bModFlag,'0' as bAddFlag,'' as vcBgColor  FROM ");
                strSql.AppendLine("(SELECT * FROM [dbo].[TSPMaster]");
                strSql.AppendLine("  WHERE 1=1 ");
                if (strSyncTime != "")
                {
                    strSql.AppendLine("AND CONVERT(varchar(10),[dSyncTime],23)='" + strSyncTime + "'");
                }
                if (strPartId != "")
                {
                    strSql.AppendLine("AND [vcPartId]='" + strPartId + "'");
                }
                if (strCarModel != "")
                {
                    strSql.AppendLine("AND [vcCarModel]='" + strCarModel + "'");
                }
                if (strReceiver != "")
                {
                    strSql.AppendLine("AND [vcReceiver]='" + strReceiver + "'");
                }
                if (strInOut != "")
                {
                    strSql.AppendLine("AND [vcInOut]='" + strInOut + "'");
                }
                if (strSupplierId != "")
                {
                    strSql.AppendLine("AND [vcSupplierId]='" + strSupplierId + "'");
                }
                if (strFromTime != "")
                {
                    strSql.AppendLine("AND [dFromTime]='" + strFromTime + "'");
                }
                if (strToTime != "")
                {
                    strSql.AppendLine("AND [dToTime]='" + strToTime + "'");
                }
                if (strHaoJiu != "")
                {
                    strSql.AppendLine("AND [vcHaoJiu]='" + strHaoJiu + "'");
                }
                strSql.AppendLine("  )T1");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT *  FROM [dbo].[TSPMaster_SupplierPlant] ");
                strSql.AppendLine("  WHERE [vcOperatorType]='1' AND [dFromTime]<=GETDATE() AND [dToTime]>=GETDATE())T2");
                strSql.AppendLine("ON T1.[vcPackingPlant]=T2.[vcPackingPlant] AND T1.[vcPartId]=T2.[vcPartId] ");
                strSql.AppendLine("AND T1.[vcReceiver]=T2.[vcReceiver] AND T1.[vcSupplierId]=T2.[vcSupplierId]");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT *  FROM [dbo].[TSPMaster_Box]");
                strSql.AppendLine("  WHERE [vcOperatorType]='1' AND [dFromTime]<=GETDATE() AND [dToTime]>=GETDATE())T3");
                strSql.AppendLine("ON T1.[vcPackingPlant]=T3.[vcPackingPlant] AND T1.[vcPartId]=T3.[vcPartId] ");
                strSql.AppendLine("AND T1.[vcReceiver]=T3.[vcReceiver] AND T1.[vcSupplierId]=T3.[vcSupplierId] AND T2.vcSupplierPlant=T3.[vcSupplierPlant]");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT *  FROM [dbo].[TSPMaster_SufferIn]");
                strSql.AppendLine("  WHERE [vcOperatorType]='1' AND [dFromTime]<=GETDATE() AND [dToTime]>=GETDATE())T4");
                strSql.AppendLine("ON T1.[vcPackingPlant]=T4.[vcPackingPlant] AND T1.[vcPartId]=T4.[vcPartId] ");
                strSql.AppendLine("AND T1.[vcReceiver]=T4.[vcReceiver] AND T1.[vcSupplierId]=T4.[vcSupplierId] ");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT *  FROM [dbo].[TSPMaster_OrderPlant]");
                strSql.AppendLine("  WHERE [vcOperatorType]='1' AND [dFromTime]<=GETDATE() AND [dToTime]>=GETDATE())T5");
                strSql.AppendLine("ON T1.[vcPackingPlant]=T5.[vcPackingPlant] AND T1.[vcPartId]=T5.[vcPartId] ");
                strSql.AppendLine("AND T1.[vcReceiver]=T5.[vcReceiver] AND T1.[vcSupplierId]=T5.[vcSupplierId] ");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT *  FROM [dbo].[TSPMaster_ImagePath]");
                strSql.AppendLine("  WHERE [vcOperatorType]='1' AND [dFromTime]<=GETDATE() AND [dToTime]>=GETDATE())T6");
                strSql.AppendLine("ON T1.[vcPartId]=T6.[vcPartId] ");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcValue,vcName FROM TCustomerInfo WHERE vcDisting='C' and vcDisable='0')T7--收货方");
                strSql.AppendLine("ON T1.vcReceiver=T7.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C003')T8--内外区分");
                strSql.AppendLine("ON T1.vcInOut=T8.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C004')T9--号旧区分");
                strSql.AppendLine("ON T1.vcHaoJiu=T9.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C000')T10--发注工厂");
                strSql.AppendLine("ON T5.vcOrderPlant=T10.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C002')T11--变更事项");
                strSql.AppendLine("ON T1.vcChanges=T11.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C051')T12--包装工厂");
                strSql.AppendLine("ON T1.vcPackingPlant=T12.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C012')T13--OE=SP");
                strSql.AppendLine("ON T1.vcOESP=T13.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C024')T14--旧型年限生产区分");
                strSql.AppendLine("ON T1.vcOldProduction=T14.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C007')T15--单据区分");
                strSql.AppendLine("ON T1.vcBillType=T15.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C047')T16--订货方式");
                strSql.AppendLine("ON T1.vcOrderingMethod=T16.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C048')T17--强制订货");
                strSql.AppendLine("ON T1.vcMandOrder=T17.vcValue");
                strSql.AppendLine("WHERE 1=1");
                if (strSupplierPlant != "")
                {
                    strSql.AppendLine("AND T2.[vcSupplierPlant]='" + strSupplierPlant + "'");
                }
                if (strOrderPlant != "")
                {
                    strSql.AppendLine("AND T5.[vcOrderPlant]='" + strOrderPlant + "'");
                }
                strSql.AppendLine("order by T1.dSyncTime desc,T1.vcReceiver,T1.vcSupplierId");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void setSPInfo(DataTable dtAddInfo, DataTable dtModInfo, DataTable dtModInfo_SP, DataTable dtModInfo_PQ, DataTable dtModInfo_SI, DataTable dtModInfo_OP, DataTable dtOperHistory,
            string strOperId, ref DataTable dtMessage)
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
                strSql_addinfo.AppendLine("INSERT INTO [dbo].[TSPMaster]");
                strSql_addinfo.AppendLine("           ([dSyncTime],[vcChanges],[vcPackingPlant],[vcPartId],[vcPartENName],[vcCarfamilyCode],[vcCarModel],[vcReceiver]");
                strSql_addinfo.AppendLine("           ,[dFromTime],[dToTime],[vcPartId_Replace],[vcInOut],[vcOESP],[vcHaoJiu],[vcOldProduction],[dOldStartTime],[dDebugTime]");
                strSql_addinfo.AppendLine("           ,[vcSupplierId],[dSupplierFromTime],[dSupplierToTime],[vcSupplierName],[vcOperatorID],[dOperatorTime])");
                strSql_addinfo.AppendLine("     VALUES");
                strSql_addinfo.AppendLine("           (case when @dSyncTime='' then null else @dSyncTime end,@vcChanges,@vcPackingPlant,@vcPartId,@vcPartENName,@vcCarfamilyCode,@vcCarfamilyCode,@vcReceiver");
                strSql_addinfo.AppendLine("           ,case when @dFromTime='' then null else @dFromTime end,case when @dToTime='' then null else @dToTime end,@vcPartId_Replace,@vcInOut,@vcOESP,@vcHaoJiu,@vcOldProduction,case when @dOldStartTime='' then null else @dOldStartTime end,case when @dDebugTime='' then null else @dDebugTime+'/01' end");
                strSql_addinfo.AppendLine("           ,@vcSupplierId,@dSupplierFromTime,@dSupplierToTime,@vcSupplierName,'"+ strOperId + "',GETDATE())");
                sqlCommand_addinfo.CommandText = strSql_addinfo.ToString();
                sqlCommand_addinfo.Parameters.AddWithValue("@dSyncTime", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcChanges", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcPackingPlant", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcPartId", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcPartENName", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcCarfamilyCode", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcReceiver", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@dFromTime", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@dToTime", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcPartId_Replace", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcInOut", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcOESP", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcHaoJiu", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcOldProduction", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@dOldStartTime", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@dDebugTime", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcSupplierId", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@dSupplierFromTime", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@dSupplierToTime", "");
                sqlCommand_addinfo.Parameters.AddWithValue("@vcSupplierName", "");
                foreach (DataRow item in dtAddInfo.Rows)
                {
                    sqlCommand_addinfo.Parameters["@dSyncTime"].Value = item["dSyncTime"].ToString();
                    sqlCommand_addinfo.Parameters["@vcChanges"].Value = item["vcChanges"].ToString();
                    sqlCommand_addinfo.Parameters["@vcPackingPlant"].Value = item["vcPackingPlant"].ToString();
                    sqlCommand_addinfo.Parameters["@vcPartId"].Value = item["vcPartId"].ToString();
                    sqlCommand_addinfo.Parameters["@vcPartENName"].Value = item["vcPartENName"].ToString();
                    sqlCommand_addinfo.Parameters["@vcCarfamilyCode"].Value = item["vcCarfamilyCode"].ToString();
                    sqlCommand_addinfo.Parameters["@vcReceiver"].Value = item["vcReceiver"].ToString();
                    sqlCommand_addinfo.Parameters["@dFromTime"].Value = item["dFromTime"].ToString();
                    sqlCommand_addinfo.Parameters["@dToTime"].Value = item["dToTime"].ToString();
                    sqlCommand_addinfo.Parameters["@vcPartId_Replace"].Value = item["vcPartId_Replace"].ToString();
                    sqlCommand_addinfo.Parameters["@vcInOut"].Value = item["vcInOut"].ToString();
                    sqlCommand_addinfo.Parameters["@vcOESP"].Value = item["vcOESP"].ToString();
                    sqlCommand_addinfo.Parameters["@vcHaoJiu"].Value = item["vcHaoJiu"].ToString();
                    sqlCommand_addinfo.Parameters["@vcOldProduction"].Value = item["vcOldProduction"].ToString();
                    sqlCommand_addinfo.Parameters["@dOldStartTime"].Value = item["dOldStartTime"].ToString();
                    sqlCommand_addinfo.Parameters["@dDebugTime"].Value = item["dDebugTime"].ToString();
                    sqlCommand_addinfo.Parameters["@vcSupplierId"].Value = item["vcSupplierId"].ToString();
                    sqlCommand_addinfo.Parameters["@dSupplierFromTime"].Value = item["dSupplierFromTime"].ToString();
                    sqlCommand_addinfo.Parameters["@dSupplierToTime"].Value = item["dSupplierToTime"].ToString();
                    sqlCommand_addinfo.Parameters["@vcSupplierName"].Value = item["vcSupplierName"].ToString();
                    sqlCommand_addinfo.ExecuteNonQuery();
                }
                #endregion

                #region sqlCommand_modinfo
                SqlCommand sqlCommand_modinfo = sqlConnection.CreateCommand();
                sqlCommand_modinfo.Transaction = sqlTransaction;
                sqlCommand_modinfo.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo = new StringBuilder();
                strSql_modinfo.AppendLine("UPDATE [dbo].[TSPMaster]");
                strSql_modinfo.AppendLine("   SET [vcSupplierPlace] = @vcSupplierPlace");
                strSql_modinfo.AppendLine("      ,[vcInteriorProject] = @vcInteriorProject");
                strSql_modinfo.AppendLine("      ,[vcPassProject] = @vcPassProject");
                strSql_modinfo.AppendLine("      ,[vcFrontProject] = @vcFrontProject");
                strSql_modinfo.AppendLine("      ,[dFrontProjectTime] = case when @dFrontProjectTime='' then null else @dFrontProjectTime end");
                strSql_modinfo.AppendLine("      ,[dShipmentTime] = case when @dShipmentTime='' then null else @dShipmentTime end");
                strSql_modinfo.AppendLine("      ,[vcBillType] = @vcBillType");
                strSql_modinfo.AppendLine("      ,[vcOrderingMethod] = @vcOrderingMethod");
                strSql_modinfo.AppendLine("      ,[vcMandOrder] = @vcMandOrder");
                strSql_modinfo.AppendLine("      ,[vcRemark1] = @vcRemark1");
                strSql_modinfo.AppendLine("      ,[vcRemark2] = @vcRemark2");
                strSql_modinfo.AppendLine("      ,[vcOperatorID] = '"+ strOperId + "'");
                strSql_modinfo.AppendLine("      ,[dOperatorTime] = GETDATE()");
                strSql_modinfo.AppendLine(" WHERE LinId=@LinId");
                sqlCommand_modinfo.CommandText = strSql_modinfo.ToString();
                sqlCommand_modinfo.Parameters.AddWithValue("@LinId", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcSupplierPlace", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcInteriorProject", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcPassProject", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcFrontProject", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@dFrontProjectTime", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@dShipmentTime", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcBillType", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcOrderingMethod", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcMandOrder", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcRemark1", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@vcRemark2", "");
                foreach (DataRow item in dtModInfo.Rows)
                {
                    sqlCommand_modinfo.Parameters["@LinId"].Value = item["LinId"].ToString();
                    sqlCommand_modinfo.Parameters["@vcSupplierPlace"].Value = item["vcSupplierPlace"].ToString();
                    sqlCommand_modinfo.Parameters["@vcInteriorProject"].Value = item["vcInteriorProject"].ToString();
                    sqlCommand_modinfo.Parameters["@vcPassProject"].Value = item["vcPassProject"].ToString();
                    sqlCommand_modinfo.Parameters["@vcFrontProject"].Value = item["vcFrontProject"].ToString();
                    sqlCommand_modinfo.Parameters["@dFrontProjectTime"].Value = item["dFrontProjectTime"].ToString();
                    sqlCommand_modinfo.Parameters["@dShipmentTime"].Value = item["dShipmentTime"].ToString();
                    sqlCommand_modinfo.Parameters["@vcBillType"].Value = item["vcBillType"].ToString();
                    sqlCommand_modinfo.Parameters["@vcOrderingMethod"].Value = item["vcOrderingMethod"].ToString();
                    sqlCommand_modinfo.Parameters["@vcMandOrder"].Value = item["vcMandOrder"].ToString();
                    sqlCommand_modinfo.Parameters["@vcRemark1"].Value = item["vcRemark1"].ToString();
                    sqlCommand_modinfo.Parameters["@vcRemark2"].Value = item["vcRemark2"].ToString();
                    sqlCommand_modinfo.ExecuteNonQuery();
                }
                #endregion

                #region sqlCommand_modinfo_sp_add
                SqlCommand sqlCommand_modinfo_sp_add = sqlConnection.CreateCommand();
                sqlCommand_modinfo_sp_add.Transaction = sqlTransaction;
                sqlCommand_modinfo_sp_add.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo_sp_add = new StringBuilder();
                strSql_modinfo_sp_add.AppendLine("INSERT INTO [dbo].[TSPMaster_SupplierPlant]([vcPackingPlant],[vcPartId],[vcReceiver],[vcSupplierId],[dFromTime],[dToTime]");
                strSql_modinfo_sp_add.AppendLine("           ,[vcSupplierPlant],[vcOperatorType],[vcOperatorID],[dOperatorTime])");
                strSql_modinfo_sp_add.AppendLine("     VALUES");
                strSql_modinfo_sp_add.AppendLine("           (@vcPackingPlant,@vcPartId,@vcReceiver,@vcSupplierId,case when @dFromTime='' then null else @dFromTime end ,case when @dToTime='' then null else @dToTime end");
                strSql_modinfo_sp_add.AppendLine("           ,@vcSupplierPlant,'1','"+strOperId+"',GETDATE())");
                sqlCommand_modinfo_sp_add.CommandText = strSql_modinfo_sp_add.ToString();
                sqlCommand_modinfo_sp_add.Parameters.AddWithValue("@vcPackingPlant", "");
                sqlCommand_modinfo_sp_add.Parameters.AddWithValue("@vcPartId", "");
                sqlCommand_modinfo_sp_add.Parameters.AddWithValue("@vcReceiver", "");
                sqlCommand_modinfo_sp_add.Parameters.AddWithValue("@vcSupplierId", "");
                sqlCommand_modinfo_sp_add.Parameters.AddWithValue("@dFromTime", "");
                sqlCommand_modinfo_sp_add.Parameters.AddWithValue("@dToTime", "");
                sqlCommand_modinfo_sp_add.Parameters.AddWithValue("@vcSupplierPlant", "");
                foreach (DataRow item in dtModInfo_SP.Rows)
                {
                    if (item["status"].ToString() == "add" && item["error"].ToString() == "")
                    {
                        sqlCommand_modinfo_sp_add.Parameters["@vcPackingPlant"].Value = item["vcPackingPlant"].ToString();
                        sqlCommand_modinfo_sp_add.Parameters["@vcPartId"].Value = item["vcPartId"].ToString();
                        sqlCommand_modinfo_sp_add.Parameters["@vcReceiver"].Value = item["vcReceiver"].ToString();
                        sqlCommand_modinfo_sp_add.Parameters["@vcSupplierId"].Value = item["vcSupplierId"].ToString();
                        sqlCommand_modinfo_sp_add.Parameters["@dFromTime"].Value = item["dFromTime"].ToString();
                        sqlCommand_modinfo_sp_add.Parameters["@dToTime"].Value = item["dToTime"].ToString();
                        sqlCommand_modinfo_sp_add.Parameters["@vcSupplierPlant"].Value = item["vcSupplierPlant"].ToString();
                        sqlCommand_modinfo_sp_add.ExecuteNonQuery();
                    }
                }
                #endregion

                #region sqlCommand_modinfo_sp_mod
                SqlCommand sqlCommand_modinfo_sp_mod = sqlConnection.CreateCommand();
                sqlCommand_modinfo_sp_mod.Transaction = sqlTransaction;
                sqlCommand_modinfo_sp_mod.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo_sp_mod = new StringBuilder();
                strSql_modinfo_sp_mod.AppendLine("UPDATE [dbo].[TSPMaster_SupplierPlant]");
                strSql_modinfo_sp_mod.AppendLine("   SET [dToTime] = case when @dToTime='' then null else  @dToTime end");
                strSql_modinfo_sp_mod.AppendLine("      ,[vcOperatorID] = '"+strOperId+"'");
                strSql_modinfo_sp_mod.AppendLine("      ,[dOperatorTime] =GETDATE()");
                strSql_modinfo_sp_mod.AppendLine(" WHERE [LinId]=@LinId");
                sqlCommand_modinfo_sp_mod.CommandText = strSql_modinfo_sp_mod.ToString();
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@LinId", "");
                sqlCommand_modinfo_sp_mod.Parameters.AddWithValue("@dToTime", "");
                foreach (DataRow item in dtModInfo_SP.Rows)
                {
                    if (item["status"].ToString() == "mod" && item["error"].ToString() == "")
                    {
                        sqlCommand_modinfo_sp_mod.Parameters["@LinId"].Value = item["LinId"].ToString();
                        sqlCommand_modinfo_sp_mod.Parameters["@dToTime"].Value = item["dToTime"].ToString();
                        sqlCommand_modinfo_sp_mod.ExecuteNonQuery();
                    }
                }
                #endregion

                #region sqlCommand_modinfo_pq_add
                SqlCommand sqlCommand_modinfo_pq_add = sqlConnection.CreateCommand();
                sqlCommand_modinfo_pq_add.Transaction = sqlTransaction;
                sqlCommand_modinfo_pq_add.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo_pq_add = new StringBuilder();
                strSql_modinfo_pq_add.AppendLine("INSERT INTO [dbo].[TSPMaster_Box]([vcPackingPlant],[vcPartId],[vcReceiver],[vcSupplierId],[vcSupplierPlant],[dFromTime],[dToTime]");
                strSql_modinfo_pq_add.AppendLine("           ,[iPackingQty],[vcBoxType],[iLength],[iWidth],[iHeight],[iVolume],[vcOperatorType],[vcOperatorID],[dOperatorTime])");
                strSql_modinfo_pq_add.AppendLine("     VALUES");
                strSql_modinfo_pq_add.AppendLine("           (@vcPackingPlant,@vcPartId,@vcReceiver,@vcSupplierId,@vcSupplierPlant,case when @dFromTime='' then null else @dFromTime end ,case when @dToTime='' then null else @dToTime end");
                strSql_modinfo_pq_add.AppendLine("           ,@iPackingQty,@vcBoxType,@iLength,@iWidth,@iHeight,@iVolume,'1','"+strOperId+"',GETDATE())");
                sqlCommand_modinfo_pq_add.CommandText = strSql_modinfo_pq_add.ToString();
                sqlCommand_modinfo_pq_add.Parameters.AddWithValue("@vcPackingPlant", "");
                sqlCommand_modinfo_pq_add.Parameters.AddWithValue("@vcPartId", "");
                sqlCommand_modinfo_pq_add.Parameters.AddWithValue("@vcReceiver", "");
                sqlCommand_modinfo_pq_add.Parameters.AddWithValue("@vcSupplierId", "");
                sqlCommand_modinfo_pq_add.Parameters.AddWithValue("@vcSupplierPlant", "");
                sqlCommand_modinfo_pq_add.Parameters.AddWithValue("@dFromTime", "");
                sqlCommand_modinfo_pq_add.Parameters.AddWithValue("@dToTime", "");
                sqlCommand_modinfo_pq_add.Parameters.AddWithValue("@iPackingQty", "");
                sqlCommand_modinfo_pq_add.Parameters.AddWithValue("@vcBoxType", "");
                sqlCommand_modinfo_pq_add.Parameters.AddWithValue("@iLength", "");
                sqlCommand_modinfo_pq_add.Parameters.AddWithValue("@iWidth", "");
                sqlCommand_modinfo_pq_add.Parameters.AddWithValue("@iHeight", "");
                sqlCommand_modinfo_pq_add.Parameters.AddWithValue("@iVolume", "");
                foreach (DataRow item in dtModInfo_PQ.Rows)
                {
                    if (item["status"].ToString() == "add" && item["error"].ToString() == "")
                    {
                        sqlCommand_modinfo_pq_add.Parameters["@vcPackingPlant"].Value = item["vcPackingPlant"].ToString();
                        sqlCommand_modinfo_pq_add.Parameters["@vcPartId"].Value = item["vcPartId"].ToString();
                        sqlCommand_modinfo_pq_add.Parameters["@vcReceiver"].Value = item["vcReceiver"].ToString();
                        sqlCommand_modinfo_pq_add.Parameters["@vcSupplierId"].Value = item["vcSupplierId"].ToString();
                        sqlCommand_modinfo_pq_add.Parameters["@vcSupplierPlant"].Value = item["vcSupplierPlant"].ToString();
                        sqlCommand_modinfo_pq_add.Parameters["@dFromTime"].Value = item["dFromTime"].ToString();
                        sqlCommand_modinfo_pq_add.Parameters["@dToTime"].Value = item["dToTime"].ToString();
                        sqlCommand_modinfo_pq_add.Parameters["@iPackingQty"].Value = item["iPackingQty"].ToString();
                        sqlCommand_modinfo_pq_add.Parameters["@vcBoxType"].Value = item["vcBoxType"].ToString();
                        sqlCommand_modinfo_pq_add.Parameters["@iLength"].Value = item["iLength"].ToString();
                        sqlCommand_modinfo_pq_add.Parameters["@iWidth"].Value = item["iWidth"].ToString();
                        sqlCommand_modinfo_pq_add.Parameters["@iHeight"].Value = item["iHeight"].ToString();
                        sqlCommand_modinfo_pq_add.Parameters["@iVolume"].Value = item["iVolume"].ToString();
                        sqlCommand_modinfo_pq_add.ExecuteNonQuery();
                    }
                }
                #endregion

                #region sqlCommand_modinfo_pq_mod
                SqlCommand sqlCommand_modinfo_pq_mod = sqlConnection.CreateCommand();
                sqlCommand_modinfo_pq_mod.Transaction = sqlTransaction;
                sqlCommand_modinfo_pq_mod.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo_pq_mod = new StringBuilder();
                strSql_modinfo_pq_mod.AppendLine("UPDATE [dbo].[TSPMaster_Box]");
                strSql_modinfo_pq_mod.AppendLine("   SET [dToTime] = case when @dToTime='' then null else  @dToTime end");
                strSql_modinfo_pq_mod.AppendLine("      ,[vcOperatorID] = '"+strOperId+"'");
                strSql_modinfo_pq_mod.AppendLine("      ,[dOperatorTime] =GETDATE()");
                strSql_modinfo_pq_mod.AppendLine(" WHERE [LinId]=@LinId");
                sqlCommand_modinfo_pq_mod.CommandText = strSql_modinfo_pq_mod.ToString();
                sqlCommand_modinfo_pq_mod.Parameters.AddWithValue("@LinId", "");
                sqlCommand_modinfo_pq_mod.Parameters.AddWithValue("@dToTime", "");
                foreach (DataRow item in dtModInfo_PQ.Rows)
                {
                    if (item["status"].ToString() == "mod" && item["error"].ToString() == "")
                    {
                        sqlCommand_modinfo_pq_mod.Parameters["@LinId"].Value = item["LinId"].ToString();
                        sqlCommand_modinfo_pq_mod.Parameters["@dToTime"].Value = item["dToTime"].ToString();
                        sqlCommand_modinfo_pq_mod.ExecuteNonQuery();
                    }
                }
                #endregion

                #region sqlCommand_modinfo_si_add
                SqlCommand sqlCommand_modinfo_si_add = sqlConnection.CreateCommand();
                sqlCommand_modinfo_si_add.Transaction = sqlTransaction;
                sqlCommand_modinfo_si_add.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo_si_add = new StringBuilder();
                strSql_modinfo_si_add.AppendLine("INSERT INTO [dbo].[TSPMaster_SufferIn]([vcPackingPlant],[vcPartId],[vcReceiver],[vcSupplierId],[dFromTime],[dToTime]");
                strSql_modinfo_si_add.AppendLine("           ,[vcSufferIn],[vcOperatorType],[vcOperatorID],[dOperatorTime])");
                strSql_modinfo_si_add.AppendLine("     VALUES");
                strSql_modinfo_si_add.AppendLine("           (@vcPackingPlant,@vcPartId,@vcReceiver,@vcSupplierId,case when @dFromTime='' then null else @dFromTime end ,case when @dToTime='' then null else @dToTime end");
                strSql_modinfo_si_add.AppendLine("           ,@vcSufferIn,'1','"+strOperId+"',GETDATE())");
                sqlCommand_modinfo_si_add.CommandText = strSql_modinfo_si_add.ToString();
                sqlCommand_modinfo_si_add.Parameters.AddWithValue("@vcPackingPlant", "");
                sqlCommand_modinfo_si_add.Parameters.AddWithValue("@vcPartId", "");
                sqlCommand_modinfo_si_add.Parameters.AddWithValue("@vcReceiver", "");
                sqlCommand_modinfo_si_add.Parameters.AddWithValue("@vcSupplierId", "");
                sqlCommand_modinfo_si_add.Parameters.AddWithValue("@dFromTime", "");
                sqlCommand_modinfo_si_add.Parameters.AddWithValue("@dToTime", "");
                sqlCommand_modinfo_si_add.Parameters.AddWithValue("@vcSufferIn", "");
                foreach (DataRow item in dtModInfo_SI.Rows)
                {
                    if (item["status"].ToString() == "add" && item["error"].ToString() == "")
                    {
                        sqlCommand_modinfo_si_add.Parameters["@vcPackingPlant"].Value = item["vcPackingPlant"].ToString();
                        sqlCommand_modinfo_si_add.Parameters["@vcPartId"].Value = item["vcPartId"].ToString();
                        sqlCommand_modinfo_si_add.Parameters["@vcReceiver"].Value = item["vcReceiver"].ToString();
                        sqlCommand_modinfo_si_add.Parameters["@vcSupplierId"].Value = item["vcSupplierId"].ToString();
                        sqlCommand_modinfo_si_add.Parameters["@dFromTime"].Value = item["dFromTime"].ToString();
                        sqlCommand_modinfo_si_add.Parameters["@dToTime"].Value = item["dToTime"].ToString();
                        sqlCommand_modinfo_si_add.Parameters["@vcSufferIn"].Value = item["vcSufferIn"].ToString();
                        sqlCommand_modinfo_si_add.ExecuteNonQuery();
                    }
                }
                #endregion

                #region sqlCommand_modinfo_si_mod
                SqlCommand sqlCommand_modinfo_si_mod = sqlConnection.CreateCommand();
                sqlCommand_modinfo_si_mod.Transaction = sqlTransaction;
                sqlCommand_modinfo_si_mod.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo_si_mod = new StringBuilder();
                strSql_modinfo_si_mod.AppendLine("UPDATE [dbo].[TSPMaster_SufferIn]");
                strSql_modinfo_si_mod.AppendLine("   SET [dToTime] = case when @dToTime='' then null else  @dToTime end");
                strSql_modinfo_si_mod.AppendLine("      ,[vcOperatorID] = '" + strOperId + "'");
                strSql_modinfo_si_mod.AppendLine("      ,[dOperatorTime] =GETDATE()");
                strSql_modinfo_si_mod.AppendLine(" WHERE [LinId]=@LinId");
                sqlCommand_modinfo_si_mod.CommandText = strSql_modinfo_si_mod.ToString();
                sqlCommand_modinfo_si_mod.Parameters.AddWithValue("@LinId", "");
                sqlCommand_modinfo_si_mod.Parameters.AddWithValue("@dToTime", "");
                foreach (DataRow item in dtModInfo_SI.Rows)
                {
                    if (item["status"].ToString() == "mod" && item["error"].ToString() == "")
                    {
                        sqlCommand_modinfo_si_mod.Parameters["@LinId"].Value = item["LinId"].ToString();
                        sqlCommand_modinfo_si_mod.Parameters["@dToTime"].Value = item["dToTime"].ToString();
                        sqlCommand_modinfo_si_mod.ExecuteNonQuery();
                    }
                }
                #endregion

                #region sqlCommand_modinfo_op_add
                SqlCommand sqlCommand_modinfo_op_add = sqlConnection.CreateCommand();
                sqlCommand_modinfo_op_add.Transaction = sqlTransaction;
                sqlCommand_modinfo_op_add.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo_op_add = new StringBuilder();
                strSql_modinfo_op_add.AppendLine("INSERT INTO [dbo].[TSPMaster_OrderPlant]([vcPackingPlant],[vcPartId],[vcReceiver],[vcSupplierId],[dFromTime],[dToTime]");
                strSql_modinfo_op_add.AppendLine("           ,[vcOrderPlant],[vcOperatorType],[vcOperatorID],[dOperatorTime])");
                strSql_modinfo_op_add.AppendLine("     VALUES");
                strSql_modinfo_op_add.AppendLine("           (@vcPackingPlant,@vcPartId,@vcReceiver,@vcSupplierId,case when @dFromTime='' then null else @dFromTime end ,case when @dToTime='' then null else @dToTime end");
                strSql_modinfo_op_add.AppendLine("           ,@vcOrderPlant,'1','"+strOperId+"',GETDATE())");
                sqlCommand_modinfo_op_add.CommandText = strSql_modinfo_op_add.ToString();
                sqlCommand_modinfo_op_add.Parameters.AddWithValue("@vcPackingPlant", "");
                sqlCommand_modinfo_op_add.Parameters.AddWithValue("@vcPartId", "");
                sqlCommand_modinfo_op_add.Parameters.AddWithValue("@vcReceiver", "");
                sqlCommand_modinfo_op_add.Parameters.AddWithValue("@vcSupplierId", "");
                sqlCommand_modinfo_op_add.Parameters.AddWithValue("@dFromTime", "");
                sqlCommand_modinfo_op_add.Parameters.AddWithValue("@dToTime", "");
                sqlCommand_modinfo_op_add.Parameters.AddWithValue("@vcOrderPlant", "");
                foreach (DataRow item in dtModInfo_OP.Rows)
                {
                    if (item["status"].ToString() == "add" && item["error"].ToString() == "")
                    {
                        sqlCommand_modinfo_op_add.Parameters["@vcPackingPlant"].Value = item["vcPackingPlant"].ToString();
                        sqlCommand_modinfo_op_add.Parameters["@vcPartId"].Value = item["vcPartId"].ToString();
                        sqlCommand_modinfo_op_add.Parameters["@vcReceiver"].Value = item["vcReceiver"].ToString();
                        sqlCommand_modinfo_op_add.Parameters["@vcSupplierId"].Value = item["vcSupplierId"].ToString();
                        sqlCommand_modinfo_op_add.Parameters["@dFromTime"].Value = item["dFromTime"].ToString();
                        sqlCommand_modinfo_op_add.Parameters["@dToTime"].Value = item["dToTime"].ToString();
                        sqlCommand_modinfo_op_add.Parameters["@vcOrderPlant"].Value = item["vcOrderPlant"].ToString();
                        sqlCommand_modinfo_op_add.ExecuteNonQuery();
                    }
                }
                #endregion

                #region sqlCommand_modinfo_op_mod
                SqlCommand sqlCommand_modinfo_op_mod = sqlConnection.CreateCommand();
                sqlCommand_modinfo_op_mod.Transaction = sqlTransaction;
                sqlCommand_modinfo_op_mod.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo_op_mod = new StringBuilder();
                strSql_modinfo_op_mod.AppendLine("UPDATE [dbo].[TSPMaster_OrderPlant]");
                strSql_modinfo_op_mod.AppendLine("   SET [dToTime] = case when @dToTime='' then null else  @dToTime end");
                strSql_modinfo_op_mod.AppendLine("      ,[vcOperatorID] = '" + strOperId + "'");
                strSql_modinfo_op_mod.AppendLine("      ,[dOperatorTime] =GETDATE()");
                strSql_modinfo_op_mod.AppendLine(" WHERE [LinId]=@LinId");
                sqlCommand_modinfo_op_mod.CommandText = strSql_modinfo_op_mod.ToString();
                sqlCommand_modinfo_op_mod.Parameters.AddWithValue("@LinId", "");
                sqlCommand_modinfo_op_mod.Parameters.AddWithValue("@dToTime", "");
                foreach (DataRow item in dtModInfo_OP.Rows)
                {
                    if (item["status"].ToString() == "mod" && item["error"].ToString() == "")
                    {
                        sqlCommand_modinfo_op_mod.Parameters["@LinId"].Value = item["LinId"].ToString();
                        sqlCommand_modinfo_op_mod.Parameters["@dToTime"].Value = item["dToTime"].ToString();
                        sqlCommand_modinfo_op_mod.ExecuteNonQuery();
                    }
                }
                #endregion

                #region sqlCommand_operhistory
                SqlCommand sqlCommand_operhistory = sqlConnection.CreateCommand();
                sqlCommand_operhistory.Transaction = sqlTransaction;
                sqlCommand_operhistory.CommandType = CommandType.Text;
                StringBuilder strSql_operhistory = new StringBuilder();
                strSql_operhistory.AppendLine("INSERT INTO [dbo].[TSPMaster_OperHistory]([vcPackingPlant],[vcPartId],[vcReceiver],[vcSupplierId]");
                strSql_operhistory.AppendLine("           ,[vcAction],[vcOperatorID],[dOperatorTime])");
                strSql_operhistory.AppendLine("     VALUES(@vcPackingPlant,@vcPartId,@vcReceiver,@vcSupplierId");
                strSql_operhistory.AppendLine("           ,@vcAction,'"+strOperId+"',GETDATE())");
                sqlCommand_operhistory.CommandText = strSql_operhistory.ToString();
                sqlCommand_operhistory.Parameters.AddWithValue("@vcPackingPlant", "");
                sqlCommand_operhistory.Parameters.AddWithValue("@vcPartId", "");
                sqlCommand_operhistory.Parameters.AddWithValue("@vcReceiver", "");
                sqlCommand_operhistory.Parameters.AddWithValue("@vcSupplierId", "");
                sqlCommand_operhistory.Parameters.AddWithValue("@vcAction", "");
                foreach (DataRow item in dtOperHistory.Rows)
                {
                    if (item["error"].ToString() == "")
                    {
                        sqlCommand_operhistory.Parameters["@vcPackingPlant"].Value = item["vcPackingPlant"].ToString();
                        sqlCommand_operhistory.Parameters["@vcPartId"].Value = item["vcPartId"].ToString();
                        sqlCommand_operhistory.Parameters["@vcReceiver"].Value = item["vcReceiver"].ToString();
                        sqlCommand_operhistory.Parameters["@vcSupplierId"].Value = item["vcSupplierId"].ToString();
                        sqlCommand_operhistory.Parameters["@vcAction"].Value = item["vcAction"].ToString();
                        sqlCommand_operhistory.ExecuteNonQuery();
                    }
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
        public void deleteInfo(List<Dictionary<string, Object>> listInfoData, string strOperId)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    string strPackingPlant = listInfoData[i]["vcPackingPlant"].ToString();
                    string strPartId = listInfoData[i]["vcPartId"].ToString();
                    string strReceiver = listInfoData[i]["vcReceiver"].ToString();
                    string strSupplierId = listInfoData[i]["vcSupplierId"].ToString();
                    stringBuilder.AppendLine("DELETE FROM TSPMaster WHERE [vcPackingPlant]='" + strPackingPlant + "' AND [vcPartId]='" + strPartId + "' AND [vcReceiver]='" + strReceiver + "' AND [vcSupplierId]='" + strSupplierId + "'");
                    stringBuilder.AppendLine("DELETE FROM TSPMaster_Box WHERE [vcPackingPlant]='" + strPackingPlant + "' AND [vcPartId]='" + strPartId + "' AND [vcReceiver]='" + strReceiver + "' AND [vcSupplierId]='" + strSupplierId + "'");
                    stringBuilder.AppendLine("DELETE FROM TSPMaster_OrderPlant WHERE [vcPackingPlant]='" + strPackingPlant + "' AND [vcPartId]='" + strPartId + "' AND [vcReceiver]='" + strReceiver + "' AND [vcSupplierId]='" + strSupplierId + "'");
                    stringBuilder.AppendLine("DELETE FROM TSPMaster_SufferIn WHERE [vcPackingPlant]='" + strPackingPlant + "' AND [vcPartId]='" + strPartId + "' AND [vcReceiver]='" + strReceiver + "' AND [vcSupplierId]='" + strSupplierId + "'");
                    stringBuilder.AppendLine("DELETE FROM TSPMaster_SupplierPlant WHERE [vcPackingPlant]='" + strPackingPlant + "' AND [vcPartId]='" + strPartId + "' AND [vcReceiver]='" + strReceiver + "' AND [vcSupplierId]='" + strSupplierId + "'");
                    string strAction = "删除手配情报=>  包装厂:" + strPackingPlant + "；品番:" + strPartId + "；收货方:" + strReceiver + "；供应商:" + strSupplierId;
                    stringBuilder.AppendLine("INSERT INTO [dbo].[TSPMaster_OperHistory]");
                    stringBuilder.AppendLine("           ([vcPackingPlant]");
                    stringBuilder.AppendLine("           ,[vcPartId]");
                    stringBuilder.AppendLine("           ,[vcReceiver]");
                    stringBuilder.AppendLine("           ,[vcSupplierId]");
                    stringBuilder.AppendLine("           ,[vcAction]");
                    stringBuilder.AppendLine("           ,[vcOperatorID]");
                    stringBuilder.AppendLine("           ,[dOperatorTime])");
                    stringBuilder.AppendLine("     VALUES");
                    stringBuilder.AppendLine("           ('" + strPackingPlant + "'");
                    stringBuilder.AppendLine("           ,'" + strPartId + "'");
                    stringBuilder.AppendLine("           ,'" + strReceiver + "'");
                    stringBuilder.AppendLine("           ,'" + strSupplierId + "'");
                    stringBuilder.AppendLine("           ,'" + strAction + "'");
                    stringBuilder.AppendLine("           ,'" + strOperId + "'");
                    stringBuilder.AppendLine("           ,GETDATE())");
                }
                excute.ExcuteSqlWithStringOper(stringBuilder.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void setAllInTime(DataTable dataTable, DataTable dtOperHistory, DateTime dFromTime, DateTime dToTime, string strOperId, ref string strErrorPartId)
        {
            SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();

            sqlConnection.Open();
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            try
            {
                #region sqlCommand_allininfo
                SqlCommand sqlCommand_allininfo = sqlConnection.CreateCommand();
                sqlCommand_allininfo.Transaction = sqlTransaction;
                sqlCommand_allininfo.CommandType = CommandType.Text;
                StringBuilder strSql_allininfo = new StringBuilder();
                strSql_allininfo.AppendLine("UPDATE [TSPMaster] SET [dFromTime]='"+ dFromTime + "',[dToTime]='"+ dToTime + "',[vcOperatorID]='"+ strOperId + "',[dOperatorTime]=GETDATE() WHERE [LinId]=@LinId");
                sqlCommand_allininfo.CommandText = strSql_allininfo.ToString();
                sqlCommand_allininfo.Parameters.AddWithValue("@LinId", "");
                foreach (DataRow item in dataTable.Rows)
                {
                    sqlCommand_allininfo.Parameters["@LinId"].Value = item["LinId"].ToString();
                    sqlCommand_allininfo.ExecuteNonQuery();
                }
                #endregion

                #region sqlCommand_operhistory
                SqlCommand sqlCommand_operhistory = sqlConnection.CreateCommand();
                sqlCommand_operhistory.Transaction = sqlTransaction;
                sqlCommand_operhistory.CommandType = CommandType.Text;
                StringBuilder strSql_operhistory = new StringBuilder();
                strSql_operhistory.AppendLine("INSERT INTO [dbo].[TSPMaster_OperHistory]([vcPackingPlant],[vcPartId],[vcReceiver],[vcSupplierId]");
                strSql_operhistory.AppendLine("           ,[vcAction],[vcOperatorID],[dOperatorTime])");
                strSql_operhistory.AppendLine("     VALUES(@vcPackingPlant,@vcPartId,@vcReceiver,@vcSupplierId");
                strSql_operhistory.AppendLine("           ,@vcAction,'" + strOperId + "',GETDATE())");
                sqlCommand_operhistory.CommandText = strSql_operhistory.ToString();
                sqlCommand_operhistory.Parameters.AddWithValue("@vcPackingPlant", "");
                sqlCommand_operhistory.Parameters.AddWithValue("@vcPartId", "");
                sqlCommand_operhistory.Parameters.AddWithValue("@vcReceiver", "");
                sqlCommand_operhistory.Parameters.AddWithValue("@vcSupplierId", "");
                sqlCommand_operhistory.Parameters.AddWithValue("@vcAction", "");
                foreach (DataRow item in dtOperHistory.Rows)
                {
                    if (item["error"].ToString() == "")
                    {
                        sqlCommand_operhistory.Parameters["@vcPackingPlant"].Value = item["vcPackingPlant"].ToString();
                        sqlCommand_operhistory.Parameters["@vcPartId"].Value = item["vcPartId"].ToString();
                        sqlCommand_operhistory.Parameters["@vcReceiver"].Value = item["vcReceiver"].ToString();
                        sqlCommand_operhistory.Parameters["@vcSupplierId"].Value = item["vcSupplierId"].ToString();
                        sqlCommand_operhistory.Parameters["@vcAction"].Value = item["vcAction"].ToString();
                        sqlCommand_operhistory.ExecuteNonQuery();
                    }
                }
                #endregion

                sqlTransaction.Commit();
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("-->") != -1)
                {//主动判断抛出的异常
                    int startIndex = ex.Message.IndexOf("-->");
                    int endIndex = ex.Message.LastIndexOf("<--");
                    strErrorPartId = ex.Message.Substring(startIndex + 3, endIndex - startIndex - 3);
                }
                //回滚事务
                if (sqlTransaction != null && sqlConnection != null)
                {
                    sqlTransaction.Rollback();
                    sqlConnection.Close();
                }
            }
        }

        public DataTable getEditLoadInfo(string strEditType, string strPackingPlant, string strPartId, string strReceiver, string strSupplierId, string strSupplierPlant)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                if (strEditType == "SupplierPlantEdit")
                {
                    stringBuilder.AppendLine("SELECT[LinId],[dFromTime],[dToTime],[vcSupplierPlant],'0' as bAddFlag,'' as vcBgColor FROM [TSPMaster_SupplierPlant] ");
                    stringBuilder.AppendLine("WHERE [vcOperatorType]='1' and [vcPackingPlant]='" + strPackingPlant + "' and [vcPartId]='" + strPartId + "' and [vcReceiver]='" + strReceiver + "' and [vcSupplierId]='" + strSupplierId + "'");
                    stringBuilder.AppendLine("ORDER BY [dFromTime]");
                }
                if (strEditType == "PackingQtyEdit")
                {
                    stringBuilder.AppendLine("SELECT [LinId],[dFromTime],[dToTime],[iPackingQty],[vcBoxType],[iLength],[iWidth],[iHeight],[iVolume],'0' as bAddFlag,'' as vcBgColor FROM [TSPMaster_Box]");
                    stringBuilder.AppendLine("WHERE [vcOperatorType]='1' and [vcPackingPlant]='" + strPackingPlant + "' and [vcPartId]='" + strPartId + "' and [vcReceiver]='" + strReceiver + "' and [vcSupplierId]='" + strSupplierId + "' AND [vcSupplierPlant]='" + strSupplierPlant + "'");
                    stringBuilder.AppendLine("ORDER BY [dFromTime]");

                }
                if (strEditType == "SufferInEdit")
                {
                    stringBuilder.AppendLine("SELECT [LinId],[dFromTime],[dToTime],[vcSufferIn],'0' as bAddFlag,'' as vcBgColor FROM [TSPMaster_SufferIn]");
                    stringBuilder.AppendLine("WHERE [vcOperatorType]='1' and [vcPackingPlant]='" + strPackingPlant + "' and [vcPartId]='" + strPartId + "' and [vcReceiver]='" + strReceiver + "' and [vcSupplierId]='" + strSupplierId + "'");
                    stringBuilder.AppendLine("ORDER BY [dFromTime]");
                }
                if (strEditType == "OrderPlantEdit")
                {
                    stringBuilder.AppendLine("SELECT [LinId],[dFromTime],[dToTime],[vcOrderPlant],'0' as bAddFlag,'' as vcBgColor FROM [TSPMaster_OrderPlant]");
                    stringBuilder.AppendLine("WHERE [vcOperatorType]='1' and [vcPackingPlant]='" + strPackingPlant + "' and [vcPartId]='" + strPartId + "' and [vcReceiver]='" + strReceiver + "' and [vcSupplierId]='" + strSupplierId + "'");
                    stringBuilder.AppendLine("ORDER BY [dFromTime]");
                }
                return excute.ExcuteSqlWithSelectToDT(stringBuilder.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
