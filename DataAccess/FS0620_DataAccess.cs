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
        ///  通过code 获取抄送者邮箱  vcMeaning='1' 启用中 默认为0 不启用
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public DataTable getCCEmail(string code)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("  select vcValue as displayName ,vcName as address from TCode where vcCodeId='C053' and vcMeaning='1' ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetPackPlant()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("  select a.vcPackPlant as vcValue,a.vcPackPlant as vcName from ( select distinct vcPackPlant as vcPackPlant from TAnnualManagement ) a order by vcPackPlant asc ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetPlant()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("  select vcInjectionFactory as vcValue,vcInjectionFactory as vcName from ( ");
                strSql.AppendLine("  select distinct vcInjectionFactory  from TAnnualManagement  ");
                strSql.AppendLine("  ) a order by vcInjectionFactory asc ");
                strSql.AppendLine("   ");
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
                strSql.AppendLine("  select vcWorkArea as vcValue,vcWorkArea as vcName from ( ");
                strSql.AppendLine("  select distinct isnull(vcWorkArea,'无') as vcWorkArea  from TAnnualManagement  ");
                strSql.AppendLine("  ) a order by vcWorkArea asc ");
                strSql.AppendLine("   ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetNeiWai()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("  select vcInsideOutsideType as vcValue,vcInsideOutsideType as vcName from ( ");
                strSql.AppendLine("  select distinct  ");
                strSql.AppendLine("  	case when  vcInsideOutsideType='0' then '内制' ");
                strSql.AppendLine("  	     when   vcInsideOutsideType='1' then '外注' ");
                strSql.AppendLine("  		 else vcInsideOutsideType end as vcInsideOutsideType ");
                strSql.AppendLine("  	  from TAnnualManagement  ");
                strSql.AppendLine("  ) a order by vcInsideOutsideType asc ");
                strSql.AppendLine("   ");
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
                strSql.AppendLine(" select vcSupplier_id as vcValue,vcSupplier_id as vcName from (  ");
                strSql.AppendLine(" select distinct vcSupplier_id  from TAnnualManagement   ");
                strSql.AppendLine(" ) a order by vcSupplier_id asc  ");
                strSql.AppendLine("   ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void updateEmailState(DataTable dtNewSupplierandWorkArea)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("  update  [TAnnualManagement] set vcEmailFlag=1 where iAutoId in(   \r\n ");
                for (int i = 0; i < dtNewSupplierandWorkArea.Rows.Count; i++)
                {
                    if (i != 0)
                        sql.Append(",");
                    int iAutoId = Convert.ToInt32(dtNewSupplierandWorkArea.Rows[i]["iAutoId"]);
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

        public DataTable getTCode(string codeId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine(" select vcValue2 as vcName from TOutCode where vcCodeId='" + codeId + "' and vcIsColum=0   ");
               
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="listInfoData"></param>
        public void Del(List<Dictionary<string, object>> listInfoData)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("  delete [TAnnualManagement] where iAutoId in(   \r\n ");
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

        public DataTable GetWorkAreaBySupplier(string vcSupplier_id)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("   select vcWorkArea as vcValue,vcWorkArea as vcName from (   ");
                strSql.AppendLine("   select distinct isnull(vcWorkArea,'无') as vcWorkArea  from TAnnualManagement where vcSupplier_id='" + vcSupplier_id + "'   ");
                strSql.AppendLine("   ) a order by vcWorkArea asc  ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetSupplierWorkArea()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("  select a.SupplierWorkArea as vcValue,a.SupplierWorkArea as vcName from ( select distinct vcSupplier_id+vcWorkArea as SupplierWorkArea from TAnnualManagement ) a order by SupplierWorkArea asc ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 通过供应商 工区 获取指定邮箱
        /// </summary>
        /// <param name="vcSupplier_id"></param>
        /// <param name="vcWorkArea"></param>
        /// <returns></returns>
        public DataTable getEmail(string vcSupplier_id, string vcWorkArea)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("  select  vcSupplier_id, vcWorkArea, vcLinkMan1, vcEmail1, vcLinkMan2, vcEmail2, vcLinkMan3,vcEmail3 from [dbo].[TSupplierInfo] where vcSupplier_id='" + vcSupplier_id + "' and vcWorkArea='"+ vcWorkArea + "'   ");
                 
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
        public DataTable Search(string dOperatorTime,  string vcTargetYear, string vcPartNo, string vcInjectionFactory, string vcInsideOutsideType, string vcSupplierId,string vcWorkArea, string vcType,string vcPackPlant,string vcReceiver)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("  select [iAutoId], d.vcName as vcType, a.vcReceiver,[vcPackPlant],cast([vcTargetYear] as int) as [vcTargetYear], [vcPartNo], [vcInjectionFactory],   ");
                strSql.AppendLine("  case when vcInsideOutsideType='0' then '内制' when vcInsideOutsideType='1' then '外注' else  vcInsideOutsideType end as [vcInsideOutsideType], [vcSupplier_id], [vcWorkArea], [vcCarType], cast(isnull(a.vcAcceptNum,0) as int) [vcAcceptNum],  ");
                strSql.AppendLine("  cast(isnull(a.vcJanuary,0) as decimal(18,2)) as  [vcJanuary],cast(isnull(a.vcFebruary,0) as decimal(18,2)) as [vcFebruary],cast(isnull(a.vcMarch,0) as decimal(18,2)) as [vcMarch],cast(isnull(a.vcApril,0) as decimal(18,2)) as [vcApril],cast(isnull(a.vcMay,0) as decimal(18,2)) as [vcMay],cast(isnull(a.vcJune,0) as decimal(18,2)) as [vcJune],cast(isnull(a.vcJuly,0) as decimal(18,2)) as [vcJuly],   ");
                strSql.AppendLine("  cast(isnull(a.vcAugust,0) as decimal(18,2)) as [vcAugust], cast(isnull(a.vcSeptember,0) as decimal(18,2)) as [vcSeptember],cast(isnull(a.vcOctober,0) as decimal(18,2)) as [vcOctober],cast(isnull(a.vcNovember,0) as decimal(18,2)) as [vcNovember],cast(isnull(a.vcDecember,0) as decimal(18,2)) as [vcDecember],   ");

                strSql.AppendLine("  cast(isnull(a.vcJanuary,0) as decimal(18,6))+cast(isnull(a.[vcFebruary],0) as decimal(18,6))+cast(isnull(a.vcMarch,0) as decimal(18,6))    ");
                strSql.AppendLine("  +cast(isnull(a.vcApril,0) as decimal(18,6))+cast(isnull(a.vcMay,0) as decimal(18,6))+cast(isnull(a.vcJune,0) as decimal(18,6))    ");
                strSql.AppendLine("  +cast(isnull(a.vcJuly,0) as decimal(18,6))+cast(isnull(a.vcAugust,0) as decimal(18,6))+cast(isnull(a.vcSeptember,0) as decimal(18,6))    ");
                strSql.AppendLine("  +cast(isnull(a.vcOctober,0) as decimal(18,6))+cast(isnull(a.vcNovember,0) as decimal(18,6))+cast(isnull(a.vcDecember,0) as decimal(18,6))as vcSum,    ");

                strSql.AppendLine("  cast(isnull(a.vcNextOneYear,0) as decimal(18,2)) as [vcNextOneYear],cast(isnull(a.vcNextTwoYear,0) as decimal(18,2)) as [vcNextTwoYear],case when isnull(vcEmailFlag,'0')='0' then '待发送' else '已发送' end as vcEmailFlag, [vcOperatorID],convert(varchar(10), dOperatorTime,111) as [dOperatorTime],'0' as vcModFlag,'0' as vcAddFlag   ");
                strSql.AppendLine("  from TAnnualManagement a  ");
                //strSql.AppendLine("  left join (select vcValue,vcName from [TCode] where vcCodeId='C000') b on a.vcInjectionFactory=b.vcValue  ");
                strSql.AppendLine("  left join (select vcValue,vcName from [TCode] where vcCodeId='C003') c on a.vcInsideOutsideType=c.vcValue  ");
                strSql.AppendLine("  left join (select vcValue,vcName from [TCode] where vcCodeId='C061') d on a.vcType=d.vcValue  ");
                strSql.AppendLine("  where 1=1  ");
                strSql.AppendLine("    ");

                if (vcTargetYear.Length > 0)
                {
                    strSql.AppendLine("  and  vcTargetYear = '" + vcTargetYear + "' ");
                }
                if (vcReceiver.Length > 0)
                {
                    strSql.AppendLine("  and  vcReceiver = '" + vcReceiver + "' ");
                }
                if (vcPackPlant.Length > 0)
                {
                    strSql.AppendLine("  and  vcPackPlant = '" + vcPackPlant + "' ");
                }
                if (vcPartNo.Length > 0)
                {
                    strSql.AppendLine("  and  vcPartNo like '" + vcPartNo + "%' ");
                }
                if (vcInjectionFactory.Length > 0)
                {
                    strSql.AppendLine("  and  vcInjectionFactory = '" + vcInjectionFactory + "' ");
                }
                if (vcInsideOutsideType.Length > 0)
                {
                    if (vcInsideOutsideType == "内制")
                    {
                        strSql.AppendLine("  and  vcInsideOutsideType = '0' ");
                    } else if (vcInsideOutsideType == "外注")
                    {
                        strSql.AppendLine("  and  vcInsideOutsideType = '1' ");
                    } else
                    {
                        strSql.AppendLine("  and  vcInsideOutsideType = '" + vcInsideOutsideType + "' ");
                    }
                }
                if (vcSupplierId.Length > 0)
                {
                    strSql.AppendLine("  and  vcSupplier_id = '" + vcSupplierId + "' ");
                }
                if (vcWorkArea.Length > 0)
                {
                    if (vcWorkArea == "无")
                    {
                        strSql.AppendLine("  and  isnull(vcWorkArea,'') = '' ");
                    }
                    else {
                        strSql.AppendLine("  and  vcWorkArea = '" + vcWorkArea + "' ");
                    }
                }
                if (vcType.Length>0)
                {
                    strSql.AppendLine("  and  vcType = '" + vcType + "' ");
                }
                
                if (dOperatorTime.Length>0)
                {
                    strSql.AppendLine("  and  convert(varchar(10), dOperatorTime,112) = '" + dOperatorTime.Replace("-", "").Replace("/", "") + "' ");
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
                    
                    string  vcType = dt.Rows[i]["vcType"] == System.DBNull.Value ? "" : dt.Rows[i]["vcType"].ToString();
                    if (vcType == "当初年计") //'0','当初年计' '1','修正年计'
                    {
                        vcType = "0";
                    }
                    else
                    {
                        vcType = "1";
                    }
                    string vcReceiver = dt.Rows[i]["vcReceiver"] == System.DBNull.Value ? "" : dt.Rows[i]["vcReceiver"].ToString();
                    string vcPackPlant = dt.Rows[i]["vcPackPlant"] == System.DBNull.Value ? "" : dt.Rows[i]["vcPackPlant"].ToString();
                    string vcTargetYear = dt.Rows[i]["vcTargetYear"] == System.DBNull.Value ? "" : dt.Rows[i]["vcTargetYear"].ToString();
                    string vcPartNo = dt.Rows[i]["vcPartNo"] == System.DBNull.Value ? "" : dt.Rows[i]["vcPartNo"].ToString();
                    string vcInjectionFactory = dt.Rows[i]["vcInjectionFactory"] == System.DBNull.Value ? "" : dt.Rows[i]["vcInjectionFactory"].ToString();
                    string vcInsideOutsideType = dt.Rows[i]["vcInsideOutsideType"] == System.DBNull.Value ? "" : dt.Rows[i]["vcInsideOutsideType"].ToString();
                    if (vcInsideOutsideType == "内制")
                    {
                        vcInsideOutsideType = "0";
                    }
                    else if (vcInsideOutsideType == "外注")
                    {
                        vcInsideOutsideType = "1";
                    }
                    else
                    { }
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
                    
                    strSql.AppendLine("  		insert into dbo.TAnnualManagement (vcType,vcReceiver,vcPackPlant, vcTargetYear, vcPartNo, vcInjectionFactory,  ");
                    strSql.AppendLine("  		 vcInsideOutsideType, vcSupplier_id, vcWorkArea, vcCarType,   ");
                    strSql.AppendLine("  		 vcAcceptNum, vcJanuary, vcFebruary, vcMarch, vcApril, vcMay, vcJune,   ");
                    strSql.AppendLine("  		 vcJuly, vcAugust, vcSeptember, vcOctober, vcNovember, vcDecember,    ");
                    strSql.AppendLine("  		 vcNextOneYear, vcNextTwoYear, vcOperatorID, dOperatorTime)   ");
                    strSql.AppendLine("  		values   ");
                    strSql.AppendLine("  		('" + vcType + "','" + vcReceiver + "','" + vcPackPlant + "','" + vcTargetYear + "','" + vcPartNo + "','" + vcInjectionFactory + "','" + vcInsideOutsideType + "',    ");
                    strSql.AppendLine("  		'" + vcSupplier_id + "','" + vcWorkArea + "','" + vcCarType + "'," + getSqlValue(vcAcceptNum,false) + "," + getSqlValue(vcJanuary, true) + ",   ");
                    strSql.AppendLine("  		" + getSqlValue(vcFebruary, true) + "," + getSqlValue(vcMarch, true) + "," + getSqlValue(vcApril, true) + "," + getSqlValue(vcMay, true) + "," + getSqlValue(vcJune, true) + ",   ");
                    strSql.AppendLine("  		" + getSqlValue(vcJuly, true) + "," + getSqlValue(vcAugust, true) + "," + getSqlValue(vcSeptember, true) + "," + getSqlValue(vcOctober, true) + "," + getSqlValue(vcNovember, true) + ",   ");
                    strSql.AppendLine("  		" + getSqlValue(vcDecember, true) + "," + getSqlValue(vcNextOneYear, true) + "," + vcNextTwoYear + ",'" + strUserId + "',GETDATE()) ;   ");

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

        #region 报表相关
        /// <summary>
        /// 获取制定对象年的 发注工厂
        /// </summary>
        /// <param name="vcTargetYear"></param>
        /// <returns></returns>
        public DataTable getPlant(string vcTargetYear, string vcType)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("  select distinct vcInjectionFactory from TAnnualManagement where vcTargetYear='"+ vcTargetYear + "' and vcType='"+ vcType + "'  ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable getDtByTargetYearAndPlant(string vcTargetYear, string plantCode, string vcType)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("   select vcInjectionFactory,vcSupplierId,  [1月] ,[2月],[3月],[4月], [5月],[6月],[7月],[8月], [9月],[10月], [11月],[12月] ,    ");
                strSql.AppendLine("    cast(isnull(T.[1月],0) as decimal(18,6))+cast(isnull(T.[2月],0) as decimal(18,6))+cast(isnull(T.[3月],0) as decimal(18,6))    ");
                strSql.AppendLine("  +cast(isnull(T.[4月],0) as decimal(18,6))+cast(isnull(T.[5月],0) as decimal(18,6))+cast(isnull(T.[6月],0) as decimal(18,6))+    ");
                strSql.AppendLine("  cast(isnull(t.[7月],0) as decimal(18,6))+cast(isnull(T.[8月],0) as decimal(18,6))+cast(isnull(T.[9月],0) as decimal(18,6))    ");
                strSql.AppendLine("  +cast(isnull(T.[10月],0) as decimal(18,6))+cast(isnull(T.[11月],0) as decimal(18,6))+cast(isnull(T.[12月],0) as decimal(18,6)) as  currentSum,    ");
                strSql.AppendLine("   t.[vcNextOneYear],t.[vcNextTwoYear]     ");
                strSql.AppendLine("   from    ");
                strSql.AppendLine("   (    ");
                strSql.AppendLine("   select vcInjectionFactory,vcSupplierId,     ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcJanuary],0) as decimal(18,6))) as  \"1月\",     ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcFebruary],0) as decimal(18,6))) as  \"2月\",     ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcMarch],0) as decimal(18,6))) as  \"3月\",     ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcApril],0) as decimal(18,6)))  as  \"4月\",     ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcMay],0) as decimal(18,6))) as  \"5月\",    ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcJune],0) as decimal(18,6)))  as  \"6月\",     ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcJuly],0) as decimal(18,6)))  as  \"7月\",    ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcAugust],0) as decimal(18,6))) as  \"8月\",    ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcSeptember],0) as decimal(18,6))) as  \"9月\",     ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcOctober],0) as decimal(18,6))) as  \"10月\",     ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcNovember],0) as decimal(18,6))) as  \"11月\",    ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcDecember],0) as decimal(18,6))) as  \"12月\",    ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcNextOneYear],0) as decimal(18,6)) ) as  [vcNextOneYear],    ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcNextTwoYear],0) as decimal(18,6)) ) as  [vcNextTwoYear]    ");
                strSql.AppendLine("  from (    ");
                strSql.AppendLine("  select [iAutoId], [vcPackPlant], [vcTargetYear], [vcPartNo], [vcInjectionFactory], [vcInsideOutsideType],    ");
                strSql.AppendLine("   case when vcName is null then '外注'    ");
                strSql.AppendLine("    else vcName end as [vcSupplierId],    ");
                strSql.AppendLine("  [vcWorkArea], [vcCarType], [vcAcceptNum], [vcJanuary], [vcFebruary], [vcMarch],     ");
                strSql.AppendLine("  [vcApril], [vcMay], [vcJune], [vcJuly], [vcAugust], [vcSeptember], [vcOctober], [vcNovember],    ");
                strSql.AppendLine("  [vcDecember], [vcNextOneYear], [vcNextTwoYear], a.[vcOperatorID], a.[dOperatorTime] ,b.*    ");
                strSql.AppendLine("  from TAnnualManagement a     ");
                strSql.AppendLine("   left join (select vcValue,vcName from TCode where vcCodeId='C055') b on a.[vcSupplier_id] = b.vcValue    ");
                strSql.AppendLine("   where  b.vcValue is not null and vcTargetYear='"+ vcTargetYear + "' and vcType='"+ vcType + "' and vcInjectionFactory='" + plantCode + "'    ");
                strSql.AppendLine("   ) S group by vcInjectionFactory,vcSupplierId    ");
                strSql.AppendLine("   ) T    ");
                strSql.AppendLine("    union all   ");
                strSql.AppendLine("     select '合计' as vcInjectionFactory,'' as vcSupplier_id,     ");
                strSql.AppendLine("     sum(W.[1月]) as [1月], sum(W.[2月]) as [2月], sum(W.[3月]) as [3月],     "); 
                strSql.AppendLine("     sum(W.[4月]) as [4月], sum(W.[5月]) as [5月], sum(W.[6月]) as [6月],     ");
                strSql.AppendLine("     sum(W.[7月]) as [7月], sum(W.[8月]) as [8月], sum(W.[9月]) as [9月],     ");
                strSql.AppendLine("     sum(W.[10月]) as [10月],sum(W.[11月]) as [11月], sum(W.[12月]) as [12月],   ");
                strSql.AppendLine("     sum(W.currentSum) as currentSum,SUM(W.vcNextOneYear) as vcNextOneYear,   ");
                strSql.AppendLine("     SUM(W.vcNextTwoYear) as vcNextTwoYear   ");
                strSql.AppendLine("      ");
                strSql.AppendLine("     from   ");
                strSql.AppendLine("     (   ");
                strSql.AppendLine("     select vcInjectionFactory,vcSupplierId,[1月] ,[2月],[3月],[4月], [5月],[6月],[7月],[8月], [9月],[10月], [11月],[12月] ,   ");
                strSql.AppendLine("     cast(isnull(T.[1月],0) as decimal(18,6))+cast(isnull(T.[2月],0) as decimal(18,6))+cast(isnull(T.[3月],0) as decimal(18,6))   ");
                strSql.AppendLine("   +cast(isnull(T.[4月],0) as decimal(18,6))+cast(isnull(T.[5月],0) as decimal(18,6))+cast(isnull(T.[6月],0) as decimal(18,6))+   ");
                strSql.AppendLine("   cast(isnull(t.[7月],0) as decimal(18,6))+cast(isnull(T.[8月],0) as decimal(18,6))+cast(isnull(T.[9月],0) as decimal(18,6))   ");
                strSql.AppendLine("   +cast(isnull(T.[10月],0) as decimal(18,6))+cast(isnull(T.[11月],0) as decimal(18,6))+cast(isnull(T.[12月],0) as decimal(18,6)) as  currentSum,   ");
                strSql.AppendLine("    t.[vcNextOneYear],t.[vcNextTwoYear]    ");
                strSql.AppendLine("    from   ");
                strSql.AppendLine("    (   ");
                strSql.AppendLine("    select vcInjectionFactory,vcSupplierId,    ");
                strSql.AppendLine("   sum(cast(isnull(S.[vcJanuary],0) as decimal(18,6))) as  \"1月\",    ");
                strSql.AppendLine("   sum(cast(isnull(S.[vcFebruary],0) as decimal(18,6))) as  \"2月\",    ");
                strSql.AppendLine("   sum(cast(isnull(S.[vcMarch],0) as decimal(18,6))) as  \"3月\",    ");
                strSql.AppendLine("   sum(cast(isnull(S.[vcApril],0) as decimal(18,6)))  as  \"4月\",    ");
                strSql.AppendLine("   sum(cast(isnull(S.[vcMay],0) as decimal(18,6))) as  \"5月\",   ");
                strSql.AppendLine("   sum(cast(isnull(S.[vcJune],0) as decimal(18,6)))  as  \"6月\",    ");
                strSql.AppendLine("   sum(cast(isnull(S.[vcJuly],0) as decimal(18,6)))  as  \"7月\",   ");
                strSql.AppendLine("   sum(cast(isnull(S.[vcAugust],0) as decimal(18,6))) as  \"8月\",   ");
                strSql.AppendLine("   sum(cast(isnull(S.[vcSeptember],0) as decimal(18,6))) as  \"9月\",    ");
                strSql.AppendLine("   sum(cast(isnull(S.[vcOctober],0) as decimal(18,6))) as  \"10月\",    ");
                strSql.AppendLine("   sum(cast(isnull(S.[vcNovember],0) as decimal(18,6))) as  \"11月\",   ");
                strSql.AppendLine("   sum(cast(isnull(S.[vcDecember],0) as decimal(18,6))) as  \"12月\",   ");
                strSql.AppendLine("   sum(cast(isnull(S.[vcNextOneYear],0) as decimal(18,6)) ) as  [vcNextOneYear],   ");
                strSql.AppendLine("   sum(cast(isnull(S.[vcNextTwoYear],0) as decimal(18,6)) ) as  [vcNextTwoYear]   ");
                strSql.AppendLine("   from (   ");
                strSql.AppendLine("   select [iAutoId], [vcPackPlant], [vcTargetYear], [vcPartNo], [vcInjectionFactory], [vcInsideOutsideType],   ");
                strSql.AppendLine("    case when vcName is null then '外注'   ");
                strSql.AppendLine("     else vcName end as [vcSupplierId],   ");
                strSql.AppendLine("   [vcWorkArea], [vcCarType], [vcAcceptNum], [vcJanuary], [vcFebruary], [vcMarch],    ");
                strSql.AppendLine("   [vcApril], [vcMay], [vcJune], [vcJuly], [vcAugust], [vcSeptember], [vcOctober], [vcNovember],   ");
                strSql.AppendLine("   [vcDecember], [vcNextOneYear], [vcNextTwoYear], a.[vcOperatorID], a.[dOperatorTime] ,b.*   ");
                strSql.AppendLine("   from TAnnualManagement a    ");
                strSql.AppendLine("    left join (select vcValue,vcName from TCode where vcCodeId='C055') b on a.[vcSupplier_id] = b.vcValue   ");
                strSql.AppendLine("    where  b.vcValue is not null and vcTargetYear='"+vcTargetYear+ "' and vcType='" + vcType + "' and vcInjectionFactory='" + plantCode+"'   ");
                strSql.AppendLine("    ) S group by vcInjectionFactory,vcSupplierId   ");
                strSql.AppendLine("    ) T   ");
                strSql.AppendLine("     ) W   ");
                strSql.AppendLine("      ");
                strSql.AppendLine("      ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 发注数据
        /// </summary>
        /// <param name="vcTargetYear"></param>
        /// <returns></returns>
        public DataTable getWaiZhuDt(string vcTargetYear, string vcType)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("   select vcInjectionFactory,vcSupplierId,[1月] ,[2月],[3月],[4月], [5月],[6月],[7月],[8月], [9月],[10月], [11月],[12月] ,  ");
                strSql.AppendLine("    cast(isnull(T.[1月],0) as decimal(18,6))+cast(isnull(T.[2月],0) as decimal(18,6))+cast(isnull(T.[3月],0) as decimal(18,6))  ");
                strSql.AppendLine("  +cast(isnull(T.[4月],0) as decimal(18,6))+cast(isnull(T.[5月],0) as decimal(18,6))+cast(isnull(T.[6月],0) as decimal(18,6))+  ");
                strSql.AppendLine("  cast(isnull(t.[7月],0) as decimal(18,6))+cast(isnull(T.[8月],0) as decimal(18,6))+cast(isnull(T.[9月],0) as decimal(18,6))  ");
                strSql.AppendLine("  +cast(isnull(T.[10月],0) as decimal(18,6))+cast(isnull(T.[11月],0) as decimal(18,6))+cast(isnull(T.[12月],0) as decimal(18,6)) as  currentSum,  ");
                strSql.AppendLine("   t.[vcNextOneYear],t.[vcNextTwoYear]   ");
                strSql.AppendLine("   from  ");
                strSql.AppendLine("   (  ");
                strSql.AppendLine("   select vcInjectionFactory,vcSupplierId,   ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcJanuary],0) as decimal(18,6))) as  \"1月\",   ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcFebruary],0) as decimal(18,6))) as  \"2月\",   ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcMarch],0) as decimal(18,6))) as  \"3月\",   ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcApril],0) as decimal(18,6)))  as  \"4月\",   ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcMay],0) as decimal(18,6))) as  \"5月\",  ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcJune],0) as decimal(18,6)))  as  \"6月\",   ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcJuly],0) as decimal(18,6)))  as  \"7月\",  ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcAugust],0) as decimal(18,6))) as  \"8月\",  ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcSeptember],0) as decimal(18,6))) as  \"9月\",   ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcOctober],0) as decimal(18,6))) as  \"10月\",   ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcNovember],0) as decimal(18,6))) as  \"11月\",  ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcDecember],0) as decimal(18,6))) as  \"12月\",  ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcNextOneYear],0) as decimal(18,6)) ) as  [vcNextOneYear],  ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcNextTwoYear],0) as decimal(18,6)) ) as  [vcNextTwoYear]  ");
                strSql.AppendLine("  from (  ");
                strSql.AppendLine("  select [iAutoId], [vcPackPlant], [vcTargetYear], [vcPartNo], [vcInjectionFactory], [vcInsideOutsideType],  ");
                strSql.AppendLine("   case when vcName is null then '外注'  ");
                strSql.AppendLine("    else vcName end as [vcSupplierId],  ");
                strSql.AppendLine("  [vcWorkArea], [vcCarType], [vcAcceptNum], [vcJanuary], [vcFebruary], [vcMarch],   ");
                strSql.AppendLine("  [vcApril], [vcMay], [vcJune], [vcJuly], [vcAugust], [vcSeptember], [vcOctober], [vcNovember],  ");
                strSql.AppendLine("  [vcDecember], [vcNextOneYear], [vcNextTwoYear], a.[vcOperatorID], a.[dOperatorTime] ,b.*  ");
                strSql.AppendLine("  from TAnnualManagement a   ");
                strSql.AppendLine("   left join (select vcValue,vcName from TCode where vcCodeId='C055') b on a.[vcSupplier_id] = b.vcValue  ");
                strSql.AppendLine("   where  b.vcValue is  null and vcTargetYear='"+vcTargetYear+"' and vcType='"+ vcType + "'  ");
                strSql.AppendLine("   ) S group by vcInjectionFactory,vcSupplierId  ");
                strSql.AppendLine("   ) T  ");
                strSql.AppendLine("    union all  ");
                strSql.AppendLine("    select '合计' as vcInjectionFactory,'' as vcSupplier_id,    ");
                strSql.AppendLine("    sum(W.[1月]) as [1月], sum(W.[2月]) as [2月], sum(W.[3月]) as [3月],    ");
                strSql.AppendLine("    sum(W.[4月]) as [4月], sum(W.[5月]) as [5月], sum(W.[6月]) as [6月],    ");
                strSql.AppendLine("    sum(W.[7月]) as [7月], sum(W.[8月]) as [8月], sum(W.[9月]) as [9月],    ");
                strSql.AppendLine("    sum(W.[10月]) as [10月],sum(W.[11月]) as [11月], sum(W.[12月]) as [12月],  ");
                strSql.AppendLine("    sum(W.currentSum) as currentSum,SUM(W.vcNextOneYear) as vcNextOneYear,  ");
                strSql.AppendLine("    SUM(W.vcNextTwoYear) as vcNextTwoYear  ");
                strSql.AppendLine("    ");
                strSql.AppendLine("    from  ");
                strSql.AppendLine("    (  ");
                strSql.AppendLine("    select vcInjectionFactory,vcSupplierId,[1月] ,[2月],[3月],[4月], [5月],[6月],[7月],[8月], [9月],[10月], [11月],[12月] ,  ");
                strSql.AppendLine("    cast(isnull(T.[1月],0) as decimal(18,6))+cast(isnull(T.[2月],0) as decimal(18,6))+cast(isnull(T.[3月],0) as decimal(18,6))  ");
                strSql.AppendLine("  +cast(isnull(T.[4月],0) as decimal(18,6))+cast(isnull(T.[5月],0) as decimal(18,6))+cast(isnull(T.[6月],0) as decimal(18,6))+  ");
                strSql.AppendLine("  cast(isnull(t.[7月],0) as decimal(18,6))+cast(isnull(T.[8月],0) as decimal(18,6))+cast(isnull(T.[9月],0) as decimal(18,6))  ");
                strSql.AppendLine("  +cast(isnull(T.[10月],0) as decimal(18,6))+cast(isnull(T.[11月],0) as decimal(18,6))+cast(isnull(T.[12月],0) as decimal(18,6)) as  currentSum,  ");
                strSql.AppendLine("   t.[vcNextOneYear],t.[vcNextTwoYear]   ");
                strSql.AppendLine("   from  ");
                strSql.AppendLine("   (  ");
                strSql.AppendLine("   select vcInjectionFactory,vcSupplierId,   ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcJanuary],0) as decimal(18,6))) as  \"1月\",   ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcFebruary],0) as decimal(18,6))) as  \"2月\",   ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcMarch],0) as decimal(18,6))) as  \"3月\",   ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcApril],0) as decimal(18,6)))  as  \"4月\",   ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcMay],0) as decimal(18,6))) as  \"5月\",  ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcJune],0) as decimal(18,6)))  as  \"6月\",   ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcJuly],0) as decimal(18,6)))  as  \"7月\",  ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcAugust],0) as decimal(18,6))) as  \"8月\",  ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcSeptember],0) as decimal(18,6))) as  \"9月\",   ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcOctober],0) as decimal(18,6))) as  \"10月\",   ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcNovember],0) as decimal(18,6))) as  \"11月\",  ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcDecember],0) as decimal(18,6))) as  \"12月\",  ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcNextOneYear],0) as decimal(18,6)) ) as  [vcNextOneYear],  ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcNextTwoYear],0) as decimal(18,6)) ) as  [vcNextTwoYear]  ");
                strSql.AppendLine("  from (  ");
                strSql.AppendLine("  select [iAutoId], [vcPackPlant], [vcTargetYear], [vcPartNo], [vcInjectionFactory], [vcInsideOutsideType],  ");
                strSql.AppendLine("   case when vcName is null then '外注'  ");
                strSql.AppendLine("    else vcName end as [vcSupplierId],  ");
                strSql.AppendLine("  [vcWorkArea], [vcCarType], [vcAcceptNum], [vcJanuary], [vcFebruary], [vcMarch],   ");
                strSql.AppendLine("  [vcApril], [vcMay], [vcJune], [vcJuly], [vcAugust], [vcSeptember], [vcOctober], [vcNovember],  ");
                strSql.AppendLine("  [vcDecember], [vcNextOneYear], [vcNextTwoYear], a.[vcOperatorID], a.[dOperatorTime] ,b.*  ");
                strSql.AppendLine("  from TAnnualManagement a   ");
                strSql.AppendLine("   left join (select vcValue,vcName from TCode where vcCodeId='C055') b on a.[vcSupplier_id] = b.vcValue  ");
                strSql.AppendLine("   where  b.vcValue is  null and vcTargetYear='" + vcTargetYear + "' and vcType='" + vcType + "'  ");
                strSql.AppendLine("   ) S group by vcInjectionFactory,vcSupplierId  ");
                strSql.AppendLine("   ) T  ");
                strSql.AppendLine("    ) W  order by vcInjectionFactory asc ");
               
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 最终合计数据
        /// </summary>
        /// <param name="vcTargetYear"></param>
        /// <returns></returns>
        public DataTable getHuiZongDt(string vcTargetYear, string vcType)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                
                strSql.AppendLine("   select vcInjectionFactory,vcSupplierId,[1月] ,[2月],[3月],[4月], [5月],[6月],[7月],[8月], [9月],[10月], [11月],[12月] ,  ");
                strSql.AppendLine("    cast(isnull(T.[1月],0) as decimal(18,6))+cast(isnull(T.[2月],0) as decimal(18,6))+cast(isnull(T.[3月],0) as decimal(18,6))  ");
                strSql.AppendLine("  +cast(isnull(T.[4月],0) as decimal(18,6))+cast(isnull(T.[5月],0) as decimal(18,6))+cast(isnull(T.[6月],0) as decimal(18,6))+  ");
                strSql.AppendLine("  cast(isnull(t.[7月],0) as decimal(18,6))+cast(isnull(T.[8月],0) as decimal(18,6))+cast(isnull(T.[9月],0) as decimal(18,6))  ");
                strSql.AppendLine("  +cast(isnull(T.[10月],0) as decimal(18,6))+cast(isnull(T.[11月],0) as decimal(18,6))+cast(isnull(T.[12月],0) as decimal(18,6))  ");
                strSql.AppendLine("  as  currentSum,  ");
                strSql.AppendLine("   t.[vcNextOneYear],t.[vcNextTwoYear]   ");
                strSql.AppendLine("   from  ");
                strSql.AppendLine("   (  ");
                strSql.AppendLine("   select '' as vcInjectionFactory,'' as vcSupplierId,   ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcJanuary],0) as decimal(18,6))) as  \"1月\",   ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcFebruary],0) as decimal(18,6))) as  \"2月\",   ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcMarch],0) as decimal(18,6))) as  \"3月\",   ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcApril],0) as decimal(18,6)))  as  \"4月\",   ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcMay],0) as decimal(18,6))) as  \"5月\",  ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcJune],0) as decimal(18,6)))  as  \"6月\",   ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcJuly],0) as decimal(18,6)))  as  \"7月\",  ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcAugust],0) as decimal(18,6))) as  \"8月\",  ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcSeptember],0) as decimal(18,6))) as  \"9月\",   ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcOctober],0) as decimal(18,6))) as  \"10月\",   ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcNovember],0) as decimal(18,6))) as  \"11月\",  ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcDecember],0) as decimal(18,6))) as  \"12月\",  ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcNextOneYear],0) as decimal(18,6)) ) as  [vcNextOneYear],  ");
                strSql.AppendLine("  sum(cast(isnull(S.[vcNextTwoYear],0) as decimal(18,6)) ) as  [vcNextTwoYear]  ");
                strSql.AppendLine("  from (  ");
                strSql.AppendLine("  select [iAutoId], [vcPackPlant], [vcTargetYear], [vcPartNo], [vcInjectionFactory], [vcInsideOutsideType],  ");
                strSql.AppendLine("   case when vcName is null then '外注'  ");
                strSql.AppendLine("    else vcName end as [vcSupplierId],  ");
                strSql.AppendLine("  [vcWorkArea], [vcCarType], [vcAcceptNum], [vcJanuary], [vcFebruary], [vcMarch],   ");
                strSql.AppendLine("  [vcApril], [vcMay], [vcJune], [vcJuly], [vcAugust], [vcSeptember], [vcOctober], [vcNovember],  ");
                strSql.AppendLine("  [vcDecember], [vcNextOneYear], [vcNextTwoYear], a.[vcOperatorID], a.[dOperatorTime] ,b.*  ");
                strSql.AppendLine("  from TAnnualManagement a   ");
                strSql.AppendLine("   left join (select vcValue,vcName from TCode where vcCodeId='C055') b on a.[vcSupplier_id] = b.vcValue  ");
                strSql.AppendLine("   where   vcTargetYear='"+ vcTargetYear + "' and vcType = '"+ vcType + "'  ");
                strSql.AppendLine("   ) S   ");
                strSql.AppendLine("   ) T  ");
                
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
