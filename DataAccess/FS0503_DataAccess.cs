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
    public class FS0503_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

      /// <summary>
      /// 获取箱种目的
      /// </summary>
      /// <returns></returns>
        public DataTable GetBoxType()
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
               
                strSql.AppendLine("  select vcBoxType as vcValue,vcBoxType as vcName from(select distinct vcBoxType from THeZiManage) a  ");
                  
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
        public DataTable Search(string vcSupplier_id, string vcWorkArea, string vcState, string vcPartNo, string vcCarType, string dExpectDeliveryDate, string strUserId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                //strSql.AppendLine("  select [iAutoId],[vcPackingPlant] ,[vcReceiver], [dSynchronizationDate], b.vcName as [vcState], [vcPartNo], [dUseStartDate],[dUserEndDate], [vcPartName],   ");
                //strSql.AppendLine("  [vcCarType],c.vcName as [vcOEOrSP], [vcSupplier_id], [vcWorkArea], [dExpectDeliveryDate], [vcExpectIntake],   ");
                //strSql.AppendLine("  [vcIntake], [vcBoxMaxIntake], [vcBoxType], [vcLength], [vcWide], [vcHeight], [vcEmptyWeight],    ");
                //strSql.AppendLine("  [vcUnitNetWeight], [dSendDate], [dReplyDate], [dAdmitDate], [dWeaveDate], [vcMemo], vcImageRoutes,    ");
                //strSql.AppendLine("  [vcInserter], [vcInserterDate],[vcFactoryOperatorID], [dFactoryOperatorTime],   ");
                //strSql.AppendLine("  [vcOperatorID], [dOperatorTime],'0' as vcModFlag,'0' as vcAddFlag from [dbo].[THeZiManage] a   ");
                //strSql.AppendLine("  left join (select vcValue,vcName from TCode where vcCodeId='C012') c on a.vcOEOrSP = c.vcValue    ");
                //strSql.AppendLine("  left join (select vcValue,vcName from TCode where vcCodeId='C033') b on a.vcState = b.vcValue  where 1=1 and a.vcState in (select vcValue from TCode where vcCodeId='C034')   ");
                strSql.AppendLine("    select a.[iAutoId],a.[vcPackingPlant] ,a.[vcReceiver], a.[dSynchronizationDate], b.vcName as [vcState],    ");
                strSql.AppendLine("     a.[vcPartNo], a.[dUseStartDate],a.[dUserEndDate], a.[vcPartName],       "); 
                strSql.AppendLine("     a.[vcCarType],c.vcName as [vcOEOrSP], a.[vcSupplier_id],     ");
                strSql.AppendLine("     case when isnull(a.vcIsEdit,'0')='1' then d.vcWorkArea else a.vcWorkArea end as [vcWorkArea],    ");
                strSql.AppendLine("     a.[dExpectDeliveryDate], a.[vcExpectIntake],     ");
                strSql.AppendLine("      case when isnull(a.vcIsEdit,'0')='1' then d.vcIntake else a.vcIntake end as [vcIntake],    ");
                strSql.AppendLine("      case when isnull(a.vcIsEdit,'0')='1' then d.vcIntake else a.vcIntake end as [vcBoxMaxIntake],    ");
                strSql.AppendLine("      case when isnull(a.vcIsEdit,'0')='1' then d.vcBoxType else a.vcBoxType end as [vcBoxType],    ");
                strSql.AppendLine("      case when isnull(a.vcIsEdit,'0')='1' then d.vcLength else a.vcLength end as [vcLength],    ");
                strSql.AppendLine("      case when isnull(a.vcIsEdit,'0')='1' then d.vcWide else a.vcWide end as [vcWide],     ");
                strSql.AppendLine("      case when isnull(a.vcIsEdit,'0')='1' then d.vcHeight else a.vcHeight end as[vcHeight],    ");
                strSql.AppendLine("      case when isnull(a.vcIsEdit,'0')='1' then d.vcEmptyWeight else a.vcEmptyWeight end as [vcEmptyWeight],       ");
                strSql.AppendLine("      case when isnull(a.vcIsEdit,'0')='1' then d.vcEmptyWeight else a.vcEmptyWeight end as [vcUnitNetWeight],    ");
                strSql.AppendLine("      a.[dSendDate], a.[dReplyDate], a.[dAdmitDate], a.[dWeaveDate], a.[vcMemo],     ");
                strSql.AppendLine("      case when isnull(a.vcIsEdit,'0')='1' then d.vcImageRoutes else a.vcImageRoutes end as vcImageRoutes,        ");
                strSql.AppendLine("     a.[vcInserter], a.[vcInserterDate],a.[vcFactoryOperatorID], a.[dFactoryOperatorTime],       ");
                strSql.AppendLine("     a.[vcOperatorID], a.[dOperatorTime],'0' as vcModFlag,'0' as vcAddFlag from [dbo].[THeZiManage] a       ");
                strSql.AppendLine("     left join (select vcValue,vcName from TCode where vcCodeId='C012') c on a.vcOEOrSP = c.vcValue        ");
                strSql.AppendLine("     left join (select vcValue,vcName from TCode where vcCodeId='C033') b on a.vcState = b.vcValue     ");
                strSql.AppendLine("     left join (select * from [THeZiManageTmp] where vcDelFlag='0') d on a.iAutoId =d.iAutoId    ");
                strSql.AppendLine("   where 1=1 and a.vcState in (select vcValue from TCode where vcCodeId='C034')     ");
                strSql.AppendLine("       ");
                strSql.AppendLine("       ");
                strSql.AppendLine("       ");


                if (vcSupplier_id.Length > 0)
                {
                    strSql.AppendLine("  and  a.vcSupplier_id  =  '" + vcSupplier_id + "' ");
                }
                if (vcWorkArea.Length > 0)
                {
                    strSql.AppendLine("  and  and case when isnull(a.vcIsEdit,'0')='1' then d.vcWorkArea   else a.vcWorkArea  like  '" + vcWorkArea + "%' ");
                }
                if (vcState.Length > 0)
                {
                    strSql.AppendLine("  and  a.vcState = '" + vcState + "' ");
                }
                if (vcPartNo.Length > 0)
                {
                    strSql.AppendLine("  and  a.vcPartNo like '" + vcPartNo + "%' ");
                }
                
                if (vcCarType.Length > 0)
                {
                    strSql.AppendLine("  and  a.vcCarType like '" + vcCarType + "%' ");
                }
                if (dExpectDeliveryDate.Length > 0)
                {
                    strSql.AppendLine("  and  CONVERT(varchar(10),  dExportDate,112) = '" + dExpectDeliveryDate.Replace("-", "").Replace("/", "") + "' ");
                }

                strSql.AppendLine("  order by a.vcState asc, dSendDate desc ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetCarType(string userId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("  select vcCarType as vcValue,vcCarType as vcName from(    ");
                strSql.AppendLine("    	select distinct isnull(vcCarType,'') as vcCarType from [THeZiManage] where vcSupplier_id='" + userId + "'   ");
                strSql.AppendLine("  ) t order by vcValue desc  ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetExpectDeliveryDate(string userId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("  select dExpectDeliveryDate as vcValue,dExpectDeliveryDate as vcName from(    ");
                strSql.AppendLine("    	select  distinct convert(varchar(10), dExpectDeliveryDate,120) as dExpectDeliveryDate from [THeZiManage] where vcSupplier_id='" + userId + "' and  vcState<>4   ");
                strSql.AppendLine("  ) t order by vcValue desc  ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetTaskNum1( string userId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("  select * from [dbo].[THeZiManage] where vcState='3' and vcSupplier_id='" + userId + "'  ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取工区
        /// </summary>
        /// <returns></returns>
        public DataTable GetWorkArea(string supplierId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("   select a.vcWorkArea  as vcValue,a.vcWorkArea as vcName from (   ");
                strSql.AppendLine("   select distinct vcWorkArea from [THeZiManage] where vcSupplier_id='"+ supplierId + "' and vcWorkArea is not null   ");
                strSql.AppendLine("   ) a   ");
                
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 通过选中一条数据获取出荷信息
        /// </summary>
        /// <param name="strIAutoId"></param>
        /// <returns></returns>
        public DataTable SearchByIAutoId(string strIAutoId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                //strSql.AppendLine("  select [iAutoId], [dSynchronizationDate], b.vcName as [vcState], [vcPartNo], [dUseStartDate], [vcPartName],   ");
                //strSql.AppendLine("  [vcCarType], [vcOEOrSP], [vcSupplier_id], [vcWorkArea], [dExpectDeliveryDate], [vcExpectIntake],   ");
                //strSql.AppendLine("  [vcIntake], [vcBoxMaxIntake], [vcBoxType], [vcLength], [vcWide], [vcHeight], [vcEmptyWeight],    ");
                //strSql.AppendLine("  [vcUnitNetWeight], [dSendDate], [dReplyDate], [dAdmitDate], [dWeaveDate], [vcMemo], vcImageRoutes,    ");
                //strSql.AppendLine("  [vcInserter], [vcInserterDate],[vcFactoryOperatorID], [dFactoryOperatorTime],   ");
                //strSql.AppendLine("  [vcOperatorID], [dOperatorTime],'0' as vcModFlag,'0' as vcAddFlag from [dbo].[THeZiManage] a   ");
                //strSql.AppendLine("  left join (select vcValue,vcName from TCode where vcCodeId='C033') b on a.vcState = b.vcValue  where iAutoId="+ strIAutoId + "   ");

                strSql.AppendLine("    select a.[iAutoId],a.[vcPackingPlant] ,a.[vcReceiver], a.[dSynchronizationDate], b.vcName as [vcState],    ");
                strSql.AppendLine("     a.[vcPartNo], a.[dUseStartDate],a.[dUserEndDate], a.[vcPartName],       ");
                strSql.AppendLine("     a.[vcCarType],c.vcName as [vcOEOrSP], a.[vcSupplier_id],     ");
                strSql.AppendLine("     case when isnull(a.vcIsEdit,'0')='1' then d.vcWorkArea else a.vcWorkArea end as [vcWorkArea],    ");
                strSql.AppendLine("     a.[dExpectDeliveryDate], a.[vcExpectIntake],     ");
                strSql.AppendLine("      case when isnull(a.vcIsEdit,'0')='1' then d.vcIntake else a.vcIntake end as [vcIntake],    ");
                strSql.AppendLine("      case when isnull(a.vcIsEdit,'0')='1' then d.vcIntake else a.vcIntake end as [vcBoxMaxIntake],    ");
                strSql.AppendLine("      case when isnull(a.vcIsEdit,'0')='1' then d.vcBoxType else a.vcBoxType end as [vcBoxType],    ");
                strSql.AppendLine("      case when isnull(a.vcIsEdit,'0')='1' then d.vcLength else a.vcLength end as [vcLength],    ");
                strSql.AppendLine("      case when isnull(a.vcIsEdit,'0')='1' then d.vcWide else a.vcWide end as [vcWide],     ");
                strSql.AppendLine("      case when isnull(a.vcIsEdit,'0')='1' then d.vcHeight else a.vcHeight end as[vcHeight],    ");
                strSql.AppendLine("      case when isnull(a.vcIsEdit,'0')='1' then d.vcEmptyWeight else a.vcEmptyWeight end as [vcEmptyWeight],       ");
                strSql.AppendLine("      case when isnull(a.vcIsEdit,'0')='1' then d.vcEmptyWeight else a.vcEmptyWeight end as [vcUnitNetWeight],    ");
                strSql.AppendLine("      a.[dSendDate], a.[dReplyDate], a.[dAdmitDate], a.[dWeaveDate], a.[vcMemo],     ");
                strSql.AppendLine("      case when isnull(a.vcIsEdit,'0')='1' then d.vcImageRoutes else a.vcImageRoutes end as vcImageRoutes,        ");
                strSql.AppendLine("     a.[vcInserter], a.[vcInserterDate],a.[vcFactoryOperatorID], a.[dFactoryOperatorTime],       ");
                strSql.AppendLine("     a.[vcOperatorID], a.[dOperatorTime],'0' as vcModFlag,'0' as vcAddFlag from [dbo].[THeZiManage] a       ");
                strSql.AppendLine("     left join (select vcValue,vcName from TCode where vcCodeId='C012') c on a.vcOEOrSP = c.vcValue        ");
                strSql.AppendLine("     left join (select vcValue,vcName from TCode where vcCodeId='C033') b on a.vcState = b.vcValue     ");
                strSql.AppendLine("     left join (select * from [THeZiManageTmp] where vcDelFlag='0') d on a.iAutoId =d.iAutoId   where a.iAutoId=" + strIAutoId + "  ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 承认操作
        /// </summary>
        /// <param name="listInfoData"></param>
        /// <param name="userId"></param>
        public void admit(List<Dictionary<string, object>> listInfoData, string userId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();

                sql.Append("update [THeZiManage] set  vcState='3', \n");
               
                sql.Append(" vcOperatorID='" + userId + "',dOperatorTime=GETDATE() where iAutoId in( \n");
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
        /// 退回操作
        /// </summary>
        /// <param name="listInfoData"></param>
        /// <param name="userId"></param>
        public void returnHandle(List<Dictionary<string, object>> listInfoData, string userId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();

                sql.Append("update [THeZiManage] set  vcState='4', \n");

                sql.Append(" vcOperatorID='" + userId + "',dOperatorTime=GETDATE() where iAutoId in( \n");
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
        /// 织入原单位
        /// </summary>
        /// <param name="listInfoData"></param>
        /// <param name="userId"></param>
        public void weaveHandle(List<Dictionary<string, object>> listInfoData, string userId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();

                //sql.Append("update [THeZiManage] set  vcState='3', \n");

                //sql.Append(" vcOperatorID='" + userId + "',dOperatorTime=GETDATE() where iAutoId in( \n");
                //for (int i = 0; i < listInfoData.Count; i++)
                //{
                //    if (i != 0)
                //        sql.Append(",");
                //    int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                //    sql.Append(iAutoId);
                //}
                //sql.Append("  )   \r\n ");
                excute.ExcuteSqlWithStringOper(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 荷姿展开
        /// </summary>
        /// <param name="listInfoData"></param>
        /// <param name="dExpectDeliveryDate"></param>
        /// <param name="userId"></param>
        public void hZZK(List<Dictionary<string, object>> listInfoData, string dExpectDeliveryDate, string userId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();

                sql.Append("update [THeZiManage] set  \n");
                if (dExpectDeliveryDate.Length > 0)
                {
                    sql.Append(" dExpectDeliveryDate='" + Convert.ToDateTime(dExpectDeliveryDate) + "', \n");
                }
               
                sql.Append(" vcOperatorID='" + userId + "',dOperatorTime=GETDATE() where iAutoId in( \n");
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
        /// 获取未发送的数据
        /// </summary>
        public DataTable GetTaskNum(string userId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("  select * from [dbo].[THeZiManage] where vcState='1' and vcSupplier_id='"+ userId + "'  ");

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
                StringBuilder sql = new StringBuilder();

                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bModFlag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool bAddFlag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    if (bAddFlag == true)
                    {//新增
                        
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                        sql.Append("   UPDATE [dbo].[THeZiManage]   \r\n");
                        sql.Append("      SET [vcExpectIntake] = " + getSqlValue(listInfoData[i]["vcExpectIntake"], false) + "   \r\n");
                       
                        sql.Append("         ,[dExpectDeliveryDate] = " + getSqlValue(listInfoData[i]["dExpectDeliveryDate"], true) + "   \r\n");
                        
                        sql.Append("         ,[vcMemo] =  " + getSqlValue(listInfoData[i]["vcMemo"], false) + "   \r\n");
                        sql.Append("  ,vcOperatorID='" + userId + "',dOperatorTime=GETDATE() \r\n");
                        sql.Append(" where iAutoId=" + iAutoId + " ;  \n");

                    }
                }

                if (sql.Length > 0)
                {
                    //以下追加验证数据库中是否存在品番区间重叠判断，如果存在则终止提交

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
                sql.Append("  delete [TOralTestManage] where iAutoId in(   \r\n ");
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
        public void allInstall(List<Dictionary<string, object>> listInfoData,  string vcIntake, string userId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();

                sql.Append("update [THeZiManage] set  \n");
                
                if (vcIntake.Length > 0)
                {
                    sql.Append(" vcIntake='" + vcIntake + "', \n");
                }
                sql.Append(" vcOperatorID='" + userId + "',dOperatorTime=GETDATE() where iAutoId in( \n");
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
        /// 导入 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="strUserId"></param>
        public void importSave(DataTable dt, string strUserId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //"vcPartNo", "vcIntake", "vcBoxMaxIntake", "vcBoxType", "vcLength", "vcWide", "vcHeight", "vcEmptyWeight", "vcUnitNetWeight", 
                    string vcPackingPlant = dt.Rows[i]["vcPackingPlant"] == System.DBNull.Value ? "" : dt.Rows[i]["vcPackingPlant"].ToString();
                    string vcReceiver = dt.Rows[i]["vcReceiver"] == System.DBNull.Value ? "" : dt.Rows[i]["vcReceiver"].ToString();
                    string vcPartNo = dt.Rows[i]["vcPartNo"] == System.DBNull.Value ? "" : dt.Rows[i]["vcPartNo"].ToString();
                    string vcIntake = dt.Rows[i]["vcIntake"] == System.DBNull.Value ? "" : dt.Rows[i]["vcIntake"].ToString();
                    string vcBoxMaxIntake = dt.Rows[i]["vcBoxMaxIntake"] == System.DBNull.Value ? "" : dt.Rows[i]["vcBoxMaxIntake"].ToString();
                    string vcBoxType = dt.Rows[i]["vcBoxType"] == System.DBNull.Value ? "" : dt.Rows[i]["vcBoxType"].ToString();
                    string vcLength = dt.Rows[i]["vcLength"] == System.DBNull.Value ? "" : dt.Rows[i]["vcLength"].ToString();
                    string vcWide = dt.Rows[i]["vcWide"] == System.DBNull.Value ? "" : dt.Rows[i]["vcWide"].ToString();
                    string vcHeight = dt.Rows[i]["vcHeight"] == System.DBNull.Value ? "" : dt.Rows[i]["vcHeight"].ToString();
                    string vcEmptyWeight = dt.Rows[i]["vcEmptyWeight"] == System.DBNull.Value ? "" : dt.Rows[i]["vcEmptyWeight"].ToString();
                    string vcUnitNetWeight = dt.Rows[i]["vcUnitNetWeight"] == System.DBNull.Value ? "" : dt.Rows[i]["vcUnitNetWeight"].ToString();
                    string vcSupplier = strUserId;
                    string vcWorkArea = dt.Rows[i]["vcWorkArea"] == System.DBNull.Value ? "" : dt.Rows[i]["vcWorkArea"].ToString();
                    strSql.AppendLine("   update [THeZiManage]  ");
                    strSql.AppendLine("    set vcWorkArea='"+ vcWorkArea + "', vcIntake ='" + vcIntake + "',  ");
                    strSql.AppendLine("    vcBoxMaxIntake='" + vcBoxMaxIntake + "',  ");
                    strSql.AppendLine("    vcBoxType='" + vcBoxType + "',  ");
                    strSql.AppendLine("    vcLength='" + vcLength + "',  ");
                    strSql.AppendLine("    vcWide='" + vcWide + "',  ");
                    strSql.AppendLine("    vcHeight='" + vcHeight + "',  ");
                    strSql.AppendLine("    vcEmptyWeight='" + vcEmptyWeight + "',  ");
                    strSql.AppendLine("   vcUnitNetWeight='" + vcUnitNetWeight +"', vcFactoryOperatorID = '" + strUserId + "', dFactoryOperatorTime = GETDATE()  ");
                    strSql.AppendLine("   where vcPackingPlant='" + vcPackingPlant + "' and vcReceiver='" + vcReceiver + "' and vcPartNo='" + vcPartNo + "' and vcSupplier_id='" + strUserId + "' and vcState in ('1','4') ");
                    
                   
                }
                if (strSql.Length>0)
                {
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

        /// <summary>
        /// 编辑确定按钮
        /// </summary>
        /// <param name="listInfo"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool editOk(dynamic dataForm, string userId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                //[vcSupplier_id], [vcWorkArea], [dBeginDate], [dEndDate],, 
                
                string iAutoId =  dataForm.iAutoId;
                string vcState = dataForm.vcState;

                if (vcState == "待回复")
                {
                    vcState = "1";
                } else if (vcState == "已回复") {
                    vcState = "2";
                }
                else if (vcState == "退回")
                {
                    vcState = "3";
                }
                else if (vcState == "已承认")
                {
                    vcState = "4";
                }
                string vcWorkArea = dataForm.vcWorkArea;
                string vcSupplier_id = dataForm.vcSupplier_id;
                string vcIntake = dataForm.vcIntake;
                string vcBoxMaxIntake =  dataForm.vcBoxMaxIntake;
                string vcBoxType =  dataForm.vcBoxType;
                string vcLength =  dataForm.vcLength;
                string vcWide =  dataForm.vcWide;
                string vcHeight =  dataForm.vcHeight;
                string vcEmptyWeight =  dataForm.vcEmptyWeight;
                string vcUnitNetWeight =  dataForm.vcUnitNetWeight;
                string vcImageRoutes =  dataForm.vcImageRoutes;
                //1   待回复  2 已回复   3退回4   已承认

                if (vcState == "1" || vcState == "3")
                {
                    strSql.AppendLine(" delete from  THeZiManageTmp where iAutoId=" + iAutoId + ";   ");
                    strSql.AppendLine("   insert into [THeZiManageTmp] (iAutoId,vcState,vcSupplier_id,vcWorkArea,vcIntake,vcBoxMaxIntake,   ");
                    strSql.AppendLine("   vcBoxType,vcLength,vcWide   ");
                    strSql.AppendLine("   ,vcHeight,vcEmptyWeight,vcUnitNetWeight,vcImageRoutes,vcDelFlag,[vcOperatorID], [dOperatorTime]) values   ");
                    strSql.AppendLine("    ( " + iAutoId + ",'" + vcState + "','" + vcSupplier_id + "','" + vcWorkArea + "','" + vcIntake + "','" + vcBoxMaxIntake + "',  ");
                    strSql.AppendLine("   '" + vcBoxType + "','" + vcLength + "','" + vcWide + "',   ");
                    strSql.AppendLine("   '" + vcHeight + "','" + vcEmptyWeight + "','" + vcUnitNetWeight + "','" + vcImageRoutes + "','0','" + userId + "',GETDATE()   ");
                    strSql.AppendLine("   );   ");
                    strSql.AppendLine("   update [dbo].[THeZiManage] set vcIsEdit='1' where iAutoId=" + iAutoId + ";   ");
                }
                else {
                    strSql.AppendLine("      update [dbo].[THeZiManage]          ");
                    strSql.AppendLine("      set vcIntake='" + vcIntake + "',          ");
                    strSql.AppendLine("      vcWorkArea='" + vcWorkArea + "',          ");
                    strSql.AppendLine("      vcBoxMaxIntake='" + vcBoxMaxIntake + "',          ");
                    strSql.AppendLine("      vcBoxType='" + vcBoxType + "',          ");
                    strSql.AppendLine("      vcLength='" + vcLength + "',          ");
                    strSql.AppendLine("      vcWide='" + vcWide + "',          ");
                    strSql.AppendLine("      vcHeight='" + vcHeight + "',          ");
                    strSql.AppendLine("      vcEmptyWeight='" + vcEmptyWeight + "',          ");
                    strSql.AppendLine("      vcUnitNetWeight='" + vcUnitNetWeight + "',          ");
                    strSql.AppendLine("      vcImageRoutes='" + vcImageRoutes + "',          ");
                    strSql.AppendLine("      vcFactoryOperatorID='" + userId + "',          ");
                    strSql.AppendLine("      dFactoryOperatorTime=GETDATE(),          ");
                    strSql.AppendLine("      vcOperatorID='" + userId + "',          ");
                    strSql.AppendLine("      dOperatorTime=GETDATE()           ");
                    strSql.AppendLine("      where iAutoId=" + iAutoId + "        ");
                    strSql.AppendLine("                ");
                }

                return excute.ExcuteSqlWithStringOper(strSql.ToString())>0;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 回复操作
        /// </summary>
        /// <param name="listInfoData"></param>
        /// <param name="userId"></param>
        public void reply(List<Dictionary<string, object>> listInfoData, string userId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                //sql.Append("update [THeZiManage] set  \n");
                //sql.Append(" vcState='2',dReplyDate=GETDATE(), \n");
                //sql.Append(" vcFactoryOperatorID='" + userId + "',dFactoryOperatorTime=GETDATE() where iAutoId in( \n");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                    strSql.AppendLine("   update a set a.vcState='2',a.vcWorkArea = b.vcWorkArea,a.vcIntake = b.vcIntake,     ");
                    strSql.AppendLine("   a.vcBoxMaxIntake = b.vcBoxMaxIntake,a.vcBoxType = b.vcBoxType,     ");
                    strSql.AppendLine("   a.vcLength = b.vcLength,a.vcWide=b.vcWide,a.vcHeight =b.vcHeight,a.vcEmptyWeight=b.vcEmptyWeight,     ");
                    strSql.AppendLine("   a.vcUnitNetWeight=b.vcUnitNetWeight,a.vcImageRoutes=b.vcImageRoutes, a.vcIsEdit='0',    ");
                    strSql.AppendLine("   a.vcFactoryOperatorID='" + userId + "', a.dFactoryOperatorTime=GETDATE()     ");
                    strSql.AppendLine("    from (select * from THeZiManageTmp where vcDelFlag='0' and iAutoId = "+ iAutoId + ") b      ");
                    strSql.AppendLine("    left join THeZiManage a  on a.iAutoId = b.iAutoId;     ");
                    strSql.AppendLine("    update  THeZiManageTmp set  vcDelFlag='1' where iAutoId = " + iAutoId + " and vcDelFlag='0' ;");
                }
                //sql.Append("  )   \r\n ");
                if (strSql.Length>0) { 
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
