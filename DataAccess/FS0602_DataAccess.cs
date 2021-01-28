using System;
using Common;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using DataEntity;
using System.Collections.Generic;

namespace DataAccess
{
    public class FS0602_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        public DataTable getSearchInfo(string strYearMonth, string strDyState, string strHyState, string strPartId, string strCarModel,
                  string strInOut, string strOrderingMethod, string strOrderPlant, string strHaoJiu, string strSupplierId, string strSupplierPlant,
                  string strDyInfo, string strHyInfo)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("SELECT TT.*,");
                strSql.AppendLine("case when TT.iTzhSOQN is null or TT.iTzhSOQN1 is null or TT.iTzhSOQN2 is null then 'partFS0602A' --无调整");
                strSql.AppendLine("	when TT.iTzhSOQN=TT.iCbSOQN and TT.iTzhSOQN1=TT.iCbSOQN1 and TT.iTzhSOQN2=TT.iCbSOQN2 then 'partFS0602A' --无调整");
                strSql.AppendLine("	else 'partFS0602B' --有调整");
                strSql.AppendLine("	end as vcBgColor,");
                strSql.AppendLine("	'0' as bModFlag,'0' as bAddFlag,CASE WHEN vcHyState='1' or vcHyState='2' then '1' else '0' end as bSelectFlag");
                strSql.AppendLine("FROM (");
                strSql.AppendLine("select T1.iAutoId AS LinId,");
                strSql.AppendLine("		T1.vcYearMonth AS vcYearMonth,");
                strSql.AppendLine("		T1.vcDyState,");
                strSql.AppendLine("		T6.vcName AS vcDyState_Name,");
                strSql.AppendLine("		T1.vcHyState,");
                strSql.AppendLine("		T7.vcName AS vcHyState_Name,");
                strSql.AppendLine("		T1.vcPart_id AS vcPart_id,");
                strSql.AppendLine("		T1.vcCarFamilyCode AS vcCarfamilyCode,");
                strSql.AppendLine("		T3.vcName AS vcHaoJiu,");
                strSql.AppendLine("		T4.vcName AS vcOrderingMethod,");
                strSql.AppendLine("		T5.vcName AS vcOrderPlant,");
                strSql.AppendLine("		T2.vcName AS vcInOut,");
                strSql.AppendLine("		T1.vcSupplier_id AS vcSupplierId,");
                strSql.AppendLine("		T1.vcSupplierPlant AS vcSupplierPlant,");
                strSql.AppendLine("		T1.iQuantityPercontainer AS iPackingQty,");
                strSql.AppendLine("		T1.iCbSOQN AS iCbSOQN,");
                strSql.AppendLine("		T1.decCbBdl AS decCbBdl,");
                strSql.AppendLine("		T1.iCbSOQN1 AS iCbSOQN1,");
                strSql.AppendLine("		T1.iCbSOQN2 AS iCbSOQN2,");
                strSql.AppendLine("		CASE WHEN T1.iTzhSOQN IS NULL THEN ISNULL(T8.iTzhSOQN,0) ELSE ISNULL(T1.iTzhSOQN,0) END AS iTzhSOQN,");
                strSql.AppendLine("		CASE WHEN T1.iTzhSOQN1 IS NULL THEN ISNULL(T8.iTzhSOQN1,0) ELSE ISNULL(T1.iTzhSOQN1,0) END AS iTzhSOQN1,");
                strSql.AppendLine("		CASE WHEN T1.iTzhSOQN2 IS NULL THEN ISNULL(T8.iTzhSOQN2,0) ELSE ISNULL(T1.iTzhSOQN2,0) END AS iTzhSOQN2,");
                strSql.AppendLine("		T1.iHySOQN AS iHySOQN,");
                strSql.AppendLine("		T1.iHySOQN1 AS iHySOQN1,");
                strSql.AppendLine("		T1.iHySOQN2 AS iHySOQN2,");
                strSql.AppendLine("		CASE WHEN T1.dExpectTime IS NULL THEN '' ELSE CONVERT(VARCHAR(10),T1.dExpectTime,23) END AS dExpectTime,");
                strSql.AppendLine("		CASE WHEN T1.dSReplyTime IS NULL THEN '' ELSE CONVERT(VARCHAR(10),T1.dSReplyTime,120) END AS dSReplyTime,");
                strSql.AppendLine("		CASE WHEN T1.dExpectTime IS NULL THEN '' ELSE (");
                strSql.AppendLine("			CASE WHEN T1.dSReplyTime IS NULL THEN (CASE WHEN CONVERT(VARCHAR(10),T1.dExpectTime,23)>=CONVERT(VARCHAR(10),GETDATE(),23) THEN '' ELSE '逾期' END) ");
                strSql.AppendLine("				ELSE (CASE WHEN CONVERT(VARCHAR(10),T1.dSReplyTime,23)<=CONVERT(VARCHAR(10),T1.dExpectTime,23) THEN '' ELSE '逾期' END) END ");
                strSql.AppendLine("		) END AS vcOverDue,		");
                strSql.AppendLine("		CASE WHEN T1.dHyTime IS NULL THEN '' ELSE CONVERT(VARCHAR(10),T1.dHyTime,120) END AS dHyTime");
                strSql.AppendLine("		from ");
                strSql.AppendLine("(select * from TSoq ");
                strSql.AppendLine("where 1=1 and vcDyState in (" + strDyInfo + ") and vcHyState in (" + strHyInfo + ") ");
                if (strYearMonth != "")
                {
                    strSql.AppendLine("and vcYearMonth='" + strYearMonth + "' ");
                }
                if (strDyState != "")
                {
                    strSql.AppendLine("and vcDyState ='" + strDyState + "'");
                }
                if (strHyState != "")
                {
                    strSql.AppendLine("and vcHyState ='" + strHyState + "'");
                }
                if (strPartId != "")
                {
                    strSql.AppendLine("and vcPart_id like '%" + strPartId + "%'");
                }
                if (strCarModel != "")
                {
                    strSql.AppendLine("and vcCarFamilyCode='" + strCarModel + "'");
                }
                if (strInOut != "")
                {
                    strSql.AppendLine("and vcInOutFlag='" + strInOut + "'");
                }
                if (strOrderingMethod != "")
                {
                    strSql.AppendLine("and vcMakingOrderType='" + strOrderingMethod + "'");
                }
                if (strOrderPlant != "")
                {
                    strSql.AppendLine("and vcFZGC='" + strOrderPlant + "'");
                }
                if (strHaoJiu != "")
                {
                    strSql.AppendLine("and vcCurrentPastcode='" + strHaoJiu + "'");
                }
                if (strSupplierId != "")
                {
                    strSql.AppendLine("and vcSupplier_id='" + strSupplierId + "'");
                }
                if (strSupplierPlant != "")
                {
                    strSql.AppendLine("and vcSupplierPlant='" + strSupplierPlant + "'");
                }
                strSql.AppendLine(")T1 ");
                strSql.AppendLine("left join ");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C003')T2--内外区分 ");
                strSql.AppendLine("on T1.vcInOutFlag=T2.vcValue ");
                strSql.AppendLine("left join ");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C004')T3--号旧区分 ");
                strSql.AppendLine("on T1.vcCurrentPastcode=T3.vcValue ");
                strSql.AppendLine("left join ");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C047')T4--订货方式 ");
                strSql.AppendLine("on T1.vcMakingOrderType=T4.vcValue ");
                strSql.AppendLine("left join ");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C000')T5--发注工厂 ");
                strSql.AppendLine("on T1.vcFZGC=T5.vcValue ");
                strSql.AppendLine("left join ");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C036')T6--对应状态 ");
                strSql.AppendLine("on T1.vcDyState=T6.vcValue ");
                strSql.AppendLine("left join ");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C037')T7--合意状态 ");
                strSql.AppendLine("on T1.vcHyState=T7.vcValue ");
                strSql.AppendLine("left join ");
                strSql.AppendLine("(select a.vcYearMonth,a.vcPart_id,a.iTzhSOQN,a.iTzhSOQN1,a.iTzhSOQN2      ");
                strSql.AppendLine("from  ");
                strSql.AppendLine("(select * from TSoq_OperHistory  ");
                strSql.AppendLine("where vcInputType in ('supplier','company'))a     ");
                strSql.AppendLine("inner join  ");
                strSql.AppendLine("(select vcYearMonth,vcPart_id,MAX(dOperatorTime) as dOperatorTime from TSoq_OperHistory ");
                strSql.AppendLine("where vcInputType in ('supplier','company')   ");
                strSql.AppendLine("group by vcYearMonth,vcPart_id     ");
                strSql.AppendLine(")b on a.vcYearMonth=b.vcYearMonth and a.vcPart_id=b.vcPart_id  and a.dOperatorTime=b.dOperatorTime)T8 ");
                strSql.AppendLine("on T1.vcYearMonth=t8.vcYearMonth and t1.vcPart_id=t8.vcPart_id");
                strSql.AppendLine(")TT");
                strSql.AppendLine("order by vcYearMonth,vcSupplierId,vcSupplierPlant,vcPart_id");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool checkDbInfo(string strYearMonth)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine(" select isnull(sum(cast(isnull(vcDyState,0) as int)),0) as DyState from [TSoq] where vcYearMonth='" + strYearMonth + "'");
                DataTable dataTable = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                if (dataTable != null && dataTable.Rows[0]["DyState"].ToString() == "0")
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setSOQInfo(string strOperationType, DataTable dtModInfo, string strOperId)
        {
            SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();

            sqlConnection.Open();
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            try
            {
                #region sqlCommand_modinfo 展开内示
                if (strOperationType == "展开内示")
                {
                    SqlCommand sqlCommand_modinfo = sqlConnection.CreateCommand();
                    sqlCommand_modinfo.Transaction = sqlTransaction;
                    sqlCommand_modinfo.CommandType = CommandType.Text;
                    StringBuilder strSql_modinfo = new StringBuilder();
                    strSql_modinfo.AppendLine("UPDATE [dbo].[TSoq]");
                    strSql_modinfo.AppendLine("   SET [vcDyState] = @vcDyState");
                    //strSql_modinfo.AppendLine("      ,[vcHyState] = @vcHyState");
                    strSql_modinfo.AppendLine("      ,[dExpectTime] = @dExpectTime");
                    strSql_modinfo.AppendLine("      ,[dOpenTime] = GETDATE()");
                    strSql_modinfo.AppendLine("      ,[vcOpenUser] = '" + strOperId + "'");
                    strSql_modinfo.AppendLine(" WHERE [vcYearMonth] = @vcYearMonth");
                    strSql_modinfo.AppendLine(" AND [vcPart_id] = @vcPart_id and vcHyState in ('0','3')");
                    strSql_modinfo.AppendLine(" declare @flag int");
                    strSql_modinfo.AppendLine(" set @flag=(select Count(*) from [TSoq_OperHistory] where vcYearMonth=@vcYearMonth and vcPart_id=@vcPart_id)");
                    strSql_modinfo.AppendLine(" if(@flag=0)");
                    strSql_modinfo.AppendLine(" begin");
                    strSql_modinfo.AppendLine(" INSERT INTO [dbo].[TSoq_OperHistory]([vcYearMonth],[vcPart_id],[iTzhSOQN],[iTzhSOQN1],[iTzhSOQN2],[vcInputType],[vcOperator],[dOperatorTime])");
                    strSql_modinfo.AppendLine(" select [vcYearMonth],[vcPart_id],iCbSOQN,iCbSOQN1,iCbSOQN2,'company' as [vcInputType],'" + strOperId + "' as [vcOperator],GETDATE() as [dOperatorTime] ");
                    strSql_modinfo.AppendLine(" from TSoq where vcYearMonth=@vcYearMonth and vcPart_id=@vcPart_id and vcHyState in ('0','3')");
                    strSql_modinfo.AppendLine(" end");
                    sqlCommand_modinfo.CommandText = strSql_modinfo.ToString();
                    sqlCommand_modinfo.Parameters.AddWithValue("@vcDyState", "");
                    //sqlCommand_modinfo.Parameters.AddWithValue("@vcHyState", "");
                    sqlCommand_modinfo.Parameters.AddWithValue("@dExpectTime", "");
                    sqlCommand_modinfo.Parameters.AddWithValue("@vcYearMonth", "");
                    sqlCommand_modinfo.Parameters.AddWithValue("@vcPart_id", "");
                    foreach (DataRow item in dtModInfo.Rows)
                    {
                        sqlCommand_modinfo.Parameters["@vcDyState"].Value = item["vcDyState"].ToString();
                        //sqlCommand_modinfo.Parameters["@vcHyState"].Value = item["vcHyState"].ToString();
                        sqlCommand_modinfo.Parameters["@dExpectTime"].Value = item["dExpectTime"].ToString();
                        sqlCommand_modinfo.Parameters["@vcYearMonth"].Value = item["vcYearMonth"].ToString();
                        sqlCommand_modinfo.Parameters["@vcPart_id"].Value = item["vcPart_id"].ToString();
                        sqlCommand_modinfo.ExecuteNonQuery();
                    }
                }
                #endregion
                #region sqlCommand_modinfo 退回内示
                if (strOperationType == "退回内示")
                {
                    SqlCommand sqlCommand_modinfo = sqlConnection.CreateCommand();
                    sqlCommand_modinfo.Transaction = sqlTransaction;
                    sqlCommand_modinfo.CommandType = CommandType.Text;
                    StringBuilder strSql_modinfo = new StringBuilder();
                    strSql_modinfo.AppendLine("DELETE FROM [TSoq]  WHERE [vcYearMonth] = @vcYearMonth ");
                    strSql_modinfo.AppendLine("DELETE FROM TSoq_OperHistory WHERE [vcYearMonth] = @vcYearMonth ");
                    sqlCommand_modinfo.CommandText = strSql_modinfo.ToString();
                    sqlCommand_modinfo.Parameters.AddWithValue("@vcYearMonth", "");
                    foreach (DataRow item in dtModInfo.Rows)
                    {
                        sqlCommand_modinfo.Parameters["@vcYearMonth"].Value = item["vcYearMonth"].ToString();
                        sqlCommand_modinfo.ExecuteNonQuery();
                    }
                }
                #endregion
                #region sqlCommand_modinfo 回复内示
                if (strOperationType == "回复内示")
                {
                    SqlCommand sqlCommand_modinfo = sqlConnection.CreateCommand();
                    sqlCommand_modinfo.Transaction = sqlTransaction;
                    sqlCommand_modinfo.CommandType = CommandType.Text;
                    StringBuilder strSql_modinfo = new StringBuilder();
                    strSql_modinfo.AppendLine("update T1 SET");
                    strSql_modinfo.AppendLine("	 T1.[vcHyState]=@vcHyState");
                    strSql_modinfo.AppendLine("	,T1.iTzhSOQN=ISNULL(T2.iTzhSOQN,0)");
                    strSql_modinfo.AppendLine("	,T1.iTzhSOQN1=ISNULL(T2.iTzhSOQN1,0)");
                    strSql_modinfo.AppendLine("	,T1.iTzhSOQN2=ISNULL(T2.iTzhSOQN2,0)");
                    strSql_modinfo.AppendLine("	,T1.dReplyTime=GETDATE()");
                    strSql_modinfo.AppendLine("	,T1.vcReplyUser='" + strOperId + "' from ");
                    strSql_modinfo.AppendLine("(select * from TSoq ");
                    strSql_modinfo.AppendLine("where 1=1 and vcDyState in ('0','1','2','3') and vcHyState in ('0','3') ");
                    strSql_modinfo.AppendLine("and vcYearMonth=@vcYearMonth and vcPart_id=@vcPart_id ");
                    strSql_modinfo.AppendLine(")T1 ");
                    strSql_modinfo.AppendLine("left join ");
                    strSql_modinfo.AppendLine("(select a.vcYearMonth,a.vcPart_id,a.iTzhSOQN,a.iTzhSOQN1,a.iTzhSOQN2      ");
                    strSql_modinfo.AppendLine("from  ");
                    strSql_modinfo.AppendLine("(select * from TSoq_OperHistory  ");
                    strSql_modinfo.AppendLine("where vcInputType in ('supplier','company'))a     ");
                    strSql_modinfo.AppendLine("inner join  ");
                    strSql_modinfo.AppendLine("(select vcYearMonth,vcPart_id,MAX(dOperatorTime) as dOperatorTime from TSoq_OperHistory");
                    strSql_modinfo.AppendLine("where vcInputType in ('supplier','company')   ");
                    strSql_modinfo.AppendLine("group by vcYearMonth,vcPart_id     ");
                    strSql_modinfo.AppendLine(")b on a.vcYearMonth=b.vcYearMonth and a.vcPart_id=b.vcPart_id)T2");
                    strSql_modinfo.AppendLine("on T1.vcYearMonth=T2.vcYearMonth and T1.vcPart_id=T2.vcPart_id ");
                    sqlCommand_modinfo.CommandText = strSql_modinfo.ToString();
                    sqlCommand_modinfo.Parameters.AddWithValue("@vcYearMonth", "");
                    sqlCommand_modinfo.Parameters.AddWithValue("@vcPart_id", "");
                    sqlCommand_modinfo.Parameters.AddWithValue("@vcHyState", "");
                    foreach (DataRow item in dtModInfo.Rows)
                    {
                        sqlCommand_modinfo.Parameters["@vcYearMonth"].Value = item["vcYearMonth"].ToString();
                        sqlCommand_modinfo.Parameters["@vcPart_id"].Value = item["vcPart_id"].ToString();
                        sqlCommand_modinfo.Parameters["@vcHyState"].Value = item["vcHyState"].ToString();
                        sqlCommand_modinfo.ExecuteNonQuery();
                    }
                }
                #endregion
                #region sqlCommand_modinfo 提交内示
                if (strOperationType == "提交内示")
                {
                    SqlCommand sqlCommand_modinfo = sqlConnection.CreateCommand();
                    sqlCommand_modinfo.Transaction = sqlTransaction;
                    sqlCommand_modinfo.CommandType = CommandType.Text;
                    StringBuilder strSql_modinfo = new StringBuilder();
                    strSql_modinfo.AppendLine("UPDATE [dbo].[TSoq]");
                    strSql_modinfo.AppendLine("   SET [vcDyState] = @vcDyState");
                    strSql_modinfo.AppendLine("      ,[dSReplyTime] = GETDATE()");
                    strSql_modinfo.AppendLine("      ,[vcSReplyUser] = '" + strOperId + "'");
                    strSql_modinfo.AppendLine(" WHERE [vcYearMonth] = @vcYearMonth");
                    strSql_modinfo.AppendLine(" AND [vcPart_id] = @vcPart_id");
                    sqlCommand_modinfo.CommandText = strSql_modinfo.ToString();
                    sqlCommand_modinfo.Parameters.AddWithValue("@vcDyState", "");
                    sqlCommand_modinfo.Parameters.AddWithValue("@vcYearMonth", "");
                    sqlCommand_modinfo.Parameters.AddWithValue("@vcPart_id", "");
                    foreach (DataRow item in dtModInfo.Rows)
                    {
                        sqlCommand_modinfo.Parameters["@vcDyState"].Value = item["vcDyState"].ToString();
                        sqlCommand_modinfo.Parameters["@vcYearMonth"].Value = item["vcYearMonth"].ToString();
                        sqlCommand_modinfo.Parameters["@vcPart_id"].Value = item["vcPart_id"].ToString();
                        sqlCommand_modinfo.ExecuteNonQuery();
                    }
                }
                #endregion
                //提交事务
                sqlTransaction.Commit();
                sqlConnection.Close();

            }
            catch (Exception ex)
            {
                //回滚事务
                if (sqlTransaction != null && sqlConnection != null)
                {
                    sqlTransaction.Rollback();
                    sqlConnection.Close();
                }
            }
        }
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
                        bool bmodflag = (bool)listInfoData[i]["bModFlag"];//true可编辑,false不可编辑
                        bool baddflag = (bool)listInfoData[i]["bAddFlag"];//true可编辑,false不可编辑
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
                strSql.AppendLine(" select * from #TSoq_temp_cr where (vcHyState not in ('0','3'))  ");//这几个状态(1,0,3)是可操作的状态
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
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
                    bool bmodflag = (bool)listInfoData[i]["bModFlag"];//true可编辑,false不可编辑
                    bool baddflag = (bool)listInfoData[i]["bAddFlag"];//true可编辑,false不可编辑
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
                        sql.Append("'" + listInfoData[i]["vcSupplierId"].ToString() + "',");
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
                    errMessageList.Add("品番" + strPart_id + "在品番基础信息里不存在");
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
                    errMessageList.Add("品番" + strPart_id + "的3个月数量不能全部为0");
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
                sql.Append("	where vcPackingPlant='" + strUnit + "' and vcReceiver='APC06'         \n");
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
                sql.Append("	where vcPackingPlant='" + strUnit + "' and vcReceiver='APC06'         \n");
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
                sql.Append("	where vcPackingPlant='" + strUnit + "' and vcReceiver='APC06'         \n");
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
        public void importSave(string strYearMonth, string strUserId, string strUnit)
        {
            try
            {
                string strLastTimeFlag = DateTime.Now.ToString("yyyyMMddHHmmss");
                DateTime now = DateTime.Now;
                StringBuilder sql = new StringBuilder();
                //插历史
                sql.Append("insert into TSoq_OperHistory (vcYearMonth,vcPart_id,iTzhSOQN,iTzhSOQN1,iTzhSOQN2,vcInputType,vcOperator,dOperatorTime)    \n");
                sql.Append("select vcYearMonth,vcPart_id,iTzhSOQN,iTzhSOQN1,iTzhSOQN2,'company',vcOperator,GETDATE()     \n");
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
                sql.Append("update t1 set     \n");
                sql.Append("t1.vcCarFamilyCode=t2.vcCarfamilyCode,    \n");
                sql.Append("t1.vcCurrentPastcode=t2.vcHaoJiu,    \n");
                sql.Append("t1.vcMakingOrderType=t2.vcReceiver,    \n");
                sql.Append("t1.vcFZGC=t3.vcOrderPlant,    \n");
                sql.Append("t1.vcInOutFlag=t2.vcInOut,    \n");
                sql.Append("t1.vcSupplier_id=t2.vcSupplierId,    \n");
                sql.Append("t1.vcSupplierPlant=t4.vcSupplierPlant,    \n");
                sql.Append("t1.iQuantityPercontainer=t5.iPackingQty   \n");
                sql.Append("from (        \n");
                sql.Append("	select * from TSoq where vcYearMonth='" + strYearMonth + "'    \n");
                sql.Append(")t1        \n");
                sql.Append("inner join (    \n");
                sql.Append("	select * from TSoq_temp where vcYearMonth='" + strYearMonth + "' and vcOperator='" + strUserId + "'    \n");
                sql.Append(")t1_1 on t1.vcYearMonth=t1_1.vcYearMonth and t1.vcPart_id=t1_1.vcPart_id    \n");
                sql.Append("left join (    \n");
                sql.Append("	select vcPartId,vcCarfamilyCode,vcHaoJiu,vcReceiver,vcPackingPlant,vcSupplierId,vcInOut    \n");
                sql.Append("	from TSPMaster     \n");
                sql.Append("	where vcPackingPlant='" + strUnit + "' and vcReceiver='APC06'     \n");
                sql.Append("	and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)     \n");
                sql.Append(")t2 on t1.vcPart_id=t2.vcPartId    \n");
                sql.Append("left join (    \n");
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcOrderPlant     \n");
                sql.Append("	from TSPMaster_OrderPlant     \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t3 on t2.vcPartId=t3.vcPartId and t2.vcPackingPlant=t3.vcPackingPlant     \n");
                sql.Append("and t2.vcReceiver=t3.vcReceiver and t2.vcSupplierId=t3.vcSupplierId    \n");
                sql.Append("left join (    \n");
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant     \n");
                sql.Append("	from TSPMaster_SupplierPlant     \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t4 on t2.vcPartId=t4.vcPartId and t2.vcPackingPlant=t4.vcPackingPlant     \n");
                sql.Append("and t2.vcReceiver=t4.vcReceiver and t2.vcSupplierId=t4.vcSupplierId    \n");
                sql.Append("left join(    \n");
                sql.Append("	select vcPartId,vcReceiver,vcPackingPlant,vcSupplierId,vcSupplierPlant,iPackingQty     \n");
                sql.Append("	from TSPMaster_Box     \n");
                sql.Append("	where vcOperatorType='1' and '" + strYearMonth + "' between convert(varchar(6),dFromTime,112) and convert(varchar(6),dToTime,112)    \n");
                sql.Append(")t5 on t2.vcPartId=t5.vcPartId and t2.vcPackingPlant=t5.vcPackingPlant     \n");
                sql.Append("and t2.vcReceiver=t5.vcReceiver and t2.vcSupplierId=t5.vcSupplierId     \n");
                sql.Append("and t4.vcSupplierPlant=t5.vcSupplierPlant    \n");
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
        public DataTable getEmail(string vcSupplier_id)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select distinct vcLinkMan,vcEmail from [dbo].[TSupplierInfo] where vcSupplier_id in (" + vcSupplier_id + ")");

                DataTable dt = excute.ExcuteSqlWithSelectToDT(strSql.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region 承认。将合意后SOQ数据复制到合意SOQ，并改变合意状态，赋予合意时间
        public int Cr(string varDxny, string varDyzt, string varHyzt, string PARTSNO)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                System.Data.SqlClient.SqlParameter[] parameters = {
                    new SqlParameter("@varDxny", SqlDbType.VarChar,10),
                    new SqlParameter("@varDyzt", SqlDbType.VarChar,2),
                    new SqlParameter("@varHyzt", SqlDbType.VarChar,2),
                    new SqlParameter("@PARTSNO", SqlDbType.VarChar,50),
                    new SqlParameter("@dHyTime", SqlDbType.VarChar,50),
                };
                parameters[0].Value = varDxny;
                parameters[1].Value = varDyzt;
                parameters[2].Value = varHyzt;
                parameters[3].Value = PARTSNO;
                parameters[4].Value = DateTime.Now;

                strSql.AppendLine(" UPDATE TSoq SET ");
                strSql.AppendLine("      iHySOQN=iTzhSOQN,");
                strSql.AppendLine("      iHySOQN1=iTzhSOQN1,");
                strSql.AppendLine("      iHySOQN2=iTzhSOQN2,");
                strSql.AppendLine("      varHyzt='2', ");
                strSql.AppendLine("      dHyTime=@dHyTime ");
                strSql.AppendLine(" WHERE 1=1 ");
                //筛选条件：对象年月
                if (!string.IsNullOrEmpty(varDxny))
                {
                    strSql.AppendLine(" AND varDxny=@varDxny ");
                }
                //筛选条件：对应状态
                if (!string.IsNullOrEmpty(varDyzt))
                {
                    strSql.AppendLine(" AND varDyzt=@varDyzt ");
                }
                //筛选条件：合意状态
                if (!string.IsNullOrEmpty(varHyzt))
                {
                    strSql.AppendLine(" AND varHyzt=@varHyzt ");
                }
                //筛选条件：品番
                if (!string.IsNullOrEmpty(PARTSNO))
                {
                    strSql.AppendLine(" AND PARTSNO like '%'+@PARTSNO+'%' ");
                }

                return excute.ExcuteSqlWithStringOper(strSql.ToString(), parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region 展开。将初版SOQ数据复制到调整后SOQ，并改变对应状态
        public int Zk(FS0602_DataEntity searchForm)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                System.Data.SqlClient.SqlParameter[] parameters = {
                    new SqlParameter("@varDxny", SqlDbType.VarChar),
                    new SqlParameter("@varDyzt", SqlDbType.VarChar),
                    new SqlParameter("@varHyzt", SqlDbType.VarChar),
                    new SqlParameter("@PARTSNO", SqlDbType.VarChar),

                    new SqlParameter("@CARFAMILYCODE", SqlDbType.VarChar),
                    new SqlParameter("@CURRENTPASTCODE", SqlDbType.VarChar),
                    new SqlParameter("@varMakingOrderType", SqlDbType.VarChar),
                    new SqlParameter("@iFZGC", SqlDbType.VarChar),

                    new SqlParameter("@INOUTFLAG", SqlDbType.VarChar),
                    new SqlParameter("@SUPPLIERCODE", SqlDbType.VarChar),
                    new SqlParameter("@iSupplierPlant", SqlDbType.Int),
                };
                parameters[0].Value = searchForm.varDxny;
                parameters[1].Value = searchForm.varDyzt;
                parameters[2].Value = searchForm.varHyzt;
                parameters[3].Value = searchForm.PARTSNO;
                parameters[4].Value = searchForm.CARFAMILYCODE;
                parameters[5].Value = searchForm.CURRENTPASTCODE;
                parameters[6].Value = searchForm.varMakingOrderType;
                parameters[7].Value = searchForm.iFZGC;
                parameters[8].Value = searchForm.INOUTFLAG;
                parameters[9].Value = searchForm.SUPPLIERCODE;
                parameters[10].Value = searchForm.iSupplierPlant == null ? 0 : searchForm.iSupplierPlant;


                strSql.AppendLine(" UPDATE TSoq SET ");
                strSql.AppendLine("      iTzhSOQN=iCbSOQN,");
                strSql.AppendLine("      iTzhSOQN1=iCbSOQN1,");
                strSql.AppendLine("      iTzhSOQN2=iCbSOQN2,");
                strSql.AppendLine("      varDyzt='1' ");
                strSql.AppendLine(" WHERE 1=1 ");

                //筛选条件：对象年月
                if (!string.IsNullOrEmpty(searchForm.varDxny))
                {
                    strSql.Append(" AND varDxny=@varDxny");
                }
                //筛选条件：对应状态
                if (!string.IsNullOrEmpty(searchForm.varDyzt))
                {
                    strSql.Append(" AND varDyzt=@varDyzt");
                }
                //筛选条件：合意状态
                if (!string.IsNullOrEmpty(searchForm.varHyzt))
                {
                    strSql.Append(" AND varHyzt=@varHyzt");
                }
                //筛选条件：品番
                if (!string.IsNullOrEmpty(searchForm.PARTSNO))
                {
                    strSql.Append(" AND PARTSNO=@PARTSNO");
                }
                //筛选条件：车型编码
                if (!string.IsNullOrEmpty(searchForm.CARFAMILYCODE))
                {
                    strSql.Append(" AND CARFAMILYCODE=@CARFAMILYCODE");
                }
                //筛选条件：号旧区分
                if (!string.IsNullOrEmpty(searchForm.CURRENTPASTCODE))
                {
                    strSql.Append(" AND CURRENTPASTCODE=@CURRENTPASTCODE");
                }
                //筛选条件：订货频度
                if (!string.IsNullOrEmpty(searchForm.varMakingOrderType))
                {
                    strSql.Append(" AND varMakingOrderType=@varMakingOrderType");
                }
                //筛选条件：发注工厂
                if (!string.IsNullOrEmpty(searchForm.iFZGC))
                {
                    strSql.Append(" AND iFZGC=@iFZGC");
                }
                //筛选条件：内外
                if (!string.IsNullOrEmpty(searchForm.INOUTFLAG))
                {
                    strSql.Append(" AND INOUTFLAG=@INOUTFLAG");
                }
                //筛选条件：供应商代码
                if (!string.IsNullOrEmpty(searchForm.SUPPLIERCODE))
                {
                    strSql.Append(" AND SUPPLIERCODE=@SUPPLIERCODE");
                }
                //筛选条件：供应商工区
                if (searchForm.iSupplierPlant != null)
                {
                    strSql.Append(" AND iSupplierPlant=@iSupplierPlant");
                }

                return excute.ExcuteSqlWithStringOper(strSql.ToString(), parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region 回复。改变合意状态
        public int Hf(FS0602_DataEntity searchForm)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                System.Data.SqlClient.SqlParameter[] parameters = {
                    new SqlParameter("@varDxny", SqlDbType.VarChar),
                    new SqlParameter("@varDyzt", SqlDbType.VarChar),
                    new SqlParameter("@varHyzt", SqlDbType.VarChar),
                    new SqlParameter("@PARTSNO", SqlDbType.VarChar),

                    new SqlParameter("@CARFAMILYCODE", SqlDbType.VarChar),
                    new SqlParameter("@CURRENTPASTCODE", SqlDbType.VarChar),
                    new SqlParameter("@varMakingOrderType", SqlDbType.VarChar),
                    new SqlParameter("@iFZGC", SqlDbType.VarChar),

                    new SqlParameter("@INOUTFLAG", SqlDbType.VarChar),
                    new SqlParameter("@SUPPLIERCODE", SqlDbType.VarChar),
                    new SqlParameter("@iSupplierPlant", SqlDbType.Int),
                };
                parameters[0].Value = searchForm.varDxny;
                parameters[1].Value = searchForm.varDyzt;
                parameters[2].Value = searchForm.varHyzt;
                parameters[3].Value = searchForm.PARTSNO;
                parameters[4].Value = searchForm.CARFAMILYCODE;
                parameters[5].Value = searchForm.CURRENTPASTCODE;
                parameters[6].Value = searchForm.varMakingOrderType;
                parameters[7].Value = searchForm.iFZGC;
                parameters[8].Value = searchForm.INOUTFLAG;
                parameters[9].Value = searchForm.SUPPLIERCODE;
                parameters[10].Value = searchForm.iSupplierPlant == null ? 0 : searchForm.iSupplierPlant;


                strSql.AppendLine(" UPDATE TSoq SET ");
                strSql.AppendLine("      varHyzt='1' ");
                strSql.AppendLine(" WHERE 1=1 ");

                //筛选条件：对象年月
                if (!string.IsNullOrEmpty(searchForm.varDxny))
                {
                    strSql.Append(" AND varDxny=@varDxny");
                }
                //筛选条件：对应状态
                if (!string.IsNullOrEmpty(searchForm.varDyzt))
                {
                    strSql.Append(" AND varDyzt=@varDyzt");
                }
                //筛选条件：合意状态
                if (!string.IsNullOrEmpty(searchForm.varHyzt))
                {
                    strSql.Append(" AND varHyzt=@varHyzt");
                }
                //筛选条件：品番
                if (!string.IsNullOrEmpty(searchForm.PARTSNO))
                {
                    strSql.Append(" AND PARTSNO=@PARTSNO");
                }
                //筛选条件：车型编码
                if (!string.IsNullOrEmpty(searchForm.CARFAMILYCODE))
                {
                    strSql.Append(" AND CARFAMILYCODE=@CARFAMILYCODE");
                }
                //筛选条件：号旧区分
                if (!string.IsNullOrEmpty(searchForm.CURRENTPASTCODE))
                {
                    strSql.Append(" AND CURRENTPASTCODE=@CURRENTPASTCODE");
                }
                //筛选条件：订货频度
                if (!string.IsNullOrEmpty(searchForm.varMakingOrderType))
                {
                    strSql.Append(" AND varMakingOrderType=@varMakingOrderType");
                }
                //筛选条件：发注工厂
                if (!string.IsNullOrEmpty(searchForm.iFZGC))
                {
                    strSql.Append(" AND iFZGC=@iFZGC");
                }
                //筛选条件：内外
                if (!string.IsNullOrEmpty(searchForm.INOUTFLAG))
                {
                    strSql.Append(" AND INOUTFLAG=@INOUTFLAG");
                }
                //筛选条件：供应商代码
                if (!string.IsNullOrEmpty(searchForm.SUPPLIERCODE))
                {
                    strSql.Append(" AND SUPPLIERCODE=@SUPPLIERCODE");
                }
                //筛选条件：供应商工区
                if (searchForm.iSupplierPlant != null)
                {
                    strSql.Append(" AND iSupplierPlant=@iSupplierPlant");
                }

                return excute.ExcuteSqlWithStringOper(strSql.ToString(), parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 退回内示。删除该对象月3个月所有SOQ数据，并将soq履历表中的状态改为退回。
        public int thns(FS0602_DataEntity searchForm)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                System.Data.SqlClient.SqlParameter[] parameters = {
                    new SqlParameter("@varDxny", SqlDbType.VarChar),
                };
                parameters[0].Value = searchForm.varDxny;

                //删除内示数据
                strSql.AppendLine(" DELETE FROM TSoq ");
                strSql.AppendLine(" WHERE varDxny=@varDxny; ");

                //将履历状态设置为被退回
                strSql.AppendLine(" UPDATE TSoqInput SET iState='2' ");
                strSql.AppendLine(" WHERE varDxny=@varDxny ");
                strSql.AppendLine(" AND iState='0'; ");


                return excute.ExcuteSqlWithStringOper(strSql.ToString(), parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}