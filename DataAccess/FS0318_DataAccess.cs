using System;
using Common;
using System.Data;
using System.Text;

namespace DataAccess
{
    public class FS0318_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        public DataTable search(string vcCarType)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine(" IF EXISTS (SELECT *  ");
                sbr.AppendLine(" FROM tempdb.dbo.sysobjects  ");
                sbr.AppendLine(" WHERE id=OBJECT_ID(N'tempdb..#temp')AND type='U') ");
                sbr.AppendLine(" DROP TABLE #temp; ");
                sbr.AppendLine("  ");
                sbr.AppendLine(" SELECT * INTO #temp FROM  ");
                sbr.AppendLine(" ( ");
                sbr.AppendLine(" 	SELECT vcPart_id,vcSupplier_id,vcOriginCompany,vcReceiver,vcCarTypeDesign,vcInOutflag,vcSYTCode FROM Tunit  ");
                sbr.AppendLine(" 	---WHERE dTimeFrom >= GETDATE() AND dTimeTo <= GETDATE() AND dTimeTo <> dTimeFrom AND vcCarTypeDesign = '"+ vcCarType + "'  ");
                sbr.AppendLine(" ) a ");
                sbr.AppendLine(" DECLARE @inSum INT ");
                sbr.AppendLine(" DECLARE @outSum INT ");
                sbr.AppendLine("  ");
                sbr.AppendLine(" select  @outSum=COUNT(*) FROM #temp WHERE vcInOutflag = '0' ");
                sbr.AppendLine(" select  @inSum=COUNT(*) FROM #temp WHERE vcInOutflag = '1' ");
                sbr.AppendLine("  ");
                sbr.AppendLine(" DECLARE @inSQ INT ");
                sbr.AppendLine(" DECLARE @outSQ INT ");
                sbr.AppendLine("  ");
                sbr.AppendLine(" SELECT @inSQ = COUNT(*)  FROM  ");
                sbr.AppendLine(" (SELECT * FROM #temp WHERE vcInOutflag = '0' ) a ");
                sbr.AppendLine(" LEFT JOIN ");
                sbr.AppendLine(" ( ");
                sbr.AppendLine(" SELECT * FROM dbo.VI_SQJD WHERE vcJD <> '1' ");
                sbr.AppendLine(" ) b ON a.vcPart_id = b.vcPart_id AND b.vcSupplier_id = a.vcSupplier_id AND b.vcSYTCode = a.vcSYTCode ");
                sbr.AppendLine(" WHERE  b.vcJD IS NOT NULL ");
                sbr.AppendLine("  ");
                sbr.AppendLine(" SELECT @outSQ = COUNT(*)  FROM  ");
                sbr.AppendLine(" (SELECT * FROM #temp WHERE vcInOutflag = '1' ) a ");
                sbr.AppendLine(" LEFT JOIN ");
                sbr.AppendLine(" ( ");
                sbr.AppendLine(" SELECT * FROM dbo.VI_SQJD WHERE vcJD <> '1' ");
                sbr.AppendLine(" ) b ON a.vcPart_id = b.vcPart_id AND b.vcSupplier_id = a.vcSupplier_id AND b.vcSYTCode = a.vcSYTCode ");
                sbr.AppendLine(" WHERE  b.vcJD IS NOT NULL ");
                sbr.AppendLine("  ");
                sbr.AppendLine(" DECLARE @inPrice INT ");
                sbr.AppendLine(" DECLARE @outPrice INT ");
                sbr.AppendLine("  ");
                sbr.AppendLine(" SELECT @inPrice = COUNT(*)  FROM  ");
                sbr.AppendLine(" (SELECT * FROM #temp WHERE vcInOutflag = '0' ) a ");
                sbr.AppendLine(" LEFT JOIN ");
                sbr.AppendLine(" ( ");
                sbr.AppendLine(" 	SELECT * FROM dbo.VI_Price ");
                sbr.AppendLine(" ) b ON a.vcPart_id = b.vcPart_id AND b.vcSupplier_id = a.vcSupplier_id AND b.vcSYTCode = a.vcSYTCode AND a.vcReceiver = b.vcReceiver ");
                sbr.AppendLine(" WHERE  b.decPriceOrigin IS NOT NULL ");
                sbr.AppendLine("  ");
                sbr.AppendLine(" SELECT @outPrice = COUNT(*)  FROM  ");
                sbr.AppendLine(" (SELECT * FROM #temp WHERE vcInOutflag = '1' ) a ");
                sbr.AppendLine(" LEFT JOIN ");
                sbr.AppendLine(" ( ");
                sbr.AppendLine(" 	SELECT * FROM dbo.VI_Price ");
                sbr.AppendLine(" ) b ON a.vcPart_id = b.vcPart_id AND b.vcSupplier_id = a.vcSupplier_id AND b.vcSYTCode = a.vcSYTCode AND a.vcReceiver = b.vcReceiver ");
                sbr.AppendLine(" WHERE  b.decPriceOrigin IS NOT NULL ");
                sbr.AppendLine("  ");
                sbr.AppendLine(" DECLARE @inHezi INT ");
                sbr.AppendLine(" DECLARE @outHezi INT ");
                sbr.AppendLine("  ");
                sbr.AppendLine(" SELECT @inHezi = COUNT(*)  FROM  ");
                sbr.AppendLine(" (SELECT * FROM #temp WHERE vcInOutflag = '0' ) a ");
                sbr.AppendLine(" LEFT JOIN ");
                sbr.AppendLine(" ( ");
                sbr.AppendLine(" 	SELECT * FROM dbo.VI_HeZi ");
                sbr.AppendLine(" ) b ON a.vcPart_id = b.vcPartNo AND b.vcSupplier_id = a.vcSupplier_id AND b.vcPackingPlant = a.vcSYTCode AND a.vcReceiver = b.vcReceiver ");
                sbr.AppendLine(" WHERE  b.vcIntake IS NOT NULL ");
                sbr.AppendLine("  ");
                sbr.AppendLine(" SELECT @outHezi = COUNT(*)  FROM  ");
                sbr.AppendLine(" (SELECT * FROM #temp WHERE vcInOutflag = '1' ) a ");
                sbr.AppendLine(" LEFT JOIN ");
                sbr.AppendLine(" ( ");
                sbr.AppendLine(" 	SELECT * FROM dbo.VI_HeZi ");
                sbr.AppendLine(" ) b ON a.vcPart_id = b.vcPartNo AND b.vcSupplier_id = a.vcSupplier_id AND b.vcPackingPlant = a.vcSYTCode AND a.vcReceiver = b.vcReceiver ");
                sbr.AppendLine(" WHERE  b.vcIntake IS NOT NULL ");
                sbr.AppendLine("  ");
                sbr.AppendLine(" DECLARE @inPack INT ");
                sbr.AppendLine(" DECLARE @outPack INT ");
                sbr.AppendLine("  ");
                sbr.AppendLine(" SELECT @inPack = COUNT(*)  FROM  ");
                sbr.AppendLine(" (SELECT * FROM #temp WHERE vcInOutflag = '0' ) a ");
                sbr.AppendLine(" LEFT JOIN ");
                sbr.AppendLine(" ( ");
                sbr.AppendLine(" 	SELECT * FROM dbo.VI_PackItem ");
                sbr.AppendLine(" ) b ON a.vcPart_id = b.vcPartsNo AND b.vcSYTCode = a.vcSYTCode AND a.vcReceiver = b.vcShouhuofangID ");
                sbr.AppendLine(" WHERE  b.vcPackNo IS NOT NULL ");
                sbr.AppendLine("  ");
                sbr.AppendLine(" SELECT @outPack = COUNT(*)  FROM  ");
                sbr.AppendLine(" (SELECT * FROM #temp WHERE vcInOutflag = '1' ) a ");
                sbr.AppendLine(" LEFT JOIN ");
                sbr.AppendLine(" ( ");
                sbr.AppendLine(" 	SELECT * FROM dbo.VI_PackItem ");
                sbr.AppendLine(" ) b ON a.vcPart_id = b.vcPartsNo AND b.vcSYTCode = a.vcSYTCode AND a.vcReceiver = b.vcShouhuofangID ");
                sbr.AppendLine(" WHERE  b.vcPackNo IS NOT NULL ");
                sbr.AppendLine("  ");
                sbr.AppendLine(" SELECT @inSum AS inSum ,@outSum AS outSum,@inSum+@outSum AS total,@inSQ AS inSQ,@outSQ AS outSQ,@inPrice AS inPrice,@outPrice AS outPrice,@inHezi AS inHezi,@outHezi AS outHezi,@inPack AS inPack,@outPack AS outPack, ");
                sbr.AppendLine(" ltrim(Convert(numeric(9,2),@inSQ*100.0/@inSum))+'%' As inSQPer,ltrim(Convert(numeric(9,2),@outSQ*100.0/@outSum))+'%' As outSQPer, ");
                sbr.AppendLine(" ltrim(Convert(numeric(9,2),@inPrice*100.0/@inSum))+'%' As inPricePer,ltrim(Convert(numeric(9,2),@outPrice*100.0/@outSum))+'%' As outPricePer, ");
                sbr.AppendLine(" ltrim(Convert(numeric(9,2),@inHezi*100.0/@inSum))+'%' As inHeziPer,ltrim(Convert(numeric(9,2),@outHezi*100.0/@outSum))+'%' As outHeziPer, ");
                sbr.AppendLine(" ltrim(Convert(numeric(9,2),@inPack*100.0/@inSum))+'%' As inPackPer,ltrim(Convert(numeric(9,2),@outPack*100.0/@outSum))+'%' As outPackPer ");

                return excute.ExcuteSqlWithSelectToDT(sbr.ToString(),"TK");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}