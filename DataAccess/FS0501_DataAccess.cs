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
        public DataTable Search(string strYearMonth, string strSupplier_id, string strPart_id, string strDyState, string strOperState, string strWorkArea, ref int num)
        {
            try
            {
                num = excute.ExecuteScalar("select count(1) from TSoq where vcYearMonth='" + strYearMonth + "' and  vcHyState in ('1','2') " +
                   "and vcSupplier_id='" + strSupplier_id + "'");//判断此供应商的数据是否TFTM已回复或FTMS已合意

                StringBuilder strSql = new StringBuilder();
                strSql.Append("select t1.iAutoId,t1.vcYearMonth,t1.dExpectTime,t1.vcHyState,t1.vcDyState,t2.vcName as vcDyState_Name,    \n");
                strSql.Append("t1.vcPart_id,t1.iQuantityPercontainer,t1.iCbSOQN,t1.iCbSOQN1,t1.iCbSOQN2,    \n");
                strSql.Append("t3.iTzhSOQN,t3.iTzhSOQN1,t3.iTzhSOQN2,t1.dOpenTime,t1.dSReplyTime,    \n");
                strSql.Append("case           \n");
                strSql.Append(" when t3.iTzhSOQN is null or t3.iTzhSOQN1 is null or t3.iTzhSOQN2 is null then 'partFS0501A' --无调整          \n");
                strSql.Append(" when t3.iTzhSOQN=t1.iCbSOQN and t3.iTzhSOQN1=t1.iCbSOQN1 and t3.iTzhSOQN2=t1.iCbSOQN2 then 'partFS0501A' --无调整        \n");
                strSql.Append(" else 'partFS0501B' --有调整          \n");
                strSql.Append(" end as vcBgColor,'0' as vcModFlag,'0' as vcAddFlag,t1.vcSupplier_id,      \n");
                strSql.Append("CASE WHEN vcDyState='1' and vcHyState in ('0','3') and isnull(t4.num,'0')='0' then '0' else '1' end as bSelectFlag    \n");
                strSql.Append("from(    \n");
                strSql.Append("	select * from TSoq     \n");
                strSql.Append("	where vcYearMonth='" + strYearMonth + "'     \n");
                strSql.Append("	and vcSupplier_id='" + strSupplier_id + "'    \n");
                if (strOperState == "0")//未提交
                    //strSql.Append("	and vcDyState='1' and vcHyState in ('0','3')    \n");
                    strSql.Append("   and vcDyState='1' and dSReplyTime is null \n");
                else if (strOperState == "1")//已提交
                    //strSql.Append("	and (vcDyState!='1' or vcHyState not in ('0','3'))    \n");
                    strSql.Append("  and dSReplyTime is not null  \n");
                if (!string.IsNullOrEmpty(strDyState))//对应状态
                    strSql.Append(" and vcDyState='" + strDyState + "' ");
                if (!string.IsNullOrEmpty(strPart_id))//品番
                    strSql.Append(" and vcPart_id like '%" + strPart_id + "%'");
                if (!string.IsNullOrEmpty(strWorkArea))//供应商工区
                    strSql.Append("and vcSupplierPlant ='" + strWorkArea + "' ");
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
                strSql.Append("left join (    \n");
                strSql.Append("	select vcSupplier_id,count(1) as num from TSoq where vcYearMonth='"+ strYearMonth + "' and  vcHyState in ('1','2')    \n");
                strSql.Append("	and vcSupplier_id='"+ strSupplier_id + "'    \n");
                strSql.Append("	group by vcSupplier_id    \n");
                strSql.Append(")t4 on t1.vcSupplier_id=t4.vcSupplier_id    \n");
                strSql.Append("order by t1.iAutoId    \n");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable Search_heji(string strYearMonth, string strSupplier_id, string strPart_id, string strDyState, string strOperState, string strWorkArea, ref int num)
        {
            try
            {
                
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select '' as vcYearMonth,'' as dExpectTime,'' as vcDyState_Name,    \n");
                strSql.Append("'' as vcPart_id,'' as iQuantityPercontainer,sum(t1.iCbSOQN) as iCbSOQN,sum(t1.iCbSOQN1) as iCbSOQN1,sum(t1.iCbSOQN2) as iCbSOQN2,    \n");
                strSql.Append("sum(t3.iTzhSOQN) as iTzhSOQN ,sum(t3.iTzhSOQN1) as iTzhSOQN1,sum(t3.iTzhSOQN2) as iTzhSOQN2,'' as dOpenTime,'' as dSReplyTime    \n");
                strSql.Append("from(    \n");
                strSql.Append("	select * from TSoq     \n");
                strSql.Append("	where vcYearMonth='" + strYearMonth + "'     \n");
                strSql.Append("	and vcSupplier_id='" + strSupplier_id + "'    \n");
                if (strOperState == "0")//未提交
                    strSql.Append("   and vcDyState='1' and dSReplyTime is null \n");
                else if (strOperState == "1")//已提交
                    strSql.Append("  and dSReplyTime is not null  \n");
                if (!string.IsNullOrEmpty(strDyState))//对应状态
                    strSql.Append(" and vcDyState='" + strDyState + "' ");
                if (!string.IsNullOrEmpty(strPart_id))//品番
                    strSql.Append(" and vcPart_id like '%" + strPart_id + "%'");
                if (!string.IsNullOrEmpty(strWorkArea))//供应商工区
                    strSql.Append("and vcSupplierPlant ='" + strWorkArea + "' ");
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
        public DataTable SearchHistory(string strYearMonth, string strUserId)
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
        public int importHistory(string strYearMonth, Dictionary<string,string> errMessageDict, string strUserId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                //先删除本对象月再新增
                strSql.Append(" DELETE FROM TSoqInputErrDetail_Save  WHERE vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth + "' ");
                foreach (KeyValuePair<string, string> kvp in errMessageDict)
                {
                    string strPart_id = kvp.Key;
                    string strMsg = kvp.Value;
                    strSql.Append(" INSERT INTO TSoqInputErrDetail_Save (vcYearMonth,vcPart_id,vcMessage,vcOperator,dOperatorTime) values ( ");
                    strSql.Append("  '" + strYearMonth + "','"+strPart_id+"','" + strMsg + "','" + strUserId + "',getdate())  ");
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
        public DataTable IsDQR(string strYearMonth, string strSupplier_id, string strPart_id, string strDyState, string strOperState, string strWorkArea)
        {
            try
            {
                int num = excute.ExecuteScalar("select count(1) from TSoq where vcYearMonth='" + strYearMonth + "' and  vcHyState in ('1','2') " +
                    "and vcSupplier_id='" + strSupplier_id + "'");//判断此供应商的数据是否TFTM已回复或FTMS已合意
                StringBuilder strSql = new StringBuilder();
                strSql.Append(" select * from TSoq ");
                //strSql.AppendLine(" WHERE (vcDyState!='1' or vcHyState not in ('0','3')) ");//这几个状态(1,0,3)是可操作的状态
                strSql.Append("	where vcYearMonth='" + strYearMonth + "'     \n");
                strSql.Append("	and vcSupplier_id='" + strSupplier_id + "'    \n");
                if (strOperState == "0")//未提交
                    //strSql.Append("	and vcDyState='1' and vcHyState in ('0','3')    \n");
                    strSql.Append("   and vcDyState='1' and dSReplyTime is null \n");
                else if (strOperState == "1")//已提交
                    //strSql.Append("	and (vcDyState!='1' or vcHyState not in ('0','3'))    \n");
                    strSql.Append("  and dSReplyTime is not null  \n");
                if (!string.IsNullOrEmpty(strDyState))//对应状态
                    strSql.Append(" and vcDyState='" + strDyState + "' ");
                if (!string.IsNullOrEmpty(strPart_id))//品番
                    strSql.Append(" and vcPart_id like '%" + strPart_id + "%'");
                if (!string.IsNullOrEmpty(strWorkArea))
                    strSql.Append(" and vcSupplierPlant='" + strWorkArea + "'  \n");

                if (num > 0)
                    strSql.Append(" and 1=1 ");//不能操作的，需要检索出数据
                else
                    strSql.Append(" and 1=2 ");

                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 是否可操作-按列表所选数据
        public DataTable IsDQR(string strYearMonth, string strSupplier_id, List<Dictionary<string, Object>> listInfoData, string strType)//strType:save(保存)、submit(提交)
        {
            try
            {
                int num = excute.ExecuteScalar("select count(1) from TSoq where vcYearMonth='" + strYearMonth + "' and  vcHyState in ('1','2') " +
                    "and vcSupplier_id='" + strSupplier_id + "'");//判断此供应商的数据是否TFTM已回复或FTMS已合意
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
                            if (num > 0)
                                strSql.Append("       '1',     \n");
                            else
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
                        if (num > 0)
                            strSql.Append("       '1',     \n");
                        else
                            strSql.Append("       '" + listInfoData[i]["vcHyState"].ToString() + "',     \n");
                        strSql.Append("       '" + listInfoData[i]["vcDyState"].ToString() + "'     \n");
                        strSql.Append("      );      \n");
                        #endregion
                    }
                }
                //strSql.Append(" select * from #TSoq_temp_cr where (vcDyState!='1' or vcHyState not in ('0','3'))  ");//这几个状态(1,0,3)是可操作的状态
                strSql.Append(" select * from #TSoq_temp_cr where (vcDyState!='1' or vcHyState in ('1','2'))  ");//不能操作条件
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 提交-按检索条件
        public int ok(string strYearMonth, string strSupplier_id, string strPart_id, string strDyState, string strOperState, string strWorkArea, string strUserId)
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
                if (strOperState == "0")//未提交
                    //strSql.Append("	and vcDyState='1' and vcHyState in ('0','3')    \n");
                    strSql.Append("   and vcDyState='1' and dSReplyTime is null \n");
                else if (strOperState == "1")//已提交
                    //strSql.Append("	and (vcDyState!='1' or vcHyState not in ('0','3'))    \n");
                    strSql.Append("  and dSReplyTime is not null  \n");
                if (!string.IsNullOrEmpty(strDyState))//对应状态
                    strSql.Append(" and vcDyState='" + strDyState + "' ");
                if (!string.IsNullOrEmpty(strPart_id))//品番
                    strSql.Append(" and vcPart_id like '%" + strPart_id + "%'");
                if (!string.IsNullOrEmpty(strWorkArea))//供应商工区
                    strSql.Append(" and vcSupplierPlant='" + strWorkArea + "'  \n");
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
                    strSql.Append(ComFunction.getSqlValue(listInfoData[i]["vcPart_id"], false) + ",   \n");
                    strSql.Append(ComFunction.getSqlValue(listInfoData[i]["vcYearMonth"], false) + ",   \n");
                    strSql.Append(ComFunction.getSqlValue(listInfoData[i]["iTzhSOQN"], false) + ",   \n");
                    strSql.Append(ComFunction.getSqlValue(listInfoData[i]["iTzhSOQN1"], false) + ",   \n");
                    strSql.Append(ComFunction.getSqlValue(listInfoData[i]["iTzhSOQN2"], false) + "   \n");
                    strSql.Append(")      \n");
                    #endregion
                }
                //更新对应状态为有调整/无调整，更新提交人和提单时间
                strSql.Append(" UPDATE TSoq SET ");
                strSql.Append("      vcDyState=case when a.iCbSOQN!=b.iTzhSOQN or a.iCbSOQN1!=b.iTzhSOQN1 or a.iCbSOQN2!=b.iTzhSOQN2 then '2' else '3' end, ");//0：未发送；1：待回复；2：有调整；3：无调整
                strSql.Append("      vcSReplyUser='" + strUserId + "', ");
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

        //public Dictionary<string, string> ErrorMsg(Dictionary<string, string> errMessageDict, DataTable dtc, DataTable dt, string strMesInfo, bool isformat)
        //{
        //    for (int i = 0; i < dtc.Rows.Count; i++)
        //    {
        //        string strpf = dtc.Rows[i]["vcPart_id"].ToString();
        //        DataRow[] dataRows = dt.Select("vcPart_id='" + strpf + "'");
        //        if (dataRows.Length != 0)
        //        {
        //            string strYm = "";
        //            for (int j = 0; j < dataRows.Length; j++)
        //            {
        //                strYm = strYm + dataRows[j]["vcYM"].ToString();
        //                if (j != dataRows.Length - 1)
        //                    strYm = strYm + ",";
        //            }
        //            if (isformat)
        //                errMessageDict.Add(strpf, string.Format(strMesInfo, strYm));
        //            else
        //                errMessageDict.Add(strpf, strMesInfo);
        //        }
        //    }
        //    return errMessageDict;
        //}

        public void ErrorMsg(ref DataTable dterrMessage, DataTable dtc, DataTable dt, string strMesInfo, bool isformat)
        {
            for (int i = 0; i < dtc.Rows.Count; i++)
            {
                string strpf = dtc.Rows[i]["vcPart_id"].ToString();
                DataRow[] dataRows = dt.Select("vcPart_id='" + strpf + "'");
                if (dataRows.Length != 0)
                {
                    string strYm = "";
                    for (int j = 0; j < dataRows.Length; j++)
                    {
                        strYm = strYm + dataRows[j]["vcYM"].ToString();
                        if (j != dataRows.Length - 1)
                            strYm = strYm + ",";
                    }
                    if (isformat)
                    {
                        DataRow dr = dterrMessage.NewRow();
                        dr["vcPart_id"] = strpf;
                        dr["vcMsg"] = string.Format(strMesInfo, strYm);
                        dterrMessage.Rows.Add(dr);

                    }
                    else
                    {
                        DataRow dr = dterrMessage.NewRow();
                        dr["vcPart_id"] = strpf;
                        dr["vcMsg"] = strMesInfo;
                        dterrMessage.Rows.Add(dr);

                    }
                }
            }
        }

        #region 保存后校验
        #region 保存后校验
        public void SaveCheck(DataTable dtMultiple, string strUserId, string strYearMonth, string strYearMonth_2, string strYearMonth_3,
            ref DataTable dterrMessage, string strUnit, string strReceiver)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(" delete TSoq_temp where vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth + "' ;  \r\n ");

                #region 先插入临时表
                for (int i = 0; i < dtMultiple.Rows.Count; i++)
                {
                    sql.Append("  INSERT INTO TSoq_temp(vcYearMonth,vcPart_id,vcSupplier_id,iTzhSOQN,iTzhSOQN1,iTzhSOQN2,vcOperator,dOperatorTime,  ");
                    sql.Append("  iCbSOQN,iCbSOQN1,iCbSOQN2) values       \n");
                    sql.Append("('" + dtMultiple.Rows[i]["vcYearMonth"].ToString() + "',");
                    sql.Append("'" + dtMultiple.Rows[i]["vcPart_id"].ToString() + "',");
                    sql.Append("'" + dtMultiple.Rows[i]["vcSupplierId"].ToString() + "',");
                    sql.Append("'" + dtMultiple.Rows[i]["iTzhSOQN"].ToString() + "',");
                    sql.Append("'" + dtMultiple.Rows[i]["iTzhSOQN1"].ToString() + "',");
                    sql.Append("'" + dtMultiple.Rows[i]["iTzhSOQN2"].ToString() + "',");
                    sql.Append("'" + strUserId + "',");
                    sql.Append("getDate(),");
                    sql.Append("'" + dtMultiple.Rows[i]["iCbSOQN"].ToString() + "',");
                    sql.Append("'" + dtMultiple.Rows[i]["iCbSOQN1"].ToString() + "',");
                    sql.Append("'" + dtMultiple.Rows[i]["iCbSOQN2"].ToString() + "'");
                    sql.Append(")");
                }
                excute.ExcuteSqlWithStringOper(sql.ToString());//先导入临时表，然后check
                #endregion

                #region 品番
                sql.Length = 0;//清空
                sql.Append("      select * from TSoq_temp where vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth + "'    \r\n ");
                DataTable dtc = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                #endregion

                #region 验证1：是否为TFTM品番（包装工厂）
                sql.Length = 0;//清空
                sql.Append("   select a.vcPart_id from    \r\n ");
                sql.Append("   (    \r\n ");
                sql.Append("      select * from TSoq_temp where vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth + "'    \r\n ");
                sql.Append("   )a    \r\n ");
                sql.Append("   left join    \r\n ");
                sql.Append("   (    \r\n ");
                sql.Append("      select * from TSPMaster where vcPackingPlant='" + strUnit + "' and vcReceiver='" + strReceiver + "'     \r\n ");
                sql.Append("   )b on a.vcPart_id=b.vcPartId and a.vcSupplier_id=b.vcSupplierId    \r\n ");
                sql.Append("   where b.vcPartId is  null    \r\n ");
                DataTable dt1 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    DataRow dr = dterrMessage.NewRow();
                    dr["vcPart_id"] = dt1.Rows[i]["vcPart_id"].ToString();
                    dr["vcMsg"] = "此品番不存在";
                    dterrMessage.Rows.Add(dr);
                }
                #endregion

                #region 验证2：N 月品番有效性(N月得有数量),有数量才校验  
                sql.Length = 0;//清空
                sql.AppendLine("select * from ");
                sql.AppendLine("(");
                sql.AppendLine("	select a.vcYM,a.item,b.vcPart_id,b.vcSupplier_id");
                sql.AppendLine("	,case when item=1 then iCbSOQN");
                sql.AppendLine("	  when item=2 then iCbSOQN1");
                sql.AppendLine("	  when item=3 then iCbSOQN2");
                sql.AppendLine("	  else 0  end as iCbSOQN");
                sql.AppendLine("	,case when item=1 then iTzhSOQN");
                sql.AppendLine("		  when item=2 then iTzhSOQN1");
                sql.AppendLine("		  when item=3 then iTzhSOQN2");
                sql.AppendLine("		  else 0  end as iTzhSOQN");
                sql.AppendLine("	from");
                sql.AppendLine("	(select '" + strYearMonth + "' as vcYM ,1 as item union all select '" + strYearMonth_2 + "' as vcYM,2 as item union all select '" + strYearMonth_3 + "' as vcYM,3 as item)a");
                sql.AppendLine("	left join");
                sql.AppendLine("	(select * from TSoq_temp where vcOperator='" + strUserId + "')b on 1=1");
                sql.AppendLine(")t1");
                sql.AppendLine("left join");
                sql.AppendLine("(");
                sql.AppendLine("	select vcPartId,vcSupplierId,dFromTime,dToTime from TSPMaster where vcPackingPlant='" + strUnit + "' and vcReceiver='" + strReceiver + "' and dFromTime<>dToTime  ");
                sql.AppendLine(")t2");
                sql.AppendLine("on t1.vcPart_id=t2.vcPartId and t1.vcSupplier_id=t2.vcSupplierId ");
                sql.AppendLine("and t1.vcYM between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)");
                sql.AppendLine("where cast(t1.iTzhSOQN as int) <>0 and t2.vcPartId is null");
                DataTable dt2 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                ErrorMsg(ref dterrMessage, dtc, dt2, "不满足{0}月有效性", true);
                #endregion

                #region 验证5：是否有价格，且在有效期内(只判断N月)，数量为0不校验；如果是强制订货，则没有价格也可以
                sql.Length = 0;
                sql.AppendLine("select * from ");
                sql.AppendLine("(");
                sql.AppendLine("	select a.vcYM,a.item,b.vcPart_id,b.vcSupplier_id");
                sql.AppendLine("	,case when item=1 then iCbSOQN");
                sql.AppendLine("	  when item=2 then iCbSOQN1");
                sql.AppendLine("	  when item=3 then iCbSOQN2");
                sql.AppendLine("	  else 0  end as iCbSOQN");
                sql.AppendLine("	,case when item=1 then iTzhSOQN");
                sql.AppendLine("		  when item=2 then iTzhSOQN1");
                sql.AppendLine("		  when item=3 then iTzhSOQN2");
                sql.AppendLine("		  else 0  end as iTzhSOQN");
                sql.AppendLine("	from");
                sql.AppendLine("	(select '" + strYearMonth + "' as vcYM ,1 as item union all select '" + strYearMonth_2 + "' as vcYM,2 as item union all select '" + strYearMonth_3 + "' as vcYM,3 as item)a");
                sql.AppendLine("	left join");
                sql.AppendLine("	(select * from TSoq_temp where vcOperator='" + strUserId + "')b on 1=1");
                sql.AppendLine(")t1");
                sql.AppendLine("left join");
                sql.AppendLine("(--手配主表");
                sql.AppendLine("	select vcPartId,vcSupplierId,vcMandOrder,dFromTime,dToTime ");
                sql.AppendLine("	from TSPMaster where vcPackingPlant='" + strUnit + "' and vcReceiver='" + strReceiver + "' and dFromTime<>dToTime ");
                sql.AppendLine(")t2 on t1.vcPart_id=t2.vcPartId and t1.vcSupplier_id=t2.vcSupplierId and t1.vcYM between convert(varchar(6),t2.dFromTime,112) and convert(varchar(6),t2.dToTime,112)");
                sql.AppendLine("left join     ");
                sql.AppendLine("(--价格     ");
                sql.AppendLine("   select vcPart_id,dPricebegin,dPriceEnd,vcSupplier_id from TPrice     ");
                sql.AppendLine(")t3 on t1.vcPart_id=t3.vcPart_id and t1.vcSupplier_id=t3.vcSupplier_id and t1.vcYM between convert(varchar(6),t3.dPricebegin,112) and convert(varchar(6),t3.dPriceEnd,112)   ");
                sql.AppendLine("where item=1 and cast(t1.iTzhSOQN as int) <>0 and t3.vcPart_id is null and isnull(t2.vcMandOrder,'')<>'1' --vcMandOrder='1' 是强制订货");
                DataTable dt5 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                ErrorMsg(ref dterrMessage, dtc, dt5, "{0}月没有价格", true);
                #endregion

                #region 验证6：手配中是否有受入、收容数、发注工厂（N、N+1、N+2都判断），数量为0不校验
                sql.Length = 0;//清空
                sql.AppendLine("select * from     ");
                sql.AppendLine("(    ");
                sql.AppendLine("	select a.vcYM,a.item,b.vcPart_id,b.vcSupplier_id    ");
                sql.AppendLine("	,case when item=1 then iCbSOQN    ");
                sql.AppendLine("	  when item=2 then iCbSOQN1    ");
                sql.AppendLine("	  when item=3 then iCbSOQN2    ");
                sql.AppendLine("	  else 0  end as iCbSOQN    ");
                sql.AppendLine("	,case when item=1 then iTzhSOQN    ");
                sql.AppendLine("		  when item=2 then iTzhSOQN1    ");
                sql.AppendLine("		  when item=3 then iTzhSOQN2    ");
                sql.AppendLine("		  else 0  end as iTzhSOQN    ");
                sql.AppendLine("	from    ");
                sql.AppendLine("	(select '" + strYearMonth + "' as vcYM ,1 as item union all select '" + strYearMonth_2 + "' as vcYM,2 as item union all select '" + strYearMonth_3 + "' as vcYM,3 as item)a    ");
                sql.AppendLine("	left join    ");
                sql.AppendLine("	(select * from TSoq_temp where vcOperator='" + strUserId + "')b on 1=1    ");
                sql.AppendLine(")t1    ");
                sql.AppendLine("left join    ");
                sql.AppendLine("(--手配主表    ");
                sql.AppendLine("	select vcPartId,vcCarfamilyCode,vcHaoJiu,vcReceiver,vcPackingPlant,vcSupplierId,vcInOut,dFromTime,dToTime     ");
                sql.AppendLine("	from TSPMaster where vcPackingPlant='" + strUnit + "' and vcReceiver='" + strReceiver + "' and dFromTime<>dToTime      ");
                sql.AppendLine(")t2 on t1.vcPart_id=t2.vcPartId and t1.vcSupplier_id=t2.vcSupplierId and t1.vcYM between convert(varchar(6),t2.dFromTime,112) and convert(varchar(6),t2.dToTime,112)    ");
                sql.AppendLine("left join (    --//供应商工区 N    ");
                sql.AppendLine("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant,dFromTime,dToTime         ");
                sql.AppendLine("	from TSPMaster_SupplierPlant         ");
                sql.AppendLine("	where vcOperatorType='1'         ");
                sql.AppendLine(")t4 on t2.vcPartId=t4.vcPartId and t2.vcPackingPlant=t4.vcPackingPlant and t2.vcReceiver=t4.vcReceiver and t2.vcSupplierId=t4.vcSupplierId       ");
                sql.AppendLine("and t1.vcYM between convert(varchar(6),t4.dFromTime,112) and convert(varchar(6),t4.dToTime,112)    ");
                sql.AppendLine("left join (    --//发注工厂 N    ");
                sql.AppendLine("	select vcValue1 as [供应商编号],vcValue2 as [工区],vcValue3 as [开始时间],        ");
                sql.AppendLine("	vcValue4 as [结束时间],vcValue5 as [发注工厂] from TOutCode         ");
                sql.AppendLine("	where vcCodeId='C010' and vcIsColum='0'         ");
                sql.AppendLine(")fzgc on t2.vcSupplierId=fzgc.供应商编号 and t4.vcSupplierPlant=fzgc.工区      ");
                sql.AppendLine("and t1.vcYM between convert(varchar(6),fzgc.[开始时间],112) and convert(varchar(6),fzgc.[结束时间],112)    ");
                sql.AppendLine("left join(    --//收容数 N    ");
                sql.AppendLine("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant,iPackingQty,dFromTime,dToTime         ");
                sql.AppendLine("	from TSPMaster_Box         ");
                sql.AppendLine("	where vcOperatorType='1'         ");
                sql.AppendLine(")t5 on t2.vcPartId=t5.vcPartId and t2.vcPackingPlant=t5.vcPackingPlant and t2.vcReceiver=t5.vcReceiver and t2.vcSupplierId=t5.vcSupplierId         ");
                sql.AppendLine("and t1.vcYM between convert(varchar(6),t5.dFromTime,112) and convert(varchar(6),t5.dToTime,112)    ");
                sql.AppendLine("left join         ");
                sql.AppendLine("(    --//受入 N    ");
                sql.AppendLine("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSufferIn,dFromTime,dToTime    ");
                sql.AppendLine("	from TSPMaster_SufferIn        ");
                sql.AppendLine("	where vcOperatorType='1'        ");
                sql.AppendLine(")t6 on t2.vcPartId=t6.vcPartId and t2.vcPackingPlant=t6.vcPackingPlant and t2.vcReceiver=t6.vcReceiver and t2.vcSupplierId=t6.vcSupplierId      ");
                sql.AppendLine("and t1.vcYM between convert(varchar(6),t6.dFromTime,112) and convert(varchar(6),t6.dToTime,112)    ");
                sql.AppendLine("where cast(t1.iTzhSOQN as int) <>0 and (fzgc.供应商编号 is null or t5.vcPartId is null or t6.vcPartId is null)    ");
                DataTable dt6 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                ErrorMsg(ref dterrMessage, dtc, dt6, "{0}月无手配信息", true);
                #endregion

                #region 验证8：品番3个月数量不能全为0
                sql.Length = 0;//清空
                sql.Append("select vcPart_id,iTzhSOQN,iTzhSOQN1,iTzhSOQN2 from TSoq_temp where vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth + "'    \n");
                sql.Append("and ISNULL(iTzhSOQN,0)=0 and ISNULL(iTzhSOQN1,0)=0 and ISNULL(iTzhSOQN2,0)=0     \n");
                DataTable dt8 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                for (int i = 0; i < dt8.Rows.Count; i++)
                {
                    DataRow dr = dterrMessage.NewRow();
                    dr["vcPart_id"] = dt8.Rows[i]["vcPart_id"].ToString();
                    dr["vcMsg"] = "3个月数量全为0";
                    dterrMessage.Rows.Add(dr);
                }
                #endregion

                #region 验证8-1：调整3个月数量加总必须等于初版3个月数量加总
                sql.Length = 0;//清空
                sql.Append("select vcPart_id,iTzhSOQN,iTzhSOQN1,iTzhSOQN2 from TSoq_temp where vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth + "'    \n");
                sql.Append("and ISNULL(iTzhSOQN,0)+ISNULL(iTzhSOQN1,0)+ISNULL(iTzhSOQN2,0)!=ISNULL(iCbSOQN,0)+ISNULL(iCbSOQN1,0)+ISNULL(iCbSOQN2,0)     \n");
                DataTable dt8_1 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                for (int i = 0; i < dt8_1.Rows.Count; i++)
                {
                    DataRow dr = dterrMessage.NewRow();
                    dr["vcPart_id"] = dt8_1.Rows[i]["vcPart_id"].ToString();
                    dr["vcMsg"] = "3个月调整数量总和与初版不一致";
                    dterrMessage.Rows.Add(dr);
                }
                #endregion

                #region 验证9：收容数整倍数
                sql.Length = 0;
                sql.AppendLine("select * from     ");
                sql.AppendLine("(    ");
                sql.AppendLine("	select a.vcYM,a.item,b.vcPart_id,b.vcSupplier_id    ");
                sql.AppendLine("	,case when item=1 then iCbSOQN    ");
                sql.AppendLine("	  when item=2 then iCbSOQN1    ");
                sql.AppendLine("	  when item=3 then iCbSOQN2    ");
                sql.AppendLine("	  else 0  end as iCbSOQN    ");
                sql.AppendLine("	,case when item=1 then iTzhSOQN    ");
                sql.AppendLine("		  when item=2 then iTzhSOQN1    ");
                sql.AppendLine("		  when item=3 then iTzhSOQN2    ");
                sql.AppendLine("		  else 0  end as iTzhSOQN    ");
                sql.AppendLine("	from    ");
                sql.AppendLine("	(select '" + strYearMonth + "' as vcYM ,1 as item union all select '" + strYearMonth_2 + "' as vcYM,2 as item union all select '" + strYearMonth_3 + "' as vcYM,3 as item)a    ");
                sql.AppendLine("	left join    ");
                sql.AppendLine("	(select * from TSoq_temp where vcOperator='" + strUserId + "')b on 1=1    ");
                sql.AppendLine(")t1    ");
                sql.AppendLine("left join    ");
                sql.AppendLine("(--手配主表    ");
                sql.AppendLine("	select vcPartId,vcCarfamilyCode,vcHaoJiu,vcReceiver,vcPackingPlant,vcSupplierId,vcInOut,dFromTime,dToTime     ");
                sql.AppendLine("	from TSPMaster where vcPackingPlant='" + strUnit + "' and vcReceiver='strReceiver' and dFromTime<>dToTime      ");
                sql.AppendLine(")t2 on t1.vcPart_id=t2.vcPartId and t1.vcSupplier_id=t2.vcSupplierId and t1.vcYM between convert(varchar(6),t2.dFromTime,112) and convert(varchar(6),t2.dToTime,112)    ");
                sql.AppendLine("left join(    --//收容数 N    ");
                sql.AppendLine("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant,iPackingQty,dFromTime,dToTime         ");
                sql.AppendLine("	from TSPMaster_Box         ");
                sql.AppendLine("	where vcOperatorType='1'         ");
                sql.AppendLine(")t5 on t2.vcPartId=t5.vcPartId and t2.vcPackingPlant=t5.vcPackingPlant and t2.vcReceiver=t5.vcReceiver and t2.vcSupplierId=t5.vcSupplierId         ");
                sql.AppendLine("and t1.vcYM between convert(varchar(6),t5.dFromTime,112) and convert(varchar(6),t5.dToTime,112)    ");
                sql.AppendLine("where cast(t1.iTzhSOQN as int) <>0 and t1.iTzhSOQN%t5.iPackingQty<>0    ");
                DataTable dt9 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                ErrorMsg(ref dterrMessage, dtc, dt9, "订货数量不是收容数的整数倍", false);
                #endregion

                #region 验证10：if一括生产 校验： 对象月 >= 实施年月时间 不能订货(数量得是0) 3个月都校验
                sql.Length = 0;
                sql.AppendLine("select * from ");
                sql.AppendLine("(");
                sql.AppendLine("	select a.vcYM,a.item,b.vcPart_id,b.vcSupplier_id");
                sql.AppendLine("	,case when item=1 then iCbSOQN");
                sql.AppendLine("	  when item=2 then iCbSOQN1");
                sql.AppendLine("	  when item=3 then iCbSOQN2");
                sql.AppendLine("	  else 0  end as iCbSOQN");
                sql.AppendLine("	,case when item=1 then iTzhSOQN");
                sql.AppendLine("		  when item=2 then iTzhSOQN1");
                sql.AppendLine("		  when item=3 then iTzhSOQN2");
                sql.AppendLine("		  else 0  end as iTzhSOQN");
                sql.AppendLine("	from");
                sql.AppendLine("	(select '" + strYearMonth + "' as vcYM ,1 as item union all select '" + strYearMonth_2 + "' as vcYM,2 as item union all select '" + strYearMonth_3 + "' as vcYM,3 as item)a");
                sql.AppendLine("	left join");
                sql.AppendLine("	(select * from TSoq_temp where vcOperator='" + strUserId + "')b on 1=1");
                sql.AppendLine(")t1");
                sql.AppendLine("left join");
                sql.AppendLine("(--手配主表");
                sql.AppendLine("	select vcPartId,vcCarfamilyCode,vcHaoJiu,vcReceiver,vcPackingPlant,vcSupplierId,vcInOut,dDebugTime,dFromTime,dToTime ");
                sql.AppendLine("	from TSPMaster where vcPackingPlant='" + strUnit + "' and vcReceiver='strReceiver' and dFromTime<>dToTime  and vcOldProduction='一括生产'");
                sql.AppendLine(")t2 on t1.vcPart_id=t2.vcPartId and t1.vcSupplier_id=t2.vcSupplierId and t1.vcYM between convert(varchar(6),t2.dFromTime,112) and convert(varchar(6),t2.dToTime,112)");
                sql.AppendLine("and t1.vcYM>=convert(varchar(6),t2.dDebugTime,112)");
                sql.AppendLine("where cast(t1.iTzhSOQN as int) <>0 and t2.vcPartId is not null");
                DataTable dt10 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                ErrorMsg(ref dterrMessage, dtc, dt10, "在{0}月以后不能订货", true);
                #endregion

                #region 验证11：特殊订货不能导入  
                sql.Length = 0;
                sql.AppendLine("select * from ");
                sql.AppendLine("(");
                sql.AppendLine("	select a.vcYM,a.item,b.vcPart_id,b.vcSupplier_id");
                sql.AppendLine("	,case when item=1 then iCbSOQN");
                sql.AppendLine("	  when item=2 then iCbSOQN1");
                sql.AppendLine("	  when item=3 then iCbSOQN2");
                sql.AppendLine("	  else 0  end as iCbSOQN");
                sql.AppendLine("	,case when item=1 then iTzhSOQN");
                sql.AppendLine("		  when item=2 then iTzhSOQN1");
                sql.AppendLine("		  when item=3 then iTzhSOQN2");
                sql.AppendLine("		  else 0  end as iTzhSOQN");
                sql.AppendLine("	from");
                sql.AppendLine("	(select '" + strYearMonth + "' as vcYM ,1 as item union all select '" + strYearMonth_2 + "' as vcYM,2 as item union all select '" + strYearMonth_3 + "' as vcYM,3 as item)a");
                sql.AppendLine("	left join");
                sql.AppendLine("	(select * from TSoq_temp where vcOperator='" + strUserId + "')b on 1=1");
                sql.AppendLine(")t1");
                sql.AppendLine("left join");
                sql.AppendLine("(--手配主表");
                sql.AppendLine("	select vcPartId,vcCarfamilyCode,vcHaoJiu,vcReceiver,vcPackingPlant,vcSupplierId,vcInOut,dDebugTime,dFromTime,dToTime ");
                sql.AppendLine("	from TSPMaster where vcPackingPlant='" + strUnit + "' and vcReceiver='strReceiver' and dFromTime<>dToTime  and vcOrderingMethod='1'");
                sql.AppendLine(")t2 on t1.vcPart_id=t2.vcPartId and t1.vcSupplier_id=t2.vcSupplierId and t1.vcYM between convert(varchar(6),t2.dFromTime,112) and convert(varchar(6),t2.dToTime,112)");
                sql.AppendLine("where cast(t1.iTzhSOQN as int) <>0 and t2.vcPartId is not null");
                DataTable dt11 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
                ErrorMsg(ref dterrMessage, dtc, dt11, "{0}月特殊品番不能订货", true);
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        //public void SaveCheck(List<Dictionary<string, Object>> listInfoData, string strUserId, string strYearMonth, string strYearMonth_2, string strYearMonth_3,
        //    ref DataTable dterrMessage, string strUnit)
        //{
        //    try
        //    {
        //        StringBuilder sql = new StringBuilder();
        //        sql.Append(" delete TSoq_temp where vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth + "' ;  \r\n ");

        //        #region 先插入临时表
        //        for (int i = 0; i < listInfoData.Count; i++)
        //        {
        //            bool bmodflag = (bool)listInfoData[i]["vcModFlag"];//true可编辑,false不可编辑
        //            bool baddflag = (bool)listInfoData[i]["vcAddFlag"];//true可编辑,false不可编辑
        //            if (baddflag == true)
        //            {//新增
        //                //没有新增情况
        //            }
        //            if (baddflag == false && bmodflag == true)
        //            {//修改
        //                sql.Append("  INSERT INTO TSoq_temp(vcYearMonth,vcPart_id,vcSupplier_id,iTzhSOQN,iTzhSOQN1,iTzhSOQN2,vcOperator,dOperatorTime,  ");
        //                sql.Append("  iCbSOQN,iCbSOQN1,iCbSOQN2) values       \n");
        //                sql.Append("('" + strYearMonth + "',");
        //                sql.Append("'" + listInfoData[i]["vcPart_id"].ToString() + "',");
        //                sql.Append("'" + listInfoData[i]["vcSupplier_id"].ToString() + "',");
        //                sql.Append("'" + listInfoData[i]["iTzhSOQN"].ToString() + "',");
        //                sql.Append("'" + listInfoData[i]["iTzhSOQN1"].ToString() + "',");
        //                sql.Append("'" + listInfoData[i]["iTzhSOQN2"].ToString() + "',");
        //                sql.Append("'" + strUserId + "',");
        //                sql.Append("getDate(),");
        //                sql.Append("'" + listInfoData[i]["iCbSOQN"].ToString() + "',");
        //                sql.Append("'" + listInfoData[i]["iCbSOQN1"].ToString() + "',");
        //                sql.Append("'" + listInfoData[i]["iCbSOQN2"].ToString() + "'");
        //                sql.Append(")");
        //            }
        //        }
        //        excute.ExcuteSqlWithStringOper(sql.ToString());//先导入临时表，然后check
        //        #endregion

        //        #region 品番
        //        sql.Length = 0;//清空
        //        sql.Append("      select * from TSoq_temp where vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth + "'    \r\n ");
        //        DataTable dtc = excute.ExcuteSqlWithSelectToDT(sql.ToString());
        //        #endregion

        //        #region 验证1：是否为TFTM品番（包装工厂）
        //        sql.Length = 0;//清空
        //        sql.Append("   select a.vcPart_id from    \r\n ");
        //        sql.Append("   (    \r\n ");
        //        sql.Append("      select * from TSoq_temp where vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth + "'    \r\n ");
        //        sql.Append("   )a    \r\n ");
        //        sql.Append("   left join    \r\n ");
        //        sql.Append("   (    \r\n ");
        //        sql.Append("      select * from TSPMaster where vcPackingPlant='" + strUnit + "' and vcReceiver='APC06'     \r\n ");
        //        sql.Append("   )b on a.vcPart_id=b.vcPartId and a.vcSupplier_id=b.vcSupplierId    \r\n ");
        //        sql.Append("   where b.vcPartId is  null    \r\n ");
        //        DataTable dt1 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
        //        for (int i = 0; i < dt1.Rows.Count; i++)
        //        {
        //            DataRow dr = dterrMessage.NewRow();
        //            dr["vcPart_id"] = dt1.Rows[i]["vcPart_id"].ToString();
        //            dr["vcMsg"] = "此品番不存在";
        //            dterrMessage.Rows.Add(dr);
        //        }
        //        #endregion

        //        #region 验证2：N 月品番有效性(N月得有数量),有数量才校验  
        //        sql.Length = 0;//清空
        //        sql.AppendLine("select * from ");
        //        sql.AppendLine("(");
        //        sql.AppendLine("	select a.vcYM,a.item,b.vcPart_id,b.vcSupplier_id");
        //        sql.AppendLine("	,case when item=1 then iCbSOQN");
        //        sql.AppendLine("	  when item=2 then iCbSOQN1");
        //        sql.AppendLine("	  when item=3 then iCbSOQN2");
        //        sql.AppendLine("	  else 0  end as iCbSOQN");
        //        sql.AppendLine("	,case when item=1 then iTzhSOQN");
        //        sql.AppendLine("		  when item=2 then iTzhSOQN1");
        //        sql.AppendLine("		  when item=3 then iTzhSOQN2");
        //        sql.AppendLine("		  else 0  end as iTzhSOQN");
        //        sql.AppendLine("	from");
        //        sql.AppendLine("	(select '" + strYearMonth + "' as vcYM ,1 as item union all select '" + strYearMonth_2 + "' as vcYM,2 as item union all select '" + strYearMonth_3 + "' as vcYM,3 as item)a");
        //        sql.AppendLine("	left join");
        //        sql.AppendLine("	(select * from TSoq_temp where vcOperator='" + strUserId + "')b on 1=1");
        //        sql.AppendLine(")t1");
        //        sql.AppendLine("left join");
        //        sql.AppendLine("(");
        //        sql.AppendLine("	select vcPartId,vcSupplierId,dFromTime,dToTime from TSPMaster where vcPackingPlant='" + strUnit + "' and vcReceiver='APC06' and dFromTime<>dToTime  ");
        //        sql.AppendLine(")t2");
        //        sql.AppendLine("on t1.vcPart_id=t2.vcPartId and t1.vcSupplier_id=t2.vcSupplierId ");
        //        sql.AppendLine("and t1.vcYM between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)");
        //        sql.AppendLine("where cast(t1.iTzhSOQN as int) <>0 and t2.vcPartId is null");
        //        DataTable dt2 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
        //        ErrorMsg(ref dterrMessage, dtc, dt2, "不满足{0}月有效性", true);
        //        #endregion

        //        #region 验证5：是否有价格，且在有效期内(只判断N月)，数量为0不校验；如果是强制订货，则没有价格也可以
        //        sql.Length = 0;
        //        sql.AppendLine("select * from ");
        //        sql.AppendLine("(");
        //        sql.AppendLine("	select a.vcYM,a.item,b.vcPart_id,b.vcSupplier_id");
        //        sql.AppendLine("	,case when item=1 then iCbSOQN");
        //        sql.AppendLine("	  when item=2 then iCbSOQN1");
        //        sql.AppendLine("	  when item=3 then iCbSOQN2");
        //        sql.AppendLine("	  else 0  end as iCbSOQN");
        //        sql.AppendLine("	,case when item=1 then iTzhSOQN");
        //        sql.AppendLine("		  when item=2 then iTzhSOQN1");
        //        sql.AppendLine("		  when item=3 then iTzhSOQN2");
        //        sql.AppendLine("		  else 0  end as iTzhSOQN");
        //        sql.AppendLine("	from");
        //        sql.AppendLine("	(select '" + strYearMonth + "' as vcYM ,1 as item union all select '" + strYearMonth_2 + "' as vcYM,2 as item union all select '" + strYearMonth_3 + "' as vcYM,3 as item)a");
        //        sql.AppendLine("	left join");
        //        sql.AppendLine("	(select * from TSoq_temp where vcOperator='" + strUserId + "')b on 1=1");
        //        sql.AppendLine(")t1");
        //        sql.AppendLine("left join");
        //        sql.AppendLine("(--手配主表");
        //        sql.AppendLine("	select vcPartId,vcSupplierId,vcMandOrder,dFromTime,dToTime ");
        //        sql.AppendLine("	from TSPMaster where vcPackingPlant='" + strUnit + "' and vcReceiver='APC06' and dFromTime<>dToTime ");
        //        sql.AppendLine(")t2 on t1.vcPart_id=t2.vcPartId and t1.vcSupplier_id=t2.vcSupplierId and t1.vcYM between convert(varchar(6),t2.dFromTime,112) and convert(varchar(6),t2.dToTime,112)");
        //        sql.AppendLine("left join     ");
        //        sql.AppendLine("(--价格     ");
        //        sql.AppendLine("   select vcPart_id,dPricebegin,dPriceEnd from TPrice     ");
        //        sql.AppendLine(")t3 on t1.vcPart_id=t3.vcPart_id and t1.vcSupplier_id=t3.vcSupplierId and t1.vcYM between convert(varchar(6),t3.dPricebegin,112) and convert(varchar(6),t3.dPriceEnd,112)   ");
        //        sql.AppendLine("where item=1 and cast(t1.iTzhSOQN as int) <>0 and t3.vcPart_id is null and isnull(t2.vcMandOrder,'')<>'1' --vcMandOrder='1' 是强制订货");
        //        DataTable dt5 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
        //        ErrorMsg(ref dterrMessage, dtc, dt5, "{0}月没有价格", true);
        //        #endregion

        //        #region 验证6：手配中是否有受入、收容数、发注工厂（N、N+1、N+2都判断），数量为0不校验
        //        sql.Length = 0;//清空
        //        sql.AppendLine("select * from     ");
        //        sql.AppendLine("(    ");
        //        sql.AppendLine("	select a.vcYM,a.item,b.vcPart_id,b.vcSupplier_id    ");
        //        sql.AppendLine("	,case when item=1 then iCbSOQN    ");
        //        sql.AppendLine("	  when item=2 then iCbSOQN1    ");
        //        sql.AppendLine("	  when item=3 then iCbSOQN2    ");
        //        sql.AppendLine("	  else 0  end as iCbSOQN    ");
        //        sql.AppendLine("	,case when item=1 then iTzhSOQN    ");
        //        sql.AppendLine("		  when item=2 then iTzhSOQN1    ");
        //        sql.AppendLine("		  when item=3 then iTzhSOQN2    ");
        //        sql.AppendLine("		  else 0  end as iTzhSOQN    ");
        //        sql.AppendLine("	from    ");
        //        sql.AppendLine("	(select '" + strYearMonth + "' as vcYM ,1 as item union all select '" + strYearMonth_2 + "' as vcYM,2 as item union all select '" + strYearMonth_3 + "' as vcYM,3 as item)a    ");
        //        sql.AppendLine("	left join    ");
        //        sql.AppendLine("	(select * from TSoq_temp where vcOperator='" + strUserId + "')b on 1=1    ");
        //        sql.AppendLine(")t1    ");
        //        sql.AppendLine("left join    ");
        //        sql.AppendLine("(--手配主表    ");
        //        sql.AppendLine("	select vcPartId,vcCarfamilyCode,vcHaoJiu,vcReceiver,vcPackingPlant,vcSupplierId,vcInOut,dFromTime,dToTime     ");
        //        sql.AppendLine("	from TSPMaster where vcPackingPlant='" + strUnit + "' and vcReceiver='APC06' and dFromTime<>dToTime      ");
        //        sql.AppendLine(")t2 on t1.vcPart_id=t2.vcPartId and t1.vcSupplier_id=t2.vcSupplierId and t1.vcYM between convert(varchar(6),t2.dFromTime,112) and convert(varchar(6),t2.dToTime,112)    ");
        //        sql.AppendLine("left join (    --//供应商工区 N    ");
        //        sql.AppendLine("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant,dFromTime,dToTime         ");
        //        sql.AppendLine("	from TSPMaster_SupplierPlant         ");
        //        sql.AppendLine("	where vcOperatorType='1'         ");
        //        sql.AppendLine(")t4 on t2.vcPartId=t4.vcPartId and t2.vcPackingPlant=t4.vcPackingPlant and t2.vcReceiver=t4.vcReceiver and t2.vcSupplierId=t4.vcSupplierId       ");
        //        sql.AppendLine("and t1.vcYM between convert(varchar(6),t4.dFromTime,112) and convert(varchar(6),t4.dToTime,112)    ");
        //        sql.AppendLine("left join (    --//发注工厂 N    ");
        //        sql.AppendLine("	select vcValue1 as [供应商编号],vcValue2 as [工区],vcValue3 as [开始时间],        ");
        //        sql.AppendLine("	vcValue4 as [结束时间],vcValue5 as [发注工厂] from TOutCode         ");
        //        sql.AppendLine("	where vcCodeId='C010' and vcIsColum='0'         ");
        //        sql.AppendLine(")fzgc on t2.vcSupplierId=fzgc.供应商编号 and t4.vcSupplierPlant=fzgc.工区      ");
        //        sql.AppendLine("and t1.vcYM between convert(varchar(6),fzgc.[开始时间],112) and convert(varchar(6),fzgc.[结束时间],112)    ");
        //        sql.AppendLine("left join(    --//收容数 N    ");
        //        sql.AppendLine("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant,iPackingQty,dFromTime,dToTime         ");
        //        sql.AppendLine("	from TSPMaster_Box         ");
        //        sql.AppendLine("	where vcOperatorType='1'         ");
        //        sql.AppendLine(")t5 on t2.vcPartId=t5.vcPartId and t2.vcPackingPlant=t5.vcPackingPlant and t2.vcReceiver=t5.vcReceiver and t2.vcSupplierId=t5.vcSupplierId         ");
        //        sql.AppendLine("and t1.vcYM between convert(varchar(6),t5.dFromTime,112) and convert(varchar(6),t5.dToTime,112)    ");
        //        sql.AppendLine("left join         ");
        //        sql.AppendLine("(    --//受入 N    ");
        //        sql.AppendLine("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSufferIn,dFromTime,dToTime    ");
        //        sql.AppendLine("	from TSPMaster_SufferIn        ");
        //        sql.AppendLine("	where vcOperatorType='1'        ");
        //        sql.AppendLine(")t6 on t2.vcPartId=t6.vcPartId and t2.vcPackingPlant=t6.vcPackingPlant and t2.vcReceiver=t6.vcReceiver and t2.vcSupplierId=t6.vcSupplierId      ");
        //        sql.AppendLine("and t1.vcYM between convert(varchar(6),t6.dFromTime,112) and convert(varchar(6),t6.dToTime,112)    ");
        //        sql.AppendLine("where cast(t1.iTzhSOQN as int) <>0 and (fzgc.供应商编号 is null or t5.vcPartId is null or t6.vcPartId is null)    ");
        //        DataTable dt6 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
        //        ErrorMsg(ref dterrMessage, dtc, dt6, "{0}月无手配信息", true);
        //        #endregion

        //        #region 验证8：品番3个月数量不能全为0
        //        sql.Length = 0;//清空
        //        sql.Append("select vcPart_id,iTzhSOQN,iTzhSOQN1,iTzhSOQN2 from TSoq_temp where vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth + "'    \n");
        //        sql.Append("and ISNULL(iTzhSOQN,0)=0 and ISNULL(iTzhSOQN1,0)=0 and ISNULL(iTzhSOQN2,0)=0     \n");
        //        DataTable dt8 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
        //        for (int i = 0; i < dt8.Rows.Count; i++)
        //        {
        //            DataRow dr = dterrMessage.NewRow();
        //            dr["vcPart_id"] = dt8.Rows[i]["vcPart_id"].ToString();
        //            dr["vcMsg"] = "3个月数量全为0";
        //            dterrMessage.Rows.Add(dr);
        //        }
        //        #endregion

        //        #region 验证8-1：调整3个月数量加总必须等于初版3个月数量加总
        //        sql.Length = 0;//清空
        //        sql.Append("select vcPart_id,iTzhSOQN,iTzhSOQN1,iTzhSOQN2 from TSoq_temp where vcOperator='" + strUserId + "' and vcYearMonth='" + strYearMonth + "'    \n");
        //        sql.Append("and ISNULL(iTzhSOQN,0)+ISNULL(iTzhSOQN1,0)+ISNULL(iTzhSOQN2,0)!=ISNULL(iCbSOQN,0)+ISNULL(iCbSOQN1,0)+ISNULL(iCbSOQN2,0)     \n");
        //        DataTable dt8_1 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
        //        for (int i = 0; i < dt8_1.Rows.Count; i++)
        //        {
        //            DataRow dr = dterrMessage.NewRow();
        //            dr["vcPart_id"] = dt8_1.Rows[i]["vcPart_id"].ToString();
        //            dr["vcMsg"] = "3个月调整数量总和与初版不一致";
        //            dterrMessage.Rows.Add(dr);
        //        }
        //        #endregion

        //        #region 验证9：收容数整倍数
        //        sql.Length = 0;
        //        sql.AppendLine("select * from     ");
        //        sql.AppendLine("(    ");
        //        sql.AppendLine("	select a.vcYM,a.item,b.vcPart_id,b.vcSupplier_id    ");
        //        sql.AppendLine("	,case when item=1 then iCbSOQN    ");
        //        sql.AppendLine("	  when item=2 then iCbSOQN1    ");
        //        sql.AppendLine("	  when item=3 then iCbSOQN2    ");
        //        sql.AppendLine("	  else 0  end as iCbSOQN    ");
        //        sql.AppendLine("	,case when item=1 then iTzhSOQN    ");
        //        sql.AppendLine("		  when item=2 then iTzhSOQN1    ");
        //        sql.AppendLine("		  when item=3 then iTzhSOQN2    ");
        //        sql.AppendLine("		  else 0  end as iTzhSOQN    ");
        //        sql.AppendLine("	from    ");
        //        sql.AppendLine("	(select '" + strYearMonth + "' as vcYM ,1 as item union all select '" + strYearMonth_2 + "' as vcYM,2 as item union all select '" + strYearMonth_3 + "' as vcYM,3 as item)a    ");
        //        sql.AppendLine("	left join    ");
        //        sql.AppendLine("	(select * from TSoq_temp where vcOperator='" + strUserId + "')b on 1=1    ");
        //        sql.AppendLine(")t1    ");
        //        sql.AppendLine("left join    ");
        //        sql.AppendLine("(--手配主表    ");
        //        sql.AppendLine("	select vcPartId,vcCarfamilyCode,vcHaoJiu,vcReceiver,vcPackingPlant,vcSupplierId,vcInOut,dFromTime,dToTime     ");
        //        sql.AppendLine("	from TSPMaster where vcPackingPlant='" + strUnit + "' and vcReceiver='APC06' and dFromTime<>dToTime      ");
        //        sql.AppendLine(")t2 on t1.vcPart_id=t2.vcPartId and t1.vcSupplier_id=t2.vcSupplierId and t1.vcYM between convert(varchar(6),t2.dFromTime,112) and convert(varchar(6),t2.dToTime,112)    ");
        //        sql.AppendLine("left join(    --//收容数 N    ");
        //        sql.AppendLine("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant,iPackingQty,dFromTime,dToTime         ");
        //        sql.AppendLine("	from TSPMaster_Box         ");
        //        sql.AppendLine("	where vcOperatorType='1'         ");
        //        sql.AppendLine(")t5 on t2.vcPartId=t5.vcPartId and t2.vcPackingPlant=t5.vcPackingPlant and t2.vcReceiver=t5.vcReceiver and t2.vcSupplierId=t5.vcSupplierId         ");
        //        sql.AppendLine("and t1.vcYM between convert(varchar(6),t5.dFromTime,112) and convert(varchar(6),t5.dToTime,112)    ");
        //        sql.AppendLine("where cast(t1.iTzhSOQN as int) <>0 and t1.iCbSOQN%t5.iPackingQty<>0    ");
        //        DataTable dt9 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
        //        ErrorMsg(ref dterrMessage, dtc, dt9, "订货数量不是收容数的整数倍", false);
        //        #endregion

        //        #region 验证10：if一括生产 校验： 对象月 >= 实施年月时间 不能订货(数量得是0) 3个月都校验
        //        sql.Length = 0;
        //        sql.AppendLine("select * from ");
        //        sql.AppendLine("(");
        //        sql.AppendLine("	select a.vcYM,a.item,b.vcPart_id,b.vcSupplier_id");
        //        sql.AppendLine("	,case when item=1 then iCbSOQN");
        //        sql.AppendLine("	  when item=2 then iCbSOQN1");
        //        sql.AppendLine("	  when item=3 then iCbSOQN2");
        //        sql.AppendLine("	  else 0  end as iCbSOQN");
        //        sql.AppendLine("	,case when item=1 then iTzhSOQN");
        //        sql.AppendLine("		  when item=2 then iTzhSOQN1");
        //        sql.AppendLine("		  when item=3 then iTzhSOQN2");
        //        sql.AppendLine("		  else 0  end as iTzhSOQN");
        //        sql.AppendLine("	from");
        //        sql.AppendLine("	(select '" + strYearMonth + "' as vcYM ,1 as item union all select '" + strYearMonth_2 + "' as vcYM,2 as item union all select '" + strYearMonth_3 + "' as vcYM,3 as item)a");
        //        sql.AppendLine("	left join");
        //        sql.AppendLine("	(select * from TSoq_temp where vcOperator='" + strUserId + "')b on 1=1");
        //        sql.AppendLine(")t1");
        //        sql.AppendLine("left join");
        //        sql.AppendLine("(--手配主表");
        //        sql.AppendLine("	select vcPartId,vcCarfamilyCode,vcHaoJiu,vcReceiver,vcPackingPlant,vcSupplierId,vcInOut,dDebugTime,dFromTime,dToTime ");
        //        sql.AppendLine("	from TSPMaster where vcPackingPlant='" + strUnit + "' and vcReceiver='APC06' and dFromTime<>dToTime  and vcOldProduction='一括生产'");
        //        sql.AppendLine(")t2 on t1.vcPart_id=t2.vcPartId and t1.vcSupplier_id=t2.vcSupplierId and t1.vcYM between convert(varchar(6),t2.dFromTime,112) and convert(varchar(6),t2.dToTime,112)");
        //        sql.AppendLine("and t1.vcYM>=convert(varchar(6),t2.dDebugTime,112)");
        //        sql.AppendLine("where cast(t1.iTzhSOQN as int) <>0 and t2.vcPartId is not null");
        //        DataTable dt10 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
        //        ErrorMsg(ref dterrMessage, dtc, dt10, "在{0}月以后不能订货", true);
        //        #endregion

        //        #region 验证11：特殊订货不能导入  
        //        sql.Length = 0;
        //        sql.AppendLine("select * from ");
        //        sql.AppendLine("(");
        //        sql.AppendLine("	select a.vcYM,a.item,b.vcPart_id,b.vcSupplier_id");
        //        sql.AppendLine("	,case when item=1 then iCbSOQN");
        //        sql.AppendLine("	  when item=2 then iCbSOQN1");
        //        sql.AppendLine("	  when item=3 then iCbSOQN2");
        //        sql.AppendLine("	  else 0  end as iCbSOQN");
        //        sql.AppendLine("	,case when item=1 then iTzhSOQN");
        //        sql.AppendLine("		  when item=2 then iTzhSOQN1");
        //        sql.AppendLine("		  when item=3 then iTzhSOQN2");
        //        sql.AppendLine("		  else 0  end as iTzhSOQN");
        //        sql.AppendLine("	from");
        //        sql.AppendLine("	(select '" + strYearMonth + "' as vcYM ,1 as item union all select '" + strYearMonth_2 + "' as vcYM,2 as item union all select '" + strYearMonth_3 + "' as vcYM,3 as item)a");
        //        sql.AppendLine("	left join");
        //        sql.AppendLine("	(select * from TSoq_temp where vcOperator='" + strUserId + "')b on 1=1");
        //        sql.AppendLine(")t1");
        //        sql.AppendLine("left join");
        //        sql.AppendLine("(--手配主表");
        //        sql.AppendLine("	select vcPartId,vcCarfamilyCode,vcHaoJiu,vcReceiver,vcPackingPlant,vcSupplierId,vcInOut,dDebugTime,dFromTime,dToTime ");
        //        sql.AppendLine("	from TSPMaster where vcPackingPlant='" + strUnit + "' and vcReceiver='APC06' and dFromTime<>dToTime  and vcOrderingMethod='1'");
        //        sql.AppendLine(")t2 on t1.vcPart_id=t2.vcPartId and t1.vcSupplier_id=t2.vcSupplierId and t1.vcYM between convert(varchar(6),t2.dFromTime,112) and convert(varchar(6),t2.dToTime,112)");
        //        sql.AppendLine("where cast(t1.iTzhSOQN as int) <>0 and t2.vcPartId is not null");
        //        DataTable dt11 = excute.ExcuteSqlWithSelectToDT(sql.ToString());
        //        ErrorMsg(ref dterrMessage, dtc, dt11, "{0}月特殊品番不能订货", true);
        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
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
                //sql.Append("update t2 set t2.iTzhSOQN=t1.iTzhSOQN,t2.iTzhSOQN1=t1.iTzhSOQN1,t2.iTzhSOQN2=t1.iTzhSOQN2,    \n");
                //sql.Append("vcOperator='" + strUserId + "',dOperatorTime=GETDATE(),vcLastTimeFlag='" + strLastTimeFlag + "'     \n");
                //sql.Append("from(    \n");
                //sql.Append("	select * from TSoq_temp where vcYearMonth='" + strYearMonth + "' and vcOperator='" + strUserId + "'    \n");
                //sql.Append(")t1    \n");
                //sql.Append("left join (    \n");
                //sql.Append("	select * from  TSoq where vcYearMonth='" + strYearMonth + "'    \n");
                //sql.Append(")t2 on t1.vcYearMonth=t2.vcYearMonth and t1.vcPart_id=t2.vcPart_id    \n");
                //sql.Append("where t1.iTzhSOQN<>t2.iTzhSOQN or t1.iTzhSOQN1<>t2.iTzhSOQN1 or t1.iTzhSOQN2<>t2.iTzhSOQN2    \n");
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

        #region 获取数据字典
        public DataTable getTCode(string strCodeId)
        {
            try
            {
                MultiExcute excute = new MultiExcute();
                System.Data.DataTable dt = new System.Data.DataTable();
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select vcName,vcValue from TCode where vcCodeId='" + strCodeId + "' and vcValue not in ('0') ORDER BY iAutoId     \n");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 获取供应商工区
        public DataTable getWorkArea(string strSupplier_id)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select vcWorkArea as vcName,vcWorkArea as vcValue from TSupplierInfo where vcSupplier_id='" + strSupplier_id + "'    \n");
                return excute.ExcuteSqlWithSelect(sql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}