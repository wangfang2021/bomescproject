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
                strSql.Append("     ,'0' as vcSCSNameModFlag,'0' as vcSCSPlaceModFlag    \n");
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
                if (!string.IsNullOrEmpty(strJD))
                {
                    strSql.Append("      and vcJD = '" + strJD + "'  ");
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
                strSql.Append("     ,'0' as vcSCSNameModFlag,'0' as vcSCSPlaceModFlag    \n");
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

        #region 一括付与
        public void SetFY(List<Dictionary<string, Object>> listInfoData, string strSupplier_BJ, string strSupplier_HK, string strUserId, ref string strErr)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);

                    sql.AppendLine("      update TSQJD set    \n");
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
                    sql.AppendLine("       ,vcOperatorId = '" + strUserId + "'      \n");
                    sql.AppendLine("       ,dOperatorTime = GETDATE()      \n");
                    sql.AppendLine("      where iAutoId = '" + iAutoId + "'      \n");
                    sql.AppendLine("       and vcJD in ('1','3')         ");
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
