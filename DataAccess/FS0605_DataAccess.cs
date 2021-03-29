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
    public class FS0605_DataAccess
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
                        strSql.AppendLine("  union all SELECT iAutoId, vcSupplier_id, vcWorkArea, vcIsSureFlag, vcLinkMan, vcPhone, vcEmail FROM [dbo].[TSupplierInfo] where vcSupplier_id='" + dtadd.Rows[i]["vcSupplier_id"] + "' and vcWorkArea='" + dtadd.Rows[i]["vcWorkArea"] + "'  ");
                    }
                    else
                    {
                        strSql.AppendLine("  SELECT iAutoId, vcSupplier_id, vcWorkArea, vcIsSureFlag, vcLinkMan, vcPhone, vcEmail FROM [dbo].[TSupplierInfo] where vcSupplier_id='" + dtadd.Rows[i]["vcSupplier_id"] + "' and vcWorkArea='" + dtadd.Rows[i]["vcWorkArea"] + "'  ");
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

        public DataTable GetSupplier()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine(" select vcSupplier_id as vcValue,vcSupplier_id as vcName from (  ");
                strSql.AppendLine(" select distinct vcSupplier_id  from TSupplierInfo   ");
                strSql.AppendLine(" ) a order by vcSupplier_id asc  ");
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
                strSql.AppendLine(" select vcWorkArea as vcValue,vcWorkArea as vcName from (  ");
                strSql.AppendLine(" select distinct isnull(vcWorkArea,'无') as vcWorkArea  from TSupplierInfo   ");
                strSql.AppendLine(" ) a order by vcWorkArea asc  ");
                strSql.AppendLine("   ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetWorkAreaBySupplier(string supplierCode)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine(" select vcWorkArea as vcValue,vcWorkArea as vcName from (  ");
                strSql.AppendLine(" select distinct isnull(vcWorkArea,'无') as vcWorkArea  from TSupplierInfo where vcSupplier_id='"+ supplierCode + "'    ");
                strSql.AppendLine(" ) a order by vcWorkArea asc  ");
                strSql.AppendLine("   ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
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
                    //"vcSupplier_id", "vcWorkArea", "vcIsSureFlag", "vcLinkMan", "vcPhone", "vcEmail"

                    StringBuilder strSql = new StringBuilder();
                    string vcSupplier_id = dt.Rows[i]["vcSupplier_id"] == System.DBNull.Value ? "" : dt.Rows[i]["vcSupplier_id"].ToString().Trim();
                    string vcWorkArea = dt.Rows[i]["vcWorkArea"] == System.DBNull.Value ? "" : dt.Rows[i]["vcWorkArea"].ToString().Trim();
                    string vcIsSureFlag = dt.Rows[i]["vcIsSureFlag"] == System.DBNull.Value ? "" : dt.Rows[i]["vcIsSureFlag"].ToString().Trim();
                    if (vcIsSureFlag == "√")
                    {
                        vcIsSureFlag = "1";
                    } else
                    {
                        vcIsSureFlag = "0";
                    }
                    string vcLinkMan1 = dt.Rows[i]["vcLinkMan1"] == System.DBNull.Value ? "" : dt.Rows[i]["vcLinkMan1"].ToString().Trim();
                    string vcPhone1 = dt.Rows[i]["vcPhone1"] == System.DBNull.Value ? "" : dt.Rows[i]["vcPhone1"].ToString().Trim();
                    string vcEmail1 = dt.Rows[i]["vcEmail1"] == System.DBNull.Value ? "" : dt.Rows[i]["vcEmail1"].ToString().Trim();

                    string vcLinkMan2 = dt.Rows[i]["vcLinkMan2"] == System.DBNull.Value ? "" : dt.Rows[i]["vcLinkMan2"].ToString().Trim();
                    string vcPhone2 = dt.Rows[i]["vcPhone2"] == System.DBNull.Value ? "" : dt.Rows[i]["vcPhone2"].ToString().Trim();
                    string vcEmail2 = dt.Rows[i]["vcEmail2"] == System.DBNull.Value ? "" : dt.Rows[i]["vcEmail2"].ToString().Trim();

                    string vcLinkMan3 = dt.Rows[i]["vcLinkMan3"] == System.DBNull.Value ? "" : dt.Rows[i]["vcLinkMan3"].ToString().Trim();
                    string vcPhone3 = dt.Rows[i]["vcPhone3"] == System.DBNull.Value ? "" : dt.Rows[i]["vcPhone3"].ToString().Trim();
                    string vcEmail3 = dt.Rows[i]["vcEmail3"] == System.DBNull.Value ? "" : dt.Rows[i]["vcEmail3"].ToString().Trim();

                    strSql.AppendLine("  declare @isExist int =0;   ");
                    strSql.AppendLine("  select @isExist=COUNT(*) from TSupplierInfo where vcSupplier_id='" + vcSupplier_id + "' and vcWorkArea='"+ vcWorkArea + "'   ");
                    strSql.AppendLine("     ");
                    strSql.AppendLine("  if @isExist>0   ");
                    strSql.AppendLine("  begin   ");
                    strSql.AppendLine("  		update TSupplierInfo set vcIsSureFlag = '" + vcIsSureFlag + "',vcLinkMan1='" + vcLinkMan1 + "',vcPhone1='" + vcPhone1 + "',vcEmail1='" + vcEmail1 + "',vcLinkMan2='" + vcLinkMan2 + "',vcPhone2='" + vcPhone2 + "',vcEmail2='" + vcEmail2 + "',vcLinkMan3='" + vcLinkMan3 + "',vcPhone3='" + vcPhone3 + "',vcEmail3='" + vcEmail3 + "',  ");
                    strSql.AppendLine("  		vcOperatorID='" + strUserId + "',dOperatorTime=GETDATE() where vcSupplier_id='" + vcSupplier_id + "' and vcWorkArea='" + vcWorkArea + "' ;  ");
                    strSql.AppendLine("  end   ");
                    strSql.AppendLine("  else   ");
                    strSql.AppendLine("  begin   ");
                    strSql.AppendLine("  		insert into dbo.TSupplierInfo (vcSupplier_id, vcWorkArea, vcIsSureFlag, vcLinkMan1, vcPhone1,vcEmail1, vcLinkMan2, vcPhone2,vcEmail2, vcLinkMan3, vcPhone3,vcEmail3,    ");
                    strSql.AppendLine("  		 vcOperatorID, dOperatorTime )    ");
                    strSql.AppendLine("  		values   ");
                    strSql.AppendLine("  		('" + vcSupplier_id + "','" + vcWorkArea + "','" + vcIsSureFlag + "','" + vcLinkMan1 + "','" + vcPhone1 + "','" + vcEmail1 + "','" + vcLinkMan2 + "','" + vcPhone2 + "','" + vcEmail2 + "','" + vcLinkMan3 + "','" + vcPhone3 + "','" + vcEmail3 + "','" + strUserId + "',GETDATE()) ;   ");
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

        public DataTable CheckDistinctByTable(DataTable dtadd)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                for (int i = 0; i < dtadd.Rows.Count; i++)
                {
                    if (strSql.Length > 0)
                    {
                        strSql.AppendLine("  union all SELECT vcSupplier_id, vcWorkArea, vcIsSureFlag FROM [dbo].[TSupplierInfo] where vcSupplier_id='" + dtadd.Rows[i]["vcSupplier_id"] + "' and vcWorkArea='" + dtadd.Rows[i]["vcWorkArea"] + "'  ");
                    }
                    else
                    {
                        strSql.AppendLine("  SELECT vcSupplier_id, vcWorkArea, vcIsSureFlag  FROM [dbo].[TSupplierInfo] where vcSupplier_id='" + dtadd.Rows[i]["vcSupplier_id"] + "' and vcWorkArea='" + dtadd.Rows[i]["vcWorkArea"] + "'  ");
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
        /// 检索数据
        /// </summary>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        public DataTable Search(string vcSupplier_id, string vcWorkArea, string vcIsSureFlag)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("   select iAutoId,  [vcSupplier_id], [vcWorkArea], vcIsSureFlag,[vcLinkMan1] ,[vcPhone1] ,[vcEmail1],[vcLinkMan2] ,[vcPhone2] ,[vcEmail2],[vcLinkMan3] ,[vcPhone3],[vcEmail3], [vcOperatorID], [dOperatorTime],'0' as vcModFlag,'0' as vcAddFlag From [dbo].[TSupplierInfo] where 1=1   ");

                if (vcSupplier_id.Length > 0)
                {
                    strSql.AppendLine("  and  vcSupplier_id like '" + vcSupplier_id + "%' ");
                }
                if (vcWorkArea.Length > 0)
                {
                    strSql.AppendLine("  and  vcWorkArea like '" + vcWorkArea + "%' ");
                }
                if (vcIsSureFlag.Length > 0)
                {
                    strSql.AppendLine("  and  vcIsSureFlag = '" + vcIsSureFlag + "' ");
                }
                strSql.AppendLine("  order by  iAutoId desc ");
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
                sql.Append("  delete [TSupplierInfo] where iAutoId in(   \r\n ");
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
                        string vcIsSureFlag = listInfoData[i]["vcIsSureFlag"] == null ? "0" : listInfoData[i]["vcIsSureFlag"].ToString();
                        sql.Append("insert into [TSupplierInfo] (vcSupplier_id, vcWorkArea, vcIsSureFlag, vcLinkMan1, vcPhone1, vcEmail1, vcLinkMan2, vcPhone2, vcEmail2,vcLinkMan3, vcPhone3, vcEmail3,vcOperatorID, dOperatorTime)  \n");
                        sql.Append(" values (  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcSupplier_id"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcWorkArea"], false) + ",  \r\n");
                        sql.Append(getSqlValue(vcIsSureFlag, false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcLinkMan1"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcPhone1"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcEmail1"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcLinkMan2"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcPhone2"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcEmail2"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcLinkMan3"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcPhone3"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcEmail3"], false) + ",  \r\n");
                        sql.Append("   '" + userId + "', GETDATE() \r\n");
                        sql.Append(" );  \r\n");
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                        string vcIsSureFlag = listInfoData[i]["vcIsSureFlag"] == null ? "0" : listInfoData[i]["vcIsSureFlag"].ToString();
                        sql.Append("  update TSupplierInfo set    \r\n");
                        sql.Append("  vcIsSureFlag=" + getSqlValue(vcIsSureFlag, false) + "   \r\n");
                        sql.Append("  ,vcLinkMan1=" + getSqlValue(listInfoData[i]["vcLinkMan1"], false) + "   \r\n");
                        sql.Append("  ,vcPhone1=" + getSqlValue(listInfoData[i]["vcPhone1"], false) + "   \r\n");
                        sql.Append("  ,vcEmail1=" + getSqlValue(listInfoData[i]["vcEmail1"], false) + "   \r\n");
                        sql.Append("  ,vcLinkMan2=" + getSqlValue(listInfoData[i]["vcLinkMan2"], false) + "   \r\n");
                        sql.Append("  ,vcPhone2=" + getSqlValue(listInfoData[i]["vcPhone2"], false) + "   \r\n");
                        sql.Append("  ,vcEmail2=" + getSqlValue(listInfoData[i]["vcEmail2"], false) + "   \r\n");
                        sql.Append("  ,vcLinkMan3=" + getSqlValue(listInfoData[i]["vcLinkMan3"], false) + "   \r\n");
                        sql.Append("  ,vcPhone3=" + getSqlValue(listInfoData[i]["vcPhone3"], false) + "   \r\n");
                        sql.Append("  ,vcEmail3=" + getSqlValue(listInfoData[i]["vcEmail3"], false) + "   \r\n");
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
                    sql.Append("  		select distinct a.vcSupplier_id from [TSupplierInfo] a   \r\n");
                    sql.Append("  		left join   \r\n");
                    sql.Append("  		(   \r\n");
                    sql.Append("  		   select * from [TSupplierInfo]   \r\n");
                    sql.Append("  		)b on a.vcSupplier_id=b.vcSupplier_id  and a.vcWorkArea=b.vcWorkArea and a.iAutoId<>b.iAutoId   \r\n");
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
                        strSql.AppendLine("  union all select iAutoId, vcSupplier_id, vcWorkArea, vcIsSureFlag, vcLinkMan, vcPhone, vcEmail, vcOperatorID, dOperatorTime from dbo.TSupplierInfo where vcSupplier_id='" + dtamod.Rows[i]["vcSupplier_id"] + "'  ");
                    }
                    else
                    {
                        strSql.AppendLine("  select iAutoId, vcSupplier_id, vcWorkArea, vcIsSureFlag, vcLinkMan, vcPhone, vcEmail, vcOperatorID, dOperatorTime from dbo.TSupplierInfo where vcSupplier_id='" + dtamod.Rows[i]["vcSupplier_id"] + "'  ");
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
