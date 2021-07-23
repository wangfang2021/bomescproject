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
    public class FS0806_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 检索
        public DataTable Search(string vcZYType, string vcBZPlant, string vcInputNo, string vcKBOrderNo,
            string vcKBLFNo, string vcSellNo, string vcPart_id, string vcBoxNo, string dStart, string dEnd, string vcLabelNo, string vcStatus, string vcSHF)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select t1.iAutoId,t1.vcZYType,t1.vcBZPlant,t1.vcInputNo,t1.vcKBOrderNo,t1.vcKBLFNo,t1.vcPart_id,t1.vcIOType, \n");
                strSql.Append("t1.vcSupplier_id,t1.vcSupplierGQ,t1.dStart,t1.dEnd,cast(isnull(t1.iQuantity,0) as int) as iQuantity,cast(isnull(t1.iQuantity,0) as int) as iQuantity_old,    \n");
                strSql.Append("t1.vcBZUnit,t1.vcSHF,t1.vcSR,t1.vcBoxNo,t1.vcSheBeiNo,t1.vcCheckType,cast(t1.iCheckNum as int) as iCheckNum,    \n");
                strSql.Append("t1.vcCheckStatus,vcCheckStatus as vcCheckStatus_old,t1.vcLabelStart,t1.vcLabelEnd,t1.vcUnlocker,t1.dUnlockTime,t1.vcSellNo,    \n");
                strSql.Append("t1.vcOperatorID,t1.dOperatorTime,t1.vcHostIp,    \n");
                strSql.Append("t3.vcName as vcBZPlantName,t2.vcName as vcZYTypeName, t4.vcUserName,'0' as vcModFlag,'0' as vcAddFlag,   \n");
                strSql.Append("case when t1.vcZYType='S1' and t5.vcPart_id is not null then 'NG' else t1.vcCheckStatus end as vcStatus,    \n");
                strSql.Append("case when t1.vcZYType='S1' and t5.vcPart_id is not null then 'NG过' else '' end as vcHaveNG    \n");
                strSql.Append("from TOperateSJ t1  \n");
                strSql.Append("left join (select vcValue,vcName from TCode where vcCodeId='C022') t2 on t1.vcZYType=t2.vcValue  \n");
                strSql.Append("left join (select vcValue,vcName from TCode where vcCodeId='C023') t3 on t1.vcBZPlant=t3.vcValue  \n");
                strSql.Append("left join SUser t4 on t1.vcOperatorID=t4.vcUserID  \n");
                strSql.Append("left join (select distinct vcPart_id,vcKBOrderNo,vcKBLFNo,vcSR from TOperateSJ_NG)t5    \n");
                strSql.Append("on t1.vcPart_id=t5.vcPart_id and t1.vcKBOrderNo=t5.vcKBOrderNo     \n");
                strSql.Append("and t1.vcKBLFNo=t5.vcKBLFNo and t1.vcSR=t5.vcSR    \n");
                strSql.Append("where 1=1  \n");
                if (vcZYType != "" && vcZYType != null)
                    strSql.Append("and isnull(t1.vcZYType,'') = '" + vcZYType + "'   \n");
                if (vcBZPlant != "" && vcBZPlant != null)
                    strSql.Append("and isnull(t1.vcBZPlant,'') = '" + vcBZPlant + "'  \n");
                if (vcInputNo != "" && vcInputNo != null)
                    strSql.Append("and isnull(t1.vcInputNo,'') = '" + vcInputNo + "'   \n");
                if (vcKBOrderNo != "" && vcKBOrderNo != null)
                    strSql.Append("and isnull(t1.vcKBOrderNo,'') = '" + vcKBOrderNo + "'   \n");
                if (vcKBLFNo != "" && vcKBLFNo != null)
                    strSql.Append("and isnull(t1.vcKBLFNo,'') = '" + vcKBLFNo + "'   \n");
                if (vcSellNo != "" && vcSellNo != null)
                    strSql.Append("and isnull(t1.vcSellNo,'') = '" + vcSellNo + "'   \n");
                if (vcPart_id != "" && vcPart_id != null)
                    strSql.Append("and isnull(t1.vcPart_id,'') = '" + vcPart_id + "'  \n");
                if (vcBoxNo != "" && vcBoxNo != null)
                    strSql.Append("and isnull(t1.vcBoxNo,'') = '" + vcBoxNo + "'  \n");
                if (dStart == "" || dStart == null)
                    dStart = "2001/01/01 00:01:00";
                if (dEnd == "" || dEnd == null)
                    dEnd = "2099/12/31 23:59:59";
                strSql.Append("and isnull(t1.dEnd,'2001/01/01 00:01:00') >= '" + dStart + "' and isnull(t1.dEnd,'2099/12/31 23:59:59') <= '" + dEnd + "'  \n");
                if (vcLabelNo != "" && vcLabelNo != null)
                    strSql.Append("and '" + vcLabelNo + "' between t1.vcLabelStart and t1.vcLabelEnd   \n");
                if (vcStatus != "" && vcStatus != null)
                    strSql.Append("and (case when t1.vcZYType='S1' and  t5.vcPart_id is not null then 'NG' else t1.vcCheckStatus end)='" + vcStatus + "'    \n");
                if (vcSHF != "" && vcSHF != null)
                    strSql.Append("and isnull(t1.vcSHF,'')='" + vcSHF + "'    \n");
                strSql.Append("order by t1.vcZYType,t1.vcInputNo,t1.dStart    \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable initSubApi(string vcPart_id, string vcKBOrderNo, string vcKBLFNo, string vcSR)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select *,'0' as vcModFlag,'0' as vcAddFlag from TOperateSJ_NG where vcPart_id='" + vcPart_id + "' and vcKBOrderNo='" + vcKBOrderNo + "'     \n");
                sql.Append("and vcKBLFNo='" + vcKBLFNo + "' and vcSR='" + vcSR + "'    \n");
                sql.Append("order by dOperatorTime desc  \n");
                return excute.ExcuteSqlWithSelectToDT(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
        {
            try
            {
                DateTime now = DateTime.Now;
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("create table #temp_fs0806(");
                sql.AppendLine("vcPart_id varchar(12) null,");
                sql.AppendLine("vcKBOrderNo varchar(50) null,");
                sql.AppendLine("vcKBLFNo varchar(50) null,");
                sql.AppendLine("vcSR varchar(50) null");
                sql.AppendLine(")");

                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bmodflag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool baddflag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑

                    //标识说明
                    //默认  bmodflag:false  baddflag:false
                    //新增  bmodflag:true   baddflag:true
                    //修改  bmodflag:true   baddflag:false

                    if (baddflag == false && bmodflag == true)
                    {//修改
                        #region modify sql
                        string iAutoId = listInfoData[i]["iAutoId"].ToString();
                        string vcPart_id = listInfoData[i]["vcPart_id"].ToString();
                        string vcKBOrderNo = listInfoData[i]["vcKBOrderNo"].ToString();
                        string vcKBLFNo = listInfoData[i]["vcKBLFNo"].ToString();
                        string vcSR = listInfoData[i]["vcSR"].ToString();
                        string vcCheckStatus = listInfoData[i]["vcCheckStatus"].ToString();
                        string vcCheckStatus_old = listInfoData[i]["vcCheckStatus_old"].ToString();
                        sql.Append("UPDATE [TOperateSJ]  \n");
                        sql.Append("   SET   \n");
                        sql.Append("       [iQuantity] = " + listInfoData[i]["iQuantity"].ToString() + "  \n");
                        sql.Append("      ,[vcCheckStatus] = '" + listInfoData[i]["vcCheckStatus"].ToString() + "'  \n");
                        sql.Append("      ,[vcOperatorID] = '" + strUserId + "'  \n");
                        sql.Append("      ,[dOperatorTime] = '" + now + "'  \n");
                        sql.Append(" WHERE iAutoId=" + iAutoId + "  \n");
                        #endregion

                        //将修改数据扔临时表时一份，下面要用
                        sql.AppendLine("insert into #temp_fs0806 ");
                        sql.AppendLine("select vcPart_id,vcKBOrderNo,vcKBLFNo,vcSR from TOperateSJ where iAutoId=" + iAutoId + "");

                        //检查状态是NG时，往NG表中插入一条数据
                        if (vcCheckStatus == "NG")
                        {
                            if (vcCheckStatus_old != "NG")
                            {
                                sql.AppendLine("INSERT INTO [TOperateSJ_NG]");
                                sql.AppendLine("           ([vcPart_id]");
                                sql.AppendLine("           ,[vcKBOrderNo]");
                                sql.AppendLine("           ,[vcKBLFNo]");
                                sql.AppendLine("           ,[vcSR]");
                                sql.AppendLine("           ,[iNGQuantity]");
                                sql.AppendLine("           ,[vcNGReason]");
                                sql.AppendLine("           ,[vcZRBS]");
                                sql.AppendLine("           ,[vcOperatorID]");
                                sql.AppendLine("           ,[dOperatorTime])");
                                sql.AppendLine("     VALUES");
                                sql.AppendLine("           ('" + vcPart_id + "'");
                                sql.AppendLine("           ,'" + vcKBOrderNo + "'");
                                sql.AppendLine("           ,'" + vcKBLFNo + "'");
                                sql.AppendLine("           ,'" + vcSR + "'");
                                sql.AppendLine("           ,null");
                                sql.AppendLine("           ,null");
                                sql.AppendLine("           ,null");
                                sql.AppendLine("           ,'" + strUserId + "'");
                                sql.AppendLine("           ,'" + now + "')");
                            }
                        }
                    }

                }
                if (sql.Length > 0)
                {
                    sql.Append("  DECLARE @errorPart varchar(4000)   \r\n");
                    sql.Append("  set @errorPart=''   \r\n");
                    sql.Append("  set @errorPart=(   \r\n");
                    sql.Append("  	select a.vcPart_id+';' from   \r\n");
                    sql.Append("  	(      \r\n");
                    sql.Append("        select distinct t1.vcPart_id+'-'+t1.vcKBOrderNo+'-'+t1.vcKBLFNo+'-'+t1.vcSR as vcPart_id from (   \r\n");
                    sql.Append("        	select ROW_NUMBER() over(order by t1.vcPart_id,t1.vcKBOrderNo,t1.vcKBLFNo,t1.vcSR,t1.vcZYType) as id,   \r\n");
                    sql.Append("        	t1.vcPart_id,t1.vcKBOrderNo,t1.vcKBLFNo,t1.vcSR,vcZYType,sum(iQuantity) as iQuantity from TOperateSJ t1   \r\n");
                    sql.Append("        	inner join (select distinct vcPart_id,vcKBOrderNo,vcKBLFNo,vcSR from #temp_fs0806) t2 on t1.vcPart_id=t2.vcPart_id and t1.vcKBOrderNo=t2.vcKBOrderNo   \r\n");
                    sql.Append("        	and t1.vcKBLFNo=t2.vcKBLFNo and t1.vcSR=t2.vcSR   \r\n");
                    sql.Append("        	group by t1.vcPart_id,t1.vcKBOrderNo,t1.vcKBLFNo,t1.vcSR,vcZYType   \r\n");
                    sql.Append("        )t1   \r\n");
                    sql.Append("        inner join    \r\n");
                    sql.Append("        (   \r\n");
                    sql.Append("        	select ROW_NUMBER() over(order by t1.vcPart_id,t1.vcKBOrderNo,t1.vcKBLFNo,t1.vcSR,t1.vcZYType) as id,   \r\n");
                    sql.Append("        	t1.vcPart_id,t1.vcKBOrderNo,t1.vcKBLFNo,t1.vcSR,vcZYType,sum(iQuantity) as iQuantity from TOperateSJ t1   \r\n");
                    sql.Append("        	inner join (select distinct vcPart_id,vcKBOrderNo,vcKBLFNo,vcSR from #temp_fs0806) t2 on t1.vcPart_id=t2.vcPart_id and t1.vcKBOrderNo=t2.vcKBOrderNo   \r\n");
                    sql.Append("        	and t1.vcKBLFNo=t2.vcKBLFNo and t1.vcSR=t2.vcSR   \r\n");
                    sql.Append("        	group by t1.vcPart_id,t1.vcKBOrderNo,t1.vcKBLFNo,t1.vcSR,vcZYType   \r\n");
                    sql.Append("        )t2 on t1.vcPart_id=t2.vcPart_id and t1.vcKBOrderNo=t2.vcKBOrderNo and t1.vcKBLFNo=t2.vcKBLFNo    \r\n");
                    sql.Append("        and t1.vcSR=t2.vcSR and t2.id=t1.id+1    \r\n");
                    sql.Append("        where t2.iQuantity>t1.iQuantity   \r\n");
                    sql.Append("  	)a for xml path('')   \r\n");
                    sql.Append("  )   \r\n");
                    sql.Append("      \r\n");
                    sql.Append("  if @errorPart<>''   \r\n");
                    sql.Append("  begin   \r\n");
                    sql.Append("    select CONVERT(int,'-->'+@errorPart+'<--')   \r\n");
                    sql.Append("  end    \r\n");
                    sql.Append("drop table #temp_fs0806    \r\n");

                    excute.ExcuteSqlWithStringOper(sql.ToString());
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("-->") != -1)
                {//主动判断抛出的异常
                    int startIndex = ex.Message.IndexOf("-->");
                    int endIndex = ex.Message.LastIndexOf("<--");
                    strErrorPartId = ex.Message.Substring(startIndex + 3, endIndex - startIndex - 3);
                }
                else
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
                    #region 先插入历史表
                    sql.AppendLine("INSERT INTO [TOperateSJ_History]");
                    sql.AppendLine("           ([vcZYType]");
                    sql.AppendLine("           ,[vcBZPlant]");
                    sql.AppendLine("           ,[vcInputNo]");
                    sql.AppendLine("           ,[vcKBOrderNo]");
                    sql.AppendLine("           ,[vcKBLFNo]");
                    sql.AppendLine("           ,[vcPart_id]");
                    sql.AppendLine("           ,[vcIOType]");
                    sql.AppendLine("           ,[vcSupplier_id]");
                    sql.AppendLine("           ,[vcSupplierGQ]");
                    sql.AppendLine("           ,[dStart]");
                    sql.AppendLine("           ,[dEnd]");
                    sql.AppendLine("           ,[iQuantity]");
                    sql.AppendLine("           ,[vcBZUnit]");
                    sql.AppendLine("           ,[vcSHF]");
                    sql.AppendLine("           ,[vcSR]");
                    sql.AppendLine("           ,[vcCaseNo]");
                    sql.AppendLine("           ,[vcBoxNo]");
                    sql.AppendLine("           ,[vcSheBeiNo]");
                    sql.AppendLine("           ,[vcCheckType]");
                    sql.AppendLine("           ,[iCheckNum]");
                    sql.AppendLine("           ,[vcCheckStatus]");
                    sql.AppendLine("           ,[vcLabelStart]");
                    sql.AppendLine("           ,[vcLabelEnd]");
                    sql.AppendLine("           ,[vcUnlocker]");
                    sql.AppendLine("           ,[dUnlockTime]");
                    sql.AppendLine("           ,[vcSellNo]");
                    sql.AppendLine("           ,[vcOperatorID]");
                    sql.AppendLine("           ,[dOperatorTime]");
                    sql.AppendLine("           ,[vcHostIp]");
                    sql.AppendLine("           ,[packingcondition]");
                    sql.AppendLine("           ,[vcPackingPlant]");
                    sql.AppendLine("		   ,[vcDeleter]");
                    sql.AppendLine("		   ,[dDeleteTime])");
                    sql.AppendLine("select ");
                    sql.AppendLine("			[vcZYType]");
                    sql.AppendLine("           ,[vcBZPlant]");
                    sql.AppendLine("           ,[vcInputNo]");
                    sql.AppendLine("           ,[vcKBOrderNo]");
                    sql.AppendLine("           ,[vcKBLFNo]");
                    sql.AppendLine("           ,[vcPart_id]");
                    sql.AppendLine("           ,[vcIOType]");
                    sql.AppendLine("           ,[vcSupplier_id]");
                    sql.AppendLine("           ,[vcSupplierGQ]");
                    sql.AppendLine("           ,[dStart]");
                    sql.AppendLine("           ,[dEnd]");
                    sql.AppendLine("           ,[iQuantity]");
                    sql.AppendLine("           ,[vcBZUnit]");
                    sql.AppendLine("           ,[vcSHF]");
                    sql.AppendLine("           ,[vcSR]");
                    sql.AppendLine("           ,[vcCaseNo]");
                    sql.AppendLine("           ,[vcBoxNo]");
                    sql.AppendLine("           ,[vcSheBeiNo]");
                    sql.AppendLine("           ,[vcCheckType]");
                    sql.AppendLine("           ,[iCheckNum]");
                    sql.AppendLine("           ,[vcCheckStatus]");
                    sql.AppendLine("           ,[vcLabelStart]");
                    sql.AppendLine("           ,[vcLabelEnd]");
                    sql.AppendLine("           ,[vcUnlocker]");
                    sql.AppendLine("           ,[dUnlockTime]");
                    sql.AppendLine("           ,[vcSellNo]");
                    sql.AppendLine("           ,[vcOperatorID]");
                    sql.AppendLine("           ,[dOperatorTime]");
                    sql.AppendLine("           ,[vcHostIp]");
                    sql.AppendLine("           ,[packingcondition]");
                    sql.AppendLine("           ,[vcPackingPlant]");
                    sql.AppendLine("		   ,'"+strUserId+"'");
                    sql.AppendLine("		   ,GETDATE()");
                    sql.AppendLine("from TOperateSJ where iAutoId="+iAutoId+"");
                    #endregion
                    sql.AppendLine("delete from TOperateSJ where iAutoId=" + iAutoId + "  ");
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

        #region 校验 数量<上一层数量
        public DataTable isQuantityOK(string vcPart_id, string vcKBOrderNo, string vcKBLFNo, string vcSR, string vcZYType)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("--取出上一层数量    \n");
                sql.Append("select top(1) ISNULL(iQuantity,0) as iQuantity from TOperateSJ     \n");
                sql.Append("where vcPart_id='" + vcPart_id + "' and vcKBOrderNo='" + vcKBOrderNo + "'     \n");
                sql.Append("and vcKBLFNo='" + vcKBLFNo + "' and vcSR='" + vcSR + "' and vcZYType<'" + vcZYType + "'    \n");
                sql.Append("order by vcZYType desc    \n");
                return excute.ExcuteSqlWithSelectToDT(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        public int isHaveAfterProject(string vcPart_id, string vcKBOrderNo, string vcKBLFNo, string vcSR, string vcZYType)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("select count(1) from TOperateSJ ");
                sql.AppendLine("where vcPart_id='" + vcPart_id + "' and vcKBOrderNo='" + vcKBOrderNo + "' and vcKBLFNo='" + vcKBLFNo + "' and vcSR='" + vcSR + "'");
                sql.AppendLine("and vcZYType>'" + vcZYType + "'");
                return excute.ExecuteScalar(sql.ToString());
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #region 保存_NG明细
        public void Save_sub(List<Dictionary<string, Object>> listInfoData, string strUserId,string vcPart_id,string vcKBOrderNo,string vcKBLFNo,string vcSR)
        {
            try
            {
                DateTime now = DateTime.Now;
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bmodflag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool baddflag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    string iNGQuantity = listInfoData[i]["iNGQuantity"].ToString();
                    string vcNGReason = listInfoData[i]["vcNGReason"].ToString();
                    string vcZRBS = listInfoData[i]["vcZRBS"].ToString();

                    if (baddflag == true && bmodflag == true)
                    {//新增
                        sql.AppendLine("INSERT INTO [TOperateSJ_NG]");
                        sql.AppendLine("           ([vcPart_id]");
                        sql.AppendLine("           ,[vcKBOrderNo]");
                        sql.AppendLine("           ,[vcKBLFNo]");
                        sql.AppendLine("           ,[vcSR]");
                        sql.AppendLine("           ,[iNGQuantity]");
                        sql.AppendLine("           ,[vcNGReason]");
                        sql.AppendLine("           ,[vcZRBS]");
                        sql.AppendLine("           ,[vcOperatorID]");
                        sql.AppendLine("           ,[dOperatorTime])");
                        sql.AppendLine("     VALUES");
                        sql.AppendLine("           ('" + vcPart_id + "'");
                        sql.AppendLine("           ,'" + vcKBOrderNo + "'");
                        sql.AppendLine("           ,'" + vcKBLFNo + "'");
                        sql.AppendLine("           ,'" + vcSR + "'");
                        sql.AppendLine("           ,nullif('" + listInfoData[i]["iNGQuantity"].ToString() + "','')");
                        sql.AppendLine("           ,'" + listInfoData[i]["vcNGReason"].ToString() + "'");
                        sql.AppendLine("           ,'" + listInfoData[i]["vcZRBS"].ToString() + "'");
                        sql.AppendLine("           ,'" + strUserId + "'");
                        sql.AppendLine("           ,'" + now + "')");
                    }
                    else if (baddflag == false && bmodflag == true)
                    {//修改
                        string iAutoId = listInfoData[i]["iAutoId"].ToString();
                        sql.Append("update TOperateSJ_NG set iNGQuantity=nullif('" + iNGQuantity + "',''),vcNGReason='" + vcNGReason + "',vcZRBS='" + vcZRBS + "',vcOperatorID='" + strUserId + "',dOperatorTime='" + now + "' where iAutoId=" + iAutoId + "    \n");
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

        #region 删除_NG明细
        public void Del_sub(List<Dictionary<string, Object>> checkedInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                for (int i = 0; i < checkedInfoData.Count; i++)
                {
                    string iAutoId = checkedInfoData[i]["iAutoId"].ToString();
                    sql.Append("delete from TOperateSJ_NG where iAutoId=" + iAutoId + "   \n");
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
    }
}
