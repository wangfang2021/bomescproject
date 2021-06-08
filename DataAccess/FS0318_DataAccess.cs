using System;
using System.Collections.Generic;
using Common;
using System.Data;
using System.Text;

namespace DataAccess
{
    public class FS0318_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        public DataTable search(string vcCarType, string timeFrom)
        {
            try
            {
                StringBuilder sbr = new StringBuilder();

                //sbr.AppendLine("DECLARE @time DATETIME");
                //sbr.AppendLine("SET @time = '" + timeFrom + "'");
                //sbr.AppendLine("IF EXISTS (SELECT *  ");
                //sbr.AppendLine("FROM tempdb.dbo.sysobjects  ");
                //sbr.AppendLine("WHERE id=OBJECT_ID(N'tempdb..#temp')AND type='U') ");
                //sbr.AppendLine("DROP TABLE #temp; ");
                //sbr.AppendLine(" ");
                //sbr.AppendLine("SELECT * INTO #temp FROM  ");
                //sbr.AppendLine("( ");
                //sbr.AppendLine("	SELECT (CASE WHEN LEN(REPLACE(vcPart_id,'-','')) = 12 THEN REPLACE(vcPart_id,'-','') WHEN LEN(REPLACE(vcPart_id,'-','')) = 10 THEN REPLACE(vcPart_id,'-','')+'00' END) as vcPart_id,vcSupplier_id,vcOriginCompany,vcReceiver,vcCarTypeDev,vcInOutflag,vcSYTCode FROM Tunit  ");
                //sbr.AppendLine("	WHERE dTimeFrom <= @time AND dTimeTo >= @time AND dTimeTo <> dTimeFrom AND vcCarTypeDev = '" + vcCarType + "'  ");
                //sbr.AppendLine(") a ");
                //sbr.AppendLine("DECLARE @inSum INT ");
                //sbr.AppendLine("DECLARE @outSum INT ");
                //sbr.AppendLine(" ");
                //sbr.AppendLine("select  @inSum=COUNT(*) FROM #temp WHERE vcInOutflag = '0' ");
                //sbr.AppendLine("select  @outSum=COUNT(*) FROM #temp WHERE vcInOutflag = '1' ");
                //sbr.AppendLine(" ");
                //sbr.AppendLine("DECLARE @inSQ INT ");
                //sbr.AppendLine("DECLARE @outSQ INT ");
                //sbr.AppendLine(" ");
                //sbr.AppendLine("SELECT @inSQ = COUNT(*)  FROM  ");
                //sbr.AppendLine("(SELECT * FROM #temp WHERE vcInOutflag = '0' ) a ");
                //sbr.AppendLine("LEFT JOIN ");
                //sbr.AppendLine("( ");
                //sbr.AppendLine("SELECT * FROM dbo.VI_SQJD WHERE vcJD <> '1' ");
                //sbr.AppendLine(") b ON a.vcPart_id = b.vcPart_id AND b.vcSupplier_id = a.vcSupplier_id AND b.vcSYTCode = a.vcSYTCode ");
                //sbr.AppendLine("WHERE  b.vcJD IS NOT NULL ");
                //sbr.AppendLine(" ");
                //sbr.AppendLine("SELECT @outSQ = COUNT(*)  FROM  ");
                //sbr.AppendLine("(SELECT * FROM #temp WHERE vcInOutflag = '1' ) a ");
                //sbr.AppendLine("LEFT JOIN ");
                //sbr.AppendLine("( ");
                //sbr.AppendLine("SELECT * FROM dbo.VI_SQJD WHERE vcJD <> '1' ");
                //sbr.AppendLine(") b ON a.vcPart_id = b.vcPart_id AND b.vcSupplier_id = a.vcSupplier_id AND b.vcSYTCode = a.vcSYTCode ");
                //sbr.AppendLine("WHERE  b.vcJD IS NOT NULL ");
                //sbr.AppendLine(" ");
                //sbr.AppendLine("DECLARE @inPrice INT ");
                //sbr.AppendLine("DECLARE @outPrice INT ");
                //sbr.AppendLine(" ");
                //sbr.AppendLine("SELECT @inPrice = COUNT(*)  FROM  ");
                //sbr.AppendLine("(SELECT * FROM #temp WHERE vcInOutflag = '0' ) a ");
                //sbr.AppendLine("LEFT JOIN ");
                //sbr.AppendLine("( ");
                //sbr.AppendLine("	SELECT * FROM dbo.VI_Price WHERE dPricebegin <= @time AND dPriceEnd >=  @time");
                //sbr.AppendLine(") b ON a.vcPart_id = b.vcPart_id AND b.vcSupplier_id = a.vcSupplier_id AND b.vcSYTCode = a.vcSYTCode AND a.vcReceiver = b.vcReceiver ");
                //sbr.AppendLine("WHERE  b.decPriceOrigin IS NOT NULL ");
                //sbr.AppendLine(" ");
                //sbr.AppendLine("SELECT @outPrice = COUNT(*)  FROM  ");
                //sbr.AppendLine("(SELECT * FROM #temp WHERE vcInOutflag = '1' ) a ");
                //sbr.AppendLine("LEFT JOIN ");
                //sbr.AppendLine("( ");
                //sbr.AppendLine("	SELECT * FROM dbo.VI_Price  WHERE dPricebegin <= @time AND dPriceEnd >=  @time");
                //sbr.AppendLine(") b ON a.vcPart_id = b.vcPart_id AND b.vcSupplier_id = a.vcSupplier_id AND b.vcSYTCode = a.vcSYTCode AND a.vcReceiver = b.vcReceiver ");
                //sbr.AppendLine("WHERE  b.decPriceOrigin IS NOT NULL ");
                //sbr.AppendLine(" ");
                //sbr.AppendLine("DECLARE @inHezi INT ");
                //sbr.AppendLine("DECLARE @outHezi INT ");
                //sbr.AppendLine(" ");
                //sbr.AppendLine("SELECT @inHezi = COUNT(*)  FROM  ");
                //sbr.AppendLine("(SELECT * FROM #temp WHERE vcInOutflag = '0' ) a ");
                //sbr.AppendLine("LEFT JOIN ");
                //sbr.AppendLine("( ");
                //sbr.AppendLine("	SELECT * FROM dbo.VI_HeZi  WHERE dUseStartDate <= @time AND dUserEndDate >=  @time");
                //sbr.AppendLine(") b ON a.vcPart_id = b.vcPartNo AND b.vcSupplier_id = a.vcSupplier_id AND b.vcPackingPlant = a.vcSYTCode AND a.vcReceiver = b.vcReceiver ");
                //sbr.AppendLine("WHERE  b.vcIntake IS NOT NULL ");
                //sbr.AppendLine(" ");
                //sbr.AppendLine("SELECT @outHezi = COUNT(*)  FROM  ");
                //sbr.AppendLine("(SELECT * FROM #temp WHERE vcInOutflag = '1' ) a ");
                //sbr.AppendLine("LEFT JOIN ");
                //sbr.AppendLine("( ");
                //sbr.AppendLine("	SELECT * FROM dbo.VI_HeZi  WHERE dUseStartDate <= @time AND dUserEndDate >=  @time");
                //sbr.AppendLine(") b ON a.vcPart_id = b.vcPartNo AND b.vcSupplier_id = a.vcSupplier_id AND b.vcPackingPlant = a.vcSYTCode AND a.vcReceiver = b.vcReceiver ");
                //sbr.AppendLine("WHERE  b.vcIntake IS NOT NULL ");
                //sbr.AppendLine(" ");
                //sbr.AppendLine("DECLARE @inPack INT ");
                //sbr.AppendLine("DECLARE @outPack INT ");
                //sbr.AppendLine(" ");
                //sbr.AppendLine("SELECT @inPack = COUNT(*)  FROM  ");
                //sbr.AppendLine("(SELECT * FROM #temp WHERE vcInOutflag = '0' ) a ");
                //sbr.AppendLine("LEFT JOIN ");
                //sbr.AppendLine("( ");
                //sbr.AppendLine("	SELECT * FROM dbo.VI_PackItem WHERE dUsedFrom <= @time AND dUsedTo >= @time");
                //sbr.AppendLine(") b ON a.vcPart_id = b.vcPartsNo AND b.vcSYTCode = a.vcSYTCode AND a.vcReceiver = b.vcShouhuofangID ");
                //sbr.AppendLine("WHERE  b.vcPackNo IS NOT NULL ");
                //sbr.AppendLine(" ");
                //sbr.AppendLine("SELECT @outPack = COUNT(*)  FROM  ");
                //sbr.AppendLine("(SELECT * FROM #temp WHERE vcInOutflag = '1' ) a ");
                //sbr.AppendLine("LEFT JOIN ");
                //sbr.AppendLine("( ");
                //sbr.AppendLine("	SELECT * FROM dbo.VI_PackItem  WHERE dUsedFrom <= @time AND dUsedTo >= @time");
                //sbr.AppendLine(") b ON a.vcPart_id = b.vcPartsNo AND b.vcSYTCode = a.vcSYTCode AND a.vcReceiver = b.vcShouhuofangID ");
                //sbr.AppendLine("WHERE  b.vcPackNo IS NOT NULL ");
                //sbr.AppendLine(" ");
                //sbr.AppendLine("SELECT @inSum AS inSum ,@outSum AS outSum,@inSum+@outSum AS total,@inSQ AS inSQ,@outSQ AS outSQ,@inPrice AS inPrice,@outPrice AS outPrice,@inHezi AS inHezi,@outHezi AS outHezi,@inPack AS inPack,@outPack AS outPack, ");
                //sbr.AppendLine("ltrim(Convert(numeric(9,2),@inSQ*100.0/(CASE @inSum WHEN 0 THEN 1 ELSE @inSum END)))+'%' As inSQPer,ltrim(Convert(numeric(9,2),@outSQ*100.0/(CASE @outSum WHEN 0 THEN 1 ELSE @outSum END)))+'%' As outSQPer, ");
                //sbr.AppendLine("ltrim(Convert(numeric(9,2),@inPrice*100.0/CASE @inSum WHEN 0 THEN 1 ELSE @inSum END))+'%' As inPricePer,ltrim(Convert(numeric(9,2),@outPrice*100.0/(CASE @outSum WHEN 0 THEN 1 ELSE @outSum END)))+'%' As outPricePer, ");
                //sbr.AppendLine("ltrim(Convert(numeric(9,2),@inHezi*100.0/CASE @inSum WHEN 0 THEN 1 ELSE @inSum END))+'%' As inHeziPer,ltrim(Convert(numeric(9,2),@outHezi*100.0/(CASE @outSum WHEN 0 THEN 1 ELSE @outSum END)))+'%' As outHeziPer, ");
                //sbr.AppendLine("ltrim(Convert(numeric(9,2),@inPack*100.0/CASE @inSum WHEN 0 THEN 1 ELSE @inSum END))+'%' As inPackPer,ltrim(Convert(numeric(9,2),@outPack*100.0/(CASE @outSum WHEN 0 THEN 1 ELSE @outSum END)))+'%' As outPackPer ");

                sbr.AppendLine("DECLARE @time DATETIME");
                sbr.AppendLine("SET @time = '" + timeFrom + "'");
                sbr.AppendLine("IF EXISTS (SELECT *  ");
                sbr.AppendLine("FROM tempdb.dbo.sysobjects  ");
                sbr.AppendLine("WHERE id=OBJECT_ID(N'tempdb..#temp')AND type='U') ");
                sbr.AppendLine("DROP TABLE #temp; ");
                sbr.AppendLine(" ");
                sbr.AppendLine("SELECT * INTO #temp FROM  ");
                sbr.AppendLine("( ");
                sbr.AppendLine("	SELECT (CASE WHEN LEN(REPLACE(vcPart_id,'-','')) = 12 THEN REPLACE(vcPart_id,'-','') WHEN LEN(REPLACE(vcPart_id,'-','')) = 10 THEN REPLACE(vcPart_id,'-','')+'00' END) as vcPart_id,vcSupplier_id,vcOriginCompany,vcReceiver,vcCarTypeDev,vcInOutflag,vcSYTCode FROM Tunit  ");
                sbr.AppendLine("	WHERE dTimeFrom <= @time AND dTimeTo >= @time AND dTimeTo <> dTimeFrom AND vcCarTypeDev = '" + vcCarType + "'  ");
                sbr.AppendLine(") a ");
                sbr.AppendLine("DECLARE @inSum INT ");
                sbr.AppendLine("DECLARE @outSum INT ");
                sbr.AppendLine(" ");
                sbr.AppendLine("select  @inSum=COUNT(*) FROM #temp WHERE vcInOutflag = '0' ");
                sbr.AppendLine("select  @outSum=COUNT(*) FROM #temp WHERE vcInOutflag = '1' ");
                sbr.AppendLine(" ");
                //sbr.AppendLine("DECLARE @inSQ INT ");
                //sbr.AppendLine("DECLARE @outSQ INT ");
                //sbr.AppendLine(" ");
                //sbr.AppendLine("SELECT @inSQ = COUNT(*)  FROM  ");
                //sbr.AppendLine("(SELECT * FROM #temp WHERE vcInOutflag = '0' ) a ");
                //sbr.AppendLine("LEFT JOIN ");
                //sbr.AppendLine("( ");
                //sbr.AppendLine("SELECT * FROM dbo.VI_SQJD WHERE vcJD <> '1' ");
                //sbr.AppendLine(") b ON a.vcPart_id = b.vcPart_id AND b.vcSupplier_id = a.vcSupplier_id  ");
                //sbr.AppendLine("WHERE  b.vcJD IS NOT NULL ");
                //sbr.AppendLine(" ");
                //sbr.AppendLine("SELECT @outSQ = COUNT(*)  FROM  ");
                //sbr.AppendLine("(SELECT * FROM #temp WHERE vcInOutflag = '1' ) a ");
                //sbr.AppendLine("LEFT JOIN ");
                //sbr.AppendLine("( ");
                //sbr.AppendLine("SELECT * FROM dbo.VI_SQJD WHERE vcJD <> '1' ");
                //sbr.AppendLine(") b ON a.vcPart_id = b.vcPart_id AND b.vcSupplier_id = a.vcSupplier_id  ");
                //sbr.AppendLine("WHERE  b.vcJD IS NOT NULL ");
                //sbr.AppendLine(" ");
                //sbr.AppendLine("DECLARE @inPrice INT ");
                //sbr.AppendLine("DECLARE @outPrice INT ");
                //sbr.AppendLine(" ");
                //sbr.AppendLine("SELECT @inPrice = COUNT(*)  FROM  ");
                //sbr.AppendLine("(SELECT * FROM #temp WHERE vcInOutflag = '0' ) a ");
                //sbr.AppendLine("LEFT JOIN ");
                //sbr.AppendLine("( ");
                //sbr.AppendLine("	SELECT * FROM dbo.VI_Price WHERE dPricebegin <= @time AND dPriceEnd >=  @time");
                //sbr.AppendLine(") b ON a.vcPart_id = b.vcPart_id AND b.vcSupplier_id = a.vcSupplier_id  AND a.vcReceiver = b.vcReceiver ");
                //sbr.AppendLine("WHERE  b.decPriceOrigin IS NOT NULL ");
                //sbr.AppendLine(" ");
                //sbr.AppendLine("SELECT @outPrice = COUNT(*)  FROM  ");
                //sbr.AppendLine("(SELECT * FROM #temp WHERE vcInOutflag = '1' ) a ");
                //sbr.AppendLine("LEFT JOIN ");
                //sbr.AppendLine("( ");
                //sbr.AppendLine("	SELECT * FROM dbo.VI_Price  WHERE dPricebegin <= @time AND dPriceEnd >=  @time");
                //sbr.AppendLine(") b ON a.vcPart_id = b.vcPart_id AND b.vcSupplier_id = a.vcSupplier_id  AND a.vcReceiver = b.vcReceiver ");
                //sbr.AppendLine("WHERE  b.decPriceOrigin IS NOT NULL ");
                //sbr.AppendLine(" ");
                //sbr.AppendLine("DECLARE @inHezi INT ");
                //sbr.AppendLine("DECLARE @outHezi INT ");
                //sbr.AppendLine(" ");
                //sbr.AppendLine("SELECT @inHezi = COUNT(*)  FROM  ");
                //sbr.AppendLine("(SELECT * FROM #temp WHERE vcInOutflag = '0' ) a ");
                //sbr.AppendLine("LEFT JOIN ");
                //sbr.AppendLine("( ");
                //sbr.AppendLine("	SELECT * FROM dbo.VI_HeZi  WHERE dUseStartDate <= @time AND dUserEndDate >=  @time");
                //sbr.AppendLine(") b ON a.vcPart_id = b.vcPartNo AND b.vcSupplier_id = a.vcSupplier_id  AND a.vcReceiver = b.vcReceiver ");
                //sbr.AppendLine("WHERE  b.vcIntake IS NOT NULL ");
                //sbr.AppendLine(" ");
                //sbr.AppendLine("SELECT @outHezi = COUNT(*)  FROM  ");
                //sbr.AppendLine("(SELECT * FROM #temp WHERE vcInOutflag = '1' ) a ");
                //sbr.AppendLine("LEFT JOIN ");
                //sbr.AppendLine("( ");
                //sbr.AppendLine("	SELECT * FROM dbo.VI_HeZi  WHERE dUseStartDate <= @time AND dUserEndDate >=  @time");
                //sbr.AppendLine(") b ON a.vcPart_id = b.vcPartNo AND b.vcSupplier_id = a.vcSupplier_id  AND a.vcReceiver = b.vcReceiver ");
                //sbr.AppendLine("WHERE  b.vcIntake IS NOT NULL ");
                //sbr.AppendLine(" ");
                //sbr.AppendLine("DECLARE @inPack INT ");
                //sbr.AppendLine("DECLARE @outPack INT ");
                //sbr.AppendLine(" ");
                //sbr.AppendLine("SELECT @inPack = COUNT(*)  FROM  ");
                //sbr.AppendLine("(SELECT * FROM #temp WHERE vcInOutflag = '0' ) a ");
                //sbr.AppendLine("LEFT JOIN ");
                //sbr.AppendLine("( ");
                //sbr.AppendLine("	SELECT * FROM dbo.VI_PackItem WHERE dUsedFrom <= @time AND dUsedTo >= @time");
                //sbr.AppendLine(") b ON a.vcPart_id = b.vcPartsNo  AND a.vcReceiver = b.vcShouhuofangID ");
                //sbr.AppendLine("WHERE  b.vcPackNo IS NOT NULL ");
                //sbr.AppendLine(" ");
                //sbr.AppendLine("SELECT @outPack = COUNT(*)  FROM  ");
                //sbr.AppendLine("(SELECT * FROM #temp WHERE vcInOutflag = '1' ) a ");
                //sbr.AppendLine("LEFT JOIN ");
                //sbr.AppendLine("( ");
                //sbr.AppendLine("	SELECT * FROM dbo.VI_PackItem  WHERE dUsedFrom <= @time AND dUsedTo >= @time");
                //sbr.AppendLine(") b ON a.vcPart_id = b.vcPartsNo  AND a.vcReceiver = b.vcShouhuofangID ");
                //sbr.AppendLine("WHERE  b.vcPackNo IS NOT NULL ");
                //sbr.AppendLine(" ");
                //sbr.AppendLine("SELECT @inSum AS inSum ,@outSum AS outSum,@inSum+@outSum AS total,@inSQ AS inSQ,@outSQ AS outSQ,@inPrice AS inPrice,@outPrice AS outPrice,@inHezi AS inHezi,@outHezi AS outHezi,@inPack AS inPack,@outPack AS outPack, ");
                //sbr.AppendLine("ltrim(Convert(numeric(9,2),@inSQ*100.0/(CASE @inSum WHEN 0 THEN 1 ELSE @inSum END)))+'%' As inSQPer,ltrim(Convert(numeric(9,2),@outSQ*100.0/(CASE @outSum WHEN 0 THEN 1 ELSE @outSum END)))+'%' As outSQPer, ");
                //sbr.AppendLine("ltrim(Convert(numeric(9,2),@inPrice*100.0/CASE @inSum WHEN 0 THEN 1 ELSE @inSum END))+'%' As inPricePer,ltrim(Convert(numeric(9,2),@outPrice*100.0/(CASE @outSum WHEN 0 THEN 1 ELSE @outSum END)))+'%' As outPricePer, ");
                //sbr.AppendLine("ltrim(Convert(numeric(9,2),@inHezi*100.0/CASE @inSum WHEN 0 THEN 1 ELSE @inSum END))+'%' As inHeziPer,ltrim(Convert(numeric(9,2),@outHezi*100.0/(CASE @outSum WHEN 0 THEN 1 ELSE @outSum END)))+'%' As outHeziPer, ");
                //sbr.AppendLine("ltrim(Convert(numeric(9,2),@inPack*100.0/CASE @inSum WHEN 0 THEN 1 ELSE @inSum END))+'%' As inPackPer,ltrim(Convert(numeric(9,2),@outPack*100.0/(CASE @outSum WHEN 0 THEN 1 ELSE @outSum END)))+'%' As outPackPer ");
                sbr.AppendLine("DECLARE @inSQ INT ");
                sbr.AppendLine("DECLARE @inSQALL INT ");
                sbr.AppendLine("DECLARE @outSQ INT ");
                sbr.AppendLine("DECLARE @outSQALL INT ");
                sbr.AppendLine(" ");
                sbr.AppendLine("SELECT @inSQALL = COUNT(*)  FROM  ");
                sbr.AppendLine("(SELECT * FROM VI_SQJD WHERE vcInOutflag = '0' ) a ");
                sbr.AppendLine("");
                sbr.AppendLine("SELECT @outSQALL = COUNT(*)  FROM  ");
                sbr.AppendLine("(SELECT * FROM VI_SQJD WHERE vcInOutflag = '1' ) a ");
                sbr.AppendLine("");
                sbr.AppendLine("SELECT @inSQ = COUNT(*)  FROM  ");
                sbr.AppendLine("(SELECT * FROM VI_SQJD WHERE vcInOutflag = '0' AND vcJD <> '1' ) a ");
                sbr.AppendLine("");
                sbr.AppendLine("SELECT @outSQ = COUNT(*)  FROM  ");
                sbr.AppendLine("(SELECT * FROM VI_SQJD WHERE vcInOutflag = '1' AND vcJD <> '1' ) a ");
                sbr.AppendLine("");
                sbr.AppendLine("DECLARE @inPrice INT ");
                sbr.AppendLine("DECLARE @inPriceALL INT ");
                sbr.AppendLine("DECLARE @outPrice INT ");
                sbr.AppendLine("DECLARE @outPriceALL INT ");
                sbr.AppendLine(" ");
                sbr.AppendLine("SELECT @inPriceALL = COUNT(*)  FROM  ");
                sbr.AppendLine("(SELECT * FROM dbo.VI_Price WHERE dPricebegin <= @time AND dPriceEnd >=  @time AND vcProjectType = '0') a ");
                sbr.AppendLine("");
                sbr.AppendLine("SELECT @outPriceALL = COUNT(*)  FROM  ");
                sbr.AppendLine("(SELECT * FROM dbo.VI_Price WHERE dPricebegin <= @time AND dPriceEnd >=  @time AND vcProjectType = '1') a ");
                sbr.AppendLine("");
                sbr.AppendLine("SELECT @inPrice = COUNT(*)  FROM  ");
                sbr.AppendLine("(SELECT * FROM dbo.VI_Price WHERE dPricebegin <= @time AND dPriceEnd >=  @time AND vcProjectType = '0' AND decPriceOrigin IS NOT NULL ) a ");
                sbr.AppendLine("");
                sbr.AppendLine("SELECT @outPrice = COUNT(*)  FROM  ");
                sbr.AppendLine("(SELECT * FROM dbo.VI_Price WHERE dPricebegin <= @time AND dPriceEnd >=  @time AND vcProjectType = '1' AND decPriceOrigin IS NOT NULL) a ");
                sbr.AppendLine("");
                sbr.AppendLine("DECLARE @inHezi INT ");
                sbr.AppendLine("DECLARE @inHeziALL INT ");
                sbr.AppendLine("DECLARE @outHezi INT ");
                sbr.AppendLine("DECLARE @outHeziALL INT ");
                sbr.AppendLine("");
                sbr.AppendLine("SET @inHezi = 0 ");
                sbr.AppendLine("SET @inHeziALL = 0 ");
                sbr.AppendLine("");
                sbr.AppendLine("SELECT @outHezi = COUNT(*)  FROM  ");
                sbr.AppendLine("(SELECT * FROM dbo.VI_HeZi  WHERE dUseStartDate <= @time AND dUserEndDate >=  @time AND vcIntake IS NOT NULL ) a ");
                sbr.AppendLine("");
                sbr.AppendLine("SELECT @outHeziALL = COUNT(*)  FROM  ");
                sbr.AppendLine("(SELECT * FROM dbo.VI_HeZi  WHERE dUseStartDate <= @time AND dUserEndDate >=  @time) a ");
                sbr.AppendLine("");
                sbr.AppendLine("");
                sbr.AppendLine("DECLARE @inPack INT ");
                sbr.AppendLine("DECLARE @intPackALL INT ");
                sbr.AppendLine("DECLARE @outPack INT ");
                sbr.AppendLine("DECLARE @outPackALL INT ");
                sbr.AppendLine("");
                sbr.AppendLine("SET @inPack = 0 ");
                sbr.AppendLine("SET @intPackALL = 0 ");
                sbr.AppendLine("");
                sbr.AppendLine("SELECT @outPack = COUNT(*)  FROM  ");
                sbr.AppendLine("(SELECT * FROM dbo.VI_PackItem WHERE dUsedFrom <= @time AND dUsedTo >= @time AND isnull(vcPackNo,'') <> ''  ) a ");
                sbr.AppendLine("");
                sbr.AppendLine("SELECT @outPackALL = COUNT(*)  FROM  ");
                sbr.AppendLine("(SELECT * FROM dbo.VI_PackItem WHERE dUsedFrom <= @time AND dUsedTo >= @time  ) a ");
                sbr.AppendLine("");
                sbr.AppendLine("SELECT @inSum AS inSum ,@outSum AS outSum,@inSum+@outSum AS total,@inSQ AS inSQ,@outSQ AS outSQ,@inPrice AS inPrice,@outPrice AS outPrice,@inHezi AS inHezi,@outHezi AS outHezi,@inPack AS inPack,@outPack AS outPack, ");
                sbr.AppendLine("ltrim(Convert(numeric(9,2),@inSQ*100.0/(CASE @inSQALL WHEN 0 THEN 1 ELSE @inSQALL END)))+'%' As inSQPer,ltrim(Convert(numeric(9,2),@outSQ*100.0/(CASE @outSQALL WHEN 0 THEN 1 ELSE @outSQALL END)))+'%' As outSQPer, ");
                sbr.AppendLine("ltrim(Convert(numeric(9,2),@inPrice*100.0/CASE @inPriceALL WHEN 0 THEN 1 ELSE @inPriceALL END))+'%' As inPricePer,ltrim(Convert(numeric(9,2),@outPrice*100.0/(CASE @outPriceALL WHEN 0 THEN 1 ELSE @outPriceALL END)))+'%' As outPricePer, ");
                sbr.AppendLine("ltrim(Convert(numeric(9,2),@inHezi*100.0/CASE @inHeziALL WHEN 0 THEN 1 ELSE @inHeziALL END))+'%' As inHeziPer,ltrim(Convert(numeric(9,2),@outHezi*100.0/(CASE @outHeziALL WHEN 0 THEN 1 ELSE @outHeziALL END)))+'%' As outHeziPer, ");
                sbr.AppendLine("ltrim(Convert(numeric(9,2),@inPack*100.0/CASE @intPackALL WHEN 0 THEN 1 ELSE @intPackALL END))+'%' As inPackPer,ltrim(Convert(numeric(9,2),@outPack*100.0/(CASE @outPackALL WHEN 0 THEN 1 ELSE @outPackALL END)))+'%' As outPackPer ");
                sbr.AppendLine("");


                return excute.ExcuteSqlWithSelectToDT(sbr.ToString(), "TK");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable getcarType()
        {
            try
            {
                StringBuilder sbr = new StringBuilder();
                sbr.AppendLine("SELECT distinct vcCarTypeDev FROM TUnit");
                return excute.ExcuteSqlWithSelectToDT(sbr.ToString(), "TK");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}