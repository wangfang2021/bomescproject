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
    public class FS0623_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        
        /// <summary>
        /// 子页面 订单区分检索
        /// </summary>
        /// <returns></returns>
        public DataTable Search_Sub()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("   select [iAutoId], [vcOrderDifferentiation], [vcOrderInitials], [vcOperatorID], [dOperatorTime],'0' as vcModFlag,'0' as vcAddFlag from  TOrderDifferentiation   ");

                strSql.AppendLine("  order by  iAutoId,vcOrderInitials  asc ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        

        /// <summary>
        /// 子页面验证重复项
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
                    if (strSql.Length > 0)/*vcOrderDifferentiation='" + dtadd.Rows[i]["vcOrderDifferentiation"] + "' or*/
                    {
                        strSql.AppendLine("  union all SELECT * FROM [dbo].[TOrderDifferentiation] where  vcOrderInitials='" + dtadd.Rows[i]["vcOrderInitials"] + "'  ");
                    }
                    else
                    {
                        strSql.AppendLine("  SELECT * FROM [dbo].[TOrderDifferentiation] where  vcOrderInitials='" + dtadd.Rows[i]["vcOrderInitials"] + "'  ");
                    }
                }
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public DataTable CheckDistinctByTableOrderGoods(DataTable dtadd)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                for (int i = 0; i < dtadd.Rows.Count; i++)
                {
                    if (strSql.Length > 0)
                    {
                        strSql.AppendLine("  union all SELECT * FROM [dbo].[TOrderGoods] where vcOrderGoods='"+ dtadd.Rows[i]["vcOrderGoods"] + "'  ");
                    }
                    else
                    {
                        strSql.AppendLine("  SELECT * FROM [dbo].[TOrderGoods] where vcOrderGoods='"+ dtadd.Rows[i]["vcOrderGoods"] + "'  ");
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
        /// 子页面保存
        /// </summary>
        /// <param name="listInfoData"></param>
        /// <param name="userId"></param>
        /// <param name="strErrorName"></param>
        public void Save_Sub(List<Dictionary<string, object>> listInfoData, string userId, string strErrorName)
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
                        sql.Append("insert into [TOrderDifferentiation] ([vcOrderDifferentiation], [vcOrderInitials], [vcOperatorID], [dOperatorTime])  \n");
                        sql.Append(" values (  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcOrderDifferentiation"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcOrderInitials"].ToString().ToUpper(), false) + ",  \r\n");
                        sql.Append("   '" + userId + "', GETDATE() \r\n");
                        sql.Append(" );  \r\n");
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);

                        sql.Append("  update TOrderDifferentiation set    \r\n");
                        sql.Append("  vcOrderInitials=" + getSqlValue(listInfoData[i]["vcOrderInitials"].ToString().ToUpper(), false) + "   \r\n");
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
                    sql.Append("  	select a.vcOrderDifferentiation+';' from   \r\n");
                    sql.Append("  	(   \r\n");
                    sql.Append("  		select distinct a.vcOrderDifferentiation from [TOrderDifferentiation] a   \r\n");
                    sql.Append("  		left join   \r\n");
                    sql.Append("  		(   \r\n");
                    sql.Append("  		   select * from [TOrderDifferentiation]   \r\n");
                    sql.Append("  		)b on a.vcOrderDifferentiation=b.vcOrderDifferentiation   and a.iAutoId<>b.iAutoId   \r\n");
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
        /// 子页面删除
        /// </summary>
        /// <param name="listInfoData"></param>
        /// <param name="userId"></param>
        public void Del_Sub(List<Dictionary<string, object>> listInfoData, string userId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("  delete [TOrderDifferentiation] where iAutoId in(   \r\n ");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (i != 0)
                        sql.Append(",");
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                    sql.Append(iAutoId);
                }
                sql.Append("  )   \r\n ");

                sql.Append("  delete [TOrderGoodsAndDifferentiation] where vcOrderDifferentiationId in(   \r\n ");
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
        /// 检索数据
        /// </summary>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        public DataSet Search()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("   select  [iAutoId], vcName as dhfs, '0' as show,'0' as vcModFlag,'0' as vcAddFlag from (select iAutoId, vcValue,vcName from TCode where  vcCodeId='C047') b;    ");
                strSql.AppendLine("   select  [vcOrderDifferentiation] from [dbo].[TOrderDifferentiation] order by iAutoId asc;    ");
                strSql.AppendLine("   select  b.vcOrderGoods,c.vcOrderDifferentiation from [dbo].[TOrderGoodsAndDifferentiation] a    ");
                strSql.AppendLine("   left join (select iAutoId,vcName as vcOrderGoods  from TCode where  vcCodeId='C047') b on a.vcOrderGoodsId=b.iAutoId    ");
                strSql.AppendLine("   left join [dbo].[TOrderDifferentiation] c on a.vcOrderDifferentiationId=c.iAutoId ;   ");
                strSql.AppendLine("   select  vcOrderDifferentiation as prop,vcOrderDifferentiation as label,vcOrderInitials as ddszm from [dbo].[TOrderDifferentiation] order by iAutoId asc    ");

                return excute.ExcuteSqlWithSelectToDS(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        /// <summary>
        /// 获取订单区分
        /// </summary>
        /// <returns></returns>
        public DataTable GetOrderDifferentiation()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("   Select iAutoId,vcOrderDifferentiation from [dbo].[TOrderDifferentiation]    ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 增加订单方式
        /// </summary>
        /// <param name="dtadd"></param>
        /// <returns></returns>
        public bool AddOrderGoods(DataTable dtadd,String userId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < dtadd.Rows.Count; i++)
                {
                    DataRow dr = dtadd.Rows[i];
                    sql.Append(" insert into [dbo].[TOrderGoods] (vcOrderGoods,vcOperatorID,dOperatorTime)  \n");
                    sql.Append(" values('" + dr["vcOrderGoods"].ToString() + "','" + userId + "',GETDATE()) \n");
                }
               
                if (sql.Length > 0)
                {
                    return excute.ExcuteSqlWithStringOper(sql.ToString())>0;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 增加中间表的数据
        /// </summary>
        /// <param name="dtaddZJB"></param>
        /// <param name="userId"></param>
        public void AddOrderGoodsAndDifferentiation(DataTable dtaddZJB, string userId)
        {

            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < dtaddZJB.Rows.Count; i++)
                {
                    DataRow dr = dtaddZJB.Rows[i];
                    sql.Append("insert into [dbo].[TOrderGoodsAndDifferentiation] (vcOrderGoodsId,vcOrderDifferentiationId,vcOperatorID,dOperatorTime)  \n");
                    sql.Append(" values((select iAutoId from [dbo].[TOrderGoods] where vcOrderGoods='" + dr["vcOrderGoods"].ToString() + "' ),(select iAutoId from [dbo].[TOrderDifferentiation] where vcOrderDifferentiation='" + dr["vcOrderDifferentiation"].ToString() + "'),'000000',getdate()); \n");
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

        public void UpdateOrderGoodsAndDifferentiation(DataTable dtamodify, DataTable dtamodifyZJB, string userId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < dtamodify.Rows.Count; i++)
                {
                    DataRow dr = dtamodify.Rows[i];
                    sql.Append("delete from [dbo].[TOrderGoodsAndDifferentiation] where vcOrderGoodsId=(select iAutoId from TCode where vcCodeId='C047' and vcName='" + dr["vcOrderGoods"].ToString() + "');  \n");
                }
                for (int i = 0; i < dtamodifyZJB.Rows.Count; i++)
                {
                    DataRow dr = dtamodifyZJB.Rows[i];
                    sql.Append("insert into [dbo].[TOrderGoodsAndDifferentiation] (vcOrderGoodsId,vcOrderDifferentiationId,vcOperatorID,dOperatorTime)  \n");
                    sql.Append(" values((select iAutoId from TCode where vcCodeId='C047' and vcName='" + dr["vcOrderGoods"].ToString() + "' ),(select iAutoId from [dbo].[TOrderDifferentiation] where vcOrderDifferentiation='" + dr["vcOrderDifferentiation"].ToString() + "'),'"+ userId + "',getdate()); \n");
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
        /// 子页面删除
        /// </summary>
        /// <param name="listInfoData"></param>
        /// <param name="userId"></param>
        public void Del(List<Dictionary<string, object>> listInfoData, string userId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("  delete [TOrderGoods] where iAutoId in(   \r\n ");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (i != 0)
                        sql.Append(",");
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                    sql.Append(iAutoId);
                }
                sql.Append("  );   \r\n ");
                sql.Append("  delete [TOrderGoodsAndDifferentiation] where vcOrderDifferentiationId in(   \r\n ");
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
