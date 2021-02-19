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
    public class FS0617_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        
        public DataTable getSearchInfo(string strPlantArea, string strOrderPlant, string strPartId, string strCarModel, string strReceiver, string strSupplier)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("SELECT  1 AS iEnum,");
                strSql.AppendLine("		T1.LinId AS LinId,");
                strSql.AppendLine("		T1.vcPackingPlant AS vcPlantArea,");
                strSql.AppendLine("		T7.vcName AS vcPlantArea_name,");
                strSql.AppendLine("		T5.vcOrderPlant,");
                strSql.AppendLine("		T8.vcName AS vcOrderPlant_name,");
                strSql.AppendLine("		T1.vcPartId AS vcPartId,");
                strSql.AppendLine("		T1.dFromTime AS dFromTime,");
                strSql.AppendLine("		T1.dToTime AS dToTime,");
                strSql.AppendLine("		T1.vcCarfamilyCode AS vcCarModel,");
                strSql.AppendLine("		T1.vcReceiver AS vcReceiver,");
                strSql.AppendLine("		T1.vcSupplierId AS vcSupplierId,");
                strSql.AppendLine("		T4.vcSufferIn AS vcSufferIn,");
                strSql.AppendLine("		T3.iPackingQty AS iPackingQty,");
                strSql.AppendLine("		T1.vcPartENName AS vcPartENName,");
                strSql.AppendLine("		T1.vcPassProject AS vcPassProject,");
                strSql.AppendLine("		T1.vcInteriorProject AS vcInteriorProject,");
                strSql.AppendLine("		T1.dFrontProjectTime AS dFrontProjectTime,");
                strSql.AppendLine("		T1.dShipmentTime AS dShipmentTime,");
                strSql.AppendLine("		T1.vcRemark1,");
                strSql.AppendLine("		T1.vcRemark2,");
                strSql.AppendLine("		T6.vcImagePath AS vcImagePath,");
                strSql.AppendLine("		'1' AS bSelectFlag");
                strSql.AppendLine("		FROM ");
                strSql.AppendLine("(SELECT *  FROM [TSPMaster] WHERE vcInOut='0' ");
                if (strPlantArea != "")
                {
                    strSql.AppendLine("AND vcPackingPlant='" + strPlantArea + "'");
                }
                if (strPartId != "")
                {
                    strSql.AppendLine("AND vcPartId like '" + strPartId + "%'");
                }
                if (strCarModel != "")
                {
                    strSql.AppendLine("AND vcCarfamilyCode='" + strCarModel + "'");
                }
                if (strReceiver != "")
                {
                    strSql.AppendLine("AND vcReceiver='" + strReceiver + "'");
                }
                if (strSupplier != "")
                {
                    strSql.AppendLine("AND vcSupplierId='" + strSupplier + "'");
                }
                strSql.AppendLine(")T1");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT * FROM TSPMaster_SupplierPlant ");
                strSql.AppendLine("WHERE [vcOperatorType]='1' AND dFromTime<=CONVERT(VARCHAR(10),GETDATE(),23) AND dToTime>=CONVERT(VARCHAR(10),GETDATE(),23))T2");
                strSql.AppendLine("ON T1.[vcPackingPlant]=T2.[vcPackingPlant] AND T1.[vcPartId]=T2.[vcPartId]");
                strSql.AppendLine("AND T1.[vcReceiver]=T2.[vcReceiver] AND T1.[vcSupplierId]=T2.[vcSupplierId]");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT * FROM TSPMaster_Box ");
                strSql.AppendLine("WHERE [vcOperatorType]='1' AND dFromTime<=CONVERT(VARCHAR(10),GETDATE(),23) AND dToTime>=CONVERT(VARCHAR(10),GETDATE(),23))T3");
                strSql.AppendLine("ON T1.[vcPackingPlant]=T3.[vcPackingPlant] AND T1.[vcPartId]=T3.[vcPartId]");
                strSql.AppendLine("AND T1.[vcReceiver]=T3.[vcReceiver] AND T1.[vcSupplierId]=T3.[vcSupplierId] AND T2.vcSupplierPlant=T3.[vcSupplierPlant]");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT * FROM TSPMaster_SufferIn ");
                strSql.AppendLine("WHERE [vcOperatorType]='1' AND dFromTime<=CONVERT(VARCHAR(10),GETDATE(),23) AND dToTime>=CONVERT(VARCHAR(10),GETDATE(),23))T4");
                strSql.AppendLine("ON T1.[vcPackingPlant]=T4.[vcPackingPlant] AND T1.[vcPartId]=T4.[vcPartId] ");
                strSql.AppendLine("AND T1.[vcReceiver]=T4.[vcReceiver] AND T1.[vcSupplierId]=T4.[vcSupplierId]");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT * FROM TSPMaster_OrderPlant ");
                strSql.AppendLine("WHERE [vcOperatorType]='1' AND dFromTime<=CONVERT(VARCHAR(10),GETDATE(),23) AND dToTime>=CONVERT(VARCHAR(10),GETDATE(),23))T5");
                strSql.AppendLine("ON T1.[vcPackingPlant]=T5.[vcPackingPlant] AND T1.[vcPartId]=T5.[vcPartId]");
                strSql.AppendLine("AND T1.[vcReceiver]=T5.[vcReceiver] AND T1.[vcSupplierId]=T5.[vcSupplierId]");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT * FROM TSPMaster_ImagePath ");
                strSql.AppendLine("WHERE [vcOperatorType]='1' AND dFromTime<=CONVERT(VARCHAR(10),GETDATE(),23) AND dToTime>=CONVERT(VARCHAR(10),GETDATE(),23))T6");
                strSql.AppendLine("ON T1.[vcPartId]=T6.[vcPartId] ");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C051')T7--包装工厂");
                strSql.AppendLine("ON T1.vcPackingPlant=T7.vcValue");
                strSql.AppendLine("LEFT JOIN");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C000')T8--发注工厂");
                strSql.AppendLine("ON T5.vcOrderPlant=T8.vcValue");
                if (strOrderPlant != "")
                {
                    strSql.AppendLine("WHERE T5.vcOrderPlant='" + strOrderPlant + "'");
                }
                strSql.AppendLine("ORDER BY T1.vcPackingPlant,T5.vcOrderPlant,T1.vcReceiver,T1.vcSupplierId,T1.vcPartId");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable getPrintInfo(string strParameter)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select iAutoId as LinId,'' as Enum,* from [SP_M_SITEM] where iAutoId in (" + strParameter + ") ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
