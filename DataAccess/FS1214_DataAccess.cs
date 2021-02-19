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
            string ssql = "select top(1) vcDataId, vcDataName from ConstMst where vcDataName='" + value + "'";
            DataTable dt = excute.ExcuteSqlWithSelectToDT(ssql);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["vcDataId"].ToString();
            }
            return "0";
        }

        public DataTable GetDataName()
        {
            string ssql = "select '' as vcValue, '' as vcName union all select distinct vcDataId, vcDataName from ConstMst";
            return excute.ExcuteSqlWithSelectToDT(ssql);
        }

        public DataTable GetSearchAll(string strDataId, string data1, string data2, string data3)
        {
            string ssql = " select vcDataId,vcDataName,vcData1,vcData2,vcData3,vcData4,vcData5,vcData6,vcData7,vcData8,vcData9,vcData10,'0'as iflag,'0' as vcModFlag,'0' as vcAddFlag,iAutoId ";
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
            if (strDataId == "")
            {
                ssql = "";
                ssql += " select * from (select TOP 100 PERCENT vcDataId,vcDataName,vcData1,vcData2,vcData3,vcData4,vcData5,vcData6,vcData7,vcData8,vcData9,vcData10,'0'as iflag,'0' as vcModFlag,'0' as vcAddFlag,iAutoId ";
                ssql += " from ConstMst";
                ssql += " where (vcDataName = '厂家联系人' or vcDataName = '车种' ) ";
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
                ssql += " select * from (select TOP 100 PERCENT vcDataId,vcDataName,vcData1,vcData2,vcData3,vcData4,vcData5,vcData6,vcData7,vcData8,vcData9,vcData10,'0'as iflag,'0' as vcModFlag,'0' as vcAddFlag,iAutoId ";
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
            else if (strDataId == "FactoryUser")
            {
                ssql += " and vcDataId = '" + strDataId + "'";
                ssql += " order by vcData3";
            }
            else if (strDataId == "CarType")
            {
                ssql += " and vcDataId = '" + strDataId + "'";
                ssql += " order by vcData3";
            }
            else
            {
                ssql += " and vcDataId = '" + strDataId + "'";
                ssql += " order by vcData1";
            }
            try
            {
                DataTable dt = excute.ExcuteSqlWithSelectToDT(ssql);
                return dt;
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
                        sql.Append("insert into ConstMst([vcDataName],[vcDataId],[vcData1],[vcData2],[vcData3],[vcData4],[vcData5],[vcData6],  \r\n");
                        sql.Append("[vcData7],[vcData8],[vcData9],[vcData10],[dCreateTime],[vcCreateUserId])  \r\n");
                        sql.Append(" values (  \r\n");  
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcDataName"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcDataId"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcData1"], true) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcData2"], true) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcData3"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcData4"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcData5"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcData6"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcData7"], true) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcData8"], true) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcData9"], false) + ",  \r\n");
                        sql.Append(ComFunction.getSqlValue(listInfoData[i]["vcData10"], false) + ",  \r\n");
                        sql.Append("getdate(),  \r\n"); 
                        sql.Append("'" + strUserId + "'  \r\n");
                        sql.Append(" );  \r\n");
                    }
                    else if (bAddFlag == false && bModFlag == true)
                    {//修改
                        int iAutoId = Convert.ToInt32(listInfoData[i]["iAutoId"]);
                        sql.Append("  update ConstMst set    \r\n");
                        sql.Append("  vcDataId=" + ComFunction.getSqlValue(listInfoData[i]["vcDataId"], false) + "   \r\n");
                        sql.Append("  ,vcDataName=" + ComFunction.getSqlValue(listInfoData[i]["vcDataName"], false) + "   \r\n");
                        sql.Append("  ,vcData1=" + ComFunction.getSqlValue(listInfoData[i]["vcData1"], true) + "   \r\n");
                        sql.Append("  ,vcData2=" + ComFunction.getSqlValue(listInfoData[i]["vcData2"], true) + "   \r\n");
                        sql.Append("  ,vcData3=" + ComFunction.getSqlValue(listInfoData[i]["vcData3"], true) + "   \r\n");
                        sql.Append("  ,vcData4=" + ComFunction.getSqlValue(listInfoData[i]["vcData4"], true) + "   \r\n");
                        sql.Append("  ,vcData5=" + ComFunction.getSqlValue(listInfoData[i]["vcData5"], true) + "   \r\n");
                        sql.Append("  ,vcData6=" + ComFunction.getSqlValue(listInfoData[i]["vcData6"], true) + "   \r\n");
                        sql.Append("  ,vcData7=" + ComFunction.getSqlValue(listInfoData[i]["vcData7"], true) + "   \r\n");
                        sql.Append("  ,vcData8=" + ComFunction.getSqlValue(listInfoData[i]["vcData8"], true) + "   \r\n");
                        sql.Append("  ,vcData9=" + ComFunction.getSqlValue(listInfoData[i]["vcData9"], true) + "   \r\n");
                        sql.Append("  ,vcData10=" + ComFunction.getSqlValue(listInfoData[i]["vcData10"], true) + "   \r\n");
                        sql.Append("  ,vcUpdateUserId='" + strUserId + "'  \r\n");
                        sql.Append("  ,dUpdateTime=getdate()   \r\n");
                        sql.Append("  where iAutoId=" + iAutoId + "  ; \r\n");
                    }
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
        public void Del(List<Dictionary<string, Object>> listInfoData, string strUserId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("DELETE FROM ConstMst where iAutoId in(   \r\n ");
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
    }
}
