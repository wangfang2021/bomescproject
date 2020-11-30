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
    public class FS1202_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        public DataTable dt_GetSearch(string ddlpro, string ddlgroup)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                SqlParameter[] paras = {
                    new SqlParameter("@ddlpro", ddlpro),
                    new SqlParameter("@ddlgroup", ddlgroup)
                };
                sql.AppendLine(" select  a.vcPorType, a.vcZB, a.KBpartType,a.LogicType, a.vcProName0, a.vcLT0, ");
                sql.AppendLine("a.vcCalendar0, a.vcProName1, a.vcLT1, a.vcCalendar1, a.vcProName2, a.vcLT2, a.vcCalendar2, a.vcProName3,");
                sql.AppendLine(" a.vcLT3, a.vcCalendar3, a.vcProName4, a.vcLT4, a.vcCalendar4 ");
                sql.AppendLine(", '0' as iflag from ProRuleMst as a");
                sql.AppendLine(" left join (select distinct vcData1 ,vcData3 from ConstMst where vcDataId = 'ProType') b ");
                sql.AppendLine("on a.vcPorType=b.vcData1 and a.vcZB=b.vcData3 where 1=1");

                if (ddlpro != " ")
                {
                    sql.AppendLine(" and vcPorType = @ddlpro");
                }
                if (ddlgroup != " ")
                {
                    sql.AppendLine(" and vcZB =@ddlgroup");
                }
                sql.AppendLine(" order by vcPorType ");

                DataTable dt = new DataTable();
                return excute.ExcuteSqlWithSelectToDT(sql.ToString(), paras);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable bindProType()
        {
            try
            {
                string ssql = "select ' ' as vcData1 union all select distinct vcData1 from dbo.ConstMst where vcDataID ='ProType'";
                DataTable dt = new DataTable();
                return excute.ExcuteSqlWithSelectToDT(ssql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable bindZB()
        {
            try
            {
                string ssql = " select ' ' as vcData3 union all select distinct vcData3 from dbo.ConstMst where vcDataID ='ProType'";
                DataTable dt = new DataTable();
                return excute.ExcuteSqlWithSelectToDT(ssql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable bindProType(string zb)
        {
            try
            {
                DataTable dt = new DataTable();
                StringBuilder ssql = new StringBuilder();
                if (zb == " ")
                {
                    ssql.AppendLine("select ' ' as vcData1 union all");
                }
                ssql.AppendLine(" select distinct vcData1 from dbo.ConstMst where vcDataID ='ProType'");
                if (zb != " ")
                {
                    ssql.AppendLine(" and vcData3 = '" + zb + "'");
                }
                return excute.ExcuteSqlWithSelectToDT(ssql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable bindZB(string Protype)
        {
            try
            {
                StringBuilder ssql = new StringBuilder();
                if (Protype == " ")
                {
                    ssql.AppendLine("select ' ' as vcData3 union all");
                }
                ssql.AppendLine(" select distinct vcData3 from dbo.ConstMst where vcDataID ='ProType'");
                if (Protype != " ")
                {
                    ssql.AppendLine(" and vcData1 = '" + Protype + "'");
                    ssql.AppendLine(" union all select distinct ' ' as vcData3 from dbo.ConstMst");
                }
                DataTable dt = new DataTable();
                return excute.ExcuteSqlWithSelectToDT(ssql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable bindState()
        {
            try
            {
                DataTable dt = new DataTable();
                string ssql = "";
                ssql += " select ' ' as state union";
                ssql += " select vcData1+'-'+vcData3 as state from ConstMst ";
                ssql += " where vcDataId='ProType' or vcDataId='CalendarType' ";
                ssql += " and vcData10 is null ";
                ssql += " union";
                ssql += " select AA+'-'+BB as state from ";
                ssql += " (SELECT distinct [vcData2] as AA FROM [ConstMst] where vcDataId='KBpartType' and vcData1 ='1') a";
                ssql += " CROSS JOIN ";
                ssql += " (SELECT distinct [vcData2] As BB FROM [ConstMst] where vcDataId='KBPlant') b";
                ssql += " order by state";
                return excute.ExcuteSqlWithSelectToDT(ssql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string Updatepro(DataTable dt, string user)
        {
            string msg = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = new SqlConnection(ComConnectionHelper.GetConnectionString());
                cmd.CommandTimeout = 0;
                cmd.Connection.Open();
                cmd.Transaction = cmd.Connection.BeginTransaction();
                SqlDataAdapter apt = new SqlDataAdapter(cmd);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["iflag"].ToString() == "1")//新增
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendFormat(" select * from  ProRuleMst where vcPorType='{0}' and vcZB='{1}'", dt.Rows[i]["vcPorType"].ToString(), dt.Rows[i]["vcZB"].ToString());
                        DataTable tmp = new DataTable();
                        cmd.CommandText = sb.ToString();
                        apt.Fill(tmp);
                        if (tmp.Rows.Count > 0)
                        {
                            msg = "\"" + dt.Rows[i]["vcPorType"].ToString() + "-" + dt.Rows[i]["vcZB"].ToString() + "\"存在相同部署和组别,更新失败！";
                            cmd.Transaction.Rollback();
                            cmd.Connection.Close();
                            return msg;
                        }
                        sb.Length = 0;
                        sb.AppendLine("  INSERT INTO ProRuleMst");
                        sb.AppendLine("             ([vcPorType],[vcZB],KBpartType,[vcProName0],[vcLT0],[vcCalendar0],[vcProName1],[vcLT1] ");
                        sb.AppendLine("  ,[vcCalendar1],[vcProName2],[vcLT2],[vcCalendar2],[vcProName3],[vcLT3],[vcCalendar3] ");
                        sb.AppendLine("  ,[vcProName4],[vcLT4],[vcCalendar4],[LogicType],[DADDTIME],[CUPDUSER]) ");
                        sb.AppendFormat("       VALUES('{0}' ", dt.Rows[i]["vcPorType"].ToString());
                        sb.AppendFormat("      , '{0}' ", dt.Rows[i]["vcZB"].ToString());
                        sb.AppendFormat("      , '{0}' ", dt.Rows[i]["KBpartType"].ToString());
                        sb.AppendFormat("      , '{0}' ", dt.Rows[i]["vcProName0"].ToString());
                        sb.AppendFormat("      , '{0}' ", dt.Rows[i]["vcLT0"].ToString());
                        sb.AppendFormat("      , '{0}' ", dt.Rows[i]["vcCalendar0"].ToString());
                        sb.AppendFormat("      , '{0}' ", dt.Rows[i]["vcProName1"].ToString());
                        sb.AppendFormat("      , '{0}' ", dt.Rows[i]["vcLT1"].ToString());
                        sb.AppendFormat("      , '{0}' ", dt.Rows[i]["vcCalendar1"].ToString());
                        sb.AppendFormat("      , '{0}' ", dt.Rows[i]["vcProName2"].ToString());
                        sb.AppendFormat("      , '{0}' ", dt.Rows[i]["vcLT2"].ToString());
                        sb.AppendFormat("      , '{0}' ", dt.Rows[i]["vcCalendar2"].ToString());
                        sb.AppendFormat("      , '{0}' ", dt.Rows[i]["vcProName3"].ToString());
                        sb.AppendFormat("      , '{0}' ", dt.Rows[i]["vcLT3"].ToString());
                        sb.AppendFormat("      , '{0}' ", dt.Rows[i]["vcCalendar3"].ToString());
                        sb.AppendFormat("      , '{0}' ", dt.Rows[i]["vcProName4"].ToString());
                        sb.AppendFormat("      , '{0}' ", dt.Rows[i]["vcLT4"].ToString());
                        sb.AppendFormat("      , '{0}' ", dt.Rows[i]["vcCalendar4"].ToString());
                        sb.AppendFormat("      , '{0}' ", dt.Rows[i]["LogicType"].ToString());
                        sb.AppendFormat("             ,getdate() ");
                        sb.AppendFormat("             ,'{0}') ", user);
                        cmd.CommandText = sb.ToString();
                        cmd.ExecuteNonQuery();
                    }
                    if (dt.Rows[i]["iflag"].ToString() == "2")//修改
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine(" UPDATE ProRuleMst");
                        sb.AppendFormat("    SET[KBpartType] = '{0}' ", dt.Rows[i]["KBpartType"].ToString());
                        sb.AppendFormat("       ,[vcProName0] = '{0}' ", dt.Rows[i]["vcProName0"].ToString());
                        sb.AppendFormat("       ,[vcLT0] = '{0}' ", dt.Rows[i]["vcLT0"].ToString());
                        sb.AppendFormat("       ,[vcCalendar0] = '{0}' ", dt.Rows[i]["vcCalendar0"].ToString());
                        sb.AppendFormat("       ,[vcProName1] = '{0}' ", dt.Rows[i]["vcProName1"].ToString());
                        sb.AppendFormat("       ,[vcLT1] = '{0}' ", dt.Rows[i]["vcLT1"].ToString());
                        sb.AppendFormat("       ,[vcCalendar1] = '{0}' ", dt.Rows[i]["vcCalendar1"].ToString());
                        sb.AppendFormat("       ,[vcProName2] = '{0}' ", dt.Rows[i]["vcProName2"].ToString());
                        sb.AppendFormat("       ,[vcLT2] = '{0}' ", dt.Rows[i]["vcLT2"].ToString());
                        sb.AppendFormat("       ,[vcCalendar2] = '{0}' ", dt.Rows[i]["vcCalendar2"].ToString());
                        sb.AppendFormat("       ,[vcProName3] = '{0}' ", dt.Rows[i]["vcProName3"].ToString());
                        sb.AppendFormat("       ,[vcLT3] = '{0}' ", dt.Rows[i]["vcLT3"].ToString());
                        sb.AppendFormat("       ,[vcCalendar3] = '{0}' ", dt.Rows[i]["vcCalendar3"].ToString());
                        sb.AppendFormat("       ,[vcProName4] = '{0}' ", dt.Rows[i]["vcProName4"].ToString());
                        sb.AppendFormat("       ,[vcLT4] = '{0}' ", dt.Rows[i]["vcLT4"].ToString());
                        sb.AppendFormat("       ,[vcCalendar4] = '{0}' ", dt.Rows[i]["vcCalendar4"].ToString());
                        sb.AppendFormat("       ,[LogicType] = '{0}' ", dt.Rows[i]["LogicType"].ToString());
                        sb.AppendLine("       ,[DUPDTIME] = getdate() ");
                        sb.AppendFormat("       ,[CUPDUSER] = '{0}' ", user);
                        sb.AppendFormat("  WHERE [vcPorType] ='{0}' and [vcZB]='{1}' ", dt.Rows[i]["vcPorType"].ToString(), dt.Rows[i]["vcZB"].ToString());
                        cmd.CommandText = sb.ToString();
                        cmd.ExecuteNonQuery();

                    }
                    if (dt.Rows[i]["iflag"].ToString() == "3")//删除
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine(" DELETE FROM ProRuleMst ");
                        sb.AppendFormat("  WHERE [vcPorType] ='{0}' and [vcZB]='{1}' ", dt.Rows[i]["vcPorType"].ToString(), dt.Rows[i]["vcZB"].ToString());
                        cmd.CommandText = sb.ToString();
                        cmd.ExecuteNonQuery();
                    }
                }
                cmd.Transaction.Commit();
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                if (cmd.Connection.State == ConnectionState.Open)
                {
                    cmd.Transaction.Rollback();
                    cmd.Connection.Close();
                }
                msg = ex.ToString();
            }
            return msg;
        }
        public string UpdateTable(DataTable dt, string user)
        {
            string msg = "";
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = new SqlConnection(ComConnectionHelper.GetConnectionString());
                cmd.CommandTimeout = 0;
                cmd.Connection.Open();
                cmd.Transaction = cmd.Connection.BeginTransaction();
                SqlDataAdapter apt = new SqlDataAdapter(cmd);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["iflag"].ToString() == "1")//新增
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendFormat(" select * from  ProRuleMst where vcPorType='{0}' and vcZB='{1}'", dt.Rows[i]["vcPorType"].ToString(), dt.Rows[i]["vcZB"].ToString());
                        DataTable tmp = new DataTable();
                        cmd.CommandText = sb.ToString();
                        apt.Fill(tmp);
                        if (tmp.Rows.Count > 0)
                        {
                            msg = "\"" + dt.Rows[i]["vcPorType"].ToString() + "-" + dt.Rows[i]["vcZB"].ToString() + "\"存在相同部署和组别,更新失败！";
                            cmd.Transaction.Rollback();
                            cmd.Connection.Close();
                            return msg;
                        }
                        sb.Length = 0;
                        sb.AppendLine("  INSERT INTO ProRuleMst");
                        sb.AppendLine("             ([vcPorType],[vcZB],KBpartType,[vcProName0],[vcLT0],[vcCalendar0],[vcProName1],[vcLT1] ");
                        sb.AppendLine("  ,[vcCalendar1],[vcProName2],[vcLT2],[vcCalendar2],[vcProName3],[vcLT3],[vcCalendar3] ");
                        sb.AppendLine("  ,[vcProName4],[vcLT4],[vcCalendar4],[LogicType],[DADDTIME],[CUPDUSER]) ");
                        sb.AppendFormat("       VALUES('{0}' ", dt.Rows[i]["vcPorType"].ToString());
                        sb.AppendFormat("      , '{0}' ", dt.Rows[i]["vcZB"].ToString());
                        sb.AppendFormat("      , '{0}' ", dt.Rows[i]["KBpartType"].ToString());
                        sb.AppendFormat("      , '{0}' ", dt.Rows[i]["vcProName0"].ToString());
                        sb.AppendFormat("      , '{0}' ", dt.Rows[i]["vcLT0"].ToString());
                        sb.AppendFormat("      , '{0}' ", dt.Rows[i]["vcCalendar0"].ToString());
                        sb.AppendFormat("      , '{0}' ", dt.Rows[i]["vcProName1"].ToString());
                        sb.AppendFormat("      , '{0}' ", dt.Rows[i]["vcLT1"].ToString());
                        sb.AppendFormat("      , '{0}' ", dt.Rows[i]["vcCalendar1"].ToString());
                        sb.AppendFormat("      , '{0}' ", dt.Rows[i]["vcProName2"].ToString());
                        sb.AppendFormat("      , '{0}' ", dt.Rows[i]["vcLT2"].ToString());
                        sb.AppendFormat("      , '{0}' ", dt.Rows[i]["vcCalendar2"].ToString());
                        sb.AppendFormat("      , '{0}' ", dt.Rows[i]["vcProName3"].ToString());
                        sb.AppendFormat("      , '{0}' ", dt.Rows[i]["vcLT3"].ToString());
                        sb.AppendFormat("      , '{0}' ", dt.Rows[i]["vcCalendar3"].ToString());
                        sb.AppendFormat("      , '{0}' ", dt.Rows[i]["vcProName4"].ToString());
                        sb.AppendFormat("      , '{0}' ", dt.Rows[i]["vcLT4"].ToString());
                        sb.AppendFormat("      , '{0}' ", dt.Rows[i]["vcCalendar4"].ToString());
                        sb.AppendFormat("      , '{0}' ", dt.Rows[i]["LogicType"].ToString());
                        sb.AppendFormat("             ,getdate() ");
                        sb.AppendFormat("             ,'{0}') ", user);
                        cmd.CommandText = sb.ToString();
                        cmd.ExecuteNonQuery();
                    }
                    if (dt.Rows[i]["iflag"].ToString() == "2")//修改
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine(" UPDATE ProRuleMst");
                        sb.AppendFormat("    SET[KBpartType] = '{0}' ", dt.Rows[i]["KBpartType"].ToString());
                        sb.AppendFormat("       ,[vcProName0] = '{0}' ", dt.Rows[i]["vcProName0"].ToString());
                        sb.AppendFormat("       ,[vcLT0] = '{0}' ", dt.Rows[i]["vcLT0"].ToString());
                        sb.AppendFormat("       ,[vcCalendar0] = '{0}' ", dt.Rows[i]["vcCalendar0"].ToString());
                        sb.AppendFormat("       ,[vcProName1] = '{0}' ", dt.Rows[i]["vcProName1"].ToString());
                        sb.AppendFormat("       ,[vcLT1] = '{0}' ", dt.Rows[i]["vcLT1"].ToString());
                        sb.AppendFormat("       ,[vcCalendar1] = '{0}' ", dt.Rows[i]["vcCalendar1"].ToString());
                        sb.AppendFormat("       ,[vcProName2] = '{0}' ", dt.Rows[i]["vcProName2"].ToString());
                        sb.AppendFormat("       ,[vcLT2] = '{0}' ", dt.Rows[i]["vcLT2"].ToString());
                        sb.AppendFormat("       ,[vcCalendar2] = '{0}' ", dt.Rows[i]["vcCalendar2"].ToString());
                        sb.AppendFormat("       ,[vcProName3] = '{0}' ", dt.Rows[i]["vcProName3"].ToString());
                        sb.AppendFormat("       ,[vcLT3] = '{0}' ", dt.Rows[i]["vcLT3"].ToString());
                        sb.AppendFormat("       ,[vcCalendar3] = '{0}' ", dt.Rows[i]["vcCalendar3"].ToString());
                        sb.AppendFormat("       ,[vcProName4] = '{0}' ", dt.Rows[i]["vcProName4"].ToString());
                        sb.AppendFormat("       ,[vcLT4] = '{0}' ", dt.Rows[i]["vcLT4"].ToString());
                        sb.AppendFormat("       ,[vcCalendar4] = '{0}' ", dt.Rows[i]["vcCalendar4"].ToString());
                        sb.AppendFormat("       ,[LogicType] = '{0}' ", dt.Rows[i]["LogicType"].ToString());
                        sb.AppendLine("       ,[DUPDTIME] = getdate() ");
                        sb.AppendFormat("       ,[CUPDUSER] = '{0}' ", user);
                        sb.AppendFormat("  WHERE [vcPorType] ='{0}' and [vcZB]='{1}' ", dt.Rows[i]["vcPorType"].ToString(), dt.Rows[i]["vcZB"].ToString());
                        cmd.CommandText = sb.ToString();
                        cmd.ExecuteNonQuery();

                    }
                    if (dt.Rows[i]["iflag"].ToString() == "3")//删除
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine(" DELETE FROM ProRuleMst ");
                        sb.AppendFormat("  WHERE [vcPorType] ='{0}' and [vcZB]='{1}' ", dt.Rows[i]["vcPorType"].ToString(), dt.Rows[i]["vcZB"].ToString());
                        cmd.CommandText = sb.ToString();
                        cmd.ExecuteNonQuery();
                    }
                }
                cmd.Transaction.Commit();
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                if (cmd.Connection.State == ConnectionState.Open)
                {
                    cmd.Transaction.Rollback();
                    cmd.Connection.Close();
                }
                msg = ex.ToString();
            }
            return msg;

        }
        public DataTable getsz()
        {
            DataTable dt = new DataTable();
            string ssql = "";
            ssql += " select vcData1,vcData3 from ConstMst ";
            ssql += " where vcDataId='ProType' ";
            try
            {
                return excute.ExcuteSqlWithSelectToDT(ssql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable getPartType()
        {
            DataTable dt = new DataTable();
            string ssql = "";
            ssql += " select distinct vcData1,vcData2 from ConstMst ";
            ssql += " where vcDataId='KBpartType' ";
            try
            {
                return excute.ExcuteSqlWithSelectToDT(ssql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
