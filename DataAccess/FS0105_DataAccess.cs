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
    public class FS0105_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        public DataTable BindConst()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("   select distinct vcCodeId,vcCodeName From [dbo].[TCode]    ");
               
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 验证新增的数据是否已经存在数据库
        /// </summary>
        /// <param name="dtadd"></param>
        /// <returns></returns>
        public bool isExistAddData(DataTable dtadd)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                for (int i = 0; i < dtadd.Rows.Count; i++)
                {
                    if (strSql.Length > 0)
                    {
                        strSql.AppendLine("  union all SELECT [vcCodeId] ,[vcCodeName]  ,[vcValue] ,[vcName] ,[vcMeaning] FROM [dbo].[TCode] where vcCodeId='" + dtadd.Rows[i]["vcCodeId"] + "' and vcValue='" + dtadd.Rows[i]["vcValue"] + "'  ");
                    }
                    else
                    {
                        strSql.AppendLine("  SELECT [vcCodeId] ,[vcCodeName]  ,[vcValue] ,[vcName] ,[vcMeaning] FROM [dbo].[TCode] where vcCodeId='" + dtadd.Rows[i]["vcCodeId"] + "' and vcValue='" + dtadd.Rows[i]["vcValue"] + "'  ");
                    }
                }
                DataTable dt= excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                return dt.Rows.Count > 0;
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
        public DataTable Search(string typeCode)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("   select  [vcCodeId] ,[vcCodeName]  ,[vcValue] ,[vcName] ,[vcMeaning],'1' as vcmodflag,'1' as vcaddflag From [dbo].[TCode] where 1=1   ");

                if (typeCode.Length>0)
                {
                    strSql.AppendLine("  and  vcCodeId='" + typeCode+"' ");
                }
                strSql.AppendLine("  order by  vcValue ");
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
        public void Del(DataTable dtdel, string userId)
        {
            StringBuilder sql = new StringBuilder();
            for (int i = 0; i < dtdel.Rows.Count; i++)
            {
                DataRow dr = dtdel.Rows[i];
                sql.Append("delete from [TCode]   \n");
                sql.Append("where vcCodeId='" + dr["vcCodeId"].ToString() + "' and vcValue='" + dr["vcValue"].ToString() + "'   \n");
                
            }
            if (sql.Length > 0)
            {
                excute.ExcuteSqlWithStringOper(sql.ToString());
            }
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="dtadd"></param>
        /// <param name="userId"></param>
        public void Save(DataTable dtadd, DataTable dtmod, string userId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < dtadd.Rows.Count; i++)
                {
                    DataRow dr = dtadd.Rows[i];
                    sql.Append("insert into [TCode] (vcCodeId ,vcCodeName  ,vcValue ,vcName ,vcMeaning,vcOperatorName,dOperatorTime)  \n");
                    sql.Append(" values('" + dr["vcCodeId"].ToString() + "','" + dr["vcCodeName"].ToString() + "','" + dr["vcValue"].ToString() + "','" + dr["vcName"].ToString() + "','" + dr["vcMeaning"].ToString() + "','" + userId + "',GETDATE()) \n");
                }
                for (int i = 0; i < dtmod.Rows.Count; i++)
                {
                    DataRow dr = dtmod.Rows[i];
                    sql.Append("update TCode set vcName='" + dr["vcName"].ToString() + "', vcMeaning='" + dr["vcMeaning"].ToString() + "',vcOperatorName='" + userId + "',dOperatorTime=GETDATE()  \n");
                    sql.Append("where vcCodeId='" + dr["vcCodeId"].ToString() + "' and vcValue='" + dr["vcValue"].ToString() + "'   \n");

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

        /// <summary>
        /// 验证修改的数据是否已经存在数据库
        /// </summary>
        /// <param name="dtamod"></param>
        /// <returns></returns>
        public bool isExistModData(DataTable dtamod)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                for (int i = 0; i < dtamod.Rows.Count; i++)
                {
                    if (strSql.Length > 0)
                    {
                        strSql.AppendLine("  union all SELECT [vcCodeId] ,[vcCodeName]  ,[vcValue] ,[vcName] ,[vcMeaning] FROM [dbo].[TCode] where vcCodeId='" + dtamod.Rows[i]["vcCodeId"] + "' and vcValue='" + dtamod.Rows[i]["vcValue"] + "'  ");
                    }
                    else
                    {
                        strSql.AppendLine("  SELECT [vcCodeId] ,[vcCodeName]  ,[vcValue] ,[vcName] ,[vcMeaning] FROM [dbo].[TCode] where vcCodeId='" + dtamod.Rows[i]["vcCodeId"] + "' and vcValue='" + dtamod.Rows[i]["vcValue"] + "'  ");
                    }
                }
                DataTable dt = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                return dt.Rows.Count > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
