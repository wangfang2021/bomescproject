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
    public class FS0620_DataAccess
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
        /// 检索数据
        /// </summary>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        public DataTable Search(string vcTargetYear, string vcPartNo, string vcInjectionFactory, string vcInsideOutsideType, string vcSupplier_id, string vcWorkArea, string vcCarType)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("  select [iAutoId], [vcPackPlant], [vcTargetYear], [vcPartNo], [vcInjectionFactory],   ");
                strSql.AppendLine("  [vcInsideOutsideType], [vcSupplier_id], [vcWorkArea], [vcCarType], [vcAcceptNum],  ");
                strSql.AppendLine("  [vcJanuary], [vcFebruary], [vcMarch], [vcApril], [vcMay], [vcJune], [vcJuly],   ");
                strSql.AppendLine("  [vcAugust], [vcSeptember], [vcOctober], [vcNovember], [vcDecember],   ");

                strSql.AppendLine("  cast(a.vcJanuary as decimal(18,6))+cast(a.[vcFebruary] as decimal(18,6))+cast(a.vcMarch as decimal(18,6))  ");
                strSql.AppendLine("  +cast(a.vcApril as decimal(18,6))+cast(a.vcMay as decimal(18,6))+cast(a.vcJune as decimal(18,6))  ");
                strSql.AppendLine("  +cast(a.vcJuly as decimal(18,6))+cast(a.vcAugust as decimal(18,6))+cast(a.vcSeptember as decimal(18,6))  ");
                strSql.AppendLine("  +cast(a.vcOctober as decimal(18,6))+cast(a.vcNovember as decimal(18,6))+cast(a.vcDecember as decimal(18,6))as vcSum,  ");

                strSql.AppendLine("  [vcNextOneYear],[vcNextTwoYear], [vcOperatorID], [dOperatorTime],'0' as vcModFlag,'0' as vcAddFlag   ");
                strSql.AppendLine("  from TAnnualManagement a  ");
                //strSql.AppendLine("  left join (select vcValue,vcName from [TCode] where vcCodeId='C000') b on a.vcInjectionFactory=b.vcValue  ");
                strSql.AppendLine("  left join (select vcValue,vcName from [TCode] where vcCodeId='C003') c on a.vcInsideOutsideType=c.vcName  ");
                strSql.AppendLine("  where 1=1  ");
                strSql.AppendLine("    ");

                if (vcTargetYear.Length > 0)
                {
                    strSql.AppendLine("  and  vcTargetYear = '" + vcTargetYear + "' ");
                }
                if (vcPartNo.Length > 0)
                {
                    strSql.AppendLine("  and  vcPartNo = '" + vcPartNo + "' ");
                }
                if (vcInjectionFactory.Length > 0)
                {
                    strSql.AppendLine("  and  vcInjectionFactory = '" + vcInjectionFactory + "' ");
                }
                if (vcInsideOutsideType.Length > 0)
                {
                    strSql.AppendLine("  and  c.vcValue = '" + vcInsideOutsideType + "' ");
                }
                if (vcSupplier_id.Length > 0)
                {
                    strSql.AppendLine("  and  vcSupplier_id like '%" + vcSupplier_id + "%' ");
                }
                if (vcWorkArea.Length > 0)
                {
                    strSql.AppendLine("  and  vcWorkArea = '" + vcWorkArea + "' ");
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

        public void importSave(DataTable dt, object strUserId)
        {
            try
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    StringBuilder strSql = new StringBuilder();
                    
                    string vcPackPlant = dt.Rows[i]["vcPackPlant"] == System.DBNull.Value ? "" : dt.Rows[i]["vcPackPlant"].ToString();
                    string vcTargetYear = dt.Rows[i]["vcTargetYear"] == System.DBNull.Value ? "" : dt.Rows[i]["vcTargetYear"].ToString();
                    string vcPartNo = dt.Rows[i]["vcPartNo"] == System.DBNull.Value ? "" : dt.Rows[i]["vcPartNo"].ToString();
                    string vcInjectionFactory = dt.Rows[i]["vcInjectionFactory"] == System.DBNull.Value ? "" : dt.Rows[i]["vcInjectionFactory"].ToString();
                    string vcInsideOutsideType = dt.Rows[i]["vcInsideOutsideType"] == System.DBNull.Value ? "" : dt.Rows[i]["vcInsideOutsideType"].ToString();
                    string vcSupplier_id = dt.Rows[i]["vcSupplier_id"] == System.DBNull.Value ? "" : dt.Rows[i]["vcSupplier_id"].ToString();
                    string vcWorkArea = dt.Rows[i]["vcWorkArea"] == System.DBNull.Value ? "" : dt.Rows[i]["vcWorkArea"].ToString();
                    string vcCarType = dt.Rows[i]["vcCarType"] == System.DBNull.Value ? "" : dt.Rows[i]["vcCarType"].ToString();
                    string vcAcceptNum = dt.Rows[i]["vcAcceptNum"] == System.DBNull.Value ? "" : dt.Rows[i]["vcAcceptNum"].ToString();
                    string vcJanuary = dt.Rows[i]["vcJanuary"] == System.DBNull.Value ? "" : dt.Rows[i]["vcJanuary"].ToString();
                    string vcFebruary = dt.Rows[i]["vcFebruary"] == System.DBNull.Value ? "" : dt.Rows[i]["vcFebruary"].ToString();
                    string vcMarch = dt.Rows[i]["vcMarch"] == System.DBNull.Value ? "" : dt.Rows[i]["vcMarch"].ToString();
                    string vcApril = dt.Rows[i]["vcApril"] == System.DBNull.Value ? "" : dt.Rows[i]["vcApril"].ToString();
                    string vcMay = dt.Rows[i]["vcMay"] == System.DBNull.Value ? "" : dt.Rows[i]["vcMay"].ToString();
                    string vcJune = dt.Rows[i]["vcJune"] == System.DBNull.Value ? "" : dt.Rows[i]["vcJune"].ToString();
                    string vcJuly = dt.Rows[i]["vcJuly"] == System.DBNull.Value ? "" : dt.Rows[i]["vcJuly"].ToString();
                    string vcAugust = dt.Rows[i]["vcAugust"] == System.DBNull.Value ? "" : dt.Rows[i]["vcAugust"].ToString();
                    string vcSeptember = dt.Rows[i]["vcSeptember"] == System.DBNull.Value ? "" : dt.Rows[i]["vcSeptember"].ToString();
                    string vcOctober = dt.Rows[i]["vcOctober"] == System.DBNull.Value ? "" : dt.Rows[i]["vcOctober"].ToString();
                    string vcNovember = dt.Rows[i]["vcNovember"] == System.DBNull.Value ? "" : dt.Rows[i]["vcNovember"].ToString();
                    string vcDecember = dt.Rows[i]["vcDecember"] == System.DBNull.Value ? "" : dt.Rows[i]["vcDecember"].ToString();
                    string vcNextOneYear = dt.Rows[i]["vcNextOneYear"] == System.DBNull.Value ? "" : dt.Rows[i]["vcNextOneYear"].ToString();
                    string vcNextTwoYear = dt.Rows[i]["vcNextTwoYear"] == System.DBNull.Value ? "" : dt.Rows[i]["vcNextTwoYear"].ToString();
                    
                    strSql.AppendLine("  		insert into dbo.TAnnualManagement (vcPackPlant, vcTargetYear, vcPartNo, vcInjectionFactory,  ");
                    strSql.AppendLine("  		 vcInsideOutsideType, vcSupplier_id, vcWorkArea, vcCarType,   ");
                    strSql.AppendLine("  		 vcAcceptNum, vcJanuary, vcFebruary, vcMarch, vcApril, vcMay, vcJune,   ");
                    strSql.AppendLine("  		 vcJuly, vcAugust, vcSeptember, vcOctober, vcNovember, vcDecember,    ");
                    strSql.AppendLine("  		 vcNextOneYear, vcNextTwoYear, vcOperatorID, dOperatorTime   ");
                    strSql.AppendLine("  		values   ");
                    strSql.AppendLine("  		('" + vcPackPlant + "','" + vcTargetYear + "','" + vcPartNo + "','" + vcInjectionFactory + "','" + vcInsideOutsideType + "',    ");
                    strSql.AppendLine("  		('" + vcSupplier_id + "','" + vcWorkArea + "','" + vcCarType + "','" + vcAcceptNum + "','" + vcJanuary + "',   ");
                    strSql.AppendLine("  		('" + vcFebruary + "','" + vcMarch + "','" + vcApril + "','" + vcMay + "','" + vcJune + "',   ");
                    strSql.AppendLine("  		('" + vcJuly + "','" + vcAugust + "','" + vcSeptember + "','" + vcOctober + "','" + vcNovember + "',   ");
                    strSql.AppendLine("  		('" + vcDecember + "','" + vcNextOneYear + "','" + vcNextTwoYear + "','" + strUserId + "',GETDATE()) ;   ");

                    excute.ExcuteSqlWithStringOper(strSql.ToString());
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
