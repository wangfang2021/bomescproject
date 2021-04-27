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
    public class FS0305_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 按检索条件返回dt
        public DataTable Search(string strJD, string strInOutFlag, string strSupplier_id, string strCarType, string strPart_id, string strUserID)
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
                strSql.Append("     	,b10.iNum    \n");
                strSql.Append("     	,'0' as vcModFlag,'0' as vcAddFlag    \n");
                strSql.Append("     	,'0' as vcSCSNameModFlag,'0' as vcSCSPlaceModFlag    \n");
                strSql.Append("     from      \n");
                strSql.Append("     (    \n");
                strSql.Append("     		select a.* from     \n");
                strSql.Append("     		(    \n");
                strSql.Append("     			select iAutoId,dSSDate,case when vcJD='1' then '1' when vcJD='2' then '2' when vcJD = '3' then '3' when vcJD='4'then '2' end as 'vcJD'    \n");
                strSql.Append("     				  ,vcPart_id,vcSPINo,vcChange,vcCarType,vcInOutflag,vcPartName,vcOE    \n");
                strSql.Append("     			      ,vcSupplier_id,vcFXDiff,vcFXNo,vcSumLater,vcNum1,vcNum2,vcNum3,vcNum4,vcNum5,vcNum6    \n");
                strSql.Append("     				  ,vcNum7,vcNum8,vcNum9,vcNum10,vcNum11,vcNum12,vcNum13,vcNum14,vcNum15,vcIsDYJG,vcIsDYFX,vcYQorNG,vcNotDY,vcTH,vcSCPlace_City    \n");
                strSql.Append("     				  ,vcSCPlace_Province,vcCHPlace_City,vcCHPlace_Province,vcSYTCode,vcSCSName,vcSCSPlace,dSupplier_BJ,dSupplier_HK,dTFTM_BJ,vcZXBZDiff    \n");
                strSql.Append("     				  ,vcZXBZNo,vcReceiver,dNqDate,vcOperatorId,dOperatorTime,dHFDate,GUID from TSQJD    \n");
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
                strSql.Append("     				  ,vcNum7,vcNum8,vcNum9,vcNum10,vcNum11,vcNum12,vcNum13,vcNum14,vcNum15,vcIsDYJG,vcIsDYFX,vcYQorNG,vcNotDY,vcTH,vcSCPlace_City    \n");
                strSql.Append("     				  ,vcSCPlace_Province,vcCHPlace_City,vcCHPlace_Province,vcSYTCode,vcSCSName,vcSCSPlace,dSupplier_BJ,dSupplier_HK,dTFTM_BJ,vcZXBZDiff    \n");
                strSql.Append("     				  ,vcZXBZNo,vcReceiver,dNqDate,vcOperatorId,dOperatorTime,dHFDate,GUID     \n");
                strSql.Append("     			from TSQJD_Supplier    \n");
                strSql.Append("     		)    \n");
                strSql.Append("     )a    \n");
                strSql.Append("     left join     \n");
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
                strSql.Append("     	select vcValue,vcName from TCode where vcCodeId = 'C016'    \n");
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
                strSql.Append("     left join    \n");
                strSql.Append("     (    \n");
                strSql.Append("     	select COUNT(*) as iNum,[GUID] from TSQJD_THlist    \n");
                strSql.Append("     	group by GUID    \n");
                strSql.Append("     )b10 on a.GUID = b10.[GUID]    \n");
                strSql.Append("     where 1=1    \n");
                strSql.Append("     and vcSupplier_id = '" + strUserID + "'    \n");
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
                strSql.Append("         \n");
                strSql.Append("     order by vcPart_id,iAutoId asc    \n");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString(), "TK");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 检索现地库中供应商可否编辑信息
        public DataTable SearchSupplierEditDT(List<string> supplierLists)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("      select vcValue1,vcValue2 from TOutCode        \n");
                strSql.Append("      where vcCodeId = 'C008' and vcIsColum = 0      \n");
                strSql.Append("      and (       \n");
                for (int i = 0; i < supplierLists.Count; i++)
                {
                    strSql.Append("      vcValue1='" + supplierLists[i] + "'      \n");
                }
                strSql.Append("           )       \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 初始化检索
        public DataTable Search(string strUserID)
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
                strSql.Append("     	,b10.iNum    \n");
                strSql.Append("     	,'0' as vcModFlag,'0' as vcAddFlag    \n");
                strSql.Append("     	,'0' as vcSCSNameModFlag,'0' as vcSCSPlaceModFlag    \n");
                strSql.Append("     from      \n");
                strSql.Append("     (    \n");
                strSql.Append("     		select a.* from     \n");
                strSql.Append("     		(    \n");
                strSql.Append("     			select iAutoId,dSSDate,case when vcJD='1' then '1' when vcJD='2' then '2' when vcJD = '3' then '3' when vcJD='4'then '2' end as 'vcJD'    \n");
                strSql.Append("     				  ,vcPart_id,vcSPINo,vcChange,vcCarType,vcInOutflag,vcPartName,vcOE    \n");
                strSql.Append("     			      ,vcSupplier_id,vcFXDiff,vcFXNo,vcSumLater,vcNum1,vcNum2,vcNum3,vcNum4,vcNum5,vcNum6    \n");
                strSql.Append("     				  ,vcNum7,vcNum8,vcNum9,vcNum10,vcNum11,vcNum12,vcNum13,vcNum14,vcNum15,vcIsDYJG,vcIsDYFX,vcYQorNG,vcNotDY,vcTH,vcSCPlace_City    \n");
                strSql.Append("     				  ,vcSCPlace_Province,vcCHPlace_City,vcCHPlace_Province,vcSYTCode,vcSCSName,vcSCSPlace,dSupplier_BJ,dSupplier_HK,dTFTM_BJ,vcZXBZDiff    \n");
                strSql.Append("     				  ,vcZXBZNo,vcReceiver,dNqDate,vcOperatorId,dOperatorTime,dHFDate,GUID from     \n");
                strSql.Append("     				  (    \n");
                strSql.Append("     						select * from TSQJD    \n");
                strSql.Append("     						where 1=1    \n");
                strSql.Append("     						and vcSupplier_id = '" + strUserID + "'    \n");
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
                strSql.Append("     				  ,vcNum7,vcNum8,vcNum9,vcNum10,vcNum11,vcNum12,vcNum13,vcNum14,vcNum15,vcIsDYJG,vcIsDYFX,vcYQorNG,vcNotDY,vcTH,vcSCPlace_City    \n");
                strSql.Append("     				  ,vcSCPlace_Province,vcCHPlace_City,vcCHPlace_Province,vcSYTCode,vcSCSName,vcSCSPlace,dSupplier_BJ,dSupplier_HK,dTFTM_BJ,vcZXBZDiff    \n");
                strSql.Append("     				  ,vcZXBZNo,vcReceiver,dNqDate,vcOperatorId,dOperatorTime,dHFDate,GUID     \n");
                strSql.Append("     			from     \n");
                strSql.Append("     			(    \n");
                strSql.Append("     				select * from TSQJD_Supplier    \n");
                strSql.Append("     				where 1=1    \n");
                strSql.Append("     			    and vcSupplier_id = '" + strUserID + "'    \n");
                strSql.Append("                     and vcJD in ('1','3')  ");
                strSql.Append("     			)a    \n");
                strSql.Append("     		)    \n");
                strSql.Append("     )a    \n");
                strSql.Append("     left join     \n");
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
                strSql.Append("     	select vcValue,vcName from TCode where vcCodeId = 'C016'    \n");
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
                strSql.Append("     left join    \n");
                strSql.Append("     (    \n");
                strSql.Append("     	select COUNT(*) as iNum,GUID from TSQJD_THlist    \n");
                strSql.Append("     	group by GUID    \n");
                strSql.Append("     )b10 on a.GUID = b10.GUID    \n");
                strSql.Append("     order by vcPart_id,iAutoId asc    \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString(), "TK");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 检索退回履历
        public DataTable SearchTHList(string strGUID)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("     select * from TSQJD_THList where GUID = '" + strGUID + "'   \n");
                strSql.Append("     order by dTHTime desc   \n");
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
                    sqlStr.Append("        insert into #TSQJD_temp         \n");
                    sqlStr.Append("        (         \n");
                    sqlStr.Append("        	 dSSDate,vcJD,vcPart_id,vcSPINo,vcChange         \n");
                    sqlStr.Append("        	,vcCarType,vcInOutflag,vcPartName,vcOE,vcSupplier_id         \n");
                    sqlStr.Append("        	,vcFXDiff,vcFXNo,vcSumLater,vcNum1,vcNum2         \n");
                    sqlStr.Append("        	,vcNum3,vcNum4,vcNum5,vcNum6,vcNum7         \n");
                    sqlStr.Append("        	,vcNum8,vcNum9,vcNum10,vcIsDYJG,vcIsDYFX         \n");
                    sqlStr.Append("        	,vcYQorNG,vcNotDY,vcTH,vcSCPlace_City,vcSCPlace_Province         \n");
                    sqlStr.Append("        	,vcCHPlace_City,vcCHPlace_Province,vcSYTCode,vcSCSName,vcSCSPlace         \n");
                    sqlStr.Append("        	,dSupplier_BJ,dSupplier_HK,dTFTM_BJ,vcZXBZDiff,vcZXBZNo         \n");
                    sqlStr.Append("        	,vcReceiver,dNqDate,vcOperatorId,dOperatorTime,dHFDate         \n");
                    sqlStr.Append("        	,GUID         \n");
                    sqlStr.Append("        )         \n");
                    sqlStr.Append("        values         \n");
                    sqlStr.Append("        (         \n");
                    sqlStr.Append("         " + ComFunction.getSqlValue(listInfoData[i]["dSSDate"], true) + "         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcJD"], false) + "         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcPart_id"], false) + "         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcSPINo"], false) + "         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcChange"], false) + "         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcCarType"], false) + "         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcInOutflag"], false) + "         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcPartName"], false) + "         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcOE"], false) + "         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcSupplier_id"], false) + "         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcFXDiff"], false) + "         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcFXNo"], false) + "         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcSumLater"], false) + "         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNum1"], true) + "         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNum2"], true) + "         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNum3"], true) + "         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNum4"], true) + "         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNum5"], true) + "         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNum6"], true) + "         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNum7"], true) + "         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNum8"], true) + "         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNum9"], true) + "         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNum10"], true) + "         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcIsDYJG"], false) + "         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcIsDYFX"], false) + "         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcYQorNG"], false) + "         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNotDY"], false) + "         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcTH"], false) + "         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcSCPlace_City"], false) + "         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcSCPlace_Province"], false) + "         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcCHPlace_City"], false) + "         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcCHPlace_Province"], false) + "         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcSYTCode"], false) + "         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcSCSName"], false) + "         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcSCSPlace"], false) + "         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["dSupplier_BJ"], true) + "         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["dSupplier_HK"], true) + "         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["dTFTM_BJ"], true) + "         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcZXBZDiff"], false) + "         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcZXBZNo"], false) + "         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcReceiver"], false) + "         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["dNqDate"], true) + "         \n");
                    sqlStr.Append("        ," + strUserId + "         \n");
                    sqlStr.Append("        ,GETDATE()         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["dHFDate"], true) + "         \n");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["GUID"], false) + "         \n");
                    sqlStr.Append("        )         \n");
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
                sqlStr.Append("         ,vcIsDYJG = b.vcIsDYJG       \r\n");
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

                if (sqlStr.Length > 0)
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
            try
            {
                StringBuilder sqlStr = new StringBuilder();

                #region 创建生确进度临时表
                sqlStr.Append("        if object_id('tempdb..#TSQJD_Supplier_temp') is not null        \r\n");
                sqlStr.Append("        begin         \r\n");
                sqlStr.Append("        drop table #TSQJD_Supplier_temp        \r\n");
                sqlStr.Append("        end        \r\n");
                sqlStr.Append("        select * into #TSQJD_Supplier_temp from TSQJD_Supplier        \r\n");
                sqlStr.Append("        where 1=0 ;       \r\n");
                #endregion

                #region 将所选的数据都插入临时表
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    sqlStr.Append("        insert into #TSQJD_Supplier_temp         ");
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
                    sqlStr.Append("         " + ComFunction.getSqlValue(listInfoData[i]["dSSDate"], true) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcJD"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcPart_id"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcSPINo"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcChange"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcCarType"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcInOutflag"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcPartName"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcOE"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcSupplier_id"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcFXDiff"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcFXNo"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcSumLater"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNum1"], true) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNum2"], true) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNum3"], true) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNum4"], true) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNum5"], true) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNum6"], true) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNum7"], true) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNum8"], true) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNum9"], true) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNum10"], true) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcIsDYJG"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcIsDYFX"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcYQorNG"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNotDY"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcTH"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcSCPlace_City"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcSCPlace_Province"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcCHPlace_City"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcCHPlace_Province"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcSYTCode"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcSCSName"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcSCSPlace"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["dSupplier_BJ"], true) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["dSupplier_HK"], true) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["dTFTM_BJ"], true) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcZXBZDiff"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcZXBZNo"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcReceiver"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["dNqDate"], true) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcOperatorId"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["dOperatorTime"], true) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["dHFDate"], true) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["GUID"], false) + "         ");
                    sqlStr.Append("        )         ");
                }
                #endregion

                #region 根据GUID更新供应商生确表，将生确进度更新为已回复
                sqlStr.Append("         update TSQJD_Supplier set        \r\n");
                sqlStr.Append("          vcJD = '2'       \r\n");
                sqlStr.Append("         from TSQJD_Supplier a       \r\n");
                sqlStr.Append("         inner join        \r\n");
                sqlStr.Append("         (       \r\n");
                sqlStr.Append("         	select GUID from #TSQJD_Supplier_temp       \r\n");
                sqlStr.Append("         ) b on a.GUID = b.GUID       \r\n");
                #endregion

                #region 根据GUID更新TFTM生确表，将生确进度更新为已回复
                sqlStr.Append("         update TSQJD set        \r\n");
                sqlStr.Append("          dSSDate = b.dSSDate       \r\n");
                sqlStr.Append("         ,vcJD = '2'       \r\n");
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
                sqlStr.Append("         ,vcIsDYJG = b.vcIsDYJG       \r\n");
                sqlStr.Append("         ,vcIsDYFX = b.vcIsDYFX       \r\n");
                sqlStr.Append("         ,vcYQorNG = b.vcYQorNG       \r\n");
                sqlStr.Append("         ,vcNotDY = b.vcNotDY       \r\n");
                sqlStr.Append("         ,vcTH = b.vcTH       \r\n");
                sqlStr.Append("         ,vcSCPlace_City = b.vcSCPlace_City       \r\n");
                sqlStr.Append("         ,vcSCPlace_Province = b.vcSCPlace_Province       \r\n");
                sqlStr.Append("         ,vcCHPlace_City = b.vcCHPlace_City       \r\n");
                sqlStr.Append("         ,vcCHPlace_Province = b.vcCHPlace_Province       \r\n");
                sqlStr.Append("         ,vcSCSName = b.vcSCSName       \r\n");
                sqlStr.Append("         ,vcSCSPlace = b.vcSCSPlace       \r\n");
                sqlStr.Append("         ,dSupplier_BJ = b.dSupplier_BJ       \r\n");
                sqlStr.Append("         ,dSupplier_HK = b.dSupplier_HK       \r\n");
                sqlStr.Append("         ,dTFTM_BJ = b.dTFTM_BJ       \r\n");
                sqlStr.Append("         ,vcZXBZDiff = b.vcZXBZDiff       \r\n");
                sqlStr.Append("         ,vcZXBZNo = b.vcZXBZNo       \r\n");
                sqlStr.Append("         ,vcReceiver = b.vcReceiver       \r\n");
                sqlStr.Append("         ,dHFDate = case when a.dHFDate is null then GETDATE()	else a.dHFDate end       \r\n");
                sqlStr.Append("         from TSQJD a       \r\n");
                sqlStr.Append("         inner join        \r\n");
                sqlStr.Append("         (       \r\n");
                sqlStr.Append("         	select * from #TSQJD_Supplier_temp       \r\n");
                sqlStr.Append("         ) b on a.GUID = b.GUID       \r\n");
                #endregion

                #region 将不可对应的/延期的数据插入履历表
                sqlStr.Append("      insert into TSQJD_THlist([GUID],vcPart_id,vcNGText,vcYQText,dTHTime)       \n");
                sqlStr.Append("      select [GUID],vcPart_id,vcNotDY,vcYQorNG,GETDATE() from #TSQJD_Supplier_temp where vcIsDYJG = 2 or (vcYQorNG is not null or vcYQorNG <> '')       \n");
                #endregion

                excute.ExcuteSqlWithStringOper(sqlStr.ToString(), "TK");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 延期说明
        public void SendYQ(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            try
            {
                StringBuilder sqlStr = new StringBuilder();

                #region 创建生确进度临时表
                sqlStr.Append("        if object_id('tempdb..#TSQJD_Supplier_temp') is not null        \r\n");
                sqlStr.Append("        begin         \r\n");
                sqlStr.Append("        drop table #TSQJD_Supplier_temp        \r\n");
                sqlStr.Append("        end        \r\n");
                sqlStr.Append("        select * into #TSQJD_Supplier_temp from TSQJD_Supplier        \r\n");
                sqlStr.Append("        where 1=0 ;       \r\n");
                #endregion

                #region 将所选的数据都插入临时表
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    sqlStr.Append("        insert into #TSQJD_Supplier_temp         ");
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
                    sqlStr.Append("         " + ComFunction.getSqlValue(listInfoData[i]["dSSDate"], true) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcJD"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcPart_id"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcSPINo"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcChange"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcCarType"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcInOutflag"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcPartName"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcOE"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcSupplier_id"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcFXDiff"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcFXNo"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcSumLater"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNum1"], true) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNum2"], true) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNum3"], true) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNum4"], true) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNum5"], true) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNum6"], true) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNum7"], true) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNum8"], true) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNum9"], true) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNum10"], true) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcIsDYJG"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcIsDYFX"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcYQorNG"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNotDY"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcTH"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcSCPlace_City"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcSCPlace_Province"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcCHPlace_City"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcCHPlace_Province"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcSYTCode"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcSCSName"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcSCSPlace"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["dSupplier_BJ"], true) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["dSupplier_HK"], true) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["dTFTM_BJ"], true) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcZXBZDiff"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcZXBZNo"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcReceiver"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["dNqDate"], true) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcOperatorId"], false) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["dOperatorTime"], true) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["dHFDate"], true) + "         ");
                    sqlStr.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["GUID"], false) + "         ");
                    sqlStr.Append("        )         ");
                }
                #endregion

                #region 根据GUID更新TFTM生确表，将生确进度更新为已回复
                sqlStr.Append("         update TSQJD set        \r\n");
                sqlStr.Append("          vcYQorNG = b.vcYQorNG       \r\n");
                sqlStr.Append("         from TSQJD a       \r\n");
                sqlStr.Append("         inner join        \r\n");
                sqlStr.Append("         (       \r\n");
                sqlStr.Append("         	select * from #TSQJD_Supplier_temp       \r\n");
                sqlStr.Append("         ) b on a.GUID = b.GUID       \r\n");
                #endregion

                excute.ExcuteSqlWithStringOper(sqlStr.ToString(), "TK");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 一括付与
        public void SetFY(List<Dictionary<string, Object>> listInfoData, string strSupplier_BJ, string strSupplier_HK, string strSCPlace_City, string strSCPlace_Province, string strCHPlace_City, string strCHPlace_Province, string strUserId, ref string strErr)
        {
            try
            {
                StringBuilder sql = new StringBuilder();

                ///用于更新【新设类】
                StringBuilder sbrSet1 = new StringBuilder();

                //用于更新【非新设类】
                StringBuilder sbrSet2 = new StringBuilder();

                #region 获取所有已选择的数据，放入临时表
                getTempData(listInfoData, sql, strUserId, ref strErr);
                #endregion

                #region 判断所选数据再供应商生确表中是否存在，如果不存在，将数据插入到供应商表中
                sql.Append("        insert into TSQJD_Supplier       \r\n");
                sql.Append("        (       \r\n");
                sql.Append("        	 dSSDate,vcJD,vcPart_id,vcSPINo,vcChange       \r\n");
                sql.Append("        	,vcCarType,vcInOutflag,vcPartName,vcOE,vcSupplier_id       \r\n");
                sql.Append("        	,vcFXDiff,vcFXNo,vcSumLater,vcNum1,vcNum2       \r\n");
                sql.Append("        	,vcNum3,vcNum4,vcNum5,vcNum6,vcNum7       \r\n");
                sql.Append("        	,vcNum8,vcNum9,vcNum10,vcIsDYJG,vcIsDYFX       \r\n");
                sql.Append("        	,vcYQorNG,vcNotDY,vcTH,vcSCPlace_City,vcSCPlace_Province       \r\n");
                sql.Append("        	,vcCHPlace_City,vcCHPlace_Province,vcSYTCode,vcSCSName,vcSCSPlace       \r\n");
                sql.Append("        	,dSupplier_BJ,dSupplier_HK,dTFTM_BJ,vcZXBZDiff,vcZXBZNo       \r\n");
                sql.Append("        	,vcReceiver,dNqDate,vcOperatorId,dOperatorTime,dHFDate       \r\n");
                sql.Append("        	,GUID       \r\n");
                sql.Append("        )       \r\n");
                sql.Append("        select        \r\n");
                sql.Append("        	 dSSDate,vcJD,vcPart_id,vcSPINo,vcChange       \r\n");
                sql.Append("        	,vcCarType,vcInOutflag,vcPartName,vcOE,vcSupplier_id       \r\n");
                sql.Append("        	,vcFXDiff,vcFXNo,vcSumLater,vcNum1,vcNum2       \r\n");
                sql.Append("        	,vcNum3,vcNum4,vcNum5,vcNum6,vcNum7       \r\n");
                sql.Append("        	,vcNum8,vcNum9,vcNum10,vcIsDYJG,vcIsDYFX       \r\n");
                sql.Append("        	,vcYQorNG,vcNotDY,vcTH,vcSCPlace_City,vcSCPlace_Province       \r\n");
                sql.Append("        	,vcCHPlace_City,vcCHPlace_Province,vcSYTCode,vcSCSName,vcSCSPlace       \r\n");
                sql.Append("        	,dSupplier_BJ,dSupplier_HK,dTFTM_BJ,vcZXBZDiff,vcZXBZNo       \r\n");
                sql.Append("        	,vcReceiver,dNqDate,vcOperatorId,dOperatorTime,dHFDate       \r\n");
                sql.Append("        	,a.GUID       \r\n");
                sql.Append("         from #TSQJD_temp a       \r\n");
                sql.Append("         left join        \r\n");
                sql.Append("         (       \r\n");
                sql.Append("        	select GUID from TSQJD_Supplier       \r\n");
                sql.Append("        	where vcJD <> '2'       \r\n");
                sql.Append("         ) b       \r\n");
                sql.Append("         on a.GUID = b.GUID       \r\n");
                sql.Append("         where b.GUID is null       \r\n");
                #endregion

                #region 拼接更新语句
                if (!string.IsNullOrEmpty(strSupplier_BJ))
                {
                    sbrSet1.Append(",dSupplier_BJ = '" + strSupplier_BJ + "'");
                    sbrSet2.Append(",dSupplier_BJ = '" + strSupplier_BJ + "'");
                }
                if (!string.IsNullOrEmpty(strSupplier_HK))
                {
                    sbrSet1.Append(",dSupplier_HK ='" + strSupplier_HK + "'");
                    sbrSet2.Append(",dSupplier_HK ='" + strSupplier_HK + "'");
                }
                if (!string.IsNullOrEmpty(strSCPlace_City))
                {
                    sbrSet1.Append(",vcSCPlace_City ='" + strSCPlace_City + "'");
                }
                if (!string.IsNullOrEmpty(strSCPlace_Province))
                {
                    sbrSet1.Append(",vcSCPlace_Province ='" + strSCPlace_Province + "'");
                }
                if (!string.IsNullOrEmpty(strCHPlace_City))
                {
                    sbrSet1.Append(",vcCHPlace_City = '" + strCHPlace_City + "'");
                }
                if (!string.IsNullOrEmpty(strCHPlace_Province))
                {
                    sbrSet1.Append(",vcCHPlace_Province ='" + strCHPlace_Province + "'");
                }
                #endregion

                #region 【新设类】一括付与
                if (sbrSet1.Length > 0)
                {
                    sql.AppendLine("      update TSQJD_Supplier set       \n");
                    sql.Append(sbrSet1.ToString().Substring(1));
                    sql.AppendLine("        FROM TSQJD_Supplier a     \n");
                    sql.AppendLine("        inner join      \n");
                    sql.AppendLine("        (     \n");
                    sql.AppendLine("        	select [GUID] from #TSQJD_temp     \n");
                    sql.AppendLine("        	where vcJD <>'2'     \n");
                    sql.AppendLine("        	and ( vcChange='1' or vcChange='2' or vcChange='8' or vcChange='10' or vcChange='12')      \n");
                    sql.AppendLine("        )b on a.[GUID] = b.[GUID]     \n");
                }
                #endregion

                #region 【非新设】一括付与
                if (sbrSet2.Length > 0)
                {
                    sql.AppendLine("      update TSQJD_Supplier set       \n");
                    sql.Append(sbrSet2.ToString().Substring(1));
                    sql.AppendLine("        FROM TSQJD_Supplier a     \n");
                    sql.AppendLine("        inner join      \n");
                    sql.AppendLine("        (     \n");
                    sql.AppendLine("        	select [GUID] from #TSQJD_temp     \n");
                    sql.AppendLine("        	where vcJD <>'2'     \n");
                    sql.AppendLine("        	and ( vcChange<>'1' or vcChange<>'2' or vcChange<>'8' or vcChange<>'10' or vcChange<>'12')      \n");
                    sql.AppendLine("        )b on a.[GUID] = b.[GUID]     \n");
                }
                #endregion

                excute.ExcuteSqlWithStringOper(sql.ToString(), "TK");
            }
            catch (Exception ex)
            {
                strErr = ex.Message;
            }
        }
        #endregion

        #region 获取临时表，并将所选数据插入临时表  临时表名称 #TSQJD_temp
        /// <summary>
        /// 获取临时表，并将所选数据插入临时表    临时表名称 #TSQJD_temp
        /// </summary>
        /// <param name="listInfoData">要处理的数据集</param>
        /// <param name="sql">sql语句</param>
        /// <param name="strUserId">当前登陆用户</param>
        /// <param name="strErr">记录的错误信息，</param>
        /// <returns></returns>
        public void getTempData(List<Dictionary<string, Object>> listInfoData, StringBuilder sql, string strUserId, ref string strErr)
        {
            try
            {
                if (listInfoData.Count <= 0)
                {
                    strErr = "无待处理数据";
                }

                #region 创建临时表
                sql.Append("        if object_id('tempdb..#TSQJD_temp') is not null  \n");
                sql.Append("        Begin  \n");
                sql.Append("        drop  table #TSQJD_temp  \n");
                sql.Append("        End  \n");
                sql.Append("        select * into #TSQJD_temp from       \n");
                sql.Append("        (      \n");
                sql.Append("      	  select * from TSQJD where 1=0      \n");
                sql.Append("        ) a      ;\n");

                #endregion

                #region 将要织入的数据存入临时表
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    sql.Append("        insert into #TSQJD_temp        \n");
                    sql.Append("        (        \n");
                    sql.Append("        dSSDate,vcJD        \n");
                    sql.Append("        ,vcPart_id,vcSPINo,vcChange,vcCarType,vcInOutflag        \n");
                    sql.Append("        ,vcPartName,vcOE,vcSupplier_id,vcFXDiff,vcFXNo        \n");
                    sql.Append("        ,vcSumLater,vcNum1,vcNum2,vcNum3,vcNum4        \n");
                    sql.Append("        ,vcNum5,vcNum6,vcNum7,vcNum8,vcNum9        \n");
                    sql.Append("        ,vcNum10,vcIsDYJG,vcIsDYFX,vcYQorNG,vcTH        \n");
                    sql.Append("        ,vcSCPlace_City,vcSCPlace_Province,vcCHPlace_City,vcCHPlace_Province,vcSYTCode        \n");
                    sql.Append("        ,vcSCSName,vcSCSPlace,dSupplier_BJ,dSupplier_HK,dTFTM_BJ,vcZXBZDiff        \n");
                    sql.Append("        ,vcZXBZNo,vcReceiver,dNqDate,vcOperatorId,dOperatorTime,GUID        \n");
                    sql.Append("        )        \n");
                    sql.Append("        values        \n");
                    sql.Append("        (        \n");
                    sql.Append("        " + ComFunction.getSqlValue(listInfoData[i]["dSSDate"], true) + "        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcJD"], false) + "        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcPart_id"], false) + "        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcSPINo"], false) + "        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcChange"], false) + "        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcCarType"], false) + "        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcInOutflag"], false) + "        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcPartName"], false) + "        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcOE"], false) + "        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcSupplier_id"], false) + "        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcFXDiff"], false) + "        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcFXNo"], false) + "        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcSumLater"], false) + "        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNum1"], true) + "        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNum2"], true) + "        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNum3"], true) + "        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNum4"], true) + "        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNum5"], true) + "        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNum6"], true) + "        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNum7"], true) + "        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNum8"], true) + "        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNum9"], true) + "        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcNum10"], true) + "        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcIsDYJG"], false) + "        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcIsDYFX"], false) + "        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcYQorNG"], false) + "        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcTH"], false) + "        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcSCPlace_City"], false) + "        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcSCPlace_Province"], false) + "        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcCHPlace_City"], false) + "        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcCHPlace_Province"], false) + "        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcSYTCode"], false) + "        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcSCSName"], false) + "        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcSCSPlace"], false) + "        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["dSupplier_BJ"], true) + "        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["dSupplier_HK"], true) + "        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["dTFTM_BJ"], true) + "        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcZXBZDiff"], false) + "        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcZXBZNo"], false) + "        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcReceiver"], false) + "        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["dNqDate"], true) + "        \n");
                    sql.Append("        ,'" + strUserId + "'        \n");
                    sql.Append("        ,GETDATE()        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["GUID"], false) + "        \n");
                    sql.Append("        )        \n");
                }
                #endregion
            }
            catch (Exception ex)
            {
                strErr = ex.Message.ToString();
            }
        }
        #endregion

        #region 获取执行标准下拉框
        public DataTable getZXBZDT()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("       select vcValue1 as 'vcName',vcValue1 as 'vcValue' from TOutCode where vcCodeId = 'C017' and vcIsColum = '0'      ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


    }
}
