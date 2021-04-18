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
        public DataTable Search(string vcSupplier_id, string vcWorkArea, string vcState, string vcPartNo, string vcCarType, string dExpectDeliveryDate, string strUserId, string dSendDate, string dReplyDate)
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
                strSql.AppendLine("    select a.[iAutoId],a.[vcPackingPlant] ,a.[vcReceiver],convert(varchar(10), a.[dSynchronizationDate],111) as [dSynchronizationDate], case when isnull(a.vcIsEdit,'0')='1' then e.vcName else (case when b.vcName='已织入' then '已承认' else b.vcName end)  end  as [vcState],    ");
                strSql.AppendLine("     a.[vcPartNo],convert(varchar(10), a.[dUseStartDate],111) as [dUseStartDate],convert(varchar(10), a.[dUserEndDate],111) as [dUserEndDate], a.[vcPartName],       "); 
                strSql.AppendLine("     a.[vcCarType],c.vcName as [vcOEOrSP], a.[vcSupplier_id],     ");
                strSql.AppendLine("     case when isnull(a.vcIsEdit,'0')='1' then d.vcWorkArea else a.vcWorkArea end as [vcWorkArea],    ");
                strSql.AppendLine("     convert(varchar(10), a.[dExpectDeliveryDate],111) as [dExpectDeliveryDate], a.[vcExpectIntake],     ");
                strSql.AppendLine("      case when isnull(a.vcIsEdit,'0')='1' then d.vcIntake else a.vcIntake end as [vcIntake],    ");
                strSql.AppendLine("      case when isnull(a.vcIsEdit,'0')='1' then d.vcBoxMaxIntake else a.vcBoxMaxIntake end as [vcBoxMaxIntake],    ");
                strSql.AppendLine("      case when isnull(a.vcIsEdit,'0')='1' then d.vcBoxType else a.vcBoxType end as [vcBoxType],    ");
                strSql.AppendLine("      case when isnull(a.vcIsEdit,'0')='1' then d.vcLength else a.vcLength end as [vcLength],    ");
                strSql.AppendLine("      case when isnull(a.vcIsEdit,'0')='1' then d.vcWide else a.vcWide end as [vcWide],     ");
                strSql.AppendLine("      case when isnull(a.vcIsEdit,'0')='1' then d.vcHeight else a.vcHeight end as[vcHeight],    ");
                strSql.AppendLine("      case when isnull(a.vcIsEdit,'0')='1' then d.vcEmptyWeight else a.vcEmptyWeight end as [vcEmptyWeight],       ");
                strSql.AppendLine("      case when isnull(a.vcIsEdit,'0')='1' then d.vcUnitNetWeight else a.vcUnitNetWeight end as [vcUnitNetWeight],    ");
                strSql.AppendLine("      convert(varchar(10), a.[dSendDate],111) as [dSendDate],convert(varchar(10), a.[dReplyDate],111) as  [dReplyDate],convert(varchar(10), a.[dAdmitDate],111) as  [dAdmitDate],convert(varchar(10), a.[dWeaveDate],111) as  [dWeaveDate], a.[vcMemo],     ");
                strSql.AppendLine("      case when isnull(a.vcIsEdit,'0')='1' then d.vcImageRoutes else a.vcImageRoutes end as vcImageRoutes,        ");
                strSql.AppendLine("     a.[vcInserter], a.[vcInserterDate],a.[vcFactoryOperatorID], a.[dFactoryOperatorTime],       ");
                strSql.AppendLine("     a.[vcOperatorID], a.[dOperatorTime],'0' as vcModFlag,'0' as vcAddFlag from [dbo].[THeZiManage] a       ");
                strSql.AppendLine("     left join (select vcValue,vcName from TCode where vcCodeId='C012') c on a.vcOEOrSP = c.vcValue        ");
                strSql.AppendLine("     left join (select vcValue,vcName from TCode where vcCodeId='C033') b on a.vcState = b.vcValue     ");
                strSql.AppendLine("     left join (select * from [THeZiManageTmp] where vcDelFlag='0') d on a.iAutoId =d.iAutoId    ");
                strSql.AppendLine("     left join (select vcValue,vcName from TCode where vcCodeId='C034') e on d.vcState = e.vcValue     ");
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
                    if (vcWorkArea == "无")
                    {
                        strSql.AppendLine("  and  (case when isnull(a.vcIsEdit,'0')='1' then isnull(d.vcWorkArea,'')   else isnull(a.vcWorkArea,'') end) = '' ");
                    }
                    else
                    {
                        strSql.AppendLine("  and  (case when isnull(a.vcIsEdit,'0')='1' then d.vcWorkArea   else a.vcWorkArea  end)  like  '" + vcWorkArea + "%' ");
                    }
                }
                if (vcState.Length > 0)
                {
                    if (vcState=="5"|| vcState=="6")
                    {
                        strSql.AppendLine("  and  d.vcState = '" + vcState + "' ");
                    } else
                    {
                        strSql.AppendLine("  and  a.vcState = '" + vcState + "' ");
                    }
                    
                }
                if (vcPartNo.Length > 0)
                {
                    strSql.AppendLine("  and  a.vcPartNo like '" + vcPartNo + "%' ");
                }
                
                if (vcCarType.Length > 0)
                {
                    if (vcCarType == "无")
                    {
                        strSql.AppendLine("  and  isnull(a.vcCarType,'') = '' ");
                    }
                    else
                    {
                        strSql.AppendLine("  and  a.vcCarType like '" + vcCarType + "%' ");
                    }
                    
                }
                if (dExpectDeliveryDate.Length > 0)
                {
                    strSql.AppendLine("  and  CONVERT(varchar(10),  a.dExpectDeliveryDate,112) = '" + dExpectDeliveryDate.Replace("-", "").Replace("/", "") + "' ");
                }
                if (dSendDate.Length > 0)
                {
                    strSql.AppendLine("  and  CONVERT(varchar(10),  a.dSendDate,112) = '" + dSendDate.Replace("-", "").Replace("/", "") + "' ");
                }
                if (dReplyDate.Length > 0)
                {
                    if (dReplyDate == "无")
                    {
                        strSql.AppendLine("  and  isnull(a.dReplyDate,'') = '' ");
                    }
                    else
                    {
                        strSql.AppendLine("  and  CONVERT(varchar(10),  a.dReplyDate,112) = '" + dReplyDate.Replace("-", "").Replace("/", "") + "' ");
                    }
                }

                strSql.AppendLine("  order by a.vcState asc, a.vcPartNo desc ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable SearchTmp(string vcSupplier_id, string vcWorkArea, string vcState, string vcPartNo, string vcCarType, string dExpectDeliveryDate, object userId, string dSendDate, string dReplyDate)
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
                strSql.AppendLine("    select a.[iAutoId],a.[vcPackingPlant] ,a.[vcReceiver],convert(varchar(10), a.[dSynchronizationDate],111) as [dSynchronizationDate],  b.vcName  as [vcState],    ");
                strSql.AppendLine("     a.[vcPartNo],convert(varchar(10), a.[dUseStartDate],111) as [dUseStartDate],convert(varchar(10), a.[dUserEndDate],111) as [dUserEndDate], a.[vcPartName],       ");
                strSql.AppendLine("     a.[vcCarType],c.vcName as [vcOEOrSP], a.[vcSupplier_id],     ");
                strSql.AppendLine("     case when isnull(a.vcIsEdit,'0')='1' then d.vcWorkArea else a.vcWorkArea end as [vcWorkArea],    ");
                strSql.AppendLine("     convert(varchar(10), a.[dExpectDeliveryDate],111) as [dExpectDeliveryDate], a.[vcExpectIntake],     ");
                strSql.AppendLine("      case when isnull(a.vcIsEdit,'0')='1' then d.vcIntake else a.vcIntake end as [vcIntake],    ");
                strSql.AppendLine("      case when isnull(a.vcIsEdit,'0')='1' then d.vcBoxMaxIntake else a.vcBoxMaxIntake end as [vcBoxMaxIntake],    ");
                strSql.AppendLine("      case when isnull(a.vcIsEdit,'0')='1' then d.vcBoxType else a.vcBoxType end as [vcBoxType],    ");
                strSql.AppendLine("      case when isnull(a.vcIsEdit,'0')='1' then d.vcLength else a.vcLength end as [vcLength],    ");
                strSql.AppendLine("      case when isnull(a.vcIsEdit,'0')='1' then d.vcWide else a.vcWide end as [vcWide],     ");
                strSql.AppendLine("      case when isnull(a.vcIsEdit,'0')='1' then d.vcHeight else a.vcHeight end as[vcHeight],    ");
                strSql.AppendLine("      case when isnull(a.vcIsEdit,'0')='1' then d.vcEmptyWeight else a.vcEmptyWeight end as [vcEmptyWeight],       ");
                strSql.AppendLine("      case when isnull(a.vcIsEdit,'0')='1' then d.vcUnitNetWeight else a.vcUnitNetWeight end as [vcUnitNetWeight],    ");
                strSql.AppendLine("      convert(varchar(10), a.[dSendDate],111) as [dSendDate],convert(varchar(10), a.[dReplyDate],111) as  [dReplyDate],convert(varchar(10), a.[dAdmitDate],111) as  [dAdmitDate],convert(varchar(10), a.[dWeaveDate],111) as  [dWeaveDate], a.[vcMemo],     ");
                strSql.AppendLine("      case when isnull(a.vcIsEdit,'0')='1' then d.vcImageRoutes else a.vcImageRoutes end as vcImageRoutes,        ");
                strSql.AppendLine("     a.[vcInserter], a.[vcInserterDate],a.[vcFactoryOperatorID], a.[dFactoryOperatorTime],       ");
                strSql.AppendLine("     a.[vcOperatorID], a.[dOperatorTime],'0' as vcModFlag,'0' as vcAddFlag from [dbo].[THeZiManage] a       ");
                strSql.AppendLine("     left join (select vcValue,vcName from TCode where vcCodeId='C012') c on a.vcOEOrSP = c.vcValue        ");
                strSql.AppendLine("     left join (select vcValue,vcName from TCode where vcCodeId='C033') b on a.vcState = b.vcValue     ");
                strSql.AppendLine("     left join (select * from [THeZiManageTmp] where vcDelFlag='0') d on a.iAutoId =d.iAutoId    ");
                strSql.AppendLine("     left join (select vcValue,vcName from TCode where vcCodeId='C034') e on d.vcState = e.vcValue     ");
                strSql.AppendLine("   where 1=1 and a.vcState in ('1','3')     ");
                strSql.AppendLine("       ");
                strSql.AppendLine("       ");
                strSql.AppendLine("       ");


                if (vcSupplier_id.Length > 0)
                {
                    strSql.AppendLine("  and  a.vcSupplier_id  =  '" + vcSupplier_id + "' ");
                }
                if (vcWorkArea.Length > 0)
                {
                    if (vcWorkArea == "无")
                    {
                        strSql.AppendLine("  and  (case when isnull(a.vcIsEdit,'0')='1' then isnull(d.vcWorkArea,'')   else isnull(a.vcWorkArea,'') end) = '' ");
                    }
                    else
                    {
                        strSql.AppendLine("  and  (case when isnull(a.vcIsEdit,'0')='1' then d.vcWorkArea   else a.vcWorkArea  end)  like  '" + vcWorkArea + "%' ");
                    }
                }
                if (vcState.Length > 0)
                {
                    if (vcState=="1")
                    {
                        strSql.AppendLine("  and  a.vcState='1'  ");
                    }
                    if (vcState == "3")
                    {
                        strSql.AppendLine("  and  a.vcState='3'  ");
                    }
                }
                if (vcPartNo.Length > 0)
                {
                    strSql.AppendLine("  and  a.vcPartNo like '" + vcPartNo + "%' ");
                }

                if (vcCarType.Length > 0)
                {
                    if (vcCarType == "无")
                    {
                        strSql.AppendLine("  and  isnull(a.vcCarType,'') = '' ");
                    }
                    else
                    {
                        strSql.AppendLine("  and  a.vcCarType like '" + vcCarType + "%' ");
                    }

                }
                if (dExpectDeliveryDate.Length > 0)
                {
                    strSql.AppendLine("  and  CONVERT(varchar(10),  a.dExpectDeliveryDate,112) = '" + dExpectDeliveryDate.Replace("-", "").Replace("/", "") + "' ");
                }
                if (dSendDate.Length > 0)
                {
                    strSql.AppendLine("  and  CONVERT(varchar(10),  a.dSendDate,112) = '" + dSendDate.Replace("-", "").Replace("/", "") + "' ");
                }
                if (dReplyDate.Length > 0)
                {
                    if (dReplyDate == "无")
                    {
                        strSql.AppendLine("  and  isnull(a.dReplyDate,'') = '' ");
                    }
                    else
                    {
                        strSql.AppendLine("  and  CONVERT(varchar(10),  a.dReplyDate,112) = '" + dReplyDate.Replace("-", "").Replace("/", "") + "' ");
                    }
                }

                strSql.AppendLine("  order by a.vcState asc, a.vcPartNo desc ");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetSendDate(string userId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("  select dSendDate as vcValue,dSendDate as vcName from(    ");
                strSql.AppendLine("    	select  distinct convert(varchar(10), dSendDate,111) as dSendDate from [THeZiManage] where vcSupplier_id='" + userId + "' and  vcState<>4  and  vcState<>5    ");
                strSql.AppendLine("  ) t order by vcValue desc  ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetReplyDate(string userId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();

                strSql.AppendLine("  select isnull(dReplyDate,'无') as vcValue,isnull(dReplyDate,'无') as vcName from(    ");
                strSql.AppendLine("    	select  distinct convert(varchar(10), dReplyDate,111) as dReplyDate from [THeZiManage] where vcSupplier_id='" + userId + "' and  vcState<>4   ");
                strSql.AppendLine("  ) t order by vcValue desc  ");

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
                strSql.AppendLine("    	select distinct isnull(vcCarType,'无') as vcCarType from [THeZiManage] where vcSupplier_id='" + userId + "'   ");
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
                strSql.AppendLine("    	select  distinct convert(varchar(10), dExpectDeliveryDate,111) as dExpectDeliveryDate from [THeZiManage] where vcSupplier_id='" + userId + "' and  vcState<>4 and  vcState<>5  ");
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
                strSql.AppendLine("   select distinct isnull(vcWorkArea,'无') as vcWorkArea from [THeZiManage] where vcSupplier_id='" + supplierId + "'   ");
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
                strSql.AppendLine("      case when isnull(a.vcIsEdit,'0')='1' then d.vcUnitNetWeight else a.vcUnitNetWeight end as [vcUnitNetWeight],    ");
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
                StringBuilder sql = new StringBuilder();
                sql.Append("DELETE FROM [dbo].[THeZiManageImportTmp4Supplier] where  vcOperatorID='" + strUserId + "' \n");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string vcState = dt.Rows[i]["vcState"] == System.DBNull.Value ? "" : dt.Rows[i]["vcState"].ToString().Trim();
                    if (vcState == "待回复")
                    {
                        vcState = "1";
                    }
                    else if (vcState == "退回")
                    {
                        vcState = "3";
                    }
                    string vcPackingPlant = dt.Rows[i]["vcPackingPlant"] == System.DBNull.Value ? "" : dt.Rows[i]["vcPackingPlant"].ToString().Trim();
                    string vcReceiver = dt.Rows[i]["vcReceiver"] == System.DBNull.Value ? "" : dt.Rows[i]["vcReceiver"].ToString().Trim();
                    //string dSynchronizationDate = dt.Rows[i]["dSynchronizationDate"] == System.DBNull.Value ? "" : dt.Rows[i]["dSynchronizationDate"].ToString().Trim();
                    
                    string vcPartNo = dt.Rows[i]["vcPartNo"] == System.DBNull.Value ? "" : dt.Rows[i]["vcPartNo"].ToString().Trim();
                    string dUseStartDate = dt.Rows[i]["dUseStartDate"] == System.DBNull.Value ? "" : dt.Rows[i]["dUseStartDate"].ToString().Trim();
                    //string dUserEndDate = dt.Rows[i]["dUserEndDate"] == System.DBNull.Value ? "" : dt.Rows[i]["dUserEndDate"].ToString();
                    string vcPartName = dt.Rows[i]["vcPartName"] == System.DBNull.Value ? "" : dt.Rows[i]["vcPartName"].ToString().Trim();
                    string vcCarType = dt.Rows[i]["vcCarType"] == System.DBNull.Value ? "" : dt.Rows[i]["vcCarType"].ToString().Trim();
                    string vcOEOrSP = dt.Rows[i]["vcOEOrSP"] == System.DBNull.Value ? "" : dt.Rows[i]["vcOEOrSP"].ToString().Trim();
                    if (vcOEOrSP == "×")
                    {
                        vcOEOrSP = "1";
                    }
                    else if (vcOEOrSP == "⭕" || vcOEOrSP == "○")
                    {
                        vcOEOrSP = "0";
                    }
                    else
                    { }
                    string vcSupplier_id = dt.Rows[i]["vcSupplier_id"] == System.DBNull.Value ? "" : dt.Rows[i]["vcSupplier_id"].ToString().Trim();
                    string vcWorkArea = dt.Rows[i]["vcWorkArea"] == System.DBNull.Value ? "" : dt.Rows[i]["vcWorkArea"].ToString();
                    string dExpectDeliveryDate = dt.Rows[i]["dExpectDeliveryDate"] == System.DBNull.Value ? "" : dt.Rows[i]["dExpectDeliveryDate"].ToString().Trim();
                    string vcExpectIntake = dt.Rows[i]["vcExpectIntake"] == System.DBNull.Value ? "" : dt.Rows[i]["vcExpectIntake"].ToString().Trim();
                    string vcIntake = dt.Rows[i]["vcIntake"] == System.DBNull.Value ? "" : dt.Rows[i]["vcIntake"].ToString().Trim();
                    string vcBoxMaxIntake = dt.Rows[i]["vcBoxMaxIntake"] == System.DBNull.Value ? "" : dt.Rows[i]["vcBoxMaxIntake"].ToString().Trim();
                    string vcBoxType = dt.Rows[i]["vcBoxType"] == System.DBNull.Value ? "" : dt.Rows[i]["vcBoxType"].ToString().Trim();
                    string vcLength = dt.Rows[i]["vcLength"] == System.DBNull.Value ? "" : dt.Rows[i]["vcLength"].ToString().Trim();
                    string vcWide = dt.Rows[i]["vcWide"] == System.DBNull.Value ? "" : dt.Rows[i]["vcWide"].ToString().Trim();
                    string vcHeight = dt.Rows[i]["vcHeight"] == System.DBNull.Value ? "" : dt.Rows[i]["vcHeight"].ToString();
                    string vcEmptyWeight = dt.Rows[i]["vcEmptyWeight"] == System.DBNull.Value ? "" : dt.Rows[i]["vcEmptyWeight"].ToString().Trim();
                    string vcUnitNetWeight = dt.Rows[i]["vcUnitNetWeight"] == System.DBNull.Value ? "" : dt.Rows[i]["vcUnitNetWeight"].ToString().Trim();
                    string dSendDate = dt.Rows[i]["dSendDate"] == System.DBNull.Value ? "" : dt.Rows[i]["dSendDate"].ToString().Trim();
                    string dReplyDate = dt.Rows[i]["dReplyDate"] == System.DBNull.Value ? "" : dt.Rows[i]["dReplyDate"].ToString().Trim();
                    string dAdmitDate = dt.Rows[i]["dAdmitDate"] == System.DBNull.Value ? "" : dt.Rows[i]["dAdmitDate"].ToString().Trim();
                    string dWeaveDate = dt.Rows[i]["dWeaveDate"] == System.DBNull.Value ? "" : dt.Rows[i]["dWeaveDate"].ToString().Trim();
                    string vcMemo = dt.Rows[i]["vcMemo"] == System.DBNull.Value ? "" : dt.Rows[i]["vcMemo"].ToString().Trim();
                    string vcOperatorID = strUserId;


                    sql.Append("   INSERT INTO [dbo].[THeZiManageImportTmp4Supplier]   \n");
                    sql.Append("              ([vcPackingPlant],[vcReceiver]   \n");
                    sql.Append("              ,[vcState] ,[vcPartNo]  ,[dUseStartDate]    \n");
                    sql.Append("              ,[vcPartName]  ,[vcCarType],[vcOEOrSP] ,[vcSupplier_id]   \n");
                    sql.Append("              ,[vcWorkArea] ,[dExpectDeliveryDate] ,[vcExpectIntake],[vcIntake]   \n");
                    sql.Append("              ,[vcBoxMaxIntake] ,[vcBoxType] ,[vcLength],[vcWide] ,[vcHeight]   \n");
                    sql.Append("              ,[vcEmptyWeight] ,[vcUnitNetWeight] ,[dSendDate] ,[dReplyDate]   \n");
                    sql.Append("              ,[dAdmitDate] ,[dWeaveDate] ,[vcMemo],[vcOperatorID],[dOperatorTime]   \n");
                    sql.Append("             ) values    \n");
                    sql.Append("   		  ( " + getSqlValue(vcPackingPlant, true) + "," + getSqlValue(vcReceiver, true) + ",  \n");
                    sql.Append("    " + getSqlValue(vcState, true) + "," + getSqlValue(vcPartNo, true) + "," + getSqlValue(dUseStartDate, true) + ",   \n");
                    sql.Append("     " + getSqlValue(vcPartName, true) + "," + getSqlValue(vcCarType, true) + "," + getSqlValue(vcOEOrSP, true) + "," + getSqlValue(vcSupplier_id, true) + ",   \n");
                    sql.Append("     " + getSqlValue(vcWorkArea, true) + "," + getSqlValue(dExpectDeliveryDate, true) + "," + getSqlValue(vcExpectIntake, true) + "," + getSqlValue(vcIntake, true) + ",   \n");
                    sql.Append("   " + getSqlValue(vcBoxMaxIntake, true) + "," + getSqlValue(vcBoxType, true) + "," + getSqlValue(vcLength, true) + "," + getSqlValue(vcWide, true) + ", " + getSqlValue(vcHeight, true) + ",   \n");
                    sql.Append("    " + getSqlValue(vcEmptyWeight, true) + "," + getSqlValue(vcUnitNetWeight, true) + "," + getSqlValue(dSendDate, true) + "," + getSqlValue(dReplyDate, true) + ",  \n");
                    sql.Append("    " + getSqlValue(dAdmitDate, true) + "," + getSqlValue(dWeaveDate, true) + "," + getSqlValue(vcMemo, true) + ",  \n");
                    sql.Append("   '" + strUserId + "' \n");
                    sql.Append("     ,getdate())  \n");
                }

                //删除
                //sql.Append("   delete from  THeZiManageTmp where iAutoId in   \n");
                //sql.Append("   (   \n");
                //sql.Append("   select b.iAutoId   \n");
                //sql.Append("    from (   \n");
                //sql.Append("   select * from THeZiManageImportTmp4Supplier where vcOperatorID='" + strUserId + "'   \n");
                //sql.Append("   ) a left join   \n");
                //sql.Append("   (   \n");
                //sql.Append("   select * from THeZiManage where vcState in ('1','3')   \n");
                //sql.Append("   ) b on a.vcPartNo=b.vcPartNo and a.vcPackingPlant=b.vcPackingPlant   \n");
                //sql.Append("    and a.vcSupplier_id=b.vcSupplier_id and a.vcReceiver=b.vcReceiver and a.vcState =b.vcState   \n");
                //sql.Append("    where b.vcPartNo is not null   \n");
                //sql.Append("   )   \n");
                //update
                sql.Append("  update c set c.vcWorkArea = a.vcWorkArea, c.vcIntake = a.vcIntake,   \n");
                sql.Append("  c.vcBoxMaxIntake=a.vcBoxMaxIntake,c.vcBoxType=a.vcBoxType   \n");
                sql.Append("  ,c.vcLength=a.vcLength,c.vcWide=a.vcWide ,c.vcHeight=a.vcHeight,   \n");
                sql.Append("  c.vcEmptyWeight=a.vcEmptyWeight,c.vcUnitNetWeight=a.vcUnitNetWeight,   \n");
                sql.Append("  c.dOperatorTime=GETDATE()   \n");
                sql.Append("   from (      \n");
                sql.Append("  select * from THeZiManageImportTmp4Supplier where vcOperatorID='" + strUserId + "'      \n");
                sql.Append("  ) a left join  (   \n");
                sql.Append("   select * from THeZiManage where vcState in ('1','3')     \n");
                sql.Append("  ) b on a.vcPartNo=b.vcPartNo and a.vcPackingPlant=b.vcPackingPlant     \n");
                sql.Append("   and a.vcSupplier_id=b.vcSupplier_id and a.vcReceiver=b.vcReceiver and a.vcState =b.vcState    \n");
                sql.Append("   left join   THeZiManageTmp c on b.iAutoId = c.iAutoId   \n");
                sql.Append("   where b.vcPartNo is not null and isnull(b.vcIsEdit,'0')='1'     \n");
                sql.Append("     \n");
                sql.Append("     \n");



                //新增
                sql.Append("   insert into [THeZiManageTmp] (iAutoId,vcState,vcSupplier_id,vcWorkArea,vcIntake,   \n");
                sql.Append("   vcBoxMaxIntake,vcBoxType,vcLength,vcWide ,vcHeight,vcEmptyWeight   \n");
                sql.Append("   ,vcUnitNetWeight,vcDelFlag,[vcOperatorID], [dOperatorTime])    \n");
                sql.Append("   select b.iAutoId,case when a.vcState='1' then '5' else '6' end as vcState,   \n");
                sql.Append("   a.vcSupplier_id,a.vcWorkArea,a.vcIntake,a.vcBoxMaxIntake,   \n");
                sql.Append("   a.vcBoxType,a.vcLength,a.vcWide ,a.vcHeight,a.vcEmptyWeight,a.vcUnitNetWeight,   \n");
                sql.Append("   '0','" + strUserId + "',GETDATE()   \n");
                sql.Append("    from (   \n");
                sql.Append("   select * from THeZiManageImportTmp4Supplier where vcOperatorID='" + strUserId + "'   \n");
                sql.Append("   ) a left join   \n");
                sql.Append("   (   \n");
                sql.Append("   select * from THeZiManage where vcState in ('1','3')   \n");
                sql.Append("   ) b on a.vcPartNo=b.vcPartNo and a.vcPackingPlant=b.vcPackingPlant   \n");
                sql.Append("    and a.vcSupplier_id=b.vcSupplier_id and a.vcReceiver=b.vcReceiver and a.vcState =b.vcState   \n");
                sql.Append("    where b.vcPartNo is not null and isnull(b.vcIsEdit,'0')='0'   \n");

                //gengxin
                sql.Append("    update [dbo].[THeZiManage] set vcIsEdit='1' where iAutoId in   \n");
                sql.Append("   (   \n");
                sql.Append("   select b.iAutoId   \n");
                sql.Append("    from (   \n");
                sql.Append("   select * from THeZiManageImportTmp4Supplier where vcOperatorID='" + strUserId + "'   \n");
                sql.Append("   ) a left join   \n");
                sql.Append("   (   \n");
                sql.Append("   select * from THeZiManage where vcState in ('1','3')   \n");
                sql.Append("   ) b on a.vcPartNo=b.vcPartNo and a.vcPackingPlant=b.vcPackingPlant   \n");
                sql.Append("    and a.vcSupplier_id=b.vcSupplier_id and a.vcReceiver=b.vcReceiver and a.vcState =b.vcState  \n");
                sql.Append("    where b.vcPartNo is not null   \n");
                sql.Append("   )   \n");
                sql.Append("      \n");
               
                if (sql.Length > 0)
                {
                    excute.ExcuteSqlWithStringOper(sql.ToString());
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            #region

            /*try
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
            }*/
            #endregion
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
                else if (vcState == "待回复(已填写)")
                {
                    vcState = "5";
                }
                else if (vcState == "退回(已填写)")
                {
                    vcState = "6";
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

                if (vcState == "1" || vcState == "3"|| vcState == "5" || vcState == "6")
                {
                    int editSate = 0;
                    if (vcState == "1")
                    {
                        editSate = 5;
                    }
                    else if (vcState == "3")
                    {
                        editSate = 6;
                    }
                    else
                    {
                        editSate = Convert.ToInt32(vcState);
                    }

                    strSql.AppendLine(" delete from  THeZiManageTmp where iAutoId=" + iAutoId + ";   ");
                    strSql.AppendLine("   insert into [THeZiManageTmp] (iAutoId,vcState,vcSupplier_id,vcWorkArea,vcIntake,vcBoxMaxIntake,   ");
                    strSql.AppendLine("   vcBoxType,vcLength,vcWide   ");
                    strSql.AppendLine("   ,vcHeight,vcEmptyWeight,vcUnitNetWeight,vcImageRoutes,vcDelFlag,[vcOperatorID], [dOperatorTime]) values   ");
                    strSql.AppendLine("    ( " + iAutoId + ",'" + editSate + "','" + vcSupplier_id + "','" + vcWorkArea + "','" + vcIntake + "','" + vcBoxMaxIntake + "',  ");
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
                    strSql.AppendLine("   a.vcUnitNetWeight=b.vcUnitNetWeight,a.vcImageRoutes=b.vcImageRoutes, a.vcIsEdit='0',a.dReplyDate=GETDATE(),    ");
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
