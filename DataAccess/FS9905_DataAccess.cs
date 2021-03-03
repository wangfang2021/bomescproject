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
    public class FS9905_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 按检索条件返回dt
        public DataTable Search(string strJD, string strInOutFlag,string strSupplier_id,string strCarType,string strPart_id)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("     select a.*    \n");
                strSql.Append("     	,b1.vcName as 'vcJD_Name'    \n");
                strSql.Append("     	,b2.vcName as 'vcChange_Name'    \n");
                strSql.Append("     	,b3.vcName as 'vcInOutflag_Name'    \n");
                strSql.Append("     	,b4.vcName as 'vcOE_Name'        \n");
                strSql.Append("     	,b5.vcName as 'vcSYTCode_Name'      \n");
                strSql.Append("     	,b6.vcName as 'vcCarType_Name'      \n");
                strSql.Append("     	,b7.vcName as 'vcFXDiff_Name'       \n");
                strSql.Append("     	,b8.vcName as 'vcIsDYJG_Name'       \n");
                strSql.Append("     	,b9.vcName as 'vcIsDYFX_Name'       \n");
                strSql.Append("     	,case when (b.vcValue2='是') then '1' else '0' end as 'vcSupplierEditFlag'       \n");
                strSql.Append("     	,'0' as vcModFlag,'0' as vcAddFlag    \n");
                strSql.Append("     	,'0' as vcSCSNameModFlag,'0' as vcSCSPlaceModFlag    \n");
                strSql.Append("     from      \n");
                strSql.Append("     (    \n");
                strSql.Append("     		select a.* from     \n");
                strSql.Append("     		(    \n");
                strSql.Append("     			select iAutoId,dSSDate,case when vcJD='1' then '1' when vcJD='2' then '2' when vcJD = '3' then '3' when vcJD='4'then '2' end as 'vcJD'    \n");
                strSql.Append("     				  ,vcPart_id,vcSPINo,vcChange,vcCarType,vcInOutflag,vcPartName,vcOE    \n");
                strSql.Append("     			      ,vcSupplier_id,vcFXDiff,vcFXNo,vcSumLater,vcNum1,vcNum2,vcNum3,vcNum4,vcNum5,vcNum6    \n");
                strSql.Append("     				  ,vcNum7,vcNum8,vcNum9,vcNum10,vcIsDYJG,vcIsDYFX,vcYQorNG,vcNotDY,vcTH,vcSCPlace_City    \n");
                strSql.Append("     				  ,vcSCPlace_Province,vcCHPlace_City,vcCHPlace_Province,vcSYTCode,vcSCSName,vcSCSPlace,dSupplier_BJ,dSupplier_HK,dTFTM_BJ,vcZXBZDiff    \n");
                strSql.Append("     				  ,vcZXBZNo,vcReceiver,dNqDate,vcOperatorId,dOperatorTime,dHFDate,GUID from     \n");
                strSql.Append("     				  (    \n");
                strSql.Append("     						select * from TSQJD    \n");
                strSql.Append("     						where 1=1    \n");
                if (!string.IsNullOrEmpty(strJD))
                {
                    if (strJD == "4")
                    {
                        strSql.Append("      and vcJD in ('1','3')  ");
                    }
                    else
                    {
                        strSql.Append("      and vcJD = '" + strJD + "'  ");
                    }
                }
                if (!string.IsNullOrEmpty(strInOutFlag))
                {
                    strSql.Append("      and vcInOutflag = '" + strInOutFlag + "'   ");
                }
                if (!string.IsNullOrEmpty(strSupplier_id))
                {
                    strSql.Append("      and vcSupplier_id like '" + strSupplier_id + "%'   ");
                }
                if (!string.IsNullOrEmpty(strCarType))
                {
                    strSql.Append("      and b6.vcName like '" + strCarType + "%'   ");
                }
                if (!string.IsNullOrEmpty(strPart_id))
                {
                    strSql.Append("      and vcPart_id like '" + strPart_id + "%'       ");
                }
                strSql.Append("     				  )a    \n");
                strSql.Append("     		) a    \n");
                strSql.Append("     		left join     \n");
                strSql.Append("     		(    \n");
                strSql.Append("     			select GUID from TSQJD_Supplier    \n");
                strSql.Append("     		) b on a.GUID = b.GUID    \n");
                strSql.Append("     		where b.GUID is null    \n");
                strSql.Append("     		union all    \n");
                strSql.Append("     		(    \n");
                strSql.Append("     			select iAutoId,dSSDate,case when vcJD='1' then '1' when vcJD='2' then '2' when vcJD = '3' then '3' when vcJD='4'then '2' end as 'vcJD'    \n");
                strSql.Append("     			      ,vcPart_id,vcSPINo,vcChange,vcCarType,vcInOutflag,vcPartName,vcOE    \n");
                strSql.Append("     				  ,vcSupplier_id,vcFXDiff,vcFXNo,vcSumLater,vcNum1,vcNum2,vcNum3,vcNum4,vcNum5,vcNum6    \n");
                strSql.Append("     				  ,vcNum7,vcNum8,vcNum9,vcNum10,vcIsDYJG,vcIsDYFX,vcYQorNG,vcNotDY,vcTH,vcSCPlace_City    \n");
                strSql.Append("     				  ,vcSCPlace_Province,vcCHPlace_City,vcCHPlace_Province,vcSYTCode,vcSCSName,vcSCSPlace,dSupplier_BJ,dSupplier_HK,dTFTM_BJ,vcZXBZDiff    \n");
                strSql.Append("     				  ,vcZXBZNo,vcReceiver,dNqDate,vcOperatorId,dOperatorTime,dHFDate,GUID     \n");
                strSql.Append("     			from     \n");
                strSql.Append("     			(    \n");
                strSql.Append("     				select * from TSQJD_Supplier    \n");
                strSql.Append("     				where 1=1    \n");
                if (!string.IsNullOrEmpty(strJD))
                {
                    if (strJD == "4")
                    {
                        strSql.Append("      and vcJD in ('1','3')  ");
                    }
                    else
                    {
                        strSql.Append("      and vcJD = '" + strJD + "'  ");
                    }
                }
                if (!string.IsNullOrEmpty(strInOutFlag))
                {
                    strSql.Append("      and vcInOutflag = '" + strInOutFlag + "'   ");
                }
                if (!string.IsNullOrEmpty(strSupplier_id))
                {
                    strSql.Append("      and vcSupplier_id like '" + strSupplier_id + "%'   ");
                }
                if (!string.IsNullOrEmpty(strCarType))
                {
                    strSql.Append("      and b6.vcName like '" + strCarType + "%'   ");
                }
                if (!string.IsNullOrEmpty(strPart_id))
                {
                    strSql.Append("      and vcPart_id like '" + strPart_id + "%'       ");
                }
                strSql.Append("     			)a    \n");
                strSql.Append("     		)    \n");
                strSql.Append("     )a    \n");
                strSql.Append("     inner join     \n");
                strSql.Append("     (    \n");
                strSql.Append("     	select vcValue,vcName from TCode where vcCodeId = 'C026'    \n");
                strSql.Append("     )b1 on a.vcJD = b1.vcValue    \n");
                strSql.Append("     inner join     \n");
                strSql.Append("     (    \n");
                strSql.Append("     	select vcValue,vcName from TCode where vcCodeId = 'C002'    \n");
                strSql.Append("     )b2 on a.vcChange = b2.vcValue    \n");
                strSql.Append("     inner join     \n");
                strSql.Append("     (    \n");
                strSql.Append("     	select vcValue, vcName from TCode where vcCodeId = 'C003'    \n");
                strSql.Append("     )b3 on a.vcInOutflag = b3.vcValue    \n");
                strSql.Append("     inner join     \n");
                strSql.Append("     (    \n");
                strSql.Append("     	select vcValue,vcName from TCode where vcCodeId = 'C012'    \n");
                strSql.Append("     )b4 on a.vcOE = b4.vcValue    \n");
                strSql.Append("     inner join     \n");
                strSql.Append("     (    \n");
                strSql.Append("     	select vcValue,vcName from TCode where vcCodeId = 'C023'    \n");
                strSql.Append("     )b5 on a.vcSYTCode = b5.vcValue    \n");
                strSql.Append("     inner join     \n");
                strSql.Append("     (    \n");
                strSql.Append("     	select vcValue,vcName from TCode where vcCodeId = 'C098'    \n");
                strSql.Append("     )b6 on a.vcCarType = b6.vcValue    \n");
                strSql.Append("     inner join     \n");
                strSql.Append("     (    \n");
                strSql.Append("     select vcValue,vcName from TCode where vcCodeId = 'C028'    \n");
                strSql.Append("     )b7 on a.vcFXDiff = b7.vcValue    \n");
                strSql.Append("     inner join     \n");
                strSql.Append("     (    \n");
                strSql.Append("     select vcValue,vcName from TCode where vcCodeId = 'C029'    \n");
                strSql.Append("     )b8 on a.vcIsDYJG = b8.vcValue    \n");
                strSql.Append("     inner join     \n");
                strSql.Append("     (    \n");
                strSql.Append("     select vcValue,vcName from TCode where vcCodeId = 'C030'    \n");
                strSql.Append("     )b9 on a.vcIsDYFX = b9.vcValue    \n");
                strSql.Append("     left join     \n");
                strSql.Append("     (    \n");
                strSql.Append("     select vcValue1,vcValue2 from TOutCode where vcCodeId = 'C008' and vcIsColum = '0'    \n");
                strSql.Append("     )b10 on a.vcIsDYFX = b9.vcValue    \n");
                strSql.Append("     order by vcPart_id    \n");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString(), "TK");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 初始化检索
        public DataTable Search()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("     select a.*    \n");
                strSql.Append("     	,b1.vcName as 'vcJD_Name'    \n");
                strSql.Append("     	,b2.vcName as 'vcChange_Name'    \n");
                strSql.Append("     	,b3.vcName as 'vcInOutflag_Name'    \n");
                strSql.Append("     	,b4.vcName as 'vcOE_Name'        \n");
                strSql.Append("     	,b5.vcName as 'vcSYTCode_Name'      \n");
                strSql.Append("     	,b6.vcName as 'vcCarType_Name'      \n");
                strSql.Append("     	,b7.vcName as 'vcFXDiff_Name'       \n");
                strSql.Append("     	,b8.vcName as 'vcIsDYJG_Name'       \n");
                strSql.Append("     	,b9.vcName as 'vcIsDYFX_Name'       \n");
                strSql.Append("     	,'0' as vcModFlag,'0' as vcAddFlag    \n");
                strSql.Append("     	,'0' as vcSCSNameModFlag,'0' as vcSCSPlaceModFlag    \n");
                strSql.Append("     from      \n");
                strSql.Append("     (    \n");
                strSql.Append("     		select a.* from     \n");
                strSql.Append("     		(    \n");
                strSql.Append("     			select iAutoId,dSSDate,case when vcJD='1' then '1' when vcJD='2' then '2' when vcJD = '3' then '3' when vcJD='4'then '2' end as 'vcJD'    \n");
                strSql.Append("     				  ,vcPart_id,vcSPINo,vcChange,vcCarType,vcInOutflag,vcPartName,vcOE    \n");
                strSql.Append("     			      ,vcSupplier_id,vcFXDiff,vcFXNo,vcSumLater,vcNum1,vcNum2,vcNum3,vcNum4,vcNum5,vcNum6    \n");
                strSql.Append("     				  ,vcNum7,vcNum8,vcNum9,vcNum10,vcIsDYJG,vcIsDYFX,vcYQorNG,vcNotDY,vcTH,vcSCPlace_City    \n");
                strSql.Append("     				  ,vcSCPlace_Province,vcCHPlace_City,vcCHPlace_Province,vcSYTCode,vcSCSName,vcSCSPlace,dSupplier_BJ,dSupplier_HK,dTFTM_BJ,vcZXBZDiff    \n");
                strSql.Append("     				  ,vcZXBZNo,vcReceiver,dNqDate,vcOperatorId,dOperatorTime,dHFDate,GUID from     \n");
                strSql.Append("     				  (    \n");
                strSql.Append("     						select * from TSQJD    \n");
                strSql.Append("     						where 1=1    \n");
                strSql.Append("                             and vcJD in ('1','3')  ");
                
                strSql.Append("     				  )a    \n");
                strSql.Append("     		) a    \n");
                strSql.Append("     		left join     \n");
                strSql.Append("     		(    \n");
                strSql.Append("     			select GUID from TSQJD_Supplier    \n");
                strSql.Append("     		) b on a.GUID = b.GUID    \n");
                strSql.Append("     		where b.GUID is null    \n");
                strSql.Append("     		union all    \n");
                strSql.Append("     		(    \n");
                strSql.Append("     			select iAutoId,dSSDate,case when vcJD='1' then '1' when vcJD='2' then '2' when vcJD = '3' then '3' when vcJD='4'then '2' end as 'vcJD'    \n");
                strSql.Append("     			      ,vcPart_id,vcSPINo,vcChange,vcCarType,vcInOutflag,vcPartName,vcOE    \n");
                strSql.Append("     				  ,vcSupplier_id,vcFXDiff,vcFXNo,vcSumLater,vcNum1,vcNum2,vcNum3,vcNum4,vcNum5,vcNum6    \n");
                strSql.Append("     				  ,vcNum7,vcNum8,vcNum9,vcNum10,vcIsDYJG,vcIsDYFX,vcYQorNG,vcNotDY,vcTH,vcSCPlace_City    \n");
                strSql.Append("     				  ,vcSCPlace_Province,vcCHPlace_City,vcCHPlace_Province,vcSYTCode,vcSCSName,vcSCSPlace,dSupplier_BJ,dSupplier_HK,dTFTM_BJ,vcZXBZDiff    \n");
                strSql.Append("     				  ,vcZXBZNo,vcReceiver,dNqDate,vcOperatorId,dOperatorTime,dHFDate,GUID     \n");
                strSql.Append("     			from     \n");
                strSql.Append("     			(    \n");
                strSql.Append("     				select * from TSQJD_Supplier    \n");
                strSql.Append("     				where 1=1    \n");
                strSql.Append("                     and vcJD in ('1','3')  ");
                strSql.Append("     			)a    \n");
                strSql.Append("     		)    \n");
                strSql.Append("     )a    \n");
                strSql.Append("     inner join     \n");
                strSql.Append("     (    \n");
                strSql.Append("     	select vcValue,vcName from TCode where vcCodeId = 'C026'    \n");
                strSql.Append("     )b1 on a.vcJD = b1.vcValue    \n");
                strSql.Append("     left join     \n");
                strSql.Append("     (    \n");
                strSql.Append("     	select vcValue,vcName from TCode where vcCodeId = 'C002'    \n");
                strSql.Append("     )b2 on a.vcChange = b2.vcValue    \n");
                strSql.Append("     left join     \n");
                strSql.Append("     (    \n");
                strSql.Append("     	select vcValue, vcName from TCode where vcCodeId = 'C003'    \n");
                strSql.Append("     )b3 on a.vcInOutflag = b3.vcValue    \n");
                strSql.Append("     left join     \n");
                strSql.Append("     (    \n");
                strSql.Append("     	select vcValue,vcName from TCode where vcCodeId = 'C012'    \n");
                strSql.Append("     )b4 on a.vcOE = b4.vcValue    \n");
                strSql.Append("     left join     \n");
                strSql.Append("     (    \n");
                strSql.Append("     	select vcValue,vcName from TCode where vcCodeId = 'C023'    \n");
                strSql.Append("     )b5 on a.vcSYTCode = b5.vcValue    \n");
                strSql.Append("     left join     \n");
                strSql.Append("     (    \n");
                strSql.Append("     	select vcValue,vcName from TCode where vcCodeId = 'C098'    \n");
                strSql.Append("     )b6 on a.vcCarType = b6.vcValue    \n");
                strSql.Append("     left join     \n");
                strSql.Append("     (    \n");
                strSql.Append("     select vcValue,vcName from TCode where vcCodeId = 'C028'    \n");
                strSql.Append("     )b7 on a.vcFXDiff = b7.vcValue    \n");
                strSql.Append("     left join     \n");
                strSql.Append("     (    \n");
                strSql.Append("     select vcValue,vcName from TCode where vcCodeId = 'C029'    \n");
                strSql.Append("     )b8 on a.vcIsDYJG = b8.vcValue    \n");
                strSql.Append("     left join     \n");
                strSql.Append("     (    \n");
                strSql.Append("     select vcValue,vcName from TCode where vcCodeId = 'C030'    \n");
                strSql.Append("     )b9 on a.vcIsDYFX = b9.vcValue    \n");
                strSql.Append("     order by vcPart_id    \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString(), "TK");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErr)
        {
            try
            {
                StringBuilder sqlStr = new StringBuilder();

                #region 创建生确进度临时表
                sqlStr.Append("        if object_id('tempdb..#TSQJD_temp') is not null        \r\n");
                sqlStr.Append("        begin         \r\n");
                sqlStr.Append("        drop table #TSQJD_temp        \r\n");
                sqlStr.Append("        end        \r\n");
                sqlStr.Append("        select * into #TSQJD_temp from TSQJD        \r\n");
                sqlStr.Append("        where 1=0 ;       \r\n");
                #endregion

                #region 将有的数据都插入临时表
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    sqlStr.Append("        insert into #TSQJD_temp         ");
                    sqlStr.Append("        (         ");
                    sqlStr.Append("        	 dSSDate,vcJD,vcPart_id,vcSPINo,vcChange         ");
                    sqlStr.Append("        	,vcCarType,vcInOutflag,vcPartName,vcOE,vcSupplier_id         ");
                    sqlStr.Append("        	,vcFXDiff,vcFXNo,vcSumLater,vcNum1,vcNum2         ");
                    sqlStr.Append("        	,vcNum3,vcNum4,vcNum5,vcNum6,vcNum7         ");
                    sqlStr.Append("        	,vcNum8,vcNum9,vcNum10,vcIsDYJG,vcIsDYFX         ");
                    sqlStr.Append("        	,vcYQorNG,vcNotDY,vcTH,vcSCPlace_City,vcSCPlace_Province         ");
                    sqlStr.Append("        	,vcCHPlace_City,vcCHPlace_Province,vcSYTCode,vcSCSName,vcSCSPlace         ");
                    sqlStr.Append("        	,dSupplier_BJ,dSupplier_HK,dTFTM_BJ,vcZXBZDiff,vcZXBZNo         ");
                    sqlStr.Append("        	,vcReceiver,dNqDate,vcOperatorId,dOperatorTime,dHFDate         ");
                    sqlStr.Append("        	,GUID         ");
                    sqlStr.Append("        )         ");
                    sqlStr.Append("        values         ");
                    sqlStr.Append("        (         ");
                    sqlStr.Append("         "+ComFunction.getSqlValue(listInfoData[i]["dSSDate"],true)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["vcJD"],false)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["vcPart_id"],false)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["vcSPINo"],false)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["vcChange"],false)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["vcCarType"],false)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["vcInOutflag"],false)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["vcPartName"],false)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["vcOE"],false)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["vcSupplier_id"],false)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["vcFXDiff"],false)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["vcFXNo"],false)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["vcSumLater"],false)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["vcNum1"],true)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["vcNum2"],true)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["vcNum3"],true)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["vcNum4"],true)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["vcNum5"],true)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["vcNum6"],true)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["vcNum7"],true)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["vcNum8"],true)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["vcNum9"],true)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["vcNum10"],true)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["vcIsDYJG"],false)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["vcIsDYFX"],false)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["vcYQorNG"],false)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["vcNotDY"],false)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["vcTH"],false)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["vcSCPlace_City"],false)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["vcSCPlace_Province"],false)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["vcCHPlace_City"],false)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["vcCHPlace_Province"],false)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["vcSYTCode"],false)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["vcSCSName"],false)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["vcSCSPlace"],false)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["dSupplier_BJ"],true)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["dSupplier_HK"],true)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["dTFTM_BJ"],true)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["vcZXBZDiff"],false)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["vcZXBZNo"],false)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["vcReceiver"],false)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["dNqDate"],true)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["vcOperatorId"],false)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["dOperatorTime"],true)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["dHFDate"],true)+"         ");
                    sqlStr.Append("        ,"+ComFunction.getSqlValue(listInfoData[i]["GUID"],false)+ "         ");
                    sqlStr.Append("        )         ");
                }
                #endregion

                #region 根据GUID插入或更新供应商生确表
                sqlStr.Append("        insert into TSQJD_Supplier       \r\n");
                sqlStr.Append("        (       \r\n");
                sqlStr.Append("        	 dSSDate,vcJD,vcPart_id,vcSPINo,vcChange       \r\n");
                sqlStr.Append("        	,vcCarType,vcInOutflag,vcPartName,vcOE,vcSupplier_id       \r\n");
                sqlStr.Append("        	,vcFXDiff,vcFXNo,vcSumLater,vcNum1,vcNum2       \r\n");
                sqlStr.Append("        	,vcNum3,vcNum4,vcNum5,vcNum6,vcNum7       \r\n");
                sqlStr.Append("        	,vcNum8,vcNum9,vcNum10,vcIsDYJG,vcIsDYFX       \r\n");
                sqlStr.Append("        	,vcYQorNG,vcNotDY,vcTH,vcSCPlace_City,vcSCPlace_Province       \r\n");
                sqlStr.Append("        	,vcCHPlace_City,vcCHPlace_Province,vcSYTCode,vcSCSName,vcSCSPlace       \r\n");
                sqlStr.Append("        	,dSupplier_BJ,dSupplier_HK,dTFTM_BJ,vcZXBZDiff,vcZXBZNo       \r\n");
                sqlStr.Append("        	,vcReceiver,dNqDate,vcOperatorId,dOperatorTime,dHFDate       \r\n");
                sqlStr.Append("        	,GUID       \r\n");
                sqlStr.Append("        )       \r\n");
                sqlStr.Append("        select        \r\n");
                sqlStr.Append("        	 dSSDate,vcJD,vcPart_id,vcSPINo,vcChange       \r\n");
                sqlStr.Append("        	,vcCarType,vcInOutflag,vcPartName,vcOE,vcSupplier_id       \r\n");
                sqlStr.Append("        	,vcFXDiff,vcFXNo,vcSumLater,vcNum1,vcNum2       \r\n");
                sqlStr.Append("        	,vcNum3,vcNum4,vcNum5,vcNum6,vcNum7       \r\n");
                sqlStr.Append("        	,vcNum8,vcNum9,vcNum10,vcIsDYJG,vcIsDYFX       \r\n");
                sqlStr.Append("        	,vcYQorNG,vcNotDY,vcTH,vcSCPlace_City,vcSCPlace_Province       \r\n");
                sqlStr.Append("        	,vcCHPlace_City,vcCHPlace_Province,vcSYTCode,vcSCSName,vcSCSPlace       \r\n");
                sqlStr.Append("        	,dSupplier_BJ,dSupplier_HK,dTFTM_BJ,vcZXBZDiff,vcZXBZNo       \r\n");
                sqlStr.Append("        	,vcReceiver,dNqDate,vcOperatorId,dOperatorTime,dHFDate       \r\n");
                sqlStr.Append("        	,a.GUID       \r\n");
                sqlStr.Append("         from #TSQJD_temp a       \r\n");
                sqlStr.Append("         left join        \r\n");
                sqlStr.Append("         (       \r\n");
                sqlStr.Append("        	select GUID from TSQJD_Supplier       \r\n");
                sqlStr.Append("         ) b       \r\n");
                sqlStr.Append("         on a.GUID = b.GUID       \r\n");
                sqlStr.Append("         where b.GUID is null       \r\n");


                sqlStr.Append("         update TSQJD_Supplier set        \r\n");
                sqlStr.Append("         dSSDate = b.dSSDate       \r\n");
                sqlStr.Append("         ,vcJD = b.vcJD       \r\n");
                sqlStr.Append("         ,vcPart_id = b.vcPart_id       \r\n");
                sqlStr.Append("         ,vcSPINo = b.vcSPINo       \r\n");
                sqlStr.Append("         ,vcChange = b.vcChange       \r\n");
                sqlStr.Append("         ,vcCarType = b.vcCarType       \r\n");
                sqlStr.Append("         ,vcInOutflag = b.vcInOutflag       \r\n");
                sqlStr.Append("         ,vcPartName = b.vcPartName       \r\n");
                sqlStr.Append("         ,vcOE = b.vcOE       \r\n");
                sqlStr.Append("         ,vcSupplier_id = b.vcSupplier_id       \r\n");
                sqlStr.Append("         ,vcFXDiff = b.vcFXDiff       \r\n");
                sqlStr.Append("         ,vcFXNo = b.vcFXNo       \r\n");
                sqlStr.Append("         ,vcSumLater = b.vcSumLater       \r\n");
                sqlStr.Append("         ,vcNum1 = b.vcNum1       \r\n");
                sqlStr.Append("         ,vcNum2 = b.vcNum2       \r\n");
                sqlStr.Append("         ,vcNum3 = b.vcNum3       \r\n");
                sqlStr.Append("         ,vcNum4 = b.vcNum4       \r\n");
                sqlStr.Append("         ,vcNum5 = b.vcNum5       \r\n");
                sqlStr.Append("         ,vcNum6 = b.vcNum6       \r\n");
                sqlStr.Append("         ,vcNum7 = b.vcNum7       \r\n");
                sqlStr.Append("         ,vcNum8 = b.vcNum8       \r\n");
                sqlStr.Append("         ,vcNum9 = b.vcNum9       \r\n");
                sqlStr.Append("         ,vcNum10 = b.vcNum10       \r\n");
                sqlStr.Append("         ,vcIsDYJG = b.vcIsDYFX       \r\n");
                sqlStr.Append("         ,vcIsDYFX = b.vcIsDYFX       \r\n");
                sqlStr.Append("         ,vcYQorNG = b.vcYQorNG       \r\n");
                sqlStr.Append("         ,vcNotDY = b.vcNotDY       \r\n");
                sqlStr.Append("         ,vcTH = b.vcTH       \r\n");
                sqlStr.Append("         ,vcSCPlace_City = b.vcSCPlace_City       \r\n");
                sqlStr.Append("         ,vcSCPlace_Province = b.vcSCPlace_Province       \r\n");
                sqlStr.Append("         ,vcCHPlace_City = b.vcCHPlace_City       \r\n");
                sqlStr.Append("         ,vcCHPlace_Province = b.vcCHPlace_Province       \r\n");
                sqlStr.Append("         ,vcSYTCode = b.vcSYTCode       \r\n");
                sqlStr.Append("         ,vcSCSName = b.vcSCSName       \r\n");
                sqlStr.Append("         ,vcSCSPlace = b.vcSCSPlace       \r\n");
                sqlStr.Append("         ,dSupplier_BJ = b.dSupplier_BJ       \r\n");
                sqlStr.Append("         ,dSupplier_HK = b.dSupplier_HK       \r\n");
                sqlStr.Append("         ,dTFTM_BJ = b.dTFTM_BJ       \r\n");
                sqlStr.Append("         ,vcZXBZDiff = b.vcZXBZDiff       \r\n");
                sqlStr.Append("         ,vcZXBZNo = b.vcZXBZNo       \r\n");
                sqlStr.Append("         ,vcReceiver = b.vcReceiver       \r\n");
                sqlStr.Append("         ,dNqDate = b.dNqDate       \r\n");
                sqlStr.Append("         ,vcOperatorId = b.vcOperatorId       \r\n");
                sqlStr.Append("         ,dOperatorTime = b.dOperatorTime       \r\n");
                sqlStr.Append("         ,dHFDate = b.dHFDate       \r\n");
                sqlStr.Append("         ,GUID = b.GUID       \r\n");
                sqlStr.Append("         from TSQJD_Supplier a       \r\n");
                sqlStr.Append("         inner join        \r\n");
                sqlStr.Append("         (       \r\n");
                sqlStr.Append("         	select * from #TSQJD_temp       \r\n");
                sqlStr.Append("         ) b on a.GUID = b.GUID       \r\n");
                #endregion

                if (sqlStr.Length>0)
                {
                    excute.CommonExcuteNonQuery(sqlStr.ToString(), "TK");
                }
            }
            catch (Exception ex)
            {
                strErr = ex.Message;
                throw ex;
            }
        }

        #endregion

        #region 生确回复
        public void Send(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            
            #region 将进度变为已退回,清空退回理由
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);

                    sql.Append("      update TSQJD set vcJD =      \n");
                    sql.Append("      (      \n");
                    sql.Append("      select vcValue from TCode where vcCodeId = 'C026' and vcName = '已回复'      \n");
                    sql.Append("      ),      \n");
                    sql.Append("      vcOperatorId = '" + strUserId + "',      \n");
                    //sql.Append("      vcTH = null,      \n");
                    sql.Append("      dOperatorTime = GETDATE()      \n");
                    sql.Append("      where iAutoId = '" + iAutoId + "'      \n");
                }
                excute.ExcuteSqlWithStringOper(sql.ToString(), "TK");
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("-->") != -1)
                {//主动判断抛出的异常
                    int startIndex = ex.Message.IndexOf("-->");
                    int endIndex = ex.Message.LastIndexOf("<--");
                    strErrorPartId = ex.Message.Substring(startIndex + 3, endIndex - startIndex - 3);
                }
                else
                    throw ex;
            }
            #endregion

        }
        #endregion
         

        /*
         * 保留问题
         * TFTM生确是否可存在多条相同主键的数据
         * 如果可以存在多条形同的数据，供应商生确，一括付与无法完成     注：仅限当前方式
         * 无法完成原因：例子：原单位生确单发行，此时，TFTM生确中所有的数据都是已联络，供应商此时可以进行一括付与操作
         * 但是，注意供应商子表中并没有数据，如果将TFTM生确中的所有数据插入到供应商生确子表中，那么如何获取用户所选择的那一条数据？AutoId此时是
         * TFTM生确表中的AutoId,与供应商生确表中的AutoId并不一致
         * 如果按照主键的话会涉及到TFTM表中存在多条相同主键的数据。
         */
        #region 一括付与
        public void SetFY(List<Dictionary<string, Object>> listInfoData, string strSupplier_BJ, string strSupplier_HK,string strSCPlace_City, string strSCPlace_Province, string strCHPlace_City, string strCHPlace_Province, string strUserId, ref string strErr)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);

                    sql.AppendLine("      update TSQJD_Supplier set    \n");
                    if (!string.IsNullOrEmpty(strSupplier_BJ))
                    {
                        sql.AppendLine("   dSupplier_BJ = '" + strSupplier_BJ + "'          ");
                    }
                    if (!string.IsNullOrEmpty(strSupplier_HK))
                    {
                        sql.AppendLine("  ,dSupplier_HK = '" + strSupplier_HK + "'          ");
                    }
                    
                    if (!string.IsNullOrEmpty(strSupplier_BJ) || !string.IsNullOrEmpty(strSupplier_HK))
                    {
                        sql.AppendLine("     ,vcIsDYJG = '1'           ");
                        sql.AppendLine("     ,vcIsDYFX = '1'           ");
                    }
                    if (!string.IsNullOrEmpty(strSCPlace_City))
                    {
                        sql.AppendLine("  ,vcSCPlace_City = '" + strSCPlace_City + "'          ");
                    }
                    if (!string.IsNullOrEmpty(strSCPlace_Province))
                    {
                        sql.AppendLine("  ,vcSCPlace_Province = '" + strSCPlace_City + "'          ");
                    }
                    if (!string.IsNullOrEmpty(strCHPlace_City))
                    {
                        sql.AppendLine("  ,vcCHPlace_City = '" + strCHPlace_City + "'          ");
                    }
                    if (!string.IsNullOrEmpty(strCHPlace_Province))
                    {
                        sql.AppendLine("  ,vcCHPlace_Province = '" + strCHPlace_Province + "'          ");
                    }
                    sql.AppendLine("       ,vcOperatorId = '" + strUserId + "'      \n");
                    sql.AppendLine("       ,dOperatorTime = GETDATE()      \n");
                    sql.AppendLine("       where iAutoId = '" + iAutoId + "'      \n");
                    sql.AppendLine("       and vcJD in ('1','3')         ");
                    sql.AppendLine("       and vcSCPlace_City is null         ");
                    sql.AppendLine("       or vcSCPlace_City = ''        ");
                    sql.AppendLine("       and strSCPlace_Province is null         ");
                    sql.AppendLine("       or strSCPlace_Province = ''        ");
                    sql.AppendLine("       and strCHPlace_City is null         ");
                    sql.AppendLine("       or strCHPlace_City = ''        ");
                    sql.AppendLine("       and strCHPlace_Province is null         ");
                    sql.AppendLine("       or strCHPlace_Province = ''        ");
                }
                excute.ExcuteSqlWithStringOper(sql.ToString(), "TK");
            }
            catch (Exception ex)
            {
                strErr = ex.Message;
            }
        }
        #endregion

    }
}
