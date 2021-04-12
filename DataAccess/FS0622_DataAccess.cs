﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Common;
using System.Data.SqlClient;
using System.Collections;

namespace DataAccess
{
    public class FS0622_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        public DataTable GetGroupDt()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("  select iAutoId as vcCodeId,vcGroupName as vcCodeName from [dbo].[TGroup]  ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 检索
        /// </summary>
        /// <param name="vcGroup"></param>
        /// <param name="vcPartNo"></param>
        /// <param name="vcFluctuationRange"></param>
        /// <returns></returns>
        public DataTable Search(string vcGroup, string vcPartNo, string vcFluctuationRange)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("    select a.iAutoId, b.vcGroupName,a.vcPartNo,cast(isnull(b.vcFluctuationRange,0) as decimal(18,2)) as vcFluctuationRange,b.vcDefinition,      ");
                strSql.AppendLine("    '0' as vcModFlag,'0' as vcAddFlag,a.vcOperatorID,a.dOperatorTime       ");
                strSql.AppendLine("    from TDaysChangeOrdersBaseData a       ");
                strSql.AppendLine("    left join (select iAutoId,vcGroupName,vcDefinition,vcFluctuationRange from [dbo].[TGroup]) b      ");
                strSql.AppendLine("    on a.vcGroupId=b.iAutoId  where 1=1     ");
                
                if (vcGroup.Length > 0)
                {
                    strSql.AppendLine("  and  b.iAutoId = '" + vcGroup + "' ");
                }
                if (vcPartNo.Length > 0)
                {
                    strSql.AppendLine("  and  a.vcPartNo like '" + vcPartNo + "%' ");
                }
                if (vcFluctuationRange.Length > 0)
                {
                    strSql.AppendLine("  and  b.vcFluctuationRange = '" + vcFluctuationRange + "' ");
                }

                strSql.AppendLine("  order by  a.iAutoId desc ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable Search_Sub()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("    select iAutoId, vcGroupName, vcDefinition, vcFluctuationRange, vcOperatorID, dOperatorTime,      ");
                strSql.AppendLine("    '0' as vcModFlag,'0' as vcAddFlag       ");
                strSql.AppendLine("    from TGroup      ");
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
                    if (strSql.Length > 0)/*vcOrderDifferentiation='" + dtadd.Rows[i]["vcOrderDifferentiation"] + "' or*/
                    {
                        strSql.AppendLine("  union all SELECT * FROM [dbo].[TGroup] where  vcGroupName='" + dtadd.Rows[i]["vcGroupName"].ToString().Trim() + "'  ");
                    }
                    else
                    {
                        strSql.AppendLine("  SELECT * FROM [dbo].[TGroup] where  vcGroupName='" + dtadd.Rows[i]["vcGroupName"].ToString().Trim() + "'  ");
                    }
                }
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable checkAddData(List<Dictionary<string, object>> listInfoData)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                string partsNosStr = "";
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    
                    if (bAddFlag == true)
                    {
                        partsNosStr += "'"+listInfoData[i]["vcPartNo"].ToString()+"',";
                    }
                }
                partsNosStr = partsNosStr.Substring(0, partsNosStr.Length - 1);
                strSql.AppendLine("  select  distinct vcPartNo from TDaysChangeOrdersBaseData where vcPartNo in (" + partsNosStr + ")  ");
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
                StringBuilder sq0 = new StringBuilder();
                sq0.Append("  select iAutoId,vcGroupName,vcDefinition,vcFluctuationRange from TGroup   \r\n");

                DataTable dt = excute.ExcuteSqlWithSelectToDT(sq0.ToString());
                StringBuilder sql = new StringBuilder();
                
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑

