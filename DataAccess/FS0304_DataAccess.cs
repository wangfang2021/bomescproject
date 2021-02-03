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
    public class FS0304_DataAccess 
    {
        private MultiExcute excute = new MultiExcute();

        #region 按检索条件返回dt
        public DataTable Search(string strSSDateMonth,string strJD, string strPart_id, string strInOutFlag, string strIsDYJG, string strCarType, string strSupplier_id)
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
                strSql.Append("     	select SUBSTRING(vcValue,1,4) as 'vcValue',vcName from TCode where vcCodeId = 'C098'    ");
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
                if (!string.IsNullOrEmpty(strSSDateMonth))
                {
                    strSql.Append("      and dSSDate = '" + strSSDateMonth + "'  ");
                }
                if (!string.IsNullOrEmpty(strJD))
                {
                    strSql.Append("      and vcJD = '" + strJD + "'   ");
                }
                if (!string.IsNullOrEmpty(strPart_id))
                {
                    strSql.Append("      and vc Part_id = '" + strPart_id + "'   ");
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
                    strSql.Append("      and vcCarType='" + strCarType + "'   ");
                }
                if (!string.IsNullOrEmpty(strSupplier_id))
                {
                    strSql.Append("      and vcSupplier_id='"+ strSupplier_id + "'   ");
                }
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

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
                        sql.Append("      dSSDateMonth = "+ ComFunction.getSqlValue(listInfoData[i]["dSSDateMonth"], true) + "      \n");
                        sql.Append("      ,vcJD = " + ComFunction.getSqlValue(listInfoData[i]["vcJD"], false) + "      \n");
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
                    }
                }
                excute.ExcuteSqlWithStringOper(sql.ToString());
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
        }
        #endregion

        #region 退回
        public void Back(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            #region 给供应商发送邮件提醒

            #endregion

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
                    sql.Append("      ),      \n");
                    sql.Append("      vcOperatorId = '" + strUserId + "',      \n");
                    sql.Append("      dOperatorTime = GETDATE()      \n");
                    sql.Append("      where iAutoId = '" + iAutoId + "'      \n");
                }
                excute.ExcuteSqlWithStringOper(sql.ToString());
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

        #region 付与日期一括付与
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
                }
                excute.ExcuteSqlWithStringOper(sql.ToString());
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
        }
        #endregion

        #region 织入原单位
        public void sendUnit(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();

                #region 创建临时表
                sql.Append("        if object_id('tempdb..#TUnit_temp') is not null  \n");
                sql.Append("        Begin  \n");
                sql.Append("        drop  table #TUnit_temp  \n");
                sql.Append("        End  \n");
                sql.Append("        select * into #TUnit_temp from       \n");
                sql.Append("        (      \n");
                sql.Append("      	  select * from TUnit where 1=0      \n");
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
                    sql.Append("        '" + listInfoData[i]["dSSDate"] + "'        \n");
                    sql.Append("        '" + listInfoData[i]["vcJD"] + "'        \n");
                    sql.Append("        '" + listInfoData[i]["vcPart_id"] + "'        \n");
                    sql.Append("        '" + listInfoData[i]["vcSPINo"] + "'        \n");
                    sql.Append("        '" + listInfoData[i]["vcChange"] + "'        \n");
                    sql.Append("        '" + listInfoData[i]["vcCarType"] + "'        \n");
                    sql.Append("        '" + listInfoData[i]["vcInOutflag"] + "'        \n");
                    sql.Append("        '" + listInfoData[i]["vcPartName"] + "'        \n");
                    sql.Append("        '" + listInfoData[i]["vcOE"] + "'        \n");
                    sql.Append("        '" + listInfoData[i]["vcSupplier_id"] + "'        \n");
                    sql.Append("        '" + listInfoData[i]["vcFXDiff"] + "'        \n");
                    sql.Append("        '" + listInfoData[i]["vcFXNo"] + "'        \n");
                    sql.Append("        '" + listInfoData[i]["vcSumLater"] + "'        \n");
                    sql.Append("        '" + listInfoData[i]["vcNum1"] + "'        \n");
                    sql.Append("        '" + listInfoData[i]["vcNum2"] + "'        \n");
                    sql.Append("        '" + listInfoData[i]["vcNum3"] + "'        \n");
                    sql.Append("        '" + listInfoData[i]["vcNum4"] + "'        \n");
                    sql.Append("        '" + listInfoData[i]["vcNum5"] + "'        \n");
                    sql.Append("        '" + listInfoData[i]["vcNum6"] + "'        \n");
                    sql.Append("        '" + listInfoData[i]["vcNum7"] + "'        \n");
                    sql.Append("        '" + listInfoData[i]["vcNum8"] + "'        \n");
                    sql.Append("        '" + listInfoData[i]["vcNum9"] + "'        \n");
                    sql.Append("        '" + listInfoData[i]["vcNum10"] + "'        \n");
                    sql.Append("        '" + listInfoData[i]["vcIsDYJG"] + "'        \n");
                    sql.Append("        '" + listInfoData[i]["vcIsDYFX"] + "'        \n");
                    sql.Append("        '" + listInfoData[i]["vcYQorNG"] + "'        \n");
                    sql.Append("        '" + listInfoData[i]["vcTH"] + "'        \n");
                    sql.Append("        '" + listInfoData[i]["vcSCPlace_City"] + "'        \n");
                    sql.Append("        '" + listInfoData[i]["vcSCPlace_Province"] + "'        \n");
                    sql.Append("        '" + listInfoData[i]["vcCHPlace_City"] + "'        \n");
                    sql.Append("        '" + listInfoData[i]["vcCHPlace_Province"] + "'        \n");
                    sql.Append("        '" + listInfoData[i]["vcSYTCode"] + "'        \n");
                    sql.Append("        '" + listInfoData[i]["vcSCSName"] + "'        \n");
                    sql.Append("        '" + listInfoData[i]["vcSCSPlace"] + "'        \n");
                    sql.Append("        '" + listInfoData[i]["dSupplier_BJ"] + "'        \n");
                    sql.Append("        '" + listInfoData[i]["dSupplier_HK"] + "'        \n");
                    sql.Append("        '" + listInfoData[i]["dTFTM_BJ"] + "'        \n");
                    sql.Append("        '" + listInfoData[i]["vcZXBZDiff"] + "'        \n");
                    sql.Append("        '" + listInfoData[i]["vcZXBZNo"] + "'        \n");
                    sql.Append("        '" + listInfoData[i]["vcReceiver"] + "'        \n");
                    sql.Append("        '" + listInfoData[i]["dNqDate"] + "'        \n");
                    sql.Append("        '" + listInfoData[i]["vcOperatorId"] + "'        \n");
                    sql.Append("        '" + listInfoData[i]["dOperatorTime"] + "'        \n");
                    sql.Append("        )        \n");
                    
                }
                #endregion

                #region 两表相同的数据进行更新操作
                #region 新设
                sql.Append("       update TUnit set        \n");
                sql.Append("        vcSYTCode = a.vcSYTCode       \n");
                sql.Append("       ,dTimeFrom = a.dTFTM_BJ       \n");
                sql.Append("       ,vcSCPlace = a.vcSCPlace_City       \n");
                sql.Append("       ,vcCHPlace = a.vcCHPlace_City       \n");
                sql.Append("       ,vcSCSName = a.vcSCSName       \n");
                sql.Append("       ,vcSCSAdress = a.vcSCSPlace       \n");
                sql.Append("       ,vcZXBZNo = a.vcZXBZNo       \n");
                sql.Append("       from        \n");
                sql.Append("       (       \n");
                sql.Append("       	select * from #TSQJD_temp       \n");
                sql.Append("       	where vcChange = '1' or vcChange = '2'       \n");
                sql.Append("       ) a       \n");
                sql.Append("       inner join TUnit b       \n");
                sql.Append("       on a.vcPart_id = b.vcPart_id       \n");
                sql.Append("       and a.vcSupplier_id = b.vcSupplier_id       \n");
                sql.Append("       and a.dSSDate = b.dSSDate       \n");
                #endregion
                #region 废止
                sql.Append("       update TUnit set        \n");
                sql.Append("        vcSYTCode = a.vcSYTCode       \n");
                sql.Append("       ,dTimeTo = a.dTFTM_BJ       \n");
                sql.Append("       ,vcSCPlace = a.vcSCPlace_City       \n");
                sql.Append("       ,vcCHPlace = a.vcCHPlace_City       \n");
                sql.Append("       ,vcSCSName = a.vcSCSName       \n");
                sql.Append("       ,vcSCSAdress = a.vcSCSPlace       \n");
                sql.Append("       ,vcZXBZNo = a.vcZXBZNo       \n");
                sql.Append("       from        \n");
                sql.Append("       (       \n");
                sql.Append("       	select * from #TSQJD_temp       \n");
                sql.Append("       	where vcChange = '4'       \n");
                sql.Append("       ) a       \n");
                sql.Append("       inner join TUnit b       \n");
                sql.Append("       on a.vcPart_id = b.vcPart_id       \n");
                sql.Append("       and a.vcSupplier_id = b.vcSupplier_id       \n");
                sql.Append("       and a.dSSDate = b.dSSDate       \n");
                #endregion
                #region 复活
                sql.Append("       update TUnit set        \n");
                sql.Append("        vcSYTCode = a.vcSYTCode       \n");
                sql.Append("       ,dTimeFrom = a.dTFTM_BJ       \n");
                sql.Append("       ,vcSCPlace = a.vcSCPlace_City       \n");
                sql.Append("       ,vcCHPlace = a.vcCHPlace_City       \n");
                sql.Append("       ,vcSCSName = a.vcSCSName       \n");
                sql.Append("       ,vcSCSAdress = a.vcSCSPlace       \n");
                sql.Append("       ,vcZXBZNo = a.vcZXBZNo       \n");
                sql.Append("       from        \n");
                sql.Append("       (       \n");
                sql.Append("       	select * from #TSQJD_temp       \n");
                sql.Append("       	where vcChange = '16'       \n");
                sql.Append("       ) a       \n");
                sql.Append("       inner join TUnit b       \n");
                sql.Append("       on a.vcPart_id = b.vcPart_id       \n");
                sql.Append("       and a.vcSupplier_id = b.vcSupplier_id       \n");
                sql.Append("       and a.dSSDate = b.dSSDate       \n");
                #endregion
                #region 旧型
                sql.Append("       update TUnit set        \n");
                sql.Append("        vcSYTCode = a.vcSYTCode       \n");
                sql.Append("       ,dJiuBegin = a.dTFTM_BJ       \n");
                sql.Append("       ,vcSCPlace = a.vcSCPlace_City       \n");
                sql.Append("       ,vcCHPlace = a.vcCHPlace_City       \n");
                sql.Append("       ,vcSCSName = a.vcSCSName       \n");
                sql.Append("       ,vcSCSAdress = a.vcSCSPlace       \n");
                sql.Append("       ,vcZXBZNo = a.vcZXBZNo       \n");
                sql.Append("       from        \n");
                sql.Append("       (       \n");
                sql.Append("       	select * from #TSQJD_temp       \n");
                sql.Append("       	where vcChange = '3' or vcChange = '5'       \n");
                sql.Append("       ) a       \n");
                sql.Append("       inner join TUnit b       \n");
                sql.Append("       on a.vcPart_id = b.vcPart_id       \n");
                sql.Append("       and a.vcSupplier_id = b.vcSupplier_id       \n");
                sql.Append("       and a.dSSDate = b.dSSDate       \n");
                #endregion
                #region 工程变更-废止

                #endregion
                #region 工程变更-新设

                #endregion
                #endregion

                #region 更新生确进度表
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    var iAutoId = listInfoData[i]["iAutoId"].ToString();
                    sql.Append("        update TSQJD set vcJD =         \n");
                    sql.Append("        (        \n");
                    sql.Append("        	select vcValue from TCode where vcCodeId = 'C026' and vcName like '%已织入%'        \n");
                    sql.Append("        )        \n");
                    sql.Append("        where iAutoId = '"+iAutoId+"'        \n");
                }
                #endregion
                if (sql.Length>0)
                {
                    excute.ExcuteSqlWithStringOper(sql.ToString());
                }
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
        }
        #endregion

    }
}
