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
    public class FS1306_DataAccess
    {
        private MultiExcute excute = new MultiExcute();
        public DataTable getDataInfo(string strPackingPlant, string strHosDate, string strBanZhi, string strBZFromTime, ref DataTable dtMessage)
        {
            try
            {
                SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();
                SqlParameter[] pars = new SqlParameter[]{
                    new SqlParameter("@PackingPlant", strPackingPlant),
                    new SqlParameter("@HosDate",strHosDate),
                    new SqlParameter("@BanZhi",strBanZhi),
                    new SqlParameter("@BZFromTime",strBZFromTime)
                };
                string cmdText = "BSP1306_DataInfo";
                SqlDataAdapter sa = new SqlDataAdapter(cmdText, sqlConnection);
                if (pars != null && pars.Length > 0)
                {
                    foreach (SqlParameter p in pars)
                    {
                        sa.SelectCommand.Parameters.Add(p);
                    }
                }
                sa.SelectCommand.CommandTimeout = 0;
                sa.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                sa.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                DataRow dataRow = dtMessage.NewRow();
                dataRow["vcMessage"] = "数据读取失败！";
                dtMessage.Rows.Add(dataRow);
                throw ex;
            }
        }
        public DataTable getDataInfo(string strPackingPlant, string strHosDate, string strBanZhi, decimal decRest, string strBZFromTime, ref DataTable dtMessage)
        {
            try
            {
                SqlConnection sqlConnection = Common.ComConnectionHelper.CreateSqlConnection();
                SqlParameter[] pars = new SqlParameter[]{
                    new SqlParameter("@PackingPlant", strPackingPlant),
                    new SqlParameter("@HosDate",strHosDate),
                    new SqlParameter("@BanZhi",strBanZhi),
                    new SqlParameter("@Rest",decRest),
                    new SqlParameter("@BZFromTime",strBZFromTime)
                };
                string cmdText = "BSP1306_DataInfo";
                SqlDataAdapter sa = new SqlDataAdapter(cmdText, sqlConnection);
                if (pars != null && pars.Length > 0)
                {
                    foreach (SqlParameter p in pars)
                    {
                        sa.SelectCommand.Parameters.Add(p);
                    }
                }
                sa.SelectCommand.CommandTimeout = 0;
                sa.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                sa.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                DataRow dataRow = dtMessage.NewRow();
                dataRow["vcMessage"] = "数据读取失败！";
                dtMessage.Rows.Add(dataRow);
                throw ex;
            }
        }
    }
}
