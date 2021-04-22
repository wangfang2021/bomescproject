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
    public class FS0621_DataAccess
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

        #region
        public DataTable GetWorkAreaBySupplier(string supplierCode)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("   select vcWorkArea as vcValue,vcWorkArea as vcName from(       ");
                strSql.AppendLine("   select distinct isnull(iSupplierPlant,'无') as vcWorkArea from     ");
                strSql.AppendLine("   TNeiShi   where SUPPLIERCODE='" + supplierCode + "'      ");
                strSql.AppendLine("   ) t  order by vcWorkArea asc        ");
                strSql.AppendLine("       ");
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

                strSql.AppendLine("     select vcWorkArea as vcValue,vcWorkArea as vcName from(   ");
                strSql.AppendLine("     select distinct isnull(iSupplierPlant,'无') as vcWorkArea from TNeiShi   ");
                strSql.AppendLine("     ) t  order by vcWorkArea asc    ");
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

                strSql.AppendLine("   select vcSupplier_id as vcValue,vcSupplier_id as vcName from(      ");
                strSql.AppendLine("    select distinct SUPPLIERCODE as vcSupplier_id from TNeiShi    ");
                strSql.AppendLine("    ) t  order by vcSupplier_id asc    ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetCarType()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("    select vcCarType as vcValue,vcCarType as vcName from(     ");
                strSql.AppendLine("    select   distinct isnull(CARFAMCODE,'无') as vcCarType from TNeiShi      ");
                strSql.AppendLine("    )s order by vcCarType asc      ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        /// <summary>
        /// 检索数据
        /// </summary>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        public DataTable Search(string vcConsignee, string vcTargetMonth, string vcPartNo, string vcCarType, string vcInsideOutsideType, string vcSupplier_id, string vcWorkArea)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("  SELECT  [iAutoId], [CPDCOMPANY],convert(varchar(10), dInputDate,111) as [dInputDate],left([TARGETMONTH],4)+'/'+right([TARGETMONTH],2) as [TARGETMONTH], [PARTSNO], [CARFAMCODE],c.vcName as [INOUTFLAG], [SUPPLIERCODE],   ");
                strSql.AppendLine("  [iSupplierPlant], [DOCK],cast(isnull(RESULTQTYTOTAL,0) as decimal(18,2)) as RESULTQTYTOTAL, [varInputUser] ,'0' as vcModFlag,'0' as vcAddFlagFROM from [TNeiShi] a   ");
                strSql.AppendLine("  left join (select vcValue,vcName from TCode where vcCodeId='C018') b on a.[CPDCOMPANY] = b.vcValue   ");
                strSql.AppendLine("  left join (select vcValue,vcName from TCode where vcCodeId='C003') c on a.[INOUTFLAG] = c.vcValue  where 1=1  ");

                if (vcConsignee.Length > 0)
                {
                    strSql.AppendLine("  and  a.[CPDCOMPANY]=  '" + vcConsignee + "' ");
                }
                if (vcTargetMonth.Length > 0)
                {
                    strSql.AppendLine("  and  TARGETMONTH=  '" + vcTargetMonth.Replace("-","").Replace("/", "") + "' ");
                }
                if (vcPartNo.Length > 0)
                {
                    strSql.AppendLine("  and  PARTSNO like '" + vcPartNo + "%' ");
                }
                if (vcCarType.Length > 0)
                {
                    if (vcCarType == "无")
                    {
                        strSql.AppendLine("  and  isnull(CARFAMCODE,'') = '' ");
                    }
                    else
                    {
                        strSql.AppendLine("  and  CARFAMCODE like '" + vcCarType + "%' ");
                    }
                }
                if (vcInsideOutsideType.Length > 0)
                {
                    strSql.AppendLine("  and  a.[INOUTFLAG]=  '" + vcInsideOutsideType + "' ");
                }
                if (vcSupplier_id.Length > 0)
                {
                    strSql.AppendLine("  and  a.[SUPPLIERCODE] like  '" + vcSupplier_id + "%' ");
                }
                if (vcWorkArea.Length > 0)
                {
                    if (vcWorkArea == "无")
                    {
                        strSql.AppendLine("  and  isnull( a.iSupplierPlant,'') = '' ");
                    }
                    else
                    {
                        strSql.AppendLine("  and   a.iSupplierPlant = '" + vcWorkArea + "' ");
                    }
                }

                strSql.AppendLine("  order by  dOperatorTime desc,a.INOUTFLAG asc,a.PARTSNO  asc");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
