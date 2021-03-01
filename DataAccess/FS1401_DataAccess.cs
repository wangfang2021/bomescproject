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
    public class FS1401_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        public DataTable getSearchInfo(string strPartId, string strSupplierId, string strSupplierPlant, string strHaoJiu, string strInOut, string strPartArea, string strSPISStatus, string strCheckType, string strFrom, string strTo, string strCarModel, string strSPISType, List<Object> listTime)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.AppendLine("select t1.LinId as LinId");
                strSql.AppendLine("		,t1.vcPartId as vcPartId");
                strSql.AppendLine("		,t1.vcPartENName as vcPartENName");
                strSql.AppendLine("		,convert(varchar(10),t1.dFromTime,111) as dFromTime");
                strSql.AppendLine("		,convert(varchar(10),t1.dToTime,111) as dToTime");
                strSql.AppendLine("		,t1.vcCarfamilyCode as vcCarfamilyCode");
                strSql.AppendLine("		,t2.vcName as vcPartArea");
                strSql.AppendLine("		,t1.vcSupplierId as vcSupplierId");
                strSql.AppendLine("		,t1.vcSupplierPlant as vcSupplierPlant");
                strSql.AppendLine("		,case when t7.vcPicUrl is null then '无' else '有' end as vcSPISType");
                strSql.AppendLine("		,t6.vcCheckP as vcCheckType");
                strSql.AppendLine("		,t3.vcName as vcInOut");
                strSql.AppendLine("		,t4.vcName as vcHaoJiu");
                strSql.AppendLine("		,t5.vcName as vcPackType");
                strSql.AppendLine("		,t1.vcSPISStatus  as vcSPISStatus,'0' as bModFlag,'0' as bAddFlag,'0' as bSelectFlag");
                strSql.AppendLine("from ");
                strSql.AppendLine("(select * from [tCheckMethod_Master] where 1=1");
                if (strPartId != "")
                {
                    strSql.AppendLine("AND vcPartId like '" + strPartId + "%'");
                }
                if (strSupplierId != "")
                {
                    strSql.AppendLine("AND vcSupplierId='" + strSupplierId + "'");
                }
                if (strSupplierPlant != "")
                {
                    strSql.AppendLine("AND vcSupplierPlant='" + strSupplierPlant + "'");
                }
                if (strHaoJiu != "")
                {
                    strSql.AppendLine("AND vcHaoJiu='" + strHaoJiu + "'");
                }
                if (strInOut != "")
                {
                    strSql.AppendLine("AND vcInOut='" + strInOut + "'");
                }
                if (strPartArea != "")
                {
                    strSql.AppendLine("AND vcPartArea='" + strPartArea + "'");
                }
                if (strSPISStatus != "")
                {
                    strSql.AppendLine("AND vcSPISStatus='" + strSPISStatus + "'");
                }
                if (strFrom != "" || strTo != "")
                {
                    if (strFrom.Length != 0)
                    {
                        strSql.AppendLine("AND (dFromTime> '" + strFrom + "' or dFromTime= '" + strFrom + "')");
                    }
                    if (strTo.Length != 0)
                    {
                        strSql.AppendLine("AND (dToTime<'" + strTo + "' or  dToTime='" + strTo + "')");
                    }
                }
                if (strCarModel != "")
                {
                    strSql.AppendLine("AND vcCarfamilyCode='" + strCarModel + "'");
                }
                if (listTime.Count != 0)
                {
                    strSql.AppendLine("and ( ");
                    for (int i = 0; i < listTime.Count; i++)
                    {
                        if (listTime[i].ToString() == "现在")
                        {
                            strSql.AppendLine("(dFromTime<=Convert(varchar(10),getdate(),23) and dToTime>=Convert(varchar(10),getdate(),23))");
                        }
                        if (listTime[i].ToString() == "将来")
                        {
                            strSql.AppendLine("(dFromTime>Convert(varchar(10),getdate(),23))");
                        }
                        if (listTime[i].ToString() == "作废")
                        {
                            strSql.AppendLine("(dToTime<Convert(varchar(10),getdate(),23))");
                        }
                        if (i < listTime.Count - 1)
                        {
                            strSql.AppendLine(" or ");
                        }
                    }
                    strSql.AppendLine(") ");
                }
                strSql.AppendLine(")t1");
                strSql.AppendLine("left join");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C063')t2");
                strSql.AppendLine("on t1.vcPartArea=t2.vcValue");
                strSql.AppendLine("left join");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C003')t3");
                strSql.AppendLine("on t1.[vcInOut]=t3.vcValue");
                strSql.AppendLine("left join");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C004')t4");
                strSql.AppendLine("on t1.[vcHaoJiu]=t4.vcValue");
                strSql.AppendLine("left join");
                strSql.AppendLine("(SELECT vcName,vcValue FROM TCode WHERE vcCodeId='C059')t5");
                strSql.AppendLine("on t1.[vcPackType]=t5.vcValue");
                strSql.AppendLine("left join ");
                strSql.AppendLine("(SELECT [vcPartId],[vcTimeFrom],[vcTimeTo],[vcCarfamilyCode],[vcSupplierCode],[vcSupplierPlant],[vcCheckP] FROM [tCheckQf]");
                strSql.AppendLine("  where [vcTimeFrom]<=convert(varchar(10),GETDATE(),23) and [vcTimeTo]>=convert(varchar(10),GETDATE(),23))t6");
                strSql.AppendLine("on t1.vcPartId=t6.vcPartId and t1.vcSupplierId=t6.vcSupplierCode and t1.vcSupplierPlant=t6.vcSupplierPlant");
                strSql.AppendLine("left join ");
                strSql.AppendLine("(SELECT [vcPartId],[vcTimeFrom],[vcTimeTo],[vcCarfamilyCode],[vcSupplierCode],[vcSupplierPlant],[vcPicUrl] FROM [tSPISQf]");
                strSql.AppendLine("  where [vcTimeFrom]<=convert(varchar(10),GETDATE(),23) and [vcTimeTo]>=convert(varchar(10),GETDATE(),23))t7");
                strSql.AppendLine("on t1.vcPartId=t7.vcPartId and t1.vcSupplierId=t7.vcSupplierCode and t1.vcSupplierPlant=t7.vcSupplierPlant");
                strSql.AppendLine("where 1=1");
                if (strCheckType != "")
                {
                    strSql.AppendLine("and t6.vcCheckP='" + strCheckType + "'");
                }
                if (strSPISType != "")
                {
                    if (strSPISType == "有")
                        strSql.AppendLine("and t7.vcPicUrl is not null");
                    if (strSPISType == "无")
                        strSql.AppendLine("and t7.vcPicUrl is null");
                }
                strSql.AppendLine("order by t1.vcPartId,t1.dFromTime");
                return excute.ExcuteSqlWithSelectToDT(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void setSaveInfo(DataTable dtImport, string strOperId, ref DataTable dtMessage)
        {
            SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();

            sqlConnection.Open();
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            try
            {
                string strNow = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
                SqlCommand sqlCommand_modinfo = sqlConnection.CreateCommand();
                sqlCommand_modinfo.Transaction = sqlTransaction;
                sqlCommand_modinfo.CommandType = CommandType.Text;
                StringBuilder strSql_modinfo = new StringBuilder();
                strSql_modinfo.AppendLine("update [tCheckMethod_Master] set [vcSPISStatus]=@vcSPISStatus,[vcOperatorID]='" + strOperId + "',[dOperatorTime]=GETDATE() where LinId=@LinId");
                sqlCommand_modinfo.CommandText = strSql_modinfo.ToString();
                sqlCommand_modinfo.Parameters.AddWithValue("@vcSPISStatus", "");
                sqlCommand_modinfo.Parameters.AddWithValue("@LinId", "");
                foreach (DataRow item in dtImport.Rows)
                {
                    sqlCommand_modinfo.Parameters["@vcSPISStatus"].Value = item["vcSPISStatus"].ToString();
                    sqlCommand_modinfo.Parameters["@LinId"].Value = item["LinId"].ToString();
                    sqlCommand_modinfo.ExecuteNonQuery();
                }
                //提交事务
                sqlTransaction.Commit();
                sqlConnection.Close();

            }
            catch (Exception ex)
            {
                DataRow dataRow = dtMessage.NewRow();
                dataRow["vcMessage"] = "数据写入数据库失败！";
                dtMessage.Rows.Add(dataRow);
                //回滚事务
                if (sqlTransaction != null && sqlConnection != null)
                {
                    sqlTransaction.Rollback();
                    sqlConnection.Close();
                }
            }
        }
    }
}
