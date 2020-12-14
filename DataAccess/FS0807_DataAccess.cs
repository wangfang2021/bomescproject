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

        #region 绑定工区
        public DataTable bindGQ()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select '1' as vcGQ_value,'工区1' as vcGQ_text   \n");
                strSql.Append("union  \n");
                strSql.Append("select '2' as vcGQ_value,'工区2' as vcGQ_text  \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 绑定供应商
        public DataTable bindSupplier()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select '1' as vcSupplier_value,'供应商1' as vcSupplier_text  \n");
                strSql.Append("union  \n");
                strSql.Append("select '2' as vcSupplier_value,'供应商2' as vcSupplier_text  \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 绑定收货方
        public DataTable bindSHF()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select '1' as vcSHF_value,'收货方1' as vcSHF_text   \n");
                strSql.Append("union  \n");
                strSql.Append("select '2' as vcSHF_value,'收货方2' as vcSHF_text \n");
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
                strSql.Append("select *,'0' as vcModFlag,'0' as vcAddFlag from TEDTZPartsNoMaster  \n");
                strSql.Append("where isnull(vcGQ,'') like '%" + vcGQ + "%' and isnull(vcSupplier_id,'') like '%" + vcSupplier_id + "%'  \n");
                strSql.Append("and isnull(vcSHF,'') like '%" + vcSHF + "%' and isnull(vcPart_id,'') like '%" + vcPart_id + "%'  \n");
                strSql.Append("and isnull(vcCarType,'') like '%" + vcCarType + "%'   \n");
                strSql.Append("and vcTimeFrom >= '" + (vcTimeFrom==""?"20010101":vcTimeFrom) + "' and vcTimeTo <= '" + (vcTimeTo==""?"20991231":vcTimeTo) + "'  \n");
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

                    if (baddflag == true && bmodflag == true)
                    {//新增
                        #region insert sql
                        sql.Append("INSERT INTO [TEDTZPartsNoMaster]  \n");
                        sql.Append("           ([vcPart_id]  \n");
                        sql.Append("           ,[vcTimeFrom]  \n");
                        sql.Append("           ,[vcTimeTo]  \n");
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
                        sql.Append("           ,'" + listInfoData[i]["vcTimeFrom"].ToString() + "'  \n");
                        sql.Append("           ,'" + listInfoData[i]["vcTimeTo"].ToString() + "'  \n");
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
                        //sql.Append("     [vcTimeFrom]  \n");
                        sql.Append("       [vcTimeTo] = '" + listInfoData[i]["vcTimeTo"].ToString() + "'  \n");
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

        #region 取出所有ED品番信息
        public DataTable GetPartsInfo()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select * from TEDTZPartsNoMaster  \n");
            return excute.ExcuteSqlWithSelectToDT(sql.ToString());
        }
        #endregion
    }
}
