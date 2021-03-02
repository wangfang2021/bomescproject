using System;
using Common;
using System.Data;
using System.Text;
using System.Collections.Generic;

namespace DataAccess
{
    public class FS0300_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        //public DataTable getList(string PartId, string Supplier_id)
        //{
        //    try
        //    {
        //        StringBuilder sbr = new StringBuilder();

        //        sbr.AppendLine(" SELECT c.vcName AS vcOriginCompany, a.vcPart_id, d.vcName AS vcReceiver, a.vcPartNameEn, a.vcPartNameCn, a.vcCarTypeDesign, a.vcCarTypeDev, a.dTimeFrom, a.dTimeTo, ");
        //        sbr.AppendLine(" a.vcPartReplace, e.vcName AS vcInOutflag, f.vcName AS vcOE, a.vcHKPart_id, g.vcName AS vcHaoJiu, a.dJiuBegin, a.dJiuEnd, a.vcJiuYear, a.vcNXQF, a.dSSDate, a.vcSupplier_id,");
        //        sbr.AppendLine(" a.vcSCPlace, a.vcCHPlace, h.vcName AS vcSYTCode, a.vcSCSName, a.vcSCSAdress, a.vcZXBZNo, a.vcCarTypeName, a.vcFXDiff, a.vcFXNo, a.decPriceOrigin, a.decPriceTNPWithTax,");
        //        sbr.AppendLine(" b.vcSupplierPlant, b.vcSufferIn, b.iPackingQty, b.vcBoxType, b.vcBZPlant, b.vcBZUnit, b.vcPackNo, a.vcSupplier_name");
        //        sbr.AppendLine(" FROM(SELECT a.*, b.decPriceOrigin, b.decPriceTNPWithTax, c.vcSupplier_name");
        //        sbr.AppendLine("      FROM(SELECT vcOriginCompany, vcPart_id, vcReceiver, vcPartNameEn, vcPartNameCn, vcCarTypeDesign, vcCarTypeDev, dTimeFrom, dTimeTo, vcPartReplace, vcInOutflag, vcOE, vcHKPart_id,");
        //        sbr.AppendLine(" 	 vcHaoJiu, dJiuBegin, dJiuEnd, vcJiuYear, vcNXQF, dSSDate, vcSupplier_id, vcSCPlace, vcCHPlace, vcSYTCode, vcSCSName, vcSCSAdress, vcZXBZNo, vcCarTypeName, vcFXDiff, vcFXNo");
        //        sbr.AppendLine("           FROM TUnit");
        //        sbr.AppendLine("           WHERE dTimeFrom<=GETDATE()AND dTimeTo>=GETDATE()) a");
        //        sbr.AppendLine("          LEFT JOIN(SELECT vcPart_id, vcReceiver, vcSupplier_id, vcOriginCompany, decPriceOrigin, decPriceTNPWithTax");
        //        sbr.AppendLine("                    FROM TPrice");
        //        sbr.AppendLine("                    WHERE dPricebegin<=GETDATE()AND dPriceEnd>=GETDATE()) b ON a.vcOriginCompany=b.vcOriginCompany AND a.vcPart_id=b.vcPart_id AND a.vcReceiver=b.vcReceiver AND a.vcSupplier_id=b.vcSupplier_id");
        //        sbr.AppendLine("          LEFT JOIN(SELECT vcSupplier_id, vcSupplier_name FROM TSupplier) c ON a.vcSupplier_id=c.vcSupplier_id) a");
        //        sbr.AppendLine("     LEFT JOIN(SELECT a.*, b.vcSupplierPlant, c.vcSufferIn, d.iPackingQty, d.vcBoxType, e.vcBZPlant, e.vcBZUnit, f.vcPackNo");
        //        sbr.AppendLine("               FROM(SELECT vcPackingPlant, vcPartId, vcReceiver, vcSupplierId");
        //        sbr.AppendLine("                    FROM sppsdb_test.dbo.TSPMaster");
        //        sbr.AppendLine("                    WHERE dFromTime<=GETDATE()AND dToTime>=GETDATE()) a");
        //        sbr.AppendLine("                   LEFT JOIN(SELECT vcPackingPlant, vcPartId, vcReceiver, vcSupplierId, vcSupplierPlant");
        //        sbr.AppendLine("                             FROM sppsdb_test.dbo.TSPMaster_SupplierPlant");
        //        sbr.AppendLine("                             WHERE dFromTime<=GETDATE()AND dToTime>=GETDATE()AND vcOperatorType='1') b ON a.vcPackingPlant=b.vcPackingPlant AND a.vcPartId=b.vcPartId AND a.vcReceiver=b.vcReceiver AND a.vcSupplierId=b.vcSupplierId");
        //        sbr.AppendLine("                   LEFT JOIN(SELECT vcPackingPlant, vcPartId, vcReceiver, vcSupplierId, vcSufferIn");
        //        sbr.AppendLine("                             FROM sppsdb_test.dbo.TSPMaster_SufferIn");
        //        sbr.AppendLine("                             WHERE dFromTime<=GETDATE()AND dToTime>=GETDATE()AND vcOperatorType='1') c ON a.vcPackingPlant=c.vcPackingPlant AND a.vcPartId=c.vcPartId AND a.vcReceiver=c.vcReceiver");
        //        sbr.AppendLine("                   LEFT JOIN(SELECT vcPackingPlant, vcPartId, vcReceiver, vcSupplierId, vcSupplierPlant, iPackingQty, vcBoxType");
        //        sbr.AppendLine("                             FROM sppsdb_test.dbo.TSPMaster_Box");
        //        sbr.AppendLine("                             WHERE dFromTime<=GETDATE()AND dToTime>=GETDATE()AND vcOperatorType='1') d ON a.vcPackingPlant=d.vcPackingPlant AND a.vcPartId=d.vcPartId AND a.vcReceiver=d.vcReceiver AND a.vcSupplierId=d.vcSupplierId AND b.vcSupplierPlant=d.vcSupplierPlant");
        //        sbr.AppendLine("                   LEFT JOIN(SELECT vcPart_id, vcReceiver, vcSupplierId, vcPackingPlant, vcBZPlant, vcBZUnit");
        //        sbr.AppendLine("                             FROM sppsdb_test.dbo.TPackageMaster");
        //        sbr.AppendLine("                             WHERE dTimeFrom<=GETDATE()AND dTimeTo>=GETDATE()) e ON a.vcPackingPlant=e.vcPackingPlant AND a.vcReceiver=e.vcReceiver AND a.vcPartId=e.vcPart_id AND a.vcSupplierId=e.vcSupplierId");
        //        sbr.AppendLine("                   LEFT JOIN(SELECT vcPartsNo, vcPackNo, vcShouhuofangID");
        //        sbr.AppendLine("                             FROM sppsdb_test.dbo.TPackItem");
        //        sbr.AppendLine("                             WHERE dUsedFrom<=GETDATE()AND dUsedTo>=GETDATE()) f ON a.vcPartId=f.vcPartsNo AND a.vcReceiver=f.vcShouhuofangID) b ON a.vcPart_id=b.vcPartId AND a.vcReceiver=b.vcReceiver AND a.vcSupplier_id=b.vcSupplierId");
        //        sbr.AppendLine("     LEFT JOIN(SELECT vcValue, vcName FROM TCode WHERE vcCodeId='C006') c ON a.vcOriginCompany=c.vcValue");
        //        sbr.AppendLine("     LEFT JOIN(SELECT vcValue, vcName FROM TCode WHERE vcCodeId='C018') d ON a.vcReceiver=d.vcValue");
        //        sbr.AppendLine("     LEFT JOIN(SELECT vcValue, vcName FROM TCode WHERE vcCodeId='C003') e ON a.vcInOutflag=e.vcValue");
        //        sbr.AppendLine("     LEFT JOIN(SELECT vcValue, vcName FROM TCode WHERE vcCodeId='C012') f ON a.vcOE=f.vcValue");
        //        sbr.AppendLine("     LEFT JOIN(SELECT vcValue, vcName FROM TCode WHERE vcCodeId='C004') g ON a.vcHaoJiu=g.vcValue");
        //        sbr.AppendLine("     LEFT JOIN(SELECT vcValue, vcName FROM TCode WHERE vcCodeId='C016') h ON a.vcSYTCode=h.vcValue");
        //        sbr.AppendLine(" WHERE a.vcPart_id LIKE '" + PartId + "%' AND a.vcSupplier_id LIKE '" + Supplier_id + "%';");

