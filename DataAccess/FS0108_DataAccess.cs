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
    public class FS0108_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        public DataTable GetWorkAreaBySupplier(string supplierCode)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("  select  vcWorkArea as vcValue,vcWorkArea as vcName  from (   ");
                strSql.AppendLine("  select distinct  isnull (vcWorkArea,'无') as vcWorkArea from  (select vcValue1 as vcSupplier_id,vcValue2 as vcWorkArea from TOutCode where vcCodeId='C010' and  and vcValue1='" + supplierCode + "' vcIsColum='0') c  ");
                strSql.AppendLine("  ) a  order by a.vcWorkArea asc   ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetSupplier()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("   select  vcSupplier_id as vcValue,vcSupplier_id as vcName  from (   ");
                strSql.AppendLine("   select distinct  isnull (vcSupplier_id,'无') as vcSupplier_id from (select vcValue1 as vcSupplier_id,vcValue2 as vcWorkArea from TOutCode where vcCodeId='C010' and  vcIsColum='0') c     ");
                strSql.AppendLine("   ) a  order by a.vcSupplier_id asc   ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetWorkArea()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("  select  vcWorkArea as vcValue,vcWorkArea as vcName  from (   ");
                strSql.AppendLine("  select distinct  isnull (vcWorkArea,'无') as vcWorkArea from (select vcValue1 as vcSupplier_id,vcValue2 as vcWorkArea from TOutCode where vcCodeId='C010' and  vcIsColum='0') c     ");
                strSql.AppendLine("  ) a  order by a.vcWorkArea asc   ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable checkData(string vcSupplier, string vcWorkArea, string vcStart, string vcEnd, string strInAutoIds)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("   select * from TOutCode where    ");
                strSql.AppendLine("   vcCodeId='C010' and  vcIsColum='0'    ");
                strSql.AppendLine("   and (( replace(vcValue3,'-','')<'"+ vcStart.Replace("/","").Replace("-","")+ "' and replace(vcValue4,'-','')>'" + vcStart.Replace("/", "").Replace("-", "") + "')    ");
                strSql.AppendLine("   or ( replace(vcValue3,'-','')<'" + vcEnd.Replace("/", "").Replace("-", "") + "' and replace(vcValue4,'-','')>'" + vcEnd.Replace("/", "").Replace("-", "") + "'))   ");
                strSql.AppendLine("   and vcValue1='"+ vcSupplier + "' and vcValue2='" + vcWorkArea + "'   ");
                strSql.AppendLine("   and iAutoId not in ("+ strInAutoIds + ")   ");

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
        public DataTable Search(string vcValue1, string vcValue2, string vcValue5)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("   select iAutoId, vcCodeId, vcCodeName, vcIsColum, vcValue1, vcValue2, vcValue3, vcValue4, b.vcName as vcValue5,'0' as vcModFlag,'0' as vcAddFlag From  (select * from [dbo].TOutCode where vcCodeId='C010' and  vcIsColum='0') a     ");
                strSql.AppendLine("   left join (select vcValue,vcName from TCode where vcCodeId='C000') b on a.vcValue5 = b.vcValue  where 1=1  ");
                if (vcValue1.Length>0)
                {
                    strSql.AppendLine("  and  vcValue1='" + vcValue1 + "' ");
                }
                if (vcValue2.Length > 0)
                {
                    strSql.AppendLine("  and  vcValue2  = '" + vcValue2 + "' ");
                }
                if (vcValue5.Length > 0)
                {
                    strSql.AppendLine("  and  vcValue5  = '" + vcValue5 + "' ");
                }
                strSql.AppendLine("  order by  a.iAutoId desc ");
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
                        sql.Append("insert into [TOutCode] (vcCodeId, vcCodeName, vcIsColum, vcValue1, vcValue2, vcValue3, vcValue4, vcValue5, vcOperatorID, dOperatorTime)  \n");
                        sql.Append(" values (  \r\n");
                        sql.Append("'C010', '发注工场与供应商关系','0',  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcValue1"], true) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcValue2"], true) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcValue3"], true) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcValue4"], true) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcValue5"], true) + ",  \r\n");
                        sql.Append("   '" + userId + "', GETDATE() \r\n");
                        sql.Append(" );  \r\n");
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                        string start = listInfoData[i]["vcValue3"] == null ? null : Convert.ToDateTime(listInfoData[i]["vcValue3"].ToString()).ToString("yyyy-MM-dd");
                        string end = listInfoData[i]["vcValue4"] == null ? null : Convert.ToDateTime(listInfoData[i]["vcValue4"].ToString()).ToString("yyyy-MM-dd");
                        sql.Append("  update TOutCode set    \r\n");
                        //sql.Append("  vcValue1=" + getSqlValue(listInfoData[i]["vcValue1"], true) + ",   \r\n");
                        //sql.Append("  vcValue2=" + getSqlValue(listInfoData[i]["vcValue2"], true) + " ,  \r\n");
                        sql.Append("  vcValue3=" + getSqlValue(start, true) + " ,  \r\n");
                        sql.Append("  vcValue4=" + getSqlValue(end, true) + "  , \r\n");
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
