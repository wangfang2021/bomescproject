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
    public class FS0627_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

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
                        strSql.AppendLine("  union all select iAutoId, vcSupplier_id, vcWorkArea, dBeginDate, dEndDate, vcOperatorID, dOperatorTime from [dbo].[TSpecialSupplier] where vcSupplier_id='" + dtadd.Rows[i]["vcSupplier_id"] + "' and  vcWorkArea='"+ dtadd.Rows[i]["vcWorkArea"] + "' ");
                    }
                    else
                    {
                        strSql.AppendLine("  select iAutoId, vcSupplier_id, vcWorkArea, dBeginDate, dEndDate, vcOperatorID, dOperatorTime from [dbo].[TSpecialSupplier] where vcSupplier_id='" + dtadd.Rows[i]["vcSupplier_id"] + "' and  vcWorkArea='" + dtadd.Rows[i]["vcWorkArea"] + "'  ");
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
        /// <summary>
        /// 绑定发注工厂
        /// </summary>
        /// <returns></returns>
        public DataTable bindInjectionFactoryApi()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("  select iFZGC as vcCodeId, iFZGC as vcCodeName from ");
                strSql.AppendLine("  ( ");
                strSql.AppendLine("  select distinct iFZGC as iFZGC from SP_M_SITEM ");
                strSql.AppendLine("  ) S ");
                DataTable dt = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                return dt;
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
        public DataSet Search(string vcInjectionFactory, string vcProject, string vcTargetYear)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("   select A.*,  ");
                strSql.AppendLine("   ([1月]+[2月]+[3月]+[4月]+[5月]+[6月]+[7月]+[8月]+[9月]+[10月]+[11月]+[12月]) as '合计'  ");
                strSql.AppendLine("   from (  ");
                strSql.AppendLine("   select * from [dbo].[VI_MonthSellDataManager] where 1=1  ");
                if (vcTargetYear.Length > 0)
                {
                    strSql.AppendLine("  and vcYear='"+ vcTargetYear + "'    ");
                }
                if (vcInjectionFactory.Length > 0)
                {
                    strSql.AppendLine("  and vcInjectionFactory='" + vcInjectionFactory + "'    ");
                }
                if (vcProject.Length > 0)
                {
                    strSql.AppendLine("  and vcSupplier_id='" + vcProject + "'    ");
                }
                strSql.AppendLine("   union all   ");
                strSql.AppendLine("  select '合计' as vcInjectionFactory,'' as vcSupplier_id,'' as vcYear,  ");
                strSql.AppendLine("  sum([1月]) as [1月], sum([2月]) as [2月], sum([3月]) as [3月],  ");
                strSql.AppendLine("  sum([4月]) as [4月], sum([5月]) as [5月], sum([6月]) as [6月],  ");
                strSql.AppendLine("  sum([7月]) as [7月], sum([8月]) as [8月], sum([9月]) as [9月],  ");
                strSql.AppendLine("  sum([10月]) as [10月],sum([11月]) as [11月], sum([12月]) as [12月]  ");
                strSql.AppendLine("  from [dbo].[VI_MonthSellDataManager] where 1=1  ");
                if (vcTargetYear.Length > 0)
                {
                    strSql.AppendLine("  and vcYear='" + vcTargetYear + "'    ");
                }
                if (vcInjectionFactory.Length > 0)
                {
                    strSql.AppendLine("  and vcInjectionFactory='" + vcInjectionFactory + "'    ");
                }
                if (vcProject.Length > 0)
                {
                    strSql.AppendLine("  and vcSupplier_id='" + vcProject + "'    ");
                }
                strSql.AppendLine("  ) A  ");
                strSql.AppendLine("  ;  ");

                strSql.AppendLine("   select A.*,  ");
                strSql.AppendLine("   ([1月]+[2月]+[3月]+[4月]+[5月]+[6月]+[7月]+[8月]+[9月]+[10月]+[11月]+[12月]) as '合计'  ");
                strSql.AppendLine("   from (  ");
                strSql.AppendLine("   select * from [dbo].[VI_MonthSellMoneyManager] where 1=1  ");
                if (vcTargetYear.Length > 0)
                {
                    strSql.AppendLine("  and vcYear='" + vcTargetYear + "'    ");
                }
                if (vcInjectionFactory.Length > 0)
                {
                    strSql.AppendLine("  and vcInjectionFactory='" + vcInjectionFactory + "'    ");
                }
                if (vcProject.Length > 0)
                {
                    strSql.AppendLine("  and vcSupplier_id='" + vcProject + "'    ");
                }
                strSql.AppendLine("   union all   ");
                strSql.AppendLine("  select '合计' as vcInjectionFactory,'' as vcSupplier_id,'' as vcYear,  ");
                strSql.AppendLine("  sum([1月]) as [1月], sum([2月]) as [2月], sum([3月]) as [3月],  ");
                strSql.AppendLine("  sum([4月]) as [4月], sum([5月]) as [5月], sum([6月]) as [6月],  ");
                strSql.AppendLine("  sum([7月]) as [7月], sum([8月]) as [8月], sum([9月]) as [9月],  ");
                strSql.AppendLine("  sum([10月]) as [10月],sum([11月]) as [11月], sum([12月]) as [12月]  ");
                strSql.AppendLine("  from [dbo].[VI_MonthSellMoneyManager] where 1=1  ");
                if (vcTargetYear.Length > 0)
                {
                    strSql.AppendLine("  and vcYear='" + vcTargetYear + "'    ");
                }
                if (vcInjectionFactory.Length > 0)
                {
                    strSql.AppendLine("  and vcInjectionFactory='" + vcInjectionFactory + "'    ");
                }
                if (vcProject.Length > 0)
                {
                    strSql.AppendLine("  and vcSupplier_id='" + vcProject + "'    ");
                }
                strSql.AppendLine("  ) A  ");
                strSql.AppendLine("  ;  ");
                return excute.ExcuteSqlWithSelectToDS(strSql.ToString());
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
                sql.Append("delete from [TSpecialSupplier]  \n");
                sql.Append("where vcSupplier_id='" + dr["vcSupplier_id"].ToString() + "'  and vcWorkArea='" + dr["vcWorkArea"].ToString() + "' \n");

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
                    sql.Append("insert into [TSpecialSupplier] (vcSupplier_id, vcWorkArea, dBeginDate, dEndDate, vcOperatorID, dOperatorTime)  \n");
                    sql.Append(" values('" + dr["vcSupplier_id"].ToString() + "','" + dr["vcWorkArea"].ToString() + "','" + dr["dBeginDate"].ToString() + "','" + dr["dEndDate"].ToString() + "','" + userId + "',GETDATE()) \n");
                }
                for (int i = 0; i < dtmod.Rows.Count; i++)
                {
                    DataRow dr = dtmod.Rows[i];
                    sql.Append("update TSpecialSupplier set dBeginDate='" + Convert.ToDateTime(dr["dBeginDate"].ToString()) + "', dEndDate='" + Convert.ToDateTime(dr["dEndDate"].ToString()) + "',vcOperatorID='" + userId + "',dOperatorTime=GETDATE()  \n");
                    sql.Append("where vcSupplier_id='" + dr["vcSupplier_id"].ToString() + "' and vcWorkArea ='" + dr["vcWorkArea"].ToString() + "' \n");

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
                        strSql.AppendLine("  union all select vcSupplier_id, vcWorkArea, vcIsSureFlag, vcLinkMan, vcPhone, vcEmail, vcOperatorID, dOperatorTime from dbo.TSupplierInfo where vcSupplier_id='" + dtamod.Rows[i]["vcSupplier_id"] + "'  ");
                    }
                    else
                    {
                        strSql.AppendLine("  select vcSupplier_id, vcWorkArea, vcIsSureFlag, vcLinkMan, vcPhone, vcEmail, vcOperatorID, dOperatorTime from dbo.TSupplierInfo where vcSupplier_id='" + dtamod.Rows[i]["vcSupplier_id"] + "'  ");
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
        /// <summary>
        /// 一括赋予
        /// </summary>
        /// <returns></returns>
        public void allInstall(DateTime dBeginDate,DateTime dEndDate,string userId) {
            try
            {
                StringBuilder sql = new StringBuilder();
                
                sql.Append("update TSpecialSupplier set dBeginDate='" + dBeginDate + "', dEndDate='" + dEndDate + "',vcOperatorID='" + userId + "',dOperatorTime=GETDATE()  \n");

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
    }
}
