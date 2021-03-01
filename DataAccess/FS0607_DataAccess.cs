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
    public class FS0607_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        
        /// <summary>
        /// 检索数据
        /// </summary>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        public DataTable Search(string vcSupplier_id,string vcWorkArea, string dBeginDate, string dEndDate, string vcMemo)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("   select iAutoId, vcSupplier_id, vcWorkArea,convert(varchar(10), dBeginDate,111) as dBeginDate, convert(varchar(10), dEndDate,111) as dEndDate,vcMemo, '0' as vcModFlag,'0' as vcAddFlag, vcOperatorID, dOperatorTime from [dbo].[TSpecialSupplier]  where 1=1  ");

                if (vcSupplier_id.Length > 0)
                {
                    strSql.AppendLine("  and  vcSupplier_id like '%" + vcSupplier_id + "%' ");
                }
                if (vcWorkArea.Length > 0)
                {
                    strSql.AppendLine("  and  vcWorkArea = '" + vcWorkArea + "' ");
                }
                if (dBeginDate.Length > 0)
                {
                    strSql.AppendLine("  and  convert(varchar(10), dBeginDate,112) = '" + dBeginDate.Replace("-", "").Replace("/", "") + "' ");
                }
                if (dEndDate.Length > 0)
                {
                    strSql.AppendLine("  and  convert(varchar(10), dEndDate,112) = '" + dEndDate.Replace("-", "").Replace("/", "") + "' ");
                }
                if (vcMemo.Length > 0)
                {
                    strSql.AppendLine("  and  vcMemo like '%" + vcMemo + "%' ");
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
                        strSql.AppendLine("  union all SELECT * FROM [dbo].[TSpecialSupplier] where vcSupplier_id='" + dtadd.Rows[i]["vcSupplier_id"] + "' and vcWorkArea='" + dtadd.Rows[i]["vcWorkArea"] + "'  ");
                    }
                    else
                    {
                        strSql.AppendLine("  SELECT * FROM [dbo].[TSpecialSupplier] where vcSupplier_id='" + dtadd.Rows[i]["vcSupplier_id"] + "' and vcWorkArea='" + dtadd.Rows[i]["vcWorkArea"] + "'  ");
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
                        sql.Append("insert into [TSpecialSupplier] ([vcSupplier_id], [vcWorkArea], [dBeginDate],[dEndDate],vcMemo,  [vcOperatorID], [dOperatorTime])  \n");
                        sql.Append(" values (  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcSupplier_id"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcWorkArea"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["dBeginDate"], true) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["dEndDate"], true) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcMemo"], true) + ",  \r\n");
                        sql.Append("   '" + userId + "', GETDATE() \r\n");
                        sql.Append(" );  \r\n");
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);

                        sql.Append("  update TSpecialSupplier set    \r\n");
                        sql.Append("  dBeginDate=" + getSqlValue(listInfoData[i]["dBeginDate"], true) + "   \r\n");
                        sql.Append("  ,dEndDate=" + getSqlValue(listInfoData[i]["dEndDate"], true) + "   \r\n");
                        sql.Append("  ,vcMemo=" + getSqlValue(listInfoData[i]["vcMemo"], true) + "   \r\n");
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
                    sql.Append("  	select a.vcSupplier_id+';' from   \r\n");
                    sql.Append("  	(   \r\n");
                    sql.Append("  		select distinct a.vcSupplier_id from [TSpecialSupplier] a   \r\n");
                    sql.Append("  		left join   \r\n");
                    sql.Append("  		(   \r\n");
                    sql.Append("  		   select * from [TSpecialSupplier]   \r\n");
                    sql.Append("  		)b on a.vcSupplier_id=b.vcSupplier_id and a.vcWorkArea=b.vcWorkArea  and a.iAutoId<>b.iAutoId   \r\n");
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
                sql.Append("  delete [TSpecialSupplier] where iAutoId in(   \r\n ");
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
        /// 一括赋予
        /// </summary>
        /// <returns></returns>
        public void allInstall(List<Dictionary<string, object>> listInfoData, DateTime dBeginDate, DateTime dEndDate, string userId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();

                sql.Append("update TSpecialSupplier set dBeginDate='" + dBeginDate + "', dEndDate='" + dEndDate + "',vcOperatorID='" + userId + "',dOperatorTime=GETDATE() where iAutoId in( \n");

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
        /// 导入 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="strUserId"></param>
        public void importSave(DataTable dt, object strUserId)
        {
            try
            {

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //[vcSupplier_id], [vcWorkArea], [dBeginDate], [dEndDate],, 

                    StringBuilder strSql = new StringBuilder();
                    string vcSupplier_id = dt.Rows[i]["vcSupplier_id"] == System.DBNull.Value ? "" : dt.Rows[i]["vcSupplier_id"].ToString();
                    string vcWorkArea = dt.Rows[i]["vcWorkArea"] == System.DBNull.Value ? "" : dt.Rows[i]["vcWorkArea"].ToString();
                    DateTime dBeginDate = Convert.ToDateTime(dt.Rows[i]["dBeginDate"].ToString());
                    DateTime dEndDate = Convert.ToDateTime(dt.Rows[i]["dEndDate"].ToString());
                    string vcMemo = dt.Rows[i]["vcMemo"].ToString();
                    strSql.AppendLine("  declare @isExist int =0;   ");
                    strSql.AppendLine("  select @isExist=COUNT(*) from TSpecialSupplier where vcSupplier_id='" + vcSupplier_id + "' and vcWorkArea='" + vcWorkArea + "' ");
                    strSql.AppendLine("     ");
                    strSql.AppendLine("  if @isExist>0   ");
                    strSql.AppendLine("  begin   ");
                    strSql.AppendLine("  		update TSpecialSupplier set dBeginDate = '" + dBeginDate + "',dEndDate='" + dEndDate + "',vcMemo='" + vcMemo + "',  ");
                    strSql.AppendLine("  		vcOperatorID='" + strUserId + "',dOperatorTime=GETDATE() where vcSupplier_id='" + vcSupplier_id + "' and vcWorkArea='" + vcWorkArea + "' ;  ");
                    strSql.AppendLine("  end   ");
                    strSql.AppendLine("  else   ");
                    strSql.AppendLine("  begin   ");
                    strSql.AppendLine("  		insert into dbo.TSpecialSupplier (vcSupplier_id, vcWorkArea, dBeginDate, dEndDate,vcMemo, ");
                    strSql.AppendLine("  		 vcOperatorID, dOperatorTime )    ");
                    strSql.AppendLine("  		values   ");
                    strSql.AppendLine("  		('" + vcSupplier_id + "','" + vcWorkArea + "','" + dBeginDate + "','" + dEndDate + "','" + vcMemo + "','" + strUserId + "',GETDATE()) ;   ");
                    strSql.AppendLine("  end ;  ");
                    strSql.AppendLine("     ");

                    excute.ExcuteSqlWithStringOper(strSql.ToString());
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
