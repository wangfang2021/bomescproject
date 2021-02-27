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
                strSql.Append("     				  ,vcZXBZNo,vcReceiver,dNqDate,vcOperatorId,dOperatorTime,dHFDate from     \n");
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
                strSql.Append("     			select vcPart_id,vcSupplier_id,vcSYTCode,vcReceiver from TSQJD_Supplier    \n");
                strSql.Append("     		) b on a.vcPart_id = b.vcPart_id and a.vcSupplier_id = b.vcSupplier_id and a.vcSYTCode = b.vcSYTCode and a.vcReceiver = b.vcReceiver    \n");
                strSql.Append("     		where b.vcPart_id is null    \n");
                strSql.Append("     		union all    \n");
                strSql.Append("     		(    \n");
                strSql.Append("     			select iAutoId,dSSDate,case when vcJD='1' then '1' when vcJD='2' then '2' when vcJD = '3' then '3' when vcJD='4'then '2' end as 'vcJD'    \n");
                strSql.Append("     			      ,vcPart_id,vcSPINo,vcChange,vcCarType,vcInOutflag,vcPartName,vcOE    \n");
                strSql.Append("     				  ,vcSupplier_id,vcFXDiff,vcFXNo,vcSumLater,vcNum1,vcNum2,vcNum3,vcNum4,vcNum5,vcNum6    \n");
                strSql.Append("     				  ,vcNum7,vcNum8,vcNum9,vcNum10,vcIsDYJG,vcIsDYFX,vcYQorNG,vcNotDY,vcTH,vcSCPlace_City    \n");
                strSql.Append("     				  ,vcSCPlace_Province,vcCHPlace_City,vcCHPlace_Province,vcSYTCode,vcSCSName,vcSCSPlace,dSupplier_BJ,dSupplier_HK,dTFTM_BJ,vcZXBZDiff    \n");
                strSql.Append("     				  ,vcZXBZNo,vcReceiver,dNqDate,vcOperatorId,dOperatorTime,dHFDate     \n");
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
                strSql.Append("     				  ,vcZXBZNo,vcReceiver,dNqDate,vcOperatorId,dOperatorTime,dHFDate from     \n");
                strSql.Append("     				  (    \n");
                strSql.Append("     						select * from TSQJD    \n");
                strSql.Append("     						where 1=1    \n");
                strSql.Append("                             and vcJD in ('1','3')  ");
                
                strSql.Append("     				  )a    \n");
                strSql.Append("     		) a    \n");
                strSql.Append("     		left join     \n");
                strSql.Append("     		(    \n");
                strSql.Append("     			select vcPart_id,vcSupplier_id,vcSYTCode,vcReceiver from TSQJD_Supplier    \n");
                strSql.Append("     		) b on a.vcPart_id = b.vcPart_id and a.vcSupplier_id = b.vcSupplier_id and a.vcSYTCode = b.vcSYTCode and a.vcReceiver = b.vcReceiver    \n");
                strSql.Append("     		where b.vcPart_id is null    \n");
                strSql.Append("     		union all    \n");
                strSql.Append("     		(    \n");
                strSql.Append("     			select iAutoId,dSSDate,case when vcJD='1' then '1' when vcJD='2' then '2' when vcJD = '3' then '3' when vcJD='4'then '2' end as 'vcJD'    \n");
                strSql.Append("     			      ,vcPart_id,vcSPINo,vcChange,vcCarType,vcInOutflag,vcPartName,vcOE    \n");
                strSql.Append("     				  ,vcSupplier_id,vcFXDiff,vcFXNo,vcSumLater,vcNum1,vcNum2,vcNum3,vcNum4,vcNum5,vcNum6    \n");
                strSql.Append("     				  ,vcNum7,vcNum8,vcNum9,vcNum10,vcIsDYJG,vcIsDYFX,vcYQorNG,vcNotDY,vcTH,vcSCPlace_City    \n");
                strSql.Append("     				  ,vcSCPlace_Province,vcCHPlace_City,vcCHPlace_Province,vcSYTCode,vcSCSName,vcSCSPlace,dSupplier_BJ,dSupplier_HK,dTFTM_BJ,vcZXBZDiff    \n");
                strSql.Append("     				  ,vcZXBZNo,vcReceiver,dNqDate,vcOperatorId,dOperatorTime,dHFDate     \n");
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

                for (int i = 0; i < listInfoData.Count; i++)
                {
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                    sqlStr.AppendLine("        update TSQJD set         ");
                    sqlStr.AppendLine("         vcIsDYJG = "            + ComFunction.getSqlValue(listInfoData[i]["vcIsDYJG"],true) + "         ");
                    sqlStr.AppendLine("        ,vcIsDYFX = "           + ComFunction.getSqlValue(listInfoData[i]["vcIsDYFX"],true) + "         ");
                    sqlStr.AppendLine("        ,vcSCPlace_Province = " + ComFunction.getSqlValue(listInfoData[i]["vcSCPlace_Province"],false) + "         ");
                    sqlStr.AppendLine("        ,vcSCPlace_City = "     + ComFunction.getSqlValue(listInfoData[i]["vcSCPlace_City"],false) + "         ");
                    sqlStr.AppendLine("        ,vcCHPlace_Province = " + ComFunction.getSqlValue(listInfoData[i]["vcCHPlace_Province"],false) + "         ");
                    sqlStr.AppendLine("        ,vcCHPlace_City     = " + ComFunction.getSqlValue(listInfoData[i]["vcCHPlace_City"],false) + "         ");
                    sqlStr.AppendLine("        ,vcYQorNG = "           + ComFunction.getSqlValue(listInfoData[i]["vcYQorNG"],false) + "         ");
                    sqlStr.AppendLine("        ,vcNotDY = "            + ComFunction.getSqlValue(listInfoData[i]["vcNotDY"], false) + "         ");
                    sqlStr.AppendLine("        ,dSupplier_BJ = "       + ComFunction.getSqlValue(listInfoData[i]["dSupplier_BJ"], true) + "         ");
                    sqlStr.AppendLine("        ,dSupplier_HK = "       + ComFunction.getSqlValue(listInfoData[i]["dSupplier_HK"], true) + "         ");
                    sqlStr.AppendLine("        ,vcZXBZDiff = "         + ComFunction.getSqlValue(listInfoData[i]["vcZXBZDiff"], false) + "         ");
                    sqlStr.AppendLine("        ,vcZXBZNo = "           + ComFunction.getSqlValue(listInfoData[i]["vcZXBZNo"], false) + "         ");
                    sqlStr.AppendLine("        ,vcSCSName = "          + ComFunction.getSqlValue(listInfoData[i]["vcSCSName"], false) + "         ");
                    sqlStr.AppendLine("        ,vcSCSPlace = "         + ComFunction.getSqlValue(listInfoData[i]["vcSCSPlace"], false) + "         ");
                    sqlStr.AppendLine("        ,vcOperatorId = '" + strUserId + "'         ");
                    sqlStr.AppendLine("        ,dOperatorTime = GETDATE()         ");
                    sqlStr.AppendLine("        where iAutoId = '" + iAutoId + "';         ");
                }
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
