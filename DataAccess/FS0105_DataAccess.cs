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
        /// 检索数据
        /// </summary>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        public DataTable Search(string typeCode)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("   select iAutoId, [vcCodeId] ,[vcCodeName]  ,[vcValue] ,[vcName] ,[vcMeaning],'0' as vcModFlag,'0' as vcAddFlag From [dbo].[TCode] where 1=1   ");

                if (typeCode.Length>0)
                {
                    strSql.AppendLine("  and  vcCodeId='" + typeCode+"' ");
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
                sql.Append("  delete [TCode] where iAutoId in(   \r\n ");
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
                        sql.Append("insert into [TCode] (vcCodeId ,vcCodeName  ,vcValue ,vcName ,vcMeaning,vcOperatorID,dOperatorTime)  \n");
                        sql.Append(" values (  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcCodeId"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcCodeName"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcValue"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcName"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcMeaning"], false) + ",  \r\n");
                        sql.Append("   '" + userId + "', GETDATE() \r\n");
                        sql.Append(" );  \r\n");
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);

                        sql.Append("  update TCode set    \r\n");
                        sql.Append("  vcName=" + getSqlValue(listInfoData[i]["vcName"], false) + "   \r\n");
                        sql.Append("  ,vcMeaning=" + getSqlValue(listInfoData[i]["vcMeaning"], false) + "   \r\n");
                        sql.Append("  ,vcOperatorID='" + userId + "',dOperatorTime=GETDATE() \r\n");
                        sql.Append(" where iAutoId=" + iAutoId + " ;  \n");
                        
                    }
                }
               
                if (sql.Length > 0)
                {
                    //以下追加验证数据库中是否存在品番区间重叠判断，如果存在则终止提交
                    sql.Append("  DECLARE @errorPart varchar(50)   \r\n");
                    sql.Append("  set @errorPart=''   \r\n");
                    sql.Append("  set @errorPart=(   \r\n");
                    sql.Append("  	select a.vcCodeId+';' from   \r\n");
                    sql.Append("  	(   \r\n");
                    sql.Append("  		select distinct a.vcCodeId,a.vcValue from [TCode] a   \r\n");
                    sql.Append("  		left join   \r\n");
                    sql.Append("  		(   \r\n");
                    sql.Append("  		   select * from [TCode]   \r\n");
                    sql.Append("  		)b on a.vcCodeId=b.vcCodeId  and a.vcValue=b.vcValue and a.iAutoId<>b.iAutoId   \r\n");
                    sql.Append("  		where b.iAutoId is not null   \r\n");
                    sql.Append("  	)a for xml path('')   \r\n");
                    sql.Append("  )   \r\n");
                    sql.Append("      \r\n");
                    sql.Append("  if @errorPart<>''   \r\n");
                    sql.Append("  begin   \r\n");
                    sql.Append("    select CONVERT(varchar(50),'-->'+@errorPart+'<--')   \r\n");
                    sql.Append("  end    \r\n");


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
