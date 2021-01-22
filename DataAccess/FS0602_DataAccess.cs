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
                strSql.AppendLine("	end as vcBgColor,'0' as vcModFlag,'0' as vcAddFlag");
                strSql.AppendLine("FROM (");
                strSql.AppendLine("select T1.iAutoId AS LinId,");
                strSql.AppendLine("		T1.vcYearMonth AS vcYearMonth,");
                strSql.AppendLine("		T6.vcName AS vcDyState_Name,");
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
                strSql.AppendLine(")b on a.vcYearMonth=b.vcYearMonth and a.vcPart_id=b.vcPart_id)T8 ");
                strSql.AppendLine("on T1.vcYearMonth=t8.vcYearMonth and t1.vcPart_id=t8.vcPart_id ");
                strSql.AppendLine(")TT");
                strSql.AppendLine("order by vcYearMonth,vcSupplierId,vcSupplierPlant,vcPart_id");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
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
                    strSql_modinfo.AppendLine(" AND [vcPart_id] = @vcPart_id");
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