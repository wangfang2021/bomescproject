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
    public class FS1214_DataAccess
    {
        private MultiExcute excute = new MultiExcute();

        public string SFXchanged(string value)
        {
            if (value == "") return "0";
            string ssql = "select top(1) vcDataId, vcDataName from ConstMst where vcDataName='"+value+"'";
            DataTable dt = excute.ExcuteSqlWithSelectToDT(ssql);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["vcDataId"].ToString();
            }
            return "0";
        }

        public DataTable GetDataName()
        {
            string ssql = "select ' ' as vcDataName union all select distinct vcDataName from ConstMst";
            return excute.ExcuteSqlWithSelectToDT(ssql);
        }

        public DataTable GetSearchAll(string strDataName, string data1, string data2, string data3)
        {
            string ssql = " select vcDataId,vcDataName,vcData1,vcData2,vcData3,vcData4,vcData5,vcData6,vcData7,vcData8,vcData9,vcData10,'0'as iflag,iAutoId ";
            ssql += " from ConstMst";
            ssql += " where 1=1   ";

            if (data1.Trim() != "")
            {
                ssql += " and  vcData1='" + data1 + "'   ";
            }
            if (data2.Trim() != "")
            {

                ssql += " and vcData2='" + data2 + "'   ";
            }
            if (data3.Trim() != "")
            {
                ssql += " and  vcData3='" + data3 + "'   ";
            }
            if (strDataName == " ")
            {
                ssql = "";
                ssql += " select * from (select TOP 100 PERCENT vcDataId,vcDataName,vcData1,vcData2,vcData3,vcData4,vcData5,vcData6,vcData7,vcData8,vcData9,vcData10,'0'as iflag,iAutoId ";
                ssql += " from ConstMst";
                ssql += " where ( vcDataName = '厂家联系人' or vcDataName = '车种' ) ";

                if (data1.Trim() != "")
                {
                    ssql += " and  vcData1='" + data1 + "'   ";
                }
                if (data2.Trim() != "")
                {

                    ssql += " and vcData2='" + data2 + "'   ";
                }
                if (data3.Trim() != "")
                {
                    ssql += " and  vcData3='" + data3 + "'   ";
                }

                ssql += " order by vcDataName,vcData3) as table1";
                ssql += " union all";
                ssql += " select * from (select TOP 100 PERCENT vcDataId,vcDataName,vcData1,vcData2,vcData3,vcData4,vcData5,vcData6,vcData7,vcData8,vcData9,vcData10,'0'as iflag,iAutoId ";
                ssql += " from ConstMst ";
                ssql += " where ( vcDataName != '厂家联系人' and vcDataName != '车种' )";

                if (data1.Trim() != "")
                {
                    ssql += " and  vcData1='" + data1 + "'   ";
                }
                if (data2.Trim() != "")
                {

                    ssql += " and vcData2='" + data2 + "'   ";
                }
                if (data3.Trim() != "")
                {
                    ssql += " and  vcData3='" + data3 + "'   ";
                }

                ssql += " order by vcDataName,vcData1) as table2";
            }
            else if (strDataName == "厂家联系人")
            {
                ssql += " and vcDataName = '" + strDataName + "'";
                ssql += " order by vcData3";
            }
            else if (strDataName == "车种")
            {
                ssql += " and vcDataName = '" + strDataName + "'";
                ssql += " order by vcData3";
            }
            else
            {
                ssql += " and vcDataName = '" + strDataName + "'";
                ssql += " order by vcData1";
            }
            DataTable dt = excute.ExcuteSqlWithSelectToDT(ssql);
            return dt;
        }

        public DataTable SearchDataName()
        {
            string ssql = " select ' ' as vcDataName union all select distinct vcDataName from ConstMst ";
            return excute.ExcuteSqlWithSelectToDT(ssql);
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
                    if (dt.Rows[i]["iflag"].ToString() == "1" || dt.Rows[i]["iflag"].ToString() == "3")//新增&复制
                    {
                        StringBuilder sb = new StringBuilder();
                        //sb.AppendFormat(" select * from  ConstMst where vcDataName='{0}' ", dt.Rows[i]["vcDataName"].ToString() );
                        //DataTable tmp = new DataTable();
                        //apt.Fill(tmp);
                        //if (tmp.Rows.Count > 0)
                        //{
                        //    msg = "\"" + dt.Rows[i]["vcPorType"].ToString() + "-" + dt.Rows[i]["vcZB"].ToString() + "\"存在相同部署和组别,更新失败！";
                        //    cmd.Transaction.Rollback();
                        //    cmd.Connection.Close();
                        //    return msg;
                        //}
                        sb.Length = 0;
                        sb.AppendLine("  insert into ConstMst ");
                        sb.AppendLine("             ([vcDataId],[vcDataName],[vcData1],[vcData2],[vcData3],[vcData4] ");
                        sb.AppendLine("  ,[vcData5],[vcData6],[vcData7],[vcData8],[vcData9] ");
                        sb.AppendLine("  ,[vcData10],[dCreateTime],[vcCreateUserId]) ");
                        sb.AppendFormat("       VALUES('{0}' ", dt.Rows[i]["vcDataId"].ToString());
                        sb.AppendFormat("              ,'{0}' ", dt.Rows[i]["vcDataName"].ToString());
                        sb.AppendFormat("              ,'{0}' ", dt.Rows[i]["vcData1"].ToString());
                        sb.AppendFormat("              ,'{0}' ", dt.Rows[i]["vcData2"].ToString());
                        sb.AppendFormat("              ,'{0}' ", dt.Rows[i]["vcData3"].ToString());
                        sb.AppendFormat("              ,'{0}' ", dt.Rows[i]["vcData4"].ToString());
                        sb.AppendFormat("              ,'{0}' ", dt.Rows[i]["vcData5"].ToString());
                        sb.AppendFormat("              ,'{0}' ", dt.Rows[i]["vcData6"].ToString());
                        sb.AppendFormat("              ,'{0}' ", dt.Rows[i]["vcData7"].ToString());
                        sb.AppendFormat("              ,'{0}' ", dt.Rows[i]["vcData8"].ToString());
                        sb.AppendFormat("              ,'{0}' ", dt.Rows[i]["vcData9"].ToString());
                        sb.AppendFormat("              ,'{0}' ", dt.Rows[i]["vcData10"].ToString());
                        sb.AppendFormat("             ,getdate() ");
                        sb.AppendFormat("             ,'{0}') ", user);
                        cmd.CommandText = sb.ToString();
                        cmd.ExecuteNonQuery();
                    }
                    if (dt.Rows[i]["iflag"].ToString() == "2")//修改
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine(" UPDATE ConstMst");
                        sb.AppendFormat("    SET[vcDataId] = '{0}' ", dt.Rows[i]["vcDataId"].ToString());
                        sb.AppendFormat("       ,[vcDataName] = '{0}' ", dt.Rows[i]["vcDataName"].ToString());
                        sb.AppendFormat("       ,[vcData1] = '{0}' ", dt.Rows[i]["vcData1"].ToString());
                        sb.AppendFormat("       ,[vcData2] = '{0}' ", dt.Rows[i]["vcData2"].ToString());
                        sb.AppendFormat("       ,[vcData3] = '{0}' ", dt.Rows[i]["vcData3"].ToString());
                        sb.AppendFormat("       ,[vcData4] = '{0}' ", dt.Rows[i]["vcData4"].ToString());
                        sb.AppendFormat("       ,[vcData5] = '{0}' ", dt.Rows[i]["vcData5"].ToString());
                        sb.AppendFormat("       ,[vcData6] = '{0}' ", dt.Rows[i]["vcData6"].ToString());
                        sb.AppendFormat("       ,[vcData7] = '{0}' ", dt.Rows[i]["vcData7"].ToString());
                        sb.AppendFormat("       ,[vcData8] = '{0}' ", dt.Rows[i]["vcData8"].ToString());
                        sb.AppendFormat("       ,[vcData9] = '{0}' ", dt.Rows[i]["vcData9"].ToString());
                        sb.AppendFormat("       ,[vcData10] = '{0}' ", dt.Rows[i]["vcData10"].ToString());
                        sb.AppendLine("       ,[dCreateTime] = getdate() ");
                        sb.AppendFormat("       ,[vcCreateUserId] = '{0}' ", user);
                        sb.AppendFormat("  WHERE [iAutoId] ='{0}' ", dt.Rows[i]["iAutoId"].ToString());
                        cmd.CommandText = sb.ToString();
                        cmd.ExecuteNonQuery();
                    }
                    if (dt.Rows[i]["iflag"].ToString() == "4")//删除
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine(" DELETE FROM ConstMst ");
                        sb.AppendFormat("  WHERE [iAutoId] ='{0}'  ", dt.Rows[i]["iAutoId"].ToString());
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
    }
}
