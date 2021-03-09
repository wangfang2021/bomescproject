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

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                getTempData(listInfoData, sql, strUserId, ref strErrorPartId);

                #region 更新生确表中的数据
                sql.Append("          update TSQJD set           \n");
                sql.Append("           vcJD = b.vcJD          \n");
                sql.Append("          ,dTFTM_BJ = b.dTFTM_BJ          \n");
                sql.Append("          ,vcOperatorId = '" + strUserId+"'          \n");
                sql.Append("          ,dOperatorTime = GETDATE()          \n");
                sql.Append("          from TSQJD a           \n");
                sql.Append("          inner join #TSQJD_temp b          \n");
                sql.Append("          on a.[GUID] = b.[GUID]          \n");
                #endregion

                #region 更新供应商生确表中的数据,注意不更新进度(供应商的生确进度没有已织入)
                sql.Append("          update TSQJD_Supplier set           \n");
                sql.Append("           dTFTM_BJ = b.dTFTM_BJ          \n");
                sql.Append("          ,vcOperatorId = '" + strUserId + "'          \n");
                sql.Append("          ,dOperatorTime = GETDATE()          \n");
                sql.Append("          from TSQJD a           \n");
                sql.Append("          inner join #TSQJD_temp b          \n");
                sql.Append("          on a.[GUID] = b.[GUID]          \n");
                #endregion

                //设变废止：TFTM调整时间需要大于等于品番开始时间
                if (sql.Length > 0)
                {
                    sql.Append("        declare @errorPart varchar(5000)        \r\n");
                    sql.Append("        set @errorPart =         \r\n");
                    sql.Append("        (        \r\n");
                    sql.Append("        select a.vcPart_id+';' from         \r\n");
                    sql.Append("        (        \r\n");
                    sql.Append("           select distinct vcPart_id from         \r\n");
                    sql.Append("           (         \r\n");
                    sql.Append("                select a.* from                 \r\n");
                    sql.Append("         	    (                \r\n");
                    sql.Append("         	    	select vcPart_id,dTimeFrom,vcSYTCode,vcSupplier_id,vcReceiver from TUnit                \r\n");
                    sql.Append("                ) a                \r\n");
                    sql.Append("         	    inner join                \r\n");
                    sql.Append("         	    (                \r\n");
                    sql.Append("                	select vcPart_id,vcSYTCode,vcSupplier_id,vcReceiver,dTFTM_BJ from TSQJD where vcChange = '4'                \r\n");
                    sql.Append("         	    ) b on a.vcPart_id = b.vcPart_id                \r\n");
                    sql.Append("         	    and a.vcSYTCode = b.vcSYTCode                \r\n");
                    sql.Append("                and a.vcSupplier_id = b.vcSupplier_id                \r\n");
                    sql.Append("         	    and a.vcReceiver = b.vcReceiver                \r\n");
                    sql.Append("         	    and b.dTFTM_BJ < a.dTimeFrom                \r\n");
                    sql.Append("           )a         \r\n");
                    sql.Append("        ) a for xml path('')        \r\n");
                    sql.Append("        )        \r\n");
                    sql.Append("        if @errorPart<>''        \r\n");
                    sql.Append("        begin        \r\n");
                    sql.Append("        select CONVERT(int,'-->'+@errorPart+'<--')        \r\n");
                    sql.Append("        end        \r\n");
                    excute.ExcuteSqlWithStringOper(sql.ToString(), "TK");
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

        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId,ref string strErr)
        {
            try
            {
                StringBuilder sqlStr = new StringBuilder();
                getTempData(listInfoData, sqlStr, strUserId,ref strErr);

                #region 删除生确表的数据
                sqlStr.Append("        delete TSQJD where GUID in ( select GUID from #TSQJD_temp )         \n");
                #endregion

                #region 删除供应商生确表中的数据
                sqlStr.Append("        delete TSQJD_Supplier where GUID in ( select GUID from #TSQJD_temp )         \n");
                #endregion

                #region 删除退回履历表中的数据
                sqlStr.Append("        delete TSQJD_THlist where GUID in ( select GUID from #TSQJD_temp )         \n");
                #endregion

                excute.ExcuteSqlWithStringOper(sqlStr.ToString(), "TK");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 退回
        public void Back(List<Dictionary<string, Object>> listInfoData, string strUserId,string strTH,ref string strErr)
        {
            try
            {
                StringBuilder sql = new StringBuilder();

                #region 获取临时表，将用户所选数据插入临时表
                getTempData(listInfoData, sql, strUserId,ref strErr);
                #endregion

                #region 更新生确进度表的进度为已退回,记录操作者，操作时间
                sql.Append("          update TSQJD set           \n");
                sql.Append("           vcJD = '3'          \n");
                sql.Append("          ,vcOperatorId = '"+strUserId+"'          \n");
                sql.Append("          ,dOperatorTime = GETDATE()          \n");
                sql.Append("          from TSQJD a          \n");
                sql.Append("          inner join           \n");
                sql.Append("          (          \n");
                sql.Append("          select GUID from #TSQJD_temp          \n");
                sql.Append("          )b          \n");
                sql.Append("          on a.GUID = b.GUID          \n");
                #endregion

                #region 更新供应商生确表进度为已退回，记录操作者，操作时间
                sql.Append("          update TSQJD_Supplier set           \n");
                sql.Append("           vcJD = '3'          \n");
                sql.Append("          ,vcOperatorId = '" + strUserId + "'          \n");
                sql.Append("          ,dOperatorTime = GETDATE()          \n");
                sql.Append("          from TSQJD a          \n");
                sql.Append("          inner join           \n");
                sql.Append("          (          \n");
                sql.Append("          select GUID from #TSQJD_temp          \n");
                sql.Append("          )b          \n");
                sql.Append("          on a.GUID = b.GUID          \n");
                #endregion

                #region 在履历表中记录退回信息
                sql.Append("      insert into TSQJD_THlist (GUID,vcPart_id,vcTHText,dTHTime,vcOperatorID,dOperatorTime)          \n");
                sql.Append("      select GUID,vcPart_id,'"+ strTH + "',GETDATE(),'"+strUserId+"',GETDATE() from #TSQJD_temp          \n");
                #endregion

                if (sql.Length>0)
                {
                    excute.ExcuteSqlWithStringOper(sql.ToString(), "TK");
                }
            }
            catch (Exception ex)
            {
                strErr += ex.Message.ToString();
            }
            


            //#region 将进度变为已退回
            //try
            //{
            //    StringBuilder sql = new StringBuilder();
            //    for (int i = 0; i < listInfoData.Count; i++)
            //    {
            //        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);

            //        sql.Append("      update TSQJD set vcJD =      \n");
            //        sql.Append("      (      \n");
            //        sql.Append("      select vcValue from TCode where vcCodeId = 'C026' and vcName = '已退回'      \n");
            //        sql.Append("      )      \n");
            //        sql.Append("      ,vcTH = '"+strTH+"'      \n");
            //        sql.Append("      ,vcOperatorId = '" + strUserId + "'      \n");
            //        sql.Append("      ,dOperatorTime = GETDATE()      \n");
            //        sql.Append("      where iAutoId = '" + iAutoId + "'      \n");
            //    }


            //    excute.ExcuteSqlWithStringOper(sql.ToString(), "TK");
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            //#endregion

        }
        #endregion

        #region 日期一括付与
        public void DateKFY(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId, string dTFTM_BJ)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                getTempData(listInfoData, sql, strUserId,ref strErrorPartId);

                #region 更新生确表中的数据
                sql.Append("          update TSQJD set           \n");
                sql.Append("          ,dTFTM_BJ = '"+dTFTM_BJ+"'          \n");
                sql.Append("          ,vcOperatorId = '" + strUserId + "'          \n");
                sql.Append("          ,dOperatorTime = GETDATE()          \n");
                sql.Append("          from TSQJD a           \n");
                sql.Append("          inner join #TSQJD_temp b          \n");
                sql.Append("          on a.[GUID] = b.[GUID]          \n");
                #endregion

                #region 更新供应商生确表中的数据
                sql.Append("          update TSQJD_Supplier set           \n");
                sql.Append("          ,dTFTM_BJ = '"+dTFTM_BJ+"'          \n");
                sql.Append("          ,vcOperatorId = '" + strUserId + "'          \n");
                sql.Append("          ,dOperatorTime = GETDATE()          \n");
                sql.Append("          from TSQJD a           \n");
                sql.Append("          inner join #TSQJD_temp b          \n");
                sql.Append("          on a.[GUID] = b.[GUID]          \n");
                #endregion

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
                    sql.Append("        ,vcZXBZNo,vcReceiver,dNqDate,vcOperatorId,dOperatorTime,GUID        \n");
                    sql.Append("        )        \n");
                    sql.Append("        values        \n");
                    sql.Append("        (        \n");
                    sql.Append("        " + ComFunction.getSqlValue(listInfoData[i]["dSSDate"], true) + "        \n");
                    sql.Append("        ," + ComFunction.getSqlValue(listInfoData[i]["vcJD"], false) + "        \n");
                    /*
                     * 品番还原
                     */
                    string strPart_id = listInfoData[i]["vcPart_id"].ToString();
                    strPart_id = strPart_id.Insert(10, "-");
                    strPart_id = strPart_id.Insert(5, "-");
                    if (strPart_id.Substring(11, 3) == "-00")
                    {
                        strPart_id = strPart_id.Remove(11, 3);
                    }
                    sql.Append("        ,'"+strPart_id+"'        \n");
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

                #region 两表相同的数据进行更新操作（这里的状态加限定：对应可否确认结果为可对应）
                #region 新设
                #region 根据品番织入TFTM调整日期
                sql.Append("        update TUnit set        \n");
                sql.Append("         dTimeFrom = b.dTFTM_BJ       \n");
                sql.Append("        ,dTimeTo = '9999/12/31'       \n");
                sql.Append("        from TUnit a       \n");
                sql.Append("        inner join       \n");
                sql.Append("        (       \n");
                sql.Append("        	select a.* from #TSQJD_temp a      \n");
                sql.Append("        	where vcChange in ('1','2')       \n");
                sql.Append("        	and vcIsDYJG = 1       \n");
                sql.Append("        )b       \n");
                sql.Append("        on a.vcPart_id = b.vcPart_id       \n");
                #endregion
                #region 根据品番和供应商代码织入生产地和出荷地
                sql.Append("        update TUnit set        \n");
                sql.Append("         vcSQState = '2'       \n");
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
                sql.Append("        and a.vcSupplier_id = b.vcSupplier_id       \n");
                #endregion

                #endregion

                #region 复活
                sql.Append("        update TUnit set        \n");
                sql.Append("         vcSQState = '2'       \n");
                sql.Append("        ,vcSQContent = 'OK'       \n");                
                sql.Append("        from TUnit a       \n");
                sql.Append("        inner join       \n");
                sql.Append("        (       \n");
                sql.Append("        	select * from #TSQJD_temp        \n");
                sql.Append("        	where vcChange in ('16')       \n");
                sql.Append("        	and vcIsDYJG = 1       \n");
                sql.Append("        )b       \n");
                sql.Append("        on a.vcPart_id = b.vcPart_id       \n");
                #endregion

                #region 旧型(打切旧型/设变旧型)
                #region 根据品番织入TFTM调整日期
                sql.Append("        update TUnit set        \n");
                sql.Append("         dJiuBegin = b.dTFTM_BJ       \n");
                sql.Append("        from TUnit a       \n");
                sql.Append("        inner join       \n");
                sql.Append("        (       \n");
                sql.Append("        	select * from #TSQJD_temp       \n");
                sql.Append("        	where vcChange in ('3','5')       \n");
                sql.Append("        	and vcIsDYJG = 1       \n");
                sql.Append("        )b       \n");
                sql.Append("        on a.vcPart_id = b.vcPart_id       \n");
                #endregion
                #region 根据品番和供应商代码织入生产地和出荷地
                sql.Append("        update TUnit set        \n");
                sql.Append("         vcSQState = '2'       \n");
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
                sql.Append("        and a.vcSupplier_id = b.vcSupplier_id       \n");
                #endregion
                #endregion

                #region 恢复现号
                #region 根据品番织入TFTM调整日期
                sql.Append("        update TUnit set        \n");
                sql.Append("         dJiuEnd = b.dTFTM_BJ       \n");
                sql.Append("        from TUnit a       \n");
                sql.Append("        inner join       \n");
                sql.Append("        (       \n");
                sql.Append("        	select a.* from #TSQJD_temp a      \n");
                sql.Append("        	where vcChange in ('6')       \n");
                sql.Append("        	and vcIsDYJG = 1       \n");
                sql.Append("        )b       \n");
                sql.Append("        on a.vcPart_id = b.vcPart_id       \n");
                #endregion
                #region 根据品番和供应商代码织入生产地和出荷地
                sql.Append("        update TUnit set        \n");
                sql.Append("         vcSQState = '2'       \n");
                sql.Append("        ,vcSQContent = 'OK'       \n");
                sql.Append("        ,vcSCSName = b.vcSCSName       \n");
                sql.Append("        ,vcSCSAdress = b.vcSCSPlace       \n");
                sql.Append("        ,vcSCPlace = b.vcSCPlace_City       \n");
                sql.Append("        ,vcCHPlace = b.vcCHPlace_City       \n");
                sql.Append("        ,vcZXBZNo = b.vcZXBZNo       \n");
                sql.Append("        from TUnit a       \n");
                sql.Append("        inner join       \n");
                sql.Append("        (       \n");
                sql.Append("        	select a.* from #TSQJD_temp a      \n");
                sql.Append("        	where vcChange in ('6')       \n");
                sql.Append("        	and vcIsDYJG = 1       \n");
                sql.Append("        )b       \n");
                sql.Append("        on a.vcPart_id = b.vcPart_id       \n");
                sql.Append("        and a.vcSupplier_id = b.vcSupplier_id       \n");
                #endregion
                #endregion

                #region 废止
                #region 根据品番织入TFTM调整日期
                sql.Append("        update TUnit set        \n");
                sql.Append("         dTimeTo = b.dTFTM_BJ       \n");
                sql.Append("        from TUnit a       \n");
                sql.Append("        inner join       \n");
                sql.Append("        (       \n");
                sql.Append("        	select a.* from #TSQJD_temp  a     \n");
                sql.Append("        	where vcChange in ('4','21')       \n");
                sql.Append("        	and vcIsDYJG = 1       \n");
                sql.Append("        )b       \n");
                sql.Append("        on a.vcPart_id = b.vcPart_id       \n");
                #endregion
                #region 根据品番和供应商代码织入生产地和出荷地
                sql.Append("        update TUnit set        \n");
                sql.Append("         vcSQState = '2'       \n");
                sql.Append("        ,vcSQContent = 'OK'       \n");
                sql.Append("        ,vcSCSName = b.vcSCSName       \n");
                sql.Append("        ,vcSCSAdress = b.vcSCSPlace       \n");
                sql.Append("        ,vcSCPlace = b.vcSCPlace_City       \n");
                sql.Append("        ,vcCHPlace = b.vcCHPlace_City       \n");
                sql.Append("        ,vcZXBZNo = b.vcZXBZNo       \n");
                sql.Append("        from TUnit a       \n");
                sql.Append("        inner join       \n");
                sql.Append("        (       \n");
                sql.Append("        	select a.* from #TSQJD_temp a      \n");
                sql.Append("        	where vcChange in ('4','21')       \n");
                sql.Append("        	and vcIsDYJG = 1       \n");
                sql.Append("        )b       \n");
                sql.Append("        on a.vcPart_id = b.vcPart_id       \n");
                sql.Append("        and a.vcSupplier_id = b.vcSupplier_id       \n");
                #endregion
                #endregion

                #region 工程变更-废止
                #region 根据品番织入TFTM调整日期
                sql.Append("        update TUnit set        \n");
                sql.Append("         dGYSTimeTo = b.dTFTM_BJ       \n");
                sql.Append("        from TUnit a       \n");
                sql.Append("        inner join       \n");
                sql.Append("        (       \n");
                sql.Append("        	select a.* from #TSQJD_temp a      \n");
                sql.Append("        	where vcChange in ('9')       \n");
                sql.Append("        	and vcIsDYJG = 1       \n");
                sql.Append("        )b       \n");
                sql.Append("        on a.vcPart_id = b.vcPart_id       \n");
                sql.Append("        and a.vcSupplier_id = b.vcSupplier_id       \n");
                #endregion
                #region 根据品番和供应商代码织入生产地和出荷地
                sql.Append("        update TUnit set        \n");
                sql.Append("         vcSQState = '2'       \n");
                sql.Append("        ,vcSQContent = 'OK'       \n");
                sql.Append("        ,vcSCSName = b.vcSCSName       \n");
                sql.Append("        ,vcSCSAdress = b.vcSCSPlace       \n");
                sql.Append("        ,vcSCPlace = b.vcSCPlace_City       \n");
                sql.Append("        ,vcCHPlace = b.vcCHPlace_City       \n");
                sql.Append("        ,vcZXBZNo = b.vcZXBZNo       \n");
                sql.Append("        from TUnit a       \n");
                sql.Append("        inner join       \n");
                sql.Append("        (       \n");
                sql.Append("        	select a.* from #TSQJD_temp a      \n");
                sql.Append("        	where vcChange in ('9')       \n");
                sql.Append("        	and vcIsDYJG = 1       \n");
                sql.Append("        )b       \n");
                sql.Append("        on a.vcPart_id = b.vcPart_id       \n");
                sql.Append("        and a.vcSupplier_id = b.vcSupplier_id       \n");
                #endregion

                #endregion

                #region 工程变更-新设
                #region 根据品番织入TFTM调整日期
                sql.Append("        update TUnit set        \n");
                sql.Append("         dGYSTimeFrom = b.dTFTM_BJ       \n");
                sql.Append("        ,dGYSTimeTo = '9999/12/31'       \n");
                sql.Append("        from TUnit a       \n");
                sql.Append("        inner join       \n");
                sql.Append("        (       \n");
                sql.Append("        	select a.* from #TSQJD_temp a      \n");
                sql.Append("        	where vcChange in ('8')       \n");
                sql.Append("        	and vcIsDYJG = 1       \n");
                sql.Append("        )b       \n");
                sql.Append("        on a.vcPart_id = b.vcPart_id       \n");
                sql.Append("        and a.vcSupplier_id = b.vcSupplier_id       \n");
                #endregion
                #region 根据品番和供应商代码织入生产地和出荷地
                sql.Append("        update TUnit set        \n");
                sql.Append("         vcSQState = '2'       \n");
                sql.Append("        ,vcSQContent = 'OK'       \n");
                sql.Append("        ,vcSCSName = b.vcSCSName       \n");
                sql.Append("        ,vcSCSAdress = b.vcSCSPlace       \n");
                sql.Append("        ,vcSCPlace = b.vcSCPlace_City       \n");
                sql.Append("        ,vcCHPlace = b.vcCHPlace_City       \n");
                sql.Append("        ,vcZXBZNo = b.vcZXBZNo       \n");
                sql.Append("        from TUnit a       \n");
                sql.Append("        inner join       \n");
                sql.Append("        (       \n");
                sql.Append("        	select a.* from #TSQJD_temp a      \n");
                sql.Append("        	where vcChange in ('8')       \n");
                sql.Append("        	and vcIsDYJG = 1       \n");
                sql.Append("        )b       \n");
                sql.Append("        on a.vcPart_id = b.vcPart_id       \n");
                sql.Append("        and a.vcSupplier_id = b.vcSupplier_id       \n");
                #endregion
                #endregion

                #region 供应商变更-废止
                #region 根据品番织入TFTM调整日期
                sql.Append("        update TUnit set        \n");
                sql.Append("         dGYSTimeTo = b.dTFTM_BJ       \n");
                sql.Append("        from TUnit a       \n");
                sql.Append("        inner join       \n");
                sql.Append("        (       \n");
                sql.Append("        	select a.* from #TSQJD_temp a      \n");
                sql.Append("        	inner join       \n");
                sql.Append("        	(       \n");
                sql.Append("        		select [GUID] from TSQJD       \n");
                sql.Append("        	)b on a.[GUID] = b.[GUID]       \n");
                sql.Append("        	where vcChange in ('11')       \n");
                sql.Append("        	and vcIsDYJG = 1       \n");
                sql.Append("        )b       \n");
                sql.Append("        on a.vcPart_id = b.vcPart_id       \n");
                sql.Append("        and a.vcSupplier_id = b.vcSupplier_id       \n");
                #endregion
                #region 根据品番和供应商代码织入生产地和出荷地
                sql.Append("        update TUnit set        \n");
                sql.Append("         vcSQState = '2'       \n");
                sql.Append("        ,vcSQContent = 'OK'       \n");
                sql.Append("        ,vcSCSName = b.vcSCSName       \n");
                sql.Append("        ,vcSCSAdress = b.vcSCSPlace       \n");
                sql.Append("        ,vcSCPlace = b.vcSCPlace_City       \n");
                sql.Append("        ,vcCHPlace = b.vcCHPlace_City       \n");
                sql.Append("        ,vcZXBZNo = b.vcZXBZNo       \n");
                sql.Append("        from TUnit a       \n");
                sql.Append("        inner join       \n");
                sql.Append("        (       \n");
                sql.Append("        	select a.* from #TSQJD_temp a      \n");
                sql.Append("        	inner join       \n");
                sql.Append("        	(       \n");
                sql.Append("        		select [GUID] from TSQJD       \n");
                sql.Append("        	)b on a.[GUID] = b.[GUID]       \n");
                sql.Append("        	where vcChange in ('11')       \n");
                sql.Append("        	and vcIsDYJG = 1       \n");
                sql.Append("        )b       \n");
                sql.Append("        on a.vcPart_id = b.vcPart_id       \n");
                sql.Append("        and a.vcSupplier_id = b.vcSupplier_id       \n");
                #endregion
                #endregion

                #region 供应商变更-新设
                #region 根据品番织入TFTM调整日期
                sql.Append("        update TUnit set        \n");
                sql.Append("         dGYSTimeFrom = b.dTFTM_BJ       \n");
                sql.Append("        ,dGYSTimeTo = '9999/12/31'       \n");
                sql.Append("        from TUnit a       \n");
                sql.Append("        inner join       \n");
                sql.Append("        (       \n");
                sql.Append("        	select a.* from #TSQJD_temp  a     \n");
                sql.Append("        	inner join       \n");
                sql.Append("        	(       \n");
                sql.Append("        		select [GUID] from TSQJD       \n");
                sql.Append("        	)b on a.[GUID] = b.[GUID]       \n");
                sql.Append("        	where vcChange in ('10')       \n");
                sql.Append("        	and vcIsDYJG = 1       \n");
                sql.Append("        )b       \n");
                sql.Append("        on a.vcPart_id = b.vcPart_id       \n");
                sql.Append("        and a.vcSupplier_id = b.vcSupplier_id       \n");
                #endregion
                #region 根据品番和供应商代码织入生产地和出荷地
                sql.Append("        update TUnit set        \n");
                sql.Append("         vcSQState = '2'       \n");
                sql.Append("        ,vcSQContent = 'OK'       \n");
                sql.Append("        ,vcSCSName = b.vcSCSName       \n");
                sql.Append("        ,vcSCSAdress = b.vcSCSPlace       \n");
                sql.Append("        ,vcSCPlace = b.vcSCPlace_City       \n");
                sql.Append("        ,vcCHPlace = b.vcCHPlace_City       \n");
                sql.Append("        ,vcZXBZNo = b.vcZXBZNo       \n");
                sql.Append("        from TUnit a       \n");
                sql.Append("        inner join       \n");
                sql.Append("        (       \n");
                sql.Append("        	select a.* from #TSQJD_temp a      \n");
                sql.Append("        	inner join       \n");
                sql.Append("        	(       \n");
                sql.Append("        		select [GUID] from TSQJD       \n");
                sql.Append("        	)b on a.[GUID] = b.[GUID]       \n");
                sql.Append("        	where vcChange in ('10')       \n");
                sql.Append("        	and vcIsDYJG = 1       \n");
                sql.Append("        )b       \n");
                sql.Append("        on a.vcPart_id = b.vcPart_id       \n");
                sql.Append("        and a.vcSupplier_id = b.vcSupplier_id       \n");
                #endregion
                #endregion
                #endregion

                #region 对应不可时，将生确状态改为NG
                sql.Append("        update TUnit set        \n");
                sql.Append("        vcSQState = '3'       \n");
                sql.Append("        ,vcSQContent = 'NG'       \n");
                sql.Append("        from TUnit a       \n");
                sql.Append("        inner join        \n");
                sql.Append("        (       \n");
                sql.Append("        	select a.vcPart_id from #TSQJD_temp a      \n");
                sql.Append("        	inner join       \n");
                sql.Append("        	(       \n");
                sql.Append("        		select [GUID] from TSQJD       \n");
                sql.Append("        	)b on a.[GUID] = b.[GUID]       \n");
                sql.Append("        	where vcIsDYJG = '2'       \n");
                sql.Append("        ) b       \n");
                sql.Append("        on a.vcPart_id = b.vcPart_id       \n");
                #endregion

                #region 更新生确表中的数据
                sql.Append("          update TSQJD set           \n");
                sql.Append("           vcJD = '4'          \n");
                sql.Append("          ,dTFTM_BJ = b.dTFTM_BJ          \n");
                sql.Append("          ,vcOperatorId = '" + strUserId + "'          \n");
                sql.Append("          ,dOperatorTime = GETDATE()          \n");
                sql.Append("          from TSQJD a           \n");
                sql.Append("          inner join #TSQJD_temp b          \n");
                sql.Append("          on a.[GUID] = b.[GUID]          \n");
                #endregion

                if (sql.Length>0)
                {
                    excute.ExcuteSqlWithStringOper(sql.ToString(), "TK");
                }
            }
            catch (Exception ex)
            {
                strErr += ex.Message.ToString();
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

    }
}
