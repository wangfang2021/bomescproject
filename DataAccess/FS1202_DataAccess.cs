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
                sql.AppendLine(" select a.vcPorType, a.vcZB, a.KBpartType, a.LogicType, a.vcProName0, a.vcLT0, ");
                sql.AppendLine("a.vcCalendar0, a.vcProName1, a.vcLT1, a.vcCalendar1, a.vcProName2, a.vcLT2, a.vcCalendar2, a.vcProName3,");
                sql.AppendLine(" a.vcLT3, a.vcCalendar3, a.vcProName4, a.vcLT4, a.vcCalendar4 ");
                sql.AppendLine(",'0' as vcModFlag,'0' as vcAddFlag, iAutoId from ProRuleMst as a");
                sql.AppendLine(" left join (select distinct vcData1 ,vcData3 from ConstMst where vcDataId='ProType') b ");
                sql.AppendLine("on a.vcPorType=b.vcData1 and a.vcZB=b.vcData3 where 1=1");

                if (ddlpro != "")
                {
                    sql.AppendLine(" and vcPorType=@ddlpro");
                }
                if (ddlgroup != "")
                {
                    sql.AppendLine(" and vcZB=@ddlgroup");
                }
                sql.AppendLine(" order by vcPorType ");

                //DataTable dt = new DataTable();
                return excute.ExcuteSqlWithSelectToDT(sql.ToString(), paras);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region 删除
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("  delete ProRuleMst where iAutoId in(   \r\n ");
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
        #endregion

        #region 保存
        public void Save(List<Dictionary<string, Object>> listInfoData, string strUserId, ref string strErrorPartId)
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
                        sql.Append("  INSERT INTO ProRuleMst(vcPorType,vcZB,KBpartType,vcProName0,vcLT0,vcCalendar0,vcProName1,vcLT1");
                        sql.Append("  ,vcCalendar1,vcProName2,vcLT2,vcCalendar2,vcProName3,vcLT3,vcCalendar3   \r\n");
                        sql.Append("  ,vcProName4,vcLT4,vcCalendar4,LogicType,DADDTIME,CUPDUSER)   \r\n");
                        sql.Append(" values (  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcPorType"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcZB"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["KBpartType"], true) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcProName0"], true) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcLT0"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcCalendar0"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcProName1"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcLT1"], true) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcCalendar1"], true) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcProName2"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcLT2"], true) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcCalendar2"], true) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcProName3"], true) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcLT3"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcCalendar3"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcProName4"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcLT4"], true) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcCalendar4"], true) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["LogicType"], true) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["DADDTIME"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["CUPDUSER"], false) + "  \r\n");
                        sql.Append(" );  \r\n");
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                        sql.Append("  update ProRuleMst set    \r\n");
                        sql.Append("  KBpartType=" + ComFunction.getSqlValue(listInfoData[i]["KBpartType"], false) + "   \r\n");
                        sql.Append("  ,vcProName0=" + ComFunction.getSqlValue(listInfoData[i]["vcProName0"], false) + "   \r\n");
                        sql.Append("  ,vcLT0=" + ComFunction.getSqlValue(listInfoData[i]["vcLT0"], true) + "   \r\n");
                        sql.Append("  ,vcCalendar0=" + ComFunction.getSqlValue(listInfoData[i]["vcCalendar0"], true) + "   \r\n");
                        sql.Append("  ,vcProName1=" + ComFunction.getSqlValue(listInfoData[i]["vcProName1"], false) + "   \r\n");
                        sql.Append("  ,vcLT1=" + ComFunction.getSqlValue(listInfoData[i]["vcLT1"], true) + "   \r\n");
                        sql.Append("  ,vcCalendar1=" + ComFunction.getSqlValue(listInfoData[i]["vcCalendar1"], true) + "   \r\n");
                        sql.Append("  ,vcProName2=" + ComFunction.getSqlValue(listInfoData[i]["vcProName2"], false) + "   \r\n");
                        sql.Append("  ,vcLT2=" + ComFunction.getSqlValue(listInfoData[i]["vcLT2"], true) + "   \r\n");
                        sql.Append("  ,vcCalendar2=" + ComFunction.getSqlValue(listInfoData[i]["vcCalendar2"], true) + "   \r\n");
                        sql.Append("  ,vcProName3=" + ComFunction.getSqlValue(listInfoData[i]["vcProName3"], false) + "   \r\n");
                        sql.Append("  ,vcLT3=" + ComFunction.getSqlValue(listInfoData[i]["vcLT3"], true) + "   \r\n");
                        sql.Append("  ,vcCalendar3=" + ComFunction.getSqlValue(listInfoData[i]["vcCalendar3"], true) + "   \r\n");
                        sql.Append("  ,vcProName4=" + ComFunction.getSqlValue(listInfoData[i]["vcProName4"], false) + "   \r\n");
                        sql.Append("  ,vcLT4=" + ComFunction.getSqlValue(listInfoData[i]["vcLT4"], true) + "   \r\n");
                        sql.Append("  ,vcCalendar4=" + ComFunction.getSqlValue(listInfoData[i]["vcCalendar4"], true) + "   \r\n");
                        sql.Append("  ,LogicType=" + ComFunction.getSqlValue(listInfoData[i]["LogicType"], false) + "   \r\n");
                        sql.Append("  ,DUPDTIME=getdate()   \r\n");
                        sql.Append("  ,CUPDUSER='" + strUserId + "'   \r\n");
                        sql.Append("  where iAutoId=" + iAutoId + "  ; \r\n");

                    }
                }
                excute.ExcuteSqlWithStringOper(sql.ToString());
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


        public DataTable bindProType()
        {
            try
            {
                string ssql = "select '' as vcValue,'' as vcName union all select distinct vcData1 as vcValue,vcData1 as vcName from ConstMst where vcDataID='ProType'";
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
                string ssql = " select distinct vcData3 as vcValue,vcData3 as vcName from ConstMst where vcDataID='ProType'";
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
                if (zb == "")
                {
                    ssql.AppendLine("select '' as vcValue,'' as vcName union all ");
                }
                ssql.AppendLine(" select distinct vcData1 as vcValue,vcData1 as vcName from ConstMst where vcDataID='ProType'");
                if (zb != "")
                {
                    ssql.AppendLine(" and vcData3='" + zb + "'");
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
                if (Protype == "")
                {
                    ssql.AppendLine("select '' as vcValue,'' as vcName union all");
                }
                ssql.AppendLine(" select distinct vcData3 as vcValue,vcData3 as vcName from ConstMst where vcDataID='ProType'");
                if (Protype != "")
                {
                    ssql.AppendLine(" and vcData1='" + Protype + "'");
                    ssql.AppendLine(" union all select distinct '' as vcValue,'' as vcName from ConstMst");
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
                ssql += " select '' as vcValue, '' as vcName ";
                ssql += " union ";
                ssql += " select vcData1+'-'+vcData3, vcData1+'-'+vcData3 from ConstMst where vcDataId='ProType' or vcDataId='CalendarType' and vcData10 is null";
                ssql += " union";
                ssql += " select AA+'-'+BB as vcValue, AA+'-'+BB as vcName from ";
                ssql += " (SELECT distinct [vcData2] as AA FROM [ConstMst] where vcDataId='KBpartType' and vcData1='1') a";
                ssql += " CROSS JOIN ";
                ssql += " (SELECT distinct [vcData2] as BB FROM [ConstMst] where vcDataId='KBPlant') b";
                ssql += " order by vcValue";
                return excute.ExcuteSqlWithSelectToDT(ssql.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string Updatepro(DataTable dt, string user)
        {
            SqlCommand cmd = new SqlCommand();
            try
            {
                DataTable dttmp = new DataTable();
                cmd.Connection = new SqlConnection(ComConnectionHelper.GetConnectionString());
                cmd.CommandTimeout = 0;
                cmd.Connection.Open();
                //   cmd.Connection.BeginTransaction();
                cmd.CommandText = "select vcPorType, vcZB, KBpartType, vcProName0, vcLT0, vcCalendar0, vcProName1, vcLT1, vcCalendar1, vcProName2, vcLT2, vcCalendar2, vcProName3, vcLT3, vcCalendar3, vcProName4, vcLT4, vcCalendar4, LogicType, DADDTIME, DUPDTIME, CUPDUSER from ProRuleMst";
                SqlDataAdapter apt = new SqlDataAdapter(cmd);
                apt.Fill(dttmp);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i][0].ToString() == "")
                        break;

                    if (dt.Rows[i][2].ToString() == "指定")
                    {
                        dt.Rows[i][2] = "0";
                    }
                    else if (dt.Rows[i][2].ToString() == "非指定")
                    {
                        dt.Rows[i][2] = "1";
                    }
                    else
                    {
                        dt.Rows[i][2] = "";
                    }
                    DataRow[] dr = dttmp.Select("vcPorType ='" + dt.Rows[i][0].ToString() + "' and vcZB ='" + dt.Rows[i][1].ToString() + "'");
                    if (dr.Length > 0)
                    {
                        dr[0]["KBpartType"] = dt.Rows[i][2].ToString();
                        dr[0]["vcProName0"] = dt.Rows[i][3].ToString();
                        dr[0]["vcLT0"] = dt.Rows[i][4].ToString();
                        dr[0]["vcCalendar0"] = dt.Rows[i][5].ToString().ToUpper();

                        dr[0]["vcProName1"] = dt.Rows[i][6].ToString();
                        dr[0]["vcLT1"] = dt.Rows[i][7].ToString();
                        dr[0]["vcCalendar1"] = dt.Rows[i][8].ToString().ToUpper();

                        dr[0]["vcProName2"] = dt.Rows[i][9].ToString();
                        dr[0]["vcLT2"] = dt.Rows[i][10].ToString();
                        dr[0]["vcCalendar2"] = dt.Rows[i][11].ToString().ToUpper();

                        dr[0]["vcProName3"] = dt.Rows[i][12].ToString();
                        dr[0]["vcLT3"] = dt.Rows[i][13].ToString();
                        dr[0]["vcCalendar3"] = dt.Rows[i][14].ToString().ToUpper();

                        dr[0]["vcProName4"] = dt.Rows[i][15].ToString();
                        dr[0]["vcLT4"] = dt.Rows[i][16].ToString();
                        dr[0]["vcCalendar4"] = dt.Rows[i][17].ToString().ToUpper();

                        // dr[0]["LogicType"] = dt.Rows[i][2].ToString();
                        dr[0]["DUPDTIME"] = DateTime.Now.ToString();
                        dr[0]["CUPDUSER"] = user;
                    }
                    else
                    {
                        dttmp.Rows.Add(dt.Rows[i][0].ToString().ToUpper(),
                                        dt.Rows[i][1].ToString(),
                                        dt.Rows[i][2].ToString().ToUpper(),
                                        dt.Rows[i][3].ToString(),
                                        dt.Rows[i][4].ToString(),
                                        dt.Rows[i][5].ToString().ToUpper(),
                                        dt.Rows[i][6].ToString(),
                                        dt.Rows[i][7].ToString(),
                                        dt.Rows[i][8].ToString().ToUpper(),
                                        dt.Rows[i][9].ToString(),
                                        dt.Rows[i][10].ToString(),
                                        dt.Rows[i][11].ToString().ToUpper(),
                                        dt.Rows[i][12].ToString(),
                                        dt.Rows[i][13].ToString(),
                                        dt.Rows[i][14].ToString().ToUpper(),
                                        dt.Rows[i][15].ToString(),
                                        dt.Rows[i][16].ToString(),
                                        dt.Rows[i][17].ToString().ToUpper(),
                                        null,
                                        DateTime.Now,
                                        DateTime.Now,
                                        user);
                    }
                }
                SqlCommandBuilder cmdbuild = new SqlCommandBuilder(apt);
                cmd.Dispose();
                apt.Update(dttmp);
                return "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
            string ssql = " select distinct vcData1 as vcValue, vcData2 as vcName from ConstMst where vcDataId='KBpartType'";
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
