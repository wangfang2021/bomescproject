﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Common;
using System.Data.SqlClient;
using System.Collections;
namespace DataAccess
{
    public class FS0304_DataAccess 
    {
        private MultiExcute excute = new MultiExcute();

        #region 按检索条件返回dt
        public DataTable Search(string strSSDate,string strJD, string strPart_id, string strInOutFlag, string strIsDYJG, string strCarType, string strSupplier_id)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("     select *    \n");
                strSql.Append("     ,b.vcName as 'vcJD_Name'    \n");
                strSql.Append("     ,b2.vcName as 'vcChange_Name'    \n");
                strSql.Append("     ,b3.vcName as 'vcInOutflag_Name'    \n");
                strSql.Append("     ,b4.vcName as 'vcOE_Name'    \n");
                strSql.Append("     ,b5.vcName as 'vcSYTCode_Name'    \n");
                strSql.Append("     ,b6.vcName as 'vcCarType_Name'    \n");
                strSql.Append("     ,b7.vcName as 'vcFXDiff_Name'    \n");
                strSql.Append("     ,b8.vcName as 'vcIsDYJG_Name'    \n");
                strSql.Append("     ,b9.vcName as 'vcIsDYFX_Name'    \n");
                strSql.Append("     ,'0' as vcModFlag,'0' as vcAddFlag    \n");
                strSql.Append("     from TSQJD a    \n");
                strSql.Append("     left join     \n");
                strSql.Append("     (    \n");
                strSql.Append("     	select vcValue,vcName from TCode where vcCodeId = 'C026'    \n");
                strSql.Append("     )b on a.vcJD = b.vcValue    \n");
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
                strSql.Append("     left join     ");
                strSql.Append("     (    ");
                strSql.Append("     	select vcValue,vcName from TCode where vcCodeId = 'C098'    ");
                strSql.Append("     )b6 on a.vcCarType = b6.vcValue    ");
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
                strSql.Append("   where 1=1  \n");
                if (!string.IsNullOrEmpty(strSSDate))
                {
                    strSql.Append("      and dSSDate = '" + strSSDate + "'  ");
                }
                if (!string.IsNullOrEmpty(strJD))
                {
                    if (strJD=="5")
                    {
                        strSql.Append("      and vcJD in ('1','2','3')   ");
                    }
                    else
                    {
                        strSql.Append("      and vcJD = '" + strJD + "'   ");
                    }
                }
                if (!string.IsNullOrEmpty(strPart_id))
                {
                    strSql.Append("      and vcPart_id like '" + strPart_id + "%'   ");
                }
                if (!string.IsNullOrEmpty(strInOutFlag))
                {
                    strSql.Append("      and vcInOutFlag = '" + strInOutFlag + "'   ");
                }
                if (!string.IsNullOrEmpty(strIsDYJG))
                {
                    strSql.Append("      and vcIsDYJG='" + strIsDYJG + "'   ");
                }
                if (!string.IsNullOrEmpty(strCarType))
                {
                    strSql.Append("      and vcCarType like '%" + strCarType + "%'   ");
                }
                if (!string.IsNullOrEmpty(strSupplier_id))
                {
                    strSql.Append("      and vcSupplier_id like '"+ strSupplier_id + "%'   ");
                }
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString(),"TK");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 检索处理中的数据
        /// <summary>
        /// 界面打开时执行的检索，检索处理中的数据。处理中：进度状态只要不是已织入的都是处理中
        /// </summary>
        /// <returns></returns>
        public DataTable Search()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("     select *    \n");
                strSql.Append("     ,b.vcName as 'vcJD_Name'    \n");
                strSql.Append("     ,b2.vcName as 'vcChange_Name'    \n");
                strSql.Append("     ,b3.vcName as 'vcInOutflag_Name'    \n");
                strSql.Append("     ,b4.vcName as 'vcOE_Name'    \n");
                strSql.Append("     ,b5.vcName as 'vcSYTCode_Name'    \n");
                strSql.Append("     ,b6.vcName as 'vcCarType_Name'    \n");
                strSql.Append("     ,b7.vcName as 'vcFXDiff_Name'    \n");
                strSql.Append("     ,b8.vcName as 'vcIsDYJG_Name'    \n");
                strSql.Append("     ,b9.vcName as 'vcIsDYFX_Name'    \n");
                strSql.Append("     ,'0' as vcModFlag,'0' as vcAddFlag    \n");
                strSql.Append("     from TSQJD a    \n");
                strSql.Append("     left join     \n");
                strSql.Append("     (    \n");
                strSql.Append("     	select vcValue,vcName from TCode where vcCodeId = 'C026'    \n");
                strSql.Append("     )b on a.vcJD = b.vcValue    \n");
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
                strSql.Append("     left join     ");
                strSql.Append("     (    ");
                strSql.Append("     	select vcValue,vcName from TCode where vcCodeId = 'C098'    ");
                strSql.Append("     )b6 on a.vcCarType = b6.vcValue    ");
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
                strSql.Append("   where 1=1  \n");
                strSql.Append("   and vcJD <> 4  \n");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString(), "TK");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        /*需要重写逻辑*/
        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    if (bModFlag == true)
                    {//修改
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                        sql.Append("      update TSQJD set       \n");
                        sql.Append("      dSSDate = "+ ComFunction.getSqlValue(listInfoData[i]["dSSDate"], true) + "      \n");
                        sql.Append("      ,vcJD = " + ComFunction.getSqlValue(listInfoData[i]["vcJD"], true) + "      \n");
                        sql.Append("      ,vcPart_id = " + ComFunction.getSqlValue(listInfoData[i]["vcPart_id"], false) + "      \n");
                        sql.Append("      ,vcSPINo = " + ComFunction.getSqlValue(listInfoData[i]["vcSPINo"], false) + "      \n");
                        sql.Append("      ,vcChange = " + ComFunction.getSqlValue(listInfoData[i]["vcChange"], false) + "      \n");
                        sql.Append("      ,vcCarType = " + ComFunction.getSqlValue(listInfoData[i]["vcCarType"], false) + "      \n");
                        sql.Append("      ,vcInOutflag = " + ComFunction.getSqlValue(listInfoData[i]["vcInOutflag"], false) + "      \n");
                        sql.Append("      ,vcPartName = " + ComFunction.getSqlValue(listInfoData[i]["vcPartName"], false) + "      \n");
                        sql.Append("      ,vcOE = " + ComFunction.getSqlValue(listInfoData[i]["vcOE"], false) + "      \n");
                        sql.Append("      ,vcSupplier_id = " + ComFunction.getSqlValue(listInfoData[i]["vcSupplier_id"], false) + "      \n");
                        sql.Append("      ,vcFXDiff = " + ComFunction.getSqlValue(listInfoData[i]["vcFXDiff"], false) + "      \n");
                        sql.Append("      ,vcSumLater = " + ComFunction.getSqlValue(listInfoData[i]["vcSumLater"], false) + "      \n");
                        sql.Append("      ,vcIsDYJG = " + ComFunction.getSqlValue(listInfoData[i]["vcIsDYJG"], false) + "      \n");
                        sql.Append("      ,vcIsDYFX = " + ComFunction.getSqlValue(listInfoData[i]["vcIsDYFX"], false) + "      \n");
                        sql.Append("      ,vcYQorNG = " + ComFunction.getSqlValue(listInfoData[i]["vcYQorNG"], false) + "      \n");
                        sql.Append("      ,vcSCPlace_City = " + ComFunction.getSqlValue(listInfoData[i]["vcSCPlace_City"], false) + "      \n");
                        sql.Append("      ,vcSCPlace_Province = " + ComFunction.getSqlValue(listInfoData[i]["vcSCPlace_Province"], false) + "      \n");
                        sql.Append("      ,vcCHPlace_City = " + ComFunction.getSqlValue(listInfoData[i]["vcCHPlace_City"], false) + "      \n");
                        sql.Append("      ,vcCHPlace_Province = " + ComFunction.getSqlValue(listInfoData[i]["vcCHPlace_Province"], false) + "      \n");
                        sql.Append("      ,vcSYTCode = " + ComFunction.getSqlValue(listInfoData[i]["vcSYTCode"], false) + "      \n");
                        sql.Append("      ,vcSCSName = " + ComFunction.getSqlValue(listInfoData[i]["vcSCSName"], false) + "      \n");
                        sql.Append("      ,vcSCSPlace = " + ComFunction.getSqlValue(listInfoData[i]["vcSCSPlace"], false) + "      \n");
                        sql.Append("      ,dSupplier_BJ = " + ComFunction.getSqlValue(listInfoData[i]["dSupplier_BJ"], true) + "      \n");
                        sql.Append("      ,dSupplier_HK = " + ComFunction.getSqlValue(listInfoData[i]["dSupplier_HK"], true) + "      \n");
                        sql.Append("      ,dTFTM_BJ = " + ComFunction.getSqlValue(listInfoData[i]["dTFTM_BJ"], true) + "      \n");
                        sql.Append("      ,vcZXBZDiff = " + ComFunction.getSqlValue(listInfoData[i]["vcZXBZDiff"], false) + "      \n");
                        sql.Append("      ,vcZXBZNo = " + ComFunction.getSqlValue(listInfoData[i]["vcZXBZNo"], false) + "      \n");
                        sql.Append("      ,vcOperatorId = '" + strUserId + "'      \n");
                        sql.Append("      ,dOperatorTime = GETDATE()      \n");
                        sql.Append("      where iAutoId = '" + iAutoId + "'      \n");
                        sql.Append("       and vcJD not in ('3')          ");
                    }
                }
                excute.ExcuteSqlWithStringOper(sql.ToString(), "TK");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        /*
         * 删除时，同时需要删除供应商生确表中的数据
         */
        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                StringBuilder sqlStr = new StringBuilder();

                /*后面还要删除退回履历表的数据*/
                #region 根据GUID删除TFTM生确表中的数据、供应商生确表中的数据
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    sqlStr.Append("        delete TSQJD where GUID = "+ComFunction.getSqlValue(listInfoData[i]["GUID"],false)+"         ");
                    sqlStr.Append("        delete TSQJD_Supplier where GUID = " + ComFunction.getSqlValue(listInfoData[i]["GUID"], false) + "         ");
                }
                #endregion

                excute.ExcuteSqlWithStringOper(sqlStr.ToString(), "TK");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        /*
         * 需要重写逻辑,更改TFTM生确进度表的进度状态，同时更改供应商生确表的进度状态
         */
        #region 退回
        public void Back(List<Dictionary<string, Object>> listInfoData, string strUserId,string strTH)
        {
            #region 将进度变为已退回
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);

                    sql.Append("      update TSQJD set vcJD =      \n");
                    sql.Append("      (      \n");
                    sql.Append("      select vcValue from TCode where vcCodeId = 'C026' and vcName = '已退回'      \n");
                    sql.Append("      )      \n");
                    sql.Append("      ,vcTH = '"+strTH+"'      \n");
                    sql.Append("      ,vcOperatorId = '" + strUserId + "'      \n");
                    sql.Append("      ,dOperatorTime = GETDATE()      \n");
                    sql.Append("      where iAutoId = '" + iAutoId + "'      \n");
                }
                excute.ExcuteSqlWithStringOper(sql.ToString(), "TK");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            #endregion

        }
        #endregion

        /*
         * 需要重写逻辑
         */
        #region 日期一括付与
        public void DateKFY(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId, string dTFTM_BJ)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);

                    sql.Append("      update TSQJD set dTFTM_BJ =   '" + dTFTM_BJ + "',   \n");
                    sql.Append("      vcOperatorId = '" + strUserId + "',      \n");
                    sql.Append("      dOperatorTime = GETDATE()      \n");
                    sql.Append("      where iAutoId = '" + iAutoId + "'      \n");
                    sql.Append("      and vcJD != '3'      \n");
                }
                excute.ExcuteSqlWithStringOper(sql.ToString(), "TK");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 织入原单位
        public void sendUnit(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErr)
        {
            try
            {
                StringBuilder sql = new StringBuilder();

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
                    sql.Append("        ,vcZXBZNo,vcReceiver,dNqDate,vcOperatorId,dOperatorTime        \n");
                    sql.Append("        )        \n");
                    sql.Append("        values        \n");
                    sql.Append("        (        \n");
                    sql.Append("        "  + ComFunction.getSqlValue(listInfoData[i]["dSSDate"] ,true)+ "        \n");
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
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcIsDYJG"],false) + "        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcIsDYFX"],false) + "        \n");
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
                    sql.Append("        )        \n");
                    
                }
                #endregion

                #region 两表相同的数据进行更新操作（这里的状态加限定：对应可否确认结果为可对应）
                #region 新设
                /*
                 * 新车新设、设变新设
                 */
                sql.Append("        update TUnit set        \n");
                sql.Append("         dTimeFrom = b.dTFTM_BJ       \n");
                sql.Append("        ,vcSQState = '2'       \n");
                sql.Append("        ,vcSQContent = 'OK'       \n");
                sql.Append("        ,vcSCSName = b.vcSCSName       \n");
                sql.Append("        ,vcSCSAdress = b.vcSCSPlace       \n");
                sql.Append("        ,vcSCPlace = b.vcSCPlace_City       \n");
                sql.Append("        ,vcCHPlace = b.vcCHPlace_City       \n");
                sql.Append("        ,vcZXBZNo = b.vcZXBZNo       \n");
                sql.Append("        from TUnit a       \n");
                sql.Append("        inner join       \n");
                sql.Append("        (       \n");
                sql.Append("        	select * from #TSQJD_temp       \n");
                sql.Append("        	where vcChange in ('1','2')       \n");
                sql.Append("        	and vcIsDYJG = 1       \n");
                sql.Append("        )b       \n");
                sql.Append("        on a.vcPart_id = b.vcPart_id       \n");
                #endregion

                #region 复活
                sql.Append("        update TUnit set        \n");
                sql.Append("         dTimeFrom = b.dTFTM_BJ       \n");
                sql.Append("        ,vcSQState = '2'       \n");
                sql.Append("        ,vcSQContent = 'OK'       \n");
                sql.Append("        ,vcSCSName = b.vcSCSName       \n");
                sql.Append("        ,vcSCSAdress = b.vcSCSPlace       \n");
                sql.Append("        ,vcSCPlace = b.vcSCPlace_City       \n");
                sql.Append("        ,vcCHPlace = b.vcCHPlace_City       \n");
                sql.Append("        ,vcZXBZNo = b.vcZXBZNo       \n");
                sql.Append("        from TUnit a       \n");
                sql.Append("        inner join       \n");
                sql.Append("        (       \n");
                sql.Append("        	select * from #TSQJD_temp       \n");
                sql.Append("        	where vcChange in ('16')       \n");
                sql.Append("        	and vcIsDYJG = 1       \n");
                sql.Append("        )b       \n");
                sql.Append("        on a.vcPart_id = b.vcPart_id       \n");
                #endregion

                #region 旧型(打切旧型/设变旧型)
                sql.Append("        update TUnit set        \n");
                sql.Append("         dJiuBegin = b.dTFTM_BJ       \n");
                sql.Append("        ,vcSQState = '2'       \n");
                sql.Append("        ,vcSQContent = 'OK'       \n");
                sql.Append("        ,vcSCSName = b.vcSCSName       \n");
                sql.Append("        ,vcSCSAdress = b.vcSCSPlace       \n");
                sql.Append("        ,vcSCPlace = b.vcSCPlace_City       \n");
                sql.Append("        ,vcCHPlace = b.vcCHPlace_City       \n");
                sql.Append("        ,vcZXBZNo = b.vcZXBZNo       \n");
                sql.Append("        from TUnit a       \n");
                sql.Append("        inner join       \n");
                sql.Append("        (       \n");
                sql.Append("        	select * from #TSQJD_temp       \n");
                sql.Append("        	where vcChange in ('3','5')       \n");
                sql.Append("        	and vcIsDYJG = 1       \n");
                sql.Append("        )b       \n");
                sql.Append("        on a.vcPart_id = b.vcPart_id       \n");
                #endregion

                #region 恢复现号
                sql.Append("        update TUnit set        \n");
                sql.Append("         dJiuEnd = b.dTFTM_BJ       \n");
                sql.Append("        ,vcSQState = '2'       \n");
                sql.Append("        ,vcSQContent = 'OK'       \n");
                sql.Append("        ,vcSCSName = b.vcSCSName       \n");
                sql.Append("        ,vcSCSAdress = b.vcSCSPlace       \n");
                sql.Append("        ,vcSCPlace = b.vcSCPlace_City       \n");
                sql.Append("        ,vcCHPlace = b.vcCHPlace_City       \n");
                sql.Append("        ,vcZXBZNo = b.vcZXBZNo       \n");
                sql.Append("        from TUnit a       \n");
                sql.Append("        inner join       \n");
                sql.Append("        (       \n");
                sql.Append("        	select * from #TSQJD_temp       \n");
                sql.Append("        	where vcChange in ('6')       \n");
                sql.Append("        	and vcIsDYJG = 1       \n");
                sql.Append("        )b       \n");
                sql.Append("        on a.vcPart_id = b.vcPart_id       \n");
                #endregion

                #region 废止
                /*
                 * 设变废止、使用废止
                 */
                sql.Append("        update TUnit set        \n");
                sql.Append("         dTimeTo = b.dTFTM_BJ       \n");
                sql.Append("        ,vcSQState = '2'       \n");
                sql.Append("        ,vcSQContent = 'OK'       \n");
                sql.Append("        ,vcSCSName = b.vcSCSName       \n");
                sql.Append("        ,vcSCSAdress = b.vcSCSPlace       \n");
                sql.Append("        ,vcSCPlace = b.vcSCPlace_City       \n");
                sql.Append("        ,vcCHPlace = b.vcCHPlace_City       \n");
                sql.Append("        ,vcZXBZNo = b.vcZXBZNo       \n");
                sql.Append("        from TUnit a       \n");
                sql.Append("        inner join       \n");
                sql.Append("        (       \n");
                sql.Append("        	select * from #TSQJD_temp       \n");
                sql.Append("        	where vcChange in ('4','21')       \n");
                sql.Append("        	and vcIsDYJG = 1       \n");
                sql.Append("        )b       \n");
                sql.Append("        on a.vcPart_id = b.vcPart_id       \n");
                #endregion

                #region 工程变更-废止
                sql.Append("        update TUnit set        \n");
                sql.Append("         dGYSTimeTo = b.dTFTM_BJ       \n");
                sql.Append("        ,vcSQState = '2'       \n");
                sql.Append("        ,vcSQContent = 'OK'       \n");
                sql.Append("        ,vcSCSName = b.vcSCSName       \n");
                sql.Append("        ,vcSCSAdress = b.vcSCSPlace       \n");
                sql.Append("        ,vcSCPlace = b.vcSCPlace_City       \n");
                sql.Append("        ,vcCHPlace = b.vcCHPlace_City       \n");
                sql.Append("        ,vcZXBZNo = b.vcZXBZNo       \n");
                sql.Append("        from TUnit a       \n");
                sql.Append("        inner join       \n");
                sql.Append("        (       \n");
                sql.Append("        	select * from #TSQJD_temp       \n");
                sql.Append("        	where vcChange in ('9')       \n");
                sql.Append("        	and vcIsDYJG = 1       \n");
                sql.Append("        )b       \n");
                sql.Append("        on a.vcPart_id = b.vcPart_id       \n");
                sql.Append("        and a.vcSupplier_id = b.vcSupplier_id       \n");
                #endregion

                #region 工程变更-新设
                sql.Append("        update TUnit set        \n");
                sql.Append("         dGYSTimeFrom = b.dTFTM_BJ       \n");
                sql.Append("        ,vcSQState = '2'       \n");
                sql.Append("        ,vcSQContent = 'OK'       \n");
                sql.Append("        ,vcSCSName = b.vcSCSName       \n");
                sql.Append("        ,vcSCSAdress = b.vcSCSPlace       \n");
                sql.Append("        ,vcSCPlace = b.vcSCPlace_City       \n");
                sql.Append("        ,vcCHPlace = b.vcCHPlace_City       \n");
                sql.Append("        ,vcZXBZNo = b.vcZXBZNo       \n");
                sql.Append("        from TUnit a       \n");
                sql.Append("        inner join       \n");
                sql.Append("        (       \n");
                sql.Append("        	select * from #TSQJD_temp       \n");
                sql.Append("        	where vcChange in ('8')       \n");
                sql.Append("        	and vcIsDYJG = 1       \n");
                sql.Append("        )b       \n");
                sql.Append("        on a.vcPart_id = b.vcPart_id       \n");
                sql.Append("        and a.vcSupplier_id = b.vcSupplier_id       \n");
                #endregion

                #region 供应商变更-废止
                sql.Append("        update TUnit set        \n");
                sql.Append("         dGYSTimeTo = b.dTFTM_BJ       \n");
                sql.Append("        ,vcSQState = '2'       \n");
                sql.Append("        ,vcSQContent = 'OK'       \n");
                sql.Append("        ,vcSCSName = b.vcSCSName       \n");
                sql.Append("        ,vcSCSAdress = b.vcSCSPlace       \n");
                sql.Append("        ,vcSCPlace = b.vcSCPlace_City       \n");
                sql.Append("        ,vcCHPlace = b.vcCHPlace_City       \n");
                sql.Append("        ,vcZXBZNo = b.vcZXBZNo       \n");
                sql.Append("        from TUnit a       \n");
                sql.Append("        inner join       \n");
                sql.Append("        (       \n");
                sql.Append("        	select * from #TSQJD_temp       \n");
                sql.Append("        	where vcChange in ('11')       \n");
                sql.Append("        	and vcIsDYJG = 1       \n");
                sql.Append("        )b       \n");
                sql.Append("        on a.vcPart_id = b.vcPart_id       \n");
                sql.Append("        and a.vcSupplier_id = b.vcSupplier_id       \n");
                #endregion

                #region 供应商变更-新设
                sql.Append("        update TUnit set        \n");
                sql.Append("         dGYSTimeFrom = b.dTFTM_BJ       \n");
                sql.Append("        ,vcSQState = '2'       \n");
                sql.Append("        ,vcSQContent = 'OK'       \n");
                sql.Append("        ,vcSCSName = b.vcSCSName       \n");
                sql.Append("        ,vcSCSAdress = b.vcSCSPlace       \n");
                sql.Append("        ,vcSCPlace = b.vcSCPlace_City       \n");
                sql.Append("        ,vcCHPlace = b.vcCHPlace_City       \n");
                sql.Append("        ,vcZXBZNo = b.vcZXBZNo       \n");
                sql.Append("        from TUnit a       \n");
                sql.Append("        inner join       \n");
                sql.Append("        (       \n");
                sql.Append("        	select * from #TSQJD_temp       \n");
                sql.Append("        	where vcChange in ('10')       \n");
                sql.Append("        	and vcIsDYJG = 1       \n");
                sql.Append("        )b       \n");
                sql.Append("        on a.vcPart_id = b.vcPart_id       \n");
                sql.Append("        and a.vcSupplier_id = b.vcSupplier_id       \n");
                #endregion
                #endregion

                #region 对应不可时，将生确状态改为NG
                sql.Append("        update TUnit set        \n");
                sql.Append("        vcSQState = '3'       \n");
                sql.Append("        ,vcSQContent = 'NG'       \n");
                sql.Append("        from TUnit a       \n");
                sql.Append("        inner join        \n");
                sql.Append("        (       \n");
                sql.Append("        	select vcPart_id from #TSQJD_temp       \n");
                sql.Append("        	where vcIsDYJG = '2'       \n");
                sql.Append("        ) b       \n");
                sql.Append("        on a.vcPart_id = b.vcPart_id       \n");
                #endregion

                #region 更新生确进度表
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    var iAutoId = listInfoData[i]["iAutoId"].ToString();
                    sql.Append("        update TSQJD set vcJD = '4'        \n");
                    sql.Append("        where iAutoId = '"+iAutoId+"'        \n");
                }
                #endregion

                if (sql.Length>0)
                {
                    excute.ExcuteSqlWithStringOper(sql.ToString(), "TK");
                }
            }
            catch (Exception ex)
            {
                strErr = ex.Message;
            }
        }
        #endregion

        #region 按检索条件返回dt
        public DataTable getSupplierEmail(string strSupplierId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("    select vcEmail1,vcEmail2,vcEmail3 from TSupplier where vcSupplier_id='" + strSupplierId + "'   \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString(), "TK");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 按检索条件返回dt
        public DataTable getEmailSetting(string strUserId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("    select vcTitle,vcContent from TMailMessageSetting where vcUserId='" + strUserId + "' and vcChildFunID = 'FS0304'  \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString(), "TK");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }
}