                    string vcGroupName = listInfoData[i]["vcGroupName"] == null ? "" : listInfoData[i]["vcGroupName"].ToString();
                    string vcGroupId = vcGroupName;
                    for (int j=0;j<dt.Rows.Count;j++)
                    {
                        if (vcGroupName== dt.Rows[j]["vcGroupName"].ToString())
                        {
                            vcGroupId = dt.Rows[j]["iAutoId"].ToString();
                            break;
                        }
                    }
;                    if (bAddFlag == true)
                    {//新增
                        sql.Append("   insert into TDaysChangeOrdersBaseData (vcGroupId,vcPartNo,vcOperatorID,dOperatorTime)      \r\n");
                        sql.Append("   values ("+getSqlValue(vcGroupId, true)+" ,    \r\n");
                        sql.Append("    " + getSqlValue(listInfoData[i]["vcPartNo"], true) + " ,      \r\n");
                        sql.Append("     '" + userId + "',GETDATE() \r\n");
                        sql.Append("     );    \r\n");
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);

                        sql.Append("  update TDaysChangeOrdersBaseData set    \r\n");
                        sql.Append("  vcGroupId=" + getSqlValue(vcGroupId, true) + "   \r\n");
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
                    sql.Append("  	select a.vcPartNo+';' from   \r\n");
                    sql.Append("  	(   \r\n");
                    sql.Append("  		select distinct a.vcPartNo from [TDaysChangeOrdersBaseData] a   \r\n");
                    sql.Append("  		left join   \r\n");
                    sql.Append("  		(   \r\n");
                    sql.Append("  		   select * from [TDaysChangeOrdersBaseData]   \r\n");
                    sql.Append("  		)b on a.vcGroupId=b.vcGroupId and a.vcPartNo=b.vcPartNo  and a.iAutoId<>b.iAutoId   \r\n");
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
                sql.Append("  delete [TDaysChangeOrdersBaseData] where iAutoId in(   \r\n ");
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
        public void allInstall(string vcGroupId, string vcFluctuationRange, string userId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();

                sql.Append("update TDaysChangeOrdersBaseData set vcFluctuationRange='" + vcFluctuationRange + "',vcOperatorID='" + userId + "',dOperatorTime=GETDATE() where vcGroupId ='" + vcGroupId + "' \n");

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
                    StringBuilder strSql = new StringBuilder();
                    string vcGroupName = dt.Rows[i]["vcGroupName"] == System.DBNull.Value ? "" : dt.Rows[i]["vcGroupName"].ToString();
                    string vcPartNo = dt.Rows[i]["vcPartNo"] == System.DBNull.Value ? "" : dt.Rows[i]["vcPartNo"].ToString();
                    //string vcFluctuationRange = dt.Rows[i]["vcFluctuationRange"] == System.DBNull.Value ? "" : dt.Rows[i]["vcFluctuationRange"].ToString();

                    strSql.AppendLine("  declare @isExist int =0;   ");
                    strSql.AppendLine("  select @isExist=COUNT(*) from TDaysChangeOrdersBaseData where vcPartNo='" + vcPartNo + "' ");
                    strSql.AppendLine("     ");
                    strSql.AppendLine("  if @isExist>0   ");
                    strSql.AppendLine("  begin   ");
                    strSql.AppendLine("  		update TDaysChangeOrdersBaseData set vcGroupId=(select iAutoId from [dbo].[TGroup] where vcGroupName='" + vcGroupName.Trim() + "' ),  ");
                    strSql.AppendLine("  		vcOperatorID='" + strUserId + "',dOperatorTime=GETDATE() where  vcPartNo='" + vcPartNo + "' ;  ");
                    strSql.AppendLine("  end   ");
                    strSql.AppendLine("  else   ");
                    strSql.AppendLine("  begin   ");
                    strSql.AppendLine("  		insert into dbo.TDaysChangeOrdersBaseData ([vcGroupId], [vcPartNo],  ");
                    strSql.AppendLine("  		 vcOperatorID, dOperatorTime )    ");
                    strSql.AppendLine("  		values   ");
                    strSql.AppendLine("  		((select iAutoId from [dbo].[TGroup] where vcGroupName='" + vcGroupName.Trim() + "' ),'" + vcPartNo + "','" + strUserId + "',GETDATE()) ;   ");
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
        /// <summary>
        /// 新增组别
        /// </summary>
        /// <param name="vcGroupName"></param>
        /// <param name="vcDefinition"></param>
        /// <param name="userId"></param>
        public void addZbConfirm(string vcGroupName, string vcDefinition, string userId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();

                sql.Append("insert into TGroup ( [vcGroupName], [vcDefinition], [vcOperatorID], [dOperatorTime]) values ('" + vcGroupName + "','" + vcDefinition + "','" + userId + "',getdate()) \n");

                excute.ExcuteSqlWithStringOper(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 新增按钮
        /// </summary>
        /// <param name="vcGroupId"></param>
        /// <param name="vcPartNo"></param>
        /// <param name="vcFluctuationRange"></param>
        /// <param name="userId"></param>
        public void addConfirm(string vcGroupId, string vcPartNo, string vcFluctuationRange, string userId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();

                sql.Append(" insert TDaysChangeOrdersBaseData ( [vcGroupId], [vcPartNo], [vcFluctuationRange], [vcOperatorID], [dOperatorTime] ) \n");
                sql.Append(" values ('" + vcGroupId + "','" + vcPartNo + "','" + vcFluctuationRange + "','" + userId + "',getdate()) \n");
                excute.ExcuteSqlWithStringOper(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 通过组名验证是否存在
        /// </summary>
        /// <param name="groupResult"></param>
        /// <returns></returns>
        public DataTable GetGroupNameDt()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("  select vcGroupName from [dbo].[TGroup]  ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 通过组别名称验证是否存在 true 存在
        /// </summary>
        /// <param name="vcGroupName"></param>
        /// <returns></returns>
        public bool CheckGroup(string vcGroupName)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("  select vcGroupName from [dbo].[TGroup] where vcGroupName='"+ vcGroupName + "'  ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString()).Rows.Count>0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 通过vcGroupID和品番验证是否存在
        /// </summary>
        /// <param name="vcGroupId"></param>
        /// <param name="vcPartNo"></param>
        /// <returns></returns>
        public bool CheckGroupbyGroupIdAndvcPartNo(string vcGroupId, string vcPartNo)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("  select * from [TDaysChangeOrdersBaseData] where vcGroupId='" + vcGroupId + "' and vcPartNo ='" + vcPartNo + "'  ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString()).Rows.Count > 0;
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
                        sql.Append("insert into [TGroup] ( vcGroupName, vcDefinition, vcFluctuationRange, vcOperatorID, dOperatorTime)  \n");
                        sql.Append(" values (  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcGroupName"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcDefinition"], false) + ",  \r\n");
                        sql.Append(getSqlValue(listInfoData[i]["vcFluctuationRange"].ToString().ToUpper(), false) + ",  \r\n");
                        sql.Append("   '" + userId + "', GETDATE() \r\n");
                        sql.Append(" );  \r\n");
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);

                        sql.Append("  update TGroup set    \r\n");
                        sql.Append("  vcDefinition=" + getSqlValue(listInfoData[i]["vcDefinition"].ToString().ToUpper(), false) + "   \r\n");
                        sql.Append("  ,vcFluctuationRange=" + getSqlValue(listInfoData[i]["vcFluctuationRange"].ToString().ToUpper(), false) + "   \r\n");
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
                    sql.Append("  	select a.vcGroupName+';' from   \r\n");
                    sql.Append("  	(   \r\n");
                    sql.Append("  		select distinct a.vcGroupName from [TGroup] a   \r\n");
                    sql.Append("  		left join   \r\n");
                    sql.Append("  		(   \r\n");
                    sql.Append("  		   select * from [TGroup]   \r\n");
                    sql.Append("  		)b on a.vcGroupName=b.vcGroupName   and a.iAutoId<>b.iAutoId   \r\n");
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
                sql.Append("  delete [TGroup] where iAutoId in(   \r\n ");
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

        public DataTable GetSubPartByGroup(List<Dictionary<string, object>> listInfoData)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("    select * from TDaysChangeOrdersBaseData where vcGroupId in  (     ");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    if (i != 0)
                        strSql.Append(",");
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                    strSql.Append(iAutoId);
                }
                strSql.Append("  )   \r\n ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
