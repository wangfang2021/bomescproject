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
    public class FS0807_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 检索所有的供应商供下拉框选择
        public DataTable getAllSupplier()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select vcSupplier_id as vcValue,vcSupplier_id+':'+vcSupplier_name as vcName from TSupplier  \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 检索
        public DataTable Search(string vcGQ, string vcSupplier_id, string vcSHF, string vcPart_id, string vcCarType, string vcTimeFrom, string vcTimeTo)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select t1.*,t3.vcName as vcSHFName,t2.vcName as vcGQName,  \n");
                strSql.Append("t1.vcSupplier_id+':'+t4.vcSupplier_name as vcSupplier_name,'0' as vcModFlag,'0' as vcAddFlag   \n");
                strSql.Append("from TEDTZPartsNoMaster t1  \n");
                strSql.Append("left join (select vcValue,vcName from TCode where vcCodeId='C017') t2 on t1.vcGQ=t2.vcValue  \n");
                strSql.Append("left join (select vcValue,vcName from TCode where vcCodeId='C018') t3 on t1.vcSHF=t3.vcValue  \n");
                strSql.Append("left join (select vcSupplier_id,vcSupplier_name from TSupplier) t4 on t1.vcSupplier_id=t4.vcSupplier_id  \n");
                strSql.Append("where 1=1  \n");
                if (vcGQ != "" && vcGQ!=null)
                    strSql.Append("and isnull(t1.vcGQ,'') like '%" + vcGQ + "%'   \n");
                if(vcSupplier_id!="" && vcSupplier_id!=null)
                    strSql.Append("and isnull(t1.vcSupplier_id,'') like '%" + vcSupplier_id + "%'  \n");
                if(vcSHF!="" && vcSHF!=null)
                    strSql.Append("and isnull(t1.vcSHF,'') like '%" + vcSHF + "%'   \n");
                if(vcPart_id!="" && vcPart_id!=null)
                    strSql.Append("and isnull(t1.vcPart_id,'') like '%" + vcPart_id + "%'  \n");
                if(vcCarType!="" && vcCarType!=null)
                    strSql.Append("and isnull(t1.vcCarType,'') like '%" + vcCarType + "%'   \n");
                if (vcTimeFrom == "" || vcTimeFrom == null)
                    vcTimeFrom = "2001/01/01";
                if (vcTimeTo == "" || vcTimeTo == null)
                    vcTimeTo = "2099/12/31";
                strSql.Append("and t1.dTimeFrom >= '" +vcTimeFrom + "' and t1.dTimeTo <= '" + vcTimeTo + "'  \n");
                strSql.Append("order by t1.vcPart_id,t1.vcSHF,t1.dTimeFrom   \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bmodflag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool baddflag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑

                    //标识说明
                    //默认  bmodflag:false  baddflag:false
                    //新增  bmodflag:true   baddflag:true
                    //修改  bmodflag:true   baddflag:false

                    if (baddflag == true)
                    {//新增
                        #region insert sql
                        sql.Append("INSERT INTO [TEDTZPartsNoMaster]  \n");
                        sql.Append("           ([vcPart_id]  \n");
                        sql.Append("           ,[dTimeFrom]  \n");
                        sql.Append("           ,[dTimeTo]  \n");
                        sql.Append("           ,[vcBZPlant]  \n");
                        sql.Append("           ,[vcSHF]  \n");
                        sql.Append("           ,[vcGQ]  \n");
                        sql.Append("           ,[vcCarType]  \n");
                        sql.Append("           ,[vcSupplier_id]  \n");
                        sql.Append("           ,[vcSR]  \n");
                        sql.Append("           ,[vcKanBanNo]  \n");
                        sql.Append("           ,[iContainerQuantity]  \n");
                        sql.Append("           ,[vcPartNameEn]  \n");
                        sql.Append("           ,[vcPartNameCn]  \n");
                        sql.Append("           ,[vcInProcess]  \n");
                        sql.Append("           ,[vcTGProcess]  \n");
                        sql.Append("           ,[vcPreProcess]  \n");
                        sql.Append("           ,[vcPreProcessPassTime]  \n");
                        sql.Append("           ,[vcInProcessSendTime]  \n");
                        //sql.Append("           ,[vcPhotoPath]  \n");
                        sql.Append("           ,[vcRemark1]  \n");
                        sql.Append("           ,[vcRemark2]  \n");
                        sql.Append("           ,[vcOperatorID]  \n");
                        sql.Append("           ,[dOperatorTime])  \n");
                        sql.Append("     VALUES  \n");
                        sql.Append("           ('" + listInfoData[i]["vcPart_id"].ToString() + "'  \n");
                        sql.Append("           ,'" + listInfoData[i]["dTimeFrom"].ToString() + "'  \n");
                        sql.Append("           ,'" + listInfoData[i]["dTimeTo"].ToString() + "'  \n");
                        sql.Append("           ,'" + listInfoData[i]["vcBZPlant"].ToString() + "'  \n");
                        sql.Append("           ,'" + listInfoData[i]["vcSHF"].ToString() + "'  \n");
                        sql.Append("           ,'" + listInfoData[i]["vcGQ"].ToString() + "'  \n");
                        sql.Append("           ,'" + listInfoData[i]["vcCarType"].ToString() + "'  \n");
                        sql.Append("           ,'" + listInfoData[i]["vcSupplier_id"].ToString() + "'  \n");
                        sql.Append("           ,'" + listInfoData[i]["vcSR"].ToString() + "'  \n");
                        sql.Append("           ,'" + listInfoData[i]["vcKanBanNo"].ToString() + "'  \n");
                        sql.Append("           ," + listInfoData[i]["iContainerQuantity"].ToString() + "  \n");
                        sql.Append("           ,'" + listInfoData[i]["vcPartNameEn"].ToString() + "'  \n");
                        sql.Append("           ,'" + listInfoData[i]["vcPartNameCn"].ToString() + "'  \n");
                        sql.Append("           ,'" + listInfoData[i]["vcInProcess"].ToString() + "'  \n");
                        sql.Append("           ,'" + listInfoData[i]["vcTGProcess"].ToString() + "'  \n");
                        sql.Append("           ,'" + listInfoData[i]["vcPreProcess"].ToString() + "'  \n");
                        sql.Append("           ,'" + listInfoData[i]["vcPreProcessPassTime"].ToString() + "'  \n");
                        sql.Append("           ,'" + listInfoData[i]["vcInProcessSendTime"].ToString() + "'  \n");
                        //sql.Append("           ,'"+listInfoData[i]["vcPhotoPath"].ToString()+"'  \n");
                        sql.Append("           ,'" + listInfoData[i]["vcRemark1"].ToString() + "'  \n");
                        sql.Append("           ,'" + listInfoData[i]["vcRemark2"].ToString() + "'  \n");
                        sql.Append("           ,'" + strUserId + "'  \n");
                        sql.Append("           ,getdate())  \n");
                        #endregion
                    }
                    else if (baddflag == false && bmodflag == true)
                    {//修改
                        #region modify sql
                        string iAutoId = listInfoData[i]["iAutoId"].ToString();
                        sql.Append("UPDATE [TEDTZPartsNoMaster]  \n");
                        sql.Append("   SET   \n");
                        //sql.Append("     [vcPart_id]  \n");
                        //sql.Append("     [dTimeFrom]  \n");
                        sql.Append("       [dTimeTo] = '" + listInfoData[i]["dTimeTo"].ToString() + "'  \n");
                        sql.Append("      ,[vcBZPlant] = '" + listInfoData[i]["vcBZPlant"].ToString() + "'  \n");
                        //sql.Append("     [vcSHF]  \n");
                        sql.Append("      ,[vcGQ] = '" + listInfoData[i]["vcGQ"].ToString() + "'  \n");
                        sql.Append("      ,[vcCarType] = '" + listInfoData[i]["vcCarType"].ToString() + "'  \n");
                        sql.Append("      ,[vcSupplier_id] = '" + listInfoData[i]["vcSupplier_id"].ToString() + "'  \n");
                        sql.Append("      ,[vcSR] = '" + listInfoData[i]["vcSR"].ToString() + "'  \n");
                        sql.Append("      ,[vcKanBanNo] = '" + listInfoData[i]["vcKanBanNo"].ToString() + "'  \n");
                        sql.Append("      ,[iContainerQuantity] = " + listInfoData[i]["iContainerQuantity"].ToString() + "  \n");
                        sql.Append("      ,[vcPartNameEn] = '" + listInfoData[i]["vcPartNameEn"].ToString() + "'  \n");
                        sql.Append("      ,[vcPartNameCn] = '" + listInfoData[i]["vcPartNameCn"].ToString() + "'  \n");
                        sql.Append("      ,[vcInProcess] = '" + listInfoData[i]["vcInProcess"].ToString() + "'  \n");
                        sql.Append("      ,[vcTGProcess] = '" + listInfoData[i]["vcTGProcess"].ToString() + "'  \n");
                        sql.Append("      ,[vcPreProcess] = '" + listInfoData[i]["vcPreProcess"].ToString() + "'  \n");
                        sql.Append("      ,[vcPreProcessPassTime] = '" + listInfoData[i]["vcPreProcessPassTime"].ToString() + "'  \n");
                        sql.Append("      ,[vcInProcessSendTime] = '" + listInfoData[i]["vcInProcessSendTime"].ToString() + "'  \n");
                        //sql.Append("     [vcPhotoPath]  \n");
                        sql.Append("      ,[vcRemark1] = '" + listInfoData[i]["vcRemark1"].ToString() + "'  \n");
                        sql.Append("      ,[vcRemark2] = '" + listInfoData[i]["vcRemark2"].ToString() + "'  \n");
                        sql.Append("      ,[vcOperatorID] = '" + strUserId + "'  \n");
                        sql.Append("      ,[dOperatorTime] = getdate()  \n");
                        sql.Append(" WHERE iAutoId=" + iAutoId + "  \n");
                        #endregion
                    }
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
        #endregion

        #region 删除
        public void Del(List<Dictionary<string, Object>> checkedInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < checkedInfoData.Count; i++)
                {
                    string iAutoId = checkedInfoData[i]["iAutoId"].ToString();
                    sql.Append("delete from TEDTZPartsNoMaster where iAutoId=" + iAutoId + "   \n");
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
        #endregion

        #region 品番+开始时间+收货方 不能重复
        public int RepeatCheck(string strPart_id, string strTimeFrom, string strSHF)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select COUNT(1) from TEDTZPartsNoMaster   \n");
                sql.Append("where vcPart_id='" + strPart_id + "' and dTimeFrom='" + strTimeFrom + "' and vcSHF='" + strSHF + "'   \n");
                return excute.ExecuteScalar(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        public int DateRegionCheck(string strPart_id, string strSHF, string strTimeFrom, string strTimeTo,string strMode,string strAutoId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select COUNT(1) from TEDTZPartsNoMaster   \n");
                sql.Append("where vcPart_id='" + strPart_id + "' and vcSHF='" + strSHF + "'    \n");
                sql.Append("and (('" + strTimeFrom + "'>=dTimeFrom and '" + strTimeFrom + "'<= dTimeTo) or  \n");
                sql.Append("('" + strTimeTo + "'>= dTimeFrom and '" + strTimeTo + "'<= dTimeTo)  )  \n");
                if(strMode=="mod")
                {
                    sql.Append("and iAutoId<>'" + strAutoId + "'  \n");
                }
                
                return excute.ExecuteScalar(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetName(string kind)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                if (kind == "gq")
                    sql.Append("select vcName from TCode where vcCodeId='C017'");
                else if (kind == "shf")
                    sql.Append("select vcMeaning as vcName  from TCode where vcCodeId='C018'");
                else if (kind == "supplier")
                    sql.Append("select vcSupplier_name as vcName from TSupplier");
                else if(kind=="bzplant")
                    sql.Append("select vcMeaning as vcName from TCode where vcCodeId='C023'");

                return excute.ExcuteSqlWithSelectToDT(sql.ToString());

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #region 导入后保存
        public void importSave(DataTable dt, string strUserId,ref string strErrorName)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("DELETE FROM [TEDTZPartsNoMaster_Temp] where vcOperatorID='" + strUserId + "' \n");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    #region 插入临时表
                    sql.Append("INSERT INTO [TEDTZPartsNoMaster_Temp]    \n");
                    sql.Append("           ([vcPart_id]    \n");
                    sql.Append("           ,[dTimeFrom]    \n");
                    sql.Append("           ,[dTimeTo]    \n");
                    sql.Append("           ,[vcBZPlant]    \n");
                    sql.Append("           ,[vcSHF]    \n");
                    sql.Append("           ,[vcGQ]    \n");
                    sql.Append("           ,[vcCarType]    \n");
                    sql.Append("           ,[vcSupplier_id]    \n");
                    sql.Append("           ,[vcSR]    \n");
                    sql.Append("           ,[vcKanBanNo]    \n");
                    sql.Append("           ,[iContainerQuantity]    \n");
                    sql.Append("           ,[vcPartNameEn]    \n");
                    sql.Append("           ,[vcPartNameCn]    \n");
                    sql.Append("           ,[vcInProcess]    \n");
                    sql.Append("           ,[vcTGProcess]    \n");
                    sql.Append("           ,[vcPreProcess]    \n");
                    sql.Append("           ,[vcPreProcessPassTime]    \n");
                    sql.Append("           ,[vcInProcessSendTime]    \n");
                    sql.Append("           ,[vcPhotoPath]    \n");
                    sql.Append("           ,[vcRemark1]    \n");
                    sql.Append("           ,[vcRemark2]    \n");
                    sql.Append("           ,[vcOperatorID]    \n");
                    sql.Append("           ,[dOperatorTime])    \n");
                    sql.Append("     VALUES    \n");
                    sql.Append("           ('"+dt.Rows[i]["vcPart_id"].ToString()+"'    \n");
                    sql.Append("           ,nullif('" + dt.Rows[i]["dTimeFrom"].ToString() + "','')    \n");
                    sql.Append("           ,nullif('" + dt.Rows[i]["dTimeTo"].ToString() + "','')    \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcBZPlant"].ToString() + "'    \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcSHF"].ToString() + "'    \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcGQ"].ToString() + "'    \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcCarType"].ToString() + "'    \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcSupplier_id"].ToString() + "'    \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcSR"].ToString() + "'    \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcKanBanNo"].ToString() + "'    \n");
                    sql.Append("           ,nullif('" + dt.Rows[i]["iContainerQuantity"].ToString() + "','')    \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcPartNameEn"].ToString() + "'    \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcPartNameCn"].ToString() + "'    \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcInProcess"].ToString() + "'    \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcTGProcess"].ToString() + "'    \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcPreProcess"].ToString() + "'    \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcPreProcessPassTime"].ToString() + "'    \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcInProcessSendTime"].ToString() + "'    \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcPhotoPath"].ToString() + "'    \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcRemark1"].ToString() + "'    \n");
                    sql.Append("           ,'" + dt.Rows[i]["vcRemark2"].ToString() + "'    \n");
                    sql.Append("           ,'"+strUserId+"'    \n");
                    sql.Append("           ,getdate())    \n");
                    #endregion
                }
                #region insert sql
                sql.Append("INSERT INTO [TEDTZPartsNoMaster]    \n");
                sql.Append("           ([vcPart_id]    \n");
                sql.Append("           ,[dTimeFrom]    \n");
                sql.Append("           ,[dTimeTo]    \n");
                sql.Append("           ,[vcBZPlant]    \n");
                sql.Append("           ,[vcSHF]    \n");
                sql.Append("           ,[vcGQ]    \n");
                sql.Append("           ,[vcCarType]    \n");
                sql.Append("           ,[vcSupplier_id]    \n");
                sql.Append("           ,[vcSR]    \n");
                sql.Append("           ,[vcKanBanNo]    \n");
                sql.Append("           ,[iContainerQuantity]    \n");
                sql.Append("           ,[vcPartNameEn]    \n");
                sql.Append("           ,[vcPartNameCn]    \n");
                sql.Append("           ,[vcInProcess]    \n");
                sql.Append("           ,[vcTGProcess]    \n");
                sql.Append("           ,[vcPreProcess]    \n");
                sql.Append("           ,[vcPreProcessPassTime]    \n");
                sql.Append("           ,[vcInProcessSendTime]    \n");
                sql.Append("           ,[vcPhotoPath]    \n");
                sql.Append("           ,[vcRemark1]    \n");
                sql.Append("           ,[vcRemark2]    \n");
                sql.Append("           ,[vcOperatorID]    \n");
                sql.Append("           ,[dOperatorTime])    \n");
                sql.Append("SELECT t1.[vcPart_id]    \n");
                sql.Append("      ,t1.[dTimeFrom]    \n");
                sql.Append("      ,t1.[dTimeTo]    \n");
                sql.Append("      ,t1.[vcBZPlant]    \n");
                sql.Append("      ,t1.[vcSHF]    \n");
                sql.Append("      ,t1.[vcGQ]    \n");
                sql.Append("      ,t1.[vcCarType]    \n");
                sql.Append("      ,t1.[vcSupplier_id]    \n");
                sql.Append("      ,t1.[vcSR]    \n");
                sql.Append("      ,t1.[vcKanBanNo]    \n");
                sql.Append("      ,t1.[iContainerQuantity]    \n");
                sql.Append("      ,t1.[vcPartNameEn]    \n");
                sql.Append("      ,t1.[vcPartNameCn]    \n");
                sql.Append("      ,t1.[vcInProcess]    \n");
                sql.Append("      ,t1.[vcTGProcess]    \n");
                sql.Append("      ,t1.[vcPreProcess]    \n");
                sql.Append("      ,t1.[vcPreProcessPassTime]    \n");
                sql.Append("      ,t1.[vcInProcessSendTime]    \n");
                sql.Append("      ,t1.[vcPhotoPath]    \n");
                sql.Append("      ,t1.[vcRemark1]    \n");
                sql.Append("      ,t1.[vcRemark2]    \n");
                sql.Append("      ,t1.[vcOperatorID]    \n");
                sql.Append("      ,t1.[dOperatorTime]    \n");
                sql.Append("  FROM [TEDTZPartsNoMaster_Temp] t1    \n");
                sql.Append("  left join TEDTZPartsNoMaster t2     \n");
                sql.Append("  on t1.vcPart_id=t2.vcPart_id and t1.vcSHF=t2.vcSHF and t1.dTimeFrom=t2.dTimeFrom    \n");
                sql.Append("  where t2.iAutoId is null and t1.vcOperatorID='" + strUserId + "'    \n");
                #endregion

                #region update sql
                sql.Append("UPDATE t2    \n");
                sql.Append("   SET t2.[dTimeTo] = t1.[dTimeTo]    \n");
                sql.Append("      ,t2.[vcBZPlant] = t1.[vcBZPlant]    \n");
                sql.Append("      ,t2.[vcGQ] = t1.[vcGQ]    \n");
                sql.Append("      ,t2.[vcCarType] = t1.[vcCarType]    \n");
                sql.Append("      ,t2.[vcSupplier_id] = t1.[vcSupplier_id]    \n");
                sql.Append("      ,t2.[vcSR] = t1.[vcSR]    \n");
                sql.Append("      ,t2.[vcKanBanNo] = t1.[vcKanBanNo]    \n");
                sql.Append("      ,t2.[iContainerQuantity] = t1.[iContainerQuantity]    \n");
                sql.Append("      ,t2.[vcPartNameEn] = t1.[vcPartNameEn]    \n");
                sql.Append("      ,t2.[vcPartNameCn] = t1.[vcPartNameCn]    \n");
                sql.Append("      ,t2.[vcInProcess] = t1.[vcInProcess]    \n");
                sql.Append("      ,t2.[vcTGProcess] = t1.[vcTGProcess]    \n");
                sql.Append("      ,t2.[vcPreProcess] = t1.[vcPreProcess]    \n");
                sql.Append("      ,t2.[vcPreProcessPassTime] = t1.[vcPreProcessPassTime]    \n");
                sql.Append("      ,t2.[vcInProcessSendTime] = t1.[vcInProcessSendTime]    \n");
                sql.Append("      ,t2.[vcPhotoPath] = t1.[vcPhotoPath]    \n");
                sql.Append("      ,t2.[vcRemark1] = t1.[vcRemark1]    \n");
                sql.Append("      ,t2.[vcRemark2] = t1.[vcRemark2]    \n");
                sql.Append("      ,t2.[vcOperatorID] = t1.[vcOperatorID]    \n");
                sql.Append("      ,t2.[dOperatorTime] = t1.[dOperatorTime]    \n");
                sql.Append("from    \n");
                sql.Append("(select * from [TEDTZPartsNoMaster_Temp])t1    \n");
                sql.Append("inner join TEDTZPartsNoMaster t2     \n");
                sql.Append("on t1.vcPart_id=t2.vcPart_id and t1.vcSHF=t2.vcSHF and t1.dTimeFrom=t2.dTimeFrom    \n");
                sql.Append("where t1.vcOperatorID='" + strUserId + "'     \n");
                #endregion

                if (sql.Length > 0)
                {
                    //以下追加验证数据库中是否存在品番区间重叠判断，如果存在则终止提交
                    sql.Append("  	  DECLARE @errorName varchar(50)      \r\n");
                    sql.Append("  	  set @errorName=''      \r\n");
                    sql.Append("  	  set @errorName=(      \r\n");
                    sql.Append("  	  	select vcName +';' from      \r\n");
                    sql.Append("  	  	(      \r\n");
                    sql.Append("  	  		select distinct a.vcPart_id,a.vcSHF from    \r\n");
                    sql.Append("  			(   \r\n");
                    sql.Append("  				select * from TEDTZPartsNoMaster a   \r\n");
                    sql.Append("  			) a      \r\n");
                    sql.Append("  	  		left join      \r\n");
                    sql.Append("  	  		(      \r\n");
                    sql.Append("  	  		   select * from TEDTZPartsNoMaster      \r\n");
                    sql.Append("  	  		)b on a.vcPart_id=b.vcPart_id and a.vcSHF=b.vcSHF and a.iAutoId<>b.iAutoId      \r\n");
                    sql.Append("  	  		   and       \r\n");
                    sql.Append("  	  		   (      \r\n");
                    sql.Append("  	  			   (a.dTimeFrom>=b.dBegin and a.dTimeFrom<=b.dEnd)      \r\n");
                    sql.Append("  	  			   or      \r\n");
                    sql.Append("  	  			   (a.dTimeTo>=b.dBegin and a.dTimeTo<=b.dEnd)      \r\n");
                    sql.Append("  	  		   )      \r\n");
                    sql.Append("  	  		where b.iAutoId is not null      \r\n");
                    sql.Append("  	  	)a for xml path('')      \r\n");
                    sql.Append("  	  )      \r\n");
                    sql.Append("  	         \r\n");
                    sql.Append("  	  if @errorName<>''      \r\n");
                    sql.Append("  	  begin      \r\n");
                    sql.Append("  	    select CONVERT(int,'-->'+@errorName+'<--')      \r\n");
                    sql.Append("  	  end       \r\n");

                    excute.ExcuteSqlWithStringOper(sql.ToString());
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("-->") != -1)
                {//主动判断抛出的异常
                    int startIndex = ex.Message.IndexOf("-->");
                    int endIndex = ex.Message.LastIndexOf("<--");
                    strErrorName = ex.Message.Substring(startIndex + 3, endIndex - startIndex - 3);
                }
                else
                    throw ex;
            }
        }
        #endregion
    }
}
