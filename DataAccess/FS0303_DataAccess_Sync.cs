using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess
{
    public class FS0303_DataAccess_Sync
    {
        
        #region 获取原单位向价格表同步的逻辑Sql语句
        /// <summary>
        /// 获取原单位向价格表同步的逻辑Sql语句
        /// </summary>
        /// <param name="listInfoData">需要同步的数据列表</param>
        /// <returns>sql语句</returns>
        public StringBuilder getDataSyncForPriceSql(List<Dictionary<string, Object>> listInfoData)
        {
            
            try
            {
                StringBuilder strSql = new StringBuilder();

                #region 获取临时表并将数据插入临时表
                strSql.Append(getTUnitTempTableSql(listInfoData));
                #endregion

                #region 不相同的数据插入价格表
                strSql.Append("       insert into TPrice       \r\n");
                strSql.Append("       (      \r\n");
                strSql.Append("       	 vcChange,vcPart_id,dUseBegin,dUseEnd,vcProjectType      \r\n");
                strSql.Append("       	,vcSupplier_id,vcSupplier_Name,dProjectBegin,dProjectEnd,vcHaoJiu      \r\n");
                strSql.Append("       	,dJiuBegin,dJiuEnd,dJiuBeginSustain,vcCarTypeDev,vcCarTypeDesign      \r\n");
                strSql.Append("       	,vcPart_Name,vcOE,vcPart_id_HK,vcStateFX,vcFXNO      \r\n");
                strSql.Append("       	,vcSumLater,vcReceiver,vcOriginCompany,dDataSyncTime      \r\n");
                strSql.Append("       )      \r\n");
                strSql.Append("       select       \r\n");
                strSql.Append("       	 a.vcChange,a.vcPart_id,a.dTimeFrom,a.dTimeTo,a.vcInOutflag      \r\n");
                strSql.Append("       	,a.vcSupplier_id,a.vcSupplier_Name,a.dGYSTimeFrom,a.dGYSTimeTo,a.vcHaoJiu      \r\n");
                strSql.Append("       	,a.dJiuBegin,a.dJiuEnd,a.vcJiuYear,a.vcCarTypeDev,a.vcCarTypeDesign      \r\n");
                strSql.Append("       	,a.vcPartNameEn,a.vcOE,a.vcHKPart_id,a.vcFXDiff,a.vcFXNo      \r\n");
                strSql.Append("       	,a.vcSumLater_Name,a.vcDownRecever,a.vcOriginCompany,GETDATE()      \r\n");
                strSql.Append("        from       \r\n");
                strSql.Append("       (      \r\n");
                strSql.Append("       	select a.*      \r\n");
                strSql.Append("       	,(CONVERT(int,vcNum1)+CONVERT(int,vcNum2)+CONVERT(int,vcNum3)      \r\n");
                strSql.Append("       	  +CONVERT(int,vcNum4)+CONVERT(int,vcNum5)+CONVERT(int,vcNum6)      \r\n");
                strSql.Append("       	  +CONVERT(int,vcNum7)+CONVERT(int,vcNum8)+CONVERT(int,vcNum9)      \r\n");
                strSql.Append("       	  +CONVERT(int,vcNum10)) as 'vcSumLater_Name'      \r\n");
                strSql.Append("       	,b.vcDownRecever from #Tunit_temp a      \r\n");
                strSql.Append("         inner join     \r\n");
                strSql.Append("         (    \r\n");
                strSql.Append("         	select vcValue1 as 'vcSYTCode',vcValue2 as 'vcRecever',vcValue3 as 'vcDownRecever' from TOutCode where vcCodeId = 'C004' and vcIsColum = 0    \r\n");
                strSql.Append("         ) b            \r\n");
                strSql.Append("       	on a.vcSYTCode = b.vcSYTCode      \r\n");
                strSql.Append("       	and a.vcReceiver = b.vcRecever      \r\n");
                strSql.Append("       ) a      \r\n");
                strSql.Append("       left join       \r\n");
                strSql.Append("       (      \r\n");
                strSql.Append("       	 select a.* from TPrice a      \r\n");
                strSql.Append("       	 inner join     \r\n");
                strSql.Append("       	 (     \r\n");
                strSql.Append("       	 	select vcPart_id,vcSupplier_id,MAX(iAutoId) as 'iMaxId' from TPrice     \r\n");
                strSql.Append("       	 	group by vcPart_id,vcSupplier_id     \r\n");
                strSql.Append("       	 ) b on a.iAutoId = b.iMaxId     \r\n");
                strSql.Append("       ) b      \r\n");
                strSql.Append("       on a.vcPart_id = b.vcPart_Id      \r\n");
                strSql.Append("       and a.vcDownRecever =b.vcReceiver      \r\n");
                strSql.Append("       and a.vcSupplier_id = b.vcSupplier_id      \r\n");
                strSql.Append("       and a.vcSYTCode = b.vcSYTCode      \r\n");
                strSql.Append("       where b.vcPart_Id is null      \r\n");
                strSql.Append("       and a.vcChange<>b.vcPriceChangeInfo      \r\n");
                #endregion

                #region 相同的数据进行更新操作
                strSql.Append("       update TPrice set       \r\n");
                strSql.Append("        dUseBegin = a.dTimeFrom      \r\n");
                strSql.Append("        dUseEnd = a.dTimeTo      \r\n");
                strSql.Append("        vcProjectType = a.vcInOutflag      \r\n");
                strSql.Append("       ,vcSupplier_Name = a.vcSupplier_Name      \r\n");
                strSql.Append("       ,dProjectBegin  = a.dGYSTimeFrom      \r\n");
                strSql.Append("       ,dProjectEnd = a.dGYSTimeTo      \r\n");
                strSql.Append("       ,vcHaoJiu = a.vcHaoJiu      \r\n");
                strSql.Append("       ,dJiuBegin = a.dJiuBegin      \r\n");
                strSql.Append("       ,dJiuEnd = a.dJiuEnd      \r\n");
                strSql.Append("       ,dJiuBeginSustain = a.dSSDate      \r\n");
                strSql.Append("       ,vcCarTypeDev = a.vcCarTypeDev      \r\n");
                strSql.Append("       ,vcCarTypeDesign = a.vcCarTypeDesign      \r\n");
                strSql.Append("       ,vcPart_Name = a.vcPartNameEn      \r\n");
                strSql.Append("       ,vcOE = a.vcOE      \r\n");
                strSql.Append("       ,vcPart_id_HK = a.vcHKPart_id      \r\n");
                strSql.Append("       ,vcStateFX = a.vcFXDiff      \r\n");
                strSql.Append("       ,vcFXNO = a.vcFXNo      \r\n");
                strSql.Append("       ,vcSumLater = a.vcSumLater      \r\n");
                strSql.Append("       ,vcOriginCompany = a.vcOriginCompany      \r\n");
                strSql.Append("       ,dDataSyncTime = GETDATE()      \r\n");
                strSql.Append("       from       \r\n");
                strSql.Append("       (      \r\n");
                strSql.Append("       	select a.*       \r\n");
                strSql.Append("       	,(      \r\n");
                strSql.Append("       	 CONVERT(int,vcNum1)      \r\n");
                strSql.Append("       	+CONVERT(int,vcNum2)      \r\n");
                strSql.Append("       	+CONVERT(int,vcNum3)      \r\n");
                strSql.Append("       	+CONVERT(int,vcNum4)      \r\n");
                strSql.Append("       	+CONVERT(int,vcNum5)      \r\n");
                strSql.Append("       	+CONVERT(int,vcNum6)      \r\n");
                strSql.Append("       	+CONVERT(int,vcNum7)      \r\n");
                strSql.Append("       	+CONVERT(int,vcNum8)      \r\n");
                strSql.Append("       	+CONVERT(int,vcNum9)      \r\n");
                strSql.Append("       	+CONVERT(int,vcNum10)      \r\n");
                strSql.Append("       	+CONVERT(int,vcNum11)      \r\n");
                strSql.Append("       	+CONVERT(int,vcNum12)      \r\n");
                strSql.Append("       	+CONVERT(int,vcNum13)      \r\n");
                strSql.Append("       	+CONVERT(int,vcNum14)      \r\n");
                strSql.Append("       	+CONVERT(int,vcNum15)      \r\n");
                strSql.Append("       	) as 'vcSumLater',vcDownRecever      \r\n");
                strSql.Append("       	from #TUnit_temp a      \r\n");
                strSql.Append("         inner join     \r\n");
                strSql.Append("         (    \r\n");
                strSql.Append("         	select vcValue1 as 'vcSYTCode',vcValue2 as 'vcRecever',vcValue3 as 'vcDownRecever' from TOutCode where vcCodeId = 'C004' and vcIsColum = 0    \r\n");
                strSql.Append("         ) b            \r\n");
                strSql.Append("       	on a.vcSYTCode = b.vcSYTCode      \r\n");
                strSql.Append("       	and a.vcReceiver = b.vcRecever      \r\n");
                strSql.Append("       ) a      \r\n");
                strSql.Append("       inner join       \r\n");
                strSql.Append("       (      \r\n");
                strSql.Append("       	select a.*,b.iMaxId from TPrice a      \r\n");
                strSql.Append("       		inner join       \r\n");
                strSql.Append("       		(      \r\n");
                strSql.Append("       			select vcPart_id,vcSupplier_id,MAX(iAutoId)as 'iMaxId' from TPrice      \r\n");
                strSql.Append("       			group by vcPart_id,vcSupplier_id      \r\n");
                strSql.Append("       		) b      \r\n");
                strSql.Append("       		on a.iAutoId = b.iMaxId      \r\n");
                strSql.Append("       ) b      \r\n");
                strSql.Append("       on  a.vcPart_id = b.vcPart_id     \r\n");
                strSql.Append("       and a.vcSupplier_id = b.vcSupplier_id      \r\n");
                strSql.Append("       and a.vcDownRecever = b.vcReceiver      \r\n");
                strSql.Append("       where a.vcChange = b.vcPriceChangeInfo      \r\n");
                #endregion

                return strSql;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 获取原单位生确单发行的逻辑Sql语句
        /// <summary>
        /// 获取原单位生确单发行的逻辑Sql语句
        /// </summary>
        /// <param name="listInfoData">需要发行的数据列表</param>
        /// <returns>sql语句</returns>
        public StringBuilder getSQSendSql(List<Dictionary<string, Object>> listInfoData)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                #region 获取临时表，将数据插入临时表
                strSql.Append(getTUnitTempTableSql(listInfoData));
                #endregion

                #region 根据品番、供应商、收货方、包装事业体，将原单位数据插入生确表
                strSql.AppendLine("        insert into TSQJD         ");
                strSql.AppendLine("        (         ");
                strSql.AppendLine("         dSSDate,vcJD,vcPart_id,vcSPINo,vcChange,vcCarType         ");
                strSql.AppendLine("         ,vcInOutflag,vcPartName,vcOE,vcSupplier_id,vcFXDiff,vcFXNo         ");
                strSql.AppendLine("         ,vcSumLater         ");
                strSql.AppendLine("         ,vcNum1,vcNum2,vcNum3,vcNum4,vcNum5,vcNum6         ");
                strSql.AppendLine("         ,vcNum7,vcNum8,vcNum9,vcNum10,vcSYTCode,vcSCSName         ");
                strSql.AppendLine("         ,vcSCPlace_City,vcCHPlace_City,vcSCSPlace,vcReceiver,dNqDate,GUID         ");
                strSql.AppendLine("        )         ");
                strSql.AppendLine("        select          ");
                strSql.AppendLine("        	a.dNqDate,'1' as 'vcJD',a.vcPart_id,a.vcSPINo,a.vcChange,a.vcCarTypeDev         ");
                strSql.AppendLine("           ,a.vcInOutflag,a.vcPartNameEn,a.vcOE,a.vcSupplier_id,a.vcFXDiff,a.vcFXNo         ");
                strSql.AppendLine("           ,a.vcSumLater_Name         ");
                strSql.AppendLine("           ,a.vcNum1,a.vcNum2,a.vcNum3,a.vcNum4,a.vcNum5,a.vcNum6         ");
                strSql.AppendLine("           ,a.vcNum7,a.vcNum8,a.vcNum9,a.vcNum10,a.vcSYTCode,a.vcProduct_name         ");
                strSql.AppendLine("           ,a.vcSCPlace,a.vcCHPlace,a.vcAddress,a.vcReceiver,a.dNqDate,'"+Guid.NewGuid().ToString()+"'         ");
                strSql.AppendLine("         from          ");
                strSql.AppendLine("        (         ");
                strSql.AppendLine("        	select a.*         ");
                strSql.AppendLine("        	,(CONVERT(int,vcNum1)+CONVERT(int,vcNum2)+CONVERT(int,vcNum3)          ");
                strSql.AppendLine("        	  +CONVERT(int,vcNum4)+CONVERT(int,vcNum5)+CONVERT(int,vcNum6)         ");
                strSql.AppendLine("        	  +CONVERT(int,vcNum7)+CONVERT(int,vcNum8)+CONVERT(int,vcNum9)         ");
                strSql.AppendLine("        	  +CONVERT(int,vcNum10)) as 'vcSumLater_Name',b.vcProduct_name,b.vcAddress         ");
                strSql.AppendLine("        	from #TUnit_temp a         ");
                strSql.AppendLine("        	 inner join          ");
                strSql.AppendLine("        	 (         ");
                strSql.AppendLine("        		select vcsupplier_id,vcProduct_name,vcAddress from TSupplier         ");
                strSql.AppendLine("        	 ) b on a.vcSupplier_id = b.vcSupplier_id         ");
                strSql.AppendLine("                 ");
                strSql.AppendLine("        ) a         ");
                strSql.AppendLine("                 ");
                #endregion

                return strSql;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 获取更新原单位纳期Sql语句
        /// <summary>
        /// 更新原单位信息
        /// </summary>
        /// <param name="strNqDate">纳期日期</param>
        /// <param name="listInfoData">要更改的数据集</param>
        /// <returns>sql语句</returns>
        public StringBuilder getUpdateUnitSql(string strNqDate,List<Dictionary<string, Object>> listInfoData)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                string date = DateTime.Now.ToString("yyyyMMdd");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    string iAuto = listInfoData[i]["iAutoId"].ToString();
                    strSql.AppendLine("      update TUnit set dNqDate = '"+strNqDate+"',vcSQContent = '确认中 "+date+ "',vcSQState = '1' where iAutoId = '" + iAuto+"'       \n");
                }
                return strSql;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 获取原单位的临时表(并且向临时表中插入数据)的Sql语句
        /// <summary>
        /// 获取原单位的临时表(并且向临时表中插入数据)的Sql语句
        /// </summary>
        /// <returns></returns>
        public StringBuilder getTUnitTempTableSql(List<Dictionary<string, Object>> listInfoData)
        {
            StringBuilder strSql = new StringBuilder();

            try
            {
                #region 创建一个原单位的临时表
                strSql = getTempTableSql("TUnit");
                #endregion

                #region 向临时表中插入数据
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    strSql.Append("      insert into #TUnit_temp       \r\n");
                    strSql.Append("      (       \r\n");
                    strSql.Append("      dSyncTime,vcChange,vcSPINo,vcSQState,vcDiff       \r\n");
                    strSql.Append("      ,vcPart_id,vcCarTypeDev,vcCarTypeDesign,vcCarTypeName,dTimeFrom       \r\n");
                    strSql.Append("      ,dTimeTo,dTimeFromSJ,vcBJDiff,vcPartReplace,vcPartNameEn       \r\n");
                    strSql.Append("      ,vcPartNameCn,vcHKGC,vcBJGC,vcInOutflag,vcSupplier_id       \r\n");
                    strSql.Append("      ,vcSupplier_Name,vcSCPlace,vcCHPlace,vcSYTCode,vcSCSName       \r\n");
                    strSql.Append("      ,vcSCSAdress,dGYSTimeFrom,dGYSTimeTo,vcOE,vcHKPart_id       \r\n");
                    strSql.Append("      ,vcHaoJiu,dJiuBegin,dJiuEnd,vcJiuYear,vcNXQF       \r\n");
                    strSql.Append("      ,dSSDate,vcMeno,vcFXDiff,vcFXNo,vcNum1       \r\n");
                    strSql.Append("      ,vcNum2,vcNum3,vcNum4,vcNum5,vcNum6       \r\n");
                    strSql.Append("      ,vcNum7,vcNum8,vcNum9,vcNum10,vcNum11       \r\n");
                    strSql.Append("      ,vcNum12,vcNum13,vcNum14,vcNum15,vcZXBZNo       \r\n");
                    strSql.Append("      ,vcReceiver,vcOriginCompany,dNqDate       \r\n");
                    strSql.Append("      )       \r\n");
                    strSql.Append("      values       \r\n");
                    strSql.Append("      (       \r\n");
                    strSql.Append("       GETDATE()       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcChange"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcSPINo"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcSQState"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcDiff"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcPart_id"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcCarTypeDev"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcCarTypeDesign"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcCarTypeName"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["dTimeFrom"], true) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["dTimeTo"], true) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["dTimeFromSJ"], true) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcBJDiff"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcPartReplace"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcPartNameEn"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcPartNameCn"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcHKGC"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcBJGC"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcInOutflag"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcSupplier_id"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcSupplier_Name"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcSCPlace"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcCHPlace"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcSYTCode"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcSCSName"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcSCSAdress"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["dGYSTimeFrom"], true) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["dGYSTimeTo"], true) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcOE"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcHKPart_id"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcHaoJiu"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["dJiuBegin"], true) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["dJiuEnd"], true) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcJiuYear"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNXQF"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["dSSDate"], true) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcMeno"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcFXDiff"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcFXNo"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum1"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum2"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum3"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum4"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum5"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum6"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum7"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum8"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum9"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum10"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum11"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum12"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum13"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum14"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcNum15"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcZXBZNo"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcReceiver"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["vcOriginCompany"], false) + "       \r\n");
                    strSql.Append("      ," + ComFunction.getSqlValue(listInfoData[i]["dNqDate"], true) + "       \r\n");
                    strSql.Append("      );       \r\n");
                }
                #endregion

                return strSql;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 获取一个临时表
        /// <summary>
        /// 获取一个临时表
        /// </summary>
        /// <param name="TableName">临时表名称</param>
        /// <returns>与此表结构相同的临时表，临时表名称#TableName_temp</returns>
        public StringBuilder getTempTableSql(string TableName)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                string tempTableName = "#" + TableName + "_temp";
                /*
                 * 时间：2021-02-05
                 * 修改人：董镇
                 * 内容描述：获取一个临时表，先判断临时表是否存在，如果存在，则删除；再创建临时表。
                 *           此临时表与原表结构相同，作为局部临时表存在,表里无数据
                 */
                strSql.Append("        if object_id('tempdb.." + tempTableName + "') is not null        \r\n");
                strSql.Append("        begin         \r\n");
                strSql.Append("        drop table " + tempTableName + "        \r\n");
                strSql.Append("        end        \r\n");
                strSql.Append("        select * into " + tempTableName + " from         \r\n");
                strSql.Append("        (        \r\n");
                strSql.Append("        select * from " + TableName + " where 1=0        \r\n");
                strSql.Append("        ) a ;       \r\n");
                return strSql;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 获取一个临时表
        /// <summary>
        /// 获取基于原单位表的临时表
        /// </summary>
        /// <param name="tempTableName">临时表名称</param>
        /// <returns></returns>
        public StringBuilder getTUnit2TempTable(string tempTableName)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                string TableName = "#" + tempTableName;
                strSql.Append("        if object_id('tempdb.." + TableName + "') is not null        \r\n");
                strSql.Append("        begin         \r\n");
                strSql.Append("        drop table " + TableName + "        \r\n");
                strSql.Append("        end        \r\n");
                strSql.Append("        select * into " + TableName + " from         \r\n");
                strSql.Append("        (        \r\n");
                strSql.Append("        select * from Tunit where 1=0        \r\n");
                strSql.Append("        ) a ;       \r\n");
                return strSql;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }
}
