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
    }
}
