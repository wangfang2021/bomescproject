using System;
using Common;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace DataAccess
{
    public class FS0501_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        #region 按检索条件检索,返回dt
        public DataTable Search(string strYearMonth, string strSupplier_id, string strPart_id, string strDyState, string strOperState)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select t1.iAutoId,t1.vcYearMonth,t1.dExpectTime,t1.vcHyState,t1.vcDyState,t2.vcName as vcDyState_Name,    \n");
                strSql.Append("t1.vcPart_id,t1.iQuantityPercontainer,t1.iCbSOQN,t1.iCbSOQN1,t1.iCbSOQN2,    \n");
                strSql.Append("t3.iTzhSOQN,t3.iTzhSOQN1,t3.iTzhSOQN2,t1.dOpenTime,t1.dSReplyTime,    \n");
                strSql.Append("case           \n");
                strSql.Append(" when t3.iTzhSOQN is null or t3.iTzhSOQN1 is null or t3.iTzhSOQN2 is null then 'partFS0501A' --无调整          \n");
                strSql.Append(" when t3.iTzhSOQN=t1.iCbSOQN and t3.iTzhSOQN1=t1.iCbSOQN1 and t3.iTzhSOQN2=t1.iCbSOQN2 then 'partFS0501A' --无调整        \n");
                strSql.Append(" else 'partFS0501B' --有调整          \n");
                strSql.Append(" end as vcBgColor,'0' as vcModFlag,'0' as vcAddFlag,t1.vcSupplier_id,      \n");
                strSql.Append("CASE WHEN vcDyState='1' and vcHyState in ('0','3') then '0' else '1' end as bSelectFlag    \n");
                strSql.Append("from(    \n");
                strSql.Append("	select * from TSoq     \n");
                strSql.Append("	where vcYearMonth='" + strYearMonth + "'     \n");
                strSql.Append("	and vcSupplier_id='" + strSupplier_id + "'    \n");
                if (strOperState == "Y")
                    strSql.Append("	and vcDyState='1' and vcHyState in ('0','3')    \n");
                else if (strOperState == "N")
                    strSql.Append("	and (vcDyState!='1' or vcHyState not in ('0','3'))    \n");
                if (!string.IsNullOrEmpty(strDyState))//对应状态
                    strSql.Append(" and vcDyState='" + strDyState + "' ");
                if (!string.IsNullOrEmpty(strPart_id))//品番
                    strSql.Append(" and vcPart_id like '%" + strPart_id + "%'");
                strSql.Append(")t1    \n");
                strSql.Append("left join (select vcValue,vcName from TCode where vcCodeId='C036')t2 on t1.vcDyState=t2.vcValue    \n");
                strSql.Append("left join (    \n");
                strSql.Append("	select a.vcYearMonth,a.vcPart_id,a.iTzhSOQN,a.iTzhSOQN1,a.iTzhSOQN2     \n");
                strSql.Append("	from (    \n");
                strSql.Append("		select * from TSoq_OperHistory where vcInputType='supplier'    \n");
                strSql.Append("	)a    \n");
                strSql.Append("	inner join (    \n");
                strSql.Append("		select vcYearMonth,vcPart_id,MAX(dOperatorTime) as dOperatorTime from TSoq_OperHistory     \n");
                strSql.Append("		where vcInputType='supplier'    \n");
                strSql.Append("		group by vcYearMonth,vcPart_id    \n");
                strSql.Append("	)b on a.vcYearMonth=b.vcYearMonth and a.vcPart_id=b.vcPart_id and a.dOperatorTime=b.dOperatorTime       \n");
                strSql.Append(")t3 on t1.vcYearMonth=t3.vcYearMonth and t1.vcPart_id=t3.vcPart_id    \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 检索导入履历
        public DataTable SearchHistory(string strYearMonth,string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select * from TSoqInputErrDetail_Save  WHERE vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth + "'    \n");
                return excute.ExcuteSqlWithSelectToDT(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 向SOQ导入履历中新增数据
        public int importHistory(string strYearMonth, List<string> errMessageList, string strUserId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                //先删除本对象月再新增
                strSql.Append(" DELETE FROM TSoqInputErrDetail_Save  WHERE vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth + "' ");
                for (int i = 0; i < errMessageList.Count; i++)
                {
                    string msg = errMessageList[i].ToString();
                    strSql.Append(" INSERT INTO TSoqInputErrDetail_Save (vcYearMonth,vcMessage,vcOperator,dOperatorTime) values ( ");
                    strSql.Append("  '" + strYearMonth + "','" + msg + "','" + strUserId + "',getdate())  ");
                }
                return excute.ExcuteSqlWithStringOper(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 是否可操作-按检索条件
        public DataTable IsDQR(string strYearMonth, string strSupplier_id, string strPart_id, string strDyState, string strOperState)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine(" select * from TSoq ");
                strSql.AppendLine(" WHERE (vcDyState!='1' or vcHyState not in ('0','3')) ");//这几个状态(1,0,3)是可操作的状态
                strSql.Append("	and vcYearMonth='" + strYearMonth + "'     \n");
                strSql.Append("	and vcSupplier_id='" + strSupplier_id + "'    \n");
                if (strOperState == "Y")
                    strSql.Append("	and vcDyState='1' and vcHyState in ('0','3')    \n");
                else if (strOperState == "N")
                    strSql.Append("	and (vcDyState!='1' or vcHyState not in ('0','3'))    \n");
                if (!string.IsNullOrEmpty(strDyState))//对应状态
                    strSql.Append(" and vcDyState='" + strDyState + "' ");
                if (!string.IsNullOrEmpty(strPart_id))//品番
                    strSql.Append(" and vcPart_id like '%" + strPart_id + "%'");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 是否可操作-按列表所选数据
        public DataTable IsDQR(string strYearMonth, List<Dictionary<string, Object>> listInfoData, string strType)//strType:save(保存)、submit(提交)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("      if object_id('tempdb..#TSoq_temp_cr') is not null       \n");
                strSql.Append("      Begin      \n");
                strSql.Append("      drop  table #TSoq_temp_cr       \n");
                strSql.Append("      End      \n");
                strSql.Append("      select * into #TSoq_temp_cr from       \n");
                strSql.Append("      (      \n");
                strSql.Append("      	select vcPart_id,vcHyState,vcDyState from TSoq where 1=0      \n");
                strSql.Append("      ) a      ;\n");
                if (strType == "save")
                {
                    for (int i = 0; i < listInfoData.Count; i++)
                    {
                        bool bmodflag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                        bool baddflag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                        if (baddflag == true)
                        {//新增
                         //没有新增情况
                        }
                        else if (baddflag == false && bmodflag == true)
                        {//修改
                            #region 将所有的数据都插入临时表
                            strSql.Append("      insert into #TSoq_temp_cr       \n");
                            strSql.Append("       (         \n");
                            strSql.Append("       vcPart_id,vcHyState,vcDyState        \n");
                            strSql.Append("       ) values         \n");
                            strSql.Append("      (      \n");
                            strSql.Append("       '" + listInfoData[i]["vcPart_id"].ToString() + "',     \n");
                            strSql.Append("       '" + listInfoData[i]["vcHyState"].ToString() + "',     \n");
                            strSql.Append("       '" + listInfoData[i]["vcDyState"].ToString() + "'     \n");
                            strSql.Append("      );      \n");
                            #endregion
                        }
                    }
                }
                else if (strType == "submit")
                {
                    for (int i = 0; i < listInfoData.Count; i++)
                    {
                        #region 将所有的数据都插入临时表
                        strSql.Append("      insert into #TSoq_temp_cr       \n");
                        strSql.Append("       (         \n");
                        strSql.Append("       vcPart_id,vcHyState,vcDyState        \n");
                        strSql.Append("       ) values         \n");
                        strSql.Append("      (      \n");
                        strSql.Append("       '" + listInfoData[i]["vcPart_id"].ToString() + "',     \n");
                        strSql.Append("       '" + listInfoData[i]["vcHyState"].ToString() + "',     \n");
                        strSql.Append("       '" + listInfoData[i]["vcDyState"].ToString() + "'     \n");
                        strSql.Append("      );      \n");
                        #endregion
                    }
                }
                strSql.AppendLine(" select * from #TSoq_temp_cr where (vcDyState!='1' or vcHyState not in ('0','3'))  ");//这几个状态(1,0,3)是可操作的状态
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 提交-按检索条件
        public int ok(string strYearMonth, string strSupplier_id, string strPart_id, string strDyState, string strOperState,string strUserId)
        {
            try
            {
                string strLastTimeFlag = DateTime.Now.ToString("yyyyMMddHHmmss");
                StringBuilder strSql = new StringBuilder();

                strSql.Append("update t1 set     \n");
                strSql.Append("      t1.vcDyState=case when t1.iCbSOQN!=t2.iTzhSOQN or t1.iCbSOQN1!=t2.iTzhSOQN1 or t1.iCbSOQN2!=t2.iTzhSOQN2 then '2' else '3' end, ");//0：未发送；1：待回复；2：有调整；3：无调整
                strSql.Append("      t1.vcSReplyUser='" + strUserId + "', ");
                strSql.Append("      t1.dSReplyTime=getdate(), ");
                strSql.Append("      t1.vcOperator='" + strUserId + "', ");
                strSql.Append("      t1.dOperatorTime=getDate(), ");
                strSql.Append("      t1.vcLastTimeFlag='" + strLastTimeFlag + "' ");
                strSql.Append("from     \n");
                strSql.Append("(   \n");
                strSql.Append(" select * from TSoq   \n");
                strSql.Append(" WHERE (vcDyState='1' and vcHyState in ('0','3')) ");//这几个状态(1,0,3)是可操作的状态
                strSql.Append("	and vcYearMonth='" + strYearMonth + "'     \n");
                strSql.Append("	and vcSupplier_id='" + strSupplier_id + "'    \n");
                if (strOperState == "Y")
                    strSql.Append("	and vcDyState='1' and vcHyState in ('0','3')    \n");
                else if (strOperState == "N")
                    strSql.Append("	and (vcDyState!='1' or vcHyState not in ('0','3'))    \n");
                if (!string.IsNullOrEmpty(strDyState))//对应状态
                    strSql.Append(" and vcDyState='" + strDyState + "' ");
                if (!string.IsNullOrEmpty(strPart_id))//品番
                    strSql.Append(" and vcPart_id like '%" + strPart_id + "%'");
                strSql.Append(")t1     \n");
                strSql.Append("left join (    \n");
                strSql.Append("select a.vcYearMonth,a.vcPart_id,a.iTzhSOQN,a.iTzhSOQN1,a.iTzhSOQN2        \n");
                strSql.Append("from (       \n");
                strSql.Append("	select * from TSoq_OperHistory where vcInputType='supplier'       \n");
                strSql.Append(")a       \n");
                strSql.Append("inner join (       \n");
                strSql.Append("	select vcYearMonth,vcPart_id,MAX(dOperatorTime) as dOperatorTime from TSoq_OperHistory        \n");
                strSql.Append("	where vcInputType='supplier'       \n");
                strSql.Append("	group by vcYearMonth,vcPart_id       \n");
                strSql.Append(")b on a.vcYearMonth=b.vcYearMonth and a.vcPart_id=b.vcPart_id and a.dOperatorTime=b.dOperatorTime       \n");
                strSql.Append(")t2 on t1.vcYearMonth=t2.vcYearMonth and t1.vcPart_id=t2.vcPart_id    \n");
                //strSql.Append(" UPDATE TSoq SET ");
                //strSql.Append("      vcDyState=case when iCbSOQN!=iTzhSOQN or iCbSOQN1!=iTzhSOQN1 or iCbSOQN2!=iTzhSOQN2 then '2' else '3' end, ");//0：未发送；1：待回复；2：有调整；3：无调整
                //strSql.Append("      vcSReplyUser='" + strUserId + "', ");
                //strSql.Append("      dSReplyTime=getdate(), ");
                //strSql.Append("      vcOperator='" + strUserId + "', ");
                //strSql.Append("      dOperatorTime=getDate(), ");
                //strSql.Append("      vcLastTimeFlag='" + strLastTimeFlag + "' ");
                //strSql.Append(" WHERE (vcDyState='1' and vcHyState in ('0','3')) ");//这几个状态(1,0,3)是可操作的状态
                //strSql.Append("	and vcYearMonth='" + strYearMonth + "'     \n");
                //strSql.Append("	and vcSupplier_id='" + strSupplier_id + "'    \n");
                //if (strOperState == "Y")
                //    strSql.Append("	and vcDyState='1' and vcHyState in ('0','3')    \n");
                //else if (strOperState == "N")
                //    strSql.Append("	and (vcDyState!='1' or vcHyState not in ('0','3'))    \n");
                //if (!string.IsNullOrEmpty(strDyState))//对应状态
                //    strSql.Append(" and vcDyState='" + strDyState + "' ");
                //if (!string.IsNullOrEmpty(strPart_id))//品番
                //    strSql.Append(" and vcPart_id like '%" + strPart_id + "%'");

                //记录日志
                strSql.Append("  INSERT INTO TSoqLog( vcYearMonth,vcPart_id,vcMessage,vcOperator,dOperatorTime)");
                strSql.Append("  select vcYearMonth,vcPart_id,'供应商提交','" + strUserId + "',getDate() from TSoq  where vcLastTimeFlag='" + strLastTimeFlag + "' and vcOperator='" + strUserId + "' ");
                return excute.ExcuteSqlWithStringOper(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 提交-按列表所选数据
        public int ok(string strYearMonth, List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                string strLastTimeFlag = DateTime.Now.ToString("yyyyMMddHHmmss");

                StringBuilder strSql = new StringBuilder();
                strSql.Append("      if object_id('tempdb..#TSoq_temp_cr') is not null       \n");
                strSql.Append("      Begin      \n");
                strSql.Append("      drop  table #TSoq_temp_cr       \n");
                strSql.Append("      End      \n");
                strSql.Append("      select * into #TSoq_temp_cr from       \n");
                strSql.Append("      (      \n");
                strSql.Append("      	select vcPart_id,vcYearMonth,iTzhSOQN,iTzhSOQN1,iTzhSOQN2 from TSoq where 1=0      \n");
                strSql.Append("      ) a      ;\n");
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    #region 将所有的数据都插入临时表
                    strSql.Append("insert into #TSoq_temp_cr       \n");
                    strSql.Append(" (         \n");
                    strSql.Append(" vcPart_id,vcYearMonth,iTzhSOQN,iTzhSOQN1,iTzhSOQN2        \n");
                    strSql.Append(" ) values         \n");
                    strSql.Append("(      \n");
                    strSql.Append(    ComFunction.getSqlValue(listInfoData[i]["vcPart_id"], false) + ",   \n");
                    strSql.Append(    ComFunction.getSqlValue(listInfoData[i]["vcYearMonth"], false) + ",   \n");
                    strSql.Append(    ComFunction.getSqlValue(listInfoData[i]["iTzhSOQN"], false) + ",   \n");
                    strSql.Append(    ComFunction.getSqlValue(listInfoData[i]["iTzhSOQN1"], false) + ",   \n");
                    strSql.Append(    ComFunction.getSqlValue(listInfoData[i]["iTzhSOQN2"], false) + "   \n");
                    strSql.Append(")      \n");
                    #endregion
                }
                //更新对应状态为有调整/无调整，更新提交人和提单时间
                strSql.Append(" UPDATE TSoq SET ");
                strSql.Append("      vcDyState=case when a.iCbSOQN!=b.iTzhSOQN or a.iCbSOQN1!=b.iTzhSOQN1 or a.iCbSOQN2!=b.iTzhSOQN2 then '2' else '3' end, ");//0：未发送；1：待回复；2：有调整；3：无调整
                strSql.Append("      vcSReplyUser='" + strUserId+"', ");
                strSql.Append("      dSReplyTime=getdate(), ");
                strSql.Append("      vcOperator='" + strUserId + "', ");
                strSql.Append("      dOperatorTime=getDate(), ");
                strSql.Append("      vcLastTimeFlag='" + strLastTimeFlag + "' ");
                strSql.Append(" from TSoq a  \n ");
                strSql.Append(" inner join  \n ");
                strSql.Append(" (  \n ");
                strSql.Append("    select * from #TSoq_temp_cr  \n ");
                strSql.Append(" )b on a.vcPart_id=b.vcPart_id and a.vcYearMonth=b.vcYearMonth  \n ");

                //记录日志
                strSql.Append("  INSERT INTO TSoqLog( vcYearMonth,vcPart_id,vcMessage,vcOperator,dOperatorTime)");
                strSql.Append("  select '" + strYearMonth + "' as vcYearMonth,vcPart_id,'供应商提交','" + strUserId + "',getDate() from TSoq where vcLastTimeFlag='" + strLastTimeFlag + "' and vcOperator='" + strUserId + "'  ");
                return excute.ExcuteSqlWithStringOper(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 保存后校验
        public void SaveCheck(List<Dictionary<string, Object>> listInfoData, string strUserId, string strYearMonth, string strYearMonth_2, string strYearMonth_3,
            ref List<string> errMessageList, string strUnit)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(" delete TSoq_temp where vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth + "' ;  \r\n ");

                #region 先插入临时表
                for (int i = 0; i < listInfoData.Count; i++)
                {
                    bool bmodflag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
                    bool baddflag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
                    if (baddflag == true)
                    {//新增
                        //没有新增情况
                    }
                    if (baddflag == false && bmodflag == true)
                    {//修改
                        sql.Append("  INSERT INTO TSoq_temp(vcYearMonth,vcPart_id,vcSupplier_id,iTzhSOQN,iTzhSOQN1,iTzhSOQN2,vcOperator,dOperatorTime,  ");
                        sql.Append("  iCbSOQN,iCbSOQN1,iCbSOQN2) values       \n");
                        sql.Append("('" + strYearMonth + "',");
                        sql.Append("'" + listInfoData[i]["vcPart_id"].ToString() + "',");
                        sql.Append("'" + listInfoData[i]["vcSupplier_id"].ToString() + "',");
                        sql.Append("'" + listInfoData[i]["iTzhSOQN"].ToString() + "',");
                        sql.Append("'" + listInfoData[i]["iTzhSOQN1"].ToString() + "',");
                        sql.Append("'" + listInfoData[i]["iTzhSOQN2"].ToString() + "',");
                        sql.Append("'" + strUserId + "',");
                        sql.Append("getDate(),");
                        sql.Append("'" + listInfoData[i]["iCbSOQN"].ToString() + "',");
                        sql.Append("'" + listInfoData[i]["iCbSOQN1"].ToString() + "',");
                        sql.Append("'" + listInfoData[i]["iCbSOQN2"].ToString() + "'");
                        sql.Append(")");
                    }
                }
                excute.ExcuteSqlWithStringOper(sql.ToString());//先导入临时表，然后check
                #endregion

                #region 验证1：是否为TFTM品番（包装工厂）
                sql.Length = 0;//清空
                sql.Append("   select a.vcPart_id from    \r\n ");
                sql.Append("   (    \r\n ");
                sql.Append("      select * from TSoq_temp where vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth + "'    \r\n ");
                sql.Append("   )a    \r\n ");
                sql.Append("   left join    \r\n ");
                sql.Append("   (    \r\n ");
                sql.Append("      select * from TSPMaster where vcPackingPlant='" + strUnit + "' and vcReceiver='APC06'     \r\n ");
                sql.Append("   )b on a.vcPart_id=b.vcPartId and a.vcSupplier_id=b.vcSupplierId    \r\n ");
                sql.Append("   where b.vcPartId is  null    \r\n ");
                DataTable dt1 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    string strPart_id = dt1.Rows[i]["vcPart_id"].ToString();
                    errMessageList.Add("品番"+strPart_id + "在品番基础信息里不存在");
                }
                #endregion

                #region 验证2：N 月品番有效性(N月得有数量),有数量才校验  
                sql.Length = 0;//清空
                sql.Append("    select a.vcPart_id from    \r\n ");
                sql.Append("    (    \r\n ");
                sql.Append("       select * from TSoq_temp where vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth + "' and iTzhSOQN<>0    \r\n ");
                sql.Append("    )a    \r\n ");
                sql.Append("    inner join      \r\n ");
                sql.Append("    (      \r\n ");
                sql.Append("       select * from TSPMaster where vcPackingPlant='" + strUnit + "' and vcReceiver='APC06' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)   \r\n ");
                sql.Append("    )b on a.vcPart_id=b.vcPartId and a.vcSupplier_id=b.vcSupplierId       \r\n ");
                sql.Append("    where b.vcPartId is  null      \r\n ");
                DataTable dt2 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    string strPart_id = dt2.Rows[i]["vcPart_id"].ToString();
                    errMessageList.Add("品番" + strPart_id + "在品番基础信息存在，但不满足" + strYearMonth + "月有效性条件");
                }
                #endregion

                #region 验证3：N+1 月品番有效性(N+1月得有数量)，有数量才校验
                sql.Length = 0;//清空
                sql.Append("    select a.vcPart_id from    \r\n ");
                sql.Append("    (    \r\n ");
                sql.Append("       select * from TSoq_temp where vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth + "' and iTzhSOQN1<>0   \r\n ");
                sql.Append("    )a    \r\n ");
                sql.Append("    inner join      \r\n ");
                sql.Append("    (      \r\n ");
                sql.Append("       select * from TSPMaster where vcPackingPlant='" + strUnit + "' and vcReceiver='APC06' and '" + strYearMonth_2 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)  \r\n ");
                sql.Append("    )b on a.vcPart_id=b.vcPartId  and a.vcSupplier_id=b.vcSupplierId       \r\n ");
                sql.Append("    where b.vcPartId is  null      \r\n ");
                DataTable dt3 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                for (int i = 0; i < dt3.Rows.Count; i++)
                {
                    string strPart_id = dt3.Rows[i]["vcPart_id"].ToString();
                    errMessageList.Add("品番" + strPart_id + "在品番基础信息存在，但不满足" + strYearMonth_2 + "月有效性条件");
                }
                #endregion

                #region 验证4：N+2 月品番有效性(N+2月得有数量)，有数量才校验 
                sql.Length = 0;//清空
                sql.Append("    select a.vcPart_id from    \r\n ");
                sql.Append("    (    \r\n ");
                sql.Append("       select * from TSoq_temp where vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth + "' and iTzhSOQN2<>0    \r\n ");
                sql.Append("    )a    \r\n ");
                sql.Append("    inner join      \r\n ");
                sql.Append("    (      \r\n ");
                sql.Append("       select * from TSPMaster where vcPackingPlant='" + strUnit + "' and vcReceiver='APC06' and '" + strYearMonth_3 + "'  between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)       \r\n ");
                sql.Append("    )b on a.vcPart_id=b.vcPartId  and a.vcSupplier_id=b.vcSupplierId       \r\n ");
                sql.Append("    where b.vcPartId is  null      \r\n ");
                DataTable dt4 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                for (int i = 0; i < dt4.Rows.Count; i++)
                {
                    string strPart_id = dt4.Rows[i]["vcPart_id"].ToString();
                    errMessageList.Add("品番" + strPart_id + "在品番基础信息存在，但不满足" + strYearMonth_3 + "月有效性条件");
                }
                #endregion

                #region 验证5：是否有价格，且在有效期内(只判断N月)，数量为0不校验；如果是强制订货，则没有价格也可以
                sql.Length = 0;//清空
                sql.Append("    select a.vcPart_id from     \r\n ");
                sql.Append("    (     \r\n ");
                sql.Append("       select * from TSoq_temp where vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth + "' and iTzhSOQN<>0    \r\n ");
                sql.Append("    )a     \r\n ");
                sql.Append("    left join     \r\n ");
                sql.Append("    (     \r\n ");
                sql.Append("       select vcPart_id,vcSupplier_id from TPrice where  convert(varchar(6),dUseBegin,112)<='" + strYearMonth + "' and convert(varchar(6),dUseEnd,112)>='" + strYearMonth + "'     \r\n ");
                sql.Append("    )b on a.vcPart_id=b.vcPart_id  and a.vcSupplier_id=b.vcSupplier_id      \r\n ");
                sql.Append("    left join       \n");
                sql.Append("    (      \n");
                sql.Append("    	select vcPartId,vcMandOrder,vcSupplierId      \n");
                sql.Append("    	from TSPMaster           \n");
                sql.Append("    	where vcPackingPlant='" + strUnit + "' and vcReceiver='APC06'           \n");
                sql.Append("    	and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)       \n");
                sql.Append("    )c on a.vcPart_id=c.vcPartId  and a.vcSupplier_id=c.vcSupplierId         \n");
                sql.Append("    where b.vcPart_id is  null  and isnull(c.vcMandOrder,'')<>'1'      \r\n ");// --  vcMandOrder='1' 是强制订货 
                DataTable dt5 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                for (int i = 0; i < dt5.Rows.Count; i++)
                {
                    string strPart_id = dt5.Rows[i]["vcPart_id"].ToString();
                    errMessageList.Add("品番" + strPart_id + "在" + strYearMonth + "月没有维护价格");
                }
                #endregion

                #region 验证6：手配中是否有受入、收容数、发注工厂（N、N+1、N+2都判断），数量为0不校验
                #region N月的
                sql.Length = 0;//清空
                sql.Append("select t1.vcPart_id,t3.vcOrderPlant,t5.iPackingQty,t6.vcSufferIn    \n");
                sql.Append("from (    \n");
                sql.Append("	select * from TSoq_temp where vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth + "' and iTzhSOQN<>0    \n");
                sql.Append(")t1        \n");
                sql.Append("left join (    \n");
                sql.Append("	select vcPartId,vcCarfamilyCode,vcHaoJiu,vcReceiver,vcPackingPlant,vcSupplierId,vcInOut    \n");
                sql.Append("	from TSPMaster     \n");
                sql.Append("	where vcPackingPlant='" + strUnit + "' and vcReceiver='APC06'     \n");
                sql.Append("	and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t2 on t1.vcPart_id=t2.vcPartId and t1.vcSupplier_id=t2.vcSupplierId     \n");
                sql.Append("left join (    \n");//发注工厂
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcOrderPlant     \n");
                sql.Append("	from TSPMaster_OrderPlant     \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t3 on t2.vcPartId=t3.vcPartId and t2.vcPackingPlant=t3.vcPackingPlant     \n");
                sql.Append("and t2.vcReceiver=t3.vcReceiver and t2.vcSupplierId=t3.vcSupplierId    \n");
                sql.Append("left join (    \n");//供应商工区
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant     \n");
                sql.Append("	from TSPMaster_SupplierPlant     \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t4 on t2.vcPartId=t4.vcPartId and t2.vcPackingPlant=t4.vcPackingPlant     \n");
                sql.Append("and t2.vcReceiver=t4.vcReceiver and t2.vcSupplierId=t4.vcSupplierId    \n");
                sql.Append("left join(    \n");//收容数
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant,iPackingQty     \n");
                sql.Append("	from TSPMaster_Box     \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t5 on t2.vcPartId=t5.vcPartId and t2.vcPackingPlant=t5.vcPackingPlant     \n");
                sql.Append("and t2.vcReceiver=t5.vcReceiver and t2.vcSupplierId=t5.vcSupplierId     \n");
                sql.Append("and t4.vcSupplierPlant=t5.vcSupplierPlant    \n");
                sql.Append("left join     \n");
                sql.Append("(    \n");//受入
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSufferIn from TSPMaster_SufferIn    \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t6 on t2.vcPartId=t6.vcPartId and t2.vcPackingPlant=t6.vcPackingPlant     \n");
                sql.Append("and t2.vcReceiver=t6.vcReceiver and t2.vcSupplierId=t6.vcSupplierId    \n");
                sql.Append("where t3.vcPartId is null or t5.vcPartId is null or t6.vcPartId is null       \r\n ");
                DataTable dt6_1 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                for (int i = 0; i < dt6_1.Rows.Count; i++)
                {
                    string strPart_id = dt6_1.Rows[i]["vcPart_id"].ToString();
                    string vcOrderPlant = dt6_1.Rows[i]["vcOrderPlant"].ToString();//发注工厂
                    string iPackingQty = dt6_1.Rows[i]["iPackingQty"].ToString();//收容数
                    string vcSufferIn = dt6_1.Rows[i]["vcSufferIn"].ToString();//受入
                    if (vcSufferIn == "")
                        errMessageList.Add("品番" + strPart_id + "在" + strYearMonth + "月没有维护受入");
                    if (iPackingQty == "")
                        errMessageList.Add("品番" + strPart_id + "在" + strYearMonth + "月没有维护收容数");
                    if (vcOrderPlant == "")
                        errMessageList.Add("品番" + strPart_id + "在" + strYearMonth + "月没有维护发注工厂");
                }
                #endregion
                #region N+1月的
                sql.Length = 0;//清空
                sql.Append("select t1.vcPart_id,t3.vcOrderPlant,t5.iPackingQty,t6.vcSufferIn    \n");
                sql.Append("from (    \n");
                sql.Append("	select * from TSoq_temp where vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth + "' and iTzhSOQN1<>0    \n");
                sql.Append(")t1        \n");
                sql.Append("left join (    \n");
                sql.Append("	select vcPartId,vcCarfamilyCode,vcHaoJiu,vcReceiver,vcPackingPlant,vcSupplierId,vcInOut    \n");
                sql.Append("	from TSPMaster     \n");
                sql.Append("	where vcPackingPlant='" + strUnit + "' and vcReceiver='APC06'     \n");
                sql.Append("	and '" + strYearMonth_2 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t2 on t1.vcPart_id=t2.vcPartId and t1.vcSupplier_id=t2.vcSupplierId    \n");
                sql.Append("left join (    \n");//发注工厂
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcOrderPlant     \n");
                sql.Append("	from TSPMaster_OrderPlant     \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth_2 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t3 on t2.vcPartId=t3.vcPartId and t2.vcPackingPlant=t3.vcPackingPlant     \n");
                sql.Append("and t2.vcReceiver=t3.vcReceiver and t2.vcSupplierId=t3.vcSupplierId    \n");
                sql.Append("left join (    \n");//供应商工区
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant     \n");
                sql.Append("	from TSPMaster_SupplierPlant     \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth_2 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t4 on t2.vcPartId=t4.vcPartId and t2.vcPackingPlant=t4.vcPackingPlant     \n");
                sql.Append("and t2.vcReceiver=t4.vcReceiver and t2.vcSupplierId=t4.vcSupplierId    \n");
                sql.Append("left join(    \n");//收容数
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant,iPackingQty     \n");
                sql.Append("	from TSPMaster_Box     \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth_2 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t5 on t2.vcPartId=t5.vcPartId and t2.vcPackingPlant=t5.vcPackingPlant     \n");
                sql.Append("and t2.vcReceiver=t5.vcReceiver and t2.vcSupplierId=t5.vcSupplierId     \n");
                sql.Append("and t4.vcSupplierPlant=t5.vcSupplierPlant    \n");
                sql.Append("left join     \n");
                sql.Append("(    \n");//受入
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSufferIn from TSPMaster_SufferIn    \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth_2 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t6 on t2.vcPartId=t6.vcPartId and t2.vcPackingPlant=t6.vcPackingPlant     \n");
                sql.Append("and t2.vcReceiver=t6.vcReceiver and t2.vcSupplierId=t6.vcSupplierId    \n");
                sql.Append("where t3.vcPartId is null or t5.vcPartId is null or t6.vcPartId is null       \r\n ");
                DataTable dt6_2 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                for (int i = 0; i < dt6_2.Rows.Count; i++)
                {
                    string strPart_id = dt6_2.Rows[i]["vcPart_id"].ToString();
                    string vcOrderPlant = dt6_2.Rows[i]["vcOrderPlant"].ToString();//发注工厂
                    string iPackingQty = dt6_2.Rows[i]["iPackingQty"].ToString();//收容数
                    string vcSufferIn = dt6_2.Rows[i]["vcSufferIn"].ToString();//受入
                    if (vcSufferIn == "")
                        errMessageList.Add("品番" + strPart_id + "在" + strYearMonth_2 + "月没有维护受入");
                    if (iPackingQty == "")
                        errMessageList.Add("品番" + strPart_id + "在" + strYearMonth_2 + "月没有维护收容数");
                    if (vcOrderPlant == "")
                        errMessageList.Add("品番" + strPart_id + "在" + strYearMonth_2 + "月没有维护发注工厂");
                }
                #endregion
                #region N+2月的
                sql.Length = 0;//清空
                sql.Append("select t1.vcPart_id,t3.vcOrderPlant,t5.iPackingQty,t6.vcSufferIn    \n");
                sql.Append("from (    \n");
                sql.Append("	select * from TSoq_temp where vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth + "' and iTzhSOQN2<>0    \n");
                sql.Append(")t1        \n");
                sql.Append("left join (    \n");
                sql.Append("	select vcPartId,vcCarfamilyCode,vcHaoJiu,vcReceiver,vcPackingPlant,vcSupplierId,vcInOut    \n");
                sql.Append("	from TSPMaster     \n");
                sql.Append("	where vcPackingPlant='" + strUnit + "' and vcReceiver='APC06'     \n");
                sql.Append("	and '" + strYearMonth_3 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t2 on t1.vcPart_id=t2.vcPartId and t1.vcSupplier_id=t2.vcSupplierId    \n");
                sql.Append("left join (    \n");//发注工厂
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcOrderPlant     \n");
                sql.Append("	from TSPMaster_OrderPlant     \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth_3 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t3 on t2.vcPartId=t3.vcPartId and t2.vcPackingPlant=t3.vcPackingPlant     \n");
                sql.Append("and t2.vcReceiver=t3.vcReceiver and t2.vcSupplierId=t3.vcSupplierId    \n");
                sql.Append("left join (    \n");//供应商工区
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant     \n");
                sql.Append("	from TSPMaster_SupplierPlant     \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth_3 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t4 on t2.vcPartId=t4.vcPartId and t2.vcPackingPlant=t4.vcPackingPlant     \n");
                sql.Append("and t2.vcReceiver=t4.vcReceiver and t2.vcSupplierId=t4.vcSupplierId    \n");
                sql.Append("left join(    \n");//收容数
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant,iPackingQty     \n");
                sql.Append("	from TSPMaster_Box     \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth_3 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t5 on t2.vcPartId=t5.vcPartId and t2.vcPackingPlant=t5.vcPackingPlant     \n");
                sql.Append("and t2.vcReceiver=t5.vcReceiver and t2.vcSupplierId=t5.vcSupplierId     \n");
                sql.Append("and t4.vcSupplierPlant=t5.vcSupplierPlant    \n");
                sql.Append("left join     \n");
                sql.Append("(    \n");//受入
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSufferIn from TSPMaster_SufferIn    \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth_3 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t6 on t2.vcPartId=t6.vcPartId and t2.vcPackingPlant=t6.vcPackingPlant     \n");
                sql.Append("and t2.vcReceiver=t6.vcReceiver and t2.vcSupplierId=t6.vcSupplierId    \n");
                sql.Append("where t3.vcPartId is null or t5.vcPartId is null or t6.vcPartId is null       \r\n ");
                DataTable dt6_3 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                for (int i = 0; i < dt6_3.Rows.Count; i++)
                {
                    string strPart_id = dt6_3.Rows[i]["vcPart_id"].ToString();
                    string vcOrderPlant = dt6_3.Rows[i]["vcOrderPlant"].ToString();//发注工厂
                    string iPackingQty = dt6_3.Rows[i]["iPackingQty"].ToString();//收容数
                    string vcSufferIn = dt6_3.Rows[i]["vcSufferIn"].ToString();//受入
                    if (vcSufferIn == "")
                        errMessageList.Add("品番" + strPart_id + "在" + strYearMonth_3 + "月没有维护受入");
                    if (iPackingQty == "")
                        errMessageList.Add("品番" + strPart_id + "在" + strYearMonth_3 + "月没有维护收容数");
                    if (vcOrderPlant == "")
                        errMessageList.Add("品番" + strPart_id + "在" + strYearMonth_3 + "月没有维护发注工厂");
                }
                #endregion
                #endregion

                #region 验证7：受入、收容数、发注工厂、供应商工区：3个月必须一样
                sql.Length = 0;//清空
                sql.Append("select t1.vcPart_id,t3.vcOrderPlant,t3_1.vcOrderPlant as vcOrderPlant_1,t3_2.vcOrderPlant as vcOrderPlant_2,    \n");
                sql.Append("t5.iPackingQty,t5_1.iPackingQty as iPackingQty_1,t5_2.iPackingQty as iPackingQty_2,    \n");
                sql.Append("t6.vcSufferIn, t6_1.vcSufferIn as vcSufferIn_1,t6_2.vcSufferIn as vcSufferIn_2   \n");
                sql.Append("from (    \n");
                sql.Append("	select * from TSoq_temp where vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth + "' and (iTzhSOQN<>0 or iTzhSOQN1<>0 or iTzhSOQN2<>0)    \n");
                sql.Append(")t1        \n");
                sql.Append("left join (    \n");
                sql.Append("	select vcPartId,vcCarfamilyCode,vcHaoJiu,vcReceiver,vcPackingPlant,vcSupplierId,vcInOut    \n");
                sql.Append("	from TSPMaster     \n");
                sql.Append("	where vcPackingPlant='" + strUnit + "' and vcReceiver='APC06'     \n");
                sql.Append("	and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t2 on t1.vcPart_id=t2.vcPartId and t1.vcSupplier_id=t2.vcSupplierId   \n");
                sql.Append("left join (    \n");//发注工厂 N月
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcOrderPlant     \n");
                sql.Append("	from TSPMaster_OrderPlant     \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t3 on t2.vcPartId=t3.vcPartId and t2.vcPackingPlant=t3.vcPackingPlant     \n");
                sql.Append("and t2.vcReceiver=t3.vcReceiver and t2.vcSupplierId=t3.vcSupplierId    \n");
                sql.Append("left join (    \n");//发注工厂 N+1月
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcOrderPlant     \n");
                sql.Append("	from TSPMaster_OrderPlant     \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth_2 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t3_1 on t2.vcPartId=t3_1.vcPartId and t2.vcPackingPlant=t3_1.vcPackingPlant     \n");
                sql.Append("and t2.vcReceiver=t3_1.vcReceiver and t2.vcSupplierId=t3_1.vcSupplierId    \n");
                sql.Append("left join (    \n");//发注工厂 N+2月
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcOrderPlant     \n");
                sql.Append("	from TSPMaster_OrderPlant     \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth_3 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t3_2 on t2.vcPartId=t3_2.vcPartId and t2.vcPackingPlant=t3_2.vcPackingPlant     \n");
                sql.Append("and t2.vcReceiver=t3_2.vcReceiver and t2.vcSupplierId=t3_2.vcSupplierId    \n");
                sql.Append("left join (    \n");//供应商工区 N月
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant     \n");
                sql.Append("	from TSPMaster_SupplierPlant     \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t4 on t2.vcPartId=t4.vcPartId and t2.vcPackingPlant=t4.vcPackingPlant     \n");
                sql.Append("and t2.vcReceiver=t4.vcReceiver and t2.vcSupplierId=t4.vcSupplierId    \n");
                sql.Append("left join (    \n");//供应商工区 N+1月
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant     \n");
                sql.Append("	from TSPMaster_SupplierPlant     \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth_2 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t4_1 on t2.vcPartId=t4_1.vcPartId and t2.vcPackingPlant=t4_1.vcPackingPlant     \n");
                sql.Append("and t2.vcReceiver=t4_1.vcReceiver and t2.vcSupplierId=t4_1.vcSupplierId    \n");
                sql.Append("left join (    \n");//供应商工区 N+2月
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant     \n");
                sql.Append("	from TSPMaster_SupplierPlant     \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth_3 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t4_2 on t2.vcPartId=t4_2.vcPartId and t2.vcPackingPlant=t4_2.vcPackingPlant     \n");
                sql.Append("and t2.vcReceiver=t4_2.vcReceiver and t2.vcSupplierId=t4_2.vcSupplierId    \n");
                sql.Append("left join(    \n");//收容数 N月
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant,iPackingQty     \n");
                sql.Append("	from TSPMaster_Box     \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t5 on t2.vcPartId=t5.vcPartId and t2.vcPackingPlant=t5.vcPackingPlant     \n");
                sql.Append("and t2.vcReceiver=t5.vcReceiver and t2.vcSupplierId=t5.vcSupplierId     \n");
                sql.Append("and t4.vcSupplierPlant=t5.vcSupplierPlant    \n");
                sql.Append("left join(    \n");//收容数 N+1月
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant,iPackingQty     \n");
                sql.Append("	from TSPMaster_Box     \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth_2 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t5_1 on t2.vcPartId=t5_1.vcPartId and t2.vcPackingPlant=t5_1.vcPackingPlant     \n");
                sql.Append("and t2.vcReceiver=t5_1.vcReceiver and t2.vcSupplierId=t5_1.vcSupplierId     \n");
                sql.Append("and t4_1.vcSupplierPlant=t5_1.vcSupplierPlant    \n");
                sql.Append("left join(    \n");//收容数 N+2月
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant,iPackingQty     \n");
                sql.Append("	from TSPMaster_Box     \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth_3 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t5_2 on t2.vcPartId=t5_2.vcPartId and t2.vcPackingPlant=t5_2.vcPackingPlant     \n");
                sql.Append("and t2.vcReceiver=t5_2.vcReceiver and t2.vcSupplierId=t5_2.vcSupplierId     \n");
                sql.Append("and t4_2.vcSupplierPlant=t5_2.vcSupplierPlant    \n");
                sql.Append("left join     \n");
                sql.Append("(    \n");//受入 N月
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSufferIn from TSPMaster_SufferIn    \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t6 on t2.vcPartId=t6.vcPartId and t2.vcPackingPlant=t6.vcPackingPlant     \n");
                sql.Append("and t2.vcReceiver=t6.vcReceiver and t2.vcSupplierId=t6.vcSupplierId    \n");
                sql.Append("left join     \n");
                sql.Append("(    \n");//受入 N+1月
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSufferIn from TSPMaster_SufferIn    \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth_2 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t6_1 on t2.vcPartId=t6_1.vcPartId and t2.vcPackingPlant=t6_1.vcPackingPlant     \n");
                sql.Append("and t2.vcReceiver=t6_1.vcReceiver and t2.vcSupplierId=t6_1.vcSupplierId    \n");
                sql.Append("left join     \n");
                sql.Append("(    \n");//受入 N+2月
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSufferIn from TSPMaster_SufferIn    \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth_3 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t6_2 on t2.vcPartId=t6_2.vcPartId and t2.vcPackingPlant=t6_2.vcPackingPlant     \n");
                sql.Append("and t2.vcReceiver=t6_2.vcReceiver and t2.vcSupplierId=t6_2.vcSupplierId    \n");
                sql.Append("where (t3.vcOrderPlant<>t3_1.vcOrderPlant or t3.vcOrderPlant<>t3_2.vcOrderPlant) or     \r\n ");//发注工厂
                sql.Append(" (t5.iPackingQty<>t5_1.iPackingQty or t5.iPackingQty<>t5_2.iPackingQty) or     \r\n ");//收容数
                sql.Append(" (t6.vcSufferIn<>t6_1.vcSufferIn or t6.vcSufferIn<>t6_2.vcSufferIn) or     \r\n ");//受入
                sql.Append(" (t4.vcSupplierPlant<>t4_1.vcSupplierPlant or t4.vcSupplierPlant<>t4_2.vcSupplierPlant)      \r\n ");//供应商工区
                DataTable dt7 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                for (int i = 0; i < dt7.Rows.Count; i++)
                {
                    string strPart_id = dt7.Rows[i]["vcPart_id"].ToString();
                    string vcOrderPlant = dt7.Rows[i]["vcOrderPlant"].ToString();//发注工厂 N月
                    string vcOrderPlant_1 = dt7.Rows[i]["vcOrderPlant_1"].ToString();//发注工厂 N+1月
                    string vcOrderPlant_2 = dt7.Rows[i]["vcOrderPlant_2"].ToString();//发注工厂 N+2月
                    string iPackingQty = dt7.Rows[i]["iPackingQty"].ToString();//收容数 N月
                    string iPackingQty_1 = dt7.Rows[i]["iPackingQty_1"].ToString();//收容数 N+1月
                    string iPackingQty_2 = dt7.Rows[i]["iPackingQty_2"].ToString();//收容数 N+2月
                    string vcSufferIn = dt7.Rows[i]["vcSufferIn"].ToString();//受入 N月
                    string vcSufferIn_1 = dt7.Rows[i]["vcSufferIn_1"].ToString();//受入 N+1月
                    string vcSufferIn_2 = dt7.Rows[i]["vcSufferIn_2"].ToString();//受入 N+2月
                    string vcSupplierPlant = dt7.Rows[i]["vcSupplierPlant"].ToString();//供应商工区 N月
                    string vcSupplierPlant_1 = dt7.Rows[i]["vcSupplierPlant_1"].ToString();//供应商工区 N+1月
                    string vcSupplierPlant_2 = dt7.Rows[i]["vcSupplierPlant_2"].ToString();//供应商工区 N+2月
                    if (vcOrderPlant != vcOrderPlant_1 || vcOrderPlant != vcOrderPlant_2)
                        errMessageList.Add("品番" + strPart_id + "在3个月维护的发注工厂不一致");
                    if (iPackingQty != iPackingQty_1 || iPackingQty != iPackingQty_2)
                        errMessageList.Add("品番" + strPart_id + "在3个月维护的收容数不一致");
                    if (vcSufferIn != vcSufferIn_1 || vcSufferIn != vcSufferIn_2)
                        errMessageList.Add("品番" + strPart_id + "在3个月维护的受入不一致");
                    if (vcSupplierPlant != vcSupplierPlant_1 || vcSupplierPlant != vcSupplierPlant_2)
                        errMessageList.Add("品番" + strPart_id + "在3个月维护的供应商工区不一致");
                }
                #endregion

                #region 验证8：品番3个月数量不能全为0
                sql.Length = 0;//清空
                sql.Append("select vcPart_id,iTzhSOQN,iTzhSOQN1,iTzhSOQN2 from TSoq_temp where vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth + "'    \n");
                sql.Append("and ISNULL(iTzhSOQN,0)=0 and ISNULL(iTzhSOQN1,0)=0 and ISNULL(iTzhSOQN2,0)=0     \n");
                DataTable dt8 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                for (int i = 0; i < dt8.Rows.Count; i++)
                {
                    string strPart_id = dt8.Rows[i]["vcPart_id"].ToString();
                    errMessageList.Add("品番"+strPart_id + "的3个月数量不能全部为0");
                }
                #endregion

                #region 验证8-1：调整3个月数量加总必须等于初版3个月数量加总
                sql.Length = 0;//清空
                sql.Append("select vcPart_id,iTzhSOQN,iTzhSOQN1,iTzhSOQN2 from TSoq_temp where vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth + "'    \n");
                sql.Append("and ISNULL(iTzhSOQN,0)+ISNULL(iTzhSOQN1,0)+ISNULL(iTzhSOQN2,0)!=ISNULL(iCbSOQN,0)+ISNULL(iCbSOQN1,0)+ISNULL(iCbSOQN2,0)     \n");
                DataTable dt8_1 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                for (int i = 0; i < dt8_1.Rows.Count; i++)
                {
                    string strPart_id = dt8_1.Rows[i]["vcPart_id"].ToString();
                    errMessageList.Add("品番" + strPart_id + "的3个月调整数量总和与初版不一致");
                }
                #endregion

                #region 验证9：收容数整倍数
                sql.Length = 0;//清空
                sql.Append("select t1.vcPart_id,t1.iTzhSOQN%t5.iPackingQty as iTzhSOQN,t1.iTzhSOQN1%t5.iPackingQty as iTzhSOQN1,    \n");
                sql.Append("t1.iTzhSOQN2%t5.iPackingQty as iTzhSOQN2    \n");
                sql.Append("from (        \n");
                sql.Append("	select * from TSoq_temp where vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth + "'         \n");
                sql.Append(")t1            \n");
                sql.Append("left join (        \n");
                sql.Append("	select vcPartId,vcCarfamilyCode,vcHaoJiu,vcReceiver,vcPackingPlant,vcSupplierId,vcInOut        \n");
                sql.Append("	from TSPMaster         \n");
                sql.Append("	where vcPackingPlant='" + strUnit + "' and vcReceiver='APC06'         \n");
                sql.Append("	and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)        \n");
                sql.Append(")t2 on t1.vcPart_id=t2.vcPartId and t1.vcSupplier_id=t2.vcSupplierId       \n");
                sql.Append("left join (    --//供应商工区    \n");
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant         \n");
                sql.Append("	from TSPMaster_SupplierPlant         \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)        \n");
                sql.Append(")t4 on t2.vcPartId=t4.vcPartId and t2.vcPackingPlant=t4.vcPackingPlant         \n");
                sql.Append("and t2.vcReceiver=t4.vcReceiver and t2.vcSupplierId=t4.vcSupplierId        \n");
                sql.Append("left join(    --//收容数    \n");
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant,iPackingQty         \n");
                sql.Append("	from TSPMaster_Box         \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)        \n");
                sql.Append(")t5 on t2.vcPartId=t5.vcPartId and t2.vcPackingPlant=t5.vcPackingPlant         \n");
                sql.Append("and t2.vcReceiver=t5.vcReceiver and t2.vcSupplierId=t5.vcSupplierId         \n");
                sql.Append("and t4.vcSupplierPlant=t5.vcSupplierPlant        \n");
                sql.Append("where t1.iTzhSOQN%t5.iPackingQty<>0 or t1.iTzhSOQN1%t5.iPackingQty<>0 or t1.iTzhSOQN2%t5.iPackingQty<>0       \n");
                DataTable dt9 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                for (int i = 0; i < dt9.Rows.Count; i++)
                {
                    string strPart_id = dt9.Rows[i]["vcPart_id"].ToString();
                    errMessageList.Add("品番" + strPart_id + "数量不是收容数的整数倍。");
                }
                #endregion

                #region 验证10：if一括生产 校验： 对象月 >= 实施年月时间 不能订货(数量得是0) 3个月都校验
                #region N月
                sql.Length = 0;//清空
                sql.Append("select t1.vcPart_id,t2.dDebugTime    \n");
                sql.Append("from (        \n");
                sql.Append("	select * from TSoq_temp where vcOperator='" + strUserId + "' and  vcYearMonth='" + strYearMonth + "'         \n");
                sql.Append(")t1            \n");
                sql.Append("left join (        \n");
                sql.Append("	select vcPartId,dDebugTime,vcSupplierId       \n");
                sql.Append("	from TSPMaster         \n");
                sql.Append("	where vcPackingPlant='"+strUnit+"' and vcReceiver='APC06'         \n");
                sql.Append("	and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)     \n");
                sql.Append(")t2 on t1.vcPart_id=t2.vcPartId and t1.vcSupplier_id=t2.vcSupplierId    \n");
                sql.Append("where '" + strYearMonth + "'>=convert(varchar(6),t2.dDebugTime,112) and t1.iTzhSOQN>0    \n");
                DataTable dt10 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                for (int i = 0; i < dt10.Rows.Count; i++)
                {
                    string strPart_id = dt10.Rows[i]["vcPart_id"].ToString();
                    errMessageList.Add("品番" + strPart_id + "在" + strYearMonth + "月不能订货。");
                }
                #endregion
                #region N+1月
                sql.Length = 0;//清空
                sql.Append("select t1.vcPart_id,t2.dDebugTime    \n");
                sql.Append("from (        \n");
                sql.Append("	select * from TSoq_temp where vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth + "'         \n");
                sql.Append(")t1            \n");
                sql.Append("left join (        \n");
                sql.Append("	select vcPartId,dDebugTime,vcSupplierId       \n");
                sql.Append("	from TSPMaster         \n");
                sql.Append("	where vcPackingPlant='"+ strUnit + "' and vcReceiver='APC06'         \n");
                sql.Append("	and '" + strYearMonth_2 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)     \n");
                sql.Append(")t2 on t1.vcPart_id=t2.vcPartId and t1.vcSupplier_id=t2.vcSupplierId     \n");
                sql.Append("where '" + strYearMonth_2 + "'>=convert(varchar(6),t2.dDebugTime,112) and t1.iTzhSOQN1>0    \n");
                DataTable dt10_1 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                for (int i = 0; i < dt10_1.Rows.Count; i++)
                {
                    string strPart_id = dt10_1.Rows[i]["vcPart_id"].ToString();
                    errMessageList.Add("品番" + strPart_id + "在" + strYearMonth_2 + "月不能订货。");
                }
                #endregion
                #region N+2月
                sql.Length = 0;//清空
                sql.Append("select t1.vcPart_id,t2.dDebugTime    \n");
                sql.Append("from (        \n");
                sql.Append("	select * from TSoq_temp where vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth + "'         \n");
                sql.Append(")t1            \n");
                sql.Append("left join (        \n");
                sql.Append("	select vcPartId,dDebugTime,vcSupplierId       \n");
                sql.Append("	from TSPMaster         \n");
                sql.Append("	where vcPackingPlant='"+ strUnit + "' and vcReceiver='APC06'         \n");
                sql.Append("	and '" + strYearMonth_3 + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)     \n");
                sql.Append(")t2 on t1.vcPart_id=t2.vcPartId  and t1.vcSupplier_id=t2.vcSupplierId    \n");
                sql.Append("where '" + strYearMonth_3 + "'>=convert(varchar(6),t2.dDebugTime,112) and t1.iTzhSOQN2>0    \n");
                DataTable dt10_2 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                for (int i = 0; i < dt10_2.Rows.Count; i++)
                {
                    string strPart_id = dt10_2.Rows[i]["vcPart_id"].ToString();
                    errMessageList.Add("品番" + strPart_id + "在" + strYearMonth_3 + "月不能订货。");
                }
                #endregion
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 导入后保存
        public void importSave(string strYearMonth, string strUserId, string strUnit)
        {
            try
            {
                string strLastTimeFlag = DateTime.Now.ToString("yyyyMMddHHmmss");
                DateTime now = DateTime.Now;
                StringBuilder sql = new StringBuilder();
                //插历史
                sql.Append("insert into TSoq_OperHistory (vcYearMonth,vcPart_id,iTzhSOQN,iTzhSOQN1,iTzhSOQN2,vcInputType,vcOperator,dOperatorTime)    \n");
                sql.Append("select vcYearMonth,vcPart_id,iTzhSOQN,iTzhSOQN1,iTzhSOQN2,'supplier',vcOperator,GETDATE()     \n");
                sql.Append("from TSoq_temp     \n");
                sql.Append("where vcYearMonth='" + strYearMonth + "' and vcOperator='" + strUserId + "'    \n");
                //更新调整后字段
                sql.Append("update t2 set t2.iTzhSOQN=t1.iTzhSOQN,t2.iTzhSOQN1=t1.iTzhSOQN1,t2.iTzhSOQN2=t1.iTzhSOQN2,    \n");
                sql.Append("vcOperator='" + strUserId + "',dOperatorTime=GETDATE(),vcLastTimeFlag='" + strLastTimeFlag + "'     \n");
                sql.Append("from(    \n");
                sql.Append("	select * from TSoq_temp where vcYearMonth='" + strYearMonth + "' and vcOperator='" + strUserId + "'    \n");
                sql.Append(")t1    \n");
                sql.Append("left join (    \n");
                sql.Append("	select * from  TSoq where vcYearMonth='" + strYearMonth + "'    \n");
                sql.Append(")t2 on t1.vcYearMonth=t2.vcYearMonth and t1.vcPart_id=t2.vcPart_id    \n");
                sql.Append("where t1.iTzhSOQN<>t2.iTzhSOQN or t1.iTzhSOQN1<>t2.iTzhSOQN1 or t1.iTzhSOQN2<>t2.iTzhSOQN2    \n");
                //更新tsoq中手配相关字段
                //sql.Append("update t1 set     \n");
                //sql.Append("t1.vcCarFamilyCode=t2.vcCarfamilyCode,    \n");
                //sql.Append("t1.vcCurrentPastcode=t2.vcHaoJiu,    \n");
                //sql.Append("t1.vcMakingOrderType=t2.vcReceiver,    \n");
                //sql.Append("t1.vcFZGC=t3.vcOrderPlant,    \n");
                //sql.Append("t1.vcInOutFlag=t2.vcInOut,    \n");
                //sql.Append("t1.vcSupplier_id=t2.vcSupplierId,    \n");
                //sql.Append("t1.vcSupplierPlant=t4.vcSupplierPlant,    \n");
                //sql.Append("t1.iQuantityPercontainer=t5.iPackingQty   \n");
                //sql.Append("from (        \n");
                //sql.Append("	select * from TSoq where vcYearMonth='" + strYearMonth + "'    \n");
                //sql.Append(")t1        \n");
                //sql.Append("inner join (    \n");
                //sql.Append("	select * from TSoq_temp where vcYearMonth='" + strYearMonth + "' and vcOperator='" + strUserId + "'    \n");
                //sql.Append(")t1_1 on t1.vcYearMonth=t1_1.vcYearMonth and t1.vcPart_id=t1_1.vcPart_id    \n");
                //sql.Append("left join (    \n");
                //sql.Append("	select vcPartId,vcCarfamilyCode,vcHaoJiu,vcReceiver,vcPackingPlant,vcSupplierId,vcInOut    \n");
                //sql.Append("	from TSPMaster     \n");
                //sql.Append("	where vcPackingPlant='" + strUnit + "' and vcReceiver='APC06'     \n");
                //sql.Append("	and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)     \n");
                //sql.Append(")t2 on t1.vcPart_id=t2.vcPartId    \n");
                //sql.Append("left join (    \n");
                //sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcOrderPlant     \n");
                //sql.Append("	from TSPMaster_OrderPlant     \n");
                //sql.Append("	where vcOperatorType='1' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                //sql.Append(")t3 on t2.vcPartId=t3.vcPartId and t2.vcPackingPlant=t3.vcPackingPlant     \n");
                //sql.Append("and t2.vcReceiver=t3.vcReceiver and t2.vcSupplierId=t3.vcSupplierId    \n");
                //sql.Append("left join (    \n");
                //sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant     \n");
                //sql.Append("	from TSPMaster_SupplierPlant     \n");
                //sql.Append("	where vcOperatorType='1' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                //sql.Append(")t4 on t2.vcPartId=t4.vcPartId and t2.vcPackingPlant=t4.vcPackingPlant     \n");
                //sql.Append("and t2.vcReceiver=t4.vcReceiver and t2.vcSupplierId=t4.vcSupplierId    \n");
                //sql.Append("left join(    \n");
                //sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant,iPackingQty     \n");
                //sql.Append("	from TSPMaster_Box     \n");
                //sql.Append("	where vcOperatorType='1' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                //sql.Append(")t5 on t2.vcPartId=t5.vcPartId and t2.vcPackingPlant=t5.vcPackingPlant     \n");
                //sql.Append("and t2.vcReceiver=t5.vcReceiver and t2.vcSupplierId=t5.vcSupplierId     \n");
                //sql.Append("and t4.vcSupplierPlant=t5.vcSupplierPlant    \n");
                //走到保存，则异常信息肯定没有了，删除TSoqInputErrDetail_Save
                sql.Append(" delete TSoqInputErrDetail_Save where vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth + "' ;  \r\n ");
                //记录日志
                sql.AppendLine("  INSERT INTO TSoqLog( vcYearMonth,vcPart_id,vcMessage,vcOperator,dOperatorTime)");
                sql.AppendLine("  select vcYearMonth,vcPart_id,'供应商保存成功','" + strUserId + "',getDate() from TSoq  where vcLastTimeFlag='" + strLastTimeFlag + "' and vcOperator='" + strUserId + "' ");

                excute.ExcuteSqlWithStringOper(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }
}