        //        return excute.ExcuteSqlWithSelectToDT(sbr.ToString(), "TK");
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        throw;
        //    }
        //}

        public DataTable getList(string PartId, string Supplier_id)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();

                //sbr.AppendLine(" SELECT a.vcOriginCompany AS vcOriginCompany, a.vcPart_id, a.vcReceiver AS vcReceiver, a.vcPartNameEn, a.vcPartNameCn, a.vcCarTypeDesign, a.vcCarTypeDev, a.dTimeFrom, a.dTimeTo, ");
                //sbr.AppendLine(" a.vcPartReplace, e.vcName AS vcInOutflag, f.vcName AS vcOE, a.vcHKPart_id, g.vcName AS vcHaoJiu, a.dJiuBegin, a.dJiuEnd, a.vcJiuYear, a.vcNXQF, a.dSSDate, a.vcSupplier_id,");
                //sbr.AppendLine(" a.vcSCPlace, a.vcCHPlace, a.vcSYTCode AS vcSYTCode, a.vcSCSName, a.vcSCSAdress, a.vcZXBZNo, a.vcCarTypeName, a.vcFXDiff, a.vcFXNo, a.decPriceOrigin, a.decPriceTNPWithTax,");
                //sbr.AppendLine(" b.vcSupplierPlant, b.vcSufferIn, b.iPackingQty, b.vcBoxType, b.vcBZPlant, b.vcBZUnit, b.vcPackNo, a.vcSupplier_name");
                //sbr.AppendLine(" FROM(SELECT a.*, b.decPriceOrigin, b.decPriceTNPWithTax, c.vcSupplier_name");
                //sbr.AppendLine("      FROM(SELECT vcOriginCompany, vcPart_id, vcReceiver, vcPartNameEn, vcPartNameCn, vcCarTypeDesign, vcCarTypeDev, dTimeFrom, dTimeTo, vcPartReplace, vcInOutflag, vcOE, vcHKPart_id,");
                //sbr.AppendLine(" 	 vcHaoJiu, dJiuBegin, dJiuEnd, vcJiuYear, vcNXQF, dSSDate, vcSupplier_id, vcSCPlace, vcCHPlace, vcSYTCode, vcSCSName, vcSCSAdress, vcZXBZNo, vcCarTypeName, vcFXDiff, vcFXNo");
                //sbr.AppendLine("           FROM TUnit");
                //sbr.AppendLine("           WHERE dTimeFrom<=GETDATE()AND dTimeTo>=GETDATE()) a");
                //sbr.AppendLine("          LEFT JOIN(SELECT vcPart_id, vcReceiver, vcSupplier_id, vcOriginCompany, decPriceOrigin, decPriceTNPWithTax");
                //sbr.AppendLine("                    FROM TPrice");
                //sbr.AppendLine("                    WHERE dPricebegin<=GETDATE()AND dPriceEnd>=GETDATE()) b ON a.vcOriginCompany=b.vcOriginCompany AND a.vcPart_id=b.vcPart_id AND a.vcReceiver=b.vcReceiver AND a.vcSupplier_id=b.vcSupplier_id");
                //sbr.AppendLine("          LEFT JOIN(SELECT vcSupplier_id, vcSupplier_name FROM TSupplier) c ON a.vcSupplier_id=c.vcSupplier_id) a");
                //sbr.AppendLine("     LEFT JOIN(SELECT a.*, b.vcSupplierPlant, c.vcSufferIn, d.iPackingQty, d.vcBoxType, e.vcBZPlant, e.vcBZUnit, f.vcPackNo");
                //sbr.AppendLine("               FROM(SELECT vcPackingPlant, vcPartId, vcReceiver, vcSupplierId");
                //sbr.AppendLine("                    FROM sppsdb_test.dbo.TSPMaster");
                //sbr.AppendLine("                    WHERE dFromTime<=GETDATE()AND dToTime>=GETDATE()) a");
                //sbr.AppendLine("                   LEFT JOIN(SELECT vcPackingPlant, vcPartId, vcReceiver, vcSupplierId, vcSupplierPlant");
                //sbr.AppendLine("                             FROM sppsdb_test.dbo.TSPMaster_SupplierPlant");
                //sbr.AppendLine("                             WHERE dFromTime<=GETDATE()AND dToTime>=GETDATE()AND vcOperatorType='1') b ON a.vcPackingPlant=b.vcPackingPlant AND a.vcPartId=b.vcPartId AND a.vcReceiver=b.vcReceiver AND a.vcSupplierId=b.vcSupplierId");
                //sbr.AppendLine("                   LEFT JOIN(SELECT vcPackingPlant, vcPartId, vcReceiver, vcSupplierId, vcSufferIn");
                //sbr.AppendLine("                             FROM sppsdb_test.dbo.TSPMaster_SufferIn");
                //sbr.AppendLine("                             WHERE dFromTime<=GETDATE()AND dToTime>=GETDATE()AND vcOperatorType='1') c ON a.vcPackingPlant=c.vcPackingPlant AND a.vcPartId=c.vcPartId AND a.vcReceiver=c.vcReceiver");
                //sbr.AppendLine("                   LEFT JOIN(SELECT vcPackingPlant, vcPartId, vcReceiver, vcSupplierId, vcSupplierPlant, iPackingQty, vcBoxType");
                //sbr.AppendLine("                             FROM sppsdb_test.dbo.TSPMaster_Box");
                //sbr.AppendLine("                             WHERE dFromTime<=GETDATE()AND dToTime>=GETDATE()AND vcOperatorType='1') d ON a.vcPackingPlant=d.vcPackingPlant AND a.vcPartId=d.vcPartId AND a.vcReceiver=d.vcReceiver AND a.vcSupplierId=d.vcSupplierId AND b.vcSupplierPlant=d.vcSupplierPlant");
                //sbr.AppendLine("                   LEFT JOIN(SELECT vcPart_id, vcReceiver, vcSupplierId, vcPackingPlant, vcBZPlant, vcBZUnit");
                //sbr.AppendLine("                             FROM sppsdb_test.dbo.TPackageMaster");
                //sbr.AppendLine("                             WHERE dTimeFrom<=GETDATE()AND dTimeTo>=GETDATE()) e ON a.vcPackingPlant=e.vcPackingPlant AND a.vcReceiver=e.vcReceiver AND a.vcPartId=e.vcPart_id AND a.vcSupplierId=e.vcSupplierId");
                //sbr.AppendLine("                   LEFT JOIN(SELECT vcPartsNo, vcPackNo, vcShouhuofangID");
                //sbr.AppendLine("                             FROM sppsdb_test.dbo.TPackItem");
                //sbr.AppendLine("                             WHERE dUsedFrom<=GETDATE()AND dUsedTo>=GETDATE()) f ON a.vcPartId=f.vcPartsNo AND a.vcReceiver=f.vcShouhuofangID) b ON a.vcPart_id=b.vcPartId AND a.vcReceiver=b.vcReceiver AND a.vcSupplier_id=b.vcSupplierId");
                ////sbr.AppendLine("     LEFT JOIN(SELECT vcValue, vcName FROM TCode WHERE vcCodeId='C006') c ON a.vcOriginCompany=c.vcValue");
                ////sbr.AppendLine("     LEFT JOIN(SELECT vcValue, vcName FROM TCode WHERE vcCodeId='C018') d ON a.vcReceiver=d.vcValue");
                //sbr.AppendLine("     LEFT JOIN(SELECT vcValue, vcName FROM TCode WHERE vcCodeId='C003') e ON a.vcInOutflag=e.vcValue");
                //sbr.AppendLine("     LEFT JOIN(SELECT vcValue, vcName FROM TCode WHERE vcCodeId='C012') f ON a.vcOE=f.vcValue");
                //sbr.AppendLine("     LEFT JOIN(SELECT vcValue, vcName FROM TCode WHERE vcCodeId='C004') g ON a.vcHaoJiu=g.vcValue");
                ////sbr.AppendLine("     LEFT JOIN(SELECT vcValue, vcName FROM TCode WHERE vcCodeId='C016') h ON a.vcSYTCode=h.vcValue");
                //sbr.AppendLine(" WHERE a.vcPart_id LIKE '" + PartId + "%' AND a.vcSupplier_id LIKE '" + Supplier_id + "%';");
                //sbr.AppendLine("SELECT a.vcOriginCompany AS vcOriginCompany, a.vcPart_id, a.vcReceiver AS vcReceiver, a.vcPartNameEn, a.vcPartNameCn, a.vcCarTypeDesign, a.vcCarTypeDev, a.dTimeFrom, a.dTimeTo, ");
                //sbr.AppendLine("a.vcPartReplace, e.vcName AS vcInOutflag, f.vcName AS vcOE, a.vcHKPart_id, g.vcName AS vcHaoJiu, a.dJiuBegin, a.dJiuEnd, a.vcJiuYear, a.vcNXQF, a.dSSDate, a.vcSupplier_id,");
                //sbr.AppendLine("a.vcSCPlace, a.vcCHPlace, a.vcSYTCode AS vcSYTCode, a.vcSCSName, a.vcSCSAdress, a.vcZXBZNo, a.vcCarTypeName, a.vcFXDiff, a.vcFXNo, a.decPriceOrigin, a.decPriceTNPWithTax,");
                //sbr.AppendLine("b.vcSupplierPlant, b.vcSufferIn, b.iPackingQty, b.vcBoxType, b.vcBZPlant, b.vcBZUnit, b.vcPackNo, a.vcSupplier_name");
                //sbr.AppendLine("FROM(SELECT a.*, b.decPriceOrigin, b.decPriceTNPWithTax, c.vcSupplier_name");
                //sbr.AppendLine("     FROM(SELECT vcOriginCompany, vcPart_id, vcReceiver, vcPartNameEn, vcPartNameCn, vcCarTypeDesign, vcCarTypeDev, dTimeFrom, dTimeTo, vcPartReplace, vcInOutflag, vcOE, vcHKPart_id,");
                //sbr.AppendLine("	 vcHaoJiu, dJiuBegin, dJiuEnd, vcJiuYear, vcNXQF, dSSDate, vcSupplier_id, vcSCPlace, vcCHPlace, vcSYTCode, vcSCSName, vcSCSAdress, vcZXBZNo, vcCarTypeName, vcFXDiff, vcFXNo");
                //sbr.AppendLine("          FROM TUnit");
                //sbr.AppendLine("          WHERE dTimeFrom<=GETDATE()AND dTimeTo>=GETDATE()) a");
                //sbr.AppendLine("         LEFT JOIN(SELECT vcPart_id, vcReceiver, vcSupplier_id, vcOriginCompany, decPriceOrigin, decPriceTNPWithTax");
                //sbr.AppendLine("                   FROM TPrice");
                //sbr.AppendLine("                   WHERE dPricebegin<=GETDATE()AND dPriceEnd>=GETDATE()) b ON a.vcOriginCompany=b.vcOriginCompany AND a.vcPart_id=b.vcPart_id AND a.vcReceiver=b.vcReceiver AND a.vcSupplier_id=b.vcSupplier_id");
                //sbr.AppendLine("         LEFT JOIN(SELECT vcSupplier_id, vcSupplier_name FROM TSupplier) c ON a.vcSupplier_id=c.vcSupplier_id) a");
                //sbr.AppendLine("    LEFT JOIN(SELECT a.*, e.vcBZPlant, e.vcBZUnit, f.vcPackNo");
                //sbr.AppendLine("              FROM(SELECT * FROM VI_TSPMaster) a  ");
                //sbr.AppendLine("                  LEFT JOIN(SELECT vcPart_id, vcReceiver, vcSupplierId, vcPackingPlant, vcBZPlant, vcBZUnit,vcSYTCode");
                //sbr.AppendLine("                            FROM VI_TPackageMaster");
                //sbr.AppendLine("                           ) e ON a.vcPackingPlant=e.vcPackingPlant AND a.vcReceiver=e.vcReceiver AND a.vcPartId=e.vcPart_id AND a.vcSupplierId=e.vcSupplierId AND A.vcSYTCode=e.vcSYTCode");
                //sbr.AppendLine("                  LEFT JOIN(SELECT vcPartsNo, vcPackNo, vcShouhuofangID,vcSYTCode FROM VI_TPackItem");
                //sbr.AppendLine("                            ) f ON a.vcPartId=f.vcPartsNo AND a.vcReceiver=f.vcShouhuofangID and a.vcSYTCode=f.vcSYTCode");
                //sbr.AppendLine("	) b ON a.vcPart_id=b.vcPartId AND a.vcReceiver=b.vcReceiver AND a.vcSupplier_id=b.vcSupplierId");
                //sbr.AppendLine("    LEFT JOIN(SELECT vcValue, vcName FROM TCode WHERE vcCodeId='C003') e ON a.vcInOutflag=e.vcValue");
                //sbr.AppendLine("    LEFT JOIN(SELECT vcValue, vcName FROM TCode WHERE vcCodeId='C012') f ON a.vcOE=f.vcValue");
                //sbr.AppendLine("    LEFT JOIN(SELECT vcValue, vcName FROM TCode WHERE vcCodeId='C004') g ON a.vcHaoJiu=g.vcValue");
                sbr.AppendLine("SELECT a.vcOriginCompany, a.vcPart_id, a.vcReceiver, a.vcPartNameEn, a.vcPartNameCn, a.vcCarTypeDesign, a.vcCarTypeDev, a.dTimeFrom, a.dTimeTo, ");
                sbr.AppendLine("a.vcPartReplace, e.vcName AS vcInOutflag, f.vcName AS vcOE, a.vcHKPart_id, g.vcName AS vcHaoJiu, a.dJiuBegin, a.dJiuEnd, a.vcJiuYear, a.vcNXQF, a.dSSDate, a.vcSupplier_id,");
                sbr.AppendLine("a.vcSCPlace, a.vcCHPlace, a.vcSYTCode, a.vcSCSName, a.vcSCSAdress, a.vcZXBZNo, a.vcCarTypeName, a.vcFXDiff, a.vcFXNo, a.decPriceOrigin, a.decPriceTNPWithTax,");
                sbr.AppendLine("b.vcSupplierPlant, b.vcSufferIn, b.iPackingQty, b.vcBoxType, b.vcBZPlant, b.vcBZUnit, b.vcPackNo, a.vcSupplier_name");
                sbr.AppendLine("FROM(SELECT a.*, b.decPriceOrigin, b.decPriceTNPWithTax, c.vcSupplier_name");
                sbr.AppendLine("     FROM(SELECT vcOriginCompany, vcPart_id, vcReceiver, vcPartNameEn, vcPartNameCn, vcCarTypeDesign, vcCarTypeDev, dTimeFrom, dTimeTo, vcPartReplace, vcInOutflag, vcOE, vcHKPart_id,");
                sbr.AppendLine("	 vcHaoJiu, dJiuBegin, dJiuEnd, vcJiuYear, vcNXQF, dSSDate, vcSupplier_id, vcSCPlace, vcCHPlace, vcSYTCode, vcSCSName, vcSCSAdress, vcZXBZNo, vcCarTypeName, vcFXDiff, vcFXNo");
                sbr.AppendLine("          FROM TUnit");
                sbr.AppendLine("          WHERE dTimeFrom<=GETDATE()AND dTimeTo>=GETDATE()) a");
                sbr.AppendLine("         LEFT JOIN(SELECT vcPart_id, vcReceiver, vcSupplier_id, vcOriginCompany, decPriceOrigin, decPriceTNPWithTax,vcSYTCode FROM VI_Price");
                sbr.AppendLine("             ) b ON a.vcOriginCompany=b.vcOriginCompany AND REPLACE(a.vcPart_id,'-','')=b.vcPart_id AND a.vcReceiver=b.vcReceiver AND a.vcSupplier_id=b.vcSupplier_id AND a.vcSYTCode=b.vcSYTCode");
                sbr.AppendLine("         LEFT JOIN(SELECT vcSupplier_id, vcSupplier_name FROM TSupplier) c ON a.vcSupplier_id=c.vcSupplier_id) a");
                sbr.AppendLine("    LEFT JOIN(SELECT a.*, e.vcBZPlant, e.vcBZUnit, f.vcPackNo");
                sbr.AppendLine("              FROM(SELECT * FROM VI_TSPMaster) a  ");
                sbr.AppendLine("                  LEFT JOIN(SELECT vcPart_id, vcReceiver, vcSupplierId, vcPackingPlant, vcBZPlant, vcBZUnit,vcSYTCode");
                sbr.AppendLine("                            FROM VI_TPackageMaster");
                sbr.AppendLine("                           ) e ON a.vcPackingPlant=e.vcPackingPlant AND a.vcReceiver=e.vcReceiver AND a.vcPartId=e.vcPart_id AND a.vcSupplierId=e.vcSupplierId AND A.vcSYTCode=e.vcSYTCode");
                sbr.AppendLine("                  LEFT JOIN(SELECT vcPartsNo, vcPackNo, vcShouhuofangID,vcSYTCode FROM VI_TPackItem");
                sbr.AppendLine("                            ) f ON a.vcPartId=f.vcPartsNo AND a.vcReceiver=f.vcShouhuofangID and a.vcSYTCode=f.vcSYTCode");
                sbr.AppendLine("	) b ON REPLACE(a.vcPart_id,'-','')=b.vcPartId AND a.vcReceiver=b.vcReceiver AND a.vcSupplier_id=b.vcSupplierId");
                sbr.AppendLine("    LEFT JOIN(SELECT vcValue, vcName FROM TCode WHERE vcCodeId='C003') e ON a.vcInOutflag=e.vcValue");
                sbr.AppendLine("    LEFT JOIN(SELECT vcValue, vcName FROM TCode WHERE vcCodeId='C012') f ON a.vcOE=f.vcValue");
                sbr.AppendLine("    LEFT JOIN(SELECT vcValue, vcName FROM TCode WHERE vcCodeId='C004') g ON a.vcHaoJiu=g.vcValue");
                sbr.AppendLine(" WHERE a.vcPart_id LIKE '" + PartId + "%' AND a.vcSupplier_id LIKE '" + Supplier_id + "%';");

                return excute.ExcuteSqlWithSelectToDT(sbr.ToString(), "TK");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


    }
}