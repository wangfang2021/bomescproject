using System;
using System.Collections.Generic;
using Common;
using System.Data;
using System.Text;

namespace DataAccess
{
    public class FS0401_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        public DataTable searchApi(string PartId, string TimeFrom, string TimeTo, string carType, string InOut, string DHFlag)
        {
            try
            {

                TimeFrom = string.IsNullOrWhiteSpace(TimeFrom) == true ? "'1900/01/01'" : "'" + TimeFrom + "'";
                TimeTo = string.IsNullOrWhiteSpace(TimeTo) == true ? "'9999/12/31'" : "'" + TimeTo + "'";

                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine("SELECT d.vcName AS vcPackingPlant ,a.vcPartId,a.dFromTime,a.dToTime,a.vcCarModel,e.vcName AS vcInOut,f.vcName AS vcHaoJiu,g.vcName as vcOrderingMethod,c.iPackingQty,c.dFromTime AS dFromTimeQty,c.dToTime AS dToTimeQty,a.vcOldProduction,a.dDebugTime,a.vcPartId_Replace");
                sbr.AppendLine("FROM ");
                sbr.AppendLine("(");
                sbr.AppendLine("SELECT vcPackingPlant,vcPartId,vcReceiver,dFromTime,dToTime,vcSupplierId,vcCarModel,vcInOut,vcHaoJiu,vcOrderingMethod,vcOldProduction,dDebugTime,vcPartId_Replace FROM TSPMaster");
                sbr.AppendLine(") a");
                sbr.AppendLine("LEFT JOIN");
                sbr.AppendLine("(");
                sbr.AppendLine("SELECT vcPackingPlant,vcPartId,vcReceiver,vcSupplierId,vcSupplierPlant FROM TSPMaster_SupplierPlant");
                sbr.AppendLine("WHERE  vcOperatorType = '1' AND dFromTime >= " + TimeFrom + " AND dToTime <= " + TimeTo + "");
                sbr.AppendLine(") b ON a.vcPackingPlant = b.vcPackingPlant AND a.vcPartId = b.vcPartId AND a.vcReceiver = b.vcReceiver AND a.vcSupplierId = b.vcSupplierId");
                sbr.AppendLine("LEFT JOIN ");
                sbr.AppendLine("(");
                sbr.AppendLine("SELECT vcPackingPlant,vcPartId,vcReceiver,vcSupplierId,iPackingQty,vcSupplierPlant,dFromTime,dToTime FROM dbo.TSPMaster_Box");
                sbr.AppendLine("WHERE dFromTime >= " + TimeFrom + " AND dToTime <= " + TimeTo + " AND vcOperatorType = '1'");
                sbr.AppendLine(") c ON a.vcPackingPlant = c.vcPackingPlant AND a.vcPartId = c.vcPartId AND a.vcReceiver = c.vcReceiver AND a.vcSupplierId = c.vcSupplierId AND b.vcSupplierPlant = c.vcSupplierPlant");
                sbr.AppendLine("LEFT JOIN ");
                sbr.AppendLine("(");
                sbr.AppendLine("SELECT vcName,vcValue FROM TCode WHERE vcCodeId = 'C023'");
                sbr.AppendLine(") d ON a.vcPackingPlant = d.vcValue");
                sbr.AppendLine("LEFT JOIN ");
                sbr.AppendLine("(");
                sbr.AppendLine("SELECT vcName,vcValue FROM TCode WHERE vcCodeId = 'C003'");
                sbr.AppendLine(") e ON a.vcInOut = e.vcValue");
                sbr.AppendLine("LEFT JOIN ");
                sbr.AppendLine("(");
                sbr.AppendLine("SELECT vcName,vcValue FROM TCode WHERE vcCodeId = 'C004'");
                sbr.AppendLine(") f ON a.vcHaoJiu = f.vcValue");
                sbr.AppendLine("LEFT JOIN ");
                sbr.AppendLine("(");
                sbr.AppendLine("SELECT vcName,vcValue FROM TCode WHERE vcCodeId = 'C047'");
                sbr.AppendLine(") g ON a.vcOrderingMethod = g.vcValue");
                sbr.AppendLine("WHERE 1=1 ");
                sbr.AppendLine("AND a.dFromTime >= " + TimeFrom + "");
                sbr.AppendLine("AND a.dToTime <= " + TimeTo + "");
                if (!string.IsNullOrWhiteSpace(PartId))
                {
                    sbr.AppendLine("AND a.vcPartId like '" + PartId + "%'");
                }
                if (!string.IsNullOrWhiteSpace(carType))
                {
                    sbr.AppendLine("AND a.vcCarModel = '" + carType + "'");
                }
                if (!string.IsNullOrWhiteSpace(InOut))
                {
                    sbr.AppendLine("AND a.vcInOut = '" + InOut + "'");
                }
                if (!string.IsNullOrWhiteSpace(DHFlag))
                {
                    sbr.AppendLine("AND a.vcOrderingMethod = '" + DHFlag + "'");
                }

                return excute.ExcuteSqlWithSelectToDT(sbr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}