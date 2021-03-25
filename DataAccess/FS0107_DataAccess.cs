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
    public class FS0107_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        public DataTable BindConst()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("   select distinct vcCodeId,vcCodeName From [dbo].TOutCode  where vcIsColum=0    ");
               
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 检索数据
        /// </summary>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        public DataTable Search(string typeCode,string vcValue1,string vcValue2)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("   select iAutoId, vcCodeId, vcCodeName, vcIsColum, vcValue1, vcValue2, vcValue3, vcValue4, vcValue5, vcValue6, vcValue7, vcValue8, vcValue9, vcValue10,'0' as vcModFlag,'0' as vcAddFlag From [dbo].TOutCode where vcIsColum=0 and 1=1     ");

                if (typeCode.Length>0)
                {
                    strSql.AppendLine("  and  vcCodeId='" + typeCode+"' ");
                }
                if (vcValue1.Length > 0)
                {
                    strSql.AppendLine("  and  vcValue1  like '" + vcValue1 + "%' ");
                }
                if (vcValue2.Length > 0)
                {
                    strSql.AppendLine("  and  vcValue2  like '" + vcValue2 + "%' ");
                }
                strSql.AppendLine("  order by  iAutoId desc ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable CheckDistinctByTable(DataTable dtadd)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                for (int i = 0; i < dtadd.Rows.Count; i++)
                {
                    if (strSql.Length > 0)
                    {
                        strSql.AppendLine("  union all SELECT vcCodeId ,vcCodeName  ,vcValue ,vcName ,vcMeaning FROM [dbo].[TCode] where vcCodeId='" + dtadd.Rows[i]["vcCodeId"] + "' and vcValue='" + dtadd.Rows[i]["vcValue"] + "'  ");
                    }
                    else
                    {
                        strSql.AppendLine("  SELECT vcCodeId ,vcCodeName  ,vcValue ,vcName ,vcMeaning FROM [dbo].[TCode] where vcCodeId='" + dtadd.Rows[i]["vcCodeId"] + "' and vcValue='" + dtadd.Rows[i]["vcValue"] + "'  ");
                    }
                }
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="dtdel"></param>
        /// <param name="userId"></param>
        public void Del(List<Dictionary<string, object>> listInfoData, string userId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("  delete [TOutCode] where iAutoId in(   \r\n ");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (i != 0)
                        sql.Append(",");
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                    sql.Append(iAutoId);
                }
                sql.Append("  )   \r\n ");
                excute.ExcuteSqlWithStringOper(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="dtadd"></param>
        /// <param name="userId"></param>

        public void Save(List<Dictionary<string, object>> listInfoData, string userId, ref string strErrorPartId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();

                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    if (bAddFlag == true)
                    {//新增
                        sql.Append("insert into [TOutCode] (vcCodeId, vcCodeName, vcIsColum, vcValue1, vcValue2, vcValue3, vcValue4, vcValue5, vcValue6, vcValue7, vcValue8, vcValue9, vcValue10, vcOperatorID, dOperatorTime)  \n");
                        sql.Append(" values (  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcCodeId"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcCodeName"], false) + ",0,  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcValue1"], true) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcValue2"], true) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcValue3"], true) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcValue4"], true) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcValue5"], true) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcValue6"], true) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcValue7"], true) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcValue8"], true) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcValue9"], true) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcValue10"], true) + ",  \r\n");
                        sql.Append("   '" + userId + "', GETDATE() \r\n");
                        sql.Append(" );  \r\n");
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);

                        sql.Append("  update TOutCode set    \r\n");
                        sql.Append("  vcValue1=" + getSqlValue(listInfoData[i]["vcValue1"], true) + ",   \r\n");
                        sql.Append("  vcValue2=" + getSqlValue(listInfoData[i]["vcValue2"], true) + " ,  \r\n");
                        sql.Append("  vcValue3=" + getSqlValue(listInfoData[i]["vcValue3"], true) + " ,  \r\n");
                        sql.Append("  vcValue4=" + getSqlValue(listInfoData[i]["vcValue4"], true) + "  , \r\n");
                        sql.Append("  vcValue5=" + getSqlValue(listInfoData[i]["vcValue5"], true) + "  , \r\n");
                        sql.Append("  vcValue6=" + getSqlValue(listInfoData[i]["vcValue6"], true) + "  , \r\n");
                        sql.Append("  vcValue7=" + getSqlValue(listInfoData[i]["vcValue7"], true) + "  , \r\n");
                        sql.Append("  vcValue8=" + getSqlValue(listInfoData[i]["vcValue8"], true) + "  , \r\n");
                        sql.Append("  vcValue9=" + getSqlValue(listInfoData[i]["vcValue9"], true) + "  , \r\n");
                        sql.Append("  vcValue10=" + getSqlValue(listInfoData[i]["vcValue10"], true) + "   \r\n");
                        sql.Append("  ,vcOperatorID='" + userId + "',dOperatorTime=GETDATE() \r\n");
                        sql.Append(" where iAutoId=" + iAutoId + " ;  \n");
                        
                    }
                }
               
                if (sql.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(sql.ToString());
                }
            }
            catch (Exception ex)
            {
                 throw ex;
            }
        }


        #region 返回insert语句值
        /// <summary>
        /// 返回insert语句值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="isObject">如果insert时间、金额或者其他对象类型数据，为true</param>
        /// <returns></returns>
        private string getSqlValue(Object obj, bool isObject)
        {
            if (obj == null)
                return "null";
            else if (obj.ToString().Trim() == "" && isObject)
                return "null";
            else
                return "'" + obj.ToString() + "'";
        }
        #endregion
    }
}
