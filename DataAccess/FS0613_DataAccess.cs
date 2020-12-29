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
    public class FS0613_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        /// <summary>
        /// 检索数据
        /// </summary>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        public DataTable Search(string vcDock, string vcCarType)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("   select iAutoId, vcDock, vcCarType ,dBeginDate,  dEndDate, '0' as vcModFlag,'0' as vcAddFlag,  vcMemo, vcOperatorID, dOperatorTime from [dbo].[TDockCar] where 1=1  ");

                if (vcDock.Length > 0)
                {
                    strSql.AppendLine("  and  vcDock like '%" + vcDock + "%' ");
                }
                if (vcCarType.Length > 0)
                {
                    strSql.AppendLine("  and  vcCarType = '" + vcCarType + "' ");
                }

                strSql.AppendLine("  order by  dOperatorTime desc ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 验证重复项
        /// </summary>
        /// <param name="dtadd"></param>
        /// <returns></returns>
        public DataTable CheckDistinctByTable(DataTable dtadd)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                for (int i = 0; i < dtadd.Rows.Count; i++)
                {
                    if (strSql.Length > 0) 
                    {
                        strSql.AppendLine("  union all SELECT * FROM [dbo].[TDockCar] where vcDock='" + dtadd.Rows[i]["vcDock"] + "' and vcCarType='" + dtadd.Rows[i]["vcCarType"] + "'  ");
                    }
                    else
                    {
                        strSql.AppendLine("  SELECT * FROM [dbo].[TDockCar] where vcDock='" + dtadd.Rows[i]["vcDock"] + "' and vcCarType='" + dtadd.Rows[i]["vcCarType"] + "'  ");
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
        /// 保存
        /// </summary>
        /// <param name="listInfoData"></param>
        /// <param name="userId"></param>
        /// <param name="strErrorPartId"></param>
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
                        sql.Append("insert into [TDockCar] (vcDock, vcCarType, dBeginDate, dEndDate,vcMemo, vcOperatorID, dOperatorTime)  \n");
                        sql.Append(" values (  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcDock"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcCarType"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["dBeginDate"], true) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["dEndDate"], true) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcMemo"], false) + ",  \r\n");
                        sql.Append("   '" + userId + "', GETDATE() \r\n");
                        sql.Append(" );  \r\n");
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);

                        sql.Append("  update TDockCar set    \r\n");
                        sql.Append("  dBeginDate=" + getSqlValue(listInfoData[i]["dBeginDate"], true) + "   \r\n");
                        sql.Append("  ,dEndDate=" + getSqlValue(listInfoData[i]["dEndDate"], true) + "   \r\n");
                        sql.Append("  ,vcMemo=" + getSqlValue(listInfoData[i]["vcMemo"], false) + "   \r\n");
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
                    sql.Append("  	select a.vcDock+';' from   \r\n");
                    sql.Append("  	(   \r\n");
                    sql.Append("  		select distinct a.vcDock from [TDockCar] a   \r\n");
                    sql.Append("  		left join   \r\n");
                    sql.Append("  		(   \r\n");
                    sql.Append("  		   select * from [TDockCar]   \r\n");
                    sql.Append("  		)b on a.vcDock=b.vcDock and a.vcCarType=b.vcCarType  and a.iAutoId<>b.iAutoId   \r\n");
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
                sql.Append("  delete [TDockCar] where iAutoId in(   \r\n ");
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
